(function ($) {
    $.outPatientPrescriptionReport = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#outPatientPrescriptionReport-form",
            formContainer: ".js-outPatientPrescriptionReport-form-container",
            gridSelector: "#outPatientPrescriptionReport-grid",
            gridContainer: ".js-outPatientPrescriptionReport-grid-container",
            previewSelector: ".js-outPatientPrescriptionReport-preview",
            clearSelector: ".js-outPatientPrescriptionReport-clear",
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

            $("body").on("click", ".js-outPatientPrescriptionReport-export", function () {
                var self = $(this);
                var billType = $("#BillTypeId").val();
                var patientType = $("#PatientTypeId").val();
                var fromDate = $("#FromDate").val();
                var toDate = $("#ToDate").val();
                var doctorId = $("#DoctorId").val();
                var referenceId = $("#ReferenceId").val();
                let reportRenderType = self.data("rendertype");
                window.open(
                    settings.baseUrl + `/Export?billTypeId=${billType}&patientTypeId=${patientType}&fromDate=${fromDate}&toDate=${toDate}&doctorId=${doctorId}&referenceId=${referenceId}&reportType=Collections&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });


            $("body").on("click", settings.previewSelector, function () {
                previewReport(); 
            });
        });

        function previewReport() {
            $(".js-outPatientPrescriptionReport-grid-container").LoadingOverlay("show", {
                background: "rgba(165, 190, 100, 0.5)"
            });
            var self = $(this);
            let reportRenderType = self.data("rendertype") ?? "PDF";
            $.ajax({
                url: settings.baseUrl + "/Export",
                method: "POST",
                data: {
                    patientTypeCode: $("#ddlpatientTypeCode").val(),
                    doctorCode: $("#ddldoctorCode").val() ?? "",
                    ReferencePersonId: $("#ddlReferencePersonId").val() ?? "",
                    VisitingReportingFee: $("#ddlVisitingReportingFee").val() ?? "",
                    fromDate: $("#FromDate").val() ?? "",
                    toDate: $("#ToDate").val() ?? "",
                  
                    reportType: "OutPatientPrescriptionwiseFee",
                    reportRenderType: reportRenderType,
                    isPreview: true
                },
                success: function (response) {
                    var url = normalizeUrl(getBaseUrl()) + response;
                    $("#js-outPatientPrescriptionReport-previewer").attr("data", url);
                    $(".js-outPatientPrescriptionReport-grid-container").LoadingOverlay("hide", true);
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
         
            initialize();
            //$('.dynamicselectpicker').select2({
            //    language: {
            //        noResults: function () {
            //            //return 'Not found <a class="add_new_item" href="javascript:void(0)">Add New</a>';
            //        }
            //    },
            //    escapeMarkup: function (markup) {
            //        return markup;
            //    },
            //    placeholder: "change your placeholder"
            //});
        }
    }
}(jQuery));

