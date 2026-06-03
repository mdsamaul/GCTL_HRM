(function ($) {
    $.medicineReports = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#medicine-form",
            formContainer: ".js-medicine-form-container",
            gridSelector: "#medicines-grid",
            gridContainer: ".js-medicine-grid-container",
            previewSelector: ".js-medicine-preview",
            clearSelector: ".js-medicine-clear",
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

                initialize();
            });

            //// Preview
            //$("body").on("click", settings.previewSelector, function () {
            //    var $valid = $(settings.formSelector).valid();
            //    if (!$valid) {
            //        return false;
            //    }

            //    var data;
            //    if (settings.haseFile)
            //        data = new FormData($(settings.formSelector)[0]);
            //    else
            //        data = $(settings.formSelector).serialize();

            //    var url = $(settings.formSelector).attr("action");

            //    $.ajax({
            //        url: settings.baseUrl + "/Commissions",
            //        type: 'POST',
            //        data: data,
            //        success: function (data) {
            //            $(settings.gridContainer).empty();
            //            $(settings.gridContainer).html(data);
            //        },
            //        error: function (error) {
            //            console.log(error);
            //        },
            //    })
            //});


            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 500 }, 500);
            });

            $("body").on("click", ".js-medicine-export", function () {
                var self = $(this);
                var categoryCode = $("#CategoryCode").val();
                let reportRenderType = self.data("rendertype");
                window.open(
                    settings.baseUrl + `/Export?categoryCode=${categoryCode}&reportType=Medicines&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });


            $("body").on("click", settings.previewSelector, function () {
                previewReport();
            });
        });


        function previewReport() {
            $(".js-medicine-grid-container").LoadingOverlay("show", {
                background: "rgba(165, 190, 100, 0.5)"
            });
            var self = $(this);
            let reportRenderType = self.data("rendertype") ?? "PDF";
            $.ajax({
                url: settings.baseUrl + "/Export",
                method: "POST",
                data: {
                    categoryCode: $("#CategoryCode").val(),
                    reportType: "Medicines",
                    reportRenderType: reportRenderType,
                    isPreview: true
                },
                success: function (response) {
                    var url = normalizeUrl(getBaseUrl()) + response;
                    $("#js-medicine-previewer").attr("data", url);
                    $(".js-medicine-grid-container").LoadingOverlay("hide", true);
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

