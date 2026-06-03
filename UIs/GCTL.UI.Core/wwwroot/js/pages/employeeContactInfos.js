
(function ($) {
    $.employeeContactInfos = function (options) {

        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#EmployeeContactInfos-form",
            formContainer: ".js-EmployeeContactInfos-form-container",
            gridSelector: "#EmployeeContactInfos-grid",
            gridContainer: ".js-EmployeeContactInfos-grid-container",
            editSelector: ".js-EmployeeContactInfos-edit",
            saveSelector: ".js-EmployeeContactInfos-save",
            selectAllSelector: "#EmployeeContactInfos-check-all",
            deleteSelector: ".js-EmployeeContactInfos-delete-confirm",
            deleteModal: "#EmployeeContactInfos-delete-modal",
            finalDeleteSelector: ".js-EmployeeContactInfos-delete",
            clearSelector: ".js-EmployeeContactInfos-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-EmployeeContactInfos-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-EmployeeContactInfos-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            /*lastCodeSelector: '#lastCode',*/
            load: function () {

            }
        }, options);


        var baseControllerNameUrl = "/EmployeeContactInfos";
        var testSave = baseControllerNameUrl + "/Setup";
        var nextCodeULR = baseControllerNameUrl + "/GenerateNextCode";
        var loadTableURL = baseControllerNameUrl + "/GetTableData";
        var deleteURL = baseControllerNameUrl + "/Delete";
        var getById = baseControllerNameUrl + "/Setup";
        var duplicateCheckURL = baseControllerNameUrl + "/CheckAvailability";
        var selectedItems = [];
        $(window).on("load", function () {
            $("#customLoadingOverlay").fadeOut(300);
        });

        $(() => {

            initialize();

            
            // Edit leave type
         
            //

            $("body").on('click', settings.editSelector, function () {


                var id = $(this).data('id');

                $.get(getById, { id: id }, function (result) {

                    $(settings.formSelector).html($(result).find(settings.formSelector).html());

                    $(settings.formSelector).attr('action', testSave);


                }).fail(function () {
                    toastr.error('Failed Update.');
                });
            });

           

            // Save button click event
            $("body").on('click', settings.saveSelector, function () {
                validation();
                $(settings.formSelector).submit();
            });

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
                        }
                        else if (result.isSuccess) {
                            $(settings.gridContainer).html(result.html);
                            initialize();
                            $(settings.lastCodeSelector).val(result.lastCode);
                            toastr.success(result.message, 'Success');
                            setTimeout(function () {
                                window.location.href = result.redirectUrl;
                            }, 1000);

                        } else {
                            $(settings.formSelector).html(result);
                            initialize();

                        }
                    },
                    error: function () {
                        toastr.error('Failed Insert.');
                    }
                });
            });



            //Duplicate

            $("body").on("keyup change", " #EmployeeId", function () {

                let code = $("#EmployeeId").val();
                let EmployeeId = $("#EmployeeId").val();


                $.ajax({
                    url: duplicateCheckURL,
                    method: "POST",
                    data: { code: code,  EmployeeId: EmployeeId },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.error(response.message);
                        }
                    }
                });

            });

            //
            $('body').on('change', "#CompanyCode", function () {
                var selectedCompany = $(this).val();
                if (!selectedCompany) {
                    toastr.warning("Please select a valid company.");
                    return;
                }

                $.ajax({
                    url: '/EmployeeContactInfos/GetEmployeeDetailsByComapnyCode',
                    type: 'GET',
                    data: { companyCode: selectedCompany },
                    success: function (data) {
                        var employeeDropdown = $('#EmployeeId');
                        employeeDropdown.empty(); // Clear the dropdown
                        if (data && data.length > 0) {
                            employeeDropdown.append('<option value="">---- Select Employee ----</option>');
                            $.each(data, function (index, employee) {
                                employeeDropdown.append('<option value="' + employee.employeeId + '">' + employee.employeeName + '</option>');
                            });
                        } else {
                            employeeDropdown.append('<option value="">No employees available</option>');
                        }
                    },
                    error: function () {
                        toastr.error('Failed to load employees.');
                    }
                });
            });

            $('body').on('change', "#CompanyCode", function () {
                var selectedCompany = $(this).val();
                if (!selectedCompany) {
                    toastr.warning("Please select a valid company.");
                    return;
                }

                $.ajax({
                    url: '/EmployeeContactInfos/GetBranchByCode',
                    type: 'GET',
                    data: { companyCode: selectedCompany },
                    success: function (data) {
                        var branchDropdown = $('#BranchCode');
                        branchDropdown.empty();
                        if (data && data.length > 0) {
                            branchDropdown.append('<option value="">---- Select Branch ----</option>');
                            $.each(data, function (index, branch) {
                                branchDropdown.append('<option value="' + branch.branchCode + '">' + branch.coreBranchName + '</option>');
                            });
                        } else {
                            branchDropdown.append('<option value="">No branches available</option>');
                        }
                    },
                    error: function () {
                        toastr.error('Failed to load branches.');
                    }
                });
            });

            $('body').on('change', "#EmployeeId", function () {
                var selectedEmployee = $(this).val();
                if (!selectedEmployee) {
                    toastr.warning("Please select a valid employee.");
                    return;
                }

                $.ajax({
                    url: '/EmployeeContactInfos/GetEmployeeNameDesDeptByCode',
                    type: 'GET',
                    data: { employeeId: selectedEmployee },
                    success: function (data) {
                        if (data) {
                            $('#DesignationName').text(data.designationName || "N/A");
                            $('#DepartmentName').text(data.departmentName || "N/A");
                            $('#FullName').text(data.employeeName || "N/A");
                        } else {
                            toastr.error("Failed to load employee details.");
                        }
                    },
                    error: function () {
                        toastr.error('Failed to fetch employee details.');
                    }
                });
            });


            $("body").on('change', "#EmployeeId", function () {

                var selectedEmployee = $(this).val();
                $.ajax({
                    url: '/EmployeeContactInfos/GetTableData',
                    type: 'GET',
                    data: { employeeId: selectedEmployee },
                    success: function (data) {
                        $(settings.gridContainer).html(data);
                        
                    }, error: function () {
                        toastr.error('Failed to load data');
                    }
                });
            });

            //

            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 0 }, 500);
            });


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
                        // alert(lastCode);
                    }
                });
            });



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
                    toastr.error('Please select employee to delete.');
                    return;
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
                        if (result.isSuccess) {

                            loadTableData();
                            GenerateNextCode();
                            toastr.success(result.message);
                            $('.checkBox').prop('checked', false);

                        } else {

                            toastr.error(result.message, 'Error');
                        }
                        $(settings.deleteModal).modal('hide');
                    },
                    error: function (xhr) {
                        toastr.error('An error.');
                        $(settings.deleteModal).modal('hide');
                    }
                });
            });

            //

            //

            $("body").on("keyup", settings.availabilitySelector, function () {
                var self = $(this);
                let code = $(".js-code").val();
                let name = self.val();

                // check
                $.ajax({
                    url: settings.baseUrl + "/CheckAvailability",
                    method: "POST",
                    data: { code: code, name: name },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.error(response.message);
                        }
                    }
                });
            });


        });



        function initialize() {
            $('.selectpickerrrr').select2({
                with:'95%',
                language: {
                    noResults: function () {

                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });

            loadTableData();
            //GenerateNextCode();
            
            /*ResetForm();*/
        }
        //
        function ResetForm() {

            $(settings.formSelector)[0].reset();

           /* $('#Id').val('');*/
            $('#AutoId').val('');
            /*$('#EmpContactId').val('');*/
           /* $('#EmployeeId').val('');*/
            $('#ParmanentAddress').val('');
            $('#ParmanentAddressBangla').val('');
            $('#ParmanentPostOffice').val('');
            $('#ParmanentThana').val('');
            $('#ParmanentPostCode').val('');
           // $('#ParmanentDistrict').val('');
            $('#ParmanentPhone').val('');
            $('#PresentAddress').val('');
            $('#PresentAddressBangla').val('');
            $('#PresentPostOffice').val('');
            $('#PresentThana').val('');
            $('#PresentPostCode').val('');
          //  $('#PresentDistrict').val('');
            $('#PresentMobile').val('');
            $('#PresentPhone').val('');
            $('#PresentFax').val('');
            $('#PresentEmail').val('');
            $('#EmContactName1').val('');
            //$('#EmContactRelation1').val('');
            $('#EmContactAddress1').val('');
            $('#EmContactPhone1').val('');
            $('#EmContactMobile1').val('');
            $('#EmContactFax1').val('');
            $('#EmContactEmail').val('');
            $('#EmContactName2').val('');
           // $('#EmContactRelation2').val('');
            $('#EmContactAddress2').val('');
            $('#EmContactPhone2').val('');
            $('#EmContactMobile2').val('');
            $('#EmContactFax2').val('');
            $('#EmContactEmai2').val('');
            
            
            $('#CompanyCode').val(null).trigger('change');
            
            $('#BranchCode').val(null).trigger('change');

            $('#EmployeeId').val(null).trigger('change');
            

            $('#LdateModifyHide').hide();
            $(settings.saveSelector).html('<i class="fas fa-save"></i> Save');
            $('.text-danger').text('');
            $(settings.formSelector).attr('action', testSave);
            GenerateNextCode();

        }

        $("body").on('click', settings.clearSelector, function () {
            ResetForm();
        });
        //
        // Validation function
        function validation() {

            var company = $("#CompanyCode").val();
            var branch = $("#BranchCode").val();
            var employeeName = $('#EmployeeId').val();          

            if (!company) {
                toastr.info('Select Company');
                $('#CompanyCode').select2('open');
                return false;
            }
            if (!branch) {
                toastr.info('Select Branch');
                $('#BranchCode').select2('open');
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
                pageLength: 10,
                destroy: true,
                lengthMenu: [ 5, 10, 25, 50, 100]
            });
        }



        function GenerateNextCode() {
            $('#EmpContactId').val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result) {

                    $('#EmpContactId').val(result);

                },
                error: function () {
                    toastr.error('Failed Next Code');

                }
            });
        }
    }

}(jQuery));

