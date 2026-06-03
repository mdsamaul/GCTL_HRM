
(function ($) {
    $.patientTypes = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            CompanyMultiSelectInput: "#companyMultiselectInput",
            CompanyDropdown: "#companyMultiselectDropdown",
            EmployeeDropdown: "#employeeMultiselectDropdown",
            EmployeeMultiselectInput: "#employeeMultiselectInput",
            EmployeeShowName:".EmployeeShowName",
            EmployeeShowDesignation: ".EmployeeShowDesignation",
            EmployeeShowDepartment: ".EmployeeShowDepartment",
            EmployeeShowJoiningDate: ".EmployeeShowJoiningDate",
            EmployeeArrow: '#employeeMultiselectArrow',
            CompanyArrow: '#companyMultiselectArrow',
            LoanSelectInput:"#loanSelectInput",
            LoanSelectItem: "#loanSelectItem",
            LoanArrow: "#loanArrow",
            LoanId:".loanId",
            LoanDate:".loanDate",
            LoanType:".loanType",
            LoanAmount:".loanAmount",
            LoanStartEndDate:".loanStartEndDate",
            LoanInstallment: ".loanInstallment",
            DateFrom: "#dateFrom",
            ToDate: "#toDate",
            MonthDeduction:".monthDeduction",
            DeductionHead: ".deductionHead",
            SaveBtn: ".js-Advance-loan-adjustment-save",
            AdjustmentAutoId: "#AdjustmentAutoId",
            Month: ".month",
            Year: ".year",
            Remarks: ".custom-remarks-textarea",
            AdjustmentMultiselectInput: "#multiselectInput",
            ClearBtn: "#js-Advance-loan-adjustment-clear",
            DeleteBtn: "#js-Advance-loan-adjustment-delete-confirm",
            AdvancePayCodeId: "#advancePayCodeId",
            CreateDateShow:"#createDateShow",
            ModifyDateshow:"#modifyDateshow",
        }, options);
        var filterUrl = commonName.baseUrl + "/GetFilterData";
        var getAllAndFilterCompanyUrl = commonName.baseUrl + "/GetAllAndFilterCompany";
        var GetEmployeesByFilterUrl = commonName.baseUrl + "/GetEmployeesByFilter";
        var GetLoadEmployeeByIdUrl = commonName.baseUrl + "/GetLoadEmployeeById";
        var GetLoanByEmployeeIdUrl = commonName.baseUrl + "/GetLoanByEmployeeId";
        var GetLoanByIdUrl = commonName.baseUrl + "/GetLoanById";
        var SaveUpdateLoanAdjustmentUrl = commonName.baseUrl + "/SaveUpdateLoanAdjustment";
        var AdjustmentAutoGanarateIdUrl = commonName.baseUrl + "/AdjustmentAutoGanarateId";
        var GetMonthUrl = commonName.baseUrl + "/GetMonth";
        var GetHeadDeductionUrl = commonName.baseUrl + "/GetHeadDeduction";
        var GetAdvancePayDataUrl = commonName.baseUrl + "/GetAdvancePayData"
        var DeleteAdvancePayUrl = commonName.baseUrl +"/DeleteAdvancePay"
        function stHeader() {
            window.addEventListener('scroll', function () {
                const header = document.getElementById('stickyHeader');
                if (window.scrollY > 0) {
                    header.classList.add('scrolled');
                } else {
                    header.classList.remove('scrolled');
                }
            });
        }
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

        let dateFromPicker = null;
        let toDatePicker = null;

        var $input = $('#multiselectInput');
        var $adjustmentArrow = $('#multiselectArrow');
       
        var $dropdown = $('#multiselectDropdown');
        var $selectedValuesSpan = $('#selectedValues');

        var adjustmentOption = [
            { value: 'Advance', label: 'Advance' },
            { value: 'Loan', label: 'Loan' }          
        ];
        function adjustmentGenerateOptions(options) {
            $dropdown.empty();

            $.each(options, function (index, option) {
                var $optionDiv = $('<div>', {
                    class: 'multiselect-option',
                    text: option.label,
                    'data-value': option.value
                });

                $optionDiv.on('click', function () {
                    $input.val(option.label);
                    $input.attr("data-selected-value", option.value);
                    $dropdown.removeClass('show');
                    $adjustmentArrow.removeClass('rotated');

                    updateAdjustmentSelectedValues();
                    logAdjustmentSelectedValues();
                });

                $dropdown.append($optionDiv);
            });
        }

        $(document).on('input', commonName.LoanAmount, function () { 
            if ($(this).val() < 0) {
                $(commonName.LoanAmount).css('border', '1px solid red');
            } else {
                $(commonName.LoanAmount).css('border', '');
            }
            $(commonName.MonthDeduction).val($(this).val());            
        })
        var loanAdjustment = false;
        function logAdjustmentSelectedValues() {
            var selectedValue = $input.attr('data-selected-value');            
            if (!selectedValue) {
                adjustmentGenerateOptions(adjustmentOption);
                return;
            }

            // Clear employee input
            $employeeInput.val('');
            $employeeInput.attr("data-selected-value", '');
            $employeeDropdown.empty();

            // Set loan adjustment flag
            loanAdjustment = selectedValue === "Loan";

            if (!loanAdjustment) {
                ClearInputData();
                $(commonName.LoanSelectInput)
                    .removeClass('enable')
                    .addClass('disable');

                $('input[name="filterType"][value="By Month"]').prop('checked', true);


                //$(commonName.DateFrom).removeClass('enable').addClass('disable');
                //$(commonName.ToDate).removeClass('enable').addClass('disable');
                toggleFlatpickr(dateFromPicker, false);
                toggleFlatpickr(toDatePicker, false);
                $(commonName.Month).removeClass('disable').addClass("enable");
                $(commonName.Year).removeClass('disable').addClass("enable");
                $(commonName.LoanAmount).removeClass('disable').addClass("enable");
                $(commonName.LoanInstallment).removeClass('enable').addClass("disable").val("1");
                $(commonName.MonthDeduction).removeClass('enable').addClass("disable");
                $(commonName.DeductionHead).removeClass('disable').addClass("enable");
                SelectDropdownMonth();
                SelectDropdownHeadDeduction();
                $(commonName.Year).val(new Date().getFullYear());
            }

            if (loanAdjustment) {
                ClearInputData();
                $('input[name="filterType"][value="By Date"]').prop('checked', true);
                $(commonName.LoanSelectInput).removeClass('disable').addClass('enable');
                //$(commonName.DateFrom).removeClass('disable').addClass('enable');
                //$(commonName.ToDate).removeClass('disable').addClass('enable');
                toggleFlatpickr(dateFromPicker, true);
                toggleFlatpickr(toDatePicker, true);
                $(commonName.Month).removeClass('enable').addClass('disable');
                $(commonName.Year).removeClass('enable').addClass('disable');
                $(commonName.MonthDeduction).removeClass('enable').addClass('disable');
                $(commonName.LoanAmount).removeClass('enable').addClass("disable");
                $(commonName.LoanInstallment).removeClass('enable').addClass("disable");
                $(commonName.MonthDeduction).removeClass('enable').addClass("disable");
                $(commonName.DeductionHead).removeClass('enable').addClass("disable");
            }



            // Reload employee filter and re-bind employee dropdown
            setTimeout(function () {
                getEmployeeByFilter('');
                initEmployeeMultiselect(); 
                $(commonName.EmployeeArrow).addClass('rotated');
            }, 100);
        }
        //month
        SelectDropdownMonth = function () {
            $.ajax({
                url: GetMonthUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.Month).empty();

                    $(commonName.Month).append(`<option value="">--Select Month--</option>`);
                    if (res.length > 0) {
                        $.each(res, function (index, month) {
                            $(commonName.Month).append(`<option value="${month.monthName}">${month.monthName}</option>  `);
                        })
                    }
                },
                error: function (e) {
                }
            });
        }
        //head deduction
        SelectDropdownHeadDeduction = function () {
            $.ajax({
                url: GetHeadDeductionUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.DeductionHead).empty();

                    $(commonName.DeductionHead).append(`<option value="">--Select Deduction Head--</option>`);

                    if (res.length > 0) {
                        $.each(res, function (index, dHead) {
                            $(commonName.DeductionHead).append(`<option value="${dHead.name}">${dHead.name}</option>  `);
                        })
                    }
                },
                error: function (e) {

                }
            });
        }
       

        // Enhanced multiselect initialization
        function initMultiselect() {
            adjustmentGenerateOptions(adjustmentOption);

            // Input click handler
            $input.on('click', function (e) {
                e.stopPropagation();

                // Toggle current dropdown
                $dropdown.toggleClass('show');

                // Hide other dropdowns
                $companyDropdown.removeClass('show');
                $employeeDropdown.removeClass('show');
                $loanDropdown.removeClass('show');

                // Toggle arrow rotation
                $adjustmentArrow.toggleClass('rotated');

                // Remove validation error when user interacts
                $(this).removeClass('validation-error');
            });

            // Enhanced document click handler for outside clicks
            $(document).on('click', function (event) {
                // Check if click is outside the multiselect container
                if ($(event.target).closest('.custom-multiselect').length === 0) {
                    hideAdjustmentDropdown();
                }
            });

            // Prevent dropdown from closing when clicking inside
            $dropdown.on('click', function (event) {
                event.stopPropagation();
            });

            updateAdjustmentSelectedValues();
            logAdjustmentSelectedValues();
        }

        // Function to hide adjustment dropdown
        function hideAdjustmentDropdown() {
            $dropdown.removeClass('show');
            $adjustmentArrow.removeClass('rotated');
        }

        // Enhanced option generation with better click handling
        function adjustmentGenerateOptions(options) {
            $dropdown.empty();

            $.each(options, function (index, option) {
                var $optionDiv = $('<div>', {
                    class: 'multiselect-option',
                    text: option.label,
                    'data-value': option.value
                });

                // Enhanced option click handler
                $optionDiv.on('click', function (e) {
                    e.stopPropagation();

                    // Set selected value
                    $input.val(option.label);
                    $input.attr("data-selected-value", option.value);

                    // Remove validation error class
                    $input.removeClass('validation-error');

                    // Hide dropdown
                    hideAdjustmentDropdown();

                    // Update and log selected values
                    updateAdjustmentSelectedValues();
                    logAdjustmentSelectedValues();

                });

                $dropdown.append($optionDiv);
            });
        }

        // Enhanced update selected values function
        function updateAdjustmentSelectedValues() {
            var selectedValue = $input.attr('data-selected-value');
            var displayText = selectedValue ? selectedValue : 'None';
            $selectedValuesSpan.text(displayText);

            // Add visual feedback for selection
            if (selectedValue) {
                $input.addClass('has-selection');
            } else {
                $input.removeClass('has-selection');
            }
        }

        // Function to programmatically open dropdown (useful for validation)
        function openAdjustmentDropdown() {
            $dropdown.addClass('show');
            $adjustmentArrow.addClass('rotated');

            // Hide other dropdowns
            $companyDropdown.removeClass('show');
            $employeeDropdown.removeClass('show');
            $loanDropdown.removeClass('show');
        }
           

        $input.on('keydown', function (e) {
            switch (e.keyCode) {
                case 13: // Enter
                    if ($dropdown.hasClass('show')) {
                        // Select first option if dropdown is open
                        $dropdown.find('.multiselect-option:first').click();
                    } else {
                        // Open dropdown if closed
                        openAdjustmentDropdown();
                    }
                    e.preventDefault();
                    break;
                case 27: // Escape
                    hideAdjustmentDropdown();
                    e.preventDefault();
                    break;
                case 40: // Down arrow
                    if (!$dropdown.hasClass('show')) {
                        openAdjustmentDropdown();
                    }
                    e.preventDefault();
                    break;
            }
        });


        //company

        var $companyInput = $(commonName.CompanyMultiSelectInput);
        var $companyDropdown = $(commonName.CompanyDropdown);

        function GetAllCompany(SearchCompanyText) {
            $.ajax({
                url: getAllAndFilterCompanyUrl,
                type: "GET",
                data: { companyName: SearchCompanyText },
                success: function (res) {
                    renderCompanyOptions(res);
                },
                error: function (e) {
                }
            });
        }

        function renderCompanyOptions(companies) {
            $companyDropdown.empty();

            if (!companies || companies.length === 0) {
                $companyDropdown.append(`<div class="multiselect-option">Company Not Found</div>`);
                return;
            }

            $.each(companies, function (index, company) {
                var $optionDiv = $('<div>', {
                    class: 'multiselect-option',
                    text: company.companyName,
                    'data-value': company.companyCode
                });

                // Click event to set value
                $optionDiv.on('click', function () {
                    var selectedName = $(this).text();
                    var selectedValue = $(this).data("value");

                    $companyInput.val(selectedName);
                    $companyInput.attr("data-selected-value", selectedValue);
                    $companyDropdown.removeClass('show');
                    $(commonName.CompanyArrow).removeClass('rotated');
                });

                $companyDropdown.append($optionDiv);
            });

            if (companies.length > 0) {
                var firstCompay = companies[0];
                $companyInput.val(firstCompay.companyName);
                $companyInput.attr("data-selected-value", firstCompay.companyCode);
            }
        }

        function initCompanyMultiselect() {
            $companyInput.on('click', function () {
                $companyDropdown.toggleClass('show');
                $dropdown.removeClass('show');
                $employeeDropdown.removeClass('show');
                $loanDropdown.removeClass('show');
                $(commonName.CompanyArrow).addClass('rotated');
            });

            $companyInput.on('input', function () {
                var searchText = $(this).val();
                GetAllCompany(searchText);
            });

            // Hide dropdown when clicking outside
            $(document).on('click', function (e) {
                if (!$(e.target).closest('.custom-multiselect').length) {
                    $companyDropdown.removeClass('show');
                    $(commonName.CompanyArrow).removeClass('rotated');
                    
                }
            });

            $companyDropdown.on('click', function (e) {
                e.stopPropagation();
            });

            GetAllCompany('');
        }
               

        //get employee

     
        var $employeeInput = $(commonName.EmployeeMultiselectInput);
        var $employeeDropdown = $(commonName.EmployeeDropdown);
        function getEmployeeByFilter(SearchEmployeeText) {
            $.ajax({
                url: GetEmployeesByFilterUrl,
                type: 'GET',
                data: {
                    employeeStatusId: '01',
                    companyCode: '001',
                    employeeName: SearchEmployeeText,
                    loanAdjustment: loanAdjustment
                },
                success: function (data) {
                    renderEmployeeOptions(data);   
                    
                }, error: function (e) {
                   
                }
            });
        }

        function renderEmployeeOptions(employees) {
            $employeeDropdown.empty();
            if (!employees || employees.length === 0) {
                $employeeDropdown.append(`<div class="EmployeeMultiselect-option">Employee Not Found</div>`);
                return;
            }

            $.each(employees, function (index, employee) {
                var $loanId = "";
                if (employee.loanId != null) {
                    $loanId = `(${employee.loanId})`
                }
                var $optionEmpDiv = $('<div>', {
                    class: 'employee-list-item',
                    html: `
                <div style="padding: 6px 10px; cursor: pointer; border-bottom: 1px solid #eee;" 
                     data-value="${employee.employeeId}">
                    <strong>${employee.fullName}</strong>
                    <small>( ${employee.employeeId} )</small>
                    <span>${$loanId}</span>
                </div>
            `
                });

                
                $optionEmpDiv.on('click', function () {
                    var selectedEmpName = $(this).find("strong").text().trim(); 
                    var selectedEmpId = $(this).find("small").text().replace(/[()]/g, '').trim(); 

                    var displayText = `${selectedEmpName} (${selectedEmpId})`;

                    $employeeInput.val(displayText);
                    $employeeInput.attr("data-selected-value", selectedEmpId);

                    $employeeDropdown.removeClass('show');
                    LoadEmployeeById(selectedEmpId);
                    $(commonName.EmployeeArrow).removeClass('rotated');
                });

                $employeeDropdown.append($optionEmpDiv);
            });
        }


        function initEmployeeMultiselect() {
            $employeeInput.off('click').on('click', function () {
                if (!$employeeDropdown.hasClass('show')) {
                    $employeeDropdown.addClass('show');
                    $(commonName.EmployeeArrow).addClass('rotated');
                }

                // Hide others
                $companyDropdown.removeClass('show');
                $dropdown.removeClass('show');
                $loanDropdown.removeClass('show');
            });

            $employeeInput.off('input').on('input', function () {
                var searchText = $(this).val();
                getEmployeeByFilter(searchText);
            });

            $(document).off('click.employeeDropdown').on('click.employeeDropdown', function (e) {
                if (!$(e.target).closest('.custom-multiselect').length) {
                    $employeeDropdown.removeClass('show');
                    $(commonName.EmployeeArrow).removeClass('rotated');
                }
            });

            $employeeDropdown.on('click', function (e) {
                e.stopPropagation();
            });

            getEmployeeByFilter('');
        }


        LoadEmployeeById = function (employeeId) {
            $.ajax({
                url:GetLoadEmployeeByIdUrl,
                type: 'POST',
                data: { employeeId },
                success: function (res) {
                    if (res != null) {
                        LoadLoanByEmployeeId(res.employeeId);
                        $(commonName.EmployeeShowName).text(res.fullName);
                        $(commonName.EmployeeShowDesignation).text(res.designationName);
                        $(commonName.EmployeeShowDepartment).text(res.departmentName);
                        $(commonName.EmployeeShowJoiningDate).text(res.joiningDate);
                    }
                },
                error: function (e) {

                }
            });
        }

        //loan id

        var $loanInput = $(commonName.LoanSelectInput);
        var $loanDropdown = $(commonName.LoanSelectItem);
        LoadLoanByEmployeeId = function (employeeId) {
            $.ajax({
                url: GetLoanByEmployeeIdUrl,
                type: 'GET',
                data: { employeeId },
                success: function (res) {
                    if (res != null) {
                        RenderLoanOption(res);
                    }
                },
                error: function (e) {

                }
            });
        }

        RenderLoanOption = function (loans) {
            $loanDropdown.empty();
            $loanInput.val("--Select Loan--");
            $loanDropdown.removeClass('show');
            if (!loans || loans.length === 0) {
                $loanDropdown.append(`<div class="loanMultiselectOption">Loan Id Not Found</div>`);                
                return;
            }

            $.each(loans, function (index, loan) {
                var $optionLoanDiv = $('<div>', {
                    class: 'loan-list-item',
                    html: `
                <div style="padding:6px 10px; cursor:pointer; border-bottom:1px solid #eee;" data-value="${loan.loanId}">
                    <strong>${loan.loanType}</strong>
                    <small>(${loan.loanId})</small>
                </div>
            `
                });

                $optionLoanDiv.on('click', function () {
                    var selectedLoanName = $(this).find("strong").text().trim();
                    var selectedLoanId = $(this).find("small").text().replace(/[()]/g, '').trim();
                    var displayText = `${selectedLoanName} (${selectedLoanId})`;
                    $loanInput.val(displayText);
                    $loanInput.attr("data-selected-value", selectedLoanId);
                    $loanDropdown.removeClass('show');
                    GetLoanById(selectedLoanId);
                    $(commonName.LoanArrow).removeClass('rotated'); 
                });

                $loanDropdown.append($optionLoanDiv);
            });
        }
        function initLoan() {
            $loanInput.on('click', function () {
                $loanDropdown.toggleClass('show');
                $companyDropdown.removeClass('show');
                $dropdown.removeClass('show');
                $employeeDropdown.removeClass('show');
                $(commonName.LoanArrow).toggleClass('rotated');
            });

            $loanInput.on('input', function () {
                var searchText = $(this).val();
                LoadLoanByEmployeeId(searchText);
            });

            $(document).on('click', function (e) {
                if (!$(e.target).closest('.custom-multiselect').length) {
                    $loanDropdown.removeClass('show');
                    $(commonName.LoanArrow).removeClass('rotated');
                }
            });

            $loanDropdown.on('click', function (e) {
                e.stopPropagation();
            });

            LoadLoanByEmployeeId('');
        }
        GetLoanById = function (loanId) {
            $.ajax({
                url: GetLoanByIdUrl,
                type: "GET",
                data: { loanId },
                success: function (res) {
                    if (res) {
                        $(commonName.DeductionHead).empty();
                        $(commonName.LoanId).text(res.loanId);
                        $(commonName.LoanDate).text(res.loanDate);
                        $(commonName.LoanType).text(res.loanType);
                        $(commonName.LoanAmount).text(res.loanAmount);
                        $(commonName.LoanAmount).val(res.loanAmount);
                        $(commonName.LoanStartEndDate).text(res.loanStartEndDate);
                        $(commonName.LoanInstallment).text(res.noOfInstallment);
                        $(commonName.LoanInstallment).val(res.noOfInstallment);
                        customDateFlatpicker($(commonName.DateFrom)[0], res.starDate);
                        customDateFlatpicker($(commonName.ToDate)[0], res.endDate);
                        $(commonName.MonthDeduction).val(res.monthlyDeduction);
                        $(commonName.DeductionHead).append(`<option value="${res.payHeadNameName}">${res.payHeadNameName}</option>  `);
                    }
                }, error: function (e) {
                }
            });
        }
        
        //customDateFlatpicker = function (id, date) {   
            
        //    flatpickr(id, CalendarService.createConfig(
        //        {
        //            dateFormat: "d/m/Y",
        //            defaultDate: date ?? "today",
        //            allowInput: false
        //        }
        //    ));
        //}
        customDateFlatpicker = function (id, date) {

            let fp = flatpickr(id, CalendarService.createConfig({
                dateFormat: "d/m/Y",
                defaultDate: date ?? "today",
                allowInput: false
            }));

            // store reference
            if (id === commonName.DateFrom) {
                dateFromPicker = fp;
            }

            if (id === commonName.ToDate) {
                toDatePicker = fp;
            }
        };

        function toggleFlatpickr(picker, isEnable) {
            if (!picker) return;

            if (isEnable) {
                picker._input.disabled = false;
                picker.set('clickOpens', true);
            } else {
                picker._input.disabled = true;
                picker.set('clickOpens', false);
            }
        }

        AdjustmentAutoGanarateId = function () {
            $.ajax({
                url: AdjustmentAutoGanarateIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.AdjustmentAutoId).val(res);
                }, error: function (e) {

                }
            });
        }
        //create

        CustomFlatDate = function (dateStr) {
            if (!dateStr) return null;
            var parts = dateStr.split('/');
            if (parts.length !== 3) return null;
            return `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}`;
        }
               
        CreateInputData = function () {
            var FormValue = {
                AdvancePayCode: $(commonName.AdvancePayCodeId).val(),
                AdjustmentType: $input.attr('data-selected-value'),
                AdvancePayId: $(commonName.AdjustmentAutoId).val(),
                EmployeeID: $(commonName.EmployeeMultiselectInput).attr("data-selected-value"),
                AdvanceAdjustStatus: $('input[name="filterType"]:checked').val(),
                AdvanceAmount: $(commonName.LoanAmount).val(),
                MonthlyDeduction: $(commonName.MonthDeduction).val(),
                SalaryMonth: $(commonName.Month).val(),
                SalaryYear: $(commonName.Year).val(),
                NoOfPaymentInstallment: $(commonName.LoanInstallment).val(),
                PayHeadNameId: $(commonName.DeductionHead).val(),
                Remarks: $(commonName.Remarks).val(),
                LoanID: $(commonName.LoanSelectInput).attr('data-selected-value'),
                FromDate: CustomFlatDate($(commonName.DateFrom).val()),
                ToDate: CustomFlatDate($(commonName.ToDate).val())
            }
            return FormValue;
        }

        $(document).on('click', commonName.ClearBtn, function () {

            ClearInputData();
            $(commonName.AdjustmentMultiselectInput).attr('data-selected-value', '').val(''); // EmployeeID
            $(commonName.EmployeeMultiselectInput).attr('data-selected-value', '').val(''); 
        })


        ClearInputData = function () {
            AdjustmentAutoGanarateId();
            $(commonName.AdjustmentAutoId).val(''); 
            $('input[name="filterType"][value="By Date"]').prop('checked', true);
            $(commonName.AdvancePayCodeId).val(0);

            $(commonName.LoanAmount).val(''); 
            $(commonName.MonthDeduction).val('');
            $(commonName.Month).val(''); 
            $(commonName.Year).val('');
            $(commonName.LoanInstallment).val(''); 
            $(commonName.DeductionHead).val(''); 
            $(commonName.Remarks).val(''); 
            $(commonName.LoanSelectInput).attr('data-selected-value', '').val('');
            customDateFlatpicker(commonName.DateFrom,null);
            customDateFlatpicker(commonName.ToDate, null);
            $(commonName.EmployeeShowName).text('');
            $(commonName.EmployeeShowDesignation).text('');
            $(commonName.EmployeeShowDepartment).text('');
            $(commonName.EmployeeShowJoiningDate).text('');
            $(commonName.LoanId).text('');
            $(commonName.LoanDate).text('');
            $(commonName.LoanType).text('');
            $(commonName.LoanAmount).text('');
            $(commonName.LoanStartEndDate).text('');
            $(commonName.LoanInstallment).text('');
            $(commonName.CreateDateShow).text('');
            $(commonName.ModifyDateshow).text('');
           
        }
               
        $(document).on('change', commonName.ToDate, function () {
            if (loanAdjustment) {
                var FormDatevalue = $(commonName.DateFrom).val();
                var toDatevalue = $(this).val();

                const FromMonth = parseInt(FormDatevalue.split('/')[1]);
                const FromYear = parseInt(FormDatevalue.split('/')[2])
                const ToMonth = parseInt(toDatevalue.split('/')[1]);
                const ToYear = parseInt(toDatevalue.split('/')[2]);              
                const totalMonths = (ToYear - FromYear) * 12 + ToMonth - FromMonth + 1;
                $(commonName.LoanInstallment).val(totalMonths);
                const loanAmount = $(commonName.LoanAmount).val()
                const monthlyDeduction =Math.ceil(loanAmount / totalMonths) ;
                $(commonName.MonthDeduction).val(monthlyDeduction);
            }              
            });
      

        $(document).on('click', commonName.SaveBtn, function () {
            var formData = CreateInputData();          

            // Enhanced validation with dropdown handling
            if (formData.AdjustmentType === undefined || formData.AdjustmentType === "" || formData.AdjustmentType == null) {
                // Force show dropdown by adding show class to dropdown element
                $dropdown.addClass('show');
                $adjustmentArrow.addClass('rotated');

                // Add validation error class to input
                $input.addClass('validation-error');

                // Hide other dropdowns
                $companyDropdown.removeClass('show');
                $employeeDropdown.removeClass('show');
                $loanDropdown.removeClass('show');

                // Focus on the input for better UX
                $input.focus();

                // Don't proceed with AJAX call
                return false;
            }


            if (formData.EmployeeID === undefined || formData.EmployeeID === "" || formData.EmployeeID == null) {
                    // Force show dropdown by adding show class to dropdown element
                $dropdown.removeClass('show');
                $adjustmentArrow.removeClass('rotated');

                    // Add validation error class to input
                    $input.addClass('validation-error');

                // Hide other dropdowns
                $companyDropdown.removeClass('show');
                $employeeDropdown.addClass('show');
                    $loanDropdown.removeClass('show');

                    // Focus on the input for better UX
                    $input.focus();

                    // Don't proceed with AJAX call
                    return false;
                }

            if (formData.AdvanceAdjustStatus === "") {
                showToast("warning","Please select adjustment status.");
                $('input[name="filterType"][value="By Date"]').prop('checked', true);

                document.getElementById("byDate").scrollIntoView({ behavior: "smooth", block: "center" });

                return;
            }
            if (!loanAdjustment && formData.AdvancePayCode == 0) {
                formData.AdvanceAmount = $("input.loanAmount.enable").val();             
            }

            if (formData.AdvanceAmount == "" || formData.AdvanceAmount == undefined || formData.AdvanceAmount == null) {
                $(commonName.LoanAmount).focus().css('border','1px solid red');
                return;
            }
            // If validation passed, remove error class
            $(commonName.AdjustmentMultiselectInput).removeClass('validation-error');           

            $.ajax({
                type: "POST",
                url: SaveUpdateLoanAdjustmentUrl,
                data: JSON.stringify(formData),
                contentType: "application/json; charset=utf-8",
                success: function (response) {
                    if (response.success) {
                        GridAdvanceLoan();
                        showToast('success', response.message);
                        ClearInputData();
                        $(commonName.AdjustmentMultiselectInput).attr('data-selected-value', '').val('');
                        $(commonName.EmployeeMultiselectInput).attr('data-selected-value', '').val('');
                    } else {
                        showToast('error', response.message);
                    }
                },
                error: function (xhr, status, error) {
                    alert("Ajax Error: " + error);
                },
                //complete: function () {  
                //    AdjustmentAutoGanarateId();
                //}
            });
        });


        // Updated GridAdvanceLoan function with proper error handling
        GridAdvanceLoan = function () {
            if ($.fn.DataTable.isDataTable('#advancePayTable')) {
                $('#advancePayTable').DataTable().clear().destroy();
            }
            var table = $('#advancePayTable').DataTable({
                "processing": true,
                "serverSide": true,
                "responsive": true,
                "pageLength": 10,
                ordering: true,
                columnDefs: [
                    { orderable: false, targets: [0] },
                    {
                        targets: '_all',
                        className: 'text-center align-middle'
                    }
                ],
                "scrollY": "600px",
                "scrollCollapse": true,
                "lengthMenu": [[5, 10, 25, 50, 100,1000], [5, 10, 25, 50, 100, 1000]],
                pagingType: 'full_numbers',
                "language": {
                    "processing": "Processing...",
                    "lengthMenu": "Show _MENU_ entries",
                    "zeroRecords": "No matching records found",
                    "info": "Showing _START_ to _END_ of _TOTAL_ entries",
                    "infoEmpty": "Showing 0 to 0 of 0 entries",
                    "infoFiltered": "(filtered from _MAX_ total entries)",
                    "search": "Search:",
                   
                    language: {
                        paginate: {
                            first: 'First',
                            previous: 'Previous',
                            next: 'Next',
                            last: 'Last'
                        }
                    },
                    "emptyTable": "No data available in table",
                    "loadingRecords": "Loading..."
                },
                "ajax": {
                    "url": GetAdvancePayDataUrl,
                    "type": "POST",
                    "data": function (d) {
                        // DataTables pagination parameters
                        d.page = Math.floor(d.start / d.length) + 1;
                        d.pageSize = d.length;
                        d.searchValue = d.search.value || "";
                        

                        // Custom filter parameters
                        d.department = $('#departmentFilter').val() || "";
                        d.month = $('#monthFilter').val() || "";
                        d.year = $('#yearFilter').val() || "";

                        return d;
                    },
                    "dataSrc": function (json) {

                        // Check for errors
                        if (json.error) {
                            alert('Error: ' + json.error);
                            return [];
                        }

                        // Validate response structure
                        if (!json.data) {
                            return [];
                        }

                        // Ensure data is an array
                        if (!Array.isArray(json.data)) {
                            return [];
                        }

                        // Set total records for pagination
                        json.recordsTotal = json.recordsTotal || 0;
                        json.recordsFiltered = json.recordsFiltered || json.recordsTotal || 0;

                        return json.data;
                    },
                    "error": function (xhr, error, thrown) {
                       

                        let errorMessage = 'Unknown error occurred';
                        try {
                            const response = JSON.parse(xhr.responseText);
                            errorMessage = response.error || response.message || errorMessage;
                        } catch (e) {
                            errorMessage = xhr.statusText || errorMessage;
                        }

                        alert('Error loading data: ' + errorMessage);
                    }
                },
                "columns": [
                    {
                        "data": null,
                        "title": `Select <br /><input type='checkbox' id='selectAllCheckbox'>`,
                        "render": function (data, type, row, meta) {
                            return `<input type="checkbox" class="row-checkbox" data-id="${row.advancePayCode}">`;
                        },
                        "orderable": false,
                        "width": "5%",
                        "className":"text-center"
                    },
                   
                    {
                        "data": "advancePayId",
                        "title": "ID",
                        "width": "5%",
                        "render": function (data, type, row, meta) {
                            if (!data) return "";
                            return `<a href="#" class="advance-id-link" data-advancepayid=${data}>${data}</a>`
                        }
                    },
                    {
                        "data": "employeeID",
                        "title": "Employee ID",                        
                    },
                    {
                        "data": "fullName",
                        "title": "Name",
                        "width": "15%",
                        "render": function (data) {
                            return data || '';
                        }
                    }, {
                        "data": "designationName",
                        "title": "Designation",
                        "width": "12%",
                        "render": function (data) {
                            return data || '';
                        }
                    },
                    {
                        "data": "loanID",
                        "title": "Loan ID",
                    },
                    
                   
                    {
                        "data": "advanceAmount",
                        "title": "Loan Amount",
                        "width": "10%",
                        "render": function (data) {
                            const amount = parseFloat(data) || 0;

                            return  amount.toLocaleString('en-US', {
                                minimumFractionDigits: 0,
                                maximumFractionDigits: 0
                            });
                            
                        }
                    },
                     {
                        "data": "noOfPaymentInstallment",
                        "title": "No. Of Inst(s)",
                        "width": "8%",
                        "render": function (data) {
                            return data || '';
                        }
                    },                   
                    {
                        "data": "monthlyDeduction",
                        "title": "Monthly Deduction",
                        "width": "10%",
                        "render": function (data) {
                            if (data == null || data === '') return '';
                            const amount = parseFloat(data) || 0;
                            return  amount.toLocaleString('en-US', {
                                minimumFractionDigits: 0,
                                maximumFractionDigits: 0
                            });
                        }
                    },
                    {
                        "data": "salaryMonth",
                        "title": "Salary Month",
                        "width": "8%",
                        "render": function (data) {
                            return data || '';
                        }
                    },
                    {
                        "data": "salaryYear",
                        "title": "Salary Year",
                        "width": "8%",
                        "render": function (data) {
                            return data || '';
                        }
                    }
                ],
                "order": [[1, "desc"]],
                "drawCallback": function (settings) {
                    $('[title]').tooltip();
                },
                "initComplete": function () {
                    $('#advancePayTable_filter input[type="search"]').attr('placeholder', 'Search Here..');
                }

            });

            return table;
        };

     
        //edit

        $(document).on('click', '.advance-id-link', function (e) {
            e.preventDefault();
          
            var id = $(this).data('advancepayid');
            var table = $('#advancePayTable').DataTable();
            var allRowsData = table.rows().data().toArray();
            $(commonName.MonthDeduction).removeClass('enable').addClass("disable");
            var rowData = allRowsData.find(row => row.advancePayId === id);


            if (rowData.advanceAdjustStatus == "By Month") {
                $(commonName.DateFrom).removeClass('enable').addClass('disable');
                $(commonName.ToDate).removeClass('enable').addClass('disable');
                $(commonName.LoanSelectInput).removeClass('enable').addClass('disable');
                $(commonName.LoanId).text('');
                $(commonName.LoanDate).text('');
                $(commonName.LoanType).text('');
                $(commonName.LoanAmount).text('');
                $(commonName.LoanStartEndDate).text('');
                $(commonName.LoanInstallment).text('');
                $(commonName.LoanSelectInput).attr('data-selected-value', '').val('');
            }
            if (rowData.advanceAdjustStatus == "By Date") {
                $(commonName.LoanId).text(rowData.loanID);
                $(commonName.LoanDate).text(rowData.loanDate);
                $(commonName.LoanType).text(rowData.loanTypeName);
                $(commonName.LoanAmount).text(rowData.advanceAmount);
                $(commonName.LoanStartEndDate).text(rowData.loanStartDate + " - " + rowData.loanEndDate);
                $(commonName.LoanInstallment).text(rowData.noOfPaymentInstallment);
                $(commonName.Month).removeClass('disable').addClass('enable');
                $(commonName.Year).removeClass('disable').addClass('enable');
                $(commonName.DateFrom).removeClass('enable').addClass('disable');
                $(commonName.ToDate).removeClass('enable').addClass('disable');
            }

            $(commonName.AdjustmentAutoId).val(rowData.advancePayId); // AdvancePayId
            $(commonName.AdjustmentMultiselectInput).attr('data-selected-value', `${rowData.adjustmentType}`).val(`${rowData.adjustmentType}`); // EmployeeID
            $(commonName.AdvancePayCodeId).val(rowData.advancePayCode);
            if (rowData) {
                const fullText = `${rowData.fullName} (${rowData.employeeID})` +
                    (rowData.loanID ? ` (${rowData.loanID})` : '');

                $(commonName.EmployeeMultiselectInput)
                    .attr('data-selected-value', rowData.employeeID)
                    .val(fullText);
            }
            if (rowData.advanceAdjustStatus == "By Date") {
                $('input[name="filterType"][value="By Date"]').prop('checked', true);
            }
            if (rowData.advanceAdjustStatus == "By Month") {
                $('input[name="filterType"][value="By Month"]').prop('checked', true);
            }


            $(commonName.LoanAmount).val(rowData.advanceAmount).prop('readOnly', true);
            $(commonName.MonthDeduction).val(rowData.monthlyDeduction).removeClass('disable');
            $(commonName.Month).val(rowData.salaryMonth);
            $(commonName.Year).val(rowData.salaryYear);
            $(commonName.LoanInstallment).val(rowData.noOfPaymentInstallment);
            $(commonName.DeductionHead).val(rowData.payHeadNameId);
            $(commonName.Remarks).val(rowData.remarks);
            if (rowData.loanID)
                if (rowData.loanID != "") {
                    $(commonName.LoanSelectInput).attr('data-selected-value', `${rowData.loanID}`).val(`${rowData.loanID}`);
                }

            $(commonName.CreateDateShow).text(rowData.createDate);
            $(commonName.ModifyDateshow).text(rowData.modifyDate);


            $(commonName.EmployeeShowName).text(rowData.fullName);
            $(commonName.EmployeeShowDesignation).text(rowData.designationName);
            $(commonName.EmployeeShowDepartment).text(rowData.departmentName);
            $(commonName.EmployeeShowJoiningDate).text(rowData.joiningDate);

            // Loan Start Date
            if (rowData.loanStartDate) {
                const fromFP = $("#dateFrom")[0]?._flatpickr;
                if (fromFP) {
                    fromFP.setDate(rowData.loanStartDate.split('T')[0], true);
                }
            }

            // Loan End Date
            if (rowData.loanEndDate) {
                const toFP = $("#toDate")[0]?._flatpickr;
                if (toFP) {
                    toFP.setDate(rowData.loanEndDate.split('T')[0], true);
                }
            }
        });

        

        function resetFields() {
            disable(
                commonName.Month,
                commonName.Year,
                commonName.DateFrom,
                commonName.ToDate,
                commonName.LoanSelectInput
            );

            $(commonName.LoanId,
                commonName.LoanDate,
                commonName.LoanType,
                commonName.LoanStartEndDate
            ).text('');

            $(commonName.LoanAmount,
                commonName.MonthDeduction,
                commonName.Month,
                commonName.Year,
                commonName.LoanInstallment,
                commonName.Remarks
            ).val('');
        }

        $(document).on('change', "#selectAllCheckbox", function () {
            var isChecked = $(this).is(':checked');
            $('.row-checkbox').prop('checked', isChecked);
        })

        $(document).on('change', '.row-checkbox', function () {
            var totalChecked = $('.row-checkbox').length;
            var checked = $('.row-checkbox:checked').length;
            $('#selectAllCheckbox').prop('checked', totalChecked === checked);
        })

        var selectedIds = [];
        $(document).on('click', commonName.DeleteBtn, function () {

            $('.row-checkbox:checked').each(function () {
                var id = $(this).data('id');
                if (id) {
                    selectedIds.push(id);
                }
            })

            if (selectedIds.length === 0) {
                showToast('warning', "Please select at least one adjustment item.");
                return;
            }
            if (confirm("Are you sure you want to delete it..!")) {
                $.ajax({
                    url: DeleteAdvancePayUrl,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(selectedIds),
                    success: function (res) {
                        if (res.isSuccess) {
                            selectedIds = [];
                            showToast('success', res.message);
                            GridAdvanceLoan();
                            $('#selectAllCheckbox').prop('checked', false);
                        } else {
                            showToast('error', res.message);
                        }


                    },
                    error: function (e) {
                        showToast('error', e.message);
                    }
                });
            }

            
        })

        var init = function () {
             $(commonName.LoanInstallment).addClass('disable').removeClass('enable');
            stHeader();
            customDateFlatpicker($(commonName.DateFrom)[0], null);
            customDateFlatpicker($(commonName.ToDate)[0], null);
            initMultiselect();
            initCompanyMultiselect();
            initLoan();
            AdjustmentAutoGanarateId();
            SelectDropdownMonth();
            SelectDropdownHeadDeduction();
            GridAdvanceLoan();           
        };
        init();
    }; advancePayTable
})(jQuery);
