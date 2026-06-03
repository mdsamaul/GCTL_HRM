function showToast(iconType, message) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 30000,
        timerProgressBar: true,
        showClass: {
            popup: 'swal2-show swal2-fade-in'
        },
        hideClass: {
            popup: 'swal2-hide swal2-fade-out'
        }
    });

    Toast.fire({
        icon: iconType,
        title: message
    });
}
$(document).ready(function () {
    setupLoadingOverlay();
    //initializeAppMultiselects();
    //loadFilterEmp();
    loadGridData();
    GetRosterScheduleApproveGride();

    $('#activityStatusSelect').multiselect({
        onInitialized: function () {
            const btn = document.querySelector(
                "#activityStatusSelect + .btn-group > .multiselect.dropdown-toggle.custom-select"
            );
            if (btn) btn.style.width = "8vw";
        }
    });
});

//date picker 
$(document).ready(function () {
    flatpickr('.flatpickr', {
        dateFormat: "Y-m-d",
        altInput: true,
        altFormat: "d/m/Y",
        allowInput: true,
        onReady: function (selectedDates, dateStr, instance) {
            instance.input.placeholder = "dd/mm/yyyy";
        }
    });

    flatpickr('.flatpickr', CalendarService.createConfig(
        {
            defaultDate: new Date(),
        }
    ));
});




//scroll top toolbars 
$(document).ready(() => {
    const header = document.getElementById("stickyHeader");

    window.addEventListener("scroll", function () {
        if (header) {
            if (window.scrollY > 50) {
                header.classList.add("sticky-scrolled");
            } else {
                header.classList.remove("sticky-scrolled");
            }
        }
    });
});

//let RosterApprovalDataTable = null;

function setupLoadingOverlay() {
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
}

function showLoading() {
    $("#customLoadingOverlay").css("display", "flex");
}

function hideLoading() {
    $("#customLoadingOverlay").hide();
}



let RosterApprovalDataTable;
let currentPage = 1;
let pageSize = 10;


//$(document).ready(async function () {
   
//    gcBindRemoteMultiselect("#companySelect", "/GcFilters/company", "Select Company");
//    gcBindRemoteMultiselect("#branchSelect", "/GcFilters/branch", "Select Branch");
//    gcBindRemoteMultiselect("#divisionSelect", "/GcFilters/division", "Select Division");
//    gcBindRemoteMultiselect("#departmentSelect", "/GcFilters/department", "Select Department");
//    gcBindRemoteMultiselect("#designationSelect", "/GcFilters/designation", "Select Designation");
//    gcBindRemoteMultiselect("#employeeSelect", "/GcFilters/employee", "Select Employee");

//    bsms_InitializeMultiselects();
//    bsms_BindCascade();
//    bsms_Reset("#companySelect");
//    await bsms_LoadNext("#companySelect", "/GcFilters/company");
//    await bsms_AutoSelectCompany("001");


//    $("#companySelect, #branchSelect, #divisionSelect, #departmentSelect, #designationSelect, #employeeSelect, #activityStatusSelect, #ToDateFilter, #FromDateFilter")
//        .on("change", function () {
//            currentPage = 1;
//            loadGridData();
//        });
//});

