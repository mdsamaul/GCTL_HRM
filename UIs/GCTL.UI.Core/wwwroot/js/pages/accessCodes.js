(function ($) {
    $.accessCodes = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#accesscode-form",
            formContainer: ".js-accesscode-form-container",
            gridSelector: "#accesscode-grid",
            gridContainer: ".js-accesscode-grid-container",
            editSelector: ".js-accesscode-edit",
            saveSelector: ".js-accesscode-save",
            selectAllSelector: "#accesscode-check-all",
            deleteSelector: ".js-accesscode-delete-confirm",
            deleteModal: "#accesscode-delete-modal",
            finalDeleteSelector: ".js-accesscode-delete",
            clearSelector: ".js-accesscode-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-accesscode-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-accesscode-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            load: function () {

            }
        }, options);


        var gridUrl = settings.baseUrl + "/grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(() => {
            //settings.load(settings.baseUrl, settings.gridSelector);
            loadAccessCodes(settings.baseUrl, settings.gridSelector);
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
            $("body").on("click", settings.saveSelector, function (e) {
                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }
                $(settings.formSelector).submit();

            });

            $("body").on("click", settings.selectAllSelector, function () {
                $(".checkBox").prop('checked',
                    $(this).prop('checked'));
            });

            $('body').on('click', '.view', function () {
                if ($(this).is(":checked")) {
                    $(this).closest('tr').find("input:checkbox").prop('checked', true);
                } else {
                    $(this).closest('tr').find("input:checkbox").prop('checked', false);
                }
            });
            $("body").on("click", '.accesscode-check-all', function () {
                if ($(this).is(":checked")) {
                    $('.checked').prop('checked', true);
                } else {
                    $('.checked').prop('checked', false);
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
                                loadAccessCodes(settings.baseUrl, settings.gridSelector);
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

            $("body").on("change", '#EmployeeId', function () {
                if ($(this).val().length > 0)
                    getEmployeeInfo($(this).val());
            });
        });

        function getEmployeeInfo(id) {
            $.ajax({
                type: "GET",
                url: settings.baseUrl + "/GetEmployee/",
                data: { id: id },
                dataType: 'json',
                success: function (data) {
                    if (data) {
                        $("#lblEmployeeName").text(data.employeeName);
                        $("#lblDepartment").text(data.departmentName);
                        $("#lblDesignation").text(data.designationName);
                    } else {
                        $("#lblEmployeeName").text("");
                        $("#lblDepartment").text("");
                        $("#lblDesignation").text("")
                    }
                }
            });
        };

        function loadAccessCodes(baseUrl, gridSelector) {
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
                        "data": "accessCodeId", "className": "text-center", width: "40px", "render": function (data) {
                            return ` <input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "accessCodeId", className: "text-center", "render": function (data) {
                            return `<a class='btn js-accesscode-edit' data-id='${data}'><i>${data}</i></a>`;
                        }
                    },
                    { "data": "accessCodeName", "autowidth": true, className: "text-center" }
                ],
                lengthChange: false,
                pageLength: 8,
                order: [[0, "Desc"]],
                sScrollY: "100%",
                scrollX: true,
                sScrollX: "100%",
                bDestroy: true
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
        }
    }

}(jQuery));

