(function ($) {
    $.costingReportFilter = function (options) {
        var settings = $.extend({
            baseUrl: "/RMG_CostingInfoReport",
            costingIds: "#costingSelect",
            buyerIds: "#buyerSelect"
        }, options);

        var filterUrl = settings.baseUrl + "/GetAllFilterData";
        var reportUrl = settings.baseUrl + "/GetAllPdfFilterData";
        var excelUrl = settings.baseUrl + "/DownloadExcel";

        var $costingSelect = $(settings.costingIds);
        var $buyerSelect = $(settings.buyerIds);

        // ====== 1. Radio Button  CSS inject  ======
        

        // Loading Overlay
        var setupLoadingOverlay = function () {
            if ($("#customLoadingOverlay").length === 0) {
                $("body").append(`
                    <div id="customLoadingOverlay" style="display:none;position:fixed;top:0;left:0;width:100%;height:100%;
                        background:rgba(0,0,0,0.6);z-index:9999;display:flex;justify-content:center;align-items:center;">
                        <div style="background:white;padding:25px 40px;border-radius:10px;box-shadow:0 0 20px rgba(0,0,0,0.3);text-align:center;">
                            <div class="spinner-border text-primary" style="width:3rem;height:3rem;"></div>
                            <p class="mt-3 mb-0 fw-bold">Loading...</p>
                        </div>
                    </div>
                `);
            }
        };
        var showLoading = () => $("#customLoadingOverlay").show();
        var hideLoading = () => $("#customLoadingOverlay").hide();

        // Initialize Dropdowns
        var initializeDropdowns = function () {
            $costingSelect.multiselect({
                includeSelectAllOption: false,
                nonSelectedText: '-- Select Costing --',
                numberDisplayed: 1,
                buttonWidth: '100%',
                maxHeight: 300,
                enableFiltering: true,
                filterPlaceholder: 'Search Costing...',
               
            });

            $buyerSelect.multiselect({
                includeSelectAllOption: false,
                nonSelectedText: '-- Select Buyer --',
                numberDisplayed: 1,
                buttonWidth: '100%',
                maxHeight: 300,
                enableFiltering: true,
                filterPlaceholder: 'Search Buyer...',
                onChange: function () {
                    filterBuyersByCosting();
                }
            });

            // Remove any centered text and force left-align
            setTimeout(function () {
                $('.multiselect.dropdown-toggle').css('text-align', 'left');
                $('.multiselect-selected-text').css({
                    'float': 'left',
                    'text-align': 'left'
                });
            }, 100);

            $costingSelect.val(null).multiselect('refresh');
            $buyerSelect.val(null).multiselect('refresh');
        };


        var getFilterData = function () {
            return {
                CostingIds: $costingSelect.val() ? [$costingSelect.val()] : [],
                BuyerIds: $buyerSelect.val() ? [$buyerSelect.val()] : []
            };
        };

        // First Load:  Costing +  Buyer
        var loadAllFiltersInitially = function () {
            showLoading();
            $.ajax({
                url: filterUrl,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ CostingIds: [], BuyerIds: [] }),
                success: function (res) {
                    if (!res.isSuccess) {
                        alert("Failed to load data");
                        hideLoading();
                        return;
                    }
                    // Costings
                    $costingSelect.empty();
                    $.each(res.data.costings || [], function (i, item) {
                        $costingSelect.append(`<option value="${item.code}">${item.name}</option>`);
                    });
                    $costingSelect.multiselect('rebuild');

                    // All Buyers
                    $buyerSelect.empty();
                    $.each(res.data.buyers || [], function (i, item) {
                        $buyerSelect.append(`<option value="${item.code}">${item.name}</option>`);
                    });
                    $buyerSelect.multiselect('rebuild');

                    $costingSelect.val(null).multiselect('refresh');
                    $buyerSelect.val(null).multiselect('refresh');
                },
                error: function () {
                    alert("Initial data load failed!");
                },
                complete: hideLoading
            });
        };

        // Costing change → Buyer filter
        var filterBuyersByCosting = function () {
            var buyerSelect = $buyerSelect.val();

            if (!buyerSelect) {
                $buyerSelect.find('option').show();
                $buyerSelect.val(null).multiselect('refresh');
                $buyerSelect.multiselect('rebuild');
                return;
            }
            showLoading();
            $.ajax({
                url: filterUrl,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ CostingIds: [], BuyerIds: [buyerSelect] }),
                success: function (res) {
                    if (!res.isSuccess || !res.data.costings) {
                        alert("No buyer found for this costing");
                        hideLoading();
                        return;
                    }
                    //$buyerSelect.empty();
                    //$.each(res.data.buyers || [], function (i, item) {
                    //    $buyerSelect.append(`<option value="${item.code}">${item.name}</option>`);
                    //});
                    //$buyerSelect.multiselect('rebuild');

                    //$buyerSelect.val(null).multiselect('refresh');
                    $costingSelect.empty();
                    $.each(res.data.costings || [], function (i, item) {
                        $costingSelect.append(`<option value="${item.code}">${item.name}</option>`);
                    });
                    $costingSelect.multiselect('rebuild');
                    $costingSelect.val(null).multiselect('refresh');
                   
                },
                error: function () {
                    alert("Failed to filter buyers");
                },
                complete: hideLoading
            });
        };

        // Report + Download
        var loadReportData = function (cb) {
            var f = getFilterData();
            if (!f.CostingIds.length) return alert("Select Costing First");
            if (!f.BuyerIds.length) return alert("Select Buyer First");

            showLoading();
            $.ajax({
                url: reportUrl,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(f),
                success: function (res) {
                    hideLoading();
                    if (res.isSuccess && res.data?.costingReports?.length > 0) {
                        cb(res.data.costingReports);
                    } else alert("No report data found!");
                },
                error: () => { hideLoading(); alert("Report failed!"); }
            });
        };

        var downloadExcel = function () {
            loadReportData(function (reports) {
                $.ajax({
                    url: excelUrl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(reports),
                    xhrFields: { responseType: "blob" },
                    success: function (blob) {
                        var link = document.createElement("a");
                        link.href = URL.createObjectURL(blob);
                        link.download = `Costing_Report_${new Date().toISOString().slice(0, 19).replace(/:/g, "")}.xlsx`;
                        link.click();
                    },
                    error: () => alert("Excel download failed!")
                });
            });
        };

        var downloadPdf = function (dp) {
            loadReportData(function (reports) {
                generatePdf(reports[0], dp);
            });
        };

        //function generatePdf(data) {
        function generatePdf(data, action) {
            const { jsPDF } = window.jspdf;

            const detailRows = (data.details || []).sort((a, b) => a.slno - b.slno);
            const cols = [30, 60, 120, 40, 45, 45, 40, 40, 35, 45, 35, 55, 55, 50];
            const tableWidth = cols.reduce((a, b) => a + b, 0);
            const margin = 40;
            const pageWidth = tableWidth + (2 * margin);
            const pageHeight = 842;

            const doc = new jsPDF('p', 'pt', [pageWidth, pageHeight]);

            const formatDate = (dateStr) => {
                if (!dateStr) return '';
                return new Date(dateStr).toLocaleDateString('en-GB');
            };

            const formatDateTime = (dateStr) => {
                if (!dateStr) return '';
                const d = new Date(dateStr);
                return d.toLocaleString('en-GB', {
                    day: '2-digit', month: '2-digit', year: 'numeric',
                    hour: '2-digit', minute: '2-digit', second: '2-digit'
                }).replace(',', '');
            };

            const formatNumber = (num) => {
                const numberValue = parseFloat(num || 0);
                if (isNaN(numberValue)) return '';
                return numberValue.toLocaleString('en-US', {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                });
            };

            const printDate = formatDateTime(new Date());

            const addHeader = (yPos) => {
                doc.setFont("times", "bold");
                doc.setFontSize(16);
                doc.text("Costing Report", pageWidth / 2, yPos, { align: "center" });
                return yPos + 20;
            };

            const addFooter = () => {
                const pageCount = doc.getNumberOfPages();
                for (let i = 1; i <= pageCount; i++) {
                    doc.setPage(i);
                    doc.setFontSize(9);
                    doc.setFont("times", "normal");
                    doc.text(`Print Date Time: ${printDate}`, margin, pageHeight - 30);
                    doc.text(`Page ${i} of ${pageCount}`, pageWidth - margin - 40, pageHeight - 30);
                    //doc.line(margin, pageHeight - 45, pageWidth - margin, pageHeight - 45);
                }
            };

            let y = margin + 10;
            y = addHeader(y);

            doc.setFontSize(10);
            doc.setFont("times", "normal");

            const left = [
                ["Costing ID", `: ${data.costingId || ''}`],
                ["Entry Date & Time", `: ${formatDateTime(data.entryDateTime)}`],
                ["Issued By", `: ${data.issuedBy || ''}`],
                ["Checked by", `: ${data.checkedName || ''}`],
                ["Remarks", `: ${data.remarks || ''}`]
            ];

            const right = [
                ["Buyer", `: ${data.buyerName || ''}`],
                ["Fun Job No.", `: ${data.funJobNo || ''}`],
                ["Style", `: ${data.styleName || ''}`],
                ["PO No.", `: ${data.poNo || ''}`],
                ["Product", `: ${data.itemName || ''}`],
                ["Product Description", `: ${data.productDescription || ''}`],
                ["Ref No./Client Ord. No.", `: ${data.refNo || ''}`],
                ["Shipment Date", `: ${formatDate(data.shipmentDate)}`]
            ];

            left.forEach(([label, value]) => {
                doc.text(label, margin, y);
                doc.text(value, margin + 120, y);
                y += 18;
            });

            y = margin + 45;
            right.forEach(([label, value]) => {
                doc.text(label, pageWidth / 2 + 20, y);
                doc.text(value, pageWidth / 2 + 130, y);
                y += 18;
            });

            y += 25;

            // Color & Size Breakup

            doc.setFont("times", "bold");
            doc.text("Color & Size Breckup Details :", margin, y);
            y += 20;

            const colWidthsBreakup = [50, 150, 150, 100];
            const breakupTableWidth = colWidthsBreakup.reduce((a, b) => a + b, 0);
            const tableHeaders = ["Sl No.", "Color", "Size", "Quantity"];

            // ================= HEADER ==================
            doc.setFillColor(220, 220, 220);
            doc.rect(margin, y - 12, breakupTableWidth, 20, 'F');
            doc.setDrawColor(0);
            doc.setLineWidth(0.5);
            doc.rect(margin, y - 12, breakupTableWidth, 20);

            doc.setFontSize(9);
            doc.setFont("times", "bold");

            let xPos = margin + 5;

            // ---- HEADER TEXT CENTER ALIGN EXCEPT LAST ----
            let headerX = margin;

            tableHeaders.forEach((header, i) => {

                const colWidth = colWidthsBreakup[i];
                let textX = headerX + colWidth / 2; // Center point

                // Last column keep default left-right behavior
                if (i === 3) {
                    doc.text(header, headerX + colWidth - 10, y, { align: "right" });
                } else {
                    doc.text(header, textX, y, { align: "center" });
                }

                if (i < tableHeaders.length - 1) {
                    doc.line(headerX + colWidth, y - 12, headerX + colWidth, y + 8);
                }

                headerX += colWidth;
            });

            y += 20;

            // ================= ROWS ==================
            doc.setFont("times", "normal");

            const validBreakups = (data.colorSizeBreakups || [])
                .filter(x => (x.colorName || x.color) && (x.sizeName || x.size));

            let totalQty = 0;

            validBreakups.forEach((item, idx) => {
                const colorName = item.colorName || item.color || '';
                const sizeName = item.sizeName || item.size || '';
                const qty = item.quantity || 0;
                totalQty += qty;

                doc.setFillColor(idx % 2 === 0 ? 255 : 250);
                doc.rect(margin, y - 12, breakupTableWidth, 18, 'F');
                doc.rect(margin, y - 12, breakupTableWidth, 18);

                let rowX = margin;

                // ---- Sl No (center) ----
                doc.text((idx + 1).toString(), rowX + colWidthsBreakup[0] / 2, y, { align: "center" });
                rowX += colWidthsBreakup[0];
                doc.line(rowX, y - 12, rowX, y + 6);

                // ---- Color (center) ----
                doc.text(colorName, rowX + colWidthsBreakup[1] / 2, y, { align: "center" });
                rowX += colWidthsBreakup[1];
                doc.line(rowX, y - 12, rowX, y + 6);

                // ---- Size (center) ----
                doc.text(sizeName, rowX + colWidthsBreakup[2] / 2, y, { align: "center" });
                rowX += colWidthsBreakup[2];
                doc.line(rowX, y - 12, rowX, y + 6);

                // ---- Quantity (RIGHT) ----
                doc.text(qty.toString(), rowX + colWidthsBreakup[3] - 10, y, { align: "right" });

                y += 18;
            });

            // ================= TOTAL ROW ==================
            doc.setFillColor(240, 240, 240);
            doc.rect(margin, y - 12, breakupTableWidth, 18, 'F');
            doc.rect(margin, y - 12, breakupTableWidth, 18);

            doc.setFont("times", "bold");

            // ----- MERGED CELL (Sl No + Color + Size) -----
            const mergedWidth = colWidthsBreakup[0] + colWidthsBreakup[1] + colWidthsBreakup[2];
            const mergedX = margin;

            // Draw merged cell border
            doc.rect(mergedX, y - 12, mergedWidth, 18);

            // Text aligned RIGHT inside merged cell
            doc.text("Total :", mergedX + mergedWidth - 5, y, { align: "right" });

            // ----- LAST COLUMN (Quantity) -----
            const qtyX = mergedX + mergedWidth;

            // right column border
            doc.rect(qtyX, y - 12, colWidthsBreakup[3], 18);

            // Value right aligned
            doc.text(totalQty.toString(), qtyX + colWidthsBreakup[3] - 5, y, { align: "right" });

            y += 30;

            // Main Details Table
            const drawDetailTableHeader = (yStart) => {
                doc.setFillColor(200, 200, 200);
                doc.rect(margin, yStart, tableWidth, 30, 'F');
                doc.rect(margin, yStart, tableWidth, 30);

                doc.setFontSize(8);
                doc.setFont("times", "bold");
                const headers = ["Sl No.", "Item", "Description", "Width", "Gar. Qty", "Cons./pcs.",
                    "Extra (%)", "Total", "Unit", "Unit Price", "Unit", "Amount($)-SH/HKG", "Amount($)-BD",
                    "Amount(THB)"];

                let x = margin + 2;
                headers.forEach((h, i) => {
                    const colWidth = cols[i];
                    const lines = doc.splitTextToSize(h, colWidth - 4);
                    const centerX = x + (colWidth / 2);
                    const textY = yStart + 10 + (lines.length > 1 ? 0 : 5);

                    // Draw centered text only
                    lines.forEach((line, idx) => {
                        doc.text(line, centerX, textY + (idx * 10), { align: "center" });
                    });

                    if (i < headers.length - 1) {
                        doc.line(x + colWidth, yStart, x + colWidth, yStart + 30);
                    }

                    x += colWidth;
                });

                return yStart + 30;
            };

            if (y + 30 > pageHeight - 100) {
                doc.addPage();
                y = margin + 10;
                y = addHeader(y);
            }

            y = drawDetailTableHeader(y);


            doc.setFont("times", "normal");
            detailRows.forEach((d, idx) => {
                const rowData = [
                    d.slno || '',
                    d.itemName || '',
                    d.description || '',
                    d.width || '',
                    d.quantity > 0 ? d.quantity : '',
                    d.consumption > 0 ? d.consumption.toFixed(2) : '',
                    d.extra > 0 ? d.extra + '%' : '',
                    d.total || '',
                    d.unit || '',
                    d.unitPrice > 0 ? d.unitPrice.toFixed(2) : '',
                    d.totalQuantityUnit || '',
                    d.totalAmountShhkg > 0 ? formatNumber(d.totalAmountShhkg) : '',
                    d.totalAmountBdt > 0 ? formatNumber(d.totalAmountBdt) : '',
                    d.totalAmountThb > 0 ? formatNumber(d.totalAmountThb) : ''
                ];

                const paddingTop = 10;
                const paddingBottom = 10;

                const colHeights = rowData.map((val, i) => {
                    const text = val.toString();
                    const lines = doc.splitTextToSize(text, cols[i] - 4);
                    return lines.length * 6 + paddingTop + paddingBottom;
                });
                const rowHeight = Math.max(...colHeights);

                if (y + rowHeight > pageHeight - 80) {
                    doc.addPage();
                    y = margin + 10;
                    y = addHeader(y);
                    y = drawDetailTableHeader(y);

                    doc.setFont("times", "normal");
                }

                doc.setFillColor(idx % 2 === 0 ? 255 : 252);
                doc.rect(margin, y, tableWidth, rowHeight, 'F');
                doc.rect(margin, y, tableWidth, rowHeight);

                let x = margin + 2;
                rowData.forEach((val, i) => {
                    const text = val.toString();
                    const lines = doc.splitTextToSize(text, cols[i] - 4);
                    const textHeight = lines.length * 6;
                    const textY = y + (rowHeight - textHeight) / 2 + 4;

                    //if (i === 2) {
                    //    doc.text(lines, x + 2, y + paddingTop);
                    //}

                    const centerCols = [0, 3, 4, 5, 6, 7, 8, 9, 10];

                    if (centerCols.includes(i)) {
                        lines.forEach((line, idxLine) => {
                            doc.text(line, x + cols[i] / 2, textY + idxLine * 6, { align: "center" });
                        });
                    }
                    else if ([11, 12, 13].includes(i)) {
                        doc.text(lines, x + cols[i] - 4, textY, { align: 'right' });
                    } else {
                        doc.text(lines, x + 2, textY);
                    }

                    if (i < cols.length - 1) {
                        doc.line(x + cols[i], y, x + cols[i], y + rowHeight);
                    }
                    x += cols[i];
                });

                y += rowHeight;
            });

            // Summary Section
            if (y + 250 > pageHeight - 80) {
                doc.addPage();
                y = margin + 10;
                y = addHeader(y);
            }

            doc.setFontSize(10);
            doc.setFont("times", "normal");

            // Calculate exact column positions
            const colX = [margin];
            for (let i = 0; i < cols.length; i++) {
                colX.push(colX[i] + cols[i]);
            }

            const summaryRowHeight = 15;
            const cellPadding = 5;
            const extraLabelWidth = 2;

            const summaryRows = [
                { label: "Sub Total:", v1: data.subTotalAmountShhkg || '0.00', v2: data.subTotalAmountBdt || '0.00', v3: data.subTotalAmountThb || '0.00', type: 'bordered' },
                { label: "Sub Total (Per Gar. Qty):", v1: (data.subTotalAmountShhkg / data.details[0].quantity) || '0.00', v2: (data.subTotalAmountBdt / data.details[0].quantity) || '0.00', v3: (data.subTotalAmountThb / data.details[0].quantity) || '0.00', type: 'bordered' },
                { label: "Damage(%)", v1: (data.subTotalAmountShhkg / data.details[0].quantity) * (data.damagePercentage / 100) || "0.00", v2: (data.subTotalAmountBdt / data.details[0].quantity) * (data.damagePercentage / 100) || '0.00', v3: (data.subTotalAmountThb / data.details[0].quantity) * (data.damagePercentage / 100) || "0.00", type: 'bordered' },
                { label: "Interest/Overhead(%)", v1: (data.subTotalAmountShhkg / data.details[0].quantity) * (data.interestOverheadPercentage / 100) || "0.00", v2: (data.subTotalAmountBdt / data.details[0].quantity) * (data.interestOverheadPercentage / 100) || "0.00", v3: (data.subTotalAmountThb / data.details[0].quantity) * (data.interestOverheadPercentage / 100) || '0.00', type: 'bordered' },
                {
                    label: "Total:", v1: (data.subTotalAmountShhkg / data.details[0].quantity) + (data.subTotalAmountShhkg / data.details[0].quantity) * (data.damagePercentage / 100) + (data.subTotalAmountShhkg / data.details[0].quantity) * (data.interestOverheadPercentage / 100) || '0.00',
                    v2: (data.subTotalAmountBdt / data.details[0].quantity) + (data.subTotalAmountBdt / data.details[0].quantity) * (data.damagePercentage / 100) + (data.subTotalAmountBdt / data.details[0].quantity) * (data.interestOverheadPercentage / 100) || "0.00",
                    v3: (data.subTotalAmountThb / data.details[0].quantity) + (data.subTotalAmountThb / data.details[0].quantity) * (data.damagePercentage / 100) + (data.subTotalAmountThb / data.details[0].quantity) * (data.interestOverheadPercentage / 100) || '0.00', type: 'bordered-total'
                },
                { label: "Total Material Cost from Overseas:", value: data.totalMaterialCostOverseas || '0.00', unit: "USD", type: 'simple' },
                { label: "Total Material Cost from Bangladesh:", value: data.totalMaterialCostBdt || '0.00', unit: "USD", type: 'simple' },
                { label: "Total Material Cost from BKK +20%:", value: data.totalAmountThb || '0.00', unit: "USD", type: 'simple' },
                { label: "CM And Profit:", value: data.cmandProfit || '0.00', unit: "USD", type: 'simple' },
                { label: "Handling Charge:", value: data.handlingCharge || '0.00', unit: "USD", type: 'simple' },
                { label: "Production Upcharge:", value: data.productionUpCharge || '0.00', unit: "USD", type: 'simple' },
                { label: "FF Price:", value: data.ffprice || '0.00', unit: "USD", type: 'simple' },
                { label: "Total:", value: data.details[0].quantity * data.ffprice || '0.00', unit: "USD", type: 'simple-total' }
            ];

            summaryRows.forEach((row, idx) => {
                const textY = y + summaryRowHeight - cellPadding;

                if (row.label.includes("Total:")) {
                    doc.setFont("times", "bold");
                } else {
                    doc.setFont("times", "normal");
                }

                doc.setLineWidth(0.5);
                doc.setDrawColor(0);

                if (row.type === 'bordered' || row.type === 'bordered-total') {
                    // Label cell - width 
                    const labelWidth = colX[11] - colX[0] + extraLabelWidth;
                    doc.rect(colX[0], y, labelWidth, summaryRowHeight, 'S');
                    doc.text(row.label, colX[0] + labelWidth - 5, textY, { align: 'right' });

                    // Column 11 - position adjust এবং width 
                    const col11X = colX[0] + labelWidth;
                    const col11W = cols[11];
                    doc.rect(col11X, y, col11W, summaryRowHeight, 'S');
                    doc.text(formatNumber(row.v1), col11X + col11W - 3, textY, { align: 'right' });

                    // Column 12 
                    doc.rect(colX[12] + 2, y, cols[12], summaryRowHeight, 'S');
                    doc.text(formatNumber(row.v2), colX[12] + cols[12] - 3, textY, { align: 'right' });

                    // Column 13 
                    doc.rect(colX[13] + 2, y, cols[13] - 2, summaryRowHeight, 'S');
                    const v3Text = row.label.includes('(%)') ? parseFloat(row.v3 || 0).toFixed(2) : formatNumber(row.v3);
                    doc.text(v3Text, colX[13] + cols[13] - 3, textY, { align: 'right' });


                } else {
                    // Simple rows
                    doc.text(row.label, colX[12] - 5, textY, { align: 'right' });

                    let valueText;
                    if (row.label.includes("CM And Profit:") || row.label.includes("Handling Charge:") ||
                        row.label.includes("Production Upcharge:") || row.label.includes("FF Price:")) {
                        valueText = parseFloat(row.value || 0).toFixed(2);
                    } else {
                        valueText = formatNumber(row.value);
                    }
                    doc.rect(colX[12] + 2, y, cols[12], summaryRowHeight, 'S');
                    doc.text(valueText, colX[12] + cols[12] - 3, textY, { align: 'right' });

                    doc.rect(colX[13] + 2, y, cols[13] - 2, summaryRowHeight, 'S');
                    if (row.unit) {
                        doc.text(row.unit, colX[13] + cols[13] - 3, textY, { align: 'right' });
                    }
                }

                y += summaryRowHeight;
            });

            //addFooter();
            //doc.save(`Costing_Report_${data.costingId}.pdf`);

            addFooter();

            // --- FINAL ACTION HANDLER ---
            if (action === "download") {
                doc.save(`Costing_Report_${data.costingId}.pdf`);
            }
            else if (action === "preview") {

                const pdfBlob = doc.output("blob");
                const pdfUrl = URL.createObjectURL(pdfBlob);

                const previewContainer = document.getElementById("pdf-preview-container");
                previewContainer.style.display = "block";

                previewContainer.innerHTML = `
            <iframe src="${pdfUrl}" style="width:100%; height:100%; border:none;"></iframe>
        `;
            }
        }
        $(document).on('click', '#downloadReport', function () {
            var type = $("#reportText").val();
           
            if (type === "downloadExcel") downloadExcel();
            else if (type === "downloadPdf") downloadPdf("download");
            //else if (type === "downloadPdf") generatePdf(globalData, "download");

            else alert("Select Report Type!");
        });
        $(document).on('click', '#btnPreviewPdf', function () {
            downloadPdf("preview");   // ⬅ CALL WITH ACTION
        });

        // Init
        var init = function () {           
            setupLoadingOverlay();
            initializeDropdowns();
            loadAllFiltersInitially();
        };

        init();
    };
})(jQuery);