
(function ($) {
    $.financialYear = function (options) {
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#FinancialYear-form",
            formContainer: ".js-FinancialYear-form-container",
            gridSelector: "#FinancialYear-grid",
            gridContainer: ".js-FinancialYear-grid-container",
            editSelector: ".js-FinancialYear-edit",
            saveSelector: ".js-FinancialYear-save",
            selectAllSelector: "#FinancialYear-check-all",
            deleteSelector: ".js-FinancialYear-delete-confirm",
            deleteModal: "#FinancialYear-delete-modal",
            finalDeleteSelector: ".js-FinancialYear-delete",
            clearSelector: ".js-FinancialYear-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-FinancialYear-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-FinancialYear-check-availability",
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

            loadTable();


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

                var id = $(this).data('id');
                $(settings.deleteSelector).data('id', id);

                $("html, body").animate({ scrollTop: 0 }, 500);
            });

            $("body").on("click", settings.saveSelector, function () {
                var validate = validation();
                if (!validate) {
                    return false;
                }

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

                                    loadTable();
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

                if (selectedItems.length > 0) {
                    $(settings.deleteModal).modal("show");
                } else {
                    toastr.info("Please select at least one item.");
                }
            });


            $("body").on('show.bs.modal', settings.deleteModal, function (event) {

                var source = $(event.relatedTarget);
                var id = source.data("ids");

                var title = source.data("title");
                title = "Are you sure want to delete these items?";
                var modal = $(this);
                $(modal).find('.title').html(title);

                $("body").on("click", settings.finalDeleteSelector, function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    e.stopImmediatePropagation();

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
                let code = $(".js-FinancialYear-code").val();
                let name = self.val();

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
                            [10, 25, 50, 'All']
                        ],
                        order: [[1, "desc"]],
                        destroy: true,
                        paging: true,
                        searching: true,
                        responsive: true,
                        processing: true,
                        deferRender: true,
                        columnDefs: [
                            {
                                targets: 0,
                                orderable: false,
                                searchable: false
                            }
                        ],
                    });

                })
                .fail(() => toastr.error("Failed to load table data."));
        }

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


                        resolve(data)
                    },
                    error: function (error) {
                        reject(error)
                    },
                })
            })
        }

        $(document).on('change', "#StartDate", populateName);
        $(document).on('change', "#EndDate", populateName);

        function populateName() {
            var startDate = $('#StartDate').val();
            var endDate = $('#EndDate').val();

            // Check if either date is missing or just whitespace
            if (!startDate?.trim() || !endDate?.trim()) {
                return null;
            }

            var startYear = new Date(startDate).getFullYear();
            var endYear = new Date(endDate).getFullYear();



            var name = startYear + '-' + endYear;
            console.log(name);
            $('#Name').val(name);
        }



        function validation() {

            var degName = $('#Name').val();
            if (!degName) {
                toastr.info('Enter Financial Year');
                $('#Name').trigger('focus');
                return false;
            }

            var startDate = $('#StartDate').val();
            var endDate = $('#EndDate').val();

            if (!startDate) {
                toastr.info('Enter Start Date');
                $('StartDate').trigger('focus');
                return false
            }

            if (endDate && new Date(startDate) >= new Date(endDate)) {
                toastr.info('Start date cannot be later than end date.')
                $('StartDate').trigger('focus');
                return false
            }

            return true;
        }

        $("body").on("keydown", `${settings.formSelector} input, ${settings.formSelector} select, ${settings.formSelector} textarea`, function (e) {
            if (e.which === 13) {
                e.preventDefault();

                var focusableElements = $(settings.formSelector).find('input:visible:enabled, select:visible:enabled, textarea:visible:enabled, button:visible:enabled');

                var currentIndex = focusableElements.index(this);

                var nextIndex = (currentIndex + 1) % focusableElements.length;

                focusableElements.eq(nextIndex).focus();
            }
        });
    }

}(jQuery));
