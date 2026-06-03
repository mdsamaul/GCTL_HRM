(function ($) {
    $.labTestReferenceSetups = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#labTestReferenceSetup-form",
            formContainer: ".js-labTestReferenceSetup-form-container",
            gridSelector: "#references-grid",
            gridContainer: ".js-labTestReferenceSetup-grid-container",
            editSelector: ".js-labTestReferenceSetup-edit",
            saveSelector: ".js-labTestReferenceSetup-save",
            selectAllSelector: "#labTestReferenceSetup-check-all",
            deleteSelector: ".js-labTestReferenceSetup-delete-confirm",
            deleteModal: "#labTestReferenceSetup-delete-modal",
            finalDeleteSelector: ".js-labTestReferenceSetup-delete",
            clearSelector: ".js-labTestReferenceSetup-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-labTestReferenceSetup-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-labTestReferenceSetup-check-availability",
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
            loadLabTestReferenceSetups(settings.baseUrl, settings.gridSelector);
            initialize();
            // $(".DetailReceiver").trigger("change");
            $("body").on("click", `${settings.editSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = settings.baseUrl + $(this).data("url");
                // let url = saveUrl + "/" + $(this).data("id") ?? "";

                loadForm(url)
                    .then((data) => {
                        /* $(".DetailReceiver").trigger("change");*/

                        var counter = $("#reference-value-grid tbody tr").length;
                        if (counter == 1) {
                            $("#reference-value-grid tbody tr .js-edit").trigger("click");
                        }
                    })
                    .catch((error) => {
                        console.log(error)
                    })
                //  $(settings.saveSelector).removeAttr("disabled");
                $("html, body").animate({ scrollTop: 500 }, 500);
            });

            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
                //  $(settings.saveSelector).attr("disabled", "disabled");
                loadLabTestReferenceSetups(settings.baseUrl, settings.gridSelector);
                loadForm(saveUrl);
                initialize();
            });

            // Save
            $("body").on("click", settings.saveSelector, function () {
                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }


                if ($("#reference-value-grid tbody tr").length == 0) {
                    toastr.info("Nothing to save", 'Info');
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
                            //loadForm(saveUrl)
                            //    .then((data) => {
                            //        if (response.secondaryMessage != undefined) {
                            //            let warningTemplate = `<div class="alertx alert-warningx js-error"><p class="text-danger m-0"><strong><em>${response.secondaryMessage}</em></strong></p></div>`;
                            //            $(".js-message").html(warningTemplate);

                            //            setTimeout(function () {
                            //                $(".js-error").fadeOut();
                            //            }, 5000);
                            //        }
                            //        loadLabTestReferenceSetups(settings.baseUrl, settings.gridSelector);


                            //    })
                            //    .catch((error) => {
                            //        console.log(error)
                            //    })
                            generateCode();
                            $("#TestChargeId").val("");
                            initialize();
                            loadLabTestReferenceSetups(settings.baseUrl, settings.gridSelector);
                            toastr.success(response.message, 'Success');

                        }


                        else {
                            if (response.secondaryMessage != undefined) {
                                let warningTemplate = `<div class="alertx alert-warningx js-error"><p class="text-danger m-0"><strong><em>${response.secondaryMessage}</em></strong></p></div>`;
                                $(".js-message").html(warningTemplate);

                                setTimeout(function () {
                                    $(".js-error").fadeOut();
                                }, 5000);
                            }
                            toastr.error(response.message, 'Error');
                            console.log(response);
                        }
                    }
                }
                if (settings.haseFile) {
                    options.processData = false;
                    options.contentType = false;
                }

              
                var preventForward = false;
                // Check existing entry
                $.ajax({
                    url: settings.baseUrl + "/CheckAvailability",
                    method: "POST",
                    data: { id: 0, name: "" },
                    success: function (response) {
                        if (response.isSuccess) {
                            let warningTemplate = `<div class="alert alert-warning js-error"><p class="m-0"><strong><em>${response.message}</em></strong></p></div>`;
                            $(".js-message").html(warningTemplate);
                            preventForward = response.isSuccess;
                            alert(preventForward);
                            return false;
                        } else {

                            if (preventForward)
                                return false;
                            else
                                $.ajax(options);
                        }
                    }
                });
                
              
            });
            function generateCode() {
                $.ajax({
                    url: settings.baseUrl + "/GetNextCode",
                    method: "POST",
                    success: function (response) {
                        if (response.isSuccess)
                            $("#LabTestReferenceId").val(response.message);
                        else
                            toastr.error(response.message);
                    }
                });
            }
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
                                loadLabTestReferenceSetups(settings.baseUrl, settings.gridSelector);
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

            $("body").on('change', '.DetailReceiver', function () {
                $(".receiveroptions").removeClass("d-none")
                    .addClass("d-none");

                var target = $(this).data("target");
                $(target).addClass("d-none")
                    .removeClass("d-none");
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


            $("body").on("click", "#reference-value-grid .js-add", function () {

                //if ($("#Detail_PersonId").val() == '') {
                //    $("#Detail_PersonIdError").addClass("d-none")
                //        .removeClass("d-none");
                //    $("#Detail_PersonId").focus();
                //    return;
                //}
                //else {
                //    $("#Detail_PersonIdError").removeClass("d-none")
                //        .addClass("d-none");
                //}
                var SDPersonName = "";
                if ($("#Detail_PersonId").val()  != '') {
                    SDPersonName = $("#Detail_PersonId option:selected").text();
                }
                var SDPeriodName = "";
                if ($("#Detail_PeriodId").val() != '') {
                    SDPeriodName = $("#Detail_PeriodId option:selected").text();
                }
                
                var SDUnitName = "";
                if ($("#Detail_UnitId").val() != '') {
                    SDUnitName = $("#Detail_UnitId option:selected").text();
                }


                if ($("#Detail_NormalValueText").val() == '') {
                    if ($("#Detail_RangeFrom").val() == '') {
                        if ($("#Detail_RangeTo").val() == '') {
                            toastr.error("Please Select Normal Value or Normal Value (Text)");
                            return;
                        }
                    }                   
                }
                

                //if ($("#Detail_UnitId").val() == '') {
                //    $("#Detail_UnitIdError").addClass("d-none")
                //        .removeClass("d-none");
                //    $("#Detail_UnitId").focus();
                //    return;
                //}
                //else {
                //    $("#Detail_UnitIdError").removeClass("d-none")
                //        .addClass("d-none");
                //}

                let referenceSerial = $("#Detail_SerialNo").val(),
                    labTestReferenceSetupId = $("#Detail_LabTestReferenceSetupId").val(),
                    person = $("#Detail_PersonId").val(),
                    personName = SDPersonName ,
                    ageFrom = $("#Detail_AgeFrom").val(),
                    ageTo = $("#Detail_AgeTo").val(),
                    period = $("#Detail_PeriodId").val(),
                    periodName = SDPeriodName,
                    rangeFrom = $("#Detail_RangeFrom").val(),
                    rangeTo = $("#Detail_RangeTo").val(),
                    normalValueText = $("#Detail_NormalValueText").val(),
                    normalValueTextHtml = $("#Detail_NormalValueText").html(),
                    unit = $("#Detail_UnitId").val(),
                    unitName = SDUnitName,
                    counter = $("#reference-value-grid tbody tr").length;

                //var persons = "";
                //$("#Detail_PersonId > option").each(function () {
                //    var item = $(this).html();
                //    var value = $(this).val();

                //    if (value == person) {
                //        persons += `<option value='${value}' selected='selected'>${item}</option>`;
                //    } else {
                //        persons += `<option value='${value}'>${item}</option>`;
                //    }
                //})

                //var periods = "";
                //$("#Detail_PeriodId > option").each(function () {
                //    var item = $(this).html();
                //    var value = $(this).val();

                //    if (period.includes(value)) {
                //        periods += `<option value='${value}' selected='selected'>${item}</option>`;
                //    } else {
                //        periods += `<option value='${value}'>${item}</option>`;
                //    }
                //})

                //var units = "";
                //$("#Detail_UnitId > option").each(function () {
                //    var item = $(this).html();
                //    var value = $(this).val();

                //    if (unit.includes(value)) {
                //        units += `<option value='${value}' selected='selected'>${item}</option>`;
                //    } else {
                //        units += `<option value='${value}'>${item}</option>`;
                //    }
                //})

                if (referenceSerial != "") {
                    $("#reference-value-" + referenceSerial + " .PersonName").html(personName);
                    $("#reference-value-" + referenceSerial + " .PersonId").val(person);
                    $("#reference-value-" + referenceSerial + " .SerialNo").val(referenceSerial);

                    $("#reference-value-" + referenceSerial + " .AgeFromTitle").html(ageFrom);
                    $("#reference-value-" + referenceSerial + " .AgeFrom").val(ageFrom);

                    $("#reference-value-" + referenceSerial + " .AgeToTitle").html(ageTo);
                    $("#reference-value-" + referenceSerial + " .AgeTo").val(ageTo);

                    $("#reference-value-" + referenceSerial + " .PeriodName").html(periodName);
                    $("#reference-value-" + referenceSerial + " .PeriodId").val(period);

                    $("#reference-value-" + referenceSerial + " .RangeFromTitle").html(rangeFrom);
                    $("#reference-value-" + referenceSerial + " .RangeFrom").val(rangeFrom);

                    $("#reference-value-" + referenceSerial + " .RangeToTitle").html(rangeTo);
                    $("#reference-value-" + referenceSerial + " .RangeTo").val(rangeTo);

                    $("#reference-value-" + referenceSerial + " .NormalValueTextTitle").html(normalValueText);
                    $("#reference-value-" + referenceSerial + " .NormalValueText").val(normalValueText);

                    $("#reference-value-" + referenceSerial + " .UnitName").html(unitName);
                    $("#reference-value-" + referenceSerial + " .UnitId").val(unit);
                } else {
                    let item =
                        `<tr id="reference-value-${counter}">
                        <td class="text-middle">
                            <span class="Person">
                                ${personName}
                            </span>
                            <input type="hidden" id="Details_${counter}__PersonId" name="Details[${counter}].PersonId" value="${person}" />                           
                            <input type="hidden" id="Details_${counter}__LabTestReferenceSetupId" name="Details[${counter}].LabTestReferenceSetupId" value="${labTestReferenceSetupId}" class="LabTestReferenceSetupId" />
                            <input type="hidden" id="Details_${counter}__SerialNo" name="Details[${counter}].SerialNo" value="${counter}" class="SerialNo" />
                        </td>
                        <td class="text-middle">         
                            <span class="AgeFrom">
                                ${ageFrom}
                            </span>
                            <input type="hidden" id="Details_${counter}__AgeFrom" name="Details[${counter}].AgeFrom" value="${ageFrom}" />
                        </td>
                        <td class="text-center text-middle" style="width:1% !important;">-</td>
                        <td class="text-middle">
                            <span class="AgeTo">
                                ${ageTo}
                            </span>
                            <input type="hidden" id="Details_${counter}__AgeTo" name="Details[${counter}].AgeTo" value="${ageTo}" />
                        </td>
                        <td class="text-middle">
                             <span class="Period">
                                ${periodName}
                            </span>
                             <input type="hidden" id="Details_${counter}__PeriodId" name="Details[${counter}].PeriodId" value="${period}" />
                        </td>
                        <td class="text-middle">
                            <span class="RangeFrom">
                                ${rangeFrom}
                            </span>
                            <input type="hidden" id="Details_${counter}__RangeFrom" name="Details[${counter}].RangeFrom" value="${rangeFrom}" />
                        </td>
                        <td class="text-center text-middle" style="width:1% !important;">-</td>
                        <td class="text-middle">
                            <span class="RangeTo">
                                ${rangeTo}
                            </span>
                             <input type="hidden" id="Details_${counter}__RangeTo" name="Details[${counter}].RangeTo" value="${rangeTo}" />
                        </td>  
                        <td class="text-middle">
 <span class="normalValueText">
                                ${normalValueText}
                            </span>
                             <input type="hidden" id="Details_${counter}__NormalValueTexts" name="Details[${counter}].normalValueText" value="${normalValueText}" />

                           
                        </td>
                        <td class="text-center text-middle">
                            <span class="UnitId">
                                ${unitName}
                            </span>
                             <input type="hidden" id="Details_${counter}__UnitId" name="Details[${counter}].UnitId" value="${unit}" />
                        </td>
                       <td class="text-middle"> 
                            <button type="button" class="btn btn-sm btn-success js-edit" title="Edit details"><i class="fa fa-pencil-alt"></i></button>
                            <button type='button' class='btn btn-sm btn-danger js-remove' title="Remove"><i class='fa fa-trash'></i></button>
                        </td>
                    </tr>`;

                    $("#reference-value-grid tbody").append(item);
                    counter++;
                }

                $("#Detail_PersonId").val("");
                $("#Detail_AgeFrom").val("");
                refreshControl();
                $("#Detail_AgeTo").val("");
                $("#Detail_PeriodId").val("");
                $("#Detail_RangeFrom").val("");
                $("#Detail_RangeTo").val("");
                $("#Detail_NormalValueText").val("");
                $("#Detail_SerialNo").val("");
                $("#reference-value-grid .js-add").html('<i class="fas fa-plus"></i>');
            });

            //#reference - value - grid.js - edit"

            $("body").on("click", "#reference-value-grid .js-edit", function (e) {
                e.preventDefault();
                var UnitId = $(this).parent().prev().find("input[type=hidden]:first").val();
            
                var NormalValueText = $(this).parent().prev().prev().find("input[type=hidden]:first").val();
           
                var RangeTo = $(this).parent().prev().prev().prev().find("input[type=hidden]:first").val();
          
                var RangeFrom = $(this).parent().prev().prev().prev().prev().prev().find("input[type=hidden]:first").val();
             
                var PeriodId = $(this).parent().prev().prev().prev().prev().prev().prev().find("input[type=hidden]:first").val();
           
                var AgeTo = $(this).parent().prev().prev().prev().prev().prev().prev().prev().find("input[type=hidden]:first").val();
             
                var AgeFrom = $(this).parent().prev().prev().prev().prev().prev().prev().prev().prev()
                    .prev().find("input[type=hidden]:first").val();
               
                var PersonId = $(this).parent().prev().prev().prev().prev().prev().prev().prev().prev()
                    .prev().prev().find("input[type=hidden]:first").val();
      
                var LabTestReferenceSetupId = $(this).closest("tr").find(".LabTestReferenceSetupId").val();

                $("#Detail_LabTestReferenceSetupId").val(LabTestReferenceSetupId);
                var SerialNo = $(this).closest("tr").find(".SerialNo").val();

                $("#Detail_SerialNo").val(SerialNo);
                // var PersonId = $(this).closest("tr").find(".PersonId").val();

                $("#Detail_PersonId").val(PersonId);
                // var AgeFrom = $(this).closest("tr").find(".AgeFrom").val();

                $("#Detail_AgeFrom").val(AgeFrom);
                //var AgeTo = $(this).closest("tr").find(".AgeTo").val();

                $("#Detail_AgeTo").val(AgeTo);
                //var PeriodId = $(this).closest("tr").find(".PeriodId").val();
                $("#Detail_PeriodId").val(PeriodId);

                //var RangeFrom = $(this).closest("tr").find(".RangeFrom").val();
                $("#Detail_RangeFrom").val(RangeFrom);

                //var RangeTo = $(this).closest("tr").find(".RangeTo").val();

                $("#Detail_RangeTo").val(RangeTo);

                // var NormalValueText = $(this).closest("tr").find(".NormalValueText").val();
                $("#Detail_NormalValueText").val(NormalValueText);

                //var UnitId = $(this).closest("tr").find(".UnitId").val();
                $("#Detail_UnitId").val(UnitId);

                $("#reference-value-grid .js-add").html('<i class="fas fa-pencil-alt"></i>');
                refreshControl();
            })


            $("body").on("click", "#reference-value-grid .js-remove", function (e) {
                e.preventDefault();
                remove($(this));

                fixIndexing();
            })


            function fixIndexing() {
                $("#reference-value-grid tbody tr").each(function (index) {
                    var html = $(this).html();
                    html = html.replace(/\_(.*?)__/g, '_' + index + '__');
                    html = html.replace(/\[(.*?)\]/g, '[' + index + ']');
                    $(this).html(html);

                    /*$(".selectpicker").select2("refresh");*/
                })

                // refreshControl();
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

            $("body").on("change", "#DepartmentId", function () {
                var self = $(this);
                if (self.val().length > 0) {
                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetDoctorsByDepartment",
                        method: "POST",
                        data: { departmentCode: self.val() },
                        success: function (response) {
                            $("#DoctorId").empty();
                            //$("#DoctorId").append($('<option>', {
                            //    value: '',
                            //    text: `---Doctor---`
                            //}));
                            $.each(response, function (i, item) {
                                $("#DoctorId").append($('<option>', {
                                    value: item.code,
                                    text: item.name
                                }));
                            });

                            refreshControl();
                        }
                    });
                }
            });

            $("body").on("change", "#TestCategoryId", function (e) {
                var self = $(this);

                if (self.val().length > 0) {
                    // getTests(self.val());

                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetTestSubCategories",
                        method: "POST",
                        data: { testCategoryCode: self.val() },
                        success: function (response) {
                            $("#TestSubCategoryId").empty();
                            $("#TestSubCategoryId").append($('<option>', {
                                value: '',
                                text: `Select Test Sub Category`
                            }));
                            $.each(response, function (i, item) {
                                $("#TestSubCategoryId").append($('<option>', {
                                    value: item.code,
                                    text: item.name
                                }));
                            });

                            getTests(self.val(), '');
                        }
                    });
                }
            });

            $("body").on("change", "#TestSubCategoryId", function () {
                var self = $(this);

                if (self.val().length > 0) {
                    getTests($("TestCategoryId").val(), self.val());
                }
            });


            $("body").on("keyup", "#VisitingFeeDetail", function () {
                calculateDetailAmount("Visiting");
            });

            $("body").on("keyup", "#ReportShowingFeeDetail", function () {
                calculateDetailAmount("ReportShowing");
            });

            $("body").on("keyup", "#VisitingFeeDetailAmount", function () {
                $("#VisitingFeeDetail").val(0);
                calculateDetail("Visiting");
            });

            $("body").on("keyup", "#ReportShowingFeeDetailAmount", function () {
                $("#ReportShowingFeeDetail").val(0);
                calculateDetail("ReportShowing");
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

            $("body").on("click", ".js-commisssionSetup-export", function () {
                if (selectedItems.length > 0) {
                    var self = $(this);
                    let reportRenderType = self.data("rendertype");

                    $(selectedItems).each(function (index, item) {
                        window.open(
                            settings.baseUrl + `/Export?commisssionSetupCode=${item}&reportType=Prescription&reportRenderType=${reportRenderType}`,
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

        function getTests(testCategoryCode, testSubCategoryCode) {
            $.ajax({
                url: normalizeUrl(getBaseUrl()) + "/Cascading/GetTests",
                method: "POST",
                data: { testCategoryCode: testCategoryCode, testSubCategoryCode: testSubCategoryCode },
                success: function (response) {
                    $("#TestChargeId").empty();
                    $("#TestChargeId").append($('<option>', {
                        value: '',
                        text: `Select Test`
                    }));
                    $.each(response, function (i, item) {
                        $("#TestChargeId").append($('<option>', {
                            value: item.code,
                            text: item.name
                        }));
                    });

                    refreshControl();
                }
            });
        }

        function calculateDetailAmount(selectorPrefix) {
            let feeSelector = $("#" + selectorPrefix + "Fee"),
                discountSelector = $("#" + selectorPrefix + "FeeDetail"),
                discountAmountSelector = $("#" + selectorPrefix + "FeeDetailAmount"),
                fee = parseFloat(feeSelector.val() || 0),
                discount = parseFloat(discountSelector.val() || 0),
                discountAmount = parseFloat(discountAmountSelector.val() || 0);

            if (fee > 0) {
                discountAmount = (fee * discount) / 100;
            }

            discountAmountSelector.val(discountAmount);
            calculateDetail(selectorPrefix);
        }

        function calculateDetail(selectorPrefix) {
            let feeSelector = $("#" + selectorPrefix + "Fee"),
                discountAmountSelector = $("#" + selectorPrefix + "FeeDetailAmount"),
                amountSelector = $("#" + selectorPrefix + "Amount"),
                fee = parseFloat(feeSelector.val() || 0),
                discountAmount = parseFloat(discountAmountSelector.val() || 0),
                amount = parseFloat(amountSelector.val() || 0);

            if (fee > 0) {
                amount = fee - discountAmount;
            }

            amountSelector.val(amount);
        }

        function loadLabTestReferenceSetups(baseUrl, gridSelector) {
            var data = {
                fromDate: $(".datefrom").val(),
                toDate: $(".dateto").val(),
                doctorCode: $(".doctor").val(),
                patientCategoryCode: $(".patientcategory").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/Grid",
                    type: "GET",
                    datatype: "json",
                    // data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "labTestReferenceSetupId", "className": "text-center", width: "5%",
                        render: function (data) {
                            return `<input type="checkbox" class="checkBox" value='${data}' />`;
                        }
                    },
                    {
                        "data": "labTestReferenceSetupId", "className": "text-center", width: "10%"
                    },
                    { "data": "labTestReferenceValue", "className": "text-left", width: "30%" },
                    { "data": "testName", "className": "text-left", width: "20%" },
                    { "data": "testSubCategory", "className": "text-center", width: "10%" },
                    { "data": "testCategory", "className": "text-center", width: "10%" },
                    {
                        "data": "labTestReferenceSetupId", "render": function (data, type, row) {
                            return `<div class='p-1 action-buttons'>
                                        <a class='btn btn-warning btn-circle btn-sm js-labTestReferenceSetup-edit' data-url="/setup/${data}" title="Edit ${row.labTestReferenceSetupId}" data-id='${data}'><i class='fas fa-pencil-alt'></i></a> 
                                        <button type="button" class="btn btn-danger btn-circle btn-sm js-labTestReferenceSetup-delete-confirm"
                                                data-target="#deleteModalx"
                                                data-id="${data}"
                                                title="Delete ${row.labTestReferenceSetupId}"
                                                data-title="Are you sure want to delete ${row.labTestReferenceSetupId}?">
                                                    <i class="fas fa-trash fa-sm"></i>
                                        </button>`;
                        },
                        "orderable": false,
                        "searchable": false,
                        width: "7%"
                    }
                ],
                lengthChange: true,
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

        function execute(url, data = {}, type = "POST") {
            return new Promise((resolve, reject) => {
                $.ajax({
                    url: url,
                    type: type,
                    data: data,
                    success: function (data) {
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
                escapeMarkup: function (markup) {
                    return markup;
                }
            });

            $('.multiselect').multiselect({
                includeSelectAllOption: true,
                enableCaseInsensitiveFiltering: true,
                buttonContainer: '<div class="btn-group w-100" />',
                onSelectAll: function (options) {
                    // alert('onSelectAll triggered, ' + options.length + ' options selected!');
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
        function refreshControl() {
            $('.multiselect').multiselect('destroy');
            initialize();
        }
    }
}(jQuery));

