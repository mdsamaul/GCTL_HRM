$(document).ready(function () {


    fetchEntryId();


    const currentDate = new Date();

    // Get current month and year
    const currentMonth = currentDate.getMonth() + 1; // Months are zero-indexed (0 = January)
    const currentYear = currentDate.getFullYear();

    // Preselect the current month
    $("#month").val(currentMonth);

    // Preselect the current year
    $("#year").val(currentYear);


    $('#refDate, #wef').datepicker({
        format: 'dd/mm/yyyy', // Change format as needed
        autoclose: true,     // Close picker automatically after selection
        todayHighlight: true // Highlight today's date
    });

    // Set default date to today's date
    $('#refDate').datepicker('setDate', new Date());


    selectTo();

    function selectTo() {
        $('#repositionForm' + ' .selectpickerRepo').select2({
            language: {
                noResults: function () {

                }
            },
            escapeMarkup: function (markup) {
                return markup;
            }
        });
    }



    // #region DD


    getCompany();



    fetchDropdownData('/LeaveApplicationEntry/GetBranchesMultiComp', { companyCode: null, isAll: true }, 'branchDropdown1', 'branchCode', 'branchName');
    fetchDropdownData('/LeaveApplicationEntry/GetDeptMultiCompBranch', { companyCode: null, branchCode: null, isAll: true }, 'deptDropdown1', 'departmentCode', 'departmentName');
    fetchDropdownData('/LeaveApplicationEntry/GetDesigMultiCompBranchDept', { companyCode: null, branchCode: null, departmentCode: null, isAll: true }, 'designationDropdown1', 'designationCode', 'designationName');



    // Company dropdown change handler
    $('#companyDropdown1').on('changed.bs.select', function () {
        var selectedCompanies = $(this).val();
        if (selectedCompanies && selectedCompanies.length > 0) {
            fetchDropdownData('/LeaveApplicationEntry/GetBranchesMultiComp', { companyCode: selectedCompanies, isAll: false }, 'branchDropdown1', 'branchCode', 'branchName');
            fetchDropdownData('/LeaveApplicationEntry/GetDeptMultiCompBranch', { companyCode: selectedCompanies, branchCode: null, isAll: false }, 'deptDropdown1', 'departmentCode', 'departmentName');
            fetchDropdownData('/LeaveApplicationEntry/GetEmpMultiCompBranchDept', { companyCode: selectedCompanies, branchCode: null, departmentCode: null, isAll: false }, 'employeeDropdown1', 'employeeId', 'employeeFirstName');


        } else {
            fetchDropdownData('/LeaveApplicationEntry/GetBranchesMultiComp', { companyCode: null, isAll: true }, 'branchDropdown1', 'branchCode', 'branchName');
            fetchDropdownData('/LeaveApplicationEntry/GetDeptMultiCompBranch', { companyCode: null, branchCode: null, isAll: true }, 'deptDropdown1', 'departmentCode', 'departmentName');
            fetchDropdownData('/LeaveApplicationEntry/GetEmpMultiCompBranchDept', { companyCode: null, branchCode: null, departmentCode: null, isAll: true }, 'employeeDropdown1', 'employeeId', 'employeeFirstName');


        }
    });

    // Company dropdown change handler
    $('#branchDropdown1').change(function () {
        var compCode = $('#companyDropdown1').val();
        var selectedBranches = $(this).val();
        if (selectedBranches && selectedBranches.length > 0) {
            fetchDropdownData('/LeaveApplicationEntry/GetDeptMultiCompBranch', { companyCode: compCode, branchCode: selectedBranches, isAll: false }, 'deptDropdown1', 'departmentCode', 'departmentName');
            fetchDropdownData('/LeaveApplicationEntry/GetEmpMultiCompBranchDept', { companyCode: compCode, branchCode: null, departmentCode: null, isAll: false }, 'employeeDropdown1', 'employeeId', 'employeeFirstName');

        } else {


            fetchDropdownData('/LeaveApplicationEntry/GetDeptMultiCompBranch', { companyCode: null, branchCode: null, isAll: true }, 'deptDropdown1', 'departmentCode', 'departmentName');
            fetchDropdownData('/LeaveApplicationEntry/GetEmpMultiCompBranchDept', { companyCode: null, branchCode: null, departmentCode: null, isAll: true }, 'employeeDropdown1', 'employeeId', 'employeeFirstName');



        }
    });

    // Company dropdown change handler
    $('#deptDropdown1').change(function () {

        var branchCode = $('#branchDropdown1').val();
        var compCode = $('#companyDropdown1').val();
        var selectedDepartments = $(this).val();
        if (selectedDepartments && selectedDepartments.length > 0) {
            //getDepartments(selectedBranches, false);
            fetchDropdownData('/LeaveApplicationEntry/GetEmpMultiCompBranchDept', { companyCode: compCode, branchCode: branchCode, departmentCode: selectedDepartments, isAll: false }, 'employeeDropdown1', 'employeeId', 'employeeFirstName');
            fetchDropdownData('/LeaveApplicationEntry/GetDesigMultiCompBranchDept', { companyCode: compCode, branchCode: branchCode, departmentCode: selectedDepartments, isAll: false }, 'designationDropdown1', 'designationCode', 'designationName');

        } else {

            fetchDropdownData('/LeaveApplicationEntry/GetEmpMultiCompBranchDept', { companyCode: null, branchCode: null, departmentCode: null, isAll: true }, 'employeeDropdown1', 'employeeId', 'employeeFirstName');
            fetchDropdownData('/LeaveApplicationEntry/GetDesigMultiCompBranchDept', { companyCode: null, branchCode: null, departmentCode: null, isAll: true }, 'designationDropdown1', 'designationCode', 'designationName');

        }
    });

    function getCompany() {
        $.ajax({
            url: '/LeaveApplicationEntry/GetCompanies',
            type: 'GET',
            success: function (company) {
                console.log(company)
                if (company.result && company.result.length > 0) {
                    var $companyDropdown = $('#companyDropdown1');
                    $companyDropdown.empty();



                    $.each(company.result, function (index, com) {
                        $companyDropdown.append(
                            `<option value="${com.companyCode}">${com.companyName}</option>`
                        );
                    });

                    initializeSelectPicker('companyDropdown1');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error fetching companies:', error);
                var $companyDropdown = $('#companyDropdown1');
                $companyDropdown.empty();
                $companyDropdown.append('<option value="">Error loading companies</option>');
                initializeSelectPicker('companyDropdown1');
            }
        });
    }


    function fetchDropdownData(endpoint, params, dropdownId, valueField, textField) {
        var $dropdown = $('#' + dropdownId);
        $dropdown.empty();
        $dropdown.append('<option value="">Loading...</option>');
        $dropdown.selectpicker('refresh');

        $.ajax({
            url: endpoint,
            type: 'GET',
            traditional: true,
            data: params,
            success: function (response) {
                $dropdown.empty();

                //console.log('this is from fetchDDdata :: ', response)

                if (response.result && response.result.length > 0) {
                    response.result.forEach(function (item) {
                        $dropdown.append(`<option value="${item[valueField]}">${item[textField]} (${item[valueField]})</option>`);
                    });
                } else {
                    $dropdown.append('<option value="">No data available</option>');
                }

                initializeSelectPicker(dropdownId);
            },
            error: function (xhr, status, error) {
                console.error('Error fetching data:', error);
                $dropdown.empty();
                $dropdown.append('<option value="">Error loading data</option>');
                initializeSelectPicker(dropdownId);
            }
        });
    }

    function initializeSelectPicker(elementId) {
        var $element = $('#' + elementId);

        // Destroy if exists
        if ($element.data('selectpicker')) {
            $element.selectpicker('destroy');
        }

        // Initialize with options
        $element.selectpicker({
            liveSearch: true,
            enableSelectedText: true,
            liveSearchPlaceholder: 'Search...',
            size: 10,
            selectedTextFormat: 'count',
            actionsBox: true,
            iconBase: 'fa',
            showTick: true,
            tickIcon: 'fa-check',
            container: 'body'
        });

        // Refresh to ensure proper rendering
        $element.selectpicker('refresh');
    }

    //$(function () {
    //    $("#dateFrom, #dateTo, #dateToStr").datepicker({
    //        dateFormat: "dd/mm/yy"
    //    });
    //});





    // #endregion

    //#region table

    let currentPage = 1;
    let pageSize = 10;
    let searchTerm = "";

    function loadRepositions() {
        $.ajax({
            url: `/reposition/paged?page=${currentPage}&pageSize=${pageSize}&searchTerm=${searchTerm}`,
            type: "GET",
            success: function (response) {

                console.log('/reposition/paged', response)
                $("#repositionTable tbody").empty();

                if (response.data.length === 0) {
                    $("#repositionTable tbody").append(`<tr><td colspan="10">No records found</td></tr>`);
                    return;
                }

                $.each(response.data, function (index, item) {
                    $("#repositionTable tbody").append(`
                       

                        <tr>
                            <td><input type="checkbox" class="row-checkbox"></td>
                     
                            <td> <button class="edit-btn1 btn btn-link" data-id="${item.repositionId}">${item.repositionId}</button></td>
                            <td>${item.employeeId}</td>
                            <td>${item.name}</td>
                            <td>${item.type}</td>
                            <td>${item.year}</td>
                            <td>${item.wef}</td>
                            <td style="display:none">${item.prevCompany}</td>
                            <td>${item.currCompany}</td>
                            <td>${item.prevDept}</td>
                            <td>${item.currDept}</td> 
                            <td>${item.previousBranchName}</td> 
                            <td>${item.currentBranchName}</td> 
                            <td>${item.prevDesig}</td>
                            <td>${item.currDesig}</td>
                            <td>${item.promotionAmount}</td>
                            <td>${item.previousSalary}</td>
                            <td>${item.currentSalary}</td>
                            <td>${item.remark}</td>
                        </tr>
                    `);
                });

                updatePagination(response.totalRecords);
            },
            error: function () {
                alert("Error fetching data");
            }
        });
    }

    function updatePagination(totalRecords) {
        let totalPages = Math.ceil(totalRecords / pageSize);
        let paginationHtml = "";
        let startPage = Math.max(1, currentPage - 2);
        let endPage = Math.min(totalPages, currentPage + 2);

        if (startPage > 1) paginationHtml += `<button class="page-btn" data-page="1">1</button> ... `;

        for (let i = startPage; i <= endPage; i++) {
            paginationHtml += `<button class="page-btn ${i === currentPage ? 'active' : ''}" data-page="${i}">${i}</button>`;
        }

        if (endPage < totalPages) paginationHtml += ` ... <button class="page-btn" data-page="${totalPages}">${totalPages}</button>`;

        $("#pagination").html(paginationHtml);

        $(".page-btn").click(function () {
            currentPage = parseInt($(this).data("page"));
            loadRepositions();
        });

        let startRecord = (currentPage - 1) * pageSize + 1;
        let endRecord = Math.min(currentPage * pageSize, totalRecords);
        $("#tableInfo").text(`Showing ${startRecord} to ${endRecord} of ${totalRecords} records`);
    }

    $("#searchInput").on("keyup", function () {
        searchTerm = $(this).val();
        currentPage = 1;
        loadRepositions();
    });

    $("#pageSizeSelect").on("change", function () {
        pageSize = parseInt($(this).val());
        currentPage = 1;
        loadRepositions();
    });

    $("#selectAll").click(function () {
        $(".row-checkbox").prop("checked", this.checked);
    });

    loadRepositions();

    //#endregion


    $(document).on('click', '.edit-btn1', function () {
        var repositionId = $(this).data('id'); // Get the repositionId from the button's data-id attribute
        console.log('Edit button clicked for repositionId: ' + repositionId);

        // Make the AJAX call for editing
        $.ajax({
            url: '/reposition/eiditt',  // Replace with your actual API URL
            method: 'GET',
            data: {
                repositionId: repositionId
                // You can send additional data if needed, like other details of the row
            },
            success: function (response) {
                //console.log('AJAX success:', response);
                if (response.success) {
                    populateFormDate(response.data)
                }
                else {
                    toastr.warning(response.message)
                }
                // You can add further actions like showing a modal for editing, updating the table, etc.
            },
            error: function (error) {
                console.error('Error with AJAX:', error);
            }
        });
    });



    function populateFormDate(data) {
        console.log(data);

        // First, set the hidden fields and non-dropdown fields
        $('#tc').val(data.tc);
        $('#repoCode').val(data.repositionId);
        $('#repoType').val(data.type);
        $('#month').val(data.monthName).trigger('change');
        $('#year').val(data.year);
        $('#wef').val(data.wef.split('T')[0]);
        $('#pAmount').val(data.promotionAmount);
        $('#refNo').val(data.refLetterNo);
        $('#refDate').val(data.refLetterDate.split('T')[0]);
        $('#remrks').val(data.remark);
       // $('#createOn').val(data.ldate.split('T')[0]);
      //  $('#updateOn').val(data.modifyDate.split('T')[0]);

        // Handle company dropdown first, then use callbacks for dependent dropdowns
        $('#companyDropdown').val(data.preCompanyCode2).trigger('change');

        // Wait for employee dropdown to be populated after company change
        setTimeout(function () {
            $('#employeeDropdown').val(data.employeeId).trigger('change');

            // Set current company related dropdowns
            setTimeout(function () {
                // Set company dropdown for the new position
                $('#companyDropdown1').val(data.currCompanyCode2).trigger('change');

                // Wait for branch dropdown to be populated
                setTimeout(function () {
                    $('#branchDropdown1').val(data.currBranchCode).trigger('change');

                    // Wait for department dropdown to be populated
                    setTimeout(function () {
                        $('#deptDropdown1').val(data.currDepartmentCode).trigger('change');

                        // Wait for designation dropdown to be populated
                        setTimeout(function () {
                            $('#designationDropdown1').val(data.currDesignationCode).trigger('change');
                            $('#gradeDropdown1').val(data.currentGradeCode).trigger('change');
                        }, 300);
                    }, 300);
                }, 300);
            }, 300);
        }, 300);
    }


   

   

    function fetchEntryId() {
        $.ajax({
            url: '/separation/getEntryId', // Replace with your actual controller name
            type: 'GET',
            success: function (response) {
                // Set the entryId to the input field
                $('#repoCode').val(response.entryId.result);
                console.log('new entry Id : ', response)
            },
            error: function (xhr, status, error) {
                console.error('Error fetching entryId:', error);
            }
        });
    }



    // Company Dropdown Change Event
    $('#companyDropdown').on('change', function () {
        var companyCode = $(this).val();
        GetEmployees(companyCode)
    });


    function GetEmployees(companyCode) {
        if (companyCode) {
            $.ajax({
                url: '/employee/employeeByCompany',
                type: 'GET',
                data: { compCode: companyCode },
                success: function (response) {
                    console.log('GetEmployeeByCode---- : ', response)
                    $('#employeeDropdown').empty().append('<option value="">--Select Employee--</option>');
                    if (response.data && response.data.length > 0) {
                        response.data.forEach(function (employee) {
                            $('#employeeDropdown').append(`<option value="${employee.employeeId}">${employee.employeeId} - ${employee.firstName}</option>`);
                        });
                    }

                },
                error: function () {
                    toastr.warning('Failed to load employees.', 'Message');

                    //alert('Failed to load employees.');
                }
            });
        } else {
            $('#employeeDropdown').empty().append('<option value="">--Select Employee--</option>');
        }
    }


    $('#employeeDropdown').on('change', function () {
        var selectedEmpId = $(this).val();

        GetEmpDetails(selectedEmpId, true)


    });

    function GetEmpDetails(selectedEmpId, restrict) {

        if (selectedEmpId) {
            $.ajax({
                url: '/employ/getInfo',
                type: 'GET',
                data: { EmpId: selectedEmpId },
                success: function (response) {
                    console.log('/employ/getInfo', response)
                    if (response.success) {
                        // Populate employee details
                        $('#employeeName').val(response.data.result.employeeFirstName + ' ' + response.data.result.employeeLastName);
                        $('#designation').val(response.data.result.designationName);
                        $('#department').val(response.data.result.departmentName);

                        $('#serviceLength').val(response.data.result.serviceLength);

                        $('#confirmationDate').val(response.data.result.confirmationDate);
                        $('#joinDate').val(response.data.result.joiningDate);
                        $('#grade').val();
                        $('#grossSalary').val(response.data.result.grossSalary);



                        if (restrict) {
                            var rest = response.data.result;

                            $('#designationDropdown1').val(rest.designationCode).trigger('change');

                            $('#deptDropdown1').val(rest.departmentCode).trigger('change');
                            $('#branchDropdown1').val(rest.branchCode).trigger('change');



                            // Handle Designation details
                            var designationOption = rest.designationName
                                ? `<option value="${rest.designationCode}">${rest.designationName}</option>`
                                : '<option value="">--Select--</option>';

                            // Handle Department details
                            var departmentOption = rest.departmentName
                                ? `<option value="${rest.departmentCode}">${rest.departmentName}</option>`
                                : '<option value="">--Select--</option>';


                            var companyOption = rest.companyName
                                ? `<option value="${rest.companyCode}">${rest.companyName}</option>`
                                : '<option value="">--Select--</option>';

                            var branchOption = rest.branchName
                                ? `<option value="${rest.branchCode}">${rest.branchName}</option>`
                                : '<option value="">--Select--</option>';

                            // Update dropdowns
                            //$('#designationDropdown1').empty().append(designationOption);
                            //$('#deptDropdown1').empty().append(departmentOption);
                            //$('#companyDropdown1').empty().append(companyOption);
                            //$('#branchDropdown1').empty().append(branchOption);
                        }

                        





                    } else {
                        toastr.warning('Failed to fetch employee details.', 'Message');

                        // alert('Failed to fetch employee details.');
                    }
                },
                error: function () {
                    toastr.warning('Error fetching employee details.', 'Message');

                    // alert('Error fetching employee details.');
                }
            });
        } else {
            // Clear fields if no employee selected
            clearEmployeeFields();
        }
    }

    // Function to clear employee fields
    function clearEmployeeFields() {
        $('#employeeName').val('');
        $('#designation').val('');
        $('#department').val('');
        $('#serviceLength').val('');
        $('#confirmationDate').val('');
        $('#joinDate').val('');
        $('#grade').val('');
        $('#grossSalary').val('');
        $('#immediateSupervisor').empty().append('<option value="">--Select Supervisor--</option>');
        $('#headOfDepartment').empty().append('<option value="">--Select HOD--</option>');
    }

   
  

    //------------------------------------

    // Form Submission
    // Replace the existing form submission with this:
   


    $("#repositionForm").submit(function (event) {
        event.preventDefault(); // Prevent default form submission

        var formData = {
           


            RepoId: $("#repoId").val() ? parseInt($("#repoId").val(), 10) : 0, // Default to 0 if null or empty
            RepoCode: $("#repoCode").val() || "", // Default to empty string
            RepoType: $("#repoType").val() || "",
            Month: $("#month").val(), // ? parseInt($("#month").val(), 10) : 1, // Default to 1
            Year: $("#year").val() ? parseInt($("#year").val(), 10) : new Date().getFullYear(), // Default to current year

            Wef: $("#wef").val() ? new Date($("#wef").val()).toISOString() : null,
            RefDate: $("#refDate").val() ? new Date($("#refDate").val()).toISOString() : null,

            PromotionAmount: $("#pAmount").val() ? parseFloat($("#pAmount").val()) : 0.0, // Default to 0.0
            RefNo: $("#refNo").val() || "",
            Remarks: $("#remrks").val() || "",
            CompanyCode: $("#companyDropdown1").val() || "",
            Branch: $("#branchDropdown1").val() || "",
            Designation: $("#designationDropdown1").val() || "",
            Department: $("#deptDropdown1").val() || "",
            Grade: $("#gradeDropdown1").val() || "",
            EmployeeId: $("#employeeDropdown").val() || "",

            ExpatriateMedical:  0.0,
            ExpatriateConveyance:  0.0,
            ExpatriateHouseRent:  0.0,
            ExpatriateBasicSalary: 0.0,

            Tc: $("#tc").val() ? parseFloat($("#tc").val()) : 0.0,
           // ExpatriateBasicSalary: $("#pAmount").val() ? parseFloat($("#pAmount").val()) : 0.0,




        };

        console.log('formData', formData);

        $.ajax({
            url: "/Reposition/SaveReposition",
            type: "POST",
            contentType: "application/json",
    

            data: JSON.stringify(formData),
            success: function (response) {
                console.log('/Reposition/SaveReposition---', response)
                if (response.isSuccess) {
                    toastr.success(response.message);
                    clearForm()
                    loadRepositions();
                } else {
                    alert("Error: " + response.message);
                }
            },
            error: function () {
                alert("An error occurred while submitting the form.");
            }
        });
    });


    $("#clearButton").on("click", function () {
        clearForm();
        clearEmployeeFields();
        fetchEntryId();

        getCompany();



        fetchDropdownData('/LeaveApplicationEntry/GetBranchesMultiComp', { companyCode: null, isAll: true }, 'branchDropdown1', 'branchCode', 'branchName');
        fetchDropdownData('/LeaveApplicationEntry/GetDeptMultiCompBranch', { companyCode: null, branchCode: null, isAll: true }, 'deptDropdown1', 'departmentCode', 'departmentName');
        fetchDropdownData('/LeaveApplicationEntry/GetDesigMultiCompBranchDept', { companyCode: null, branchCode: null, departmentCode: null, isAll: true }, 'designationDropdown1', 'designationCode', 'designationName');

    });

    function clearForm() {
        $("#repoId").val('');
       // $("#repoCode").val('');
        $("#repoType").val('');
        $("#month").val('');
        $("#year").val('');
        $("#wef").val('');
        $("#refDate").val('');
        $("#pAmount").val('');
        $("#refNo").val('');
        $("#remrks").val('');
        $("#companyDropdown1").val('');
        $("#branchDropdown1").val('');
        $("#designationDropdown1").val('');
        $("#deptDropdown1").val('');
        $("#gradeDropdown1").val('');
        $("#employeeDropdown").val('');
        $("#tc").val('');

        // Optionally clear dependent fields as well, if needed.
        // For instance, if you're using Select2 dropdowns:
        $(".select2").val(null).trigger('change');
    }



    $("#deleteButton").click(function () {
        let selectedIds = [];

        // Loop through checked checkboxes and get their IDs
        $(".row-checkbox:checked").each(function () {
            let id = $(this).closest("tr").find(".edit-btn1").data("id");
            if (id) {
                selectedIds.push(id);
            }
        });

        // Check if at least one ID is selected
        if (selectedIds.length === 0) {
            toastr.warning("Please select at least one record to delete.");
            return;
        }

        // Confirm deletion
        if (!confirm("Are you sure you want to delete the selected records?")) {
            return;
        }

        // Send AJAX request to delete selected records
        $.ajax({
            url: "/reposition/delete",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ ids: selectedIds }),
            success: function (response) {
                console.log(response);
                if (response.success) {
                    toastr.success("Records deleted successfully!");
                }
                else {
                    toastr.warning(response.message);
                    var notdel = response.data.notDeleted.join(", ");
                    toastr.warning("Not deleted ids are " + notdel);
                }
                
                loadRepositions(); // Reload table after deletion
            },
            error: function () {
                toastr.warning("Error deleting records.");
            }
        });
    });
   


   
});



