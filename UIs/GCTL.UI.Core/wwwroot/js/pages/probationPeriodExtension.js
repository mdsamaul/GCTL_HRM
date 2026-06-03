(function ($) {
    $.probationPeriodExtension = function (options) {

        //#region Default options

        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#ProbationPeriodExtension-form",
            formContainer: ".js-ProbationPeriodExtension-form-container",
            gridSelector: "#ProbationPeriodExtension-grid",
            gridContainer: ".js-ProbationPeriodExtension-grid-container",
            editSelector: ".js-ProbationPeriodExtension-edit",
            saveSelector: ".js-ProbationPeriodExtension-save",
            selectAllSelector: "#ProbationPeriodExtension-check-all",
            deleteSelector: ".js-ProbationPeriodExtension-delete-confirm",
            deleteModal: "#ProbationPeriodExtension-delete-modal",
            finalDeleteSelector: ".js-ProbationPeriodExtension-delete",
            clearSelector: ".js-ProbationPeriodExtension-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-ProbationPeriodExtension-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-ProbationPeriodExtension-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            Period: "#Period",
            Extended: "#Extended",
            ProbationPeriod:"#ProbationPeriod",
            load: function () {

            }
        }, options);

        var gridUrl = settings.baseUrl + "/Grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        let ProbationPeriodExtensionTable = null;

        //#endregion

        $(() => {
            initialize();
            loadProbationData();
            enterKeyNavigation();
            loadTable();
            ResetForm();
            loadSNCId();
            selectMonth();

            //#region filter company & employee

            $('#EmployeeId').on('change', function () {
                const employeeId = $(this).val();
                const companyCode = $('#CompanyCode').val();
                loadProbationDataDetails(employeeId, companyCode);
            });

            function loadProbationData(employeeId, companyCode) {
                $.ajax({
                    url: '/ProbationPeriodExtension/GetProbationData',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        EmployeeID: employeeId,
                        CompanyCode: companyCode
                    }),
                    success: function (data) {
                      //  console.log(data.employeeList);

                        // ✅ Update Company dropdown
                        const companySelect = $('#CompanyCode');
                        companySelect.empty(); // Clear existing options
                        // companySelect.append('<option value="">Select Company</option>'); 

                        data.companyList.forEach(function (company) {
                            companySelect.append(
                                `<option value="${company.companyCode}">${company.companyName}</option>`
                            );
                        });

                        // ✅ If using Bootstrap selectpicker or similar plugin
                        // if (companySelect.hasClass('Expselectpicker')) {
                        //     companySelect.selectpicker('refresh');
                        // }
                        const employeeSelect = $('#EmployeeId');
                        employeeSelect.empty(); // Clear existing options
                        employeeSelect.append('<option value="">Select Employee</option>');

                        data.employeeList.forEach(function (emp) {
                            employeeSelect.append(
                                `<option value="${emp.employeeID}">${emp.fullName}(${emp.employeeID})</option>`
                            );
                        });

                    },
                    error: function (xhr, status, error) {
                        alert('Failed to load probation data: ' + error);
                        console.error(xhr.responseText);
                    }
                });
            }

            function loadProbationDataDetails(employeeId, companyCode) {
                try {
                    $.ajax({
                        url: '/ProbationPeriodExtension/GetProbationData',
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify({
                            EmployeeID: employeeId,
                            CompanyCode: companyCode
                        }),
                        success: function (data) {
                            var empDetails = data.fullList;
                            $("#FullName").text(empDetails[0].fullName);
                            $("#DepartmentName").text(empDetails[0].departmentName);
                            $("#DesignationName").text(empDetails[0].designationName);
                            $("#GrossSalary").text(empDetails[0].grossSalary);
                            $("#JoiningDate").text(empDetails[0].showJoiningDate);
                            $("#ProbationPeriod").text(empDetails[0].probationPeriod);
                            $("#ContractEndDate").text(empDetails[0].showContractEndDate);
                            $("#DurationSinceJoining").text(empDetails[0].durationSinceJoining)
                        },
                        error: function (xhr, status, error) {
                            alert('Failed to load probation data: ' + error);
                            console.error(xhr.responseText);
                        }
                    });

                } catch (e) {

                }
            }

            // #endregion

            //#region formatDateTime

            function formatDateTime(dateStr) {
                var date = new Date(dateStr);
                return date.toLocaleString('en-GB', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                    hour12: true
                });
            }

            //#endregion

            //#region flatpickr

            $(document).ready(function () {
                flatpickr('.flatpickr', CalendarService.createConfig({
                    dateFormat: "Y-m-d",
                    altInput: true,
                    altFormat: "d/m/Y",
                    allowInput: true,
                    onReady: function (selectedDates, dateStr, instance) {
                        instance.input.placeholder = "dd/mm/yyyy";
                    }
                }));
            })

            //#endregion

            //#region GetById

            $(document).on('click', '.js-ProbationPeriodExtension-edit', function (e) {
                e.preventDefault();

                var id = $(this).data('id');

                const companyCode = $('#CompanyCode').val();
                

                $.ajax({
                    url: '/ProbationPeriodExtension/GetById',
                    type: 'GET',
                    data: { code: id }, // Updated from ppeid to match controller signature
                    success: function (data) {
                        if (data) {
                           // console.log(data);
                            $('#AutoId').val(data.autoId);
                            $('#Ppeid').val(data.ppeid);
                            $('#EmployeeId').val(data.employeeId);
                            $('#Extended').val(data.extended); // This is your number field
                            $('#Period').val(data.periodInfoId).trigger('change');

                            const wefParts = data.wef.split('/');
                            const parsedWefDate = new Date(wefParts[2], wefParts[1] - 1, wefParts[0]);

                            if ($('#Wef')[0]._flatpickr) {
                                $('#Wef')[0]._flatpickr.setDate(parsedWefDate, true);
                            } else {
                                $('#Wef').val(data.wef);
                            }

                            $('#ExtensionSalary').val(data.extensionSalary);
                            $('#RefLetterNo').val(data.refLetterNo);

                            const dateParts = data.refLetterDate.split('/');
                            const parsedDate = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]); 
                            if ($('#RefLetterDate')[0]._flatpickr) {
                                $('#RefLetterDate')[0]._flatpickr.setDate(parsedDate, true);
                            } else {
                                $('#RefLetterDate').val(data.refLetterDate); // just use original string
                            }

                            $('#Remarks').val(data.remarks);
                            $('#Luser').val(data.luser);
                            $('#Lip').val(data.lip);
                            $('#Lmac').val(data.lmac);

                            $('#LdateModifyHide').show();
                            $('#sectionBreak').show();

                            if (data.ldate) {
                                $('#creationDateSpan').text(formatDateTime(data.ldate));
                            } else {
                                $('#creationDateSpan').text('');
                            }

                            if (data.modifyDate) {
                                $('#modifyDateSpan').text(formatDateTime(data.modifyDate));
                            } else {
                                $('#modifyDateSpan').text('');
                            }

                            LoadProvisitionData(data)

                        }
                    },
                    error: function () {
                        alert('Failed to load probation period extension data.');
                    }
                });
            });


            function LoadProvisitionData(data) {
         
                $("#EmployeeId").val(data.employeeId).trigger('change');
                $("#FullName").text(data.employeeName);
                $("#DepartmentName").text(data.departmentName);
                $("#DesignationName").text(data.designationName);
                $("#GrossSalary").text(data.grossSalary);
                $("#JoiningDate").text(data.joiningDate);
                $("#ProbationPeriod").text(data.probationPeriod);
                $("#ContractEndDate").text(data.contractEndDate);
                $("#DurationSinceJoining").text(data.durationSinceJoining)
            }

            // #endregion

            //#region Save

            $(document).on("click", settings.saveSelector, function () {
                if (typeof validation === 'function' && !validation()) return false;
                const $form = $(settings.formSelector);
                const $saveButton = $(settings.saveSelector);

                // Prepare data
                let data = settings.haseFile ? new FormData($form[0]) : $form.serialize();
                console.log(data);
                $saveButton.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Saving...');

                $.ajax({
                    url: saveUrl,
                    method: "POST",
                    data: data,
                    processData: !settings.haseFile,
                    contentType: settings.haseFile ? false : "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (response) {
                        if (response.isSuccess) {
                            toastr.success(response.message);
                            // Refresh table
                            loadTable();
                            ResetForm();
                            // Update the PPEID field (if needed)
                            $(".js-Ppeid-code").val(response.lastCode);

                           // loadProbationDataDetails(response.employeeId, response.compCode)

                        } else {
                            toastr.warning(response.message || "Save failed");
                        }
                    },
                    error: function (xhr, status, error) {
                        toastr.error("An error occurred while saving the data.");
                        console.error("Ajax error:", status, error, xhr.responseText);
                    },
                    complete: function () {
                        $saveButton.prop('disabled', false).html('Save');
                    }
                });
            });


            //#endregion

            //#region Add Per Service Length

            let originalProbationNumber = null;
            let originalUnit = "";
            let originalContractEndDate = null;
            let joiningDate = null;
            let lastInputDays = 0;

            // When Period dropdown changes
            $(settings.Period).on('change', function () {
                let periodValue = $(this).val();

                if (periodValue === '01' || periodValue === '06' || periodValue === '05' || periodValue === '04') {
                    let probationText = $(settings.ProbationPeriod).text().trim(); // e.g., "14 days"
                    let parts = probationText.split(" ");
                    originalProbationNumber = parseInt(parts[0]) || 0;
                    originalUnit = parts[1] || getUnitByPeriod(periodValue);

                    // Store original contract end date
                    let contractDateStr = $('#ContractEndDate').text().trim();
                    originalContractEndDate = parseDateFromDDMMYYYY(contractDateStr);

                    // Store joining date
                    let joiningDateStr = $('#JoiningDate').text().trim();
                    joiningDate = parseDateFromDDMMYYYY(joiningDateStr);

                    lastInputDays = 0;

                    console.log("Period changed to:", periodValue);
                    console.log("Original Probation Number set:", originalProbationNumber);
                    console.log("Original Unit set:", originalUnit);
                    console.log("Original Contract End Date stored:", contractDateStr);
                    console.log("Joining Date stored:", joiningDateStr);
                }
            });

            // When user types in Extended input

            $(settings.Extended).on('input', function () {
                console.log("Extended input triggered");

                if (originalProbationNumber === null || originalContractEndDate === null || joiningDate === null) {
                    console.log("Original values not initialized yet.");
                    return;
                }

                let inputValue = $(this).val().trim();
                let periodListId = $(settings.Period).val();

                console.log("Input value:", inputValue);
                console.log("Period List ID:", periodListId);

                if (periodListId === '01' || periodListId === '06' || periodListId === '05' || periodListId === '04') {
                    // If input is empty, reset to original values
                    if (inputValue === "") {
                        console.log("Empty input. Resetting to original values.");
                        // Reset probation period - always show in days for consistency
                        $(settings.ProbationPeriod).text(originalProbationNumber + " days");
                        $('#ContractEndDate').text(formatDateToDDMMYYYY(originalContractEndDate));
                        lastInputDays = 0;
                        return;
                    }

                    // If input is not a valid number, don't proceed
                    if (isNaN(inputValue)) {
                        console.log("Invalid input. Keeping previous values.");
                        return;
                    }

                    let inputNumber = parseInt(inputValue);

                    // Calculate actual days for display based on period type
                    let actualDaysToAdd = 0;
                    if (periodListId === '01') {
                        // For years, calculate actual days by adding years to original contract end date
                        let tempDate = new Date(originalContractEndDate);
                        let dateAfterYears = addYears(tempDate, inputNumber);
                        actualDaysToAdd = Math.floor((dateAfterYears - originalContractEndDate) / (1000 * 60 * 60 * 24));
                    } else if (periodListId === '04') {
                        // For months, calculate actual days by adding months to original contract end date
                        let tempDate = new Date(originalContractEndDate);
                        let dateAfterMonths = addMonths(tempDate, inputNumber);
                        actualDaysToAdd = Math.floor((dateAfterMonths - originalContractEndDate) / (1000 * 60 * 60 * 24));
                    } else if (periodListId === '05') {
                        // For weeks, convert to days (1 week = 7 days)
                        actualDaysToAdd = inputNumber * 7;
                    } else {
                        // For days
                        actualDaysToAdd = inputNumber;
                    }

                    // Update probation period display with actual days
                    let totalDays = originalProbationNumber + actualDaysToAdd;
                    $(settings.ProbationPeriod).text(totalDays + " days");
                    console.log("Updated probation text with actual days:", totalDays + " days");
                    console.log("Actual days added:", actualDaysToAdd);

                    // Calculate new contract end date based on period type
                    let newDateObj = calculateNewContractEndDate(periodListId, inputNumber);
                    let newDateStr = formatDateToDDMMYYYY(newDateObj);

                    console.log("Period ID:", periodListId);
                    console.log("Input Number:", inputNumber);
                    if (periodListId === '01') {
                        console.log("Years - Actual days calculated:", actualDaysToAdd);
                    } else if (periodListId === '04') {
                        console.log("Months - Actual days calculated:", actualDaysToAdd);
                    } else if (periodListId === '05') {
                        console.log("Weeks converted to days:", inputNumber * 7);
                    }
                    console.log("Original Contract End Date:", formatDateToDDMMYYYY(originalContractEndDate));
                    console.log("Calculated New Date Object:", newDateObj);
                    console.log("New date string:", newDateStr);

                    // Update ContractEndDate span
                    $('#ContractEndDate').text(newDateStr);

                    // Update last input
                    lastInputDays = inputNumber;

                    console.log("Updated Contract End Date:", newDateStr);
                }
            });

            // Calculate new contract end date based on period type
            function calculateNewContractEndDate(periodId, inputNumber) {
                // Start from original contract end date and add the extended period
                let baseDate = new Date(originalContractEndDate);

                if (periodId === '01') {
                    // Years - add input years to original contract end date
                    return addYears(baseDate, inputNumber);
                } else if (periodId === '06') {
                    // Days - add input days to original contract end date
                    return addDays(baseDate, inputNumber);
                } else if (periodId === '05') {
                    // Weeks - convert weeks to days (1 week = 7 days)
                    let daysToAdd = inputNumber * 7;
                    return addDays(baseDate, daysToAdd);
                } else if (periodId === '04') {
                    // Months - use proper month addition to handle varying month lengths
                    return addMonths(baseDate, inputNumber);
                }

                return baseDate;
            }

            // Get unit name by period ID
            function getUnitByPeriod(periodId) {
                if (periodId === '01') return 'year';
                if (periodId === '04') return 'months';
                if (periodId === '05') return 'weeks';
                if (periodId === '06') return 'days';
                return 'days';
            }

            // Parse DD/MM/YYYY string to Date object
            function parseDateFromDDMMYYYY(dateStr) {
                if (!dateStr) return null;

                let parts = dateStr.split('/');
                if (parts.length !== 3) return null;

                let day = parseInt(parts[0], 10);
                let month = parseInt(parts[1], 10) - 1;
                let year = parseInt(parts[2], 10);

                // Validate date parts
                if (isNaN(day) || isNaN(month) || isNaN(year)) return null;
                if (day < 1 || day > 31 || month < 0 || month > 11 || year < 1900) return null;

                let d = new Date(year, month, day);
                return isNaN(d.getTime()) ? null : d;
            }

            // Format Date object to DD/MM/YYYY string
            function formatDateToDDMMYYYY(date) {
                if (!date || isNaN(date.getTime())) return '';

                let day = String(date.getDate()).padStart(2, '0');
                let month = String(date.getMonth() + 1).padStart(2, '0');
                let year = date.getFullYear();
                return `${day}/${month}/${year}`;
            }

            // Add days to a Date object and return new Date
            function addDays(date, days) {
                if (!date || isNaN(date.getTime()) || isNaN(days)) return date;

                let result = new Date(date);
                result.setDate(result.getDate() + days);
                return result;
            }

            // Add weeks to a Date object and return new Date
            function addWeeks(date, weeks) {
                if (!date || isNaN(date.getTime()) || isNaN(weeks)) return date;

                let result = new Date(date);
                let daysToAdd = weeks * 7;
                result.setDate(result.getDate() + daysToAdd);

                console.log("addWeeks - Original date:", formatDateToDDMMYYYY(date));
                console.log("addWeeks - Adding weeks:", weeks);
                console.log("addWeeks - Days to add:", daysToAdd);
                console.log("addWeeks - Result date:", formatDateToDDMMYYYY(result));

                return result;
            }

            // Add months to a Date object and return new Date
            function addMonths(date, months) {
                if (!date || isNaN(date.getTime()) || isNaN(months)) return date;

                let result = new Date(date);
                let originalDay = result.getDate();

                // Add months
                result.setMonth(result.getMonth() + months);

                // Handle edge case where day doesn't exist in target month
                // For example, Jan 31 + 1 month should be Feb 28/29
                if (result.getDate() !== originalDay) {
                    result.setDate(0); // Set to last day of previous month
                }

                console.log("addMonths - Original date:", formatDateToDDMMYYYY(date));
                console.log("addMonths - Adding months:", months);
                console.log("addMonths - Result date:", formatDateToDDMMYYYY(result));

                return result;
            }

            // Add years to a Date object and return new Date
            function addYears(date, years) {
                if (!date || isNaN(date.getTime()) || isNaN(years)) return date;

                let result = new Date(date);
                result.setFullYear(result.getFullYear() + years);

                // Handle leap year edge case (Feb 29 + 1 year)
                if (result.getMonth() !== date.getMonth()) {
                    result.setDate(0); // Set to last day of previous month
                }

                console.log("addYears - Original date:", formatDateToDDMMYYYY(date));
                console.log("addYears - Adding years:", years);
                console.log("addYears - Result date:", formatDateToDDMMYYYY(result));

                return result;
            }

            // Optional: Add initialization function to set original values on page load
            function initializeProbationData() {
                let periodValue = $(settings.Period).val();

                if (periodValue === '01') {
                    // Years
                    let probationText = $(settings.ProbationPeriod).text().trim();
                    let parts = probationText.split(" ");
                    originalProbationNumber = parseInt(parts[0]) || 0;
                    originalUnit = parts[1] || "year";

                    // Store original contract end date
                    let contractDateStr = $('#ContractEndDate').text().trim();
                    originalContractEndDate = parseDateFromDDMMYYYY(contractDateStr);

                    // Store joining date
                    let joiningDateStr = $('#JoiningDate').text().trim();
                    joiningDate = parseDateFromDDMMYYYY(joiningDateStr);

                    lastInputDays = 0;

                    console.log("Initialized - Years - Original Probation Number:", originalProbationNumber);
                    console.log("Initialized - Years - Original Contract End Date:", contractDateStr);
                    console.log("Initialized - Years - Joining Date:", joiningDateStr);

                } else if (periodValue === '06') {
                    // Days
                    let probationText = $(settings.ProbationPeriod).text().trim();
                    let parts = probationText.split(" ");
                    originalProbationNumber = parseInt(parts[0]) || 0;
                    originalUnit = parts[1] || "days";

                    // Store original contract end date
                    let contractDateStr = $('#ContractEndDate').text().trim();
                    originalContractEndDate = parseDateFromDDMMYYYY(contractDateStr);

                    // Store joining date
                    let joiningDateStr = $('#JoiningDate').text().trim();
                    joiningDate = parseDateFromDDMMYYYY(joiningDateStr);

                    lastInputDays = 0;

                    console.log("Initialized - Days - Original Probation Number:", originalProbationNumber);
                    console.log("Initialized - Days - Original Contract End Date:", contractDateStr);
                    console.log("Initialized - Days - Joining Date:", joiningDateStr);

                } else if (periodValue === '05') {
                    // Weeks
                    let probationText = $(settings.ProbationPeriod).text().trim();
                    let parts = probationText.split(" ");
                    originalProbationNumber = parseInt(parts[0]) || 0;
                    originalUnit = parts[1] || "weeks";

                    // Store original contract end date
                    let contractDateStr = $('#ContractEndDate').text().trim();
                    originalContractEndDate = parseDateFromDDMMYYYY(contractDateStr);

                    // Store joining date
                    let joiningDateStr = $('#JoiningDate').text().trim();
                    joiningDate = parseDateFromDDMMYYYY(joiningDateStr);

                    lastInputDays = 0;

                    console.log("Initialized - Weeks - Original Probation Number:", originalProbationNumber);
                    console.log("Initialized - Weeks - Original Contract End Date:", contractDateStr);
                    console.log("Initialized - Weeks - Joining Date:", joiningDateStr);

                } else if (periodValue === '04') {
                    // Months
                    let probationText = $(settings.ProbationPeriod).text().trim();
                    let parts = probationText.split(" ");
                    originalProbationNumber = parseInt(parts[0]) || 0;
                    originalUnit = parts[1] || "months";

                    // Store original contract end date
                    let contractDateStr = $('#ContractEndDate').text().trim();
                    originalContractEndDate = parseDateFromDDMMYYYY(contractDateStr);

                    // Store joining date
                    let joiningDateStr = $('#JoiningDate').text().trim();
                    joiningDate = parseDateFromDDMMYYYY(joiningDateStr);

                    lastInputDays = 0;

                    console.log("Initialized - Months - Original Probation Number:", originalProbationNumber);
                    console.log("Initialized - Months - Original Contract End Date:", contractDateStr);
                    console.log("Initialized - Months - Joining Date:", joiningDateStr);
                }
            }

            // Call initialization on document ready
            $(document).ready(function () {
                initializeProbationData();
            });

            //#endregion

            //#region selectAllSelector deleteSelector 

            $("body").on("click", settings.selectAllSelector, function () {
                $(".checkBox").prop('checked',
                    $(this).prop('checked'));
            });


            $("body").on("click", settings.deleteSelector, function (e) {
                e.preventDefault();
                $('input:checkbox.checkBox').each(function () {
                    if ($(this).prop('checked')) {
                        if (!selectedItems.includes($(this).val())) {
                            selectedItems.push($(this).val());
                        }
                    }
                });

                if (selectedItems.length > 0) {
                    $(settings.deleteModal).modal("show");
                } else {
                    toastr.info("Please select at least one item.");
                }

            });

            //#endregion

            //#region Delete

            $("body").on('show.bs.modal', settings.deleteModal, function (event) {

                var source = $(event.relatedTarget);
                var id = source.data("ids");

                // Extract value from data-* attributes
                var title = source.data("title");
                title = "Are you sure want to delete these items?";
                var modal = $(this);
                $(modal).find('.title').html(title);

                $("body").on("click", settings.finalDeleteSelector, function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    e.stopImmediatePropagation();


                    // Delete
                    $.ajax({
                        url: deleteUrl,
                        method: "POST",
                        contentType: "application/json",
                        data: JSON.stringify(selectedItems),
                        success: function (response) {

                            $(modal).modal("hide");

                            if (response.success) {
                                // console.log(response);
                                loadTable();
                                ResetForm();
                                $(".js-Ppeid-code").val(response.lastCode)
                                // currentDate();
                                toastr.success(response.message);
                            }
                            else {
                                toastr.error(response.message);
                                // console.log(response);
                            }
                        }
                    });
                });

            }).on('hide.bs.modal', function () {
                $("body").off("click", settings.finalDeleteSelector);
            });

            //#endregion

            //#region topSelector decimalSelector

            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 0 }, 500);
            });


            $("body").on("keyup", settings.decimalSelector, function () {
                var self = $(this);
                showDecimalPlaces(self.val(), self.parent().find(".input-group-text"));
            });

            //#endregion

        });

        //#region loadTable

        function loadTable() {
            $.get(settings.baseUrl + "/GetTableData")
                .done(html => {
                    $(".js-ProbationPeriodExtension-grid-container").html(html);
                    if ($.fn.DataTable.isDataTable("#ProbationPeriodExtension-grid")) {
                        $("#ProbationPeriodExtension-grid").DataTable().destroy();
                    }
                    setTimeout(() => {
                        $("#ProbationPeriodExtension-grid").DataTable({
                            lengthChange: true,
                            pageLength: 10,
                            lengthMenu: [
                                [10, 25, 50, -1],
                                [10, 25, 50, 'All'],
                            ],
                            order: [[1, "desc"]],
                            destroy: true,
                            paging: true,
                            searching: true,
                            responsive: true,
                            autoWidth: false,
                            columnDefs: [
                                { targets: 0, orderable: false }
                            ]
                        });
                    }, 0);
                })
                .fail((xhr, status, error) => {
                    console.error("Error loading table:", status, error, xhr.responseText);
                    toastr.error("Failed to load table data.");
                    loadForm(url);
                });
        }

        //#endregion

        //#region loadForm

        function loadForm(url) {
            return new Promise((resolve, reject) => {
              //  var employeeId = $('#EmployeeId').val();
                $.ajax({
                    url: url,
                    type: 'GET',
                    cache: false,
                    success: function (data) {
                        $(settings.formContainer).empty();
                        $(settings.formContainer).html(data);
                        $('#EmployeeId').val(employeeId);
                        $.validator.unobtrusive.parse($(settings.formSelector));

                       
                    },
                    error: function (error) {
                        reject(error)
                    },
                })
            })
        }

        //#endregion

        //#region select2
        function initialize() {
            $(settings.formSelector + ' .selectpickerrr').select2({

                language: {
                    noResults: function () {

                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });
        }
        // #endregion

        //#region EnterKeyNavigation

        function enterKeyNavigation() {
            const $form = $('#ProbationPeriodExtension-form');
            if (!$form.length) return;

            $form.on('keydown', function (e) {
                if (e.key === 'Enter' && !$(e.target).is('textarea')) {
                    e.preventDefault();

                    const $focusable = $form.find('input:visible, select:visible, textarea:visible, button:visible').not(':disabled');
                    const currentIndex = $focusable.index(e.target.closest('input, select, textarea, button'));
                    const $next = $focusable.eq((currentIndex + 1) % $focusable.length);

                    if ($next.data('select2')) {
                        $next.select2('open');
                    } else {
                        $next.focus();
                    }
                }
            });

            // Handle Select2 close event
            $form.on('select2:close', 'select', function () {
                const $focusable = $form.find('input:visible, select:visible, textarea:visible, button:visible').not(':disabled');
                const currentIndex = $focusable.index(this);
                const $next = $focusable.eq((currentIndex + 1) % $focusable.length);

                setTimeout(() => {
                    if ($next.data('select2')) {
                        $next.select2('open');
                    } else {
                        $next.focus();
                    }
                }, 50);
            });
        }

        // #endregion

        //#region validation
        function validation() {
            var emp = $('#EmployeeId').val();
            var duration = $('#Extended').val();
            var compName = $('#Period').val();

            if (!emp) {
                toastr.info('Select Employee');
                $('#EmployeeId').select2('open')
                return false;
            }
            if (!duration) {
                toastr.info('Enter Extended Period');
                $('#Extended').trigger('focus');
                return false;
            }
            if (!compName) {
                toastr.info('Select Period Name');
                $('#Period').select2('open')
                return false;
            }

            console.log(duration);
           
            return true;
        }

        //#endregion

        //#region ResetForm

        $(document).ready(function () {
            $('.js-ProbationPeriodExtension-clear').on('click', function () {
                ResetForm();
            });
        });
        
        function ResetForm() {

            // Clear dropdowns
            $('#EmployeeId').val('');
            $('#Period').val('').trigger('change');

            // Clear input fields
            $('input[name="Extended"]').val('');
            $('#ExtensionSalary').val('');
            $('#RefLetterNo').val('');
            $('#Remarks').val('');
            loadSNCId();
            // Clear Flatpickr-enabled date fields
            const wef = $('[name="Wef"]')[0];
            if (wef && wef._flatpickr) {
                wef._flatpickr.clear();
            } else {
                $('[name="Wef"]').val('');
            }

            $('#LdateModifyHide').hide();
            $('#sectionBreak').hide();

            const refDate = $('[name="RefLetterDate"]')[0];
            if (refDate && refDate._flatpickr) {
                refDate._flatpickr.clear();
            } else {
                $('[name="RefLetterDate"]').val('');
            }
            $("#ProbationPeriodExtension-check-all").prop("checked", false);
            $('#ProbationPeriodExtension-grid input[type="checkbox"]').prop('checked', false);
           
            // Clear span-based employee info
            $('#FullName').text('');
            $('#DepartmentName').text('');
            $('#DesignationName').text('');
            $('#GrossSalary').text('');
            $('#JoiningDate').text('');
            $('#ProbationPeriod').text('');
            $('#ContractEndDate').text('');
            $('#DurationSinceJoining').text('');
            $('#select2-EmployeeId-container').text('');
            selectMonth();

        }

        //#endregion

        //#region selectMonth
        function selectMonth() {
            var $dropdown = $('select[name="Period"]');

            $dropdown.find('option').each(function () {
                if ($(this).text().trim() === 'Month') {
                    $(this).prop('selected', true);
                    return false;
                }
            });

            $dropdown.trigger('change');
            $dropdown.selectpicker('refresh'); // If using Bootstrap Select
        }

        //#endregion

        //#region GenerateNewId
        function loadSNCId() {
            $.ajax({
                url: "/ProbationPeriodExtension/GenerateNewId",
                type: "GET",
                dataType: "json",
                success: function (data) {

                    if (data) {
                        $('#Ppeid').val(data);
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error fetching Mobile Bill  ID:", error);
                }
            })
        }

        //#endregion

    }

}(jQuery));
