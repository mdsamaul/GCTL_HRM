(function ($) {
    $.discharges = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#discharge-form",
            formContainer: ".js-discharge-form-container",
            gridSelector: "#patients-grid",
            gridContainer: ".js-discharge-grid-container",
            editSelector: ".js-discharge-edit",
            saveSelector: ".js-discharge-save",
            selectAllSelector: "#discharge-check-all",
            deleteSelector: ".js-discharge-delete-confirm",
            deleteModal: "#discharge-delete-modal",
            finalDeleteSelector: ".js-discharge-delete",
            clearSelector: ".js-discharge-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-discharge-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-discharge-check-availability",
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
            $(settings.saveSelector).attr("disabled", "disabled");
            $("body").on("click", `${settings.editSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = settings.baseUrl + $(this).data("url");
                // let url = saveUrl + "/" + $(this).data("id") ?? "";

                loadForm(url)
                    .then((data) => {
                    })
                    .catch((error) => {
                        console.log(error)
                    })
                $(settings.saveSelector).removeAttr("disabled");
                //$("#labTestDetails").offset().top
                $("html, body").animate({ scrollTop: 500 }, 500);
            });

            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                loadPatients(settings.baseUrl, settings.gridSelector);
                loadForm(saveUrl);
                initialize();
                $(settings.saveSelector).attr("disabled", "disabled");
                // $("html, body").animate({ scrollTop: 0 }, 500);
            });



            $("body").on("click", "#medicine-grid .js-add", function () {
                if ($("#Medicine_MedicineCode").val() == '') {
                    $("#Medicine_MedicineCodeError").addClass("d-none")
                        .removeClass("d-none");
                    $("#Medicine_MedicineCode").focus();
                    return;
                }
                else {
                    $("#Medicine_MedicineCodeError").removeClass("d-none")
                        .addClass("d-none");
                }

                if ($("#Medicine_DosageCode").val() == '') {
                    $("#Medicine_DosageCodeError").addClass("d-none")
                        .removeClass("d-none");
                    $("#Medicine_DosageCode").focus();
                    return;
                }
                else {
                    $("#Medicine_DosageCodeError").removeClass("d-none")
                        .addClass("d-none");
                }

                let medicineCode = $("#Medicine_MedicineCode").val(),
                    medicineName = $("#Medicine_MedicineCode option:selected").text(),
                    medicineSerial = $("#Medicine_SerialNo").val(),

                    dosageName = $("#Medicine_DosageCode option:selected").text(),
                    dosage = $("#Medicine_DosageCode").val(),
                    takingTime = $("#Medicine_TakeTime").val(),
                    takingTimeUnit = $("#TakeUnit").val(),
                    takingWhen = $("#Medicine_TakeCode option:selected").text(),
                    takingCode = $("#Medicine_TakeCode").val(),
                    duration = $("#Medicine_Duration").val(),
                    durationUnitName = $("#Medicine_DurationCode option:selected").text(),
                    durationUnit = $("#Medicine_DurationCode").val(),
                    medicineInstruction = $("#Medicine_Instruction").val(),
                    counter = $("#medicine-grid tbody tr").length;

                if (medicineSerial != "") {
                    $("#medicine-" + medicineSerial + " .medicinename-title").html(medicineName);
                    $("#medicine-" + medicineSerial + " .MedicineCode").val(medicineCode);
                    $("#medicine-" + medicineSerial + " .SerialNo").val(medicineSerial);

                    $("#medicine-" + medicineSerial + " .dosage-title").html(dosageName);
                    $("#medicine-" + medicineSerial + " .DosageCode").val(dosage);

                    $("#medicine-" + medicineSerial + " .taketime-title").html(takingTime + " " + takingTimeUnit);
                    $("#medicine-" + medicineSerial + " .TakeTime").val($("#Medicine_TakeTime").val());

                    $("#medicine-" + medicineSerial + " .takename-title").html(takingWhen);
                    $("#medicine-" + medicineSerial + " .TakeCode").val(takingCode);

                    $("#medicine-" + medicineSerial + " .duration-title").html(duration);
                    $("#medicine-" + medicineSerial + " .Duration").val(duration);

                    $("#medicine-" + medicineSerial + " .durationname-title").html(durationUnitName);
                    $("#medicine-" + medicineSerial + " .DurationCode").val(durationUnit);

                    $("#medicine-" + medicineSerial + " .instruction-title").html(medicineInstruction);
                    $("#medicine-" + medicineSerial + " .Instruction").val(medicineInstruction);
                } else {
                    let item =
                        `<tr id="medicine-${counter}">
                        <td>
                            <span class="medicinename-title">${medicineName}</span>
                            <input type="hidden" id="Medicines_${counter}__MedicineCode" name="Medicines[${counter}].MedicineCode" value="${medicineCode}" class="MedicineCode" />
                            <input type="hidden" id="Medicines_${counter}__SerialNo" name="Medicines[${counter}].SerialNo" value="${counter}" class="SerialNo" />
                        </td>
                        <td>
                            <span class="dosage-title">${dosageName}</span>
                            <input type="hidden" id="Medicines_${counter}__DosageCode" name="Medicines[${counter}].DosageCode" value="${dosage}" class="DosageCode"/>
                        </td>
                        <td>
                            <span class="taketime-title">
                                    ${takingTime} ${takingTimeUnit}
                             </span>
                            <input type="hidden" id="Medicines_${counter}__TakeTime" name="Medicines[${counter}].TakeTime" value="${takingTime}" class="TakeTime" />
                            <input type="hidden" id="Medicines_${counter}__TakeUnit" name="Medicines[${counter}].TakeUnit" value="${takingTimeUnit}" class="TakeUnit" />
                        </td>
                        <td>
                            <span class="takename-title">${takingWhen}</span>
                            <input type="hidden" id="Medicines_${counter}__TakeCode" name="Medicines[${counter}].TakeCode" value="${takingCode}" class="TakeCode" />
                        </td>
                        <td>
                            <span class="duration-title">${duration}</span>
                            <input type="hidden" id="Medicines_${counter}__Duration" name="Medicines[${counter}].Duration" value="${duration}" class="Duration" />
                        </td>
                        <td>
                            <span class="durationname-title">${durationUnitName}</span>
                            <input type="hidden" id="Medicines_${counter}__DurationCode" name="Medicines[${counter}].DurationCode" value="${durationUnit}" class="DurationCode" />
                        </td>
                        <td>
                            <span class="instruction-title">${medicineInstruction}</span>
                            <input type="hidden" id="Medicines_${counter}__Instruction" name="Medicines[${counter}].Instruction" value="${medicineInstruction}" class="Instruction" />
                        </td>
                       <td>
                            <button type="button" class="btn btn-sm btn-success js-edit" title="Edit medicine"><i class="fas fa-pencil-alt"></i></button>
                            <button type='button' class='btn btn-sm btn-danger js-remove' title="Remove medicine"><i class='fa fa-trash'></i></button>
                       </td>
                    </tr>`;

                    $("#medicine-grid tbody").append(item);
                    counter++;
                }

                $("#Medicine_MedicineCode").val("");
                $("#Medicine_SerialNo").val("");
                $("#Medicine_DosageCode").val("");
                $("#Medicine_TakeTime").val("");
                $("#TakeUnit").val("");
                $("#Medicine_TakeCode").val("");
                $("#Medicine_Duration").val("");
                $("#Medicine_DurationCode").val("");
                $("#Medicine_Instruction").val("");


                initialize();
            });

            $("body").on("click", "#medicine-grid .js-edit", function (e) {
                e.preventDefault();
                $("#Medicine_SerialNo").val($(this).closest("tr").find(".SerialNo").val());
                $("#Medicine_MedicineCode").val($(this).closest("tr").find(".MedicineCode").val());
                $("#Medicine_DosageCode").val($(this).closest("tr").find(".DosageCode").val());
                $("#Medicine_TakeTime").val($(this).closest("tr").find(".TakeTime").val());
                $("#Medicine_TakeCode").val($(this).closest("tr").find(".TakeCode").val());
                $("#Medicine_Duration").val($(this).closest("tr").find(".Duration").val());
                $("#Medicine_DurationCode").val($(this).closest("tr").find(".DurationCode").val());
                $("#Medicine_Instruction").val($(this).closest("tr").find(".Instruction").val());

                $("#medicine-grid .js-add").html('<i class="fas fa-pencil-alt">');
                initialize($(this).closest("tr").find(".medicinename-title").text(), $(this).closest("tr").find(".MedicineCode").val());
            })

            $("body").on("click", "#medicine-grid .js-remove", function (e) {
                e.preventDefault();
                remove($(this));

                fixMedicineIndexing();
            })

            function remove(selector) {
                $(selector).closest('tr').remove();
            }
            function fixMedicineIndexing() {
                $("#medicine-grid tbody tr").each(function (index) {
                    var html = $(this).html();
                    html = html.replace(/\_(.*?)__/g, '_' + index + '__');
                    html = html.replace(/\[(.*?)\]/g, '[' + index + ']');
                    $(this).html(html);
                })
            }



            // Save
            $("body").on("click", settings.saveSelector, function () {
                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }

                var data;
                if (settings.haseFile)
                    data = new FormData($(settings.formSelector)[0]);
                else
                    data = $(settings.formSelector).serialize();

                var url = $(settings.formSelector).attr("action");

                var options = {
                    url: url,
                    method: "POST",
                    data: data,
                    success: function (response) {
                        if (response.isSuccess) {
                            loadForm(saveUrl)
                                .then((data) => {
                                    $(settings.saveSelector).attr("disabled", "disabled");
                                    loadPatients(settings.baseUrl, settings.gridSelector);
                                    /*settings.load(settings.baseUrl, settings.gridSelector);*/
                                })
                                .catch((error) => {
                                    console.log(error)
                                })

                            toastr.success(response.message, 'Success');
                        }
                        else {
                            toastr.error(response.message, 'Error');
                            console.log(response);
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
                            if (response.success) {
                                toastr.success(response.message, 'Success');
                                selectedItems = [];
                                /*settings.load(settings.baseUrl);*/
                                loadPatients(settings.baseUrl, settings.gridSelector);
                                loadForm(saveUrl);
                            }
                            else {
                                toastr.error(response.message, 'Error');
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

            $("body").on('click', '.select-item', function () {
                $('input[class="select-item"]').not(this).prop('checked', false);
                if ($(this).is(":checked")) {

                    let url = settings.baseUrl + $(this).data("url");
                    loadForm(url);

                    $(settings.saveSelector).removeAttr("disabled");
                    $("html, body").animate({ scrollTop: 500 }, 500);
                }
            });

            $("body").on("click", "input:checkbox.checkBox", function (e) {
                if ($(this).prop('checked')) {
                    if (!selectedItems.includes($(this).val())) {
                        selectedItems.push($(this).val());
                    }
                } else {
                    selectedItems.splice($.inArray($(this).val(), selectedItems), 1);;
                }
                console.log(selectedItems);
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
                    $(settings.quickAddModal + " .modal-body #header").hide()
                    $(settings.quickAddModal + " .modal-body #left_menu").hide()
                    $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")

                    $("body").removeClass("sidebar-mini");
                })
            });

            $("body").on("click", ".js-modal-dismiss", function () {
                $("body").removeClass("sidebar-mini").addClass("sidebar-mini");
                $(settings.quickAddModal + " .modal-body #header").show()
                $(settings.quickAddModal + " .modal-body #left_menu").show()

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

            $("body").on("click", ".js-discharge-export", function () {
                if (selectedItems.length > 0) {
                    var self = $(this);
                    let reportRenderType = self.data("rendertype");

                    $(selectedItems).each(function (index, item) {
                        window.open(
                            settings.baseUrl + `/Export?DischargeId=${item}&reportType=Discharge&reportRenderType=${reportRenderType}`,
                            "_blank"
                        )
                    })
                } else {
                    toastr.info("Please select at least one item.", 'Warning');
                }
            });
          
            $("body").on("click", ".js-discharge-preview", function () {
                if (selectedItems.length > 0) {
                    var self = $(this);
                    let reportRenderType = self.data("rendertype") ?? "PDF";
                    $(selectedItems).each(function (index, item) {
                        $.ajax({
                            url: settings.baseUrl + "/Export",
                            method: "POST",
                            data: {
                                DischargeId: item,
                                ReportType: "Discharge",
                                ReportRenderType: reportRenderType,
                                IsPreview: true
                            },
                            success: function (response) {
                             ;
                                window.open(
                                    normalizeUrl(getBaseUrl()) + `/Preview/Discharge`,
                                    "_blank"
                                )
                                
                            }
                        });
                    })
                }
                else {
                    toastr.info("Please select at least one item.", 'Warning');
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

            $("body").on("change", ".patientType, .admissionType, .patient", function () {
                loadPatients(settings.baseUrl, settings.gridSelector);
            });

            $("body").on("datefrom", ".dateto", function () {
                loadDischarges(settings.baseUrl, "#discharge-grid");
            });

        });

        function loadPatients(baseUrl, gridSelector) {
            var data = {
                patientType: $(".patientType").val(),
                admissionType: $(".admissionType").val(),
                patient: $(".patient").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/AllPatients",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "patientCode", "className": "text-center", width: "20px", "render": function (data) {
                            return `<input class="select-item" type='checkbox' value='${data}' data-url="/setup?patientCode=${data}" />`;
                        }
                    },
                    {
                        "data": "patientCode", "className": "text-center", width: "100px",
                        render: function (data, type, row) {
                            return `<a class='js-discharge-edit' data-url="/setup?patientCode=${data}" href='#' title="Select ${row.patientName}" data-id='${data}'>${data}</a>`;
                        }
                    },
                    { "data": "patientName", "className": "text-left", width: "150px" },
                    { "data": "phone", "className": "text-center", width: "150px" },
                    {
                        "data": "registrationId", "className": "text-center", width: "100px"
                    },
                    { "data": "visitDate", "width": "150px" },
                    {
                        "data": "admissionType", "className": "text-left", width: "150px"
                    },
                    {
                        "data": "doctorName", "className": "text-left", width: "150px"
                    },
                    {
                        "data": "referencePerson", "className": "text-left", width: "150px"
                    }
                ],
                lengthChange: false,
                pageLength: 10,
                order: [],
                sScrollY: "100%",
                scrollX: true,
                sScrollX: "100%",
                bDestroy: true
            });   

            loadDischarges(settings.baseUrl, "#discharge-grid");
        }

        function loadDischarges(baseUrl, gridSelector) {
            var data = {
                fromDate: $(".datefrom").val(),
                toDate: $(".dateto").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/Discharges",
                    type: "GET",
                    datatype: "json",
                     data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "dischargeId", "className": "text-center", width: "30px",
                        render: function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "dischargeId", "className": "text-center", width: "130px"
                    },
                    { "data": "patientCode", "className": "text-center", width: "80px" },
                    { "data": "patientName", "className": "text-left", width: "150px" },
                    { "data": "phone", "width": "50px" },
                    { "data": "dischargeDateTime", "className": "text-center", width: "130px" },
                    { "data": "dischargeTypeName", "className": "text-left", width: "130px" },
                    { "data": "finalPayment", "className": "text-center", width: "130px" },
                    { "data": "isPaid", "className": "text-center", width: "130px" },
                    { "data": "remarks", "className": "text-center", width: "130px" },
                
                    {
                        "data": "dischargeId", "render": function (data, type, row) {
                            return `<div class='action-buttons p-1'>
                                        <a class='btn btn-success btn-circle btn-sm js-discharge-edit' data-url="/setup/${data}" title="Edit ${row.patientName}" data-id='${data}'><i class='fas fa-pencil-alt'></i></a> 
                                        <button type="button" class="btn btn-danger btn-circle btn-sm js-discharge-delete-confirm"
                                                data-target="#deleteModalx"
                                                data-id="${data}"
                                                title="Delete ${row.patientName}"
                                                data-title="Are you sure want to delete ${row.patientName}?">
                                                    <i class="fas fa-trash fa-sm"></i>
                                        </button>`;
                        },
                        "orderable": false,
                        "searchable": false,
                        width: "100px"
                    }
                ],
                lengthChange: false,
                pageLength: 10,
                order: [],
                sScrollY: "100%",
                scrollX: true,
                sScrollX: "100%",
                bDestroy: true
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

        function initialize(selectedText = '', selectedValue = '') {
            $('.selectpicker').select2({
                language: {
                    noResults: function () {
                        //return 'Not found <a class="add_new_item" href="javascript:void(0)">Add New</a>';
                    }
                },
                dropdownAutoWidth: true,

                escapeMarkup: function (markup) {
                    return markup;
                }
            });

            $('#Medicine_MedicineCode').select2({
                placeholder: "Select Medicine",
                dropdownAutoWidth: true,
                minimumInputLength: 0,
                allowClear: true,
                ajax: {
                    url: normalizeUrl(getBaseUrl()) + "/Cascading/GetPagedMedicines",
                    dataType: 'json',
                    type: 'POST',
                    data: function (params) {
                        var query = {
                            match: params.term,
                            page: params.page || 1,
                            pageSize: params.pageSize || 20
                        }
                        return query;
                    },
                    processResults: function (data, params) {
                        return {
                            results: data.items,
                            page: data.page,
                            pagination: {
                                more: (data.page * data.pageSize) < data.total_count
                            }
                        }
                    },
                    formatSelection: function (element) {
                        return element.text + ' (' + element.id + ')';
                    },
                    escapeMarkup: function (m) {
                        return m;
                    }
                },
            });

            // Fetch the preselected item, and add to the control
            var medicineSelect = $('#Medicine_MedicineCode');
            var option = new Option(selectedText, selectedValue, true, true);
            medicineSelect.append(option).trigger('change');

            medicineSelect.trigger({
                type: 'select2:select',
                params: {
                    data: option
                }
            });


            // on first focus (bubbles up to document), open the menu
            $(document).on('focus', '.select2-selection.select2-selection--single', function (e) {
                $(this).closest(".select2-container").siblings('select:enabled').select2('open');
            });

            // steal focus during close - only capture once and stop propogation
            $('select.select2').on('select2:closing', function (e) {
                $(e.target).data("select2").$selection.one('focus focusin', function (e) {
                    e.stopPropagation();
                });
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

            $('.filterdatetimepicker').datetimepicker({
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
            }).on("dp.change", function (e) {
                loadDischarges(settings.baseUrl, "#discharge-grid");
            });
        }
    }
}(jQuery));

