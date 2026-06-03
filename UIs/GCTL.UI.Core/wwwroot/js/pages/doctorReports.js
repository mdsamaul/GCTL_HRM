(function ($) {
    $.doctorReports = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#doctor-form",
            formContainer: ".js-doctor-form-container",
            gridSelector: "#doctor-grid",
            gridContainer: ".js-doctor-grid-container",
            previewSelector: ".js-commission-payment-preview",        
            load: function () {

            }
        }, options);

        $(() => {
            loadDoctors(settings.baseUrl, settings.gridSelector);
            initialize();   


            $("body").on("change", ".doctortype, .department, .speciality, .qualification", function ()
            {
                loadDoctors(settings.baseUrl, settings.gridSelector);
            });

            $("body").on("click", ".js-export", function () {
                var self = $(this);
                var doctorTypeCode = $(".doctortype").val();
                var departmentCode = $(".department").val();
                var specialityCode = $(".speciality").val();
                var qualificationCode = $(".qualification").val();

                let reportRenderType = $(".export-format").val();
                window.open(
                    settings.baseUrl + `/Export?doctorTypeCode=${doctorTypeCode}&departmentCode=${departmentCode}&specialityCode=${specialityCode}&qualificationCod=${qualificationCode}&reportType=Doctors&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });
        });    

        function loadDoctors(baseUrl, gridSelector)
        {
            var data =
            {
                doctorTypeCode: $(".doctortype").val(),
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
                    { "data": "doctorCode", "width": "80px","className": "text-center" },
                    { "data": "doctorName", "width": "200px" },
                    { "data": "designationName", "className": "text-center", width: "120px" },
                    { "data": "departmentName", "className": "text-center", width: "120px" },
                    { "data": "joiningDate", "className": "text-center", width: "80px" },
                    { "data": "specialist", "className": "text-center", width: "120px" },
                    { "data": "qualification", "className": "text-center", width: "120px" },
                    { "data": "appointmentDays", "className": "text-left", width: "230px" },
                    { "data": "salary", width: "30px" },
                    { "data": "visitingFee", width: "30px" },
                    { "data": "phone", "className": "text-center", width: "50px" },
                    { "data": "email", "className": "text-center", width: "50px" },
                    {
                        "data": "photoUrl", "className": "text-center", width: "120px",
                        render: function (data) {
                            if (data)
                                return `<img id="photoPreview" class="img-fluid img-thumbnail img-roundedr" src="${getBaseUrl()}/Uploads/Images/Doctors/${data}">`;
                            else
                                return '';
                        }
                    },
                    {
                        "data": "activityStatus", "className": "text-center", width: "120px",
                        render: function (data) {
                            if (data == "Active") {
                                return 'Yes';
                               // return `<span class='btn btn-sm btn-success' title="${data}"><i class='fa fa-check-circle'></i></span>`;
                            } else if (data = "Inactive") {
                                return 'No';
                               // return `<span class='btn btn-sm btn-danger' title="${data}"><i class='fa fa-times-circle'></i></span>`;
                            }

                            return 'No';
                        }
                    }
                ],
                lengthChange: true,
                pageLength: 10,
                order: [[0, "Desc"]],
                sScrollY: "100%",
                scrollX: true,
                sScrollX: "100%",
                bDestroy: true
            });
        }

        function initialize() {
            $('.selectpicker').select2({
                language: {
                    noResults: function ()
                    {
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

