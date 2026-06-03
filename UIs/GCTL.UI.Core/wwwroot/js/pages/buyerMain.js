// main.js - Shared utilities and functions

// Utility: Get active tab identifier
const getActiveTab = () => {
    const tabMap = {
        'nav-home-tab': 'buyer',
        'nav-profile-tab': 'brand',
        'nav-contact-tab': 'dladdress'
    };
    return tabMap[$('.nav-link.active').attr('id')] || null;
};

// Utility: Initialize Select2 dropdowns
const initializeSelect = (containerSelector) => {
    // If containerSelector is provided, scope to that container
    // Otherwise, scope to the main form to avoid modal conflicts
    const $container = containerSelector
        ? $(containerSelector)
        : $(document);

    // Only initialize Select2 on elements within this container
    // that don't already have Select2 initialized
    $container.find('.searchableSelect, .searchable-select').each(function () {
        const $select = $(this);

        // Skip if already initialized
        if ($select.data('select2')) {
            return;
        }

        // Skip if this select belongs to a modal (when initializing main page)
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
};

// Utility: Format date to dd/MM/yyyy
const formatedDateddMMyyyy = (dateString) => {
    if (!dateString) return '';

    const date = new Date(dateString);
    if (isNaN(date)) return '';

    //const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
}

// Utility: Debounce function
const debounce = (func, wait = 300) => {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
};

// Utility: Ajax Request Manager
const AjaxRequestManager = (() => {
    let activeRequests = new Map();

    const abort = (key) => {
        if(activeRequests.has(key)) {
            activeRequests.get(key).abort();
            activeRequests.delete(key);
        }
    }

    const register = (key, xhr) => {
        abort(key);
        activeRequests.set(key, xhr);
    };

    const remove = (key) => {
        activeRequests.delete(key);
    }

    const abortAll = () => {
        activeRequests.forEach((xhr) => xhr.abort());
        activeRequests.clear();
    }

    return { abort, register, remove, abortAll };
})();

// Loading overlay setup and controls
const LoadingOverlay = (() => {
    const setup = () => {
        if ($("#loadingOverlay").length) return;

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
                    <div class="spinner-border text-primary" role="status"></div>
                </div>
            </div>
        `);
    };

    const show = () => {
        $('body').css('overflow', 'hidden');
        $("#loadingOverlay").css('display', 'flex').fadeIn(200);
    };

    const hide = () => {
        $('body').css('overflow', '');
        $("#loadingOverlay").fadeOut(200);
    };

    return { setup, show, hide };
})();

// Notification utility
const showNotification = (message, type = 'info') => {
    if (typeof toastr !== 'undefined') {
        const title = { success: 'Success', error: 'Error', warning: 'Warning' }[type] || 'Info';
        toastr[type](message, title);
    } else {
        alert(message);
    }
};

// Confirmation utility
const showConfirmation = (message, title = 'Are you sure?', onConfirm) => {
    if (typeof Swal === 'undefined') {
        // Fallback to native confirm if SweetAlert2 is not loaded
        if (confirm(`${message}`)) {
            onConfirm();
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
            onConfirm();
        }
    });
};

// Date info handler for forms
const handleDateInfoPartial = (formPrefix, isEditMode) => {
    const dateInfoElement = $(`#${formPrefix}_DateInfo`);
    const ldate = $(`#${formPrefix}_LDate`);
    const modifyDate = $(`#${formPrefix}_ModifyDate`);

    if (isEditMode) {
        dateInfoElement.removeClass('d-none');
    } else {
        dateInfoElement.addClass('d-none');
        ldate.text('');
        modifyDate.text('');
    }
};

// DataTable initialization helper
const initDataTable = (tableId, config) => {
    if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
        $(`#${tableId}`).DataTable().destroy();
        $(`#${tableId}`).empty();
    }

    //$(`#${tableId}-body`).empty();

    const defaultConfig = {
        processing: true,
        serverSide: true,
        autoWidth: false,
        fixedHeader: false,
        info: true,
        lengthChange: true,
        lengthMenu: [[10, 25, 50, 100, 1000, -1], [10, 25, 50, 100, 1000, "All"]],
        ordering: true,
        order: [1, 'desc'],
        pageLength: 10,
        paging: true,
        responsive: true,
        scrollCollapse: true,
        scrollX: true,
        searching: true,
        deferRender: true,
        searchDelay: null, 
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
        },
        initComplete: function () {
            const api = this.api();
            const $searchInput = $(`#${tableId}_wrapper .dataTables_filter input`);

            $searchInput.unbind();
            $searchInput.off();

            const debouncedSearch = debounce(function (searchTerm) {
                api.search(searchTerm).draw();
            }, 500);

            $searchInput.on('keyup.DT cut.DT paste.DT input.DT', function (e) {
                e.stopImmediatePropagation();
                debouncedSearch(this.value);
            });
        }
    };

    return $(`#${tableId}`).DataTable({ ...defaultConfig, ...config });
};

// Checkbox selection manager
class CheckboxManager {
    constructor(gridId, checkAllId) {
        this.gridId = gridId;
        this.checkAllId = checkAllId;
        this.selectedIds = new Set();
        this.init();
    }

    init() {
        $(`#${this.checkAllId}`).on('change', (e) => {
            const isChecked = $(e.target).is(':checked');
            $(`#${this.gridId}-body input[type="checkbox"]`).prop('checked', isChecked);
            this.updateSelected();
        });

        $(document).on('change', `#${this.gridId}-body input[type="checkbox"]`, (e) => {
            const id = $(e.target).data('id');
            if ($(e.target).is(':checked')) {
                this.selectedIds.add(id);
            } else {
                this.selectedIds.delete(id);
            }
            this.updateCheckAll();
        });
    }

    updateSelected() {
        $(`#${this.gridId}-body input[type="checkbox"]`).each((_, el) => {
            const id = $(el).data('id');
            if ($(el).is(':checked')) {
                this.selectedIds.add(id);
            } else {
                this.selectedIds.delete(id);
            }
        });
    }

    updateCheckAll() {
        const total = $(`#${this.gridId}-body input[type="checkbox"]`).length;
        const checked = $(`#${this.gridId}-body input[type="checkbox"]:checked`).length;
        $(`#${this.checkAllId}`).prop('checked', total > 0 && total === checked);
    }

    syncWithTable() {
        $(`#${this.gridId}-body input[type="checkbox"]`).each((_, el) => {
            const id = $(el).data('id');
            $(el).prop('checked', this.selectedIds.has(id));
        });
        this.updateCheckAll();
    }

    clear() {
        this.selectedIds.clear();
    }

    getSelected() {
        return Array.from(this.selectedIds);
    }
}

// Image preview handler
const setupImagePreview = (inputId, previewId, placeholderId, deleteBtnId, deleteUrlConfig = null) => {
    $(`#${inputId}`).on('change', function () {
        const file = this.files[0];
        if (!file) {
            $(`#${previewId}`).hide();
            $(`#${placeholderId}`).show();
            $(`#${deleteBtnId}`).hide();
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
            $(`#${previewId}`).attr('src', e.target.result).show();
            $(`#${placeholderId}`).hide();
            $(`#${deleteBtnId}`).show();

            $(`#${deleteBtnId}`).data('from-db', false);
        };
        reader.readAsDataURL(file);
    });

    $(`#${deleteBtnId}`).on('click', function () {
        const isFromDB = $(this).data('from-db');

        if (isFromDB && deleteUrlConfig) {

            showConfirmation('Are you sure you want to delete this photo?',
                'Confirmation',
                () => {


                    LoadingOverlay.show();

                    $.ajax({
                        url: deleteUrlConfig.url,
                        type: 'POST',
                        data: deleteUrlConfig.getData(),
                        success: (res) => {
                            if (res.success) {
                                showNotification(res.message);
                                clearImage();

                                if (deleteUrlConfig.onSuccess) {
                                    deleteUrlConfig.onSuccess();
                                }
                            } else {
                                showNotification(res.message);
                            }
                        }, error: () => {
                            showNotification('Error occurred while deleting photo', 'error');
                        }, complete: LoadingOverlay.hide
                    });
                }
            )
        } else {
            clearImage();
        }
    });

    function clearImage() {
        $(`#${previewId}`).attr('src', '').hide();
        $(`#${placeholderId}`).show();
        $(`#${deleteBtnId}`).hide().removeData('from-db');
        $(`#${inputId}`).val('');
    }
};


const QuickAddModal = (() => {
    // Stack to track multiple modal instances
    const modalStack = [];
    let mutationObservers = new Map();
    let processingFlags = new Map();

    /**
     * Generate unique modal ID for each level
     */
    const getModalId = (level) => {
        return level === 0 ? 'quickAddModal' : `quickAddModal_level${level}`;
    };

    /**
     * Get or create modal element for specific level
     */
    const getOrCreateModal = (level) => {
        const modalId = getModalId(level);
        let $modal = $(`#${modalId}`);

        if ($modal.length === 0 && level > 0) {
            // Clone the base modal for nested levels
            $modal = $('#quickAddModal').clone();
            $modal.attr('id', modalId);
            $modal.css('z-index', 1050 + (level * 10)); // Increase z-index for each level

            // Update backdrop z-index
            $modal.on('shown.bs.modal', function () {
                $(`.modal-backdrop`).eq(level).css('z-index', 1040 + (level * 10));
            });

            $('body').append($modal);
        }

        return $modal;
    };

    const open = (config) => {
        console.log('open');

        const currentLevel = modalStack.length;
        const modalId = getModalId(currentLevel);
        const $modal = getOrCreateModal(currentLevel);

        // Store config in stack
        modalStack.push({
            loadUrl: config.loadUrl,
            target: config.target,
            reloadUrl: config.reloadUrl,
            title: config.title,
            level: currentLevel,
            modalId: modalId,
            lastCode: null
        });

        // Set modal title and clear body
        $modal.find('.modal-title').html(config.title);
        $modal.find('.modal-body').empty();

        // Load external content into modal
        $modal.find('.modal-body').load(config.loadUrl, () => {
            // Show modal with static backdrop
            $modal.modal({
                backdrop: 'static',
                keyboard: false,
                show: true
            });
            $modal.modal("show");

            // Wait for remote page's JS to finish
            setTimeout(() => {
                // Remove ALL Select2 DOM elements
                $modal.find('.select2-container').remove();

                // Destroy any remaining instances
                destroyAllModalSelect2(modalId);

                // Remove Select2 data
                $modal.find('select').removeData('select2');

                // Initialize fresh
                initModalSelect2(modalId);
            }, 500);

            // Start watching for dynamically added Select2 elements
            watchModalForSelect2(modalId);

            // Only hide UI elements for first level modal
            if (currentLevel === 0) {
                $("#header").hide();
                $("#left_menu").hide();
                $("#main-content").toggleClass("collapse-main");
                $("body").removeClass("sidebar-mini");
            }

            // Hide header and menu within modal content
            $modal.find('#header').hide();
            $modal.find('#left_menu').hide();
            $modal.find('#main-content').toggleClass("collapse-main");
        });
    };

    const close = () => {
        console.log('close');

        // Check if there are any open modals
        if (modalStack.length === 0) {
            console.warn('No QuickAddModal instances to close');
            return;
        }

        // Get the most recent modal instance
        const currentModal = modalStack.pop();
        const { modalId, target, reloadUrl, title, level } = currentModal;
        const $modal = $(`#${modalId}`);

        // Get lastCode from the modal being closed
        let lastCode = $modal.find('#lastCode').val();
        if (!lastCode || lastCode.trim() === '') {
            lastCode = $(`#lastCode`).val();
        }
        currentModal.lastCode = lastCode;

        // Destroy Select2 instances in this modal
        $modal.find('select').each(function () {
            const $select = $(this);
            if ($select.data('select2')) {
                try {
                    $select.select2('close');
                    $select.select2('destroy');
                } catch (error) {
                    console.error('Error destroying Select2:', error);
                }
            }
            $select.removeData();
        });

        // Disconnect observer for this modal
        disconnectObserver(modalId);

        // Clean up event handlers and data
        $modal.find('.modal-body *').off();
        $modal.find('.modal-body').off();
        $modal.find('.modal-body *').removeData();
        $modal.find('.modal-body').removeData();

        // Restore UI only if closing the base modal
        if (level === 0) {
            $("body").removeClass("sidebar-mini").addClass("sidebar-mini");
            $("#header").show();
            $("#left_menu").show();
            $("#main-content").toggleClass("collapse-main");
        }

        // Clear and hide modal
        $modal.find('.modal-body').empty();
        $modal.modal("hide");

        // Remove cloned modal elements (keep base modal)
        if (level > 0) {
            setTimeout(() => {
                $modal.remove();
            }, 300); // Wait for hide animation
        }

        // Handle specific cases
        if (title === "Contact Person") {
            if (typeof window.loadCP === 'function') {
                window.loadCP();
            }
            return;
        }

        console.log('Closing modal with lastCode:', lastCode);

        // Reload dropdown if target exists
        if (target && reloadUrl) {
            reloadDropdown(target, reloadUrl, title, lastCode);
        }
    };

    /**
     * Close all modals in the stack
     */
    const closeAll = () => {
        while (modalStack.length > 0) {
            close();
        }
    };

    /**
     * Destroys all Select2 instances in a specific modal
     */
    const destroyAllModalSelect2 = (modalId) => {
        // Only destroy Select2 instances that belong to THIS modal
        $(`#${modalId} > .modal-dialog > .modal-content > .modal-body`).find('select').each(function () {
            const $select = $(this);

            // Skip if this select belongs to a nested modal
            if ($select.closest('.modal').attr('id') !== modalId) {
                return;
            }

            if ($select.data('select2')) {
                try {
                    $select.select2('destroy');
                } catch (error) {
                    console.error('Error destroying Select2:', error);
                }
            }
        });
    };

    /**
     * Reloads the target dropdown with fresh data from server
     */
    const reloadDropdown = (target, reloadUrl, title, lastCode) => {
        if (!target) {
            console.warn('No target defined for reload');
            return;
        }

        $(target).empty();
        $(target).append($('<option>', {
            value: '',
            text: `--Select ${title}--`
        }));

        $.ajax({
            url: reloadUrl,
            method: "GET",
            success: (response) => {
                $.each(response, (i, item) => {
                    $(target).append($('<option>', {
                        value: item.code,
                        text: item.name
                    }));
                });
                if (lastCode) {
                    $(target).val(lastCode);
                }
            },
            error: (error) => {
                console.error('Error reloading dropdown:', error);
            }
        });
    };

    const initModalSelect2 = (modalId) => {
        const select2Classes = ['.selectpickers9', '.selectpickersCom', '.selectpickers', '.searchable-select'];

        select2Classes.forEach(className => {
            // CRITICAL: Use direct children selector to avoid nested modal selects
            $(`#${modalId} > .modal-dialog > .modal-content > .modal-body`).find(className).each(function () {
                const $select = $(this);

                // Skip if already initialized
                if ($select.data('select2')) {
                    return;
                }

                // Skip if this select belongs to a nested modal
                if ($select.closest('.modal').attr('id') !== modalId) {
                    return;
                }

                $select.select2({
                    width: '98%',
                    dropdownParent: $(`#${modalId}`),
                    language: { noResults: () => "No results found" },
                    escapeMarkup: markup => markup
                });
            });
        });
    };

    const reinitializeSelect2 = (modalId) => {
        const processingKey = modalId;
        if (processingFlags.get(processingKey)) return;
        processingFlags.set(processingKey, true);

        // First destroy any existing instances
        destroyAllModalSelect2(modalId);

        // Then reinitialize
        const select2Classes = ['.selectpickers9', '.selectpickersCom', '.selectpickers', '.searchable-select'];

        select2Classes.forEach(className => {
            // CRITICAL: Only select elements that belong to THIS modal, not nested ones
            $(`#${modalId} > .modal-dialog > .modal-content > .modal-body`).find(className).each(function () {
                const $select = $(this);

                // Skip if this select belongs to a nested modal
                if ($select.closest('.modal').attr('id') !== modalId) {
                    return;
                }

                // Destroy if exists
                if ($select.data('select2')) {
                    $select.select2('destroy');
                }

                // Remove any leftover Select2 containers
                $select.next('.select2-container').remove();
                $select.siblings('.select2-container').remove();

                // Clean up classes and attributes
                $select.removeClass('select2-hidden-accessible');
                $select.removeAttr('data-select2-id aria-hidden tabindex');

                // Initialize fresh with correct dropdownParent
                $select.select2({
                    width: '98%',
                    dropdownParent: $(`#${modalId}`),
                    language: { noResults: () => 'No results found' },
                    escapeMarkup: markup => markup
                });
            });
        });

        setTimeout(() => {
            processingFlags.set(processingKey, false);
        }, 1000);
    };

    const watchModalForSelect2 = (modalId) => {
        const targetNode = document.querySelector(`#${modalId} > .modal-dialog > .modal-content > .modal-body`);

        if (!targetNode) {
            setTimeout(() => watchModalForSelect2(modalId), 500);
            return;
        }

        disconnectObserver(modalId);

        const config = { childList: true, subtree: true };
        let debounceTimer;

        const callback = function (mutationsList, observerInstance) {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => {
                // Only check for selects that belong to THIS modal
                const $modal = $(`#${modalId} > .modal-dialog > .modal-content > .modal-body`);

                const $selectsCom = $modal.find('.selectpickersCom').filter(function () {
                    return $(this).closest('.modal').attr('id') === modalId;
                });
                const $selects9 = $modal.find('.selectpickers9').filter(function () {
                    return $(this).closest('.modal').attr('id') === modalId;
                });
                const $selects = $modal.find('.selectpickers').filter(function () {
                    return $(this).closest('.modal').attr('id') === modalId;
                });
                const $select = $modal.find('.searchable-select').filter(function () {
                    return $(this).closest('.modal').attr('id') === modalId;
                });

                if ($selectsCom.length > 0 || $selects9.length > 0 || $selects.length > 0 || $select.length > 0) {
                    reinitializeSelect2(modalId);
                }
            }, 300);
        };

        const observer = new MutationObserver(callback);
        observer.observe(targetNode, config);
        mutationObservers.set(modalId, observer);
    };

    const disconnectObserver = (modalId) => {
        const observer = mutationObservers.get(modalId);
        if (observer) {
            observer.disconnect();
            mutationObservers.delete(modalId);
        }
    };

    // Public API
    return {
        open,
        close,
        closeAll,
        getStackDepth: () => modalStack.length,
        isOpen: () => modalStack.length > 0
    };
})();

function initQuickAddModal() {
    console.log('initQuickAddModal');

    $("body").on("click", '.js-quick-add', function (e) {
        e.stopPropagation();
        e.preventDefault();
        e.stopImmediatePropagation();

        QuickAddModal.open({
            loadUrl: $(this).data("url"),
            target: $(this).data("target"),
            reloadUrl: $(this).data("reload-url"),
            title: $(this).data("title")
        });
    });

    $("body").on("click", ".js-modal-dismiss", () => QuickAddModal.close());
}

//// Quick Add Modal Handler
//const QuickAddModal = (() => {
//    let loadUrl, target, reloadUrl, title, lastCode;

//    const open = (config) => {
//        ({ loadUrl, target, reloadUrl, title } = config);
//        $("#quickAddModal .modal-title").html(title);
//        $("#quickAddModal .modal-body").empty();
//        $("#quickAddModal .modal-body").load(loadUrl, () => {
//            $('#quickAddModal').modal({
//                backdrop: 'static',
//                keyboard: false,
//                show: true
//            });
//            $('#quickAddModal').modal("show");

//            // Initialize Select2 for elements in modal
//            setTimeout(() => {
//                $('#quickAddModal').find('.selectpickersCom').each(function () {
//                    $(this).select2({
//                        width: '98%',
//                        dropdownParent: $('#quickAddModal'),
//                        language: {
//                            noResults: function () { return "No results found"; }
//                        },
//                        escapeMarkup: function (markup) { return markup; }
//                    });
//                });

//                $('#quickAddModal').find('.selectpickers9').each(function () {
//                    $(this).select2({
//                        width: '98%',
//                        dropdownParent: $('#quickAddModal'),
//                        language: {
//                            noResults: function () { return "No results found"; }
//                        },
//                        escapeMarkup: function (markup) { return markup; }
//                    });
//                });
//            }, 100);

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
//                    console.error('Error occurred while unbinding events:', error);
//                }
//            }

//            $select.removeData();
//        });

//        watchModalForSelect2.disconnect();

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
//        lastCode = $("#quickAddModal #lastCode").val();
//        $("#quickAddModal .modal-body").empty();
//        $("#quickAddModal").modal("hide");

//        if (title === "Contact Person") {
//            if (typeof loadCP === 'function') {
//                loadCP();
//            }
//            return;
//        }

//        $(target).empty("");
//        $(target).append($('<option>', {
//            value: '',
//            text: `--Select ${title}--`
//        }));

//        $.ajax({
//            url: reloadUrl,
//            method: "GET",
//            success: (response) => {
//                console.log(response);
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

//    return { open, close };
//})();

//// Event bindings
//$("body").on("click", '.js-quick-add', function (e) {
//    e.stopPropagation();
//    e.preventDefault();
//    e.stopImmediatePropagation();
//    QuickAddModal.open({
//        loadUrl: $(this).data("url"),
//        target: $(this).data("target"),
//        reloadUrl: $(this).data("reload-url"),
//        title: $(this).data("title")
//    });
//});

//$("body").on("click", ".js-modal-dismiss", () => QuickAddModal.close());

const watchModalForSelect2 = (() => {
    let observer = null;
    let isProcessing = false;

    const disconnect = () => {
        if (observer) {
            observer.disconnect();
            observer = null;
        }
    }

    const reinitializeSelect2 = () => {
        if (isProcessing) return;
        isProcessing = true;

        $('#quickAddModal .selectpickersCom').each(function () {

            const $select = $(this);
            if (!$select.data('select2')) { // Only init if not already initialized
                $select.select2({
                    width: '98%',
                    dropdownParent: $('#quickAddModal'),
                    language: { noResults: () => 'No results found' },
                    escapeMarkup: markup => markup
                });
            }

            //if ($(this).data('select2')) {
            //    $(this).select2('destroy');
            //}
            //$(this).select2({
            //    width: '98%',
            //    dropdownParent: $('#quickAddModal'),
            //    language: {
            //        noResults: function () { return "No results found"; }
            //    },
            //    escapeMarkup: function (markup) { return markup; }
            //});
        });

        // Handle selectpickers9
        $('#quickAddModal .selectpickers9').each(function () {
            if ($(this).data('select2')) {
                $(this).select2('destroy');
            }
            $(this).select2({
                width: '98%',
                dropdownParent: $('#quickAddModal'),
                language: { noResults: () => 'No results found' },
                escapeMarkup: markup => markup
            });
        });

        setTimeout(() => { isProcessing = false; }, 1000);
    };

    const init = () => {
        const targetNode = document.querySelector('#quickAddModal .modal-body');

        if (!targetNode) {
            setTimeout(init, 500);
            return;
        }

        disconnect();

        const config = { childList: true, subtree: true };
        let debounceTimer;

        const callback = function (mutationsList, observerInstance) {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => {
                const $selectsCom = $('#quickAddModal .selectpickersCom');
                const $selects9 = $('#quickAddModal .selectpickers9');

                if ($selectsCom.length > 0 || $selects9.length > 0) {
                    reinitializeSelect2();
                }
            }, 300);
        };

        observer = new MutationObserver(callback);
        observer.observe(targetNode, config);
    };

    return { init, disconnect };
})();

// Initialize on document ready
$(document).ready(() => {
    LoadingOverlay.setup();
    initializeSelect();

    setTimeout(watchModalForSelect2.init, 100);

    document.querySelectorAll('#buyer-nav-tab button[data-bs-toggle="tab"]').forEach(button => {
        button.addEventListener('shown.bs.tab', function (e) {
            const targetId = e.target.getAttribute('data-bs-target');
            const targetPane = document.querySelector(targetId);

            const tabText = e.target.textContent.trim(); // or e.target.innerText
            const pageTitle = document.querySelector('.page-title');

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

    initQuickAddModal();


    //$('#quickAddModal').on('hidden.bs.modal', function () {
    //    watchModalForSelect2.disconnect();
    //});
});