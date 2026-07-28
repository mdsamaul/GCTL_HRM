(function ($) {
    $.manualAttendance = function (options) {
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#manualAttendance-form",
            gridSelector: ".manualAttendanceTable",
            gridContainer: ".js-manualAttendance-grid-container",
            editSelector: ".js-manualAttendance-edit",
            saveSelector: ".js-manualAttendance-save",
            selectAllSelector: "#manualAttendance-check-all",
            deleteSelector: ".js-manualAttendance-delete-confirm",
            deleteModal: "#manualAttendance-delete-modal",
            finalDeleteSelector: ".js-manualAttendance-delete",
            clearSelector: ".js-manualAttendance-clear",
            topSelector: ".js-go",
            showNagativeFormat: false,
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            load: function () { }
        }, options);

        var gridUrl = settings.baseUrl + "/GetAll";
        var saveUrl = settings.baseUrl + "/Setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];

        // ============================================================
        // Employee Select2 Remote
        // ============================================================
        var EMP_SELECTOR = "#EmployeeId";
        var EMP_URL = "/GcFilters/employee";

        function empBuildReq(page, search) {
            var companyVal = $("#CompanyCode").val();
            return {
                CompanyCodes: companyVal ? [companyVal] : [],
                BranchCodes: [],
                DivisionCodes: [],
                DepartmentCodes: [],
                DesignationCodes: [],
                EmployeeStatuses: [],
                Page: page || 1,
                PageSize: 10,
                Search: search || ""
            };
        }

        var empState = { page: 1, more: true, loading: false, search: "" };
        var empSearchTimer = null;

        function empResetState() {
            empState = { page: 1, more: true, loading: false, search: "" };
        }

        // ============================================================
        // Copy helpers
        // ============================================================
        function _empCopyToClipboard(text) {
            if (!text) return;
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(text).then(function () {
                    _empShowCopyToast(text);
                }).catch(function () {
                    _empFallbackCopy(text);
                });
            } else {
                _empFallbackCopy(text);
            }
        }

        function _empFallbackCopy(text) {
            var $temp = $('<textarea>')
                .css({ position: 'fixed', top: 0, left: 0, opacity: 0 })
                .val(text)
                .appendTo('body');
            $temp[0].select();
            try { document.execCommand('copy'); _empShowCopyToast(text); } catch (e) { }
            $temp.remove();
        }

        function _empShowCopyToast(text) {
            if (typeof toastr !== 'undefined') {
                toastr.info('Copied: ' + text, '', { timeOut: 1500, positionClass: 'toast-bottom-right' });
                return;
            }
            var $toast = $(
                '<div style="position:fixed;bottom:20px;right:20px;z-index:99999;' +
                'background:#333;color:#fff;padding:8px 16px;border-radius:6px;' +
                'font-size:13px;opacity:0;transition:opacity .2s;pointer-events:none;">' +
                'Copied: ' + text + '</div>'
            ).appendTo('body');
            setTimeout(function () { $toast.css('opacity', 1); }, 10);
            setTimeout(function () { $toast.css('opacity', 0); }, 1600);
            setTimeout(function () { $toast.remove(); }, 1900);
        }

        function _empAddCopyIcon($sel) {
            $('.emp-selected-copy').remove();
            $(EMP_SELECTOR).next('.select2-container').find('.emp-selected-copy').remove();

            var selectedText = $sel.val() || '';
            if (!selectedText) return;

            var $container = $sel.next('.select2-container');
            if (!$container.length) return;

            var $icon = $(
                '<span class="emp-selected-copy" title="Copy ID" ' +
                'style="position:absolute;right:38px;top:50%;transform:translateY(-50%);' +
                'cursor:pointer;color:#888;font-size:11px;z-index:10;padding:2px 5px;' +
                'background:#f0f0f0;border-radius:3px;line-height:1;">' +
                '<i class="fa fa-copy"></i>' +
                '</span>'
            );

            $container.css('position', 'relative');

            $icon.on('mousedown click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                var currentVal = $(EMP_SELECTOR).val() || '';
                _empCopyToClipboard(currentVal);
            });

            $container.append($icon);
        }
        // copy icon
        function _empRemoveCopyIcon() {
            $(EMP_SELECTOR).next('.select2-container').find('.emp-selected-copy').remove();
        }

        // ============================================================
        // empLoadNext
        // ============================================================
        async function empLoadNext() {
            if (empState.loading || !empState.more) return;
            empState.loading = true;
            var req = empBuildReq(empState.page, empState.search);
            try {
                var res = await $.ajax({
                    url: EMP_URL,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(req)
                });
                if (!res || !res.isSuccess) return;

                var items = res.data.items || res.data.Items || [];
                var more = res.data.More ?? res.data.more ?? false;
                empState.page++;
                empState.more = more;

                var $sel = $(EMP_SELECTOR);

                items.forEach(function (x) {
                    var code = x.code || x.Code;
                    var name = x.name || x.Name;
                    if (!code) return;
                    if ($sel.find('option[value="' + code + '"]').length === 0) {
                        $sel.append(new Option(name, code, false, false));
                    }
                });

                $sel.trigger('change.select2');

                var $resultsList = $('.select2-results__options');
                if ($resultsList.length) {
                    $resultsList.find('.select2-results__option--loading').remove();

                    if (items.length === 0 && empState.page === 2) {
                        if ($resultsList.find('.emp-no-result').length === 0) {
                            $resultsList.append(
                                '<li class="select2-results__option emp-no-result" ' +
                                'style="color:#999;padding:6px 12px;">No results found</li>'
                            );
                        }
                        return;
                    }

                    items.forEach(function (x) {
                        var code = x.code || x.Code;
                        var name = x.name || x.Name;
                        if (!code) return;
                        if ($resultsList.find('li[data-emp-code="' + code + '"]').length === 0) {
                            var $li = $(
                                '<li class="select2-results__option" role="option" ' +
                                'data-emp-code="' + code + '">' + name + '</li>'
                            );
                            $li.on('mousedown', function (e) {
                                e.preventDefault();
                                $sel.find('option').prop('selected', false);
                                $sel.append(new Option(name, code, true, true));
                                $sel.val(code).trigger('change');
                                $sel.select2('close');
                                setTimeout(function () {
                                    //_empAddCopyIcon($sel);
                                }, 50);
                            });
                            $resultsList.append($li);
                        }
                    });
                }

                if ($sel.val()) {
                    setTimeout(function () {
                        //_empAddCopyIcon($sel);
                    }, 50);
                }

            } catch (err) {
                console.error(err);
            } finally {
                empState.loading = false;
            }
        }

        function empBindScroll() {
            var $resultsContainer = $(document).find('.select2-results__options');
            if (!$resultsContainer || !$resultsContainer.length) return;
            $resultsContainer.off('scroll.empPaging').on('scroll.empPaging', async function () {
                var distFromBottom = this.scrollHeight - this.scrollTop - this.clientHeight;
                if (distFromBottom < 80 && empState.more && !empState.loading) {
                    await empLoadNext();
                }
            });
        }

        function empBindSearch() {
            var $searchInput = $(document).find('.select2-search__field');
            if (!$searchInput || !$searchInput.length) return;

            $searchInput.off('input.empSearch').on('input.empSearch', function () {
                var term = $(this).val() || "";
                clearTimeout(empSearchTimer);

                empSearchTimer = setTimeout(async function () {
                    // ✅ State reset
                    empState.page = 1;
                    empState.more = true;
                    empState.loading = false;
                    empState.search = term;

                    $(EMP_SELECTOR).find('option:not([value=""])').remove();
                    $(EMP_SELECTOR).trigger('change.select2');

                    var $resultsList = $('.select2-results__options');
                    if ($resultsList.length) {
                        $resultsList.empty();

                        $resultsList.append(
                            '<li class="select2-results__option select2-results__option--loading emp-loading"' +
                            'style="color:#999;padding:6px 12px;">Loading...</li>'
                        );
                    }

                    await empLoadNext();

                    empBindScroll();

                }, 350);
            });
        }

        function empInit() {
            var $sel = $(EMP_SELECTOR);
            if (!$sel.length) return;
            if ($sel.hasClass('select2-hidden-accessible')) {
                try { $sel.select2('destroy'); } catch (e) { }
            }
            $sel.removeAttr('multiple');
            $sel.select2({
                placeholder: "Select Employee",
                allowClear: true,
                width: '100%',
                minimumResultsForSearch: 0,
            });

            $sel.on('select2:open.empRemote', function () {
                setTimeout(function () {
                    empBindScroll();
                    empBindSearch();
                }, 150);
            });

            $sel.on('select2:clear.empRemote', function () {
                empResetState();
                $sel.find('option:not([value=""])').remove();
                $sel.trigger('change.select2');
                _empRemoveCopyIcon();
            });

            // ✅ select  selected box copy icon 
            $sel.on('select2:select', function () {
                setTimeout(function () {
                    // _empAddCopyIcon($sel);
                }, 50);
            });
        }

        async function loadEmployeesByCompany(companyId, selectedEmployeeId) {
            empResetState();
            $(EMP_SELECTOR).find('option:not([value=""])').remove();
            $(EMP_SELECTOR).val(null).trigger('change.select2');
            _empRemoveCopyIcon();

            await empLoadNext();

            if (selectedEmployeeId) {
                var $sel = $(EMP_SELECTOR);

                if ($sel.find('option[value="' + selectedEmployeeId + '"]').length > 0) {
                    $sel.val(selectedEmployeeId).trigger('change');
                    setTimeout(function () { // _empAddCopyIcon($sel); 
                    }, 50);
                } else {
                    try {
                        var res = await $.ajax({
                            url: '/ManualAttendance/GetEmployeeDetailsById',
                            type: 'GET',
                            data: { id: selectedEmployeeId }
                        });

                        var name = (res && res.employeeFullName)
                            ? res.employeeFullName + ' (' + selectedEmployeeId + ')'
                            : selectedEmployeeId;

                        $sel.append(new Option(name, selectedEmployeeId, true, true));
                        $sel.val(selectedEmployeeId).trigger('change');
                        setTimeout(function () {
                            // _empAddCopyIcon($sel);
                        }, 50);
                    } catch (e) {
                        $sel.append(new Option(selectedEmployeeId, selectedEmployeeId, true, true));
                        $sel.val(selectedEmployeeId).trigger('change');
                    }
                }
            }
        }

        function destroyTimePicker(inputId) {
            const input = document.getElementById(inputId);
            if (!input) return;
            if (input._flatpickr) {
                try { input._flatpickr.destroy(); } catch (e) { }
                input._flatpickr = undefined;
            }
        }

        // ============================================================
        // ExitTime enable/disable helper
        // ============================================================
        function enableExitTime() {
            destroyTimePicker('ExitTime');
            $('#ExitTime').prop('disabled', false);
            setTimeout(function () {
                initTimePicker('ExitTime', new Date(), false);
                const exitEl = document.getElementById('ExitTime');
                if (exitEl && exitEl._flatpickr && exitEl._flatpickr.calendarContainer) {
                    exitEl._flatpickr.calendarContainer.classList.remove('fp-ui-disabled');
                }
            }, 50);
        }

        function disableExitTime() {
            const exitEl = document.getElementById('ExitTime');
            $('#ExitTime').prop('disabled', true).val('');
            $('#ExitTimeHidden').val('');

            if (exitEl && exitEl._flatpickr && exitEl._flatpickr.calendarContainer) {
                exitEl._flatpickr.calendarContainer.classList.add('fp-ui-disabled');
            }

            if (!exitEl || !exitEl._flatpickr) {
                initTimePicker('ExitTime', new Date(), false);
                $('#ExitTime').prop('disabled', true);
                if (exitEl && exitEl._flatpickr && exitEl._flatpickr.calendarContainer) {
                    exitEl._flatpickr.calendarContainer.classList.add('fp-ui-disabled');
                }
            }
        }



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
                secondIncrement: 1,                          // ✅ add
                onReady: function (selectedDates, dateStr, instance) {
                    applyTwoDigitLimit(instance.calendarContainer); // ✅ add
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

        // ============================================================
        // Clear
        // ============================================================
        function clear(mode) {
            mode = mode || 'manual';
            if (mode === 'manual') {
                $('#manualAttendance-form')[0].reset();
                $(".selectpickermanualAttendance").val(null).trigger("change");
            }

            _empRemoveCopyIcon();
            disableExitTime();
            $('.ExitTimeLabel').show();
            $('#ISBothInOutEntry').prop({ checked: false, disabled: true });
            $("#AttendanceTypeCodeTwo").prop("disabled", true);
            $('#AutoId').val('0');
            $('.datepicker').val('');
            $('.text-danger').text('');
            $('#EntryTime').css('border', '');
            $('#AttendanceTypeCode').css('border', '');
            $('#ExitTime').css('border', '');
            $('#DateFrom').css('border', '');
            $('#DateTo').css('border', '');
            $('#DesignationNameShow').text('');
            $('#EmployeeFullNameShow').text('');
            $('#DepartmentNameShow').text('');
            $("#CompanyCode").val("001").trigger("change");
            getDate();
            dataTable();
        }

        // ============================================================
        // Helper
        // ============================================================
        function convertToISODate(dateStr) {
            if (!dateStr) return null;
            const parts = dateStr.split('/');
            if (parts.length === 3) return `${parts[2]}-${parts[1]}-${parts[0]}`;
            return null;
        }

        function convertTo24Hour(timeStr) {
            if (!timeStr) return null;
            if (/^\d{2}:\d{2}:\d{2}$/.test(timeStr)) return timeStr;
            const match = timeStr.match(/(\d{1,2}):(\d{2}):(\d{2})\s*(AM|PM)/i);
            if (match) {
                let hours = parseInt(match[1]);
                const minutes = match[2];
                const seconds = match[3];
                const period = match[4].toUpperCase();
                if (period === 'PM' && hours !== 12) hours += 12;
                else if (period === 'AM' && hours === 12) hours = 0;
                return `${String(hours).padStart(2, '0')}:${minutes}:${seconds}`;
            }
            return timeStr;
        }

        // ============================================================
        // DataTable
        // ============================================================
        function dataTable() {
            if ($.fn.DataTable.isDataTable('#manualAttendanceTable'))
                $('#manualAttendanceTable').DataTable().destroy();
            $('#manualAttendanceTable').DataTable({
                processing: true, serverSide: true, responsive: true,
                pageLength: 10, lengthMenu: [10, 25, 50, 100],
                ajax: { url: gridUrl, type: 'POST' },
                columns: [
                    { data: 'manualCode', render: function (data) { return `<input type="checkbox" class="checkBox" value="${data}" />`; }, orderable: false, className: "text-center" },
                    { data: 'manualCode', className: "text-center" },
                    { data: 'employeeId', className: "text-center" },
                    { data: 'employeeFullName' },
                    { data: 'attendanceTypeName', className: "text-center" },
                    { data: 'dateFrom', className: "text-center" },
                    { data: 'showEntryTime', className: "text-center" },
                    { data: 'remarks' },
                    { data: 'luser', className: "text-center" }
                ],
                order: [[1, "desc"]],
                language: { emptyTable: "No data available!" }
            });
        }

        function initDataTable(selectedEmployee) {
            return $('#manualAttendanceTable').DataTable({
                processing: true, serverSide: true, responsive: true,
                pageLength: 10, lengthMenu: [10, 25, 50, 100],
                ajax: {
                    url: '/ManualAttendance/GetEmployeeTableDataById',
                    type: 'POST',
                    data: function (d) { d.employeeId = selectedEmployee; }
                },
                columns: [
                    { data: 'manualCode', render: function (data) { return `<input type="checkbox" class="checkBox" value="${data}" />`; }, orderable: false, className: "text-center" },
                    { data: 'manualCode', className: "text-center" },
                    { data: 'employeeId', className: "text-center" },
                    { data: 'employeeFullName' },
                    { data: 'attendanceTypeName', className: "text-center" },
                    { data: 'dateFrom', className: "text-center" },
                    { data: 'showEntryTime', className: "text-center" },
                    { data: 'remarks' },
                    { data: 'luser', className: "text-center" }
                ],
                order: [[1, "desc"]],
                language: { emptyTable: "No data available!" }
            });
        }

        // ============================================================
        // Validations
        // ============================================================
        function validateCompanyDD() {
            var $el = $('#CompanyCode');
            var hasVal = $el.val() && $el.val().trim() !== '';
            $el.next('.select2-container').css({ 'border': hasVal ? '' : '1px solid red', 'border-radius': hasVal ? '' : '5px' });
        }
        function validateEmployeeDD() {
            var $el = $('#EmployeeId');
            var hasVal = $el.val() && $el.val().trim() !== '';
            $el.next('.select2-container').css({ 'border': hasVal ? '' : '1px solid red', 'border-radius': hasVal ? '' : '5px' });
        }
        function validateAttendanceTypeDD() {
            var $el = $('#AttendanceTypeCode');
            var hasVal = $el.val() && $el.val().trim() !== '';
            $el.next('.select2-container').css({ 'border': hasVal ? '' : '1px solid red', 'border-radius': hasVal ? '' : '5px' });
        }
        function validateDateFrom() {
            $('#DateFrom').css('border', $('#DateFrom').val().trim() === '' ? '1px solid red' : '');
        }
        function validateDateTo() {
            $('#DateTo').css('border', $('#DateTo').val().trim() === '' ? '1px solid red' : '');
        }
        function validateEntryTime() {
            $('#EntryTime').css('border', $('#EntryTime').val().trim() === '' ? '1px solid red' : '');
        }
        function validateExitTime() {
            $('#ExitTime').css('border', $('#ExitTime').val().trim() === '' ? '1px solid red' : '');
        }

        // ============================================================
        // Main DOM Ready
        // ============================================================
        $(() => {
            initialize();

            // init EntryTime picker
            initTimePicker('EntryTime');

            // ExitTime — init disabled 
            initTimePicker('ExitTime', new Date(), false);
            $('#ExitTime').prop('disabled', true);
            const exitEl = document.getElementById('ExitTime');
            if (exitEl && exitEl._flatpickr && exitEl._flatpickr.calendarContainer) {
                exitEl._flatpickr.calendarContainer.classList.add('fp-ui-disabled');
            }

            $('.ExitTimeLabel').show();
            $('#ISBothInOutEntry').prop({ checked: false, disabled: true });

            $('#DateFrom').val('');
            $('#DateTo').val('');
            $('#EntryTime').val('');
            $('#ExitTime').val('');

            dataTable();

            // ------------------------------------------------
            // Save button
            // ------------------------------------------------
            $(settings.saveSelector).on('click', function () {
                var companyDD = $('#CompanyCode').val();
                var employeeDD = $('#EmployeeId').val();
                var attendanceTypeDD = $('#AttendanceTypeCode').val();
                var dateFrom = $('#DateFrom').val();
                var dateTo = $('#DateTo').val();
                var entryTime = $('#EntryTime').val();

                validateCompanyDD();
                validateEmployeeDD();
                validateAttendanceTypeDD();
                validateDateFrom();
                validateDateTo();
                validateEntryTime();

                if (!companyDD) { toastr.warning('Please select company!'); $('#CompanyCode').select2('open'); return false; }
                if (!employeeDD) { toastr.warning('Please select employee!'); $('#EmployeeId').select2('open'); return false; }
                if (!attendanceTypeDD) { toastr.warning('Please select attendance type!'); $('#AttendanceTypeCode').select2('open'); return false; }
                if (!dateFrom) { toastr.warning('Please select from date!'); $('#DateFrom').datepicker('show'); return false; }
                if (!dateTo) { toastr.warning('Please select to date!'); $('#DateTo').datepicker('show'); return false; }
                if (!entryTime) { toastr.warning('Please select entry time!'); return false; }

                $(settings.formSelector).submit();
            });

            // ------------------------------------------------
            // Form submit
            // ------------------------------------------------
            $('body').on('submit', settings.formSelector, function (e) {
                e.preventDefault();

                let entryTime = $('#EntryTime').val();
                let exitTime = $('#ExitTime').val();
                const dateFrom = $('#DateFrom').val();
                const dateTo = $('#DateTo').val();
                const dateFromISO = convertToISODate(dateFrom);
                const dateToISO = convertToISODate(dateTo);

                entryTime = convertTo24Hour(entryTime);
                exitTime = convertTo24Hour(exitTime);

                if (entryTime && dateFromISO) $('#EntryTimeHidden').val(`${dateFromISO} ${entryTime}`);
                if (exitTime && dateToISO) $('#ExitTimeHidden').val(`${dateToISO} ${exitTime}`);

                $("#AttendanceTypeCodeTwo").prop("disabled", false);
                const formData = new FormData(this);
                const attendanceTypeCode = formData.get("AttendanceTypeCode");
                if (attendanceTypeCode === "3") formData.set("AttendanceTypeName", "On Tour");

                $.ajax({
                    type: 'POST',
                    url: $(this).attr('action'),
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        if (result.noSavePermission || result.noUpdatePermission || result.isDuplicate) {
                            toastr.error(result.message);
                        } else if (result.isSuccess) {
                            $(settings.gridContainer).html(result.html);
                            clear('save');
                            toastr.success(result.message, 'Success');
                            if (result.result?.companyCode) {
                                $("#CompanyCode").val(result.result.companyCode);
                                loadEmployeesByCompany(result.result.companyCode, result.result.employeeId);
                            }
                            if (result.result?.attendanceTypeCode) {
                                $("#AttendanceTypeCode").val(result.result.attendanceTypeCode).trigger('change');
                            }
                        } else {
                            $(settings.formSelector).html(result);
                        }
                    },
                    error: function () { toastr.error('Failed to save data.'); }
                });
            });

            // ------------------------------------------------
            // Company change
            // ------------------------------------------------
            $('#CompanyCode').on('change', function () {
                var selectedCompany = $(this).val();
                if (!selectedCompany) return;
                $.ajax({
                    url: '/ManualAttendance/GetCompanyTableDataById',
                    type: 'GET',
                    data: { companyId: selectedCompany },
                    success: function (data) {
                        $(settings.gridContainer).html(data);
                        dataTable();
                        loadEmployeesByCompany(selectedCompany);
                    }
                });
            });

            // ------------------------------------------------
            // Clear button
            // ------------------------------------------------
            $('.js-manualAttendance-clear').on('click', function () { clear('manual'); });

            // ------------------------------------------------
            // AttendanceTypeCode change
            // ------------------------------------------------
            $('#AttendanceTypeCode').on('change keyup', function () {
                const selectedValue = $(this).val();

                $('#AttendanceTypeCodeTwo').val('').trigger('change');
                $('#ISBothInOutEntry').prop({ checked: false, disabled: true });

                disableExitTime();
                $('.ExitTimeLabel').show();

                if (selectedValue === "1") {
                    $('#AttendanceTypeCodeTwo').val('2').trigger('change');
                    $('#ISBothInOutEntry').prop('disabled', false);
                    initTimePicker("EntryTime", new Date(), false);
                } else if (selectedValue === "3") {
                    $('#AttendanceTypeCodeTwo').val('3').trigger('change');
                    $('#ISBothInOutEntry').prop('disabled', false);
                } else {
                    initTimePicker("EntryTime", new Date(), false);
                }
            });

            // ------------------------------------------------
            // ISBothInOutEntry checkbox
            // ------------------------------------------------
            $('#ISBothInOutEntry').on('change', function () {
                if ($(this).is(':checked')) {
                    enableExitTime();
                } else {
                    disableExitTime();
                }
            });

            // ------------------------------------------------
            // DateFrom / EmployeeId change (On Tour shift time)
            // ------------------------------------------------
            $(document).on('change', '#DateFrom, #EmployeeId', function () {
                var employeeId = $("#EmployeeId").val();
                var ontourId = $("#AttendanceTypeCode").val();

                if (ontourId !== "3") {
                    initTimePicker("EntryTime", new Date(), false);
                    return;
                }

                var formdate = $("#DateFrom").val();
                const dateFromISO = convertToISODate(formdate);

                if (employeeId && formdate) {
                    $.ajax({
                        url: '/ManualAttendance/SandRTimeByEmployee',
                        type: "POST",
                        contentType: 'application/json',
                        data: JSON.stringify({ EmployeeId: employeeId, FromDate: dateFromISO }),
                        success: function (res) {
                            var data = res.result;
                            initTimePicker("EntryTime", data.showShiftStartTime, true);
                            if ($('#ISBothInOutEntry').is(':checked')) {
                                initTimePicker("ExitTime", data.showShiftEndTime, true);
                            }
                        },
                        error: function (e) { console.log(e); }
                    });
                }
            });

            // ------------------------------------------------
            // Validation bindings
            // ------------------------------------------------
            $('#AttendanceTypeCode').on('change blur', validateAttendanceTypeDD);
            $('#DateFrom').on('change blur', validateDateFrom);
            $('#DateTo').on('change blur', validateDateTo);
            $('#EntryTime').on('change blur', validateEntryTime);
            $('#ExitTime').on('change blur', validateExitTime);

            // ------------------------------------------------
            // EmployeeId change
            // ------------------------------------------------
            $('#EmployeeId').on('change', function () {
                var selectedEmployee = $(this).val();
                if (!selectedEmployee) {
                    if ($.fn.DataTable.isDataTable('#manualAttendanceTable'))
                        $('#manualAttendanceTable').DataTable().destroy();
                    dataTable();
                    $('#DesignationNameShow, #DepartmentNameShow, #EmployeeFullNameShow').text('');
                    _empRemoveCopyIcon();
                    return;
                }
                validateEmployeeDD();
                if ($.fn.DataTable.isDataTable('#manualAttendanceTable'))
                    $('#manualAttendanceTable').DataTable().destroy();
                initDataTable(selectedEmployee);

                $.ajax({
                    url: '/ManualAttendance/GetEmployeeDetailsById',
                    type: 'GET',
                    data: { id: selectedEmployee },
                    success: function (data) {
                        if (data) {
                            $('#DesignationNameShow').text(data.designationName || '');
                            $('#DepartmentNameShow').text(data.departmentName || '');
                            $('#EmployeeFullNameShow').text(data.employeeFullName || '');
                        } else {
                            $('#DesignationNameShow, #DepartmentNameShow, #EmployeeFullNameShow').text('');
                        }
                    },
                    error: function () { toastr.error('Failed to load employee details'); }
                });
            });

            // ------------------------------------------------
            // Delete
            // ------------------------------------------------
            $("body").on('click', settings.selectAllSelector, function () {
                $('.checkBox').prop('checked', $(this).prop('checked'));
            });

            $("body").on('click', settings.deleteSelector, function (e) {
                e.preventDefault();
                var selectedIds = [];
                var selectedEmployeeIds = [];
                $('.checkBox:checked').each(function () { selectedIds.push($(this).val()); });
                var selectedEmp = $("#EmployeeId").val();
                if (selectedEmp) selectedEmployeeIds.push(selectedEmp);

                if (selectedIds.length === 0 && selectedEmployeeIds.length === 0) {
                    toastr.error('Please select records to delete.');
                    return;
                }
                $(settings.deleteModal + ' ' + settings.finalDeleteSelector)
                    .data('ids', selectedIds)
                    .data('selectedEmployeeIds', selectedEmployeeIds)
                    .data('attendanceTypeCode', $('#AttendanceTypeCode').val())
                    .data('fromDate', $('#DateFrom').val())
                    .data('toDate', $('#DateTo').val());
                $(settings.deleteModal).modal('show');
            });

            //$("body").on('click', settings.finalDeleteSelector, function (e) {
            //    e.preventDefault();
            //    $.ajax({
            //        type: 'POST',
            //        url: '/ManualAttendance/Delete',
            //        contentType: 'application/json',
            //        data: JSON.stringify({
            //            ids: $(this).data('ids'),
            //            selectedEmployeeIds: $(this).data('selectedEmployeeIds'),
            //            attendanceTypeCode: $(this).data('attendanceTypeCode'),
            //            fromDate: $(this).data('fromDate'),
            //            toDate: $(this).data('toDate')
            //        }),
            //        success: function (result) {
            //            if (result.isSuccess) {
            //                clear('delete');
            //                toastr.success(result.message);
            //                if (result.deletedRecord?.companyCode) {
            //                    $("#CompanyCode").val(result.deletedRecord.companyCode);
            //                    loadEmployeesByCompany(result.deletedRecord.companyCode, result.deletedRecord.employeeId);
            //                }
            //                if (result.deletedRecord?.attendanceTypeCode)
            //                    $("#AttendanceTypeCode").val(result.deletedRecord.attendanceTypeCode).trigger('change');
            //                $('.checkBox, .EmployeeListCheckBox').prop('checked', false);
            //            } else {
            //                toastr.error(result.message, 'Error');
            //            }
            //            $(settings.deleteModal).modal('hide');
            //        },
            //        error: function () {
            //            toastr.error('An error occurred while deleting.');
            //            $(settings.deleteModal).modal('hide');
            //        }
            //    });
            //});

            $("body").on('click', settings.finalDeleteSelector, function (e) {
                e.preventDefault();

                $.ajax({
                    type: 'POST',
                    url: '/ManualAttendance/Delete',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        ids: $(this).data('ids'),
                        selectedEmployeeIds: $(this).data('selectedEmployeeIds'),
                        attendanceTypeCode: $(this).data('attendanceTypeCode'),
                        fromDate: $(this).data('fromDate'),
                        toDate: $(this).data('toDate'),
                        isBothInOutEntry: $('#ISBothInOutEntry').is(':checked') // ✅ checkbox থেকে value
                    }),
                    success: function (result) {
                        if (result.isSuccess) {
                            clear('delete');
                            toastr.success(result.message);
                            if (result.deletedRecord?.companyCode) {
                                $("#CompanyCode").val(result.deletedRecord.companyCode);
                                loadEmployeesByCompany(result.deletedRecord.companyCode, result.deletedRecord.employeeId);
                            }
                            if (result.deletedRecord?.attendanceTypeCode)
                                $("#AttendanceTypeCode").val(result.deletedRecord.attendanceTypeCode).trigger('change');
                            $('.checkBox, .EmployeeListCheckBox').prop('checked', false);
                        } else {
                            toastr.error(result.message, 'Error');
                        }
                        $(settings.deleteModal).modal('hide');
                    },
                    error: function () {
                        toastr.error('An error occurred while deleting.');
                        $(settings.deleteModal).modal('hide');
                    }
                });
            });

        }); // end $()

        function getDate() {
            $('.datepicker').datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: '1950:2050'
            }).datepicker('setDate', new Date());
        }

        // ============================================================
        // Initialize
        // ============================================================
        function initialize() {
            $('.selectpickermanualAttendance').select2({
                placeholder: "Select Option",
                allowClear: true,
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
                //$('.datepicker').datepicker({
                //    dateFormat: 'dd/mm/yy',
                //    changeMonth: true,
                //    changeYear: true,
                //    yearRange: '1950:2050'
                //}).datepicker('setDate', new Date());
                getDate();
            });

            empInit();

            var defaultCompany = $("#CompanyCode").val();
            if (defaultCompany) {
                loadEmployeesByCompany(defaultCompany);
            }
        }

    }
}(jQuery));