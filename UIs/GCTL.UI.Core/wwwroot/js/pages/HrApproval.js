

$(document).ready(function () {

    let leavePendingTable;
    initializeDataTable();

    // Initialize an array to store selected IDs
    let selectedLeaveIds = [];

    // Call the function to initialize the table
    initLeaveApplicationsTable();


    $('#clearBtn').on('click', function () {
        resetLeaveApprovalForm();
    });

    // Handle form submission
    $('#leaveApprovalForm').submit(function (e) {
        e.preventDefault();
        // Get all form data
        // Get the values of the required fields
        var leaveAppEntryId = $('#leaveAppEntryId').val();
        var approvalStatus = $('#approvalStatus').val();
        var confirmRemark = $('#confirmRemark').val();

        var LeaveApprovalViewModel = {
            LeaveAppEntryId: leaveAppEntryId,
            ApprovalStatus: approvalStatus,
            ConfirmationRemark: confirmRemark
        };




        // AJAX call
        $.ajax({
            url: '/LeaveApplicationEntry/SubmitLeaveApproval',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(LeaveApprovalViewModel),
            success: function (response) {
                console.log("Action performed successfully:", response);
                toastr.success("Action performed successfully.");
                // Reload the table
                // leavePendingTable.ajax.reload();
                initializeDataTable();
                resetLeaveApprovalForm();
            },
            error: function (xhr, status, error) {
                console.error("Error performing action:", error);
                toastr.warning("An error occurred while performing the action.");
            }
        });
    })





    function initializeDataTable() {
        // If table already exists, destroy it first
        if ($.fn.DataTable.isDataTable('#leavePendingTable')) {
            $('#leavePendingTable').DataTable().destroy();
        }

        const leavePendingTable = $('#leavePendingTable').DataTable({
            ajax: {
                url: '/LeaveApplicationEntry/GetLeavePendingForHR',
                type: 'GET',
                dataSrc: function (response) {
                    console.log("Processing response:", response);
                    if (response.success && Array.isArray(response.data)) {
                        console.log("Returned data array:", response.data);
                        return response.data;
                    }
                    return [];
                },
                error: function (xhr, error, thrown) {
                    console.error('DataTables Ajax Error:', error, thrown);
                    $('#leavePendingTable_wrapper')
                        .prepend('<div class="alert alert-danger">Unable to load leave applications. Please try refreshing the page or contact support if the problem persists.</div>');
                }
            },
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return `<input type="checkbox" class="row-checkbox" data-id="${row.leaveAppEntryId}" />`;
                    },
                    orderable: false,
                    className: 'select-checkbox',
                    width: "5%"
                },
                {
                    data: 'leaveAppEntryId',
                    render: function (data, type, row) {
                        return `<button style="text-decoration: underline; color: rebeccapurple;" class="btn leave-entry-btn" data-id="${row.leaveAppEntryCode}"> ${data} </button>`;
                    },
                    width: "10%"
                },
                {
                    data: 'leaveTypeId',
                    render: function (data) {
                        return data || '';
                    },
                    width: "10%"
                },
                {
                    data: 'employeeID',
                    render: function (data) {
                        return data || '';
                    },
                    width: "20%"
                },
                {
                    data: 'employeeFirstName',
                    render: function (data) {
                        return data || '';
                    },
                    width: "20%"
                },
                {
                    data: 'designationName',
                    render: function (data) {
                        return data || '';
                    },
                    width: "20%"
                }
            ],
            processing: true,
            serverSide: false,
            language: {
                processing: "Loading leave applications...",
                zeroRecords: "No leave applications found",
                emptyTable: "No leave applications available"
            },
            order: [[1, 'desc']],
            responsive: true,
            autoWidth: false,
            drawCallback: function (settings) {
                console.log("Table draw complete", settings);
            },
            initComplete: function () {
                // Add a "Select All" checkbox in the header
                $('#leavePendingTable thead tr th:first-child').html('<input type="checkbox" id="select-all" />');
            }
        });

        // Track selected IDs
        let selectedIds = [];

        // Handle row checkbox selection
        $('#leavePendingTable').on('change', '.row-checkbox', function () {
            const leaveId = $(this).data('id');
            if ($(this).is(':checked')) {
                if (!selectedIds.includes(leaveId)) {
                    selectedIds.push(leaveId);
                }
            } else {
                selectedIds = selectedIds.filter(id => id !== leaveId);
            }
            console.log("Selected IDs after checkbox change:", selectedIds);
        });

        // Handle "Select All" checkbox
        $('#leavePendingTable').on('change', '#select-all', function () {
            const isChecked = $(this).is(':checked');
            $('.row-checkbox').prop('checked', isChecked).trigger('change');
        });

        // External function to submit selected rows
        function submitSelectedLeaves(ids) {
            console.log("Preparing to submit selected leave IDs:", ids);

            if (!Array.isArray(ids) || ids.length === 0) {
                console.warn("No valid IDs to submit. Skipping request.");
                return;
            }

            $.ajax({
                url: '/LeaveApplicationEntry/SubmitSelectedLeaves', // Update with your actual endpoint
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ leaveIds: ids }),
                beforeSend: function () {
                    console.log("Sending request to /LeaveApplicationEntry/SubmitSelectedLeaves with data:", { leaveIds: ids });
                },
                success: function (response) {
                    console.log("Response from server:", response);
                    toastr.success("Selected leave applications submitted successfully.");

                    initializeDataTable();

                    initLeaveApplicationsTable();

                    // Clear selection
                    selectedIds = [];
                    $('#leavePendingTable').find('.row-checkbox').prop('checked', false);
                    $('#select-all').prop('checked', false);
                },
                error: function (xhr, status, error) {
                    console.error("Error during submission. Status:", status, "Error:", error, "Response:", xhr.responseText);
                    toastr.warning("An error occurred while submitting the leave applications. Please try again.");
                }
            });
        }

        // Button to submit selected rows
        $('#submitSelected').on('click', function () {
            if (selectedIds.length === 0) {
                toastr.warning("Please select at least one leave application.");
                return;
            }

            submitSelectedLeaves(selectedIds);
        });

        // Button to submit rejected rows
        $('#submitRejected').on('click', function () {


            if (selectedIds.length === 0) {
                toastr.warning("Please select at least one leave application.");
                return;
            }

            submitRejectedLeaves(selectedIds);
        });

        function submitRejectedLeaves(ids) {
            console.log("Preparing to submit rejected leave IDs:", ids);

            if (!Array.isArray(ids) || ids.length === 0) {
                console.warn("No valid IDs to submit. Skipping request.");
                return;
            }

            $.ajax({
                url: '/LeaveApplicationEntry/SubmitRejectedLeaves', // Update with your actual endpoint
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ leaveIds: ids }),
                beforeSend: function () {
                    console.log("Sending request to /LeaveApplicationEntry/SubmitSelectedLeavesRejected with data:", { leaveIds: ids });
                },
                success: function (response) {
                    console.log("Response from server:", response);
                    toastr.success("Rejected leave applications submitted successfully.");

                    initializeDataTable();

                    // Clear selection
                    selectedIds = [];
                    $('#leavePendingTable').find('.row-checkbox').prop('checked', false);
                    $('#select-all').prop('checked', false);
                },
                error: function (xhr, status, error) {
                    console.error("Error during submission. Status:", status, "Error:", error, "Response:", xhr.responseText);
                    toastr.warning("An error occurred while submitting the rejected leave applications. Please try again.");
                }
            });
        }


    }





    // Handle "Select All" checkbox
    $('#leavePendingTable').on('change', '#select-all', function () {
        const isChecked = $(this).is(':checked');
        $('#leavePendingTable .row-checkbox').prop('checked', isChecked);
    });

    // Handle individual row checkbox selection
    $('#leavePendingTable').on('change', '.row-checkbox', function () {
        const allChecked = $('.row-checkbox').length === $('.row-checkbox:checked').length;
        $('#select-all').prop('checked', allChecked);
    });

    // Handle button click in the leaveAppEntryId column
    $('#leavePendingTable').on('click', '.leave-entry-btn', function () {
        const id = $(this).data('id');

        console.log("Button clicked for LeaveAppEntryId:", id);

        // AJAX call
        $.ajax({
            url: '/LeaveApplicationEntry/GetLeaveApplication1',
            type: 'POST',
            data: { id },
            success: function (response) {
                console.log("Action performed successfully for:", id);
                console.log("Action performed successfully Get :", response);

                populateForm(response.data)
                getEmployeeInfo(response.data.employeeId);
                getLeaveInfo(response.data.leaveTypeId, response.data.employeeId);

            },
            error: function (xhr, status, error) {
                console.error("Error performing action for:", id, error);
                toastr.warning("An error occurred while performing the action.");
            }
        });

    });

    function getLeaveInfo(LeaveTypeId, EmpId) {
        console.log("Action performed getLeaveInfo id:", LeaveTypeId);
        $.ajax({
            url: '/LeaveApplicationEntry/GetLeaveTypeByLeaveTypeIdEmpId',
            type: 'POST',
            data: { LeaveTypeId, EmpId },
            success: function (response) {
                console.log("Action performed successfully for:", LeaveTypeId);
                populateFormLeave(response.data)
            },
            error: function (xhr, status, error) {
                console.error("Error performing action for:", id, error);
                toastr.warning("An error occurred while performing the action.");
            }
        });
    }

    function populateFormLeave(data) {
        console.log("Populating form with Leave data:", data);

        // Handle HOD details
        var leaveOption = `<option value="${data.leaveTypeCode}">${data.leaveName || ''}</option>`;

        $('#leaveType').empty().append(leaveOption);

        // Assume data contains more values you want to display
        let availableLeave = data.remainingLeave; // Current leave status
        let totalLeave = data.availableLeave; // Total leave available
        let usedLeave = data.totalLeaveTaken; // Leave already used

        // Update the leaveStatus span with more values
        $('#leaveStatus').html(`<b>YGL: ${totalLeave}, AL: ${usedLeave}, BL: ${availableLeave}</b>`);



        // Populate leave type details
        //$('#leaveType').val(data.leaveTypeName);
        //$('#leaveDays').val(data.leaveDays);
        //$('#leaveBalance').val(data.leaveBalance);
        //$('#leaveEntitlement').val(data.leaveEntitlement);
        //$('#leaveCarryOver').val(data.leaveCarryOver);
        //$('#leaveTaken').val(data.leaveTaken
    }

    function getEmployeeInfo(EmpId) {
        console.log("Action performed getEmployeeInfo id:", EmpId);

        $.ajax({
            url: '/LeaveApplicationEntry/GetEmployeeOfficialByEmpId',
            type: 'POST',
            data: { EmpId },
            success: function (response) {
                console.log("Action performed successfully for:", EmpId);

                populateFormEmp(response.data)


            },
            error: function (xhr, status, error) {
                console.error("Error performing action for:", id, error);
                toastr.warning("An error occurred while performing the action.");
            }
        });
    }


    function populateFormEmp(data1) {
        console.log("Populating form with Emp data:---1", data1);

        data = data1.result;

        console.log("Populating form with Emp data:---2", data);


        // Populate employee details
        $('#employeeName').val(data.employeeFirstName + ' ' + data.employeeLastName);
        $('#designation').val(data.designationName);
        $('#department').val(data.departmentName);

        // Handle supervisor details
        var supervisorOption = data.reportingFirstName
            ? `<option value="${data.reportingTo}">${data.reportingFirstName} ${data.reportingLastName || ''}</option>`
            : '<option value="">--No Supervisor--</option>';

        // Handle HOD details
        var hodOption = `<option value="${data.hod}">${data.hodFirstName} ${data.hodLastName || ''}</option>`;

        // Update dropdowns
        $('#immediateSupervisor').empty().append(supervisorOption);
        $('#headOfDepartment').empty().append(hodOption);



    }

    function populateForm(data) {
        console.log("Populating form with data:", data);



        var empOption = `<option value="${data.employeeId}">${data.employeeId || ''}</option>`;
        $('#employeeDropdown').empty().append(empOption);

        $('#leaveStatus').val(data.isApproved); // Leave status

        $('#leaveAppEntryId').val(data.leaveAppEntryId); // Leave status



        // Populate Leave Type
        $('#applyLeaveFormat').val(data.applyLeaveFormat); // Set leave format


        // Format specific data population
        switch (data.applyLeaveFormat) {
            case 'fullDayLeave':
                $('#fullDayInputs').show();
                $('#oneDayInputs').hide();
                $('#halfDayInputs').hide();

                if (data.startDate) {
                    $('#fromDate').val(data.startDate.split('T')[0]);
                }
                if (data.endDate) {
                    $('#toDate').val(data.endDate.split('T')[0]);
                }
                $('#daysBetween').val(data.noOfDay);


                //if (data.days && data.days.length > 0) {
                //    const calendarDates = data.days.map(day => day.split("T")[0]).join(", ");
                //    $("#days").val(calendarDates); // Assuming #employeeName displays calendar dates
                //}
                if (data.days && data.days.length > 0) {
                    const calendarDates = data.days.map(day => {
                        const date = new Date(day);
                        const dayPart = String(date.getDate()).padStart(2, '0');
                        const monthPart = String(date.getMonth() + 1).padStart(2, '0'); // Months are zero-based
                        const yearPart = date.getFullYear();
                        return `${dayPart}/${monthPart}/${yearPart}`;
                    }).join(", ");
                    $("#days").val(calendarDates); // Assuming #employeeName displays calendar dates
                }



                break;



            case 'halfDayLeave':
                $('#fullDayInputs').hide();
                $('#oneDayInputs').hide();
                $('#halfDayInputs').show();


                $('#halfDayCheckbox').prop('checked', data.halfDay === "1");
                if (data.firstOrSecondHalf === "1") {
                    $('#firstHalf').prop('checked', true);
                } else if (data.firstOrSecondHalf === "2") {
                    $('#secondHalf').prop('checked', true);
                }

                $("#days").val('');

                break;

            case 'shortLeave':
                $('#fullDayInputs').hide();
                $('#oneDayInputs').show();
                $('#halfDayInputs').hide();


                if (data.shortLeaveFrom) {
                    // Extract only the time portion and format it to HH:mm
                    const fromTime = data.shortLeaveFrom.split('T')[1].substring(0, 5);
                    $('#fromTime').val(fromTime);
                }
                if (data.shortLeaveTo) {
                    // Extract only the time portion and format it to HH:mm
                    const toTime = data.shortLeaveTo.split('T')[1].substring(0, 5);
                    $('#toTime').val(toTime);
                }

                $("#days").val('');


                break;
        }


        // Populate Reason
        $('textarea').first().val(data.reason); // Assuming the first textarea is for reason




        // Populate Create At and Last Update At
        if (data.ldate) {
            $('#createAt').val(new Date(data.ldate).toISOString().split('T')[0]); // Creation date
        } else {
            console.error("data.ldate is null or undefined");
        }

        if (data.modifyDate) {
            $('#updateAt').val(new Date(data.modifyDate).toISOString().split('T')[0]); // Last update date
        } else {
            $('#updateAt').val("Not updated");
        }

    }


    function resetLeaveApprovalForm() {

        // Clear text inputs
        $('#leaveApprovalForm input[type="text"], #leaveApprovalForm input[type="date"], #leaveApprovalForm input[type="time"]').val('');


        // Reset each select element manually
        $('#employeeDropdown').val('');
        $('#immediateSupervisor').val('');
        $('#headOfDepartment').val('');
        $('#leaveType').val('');
        // $('#approvalStatus').val('');


        var appStatus = `<option selected>--Select Approval Status--</option>
                                    <option value="approved">Approved</option>
                                    <option value="rejected">Rejected</option>`

        $('#approvalStatus').empty().append(appStatus);

        //// Clear and reset select elements
        //$('#leaveApprovalForm select').each(function () {
        //    $(this).empty().append('<option value=""></option>');
        //});

        // Clear textareas
        $('#leaveApprovalForm textarea').val('');

        // Reset any checkboxes or radio buttons
        $('#leaveApprovalForm input[type="checkbox"], #leaveApprovalForm input[type="radio"]').prop('checked', false);

        // Optionally, hide any dynamic input sections
        $('#fullDayInputs, #halfDayInputs, #oneDayInputs').hide();

        // Reset any displayed values
        $('#daysBetween, #hoursBetween').val('');
        $('#leaveStatus').text('');
        $('#days').val('');
        $('#createAt').val('');
        $('#updateAt').val('');
    }

    //LeaveApplicationEntry/GetApprovedLeaveApplications

    function initLeaveApplicationsTable() {

        if ($.fn.DataTable.isDataTable('#leaveApplicationsTable')) {
            $('#leaveApplicationsTable').DataTable().destroy();
        }


        $('#leaveApplicationsTable').DataTable({
            processing: true,
            serverSide: true,
            ajax: {
                url: '/LeaveApplicationEntry/GetApprovedLeaveApplications', // Replace with your actual controller route
                type: 'POST',
                data: function (d) {
                    // Pass additional data if needed
                    return d;
                },
                error: function (xhr, error, code) {
                    console.error("Error occurred while loading data:", error);
                }
            },
            columns: [
                {
                    data: null,
                    render: function (data) {
                        return `<div class="form-check">
                        <input type="checkbox" class="form-check-input leave-select"
                            id="check_${data.leaveAppEntryId}"
                            value="${data.leaveAppEntryId}">
                    </div>`;
                    },
                    orderable: false,
                    searchable: false
                },
                {
                    data: 'leaveAppEntryId',
                    title: 'Leave Id'
                },
                {
                    data: 'ldate',
                    title: 'Apply Date',
                    render: function (data) {
                        return data ? new Date(data).toLocaleDateString('en-GB') : '';
                    }
                },
                {
                    data: 'applyLeaveFormat',
                    title: 'Leave Format'
                },
                {
                    data: null,
                    title: 'Approval Status',
                    render: function (data) {
                        let status = '';
                        let className = '';

                        if (data.hodApprovalStatus === 'Approved' && data.hrApprovalStatus === 'Approved') {
                            status = 'Approved';
                            className = 'text-success';
                        } else if (data.hodApprovalStatus === 'Rejected' || data.hrApprovalStatus === 'Rejected') {
                            status = 'Rejected';
                            className = 'text-danger';
                        } else {
                            status = 'Pending';
                            className = 'text-warning';
                        }

                        return `<span class="${className} fw-bold">${status}</span>`;
                    }
                },
                {
                    data: 'employeeID',
                    title: 'Employee ID'
                },
                {
                    data: null,
                    title: 'Name',
                    render: function (data) {
                        return `<div>
                        ${data.employeeFirstName}
                        <small class="d-block text-muted">
                            ${data.departmentName} - ${data.designationName}
                        </small>
                    </div>`;
                    }
                },
                {
                    data: 'leaveTypeId',
                    title: 'Leave Type'
                },
                {
                    data: 'startDate',
                    title: 'Start Date',
                    render: function (data) {
                        return data ? new Date(data).toLocaleDateString('en-GB') : '';
                    }
                },
                {
                    data: 'endDate',
                    title: 'End Date',
                    render: function (data) {
                        return data ? new Date(data).toLocaleDateString('en-GB') : '';
                    }
                },
                {
                    data: 'days',
                    title: 'Calendar Dates',
                    render: function (data) {
                        if (!data) return '';
                        return Array.isArray(data)
                            ? data.map(date => new Date(date).toLocaleDateString('en-GB')).join(', ')
                            : '';
                    }
                },
                {
                    data: 'noOfDay',
                    title: 'No Of Days'
                },
                {
                    data: 'reason',
                    title: 'Reason'
                }
            ],
            order: [[2, 'desc']], // Default sorting (column index 2: Apply Date)
            responsive: true,
            lengthMenu: [10, 25, 50, 100],
            pageLength: 10,
            columnDefs: [
                {
                    targets: 0, // Disable ordering on the checkbox column
                    orderable: false
                }
            ],
            drawCallback: function () {
                $('[data-bs-toggle="tooltip"]').tooltip({
                    container: 'body',
                    html: true
                });
            },
            // Add tooltip to rows
            createdRow: function (row, data) {
                $(row).attr('data-bs-toggle', 'tooltip')
                    .attr('data-bs-html', 'true')
                    .attr('title', `
                    <div class="tooltip-content">
                        <strong>HOD:</strong> ${data.hodFirstName} (${data.hodApprovalStatus || 'Pending'})<br>
                        <strong>Supervisor:</strong> ${data.supervisorFirstName}<br>
                        <strong>HR Status:</strong> ${data.hrApprovalStatus || 'Pending'}<br>
                        <strong>HR Remarks:</strong> ${data.hrApprovalRemarks || 'N/A'}
                    </div>
                `);
            }
        });
    }




    const styles = `
    .text-success { color: #28a745 !important; }
    .text-warning { color: #ffc107 !important; }
    .text-danger { color: #dc3545 !important; }
    .text-muted { color: #6c757d !important; }
    .fw-bold { font-weight: bold; }
    .d-block { display: block; }
    .tooltip-content {
        font-size: 12px;
        line-height: 1.4;
    }
    .dataTables_wrapper .top {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 1rem;
    }
    .dataTables_wrapper .dt-buttons {
        margin-right: 1rem;
    }
    .form-check-input {
        margin-top: 0;
    }
`;

    const styleSheet = document.createElement("style");
    styleSheet.innerText = styles;
    document.head.appendChild(styleSheet);






















});
