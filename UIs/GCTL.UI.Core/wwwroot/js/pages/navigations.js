(function ($) {
    $.navigations = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#navigation-form",
            formContainer: ".js-navigation-form-container",
            gridSelector: "#navigation-grid",
            gridContainer: ".js-navigation-grid-container",
            editSelector: ".js-navigation-edit",
            saveSelector: ".js-navigation-save",
            selectAllSelector: "#navigation-check-all",
            deleteSelector: ".js-navigation-delete-confirm",
            deleteModal: "#navigation-delete-modal",
            finalDeleteSelector: ".js-navigation-delete",
            clearSelector: ".js-navigation-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-navigation-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-navigation-check-availability",
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
            loadNavigations(settings.baseUrl, settings.gridSelector);
            initialize();
            $("body").on("click", `${settings.editSelector},${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = saveUrl + "/" + $(this).data("id") ?? "";
                loadForm(url);

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
                                    loadNavigations(settings.baseUrl, settings.gridSelector);
                                    $(settings.lastCodeSelector).val(response.lastCode);
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
                                loadNavigations(settings.baseUrl, settings.gridSelector);
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
              

            $("body").on("click", ".js-orders", function (e) {
                e.preventDefault();
                var items = [];
                $(settings.gridSelector + " tr").each(function (i, item) {
                    if ($(item).find(".checkBox").data("id") !== "")
                        items.push($(item).find(".checkBox").data("id"));
                });

                if (items.length > 0) {
                    $.ajax({
                        url: settings.baseUrl + "/Orders",
                        method: "POST",
                        data: { "items": items },
                        success: function (data) {
                            location.reload();
                        }
                    });
                }
            });

            $("body").on("keyup", settings.availabilitySelector, function () {
                var self = $(this);
                let code = $(".js-navigation-code").val();
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

        function loadNavigations(baseUrl, gridSelector) {
            $.ajax({
                url: settings.baseUrl + "/Grid",
                method: "GET",
                success: function (response) {
                    $(settings.gridContainer).empty();
                    $(settings.gridContainer).html(response);


                    $('.sortable').multisortable({
                        items: "tr"
                    });
                    $(gridSelector).DataTable({
                        pageLength: 100,
                        order: []
                    });
                }
            });

            //var dataTable = $(gridSelector).DataTable({
            //    ajax: {
            //        url: baseUrl + "/grid",
            //        type: "GET",
            //        datatype: "json"
            //    },

            //    columnDefs: [
            //        { targets: [0], orderable: false },
            //    ],
            //    columns: [
            //        {
            //            "data": "menuId", "className": "text-center", "render": function (data, type, row) {
            //                return `<input type="checkbox" class="checkBox" value="${data}" data-id="${row.id}" />`;
            //            }
            //        },
            //        {
            //            "data": "menuId", "render": function (data) {
            //                return `<a class='btn js-navigation-edit' data-id='${data}'><i>${data}</i></a>`;
            //            }
            //        },
            //        { "data": "title", "autowidth": true, "className": "text-center" },
            //        { "data": "parentId", "autowidth": true, "className": "text-center" },
            //        { "data": "controllerName", "autowidth": true, "className": "text-center" },
            //        { "data": "viewName", "autowidth": true, "className": "text-center" },
            //        { "data": "orderBy", "autowidth": true, "className": "text-center" },
            //        { "data": "icon", "autowidth": true, "className": "text-center" }
            //    ],
            //    lengthChange: false,
            //    pageLength: 100,
            //    order: [[0, "Desc"]],
            //    sScrollY: "100%",
            //    scrollX: true,
            //    sScrollX: "100%",
            //    bDestroy: true
            //});
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

