(function ($) {
    $.DashboardIndex = function (options) {

        var settings = $.extend({ baseUrl: "/" }, options);
        var chart = null;
        var dataTable = null;
        var leaveTable = null;
        var _leaveTypes = [];
        var DEFAULT_PHOTO = "/images/default-avatar.png";
        var _connection = null;
        var _reloadPending = false;

        // ════════════════════════════════════════════════════════
        // ── ATTENDANCE ──────────────────────────────────────────
        // ════════════════════════════════════════════════════════

        function getFilters() {
            return {
                companyCode: $("#companySelect").val() || "",
                branchCode: $("#branchSelect").val() || "",
                departmentCode: $("#departmentSelect").val() || ""
            };
        }

        function renderSummary(s) {
            if (!s) return;
            animateCount("#totalEmployee", s.totalEmployees);
            animateCount("#presentCount", s.presentCount);
            animateCount("#absentCount", s.absentCount);
            animateCount("#lateCount", s.lateCount);
            animateCount("#onLeaveCount", s.onLeaveCount);
            $("#presentPct").text(s.presentPct + "%");
            $("#absentPct").text(s.absentPct + "%");
            $("#latePct").text(s.latePct + "%");
            $("#onLeavePct").text(s.onLeavePct + "%");
            if (!s.isToday) {
                $("#dataDateNotice")
                    .html('<i class="fa fa-triangle-exclamation"></i> Today\'s data unavailable. Showing: <strong>' + s.dataDate + '</strong>')
                    .show();
                $("#chartDataDate").text("Data: " + s.dataDate);
            } else {
                $("#dataDateNotice").hide();
                $("#chartDataDate").text("");
            }
            updateChart(s);
        }

        function animateCount(selector, target) {
            var el = $(selector);
            var cur = parseInt(el.text()) || 0;
            if (cur === target) return;
            $({ val: cur }).animate({ val: target }, {
                duration: 600, easing: "swing",
                step: function () { el.text(Math.ceil(this.val)); },
                complete: function () { el.text(target); }
            });
        }

        function getPhotoSrc(row) {
            var src = (row && row.photoSrc) ? String(row.photoSrc).trim() : "";
            return src || DEFAULT_PHOTO;
        }

        // function statusBadge(status, row) {
        //     switch ((status || "").toLowerCase()) {
        //         case "present":
        //             return '<span class="status-badge" style="background:#4caf50;">Present</span>';
        //         case "late":
        //             var tip = (row && row.lateByMinutes > 0) ? lateTooltipText(row.lateByMinutes) : "";
        //             return '<span class="status-badge" style="background:#ff9800;cursor:help;" ' +
        //                 (tip ? 'data-bs-toggle="tooltip" data-bs-placement="top" title="' + tip + '"' : '') +
        //                 '>Late</span>';
        //         case "on leave":
        //             return '<span class="status-badge" style="background:#9c27b0;">On Leave</span>';
        //         default:
        //             return '<span class="status-badge" style="background:#f44336;">Absent</span>';
        //     }
        // }
        function statusBadge(status, row) {

            const badgeStyle = `
        width:55px;
        display:inline-block;
        text-align:center;
        padding:4px 8px;
        border-radius:4px;
        color:#fff;
        font-weight:500;
    `;

            switch ((status || "").toLowerCase()) {
                case "present":
                    return `<span class="status-badge" style="${badgeStyle};background:#4caf50;">Present</span>`;

                case "late":
                    var tip = (row && row.lateByMinutes > 0)
                        ? lateTooltipText(row.lateByMinutes)
                        : "";

                    return `<span class="status-badge"
                        style="${badgeStyle};background:#ffa400;cursor:help;"
                        ${tip ? `data-bs-toggle="tooltip" data-bs-placement="top" title="${tip}"` : ""}>
                        Late
                    </span>`;

                case "on leave":
                    return `<span class="status-badge" style="${badgeStyle};background:#9c27b0;">On Leave</span>`;

                default:
                    return `<span class="status-badge" style="${badgeStyle};background:#f55942;">Absent</span>`;
            }
        }
        function checkInColor(status) {
            switch ((status || "").toLowerCase()) {
                case "late": return "#ff9800";
                case "present": return "#4caf50";
                case "on leave": return "#9c27b0";
                default: return "#f44336";
            }
        }

        // ── Late duration tooltip text: "X hr Y min late" / "X hrs Y min late" ──
        function lateTooltipText(minutes) {
            if (!minutes || minutes <= 0) return "";
            var h = Math.floor(minutes / 60);
            var m = minutes % 60;
            var parts = [];
            if (h > 0) parts.push(h + (h === 1 ? " hr" : " hrs"));
            if (m > 0) parts.push(m + " min");
            return parts.join(" ") + "";
        }

        function initChart() {
            var canvas = document.getElementById("attendanceChart");
            if (!canvas || typeof Chart === "undefined") return;
            chart = new Chart(canvas.getContext("2d"), {
                type: "doughnut",
                data: {
                    labels: ["Present", "Absent", "Late", "On Leave"],
                    datasets: [{
                        data: [0, 0, 0, 0],
                        backgroundColor: ["#4caf50", "#f55942", "#ffa400", "#9c27b0"],
                        hoverOffset: 8, borderWidth: 2, borderColor: "#ffffff"
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false, 
                    cutout: "68%",
                    animation: { animateRotate: true, duration: 700 },
                    plugins: {
                        legend: { position: "bottom", labels: { boxWidth: 12, font: { size: 11 }, padding: 14 } },
                        tooltip: {
                            callbacks: {
                                label: function (ctx) {
                                    var t = ctx.dataset.data.reduce(function (a, b) { return a + b; }, 0);
                                    var pct = t > 0 ? ((ctx.raw / t) * 100).toFixed(1) : 0;
                                    return "  " + ctx.raw + " (" + pct + "%)";
                                }
                            }
                        }
                    }
                }
            });
        }

        function updateChart(s) {
            if (!chart) return;
            chart.data.datasets[0].data = [
                s.presentCount || 0, s.absentCount || 0,
                s.lateCount || 0, s.onLeaveCount || 0
            ];
            chart.update();
        }

        function initDataTable() {
            if ($.fn.DataTable.isDataTable("#attendanceTable")) {
                $("#attendanceTable").DataTable().destroy();
                $("#attendanceTable tbody").empty();
            }
            dataTable = $("#attendanceTable").DataTable({

                processing: true,
                serverSide: true,
                searching: true,
                ordering: false,

                responsive: true,
                autoWidth: false,

                pageLength: 5,
                lengthMenu: [[5, 10, 20, 50, 100], [5, 10, 20, 50, 100]],
                language: {
                    processing: '<span><i class="fa fa-spinner fa-spin"></i> Loading...</span>',
                    emptyTable: "No attendance data found",
                    zeroRecords: "No matching records",
                    info: "Showing _START_ to _END_ of _TOTAL_ employees",
                    infoEmpty: "No records", search: "Search:", lengthMenu: "Show _MENU_"
                },
                ajax: {
                    url: "/Dashboard/attendance-datatable",
                    type: "POST",
                    data: function (d) {
                        var f = getFilters();
                        d.companyCode = f.companyCode;
                        d.branchCode = f.branchCode;
                        d.departmentCode = f.departmentCode;
                        return d;
                    },
                    dataSrc: function (json) {
                        renderSummary(json.summary);
                        return json.data || [];
                    },
                    error: function (xhr, err) {
                        console.error("Attendance AJAX error:", err, xhr.responseText);
                    }
                },
                columns: [
                    {
                        data: "employeeId",
                        render: function (d) {
                            return '<span style="font-weight:600;font-size:12px;">' + (d || '') + '</span>';
                        }
                    },
                    {
                        // Name + Designation merged into single cell (image left, stacked name/designation)
                        data: "name",
                        render: function (d, t, row) {
                            var src = getPhotoSrc(row);
                            var designation = row && row.designation ? row.designation : '';
                            var nameHtml = '<div class="emp-profile">' +
                                '<img src="' + src + '" alt="" ' +
                                'onerror="this.onerror=null;this.src=\'' + DEFAULT_PHOTO + '\'" ' +
                                'style="width:30px;height:30px;border-radius:50%;object-fit:cover;">' +
                                '<div class="emp-meta">' +
                                '<span class="emp-name" title="' + (d || '') + '">' + (d || '') + '</span>' +
                                '<span class="emp-designation" title="' + designation + '">' + (designation || '—') + '</span>' +
                                '</div></div>';
                            return nameHtml;
                        }
                    },
                    {
                        data: "checkIn",
                        render: function (d, t, row) {
                            if (!d) {
                                var color = row.status === "On Leave" ? "#9c27b0" : "#f44336";
                                return '<span style="color:' + color + ';">—</span>';
                            }
                            return '<span style="color:' + checkInColor(row.status) + ';font-weight:600;">' + d + '</span>';
                        }
                    },
                    {
                        data: "checkOut",
                        render: function (d) {
                            if (!d) return '<span style="color:#94a3b8;">—</span>';
                            return '<span style="color:#4caf50;font-weight:600;">' + d + '</span>';
                        }
                    },
                    {
                        data: "status", className: "text-center",
                        render: function (d, t, row) { return statusBadge(d, row); }
                    },
                    {
                        // ── Movement column ──────────────────────────────────────
                        data: "movement",
                        render: function (d, t, row) {
                            if (!d || d.trim() === "") {
                                if (row.status === "On Leave")
                                    return '<span style="color:#9c27b0;font-size:12px;"><i class="fa fa-umbrella-beach"></i> On Leave</span>';
                                if (row.status === "Absent")
                                    return '<span style="color:#f44336;font-size:12px;"><i class="fa fa-circle-xmark"></i> Absent</span>';
                                return '<span style="color:#94a3b8;">—</span>';
                            }
                            var items = d.split(",").map(function (x) { return x.trim(); }).filter(Boolean);
                            function badge(txt) {
                                return '<span style="display:inline-block;background:#f1f5f9;color:#334155;' +
                                    'border-radius:4px;padding:2px 6px;margin:2px;font-size:11px;">' + txt + '</span>';
                            }
                            if (items.length <= 2) return items.map(badge).join(" ");
                            var visible = items.slice(0, 2).map(badge).join(" ");
                            return visible +
                                '<span class="ms-1" data-bs-toggle="tooltip" data-bs-placement="top" title="' +
                                items.join(", ") + '">' +
                                '<span style="cursor:pointer;color:#0d6efd;font-weight:600;">+' +
                                (items.length - 2) + '</span></span>';
                        }
                    },
                    {
                        // ── Remarks column (Manual entry এর remarks) ────────────
                        data: "remarks", 
                        render: function (d) {
                            if (!d || d.trim() === "")
                                return '<span style="color:#94a3b8;font-size:12px;">—</span>';

                            var items = d.split(",").map(function (x) { return x.trim(); }).filter(Boolean);
                            if (items.length === 1) {
                                // single remark: tooltip-এ
                                var txt = items[0];
                                if (txt.length <= 20) {
                                    return '<span style="font-size:12px;color:#334155;">' + txt + '</span>';
                                }
                                return '<span style="font-size:12px;color:#334155;cursor:help;" ' +
                                    'data-bs-toggle="tooltip" data-bs-placement="top" title="' + txt + '">' +
                                    txt.substring(0, 18) + '…</span>';
                            }

                            // Multiple remarks: '+N' tooltip-এ
                            var first = items[0].length <= 15 ? items[0] : items[0].substring(0, 13) + '…';
                            return '<span style="font-size:12px;color:#334155;">' + first + '</span> ' +
                                '<span data-bs-toggle="tooltip" data-bs-placement="top" title="' +
                                items.join(", ") + '" style="cursor:pointer;color:#0d6efd;font-weight:600;font-size:11px;">' +
                                '+' + (items.length - 1) + '</span>';
                        }
                    }
                ],
                drawCallback: function () {
                    // Bootstrap tooltip সব জায়গায় init করো
                    $('[data-bs-toggle="tooltip"]').tooltip({ trigger: 'hover' });
                }
            });
        }

        // ════════════════════════════════════════════════════════
        // ── SignalR ─────────────────────────────────────────────
        // ════════════════════════════════════════════════════════
        function initSignalR() {
            if (typeof signalR === "undefined") {
                console.warn("SignalR script not loaded.");
                return;
            }
            _connection = new signalR.HubConnectionBuilder()
                .withUrl("/attendanceHub")
                .withAutomaticReconnect([0, 2000, 5000, 10000])
                .build();

            _connection.on("ReceiveAttendanceUpdate", function () {
                if (!dataTable || _reloadPending) return;
                _reloadPending = true;
                dataTable.ajax.reload(function () {
                    _reloadPending = false;
                }, false);
            });

            _connection.onreconnected(function () {
                if (dataTable) dataTable.ajax.reload(null, false);
            });

            _connection.start()
                .then(function () {
                    setInterval(function () {
                        if (_connection.state === signalR.HubConnectionState.Connected)
                            _connection.invoke("KeepAlive").catch(function () { });
                    }, 30000);
                })
                .catch(function (err) { console.error("SignalR failed:", err); });
        }

        // ════════════════════════════════════════════════════════
        // ── Live clock (dashboard page) ─────────────────────────
        // ════════════════════════════════════════════════════════
        function startClock() {
            function pad(n) { return n < 10 ? '0' + n : n; }
            var days = ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'];
            var months = ['January','February','March','April','May','June','July','August','September','October','November','December'];

            function formatDate(d) {
                return days[d.getDay()] + ', ' + d.getDate() + ' ' + months[d.getMonth()] + ' ' + d.getFullYear();
            }

            function formatTime(d) {
                var h = d.getHours();
                var ampm = h >= 12 ? 'PM' : 'AM';
                var hh = h % 12;
                hh = hh ? hh : 12;
                return pad(hh) + ':' + pad(d.getMinutes()) + ':' + pad(d.getSeconds()) + ' ' + ampm;
            }

            function tick() {
                var now = new Date();
                var te = document.getElementById("currentTime");
                var de = document.getElementById("currentDate");
                var hTe = document.getElementById("headerTime");
                var hDe = document.getElementById("headerDate");

                if (te) te.textContent = formatTime(now);
                if (de) de.textContent = formatDate(now);

                // Also update shared header center clock (keeps header in sync when visible)
                if (hTe) hTe.textContent = formatTime(now);
                if (hDe) hDe.textContent = formatDate(now);
            }
            tick();
            setInterval(tick, 500);
        }

        // ════════════════════════════════════════════════════════
        // ── LEAVE DASHBOARD ─────────────────────────────────────
        // ════════════════════════════════════════════════════════

        function initYearDropdown() {
            var currentYear = new Date().getFullYear();
            var $sel = $("#getYearLeave");
            $sel.empty();
            for (var y = currentYear; y >= currentYear - 20; y--) {
                $sel.append($("<option>", { value: y, text: y }));
            }
            $sel.val(currentYear);
        }

        function getLeaveFilters() {
            return {
                companyCode: $("#companySelectLeave").val() || "",
                branchCode: $("#branchSelectLeave").val() || "",
                departmentCode: $("#departmentSelectLeave").val() || "",
                year: $("#getYearLeave").val() || new Date().getFullYear(),
                employeeId: $("#employeeSelectLeave").val() || ""   // ← নতুন
            };
        }

        function renderLeaveSummary(s) {
            if (!s) return;
            animateCount("#lvTotalApplied", s.totalApplied);
            animateCount("#lvApproved", s.approved);
            animateCount("#lvCanceled", s.canceled);
            animateCount("#lvPending", s.pending);
        }

        // ── Build dynamic columns ──────────────────────────────
        function buildLeaveColumns(leaveTypes) {
            if (leaveTable && $.fn.DataTable.isDataTable("#leaveSummaryTable")) {
                leaveTable.destroy();
                leaveTable = null;
            }

            var $thead = $("#leaveSummaryTable thead");
            $thead.empty();
            var n = leaveTypes.length;
            var r1 = '<tr>' +
                '<th rowspan="2" style="text-align:center;">Emp ID</th>' +
                '<th rowspan="2" style="text-align:center; color:#000;">Name</th>' +
                '<th rowspan="2" style="text-align:center;color:#000;">Designation</th>' +
                '<th rowspan="2" style="text-align:center;color:#000;">Joining Date</th>' +
                '<th colspan="' + n + '" class="head-granted">Granted Leave</th>' +
                '<th colspan="' + n + '" class="head-availed">Availed Leave</th>' +
                '<th colspan="' + n + '" class="head-balanced">Balanced Leave</th>' +
                '</tr>';
            var r2 = '<tr style="font-size:11px;">';
            ['head-granted', 'head-availed', 'head-balanced'].forEach(function (cls) {
                leaveTypes.forEach(function (lt) {
                    r2 += '<th class="' + cls + '">' + lt.shortName + '</th>';
                });
            });
            r2 += '</tr>';
            $thead.html(r1 + r2);
            $("#leaveSummaryTable tbody").empty();

            var cols = [
                {
                    data: "employeeId", className: "text-center",
                    render: function (d) {
                        return '<span style="font-weight:600;font-size:12px;">' + (d || '') + '</span>';
                    }
                },
                {
                    data: "name",
                    render: function (d) {
                        return '<span style="font-size:13px; color:#000;">' + (d || '') + '</span>';
                    }
                },
                {
                    data: "designation",
                    render: function (d) {
                        return '<span style="font-size:12px;color:#000;">' + (d || '—') + '</span>';
                    }
                },
                {
                    data: "joiningDate", className: "text-center",
                    render: function (d) {
                        return '<span style="font-size:12px;color:#000;">' + (d || '') + '</span>';
                    }
                }
            ];

            leaveTypes.forEach(function (lt) {
                cols.push({
                    data: null, className: "text-center",
                    render: function (d, t, row) {
                        var val = getLeaveVal(row, lt.leaveTypeCode, 'grantedDays');
                        return '<span style="color:#000; ">' + fmtDay(val) + '</span>';
                    }
                });
            });

            leaveTypes.forEach(function (lt) {
                cols.push({
                    data: null, className: "text-center",
                    render: function (d, t, row) {
                        var val = getLeaveVal(row, lt.leaveTypeCode, 'availedDays');
                        return '<span style="color:#000;">' + fmtDay(val) + '</span>';
                    }
                });
            });

            leaveTypes.forEach(function (lt) {
                cols.push({
                    data: null, className: "text-center",
                    render: function (d, t, row) {
                        var val = getLeaveVal(row, lt.leaveTypeCode, 'balancedDays');
                        var color = parseFloat(val) < 0 ? '#000' : '#000';
                        return '<span style="color:' + color + ';">' + fmtDay(val) + '</span>';
                    }
                });
            });

            return cols;
        }

        function pivotRows(flatRows) {
            var empMap = {};
            var empOrder = [];
            flatRows.forEach(function (r) {
                if (!empMap[r.employeeId]) {
                    empMap[r.employeeId] = {
                        employeeId: r.employeeId,
                        name: r.name,
                        designation: r.designation,
                        joiningDate: r.joiningDate,
                        rowNum: r.rowNum,
                        totalCount: r.totalCount,
                        _leaves: {}
                    };
                    empOrder.push(r.employeeId);
                }
                empMap[r.employeeId]._leaves[r.leaveTypeCode] = {
                    grantedDays: r.grantedDays,
                    availedDays: r.availedDays,
                    balancedDays: r.balancedDays
                };
            });
            return empOrder.map(function (id) { return empMap[id]; });
        }

        function getLeaveVal(row, leaveTypeCode, field) {
            var leaves = row._leaves || {};
            return leaves[leaveTypeCode] ? leaves[leaveTypeCode][field] : 0;
        }

        function fmtDay(val) {
            var n = parseFloat(val) || 0;
            return n % 1 === 0 ? n.toString() : n.toFixed(2);
        }

        // ── Leave DataTable init ──────────────────────────────
        function initLeaveTable(leaveTypes) {
            var cols = buildLeaveColumns(leaveTypes);

            leaveTable = $("#leaveSummaryTable").DataTable({
                processing: true,
                serverSide: true,
                searching: true,
                ordering: false,
                pageLength: 10,
                lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
                autoWidth: false,
                language: {
                    processing: '<span><i class="fa fa-spinner fa-spin"></i> Loading...</span>',
                    emptyTable: "No leave data found",
                    zeroRecords: "No matching records",
                    info: "Showing _START_ to _END_ of _TOTAL_ employees",
                    infoEmpty: "No records",
                    search: "Search:",
                    lengthMenu: "Show _MENU_"
                },
                ajax: {
                    url: "/Dashboard/leave-dashboard",
                    type: "POST",
                    data: function (d) {
                        var f = getLeaveFilters();
                        d.companyCode = f.companyCode;
                        d.branchCode = f.branchCode;
                        d.departmentCode = f.departmentCode;
                        d.year = f.year;
                        d.employeeId = f.employeeId;  
                        return d;
                    },
                    dataSrc: function (json) {
                        renderLeaveSummary(json.summary);

                        var newTypes = json.leaveTypes || [];
                        if (JSON.stringify(newTypes) !== JSON.stringify(_leaveTypes)) {
                            _leaveTypes = newTypes;
                            setTimeout(function () {
                                initLeaveTable(_leaveTypes);
                            }, 0);
                            return [];
                        }

                        return pivotRows(json.data || []);
                    },
                    error: function (xhr, err) {
                        console.error("Leave AJAX error:", err, xhr.responseText);
                    }
                },
                columns: cols,
                drawCallback: function () { }
            });
        }

        // ════════════════════════════════════════════════════════
        // ── Filter setup ────────────────────────────────────────
        // ════════════════════════════════════════════════════════
        async function setupFilters() {
            if (typeof s2_InitSingle === "function") {
                s2_InitSingle("#companySelect", "/GcFilters/company", "Select Company", "company", ["#branchSelect", "#departmentSelect"]);
                s2_InitSingle("#branchSelect", "/GcFilters/branch", "Select Branch", "branch", ["#departmentSelect"]);
                s2_InitSingle("#departmentSelect", "/GcFilters/department", "Select Department", "department");

                s2_InitSingle("#companySelectLeave", "/GcFilters/company", "Select Company", "company", ["#branchSelectLeave", "#departmentSelectLeave"]);
                s2_InitSingle("#branchSelectLeave", "/GcFilters/branch", "Select Branch", "branch", ["#departmentSelectLeave"]);
                s2_InitSingle("#departmentSelectLeave", "/GcFilters/department", "Select Department", "department");
                s2_InitSingle("#employeeSelectLeave", "/GcFilters/employee", "Select Employee", "employee");

                // Employee ID dropdown for Leave filter — /GcFilters/employee এ route থাকলে
                // otherwise plain text input
                //if (typeof s2_InitEmployee === "function") {
                //    s2_InitEmployee("#employeeSelectLeave", "Select Employee");
                //}
            }

            // ── Attendance filter ──────────────────────────────
            $("#companySelect, #branchSelect, #departmentSelect")
                .off("change.atd")
                .on("change.atd", function () {
                    if (dataTable) dataTable.ajax.reload(null, false);
                });

            // ── Leave filter (company/branch/dept/year/employeeId) ──
            $("#companySelectLeave, #branchSelectLeave, #departmentSelectLeave, #getYearLeave, #employeeSelectLeave")
                .off("change.leave")
                .on("change.leave", function () {
                    if (leaveTable) leaveTable.ajax.reload(null, false);
                });

            // ── Default company ────────────────────────────────
            if (typeof s2_AutoSelectCompany === "function") {
                await s2_AutoSelectCompany("001", "#companySelect");
                await s2_AutoSelectCompany("001", "#companySelectLeave");
            }
        }

        // ════════════════════════════════════════════════════════
        // ── Boot ────────────────────────────────────────────────
        // ════════════════════════════════════════════════════════
        $(document).ready(async function () {
            startClock();
            initChart();
            await setupFilters();
            initDataTable();
            initSignalR();

            initYearDropdown();

            // Leave table: প্রথমে leaveTypes fetch করে পরে table init
            $.ajax({
                url: "/Dashboard/leave-dashboard",
                type: "POST",
                data: $.extend(getLeaveFilters(), {
                    draw: 1, start: 0, length: 10,
                    "search[value]": ""
                }),
                success: function (res) {
                    renderLeaveSummary(res.summary);
                    _leaveTypes = res.leaveTypes || [];
                    initLeaveTable(_leaveTypes);
                },
                error: function (xhr, err) {
                    console.error("Leave init error:", err);
                    initLeaveTable([]);
                }
            });
        });
    };
}(jQuery));