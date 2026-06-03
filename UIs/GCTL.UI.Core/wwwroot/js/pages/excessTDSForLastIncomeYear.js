(function ($) {
    $.excessTDSForLastIncomeYear = function (options) {

        //#region Default options

        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#ExcessTDSForLastIncomeYear-form",
            formContainer: ".js-ExcessTDSForLastIncomeYear-form-container",
            gridSelectorDoun: "#ExcessTDSForLastIncomeYear-grid",
            gridSelectorUp: "#ExcessTDSForLastIncomeYear2-gridTT",
            gridContainer: ".js-ExcessTDSForLastIncomeYear-grid-container",
            editSelector: ".js-ExcessTDSForLastIncomeYear-edit",
            saveSelector: ".js-ExcessTDSForLastIncomeYear-save",
            selectAllSelectorDoun: "#ExcessTDSForLastIncomeYear-check-all",
            selectAllSelectorUp: "#ExcessTDSForLastIncomeYear-check-all",
            deleteSelector: ".js-ExcessTDSForLastIncomeYear-delete-confirm",
            deleteModal: "#ExcessTDSForLastIncomeYear-delete-modal",
            finalDeleteSelector: ".js-ExcessTDSForLastIncomeYear-delete",
            clearSelector: ".js-ExcessTDSForLastIncomeYear-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-ExcessTDSForLastIncomeYear-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-ExcessTDSForLastIncomeYear-check-availability",
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
        let excessTDSForLastIncomeYearTable = null;
        let excessTDSForLastIncomeYearUpIds = new Set();
        let excessTDSForLastIncomeYearDounIds = new Set();

        //#endregion

        $(() => {

            select2DD();
            loadTable();
            loadSNCId();
            ResetForm();
            loadFilterEmp();
            loadTableData();
            enterKeyNavigation();
            initializeMultiselects();

            //#region getAllFilter
       
            function initializeMultiselects() {
                $('#companySelect, #branchSelect, #departmentSelect, #designationSelect, #employeeTypeSelect, #employmentNatureSelect, #activityStatusSelect, #employeeSelect').multiselect({
                    enableFiltering: true,
                    includeSelectAllOption: true,
                    selectAllText: 'Select All',
                    nonSelectedText: 'Select Items',
                    nSelectedText: 'Selected',
                    allSelectedText: 'All Selected',
                    filterPlaceholder: 'Search.......',
                    buttonWidth: '100%',
                    maxHeight: 350,
                    enableClickableOptGroups: true,
                    dropUp: false,
                    numberDisplayed: 1,
                    enableCaseInsensitiveFiltering: true
                });
            }

            function getAllFilterVal() {
                const fromDateVal = $("#FromDateFilter").val();
                const toDateVal = $("#ToDateFilter").val();
                const filterData = {
                    CompanyCodes: toArray($("#companySelect").val()),
                    BranchCodes: toArray($("#branchSelect").val()),
                    DepartmentCodes: toArray($("#departmentSelect").val()),
                    DesignationCodes: toArray($("#designationSelect").val()),
                    EmployeeTypeCodes: toArray($("#employeeTypeSelect").val()),
                    EmploymentNatureId: toArray($("#employmentNatureSelect").val()),
                    ActivityStatuses: toArray($("#activityStatusSelect").val()),
                    EmployeeIDs: toArray($("#employeeSelect").val())

                    //FromDate: $("#FromDateFilter").val() ? new Date($("#FromDateFilter").val()).toISOString() : null,
                    //ToDate: $("#ToDateFilter").val() ? new Date($("#ToDateFilter").val()).toISOString() : null,
                };
                return filterData;
            }

            function toArray(value) {

                if (!value) return [];

                if (Array.isArray(value)) return value;

                if (typeof value === "string" && value.includes(',')) {

                    return value.split(',').map(v => v.trim());

                }

                return [value];

            }

            function loadFilterEmp() {
                var filterData = getAllFilterVal();
                // console.log(filterData);
                $.ajax({
                    url: `/ExcessTDSForLastIncomeYear/getResult`,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(filterData),
                    success: function (res) {

                        $("#companySelect, #branchSelect, #departmentSelect, #designationSelect, #employeeTypeSelect, #employmentNatureSelect, #activityStatusSelect, #employeeSelect")
                            .off("change");
                        loadTableData(res);
                        //debugger
                        const data = res.data;
                       //  console.log(data);
                        if (data.companies && data.companies.length > 0 && data.companies.some(x => x.code != null && x.name != null)) {
                            var Companys = data.companies;
                            //console.log(dataCompany);
                            var optCompany = $("#companySelect");
                            $.each(Companys, function (index, company) {
                                if (company.code != null && company.name != null && optCompany.find(`option[value="${company.code}"]`).length === 0) {
                                    optCompany.append(`<option value="${company.code}">${company.name}</option>`);
                                }
                            });

                            optCompany.multiselect('rebuild');
                        }

                        if (data.branches && data.branches.length > 0 && data.branches.some(b => b.code != null && b.name != null)) {
                            var dataBranch = data.branches;
                            var optBranch = $("#branchSelect");

                            $.each(dataBranch, function (index, item) {
                                if (item.code != null && item.name != null && optBranch.find(`option[value="${item.code}"]`).length === 0) {
                                    optBranch.append(`<option value="${item.code}">${item.name}</option>`);
                                }
                            });

                            optBranch.multiselect('rebuild');
                        }

                        if (data.departments && data.departments.length > 0 && data.departments.some(b => b.code != null && b.name != null)) {
                            var dataDepartments = data.departments;
                            var optDepartments = $("#departmentSelect");

                            $("#branchSelect").change(function () {
                                optDepartments.empty();
                            })

                            $.each(dataDepartments, function (index, item) {
                                if (item.code != null && item.name != null && optDepartments.find(`option[value="${item.code}"]`).length === 0) {
                                    optDepartments.append(`<option value="${item.code}">${item.name}</option>`);
                                }
                            });

                            optDepartments.multiselect('rebuild');
                        }

                        if (data.designations && data.designations.length > 0 && data.designations.some(b => b.code != null && b.name != null)) {
                            var dataDesignations = data.designations;
                            var optDesignations = $("#designationSelect");
                            $("#branchSelect").change(function () {
                                optDesignations.empty();
                            })

                            $("#departmentSelect").change(function () {
                                optDesignations.empty();
                            })

                            $.each(dataDesignations, function (index, item) {
                                if (item.code != null && item.name != null && optDesignations.find(`option[value="${item.code}"]`).length === 0) {
                                    optDesignations.append(`<option value="${item.code}">${item.name}</option>`);
                                }
                            });

                            optDesignations.multiselect('rebuild');
                        }

                        if (data.employeeTypes && data.employeeTypes.length > 0 && data.employeeTypes.some(b => b.code != null && b.name != null)) {
                            var dataEmployees = data.employeeTypes;
                            var optEmployees = $("#employeeTypeSelect");
                            $("#branchSelect").change(function () {
                                optEmployees.empty();
                            })
                            $("#departmentSelect").change(function () {
                                optEmployees.empty();
                            })
                            $("#designationSelect").change(function () {
                                optEmployees.empty();
                            })
                            $.each(dataEmployees, function (index, item) {
                                // console.log(item);
                                if (item.code != null && item.name != null && optEmployees.find(`option[value="${item.code}"]`).length === 0) {
                                    optEmployees.append(`<option value="${item.code}">${item.name}<b> ( ${item.code} )</b></option>`);
                                }
                            });
                            optEmployees.multiselect('rebuild');
                        }

                        if (data.employmentNature && data.employmentNature.length > 0 && data.employmentNature.some(l => l.code != null && l.name != null)) {
                            // console.log(data.employmentNature);
                            var optLunchBill = $("#employmentNatureSelect");
                            var currentSelection = optLunchBill.val();
                            $("#branchSelect").change(function () {
                                optLunchBill.empty();
                            })

                            $("#departmentSelect").change(function () {
                                optLunchBill.empty();
                            })

                            $("#designationSelect").change(function () {
                                optLunchBill.empty();
                            })

                            $("#employeeTypeSelect").change(function () {
                                optLunchBill.empty();
                            })

                            $.each(data.employmentNature, function (index, item) {
                                if (item.code != null && item.name != null && optLunchBill.find(`option[value="${item.code}"]`).length === 0) {
                                    optLunchBill.append(`<option value="${item.code}">${item.name}</option>`);
                                }
                            });

                            optLunchBill.multiselect('rebuild');
                        }
                        if (data.employees && data.employees.length > 0 && data.employees.some(l => l.code != null && l.name != null)) {
                            // console.log(data.employmentNature);
                            //debugger
                            var optLunchBill = $("#employeeSelect");
                            var currentSelection = optLunchBill.val();
                            $("#branchSelect").change(function () {
                                optLunchBill.empty();
                            })

                            $("#departmentSelect").change(function () {
                                optLunchBill.empty();
                            })

                            $("#designationSelect").change(function () {
                                optLunchBill.empty();
                            })

                            $("#employeeTypeSelect").change(function () {
                                optLunchBill.empty();
                            })
                            //$("#employeeSelect").change(function () {
                            //    optLunchBill.empty();
                            //})

                            $.each(data.employees, function (index, item) {
                                if (item.code != null && item.name != null && optLunchBill.find(`option[value="${item.code}"]`).length === 0) {
                                    optLunchBill.append(`<option value="${item.code}">${item.name + "("+item.code+")"}</option >`);
                                }
                            });

                            optLunchBill.multiselect('rebuild');
                        }

                        $("#activityStatusSelect").multiselect('rebuild');

                        // Mark that initial load is complete
                        isInitialLoad = false;

                        $("#companySelect, #branchSelect, #departmentSelect, #designationSelect, #employeeTypeSelect, #employmentNatureSelect, #activityStatusSelect, #FromDateFilter, #ToDateFilter,#employeeSelect")
                            .on("change", function () {
                                loadFilterEmp();
                            });
                    },
                    complete: function () {
                        //hideLoading();
                    },
                    error: function (error) {
                        //showToast("error", error.message);
                        //hideLoading();
                    }
                });
            }

            //#endregion

            //#region formatDateTime

            function formatDateTime(dateStr) {
                var date = new Date(dateStr);
                return date.toLocaleString('en-GB', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                    hour12: true
                });
            }

            //#endregion

            //#region Modal & quickAddSelector
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

            //#endregion

            //#region GetById

            $(document).on('click', '.js-ExcessTDSForLastIncomeYear-edit', function (e) {
                e.preventDefault();

                var id = $(this).data('id');
                //console.log(data);
                $.ajax({
                    url: '/ExcessTDSForLastIncomeYear/GetById',
                    type: 'GET',
                    data: { code: id },
                    success: function (data) {
                        if (data) {
                            //  console.log(data)
                            $('#AutoId').val(data.autoId);
                            $('#Etdsliyid').val(data.etdsliyid);
                            $('#EmployeeId').val(data.employeeId);

                            excessTDSForLastIncomeYearUpIds.clear();
                            excessTDSForLastIncomeYearUpIds.add(data.employeeId);
                       
                            $('#FinancialCodeNo').val(data.financialCodeNo).trigger('change');
                            
                            if ($('#EffectiveDate')[0]._flatpickr) {
                                $('#EffectiveDate')[0]._flatpickr.setDate(data.effectiveDate, true);
                            } else {
                                $('#EffectiveDate').val(data.effectiveDate);
                            }

                            $('#Tdsamount').val(data.tdsamount);
                            $('#IsfullAmountAdjust').prop('checked', data.isfullAmountAdjust === true || data.isfullAmountAdjust === 'true' || data.isfullAmountAdjust === 1);
                            $('#Remark').val(data.remark);
                            $('#SalaryMonth').val(data.salaryMonth);
                            $('#SalaryYear').val(data.salaryYear);
                            $('#ApprovedStatus').val(data.approvedStatus);

                            $('#LdateModifyHide').show();
                            $('#sectionBreak').show();

                            if (data.ldate) {
                                $('#creationDateSpan').text(formatDateTime(data.ldate));
                            } else {
                                $('#creationDateSpan').text('');
                            }

                            if (data.modifyDate) {
                                $('#modifyDateSpan').text(formatDateTime(data.modifyDate));
                            } else {
                                $('#modifyDateSpan').text('');
                            }

                            getPopulateEmployee(data.employeeId);
                        }

                    },
                    error: function () {
                        alert('Failed to load data.');
                    }
                });
            });

            //#endregion

            //#region PopulateEmployee

            function getPopulateEmployee(employeeId) {
                $.ajax({
                    url: '/ExcessTDSForLastIncomeYear/GetEmployeeById',
                    type: 'GET',
                    data: { employeeId: employeeId },
                    success: function (data) {
                        //  console.log(data);
                        if (data) {
                            loadTableData2(data)
                            excessTDSForLastIncomeYearUpIds.clear();
                            excessTDSForLastIncomeYearUpIds.add(data.data[0].code);
                            // console.log(selectedDeductionIds);
                            $('#excessTDSForLastIncomeYearTableBody input[type="checkbox"]').prop('checked', true);
                        }
                    }
                });
            }

            //#endregion

            //#region Save

            $("body").on("click", settings.saveSelector, function () {

                if (excessTDSForLastIncomeYearUpIds.size === 0) {
                    toastr.warning("Select at lest one employee");
                    return;
                }

                if (typeof validation === 'function' && !validation()) return false;
                // console.log(response);
                if (!$(settings.formSelector).valid()) return false;
                const $form = $(settings.formSelector);
                const $saveButton = $(settings.saveSelector);
                let data = settings.haseFile ? new FormData($form[0]) : $form.serialize();

                if (settings.haseFile) {
                    excessTDSForLastIncomeYearUpIds.forEach(id => data.append("SelectedEmployeeIds", id));
                } else {
                    excessTDSForLastIncomeYearUpIds.forEach(id => data += `&SelectedEmployeeIds=${encodeURIComponent(id)}`);
                }

                $saveButton.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Saving...');
                // $saveButton.prop('disabled', true);
                $.ajax({
                    url: saveUrl,
                    method: "POST",
                    data: data,
                    processData: !settings.haseFile,
                    contentType: settings.haseFile ? false : "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (response) {
                        // console.log(response);
                        if (response.isSuccess) {

                            toastr.success(response.message);
                            loadTable();
                            $(".js-Etdsliyid-code").val(response.lastCode);
                            loadTableData();
                            loadFilterEmp();
                            ResetForm();
                        }
                    },
                    error: function (xhr, status, error) {
                        toastr.error("An error occurred while saving the data.");
                        console.error("Ajax error:", status, error, xhr.responseText);
                    },
                    complete: function () {
                        $saveButton.prop('disabled', false).html('Save');
                        // $saveButton.prop('disabled', false);
                        //   ResetForm();
                    }
                });
            })

            //#endregion

            //#region selectAllSelector deleteSelector 

            $("body").on("click", settings.selectAllSelector, function () {
                $(".checkBox").prop('checked',
                    $(this).prop('checked'));
            });

            $("body").on("click", settings.deleteSelector, function (e) {
                e.preventDefault();

                if (excessTDSForLastIncomeYearDounIds.size > 0) {
                    $(settings.deleteModal).modal("show");
                } else {
                    toastr.info("Please select at least one item.");
                }

            });

            //#endregion

            //#region Delete

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
                        data: JSON.stringify([...excessTDSForLastIncomeYearDounIds]),
                        success: function (response) {

                            $(modal).modal("hide");

                            if (response.success) {
                                loadTable();
                                loadTableData();
                                loadFilterEmp();
                                ResetForm();
                                $(".js-Etdsliyid-code").val(response.lastCode)
                                toastr.success(response.message);
                            }
                            else {
                                toastr.error(response.message);
                                // console.log(response);
                            }
                        }
                    });
                });

            }).on('hide.bs.modal', function () {
                $("body").off("click", settings.finalDeleteSelector);
            });

            //#endregion

            //#region topSelector decimalSelector

            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 0 }, 500);
            });

            $("body").on("keyup", settings.decimalSelector, function () {
                var self = $(this);
                showDecimalPlaces(self.val(), self.parent().find(".input-group-text"));
            });

            //#endregion

            //#region loadTableData

            function loadTableData(res) {
                if (!res || !res.data || !res.data.employees) {
                    return;
                }
                // console.log(res.data.employees);
                var tableData = res.data.employees;
                //console.log(tableData);
                if (excessTDSForLastIncomeYearTable !== null) { excessTDSForLastIncomeYearTable.destroy(); }

                var tableBody = $("#excessTDSForLastIncomeYearTableBody");
                tableBody.empty();

                $.each(tableData, function (index, employee) {
                    var joiningDate = employee.joiningDate ? new Date(employee.joiningDate).toLocaleDateString('en-GB') : '';

                    var row = $('<tr>');
                    row.append(`<td class="text-center no-sort" style="width:60px !important;"><input type="checkbox" data-id=` + employee.employeeId + ` /></td>`);
                    row.append('<td class="text-center">' + (employee.employeeId || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.name || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.designationName || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.departmentName || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.branchName || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.companyName || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.employeeType || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.employmentNature || '') + '</td>');
                    row.append('<td class="text-center">' + joiningDate + '</td>');

                    tableBody.append(row);
                });

                initializeDataTable();
            }

            //#endregion

            //#region initializeDataTable

            function initializeDataTable() {
                if ($.fn.DataTable.isDataTable('#ExcessTDSForLastIncomeYear2-gridTT')) {
                    $('#ExcessTDSForLastIncomeYear2-gridTT').DataTable().clear().destroy();
                }

                excessTDSForLastIncomeYearTable = $('#ExcessTDSForLastIncomeYear2-gridTT').DataTable({
                    paging: true,
                    pageLength: 10,
                    lengthMenu: [[10, 25, 50, 100, 1000, -1], [10, 25, 50, 100, 1000, "All"]],
                    lengthChange: true,
                    searching: true,
                    ordering: true,
                    info: true,
                    autoWidth: false,
                    responsive: true,
                    fixedHeader: false,
                    scrollX: true,
                    scrollCollapse: true,
                    language: {
                        search: "🔍 Search:",
                        lengthMenu: "Show _MENU_ entries",
                        searchPlaceholder: "Search here.......",
                        info: "Showing _START_ to _END_ of _TOTAL_ entries",
                        paginate: {
                            first: "First",
                            previous: "Prev",
                            next: "Next",
                            last: "Last"
                        },
                        emptyTable: "No data available"
                    },
                    columnDefs: [
                        { targets: 'no-sort', orderable: false }
                    ],
                    initComplete: function () {
                        $('.dataTables_filter input').css({
                            'width': '250px',
                            'padding': '6px 12px',
                            'border': '1px solid #ddd',
                            'border-radius': '4px'
                        });
                    },
                    drawCallback: function () {
                        $('#excessTDSForLastIncomeYearTableBody input[type="checkbox"]').each(function () {
                            const id = $(this).data('id');
                            $(this).prop('checked', excessTDSForLastIncomeYearUpIds.has(id));
                        });

                        const total = $('#excessTDSForLastIncomeYearTableBody input[type="checkbox"]').length;
                        const checked = $('#excessTDSForLastIncomeYearTableBody input[type="checkbox"]:checked').length;
                        $("#EmpExcessTDSForLastIncomeYear-check-all").prop('checked', total > 0 && total === checked);


                    }
                });
            }

            //#endregion

            //#region loadTableData2

            $(document).ready(function () {
                loadTableData2({ data: { employees: [] } });
            });

            function loadTableData2(res) {

                var tableData = res.data;

                if (excessTDSForLastIncomeYearTable !== null) {
                    excessTDSForLastIncomeYearTable.destroy();
                }

                var tableBody = $("#excessTDSForLastIncomeYearTableBody");
                tableBody.empty();
                $.each(tableData, function (index, employee) {
                    //  console.log(employee);
                    var joiningDate = employee.joiningDate ? new Date(employee.joiningDate).toLocaleDateString('en-GB') : '';

                    var row = $('<tr>');
                    row.append('<td class="text-center" style="width:60px !important;"><input type="checkbox" /></td>');
                    row.append('<td class="text-center">' + (employee.code || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.name || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.designation || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.department || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.branch || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.company || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.employeeType || '') + '</td>');
                    row.append('<td class="text-center">' + (employee.employmentNature || '') + '</td>');
                    row.append('<td class="text-center">' + joiningDate + '</td>');

                    tableBody.append(row);
                });

                initializeDataTable();
            }

            //#endregion

            //#region flatpickr

            $(document).ready(function () {
                const flatpickrConfig = {
                    dateFormat: "Y-m-d",
                    altInput: true,
                    altFormat: "d/m/Y",
                    allowInput: true,
                    onReady: function (selectedDates, dateStr, instance) {
                        instance.altInput.placeholder = "dd/mm/yyyy";
                    }
                };

                flatpickr('.flatpickr, #FromDateFilter, #ToDateFilter', flatpickrConfig);

                $(document).on('change', '#FromDateFilter', function () {
                    const minDate = $(this).val();
                    const toPicker = $('#ToDateFilter')[0]._flatpickr;

                    if (toPicker) {
                        toPicker.destroy();
                    }

                    flatpickr('#ToDateFilter', {
                        ...flatpickrConfig,
                        minDate: minDate || null
                    });
                });
            });

            //#endregion

            //#region ready Checkbox

            $(document).ready(function () {

                // 1st Check-All

                $("#EmpExcessTDSForLastIncomeYear-check-all").on('change', function () {
                    const isChecked = $(this).is(':checked');
                    $('#excessTDSForLastIncomeYearTableBody input[type="checkbox"]').each(function () {
                        $(this).prop('checked', isChecked);
                        const id = $(this).data('id');
                        if (isChecked) {
                            excessTDSForLastIncomeYearUpIds.add(id);
                        } else {
                            excessTDSForLastIncomeYearUpIds.delete(id);
                        }
                    });
                });

                $(document).on('change', '#excessTDSForLastIncomeYearTableBody input[type="checkbox"]', function () {

                    const id = $(this).data('id');

                    if ($(this).is(':checked')) {
                        excessTDSForLastIncomeYearUpIds.add(id);
                    } else {
                        excessTDSForLastIncomeYearUpIds.delete(id);
                    }

                    const total = $('#excessTDSForLastIncomeYearTableBody input[type="checkbox"]').length;
                    const checked = $('#excessTDSForLastIncomeYearTableBody input[type="checkbox"]:checked').length;
                    $("#EmpExcessTDSForLastIncomeYear-check-all").prop('checked', total > 0 && total === checked);
                });

                //2nd Check-All

                $(document).on('change', '#ExcessTDSForLastIncomeYear-check-all', function () {
                    const isChecked = $(this).is(':checked');
                    $('#ExcessTDSForLastIncomeYearGridBody input[type="checkbox"]').each(function () {
                        $(this).prop('checked', isChecked);
                        const id = $(this).data('id');
                        if (isChecked) {
                            excessTDSForLastIncomeYearDounIds.add(id);
                        } else {
                            excessTDSForLastIncomeYearDounIds.delete(id);
                        }
                    });
                });

                $(document).on('change', '#ExcessTDSForLastIncomeYearGridBody input[type="checkbox"]', function () {

                    const id = $(this).data('id');

                    if ($(this).is(':checked')) {
                        excessTDSForLastIncomeYearDounIds.add(id);
                    } else {
                        excessTDSForLastIncomeYearDounIds.delete(id);
                    }

                    const total = $('#ExcessTDSForLastIncomeYearGridBody input[type="checkbox"]').length;
                    const checked = $('#ExcessTDSForLastIncomeYearGridBody input[type="checkbox"]:checked').length;
                    $("#ExcessTDSForLastIncomeYear-check-all").prop('checked', total > 0 && total === checked);
                });


                // Clear all checkboxes and reset filters

                $(".js-ExcessTDSForLastIncomeYear-clear").click(function () {
                    $("#ExcessTDSForLastIncomeYear-check-all").prop("checked", false);
                    $("#ExcessTDSForLastIncomeYear-grid input[type='checkbox']").prop("checked", false);
                    $("#EmpExcessTDSForLastIncomeYear-check-all").prop("checked", false);
                    $('#excessTDSForLastIncomeYearTableBody input[type="checkbox"]').prop('checked', false);
                    excessTDSForLastIncomeYearUpIds.clear();
                    excessTDSForLastIncomeYearDounIds.clear();
                    loadTable();
                    loadTableData();
                    loadFilterEmp();
                });
            });

            //#endregion

        });

        //#region loadTable

        function loadTable() {
            $.get(settings.baseUrl + "/GetTableDataSalary")
                .done(html => {
                    $(".js-ExcessTDSForLastIncomeYear-grid-container").html(html);
                    if ($.fn.DataTable.isDataTable("#ExcessTDSForLastIncomeYear-grid")) {
                        $("#ExcessTDSForLastIncomeYear-grid").DataTable().destroy();
                    }
                    setTimeout(() => {
                        $("#ExcessTDSForLastIncomeYear-grid").DataTable({
                            lengthChange: true,
                            pageLength: 5,
                            lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, 'All']],
                            order: [[1, "desc"]],
                            destroy: true,
                            paging: true,
                            searching: true,
                            responsive: true,
                            autoWidth: false,
                            columnDefs: [{ targets: 0, orderable: false }],
                            drawCallback: function () {
                                $('#ExcessTDSForLastIncomeYearGridBody input[type="checkbox"]').each(function () {
                                    const id = $(this).data('id');
                                    $(this).prop('checked', excessTDSForLastIncomeYearDounIds.has(id));
                                });

                                const total = $('#ExcessTDSForLastIncomeYearGridBody input[type="checkbox"]').length;
                                const checked = $('#ExcessTDSForLastIncomeYearGridBody input[type="checkbox"]:checked').length;
                                $("#ExcessTDSForLastIncomeYear-check-all").prop('checked', total > 0 && total === checked);

                            }
                        });
                    }, 0);
                })
                .fail((xhr, status, error) => {
                    console.error("Error loading table:", status, error, xhr.responseText);
                    toastr.error("Failed to load table data.");
                    loadForm(url);
                });
        }

        //#endregion

        //#region loadForm

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
                        initializeMultiselects()
                        resolve(data)
                    },
                    error: function (error) {
                        reject(error)
                    },
                })
            })
        }

        //#endregion

        //#region GenerateNewId
        function loadSNCId() {
            $.ajax({
                url: "/ExcessTDSForLastIncomeYear/GenerateNewId",
                type: "GET",
                dataType: "json",
                success: function (data) {

                    if (data) {
                        $('#Etdsliyid').val(data);
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error fetching Etdsliyid:", error);
                }
            })
        }

        //#endregion

        //#region EnterKeyNavigation
        function enterKeyNavigation() {
            const $form = $('#ExcessTDSForLastIncomeYear-form');
            if (!$form.length) return;

            $form.on('keydown', function (e) {
                // Allow Shift+Enter in textarea for new lines
                if (e.key === 'Enter' && !(e.shiftKey && $(e.target).is('textarea'))) {
                    e.preventDefault();

                    const $focusable = $form.find('input:visible, select:visible, textarea:visible, button:visible').not(':disabled');
                    const currentIndex = $focusable.index(e.target.closest('input, select, textarea, button'));
                    const $next = $focusable.eq((currentIndex + 1) % $focusable.length);

                    if ($next.data('select2')) {
                        $next.select2('open');
                    } else {
                        $next.focus();
                    }
                }
            });

            // Handle Select2 close event
            $form.on('select2:close', 'select', function () {
                const $focusable = $form.find('input:visible, select:visible, textarea:visible, button:visible').not(':disabled');
                const currentIndex = $focusable.index(this);
                const $next = $focusable.eq((currentIndex + 1) % $focusable.length);

                setTimeout(() => {
                    if ($next.data('select2')) {
                        $next.select2('open');
                    } else {
                        $next.focus();
                    }
                }, 50);
            });
        }

        // #endregion

        //#region ResetForm

        $(document).ready(function () {
            $('.js-ExcessTDSForLastIncomeYear-clear').on('click', function () {
                ResetForm();          
            });
            $('#LdateModifyHide').hide();
            $('#sectionBreak').hide();        
        });

        function ResetForm() {

            // Clear input fields
            $('#EmployeeId').val('');
            $('[name="Tdsamount"]').val('');
            $('#SalaryMonth').val('');
            $('[name="SalaryYear"]').val('');
            $('[name="Remark"]').val('');
            $('[name="IsfullAmountAdjust"]').prop('checked', false);
            $('[name="ApprovedStatus"]').val('');
            if ($('#EffectiveDate')[0] && $('#EffectiveDate')[0]._flatpickr) {
                $('#EffectiveDate')[0]._flatpickr.clear();
            } else {
                $('#EffectiveDate').val('');
            }

            loadSNCId();
            excessTDSForLastIncomeYearUpIds.clear();
            excessTDSForLastIncomeYearDounIds.clear();

            $('#LdateModifyHide').hide();
            $('#sectionBreak').hide();

            // Uncheck checkboxes
            $("#EmpExcessTDSForLastIncomeYear-check-all").prop("checked", false);
            $('#excessTDSForLastIncomeYearTableBody input[type="checkbox"]').prop('checked', false);
            $('#AutoId').val('0');

            $("#ExcessTDSForLastIncomeYear-check-all").prop("checked", false);
            $('#ExcessTDSForLastIncomeYear-grid input[type="checkbox"]').prop('checked', false);

            var defaultFinancialYear = $('#DefaultFinancialCodeNo').val();
            $('#FinancialCodeNo').val(defaultFinancialYear).trigger('change');
           
        };

        //#endregion

        //#region select2
        function select2DD() {

            var preSelectedValue = $('#FinancialCodeNo').val();

            // Initialize Select2
            $('#FinancialCodeNo').select2({
                placeholder: "Select Financial Year",
                allowClear: true,
                width: '95%'
            });

             setTimeout(function() {
                 if (preSelectedValue) {
                     $('#FinancialCodeNo').val(preSelectedValue).trigger('change');
                 }
             }, 100);

            //$('#FinancialCodeNo').select2({
            //    width: '95%',
            //    language: {
            //        noResults: function () { return "No results found"; }
            //    },
            //    escapeMarkup: function (markup) { return markup; }
            //});
        }

        //#endregion

        //#region validation
        function validation() {

            var com = $('#FinancialCodeNo').val();
            var jon = $('#EffectiveDate').val().trim();
            var add = $('#Tdsamount').val();

            if (!com) {
                toastr.info('Select Financial Year');
                $('#FinancialCodeNo').select2('open')
                return false;
            }

            if (!jon) {
                toastr.info('Enter Effective Date');
                $('#EffectiveDate').focus();

                // Open flatpickr calendar if attached
                if ($('#EffectiveDate')[0]._flatpickr) {
                    $('#EffectiveDate')[0]._flatpickr.open();
                }
                return false;
            }

            if (!add) {
                toastr.info('Enter TDS Amount');
                $('#Tdsamount').trigger('focus');
                return false;
            }

            return true;
        }

        //#endregion

    }

}(jQuery));