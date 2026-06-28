(function ($) {
    $.patientTypes = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
        }, options);

        var filterUrl = commonName.baseUrl + "/GetLoanDetails";
        var GenerateExcelUrl = commonName.baseUrl + "/GenerateExcel";
        loadEmployeeLoanReport = function (companyId = null, laonTypeId = null, employeeId = null, loanId = null, dateFrom = null, dateTo = null) {
            $.ajax({
                url: filterUrl,
                type: "GET",
                data: {
                    CompanyID: companyId,
                    EmployeeID: employeeId,
                    LoanID: loanId,
                    DateFrom: dateFrom,
                    DateTo: dateTo,
                    LoanTypeId: laonTypeId,
                },
                success: function (data) {
                    DropdownAppendCompany(data.companies);
                    DropdownAppendLoanType(data.loanTypes);
                },
                error: function () {
                    alert("Error fetching data");
                }
            });
        }

        DropdownAppendCompany = function (dropdownDataCompany) {
            window.dropdownDataCompany = dropdownDataCompany;

            if (!dropdownDataCompany || !Array.isArray(dropdownDataCompany)) {
                return;
            }

            const $container = $('[data-field="companyId"]');
            const $optionContainer = $container.find(".multiselect-options");
            $optionContainer.empty();

            dropdownDataCompany.forEach((company, index) => {
                const $option = $('<div class="multiselect-option"></div>')
                    .text(company.companyName)
                    .attr("data-value", company.companyCode);

                $option.on("click", function () {
                    $container.find(".selected-items").text(company.companyName);
                    $container.find(".placeholder-text").hide();
                    $container.find(".multiselect-dropdown").hide();
                    $container.find(".multiselect-arrow").removeClass("rotate");
                    $container.find("#companyId").val(company.companyCode);

                    loadEmployeeLoanReport(company.companyCode, null, null, null, null);

                });

                $optionContainer.append($option);

                if (index === 0) {
                    setCompanySelection($container, company.companyName, company.companyCode);
                }
            });
        };

        function setCompanySelection($container, name, code) {
            $container.find(".selected-items")
                .text(name)
                .attr("data-id", code); 
            $container.find(".selected-items").text(name);
            $container.find(".placeholder-text").hide();
            $container.find(".multiselect-dropdown").hide();
            $container.find(".multiselect-arrow").removeClass("rotate");
            $container.find("#companyId").val(code);
            setTextClear();
            const $containerLoan = $('[data-field="loanTypeId"]'); 
            $containerLoan.find(".multiselect-options").empty(); 
            $containerLoan.find(".selected-items").text('');     
            $containerLoan.find(".placeholder-text").show();     

            $containerLoan.find("#loanTypeId").val('');
            setTextClear();
            const $containerEmployee = $('[data-field="employeeId"]');
            $containerEmployee.find(".multiselect-options").empty();
            $containerEmployee.find(".selected-items").text('');
            $containerEmployee.find(".placeholder-text").show();
            $containerEmployee.find("#employeeId").val(''); 

            setTextClear();
            const $containerLoanType = $('[data-field="loanType"]');
            $containerLoanType.find(".multiselect-options").empty();
            $containerLoanType.find(".selected-items").text('');
            $containerLoanType.find(".placeholder-text").show();

        }

        // Search company
        $('[data-field="companyId"] .multiselect-search input').on("input", function () {
            const searchTerm = $(this).val().toLowerCase().trim();
            const $container = $(this).closest('[data-field="companyId"]');
            const $optionContainer = $container.find(".multiselect-options");

            const allCompanies = window.dropdownDataCompany || [];

            $optionContainer.empty();

            const filtered = allCompanies.filter(company =>
                company.companyName.toLowerCase().includes(searchTerm)
            );

            if (filtered.length === 0) {
                $optionContainer.append('<div class="multiselect-option disabled">No data available</div>');
            } else {
                filtered.forEach(company => {
                    const $option = $('<div class="multiselect-option"></div>')
                        .text(company.companyName)
                        .attr("data-value", company.companyCode);

                    $option.on("click", function () {
                        $container.find(".selected-items").text(company.companyName);
                        $container.find(".placeholder-text").hide();
                        $container.find(".multiselect-dropdown").hide();
                        $container.find(".multiselect-arrow").removeClass("rotate");
                        $container.find("#companyId").val(company.companyCode);
                    });

                    $optionContainer.append($option);
                });
            }
        });

        // Dropdown toggle
        $('[data-field="companyId"] .multiselect-input').on("click", function () {
            const $container = $(this).closest('[data-field="companyId"]');
            const $dropdown = $container.find(".multiselect-dropdown");
            const $arrow = $container.find(".multiselect-arrow");
            $(".multiselect-dropdown").not($dropdown).hide();
            $(".multiselect-arrow").not($arrow).removeClass("rotate");
            $dropdown.toggle();
            $arrow.toggleClass("rotate", $dropdown.is(":visible"));
        });

        // Close on outside click
        $(document).on("click", function (e) {
            if (!$(e.target).closest('[data-field="companyId"]').length) {
                $('[data-field="companyId"] .multiselect-dropdown').hide();
                $('[data-field="companyId"] .multiselect-arrow').removeClass("rotate");
            }
        });

           
        //loan type id
        function setLoanIdSelection($container, value) {
            $container.find(".selected-items").text(value);
            $container.find(".placeholder-text").hide();
            $container.find(".multiselect-dropdown").hide();
            $container.find(".multiselect-arrow").removeClass("rotate");

            let companyId = $('[data-field="companyId"] .selected-items').attr("data-id");            
            const employeeId = $('[data-field="employeeId"] .selected-items').attr("data-value");
            let LoanTypeId = $('[data-field="loanType"] .selected-items').attr("data-id");    
            loadLoanDetailsByEmployeeIdAndCompanyIdReport(companyId, LoanTypeId, employeeId, value, null, null, null);

        }

        DropdownAppendLoanId = function (loanTypesArray) {
            window.dropdownLoanTypes = loanTypesArray;           
            const $container = $('[data-field="loanTypeId"]');
            const $optionContainer = $container.find(".multiselect-options");
            $optionContainer.empty();

            loanTypesArray.forEach((loanId, index) => {
                const $option = $('<div class="multiselect-option"></div>')
                    .text(loanId.loanIDs)
                    .attr("data-value", loanId.loanIDs);

                $option.on("click", function () {
                    setLoanIdSelection($container, loanId.loanIDs);
                });

                $optionContainer.append($option);

            });
        };

        // Dropdown toggle
        $('[data-field="loanTypeId"] .multiselect-input').on("click", function (e) {
            const $container = $(this).closest('[data-field="loanTypeId"]');
            const $dropdown = $container.find(".multiselect-dropdown");
            const $arrow = $container.find(".multiselect-arrow");

            $(".multiselect-dropdown").not($dropdown).hide();
            $(".multiselect-arrow").not($arrow).removeClass("rotate");

            $dropdown.toggle();
            $arrow.toggleClass("rotate", $dropdown.is(":visible"));
        });

        // Outside click
        $(document).on("click", function (e) {
            if (!$(e.target).closest('[data-field="loanTypeId"]').length) {
                $('[data-field="loanTypeId"] .multiselect-dropdown').hide();
                $('[data-field="loanTypeId"] .multiselect-arrow').removeClass("rotate");
            }
        });

        // Search functionality
        $('[data-field="loanTypeId"] .multiselect-search input').on("input", function () {
            const searchTerm = $(this).val().toLowerCase().trim();
            const $container = $(this).closest('[data-field="loanTypeId"]');
            const $optionContainer = $container.find(".multiselect-options");

            const allLoanTypes = window.dropdownLoanTypes || [];

            $optionContainer.empty();

            const filtered = allLoanTypes
                .filter(x => typeof x.loanIDs === "string" && x.loanIDs.toLowerCase().includes(searchTerm));

            if (filtered.length === 0) {
                $optionContainer.append('<div class="multiselect-option disabled">No data available</div>');
            } else {
                filtered.forEach(item => {
                    const $option = $('<div class="multiselect-option"></div>')
                        .text(item.loanIDs)
                        .attr("data-value", item.loanIDs);

                    $option.on("click", function () {
                        setLoanIdSelection($container, item.loanIDs);
                    });

                    $optionContainer.append($option);
                });
            }
        });

        //loan type
        function setLoanTypeSelection($container, loanTypeId, loanType) {
            $container.find(".selected-items")
                .text(loanType)
                .attr("data-id", loanTypeId);
            setTextClear();
            $container.find(".placeholder-text").hide();
            $container.find(".multiselect-dropdown").hide();
            $container.find(".multiselect-arrow").removeClass("rotate");
            let companyId = $('[data-field="companyId"] .selected-items').attr("data-id");


            setTextClear();
            const $containerLoan = $('[data-field="loanTypeId"]');
            $containerLoan.find(".multiselect-options").empty();
            $containerLoan.find(".selected-items").text('');
            $containerLoan.find(".placeholder-text").show();

            $containerLoan.find("#loanTypeId").val('');
            setTextClear();
            const $containerEmployee = $('[data-field="employeeId"]');
            $containerEmployee.find(".multiselect-options").empty();
            $containerEmployee.find(".selected-items").text('');
            $containerEmployee.find(".placeholder-text").show();
            $containerEmployee.find("#employeeId").val(''); 

            loadEmployeeByLoanTypeIdAndCompanyIdReport(companyId, loanTypeId, null, null, null, null);
        }

        DropdownAppendLoanType = function (loanTypesArray) {
            window.dropdownLoanTypes = loanTypesArray;
            const $container = $('[data-field="loanType"]');
            const $optionContainer = $container.find(".multiselect-options");
            $optionContainer.empty();

            loanTypesArray.forEach((loanTypeObj, index) => {
                const $option = $('<div class="multiselect-option"></div>')
                    .text(loanTypeObj.loanType)
                    .attr("data-value", loanTypeObj.loanTypeId);

                $option.on("click", function () {
                    setLoanTypeSelection($container, loanTypeObj.loanTypeId, loanTypeObj.loanType);
                });

                $optionContainer.append($option);
            });
        };

        // Dropdown toggle
        $('[data-field="loanType"] .multiselect-input').on("click", function (e) {
            const $container = $(this).closest('[data-field="loanType"]');
            const $dropdown = $container.find(".multiselect-dropdown");
            const $arrow = $container.find(".multiselect-arrow");

            $(".multiselect-dropdown").not($dropdown).hide();
            $(".multiselect-arrow").not($arrow).removeClass("rotate");

            $dropdown.toggle();
            $arrow.toggleClass("rotate", $dropdown.is(":visible"));
        });

        // Outside click
        $(document).on("click", function (e) {
            if (!$(e.target).closest('[data-field="loanType"]').length) {
                $('[data-field="loanType"] .multiselect-dropdown').hide();
                $('[data-field="loanType"] .multiselect-arrow').removeClass("rotate");
            }
        });
              

        $('[data-field="loanType"] .multiselect-search input').on("input", function () {
            const searchTerm = $(this).val().toLowerCase().trim();
            const $container = $(this).closest('[data-field="loanType"]');
            const $optionContainer = $container.find(".multiselect-options");

            const allLoanTypes = window.dropdownLoanTypes || [];

            // Clear existing options
            $optionContainer.empty();

            const filtered = allLoanTypes
                .filter(x => x && typeof x.loanType === "string")
                .filter(x => x.loanType.toLowerCase().includes(searchTerm));

            if (filtered.length === 0) {
                $optionContainer.append('<div class="multiselect-option disabled">No data available</div>');
            } else {
                filtered.forEach(item => {
                    const $option = $('<div class="multiselect-option"></div>')
                        .text(item.loanType)
                        .attr("data-value", item.loanTypeId);

                    $option.on("click", function () {
                        setLoanTypeSelection($container, item.loanTypeId, item.loanType);
                    });

                    $optionContainer.append($option);
                });
            }
        });



        loadEmployeeByLoanTypeIdAndCompanyIdReport = function (companyId = null, laonTypeId = null, employeeId = null, loanId = null, dateFrom = null, dateTo = null) {
            $.ajax({
                url: filterUrl,
                type: "GET",
                data: {
                    CompanyID: companyId,
                    EmployeeID: employeeId,
                    LoanTypeId: laonTypeId,
                    LoanID: loanId,
                    DateFrom: dateFrom,
                    DateTo: dateTo
                },
                success: function (data) {
                    DropdownAppendEmployee(data.employees);
                },
                error: function () {
                    alert("Error fetching data");
                }
            });
        }

        function setEmployeeSelection($container, employee) {
            const empText = `${employee.fullName} (${employee.employeeID})`; 
            const empId = employee.employeeID; 

            $container.find(".selected-items")
                .text(empText)
                .attr("data-value", empId); 

            $container.find(".placeholder-text").hide();
            $container.find(".multiselect-dropdown").hide();
            $container.find(".multiselect-arrow").removeClass("rotate");

            let companyId = $('[data-field="companyId"] .selected-items').attr("data-id") || '';
            let loanTypeId = $('[data-field="loanTypeId"] .selected-items').text() || '';
            const $containerLoan = $('[data-field="loanTypeId"]');
            $containerLoan.find(".multiselect-options").empty();
            $containerLoan.find(".selected-items").text('');
            $containerLoan.find(".placeholder-text").show();

            $containerLoan.find("#loanTypeId").val('');
            
            if (dateFromPicker) dateFromPicker.clear();
            if (toDatePicker) toDatePicker.clear();
            $("#showLoanDate").text("");
            $("#showloanTypeName").text("");
            $("#showloanAmount").text("");
            $("#showInstStartEndDate").text("");
            $("#showNoOfInstallment").text("");


            let LoanTypeId = $('[data-field="loanType"] .selected-items').attr("data-id");    
            // Call the report loader
            loadEmployeeDetailsByEmployeeIdLoanIdAndCompanyIdReport(companyId, LoanTypeId, empId, null, null, null);
        }


        DropdownAppendEmployee = function (employeeList) {
            window.dropdownEmployees = employeeList;
            const $container = $('[data-field="employeeId"]');
            const $optionContainer = $container.find(".multiselect-options");
            $optionContainer.empty();

            employeeList.forEach((emp, index) => {
                const displayText = `${emp.fullName} (${emp.employeeID})`;

                const $option = $('<div class="multiselect-option"></div>')
                    .text(displayText)
                    .attr("data-value", emp.employeeID);

                $option.on("click", function () {
                    setEmployeeSelection($container, emp);
                });

                $optionContainer.append($option);
            });
        };


        // Dropdown toggle
        $('[data-field="employeeId"] .multiselect-input').on("click", function (e) {
            const $container = $(this).closest('[data-field="employeeId"]');
            const $dropdown = $container.find(".multiselect-dropdown");
            const $arrow = $container.find(".multiselect-arrow");

            $(".multiselect-dropdown").not($dropdown).hide();
            $(".multiselect-arrow").not($arrow).removeClass("rotate");

            $dropdown.toggle();
            $arrow.toggleClass("rotate", $dropdown.is(":visible"));
        });

        // Click outside to close
        $(document).on("click", function (e) {
            if (!$(e.target).closest('[data-field="employeeId"]').length) {
                $('[data-field="employeeId"] .multiselect-dropdown').hide();
                $('[data-field="employeeId"] .multiselect-arrow').removeClass("rotate");
            }
        });

        // Search functionality
        $('[data-field="employeeId"] .multiselect-search input').on("input", function () {
            const searchTerm = $(this).val().toLowerCase().trim();
            const $container = $(this).closest('[data-field="employeeId"]');
            const $optionContainer = $container.find(".multiselect-options");
            const allEmployees = window.dropdownEmployees || [];
            $optionContainer.empty();
            const filtered = allEmployees.filter(emp =>
                emp.fullName.toLowerCase().includes(searchTerm) ||
                emp.employeeID.toLowerCase().includes(searchTerm)
            );

            if (filtered.length === 0) {
                $optionContainer.append('<div class="multiselect-option disabled">No data available</div>');
            } else {
                filtered.forEach(emp => {
                    const displayText = `${emp.fullName} (${emp.employeeID})`;

                    const $option = $('<div class="multiselect-option"></div>')
                        .text(displayText)
                        .attr("data-value", emp.employeeID);

                    $option.on("click", function () {
                        setEmployeeSelection($container, emp);
                    });

                    $optionContainer.append($option);
                });
            }
        });

        setTextClear = function () {
            $("#showEmployeeName").text('');
            $("#showDepartmentName").text('');
            $("#showDesignationName").text(''); 
            if (dateFromPicker) dateFromPicker.clear();
            if (toDatePicker) toDatePicker.clear();
            $("#showLoanDate").text("");
            $("#showloanTypeName").text("");
            $("#showloanAmount").text("");
            $("#showInstStartEndDate").text("");
            $("#showNoOfInstallment").text("");
        }

        loadEmployeeDetailsByEmployeeIdLoanIdAndCompanyIdReport = function (companyId = null, laonTypeId = null, employeeId = null, loanId = null, dateFrom = null, dateTo = null) {
            $.ajax({
                url: filterUrl,
                type: "GET",
                data: {
                    CompanyID: companyId,
                    EmployeeID: employeeId,
                    LoanTypeId: laonTypeId,
                    LoanID: loanId,
                    DateFrom: dateFrom,
                    DateTo: dateTo
                },
                success: function (data) {
                    var empData = data.employees;
                    if (!empData || empData.length > 0) {
                        $("#showEmployeeName").text(empData[0].fullName ?? "");
                        $("#showDepartmentName").text(empData[0].departmentName ?? "");
                        $("#showDesignationName").text(empData[0].designationName ?? "");
                        DropdownAppendLoanId(data.loanIDs);
                    }                  
                },
                error: function () {
                    alert("Error fetching data");
                }
            });
        }      
      
        let startDateStr = null;
        let endDateStr = null;

        let dateFromPicker = null;
        let toDatePicker = null;

        function loadLoanDetailsByEmployeeIdAndCompanyIdReport(companyId = null,laonTypeId = null, employeeId = null, loanId = null, dateFrom = null, dateTo = null) {
            $.ajax({
                url: filterUrl,
                type: "GET",
                data: {
                    CompanyID: companyId,
                    LoanTypeId: laonTypeId,
                    EmployeeID: employeeId,
                    LoanID: loanId,
                    DateFrom: dateFrom,
                    DateTo: dateTo
                },
                success: function (data) {
                    var loanData = data.loanIDs;                    
                    if (loanData && loanData.length > 0) {
                        $("#showLoanDate").text(loanData[0].loanDate ?? "");
                        $("#showloanTypeName").text(loanData[0].loanType ?? "");
                        $("#showloanAmount").text(loanData[0].loanAmount ?? "");
                        $("#showInstStartEndDate").text(loanData[0].instStartEndDate ?? "");
                        $("#showNoOfInstallment").text(loanData[0].noOfInstallment ?? "");

                        let [startDate, endDate] = loanData[0].instStartEndDate.split(" - ");
                        startDateStr = startDate;
                        endDateStr = endDate;

                        // Destroy old pickers if they exist
                        if (dateFromPicker) dateFromPicker.clear();
                        if (toDatePicker) toDatePicker.clear();

                        // Re-initialize flatpickr with new dates
                        //dateFromPicker = flatpickr("#dateFrom", {
                        //    altInput: true,
                        //    altFormat: "d/m/Y",
                        //    dateFormat: "Y-m-d",
                        //    allowInput: true,
                        //    minDate: convertToYMD(startDateStr),
                        //    clickOpens: false
                        //});
                        dateFromPicker = flatpickr("#dateFrom", CalendarService.createConfig(
                            {
                                defaultDate: new Date(),
                                minDate: convertToYMD(startDateStr),
                                clickOpens: false
                            }
                        ));
                        dateFromPicker = flatpickr("#toDate", CalendarService.createConfig(
                            {
                                defaultDate: new Date(),
                                maxDate: convertToYMD(endDateStr),
                                clickOpens: false
                            }
                        ));
                        //toDatePicker = flatpickr("#toDate", {
                        //    altInput: true,
                        //    altFormat: "d/m/Y",
                        //    dateFormat: "Y-m-d",
                        //    allowInput: true,
                        //    maxDate: convertToYMD(endDateStr),
                        //    clickOpens: false
                        //});
                    }
                },
                error: function () {
                    alert("Error fetching data");
                }
            });
        }

        // Helper function to convert dd/mm/yyyy → yyyy-mm-dd
        function convertToYMD(dateStr) {
            if (!dateStr) return null;
            const [day, month, year] = dateStr.split('/');
            return `${year}-${month}-${day}`;
        }
        let isDownloadPdf = false;
        let isDownloadExcel = false;
        let isDownloadWord = false;
        let isPreviewPdf = false;
        $(document).on('click', "#btnPreview", function () {
            isPreviewPdf = true;
            let companyId = $('[data-field="companyId"] .selected-items').attr("data-id");
            const loanId = $('[data-field="loanTypeId"] .selected-items').text();
            const employeeId = $('[data-field="employeeId"] .selected-items').attr("data-value");

            let dateFrom = $("#dateFrom").val();
            let toDate = $("#toDate").val();
            const loanTypeId = $('[data-field="loanType"] .selected-items').attr("data-id");  
            loadEmployeeDetailsDocumentReport(companyId, loanTypeId, employeeId, loanId, dateFrom, toDate);
        })
        $(document).on('click', "#downloadReport", function () {
            let companyId = $('[data-field="companyId"] .selected-items').attr("data-id");
            const loanId = $('[data-field="loanTypeId"] .selected-items').text();
            const employeeId = $('[data-field="employeeId"] .selected-items').attr("data-value");
            const loanTypeId = $('[data-field="loanType"] .selected-items').attr("data-id");
            let dateFrom = $("#dateFrom").val();
            let toDate = $("#toDate").val();

            var downloadText = $("#reportText").val();
            if (downloadText === "downloadPdf") {
                isDownloadPdf = true;
                isDownloadExcel = false;
                isDownloadWord = false;
                isPreviewPdf = false;
            } else if (downloadText === "downloadExcel") {
                isDownloadPdf = false;
                isDownloadExcel = true;
                isDownloadWord = false;
                isPreviewPdf = false;
            } else if (downloadText === "downloadWord") {
                isDownloadPdf = false;
                isDownloadExcel = false;
                isDownloadWord = true;
                isPreviewPdf = false;
            } else {
                isDownloadPdf = true;
                isDownloadExcel = false;
                isDownloadWord = false;
                isPreviewPdf = false;
            }

            loadEmployeeDetailsDocumentReport(companyId, loanTypeId, employeeId, loanId, dateFrom, toDate);
        });
        loadEmployeeDetailsDocumentReport = function (companyId = null, laonTypeId = null, employeeId = null, loanId = null, dateFrom = null, dateTo = null) {
            $.ajax({
                url: filterUrl,
                type: "GET",
                data: {
                    CompanyID: companyId,
                    LoanTypeId: laonTypeId,
                    EmployeeID: employeeId,
                    LoanID: loanId,
                    DateFrom: dateFrom,
                    DateTo: dateTo
                },
                success: function (data) {

                    if (isDownloadPdf) {
                        GeneratePdf(data.loanReports);
                        isDownloadPdf = false;
                        isDownloadExcel = false;
                        isDownloadWord = false;
                        isPreviewPdf = false;
                    } else if (isDownloadExcel) {
                        GenerateExcel(data.loanReports);
                        isDownloadPdf = false;
                        isDownloadExcel = false;
                        isDownloadWord = false;
                        isPreviewPdf = false;
                    } else if (isDownloadWord) {
                        GenerateWordDocument(data.loanReports);
                        isDownloadPdf = false;
                        isDownloadExcel = false;
                        isDownloadWord = false;
                        isPreviewPdf = false;
                    } else {
                        GeneratePdf(data.loanReports);
                        isDownloadPdf = false;
                        isDownloadExcel = false;
                        isDownloadWord = false;
                        isPreviewPdf = false;
                    }                   
                },
                error: function () {
                    alert("Error fetching data");
                }
            });
        }

        // PDF Download button click handler
        
        FormateAmountEN = function (amount) {
            if (amount >= 10000000) {
                return (amount / 10000000).toFixed(1).replace(/\.0$/, '') + 'Cr';
            } else if (amount >= 100000) {
                return (amount / 100000).toFixed(1).replace(/\.0$/, '') + 'Lakh';
            } else if (amount >= 1000) {
                return (amount / 1000).toFixed(1).replace(/\.0$/, '') + 'K';
            } else {
                return amount + ' TK';
            }
        }

        async function loadImageBase64(url) {
            return new Promise((resolve) => {
                const img = new Image();
                img.crossOrigin = 'Anonymous';
                img.onload = function () {
                    const canvas = document.createElement('canvas');
                    canvas.width = img.naturalWidth;
                    canvas.height = img.naturalHeight;
                    canvas.getContext('2d').drawImage(img, 0, 0);
                    resolve({
                        base64: canvas.toDataURL('image/png'),
                        natW: img.naturalWidth,
                        natH: img.naturalHeight
                    });
                };
                img.onerror = function () {
                    resolve({ base64: null, natW: 0, natH: 0 });
                };
                img.src = url;
            });
        }


        GeneratePdf = async function (installmentData) {
            const { jsPDF } = window.jspdf;
            const doc = new jsPDF('p', 'pt', 'a4');
            const pageWidth = doc.internal.pageSize.getWidth();
            const marginLeft = 40;
            let y = 35;

            if (!installmentData || installmentData.length === 0) {
                alert("No data available for PDF");
                return;
            }

            // ★ Logo load — aspect ratio auto
            const LOGO_TARGET_HEIGHT = 40;  // ← এই height বাড়ান/কমান
            let logoBase64 = null;
            let logoWidth = LOGO_TARGET_HEIGHT;
            try {
                const logoData = await loadImageBase64('/images/DP_logo.png');
                logoBase64 = logoData.base64;
                if (logoData.natH > 0) {
                    logoWidth = (logoData.natW / logoData.natH) * LOGO_TARGET_HEIGHT;
                }
            } catch (e) { }
     

            function drawSectionTitle(titleText) {
                doc.setFontSize(14);
                doc.setFont(undefined, 'normal');
                let paddingY = 4;
                let rowHeight = 20;
                let fullWidth = pageWidth - marginLeft * 2;
                let textWidth = doc.getTextWidth(titleText);
                let textX = (pageWidth - textWidth) / 2;
                let rectY = y - rowHeight + paddingY;
                doc.setLineWidth(0.5);
                doc.rect(marginLeft, rectY, fullWidth, rowHeight);
                doc.text(titleText, textX, y);
                y += rowHeight + 15;
            }

            installmentData.forEach(function (loan, index) {
                if (index > 0) {
                    doc.addPage();
                    y = 35;
                }
                // Logo — left aligned,  y update
                if (logoBase64) {
                    doc.addImage(logoBase64, 'PNG', marginLeft, y, logoWidth, LOGO_TARGET_HEIGHT);
                }

                // Company name — logo 
                doc.setFontSize(18);
                doc.setFont(undefined, 'bold');
                let companyText = `${loan.companyName}`;
                let textWidth = doc.getTextWidth(companyText);

                // Logo height  center এ text align 
                let textY = y + (LOGO_TARGET_HEIGHT / 2) + 5; // vertically center with logo
                doc.text(companyText, (pageWidth - textWidth) / 2, textY);

                y += LOGO_TARGET_HEIGHT + 10; 


               

                // Installment Details Text
                let installmentDetailsText = `Installments of ${FormateAmountEN(loan.monthlyDeduction)} tk per month to be deposited to designated account.`;

                drawSectionTitle("Employee Loan Information");

                doc.setFontSize(11);
                const centerX = pageWidth / 2.7;
                const colonX = centerX;
                const labelX = colonX - 2;
                const valueX = colonX + 5;

                // Updated drawKeyValue Function                

                function drawKeyValue(label, value, yPos) {
                    doc.setFont(undefined, 'normal');

                    if (label === "Installment Details") {
                        const paddingRight = 40;
                        const maxWidth = pageWidth - colonX - paddingRight;  // Available width from colon to right margin

                        const wrappedText = doc.splitTextToSize(value, maxWidth);

                        // Draw label and colon
                        doc.text(label, labelX, yPos, { align: "right" });
                        doc.text(":", colonX, yPos, { align: "center" });

                        // Draw wrapped value aligned to left
                     
                        wrappedText.forEach((line, i) => {
                            const lineX = colonX + 5;           
                            const lineY = yPos + (i * 12);     
                            doc.text(line, lineX, lineY, { align: "left" });
                        });

                        y += wrappedText.length * 10; // Update y position
                    } else {
                        doc.text(label, labelX, yPos, { align: "right" });
                        doc.text(":", colonX, yPos, { align: "center" });
                        doc.text(value, valueX, yPos, { align: "left" });
                        //y += 18;
                    }
                }


                drawKeyValue("Employee ID", loan.employeeID.toString(), y -= 10); y += 18;
                drawKeyValue("Name", loan.fullName, y); y += 18;
                drawKeyValue("Department", loan.departmentName, y); y += 18;
                drawKeyValue("Designation", loan.designationName, y); y += 18;
                drawKeyValue("Reason of Loan taken", loan.reason || "", y); y += 18;
                drawKeyValue("Loan Number", loan.totalLoans.toString(), y); y += 18;
                drawKeyValue("Loan Amount", `BDT ${loan.loanAmount.toFixed(2)}`, y); y += 18;
                drawKeyValue("Loan Disbursed Date", new Date(loan.startDate).toLocaleDateString(), y); y += 18;
                drawKeyValue("Loan Repayment Method", loan.loanRepaymentMethod.toString(), y); y += 18;
                drawKeyValue("Installment Details", installmentDetailsText, y); y += 8;
                drawKeyValue("Loan Paidout Date", new Date(loan.endDate).toLocaleDateString(), y); y += 30;
                drawKeyValue("Remarks", loan.remarks || "", y); y += 25;

                drawSectionTitle("Loan Installment Details");

                let totalDeposit = 0;
                let installmentRows = loan.installments.map(inst => {
                    totalDeposit += inst.deposit || 0;
                    return [
                        inst.installmentNo,
                        inst.installmentDate || "",
                        inst.paymentMode || "",
                        inst.deposit ? `BDT ${inst.deposit.toFixed(2)}` : "",
                        `BDT ${inst.outstandingBalance.toFixed(2)}`
                    ];
                });
                y -= 10;

                doc.autoTable({
                    startY: y,
                    head: [['Installment No', 'Transaction Date', 'Installment Details (Cash/Cheque)', 'Deposit Amount', 'Outstanding Balance']],
                    body: installmentRows,
                    margin: { left: marginLeft, right: marginLeft },
                    styles: {
                        fontSize: 10,
                        lineColor: [0, 0, 0],
                        lineWidth: 0.2,
                        textColor: [0, 0, 0],
                        halign: 'center',
                        valign: 'middle'
                    },
                    headStyles: {
                        fillColor: [255, 255, 255],
                        textColor: [0, 0, 0],
                        lineColor: [0, 0, 0],
                        lineWidth: 0.2,
                        halign: 'center',
                        valign: 'middle'
                    },
                    theme: 'grid',
                    columnStyles: {
                        0: { cellWidth: 80 },
                        1: { cellWidth: 100 },
                        2: { cellWidth: 110 },
                        3: { cellWidth: 115, halign: 'right' },
                        4: { cellWidth: 'auto', halign: 'right' }
                    },
                    didDrawPage: (data) => {
                        y = data.cursor.y + 10;
                    }
                });

                // Total Deposit Summary
                let depositColumnIndex = 3;
                let cellWidth = (pageWidth - marginLeft * 2) / 5;
                let depositColumnX = marginLeft + depositColumnIndex * cellWidth;

                let valueText = `BDT ${totalDeposit.toFixed(2)}`;
                let textY = y + 5;
                doc.setFontSize(11);
                doc.setFont(undefined, 'bold');

                doc.setLineWidth(0.5);
                doc.line(
                    depositColumnX + (cellWidth - doc.getTextWidth(valueText)) / 2,
                    textY - 6,
                    depositColumnX + (cellWidth + doc.getTextWidth(valueText)) / 2,
                    textY - 6
                );

                textY += 5;
                doc.text(valueText, depositColumnX + cellWidth / 2, textY, { align: 'center' });

                let textWidthValue = doc.getTextWidth(valueText);
                let lineY1 = textY + 2;
                doc.line(
                    depositColumnX + (cellWidth - textWidthValue) / 2,
                    lineY1,
                    depositColumnX + (cellWidth + textWidthValue) / 2,
                    lineY1
                );

                let lineY2 = textY + 4;
                doc.line(
                    depositColumnX + (cellWidth - textWidthValue) / 2,
                    lineY2,
                    depositColumnX + (cellWidth + textWidthValue) / 2,
                    lineY2
                );

                y = textY + 15;
            });

            if (isPreviewPdf) {
                const pdfBlob = doc.output('blob');
                const pdfUrl = URL.createObjectURL(pdfBlob);
                const container = document.getElementById("pdfContainer");
                container.innerHTML = "";
                const embed = document.createElement("embed");
                embed.src = pdfUrl;
                embed.type = "application/pdf";
                embed.width = "100%";
                embed.height = "100%";
                container.appendChild(embed);
            } else if (isDownloadPdf) {
                doc.save("Loan_Report.pdf");
            }
        }

        //word 
        GenerateWordDocument = function (installmentData) {
            try {
                if (!installmentData || installmentData.length === 0) {
                    alert("No data available for Word document");
                    return;
                }

                const now = new Date().toLocaleString('en-US', {
                    year: 'numeric', month: 'short', day: 'numeric',
                    hour: 'numeric', minute: 'numeric', hour12: true
                });

                let htmlContent = `
<!DOCTYPE html>
<html xmlns:v="urn:schemas-microsoft-com:vml" 
      xmlns:o="urn:schemas-microsoft-com:office:office" 
      xmlns:w="urn:schemas-microsoft-com:office:word" 
      xmlns:m="http://schemas.microsoft.com/office/2004/12/omml" 
      xmlns="http://www.w3.org/TR/REC-html40">
<head>
    <meta charset="utf-8">
    <title>Loan Report</title>
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
        @page Section1 { 
            size: 595.3pt 841.9pt; 
            mso-page-orientation: portrait; 
            margin: 0.5in; 
        }
        div.Section1 { page: Section1; }
        body { 
            font-family: 'Times New Roman', serif; 
            margin: 0; 
            padding: 20px; 
            font-size: 12pt;
        }
        .page-break { 
            page-break-before: always; 
        }
        .company-header { 
            text-align: center; 
            font-size: 18pt; 
            font-weight: bold; 
            margin-bottom: 1pt; 
        }
        .report-title { 
            text-align: center; 
            font-size: 14pt; 
            margin-bottom: 2pt;
            border:2px solid #000;
        }
        .employee-info { 
            margin-bottom: 2pt; 
        }
        .info-table { 
            width: 100%; 
            border-collapse: collapse; 
            margin-bottom: 2pt; 
        }
        .info-table td { 
            padding: 2pt 5pt; 
            border: none; 
            font-size: 11pt; 
            vertical-align: top; 
        }
        .info-label { 
            text-align: right; 
            width: 48%; 
            padding-right: -5pt; 
        }
        .info-colon { 
            text-align: center; 
            width: 1%; 
        }
        .info-value { 
            text-align: left; 
            width: 50%; 
            padding-left: 2pt; 
        }
        .installment-table { 
            width: 100%; 
            border-collapse: collapse; 
            margin-bottom: 15pt; 
        }
        .installment-table, 
        .installment-table th, 
        .installment-table td { 
            border: 1px solid black; 
            padding: 2pt 1pt; 
            font-size: 10pt; 
            text-align: center; 
            vertical-align: middle; 
        }
        .installment-table th { 
            background-color: #f0f0f0; 
            font-weight: bold; 
        }
        .installment-table .amount-col { 
            text-align: right; 
        }
        .total-section { 
            margin-top: 10pt; 
            text-align: center;           
        }
        .total-amount { 
            font-weight: bold; 
            font-size: 11pt; 
            border-top: 1px solid black; 
            border-bottom: 2px double black; 
            padding: 3pt 10pt; 
            display: inline-block; 
            margin-top: 5pt;
             margin-left: 282px;
        }

       
        .footer-info { 
            margin-top: 20pt; 
            font-size: 10pt; 
        }
    </style>
</head>
<body>
<div class="Section1">
`;

                installmentData.forEach(function (loan, index) {
                    if (index > 0) {
                        htmlContent += `<div class="page-break"></div>`;
                    }

                    // Company header
                    htmlContent += `
    <div class="company-header">${loan.companyName || 'Company Name'}</div>
    <div class="report-title">Employee Loan Information</div>
    
    <div class="employee-info">
        <table class="info-table">
            <tr>
                <td class="info-label">Employee ID</td>
                <td class="info-colon">:</td>
                <td class="info-value">${loan.employeeID || ''}</td>
            </tr>
            <tr>
                <td class="info-label">Name</td>
                <td class="info-colon">:</td>
                <td class="info-value">${loan.fullName || ''}</td>
            </tr>
            <tr>
                <td class="info-label">Department</td>
                <td class="info-colon">:</td>
                <td class="info-value">${loan.departmentName || ''}</td>
            </tr>
            <tr>
                <td class="info-label">Designation</td>
                <td class="info-colon">:</td>
                <td class="info-value">${loan.designationName || ''}</td>
            </tr>
            <tr>
                <td class="info-label">Reason of Loan taken</td>
                <td class="info-colon">:</td>
                <td class="info-value">${loan.reason || ''}</td>
            </tr>
            <tr>
                <td class="info-label">Loan Number</td>
                <td class="info-colon">:</td>
                <td class="info-value">${loan.totalLoans || ''}</td>
            </tr>
            <tr>
                <td class="info-label">Loan Amount</td>
                <td class="info-colon">:</td>
                <td class="info-value">BDT ${(loan.loanAmount || 0).toFixed(2)}</td>
            </tr>
            <tr>
                <td class="info-label">Loan Disbursed Date</td>
                <td class="info-colon">:</td>
                <td class="info-value">${loan.startDate ? new Date(loan.startDate).toLocaleDateString() : ''}</td>
            </tr>
            <tr>
                <td class="info-label">Loan Repayment Method</td>
                <td class="info-colon">:</td>
                <td class="info-value">${loan.loanRepaymentMethod || ''}</td>
            </tr>
            <tr>
                <td class="info-label">Installment Details</td>
                <td class="info-colon">:</td>
                <td class="info-value">${loan.installmentDetails || ''}</td>
            </tr>
            <tr>
                <td class="info-label">Loan Paidout Date</td>
                <td class="info-colon">:</td>
                <td class="info-value">${loan.endDate ? new Date(loan.endDate).toLocaleDateString() : ''}</td>
            </tr>
            <tr>
                <td class="info-label">Remarks</td>
                <td class="info-colon">:</td>
                <td class="info-value">${loan.remarks || ''}</td>
            </tr>
        </table>
    </div>
    <div class="report-title">Loan Installment Details</div>
`;
                    

                    // Installments table
                    let totalDeposit = 0;

                    htmlContent += `
    <table class="installment-table">
        <thead>
            <tr>
                <th style="width: 15%;">Installment No</th>
                <th style="width: 20%;">Transaction Date</th>
                <th style="width: 25%;">Installment Details<br>(Cash/Cheque)</th>
                <th style="width: 20%;">Deposit Amount</th>
                <th style="width: 20%;">Outstanding Balance</th>
            </tr>
        </thead>
        <tbody>
`;

                    if (loan.installments && loan.installments.length > 0) {
                        loan.installments.forEach(function (inst) {
                            const deposit = inst.deposit || 0;
                            totalDeposit += deposit;

                            htmlContent += `
            <tr>
                <td>${inst.installmentNo || ''}</td>
                <td>${inst.installmentDate || ''}</td>
                <td>${inst.paymentMode || ''}</td>
                <td class="amount-col">${deposit ? `BDT ${deposit.toFixed(2)}` : ''}</td>
                <td class="amount-col">BDT ${(inst.outstandingBalance || 0).toFixed(2)}</td>
            </tr>
`;
                        });
                    }

                    htmlContent += `
        </tbody>
    </table>
    
    <div class="total-section">
        <div class="total-amount">BDT ${totalDeposit.toFixed(2)}</div>
    </div>
    </div>
</body>
</html>
`;
                });

                // Create and download the Word document
                const blob = new Blob([htmlContent], { type: 'application/msword' });
                const url = URL.createObjectURL(blob);
                const link = document.createElement("a");
                link.href = url;
                link.download = "Loan_Report.doc";
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                URL.revokeObjectURL(url);

            } catch (error) {
                alert("Error generating Word document: " + error.message);                
            }
        }

        // Fixed JavaScript Function
        function GenerateExcel(installmentData) {
            if (!installmentData || installmentData.length === 0) {
                alert("No data available for Excel");
                return;
            }

            // Show loading indicator
            var loadingElement = document.getElementById('loadingIndicator');
            if (loadingElement) {
                loadingElement.style.display = 'block';
            }

            $.ajax({
                url: GenerateExcelUrl,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(installmentData),
                xhrFields: {
                    responseType: 'blob'
                },
                success: function (data, status, xhr) {
                    // Create blob and download
                    var blob = new Blob([data], {
                        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                    });

                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement('a');
                    link.href = url;
                    link.download = 'Loan_Report.xlsx';
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                    window.URL.revokeObjectURL(url);

                    // Hide loading indicator
                    if (loadingElement) {
                        loadingElement.style.display = 'none';
                    }
                },
                error: function (xhr, status, error) {
                    alert('Error generating Excel file: ' + (xhr.responseText || error));

                    // Hide loading indicator
                    if (loadingElement) {
                        loadingElement.style.display = 'none';
                    }
                }
            });
        }

        init = function () {
                loadEmployeeLoanReport();             
        }
        init();
    }
})(jQuery);
