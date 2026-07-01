(function ($) {
    $.dailyAttendanceDetails = function (options) {
        var settings = $.extend({ baseUrl: "/" }, options);

        var GetSummaryUrl = settings.baseUrl + "/GetSummary";
        var DownloadExcelUrl = settings.baseUrl + "/DownloadExcel";

        // ── AttendanceTypeCode → ReportType map ──────────────────────
        var CODE_TO_REPORT = {
            '01': 'Present',
            '02': 'Late',
            '03': 'Absent',
            '04': 'InOut',
            '05': 'MissingCheckOut',
            '06': 'EarlyLeave'
        };

        var REPORT_TITLES = {
            'Present': 'Daily Present Report',
            'Late': 'Daily Late Report',
            'Absent': 'Daily Absent Report',
            'InOut': 'Daily In-Out Report',
            'MissingCheckOut': 'Daily Missing Check-Out Report',
            'EarlyLeave': 'Daily Early Office Leave Report'
        };

        var LEGENDS = {
            'Present': 'Status Legend: P- Present, L- Late',
            'Late': 'Status Legend: L - Late',
            'Absent': 'Status Legend: A- Absent',
            'InOut': 'Status Legend: P-Present, L- Late, A- Absent, W- Weekend, H- Holiday, CL- Casual Leave, SL- Sick Leave, UL- Unpaid Leave, ML- Maternity Leave, PL- Paternity Leave, MarL- Marriage Leave, HL- Hajj Leave, UmrL- Umrah Leave',
            'MissingCheckOut': 'Status Legend: MCO- Missing Check-Out',
            'EarlyLeave': 'Status Legend: EOL- Early Office Leave'
        };

        // ── Loading Overlay ──────────────────────────────────────────
        (function setupOverlay() {
            if ($("#customLoadingOverlay").length === 0) {
                $("body").append(`
                    <div id="customLoadingOverlay" style="
                        display:none;position:fixed;top:0;left:0;
                        width:100%;height:100%;background:rgba(0,0,0,.5);
                        z-index:9999;justify-content:center;align-items:center;">
                        <div style="background:#fff;padding:20px;border-radius:5px;
                                    box-shadow:0 0 10px rgba(0,0,0,.3);text-align:center;">
                            <div class="spinner-border text-primary" role="status">
                                <span class="sr-only">Loading...</span>
                            </div>
                            <p style="margin-top:10px;margin-bottom:0;">Loading data...</p>
                        </div>
                    </div>`);
            }
        })();

        function showLoading() { $("#customLoadingOverlay").css("display", "flex"); }
        function hideLoading() { $("#customLoadingOverlay").hide(); }

        // ── Filter ───────────────────────────────────────────────────
        function getFilter() {
            function getMultiVal(sel) {
                var v = $(sel).val();
                if (!v) return null;
                var arr = Array.isArray(v) ? v : [v];
                arr = arr.filter(function (x) { return x && x !== ""; });
                return arr.length > 0 ? arr : null;
            }
            var typeCode = $("#reportTypeSelect").val() || "01";
            return {
                CompanyCode: $("#companySelect").val() || null,
                BranchCodes: getMultiVal("#branchSelect"),
                DepartmentCodes: getMultiVal("#departmentSelect"),
                EmployeeIds: getMultiVal("#employeeSelect"),
                FromDate: $("#DateSelect").val() || null,
                ReportType: CODE_TO_REPORT[typeCode] || 'Present',
                AttendanceTypeCode: typeCode
            };
        }

        // ── Date picker ──────────────────────────────────────────────
        flatpickr("#DateSelect", CalendarService.createConfig({ defaultDate: new Date() }));

        // ── Load data ────────────────────────────────────────────────
        function loadData(callback) {
            showLoading();
            $.ajax({
                url: GetSummaryUrl,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(getFilter()),
                success: function (res) {
                    hideLoading();                    
                    if (res.success && res.data) { callback(res.data); }
                    else { alert(res.message || 'No data found.'); }
                },
                error: function () { hideLoading(); alert('Failed to load data.'); }
            });
        }

        // ── Helpers ──────────────────────────────────────────────────
        function formatPrintDT() {
            var d = new Date();
            var dd = String(d.getDate()).padStart(2, '0');
            var mm = String(d.getMonth() + 1).padStart(2, '0');
            var yy = d.getFullYear();
            var h = d.getHours();
            var min = String(d.getMinutes()).padStart(2, '0');
            var sec = String(d.getSeconds()).padStart(2, '0');
            var ap = h >= 12 ? 'PM' : 'AM';
            h = h % 12 || 12;
            return dd + '/' + mm + '/' + yy + ' ' + String(h).padStart(2, '0') + ':' + min + ':' + sec + ' ' + ap;
        }

        function getImageBase64(url, cb) {
            var img = new Image();
            img.crossOrigin = 'Anonymous';
            img.onload = function () {
                var c = document.createElement('canvas');
                c.width = img.naturalWidth;
                c.height = img.naturalHeight;
                c.getContext('2d').drawImage(img, 0, 0);
                cb(c.toDataURL('image/png'), img.naturalWidth, img.naturalHeight);
            };
            img.onerror = function () { cb(null, 0, 0); };
            img.src = url;
        }

        // ── Status text → abbreviated code ──────────────────────────
        function abbrStatus(s) {
            var v = (s || '').toString().trim();
            if (v === 'Present') return 'P';
            if (v === 'Late') return 'L';
            if (v === 'Absent') return 'A';
            return v;
        }

        // ── Format time as 12-hour AM/PM (with seconds) ───────────────
        // Accepts "HH:MM", "HH:MM:SS", or a full datetime string.
        // Returns '' for empty values; leaves unrecognized formats as-is.
        function formatAmPm(val) {
            if (val === null || val === undefined || val === '') return '';
            var s = val.toString().trim();
            if (s === '') return '';

            var h, m, sec;

            // Plain time strings like "14:05" or "14:05:30"
            var timeMatch = s.match(/^(\d{1,2}):(\d{2})(:(\d{2}))?$/);
            if (timeMatch) {
                h = parseInt(timeMatch[1], 10);
                m = parseInt(timeMatch[2], 10);
                sec = timeMatch[4] !== undefined ? parseInt(timeMatch[4], 10) : 0;
            } else {
                // Try parsing as a full date/datetime string
                var d = new Date(s);
                if (!isNaN(d.getTime())) {
                    h = d.getHours();
                    m = d.getMinutes();
                    sec = d.getSeconds();
                } else {
                    return s; // unrecognized format, leave untouched
                }
            }

            var ap = h >= 12 ? 'PM' : 'AM';
            var h12 = h % 12 || 12;
            return String(h12).padStart(2, '0') + ':' + String(m).padStart(2, '0') + ':' + String(sec).padStart(2, '0') + ' ' + ap;
        }

        // ── Format late duration as "X Hrs. Y Min." ───────────────────
        // Accepts "HH:MM", "H:MM:SS", or a plain number of minutes.
        // Returns '' for empty/zero values.
        function formatHrsMin(val) {
            if (val === null || val === undefined || val === '') return '';
            var totalMinutes = 0;
            var s = val.toString().trim();

            if (s.indexOf(':') !== -1) {
                var parts = s.split(':');
                var h = parseInt(parts[0], 10) || 0;
                var m = parseInt(parts[1], 10) || 0;
                totalMinutes = (h * 60) + m;
            } else {
                var n = parseFloat(s);
                if (isNaN(n)) return s; // not a recognizable numeric/time format, leave as-is
                totalMinutes = Math.round(n);
            }

            if (totalMinutes <= 0) return '';

            var hrs = Math.floor(totalMinutes / 60);
            var mins = totalMinutes % 60;

            if (hrs > 0 && mins > 0) return hrs + ' Hrs. ' + mins + ' Min.';
            if (hrs > 0) return hrs + ' Hrs.';
            return mins + ' Min.';
        }

        
        // ── Status cell text color ───────────────────────────────────
        function statusColor(raw) {
            var v = (raw || '').toString().trim();
            if (v === 'P' || v === 'Present') return [0, 128, 0];
            if (v === 'L' || v === 'Late') return [200, 120, 0];
            if (v === 'A' || v === 'Absent') return [200, 0, 0];
            if (v === 'MCO') return [180, 80, 0];
            if (v === 'EOL') return [100, 0, 150];
            return [0, 0, 0];
        }

        // ── Page dimensions (pt) ─────────────────────────────────────
        // A4: 595 × 842 pt  |  A4 landscape: 842 × 595 pt
        // Left+Right margin = 25+25 = 50 pt each orientation
        var A4_PORTRAIT_USABLE = 595 - 50;   // 545 pt
        var A4_LANDSCAPE_USABLE = 842 - 50;   // 792 pt

        // ── Column config per report type ────────────────────────────
        // Returns { rows, headCols, bodyMapper, statusColIdx }
        // headCols[*].styles.cellWidth are MINIMUM widths; the Name
        // column (index 2) is replaced later by fitNameColumn().
        // After that, all fixed columns are scaled proportionally so
        // the table fills the usable page width exactly.
        function getReportConfig(rptType, data) {
            var rows, headCols, bodyMapper, statusColIdx;            
            console.log(data);
            if (rptType === 'Present') {
                rows = data.presentRows || [];
                statusColIdx = 7;
                headCols = [
                    { content: 'SN', styles: { cellWidth: 22 } },
                    { content: 'Emp. ID', styles: { cellWidth: 55 } },
                    { content: 'Name', styles: { cellWidth: 'auto' } },
                    { content: 'Designation', styles: { cellWidth: 90 } },
                    { content: 'Shift', styles: { cellWidth: 70 } },
                    { content: 'In Time', styles: { cellWidth: 52 } },
                    { content: 'Late', styles: { cellWidth: 36 } },
                    { content: 'Status', styles: { cellWidth: 36 } },
                    { content: 'Remarks', styles: { cellWidth: 50 } }
                ];
                bodyMapper = function (r, sn) {
                    return [sn, r.employeeId, r.employeeName, r.designation,
                        r.shiftName, formatAmPm(r.inTime), r.lateDisplay, abbrStatus(r.status), r.remarks];
                };

            } else if (rptType === 'Absent') {
                rows = data.absentRows || [];
                statusColIdx = 4;
                headCols = [
                    { content: 'SN', styles: { cellWidth: 28 } },
                    { content: 'Emp. ID', styles: { cellWidth: 65 } },
                    { content: 'Name', styles: { cellWidth: 'auto' } },
                    { content: 'Designation', styles: { cellWidth: 90 } },
                    { content: 'Status', styles: { cellWidth: 40 } }
                ];
                bodyMapper = function (r, sn) {
                    return [sn, r.employeeId, r.employeeName, r.designation, abbrStatus(r.status)];
                };

            } else if (rptType === 'Late') {
                rows = data.lateRows || [];
                statusColIdx = 7;
                headCols = [
                    { content: 'SN', styles: { cellWidth: 22 } },
                    { content: 'Emp. ID', styles: { cellWidth: 55 } },
                    { content: 'Name', styles: { cellWidth: 'auto' } },
                    { content: 'Designation', styles: { cellWidth: 90 } },
                    { content: 'Shift', styles: { cellWidth: 70 } },
                    { content: 'In Time', styles: { cellWidth: 52 } },
                    { content: 'Late', styles: { cellWidth: 60 } },
                    { content: 'Status', styles: { cellWidth: 36 } },
                    { content: 'Remarks', styles: { cellWidth: 50 } }
                ];
                bodyMapper = function (r, sn) {
                    return [sn, r.employeeId, r.employeeName, r.designation,
                        r.shiftName, formatAmPm(r.inTime), r.lateDisplay, abbrStatus(r.status), r.remarks];
                };

            } else if (rptType === 'InOut') {
                rows = data.inOutRows || [];
                statusColIdx = 11;
                headCols = [
                    { content: 'SN', styles: { cellWidth: 20 } },
                    { content: 'Emp. ID', styles: { cellWidth: 65 } },
                    { content: 'Name', styles: { cellWidth: 'auto' } },
                    { content: 'Designation', styles: { cellWidth: 80 } },
                    { content: 'Shift', styles: { cellWidth: 95 } },
                    { content: 'In Time', styles: { cellWidth: 62 } },
                    { content: 'Late', styles: { cellWidth: 55 } },
                    { content: 'Out Time', styles: { cellWidth: 62 } },
                    { content: 'Early Out', styles: { cellWidth: 50 } },
                    { content: 'W.Hour(s)', styles: { cellWidth: 55 } },
                    { content: 'OT(H)', styles: { cellWidth: 40 } },
                    { content: 'Status', styles: { cellWidth: 34 } },
                    { content: 'Remarks', styles: { cellWidth: 50 } }
                ];
                bodyMapper = function (r, sn) {
                    return [sn, r.employeeId, r.employeeName, r.designation,
                        r.shiftName, formatAmPm(r.inTime), r.lateDisplay,
                        formatAmPm(r.outTime), r.earlyOut, r.workHours,
                        r.otHours, abbrStatus(r.status), r.remarks];
                };

            } else if (rptType === 'MissingCheckOut') {
                rows = data.missingCheckOutRows || [];
                statusColIdx = 10;
                headCols = [
                    { content: 'SN', styles: { cellWidth: 20 } },
                    { content: 'Emp. ID', styles: { cellWidth: 65 } },
                    { content: 'Name', styles: { cellWidth: 'auto' } },
                    { content: 'Designation', styles: { cellWidth: 80 } },
                    { content: 'Shift', styles: { cellWidth: 95 } },
                    { content: 'In Time', styles: { cellWidth: 62 } },
                    { content: 'Late', styles: { cellWidth: 55 } },
                    { content: 'Out Time', styles: { cellWidth: 62 } },
                    { content: 'Early Out', styles: { cellWidth: 50 } },
                    { content: 'W.Hour(s)', styles: { cellWidth: 55 } },
                    { content: 'Status', styles: { cellWidth: 34 } },
                    { content: 'Remarks', styles: { cellWidth: 50 } }
                ];
                bodyMapper = function (r, sn) {
                    return [sn, r.employeeId, r.employeeName, r.designation,
                        r.shiftName, formatAmPm(r.inTime), r.lateDisplay,
                        formatAmPm(r.outTime), r.earlyOut, r.workHours,
                        abbrStatus(r.status), r.remarks];
                };

            } else { // EarlyLeave
                rows = data.earlyLeaveRows || [];
                statusColIdx = 10;
                headCols = [
                    { content: 'SN', styles: { cellWidth: 20 } },
                    { content: 'Emp. ID', styles: { cellWidth: 65 } },
                    { content: 'Name', styles: { cellWidth: 'auto' } },
                    { content: 'Designation', styles: { cellWidth: 80 } },
                    { content: 'Shift', styles: { cellWidth: 95 } },
                    { content: 'In Time', styles: { cellWidth: 62 } },
                    { content: 'Late', styles: { cellWidth: 55 } },
                    { content: 'Out Time', styles: { cellWidth: 62 } },
                    { content: 'Early Out', styles: { cellWidth: 50 } },
                    { content: 'W.Hour(s)', styles: { cellWidth: 55 } },
                    { content: 'Status', styles: { cellWidth: 34 } },
                    { content: 'Remarks', styles: { cellWidth: 50 } }
                ];
                bodyMapper = function (r, sn) {
                    return [sn, r.employeeId, r.employeeName, r.designation,
                        r.shiftName, formatAmPm(r.inTime), r.lateDisplay,
                        formatAmPm(r.outTime), r.earlyOut, r.workHours,
                        abbrStatus(r.status), r.remarks];
                };
            }

            return { rows: rows, headCols: headCols, bodyMapper: bodyMapper, statusColIdx: statusColIdx };
        }

        // ── Measure longest name and set Name column width ───────────
        // Returns the computed name column width (pt).
        function fitNameColumn(measureDoc, headCols, rows) {
            measureDoc.setFont('times', 'normal');
            measureDoc.setFontSize(7.5);
            var maxW = measureDoc.getTextWidth('Name');
            rows.forEach(function (r) {
                var w = measureDoc.getTextWidth(r.employeeName || '');
                if (w > maxW) maxW = w;
            });
            var nameWidth = Math.max(Math.ceil(maxW) + 10, 55);
            headCols[2].styles.cellWidth = nameWidth;
            return nameWidth;
        }

        // ── Sum of all numeric column widths ─────────────────────────
        function sumColWidths(headCols) {
            var total = 0;
            headCols.forEach(function (c) {
                var w = c.styles && c.styles.cellWidth;
                if (typeof w === 'number') total += w;
            });
            return total;
        }

        // ── Scale fixed columns so table fills the usable page width ─
        // The Name column (index 2) grows/shrinks with the rest.
        // Guarantees left & right margins are equal on the chosen orientation.
        function scaleColumnsToFit(headCols, usableWidth) {
            var currentTotal = sumColWidths(headCols);
            if (currentTotal <= 0) return;
            var ratio = usableWidth / currentTotal;
            headCols.forEach(function (c) {
                if (typeof c.styles.cellWidth === 'number') {
                    // Round to 2 dp; keep a sensible minimum
                    c.styles.cellWidth = Math.max(Math.round(c.styles.cellWidth * ratio * 100) / 100, 18);
                }
            });
        }

        // ── Always A4 Portrait — scale columns to fit usable width ────
        // Returns the usable width used (pt).
        function resolveLayout(headCols) {
            scaleColumnsToFit(headCols, A4_PORTRAIT_USABLE);
            return { orientation: 'portrait', usable: A4_PORTRAIT_USABLE };
        }

        // ── PDF Builder ──────────────────────────────────────────────
        function buildPdf(data, forPreview) {
            
            getImageBase64('/images/DP_logo.png', function (b64Logo, natW, natH) {
                var LOGO_H = 25;
                var logoW = natH > 0 ? (natW / natH) * LOGO_H : LOGO_H;

                var company = data.header.companyName || "DataPath Ltd.";
                var dataDate = data.header.dataDate || "";
                var rptType = data.header.reportType || "Present";
                var printDT = formatPrintDT();

                var reportTitle = REPORT_TITLES[rptType] || "Daily Attendance Details";
                var legText = LEGENDS[rptType] || "";

                var cfg = getReportConfig(rptType, data);
                var rows = cfg.rows;
                var headCols = cfg.headCols;
                var bodyMapper = cfg.bodyMapper;
                var statusColIdx = cfg.statusColIdx;
                var colCount = headCols.length;

                var { jsPDF } = window.jspdf;

                // Step 1 — measure Name column with a throwaway doc
                var measureDoc = new jsPDF({ unit: 'pt', format: 'a4' });
                fitNameColumn(measureDoc, headCols, rows);

                // Step 2 — decide orientation & scale ALL columns to fill the page
                var layout = resolveLayout(headCols);
                var orientation = layout.orientation;
                var usableWidth = layout.usable;       // already applied to headCols

                // Step 3 — create the real document
                var doc = new jsPDF({ orientation: orientation, unit: 'pt', format: 'a4' });

                var PW = doc.internal.pageSize.getWidth();
                var PH = doc.internal.pageSize.getHeight();

                var LEFT_MARGIN = 25;
                var RIGHT_MARGIN = 25;
                var TOP_MARGIN = 68;
                var BOTTOM_MARGIN = 35;

                // Table starts at LEFT_MARGIN and spans exactly usableWidth
                // so right edge = LEFT_MARGIN + usableWidth = PW - RIGHT_MARGIN
                var tableRightX = LEFT_MARGIN + usableWidth;

                // ── Header — identical on every page ──────────────────
                function drawHeader() {
                    if (b64Logo) doc.addImage(b64Logo, 'PNG', LEFT_MARGIN, 15, logoW, LOGO_H);
                    doc.setFont("times", "bold");
                    doc.setFontSize(15);
                    doc.text(company, PW / 2, 28, { align: 'center' });
                    doc.setFont("times", "normal");
                    doc.setFontSize(11);
                    doc.text(reportTitle, PW / 2, 44, { align: 'center' });
                    var tw = doc.getTextWidth(reportTitle);
                    doc.setLineWidth(0.5);
                    doc.line((PW - tw) / 2, 47, (PW + tw) / 2, 47);
                    doc.setFontSize(9);
                    const date = new Date(dataDate);

                    const dateOnly = date.toLocaleDateString('en-GB'); // dd/MM/yyyy

                    doc.text("Date: " + dateOnly, PW / 2, 58, { align: 'center' });
                }

                // ── Footer — identical on every page ──────────────────
                function drawFooter(pageNumber) {
                   
                    var actualRightX = tableRightX;
                    if (doc.lastAutoTable && doc.lastAutoTable.table) {
                        actualRightX = doc.lastAutoTable.table.startX + doc.lastAutoTable.table.width;
                    }
                    // doc.setFont("times", "normal");
                    doc.setFont("times", "normal");
                    doc.setFontSize(5.5);
                    doc.setTextColor(26, 26, 26);
                    doc.text('Print Datetime: ' + printDT, LEFT_MARGIN, PH - 15);
                    doc.text(legText, PW / 2, PH - 27, { align: 'center', maxWidth: PW * 0.50 });
                    doc.text('Page ' + pageNumber + ' of {total_pages_count_string}', actualRightX+55, PH - 15, { align: 'right' });
                    doc.setTextColor(26, 26, 26);
                }

                

                drawHeader();

                // ── One autoTable per department ───────────────────────
                var deptNames = [...new Set(rows.map(function (r) { return r.departmentName; }))];
                var currentY = TOP_MARGIN;

                deptNames.forEach(function (dept) {
                    var deptRows = rows.filter(function (r) { return r.departmentName === dept; });
                    var sn = 1;
                    var body = deptRows.map(function (r) { return bodyMapper(r, sn++); });

                    doc.autoTable({
                        startY: currentY,
                        tableWidth: usableWidth,          
                        margin: { left: LEFT_MARGIN, right: RIGHT_MARGIN, top: TOP_MARGIN, bottom: BOTTOM_MARGIN },
                        theme: 'grid',
                        head: [
                            [{
                                content: 'Department: ' + dept,
                                colSpan: colCount,
                                styles: {
                                    halign: 'left',
                                    valign: 'middle',
                                    fontStyle: 'bold',
                                    fontSize: 8,
                                    font: 'times',
                                    textColor: [0, 0, 0],
                                    lineColor: [255, 255, 255],
                                    lineWidth: 0,
                                    fillColor: [255, 255, 255],
                                    cellPadding: { top: 3, right: 3, bottom: 3, left: 0 }
                                }
                            }],
                            headCols
                        ],
                        body: body,
                        styles: {
                            font: 'times',
                            fontSize: 6,
                            cellPadding: 3,
                            halign: 'center',
                            valign: 'middle',
                            textColor: [0, 0, 0],
                            lineColor: [0, 0, 0],
                            lineWidth: 0.25,
                            fillColor: [255, 255, 255]
                        },
                        headStyles: {
                            font: 'times',
                            fontStyle: 'normal',
                            halign: 'center',
                            valign: 'middle',
                            fontSize: 7,
                            textColor: [0, 0, 0],
                            lineColor: [0, 0, 0],
                            lineWidth: 0.25,
                            fillColor: [255, 255, 255]
                        },
                        columnStyles: {},
                        didParseCell: function (cellData) {
                            if (cellData.section === 'body' && (cellData.column.index === 2 || cellData.column.index === 3)) {
                                cellData.cell.styles.halign = 'left';
                            }
                        },
                        willDrawCell: function (cellData) {
                            if (cellData.section === 'body' && cellData.column.index === statusColIdx) {
                                var rgb = statusColor(cellData.cell.raw);
                                doc.setTextColor(rgb[0], rgb[1], rgb[2]);
                            }
                        },
                        didDrawPage: function () {
                            drawHeader();
                            drawFooter(doc.internal.getCurrentPageInfo().pageNumber);
                        }
                    });

                    currentY = doc.lastAutoTable.finalY + 6;
                });

                if (typeof doc.putTotalPages === 'function')
                    doc.putTotalPages('{total_pages_count_string}');

                var fileName = 'Daily' + rptType + 'Report_' +
                    new Date().toISOString().slice(0, 10).replace(/-/g, '') + '.pdf';

                if (forPreview) {
                    var blob = doc.output('blob');
                    var url = URL.createObjectURL(blob);
                    $('#pdf-preview-container')
                        .html('<iframe src="' + url + '" width="100%" height="100%" style="border:none;"></iframe>')
                        .show();
                } else {
                    doc.save(fileName);
                }
            });
        }

        // ── Excel Download ───────────────────────────────────────────
        function downloadExcel() {
            showLoading();
            $.ajax({
                url: DownloadExcelUrl,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(getFilter()),
                xhrFields: { responseType: 'blob' },
                success: function (blob, status, xhr) {
                    hideLoading();

                    // If the server returned JSON (error), show the message
                    if (blob.type && blob.type.indexOf('application/json') !== -1) {
                        var reader = new FileReader();
                        reader.onload = function () {
                            try {
                                var err = JSON.parse(reader.result);
                                alert(err.message || 'Excel download failed!');
                            } catch (e) {
                                alert('Excel download failed!');
                            }
                        };
                        reader.readAsText(blob);
                        return;
                    }

                    // Determine filename from Content-Disposition header if present
                    var cd = xhr.getResponseHeader('Content-Disposition');
                    var ts = new Date().toISOString().replace(/[-T:.Z]/g, '').slice(0, 15);
                    var fileName = 'DailyAttendance_' + ts + '.xlsx';
                    if (cd) {
                        var match = cd.match(/filename[^;=\n]*=(['"]?)([^'";\n]+)\1/);
                        if (match && match[2]) fileName = match[2];
                    }

                    var link = document.createElement('a');
                    link.href = URL.createObjectURL(blob);
                    link.download = fileName;
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                    URL.revokeObjectURL(link.href);
                },
                error: function (xhr) {
                    hideLoading();
                    var msg = 'Excel download failed!';
                    try {
                        var err = JSON.parse(xhr.responseText);
                        if (err.message) msg = err.message;
                    } catch (e) { }
                    alert(msg);
                }
            });
        }

        // ── Button Events ────────────────────────────────────────────
        $(document).on('click', '#btnPreviewPdf', function () {
            loadData(function (data) { buildPdf(data, true); });
        });

        $(document).on('click', '#downloadReport', function () {
            var fmt = $("#reportText").val();
            if (fmt === "downloadPdf") {
                loadData(function (data) { buildPdf(data, false); });
            } else if (fmt === "downloadExcel") {
                downloadExcel();
            } else {
                alert("Please select a report format.");
            }
        });

        // ── Init ─────────────────────────────────────────────────────
        settings.load();
    };
})(jQuery);