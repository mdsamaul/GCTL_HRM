(function ($) {
    $.patientTypes = function (options) {
        var settings = $.extend({
            baseUrl: "/",
            companyIds: "#companySelect",
            branchIds: "#branchSelect",
            departmentIds: "#departmentSelect",
            designationIds: "#designationSelect",
            employeeIds: "#employeeSelect",
            FromDate: "#FromDateSelect",
            FlatPicker: ".flatDate",
            ToDate: "#ToDateSelect",         
        }, options);

        var filterUrl = settings.baseUrl + "/getAllFilterEmp";
        var DownloadUrl = settings.baseUrl + "/getAllPdfFilterEmp";

        var setupLoadingOverlay = function () {
            if ($("#customLoadingOverlay").length === 0) {
                $("body").append(`
                    <div id="customLoadingOverlay" style="
                        display: none;
                        position: fixed;
                        top: 0;
                        left: 0;
                        width: 100%;
                        height: 100%;
                        background-color: rgba(0, 0, 0, 0.5);
                        z-index: 9999;
                        justify-content: center;
                        align-items: center;">
                        <div style="
                            background-color: white;
                            padding: 20px;
                            border-radius: 5px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.3);
                            text-align: center;">
                            <div class="spinner-border text-primary" role="status">
                                <span class="sr-only">Loading...</span>
                            </div>
                            <p style="margin-top: 10px; margin-bottom: 0;">Loading data...</p>
                        </div>
                    </div>
                `);
            }
        };

        function showLoading() {
            $("#customLoadingOverlay").css("display", "flex");
        }

        function hideLoading() {
            $("#customLoadingOverlay").hide();
        }
        var reportDataTable = null;
        

        var GetFlatDate = function () {
            flatpickr($(settings.FlatPicker), CalendarService.createConfig(
                {
                    defaultDate: new Date(),
                }
            ));
        };

         


        // Filter Value Getter
        var getFilterValue = function () {
            const fromDateVal = $(settings.FromDate).val();
            const toDateVal = $(settings.ToDate).val();
            var filterData = {
                CompanyCodes: toArray($(settings.companyIds).val()),
                BranchCodes: toArray($(settings.branchIds).val()),
                DepartmentCodes: toArray($(settings.departmentIds).val()),
                DesignationCodes: toArray($(settings.designationIds).val()),
                EmployeeIDs: toArray($(settings.employeeIds).val()),
                FromDate: fromDateVal ? new Date(fromDateVal).toISOString().split('T')[0] : null,
                ToDate: toDateVal ? new Date(toDateVal).toISOString().split('T')[0] : null
            };
            return filterData;
        };
        var toArray = function (value) {
            if (!value) return [];
            if (Array.isArray(value)) return value;
            return [value];
        };


        $(document).ready(async function () {
            gcBindRemoteMultiselect("#companySelect", "/GcFilters/company", "Select Company");
            gcBindRemoteMultiselect("#branchSelect", "/GcFilters/branch", "Select Branch");
            gcBindRemoteMultiselect("#divisionSelect", "/GcFilters/division", "Select Division");
            gcBindRemoteMultiselect("#departmentSelect", "/GcFilters/department", "Select Department");
            gcBindRemoteMultiselect("#designationSelect", "/GcFilters/designation", "Select Designation");
            gcBindRemoteMultiselect("#employeeSelect", "/GcFilters/employee", "Select Employee");

            bsms_InitializeMultiselects();
            bsms_BindCascade();
            bsms_Reset("#companySelect");
            await bsms_LoadNext("#companySelect", "/GcFilters/company");
            await bsms_AutoSelectCompany("001");


            //$("#companySelect, #branchSelect, #divisionSelect, #departmentSelect, #designationSelect, #employeeSelect, #activityStatusSelect, #ToDateFilter, #FromDateFilter")
            //    .on("change", function () {
            //        currentPage = 1;
            //        loadGridData();
            //    });
        });



        // Fixed download event handler
        $(document).on('click', '#downloadReport', function () {
            var reportValue = $("#reportText").val();
            if (reportValue === "downloadPdf") {
                PdfDownload();
            } else if (reportValue === "downloadWord") {
                downloadTableAsWord();
            } else if (reportValue === "downloadExcel") {
                downloadTableAsExcel();
            } else {
                showToast("warning", "Please Select Report Option");
            }
        });

        function LoadFullRosterTable(callback) {
            showLoading();
            var filterData = getFilterValue();
            //console.log(filterData);
            $.ajax({
                url: DownloadUrl,
                type: 'POST',
                data: JSON.stringify(filterData),
                contentType: 'application/json',
                success: function (res) {
                    var tableBody = $('#RosterScheduleReport-grid tbody');
                    tableBody.empty();
                    hideLoading();
                    if (res.data && res.data.employees && res.data.employees.length > 0) {
                        if (callback && typeof callback === 'function') {
                            callback(res.data.employees); 
                        }
                    } else {
                        alert('No data found to download');
                        hideLoading();
                    }
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data');
                }
            });
        }


        var PdfDownload = function () {
            LoadFullRosterTable(function (employees) {              

                getImageBase64FromUrl('/images/DP_logo.png', function (base64Logo) {
                    const { jsPDF } = window.jspdf;
                    const doc = new jsPDF({
                        orientation: 'landscape',
                        unit: 'pt',
                        format: [842, 595]
                    });

                    const pageWidth = doc.internal.pageSize.getWidth();
                    const companyName = employees[0].companyName || "";
                    const fromDate = employees[0].fromDate || "";
                    const toDate = employees[0].toDate || "";
                    const now = new Date();

                    const day = String(now.getDate()).padStart(2, '0');
                    const month = String(now.getMonth() + 1).padStart(2, '0');
                    const year = now.getFullYear();

                    let hours = now.getHours();
                    const minutes = String(now.getMinutes()).padStart(2, '0');
                    const seconds = String(now.getSeconds()).padStart(2, '0');
                    const ampm = hours >= 12 ? 'PM' : 'AM';
                    hours = hours % 12 || 12;
                    hours = String(hours).padStart(2, '0');
                    const currentDate = `${day}/${month}/${year} ${hours}:${minutes}:${seconds} ${ampm}`;

                    let TotalEmpCount = 0;

                    function drawHeader(doc) {
                        if (base64Logo) {
                            doc.addImage(base64Logo, 'PNG', 15, 10, 80, 50); 
                        }

                        // Company Name
                        doc.setFontSize(18);
                        doc.setFont("times", "bold");
                        doc.text(companyName, pageWidth / 2, 40, { align: 'center' });

                        doc.setFontSize(13);
                        doc.setFont("times", "semibold");
                        doc.text("Roster Schedule Report", pageWidth / 2, 58, { align: 'center' });

                        const lineLength = pageWidth / 6.8;
                        const startX = (pageWidth - lineLength) / 2;
                        const endX = startX + lineLength;
                        doc.setDrawColor(0);
                        doc.setLineWidth(0.5);
                        doc.line(startX, 63, endX, 63);

                        doc.setFontSize(10);
                        doc.setFont("times", "normal");
                        const fromToText = "Date: " + fromDate + "-" + toDate;
                        doc.text(fromToText, pageWidth / 2, 75, { align: 'center' });
                    }

                    drawHeader(doc);

                    let startY = 95;

                    let departmentGroups = {};
                    employees.forEach(function (emp) {
                        let dept = emp.departmentName || "Unknown";
                        if (!departmentGroups[dept]) {
                            departmentGroups[dept] = [];
                        }
                        departmentGroups[dept].push(emp);
                    });

                    for (const dept in departmentGroups) {

                        const estimatedHeaderHeight = 25;
                        const estimatedRowHeight = 15;
                        const pageHeight = doc.internal.pageSize.getHeight();

                        if (startY + estimatedHeaderHeight + estimatedRowHeight > pageHeight - 40) {
                            doc.addPage();
                            drawHeader(doc);
                            startY = 95;
                        }

                        doc.setFontSize(14);
                        doc.setFont("times", "bold");
                        doc.text("Department: " + dept, 20, startY);
                        startY += 10;

                        let tempTable = $('<table>');
                        tempTable.append($('#RosterScheduleReport-grid thead').clone());
                        let tbody = $('<tbody>');

                        departmentGroups[dept].forEach(function (emp, index) {
                            TotalEmpCount++;
                            let row = $('<tr>');
                            row.append('<td>' + (index + 1) + '</td>');
                            row.append('<td>' + emp.code + '</td>');
                            row.append('<td>' + emp.name + '</td>');
                            row.append('<td>' + emp.designationName + '</td>');
                            row.append('<td>' + emp.branchName + '</td>');
                            row.append('<td>' + emp.showDate + '</td>');
                            row.append('<td>' + emp.dayName + '</td>');
                            row.append('<td>' + emp.shiftName + '</td>');
                            row.append('<td>' + emp.remark + '</td>');
                            row.append('<td>' + emp.approvalStatus + '</td>');
                            row.append('<td>' + emp.approvedBy + '</td>');
                            row.append('<td>' + emp.showApprovalDatetime + '</td>');
                            tbody.append(row);
                        });

                        tempTable.append(tbody);
                        doc.autoTable({
                            html: tempTable[0],
                            startY: startY,
                            theme: 'grid',
                            margin: { top: 110, left: 15, right: 15 },

                            styles: {
                                fontSize: 8,
                                cellPadding: 2,
                                textColor: [0, 0, 0],
                                lineColor: [0, 0, 0],
                                lineWidth: 0.3
                            },

                            headStyles: {
                                fillColor: [255, 255, 255],
                                textColor: [0, 0, 0],
                                fontStyle: 'bold',
                                lineColor: [0, 0, 0],
                                lineWidth: 0.3,
                                halign: 'center',
                                valign: 'middle'
                            },
                            columnStyles: {
                                0: { cellWidth: 20, halign: 'center', valign: 'middle' },
                                1: { cellWidth: 60, halign: 'center', valign: 'middle' },
                                2: { cellWidth: 80, halign: 'left', valign: 'middle' },
                                3: { cellWidth: 70, halign: 'left', valign: 'middle' },
                                4: { cellWidth: 70, halign: 'left', valign: 'middle' },
                                5: { cellWidth: 50, halign: 'center', valign: 'middle' },
                                6: { cellWidth: 50, halign: 'center', valign: 'middle' },
                                7: { cellWidth: 125, halign: 'left', valign: 'middle' },
                                8: { cellWidth: 75, halign: 'center', valign: 'middle' },
                                9: { cellWidth: 50, halign: 'center', valign: 'middle' },
                                10: { cellWidth: 80, halign: 'center', valign: 'middle' },
                                11: { cellWidth: 80, halign: 'center', valign: 'middle' },
                            },

                            didDrawPage: function (data) {
                                drawHeader(doc);

                                const pageNumber = doc.internal.getCurrentPageInfo().pageNumber;
                                const totalPages = '{total_pages_count_string}';
                                const pageSize = doc.internal.pageSize;
                                const pageHeight = pageSize.height || pageSize.getHeight();

                                // 🔻 Footer
                                doc.setFontSize(10);
                                doc.setTextColor(50, 50, 50);
                                doc.setFont("times", "normal");

                                doc.text('Print Datetime: ' + currentDate, 15, pageHeight - 10);
                                doc.text(
                                    'Page ' + pageNumber + ' of ' + totalPages,
                                    pageWidth + 85,
                                    pageHeight - 10,
                                    { align: 'right' }
                                );

                                if (data.pageNumber > 1) {
                                    doc.setFontSize(14);
                                    doc.setFont("times", "bold");
                                    doc.text("Department: " + dept, 20, 95);
                                }
                            }

                        });


                        startY = doc.lastAutoTable.finalY + 15;
                    }

                    if (typeof doc.putTotalPages === 'function') {
                        doc.putTotalPages('{total_pages_count_string}');
                    }

                    let finalY = doc.lastAutoTable.finalY || 270;
                    let pageHeight = doc.internal.pageSize.getHeight();

                    if (finalY + 20 > pageHeight - 20) {
                        doc.addPage();
                        drawHeader(doc);
                        finalY = 100;
                    }

                    doc.setFontSize(10);
                    doc.setTextColor(0, 0, 0);
                    doc.text('Total Employee : ' + TotalEmpCount, 15, finalY + 20);

                    doc.save('RosterScheduleReport.pdf');
                });
            })
        };

        $('#btnPreviewPdf').on('click', function () {
            PdfPreview();
        });
             


        function getImageBase64FromUrl(url, callback) {
            var img = new Image();
            img.crossOrigin = 'Anonymous';
            img.onload = function () {
                var canvas = document.createElement('canvas');
                canvas.width = img.width;
                canvas.height = img.height;
                var ctx = canvas.getContext('2d');
                ctx.drawImage(img, 0, 0);
                var dataURL = canvas.toDataURL('image/png');
                callback(dataURL);
            };
            img.onerror = function () {
                //console.error("Image not found or CORS issue.");
                callback(null);
            };
            img.src = url;
        }


        var PdfPreview = function () {
            LoadFullRosterTable(function (employees) {
                if (!employees || employees.length === 0) {
                    alert("No data found.");
                    return;
                }

                getImageBase64FromUrl('/images/DP_logo.png', function (base64Logo) {
                    const { jsPDF } = window.jspdf;
                    const doc = new jsPDF({
                        orientation: 'landscape',
                        unit: 'pt',
                        format: [842, 595]
                    });

                    const pageWidth = doc.internal.pageSize.getWidth();
                    const companyName = employees[0].companyName || "";
                    const fromDate = employees[0].fromDate || "";
                    const toDate = employees[0].toDate || "";
                    const now = new Date();

                    const day = String(now.getDate()).padStart(2, '0');
                    const month = String(now.getMonth() + 1).padStart(2, '0');
                    const year = now.getFullYear();

                    let hours = now.getHours();
                    const minutes = String(now.getMinutes()).padStart(2, '0');
                    const seconds = String(now.getSeconds()).padStart(2, '0');
                    const ampm = hours >= 12 ? 'PM' : 'AM';
                    hours = hours % 12 || 12;
                    hours = String(hours).padStart(2, '0');
                    const currentDate = `${day}/${month}/${year} ${hours}:${minutes}:${seconds} ${ampm}`;

                    let TotalEmpCount = 0;

                    function drawHeader(doc) {
                        // Logo (if available)
                        if (base64Logo) {
                            doc.addImage(base64Logo, 'PNG', 15, 10, 80, 50);
                        }

                        // Company Name
                        doc.setFontSize(18);
                        doc.setFont("times", "bold");
                        doc.text(companyName, pageWidth / 2, 40, { align: 'center' });

                        doc.setFontSize(13);
                        doc.setFont("times", "normal");
                        doc.text("Roster Schedule Report", pageWidth / 2, 58, { align: 'center' });

                        const lineLength = pageWidth / 6.8;
                        const startX = (pageWidth - lineLength) / 2;
                        const endX = startX + lineLength;
                        doc.setDrawColor(0);
                        doc.setLineWidth(0.5);
                        doc.line(startX, 63, endX, 63);

                        doc.setFontSize(10);
                        doc.setFont("times", "normal");
                        const fromToText = "Date: " + fromDate + "-" + toDate;
                        doc.text(fromToText, pageWidth / 2, 75, { align: 'center' });
                    }

                    drawHeader(doc);

                    let startY = 95;

                    let departmentGroups = {};
                    employees.forEach(function (emp) {
                        let dept = emp.departmentName || "Unknown";
                        if (!departmentGroups[dept]) {
                            departmentGroups[dept] = [];
                        }
                        departmentGroups[dept].push(emp);
                    });


                    for (const dept in departmentGroups) {

                        const estimatedHeaderHeight = 25;
                        const estimatedRowHeight = 15;  
                        const pageHeight = doc.internal.pageSize.getHeight();

                        if (startY + estimatedHeaderHeight + estimatedRowHeight > pageHeight - 40) {
                            doc.addPage();
                            drawHeader(doc);
                            startY = 95;
                        }

                        doc.setFontSize(14);
                        doc.setFont("times", "bold");
                        doc.text("Department: " + dept, 20, startY);
                        startY += 10;

                        let tempTable = $('<table>');
                        tempTable.append($('#RosterScheduleReport-grid thead').clone());
                        let tbody = $('<tbody>');

                        departmentGroups[dept].forEach(function (emp, index) {
                            TotalEmpCount++;
                            let row = $('<tr>');
                            row.append('<td>' + (index + 1) + '</td>');
                            row.append('<td>' + emp.code + '</td>');
                            row.append('<td>' + emp.name + '</td>');
                            row.append('<td>' + emp.designationName + '</td>');
                            row.append('<td>' + emp.branchName + '</td>');
                            row.append('<td>' + emp.showDate + '</td>');
                            row.append('<td>' + emp.dayName + '</td>');
                            row.append('<td>' + emp.shiftName + '</td>');
                            row.append('<td>' + emp.remark + '</td>');
                            row.append('<td>' + emp.approvalStatus + '</td>');
                            row.append('<td>' + emp.approvedBy + '</td>');
                            row.append('<td>' + emp.showApprovalDatetime + '</td>');
                            tbody.append(row);
                        });

                        tempTable.append(tbody);
                        doc.autoTable({
                            html: tempTable[0],
                            startY: startY,
                            theme: 'grid',
                            margin: { top: 110, left: 15, right: 15 },

                            styles: {
                                fontSize: 8,
                                cellPadding: 2,
                                textColor: [0, 0, 0],
                                lineColor: [0, 0, 0],   
                                lineWidth: 0.3
                            },

                            headStyles: {
                                fillColor: [255, 255, 255], 
                                textColor: [0, 0, 0],       
                                fontStyle: 'bold',
                                lineColor: [0, 0, 0],     
                                lineWidth: 0.3,
                                halign: 'center',
                                valign: 'middle'
                            },
                            columnStyles: {
                            0: { cellWidth: 20, halign: 'center', valign: 'middle' },
                            1: { cellWidth: 60, halign: 'center', valign: 'middle' },
                            2: { cellWidth: 80, halign: 'left', valign: 'middle' },
                            3: { cellWidth: 70, halign: 'left', valign: 'middle' },
                            4: { cellWidth: 70, halign: 'left', valign: 'middle' },
                            5: { cellWidth: 50, halign: 'center', valign: 'middle' },
                            6: { cellWidth: 50, halign: 'center', valign: 'middle' },
                            7: { cellWidth: 125, halign: 'left', valign: 'middle' },
                            8: { cellWidth: 75, halign: 'center', valign: 'middle' },
                            9: { cellWidth: 50, halign: 'center', valign: 'middle' },
                            10: { cellWidth: 80, halign: 'center', valign: 'middle' },
                            11: { cellWidth: 80, halign: 'center', valign: 'middle' },
                        },

                            didDrawPage: function (data) {
                                drawHeader(doc);

                                const pageNumber = doc.internal.getCurrentPageInfo().pageNumber;
                                const totalPages = '{total_pages_count_string}';
                                const pageSize = doc.internal.pageSize;
                                const pageHeight = pageSize.height || pageSize.getHeight();

                                // 🔻 Footer
                                doc.setFontSize(10);
                                doc.setTextColor(50, 50, 50);
                                doc.setFont("times", "normal");

                                doc.text('Print Datetime: ' + currentDate, 15, pageHeight - 10);
                                doc.text(
                                    'Page ' + pageNumber + ' of ' + totalPages,
                                    pageWidth + 85,
                                    pageHeight - 10,
                                    { align: 'right' }
                                );

                                // 🔻 Department name ONLY if table continues
                                if (data.pageNumber > 1) {
                                    doc.setFontSize(14);
                                    doc.setFont("times", "bold");
                                    doc.text("Department: " + dept, 20, 95);
                                }
                            }

                        });


                        startY = doc.lastAutoTable.finalY + 15;
                    }


                    if (typeof doc.putTotalPages === 'function') {
                        doc.putTotalPages('{total_pages_count_string}');
                    }

                    let finalY = doc.lastAutoTable.finalY || 270;
                    let pageHeight = doc.internal.pageSize.getHeight();

                    if (finalY + 20 > pageHeight - 20) {
                        doc.addPage();
                        drawHeader(doc);
                        finalY = 100;
                    }

                    doc.setFontSize(10);
                    doc.setTextColor(0, 0, 0);
                    doc.text('Total Employee : ' + TotalEmpCount, 15, finalY + 20);

                    const blob = doc.output('blob');
                    const url = URL.createObjectURL(blob);

                    $('#pdf-preview-container').html(`<iframe src="${url}" width="100%" height="100%" style="border:1px solid #ccc;"></iframe>`);
                    $('#pdf-preview-container').show();
                });
            });
        };

        var downloadTableAsWord = function () {
            LoadFullRosterTable(function (employees) {
                if (!employees || employees.length === 0) {
                    alert("No data found!");
                    return;
                }
                let departmentGroups = {};
                employees.forEach(function (emp) {
                    let dept = emp.departmentName || "Unknown";
                    if (!departmentGroups[dept]) {
                        departmentGroups[dept] = [];
                    }
                    departmentGroups[dept].push(emp);
                });

                var companyName = employees[0].companyName || "";
                var reportTitle = "Employee Roster Report";
                var fromDate = employees[0].fromDate || "";
                var toDate = employees[0].toDate || "";               

                // With this:
                var currentDate = new Date().toLocaleString('en-US', {
                    year: 'numeric', month: 'short', day: 'numeric',
                    hour: 'numeric', minute: 'numeric', hour12: true
                });

                var userName = employees[0].luser || "";

                const columnWidths = ["50px", "100px", "200px", "150px", "100px", "80px", "100px", "150px", "100px", "100px", "100px", "100px"];

                var header = "<!DOCTYPE html>" +
                    "<html xmlns:v='urn:schemas-microsoft-com:vml' " +
                    "xmlns:o='urn:schemas-microsoft-com:office:office' " +
                    "xmlns:w='urn:schemas-microsoft-com:office:word' " +
                    "xmlns:m='http://schemas.microsoft.com/office/2004/12/omml' " +
                    "xmlns='http://www.w3.org/TR/REC-html40'>" +
                    "<head>" +
                    "<meta charset='utf-8'>" +
                    "<title>" + reportTitle + "</title>" +
                    "<!--[if gte mso 9]>" +
                    "<xml><w:WordDocument><w:View>Print</w:View><w:Zoom>90</w:Zoom></w:WordDocument></xml>" +
                    "<![endif]-->" +
                    "<style>" +
                    "@page Section1 { size: 842pt 595pt; mso-page-orientation: landscape; margin: 0.5in; } " +
                    "div.Section1 { page: Section1; } " +
                    "body { font-family: 'Times New Roman', serif; margin: 0; padding: 0; } " +
                    ".header { text-align: center; margin-top: 10px; font-size: 20px; font-weight: bold; } " +
                    ".sub-header { text-align: center; font-size: 14px; margin-top: 5px; } " +
                    "h2 { font-size: 16px; font-weight: bold; margin: 20px 0 10px; color: #333; padding-bottom: 5px; display: inline-block; } " +
                    "table { border-collapse: collapse; width: 100%; margin: 0; } " +
                    "table, th, td { border: 1px solid black; padding: 0; margin: 0; font-size: 10px; vertical-align: top; line-height: 1; } " +
                    "th { background-color: #ffffff; font-weight: bold; text-align: center; } " +
                    "tr { height: auto; } " +
                    "td { height: auto; } " +
                    ".footer { margin-top: 20px; font-size: 12px; } " +
                    ".page-info { text-align: left; display: inline-block; width: 50%; } " +
                    ".date-user { text-align: right; display: inline-block; width: 49%; } " +
                    "</style>" +
                    "</head>" +
                    "<body><div class='Section1'>" +
                    "<div class='header'>" + companyName + "</div>" +
                    "<div class='sub-header'>" + reportTitle + "</div>" +
                    "<div class='sub-header' style='font-size: 10px;'>Date: " + fromDate + " - " + toDate + "</div>";

                var originalTable = document.getElementById("RosterScheduleReport-grid");
                var headerRow = '<tr>';
                var headers = originalTable ? originalTable.querySelectorAll('thead th') : [];
                if (headers.length > 0) {
                    headers.forEach(function (header, index) {
                        var width = columnWidths[index] || "100px";
                        headerRow += '<th style="width:' + width + '; padding: 0; margin: 0;">' + (header.innerText || header.textContent || '') + '</th>';
                    });
                } else {
                    const defaultHeaders = ["SN", "Code", "Name", "Designation", "Branch", "Date", "Day", "Shift", "Remark", "Approval Status", "Approved By", "Approval DateTime"];
                    defaultHeaders.forEach(function (header, index) {
                        var width = columnWidths[index] || "100px";
                        headerRow += '<th style="width:' + width + '; padding: 0; margin: 0;">' + header + '</th>';
                    });
                }
                headerRow += '</tr>';

                var content = '';
                for (const dept in departmentGroups) {
                    content += '<h2>Department: ' + dept + '</h2>';
                    content += '<table>' + headerRow;

                    departmentGroups[dept].forEach(function (emp, index) {
                        content += '<tr>';
                        content += '<td style="text-align: center;">' + (index + 1) + '</td>';
                        content += '<td style="text-align: center;">' + (emp.code || '') + '</td>';
                        content += '<td style="padding-left: 5px;">' + (emp.name || '') + '</td>';
                        content += '<td style="padding-left: 5px;">' + (emp.designationName || '') + '</td>';
                        content += '<td style="text-align: center;">' + (emp.branchName || '') + '</td>';
                        content += '<td style="text-align: center;">' + (emp.showDate || '') + '</td>';
                        content += '<td style="text-align: center;">' + (emp.dayName || '') + '</td>';
                        content += '<td style="padding-left: 5px;">' + (emp.shiftName || '') + '</td>';
                        content += '<td style="word-wrap: break-word; padding-left: 5px;">' + (emp.remark || '') + '</td>';
                        content += '<td style="text-align: center;">' + (emp.approvalStatus || '') + '</td>';
                        content += '<td style="text-align: center;">' + (emp.approvedBy || '') + '</td>';
                        content += '<td style="text-align: center;">' + (emp.showApprovalDatetime || '') + '</td>';
                        content += '</tr>';
                    });

                    content += '</table>';
                }

                var footer = "<div class='footer'>" +
                    "<div class='page-info'>Page: 1 of 1</div>" +
                    "<div class='date-user'>Generated on: " + currentDate + " | By: " + userName + "</div>" +
                    "</div>" +
                    "</div></body></html>";

                var sourceHTML = header + content + footer;
                var source = 'data:application/vnd.ms-word;charset=utf-8,' + encodeURIComponent(sourceHTML);
                var fileDownload = document.createElement("a");
                document.body.appendChild(fileDownload);
                fileDownload.href = source;
                fileDownload.download = 'RosterScheduleReport.doc';
                fileDownload.click();
                document.body.removeChild(fileDownload);
            });
        };


        var downloadTableAsExcel = function () {
            LoadFullRosterTable(function (employees) {
                if (!employees || employees.length === 0) {
                    alert("No data found!");
                    return;
                }
                $.ajax({
                    url: "/RosterScheduleReport/DownloadExcel",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(employees),
                    xhrFields: { responseType: "blob" },
                    success: function (res) {
                        var link = document.createElement("a");
                        link.href = URL.createObjectURL(res);
                        link.download = "RosterScheduleReport.xlsx";
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                    },
                    error: function (e) {
                        //console.log(e.message);
                        //showToast("error", "Excel download failed!");
                    }
                });
            });
        }

        $(document).on('shown.bs.dropdown', '.btn-group', function () {

            const $group = $(this);

            if (!$group.find('.multiselect-search').length) return;

            setTimeout(function () {
                $group.find('.multiselect-search').focus();
            }, 0);
        });



        var init = function () {
            GetFlatDate();
            settings.load(); 
            //initializeMultiselects();
            setupLoadingOverlay();
            //loadFilterEmp();    
        };
        init();
    };
})(jQuery);
