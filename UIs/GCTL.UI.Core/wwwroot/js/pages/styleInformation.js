(function ($) {
    $.styleInformation = function (options) {

        //#region Default options

        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#StyleInformation-form",
            formContainer: ".js-StyleInformation-form-container",
            gridSelector: "#StyleInformation-grid",
            gridContainer: ".js-StyleInformation-grid-container",
            editSelector: ".js-StyleInformation-edit",
            saveSelector: ".js-StyleInformation-save",
            selectAllSelector: "#StyleInformation-check-all",
            deleteSelector: ".js-StyleInformation-delete-confirm",
            deleteModal: "#StyleInformation-delete-modal",
            finalDeleteSelector: ".js-StyleInformation-delete",
            clearSelector: ".js-StyleInformation-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-StyleInformation-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-StyleInformation-check-availability",
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

        //#endregion

        $(() => {

            select2DD();
            loadTable();

            //#region editSelector & clearSelector

            $("body").on("click", `${settings.editSelector},${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
               
                let url = saveUrl + ($(this).data("id") ? "/" + $(this).data("id") : "");

                loadForm(url).then((data) => {
                    console.info("Form Loaded Successfully", data);
                }).catch((error) => {
                    console.error("Failed to load form", error);
                });

                // Setting id on the delete selector for delete
                var id = $(this).data('id');
                $(settings.deleteSelector).data('id', id);
                $("html, body").animate({ scrollTop: 0 }, 500);
                
            });

            //#endregion

            //#region Save

            $("body").on("click", settings.saveSelector, function () {
                validation();
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
                       // console.log(response);                       
                        if (response.isSuccess) {
                           // console.log(response.lastCode);
                            $("#StyleId").val((parseInt(response.lastCode) + 1).toString().padStart(3, '0'));
                            select2DD();                          
                            let bId = $("#BuyerId").val();
                            $("#BuyerId").val(bId).trigger('change');
                            $("#Style").val('');
                            $("#ShortName").val('');
                            loadTable();
                            
                            toastr.success(response.message);
                        }
                        else {
                            toastr.error(response.message);
                            console.log(response);
                            select2DD();
                        }
                    }
                }
                if (settings.haseFile) {
                    options.processData = false;
                    options.contentType = false;
                }
                $.ajax(options);
            });

            //#endregion

            //#region selectAllSelector & deleteSelector

            $("body").on("click", settings.selectAllSelector, function () {
                $(".checkBox").prop('checked',
                    $(this).prop('checked'));
            });

            $("body").on("click", settings.deleteSelector, function (e) {
                e.preventDefault();
                if ($(this).data('id')) {
                    selectedItems.push($(this).data('id'));
                } else {
                    $('input:checkbox.checkBox').each(function () {
                        if ($(this).prop('checked')) {
                            if (!selectedItems.includes($(this).val())) {
                                selectedItems.push($(this).val());
                            }
                        }
                    });
                }

                if (selectedItems.length > 0) {
                    $(settings.deleteModal).modal("show");
                } else {
                    toastr.info("Please select at least one item.");
                }
            });

            //#endregion

            //#region Delete & deleteModal

            $("body").on('show.bs.modal', settings.deleteModal, function (event) {

                var source = $(event.relatedTarget);
                var id = source.data("ids");

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
                        url: deleteUrl,
                        method: "POST",
                        contentType: "application/json",
                        data: JSON.stringify(selectedItems),
                        success: function (response) {
                           /// console.log(response);
                            $(modal).modal("hide");
                            if (response.success) {
                                toastr.success(response.message);
                                selectedItems = [];

                                loadTable();
                                let bId = $("#BuyerId").val();
                                $("#BuyerId").val(bId).trigger('change');
                                $("#Style").val('');
                                $("#ShortName").val('');

                               // loadForm(saveUrl);
                            }
                            else {
                                toastr.error(response.message);
                                console.log(response);
                            }
                        }
                    });
                });

            }).on('hide.bs.modal', function () {
                $("body").off("click", settings.finalDeleteSelector);
            });

            //#endregion

            //#region topSelector & decimalSelector

            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 0 }, 500);
            });


            $("body").on("keyup", settings.decimalSelector, function () {
                var self = $(this);
                showDecimalPlaces(self.val(), self.parent().find(".input-group-text"));
            });

            //#endregion

            //#region availabilitySelector

            $("body").on("keyup", settings.availabilitySelector, function () {
                var self = $(this);
                let code = $(".js-StyleId-code").val();
                let name = self.val();
                // check
                $.ajax({
                    url: settings.baseUrl + "/CheckAvailability",
                    method: "POST",
                    data: { code: code, name: name },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.error(response.message);
                        }
                    }
                });
            });

            //#endregion

        });

        //#region loadTable

        $(document).on('change', "#BuyerId", function () {
            
            var id = $(this).val();
            if (id) {  // Only load if a value is selected
                $.get(settings.baseUrl + "/GetTableData", { id: id })
                    .done(html => {
                        $(settings.gridContainer).html(html);
                        if ($.fn.DataTable.isDataTable(settings.gridSelector)) {
                            $(settings.gridSelector).DataTable().destroy();
                        }
                        $(settings.gridSelector).DataTable({
                            lengthChange: true,
                            pageLength: 10,
                            lengthMenu: [
                                [10, 25, 50, -1],
                                [10, 25, 50, 'All'],
                            ],
                            order: [[1, "desc"]],
                            destroy: true,
                            paging: true,
                            searching: true,
                            responsive: true,
                        });
                    })
                    .fail(() => toastr.error("Failed to load table data."));
            }
        });
        //loadTable();
        function loadTable() {
            $.get(settings.baseUrl + "/GetTableData")
                .done(html => {
                    //$(settings.gridContainer).html(html);
                    if ($.fn.DataTable.isDataTable(settings.gridSelector)) {
                        $(settings.gridSelector).DataTable().destroy();
                    }
                    $(settings.gridSelector).DataTable({
                        lengthChange: true,
                        pageLength: 10,
                        lengthMenu: [
                            [10, 25, 50, -1],
                            [10, 25, 50, 'All'],
                        ],
                        order: [[1, "desc"]],
                        destroy: true,
                        paging: true,
                        searching: true,
                        responsive: true,
                    });
                })
                .fail(() => toastr.error("Failed to load table data."));
        }
        
        //#endregion

        //#region loadForm

        function loadForm(url) {
            return new Promise((resolve, reject) => {
                $.ajax({
                    url: url,
                    type: 'GET',
                    cache: false,
                    success: function (data) {
                        //console.log(data);
                        $(settings.formContainer).empty();
                        $(settings.formContainer).html(data);
                        $.validator.unobtrusive.parse($(settings.formSelector));

                        select2DD();
                        resolve(data)
                    },
                    error: function (error) {
                        reject(error)
                    },
                })
            })
        }

        //#endregion

        //#region validation

        function validation() {

            var buy = $('#BuyerId').val();
            var sty = $('#Style').val();

            if (!buy) {
                toastr.info('select Buyer');
                $('#BuyerId').select2('open');
                return false;
            }

            //if (!buy) {
            //    toastr.info('Select Buyer');
            //    $('#BuyerId').parent().find('button.dropdown-toggle').dropdown('toggle');
            //    return false;
            //}

            if (!sty) {
                toastr.info('Enter Style');
                $('#Style').trigger('focus');
                return false;
            }
            return true;
        }

        //#endregion

        //#region select2
        function select2DD() {
            $('.selectpickers').select2({
                language: {
                    noResults: function () { return "No results found"; }
                },
                escapeMarkup: function (markup) { return markup; }
            });
        }
        //#endregion

    }

}(jQuery));