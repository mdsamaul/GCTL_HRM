(function ($) {
    $.patients = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#patient-form",
            formContainer: ".js-patient-form-container",
            gridSelector: "#patient-grid",
            gridContainer: ".js-patient-grid-container",
            editSelector: ".js-patient-edit",
            saveSelector: ".js-patient-save",
            selectAllSelector: "#patient-check-all",
            deleteSelector: ".js-patient-delete-confirm",
            deleteModal: "#patient-delete-modal",
            finalDeleteSelector: ".js-patient-delete",
            clearSelector: ".js-patient-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-patient-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-patient-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            load: function () {

            }
        }, options);


        var gridUrl = settings.baseUrl + "/grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(() => {
            //settings.load(settings.baseUrl, settings.gridSelector);
            loadPatients(settings.baseUrl, settings.gridSelector);
            initialize();
            referenceSummary($("#ReferenceId").val());
            loadByPatientType($("#PatientTypeCode").val());
            $("body").on("click", `${settings.editSelector},${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = saveUrl + "/" + $(this).data("id") ?? "";

                loadForm(url)
                    .then((data) => {
                        referenceSummary($("#ReferenceId").val());
                    })
                    .catch((error) => {
                        console.log(error)
                    })

                $("html, body").animate({ scrollTop: 0 }, 500);
            });

            // Save
            $("body").on("click", settings.saveSelector, function () {
                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }

                $(settings.formSelector).submit();
                //var data;
                //if (settings.haseFile)
                //    data = new FormData($(settings.formSelector)[0]);
                //else
                //    data = $(settings.formSelector).serialize();

                //var url = $(settings.formSelector).attr("action");

                //var options = {
                //    url: url,
                //    method: "POST",
                //    data: data,
                //    success: function (response) {
                //        if (response.isSuccess) {
                //            loadForm(saveUrl)
                //                .then((data) => {
                //                    loadPatients(settings.baseUrl, settings.gridSelector);
                //                    /*settings.load(settings.baseUrl, settings.gridSelector);*/
                //                })
                //                .catch((error) => {
                //                    console.log(error)
                //                })

                //            toastr.success(response.success, 'Success');
                //        }
                //        else {
                //            toastr.error(response.message, 'Error');
                //            console.log(response);
                //        }
                //    }
                //}
                //if (settings.haseFile) {
                //    options.processData = false;
                //    options.contentType = false;
                //}
                //$.ajax(options);
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
                    toastr.info("Please select at least one item.", 'Warning');
                }
            });


            $("body").on("click", ".js-patient-export", function () {
              
                $('input:checkbox.checkBox').each(function () {
                    if ($(this).prop('checked')) {
                        if (!selectedItems.includes($(this).val())) {
                            selectedItems.push($(this).val());
                        }
                    }
                });

                if (selectedItems.length > 0) {
                    var self = $(this);
                    let reportRenderType = self.data("rendertype");

                    $(selectedItems).each(function (index, item) {
                        window.open(
                            settings.baseUrl + `/Export?PatientCode=${item}&reportType=PatientSingelReport&reportRenderType=${reportRenderType}`,
                            "_blank"
                        )
                    })
                } else {
                    toastr.info("Please select at least one item.", 'Warning');
                }
            });

            $("body").on('show.bs.modal', settings.deleteModal, function (event) {
                //event.preventDefault();
                // Get button that triggered the modal
                var source = $(event.relatedTarget);
                var id = source.data("id");

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
                        url: deleteUrl + "/" + selectedItems,
                        method: "POST",
                        success: function (response) {
                            console.log(response);
                            $(modal).modal("hide");
                            if (response.isSuccess) {
                                toastr.success(response.message, 'Success');
                                selectedItems = [];
                                /*settings.load(settings.baseUrl);*/
                                loadPatients(settings.baseUrl, settings.gridSelector);
                                loadForm(saveUrl);
                            }
                            else {
                                selectedItems = [];
                                toastr.error(response.message, 'Error');
                                console.log(response);
                            }
                        }
                    });
                });

            }).on('hide.bs.modal', function () {
                $("body").off("click", settings.finalDeleteSelector);
            });



            $("body").on("click", ".js-patient-sms-confirm", function (e) {
                e.preventDefault();
                $('input:checkbox.checkBox').each(function () {
                    if ($(this).prop('checked')) {
                        if (!selectedItems.includes($(this).val())) {
                            selectedItems.push($(this).val());
                        }
                    }
                });

                if (selectedItems.length > 0) {
                    $("#patient-sms-modal").modal("show");
                } else {
                    toastr.info("Please select at least one item.", 'Warning');
                }
            });


            $("body").on('show.bs.modal', "#patient-sms-modal", function (event) {
                //event.preventDefault();
                // Get button that triggered the modal
                var source = $(event.relatedTarget);
                var id = source.data("id");

                // Extract value from data-* attributes
                var title = source.data("title");
                title = "Are you sure want to send sms to these patients?";
                var modal = $(this);
                $(modal).find('.title').html(title);

                $("body").on("click", ".js-patient-sms", function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    e.stopImmediatePropagation();


                    // SMS
                    $.ajax({
                        url: settings.baseUrl + "/SendSMS/" + selectedItems,
                        method: "POST",
                        success: function (response) {
                            $(modal).modal("hide");
                            if (response.isSuccess) {
                                toastr.success(response.message, 'Success');
                                selectedItems = [];
                            }
                            else {
                                selectedItems = [];
                                toastr.error(response.message, 'Error');
                                console.log(response);
                            }
                        }
                    });
                });

            }).on('hide.bs.modal', function () {
                $("body").off("click", ".js-patient-sms");
            });

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
                    // $("#header").hide();
                    $(settings.quickAddModal + " .modal-body #header").hide()

                    // $("#left_menu").hide();
                    $(settings.quickAddModal + " .modal-body #left_menu").hide()

                    // $("#main-content").toggleClass("collapse-main");
                    $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")

                    $("body").removeClass("sidebar-mini");
                })
            });

            $("body").on("click", ".js-modal-dismiss", function () {
                $("body").removeClass("sidebar-mini").addClass("sidebar-mini");
                $(settings.quickAddModal + " .modal-body #header").show()

                //  $("#left_menu").show();
                $(settings.quickAddModal + " .modal-body #left_menu").show()

                // $("#main-content").toggleClass("collapse-main");
                $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")


                lastCode = $(settings.quickAddModal + " #lastCode").val();

                $(settings.quickAddModal + " .modal-body").empty();
                $(settings.quickAddModal).modal("hide");


                $(target).empty("");
                $(target).append($('<option>', {
                    value: '',
                    text: `Select ${title}`
                }));
                $.ajax({
                    url: reloadUrl,
                    method: "GET",
                    success: function (response) {
                        $.each(response, function (i, item) {
                            $(target).append($('<option>', {
                                value: item.code,
                                text: item.name
                            }));
                        });
                        $(target).val(lastCode);
                    }
                });
            });

            $("body").on("change", "#RoomTypeCode", function () {
                var self = $(this);
                if (self.val().length > 0) {
                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetRooms",
                        method: "POST",
                        data: { roomTypeCode: self.val() },
                        success: function (response) {
                            $("#RoomCode").empty();
                            $("#RoomCode").append($('<option>', {
                                value: '',
                                text: `Select Room`
                            }));
                            $("#BedCode").empty();
                            $("#BedCode").append($('<option>', {
                                value: '',
                                text: `Select Bed`
                            }));
                            $.each(response, function (i, item) {
                                $("#RoomCode").append($('<option>', {
                                    value: item.code,
                                    text: item.name
                                }));
                            });
                        }
                    });
                }
            });

            $("body").on("change", "#RoomCode", function () {
                var self = $(this);

                if (self.val().length > 0) {
                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetBeds",
                        method: "POST",
                        data: { roomCode: self.val() },
                        success: function (response) {
                            $("#BedCode").empty();
                            $("#BedCode").append($('<option>', {
                                value: '',
                                text: `Select Bed`
                            }));
                            $.each(response, function (i, item) {
                                $("#BedCode").append($('<option>', {
                                    value: item.code,
                                    text: item.name
                                }));
                            });
                        }
                    });
                }
            });

            $("body").on("change", "#ReferenceId", function () {
                var self = $(this);
                referenceSummary(self.val());
            });

            $("body").on("change", "#PatientTypeCode", function () {
                var self = $(this);

                //console.log($("#PatientTypeCode").find(':selected').text());
                //var patientType = $("#PatientTypeCode").select2('data').text;
                //if (patientType.includes("Out")) {
                //    alert("ok");
                //}
                //console.log(patientType);
                if (self.val().length > 0) {
                    $.ajax({
                        url: settings.baseUrl + "/GenerateCode",
                        method: "POST",
                        data: { patientTypeCode: self.val(), patientCode: $("#PatientCode").val(), patientId: $("#PatientId").val() },
                        success: function (response) {
                            if (response.isSuccess)
                                $("#PatientCode").val(response.message);
                            else
                                toastr.error(response.message);
                        }
                    });

                    loadByPatientType(self.val());
                }
            });

            $("body").on("change", "#DoctorCode", function () {
                var self = $(this);

                if (self.val().length > 0) {
                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetDoctorSummary",
                        method: "POST",
                        data: { doctorCode: self.val() },
                        success: function (response) {
                            let template = `<table class="fixed-table">
                                                <tr>
                                                    <td>${response.doctorName}</td>
                                                </tr>
                                                <tr>
                                                    <td>${response.qualification}</td>
                                                </tr>
                                                <tr>
                                                  <td>${response.specialist}</td>
                                                </tr>
                                                <tr>
                                                  <td>${response.designationName}</td>
                                                </tr>
                                                <tr>
                                                   <td>${response.appointmentDays}</td>
                                                </tr>
                                            </table>`;

                            $(".doctor-details").addClass("d-none")
                                .removeClass("d-none");
                            $(".doctor-details").empty();
                            $(".doctor-details").html(template);
                        }
                    });
                }
            });

            $("body").on("keyup", settings.availabilitySelector, function () {
                var self = $(this);
                let code = $(".js-code").val();
                let id = $("#PatientId").val();
                let name = self.val();

                // check
                $.ajax({
                    url: settings.baseUrl + "/CheckAvailability",
                    method: "POST",
                    data: { id: id, name: name },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.error(response.message);
                        }
                    }
                });
            });

            $("body").on("change", ".patienttype, .patientcategory, .department, .doctor, .activitystatus, .datefrom, .dateto", function () {
                loadPatients(settings.baseUrl, settings.gridSelector);
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

            $("body").on("change", "#SurnameId", function () {
                var surname = $("#SurnameId option:selected").text().toLowerCase();;
                if (surname.includes("mst") || surname.includes("mrs")) {
                    $("#SexCode").val(2);
                } else {
                    $("#SexCode").val(1);
                }

                $(".selectpicker").select2();
            });
        });

        function remove(selector) {
            $(selector).closest('tr').remove();
        }

        function showImagePreview(input, target) {
            //var target = $(input).data("target");
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

        function loadByPatientType(patientTypeCode) {
            $.ajax({
                url: settings.baseUrl + "/GetPatientType",
                method: "POST",
                data: { patientTypeCode: patientTypeCode },
                success: function (response) {
                    if (response.isSuccess) {
                        var patientType = response.message.patientTypeShortName;
                        if (patientType == "IDP") {
                            $(".lblAdmissionDate").text("Admission Date");
                            $(".IDP").fadeIn();
                        } else {
                            $(".lblAdmissionDate").text("Visit Date");
                            $(".IDP").fadeOut();
                        }
                    }
                    else
                        console.log(response.message);
                }
            });
        }

        function referenceSummary(referencePersonId) {
            if (referencePersonId != undefined && referencePersonId.length > 0) {
                $.ajax({
                    url: normalizeUrl(getBaseUrl()) + "/Cascading/GetReferenceSummary",
                    method: "POST",
                    data: { referencePersonId: referencePersonId },
                    success: function (response) {
                        $("#ReferenceMobile").val(response.referencePersonMobile);
                    }
                });
            }
        }

        function loadPatients(baseUrl, gridSelector) {
            var data = {
                patientTypeCode: $(".patienttype").val(),
                patientCategoryCode: $(".patientcategory").val(),
                departmentCode: $(".department").val(),
                doctorCode: $(".doctor").val(),
                activityStatus: $(".activitystatus").val(),
                fromDate: $(".datefrom").val(),
                toDate: $(".dateto").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/grid",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "patientCode", "className": "text-center", "render": function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "registrationId", "className": "text-center", autowidth: true,
                        render: function (data, type, row) {
                            return `<a href='${baseUrl}/Setup/${row.patientId}'>${data}</a>`;
                        }
                    },
                    { "data": "registrationDate", "width": "200px" },
                    { "data": "patientCode", "className": "text-center", width: "150px" },
                    { "data": "patientName", "className": "text-center", width: "150px" },
                    { "data": "phone", "className": "text-center", width: "120px" },
                    { "data": "patientCategoryName", "className": "text-center", width: "120px" },
                    { "data": "admissionDate", "className": "text-center", width: "180px" },
                    { "data": "disease", "className": "text-center", width: "200px" },
                    {
                        "data": "roomDetails", "className": "text-center", width: "200px",
                        render: function (data) {
                            // return `<a href='#'>${data}</a>`;
                            return `${data}`;
                        }
                    },
                    { "data": "serialNo", "className": "text-center", width: "120px" },                   
                    {
                        "data": "activityStatus", "className": "text-center", width: "120px",
                        render: function (data) {
                            return data;
                            //if (data == "Active") {
                            //    return 'Yes';
                            //    // return `<span class='btn btn-sm btn-success' title="${data}"><i class='fa fa-check-circle'></i></span>`;
                            //} else if (data = "Inactive") {
                            //    return 'No';
                            //    // return `<span class='btn btn-sm btn-danger' title="${data}"><i class='fa fa-times-circle'></i></span>`;
                            //}

                            //return 'No';
                        }
                    },
                    {
                        "data": "doctorName", "className": "text-center", width: "150px",
                        render: function (data) {
                            /*return `<a href='${baseUrl}/Setup/${data}'>${data}</a>`;*/
                            return `${data}`;
                        }
                    },
                    { "data": "departmentName", "className": "text-center", width: "200px" },
                    {
                        "data": "patientId", width: "150px", "render": function (data, type, row) {
                            return `<div class='action-buttons' style='width:60px'>
                                        <a class='btn btn-warning btn-circle btn-sm' title="Edit ${row.patientName}" href='${baseUrl}/Setup/${data}'><i class='fas fa-pencil-alt'></i></a>     
                                        <button type="button" class="btn btn-danger btn-circle btn-sm js-patient-delete-confirm"
                                                data-target="#deleteModalx"
                                                data-id="${data}"
                                                title="Delete ${row.patientName}"
                                                data-title="Are you sure want to delete ${row.patientName}?">
                                                    <i class="fas fa-trash fa-sm"></i>
                                        </button>
                                         <button type="button" class="btn btn-danger btn-circle btn-sm js-patient-sms-confirm"
                                                data-id="${data}"
                                                title="Send SMS to ${row.patientName}"
                                                data-title="Are you sure want to send sms to ${row.patientName}?">
                                                    <i class="fas fa-sms fa-sm"></i>
                                        </button>`;
                        },
                        "orderable": false,
                        "searchable": false,
                        width: "100px"
                    }
                ],
                lengthChange: true,
                pageLength: 10,
                lengthMenu: [
                    [10, 25, 50, -1],
                    [10, 25, 50, 'All'],
                ],
                order: [],
                //order: [[1, "Desc"]],
                destroy: true
            });
        }

        function loadForm(url) {
            return new Promise((resolve, reject) => {
                $.ajax({
                    url: url,
                    type: 'GET',
                    success: function (data) {
                        $(settings.formContainer).empty();
                        $(settings.formContainer).html(data);
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

        function initialize() {
            $('.selectpicker').select2({
                language: {
                    noResults: function () {
                        //return 'Not found <a class="add_new_item" href="javascript:void(0)">Add New</a>';
                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });

            $('.datepicker').datetimepicker({
                format: 'DD/MM/YYYY',
                /*showTodayButton: true,*/
                // Your Icons
                // as Bootstrap 4 is not using Glyphicons anymore
                icons: {
                    time: 'fas fa-clock',
                    date: 'fas fa-calendar',
                    up: 'fas fa-chevron-up',
                    down: 'fas fa-chevron-down',
                    previous: 'fas fa-chevron-left',
                    next: 'fas fa-chevron-right',
                    today: 'fas fa-check',
                    clear: 'fas fa-trash',
                    close: 'fas fa-times'
                }
            });

            $('.timepicker').datetimepicker({
                format: 'hh:mm A',
                /*showTodayButton: true,*/
                // Your Icons
                // as Bootstrap 4 is not using Glyphicons anymore
                icons: {
                    time: 'fas fa-clock',
                    date: 'fas fa-calendar',
                    up: 'fas fa-chevron-up',
                    down: 'fas fa-chevron-down',
                    previous: 'fas fa-chevron-left',
                    next: 'fas fa-chevron-right',
                    today: 'fas fa-check',
                    clear: 'fas fa-trash',
                    close: 'fas fa-times'
                }
            });

            $('.datetimepicker').datetimepicker({
                format: 'DD/MM/YYYY hh:mm A',
                /*showTodayButton: true,*/
                // Your Icons
                // as Bootstrap 4 is not using Glyphicons anymore
                icons: {
                    time: 'fas fa-clock',
                    date: 'fas fa-calendar',
                    up: 'fas fa-chevron-up',
                    down: 'fas fa-chevron-down',
                    previous: 'fas fa-chevron-left',
                    next: 'fas fa-chevron-right',
                    today: 'fas fa-check',
                    clear: 'fas fa-trash',
                    close: 'fas fa-times'
                }
            });
        }
    }

}(jQuery));

