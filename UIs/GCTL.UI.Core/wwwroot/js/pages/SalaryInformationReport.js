(function ($) {
    $.salaryInformationReport = function (options) {
        console.log("loaded");
        var settings = $.extend({
            baseUrl: '',
            load: function () { }
        }, options);

        var lastResult = []; // cached rows for PDF export

        function getSelectedValues(id) {
            var v = $(id).val();
            if (!v) return null;
            return Array.isArray(v) ? v.join(',') : v;
        }

        function buildFilter() {
            var generateType = $('input[name="salaryGenerate"]:checked').val();

            return {
                CompanyCode: getSelectedValues('#companySelect'),
                BranchCode: getSelectedValues('#branchSelect'),
                DepartmentCode: getSelectedValues('#departmentSelect'),
                EmployeeID: getSelectedValues('#employeeSelect'),
                ModeOfPayment: getSelectedValues('#modeOfPaymentSelect'),
                EmploymentNature: getSelectedValues('#employmentNatureSelect'),

                GenerateType: generateType,

                DateFrom: generateType === 'ByDate' ? ($('#dateFrom').val() || null) : null,

                DateTo: generateType === 'ByDate' ? ($('#dateTo').val() || null) : null,
                MonthName: generateType === 'ByMonth' ? $('#monthSelect option:selected').text() : null,
                YearName: generateType === 'ByMonth' ? parseInt($('#yearSelect').val(), 10) : null,

                AsOnDate: null,
                ExportFormat: $('#exportFormatSelect').val(),
                MasterFileType: $('#masterFileTypeSelect').val() || null
            };
        }

        function showLoading() {
            if (window.$.blockUI) { $.blockUI(); }
        }
        function hideLoading() {
            if (window.$.unblockUI) { $.unblockUI(); }
        }

        function toast(msg, isError) {
            if (window.toastr) {
                isError ? toastr.error(msg) : toastr.success(msg);
            } else {
                alert(msg);
            }
        }

        // ── Date picker ──────────────────────────────────────────────
        flatpickr("#dateFrom, #dateTo", CalendarService.createConfig({ defaultDate: new Date() }));
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
            img.onerror = function () {
                callback(null, 0, 0);
            };
            img.src = url;
        }

        function buildPdfDoc(filter, rows, callback) {
            getImageBase64FromUrl('/images/DP_logo.png', function (base64Logo, natW, natH) {

                var LOGO_TARGET_HEIGHT = 26;
                var logoWidth = (natH > 0) ? (natW / natH) * LOGO_TARGET_HEIGHT : LOGO_TARGET_HEIGHT;

                var jsPDFCtor = window.jspdf ? window.jspdf.jsPDF : window.jsPDF;
                var doc = new jsPDFCtor({
                    orientation: 'landscape',
                    unit: 'pt',
                    format: [1085, 650]
                });

                var pageWidth = doc.internal.pageSize.getWidth();
                var pageHeight = doc.internal.pageSize.getHeight();
                var leftMargin = 12;
                var rightMargin = 12;
                var topMargin = 70;
                var bottomMargin = 40;

                // ===== Header =====
                function drawHeader() {
                    if (base64Logo) {
                        doc.addImage(base64Logo, 'PNG', 14, 20, logoWidth, LOGO_TARGET_HEIGHT);
                    }

                    doc.setFont("times", "bold");
                    doc.setFontSize(13);
                    doc.text('DataPath Ltd.', pageWidth / 2, 22, { align: 'center' });

                    doc.setFontSize(11);
                    doc.text('Payroll Master File - General', pageWidth / 2, 38, { align: 'center' });

                    doc.setFontSize(8);
                    doc.setFont("times", "italic");
                    var periodText = filter.GenerateType === 'ByMonth'
                        ? 'For the month of ' + filter.MonthName + ', ' + filter.YearName
                        : 'For the period ' + (formatDateDMY(filter.DateFrom) || '') + ' to ' + (formatDateDMY(filter.DateTo) || '');
                    doc.text(periodText, pageWidth / 2, 52, { align: 'center' });
                }

                // ===== Footer =====
                function drawFooter() {
                    var pageCount = doc.internal.getNumberOfPages();
                    var currentPage = doc.internal.getCurrentPageInfo().pageNumber;

                    var now = new Date();
                    var hours = now.getHours();
                    var minutes = now.getMinutes().toString().padStart(2, '0');
                    var ampm = hours >= 12 ? 'PM' : 'AM';
                    hours = hours % 12 || 12;
                    var day = now.getDate().toString().padStart(2, '0');
                    var month = (now.getMonth() + 1).toString().padStart(2, '0');
                    var year = now.getFullYear();
                    var printDateTime = day + '/' + month + '/' + year + ' ' + hours + ':' + minutes + ' ' + ampm;

                    doc.setFont("times", "normal");
                    doc.setFontSize(8);
                    doc.setTextColor(80);

                    doc.text('Print Datetime: ' + printDateTime, leftMargin, pageHeight - 18);
                    doc.text('GCTL Infosys - HRM & Finance System', pageWidth / 2, pageHeight - 18, { align: 'center' });
                    doc.text('Page ' + currentPage + ' of ' + pageCount, pageWidth - rightMargin, pageHeight - 18, { align: 'right' });

                    doc.setTextColor(0);
                }

                // ===== Table Data =====
                var head = [[
                    'SL', 'ID NO.', 'Pay ID', 'DP User ID', 'DBBL Employees Name',
                    'UCBL Employees Name', 'Status', 'DEPARTMENT', 'DESIGNATION',
                    'DOH', 'DOT', 'Duration', 'DBBL', 'UCBL', 'Salary',
                    'Yearly Bonus', 'Gratuity', 'Eid Bonus', 'PF Elig.', 'Gender',
                    'Cell Phone', 'Special Notes', 'End of Probation'
                ]];

                var body = rows.map(function (row) {
                    return [
                        row.sl, row.idNo, row.payId, row.dpUserId, row.dbblEmployeesName,
                        row.ucblEmployeesName, row.status, row.department, row.designation,
                        row.doh, row.dot, row.duration, row.dbbl, row.ucbl, row.salary,
                        row.yearlyBonusEligibility, row.gratuityEligibility, row.eidBonusEligibility,
                        row.pfEligiblity, row.gender, row.cellPhone, row.specialNotes,
                        row.endOfProbation
                    ];
                });

                var totalSalary = rows.reduce(function (sum, row) {
                    return sum + (parseFloat(row.salary) || 0);
                }, 0);

                // ===== Fixed Column Widths (Scale নেই) =====
                var columnStyles = {
                    0: { cellWidth: 22, halign: 'center' },  // SL
                    1: { cellWidth: 42, halign: 'center' },  // ID NO
                    2: { cellWidth: 32, halign: 'center' },  // Pay ID
                    3: { cellWidth: 48 },                     // DP User ID
                    4: { cellWidth: 90 },                     // DBBL Name
                    5: { cellWidth: 90 },                     // UCBL Name
                    6: { cellWidth: 32, halign: 'center' },  // Status
                    7: { cellWidth: 55 },                     // Department
                    8: { cellWidth: 55 },                     // Designation
                    9: { cellWidth: 42, halign: 'center' },  // DOH
                    10: { cellWidth: 42, halign: 'center' },  // DOT
                    11: { cellWidth: 32, halign: 'center' },  // Duration
                    12: { cellWidth: 60, halign: 'center' },     // DBBL
                    13: { cellWidth: 50, halign: 'center' },  // UCBL
                    14: { cellWidth: 58, halign: 'right' },  // Salary (বড়)
                    15: { cellWidth: 38, halign: 'center' },  // Yearly Bonus
                    16: { cellWidth: 32, halign: 'center' },  // Gratuity
                    17: { cellWidth: 32, halign: 'center' },  // Eid Bonus
                    18: { cellWidth: 32, halign: 'center' },  // PF Elig
                    19: { cellWidth: 32, halign: 'center' },  // Gender
                    20: { cellWidth: 50, halign: 'center' },                     // Cell Phone
                    21: { cellWidth: 48, halign: 'center' },                     // Special Notes
                    22: { cellWidth: 48, halign: 'center' }   // End of Probation
                };

                // Foot (Total merge)
                var footRow = [
                    {
                        content: 'Total',
                        colSpan: 14,
                        styles: { halign: 'left', fontStyle: 'bold', fontSize: 7 }
                    },
                    {
                        content: totalSalary.toFixed(),
                        styles: { halign: 'right', fontStyle: 'bold', fontSize: 7 }
                    }
                ];

                // ===== autoTable =====
                doc.autoTable({
                    head: head,
                    body: body,
                    startY: topMargin,
                    styles: {
                        font: "times",
                        fontSize: 6.5,
                        textColor: 0,
                        cellPadding: 1.5,
                        valign: 'middle',
                        overflow: 'linebreak',
                        lineWidth: 0.3,
                        lineColor: [0, 0, 0]
                    },
                    headStyles: {
                        font: "times",
                        fillColor: false,
                        textColor: 0,
                        fontStyle: "bold",
                        fontSize: 7,
                        halign: 'center',
                        valign: 'middle',
                        lineWidth: 0.3,
                        lineColor: [0, 0, 0]
                    },
                    columnStyles: columnStyles,
                    tableWidth: 'wrap',                 // ★ important
                    foot: [footRow],
                    footStyles: {
                        font: "times",
                        fillColor: false,
                        fontStyle: "bold",
                        fontSize: 7,
                        lineWidth: 0.3,
                        lineColor: [0, 0, 0]
                    },
                    margin: {
                        top: topMargin,
                        bottom: bottomMargin,
                        left: leftMargin,
                        right: rightMargin
                    },
                    didDrawPage: function () {
                        drawHeader();
                        drawFooter();
                    }
                });

                if (typeof callback === 'function') {
                    callback(doc);
                }
            });
        }
        // ---------- Excel download (server-side EPPlus, binary via ajax) ----------
        function exportExcel(filter) {

            showLoading();
            $.ajax({
                url: settings.baseUrl + '/ExportToExcel',
                type: 'POST',
                data: JSON.stringify(filter),
                contentType: 'application/json',
                xhrFields: {
                    responseType: 'blob'
                },
                success: function (data, textStatus, jqXHR) {
                    hideLoading();

                    var contentType = jqXHR.getResponseHeader('Content-Type') || '';

                    if (contentType.indexOf('application/json') !== -1 || contentType.indexOf('text/html') !== -1) {
                        var reader = new FileReader();
                        reader.onload = function () {
                            try {
                                var errObj = JSON.parse(reader.result);
                                alert(errObj.message || 'Failed to generate Excel file');
                            } catch (e) {
                                alert('Failed to generate Excel file');
                            }
                        };
                        reader.readAsText(data);
                        return;
                    }

                    if (!data || data.size === 0) {
                        alert('No data found to download');
                        return;
                    }

                    var url = window.URL.createObjectURL(data);
                    var a = document.createElement('a');
                    a.href = url;
                    a.download = 'PayrollMasterFile_General_' + Date.now() + '.xlsx';
                    document.body.appendChild(a);
                    a.click();
                    a.remove();
                    window.URL.revokeObjectURL(url);
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data');
                }
            });
        }

        // ---------- PDF export (client-side, jsPDF + autotable) ----------
        function exportPdf(filter) {
            showLoading();
            $.ajax({
                url: settings.baseUrl + '/GetPayrollMasterFile',
                type: 'POST',
                data: JSON.stringify(filter),
                contentType: 'application/json',
                success: function (res) {
                    hideLoading();
                    if (!res.success) {
                        alert(res.message || 'Failed to load data');
                        return;
                    }
                    lastResult = res.data || [];
                    if (lastResult.length === 0) {
                        alert('No data found to download');
                        return;
                    }

                    // ★ callback দিয়ে কল করুন
                    buildPdfDoc(filter, lastResult, function (doc) {
                        doc.save('PayrollMasterFile_General_' + Date.now() + '.pdf');
                    });
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data');
                }
            });
        }
        // ---------- PDF preview (renders inline into #pdf-preview-container) ----------
        function previewPdf(filter) {
            showLoading();
            $.ajax({
                url: settings.baseUrl + '/GetPayrollMasterFile',
                type: 'POST',
                data: JSON.stringify(filter),
                contentType: 'application/json',
                success: function (res) {
                    hideLoading();
                    if (!res.success) {
                        alert(res.message || 'Failed to load data');
                        return;
                    }
                    lastResult = res.data || [];
                    if (lastResult.length === 0) {
                        alert('No data found to preview');
                        return;
                    }

                    // ★ callback দিয়ে কল করুন
                    buildPdfDoc(filter, lastResult, function (doc) {
                        var blobUrl = doc.output('bloburl');
                        var $container = $('#pdf-preview-container');
                        $container.empty();
                        var $iframe = $('<iframe>', {
                            src: blobUrl,
                            style: 'width:100%; height:100%; border:0;'
                        });
                        $container.append($iframe).show();
                        $container[0].scrollIntoView({ behavior: 'smooth', block: 'start' });
                    });
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data');
                }
            });
        }

        function exportExcelGratuity(filter) {
            showLoading();
            $.ajax({
                url: settings.baseUrl + '/ExportToExcelGratuity',
                type: 'POST',
                data: JSON.stringify(filter),
                contentType: 'application/json',
                xhrFields: { responseType: 'blob' },
                success: function (data, textStatus, jqXHR) {
                    hideLoading();
                    // same blob handling as your existing exportExcel
                    var url = window.URL.createObjectURL(data);
                    var a = document.createElement('a');
                    a.href = url;
                    a.download = 'PayrollMasterFile_Gratuity_' + Date.now() + '.xlsx';
                    document.body.appendChild(a);
                    a.click();
                    a.remove();
                    window.URL.revokeObjectURL(url);
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data');
                }
            });
        }

        function exportPdfGratuity(filter) {
            showLoading();
            $.ajax({
                url: settings.baseUrl + '/GetPayrollMasterFileGratuity',
                type: 'POST',
                data: JSON.stringify(filter),
                contentType: 'application/json',
                success: function (res) {
                    hideLoading();
                    if (!res.success) {
                        alert(res.message || 'Failed to load data');
                        return;
                    }
                    var rows = res.data || [];
                    if (rows.length === 0) {
                        alert('No data found to download');
                        return;
                    }

                    buildPdfDocGratuity(filter, rows, function (doc) {
                        doc.save('PayrollMasterFile_Gratuity_' + Date.now() + '.pdf');
                    });
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data');
                }
            });
        }
        function previewPdfGratuity(filter) {
            showLoading();
            $.ajax({
                url: settings.baseUrl + '/GetPayrollMasterFileGratuity',
                type: 'POST',
                data: JSON.stringify(filter),
                contentType: 'application/json',
                success: function (res) {
                    hideLoading();
                    if (!res.success) {
                        alert(res.message || 'Failed to load data');
                        return;
                    }
                    var rows = res.data || [];
                    if (rows.length === 0) {
                        alert('No data found to preview');
                        return;
                    }

                    buildPdfDocGratuity(filter, rows, function (doc) {
                        var blobUrl = doc.output('bloburl');
                        var $container = $('#pdf-preview-container');
                        $container.empty();
                        var $iframe = $('<iframe>', {
                            src: blobUrl,
                            style: 'width:100%; height:100%; border:0;'
                        });
                        $container.append($iframe).show();
                        $container[0].scrollIntoView({ behavior: 'smooth', block: 'start' });
                    });
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data');
                }
            });
        }
        function buildPdfDocGratuity(filter, rows, callback) {
            getImageBase64FromUrl('/images/DP_logo.png', function (base64Logo, natW, natH) {

                var LOGO_TARGET_HEIGHT = 26;
                var logoWidth = (natH > 0) ? (natW / natH) * LOGO_TARGET_HEIGHT : LOGO_TARGET_HEIGHT;

                var jsPDFCtor = window.jspdf ? window.jspdf.jsPDF : window.jsPDF;
                var doc = new jsPDFCtor({
                    orientation: 'portrait',
                    unit: 'pt',
                    format: 'a4'
                });

                var pageWidth = doc.internal.pageSize.getWidth();
                var pageHeight = doc.internal.pageSize.getHeight();
                var leftMargin = 18;
                var rightMargin = 18;
                var topMargin = 70;
                var bottomMargin = 40;

                // ===== Header =====
                function drawHeader() {
                    if (base64Logo) {
                        doc.addImage(base64Logo, 'PNG', 18, 18, logoWidth, LOGO_TARGET_HEIGHT);
                    }

                    doc.setFont("times", "bold");
                    doc.setFontSize(13);
                    doc.text('DataPath Ltd.', pageWidth / 2, 22, { align: 'center' });

                    doc.setFontSize(11);
                    doc.text('Payroll Master File - Gratuity', pageWidth / 2, 38, { align: 'center' });

                    doc.setFontSize(8);
                    doc.setFont("times", "italic");
                    var periodText = filter.GenerateType === 'ByMonth'
                        ? 'For the month of ' + filter.MonthName + ', ' + filter.YearName
                        : 'For the period ' + (formatDateDMY(filter.DateFrom) || '') + ' to ' + (formatDateDMY(filter.DateTo) || '');
                    doc.text(periodText, pageWidth / 2, 52, { align: 'center' });
                }

                // ===== Footer =====
                function drawFooter() {
                    var pageCount = doc.internal.getNumberOfPages();
                    var currentPage = doc.internal.getCurrentPageInfo().pageNumber;

                    var now = new Date();
                    var hours = now.getHours();
                    var minutes = now.getMinutes().toString().padStart(2, '0');
                    var ampm = hours >= 12 ? 'PM' : 'AM';
                    hours = hours % 12 || 12;
                    var day = now.getDate().toString().padStart(2, '0');
                    var month = (now.getMonth() + 1).toString().padStart(2, '0');
                    var year = now.getFullYear();
                    var printDateTime = day + '/' + month + '/' + year + ' ' + hours + ':' + minutes + ' ' + ampm;

                    doc.setFont("times", "normal");
                    doc.setFontSize(8);
                    doc.setTextColor(80);

                    doc.text('Print Datetime: ' + printDateTime, leftMargin, pageHeight - 18);
                    doc.text('GCTL Infosys - HRM & Finance System', pageWidth / 2, pageHeight - 18, { align: 'center' });
                    doc.text('Page ' + currentPage + ' of ' + pageCount, pageWidth - rightMargin, pageHeight - 18, { align: 'right' });

                    doc.setTextColor(0);
                }

                // ===== Table Headers =====
                var head = [[
                    'SL.', 'ID NO.', 'Pay ID', 'Name of the Employee', 'Status',
                    'DEPARTMENT', 'DOH', 'DOT', 'Bank Account No',
                    'Salary', 'Tenure', 'Gratuity', 'Note'
                ]];

                // ===== Table Body =====
                var body = rows.map(function (row) {
                    return [
                        row.sl,
                        row.idNo,
                        row.payId,
                        row.nameOfTheEmployee,
                        row.status,
                        row.department,
                        row.dateOfHire,
                        row.dot,
                        row.bankAccountNo,
                        row.salary != null ? parseFloat(row.salary).toFixed() : '',
                        row.tenure != null ? parseInt(row.tenure) : '',                    
                        row.gratuity != null ? parseFloat(row.gratuity).toFixed() : '', 
                        row.note
                    ];
                });

                // Total Salary
                var totalSalary = rows.reduce(function (sum, row) {
                    return sum + (parseFloat(row.salary) || 0);
                }, 0);
                var totalGratuity = rows.reduce(function (sum, row) {
                    return sum + (parseFloat(row.gratuity) || 0);
                }, 0);
                // Column widths
                var columnStyles = {
                    0: { cellWidth: 22, halign: 'center' }, // SL.
                    1: { cellWidth: 45, halign: 'center' }, // ID NO.
                    2: { cellWidth: 20, halign: 'center' }, // Pay ID
                    3: { cellWidth: 70 },                   // Name
                    4: { cellWidth: 35, halign: 'center' }, // Status
                    5: { cellWidth: 60 },                   // Department
                    6: { cellWidth: 45, halign: 'center' }, // DOH
                    7: { cellWidth: 40, halign: 'center' }, // DOT
                    8: { cellWidth: 65 },                   // Bank Account
                    9: { cellWidth: 45, halign: 'right' },  // Salary
                    10: { cellWidth: 30, halign: 'center' }, // Tenure
                    11: { cellWidth: 45, halign: 'right' },  // Gratuity
                    12: { cellWidth: 40 }                    // Note
                };

                // Footer Total row
                var footRow = [
                    {
                        content: 'Total',
                        colSpan: 9,                                   // SL থেকে Bank Account No পর্যন্ত
                        styles: { halign: 'left', fontStyle: 'bold', fontSize: 8 }
                    },
                    {
                        content: totalSalary.toFixed(),              // Salary Total
                        styles: { halign: 'right', fontStyle: 'bold', fontSize: 8 }
                    },
                    {
                        content: '',                                  // Tenure খালি
                        styles: { halign: 'center', fontStyle: 'bold', fontSize: 8 }
                    },
                    {
                        content: totalGratuity.toFixed(),            // ★ Gratuity Total
                        styles: { halign: 'right', fontStyle: 'bold', fontSize: 8 }
                    },
                    {
                        content: '',                                  // Note খালি
                        styles: {}
                    }
                ];

                // ===== autoTable =====
                doc.autoTable({
                    head: head,
                    body: body,
                    startY: topMargin,
                    styles: {
                        font: "times",
                        fontSize: 7.5,
                        textColor: 0,
                        cellPadding: 2,
                        valign: 'middle',
                        overflow: 'linebreak',
                        lineWidth: 0.3,
                        lineColor: [0, 0, 0]
                    },
                    headStyles: {
                        font: "times",
                        fillColor: false,
                        textColor: 0,
                        fontStyle: "bold",
                        fontSize: 8,
                        halign: 'center',
                        valign: 'middle',
                        lineWidth: 0.3,
                        lineColor: [0, 0, 0]
                    },
                    columnStyles: columnStyles,
                    tableWidth: 'auto',
                    foot: [footRow],
                    footStyles: {
                        font: "times",
                        fillColor: false,
                        fontStyle: "bold",
                        fontSize: 8,
                        lineWidth: 0.3,
                        lineColor: [0, 0, 0]
                    },
                    margin: {
                        top: topMargin,
                        bottom: bottomMargin,
                        left: leftMargin,
                        right: rightMargin
                    },
                    didDrawPage: function () {
                        drawHeader();
                        drawFooter();
                    }
                });

                if (typeof callback === 'function') {
                    callback(doc);
                }
            });
        }



        // // ---------- CSV export ----------
        // function exportCsv(filter) {
        //     console.log(filter);
        //     showLoading();
        //     $.ajax({
        //         url: settings.baseUrl + '/GetPayrollMasterFile',
        //         type: 'POST',
        //         data: JSON.stringify(filter),
        //         contentType: 'application/json',
        //         success: function (res) {
        //             hideLoading();

        //             if (!res.success || !res.data || !res.data.length) {
        //                 alert('No data found to download');
        //                 return;
        //             }

        //             var rows = res.data;
        //             var headers = Object.keys(rows[0]);
        //             var csv = [headers.join(',')]
        //                 .concat(rows.map(function (r) {
        //                     return headers.map(function (h) {
        //                         var v = r[h] == null ? '' : String(r[h]).replace(/"/g, '""');
        //                         return '"' + v + '"';
        //                     }).join(',');
        //                 }))
        //                 .join('\n');
        //             var blob = new Blob([csv], { type: 'text/csv' });
        //             var url = window.URL.createObjectURL(blob);
        //             var a = document.createElement('a');
        //             a.href = url;
        //             a.download = 'PayrollMasterFile_General_' + Date.now() + '.csv';
        //             document.body.appendChild(a);
        //             a.click();
        //             a.remove();
        //             window.URL.revokeObjectURL(url);
        //         },
        //         error: function () {
        //             hideLoading();
        //             alert('Failed to load data');
        //         }
        //     });
        // }

        // $('#btnExportReport').off('click').on('click', function () {
        //     var filter = buildFilter();
        //     var format = filter.ExportFormat;
        //     var masterVal = $("#masterFileTypeSelect").val();

        //     if (!masterVal || masterVal === "" || (Array.isArray(masterVal) && masterVal.length === 0)) {
        //         alert("Select Master File Type");
        //         $("#masterFileTypeSelect").select2('open');
        //         return;
        //     }
        //     // if (format === 'Excel') {
        //     //     exportExcel(filter);
        //     // } else if (format === 'PDF') {
        //     //     exportPdf(filter);
        //     // } else if (format === 'CSV') {
        //     //     exportCsv(filter);
        //     // }
        //     // Inside your existing $('#btnExportReport').click handler

        //     if (format === 'Excel') {
        //         if (masterVal === "01") {               // General
        //             exportExcel(filter);                // existing → /ExportToExcel
        //         } else if (masterVal === "02") {        // Gratuity (adjust ID as per your master table)
        //             exportExcelGratuity(filter);
        //         }
        //     } else if (format === 'PDF') {
        //         if (masterVal === "01") {
        //             exportPdf(filter);                  // existing
        //         } else if (masterVal === "02") {
        //             exportPdfGratuity(filter);
        //         }
        //     }
        // });

        // $('#btnPreviewPdf').off('click').on('click', function () {
        //     var filter = buildFilter();
        //     previewPdf(filter);
        // });

        // ===================== YEARLY BONUS - Excel =====================
        function exportExcelYearlyBonus(filter) {
            showLoading();
            $.ajax({
                url: settings.baseUrl + '/ExportToExcelYearlyBonus',
                type: 'POST',
                data: JSON.stringify(filter),
                contentType: 'application/json',
                xhrFields: { responseType: 'blob' },
                success: function (data) {
                    hideLoading();
                    if (!data || data.size === 0) {
                        alert('No data found to download');
                        return;
                    }
                    var url = window.URL.createObjectURL(data);
                    var a = document.createElement('a');
                    a.href = url;
                    a.download = 'YearlyBonus_Report_' + Date.now() + '.xlsx';
                    document.body.appendChild(a);
                    a.click();
                    a.remove();
                    window.URL.revokeObjectURL(url);
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data');
                }
            });
        }

        function formatDateDMY(dateStr) {
            if (!dateStr) return '';
            var d = new Date(dateStr);
            if (isNaN(d.getTime())) return dateStr;   // যদি parse না হয় তাহলে আগের মতো রাখো

            var day = String(d.getDate()).padStart(2, '0');
            var month = String(d.getMonth() + 1).padStart(2, '0');
            var year = d.getFullYear();
            return day + '/' + month + '/' + year;
        }

        // ===================== YEARLY BONUS - PDF Builder =====================
        function buildPdfDocYearlyBonus(filter, rows, callback) {
            getImageBase64FromUrl('/images/DP_logo.png', function (base64Logo, natW, natH) {

                var LOGO_TARGET_HEIGHT = 26;
                var logoWidth = (natH > 0) ? (natW / natH) * LOGO_TARGET_HEIGHT : LOGO_TARGET_HEIGHT;

                var jsPDFCtor = window.jspdf ? window.jspdf.jsPDF : window.jsPDF;
                var doc = new jsPDFCtor({
                    orientation: 'portrait',
                    unit: 'pt',
                    format: 'a4'
                });

                var pageWidth = doc.internal.pageSize.getWidth();
                var pageHeight = doc.internal.pageSize.getHeight();
                var leftMargin = 15;
                var rightMargin = 15;
                var topMargin = 70;
                var bottomMargin = 40;

                function drawHeader() {
                    if (base64Logo) {
                        doc.addImage(base64Logo, 'PNG', 18, 18, logoWidth, LOGO_TARGET_HEIGHT);
                    }
                    doc.setFont("times", "bold");
                    doc.setFontSize(13);
                    doc.text('DataPath Ltd.', pageWidth / 2, 22, { align: 'center' });
                    doc.setFontSize(11);
                    doc.setFont("times", "normal");
                    doc.text('Yearly Bonus Report', pageWidth / 2, 38, { align: 'center' });
                    doc.setFontSize(8);
                    doc.setFont("times", "italic");
                    var periodText = filter.GenerateType === 'ByMonth'
                        ? 'For the month of ' + filter.MonthName + ', ' + filter.YearName
                        : 'For the period ' + (formatDateDMY(filter.DateFrom) || '') + ' to ' + (formatDateDMY(filter.DateTo) || '');
                    doc.text(periodText, pageWidth / 2, 52, { align: 'center' });
                }

                function drawFooter() {
                    var pageCount = doc.internal.getNumberOfPages();
                    var currentPage = doc.internal.getCurrentPageInfo().pageNumber;
                    var now = new Date();
                    var hours = now.getHours();
                    var minutes = now.getMinutes().toString().padStart(2, '0');
                    var ampm = hours >= 12 ? 'PM' : 'AM';
                    hours = hours % 12 || 12;
                    var day = now.getDate().toString().padStart(2, '0');
                    var month = (now.getMonth() + 1).toString().padStart(2, '0');
                    var year = now.getFullYear();
                    var printDateTime = day + '/' + month + '/' + year + ' ' + hours + ':' + minutes + ' ' + ampm;

                    doc.setFont("times", "normal");
                    doc.setFontSize(8);
                    doc.setTextColor(80);
                    doc.text('Print Datetime: ' + printDateTime, leftMargin, pageHeight - 18);
                    doc.text('GCTL Infosys - HRM & Finance System', pageWidth / 2, pageHeight - 18, { align: 'center' });
                    doc.text('Page ' + currentPage + ' of ' + pageCount, pageWidth - rightMargin, pageHeight - 18, { align: 'right' });
                    doc.setTextColor(0);
                }

                var head = [[
                    'SL.', 'ID NO.', 'Pay ID', 'Name of the Employee', 'Status',
                    'DEPARTMENT', 'DOH', 'DOT', 'Bank Account No',
                    'Salary', 'Yearly Bonus', 'Note'
                ]];

                var body = rows.map(function (row) {
                    return [
                        row.sl,
                        row.idNo,
                        row.payId,
                        row.nameOfTheEmployee,
                        row.status,
                        row.department,
                        row.dateOfHire,
                        row.dot,
                        row.bankAccountNo,
                        row.salary != null ? parseFloat(row.salary).toFixed() : '',
                        row.yearlyBonus != null ? parseFloat(row.yearlyBonus).toFixed() : '',
                        row.note
                    ];
                });

                var totalSalary = rows.reduce(function (sum, row) {
                    return sum + (parseFloat(row.salary) || 0);
                }, 0);

                var totalBonus = rows.reduce(function (sum, row) {
                    return sum + (parseFloat(row.yearlyBonus) || 0);
                }, 0);

                var columnStyles = {
                    0: { cellWidth: 15, halign: 'center' },
                    1: { cellWidth: 45, halign: 'center' },
                    2: { cellWidth: 25, halign: 'center' },
                    3: { cellWidth: 85 },
                    4: { cellWidth: 35, halign: 'center' },
                    5: { cellWidth: 60 },
                    6: { cellWidth: 45, halign: 'center' },
                    7: { cellWidth: 42, halign: 'center' },
                    8: { cellWidth: 65, halign: 'center' },
                    9: { cellWidth: 50, halign: 'right' },
                    10: { cellWidth: 50, halign: 'right' },
                    11: { cellWidth: 50 }
                };

                var footRow = [
                    { content: 'Total', colSpan: 9, styles: { halign: 'left', fontStyle: 'bold', fontSize: 8 } },
                    { content: totalSalary.toFixed(), styles: { halign: 'right', fontStyle: 'bold', fontSize: 8 } },
                    { content: totalBonus.toFixed(), styles: { halign: 'right', fontStyle: 'bold', fontSize: 8 } },
                    { content: '', styles: {} }
                ];

                doc.autoTable({
                    head: head,
                    body: body,
                    startY: topMargin,
                    styles: {
                        font: "times",
                        fontSize: 7.5,
                        textColor: 0,
                        cellPadding: 2,
                        valign: 'middle',
                        overflow: 'linebreak',
                        lineWidth: 0.3,
                        lineColor: [0, 0, 0]
                    },
                    headStyles: {
                        font: "times",
                        fillColor: false,
                        textColor: 0,
                        fontStyle: "bold",
                        fontSize: 8,
                        halign: 'center',
                        valign: 'middle',
                        lineWidth: 0.3,
                        lineColor: [0, 0, 0]
                    },
                    columnStyles: columnStyles,
                    tableWidth: 'auto',
                    foot: [footRow],
                    footStyles: {
                        font: "times",
                        fillColor: false,
                        fontStyle: "bold",
                        fontSize: 8,
                        lineWidth: 0.3,
                        lineColor: [0, 0, 0]
                    },
                    margin: { top: topMargin, bottom: bottomMargin, left: leftMargin, right: rightMargin },
                    didDrawPage: function () {
                        drawHeader();
                        drawFooter();
                    }
                });

                if (typeof callback === 'function') {
                    callback(doc);
                }
            });
        }

        // ===================== YEARLY BONUS - PDF Export =====================
        function exportPdfYearlyBonus(filter) {
            showLoading();
            $.ajax({
                url: settings.baseUrl + '/GetPayrollMasterFileYearlyBonus',
                type: 'POST',
                data: JSON.stringify(filter),
                contentType: 'application/json',
                success: function (res) {
                    hideLoading();
                    if (!res.success) {
                        alert(res.message || 'Failed to load data');
                        return;
                    }
                    var rows = res.data || [];
                    if (rows.length === 0) {
                        alert('No data found to download');
                        return;
                    }
                    buildPdfDocYearlyBonus(filter, rows, function (doc) {
                        doc.save('YearlyBonus_Report_' + Date.now() + '.pdf');
                    });
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data');
                }
            });
        }

        // ===================== YEARLY BONUS - PDF Preview =====================
        function previewPdfYearlyBonus(filter) {
            showLoading();
            $.ajax({
                url: settings.baseUrl + '/GetPayrollMasterFileYearlyBonus',
                type: 'POST',
                data: JSON.stringify(filter),
                contentType: 'application/json',
                success: function (res) {
                    hideLoading();
                    if (!res.success) {
                        alert(res.message || 'Failed to load data');
                        return;
                    }
                    var rows = res.data || [];
                    if (rows.length === 0) {
                        alert('No data found to preview');
                        return;
                    }
                    buildPdfDocYearlyBonus(filter, rows, function (doc) {
                        var blobUrl = doc.output('bloburl');
                        var $container = $('#pdf-preview-container');
                        $container.empty();
                        var $iframe = $('<iframe>', {
                            src: blobUrl,
                            style: 'width:100%; height:100%; border:0;'
                        });
                        $container.append($iframe).show();
                        $container[0].scrollIntoView({ behavior: 'smooth', block: 'start' });
                    });
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data');
                }
            });
        }

        // ===================== YEARLY BONUS - PDF Preview =====================
        function previewPdfYearlyBonus(filter) {
            showLoading();
            $.ajax({
                url: settings.baseUrl + '/GetPayrollMasterFileYearlyBonus',
                type: 'POST',
                data: JSON.stringify(filter),
                contentType: 'application/json',
                success: function (res) {
                    hideLoading();
                    if (!res.success) {
                        alert(res.message || 'Failed to load data');
                        return;
                    }
                    var rows = res.data || [];
                    if (rows.length === 0) {
                        alert('No data found to preview');
                        return;
                    }
                    buildPdfDocYearlyBonus(filter, rows, function (doc) {
                        var blobUrl = doc.output('bloburl');
                        var $container = $('#pdf-preview-container');
                        $container.empty();
                        var $iframe = $('<iframe>', {
                            src: blobUrl,
                            style: 'width:100%; height:100%; border:0;'
                        });
                        $container.append($iframe).show();
                        $container[0].scrollIntoView({ behavior: 'smooth', block: 'start' });
                    });
                },
                error: function () {
                    hideLoading();
                    alert('Failed to load data');
                }
            });
        }
        $('#btnExportReport').off('click').on('click', function () {
            var filter = buildFilter();
            var format = filter.ExportFormat;
            var masterVal = $("#masterFileTypeSelect").val();

            if (!masterVal) {
                alert("Select Master File Type");
                $("#masterFileTypeSelect").select2('open');
                return;
            }

            if (format === 'Excel') {
                if (masterVal === "01") exportExcel(filter);                  // General
                else if (masterVal === "02") exportExcelGratuity(filter);     // Gratuity
                else if (masterVal === "03") exportExcelYearlyBonus(filter);  // Yearly Bonus
            }
            else if (format === 'PDF') {
                if (masterVal === "01") exportPdf(filter);
                else if (masterVal === "02") exportPdfGratuity(filter);
                else if (masterVal === "03") exportPdfYearlyBonus(filter);
            }
        });

        $('#btnPreviewPdf').off('click').on('click', function () {
            var filter = buildFilter();
            var masterVal = $("#masterFileTypeSelect").val();

            if (masterVal === "01") previewPdf(filter);
            else if (masterVal === "02") previewPdfGratuity(filter);
            else if (masterVal === "03") previewPdfYearlyBonus(filter);
        });
        // toggle date-vs-month sections based on radio selection
        function toggleGenerateSections() {
            var mode = $('input[name="salaryGenerate"]:checked').val();
            var isByDate = mode === 'ByDate';
            $('#dateFrom, #dateTo').closest('.col-12').toggle(isByDate);
            $('#monthSelect, #yearSelect').closest('.col-12').toggle(!isByDate);
        }
        $('input[name="salaryGenerate"]').on('change', toggleGenerateSections);
        toggleGenerateSections();

        settings.load();
    };
})(jQuery);