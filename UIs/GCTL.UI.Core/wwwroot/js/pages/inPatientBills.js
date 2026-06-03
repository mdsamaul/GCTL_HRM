(function ($) {
    $.inPatientBills = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#billentry-form",
            formContainer: ".js-billentry-form-container",
            gridSelector: "#patients-grid",
            gridContainer: ".js-billentry-grid-container",
            editSelector: ".js-billentry-edit",
            saveSelector: ".js-billentry-save",
            selectAllSelector: "#billentry-check-all",
            deleteSelector: ".js-billentry-delete-confirm",
            deleteModal: "#billentry-delete-modal",
            finalDeleteSelector: ".js-billentry-delete",
            clearSelector: ".js-billentry-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-billentry-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-billentry-check-availability",
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
            $("body").on("click", `${settings.editSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = settings.baseUrl + $(this).data("url");

                loadForm(url)
                    .then((data) => {
                        discountRequirement();
                    })
                    .catch((error) => {
                        console.log(error)
                    })
                $("html, body").animate({ scrollTop: $("#billEntryDetailsHeader").offset().top }, 500);
            });

            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                loadPatients(settings.baseUrl, settings.gridSelector);
                loadForm(saveUrl);
                initialize();
            });

            // Save
            $("body").on("click", settings.saveSelector, function () {
                if ($("#billEntryDetails tbody tr").length == 0) {
                    toastr.info("Nothing to save", 'Info');
                    return false;
                }

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
                                    loadPatients(settings.baseUrl, settings.gridSelector);
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

            $("body").on("click", ".js-billentry-sms-confirm", function (e) {
                e.preventDefault();
                $('input:checkbox.checkBox').each(function () {
                    if ($(this).prop('checked')) {
                        if (!selectedItems.includes($(this).val())) {
                            selectedItems.push($(this).val());
                        }
                    }
                });

                if (selectedItems.length > 0) {
                    $("#billentry-sms-modal").modal("show");
                } else {
                    toastr.info("Please select at least one item.", 'Warning');
                }
            });


            $("body").on('show.bs.modal', "#billentry-sms-modal", function (event) {
                //event.preventDefault();
                // Get button that triggered the modal
                var source = $(event.relatedTarget);
                var id = source.data("id");

                // Extract value from data-* attributes
                var title = source.data("title");
                title = "Are you sure want to send sms to these patients?";
                var modal = $(this);
                $(modal).find('.title').html(title);

                $("body").on("click", ".js-billentry-sms", function (e) {
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
                $("body").off("click", ".js-billentry-sms");
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
                    $("html, body").animate({ scrollTop: $("#billEntryDetailsHeader").offset().top }, 500);
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
            });

            var counter = 0;
            $("body").on("click", "#billEntryDetails .js-add", function () {
                if ($("#BillEntry_ServiceChargeGroupId").val() == '') {
                    $("#BillEntry_ServiceChargeGroupIdError").addClass("d-none")
                        .removeClass("d-none");
                    $("#BillEntry_ServiceChargeGroupId").focus();
                    return;
                }
                else {
                    $("#BillEntry_ServiceChargeGroupIdError").removeClass("d-none")
                        .addClass("d-none");
                }

                if ($("#BillEntry_ServiceChargeHeadId").val() == '') {
                    $("#BillEntry_ServiceChargeHeadIdError").addClass("d-none")
                        .removeClass("d-none");
                    $("#BillEntry_ServiceChargeHeadId").focus();
                    return;
                }
                else {
                    $("#BillEntry_ServiceChargeHeadIdError").removeClass("d-none")
                        .addClass("d-none");
                }

                if (parseFloat($("#BillEntry_Amount").val() | 0) == 0) {
                    $("#BillEntry_AmountError").addClass("d-none")
                        .removeClass("d-none");
                    $("#BillEntry_Amount").focus();
                    return;
                }
                else {
                    $("#BillEntry_AmountError").removeClass("d-none")
                        .addClass("d-none");
                }

                let billServiceChargeGroup = $("#BillEntry_ServiceChargeGroupId option:selected").text(),
                    billServiceChargeHead = $("#BillEntry_ServiceChargeHeadId option:selected").text(),
                    billSerialNo = $("#BillEntry_SerialNo").val(),
                    billSerialCounter = $("#BillEntry_SerialCounter").html(),
                    billServiceChargeGroupId = $("#BillEntry_ServiceChargeGroupId").val(),
                    billServiceChargeHeadId = $("#BillEntry_ServiceChargeHeadId").val(),

                    unit = $("#BillEntry_NoOfDaysOrQuantity").val(),
                    rate = $("#BillEntry_Rate").val(),
                    amount = $("#BillEntry_Amount").val(),

                    discountPercent = $("#BillEntry_DiscountPercent").val(),
                    discountAmount = $("#BillEntry_DiscountAmount").val(),
                    lineTotal = $("#BillEntry_LineTotal").val(),


                    counter = $("#billEntryDetails tbody tr").length;

                if (billSerialNo != "") {
                    $("#details-" + billSerialNo + " .SerialCounter").html(billSerialCounter);
                    $("#details-" + billSerialNo + " .SerialNo").val(billSerialNo);

                    $("#details-" + billSerialNo + " .ServiceChargeGroupName").html(billServiceChargeGroup);
                    $("#details-" + billSerialNo + " .ServiceChargeGroupId").val(billServiceChargeGroupId);

                    $("#details-" + billSerialNo + " .ServiceChargeHeadName").html(billServiceChargeHead);
                    $("#details-" + billSerialNo + " .ServiceChargeHeadId").val(billServiceChargeHeadId);

                    $("#details-" + billSerialNo + " .UnitName").html(unit);
                    $("#details-" + billSerialNo + " .NoOfDaysOrQuantity").val(unit);

                    $("#details-" + billSerialNo + " .RateDisplay").html(rate);
                    $("#details-" + billSerialNo + " .Rate").val(rate);

                    $("#details-" + billSerialNo + " .AmountDisplay").html(amount);
                    $("#details-" + billSerialNo + " .Amount").val(amount);

                    $("#details-" + billSerialNo + " .DiscountPercentDisplay").html(discountPercent);
                    $("#details-" + billSerialNo + " .DiscountPercent").val(discountPercent);

                    $("#details-" + billSerialNo + " .DiscountAmountDisplay").html(discountAmount);
                    $("#details-" + billSerialNo + " .DiscountAmount").val(discountAmount);

                    $("#details-" + billSerialNo + " .LineTotalDisplay").html(lineTotal);
                    $("#details-" + billSerialNo + " .LineTotal").val(lineTotal);

                } else {
                    let item =
                        `<tr id="details-${counter}">
                         <td class="text-center text-middle">
                            <span class="SerialCounter" id="BillEntries_${counter}__SerialNo" name="BillEntries[${counter}].SerialNo">${counter + 1}</span>
                            <input type="hidden" class="SerialNo" id="BillEntries_${counter}__SerialNo" name="BillEntries[${counter}].SerialNo" value="${counter}" />
                        </td>
                        <td class="text-middle">
                            <span class="ServiceChargeGroupName">${billServiceChargeGroup}</span>
                            <input type="hidden" id="BillEntries_${counter}__ServiceChargeGroupId" name="BillEntries[${counter}].ServiceChargeGroupId" value="${billServiceChargeGroupId}" class="ServiceChargeGroupId" />
                        </td>
                        <td class="text-middle">
                            <span class="ServiceChargeHeadName">${billServiceChargeHead}</span>
                            <input type="hidden" id="BillEntries_${counter}__ServiceChargeHeadId" name="BillEntries[${counter}].ServiceChargeHeadId" value="${billServiceChargeHeadId}" class="ServiceChargeHeadId" />
                        </td>
                        <td class="text-middle">
                            <span class="UnitName"> ${unit}</span>
                            <input type="hidden" id="BillEntries_${counter}__NoOfDaysOrQuantity" name="BillEntries[${counter}].NoOfDaysOrQuantity" value="${unit}" class="NoOfDaysOrQuantity" />
                        </td>
                        <td class="text-middle">
                            <span class="RateDisplay"> ${rate}</span>
                            <input type="hidden" id="BillEntries_${counter}__Rate" name="BillEntries[${counter}].Rate" value="${rate}" class="Rate" />
                        </td>
                        <td class="text-right text-middle">
                            <span class="AmountDisplay"> ${amount}</span>
                            <input type="hidden" id="BillEntries_${counter}__Amount" name="BillEntries[${counter}].Amount" value="${amount}" class="Amount" />
                        </td>                  
                        <td class="text-right text-middle">
                            <span class="DiscountPercentDisplay">${discountPercent}</span>
                            <input type="hidden" id="BillEntries_${counter}__DiscountPercent" name="BillEntries[${counter}].DiscountPercent" value="${discountPercent}" class="DiscountPercent" />
                        </td>
                        <td class="text-right text-middle">
                            <span class="DiscountAmountDisplay">${discountAmount}</span>
                            <input type="hidden" id="BillEntries_${counter}__DiscountAmount" name="BillEntries[${counter}].DiscountAmount" value="${discountAmount}" class="DiscountAmount" />
                        </td>
                        <td class="text-right text-middle">
                            <span class="LineTotalDisplay">${lineTotal}</span>
                            <input type="hidden" id="BillEntries_${counter}__LineTotal" name="BillEntries[${counter}].LineTotal" value="${lineTotal}" class="LineTotal" />
                        </td>
                       <td class="text-middle">
                            <button type="button" class="btn btn-sm btn-success js-edit" title="Select for edit"><i class="fa fa-pencil-alt"></i></button>
                            <button type='button' class='btn btn-sm btn-danger js-remove' title="Delete Bill"><i class='fa fa-trash'></i></button>
                        </td>
                    </tr>`;

                    $("#billEntryDetails tbody").append(item);
                    counter++;
                }

                $("#BillEntry_ServiceChargeGroupId").val("");
                $("#BillEntry_ServiceChargeHeadId").val("");
                $("#BillEntry_NoOfDaysOrQuantity").val("");
                $("#BillEntry_Rate").val("");
                $("#BillEntry_Amount").val("");
                $("#BillEntry_DiscountPercent").val("");
                $("#BillEntry_DiscountAmount").val("");
                $("#BillEntry_LineTotal").val("");
                $("#BillEntry_SerialNo").val("");
                initialize();


                $("#billEntryDetails .js-add").html('<i class="fas fa-plus"></i>');

                calculate();
            });

            $("body").on("click", "#billEntryDetails .js-edit", function (e) {
                e.preventDefault();
                $("#BillEntry_SerialNo").val($(this).closest("tr").find(".SerialNo").val());
                $("#BillEntry_ServiceChargeGroupId").val($(this).closest("tr").find(".ServiceChargeGroupId").val());
                $("#BillEntry_ServiceChargeHeadId").val($(this).closest("tr").find(".ServiceChargeHeadId").val());
                $("#BillEntry_NoOfDaysOrQuantity").val($(this).closest("tr").find(".NoOfDaysOrQuantity").val());
                $("#BillEntry_Rate").val($(this).closest("tr").find(".Rate").val());
                $("#BillEntry_Amount").val($(this).closest("tr").find(".Amount").val());
                $("#BillEntry_DeliveryDate").val($(this).closest("tr").find(".DeliveryDate").val());
                $("#BillEntry_DiscountPercent").val($(this).closest("tr").find(".DiscountPercent").val());
                $("#BillEntry_DiscountAmount").val($(this).closest("tr").find(".DiscountAmount").val());
                $("#BillEntry_LineTotal").val($(this).closest("tr").find(".LineTotal").val());
                initialize();

                $("#billEntryDetails .js-add").html('<i class="fa fa-pencil-alt"></i>');
            })

            $("body").on("click", "#billEntryDetails .js-remove", function (e) {
                e.preventDefault();
                remove($(this));

                calculate();


                fixIndexing();
            })


            function fixIndexing() {
                $("#billEntryDetails tbody tr").each(function (index) {
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

                        $("#BillEntry_ServiceChargeGroupId").trigger("change");
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
                            $("#RoomNo").empty();
                            $("#RoomNo").append($('<option>', {
                                value: '',
                                text: `Select Room`
                            }));
                            $.each(response, function (i, item) {
                                $("#RoomNo").append($('<option>', {
                                    value: item.code,
                                    text: item.name
                                }));
                            });
                        }
                    });
                }
            });

            $("body").on("change", "#BillEntry_ServiceChargeGroupId", function () {
                var self = $(this);
                if (self.val().length > 0) {
                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetServiceChargeHeads",
                        method: "GET",
                        data: { serviceChargeGroupId: self.val() },
                        success: function (response) {
                            console.log(response);
                            $("#BillEntry_ServiceChargeHeadId").empty();
                            $("#BillEntry_ServiceChargeHeadId").append($('<option>', {
                                value: '',
                                text: `Select Service Charge Head`
                            }));
                            $.each(response, function (i, item) {
                                $("#BillEntry_ServiceChargeHeadId").append($('<option>', {
                                    value: item.code,
                                    text: item.name
                                }));
                            });
                        }
                    });
                }
            });

            $("body").on("keyup", ".js-calculate-amount", function () {
                calculateAmount();
            });

            $("body").on("keyup", "#BillEntry_DiscountPercent", function () {
                calculateDiscountAmount();
            });

            $("body").on("keyup", "#BillEntry_DiscountAmount", function () {
                $("#BillEntry_DiscountPercent").val(0);
                calculateDiscount();
            });


            $("body").on("keyup", ".js-calculate", function () {
                calculate();
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

            $("body").on("change", ".patientCategory, .department, .doctor, .referenceId, .datefrom, .dateto, .roomType, .roomNo", function () {
                loadPatients(settings.baseUrl, settings.gridSelector);
            });

            $("body").on("click", ".js-billentry-export", function () {
                if (selectedItems.length > 0) {
                    var self = $(this);
                    let reportRenderType = self.data("rendertype");

                    $(selectedItems).each(function (index, item) {
                        window.open(
                            settings.baseUrl + `/Export?billEntryNo=${item}&reportType=InPatientBill&reportRenderType=${reportRenderType}`,
                            "_blank"
                        );
                    })
                } else {
                    toastr.info("Please select at least one item.", 'Warning');
                }
            });

            $("body").on("click", ".js-billentry-preview", function () {
                if (selectedItems.length > 0) {
                    // Text
                    $.LoadingOverlay("show", {
                        image: "",
                        text: "Please wait..."
                    });
                    var self = $(this);
                    let reportRenderType = self.data("rendertype") ?? "PDF";

                    $(selectedItems).each(function (index, item) {
                        $.ajax({
                            url: settings.baseUrl + "/Export",
                            method: "POST",
                            data: {
                                billEntryNo: item,
                                reportType: "InPatientBill",
                                reportRenderType: reportRenderType,
                                isPreview: true
                            },
                            success: function (response) {
                                $.LoadingOverlay("hide");
                                window.open(
                                    normalizeUrl(getBaseUrl()) + `/Preview/InPatientBill`,
                                    "_blank"
                                )
                            }
                        });
                    })
                } else {
                    toastr.info("Please select at least one item.", 'Warning');
                }
            });

            $("body").on("keyup", "#DiscountAmount", function () {
                $("#DiscountPercent").val("");
                calculate();                
            })
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

        function calculateAmount() {
            let unitSelector = $("#BillEntry_NoOfDaysOrQuantity"),
                rateSelector = $("#BillEntry_Rate"),
                amountSelector = $("#BillEntry_Amount"),
                amount = parseFloat(amountSelector.val() || 0),
                unit = parseFloat(unitSelector.val() || 0),
                rate = parseFloat(rateSelector.val() || 0);

            if (unit > 0 && rate > 0)
                amount = unit * rate;

            amountSelector.val(amount);
            calculateDiscount();
        }


        function calculateDiscountAmount() {
            let amountSelector = $("#BillEntry_Amount"),
                discountPercentSelector = $("#BillEntry_DiscountPercent"),
                discountAmountSelector = $("#BillEntry_DiscountAmount"),
                amount = parseFloat(amountSelector.val() || 0),
                discountPercent = parseFloat(discountPercentSelector.val() || 0),
                discountAmount = parseFloat(discountAmountSelector.val() || 0);

            if (amount > 0) {
                discountAmount = (amount * discountPercent) / 100;
            }

            discountAmountSelector.val(discountAmount);
            calculateDiscount();
        }


        function calculateDiscount() {
            let amountSelector = $("#BillEntry_Amount"),
                discountAmountSelector = $("#BillEntry_DiscountAmount"),
                lineTotalSelctor = $("#BillEntry_LineTotal"),
                amount = parseFloat(amountSelector.val() || 0),
                discountAmount = parseFloat(discountAmountSelector.val() || 0),
                lineTotal = parseFloat(lineTotalSelctor.val() || 0);

            if (amount > 0) {
                lineTotal = amount - discountAmount;
            }

            lineTotalSelctor.val(lineTotal);
            calculate();
        }


        function calculate() {
            let rowSelector = $("#billEntryDetails tbody tr .Amount"),
                totalAmountSelector = $("#TotalAmount"),
                discountPercentSelector = $("#DiscountPercent"),
                discountAmountSelector = $("#DiscountAmount"),
                afterDiscountAmountSelector = $("#AfterDiscountAmount"),
                vatPercentSelector = $("#VatPercent"),
                vatAmountSelector = $("#VatAmount"),
                taxPercentSelector = $("#TaxPercent"),
                taxAmountSelector = $("#TaxAmount"),
                netAmountSelector = $("#NetAmount"),
                advanceSelector = $("#Advance"),
                dueSelector = $("#Due"),

                totalAmount = parseFloat(totalAmountSelector.val() || 0),
                discountPercent = parseFloat(discountPercentSelector.val() || 0),
                discountAmount = parseFloat(discountAmountSelector.val() || 0),
                afterDiscountAmount = parseFloat(afterDiscountAmountSelector.val() || 0),
                vatPercent = parseFloat(vatPercentSelector.val() || 0),
                vatAmount = parseFloat(vatAmountSelector.val() || 0),
                taxPercent = parseFloat(taxPercentSelector.val() || 0),
                taxAmount = parseFloat(taxAmountSelector.val() || 0),

                netAmount = parseFloat(netAmountSelector.val() || 0),
                advance = parseFloat(advanceSelector.val() || 0),
                dueAmount = parseFloat(dueSelector.val() || 0);

            discountRequirement();

            totalAmount = 0; // Reset totalAmount
            $.map(rowSelector, function (element) {
                totalAmount += parseFloat(element.value);
            });

            totalAmountSelector.val(totalAmount);
            var individualDiscountAmount = 0;
            $("#billEntryDetails tbody tr").each(function () {
                let disAmount = parseFloat($(this).find(".DiscountAmountDisplay").text() || 0);
                individualDiscountAmount += disAmount;
            });

            if (individualDiscountAmount > 0) {
                discountAmount = individualDiscountAmount;
            }

            if (discountPercent > 0) {
                discountAmount = (totalAmount * discountPercent) / 100;
                discountAmountSelector.val(discountAmount);
            }

            afterDiscountAmount = totalAmount - discountAmount;
            afterDiscountAmountSelector.val(afterDiscountAmount);

            vatAmount = (afterDiscountAmount * vatPercent) / 100;
            vatAmountSelector.val(vatAmount);

            taxAmount = (afterDiscountAmount * taxPercent) / 100;
            taxAmountSelector.val(taxAmount);

            netAmount = afterDiscountAmount + vatAmount + taxAmount;
            netAmountSelector.val(netAmount);

            dueAmount = netAmount - advance;
            dueSelector.val(dueAmount);
        }

        function discountRequirement() {
            var individualDiscountAmount = 0;
            $("#billEntryDetails tbody tr").each(function () {
                let disAmount = parseFloat($(this).find(".DiscountAmountDisplay").text() || 0);
                individualDiscountAmount += disAmount;
            });

            if (individualDiscountAmount > 0) {
                $("#DiscountAmount").val(individualDiscountAmount);
                $("#DiscountPercent").val('');
                $("#DiscountPercent").attr("readonly", "readonly");
                $("#DiscountAmount").attr("readonly", "readonly");
            }
            else {
                $("#DiscountPercent").removeAttr("readonly");
                $("#DiscountAmount").removeAttr("readonly");
            }
        }

        //function discountRequirement() {
        //    var isIndividualDiscountApplied = false;
        //    var isDiscountApplied = false;
        //    $("#billEntryDetails tbody tr").each(function () {
        //        let disPercent = parseFloat($(this).find(".DiscountPercentDisplay").text() || 0);
        //        let disAmount = parseFloat($(this).find(".DiscountAmountDisplay").text() || 0);

        //        if (disPercent > 0 || disAmount > 0) {
        //            isIndividualDiscountApplied = true;
        //            return false; // breaks
        //        }
        //    });

        //    if (isIndividualDiscountApplied) {
        //        $("#DiscountAmount").val('');
        //        $("#DiscountPercent").val('');
        //        $("#DiscountPercent").attr("readonly", "readonly");
        //        $("#DiscountAmount").attr("readonly", "readonly");
        //    } else {
        //        $("#DiscountPercent").removeAttr("readonly");
        //        $("#DiscountAmount").removeAttr("readonly");
        //    }

        //    let discoutPercent = parseFloat($("#DiscountPercent").val() || 0);
        //    let discountAmount = parseFloat($("#DiscountAmount").val() || 0);

        //    if (discoutPercent > 0 || discountAmount > 0) {
        //        isDiscountApplied = true;
        //    }

        //    if (isDiscountApplied) {
        //        $("#BillEntry_DiscountAmount").val('');
        //        $("#BillEntry_DiscountPercent").val('');
        //        $("#BillEntry_DiscountPercent").attr("readonly", "readonly");
        //        $("#BillEntry_DiscountAmount").attr("readonly", "readonly");
        //    } else {
        //        $("#BillEntry_DiscountPercent").removeAttr("readonly");
        //        $("#BillEntry_DiscountAmount").removeAttr("readonly");
        //    }
        //}

        function loadPatients(baseUrl, gridSelector) {
            var data = {
                fromDate: $(".datefrom").val(),
                toDate: $(".dateto").val(),
                patientCategoryCode: $(".patientCategory").val(),
                departmentCode: $(".department").val(),
                doctorCode: $(".doctor").val(),
                referenceId: $(".referenceId").val(),
                roomTypeCode: $(".roomType").val(),
                roomNo: $(".roomNo").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/InPatients",
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
                    { "data": "patientCode", "className": "text-center", width: "100px" },
                    { "data": "patientName", "className": "text-left", width: "150px" },
                    { "data": "phone", "className": "text-center", width: "100px" },
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
                    },
                    {
                        "data": "dischargeDate", "className": "text-left", width: "150px"
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

            loadBillEntries(baseUrl, "#billentry-grid");
        }

        function loadBillEntries(baseUrl, gridSelector) {
            var data = {
                fromDate: $(".datefrom").val(),
                toDate: $(".dateto").val(),
                patientCategoryCode: $(".patientCategory").val(),
                departmentCode: $(".department").val(),
                doctorCode: $(".doctor").val(),
                referenceId: $(".referenceId").val(),
                roomTypeCode: $(".roomType").val(),
                roomNo: $(".roomNo").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/BillEntries",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "billEntryNo", "className": "text-center", width: "30px",
                        render: function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "billEntryNo", "className": "text-center", width: "130px"
                    },
                    { "data": "billEntryDateTime", "className": "text-center", width: "130px" },
                    { "data": "netAmount", "className": "text-center", width: "100px" },
                    { "data": "patientCode", "className": "text-center", width: "80px" },
                    { "data": "patientName", "className": "text-left", width: "150px" },
                    { "data": "phone", "width": "50px" },
                    { "data": "registrationId", "className": "text-center", width: "50px" },
                    {
                        "data": "doctorName", "className": "text-left", width: "130px"
                    },
                    { "data": "referencePerson", "className": "text-left", width: "130px" },
                    /*{ "data": "entryUser", "className": "text-center", width: "130px" },*/
                    {
                        "data": "billEntryNo", "render": function (data, type, row) {
                            return `<div class='action-buttons p-1'>
                                        <a class='btn btn-success btn-circle btn-sm js-billentry-edit' data-url="/setup/${data}" title="Edit ${row.patientName}" data-id='${data}'><i class='fas fa-pencil-alt'></i></a> 
                                        <button type="button" class="btn btn-danger btn-circle btn-sm js-billentry-delete-confirm"
                                                data-target="#deleteModalx"
                                                data-id="${data}"
                                                title="Delete ${row.patientName}"
                                                data-title="Are you sure want to delete ${row.patientName}?">
                                                    <i class="fas fa-trash fa-sm"></i>
                                        </button>
                                        <button type="button" class="btn btn-danger btn-circle btn-sm js-billentry-sms-confirm d-none"
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
                escapeMarkup: function (markup) {
                    return markup;
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
                loadPatients(settings.baseUrl, settings.gridSelector);
            });
        }
    }
}(jQuery));

