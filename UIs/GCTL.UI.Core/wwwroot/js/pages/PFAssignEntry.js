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

$(document).ready(function(){
    setupLoadingOverlay();
    //initializeMultiselects();
    loadFilterEmp();
    getPfAssign();
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


let PFAssignDataTable = null;


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
    const filterData = {
        CompanyCodes: toArray($("#companySelect").val()),
        BranchCodes: toArray($("#branchSelect").val()),
        DivisionCodes: toArray($("#divisionSelect").val()),
        DepartmentCodes: toArray($("#departmentSelect").val()),
        DesignationCodes: toArray($("#designationSelect").val()),
        EmployeeIDs: toArray($("#employeeSelect").val()),
        EmployeeStatuses: toArray($("#activityStatusSelect").val()),
    };
    return filterData;
}
function toArray(value) {
    if (!value) return [];
    if (Array.isArray(value)) return value;
    return [value];
}

$(document).ready(async function () {
    
    gcBindRemoteMultiselect("#companySelect", "/GcFilters/company", "Select Company");
    gcBindRemoteMultiselect("#branchSelect", "/GcFilters/branch", "Select Branch");
    gcBindRemoteMultiselect("#divisionSelect", "/GcFilters/division", "Select Division");
    gcBindRemoteMultiselect("#departmentSelect", "/GcFilters/department", "Select Department");
    gcBindRemoteMultiselect("#designationSelect", "/GcFilters/designation", "Select Designation");
    gcBindRemoteMultiselect("#employeeSelect", "/GcFilters/employee", "Select Employee");

    bsms_InitializeMultiselects();
    bsms_BindCascade();
    bsms_Reset("#companySelect");
    await bsms_LoadNext("#companySelect", "/GcFilters/company");
    await bsms_AutoSelectCompany("001");

    $("#companySelect, #branchSelect, #divisionSelect, #departmentSelect, #designationSelect, #employeeSelect, #activityStatusSelect")
        .on("change", function () {
            loadFilterEmp();
        });
});

function loadFilterEmp() {
    //showLoading();
    var filterData = getAllFilterVal();
    //console.log(filterData);
    $.ajax({
        url: `/PFAssignEntry/getAllFilterEmp`,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(filterData),
        success: function (res) {
            if (!res.isSuccess) {
                showToast('error', res.message);
                return;
            }
            //showToast('success', res.message);
          
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
    //console.log(res);
    var tableData = res.data.employees;
    if (PFAssignDataTable !== null) {
        PFAssignDataTable.destroy();
    }
    var tableBody = $("#employee-PF-Assign-Entry-grid-body");
    tableBody.empty();
    $.each(tableData, function (index, employee) {
        //console.log(employee);
        var row = $('<tr>');
        row.append('<td class="text-center" style="width:60px !important;"><input type="checkbox" /></td >');
        row.append('<td class="text-center">' + employee.code + '</td>');
        row.append('<td class="text-start">' + employee.name + '</td>');
        row.append('<td class="text-start">' + employee.designation + '</td>');
        row.append('<td class="text-center">' + employee.department + '</td>');
        row.append('<td class="text-center">' + employee.branch + '</td>');
        row.append('<td class="text-center">' + employee.company + '</td>');
        row.append('<td class="text-center">' + employee.employeeType + '</td>');
        row.append('<td class="text-center">' + employee.employmentNature + '</td>');
        row.append('<td class="text-center">' + employee.joiningDate + '</td>');
        row.append('<td class="text-center"  width="113">' + parseFloat(employee.serviceDuration).toFixed(2) + '</td>');
        //row.append('<td class="text-center">' + employee.serviceDuration2 + '</td>');

        tableBody.append(row);
    });


    initializeDataTable();
}

function initializeDataTable() {
    PFAssignDataTable = $("#employee-PF-Assign-Entry-grid").DataTable({
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
            hideLoading();
            $('.dataTables_filter input').css({
                'width': '250px',
                'padding': '6px 12px',
                'border': '1px solid #ddd',
                'border-radius': '4px'
            });
        }
    });
}

//date wef
$(document).ready(function () {
    flatpickr("#WEFDatePicker", CalendarService.createConfig({
        dateFormat: "Y-m-d",
        defaultDate: "today",
        minDate: "2020-01-01",
        maxDate: "2030-12-31"
    }));
})

function DatePicker() {
    flatpickr("#WEFDatePicker", CalendarService.createConfig({
        dateFormat: "Y-m-d",
        defaultDate: "today",
        minDate: "2020-01-01",
        maxDate: "2030-12-31"
    }));
}

function getValueFromPFAssgin() {
    var fromData = {
        PFApprovedStatus: $("#PFApprovedStatusSelect").val(),
        ApprovalRemark: $("#remarks").val(),
        EFDate: $("#WEFDatePicker").val(),
        CompanyCode: $("#companySelect").val()
    };
    return fromData;
}
$("#js-pf-assign-entry-clear").click(() => {
    FormAssignClear();
})
function FormAssignClear() {  
    $("#remarks").val('');
    $("#hiddenPFAssignId").val('');
    $("#PFAutoIdDelete").val('');
    DatePicker();


    $("#PFApprovedStatusSelect").val('Confirm');
    $('#employee-PF-Assign-Entry-grid-body input[type="checkbox"]').prop('disabled', false);
    $("#pfAssign-check-all").prop('disabled', false);
    $("#submitButton").text("Save");
    $('#employee-PF-Assign-Entry-grid-body input[type="checkbox"]').prop('checked', false);
    $("#pfAssign-check-all").prop('checked', false);

    $('#PFAssign-grid-body-show input[type="checkbox"]').prop('checked', false);
    $("#PFAssign-grid-check-all").prop('checked', false);
}
$(document).ready(function () {
    //all check
    $("#pfAssign-check-all").on('change', function () {
        var isChecked = $(this).is(':checked');
        $('#employee-PF-Assign-Entry-grid-body input[type="checkbox"]').prop('checked', isChecked);
    })


    $(document).on('change', '#employee-PF-Assign-Entry-grid-body input[type="checkbox"]', function () {
        var totalCheck = $('#employee-PF-Assign-Entry-grid-body input[type="checkbox"]').length;
        var singlecheck = $('#employee-PF-Assign-Entry-grid-body input[type="checkbox"]:checked').length;

        if (totalCheck === singlecheck) {
            $("#pfAssign-check-all").prop('checked', true);
        } else {
            $("#pfAssign-check-all").prop('checked', false);
        }
    });

    $(document).on('input', '#WEFDatePicker', () => {
        var date = $("#WEFDatePicker").val();
        console.log(date);
        if (date === "") {
            //console.log("date empty");
            $(".js-pf-assign-entry-save").prop("disabled", true);
            $("#pfAssignDate").fadeIn(500);
            $("#WEFDatePicker").css("border", "1px solid red");
        } else {
            $("#pfAssignDate").fadeOut(500);
            $("#WEFDatePicker").css("border", "");
            $(".js-pf-assign-entry-save").prop("disabled", false);
        }       
    });


    $(".js-pf-assign-entry-save").click(function () {

      
        var fromData = getValueFromPFAssgin();

        var selectEmployeeIds = [];
        var pfId = $("#hiddenPFAssignId").val();
        if (pfId === "") {
            $('#employee-PF-Assign-Entry-grid-body input[type="checkbox"]:checked').each(function () {
                var row = $(this).closest('tr');
                var empId = row.find('td:nth-child(2)').text().trim();
                selectEmployeeIds.push(empId);
            })
            //console.log(fromData);
            if (selectEmployeeIds.length === 0) {
                showToast("warning", "Please Select at Lest one employee.");
                return;
            }
            fromData.isUpdate = false;
            fromData.EmployeeIds = selectEmployeeIds;
            console.log(fromData);
            $.ajax({
                url: `/PFAssignEntry/CreateEditPFAssignEntry`,
                type: "POST",
                data: JSON.stringify(fromData),
                contentType: "application/json; charset=utf-8",
                success: (res) => {
                    if (res.isSuccess) {
                        showToast("success", res.message);
                        getPfAssign();
                        FormAssignClear();
                    } else {
                        showToast("warning", res.message);
                        getPfAssign();
                    }
                    //console.log(res);
                    
                },               
                error: function (error) {
                    showToast("error", error.message);
                }
            });
        } else {
            fromData.pfAssignID = pfId;
            //console.log($("#PFEmployeeId").val());
            fromData.employeeId = $("#PFEmployeeId").val();
            fromData.isUpdate = true;
            $.ajax({
                url: `/PFAssignEntry/CreateEditPFAssignEntry`,
                type: "POST",
                data: JSON.stringify(fromData),
                contentType: "application/json; charset=utf-8",
                success: (res) => {
                    if (res.isSuccess) {
                        showToast("success", res.message);
                        getPfAssign();
                        FormAssignClear();
                    } else {
                        showToast("warning", res.message);
                        getPfAssign();
                        FormAssignClear();
                    }

                },                
                error: function (error) {
                    showToast("error", error.message);
                }
            });
          
        }

       
    })
})

$(document).on('change', "#excelFileInput", function () {
    if (this.files && this.files.length > 0) {
        //console.log("click");
        $("#choosefileText").text(this.files[0].name);
    } else {
        $("#choosefileText").text('No file chosen');
    }
})




$("#excelUploadForm").submit(function (e) {
    e.preventDefault();
    var formData = new FormData(this);
    var comId = $("#companySelect").val();
    if (comId) {
        formData.append('CompanyCode', comId);
    }

    for (var pair of formData.entries()) {
        //console.log(pair[0] + ': ' + pair[1]);
    }

    $.ajax({
        url: `/PFAssignEntry/UploadExcel`,
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.isSuccss) {
                $("#excelFileInput").val("");
                $("#choosefileText").text("Choose File");
                showToast('success', res.message);
                //getEarnLeaveEmployee();
                getPfAssign();
            }
        },
        error: function (xhr, status, error) {
            $("#checkExcelFileFormate").text("Only .xlsx or .xls files are allowed").fadeIn().delay(500).fadeOut(5000);
        }
    });

});

//get pf assign entry employee data 
function getPfAssign() {
    $.ajax({
        url: `/PFAssignEntry/GetPfAssignData`,
        type: "GET",
        contentType: "application/json",
        success: function (res) {
            //console.log(res);
            loadPfAssignData(res);
        },
        error: function (error) {
            showToast('error', error.message);
        }
    })
}


function loadPfAssignData(res) {
    var tableDataItem = res.data.result;
    if ($.fn.DataTable.isDataTable("#PFAssign-grid-show")) {
        $("#PFAssign-grid-show").DataTable().clear().destroy();
    }

    var tableBody = $("#PFAssign-grid-body-show");
    tableBody.empty();
    $.each(tableDataItem, function (index, employee) {
        //console.log(employee);
        var row = $(`<tr class="empAssignRow" data-id="${employee.autoId}"></tr>`);
        row.append(`<td class="text-center" width="60"><input class="empEarnSelect" type="checkbox" data-id="${employee.autoId}" /></td>`);
        
        row.append(`<td class="text-center"  width="120"><a data-id='${employee.pfAssignID}'>` + employee.pfAssignID + '</a></td>');
        row.append('<td class="text-center"  width="150">' + employee.employeeId + '</td>');
        row.append('<td class="text-start">' + employee.employeeName + '</td>');
        row.append('<td class="text-start">' + employee.designation + '</td>');
        row.append('<td class="text-center"  width="92">' + employee.pfApprovedStatus + '</td>');
        row.append('<td class="text-center">' + employee.approvalRemark + '</td>');
        row.append('<td class="text-center">' + employee.efDateShow + '</td>');
        row.append('<td class="text-center">' + employee.entryUser + '</td>');
        tableBody.append(row);
    });
    $('#PFAssign-grid-show').DataTable({

        responsive: true,
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
        scrollY: "400px",
        scrollCollapse: true,
        language: {
            search: "🔍 Search:",
            searchPlaceholder: "Search here...",
            lengthMenu: "Show _MENU_ entries",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            paginate: {
                first: "First",
                previous: "Prev",
                next: "Next",
                last: "Last"
            },
            emptyTable: "No data available"
        },

    });
}

$(document).ready(function () {
    var listAssignEmp = [];

    // row click
    $(document).on('click', '.empAssignRow', function () {
        var autoId = $(this).data('id');
        listAssignEmp.push(autoId);
    });

    // top checkbox
    $("#PFAssign-grid-check-all").on('change', function () {
        var isChecked = $(this).is(':checked');
        $('#PFAssign-grid-body-show input[type="checkbox"]').prop('checked', isChecked);
    });

    // individual checkbox change → update top checkbox
    $(document).on('change', '#PFAssign-grid-body-show input[type="checkbox"]', function () {
        var total = $('#PFAssign-grid-body-show input[type="checkbox"]').length;
        var checked = $('#PFAssign-grid-body-show input[type="checkbox"]:checked').length;

        if (total === checked) {
            $("#PFAssign-grid-check-all").prop('checked', true);
        } else {
            $("#PFAssign-grid-check-all").prop('checked', false);
        }
    });

    // delete button click
    $("#js-pf-assign-entry-delete-confirm").click(function () {
        var autoId = $("#PFAutoIdDelete").val();
        //console.log(autoId);
        listAssignEmp = []; // prevent duplicate id accumulation
        $('#PFAssign-grid-body-show input[type="checkbox"]:checked').each(function () {
            listAssignEmp.push($(this).data('id'));
        });
        //console.log(autoId);

        if (autoId != '') {
            listAssignEmp.push(parseFloat(autoId));
        }
        if (listAssignEmp.length == 0) {
            showToast("info", "Select at least one employee");
            //alert("Select at least one employee");
            return;
        }

        $.ajax({
            url: `/PFAssignEntry/BulkDeleteEmpPFAssign`,
            type: "POST",
            data: { ids: listAssignEmp },
            traditional: true,
            success: function (res) {
                showToast("success", res.message);
                //getEarnLeaveEmployee();
                //FormEarnLeaveValueClear();
                FormAssignClear();
                getPfAssign();
            },
            error: function (error) {
                showToast("error", error.message);
            }
        });
    });
});


///edit get value



$(document).ready(function () {
    $(document).on('click', 'a[data-id]', function (e) {
        e.preventDefault();
        //console.log("samaul");

        $('#employee-PF-Assign-Entry-grid-body input[type="checkbox"]').prop('disabled', true);
        $("#pfAssign-check-all").prop('disabled', true);


        var id = $(this).data('id');
       
        $("#submitButton").text("Update");
        //$("#availedLeaveDay").css("border", "");
        //$("#balancedLeaveDay").css("border", "");
        //$("#checkFileFormate").fadeOut(500);

        $.ajax({
            url: `/PFAssignEntry/EditGetAssignValue`,
            type: "POST",
            data: { id: id },
            traditional: true,
            success: function (res) {
                //console.log(res.data);

                //$('#datepicker').flatpickr().setDate(res.weekendDate);
                //$('#remark').val(res.remarks);
                $('#hiddenPFAssignId').val(res.data.pfAssignID);
                $('#PFAutoIdDelete').val(res.data.autoId);
                $('#PFApprovedStatusSelect').val(res.data.pfApprovedStatus);
                $('#remarks').val(res.data.approvalRemark);
                $('#WEFDatePicker').val(res.data.efDateShow);
                $('#PFEmployeeId').val(res.data.employeeId);
                //$('#remarks').val(res.data.remarks);
                //$('#autoIdDelete').val(res.data.autoId);
                //loadAllFilterEmp();
            }, error: function (error) {
                console.log(error);
            }
        })
    })
})