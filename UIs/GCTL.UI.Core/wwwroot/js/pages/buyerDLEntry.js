(function ($) {
    const CONFIG = {
        namespace: 'dlAddressModule',
        baseUrl: '/BuyerInfo',

        formSelector: '#buyerDLForm',
        fieldPrefix: 'DLAddress',
        gridSelector: '#dlAddress-grid',
        gridBodySelector: '#dlAddress-grid-body',

        saveSelector: '.js-buyer-group-save',
        clearSelector: '#js-buyer-group-clear',
        deleteSelector: '#js-buyer-group-delete-confirm',
        selectAllSelector: '#dlAddress-check-all',
        select2selector: '.dlSelect',

        idLinkClass: 'dl-address-id-link',
        dateCreateClass: '[name="DLAddress.LDate"]',
        dateModifyClass: '[name="DLAddress.ModifyDate"]',
        dateInfoClass: '#DLAddress_DateInfo',

        fields: {
            tc: 'Tc',
            id: 'DeliveryAddressId',
            name: 'Name',
            buyerId: 'BuyerId',
            additional: [
                'DeliveryAddress',
                'ContactPerson',
                'Designation',
                'Phone',
                'Email'
            ]
        },

        apiFieldMap: {
            tc: 'tc',
            id: 'deliveryAddressId',
            name: 'name',
            buyerid: 'buyerId',
            deliveryaddress: 'deliveryAddress',
            contactperson: 'contactPerson',
            designation: 'designation',
            phone: 'phone',
            email: 'email',
            createDate: 'ldate',
            modifyDate: 'modifyDate'
        },

        tableColumns: [
            {
                data: null,
                orderable: false,
                className: 'text-center no-sort',
                width: '5%',
                render: function (data, type, row) {
                    return `<input type="checkbox" class="py-0 no-sort" data-id="${row.tc}"/>`;
                }
            },
            {
                data: 'deliveryAddressId',
                className: 'text-center',
                width: '10%',
                render: function (data, type, row) {
                    return `<a href="#buyerDLForm" class="py-0 dl-address-id-link" data-id="${row.tc}">${data}</a>`;
                }
            },
            {
                data: 'name',
                className: 'py-0 text-left',
                width: '15%'
            },
            {
                data: 'deliveryAddress',
                className: 'py-0 text-left',
                width: '20%'
            },
            {
                data: 'contactPerson',
                className: 'py-0 text-center',
                width: '15%'
            },
            {
                data: 'designation',
                className: 'py-0 text-center',
                width: '12%'
            },
            {
                data: 'phone',
                className: 'py-0 text-center',
                width: '12%'
            },
            {
                data: 'email',
                className: 'py-0 text-center',
                width: '11%'
            }
        ],

        tableOptions: {
            processing: true,
            serverSide: true,
            autoWidth: false,
            fixedHeader: false,
            info: true,
            lengthChange: true,
            lengthMenu: [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]],
            order: [[1, 'desc']],
            ordering: true,
            pageLength: 5,
            paging: true,
            responsive: true,
            scrollCollapse: true,
            scrollX: true,
            searching: true,

            language: {
                search: "🔍 Search:",
                lengthMenu: "Show _MENU_ entries",
                info: "Showing _START_ to _END_ of _TOTAL_ entries",
                paginate: {
                    first: "First",
                    previous: "Prev",
                    next: "Next",
                    last: "Last"
                },
                emptyTable: "No data available",
                processing: "Loading data..."
            }
        },

        features: {
            select2: true,
            quickAdd: false,
            contactPerson: false,
            photo: false
        }
    };

    const URLS = {
        save: CONFIG.baseUrl + '/SaveBuyerDLAddress',
        list: CONFIG.baseUrl + '/GetBuyerDLAddressList',
        details: CONFIG.baseUrl + '/GetBuyerDLAddressById',
        delete: CONFIG.baseUrl + '/BulkBuyerDLAddressDelete'
    };

    const state = {
        selectedIds: new Set(),
        isEditMode: false,
        currentTable: null
    };

    // ========================================================================
    // MAIN MODULE INITIALIZATION
    // ========================================================================

    $(document).ready(function () {
        init();
    });

    function init() {
        setupLoadingOverlay();
        bindEvents();
        loadTableData();

        if (CONFIG.features.select2) {
            initializeSelect();
        }

        setupEnterKeyNavigation();
    }

    function bindEvents() {
        const ns = `.${CONFIG.namespace}`;

        $("body").off(ns);
        $(CONFIG.gridSelector).off(ns);

        $("body").on(`click${ns}`, CONFIG.saveSelector, function () {
            if (getActiveTab() === 'dladdress') handleFormSubmit();
        });

        $("body").on(`click${ns}`, CONFIG.clearSelector, function () {
            console.log(getActiveTab());
            if (getActiveTab() === 'dladdress') {
                clearForm();
                clearAllSelections();
            }
        });

        $("body").on(`click${ns}`, CONFIG.deleteSelector, function () {
            if (getActiveTab() === 'dladdress') handleBulkDelete();
        });

        $(document).on(`click${ns}`, `.${CONFIG.idLinkClass}`, function (e) {
            e.preventDefault();
            const id = $(this).data("id");
            if (id) populateForm(id);
        });

        $(CONFIG.selectAllSelector).off(`change${ns}`).on(`change${ns}`, function () {
            const isChecked = $(this).is(':checked');
            $(`${CONFIG.gridBodySelector} input[type="checkbox"]`).prop('checked', isChecked);
            updateSelectedIds();
        });

        $(document).on(`change${ns}`, `${CONFIG.gridBodySelector} input[type="checkbox"]`, function () {
            const id = $(this).data('id');
            if ($(this).is(':checked')) {
                state.selectedIds.add(id);
            } else {
                state.selectedIds.delete(id);
            }
            updateCheckAllState();
        });

        // Search functionality
        const debouncedDLBuyerFilter = debounce((id) => { loadTableData(null, id) }, 300);
        const debouncedDLSearch = debounce((id) => { loadTableData(id) }, 300);

        $(`#${CONFIG.fieldPrefix}_${CONFIG.fields.buyerId}`).on('change', function () {
            const id = $(this).val();
            if (id) debouncedDLBuyerFilter(id);
            else loadTableData();
        });

        $(`#${CONFIG.fieldPrefix}_Search`).on('click', () => {
            const id = $(`#${CONFIG.fieldPrefix}_${CONFIG.fields.id}`).val();
            if (id) debouncedDLSearch(id);
            else loadTableData();
        });
    }

    function getActiveTab() {
        const tabMap = {
            'nav-buyer-tab': 'buyer',
            'nav-brand-tab': 'brand',
            'nav-dladdress-tab': 'dladdress'
        };
        return tabMap[$('.nav-link.active').attr('id')] || null;
    }

    function setupLoadingOverlay() {
        if ($("#loadingOverlay").length === 0) {
            $("body").append(`
                <div id="loadingOverlay" style="
                    display: none;
                    position: fixed;
                    top: 0;
                    left: 0;
                    width: 100%;
                    height: 100%;
                    background-color: rgba(0, 0, 0, 0.5);
                    z-index: 9999;
                    justify-content: center;
                    align-items: center;">
                    <div style="
                        background-color: white;
                        padding: 20px;
                        border-radius: 5px;
                        box-shadow: 0 0 10px rgba(0,0,0,0.3);
                        text-align: center;">
                        <div class="spinner-border text-primary" role="status">
                        </div>
                    </div>
                </div>
            `);
        }
    }

    function showLoading() {
        $('body').css('overflow', 'hidden');
        $("#loadingOverlay").css('display', 'flex').fadeIn(200);
    }

    function hideLoading() {
        $('body').css('overflow', '');
        $("#loadingOverlay").fadeOut(200);
    }

    function formatDate(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        if (isNaN(date)) return '';

        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();

        return `${day}/${month}/${year}`;
    }

    function showNotification(message, type = 'info') {
        if (typeof toastr !== 'undefined') {
            const title = { success: 'Success', error: 'Error', warning: 'Warning' }[type] || 'Info';
            toastr[type](message, title);
        } else {
            alert(message);
        }
    }

    function initializeSelect(containerSelector) {
        const $container = containerSelector
            ? $(containerSelector)
            : $(CONFIG.formSelector);

        $container.find(CONFIG.select2selector).each(function () {
            const $select = $(this);

            if ($select.data('select2')) {
                return;
            }

            if (!containerSelector && $select.closest('.modal-body').length > 0) {
                return;
            }

            const placeholderText = $select.find('option[value=""]').text().trim() || 'Select an option';

            $select.select2({
                width: '100%',
                allowClear: true,
                placeholder: placeholderText,
                language: { noResults: () => 'No results found' },
                escapeMarkup: markup => markup
            });
        });
    }

    function showConfirmation(message, title, callback) {
        if (typeof Swal === 'undefined') {
            if (confirm(`${message}`)) {
                callback();
            }
            return;
        }

        Swal.fire({
            title: title,
            text: message,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, proceed!',
            cancelButtonText: 'No, cancel'
        }).then(function (result) {
            if (result.value === true || result.isConfirmed) {
                callback();
            }
        });
    }

    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    function getFieldSelector(fieldName) {
        return `[name="${CONFIG.fieldPrefix}.${fieldName}"]`;
    }

    function setFieldValue(fieldName, value) {
        $(getFieldSelector(fieldName)).val(value);
    }

    function getFieldValue(fieldName) {
        return $(getFieldSelector(fieldName)).val();
    }

    function populateForm(id) {
        $.ajax({
            url: URLS.details,
            data: { id },
            type: "GET",
            success: function (res) {
                if (!res || !res.data) {
                    showNotification("Data not found", "error");
                    return;
                }

                try {
                    const data = res.data;
                    const map = CONFIG.apiFieldMap;

                    setFieldValue(CONFIG.fields.tc, data[map.tc]);
                    setFieldValue(CONFIG.fields.id, data[map.id]);
                    setFieldValue(CONFIG.fields.name, data[map.name]);
                    setFieldValue(CONFIG.fields.buyerId, data[map.buyerid]);

                    CONFIG.fields.additional.forEach(fieldName => {
                        const apiKey = map[fieldName.toLowerCase()] || fieldName.toLowerCase();
                        if (data[apiKey] !== undefined) {
                            setFieldValue(fieldName, data[apiKey]);
                        }
                    });

                    $(CONFIG.formSelector).find('.searchableSelect, .searchable-select, .dlSelect').trigger('change');

                    // Disable buyer dropdown and add hidden input
                    const $buyerSelect = $(getFieldSelector(CONFIG.fields.buyerId));
                    $buyerSelect.prop('disabled', true);

                    if (!$('.buyer-id-hidden').length) {
                        $('<input>').attr({
                            type: 'hidden',
                            name: `${CONFIG.fieldPrefix}.${CONFIG.fields.buyerId}`,
                            class: 'buyer-id-hidden'
                        }).val(data[map.buyerid]).insertAfter($buyerSelect);
                    }

                    const idSelector = getFieldSelector(CONFIG.fields.id);
                    $(idSelector).prop('readonly', true);

                    $(CONFIG.dateCreateClass).text(formatDate(data[map.createDate]));
                    $(CONFIG.dateModifyClass).text(formatDate(data[map.modifyDate]));

                    state.isEditMode = true;
                    handleDateInfoDisplay();

                    $("html, body").animate({ scrollTop: 0 }, 500);

                } catch (e) {
                    console.error("Error populating form:", e);
                    showNotification("Error loading record details", "error");
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching record:", error);
                showNotification("Failed to load record details", "error");
            }
        });
    }

    function validateForm() {
        const buyerId = getFieldValue(CONFIG.fields.buyerId);
        const deliveryAddress = getFieldValue('DeliveryAddress');
        const email = getFieldValue('Email');

        if (!buyerId || $.trim(buyerId) === '') {
            showNotification("Buyer is required.", "warning");
            return false;
        }

        if (!deliveryAddress || $.trim(deliveryAddress) === '') {
            showNotification("Delivery Address is required.", "warning");
            return false;
        }

        if (email && $.trim(email) !== '') {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email)) {
                showNotification("Please enter a valid email address.", "warning");
                return false;
            }
        }

        return true;
    }

    function handleFormSubmit() {
        if (!validateForm()) return;

        const empId = getFieldValue(CONFIG.fields.buyerId);
        showLoading();

        const formData = new FormData($(CONFIG.formSelector)[0]);

        $.ajax({
            url: URLS.save,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    showNotification(response.message, "success");
                    clearForm();

                    // Re-select buyer after clear
                    setFieldValue(CONFIG.fields.buyerId, empId);
                    $(getFieldSelector(CONFIG.fields.buyerId)).trigger('change');

                    loadTableData();
                } else {
                    showNotification(response.message, "error");
                }
            },
            error: function () {
                showNotification('Error occurred while saving.', "error");
            },
            complete: hideLoading
        });
    }

    function clearForm() {
        $(CONFIG.formSelector)[0].reset();
        setFieldValue(CONFIG.fields.tc, 0);

        $(CONFIG.formSelector).find('.searchableSelect, .searchable-select, .dlSelect').each(function () {
            if ($(this).data('select2')) {
                $(this).val('').trigger('change');
            } else {
                $(this).val('');
            }
        });

        $(CONFIG.formSelector).find('select').each(function () {
            $(this).find('option:first').prop('selected', true);
            if ($(this).data('select2')) {
                $(this).trigger('change');
            }
        });

        // Re-enable buyer dropdown and remove hidden input
        const $buyerSelect = $(getFieldSelector(CONFIG.fields.buyerId));
        if ($buyerSelect.prop('disabled')) {
            $buyerSelect.prop('disabled', false);
            $('.buyer-id-hidden').remove();
        }

        const idSelector = getFieldSelector(CONFIG.fields.id);
        $(idSelector).prop('readonly', false);

        state.isEditMode = false;
        handleDateInfoDisplay();
    }

    function clearAllSelections() {
        state.selectedIds.clear();
        $(`${CONFIG.gridBodySelector} input[type="checkbox"]`).prop('checked', false);
        $(CONFIG.selectAllSelector).prop('checked', false);
    }

    function handleDateInfoDisplay() {
        if (state.isEditMode) {
            $(CONFIG.dateInfoClass).removeClass('d-none');
        } else {
            $(CONFIG.dateInfoClass).addClass('d-none');
            $(CONFIG.dateCreateClass).text('');
            $(CONFIG.dateModifyClass).text('');
        }
    }

    function loadTableData(id = null, buyerId = null) {
        displayTableData(id, buyerId);
    }

    function displayTableData(id = null, buyerId = null) {
        if ($.fn.DataTable.isDataTable(CONFIG.gridSelector)) {
            $(CONFIG.gridSelector).DataTable().clear().destroy();
        }

        $(CONFIG.gridBodySelector).empty();

        const filterData = {};
        if (id) filterData.id = id;
        if (buyerId) filterData.buyerId = buyerId;

        state.currentTable = $(CONFIG.gridSelector).DataTable({
            ...CONFIG.tableOptions,
            ajax: {
                url: URLS.list,
                type: 'POST',
                data: function (d) {
                    return { ...d, ...filterData };
                }
            },
            columns: CONFIG.tableColumns,
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    className: 'no-sort'
                }
            ],
            initComplete: function () {
                setupTableSearch(this.api());
            },
            drawCallback: function () {
                restoreCheckboxStates();
                updateCheckAllState();
            }
        });
    }

    function setupTableSearch(api) {
        const tableId = CONFIG.gridSelector.replace('#', '');
        let debounceTimeout;

        $(`#${tableId}_wrapper .dataTables_filter input`)
            .off('input.custom')
            .on('input.custom', function () {
                clearTimeout(debounceTimeout);
                const searchTerm = this.value;

                debounceTimeout = setTimeout(function () {
                    api.search(searchTerm).page('first').draw('page');
                }, 500);
            });
    }

    function restoreCheckboxStates() {
        $(`${CONFIG.gridBodySelector} input[type="checkbox"]`).each(function () {
            const id = $(this).data('id');
            $(this).prop('checked', state.selectedIds.has(id));
        });
    }

    function updateSelectedIds() {
        const checkboxes = $(`${CONFIG.gridBodySelector} input[type="checkbox"]`);

        checkboxes.each(function () {
            const id = $(this).data('id');
            if ($(this).is(':checked')) {
                state.selectedIds.add(id);
            } else {
                state.selectedIds.delete(id);
            }
        });
    }

    function updateCheckAllState() {
        const total = $(`${CONFIG.gridBodySelector} input[type="checkbox"]`).length;
        const checked = $(`${CONFIG.gridBodySelector} input[type="checkbox"]:checked`).length;
        $(CONFIG.selectAllSelector).prop('checked', total > 0 && total === checked);
    }

    function handleBulkDelete() {
        const selectedIds = Array.from(state.selectedIds);
        if (selectedIds.length === 0) {
            showNotification("Please select records to delete", "warning");
            return;
        }

        showConfirmation(
            `Are you sure you want to delete ${selectedIds.length} selected record(s)?`,
            'Confirmation',
            function () {
                executeDelete(selectedIds);
            }
        );
    }

    function executeDelete(selectedIds) {
        showLoading();
        $.ajax({
            url: URLS.delete,
            type: "DELETE",
            contentType: "application/json",
            data: JSON.stringify(selectedIds),
            success: function (response) {
                state.selectedIds.clear();
                showNotification(response.message || "Successfully deleted", "success");
                clearForm();
                loadTableData();
            },
            error: function (xhr) {
                console.error("Error details:", xhr.responseText);
                showNotification("Error deleting records", "error");
            },
            complete: hideLoading
        });
    }

    function setupEnterKeyNavigation() {
        const $form = $(CONFIG.formSelector);
        if (!$form.length) return;

        const ns = `.${CONFIG.namespace}Enter`;
        $form.off(ns).on(`keydown${ns}`, 'input, select, textarea, button, [tabindex]:not([tabindex="-1"])', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();

                const $focusable = $form
                    .find('input:not([disabled]), select:not([disabled]), textarea:not([disabled]), button, [href], [tabindex]:not([tabindex="-1"])')
                    .filter(':visible');
                const index = $focusable.index(this);
                if (index > -1) {
                    const $next = $focusable.eq(index + 1).length ?
                        $focusable.eq(index + 1) : $focusable.eq(0);
                    $next.focus();
                }
            }
        });
    }

    //// Expose public API
    //window.DLAddressModule = {
    //    loadTableData: loadTableData,
    //    clearDLForm: clearForm
    //};

}(jQuery));



