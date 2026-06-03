$(document).ready(function () {


    // #region DD


    getCompany();



    fetchDropdownData('/LeaveApplicationEntry/GetBranchesMultiComp', { companyCode: null, isAll: true }, 'branchDropdown1', 'branchCode', 'branchName');
    fetchDropdownData('/LeaveApplicationEntry/GetDeptMultiCompBranch', { companyCode: null, branchCode: null, isAll: true }, 'departmentDropdown1', 'departmentCode', 'departmentName');
    fetchDropdownData('/LeaveApplicationEntry/GetEmpMultiCompBranchDept', { companyCode: null, branchCode: null, departmentCode: null, isAll: true }, 'employeeDropdown1', 'employeeId', 'employeeFirstName');
    fetchDropdownData('/LeaveApplicationEntry/GetDesigMultiCompBranchDept', { companyCode: null, branchCode: null, departmentCode: null, isAll: true }, 'designationDropdown1', 'designationCode', 'designationName');



    // Company dropdown change handler
    $('#companyDropdown1').on('changed.bs.select', function () {
        var selectedCompanies = $(this).val();
        if (selectedCompanies && selectedCompanies.length > 0) {
            fetchDropdownData('/LeaveApplicationEntry/GetBranchesMultiComp', { companyCode: selectedCompanies, isAll: false }, 'branchDropdown1', 'branchCode', 'branchName');
            fetchDropdownData('/LeaveApplicationEntry/GetDeptMultiCompBranch', { companyCode: selectedCompanies, branchCode: null, isAll: false }, 'departmentDropdown1', 'departmentCode', 'departmentName');
            fetchDropdownData('/LeaveApplicationEntry/GetEmpMultiCompBranchDept', { companyCode: selectedCompanies, branchCode: null, departmentCode: null, isAll: false }, 'employeeDropdown1', 'employeeId', 'employeeFirstName');


        } else {
            fetchDropdownData('/LeaveApplicationEntry/GetBranchesMultiComp', { companyCode: null, isAll: true }, 'branchDropdown1', 'branchCode', 'branchName');
            fetchDropdownData('/LeaveApplicationEntry/GetDeptMultiCompBranch', { companyCode: null, branchCode: null, isAll: true }, 'departmentDropdown1', 'departmentCode', 'departmentName');
            fetchDropdownData('/LeaveApplicationEntry/GetEmpMultiCompBranchDept', { companyCode: null, branchCode: null, departmentCode: null, isAll: true }, 'employeeDropdown1', 'employeeId', 'employeeFirstName');


        }
    });

    // Company dropdown change handler
    $('#branchDropdown1').change(function () {
        var compCode = $('#companyDropdown1').val();
        var selectedBranches = $(this).val();
        if (selectedBranches && selectedBranches.length > 0) {
            fetchDropdownData('/LeaveApplicationEntry/GetDeptMultiCompBranch', { companyCode: compCode, branchCode: selectedBranches, isAll: false }, 'departmentDropdown1', 'departmentCode', 'departmentName');
            fetchDropdownData('/LeaveApplicationEntry/GetEmpMultiCompBranchDept', { companyCode: compCode, branchCode: null, departmentCode: null, isAll: false }, 'employeeDropdown1', 'employeeId', 'employeeFirstName');

        } else {


            fetchDropdownData('/LeaveApplicationEntry/GetDeptMultiCompBranch', { companyCode: null, branchCode: null, isAll: true }, 'departmentDropdown1', 'departmentCode', 'departmentName');
            fetchDropdownData('/LeaveApplicationEntry/GetEmpMultiCompBranchDept', { companyCode: null, branchCode: null, departmentCode: null, isAll: true }, 'employeeDropdown1', 'employeeId', 'employeeFirstName');



        }
    });

    // Company dropdown change handler
    $('#departmentDropdown1').change(function () {

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

    //#region date and 

    // Initialize datepicker
    $('#dateFrom, #dateTo').datepicker({
        format: 'dd/MM/yyyy',
        autoclose: true,
        todayHighlight: true
    });

    // Enable/disable fields based on radio selection
    $('input[name="searchBy"]').change(function () {
        if ($('#date').is(':checked')) {
            $('#dateFrom, #dateTo').prop('disabled', false);
            $('#yearInput').prop('disabled', true);
        } else {
            $('#dateFrom, #dateTo').prop('disabled', true);
            $('#yearInput').prop('disabled', false);
        }
    }).trigger('change');


    // Set current date and year
    var currentDate = new Date();
    var formattedDate = currentDate.getDate().toString().padStart(2, '0') + '/' + (currentDate.getMonth() + 1).toString().padStart(2, '0') + '/' + currentDate.getFullYear();
    var currentYear = currentDate.getFullYear();

    $('#dateFrom, #dateTo').val(formattedDate);
    $('#yearInput').val(currentYear);

    //#endregion


    //#region render table
    function renderReport(data) {
        console.log(data)
        var tableHtml = '<table class="table table-bordered">';
        tableHtml += `
        <thead>
            <tr>
                <th>Sl#</th>
                <th> Emp. ID</th>
                <th>Name</th>
                <th>Previous Designation</th>
                <th>Current Designation</th>
                <th>Previous Department</th>
                <th>Current Department</th>
                <th>Previous Company</th>
                <th>Current Company</th>
                <th>Year</th>
                <th>Month</th>
                <th>Previous Salary</th>
                <th>Promotion Amount</th>
                <th>Current Salary</th>
                <th>Effective Date</th>
                <th>Joining Date</th>
                <th>Reposition Type</th>
            </tr>
        </thead>
        <tbody>
    `;

        var serialNumber = 1;
        data.forEach(function (item) {
            tableHtml += `
            <tr>
                <td>${serialNumber}</td>
                <td>${item.employeeID}</td>
                <td>${item.name}</td>
                <td>${item.previousDesignation}</td>
                <td>${item.currentDesignation}</td>
                <td>${item.previousDepartment}</td>
                <td>${item.currentDepartment}</td>
                <td>${item.previousCompany}</td>
                <td>${item.currentCompany}</td>
                <td>${item.year}</td>
                <td>${item.month}</td>
                <td>${item.previousSalary.toLocaleString()}</td>
                <td>${item.promotionAmount.toLocaleString()}</td>
                <td>${item.currentSalary.toLocaleString()}</td>
                <td>${new Date(item.effectiveDate).toLocaleDateString()}</td>
                <td>${new Date(item.joiningDate).toLocaleDateString()}</td>
                <td>${item.repositionType}</td>
            </tr>
        `;
            serialNumber++;
        });

        tableHtml += '</tbody></table>';

        // Insert the generated table HTML into the reportContainer
        $('#reportContainer').html(tableHtml);
    }


    //#endregion



    //#region pdf prev


    var pdfDoc = null,
        currentPage = 1,
        pageRendering = false,
        pageNumPending = null,
        searchKeyword = '';

    const canvas = document.getElementById('pdf-canvas');
    const ctx = canvas.getContext('2d');

    // Initialize PDF.js
    function loadPDF(pdfData) {
        const loadingTask = pdfjsLib.getDocument({ data: pdfData });
        loadingTask.promise.then(function (pdf) {
            pdfDoc = pdf;
            document.getElementById('page-count').textContent = pdf.numPages;
            renderPage(currentPage);
        });
    }

    // Render a specific page
    function renderPage(pageNum) {
        pageRendering = true;

        pdfDoc.getPage(pageNum).then(function (page) {
            const viewport = page.getViewport({ scale: 1.5 });
            canvas.width = viewport.width;
            canvas.height = viewport.height;

            const renderContext = {
                canvasContext: ctx,
                viewport: viewport,
            };

            page.render(renderContext).promise.then(function () {
                pageRendering = false;
                if (pageNumPending !== null) {
                    renderPage(pageNumPending);
                    pageNumPending = null;
                }
            });

            document.getElementById('page-num').textContent = pageNum;
        });
    }

    // Go to the next page
    document.getElementById('next-page').addEventListener('click', function () {
        if (currentPage < pdfDoc.numPages) {
            currentPage++;
            renderPage(currentPage);
        }
    });

    // Go to the previous page
    document.getElementById('prev-page').addEventListener('click', function () {
        if (currentPage > 1) {
            currentPage--;
            renderPage(currentPage);
        }
    });

    // Search function
    document.getElementById('search-button').addEventListener('click', function () {
        searchKeyword = document.getElementById('search-input').value;
        highlightSearch();
    });

    // Highlight matching keywords
    function highlightSearch() {
        if (searchKeyword.trim() === '') {
            return;
        }

        pdfDoc.getPage(currentPage).then(function (page) {
            page.getTextContent().then(function (textContent) {
                const textItems = textContent.items;
                const textDivs = [];

                for (let i = 0; i < textItems.length; i++) {
                    const item = textItems[i];
                    const textDiv = document.createElement('div');
                    textDiv.style.position = 'absolute';
                    textDiv.style.left = `${item.transform[4]}px`;
                    textDiv.style.top = `${item.transform[5]}px`;
                    textDiv.style.fontSize = `${item.height}px`;
                    textDiv.textContent = item.str;

                    if (item.str.toLowerCase().includes(searchKeyword.toLowerCase())) {
                        textDiv.style.backgroundColor = 'yellow'; // Highlight the matched text
                    }

                    textDivs.push(textDiv);
                }

                const pdfViewer = document.getElementById('pdf-container');
                textDivs.forEach(div => {
                    pdfViewer.appendChild(div);
                });
            });
        });
    }

    //#endregion


    //#region submit and clear

    $('#submit').click(function () {
        var searchBy = $("input[name='searchBy']:checked").val();
        console.log('search by value', searchBy);

        // Create the form data with proper naming to match your C# model
        var formData = {
            searchBy: searchBy,                           // Changed to lowercase to match property name
            dateFrom: $('#dateFrom').val(),
            dateTo: $('#dateTo').val(),
            year: $('#yearInput').val(),                  // Changed from yearInput to year
            company: $('#companyDropdown1').val(),        // Array values
            department: $('#departmentDropdown1').val(),  // Array values
            employee: $('#employeeDropdown1').val(),      // Array values
         
            branch: $('#branchDropdown1').val(),          // Array values
            designation: $('#designationDropdown1').val(),// Array values
            reprtOption: $('#reprtOption').val(),// Array values
           
        };

        console.log('Form data being sent:', formData);

        


        $.ajax({
            url: '/Reposition/RepositionReport',
            type: 'POST',
            data: formData,
            success: function (response) {

                console.log(response)

                if (response.success && response.data && response.data.length > 0) {
                    renderReport(response.data);
                    

                    //var pdfData = new Uint8Array(response.data); // Convert response to Uint8Array
                    //loadPDF(pdfData);


                } else {
                    $('#reportContainer').html('<div class="alert alert-warning">No data found</div>');
                }
                console.log('Response:', response);
            },
            error: function (err) {
                console.log("Error: " + err);
            }
        });
    });



    $("#exportBtn").click(function () {

        var searchBy = $("input[name='searchBy']:checked").val();
        console.log('search by value', searchBy);

        // Create the form data with proper naming to match your C# model
        var formData = {
            searchBy: searchBy,                           // Changed to lowercase to match property name
            dateFrom: $('#dateFrom').val(),
            dateTo: $('#dateTo').val(),
            year: $('#yearInput').val(),                  // Changed from yearInput to year
            company: $('#companyDropdown1').val(),        // Array values
            department: $('#departmentDropdown1').val(),  // Array values
            employee: $('#employeeDropdown1').val(),      // Array values

            branch: $('#branchDropdown1').val(),          // Array values
            designation: $('#designationDropdown1').val(),// Array values
            reprtOption: $('#reprtOption').val(),// Array values

        };

        var format = $('#reprtOption').val();
        console.log('Form data being sent:', formData);

        storeValue = formData


        $.ajax({
            url: '/Reposition/DownLoadReport',
            type: 'POST',
            data: formData,
           
            xhrFields: {
                responseType: 'blob' // Handle binary data properly
            },
            success: function (blob) {

                if (format == 'pdf') {
                    var url = window.URL.createObjectURL(blob);
                    var a = document.createElement('a');
                    a.href = url;
                    a.download = 'Reposition_Report.pdf';
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                    window.URL.revokeObjectURL(url); // Clean up
                } else {
                    var url = window.URL.createObjectURL(blob);
                    var a = document.createElement('a');
                    a.href = url;
                    a.download = 'Reposition_Report.xlsx'; // Changed from .pdf to .xlsx
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                    window.URL.revokeObjectURL(url); // Clean up
                }
                
                toastr.success("Generated Succesf")

                clearForm();


            },


            error: function (err) {
                console.log("Error: ", err);
            }
        });


    });


    function clearForm() {
        // Clear text inputs
        $('#dateFrom, #dateTo').val(formattedDate);
        $('#yearInput').val(currentYear);

        // Reset all dropdowns to default
        $('#companyDropdown1').val('').trigger('change');
        $('#branchDropdown1').val('').trigger('change');
        $('#departmentDropdown1').val('').trigger('change');
        $('#designationDropdown1').val('').trigger('change');
        $('#employeeDropdown1').val('').trigger('change');
        $('#reprtOption').val('pdf').trigger('change');

        // Reset radio buttons to default selection
        $('#date').prop('checked', true);
        $('#year').prop('checked', false);

        // Clear report container
        $('#reportContainer').html('');
    }

    //#endregion

});