(function ($) {
    $.accessCodeEntries = function (options) {
        const settings = $.extend({

            onSaved: function (id) {

            },
            onDelete: function () {

            },
            load: function () {

            }
        }, options);

        // Selectors
        const selectors = {
            grid: '#access-code-grid',
            form: '#access-code-form',
            checkAll: '#access-code-check-all',
            btnSave: '.js-access-code-save',
            btnDelete: '.js-access-code-delete-confirm',
            btnClear: '.js-access-code-clear',
            inputCode: '.js-access-code',
            inputName: '.js-access-code-name',
            hiddenLastCode: 'input[name="lastCode"]',
            quickAddModal: "#quickAddModal",
        };

        let dataTable;

        const initialize = () => {
            initGrid();
            bindEvents();
        };

        const initGrid = () => {
            dataTable = $(selectors.grid).DataTable({
                ajax: {
                    url: '/menuTab/GetAccessListTable',
                    //dataSrc: '',
                    type: 'GET',
                    dataSrc: function (json) {
                        return json.data;
                    }
                },
                dom: '<"d-flex justify-content-between align-items-center mb-2"lf>rt<"d-flex justify-content-between align-items-center mt-2"ip>',
                columns: [
                    {
                        data: 'accessCodeId',
                        orderable: false,
                        className: 'text-center px-0 no-sort',
                        width: "40px",
                        render: function (data) {
                            return `<input type="checkbox" class="js-grid-checkbox" value="${data}" />`;
                        }
                    },
                    {
                        data: 'accessCodeId', className: "text-center",
                        render: function (data, type, row) {
                            return `<button type="button" class="btn btn-link p-0 access-code-id-btn" 
                                data-id="${data}">
                                ${data}
                            </button>`;
                        }
                    },
                    { data: 'accessCodeName' }
                ],
                columnDefs: [
                    {
                        targets: [0], orderable: false,
                        createdCell: function (td) {
                            $(td).html(`<div class="d-flex align-items-center justify-content-center px-0">${$(td).html()}</div>`);
                        }
                    }
                ],
                lengthChange: true,
                pageLength: 10,
                lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                autoWidth: false,
                responsive: true,
                fixedHeader: true,
                order: [[1, "desc"]],
                bDestroy: true,
            });
        };

        const bindEvents = () => {
            // Save / Update
            $(selectors.btnSave).on('click', function () {
                saveAccessCode();
            });

            // Multi-Delete
            $(selectors.btnDelete).on('click', function () {
                deleteSelected();
            });

            // Clear Form
            $(selectors.btnClear).on('click', function () {
                clearForm();
            });

            // Select All Checkbox
            $(selectors.checkAll).on('change', function () {
                $('.js-grid-checkbox').prop('checked', $(this).prop('checked'));
            });

            // Row Click (Edit)
            //$('.access-code-id-btn').on('click', 'tr', function () {
            // Don't trigger if clicking the checkbox
            //if ($(e.target).hasClass('js-grid-checkbox')) return;
            $(document).on('click', ".access-code-id-btn", function () {
                const row = $(this).closest('tr');
                const data = dataTable.row(row).data();
                console.log(row);
                if (data) {
                    console.log(data);
                    fillForm(data);
                }
            });
        };

        const saveAccessCode = () => {
            const $form = $(selectors.form);

            if (!$form.valid()) return;

            // Use URLSearchParams or serialize to include the Anti-Forgery Token naturally
            const formData = $form.serialize();

            $.ajax({
                url: '/MenuTab/AccessCodeSetup', // Ensure this matches your Controller Route
                type: 'POST',
                data: formData,
                success: function (response) {
                    //console.log(response);
                    if (response.isSuccess) {
                        alert(response.message || "Saved successfully");
                        if (settings.isModal && typeof settings.onSaved === 'function') {
                            var savedId = response && response.lastCode ? response.lastCode : null;
                            settings.onSaved(savedId);
                            return;
                        }
                        clearForm();
                        initGrid();
                        //dataTable.ajax.reload();
                    } else {
                        alert("Error: " + response.message);
                    }
                },
                error: function (xhr) {
                    alert("An error occurred on the server.");
                }
            });
        };

        const deleteSelected = () => {
            const selectedIds = [];
            $('.js-grid-checkbox:checked').each(function () {
                selectedIds.push($(this).val());
            });

            if (selectedIds.length === 0) {
                alert('Please select at least one item to delete.');
                return;
            }

            if (confirm(`Are you sure you want to delete ${selectedIds.length} items?`)) {
                $.ajax({
                    url: '/menuTab/DeleteAccessCodes',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(selectedIds),
                    success: function (response) {
                        if (settings.isModal && typeof settings.onDelete === 'function') {

                            settings.onDelete();
                            return;
                        }
                        alert('Deleted successfully!');
                        $(selectors.checkAll).prop('checked', false);
                        dataTable.ajax.reload();
                        clearForm();
                    },
                    error: function (xhr) {
                        alert('Error occurred while deleting.');
                    }
                });
            }
        };

        const fillForm = (data) => {
            $(selectors.inputCode).val(data.accessCodeId);
            $(selectors.inputName).val(data.accessCodeName);
            $(selectors.hiddenLastCode).val(data.accessCodeId);
        };

        const clearForm = () => {
            $(selectors.form)[0].reset();
            $(selectors.hiddenLastCode).val('');
            $(selectors.inputCode).prop('readonly', false);
            // Reset validation messages
            $(selectors.form).validate().resetForm();
        };

        initialize();
    };
})(jQuery);