$(document).ready(async function () {

    bindRemoteMultiselect("#companySelect", "/GcAccessFilter/companies", "Select Company", "company");
    bindRemoteMultiselect("#branchSelect", "/GcAccessFilter/branches", "Select Branch", "branch");
    bindRemoteMultiselect("#divisionSelect", "/GcAccessFilter/divisions", "Select Division", "division");
    bindRemoteMultiselect("#departmentSelect", "/GcAccessFilter/departments", "Select Department", "department");
    bindRemoteMultiselect("#designationSelect", "/GcAccessFilter/designations", "Select Designation", "designation");
    bindRemoteMultiselect("#employeeSelect", "/GcAccessFilter/employees", "Select Employee", null);

    var accessCode = $("#hdnAccessCode").val();
    var isReadonly = accessCode === "0005";

    ms_InitializeMultiselects();
    ms_BindCascade();
    if (isReadonly) {
        ms_InitializeMultiselects(null, null, true);
        await ms_ApplyAccessCodeToAll(accessCode);
    } else {
        ms_InitializeMultiselects();
        ms_BindCascade();
        ms_Reset("#companySelect");
        await ms_LoadNext("#companySelect", "...");
        await ms_AutoSelectCompany("001");
    }

    $("#companySelect, #branchSelect, #divisionSelect, #departmentSelect, #designationSelect, #employeeSelect, #activityStatusSelect, #ToDateFilter, #FromDateFilter")
        .on("change", function () {
            currentPage = 1;
            loadGridData();
        });

});



// Get all filter values
function getAllFilterVal() {
    const fromDateVal = $("#FromDateFilter").val();
    const toDateVal = $("#ToDateFilter").val();

    return {
        CompanyCodes: toArray($("#companySelect").val() || ['001']),
        BranchCodes: toArray($("#branchSelect").val()),
        DivisionCodes: toArray($("#divisionSelect").val()),
        DepartmentCodes: toArray($("#departmentSelect").val()),
        DesignationCodes: toArray($("#designationSelect").val()),
        EmployeeIDs: toArray($("#employeeSelect").val()),
        EmployeeStatuses: toArray($("#activityStatusSelect").val()),
        FromDate: fromDateVal ? new Date(fromDateVal).toISOString() : null,
        ToDate: toDateVal ? new Date(toDateVal).toISOString() : null,
        PageNumber: currentPage,
        PageSize: pageSize,
        SortColumn: "",
        SortDirection: "asc",
        SearchValue: ""
    };
}

function toArray(value) {
    if (!value) return [];
    if (Array.isArray(value)) return value;
    return [value];
}


// Load grid data with server-side pagination
function loadGridData() {
    showLoading();
    var filterData = getAllFilterVal();

    // Get search value from DataTable if exists
    if (RosterApprovalDataTable) {
        filterData.SearchValue = RosterApprovalDataTable.search();
    }

    $.ajax({
        url: `/RosterScheduleApproval/GetRosterGridData`,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(filterData),
        success: function (res) {
            if (!res.isSuccess) {
                showToast('error', res.message);
                return;
            }

            loadTableData(res.data, res.recordsTotal, res.recordsFiltered);
        },
        complete: function () {
            hideLoading();
        },
        error: function (xhr, status, error) {
            showToast("error", error || "Failed to load data");
            hideLoading();
        }
    });
}

// Load table data with DataTables server-side processing
function loadTableData(tableData, recordsTotal, recordsFiltered) {
    if ($.fn.DataTable.isDataTable("#RosterScheduleApprove-grid")) {
        RosterApprovalDataTable.destroy();
    }

    initializeDataTable(recordsTotal, recordsFiltered);
}

