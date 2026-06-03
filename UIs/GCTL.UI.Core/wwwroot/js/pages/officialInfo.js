(function ($) {
    $.officialInfo = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#officialInfo-form",
            gridSelector: "#officialInfo-grid",
            gridContainer: ".js-officialInfo-grid-container",
            editSelector: ".js-officialInfo-edit",
            saveSelector: ".js-officialInfo-save",
            selectAllSelector: "#officialInfo-check-all",
            deleteSelector: ".js-officialInfo-delete-confirm",
            deleteModal: "#officialInfo-delete-modal",
            finalDeleteSelector: ".js-officialInfo-delete",
            clearSelector: ".js-officialInfo-clear",
            topSelector: ".js-go",
            showNagativeFormat: false,
            haseFile: false,
            // modal for Branch
            quickAddSelector: ".js-quick-add-officialInfo",
            quickAddModalOfficialInfo: "#quickAddModal-officialInfo",
            load: function () {

            }
        }, options);

        var gridUrl = settings.baseUrl + "/GetAll";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];

        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });

        $(window).on("load", function () {
            $("#customLoadingOverlay").fadeOut(300);
        });
        $(() => {
            initialize();

            $('#EmployeeId').on('change', function () {
                var selectedEmployee = $(this).val();

                $.ajax({
                    url: '/OfficialInfo/GetEmployeeDetailsByCode',
                    type: 'GET',
                    data: { code: selectedEmployee },
                    success: function (response) {
                        if (!response) {
                            console.error('No response received.');
                            return;
                        }
                        var data = response.data || response || {};
                        console.log(response);
                            //debugger;
                        if (data) {
                            // ── Text Inputs ──────────────────────────────────────────
                            $('#AutoId').val(response.autoId || '0');
                            $('#FullName').val(data.employeeName || '');
                            $('#MobileNo').val(data.officialPhone || '');
                            $('#Email').val(data.officialEmail || '');
                            $('#PayId').val(data.payId || '');
                            $('#AttendenceId').val(data.attendenceId || '');
                            $('#DesignationName').val(data.designationName || '');
                            $('#DepartmentName').val(data.departmentName || '');
                            $('#ProbationPeriod').val(data.probationPeriod || '');

                            // ── Select2 Dropdowns (value, label) ─────────────────────
                            setSelect2('OfficialInfoCompanyCode', data.companyCode, data.companyName);
                            setSelect2('OfficialInfoBranchCode', data.branchCode, data.branchName);
                            setSelect2('DepartmentCode', data.departmentCode, data.departmentName);
                            setSelect2('DesignationCode', data.designationCode, data.designationName);
                            setSelect2('EmpTypeCode', data.employeeTypeCode, data.employeeType);
                            setSelect2('EmploymentNatureId', data.employmentNatureId, data.employmentNature);
                            setSelect2('GradeCode', data.gradeCode, data.gradeNo);
                            setSelect2('ShiftCode', data.shiftCode, data.shiftName);
                            setSelect2('ReportingTo', data.immediateSupervisorID, data.immediateSupervisor);
                            setSelect2('Hod', data.headOfDepartmentID, data.headOfDepartment);
                            setSelect2('EmployeeStatus', data.employeeStatusId, data.employeeStatus);
                            setSelect2('GradeCode', data.gradeCode, data.gradeNo);

                            var joiningFp = document.querySelector("#JoiningDate")?._flatpickr;
                            var confirmFp = document.querySelector("#ConfirmeDate")?._flatpickr;
                            var appointmentFp = document.querySelector("#AppointmentLetterDate")?._flatpickr;
                            var contractEndFp = document.querySelector("#ContractEndDate")?._flatpickr;

                            if (joiningFp) joiningFp.setDate(toFlatpickrFormat(data.joiningDate), true);
                            if (confirmFp) confirmFp.setDate(toFlatpickrFormat(data.confirmationDate), true);
                            if (appointmentFp) appointmentFp.setDate(toFlatpickrFormat(data.appointmentLetterDate));
                            if (contractEndFp) contractEndFp.setDate(toFlatpickrFormat(data.contractEndDate), true);

                            if (data.isSuccess) {
                                toastr.info(data.message);
                            }
                        }
                    },
                    error: function () {
                        console.error('Failed to fetch employee details.');
                    }
                });
            });
            function setSelect2(id, value, label) {
                if (!value) return;

                var $el = $('#' + id);
                var displayLabel = label || value; // label না থাকলে value দেখাবে

                // Option আগে থেকে থাকলে শুধু select করো
                if ($el.find('option[value="' + value + '"]').length > 0) {
                    $el.val(value).trigger('change');
                } else {
                    // নতুন option তৈরি করে label সহ add করো
                    var newOption = new Option(displayLabel, value, true, true);
                    $el.append(newOption).trigger('change');
                }
            }

            function setFlatpickr(selector, dateStr) {
                if (!dateStr) return;
                var el = document.querySelector(selector);
                if (!el) return;
                var fp = el._flatpickr;
                if (!fp) return;
                fp.setDate(toFlatpickrFormat(dateStr), true, "m-d-Y");
            }

            // Date format converter: DD/MM/YYYY → MM-DD-YYYY
            function toFlatpickrFormat(dateStr) {
                if (!dateStr) return '';
                var parts = dateStr.split('/');
                if (parts.length === 3) {
                    return parts[1] + '-' + parts[0] + '-' + parts[2];
                }
                return dateStr;
            }

            $('body').on('click', settings.saveSelector, function () {

                validation();
                validateEmployeeDD();
                validateCompanyCodeDD();
                validateDepartmentCodeDD();
                validateDesignationCodeDD();
                isExistsByCode();
                $(settings.formSelector).submit();

            });

            $('body').on('submit', settings.formSelector, function (e) {
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
                            toastr.info(result.message);
                        }
                        else if (result.isSuccess) {
                            /*$(settings.gridContainer).html(result.html);*/
                            /*initialize();*/
                            toastr.success(result.message, 'Success');
                            setTimeout(function () {
                                window.location.href = result.redirectUrl;
                            }, 1000);

                        } else {
                            $(settings.formSelector).html(result);
                            /*initialize();*/
                        }
                    },
                    error: function () {
                        toastr.error('Failed Insert.');
                    }
                });
            });

            function validation() {
                var employee = $('#EmployeeId').val();
                var company = $('#OfficialInfoCompanyCode').val();
                var department = $('#DepartmentCode').val();
                var designation = $('#DesignationCode').val();

                if (!employee) {
                    toastr.info('Select Employee');
                    $('#EmployeeId').select2('open');
                    return false;
                }

                if (!company) {
                    toastr.info('Select Company');
                    $('#OfficialInfoCompanyCode').select2('open');
                    return false;
                }

                if (!department) {
                    toastr.info('Select Department');
                    $('#DepartmentCode').select2('open');
                    return false;
                }

                if (!designation) {
                    toastr.info('Select Designation');
                    $('#DesignationCode').select2('open');
                    return false;
                }
            }

            $('#EmployeeId').on('change blur', function () {
                validateEmployeeDD();
            });

            $('#OfficialInfoCompanyCode').on('change blur', function () {
                validateCompanyCodeDD();
            });

            $('#DepartmentCode').on('change blur', function () {
                validateDepartmentCodeDD();
            });

            $('#DesignationCode').on('change blur', function () {
                validateDesignationCodeDD();
            });

            function validateEmployeeDD() {
                if ($('#EmployeeId').val().trim() == '') {
                    $('#EmployeeId').next('.select2-container').css('border', '1px solid red');
                } else {
                    $('#EmployeeId').next('.select2-container').css('border', '');
                }
            }

            function validateCompanyCodeDD() {
                if ($('#OfficialInfoCompanyCode').val().trim() == '') {
                    $('#OfficialInfoCompanyCode').next('.select2-container').css('border', '1px solid red');
                } else {
                    $('#OfficialInfoCompanyCode').next('.select2-container').css('border', '');
                }
            }

            function validateDepartmentCodeDD() {
                if ($('#DepartmentCode').val().trim() == '') {
                    $('#DepartmentCode').next('.select2-container').css('border', '1px solid red');
                } else {
                    $('#DepartmentCode').next('.select2-container').css('border', '');
                }
            }

            function validateDesignationCodeDD() {
                if ($('#DesignationCode').val().trim() == '') {
                    $('#DesignationCode').next('.select2-container').css('border', '1px solid red');
                } else {
                    $('#DesignationCode').next('.select2-container').css('border', '');
                }
            }

            $('body').on('click', settings.editSelector, function () {
                var id = $(this).data('id');

                $.get('/OfficialInfo/Setup', { id: id }, function (result) {
                    
                    console.log(result);
                    $(settings.formSelector).html($(result).find(settings.formSelector).html());
                    $(settings.formSelector).attr('action', 'OfficialInfo/Setup');
                    
                }).fail(function () {
                    toastr.error('Failed Update!');
                });
            });

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
                    var deleteId = $(this).closest('tr').find('.checkBox').data('id')+"";
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
                    url: '/OfficialInfo/Delete',
                    contentType: 'application/json',
                    data: JSON.stringify(selectedIds),
                    success: function (result) {
                        if (result.isSuccess) {

                            loadTableData();
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

            $('.js-officialInfo-clear').on('click', function () {
                clear();
            });

            function clear() {
                $('#officialInfo-form')[0].reset();
                $('#AutoId').val('0');
                $(".selectpickerOfficialInfo").val(null).trigger("change");
                $('.officialInfoDatepicker').val('');
                $('#LdateModifyDate').hide();
                $(settings.saveSelector).html('<i class="fas fa-save"></i> Save');
                $('.text-danger').text('');
                $('.selectpickerOfficialInfo').next('.select2-container').css('border', '');
                // Update the URL
                history.replaceState(null, '', '/OfficialInfo/Setup');
                // Handle Company Dropdown
                var companyOptions = $('#OfficialInfoCompanyCode option');
                if (companyOptions.length === 2) { // Includes default "---- Select Company ----"
                    $('#OfficialInfoCompanyCode').val(companyOptions.eq(1).val()).trigger("change"); // Select the only available company
                } else {
                    $('#OfficialInfoCompanyCode').val('').trigger("change"); // Clear selection
                }
            }

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

                $(settings.quickAddModalOfficialInfo + " .modal-title").html(title);
                $(settings.quickAddModalOfficialInfo + " .modal-body").empty();

                $(settings.quickAddModalOfficialInfo + " .modal-body").load(loadUrl, function () {
                    $(settings.quickAddModalOfficialInfo).modal("show");
                    $("#header").hide();
                    $(settings.quickAddModalOfficialInfo + " .modal-body #header").hide()

                    $("#left_menu").hide();
                    $(settings.quickAddModalOfficialInfo + " .modal-body #left_menu").hide()

                    $("#main-content").toggleClass("collapse-main");
                    $(settings.quickAddModalOfficialInfo + " .modal-body #main-content").toggleClass("collapse-main")

                    $("body").removeClass("sidebar-mini");
                })
            });

            $("body").on("click", ".js-modal-dismiss-officialInfo", function () {
                $("body").removeClass("sidebar-mini").addClass("sidebar-mini");

                $("#header").show();
                $(settings.quickAddModalOfficialInfo + " .modal-body #header").show()

                $("#left_menu").show();

                $(settings.quickAddModalOfficialInfo + " .modal-body #left_menu").show()

                $("#main-content").toggleClass("collapse-main");
                $(settings.quickAddModalOfficialInfo + " .modal-body #main-content").toggleClass("collapse-main")

                lastCode = $(settings.quickAddModalOfficialInfo + " #lastCode").val();

                $(settings.quickAddModalOfficialInfo + " .modal-body").empty();
                $(settings.quickAddModalOfficialInfo).modal("hide");

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
                        console.log("Test", lastCode);
                        $(target).val(lastCode);
                    }
                });
            });
            
        });

        function initialize() {
            $(document).ready(async function () {
                s2_InitSingle("#EmployeeId", "/GcFilters/employeeOff", "Select Employee", null);

                s2_InitSingle(
                    "#OfficialInfoCompanyCode",
                    "/GcFilters/company",
                    "Select Company",
                    "company",
                    ["#OfficialInfoBranchCode", "#DivisionCode", "#DepartmentCode", "#DesignationCode"]
                );
                s2_InitSingle("#OfficialInfoBranchCode", "/GcFilters/branch", "Select Branch", "branch",
                    ["#DivisionCode", "#DepartmentCode", "#DesignationCode"]
                );
                s2_InitSingle("#DivisionCode", "/GcFilters/division", "Select Division", "division",
                    ["#DepartmentCode", "#DesignationCode"]
                );
                s2_InitSingle("#DepartmentCode", "/GcFilters/department", "Select Department", "department",
                    ["#DesignationCode"]
                );
                s2_InitSingle("#DesignationCode", "/GcFilters/designation", "Select Designation", "designation");

                await s2_LoadNext("#OfficialInfoCompanyCode", "/GcFilters/company");

                var $comp = $("#OfficialInfoCompanyCode");
                var defaultCode = "001";
                var defaultName = $comp.find(`option[value="${defaultCode}"]`).text();
                await s2_SetDefault("#OfficialInfoCompanyCode", defaultCode, defaultName, true);
            });

            loadTableData();

            //$('.selectpickerOfficialInfo').select2({
            //    width: '100%',
            //    language: {
            //        noResults: function () {

            //        }
            //    },
            //    escapeMarkup: function (markup) {
            //        return markup;
            //    }
            //});

        }

        //#region flatpickr

        flatpickr($("#JoiningDate, #ConfirmeDate, #AppointmentLetterDate, #ContractEndDate"), CalendarService.createConfig(
            {
                dateFormat: "m-d-Y",
            }
        ));

        //#endregion

        function isExistsByCode() {
            $('#EmployeeId').on('change', function () {
                var code = $('#EmployeeId').val();

                $.ajax({
                    method: 'GET',
                    data: { code: code },
                    url: '/OfficialInfo/IsExistsByCode',
                    success: function (response) {
                        if (response.isSuccess) {
                            toastr.info(response.message);
                        }
                    }
                });
            });
        }

        function loadTableData() {
            $.ajax({
                type: 'GET',
                url: gridUrl,
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
                pageLength: 15,
                destroy: true,
                lengthMenu: [15, 50, 75, 100],
                order: [[0, "DESC"]],
            });
        }
    }

}(jQuery));

