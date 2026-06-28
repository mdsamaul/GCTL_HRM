(function ($) {
    $.attendanceMachineData = function (options) {
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
        var samaul = settings.baseUrl + "/PagedLookup";
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



        $(document).on('shown.bs.dropdown', '.btn-group', function () {

            const $group = $(this);

            if (!$group.find('.multiselect-search').length) return;

            setTimeout(function () {
                $group.find('.multiselect-search').focus();
            }, 0);
        });




        var reportDataTable = null;
       

        var GetFlatDate = function () {
           
            flatpickr($("#ToDateSelect, #FromDateSelect"), CalendarService.createConfig(
                {
                    defaultDate: new Date(),
                }));
        };

        //$(document).ready(async function () {

        //    gcBindRemoteMultiselect("#companySelect", "/GcFilters/company", "Select Company");
        //    gcBindRemoteMultiselect("#branchSelect", "/GcFilters/branch", "Select Branch");
        //    gcBindRemoteMultiselect("#divisionSelect", "/GcFilters/division", "Select Division");
        //    gcBindRemoteMultiselect("#departmentSelect", "/GcFilters/department", "Select Department");
        //    gcBindRemoteMultiselect("#designationSelect", "/GcFilters/designation", "Select Designation");
        //    gcBindRemoteMultiselect("#employeeSelect", "/GcFilters/employee", "Select Employee");


        //    bsms_InitializeMultiselects();
        //    bsms_BindCascade();
        //    bsms_Reset("#companySelect");
        //    await bsms_LoadNext("#companySelect", "/GcFilters/company");
        //    await bsms_AutoSelectCompany("001");

        //});

        //$(document).ready(async function () {
        //    gcBindRemoteMultiselect("#companySelect", "/GcAccessFilter/companies", "Select Company");
        //    gcBindRemoteMultiselect("#branchSelect", "/GcAccessFilter/branches", "Select Branch");
        //    gcBindRemoteMultiselect("#divisionSelect", "/GcAccessFilter/divisions", "Select Division");
        //    gcBindRemoteMultiselect("#departmentSelect", "/GcAccessFilter/departments", "Select Department");
        //    gcBindRemoteMultiselect("#designationSelect", "/GcAccessFilter/designations", "Select Designation");
        //    gcBindRemoteMultiselect("#employeeSelect", "/GcAccessFilter/employees", "Select Employee");

        //    bsms_InitializeMultiselects();
        //    bsms_BindCascade();
        //    bsms_Reset("#companySelect");
        //    await bsms_LoadNext("#companySelect", "/GcAccessFilter/companies");
        //    await bsms_AutoSelectCompany("001");
        //});
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
                url: '/AttendanceMovementRegisterReport/GetAttendanceMachineData',
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

            getImageBase64FromUrl('/images/DP_logo.png', function (base64Logo, natW, natH) {

               
                var targetH = 20;
                var logoDrawWidth = (natW && natH) ? (natW / natH) * targetH : 60;
                var logoDrawHeight = natH ? targetH : 30;

                const { jsPDF } = window.jspdf;
                const doc = new jsPDF({ orientation: 'portrait', unit: 'pt', format: 'a4' });

                const pageWidth = doc.internal.pageSize.getWidth();
                const pageHeight = doc.internal.pageSize.getHeight();
                const leftMargin = 10;
                const rightMargin = 10;
                const contentWidth = pageWidth - leftMargin - rightMargin;

                const companyName = groupedData[0]?.employees[0]?.companyName || "Company Name";
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

                // ✅ drawHeader — শুধু draw করে, কোনো async নেই
                function drawHeader(doc) {
                    if (base64Logo) {
                        doc.addImage(base64Logo, 'PNG', leftMargin, 8, logoDrawWidth, logoDrawHeight);
                        //                                          ↑y=8, height=25 → logo শেষ হয় y=33 এ
                    }
                    doc.setFontSize(16);
                    doc.setFont("times", "bold");
                    doc.text(companyName, pageWidth / 2, 22, { align: 'center' });  // 22

                    doc.setFontSize(12);
                    doc.setFont("times", "semibold");
                    doc.text("Attendance Movement Register Report", pageWidth / 2, 36, { align: 'center' }); // 36

                    const lineLength = contentWidth / 3;
                    const startX = (pageWidth - lineLength) / 2;
                    const endX = startX + lineLength;
                    doc.setDrawColor(0);
                    doc.setLineWidth(0.5);
                    doc.line(startX, 40, endX, 40); // 40

                    doc.setFontSize(9);
                    doc.setFont("times", "normal");

                    if (fromDate && toDate) {
                        if (fromDate === toDate) {
                            doc.text(fromDate, pageWidth / 2, 50, { align: 'center' }); // 50
                        } else {
                            doc.text("Date: " + fromDate + " - " + toDate, pageWidth / 2, 50, { align: 'center' });
                        }
                    } else if (filter.MonthIDs && filter.YearIDs) {
                        const monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
                        const monthId = filter.MonthIDs[0];
                        const yearId = filter.YearIDs[0];
                        if (monthId && yearId) {
                            doc.text(monthNames[monthId - 1] + ", " + yearId, pageWidth / 2, 50, { align: 'center' });
                        }
                    }
                }

                drawHeader(doc);

                let startY = 65;
                const allLocationLinks = [];

                groupedData.forEach(function (dept, deptIndex) {
                    if (deptIndex > 0) startY += 15;

                    doc.setFontSize(11);
                    doc.setFont("times", "bold");
                    doc.text("Department: " + dept.departmentName, leftMargin, startY);
                    startY += 5;

                    let tableData = [];
                    dept.employees.forEach(function (emp, index) {
                        totalRecords++;
                        let dateStr = emp.date ? new Date(emp.date).toLocaleDateString('en-GB') : "";
                        let timeStr = "";
                        if (emp.time) {
                            let timeObj = new Date(emp.time);
                            let h = timeObj.getHours();
                            let m = String(timeObj.getMinutes()).padStart(2, '0');
                            let s = String(timeObj.getSeconds()).padStart(2, '0');
                            let ap = h >= 12 ? 'PM' : 'AM';
                            h = h % 12 || 12;
                            h = String(h).padStart(2, '0');
                            timeStr = `${h}:${m}:${s} ${ap}`;
                        }

                        const locationText = (emp.latitude && emp.longitude)
                            ? `https://www.google.com/maps/dir/?api=1&destination=${emp.latitude},${emp.longitude}&travelmode=driving`
                            : "";
                        allLocationLinks.push(locationText || "");

                        tableData.push([
                            index + 1,
                            emp.employeeID || "",
                            emp.fullName || "",
                            emp.designationName || "",
                            emp.branchName || "",
                            dateStr,
                            timeStr,
                            emp.machineId || "",
                            locationText ? "View Location" : ""
                        ]);
                    });

                    const deptStartIndex = allLocationLinks.length - tableData.length;

                    doc.autoTable({
                        head: [['SN', 'Employee ID', 'Name', 'Designation', 'Branch', 'Date', 'Time', 'Machine', 'Location']],
                        body: tableData,
                        startY: startY,
                        theme: 'grid',
                        margin: { top: 65, left: leftMargin, right: rightMargin }, 
                        styles: { fontSize: 7, cellPadding: 2, lineColor: [200, 200, 200], lineWidth: 0.1, textColor: [0, 0, 0] },
                        headStyles: { fillColor: [211, 211, 211], textColor: [0, 0, 0], fontStyle: 'bold', lineColor: [180, 180, 180], lineWidth: 0.1, halign: 'center', valign: 'middle' },
                        columnStyles: {
                            0: { cellWidth: 22, halign: 'center', valign: 'middle' },
                            1: { cellWidth: 50, halign: 'center', valign: 'middle' },
                            2: { cellWidth: 100, halign: 'left', valign: 'middle' },
                            3: { cellWidth: 70, halign: 'left', valign: 'middle' },
                            4: { cellWidth: 60, halign: 'center', valign: 'middle' },
                            5: { cellWidth: 50, halign: 'center', valign: 'middle' },
                            6: { cellWidth: 55, halign: 'center', valign: 'middle' },
                            7: { cellWidth: 60, halign: 'center', valign: 'middle' },
                            8: { cellWidth: 'auto', halign: 'center', valign: 'middle', fontSize: 6 },
                        },
                        pageBreak: 'auto',
                        didDrawPage: function (data) {
                            drawHeader(doc);
                            const pageNumber = doc.internal.getCurrentPageInfo().pageNumber;
                            const totalPages = '{total_pages_count_string}';
                            doc.setFontSize(8); doc.setTextColor(50, 50, 50); doc.setFont("times", "normal");
                            doc.text('Print: ' + currentDate, leftMargin, pageHeight - 20);
                            doc.text('Page ' + pageNumber + ' of ' + totalPages, pageWidth + 70, pageHeight - 20, { align: 'right' });
                        },
                        didDrawCell: function (data) {
                            if (data.column.index === 8 && data.cell.section === 'body' && data.cell.raw === "View Location") {
                                const url = allLocationLinks[deptStartIndex + data.row.index];
                                if (url) {
                                    doc.setFillColor(255, 255, 255);
                                    doc.rect(data.cell.x + 1, data.cell.y + 1, data.cell.width - 2, data.cell.height - 2, 'F');
                                    doc.setFontSize(7); doc.setTextColor(0, 102, 204);
                                    doc.textWithLink("View Location", data.cell.x + data.cell.width / 2, data.cell.y + data.cell.height / 2 + 2, { url: url, align: 'center' });
                                    doc.setTextColor(0, 0, 0);
                                }
                            }
                        }
                    });

                    startY = doc.lastAutoTable.finalY;
                });

                if (typeof doc.putTotalPages === 'function') doc.putTotalPages('{total_pages_count_string}');

                let finalY = doc.lastAutoTable.finalY || 200;
                if (finalY + 20 > pageHeight - 30) { doc.addPage(); drawHeader(doc); finalY = 90; }

                doc.setFontSize(9); doc.setTextColor(0, 0, 0); doc.setFont("times", "bold");
                doc.text('Total Records: ' + totalRecords, leftMargin, finalY + 15);

                if (isPreview) {
                    const pdfBlob = doc.output('blob');
                    const pdfUrl = URL.createObjectURL(pdfBlob);
                    const previewContainer = document.getElementById("pdf-preview-container");
                    previewContainer.style.display = "block";
                    previewContainer.innerHTML = "";
                    const iframe = document.createElement("iframe");
                    iframe.style.width = "100%";
                    iframe.style.height = "100%";
                    iframe.src = pdfUrl;
                    previewContainer.appendChild(iframe);
                } else {
                    doc.save('AttendanceMovementRegister.pdf');
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
                // ✅ base64 এর সাথে natural size ও পাঠাচ্ছি
                callback(dataURL, img.naturalWidth, img.naturalHeight);
            };
            img.onerror = function () { callback(null, 0, 0); };
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
                url: '/AttendanceMovementRegisterReport/ExcelDownload',
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
            setupLoadingOverlay();
        };
        init();
    };
})(jQuery);