// dladdress.js - Delivery Address specific functionality

//const DLAddressModule = (() => {
//    const URLs = {
//        Save: '/BuyerInfo/SaveBuyerDLAddress',
//        List: '/BuyerInfo/GetBuyerDLAddressList',
//        Details: '/BuyerInfo/GetBuyerDLAddressById',
//        Delete: '/BuyerInfo/BulkBuyerDLAddressDelete'
//    };

//    let checkboxManager;
//    let dlAddressTable = null;

//    // Initialize module
//    const init = () => {
//        checkboxManager = new CheckboxManager('dlAddress-grid', 'dlAddress-check-all');
//        loadTableData();
//        initEventHandlers();
//    };

//    // Event handlers
//    const initEventHandlers = () => {
//        $(".js-buyer-group-save").off('click.dladdress').on('click.dladdress', () => {
//            if (getActiveTab() === 'dladdress') handleFormSubmit();
//        });

//        $("#js-buyer-group-delete-confirm").off('click.dladdress').on('click.dladdress', () => {
//            if (getActiveTab() === 'dladdress') handleBulkDelete();
//        });

//        $("#js-buyer-group-clear").off('click.dladdress').on('click.dladdress', () => {
//            if (getActiveTab() === 'dladdress') clearDLForm();
//        });

