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
    //initializeMultiselects();
    loadFilterEmp();
    GetManualEntryApproveGrid();
});

// Date picker 
$(document).ready(function () {
    //flatpickr('.flatpickr', {
    //    dateFormat: "Y-m-d",
    //    altInput: true,
    //    altFormat: "d/m/Y",
    //    allowInput: true,
    //    onReady: function (selectedDates, dateStr, instance) {
    //        instance.input.placeholder = "dd/mm/yyyy";
    //    }
    //});
    flatpickr($(".flatpickr"), CalendarService.createConfig(
        {

            defaultDate: new Date(),
        }
    ));
});




$(document).on('shown.bs.dropdown', '.btn-group', function () {

    const $group = $(this);

    if (!$group.find('.multiselect-search').length) return;

    setTimeout(function () {
        $group.find('.multiselect-search').focus();
    }, 0);
});

// Scroll top toolbars 
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

let ManualApprovalDataTable = null;

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


function getAllFilterVal() {
    const fromDateVal = $("#FromDateFilter").val();
    const toDateVal = $("#ToDateFilter").val();

    const filterData = {
        CompanyCodes: toArray($("#companySelect").val()),
        BranchCodes: toArray($("#branchSelect").val()),
        DivisionCodes: toArray($("#divisionSelect").val()),
        DepartmentCodes: toArray($("#departmentSelect").val()),
        DesignationCodes: toArray($("#designationSelect").val()),
        EmployeeIDs: toArray($("#employeeSelect").val()),
        EmployeeStatuses: toArray($("#activityStatusSelect").val()),
        FromDate: fromDateVal ? new Date(fromDateVal).toISOString() : null,
        ToDate: toDateVal ? new Date(toDateVal).toISOString() : null
    };
    return filterData;
}

function toArray(value) {
    if (!value) return [];
    if (Array.isArray(value)) return value;
    return [value];
}
//$(document).ready(async function () {

//    gcBindRemoteMultiselect("#companySelect", "/GcFilters/company", "Select Company");
//    gcBindRemoteMultiselect("#branchSelect", "/GcFilters/branch", "Select Branch");
//    gcBindRemoteMultiselect("#divisionSelect", "/GcFilters/division", "Select Division");
//    gcBindRemoteMultiselect("#departmentSelect", "/GcFilters/department", "Select Department");
//    gcBindRemoteMultiselect("#designationSelect", "/GcFilters/designation", "Select Designation");
//    gcBindRemoteMultiselect("#employeeSelect", "/GcFilters/employee", "Select Employee");

//    gcRegisterSelector("#activityStatusSelect", "employeeStatus");

//    bsms_InitializeMultiselects();
//    bsms_BindCascade();
//    bsms_Reset("#companySelect");
//    await bsms_LoadNext("#companySelect", "/GcFilters/company");
//    await bsms_AutoSelectCompany("001");

//    $("#companySelect, #branchSelect, #divisionSelect, #departmentSelect, #designationSelect, #employeeSelect, #activityStatusSelect")
//        .on("change", function () {
//            loadFilterEmp();
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
        $("#companySelect, #branchSelect, #divisionSelect, #departmentSelect, #designationSelect, #employeeSelect, #activityStatusSelect")
            .on("change", function () {
                loadFilterEmp();
            });
});
function loadFilterEmp() {
   
    var filterData = getAllFilterVal();
    filterData.CompanyCodes.push('001');
    console.log(filterData);
    $.ajax({
        url: `/ManualEntryApproval/GetManualEntryFilter`,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(filterData),
        success: function (res) {
            console.log(res);
            if (!res.isSuccess) {
                showToast('error', res.message);
                return;
            }          
           
            loadTableData(res);          
           
        },
        complete: function () {
            hideLoading();
        },
        error: function (error) {
            showToast("error", error.message);
            hideLoading();
        }
    })
}

function loadTableData(res) {
    var tableData = res.data.employees;
    console.log(tableData);

    if ($.fn.DataTable.isDataTable("#ManualEntryApprove-grid")) {
        ManualApprovalDataTable.destroy();
    }

    var tableBody = $("#ManualEntryApprove-grid-body");
    tableBody.empty();

    $.each(tableData, function (index, employee) {
        var row = $('<tr>');
        row.append('<td class="text-center"><input type="checkbox" data-manual-code="' + employee.manualId + '" /></td>');
        row.append('<td class="text-center">' + (employee.code || '') + '</td>');
        row.append('<td class="text-left">' + (employee.empId || '') + '</td>');
        row.append('<td class="text-left">' + (employee.name || '') + '</td>');
        row.append('<td class="text-left">' + (employee.designationName || '') + '</td>');
        row.append('<td class="text-left">' + (employee.divisionName || '') + '</td>');
        row.append('<td class="text-center">' + (employee.departmentName || '') + '</td>');
        row.append('<td class="text-center">' + (employee.attandanceType || '') + '</td>');
        row.append('<td class="text-center">' + (employee.showDate || '') + '</td>');
        row.append('<td class="text-center">' + (employee.showTime || '') + '</td>');
        row.append('<td class="text-center">' + (employee.remark || '') + '</td>');
        tableBody.append(row);
    });

    initializeDataTable();
}

