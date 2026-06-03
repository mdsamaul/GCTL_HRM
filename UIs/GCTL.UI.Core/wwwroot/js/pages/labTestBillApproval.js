(function ($) {
    $.labTestBillApproval = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#doctor-form",
            formContainer: ".js-doctor-form-container",
            gridSelector: "#labTestBillApproval-grid",
            gridContainer: ".js-labTestBillApproval-grid-container",
            previewSelector: ".js-commission-payment-preview",        
            load: function () {

            }
        }, options);

        $(() => {
           
            initialize();   
            $(".ApprovedStatus").val("N").change();
            loadLabTestBill(settings.baseUrl, settings.gridSelector);
            $("body").on("click", '.labTestBill-check-all', function () {
                if ($(this).is(":checked")) {
                    $('.checkBox').prop('checked', true);
                    
                } else {
                    $('.checkBox').prop('checked', false);
                }
            });

            $("body").on("click", '.js-LabTestBill-preview', function () {
                loadLabTestBill(settings.baseUrl, settings.gridSelector);
            });

            
            $("body").on("click", ".js-LabTestBillPrintPreview", function () {
                event.preventDefault();
                var labTestNo = $(this).data("id");
                let reportRenderType ="PDF";
                $.ajax({
                    url: settings.baseUrl + "/Export",
                    method: "POST",
                    data: {
                        labTestNo: labTestNo,
                        reportType: "MoneyReceipts",
                        reportRenderType: reportRenderType,
                        isPreview: true
                    },
                    success: function (response) {
                        // $.LoadingOverlay("hide");
                        window.open(
                            normalizeUrl(getBaseUrl()) + `/Preview/MoneyReceipts`,
                            "_blank"
                        )
                        //$("#reportPreivew").attr("data", response);
                    }
                });
            

            });
            $("body").on("click", ".js-Approved", function () {
                event.preventDefault();

                var labTestNo = "";
                $("#labTestBillApproval-grid TBODY TR").each(function () {
                    var row = $(this);
              
                    if (row.find(".checkBox").is(':checked')) {
                     
                        labTestNo += row.find(".checkBox").val() + ",";
                    }
                });               
                if (labTestNo == "") {
                    var message = $("#message");
                    message.css("color", "red");
                    message.html("Please Select From List");
                    return;
                }
                labTestNo = labTestNo.replace(/,\s*$/, "");                
                $.ajax({
                    type: "GET",
                    url: settings.baseUrl + "/UpdateInfo",
                    data: { labTestNo: labTestNo },
                    dataType: "JSON",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {                  
                        loadLabTestBill(settings.baseUrl, settings.gridSelector);
                        var message = $("#message");
                        message.css("color", "red");
                        message.html(data.message);                      
                    }
                });
            });

            $("body").on("click", ".js-Cancle", function () {
                event.preventDefault();
                
                var labTestNo = "";
                $("#labTestBillApproval-grid TBODY TR").each(function () {
                    var row = $(this);

                    if (row.find(".checkBox").is(':checked')) {

                        labTestNo += row.find(".checkBox").val() + ",";
                    }
                });
                if (labTestNo == "") {
                    var message = $("#message");
                    message.css("color", "red");
                    message.html("Please Select From List");
                    return;
                }
                labTestNo = labTestNo.replace(/,\s*$/, "");
                $.ajax({
                    type: "GET",
                    url: settings.baseUrl + "/CancleApprovedInfo",
                    data: { labTestNo: labTestNo },
                    dataType: "JSON",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        loadLabTestBill(settings.baseUrl, settings.gridSelector);
                        var message = $("#message");
                        message.css("color", "red");
                        message.html(data.message);
                    }
                });
            });
            


        });    

        function loadLabTestBill(baseUrl, gridSelector) {
            var data = {
                ApprovedStatus:$(".ApprovedStatus").val(),
                fromDate: $(".datefrom").val(),
                toDate: $(".dateto").val(),
                doctorCode: $(".doctorCode").val(),
                referencePersonId: $(".referencePersonId").val()
            };

            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/TestEntries",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "labTestNo", "className": "text-center", width: "30px",
                        render: function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "labTestNo", "className": "text-center", width: "130px"
                    },
                    { "data": "labTestDateTime", "className": "text-center", width: "130px" },
                    { "data": "mrNo", "className": "text-center", width: "80px" },
                    { "data": "totalAmount", "className": "text-right", width: "80px" },
                    { "data": "discount", "className": "text-right", width: "80px" },
                    { "data": "payable", "className": "text-right", width: "80px" },
                    { "data": "due", "className": "text-right", width: "80px" },
                    { "data": "patientCode", "className": "text-center", width: "80px" },
                    { "data": "patientName", "className": "text-left", width: "150px" },
                    { "data": "phone", "width": "50px" },
                    /*{ "data": "registrationId", "className": "text-center", width: "50px" },*/
                    {
                        "data": "doctorName", "className": "text-left", width: "130px"
                    },
                    { "data": "referencePerson", "className": "text-left", width: "130px" },
                    { "data": "isApproved", "className": "text-center", width: "130px" },
                    {
                        "data": "labTestNo", "render": function (data, type, row) {
                            var actions = `<div class='action-buttons p-1'>
                                       
                                        <button type="button" class="btn btn-danger btn-circle btn-sm js-LabTestBillPrintPreview"
                                                data-id="${data}">
                                                    <i class="fas fa-print fa-sm"></i>
                                        </button>`;

                          

                            return actions;
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

