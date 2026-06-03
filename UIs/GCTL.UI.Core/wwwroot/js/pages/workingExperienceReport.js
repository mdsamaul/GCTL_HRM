
(function ($) {
    $.workingExperienceReport = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#WorkingExperienceReport-form",
            formContainer: ".js-WorkingExperienceReport-form-container",
            gridSelector: "#WorkingExperienceReport-grid",
            gridContainer: ".js-WorkingExperienceReport-grid-container",
            editSelector: ".js-WorkingExperienceReport-edit",
            saveSelector: ".js-WorkingExperienceReport-save",
            selectAllSelector: "#WorkingExperienceReport-check-all",
            deleteSelector: ".js-WorkingExperienceReport-delete-confirm",
            deleteModal: "#WorkingExperienceReport-delete-modal",
            finalDeleteSelector: ".js-WorkingExperienceReport-delete",
            clearSelector: "#resetButton",
            topSelector: ".js-go",
            decimalSelector: ".js-WorkingExperienceReport-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-WorkingExperienceReport-check-availability",
            haseFile: false,

            load: function () {

            }
        }, options);




        $(() => {




            $(document).ready(function () {

                $(document).on('click', "#resetButton", function () {
                    ResetForm();
                });


            });

            function ResetForm() {
                $("#WorkingExperienceReport-form")[0].reset();
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


                $.ajax({
                    type: 'GET',
                    url: '/WorkingExperienceReport/GetBranchByCompany',
                    data: { companyId: selectedCompanies },
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


                $.ajax({
                    type: 'GET',
                    url: '/WorkingExperienceReport/GetDepartmentByCompany',
                    data: { companyId: selectedCompanies },
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

                $.ajax({
                    type: 'GET',
                    url: '/WorkingExperienceReport/GetDesignationByCompany',
                    data: { companyId: selectedCompanies },
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

                $.ajax({
                    type: 'GET',
                    url: '/WorkingExperienceReport/GetEmployeeByCompany',
                    data: { companyId: selectedCompanies },
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


                $.ajax({
                    type: 'GET',
                    url: '/WorkingExperienceReport/GetDepartmentByBranch',
                    data: { branchId: selectedBranches },
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


                $.ajax({
                    type: 'GET',
                    url: '/WorkingExperienceReport/GetDesignationByBranch',
                    data: { branchId: selectedBranches },
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


                $.ajax({
                    type: 'GET',
                    url: '/WorkingExperienceReport/GetEmployeeByBranch',
                    data: { branchId: selectedBranches },
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



                $.ajax({
                    type: 'GET',
                    url: '/WorkingExperienceReport/GetDesignationByDepartment',
                    data: { departmentId: selectedDepartments },
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


                $.ajax({
                    type: 'GET',
                    url: '/WorkingExperienceReport/GetEmployeeByDepartment',
                    data: { departmentId: selectedDepartments },
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





                $.ajax({
                    type: 'GET',
                    url: '/WorkingExperienceReport/GetEmployeeByDesignation',
                    data: { designationId: selectedDesignations },
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



                    var departmentCode = $("#DepartmentName").val() || '';
                    var designationCode = $("#DesigantionCode").val() || '';
                    var employeeCode = $("#EmployeeCode").val() || '';
                    var branchCode = $("#BranchCode").val() || '';
                    var companyCode = $("#CompanyCode").val() || '';
                    var nationalID = $("#NationalID").val() || '';
                    //


                    // $("#pdfPreviewModal").modal('show');
                    // $("#loadingIndicator").show();
                    $("#spi").show();

                    let previewUrl = "/WorkingExperienceReport/WorkingExperienceReportPreView?" +
                        "departmentCode=" + encodeURIComponent(departmentCode) +
                        "&designationCode=" + encodeURIComponent(designationCode) +
                        "&employeeCode=" + encodeURIComponent(employeeCode) +
                        "&branchCode=" + encodeURIComponent(branchCode) +
                        "&companyCode=" + encodeURIComponent(companyCode) +
                        "&nationalID=" + encodeURIComponent(nationalID) +
                        //nationalID

                        "&preview=true";

                    console.log(previewUrl);

                    //  $("#pdfPreviewModal").modal('hide');

                    $("#spi").hide();

                    $("#pdfPreviewFrame").attr("src", previewUrl);
                    $("#pdfPreviewFrame").show();

                });
            });




            $("#exportReportBtn").click(function () {
                // $("#loadingIndicator").show();
                var departmentCode = $("#DepartmentName").val() || '';
                var designationCode = $("#DesignationCode").val() || '';
                var exportFormat = $("#exportFormatDropdown").val();
                var employeeCode = $("#EmployeeCode").val() || '';
                var branchCode = $("#BranchCode").val() || '';
                var companyCode = $("#CompanyCode").val() || '';
                var nationalID = $("#NationalID").val() || '';


                if (!departmentCode && !designationCode && !employeeCode && !branchCode && !companyCode && !nationalID) {
                    toastr.info("Please select type.");
                    return;
                }
                if (!exportFormat) {
                    toastr.info("Please select a Format.");
                    return;
                }



                var queryString = "";
                if (departmentCode) {
                    queryString += "departmentCode=" + departmentCode;
                }
                if (designationCode) {
                    queryString += (queryString ? "&" : "") + "designationCode=" + designationCode;
                }
                if (employeeCode) {
                    queryString += (queryString ? "&" : "") + "employeeCode=" + employeeCode;
                }
                if (branchCode) {
                    queryString += (queryString ? "&" : "") + "branchCode=" + branchCode;
                }
                if (companyCode) {
                    queryString += (queryString ? "&" : "") + "companyCode=" + companyCode;
                }
                if (nationalID) {
                    queryString += (queryString ? "&" : "") + "nationalID=" + nationalID;
                }


                if (exportFormat === "Excel") {
                    window.location.href = "/WorkingExperienceReport/WorkingExperienceReportExcel?" + queryString;

                } else if (exportFormat === "PDF") {
                    window.location.href = "/WorkingExperienceReport/WorkingExperienceReportPdf?" + queryString;
                }

                ResetForm();
                console.log(queryString);


            });



        });







    }

}(jQuery));




