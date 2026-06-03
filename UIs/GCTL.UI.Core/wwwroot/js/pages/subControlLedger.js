(function ($) {
    $.subControlLedger = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#subControlLedger-form",
            formContainer: ".js-subControlLedger-form-container",
            gridSelector: "#subControlLedger-grid",
            gridContainer: ".js-subControlLedger-grid-container",
            editSelector: ".js-subControlLedger-edit",
            saveSelector: ".js-subControlLedger-save",
            selectAllSelector: "#subControlLedger-check-all",
            deleteSelector: ".js-subControlLedger-delete-confirm",
            deleteModal: "#subControlLedger-delete-modal",
            finalDeleteSelector: ".js-subControlLedger-delete",
            clearSelector: ".js-subControlLedger-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-subControlLedger-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-subControlLedger-check-availability",
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
            $("body").on("click", `${settings.editSelector},${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = saveUrl + "/" + $(this).data("id") ?? "";
                loadForm(url);

                $("html, body").animate({ scrollTop: 0 }, 500);

                //var ControlLedgerCodeNo = $("#ControlLedgerCodeNo").val();

                //if ($("#ControlLedgerCodeNo").val() != "") {
                //    $("#txtGRLCode").val(ControlLedgerCodeNo);
                //}
                //else {
                //    $("#txtGRLCode").val("");
                //}

            });

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
                
                    $("#SubControlLedgerCodeNo").val("");
                    $("#SubControlLedgerName").val("");
                    $("#ShortName").val("");
                }
            
            $("body").on("change", ".ControlLedgerCodeNo", function () {
                LoadGridData(settings.baseUrl, settings.gridSelector);

                var ControlLedgerCodeNo = $("#ControlLedgerCodeNo").val();

                if ($("#ControlLedgerCodeNo").val() != "") {
                    $("#txtGRLCode").val(ControlLedgerCodeNo);
                    MaxID();
                }
                else {
                    $("#txtGRLCode").val("");
                    $("#SubControlLedgerCodeNo").val("");

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
                var ControlLedgerCodeNo = $("#ControlLedgerCodeNo").val();

                $("#SubControlLedgerCodeNo").val("");
                if (ControlLedgerCodeNo != "") {
                    $.ajax({
                        type: "GET",
                        url: settings.baseUrl + "/MaxID",
                        data: { ControlLedgerCodeNo: ControlLedgerCodeNo },
                        dataType: 'json',
                        success: function (response) {
                            $('#SubControlLedgerCodeNo').val(response);

                        }
                    })
                }
                else
                {
                    $("#SubControlLedgerCodeNo").val("");

                }
            }

            //$("body").on("keyup", settings.availabilitySelector, function () {
            //    var self = $(this);
            //    let code = $(".js-controlLedger-code").val();
            //    let name = self.val();

            //    // check
            //    $.ajax({
            //        url: settings.baseUrl + "/CheckAvailability",
            //        method: "POST",
            //        data: { code: code, name: name },
            //        success: function (response) {
            //            console.log(response);
            //            if (response.isSuccess) {
            //                toastr.error(response.message);
            //            }
            //        }
            //    });
            //});


        });

        function LoadGridData(baseUrl, gridSelector) {
            var ControlLedgerCodeNo = $("#ControlLedgerCodeNo").val();
            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/grid",
                    type: "GET",
                    datatype: "json",
                    data: { ControlLedgerCodeNo: ControlLedgerCodeNo }
                },

                columnDefs: [
                    { targets: [0], orderable: false },
                ],
                columns: [
                    {
                        "data": "subControlLedgerCodeNo", "className": "text-center", width: "2%",
                        "render": function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "subControlLedgerCodeNo", width:"5%", "render": function (data) {
                            return `<a class='btn js-subControlLedger-edit' style='color:blue' data-id='${data}'><i>${data}</i></a>`;
                        }
                    },
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

