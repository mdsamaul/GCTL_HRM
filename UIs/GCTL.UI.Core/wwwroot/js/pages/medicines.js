(function ($) {
    $.medicines = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#medicine-form",
            formContainer: ".js-medicine-form-container",
            gridSelector: "#medicines-grid",
            gridContainer: ".js-medicine-grid-container",
            editSelector: ".js-medicine-edit",
            saveSelector: ".js-medicine-save",
            selectAllSelector: "#medicine-check-all",
            deleteSelector: ".js-medicine-delete-confirm",
            deleteModal: "#medicine-delete-modal",
            finalDeleteSelector: ".js-medicine-delete",
            clearSelector: ".js-medicine-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-medicine-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-medicine-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: "#lastCode",
            load: function () {

            }
        }, options);


        var gridUrl = settings.baseUrl + "/grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(() => {
            loadMedicines(settings.baseUrl, settings.gridSelector);
            initialize();

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
                                loadMedicines(settings.baseUrl, settings.gridSelector);
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

            $("body").on("change", ".categoryCode", function (e) {
                loadMedicines(settings.baseUrl, settings.gridSelector);
            });

            $("body").on("click", ".js-medicine-export", function () {
                var self = $(this);
                var categoryCode = $("#MedicineCategoryCode").val();
                let reportRenderType = self.data("rendertype");
                window.open(
                    settings.baseUrl + `/Export?categoryCode=${categoryCode}&reportType=Medicines&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });

        });


        function loadMedicines(baseUrl, gridSelector) {
            var dataTable = $(gridSelector).DataTable({
                proccessing: true,
                serverSide: true,
                ajax: {
                    url: baseUrl + "/grid",
                    type: "POST",
                    datatype: "json",
                    data: function (data) {
                        data.filter = {
                            CategoryCode: $(".categoryCode").val()
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
                        "data": "medicineCode", "className": "text-center", "render": function (data) {
                            return ` <input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "medicineCode", "render": function (data) {
                            return `<a class='btn js-medicine-edit' data-id='${data}'><i>${data}</i></a>`;
                        }
                    },
                    { "data": "medicineDescription", "autowidth": true, "className": "text-center" },
                    { "data": "typeName", "autowidth": true, "className": "text-center" },
                    { "data": "manufacturerName", "autowidth": true, "className": "text-center" },
                    { "data": "unitName", "autowidth": true, "className": "text-center" },
                    { "data": "salesPrice", "autowidth": true, "className": "text-center" },
                    { "data": "purchasePrice", "autowidth": true, "className": "text-center" },
                    { "data": "categoryName", "autowidth": true, "className": "text-center" },
                    {
                        "data": "medicineCode", width: "150px", "render": function (data, type, row) {
                            return `<div class='action-buttons' style='width:60px'>
                                        <a class='btn btn-warning btn-circle btn-sm' title="Edit ${row.medicineName}" href='${baseUrl}/Setup/${data}'><i class='fas fa-pencil-alt'></i></a>     
                                        <button type="button" class="btn btn-danger btn-circle btn-sm js-medicine-delete-confirm"
                                                data-target="#deleteModalx"
                                                data-id="${data}"
                                                title="Delete ${row.medicineName}"
                                                data-title="Are you sure want to delete ${row.medicineName}?">
                                                    <i class="fas fa-trash fa-sm"></i>
                                        </button>`;
                        },
                        "orderable": false,
                        "searchable": false,
                        width: "100px"
                    }
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
        }
    }
}(jQuery));