// Initialize DataTable with server-side pagination
function initializeDataTable(recordsTotal, recordsFiltered) {
    RosterApprovalDataTable = $("#RosterScheduleApprove-grid").DataTable({
        destroy: true,
        serverSide: true,
        processing: false,
        paging: true,
        pageLength: pageSize,
        lengthMenu: [[10, 25, 50, 100, 1000, 10000], [10, 25, 50, 100, 1000, 10000]],
        lengthChange: true,
        searching: true,
        info: true,
        autoWidth: false,
        scrollX: true,
        ordering: true,
        responsive: false,
        ajax: function (data, callback, settings) {
            currentPage = Math.floor(data.start / data.length) + 1;
            pageSize = data.length;          
            var filterData = getAllFilterVal();
            filterData.PageNumber = currentPage;          

            filterData.PageSize = pageSize;
            filterData.SearchValue = data.search.value;

            if (data.order && data.order.length > 0) {
                var orderColumn = data.columns[data.order[0].column];
                filterData.SortColumn = orderColumn.name || "";
                filterData.SortDirection = data.order[0].dir;
            }

            $.ajax({
                url: `/RosterScheduleApproval/GetRosterGridData`,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(filterData),
                success: function (res) {
                    if (res.isSuccess) {
                        callback({
                            draw: data.draw,
                            recordsTotal: res.recordsTotal,
                            recordsFiltered: res.recordsFiltered,
                            data: res.data
                        });
                    } else {
                        showToast('error', res.message);
                        callback({
                            draw: data.draw,
                            recordsTotal: 0,
                            recordsFiltered: 0,
                            data: []
                        });
                    }
                },
                error: function (xhr, status, error) {
                    showToast("error", error || "Failed to load data");
                    callback({
                        draw: data.draw,
                        recordsTotal: 0,
                        recordsFiltered: 0,
                        data: []
                    });
                }
            });
        },
        columns: [
            {
                data: null,
                orderable: false,
                render: function (data, type, row) {
                    return '<input type="checkbox" data-roster-id="' + row.rosterScheduleId + '" />';
                }
            },
            { data: 'rosterScheduleId', name: 'RosterScheduleId' },
            { data: 'code', name: 'EmpId' },
            { data: 'name', name: 'EmpName' },
            { data: 'designationName', name: 'Designation' },
            { data: 'departmentName', name: 'Department' },
            { data: 'showDate', name: 'Date' },
            { data: 'dayName', name: 'DayName' },
            { data: 'shiftName', name: 'ShiftName' },
            { data: 'remark', name: 'Remark' }
        ],
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
            emptyTable: "No data available",
            processing: "Loading..."
        },
        columnDefs: [
            { orderable: false, targets: 0 },
            //{ className: "text-left", targets: [3, 4,5,9] },
            //{ className: "text-center", targets: [0,1,2,6,7,8] },
            { className: "text-center align-middle", targets: "_all" },
            { targets: 0, width: "80px" }
        ],
        initComplete: function () {
            $('.dataTables_filter input').css({
                width: '250px',
                padding: '6px 12px',
                border: '1px solid #ddd',
                borderRadius: '4px'
            });
           

        }
    });

    setTimeout(function () {
        RosterApprovalDataTable.columns.adjust().draw(false);        
    }, 300);
}



$(document).ready(function () {
    $("#rosterScheduleApprove-check-all").on('change', function () {
        var isChecked = $(this).is(":checked");
        $('#RosterScheduleApprove-grid-body input[type="checkbox"]').prop('checked', isChecked);
    });

    $(document).on('change', '#RosterScheduleApprove-grid-body input[type="checkbox"]', function () {
        var totalCheck = $('#RosterScheduleApprove-grid-body input[type="checkbox"]').length;
        var singleCheck = $('#RosterScheduleApprove-grid-body input[type="checkbox"]:checked').length;
        if (totalCheck === singleCheck) {
            $("#rosterScheduleApprove-check-all").prop("checked", true);
        } else {
            $("#rosterScheduleApprove-check-all").prop("checked", false);
        }
    });

    $(".js-roster-approval-save").click(function () {        
        var checkedApprovalList = [];
        $('#RosterScheduleApprove-grid-body input[type="checkbox"]:checked').each(function () {
            var row = $(this).closest('tr');
            var rosterId = row.find('td:nth-child(2)').text().trim();
            checkedApprovalList.push(rosterId);
        });
        if (checkedApprovalList.length === 0) {
            showToast("warning", "Please Select at Lest one roster Item.");
            return;
        }
        var remark = $("#remarks").val();
        var FromData = {
            checkedApprovalList: checkedApprovalList,
            remark: remark
        }
        $.ajax({
            url: "/RosterScheduleApproval/ApprovalSetUp",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(FromData),
            success: function (res) {
                
                loadGridData();
                GetRosterScheduleApproveGride();

                $("#remarks").val("");
                $("#rosterScheduleApprove-check-all").prop('checked', false);
            },
            error: function (e) {
                console.log(e.message);
            }
        });
    })

})

