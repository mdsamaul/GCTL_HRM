(function ($) {
    $.hrmEmployee2 = function (options) {

        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#HrmEmployee2-form",
            formContainer: ".js-HrmEmployee2-form-container",
            gridSelector: "#HrmEmployee2-grid",
            gridContainer: ".js-HrmEmployee2-grid-container",
            editSelector: ".js-HrmEmployee2-edit",
            saveSelector: ".js-HrmEmployee2-save",
            selectAllSelector: "#HrmEmployee2-check-all",
            deleteSelector: ".js-HrmEmployee2-delete-confirm",
            deleteModal: "#HrmEmployee2-delete-modal",
            finalDeleteSelector: ".js-HrmEmployee2-delete",
            clearSelector: ".js-HrmEmployee2-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-HrmEmployee2-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-HrmEmployee2-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            load: function () {

            }
        }, options);


        var baseControllerNameUrl = "/HrmEmployee2";
        var testSave = baseControllerNameUrl + "/Setup";
        var nextCodeULR = baseControllerNameUrl + "/GenerateNextCode";
        var loadTableURL = baseControllerNameUrl + "/GetTableData";
        var deleteURL = baseControllerNameUrl + "/Delete";
        var getById = baseControllerNameUrl + "/Setup";
        var duplicateCheckURL = baseControllerNameUrl + "/CheckAvailability";
        var selectedItems = [];
        $(window).on("load", function () {
            $("#customLoadingOverlay").fadeOut(300);
        });


        $(() => {


            initialize();



            //
            // Edit leave type


            $("body").on('click', settings.editSelector, function () {


                var id = $(this).data('id');

                $.get(getById, { id: id }, function (result) {

                    $(settings.formSelector).html($(result).find(settings.formSelector).html());

                    $(settings.formSelector).attr('action', testSave);


                }).fail(function () {
                    toastr.error('Failed Update.');
                });
            });



            // Save button click event
            $("body").on('click', settings.saveSelector, function () {
                validation();
                $(settings.formSelector).submit();
            });

            // Form submission event
            $("body").on('submit', settings.formSelector, function (e) {
                e.preventDefault();
                var form = $(this)[0];
                var formData = new FormData(form);
                var actionUrl = $(this).attr('action');

                $.ajax({
                    type: 'POST',
                    url: actionUrl,
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        

                        if (result.noSavePermission || result.noUpdatePermission || result.isDuplicate) {
                            toastr.error(result.message);
                        } else if (result.isSuccess) {
                            $(settings.gridContainer).html(result.html);
                            initialize();

                            toastr.success(result.message);
                            setTimeout(function () {
                                window.location.href = result.redirectUrl;
                            }, 1000);

                        } else {
                            $(settings.formSelector).html(result);
                            initialize();

                        }
                    },
                    error: function () {
                        toastr.error('Failed Insert.');
                    }
                });
            });



            //Duplicate

            $("body").on("keyup change", "#FirstName, #FatherName, #DateOfBirthOrginalHidden,#EmployeeId", function () {

                let code = $("#EmployeeId").val();
                let firstName = $("#FirstName").val();
                let fathersName = $("#FatherName").val();
                let dateOfBirthOriginal = $("#DateOfBirthOrginalHidden").val();

                $.ajax({
                    url: duplicateCheckURL,
                    method: "POST",
                    data: { code: code, firstName: firstName, fathersName: fathersName, dateOfBirthOriginal: dateOfBirthOriginal },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.error(response.message);
                        }
                    }
                });

            });

            //
            $('#CompanyCode').change(function () {
                const companyCode = $(this).val();
                console.log("Selected company code:", companyCode); // Debugging the selected company code

                $.getJSON('/HrmEmployee2/GetBranchesByCompanyCode', { companyCode: companyCode }, function (data) {
                    console.log("Branches data:", data); // Debugging the response data
                    const branchDropdown = $('#BranchCode');
                    branchDropdown.empty();
                    if (data.length > 0) {
                        branchDropdown.append('<option value="">--Select Branch--</option>');
                        $.each(data, function (index, item) {
                            branchDropdown.append(`<option value="${item.value}">${item.text}</option>`);
                        });
                    }
                    else {
                        branchDropdown.append('<option value="">No branches available</option>');
                    }
                });
            });


            //


            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 0 }, 500);
            });


            let loadUrl,
                target,
                reloadUrl,
                title,
                lastCode;
            // Quick add
            $("body").on("click", settings.quickAddSelector, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                loadUrl = $(this).data("url");
                target = $(this).data("target");
                reloadUrl = $(this).data("reload-url");
                title = $(this).data("title");

                $(settings.quickAddModal + " .modal-title").html(title);
                $(settings.quickAddModal + " .modal-body").empty();

                $(settings.quickAddModal + " .modal-body").load(loadUrl, function () {
                    $(settings.quickAddModal).modal("show");
                    $("#header").hide();
                    $(settings.quickAddModal + " .modal-body #header").hide()

                    $("#left_menu").hide();
                    $(settings.quickAddModal + " .modal-body #left_menu").hide()

                    $("#main-content").toggleClass("collapse-main");
                    $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")

                    $("body").removeClass("sidebar-mini");
                })
            });

            $("body").on("click", ".js-modal-dismiss", function () {
                $("body").removeClass("sidebar-mini").addClass("sidebar-mini");

                $("#header").show();
                $(settings.quickAddModal + " .modal-body #header").show()

                $("#left_menu").show();

                $(settings.quickAddModal + " .modal-body #left_menu").show()

                $("#main-content").toggleClass("collapse-main");
                $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")


                lastCode = $(settings.quickAddModal + " #lastCode").val();

                $(settings.quickAddModal + " .modal-body").empty();
                $(settings.quickAddModal).modal("hide");


                $(target).empty("");
                $(target).append($('<option>', {
                    value: '',
                    text: `--Select ${title}--`
                }));
                $.ajax({
                    url: reloadUrl,
                    method: "GET",
                    success: function (response) {
                        console.log(response);
                        $.each(response, function (i, item) {
                            $(target).append($('<option>', {
                                value: item.code,
                                text: item.name
                            }));
                        });
                        console.log("Test", lastCode);
                        $(target).val(lastCode);
                    }
                });
            });



            // Select all checkboxes
            $("body").on('click', settings.selectAllSelector, function () {
                $('.checkBox').prop('checked', $(this).prop('checked'));
            });

            // Delete confirmation
            $("body").on('click', settings.deleteSelector, function (e) {
                e.preventDefault();
                var selectedIds = [];

                $('.checkBox:checked').each(function () {
                    selectedIds.push($(this).val());
                });

                if (selectedIds.length === 0) {
                    toastr.error('Please select employee to delete.');
                    return;
                }

                $(settings.deleteModal + ' ' + settings.finalDeleteSelector).data('ids', selectedIds);
                $(settings.deleteModal).modal('show');
            });

            // Final delete action
            $("body").on('click', settings.finalDeleteSelector, function (e) {
                e.preventDefault();
                var selectedIds = $(this).data('ids');

                $.ajax({
                    type: 'POST',
                    url: deleteURL,
                    contentType: 'application/json',
                    data: JSON.stringify(selectedIds),
                    success: function (result) {
                        if (result.isSuccess) {

                            loadTableData();
                            // GenerateNextCode();
                            toastr.success(result.message);
                            $('.checkBox').prop('checked', false);

                        } else if (result.refSuccess) {
                            toastr.warning(result.message);
                        } else {

                            toastr.error(result.message, 'Error');
                        }
                        $(settings.deleteModal).modal('hide');
                    },
                    error: function (xhr) {
                        toastr.error('An error.');
                        $(settings.deleteModal).modal('hide');
                    }
                });
            });

            //

            //

            $("body").on("keyup", settings.availabilitySelector, function () {
                var self = $(this);
                let code = $(".js-code").val();
                let name = self.val();

                // check
                $.ajax({
                    url: settings.baseUrl + "/CheckAvailability",
                    method: "POST",
                    data: { code: code, name: name },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.error(response.message);
                        }
                    }
                });
            });





            $("body").on("click", ".js-file-chooser", function (e) {
                e.preventDefault();
                var target = $(this).data("target");
                $(target).trigger("click");
            })

            $("body").on("change", ".js-file", function (e) {
                e.preventDefault();
                var target = $(this).data("target");
                showImagePreview($(this), target);
            })

            $("body").on("click", ".js-clear-file", function (e) {
                e.preventDefault();
                var file = $(this).data("file");
                var tag = $(this).data("tag");
                clearImage(file, tag);
            })


        });



        function showImagePreview(input, target) {

            if (input[0].files && input[0].files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $(target).prop('src', e.target.result);
                };
                reader.readAsDataURL(input[0].files[0]);
            }
        }

        function clearImage(file, tag) {

            console.log(file);
            console.log(tag);
            $(file).removeAttr("src");
            $(tag).val(true);
        }




        function initialize() {
            $('.selectpicker4').select2({
                language: {
                    noResults: function () {

                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });
            loadTableData();
            //GenerateNextCode();
            AllDatePicker();
            //ResetForm();
        }
        //
        function ResetForm() {

            $(settings.formSelector)[0].reset();

            $('#Id').val('');
            $('#AutoId').val('');
            $('#EmployeeId').val('');
            $('#FirstName').val('');
            $('#LastName').val('');
            $('#FatherName').val('');
            $('#MotherName').val('');
            $('#DateOfBirthCertificate').val('');
            $('#DateOfBirthOrginal').val('');
            $('#DateOfBirthOrginalHidden').val('');
            $('#BirthCertificateNo').val('');
            $('#PlaceOfBirth').val('');
            $('#MarriageDate').val('');
            $('#NoOfSon').val('');
            $('#NoOfDaughters').val('');
            $('#CardNo').val('');
            $('#UserInfoEmployeeId').val('');
            $('#PersonalEmail').val('');
            $('#NationalIdno').val('');
            $('#TinNo').val('');
            $('#Telephone').val('');
            $('#ExtraCurriActivities').val('');
            $('#Remarks').val('');
            $('#FirstNameBangla').val('');
            $('#LastNameBangla').val('');
            $('#FatherOccupation').val('');
            $('#MotherOccupation').val('');
            $('#Spouse')
            $('#BloodGroupCode').val(null).trigger('change');
            //$('#CompanyCode').val(null).trigger('change');
            $('#NationalityCode').val(null).trigger('change');
            $('#SexCode').val(null).trigger('change');
            $('#BranchCode').val(null).trigger('change');
            $('#ReligionCode').val(null).trigger('change');
            $('#MaritalStatusCode').val(null).trigger('change');

            $('#LdateModifyHide').hide();
            $(settings.saveSelector).html('<i class="fas fa-save"></i> Save');
            $('.text-danger').text('');
            $(settings.formSelector).attr('action', testSave);
            // GenerateNextCode();

        }

        $("body").on('click', settings.clearSelector, function () {
            ResetForm();
        });
        //
        // Validation function
        function validation() {
            var employeeId = $("#EmployeeId").val();
            var company = $("#CompanyCode").val();
            var branch = $("#BranchCode").val();
            var fName = $("#FirstName").val();
            var gender = $("#SexCode").val();
            var dOB = $('#DateOfBirthOrginal').val();

            if (!employeeId) {
                toastr.info('Select EmployeeId');
                $('#CompanyCode').trigger('focus');
                return false;
            }

            if (!company) {
                toastr.info('Select Company');
                $('#CompanyCode').select2('open');
                return false;
            }
            if (!branch) {
                toastr.info('Select Branch');
                $('#BranchCode').select2('open');
                return false;
            }
            if (!fName) {
                toastr.info('Enter  First Name');
                $('#FirstName').trigger('focus');
                return false;
            }

            if (!dOB) {
                toastr.info('Select Date of Birth(Orginal)');
                $('#DateOfBirthOrginal').trigger('focus');
                return false;
            }

            if (!gender) {
                toastr.info('Select Gender');
                $('#SexCode').select2('open');
                return false;
            }

        }



        //Load  list


        //function loadTableData() {
        //    $.ajax({
        //        type: 'Get',
        //        url: loadTableURL,
        //        success: function (data) {
        //            $(settings.gridContainer).html(data);
        //            dataTable();
        //        },
        //        error: function () {
        //            toastr.error('Failed Load Data');
        //        }
        //    });
        //}


        function loadTableData() {
            $.ajax({
                type: 'GET',
                url: loadTableURL, // example: "/HrmEmployee2/GetTableData"
                success: function (data) {
                    console.log("Datataaaaaaaa", data);
                    var table = $(settings.gridSelector).DataTable();
                    table.destroy();
                    if (data && data.length > 0) {
                        var tbody = $('#HrmEmployee2-grid tbody');
                        tbody.empty(); // Clear old rows

                        $.each(data.sort((a, b) => b.employeeId - a.employeeId), function (index, item) {
                            var row = `<tr>
                <td class="text-center text-middle">
                    <input type="checkbox" class="text-center text-middle checkBox" data-id="${item.employeeId}" value="${item.employeeId}" />
                </td>
                <td class="text-center text-middle">
                    <a title="Edit ${item.firstName} ${item.lastName}" style="text-decoration: underline;" href="/HrmEmployee2/Setup?id=${item.employeeId}">
                        ${item.employeeId}
                    </a>
                </td>
                <td class="text-left">${item.firstName} ${item.lastName}</td>
                <td class="text-left">${item.firstNameBangla} ${item.lastNameBangla}</td>
                <td class="text-center text-middle">${item.sexCode}</td>
                <td class="text-center text-middle">${item.nationalityCode}</td>
                <td class="text-center text-middle">${item.nationalIdno}</td>
                <td class="text-center text-middle">${item.telephone}</td>
                <td class="text-center text-middle">
                    ${item.photoUrl ? `<img src="${item.photoUrl}" class="rounded-circle" height="50" width="50" />` : `<div style="height: 90px; width: 90px;"></div>`}
                </td>
                <td class="text-center text-middle">
                    <a title="Edit ${item.firstName} ${item.fastName}" class="btn btn-sm" href="/HrmEmployee2/Setup?id=${item.employeeId}">
                        <i class="fas fa-pencil-alt"></i>
                    </a>
                    <button type="button" title="Delete ${item.firstName} ${item.lastName}" class="btn btn-sm js-HrmEmployee2-delete-confirm">
                        <i class="fas fa-trash fa-sm">&nbsp;</i>
                    </button>
                </td>
            </tr>`;

                            tbody.append(row);
                        });

                        dataTable(); // Initialize/Refresh DataTable
                    } else {
                        var tbody = $('#HrmEmployee2-grid tbody');
                        tbody.html(`<tr><td colspan="10" class="text-center text-middle">No Employee Information Available.</td></tr>`);
                    }
                },
                error: function () {
                    toastr.error('Failed Load Data');
                }
            });
        }


        // Data table initialization function
        function dataTable() {
            $(settings.gridSelector).DataTable({
                responsive: true,
                pageLength: 10,
                destroy: true,
                lengthMenu: [5, 10, 25, 50, 100],
            });
        }





        function AllDatePicker() {


            $("#DateOfBirthOrginal").datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: "-50:+0",
                beforeShow: function (input, inst) {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onClose: function (dateText, inst) {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onSelect: function (dateText) {
                    var formattedDate = $.datepicker.formatDate('mm/dd/yy', $(this).datepicker('getDate'));
                    $('#DateOfBirthOrginalHidden').val(formattedDate).trigger('change');
                    $("#DateOfBirthOrginal").valid();
                }
            });

            var initialDate = $("#DateOfBirthOrginal").val();
            if (initialDate) {
                var formattedInitialDate = $.datepicker.formatDate('mm/dd/yy', $("#DateOfBirthOrginal").datepicker('getDate'));
                $('#DateOfBirthOrginalHidden').val(formattedInitialDate);
            }

            $(".datepicker").trigger("input");


            //   


            $("#DateOfBirthCertificate").datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: "-50:+0",
                beforeShow: function (input, inst) {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onClose: function (dateText, inst) {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onSelect: function (dateText) {
                    var formattedDate = $.datepicker.formatDate('mm/dd/yy', $(this).datepicker('getDate'));
                    $('#DateOfBirthCertificateHidden').val(formattedDate).trigger('change');
                    $("#DateOfBirthCertificate").valid();
                }
            });

            var initialDate = $("#DateOfBirthCertificate").val();
            if (initialDate) {
                var formattedInitialDate = $.datepicker.formatDate('mm/dd/yy', $("#DateOfBirthCertificate").datepicker('getDate'));
                $('#DateOfBirthCertificateHidden').val(formattedInitialDate);
            }

            $(".datepicker").trigger("input");



            //    

            $("#MarriageDate").datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: "-50:+0",
                beforeShow: function (input, inst) {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onClose: function (dateText, inst) {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onSelect: function (dateText) {
                    var formattedDate = $.datepicker.formatDate('mm/dd/yy', $(this).datepicker('getDate'));
                    $('#MarriageDateHidden').val(formattedDate).trigger('change');
                    $("#MarriageDate").valid();
                }
            });

            var initialDate = $("#MarriageDate").val();
            if (initialDate) {
                var formattedInitialDate = $.datepicker.formatDate('mm/dd/yy', $("#MarriageDate").datepicker('getDate'));
                $('#MarriageDateHidden').val(formattedInitialDate);
            }
        }
    }
}(jQuery));