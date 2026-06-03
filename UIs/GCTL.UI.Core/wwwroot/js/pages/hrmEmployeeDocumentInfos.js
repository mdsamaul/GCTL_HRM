(function ($) {
    $.hrmEmployeeDocumentInfos = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#HrmEmployeeDocumentInfos-form",
            formContainer: ".js-HrmEmployeeDocumentInfos-form-container",
            gridSelector: "#HrmEmployeeDocumentInfos-grid",
            gridContainer: ".js-HrmEmployeeDocumentInfos-grid-container",
            editSelector: ".js-HrmEmployeeDocumentInfos-edit",
            saveSelector: ".js-HrmEmployeeDocumentInfos-save",
            selectAllSelector: "#HrmEmployeeDocumentInfos-check-all",
            deleteSelector: ".js-HrmEmployeeDocumentInfos-delete-confirm",
            deleteModal: "#HrmEmployeeDocumentInfos-delete-modal",
            finalDeleteSelector: ".js-HrmEmployeeDocumentInfos-delete",
            clearSelector: ".js-HrmEmployeeDocumentInfos-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-HrmEmployeeDocumentInfos-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-HrmEmployeeDocumentInfos-check-availability",
            haseFile: true,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () {

            }
        }, options);



        var gridUrl = settings.baseUrl + "/Grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(window).on("load", function () {
            $("#customLoadingOverlay").fadeOut(300);
        });

        $(() => {

            var employeeId = $('#EmployeeId').val();
            if (employeeId) {
                loadTable(employeeId);
            }

            initialize();

            $(document).ready(function () {
                // Loader styling jQuery দিয়ে
                $("#loader").css({
                    "border": "16px solid #f3f3f3",
                    "border-top": "16px solid #3498db",
                    "border-radius": "50%",
                    "width": "120px",
                    "height": "120px",
                    "position": "absolute",
                    "top": "50%",
                    "left": "50%",
                    "transform": "translate(-50%, -50%)"
                });

                // Animation তৈরি jQuery দিয়ে
                let deg = 0;
                setInterval(function () {
                    deg += 10;
                    $("#loader").css("transform", "translate(-50%, -50%) rotate(" + deg + "deg)");
                }, 50);

                // Loader hide করে content দেখানো
                $(window).on("load", function () {
                    $("#loader").fadeOut("slow", function () {
                        $("#content").fadeIn("slow");
                    });
                });
            });
           
            $("body").on("click", `${settings.editSelector},${settings.clearSelector}`, function (e) {
              
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();

                const id = $(this).data("id");      
                const employeeId = $(this).data("id2");   

                let url = saveUrl + (id ? "/" + id : "");
           

                loadForm(url).then((data) => {
                    loadTable(employeeId); // Pass EmployeeId to filter the grid
                    console.info("Form Loaded Successfully", data);
                    $("#customLoadingOverlay").fadeOut(250);
                }).catch((error) => {
                    console.error("Failed to load form", error);
                    $("#customLoadingOverlay").fadeOut(250);
                });

                $("html, body").animate({ scrollTop: 0 }, 500);
            });



            // Save
            $("body").on("click", settings.saveSelector, function () {

                if (!validation()) return false;
                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }
                $("#customLoadingOverlay").fadeIn(200);
                var data;
                if (settings.haseFile) {
                    data = new FormData($(settings.formSelector)[0]);
                    var fileInput = $('#fileInput')[0].files[0];
                    if (fileInput) {
                        data.append('Photo', fileInput); 
                    }
                }
                else
                    data = $(settings.formSelector).serialize();

                var url = $(settings.formSelector).attr("action");
                var employeeId = $('#EmployeeId').val();
                var options = {
                    url: url,
                    method: "POST",
                    data: data,
                    success: function (response) {
                        if (response.isSuccess) {
                            loadForm(saveUrl)
                                .then((data) => {
                                    loadTable(employeeId);
                                    $(settings.lastCodeSelector).val(response.lastCode);
                                       $("#customLoadingOverlay").fadeOut(250);
                                })
                                .catch((error) => {
                                    $("#customLoadingOverlay").fadeOut(250);
                                    console.log(error)
                                })

                            toastr.success(response.message);
                        }
                        else {
                            toastr.error(response.message);
                            console.log(response);
                            $("#customLoadingOverlay").fadeOut(250);
                        }
                    }
                }
                if (settings.haseFile) {
                    options.processData = false;
                    options.contentType = false;
                }
                $.ajax(options);
            });

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


            $("body").on('show.bs.modal', settings.deleteModal, function (event) {

                var source = $(event.relatedTarget);
                var id = source.data("ids");
                var employeeId = $('#EmployeeId').val();
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
                            console.log(response);
                            $(modal).modal("hide");

                            if (response.success) {
                                loadForm(saveUrl)
                                    .then((data) => {
                                        selectedItems = [];
                                        loadTable(employeeId);
                                        $(settings.lastCodeSelector).val(response.lastCode);

                                    })
                                    .catch((error) => {
                                        console.log(error)
                                    })

                                toastr.success(response.message);
                            }
                            else {
                                toastr.error(response.message);
                                console.log(response);
                            }
                        }
                    });
                });

            }).on('hide.bs.modal', function () {
                $("body").off("click", settings.finalDeleteSelector);
            });

            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 0 }, 500);
            });


            $("body").on("keyup", settings.decimalSelector, function () {
                var self = $(this);
                showDecimalPlaces(self.val(), self.parent().find(".input-group-text"));
            });






            //

            //$("body").on("keyup change", "#EmployeeId, #EmpQualificationId, #CourseCode, #CourseTitleCode", function () {
            //    // Get values from the form
            //    let code = $("#EmpQualificationId").val();
            //    let employeeCode = $("#EmployeeId").val();
            //    let courseTypeId = $("#CourseCode").val();
            //    let courseTitleId = $("#CourseTitleCode").val();

            //    // Perform an AJAX call to check for duplicates
            //    $.ajax({
            //        url: settings.baseUrl + "/CheckAvailability",
            //        method: "POST",
            //        data: {
            //            code: code,
            //            employeeCode: employeeCode,
            //            courseTypeId: courseTypeId,
            //            couresetitleID: courseTitleId
            //        },
            //        success: function (response) {
            //            if (response.isSuccess) {
            //                // Show warning message if duplicate exists
            //                toastr.warning(response.message);
            //            }
            //        },
            //        error: function () {
            //            toastr.error("An error occurred while checking for duplicates.");
            //        }
            //    });
            //});


            //

            $('body').on('change', "#EmployeeId", function () {

                var selectedEmployee = $(this).val();

                $.ajax({
                    url: '/HrmEmployeeDocumentInfos/GetEmployeeNameDesDeptByCode',
                    type: 'GET',
                    data: { employeeId: selectedEmployee },
                    success: function (data) {
                        $('#DesignationName').text(data.designationName);
                        $('#DepartmentName').text(data.departmentName);
                        $('#FullName').text(data.employeeName);
                        //console.log(data);
                    },
                    error: function () {

                    }
                });
            });

            //


            $('body').on('change', "#CompanyCode", function () {

                var selectedComapny = $(this).val();

                $.ajax({
                    url: '/HrmEmployeeDocumentInfos/GetBranchByCode',
                    type: 'GET',
                    data: { ComapnyCode: selectedComapny },
                    success: function (data) {


                        if (data && data.length > 0) {
                            var braDropdown = $('#BranchCode');
                            braDropdown.empty();
                            braDropdown.append('<option value="">--Select Branch--</option>');
                            $.each(data, function (index, br) {
                                console.log('this is br : ', br);
                                braDropdown.append('<option value="' + br.branchCode + '">' + br.coreBranchName + '</option>');
                            });

                            braDropdown.trigger('change');
                        } else {
                            var braDropdown = $('#BranchCode');
                            braDropdown.empty();
                            braDropdown.append('<option value="">No Branches available</option>');
                        }
                    },
                    error: function () {

                    }
                });
            }); 


            $('body').on('change', "#CompanyCode", function () {
                var selectedComapny = $(this).val();
                $.ajax({
                    url: '/HrmEmployeeDocumentInfos/GetEmployeeDetailsByComapnyCode',
                    type: 'GET',
                    data: { companyCode: selectedComapny },
                    success: function (data) {

                        //
                        if (data && data.length > 0) {
                            var employeeDropdown = $('#EmployeeId');
                            employeeDropdown.empty();
                            employeeDropdown.append('<option value="">---- Select Employee ----</option>');
                            $.each(data, function (index, employee) {
                                employeeDropdown.append('<option value="' + employee.employeeId + '">' + employee.employeeName + '</option>');
                            });

                            employeeDropdown.trigger('change');
                        } else {
                            var employeeDropdown = $('#EmployeeId');
                            employeeDropdown.empty();
                            employeeDropdown.append('<option value="">No employees available</option>');
                        }

                        //


                        //console.log(data);
                    },
                    error: function () {

                    }
                });

            });
            //

            $("body").on('change', "#EmployeeId", function () {

                var selectedEmployee = $(this).val();
                $.ajax({
                    url: '/HrmEmployeeDocumentInfos/GetTableData',
                    type: 'GET',
                    data: { employeeId: selectedEmployee },
                    success: function (data) {
                        $(settings.gridContainer).html(data);
                        loadTable(selectedEmployee);
                    }, error: function () {
                        toastr.error('Failed to load data');
                    }
                });
            });

            //




            //

        });


        //

        //
        function loadTable(employeeId) {
            $.get(settings.baseUrl + "/GetTableData", { employeeId: employeeId })
                .done(html => {
                    $(settings.gridContainer).html(html);

                    if ($.fn.DataTable.isDataTable(settings.gridSelector)) {
                        $(settings.gridSelector).DataTable().destroy();
                    }

                    $(settings.gridSelector).DataTable({
                        lengthChange: true,
                        pageLength: 10,
                        lengthMenu: [
                            [10, 25, 50, -1],
                            [10, 25, 50, 'All'],
                        ],
                        order: [[1, "desc"]],
                        destroy: true, // Allow reinitialization
                        paging: true,
                        searching: true,
                        responsive: true,
                    });

                })
                .fail(() => toastr.error("Failed to load table data."));
        }


        function loadForm(url) {
            return new Promise((resolve, reject) => {
                var employeeId = $('#EmployeeId').val();
                $.ajax({
                    url: url,
                    type: 'GET',
                    cache: false,
                    success: function (data) {
                        $(settings.formContainer).empty();
                        $(settings.formContainer).html(data);
                        $('#EmployeeId').val(employeeId);
                        $.validator.unobtrusive.parse($(settings.formSelector));

                        initialize();
                        resolve(data)
                    },
                    error: function (error) {
                        reject(error)
                    },
                })
            })
        }

        //

        function validation() {

            var compName = $('#CompanyCode').val();
            var empName = $('#EmployeeId').val();
            if (!compName) {
                toastr.info('Select Company');
                $('#CompanyCode').select2('open');
                return false;
            }
            if (!empName) {
                toastr.info('Select Employee');
                $('#EmployeeId').select2('open');
                return false;
            }

            return true;

        }
        //


        function initialize() {
            $(settings.formSelector + ' .HrmEmployeeDocumentInfosselectpicker').select2({

                language: {
                    noResults: function () {

                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });

        }



    }

}(jQuery));  