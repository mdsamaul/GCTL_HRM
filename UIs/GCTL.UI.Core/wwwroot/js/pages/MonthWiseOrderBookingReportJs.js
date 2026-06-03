(function ($) {
    $.MonthWiseOrderBookingReportJs = function (options) {
        var settings = $.extend({
            baseUrl: "/",
            companyIds: "#companySelect",
            branchIds: "#branchSelect",
            departmentIds: "#departmentSelect",
            buyerIdSelect: "#buyerIdSelect",
            styleIds: "#styleSelect",
        }, options);

        // -------------------- Loading Overlay --------------------
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

        // -------------------- Multi-select Initialization --------------------
        var initializeMultiselects = function () {
            var selectors = [
                settings.companyIds,
                settings.branchIds,
                settings.departmentIds,
                settings.buyerIdSelect,
                settings.styleIds
            ].join(", ");

            $(selectors).multiselect({
                enableFiltering: true,
                includeSelectAllOption: true,
                selectAllText: 'Select All',
                nonSelectedText: '--Select Buyer--',
                nSelectedText: 'Selected',
                allSelectedText: 'All Selected',
                filterPlaceholder: 'Search.......',
                buttonWidth: '100%',
                maxHeight: 350,
                enableClickableOptGroups: true,
                dropUp: false,
                numberDisplayed: 1,
                enableCaseInsensitiveFiltering: true
            });
        };

        // =======================
        // Initialize Flatpickr for date inputs
        // =======================
        var initializeFlatDates = function () {
            flatpickr($('.flatDate'), CalendarService.createConfig({
                dateFormat: "Y-m-d",      // Backend format yyyy-MM-dd
                altInput: true,           // Show user-friendly format
                altFormat: "d/m/Y",       // Display format for users
                allowInput: true,
                defaultDate: "today",
                onReady: function (selectedDates, dateStr, instance) {
                    instance.input.placeholder = "dd/mm/yyyy";
                }
            }));
        };

        // =======================
        // Document Ready
        // =======================
        $(document).ready(function () {
            initializeFlatDates();

            // Toggle Date/Year inputs based on radio selection
            function toggleInputs() {
                if ($('#Date').is(':checked')) {
                    $('#FromDateSelect, #ToDateSelect')
                        .prop('disabled', false)
                        .closest('.col-12').show();

                    $('#YearFrom, #YearTo')
                        .prop('disabled', true)
                        .closest('.col-12').hide();
                } else if ($('#Year').is(':checked')) {
                    $('#FromDateSelect, #ToDateSelect')
                        .prop('disabled', true)
                        .closest('.col-12').hide();

                    $('#YearFrom, #YearTo')
                        .prop('disabled', false)
                        .closest('.col-12').show();
                }
            }

            toggleInputs();
            $('input[name="durationType"]').change(toggleInputs);
        });

        // =======================
        // Get selected filters
        // =======================
        function getFilteredValues() {
            function getMultiSelectValues(selector) {
                let val = $(selector).val();
                return val && val.length > 0 ? val : [];
            }

            return {
                FromDate: null,
                ToDate: null,
                FromYear: null,
                ToYear: null,
                BuyerIds: getMultiSelectValues("#buyerIdSelect"),
                StyleIds: getMultiSelectValues("#styleSelect")
            };
        }

        // =======================
        // Format date to yyyy-MM-dd
        // =======================
        function formatDateForBackend(dateStr) {
            if (!dateStr) return null;
            let date = new Date(dateStr);
            let month = (date.getMonth() + 1).toString().padStart(2, '0');
            let day = date.getDate().toString().padStart(2, '0');
            return `${date.getFullYear()}-${month}-${day}`;
        }

        // =======================
        // Download Month Wise Order All Style Excel
        // =======================
        function GetOrderReportAllStyleDownloadExcelReport() {
            var request = getFilteredValues();

            // Date or Year filter
            if ($('#Date').is(':checked')) {
                request.FromDate = formatDateForBackend($('#FromDateSelect').val());
                request.ToDate = formatDateForBackend($('#ToDateSelect').val());
            } else if ($('#Year').is(':checked')) {
                request.FromYear = parseInt($('#YearFrom').val()) || null;
                request.ToYear = parseInt($('#YearTo').val()) || null;
            }

            
            $.ajax({
                url: '/MonthWiseOrderBookingReport/DownloadOrderAllStyleReport',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                xhrFields: { responseType: 'blob' },
                beforeSend: showLoading,
                success: function (blob) {
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = 'MonthWiseOrderReport_AllStyle_' + new Date().getTime() + '.xlsx';
                    link.click();
                    window.URL.revokeObjectURL(link.href);
                },
                error: function (xhr, status, error) {
                    alert("Failed to download report");
                },
                complete: hideLoading
            });
        }

        // =======================
        // Download Month Wise Order Style Excel (Optional separate report)
        // =======================
        function GetOrderReportStyleDownloadExcelReport() {
            var request = getFilteredValues();

            if ($('#Date').is(':checked')) {
                request.FromDate = formatDateForBackend($('#FromDateSelect').val());
                request.ToDate = formatDateForBackend($('#ToDateSelect').val());
            } else if ($('#Year').is(':checked')) {
                request.FromYear = parseInt($('#YearFrom').val()) || null;
                request.ToYear = parseInt($('#YearTo').val()) || null;
            }
           
            
            $.ajax({
                url: '/MonthWiseOrderBookingReport/DownloadOrderStyleReport',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                xhrFields: { responseType: 'blob' },
                beforeSend: showLoading,
                success: function (blob) {
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = 'MonthWiseOrderReport_Style_' + new Date().getTime() + '.xlsx';
                    link.click();
                    window.URL.revokeObjectURL(link.href);
                },
                error: function (xhr, status, error) {
                    alert("Failed to download report");
                },
                complete: hideLoading
            });
        }

        



        // =======================
        // Download Month Wise Order Style Excel
        // =======================
        function GetOrderReportStyleDownloadExcelReport() {
            var request = getFilteredValues();

            if ($('#Date').is(':checked')) {
                request.FromDate = formatDateForBackend($('#FromDateSelect').val());
                request.ToDate = formatDateForBackend($('#ToDateSelect').val());
            } else if ($('#Year').is(':checked')) {
                request.FromYear = parseInt($('#YearFrom').val()) || null;
                request.ToYear = parseInt($('#YearTo').val()) || null;
            }
            
            
            $.ajax({
                url: '/MonthWiseOrderBookingReport/DownloadOrderStyleReport',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                xhrFields: { responseType: 'blob' },
                beforeSend: showLoading,
                success: function (blob) {
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = 'MonthWiseOrderReport_Style_' + new Date().getTime() + '.xlsx';
                    link.click();
                    window.URL.revokeObjectURL(link.href);
                },
                error: function (xhr, status, error) {
                    
                    alert("Failed to download report");
                },
                complete: hideLoading
            });
        }
        // =======================
        // Download Month Wise Order Style Excel
        // =======================
        function GetOrderReportStylePoDownloadExcelReport() {
            var request = getFilteredValues();

            if ($('#Date').is(':checked')) {
                request.FromDate = formatDateForBackend($('#FromDateSelect').val());
                request.ToDate = formatDateForBackend($('#ToDateSelect').val());
            } else if ($('#Year').is(':checked')) {
                request.FromYear = parseInt($('#YearFrom').val()) || null;
                request.ToYear = parseInt($('#YearTo').val()) || null;
            }
            
            
            $.ajax({
                url: '/MonthWiseOrderBookingReport/DownloadOrderStylePoReport',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                xhrFields: { responseType: 'blob' },
                beforeSend: showLoading,
                success: function (blob) {
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = 'MonthWiseOrderReport_Style_' + new Date().getTime() + '.xlsx';
                    link.click();
                    window.URL.revokeObjectURL(link.href);
                },
                error: function (xhr, status, error) {
                    
                    alert("Failed to download report");
                },
                complete: hideLoading
            });
        }


        // =======================
        // Download Month Wise Order Style Excel
        // =======================
        function GetOrderReportStylePoCSDownloadExcelReport() {
            var request = getFilteredValues();

            if ($('#Date').is(':checked')) {
                request.FromDate = formatDateForBackend($('#FromDateSelect').val());
                request.ToDate = formatDateForBackend($('#ToDateSelect').val());
            } else if ($('#Year').is(':checked')) {
                request.FromYear = parseInt($('#YearFrom').val()) || null;
                request.ToYear = parseInt($('#YearTo').val()) || null;
            }
            
            
            $.ajax({
                url: '/MonthWiseOrderBookingReport/DownloadOrderStylePoCSReport',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                xhrFields: { responseType: 'blob' },
                beforeSend: showLoading,
                success: function (blob) {
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = 'MonthWiseOrderReport_Style_' + new Date().getTime() + '.xlsx';
                    link.click();
                    window.URL.revokeObjectURL(link.href);
                },
                error: function (xhr, status, error) {
                    
                    alert("Failed to download report");
                },
                complete: hideLoading
            });
        }


        //pdf


        /* --------------------------------------------------------------
   PDF Generation – Month-wise Order Booking (All Styles)
   -------------------------------------------------------------- */
        function GetOrderReportAllStyleDownloadPdf() {
            const request = getFilteredValues();

            if ($('#Date').is(':checked')) {
                request.fromDate = formatDateForBackend($('#FromDateSelect').val());
                request.toDate = formatDateForBackend($('#ToDateSelect').val());
            } else if ($('#Year').is(':checked')) {
                request.fromYear = parseInt($('#YearFrom').val(), 10) || null;
                request.toYear = parseInt($('#YearTo').val(), 10) || null;
            }

            $.ajax({
                url: '/MonthWiseOrderBookingReport/DownloadOrderAllStylePdfReport',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                dataType: 'json',
                beforeSend: showLoading,
                success: function (reportData) {
                   
                    generateMonthWiseOrderPdf(reportData);
                },
                error: function (xhr, status, err) {
                    
                    alert('Failed to generate PDF');
                },
                complete: hideLoading
            });
        }

        /* --------------------------------------------------------------
           Core PDF builder
           -------------------------------------------------------------- */
        function generateMonthWiseOrderPdf(reportData) {
            if (reportData?.data.length == 0) {
                alert("No  data found");
                return;
            }

            const { jsPDF } = window.jspdf;

            const margin = 15;
            const doc = new jsPDF({ orientation: "landscape", unit: "pt", format: "a4" });
            const pageWidth = doc.internal.pageSize.getWidth();
            const pageHeight = doc.internal.pageSize.getHeight();
            const availableWidth = pageWidth - margin * 2;

            const baseCol = { slNo: 28, buyerName: 70, style: 50, item: 100, totalQty: 40 };

            const monthColumns = reportData.data?.length > 0
                ? Object.keys(reportData.data[0].monthlyQuantities || {})
                : [];

            /* ✅ 12 Months Limit */
            if (monthColumns.length > 12) {
                alert(`⚠️ Too many months selected (${monthColumns.length}). Maximum allowed is 12.`);
                return;
            }

            const fixedPartWidth = baseCol.slNo + baseCol.buyerName + baseCol.style + baseCol.item + baseCol.totalQty;
            const monthCount = monthColumns.length;
            const monthWidth = monthCount > 0 ? Math.floor((availableWidth - fixedPartWidth) / monthCount) : 50;

            const col = { ...baseCol, month: monthWidth };
            const headerWidths = [col.slNo, col.buyerName, col.style, col.item, col.totalQty, ...monthColumns.map(() => col.month)];
            const tableWidth = headerWidths.reduce((a, b) => a + b, 0);

            /* Prevent table overflow */
            if (tableWidth > availableWidth) {
                alert("Cannot generate PDF: Table width exceeds page width.");
                return;
            }

            /* DateTime */
            const now = new Date();
            const printDateTime = now.toLocaleString("en-US", {
                day: "2-digit", month: "short", year: "numeric",
                hour: "2-digit", minute: "2-digit", second: "2-digit",
                hour12: true
            }).replace(",", "");

            function addFooter(pageNumber) {
                const y = pageHeight - 25;
                doc.setFontSize(8).setTextColor(0, 0, 0);
                doc.text(`Printed on: ${printDateTime}`, margin, y);
                doc.text(`Page ${pageNumber} of ${doc.internal.getNumberOfPages()}`, pageWidth - margin, y, { align: "right" });
            }

            let yPos = margin + 20;

            /* Header */
            doc.setFont("helvetica", "bold").setFontSize(14);
            doc.text(reportData.companyName || "", pageWidth / 2, yPos, { align: "center" });
            yPos += 25;

            doc.setFontSize(12);
            doc.text(`${reportData.reportTitle || ""} ${reportData.reportYear || ""}`, pageWidth / 2, yPos, { align: "center" });
            yPos += 35;

            /* Table Header */
            let xPos = margin;
            doc.setFont("helvetica", "bold").setFontSize(9);
            doc.setFillColor(220, 220, 220);
            doc.rect(xPos, yPos, tableWidth, 20, "F");

            const headers = ["Sl No.", "Buyer Name", "Style", "Item", "T.O.Qty", ...monthColumns];
            headers.forEach((h, i) => {
                const align = i === 0 || i >= 4 ? "center" : "left";
                const cellX = align === "center" ? xPos + headerWidths[i] / 2 : xPos + 4;
                doc.text(h, cellX, yPos + 14, { align });
                /* Border */
                doc.setDrawColor(0);
                doc.rect(xPos, yPos, headerWidths[i], 20);
                xPos += headerWidths[i];
            });

            yPos += 20;
            doc.setFont("helvetica", "normal").setFontSize(8);

            let currentPage = 1;

            reportData.data.forEach((item) => {
                const rowValues = [
                    item.slNo, item.buyerName, item.style, item.item, item.totalOrderQuantity,
                    ...monthColumns.map(m => item.monthlyQuantities[m] ?? "")
                ];

                const cellTexts = rowValues.map((v, i) =>
                    (i >= 4 && v && !isNaN(v)) ? parseFloat(v).toLocaleString("en-US") : (v ?? "").toString()
                );

                const lines = cellTexts.map((t, i) => doc.splitTextToSize(t, headerWidths[i] - 6));
                const lineHeight = 10;
                const padding = 6;
                const rowHeight = Math.max(...lines.map(l => l.length)) * lineHeight + padding * 2;

                /* Prevent Footer Overlap */
                if (yPos + rowHeight + 30 > pageHeight - 30) {
                    addFooter(currentPage++);
                    doc.addPage();
                    yPos = margin + 20;
                }

                /* Alternate Row Background */
                if (item.slNo % 2 === 0) {
                    doc.setFillColor(245, 245, 245);
                    doc.rect(margin, yPos, tableWidth, rowHeight, "F");
                }

                xPos = margin;

                /* Row Cells & Borders */
                cellTexts.forEach((txt, i) => {
                    const lines = doc.splitTextToSize(txt, headerWidths[i] - 4);
                    const align = i === 0 || i >= 4 ? "center" : "left";
                    const cellX = align === "center" ? xPos + headerWidths[i] / 2 : xPos + 4;
                    const cellY = yPos + padding + 8;

                    /* Text */
                    lines.forEach((line, idx) => {
                        doc.text(line, cellX, cellY + (idx * lineHeight), { align });
                    });

                    /* Border */
                    doc.setDrawColor(0);
                    doc.rect(xPos, yPos, headerWidths[i], rowHeight);

                    xPos += headerWidths[i];
                });

                yPos += rowHeight;
            });

            addFooter(currentPage);

            doc.save(`MonthWiseOrderReport.pdf`);
        }


        //style

        function GetOrderReportStyleDownloadPdf() {
            const request = getFilteredValues();

            if ($('#Date').is(':checked')) {
                request.fromDate = formatDateForBackend($('#FromDateSelect').val());
                request.toDate = formatDateForBackend($('#ToDateSelect').val());
            } else if ($('#Year').is(':checked')) {
                request.fromYear = parseInt($('#YearFrom').val(), 10) || null;
                request.toYear = parseInt($('#YearTo').val(), 10) || null;
            }

            $.ajax({
                url: '/MonthWiseOrderBookingReport/DownloadOrderStylePdfReport',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                dataType: 'json',
                beforeSend: showLoading,
                success: function (reportData) {
                   
                    generateStyleReportPdf(reportData);
                },
                error: function (xhr, status, err) {
                    
                    alert('Failed to generate PDF');
                },
                complete: hideLoading
            });
        }

        function generateStyleReportPdf(reportData) {
            if (reportData?.data.length == 0) {
                alert("No  data found");
                return;
            }
            const { jsPDF } = window.jspdf;

            const margin = 15;
            const doc = new jsPDF({ orientation: 'landscape', unit: 'pt', format: 'a4' });
            const pageWidth = doc.internal.pageSize.getWidth();
            const pageHeight = doc.internal.pageSize.getHeight();
            const availableWidth = pageWidth - margin * 2;

            const baseCol = { buyerName: 70, style: 50, item: 100, totalQty: 40 };
            const monthColumns = reportData.monthColumns || [];

            if (monthColumns.length > 12) {
                alert(`Cannot generate PDF: Maximum allowed 12 months. You selected ${monthColumns.length}.`);
                return;
            }

            const fixedPartWidth = baseCol.buyerName + baseCol.style + baseCol.item + baseCol.totalQty;
            const remainingWidth = availableWidth - fixedPartWidth;
            const monthWidth = monthColumns.length > 0 ? Math.floor(remainingWidth / monthColumns.length) : 50;

            const col = { ...baseCol, month: monthWidth };
            const headerWidths = [col.buyerName, col.style, col.item, col.totalQty, ...monthColumns.map(() => col.month)];
            const tableWidth = headerWidths.reduce((a, b) => a + b, 0);

            if (tableWidth > availableWidth) {
                alert("Cannot generate PDF: Table width exceeds available space.");
                return;
            }

            /* Date Time */
            const now = new Date();
            const printDateTime = now.toLocaleString('en-US', {
                day: '2-digit', month: 'short', year: 'numeric',
                hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: true
            }).replace(',', '');

            function addFooter(pageNumber) {
                const y = pageHeight - 25;
                doc.setFontSize(8).setTextColor(0, 0, 0);
                doc.text(`Printed on: ${printDateTime}`, margin, y);
                doc.text(`Page ${pageNumber} of ${doc.internal.getNumberOfPages()}`, pageWidth - margin, y, { align: "right" });
            }

            let yPos = margin + 15;

            /* Header */
            doc.setFont("helvetica", "bold").setFontSize(14);
            doc.text(reportData.companyName || "", pageWidth / 2, yPos, { align: "center" });
            yPos += 25;

            doc.setFontSize(12);
            doc.text(`${reportData.reportTitle || ''} ${reportData.reportYear || ''}`, pageWidth / 2, yPos, { align: "center" });
            yPos += 35;

            /* Table Header */
            doc.setFont("helvetica", "bold").setFontSize(9).setFillColor(220, 220, 220);
            doc.rect(margin, yPos, tableWidth, 20, 'F');

            let xPos = margin;
            const headers = ["Buyer Name", "Style", "Item", "Total Order Qty", ...monthColumns];

            headers.forEach((h, i) => {
                doc.text(h, xPos + headerWidths[i] / 2, yPos + 14, { align: "center" });
                xPos += headerWidths[i];
            });

            yPos += 20;

            /* Table Rows */
            doc.setFont("helvetica", "normal").setFontSize(8);
            let currentPage = 1;

            reportData.data.forEach((item, rowIndex) => {

                const rowValues = [
                    item.buyerName, item.style, item.item, item.totalOrderQuantity,
                    ...monthColumns.map(m => item.monthlyQuantities[m] ?? '')
                ];

                const cellTexts = rowValues.map((v, i) => {
                    let t = v ?? "";
                    return (i >= 3 && t && !isNaN(t)) ? parseFloat(t).toLocaleString('en-US') : t.toString();
                });

                const lines = cellTexts.map((t, i) => doc.splitTextToSize(t, headerWidths[i] - 6));

                /* ✅ Top & Bottom Padding + Consistent Row Height */
                const lineHeight = 10;
                const padding = 6;
                const rowHeight = (Math.max(...lines.map(l => l.length)) * lineHeight) + (padding * 2);

                /* ✅ Prevent Footer Overlap */
                if (yPos + rowHeight + 30 > pageHeight - 30) {
                    addFooter(currentPage++);
                    doc.addPage();
                    yPos = margin + 15;
                }

                /* Alternate Row Color */
                if (rowIndex % 2 === 1) {
                    doc.setFillColor(245, 245, 245);
                    doc.rect(margin, yPos, tableWidth, rowHeight, 'F');
                }

                xPos = margin;

                lines.forEach((txtLines, i) => {

                    /* ✅ Perfect Cell Border (NO GAP) */
                    doc.rect(xPos, yPos, headerWidths[i], rowHeight);

                    const align = i >= 3 ? "center" : "left";
                    const cellX = align === "center" ? xPos + headerWidths[i] / 2 : xPos + 4;

                    /* ✅ Text Y with padding (Top & Bottom spacing perfect) */
                    txtLines.forEach((line, l) => {
                        const textY = yPos + padding + (l * lineHeight) + 8;
                        doc.text(line, cellX, textY, { align });
                    });

                    xPos += headerWidths[i];
                });

                /* Next Row */
                yPos += rowHeight;
            });

            addFooter(currentPage);

            doc.save(`MonthWiseOrderBookingStyleReport.pdf`);
        }



        //style po

        function GetOrderReportStylePoDownloadPdf() {
            const request = getFilteredValues();

            if ($('#Date').is(':checked')) {
                request.fromDate = formatDateForBackend($('#FromDateSelect').val());
                request.toDate = formatDateForBackend($('#ToDateSelect').val());
            } else if ($('#Year').is(':checked')) {
                request.fromYear = parseInt($('#YearFrom').val(), 10) || null;
                request.toYear = parseInt($('#YearTo').val(), 10) || null;
            }

            $.ajax({
                url: '/MonthWiseOrderBookingReport/DownloadOrderStylePoPdfReport',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                dataType: 'json',
                beforeSend: showLoading,
                success: function (reportData) {
                   
                    generateStylePoReportPdf(reportData);
                },
                error: function (xhr, status, err) {
                    
                    alert('Failed to generate PDF');
                },
                complete: hideLoading
            });
        }

        function generateStylePoReportPdf(reportData) {
            if (reportData?.data.length == 0) {
                alert("No  data found");
                return;
            }
            const { jsPDF } = window.jspdf;

            const col = {
                buyerName: 100,
                style: 90,
                item: 130,
                po: 70,
                orderQty: 70,
                month: 55
            };

            const monthColumns = reportData.monthColumns || [];

            // Maximum 12 months check
            if (monthColumns.length > 12) {
                alert(`⚠️ Too many months selected (${monthColumns.length}). Maximum allowed is 12.`);
                return;
            }

            const fixedPartWidth = col.buyerName + col.style + col.item + col.po + col.orderQty;
            const monthsWidth = monthColumns.length * col.month;
            const tableWidth = fixedPartWidth + monthsWidth;

            const margin = 15;
            const pageWidth = Math.max(842, tableWidth + margin * 2);
            const pageHeight = 595;

            const doc = new jsPDF({
                orientation: 'landscape',
                unit: 'pt',
                format: [pageWidth, pageHeight]
            });

            const actualPageHeight = doc.internal.pageSize.getHeight();
            const startX = margin;
            let yPos = margin;

            // Print DateTime
            const now = new Date();
            const printDateTime = now.toLocaleString('en-US', {
                day: '2-digit', month: 'short', year: 'numeric',
                hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: true
            }).replace(',', '');

            // Footer function
            function addFooter(pageNumber) {
                const footerY = actualPageHeight - 25;
                doc.setFont('helvetica', 'normal');
                doc.setFontSize(8);
                doc.setTextColor(0, 0, 0);
                doc.text(`Printed on: ${printDateTime}`, margin, footerY);
                const totalPages = doc.internal.getNumberOfPages();
                doc.text(`Page ${pageNumber} of ${totalPages}`, pageWidth - margin, footerY, { align: 'right' });
            }

            // Header
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(14);
            doc.text(reportData.companyName || '', pageWidth / 2, yPos + 10, { align: 'center' });
            yPos += 25;
            doc.setFontSize(12);
            doc.text(`${reportData.reportTitle || ''} ${reportData.reportYear || ''}`, pageWidth / 2, yPos + 10, { align: 'center' });
            yPos += 35;

            // Table Header
            let xPos = startX;
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(9);
            doc.setFillColor(220, 220, 220);
            doc.setTextColor(0, 0, 0);

            const headers = ['Buyer Name', 'Style', 'Item', 'P.O', 'Order Qty', ...monthColumns];
            const headerWidths = [
                col.buyerName, col.style, col.item, col.po, col.orderQty,
                ...Array(monthColumns.length).fill(col.month)
            ];

            headers.forEach((h, i) => {
                doc.setFillColor(220, 220, 220);
                doc.rect(xPos, yPos, headerWidths[i], 20, 'F');
                doc.setDrawColor(0);
                doc.rect(xPos, yPos, headerWidths[i], 20);
                doc.text(h, xPos + headerWidths[i] / 2, yPos + 14, { align: 'center' });
                xPos += headerWidths[i];
            });

            yPos += 20;

            // Data Rows
            doc.setFont('helvetica', 'normal');
            doc.setFontSize(8);
            let currentPage = 1;
            let currentBuyer = '', currentStyle = '';
            let buyerStartY = 0, styleStartY = 0;
            let rowYPositions = [];

            reportData.data.forEach((item, idx) => {
                const rowValues = [
                    item.buyerName,
                    item.style,
                    item.item,
                    item.purchaseOrder,
                    item.orderQuantity,
                    ...monthColumns.map(m => item.monthlyQuantities[m] ?? '')
                ];

                const cellTexts = rowValues.map((cell, i) => {
                    let txt = (cell ?? '').toString();
                    if (i >= 4 && txt && !isNaN(txt)) txt = parseFloat(txt).toLocaleString('en-US');
                    return txt;
                });

                const linesPerCell = cellTexts.map((txt, i) => {
                    const w = i === 0 ? col.buyerName :
                        i === 1 ? col.style :
                            i === 2 ? col.item :
                                i === 3 ? col.po :
                                    i === 4 ? col.orderQty : col.month;
                    return doc.splitTextToSize(txt, w - 8);
                });

                const rowHeight = Math.max(...linesPerCell.map(l => l.length * 12), 18) + 6;

                // Footer overlap check
                if (yPos + rowHeight + 30 > actualPageHeight - 30) {
                    drawPendingStylePoMerges(doc, rowYPositions, buyerStartY, styleStartY, startX, col, currentBuyer, currentStyle);
                    addFooter(currentPage++);
                    doc.addPage();
                    yPos = margin;
                    rowYPositions = [];
                }

                rowYPositions.push({ y: yPos, item });

                // Alternate row shading
                if (idx % 2 === 1) {
                    doc.setFillColor(245, 245, 245);
                    doc.rect(startX, yPos, tableWidth, rowHeight, 'F');
                }

                xPos = startX;

                // Draw cells with borders and text centered (horizontal + vertical) + padding
                cellTexts.forEach((txt, i) => {
                    const lines = linesPerCell[i];
                    const cellY = yPos + 10 + (rowHeight - lines.length * 12) / 2; // top padding 3
                    const align = 'center';

                    lines.forEach((line, lIdx) => {
                        doc.text(line, xPos + headerWidths[i] / 2, cellY + lIdx * 12, { align });
                    });

                    // Draw border for all cells including Buyer and Style
                    doc.setDrawColor(0);
                    doc.rect(xPos, yPos, headerWidths[i], rowHeight);

                    xPos += headerWidths[i];
                });

                // Buyer merge tracking
                if (currentBuyer !== item.buyerName) {
                    if (currentBuyer !== '' && idx > 0) {
                        drawStylePoMerge(doc, buyerStartY, yPos, startX, col.buyerName, false, currentBuyer);
                    }
                    currentBuyer = item.buyerName;
                    buyerStartY = yPos;
                }

                // Style merge tracking
                if (currentStyle !== item.style || currentBuyer !== item.buyerName) {
                    if (currentStyle !== '' && idx > 0) {
                        drawStylePoMerge(doc, styleStartY, yPos, startX + col.buyerName, col.style, false, currentStyle);
                    }
                    currentStyle = item.style;
                    styleStartY = yPos;
                }

                yPos += rowHeight;
            });

            // Draw last merged cells
            if (rowYPositions.length > 0) {
                drawStylePoMerge(doc, buyerStartY, yPos, startX, col.buyerName, false, currentBuyer);
                drawStylePoMerge(doc, styleStartY, yPos, startX + col.buyerName, col.style, false, currentStyle);
            }

            addFooter(currentPage);
            doc.save(`MonthWiseOrderBookingStylePoReport.pdf`);
        }

        // Helper: Draw vertical merge cells with black border and center text
        function drawStylePoMerge(doc, startY, endY, x, width, isBorderOnly, text) {
            if (startY >= endY) return;
            const height = endY - startY;

            if (!isBorderOnly) {
                doc.setFillColor(255, 255, 255);
                doc.rect(x, startY, width, height, 'F');
            }

            if (text) {
                doc.setFont('helvetica', 'normal');
                doc.setFontSize(8);
                doc.setTextColor(0, 0, 0);
                const lines = doc.splitTextToSize(text, width - 8);
                const totalH = lines.length * 12;
                const textY = startY + (height - totalH) / 2;
                lines.forEach((line, i) => {
                    doc.text(line, x + width / 2, textY + i * 12, { align: 'center' });
                });
            }

            // Black borders
            doc.setDrawColor(0);
            doc.rect(x, startY, width, height);
        }

        // Helper: Draw pending merged cells
        function drawPendingStylePoMerges(doc, rowYPositions, buyerStartY, styleStartY, startX, col, currentBuyer, currentStyle) {
            if (rowYPositions.length === 0) return;
            const lastY = rowYPositions[rowYPositions.length - 1].y + 28;
            drawStylePoMerge(doc, buyerStartY, lastY, startX, col.buyerName, false, currentBuyer);
            drawStylePoMerge(doc, styleStartY, lastY, startX + col.buyerName, col.style, false, currentStyle);
        }

        function GetOrderReportStylePoCSDownloadPdf() {
            const request = getFilteredValues();

            if ($('#Date').is(':checked')) {
                request.fromDate = formatDateForBackend($('#FromDateSelect').val());
                request.toDate = formatDateForBackend($('#ToDateSelect').val());
            } else if ($('#Year').is(':checked')) {
                request.fromYear = parseInt($('#YearFrom').val(), 10) || null;
                request.toYear = parseInt($('#YearTo').val(), 10) || null;
            }

            $.ajax({
                url: '/MonthWiseOrderBookingReport/DownloadOrderStylePoCSPdfReport',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                dataType: 'json',
                beforeSend: showLoading,
                success: function (reportData) {
                   
                    generateStylePoCSReportPdf(reportData);
                },
                error: function (xhr, status, err) {
                    
                    alert('Failed to generate PDF');
                },
                complete: hideLoading
            });
        }



        function generateStylePoCSReportPdf(reportData) {
            if (reportData?.data.length == 0) {
                alert("No  data found");
                return;
            }
            if (reportData?.data.length == 0) {
                alert("No  data found");
                return;
            }
            const { jsPDF } = window.jspdf;
            const monthColumns = reportData.monthColumns || [];
            const numMonths = monthColumns.length;

            /* ---------- Fixed Columns (pt) ---------- */
            const fixedCols = {
                buyerName: 100,
                style: 90,
                item: 130,
                po: 60,
                orderQty: 70
            };
            const fixedPartWidth = Object.values(fixedCols).reduce((a, b) => a + b, 0);

            /* ---------- Dynamic Monthly Columns ---------- */
            const monthlyColWidth = { color: 50, size: 50, qty: 60 };
            const tableWidth = fixedPartWidth + numMonths * (monthlyColWidth.color + monthlyColWidth.size + monthlyColWidth.qty);

            /* ---------- Page Setup ---------- */
            const LEFT_MARGIN = 20;
            const RIGHT_MARGIN = 20;
            const MARGIN = LEFT_MARGIN + RIGHT_MARGIN;
            const pageWidth = tableWidth + MARGIN;
            const pageHeight = (pageWidth * 595) / 842;
            const doc = new jsPDF({
                orientation: 'landscape',
                unit: 'pt',
                format: [pageWidth, pageHeight]
            });

            const actualPageWidth = doc.internal.pageSize.getWidth();
            const actualPageHeight = doc.internal.pageSize.getHeight();
            const tableLeft = LEFT_MARGIN;
            const footerReserve = 70;
            let yPos = LEFT_MARGIN + 20;

            /* ---------- Print DateTime ---------- */
            const now = new Date();
            const printDateTime = now.toLocaleString('en-US', {
                day: '2-digit', month: 'short', year: 'numeric',
                hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: true
            }).replace(',', '');

            /* ---------- Footer ---------- */
            function addFooter(pageNumber) {
                const footerY = actualPageHeight - 30;
                doc.setFont('helvetica', 'normal');
                doc.setFontSize(8);
                doc.setTextColor(0, 0, 0);
                doc.text(`Printed on: ${printDateTime}`, tableLeft, footerY);
                const totalPages = doc.internal.getNumberOfPages();
                doc.text(`Page ${pageNumber} of ${totalPages}`, actualPageWidth - RIGHT_MARGIN, footerY, { align: 'right' });
            }

            /* ---------- Header ---------- */
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(14);
            doc.setTextColor(0, 0, 0);
            doc.text(reportData.companyName || '', actualPageWidth / 2, yPos, { align: 'center' });
            yPos += 25;
            doc.setFontSize(12);
            doc.text(`${reportData.reportTitle || ''} ${reportData.reportYear || ''}`, actualPageWidth / 2, yPos, { align: 'center' });
            yPos += 40;

            /* ---------- Table Header with Borders ---------- */
            let xPos = tableLeft;

            // Main Header with borders
            doc.setFillColor(211, 211, 211);
            doc.rect(xPos, yPos - 12, tableWidth, 20, 'F');
            doc.setDrawColor(0); // Black border
            doc.rect(xPos, yPos - 12, tableWidth, 20); // Draw border
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(9);
            doc.setTextColor(0, 0, 0);

            const fixedHeaders = ['Buyer Name', 'Style', 'Item', 'P.O', 'Order Qty'];
            const fixedWidths = [fixedCols.buyerName, fixedCols.style, fixedCols.item, fixedCols.po, fixedCols.orderQty];

            fixedHeaders.forEach((h, i) => {
                doc.text(h, xPos + fixedWidths[i] / 2, yPos, { align: 'center' });
                xPos += fixedWidths[i];
            });

            let monthStartX = tableLeft + fixedPartWidth;
            monthColumns.forEach(month => {
                const w = monthlyColWidth.color + monthlyColWidth.size + monthlyColWidth.qty;
                doc.text(month, monthStartX + w / 2, yPos, { align: 'center' });
                monthStartX += w;
            });

            // Sub Header with borders
            const subHeaderTop = yPos + 20;
            doc.setFillColor(211, 211, 211);
            doc.rect(tableLeft, subHeaderTop - 12, tableWidth, 20, 'F');
            doc.setDrawColor(0); // Black border
            doc.rect(tableLeft, subHeaderTop - 12, tableWidth, 20); // Draw border

            xPos = tableLeft + fixedPartWidth;
            monthColumns.forEach(() => {
                doc.text('Col.', xPos + monthlyColWidth.color / 2, subHeaderTop, { align: 'center' });
                xPos += monthlyColWidth.color;
                doc.text('Size', xPos + monthlyColWidth.size / 2, subHeaderTop, { align: 'center' });
                xPos += monthlyColWidth.size;
                doc.text('Qty', xPos + monthlyColWidth.qty / 2, subHeaderTop, { align: 'center' });
                xPos += monthlyColWidth.qty;
            });

            yPos = subHeaderTop + 8; // Start data right after sub-header

            /* ---------- Data Rows ---------- */
            doc.setFont('helvetica', 'normal');
            doc.setFontSize(8);
            doc.setTextColor(0, 0, 0);

            let currentPage = 1;
            let currentBuyer = '', currentStyle = '';
            let buyerStartY = 0, styleStartY = 0;
            let rowYPositions = [];

            reportData.data.forEach((item, idx) => {
                let maxRows = 1;
                monthColumns.forEach(m => {
                    if (item.monthlyData?.[m]) maxRows = Math.max(maxRows, item.monthlyData[m].length);
                });
                const blockHeight = maxRows * 25;

                // Page break
                if (yPos + blockHeight + 10 > actualPageHeight - footerReserve) {
                    // Draw pending borders before page break
                    drawPendingMerges(doc, rowYPositions, buyerStartY, styleStartY, tableLeft, fixedCols, currentBuyer, currentStyle);
                    addFooter(currentPage++);
                    doc.addPage();
                    yPos = LEFT_MARGIN + 20;
                    rowYPositions = [];
                }

                const itemStartY = yPos;
                rowYPositions.push({ y: itemStartY, maxRows, blockHeight });

                // Alternate row background
                if (idx % 2 === 1) {
                    doc.setFillColor(248, 248, 248);
                    doc.rect(tableLeft, yPos, tableWidth, blockHeight, 'F');
                }

                // Fixed Columns (skip buyer & style for now - we'll draw them with merges)
                const fixedTexts = [
                    item.buyerName || '',
                    item.style || '',
                    item.item || '',
                    item.purchaseOrder || '',
                    item.orderQuantity ? parseFloat(item.orderQuantity).toLocaleString('en-US') : ''
                ];
                xPos = tableLeft;
                fixedTexts.forEach((txt, i) => {
                    if (i === 0 || i === 1) {
                        xPos += fixedWidths[i];
                        return;
                    }
                    const w = fixedWidths[i];
                    const lines = doc.splitTextToSize(txt, w - 8);
                    const lineHeight = 12;
                    const totalH = lines.length * lineHeight;
                    const startCellY = yPos + (blockHeight - totalH) / 2;
                    lines.forEach((line, lIdx) => {
                        const align = i >= 3 ? 'center' : 'left';
                        const cellX = align === 'center' ? xPos + w / 2 : xPos + 4;
                        doc.text(line, cellX, startCellY + lIdx * lineHeight, { align });
                    });
                    xPos += w;
                });

                // Monthly Data
                let monthX = tableLeft + fixedPartWidth;
                monthColumns.forEach(month => {
                    const details = item.monthlyData?.[month] || [];
                    details.forEach((d, i) => {
                        if (i < maxRows) {
                            const cellY = yPos + (i * 25) + 12;
                            const colorLines = doc.splitTextToSize(d.color || '', monthlyColWidth.color - 8);
                            colorLines.forEach((line, l) => doc.text(line, monthX + monthlyColWidth.color / 2, cellY + l * 12, { align: 'center' }));
                            const sizeLines = doc.splitTextToSize(d.size || '', monthlyColWidth.size - 8);
                            sizeLines.forEach((line, l) => doc.text(line, monthX + monthlyColWidth.color + monthlyColWidth.size / 2, cellY + l * 12, { align: 'center' }));
                            if (d.quantity) {
                                const qty = parseFloat(d.quantity).toLocaleString('en-US');
                                const qtyLines = doc.splitTextToSize(qty, monthlyColWidth.qty - 8);
                                qtyLines.forEach((line, l) => doc.text(line, monthX + monthlyColWidth.color + monthlyColWidth.size + monthlyColWidth.qty / 2, cellY + l * 12, { align: 'center' }));
                            }
                        }
                    });
                    monthX += monthlyColWidth.color + monthlyColWidth.size + monthlyColWidth.qty;
                });

                // Grid Lines (skip buyer and style columns - handled by merge)
                doc.setDrawColor(0); // Black borders
                xPos = tableLeft + fixedCols.buyerName + fixedCols.style; // Start after buyer and style
                [fixedCols.item, fixedCols.po, fixedCols.orderQty].forEach(w => {
                    doc.line(xPos, itemStartY, xPos, itemStartY + blockHeight);
                    xPos += w;
                });
                doc.line(xPos, itemStartY, xPos, itemStartY + blockHeight);

                // Monthly grid lines
                monthX = tableLeft + fixedPartWidth;
                monthColumns.forEach(() => {
                    doc.line(monthX, itemStartY, monthX, itemStartY + blockHeight);
                    monthX += monthlyColWidth.color;
                    doc.line(monthX, itemStartY, monthX, itemStartY + blockHeight);
                    monthX += monthlyColWidth.size;
                    doc.line(monthX, itemStartY, monthX, itemStartY + blockHeight);
                    monthX += monthlyColWidth.qty;
                });

                // Horizontal lines
                for (let i = 0; i <= maxRows; i++) {
                    const lineY = itemStartY + (i * 25);
                    doc.line(tableLeft, lineY, tableLeft + tableWidth, lineY);
                }

                // === Group Change Detection & Border Drawing ===
                const isLastItem = idx === reportData.data.length - 1;
                const nextItem = reportData.data[idx + 1];

                // Buyer Change Detection
                if (currentBuyer !== item.buyerName) {
                    // Close previous buyer group
                    if (currentBuyer !== '') {
                        drawStylePoMerge(doc, buyerStartY, itemStartY, tableLeft, fixedCols.buyerName, false, currentBuyer);
                    }
                    // Start new buyer group
                    currentBuyer = item.buyerName;
                    buyerStartY = itemStartY;
                }

                // Style Change Detection
                if (currentStyle !== item.style || currentBuyer !== item.buyerName) {
                    // Close previous style group
                    if (currentStyle !== '') {
                        drawStylePoMerge(doc, styleStartY, itemStartY, tableLeft + fixedCols.buyerName, fixedCols.style, false, currentStyle);
                    }
                    // Start new style group
                    currentStyle = item.style;
                    styleStartY = itemStartY;
                }

                yPos += blockHeight;
            });

            // Final merge with border
            if (rowYPositions.length > 0) {
                const lastY = yPos;
                drawStylePoMerge(doc, buyerStartY, lastY, tableLeft, fixedCols.buyerName, false, currentBuyer);
                drawStylePoMerge(doc, styleStartY, lastY, tableLeft + fixedCols.buyerName, fixedCols.style, false, currentStyle);
            }

            addFooter(currentPage);
            const fileName = `MonthWiseOrderBookingStylePOColorSizeReport.pdf`;
            doc.save(fileName);
        }

        /* ---------- Draw Merge with Full Border (Matching StylePoReport) ---------- */
        function drawStylePoMerge(doc, startY, endY, x, width, isBorderOnly, text) {
            if (startY == null || endY == null || startY >= endY) return;

            const height = endY - startY;

            // Fill background
            if (!isBorderOnly) {
                doc.setFillColor(255, 255, 255);
                doc.rect(x, startY, width, height, 'F');
            }

            // Draw text if provided
            if (text) {
                doc.setFont('helvetica', 'normal');
                doc.setFontSize(8);
                doc.setTextColor(0, 0, 0);
                const lines = doc.splitTextToSize(text, width - 8);
                const lineHeight = 12;
                const totalH = lines.length * lineHeight;
                const textY = startY + (height - totalH) / 2;
                lines.forEach((line, i) => {
                    doc.text(line, x + width / 2, textY + i * lineHeight, { align: 'center' });
                });
            }

            // Draw borders (black like in StylePoReport)
            doc.setDrawColor(0);
            doc.setLineWidth(0.5);
            doc.rect(x, startY, width, height);
        }

        /* ---------- Page Break Pending Merges with Border ---------- */
        function drawPendingMerges(doc, rowYPositions, buyerStartY, styleStartY, tableLeft, fixedCols, currentBuyer, currentStyle) {
            if (rowYPositions.length === 0) return;
            const lastY = rowYPositions[rowYPositions.length - 1].y + rowYPositions[rowYPositions.length - 1].blockHeight;
            drawStylePoMerge(doc, buyerStartY, lastY, tableLeft, fixedCols.buyerName, false, currentBuyer);
            drawStylePoMerge(doc, styleStartY, lastY, tableLeft + fixedCols.buyerName, fixedCols.style, false, currentStyle);
        }

        // -------------------- Download Event Handler --------------------
        $(document).on('click', '#downloadReport', function () {
           
            var reportValue = $("#reportText").val();
            
            //debugger
            if (reportValue === "downloadPdf") {
                //PdfDownload();
                var styleId = $(settings.styleIds).val()
                if (styleId == '001') {
                    //GetOrderReportAllStyleDownloadPdfReport();
                    GetOrderReportAllStyleDownloadPdf();
                } else if (styleId != null && styleId == '002') {
                    GetOrderReportStyleDownloadPdf();
                } else if (styleId != null && styleId == '003') {
                    GetOrderReportStylePoDownloadPdf();
                } else if (styleId != null && styleId == '004') {
                    GetOrderReportStylePoCSDownloadPdf();
                } else {
                    showToast("warning", "Please Select Style for Excel Report");
                }
            } else if (reportValue === "downloadWord") {
                downloadTableAsWord();
            } else if (reportValue === "downloadExcel") {
                var styleId = $(settings.styleIds).val()
                if (styleId == '001') {
                    GetOrderReportAllStyleDownloadExcelReport();
                } else if (styleId != null && styleId == '002') {
                    GetOrderReportStyleDownloadExcelReport();
                } else if (styleId != null && styleId == '003') {
                    GetOrderReportStylePoDownloadExcelReport();
                }else if (styleId != null && styleId == '004') {
                    GetOrderReportStylePoCSDownloadExcelReport();
                } else {
                    showToast("warning", "Please Select Style for Excel Report");
                }
            } else {
                showToast("warning", "Please Select Report Option");
            }
        });

        // -------------------- Initialization --------------------
        var init = function () {
            showLoading();
            initializeMultiselects();
            setupLoadingOverlay();
            //GetFlatDate();
        };

        init();
    };
})(jQuery);
