(function ($) {
    $.generalLedger = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#generalLedger-form",
            formContainer: ".js-generalLedger-form-container",
            gridSelector: "#generalLedger-grid",
            gridContainer: ".js-generalLedger-grid-container",
            editSelector: ".js-generalLedger-edit",
            saveSelector: ".js-generalLedger-save",
            selectAllSelector: "#generalLedger-check-all",
            deleteSelector: ".js-generalLedger-delete-confirm",
            deleteModal: "#generalLedger-delete-modal",
            finalDeleteSelector: ".js-generalLedger-delete",
            clearSelector: ".js-generalLedger-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-generalLedger-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-generalLedger-check-availability",
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
            LoadGridData(settings.baseUrl, settings.gridSelector);
            initialize();
            $("body").on("click", `${settings.editSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = saveUrl + "/" + $(this).data("id") ?? "";
                loadForm(url);

                $("html, body").animate({ scrollTop: 0 }, 500);
         
               
            });
            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = saveUrl + "/" + $(this).data("id") ?? "";
                loadFormClear(url);

                $("html, body").animate({ scrollTop: 0 }, 500);
                $("#ControlLedgerCodeNo").prop("disabled", false);
                $("#SubControlLedgerCodeNo").prop("disabled", false);

            });


            // Save
            $("body").on("click", settings.saveSelector, function () {

                $("#ControlLedgerCodeNo").prop("disabled", false);
                $("#SubControlLedgerCodeNo").prop("disabled", false);

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

                            LoadGridData(settings.baseUrl, settings.gridSelector);
                            Clear();
                            MaxID();
                            toastr.success(response.success, 'Success');
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
            function Clear() {
                
                $("#GeneralLedgerName").val("");
               $("#ShortName").val("");
            }

            
            $("body").on("change", ".ControlLedgerCodeNo", function () {
                LoadGridData(settings.baseUrl, settings.gridSelector);

                var ControlLedgerCodeNo = $("#ControlLedgerCodeNo").val();

                if ($("#ControlLedgerCodeNo").val() != "") {
                    $("#txtGRLCode").val(ControlLedgerCodeNo);
                

                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetSubControlLedger",
                        method: "POST",
                        data: { ControlLedgerCodeNo: ControlLedgerCodeNo },
                        success: function (response) {
                            $("#SubControlLedgerCodeNo").empty();
                            $("#SubControlLedgerCodeNo").append($('<option>', {
                                value: '',
                                text: `Select CL Name`
                            }));
                            $.each(response, function (i, item) {
                                $("#SubControlLedgerCodeNo").append($('<option>', {
                                    value: item.code,
                                    text: item.name
                                }));
                            });

                          
                        }
                    });

                }
                else {
                    $("#txtGRLCode").val("");
                    $("#SubControlLedgerCodeNo").val("");
                }
            });

            $("body").on("change", ".SubControlLedgerCodeNo", function () {
                LoadGridData(settings.baseUrl, settings.gridSelector);

                var SubControlLedgerCodeNo = $("#SubControlLedgerCodeNo").val();

                if ($("#SubControlLedgerCodeNo").val() != "") {
                    $("#txtClCode").val(SubControlLedgerCodeNo);
                    MaxID();
                }
                else {
                    $("#txtClCode").val("");
                    $("#GeneralLedgerCodeNo").val("");
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
                                LoadGridData(settings.baseUrl, settings.gridSelector);
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


            $("body").on("keyup", settings.decimalSelector, function () {
                var self = $(this);
                showDecimalPlaces(self.val(), self.parent().find(".input-group-text"));
            });
            function MaxID() {
                var SubControlLedgerCodeNo = $("#SubControlLedgerCodeNo").val();

                $("#GeneralLedgerCodeNo").val("");
                if (SubControlLedgerCodeNo != "") {
                    $.ajax({
                        type: "GET",
                        url: settings.baseUrl + "/MaxID",
                        data: { SubControlLedgerCodeNo: SubControlLedgerCodeNo },
                        dataType: 'json',
                        success: function (response) {
                            $('#GeneralLedgerCodeNo').val(response);

                        }
                    })
                }
                else
                {
                    $("#GeneralLedgerCodeNo").val("");

                }
            }

            

        });

        function LoadGridData(baseUrl, gridSelector) {
            var ControlLedgerCodeNo = $("#ControlLedgerCodeNo").val();
            var SubControlLedgerCodeNo = $("#SubControlLedgerCodeNo").val();

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/grid",
                    type: "GET",
                    datatype: "json",
                    data: { ControlLedgerCodeNo: ControlLedgerCodeNo, SubControlLedgerCodeNo: SubControlLedgerCodeNo }
                },

                columnDefs: [
                    { targets: [0], orderable: false },
                ],
                columns: [
                    {
                        "data": "generalLedgerCodeNo", "className": "text-center", width: "2%",
                        "render": function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "generalLedgerCodeNo", width:"5%", "render": function (data) {
                            return `<a class='btn js-generalLedger-edit' style='color:blue' data-id='${data}'><i>${data}</i></a>`;
                        }
                    },
                    { "data": "generalLedgerName", "autowidth": true, "className": "text-center" },
                    { "data": "subControlLedgerName", "autowidth": true, "className": "text-center" },
                    { "data": "controlLedgerName", "autowidth": true, "className": "text-center" },
                
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
                        var ControlLedgerCodeNo = $("#ControlLedgerCodeNo").val();
                        $("#txtGRLCode").val(ControlLedgerCodeNo);
                       
                        var SubControlLedgerCodeNo = $("#SubControlLedgerCodeNo").val();
                        $("#txtClCode").val(SubControlLedgerCodeNo);
                        $("#ControlLedgerCodeNo").prop("disabled", true);
                        $("#SubControlLedgerCodeNo").prop("disabled", true);
                    },
                    error: function (error) {
                        reject(error)
                    },
                })
            })
        }
        function loadFormClear(url) {
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
                        var ControlLedgerCodeNo = $("#ControlLedgerCodeNo").val();
                        $("#txtGRLCode").val(ControlLedgerCodeNo);

                        var SubControlLedgerCodeNo = $("#SubControlLedgerCodeNo").val();
                        $("#txtClCode").val(SubControlLedgerCodeNo);
                        $("#ControlLedgerCodeNo").prop("disabled", false);
                        $("#SubControlLedgerCodeNo").prop("disabled", false);
                    },
                    error: function (error) {
                        reject(error)
                    },
                })
            })
        }
        function initialize() {
            $(settings.formSelector + ' .selectpicker').select2({
                language: {
                    noResults: function () {
                        //return 'Not found <a class="add_new_item" href="javascript:void(0)">Add New</a>';
                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });
        }
    }

}(jQuery));