//(function ($) {
//    $.accessCodeEntries = function (options) {
//        var settings = $.extend({
//            baseUrl: "/",
//            lastCodeSelector: "#lastCode",
//            quickAddModal: "#quickAddModal",
//            formSelector: "#access-code-form",
//            saveSelector: ".js-access-code-save",
//            editSelector: ".js-access-code-edit",
//            clearSelector: ".js-access-code-clear",
//            deleteSelector: ".js-access-code-delete-confirm",

//            onSaved: function (deductionTypeId) {

//            },
//            onDelete: function () {

//            },
//            load: function () {

//            }
//        }, options);

//        var gridUrl = '/menuTab/GetAccessListTable';
//        var saveUrl = '/MenuTab/AddAccessCode';
//        var deleteUrl = '/menuTab/DeleteAccessCodes';
//        var selectedItems = [];

//        var namespace = settings.isModal ? '.accessCodeModal' : '.accessCodeStandalone';

//        $(() => {
//            loadAccessCodeGrid();
//            init();

//            $("body").off("click" + namespace).on("click" + namespace, `${settings.editSelector}, ${settings.clearSelector}`, function (e) {
//                e.stopPropagation();
//                e.preventDefault();
//                e.stopImmediatePropagation();

//                let url = saveUrl + "/" + ($(this).data("id") || "");
//                loadForm(url);

//                var id = $(this).data('id');
//                $(settings.deleteSelector).data('id', id);

//                $("html, body").animate({ scrollTop: 0 }, 500);
//            });

//        })

//        function loadAccessCodeGrid() {
//            if (!$("#access-code-grid").length) {
//                return;
//            }

//            var dataTable = $("#access-code-grid").DataTable({
//                ajax: {
//                    url: '/menuTab/GetAccessListTable',
//                    type: "GET",
//                    dataType: "json"
//                },
//                dom: '<"d-flex justify-content-between align-items-center mb-2"lf>rt<"d-flex justify-content-between align-items-center mt-2"ip>',
//                columns: [
//                    {
//                        data: "accessCodeId",
//                        className: "text-center px-0 no-sort",
//                        width: "40px",
//                        render: function (data) {
//                            return `<input type="checkbox" class="text-center align-middle checkBox" value="${data}">`;
//                        }
//                    },
//                    {
//                        data: "accessCodeId",
//                        className: "text-center",
//                        render: function (data, type, row) {
//                            return `<button type="button" class="btn btn-link p-0 access-id-btn"
//                        data-id="${data}"
//                        data-name="${row.accessCodeName}">
//                        ${data}
//                    </button>`;
//                        }
//                    },
//                    { data: "accessCodeName", className: "text-start" }
//                ],
//                columnDefs: [
//                    {
//                        targets: [0], orderable: false,
//                        createdCell: function (td) {
//                            $(td).html(`<div class="d-flex align-items-center justify-content-center px-0">${$(td).html()}</div>`);
//                        }
//                    }
//                ],
//                lengthChange: true,
//                pageLength: 10,
//                lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
//                autoWidth: false,
//                responsive: true,
//                fixedHeader: true,
//                order: [[1, "desc"]],
//                bDestroy: true,
//            });
//        }

//        function init() {
//            setTimeout(function () {
//                const $accessCode = $("#access-code-form").find('#AccessCodeId');
//                if ($accessCode.length) {
//                    $accessCode.focus();
//                }
//            }, 0);

//            $(document).off('keydown' + namespace, settings.formSelector + ' input, ' + settings.formSelector + ' select, ' + settings.formSelector + ' textarea, ' + settings.formSelector + ' button, ' + settings.formSelector + ' [tabindex]:not([tabindex="-1"])')
//                .on('keydown' + namespace, settings.formSelector + ' input, ' + settings.formSelector + ' select, ' + settings.formSelector + ' textarea, ' + settings.formSelector + ' button, ' + settings.formSelector + ' [tabindex]:not([tabindex="-1"])', function (e) {
//                    if (e.key === 'Enter') {
//                        e.preventDefault();

//                        const $form = $(settings.formSelector);
//                        if (!$form.length) return;

//                        const $focusable = $form
//                            .find('input:not([disabled]), select:not([disabled]), textarea:not([disabled]), button, [href], [tabindex]:not([tabindex="-1"])')
//                            .filter(':visible');

//                        const index = $focusable.index(this);
//                        if (index > -1) {
//                            const $next = $focusable.eq(index + 1).length ? $focusable.eq(index + 1) : $focusable.eq(0);
//                            $next.focus();
//                        }
//                    }
//                });
//        }
//    }
//}(jQuery));