//        const debouncedDLBuyerFilter = debounce((id) => {
//            loadTableData(null, id);
//        }, 300);

//        const debouncedDLSearch = debounce((id) => {
//            loadTableData(id);
//        }, 300);

//        $('#DLAddress_BuyerId').on('change', function () {
//            const id = $(this).val();
//            if (id) debouncedDLBuyerFilter(id);
//            else loadTableData();
//        });

//        $('#DLAddress_Search').on('click', () => {
//            const id = $('#DLAddress_DeliveryAddressId').val();
//            if (id) debouncedDLSearch(id);
//            else loadTableData();
//        });

//        $(document).on("click", ".dl-address-id-link", function () {
//            const id = $(this).data("id");
//            if (id) populateForm(id);
//        });
//    };

//    // Populate form with data
//    const populateForm = (id) => {
//        $.ajax({
//            url: URLs.Details,
//            data: { id },
//            type: "GET",
//            success: (res) => {
//                if (!res?.data) {
//                    showNotification("Data not found", "error");
//                    return;
//                }

//                const data = res.data;
//                console.log(data);
//                const fields = {
//                    'DLAddress.Tc': data.tc,
//                    'DLAddress.BuyerId': data.buyerId,
//                    'DLAddress.DeliveryAddressId': data.deliveryAddressId,
//                    'DLAddress.Name': data.name,
//                    'DLAddress.DeliveryAddress': data.deliveryAddress,
//                    'DLAddress.ContactPerson': data.contactPerson,
//                    'DLAddress.Designation': data.designation,
//                    'DLAddress.Phone': data.phone,
//                    'DLAddress.Email': data.email
//                };

