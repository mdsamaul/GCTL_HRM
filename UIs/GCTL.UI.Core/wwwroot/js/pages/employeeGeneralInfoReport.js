
(function ($) {
    $.employeeGeneralInfoReport = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#EmployeeGeneralInfoReport-form",
            formContainer: ".js-EmployeeGeneralInfoReport-form-container",
            gridSelector: "#EmployeeGeneralInfoReport-grid",
            gridContainer: ".js-EmployeeGeneralInfoReport-grid-container",
            editSelector: ".js-EmployeeGeneralInfoReport-edit",
            saveSelector: ".js-EmployeeGeneralInfoReport-save",
            selectAllSelector: "#EmployeeGeneralInfoReport-check-all",
            deleteSelector: ".js-EmployeeGeneralInfoReport-delete-confirm",
            deleteModal: "#EmployeeGeneralInfoReport-delete-modal",
            finalDeleteSelector: ".js-EmployeeGeneralInfoReport-delete",
            clearSelector: "#resetButton",
            topSelector: ".js-go",
            decimalSelector: ".js-EmployeeGeneralInfoReport-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-EmployeeGeneralInfoReport-check-availability",
            haseFile: false,

            load: function () {

            }
        }, options);




        $(() => {

          


            $(document).ready(function () {
                //exportReportBtn
                $(document).on('click', "#resetButton", function () {
                    ResetForm();
                });


            });

            function ResetForm() {
                $("#EmployeeGeneralInfoReport-form")[0].reset();
                $('.multiselectCustom').val(null).trigger('change');

                if ($('.multiselectCustom').hasClass("select2-hidden-accessible")) {
                    $('.multiselectCustom').select2("val", "");
                }

                $('.selectpickerReport').val("").trigger('change');
                $('#exportFormatDropdown').val(null).trigger('change');
            }


            //
            $(document).on('change', '#CompanyCode', function () {

                var selectedCompanies = $(this).val();
                if (!selectedCompanies) {
                    return;
                }

                var stringIds = selectedCompanies.join(',');

                $.ajax({
                    type: 'GET',
                    url: '/EmployeeGeneralInfoReport/GetBranchByCompany',
                    data: { companyId: stringIds },
                    success: function (data) {
                        var branchDD = $('#BranchCode');
                        branchDD.empty();

                        if (data && data.length > 0) {

                            $.each(data, function (index, branch) {
                                branchDD.append('<option value="' + branch.branchCode + '">' + branch.branchName + '</option>');
                            });

                            branchDD.selectpicker('refresh');
                        } else {
                            branchDD.append('<option>No branch available</option>');
                        }
                        branchDD.selectpicker('refresh');
                    },
                    error: function () {
                        console.error('Failed to fetch branch');
                        toastr.info('Failed to fetch branch');
                    }
                });
                getDepartmentByCompanyId(selectedCompanies);
                getDesignationByCompanyId(selectedCompanies);
                getEmployeeByCompanyId(selectedCompanies);
            });


            function getDepartmentByCompanyId(selectedCompanies) {

                var stringIds = selectedCompanies.join(',');
                $.ajax({
                    type: 'GET',
                    url: '/EmployeeGeneralInfoReport/GetDepartmentByCompany',
                    data: { companyId: stringIds },
                    success: function (data) {
                        var departmentDD = $('#DepartmentName');
                        departmentDD.empty();
                        console.log(data);
                        if (data && data.length > 0) {

                            $.each(data, function (index, department) {
                                departmentDD.append('<option value="' + department.departmentCode + '">' + department.departmentName + '</option>');
                            });

                            departmentDD.selectpicker('refresh');
                        } else {
                            departmentDD.append('<option>No department available</option>');
                        }
                        departmentDD.selectpicker('refresh');
                    },
                    error: function () {
                        console.error('Failed to fetch department');
                        toastr.info('Failed to fetch department');
                    }
                });
            }

            function getDesignationByCompanyId(selectedCompanies) {
                var stringIds = selectedCompanies.join(',');
                $.ajax({
                    type: 'GET',
                    url: '/EmployeeGeneralInfoReport/GetDesignationByCompany',
                    data: { companyId: stringIds },
                    success: function (data) {
                        var designationDD = $('#DesignationCode');
                        designationDD.empty();

                        if (data && data.length > 0) {

                            $.each(data, function (index, designation) {
                                designationDD.append('<option value="' + designation.designationCode + '">' + designation.designationName + '</option>');
                            });

                            designationDD.selectpicker('refresh');
                        } else {
                            designationDD.append('<option>No designation available</option>');
                        }
                        designationDD.selectpicker('refresh');
                    },
                    error: function () {
                        console.error('Failed to fetch designation');
                        toastr.info('Failed to fetch designation');
                    }
                });
            }

            function getEmployeeByCompanyId(selectedCompanies) {
                var stringIds = selectedCompanies.join(',');
                $.ajax({
                    type: 'GET',
                    url: '/EmployeeGeneralInfoReport/GetEmployeeByCompany',
                    data: { companyId: stringIds },
                    success: function (data) {
                        var employeeDD = $('#EmployeeCode');
                        employeeDD.empty();

                        if (data && data.length > 0) {

                            $.each(data, function (index, employee) {
                                employeeDD.append('<option value="' + employee.employeeId + '">' + employee.employeeName + '</option>');
                            });

                            employeeDD.selectpicker('refresh');
                        } else {
                            employeeDD.append('<option>No employee available</option>');
                        }
                        employeeDD.selectpicker('refresh');
                    },
                    error: function () {
                        console.error('Failed to fetch employee');
                        toastr.info('Failed to fetch employee');
                    }
                });
            }


            $(document).on('change', '#BranchCode', function () {
                var selectedBranches = $(this).val();
                if (!selectedBranches) {
                    return;
                }

                var stringIds = selectedBranches.join(',');

                $.ajax({
                    type: 'GET',
                    url: '/EmployeeGeneralInfoReport/GetDepartmentByBranch',
                    data: { branchId: stringIds },
                    success: function (data) {
                        var departmentDD = $('#DepartmentName');
                        departmentDD.empty();
                        console.log(data);
                        if (data && data.length > 0) {

                            $.each(data, function (index, department) {
                                departmentDD.append('<option value="' + department.departmentCode + '">' + department.departmentName + '</option>');
                            });

                            departmentDD.selectpicker('refresh');
                        } else {
                            departmentDD.append('<option>No department available</option>');
                        }
                        departmentDD.selectpicker('refresh');
                    },
                    error: function () {
                        console.error('Failed to fetch department');
                        toastr.info('Failed to fetch department');
                    }
                });
                getDesignationByBranch(selectedBranches);
                getEmployeeByBranch(selectedBranches);
            });


            function getDesignationByBranch(selectedBranches) {
                var stringIds = selectedBranches.join(',');

                $.ajax({
                    type: 'GET',
                    url: '/EmployeeGeneralInfoReport/GetDesignationByBranch',
                    data: { branchId: stringIds },
                    success: function (data) {
                        var designationDD = $('#DesignationCode');
                        designationDD.empty();

                        if (data && data.length > 0) {

                            $.each(data, function (index, designation) {
                                designationDD.append('<option value="' + designation.designationCode + '">' + designation.designationName + '</option>');
                            });

                            designationDD.selectpicker('refresh');
                        } else {
                            designationDD.append('<option>No designation available</option>');
                        }
                        designationDD.selectpicker('refresh');
                    },
                    error: function () {
                        console.error('Failed to fetch designation');
                        toastr.info('Failed to fetch designation');
                    }
                });
            }


            function getEmployeeByBranch(selectedBranches) {
                var stringIds = selectedBranches.join(',');

                $.ajax({
                    type: 'GET',
                    url: '/EmployeeGeneralInfoReport/GetEmployeeByBranch',
                    data: { branchId: stringIds },
                    success: function (data) {
                        var employeeDD = $('#EmployeeCode');
                        employeeDD.empty();

                        if (data && data.length > 0) {

                            $.each(data, function (index, employee) {
                                employeeDD.append('<option value="' + employee.employeeId + '">' + employee.employeeName + '</option>');
                            });

                            employeeDD.selectpicker('refresh');
                        } else {
                            employeeDD.append('<option>No employee available</option>');
                        }
                        employeeDD.selectpicker('refresh');
                    },
                    error: function () {
                        console.error('Failed to fetch employee');
                        toastr.info('Failed to fetch employee');
                    }
                });
            }


            $(document).on('change', '#DepartmentName', function () {
                var selectedDepartments = $(this).val();
                if (!selectedDepartments) {
                    return;
                }

                var stringIds = selectedDepartments.join(',');

                $.ajax({
                    type: 'GET',
                    url: '/EmployeeGeneralInfoReport/GetDesignationByDepartment',
                    data: { departmentId: stringIds },
                    success: function (data) {
                        var designationDD = $('#DesignationCode');
                        designationDD.empty();

                        if (data && data.length > 0) {

                            $.each(data, function (index, designation) {
                                designationDD.append('<option value="' + designation.designationCode + '">' + designation.designationName + '</option>');
                            });

                            designationDD.selectpicker('refresh');
                        } else {
                            designationDD.append('<option>No designation available</option>');
                        }
                        designationDD.selectpicker('refresh');
                    },
                    error: function () {
                        console.error('Failed to fetch designation');
                        toastr.info('Failed to fetch designation');
                    }
                });
                getEmployeeByDepartment(selectedDepartments);
            });


            function getEmployeeByDepartment(selectedDepartments) {
                var stringIds = selectedDepartments.join(',');

                $.ajax({
                    type: 'GET',
                    url: '/EmployeeGeneralInfoReport/GetEmployeeByDepartment',
                    data: { departmentId: stringIds },
                    success: function (data) {
                        var departmentDD = $('#EmployeeCode');
                        departmentDD.empty();

                        if (data && data.length > 0) {

                            $.each(data, function (index, employee) {
                                departmentDD.append('<option value="' + employee.employeeId + '">' + employee.employeeName + '</option>');
                            });

                            departmentDD.selectpicker('refresh');
                        } else {
                            departmentDD.append('<option>No employee available</option>');
                        }
                        departmentDD.selectpicker('refresh');
                    },
                    error: function () {
                        console.error('Failed to fetch employee');
                        toastr.info('Failed to fetch employee');
                    }
                });
            }

            // Reset the form within the container




            $(document).on('change', '#DesignationCode', function () {
                var selectedDesignations = $(this).val();
                if (!selectedDesignations) {
                    return;
                }



                var stringIds = selectedDesignations.join(',');

                $.ajax({
                    type: 'GET',
                    url: '/EmployeeGeneralInfoReport/GetEmployeeByDesignation',
                    data: { designationId: stringIds },
                    success: function (data) {
                        var employeeDD = $('#EmployeeCode');
                        employeeDD.empty();

                        if (data && data.length > 0) {

                            $.each(data, function (index, employee) {
                                employeeDD.append('<option value="' + employee.employeeId + '">' + employee.employeeName + '</option>');
                            });

                            employeeDD.selectpicker('refresh');
                        } else {
                            employeeDD.append('<option>No employee available</option>');
                        }
                        employeeDD.selectpicker('refresh');
                    },
                    error: function () {
                        console.error('Failed to fetch employee');
                        toastr.info('Failed to fetch employee');
                    }
                });
            });



            initialize();

            function initialize() {
                $('.selectpickerReport').select2({
                    language: {
                        noResults: function () {

                        }
                    },
                    escapeMarkup: function (markup) {
                        return markup;
                    }
                });




                $('.multiselectCustom').selectpicker({
                    liveSearch: true,
                   
                    enableSelectedText: true,
                    liveSearchPlaceholder: 'Search...',
                    size: 10,
                    selectedTextFormat: 'count',

                    actionsBox: true,
                    iconBase: 'fa',
                    showTick: true,
                    tickIcon: 'fa-check',

                    container: 'body',
                });
                $('.multiselectCustom').selectpicker('deselectAll');
                //

            }

            // PreView
            $(document).ready(function () {

                $('#previewReportBtn').click(function () {



                    var departmentCode = $("#DepartmentName").val() || [];
                    var designationCode = $("#DesigantionCode").val() || [];
                    var employeeCode = $("#EmployeeCode").val() || [];
                    var branchCode = $("#BranchCode").val() || [];
                    var companyCode = $("#CompanyCode").val() || [];
                    //
                   
                    var nationalityCode = $("#NationalityCode").val() || '';
                    var genderCode = $("#GenderCode").val() || '';
                    var bloodGroupCode = $("#BloodGroupCode").val() || '';
                    var religionCode = $("#ReligionCode").val() || '';
                    var maritalStatusCode = $("#MaritalStatusCode").val() || '';


                    var formattedDepartmentCode = Array.isArray(departmentCode) ? departmentCode.join(',') : departmentCode;
                    var formattedDesignationCode = Array.isArray(designationCode) ? designationCode.join(',') : designationCode;
                    var formattedEmployeeCode = Array.isArray(employeeCode) ? employeeCode.join(',') : employeeCode;
                    var formattedBranchCode = Array.isArray(branchCode) ? branchCode.join(',') : branchCode;
                    var formattedCompanyCode = Array.isArray(companyCode) ? companyCode.join(',') : companyCode;

                    // $("#pdfPreviewModal").modal('show');
                    // $("#loadingIndicator").show();
                    $("#spi").show();

                    let previewUrl = "/EmployeeGeneralInfoReport/ExportEmployeeInfoToPdfPreView?" +
                        "departmentCode=" + encodeURIComponent(formattedDepartmentCode) +
                        "&designationCode=" + encodeURIComponent(formattedDesignationCode) +
                        "&employeeCode=" + encodeURIComponent(formattedEmployeeCode) +
                        "&branchCode=" + encodeURIComponent(formattedBranchCode) +
                        "&companyCode=" + encodeURIComponent(formattedCompanyCode) +

                        "&nationalityCode=" + encodeURIComponent(nationalityCode) +

                        "&genderCode=" + encodeURIComponent(genderCode) +
                        "&bloodGroupCode=" + encodeURIComponent(bloodGroupCode) +
                        "&religionCode=" + encodeURIComponent(religionCode) +
                        "&maritalStatusCode=" + encodeURIComponent(maritalStatusCode) +

                        "&preview=true";

                    console.log(previewUrl);

                    //  $("#pdfPreviewModal").modal('hide');

                    $("#spi").hide();

                    $("#pdfPreviewFrame").attr("src", previewUrl);
                    $("#pdfPreviewFrame").show();

                });
            });


            //

           

            //$(document).ready(function () {

            //    $('#previewReportBtn').click(function () {



            //        var departmentCode = $("#DepartmentName").val() || [];
            //        var designationCode = $("#DesigantionCode").val() || [];
            //        var employeeCode = $("#EmployeeCode").val() || [];
            //        var branchCode = $("#BranchCode").val() || [];
            //        var companyCode = $("#CompanyCode").val() || [];
                   

            //        var formattedDepartmentCode = Array.isArray(departmentCode) ? departmentCode.join(',') : departmentCode;
            //        var formattedDesignationCode = Array.isArray(designationCode) ? designationCode.join(',') : designationCode;
            //        var formattedEmployeeCode = Array.isArray(employeeCode) ? employeeCode.join(',') : employeeCode;
            //        var formattedBranchCode = Array.isArray(branchCode) ? branchCode.join(',') : branchCode;
            //        var formattedCompanyCode = Array.isArray(companyCode) ? companyCode.join(',') : companyCode;

            //        // $("#pdfPreviewModal").modal('show');
            //        // $("#loadingIndicator").show();
            //        $("#spi").show();

            //        let previewUrl = "/EmployeeGeneralInfoReport/ExportEmployeeOfficialInfoToPdfPreView?" +
            //            "departmentCode=" + encodeURIComponent(formattedDepartmentCode) +
            //            "&designationCode=" + encodeURIComponent(formattedDesignationCode) +
            //            "&employeeCode=" + encodeURIComponent(formattedEmployeeCode) +
            //            "&branchCode=" + encodeURIComponent(formattedBranchCode) +
            //            "&companyCode=" + encodeURIComponent(formattedCompanyCode) +
            //            "&employeeTypeCode=" + encodeURIComponent(employeeTypeCode) +
            //            "&employmentNatureId=" + encodeURIComponent(employmentNatureId) +
            //            "&nationalId=" + encodeURIComponent(nationalId) +


            //            "&preview=true";

            //        console.log(previewUrl);

            //        //  $("#pdfPreviewModal").modal('hide');

            //        $("#spi").hide();

            //        $("#pdfPreviewFrame").attr("src", previewUrl);
            //        $("#pdfPreviewFrame").show();

            //    });
            //});



            $("#exportReportBtn").click(function () {
                // $("#loadingIndicator").show();
                var departmentCode = $("#DepartmentName").val() || [];
                var designationCode = $("#DesigantionCode").val() || [];
                var exportFormat = $("#exportFormatDropdown").val();
                var employeeCode = $("#EmployeeCode").val() || [];
                var branchCode = $("#BranchCode").val() || [];
                var companyCode = $("#CompanyCode").val() || [];
                var nationalityCode = $("#NationalityCode").val() || '';
                var genderCode = $("#GenderCode").val() || '';
                var bloodGroupCode = $("#BloodGroupCode").val() || '';
                var religionCode = $("#ReligionCode").val() || '';
                var maritalStatusCode = $("#MaritalStatusCode").val() || '';
               
               


                if (!departmentCode && !designationCode && !employeeCode && !branchCode && !immediateSup && !companyCode && !nationalityCode && !genderCode && !bloodGroupCode && !religionCode && !maritalStatusCode)
                {
                    alert("Please select type.");
                    return;
                }
                if (!exportFormat) {
                    alert("Please select a Format.");
                    return;
                }

                var formattedDepartmentCode = Array.isArray(departmentCode) ? departmentCode.join(',') : departmentCode;
                var formattedDesignationCode = Array.isArray(designationCode) ? designationCode.join(',') : designationCode;
                var formattedemployeeCode = Array.isArray(employeeCode) ? employeeCode.join(',') : employeeCode;
                var formattedbranchCode = Array.isArray(branchCode) ? branchCode.join(',') : branchCode;
                var formattedcompanyCode = Array.isArray(companyCode) ? companyCode.join(',') : companyCode;

           


                var queryString = "";
                if (formattedDepartmentCode) {
                    queryString += "departmentCode=" + formattedDepartmentCode;
                }
                if (formattedDesignationCode) {
                    queryString += (queryString ? "&" : "") + "designationCode=" + formattedDesignationCode;
                }
                if (formattedemployeeCode) {
                    queryString += (queryString ? "&" : "") + "employeeCode=" + formattedemployeeCode;
                }
                if (formattedbranchCode) {
                    queryString += (queryString ? "&" : "") + "branchCode=" + formattedbranchCode;
                }
                if (formattedcompanyCode) {
                    queryString += (queryString ? "&" : "") + "companyCode=" + formattedcompanyCode;
                }
                if (nationalityCode) {
                    queryString += (queryString ? "&" : "") + "nationalityCode=" + nationalityCode;
                }
                if (genderCode) {
                    queryString += (queryString ? "&" : "") + "genderCode=" + genderCode;
                }
                if (bloodGroupCode) {
                    queryString += (queryString ? "&" : "") + "bloodGroupCode=" + bloodGroupCode;
                }

                if (religionCode) {
                    queryString += (queryString ? "&" : "") + "religionCode=" + religionCode;
                }

                if (maritalStatusCode) {
                    queryString += (queryString ? "&" : "") + "maritalStatusCode=" + maritalStatusCode;
                }


                if (exportFormat === "Excel") {
                    window.location.href = "/EmployeeGeneralInfoReport/Employee_Personal_Info_Report?" + queryString;

                } else if (exportFormat === "PDF") {
                    window.location.href = "/EmployeeGeneralInfoReport/Employee_Personal_Info_ReportPdf?" + queryString;
                }

                ResetForm();
                console.log(queryString);


            });



        });



     



    }

}(jQuery));




