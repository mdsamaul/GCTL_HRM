(function ($) {
    $.voucherEntry = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#voucherEntry-form",
            formContainer: ".js-voucherEntry-form-container",
            gridSelector: "#voucherEntry-grid",
            gridContainer: ".js-voucherEntry-grid-container",
            editSelector: ".js-voucherEntry-edit",

            saveSelector: ".js-voucherEntry-save",
            selectAllSelector: "#voucherEntry-check-all",
            deleteSelector: ".js-voucherEntry-delete-confirm",
            deleteModal: "#voucherEntry-delete-modal",
            finalDeleteSelector: ".js-voucherEntry-delete",
            clearSelector: ".js-voucherEntry-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-voucherEntry-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-voucherEntry-check-availability",
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
           
            $('#txtFromDate').val('01' + '/' +(month < 10 ? '0' : '') + month + '/' + d.getFullYear());
            $('#txtToDate').val(output);

            $('#VoucherDate').val(output);
            LoadGridData(settings.baseUrl, settings.gridSelector);
            GeneratedTempGrid();


            $("body").on("click", `${settings.editSelector}`, function (e) {
                //let url = saveUrl + "/" + $(this).data("id") ?? "";
                //loadForm(url);
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
                var VoucherNo = $(this).data("id");
               
                $.ajax({
                    type: "GET",
                    url: settings.baseUrl + "/GetInfo",
                    //url: '@Url.Action("GetSingleData", "Con_VoucherEntry")',
                    data: { VoucherNo: VoucherNo },
                    dataType: "json",
                    success: function (data) {
                        $("#lblhidValue").text(data.voucherNo);
                        $("#ddlCompanyCode").val(data.main_CompanyCode).change();
                        $("#VoucherType_Code").val(data.voucherType_Code).change();
                        $("#VoucherNo").val(data.voucherNo);
                        $("#VoucherDate").val(data.voucherDate);
                        $("#Narration").val(data.narration);                       
                    }               
                });

                $.ajax({
                    type: "GET",
                    url: settings.baseUrl + "/GetInfoDetails",
                    //url: '@Url.Action("GetSingleData", "Con_VoucherEntry")',
                    data: { VoucherNo: VoucherNo },
                    dataType: "json",
                    success: function (data) {
                     
                        if (data != null) {
                            OrderItems = [];
                            for (var i = 0; i < data.length; i++) {
                              
                                OrderItems.push({
                                    AccCode: data[i]["accCode"],
                                    AccountHead: data[i]["accountHead"],
                                    Description: data[i]["description"],
                                    DebitAmount: data[i]["debitAmount"],
                                    CreditAmount: data[i]["creditAmount"],
                                    TrType: data[i]["trType"],
                                    ChequeNo: data[i]["chequeNo"],
                                    ChequeDate: data[i]["chequeDate"]
                                });
                            }
                            GeneratedTempGrid();
                        }
                    }
                });

                
                $("html, body").animate({ scrollTop: 0 }, 500);
         
               
            });
            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                Clear();
                $(".divRecieved").hide();
                $(".headDetails").hide();

                var d = new Date();
                var month = d.getMonth() + 1;
                var day = d.getDate();

                var output = (day < 10 ? '0' : '') + day + '/' +
                    (month < 10 ? '0' : '') + month + '/' + d.getFullYear();
                $('#VoucherDate').val(output);
                $('#txtFromDate').val('01' + '/' + (month < 10 ? '0' : '') + month + '/' + d.getFullYear());
                $('#txtToDate').val(output);
          
                $("html, body").animate({ scrollTop: 0 }, 500);

            });
            $("body").on("click", '#btnTempSubmit', function (e) {
                e.preventDefault();

                if ($("#ddlTrType").val() == "")
                {
                    var message = $("#TempMessage");
                    message.css("color", "red");
                    message.html("Please Select Tr Type");
                    return;
                }
                if ($("#ddlAccCode").val() == "") {
                    var message = $("#TempMessage");
                    message.css("color", "red");
                    message.html("Please Select Account Head");
                    return;
                }
                if ($("#txtDescription").val() == "") {
                    var message = $("#TempMessage");
                    message.css("color", "red");
                    message.html("Please Enter Description");
                    return;
                }
                if ($("#txtAmount").val() == "") {
                    var message = $("#TempMessage");
                    message.css("color", "red");
                    message.html("Please Enter Amount");
                    return;
                }

                if ($("#txtChequeNo").val() == "") {
                    $("#txtChequeNo").val("");
                }

                var ConvertDate = "";
                var chequeDate = $("#txtChequeDate").val();
                if (chequeDate=== "") {
                    
                    
                }
                else {
                     ConvertDate = $("#txtChequeDate").val();                 
                }

                var DebitAmount = 0;
                if ($("#ddlTrType").val() == "Dr") {
                    DebitAmount = $("#txtAmount").val();
                }
                var CreditAmount = 0;
                if ($("#ddlTrType").val() == "Cr") {
                    CreditAmount = $("#txtAmount").val();
                }

                if ($("#lblTempID").text() == "") {
                    var Exis = "0";
                    $('table tr td #lnkIDTemp').each(function () {
                        var value = $(this).text();
                        if (value == $('#ddlAccCode').val()) {
                            Exis = "1";
                        }
                    });
                    if (Exis == "1") {
                        var message = $("#TempMessage");
                        message.css("color", "red");
                        message.html("Already Exists");
                        return;
                    }
                }

                else {
                    var Exis = "0";
                    $('table tr td #lnkIDTemp').each(function () {
                        var value = $(this).text();
                        if (value == $('#ddlAccCode').val()) {
                            Exis = $('#ddlAccCode').val();
                        }
                    });

                    for (var i = 0; i < OrderItems.length; i++) {
                        if (OrderItems[i].AccCode == Exis) {
                            OrderItems.splice(i, 1);
                        }
                    }
                }

                var data = $('#ddlAccCode').select2('data');

                OrderItems.push({
                    AccCode: $('#ddlAccCode').val().trim(),
                    AccountHead: data[0].text,
                    TrType: $('#ddlTrType').val(),
                    Description: $('#txtDescription').val().trim(),
                    DebitAmount: DebitAmount,
                    CreditAmount: CreditAmount,
                    ChequeNo: $('#txtChequeNo').val().trim(),
                    ChequeDate: ConvertDate
                });
                GeneratedTempGrid();
                TempClear();

            });

            function TempClear() {

                $("#ddlAccCode").val("").change();
                $("#ddlTrType").val("").change();
                $("#txtDescription").val("");
                $("#txtAmount").val("");
                $("#txtChequeNo").val("");
                $("#txtChequeDate").val("");
                $("#TempMessage").html("");
                $('#lblTempID').text("");
            }
            // Save
            $("body").on("click", settings.saveSelector, function () {



                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }
                ;
                if ($(".TempGridTable  tbody tr").length == 0) {
                    toastr.info("Please add Voucher Data First.", 'Info');
                    return false;
                }
                var TotalDr = 0;
                $('table tr td #lblDebitAmount').each(function () {

                    var value = $(this).text();
                    if (isNaN(parseFloat(value)) == false) {
                        TotalDr = TotalDr + parseFloat(value);
                    }
                }, null);


                var TotalCr = 0;
                $('table tr td #lblCreditAmount').each(function () {

                    var value = $(this).text();
                    if (isNaN(parseFloat(value)) == false) {
                        TotalCr = TotalCr + parseFloat(value);
                    }
                }, null);


                if (parseFloat(TotalDr) != parseFloat(TotalCr)) {
                    toastr.info("Debit Amount And Crdit Amount are not equal", 'Info');
                    return false;
                }


                var data = {
                    Main_CompanyCode: $("#Main_CompanyCode").val(),
                    VoucherType_Code: $("#VoucherType_Code").val(),
                    VoucherDate: $("#VoucherDate").val(),
                    VoucherNo: $("#VoucherNo").val(),
                    Narration: $("#Narration").val(),

                    voucherDetails: OrderItems
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
                    //dataType: "json",
                    //contentType: "application/json",
                    success: function (data)
                    {
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

            $("body").on("click", settings.selectAllSelector, function ()
            {
                $(".checkBox").prop('checked',
                    $(this).prop('checked'));
            });
            function Clear() {

                $("#VoucherType_Code").val("").change();
                $("#VoucherNo").val("");
                $("#Narration").val("");
                TempClear();
                OrderItems = [];
                GeneratedTempGrid();

            }

            
            $("body").on("change", ".VoucherType_Code", function ()
            {               
                var VoucherType_Code = $("#VoucherType_Code").val();
                if ($("#VoucherType_Code").val() != "") {

                    var hidValue = $("#lblhidValue").text();
                    if (hidValue == " ") {
                        hidValue = "";
                    }
                    if (hidValue == "") {
                        MaxID();
                    }
                    else {
                       

                    }

                   
                    if (VoucherType_Code == "1") {
                        $(".divRecieved").show();
                    }
                    else {
                        $(".divRecieved").hide();
                    }

                }
                else {
                    $("#VoucherNo").val("");
                    $(".divRecieved").hide();
                
                }
            });



            $("body").on("change", ".AccCode", function () {
               

                var SubSusidiaryLedgerCodeNo = $("#ddlAccCode").val();

                if ($("#ddlAccCode").val() != "") {
                    $('.headDetails').show();
                    $.ajax({
                        url: settings.baseUrl + "/GetAccountHeadDetails",
                        method: "GET",
                        data: { SubSusidiaryLedgerCodeNo: SubSusidiaryLedgerCodeNo },
                        dataType: 'json',
                        success: function (data) {
                          
                            $("#lblGroupLedger").text(data[0]["controlLedgerName"]);
                            $("#lblControlLedger").text(data[0]["subControlLedgerName"]);
                            $("#lblSubControlLedger").text(data[0]["generalLedgerName"]);
                            $("#lblSubsidiaryLedger").text(data[0]["subsidiaryLedgerName"]);
                            $("#txtDescription").val(data[0]["subSubsidiaryLedgerName"]);
                        }
                    });
            
                }
                else {
                    $("#lblGroupLedger").text("");
                    $("#lblControlLedger").text("");
                    $("#lblSubControlLedger").text("");
                    $("#lblSubsidiaryLedger").text("");
                    $("#txtDescription").val("");
                    $('.headDetails').hide();
                   
                }
            });


            $("body").on("click", ".lnkIDTempTest", function (e) {
                e.preventDefault();

                var a = $(this).text();
                $('#lblTempID').text(a);
                $('#ddlAccCode').val(a).change();

                var Description = $(this).closest('td').next('td').next('td').html();
                $("#txtDescription").val(Description);
                var DrAmount = $(this).closest('td').next('td').next('td').next('td').find('#lblDebitAmount').text();
                var CrAmount = $(this).closest('td').next('td').next('td').next('td').next('td').find('#lblCreditAmount').text();
                if (parseFloat(DrAmount) > parseFloat(CrAmount)) {
                    $("#txtAmount").val(DrAmount);
                }
                else {
                    $("#txtAmount").val(CrAmount);
                }
                var TrT = $(this).closest('td').next('td').next('td').next('td').next('td').next('td').html();
                $('#ddlTrType').val(TrT).change();
            });

            $("body").on("click", "#btnTempDelete", function (e) {
                e.preventDefault();

                var Exis = "0";
                $('table tr td #lnkIDTemp').each(function () {
                    var value = $(this).text();
                    if (value == $("#ddlAccCode").val()) {
                        Exis = $("#ddlAccCode").val();
                    }
                });

                for (var i = 0; i < OrderItems.length; i++) {
                    if (OrderItems[i].AccCode == Exis) {
                        OrderItems.splice(i, 1);
                    }
                }
                GeneratedTempGrid();
                TempClear();

            });
            $("body").on("click", "#btnTempClear", function (e) {
                e.preventDefault();
                GeneratedTempGrid();
                TempClear();

            });
            $("body").on("click", ".js-voucherEntry-GridPreivew", function (e) {
                e.preventDefault();
                LoadGridData(settings.baseUrl, settings.gridSelector);
            });

            $("body").on("click", ".js-voucherEntry-print", function () {
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
                                voucherNo: item,
                                reportType: "VoucherEntryReport",
                                reportRenderType: reportRenderType,
                                isPreview: true
                            },
                            success: function (response) {
                                window.open(
                                    normalizeUrl(getBaseUrl()) + `/Preview/VoucherEntryReport`,
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
                //event.preventDefault();
                // Get button that triggered the modal
                var source = $(event.relatedTarget);
                var id = source.data("id");

                // Extract value from data-* attributes
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
                                /*settings.load(settings.baseUrl);*/
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
                var VoucherType_Code = $("#VoucherType_Code").val();

                $("#SubSusidiaryLedgerCodeNo").val("");
                if (VoucherType_Code != "") {
                    $.ajax({
                        type: "GET",
                        url: settings.baseUrl + "/MaxID",
                        data: { VoucherType_Code: VoucherType_Code },
                        dataType: 'json',
                        success: function (response) {
                            $('#VoucherNo').val(response);
                        }
                    })
                }
                else
                {
                    $("#VoucherNo").val("");

                }
            }        
        });

    


        var OrderItems = [];
       
        function GeneratedTempGrid() {
            if (OrderItems.length > 0) {
                var $table = $('<table class="table TempGridTable  table-striped table-bordered"/>')
                $table.append('<thead><tr><th style="text-align:center">Acc Code</th><th style="text-align:center">Account Head</th><th style="text-align:center">Description</th><th style="text-align:center">Debit</th><th style="text-align:center">Credit</th><th style="display:none">TrType</th><th style="display:none">Cheqe No</th><th style="display:none">cheqe date</th></tr></thead>');
                var $tbody = $('<tbody/>');
                $.each(OrderItems, function (i, val) {
                    var $row = $('<tr/>');
                    $row.append($('<td  style="text-align:center"/>').html("<a class='lnkIDTempTest pointTer' id='lnkIDTemp'>" + val.AccCode + "</a>"));
                    $row.append($('<td style = "text-align:left"/>').html(val.AccountHead));
                    $row.append($('<td style = "text-align:left"/>').html(val.Description));
                    $row.append($('<td style="text-align:right"/>').html("<label  id='lblDebitAmount'>" + val.DebitAmount + "</label>"));
                    $row.append($('<td style="text-align:right"/>').html("<label  id='lblCreditAmount'>" + val.CreditAmount + "</label>"));
                    $row.append($('<td style="display:none"/>').html(val.TrType));
                    $row.append($('<td style="display:none"/>').html(val.ChequeNo));
                    $row.append($('<td style="display:none" />').html(val.ChequeDate));

                    $tbody.append($row);
                });
                $table.append($tbody);
                $('#orderItems').html($table);
            }

            else {

                var $table = $('<table class="table  table-striped table-bordered"/>')
                $table.append('<thead><tr><th style="text-align:center">Acc Code</th><th style="text-align:center">Account Head</th><th style="text-align:center">Description</th><th style="text-align:center">Debit</th><th style="text-align:center">Credit</th></tr></thead>');
                var $tbody = $('<tbody/>')
                var $row = $('<tr/>');
                //$row.append($('<td/>').html(val.itemID));
                $row.append($('<td  style="text-align:center"/>').html(""));
                $row.append($('<td style="text-align:center"/>').html(""));
                $row.append($('<td style="text-align:center"/>').html(""));
                $row.append($('<td style="text-align:center"/>').html(""));
                $row.append($('<td style="text-align:center"/>').html(""));

                $tbody.append($row);
                $table.append($tbody);
                $('#orderItems').html($table);
            }
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
                dropdownAutoWidth:true,
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
            var ToDate = $('#txtToDate').val();
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
                        "data": "voucherNo", "className": "text-center", width: "2%",
                        "render": function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "voucherNo", width: "5%", "render": function (data) {
                            return `<a class='btn js-voucherEntry-edit' style='color:blue' data-id='${data}'><i>${data}</i></a>`;
                        }
                    },
                    { "data": "voucherDate", "autowidth": true, "className": "text-center" },
                    { "data": "voucherType_Code", "autowidth": true, "className": "text-center" },
                    { "data": "narration", "autowidth": true, "className": "text-center" },
                    { "data": "totalAmount", "autowidth": true, "className": "text-center" },
                    { "data": "invoiceNo", "autowidth": true, "className": "text-center" }
                    
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

