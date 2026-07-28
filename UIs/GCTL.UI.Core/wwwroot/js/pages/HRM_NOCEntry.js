(function ($) {
    $.HRM_NOCEntry = function (options) {

        var settings = $.extend({
            baseUrl: "/",
            EmployeeSelect: "#employeeSelect",
            SCEmployeeSelect: "#sCEmployeeSelect",
            load: function () { }
        }, options);

        var empDetailsUrl = settings.baseUrl + "/EmpDetails";
        var saveUrl = settings.baseUrl + "/Save";
        var updateUrl = settings.baseUrl + "/Update";
        var deleteUrl = settings.baseUrl + "/Delete";       
        var getByIdUrl = settings.baseUrl + "/GetById";
        var getNewNocIdUrl = settings.baseUrl + "/GetNewNocId";
        var getListUrl = settings.baseUrl + "/GetList";

        var EMP_SELECTOR = '#employeeSelect';
        var _currentAutoId = null;
        var _currentNocType = "travel";
        var _dtNoc = null;

        // ─────────────────────────────────────────────────────────────────
        // INIT
        // ─────────────────────────────────────────────────────────────────
        $(document).ready(async function () {
            s2_InitSingle("#companySelect", "/GcFilters/company", "Select Company", "company");
            s2_InitSingle("#employeeSelect", "/GcFilters/employee", "Select Employee", "employee");

            $('[data-toggle="tooltip"]').tooltip();
            await s2_AutoSelectCompany("001");

            flatpickr("#iDateFrom", CalendarService.createConfig({ defaultDate: new Date() }));
            flatpickr("#iDateTo", CalendarService.createConfig({ defaultDate: new Date() }));



            _currentNocType = $("#nocTypeSelect").val() || "travel";
            _toggleNocSections(_currentNocType);
            _loadNewNocId();
            _initGrid(_currentNocType);

            settings.load();
        });
       
        // ─────────────────────────────────────────────────────────────────
        // NOC TYPE TOGGLE
        // ─────────────────────────────────────────────────────────────────
        $(document).on("change", "#nocTypeSelect", function () {
            _currentNocType = $(this).val();
            _toggleNocSections(_currentNocType);
            _initGrid(_currentNocType);
        });

        function _toggleNocSections(type) {
            if (type === "travel") {
                $("#section-travel").show();
                $("#section-travel-passport").show();
                $("#section-education").hide();
            } else {
                $("#section-travel").hide();
                $("#section-travel-passport").hide();
                $("#section-education").show();
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // THEAD
        // ─────────────────────────────────────────────────────────────────
        function _buildThead(nocType) {
            var chk = '<th style="width:40px;text-align:center;">' +
                '<input type="checkbox" id="chkSelectAll" title="Select All" /></th>';

            var travelHead =
                '<tr>' + chk +
                '<th>NOC ID</th><th>Employee ID</th><th>Name</th>' +
                '<th>Place of Visit</th><th>From Date</th><th>To Date</th><th>Remarks</th>' +
                '</tr>';

            var educationHead =
                '<tr>' + chk +
                '<th>NOC ID</th><th>Employee ID</th><th>Name</th>' +
                '<th>University Name</th><th>Course Name</th><th>Remarks</th>' +
                '</tr>';

            $("#tblNocHead").html(nocType === "travel" ? travelHead : educationHead);
        }

        // ─────────────────────────────────────────────────────────────────
        // DATATABLE
        // ─────────────────────────────────────────────────────────────────
        function _initGrid(nocType) {
            if (_dtNoc) { _dtNoc.destroy(); $("#tblNoc tbody").empty(); }
            _buildThead(nocType);

            _dtNoc = $('#tblNoc').DataTable({
                processing: true,
                serverSide: false,
                ajax: {
                    url: getListUrl,
                    type: 'GET',
                    data: function (d) { d.nocType = nocType; },
                    dataSrc: ''
                },
                columns: _buildColumns(nocType),
                order: [[1, 'asc']],
                pageLength: 10,
                lengthMenu: [5, 10, 25, 50],
                language: {
                    emptyTable: "No data available in table",
                    zeroRecords: "No matching records found",
                    info: "Showing _START_ to _END_ of _TOTAL_ entries",
                    infoEmpty: "Showing 0 to 0 of 0 entries",
                    infoFiltered: "(filtered from _MAX_ total entries)",
                    lengthMenu: "Show _MENU_ entries",
                    search: "Search:",
                    paginate: { first: "First", previous: "Previous", next: "Next", last: "Last" }
                },
                dom: '<"row"<"col-sm-4"l><"col-sm-8 text-right"f>>' +
                    '<"row"<"col-sm-12"tr>>' +
                    '<"row"<"col-sm-5"i><"col-sm-7 text-right"p>>',
                columnDefs: [{ targets: 0, className: 'text-center' }]
            });
        }

        function _buildColumns(nocType) {
            var cols = [
                {
                    data: null,
                    orderable: false,
                    searchable: false,
                    className: "text-center",
                    render: function (data, type, row) {
                        return '<input type="checkbox" class="noc-row-check" data-id="' + row.autoId + '" />';
                    }
                },
                {
                    data: 'nocId',
                    className: "text-center",
                    render: function (data, type, row) {
                        return '<a href="javascript:void(0)" class="noc-edit-link text-primary fw-bold" ' +
                            'style="text-decoration:underline;" data-id="' + row.autoId + '">' +
                            (data || '') + '</a>';
                    }
                },
                {
                    data: 'employeeID',
                    className: "text-center"
                },
                {
                    data: 'employeeName',
                    className: "text-start"
                }
            ];

            if (nocType === "travel") {
                cols.push(
                    {
                        data: 'placeofVisit',
                        defaultContent: '',
                        className: "text-center"
                    },
                    {
                        data: 'fromDate',
                        className: "text-center",
                        render: function (d) {
                            return _formatDate(d);
                        }
                    },
                    {
                        data: 'toDate',
                        className: "text-center",
                        render: function (d) {
                            return _formatDate(d);
                        }
                    },
                    {
                        data: 'remarks',
                        defaultContent: '',
                        className: "text-start"
                    }
                );
            } else {
                cols.push(
                    {
                        data: 'universityName',
                        defaultContent: '',
                        className: "text-center"
                    },
                    {
                        data: 'courseName',
                        defaultContent: '',
                        className: "text-center"
                    },
                    {
                        data: 'remarks',
                        defaultContent: '',
                        className: "text-start"
                    }
                );
            }

            return cols;
        }

        // ─────────────────────────────────────────────────────────────────
        // SELECT ALL CHECKBOX
        // ─────────────────────────────────────────────────────────────────
        $(document).on("change", "#chkSelectAll", function () {
            var checked = $(this).is(":checked");
            $("#tblNoc tbody .noc-row-check").prop("checked", checked);
            _syncDeleteBtn();
        });

        $(document).on("change", ".noc-row-check", function () {
            var total = $("#tblNoc tbody .noc-row-check").length;
            var checked = $("#tblNoc tbody .noc-row-check:checked").length;
            $("#chkSelectAll")
                .prop("indeterminate", checked > 0 && checked < total)
                .prop("checked", checked > 0 && checked === total);
            _syncDeleteBtn();
        });



        function _syncDeleteBtn() {
            var count = $("#tblNoc tbody .noc-row-check:checked").length;
            if (count > 0) {
                // Checkbox mode: bulk delete label
                $("#btnDelete")
                    .attr("title", "Delete Selected (" + count + ")")
                    .find("i").next().length
                    ? $("#btnDelete").find("i").next().text(" Delete (" + count + ")")
                    : $("#btnDelete").html('<i class="fa fa-trash">&nbsp;</i> Delete (' + count + ')');
            } else {
                // Single / no selection mode: normal label
                $("#btnDelete")
                    .attr("title", "Delete")
                    .html('<i class="fa fa-trash">&nbsp;</i> Delete');
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // GRID ROW CLICK — form 
        // ─────────────────────────────────────────────────────────────────
        $(document).on("click", ".noc-edit-link", function () {
            // ১. checkbox uncheck
            _clearAllCheckboxes();

            // ২. Form record load 
            _loadRecord($(this).data("id"));
            $("html, body").animate({ scrollTop: $(".panel-box").offset().top - 20 }, 300);
        });

        function _clearAllCheckboxes() {
            $("#tblNoc tbody .noc-row-check").prop("checked", false);
            $("#chkSelectAll").prop("checked", false).prop("indeterminate", false);
            _syncDeleteBtn();
        }

        // ─────────────────────────────────────────────────────────────────
        // UNIFIED DELETE BUTTON
       
        // ─────────────────────────────────────────────────────────────────
        $(document).on("click", "#btnDelete", function () {
            var checkedIds = [];
            $("#tblNoc tbody .noc-row-check:checked").each(function () {
                checkedIds.push(parseInt($(this).data("id")));
            });

            if (checkedIds.length > 0) {
                // ── BULK DELETE ──────────────────────────────────────────
                if (!confirm("Are you sure you want to delete " + checkedIds.length + " selected record(s)?")) return;

                $.ajax({
                    url: deleteUrl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(checkedIds),        
                    beforeSend: function () { _toggleBtns(true); },
                    success: function () {
                        toastr.success(checkedIds.length + " record(s) deleted successfully.");
                       
                        if (_currentAutoId && checkedIds.indexOf(_currentAutoId) !== -1) {
                            _resetForm();
                        } else {
                            _reloadGrids();
                        }
                        _clearAllCheckboxes();
                    },
                    error: function (xhr) { toastr.error(xhr.responseJSON?.message || "Delete failed."); },
                    complete: function () { _toggleBtns(false); }
                });

            } else if (_currentAutoId) {
                // ── SINGLE DELETE ────────────────────────────────────────
                if (!confirm("Are you sure you want to delete this NOC record?")) return;

                $.ajax({
                    url: deleteUrl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify([_currentAutoId]),    // single item array
                    beforeSend: function () { _toggleBtns(true); },
                    success: function () {
                        toastr.success("NOC deleted successfully.");
                        _resetForm();
                        _reloadGrids();
                    },
                    error: function (xhr) { toastr.error(xhr.responseJSON?.message || "Delete failed."); },
                    complete: function () { _toggleBtns(false); }
                });

            } else {
                toastr.warning("Please select a record or check rows to delete.");
            }
        });

        // ─────────────────────────────────────────────────────────────────
        // EMPLOYEE SELECT CHANGE
        // ─────────────────────────────────────────────────────────────────
        $(document).on('change', settings.EmployeeSelect + ', ' + settings.SCEmployeeSelect, function () {
            var value = $(this).val();
            var id = this.id;
            if (!value) { _clearEmpFields(id); return; }
            _empDetails(value, id);
        });

        function _setValue(selector, value) {
            var el = $(selector);
            el.is("input") ? el.val(value || "") : el.text(value || "");
        }

        function _clearEmpFields(sourceId) {
            if (sourceId === "employeeSelect") {
                $("#iDept, #iJoin, #iNid, #iName, #iDesig, #iSrv, #iGross").text("");
                $("#passNo, #passDob, #passPob, #passPoI, #passDoI, #passExp").text("");
            }
        }

        function _empDetails(empId, sourceId) {
            $.ajax({
                url: empDetailsUrl,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(empId),
                success: function (res) {
                    if (sourceId === "employeeSelect") {
                        _setValue("#iDept", res.departmentName);
                        _setValue("#iJoin", res.joiningDate);
                        _setValue("#iNid", res.nationalIDNO);
                        _setValue("#iName", res.employeeName);
                        _setValue("#iDesig", res.designationName);
                        _setValue("#iSrv", res.serviceLength);
                        _setValue("#iGross", res.grossSalary ? res.grossSalary.toLocaleString() : "");
                        _setValue("#passNo", res.passportNo);
                        _setValue("#passDob", res.dateOfBirthOrginal);
                        _setValue("#passPob", res.placeOfBirth);
                        _setValue("#passPoI", "");
                        _setValue("#passDoI", "");
                        _setValue("#passExp", res.passportExpiryDate);
                        // _empAddCopyIcon($(EMP_SELECTOR));
                    }
                },
                error: function () { toastr.error("Failed to load employee details."); }
            });
        }

        // ─────────────────────────────────────────────────────────────────
        // SELECT2 SAFE SETTER
        // ─────────────────────────────────────────────────────────────────
        function _s2SetValue(selector, value, label) {
            var $el = $(selector);
            if (!$el.length) return;
            var displayText = label ? label : value;
            var $existing = $el.find('option[value="' + value + '"]');
            if ($existing.length) {
                $existing.text(displayText);
                $el.val(value).trigger("change");
                return;
            }
            $el.append(new Option(displayText, value, true, true)).trigger("change");
        }

        // ─────────────────────────────────────────────────────────────────
        // EMPLOYEE LOCK / UNLOCK
        // ─────────────────────────────────────────────────────────────────
        function _lockEmployeeSelect() {
            $(EMP_SELECTOR).next('.select2-container')
                .css({ 'pointer-events': 'none', 'opacity': '0.75' });
            $(EMP_SELECTOR).prop('disabled', true);
        }

        function _unlockEmployeeSelect() {
            $(EMP_SELECTOR).next('.select2-container')
                .css({ 'pointer-events': '', 'opacity': '' });
            $(EMP_SELECTOR).prop('disabled', false);
        }

        // ─────────────────────────────────────────────────────────────────
        // LOAD NEW NOC ID
        // ─────────────────────────────────────────────────────────────────
        function _loadNewNocId() {
            $.ajax({
                url: getNewNocIdUrl, type: 'GET',
                success: function (res) {
                    $("#iNocId").val(res.nocId || "");
                    $("#iNocId_edu").val(res.nocId || "");
                }
            });
        }

        // ─────────────────────────────────────────────────────────────────
        // COLLECT FORM DATA
        // ─────────────────────────────────────────────────────────────────
        function _collectRequest() {
            var nocType = $("#nocTypeSelect").val();
            return {
                autoId: _currentAutoId,
                nocId: $("#iNocId").val() || $("#iNocId_edu").val(),
                nocTypeId: nocType,
                employeeID: $("#employeeSelect").val(),
                placeofVisit: nocType === "travel" ? $("#iPlaceOfVisit").val() : null,
                fromDate: nocType === "travel" ? $("#iDateFrom").val() : null,
                toDate: nocType === "travel" ? $("#iDateTo").val() : null,
                universityName: nocType === "education" ? $("#iUniversityName").val() : null,
                courseName: nocType === "education" ? $("#iCourseName").val() : null,
                remarks: nocType === "travel" ? $("#iRemarks").val()
                    : $("#iRemarks_edu").val()
            };
        }

        // ─────────────────────────────────────────────────────────────────
        // VALIDATE
        // ─────────────────────────────────────────────────────────────────
        function _validate(req) {
            if (!req.employeeID) {
                toastr.warning("Please select an Employee.");
                $("#employeeSelect").select2("open");
                return false;
            }
            if (!req.nocTypeId) { toastr.warning("Please select NOC Type."); return false; }
            if (req.nocTypeId === "travel") {
                if (!req.placeofVisit) { toastr.warning("Please enter Place of Visit."); $("#iPlaceOfVisit").focus(); return false; }
                if (!req.fromDate) { toastr.warning("Please select Date From."); return false; }
                if (!req.toDate) { toastr.warning("Please select Date To."); return false; }
            }
            if (req.nocTypeId === "education") {
                if (!req.universityName) { toastr.warning("Please enter University Name."); $("#iUniversityName").focus(); return false; }
                if (!req.courseName) { toastr.warning("Please enter Course Name."); $("#iCourseName").focus(); return false; }
            }
            return true;
        }

        // ─────────────────────────────────────────────────────────────────
        // SAVE
        // ─────────────────────────────────────────────────────────────────
        $(document).on("click", "#btnSave", function () {
            var req = _collectRequest();
            if (!_validate(req)) return;
            $.ajax({
                url: saveUrl, type: "POST", contentType: "application/json",
                data: JSON.stringify(req),
                beforeSend: function () { _toggleBtns(true); },
                success: function (res) {
                    toastr.success("NOC saved successfully.");
                    //_currentAutoId = res.autoId;
                    //$("#iNocId").val(res.nocId);
                    //$("#iNocId_edu").val(res.nocId);
                    //_setCreationDate(res.lDate);
                    //_switchToEditMode();
                    _resetForm();
                    _reloadGrids();
                },
                error: function (xhr) { toastr.error(xhr.responseJSON?.message || "Save failed."); },
                complete: function () { _toggleBtns(false); }
            });
        });

        // ─────────────────────────────────────────────────────────────────
        // UPDATE
        // ─────────────────────────────────────────────────────────────────
        $(document).on("click", "#btnUpdate", function () {
            if (!_currentAutoId) { toastr.warning("Nothing to update."); return; }
            var req = _collectRequest();
            if (!_validate(req)) return;
            $.ajax({
                url: updateUrl, type: "POST", contentType: "application/json",
                data: JSON.stringify(req),
                beforeSend: function () { _toggleBtns(true); },
                success: function (res) {
                    toastr.success("NOC updated successfully.");
                    _setModifyDate(res.modifyDate);
                    _reloadGrids();
                },
                error: function (xhr) { toastr.error(xhr.responseJSON?.message || "Update failed."); },
                complete: function () { _toggleBtns(false); }
            });
        });

        // ─────────────────────────────────────────────────────────────────
        // NEW / CLEAR
        // ─────────────────────────────────────────────────────────────────
        $(document).on("click", "#js-noc-entry-clear", function () { _resetForm(); });

        // ─────────────────────────────────────────────────────────────────
        // LOAD RECORD INTO FORM
        // ─────────────────────────────────────────────────────────────────
        function _loadRecord(autoId) {
            $.ajax({
                url: getByIdUrl, type: 'GET', data: { autoId: autoId },
                success: function (res) {
                    _currentAutoId = res.autoId;

                    if (res.companyCode) _s2SetValue("#companySelect", res.companyCode);
                    $("#nocTypeSelect").val(res.nocTypeId).trigger("change");

                    if (res.employeeID) {
                        $.ajax({
                            url: empDetailsUrl,
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: JSON.stringify(res.employeeID),
                            success: function (emp) {
                                var label = emp.employeeName
                                    ? emp.employeeName + " (" + res.employeeID + ")"
                                    : res.employeeID;
                                _s2SetValue("#employeeSelect", res.employeeID, label);
                                _setValue("#iDept", emp.departmentName);
                                _setValue("#iJoin", emp.joiningDate);
                                _setValue("#iNid", emp.nationalIDNO);
                                _setValue("#iName", emp.employeeName);
                                _setValue("#iDesig", emp.designationName);
                                _setValue("#iSrv", emp.serviceLength);
                                _setValue("#iGross", emp.grossSalary ? emp.grossSalary.toLocaleString() : "");
                                _setValue("#passNo", emp.passportNo);
                                _setValue("#passDob", emp.dateOfBirthOrginal);
                                _setValue("#passPob", emp.placeOfBirth);
                                _setValue("#passPoI", "");
                                _setValue("#passDoI", "");
                                _setValue("#passExp", emp.passportExpiryDate);                               

                                _lockEmployeeSelect();
                                // _empAddCopyIcon($(EMP_SELECTOR));
                            },
                            error: function () {
                                _s2SetValue("#employeeSelect", res.employeeID, res.employeeID);
                                _lockEmployeeSelect();
                            }
                        });
                    }

                    $("#iNocId").val(res.nocid || res.nocId || "");
                    $("#iNocId_edu").val(res.nocid || res.nocId || "");
                    $("#iPlaceOfVisit").val(res.placeofVisit || "");
                    if (res.fromDate && $("#iDateFrom")[0]?._flatpickr)
                        $("#iDateFrom")[0]._flatpickr.setDate(res.fromDate);
                    if (res.toDate && $("#iDateTo")[0]?._flatpickr)
                        $("#iDateTo")[0]._flatpickr.setDate(res.toDate);
                    $("#iUniversityName").val(res.universityName || "");
                    $("#iCourseName").val(res.courseName || "");
                    $("#iRemarks").val(res.remarks || "");
                    $("#iRemarks_edu").val(res.remarks || "");
                    _setCreationDate(res.ldate);
                    _setModifyDate(res.modifyDate);
                    _switchToEditMode();
                },
                error: function () { toastr.error("Failed to load NOC record."); }
            });
        }

        // ─────────────────────────────────────────────────────────────────
        // RELOAD GRID
        // ─────────────────────────────────────────────────────────────────
        function _reloadGrids() {
            if (_dtNoc) _dtNoc.ajax.reload(null, false);
        }

        // ─────────────────────────────────────────────────────────────────
        // RESET FORM
        // ─────────────────────────────────────────────────────────────────
        function _resetForm() {
            _currentAutoId = null;
            _unlockEmployeeSelect();
            $(EMP_SELECTOR).val(null).trigger("change");
            $(EMP_SELECTOR).find('option:not([value=""])').remove();
            _clearEmpFields("employeeSelect");
            $("#nocTypeSelect").val("travel").trigger("change");
            $("#iPlaceOfVisit").val("");
            if ($("#iDateFrom")[0]?._flatpickr) $("#iDateFrom")[0]._flatpickr.clear();
            if ($("#iDateTo")[0]?._flatpickr) $("#iDateTo")[0]._flatpickr.clear();
            $("#iUniversityName").val("");
            $("#iCourseName").val("");
            $("#iRemarks").val("");
            $("#iRemarks_edu").val("");
            $("#CreationDate").text("");
            $("#LastUpdateDate").text("");
            $('.emp-selected-copy').remove();
            _clearAllCheckboxes();
            _switchToNewMode();
            _loadNewNocId();
            flatpickr("#iDateFrom", CalendarService.createConfig({ defaultDate: new Date() }));
            flatpickr("#iDateTo", CalendarService.createConfig({ defaultDate: new Date() }));

        }

        // ─────────────────────────────────────────────────────────────────
        // MODE SWITCH
        // ─────────────────────────────────────────────────────────────────
        function _switchToNewMode() { $("#btnSave").show(); $("#btnUpdate").hide(); }
        function _switchToEditMode() { $("#btnSave").hide(); $("#btnUpdate").show(); }

        function _toggleBtns(disabled) {
            $("#btnSave, #btnUpdate, #btnDelete, #js-noc-entry-clear")
                .prop("disabled", disabled);
        }

        // ─────────────────────────────────────────────────────────────────
        // DATE HELPERS
        // ─────────────────────────────────────────────────────────────────
        function _setCreationDate(val) {
            if (!val) return;
            var d = new Date(val);
            $("#CreationDate").text(isNaN(d) ? val : d.toLocaleDateString("en-GB"));
        }

        function _setModifyDate(val) {
            if (!val) return;
            var d = new Date(val);
            $("#LastUpdateDate").text(isNaN(d) ? val : d.toLocaleDateString("en-GB"));
        }

        function _formatDate(val) {
            if (!val) return "";
            var d = new Date(val);
            return isNaN(d) ? val : d.toLocaleDateString("en-GB");
        }

        // ─────────────────────────────────────────────────────────────────
        // COPY ICON
        // ─────────────────────────────────────────────────────────────────
        function _empAddCopyIcon($sel) {
            $('.emp-selected-copy').remove();
            var selectedText = $sel.val() || '';
            if (!selectedText) return;
            var $container = $sel.next('.select2-container');
            if (!$container.length) return;
            var $icon = $(
                '<span class="emp-selected-copy" title="Copy ID" ' +
                'style="position:absolute;right:38px;top:50%;transform:translateY(-50%);' +
                'cursor:pointer;color:#888;font-size:11px;z-index:10;padding:2px 5px;' +
                'background:#f0f0f0;border-radius:3px;line-height:1;">' +
                '<i class="fa fa-copy"></i></span>'
            );
            $container.css('position', 'relative');
            $icon.on('click', function (e) {
                e.preventDefault(); e.stopPropagation();
                _empCopyToClipboard($(EMP_SELECTOR).val() || '');
            });
            $container.append($icon);
        }

        function _empCopyToClipboard(text) {
            if (!text) return;
            toastr.options = { positionClass: "toast-bottom-right" };
            try {
                var $temp = $('<input>');
                $('body').append($temp);
                $temp.val(text).select();
                document.execCommand('copy');
                $temp.remove();
                toastr.success('ID copied: ' + text);
            } catch (e) { toastr.error('Copy failed.'); }
        }

        // ─────────────────────────────────────────────────────────────────
        // PUBLIC API
        // ─────────────────────────────────────────────────────────────────
        return {
            loadRecord: _loadRecord,
            resetForm: _resetForm,
            reloadGrids: _reloadGrids
        };
    };
}(jQuery));