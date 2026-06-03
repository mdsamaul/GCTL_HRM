
(function ($) {
    $.employeeReferenceInfos = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#EmployeeReferenceInfos-form",
            formContainer: ".js-EmployeeReferenceInfos-form-container",
            gridSelector: "#EmployeeReferenceInfos-grid",
            gridContainer: ".js-EmployeeReferenceInfos-grid-container",
            editSelector: ".js-EmployeeReferenceInfos-edit",
            saveSelector: ".js-EmployeeReferenceInfos-save",
            selectAllSelector: "#EmployeeReferenceInfos-check-all",
            deleteSelector: ".js-EmployeeReferenceInfos-delete-confirm",
            deleteModal: "#EmployeeReferenceInfos-delete-modal",
            finalDeleteSelector: ".js-EmployeeReferenceInfos-delete",
            clearSelector: ".js-EmployeeReferenceInfos-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-EmployeeReferenceInfos-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-EmployeeReferenceInfos-check-availability",
            haseFile: false,
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
            initialize();
            var employeeId = $('#EmployeeId').val();
            loadTable(employeeId);

            $("body").on("click", `${settings.editSelector},${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
                var employeeId = $(this).data("id2");

                let url = saveUrl + ($(this).data("id") ? "/" + $(this).data("id") : "");
                // var employeeId = $('#EmployeeId').val();
                loadForm(url).then((data) => {
                    loadTable(employeeId);
                    console.info("Form Loaded Successfully", data);
                    $("#customLoadingOverlay").fadeOut(250);
                }).catch((error) => {
                    console.error("Failed to load form", error);
                    $("#customLoadingOverlay").fadeOut(250);
                });

               // $("html, body").animate({ scrollTop: 0 }, 500);
            });

            //

            $("body").on("click", settings.saveSelector, function () {

                if (!validation()) return false;
                //var $valid = $(settings.formSelector).valid();
                //if (!$valid) {
                //    return false;
                //}

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
                    beforeSend: function () {
                        showLoadingIndicator();
                    },
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
                    },
                    complete: function () {
                        hideLoadingIndicator();
                    }
                }
                if (settings.haseFile) {
                    options.processData = false;
                    options.contentType = false;
                }
                $.ajax(options);
            });

            $("body").on("click", settings.deleteSelector, function (e) {
                e.preventDefault();

                selectedItems = []; // Reset selected items
                $(".checkBox:checked").each(function () {
                    selectedItems.push($(this).val());
                });

                if (selectedItems.length > 0) {
                    $(settings.deleteModal).modal("show");
                } else {
                    toastr.info("Please select at least one item to delete.");
                }
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

            //Oncahnege Duplicate
            $("body").on("keyup change", "#EmployeeId,#EmpReferenceId,#Name", function () {

                let code = $("#EmpReferenceId").val();
                let employeeCode = $("#EmployeeId").val();
                let name = $('#Name').val();

                $.ajax({
                    url: settings.baseUrl + "/CheckAvailability",
                    method: "POST",
                    data: { code: code, employeeCode: employeeCode, name: name },
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

        $('body').on('change', "#EmployeeId", function () {

            var selectedEmployee = $(this).val();

            $.ajax({
                url: '/EmployeeReferenceInfos/GetEmployeeNameDesDeptByCode',
                type: 'GET',
                data: { employeeId: selectedEmployee },
                success: function (data) {
                    $('#DesignationName').text(data.designationName || '');
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
                url: '/EmployeeReferenceInfos/GetEmployeeDetailsByComapnyCode',
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

        // BranchName and Id 

        $('body').on('change', "#CompanyCode", function () {

            var selectedComapny = $(this).val();

            $.ajax({
                url: '/EmployeeReferenceInfos/GetBranchByCode',
                type: 'GET',
                data: { ComapnyCode: selectedComapny },
                success: function (data) {
                    console.log("AJAX Response:", data); // Debugging

                    if (!data) {
                        console.log("Error: Data is null or undefined.");
                        $('#DesignationName, #DepartmentName, #FullName').text('No Data Found');
                        return;
                    }

                    $('#DesignationName').text(data.designationName || '');
                    $('#DepartmentName').text(data.departmentName || '');
                    $('#FullName').text(data.employeeName || '');
                }
            });
        });

        //
        $('body').on('change', "#CompanyCode", function () {

            var selectedComapny = $(this).val();

            $.ajax({
                url: '/EmployeeReferenceInfos/GetBranchByCode',
                type: 'GET',
                data: { ComapnyCode: selectedComapny },
                success: function (data) {

                    if (data && data.length > 0) {
                        var braDropdown = $('#BranchCode');
                        braDropdown.empty();
                        braDropdown.append('<option value="">--Select Branch--</option>');
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
        //

        $("body").on('change', "#EmployeeId", function () {

            var selectedEmployee = $(this).val();
            $.ajax({
                url: '/EmployeeReferenceInfos/GetTableData',
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

                })
                .fail(() => toastr.error("Failed to load table data."));
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
            var name = $('#ReferenceName').val();

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

            if (!name) {
                toastr.info('Enter ReferenceName');
                $('#ReferenceName').trigger('focus');
                return false;
            }

            return true;

        }

        function initialize() {
            $(settings.formSelector + ' .selectpickerrr').select2({
                width: '95%',
                language: {
                    noResults: function () {

                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });
            /*DatePicker();*/
        }

        //function DatePicker() {

        //    $('.employeeFamilyDatepicker').datepicker({
        //        dateFormat: 'dd/mm/yy',
        //        changeMonth: true,
        //        changeYear: true,
        //        yearRange: '1950:2050',
        //        //maxDate: '100Y', 
        //        //showAnim: 'fadeIn',
        //        //showButtonPanel: true, 
        //        //defaultDate: '+1w', 
        //        //currentText: 'Today', 
        //        //firstDay: 1, 
        //        //weekHeader: 'Wk', 
        //    });
        //}

    }

}(jQuery));
