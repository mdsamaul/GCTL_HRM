(function ($) {
    $.commissionDueReports = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#commission-due-form",
            formContainer: ".js-commission-due-form-container",
            gridSelector: "#commission-dues-grid",
            gridContainer: ".js-commission-due-grid-container",           
            previewSelector: ".js-commission-due-preview",          
            clearSelector: ".js-commission-due-clear",
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

            // Save
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
            //        url: settings.baseUrl + "/Dues",
            //        type: 'POST',
            //        data: data,
            //        success: function (data) {
            //            $(settings.gridContainer).empty();
            //            $(settings.gridContainer).html(data);
            //            $("#commissionDue-grid").DataTable();
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

            $("body").on('change', "#DoctorId", function () {
                if ($(this).val().length > 0) {
                    $("#ReferenceId").val("");
                    // $("#Receiver").val("Doctor");
                    $("#CommissionReceiver").val("Doctor");
                    $("#CommissionReceiverId").val($(this).val());
                    // $("#ReceiverName").val($("#DoctorId option:selected").text());

                    refreshControl();
                }
            });

            $("body").on('change', "#ReferenceId", function () {
                if ($(this).val().length > 0) {
                    $("#DoctorId").val("");
                    //  $("#Receiver").val("Ref. Doctor/Person");

                    $("#CommissionReceiver").val("Ref. Doctor/Person");
                    $("#CommissionReceiverId").val($(this).val());
                    //  $("#ReceiverName").val($("#ReferenceId option:selected").text());

                    refreshControl();
                }
            });

            $("body").on('change', '.CommissionReceiver', function () {
                $("#DepartmentId").val("");
                $("#DoctorId").val("");
                $("#ReferenceId").val("");
                $("#CommissionReceiver").val("");
                $("#CommissionReceiverId").val("");

                $("#CommissionReceiver").val($(this).val());
                refreshControl();

                $(".receiveroptions").removeClass("d-none")
                    .addClass("d-none");

                var target = $(this).data("target");

                $(target).addClass("d-none")
                    .removeClass("d-none");
            });

            $("body").on("change", "#DepartmentId", function () {
                var self = $(this);
                if (self.val().length > 0) {
                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetDoctorsByDepartment",
                        method: "POST",
                        data: { departmentCode: self.val() },
                        success: function (response) {
                            $("#DoctorId").empty();
                            //$("#DoctorId").append($('<option>', {
                            //    value: '',
                            //    text: `Doctor`
                            //}));
                            $.each(response, function (i, item) {
                                $("#DoctorId").append($('<option>', {
                                    value: item.code,
                                    text: item.name
                                }));
                            });

                            refreshControl();
                        }
                    });
                }
            });


            $("body").on("click", ".js-commission-due-export", function () {
                var self = $(this);
                var commissionReceiver = $("input:radio.CommissionReceiver:checked").val();
                var commissionReceiverId = $("#CommissionReceiverId").val();
                var fromDate = $("#FromDate").val();
                var toDate = $("#ToDate").val();
                let reportRenderType = self.data("rendertype");
                window.open(
                    settings.baseUrl + `/Export?commissionReceiver=${commissionReceiver}&commissionReceiverId=${commissionReceiverId}&fromDate=${fromDate}&toDate=${toDate}&reportType=CommissionDue&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });


            $("body").on("click", settings.previewSelector, function () {
                previewReport();
            });
        });

        function previewReport() {
            $(".js-commission-due-grid-container").LoadingOverlay("show", {
                background: "rgba(165, 190, 100, 0.5)"
            });
            var self = $(this);
            let reportRenderType = self.data("rendertype") ?? "PDF";
            $.ajax({
                url: settings.baseUrl + "/Export",
                method: "POST",
                data: {
                    commissionReceiver: $("input:radio.CommissionReceiver:checked").val(),
                    commissionReceiverId: $("#CommissionReceiverId").val(),
                    fromDate: $("#FromDate").val() ?? "",
                    toDate: $("#ToDate").val() ?? "",
                    reportType: "CommissionDue",
                    reportRenderType: reportRenderType,
                    isPreview: true
                },
                success: function (response) {
                    var url = normalizeUrl(getBaseUrl()) + response;
                    $("#js-commission-due-previewer").attr("data", url);
                    $(".js-commission-due-grid-container").LoadingOverlay("hide", true);
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

