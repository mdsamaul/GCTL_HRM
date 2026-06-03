(function ($) {
    $.AttendanceMovementRegisterReportCount = function (options) {
        var settings = $.extend({
            baseUrl: "/",
            companyIds: "#companySelect",
            branchIds: "#branchSelect",
            departmentIds: "#departmentSelect",
            designationIds: "#designationSelect",
            employeeIds: "#employeeSelect",
            FromDateSelect: "#FromDateSelect",
            MonthIDs: "#MonthIds",
            FlatPicker: ".flatDate",
            ToDate: "#ToDateSelect",
            YearIDs: "#YearTo",
        }, options);

        var filterUrl = settings.baseUrl + "/GetFilters";
        var DownloadUrl = settings.baseUrl + "/GetAttendanceMachineData";
        var DownloadExcelUrl = settings.baseUrl + "/AttendanceMovementRegisterReport/ExportAttendanceMovementRegisterExcelAsync";


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

        $(document).on('shown.bs.dropdown', '.btn-group', function () {

            const $group = $(this);

            if (!$group.find('.multiselect-search').length) return;

            setTimeout(function () {
                $group.find('.multiselect-search').focus();
            }, 0);
        });

        function showLoading() {
            $("#customLoadingOverlay").css("display", "flex");
        }

        function hideLoading() {
            $("#customLoadingOverlay").hide();
        }

        $(document).ready(function () {

            function toggleDurationFields() {

                if ($("#Date").is(":checked")) {
                    // Show Date fields
                    $("#FromDateSelect").closest(".col-12").show();
                    $("#ToDateSelect").closest(".col-12").show();

                    // Hide Year fields + reset values
                    $("#MonthIds").closest(".col-12").hide();
                    $("#YearTo").closest(".col-12").hide();
                    $("#MonthIds").val("");
                    $("#YearTo").val("");
                } else if ($("#Year").is(":checked")) {
                    // Show Year fields
                    $("#MonthIds").closest(".col-12").show();
                    $("#YearTo").closest(".col-12").show();

                    // Hide Date fields + reset values
                    $("#FromDateSelect").closest(".col-12").hide();
                    $("#ToDateSelect").closest(".col-12").hide();
                    $("#FromDateSelect").val("");
                    $("#ToDateSelect").val("");

                    var now = new Date();

                    var monthNumber = now.getMonth() + 1;
                    var year = now.getFullYear();


                    $("#MonthIds").val(monthNumber);
                    $("#YearTo").val(year);
                }
            }

            // Initial call
            toggleDurationFields();

            // On radio change
            $("input[name='durationType']").change(function () {
                toggleDurationFields();
            });
        });






        var reportDataTable = null;
       
        $(document).ready(async function () {

            bindRemoteMultiselect("#companySelect", "/GcAccessFilter/companies", "Select Company", "company");
            bindRemoteMultiselect("#branchSelect", "/GcAccessFilter/branches", "Select Branch", "branch");
            bindRemoteMultiselect("#divisionSelect", "/GcAccessFilter/divisions", "Select Division", "division");
            bindRemoteMultiselect("#departmentSelect", "/GcAccessFilter/departments", "Select Department", "department");
            bindRemoteMultiselect("#designationSelect", "/GcAccessFilter/designations", "Select Designation", "designation");
            bindRemoteMultiselect("#employeeSelect", "/GcAccessFilter/employees", "Select Employee", null);

            var accessCode = $("#hdnAccessCode").val();
            var isReadonly = accessCode === "0005";

            ms_InitializeMultiselects(); 
            ms_BindCascade();
            if (isReadonly) {
                ms_InitializeMultiselects(null, null, true); 
                await ms_ApplyAccessCodeToAll(accessCode);   
            } else {
                ms_InitializeMultiselects();               
                ms_BindCascade();
                ms_Reset("#companySelect");
                await ms_LoadNext("#companySelect", "...");
                await ms_AutoSelectCompany("001");
            }

        });

        var GetFlatDate = function () {

            flatpickr($("#ToDateSelect, #FromDateSelect"), CalendarService.createConfig(
                {
                    defaultDate: new Date(),
                }));
        };


        // Filter Value Getter
        // Filter Value Getter
        var getFilterValue = function () {

            const fromDateVal = $(settings.FromDateSelect).val();
            const toDateVal = $(settings.ToDate).val();
            const monthVal = $(settings.MonthIDs).val();
            const yearVal = $(settings.YearIDs).val();

            var filterData = {
                // Dropdown values
                CompanyCodes: toArray($(settings.companyIds).val()),
                BranchCodes: toArray($(settings.branchIds).val()),
                DepartmentCodes: toArray($(settings.departmentIds).val()),
                DesignationCodes: toArray($(settings.designationIds).val()),
                EmployeeIDs: toArray($(settings.employeeIds).val()),

                // Date values (ISO format yyyy-MM-dd)
                FromDate: fromDateVal ? new Date(fromDateVal).toISOString().split('T')[0] : null,
                ToDate: toDateVal ? new Date(toDateVal).toISOString().split('T')[0] : null,

                // Month & Year values (numeric arrays)
                MonthIDs: monthVal ? toArray(monthVal).map(x => parseInt(x, 10)) : [],
                YearIDs: yearVal ? toArray(yearVal).map(x => parseInt(x, 10)) : []
            };

            return filterData;
        };

        // Helper function to normalize values into array
        var toArray = function (value) {
            if (!value) return [];
            if (Array.isArray(value)) return value;
            return [value];
        };
        

        // Fixed download event handler
        $(document).on('click', '#downloadReport', function () {
            var reportValue = $("#reportText").val();
            if (reportValue === "downloadPdf") {
                PdfDownload();
            }
            //else if (reportValue === "downloadWord") {
            //    downloadTableAsWord();
            //}
            else if (reportValue === "downloadExcel") {
                downloadTableAsExcel();
            } else {
                showToast("warning", "Please Select Report Option");
            }
        });


        var PdfDownload = function (isPreview = false) {

            var filterData = getFilterValue();
            // Check selected mode
            const isByDate = $("#Date").is(":checked");
            const isByMonth = $("#Year").is(":checked");

            // Date + Month value presence
            const hasFrom = filterData.FromDate && filterData.FromDate !== "";
            const hasTo = filterData.ToDate && filterData.ToDate !== "";
            const hasMonth = filterData.MonthIDs && filterData.MonthIDs.length > 0;
            const hasYear = filterData.YearIDs && filterData.YearIDs.length > 0;


            // ------------------- By DATE -------------------
            if (isByDate) {

                if (hasFrom && hasTo) {
                    filterData.MonthIDs = [];
                    filterData.YearIDs = [];
                }
                else {
                    alert("Please select both From Date and To Date.");

                    //if (hasFrom && !hasTo) {
                    //    flatpickr("#ToDateSelect").open();
                    //}
                    //else if (!hasFrom && hasTo) {
                    //    flatpickr("#FromDateSelect").open();
                    //}
                    //else {
                    //    flatpickr("#FromDateSelect").open();
                    //}

                    return;
                }
            }

            // ------------------- By MONTH -------------------
            if (isByMonth) {

                // YearIDs must exist
                if (!hasYear) {
                    alert("Please select a Year.");
                    return;
                }

                // Validate year range (YearIDs is array)
                const year = filterData.YearIDs[0]; // assume single selection
                if (year < 2000 || year > 2100) {
                    alert("Invalid Year.");
                    return;
                }

                // Must have both Month & Year
                if (hasMonth && hasYear) {
                    filterData.FromDate = null;
                    filterData.ToDate = null;
                } else {
                    alert("Please select both Month and Year.");
                    return;
                }
            }
            $.ajax({
                url: settings.baseUrl + '/GetAttendanceMachineData',
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(filterData),
                beforeSend: function () {
                    showLoading();
                },
                success: function (response) {                    
                    if (response.isSuccess && response.data) {
                        generatePdf(response.data, filterData, isPreview);
                    } else {
                        alert("No data found to generate PDF");
                    }
                },
                error: function () {
                    alert("Failed to load data for PDF");
                },
                complete: function () {
                    hideLoading();
                }

            });
        };

        function generatePdf(groupedData, filter, isPreview) {
            if (!groupedData || groupedData.length === 0) {
                alert("Data not found");
                return;
            }

            getImageBase64FromUrl('/images/DP_logo.png', function (base64Logo) {
                const { jsPDF } = window.jspdf;
                const doc = new jsPDF({ orientation: 'portrait', unit: 'pt', format: 'a4' });
             
                const pageWidth = doc.internal.pageSize.getWidth();
                const pageHeight = doc.internal.pageSize.getHeight();
                const leftMargin = 20;
                const rightMargin = 20;
                const contentWidth = pageWidth - leftMargin - rightMargin;

                const companyName = groupedData[0]?.companyName || "Company Name";
                const companyAddress = groupedData[0]?.companyAddress || "";
                const fromDate = filter.FromDate ? new Date(filter.FromDate).toLocaleDateString('en-GB') : "";
                const toDate = filter.ToDate ? new Date(filter.ToDate).toLocaleDateString('en-GB') : "";

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

                let totalRecords = 0;

                
                function drawPageHeader(doc) {
                    let y = 20;

                    // Add logo on the left side
                    if (base64Logo) {
                        const logoWidth = 60;  // Adjust size as needed
                        const logoHeight = 30; // Adjust size as needed
                        doc.addImage(base64Logo, 'PNG', leftMargin, y-5, logoWidth, logoHeight);
                    }

                    doc.setFontSize(14); doc.setFont("times", "bold");
                    doc.text(companyName, pageWidth / 2, y, { align: 'center' });

                    doc.setFontSize(10); doc.setFont("times", "normal"); y += 15;
                    doc.text(companyAddress, pageWidth / 2, y, { align: 'center' });

                    doc.setFontSize(12); doc.setFont("times", "normal"); y += 15;
                    doc.text("Attendance Movement Register Report Count", pageWidth / 2, y, { align: 'center' });

                    return y + 10;
                }
              
                function drawDepartmentHeader(doc, deptName, yPos) {
                    doc.setFontSize(10);
                    doc.setFont("times", "bold");

                    const deptLines = doc.splitTextToSize(`Department: ${deptName || ''}`, contentWidth - 100);
                    deptLines.forEach((line, index) => {
                        doc.text(line, leftMargin, yPos + (index * 12));
                    });

                    const deptHeight = deptLines.length * 12;
                    return yPos + deptHeight;
                }

                let firstTable = true;

                groupedData.forEach(function (dept) {
                    let currentDate_str = dept.date ? new Date(dept.date).toLocaleDateString('en-GB') :
                        (fromDate && toDate ? (fromDate === toDate ? fromDate : `${fromDate} - ${toDate}`) : '');

                    let tableData = [];
                    dept.employees.forEach(function (emp, index) {
                        totalRecords++;
                        tableData.push([
                            emp.sn || (index + 1),
                            emp.employeeID || "",
                            emp.fullName || "",
                            emp.designationName || "",
                            emp.branchName || "",
                            emp.date ? new Date(emp.date).toLocaleDateString('en-GB') : "",   
                            emp.inOutCount || 0,                                             
                            {
                                content: 'View',
                                link: emp.viewLink,
                                styles: { textColor: [0, 0, 255], fontStyle: 'bold', halign: 'center' }
                            }
                        ]);
                    });


                    let startY;
                    if (firstTable) {
                        let headerY = drawPageHeader(doc);
                        startY = drawDepartmentHeader(doc, dept.departmentName, headerY + 10);
                        startY += 2;
                        firstTable = false;
                    } else {
                        startY = doc.lastAutoTable.finalY + 20;
                        if (startY + 80 > pageHeight - 60) {
                            doc.addPage();
                            let headerY = drawPageHeader(doc);
                            startY = drawDepartmentHeader(doc, dept.departmentName, headerY + 10);
                            startY += 2;
                        } else {
                            startY = drawDepartmentHeader(doc, dept.departmentName, startY);
                            startY += 2;
                        }
                    }


                    doc.autoTable({
                        head: [['SN', 'Employee ID', 'Name', 'Designation', 'Branch', 'Date', 'Count', 'View Details']],
                        body: tableData,
                        startY: startY,
                        theme: 'grid',
                        margin: { top: 120, left: leftMargin, right: rightMargin, bottom: 40 },
                        styles: {
                            fontSize: 9, cellPadding: 3, textColor: [0, 0, 0], lineColor: [0, 0, 0], lineWidth: 0.1, halign: 'center',  
                            valign: 'middle',
},
                        headStyles: {
                            fillColor: false, fontStyle: 'bold', halign: 'center', textColor: [0, 0, 0], lineColor: [0, 0, 0], halign: 'center',  
                            valign: 'middle',
},
                        columnStyles: {
                            0: { halign: 'center', cellWidth: 20 },
                            1: { halign: 'center', cellWidth: 75 },
                            2: { halign: 'left', cellWidth: contentWidth * 0.25 },
                            3: { halign: 'left', cellWidth: contentWidth * 0.2 },
                            4: { halign: 'center', cellWidth: 60 },
                            5: { halign: 'center', cellWidth: 55 },   
                            6: { halign: 'center', cellWidth: 40 },   
                            7: { halign: 'center', cellWidth: 50 }
                        },
                        didDrawCell: function (data) {
                            try {
                                if (data.section === 'body' && data.column.index === 7) {
                                    const row = tableData[data.row.index];
                                    if (!row) return;
                                    const linkData = row[7];
                                    if (!linkData || !linkData.link) return;

                                    doc.link(
                                        data.cell.x,
                                        data.cell.y,
                                        data.cell.width,
                                        data.cell.height,
                                        { url: linkData.link }
                                    );
                                }
                            } catch (err) {
                                console.error('didDrawCell error:', err);
                            }
                        },
                        pageBreak: 'auto',
                        didDrawPage: function (data) { drawPageHeader(doc); }
                    });
                });



                let finalY = doc.lastAutoTable.finalY || 200;
                if (finalY + 30 > pageHeight - 40) { doc.addPage(); finalY = 120; }

                doc.setFontSize(10); doc.setTextColor(0, 0, 0); doc.setFont("times", "bold");
                doc.text('Total Records: ' + totalRecords, leftMargin, finalY + 20);

                const totalPages = doc.getNumberOfPages();
                for (let i = 1; i <= totalPages; i++) {
                    doc.setPage(i);
                    doc.setFontSize(8); doc.setTextColor(50, 50, 50); doc.setFont("times", "normal");
                    doc.text('Printed: ' + currentDate, leftMargin, pageHeight - 20);
                    doc.text('Page ' + i + ' of ' + totalPages, pageWidth - rightMargin - 40, pageHeight - 20);
                }

                if (isPreview) {
                    const pdfBlob = doc.output('blob');
                    const pdfUrl = URL.createObjectURL(pdfBlob);
                    const previewContainer = document.getElementById("pdf-preview-container");
                    previewContainer.style.display = "block"; previewContainer.innerHTML = "";
                    const iframe = document.createElement("iframe");
                    iframe.style.width = "100%"; iframe.style.height = "100%"; iframe.src = pdfUrl;
                    previewContainer.appendChild(iframe);
                } else {
                    const timestamp = new Date().getTime();
                    doc.save(`AttendanceMovementRegisterCount.pdf`);
                }
            });
        }
















        var PdfPreview = function () {
            PdfDownload(true);
        };
        $('#btnPreviewPdf').on('click', function () {
            PdfPreview(); //  PdfDownload(true);
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
                callback(null);
            };
            img.src = url;
        }


        var downloadTableAsExcel = function () {


            var filterData = getFilterValue();
            // Check selected mode
            const isByDate = $("#Date").is(":checked");
            const isByMonth = $("#Year").is(":checked");

            // Date + Month value presence
            const hasFrom = filterData.FromDate && filterData.FromDate !== "";
            const hasTo = filterData.ToDate && filterData.ToDate !== "";
            const hasMonth = filterData.MonthIDs && filterData.MonthIDs.length > 0;
            const hasYear = filterData.YearIDs && filterData.YearIDs.length > 0;


            // ------------------- By DATE -------------------
            if (isByDate) {

                if (hasFrom && hasTo) {
                    filterData.MonthIDs = [];
                    filterData.YearIDs = [];
                }
                else {
                    alert("Please select both From Date and To Date.");

                    //if (hasFrom && !hasTo) {
                    //    flatpickr("#ToDateSelect").open();
                    //}
                    //else if (!hasFrom && hasTo) {
                    //    flatpickr("#FromDateSelect").open();
                    //}
                    //else {
                    //    flatpickr("#FromDateSelect").open();
                    //}

                    return;
                }
            }

            // ------------------- By MONTH -------------------
            if (isByMonth) {

                // YearIDs must exist
                if (!hasYear) {
                    alert("Please select a Year.");
                    return;
                }

                // Validate year range (YearIDs is array)
                const year = filterData.YearIDs[0]; // assume single selection
                if (year < 2000 || year > 2100) {
                    alert("Invalid Year.");
                    return;
                }

                // Must have both Month & Year
                if (hasMonth && hasYear) {
                    filterData.FromDate = null;
                    filterData.ToDate = null;
                } else {
                    alert("Please select both Month and Year.");
                    return;
                }
            }







            $.ajax({
                url: settings.baseUrl + '/ExcelDownload',
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(filterData),
                xhrFields: { responseType: "blob" },
                beforeSend: function () {
                    showLoading();
                },
                success: function (res, status, xhr) {
                    var contentType = xhr.getResponseHeader("Content-Type");

                    // Check if it's JSON error response
                    if (contentType && contentType.includes("application/json")) {
                        var reader = new FileReader();
                        reader.onload = function () {
                            var error = JSON.parse(reader.result);
                            alert(error.message || "No data to export");
                        };
                        reader.readAsText(res);
                        return;
                    }

                    // Download Excel file
                    var link = document.createElement("a");
                    link.href = URL.createObjectURL(res);
                    link.download = "MovementRegister_" + new Date().getTime() + ".xlsx";
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                },
                error: function (xhr) {
                    console.error("Error:", xhr);
                    if (xhr.response instanceof Blob) {
                        var reader = new FileReader();
                        reader.onload = function () {
                            try {
                                var error = JSON.parse(reader.result);
                                alert(error.message || "Excel download failed");
                            } catch (e) {
                                alert("Excel download failed");
                            }
                        };
                        reader.readAsText(xhr.response);
                    } else {
                        alert("Excel download failed");
                    }
                },
                complete: function () {
                    hideLoading();
                }
            });
        }

        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
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
