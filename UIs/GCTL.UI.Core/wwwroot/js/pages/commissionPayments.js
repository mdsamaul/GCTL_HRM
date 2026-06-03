(function ($) {
    $.commissionPayments = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#commissionPayment-form",
            formContainer: ".js-commissionPayment-form-container",
            gridSelector: "#payments-grid",
            gridContainer: ".js-commissionPayment-grid-container",
            editSelector: ".js-commissionPayment-edit",
            saveSelector: ".js-commissionPayment-save",
            selectAllSelector: "#commissionPayment-check-all",
            deleteSelector: ".js-commissionPayment-delete-confirm",
            deleteModal: "#commissionPayment-delete-modal",
            finalDeleteSelector: ".js-commissionPayment-delete",
            clearSelector: ".js-commissionPayment-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-commissionPayment-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-commissionPayment-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            load: function () {

            }
        }, options);


        var gridUrl = settings.baseUrl + "/grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedPaymentHistories = [];
        var selectedItems = [];
        $(() => {
            //  $(settings.saveSelector).attr("disabled", "disabled");
            //settings.load(settings.baseUrl, settings.gridSelector);
            // loadPayableCommissions("#payable-commissions");
            //loadCommissionPayments();

            //if ($(".commissionReceiver").val() != "")
            //    loadCommissionPayments();

            var d = new Date();
            var month = d.getMonth() + 1;
            var day = d.getDate();

            var output = (day < 10 ? '0' : '') + day + '/' +
                (month < 10 ? '0' : '') + month + '/' + d.getFullYear();

            $('#txtGridFromDate').val('01' + '/' + (month < 10 ? '0' : '') + month + '/' + d.getFullYear());
            $('#txtGridToDate').val(output);
            initialize();


            // $(".CommissionReceiver").trigger("change");
            $("body").on("click", `${settings.editSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = settings.baseUrl + $(this).data("url");
                // let url = saveUrl + "/" + $(this).data("id") ?? "";

                loadForm(url)
                    .then((data) => {
                        /* $(".CommissionReceiver").trigger("change");*/
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
                loadCommissionSetups(settings.baseUrl, settings.gridSelector);
                loadForm(saveUrl);
                initialize();
            });

            // Save
            $("body").on("click", settings.saveSelector, function () {
                if (selectedPaymentHistories.length == 0) {
                    toastr.info("Nothing to save", 'Info');
                    return false;
                }

                $("#PaymentHistoryId").val(selectedPaymentHistories);

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
                                    if (response.secondaryMessage != undefined) {
                                        let warningTemplate = `<div class="alertx alert-warningx js-error"><p class="text-danger m-0"><strong><em>${response.secondaryMessage}</em></strong></p></div>`;
                                        $(".js-message").html(warningTemplate);

                                        setTimeout(function () {
                                            $(".js-error").fadeOut();
                                        }, 5000);
                                    }
                                    paidAmount = 0;
                                    selectedPaymentHistories = [];
                                    $(".commissionReceiver").trigger("change");
                                    loadCommissionPayments();
                                })
                                .catch((error) => {
                                    console.log(error)
                                })

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


                //debugger;
                //if (preventForward)
                //    return false;
                //else
                //    $.ajax(options);
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
                                loadCommissionPayments();
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

            $("body").on('change', "#DoctorId", function () {
                if ($(this).val().length > 0) {
                    $("#ReferenceId").val("");
                    $("#Receiver").val("Doctor");

                    $("#ReceiverId").val($(this).val());
                    $("#ReceiverName").val($("#DoctorId option:selected").text());
                }
            });

            $("body").on('change', "#ReferenceId", function () {
                if ($(this).val().length > 0) {
                    $("#DoctorId").val("");
                    $("#Receiver").val("Ref. Doctor/Person");

                    $("#ReceiverId").val($(this).val());
                    $("#ReceiverName").val($("#ReferenceId option:selected").text());
                }
            });
            //$("body").on('change', '.commissionReceiver', function () {
            //    var receiver = $(".commissionReceiver").data("receiver");
            //    $("#Receiver").val(receiver);

            //    $(target).addClass("d-none")
            //        .removeClass("d-none");
            //    $("#ReceiverId").val($(".commissionReceiver").val());
            //    $("#ReceiverName").val('');
            //    if ($(this).val().length > 0)
            //        $("#ReceiverName").val($(".commissionReceiver option:selected").text().replace("Ref. Doctor/Person", "").replace("Doctor", ""));
            //    else
            //        $("#ReceiverName").val('');
            //});

            $("body").on('change', '#PaymentModeId', function () {
                $(".paymentmode").removeClass("d-none")
                    .addClass("d-none");

                var option = $("#PaymentModeId option:selected").text().toLowerCase();
                var PaymentMode = $("#PaymentModeId").val();
                console.log(option);
                if (PaymentMode == "001") {
                    $(".bkash").removeClass("d-none").addClass("d-none");
              
                }
                if (PaymentMode == "002") {
                    $(".bkash").removeClass("d-none").addClass("d-none");
                    $(".cheque").addClass("d-none")
                        .removeClass("d-none");
                }
                if (PaymentMode == "003") {
                    $(".transfer").addClass("d-none")
                        .removeClass("d-none");
                    $(".bkash").removeClass("d-none").addClass("d-none");
                }
                if (PaymentMode == "004") {
                    $(".bkash").addClass("d-none")
                        .removeClass("d-none");
                }
                $('.selectpicker').select2({});
            });

            //$("body").on('change', '.CommissionReceiver', function () {
            //    $(".receiveroptions").removeClass("d-none")
            //        .addClass("d-none");

            //    var target = $(this).data("target");
            //    $(target).addClass("d-none")
            //        .removeClass("d-none");

            //    if ($(this).val() != "Doctor") {
            //        $.ajax({
            //            url: normalizeUrl(getBaseUrl()) + "/Cascading/GetReferencePersons",
            //            method: "GET",
            //            success: function (response) {
            //                $("#CommissionReceiverId").empty();
            //                $("#CommissionReceiverId").append($('<option>', {
            //                    value: '',
            //                    text: `Ref. Doctor/Person`
            //                }));
            //                $.each(response, function (i, item) {
            //                    $("#CommissionReceiverId").append($('<option>', {
            //                        value: item.code,
            //                        text: item.name
            //                    }));
            //                });

            //                refreshControl();
            //            }
            //        });
            //    }
            //});

            var paidAmount = 0;
            $("body").on("click", "#payment-history-check-all", function () {
                selectedPaymentHistories = [];
                $(".paymenthistory").prop('checked',
                    $(this).prop('checked'));

                if ($(this).is(':checked')) {
                    $(".paymenthistory").each(function (element) {
                        if (!selectedPaymentHistories.includes($(this).val())) {
                            selectedPaymentHistories.push($(this).val());
                            paidAmount += parseFloat($(this).data("amount"));
                        }
                    });
                } else {
                    paidAmount = 0;
                }

                $("#PaidAmount").val(paidAmount.toFixed(2));
            });


            $("body").on("click", "input:checkbox.paymenthistory", function (e) {
                if ($(this).prop('checked')) {
                    if (!selectedPaymentHistories.includes($(this).val())) {
                        selectedPaymentHistories.push($(this).val());
                        paidAmount += parseFloat($(this).data("amount"));
                    }
                } else {
                    selectedPaymentHistories.splice($.inArray($(this).val(), selectedPaymentHistories), 1);
                    paidAmount -= parseFloat($(this).data("amount"));
                }

                $("#PaidAmount").val(paidAmount.toFixed(2));
            });


            $("body").on('click', '.select-item', function () {
                $('input[class="select-item"]').not(this).prop('checked', false);
                if ($(this).is(":checked")) {

                    let url = settings.baseUrl + $(this).data("url");
                    loadForm(url);

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
            $("body").on("click", ".js-Commission-previewPaybleList", function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                loadPayableCommissions("#payable-commissions");
                loadCommissionSummary();
            });
            $("body").on("click", ".js-Commission-GridPreivew", function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                loadCommissionPayments();
            });

            //$("body").on("change", ".commissionReceiver, .datefrom, .dateto", function () {
            //    loadCommissionPayments();
            //});

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

        function loadCommissionPayments() {
            var data = {
                commissionReceiver: "",
                commissionReceiverId: "",
                fromDate: $("#txtGridFromDate").val(),
                toDate: $("#txtGridToDate").val()
            };

            var dataTable = $(settings.gridSelector).DataTable({
                ajax: {
                    url: settings.baseUrl + "/Grid",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "paymentNo", "className": "text-center", width: "30px",
                        render: function (data) {
                            return `<input type="checkbox" class="checkBox" value='${data}' />`;
                        }
                    },
                    {
                        "data": "paymentNo", "className": "text-center", width: "40px"
                    },
                    { "data": "paymentDateTime", "className": "text-center", width: "120px" },
                    { "data": "receiverId", "className": "text-center", width: "80px" },
                    { "data": "receiverName", "className": "text-left", width: "250px" },
                    { "data": "paymentType", "className": "text-center", width: "50px" },
                    { "data": "paymentMode", "className": "text-center", width: "50px" },
                    {
                        "data": "paidAmount", "className": "text-center", width: "50px",
                        render: function (data) {
                            var amount = Number(data);
                            if (amount > 0) {
                                if (amount % 1 != 0)
                                    return amount.toFixed(2);
                                else
                                    return data;
                            }
                            else
                                return '';
                        }
                    },
                    { "data": "remarks", "className": "text-center", width: "50px" },
                    { "data": "paidBy", "className": "text-center", width: "50px" }
                 
                ],
                lengthChange: true,
                lengthMenu: [
                    [10, 25, 50, -1],
                    [10, 25, 50, 'All'],
                ],
                pageLength: 10,
                order: [],
                sScrollY: "100%",
                scrollX: true,
                sScrollX: "100%",
                bDestroy: true
            });

        }

        function loadPayableCommissions(gridSelector) {
            var data = {
                commissionReceiver: $("#Receiver").val(),
                commissionReceiverId: $("#ReceiverId").val(),
                fromDate: $(".datefrom").val(),
                toDate: $(".dateto").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: settings.baseUrl + "/PayableCommissions",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "autoId", "className": "text-center", width: "30px",
                        render: function (data, type, row) {
                            return `<input type="checkbox" class="paymenthistory" value='${data}' data-amount='${row.due}' />`;
                        }
                    },
                    {
                        "data": "patientId", "className": "text-center", width: "40px"
                    },
                    { "data": "patientName", "className": "text-center", width: "120px" },
                    { "data": "mrno", "className": "text-center", width: "120px" },
                    { "data": "mrdateTime", "className": "text-center", width: "120px" },
                    { "data": "testCategory", "className": "text-center", width: "80px" },
                    { "data": "testName", "className": "text-left", width: "250px" },
                    { "data": "testAmount", "className": "text-center", width: "50px" },
                    { "data": "netAmount", "className": "text-center", width: "50px" },
                    { "data": "commissionPercent", "className": "text-center", width: "50px" },
                    {
                        "data": "commissionAmount", "className": "text-center", width: "50px",
                        render: function (data) {
                            var amount = Number(data);
                            if (amount > 0) {
                                if (amount % 1 != 0)
                                    return amount.toFixed(2);
                                else
                                    return data;
                            }
                            else
                                return '';
                        }
                    },
                    {
                        "data": "payable", "className": "text-center", width: "50px",
                        render: function (data) {
                            var amount = Number(data);
                            if (amount > 0) {
                                if (amount % 1 != 0)
                                    return amount.toFixed(2);
                                else
                                    return data;
                            }
                            else
                                return '';
                        }
                    },
                    {
                        "data": "paid", "className": "text-center", width: "50px",
                        render: function (data) {
                            var amount = Number(data);
                            if (amount > 0) {
                                if (amount % 1 != 0)
                                    return amount.toFixed(2);
                                else
                                    return data;
                            }
                            else
                                return '';
                        }
                    },
                    {
                        "data": "due", "className": "text-center", width: "50px",
                        render: function (data) {
                            var amount = Number(data);
                            if (amount > 0) {
                                if (amount % 1 != 0)
                                    return amount.toFixed(2);
                                else
                                    return data;
                            }
                            else
                                return '';
                        }
                    }
                ],
                lengthChange: true,
                lengthMenu: [
                    [10, 25, 50, -1],
                    [10, 25, 50, 'All'],
                ],
                pageLength: 10,
                order: [],
                sScrollY: "100%",
                scrollX: true,
                sScrollX: "100%",
                bDestroy: true
            });
        }

        function loadCommissionSummary() {
            var data = {
                commissionReceiver: $("#Receiver").val(),
                commissionReceiverId: $("#ReceiverId").val(),
                fromDate: $(".datefrom").val(),
                toDate: $(".dateto").val()
            };


            var dataTable = $("#commission-summary").DataTable({
                ajax: {
                    url: settings.baseUrl + "/CommissionSummary",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "commissionReceiverId", "className": "text-center", width: "40px"
                    },
                    { "data": "commissionReceiverName", "className": "text-center", width: "120px" },
                    {
                        "data": "totalPayableCommission", "className": "text-center", width: "120px",
                        render: function (data) {
                            return data;
                            //return Math.round(data);
                        }
                    },
                    {
                        "data": "totalDue", "className": "text-center", width: "80px",
                        render: function (data) {
                            return data;
                            //return Math.round(data);
                        }
                    },
                    {
                        "data": "lastPayment", "className": "text-center", width: "80px",
                        render: function (data) {
                            return data;
                            // return Math.round(data);
                        }
                    },
                    { "data": "lastPaymentDate", "className": "text-center", width: "80px" }
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
                dropdownAutoWidth: true,
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

