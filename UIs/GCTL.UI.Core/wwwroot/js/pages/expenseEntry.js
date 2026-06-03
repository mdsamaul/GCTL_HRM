(function ($) {
    $.expenseEntry = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#expenseEntry-form",
            formContainer: ".js-expenseEntry-form-container",
            gridSelector: "#expenseEntry-grid",
            gridContainer: ".js-expenseEntry-grid-container",
            editSelector: ".js-expenseEntry-edit",

            saveSelector: ".js-expenseEntry-save",
            selectAllSelector: "#expenseEntry-check-all",
            deleteSelector: ".js-expenseEntry-delete-confirm",
            deleteModal: "#expenseEntry-delete-modal",
            finalDeleteSelector: ".js-expenseEntry-delete",
            clearSelector: ".js-expenseEntry-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-expenseEntry-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-expenseEntry-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () {

            }
        }, options);



        var gridUrl = settings.baseUrl + "/grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(() => {
            //settings.load(settings.baseUrl, settings.gridSelector);
          
            initialize();
            $(".divRecieved").hide();
            $(".headDetails").hide();

            var d = new Date();
            var month = d.getMonth() + 1;
            var day = d.getDate();

            var output = (day < 10 ? '0' : '') + day + '/' +
                (month < 10 ? '0' : '') + month + '/' + d.getFullYear();
            $('#ExpenseDate').val(output);
            $('#txtFromDate').val('01' + '/' + (month < 10 ? '0' : '') + month + '/' + d.getFullYear());
            $('#txtToDate').val(output);
            LoadGridData(settings.baseUrl, settings.gridSelector);


            $("body").on("click", `${settings.editSelector}`, function (e) {
                //let url = saveUrl + "/" + $(this).data("id") ?? "";
                //loadForm(url);
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
                var ExpenseCode = $(this).data("id");

                $.ajax({
                    type: "GET",
                    url: settings.baseUrl + "/GetInfo",
                    //url: '@Url.Action("GetSingleData", "Con_expenseEntry")',
                    data: { ExpenseCode: ExpenseCode },
                    dataType: "json",
                    success: function (data) {
                        $("#lblhidValue").text(data.expenseCode);
                        $("#ExpenseCode").val(data.expenseCode);
                        $("#ExpenseDate").val(data.expenseDate);
                        $("#SubSusidiaryLedgerCodeNo").val(data.subSusidiaryLedgerCodeNo).change();
                        $("#Amount").val(data.amount);
                        $("#PaymentMode").val(data.paymentMode).change();
                        $("#BkashNo").val(data.bkashNo).change();
                        $("#ChequeNo").val(data.chequeNo);
                        $("#ChequeDate").val(data.chequeDate);
                        $("#BankAccountNo").val(data.bankAccountNo).change();
                        $("#TransferBankFrom").val(data.transferBankFrom).change();
                        $("#TransferBankTo").val(data.transferBankTo);
                        $("#Remarks").val(data.remarks);
                    }
                });

              


                $("html, body").animate({ scrollTop: 0 }, 500);


            });
            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
                Clear();          
                var d = new Date();
                var month = d.getMonth() + 1;
                var day = d.getDate();

                var output = (day < 10 ? '0' : '') + day + '/' +
                    (month < 10 ? '0' : '') + month + '/' + d.getFullYear();
                $('#ExpenseDate').val(output);
                $('#txtFromDate').val('01' + '/' + (month < 10 ? '0' : '') + month + '/' + d.getFullYear());
                $('#txtToDate').val(output);

                $("html, body").animate({ scrollTop: 0 }, 500);

            });
            let loadUrl,
                target,
                reloadUrl,
                title,
                lastCode;
            $("body").on("click", settings.quickAddSelector, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
                alert("FF");
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
            // Save
            $("body").on("click", settings.saveSelector, function () {

                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }
                
                if ($("#ExpenseDate").val()=="") {
                    toastr.info("Expense Date is Requird", 'Info');
                    return false;
                }
                if ($("#SubSusidiaryLedgerCodeNo").val() == "") {
                    toastr.info("Expense Head is Requird", 'Info');
                    return false;
                }
                if ($("#Amount").val() == "") {
                    toastr.info("Expense Head is Requird", 'Info');
                    return false;
                }
                if ($("#PaymentMode").val() == "") {
                    toastr.info("Payment Mode is Requird", 'Info');
                    return false;
                }
                if ($("#PaymentMode").val() == "002") {
                    if ($("#BankAccountNo").val() == "") {
                        toastr.info("Select Bank Account", 'Info');
                        return false;
                    }
                   
                }
                if ($("#PaymentMode").val() == "003") {
                    if ($("#TransferBankFrom").val() == "") {
                        toastr.info("Select Bank Account", 'Info');
                        return false;
                    }
                }
                if ($("#PaymentMode").val() == "004") {
                    if ($("#BkashNo").val() == "") {
                        toastr.info("Select bkash No", 'Info');
                        return false;
                    }
                }
                var data = {
                    ExpenseCode: $("#ExpenseCode").val(),
                    ExpenseDate: $("#ExpenseDate").val(),
                    SubSusidiaryLedgerCodeNo: $("#SubSusidiaryLedgerCodeNo").val(),
                    Amount: $("#Amount").val(),
                    PaymentMode: $("#PaymentMode").val(),
                    BkashNo: $("#BkashNo").val(),
                    ChequeNo: $("#ChequeNo").val(),
                    ChequeDate: $("#ChequeDate").val(),
                    BankAccountNo: $("#BankAccountNo").val(),
                    TransferBankFrom: $("#TransferBankFrom").val(),
                    TransferBankTo: $("#TransferBankTo").val(),
                    Remarks: $("#Remarks").val()
                    
                }
                var hidValue = $("#lblhidValue").text();
                if (hidValue == " ") {
                    hidValue = "";
                }

                var url = "";
                if (hidValue != "") {
                    url = settings.baseUrl + "/Update";
                }
                else {
                    url = settings.baseUrl + "/SaveData";

                }




                var options = {
                    url: url,
                    type: "POST",
                    data: data,
                    success: function (data) {
                        toastr.success(data.message, 'Success');
                        LoadGridData(settings.baseUrl, settings.gridSelector);
                        Clear();
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
            function Clear() {

                $("#lblhidValue").text("");
                $("#ExpenseCode").val("");
                $("#ExpenseDate").val("");
                $("#SubSusidiaryLedgerCodeNo").val("").change();
                $("#Amount").val("");
                $("#PaymentMode").val("").change();
                $("#BkashNo").val("").change();
                $("#ChequeNo").val("");
                $("#ChequeDate").val("");
                $("#BankAccountNo").val("").change();
                $("#TransferBankFrom").val("").change();
                $("#TransferBankTo").val("");
                $("#Remarks").val("");
                MaxID();
                
               

            }

            $("body").on('change', '.PaymentMode', function () {

                var PaymentMode = $("#PaymentMode").val();

                if (PaymentMode == "001") {
                    $(".bkash").removeClass("d-none").addClass("d-none");
                    $(".cheque").removeClass("d-none").addClass("d-none");
                    $(".transfer").removeClass("d-none").addClass("d-none");

                }
                if (PaymentMode == "002") {
                    $(".bkash").removeClass("d-none").addClass("d-none");
                    $(".transfer").removeClass("d-none").addClass("d-none");
                    $(".cheque").addClass("d-none")
                        .removeClass("d-none");
                }
                if (PaymentMode == "003") {
                    $(".bkash").removeClass("d-none").addClass("d-none");
                    $(".cheque").removeClass("d-none").addClass("d-none");
                    $(".transfer").addClass("d-none")
                        .removeClass("d-none");

                }
                if (PaymentMode == "004") {
                    $(".cheque").removeClass("d-none").addClass("d-none");
                    $(".transfer").removeClass("d-none").addClass("d-none");
                    $(".bkash").addClass("d-none")
                        .removeClass("d-none");
                }
                if (PaymentMode == "") {
                    $(".cheque").removeClass("d-none").addClass("d-none");
                    $(".transfer").removeClass("d-none").addClass("d-none");
                    $(".bkashbkash").removeClass("d-none").addClass("d-none");
                }
            });


            $("body").on("click", ".js-voucherEntry-GridPreivew", function (e) {

                LoadGridData(settings.baseUrl, settings.gridSelector);
            });

            $("body").on("click", ".js-expenseEntry-print", function () {
                $('input:checkbox.checkBox').each(function () {
                    if ($(this).prop('checked')) {
                        if (!selectedItems.includes($(this).val())) {
                            selectedItems.push($(this).val());
                        }
                    }
                });
                if (selectedItems.length > 0) {
                    var self = $(this);
                    let reportRenderType = self.data("rendertype") ?? "PDF";
                    $(selectedItems).each(function (index, item) {
                        $.ajax({
                            url: settings.baseUrl + "/Export",
                            method: "POST",
                            data: {
                                expenseCode: item,
                                reportType: "expenseEntryReport",
                                reportRenderType: reportRenderType,
                                isPreview: true
                            },
                            success: function (response) {
                                window.open(
                                    normalizeUrl(getBaseUrl()) + `/Preview/expenseEntryReport`,
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

                var source = $(event.relatedTarget);
                var id = source.data("id");
                var title = source.data("title");
                title = "Are you sure want to Delete these items?";
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
                                LoadGridData(settings.baseUrl, settings.gridSelector);
                                Clear();
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


            $("body").on("keyup", settings.decimalSelector, function () {
                var self = $(this);
                showDecimalPlaces(self.val(), self.parent().find(".input-group-text"));
            });
            function MaxID() {
                $('#ExpenseCode').val("");
                    $.ajax({
                        type: "GET",
                        url: settings.baseUrl + "/MaxID",
                      
                        dataType: 'json',
                        success: function (response) {
                            $('#ExpenseCode').val(response);
                        }
                    })                
            }
        });




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
                        resolve(data);

                    },
                    error: function (error) {
                        reject(error)
                    },
                })
            })
        }


        function initialize() {
            $(settings.formSelector + ' .selectpicker').select2({
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
        }


        function LoadGridData(baseUrl, gridSelector) {
            var FromDate = $('#txtFromDate').val();
            var ToDate = $('#txtToDate').val();;
            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/grid",
                    type: "GET",
                    datatype: "json",
                    data: {
                        FromDate: FromDate,
                        ToDate: ToDate
                    }
                },

                columnDefs: [
                    { targets: [0], orderable: false },
                ],
                columns: [
                    {
                        "data": "expenseCode", "className": "text-center", width: "2%",
                        "render": function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "expenseCode", width: "5%", "render": function (data) {
                            return `<a class='btn js-expenseEntry-edit' style='color:blue' data-id='${data}'><i>${data}</i></a>`;
                        }
                    },
                    { "data": "subSusidiaryLedgerCodeNo", "autowidth": true, "className": "text-center" },
                    { "data": "expenseDate", "autowidth": true, "className": "text-center" },
                    { "data": "amount", "autowidth": true, "className": "text-center" }

                ],
                lengthChange: true,
                pageLength: 10,
                lengthMenu: [
                    [10, 25, 50, -1],
                    [10, 25, 50, 'All'],
                ],
                order: [[1, "Desc"]],
                destroy: true
            });
        }
    }

}(jQuery));

