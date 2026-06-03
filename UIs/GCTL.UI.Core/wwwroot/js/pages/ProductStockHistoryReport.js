(function ($) {
    $.fn.ProductStockHistoryReportJs = function (options) {
        const settings = $.extend({
            noResultsText: 'No results found.',
            multiSelect: true,
            onSelect: function (values, texts) { }
        }, options);

        return this.each(function () {
            const $originalSelect = $(this);
            const selectId = $originalSelect.attr('id') || 'dropdown_' + Math.random().toString(36).substr(2, 9);
            const selectName = $originalSelect.attr('name') || '';

            let selectedValues = [];
            let selectedTexts = [];

            const dropdownHtml = `
                <div class="searchable-dropdown">
                    <div class="position-relative">
                        <input type="text"
                               class="form-control dropdown-input form-control form-control-sm"
                               id="${selectId}_input"
                               placeholder="${$originalSelect.find('option:first').text()}"
                               autocomplete="off">
                        <i class="bi bi-search search-icon"></i>
                        <i class="bi bi-chevron-down dropdown-arrow" id="${selectId}_arrow"></i>
                    </div>
                    <input type="hidden" id="${selectId}_value" name="${selectName}" value="">
                    <ul class="dropdown-menu" id="${selectId}_menu" style="display: none;">
                        ${$originalSelect.find('option:not(:first)').map(function () {
                return `<li>
                            <a class="dropdown-item d-flex align-items-center" href="#" data-value="${$(this).val()}">
                                <input type="checkbox" class="me-2" style="pointer-events: none;">
                                <span>${$(this).text()}</span>
                            </a>
                        </li>`;
            }).get().join('')}
                    </ul>
                </div>
            `;

            $originalSelect.hide().after(dropdownHtml);

            const $dropdownInput = $(`#${selectId}_input`);
            const $dropdownMenu = $(`#${selectId}_menu`);
            const $dropdownArrow = $(`#${selectId}_arrow`);
            const $hiddenInput = $(`#${selectId}_value`);
            let $dropdownItems = $dropdownMenu.find('.dropdown-item');

            let isOpen = false;
            let isSearchMode = false;

            function updateDisplayText() {
                if (selectedTexts.length === 0) {
                    $dropdownInput.val('');
                    $dropdownInput.attr('placeholder', $originalSelect.find('option:first').text());
                } else if (selectedTexts.length === 1) {
                    $dropdownInput.val(selectedTexts[0]);
                } else {
                    $dropdownInput.val(selectedTexts.slice(0, 1).join(', ') + ` + ${selectedTexts.length - 1} items`);
                }
                isSearchMode = false;
            }

            function updateHiddenInput() {
                $hiddenInput.val(selectedValues.join(','));
                setTimeout(() => {
                    settings.onSelect(selectedValues, selectedTexts);
                }, 0);
            }

            function enterSearchMode() {
                if (selectedTexts.length > 0) {
                    $dropdownInput.val('');
                    $dropdownInput.attr('placeholder', 'Search...');
                    isSearchMode = true;
                }
            }

            $dropdownInput.on('click', function (e) {
                if (!isOpen) {
                    isSearchMode = false;
                    openDropdown();
                } else if (!isSearchMode && selectedTexts.length > 0) {
                    enterSearchMode();
                }
            });

            $dropdownInput.on('focus', function (e) {
                if (!isOpen) {
                    isSearchMode = false;
                    openDropdown();
                }
            });

            $dropdownInput.on('input', function () {
                const searchTerm = $(this).val().toLowerCase();
                isSearchMode = true;
                filterOptions(searchTerm);
                if (!isOpen) {
                    openDropdown();
                }
            });

            $dropdownInput.on('blur', function () {
                setTimeout(() => {
                    if (!isOpen && !isSearchMode) {
                        updateDisplayText();
                    }
                }, 200);
            });

            $(document).on('click', function (e) {
                if (!$(e.target).closest('.searchable-dropdown').is($dropdownInput.parent().parent())) {
                    if (isOpen) {
                        closeDropdown();
                    }
                }
            });

            $dropdownMenu.on('click', '.dropdown-item', function (e) {
                e.preventDefault();
                const value = $(this).data('value');
                const text = $(this).find('span').text();
                const checkbox = $(this).find('input[type="checkbox"]');

                if (selectedValues.includes(value.toString())) {
                    selectedValues = selectedValues.filter(v => v.toString() !== value.toString());
                    selectedTexts = selectedTexts.filter(t => t !== text);
                    checkbox.prop('checked', false);
                } else {
                    selectedValues.push(value.toString());
                    selectedTexts.push(text);
                    checkbox.prop('checked', true);
                }

                updateHiddenInput();

                setTimeout(() => {
                    if (isSearchMode) {
                        $dropdownInput.focus();
                    } else {
                        updateDisplayText();
                    }
                }, 10);
            });

            $dropdownInput.on('keydown', function (e) {
                if (e.key === 'Escape') {
                    closeDropdown();
                    updateDisplayText();
                } else if (e.key === 'Tab') {
                    closeDropdown();
                } else if (e.key === 'Backspace' && $(this).val() === '' && selectedTexts.length > 0 && isSearchMode) {
                    const removedValue = selectedValues.pop();
                    selectedTexts.pop();

                    $dropdownMenu.find('.dropdown-item').each(function () {
                        if ($(this).data('value').toString() === removedValue) {
                            $(this).find('input[type="checkbox"]').prop('checked', false);
                        }
                    });

                    updateHiddenInput();
                    if (selectedTexts.length === 0) {
                        updateDisplayText();
                    }
                }
            });
            function openDropdown() {
                $('.searchable-dropdown .dropdown-menu').hide();
                $('.searchable-dropdown .dropdown-arrow').removeClass('rotated');
                $('.searchable-dropdown').removeClass('active');

                $dropdownMenu.show();
                $dropdownArrow.addClass('rotated');
                $originalSelect.closest('.searchable-dropdown').addClass('active');

                if (!isSearchMode) {
                    filterOptions('');
                } else {
                    filterOptions($dropdownInput.val().toLowerCase());
                }
                isOpen = true;
            }
            function closeDropdown() {
                $dropdownMenu.hide();
                $dropdownArrow.removeClass('rotated');
                $originalSelect.closest('.searchable-dropdown').removeClass('active');
                updateDisplayText();
                isOpen = false;
                isSearchMode = false;

                if (selectedTexts.length === 0) {
                    $dropdownInput.val('');
                }
            }

            function filterOptions(searchTerm) {
                let hasResults = false;
                $dropdownMenu.find('.dropdown-item').each(function () {
                    const text = $(this).find('span').text().toLowerCase();
                    const value = $(this).data('value');

                    const checkbox = $(this).find('input[type="checkbox"]');
                    checkbox.prop('checked', selectedValues.includes(value.toString()));

                    if (text.includes(searchTerm)) {
                        $(this).parent().show();
                        hasResults = true;
                    } else {
                        $(this).parent().hide();
                    }
                });

                let $noResultsItem = $dropdownMenu.find('.no-results');
                if (!hasResults) {
                    if (!$noResultsItem.length) {
                        $dropdownMenu.append('<li class="no-results" style="padding: 0.5rem 1rem; color: #6c757d;">' + settings.noResultsText + '</li>');
                    }
                    $dropdownMenu.find('.no-results').show();
                } else {
                    if ($noResultsItem.length) {
                        $noResultsItem.remove();
                    }
                }
            }

            $originalSelect[0].getSelectedValues = function () {
                return selectedValues;
            };

            $originalSelect[0].getSelectedTexts = function () {
                return selectedTexts;
            };

            $originalSelect[0].clearSelections = function () {
                selectedValues = [];
                selectedTexts = [];
                $dropdownMenu.find('.dropdown-item input[type="checkbox"]').prop('checked', false);
                updateDisplayText();
                updateHiddenInput();
            };

            $originalSelect[0].refreshDropdown = function () {
                $dropdownItems = $dropdownMenu.find('.dropdown-item');
                filterOptions('');
            };
        });
    };

    function getSelectedIds(selector) {
        const element = $(selector)[0];
        return (element && typeof element.getSelectedValues === 'function')
            ? element.getSelectedValues()
            : [];
    }

    function populateDropdown(selector, items, defaultText = '-- Select --') {
        const $dropdown = $(selector);

        $dropdown.empty().append($('<option>').val('').text(defaultText));

        if (items && Array.isArray(items)) {
            items.forEach(item => {
                if (item && item.id !== undefined && item.name !== undefined) {
                    $dropdown.append($('<option>').val(item.id).text(item.name));
                }
            });
        }

        const selectId = $dropdown.attr('id');
        const $dropdownMenu = $(`#${selectId}_menu`);
        const $dropdownInput = $(`#${selectId}_input`);

        if ($dropdownMenu.length && $dropdownInput.length) {
            $dropdownMenu.empty();

            if (items && Array.isArray(items)) {
                items.forEach(item => {
                    if (item && item.id !== undefined && item.name !== undefined) {
                        const itemHtml = `
                            <li>
                                <a class="dropdown-item d-flex align-items-center" href="#" data-value="${item.id}">
                                    <input type="checkbox" class="me-2" style="pointer-events: none;">
                                    <span>${item.name}</span>
                                </a>
                            </li>
                        `;
                        $dropdownMenu.append(itemHtml);
                    }
                });
            }

            if (typeof $dropdown[0].clearSelections === 'function') {
                $dropdown[0].clearSelections();
            }

            if (typeof $dropdown[0].refreshDropdown === 'function') {
                $dropdown[0].refreshDropdown();
            }

            $dropdownInput.attr('placeholder', defaultText);
            $dropdownInput.val('');
        }
    }

    function loadFilteredDropdowns(filterData, updateDropdowns = []) {
        filterData.FromDate = null;
        filterData.ToDate = null;
        return $.ajax({
            url: "/ProductStockHistoryReport/GetFilteredDropdowns",
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(filterData),
            success: function (res) {
                updateDropdowns.forEach(dropdown => {
                    switch (dropdown) {
                        case 'category':
                            populateDropdown("#categoryDropdown", res.categoryIds, "~~Select Category~~");
                            break;
                        case 'product':
                            populateDropdown("#productDropdown", res.productIds, "~~Select Product~~");
                            break;
                        case 'brand':
                            populateDropdown("#brandDropdown", res.brandIds, "~~Select Brand~~");
                            break;
                        case 'model':
                            populateDropdown("#modelDropdown", res.modelIds, "~~Select Model~~");
                            break;
                    }
                });
            },
            error: function (e) {
            }
        });
    }

    // Get current filter data
    function getCurrentFilterData() {
        return {
            CategoryIds: getSelectedIds('#categoryDropdown'),
            ProductCodes: getSelectedIds('#productDropdown'),
            BrandIds: getSelectedIds('#brandDropdown'),
            ModelIds: getSelectedIds('#modelDropdown'),
            FromDate: $('#formDate').val() ? new Date($('#formDate').val()).toISOString() : null,
            ToDate: $('#toDate').val() ? new Date($('#toDate').val()).toISOString() : null
        };
    }

    function CategoryLoad() {
        const filterData = getCurrentFilterData();
        return loadFilteredDropdowns(filterData, ['category', 'product', 'brand', 'model']);
    }

    function initializeDropdowns() {
        $('#categoryDropdown').ProductStockHistoryReportJs({
            onSelect: function (values, texts) {
                const filterData = getCurrentFilterData();
                loadFilteredDropdowns(filterData, ['product', 'brand', 'model']);
            }
        });

        $('#productDropdown').ProductStockHistoryReportJs({
            onSelect: function (values, texts) {
                const filterData = getCurrentFilterData();
                loadFilteredDropdowns(filterData, ['brand', 'model']);
            }
        });

        $('#brandDropdown').ProductStockHistoryReportJs({
            onSelect: function (values, texts) {
                const filterData = getCurrentFilterData();
                loadFilteredDropdowns(filterData, ['model']);
            }
        });
        $('#modelDropdown').ProductStockHistoryReportJs({
            onSelect: function (values, texts) {
                const filterData = getCurrentFilterData();
                loadFilteredDropdowns(filterData, []);
            }
        });

    }

    function formatDateSubtitle(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('en-GB');
    }

    function formatDate(dateString) {
        const date = new Date(dateString);
        let formatted = date.toLocaleString('en-GB', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            hour12: true
        });
        return formatted.replace(/am|pm/i, match => match.toUpperCase());
    }

    function formatDateTime(dateString) {
        const date = new Date(dateString);
        let formatted = date.toLocaleString('en-GB', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            hour12: true
        });
        return formatted.replace(/am|pm/i, match => match.toUpperCase());
    }

    function generatePDFWithData(responseData, fromDate, toDate, isPdf) {
        const { jsPDF } = window.jspdf;
        //const doc = new jsPDF({ unit: 'mm', format: 'a4' });
        const doc = new jsPDF({ unit: 'mm', format: 'a4', orientation: 'landscape' });
        const fromDateFormatted = fromDate ? formatDateSubtitle(fromDate) : '';
        const toDateFormatted = toDate ? formatDateSubtitle(toDate) : '';
        const now = new Date();
        const printDateTime = formatDateTime(now);
        const pageWidth = doc.internal.pageSize.width;
        const pageHeight = doc.internal.pageSize.height;
        const margin = 5;
        doc.setFont('times', 'bold');
        doc.setTextColor(0);
        doc.setFontSize(18);
        doc.text('Product Stock History Report', pageWidth / 2, 25, { align: 'center' });
        if (fromDate != null && toDate != null) {
            doc.setFont('times', 'normal');
            doc.setFontSize(12);
            doc.text(`Date From: ${fromDateFormatted} to ${toDateFormatted}`, pageWidth / 2, 35, { align: 'center' });
        }

        if (!responseData.data || responseData.data.length === 0) {
            alert("No data found for the selected date range.");
            return;
        }
        const formatNumber = (val, decimals = 0) =>
            val != null && !isNaN(val) ? Number(val).toFixed(decimals) : '0'.padEnd(decimals ? decimals + 2 : 1, '0');

        const tableData = responseData.data.map(item => [
            item.productCode || '',
            item.productName || '',
            item.description || '',
            item.brandName || '',
            item.modelName || '',
            item.sizeName || '',
            item.unitName || '',
            formatNumber(item.unitPrice, 2),
            formatNumber(item.openingQty),
            formatNumber(item.receivedQty),
            formatNumber(item.stockQty),
            formatNumber(item.issuedQty),
            formatNumber(item.balanceQty),
            formatNumber(item.stockValue, 2)
        ]);


        const grandTotal = responseData.data.reduce((sum, item) => sum + item.stockValue, 0);

        const headers = [
            'Product Code',
            'Product',
            'Description',
            'Brand',
            'Model',
            'Size',
            'Unit',
            'Unit Price',
            'Opening Qty',
            'Received Qty',
            'Stock Qty',
            'Issued Qty',
            'Balance Qty',
            'Stock Value (BDT)'
        ];
        let startY = 30;
        if (fromDate != null && toDate != null) {
            startY = 40;
        }
        doc.autoTable({
            head: [headers],
            body: tableData,
            startY: startY,


            theme: 'grid',
            styles: {
                font: 'times',
                fontSize: 8,
                cellPadding: 2,
                halign: 'left',
                valign: 'middle',
                textColor: 0,
                lineColor: [0, 0, 0],
                lineWidth: 0.2
            },
            headStyles: {
                fillColor: false,
                textColor: 0,
                fontStyle: 'bold',
                lineColor: [0, 0, 0],
                lineWidth: 0.2,
                halign: 'center',
                valign: 'middle',
                fontSize: 8,
            },
            columnStyles: {
                0: { cellWidth: 25, halign: 'center' },
                1: { cellWidth: 'auto', halign: 'left' },
                2: { cellWidth: 40, halign: 'left' },
                3: { cellWidth: 20, halign: 'center' },
                4: { cellWidth: 20, halign: 'center' },
                5: { cellWidth: 10, halign: 'center' },
                6: { cellWidth: 15, halign: 'center' },
                7: { cellWidth: 15, halign: 'center' },
                8: { cellWidth: 15, halign: 'center' },
                9: { cellWidth:15, halign: 'center' },
                10: { cellWidth:15, halign: 'center' },
                11: { cellWidth:15, halign: 'center' },
                12: { cellWidth: 20, halign: 'center' },
                13: { cellWidth: 20, halign: 'right' }
            },
            margin: { left: margin, right: margin },
            didDrawPage: function (data) {
                doc.setFont('times', 'normal');
                doc.setFontSize(10);
                doc.setTextColor(0);
                doc.text(`Print Datetime: ${printDateTime}`, margin, pageHeight - 15);
                const pageCount = doc.internal.getNumberOfPages();
                const currentPage = doc.internal.getCurrentPageInfo().pageNumber;
                doc.text(`Page ${currentPage} of ${pageCount}`, pageWidth - margin, pageHeight - 15, { align: 'right' });
            }
        });


        const finalY = doc.lastAutoTable.finalY + 5;
        doc.setFontSize(9);
        doc.setFont('times', 'bold');
        doc.setTextColor(0);
        doc.text('Total:', pageWidth - margin - 28, finalY);
        doc.text(`${grandTotal.toFixed(2)}`, pageWidth - margin - 1, finalY, { align: 'right' });
        if (isPdf) {
            doc.save(`Printing_Stationery_Report.pdf`);
        } else {
            const blob = doc.output('blob');
            const blobUrl = URL.createObjectURL(blob);
            document.getElementById('pdfPreviewFrame').src = blobUrl;

        }
    }

    $(document).ready(function () {
        CategoryLoad().then(() => {
            initializeDropdowns();
        });

        flatpickr(".dateInput", CalendarService.createConfig({
            dateFormat: "Y-m-d",
            defaultDate: new Date(),
            altInput: true,
            altFormat: "d/m/Y",
            allowInput: true,
        }));

        $(document).on('click', '#btnPreviewPdf', function () {
            const filterData = getCurrentFilterData();

            $.ajax({
                url: "/ProductStockHistoryReport/GetFilteredStock",
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(filterData),
                success: function (res) {
                    generatePDFWithData(res, $("#formDate").val() || null, $("#toDate").val() || null, false);
                },
                error: function (e) {
                }
            });
        });

        $(document).on('click', '#downloadReport', function () {
            var format = $("#formatDropdown").val();
            if (format === 'pdf') {
                const filterData = getCurrentFilterData();
                $.ajax({
                    url: "/ProductStockHistoryReport/GetFilteredStock",
                    type: "POST",
                    contentType: 'application/json',
                    data: JSON.stringify(filterData),
                    success: function (res) {
                        generatePDFWithData(res, $("#formDate").val() || null, $("#toDate").val() || null, true);
                    },
                    error: function (e) {
                    }
                });
            } else if (format === 'excel') {
                const filterData = getCurrentFilterData();

                $.ajax({
                    url: "/ProductStockHistoryReport/ExportToExcel",
                    type: "POST",
                    contentType: 'application/json',
                    data: JSON.stringify(filterData),
                    xhrFields: {
                        responseType: 'blob'
                    },
                    success: function (blob, status, xhr) {
                        const contentType = xhr.getResponseHeader("Content-Type");

                        // If response is JSON (indicating no data), read it as text and parse
                        if (contentType && contentType.includes("application/json")) {
                            const reader = new FileReader();
                            reader.onload = function () {
                                try {

                                    const responseJson = JSON.parse(reader.result);
                                    if (responseJson.isIsuccess === false) {
                                        alert("No data found for the selected date range.");
                                        return;
                                    }
                                } catch (e) {
                                    alert("No data found for the selected date range.");
                                }
                            };
                            reader.readAsText(blob);
                            return;
                        }

                        // Otherwise, it's a valid Excel file
                        let disposition = xhr.getResponseHeader("Content-Disposition");
                        let filename = "Product_Stock_History_Report.xlsx";

                        if (disposition && disposition.indexOf("filename=") !== -1) {
                            const match = disposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
                            if (match && match[1]) {
                                filename = match[1].replace(/['"]/g, '');
                            }
                        }

                        const blobUrl = window.URL.createObjectURL(blob);
                        const link = document.createElement('a');
                        link.href = blobUrl;
                        link.download = filename;
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                        window.URL.revokeObjectURL(blobUrl);
                    },
                    error: function (xhr, status, error) {
                        alert("No data found for the selected date range.");
                        return;
                    }
                });
            }else {
                const filterData = getCurrentFilterData();
                $.ajax({
                    url: "/ProductStockHistoryReport/GetFilteredStock",
                    type: "POST",
                    contentType: 'application/json',
                    data: JSON.stringify(filterData),
                    success: function (res) {
                        generatePDFWithData(res, $("#formDate").val() || null, $("#toDate").val() || null, true);
                    },
                    error: function (e) {
                    }
                });
            }

        });
    });

})(jQuery);