function initializeDataTable() {
    ManualApprovalDataTable = $("#ManualEntryApprove-grid").DataTable({
        destroy: true,
        paging: true,
        pageLength: 10,
        lengthMenu: [[10, 25, 50, 100, 1000, -1], [10, 25, 50, 100, 1000, "All"]],
        lengthChange: true,
        searching: true,
        info: true,
        autoWidth: false,
        scrollX: true,
        ordering: true, 
        responsive: false,
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
            { orderable: false, targets: 0 },           
            { targets: 0, width: "40px" }
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
        ManualApprovalDataTable.columns.adjust().draw(false);
    }, 300);
}

$(document).ready(function () {
    $("#manualEntryApprove-check-all").on('change', function () {
        var isChecked = $(this).is(":checked");
        $('#ManualEntryApprove-grid-body input[type="checkbox"]').prop('checked', isChecked);
    });

    $(document).on('change', '#ManualEntryApprove-grid-body input[type="checkbox"]', function () {
        var totalCheck = $('#ManualEntryApprove-grid-body input[type="checkbox"]').length;
        var singleCheck = $('#ManualEntryApprove-grid-body input[type="checkbox"]:checked').length;
        if (totalCheck === singleCheck) {
            $("#manualEntryApprove-check-all").prop("checked", true);
        } else {
            $("#manualEntryApprove-check-all").prop("checked", false);
        }
    });

   
    $(".js-manual-approval-save").click(function () {
        var checkedApprovalList = [];
      
        $('#ManualEntryApprove-grid-body input[type="checkbox"]:checked').each(function () {
            var manualCode = $(this).data('manual-code');
            if (manualCode !== undefined && manualCode !== null) {
                checkedApprovalList.push(String(manualCode));
            }
        });

        if (checkedApprovalList.length === 0) {
            showToast("warning", "Please select at least one manual entry item.");
            return;
        }

        var remark = $("#remarks").val();
        var formData = {
            CheckedApprovalList: checkedApprovalList,
            Remark: remark
        }
        console.log(formData);
        $.ajax({
            url: "/ManualEntryApproval/ApprovalSetUp",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (res) {
                if (res.isSuccess) {
                    showToast("success", res.message);
                    loadFilterEmp();
                    GetManualEntryApproveGrid();
                    $("#remarks").val("");
                    $("#manualEntryApprove-check-all").prop('checked', false);
                } else {
                    showToast("error", res.message);
                }
            },
            error: function (e) {
                showToast("error", "An error occurred while approving.");
                console.log(e.message);
            }
        });
    });
});

function GetManualEntryApproveGrid() {
    showLoading();
    if ($.fn.DataTable.isDataTable('#ManualEntryApproveShowData-grid')) {
        $('#ManualEntryApproveShowData-grid').DataTable().clear().destroy();
    }
    $('#ManualEntryApproveShowData-grid').DataTable({
        processing: true,
        serverSide: true,
        scrollY: "550px",
        scrollX: true,
        scrollCollapse: true,
        ajax: {
            url: "/ManualEntryApproval/GetManualEntryApproveGrid",
            type: "POST",
            dataSrc: function (json) {
                console.log(json);
                return json.data || json;
            },
            error: function (xhr, error, thrown) {               
                hideLoading();
                alert("An error occurred while loading data. Please check console for details.");
            }
        },
        columns: [
            { data: "manualCode", orderable: false, className: "text-center" },
            { data: "employeeId", orderable: false, className: "text-center" },
            { data: "employeeName", orderable: false, className: "text-left" },
            { data: "designationName", orderable: false, className: "text-left" },
            { data: "showDate", orderable: false, className: "text-left" },       
            {
                data: "time",
                render: function (data) {
                    if (!data) return '';
                    let t = new Date(data);
                    return t.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: true });
                },
                orderable: false,
                className: "text-center"
            },
            { data: "attendanceTypeName", orderable: false, className: "text-center" },
            { data: "remarks", orderable: false, className: "text-left" },
            { data: "approvalStatus", orderable: false, className: "text-center" },
            { data: "approvedBy", orderable: false, className: "text-center" },
            { data: "showApprovalDatetime", orderable: false, className: "text-center" },
            
            { data: "entryUser", orderable: false, className: "text-center" }
        ],
        order: [],
        ordering: false,
        pageLength: 10,
        lengthMenu: [[5, 10, 25, 50, 100, 1000, -1], [5, 10, 25, 50, 100, 1000, "All"]],
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
                $('#ManualEntryApproveShowData-grid').DataTable().columns.adjust().draw(false);
            }, 300);
            $(window).off('resize.manualGridResize').on('resize.manualGridResize', function () {
                $('#ManualEntryApproveShowData-grid').DataTable().columns.adjust();
            });
        },
        drawCallback: function () {
            setTimeout(function () {
                $('#ManualEntryApproveShowData-grid').DataTable().columns.adjust();
            }, 300);
        }
    });
}

$(document).ready(function () {
    $(document).on('click', "#js-manual-approval-clear", function () {
        clearManualApproveForm();
    });
});

function clearManualApproveForm() {
    $("#manualEntryApprove-check-all").prop('checked', false);
    $('#ManualEntryApprove-grid input[type="checkbox"]').prop('checked', false);
    $("#remarks").val("");
}