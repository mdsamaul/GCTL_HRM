(function ($) {
    $.employeeOfficialInfoReport = function (options) {
        var settings = $.extend({
            baseUrl: "/",
            companyIds: "#CompanyCode",
            branchIds: "#BranchCode",
            departmentIds: "#DepartmentName",
            designationIds: "#DesignationCode",
            employeeIds: "#EmployeeCode",
            EmployeeType: "#EmployeeType",
            EmploymentNature: "#EmploymentNature",
            IsExpatriate: "#IsExpatriate",
            HeadOfDepartmentCode: "#HeadOfDepartmentCode",
            Shift: "#Shift",
            EmployeeStatusCode: "#EmployeeStatusCode",
            ImmediateSupervisorCode: "#ImmediateSupervisorCode",
            TIN: "#TIN",
            NationalID: "#NationalID",
            DrivingLicense: "#DrivingLicense",
            PassportNo: "#PassportNo",
            formSelector: "#EmployeeOfficialInfoReport-form",
            load: function () { }
        }, options);

        var setupLoadingOverlay = function () {
            if ($("#customLoadingOverlay").length === 0) {
                $("body").append(`
                    <div id="customLoadingOverlay" style="
                        display: none;
                        position: fixed;
                        top: 0;
                        left: 0;
                        width: 100%;
                        height: 100%;
                        background-color: rgba(0, 0, 0, 0.5);
                        z-index: 9999;
                        justify-content: center;
                        align-items: center;">
                        <div style="
                            background-color: white;
                            padding: 20px;
                            border-radius: 5px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.3);
                            text-align: center;">
                            <div class="spinner-border text-primary" role="status">
                                <span class="sr-only">Loading...</span>
                            </div>
                            <p style="margin-top: 10px; margin-bottom: 0;">Loading data...</p>
                        </div>
                    </div>
                `);
            }
        };

        function showLoading() {
            $("#customLoadingOverlay").css("display", "flex");
        }

        function hideLoading() {
            $("#customLoadingOverlay").hide();
        }

        var GetFlatDate = function () {

            flatpickr($("#ConfirmationDateTo, #ProbationDateTo, #TerminationDateTo,#JoiningDateTo,#AppoinmentDateTo, #ConfirmationDateFrom, #ProbationDateFrom,#TerminationDateFrom,#JoiningDateFrom,#AppoinmentDateFrom"), CalendarService.createConfig(
                {
                    //defaultDate: new Date(),
                }));
        };

        var initializeMultiSelectDropdowns = function () {
            var allSelectors = [
                settings.companyIds,
                settings.branchIds,
                settings.departmentIds,
                settings.designationIds,
                settings.employeeIds,
                settings.EmployeeType,
                settings.EmploymentNature,
                settings.IsExpatriate,
                settings.HeadOfDepartmentCode,
                settings.Shift,
                settings.EmployeeStatusCode,
                settings.ImmediateSupervisorCode,
                settings.TIN,
                settings.NationalID,
                settings.DrivingLicense,
                settings.PassportNo
            ];

            // Mapping for nonSelectedText
            var placeholderMap = {};
            placeholderMap[settings.companyIds] = "~~ Select Company ~~";
            placeholderMap[settings.branchIds] = "~~ Select Branch ~~";
            placeholderMap[settings.departmentIds] = "~~ Select Department ~~";
            placeholderMap[settings.designationIds] = "~~ Select Designation ~~";
            placeholderMap[settings.employeeIds] = "~~ Select Employee ~~";
            placeholderMap[settings.EmployeeType] = "~~ Select Employee Type ~~";
            placeholderMap[settings.EmploymentNature] = "~~ Select Employment Nture ~~";
            placeholderMap[settings.IsExpatriate] = "~~ Select expatriateatus ~~";
            placeholderMap[settings.HeadOfDepartmentCode] = "~~ Select HOD ~~";
            placeholderMap[settings.Shift] = "~~ Select Shift ~~";
            placeholderMap[settings.EmployeeStatusCode] = "~~ Select Status ~~";
            placeholderMap[settings.ImmediateSupervisorCode] = "~~ Select Supervisor ~~";
            placeholderMap[settings.TIN] = "~~ Select TIN ~~";
            placeholderMap[settings.NationalID] = "~~ Select National ~~";
            placeholderMap[settings.DrivingLicense] = "~~ Select License ~~";
            placeholderMap[settings.PassportNo] = "~~ Select Passport ~~";

            allSelectors.forEach(function (selector) {
                var $element = $(selector);
                if ($element.length) {
                    if ($element.data('multiselect')) {
                        $element.multiselect('destroy');
                    }

                    $element.multiselect({
                        enableFiltering: true,
                        includeSelectAllOption: true,
                        selectAllText: 'Select All',
                        nonSelectedText: placeholderMap[selector] || '--select items--',
                        nSelectedText: 'Selected',
                        allSelectedText: 'All Selected',
                        filterPlaceholder: 'Search.......',
                        buttonWidth: '100%',     
                        maxHeight: 250,
                        enableClickableOptGroups: true,
                        dropUp: false,
                        numberDisplayed: 2,
                        enableCaseInsensitiveFiltering: true
                    });

                    $element.next('.btn-group').find('button.multiselect').off('click').on('click', function (e) {
                        e.stopPropagation();

                        var $dropdown = $(this).siblings('.multiselect-container');

                        $('.multiselect-container').not($dropdown).hide();

                        $dropdown.toggle();
                    });

                    $(document).off('click.multiselect').on('click.multiselect', function (e) {
                        if (!$(e.target).closest('.btn-group').length) {
                            $('.multiselect-container').hide();
                        }
                    });
                }
            });
        };

        $(document).on('shown.bs.dropdown', '.btn-group', function () {
            debugger
            const $group = $(this);

            if (!$group.find('.multiselect-search').length) return;

            setTimeout(function () {
                $group.find('.multiselect-search').focus();
            }, 0);
        });
       
        function ResetForm() {
            $(settings.formSelector)[0].reset();

            var allSelectors = [
                settings.companyIds,
                settings.branchIds,
                settings.departmentIds,
                settings.designationIds,
                settings.employeeIds,
                settings.EmployeeType,
                settings.EmploymentNature,
                settings.IsExpatriate,
                settings.HeadOfDepartmentCode,
                settings.Shift,
                settings.EmployeeStatusCode,
                settings.ImmediateSupervisorCode,
                settings.TIN,
                settings.NationalID,
                settings.DrivingLicense,
                settings.PassportNo
            ];

            allSelectors.forEach(function (selector) {
                var $element = $(selector);
                if ($element.length && $element.data('multiselect')) {
                    $element.multiselect('deselectAll', false);
                    $element.multiselect('updateButtonText');
                }
            });            
            loadOfficialInfo();
        }

        function toArray(value) {
            if (!value || value.length === 0) return [];
            if (Array.isArray(value)) return value;
            return [value];
        }

        function getFirstValue(value) {
            if (!value || value.length === 0) return null;
            if (Array.isArray(value)) return value[0] || null;
            return value;
        }

        function loadOfficialInfo() {
         
            var filters = {
                CompanyCodes: toArray($(settings.companyIds).val()).length ? toArray($(settings.companyIds).val()) :["001"],
                BranchCodes: toArray($(settings.branchIds).val()),
                DepartmentCodes: toArray($(settings.departmentIds).val()),
                DesignationCodes: toArray($(settings.designationIds).val()),
                EmployeeCodes: toArray($(settings.employeeIds).val()),
                EmployeeTypeCode: getFirstValue($(settings.EmployeeType).val()),
                EmploymentNatureId: getFirstValue($(settings.EmploymentNature).val()),
                IsExpatriate: getFirstValue($(settings.IsExpatriate).val()),
                ImmediateSup: getFirstValue($(settings.ImmediateSupervisorCode).val()),
                HOD: getFirstValue($(settings.HeadOfDepartmentCode).val()),
                ShiftCode: getFirstValue($(settings.Shift).val()),
                EmployeeStatus: getFirstValue($(settings.EmployeeStatusCode).val()),
                NationalId: getFirstValue($(settings.NationalID).val()),
                TinNo: getFirstValue($(settings.TIN).val()),
                PassportNo: getFirstValue($(settings.PassportNo).val()),
                DrivingLicense: getFirstValue($(settings.DrivingLicense).val())
            };

            $.ajax({
                url: "/EmployeeOfficialInfoReport/GetDropdown",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(filters),
                success: function (res) {
                    hideLoading();

                    if (!res.isSuccess) {
                        alert(res.message);
                        return;
                    }
                    $(settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds).off('change');
                    //loadTableData(res);
                    const data = res.data;
                    console.log(data);

                    if (data.companies && data.companies.length > 0 && data.companies.some(x => x.id != null && x.value != null)) {
                        var optCompany = $(settings.companyIds);

                        $.each(data.companies, function (index, company) {
                            if (company.id != null && company.value != null && company.value != '' && optCompany.find(`option[value="${company.id}"]`).length === 0) {
                                optCompany.append(`<option value="${company.id}">${company.value}</option>`);
                            }
                        });

                        var currentSelected = optCompany.val() || [];
                        if (currentSelected.indexOf('001') === -1) {
                            currentSelected.push('001');
                        }
                        optCompany.val(currentSelected);

                        optCompany.multiselect('rebuild');
                    }

                    if (data.branches && data.branches.length > 0 && data.branches.some(x => x.id != null && x.value != null)) {
                        var branches = data.branches;
                        var optbranche = $(settings.branchIds);
                        $.each(branches, function (index, branche) {
                            if (branche.id != null && branche.value != null && optbranche.find(`option[value="${branche.id}"]`).length === 0) {
                                optbranche.append(`<option value="${branche.id}">${branche.value}</option>`)
                            }
                        });
                        optbranche.multiselect('rebuild');
                    }
                    if (data.departments && data.departments.length > 0 && data.departments.some(x => x.id != null && x.value != null)) {
                        var departments = data.departments;
                        var optDepartments = $(settings.departmentIds);
                        [settings.branchIds, settings.companyIds].forEach(function (selector) {
                            $(selector).change(function () {
                                optDepartments.empty();
                            });
                        });
                        $.each(departments, function (index, department) {
                            if (department.id != null && department.value != null && optDepartments.find(`option[value="${department.id}"]`).length === 0) {
                                optDepartments.append(`<option value="${department.id}">${department.value}</option>`)
                            }
                        });
                        optDepartments.multiselect('rebuild');
                    }
                    if (data.designations && data.designations.length > 0 && data.designations.some(x => x.id != null && x.value != null)) {
                        var designations = data.designations;
                        var optDesignations = $(settings.designationIds);
                        $(settings.branchIds).change(function () {
                            optDesignations.empty();
                        });

                        $(settings.companyIds).change(function () {
                            optDesignations.empty();
                        });

                        $(settings.departmentIds).change(function () {
                            optDesignations.empty();
                        });

                        $.each(designations, function (index, designation) {
                            if (designation.id != null && designation.value != null && optDesignations.find(`option[value="${designation.id}"]`).length === 0) {
                                optDesignations.append(`<option value="${designation.id}">${designation.value}</option>`)
                            }
                        });
                        optDesignations.multiselect('rebuild');
                    }

                    if (data.employees && data.employees.length > 0 && data.employees.some(x => x.id != null && x.value != null)) {
                        var employees = data.employees;
                        var optEmployee = $(settings.employeeIds);
                        [settings.branchIds, settings.departmentIds, settings.designationIds].forEach(function (selector) {
                            $(selector).change(function () {
                                optEmployee.empty();
                            });
                        });

                        $.each(employees, function (index, employee) {
                            if (employee.id != null && employee.value != null && optEmployee.find(`option[value=${employee.id}]`).length === 0) {
                                optEmployee.append(`<option value=${employee.id}>${employee.value}</option>`)
                            }
                        });
                        optEmployee.multiselect('rebuild');
                    }
                    $(`${settings.companyIds}, ${settings.branchIds}, ${settings.departmentIds}, ${settings.designationIds}, ${settings.employeeIds}, ${settings.FromDate}, ${settings.ToDate}`).on('change', function () {
                        //console.log("Filter changed");
                        loadOfficialInfo();
                    });

                    // Employment Nature
                    var optEmpNature = $(settings.EmploymentNature);
                    [settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds].forEach(function (selector) {
                        $(selector).change(function () {
                            optEmpNature.empty().append(`<option value="">~~ Select Employment Nature ~~</option>`);
                        });
                    });
                    optEmpNature.empty().append(`<option value="">~~ Select Employment Nature ~~</option>`);
                    if (data.employmentNatures && data.employmentNatures.length > 0 && data.employmentNatures.some(x => x.id != null && x.value != null)) {
                        $.each(data.employmentNatures, function (index, item) {
                            if (item.id != null && item.value != null && optEmpNature.find(`option[value="${item.id}"]`).length === 0) {
                                optEmpNature.append(`<option value="${item.id}">${item.value}</option>`);
                            }
                        });
                    }
                    optEmpNature.multiselect('rebuild');

                    // Employee Type
                    var optEmpType = $(settings.EmployeeType);
                    [settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds].forEach(function (selector) {
                        $(selector).change(function () {
                            optEmpType.empty().append(`<option value="">~~ Select Employment Type ~~</option>`);
                        });
                    });
                    optEmpType.empty().append(`<option value="">~~ Select Employment Type ~~</option>`);
                    if (data.employeeTypes && data.employeeTypes.length > 0 && data.employeeTypes.some(x => x.id != null && x.value != null)) {
                        $.each(data.employeeTypes, function (index, item) {
                            if (item.id != null && item.value != null && optEmpType.find(`option[value="${item.id}"]`).length === 0) {
                                optEmpType.append(`<option value="${item.id}">${item.value}</option>`);
                            }
                        });
                    }
                    optEmpType.multiselect('rebuild');

                    // Immediate Supervisors
                    var optSup = $(settings.ImmediateSupervisorCode);
                    [settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds].forEach(function (selector) {
                        $(selector).change(function () {
                            optSup.empty().append(`<option value="">~~ Select Supervisor ~~</option>`);
                        });
                    });
                    optSup.empty().append(`<option value="">~~ Select Supervisor ~~</option>`);
                    if (data.immediateSupervisors && data.immediateSupervisors.length > 0 && data.immediateSupervisors.some(x => x.id != null && x.value != null)) {
                        $.each(data.immediateSupervisors, function (index, item) {
                            if (item.id != null && item.value != null && optSup.find(`option[value="${item.id}"]`).length === 0) {
                                optSup.append(`<option value="${item.id}">${item.value}</option>`);
                            }
                        });
                    }
                    optSup.multiselect('rebuild');

                    // HODs
                    var optHod = $(settings.HeadOfDepartmentCode);
                    [settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds].forEach(function (selector) {
                        $(selector).change(function () {
                            optHod.empty().append(`<option value="">~~ Select HOD ~~</option>`);
                        });
                    });
                    optHod.empty().append(`<option value="">~~ Select HOD ~~</option>`);
                    if (data.hoDs && data.hoDs.length > 0 && data.hoDs.some(x => x.id != null && x.value != null)) {
                        $.each(data.hoDs, function (index, item) {
                            if (item.id != null && item.value != null && optHod.find(`option[value="${item.id}"]`).length === 0) {
                                optHod.append(`<option value="${item.id}">${item.value}</option>`);
                            }
                        });
                    }
                    optHod.multiselect('rebuild');

                    // Shifts
                    var optShift = $(settings.Shift);
                    [settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds].forEach(function (selector) {
                        $(selector).change(function () {
                            optShift.empty().append(`<option value="">~~ Select Shift ~~</option>`);
                        });
                    });
                    optShift.empty().append(`<option value="">~~ Select Shift ~~</option>`);
                    if (data.shifts && data.shifts.length > 0 && data.shifts.some(x => x.id != null && x.value != null)) {
                        $.each(data.shifts, function (index, item) {
                            if (item.id != null && item.value != null && optShift.find(`option[value="${item.id}"]`).length === 0) {
                                optShift.append(`<option value="${item.id}">${item.value}</option>`);
                            }
                        });
                    }
                    optShift.multiselect('rebuild');

                    // Activity Status
                    var optStatus = $(settings.EmployeeStatusCode);
                    [settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds].forEach(function (selector) {
                        $(selector).change(function () {
                            optStatus.empty().append(`<option value="">~~ Select Status ~~</option>`);
                        });
                    });
                    optStatus.empty().append(`<option value="">~~ Select Status ~~</option>`);
                    if (data.activityStatuses && data.activityStatuses.length > 0 && data.activityStatuses.some(x => x.id != null && x.value != null)) {
                        $.each(data.activityStatuses, function (index, item) {
                            if (item.id != null && item.value != null && optStatus.find(`option[value="${item.id}"]`).length === 0) {
                                optStatus.append(`<option value="${item.id}">${item.value}</option>`);
                            }
                        });
                    }
                    optStatus.multiselect('rebuild');

                    // National IDs
                    var optNatId = $(settings.NationalID);
                    [settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds].forEach(function (selector) {
                        $(selector).change(function () {
                            optNatId.empty().append(`<option value="">~~ Select National ID ~~</option>`);
                        });
                    });
                    optNatId.empty().append(`<option value="">~~ Select National ID ~~</option>`);
                    if (data.nationalIds && data.nationalIds.length > 0 && data.nationalIds.some(x => x.id != null && x.value != null)) {
                        $.each(data.nationalIds, function (index, item) {
                            if (item.id != null && item.value != null && optNatId.find(`option[value="${item.id}"]`).length === 0) {
                                optNatId.append(`<option value="${item.id}">${item.value}</option>`);
                            }
                        });
                    }
                    optNatId.multiselect('rebuild');

                    // TIN Numbers
                    var optTin = $(settings.TIN);
                    [settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds].forEach(function (selector) {
                        $(selector).change(function () {
                            optTin.empty().append(`<option value="">~~ Select TIN ~~</option>`);
                        });
                    });
                    optTin.empty().append(`<option value="">~~ Select TIN ~~</option>`);
                    if (data.tinNumbers && data.tinNumbers.length > 0 && data.tinNumbers.some(x => x.id != null && x.value != null)) {
                        $.each(data.tinNumbers, function (index, item) {
                            if (item.id != null && item.value != null && optTin.find(`option[value="${item.id}"]`).length === 0) {
                                optTin.append(`<option value="${item.id}">${item.value}</option>`);
                            }
                        });
                    }
                    optTin.multiselect('rebuild');

                    // Passports
                    var optPass = $(settings.PassportNo);
                    [settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds].forEach(function (selector) {
                        $(selector).change(function () {
                            optPass.empty().append(`<option value="">~~ Select Passport ~~</option>`);
                        });
                    });
                    optPass.empty().append(`<option value="">~~ Select Passport ~~</option>`);
                    if (data.passports && data.passports.length > 0 && data.passports.some(x => x.id != null && x.value != null)) {
                        $.each(data.passports, function (index, item) {
                            if (item.id != null && item.value != null && optPass.find(`option[value="${item.id}"]`).length === 0) {
                                optPass.append(`<option value="${item.id}">${item.value}</option>`);
                            }
                        });
                    }
                    optPass.multiselect('rebuild');

                    // Driving Licenses
                    var optLic = $(settings.DrivingLicense);
                    [settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds].forEach(function (selector) {
                        $(selector).change(function () {
                            optLic.empty().append(`<option value="">~~ Select License ~~</option>`);
                        });
                    });
                    optLic.empty().append(`<option value="">~~ Select License ~~</option>`);
                    // Driving Licenses
                    var optLic = $(settings.DrivingLicense);
                    [settings.companyIds, settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds].forEach(function (selector) {
                        $(selector).change(function () {
                            optLic.empty().append(`<option value="">~~ Select License ~~</option>`);
                        });
                    });

                    optLic.empty().append(`<option value="">~~ Select License ~~</option>`);

                    if (data.drivingLicenses && data.drivingLicenses.length > 0 && data.drivingLicenses.some(x => x.id != null && x.value != null)) {
                        $.each(data.drivingLicenses, function (index, item) {
                            if (item.id != null && item.value != null && optLic.find(`option[value="${item.id}"]`).length === 0) {
                                optLic.append(`<option value="${item.id}">${item.value}</option>`);
                            }
                        });
                    }
                },

                complete: function () {
                    hideLoading();
                },
                error: function (e) {
                    hideLoading();
                    console.error("Error loading official info:", e);
                    alert("Failed to load data. Please try again.");
                }
            });



            function buildEmployeeOfficialInfoFilters() {
                function safeVal(selector) {
                    var val = $(selector).val();
                    return val && val.length > 0 ? val : null;
                }

                function safeDecimal(selector) {
                    var val = $(selector).val();
                    return val && val.length > 0 ? parseFloat(val) : null;
                }

                function safeDate(selector) {
                    var val = $(selector).val();
                    return val && val.length > 0 ? val : null; // already "YYYY-MM-DD" from flatpickr
                }

                return {
                    CompanyCodes: toArray($(settings.companyIds).val()),
                    BranchCodes: toArray($(settings.branchIds).val()),
                    DepartmentCodes: toArray($(settings.departmentIds).val()),
                    DesignationCodes: toArray($(settings.designationIds).val()),
                    EmployeeCodes: toArray($(settings.employeeIds).val()),

                    EmployeeTypeCode: getFirstValue($(settings.EmployeeType).val()) || null,
                    EmploymentNatureId: getFirstValue($(settings.EmploymentNature).val()) || null,
                    IsExpatriate: getFirstValue($(settings.IsExpatriate).val()) || null,
                    ImmediateSup: getFirstValue($(settings.ImmediateSupervisorCode).val()) || null,
                    HOD: getFirstValue($(settings.HeadOfDepartmentCode).val()) || null,
                    ShiftCode: getFirstValue($(settings.Shift).val()) || null,
                    EmployeeStatus: getFirstValue($(settings.EmployeeStatusCode).val()) || null,
                    NationalId: getFirstValue($(settings.NationalID).val()) || null,
                    TinNo: getFirstValue($(settings.TIN).val()) || null,
                    PassportNo: getFirstValue($(settings.PassportNo).val()) || null,
                    DrivingLicense: getFirstValue($(settings.DrivingLicense).val()) || null,

                    // Salary Range (decimal? safe)
                    SalaryFrom: safeDecimal("#GrossSalaryRangeFrom"),
                    SalaryTo: safeDecimal("#GrossSalaryRangeTo"),

                    // Dates (DateTime? safe)
                    AppointmentDateFrom: safeDate("#AppoinmentDateFrom"),
                    AppointmentDateTo: safeDate("#AppoinmentDateTo"),

                    JoiningDateFrom: safeDate("#JoiningDateFrom"),
                    JoiningDateTo: safeDate("#JoiningDateTo"),

                    TerminationDateFrom: safeDate("#TerminationDateFrom"),
                    TerminationDateTo: safeDate("#TerminationDateTo"),

                    ProbationDateFrom: safeDate("#ProbationDateFrom"),
                    ProbationDateTo: safeDate("#ProbationDateTo"),

                    ConfirmationDateFrom: safeDate("#ConfirmationDateFrom"),
                    ConfirmationDateTo: safeDate("#ConfirmationDateTo")
                };
            }


            function getEmployeeOfficialInfoReport() {
                var filters = buildEmployeeOfficialInfoFilters();

                $.ajax({
                    url: '/EmployeeOfficialInfoReport/GetEmployeeOfficialInfo',
                    type: 'POST',
                    beforeSend: function () {
                        showLoading();
                    },
                    contentType: 'application/json',
                    data: JSON.stringify(filters),
                    xhrFields: {
                        responseType: 'arraybuffer'
                    },
                    success: function (response) {
                        var blob = new Blob([response], {
                            type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        });

                        // Create download link
                        var link = document.createElement('a');
                        link.href = window.URL.createObjectURL(blob);
                        link.download = "Employee_Official_Info_Report_" + new Date().getTime() + ".xlsx";

                        // Trigger download
                        document.body.appendChild(link);
                        link.click();

                        // Cleanup
                        setTimeout(function () {
                            document.body.removeChild(link);
                            window.URL.revokeObjectURL(link.href);
                        }, 100);
                    },
                    error: function (xhr, status, error) {

                        alert("Error generating report: " + error);
                    },
                    complete: function () {
                        hideLoading();
                    }
                });
            }


            $("#downloadReport").click(function () {
                //debugger;
                var exportFormat = $("#exportFormatDropdown").val();
                if (!exportFormat) {
                    alert("Please select a Format.");
                    return;
                }

                if (exportFormat === "Excel") {
                    getEmployeeOfficialInfoReport();

                } else if (exportFormat === "PDF") {
                    getEmployeeOfficialInfoReportPDFWithPreview(false);
                } else {
                    getEmployeeOfficialInfoReportPDFWithPreview(false);
                }
                // ResetForm();


            });
            $("#btnPreviewPdf").click(function () {
                getEmployeeOfficialInfoReportPDFWithPreview(true);
            });





            // Updated AJAX Function
            function getEmployeeOfficialInfoReportPDFWithPreview(isPreview) {
                var filters = buildEmployeeOfficialInfoFilters();
                console.log("Filters:", filters);

                // Show loading indicator
                showLoading();

                $.ajax({
                    url: '/EmployeeOfficialInfoReport/ExportEmployeeOfficialInfoToPdf',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(filters),
                    success: function (response) {
                        hideLoading();
                        if (response && response.isSuccess) {
                            generateAndDownloadPDF(response, isPreview);
                        } else {
                            alert("Error: " + (response ? response.message : 'Unknown error'));
                        }
                    },
                    error: function (xhr, status, error) {
                        hideLoading();
                        alert("Error fetching data: " + error);
                    }
                });
            }


            async function generateAndDownloadPDF(data, isPreview = false) {
                const { jsPDF } = window.jspdf;

                const margin = 5;

                const columns = [
                    { title: "Emp ID", width: 22, align: "center" },
                    { title: "Name", width: 35, align: "left" },
                    { title: "Designation", width: 24, align: "left" },
                    { title: "Branch", width: 18, align: "center" },
                    { title: "Emp Nature", width: 20, align: "center", wrap: true },
                    { title: "Emp Type", width: 18, align: "center" },
                    { title: "Joining", width: 16, align: "center" },
                    { title: "Termination", width: 20, align: "center" },
                    { title: "Service", width: 26, align: "center" },
                    { title: "Shift", width: 20, align: "center" },
                    { title: "Supervisor", width: 24, align: "left", wrap: true },
                    { title: "HOD", width: 20, align: "left" },
                    { title: "Phone", width: 26, align: "center" },
                    { title: "Email", width: 36, align: "left", wrap: true },
                    { title: "Status", width: 13, align: "center" }
                ];

                const totalColumnWidth = columns.reduce((sum, col) => sum + col.width, 0);

                const doc = new jsPDF({
                    orientation: "landscape",
                    unit: "mm",
                    format: [totalColumnWidth + margin * 2, 210]
                });

                const pageWidth = totalColumnWidth + margin * 2;
                const pageHeight = doc.internal.pageSize.getHeight();
                let currentY = margin + 20;
                let pageNumber = 1;
                let grandTotal = 0;
                let currentDeptName = "";

                function drawHeader() {
                    doc.setFontSize(14); doc.setFont("helvetica", "bold");
                    doc.text(data.companyName ?? "Company Name", pageWidth / 2, margin, { align: "center" });

                    doc.setFontSize(9); doc.setFont("helvetica", "normal");
                    const addressLines = doc.splitTextToSize(data.companyAddress ?? "", pageWidth - 40);
                    doc.text(addressLines, pageWidth / 2, margin + 6, { align: "center" });

                    doc.setFontSize(11); doc.setFont("helvetica", "bold");
                    doc.text("Employee Official Info Report", pageWidth / 2, margin + 12, { align: "center" });

                    currentY = margin + 10;
                }
                          

                function drawTableHeader() {
                    doc.setFontSize(8); doc.setFont("helvetica", "bold");

                    let x = margin;
                    const headerHeight = 7;

                    columns.forEach(col => {
                        doc.rect(x, currentY, col.width, headerHeight);
                        doc.text(col.title, x + col.width / 2, currentY + 4.5, { align: "center" });
                        x += col.width;
                    });

                    currentY += headerHeight;
                }

                function drawDeptHeader() {
                    if (!currentDeptName) return;
                    doc.setFontSize(10); doc.setFont("helvetica", "bold");
                    doc.text(`Department: ${currentDeptName}`, margin, currentY + 5);
                    currentY += 8;
                }

                function ensureSpace(rowHeight = 6) {
                    const bottomLimit = pageHeight - (margin + 10);
                    if (currentY + rowHeight > bottomLimit) {
                        // ❌ drawFooter removed here also
                        doc.addPage([pageWidth, pageHeight]);
                        pageNumber++;

                        drawHeader();
                        drawDeptHeader();
                        drawTableHeader();
                    }
                }

                function drawRow(emp) {
                    doc.setFontSize(8); doc.setFont("helvetica", "normal");

                    let x = margin;
                    const baseLineHeight = 4;

                    const values = [
                        emp.employeeID, emp.empName, emp.designationName, emp.branchName,
                        emp.employmentNature, emp.empTypeName, emp.joiningDate,
                        emp.separationDate, emp.serviceLength, emp.shiftName,
                        emp.immediateSupervisorName, emp.headOfDepartmentName,
                        emp.mobileNo, emp.email, emp.employeeStatus
                    ];

                    const wrappedValues = values.map((v, i) => {
                        const col = columns[i];
                        const text = String(v || "");
                        return doc.splitTextToSize(text, col.width - 2);
                    });

                    const maxLines = Math.max(...wrappedValues.map(lines => lines.length));
                    const rowHeight = maxLines * baseLineHeight;

                    ensureSpace(rowHeight);

                    wrappedValues.forEach((lines, i) => {
                        const col = columns[i];
                        doc.rect(x, currentY, col.width, rowHeight);

                        lines.forEach((line, j) => {
                            const lineY = currentY + baseLineHeight * (j + 1) - 1;

                            if (col.align === "center") {
                                doc.text(line, x + col.width / 2, lineY, { align: "center" });
                            } else {
                                doc.text(line, x + 1.5, lineY, { align: "left" });
                            }
                        });

                        x += col.width;
                    });

                    currentY += rowHeight;
                }

                drawHeader();

                data.departmentGroups.forEach(dept => {
                    if (!dept.employees?.length) return;

                    currentDeptName = dept.departmentName;

                    drawDeptHeader();
                    drawTableHeader();

                    dept.employees.forEach(emp => drawRow(emp));

                    ensureSpace();
                    doc.setFontSize(9); doc.setFont("helvetica", "bold");
                    doc.text(`Total ${dept.departmentName}: ${dept.employees.length}`, margin, currentY + 5);
                    currentY += 10;

                    grandTotal += dept.employees.length;
                });

                ensureSpace();
                doc.setFontSize(10); doc.setFont("helvetica", "bold");
                doc.text(`Total Employees: ${grandTotal}`, margin, currentY + 5);

                // ---------- FINAL PASS ----------
                const totalPages = doc.getNumberOfPages();

                const now = new Date();
                const printDateTime = now.toLocaleString('en-US', {
                    day: '2-digit', month: '2-digit', year: 'numeric',
                    hour: '2-digit', minute: '2-digit', second: '2-digit',
                    hour12: true
                });

                for (let i = 1; i <= totalPages; i++) {
                    doc.setPage(i);
                    doc.setFontSize(8);
                    doc.text(`Print DateTime: ${printDateTime}`, margin, pageHeight - margin);
                    doc.text(`Page ${i} of ${totalPages}`, pageWidth - margin, pageHeight - margin, { align: "right" });
                }

                if (isPreview) {
                    const pdfBlob = doc.output('blob');
                    const pdfUrl = URL.createObjectURL(pdfBlob);
                    const previewContainer = document.getElementById("pdf-preview-container");
                    previewContainer.style.display = "block";
                    previewContainer.innerHTML = "";
                    const iframe = document.createElement("iframe");
                    iframe.style.width = "100%";
                    iframe.style.height = "100%";
                    iframe.src = pdfUrl;
                    previewContainer.appendChild(iframe);
                } else {
                    doc.save('EmployeeOfficialInfoReport.pdf');
                }
            }

        }

        $(document).ready(function () {
            GetFlatDate();
            setupLoadingOverlay();
            initializeMultiSelectDropdowns();

            $(document).on('click', "#resetButton", function () {
                ResetForm();
            });

            loadOfficialInfo();
        });
    };
}(jQuery));