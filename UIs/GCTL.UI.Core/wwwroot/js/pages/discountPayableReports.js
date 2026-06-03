(function ($) {
    $.discountPayableReports = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#discountpayable-form",
            formContainer: ".js-discountpayable-form-container",
            gridSelector: "#discountpayables-grid",
            gridContainer: ".js-discountpayable-grid-container",           
            previewSelector: ".js-discountpayable-preview",          
            clearSelector: ".js-discountpayable-clear",
            topSelector: ".js-go",            
            load: function () {

            }
        }, options);

        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(() => {
            initialize();

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

                $.ajax({
                    url: settings.baseUrl + "/Payables",
                    type: 'POST',
                    data: data,
                    success: function (data) {
                        $(settings.gridContainer).empty();
                        $(settings.gridContainer).html(data);
                        $("#discountpayable-grid").DataTable();
                    },
                    error: function (error) {
                        console.log(error);
                    },
                })
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


            $("body").on("click", ".js-discountpayable-export", function () {
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
                /*showTodayButton: true,*/
                // Your Icons
                // as Bootstrap 4 is not using Glyphicons anymore
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

            $('.timepicker').datetimepicker({
                format: 'hh:mm A',
                /*showTodayButton: true,*/
                // Your Icons
                // as Bootstrap 4 is not using Glyphicons anymore
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

            $('.datetimepicker').datetimepicker({
                format: 'DD/MM/YYYY hh:mm A',
                /*showTodayButton: true,*/
                // Your Icons
                // as Bootstrap 4 is not using Glyphicons anymore
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

