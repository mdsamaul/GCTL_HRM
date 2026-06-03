(function ($) {
    $.patientTypes = function (options) {
        var settings = $.extend({
            baseUrl: "/",
            companyIds: "#companySelect",
            branchIds: "#branchSelect",
            departmentIds: "#departmentSelect",
            designationIds: "#designationSelect",
            employeeIds: "#employeeSelect",
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

        $(document).on('shown.bs.dropdown', '.btn-group', function () {

            const $group = $(this);

            if (!$group.find('.multiselect-search').length) return;

            setTimeout(function () {
                $group.find('.multiselect-search').focus();
            }, 0);
        });

        var reportDataTable = null;
       
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

        function LoadFullPFAssignTable(callback) {
            showLoading();
            var filterData = getFilterValue();
            //console.log(filterData);
            $.ajax({
                url: DownloadUrl,
                type: 'POST',
                data: JSON.stringify(filterData),
                contentType: 'application/json',
                success: function (res) {
                    var tableBody = $('#employee-pfAssign-grid-report tbody');
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
            LoadFullPFAssignTable(function (employees) {
                if (!employees || employees.length === 0) {
                    alert("No data found.");
                    return;
                }
                //console.log(employees);
                getImageBase64FromUrl('/images/DP_logo.png', function (base64Logo) {
                    const { jsPDF } = window.jspdf;
                    const doc = new jsPDF({
                        orientation: 'portrait',
                        unit: 'pt',
                        format: [842, 595]
                    });                   
                        const pageWidth = doc.internal.pageSize.getWidth();
                        const companyName = employees[0].company || "";
                        const fromDate = employees[0].fromDate || "";
                        const toDate = employees[0].toDate || "";
                        //const userName = employees[0].luser || "";
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
                        //const Address = 'Corner Glaze, Block#L, Road#8, Plot#2490/B, Bashundhara Residential Area, Dhaka-1229';

                        let TotalEmpCount = 0;

                        function drawHeader(doc) {
                            // Logo (if available)
                            if (base64Logo) {
                                doc.addImage(base64Logo, 'PNG', 15, 10, 80, 50);
                            }

                            // Company Name
                            doc.setFontSize(18);
                            doc.setFont("times", "bold");
                            doc.text(companyName, pageWidth / 2, 45, { align: 'center' });

                            doc.setFontSize(13);
                            doc.setFont("times", "normal");
                            doc.text("PF Assign Report", pageWidth / 2, 60, { align: 'center' });

                            const lineLength = pageWidth / 6.5;
                            const startX = (pageWidth - lineLength) / 2;
                            const endX = startX + lineLength;
                            doc.setDrawColor(0);
                            doc.setLineWidth(0.5);
                            doc.line(startX, 65, endX, 65);

                            //doc.setFontSize(8);
                            //doc.setFont("times", "normal");
                            //const fromToText = Address;
                            //doc.text(fromToText, pageWidth / 2, 58, { align: 'center' });
                        }

                        drawHeader(doc);

                        let startY = 85;

                        let departmentGroups = {};
                        employees.forEach(function (emp) {
                            let dept = emp.department || "Unknown";
                            if (!departmentGroups[dept]) {
                                departmentGroups[dept] = [];
                            }
                            departmentGroups[dept].push(emp);
                        });

                        for (const dept in departmentGroups) {
                            if (startY !== 85) startY += 20;

                            doc.setFontSize(12);
                            doc.setFont("times", "bold");
                            doc.text("Department: " + dept, 20, startY);
                            startY += 6;

                            let tempTable = $('<table>');
                            tempTable.append($('#employee-pfAssign-grid-report thead').clone());
                            let tbody = $('<tbody>');

                            departmentGroups[dept].forEach(function (emp, index) {
                                TotalEmpCount++;
                                let row = $('<tr>');
                                row.append('<td>' + (index + 1) + '</td>');
                                row.append('<td>' + emp.code + '</td>');
                                row.append('<td>' + emp.name + '</td>');
                                row.append('<td>' + emp.designation + '</td>');
                                row.append('<td>' + emp.branch + '</td>');
                                row.append('<td>' + emp.pfApprove + '</td>');
                                row.append('<td>' + emp.showDate + '</td>');
                                //row.append('<td>' + emp.dayName + '</td>');
                                row.append('<td>' + emp.remarks + '</td>');
                                tbody.append(row);
                            });

                            tempTable.append(tbody);
                            doc.autoTable({
                                head: [['SN', 'Employee ID', 'Name', 'Designation', 'Branch', 'PF Approval', 'Effective Date', 'Remarks']],
                                body: departmentGroups[dept].map((emp, index) => [
                                    index + 1,
                                    emp.code,
                                    emp.name,
                                    emp.designation,
                                    emp.branch,
                                    emp.pfApprove,
                                    emp.showDate,
                                    //emp.dayName,
                                    emp.remarks
                                ]),
                                startY: startY,
                                theme: 'grid',
                                tableWidth: 'auto', // or 'wrap' depending on fit
                                margin: { top: 100, left: 15, right: 15 },
                                styles: {
                                    fontSize: 8,
                                    cellPadding: 2,
                                    cellWidth: 'wrap',
                                    lineColor: [0, 0, 0],
                                    lineWidth: 0.1,
                                    textColor: [0, 0, 0],
                                },
                                headStyles: {
                                    fillColor: [255, 255, 255],
                                    textColor: [0, 0, 0],
                                    fontStyle: 'bold',
                                    halign: 'center',
                                    valign: 'middle',
                                    lineWidth: 0.1,
                                    lineColor: [0, 0, 0],
                                },
                                columnStyles: {
                                    0: { cellWidth: 20, halign: 'center' },
                                    1: { cellWidth: 60, halign: 'center' },
                                    2: { cellWidth: 90 },
                                    3: { cellWidth: 90 },
                                    4: { cellWidth: 70, halign: 'left' },
                                    5: { cellWidth: 70, halign: 'center' },
                                    6: { cellWidth: 60, halign: 'center' },
                                    7: { cellWidth: 105, halign: 'left' },
                                    //8: { cellWidth: 70 }                      
                                },
                                pageBreak: 'auto',
                                didDrawPage: function (data) {
                                    drawHeader(doc);
                                    const pageNumber = doc.internal.getCurrentPageInfo().pageNumber;
                                    const totalPages = '{total_pages_count_string}';
                                    const pageHeight = doc.internal.pageSize.getHeight();

                                    let leftText = 'Print Datetime: ' + currentDate;
                                    let rightText = 'Page ' + pageNumber + ' of ' + totalPages;

                                    doc.setFontSize(10);
                                    doc.setTextColor(50, 50, 50);
                                    doc.setFont("times", "normal");

                                    doc.text(leftText, 15, pageHeight - 10);
                                    doc.text(rightText, pageWidth + 85, pageHeight - 10, { align: 'right' });
                                }
                            });

                            startY = doc.lastAutoTable.finalY;
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
                   
                    doc.save('Employee pfAssign Report.pdf');
                });
            });
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
            LoadFullPFAssignTable(function (employees) {
                if (!employees || employees.length === 0) {
                    alert("No data found.");
                    return;
                }
                //console.log(employees);
                getImageBase64FromUrl('/images/DP_logo.png', function (base64Logo) {
                    const { jsPDF } = window.jspdf;
                    const doc = new jsPDF({
                        orientation: 'portrait',
                        unit: 'pt',
                        format: 'a4'
                    });


                    const pageWidth = doc.internal.pageSize.getWidth();
                    const companyName = employees[0].company || "";
                    const fromDate = employees[0].fromDate || "";
                    const toDate = employees[0].toDate || "";
                    //const userName = employees[0].luser || "";
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
                    //const Address = 'Corner Glaze, Block#L, Road#8, Plot#2490/B, Bashundhara Residential Area, Dhaka-1229';

                    let TotalEmpCount = 0;

                    function drawHeader(doc) {
                        // Logo (if available)
                        if (base64Logo) {
                            doc.addImage(base64Logo, 'PNG', 15, 10, 80, 50);
                        }

                        // Company Name
                        doc.setFontSize(18);
                        doc.setFont("times", "bold");
                        doc.text(companyName, pageWidth / 2, 45, { align: 'center' });

                        doc.setFontSize(13);
                        doc.setFont("times", "normal");
                        doc.text("PF Assign Report", pageWidth / 2, 60, { align: 'center' });

                        const lineLength = pageWidth / 6.5;
                        const startX = (pageWidth - lineLength) / 2;
                        const endX = startX + lineLength;
                        doc.setDrawColor(0);
                        doc.setLineWidth(0.5);
                        doc.line(startX, 65, endX, 65);

                        //doc.setFontSize(8);
                        //doc.setFont("times", "normal");
                        //const fromToText = Address;
                        //doc.text(fromToText, pageWidth / 2, 58, { align: 'center' });
                    }

                    drawHeader(doc);

                    let startY = 85;

                    let departmentGroups = {};
                    employees.forEach(function (emp) {
                        let dept = emp.department || "Unknown";
                        if (!departmentGroups[dept]) {
                            departmentGroups[dept] = [];
                        }
                        departmentGroups[dept].push(emp);
                    });

                    for (const dept in departmentGroups) {
                        if (startY !== 85) startY += 20;

                        doc.setFontSize(12);
                        doc.setFont("times", "bold");
                        doc.text("Department: " + dept, 20, startY);
                        startY += 6;

                        let tempTable = $('<table>');
                        tempTable.append($('#employee-pfAssign-grid-report thead').clone());
                        let tbody = $('<tbody>');

                        departmentGroups[dept].forEach(function (emp, index) {
                            TotalEmpCount++;
                            let row = $('<tr>');
                            row.append('<td>' + (index + 1) + '</td>');
                            row.append('<td>' + emp.code + '</td>');
                            row.append('<td>' + emp.name + '</td>');
                            row.append('<td>' + emp.designation + '</td>');
                            row.append('<td>' + emp.branch+ '</td>');
                            row.append('<td>' + emp.pfApprove + '</td>');
                            row.append('<td>' + emp.showDate + '</td>');
                            //row.append('<td>' + emp.dayName + '</td>');
                            row.append('<td>' + emp.remarks + '</td>');
                            tbody.append(row);
                        });

                        tempTable.append(tbody);
                        doc.autoTable({
                            head: [['SN', 'Employee ID', 'Name', 'Designation', 'Branch', 'PF Approval', 'Effective Date', 'Remarks']],
                            body: departmentGroups[dept].map((emp, index) => [
                                index + 1,
                                emp.code,
                                emp.name,
                                emp.designation,
                                emp.branch,
                                emp.pfApprove,
                                emp.showDate,
                                //emp.dayName,
                                emp.remarks
                            ]),
                            startY: startY,
                            theme: 'grid',
                            tableWidth: 'auto', // or 'wrap' depending on fit
                            margin: { top: 100, left: 15, right: 15 },
                            styles: {
                                fontSize: 8,
                                cellPadding: 2,
                                cellWidth: 'wrap',
                                lineColor: [0, 0, 0],
                                lineWidth: 0.1,
                                textColor: [0, 0, 0],
                            },
                            headStyles: {
                                fillColor: [255, 255, 255],
                                textColor: [0, 0, 0],
                                fontStyle: 'bold',
                                halign: 'center',
                                valign: 'middle',
                                lineWidth: 0.1,
                                lineColor: [0, 0, 0],
                            },
                            columnStyles: {
                                0: { cellWidth: 20, halign: 'center' },    
                                1: { cellWidth: 60, halign: 'center' },   
                                2: { cellWidth: 90 },                      
                                3: { cellWidth: 90 },                      
                                4: { cellWidth: 70, halign: 'left' },   
                                5: { cellWidth: 70, halign: 'center' },  
                                6: { cellWidth: 60, halign: 'center' },   
                                7: { cellWidth: 105, halign: 'left' },    
                                //8: { cellWidth: 70 }                      
                            },
                            pageBreak: 'auto',
                            didDrawPage: function (data) {
                                drawHeader(doc);
                                const pageNumber = doc.internal.getCurrentPageInfo().pageNumber;
                                const totalPages = '{total_pages_count_string}';
                                const pageHeight = doc.internal.pageSize.getHeight();

                                let leftText = 'Print Datetime: ' + currentDate;
                                let rightText = 'Page ' + pageNumber + ' of ' + totalPages;

                                doc.setFontSize(10);
                                doc.setTextColor(50, 50, 50);
                                doc.setFont("times", "normal");

                                doc.text(leftText, 15, pageHeight - 10);
                                doc.text(rightText, pageWidth + 85, pageHeight - 10, { align: 'right' });
                            }
                        });

                        startY = doc.lastAutoTable.finalY;
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

                    $('#pdf-preview-container').html(`<iframe src="${url}" width="100%" height="600px" style="border:1px solid #ccc;"></iframe>`);
                    $('#pdf-preview-container').show();
                });
            });
        };      

        var downloadTableAsWord = function () {
            LoadFullPFAssignTable(function (employees) {
                if (!employees || employees.length === 0) {
                    alert("No data found!");
                    return;
                }

                let departmentGroups = {};
                employees.forEach(function (emp) {
                    let dept = emp.department || "Unknown";
                    if (!departmentGroups[dept]) {
                        departmentGroups[dept] = [];
                    }
                    departmentGroups[dept].push(emp);
                });

                var companyName = employees[0].company || "";
                var reportTitle = "PF Assign Report";
               
                var currentDate = new Date().toLocaleString('en-US', {
                    year: 'numeric', month: 'short', day: 'numeric',
                    hour: 'numeric', minute: 'numeric', hour12: true
                });
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
                //"@page Section1 { size: 842pt 595pt; mso-page-orientation: landscape; margin: 0.5in; } " +
                    "@page Section1 { size: 595.3pt 841.9pt; mso-page-orientation: portrait; margin: 0.5in; } " +
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
                    "<div class='sub-header'>" + reportTitle + "</div>";

                // Column widths (same for all tables)
                const columnWidths = ["20px", "70px", "120px", "100px", "90px", "60px", "70px", "150px"];

                // Table header
                var headerRow = '<tr>';
                var headerTitles = ["SN", "Employee ID", "Name", "Designation", "Branch", "PF Approval", "Effective Date", "Remarks"];
                headerTitles.forEach(function (title, index) {
                    headerRow += '<th style="width:' + columnWidths[index] + '; padding:0; margin:0;">' + title + '</th>';
                });
                headerRow += '</tr>';

                var content = '';
                for (const dept in departmentGroups) {
                    content += '<h2>Department: ' + dept + '</h2>';
                    content += '<table>' + headerRow;

                    departmentGroups[dept].forEach(function (emp, index) {
                        content += '<tr>';
                        content += '<td style="width:' + columnWidths[0] + '; text-align:center;">' + (index + 1) + '</td>';
                        content += '<td style="width:' + columnWidths[1] + '; text-align:center;">' + (emp.code || '') + '</td>';
                        content += '<td style="width:' + columnWidths[2] + '; padding-left:5px;">' + (emp.name || '') + '</td>';
                        content += '<td style="width:' + columnWidths[3] + '; padding-left:5px;">' + (emp.designation || '') + '</td>';
                        content += '<td style="width:' + columnWidths[4] + '; padding-left:5px;">' + (emp.branch || '') + '</td>';
                        content += '<td style="width:' + columnWidths[5] + '; text-align:center;">' + (emp.pfApprove || '') + '</td>';
                        content += '<td style="width:' + columnWidths[6] + '; text-align:center;">' + (emp.showDate || '') + '</td>';
                        content += '<td style="width:' + columnWidths[7] + '; padding-left:5px;">' + (emp.remarks || '') + '</td>';
                        content += '</tr>';
                    });

                    content += '</table>';
                }

                var footer = "<div class='footer'>" +
                    "<div class='page-info'>Page: 1 of 1</div>" +
                    "<div class='date-user'>Generated on: " + currentDate + "</div>" +
                    "</div>" +
                    "</div></body></html>";

                var sourceHTML = header + content + footer;
                var source = 'data:application/vnd.ms-word;charset=utf-8,' + encodeURIComponent(sourceHTML);
                var fileDownload = document.createElement("a");
                document.body.appendChild(fileDownload);
                fileDownload.href = source;
                fileDownload.download = 'PF Assign Report.doc';
                fileDownload.click();
                document.body.removeChild(fileDownload);
            });
        };

        
        var downloadTableAsExcel = function () {
            LoadFullPFAssignTable(function (employees) {
                if (!employees || employees.length === 0) {
                    alert("No data found!");
                    return;
                }
                //console.log(employees);
                $.ajax({
                    url: "/PFAssignEntryReport/DownloadExcel",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(employees),
                    xhrFields: { responseType: "blob" },
                    success: function (res) {
                        var link = document.createElement("a");
                        link.href = URL.createObjectURL(res);
                        link.download = "Employee pfAssign Report.xlsx";
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


        var init = function () {
            showLoading();
            //GetFlatDate();
            settings.load();
            setupLoadingOverlay();
            //loadFilterEmp();
        };
        init();
    };
})(jQuery);
