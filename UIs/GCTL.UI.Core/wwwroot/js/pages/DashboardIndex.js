(function ($) {
    $.DashboardIndex = function (options) {

        var settings = $.extend({ baseUrl: "/" }, options);
        var chart = null;
        var dataTable = null;
        var leaveTable = null;       // ← Leave DataTable instance
        var _leaveTypes = [];         // ← RS2 cache (column headers এর জন্য)
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

        function statusBadge(status) {
            switch ((status || "").toLowerCase()) {
                case "present": return '<span class="status-badge" style="background:#4caf50;">Present</span>';
                case "late": return '<span class="status-badge" style="background:#ff9800;">Late</span>';
                case "on leave": return '<span class="status-badge" style="background:#9c27b0;">On Leave</span>';
                default: return '<span class="status-badge" style="background:#f44336;">Absent</span>';
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

        function initChart() {
            var canvas = document.getElementById("attendanceChart");
            if (!canvas || typeof Chart === "undefined") return;
            chart = new Chart(canvas.getContext("2d"), {
                type: "doughnut",
                data: {
                    labels: ["Present", "Absent", "Late", "On Leave"],
                    datasets: [{
                        data: [0, 0, 0, 0],
                        backgroundColor: ["#4caf50", "#FF6156", "#ff9800", "#9c27b0"],
                        hoverOffset: 8, borderWidth: 2, borderColor: "#ffffff"
                    }]
                },
                options: {
                    responsive: false, cutout: "68%",
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
                processing: true, serverSide: true,
                searching: true, ordering: false,
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
                        data: "employeeId", width: "90px",
                        render: function (d) {
                            return '<span style="font-weight:600;font-size:12px;">' + (d || '') + '</span>';
                        }
                    },
                    {
                        data: "name",
                        render: function (d, t, row) {
                            var src = getPhotoSrc(row);
                            return '<div class="emp-profile">' +
                                '<img src="' + src + '" alt="" ' +
                                'onerror="this.onerror=null;this.src=\'' + DEFAULT_PHOTO + '\'" ' +
                                'style="width:32px;height:32px;border-radius:50%;object-fit:cover;">' +
                                '<span>' + (d || '') + '</span></div>';
                        }
                    },
                    {
                        data: "designation",
                        render: function (d) {
                            return '<span style="font-size:12px;color:#475569;">' + (d || '—') + '</span>';
                        }
                    },
                    {
                        data: "checkIn", width: "95px",
                        render: function (d, t, row) {
                            if (!d) {
                                var color = row.status === "On Leave" ? "#9c27b0" : "#f44336";
                                return '<span style="color:' + color + ';">—</span>';
                            }
                            return '<span style="color:' + checkInColor(row.status) + ';font-weight:600;">' + d + '</span>';
                        }
                    },
                    {
                        data: "checkOut", width: "95px",
                        render: function (d) {
                            if (!d) return '<span style="color:#94a3b8;">—</span>';
                            return '<span style="color:#4caf50;font-weight:600;">' + d + '</span>';
                        }
                    },
                    {
                        data: "status", width: "80px", className: "text-center",
                        render: function (d) { return statusBadge(d); }
                    },
                    {
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
                            if (items.length <= 2) return items.map(badge).join(", ");
                            var visible = items.slice(0, 2).map(badge).join(", ");
                            return visible +
                                '<span class="ms-1" data-bs-toggle="tooltip" data-bs-placement="top" title="' +
                                items.join(", ") + '">' +
                                '<span style="cursor:pointer;color:#0d6efd;font-weight:600;">+' +
                                (items.length - 2) + '</span></span>';
                        }
                    }
                ],
                drawCallback: function () {
                    $('[data-bs-toggle="tooltip"]').tooltip();
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
        // ── Live clock ──────────────────────────────────────────
        // ════════════════════════════════════════════════════════
        function startClock() {
            function tick() {
                var now = new Date();
                var te = document.getElementById("currentTime");
                var de = document.getElementById("currentDate");
                if (te) te.textContent = now.toLocaleTimeString("en-US", {
                    hour: "2-digit", minute: "2-digit", second: "2-digit", hour12: true
                });
                if (de) de.textContent = now.toLocaleDateString("en-GB", {
                    day: "numeric", month: "short", year: "numeric"
                });
            }
            tick();
            setInterval(tick, 1000);
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
                year: $("#getYearLeave").val() || new Date().getFullYear()
            };
        }

        function renderLeaveSummary(s) {
            if (!s) return;
            animateCount("#lvTotalApplied", s.totalApplied);
            animateCount("#lvApproved", s.approved);
            animateCount("#lvCanceled", s.canceled);
            animateCount("#lvPending", s.pending);
        }

        // ── Build dynamic columns (called once on first load / leaveTypes change) ──
        function buildLeaveColumns(leaveTypes) {
            // Destroy existing table first
            if (leaveTable && $.fn.DataTable.isDataTable("#leaveSummaryTable")) {
                leaveTable.destroy();
                leaveTable = null;
            }

            // thead rebuild
            var $thead = $("#leaveSummaryTable thead");
            $thead.empty();
            var n = leaveTypes.length;
            var r1 = '<tr>' +
                '<th rowspan="2" style="text-align:center;">Emp ID</th>' +
                '<th rowspan="2" style="text-align:center;">Name</th>' +
                '<th rowspan="2" style="text-align:center;">Designation</th>' +
                '<th rowspan="2" style="text-align:center;">Joining Date</th>' +
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

            // Build columns array for DataTable
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
                        return '<span style="font-size:13px;">' + (d || '') + '</span>';
                    }
                }, {
                    data: "designation",
                    render: function (d) {
                        return '<span style="font-size:12px;color:#475569;">' + (d || '—') + '</span>';
                    }
                },
                {
                    data: "joiningDate", className: "text-center",
                    render: function (d) {
                        return '<span style="font-size:12px;color:#475569;">' + (d || '') + '</span>';
                    }
                }
            ];

            // Granted columns
            leaveTypes.forEach(function (lt) {
                cols.push({
                    data: null, className: "text-center",
                    render: function (d, t, row) {
                        var val = getLeaveVal(row, lt.leaveTypeCode, 'grantedDays');
                        return '<span style="color:#2e7d32;">' + fmtDay(val) + '</span>';
                    }
                });
            });

            // Availed columns
            leaveTypes.forEach(function (lt) {
                cols.push({
                    data: null, className: "text-center",
                    render: function (d, t, row) {
                        var val = getLeaveVal(row, lt.leaveTypeCode, 'availedDays');
                        return '<span style="color:#1565c0;">' + fmtDay(val) + '</span>';
                    }
                });
            });

            // Balanced columns
            leaveTypes.forEach(function (lt) {
                cols.push({
                    data: null, className: "text-center",
                    render: function (d, t, row) {
                        var val = getLeaveVal(row, lt.leaveTypeCode, 'balancedDays');
                        var color = parseFloat(val) < 0 ? '#f44336' : '#7b1fa2';
                        return '<span style="color:' + color + ';">' + fmtDay(val) + '</span>';
                    }
                });
            });

            return cols;
        }

        // flat rows → grouped map: empId → { leaveTypeCode: { grantedDays, availedDays, balancedDays } }
        // DataTable এর জন্য pivot করে একটা row per employee বানাই
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
            // decimal হলে 2 digit, integer হলে clean
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
                //scrollY: "450px",   
                //scrollCollapse: true,
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
                        return d;
                    },
                    dataSrc: function (json) {
                        // Summary cards update
                        renderLeaveSummary(json.summary);

                        // LeaveTypes পরিবর্তন হলে table rebuild
                        var newTypes = json.leaveTypes || [];
                        if (JSON.stringify(newTypes) !== JSON.stringify(_leaveTypes)) {
                            _leaveTypes = newTypes;
                            // next tick এ rebuild করে reload
                            setTimeout(function () {
                                initLeaveTable(_leaveTypes);
                            }, 0);
                            return [];
                        }

                        // Pivot flat rows → 1 row per employee
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
            }

            // ── Attendance filter — শুধু attendance reload ──────
            $("#companySelect, #branchSelect, #departmentSelect")
                .off("change.atd")
                .on("change.atd", function () {
                    if (dataTable) dataTable.ajax.reload(null, false);
                });

            // ── Leave filter — শুধু leave reload ────────────────
            $("#companySelectLeave, #branchSelectLeave, #departmentSelectLeave, #getYearLeave")
                .off("change.leave")
                .on("change.leave", function () {
                    if (leaveTable) leaveTable.ajax.reload(null, false);
                });

            // ── Default company select ───────────────────────────
            if (typeof s2_AutoSelectCompany === "function") {
                //await s2_AutoSelectCompany("001");
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

            // Leave table: প্রথমে leaveTypes fetch করে তারপর table init
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