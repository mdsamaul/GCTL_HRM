(function ($) {
    $.outPatientPrescriptions = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#outpatientprescription-form",
            formContainer: ".js-outpatientprescription-form-container",
            gridSelector: "#patients-grid",
            gridContainer: ".js-outpatientprescription-grid-container",
            editSelector: ".js-outpatientprescription-edit",
            saveSelector: ".js-outpatientprescription-save",
            selectAllSelector: "#outpatientprescription-check-all",
            deleteSelector: ".js-outpatientprescription-delete-confirm",
            deleteModal: "#outpatientprescription-delete-modal",
            finalDeleteSelector: ".js-outpatientprescription-delete",
            clearSelector: ".js-outpatientprescription-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-outpatientprescription-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-outpatientprescription-check-availability",
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
            $(settings.saveSelector).attr("disabled", "disabled");
            //settings.load(settings.baseUrl, settings.gridSelector);

            var d = new Date();
            var month = d.getMonth() + 1;
            var day = d.getDate();

            var output = (day < 10 ? '0' : '') + day + '/' +
                (month < 10 ? '0' : '') + month + '/' + d.getFullYear();
            $('#txtGridFromDate').val('01' + '/' + (month < 10 ? '0' : '') + month + '/' + d.getFullYear());
            $('#txtGridToDate').val(output);

            $('#FromDate').val('01' + '/' + (month < 10 ? '0' : '') + month + '/' + d.getFullYear());
            $('#ToDate').val(output);
           
            initialize();
            loadPatients(settings.baseUrl, settings.gridSelector);
            loadPrescriptions(settings.baseUrl, "#prescriptions-grid");
            $("body").on("click", `${settings.editSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = settings.baseUrl + $(this).data("url");
                // let url = saveUrl + "/" + $(this).data("id") ?? "";

                loadForm(url);

                $(settings.saveSelector).removeAttr("disabled");
                $("html, body").animate({ scrollTop: 500 }, 500);
            });

            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
                $(settings.saveSelector).attr("disabled", "disabled");
                loadPatients(settings.baseUrl, settings.gridSelector);
                loadPrescriptions(settings.baseUrl, "#prescriptions-grid");
                loadForm(saveUrl);
                initialize();
            });

            // Save
            $("body").on("click", settings.saveSelector, function () {
                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }

                var complains = $("#complain-grid tbody tr").length;
                var medicines = $("#medicine-grid tbody tr").length;
                var diagnoses = $("#diagnosis-grid tbody tr").length;

                if (complains == 0 && medicines == 0 && diagnoses == 0) {
                    toastr.info("Please add at least one of 3 items (Complaints, Medicines, Diagnosis)!", 'Info');
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
                                    $(settings.saveSelector).removeAttr("disabled");
                                    loadPatients(settings.baseUrl, settings.gridSelector);
                                    loadPrescriptions(settings.baseUrl, "#prescriptions-grid");
                                    $(settings.saveSelector).fadeOut();
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
                                loadPrescriptions(settings.baseUrl, "#prescriptions-grid");
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
                $("html, body").animate({ scrollTop: 500 }, 500);
            });

            $("body").on('click', '.select-item', function () {
                $('input[class="select-item"]').not(this).prop('checked', false);
                if ($(this).is(":checked")) {
                    console.log($(this).val());

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

            $("body").on("click", "#complain-grid .js-add", function () {
                if ($("#Complaint_ComplaintName").val() == '') {
                    $("#Complaint_ComplaintNameError").addClass("d-none")
                        .removeClass("d-none");
                    $("#Complaint_ComplaintName").focus();
                    return;
                }
                else {
                    $("#Complaint_ComplaintNameError").removeClass("d-none")
                        .addClass("d-none");
                }

                let complaintName = $("#Complaint_ComplaintName").val(),
                    complaintCode = $("#Complaint_ComplaintCode").val(),
                    complaintSerial = $("#Complaint_SerialNo").val(),
                    counter = $("#complain-grid tbody tr").length;

                if (complaintSerial.length != "") {
                    $("#complaint-" + complaintSerial + " .item-title").html(complaintName);
                    $("#complaint-" + complaintSerial + " .ComplaintNameTitle").html(complaintName);
                    $("#complaint-" + complaintSerial + " .ComplaintName").val(complaintName);
                    $("#complaint-" + complaintSerial + " .ComplaintCode").val(complaintCode);
                } else {
                    let item =
                        `<tr id="complaint-${counter}">
                        <td>
                            <span class="ComplaintNameTitle">${complaintName}</span>
                            <input type="hidden" id="Complaints_${counter}__ComplaintName" name="Complaints[${counter}].ComplaintName" value="${complaintName}" class="ComplaintName" />
                            <input type="hidden" id="Complaints_${counter}__ComplaintCode" name="Complaints[${counter}].ComplaintCode" value="${complaintCode}" class="ComplaintCode" />
                            <input type="hidden" id="Complaints_${counter}__SerialNo" name="Complaints[${counter}].SerialNo" value="${counter}" class="SerialNo" />
                        </td>
                        <td>
                            <button type="button" class="btn btn-sm btn-success js-edit" title="Edit complaint"><i class="fas fa-pencil-alt"></i></button>
                            <button type='button' class='btn btn-sm btn-danger js-remove' title="Remove complaint"><i class='fa fa-trash'></i></button>
                        </td>
                        </tr>`;

                    $("#complain-grid tbody").append(item);
                    counter++;
                }

                $("#complain-grid .js-add").html('<i class="fas fa-plus">');
                $("#Complaint_ComplaintName").val("");
                $("#Complaint_SerialNo").val("");
            });

            $("body").on("click", "#complain-grid .js-edit", function (e) {
                e.preventDefault();
                $("#Complaint_ComplaintName").val($(this).closest("tr").find(".ComplaintName").val());
                $("#Complaint_ComplaintCode").val($(this).closest("tr").find(".ComplaintCode").val());
                $("#Complaint_SerialNo").val($(this).closest("tr").find(".SerialNo").val());
                $("#complain-grid .js-add").html('<i class="fas fa-pencil-alt">');
            })

            $("body").on("click", "#complain-grid .js-remove", function (e) {
                e.preventDefault();
                remove($(this));

                fixComplainIndexing();
            })


            function fixComplainIndexing() {
                $("#complain-grid tbody tr").each(function (index) {
                    var html = $(this).html();
                    html = html.replace(/\_(.*?)__/g, '_' + index + '__');
                    html = html.replace(/\[(.*?)\]/g, '[' + index + ']');
                    $(this).html(html);
                })
            }


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


            function fixMedicineIndexing() {
                $("#medicine-grid tbody tr").each(function (index) {
                    var html = $(this).html();
                    html = html.replace(/\_(.*?)__/g, '_' + index + '__');
                    html = html.replace(/\[(.*?)\]/g, '[' + index + ']');
                    $(this).html(html);
                })
            }


            $("body").on("click", "#diagnosis-grid .js-add", function () {
                if ($("#Diagnosis_TestChargeCode").val() == '') {
                    $("#Diagnosis_TestChargeCodeError").addClass("d-none")
                        .removeClass("d-none");
                    $("#Diagnosis_TestChargeCode").focus();
                    return;
                }
                else {
                    $("#Diagnosis_TestChargeCodeError").removeClass("d-none")
                        .addClass("d-none");
                }

                let testName = $("#Diagnosis_TestChargeCode option:selected").text(),
                    diagnosisSerial = $("#Diagnosis_SerialNo").val(),
                    diagnosisCode = $("#Diagnosis_PrescriptionDiagnosisCode").val(),
                    testCode = $("#Diagnosis_TestChargeCode").val(),
                    diagnosisInstruction = $("#Diagnosis_Instruction").val(),
                    counter = $("#diagnosis-grid tbody tr").length;


                if (diagnosisSerial != "") {
                    $("#diagnosis-" + diagnosisSerial + " .test-title").html(testName);
                    $("#diagnosis-" + diagnosisSerial + " .SerialNo").val(diagnosisSerial);
                    $("#diagnosis-" + diagnosisSerial + " .TestChargeCode").val(testCode);

                    $("#diagnosis-" + diagnosisSerial + " .test-instruction").html(diagnosisInstruction);
                    $("#diagnosis-" + diagnosisSerial + " .Instruction").val(diagnosisInstruction);
                } else {
                    let item =
                        `<tr id="diagnosis-${counter}">
                        <td>
                            <span class="test-title">${testName}</span>
                            <input type="hidden" id="Diagnoses_${counter}__TestChargeCode" name="Diagnoses[${counter}].TestChargeCode" value="${testCode}" class="TestChargeCode" />
                            <input type="hidden" id="Diagnoses_${counter}__PrescriptionDiagnosisCode" name="Diagnoses[${counter}].PrescriptionDiagnosisCode" value="${diagnosisCode}" class="PrescriptionDiagnosisCode" />
                            <input type="hidden" id="Diagnoses_${counter}__SerialNo" name="Diagnoses[${counter}].SerialNo" value="${counter}" class="SerialNo" />
                        </td>
                        <td>
                            <span class="test-instruction">${diagnosisInstruction}</span>
                            <input type="hidden" id="Diagnoses_${counter}__Instruction" name="Diagnoses[${counter}].Instruction" value="${diagnosisInstruction}" class="Instruction" />
                        </td>
                       <td>
                            <button type="button" class="btn btn-sm btn-success js-edit" title="Edit diagnosis"><i class="fa fa-pencil-alt"></i></button>
                            <button type='button' class='btn btn-sm btn-danger js-remove' title="Remove diagnosis"><i class='fa fa-trash'></i></button>
                        </td>
                    </tr>`;

                    $("#diagnosis-grid tbody").append(item);
                    counter++;
                }
                $("#Diagnosis_TestChargeCode").val("");
                initialize();
                $("#Diagnosis_Instruction").val("");
                $("#Diagnosis_SerialNo").val("");
                $("#diagnosis-grid .js-add").html('<i class="fas fa-plus"></i>');
            });


            $("body").on("click", "#diagnosis-grid .js-edit", function (e) {
                e.preventDefault();
                $("#Diagnosis_TestChargeCode").val($(this).closest("tr").find(".TestChargeCode").val());
                $("#Diagnosis_Instruction").val($(this).closest("tr").find(".Instruction").val());
                $("#Diagnosis_PrescriptionDiagnosisCode").val($(this).closest("tr").find(".PrescriptionDiagnosisCode").val());
                $("#Diagnosis_SerialNo").val($(this).closest("tr").find(".SerialNo").val());
                $("#diagnosis-grid .js-add").html('<i class="fas fa-pencil-alt"></i>');
                initialize();
            })


            $("body").on("click", "#diagnosis-grid .js-remove", function (e) {
                e.preventDefault();
                remove($(this));


                fixDiagnosisIndexing();
            })


            function fixDiagnosisIndexing() {
                $("#diagnosis-grid tbody tr").each(function (index) {
                    var html = $(this).html();
                    html = html.replace(/\_(.*?)__/g, '_' + index + '__');
                    html = html.replace(/\[(.*?)\]/g, '[' + index + ']');
                    $(this).html(html);
                })
            }


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


                $(settings.quickAddModal + " .modal-body").load(loadUrl, function (response) {
                    $(settings.quickAddModal).modal("show");

                    // $.sidebarMenu($('.sidebar-menu'));

                    $("#header").hide();
                    $(settings.quickAddModal + " .modal-body #header").hide();

                    $("#left_menu").hide();
                    $(settings.quickAddModal + " .modal-body #left_menu").hide();

                    $("#main-content").toggleClass("collapse-main");
                    $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main");

                    /*  $("body").removeClass("sidebar-mini");*/
                })
            });

            $("body").on("click", ".js-modal-dismiss", function () {
                /*       $("body").removeClass("sidebar-mini").addClass("sidebar-mini");*/

                $("#header").show();
                $(settings.quickAddModal + " .modal-body #header").show();

                $("#left_menu").show();
                $(settings.quickAddModal + " .modal-body #left_menu").show();

                $("#main-content").toggleClass("collapse-main");
                $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main");

                // $.sidebarMenu($('.sidebar-menu'));

                lastCode = $(settings.quickAddModal + " #lastCode").val();


                $(settings.quickAddModal + " .modal-body").empty();
                $(settings.quickAddModal).modal("hide");


                $(target).empty("");
                $(target).append($('<option>', {
                    value: '',
                    text: `---Select ${title}---`
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

            $("body").on("change", "#TestCategoryCode", function () {
                var self = $(this);
                if (self.val().length > 0) {
                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetTestSubCategories",
                        method: "POST",
                        data: { testCategoryCode: self.val() },
                        success: function (response) {
                            $("#TestSubCategoryCode").empty();
                            $("#TestSubCategoryCode").append($('<option>', {
                                value: '',
                                text: `---Sub Category---`
                            }));
                            $.each(response, function (i, item) {
                                $("#TestSubCategoryCode").append($('<option>', {
                                    value: item.code,
                                    text: item.name
                                }));
                            });

                            getTests(self.val(), $("#TestSubCategoryCode").val());
                        }
                    });
                }
            });

            $("body").on("change", "#TestSubCategoryCode", function () {
                var self = $(this);

                if (self.val().length > 0) {
                    getTests($("#TestCategoryCode").val(), self.val());
                }
            });

            $("body").on("keyup", "#VisitingFeeDiscount", function () {
                calculateDiscountAmount("Visiting");
            });

            $("body").on("keyup", "#ReportShowingFeeDiscount", function () {
                calculateDiscountAmount("ReportShowing");
            });

            $("body").on("keyup", "#VisitingFeeDiscountAmount", function () {
                $("#VisitingFeeDiscount").val(0);
                calculateDiscount("Visiting");
            });

            $("body").on("keyup", "#ReportShowingFeeDiscountAmount", function () {
                $("#ReportShowingFeeDiscount").val(0);
                calculateDiscount("ReportShowing");
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

      
            
            $("body").on("click", ".js-prescription-patientGridPreivew", function (e) {
              
                e.preventDefault();
                loadPatients(settings.baseUrl, settings.gridSelector);
            })
            $("body").on("click", ".js-prescription-GridPreivew", function (e) { 
                e.preventDefault(); 
                loadPrescriptions(settings.baseUrl, "#prescriptions-grid");
            })
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

            $("body").on("click", ".js-prescription-exportBlank", function () {
                if (selectedItems.length > 0) {
                    var self = $(this);
                    let reportRenderType = self.data("rendertype");

                    $(selectedItems).each(function (index, item) {
                        window.open(
                            settings.baseUrl + `/ExportBlank?prescriptionCode=${item}&reportType=Prescription&reportRenderType=${reportRenderType}`,
                            "_blank"
                        )
                    })
                } else {
                    toastr.info("Please select at least one item.", 'Warning');
                }
            });

            $("body").on("click", ".js-prescription-export", function () {
                if (selectedItems.length > 0) {
                    var self = $(this);
                    let reportRenderType = self.data("rendertype");
                  
                    $(selectedItems).each(function (index, item) {
                        window.open(
                            settings.baseUrl + `/Export?prescriptionCode=${item}&reportType=Prescription&reportRenderType=${reportRenderType}`,
                            "_blank"
                        )
                    })
                } else {
                    toastr.info("Please select at least one item.", 'Warning');
                }
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

        function getTests(testCategoryCode , testSubCategoryCode) {
            $.ajax({
                url: normalizeUrl(getBaseUrl()) + "/Cascading/GetTests",
                method: "POST",
                data: { testCategoryCode: testCategoryCode, testSubCategoryCode: testSubCategoryCode },
                success: function (response) {
                    console.log(response);
                    $("#Diagnosis_TestChargeCode").empty();
                    $("#Diagnosis_TestChargeCode").append($('<option>', {
                        value: '',
                        text: `---Select Test---`
                    }));
                    $.each(response, function (i, item) {
                        $("#Diagnosis_TestChargeCode").append($('<option>', {
                            value: item.code,
                            text: item.name
                        }));
                    });
                }
            });
        }

        function calculateDiscountAmount(selectorPrefix) {
            let feeSelector = $("#" + selectorPrefix + "Fee"),
                discountSelector = $("#" + selectorPrefix + "FeeDiscount"),
                discountAmountSelector = $("#" + selectorPrefix + "FeeDiscountAmount"),
                fee = parseFloat(feeSelector.val() || 0),
                discount = parseFloat(discountSelector.val() || 0),
                discountAmount = parseFloat(discountAmountSelector.val() || 0);

            if (fee > 0) {
                discountAmount = (fee * discount) / 100;
            }

            discountAmountSelector.val(discountAmount);
            calculateDiscount(selectorPrefix);
        }

        function calculateDiscount(selectorPrefix) {
            let feeSelector = $("#" + selectorPrefix + "Fee"),
                discountAmountSelector = $("#" + selectorPrefix + "FeeDiscountAmount"),
                amountSelector = $("#" + selectorPrefix + "Amount"),
                fee = parseFloat(feeSelector.val() || 0),
                discountAmount = parseFloat(discountAmountSelector.val() || 0),
                amount = parseFloat(amountSelector.val() || 0);

            if (fee > 0) {
                amount = fee - discountAmount;
            }

            amountSelector.val(amount);
        }

        function loadPatients(baseUrl, gridSelector) {
            var data = {
                fromDate: $(".datefrom").val(),
                toDate: $(".dateto").val(),
                doctorCode: $(".doctor").val(),
                patientTypeCode: $(".patientTypeCode").val(),
                patientCategoryCode: $(".patientcategory").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/OutPatients",
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
                            return `<a class='js-outpatientprescription-edit' data-url="/setup?patientCode=${data}" href='#' title="Select ${row.patientName}" data-id='${data}'>${data}</a>`;
                        }
                    },
                    { "data": "patientName", "className": "text-left", width: "150px" },
                    {
                        "data": "registrationId", "className": "text-center", width: "100px",
                        render: function (data, type, row) {
                            return `<a class='js-outpatientprescription-edit' data-url="/setup?patientCode=${row.patientCode}" href='#' title="Select ${row.patientName}" data-id='${row.patientCode}'>${data}</a>`;
                        }
                    },
                    { "data": "registrationDate", "width": "150px" },
                    { "data": "patientCategoryName", "className": "text-center", width: "60px" },
                    {
                        "data": "doctorName", "className": "text-left", width: "150px"
                    },
                    { "data": "visitingFee", "className": "text-center", width: "100px" },
                    { "data": "visitDate", "className": "text-center", width: "150px" }
                ],
                lengthChange: false,
                pageLength: 8,
                order: [],
                sScrollY: "100%",
                scrollX: true,
                sScrollX: "100%",
                bDestroy: true
            });

            
        }

        function loadPrescriptions(baseUrl, gridSelector) {
       
            var data = {
                gridFromDate: $(".gridFromDate").val(),
                gridToDate: $(".gridToDate").val(),
                doctorCode: $(".doctor").val(),
                patientCategoryCode: $(".patientcategory").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/Prescriptions",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "prescriptionCode", "className": "text-center", width: "30px",
                        render: function (data) {
                            return `<input type="checkbox" class="checkBox" value='${data}' />`;
                        }
                    },
                    {
                        "data": "prescriptionCode", "className": "text-center", width: "130px"
                    },
                    { "data": "prescriptionDate", "className": "text-center", width: "130px" },
                    { "data": "patientCode", "className": "text-center", width: "80px" },
                    { "data": "patientName", "className": "text-center", width: "150px" },
                    { "data": "fullAge", "width": "100px" },
                    { "data": "sex", "className": "text-center", width: "50px" },
                    {
                        "data": "doctorName", "className": "text-center", width: "130px"
                    },
                    { "data": "referencePerson", "className": "text-center", width: "130px" },
                    { "data": "entryUser", "className": "text-center", width: "130px" },
                    {
                        "data": "prescriptionCode", "render": function (data, type, row) {
                            return `<div class='p-1 action-buttons'>
                                        <a class='btn btn-warning btn-circle btn-sm js-outpatientprescription-edit' data-url="/setup/${data}" title="Edit ${row.patientName}" data-id='${data}'><i class='fas fa-pencil-alt'></i></a> 
                                        <button type="button" class="btn btn-danger btn-circle btn-sm js-outpatientprescription-delete-confirm"
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
            // create the option and append to Select2
            var option = new Option(selectedText, selectedValue, true, true);
            medicineSelect.append(option).trigger('change');

            // manually trigger the `select2:select` event
            medicineSelect.trigger({
                type: 'select2:select',
                params: {
                    data: option
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

