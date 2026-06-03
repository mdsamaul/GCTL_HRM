(function ($) {
    $.designations = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#designation-form",
            formContainer: ".js-designation-form-container",
            gridSelector: "#designation-grid",
            gridContainer: ".js-designation-grid-container",
            editSelector: ".js-designation-edit",
            saveSelector: ".js-designation-save",
            selectAllSelector: "#designation-check-all",
            deleteSelector: ".js-designation-delete-confirm",
            deleteModal: "#designation-delete-modal",
            finalDeleteSelector: ".js-designation-delete",
            clearSelector: ".js-designation-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-designation-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-designation-check-availability",
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
            loadDesignationss(settings.baseUrl, settings.gridSelector);
            initialize();
            $("body").on("click", `${settings.editSelector},${settings.clearSelector}`, function (e)
            {
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


            $("body").on('click', '#exportPdf', function () {
                $("#loadingIndicator").show();
                $.ajax({
                    url: '/Designations/ExportToPdf',
                    type: 'Get',

                    success: function () {
                        window.location.href = '/Designations/ExportToPdf'
                    },
                    error: function () {
                       // toastr.error('An error occurred while exporting the PDF.');
                    },
                    complete: function () {
                        $('#loadingIndicator').hide();
                    }

                });
            });


            $("body").on('click', '#exportExcel', function () {
                $("#loadingIndicator").show();
                $.ajax({
                    url: '/Designations/ExportToExcel',
                    method: 'GET',
                    success: function () {
                        window.location.href = '/Designations/ExportToExcel';
                    },
                    error: function () {
                       // toastr.error('Error to Export Excel')
                    },
                    complete: function () {
                        $('#loadingIndicator').hide();
                    }
                });
            });

            $("body").on('click', '#exportPdf', function () {
                $("#loadingIndicator").show();
                $.ajax({
                    url: '/Designations/ExportToPdf',
                    type: 'Get',
                    
                    success: function (response)
                    {
                        window.location.href ='/Designations/ExportToPdf'
                    },
                    error: function ()
                    {
                        toastr.error('An error occurred while exporting the PDF.');
                    },
                    complete: function ()
                    {
                        $('#loadingIndicator').hide();
                    }
                    
                });
            });

            $("body").on('click', '#exportExcel', function () {
                $("#loadingIndicator").show();
                $.ajax({
                    url: '/Designations/ExportToExcel',
                    method: 'GET',
                    success: function (response) {
                        window.location.href = '/Designations/ExportToExcel';
                    },
                    error: function () {
                        toastr.error('Error to Export Excel');
                    },
                    complete: function () {
                        $('#loadingIndicator').hide();
                    }
                });
            });

            //
            // Save
            $("body").on("click", settings.saveSelector, function () {
                if (!validation()) return false;
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
                                    loadDesignationss(settings.baseUrl, settings.gridSelector);
                                    $(settings.lastCodeSelector).val(response.lastCode);
                                  

                                })
                                .catch((error) => {
                                    console.log(error)
                                })

                            toastr.success(response.message);
                        }
                        else {
                            toastr.error(response.message);
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

                if (selectedItems.length > 0)
                {
                    $(settings.deleteModal).modal("show");
                } else
                {
                    toastr.info("Please select at least one item.");
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
                                toastr.success(response.message);
                                selectedItems = [];
                                /*settings.load(settings.baseUrl);*/
                                loadDesignationss(settings.baseUrl, settings.gridSelector);
                                loadForm(saveUrl);
                            }
                            else {
                                if (response.refSuccess) {
                                    toastr.warning(response.message, 'Warning');
                                }

                                if (!response.success) {
                                    toastr.error(response.message, 'Error');
                                }
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

            //$("body").on("keyup", settings.availabilitySelector, function () {
            //    var self = $(this);
            //    let code = $(".js-designation-code").val();
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

        function loadDesignationss(baseUrl, gridSelector) {
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
                        "data": "designationCode", "className": "text-center", width: "2%", "render": function (data)
                        {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }

                    },
                    {
                        "data": "designationCode", width: "5%", "render": function (data) {
                           
                            return `<a class='btn js-designation-edit' data-id='${data}'><i>${data}</i></a>`;
                        }
                    },
                    { "data": "designationName", "autoWidth": true, "className": "text-left" },
                    { "data": "designationShortName", "autoWidth": true, "className": "text-center" },
                    { "data": "banglaDesignation", "autoWidth": true, "className": "text-left" },
                ],
                lengthChange: true,
                pageLength: 10,
                lengthMenu: [
                    [10, 25, 50, -1],
                    [10, 25, 50, 'All'],
                ],
                order: [[1, "desc"]],
                destroy: true
            });
        }

        function loadForm(url)
        {
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

        function initialize()
        {
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

        function validation() {

            var dep = $('#DesignationName').val();

            if (!dep) {
                toastr.info('Enter Designation Name');
                $('#DesignationName').trigger('focus');
                return false;
            }

            return true;

        }
    }

}(jQuery));

