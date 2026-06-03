(function ($) {
    $.hrmEmployeeAdditionalInfos = function (options) {

        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#HrmEmployeeAdditionalInfos-form",
            formContainer: ".js-HrmEmployeeAdditionalInfos-form-container",
            gridSelector: "#HrmEmployeeAdditionalInfos-grid",
            gridContainer: ".js-HrmEmployeeAdditionalInfos-grid-container",
            editSelector: ".js-HrmEmployeeAdditionalInfos-edit",
            saveSelector: ".js-HrmEmployeeAdditionalInfos-save",
            selectAllSelector: "#HrmEmployeeAdditionalInfos-check-all",
            deleteSelector: ".js-HrmEmployeeAdditionalInfos-delete-confirm",
            deleteModal: "#HrmEmployeeAdditionalInfos-delete-modal",
            finalDeleteSelector: ".js-HrmEmployeeAdditionalInfos-delete",
            clearSelector: ".js-HrmEmployeeAdditionalInfos-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-HrmEmployeeAdditionalInfos-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-HrmEmployeeAdditionalInfos-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            load: function () {

            }
        }, options);

        var baseControllerNameUrl = "/HrmEmployeeAdditionalInfos";
        var nextCodeULR = baseControllerNameUrl + "/GenerateNextCode";
        var loadTableURL = settings.baseUrl + "/GetTableData";
        var deleteURL = baseControllerNameUrl + "/Delete";
        var getById = baseControllerNameUrl + "/Setup";
        var duplicateCheckURL = baseControllerNameUrl + "/CheckAvailability";
        var selectedItems = [];

        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });

        $(window).on("load", function () {
            $("#customLoadingOverlay").fadeOut(300);
        });

        $(() => {
            initialize();
            initializeBankBranchFilters();


            //Employee Get according to Employee Name 
            $('body').on('change', "#EmployeeId", function () {

                var selectedEmployee = $(this).val();

                $.ajax({
                    url: '/HrmEmployeeAdditionalInfos/GetEmployeeDetailsByCode',
                    type: 'GET',
                    data: { code: selectedEmployee },
                    success: function (data) {

                        $('#DesignationName').text(data.designationName);
                        $('#DepartmentName').text(data.departmentName);
                        $('#FullName').text(data.employeeName);
                        //console.log(data);
                    },
                    error: function () {

                    }
                });
            });



            //     Employee Name and Id 

            //$('body').on('change', "#CompanyCode", function () {

            //    var selectedComapny = $(this).val();

            //    $.ajax({
            //        url: '/HrmEmployeeAdditionalInfos/GetCompanyByCode',
            //        type: 'GET',
            //        data: { ComapnyCode: selectedComapny },
            //        success: function (data) {

            //            //
            //            if (data && data.length > 0) {
            //                var employeeDropdown = $('#EmployeeId');
            //                employeeDropdown.empty();
            //                employeeDropdown.append('<option value="">---- Select Employee ----</option>');
            //                $.each(data, function (index, employee) {
            //                    employeeDropdown.append('<option value="' + employee.employeeId + '">' + employee.employeeName + '</option>');
            //                });

            //                employeeDropdown.trigger('change');
            //            } else {
            //                var employeeDropdown = $('#EmployeeId');
            //                employeeDropdown.empty();
            //                employeeDropdown.append('<option value="">No employees available</option>');
            //            }

            //            //


            //            //console.log(data);
            //        },
            //        error: function () {

            //        }
            //    });
            //});

            ////

            //// BranchName and Id 

            //$('body').on('change', "#CompanyCode", function () {

            //    var selectedComapny = $(this).val();

            //    $.ajax({
            //        url: '/HrmEmployeeAdditionalInfos/GetBranchByCode',
            //        type: 'GET',
            //        data: { ComapnyCode: selectedComapny },
            //        success: function (data) {


            //            if (data && data.length > 0) {
            //                var braDropdown = $('#BranchCode');
            //                braDropdown.empty();
            //                braDropdown.append('<option value="">--Select Branch--</option>');
            //                $.each(data, function (index, br) {
            //                    console.log('this is br : ', br);
            //                    braDropdown.append('<option value="' + br.branchCode + '">' + br.coreBranchName + '</option>');
            //                });

            //                braDropdown.trigger('change');
            //            } else {
            //                var braDropdown = $('#BranchCode');
            //                braDropdown.empty();
            //                braDropdown.append('<option value="">No Branches available</option>');
            //            }




            //        },
            //        error: function () {

            //        }
            //    });
            //});

            //



            $("body").on('click', `${settings.editSelector},${settings.clearSelector}`, function () {

                var id = $(this).data('id');

                // Dynamically set the URL using the ternary operator
                let url = id ? `${getById}/${id}` : getById;
                //var id = $(this).data('id');
                $("#customLoadingOverlay").fadeOut(250);
                $.get(url, { id: id }, function (result) {

                    $(settings.formSelector).html($(result).find(settings.formSelector).html());
                    initialize();
                    $("#customLoadingOverlay").fadeOut(250);
                    initializeMultiselects('.gc-multiselect');
                    initInlineSearchSelect2('.gc-select2');
                    $(settings.formSelector).attr('action', getById);
                }).fail(function () {
                    toastr.error('Failed Update.');
                });
            });  

            //
            //
            //


            // Save button click event
            $("body").on('click', settings.saveSelector, function () {
                validation();
                $(settings.formSelector).submit();
            });

           // $("#customLoadingOverlay").fadeIn(200);

            // Form submission event
            $("body").on('submit', settings.formSelector, function (e) {
                e.preventDefault();
                var form = $(this)[0];
                var formData = new FormData(form);
                var actionUrl = $(this).attr('action');

                $.ajax({
                    type: 'POST',
                    url: actionUrl,
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (result) {


                        if (result.noSavePermission || result.noUpdatePermission || result.isDuplicate) {
                          
                            toastr.error(result.message);
                            $("#customLoadingOverlay").fadeOut(250);
                        }
                        else if (result.isSuccess) {
                            $(settings.gridContainer).html(result.html);
                            initialize();
                            $("#customLoadingOverlay").fadeOut(250);

                            toastr.success(result.message, 'Success');
                            $("#customLoadingOverlay").fadeOut(250);
                            setTimeout(function () {
                                window.location.href = result.redirectUrl;
                            }, 1000);

                        } else {
                            $(settings.formSelector).html(result);
                            initialize();
                            $("#customLoadingOverlay").fadeOut(250);

                        }
                    },
                    error: function () {
                        toastr.error('Failed Insert.');
                        $("#customLoadingOverlay").fadeOut(250);
                    }
                });
            });



            //Duplicate

            $("body").on("keyup change", "#EmployeeId,#EmployeAddInfoId", function () {

                let code = $("#EmployeAddInfoId").val();
                let employeeCode = $("#EmployeeId").val();

                $.ajax({
                    url: duplicateCheckURL,
                    method: "POST",
                    data: { code: code, employeeCode: employeeCode },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.warning(response.message);
                        }
                    }
                });

            });



            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 0 }, 500);
            });



            //


            //  popup modal 
            let loadUrl,
                target,
                reloadUrl,
                title,
                lastCode;
            // Quick add
            $("body").on("click", settings.quickAddSelector, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                loadUrl = $(this).data("url");
                target = $(this).data("target");
                reloadUrl = $(this).data("reload-url");
                title = $(this).data("title");

                $(settings.quickAddModal + " .modal-title").html(title);
                $(settings.quickAddModal + " .modal-body").empty();

                $(settings.quickAddModal + " .modal-body").load(loadUrl, function () {
                    $(settings.quickAddModal).modal("show");
                    $("#header").hide();
                    $(settings.quickAddModal + " .modal-body #header").hide()

                    $("#left_menu").hide();
                    $(settings.quickAddModal + " .modal-body #left_menu").hide()

                    $("#main-content").toggleClass("collapse-main");
                    $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")

                    $("body").removeClass("sidebar-mini");
                })
            });

            $("body").on("click", ".js-modal-dismiss", function () {
                $("body").removeClass("sidebar-mini").addClass("sidebar-mini");

                $("#header").show();
                $(settings.quickAddModal + " .modal-body #header").show()

                $("#left_menu").show();

                $(settings.quickAddModal + " .modal-body #left_menu").show()

                $("#main-content").toggleClass("collapse-main");
                $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")


                lastCode = $(settings.quickAddModal + " #lastCode").val();

                $(settings.quickAddModal + " .modal-body").empty();
                $(settings.quickAddModal).modal("hide");


                $(target).empty("");
                $(target).append($('<option>', {
                    value: '',
                    text: `--Select ${title}--`
                }));
                $.ajax({
                    url: reloadUrl,
                    method: "GET",
                    success: function (response) {
                        console.log(response);
                        $.each(response, function (i, item) {
                            $(target).append($('<option>',
                                {
                                    value: item.code,
                                    text: item.name
                                }));
                        });


                        console.log("Selected value:", lastCode);
                        $(target).val(lastCode).trigger('change');
                        //alert(lastCode);
                    }
                });
            });


            function bankBranchFilterDD(dropdownClass, branchDropdownClass, addressInputClass) {
                $("body").on("change", dropdownClass, function () {
                    var bankId = $(this).val();
                    var branchDropdown = $(branchDropdownClass);
                    var addressInput = $(addressInputClass);

                    branchDropdown.empty().append('<option value="">--Select Branch Name--</option>');
                    addressInput.val(''); // Clear the address input field

                    if (bankId) {
                        $.ajax({
                            type: 'GET',
                            url: '/BankAccountInfo/GetBranchesByBankId',
                            data: { bankId: bankId },
                            success: function (data) {
                                if (data && data.length > 0) {
                                    $.each(data, function (index, item) {
                                        branchDropdown.append('<option value="' + item.value + '">' + item.text + '</option>');
                                    });
                                } else {
                                    toastr.warning('No branches found for the selected bank.');
                                }
                            },
                            error: function () {
                                toastr.error('Failed to load branches for filtering.');
                            }
                        });
                    }
                });

                $("body").on("change", branchDropdownClass, function () {
                    var selectedBranch = $(this).find(":selected").text();
                    var bankId = $(dropdownClass).val();

                    // Fetch branches again to find the selected branch address
                    if (bankId) {
                        $.ajax({
                            type: 'GET',
                            url: '/BankAccountInfo/GetBranchesByBankId',
                            data: { bankId: bankId },
                            success: function (data) {
                                if (data && data.length > 0) {
                                    var branch = data.find(branch => branch.text === selectedBranch);
                                    var branchAddress = branch ? branch.address : '';
                                    $(addressInputClass).val(branchAddress);
                                    // alert(branch.address);
                                }
                            },
                            error: function () {
                                toastr.error('Failed to load branches for address.');
                            }
                        });
                    }
                });
            }


            function initializeBankBranchFilters() {
                // Initialize for DBBL
                bankBranchFilterDD('.BnakIdDDFilterDBBL', '.BankBranchDDFilterDBBL', '.BranchAddressInputDbbl');

                // Initialize for UCBL
                bankBranchFilterDD('.BnakIdDDFilterUCBL', '.BankBranchDDFilterUCBL', '.BranchAddressInputUcbl');

                // Initialize for SIBL
                bankBranchFilterDD('.BnakIdDDFilterSIBL', '.BankBranchDDFilterSIBL', '.BranchAddressInputSibl');
            }


            //

            // Select all checkboxes
            $("body").on('click', settings.selectAllSelector, function () {
                $('.checkBox').prop('checked', $(this).prop('checked'));
            });

            // Delete confirmation
            $("body").on('click', settings.deleteSelector, function (e) {
                e.preventDefault();
                var selectedIds = [];

                $('.checkBox:checked').each(function () {
                    selectedIds.push($(this).val());
                });

                if (selectedIds.length === 0) {
                    var deleteId = $(this).closest('tr').find('.checkBox').data('id') + "";
                    if (deleteId) {
                        selectedIds.push(deleteId);
                    } else {
                        toastr.error('Please select employee to delete.');
                        return;
                    }
                }

                $(settings.deleteModal + ' ' + settings.finalDeleteSelector).data('ids', selectedIds);
                $(settings.deleteModal).modal('show');
            });

            // Final delete action
            $("body").on('click', settings.finalDeleteSelector, function (e) {
                e.preventDefault();
                var selectedIds = $(this).data('ids');

                $.ajax({
                    type: 'POST',
                    url: deleteURL,
                    contentType: 'application/json',
                    data: JSON.stringify(selectedIds),
                    success: function (result) {
                        if (result.success) {

                            loadTableData();
                            GenerateNextCode();
                            toastr.success(result.message);
                            $('.checkBox').prop('checked', false);

                        } else {

                            toastr.error(result.message);
                        }
                        $(settings.deleteModal).modal('hide');
                    },
                    error: function (xhr) {
                        toastr.error('An error.');
                        $(settings.deleteModal).modal('hide');
                    }
                });
            });

        });
        //

        //

        function initialize() {

            $(document).ready(async function () {
                s2_InitSingle("#EmployeeId", "/GcFilters/employee", "Select Employee", null);

                s2_InitSingle(
                    "#CompanyCode",
                    "/GcFilters/company",
                    "Select Company",
                    "company",
                    ["#BranchCode", "#EmployeeId"]
                );
                s2_InitSingle("#BranchCode", "/GcFilters/branch", "Select Branch", "branch",
                    ["#EmployeeId"]
                );

                await s2_LoadNext("#CompanyCode", "/GcFilters/company");

                var $comp = $("#CompanyCode");
                var defaultCode = "001";
                var defaultName = $comp.find(`option[value="${defaultCode}"]`).text();
                await s2_SetDefault("#CompanyCode", defaultCode, defaultName, true);
            });

            //width: '100%',
            //    $('.selectpicker4').select2({
            //        language: {
            //            noResults: function () {

            //            }
            //        },
            //        escapeMarkup: function (markup) {
            //            return markup;
            //        }
            //    });
            loadTableData();
            //GenerateNextCode();
            AllDatePicker();
            // ResetForm();
            $("#customLoadingOverlay").fadeOut(250);
        }
        //
        function ResetForm() {
            GenerateNextCode();
        }

        $("body").on('click', settings.clearSelector, function () {
            ResetForm();
        });
        //
        // Validation function
        function validation() {


            var employeeName = $('#EmployeeId').val();
            var dBBlBankName = $('#SalaryBankId').val();
            var siBLBankName = $('#BankIdsibl').val();
            var uCBLBankName = $('#BankIducbl').val();

            if (!uCBLBankName) {
                toastr.info('Select Bank(UCBL)');
                $('#SalaryBankId').select2('open');
                return false;
            }
            if (!siBLBankName) {
                toastr.info('Select Bank(SIBL)');
                $('#SalaryBankId').select2('open');
                return false;
            }
            if (!dBBlBankName) {
                toastr.info('Select Bank(DBBL)');
                $('#SalaryBankId').select2('open');
                return false;
            }

            if (!employeeName) {

                toastr.info('Select Employee');
                $('#EmployeeId').select2('open');
                return false;
            }
        }

        //Load  list


        function loadTableData() {
            $.ajax({
                type: 'Get',
                url: loadTableURL,
                success: function (data) {
                    $(settings.gridContainer).html(data);
                    dataTable();
                },
                error: function () {
                    toastr.error('Failed Load Data');

                }
            });
        }

        // Data table initialization function
        function dataTable() {
            $(settings.gridSelector).DataTable({
                responsive: true,
                pageLength: 5,
                destroy: true,
                lengthMenu: [5, 10, 25, 50, 100],
            });
        }

        function GenerateNextCode() {
            $('#EmployeAddInfoId').val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result) {

                    $('#EmployeAddInfoId').val(result);


                },
                error: function () {
                    toastr.error('Failed Next Code');

                }
            });
        }

        function AllDatePicker() {

            flatpickr($("#PassportIssueDate, #PassportExpiryDate, #LicenseIssueDate, #LicenseExpireDate, #WpExpireDate, #WpEffectiveDate"), CalendarService.createConfig(
                {
                    dateFormat: "Y-m-d",
                }
            ));

        }
    }
}(jQuery));