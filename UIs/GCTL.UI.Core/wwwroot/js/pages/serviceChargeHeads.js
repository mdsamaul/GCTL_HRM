(function ($) {
    $.serviceChargeHeads = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#serviceChargeHead-form",
            formContainer: ".js-serviceChargeHead-form-container",
            gridSelector: "#serviceChargeHead-grid",
            gridContainer: ".js-serviceChargeHead-grid-container",
            editSelector: ".js-serviceChargeHead-edit",
            saveSelector: ".js-serviceChargeHead-save",
            selectAllSelector: "#serviceChargeHead-check-all",
            deleteSelector: ".js-serviceChargeHead-delete-confirm",
            deleteModal: "#serviceChargeHead-delete-modal",
            finalDeleteSelector: ".js-serviceChargeHead-delete",
            clearSelector: ".js-serviceChargeHead-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-serviceChargeHead-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-serviceChargeHead-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () {

            }
        }, options);

        var dataTable;
        var index;
        var data;
        var gridUrl = settings.baseUrl + "/grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(() => {
            //settings.load(settings.baseUrl, settings.gridSelector);
            loadServiceChargeHeads(settings.baseUrl, settings.gridSelector);
            initialize();
            $("body").on("click", `${settings.editSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = saveUrl + "/" + $(this).data("id") ?? "";
                loadForm(url);

                // alert('Row index: ' + dataTable.row(this).index());
                $("html, body").animate({ scrollTop: 0 }, 500);
            });

            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                loadForm(saveUrl);

                // $("html, body").animate({ scrollTop: 0 }, 500);
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
                            data = response.data;
                            console.log(data);
                            console.log(index);
                            dataTable.draw();
                            //if (index >= 0) {
                            //    dataTable
                            //        .row(index)
                            //        .data(data)
                            //        .draw();
                            //} else {
                            //    dataTable
                            //        .row.add(data)
                            //        .draw();
                            //}

                            let url = saveUrl + "?serviceChargeGroupId=" + $("#ServiceChargeGroupId").val();
                            loadForm(url)
                                .then((data) => {
                                    // loadServiceChargeHeads(settings.baseUrl, settings.gridSelector);
                                    $(settings.lastCodeSelector).val(response.lastCode);
                                    /*settings.load(settings.baseUrl, settings.gridSelector);*/
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
                                //  loadServiceChargeHeads(settings.baseUrl, settings.gridSelector);
                                let url = saveUrl + "?serviceChargeGroupId=" + $("#ServiceChargeGroupId").val();
                                loadForm(url);
                                dataTable.draw();
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

            $("body").on("change", "#ServiceChargeGroupId", function () {
                if ($(this).val().length > 0) {
                    dataTable.draw();
                }
            });
        });

        function loadServiceChargeHeads(baseUrl, gridSelector) {
            dataTable = $(gridSelector).DataTable({
                //stateSave: true,
                proccessing: true,
                serverSide: true,
                ajax: {
                    url: baseUrl + "/grid",
                    type: "POST",
                    datatype: "json",
                    data: function (data) {
                        data.filter = {
                            serviceChargeGroupId: $("#ServiceChargeGroupId").val()
                        }
                    },
                },
                language: {
                    processing: "Fetching Data. Please wait..."
                },
                columnDefs: [
                    { targets: [0], orderable: false },
                ],
                columns: [
                    {
                        "data": "serviceChargeHeadId", "className": "text-center", "render": function (data) {
                            return ` <input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "serviceChargeHeadId", "render": function (data) {
                            return `<a class='btn js-serviceChargeHead-edit' data-id='${data}'><i>${data}</i></a>`;
                        }
                    },
                    { "data": "serviceChargeHead", "autowidth": true, "className": "text-center" },
                    { "data": "shortName", "autowidth": true, "className": "text-center" },
                    { "data": "serviceChargeGroup", "autowidth": true, "className": "text-center" }
                ],
                lengthChange: false,
                pageLength: 8,
                order: [[1, "Desc"]],
                sScrollY: "100%",
                scrollX: true,
                sScrollX: "100%",
                bDestroy: false
            });

            $(settings.gridSelector + ' tbody').on('click', settings.editSelector, function (e) {
                if ($(e.currentTarget).closest('tr').hasClass('selected')) {
                    $(e.currentTarget).closest('tr').removeClass('selected');
                } else {
                    dataTable.$('tr.selected').removeClass('selected');
                    $(e.currentTarget).closest('tr').addClass('selected');

                    let url = saveUrl + "/" + $(this).data("id") ?? "";
                    loadForm(url);

                    index = dataTable.row('.selected').index();
                    $("html, body").animate({ scrollTop: 0 }, 500);
                }

                //var selectedData = dataTable.row('.selected').data();               
                //console.log(selectedData);
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

