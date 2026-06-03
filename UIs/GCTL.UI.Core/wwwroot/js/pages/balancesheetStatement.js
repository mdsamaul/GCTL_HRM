(function ($) {
    $.balancesheetStatement = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#balancesheetStatement-form",
            formContainer: ".js-balancesheetStatement-form-container",
            gridSelector: "#balancesheetStatement-grid",
            gridContainer: ".js-balancesheetStatement-grid-container",
            previewSelector: ".js-balancesheetStatement-preview",
            clearSelector: ".js-balancesheetStatement-clear",
            topSelector: ".js-go",
            load: function () {

            }
        }, options);

        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(() => {
            initialize();

            var d = new Date();
            var month = d.getMonth() + 1;
            var day = d.getDate();

            var output = (day < 10 ? '0' : '') + day + '/' +
                (month < 10 ? '0' : '') + month + '/' + d.getFullYear();
            
            $('#FromDate').val('01' + '/' + (month < 10 ? '0' : '') + month + '/' + d.getFullYear());
            $('#ToDate').val(output);


            //previewReport();

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

            $("body").on("click", ".js-balancesheetStatement-export", function () {
                var self = $(this);
                     
                var fromDate = $("#FromDate").val();
                var toDate = $("#ToDate").val();

                let reportRenderType = self.data("rendertype");
           
                window.open(
                    settings.baseUrl + `/Export?fromDate=${fromDate}&toDate=${toDate}&isPreview="N"&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });


            $("body").on("click", settings.previewSelector, function () {
                previewReport(); 
            });
        });

        function previewReport() {
            $(".js-balancesheetStatement-grid-container").LoadingOverlay("show", {
                background: "rgba(165, 190, 100, 0.5)"
            });
            var self = $(this);
            let reportRenderType = "PDF";
            $.ajax({
                url: settings.baseUrl + "/Export",
                method: "POST",
                data: {
                             
                    fromDate: $("#FromDate").val() ?? "",
                    toDate: $("#ToDate").val() ?? "",
                    isPreview: "Y",
                    reportRenderType: reportRenderType
                },
                success: function (response) {
                    var url = normalizeUrl(getBaseUrl()) + response;
                    $("#js-balancesheetStatement-previewer").attr("data", url);
                    $(".js-balancesheetStatement-grid-container").LoadingOverlay("hide", true);
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


        }
        function refreshControl() {
            $('.multiselect').multiselect('destroy');
            initialize();


        }
    }
}(jQuery));

