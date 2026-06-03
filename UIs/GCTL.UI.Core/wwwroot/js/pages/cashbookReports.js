(function ($) {
    $.cashbookReport = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#cashbook-form",
            formContainer: ".js-cashbook-form-container",
            gridSelector: "#cashbooks-grid",
            gridContainer: ".js-cashbook-grid-container",
            previewSelector: ".js-cashbook-preview",
            clearSelector: ".js-cashbook-clear",
            topSelector: ".js-go",
            load: function () {

            }
        }, options);

        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(() => {
            initialize();
           // previewReport();

            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();
                loadForm(saveUrl);
                initialize();
            });



            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 500 }, 500);
            });

            $("body").on("click", ".js-cashbook-export", function () {
                var self = $(this);

                var isWithOpening = "";
                if ($("#chkisWithOpening").prop('checked') != true) {
                    isWithOpening = 'N'
                }
                else {
                    isWithOpening = 'Y'
                }
                var fromDate = $("#FromDate").val();
                var toDate = $("#ToDate").val();
             
                let reportRenderType = self.data("rendertype");
                window.open(
                    settings.baseUrl + `/Export?isWithOpening=${isWithOpening}&fromDate=${fromDate}&toDate=${toDate}&isPreview="N"&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });


            $("body").on("click", settings.previewSelector, function () {
                previewReport(); 
            });
        });

        function previewReport() {
            $(".js-cashbook-grid-container").LoadingOverlay("show", {
                background: "rgba(165, 190, 100, 0.5)"
            });
            var self = $(this);

            var isWithOpening = "";
            if ($("#chkisWithOpening").prop('checked') != true) {
                isWithOpening = 'N'
            }
            else {
                isWithOpening = 'Y'
            }
            let reportRenderType = "PDF";
            $.ajax({
                url: settings.baseUrl + "/Export",
                method: "POST",
                data: {
                    isWithOpening: isWithOpening,
                    fromDate: $("#FromDate").val() ?? "",
                    toDate: $("#ToDate").val() ?? "",                 
                    isPreview: "Y",
                    reportRenderType: reportRenderType
                },
                success: function (response) {
                    var url = normalizeUrl(getBaseUrl()) + response;

                    $("#js-cashbook-previewer").attr("data", url);
                    $(".js-cashbook-grid-container").LoadingOverlay("hide", true);
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


        }
        function refreshControl() {
          
            initialize();


        }
    }
}(jQuery));

