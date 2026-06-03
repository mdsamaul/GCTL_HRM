(function ($) {
    $.patientReports = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#patient-form",
            formContainer: ".js-patient-form-container",
            gridSelector: "#patient-grid",
            gridContainer: ".js-patient-grid-container",
            previewSelector: ".js-commission-payment-preview",
            load: function () {

            }
        }, options);

        $(() => {
            loadPatients(settings.baseUrl, settings.gridSelector);
            initialize();


            $("body").on("change", ".patienttype, .patientcategory, .department, .doctor, .activitystatus, .datefrom, .dateto", function () {
                loadPatients(settings.baseUrl, settings.gridSelector);
            });


            $("body").on("click", ".js-export", function () {
                var self = $(this);
                var patientTypeCode = $(".patienttype").val();
                var patientCategoryCode = $(".patientcategory").val();
                var departmentCode = $(".department").val();
                var doctorCode = $(".doctor").val();
                var activityStatus = $(".activitystatus").val();
                var fromDate = $(".datefrom").val();
                var toDate = $(".dateto").val();

                let reportRenderType = $(".export-format").val();
                window.open(
                    settings.baseUrl + `/Export?patientTypeCode=${patientTypeCode}&patientCategoryCode=${patientCategoryCode}&departmentCode=${departmentCode}&doctorCode=${doctorCode}&activityStatus=${activityStatus}&fromDate=${fromDate}&toDate=${toDate}&reportType=Patients&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });
        });


        function loadPatients(baseUrl, gridSelector) {
            var data = {
                patientTypeCode: $(".patienttype").val(),
                patientCategoryCode: $(".patientcategory").val(),
                departmentCode: $(".department").val(),
                doctorCode: $(".doctor").val(),
                activityStatus: $(".activitystatus").val(),
                fromDate: $(".datefrom").val(),
                toDate: $(".dateto").val()
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
                        "data": "registrationId", "className": "text-center", autowidth: true
                    },
                    { "data": "registrationDate", "width": "200px" },
                    { "data": "patientCode", "className": "text-center", width: "150px" },
                    { "data": "patientName", "className": "text-center", width: "150px" },
                    { "data": "patientCategoryName", "className": "text-center", width: "120px" },
                    { "data": "admissionDate", "className": "text-center", width: "180px" },
                    { "data": "disease", "className": "text-center", width: "200px" },
                    {
                        "data": "roomTypeName", "className": "text-center", width: "120px"
                    },
                    { "data": "serialNo", "className": "text-center", width: "120px" },
                    { "data": "phone", "className": "text-center", width: "120px" },
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
                    },
                    {
                        "data": "doctorName", "className": "text-center", width: "150px"
                    },
                    { "data": "departmentName", "className": "text-center", width: "200px" }
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

