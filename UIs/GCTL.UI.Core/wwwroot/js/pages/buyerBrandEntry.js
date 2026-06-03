(function ($) {
    const CONFIG = {
        namespace: 'brandModule',
        baseUrl: '/BuyerInfo',

        formSelector: '#buyerBrandForm',
        fieldPrefix: 'Brand',
        gridSelector: '#buyer-brand-grid',
        gridBodySelector: '#buyer-brand-grid-body',

        saveSelector: '.js-buyer-group-save',
        clearSelector: '#js-buyer-group-clear',
        deleteSelector: '#js-buyer-group-delete-confirm',
        selectAllSelector: '#buyer-brand-check-all',
        select2selector: '.brandSelect',

        idLinkClass: 'buyer-brand-id-link',
        lastCodeSelector: '#Brand_lastCode',
        dateCreateClass: '#Brand_LDate',
        dateModifyClass: '#Brand_ModifyDate',
        dateInfoClass: '#Brand_DateInfo',

        fields: {
            tc: 'Tc',
            id: 'BrandId',
            name: 'Name',
            buyerId: 'BuyerId',
            additional: [
                'Detail'
            ]
        },

        apiFieldMap: {
            tc: 'tc',
            id: 'brandId',
            name: 'name',
            buyerid: 'buyerId',
            detail: 'detail',
            createDate: 'ldate',
            modifyDate: 'modifyDate'
        },

        photo: {
            enabled: true,
            inputSelector: '#Brand_logoPhoto',
            previewSelector: '#buyerBrandPhotoPreview',
            placeholderSelector: '#photoBrandPlaceholder',
            deleteButtonSelector: '#btnDeleteBrandPhoto',
            photoField: 'logoMonogram',
            photoTypeField: 'logoType'
        },

        tableColumns: [
            {
                data: null,
                orderable: false,
                className: 'text-center no-sort',
                width: '7%',
                render: function (data, type, row) {
                    return `<input type="checkbox" class="py-0 no-sort" data-id="${row.tc}"/>`;
                }
            },
            {
                data: 'brandId',
                className: 'text-center',
                width: '10%',
                render: function (data, type, row) {
                    return `<a href="#buyer-brand-form" class="py-0 buyer-brand-id-link" data-id="${row.tc}">${data}</a>`;
                }
            },
            {
                data: 'buyerName',
                width: '20%',
                className: 'py-0 text-center'
            },
            {
                data: 'name',
                className: 'py-0 text-left',
                width: '25%',
            },
            {
                data: 'logoMonogram',
                className: 'py-0 text-center',
                width: '15%',
                orderable: false,
                render: function (data, type, row) {
                    if (!data?.trim()) return '';
                    return `<div style="width: 100%; height: 100%; display: flex; align-items: center; justify-content: center;">
                        <img src="data:image/jpeg;base64,${data}" 
                             alt="Brand Logo" 
                             style="width: 50px; height: 50px; object-fit: contain; border-radius: 4px; border: 1px solid #ddd;"
                             onerror="this.style.display='none';" />
                    </div>`;
                }
            },
            {
                data: 'detail',
                className: 'py-0 text-center',
                width: '23%'
            }
        ],

        tableOptions: {
            processing: true,
            serverSide: true,
            autoWidth: false,
            fixedHeader: true,
            info: true,
            lengthChange: true,
            lengthMenu: [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]],
            order: [[1, 'asc']],
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
            photo: true
        }
    };

    const URLS = {
        save: CONFIG.baseUrl + '/SaveBuyerBrand',
        list: CONFIG.baseUrl + '/GetBuyerBrandList',
        details: CONFIG.baseUrl + '/GetBuyerBrandById',
        delete: CONFIG.baseUrl + '/BulkBuyerBrandDelete',
        deletePhoto: CONFIG.baseUrl + '/DeleteBrandImage'
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

        if (CONFIG.features.photo) {
            initPhotoHandlers();
        }
    }

    function bindEvents() {
        const ns = `.${CONFIG.namespace}`;

        $("body").off(ns);
        $(CONFIG.gridSelector).off(ns);

        $("body").on(`click${ns}`, CONFIG.saveSelector, function () {
            if (getActiveTab() === 'brand') handleFormSubmit();
        });

        $("body").on(`click${ns}`, CONFIG.clearSelector, function () {
            console.log(getActiveTab());
            if (getActiveTab() === 'brand') {
                clearForm();
                clearAllSelections();
            }
        });

        $("body").on(`click${ns}`, CONFIG.deleteSelector, function () {
            if (getActiveTab() === 'brand') handleBulkDelete();
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
        const debouncedBrandBuyerSearch = debounce((id) => { loadTableData(null, id) }, 300);
        const debouncedBrandSearch = debounce((id) => { loadTableData(id) }, 300);

        $(`#${CONFIG.fieldPrefix}_${CONFIG.fields.buyerId}`).on('change', function () {
            const id = $(this).val();
            if (id) debouncedBrandBuyerSearch(id);
            else loadTableData();
        });

        $('#Buyer_BrandSearch').on('click', () => {
            const id = $(`#${CONFIG.fieldPrefix}_${CONFIG.fields.id}`).val();
            if (id) debouncedBrandSearch(id);
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

                    $(CONFIG.formSelector).find('.searchableSelect, .searchable-select, .brandSelect').trigger('change');

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

                    if (CONFIG.photo.enabled) {
                        handlePhotoDisplay(data);
                    }

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

    function initPhotoHandlers() {
        if (!CONFIG.photo.enabled) return;

        const photoConfig = CONFIG.photo;

        $(photoConfig.inputSelector).on('change', function () {
            const file = this.files[0];
            if (!file) {
                $(photoConfig.previewSelector).hide();
                $(photoConfig.placeholderSelector).show();
                $(photoConfig.deleteButtonSelector).hide();
                return;
            }

            if (!file.type.startsWith('image/')) {
                showNotification('Please select an image file.', "warning");
                this.value = '';
                return;
            }

            if (file.size > 5 * 1024 * 1024) {
                showNotification('File size must be less than 5MB.', "warning");
                this.value = '';
                return;
            }

            const reader = new FileReader();
            reader.onload = (e) => {
                $(photoConfig.previewSelector).attr('src', e.target.result).show();
                $(photoConfig.placeholderSelector).hide();
                $(photoConfig.deleteButtonSelector).show();
                $(photoConfig.deleteButtonSelector).data('from-db', false);
            };
            reader.readAsDataURL(file);
        });

        $(photoConfig.deleteButtonSelector).on('click', function () {
            const isFromDB = $(this).data('from-db');

            if (isFromDB) {
                showConfirmation('Are you sure you want to delete this photo?',
                    'Confirmation',
                    () => {
                        showLoading();

                        $.ajax({
                            url: URLS.deletePhoto,
                            type: 'POST',
                            data: { id: getFieldValue(CONFIG.fields.tc) },
                            success: (res) => {
                                if (res.success) {
                                    showNotification(res.message, 'success');
                                    clearPhoto();
                                    loadTableData();
                                } else {
                                    showNotification(res.message, 'error');
                                }
                            },
                            error: () => {
                                showNotification('Error occurred while deleting photo', 'error');
                            },
                            complete: hideLoading
                        });
                    }
                );
            } else {
                clearPhoto();
            }
        });
    }

    function handlePhotoDisplay(data) {
        const photoConfig = CONFIG.photo;
        $(photoConfig.inputSelector).val('');

        if (data[photoConfig.photoField]) {
            $(photoConfig.previewSelector)
                .attr('src', `data:image/jpeg;base64,${data[photoConfig.photoField]}`)
                .show();
            $(photoConfig.placeholderSelector).hide();
            $(photoConfig.deleteButtonSelector).show().data('from-db', true);
        } else {
            $(photoConfig.previewSelector).attr('src', '').hide();
            $(photoConfig.placeholderSelector).show();
            $(photoConfig.deleteButtonSelector).hide().removeData('from-db');
        }
    }

    function clearPhoto() {
        if (!CONFIG.photo.enabled) return;

        const photoConfig = CONFIG.photo;
        $(photoConfig.previewSelector).attr('src', '').hide();
        $(photoConfig.placeholderSelector).show();
        $(photoConfig.deleteButtonSelector).hide().removeData('from-db');
        $(photoConfig.inputSelector).val('');
    }

    function validateForm() {
        const nameValue = getFieldValue(CONFIG.fields.name);
        const buyerId = getFieldValue(CONFIG.fields.buyerId);

        if (!nameValue || $.trim(nameValue) === '') {
            showNotification(`Brand Name is required.`, "warning");
            $(`#${CONFIG.fieldPrefix}_${CONFIG.fields.name}`).focus();
            return false;
        }

        if (!buyerId || $.trim(buyerId) === '') {
            showNotification("Buyer is required.", "warning");
            return false;
        }

        return true;
    }

    function handleFormSubmit() {
        if (!validateForm()) return;

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

        $(CONFIG.formSelector).find('.searchableSelect, .searchable-select, .brandSelect').each(function () {
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

        if (CONFIG.photo.enabled) {
            clearPhoto();
        }

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
                loadTableData();
                clearForm();
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

    // Expose public API for external access if needed
    //window.BrandModule = {
    //    loadTableData: loadTableData,
    //    clearBrandForm: clearForm
    //};

}(jQuery));