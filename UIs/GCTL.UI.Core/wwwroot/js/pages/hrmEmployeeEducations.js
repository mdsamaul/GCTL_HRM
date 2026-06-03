(function ($) {
    $.hrmEmployeeEducations = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#HrmEmployeeEducations-form",
            formContainer: ".js-HrmEmployeeEducations-form-container",
            gridSelector: "#HrmEmployeeEducations-grid",
            gridContainer: ".js-HrmEmployeeEducations-grid-container",
            editSelector: ".js-HrmEmployeeEducations-edit",
            saveSelector: ".js-HrmEmployeeEducations-save",
            selectAllSelector: "#HrmEmployeeEducations-check-all",
            deleteSelector: ".js-HrmEmployeeEducations-delete-confirm",
            deleteModal: "#HrmEmployeeEducations-delete-modal",
            finalDeleteSelector: ".js-HrmEmployeeEducations-delete",
            clearSelector: ".js-HrmEmployeeEducations-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-HrmEmployeeEducations-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-HrmEmployeeEducations-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () {

            }
        }, options);



        //   var gridUrl = settings.baseUrl + "/Grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];

        $(window).on("load", function () {
            $("#customLoadingOverlay").fadeOut(300);
        });

        $(() => {

            initialize();


            $("body").on("click", `${settings.editSelector},${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
                var employeeId=  $(this).data("id2")
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

            //



            //
            // Save
            $("body").on("click", settings.saveSelector, function () {
                if (!validation()) return false;
                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }
                $("#customLoadingOverlay").fadeIn(200);
                var data;
                if (settings.haseFile)
                    data = new FormData($(settings.formSelector)[0]);
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
                                    // loadHrmEmployeeEducations(settings.baseUrl, settings.gridSelector);
                                    $(settings.lastCodeSelector).val(response.lastCode);
                                    ("#customLoadingOverlay").fadeOut(250);
                                })
                                .catch((error) => {
                                    $("#customLoadingOverlay").fadeOut(250);
                                    console.log(error)
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


                    // Delete
                    $.ajax({
                        url: deleteUrl,
                        method: "POST",
                        contentType: "application/json",
                        data: JSON.stringify(selectedItems),
                        success: function (response) {
                            console.log(response);
                            $(modal).modal("hide");
                            //
                            if (response.success) {
                                loadForm(saveUrl)
                                    .then((data) => {
                                        selectedItems = [];
                                        // loadTable(employee);

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




            //Oncahnege Duplicate
            $("body").on("keyup change", "#EmployeeId,#EmpEduCode,#DegreeCode", function () {

                let code = $("#EmpEduCode").val();
                let employeeCode = $("#EmployeeId").val();
                let degreeCode = $('#DegreeCode').val();
                // alert(`${$("#DegreeCode").val()}`);

                $.ajax({
                    url: settings.baseUrl + "/CheckAvailability",
                    method: "POST",
                    data: { code: code, employeeCode: employeeCode, degreeCode: degreeCode },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.warning(response.message);
                        }
                    }
                });

            });




        });
        //
        //

        $('body').on('change', "#CompanyCode", function () {

            var selectedComapny = $(this).val();

            $.ajax({
                url: '/HrmEmployeeEducations/GetEmployeeDetailsByComapnyCode',
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
                url: '/HrmEmployeeEducations/GetTableData',
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

        $('body').on('change', "#EmployeeId", function () {

            var selectedEmployee = $(this).val();

            $.ajax({
                url: '/HrmEmployeeEducations/GetEmployeeNameDesDeptByCode',
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




        //



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
        function loadTable(employeeId) {

            $.get(settings.baseUrl + "/GetTableData", { employeeId: employeeId })
                .done(html => {
                    $(settings.gridContainer).html(html);

                }).fail(() => toastr.error("Failed to load table data."));
        }


        function loadForm(url) {
            return new Promise((resolve, reject) => {
                $.ajax({
                    url: url,
                    type: 'GET',
                    cache: false,
                    success: function (data) {
                        $(settings.formContainer).empty();
                        $(settings.formContainer).html(data);
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
            var degreeName = $('#DegreeCode').val();
            var examtilteName = $('#ExamTitleCode').val();
            var instName = $('#InstitueCode').val();
            var boardName = $('#BoardCode').val();
            var groupName = $('#GroupCode').val();
        //    var resultDivision = $('#ResultDivision').val();
            var cgpaMarks = $('#CgpaMarks').val();
            var sacleOf = $('#ScaleOf').val();
            var yearofPassing = $('#YearofPasssing').val();
            var duration = $('#Dueration').val();
            var durationType = $('#DuratioinType').val();

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
            if (!degreeName) {
                toastr.info('Select Degree');
                $('#DegreeCode').select2('open');
                return false;
            }
            if (!examtilteName) {
                toastr.info('Select Exam Title');
                $('#ExamTitleCode').select2('open');
                return false;
            }
            if (!instName) {
                toastr.info('Select Institute');
                $('#InstitueCode').select2('open');
                return false;
            }
            if (!boardName) {
                toastr.info('Select Board');
                $('#BoardCode').select2('open');
                return false;
            }
            if (!groupName) {
                toastr.info('Select Group');
                $('#GroupCode').select2('open');
                return false;
            }
            //if (!resultDivision) {
            //    toastr.info('Enter Result Division');
            //    $('#ResultDivision').trigger('focus');
            //    return false;
            //}

            if (!cgpaMarks) {
                toastr.info('Enter CGPA Marks');
                $('#CgpaMarks').trigger('focus');
                return false;
            }
            if (!sacleOf) {
                toastr.info('Enter Scale/Out Of');
                $('#ScaleOf').trigger('focus');
                return false;
            }
            if (!yearofPassing) {
                toastr.info('Enter Year of Passsing');
                $('#YearofPasssing').trigger('focus');
                return false;
            }
            if (!duration) {
                toastr.info('Enter Duration');
                $('#Dueration').trigger('focus');
                return false;
            }

            if (!durationType) {
                toastr.info('Enter Duration Type');
                $('#DuratioinType').select2('open');
                return false;
            }
            return true;

        }
        //


        function initialize() {
            $('.selectpickerEmpEducation').select2({
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