//                Object.entries(fields).forEach(([name, value]) => {
//                    const $field = $(`[name="${name}"]`);
//                    $field.val(value);
//                    if ($field.hasClass('searchable-select')) {
//                        $field.trigger('change');
//                    }
//                });

//                // Handle buyer ID with hidden field for disabled dropdown
//                $('[name="DLAddress.BuyerId"]').prop('disabled', true);
//                if (!$('[name="DLAddress.BuyerId_hidden"]').length) {
//                    $('<input>').attr({
//                        type: 'hidden',
//                        name: 'DLAddress.BuyerId',
//                        class: 'buyer-id-hidden'
//                    }).val(data.buyerId).insertAfter('[name="DLAddress.BuyerId"]');
//                }

//                $('[name="DLAddress.DeliveryAddressId"]').prop('readonly', true);

//                $('[name="DLAddress.LDate"]').text(formatedDateddMMyyyy(data.ldate));
//                $('[name="DLAddress.ModifyDate"]').text(formatedDateddMMyyyy(data.modifyDate));
//                handleDateInfoPartial('DLAddress', true);
//            },
//            error: (xhr, status, error) => {
//                console.error("Error fetching record:", error);
//                showNotification("Failed to load record details", "error");
//            }
//        });
//    };

//    // Load table data
//    const loadTableData = (id = null, buyerId = null) => {
//        const filterData = {};
//        if (id) filterData.id = id;
//        if (buyerId) filterData.buyerId = buyerId;

