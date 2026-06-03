//success, error, warning, info, question
// showToast('success', "save success");
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
    loadFilterEmp();    
    CurrentYear();
    getEarnLeaveEmployee();
});
let earnLeaveDataTable = null;

function CurrentYear() {
    var cYear = new Date().getFullYear();
    $("#yearpicker").val(cYear);
}


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

function loadFilterEmp() {
    //showLoading();
    var filterData = getAllFilterVal();
    //console.log(filterData);
    $.ajax({
        url: `/ManualEarnLeaveEntry/getAllFilterEmp`,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(filterData),
        success: function (res) {
            if (!res.isSuccess) {
                showToast('error', res.message);
                return;
            }
            loadTableData(res);
            //showToast('success', res.message);
            //console.log(res.data);
           
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
//function loadFilterEmp() {
//    //showLoading();
//    var filterData = getAllFilterVal();
//    //console.log(filterData);
//    $.ajax({
//        url: `/ManualEarnLeaveEntry/getAllFilterEmp`,
//        type: "POST",
//        contentType: "application/json",
//        data: JSON.stringify(filterData),
//        success: function (res) {
//            if (!res.isSuccess) {
//                showToast('error', res.message);
//                return;
//            }
//            //showToast('success', res.message);
//            //console.log(res.data);
//            $("#companySelect, #branchSelect, #divisionSelect, #departmentSelect, #designationSelect, #employeeSelect, #employeeStatus, #activityStatusSelect")
//                .off("change");
//            loadTableData(res);
//            //console.log(res);
//            const data = res.data;
//            //console.log(data.companies);
//            if (data.companies && data.companies.length > 0 && data.companies.some(x => x.code != null && x.name != null)) {
//                var Companys = data.companies;
//                //console.log(dataCompany);
//                var optCompany = $("#companySelect");
//                $.each(Companys, function (index, company) {
//                    if (company.code != null && company.name != null && optCompany.find(`option[value="${company.code}"]`).length === 0) {
//                        optCompany.append(`<option value="${company.code}">${company.name}</option>`);
//                    }
//                });
//                optCompany.multiselect('rebuild');
//            }
//            if (data.branches && data.branches.length > 0 && data.branches.some(b => b.code != null && b.name != null)) {
//                var dataBranch = data.branches;
//                var optBranch = $("#branchSelect");

//                $.each(dataBranch, function (index, item) {
//                    if (item.code != null && item.name != null && optBranch.find(`option[value="${item.code}"]`).length === 0) {
//                        optBranch.append(`<option value="${item.code}">${item.name}</option>`);
//                    }
//                });
//                optBranch.multiselect('rebuild');
//            }
//            if (data.departments && data.departments.length > 0 && data.departments.some(b => b.code != null && b.name != null)) {
//                var dataDepartments = data.departments;
//                var optDepartments = $("#departmentSelect");

//                $("#branchSelect").change(function () {
//                    optDepartments.empty();
//                })
//                $("#divisionSelect").change(function () {
//                    optDepartments.empty();
//                })
//                $.each(dataDepartments, function (index, item) {
//                    if (item.code != null && item.name != null && optDepartments.find(`option[value="${item.code}"]`).length === 0) {
//                        optDepartments.append(`<option value="${item.code}">${item.name}</option>`);
//                    }
//                });

//                optDepartments.multiselect('rebuild');
//            }


//            // Division
//            if (data.divisions && data.divisions.length > 0 && data.divisions.some(b => b.code != null && b.name != null)) {
//                var dataDivisions = data.divisions;
//                var optDivisions = $("#divisionSelect");

//                $("#branchSelect").change(function () {
//                    optDivisions.empty();
//                })
//                $.each(dataDivisions, function (index, item) {
//                    if (item.code != null && item.name != null && optDivisions.find(`option[value="${item.code}"]`).length === 0) {
//                        optDivisions.append(`<option value="${item.code}">${item.name}</option>`);
//                    }
//                });
//                optDivisions.multiselect('rebuild');
//            }




//            // Designation
//            if (data.designations && data.designations.length > 0 && data.designations.some(b => b.code != null && b.name != null)) {
//                var dataDesignations = data.designations;
//                var optDesignations = $("#designationSelect");
//                $("#branchSelect").change(function () {
//                    optDesignations.empty();
//                })
//                $("#divisionSelect").change(function () {
//                    optDesignations.empty();
//                })
//                $("#departmentSelect").change(function () {
//                    optDesignations.empty();
//                })
//                $.each(dataDesignations, function (index, item) {
//                    if (item.code != null && item.name != null && optDesignations.find(`option[value="${item.code}"]`).length === 0) {
//                        optDesignations.append(`<option value="${item.code}">${item.name}</option>`);
//                    }
//                });
//                optDesignations.multiselect('rebuild');
//            }

//            if (data.employees && data.employees.length > 0 && data.employees.some(b => b.code != null && b.name != null)) {
//                var dataEmployees = data.employees;
//                var optEmployees = $("#employeeSelect");
//                $("#branchSelect").change(function () {
//                    optEmployees.empty();
//                })
//                $("#divisionSelect").change(function () {
//                    optEmployees.empty();
//                })
//                $("#departmentSelect").change(function () {
//                    optEmployees.empty();
//                })
//                $("#designationSelect").change(function () {
//                    optEmployees.empty();
//                })
//                $.each(dataEmployees, function (index, item) {
//                    if (item.code != null && item.name != null && optEmployees.find(`option[value="${item.code}"]`).length === 0) {
//                        optEmployees.append(`<option value="${item.code}">${item.name}<b> ( ${item.code} )</b></option>`);
//                    }
//                });
//                optEmployees.multiselect('rebuild');
//            }

//            $("#companySelect, #branchSelect, #divisionSelect, #departmentSelect, #designationSelect, #employeeSelect, #employeeStatus, #activityStatusSelect")
//                .on("change", function () {
//                    loadFilterEmp();
//                });
//        },
//        complete: function () {
//            hideLoading();
//        },
//        error: function (error) {
//            showToast("error", error.message);
//            hideLoading();
//        }
//    })
//}

function loadTableData(res) {
    //console.log(res.data.employees);
    var tableData = res.data.employees;
    if (earnLeaveDataTable !== null) {
        earnLeaveDataTable.destroy();
    }
    var tableBody = $("#employee-earn-leave-grid-body");
    tableBody.empty();
    $.each(tableData, function (index, employee) {
        //console.log(employee);
        var row = $('<tr>');
        row.append('<td class="text-center" style="width:60px !important;"><input type="checkbox" /></td >');
        row.append('<td class="text-center">' + employee.code + '</td>');
        row.append('<td class="text-center">' + employee.name + '</td>');
        row.append('<td class="text-center">' + employee.designation + '</td>');
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
    earnLeaveDataTable = $("#employee-earn-leave-grid").DataTable({
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
//clear all
//$(document).on('click', "#js-emp-earn-leave-clear", function () {
//    console.log("click");
//})
$("#js-emp-earn-leave-clear").click(function () {
    FormEarnLeaveValueClear();
});


$(document).ready(function () {
    $('#yearpicker').datepicker({
        format: "yyyy",
        viewMode: "years",
        minViewMode: "years",
        autoclose: true
    }).on('changeDate', function (e) {
        const selectedYear = e.format(0, "yyyy");
        //console.log("Selected Year: " + selectedYear);
    });
});


function FormEarnLeaveValue() {
    var fromData = {
        Year: $("#yearpicker").val(),
        GrantedLeaveDays: parseFloat($("#grantedLeavDay").val()), 
        AvailedLeaveDays: parseFloat($("#availedLeaveDay").val()),
        BalancedLeaveDays: parseFloat($("#balancedLeaveDay").val()),
        Remarks: $("#remarks").val(),
        CompanyCode: $("#companySelect").val()
    };
    return fromData;
}

function FormEarnLeaveValueClear() {
    //hiddenEarnId
    //autoIdDelete
    $("#hiddenEarnId").val('');
    $("#autoIdDelete").val('');
    $("#grantedLeavDay").val('');
    $("#availedLeaveDay").val('');
    $("#balancedLeaveDay").val('');
    $("#remarks").val('');
    $("#companyCode").val('');
    $("#submitButton").text("Save");
    CurrentYear();
    $('#employee-earn-leave-grid-body input[type="checkbox"]').prop('disabled', false);
    $("#earn-leave-employee-check-all").prop('disabled', false);

    $('#employee-earn-leave-grid-body input[type="checkbox"]').prop('checked', false);
    $("#earn-leave-employee-check-all").prop('checked', false);
    //todo
    $('#employee-earn-leave-grid-body-show input[type="checkbox"]').prop('checked', false);
    $("#employee-earn-leave-check-all").prop('checked', false);

}


$(document).ready(function () {
    $("#earn-leave-employee-check-all").on('change', function () {
        var isChecked = $(this).is(':checked');
        $('#employee-earn-leave-grid-body input[type="checkbox"]').prop('checked', isChecked);
    })
    // individual checkbox change → update top checkbox
    $(document).on('change', '#employee-earn-leave-grid-body input[type="checkbox"]', function () {
        var total = $('#employee-earn-leave-grid-body input[type="checkbox"]').length;
        var checked = $('#employee-earn-leave-grid-body input[type="checkbox"]:checked').length;

        if (total === checked) {
            $("#earn-leave-employee-check-all").prop('checked', true);
        } else {
            $("#earn-leave-employee-check-all").prop('checked', false);
        }
    });

    $(document).on('input', "#grantedLeavDay", function () {
        var gDay = parseInt($("#grantedLeavDay").val());
        $("#grantedLeavDay").css("border", "");
        if (!isNaN(gDay)) {
            //$("#availedLeaveDay").attr('placeholder',"0");
            $("#availedLeaveDay").val(0);
            $("#balancedLeaveDay").val(0); 
        } else {
            $("#availedLeaveDay").val('');
            $("#balancedLeaveDay").val('');      
        }
    });
    $(document).on('input', "#yearpicker", function () {
        var year = parseInt($("#yearpicker").val());
        if (year < 1900 || year > 2100 ) {
            $("#yearpicker").css("border", "1px solid red");
            $(".js-emp-manual-earn-leave-save").prop("disabled", true);
        }else {
            $("#yearpicker").css("border", "");
            $(".js-emp-manual-earn-leave-save").prop("disabled", false);
        }      
    });

    $(document).on('input', "#grantedLeavDay, #availedLeaveDay", function () {
        var gDay = parseInt($("#grantedLeavDay").val());
        var aDay = parseInt($("#availedLeaveDay").val());    
        if (isNaN(aDay)) {
            //$("#availedLeaveDay").val(0); 
            //$("#availedLeaveDay").attr('placeholder', "0"); 
            $("#availedLeaveDay , #balancedLeaveDay").css("border", "");
            $("#checkFileFormate").fadeOut(500);
            $(".js-emp-manual-earn-leave-save").prop("disabled", false);
        }
      
        if (!isNaN(gDay) && !isNaN(aDay)) {
            if (aDay > gDay) {
                //showToast('warning', "Availed Leave Day cannot be greater than Granted Leave Day");
                $("#balancedLeaveDay").val('');
                $("#availedLeaveDay , #balancedLeaveDay").css("border", "1px solid red");
                //$("#balancedLeaveDay").css("background-color", "lightcoral");
                $("#checkFileFormate").fadeIn(500);
                $(".js-emp-manual-earn-leave-save").prop("disabled", true);
                //console.log("availedLeaveDay");

            } else {
                var bDay = gDay - aDay;
                $("#balancedLeaveDay").val(bDay);
                $("#availedLeaveDay").css("border", "");
                $("#balancedLeaveDay").css("border", "");
                $("#checkFileFormate").fadeOut(500);
                $(".js-emp-manual-earn-leave-save").prop("disabled", false);
            }
        } else {
           
            $("#balancedLeaveDay").val(gDay);
            $("#availedLeaveDay").css("border", "");
        }
    });


    $(".js-emp-manual-earn-leave-save").click(function () {



        var formData = FormEarnLeaveValue();
        var selectEmployeeIds = [];

        var earnLeaveId = $("#hiddenEarnId").val();


        if (earnLeaveId === "") {
            $('#employee-earn-leave-grid-body input[type="checkbox"]:checked').each(function () {
                var row = $(this).closest('tr');
                var empId = row.find('td:nth-child(2)').text().trim();
                selectEmployeeIds.push(empId);
            });

            if (selectEmployeeIds.length === 0) {
                showToast('warning', "Please select at least one employee.");
                var gDay = $("#grantedLeavDay").val();
                console.log(gDay);
                if (isNaN(gDay)) {
                    $("#grantedLeavDay").css("border", "1px solid red").focus();
                }
                return;
            }

            formData.EmployeeID = selectEmployeeIds;  // property name match
            //console.log(formData);

            $.ajax({
                url: `/ManualEarnLeaveEntry/CreateManualEarnLeave`,
                type: "POST",
                data: JSON.stringify(formData),
                contentType: "application/json; charset=utf-8",
                success: function (res) {
                    console.log(res.message);
                    console.log(res.message.substring(0, 12));
                    if (res.success) {
                        showToast('success', res.message);
                        getEarnLeaveEmployee();
                        $('#employee-earn-leave-grid-body input[type="checkbox"]').prop('checked', false);
                        $("#earn-leave-employee-check-all").prop('checked', false);
                        FormEarnLeaveValueClear();
                    } else if (res.message.substring(0, 12) == "Invalid year")
 {
                        showToast('warning', res.message);
                        $("#yearpicker").css("border", "1px solid red").focus();
                    } else {
                        showToast('warning', res.message);
                        $("#grantedLeavDay").css("border", "1px solid red").focus();
                    }
                   
                },
                error: function (error) {
                    //console.log(error);
                    showToast('error', "An error occurred while saving.");
                }
            });

        } else {
           
            formData.EarnLeaveID = earnLeaveId;
            formData.isUpdate = true;
           
            $.ajax({
                //url: `/ManualEarnLeaveEntry/UpdateEarnLeaveEmployee`,
                url: `/ManualEarnLeaveEntry/CreateManualEarnLeave`,
                type: 'POST',
                data: JSON.stringify(formData),
                contentType:'application/json',
                success: function (res) {
                   
                    if (res.success) {
                        showToast('success', res.message);
                        getEarnLeaveEmployee();
                        FormEarnLeaveValueClear();
                        $("#submitButton").text("Save");
                    } else if (res.message.substring(0, 12) == "Invalid year") {
                        showToast('warning', res.message);
                        $("#yearpicker").css("border", "1px solid red").focus();
                    } else {
                        showToast('warning', res.message);
                        $("#grantedLeavDay").css("border", "1px solid red").focus();
                    }
                   
                },
                error: function (error) {
                    $("#massageError").html(`<i class="fa fa-times-circle text-danger  px-2"></i>` + error.message).fadeIn().delay(500).fadeOut(5000);
                }
            });
        }
    });
})


//get earn leave employee data 
function getEarnLeaveEmployee() {
    $.ajax({
        url: `/ManualEarnLeaveEntry/GetEarmLeaveEmployee`,
        type: "GET",
        contentType: "application/json",
        success: function (res) {
            //console.log(res);
            loadEarnLeaveEmployee(res);
        },
        error: function (error) {
            showToast('error', error.message);
        }
    })
}
function loadEarnLeaveEmployee(res) {
    var tableDataItem = res.data.result;
    if ($.fn.DataTable.isDataTable("#employee-earn-leave-grid-show")) {
        $("#employee-earn-leave-grid-show").DataTable().clear().destroy();
    }

    var tableBody = $("#employee-earn-leave-grid-body-show");
    tableBody.empty();
    $.each(tableDataItem, function (index, employee) {
        //console.log(employee);
        var row = $(`<tr class="empEarnRow" data-id="${employee.autoId}"></tr>`);
        row.append(`<td class="text-center" width="60"><input class="empEarnSelect" type="checkbox" data-id="${employee.autoId}" /></td>`);
        //row.append('<td class="text-center">' + employee.earnLeaveID + '</td>');
        row.append(`<td class="text-center"><a data-id='${employee.earnLeaveID}'>` + employee.earnLeaveID + '</a></td>');
        row.append('<td class="text-center">' + employee.empId + '</td>');
        row.append('<td class="text-start">' + employee.employeeName + '</td>');
        row.append('<td class="text-start">' + employee.designation + '</td>');
        row.append('<td class="text-center"  width="92">' + employee.year + '</td>');
        row.append('<td class="text-center">' + employee.grantedLeaveDays + '</td>');
        row.append('<td class="text-center">' + employee.availedLeaveDays + '</td>');
        row.append('<td class="text-center">' + employee.balancedLeaveDays + '</td>');
        row.append('<td class="text-center">' + employee.remarks + '</td>');
        row.append('<td class="text-center">' + employee.entryUser + '</td>');
        tableBody.append(row);
    });
    $('#employee-earn-leave-grid-show').DataTable({

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
//$(document).ready(function () {
//    $('#excelFileInput').on('change', function () {
//        if (this.files && this.files.length > 0) {
//            $("#choosefileText").text(this.files[0].name);
//        } else {
//            $("#choosefileText").text('No file chosen');
//        }
//    });
//});
$(document).on('change', "#excelFileInput", function () {
    if (this.files && this.files.length > 0) {
        console.log("click");
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
        url: `/ManualEarnLeaveEntry/UploadExcel`,
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.isSuccss) {
                $("#excelFileInput").val("");
                $("#choosefileText").text("Choose File");   
                showToast('success', res.message);
                getEarnLeaveEmployee();
            } 
        },
        error: function (xhr, status, error) {
            $("#checkExcelFileFormate").text("Only .xlsx or .xls files are allowed").fadeIn().delay(500).fadeOut(5000);
        }
    });

});
$(document).ready(function () {
    var listWekEmp = [];

    // row click
    $(document).on('click', '.empEarnRow', function () {
        var autoId = $(this).data('id');
        listWekEmp.push(autoId);
    });

    console.log(listWekEmp);

    // top checkbox
    $("#employee-earn-leave-check-all").on('change', function () {
        var isChecked = $(this).is(':checked');
        $('#employee-earn-leave-grid-body-show input[type="checkbox"]').prop('checked', isChecked);
    });

    // individual checkbox change → update top checkbox
    $(document).on('change', '#employee-earn-leave-grid-body-show input[type="checkbox"]', function () {
        var total = $('#employee-earn-leave-grid-body-show input[type="checkbox"]').length;
        var checked = $('#employee-earn-leave-grid-body-show input[type="checkbox"]:checked').length;

        if (total === checked) {
            $("#employee-earn-leave-check-all").prop('checked', true);
        } else {
            $("#employee-earn-leave-check-all").prop('checked', false);
        }
    });

    // delete button click
    $("#js-emp-earn-leave-delete-confirm").click(function () {
        var autoId = $("#autoIdDelete").val();
        
        listWekEmp = []; // prevent duplicate id accumulation
        $('#employee-earn-leave-grid-body-show input[type="checkbox"]:checked').each(function () {
            listWekEmp.push($(this).data('id'));
        });
        console.log(autoId);
        
        if (autoId !='') {
            listWekEmp.push(parseFloat(autoId));
        }
        console.log(listWekEmp);
        if (listWekEmp.length == 0) {
            showToast("info", "Select at least one employee");
            //alert("Select at least one employee");
            return;
        }

        $.ajax({
            url: `/ManualEarnLeaveEntry/BulkDeleteEmpWeelend`,
            type: "POST",
            data: { ids: listWekEmp },
            traditional: true,
            success: function (res) {
                showToast("success", res.message);
                getEarnLeaveEmployee();
                FormEarnLeaveValueClear();
            },
            error: function (error) {
                showToast("error", error.message);
            }
        });
    });
});


//edit


$(document).ready(function () {
    $(document).on('click', 'a[data-id]', function (e) {
        e.preventDefault();

        $('#employee-earn-leave-grid-body input[type="checkbox"]').prop('disabled', true);
        $("#earn-leave-employee-check-all").prop('disabled', true);


        var id = $(this).data('id');
        $("#submitButton").text("Update");
        $("#availedLeaveDay").css("border", "");
        $("#balancedLeaveDay").css("border", "");
        $("#checkFileFormate").fadeOut(500);
        $.ajax({
            url: `/ManualEarnLeaveEntry/EditEarnLeaveEmployee`,
            type: "POST",
            data: { id: id },
            traditional: true,
            success: function (res) {
                console.log(res.data);

                //$('#datepicker').flatpickr().setDate(res.weekendDate);
                //$('#remark').val(res.remarks);
                $('#hiddenEarnId').val(res.data.earnLeaveID);
                $('#yearpicker').val(res.data.year);
                $('#grantedLeavDay').val(res.data.grantedLeaveDays);
                $('#availedLeaveDay').val(res.data.availedLeaveDays);
                $('#balancedLeaveDay').val(res.data.balancedLeaveDays);
                $('#remarks').val(res.data.remarks);
                $('#autoIdDelete').val(res.data.autoId);
                //loadAllFilterEmp();
            }, error: function (error) {
                console.log(error);
            }
        })
    })
})