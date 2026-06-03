(function ($) {
    $.paymentTerms = function (options) {

        //#region Default options

        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#PaymentTerms-form",
            formContainer: ".js-PaymentTerms-form-container",
            gridSelector: "#PaymentTerms-grid",
            gridContainer: ".js-PaymentTerms-grid-container",
            editSelector: ".js-PaymentTerms-edit",
            saveSelector: ".js-PaymentTerms-save",
            selectAllSelector: "#PaymentTerms-check-all",
            deleteSelector: ".js-PaymentTerms-delete-confirm",
            deleteModal: "#PaymentTerms-delete-modal",
            finalDeleteSelector: ".js-PaymentTerms-delete",
            clearSelector: ".js-PaymentTerms-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-PaymentTerms-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-PaymentTerms-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            Percentise:"#Percentise",
            CreditDays:"#CreditDays",
            PaymentTermsName:"#PaymentTermsName",
            load: function () {

            }
        }, options);


        var gridUrl = settings.baseUrl + "/grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];

        //#endregion

        $(() => {

            $(settings.CreditDays).prop("disabled", true);
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
                        if (response.isSuccess) {
                            loadForm(saveUrl)

                                .then((data) => {
                                    select2DD();
                                    loadTable();
                                    $(settings.lastCodeSelector).val(response.lastCode);
                                    // alert("The last code has been updated to: " + response.lastCode);
                                })
                                .catch((error) => {
                                    console.log(error)
                                })

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
                            console.log(response);
                            $(modal).modal("hide");
                            if (response.success) {
                                toastr.success(response.message);
                                selectedItems = [];

                                loadTable();
                                loadForm(saveUrl);
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
                let code = $(".js-PaymentTermsId-code").val();
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

        //#region PaymentTermsName Calculating

        $(document).on('input', settings.Percentise, function () {          
            let per = $(this).val();
            if (per == 100) {
                $(settings.CreditDays).val("");
                $(settings.CreditDays).prop("disabled", true);
                $(settings.PaymentTermsName).val("100 % adv.");
                $(settings.Percentise).removeClass('border-danger');              
            } else if (per > 100) { 
                $(settings.PaymentTermsName).val('');
                toastr.info("Percentise In Less Then 100");
                $(settings.CreditDays).prop("disabled", true);
                $(settings.Percentise).addClass('border-danger');
            } else if (per >= 0 && per <= 100) {              
                $(settings.PaymentTermsName).val(`${per}% adv.`);
                $(settings.CreditDays).prop("disabled", false);
                $(settings.Percentise).removeClass('border-danger');
            } else if (per < 0) {               
                $(settings.CreditDays).prop("disabled", true);
                $(settings.Percentise).addClass('border-danger');
                $(settings.PaymentTermsName).val('');
            }

            if (per == '') {
                $(settings.CreditDays).prop("disabled", true);
                $(settings.PaymentTermsName).val('');
            }
            let day = $(settings.CreditDays).val();
            if (day != "" && per != "") {
                $(settings.PaymentTermsName).val(`${per}% adv. + ${100 - per}%Cr. ( ${day} Days)`);
            }
        })

        $(document).on('input', settings.CreditDays, function () {
            let day = $(this).val();
            let per = $(settings.Percentise).val();
            $(settings.PaymentTermsName).val(`${per}% adv. + ${100-per}%Cr. ( ${day} Days)`);
        })

        //#endregion

        //#region loadTable
        function loadTable() {
            $.get(settings.baseUrl + "/GetTableData")
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

        //#endregion

        //#region loadForm

        function loadForm(url) {
            return new Promise((resolve, reject) => {
                $.ajax({
                    url: url,
                    type: 'GET',
                    cache: false,
                    success: function (data) {
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

            var buy = $('#Type').val();
            var sty = $('#Percentise').val();

            if (!buy) {
                toastr.info('select Type');
                $('#Type').select2('open');
                return false;
            }

            if (!sty) {
                toastr.info('Enter Percentise');
                $('#Percentise').trigger('focus');
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