//        AjaxRequestManager.abort('dladdress-list');

//        if (dlAddressTable !== null) {
//            dlAddressTable.settings()[0].ajax.data = function (d) {
//                return { ...d, ...filterData };
//            }
//            dlAddressTable.ajax.reload(null, false);
//            return;
//        }

//        dlAddressTable = initDataTable('dlAddress-grid', {
//            ajax: {
//                url: URLs.List,
//                type: 'POST',
//                data: function (d) {
//                    return { ...d, ...filterData };
//                },
//                beforeSend: function (xhr) {
//                    AjaxRequestManager.register('dladdress-list', xhr);
//                },
//                error: function (xhr, error, thrown) {
//                    if (error !== 'abort') {
//                        console.error("Error fetching data:", error);
//                        showNotification("Failed to load data", "error");
//                    }
//                },
//                complete: function (xhr) {
//                    AjaxRequestManager.remove('dladdress-list');
//                }
//            },
//            columns: [
//                {
//                    data: null,
//                    orderable: false,
//                    className: 'text-center',
//                    render: (data, type, row) => `<input type="checkbox" class="py-0" data-id="${row.tc}"/>`
//                },
//                {
//                    data: 'deliveryAddressId',
//                    className: 'text-center',
//                    render: (data, type, row) => `<a href="#buyerDLForm" class="py-0 dl-address-id-link" data-id="${row.tc}">${data}</a>`
//                },
//                { data: 'name', className: 'py-0 text-left' },
//                { data: 'deliveryAddress', className: 'py-0 text-left' },
//                { data: 'contactPerson', className: 'py-0 text-center' },
//                { data: 'designation', className: 'py-0 text-center' },
//                { data: 'phone', className: 'py-0 text-center' },
//                { data: 'email', className: 'py-0 text-center' }
//            ],
//            columnDefs: [{ targets: 0, orderable: false, className: 'no-sort' }],
//            drawCallback: function () {
//                checkboxManager.syncWithTable();
//            }
//        });

//    };

//    // Form validation
//    const validateForm = () => {
//        const buyerId = $('#DLAddress_BuyerId').val()?.trim();
//        const deliveryAddress = $('#DLAddress_DeliveryAddress').val()?.trim();
//        const email = $('#DLAddress_Email').val()?.trim();

//        if (!buyerId) {
//            showNotification("Buyer is required.", "warning");
//            return false;
//        }

//        if (!deliveryAddress) {
//            showNotification("Delivery Address is required.", "warning");
//            return false;
//        }

//        if (email) {
//            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
//            if (!emailRegex.test(email)) {
//                showNotification("Please enter a valid email address.", "warning");
//                return false;
//            }
//        }

//        return true;
//    };

//    // Handle form submission
//    const handleFormSubmit = () => {
//        if (!validateForm()) return;

//        let empId = $('#DLAddress_BuyerId').val();

//        LoadingOverlay.show();

//        $.ajax({
//            url: URLs.Save,
//            type: 'POST',
//            data: new FormData($('#buyerDLForm')[0]),
//            processData: false,
//            contentType: false,
//            success: (response) => {
//                if (response.success) {
//                    showNotification(response.message, "success");
//                    clearDLForm();
//                    $('#DLAddress_BuyerId').val(empId).trigger('change');

//                } else {
//                    showNotification(response.message, "error");
//                }
//            },
//            error: () => showNotification('Error occurred while saving delivery address information.', "error"),
//            complete: LoadingOverlay.hide
//        });
//    };

//    // Clear form
//    const clearDLForm = () => {
//        $('#buyerDLForm')[0].reset();
//        $('#DLAddress_Tc').val(0);

//        $('.dlSelect').each(function () {
//            if ($(this).data('select2')) {
//                $(this).val(null).trigger('change');
//            } else {
//                $(this).val('');
//            }
//        });

//        loadTableData();

