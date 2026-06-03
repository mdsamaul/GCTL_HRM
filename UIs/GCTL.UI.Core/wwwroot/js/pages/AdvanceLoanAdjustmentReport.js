(function ($) {
    $.patientTypes = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
        }, options);

        var filterUrl = commonName.baseUrl + "/GetFilterData";

        $('#downloadReport').click(function () {
            var pdf = $("#reportText").val();
            if (pdf === "downloadPdf") {
                isPdf = true;
                isWord = false;
                searchAdvancePayReportGrouped();
            } else if (pdf === "downloadExcel") {
                downloadAdvancePayReportExcel();
            } else if (pdf === "downloadWord") {
                isWord = true;
                isPdf = false;
                searchAdvancePayReportGrouped();
            } 
        });

        $('#btnPreview').click(function () {
            isPdf = false;
            isWord = false;
            searchAdvancePayReportGrouped();
        });

    };
    let isPdf = true;
    let isWord = false;
    var setupLoadingOverlay = function () {
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
    };

    function showLoading() {
        $("#customLoadingOverlay").css("display", "flex");
    }

    function hideLoading() {
        $("#customLoadingOverlay").hide();
    }

    const fieldData = {
        companyCodes: [],
        branchCodes: [],
        departmentCodes: [ ],
        designationCodes: [ ],
        employeeIDs: [],
        payHeadIDs: [],
        monthIDs: [],
        yearIDs: []
    };

    let selectedValues = {};

    // Initialize all multiselect components
    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.multiselect-container').forEach(container => {
            initializeMultiSelect(container);
        });
    });

    //function initializeMultiSelect(container) {
    //    const field = container.dataset.field;
    //    const input = container.querySelector('.multiselect-input');
    //    const dropdown = container.querySelector('.multiselect-dropdown');
    //    const searchInput = container.querySelector('.multiselect-search input');
    //    const optionsContainer = container.querySelector('.multiselect-options');
    //    const arrow = container.querySelector('.multiselect-arrow');

    //    selectedValues[field] = [];

    //    // Populate options
    //    populateOptions(field, optionsContainer, '');

    //    // Toggle dropdown
    //    input.addEventListener('click', (e) => {
    //        e.stopPropagation();
    //        closeAllDropdowns();
    //        dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
    //        arrow.classList.toggle('open', dropdown.style.display === 'block');
    //        if (dropdown.style.display === 'block') {
    //            searchInput.focus();
    //        }
    //    });

    //    // Search functionality
    //    searchInput.addEventListener('input', (e) => {
    //        populateOptions(field, optionsContainer, e.target.value);
    //    });

    //    // Prevent dropdown close when clicking inside
    //    dropdown.addEventListener('click', (e) => {
    //        e.stopPropagation();
    //    });
    //}
    function initializeMultiSelect(container) {
        const field = container.dataset.field;
        const input = container.querySelector('.multiselect-input');
        const dropdown = container.querySelector('.multiselect-dropdown');
        const searchInput = container.querySelector('.multiselect-search input');
        const optionsContainer = container.querySelector('.multiselect-options');
        const arrow = container.querySelector('.multiselect-arrow');

        selectedValues[field] = [];

        // ✅ Search input কে dropdown এর ভেতরে সবার আগে move করো
        const searchWrapper = container.querySelector('.multiselect-search');
        if (searchWrapper && dropdown) {
            // dropdown এর প্রথমে insert করো
            dropdown.insertBefore(searchWrapper, dropdown.firstChild);

            // ✅ Search wrapper এর style ঠিক করো
            searchWrapper.style.padding = '8px';
            searchWrapper.style.borderBottom = '1px solid #ddd';
            searchWrapper.style.position = 'sticky';
            searchWrapper.style.top = '0';
            searchWrapper.style.backgroundColor = '#fff';
            searchWrapper.style.zIndex = '1';

            // ✅ Search input এর style
            if (searchInput) {
                searchInput.style.width = '100%';
                searchInput.style.border = '1px solid #ccc';
                searchInput.style.borderRadius = '4px';
                searchInput.style.padding = '5px 8px';
                searchInput.style.fontSize = '13px';
                searchInput.style.boxSizing = 'border-box';
            }
        }

        // Populate options
        populateOptions(field, optionsContainer, '');

        // Toggle dropdown
        input.addEventListener('click', (e) => {
            e.stopPropagation();
            closeAllDropdowns();
            dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
            arrow.classList.toggle('open', dropdown.style.display === 'block');
            if (dropdown.style.display === 'block') {
                searchInput.focus();
            }
        });

        // Search functionality
        searchInput.addEventListener('input', (e) => {
            populateOptions(field, optionsContainer, e.target.value);
        });

        // Prevent dropdown close when clicking inside
        dropdown.addEventListener('click', (e) => {
            e.stopPropagation();
        });
    }
    function populateOptions(field, container, searchTerm) {
        
        const data = fieldData[field] || [];
        const isMultiSelect = container.closest('.multiselect-container').dataset.multiselect !== "false";

        const cleanData = data.filter(item => item && typeof item.id === 'string' && typeof item.name === 'string');
        const filtered = cleanData.filter(item =>
            item.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
            item.name.toLowerCase().includes(searchTerm.toLowerCase())
        );

        container.innerHTML = '';

        if (filtered.length === 0) {
            container.innerHTML = '<div class="multiselect-option disabled">No data available</div>';
            return;
        }

        if (field === 'companyCodes' && selectedValues[field].length === 0 && cleanData.length > 0) {
            selectedValues[field] = [cleanData[0].id];
            updateDisplay(field);
            fetchFilterOptionsBasedOn(field);
        }

        filtered.forEach(item => {
            const option = document.createElement('div');
            option.className = 'multiselect-option';

            if (isMultiSelect) {
                const checkbox = document.createElement('input');
                checkbox.type = 'checkbox';
                checkbox.value = item.id;
                checkbox.checked = selectedValues[field].includes(item.id);

                const label = document.createElement('span');
                label.textContent = item.name;

                option.appendChild(checkbox);
                option.appendChild(label);

                option.addEventListener('click', (e) => {
                    e.stopPropagation();
                    checkbox.checked = !checkbox.checked;
                    toggleSelection(field, item.id, checkbox.checked);
                });

                checkbox.addEventListener('change', (e) => {
                    e.stopPropagation();
                    toggleSelection(field, item.id, e.target.checked);
                });

            } else {
                option.textContent = item.name;

                option.addEventListener('click', (e) => {
                    e.stopPropagation();
                    selectedValues[field] = [item.id];
                    updateDisplay(field);

                    const dropdown = container.closest('.multiselect-dropdown');
                    dropdown.style.display = 'none';

                    const arrow = container.closest('.multiselect-container').querySelector('.multiselect-arrow');
                    if (arrow) arrow.classList.remove('open');

                    fetchFilterOptionsBasedOn(field);
                });
            }

            container.appendChild(option);
        });
    }

    function toggleSelection(field, value, isSelected) {
        const isMultiSelect = document.querySelector(`[data-field="${field}"]`).dataset.multiselect !== "false";

        if (isMultiSelect) {
            if (isSelected && !selectedValues[field].includes(value)) {
                selectedValues[field].push(value);
            } else if (!isSelected) {
                selectedValues[field] = selectedValues[field].filter(v => v !== value);
            }
        } else {
            selectedValues[field] = isSelected ? [value] : [];
        }

        updateDisplay(field);

        if (["companyCodes", "branchCodes", "departmentCodes", "designationCodes", "employeeIDs", "payHeadIDs", "monthIDs"].includes(field)) {
            fetchFilterOptionsBasedOn(field);
        }
    }

    function fetchFilterOptionsBasedOn(changedField) {
        const filterData = {
            companyCodes: selectedValues.companyCodes || undefined,
            branchCodes: selectedValues.branchCodes || undefined,
            departmentCodes: selectedValues.departmentCodes || undefined,
            designationCodes: selectedValues.designationCodes || undefined,
            employeeIDs: selectedValues.employeeIDs || undefined,
            payHeadIDs: selectedValues.payHeadIDs || undefined,
            monthIDs: selectedValues.monthIDs || undefined,
            yearIDs: selectedValues.yearIDs || undefined
        };

       
        showLoading();
        $.ajax({
            url: '/AdvanceLoanAdjustmentReport/GetAdvancePayFilterReport',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(filterData),
            success: function (response) {
               
                hideLoading();
                if (changedField === "companyCodes" ) {
                  
                    fieldData.branchCodes = response.branches || [];
                    fieldData.departmentCodes = response.departments || [];
                    fieldData.designationCodes = response.designations || [];
                    fieldData.employeeIDs = response.employees || [];
                    fieldData.payHeadIDs = response.payHeads || [];
                    fieldData.monthIDs = response.months || [];
                    fieldData.yearIDs = response.years || [];

                    selectedValues.branchCodes = [];
                    selectedValues.departmentCodes = [];
                    selectedValues.designationCodes = [];
                    selectedValues.employeeIDs = [];
                    selectedValues.payHeadIDs = [];
                    selectedValues.monthIDs = [];
                    selectedValues.yearIDs = [];

                    populateOptions("branchCodes", document.querySelector('[data-field="branchCodes"] .multiselect-options'), '');
                    populateOptions("departmentCodes", document.querySelector('[data-field="departmentCodes"] .multiselect-options'), '');
                    populateOptions("designationCodes", document.querySelector('[data-field="designationCodes"] .multiselect-options'), '');
                    populateOptions("employeeIDs", document.querySelector('[data-field="employeeIDs"] .multiselect-options'), '');
                    populateOptions("payHeadIDs", document.querySelector('[data-field="payHeadIDs"] .multiselect-options'), '');
                    populateOptions("monthIDs", document.querySelector('[data-field="monthIDs"] .multiselect-options'), '');
                    populateOptions("yearIDs", document.querySelector('[data-field="yearIDs"] .multiselect-options'), '');

                    updateDisplay("branchCodes");
                    updateDisplay("departmentCodes");
                    updateDisplay("designationCodes");
                    updateDisplay("employeeIDs");
                    updateDisplay("payHeadIDs");
                    updateDisplay("monthIDs");
                    updateDisplay("yearIDs");
                }
                if ( changedField === "branchCodes") {
                             
                    fieldData.departmentCodes = response.departments || [];
                    fieldData.designationCodes = response.designations || [];
                    fieldData.employeeIDs = response.employees || [];
                    fieldData.payHeadIDs = response.payHeads || [];
                    fieldData.monthIDs = response.months || [];
                    fieldData.yearIDs = response.years || [];

                   
                    selectedValues.departmentCodes = [];
                    selectedValues.designationCodes = [];
                    selectedValues.employeeIDs = [];
                    selectedValues.payHeadIDs = [];
                    selectedValues.monthIDs = [];
                    selectedValues.yearIDs = [];

                   
                    populateOptions("departmentCodes", document.querySelector('[data-field="departmentCodes"] .multiselect-options'), '');
                    populateOptions("designationCodes", document.querySelector('[data-field="designationCodes"] .multiselect-options'), '');
                    populateOptions("employeeIDs", document.querySelector('[data-field="employeeIDs"] .multiselect-options'), '');
                    populateOptions("payHeadIDs", document.querySelector('[data-field="payHeadIDs"] .multiselect-options'), '');
                    populateOptions("monthIDs", document.querySelector('[data-field="monthIDs"] .multiselect-options'), '');
                    populateOptions("yearIDs", document.querySelector('[data-field="yearIDs"] .multiselect-options'), '');

                    updateDisplay("departmentCodes");
                    updateDisplay("designationCodes");
                    updateDisplay("employeeIDs");
                    updateDisplay("payHeadIDs");
                    updateDisplay("monthIDs");
                    updateDisplay("yearIDs");
                }
                if (changedField === "departmentCodes") {               
                    
                    fieldData.designationCodes = response.designations || [];
                    fieldData.employeeIDs = response.employees || [];
                    fieldData.payHeadIDs = response.payHeads || [];
                    fieldData.monthIDs = response.months || [];
                    fieldData.yearIDs = response.years || [];


                   
                    selectedValues.designationCodes = [];
                    selectedValues.employeeIDs = [];
                    selectedValues.payHeadIDs = [];
                    selectedValues.monthIDs = [];
                    selectedValues.yearIDs = [];
                
                    populateOptions("designationCodes", document.querySelector('[data-field="designationCodes"] .multiselect-options'), '');
                    populateOptions("employeeIDs", document.querySelector('[data-field="employeeIDs"] .multiselect-options'), '');
                    populateOptions("payHeadIDs", document.querySelector('[data-field="payHeadIDs"] .multiselect-options'), '');
                    populateOptions("monthIDs", document.querySelector('[data-field="monthIDs"] .multiselect-options'), '');
                    populateOptions("yearIDs", document.querySelector('[data-field="yearIDs"] .multiselect-options'), '');

                    updateDisplay("designationCodes");
                    updateDisplay("employeeIDs");
                    updateDisplay("payHeadIDs");
                    updateDisplay("monthIDs");
                    updateDisplay("yearIDs");
                }
                if (changedField === "designationCodes") {             

                    fieldData.employeeIDs = response.employees || [];
                    fieldData.payHeadIDs = response.payHeads || [];
                    fieldData.monthIDs = response.months || [];
                    fieldData.yearIDs = response.years || [];



                    selectedValues.employeeIDs = [];
                    selectedValues.payHeadIDs = [];
                    selectedValues.monthIDs = [];
                    selectedValues.yearIDs = [];
          
                    populateOptions("employeeIDs", document.querySelector('[data-field="employeeIDs"] .multiselect-options'), '');
                    populateOptions("payHeadIDs", document.querySelector('[data-field="payHeadIDs"] .multiselect-options'), '');
                    populateOptions("monthIDs", document.querySelector('[data-field="monthIDs"] .multiselect-options'), '');
                    populateOptions("yearIDs", document.querySelector('[data-field="yearIDs"] .multiselect-options'), '');

                    updateDisplay("employeeIDs");
                    updateDisplay("payHeadIDs");
                    updateDisplay("monthIDs");
                    updateDisplay("yearIDs");
                }
                if (changedField === "employeeIDs") {             

                    fieldData.payHeadIDs = response.payHeads || [];
                    fieldData.monthIDs = response.months || [];
                    fieldData.yearIDs = response.years || [];



                    selectedValues.payHeadIDs = [];
                    selectedValues.monthIDs = [];
                    selectedValues.yearIDs = [];
            
                   
                    populateOptions("payHeadIDs", document.querySelector('[data-field="payHeadIDs"] .multiselect-options'), '');
                    populateOptions("monthIDs", document.querySelector('[data-field="monthIDs"] .multiselect-options'), '');
                    populateOptions("yearIDs", document.querySelector('[data-field="yearIDs"] .multiselect-options'), '');

                    updateDisplay("payHeadIDs");
                    updateDisplay("monthIDs");
                    updateDisplay("yearIDs");
                }
                if (changedField === "payHeadIDs") {               

                    fieldData.monthIDs = response.months || [];
                    fieldData.yearIDs = response.years || [];



                    selectedValues.monthIDs = [];
                    selectedValues.yearIDs = [];
             

                   
                    populateOptions("monthIDs", document.querySelector('[data-field="monthIDs"] .multiselect-options'), '');
                    populateOptions("yearIDs", document.querySelector('[data-field="yearIDs"] .multiselect-options'), '');

                    updateDisplay("monthIDs");
                    updateDisplay("yearIDs");
                }
                if (changedField === "monthIDs") { 
                    fieldData.yearIDs = response.years || [];
                    selectedValues.yearIDs = [];
                    populateOptions("yearIDs", document.querySelector('[data-field="yearIDs"] .multiselect-options'), '');
                    updateDisplay("yearIDs");
                }
            },
            error: function (xhr) {               
                hideLoading();
                alert("Error fetching filters");
            }
        });
    }

    
    function updateDisplay(field) {
        
        const container = document.querySelector(`[data-field="${field}"]`);
        const selectedItemsContainer = container.querySelector('.selected-items');
        const placeholderText = container.querySelector('.placeholder-text');
        const isMultiSelect = container.dataset.multiselect !== "false";

        selectedItemsContainer.innerHTML = '';

        if (selectedValues[field].length > 0) {
            placeholderText.style.display = 'none';
            
            if (isMultiSelect) {
                if (selectedValues[field].length === 1) {
                    const item = fieldData[field].find(item => item.id === selectedValues[field][0]);
                    if (item) {
                        const selectedItem = document.createElement('div');
                        selectedItem.className = 'selected-item';
                        selectedItem.innerHTML = `<span>${item.name}</span>`;
                        selectedItemsContainer.appendChild(selectedItem);
                    }
                } else {
                    const countDisplay = document.createElement('div');
                    countDisplay.className = 'count-display';
                    countDisplay.textContent = `${selectedValues[field].length} items selected`;
                    selectedItemsContainer.appendChild(countDisplay);
                }
            } else {
                
                // Single select mode – show only one selected item name
                const item = fieldData[field].find(item => item.id === selectedValues[field][0]);
                if (item) {
                    const selectedItem = document.createElement('div');
                    selectedItem.className = 'selected-item';
                    selectedItem.innerHTML = `<span>${item.name}</span>`;
                    selectedItemsContainer.appendChild(selectedItem);
                }
            }

        } else {
            placeholderText.style.display = 'block';
        }
    }

    function closeAllDropdowns() {
        document.querySelectorAll('.multiselect-dropdown').forEach(dropdown => {
            dropdown.style.display = 'none';
        });
        document.querySelectorAll('.multiselect-arrow').forEach(arrow => {
            arrow.classList.remove('open');
        });
    } 

    document.addEventListener('click', closeAllDropdowns);






    function downloadAdvancePayReportExcel() {
        showLoading();
        var filterData = {
            companyCodes: selectedValues.companyCodes || undefined,
            branchCodes: selectedValues.branchCodes || undefined,
            departmentCodes: selectedValues.departmentCodes || undefined,
            designationCodes: selectedValues.designationCodes || undefined,
            employeeIDs: selectedValues.employeeIDs || undefined,
            payHeadIDs: selectedValues.payHeadIDs || undefined,
            monthIDs: selectedValues.monthIDs || undefined,
            yearIDs: selectedValues.yearIDs || undefined
        };

        $.ajax({
            url: '/AdvanceLoanAdjustmentReport/GetAdvancePayReportGrouped',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(filterData),
            success: function (response) {
                if (response.success) {
                    $.ajax({
                        url: '/AdvanceLoanAdjustmentReport/ExportAdvancePayReportExcel',
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify(response.data),
                        xhrFields: {
                            responseType: 'blob'
                        },
                        success: function (blob) {
                            hideLoading();
                            const url = window.URL.createObjectURL(blob);
                            const a = document.createElement('a');
                            a.href = url;
                            a.download = "AdvancePayReport.xlsx";
                            document.body.appendChild(a);
                            a.click();
                            a.remove();
                            window.URL.revokeObjectURL(url);
                        },
                        error: function () {
                            hideLoading();
                            showError('Excel download failed');
                        }
                    });
                } else {
                    hideLoading();
                    showError(response.message || 'Failed to retrieve grouped data');
                }
            },
            error: function () {
                hideLoading();
                showError('Something went wrong.');
            }
        });
    }
    function searchAdvancePayReportGrouped() { 
        showLoading();
        var filterData = {
            companyCodes: selectedValues.companyCodes || undefined,
            branchCodes: selectedValues.branchCodes || undefined,
            departmentCodes: selectedValues.departmentCodes || undefined,
            designationCodes: selectedValues.designationCodes || undefined,
            employeeIDs: selectedValues.employeeIDs || undefined,
            payHeadIDs: selectedValues.payHeadIDs || undefined,
            monthIDs: selectedValues.monthIDs || undefined,
            yearIDs: selectedValues.yearIDs || undefined
        };

        $.ajax({
            url: '/AdvanceLoanAdjustmentReport/GetAdvancePayReportGrouped',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(filterData),
            success: function (response) {               
                hideLoading();
                if (response.success) {
                    if (isPdf) {
                        generatePDF(response.data);
                    } else if (isWord) {
                        downloadWordReport(response.data);
                    } else {
                        generatePDF(response.data);
                    }
                } else {
                    showError(response.message || 'Failed to retrieve grouped data');
                }
            },
            error: function (xhr, status, error) {
               
                hideLoading();
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                } else if (xhr.responseText) {
                    try {
                        var errorObj = JSON.parse(xhr.responseText);
                        errorMessage = errorObj.message || errorMessage;
                    } catch (e) {
                        errorMessage = xhr.responseText;
                    }
                }
                showError(errorMessage);
            }
        });
    }
    initializeLoadDataFromAdvances = function () {  
        showLoading();
        var filterData = {
            companyCodes: selectedValues.companyCodes || undefined,
            branchCodes: selectedValues.branchCodes || undefined,
            departmentCodes: selectedValues.departmentCodes || undefined,
            designationCodes: selectedValues.designationCodes || undefined,
            employeeIDs: selectedValues.employeeIDs || undefined,
            payHeadIDs: selectedValues.payHeadIDs || undefined,
            monthIDs: selectedValues.monthIDs || undefined,
            yearIDs: selectedValues.yearIDs || undefined
        };       
        showLoading();
        $.ajax({
            url: '/AdvanceLoanAdjustmentReport/GetAdvancePayFilterReport',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(filterData),
            success: function (response) {
                hideLoading();
                            const data = response;
                            fieldData.companyCodes = data.companies || [];
                            fieldData.branchCodes = data.branches || [];
                            fieldData.departmentCodes = data.departments || [];
                            fieldData.designationCodes = data.designations || [];
                            fieldData.employeeIDs = data.employees || [];
                            fieldData.payHeadIDs = data.payHeads || [];
                            fieldData.monthIDs = data.months || [];
                            fieldData.yearIDs = data.years || [];

                            document.querySelectorAll('.multiselect-container').forEach(container => {
                                const field = container.dataset.field;
                                const optionsContainer = container.querySelector('.multiselect-options');
                                const searchInput = container.querySelector('.multiselect-search input');

                                selectedValues[field] = []; 
                                populateOptions(field, optionsContainer, searchInput ? searchInput.value : '');
                                updateDisplay(field);
                            });


            },
            error: function (xhr, status, error) {
               
                hideLoading();
                let message = "Error fetching report.";
                if (xhr.responseJSON?.message) message = xhr.responseJSON.message;
                else if (xhr.responseText) message = xhr.responseText;
               
                alert(message);
            }
        });
    }
    function showError(message) {
        $('#errorText').text(message);
        $('#errorMessage').show();
    }

    

    async function loadImageBase64(url) {
        try {
            // URL validation
            if (!url || typeof url !== 'string') {
                throw new Error('Invalid URL provided');
            }

            // Fetch the image
            const response = await fetch(url);

            // Check if response is ok
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status} - ${response.statusText}`);
            }

            // Check if it's actually an image
            const contentType = response.headers.get('content-type');
            if (!contentType || !contentType.startsWith('image/')) {
                console.warn('Content type is not an image:', contentType);
            }

            const blob = await response.blob();

            return new Promise((resolve, reject) => {
                const reader = new FileReader();

                reader.onloadend = () => {
                    resolve(reader.result);
                };

                reader.onerror = () => {
                    reject(new Error('Failed to read file as base64'));
                };

                reader.readAsDataURL(blob);
            });

        } catch (error) {
            console.error('Error loading image as base64:', error);
            throw error; // Re-throw to let caller handle it
        }
    }

    // Usage example with error handling:
    async function useLoadImageBase64() {
        try {
            const base64Image = await loadImageBase64('/path/to/image.jpg');
            console.log('Image loaded successfully:', base64Image);
            // Use the base64Image here
        } catch (error) {
            console.error('Failed to load image:', error.message);
            // Handle the error appropriately
        }
    }

    const now = new Date();
    const formatDateTime = (date) => {
        const pad = (n) => n < 10 ? '0' + n : n;

        const day = pad(date.getDate());
        const month = pad(date.getMonth() + 1);
        const year = date.getFullYear();

        let hours = date.getHours();
        const minutes = pad(date.getMinutes());
        const seconds = pad(date.getSeconds());

        const ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12 || 12;
        const formattedTime = `${pad(hours)}:${minutes}:${seconds} ${ampm}`;

        return `${day}/${month}/${year} ${formattedTime}`;
    };

    async function generatePDF(groupedResponse) {
        if (!groupedResponse || groupedResponse.length === 0 || !groupedResponse[0].employees || groupedResponse[0].employees.length === 0) {
            alert("Data not Found");
            return; 
        }

        const { jsPDF } = window.jspdf;
        const doc = new jsPDF('p', 'mm', 'a4');
        const pageWidth = doc.internal.pageSize.getWidth();
        const pageHeight = doc.internal.pageSize.getHeight();

        const companyName = `${groupedResponse[0].employees[0].companyName}`;
        const reportName = "Advance Adjustment Report";
        const reportDetails = `For the month of ${groupedResponse[0].employees[0].monthName} - ${groupedResponse[0].employees[0].salaryYear}`;
        const createdBy = `Print Datetime: ${formatDateTime(now)} || ${groupedResponse[0].luser}`;
        const logoUrl = '/images/DP_logo.png';

        let logo = null;
        try {
            logo = await loadImageBase64(logoUrl);
        } catch (e) { }

        let currentY = 30;
        let currentDepartmentName = "";
        let isTableActive = false;
        const headerHeight = 35; // Fixed header height including department name

        function drawHeader(showDepartment = false) {
            doc.setFontSize(14);
            doc.setFont(undefined, 'bold');

            if (logo) {
                doc.addImage(logo, 'PNG', 5, 2, 40, 17);
            }

            doc.text(companyName, pageWidth / 2, 10, { align: 'center' });
            doc.setFontSize(10);
            doc.setFont(undefined, 'normal');
            doc.text(reportName, pageWidth / 2, 15, { align: 'center' });

            const textWidth = doc.getTextWidth(reportName);
            const centerX = pageWidth / 2;
            const lineY = 16;
            const lineStartX = centerX - textWidth / 2;
            const lineEndX = centerX + textWidth / 2;
            doc.setLineWidth(.05);
            doc.line(lineStartX, lineY, lineEndX, lineY);

            doc.setFontSize(8);
            doc.setFont(undefined, 'normal');
            doc.text(reportDetails, pageWidth / 2, 19, { align: 'center' });

            if (showDepartment && currentDepartmentName) {
                doc.setFontSize(12);
                doc.setFont(undefined, 'bold');
                doc.text(currentDepartmentName, 10, 28);
            }
        }

        function drawFooter() {
            const pageCount = doc.internal.getNumberOfPages();
            for (let i = 1; i <= pageCount; i++) {
                doc.setPage(i);
                doc.setFontSize(9);
                doc.setFont(undefined, 'normal');
                doc.text(`Page ${i} of ${pageCount}`, 5, pageHeight - 2);
                doc.text(createdBy, pageWidth - 5, pageHeight - 2, { align: 'right' });
            }
        }

        drawHeader(false);

        let totalAdvanceAmountGrant = 0;
        let totalMonthlyDeductionGrant = 0;

        for (let group of groupedResponse) {
            currentDepartmentName = `Department: ${group.departmentName}`;
            isTableActive = true;

            let totalAdvanceAmountDepartment = 0;
            let totalMonthlyDeductionDepartment = 0;

            group.employees.forEach(emp => {
                totalAdvanceAmountGrant += emp.advanceAmount;
                totalMonthlyDeductionGrant += emp.monthlyDeduction;

                totalAdvanceAmountDepartment += emp.advanceAmount;
                totalMonthlyDeductionDepartment += emp.monthlyDeduction;
            });

            // Check if we need a new page for department header and some table content
            if (currentY > pageHeight - 80) {
                doc.addPage();
                currentY = headerHeight;
                drawHeader(true);
            } else {
                // Draw department name on same page
                doc.setFontSize(12);
                doc.setFont(undefined, 'bold');
                doc.text(currentDepartmentName, 10, currentY);
                currentY += 4;
            }

            doc.autoTable({
                startY: currentY,
                head: [[
                    'SN', 'Employee ID', 'Name', 'Designation', 'Branch',
                    'Advance Amount', 'Monthly Deduction', 'Remarks'
                ]],
                body: group.employees.map((emp, index) => [
                    index + 1,
                    emp.employeeID,
                    emp.fullName,
                    emp.designationName,
                    emp.branchName,
                    emp.advanceAmount.toLocaleString(undefined, { minimumFractionDigits: 0 }),
                    emp.monthlyDeduction.toLocaleString(undefined, { minimumFractionDigits: 0 }),
                    emp.remarks
                ]),
                margin: { left: 10, right: 10, top: headerHeight },
                columnStyles: {
                    0: { cellWidth: 10 },
                    1: { cellWidth: 25 }, 
                    2: { cellWidth: 30, halign: 'left' }, 
                    3: { cellWidth: 24 },
                    4: { cellWidth: 22 }, 
                    5: { cellWidth: 24, halign: 'right' },
                    6: { cellWidth: 28, halign: 'right' }, 
                    7: { cellWidth: 'auto', halign: 'left' }
                },

                styles: {
                    halign: 'center',
                    valign: 'middle',
                    lineWidth: 0.1,
                    lineColor: [0, 0, 0],
                    fontSize: 10,
                    cellPadding: 2,
                    textColor: 0,
                },
                headStyles: {
                    halign: 'center',
                    valign: 'middle',
                    fillColor: [255, 255, 255],
                    textColor: 0,
                    fontStyle: 'bold',
                    lineWidth: 0.2,
                    lineColor: [0, 0, 0]
                },
                alternateRowStyles: {
                    fillColor: [255, 255, 255]
                },
                didDrawPage: function (data) {
                    // Only draw header on new pages (not the first page of the table)
                    if (data.pageCount > 1) {
                        drawHeader(true);
                    }
                },
                // Ensure proper page breaking
                pageBreak: 'auto',
                rowPageBreak: 'avoid',
                // Set minimum space for table continuation
                tableLineColor: [0, 0, 0],
                tableLineWidth: 0.1,
            });

            isTableActive = false;
            currentY = doc.lastAutoTable.finalY + 5;

            // Check if we need new page for department total
            if (currentY > pageHeight - 25) {
                doc.addPage();
                currentY = headerHeight;
                drawHeader(false);
            }

            doc.setFontSize(10);
            doc.setFont(undefined, 'bold');
            doc.text(`Total: `, 112, currentY);
            doc.text(totalAdvanceAmountDepartment.toLocaleString(undefined, { minimumFractionDigits: 0 }), 132, currentY);
            doc.text(totalMonthlyDeductionDepartment.toLocaleString(undefined, { minimumFractionDigits: 0 }), 159, currentY);

            currentY += 8; // Add more space between departments
        }

        // Grand total section
        currentY = doc.lastAutoTable.finalY + 15;

        if (currentY > pageHeight - 25) {
            doc.addPage();
            currentY = headerHeight;
            drawHeader(false);
        }

        doc.setFontSize(11);
        doc.setFont(undefined, 'bold');
        doc.text(`Grant Total:`, 100, currentY);
        doc.text(totalAdvanceAmountGrant.toLocaleString(undefined, { minimumFractionDigits: 0 }), 128, currentY);
        doc.text(totalMonthlyDeductionGrant.toLocaleString(undefined, { minimumFractionDigits: 0 }), 157, currentY);

        drawFooter();

        if (isPdf) {
            doc.save("AdvancePayReport.pdf");
        } else {
            const pdfBlob = doc.output('blob');
            const blobUrl = URL.createObjectURL(pdfBlob);
            document.getElementById('pdfViewer').src = blobUrl;
        }
    }


    function downloadWordReport(groupedResponse) {
        try {
            if (!groupedResponse || groupedResponse.length === 0) {
                alert("No data found!");
                return;
            }
            const companyName = groupedResponse[0].employees[0].companyName || "";
            const reportName = "Advance Adjustment Report";
            const salaryMonth = groupedResponse[0].employees[0].monthName || "";
            const salaryYear = groupedResponse[0].employees[0].salaryYear || "";
            const createdBy = groupedResponse[0].luser || "";
            const now = new Date().toLocaleString('en-US', {


                year: 'numeric', month: 'short', day: 'numeric',
                hour: 'numeric', minute: 'numeric', hour12: true
            });

            // Initialize grand totals
            let totalAdvanceAmountGrant = 0;
            let totalMonthlyDeductionGrant = 0;

            let htmlContent = `
<!DOCTYPE html>
<html xmlns:v="urn:schemas-microsoft-com:vml" 
      xmlns:o="urn:schemas-microsoft-com:office:office" 
      xmlns:w="urn:schemas-microsoft-com:office:word" 
      xmlns:m="http://schemas.microsoft.com/office/2004/12/omml" 
      xmlns="http://www.w3.org/TR/REC-html40">
<head>
    <meta charset="utf-8">
    <title>${reportName}</title>
    <!--[if gte mso 9]>
    <xml>
    <w:WordDocument>
        <w:View>Print</w:View>
        <w:Zoom>90</w:Zoom>
        <w:DoNotPromptForConvert/>
        <w:DoNotShowInsertionsAndDeletions/>
    </w:WordDocument>
    </xml>
    <![endif]-->
    <style>
        @page Section1 { size: 595.3pt 841.9pt; mso-page-orientation: portrait; margin: 0.5in; }
        div.Section1 { page: Section1; }
        body { font-family: 'Times New Roman', serif; margin: 0; padding: 0; }
        .header { text-align: center; margin-top: 10px; font-size: 20px; font-weight: bold; }
        .sub-header { text-align: center; font-size: 14px; margin-top: 5px; }
        .top-date { text-align: center; font-size: 12px; margin-top: 5px; }
        h2 { font-size: 16px; font-weight: bold; margin: 20px 0 10px; color: #333; padding-bottom: 5px; display: inline-block; }
        table { border-collapse: collapse; width: 100%; margin: 0; }
        table, th, td { border: 1px solid black; padding: 4px 5px; margin: 0; font-size: 12px; vertical-align: top; line-height: 1.2; }
        th { background-color: #f0f0f0; font-weight: bold; text-align: center; }
        td { text-align: left; }
        .total-row { font-weight: bold; background-color: #f5f5f5; }
        .grand-total-row { font-weight: bold; background-color: #e0e0e0; font-size: 14px; }
        .footer { margin-top: 20px; font-size: 12px; }
        .page-info { text-align: left; display: inline-block; width: 50%; }
        .date-user { text-align: right; display: inline-block; width: 49%; }
    </style>
</head>
<body>
<div class="Section1">
    <div class="header">${companyName}</div>
    <div class="sub-header">${reportName}</div>
    <h6 class="top-date">For the ${salaryMonth} of ${salaryYear}</h6>
    <h6 class="top-date">Generated on: ${now} | Created by: ${createdBy}</h6>
`;

            groupedResponse.forEach(group => {
                // Initialize department totals
                let totalAdvanceAmountDepartment = 0;
                let totalMonthlyDeductionDepartment = 0;

                htmlContent += `<h2>Department: ${group.departmentName || "Unknown"}</h2>`;
                htmlContent += `<table>
                <thead>
                    <tr>
                        <th style="width:5%;">SN</th>
                        <th style="width:12%;">Employee ID</th>
                        <th style="width:25%;">Name</th>
                        <th style="width:15%;">Designation</th>
                        <th style="width:15%;">Branch</th>
                        <th style="width:12%; text-align:center;">Advance Amount</th>
                        <th style="width:12%; text-align:center;">Monthly Deduction</th>
                        <th style="width:150px;">Remarks</th>
                    </tr>
                </thead>
                <tbody>
            `;

                group.employees.forEach((emp, index) => {
                    const advanceAmount = parseFloat(emp.advanceAmount) || 0;
                    const monthlyDeduction = parseFloat(emp.monthlyDeduction) || 0;

                    // Add to department totals
                    totalAdvanceAmountDepartment += advanceAmount;
                    totalMonthlyDeductionDepartment += monthlyDeduction;

                    htmlContent += `
                    <tr>
                        <td style="text-align:center;">${++index || ''}</td>
                        <td style="text-align:center;">${emp.employeeID || ''}</td>
                        <td>${emp.fullName || ''}</td>
                        <td>${emp.designationName || ''}</td>
                        <td>${emp.branchName || ''}</td>
                        <td style="text-align:right;">${advanceAmount.toFixed(2)}</td>
                        <td style="text-align:right;">${monthlyDeduction.toFixed(2)}</td>
                        <td>${emp.remarks || ''}</td>
                    </tr>
                `;
                });

                // Add department total row
                htmlContent += `
                <tr class="total-row">
                    <td colspan="5" style="text-align:right; font-weight:bold;">Total:</td>
                    <td style="text-align:right; font-weight:bold;">${totalAdvanceAmountDepartment.toFixed(2)}</td>
                    <td style="text-align:right; font-weight:bold;">${totalMonthlyDeductionDepartment.toFixed(2)}</td>
                    <td></td>
                </tr>
            `;

                htmlContent += `</tbody></table><br>`;

                // Add to grand totals
                totalAdvanceAmountGrant += totalAdvanceAmountDepartment;
                totalMonthlyDeductionGrant += totalMonthlyDeductionDepartment;
            });

            // Add grand total section
            htmlContent += `
            <table style="margin-top: 20px;">
                <tr class="grand-total-row">
                    <td colspan="4" style="text-align:right; font-weight:bold; font-size:14px;">Grand Total:</td>
                    <td style="text-align:right; font-weight:bold; font-size:14px; width:12%;">${totalAdvanceAmountGrant.toFixed(2)}</td>
                    <td style="text-align:right; font-weight:bold; font-size:14px; width:12%;">${totalMonthlyDeductionGrant.toFixed(2)}</td>
                    <td style="width:60px;"></td>
                </tr>
            </table>
        `;

            htmlContent += `
   
</div>
</body>
</html>`;

            const blob = new Blob([htmlContent], { type: 'application/msword' });
            const url = URL.createObjectURL(blob);
            const link = document.createElement("a");
            link.href = url;
            link.download = "AdvanceAdjustmentReport.doc";
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);

        } catch (error) {
            alert("Error generating document: " + error.message);
        }
    }



    init = function () {
        initializeLoadDataFromAdvances();
        setupLoadingOverlay();
    }
    init();
})(jQuery);
