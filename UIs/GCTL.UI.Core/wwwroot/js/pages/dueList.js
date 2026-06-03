(function ($) {
    $.dueList = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#collection-form",
            formContainer: ".js-collection-form-container",
            gridSelector: "#collections-grid",
            gridContainer: ".js-collection-grid-container",
            previewSelector: ".js-collection-preview",
            clearSelector: ".js-collection-clear",
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
    
                loadForm(saveUrl);
                initialize();
            });

     

            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 500 }, 500);
            });

            $("body").on("click", ".js-collection-export", function () {
                var self = $(this);
                var billType = $("#BillTypeId").val();
                var patientType = $("#PatientTypeId").val();
                var fromDate = $("#FromDate").val();
                var toDate = $("#ToDate").val();
                var doctorId = $("#DoctorId").val();
                var referenceId = $("#ReferenceId").val();
                let reportRenderType = self.data("rendertype");
                window.open(
                    settings.baseUrl + `/Export?billTypeId=${billType}&patientTypeId=${patientType}&fromDate=${fromDate}&toDate=${toDate}&doctorId=${doctorId}&referenceId=${referenceId}&reportType=DueList&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });


            $("body").on("click", settings.previewSelector, function () {
                previewReport();
            });
        });


        function previewReport() {
            $(".js-collection-grid-container").LoadingOverlay("show", {
                background: "rgba(165, 190, 100, 0.5)"
            });
            var self = $(this);
            let reportRenderType = self.data("rendertype") ?? "PDF";
            $.ajax({
                url: settings.baseUrl + "/Export",
                method: "POST",
                data: {
                    billTypeId: $("#BillTypeId").val(),
                    patientTypeId: $("#PatientTypeId").val() ?? "",
                    fromDate: $("#FromDate").val() ?? "",
                    toDate: $("#ToDate").val() ?? "",
                    doctorId: $("#DoctorId").val() ?? "",
                    referenceId: $("#ReferenceId").val() ?? "",
                    reportType: "DueList",
                    reportRenderType: reportRenderType,
                    isPreview: true
                },
                success: function (response) {
                    var url = normalizeUrl(getBaseUrl()) + response;
                    $("#js-collection-previewer").attr("data", url);
                    $(".js-collection-grid-container").LoadingOverlay("hide", true);
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

        
        
        }
        function refreshControl() {
            $('.multiselect').multiselect('destroy');
            initialize();        
        }
    }
}(jQuery));