//        handleDateInfoPartial('DLAddress', false);
//        checkboxManager.clear();
//        if ($('#DLAddress_BuyerId').prop('disabled')) {
//            $('#DLAddress_BuyerId').prop('disabled', false);
//            $('.buyer-id-hidden').remove();
//        }

//        $('#DLAddress_DeliveryAddressId').prop('readonly', false);
//    };

//    // Handle bulk delete
//    const handleBulkDelete = () => {
//        const selectedIds = checkboxManager.getSelected();

//        if (selectedIds.length === 0) {
//            showNotification("Please select record to delete", "warning");
//            return;
//        }

//        showConfirmation(`Are you sure you want to delete ${selectedIds.length} selected Data(s)?`,
//            'Confirmation',
//            () => {
//                LoadingOverlay.show();

//                $.ajax({
//                    url: URLs.Delete,
//                    type: "DELETE",
//                    contentType: "application/json",
//                    data: JSON.stringify(selectedIds),
//                    success: (response) => {
//                        checkboxManager.clear();
//                        showNotification(response.message || "Successfully deleted", "success");
//                        //loadTableData();
//                        clearDLForm();
//                    },
//                    error: (xhr) => {
//                        console.error("Error details:", xhr.responseText);
//                        showNotification("Error deleting records", "error");
//                    },
//                    complete: LoadingOverlay.hide
//                });
//            }
//        )
//    };

//    // Public API
//    return { init, loadTableData, clearDLForm };
//})();

//// Initialize on document ready
//$(document).ready(() => DLAddressModule.init());

////let selectedDLAddressId = new Set();

////const DlUrl = {
////    Save: '/BuyerInfo/SaveBuyerDLAddress',
////    List: '/BuyerInfo/GetBuyerDLAddressList',
////    Details: '/BuyerInfo/GetBuyerDLAddressById',
////    Delete: `/BuyerInfo/BulkBuyerDLAddressDelete`
////}

////$(document).ready(function () {
////    setupLoadingOverlay();
////    initializeDLEventHandlers();
////    //loadDLAddressTableData();
////    initializeSelect();
////});

////// #region default
////function initializeSelect() {
////    $('.searchable-select').select2({
////        width: '100%',
////        language: {
////            noResults: () => 'No results found'
////        },
////        escapeMarkup: (markup) => markup
////    });
////}

////function setupLoadingOverlay() {
////    console.log("Loading");
////    if ($("#loadingOverlay").length === 0) {
////        $("body").append(`
////            <div id="loadingOverlay" style="
////                display: none;
////                position: fixed;
////                top: 0;
////                left: 0;
////                width: 100%;
////                height: 100%;
////                background-color: rgba(0, 0, 0, 0.5);
////                z-index: 9999;
////                justify-content: center;
////                align-items: center;">
////                <div style="
////                    background-color: white;
////                    padding: 20px;
////                    border-radius: 5px;
////                    box-shadow: 0 0 10px rgba(0,0,0,0.3);
////                    text-align: center;">
////                    <div class="spinner-border text-primary" role="status">
////                    </div>
////                </div>
////            </div>
////        `);
////    }
////}

////function showLoading() {
////    $('body').css('overflow', 'hidden');
////    $("#loadingOverlay").css('display', 'flex').fadeIn(200);
////}

////function hideLoading() {
////    $('body').css('overflow', '');
////    $("#loadingOverlay").fadeOut(200);
////}

////function setupEnterKeyNavigation() {
////    const $form = $('#buyerDLForm');
////    if (!$form.length) return;

////    $form.on('keydown', 'input, select, textarea, button, [tabindex]:not([tabindex="-1"])', function (e) {
////        if (e.key === 'Enter') {
////            e.preventDefault();

////            const $focusable = $form
////                .find('input:not([disabled]), select:not([disabled]), textarea:not([disabled]), button, [herf], [tabindex]:not([tabindex="-1"])')
////                .filter(':visible');

////            const index = $focusable.index(this);
////            if (index > -1) {
////                const $next = $focusable.eq(index + 1).length ?
////                    $focusable.eq(index + 1) : $focusable.eq(0);
////                $next.focus();
////            }
////        }
////    });
////}
//////#endregion

////function getActiveTab() {
////    const activeTab = $('.nav-link.active').attr('id');
////    if (activeTab === 'nav-home-tab') return 'buyer';
////    if (activeTab === 'nav-profile-tab') return 'brand';
////    if (activeTab === 'nav-contact-tab') return 'dladdress';
////    return null;
////}

////function initializeDLEventHandlers() {
////    $(".js-buyer-group-save").off('click.dladdress').on('click.dladdress', function () {
////        if (getActiveTab() === 'dladdress') {
////            handleDLAddressFormSubmission();
////        }
////    });

////    $("#js-buyer-group-delete-confirm").off('click.dladdress').on('click.dladdress', function () {
////        if (getActiveTab() === 'dladdress') {
////            handleDLAddressBulkDelete();
////        }
////    });

////    $("#js-buyer-group-clear").off('click.dladdress').on('click.dladdress', function () {
////        if (getActiveTab() === 'dladdress') {
////            clearDLAddressForm();
////        }
////    });


////    $('#DLAddress_BuyerId').on('change', function () {
////        let id = $('#DLAddress_BuyerId').val();
////        console.log(id);
////        displayDLAddressData(null, id);
////    });

////    $('#DLAddress_Search').on('click', function () {
////        let id = $('#DLAddress_DeliveryAddressId').val();
////        displayDLAddressData(id);
////    });

