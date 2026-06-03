(function ($) {
    $.nurseReports = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#nurse-form",
            formContainer: ".js-nurse-form-container",
            gridSelector: "#nurse-grid",
            gridContainer: ".js-nurse-grid-container",
            previewSelector: ".js-commission-payment-preview",
            load: function () {

            }
        }, options);

        $(() => {
            loadNurses(settings.baseUrl, settings.gridSelector);
            initialize();


            $("body").on("change", ".nursetype, .department, .speciality, .qualification", function () {
                loadNurses(settings.baseUrl, settings.gridSelector);
            });


            $("body").on("click", ".js-export", function () {
                var self = $(this);
                var departmentCode = $(".department").val();
                var specialityCode = $(".speciality").val();
                var qualificationCode = $(".qualification").val();

                let reportRenderType = $(".export-format").val();
                window.open(
                    settings.baseUrl + `/Export?departmentCode=${departmentCode}&specialityCode=${specialityCode}&qualificationCode=${qualificationCode}&reportType=Nurses&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });
        });


        function loadNurses(baseUrl, gridSelector) {
            var data = {
                departmentCode: $(".department").val(),
                specialityCode: $(".speciality").val(),
                qualificationCode: $(".qualification").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/Reports",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "nurseCode", "className": "text-center", autowidth: true
                    },
                    { "data": "nurseName", "width": "200px" },
                    { "data": "departmentName", "className": "text-center", width: "120px" },
                    { "data": "specialist", "className": "text-center", width: "120px" },
                    { "data": "qualification", "className": "text-center", width: "180px" },
                    { "data": "phone", "className": "text-center", width: "120px" },
                    { "data": "email", "className": "text-center", width: "120px" },
                    {
                        "data": "photoUrl", "className": "text-center", width: "120px",
                        render: function (data) {
                            if (data)
                                return `<img id="photoPreview" class="img-fluid img-thumbnail img-roundedr" src="${getBaseUrl()}/Uploads/Images/Nurses/${data}">`;
                            else
                                return '';
                        }
                    },
                    {
                        "data": "activityStatus", "className": "text-center", width: "120px",
                        render: function (data) {
                            if (data == "Active") {
                                return 'Active';
                                // return `<span class='btn btn-sm btn-success' title="${data}"><i class='fa fa-check-circle'></i></span>`;
                            } else if (data = "Inactive") {
                                return 'Inactive';
                                // return `<span class='btn btn-sm btn-danger' title="${data}"><i class='fa fa-times-circle'></i></span>`;
                            }

                            return 'Inactive';
                        }
                    }
                ],
                lengthChange: true,
                pageLength: 10,
                lengthMenu: [
                    [10, 25, 50, -1],
                    [10, 25, 50, 'All'],
                ],
                order: [[1, "Desc"]],
                destroy: true
            });
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

