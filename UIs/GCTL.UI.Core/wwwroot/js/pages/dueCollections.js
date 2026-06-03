(function ($) {
    $.testEntries = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#testentry-form",
            formContainer: ".js-testentry-form-container",
            gridSelector: "#due-grid",
            gridContainer: ".js-testentry-grid-container",
            editSelector: ".js-testentry-edit",
            saveSelector: ".js-testentry-save",
            selectAllSelector: "#testentry-check-all",
            deleteSelector: ".js-testentry-delete-confirm",
            deleteModal: "#testentry-delete-modal",
            finalDeleteSelector: ".js-testentry-delete",
            clearSelector: ".js-testentry-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-testentry-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-testentry-check-availability",
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
          
            initialize();

            var d = new Date();
            var month = d.getMonth() + 1;
            var day = d.getDate();
            var output = (day < 10 ? '0' : '') + day + '/' +
                (month < 10 ? '0' : '') + month + '/' + d.getFullYear();
            $('#FromDate').val('01' + '/' + (month < 10 ? '0' : '') + month + '/' + d.getFullYear());
            $('#ToDate').val(output);

            loadDueReceipts(settings.baseUrl, settings.gridSelector);
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
                //$("#labTestDetails").offset().top
                $("html, body").animate({ scrollTop: 500 }, 500);
            });

            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                /*loadPaidReceipts(settings.baseUrl, settings.gridSelector);*/
                loadDueReceipts(settings.baseUrl, "#paid-grid");
                $("#IsLabTestPatient").trigger("change");
                /*       loadForm(saveUrl);*/
                initialize();
                // $("html, body").animate({ scrollTop: 0 }, 500);
            });

            // Save
            $("body").on("click", settings.saveSelector, function () {
                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }

                var dueReceived = parseFloat($("#DueReceived").val());
                var due = parseFloat($("#DueReceived").data("due"));

                if (due > dueReceived) {
                    if ($("#Remarks").val() == '') {
                        $("#RemarksError").addClass("d-none")
                            .removeClass("d-none");
                        $("#Remarks").focus();
                        return;
                    }
                    else {
                        $("#RemarksError").removeClass("d-none")
                            .addClass("d-none");
                    }
                }
                else {
                    $("#Remarks").removeClass("d-none")
                        .addClass("d-none");
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
                                    $("#IsLabTestPatient").trigger("change");
                                    loadDueReceipts(settings.baseUrl, "#paid-grid");
                                    /*loadPaidReceipts(settings.baseUrl, settings.gridSelector);*/
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
                        url: deleteUrl + "/" + selectedItems + "?billTypeId=" + $(".billtype").val(),
                        method: "POST",
                        success: function (response) {
                            console.log(response);
                            $(modal).modal("hide");
                            if (response.isSuccess) {
                                toastr.success(response.message, 'Success');
                                selectedItems = [];
                                /*settings.load(settings.baseUrl);*/
                                loadDueReceipts(settings.baseUrl, settings.gridSelector);
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

            $("body").on("click", ".js-labtest-sms-confirm", function (e) {
                e.preventDefault();
                $('input:checkbox.checkBox').each(function () {
                    if ($(this).prop('checked')) {
                        if (!selectedItems.includes($(this).val())) {
                            selectedItems.push($(this).val());
                        }
                    }
                });

                if (selectedItems.length > 0) {
                    $("#labtest-sms-modal").modal("show");
                } else {
                    toastr.info("Please select at least one item.", 'Warning');
                }
            });


            $("body").on('show.bs.modal', "#labtest-sms-modal", function (event) {
                //event.preventDefault();
                // Get button that triggered the modal
                var source = $(event.relatedTarget);
                var id = source.data("id");

                // Extract value from data-* attributes
                var title = source.data("title");
                title = "Are you sure want to send sms to these patients?";
                var modal = $(this);
                $(modal).find('.title').html(title);

                $("body").on("click", ".js-labtest-sms", function (e) {
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
                $("body").off("click", ".js-labtest-sms");
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
            });

            var counter = 0;
            $("body").on("click", "#testentrydetails .js-add", function () {
                if ($("#TestEntry_TestCategoryCode").val() == '') {
                    $("#TestEntry_TestCategoryCodeError").addClass("d-none")
                        .removeClass("d-none");
                    $("#TestEntry_TestCategoryCode").focus();
                    return;
                }
                else {
                    $("#TestEntry_TestCategoryCodeError").removeClass("d-none")
                        .addClass("d-none");
                }

                if ($("#TestEntry_TestChargeCode").val() == '') {
                    $("#TestEntry_TestChargeCodeError").addClass("d-none")
                        .removeClass("d-none");
                    $("#TestEntry_TestChargeCode").focus();
                    return;
                }
                else {
                    $("#TTestEntry_TestChargeCodeError").removeClass("d-none")
                        .addClass("d-none");
                }

                if (exceedingDiscountLimit == true) {
                    return;
                }

                let testCategoryName = $("#TestEntry_TestCategoryCode option:selected").text(),
                    testSerialNo = $("#TestEntry_SerialNo").val(),
                    testSerialCounter = $("#TestEntry_SerialCounter").html(),
                    testCategoryCode = $("#TestEntry_TestCategoryCode").val(),

                    testChargeName = $("#TestEntry_TestChargeCode option:selected").text(),
                    testChargeCode = $("#TestEntry_TestChargeCode").val(),

                    deliveryDate = $("#TestEntry_DeliveryDateTime").val(),
                    amount = $("#TestEntry_Amount").val(),

                    discountPercent = $("#TestEntry_DiscountPercent").val(),
                    discountAmount = $("#TestEntry_DiscountAmount").val(),
                    lineTotal = $("#TestEntry_LineTotal").val(),

                    testEntryDetailsId = $("#TestEntry_TestEntryDetailsId").val(),

                    counter = $("#testentrydetails tbody tr").length;

                if (testSerialNo != "") {
                    $("#details-" + testSerialNo + " .SerialCounter").html(testSerialCounter);
                    $("#details-" + testSerialNo + " .SerialNo").val(testSerialNo);

                    $("#details-" + testSerialNo + " .TestCategoryName").html(testCategoryName);
                    $("#details-" + testSerialNo + " .TestCategoryCode").val(testCategoryCode);

                    $("#details-" + testSerialNo + " .TestName").html(testChargeName);
                    $("#details-" + testSerialNo + " .TestChargeCode").val(testChargeCode);

                    $("#details-" + testSerialNo + " .DeliveryDateLabel").html(deliveryDate);
                    $("#details-" + testSerialNo + " .DeliveryDate").val(deliveryDate);

                    $("#details-" + testSerialNo + " .AmountDisplay").html(amount);
                    $("#details-" + testSerialNo + " .Amount").val(amount);

                    $("#details-" + testSerialNo + " .DiscountPercentDisplay").html(discountPercent);
                    $("#details-" + testSerialNo + " .DiscountPercent").val(discountPercent);

                    $("#details-" + testSerialNo + " .DiscountAmountDisplay").html(discountAmount);
                    $("#details-" + testSerialNo + " .DiscountAmount").val(discountAmount);

                    $("#details-" + testSerialNo + " .LineTotalDisplay").html(lineTotal);
                    $("#details-" + testSerialNo + " .LineTotal").val(lineTotal);

                } else {
                    let item =
                        `<tr id="details-${counter}">
                         <td class="text-center text-middle">
                            <span class="SerialCounter" id="TestEntries_${counter}__SerialNo" name="TestEntries[${counter}].SerialNo">${counter + 1}</span>
                            <input type="hidden" class="SerialNo" id="TestEntries_${counter}__SerialNo" name="TestEntries[${counter}].SerialNo" value="${counter}" />
                        </td>
                        <td class="text-middle">
                            <span class="TestCategoryName">${testCategoryName}</span>
                            <input type="hidden" id="TestEntries_${counter}__TestCategoryCode" name="TestEntries[${counter}].TestCategoryCode" value="${testCategoryCode}" class="TestCategoryCode" />
                        </td>
                        <td class="text-middle">
                            <span class="TestName">${testChargeName}</span>
                            <input type="hidden" id="TestEntries_${counter}__TestChargeCode" name="TestEntries[${counter}].TestChargeCode" value="${testChargeCode}" class="TestChargeCode" />
                        </td>
                        <td class="text-middle">
                            <span class="DeliveryDateLabel">${deliveryDate}</span>
                            <input type="hidden" id="TestEntries_${counter}__DeliveryDate" name="TestEntries[${counter}].DeliveryDate" value="${deliveryDate}" class="DeliveryDate" />
                        </td>
                        <td class="text-right text-middle">
                            <span class="AmountDisplay"> ${amount}</span>
                            <input type="hidden" id="TestEntries_${counter}__Amount" name="TestEntries[${counter}].Amount" value="${amount}" class="Amount" />
                        </td>                  
                        <td class="text-right text-middle">
                            <span class="DiscountPercentDisplay">${discountPercent}</span>
                            <input type="hidden" id="TestEntries_${counter}__DiscountPercent" name="TestEntries[${counter}].DiscountPercent" value="${discountPercent}" class="DiscountPercent" />
                        </td>
                        <td class="text-right text-middle">
                            <span class="DiscountAmountDisplay">${discountAmount}</span>
                            <input type="hidden" id="TestEntries_${counter}__DiscountAmount" name="TestEntries[${counter}].DiscountAmount" value="${discountAmount}" class="DiscountAmount" />
                        </td>
                        <td class="text-right text-middle">
                            <span class="LineTotalDisplay">${lineTotal}</span>
                            <input type="hidden" id="TestEntries_${counter}__LineTotal" name="TestEntries[${counter}].LineTotal" value="${lineTotal}" class="LineTotal" />
                        </td>
                       <td class="text-middle">
                            <button type="button" class="btn btn-sm btn-success js-edit" title="Select for edit"><i class="fa fa-pencil-alt"></i></button>
                            <button type='button' class='btn btn-sm btn-danger js-remove' title="Delete Test"><i class='fa fa-trash'></i></button>
                        </td>
                    </tr>`;

                    $("#testentrydetails tbody").append(item);
                    counter++;
                }

                $("#TestEntry_TestCategoryCode").val("");
                $("#TestEntry_TestSubCategoryCode").val("");
                $("#TestEntry_TestChargeCode").val("");
                // $("#TestEntry_DeliveryDate").val("");
                $("#TestEntry_Amount").val("");
                $("#TestEntry_DiscountPercent").val("");
                $("#TestEntry_DiscountAmount").val("");
                $("#TestEntry_LineTotal").val("");

                initialize();

                $("#TestEntry_SerialNo").val("");
                $("#testentrydetails .js-add").html('<i class="fas fa-plus"></i>');
                $(".maxdiscountpercent").text("");
                $(".maxdiscountamount").text("");
                calculate();
            });

            $("body").on("click", "#testentrydetails .js-edit", function (e) {
                e.preventDefault();
                $("#TestEntry_LabTestDetailsId").val($(this).closest("tr").find(".TestEntryDetailsId").val());
                $("#TestEntry_SerialNo").val($(this).closest("tr").find(".SerialNo").val());
                $("#TestEntry_TestCategoryCode").val($(this).closest("tr").find(".TestCategoryCode").val());
                // $("#TestEntry_TestSubCategoryCode").val($(this).closest("tr").find(".TestSubCategoryCode").val());
                $("#TestEntry_TestChargeCode").val($(this).closest("tr").find(".TestChargeCode").val());
                $("#TestEntry_Amount").val($(this).closest("tr").find(".Amount").val());
                $("#TestEntry_DeliveryDate").val($(this).closest("tr").find(".DeliveryDate").val());
                $("#TestEntry_DiscountPercent").val($(this).closest("tr").find(".DiscountPercent").val());
                $("#TestEntry_DiscountAmount").val($(this).closest("tr").find(".DiscountAmount").val());
                $("#TestEntry_LineTotal").val($(this).closest("tr").find(".LineTotal").val());
                initialize();

                $("#testentrydetails .js-add").html('<i class="fa fa-pencil-alt"></i>');

                $("#TestEntry_TestChargeCode").trigger("change");
            })

            $("body").on("click", "#testentrydetails .js-remove", function (e) {
                e.preventDefault();
                remove($(this));

                calculate();
            })

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

            $("body").on("change", "#TestEntry_TestCategoryCode", function () {
                var self = $(this);
                if (self.val().length > 0) {
                    calculateDeliveryTime(self.val(), $("#TestEntry_TestChargeCode").val());
                    getTests(self.val(), '');
                    //$.ajax({
                    //    url: normalizeUrl(getBaseUrl()) + "/Cascading/GetTestSubCategories",
                    //    method: "POST",
                    //    data: { testCategoryCode: self.val() },
                    //    success: function (response) {
                    //        $("#TestEntry_TestSubCategoryCode").empty();
                    //        $("#TestEntry_TestSubCategoryCode").append($('<option>', {
                    //            value: '',
                    //            text: `Sub Category`
                    //        }));
                    //        $.each(response, function (i, item) {
                    //            $("#TestEntry_TestSubCategoryCode").append($('<option>', {
                    //                value: item.code,
                    //                text: item.name
                    //            }));
                    //        });

                    //        getTests(self.val(), '');
                    //    }
                    //});
                }
            });

            $("body").on("change", "#TestEntry_TestSubCategoryCode", function () {
                var self = $(this);

                if (self.val().length > 0) {
                    getTests($("#TestEntry_TestCategoryCode").val(), self.val());
                }
            });

            $("body").on("change", "#TestEntry_TestChargeCode", function () {
                var self = $(this);
                if (self.val().length > 0) {
                    calculateDeliveryTime($("#TestEntry_TestCategoryCode").val(), $("#TestEntry_TestChargeCode").val());
                    $.ajax({
                        url: settings.baseUrl + "/GetTestDiscountDetails",
                        method: "POST",
                        data: { testChargeCode: self.val() },
                        success: function (response) {
                            console.log(response);
                            $("#TestEntry_Amount").val(response.testAmount);
                            $("#TestEntry_LineTotal").val(response.testAmount);

                            if (response.discountPercent > 0) {
                                $("#TestEntry_DiscountPercent").attr("data-discountpercent", response.discountPercent);
                                calculateDiscountAmount();
                            }
                            else {
                                $("#TestEntry_DiscountPercent").attr("readonly", "readonly");
                                if (response.discountAmount > 0) {
                                    $("#TestEntry_DiscountAmount").attr("data-discountamount", response.discountAmount);
                                    calculateDiscountAmount();
                                }
                                else
                                    $("#TestEntry_DiscountAmount").attr("readonly", "readonly");
                            }
                        }
                    });
                }
            });

            var exceedingDiscountLimit;

            $("body").on("keyup", "#TestEntry_DiscountPercent", function () {
                var maxDiscountPercent = parseFloat($(this).data("discountpercent") | 0);
                var discountPercent = parseFloat($(this).val()).toFixed(3);
                if (discountPercent <= maxDiscountPercent) {
                    calculateDiscountAmount();
                    $(".maxdiscountpercent").text('');
                    exceedingDiscountLimit = false;
                }
                else {
                    //$(this).val(maxDiscountPercent);
                    $(".maxdiscountpercent").text(`Max Dis. ${Math.round(maxDiscountPercent)}%!`);
                    exceedingDiscountLimit = true;
                }
            });

            $("body").on("keyup", "#TestEntry_DiscountAmount", function () {
                $("#TestEntry_DiscountPercent").val(0);
                var maxDiscountAmount = parseFloat($(this).data("discountamount")).toFixed(3);
                var discountAmount = parseFloat($(this).val() | 0);
                if (discountAmount <= maxDiscountAmount) {
                    calculateDiscount();
                    $(".maxdiscountamount").text('');
                    exceedingDiscountLimit = false;
                }
                else {
                    //$(this).val(maxDiscountAmount);
                    $(".maxdiscountamount").text(`Max Dis. ${Math.round(maxDiscountAmount)}!`);
                    exceedingDiscountLimit = true;
                }
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

            //$("body").on("change", ".billtype, .patienttype, .datefrom, .dateto, .doctor,.referencePersonId", function () {
            //    loadDueReceipts(settings.baseUrl, settings.gridSelector);
            //});

            $("body").on("click", ".js-DueCollection-GridPreivew", function () {
                loadDueReceipts(settings.baseUrl, settings.gridSelector);
            });

            $("body").on("click", ".js-testentry-export", function () {
                if (selectedItems.length > 0) {
                    var billTypeId = $(".billtype").val();

                    var self = $(this);
                    let reportRenderType = self.data("rendertype") ?? "PDF";
                    var reportType = "MoneyReceipts";
                    if (billTypeId == "001") {
                        reportType = "MoneyReceipts";
                    } else if (billTypeId == "002") {
                        reportType = "InPatientBill";
                    } else {
                        reportType = "OutPatientBill";
                    }

                    $(selectedItems).each(function (index, item) {
                        window.open(
                            settings.baseUrl + `/Export?billNo=${item}&billTypeId=${billTypeId}&reportType=${reportType}&reportRenderType=${reportRenderType}`,
                            "_blank"
                        )
                    })
                } else {
                    toastr.info("Please select at least one item.", 'Warning');
                }
            });

            $("body").on("click", ".js-testentry-preview", function () {
                if (selectedItems.length > 0) {
                    // Text
                    $.LoadingOverlay("show", {
                        image: "",
                        text: "Please wait..."
                    });

                    var billTypeId = $(".billtype").val();

                    var self = $(this);
                    let reportRenderType = self.data("rendertype") ?? "PDF";
                    var reportType = "MoneyReceipts";
                    if (billTypeId == "001") {
                        reportType = "MoneyReceipts";
                    } else if (billTypeId == "002") {
                        reportType = "InPatientBill";
                    } else {
                        reportType = "OutPatientBill";
                    }

                    $(selectedItems).each(function (index, item) {
                        $.ajax({
                            url: settings.baseUrl + "/Export",
                            method: "POST",
                            data: {
                                billTypeId: $(".billtype").val(),
                                billNo: item,
                                reportType: reportType,
                                reportRenderType: reportRenderType,
                                isPreview: true
                            },
                            success: function (response) {
                                $.LoadingOverlay("hide");
                                window.open(
                                    normalizeUrl(getBaseUrl()) + `/Preview/${reportType}`,
                                    "_blank"
                                )
                                //$("#reportPreivew").attr("data", response);
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

            $("body").on("change", "#Patient_SurnameId", function () {
                var surname = $("#Patient_SurnameId option:selected").text().toLowerCase();;
                if (surname.includes("mst") || surname.includes("mrs")) {
                    $("#Patient_SexCode").val(2);
                } else {
                    $("#Patient_SexCode").val(1);
                }

                $(".selectpicker").select2();
            });

            $("body").on("keyup", "#DueReceived", function () {
                var dueReceived = parseFloat($("#DueReceived").val());
                var due = parseFloat($("#DueReceived").data("due"));

                if (due > dueReceived)
                    $("#Less").val(due - dueReceived);

                if (dueReceived > due) {
                    $("#DueReceived").val(due);
                    $("#Less").val(0);
                }
            });
            $("body").on("click", ".js-testentry-printMoneyRecipt", function () {
                var labTestNo = $(this).data("id");



                let reportRenderType = "PDF";

         
                        window.open(
                            `../TestEntries/Export?labTestNo=${labTestNo}&reportType=MoneyReceipts&reportRenderType=${reportRenderType}`,
                            "_blank"
                        )
               
               
            });


        });

        function calculateDeliveryTime(testCategoryCode, testChargeCode) {
            $.ajax({
                url: settings.baseUrl + "/CalculateReportDeliveryTime",
                method: "POST",
                data: { testCategoryCode: testCategoryCode, testChargeCode: testChargeCode },
                success: function (response) {
                    console.log(response);
                    $("#TestEntry_DeliveryDateTime").val(response);
                }
            });
        }

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
                    $("#TestEntry_TestChargeCode").empty();
                    $("#TestEntry_TestChargeCode").append($('<option>', {
                        value: '',
                        text: `Select Test`
                    }));
                    $.each(response, function (i, item) {
                        $("#TestEntry_TestChargeCode").append($('<option>', {
                            value: item.code,
                            text: item.name
                        }));
                    });
                }
            });
        }

        function checkAllowedDiscount(testChargeCode) {
            $("body").on("change", "#TestEntry_TestChargeCode", function () {
                var self = $(this);
                if (self.val().length > 0) {
                    calculateDeliveryTime($("#TestEntry_TestCategoryCode").val(), $("#TestEntry_TestChargeCode").val());
                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetTestCharge",
                        method: "POST",
                        data: { testChargeCode: self.val() },
                        success: function (response) {
                            $("#TestEntry_Amount").val(response.amount);
                            $("#TestEntry_LineTotal").val(response.amount);
                            calculateDiscountAmount();
                        }
                    });
                }
            });
        }

        function calculateDiscountAmount() {
            let amountSelector = $("#TestEntry_Amount"),
                discountPercentSelector = $("#TestEntry_DiscountPercent"),
                discountAmountSelector = $("#TestEntry_DiscountAmount"),
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
            let amountSelector = $("#TestEntry_Amount"),
                discountAmountSelector = $("#TestEntry_DiscountAmount"),
                lineTotalSelctor = $("#TestEntry_LineTotal"),
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
            let rowSelector = $("#testentrydetails tbody tr .Amount"),
                totalAmountSelector = $("#TotalAmount"),
                discountPercentSelector = $("#DiscountPercent"),
                discountAmountSelector = $("#DiscountAmount"),
                payableAmountSelector = $("#Payable"),
                receivedSelector = $("#Received"),
                dueSelector = $("#Due"),

                totalAmount = parseFloat(totalAmountSelector.val() || 0),
                discountPercent = parseFloat(discountPercentSelector.val() || 0),
                discountAmount = parseFloat(discountAmountSelector.val() || 0),

                payableAmount = parseFloat(payableAmountSelector.val() || 0),
                receivedAmount = parseFloat(receivedSelector.val() || 0),
                dueAmount = parseFloat(dueSelector.val() || 0);

            discountRequirement();

            totalAmount = 0; // Reset totalAmount
            $.map(rowSelector, function (element) {
                totalAmount += parseFloat(element.value);
            });

            totalAmountSelector.val(totalAmount);
            var individualDiscountAmount = 0;
            $("#testentrydetails tbody tr").each(function () {
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

            payableAmount = totalAmount - discountAmount;
            payableAmountSelector.val(payableAmount);

            dueAmount = payableAmount - receivedAmount;
            dueSelector.val(dueAmount);
        }


        function discountRequirement() {
            var individualDiscountAmount = 0;
            $("#testentrydetails tbody tr").each(function () {
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

        //function discountRequirementBak() {
        //    var discountAmount = 0;
        //    var isIndividualDiscountApplied = false;
        //    var isDiscountApplied = false;
        //    $("#testentrydetails tbody tr").each(function () {
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
        //        $("#TestEntry_DiscountAmount").val('');
        //        $("#TestEntry_DiscountPercent").val('');
        //        $("#TestEntry_DiscountPercent").attr("readonly", "readonly");
        //        $("#TestEntry_DiscountAmount").attr("readonly", "readonly");
        //    } else {
        //        $("#TestEntry_DiscountPercent").removeAttr("readonly");
        //        $("#TestEntry_DiscountAmount").removeAttr("readonly");
        //    }
        //}

        function loadDueReceipts(baseUrl, gridSelector) {
            var data = {
                billTypeId: $(".billtype").val(),
                patientType: $(".patienttype").val(),
                fromDate: $("#FromDate").val(),
                toDate: $("#ToDate").val(),
                doctorCode: $(".doctor").val(),
                referencePersonId: $(".referencePersonId").val()
            };

            var billTypeId = data.billTypeId;

            var dataTable = $("#due-grid").DataTable({
                ajax: {
                    url: baseUrl + "/GetDueReceipts",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    //{
                    //    "data": "billNo", "className": "text-center", width: "30px",
                    //    render: function (data) {
                    //        return `<input type="checkbox" class="checkBox" value="${data}" />`;
                    //    }
                    //},
                    { "data": "mrNo", "className": "text-center", width: "80px" },
                    { "data": "billDateTimeFormatted", "className": "text-center", width: "130px" },
                    { "data": "payable", "className": "text-right", width: "80px" },
                    { "data": "due", "className": "text-right", width: "80px" },
                    { "data": "patientCode", "className": "text-center", width: "80px" },
                    { "data": "patientName", "className": "text-left", width: "150px" },
                    { "data": "phone", "width": "50px" },
                    { "data": "registrationId", "className": "text-center", width: "50px" },
                    {
                        "data": "doctorName", "className": "text-left", width: "130px"
                    },
                    { "data": "referencePersonName", "className": "text-left", width: "130px" },
                    /*{ "data": "entryUser", "className": "text-center", width: "130px" },*/
                    {
                        "data": "billNo", "render": function (data, type, row) {
                            return `<div class='action-buttons p-1'>
                                        <a class='btn btn-success btn-circle btn-sm js-testentry-edit' data-url="/setup/${data}?billTypeId=${billTypeId}" title="Edit ${row.patientName}" data-id='${data}'><i class='fas fa-pencil-alt'></i></a> 
                                        <a class='btn btn-success btn-circle btn-sm js-testentry-printMoneyRecipt'  data-id='${data}'><i class='fas fa-print'></i></a> `;
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

            loadPaidReceipts(settings.baseUrl, "#paid-grid");
        }

        function loadPaidReceipts(baseUrl, gridSelector) {
            var data = {
                billTypeId: $(".billtype").val(),
                patientType: $(".patienttype").val(),
                fromDate: $("#FromDate").val(),
                toDate: $("#ToDate").val(),
                doctorCode: $(".doctor").val(),
                referencePersonId: $(".referencePersonId").val()
            };

            var billTypeId = data.billTypeId;

            var dataTable = $("#paid-grid").DataTable({
                ajax: {
                    url: baseUrl + "/GetPaidReceipts",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "billNo", "className": "text-center", width: "30px",
                        render: function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    { "data": "mrNo", "className": "text-center", width: "80px" },
                    { "data": "payable", "className": "text-right", width: "80px" },
                    { "data": "due", "className": "text-right", width: "80px" },
                    { "data": "dueReceived", "className": "text-right", width: "80px" },
                    { "data": "paidStatus", "className": "text-center", width: "80px" },
                    { "data": "remarks", "className": "text-left", width: "150px" },
                    { "data": "patientCode", "className": "text-center", width: "80px" },
                    { "data": "patientName", "className": "text-left", width: "150px" },
                    {
                        "data": "doctorName", "className": "text-left", width: "130px"
                    },
                    { "data": "referencePersonName", "className": "text-left", width: "130px" },
                    {
                        "data": "billNo", "render": function (data, type, row) {
                            return `<div class='action-buttons p-1'>
                                        <a class='btn btn-success btn-circle btn-sm js-testentry-edit' data-url="/setup/${data}?billTypeId=${billTypeId}" title="Edit ${row.patientName}" data-id='${data}'><i class='fas fa-pencil-alt'></i></a>`;
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
        }
    }
}(jQuery));

