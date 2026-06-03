(function ($) {
    $.hrmDefBankAndNomineeInfos = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#HrmDefBankAndNomineeInfos-form",
            formContainer: ".js-HrmDefBankAndNomineeInfos-form-container",
            gridSelector: "#HrmDefBankAndNomineeInfos-grid",
            gridContainer: ".js-HrmDefBankAndNomineeInfos-grid-container",
            editSelector: ".js-HrmDefBankAndNomineeInfos-edit",
            saveSelector: ".js-HrmDefBankAndNomineeInfos-save",
            selectAllSelector: "#HrmDefBankAndNomineeInfos-check-all",
            deleteSelector: ".js-HrmDefBankAndNomineeInfos-delete-confirm",
            deleteModal: "#HrmDefBankAndNomineeInfos-delete-modal",
            finalDeleteSelector: ".js-HrmDefBankAndNomineeInfos-delete",
            clearSelector: ".js-HrmDefBankAndNomineeInfos-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-HrmDefBankAndNomineeInfos-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-HrmDefBankAndNomineeInfos-check-availability",
            haseFile: true,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () {

            }
        }, options);



        var gridUrl = settings.baseUrl + "/Grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];

        $(window).on("load", function () {
            $("#customLoadingOverlay").fadeOut(300);
        });

        $(() => {


            loadTable();
            initialize();

            $("body").on("click", `${settings.editSelector},${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
           
                var employeeId = $(this).data("id2");
                let url = saveUrl + ($(this).data("id") ? "/" + $(this).data("id") : "");

                loadForm(url).then((data) => {
                    loadTable(employeeId);
                    console.info("Form Loaded Successfully", data);
                    $("#customLoadingOverlay").fadeOut(250);
                }).catch((error) => {
                    console.error("Failed to load form", error);
                    $("#customLoadingOverlay").fadeOut(250);
                });

                $("html, body").animate({ scrollTop: 0 }, 500);
            });

            // Image 
            function showImagePreview(input, target) {
                if (input[0].files && input[0].files[0]) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $(target).attr('src', e.target.result);
                    };
                    reader.readAsDataURL(input[0].files[0]);
                }
            }

            function clearImage(file, tag) {
                $(file).attr("src", " "); // Remove the image preview
                $(tag).val(true); // Update the hidden input field to indicate clearing
            }

            $("body").on("change", ".js-file", function () {
                var target = $(this).data("target");
                showImagePreview($(this), target);
            });

            $("body").on("click", ".js-clear-file", function () {
                var file = $(this).data("file");
                var tag = $(this).data("tag");
                clearImage(file, tag);
            });
            //

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
                            $(target).append($('<option>', {
                                value: item.code,
                                text: item.name
                            }));
                        });

                        $(target).val(lastCode);
                        console.log("Testttt", lastCode);

                    }
                });
            });

            //


            //
            // Save
            $("body").on("click", settings.saveSelector, function () {

                if (!validation()) return false;
                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }

                var data;
                if (settings.haseFile) {
                    data = new FormData($(settings.formSelector)[0]);
                    var fileInputPhoto = $('#NomineePhoto')[0].files[0];
                    var fileInputSignature = $('#NomineeSignature')[0].files[0];

                    if (fileInputPhoto) {
                        data.append('NomineePhoto', fileInputPhoto);
                    }
                    if (fileInputSignature) {
                        data.append('NomineeSignature', fileInputSignature);
                    }
                }
                else
                    data = $(settings.formSelector).serialize();

                var url = $(settings.formSelector).attr("action");
                var employeeId = $('#EmployeeId').val();
                var options = {
                    url: url,
                    method: "POST",
                    data: data,
                    success: function (response) {
                        if (response.isSuccess) {
                            loadForm(saveUrl)
                                .then((data) => {

                                    loadTable(employeeId);
                                    $(settings.lastCodeSelector).val(response.lastCode);
                                    $("#customLoadingOverlay").fadeOut(250);
                                })
                                .catch((error) => {
                                    console.log(error)
                                    $("#customLoadingOverlay").fadeOut(250);
                                })

                            toastr.success(response.message);
                        }
                        else {
                            toastr.error(response.message);
                            console.log(response);
                            $("#customLoadingOverlay").fadeOut(250);
                        }
                    }
                }
                if (settings.haseFile) {
                    options.processData = false;
                    options.contentType = false;
                }
                $.ajax(options);
            });

            $("body").on("click", settings.selectAllSelector, function () {
                $(".checkBox").prop('checked',
                    $(this).prop('checked'));
            });


            $("body").on("click", settings.deleteSelector, function (e) {
                e.preventDefault();
                $('input:checkbox.checkBox').each(function () {
                    if ($(this).prop('checked')) {
                        if (!selectedItems.includes($(this).val())) {
                            selectedItems.push($(this).val());
                        }
                    }
                });

                if (selectedItems.length > 0) {
                    $(settings.deleteModal).modal("show");
                } else {
                    toastr.info("Please select at least one item.");
                }
            });


            $("body").on('show.bs.modal', settings.deleteModal, function (event) {

                var source = $(event.relatedTarget);
                var id = source.data("ids");

                // Extract value from data-* attributes
                var title = source.data("title");
                title = "Are you sure want to delete these items?";
                var modal = $(this);
                $(modal).find('.title').html(title);

                $("body").on("click", settings.finalDeleteSelector, function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    var employeeId = $('#EmployeeId').val();
                    // Delete
                    $.ajax({
                        url: deleteUrl,
                        method: "POST",
                        contentType: "application/json",
                        data: JSON.stringify(selectedItems),
                        success: function (response) {
                            console.log(response);
                            $(modal).modal("hide");

                            if (response.success) {
                                loadForm(saveUrl)
                                    .then((data) => {
                                        selectedItems = [];
                                        loadTable(employeeId);
                                        $(settings.lastCodeSelector).val(response.lastCode);

                                    })
                                    .catch((error) => {
                                        console.log(error)
                                    })

                                toastr.success(response.message);
                            }
                            else {
                                toastr.error(response.message);
                                console.log(response);
                            }
                        }
                    });
                });

            }).on('hide.bs.modal', function () {
                $("body").off("click", settings.finalDeleteSelector);
            });

            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 0 }, 500);
            });


            $("body").on("keyup", settings.decimalSelector, function () {
                var self = $(this);
                showDecimalPlaces(self.val(), self.parent().find(".input-group-text"));
            });


            // Modal Open nad Close




            //

            $("body").on("keyup change", "#BankId, #BankBranchId, #BankAccountName, #CourseTitleCode,#BankAccountNo,#NomineeName,#BankAndNomineeId", function () {
                // Get values from the form

                var bankCode = $("#BankId").val();
                var branchBankCode = $("#BankBranchId").val();
                var acName = $("#BankAccountName").val();
                var acNO = $("#BankAccountNo").val();
                var nomineeName = $("#NomineeName").val();
                var code = $("#BankAndNomineeId").val();

                // Perform an AJAX call to check for duplicates
                $.ajax({
                    url: settings.baseUrl + "/CheckAvailability",
                    method: "POST",
                    data: {
                        bankCode: bankCode,
                        branchBankCode: branchBankCode,
                        acName: acName,
                        acName: acNO,
                        nomineeName: nomineeName,
                        code: code
                    },
                    success: function (response) {
                        if (response.isSuccess) {
                            // Show warning message if duplicate exists
                            toastr.warning(response.message);
                        }
                    },
                    error: function () {
                        toastr.error("An error occurred while checking for duplicates.");
                    }
                });
            });


            //

            $('body').on('change', "#EmployeeId", function () {

                var selectedEmployee = $(this).val();

                $.ajax({
                    url: '/HrmDefBankAndNomineeInfos/GetEmployeeNameDesDeptByCode',
                    type: 'GET',
                    data: { employeeId: selectedEmployee },
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

            //


            $('body').on('change', "#CompanyCode", function () {

                var selectedComapny = $(this).val();

                $.ajax({
                    url: '/HrmDefBankAndNomineeInfos/GetBranchByCode',
                    type: 'GET',
                    data: { ComapnyCode: selectedComapny },
                    success: function (data) {


                        if (data && data.length > 0) {
                            var braDropdown = $('#BranchCode');
                            braDropdown.empty();
                            braDropdown.append('<option value="">Select Branch</option>');
                            $.each(data, function (index, br) {
                                console.log('this is br : ', br);
                                braDropdown.append('<option value="' + br.branchCode + '">' + br.coreBranchName + '</option>');
                            });

                            braDropdown.trigger('change');
                        } else {
                            var braDropdown = $('#BranchCode');
                            braDropdown.empty();
                            braDropdown.append('<option value="">No Branches available</option>');
                        }
                    },
                    error: function () {

                    }
                });
            });


            $('body').on('change', "#CompanyCode", function () {
                var selectedComapny = $(this).val();
                $.ajax({
                    url: '/HrmDefBankAndNomineeInfos/GetEmployeeDetailsByComapnyCode',
                    type: 'GET',
                    data: { companyCode: selectedComapny },
                    success: function (data) {

                        //
                        if (data && data.length > 0) {
                            var employeeDropdown = $('#EmployeeId');
                            employeeDropdown.empty();
                            employeeDropdown.append('<option value="">---- Select Employee ----</option>');
                            $.each(data, function (index, employee) {
                                employeeDropdown.append('<option value="' + employee.employeeId + '">' + employee.employeeName + '</option>');
                            });

                            employeeDropdown.trigger('change');
                        } else {
                            var employeeDropdown = $('#EmployeeId');
                            employeeDropdown.empty();
                            employeeDropdown.append('<option value="">No employees available</option>');
                        }

                        //


                        //console.log(data);
                    },
                    error: function () {

                    }
                });
            });
            //

            $("body").on('change', "#EmployeeId", function () {

                var selectedEmployee = $(this).val();
                $.ajax({
                    url: '/HrmDefBankAndNomineeInfos/GetTableData',
                    type: 'GET',
                    data: { employeeId: selectedEmployee },
                    success: function (data) {
                        $(settings.gridContainer).html(data);
                        loadTable(selectedEmployee);
                    }, error: function () {
                        toastr.error('Failed to load data');
                    }
                });
            });

            //

            $("body").on('change', "#BankId", function () {
                var bankId = $(this).val();

                $.ajax({
                    url: '/HrmDefBankAndNomineeInfos/GetBranchBank',
                    type: 'GET',
                    data: { bankId: bankId },
                    success: function (data) {
                        var branchDropdown = $('#BankBranchId');
                        branchDropdown.empty();

                        if (data && data.length > 0) {
                            branchDropdown.append('<option value="">---- Select Branch ----</option>');

                            $.each(data, function (index, branch) {
                                branchDropdown.append('<option value="' + branch.bankBranchId + '">' + branch.bankBranchName + '</option>');
                            });

                            branchDropdown.trigger('change');
                        } else {
                            branchDropdown.append('<option value="">No branches available</option>');
                        }
                    },
                    error: function () {
                        toastr.error('Failed to load branch data');
                    }
                });
            });



            //

        });


        //

        //
        function loadTable(employeeId) {
            $.get(settings.baseUrl + "/GetTableData", { employeeId: employeeId })
                .done(html => {
                    $(settings.gridContainer).html(html);

                    if ($.fn.DataTable.isDataTable(settings.gridSelector)) {
                        $(settings.gridSelector).DataTable().destroy();
                    }

                    $(settings.gridSelector).DataTable({
                        lengthChange: true,
                        pageLength: 10,
                        lengthMenu: [
                            [10, 25, 50, -1],
                            [10, 25, 50, 'All'],
                        ],
                        order: [[1, "desc"]],
                        destroy: true, // Allow reinitialization
                        paging: true,
                        searching: true,
                        responsive: true,
                    });


                }).fail(() => toastr.error("Failed to load table data."));
        }


        function loadForm(url) {
            return new Promise((resolve, reject) => {
                var employeeId = $('#EmployeeId').val();
                $.ajax({
                    url: url,
                    type: 'GET',
                    cache: false,
                    success: function (data) {
                        $(settings.formContainer).empty();
                        $(settings.formContainer).html(data);
                        $('#EmployeeId').val(employeeId);
                        $.validator.unobtrusive.parse($(settings.formSelector));

                        initialize();
                        resolve(data)
                    },
                    error: function (error) {
                        reject(error)
                    },
                })
            })
        }

        //

        function validation() {

            var compName = $('#CompanyCode').val();
            var empName = $('#EmployeeId').val();
            var bankName = $('#BankId').val();
            var branchName = $('#BankBranchId').val();
            var nomineeName = $('#NomineeName').val();
            var relationName = $('#Relation').val();
            var acNo = $('#BankAccountNo').val();
            var acName = $('#BankAccountName').val();
            var preAddress = $('#PresentAddress').val();
            if (!compName) {
                toastr.info('Select Company');
                $('#CompanyCode').select2('open');
                return false;
            }
            if (!empName) {
                toastr.info('Select Employee');
                $('#EmployeeId').select2('open');
                return false;
            }

            if (!bankName) {
                toastr.info('Select Bank');
                $('#BankId').select2('open');
                return false;
            }
            if (!branchName) {
                toastr.info('Select Branch');
                $('#BankBranchId').select2('open');
                return false;
            }
            if (!relationName) {
                toastr.info('Select Relation');
                $('#Relation').select2('open');
                return false;
            }
            if (!nomineeName) {
                toastr.info('Enter Nominee Name');
                $('#NomineeName').trigger('focus');
                return false;
            }
            if (!acName) {
                toastr.info('Enter Bank Account Name');
                $('#BankAccountName').trigger('focus');
                return false;
            }
            if (!acNo) {
                toastr.info('Enter Bank Account No');
                $('#BankAccountNo').trigger('focus');
                return false;
            }
            if (!preAddress) {
                toastr.info('Enter Present Address');
                $('#PresentAddress').trigger('focus');
                return false;
            }
            return true;

        }
        //


        function initialize() {
            $(settings.formSelector + ' .HrmDefBankAndNomineeInfosselectpicker').select2({

                language: {
                    noResults: function () {

                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });

        }



    }

}(jQuery));