function GetRosterScheduleApproveGride() {
    showLoading();

    if ($.fn.DataTable.isDataTable('#RosterScheduleApproveShowData-grid')) {
        $('#RosterScheduleApproveShowData-grid').DataTable().clear().destroy();
    }

    $('#RosterScheduleApproveShowData-grid').DataTable({
        processing: true,
        serverSide: true,
        scrollY: "350px",
        scrollX: true,
        scrollCollapse: true,
        ajax: {
            url: "/RosterScheduleApproval/GetRosterScheduleApproveGrid",
            type: "POST",
            dataSrc: function (json) {
                return json.data || json;
            },
            error: function (xhr, error, thrown) {
              
                hideLoading();
                alert("An error occurred while loading data. Please check console for details.");
            }
        },
        columns: [
            { data: "rosterScheduleId", orderable: false },
            { data: "employeeID", orderable: false },
            { data: "name", orderable: false },
            { data: "designationName", orderable: false },
            {
                data: "date",
                render: function (data) {
                    if (!data) return '';
                    let d = new Date(data);
                    return d.toLocaleDateString();
                },
                orderable: false
            },
            { data: "shiftName", orderable: false },
            { data: "remark", orderable: false },
            {
                data: "approvalStatus",
                render: function (data) {
                    let statusClass = '';
                    if (data === 'Approved') statusClass = 'text-dark font-weight-bold';
                    else if (data === 'Rejected') statusClass = 'text-danger font-weight-bold';
                    else statusClass = 'text-warning';

                    return '<span class="' + statusClass + '">' + data + '</span>';
                },
                orderable: false
            },
            { data: "approvedBy", orderable: false },
            {
                data: "approvalDatetime",
                render: function (data) {
                    if (!data) return '';
                    let d = new Date(data);
                    return d.toLocaleString('en-US', {
                        year: 'numeric',
                        month: '2-digit',
                        day: '2-digit',
                        hour: '2-digit',
                        minute: '2-digit',
                        hour12: true
                    });
                },
                orderable: false
            },
            { data: "luser", orderable: false }
        ],
        order: [],
        ordering: false,
        pageLength: 10,
        lengthMenu: [[10, 25, 50, 100, 1000, -1], [10, 25, 50, 100, 1000, "All"]],
        language: {
            search: "🔍 Search:",
            lengthMenu: "Show _MENU_ entries",
            searchPlaceholder: "Search here...",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            paginate: {
                first: "First",
                previous: "Prev",
                next: "Next",
                last: "Last"
            },
            emptyTable: "No data available"
        },
        initComplete: function () {
            hideLoading();

            $('.dataTables_filter input').css({
                width: '250px',
                padding: '6px 12px',
                border: '1px solid #ddd',
                borderRadius: '4px'
            });

            setTimeout(function () {
                $('#RosterScheduleApproveShowData-grid').DataTable().columns.adjust().draw(false);
            }, 300);


            $(window).off('resize.rosterGridResize').on('resize.rosterGridResize', function () {
                $('#RosterScheduleApproveShowData-grid').DataTable().columns.adjust();
            });
        },
        drawCallback: function () {
            setTimeout(function () {
                $('#RosterScheduleApproveShowData-grid').DataTable().columns.adjust();
            }, 300);
        }
    });
}


$(document).ready(function () {
    $(document).on('click', "#js-roster-approval-clear", function () {
        clearRosterApproveFrom();
    });

})
function clearRosterApproveFrom() {
    $("#rosterScheduleApprove-check-all").prop('checked', false);
    $('#RosterScheduleApprove-grid input[type="checkbox"]').prop('checked', false);
    $("#remarks").val("");
}