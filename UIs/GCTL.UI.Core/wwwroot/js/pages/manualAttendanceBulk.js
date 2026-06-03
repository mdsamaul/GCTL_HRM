(function ($) {
    $.manualAttendanceBulk = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#manualAttendanceBulk-form",
            gridSelector: ".manualAttendanceBulkTable",
            gridContainer: ".js-manualAttendanceBulk-grid-container",
            editSelector: ".js-manualAttendanceBulk-edit",
            saveSelector: ".js-manualAttendanceBulk-save",
            selectAllSelector: "#manualAttendanceBulk-check-all",
            deleteSelector: ".js-manualAttendanceBulk-delete-confirm",
            deleteModal: "#manualAttendanceBulk-delete-modal",
            finalDeleteSelector: ".js-manualAttendanceBulk-delete",
            clearSelector: ".js-manualAttendanceBulk-clear",
            topSelector: ".js-go",
            showNagativeFormat: false,
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            load: function () {

            }
        }, options);


        var gridUrl = settings.baseUrl + "/GetAll";
        var saveUrl = settings.baseUrl + "/Setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var companyCode = $('#CompanyCode').val();
        var selectedItems = [];
        $(() => {
            initialize();

            $('body').on('submit', settings.formSelector, function (e) {
                e.preventDefault();

                var selectedEmployeeIds = [];
                $('.EmployeeListCheckBox:checked').each(function () {
                    selectedEmployeeIds.push($(this).data('id'));
                });


                // Get time values from hidden inputs
                let entryTime = $('#EntryTime').val();
                let exitTime = $('#ExitTime').val();
                const dateFrom = $('#DateFrom').val();
                const dateTo = $('#DateTo').val();

                // Convert dd/mm/yyyy to yyyy-mm-dd
                function convertToISODate(dateStr) {
                    if (!dateStr) return null;
                    const parts = dateStr.split('/');
                    if (parts.length === 3) {
                        return `${parts[2]}-${parts[1]}-${parts[0]}`; // yyyy-mm-dd
                    }
                    return null;
                }

                // Convert time to 24-hour format if it's in 12-hour format
                function convertTo24Hour(timeStr) {
                    if (!timeStr) return null;

                    // If already in HH:mm:ss format (24-hour), return as is
                    if (/^\d{2}:\d{2}:\d{2}$/.test(timeStr)) {
                        return timeStr;
                    }

                    // If in 12-hour format (HH:mm:ss AM/PM)
                    const match = timeStr.match(/(\d{1,2}):(\d{2}):(\d{2})\s*(AM|PM)/i);
                    if (match) {
                        let hours = parseInt(match[1]);
                        const minutes = match[2];
                        const seconds = match[3];
                        const period = match[4].toUpperCase();

                        if (period === 'PM' && hours !== 12) {
                            hours += 12;
                        } else if (period === 'AM' && hours === 12) {
                            hours = 0;
                        }

                        return `${String(hours).padStart(2, '0')}:${minutes}:${seconds}`;
                    }

                    return timeStr;
                }

                const dateFromISO = convertToISODate(dateFrom);
                const dateToISO = convertToISODate(dateTo);

                // Convert times to 24-hour format
                entryTime = convertTo24Hour(entryTime);
                exitTime = convertTo24Hour(exitTime);

                // Update hidden inputs with combined datetime
                if (entryTime && dateFromISO) {
                    const combinedEntryTime = `${dateFromISO} ${entryTime}`;
                    $('#EntryTimeHidden').val(combinedEntryTime);
                    console.log('Combined Entry Time:', combinedEntryTime);
                }

                if (exitTime && dateToISO) {
                    const combinedExitTime = `${dateToISO} ${exitTime}`;
                    $('#ExitTimeHidden').val(combinedExitTime);
                    console.log('Combined Exit Time:', combinedExitTime);
                }


                // Enable AttendanceTypeCodeTwo before submission
                $("#AttendanceTypeCodeTwo").prop("disabled", false);

                const formData = new FormData(this);          

                //$("#AttendanceTypeCodeTwo").prop("disabled", false);
                //var form = $(this)[0];
                //var formData = new FormData(form);
                const attendanceTypeCode = formData.get("AttendanceTypeCode");

                if (attendanceTypeCode === "3") {
                    // Add a new field AttendanceTypeName
                    formData.set("AttendanceTypeName", "On Tour");
                }


                formData.append('SelectedEmployeeIds', JSON.stringify(selectedEmployeeIds));
                var actionUrl = $(this).attr('action');
                formData.forEach((value, key) => {
                    console.log(key, value);
                });
                $.ajax({
                    type: 'POST',
                    url: actionUrl,
                    data: formData,
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        showLoadingIndicator();
                    },
                    success: function (result) {
                        if (result.noSavePermission || result.noUpdatePermission || result.isDuplicate) {
                            toastr.error(result.message);
                        } else if (result.isSuccess) {
                            $(settings.gridContainer).html(result.html);
                            clear();
                            initialize();
                            toastr.success(result.message, 'Success');
                            if (result.companyCode) {
                                $('#CompanyCode').val(result.companyCode).trigger('change');
                            }
                            // Trigger the custom event for CompanyCode
                            $('#CompanyCode').trigger(result.companyCode);
                        } else {
                            $(settings.formSelector).html(result);
                            /*initialize();*/
                        }
                    },
                    error: function () {
                        toastr.error('Failed Insert.');
                    },
                    complete: function () {
                        hideLoadingIndicator();
                    }
                });
            });


            //$(document).on('change', '#BranchCode', function () {
            //    var selectedBranch = $(this).val();
            //    if (!selectedBranch) return;

            //    getDepartmentByCompany(selectedBranch);
            //});

            $(document).on('change', '#CompanyCode', function () {
                var selectedCompany = $(this).val();
                if (!selectedCompany) {
                    return;
                }

                $.ajax({
                    url: '/ManualAttendanceBulk/GetBranchByCompany',
                    type: 'GET',
                    data: { companyId: selectedCompany },
                    success: function (data) {
                        if (data && data.length > 0) {
                            var branchDropDown = $('#BranchCode');
                            branchDropDown.empty();

                            branchDropDown.append('<option value="">---- Select Branch ----</option>')

                            $.each(data, function (index, branch) {
                                branchDropDown.append('<option value="' + branch.branchCode + '">' + branch.branchName + '</option>')
                            });

                            branchDropDown.trigger('change');
                        } else {
                            branchDropDown = $('#BranchCode');
                            branchDropDown.empty();
                            branchDropDown.append('<option>No branch available</option>')
                        }
                    },
                    error: function () {
                        console.error('Failed to fetch branch.');
                        toastr.error('Failed to fetch branch.');
                    }
                });
                loadTableData(selectedCompany);
                loadEmployeeData(selectedCompany);
                getDepartmentByCompany(selectedCompany);
                getDesignationByCompany(selectedCompany);
            });





            $("body").on('click', settings.selectAllSelector, function () {
                $('.checkBox').prop('checked', $(this).prop('checked'));
            });

            $("body").on('click', '#manualAttendanceEmployeeList-check-all', function () {
                $('.EmployeeListCheckBox').prop('checked', $(this).prop('checked'));
            });

            // Delete confirmation
            $("body").on('click', settings.deleteSelector, function (e) {
                e.preventDefault();
                var selectedIds = [];
                var selectedEmployeeIds = [];

                $('.checkBox:checked').each(function () {
                    selectedIds.push($(this).val());
                });

                $('.EmployeeListCheckBox:checked').each(function () {
                    selectedEmployeeIds.push($(this).data('id'));
                });

                var attendanceTypeCode = $('#AttendanceTypeCode').val();
                var fromDate = $('#DateFrom').val();
                var toDate = $('#DateTo').val();

                if (selectedIds.length === 0 && selectedEmployeeIds.length === 0) {
                    toastr.info('Please select records to delete.');
                    return;
                }

                $(settings.deleteModal + ' ' + settings.finalDeleteSelector).data('ids', selectedIds);
                $(settings.deleteModal + ' ' + settings.finalDeleteSelector).data('selectedEmployeeIds', selectedEmployeeIds);
                $(settings.deleteModal + ' ' + settings.finalDeleteSelector).data('attendanceTypeCode', attendanceTypeCode);
                $(settings.deleteModal + ' ' + settings.finalDeleteSelector).data('fromDate', fromDate);
                $(settings.deleteModal + ' ' + settings.finalDeleteSelector).data('toDate', toDate);

                $(settings.deleteModal).modal('show');
            });

            // Final delete action
            //$("body").on('click', settings.finalDeleteSelector, function (e) {
            //    e.preventDefault();
            //    var selectedIds = $(this).data('ids');
            //    var selectedEmployeeIds = $(this).data('selectedEmployeeIds');
            //    var attendanceTypeCode = $(this).data('attendanceTypeCode');
            //    var fromDate = $(this).data('fromDate');
            //    var toDate = $(this).data('toDate');

            //    var idsString = selectedIds.join(',');
            //    var employeeIdsString = selectedEmployeeIds.join(',');

            //    $.ajax({
            //        type: 'POST',
            //        url: '/ManualAttendanceBulk/Delete',
            //        data: {
            //            ids: idsString,
            //            selectedEmployeeIds: employeeIdsString,
            //            attendanceTypeCode: attendanceTypeCode,
            //            fromDate: fromDate,
            //            toDate: toDate
            //        },
            //        beforeSend: function () {
            //            showLoadingIndicator();
            //        },
            //        success: function (result) {
            //            if (result.isSuccess) {
            //                clear();
            //                toastr.success(result.message);
            //                $('.checkBox').prop('checked', false);
            //                $('.EmployeeListCheckBox').prop('checked', false);
            //            } else {
            //                toastr.error(result.message, 'Error');
            //            }
            //            $(settings.deleteModal).modal('hide');
            //        },
            //        error: function (xhr) {
            //            toastr.error('An error occurred while deleting the records.');
            //            $(settings.deleteModal).modal('hide');
            //        },
            //        complete: function () {
            //            hideLoadingIndicator();
            //        }
            //    });
            //});

            $("body").on('click', settings.finalDeleteSelector, function (e) {
                e.preventDefault();

                var selectedEmployeeIds = $(this).data('selectedEmployeeIds');
                var attendanceTypeCode = $(this).data('attendanceTypeCode');
                var fromDate = $(this).data('fromDate');
                var toDate = $(this).data('toDate');
                var isBothInOutEntry = $('#ISBothInOutEntry').is(':checked'); // checkbox থেকে value নেওয়া

                var employeeIdsString = Array.isArray(selectedEmployeeIds)
                    ? selectedEmployeeIds.join(',')
                    : selectedEmployeeIds;

                $.ajax({
                    type: 'POST',
                    url: '/ManualAttendanceBulk/Delete',
                    data: {
                        selectedEmployeeIds: employeeIdsString,
                        attendanceTypeCode: attendanceTypeCode,
                        fromDate: fromDate,
                        toDate: toDate,
                        isBothInOutEntry: isBothInOutEntry
                    },
                    beforeSend: function () { showLoadingIndicator(); },
                    success: function (result) {
                        if (result.isSuccess) {
                            clear();
                            toastr.success(result.message);
                            $('.checkBox').prop('checked', false);
                            $('.EmployeeListCheckBox').prop('checked', false);
                        } else {
                            toastr.error(result.message, 'Error');
                        }
                        $(settings.deleteModal).modal('hide');
                    },
                    error: function () {
                        toastr.error('An error occurred while deleting the records.');
                        $(settings.deleteModal).modal('hide');
                    },
                    complete: function () { hideLoadingIndicator(); }
                });
            });





            $('.js-manualAttendanceBulk-clear').on('click', function () {
                clear();
            });

            function clear() {
                $('#manualAttendanceBulk-form')[0].reset();
                $('#AutoId').val('0');

                // Select2 reset
                $(".selectpickermanualAttendanceBulk").val(null).trigger("change");
                $('.selectpickermanualAttendanceBulk').next('.select2-container').css('border', '');

                // Datepicker reset
                $('.datepicker').val('');
                $('#DateFrom').css('border', '');
                $('#DateTo').css('border', '');

                // Validation reset
                $('.text-danger').text('');

                // AttendanceTypeCodeTwo disable
                $("#AttendanceTypeCodeTwo").prop("disabled", true);

                // Remarks reset
                $("#Remarks").val('');

                // ISBothInOutEntry reset
                $('#ISBothInOutEntry').prop({ checked: false, disabled: true });

                const entryEl = document.getElementById('EntryTime');
                $('#EntryTime').css('border', '');
                $('#EntryTimeHidden').val('');
                if (entryEl && entryEl._flatpickr) {
                    entryEl._flatpickr.setDate(new Date());
                } else {
                    setTimeout(function () {
                        initTimePicker('EntryTime');
                    }, 50);
                }

                destroyTimePicker('ExitTime');
                $('#ExitTime').css('border', '');
                $('#ExitTimeHidden').val('');
                setTimeout(function () {
                    initTimePicker('ExitTime');
                    setTimeout(function () {
                        disableExitTime();
                    }, 60);
                }, 50);

                // Company Dropdown
                var companyOptions = $('#CompanyCode option');
                if (companyOptions.length === 2) {
                    $('#CompanyCode').val(companyOptions.eq(1).val()).trigger("change");
                } else {
                    $('#CompanyCode').val('').trigger("change");
                    $('#CompanyCode').next('.select2-container').css('border', '');
                }

                // Table reload
                loadTableData(companyCode);
                loadEmployeeData();
                getDate();
            }




            //$('#AttendanceTypeCode').on('change keyup', function () {
            //    var selectedValue = $(this).val();

            //    if (selectedValue == "01") {
            //        $('#AttendanceTypeCodeTwo').val('02').trigger('change');
            //        $('#ISBothInOutEntry').prop('disabled', false);
            //    } else {
            //        $('#AttendanceTypeCodeTwo').val('').trigger('change');
            //        $('#ISBothInOutEntry').prop('checked', false);
            //        $('#ISBothInOutEntry').prop('disabled', true);
            //        $('#ExitTime').val('');
            //        $('#ExitTime').prop('disabled', true);
            //    }
            //});


            //$('#AttendanceTypeCode').on('change keyup', function () {       
            //    const selectedValue = $(this).val();
            //    const $exitTime = $('#ExitTime');
            //    const $exitLabel = $('.ExitTimeLabel');
            //    const exitInput = document.getElementById('ExitTime');

            //    // Default reset
            //    $exitLabel.show();
            //    $exitTime.show();
            //    $('#AttendanceTypeCodeTwo').val('').trigger('change');
            //    $('#ISBothInOutEntry').prop({ checked: false, disabled: true });

            //    // Cleanup flatpickr if exists
            //    if (exitInput && exitInput._flatpickr) {
            //        try {
            //            exitInput._flatpickr.destroy();
            //            exitInput._flatpickr = undefined;
            //        } catch (e) { }
            //    }

            //    if (selectedValue === "1") {
            //        $('#AttendanceTypeCodeTwo').val('2').trigger('change');
            //        $('#ISBothInOutEntry').prop('checked', false);
            //        $('#ISBothInOutEntry').prop('disabled', false);

            //        $exitLabel.fadeIn();
            //        $exitTime.prop('disabled', false).fadeIn();

            //        setTimeout(() => {
            //            initTimePicker('ExitTime');
            //            $exitLabel.fadeIn();
            //        }, 200);

            //    } else if (selectedValue === "3") {
            //        //$("#Remarks").val("On Tour ");
            //        $('#AttendanceTypeCodeTwo').val('3').trigger('change');
            //        $('#ISBothInOutEntry').prop('disabled', true);

            //        $exitLabel.fadeIn();
            //        $exitTime.prop('disabled', false).fadeIn();

            //        setTimeout(() => {
            //            initTimePicker('ExitTime');
            //            $exitLabel.fadeIn();
            //        }, 200);
            //    }

            //    if (selectedValue === "3") {
            //        $("#Remarks").val("On Tour ");
            //    } else if (selectedValue === "1") {
            //        $("#Remarks").val("Manual Enter & Exit ");
            //    } else if (selectedValue === "2") {
            //        $("#Remarks").val("Manual Exit ");
            //    } else if (selectedValue === "4") {
            //        $("#Remarks").val("Office Visit (Enter) ");
            //    } else if (selectedValue === "5") {
            //        $("#Remarks").val("Office Visit (Exit) ");
            //    } else if (selectedValue === "6") {
            //        $("#Remarks").val("Absent ");
            //    } else {
            //        $("#Remarks").val("");
            //    }

            //});


            //$('#ISBothInOutEntry').prop('disabled', true);
            //$('#ISBothInOutEntry').on('change', function () {
            //    if ($(this).is(':checked')) {
            //        $('#ExitTime').prop('disabled', false);
            //    } else {
            //        $('#ExitTime').prop('disabled', true);
            //    }
            //});
            //$('#ISBothInOutEntry').trigger('change');

            // ------------------------------------------------
            // Helper functions
            // ------------------------------------------------
            function destroyTimePicker(inputId) {
                const el = document.getElementById(inputId);
                if (el && el._flatpickr) {
                    try {
                        el._flatpickr.destroy();
                        el._flatpickr = undefined;
                    } catch (e) { }
                }
            }

            //function initTimePicker(inputId) {
            //    const input = document.getElementById(inputId);
            //    const hiddenInput = document.getElementById(inputId + 'Hidden');
            //    if (!input || input._flatpickr) return;

            //    flatpickr(input, {
            //        enableTime: true,
            //        noCalendar: true,
            //        dateFormat: "h:i:S K",
            //        time_24hr: false,
            //        enableSeconds: true,
            //        inline: true,
            //        defaultDate: new Date(),
            //        minuteIncrement: 1,
            //        onChange: function (selectedDates) {
            //            if (selectedDates.length > 0) {
            //                const formatted = selectedDates[0].toLocaleTimeString('en-US', {
            //                    hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: true
            //                });
            //                if (hiddenInput) hiddenInput.value = formatted;
            //            }
            //        }
            //    });
            //}

            // ============================================================
            // Two Digit Limit Helper
            // ============================================================
            function applyTwoDigitLimit(container) {
                const numInputs = container.querySelectorAll(".numInput");
                numInputs.forEach(function (input) {
                    input.addEventListener("keydown", function (e) {
                        const allowedKeys = ["Backspace", "Delete", "ArrowLeft", "ArrowRight", "ArrowUp", "ArrowDown", "Tab"];
                        if (allowedKeys.includes(e.key)) return;
                        if (!/[0-9]/.test(e.key)) { e.preventDefault(); return; }
                        const currentVal = String(input.value || "");
                        if (currentVal.length <= 2 && input._isSelected) {
                            input._isSelected = false;
                            return;
                        }
                        if (currentVal.length >= 2) e.preventDefault();
                    });
                    input.addEventListener("focus", function () { input._isSelected = true; });
                    input.addEventListener("mousedown", function () { input._isSelected = true; });
                    input.addEventListener("input", function () {
                        input._isSelected = false;
                        if (String(input.value).length > 2) {
                            input.value = String(input.value).slice(0, 2);
                        }
                    });
                    input.addEventListener("change", function () { input._isSelected = false; });
                });
            }

            // ============================================================
            // TimePicker (flatpickr inline)
            // ============================================================
            function initTimePicker(inputId, defaultTime, disableTime) {
                const input = document.getElementById(inputId);
                const hiddenInput = document.getElementById(inputId + 'Hidden');
                if (!input) return;

                if (input._flatpickr) {
                    try { input._flatpickr.destroy(); } catch (e) { }
                    input._flatpickr = undefined;
                }

                const fp = flatpickr(input, {
                    enableTime: true,
                    noCalendar: true,
                    dateFormat: "h:i:s K",
                    time_24hr: false,
                    enableSeconds: true,
                    inline: true,
                    defaultDate: defaultTime || new Date(),
                    minuteIncrement: 1,
                    secondIncrement: 1,                         
                    onReady: function (selectedDates, dateStr, instance) {
                        applyTwoDigitLimit(instance.calendarContainer); 
                    },
                    onChange: function (selectedDates) {
                        if (selectedDates.length > 0 && hiddenInput) {
                            hiddenInput.value = selectedDates[0].toLocaleTimeString('en-US', {
                                hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: true
                            });
                        }
                    }
                });

                if (disableTime) {
                    input.disabled = true;
                    input.readOnly = true;
                    if (fp.calendarContainer) fp.calendarContainer.classList.add('fp-ui-disabled');
                } else {
                    input.disabled = false;
                    input.readOnly = false;
                    if (fp.calendarContainer) fp.calendarContainer.classList.remove('fp-ui-disabled');
                }
            }

            function enableExitTime() {
                $('#ExitTime').prop('disabled', false);

                const exitEl = document.getElementById('ExitTime');

                if (!exitEl._flatpickr) {
                    initTimePicker('ExitTime');
                }

                setTimeout(function () {
                    if (exitEl._flatpickr && exitEl._flatpickr.calendarContainer) {
                        exitEl._flatpickr.calendarContainer.classList.remove('fp-ui-disabled');
                    }
                }, 60);
            }
         
            function disableExitTime() {
                $('#ExitTime').prop('disabled', true);

                const exitEl = document.getElementById('ExitTime');

                if (!exitEl._flatpickr) {
                    initTimePicker('ExitTime');
                }

                setTimeout(function () {
                    if (exitEl._flatpickr && exitEl._flatpickr.calendarContainer) {
                        exitEl._flatpickr.calendarContainer.classList.add('fp-ui-disabled');
                    }
                }, 60);
            }

            // ------------------------------------------------
            // Page load — EntryTime  ExitTime init
            // ------------------------------------------------
            $(document).ready(function () {
                initTimePicker('EntryTime');
                disableExitTime(); 
            });

            // ------------------------------------------------
            // AttendanceTypeCode change
            // ------------------------------------------------
            $('#AttendanceTypeCode').on('change keyup', function () {
                const selectedValue = $(this).val();

                // Reset
                $('#AttendanceTypeCodeTwo').val('').trigger('change');
                $('#ISBothInOutEntry').prop({ checked: false, disabled: true });
                disableExitTime();
                $('.ExitTimeLabel').show();

                if (selectedValue === "1") {
                    $('#AttendanceTypeCodeTwo').val('2').trigger('change');
                    $('#ISBothInOutEntry').prop('disabled', false);
                } else if (selectedValue === "3") {
                    $('#AttendanceTypeCodeTwo').val('3').trigger('change');
                    $('#ISBothInOutEntry').prop('disabled', false);
                    // Type 3 এ সরাসরি ExitTime enable
                    enableExitTime();
                }

                // Remarks
                //const remarksMap = {
                //    "1": "Manual Enter & Exit ",
                //    "2": "Manual Exit ",
                //    "3": "On Tour ",
                //    "4": "Office Visit (Enter) ",
                //    "5": "Office Visit (Exit) ",
                //    "6": "Absent "
                //};
                //$("#Remarks").val(remarksMap[selectedValue] || "");
            });

            // ------------------------------------------------
            // ISBothInOutEntry checkbox
            // ------------------------------------------------
            $('#ISBothInOutEntry').prop('disabled', true);
            $('#ISBothInOutEntry').on('change', function () {
                if ($(this).is(':checked')) {
                    enableExitTime();
                } else {
                    disableExitTime();
                }
            });


            // Front side validation start
            $(settings.saveSelector).on('click', function () {
                var companyDD = $('#CompanyCode').val();
                var employeeChecked = $('.EmployeeListCheckBox:checked').length > 0;
                var attendanceTypeDD = $('#AttendanceTypeCode').val();
                var dateFrom = $('#DateFrom').val();
                var dateTo = $('#DateTo').val();
                var entryTime = $('#EntryTime').val();
                var exitTime = $('#ExitTime').val();

                validateCompanyDD();
                validateEmpBorder();
                validateAttendanceTypeDD();
                validateDateFrom();
                validateDateTo();
                validateEntryTime();
                //validateExitTime();

                if (!companyDD) {
                    toastr.warning('Please select Company');
                    $('#CompanyCode').select2('open');
                    return false;
                }

                if (!employeeChecked) {
                    toastr.warning('Please select Employee!');
                    return false;
                }

                if (!attendanceTypeDD) {
                    toastr.warning('Please select attendance type!');
                    $('#AttendanceTypeCode').select2('open');
                    return false;
                }

                if (dateFrom === '') {
                    toastr.warning('Please select from date!');
                    $('#DateFrom').datepicker('show');
                    return false;
                }

                if (dateTo === '') {
                    toastr.warning('Please select to date!');
                    $('#DateTo').datepicker('show');
                    return false;
                }

                if (entryTime === '') {
                    toastr.warning('Please select entry time!');
                    //$('#EntryTime').datetimepicker('show');
                    return false;
                }

                //if (exitTime == '') {
                //    toastr.warning('Please select exit time!');
                //    $('#ExitTime').datetimepicker('show');
                //    return false;
                //}
            });


            $('#CompanyCode').on('change', function () {
                validateCompanyDD();
            });


            function validateCompanyDD() {
                var selectElement = $('#CompanyCode');
                var selectContainer = selectElement.next('.select2-container');

                if (selectElement.val().trim() == '') {
                    selectContainer.css({
                        'border': '1px solid red', 'border-radius': '5px'
                    });
                    selectElement.css({
                        'border': '1px solid red', 'border-radius': '5px'
                    });
                } else {
                    selectContainer.css({
                        'border': '', 'border-radius': ''
                    });
                    selectElement.css({
                        'border': '', 'border-radius': ''
                    });
                }
            }


            $('.EmployeeListCheckBox').on('click change blur', function () {
                validateEmpBorder();
            });

            function validateEmpBorder() {
                var employeeChecked = $('.EmployeeListCheckBox:checked').length > 0;
                var empTableDiv = $('.manualAttendanceEmpBorderValidate');

                if (!employeeChecked) {
                    empTableDiv.css('border', '1px solid red');
                } else {
                    empTableDiv.css('border', '1px solid lightgray');
                }
            }

            $('#AttendanceTypeCode').on('change click blur', function () {
                validateAttendanceTypeDD();
            });

            function validateAttendanceTypeDD() {
                var selectElement = $('#AttendanceTypeCode');
                var selectContainer = selectElement.next('.select2-container');

                if (selectElement.val().trim() == '') {
                    selectContainer.css({
                        'border': '1px solid red', 'border-radius': '5px'
                    });
                    selectElement.css({
                        'border': '1px solid red', 'border-radius': '5px'
                    });
                } else {
                    selectContainer.css({
                        'border': '', 'border-radius': ''
                    });
                    selectElement.css({
                        'border': '', 'border-radius': ''
                    });
                }
            }

            $('#DateFrom').on('change blur', function () {
                validateDateFrom();
            });

            function validateDateFrom() {
                if ($('#DateFrom').val().trim() === '' || $('#DateFrom').val().trim() === null) {
                    $('#DateFrom').css('border', '1px solid red');
                } else {
                    $('#DateFrom').css('border', '');
                }
            }

            $('#DateTo').on('change blur', function () {
                validateDateTo();
            });

            function validateDateTo() {
                if ($('#DateTo').val().trim() === '' || $('#DateTo').val().trim() === null) {
                    $('#DateTo').css('border', '1px solid red');
                } else {
                    $('#DateTo').css('border', '');
                }
            }

            $('#EntryTime').on('change blur', function () {
                validateEntryTime();
            });

            function validateEntryTime() {
                if ($('#EntryTime').val().trim() === '' || $('#EntryTime').val().trim() === null) {
                    $('#EntryTime').css('border', '1px solid red');
                } else {
                    $('#EntryTime').css('border', '');
                }
            }

            $('#ExitTime').on('change blur', function () {
                validateExitTime();
            });

            function validateExitTime() {
                if ($('#ExitTime').val().trim() === '' || $('#ExitTime').val().trim() === null) {
                    $('#ExitTime').css('border', '1px solid red');
                } else {
                    $('#ExitTime').css('border', '');
                }
            }
            // Front side validation end

            // Initialization for not showing date/time on page load
            $('#DateFrom').val('');
            $('#DateTo').val('');
            $('#EntryTime').val('');
            $('#ExitTime').val('');



            $('body').on('click', settings.saveSelector, function () {
                validateCompanyDD();
                validateAttendanceTypeDD();
                validateDateFrom();
                validateEntryTime();
                validateExitTime();
                $(settings.formSelector).submit();
            });

            $(document).on('change', '#BranchCode', function () {
                var selectedBranch = $(this).val();
                var selectedCompany = $('#CompanyCode').val();
                if (!selectedBranch) return;

                // Department load
                $.ajax({
                    type: 'GET',
                    url: '/ManualAttendanceBulk/GetDepartmentByBranchId',
                    data: { branchId: selectedBranch },
                    success: function (data) {
                        var $dept = $('.gc-department');
                        try { $dept.multiselect('destroy'); } catch (e) { }
                        $dept.empty();

                        if (data && data.length > 0) {
                            $.each(data, function (index, department) {
                                $dept.append('<option value="' + department.departmentCode + '">' + department.departmentName + '</option>');
                            });
                        }

                        bsms_Reset(".gc-department");
                        gcBindRemoteMultiselect(".gc-department", "/GcFilters/department", "Select Department", "department");
                        bsms_InitializeMultiselects({ '.gc-department': 'Select Department' });
                    },
                    error: function () { toastr.error('Failed to fetch department'); }
                });

                // Designation load
                $.ajax({
                    type: 'GET',
                    url: '/ManualAttendanceBulk/GetDesignationByBranch',
                    data: { branchId: selectedBranch },
                    success: function (data) {
                        var $desig = $('.gc-designation');
                        try { $desig.multiselect('destroy'); } catch (e) { }
                        $desig.empty();

                        if (data && data.length > 0) {
                            $.each(data, function (index, designation) {
                                $desig.append('<option value="' + designation.designationCode + '">' + designation.designationName + '</option>');
                            });
                        }

                        bsms_Reset(".gc-designation");
                        gcBindRemoteMultiselect(".gc-designation", "/GcFilters/designation", "Select Designation", "designation");
                        bsms_InitializeMultiselects({ '.gc-designation': 'Select Designation' });
                    },
                    error: function () { toastr.error('Failed to fetch designation'); }
                });
            });

            //$(document).on('change', '.gc-department,#ActivityStatusCode', function () {
            //    var selectedDepartment = $('.gc-department').val(); // array
            //    var selectedCompany = $('#CompanyCode').val();
            //    var selectedBranch = $('#BranchCode').val();
            //    var selectedListType = $('#ListTypeCode').val();
            //    var selectedActiveStatus = $('#ActivityStatusCode').val();

            //    if (!selectedDepartment || selectedDepartment.length === 0) return;

            //    // Designation update
            //    $.ajax({
            //        type: 'GET',
            //        url: '/ManualAttendanceBulk/GetDesignationByDepartment',
            //        traditional: true,
            //        data: { departmentId: selectedDepartment },
            //        success: function (data) {
            //            var $desig = $('.gc-designation');
            //            try { $desig.multiselect('destroy'); } catch (e) { }
            //            $desig.empty();

            //            if (data && data.length > 0) {
            //                $.each(data, function (index, designation) {
            //                    $desig.append('<option value="' + designation.designationCode + '">' + designation.designationName + '</option>');
            //                });
            //            }

            //            bsms_Reset(".gc-designation");
            //            gcBindRemoteMultiselect(".gc-designation", "/GcFilters/designation", "Select Designation", "designation");
            //            bsms_InitializeMultiselects({ '.gc-designation': 'Select Designation' });
            //        },
            //        error: function () { toastr.error('Failed to fetch designation'); }
            //    });

            //    // Employee table update
            //    $.ajax({
            //        type: 'GET',
            //        url: '/ManualAttendanceBulk/GetEmployeeByDepartment',
            //        traditional: true,
            //        data: {
            //            companyId: selectedCompany,
            //            branchId: selectedBranch,
            //            departmentId: selectedDepartment,
            //            selectedListType: selectedListType,
            //            selectedActiveStatus: selectedActiveStatus,
            //        },
            //        success: function (data) {
            //            if (data && data.length > 0) {
            //                $('.js-manualAttendanceEmployee-grid-container').html(data);
            //                EmployeeDataTable();
            //            } else {
            //                const noDataMessage = `<tr><td colspan="5" class="text-center">No data found</td></tr>`;
            //                if ($.fn.dataTable.isDataTable('.manualAttendanceEmployeeList')) {
            //                    $('.manualAttendanceEmployeeList').DataTable().clear().destroy();
            //                }
            //                $('.manualAttendanceEmployeeList tbody').html(noDataMessage);
            //            }
            //        },
            //        error: function () { toastr.error('Failed to load employees'); }
            //    });
            //});


            // ✅ Designation change → Employee table update
            $(document).on('change', '#BranchCode, .gc-designation, .gc-department,#ListTypeCode, #ActivityStatusCode', function () {
               
                var selectedDesignation = $('.gc-designation').val(); // array
                var selectedCompany = $('#CompanyCode').val();
                var selectedBranch = $('#BranchCode').val();
                var selectedDepartment = $('.gc-department').val(); // array
                var selectedListType = $('#ListTypeCode').val();
                var selectedActiveStatus = $('#ActivityStatusCode').val();
                //if (!selectedDesignation || selectedDesignation.length === 0) return;

                $.ajax({
                    type: 'GET',
                    url: '/ManualAttendanceBulk/GetEmployeeByDesignation',
                    traditional: true,
                    data: {
                        companyId: selectedCompany,
                        branchId: selectedBranch,
                        departmentId: selectedDepartment,
                        designationId: selectedDesignation,
                        selectedListType: selectedListType,
                        selectedActiveStatus: selectedActiveStatus,
                    },
                    success: function (data) {
                       
                        if (data && data.length > 0) {
                            $('.js-manualAttendanceEmployee-grid-container').html(data);
                            EmployeeDataTable();
                        } else {
                            const noDataMessage = `<tr><td colspan="5" class="text-center">No data found</td></tr>`;
                            if ($.fn.dataTable.isDataTable('.manualAttendanceEmployeeList')) {
                                $('.manualAttendanceEmployeeList').DataTable().clear().destroy();
                            }
                            $('.manualAttendanceEmployeeList tbody').html(noDataMessage);
                        }
                    },
                    error: function () {
                        //toastr.error('Failed to load employees');
                    }
                });

            });
            //    function initTimePicker(inputId) {

            //        const input = document.getElementById(inputId);
            //        const hiddenInput = document.getElementById(inputId + 'Hidden');

            //        if (!input || input.disabled) return;

            //        // Destroy existing instance first
            //        if (input._flatpickr) {
            //            input._flatpickr.destroy();
            //            input._flatpickr = null;
            //        }

            //        try {
            //            const fp = flatpickr(input, {
            //                enableTime: true,
            //                noCalendar: true,
            //                dateFormat: "h:i:S K",
            //                time_24hr: false,
            //                enableSeconds: true,
            //                inline: true,
            //                defaultDate: new Date(),
            //                minuteIncrement: 1,
            //                onChange: function (selectedDates, dateStr, instance) {
            //                    if (selectedDates.length > 0) {
            //                        const date = selectedDates[0];
            //                        const formatted = date.toLocaleTimeString('en-US', {
            //                            hour: '2-digit',
            //                            minute: '2-digit',
            //                            second: '2-digit',
            //                            hour12: true
            //                        });
            //                        hiddenInput.value = formatted;
            //                    }
            //                }
            //            });
            //        } catch (error) {
            //            console.error('Flatpickr init error:', error);
            //        }
            //    }
            //    $(document).ready(function () {
            //        initTimePicker('EntryTime');

            //        $('#ISBothInOutEntry').on('change', function () {
            //            const exitInput = document.getElementById('ExitTime');

            //            if ($(this).is(':checked')) {
            //                // Cleanup first
            //                if (exitInput && exitInput._flatpickr) {
            //                    try {
            //                        exitInput._flatpickr.destroy();
            //                        exitInput._flatpickr = undefined;
            //                    } catch (e) { }
            //                }

            //                $('#ExitTime').prop('disabled', false);
            //                setTimeout(() => {
            //                    initTimePicker('ExitTime')
            //                    $('.ExitTimeLabel').show().fadeIn();
            //                }, 200);
            //            } else {
            //                // Cleanup
            //                if (exitInput && exitInput._flatpickr) {
            //                    try {
            //                        exitInput._flatpickr.destroy();
            //                        exitInput._flatpickr = undefined;
            //                    } catch (e) { }
            //                }

            //                $('#ExitTime').prop('disabled', true);
            //                $('.ExitTimeLabel').hide().fadeOut();
            //                $('#ExitTimeHidden').val('');
            //            }
            //        });
            //    });
            //});


            //// #region initTimePicker
            //function initTimePicker(inputId) {
            //    const input = document.getElementById(inputId);
            //    const hiddenInput = document.getElementById(inputId + 'Hidden');

            //    if (!input || input.disabled || input._flatpickr) return;

            //    flatpickr(input, {
            //        enableTime: true,
            //        noCalendar: true,
            //        dateFormat: "h:i:S K",
            //        time_24hr: false,
            //        enableSeconds: true,
            //        inline: true,
            //        defaultDate: new Date(),
            //        minuteIncrement: 1,
            //        onChange: function (selectedDates, dateStr, instance) {
            //            if (selectedDates.length > 0) {
            //                const date = selectedDates[0];
            //                const formatted = date.toLocaleTimeString('en-US', {
            //                    hour: '2-digit',
            //                    minute: '2-digit',
            //                    second: '2-digit',
            //                    hour12: true
            //                });
            //                hiddenInput.value = formatted;
            //            }
            //        }
            //    });
            //}
            //// #endregion


            //// #region ISBothInOutEntry on change
            //$(document).ready(function () {
            //    initTimePicker('EntryTime');

            //    $('#ISBothInOutEntry').on('change', function () {
            //        const exitInput = document.getElementById('ExitTime');

            //        if ($(this).is(':checked')) {
            //            // Cleanup first
            //            if (exitInput && exitInput._flatpickr) {
            //                try {
            //                    exitInput._flatpickr.destroy();
            //                    exitInput._flatpickr = undefined;
            //                } catch (e) { }
            //            }

            //            $('#ExitTime').prop('disabled', false);
            //            setTimeout(() => {
            //                initTimePicker('ExitTime')
            //                $('.ExitTimeLabel').show().fadeIn();
            //            }, 200);
            //        } else {
            //            // Cleanup
            //            if (exitInput && exitInput._flatpickr) {
            //                try {
            //                    exitInput._flatpickr.destroy();
            //                    exitInput._flatpickr = undefined;
            //                } catch (e) { }
            //            }

            //            $('#ExitTime').prop('disabled', true);
            //            $('.ExitTimeLabel').show().fadeIn();
            //            $('#ExitTimeHidden').val('');
            //        }
            //    });
            //});
            //// #endregion

        });

        //function initialize() {

        //    loadTableData(companyCode);
        //    loadEmployeeData();

        //    $('.selectpickermanualAttendanceBulk').select2({
        //        language: {
        //            noResults: function () {

        //            }
        //        },
        //        escapeMarkup: function (markup) {
        //            return markup;
        //        }
        //    });


        //    $('.datepicker').datepicker({
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

        //    $('.ExitTimeLabel').hide().fadeOut();
        //}

        function initialize() {

            loadTableData(companyCode);
            loadEmployeeData(companyCode);

            // ✅ ViewBag pre-loaded dropdowns — simple select2
            // CompanyCode, BranchCode, ActivityStatusCode, ListTypeCode
            $('.selectpickermanualAttendanceBulk').select2({
                placeholder: "---- Select ----",
                allowClear: true,
                width: '100%',
                language: { noResults: function () { } },
                escapeMarkup: function (markup) { return markup; }
            });

            //$('.datepicker').datepicker({
            //    dateFormat: 'dd/mm/yy',
            //    changeMonth: true,
            //    changeYear: true,
            //    yearRange: '1950:2050',
            //});

            $(document).ready(function () {               
                getDate();
            });


            $('.ExitTimeLabel').show().fadeIn();

        }


        $(document).ready(async function () {

            // Select2 single selects
            s2_InitSingle(
                "#CompanyCode",
                "/GcFilters/company",
                "Select Company",
                "company",
                ["#BranchCode", "#DivisionCode", "#DesignationCode"]
            );
            s2_InitSingle("#BranchCode", "/GcFilters/branch", "Select Branch", "branch",
                ["#DivisionCode", "#DesignationCode"]
            );
            s2_InitSingle("#DivisionCode", "/GcFilters/division", "Select Division", "division",
                ["#DesignationCode"]
            );

            // Multiselects
            gcBindRemoteMultiselect(".gc-department", "/GcFilters/department", "Select Department", "department");
            gcBindRemoteMultiselect(".gc-designation", "/GcFilters/designation", "Select Designation", "designation");

            bsms_InitializeMultiselects();
            bsms_BindCascade();
            bsms_Reset(".gc-department");
            bsms_Reset(".gc-designation");

            // Load
            await s2_LoadNext("#CompanyCode", "/GcFilters/company");

            var $comp = $("#CompanyCode");
            var defaultCode = "001";
            var defaultName = $comp.find(`option[value="${defaultCode}"]`).text();
            await s2_SetDefault("#CompanyCode", defaultCode, defaultName, true);

            await bsms_LoadNext(".gc-department", "/GcFilters/department");
            await bsms_LoadNext(".gc-designation", "/GcFilters/designation");
        });

        function getDepartmentByCompany(selectedCompany) {
            $.ajax({
                url: '/ManualAttendanceBulk/GetDepartmentByCompany',
                type: 'GET',
                data: { companyId: selectedCompany },
                success: function (data) {
                    var $dept = $('.gc-department');

                    try { $dept.multiselect('destroy'); } catch (e) { }
                    $dept.empty();

                    if (data && data.length > 0) {
                        $.each(data, function (index, department) {
                            $dept.append('<option value="' + department.departmentCode + '">' + department.departmentName + '</option>');
                        });
                    }

                    bsms_Reset(".gc-department");
                    gcBindRemoteMultiselect(".gc-department", "/GcFilters/department", "Select Department", "department");
                    bsms_InitializeMultiselects({ '.gc-department': 'Select Department' });
                },
                error: function () {
                    console.error('Error fetching department.');
                    toastr.error('Error! fetching department.');
                }
            });
        }

        function getDesignationByCompany(selectedCompany) {
            $.ajax({
                url: '/ManualAttendanceBulk/GetDesignationByCompany',
                type: 'GET',
                data: { companyId: selectedCompany },
                success: function (data) {
                    var $desig = $('.gc-designation');

                    try { $desig.multiselect('destroy'); } catch (e) { }
                    $desig.empty();

                    if (data && data.length > 0) {
                        $.each(data, function (index, designation) {
                            $desig.append('<option value="' + designation.designationCode + '">' + designation.designationName + '</option>');
                        });
                    }

                    bsms_Reset(".gc-designation");
                    gcBindRemoteMultiselect(".gc-designation", "/GcFilters/designation", "Select Designation", "designation");
                    bsms_InitializeMultiselects({ '.gc-designation': 'Select Designation' });
                },
                error: function () {
                    console.error('Error getting designation');
                    toastr.error('Error! getting designation.');
                }
            });
        }
        function loadTableData(selectedCompany) {

           
            if ($.fn.DataTable.isDataTable('#manualAttendanceBulkTable')) {
                $('#manualAttendanceBulkTable').DataTable().destroy();               
            }


            $('#manualAttendanceBulkTable').DataTable({
                processing: true,
                serverSide: true,
                responsive: true,
                destroy: true,
                pageLength: 10,
                lengthMenu: [10, 25, 50, 100],
                order: [[1, "desc"]],
                ajax: {
                    url: gridUrl,
                    type: 'POST',
                    data: function (d) {
                        d.companyId = selectedCompany; 
                    }
                },
                columns: [
                    {
                        data: 'manualCode',
                        render: function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        },
                        orderable: false,
                        searchable: false,
                        className:"text-center"
                    },
                    { data: 'manualCode' , className: "text-center" },
                    { data: 'bulkEntryId', className: "text-center" },
                    { data: 'employeeId', className: "text-center" },
                    { data: 'employeeFullName' },
                    { data: 'attendanceTypeName', className: "text-center" },
                    { data: 'dateFrom', className: "text-center" },
                    { data: 'showEntryTime', className: "text-center" },
                    { data: 'remarks', className: "text-center" },
                    { data: 'luser', className: "text-center" }
                ],
                language: {
                    emptyTable: "No data available!"
                }
            });
        }

        function loadEmployeeData(selectedCompany) {
            $.ajax({
                type: 'GET',
                url: '/ManualAttendanceBulk/GetEmployeeByCompany',
                data: { companyId: selectedCompany },
                success: function (data) {
                    if (data && data.length > 0) {
                        $('.js-manualAttendanceEmployee-grid-container').html(data);
                        EmployeeDataTable();
                    } else {
                        const noDataMessage = `
                            <tr>
                                <td colspan="5" class="text-center">No data found</td>
                            </tr>
                        `;
                        if ($.fn.dataTable.isDataTable('.manualAttendanceEmployeeList')) {
                            $('.manualAttendanceEmployeeList').DataTable().clear().destroy();
                        }
                        $('.manualAttendanceEmployeeList' + " tbody").html(noDataMessage);
                    }
                },
                error: function () {
                    toastr.error('Failed to load data');
                }
            });
        }
        function getDate() {

            $('.datepicker').datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: '1950:2050'
            }).datepicker('setDate', new Date());
        }
        //function EmployeeDataTable() {
        //    if (!$.fn.dataTable.isDataTable('.manualAttendanceEmployeeList')) {
        //        $('.manualAttendanceEmployeeList').DataTable({
        //            paging: true,
        //            searching: true,
        //            ordering: true,
        //            info: true,
        //            pageLength: 15,
        //            lengthMenu: [15, 25, 50, 100],
        //            order: [[1, "desc"]],
        //        });
        //    }
        //}

        function EmployeeDataTable() {
            if (!$.fn.dataTable.isDataTable('.manualAttendanceEmployeeList')) {
                $('.manualAttendanceEmployeeList').DataTable({
                    paging: true,
                    searching: true,
                    ordering: true,
                    info: true,

                    pageLength: -1,           
                    lengthMenu: [
                        [15, 25, 50, 100, -1],
                        [15, 25, 50, 100, "All"]
                    ],

                    scrollY: "535px",
                    scrollCollapse: true,
                    scrollX: true,

                    order: [[1, "desc"]],
                });
            }
        }
    }
}(jQuery));

