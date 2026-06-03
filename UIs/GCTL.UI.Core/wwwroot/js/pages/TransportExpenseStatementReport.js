(function ($) {
    $.fn.TransportExpenseStatementReportJs = function (options) {
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
            url: "/TransportExpenseStatementReport/GetFilterDropdownDataReport",
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(filterData),
            success: function (res) {
                updateDropdowns.forEach(dropdown => {
                    switch (dropdown) {
                        case 'transportType':
                            populateDropdown("#transportTypeDropdown", res.transportTypeIds, "Transport Type...");
                            break;
                        case 'transportNo':
                            populateDropdown("#transportNoDropdown", res.transportIds, "Transport No...");
                            break;
                        case 'driver':
                            populateDropdown("#driverDropdown", res.driverEmployeeIds, "Driver...");
                            break;
                    }
                });
            },
            error: function (xhr, status, error) {
            }
        });
    }

    function getCurrentFilterData() {
        let active = $('input[name="inlineRadioOptions"]:checked').val();

        if (active === "bydate") {
            // By Date selected
            return {
                TransportTypeIds: getSelectedIds('#transportTypeDropdown'),
                TransportIds: getSelectedIds('#transportNoDropdown'),
                DriverEmployeeIds: getSelectedIds('#driverDropdown'),
                FromDate: $('#formDate').val() ? new Date($('#formDate').val()).toISOString() : null,
                ToDate: $('#toDate').val() ? new Date($('#toDate').val()).toISOString() : null,
                Month: null,
                Year: null
            };
        } else if (active === "bymonth") {
            // By Month selected
            return {
                TransportTypeIds: getSelectedIds('#transportTypeDropdown'),
                TransportIds: getSelectedIds('#transportNoDropdown'),
                DriverEmployeeIds: getSelectedIds('#driverDropdown'),
                FromDate: null,
                ToDate: null,
                Month: $('#dateMonth').val() || null,
                Year: $('#dateYear').val() || null
            };
        } else {
            // Default fallback
            return {
                TransportTypeIds: getSelectedIds('#transportTypeDropdown'),
                TransportIds: getSelectedIds('#transportNoDropdown'),
                DriverEmployeeIds: getSelectedIds('#driverDropdown'),
                FromDate: null,
                ToDate: null,
                Month: null,
                Year: null
            };
        }
    }

    function TransportTypeLoad() {
        const filterData = getCurrentFilterData();
        return loadFilteredDropdowns(filterData, ['transportType', 'transportNo', 'driver']);
    }

    function initializeDropdowns() {
        $('#transportTypeDropdown').TransportExpenseStatementReportJs({
            onSelect: function (values, texts) {
                const filterData = getCurrentFilterData();
                loadFilteredDropdowns(filterData, ['transportNo', 'driver']);
            }
        });

        $('#transportNoDropdown').TransportExpenseStatementReportJs({
            onSelect: function (values, texts) {
                const filterData = getCurrentFilterData();
                loadFilteredDropdowns(filterData, ['driver']);
            }
        });

        $('#driverDropdown').TransportExpenseStatementReportJs({
            onSelect: function (values, texts) {
                // Optional: You can load additional data or trigger other actions
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

    $(document).ready(function () {
        var currentYear = new Date().getFullYear(); 
        $("#dateYear").val(currentYear);
    });

    function generatePDFWithData(responseData, fromDate, toDate, isPdf) {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF({ unit: 'mm', format: 'a4', orientation: 'landscape' });
        const now = new Date();
        const printDateTime = formatDateTime(now);
        const pageWidth = doc.internal.pageSize.width;
        const pageHeight = doc.internal.pageSize.height;
        const margin = 5;

        let startY = 30; // default startY

        // ---------- Title ----------
        doc.setFont('times', 'bold');
        doc.setFontSize(16);
        doc.setTextColor(0);
        doc.text('Transport Expenses Statement', pageWidth / 2, 22, { align: 'center' });

        // ---------- Subtitle (By Date / By Month) ----------
        if (fromDate && toDate && (fromDate.includes('/') || fromDate.includes('-')) && (toDate.includes('/') || toDate.includes('-'))) {
            // By Date
            const fromDateFormatted = formatDateSubtitle(fromDate);
            const toDateFormatted = formatDateSubtitle(toDate);
            doc.setFont('times', 'bold');
            doc.setFontSize(12);
            doc.text(`From ${fromDateFormatted} to ${toDateFormatted}`, pageWidth / 2, 28, { align: 'center' });
            startY = 35;
        } else if (fromDate && toDate) {
            // By Month (numeric)
            const month = parseInt(fromDate);
            const year = parseInt(toDate);
            const monthName = new Date(year, month - 1).toLocaleString('default', { month: 'long' });
            doc.setFont('times', 'bold');
            doc.setFontSize(12);
            doc.text(`For the Month Of ${monthName}, ${year}`, pageWidth / 2, 28, { align: 'center' });
            startY = 35;
        }

        if (!responseData.data || responseData.data.length === 0) {
            alert("No data found for the selected criteria.");
            return;
        }        
        const formatNumber = (val) => {
            if (val == null || isNaN(val) || Number(val) === 0) return "-";
            return Number(val).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        };

        // ---------- Table Headers ----------
        const headers = [
            'SL.No.', 'Transport.No', 'Transport Type', 'Transport Capacity/Persons', 'Driver Name', 'Helper Name',
            'CNG /Gas Bill', 'R/M Bill', 'Fuel/Octane Bill', 'Police Donation', 'Toll/others Bill', 'TAX ,Fitness And Rute Permit',
            'Salary(Driver,Helper)', 'Mechanic Salary', 'Monthly Police Donation', 'Monthly Eng.Oil Purchase',
            'Akesh Technology', 'Garage Rent', 'Total'
        ];

        // ---------- Prepare table data ----------
        const tableData = responseData.data.map((item, index) => [
            index + 1,
            item.vehicleNo || '',
            item.vehicleType || '',
            item.transportCapacity || '',
            item.fullName || '',
            item.helperName || '',
            formatNumber(item.cngGasBill),
            formatNumber(item.rmBill),
            formatNumber(item.fuelOctaneBill),
            formatNumber(item.policeDonation),
            formatNumber(item.tollOthersBill),
            formatNumber(item.taxFitnessAndRutePermit),
            formatNumber(item.salaryDriverHelper),
            formatNumber(item.mechanicSalary),
            formatNumber(item.monthlyPoliceDonation),
            formatNumber(item.monthlyEngOilPurchase),
            formatNumber(item.akeshTechnology),
            formatNumber(item.garageRent),
            formatNumber(item.totalExpense)
        ]);

        // ---------- Calculate totals ----------
        let totalCNG = 0, totalRM = 0, totalFuel = 0, totalPolice = 0, totalToll = 0;
        let totalTax = 0, totalSalary = 0, totalMechanic = 0, totalMonthlyPolice = 0;
        let totalEngOil = 0, totalAkesh = 0, totalGarage = 0, grandTotal = 0;

        responseData.data.forEach(item => {
            totalCNG += item.cngGasBill || 0;
            totalRM += item.rmBill || 0;
            totalFuel += item.fuelOctaneBill || 0;
            totalPolice += item.policeDonation || 0;
            totalToll += item.tollOthersBill || 0;
            totalTax += item.taxFitnessAndRutePermit || 0;
            totalSalary += item.salaryDriverHelper || 0;
            totalMechanic += item.mechanicSalary || 0;
            totalMonthlyPolice += item.monthlyPoliceDonation || 0;
            totalEngOil += item.monthlyEngOilPurchase || 0;
            totalAkesh += item.akeshTechnology || 0;
            totalGarage += item.garageRent || 0;
            grandTotal += item.totalExpense || 0;
        });

        // ---------- Add Totals Row ----------
        const totalsRow = [
            '', '', '', '', '', 'Total:',
            formatNumber(totalCNG),
            formatNumber(totalRM),
            formatNumber(totalFuel),
            formatNumber(totalPolice),
            formatNumber(totalToll),
            formatNumber(totalTax),
            formatNumber(totalSalary),
            formatNumber(totalMechanic),
            formatNumber(totalMonthlyPolice),
            formatNumber(totalEngOil),
            formatNumber(totalAkesh),
            formatNumber(totalGarage),
            formatNumber(grandTotal),
        ];
        tableData.push(totalsRow);

        // ---------- Generate PDF Table ----------
        doc.autoTable({
            head: [headers],
            body: tableData,
            startY: startY,
            theme: 'grid',
            styles: {
                font: 'times',
                fontSize: 5,
                cellPadding: 0.5,
                halign: 'center',
                valign: 'middle',
                textColor: 0,
                lineColor: [0, 0, 0],
                lineWidth: 0.2
            },
            headStyles: {
                fillColor: [255, 255, 255],
                textColor: 0,
                fontStyle: 'bold',
                halign: 'center',
                valign: 'middle',
                fontSize: 5,
                cellPadding: 0.5,
                lineColor: [0, 0, 0],
                lineWidth: 0.2
            },
            columnStyles: {
                0: { cellWidth: '8' }, 1: { cellWidth: 30, halign: 'left' }, 2: { cellWidth: 15 }, 3: { cellWidth: 12 }, 4: { cellWidth: 'auto', halign:'left' }, 5: { cellWidth: 15 },
                6: { cellWidth: 13, halign: 'right' }, 7: { cellWidth: 10, halign: 'right' }, 8: { cellWidth: 13, halign: 'right' }, 9: { cellWidth: 12, halign: 'right' },
                10: { cellWidth: 12, halign: 'right' }, 11: { cellWidth: 13, halign: 'right' }, 12: { cellWidth: 13, halign: 'right' }, 13: { cellWidth: 12, halign: 'right' },
                14: { cellWidth: 15, halign: 'right' }, 15: { cellWidth: 15, halign: 'right' }, 16: { cellWidth: 12, halign: 'right' }, 17: { cellWidth: 12, halign: 'right' },
                18: { cellWidth: 15, halign: 'right' }
            },
            didParseCell: function (data) {
                // Merge subtotal row first 6 cells and right-align "Total:"
                if (data.row.index === tableData.length - 1) {
                    if (data.column.index === 0) {
                        data.cell.colSpan = 6;
                        data.cell.styles.halign = 'right';
                        data.cell.styles.fontStyle = 'bold';
                        data.cell.styles.textColor = [0, 0, 0];
                        data.cell.styles.lineWidth = 0.1;
                        data.cell.text = ['Total:'];
                    }
                    // Make all numeric cells in subtotal row bold
                    else if (data.column.index >= 6) {
                        data.cell.styles.fontStyle = 'bold';
                        data.cell.styles.halign = 'right';
                    }
                }
            },
            margin: { left: margin, right: margin },
            didDrawCell: function (data) {
                if (data.row.index === tableData.length - 1 && data.column.index >= 6) {
                    data.cell.styles.fillColor = [255, 255, 224];
                    data.cell.styles.fontStyle = 'bold';
                }
            },
            didDrawPage: function (data) {
                doc.setFont('times', 'normal');
                doc.setFontSize(8);
                doc.text(`Print Datetime: ${printDateTime}`, margin, pageHeight - 10);
                const pageCount = doc.internal.getNumberOfPages();
                const currentPage = doc.internal.getCurrentPageInfo().pageNumber;
                doc.text(`Page ${currentPage} of ${pageCount}`, pageWidth - margin, pageHeight - 10, { align: 'right' });
            }
        });

        // ---------- Save or Preview ----------
        if (isPdf) {
            doc.save(`Transport_Expense_Statement_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`);
        } else {
            const blob = doc.output('blob');
            const blobUrl = URL.createObjectURL(blob);
            document.getElementById('pdfPreviewFrame').src = blobUrl;
        }
    }
    
    $(document).ready(function () {
        function toggleDateFilters(activeValue) {

            if (activeValue === "bymonth") {
                $(".dateMonthYear").removeClass("dateFilterHide").addClass("dateFilterShow");
                $(".dateFromTo").removeClass("dateFilterShow").addClass("dateFilterHide");
            } else if (activeValue === "bydate") {
                $(".dateFromTo").removeClass("dateFilterHide").addClass("dateFilterShow");
                $(".dateMonthYear").removeClass("dateFilterShow").addClass("dateFilterHide");
            }
        }

        // Radio button change event
        $('input[name="inlineRadioOptions"]').change(function () {
            toggleDateFilters($(this).val());
        });

        // Initialize with default checked radio button
        let active = $('input[name="inlineRadioOptions"]:checked').val();
        if (active) {
            toggleDateFilters(active);
        }
    });

    $(document).ready(function () {

        TransportTypeLoad().then(() => {
            initializeDropdowns();
        }).catch((error) => {
            // Initialize dropdowns anyway to enable basic functionality
            initializeDropdowns();
        });

        //flatpickr(".dateInput", {
        //    dateFormat: "Y-m-d",
        //    defaultDate: new Date(),
        //    altInput: true,
        //    altFormat: "d/m/Y",
        //    allowInput: true,
        //});
        flatpickr('.dateInput', CalendarService.createConfig(
            {
                defaultDate: new Date(),
            }
        ));

        $(document).on('click', '#btnPreviewPdf', function () {
            const filterData = getCurrentFilterData();

            $.ajax({
                url: "/TransportExpenseStatementReport/GetFilterResultReport",
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(filterData),
                success: function (res) {
                    //generatePDFWithData(res, $("#formDate").val() || null, $("#toDate").val() || null, false);
                    let active = $('input[name="inlineRadioOptions"]:checked').val();
                    if (active === "bydate") {
                        generatePDFWithData(
                            res,
                            $("#formDate").val() || null,
                            $("#toDate").val() || null,
                            false
                        );
                    }
                    else if (active === "bymonth") {
                        generatePDFWithData(
                            res,
                            $("#dateMonth").val() || null,
                            $("#dateYear").val() || null,
                            false
                        );
                    }
                    else {
                        // fallback case
                        generatePDFWithData(res, null, null, false);
                    }
                },
                error: function (xhr, status, error) {
                    alert("Error generating preview. Please try again.");
                }
            });
        });

        $(document).on('click', '#downloadReport', function () {
            var format = $("#formatDropdown").val();
            const filterData = getCurrentFilterData();
            if (!format) {
                alert("Please select a format before downloading.");
                return;
            }


            if (format === 'pdf') {
                let filterData = getCurrentFilterData();

                $.ajax({
                    url: "/TransportExpenseStatementReport/GetFilterResultReport",
                    type: "POST",
                    contentType: 'application/json',
                    data: JSON.stringify(filterData),
                    success: function (res) {

                        let active = $('input[name="inlineRadioOptions"]:checked').val();
                        if (active === "bydate") {
                            generatePDFWithData(
                                res,
                                $("#formDate").val() || null,
                                $("#toDate").val() || null,
                                true
                            );
                        }
                        else if (active === "bymonth") {
                            generatePDFWithData(
                                res,
                                $("#dateMonth").val() || null,
                                $("#dateYear").val() || null,
                                true
                            );
                        }
                        else {
                            // fallback case
                            generatePDFWithData(res, null, null, true);
                        }
                    },
                    error: function (xhr, status, error) {
                        alert("Error generating PDF report. Please try again.");
                    }
                });
            } else if (format === 'excel') {
                $.ajax({
                    url: "/TransportExpenseStatementReport/ExportToExcel",
                    type: "POST",
                    contentType: 'application/json',
                    data: JSON.stringify(filterData),
                    xhrFields: {
                        responseType: 'blob'
                    },
                    success: function (blob, status, xhr) {
                        const contentType = xhr.getResponseHeader("Content-Type");

                        if (contentType && contentType.includes("application/json")) {
                            const reader = new FileReader();
                            reader.onload = function () {
                                try {
                                    const responseJson = JSON.parse(reader.result);
                                    if (responseJson.isSuccess === false) {
                                        alert("No data found for the selected criteria.");
                                        return;
                                    }
                                } catch (e) {
                                    alert("No data found for the selected criteria.");
                                }
                            };
                            reader.readAsText(blob);
                            return;
                        }

                        let disposition = xhr.getResponseHeader("Content-Disposition");
                        let filename = "Transport_Expense_Statement_Report.xlsx";

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
                        alert("Error generating Excel report. Please try again.");
                    }
                });
            }
        });
    });

})(jQuery);