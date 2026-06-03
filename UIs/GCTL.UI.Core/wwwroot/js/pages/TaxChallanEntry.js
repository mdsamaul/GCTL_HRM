(function ($) {
    $.fn.TaxChallanEntry = function (options) {
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
                        $dropdownMenu.append('<li class="no-results" style="padding: 0.2rem .4rem; color: #6c757d;">' + settings.noResultsText + '</li>');
                    }
                    $dropdownMenu.find('.no-results').show();
                } else {
                    if ($noResultsItem.length) {
                        $noResultsItem.remove();
                    }
                }
            }

            // Add method to programmatically select values
            $originalSelect[0].selectValues = function (values, texts) {
                selectedValues = Array.isArray(values) ? values.map(v => v.toString()) : [values.toString()];
                selectedTexts = Array.isArray(texts) ? texts : [texts];

                // Update checkboxes
                $dropdownMenu.find('.dropdown-item').each(function () {
                    const value = $(this).data('value').toString();
                    const checkbox = $(this).find('input[type="checkbox"]');
                    checkbox.prop('checked', selectedValues.includes(value));
                });

                updateDisplayText();
                updateHiddenInput();
            };

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

    // SweetAlert toast message
    function showToast(iconType, message) {
        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 5000,
            timerProgressBar: true,
            showClass: {
                popup: 'swal2-show swal2-fade-in'
            },
            hideClass: {
                popup: 'swal2-hide swal2-fade-out'
            }
        });

        Toast.fire({
            icon: iconType,
            title: message
        });
    }

    function populateDropdown(selector, items, defaultText = '-- Select --') {
        const $dropdown = $(selector);

        // Clear existing options
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

            // Clear previous selections first
            if (typeof $dropdown[0].clearSelections === 'function') {
                $dropdown[0].clearSelections();
            }

            // Auto-select company ID "001" if this is the company dropdown
            if (selector === "#taxChallanCompanyDropdown") {
                const defaultItem = items.find(i => i.id === "001");
                if (defaultItem && typeof $dropdown[0].selectValues === 'function') {
                    // Small delay to ensure dropdown is fully initialized
                    setTimeout(() => {
                        $dropdown[0].selectValues(["001"], [defaultItem.name]);
                    }, 100);
                }
            }

            if (typeof $dropdown[0].refreshDropdown === 'function') {
                $dropdown[0].refreshDropdown();
            }

            $dropdownInput.attr('placeholder', defaultText);
        }
    }

    function loadFilteredDropdowns(filterData, updateDropdowns = []) {
        console.log(filterData);
        return $.ajax({
            url: "/TaxChallanEntry/GetFilterDropdownData",
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(filterData),
            success: function (res) {
                if (res.employeeList.length > 0) {
                    employeeTable(res.employeeList);
                }
                updateDropdowns.forEach(dropdown => {
                    switch (dropdown) {
                        case 'company':
                            populateDropdown("#taxChallanCompanyDropdown", res.companyesList, "Company...");
                            break;
                        case 'branch':
                            populateDropdown("#taxChallanBranchDropdown", res.branchList, "Branch...");
                            break;
                        case 'department':
                            populateDropdown("#taxChallanDepartmentDropdown", res.departmentsList, "Department...");
                            break;
                        case 'designation':
                            populateDropdown("#taxChallanDesignationDropdown", res.designationsList, "Designation...");
                            break;
                    }
                });
            },
            error: function (xhr, status, error) {
            }
        });
    }

    function getCurrentFilterData() {
        return {
            CompanyCodes: getSelectedIds('#taxChallanCompanyDropdown'),
            BranchCodes: getSelectedIds('#taxChallanBranchDropdown'),
            DesignationCodes: getSelectedIds('#taxChallanDesignationDropdown'),
            DepartmentCodes: getSelectedIds('#taxChallanDepartmentDropdown'),
            ActiveStatus: getStatusValue('#taxChallanEmployeeStatusDropdown'),
        };
    }

    function getStatusValue(selector) {
        return $(selector).val() || ""; 
    }

    function TaxChallanLoad() {
        const filterData = getCurrentFilterData();
        return loadFilteredDropdowns(filterData, ['company', 'branch', 'department', 'designation']);
    }

    function initializeDropdowns() {
        $('#taxChallanCompanyDropdown').TaxChallanEntry({
            onSelect: function (values, texts) {
                const filterData = getCurrentFilterData();
                loadFilteredDropdowns(filterData, ['branch', 'department', 'designation']);
            }
        });

        $('#taxChallanBranchDropdown').TaxChallanEntry({
            onSelect: function (values, texts) {
                const filterData = getCurrentFilterData();
                loadFilteredDropdowns(filterData, ['department', 'designation']);
            }
        });

        $('#taxChallanDepartmentDropdown').TaxChallanEntry({
            onSelect: function (values, texts) {
                const filterData = getCurrentFilterData();
                loadFilteredDropdowns(filterData, ['designation']);
            }
        });

        $('#taxChallanDesignationDropdown').TaxChallanEntry({
            onSelect: function (values, texts) {
                // Optional: You can load additional data or trigger other actions
                const filterData = getCurrentFilterData();
                loadFilteredDropdowns(filterData, []);
            }
        });
        //$('#taxChallanEmployeeStatusDropdown').TaxChallanEntry({
        //    onSelect: function (values, texts) {
        //        // Optional: You can load additional data or trigger other actions
        //        const filterData = getCurrentFilterData();
        //        loadFilteredDropdowns(filterData, []);
        //    }
        //});
        $(document).on('change', "#taxChallanEmployeeStatusDropdown", function () {
            console.log("click");
            const filterData = getCurrentFilterData();
            console.log(filterData);
                    loadFilteredDropdowns(filterData, []);
        })
        // Auto-select company "001" after all dropdowns are initialized
        setTimeout(() => {
            autoSelectCompany001();
        }, 500);
    }

    // Separate function to auto-select company 001
    function autoSelectCompany001() {

        const $companyDropdown = $('#taxChallanCompanyDropdown');
        const $companyMenu = $('#taxChallanCompanyDropdown_menu');

        if ($companyMenu.length > 0) {
            const $targetItem = $companyMenu.find('[data-value="001"]');

            if ($targetItem.length > 0) {
                $targetItem.click();

                // Verify selection
                setTimeout(() => {
                    const selectedValues = $companyDropdown[0].getSelectedValues ? $companyDropdown[0].getSelectedValues() : [];
                }, 100);
            } else {
                // List all available items for debugging
                $companyMenu.find('[data-value]').each(function () {
                    //console.log("Available item:", $(this).data('value'), $(this).find('span').text());
                });
            }
        } else {
            //console.log("Company dropdown menu not found");
        }
    }




    $(document).ready(function () {
        var currentYear = new Date().getFullYear();
        $("#dateYear").val(currentYear);

        // Initialize dropdowns with data loading
        TaxChallanLoad().then(() => {
            initializeDropdowns();
        }).catch((error) => {
            // Initialize dropdowns anyway to enable basic functionality
            initializeDropdowns();
        });

        // Initialize date picker

        initTaxChallanDatePicker();
    });
    var taxChallanDatePicker;
    function initTaxChallanDatePicker() {
        taxChallanDatePicker =  flatpickr("#Setup_TaxChallanDate", CalendarService.createConfig(
            {
                defaultDate: new Date(),
            }
        ));
    }
    let dataTable = null;

    function employeeTable(employeeList) {

        // Initialize DataTable if not already done
        if (!dataTable) {
            dataTable = $('#TaxChallanTable').DataTable({
                data: employeeList,
                columns: [
                    {
                        title: '<input type="checkbox" id="selectAll" /> <small>Select</small>',
                        data: null,
                        orderable: false,
                        searchable: false,
                        width: "80px",
                        className: "text-center",
                        render: function (data, type, row, meta) {
                            return `<input type="checkbox" class="employee-select" value="${row.employeeID}" data-company="${row.companyCode}">`;
                        }
                    },
                    {
                        title: 'Employee ID',
                        data: 'employeeID',
                        width: "120px",
                        className: "text-center"
                    },
                    {
                        title: 'Name',
                        data: 'fullName',
                        className: "text-left"
                    },
                    {
                        title: 'Designation',
                        data: 'designationName',
                        className: "text-left"
                    },
                    {
                        title: 'Department',
                        data: 'departmentName',
                        className: "text-left"
                    },
                    {
                        title: 'Branch',
                        data: 'branchName',
                        className: "text-left"
                    },
                    {
                        title: 'Joining Date',
                        data: 'joiningDate',
                        width: "110px",
                        className: "text-center",
                        render: function (data, type, row) {
                            if (data) {
                                const date = new Date(data);
                                return date.toLocaleDateString('en-GB');
                            }
                            return '';
                        }
                    },
                    {
                        title: 'Tax Amount (BDT)',
                        data: 'grossSalary',
                        width: "130px",
                        className: "text-right",
                        //render: function (data, type, row) {
                        //    if (data) {
                        //        return new Intl.NumberFormat('en-BD', {
                        //            style: 'currency',
                        //            currency: '',
                        //            minimumFractionDigits: 0,
                        //            maximumFractionDigits: 0
                        //        }).format(data);
                        //    }
                        //    return '';
                        //}
                    }
                ],
                responsive: true,
                pageLength: 5,
                lengthMenu: [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]],
                language: {
                    search: "Search Employees:",
                    lengthMenu: "Show _MENU_",
                    info: "Showing _START_ to _END_ of _TOTAL_ employees",
                    infoEmpty: "No employees found",
                    infoFiltered: "(filtered from _MAX_ total employees)",
                    zeroRecords: "No matching employees found",
                    emptyTable: "No employee data available",
                },
                dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
                    '<"row"<"col-sm-12"tr>>' +
                    '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
                order: [[1, 'asc']], // Sort by Employee ID by default
                drawCallback: function () {
                    updateSelectAllState();
                    updateSelectedCount();
                }
            });

            // Initialize event handlers
            initializeTableEvents();
        } else {
            // Update existing DataTable with new data
            dataTable.clear().rows.add(employeeList).draw();
        }
    }
    function initializeTableEvents() {
        // Select All functionality
        $('#TaxChallanTable').on('change', '#selectAll', function () {
            const isChecked = $(this).prop('checked');
            $('.employee-select:visible').prop('checked', isChecked);
            updateSelectedCount();
        });

        // Individual checkbox change
        $('#TaxChallanTable').on('change', '.employee-select', function () {
            updateSelectAllState();
            updateSelectedCount();
        });
    }

    function updateSelectAllState() {
        const totalVisible = $('.employee-select:visible').length;
        const selectedVisible = $('.employee-select:visible:checked').length;
        const selectAllCheckbox = $('#selectAll');

        if (selectedVisible === 0) {
            selectAllCheckbox.prop('indeterminate', false);
            selectAllCheckbox.prop('checked', false);
        } else if (selectedVisible === totalVisible) {
            selectAllCheckbox.prop('indeterminate', false);
            selectAllCheckbox.prop('checked', true);
        } else {
            selectAllCheckbox.prop('indeterminate', true);
        }
    }

    function updateSelectedCount() {
        const selectedCount = $('.employee-select:checked').length;

        // Update status display if element exists
        const statusElement = $('#selectedEmployeesStatus');
        if (statusElement.length) {
            statusElement.text(`Selected: ${selectedCount} employees`);
        }

        // Enable/disable buttons based on selection
        const generateBtn = $('#generateChallan');
        if (generateBtn.length) {
            generateBtn.prop('disabled', selectedCount === 0);
        }
    }

    // Global function to get selected employees
    window.getSelectedEmployees = function () {
        const selectedEmployees = [];
        $('.employee-select:checked').each(function () {
            selectedEmployees.push($(this).val());
        });
        return selectedEmployees;
    };

    // Remove is-invalid on change/typing
    $(document).on('change keyup', 'input, select', function () {
        $(this).removeClass('is-invalid');
    });

    // Bootstrap-Select change
    $('.selectpicker').on('changed.bs.select', function () {
        $(this).removeClass('is-invalid');
    });

    // Flatpickr change
    if (typeof taxChallanDatePicker !== "undefined") {
        taxChallanDatePicker.config.onChange.push(function (selectedDates, dateStr, instance) {
            $("#Setup_TaxChallanDate").removeClass("is-invalid");
        });
    }
    //#region save & Edit
    $(document).on('click', ".js-tax-challan-entry-save", function () {
        var formData = getFormData();

        if (formData.TaxDepositCode == 0) {
            // EmployeeIds
            if (!formData.EmployeeIds || formData.EmployeeIds.length === 0) {
                showToast('error', "Please select at least one Employee.");
                $("#taxChallanEmployeeDropdown").addClass("is-invalid").focus();
                return;
            }
        }


        //FinancialCodeNo

        if (!formData.FinancialCodeNo || formData.FinancialCodeNo === "") {
            showToast('error', "Please select a Bank.");
            $("#Setup_BankId").addClass("is-invalid").focus();
            return;
        }
       

        // Salary Month
        if (
            !formData.SalaryMonth ||
            formData.SalaryMonth === "" ||
            isNaN(formData.SalaryMonth) ||
            parseInt(formData.SalaryMonth) < 1 ||
            parseInt(formData.SalaryMonth) > 12
        ) {
            showToast('error', "Please select Salary Month.");
            $("#Setup_SalaryMonth").addClass("is-invalid").focus();
            return;
        }
        // Salary Year
        if (
            !formData.SalaryYear ||
            formData.SalaryYear === "" ||
            isNaN(formData.SalaryYear) ||
            parseInt(formData.SalaryYear) < 1900 ||
            parseInt(formData.SalaryYear) > 2100
        ) {
            showToast('error', "Please select a valid Salary Year (1900 - 2100).");
            $("#Setup_SalaryYear").addClass("is-invalid").focus();
            return;
        }

       
        // Tax Challan No
        if (!formData.TaxChallanNo || formData.TaxChallanNo.trim() === "") {
            showToast('error', "Please enter Tax Challan No.");
            $("#Setup_TaxChallanNo").addClass("is-invalid").focus();
            return;
        }
        // Tax Challan Date
        if (!formData.TaxChallanDate) {
            showToast('error', "Please select Tax Challan Date.");
            $("#Setup_TaxChallanDate").addClass("is-invalid").focus();
            if (typeof taxChallanDatePicker !== "undefined") {
                taxChallanDatePicker.open();
            }
            return;
        }

        // Challan Amount
        if (!formData.ChallanAmount || formData.ChallanAmount <= 0) {
            showToast('error',"Please enter Challan Amount.");
            $("#Setup_ChallanAmount").addClass("is-invalid").focus();
            return;
        }

        
        // Bank
        if (!formData.BankId || formData.BankId === "") {
            showToast('error', "Please select a Bank.");
            $("#Setup_BankId").addClass("is-invalid").focus();

            $("#Setup_BankId").selectpicker('show');
            return;
        }

       

        $.ajax({
            url: "/TaxChallanEntry/TaxChallanSaveEdit",
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (res) {
                console.log(res);
                if (res.isSuccess) {
                    showToast("success", res.message);
                }
                if (!res.isSuccess) {
                    showToast("error", res.message);
                }
               

            },
            error: function (e) {
                console.log(e);
            },
                complete: function() {
                taxDipositAutoId();
                LoadChallanEntryGrid();
                resetFormData();
                }
        });
    })
    //#endregion
    //#region reset from

    $(document).on('click', "#js-clear", function () {
        resetFormData();
    })
    function resetFormData() {
        taxDipositAutoId();
       
        $("#Setup_TaxDepositCode").val('');
        $("#Setup_TaxDepositId").val('');
        //$("#Setup_FinancialCodeNo").val('');
        $("#Setup_ChallanAmount").val('');
        $("#Setup_SalaryMonth").val(new Date().getMonth()+1);
        $("#Setup_SalaryYear").val(new Date().getFullYear());
        $("#Setup_TaxChallanNo").val('');
        //$("#Setup_TaxChallanDate").val('');
        $("#Setup_BankId").val('').trigger('change');
        $("#Setup_BankBranchId").val('');
        $("#Setup_Remark").val('');
       
        if (typeof taxChallanDatePicker !== 'undefined') {
            taxChallanDatePicker.clear(); 
        }
        initTaxChallanDatePicker();

        $(".employee-select").prop("checked", false);
        $("#selectAll").prop("checked", false).prop("indeterminate", false);
      
        $(".row-select").prop("checked", false);
        $("#selectTaxChallanAll").prop("checked", false).prop("indeterminate", false);


        // status reset
        $("#selectedEmployeesStatus").text("Selected: 0 employees");

        // Button disable
        $("#generateChallan").prop("disabled", true);
        FinancialYear();
        $(".createDate").text("");
        $(".updateDate").text("");
    }

    //#endregion 

    //#region get form data
    function getFormData() {
        var formData = {
            TaxDepositCode: parseInt($("#Setup_TaxDepositCode").val())||0,
            TaxDepositId: $("#Setup_TaxDepositId").val(),
            FinancialCodeNo: $("#Setup_FinancialCodeNo").val(),
            TaxDepositAmount: parseInt($("#Setup_ChallanAmount").val()),
            SalaryMonth: $("#Setup_SalaryMonth").val(),
            SalaryYear: $("#Setup_SalaryYear").val(),
            //TaxChallanNoPrefix :$("#").val(),
            TaxChallanNo: $("#Setup_TaxChallanNo").val(),
            TaxChallanDate: formatDate($("#Setup_TaxChallanDate").val()),
            BankId: $("#Setup_BankId").val(),
            BankBranchId: $("#Setup_BankBranchId").val(),
            //ApprovedStatus :$("#").val(),
            Remark: $("#Setup_Remark").val(),
            //CompanyCode :$("#").val(),
            ChallanAmount: parseInt($("#Setup_ChallanAmount").val()),
            EmployeeIds: getSelectedEmployees(),
        }
        return formData;
    }

    function formatDate(dateStr) {
        if (!dateStr) return null;
        let d = new Date(dateStr);
        if (isNaN(d.getTime())) return null;

        return d.toISOString();
    }

    //#endregion


    ////#region financial year 2024-2025
    //function FinancialYear() {
    //    const startYear = 2020;
    //    const endYear = 2030
    //    const $dropdown = $("#Setup_FinancialCodeNo");
    //    $dropdown.empty();
    //    for (let y = startYear; y < endYear; y++) {
    //        $dropdown.append(
    //            $("<option>", {
    //                value: `${y}-${y + 1}`,
    //                text: `${y}-${y + 1}`
    //            })
    //        );
    //    }
    //    const currentYear = new Date().getFullYear();
    //    const defaultValue = `${currentYear}-${currentYear + 1}`;
    //    $dropdown.val(defaultValue);
    //};
    ////#endregion

    //#region bank id get branch and address
    $(document).on('change', '#Setup_BankId', function () {
        var bankId = $(this).val();
        $.ajax({
            url: "/TaxChallanEntry/GetBankDetails",
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(bankId),
            success: function (res) {
                var $branchOption = $("#Setup_BankBranchId");
                $branchOption.empty();
                $("#showBranchAddress").val('');
                if (res.length > 0) {
                    res.forEach((item) => {
                        $branchOption.append(
                            $("<option>", {
                                value: item.bankBranchId,
                                text: item.bankBranchName
                            })
                        );
                    });
                    $("#showBranchAddress").val(res[0].bankBranchAddress);
                }
            },
            error: function (e) {
            }
        });
    })
    //#endregion
    //#region branch id get address
    $(document).on('change', '#Setup_BankBranchId', function () {
        var branchId = $(this).val();
        $.ajax({
            url: "/TaxChallanEntry/GetBankBranchAddress",
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(branchId),
            success: function (res) {
                $("#showBranchAddress").val(res);
            },
            error: function (e) {
            }
        });
    })
    //#endregion

    //#region Set Year
    $(document).ready(function () {
        const currentYear = new Date().getFullYear();
        $("#Setup_SalaryYear").val(currentYear);
    })
    //#endregion

    //#region tax auto id
    function taxDipositAutoId() {
        $.ajax({
            url: "/TaxChallanEntry/TaxDipositAutoId",
            type: "GET",
            success: function (res) {
                $("#Setup_TaxDepositId").val(res);
            },
            error: function (e) {
            }
        });
    }

    //#endregion
    let selectedEmployees = [];
 
    function LoadChallanEntryGrid() {
        // Destroy existing table if it exists
        if ($.fn.DataTable.isDataTable('#TaxChallanEntryTable')) {
            $('#TaxChallanEntryTable').DataTable().destroy();
        }

        $('#TaxChallanEntryTable').DataTable({
            serverSide: true,
            processing: true,
            destroy: true,
            ajax: {
                url: "/TaxChallanEntry/GetchallanEntryGrid",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: function (d) {
                    // Ensure all required properties are sent
                    var requestData = {
                        Draw: d.draw || 0,
                        Start: d.start || 0,
                        Length: d.length || 10,
                        Search: {
                            Value: (d.search && d.search.value) ? d.search.value : "",
                            Regex: (d.search && d.search.regex) ? d.search.regex : false
                        },
                        Columns: d.columns || [],
                        Order: d.order || []
                    };

                    console.log("Sending request data:", requestData); // Debug log
                    return JSON.stringify(requestData);
                },
                error: function (xhr, error, code) {
                    console.error("AJAX Error:", error);
                    console.error("Response:", xhr.responseText);
                    alert("Error loading data: " + error);
                }
            },
            columns: [
                {
                    data: "taxDepositCode",
                    className: "text-center",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<input type="checkbox" class="row-select" value="${data}" />`;
                    }
                },
                {
                    data: "taxDepositId",
                    className: "text-center",
                    render: function (data, type, row) {
                        return `<a href="#defaultScroll" class="view-tax-chalan-row" data-id="${row.taxDepositCode}">${data || ''}</a>`;
                    }
                },
                { data: "employeeId", className: "text-center" },
                { data: "empName" },
                { data: "designationName" },
                { data: "taxChallanNo", className: "text-center" },
                {
                    data: "taxDepositAmount",
                    className: "text-center",
                    render: function (data, type, row) {
                        return data ? parseFloat(data).toFixed(2) : '0.00';
                    }
                },
                { data: "showTaxChallanDate", className: "text-center" },
                { data: "salaryMonthName", className: "text-center" },
                { data: "salaryYear", className: "text-center" },
                { data: "showFinancialCodeNo", className: "text-center" },
                { data: "remark" }
            ],
            lengthMenu: [[5, 10, 25, 50, 100, 1000], [5, 10, 25, 50, 100, 1000]],
            pageLength: 10,
            order: [[0, 'desc']], 
            language: {
                processing: "Loading...",
                emptyTable: "No data available",
                info: "Showing _START_ to _END_ of _TOTAL_ entries",
                infoEmpty: "Showing 0 to 0 of 0 entries",
                infoFiltered: "(filtered from _MAX_ total entries)",
                lengthMenu: "Show _MENU_ entries",
                search: "Search:",
                paginate: {
                    first: "First",
                    last: "Last",
                    next: "Next",
                    previous: "Previous"
                }
            }
        });
    }



        $(document).on('click', '.view-tax-chalan-row', function (e) {
            e.preventDefault();

            var target = $("#defaultScroll");

            if (target.length) {
                $('html, body').animate({
                    scrollTop: target.offset().top - 60
                }, 100);
            }

            var table = $('#TaxChallanEntryTable').DataTable();
            var rowData = table.row($(this).closest('tr')).data();
          
          


            if (rowData && rowData.taxChallanDate) {
                taxChallanDatePicker.setDate(rowData.taxChallanDate, true);
            }

            console.log(rowData);
            $("#Setup_ChallanAmount").val(rowData.taxDepositAmount);
            $("#Setup_TaxDepositCode").val(rowData.taxDepositCode);
            $("#Setup_TaxDepositId").val(rowData.taxDepositId);
            $("#Setup_FinancialCodeNo").val(rowData.financialCodeNo);
            $("#Setup_SalaryMonth").val(rowData.salaryMonth);
            $("#Setup_SalaryYear").val(rowData.salaryYear);
            $("#Setup_TaxChallanNo").val(rowData.taxChallanNo);
            $("#Setup_BankId").val(rowData.bankId).trigger('change');
            //$("#Setup_BankBranchId").val(rowData.bankBranchId);
            $("#Setup_Remark").val(rowData.remark);
            $(".createDate").text(rowData.showCreateDate);
            $(".updateDate").text(rowData.showModifyDate);

            selectedEmployees = [];
            $(".row-select").prop('checked', false);
            $("#selectTaxChallanAll").prop('checked', false);
            console.log(rowData.taxDepositCode);
            selectedEmployees.push(rowData.taxDepositCode + "");

        });


    // Button click e selected IDs console korte
    $('#js-tax-challan-delete-confirm').on('click', function () {
        var selectedIds;
        if (selectedEmployees.length == 1) {
            selectedIds = selectedEmployees;
        } else {
            selectedIds = window.getSelectedTaxChallanEmployees();
        }
        //console.log("Selected Employee IDs:", selectedIds);
        $.ajax({
            url: "/TaxChallanEntry/DeleteTaxChallanEntryGrid",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(selectedIds),
            success: function (res) {
                console.log(res);
                if (res.isSuccess) {
                    showToast('success', res.message);
                } else {
                    showToast('error', res.message);
                }
                selectedEmployees = [];
            }, error: function (e) {
                console.log(e);
                selectedEmployees = [];
            }, complete: function () {
                resetFormData();
                taxDipositAutoId();
                LoadChallanEntryGrid();
            }
        })


    });


    // Select All Checkbox handle
    //$(document).on("change", "#selectTaxChallanAll", function () {
    //    var isChecked = $(this).is(":checked");
    //    $(".row-select").prop("checked", isChecked);
    //});
    function initializeTaxChallanTableEvents() {
        // Select All functionality
        $('#TaxChallanEntryTable').on('change', '#selectTaxChallanAll', function () {
            const isChecked = $(this).prop('checked');
            $('.row-select:visible').prop('checked', isChecked);
            updateTaxChallanSelectedCount();
        });

        // Individual checkbox change
        $('#TaxChallanEntryTable').on('change', '.row-select', function () {
            updateSelectAllTaxChallanState();
            updateTaxChallanSelectedCount();
        });
    }

    function updateSelectAllTaxChallanState() {
        const totalVisible = $('.row-select:visible').length;
        const selectedVisible = $('.row-select:visible:checked').length;
        const selectAllCheckbox = $('#selectTaxChallanAll');

        if (selectedVisible === 0) {
            selectAllCheckbox.prop('indeterminate', false);
            selectAllCheckbox.prop('checked', false);
        } else if (selectedVisible === totalVisible) {
            selectAllCheckbox.prop('indeterminate', false);
            selectAllCheckbox.prop('checked', true);
        } else {
            selectAllCheckbox.prop('indeterminate', true);
        }
    }

    function updateTaxChallanSelectedCount() {
        const selectedCount = $('.row-select:checked').length;

        // Update status display if element exists
        const statusElement = $('#selectedEmployeesStatus');
        if (statusElement.length) {
            statusElement.text(`Selected: ${selectedCount} employees`);
        }

        // Enable/disable buttons based on selection
        const generateBtn = $('#generateChallan');
        if (generateBtn.length) {
            generateBtn.prop('disabled', selectedCount === 0);
        }
    }

    // Global function to get selected employees
    window.getSelectedTaxChallanEmployees = function () {
        selectedEmployees = [];
        $('.row-select:checked').each(function () {
            selectedEmployees.push($(this).val());
        });
        return selectedEmployees;
    };


    //#endregion

    //#region financial year
    function FinancialYear() {
        $.ajax({
            url: "/TaxChallanEntry/FinancialYearGet",
            type: "GET",
            success: function (res) {

                if (res.length >0) {
                    const $dropdown = $("#Setup_FinancialCodeNo");
                    $dropdown.empty();
                    $.each(res, function (index, item) {
                        $dropdown.append(
                            $("<option>", {
                                value: item.financialCodeNo,
                                text:item.name
                            })
                        )
                    })
                    const defaultValue = res[res.length - 1];

                    $dropdown.val(defaultValue.financialCodeNo);
                   
                }
            }, error: function (e) {
                console.log(e);
            }
        })
    }
    //#endregion

    //#region init
    $(document).ready(function () {
        taxDipositAutoId();
        LoadChallanEntryGrid();
        initializeTaxChallanTableEvents();
        FinancialYear();
    })
    //#endregion
})(jQuery);