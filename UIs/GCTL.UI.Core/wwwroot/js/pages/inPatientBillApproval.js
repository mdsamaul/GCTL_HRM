(function ($) {
    $.inPatientBillApproval = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#doctor-form",
            formContainer: ".js-doctor-form-container",
            gridSelector: "#inPatientBillApproval-grid",
            gridContainer: ".js-inPatientBillApproval-grid-container",
            previewSelector: ".js-commission-payment-preview",
            load: function () {

            }
        }, options);

        $(() => {

            initialize();
            $(".ApprovedStatus").val("N").change();
            loadInPatientBill(settings.baseUrl, settings.gridSelector);
            $("body").on("click", '.inPatientBillApproval-check-all', function () {
                if ($(this).is(":checked")) {
                    $('.checkBox').prop('checked', true);

                } else {
                    $('.checkBox').prop('checked', false);
                }
            });

            $("body").on("click", '.js-inPatientBillApproval-preview', function () {
                loadInPatientBill(settings.baseUrl, settings.gridSelector);
            });


            $("body").on("click", ".js-inPatientBillApprovalPrintPreview", function () {
                event.preventDefault();
                var billEntryNo = $(this).data("id");
                let reportRenderType = "PDF";
                $.ajax({
                    url: settings.baseUrl + "/Export",
                    method: "POST",
                    data: {
                        billEntryNo: billEntryNo,
                        reportType: "InPatientBill",
                        reportRenderType: reportRenderType,
                        isPreview: true
                    },
                    success: function (response) {
                        window.open(
                            normalizeUrl(getBaseUrl()) + `/Preview/InPatientBill`,
                            "_blank"
                        )
                        
                    }
                });


            });
            $("body").on("click", ".js-Approved", function () {
                event.preventDefault();
                var billEntryNo = "";
                $("#inPatientBillApproval-grid TBODY TR").each(function () {
                    var row = $(this);
                    if (row.find(".checkBox").is(':checked')) {
                        billEntryNo += row.find(".checkBox").val() + ",";
                    }
                });

                if (billEntryNo == "") {
                    var message = $("#message");
                    message.css("color", "red");
                    message.html("Please Select From List");
                    return;
                }
                billEntryNo = billEntryNo.replace(/,\s*$/, "");
                $.ajax({
                    type: "GET",
                    url: settings.baseUrl + "/UpdateInfo",
                    data: { billEntryNo: billEntryNo },
                    dataType: "JSON",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        loadInPatientBill(settings.baseUrl, settings.gridSelector);
                        var message = $("#message");
                        message.css("color", "red");
                        message.html(data.message);
                    }
                });
            });

            $("body").on("click", ".js-Cancle", function () {
                event.preventDefault();
                var billEntryNo = "";
                $("#inPatientBillApproval-grid TBODY TR").each(function () {
                    var row = $(this);
                    if (row.find(".checkBox").is(':checked')) {
                        billEntryNo += row.find(".checkBox").val() + ",";
                    }
                });

                if (billEntryNo == "") {
                    var message = $("#message");
                    message.css("color", "red");
                    message.html("Please Select From List");
                    return;
                }
                billEntryNo = billEntryNo.replace(/,\s*$/, "");
                $.ajax({
                    type: "GET",
                    url: settings.baseUrl + "/CancleApprovedInfo",
                    data: { billEntryNo: billEntryNo },
                    dataType: "JSON",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        loadInPatientBill(settings.baseUrl, settings.gridSelector);
                        var message = $("#message");
                        message.css("color", "red");
                        message.html(data.message);
                    }
                });
            });

        });

        function loadInPatientBill(baseUrl, gridSelector) {
            var data = {
                ApprovedStatus: $(".ApprovedStatus").val(),
                fromDate: $(".datefrom").val(),
                toDate: $(".dateto").val(),
                doctorCode: $(".doctorCode").val(),
                referencePersonId: $(".referencePersonId").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/BillEntries",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "billEntryNo", "className": "text-center", width: "30px",
                        render: function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "billEntryNo", "className": "text-center", width: "130px"
                    },
                    { "data": "billEntryDateTime", "className": "text-center", width: "130px" },
                    { "data": "netAmount", "className": "text-center", width: "100px" },
                    { "data": "patientCode", "className": "text-center", width: "80px" },
                    { "data": "patientName", "className": "text-left", width: "150px" },
                    { "data": "phone", "width": "50px" },
                    { "data": "registrationId", "className": "text-center", width: "50px" },
                    {
                        "data": "doctorName", "className": "text-left", width: "130px"
                    },
                    { "data": "referencePerson", "className": "text-left", width: "130px" },
                    { "data": "isApproved", "className": "text-center", width: "130px" },
                    {
                        "data": "billEntryNo", "render": function (data, type, row) {
                            return `<div class='action-buttons p-1'>
                                           <button type="button" class="btn btn-danger btn-circle btn-sm js-inPatientBillApprovalPrintPreview"
                                                data-id="${data}">
                                                    <i class="fas fa-print fa-sm"></i>
                                        </button></div>`;
                        },
                        "orderable": false,
                        "searchable": false,
                        width: "100px"
                    }
                ],
                lengthChange: false,
                pageLength: 10,
                order: [],
                sScrollY: "100%",
                scrollX: true,
                sScrollX: "100%",
                bDestroy: true
            });
        }

        function initialize() {
            $('.selectpicker').select2({
                language: {
                    noResults: function () {
                        //return 'Not found <a class="add_new_item" href="javascript:void(0)">Add New</a>';
                    }
                },
                dropdownAutoWidth: true,
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
    }
}(jQuery));

