(function ($) {
    $.companies = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#company-form",
            formContainer: ".js-company-form-container",
            gridSelector: "#company-grid",
            gridContainer: ".js-company-grid-container",
            editSelector: ".js-company-edit",
            saveSelector: ".js-company-save",
            selectAllSelector: "#company-check-all",
            deleteSelector: ".js-company-delete-confirm",
            deleteModal: "#company-delete-modal",
            finalDeleteSelector: ".js-company-delete",
            clearSelector: ".js-company-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-company-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-company-check-availability",
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
            loadCompanies(settings.baseUrl, settings.gridSelector);
            initialize();
            $("body").on("click", `${settings.editSelector},${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = saveUrl + "/" + $(this).data("id") ?? "";
                loadForm(url);

                // Setting id on the delete selector for delete
                var id = $(this).data('id');
                $(settings.deleteSelector).data('id', id);

                $("html, body").animate({ scrollTop: 0 }, 500);
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
                            loadForm(saveUrl)
                                .then((data) => {
                                    loadCompanies(settings.baseUrl, settings.gridSelector);
                                    $(settings.lastCodeSelector).val(response.lastCode);
                                    //alert($(settings.lastCodeSelector).val());
                                })
                                .catch((error) => {
                                    console.log(error)
                                })

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
                                loadCompanies(settings.baseUrl, settings.gridSelector);
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

            $("body").on("keyup", settings.availabilitySelector, function () {
                var self = $(this);
                let code = $(".js-code").val();
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
        });

        function loadCompanies(baseUrl, gridSelector) {
            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/grid",
                    type: "GET",
                    datatype: "json"
                },

                columnDefs: [
                    { targets: [0], orderable: false },
                ],
                columns: [
                    {
                        "data": "companyCode", "className": "text-center", "render": function (data) {
                            return ` <input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "companyCode", "render": function (data) {
                            return `<a class='btn js-company-edit' data-id='${data}'><i>${data}</i></a>`;
                        }
                    },
                    { "data": "companyName", "autowidth": true },
                    { "data": "address1", "autowidth": true },
                    { "data": "phone1", "className": "text-center", "autowidth": true },
                    { "data": "email", "className": "text-center", "autowidth": true },
                ],
                lengthChange: true,
                pageLength: 10,
                lengthMenu: [
                    [10, 25, 50, -1],
                    [10, 25, 50, 'All'],
                ],
                order: [[1, "Asc"]],
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

            $(".phone-validation").AppNumericOnly();

            showDecimalPlaces($(settings.decimalSelector).val(), $(settings.decimalSelector).parent().find(".input-group-text"));
        }

        function showDecimalPlaces(selector, target) {
            let places = parseInt(selector);
            let length = places > settings.maxDecimalPlace ? settings.maxDecimalPlace : places, numberOfDecimals = "";
            if (places > 0) {
                numberOfDecimals = "1.";
                for (var i = 1; i <= length; i++) {
                    numberOfDecimals += i;
                }
            } else {
                if (places == 0)
                    numberOfDecimals = "1";
                else
                    numberOfDecimals = "";
            }

            $(target).text(numberOfDecimals);

            if (settings.showNagativeFormat && numberOfDecimals.length > 0) {
                var symbol = $(".symbol").val();
                var options = [];
                options.push(`<option>-${numberOfDecimals} ${symbol}</option>`);
                options.push(`<option>(${numberOfDecimals}) ${symbol}</opttion>`);

                $(".negative-format").empty();
                $(".negative-format").append(options);
            }
        }
    }

}(jQuery));

