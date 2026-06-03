(function ($) {
    $.commissionPayableReports = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#commissionpayable-form",
            formContainer: ".js-commissionpayable-form-container",
            gridSelector: "#commissionpayables-grid",
            gridContainer: ".js-commissionpayable-grid-container",
            previewSelector: ".js-commissionpayable-preview",
            clearSelector: ".js-commissionpayable-clear",
            topSelector: ".js-go",
            load: function () {

            }
        }, options);

        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(() => {
            initialize();
            //loadPayables();

            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
                //  $(settings.saveSelector).attr("disabled", "disabled");
                //loadCommissionSetups(settings.baseUrl, settings.gridSelector);
                loadForm(saveUrl);
                initialize();
            });

            // Save
            $("body").on("click", settings.previewSelector, function () {
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

                loadPayables();
            });


            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 500 }, 500);
            });

            $("body").on('change', "#DoctorId", function () {
                $("#ReferenceId").val("");
                $("#CommissionReceiver").val("Doctor");
                $("#CommissionReceiverId").val($(this).val());
                $('.selectpicker').select2({});
            });

            $("body").on('change', "#ReferenceId", function () {
                $("#DoctorId").val("");
                $("#CommissionReceiver").val("Ref. Doctor/Person");
                $("#CommissionReceiverId").val($(this).val());
                $('.selectpicker').select2({});
            });


            $("body").on('change', '.CommissionReceiver', function () {
                $(".receiveroptions").removeClass("d-none")
                    .addClass("d-none");

                var target = $(this).data("target");
                $(target).addClass("d-none")
                    .removeClass("d-none");

                if ($(this).val() != "Doctor") {
                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetReferencePersons",
                        method: "GET",
                        success: function (response) {
                            $("#CommissionReceiverId").empty();
                            $("#CommissionReceiverId").append($('<option>', {
                                value: '',
                                text: `Ref. Doctor/Person`
                            }));
                            $.each(response, function (i, item) {
                                $("#CommissionReceiverId").append($('<option>', {
                                    value: item.code,
                                    text: item.name
                                }));
                            });

                            refreshControl();
                        }
                    });
                }
            });


            $("body").on("click", ".js-commissionpayable-export", function () {
                var self = $(this);
                var commissionReceiver = $("#CommissionReceiver").val();
                var commissionReceiverId = $("#CommissionReceiverId").val();
                var fromDate = $("#FromDate").val();
                var toDate = $("#ToDate").val();
                window.open(
                    settings.baseUrl + `/Export?commissionReceiver=${commissionReceiver}&commissionReceiverId=${commissionReceiverId}&fromDate=${fromDate}&toDate=${toDate}`,
                    "_blank"
                )
            });
        });


        function loadPayables() {
            $.ajax({
                url: settings.baseUrl + "/Payables",
                type: 'POST',
                data: $(settings.formSelector).serialize(),
                success: function (data) {
                    $(settings.gridContainer).empty();
                    $(settings.gridContainer).html(data);
                    $("#commissionpayable-grid").DataTable();
                },
                error: function (error) {
                    console.log(error);
                },
            })
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

        function initialize(selectedText = '', selectedValue = '') {
            $('.selectpicker').select2({

                dropdownAutoWidth: true,
                language: {
                    noResults: function () {
                        //return 'Not found <a class="add_new_item" href="javascript:void(0)">Add New</a>';
                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });

            $('.multiselect').multiselect({
                includeSelectAllOption: true,
                enableCaseInsensitiveFiltering: true,
                buttonContainer: '<div class="btn-group w-100" />',
                onSelectAll: function (options) {
                    // alert('onSelectAll triggered, ' + options.length + ' options selected!');
                }
            });

            $('.datepicker').datetimepicker({
                format: 'DD/MM/YYYY',
              
                icons: {
                    time: 'fas fa-clock',
                    date: 'fas fa-calendar',
                    up: 'fas fa-chevron-up',
                    down: 'fas fa-chevron-down',
                    previous: 'fas fa-chevron-left',
                    next: 'fas fa-chevron-right',
                    today: 'fas fa-check',
                    clear: 'fas fa-trash',
                    close: 'fas fa-times'
                }
            });

         

        }
        function refreshControl() {
            $('.multiselect').multiselect('destroy');
            initialize();
        }
    }
}(jQuery));

