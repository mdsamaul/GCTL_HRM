(function ($) {
    $.patientTypes = function (options) {
        var settings = $.extend({
            baseUrl: "/",
        }, options);

        var GetSummaryUrl = settings.baseUrl + "/GetSummary";

        // ─── Loading Overlay ───────────────────────────────────────
        var setupLoadingOverlay = function () {
            if ($("#customLoadingOverlay").length === 0) {
                $("body").append(`
                    <div id="customLoadingOverlay" style="
                        display:none; position:fixed; top:0; left:0;
                        width:100%; height:100%;
                        background-color:rgba(0,0,0,0.5);
                        z-index:9999; justify-content:center; align-items:center;">
                        <div style="background:#fff; padding:20px; border-radius:5px;
                                    box-shadow:0 0 10px rgba(0,0,0,0.3); text-align:center;">
                            <div class="spinner-border text-primary" role="status">
                                <span class="sr-only">Loading...</span>
                            </div>
                            <p style="margin-top:10px; margin-bottom:0;">Loading data...</p>
                        </div>
                    </div>`);
            }
        };

        function showLoading() { $("#customLoadingOverlay").css("display", "flex"); }
        function hideLoading() { $("#customLoadingOverlay").hide(); }

        // ─── Filter value ──────────────────────────────────────────
        function getFilterValue() {
            var deptVal = $("#departmentSelect").val();
            var deptArr = [];
            if (deptVal) {
                deptArr = Array.isArray(deptVal) ? deptVal : [deptVal];
                deptArr = deptArr.filter(function (v) { return v && v !== ""; });
            }
            return {
                CompanyCode: $("#companySelect").val() || null,
                DepartmentCodes: deptArr.length > 0 ? deptArr : null,
                FromDate: $("#DateSelect").val() || null
            };
        }

        flatpickr($("#DateSelect"), CalendarService.createConfig(
            {
                defaultDate: new Date(),
            }
        ));
        // ─── Load data from server ─────────────────────────────────
        function loadSummaryData(callback) {
            showLoading();
            var filter = getFilterValue();
            $.ajax({
                url: GetSummaryUrl,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(filter),
                success: function (res) {
                    hideLoading();
                    if (res.success && res.data) {
                        if (typeof callback === 'function') callback(res.data);
                    } else {
                        alert(res.message || 'No data found.');
                    }
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data.');
                }
            });
        }

        // ─── Helpers ───────────────────────────────────────────────
        function formatDateTime(d) {
            const dd = String(d.getDate()).padStart(2, '0');
            const mm = String(d.getMonth() + 1).padStart(2, '0');
            const yyyy = d.getFullYear();
            let h = d.getHours();
            const min = String(d.getMinutes()).padStart(2, '0');
            const sec = String(d.getSeconds()).padStart(2, '0');
            const ampm = h >= 12 ? 'PM' : 'AM';
            h = h % 12 || 12;
            return `${dd}/${mm}/${yyyy} ${String(h).padStart(2, '0')}:${min}:${sec} ${ampm}`;
        }

        // base64 + natural width/height একসাথে return করে (sync-safe)
        // callback(base64, naturalWidth, naturalHeight)
        function getImageBase64FromUrl(url, callback) {
            var img = new Image();
            img.crossOrigin = 'Anonymous';
            img.onload = function () {
                var canvas = document.createElement('canvas');
                canvas.width = img.naturalWidth;
                canvas.height = img.naturalHeight;
                canvas.getContext('2d').drawImage(img, 0, 0);
                callback(canvas.toDataURL('image/png'), img.naturalWidth, img.naturalHeight);
            };
            img.onerror = function () { callback(null, 0, 0); };
            img.src = url;
        }

        // ─── Build PDF ─────────────────────────────────────────────
        function buildPdf(data, forPreview) {

            // naturalWidth / naturalHeight callback এ পাচ্ছি
            getImageBase64FromUrl('/images/DP_logo.png', function (base64Logo, natW, natH) {

               
                var LOGO_TARGET_HEIGHT = 20;  
                var logoWidth = (natH > 0)
                    ? (natW / natH) * LOGO_TARGET_HEIGHT
                    : LOGO_TARGET_HEIGHT;    

                const { jsPDF } = window.jspdf;
                const doc = new jsPDF({
                    orientation: 'portrait',
                    unit: 'pt',
                    format: 'a4'
                });

                const pageWidth = doc.internal.pageSize.getWidth();
                const pageHeight = doc.internal.pageSize.getHeight();
                const companyName = data.companyName || "DataPath Ltd.";
                const reportTitle = "Daily Attendance Summary Report";
                const dataDate = data.dataDate || "";
                const printDT = formatDateTime(new Date());

                
                function drawHeader() {
                    if (base64Logo) {
                        // x=15, y=12 fixed; width=auto, height=LOGO_TARGET_HEIGHT
                        doc.addImage(base64Logo, 'PNG', 30,25, logoWidth, LOGO_TARGET_HEIGHT);
                    }

                    doc.setFontSize(16);
                    doc.setFont("times", "bold");
                    doc.text(companyName, pageWidth / 2, 32, { align: 'center' });

                    doc.setFontSize(12);
                    doc.setFont("times", "normal");
                    doc.text(reportTitle, pageWidth / 2, 50, { align: 'center' });

                    const tw = doc.getTextWidth(reportTitle);
                    doc.setDrawColor(0);
                    doc.setLineWidth(0.5);
                    doc.line((pageWidth - tw) / 2, 53, (pageWidth + tw) / 2, 53);

                    doc.setFontSize(10);
                    doc.text("Date: " + dataDate, pageWidth / 2, 68, { align: 'center' });
                }

                drawHeader();

                var tableBody = data.departments.map(function (d) {
                    return [
                        d.departmentName,
                        d.noOfEmps,
                        d.presentCount,
                        d.lateCount,
                        d.leaveCount,
                        d.absentCount
                    ];
                });

                doc.autoTable({
                    head: [[
                        { content: 'Department', styles: { halign: 'center' } },
                        { content: 'No.of Emps', styles: { halign: 'center' } },
                        { content: 'Present', styles: { halign: 'center' } },
                        { content: 'Late', styles: { halign: 'center' } },
                        { content: 'Leave', styles: { halign: 'center' } },
                        { content: 'Absent', styles: { halign: 'center' } }
                    ]],
                    body: tableBody,
                    foot: [[
                        { content: '', styles: { halign: 'left' } },
                        { content: String(data.totalNoOfEmps), styles: { halign: 'center', fontStyle: 'bold' } },
                        { content: String(data.totalPresent), styles: { halign: 'center', fontStyle: 'bold' } },
                        { content: String(data.totalLate), styles: { halign: 'center', fontStyle: 'bold' } },
                        { content: String(data.totalLeave), styles: { halign: 'center', fontStyle: 'bold' } },
                        { content: String(data.totalAbsent), styles: { halign: 'center', fontStyle: 'bold' } }
                    ]],
                    startY: 80,
                    margin: { left: 30, right: 30 },
                    theme: 'grid',
                    styles: {
                        fontSize: 9,
                        cellPadding: 4,
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
                    footStyles: {
                        fillColor: [255, 255, 255],
                        textColor: [0, 0, 0],
                        lineColor: [0, 0, 0],
                        lineWidth: 0.3
                    },
                    columnStyles: {
                        0: { cellWidth: 'auto', halign: 'left', valign: 'middle' },
                        1: { cellWidth: 60, halign: 'center', valign: 'middle' },
                        2: { cellWidth: 55, halign: 'center', valign: 'middle' },
                        3: { cellWidth: 50, halign: 'center', valign: 'middle' },
                        4: { cellWidth: 50, halign: 'center', valign: 'middle' },
                        5: { cellWidth: 50, halign: 'center', valign: 'middle' }
                    },
                    didDrawPage: function () {
                        drawHeader();
                        const pageNum = doc.internal.getCurrentPageInfo().pageNumber;
                        const totalPg = '{total_pages_count_string}';
                        doc.setFontSize(8);
                        doc.setFont("times", "normal");
                        doc.setTextColor(80, 80, 80);
                        doc.text('Print Datetime:  ' + printDT, 30, pageHeight - 12);
                        doc.text('GCTL- Human Resource Management', pageWidth / 2, pageHeight - 12, { align: 'center' });
                        doc.text('Page ' + pageNum + ' of ' + totalPg, pageWidth - 30, pageHeight - 12, { align: 'right' });
                    }
                });

                if (typeof doc.putTotalPages === 'function') {
                    doc.putTotalPages('{total_pages_count_string}');
                }

                if (forPreview) {
                    const blob = doc.output('blob');
                    const url = URL.createObjectURL(blob);
                    $('#pdf-preview-container')
                        .html(`<iframe src="${url}" width="100%" height="100%" style="border:1px solid #ccc;"></iframe>`)
                        .show();
                } else {
                    doc.save('DailyAttendanceSummaryReport.pdf');
                }
            });
        }

        // ─── Button events ─────────────────────────────────────────
        // $(document).on('click', '#downloadReport', function () {
        //     var fmt = $("#reportText").val();
        //     if (fmt === "downloadPdf") {
        //         loadSummaryData(function (data) { buildPdf(data, false); });
        //     } else {
        //         alert("Only PDF export is available for this report.");
        //     }
        // });

        $(document).on('click', '#downloadReport', function () {
            var fmt = $("#reportText").val();
            if (fmt === "downloadPdf") {
                loadSummaryData(function (data) { buildPdf(data, false); });
            } else if (fmt === "downloadExcel") {
                downloadExcel(); 
            } else {
                alert("Please select a report format.");
            }
        });

        // ─── Excel Download ────────────────────────────────────────────
        function downloadExcel() {
            showLoading();
            var filter = getFilterValue();

            $.ajax({
                url: settings.baseUrl + "/DownloadExcel",
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(filter),
                xhrFields: { responseType: 'blob' },
                success: function (blob) {
                    hideLoading();
                    var now = new Date();
                    var ts = now.getFullYear().toString() +
                        String(now.getMonth() + 1).padStart(2, '0') +
                        String(now.getDate()).padStart(2, '0') + '_' +
                        String(now.getHours()).padStart(2, '0') +
                        String(now.getMinutes()).padStart(2, '0') +
                        String(now.getSeconds()).padStart(2, '0');

                    var link = document.createElement('a');
                    link.href = URL.createObjectURL(blob);
                    link.download = 'DailyAttendanceSummaryReport_' + ts + '.xlsx';
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                    URL.revokeObjectURL(link.href);
                },
                error: function () {
                    hideLoading();
                    alert('Excel download failed!');
                }
            });
        }

        $(document).on('click', '#btnPreviewPdf', function () {
            loadSummaryData(function (data) { buildPdf(data, true); });
        });

        // ─── Init ──────────────────────────────────────────────────
        var init = function () {
            setupLoadingOverlay();
            settings.load();
        };
        init();
    };
})(jQuery);