(function ($) {
    $.experianceInfos = function (options) {

        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#ExperianceInfos-form",
            formContainer: ".js-ExperianceInfos-form-container",
            gridSelector: "#ExperianceInfos-grid",
            gridContainer: ".js-ExperianceInfos-grid-container",
            editSelector: ".js-ExperianceInfos-edit",
            saveSelector: ".js-ExperianceInfos-save",
            selectAllSelector: "#ExperianceInfos-check-all",
            deleteSelector: ".js-ExperianceInfos-delete-confirm",
            deleteModal: "#ExperianceInfos-delete-modal",
            finalDeleteSelector: ".js-ExperianceInfos-delete",
            clearSelector: ".js-ExperianceInfos-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-ExperianceInfos-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-ExperianceInfos-check-availability",
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
            loadTable();

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

                // $("html, body").animate({ scrollTop: 0 }, 500);
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
                //$("#customLoadingOverlay").fadeIn(200);
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
                var employeeId = $(this).data("id2");
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
            $("body").on("keyup change", "#EmployeeId,#EmpExpId,#CompanyNameId", function () {

                let code = $("#EmpExpId").val();
                let employeeCode = $("#EmployeeId").val();
                let companyNameId = $('#CompanyNameId').val();

                $.ajax({
                    url: settings.baseUrl + "/CheckAvailability",
                    method: "POST",
                    data: { code: code, employeeCode: employeeCode, CompanyNameId: companyNameId },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.warning(response.message);
                        }
                    }
                });

            });

            //


            $('body').on('change', "#EmployeeId", function () {

                var selectedEmployee = $(this).val();

                $.ajax({
                    url: '/ExperianceInfos/GetEmployeeNameDesDeptByCode',
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
                    url: '/ExperianceInfos/GetEmployeeDetailsByComapnyCode',
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
                    url: '/ExperianceInfos/GetTableData',
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
            var companyNameId = $('#CompanyNameId').val();
            var businessType = $('#BusinessType').val();
            var address = $('#Address').val();
            var department = $('#DepartmentId').val();
            var designation = $('#DesignationId').val();
            var responsibilities = $('#Responsibilities').val();
            var jobNatureId = $('#JobNatureId').val();
            var fromDate = $('#FromDate').val();
            var toDate = $('#ToDate').val();
            var salary = $('#Salary').val();
            var remarks = $('#Remarks').val();

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

            if (!companyNameId) {
                toastr.info('Enter CompanyName');
                $('#CompanyNameId').select2('open');
                return false;
            }

            if (!businessType) {
                toastr.info('Enter Business');
                $('#BusinessType').trigger('focus');
                return false;
            }

            if (!address) {
                toastr.info('Enter Address');
                $('#Address').trigger('focus');
                return false;
            }

            if (!department) {
                toastr.info('Enter Department');
                $('#DepartmentId').select2('open');
                return false;
            }

            if (!designation) {
                toastr.info('Enter Designation');
                $('#DesignationId').select2('open');
                return false;
            }

            if (!responsibilities) {
                toastr.info('Enter Responsibilities');
                $('#Responsibilities').trigger('focus');
                return false;
            }

            if (!jobNatureId) {
                toastr.info('Enter JobNature');
                $('#JobNatureId').select2('open');
                return false;
            }

            if (!fromDate) {
                toastr.info('Enter Joining Date');
                $('#FromDate').trigger('open');
                return false;
            }

            if (!toDate) {
                toastr.info('Enter Resign Date');
                $('#ToDate').trigger('open');
                return false;
            }

            if (!salary) {
                toastr.info('Enter Last Salary');
                $('#Salary').trigger('open');
                return false;
            }

            if (!remarks) {
                toastr.info('Enter Remarks');
                $('#Remarks').trigger('focus');
                return false;
            }

            return true;

        }
        //


        function initialize() {
            $(settings.formSelector + ' .Expselectpicker').select2({

                language: {
                    noResults: function () {

                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });
            //initFilterPickers();
            DatePicker();
        }

        //#region flatpickr




        //let holidayMap = {};
        //let weekendMap = {};
        //let loadedYear = null;

        //let effectivePicker, fromFilterPicker;

        //const flatpickrConfig = {
        //    dateFormat: "Y-m-d",
        //    altInput: true,
        //    altFormat: "d/m/Y",
        //    allowInput: true,

        //    onDayCreate: function (dObj, dStr, fp, dayElem) {
        //        const date =
        //            dayElem.dateObj.getFullYear() + "-" +
        //            String(dayElem.dateObj.getMonth() + 1).padStart(2, '0') + "-" +
        //            String(dayElem.dateObj.getDate()).padStart(2, '0');

        //        if (holidayMap[date]) {
        //            dayElem.classList.add("holiday-day");
        //            dayElem.title = holidayMap[date];
        //        } else if (weekendMap[date]) {
        //            dayElem.classList.add("weekend-day");
        //            dayElem.title = weekendMap[date];
        //        }
        //    },

        //    onMonthChange: (_, __, instance) => instance.redraw(),

        //    onYearChange: function (_, __, instance) {
        //        const newYear = instance.currentYear;
        //        if (loadedYear !== newYear) {
        //            loadCalendarData(newYear);
        //        }
        //    },

        //    onReady: function (_, __, instance) {
        //        instance.altInput.placeholder = "dd/mm/yyyy";
        //    }
        //};

        //$(document).ready(function () {
        //    const currentYear = new Date().getFullYear();
        //    loadCalendarData(currentYear);
        //});

        //function initFilterPickers() {

        //    // Effective Date
        //    effectivePicker = flatpickr('#FromDate', {
        //        ...flatpickrConfig
        //    });

        //    // From Date Filter
        //    fromFilterPicker = flatpickr('#ToDate', {
        //        ...flatpickrConfig,
        //        onChange: function (selectedDates) {
        //            toFilterPicker.set('minDate', selectedDates[0] || null);
        //        }
        //    });

        //    //// To Date Filter
        //    //toFilterPicker = flatpickr('#ToDateFilter', {
        //    //    ...flatpickrConfig,
        //    //    onChange: function (selectedDates) {
        //    //        fromFilterPicker.set('maxDate', selectedDates[0] || null);
        //    //    }
        //    //});
        //}

        //function loadCalendarData(year) {

        //    if (loadedYear === year && effectivePicker) {
        //        redrawPickers();
        //        return;
        //    }

        //    loadedYear = year;

        //    $.ajax({
        //        url: '/ExperianceInfos/GetCalendarData',
        //        type: 'GET',
        //        data: { year: year },
        //        success: function (data) {

        //            holidayMap = {};
        //            weekendMap = {};

        //            $.each(data, function (_, x) {
        //                if (x.type === "holiday") {
        //                    holidayMap[x.date] = x.title;
        //                }
        //                if (x.type === "weekend") {
        //                    weekendMap[x.date] = x.title;
        //                }
        //            });

        //            if (!effectivePicker) {
        //                initFilterPickers();
        //            } else {
        //                redrawPickers();
        //            }
        //        },
        //        error: function (err) {
        //            console.error("Calendar data load failed", err);
        //        }
        //    });
        //}

        //function redrawPickers() {
        //    [effectivePicker, fromFilterPicker].forEach(p => {
        //        if (p) p.redraw();
        //    });
        //}


        //#endregion


        function DatePicker() {

            const toPicker = flatpickr($("#ToDate"), CalendarService.createConfig(
                {
                    onChange: function (selectedDates, dateStr, instance) {
                        fromPicker.set('maxDate', selectedDates[0] || null);
                    }
                }
            ));

            const fromPicker = flatpickr($("#FromDate"), CalendarService.createConfig(
                {
                    onChange: function (selectedDates, dateStr, instance) {
                        toPicker.set('minDate', selectedDates[0] || null);
                    }
                }
            ));
        }
    }

}(jQuery));