////    $(document).on("click", ".dl-address-id-link", function () {
////        const id = $(this).data("id");
////        console.log(id);
////        if (!id) return;

////        populateDLAddressForm(id);
////    });

////    setTimeout(function () {
////        $('#dlAddress-grid').DataTable().columns.adjust().draw();
////    }, 100);

////    $('#tdsLessInvestment-check-all').on('change', function () {
////        const isChecked = $(this).is(':checked');
////        $('#dlAddress-grid-body input[type="checkbox"]').prop('checked', isChecked);
////        updateSelectedDLAddressIds();
////    });
////}

////function populateDLAddressForm(id) {
////    $.ajax({
////        url: DlUrl.Details,
////        data: { id },
////        type: "GET",
////        success: function (res) {
////            console.log(res);
////            if (!res || !res.data) {
////                showNotification("Data not found", "error");
////                return;
////            }
////            let data = res.data;
////            console.log(data);
////            try {
////                $('[name="DLAddress.Tc"]').val(data.tc);

////                $('[name="DLAddress.BuyerId"]').val(data.buyerId).trigger('change');
////                $('[name="DLAddress.BuyerId"]').prop('readonly', true);
////                if (!$('[name="DLAddress.BuyerId_hidden"]').length) {
////                    $('<input>').attr({
////                        type: 'hidden',
////                        name: 'DLAddress.BuyerId',
////                        class: 'buyer-id-hidden'
////                    }).val(data.buyerId).insertAfter('[name="DLAddress.BuyerId"]');
////                }

////                $('[name="DLAddress.DeliveryAddressId"]').val(data.deliveryAddressId);
////                $('[name="DLAddress.DeliveryAddressId"]').prop('readonly', true);

////                $('[name="DLAddress.Name"]').val(data.name);
////                $('[name="DLAddress.DeliveryAddress"]').val(data.deliveryAddress);
////                $('[name="DLAddress.ContactPerson"]').val(data.contactPerson);
////                $('[name="DLAddress.Designation"]').val(data.designation);
////                $('[name="DLAddress.Phone"]').val(data.phone);
////                $('[name="DLAddress.Email"]').val(data.email);

////                $('[name="DLAddress.LDate"]').text(data.ldate);
////                $('[name="DLAddress.ModifyDate"]').text(data.modifyDate);

////                handleDateInfoPartial('DLAddress', true);
////            } catch (e) {
////                console.error("Error populating form:", e);
////                showNotification("Error loading record details", "error");
////            }
////        },
////        error: function (xhr, status, error) {
////            console.error("Error fetching record:", error);
////            showNotification("Failed to load record details", "error")
////        }
////    })
////}

////$(document).on('change', '#dlAddress-grid-body input[type="checkbox"]', function () {
////    const id = $(this).data('id');

////    if ($(this).is(':checked')) {
////        selectedDLAddressId.add(id);
////    } else {
////        selectedDLAddressId.delete(id);
////    }

////    const total = $('#dlAddress-grid-body input[type="checkbox"]').length;
////    const checked = $('#dlAddress-grid-body input[type="checkbox"]:checked').length;
////    $("#tdsLessInvestment-check-all").prop('checked', total > 0 && total === checked);
////});

////function updateSelectedDLAddressIds() {
////    const currentPageCheckboxes = $('#dlAddress-grid-body input[type="checkbox"]');

////    currentPageCheckboxes.each(function () {
////        const id = $(this).data('id');

////        if ($(this).is(':checked')) {
////            selectedDLAddressId.add(id);
////        } else {
////            selectedDLAddressId.delete(id);
////        }
////    });
////}

////function loadDLAddressTableData() {
////    displayDLAddressData();
////}

////function displayDLAddressData(id = null, buyerId = null) {
////    if ($.fn.DataTable.isDataTable("#dlAddress-grid")) {
////        $("#dlAddress-grid").DataTable().clear().destroy();
////    }

////    const tableBody = $("#dlAddress-grid-body");
////    tableBody.empty();
////    const filterData = {};
////    if (id) {
////        filterData.id = id;
////    }

////    if (buyerId) {
////        filterData.buyerId = buyerId;
////    }

////    $('#dlAddress-grid').DataTable({
////        processing: true,
////        serverSide: true,
////        ajax: {
////            url: DlUrl.List,
////            type: 'POST',
////            data: filterData
////        },
////        columns: [
////            {
////                data: null,
////                orderable: false,
////                className: 'text-center',
////                render: function (data, type, row) {
////                    return `<input type="checkbox" width="1%" style="padding:0;" class="py-0" data-id="${row.tc}"/>`;
////                }
////            },
////            {
////                data: 'deliveryAddressId',
////                className: 'text-center',
////                render: function (data, type, row) {
////                    return `<a href="#buyerDLForm" class="py-0 dl-address-id-link" data-id="${row.tc}">${data}</a>`;
////                }
////            },
////            { data: 'name', className: 'py-0 text-left' },
////            { data: 'deliveryAddress', className: 'py-0 text-left' },
////            { data: 'contactPerson', className: 'py-0 text-center' },
////            { data: 'designation', className: 'py-0 text-center' },
////            { data: 'phone', className: 'py-0 text-center' },
////            { data: 'email', className: 'py-0 text-center' }
////        ],
////        autoWidth: false,
////        fixedHeader: false,
////        info: true,
////        lengthChange: true,
////        lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
////        ordering: true,
////        pageLength: 10,
////        paging: true,
////        responsive: true,
////        scrollCollapse: true,
////        scrollX: true,
////        searching: true,
////        columnDefs: [
////            {
////                targets: 0,
////                orderable: false,
////                className: 'no-sort'
////            }
////        ],
////        language: {
////            search: "🔍 Search:",
////            lengthMenu: "Show _MENU_ entries",
////            info: "Showing _START_ to _END_ of _TOTAL_ entries",
////            paginate: {
////                first: "First",
////                previous: "Prev",
////                next: "Next",
////                last: "Last"
////            },
////            emptyTable: "No data available",
////            processing: "Loading data..."
////        },
////        initComplete: function () {
////            const api = this.api();
////            const tableId = 'dlAddress-grid';

