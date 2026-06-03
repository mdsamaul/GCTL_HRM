(function ($) {
    $.logs = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#log-form",
            formContainer: ".js-log-form-container",
            gridSelector: "#log-grid",
            gridContainer: ".js-log-grid-container",
            editSelector: ".js-log-edit",
            saveSelector: ".js-log-save",
            selectAllSelector: "#log-check-all",
            deleteSelector: ".js-log-delete-confirm",
            deleteModal: "#log-delete-modal",
            finalDeleteSelector: ".js-log-delete",
            clearSelector: ".js-log-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-log-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-log-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: "#lastCode",
            load: function () {

            }
        }, options);

        var dataTable
        $(() => {
            loadLogs(settings.baseUrl, settings.gridSelector);

            $("#LogLevel").change(function () {
                dataTable.draw();
            })
        });

        function loadLogs(baseUrl, gridSelector) {
            dataTable = $(gridSelector).DataTable({
                proccessing: true,
                serverSide: true,
                ajax: {
                    url: baseUrl + "/GetLogs",
                    type: 'POST',
                    datatype: "json",
                    data: function (data) {
                        data.filter = {
                            logLevel: $("#LogLevel").val()
                        }
                    },
                    //beforeSend: function () {
                    //    //$('#example > tbody').html(
                    //    //    '<tr class="odd">' +
                    //    //    '<td valign="top" colspan="16" class="dataTables_empty"><i class="fa fa-spinner fa-spin fa-2x fa-fw"></i></td>' +
                    //    //    '</tr>'
                    //    //);
                    //}
                },
                language: {
                    processing: "Fetching Data. Please wait..."
                },
                columnDefs: [
                    { targets: [0], orderable: false },
                ],
                columns: [
                    {
                        "data": "id", "className": "text-center", orderable: false, "render": function (data) {
                            return ` <div class="custom-control custom-checkbox">
                                        <input type="checkbox" class="custom-control-input checkBox" id="${data}" value="${data}">
                                        <label class="custom-control-label" for="${data}"></label>
                                    </div>`;
                        }
                    },
                    {
                        "data": "id", "render": function (data) {
                            return `<a class='btn js-log-edit' data-id='${data}'><i>${data}</i></a>`;
                        }
                    },
                    { "data": "level", "autowidth": true },
                    { "data": "message", "autowidth": true },
                    { "data": "exception", "autowidth": true },
                    {
                        "data": "timeStamp", "autowidth": true, render: function (data) {
                            var date = moment(data);
                            return date.format("DD/MM/YYYY hh:mm A");
                        }
                    }
                ],
                lengthChange: false,
                pageLength: 8,
                order: [[1, "Desc"]],
                sScrollY: "100%",
                scrollX: true,
                sScrollX: "100%",
                bDestroy: true
            });
        }
    }

}(jQuery));

