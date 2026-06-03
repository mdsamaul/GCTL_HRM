(function ($) {
    const CONFIG = {
        namespace: 'buyerModule',
        baseUrl: '/BuyerInfo',

        formSelector: '#buyerForm',
        fieldPrefix: 'Setup',
        gridSelector: '#buyer-grid',
        gridBodySelector: '#buyer-grid-body',

        saveSelector: '.js-buyer-group-save',
        clearSelector: '#js-buyer-group-clear',
        deleteSelector: '#js-buyer-group-delete-confirm',
        selectAllSelector: '#buyer-check-all',
        select2selector: '.buyerSelect',

        idLinkClass: 'buyer-id-link',
        lastCodeSelector: '#Setup_lastCode',
        dateCreateClass: '#Setup_LDate',
        dateModifyClass: '#Setup_ModifyDate',
        dateInfoClass: '#Setup_DateInfo',

        fields: {
            tc: 'Tc',
            id: 'BuyerId',
            name: 'BuyerName',
            additional: [
                'CompanyId',
                'Address',
                'LocalOfficeAddress',
                'BuyerDepartmentId',
                'CountryId',
                'Phone',
                'Fax',
                'Email',
                'Url',
                'BuyerTypeId',
                'SalesPersonId',
                'Remarks',
                'Active'
            ]
        },

        apiFieldMap: {
            tc: 'tc',
            id: 'buyerId',
            name: 'buyerName',
            companyid: 'companyId',
            address: 'address',
            localofficeaddress: 'localOfficeAddress',
            buyerdepartmentid: 'buyerDepartmentId',
            countryid: 'countryId',
            phone: 'phone',
            fax: 'fax',
            email: 'email',
            url: 'url',
            buyertypeid: 'buyerTypeId',
            salespersonid: 'salesPersonId',
            remarks: 'remarks',
            active: 'active',
            createDate: 'ldate',
            modifyDate: 'modifyDate'
        },

        photo: {
            enabled: true,
            inputSelector: '#Setup_BuyerPhoto',
            previewSelector: '#buyerPhotoPreview',
            placeholderSelector: '#photoPlaceholder',
            deleteButtonSelector: '#btnDeleteBuyerPhoto',
            photoField: 'photo',
            photoTypeField: 'photoType'
        },

        contactPerson: {
            hiddenInputName: 'Setup.ContatPerson1',
            apiField: 'contatPerson1'
        },

        tableColumns: [
            {
                data: null,
                orderable: false,
                className: 'text-center no-sort',
                width: '2%',
                render: function (data, type, row) {
                    return `<input type="checkbox" class="py-0 no-sort" data-id="${row.tc}"/>`;
                }
            },
            {
                data: 'buyerId',
                className: 'text-center',
                width: '8%',
                render: function (data, type, row) {
                    return `<a href="#buyer-form" class="py-0 buyer-id-link" data-id="${row.tc}">${data}</a>`;
                }
            },
            {
                data: 'buyerName',
                width: '15%',
                className: 'py-0 text-left text-nowrap'
            },
            {
                data: 'address',
                className: 'py-0 text-left',
                width: '20%',
            },
            {
                data: 'countryName',
                className: 'py-0 text-left',
                width: '10%'
            },
            {
                data: 'phone',
                className: 'py-0 text-center',
                width: '10%'
            },
            {
                data: 'email',
                className: 'py-0 text-center',
                width: '10%'
            },
            {
                data: 'contactPersonName',
                className: 'py-0 text-center',
                width: '25%',
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
            quickAdd: true,
            contactPerson: true,
            photo: true
        }
    };

    const URLS = {
        newId: CONFIG.baseUrl + '/GenerateBuyerId',
        save: CONFIG.baseUrl + '/SaveBuyer',
        list: CONFIG.baseUrl + '/GetBuyerList',
        details: CONFIG.baseUrl + '/GetBuyerById',
        delete: CONFIG.baseUrl + '/BulkBuyerDelete',
        deletePhoto: CONFIG.baseUrl + '/DeleteBuyerPhoto',
        cp: CONFIG.baseUrl + '/GetContactPersons',
        buyerDD: CONFIG.baseUrl + '/GetBuyerForDD'
    };

    const state = {
        selectedIds: new Set(),
        isEditMode: false,
        currentTable: null
    };

    // ========================================================================
    // CONTACT PERSON MODULE
    // ========================================================================

    const ContactPersonModule = (() => {
        let selectedCPs = [];
        let allCPs = [];

        const init = () => {
            initDropdown();
            initSearch();
            loadContactPersons();
        };

        const initDropdown = () => {
            $('#contactPersonButton').on('click', function (e) {
                e.stopPropagation();
                const $menu = $('#contactPersonMenu');
                if ($menu.is(':visible')) {
                    $menu.hide();
                } else {
                    positionDropdown();
                    $menu.show();
                }
            });

            $(document).on('click', (e) => {
                if (!$(e.target).closest('#contactPersonDropdown').length) {
                    $('#contactPersonMenu').hide();
                }
            });

            $('#clearSelection').on('click', (e) => {
                e.stopPropagation();
                clearSelection();
            });

            $('#selectAllCP').on('change', function (e) {
                e.stopPropagation();
                const isChecked = $(this).prop('checked');
                $('#contactPersonTableBody tr:visible .contact-checkbox').prop('checked', isChecked);
                updateSelectedCPs();
            });
        };

        const initSearch = () => {
            const $searchInput = $('#cpSearch');
            if (!$searchInput.length) return;

            $searchInput.off('input').on('input', debounce(function () {
                const searchTerm = $('#cpSearch').val().toLowerCase().trim();
                filterTable(searchTerm);
            }, 200));
        };

        const filterTable = (searchTerm) => {
            if (!searchTerm) {
                $('#contactPersonTableBody tr').show();
                updateSelectAllCheckbox();
                return;
            }

            $('#contactPersonTableBody tr').each(function () {
                const text = $(this).text().toLowerCase();
                $(this).toggle(text.includes(searchTerm));
            });
            updateSelectAllCheckbox();
        };

        const loadContactPersons = () => {
            $.ajax({
                url: URLS.cp,
                type: 'GET',
                success: (response) => {
                    if (response.success && response.data) {
                        allCPs = response.data;
                        buildTable();
                    }
                },
                error: (xhr, status, error) => console.error('Error loading contact persons:', error)
            });
        };

        const buildTable = () => {
            const $tbody = $('#contactPersonTableBody');

            if (!allCPs.length) {
                $tbody.html('<tr><td colspan="5" class="text-center text-muted p-3">No contact persons found</td></tr>');
                return;
            }

            const rows = allCPs.map(cp => {
                const isSelected = selectedCPs.some(selected => selected.cpid === cp.cpid);
                return `
                <tr>
                    <td class="text-center align-middle p-0">
                        <input type="checkbox" class="p-0 contact-checkbox"
                               value="${cp.cpid}" data-name="${cp.contactPersonName}"
                               ${isSelected ? 'checked' : ''}/>
                    </td>
                    <td class="text-wrap">${cp.contactPersonName}</td>
                    <td class="text-wrap text-nowrap">${cp.designation || ''}</td>
                    <td class="text-wrap text-nowrap">${cp.phone || ''}</td>
                    <td class="text-wrap">${cp.email || ''}</td>
                </tr>`;
            }).join('');

            $tbody.html(rows);

            $tbody.off('change', '.contact-checkbox').on('change', '.contact-checkbox', function () {
                updateSelectedCPs();
                updateSelectAllCheckbox();
            });

            initSearch();
            updateSelectAllCheckbox();
        };

        const updateSelectedCPs = () => {
            selectedCPs = $('.contact-checkbox:checked').map(function () {
                return { cpid: $(this).val(), name: $(this).data('name') };
            }).get();

            updateDisplay();
            updateHiddenInput();
        };

        const updateSelectAllCheckbox = () => {
            const $visibleCheckboxes = $('#contactPersonTableBody tr:visible .contact-checkbox');
            const $selectAll = $('#selectAllCP');

            if ($visibleCheckboxes.length === 0) {
                $selectAll.prop('checked', false).prop('indeterminate', false);
                return;
            }

            const checkedCount = $visibleCheckboxes.filter(':checked').length;

            if (checkedCount === 0) {
                $selectAll.prop('checked', false).prop('indeterminate', false);
            } else if (checkedCount === $visibleCheckboxes.length) {
                $selectAll.prop('checked', true).prop('indeterminate', false);
            } else {
                $selectAll.prop('checked', false).prop('indeterminate', true);
            }
        };

        const updateDisplay = () => {
            const $button = $('#contactPersonButton .selected-text');
            const $clearBtn = $('#clearSelection');
            const count = selectedCPs.length;

            if (count === 0) {
                $button.text('--Select Contact Person--').addClass('text-muted');
                $clearBtn.hide();
            } else if (count === 1) {
                $button.text(selectedCPs[0].name).removeClass('text-muted');
                $clearBtn.show();
            } else {
                $button.html(`<span class="selected-count">${count} contacts selected</span>`).removeClass('text-muted');
                $clearBtn.show();
            }
        };

        const updateHiddenInput = () => {
            const cpids = selectedCPs.map(cp => cp.cpid).join(',');
            $(`[name="${CONFIG.contactPerson.hiddenInputName}"]`).val(cpids);
        };

        const clearSelection = () => {
            $('.contact-checkbox').prop('checked', false);
            selectedCPs = [];
            updateDisplay();
            updateHiddenInput();
            updateSelectAllCheckbox();
        };

        const setSelected = (cpids) => {
            if (!cpids) {
                clearSelection();
                return;
            }

            const cpidArray = cpids.split(',').map(id => id.trim());

            selectedCPs = cpidArray
                .map(cpid => allCPs.find(contact => contact.cpid === cpid))
                .filter(cp => cp)
                .map(cp => ({ cpid: cp.cpid, name: cp.contactPersonName }));

            $('.contact-checkbox').each(function () {
                $(this).prop('checked', cpidArray.includes($(this).val()));
            });

            updateDisplay();
            updateHiddenInput();
            updateSelectAllCheckbox();
        };

        const positionDropdown = () => {
            const $btn = $('#contactPersonButton');
            const $menu = $('#contactPersonMenu');
            const top = $btn.offset().top - $(window).scrollTop();
            const below = $(window).height() - top - $btn.outerHeight();
            const showAbove = below < 300 && top > 300;

            $menu.toggleClass('show-above', showAbove).toggleClass('show-below', !showAbove);
        };

        window.loadCP = loadContactPersons;

        return {
            init,
            loadContactPersons,
            setSelected,
            clearSelection
        };
    })();

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

        if (CONFIG.features.contactPerson) {
            ContactPersonModule.init();
        }

        if (CONFIG.features.photo) {
            initPhotoHandlers();
        }

        document.querySelectorAll('button[data-bs-toggle="tab"]').forEach(button => {
            button.addEventListener('shown.bs.tab', function (e) {
                const targetId = e.target.getAttribute('data-bs-target');
                const targetPane = document.querySelector(targetId);

                const tabText = e.target.textContent.trim(); // or e.target.innerText
                const pageTitle = document.querySelector('#buyer-title');

                const titleMap = {
                    'Buyer Info': 'Buyer Info :',
                    'Brand Info': 'Buyer Brand Info :',
                    'Delivery Address': 'Buyer Delivery Address :'
                };

                if (pageTitle) {
                    pageTitle.textContent = titleMap[tabText] || tabText;
                }

                if (targetPane) {
                    $(targetPane).find('table.dataTable').each(function () {
                        if ($.fn.DataTable.isDataTable(this)) {
                            $(this).DataTable().columns.adjust();
                        }
                    });
                }
            });
        });

        $(CONFIG.gridSelector).DataTable().columns.adjust();
    }

    function bindEvents() {
        const ns = `.${CONFIG.namespace}`;

        $("body").off(ns);
        $(CONFIG.gridSelector).off(ns);

        $("body").on(`click${ns}`, CONFIG.saveSelector, function () {
            if (getActiveTab() === 'buyer')
                handleFormSubmit();
        });

        $("body").on(`click${ns}`, CONFIG.clearSelector, function () {
            console.log(getActiveTab());
            if (getActiveTab() === 'buyer') {
                clearForm();
                clearAllSelections();
            }
        });

        $("body").on(`click${ns}`, CONFIG.deleteSelector, function () {
            
            if (getActiveTab() === 'buyer')
                handleBulkDelete();
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
        const debouncedSearch = debounce((id) => { loadTableData(id) }, 300);
        $('#Setup_BuyerSearch').on('click', () => {
            const id = $('#Setup_BuyerId').val();
            if (id) debouncedSearch(id);
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

                    CONFIG.fields.additional.forEach(fieldName => {
                        const apiKey = map[fieldName.toLowerCase()] || fieldName.toLowerCase();
                        if (data[apiKey] !== undefined) {
                            setFieldValue(fieldName, data[apiKey]);
                        }
                    });

                    $(CONFIG.formSelector).find('.searchableSelect, .searchable-select, .buyerSelect').trigger('change');

                    const idSelector = getFieldSelector(CONFIG.fields.id);
                    $(idSelector).prop('readonly', true);

                    if (CONFIG.features.contactPerson && data[CONFIG.contactPerson.apiField]) {
                        if (typeof ContactPersonModule !== 'undefined') {
                            ContactPersonModule.setSelected(data[CONFIG.contactPerson.apiField]);
                        }
                    }

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

        if (data[photoConfig.photoField] && data[photoConfig.photoTypeField]) {
            $(photoConfig.previewSelector)
                .attr('src', `data:${data[photoConfig.photoTypeField]};base64,${data[photoConfig.photoField]}`)
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

        if (!nameValue || $.trim(nameValue) === '') {
            showNotification(`${CONFIG.fields.name} is required.`, "warning");
            $(`#${CONFIG.fieldPrefix}_${CONFIG.fields.name}`).focus();
            return false;
        }

        const buyerTypeId = getFieldValue('BuyerTypeId');
        if (!buyerTypeId || $.trim(buyerTypeId) === '') {
            showNotification("Buyer Type is required.", "warning");
            return false;
        }

        const buyerEmail = getFieldValue('Email');
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (buyerEmail && $.trim(buyerEmail) !== '' && !emailRegex.test(buyerEmail)) {
            showNotification("Please enter a valid email address.", "warning");
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

                    if (response.data[CONFIG.apiFieldMap.id]) {
                        $(CONFIG.lastCodeSelector).val(response.data[CONFIG.apiFieldMap.id]);
                    }

                    loadTableData();

                    if (typeof window.loadCP === 'function') {
                        window.loadCP();
                    }
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
        $(CONFIG.formSelector).find('.searchableSelect, .searchable-select, .buyerSelect').each(function () {
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

        if (CONFIG.features.contactPerson && typeof ContactPersonModule !== 'undefined') {
            ContactPersonModule.clearSelection();
        }

        if (CONFIG.photo.enabled) {
            clearPhoto();
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

    function loadTableData(id = null) {
        displayTableData(id);
    }

    function displayTableData(id = null) {
        if ($.fn.DataTable.isDataTable(CONFIG.gridSelector)) {
            $(CONFIG.gridSelector).DataTable().clear().destroy();
        }

        $(CONFIG.gridBodySelector).empty();

        const filterData = id ? { id } : {};

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

                // Refresh other modules if they exist
                if (typeof BrandModule !== 'undefined' && typeof BrandModule.loadTableData === 'function') {
                    BrandModule.loadTableData();
                    BrandModule.clearBrandForm();
                }

                if (typeof DLAddressModule !== 'undefined' && typeof DLAddressModule.loadTableData === 'function') {
                    DLAddressModule.loadTableData();
                    DLAddressModule.clearDLForm();
                }
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

}(jQuery));









//// buyer.js - Buyer specific functionality

//const BuyerModule = (() => {
//    const URLs = {
//        Save: '/BuyerInfo/SaveBuyer',
//        List: '/BuyerInfo/GetBuyerList',
//        Details: '/BuyerInfo/GetBuyerById',
//        Delete: '/BuyerInfo/BulkBuyerDelete',
//        DeleteBuyerPhoto: '/BuyerInfo/DeleteBuyerPhoto',
//        ContactPerson: '/BuyerInfo/GetContactPersons',
//        BuyerDD :'/BuyerInfo/GetBuyerForDD'
//    };

//    let checkboxManager;
//    let buyerTable = null;

//    // Initialize module
//    const init = () => {
//        checkboxManager = new CheckboxManager('buyer-grid', 'buyer-check-all');
//        initEventHandlers();
//        loadTableData();
//        initImagePreview();
//        initContactPersonDropdown();
//        loadContactPersons();
//    };

//    // Event handlers
//    const initEventHandlers = () => {
//        $(".js-buyer-group-save").off('click.buyer').on('click.buyer', () => {
//            if (getActiveTab() === 'buyer') handleFormSubmit();
//        });

//        $("#js-buyer-group-delete-confirm").off('click.buyer').on('click.buyer', () => {
//            if (getActiveTab() === 'buyer') handleBulkDelete();
//        });

//        $("#js-buyer-group-clear").off('click.buyer').on('click.buyer', () => {
//            if (getActiveTab() === 'buyer') clearBuyerForm();
//        });

//        const debouncedBuyerSearch = debounce((id) => { loadTableData(id) }, 300);

//        $('#Setup_BuyerSearch').on('click', () => {
//            const id = $('#Setup_BuyerId').val();
//            if (id) debouncedBuyerSearch(id);
//            else loadTableData();
//        });

//        $(document).on("click", ".buyer-id-link", function () {
//            const id = $(this).data("id");
//            if (id) populateForm(id);
//        });
//    };

//    // Image preview setup
//    const initImagePreview = () => {
//        setupImagePreview('Setup_BuyerPhoto', 'buyerPhotoPreview', 'photoPlaceholder', 'btnDeleteBuyerPhoto', { url: URLs.DeleteBuyerPhoto, getData: () => ({id:$('#Setup_Tc').val() })});
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
//                const fields = {
//                    'Setup.Tc': data.tc,
//                    'Setup.BuyerId': data.buyerId,
//                    'Setup.BuyerName': data.buyerName,
//                    'Setup.CompanyId': data.companyId,
//                    'Setup.Address': data.address,
//                    'Setup.LocalOfficeAddress': data.localOfficeAddress,
//                    'Setup.BuyerDepartmentId': data.buyerDepartmentId,
//                    'Setup.CountryId': data.countryId,
//                    'Setup.Phone': data.phone,
//                    'Setup.Fax': data.fax,
//                    'Setup.Email': data.email,
//                    'Setup.Url': data.url,
//                    'Setup.BuyerTypeId': data.buyerTypeId,
//                    'Setup.SalesPersonId': data.salesPersonId,
//                    'Setup.Remarks': data.remarks,
//                    'Setup.Active': data.active
//                };

//                Object.entries(fields).forEach(([name, value]) => {
//                    const $field = $(`[name="${name}"]`);
//                    $field.val(value);
//                    if ($field.hasClass('searchableSelect') || $field.hasClass('searchable-select')) {
//                        $field.trigger('change');
//                    }
//                });

//                $('[name="Setup.BuyerId"]').trigger('change');
//                $('[name="Setup.BuyerId"]').prop('readonly', true);
//                setSelectedContactPerson(data.contatPerson1);

//                $('#Setup_LDate').text(formatedDateddMMyyyy(data.ldate));
//                $('#Setup_ModifyDate').text(formatedDateddMMyyyy(data.modifyDate));
//                handleDateInfoPartial('Setup', true);

//                // Handle photo
//                $('#Setup_BuyerPhoto').val('');
//                if (data.photo && data.photoType) {
//                    $('#buyerPhotoPreview').attr('src', `data:${data.photoType};base64,${data.photo}`).show();
//                    $('#photoPlaceholder').hide();
//                    $('#btnDeleteBuyerPhoto').show().data('from-db', true); 
//                } else {
//                    $('#buyerPhotoPreview').attr('src', '').hide();
//                    $('#photoPlaceholder').show();
//                    $('#btnDeleteBuyerPhoto').hide().removeData('from-db');
//                }
//            },
//            error: (xhr, status, error) => {
//                console.error("Error fetching record:", error);
//                showNotification("Failed to load record details", "error");
//            }
//        });
//    };
    
//    // Load table data
//    const loadTableData = (id = null) => {
//        const filterData = id ? { id } : {};

//        AjaxRequestManager.abort('buyer-list');

//        if (buyerTable !== null) {
//            buyerTable.settings()[0].ajax.data = function (d) {
//                return { ...d, ...filterData };
//            }
//            buyerTable.ajax.reload(null, false);
//            return;
//        }

//        buyerTable = initDataTable('buyer-grid', {
//            ajax: {
//                url: URLs.List,
//                type: 'POST',
//                data: function (d) {
//                    return { ...d, ...filterData };
//                },
//                beforeSend: function (xhr) {
//                    AjaxRequestManager.register('buyer-list', xhr);
//                },
//                error: (xhr, error, thrown) => {
//                    if (error !== 'abort') {
//                        console.error("Error fetching buyer list:", error);
//                        showNotification("Failed to load buyer list", "error");
//                    }
//                },
//                complete: function () {
//                    AjaxRequestManager.remove('buyer-list');
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
//                    data: 'buyerId',
//                    className: 'text-center',
//                    render: (data, type, row) => `<a href="#buyer-form" class="py-0 buyer-id-link" data-id="${row.tc}">${data}</a>`
//                },
//                { data: 'buyerName', className: 'py-0 text-center text-nowrap' },
//                { data: 'address', className: 'py-0 text-left' },
//                { data: 'countryName', className: 'py-0 text-left' },
//                { data: 'phone', className: 'py-0 text-center' },
//                { data: 'email', className: 'py-0 text-center' },
//                {
//                    data: 'contactPersonName',
//                    className: 'py-0 text-center'
//                }
//            ],
//            columnDefs: [{ targets: 0, orderable: false, className: 'no-sort' }],
//            drawCallback: function () { checkboxManager.syncWithTable() }
//        });
//    };

//    // Form validation
//    const validateForm = () => {
//        const buyerName = $('#Setup_BuyerName').val()?.trim();
//        const buyerTypeId = $('#Setup_BuyerTypeId').val()?.trim();
//        const buyerEmail = $('#Setup_Email').val()?.trim();


//        if (!buyerName) {
//            showNotification("Buyer Name is required.", "warning");
//            return false;
//        }

//        if (!buyerTypeId) {
//            showNotification("Buyer Type is required.", "warning");
//            return false;
//        }

//        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
//        if (buyerEmail && !emailRegex.test(buyerEmail)) {
//            showNotification("Please enter a valid email address.", "warning");
//            return false;
//        }

//        return true;
//    };

//    // Handle form submission
//    const handleFormSubmit = () => {
//        if (!validateForm()) return;

//        LoadingOverlay.show();

//        $.ajax({
//            url: URLs.Save,
//            type: 'POST',
//            data: new FormData($('#buyerForm')[0]),
//            processData: false,
//            contentType: false,
//            success: (response) => {
//                if (response.success) {
//                    showNotification(response.message, "success");
//                    clearBuyerForm();
//                    if (typeof loadCP === 'function') loadCP();
//                    refreshBuyerDropdowns();

//                } else {
//                    showNotification(response.message, "error");
//                }
//            },
//            error: () => showNotification('Error occurred while saving buyer information.', "error"),
//            complete: LoadingOverlay.hide
//        });
//    };

//    // Clear form
//    const clearBuyerForm = () => {
//        $('#buyerForm')[0].reset();

//        $('.buyerSelect').each(function () {
//            if ($(this).data('select2')) {
//                $(this).val('').trigger('change');
//            } else {
//                $(this).val('');
//            }
//        });

//        clearContactPersonSelection();
//        $('#buyerPhotoPreview').attr('src', '').hide();
//        $('#photoPlaceholder').show();
//        $('#btnDeleteBuyerPhoto').hide();
//        $('#Setup_BuyerPhoto').val('');
//        loadTableData();
//        handleDateInfoPartial('Setup', false);
//        $('#Setup_BuyerId').prop('readonly', false);
//        checkboxManager.clear();
//        $('#Setup_BuyerTypeId, #Setup_Active, #Setup_SalesPersonId').each(function () {
//            $(this).find('option:first').prop('selected', true);
//            $(this).trigger('change');
//        });
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
//                        loadTableData();
//                        clearBuyerForm();

//                        refreshBuyerDropdowns();

//                        if (typeof BrandModule !== 'undefined') {

//                            BrandModule.loadTableData();
//                            BrandModule.clearBrandForm();
//                            DLAddressModule.loadTableData();
//                            DLAddressModule.clearDLForm();

//                        }

//                        if (typeof BrandModule !== 'undefined' && typeof BrandModule.clearBrandForm === 'function') {
//                            BrandModule.clearBrandForm();
//                        }

//                        // Clear DL Address form and reload grid
//                        if (typeof DLAddressModule !== 'undefined' && typeof DLAddressModule.clearDLForm === 'function') {
//                            DLAddressModule.clearDLForm();
//                        }
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

//    const refreshBuyerDropdowns = debounce(() => {
//        $.ajax({
//            url: URLs.BuyerDD,
//            type: 'GET',
//            beforeSend: function (jqXHR) {
//                AjaxRequestManager.register('refresh-buyer-dd', jqXHR);
//            },
//            success: (response) => {
//                if (response.success && response.data) {
//                    // Update Brand module buyer dropdown
//                    const $brandBuyerDropdown = $('#Brand_BuyerId');
//                    if ($brandBuyerDropdown.length) {
//                        $brandBuyerDropdown.empty().append(
//                            $('<option>', { value: '', text: '--Select Buyer--' })
//                        );
//                        response.data.forEach(buyer => {
//                            $brandBuyerDropdown.append(
//                                $('<option>', { value: buyer.buyerId, text: buyer.buyerName })
//                            );
//                        });
//                        $brandBuyerDropdown.trigger('change');
//                    }

//                    // Update DL Address module buyer dropdown
//                    const $dlBuyerDropdown = $('#DLAddress_BuyerId');
//                    if ($dlBuyerDropdown.length) {
//                        $dlBuyerDropdown.empty().append(
//                            $('<option>', { value: '', text: '--Select Buyer--' })
//                        );
//                        response.data.forEach(buyer => {
//                            $dlBuyerDropdown.append(
//                                $('<option>', { value: buyer.buyerId, text: buyer.buyerName })
//                            );
//                        });
//                        $dlBuyerDropdown.trigger('change');
//                    }
//                }
//            },
//            error: (xhr, status, error) => {
//                console.error('Error refreshing buyer dropdowns:', error);
//            },
//            complete: function () {
//                AjaxRequestManager.remove('refresh-buyer-dd');
//            }
//        });
//    }, 300);


//    //#region Contact Person Dropdown
//    let selectedCPs = [];
//    let allCPs = [];

//    const initContactPersonDropdown = () => {

//        $('#contactPersonButton').on('click', function (e) {
//            e.stopPropagation();
//            const $menu = $('#contactPersonMenu');
//            if ($menu.is(':visible')) {
//                $menu.hide();
//            } else {
//                positionDropdown();
//                $menu.show();
//            }
//        });

//        $(document).on('click', (e) => {
//            if (!$(e.target).closest('#contactPersonDropdown').length) {
//                $('#contactPersonMenu').hide();
//            }
//        });

//        $('#clearSelection').on('click', (e) => {
//            e.stopPropagation();
//            clearContactPersonSelection();
//        });

//        // Select All checkbox handler
//        $('#selectAllCP').on('change', function (e) {
//            e.stopPropagation();
//            const isChecked = $(this).prop('checked');
//            $('#contactPersonTableBody tr:visible .contact-checkbox').prop('checked', isChecked);
//            updateSelectedCPs();
//        });

//    };

//    const initContactPersonSearch = () => {
//        const $searchInput = $('#cpSearch');
//        if (!$searchInput.length) return;

//        $searchInput.off('input').on('input', debounce(function () {
//            // Capture the input element reference before debounce loses context
//            const searchTerm = $('#cpSearch').val().toLowerCase().trim();
//            filterContactPersonTable(searchTerm);
//        }, 200));
//    };
//    const filterContactPersonTable = (searchTerm) => {
//        if (!searchTerm) {
//            $('#contactPersonTableBody tr').show();
//            updateSelectAllCheckbox();
//            return;
//        }

//        $('#contactPersonTableBody tr').each(function () {
//            const text = $(this).text().toLowerCase();
//            $(this).toggle(text.includes(searchTerm));
//        });
//        updateSelectAllCheckbox();
//    };

//    const loadContactPersons = () => {
//        $.ajax({
//            url: URLs.ContactPerson,
//            type: 'GET',
//            success: (response) => {
//                if (response.success && response.data) {
//                    allCPs = response.data;
//                    buildContactPersonTable();
//                }
//            },
//            error: (xhr, status, error) => console.error('Error loading contact persons:', error)
//        });
//    };

//    const buildContactPersonTable = () => {
//        const $tbody = $('#contactPersonTableBody');

//        if (!allCPs.length) {
//            $tbody.html('<tr><td colspan="5" class="text-center text-muted p-3">No contact persons found</td></tr>');
//            return;
//        }

//        const rows = allCPs.map(cp => {
//            const isSelected = selectedCPs.some(selected => selected.cpid === cp.cpid);
//            return `
//            <tr>
//                <td class="text-center align-middle p-0"><input type="checkbox" class="p-0 contact-checkbox"
//                           value="${cp.cpid}" data-name="${cp.contactPersonName}"
//                           ${isSelected ? 'checked' : ''}/></td>
//                <td class="text-wrap">${cp.contactPersonName}</td>
//                <td class="text-wrap text-nowrap">${cp.designation || ''}</td>
//                <td class="text-wrap text-nowrap">${cp.phone || ''}</td>
//                <td class="text-wrap">${cp.email || ''}</td>
//            </tr>
//        `;
//        }).join('');

//        $tbody.html(rows);
//        $tbody.off('change', '.contact-checkbox').on('change', '.contact-checkbox', function () {
//            updateSelectedCPs();
//            updateSelectAllCheckbox();
//        });
//        initContactPersonSearch();
//        updateSelectAllCheckbox();
//    };

//    const updateSelectedCPs = () => {
//        selectedCPs = $('.contact-checkbox:checked').map(function () {
//            return { cpid: $(this).val(), name: $(this).data('name') };
//        }).get();

//        updateCPDisplay();
//        updateCPHiddenInput();
//    };

//    const updateSelectAllCheckbox = () => {
//        const $visibleCheckboxes = $('#contactPersonTableBody tr:visible .contact-checkbox');
//        const $selectAll = $('#selectAllCP');

//        if ($visibleCheckboxes.length === 0) {
//            $selectAll.prop('checked', false).prop('indeterminate', false);
//            return;
//        }

//        const checkedCount = $visibleCheckboxes.filter(':checked').length;

//        if (checkedCount === 0) {
//            $selectAll.prop('checked', false).prop('indeterminate', false);
//        } else if (checkedCount === $visibleCheckboxes.length) {
//            $selectAll.prop('checked', true).prop('indeterminate', false);
//        } else {
//            $selectAll.prop('checked', false).prop('indeterminate', true);
//        }
//    };

//    const updateCPDisplay = () => {
//        const $button = $('#contactPersonButton .selected-text');
//        const $clearBtn = $('#clearSelection');
//        const count = selectedCPs.length;

//        if (count === 0) {
//            $button.text('--Select Contact Person--');
//            $clearBtn.hide();
//        } else if (count === 1) {
//            $button.text(selectedCPs[0].name);
//            $clearBtn.show();
//        } else {
//            $button.html(`<span class="selected-count">${count} contacts selected</span>`);
//            $clearBtn.show();
//        }
//    };

//    const updateCPHiddenInput = () => {
//        const cpids = selectedCPs.map(cp => cp.cpid).join(',');
//        $('[name="Setup.ContatPerson1"]').val(cpids);
//    };

//    const clearContactPersonSelection = () => {
//        $('.contact-checkbox').prop('checked', false);
//        selectedCPs = [];
//        updateCPDisplay();
//        updateCPHiddenInput();
//        updateSelectAllCheckbox();
//    };

//    const setSelectedContactPerson = (cpids) => {
//        if (!cpids) {
//            clearContactPersonSelection();
//            return;
//        }

//        const cpidArray = cpids.split(',').map(id => id.trim());

//        selectedCPs = cpidArray
//            .map(cpid => allCPs.find(contact => contact.cpid === cpid))
//            .filter(cp => cp)
//            .map(cp => ({ cpid: cp.cpid, name: cp.contactPersonName }));

//        $('.contact-checkbox').each(function () {
//            $(this).prop('checked', cpidArray.includes($(this).val()));
//        });

//        updateCPDisplay();
//        updateCPHiddenInput();
//        updateSelectAllCheckbox();
//    };

//    const positionDropdown = () => {
//        const $btn = $('#contactPersonButton');
//        const $menu = $('#contactPersonMenu');
//        const top = $btn.offset().top - $(window).scrollTop();
//        const below = $(window).height() - top - $btn.outerHeight();
//        const showAbove = below < 300 && top > 300;

//        $menu.toggleClass('show-above', showAbove).toggleClass('show-below', !showAbove);
//    };

//    //#endregion


//    // Public API
//    return {
//        init,
//        loadContactPersons,
//        clearContactPersonSelection,
//        setSelectedContactPerson
//    };
//})();

//// Make loadCP globally accessible for quick add modal
//window.loadCP = BuyerModule.loadContactPersons;
//window.clearCPSelection = BuyerModule.clearContactPersonSelection;
//window.setSelectedCP = BuyerModule.setSelectedContactPerson;




//// Initialize on document ready
//$(document).ready(() => BuyerModule.init());



////let selectedBuyerId = new Set();
////let isBuyerEditMode = false;

////const BuyerUrl = {
////    Save: '/BuyerInfo/SaveBuyer',
////    List: '/BuyerInfo/GetBuyerList',
////    Details: '/BuyerInfo/GetBuyerById',
////    Delete: `/BuyerInfo/BulkBuyerDelete`
////}

////$(document).ready(function () {
////    setupLoadingOverlay();
////    initializeBuyerEventHandlers();
////    loadBuyerTableData();
////    // setupEnterKeyNavigation();
////    initializeSelect();

////});


////// #region default
////function getActiveTab() {
////    const activeTab = $('.nav-link.active').attr('id');
////    if (activeTab === 'nav-home-tab') {
////        return 'buyer';
////    }
////    if (activeTab === 'nav-profile-tab') {
////        return 'brand';
////    }
////    if (activeTab === 'nav-contact-tab') {
////        return 'dladdress';
////    }
////    return null;
////}

////function initializeSelect() {
////    $('.searchableSelect').select2({
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
////    const $form = $('#buyer-form');
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

////function initializeBuyerEventHandlers() {

////    $(".js-buyer-group-save").off('click.buyer').on('click.buyer', function () {
////        if (getActiveTab() === 'buyer') {
////            handleBuyerFormSubmission();
////        }
////    });

////    $('#btnDeleteBuyerPhoto').on('click', clearPhotoDisplay);

////    $("#js-buyer-group-delete-confirm").off('click.buyer').on('click.buyer', function () {
////        if (getActiveTab() === 'buyer') {
////            handleBuyerBulkDelete();
////        }
////    });

////    $("#js-buyer-group-clear").off('click.buyer').on('click.buyer', function () {
////        if (getActiveTab() === 'buyer') {
////            clearBuyerForm();
////        }
////    });

////    $('#Setup_BuyerPhoto').change('change', imageLoad);
////    $('#Setup_BuyerSearch').on('click', function () {
////        let id = $('#Setup_BuyerId').val();
////        displayBuyerData(id);
////    })

////    $(document).on("click", ".buyer-id-link", function () {
////        const id = $(this).data("id");
////        console.log(id);
////        if (!id) return;

////        populateBuyerForm(id);
////    });

////    $('#buyer-grid').DataTable().columns.adjust().draw();

////    $('#buyer-check-all').on('change', function () {
////        const isChecked = $(this).is(':checked');
////        $('#buyer-grid-body input[type="checkbox"]').prop('checked', isChecked);

////        updateSelectedBuyerIds();
////    });
////}

////function populateBuyerForm(id) {
////    $.ajax({
////        url: BuyerUrl.Details,
////        data: { id },
////        type: "GET",
////        success: function (res){
////            console.log(res);
////            if (!res || !res.data) {
////                showNotification("Data not found", "error");
////                return;
////            }
////            let data = res.data;
////            try {
////                $('[name="Setup.Tc"]').val(data.tc);
////                $('[name="Setup.BuyerId"]').val(data.buyerId);
////                $('[name="Setup.BuyerId"]').prop('readonly', true);
////                $('[name="Setup.BuyerName"]').val(data.buyerName);
////                $('[name="Setup.CompanyId"]').val(data.companyId).trigger('change');
////                $('[name="Setup.Address"]').val(data.address);
////                $('[name="Setup.LocalOfficeAddress"]').val(data.localOfficeAddress);
////                $('[name="Setup.BuyerDepartmentId"]').val(data.buyerDepartmentId).trigger('change');
////                $('[name="Setup.CountryId"]').val(data.countryId).trigger('change');
////                $('[name="Setup.Phone"]').val(data.phone);
////                $('[name="Setup.Fax"]').val(data.fax);
////                $('[name="Setup.Email"]').val(data.email);
////                $('[name="Setup.Url"]').val(data.url);
////                //$('[name="Setup.ContatPerson1"]').val(data.contatPerson1).trigger('change');
////                setSelectedCP(data.contatPerson1);
////                $('[name="Setup.BuyerTypeId"]').val(data.buyerTypeId).trigger('change');
////                $('[name="Setup.SalesPersonId"]').val(data.salesPersonId).trigger('change');
////                $('[name="Setup.Remarks"]').val(data.remarks);
////                $('[name="Setup.Active"]').val(data.active).trigger('change');
////                // Clear file input first

////                $('#Setup_LDate').text(data.ldate);
////                $('#Setup_ModifyDate').text(data.modifyDate);

////                handleDateInfoPartial('Setup', true);


////                $('#buyerPhoto').val('');

////                // Handle photo display
////                if (data.photo && data.photoType) {
////                    var photoSrc = 'data:' + data.photoType + ';base64,' + data.photo;
////                    $('#buyerPhotoPreview').attr('src', photoSrc).show();
////                    $('#photoPlaceholder').hide();
////                    $('#btnDeletePhoto').show();
////                } else {
////                    clearPhotoDisplay();
////                }


////            } catch (e) {
////                console.error("Error populating form:", e);
////                showNotification("Error loading record details", "error");
////            }
////        }, error: function (xhr, status, error) {
////            console.error("Error fetching record:", error);
////            showNotification("Failed to load record details", "error")
////        }
////    })
////}

////$(document).on('change', '#buyer-grid-body input[type="checkbox"]', function () {
////    const id = $(this).data('id');

////    if ($(this).is(':checked')) {
////        selectedBuyerId.add(id);
////    } else {
////        selectedBuyerId.delete(id);
////    }

////    const total = $('#buyer-grid-body input[type="checkbox"]').length;
////    const checked = $('#buyer-grid-body input[type="checkbox"]:checked').length;
////    $("#buyer-check-all").prop('checked', total > 0 && total === checked);
////});

////function updateSelectedBuyerIds() {
////    const currentPageCheckboxes = $('#buyer-grid-body input[type="checkbox"]');

////    currentPageCheckboxes.each(function () {

////        const id = $(this).data('id');

////        if ($(this).is(':checked')) {
////            selectedBuyerId.add(id);
////        } else {
////            selectedBuyerId.delete(id);
////        }
////    });
////}

////function loadBuyerTableData() {
////    displayBuyerData();
////}

////function displayBuyerData(id = null) {
////    if ($.fn.DataTable.isDataTable("#buyer-grid")) {
////        $("#buyer-grid").DataTable().clear().destroy();
////    }

////    const tableBody = $("#buyer-grid-body");
////    tableBody.empty();
////    const filterData = {};
////    if (id) {
////        filterData.id = id;
////    }

////    $('#buyer-grid').DataTable({
////        processing: true,
////        serverSide: true,
////        ajax: {
////            url: BuyerUrl.List,
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
////                data: 'buyerId',
////                className: 'text-center',
////                render: function (data, type, row) {
////                    return `<a href="#buyer-form" class="py-0 buyer-id-link" data-id="${row.tc}">${data}</a>`;
////                }
////            },
////            { data: 'buyerName', className: 'py-0 text-center' },
////            { data: 'address', className: 'py-0 text-left' },
////            { data: 'countryName', className: 'py-0 text-left' },
////            { data: 'phone', className: 'py-0 text-center' },
////            { data: 'email', className: 'py-0 text-center' },
////            { data: 'contactPersonName', className: 'py-0 text-center' }
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
////            const tableId = 'buyer-grid'; // Make this unique for each table
////            let debounceTimeout;

////            // Target the specific table's search input
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
////            $('#buyer-grid-body input[type="checkbox"]').each(function () {
////                const id = $(this).data('id');
////                $(this).prop('checked', selectedBuyerId.has(id));
////            });

////            const total = $('#buyer-grid-body input[type="checkbox"]').length;
////            const checked = $('#buyer-grid-body input[type="checkbox"]:checked').length;
////            $("#buyer-check-all").prop('checked', total > 0 && total === checked);
////        }
////    });
////}

////function validateBuyerForm() {
////    var buyerTypeId = $('#Setup_BuyerTypeId').val();
////    var buyerName = $('#Setup_BuyerName').val();

////    if (!buyerName || $.trim(buyerName) === '') {
////        showNotification("Buyer Name is required.", "warning");
////        return false;
////    }

////    if (!buyerTypeId || $.trim(buyerTypeId) === '') {
////        showNotification("Buyer Type is required.", "warning");
////        return false;
////    }

////    return true;
////}

////function handleBuyerFormSubmission() {
////    if (!validateBuyerForm()) return;

////    showLoading();

////    var formData = new FormData($('#buyerForm')[0]);

////    $.ajax({
////        url: BuyerUrl.Save,
////        type: 'POST',
////        data: formData,
////        processData: false,
////        contentType: false,
////        success: function (response) {
////            if (response.success) {
////                showNotification(response.message, "success");

////                clearBuyerForm();
////                loadBuyerTableData();
////                loadCP();
////            } else {
////                showNotification(response.message, "error");
////            }
////        },
////        error: function () {
////            showNotification('Error occurred while saving buyer information.', "error");
////        },
////        complete: hideLoading
////    });
////}

////function clearPhotoDisplay() {
////    $('#buyerPhotoPreview').attr('src', '').hide();
////    $('#photoPlaceholder').show();
////    $('#btnDeletePhoto').hide();
////    $('#buyerPhoto').val('');
////}

////function clearBuyerForm() {
////    $('#buyerForm')[0].reset();
////    $('.searchable-select').val('').trigger('change');
////    clearCPSelection();
////    clearPhotoDisplay();
////    loadBuyerTableData();
////    handleDateInfoPartial('Setup', false);

////    if ($('#Setup_BuyerId').prop('readonly')) {
////        $('#Setup_BuyerId').prop('readonly', false);
////    }
////}

////function imageLoad() {
////    var file = this.files[0];
////    if (file) {
////        // Validate file type
////        if (!file.type.startsWith('image/')) {
////            alert('Please select an image file.');
////            this.value = '';
////            return;
////        }

////        // Validate file size (5MB limit)
////        if (file.size > 5 * 1024 * 1024) {
////            alert('File size must be less than 5MB.');
////            this.value = '';
////            return;
////        }

////        var reader = new FileReader();
////        reader.onload = function (e) {
////            $('#buyerPhotoPreview').attr('src', e.target.result).show();
////            $('#photoPlaceholder').hide();
////            $('#btnDeletePhoto').show();
////        };
////        reader.readAsDataURL(file);
////    } else {
////        // If no file selected, hide preview
////        $('#buyerPhotoPreview').hide();
////        $('#photoPlaceholder').show();
////        $('#btnDeletePhoto').hide();
////    }
////};

////function handleBuyerBulkDelete() {
////    const selectedIds = Array.from(selectedBuyerId);

////    if (selectedIds.length === 0) {
////        showNotification("Please select record to delete", "warning");
////        return;
////    }

////    if (!confirm(`Are you sure you want to delete ${selectedIds.length} selected Data(s)?`)) {
////        return;
////    }

////    showLoading();

////    $.ajax({
////        url: BuyerUrl.Delete,
////        type: "DELETE",
////        contentType: "application/json",
////        data: JSON.stringify(selectedIds),
////        success: function (response) {
////            selectedBuyerId.clear();
////            showNotification(response.message || "Successfully deleted", "success");
////            loadBuyerTableData();
////            clearBuyerForm();
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

////function handleBuyerDateInfo() {
////    if (isBuyerDepEditMode) {
////        $('.dateInfo').removeClass('d-none');
////    } else {
////        $('.dateInfo').addClass('d-none');
////        $('.showDepCreateDate').text('');
////        $('.showDepModifyDate').text('');
////    }
////}

//////#region CP - Optimized
////let selectedCPs = [];
////let allCPs = [];

////$(document).ready(function () {
////    initializeCPDD();
////    loadCP();
////});

////function initializeCPDD() {
////    const $button = $('#contactPersonButton');
////    const $menu = $('#contactPersonMenu');

////    // Button click handler - consolidated
////    $button.on('click', function (e) {
////        e.stopPropagation();
////        if ($menu.is(':visible')) {
////            $menu.hide();
////        } else {
////            positionDropdown();
////            $menu.show();
////        }
////    });

////    // Click outside to close
////    $(document).on('click', function (e) {
////        if (!$(e.target).closest('#contactPersonDropdown').length) {
////            $menu.hide();
////        }
////    });

////    // Clear selection
////    $('#clearSelection').on('click', function (e) {
////        e.stopPropagation();
////        clearCPSelection();
////    });
////}

////function loadCP() {
////    $.ajax({
////        url: '/BuyerInfo/GetContactPersons',
////        type: 'GET',
////        success: function (response) {
////            if (response.success && response.data) {
////                allCPs = response.data; // Store for later use
////                buildCPTable();
////            }
////        },
////        error: function (xhr, status, error) {
////            console.error('Error loading contact persons:', error);
////        }
////    });
////}

////function buildCPTable() {
////    data = allCPs;

////    const $tbody = $('#contactPersonTableBody');

////    if (!data.length) {
////        $tbody.html('<tr><td colspan="5" class="text-center text-muted p-3">No contact persons found</td></tr>');
////        return;
////    }

////    const rows = data.map(cp => {
////        const isSelected = selectedCPs.some(selected => selected.cpid === cp.cpid);
////        return `
////            <tr>
////                <td><input type="checkbox" class="contact-checkbox form-check-input"
////                           value="${cp.cpid}" data-name="${cp.contactPersonName}"
////                           ${isSelected ? 'checked' : ''}></td>
////                <td>${cp.contactPersonName}</td>
////                <td>${cp.designation || ''}</td>
////                <td>${cp.phone || ''}</td>
////                <td>${cp.email || ''}</td>
////            </tr>
////        `;
////    }).join('');

////    $tbody.html(rows);

////    $tbody.on('change', '.contact-checkbox', updateSelectedCPs);
////}

////function updateSelectedCPs() {
////    selectedCPs = $('.contact-checkbox:checked').map(function () {
////        return {
////            cpid: $(this).val(),
////            name: $(this).data('name')
////        };
////    }).get();

////    updateCPDisplay();
////    updateHiddenInput();
////}

////function updateCPDisplay() {
////    const $button = $('#contactPersonButton .selected-text');
////    const $clearBtn = $('#clearSelection');
////    const count = selectedCPs.length;

////    if (count === 0) {
////        $button.text('--Select Contact Person--');
////        $clearBtn.hide();
////    } else if (count === 1) {
////        $button.text(selectedCPs[0].name);
////        $clearBtn.show();
////    } else {
////        $button.html(`<span class="selected-count">${count} contacts selected</span>`);
////        $clearBtn.show();
////    }
////}

////function updateHiddenInput() {
////    const cpids = selectedCPs.map(cp => cp.cpid).join(',');
////    $('#contactPersonHidden').val(cpids);
////}

////function clearCPSelection() {
////    $('.contact-checkbox').prop('checked', false);
////    selectedCPs = [];
////    updateCPDisplay();
////    updateHiddenInput();
////}

////function setSelectedCP(cpids) {
////    if (!cpids) {
////        clearCPSelection();
////        return;
////    }

////    const cpidArray = cpids.split(',').map(id => id.trim());

////    selectedCPs = cpidArray
////        .map(cpid => allCPs.find(contact => contact.cpid === cpid))
////        .filter(cp => cp)
////        .map(cp => ({ cpid: cp.cpid, name: cp.contactPersonName }));

////    $('.contact-checkbox').each(function () {
////        $(this).prop('checked', cpidArray.includes($(this).val()));
////    });

////    updateCPDisplay();
////    updateHiddenInput();
////}

////const positionDropdown = () => {
////    const $btn = $('#contactPersonButton');
////    const $menu = $('#contactPersonMenu');
////    const top = $btn.offset().top - $(window).scrollTop();
////    const below = $(window).height() - top - $btn.outerHeight();
////    const showAbove = below < 300 && top > 300;

////    $menu.toggleClass('show-above', showAbove)
////        .toggleClass('show-below', !showAbove);
////};

//////#endregion

////// #region Quick add

////let loadUrl,
////    target,
////    reloadUrl,
////    title,
////    lastCode;
////// Quick add
////$("body").on("click", '.js-quick-add', function (e) {
////    e.stopPropagation();
////    e.preventDefault();
////    e.stopImmediatePropagation();

////    loadUrl = $(this).data("url");
////    target = $(this).data("target");
////    reloadUrl = $(this).data("reload-url");
////    title = $(this).data("title");

////    $("#quickAddModal .modal-title").html(title);
////    $("#quickAddModal .modal-body").empty();

////    $("#quickAddModal .modal-body").load(loadUrl, function () {
////        $('#quickAddModal').modal({
////            backdrop: 'static',
////            keyboard: false,
////            show: true
////        });

////        $('#quickAddModal').modal("show");
////        $("#header").hide();
////        $("#quickAddModal .modal-body #header").hide()

////        $("#left_menu").hide();
////        $("#quickAddModal .modal-body #left_menu").hide()

////        $("#main-content").toggleClass("collapse-main");
////        $("#quickAddModal .modal-body #main-content").toggleClass("collapse-main")

////        $("body").removeClass("sidebar-mini");
////    })
////});

////$("body").on("click", ".js-modal-dismiss", function () {
////    $("body").removeClass("sidebar-mini").addClass("sidebar-mini");

////    $("#header").show();
////    $("#quickAddModal .modal-body #header").show()

////    $("#left_menu").show();

////    $("#quickAddModal .modal-body #left_menu").show()

////    $("#main-content").toggleClass("collapse-main");
////    $("#quickAddModal .modal-body #main-content").toggleClass("collapse-main");

////    lastCode = $("#quickAddModal #lastCode").val();

////    $("#quickAddModal .modal-body").empty();
////    $("#quickAddModal").modal("hide");

////    $(target).empty("");
////    $(target).append($('<option>', {
////        value: '',
////        text: `--Select ${title}--`
////    }));

////    if (title === "Contact Person") {
////        if (typeof loadCP === 'function') {
////            loadCP();
////        }
////        return;
////    }

////    $.ajax({
////        url: reloadUrl,
////        method: "GET",
////        success: function (response) {
////            console.log(response);
////            $.each(response, function (i, item) {
////                $(target).append($('<option>', {
////                    value: item.code,
////                    text: item.name
////                }));
////            });

////            $(target).val(lastCode);
////            console.log("Testttt", lastCode);

////        }
////    });
////});

////// #endregion