////            let debounceTimeout;

////            $(`#${tableId}_wrapper .dataTables_filter input`)
////                .off('input.custom') // Use namespaced events
////                .on('input.custom', function () {
////                    clearTimeout(debounceTimeout);
////                    const searchTerm = this.value;
////                    debounceTimeout = setTimeout(function () {
////                        api.search(searchTerm).page('first').draw('page');
////                    }, 500);
////                });
////        },
////        drawCallback: function () {
////            $('#dlAddress-grid-body input[type="checkbox"]').each(function () {
////                const id = $(this).data('id');
////                $(this).prop('checked', selectedDLAddressId.has(id));
////            });

////            const total = $('#dlAddress-grid-body input[type="checkbox"]').length;
////            const checked = $('#dlAddress-grid-body input[type="checkbox"]:checked').length;
////            $("#dlAddress-check-all").prop('checked', total > 0 && total === checked);
////        }
////    });
////}

////function validateDLAddressForm() {
////    var buyerId = $('#DLAddress_BuyerId').val();


////    var deliveryAddress = $('#DLAddress_DeliveryAddress').val();
////    var email = $('#DLAddress_Email').val();

////    if (!buyerId || $.trim(buyerId) === '') {
////        showNotification("Buyer is required.", "warning");
////        return false;
////    }

////    if (!deliveryAddress || $.trim(deliveryAddress) === '') {
////        showNotification("Delivery Address is required.", "warning");
////        return false;
////    }

////    if (email && $.trim(email) !== '') {
////        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
////        if (!emailRegex.test(email)) {
////            showNotification("Please enter a valid email address.", "warning");
////            return false;
////        }
////    }

////    return true;
////}

////function handleDLAddressFormSubmission() {
////    if (!validateDLAddressForm()) return;

////    showLoading();

////    var formData = new FormData($('#buyerDLForm')[0]);

////    $.ajax({
////        url: DlUrl.Save,
////        type: 'POST',
////        data: formData,
////        processData: false,
////        contentType: false,
////        success: function (response) {
////            if (response.success) {
////                showNotification(response.message, "success");
////                clearDLAddressForm();
////                loadDLAddressTableData();
////            } else {
////                showNotification(response.message, "error");
////            }
////        },
////        error: function () {
////            showNotification('Error occurred while saving delivery address information.', "error");
////        },
////        complete: hideLoading
////    });
////}

////function clearDLAddressForm() {
////    $('#buyerDLForm')[0].reset();
////    $('#DLAddress_Tc').val(0);
////    $('.searchable-select').val('').trigger('change');
////    loadDLAddressTableData();
////    handleDateInfoPartial('DLAddress', false);

////    if ($('#DLAddress_BuyerId').prop('disabled')) {
////        $('#DLAddress_BuyerId').prop('disabled', false);
////        $('.buyer-id-hidden').remove();
////    }
////    if ($('#DLAddress_DeliveryAddressId').prop('readonly')) {
////        $('#DLAddress_DeliveryAddressId').prop('readonly', false);
////    }
////}

////function handleDLAddressBulkDelete() {
////    const selectedIds = Array.from(selectedDLAddressId);

////    if (selectedIds.length === 0) {
////        showNotification("Please select record to delete", "warning");
////        return;
////    }

////    if (!confirm(`Are you sure you want to delete ${selectedIds.length} selected Data(s)?`)) {
////        return;
////    }

////    showLoading();

////    $.ajax({
////        url: DlUrl.Delete,
////        type: "DELETE",
////        contentType: "application/json",
////        data: JSON.stringify(selectedIds),
////        success: function (response) {
////            selectedDLAddressId.clear();
////            showNotification(response.message || "Successfully deleted", "success");
////            loadDLAddressTableData();
////            clearDLAddressForm();
////        },
////        error: function (xhr, status, error) {
////            console.error("Error details:", xhr.responseText);
////            showNotification("Error deleting records", "error");
////        },
////        complete: hideLoading
////    });
////}

////function showNotification(message, type) {
////    if (typeof toastr !== 'undefined') {
////        toastr[type](message, type === 'success' ? 'Success' : type === 'error' ? 'Error' : 'Warning');
////    } else {
////        alert(message);
////    }
////}

////function handleDateInfoPartial(formPrefix, isEditMode) {
////    const dateInfoElement = $(`#${formPrefix}_DateInfo`);
////    const ldate = $(`#${formPrefix}_LDate`);
////    const modifyDate = $(`#${formPrefix}_ModifyDate`);

////    if (isEditMode) {
////        dateInfoElement.removeClass('d-none');
////    } else {
////        dateInfoElement.addClass('d-none');
////        ldate.val('');
////        modifyDate.val('');
////    }
////}
