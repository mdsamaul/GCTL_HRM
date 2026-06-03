
(function ($) {
    'use strict';

    const CONFIG = {
        namespace: 'buyerDepartment',   // Module namespace for event binding to prevent conflicts

        baseUrl: '/BuyerDepartment',    // Base URL for API endpoints

        formSelector: '#buyerDepForm',              // Main form element
        fieldPrefix: 'Department',                   // Prefix for form field names
        gridSelector: '#buyer-dep-grid',            // DataTable container
        gridBodySelector: '#buyer-dep-grid-body',   // DataTable body for checkbox management

        saveSelector: '.js-buyer-dep-save',
        clearSelector: '#js-buyer-dep-clear',
        deleteSelector: '#js-buyer-dep-delete-confirm',
        selectAllSelector: '#buyer-dep-check-all',

        idLinkClass: 'buyer-dep-id-link',           // CSS class for clickable ID links
        lastCodeSelector: '#lastCode',               // Hidden input for last generated code
        dateCreateClass: '.showDepCreateDate',       // Display element for creation date
        dateModifyClass: '.showDepModifyDate',       // Display element for modification date
        dateInfoClass: '.dateInfo',                  // Container for date information

        fields: {
            tc: 'Tc',                                // (hidden primary key)
            id: 'BuyerDepartmentId',                // Display ID field
            name: 'DepartmentName',                 // Department name (required)
            additional: ['ShortName']               // Additional optional fields
        },

        apiFieldMap: {
            tc: 'tc',
            id: 'buyerDepartmentId',
            name: 'departmentName',
            shortName: 'shortName',
            createDate: 'ldate',
            modifyDate: 'modifyDate'
        },

        photo: {
            enabled: false,
            inputSelector: '#Setup_BuyerPhoto',
            previewSelector: '#buyerPhotoPreview',
            placeholderSelector: '#photoPlaceholder',
            deleteButtonSelector: '#btnDeleteBuyerPhoto',
            photoField: 'photo',
            photoTypeField: 'photoType'
        },

        contactPerson: {
            hiddenInputName: 'Setup.ContatPerson1', // Hidden input to store selected CP IDs
            apiField: 'contatPerson1'               // API field name for contact persons
        },

        tableColumns: [
            {
                data: null,
                orderable: false,
                className: 'text-center no-sort',
                render: function (data, type, row) {
                    return `<input type="checkbox" width="1%" style="padding:0;" class="py-0 no-sort" data-id="${row.tc}"/>`;
                }
            },
            {
                data: 'buyerDepartmentId',
                className: 'text-center',
                render: function (data, type, row) {
                    return `<a href="#buyer-dep-form" class="py-0 buyer-dep-id-link" data-id="${row.tc}">${data}</a>`;
                }
            },
            {
                data: 'departmentName',
                className: 'py-0 text-center'
            },
            {
                data: 'shortName',
                className: 'py-0 text-left'
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
            quickAdd: false,            // Enable Quick Add Modal for related entities
            contactPerson: false        // Enable Contact Person multi-select dropdown
        }
    };

    const URLS = {
        newId: CONFIG.baseUrl + '/GenerateDepID',
        save: CONFIG.baseUrl + '/SaveBuyerDepartment',
        list: CONFIG.baseUrl + '/GetBuyerDepartmentList',
        details: CONFIG.baseUrl + '/GetBuyerDepartmentById',
        delete: CONFIG.baseUrl + '/BulkBuyerDepartmentDelete',
        cp: '/ContactPerson/GetContactPersonList'
    };

    const state = {
        selectedIds: new Set(),     // Set of selected record IDs for bulk operations
        isEditMode: false,          // Flag indicating if form is in edit mode
        currentTable: null          // Reference to current DataTable instance
    };

    //#region add Modal
    // ========================================================================
    // QUICK ADD MODAL MODULE
    // ========================================================================

    //const QuickAddModal = (() => {
    //    // Private variables
    //    let loadUrl, target, reloadUrl, title, lastCode;
    //    let mutationObserver = null;
    //    let isProcessing = false;

    //    const open = (config) => {
    //        ({ loadUrl, target, reloadUrl, title } = config);

    //        // Set modal title and clear body
    //        $("#quickAddModal .modal-title").html(title);
    //        $("#quickAddModal .modal-body").empty();

    //        // Load external content into modal
    //        $("#quickAddModal .modal-body").load(loadUrl, () => {
    //            // Show modal with static backdrop (prevents accidental closure)
    //            $('#quickAddModal').modal({
    //                backdrop: 'static',
    //                keyboard: false,
    //                show: true
    //            });
    //            $('#quickAddModal').modal("show");

    //            setTimeout(() => {
    //                initModalSelect2();
    //            }, 100);

    //            // Start watching for dynamically added Select2 elements
    //            watchModalForSelect2();

    //            $("#header").hide();
    //            $("#quickAddModal .modal-body #header").hide();
    //            $("#left_menu").hide();
    //            $("#quickAddModal .modal-body #left_menu").hide();
    //            $("#main-content").toggleClass("collapse-main");
    //            $("#quickAddModal .modal-body #main-content").toggleClass("collapse-main");
    //            $("body").removeClass("sidebar-mini");
    //        });
    //    };

    //    const close = () => {
    //        $('#quickAddModal').find('select').each(function () {
    //            const $select = $(this);
    //            if ($select.data('select2')) {
    //                try {
    //                    $select.select2('close');
    //                    $select.select2('destroy');
    //                } catch (error) {
    //                    console.error('Error destroying Select2:', error);
    //                }
    //            }
    //            $select.removeData();
    //        });

    //        disconnectObserver();

    //        $('#quickAddModal .modal-body *').off();
    //        $('#quickAddModal .modal-body').off();
    //        $('#quickAddModal .modal-body *').removeData();
    //        $('#quickAddModal .modal-body').removeData();

    //        $("body").removeClass("sidebar-mini").addClass("sidebar-mini");
    //        $("#header").show();
    //        $("#quickAddModal .modal-body #header").show();
    //        $("#left_menu").show();
    //        $("#quickAddModal .modal-body #left_menu").show();
    //        $("#main-content").toggleClass("collapse-main");
    //        $("#quickAddModal .modal-body #main-content").toggleClass("collapse-main");

    //        $("#quickAddModal .modal-body").empty();
    //        $("#quickAddModal").modal("hide");

    //        if (title === "Contact Person") {
    //            if (typeof window.loadCP === 'function') {
    //                window.loadCP();
    //            }
    //            return;
    //        }

    //        lastCode = $("#quickAddModal #lastCode").val();

    //        reloadDropdown();
    //    };

    //    /**
    //     * Reloads the target dropdown with fresh data from server
    //     */
    //    const reloadDropdown = () => {
    //        $(target).empty("");
    //        $(target).append($('<option>', {
    //            value: '',
    //            text: `--Select ${title}--`
    //        }));

    //        $.ajax({
    //            url: reloadUrl,
    //            method: "GET",
    //            success: (response) => {
    //                $.each(response, (i, item) => {
    //                    $(target).append($('<option>', {
    //                        value: item.code,
    //                        text: item.name
    //                    }));
    //                });
    //                $(target).val(lastCode);
    //            }
    //        });
    //    };

    //    const initModalSelect2 = () => {
    //        $('#quickAddModal').find('.selectpickersCom, .selectpickers9, .selectpickers').each(function () {
    //            $(this).select2({
    //                width: '98%',
    //                dropdownParent: $('#quickAddModal'),
    //                language: { noResults: () => "No results found" },
    //                escapeMarkup: markup => markup
    //            });
    //        });
    //    };

    //    const reinitializeSelect2 = () => {
    //        if (isProcessing) return;
    //        isProcessing = true;
    //        $('#quickAddModal .selectpickersCom, #quickAddModal .selectpickers9, #quickAddModal .selectpickers').each(function () {
    //            const $select = $(this);
    //            if (!$select.data('select2')) {
    //                $select.select2({
    //                    width: '98%',
    //                    dropdownParent: $('#quickAddModal'),
    //                    language: { noResults: () => 'No results found' },
    //                    escapeMarkup: markup => markup
    //                });
    //            }
    //        });
    //        setTimeout(() => { isProcessing = false; }, 1000);
    //    };

    //    const watchModalForSelect2 = () => {
    //        const targetNode = document.querySelector('#quickAddModal .modal-body');

    //        if (!targetNode) {
    //            setTimeout(watchModalForSelect2, 500);
    //            return;
    //        }

    //        disconnectObserver();

    //        const config = { childList: true, subtree: true };
    //        let debounceTimer;

    //        const callback = function (mutationsList, observerInstance) {
    //            clearTimeout(debounceTimer);
    //            debounceTimer = setTimeout(() => {
    //                const $selectsCom = $('#quickAddModal .selectpickersCom');
    //                const $selects9 = $('#quickAddModal .selectpickers9');
    //                const $selects = $('#quickAddModal .selectpickers');

    //                if ($selectsCom.length > 0 || $selects9.length > 0 || $selects.length > 0) {
    //                    reinitializeSelect2();
    //                }
    //            }, 300);
    //        };

    //        mutationObserver = new MutationObserver(callback);
    //        mutationObserver.observe(targetNode, config);
    //    };

    //    const disconnectObserver = () => {
    //        if (mutationObserver) {
    //            mutationObserver.disconnect();
    //            mutationObserver = null;
    //        }
    //    };

    //    return { open, close };
    //})();
    //#endregion add Modal

    // #region Contact Person Module ========================================================================
    // CONTACT PERSON MODULE
    // ======================================================================================================

    const ContactPersonModule = (() => {
        // Private variables
        let selectedCPs = [];           // Array of selected contact persons
        let allCPs = [];                // Array of all available contact persons
        const CP_URL = '/ContactPerson/GetContactPersonList'; // API endpoint

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

        /**
         * Initializes search functionality for contact person table
         */
        const initSearch = () => {
            const $searchInput = $('#cpSearch');
            if (!$searchInput.length) return;

            $searchInput.off('input').on('input', debounce(function () {
                const searchTerm = $('#cpSearch').val().toLowerCase().trim();
                filterTable(searchTerm);
            }, 200));
        };

        /**
         * Filters contact person table based on search term
         */
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

        /**
         * Loads contact persons from server via AJAX
         */
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

        /**
         * Builds the contact person table HTML
         */
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

        /**
         * Updates the selectedCPs array based on checked checkboxes
         */
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
                $button.text('--Select Contact Person--');
                $clearBtn.hide();
            } else if (count === 1) {
                $button.text(selectedCPs[0].name);
                $clearBtn.show();
            } else {
                $button.html(`<span class="selected-count">${count} contacts selected</span>`);
                $clearBtn.show();
            }
        };

        const updateHiddenInput = () => {
            const cpids = selectedCPs.map(cp => cp.cpid).join(',');
            const inputName = window.CONFIG?.contactPerson?.hiddenInputName || 'Setup.ContatPerson1';
            $(`[name="${inputName}"]`).val(cpids);
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
    // #endregion Contact Person Module ========================================================================

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
        generateNewId();

        setupEnterKeyNavigation();

        //if (CONFIG.features.quickAdd) {
        //    initQuickAddModal();
        //}
        //if (CONFIG.features.contactPerson) {
        //    ContactPersonModule.init();
        //}
    }

    //function initQuickAddModal() {
    //    $("body").on("click", '.js-quick-add', function (e) {
    //        e.stopPropagation();
    //        e.preventDefault();
    //        e.stopImmediatePropagation();

    //        QuickAddModal.open({
    //            loadUrl: $(this).data("url"),
    //            target: $(this).data("target"),
    //            reloadUrl: $(this).data("reload-url"),
    //            title: $(this).data("title")
    //        });
    //    });

    //    $("body").on("click", ".js-modal-dismiss", () => QuickAddModal.close());
    //}

    function bindEvents() {
        const ns = `.${CONFIG.namespace}`;

        $("body").off(ns);
        $(CONFIG.gridSelector).off(ns);

        $("body").on(`click${ns}`, CONFIG.saveSelector, handleFormSubmit);

        $("body").on(`click${ns}`, CONFIG.clearSelector, function () {
            clearForm();
            generateNewId();
            clearAllSelections();
        });

        $("body").on(`click${ns}`, CONFIG.deleteSelector, handleBulkDelete);

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

    function showConfirmation(message, title, callback) {
        if (typeof Swal === 'undefined') {
            // Fallback to native confirm if SweetAlert2 is not loaded
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

                    $(CONFIG.formSelector).find('.searchableSelect, .searchable-select').trigger('change');

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
                    generateNewId();

                    if (response.lastCode) {
                        $(CONFIG.lastCodeSelector).val(response.lastCode);
                    }

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

    function generateNewId() {
        $.ajax({
            url: URLS.newId,
            type: "GET",
            success: function (res) {
                setFieldValue(CONFIG.fields.id, res);
            },
            error: function (xhr, status, error) {
                showNotification("Error generating new ID", "error");
                console.error("Error:", status, error);
            }
        });
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

    function loadTableData() {
        displayTableData();
    }

    function displayTableData() {
        if ($.fn.DataTable.isDataTable(CONFIG.gridSelector)) {
            $(CONFIG.gridSelector).DataTable().clear().destroy();
        }

        $(CONFIG.gridBodySelector).empty();

        state.currentTable = $(CONFIG.gridSelector).DataTable({
            ...CONFIG.tableOptions,
            ajax: {
                url: URLS.list,
                type: 'POST'
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
                generateNewId();
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