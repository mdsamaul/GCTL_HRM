(function ($) {
    $.dischargeReports = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#discharge-form",
            formContainer: ".js-discharge-form-container",
            gridSelector: "#discharges-grid",
            gridContainer: ".js-discharge-grid-container",
            previewSelector: ".js-discharge-preview",
            clearSelector: ".js-discharge-clear",
            topSelector: ".js-go",
            load: function () {

            }
        }, options);

        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(() => {
            initialize();
            previewReport();

            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
                //  $(settings.saveSelector).attr("disabled", "disabled");
                //loadCommissionSetups(settings.baseUrl, settings.gridSelector);
                loadForm(saveUrl);
                initialize();
            });

            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 500 }, 500);
            });

            $("body").on("click", ".js-discharge-export", function () {
                var self = $(this);
                var patientTypeId = $("#PatientTypeId").val();
                var admissionTypeId = $("#AdmissionTypeId").val();
                var patientId = $("#PatientId").val();
                var fromDate = $("#FromDate").val();
                var toDate = $("#ToDate").val();
                let reportRenderType = self.data("rendertype");
                window.open(
                    settings.baseUrl + `/Export?patientTypeId=${patientTypeId}&admissionTypeId=${admissionTypeId}&patientId=${patientId}&fromDate=${fromDate}&toDate=${toDate}&reportType=Discharges&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });

            $("body").on("click", settings.previewSelector, function () {
                previewReport();
            });
        });


        function previewReport() {
            $(".js-discharge-grid-container").LoadingOverlay("show", {
                background: "rgba(165, 190, 100, 0.5)"
            });
            var self = $(this);
            let reportRenderType = self.data("rendertype") ?? "PDF";
            $.ajax({
                url: settings.baseUrl + "/Export",
                method: "POST",
                data: {
                    patientTypeId: $("#PatientTypeId").val() ?? "",
                    admissionTypeId: $("#AdmissionTypeId").val() ?? "",
                    patientId: $("#PatientId").val() ?? "",
                    fromDate: $("#FromDate").val() ?? "",
                    toDate: $("#ToDate").val() ?? "",
                    reportType: "Discharges",
                    reportRenderType: reportRenderType,
                    isPreview: true
                },
                success: function (response) {
                    var url = normalizeUrl(getBaseUrl()) + response;
                    $("#js-discharge-previewer").attr("data", url);
                    $(".js-discharge-grid-container").LoadingOverlay("hide", true);
                }
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
    }
}(jQuery));

