(function ($) {
    $.supplierInformation = function (options) {

        //#region Default options

        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#SupplierInformation-form",
            formContainer: ".js-SupplierInformation-form-container",
            gridSelector: "#SupplierInformation-grid",
            gridContainer: ".js-SupplierInformation-grid-container",
            gridSelectorBank: "#SupplierBankAccountTemp-grid",
            gridContainerBank: ".js-SupplierBankAccountTemp-grid-container",
            editSelector: ".js-SupplierInformation-edit",
            saveSelector: ".js-SupplierInformation-save",
            selectAllSelector: "#SupplierInformation-check-all",
            deleteSelector: ".js-SupplierInformation-delete-confirm",
            deleteModal: "#SupplierInformation-delete-modal",
            finalDeleteSelector: ".js-SupplierInformation-delete",
            clearSelector: ".js-SupplierInformation-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-SupplierInformation-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-SupplierInformation-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () {

            }
        }, options);

        var gridUrl = settings.baseUrl + "/Grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        let supplierInformationTable = null;

        //#endregion

        $(() => {

            loadTable();
            //ResetForm();
            select2DD();
            loadSNCId();
            loadBankAccountInfoTable();
            enterKeyNavigation();
            BankAccountInfoClearTable();
            $('#LdateModifyHide').hide();
            $('#sectionBreak').hide();
            $('.SupplierBankAccountTemp-grid-container').hide();

            //#region formatDateTime

            function formatDateTime(dateStr) {
                var date = new Date(dateStr);
                return date.toLocaleString('en-GB', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                    hour12: true
                });
            }

            //#endregion

            //#region GetById

            $(document).on('click', '.js-SupplierInformation-edit', function (e) {
                e.preventDefault();
                var id = $(this).data('id');
                $.ajax({
                    url: '/SupplierInformation/GetById',
                    type: 'GET',
                    data: { code: id },
                    success: function (data) {
                        if (data) {
                           // console.log(data);
                            $('#Tc').val(data.tc);
                            $('#SupplierId').val(data.supplierId);
                            $('#SupplierName').val(data.supplierName);
                            $('#SupplierCode').val(data.supplierCode);
                            $('#SupplierCategoryId').val(data.supplierCategoryId).trigger('change');
                            $('#SupplierTypeId').val(data.supplierTypeId).trigger('change');
                            $('#SupplierOriginId').val(data.supplierOriginId).trigger('change');
                            $('#CompanyId').val(data.companyId).trigger('change');
                            $('#Optype').val(data.optype).trigger('change');
                            $('#Active').val(data.active).trigger('change');
                            $('#CountryId').val(data.countryId).trigger('change');
                            //$('#ContatPerson1').val(data.contatPerson1).trigger('change');
                            setSelectedCP(data.contatPerson1);
                            $('#SalesPersonId').val(data.salesPersonId).trigger('change');
                            $('#SupplierBankId').val(data.supplierBankId).trigger('change');
                            $('#SupplierBankBranchId').val(data.supplierBankBranchId).trigger('change');
                            $('#Address').val(data.address);
                            $('#LocalOfficeAddress').val(data.localOfficeAddress);
                            $('#City').val(data.city);
                            $('#State').val(data.state);
                            $('#ZipCode').val(data.zipCode);
                            $('#Phone').val(data.phone);
                            $('#Fax').val(data.fax);
                            $('#Email').val(data.email);
                            $('#Url').val(data.url);
                            $('#Bin').val(data.bin);
                            $('#VatregNo').val(data.vatregNo);
                            $('#SupplierTin').val(data.supplierTin);
                            $('#ExportLicenceNo').val(data.exportLicenceNo);
                            $('#AccountNo').val(data.accountNo);
                            $('#OpeningBalance').val(data.openingBalance);
                            $('#Remarks').val(data.remarks);

                            $('#LdateModifyHide').show();
                            $('#sectionBreak').show();

                            if (data.ldate) {
                                $('#creationDateSpan').text(formatDateTime(data.ldate));
                            } else {
                                $('#creationDateSpan').text('');
                            }

                            if (data.modifyDate) {
                                $('#modifyDateSpan').text(formatDateTime(data.modifyDate));
                            } else {
                                $('#modifyDateSpan').text('');
                            }
                        }
                    },
                    error: function () {
                        alert('Failed to load appointed employee data.');
                    }, complete: function () {
                        ResetBankAccountInfoForm();
                        loadBankAccountInfoTable();
                    }
                });
            });

            // #endregion

            //#region Save

            $(document).on("click", settings.saveSelector, function () {
                if (typeof validation === 'function' && !validation()) return false;
                const $form = $(settings.formSelector);
                const $saveButton = $(settings.saveSelector);

                // Prepare data
                let data = settings.haseFile ? new FormData($form[0]) : $form.serialize();
                // console.log(data);
                $saveButton.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Saving...');

                $.ajax({
                    url: saveUrl,
                    method: "POST",
                    data: data,
                    processData: !settings.haseFile,
                    contentType: settings.haseFile ? false : "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (response) {
                        if (response.isSuccess) {
                            toastr.success(response.message);
                            // Refresh table
                            loadTable();
                            loadCP();
                            ResetForm();
                            loadSNCId();
                            ResetBankAccountInfoForm();
                            loadBankAccountInfoTable();
                            // Update the Aeid field (if needed)
                            //$(".js-Aeid-code").val(response.lastCode);

                        } else {
                            toastr.warning(response.message || "Save failed");
                        }
                    },
                    error: function (xhr, status, error) {
                        toastr.error("An error occurred while saving the data.");
                        console.error("Ajax error:", status, error, xhr.responseText);
                    },
                    complete: function () {
                        $saveButton.prop('disabled', false).html('Save');
                    }
                });
            });

            //#endregion

            //#region selectAllSelector deleteSelector 

            $("body").on("click", settings.selectAllSelector, function () {
                $(".checkBox").prop('checked',
                    $(this).prop('checked'));
            });

            $("body").on("click", settings.deleteSelector, function (e) {
                e.preventDefault();
                $('input:checkbox.checkBox').each(function () {
                    if ($(this).prop('checked')) {
                        if (!selectedItems.includes($(this).val())) {
                            selectedItems.push($(this).val());
                        }
                    }
                });

                if (selectedItems.length > 0) {
                    $(settings.deleteModal).modal("show");
                } else {
                    toastr.info("Please select at least one item.");
                }

            });

            //#endregion

            //#region Delete

            $("body").on('show.bs.modal', settings.deleteModal, function (event) {

                var source = $(event.relatedTarget);
                var id = source.data("ids");

                // Extract value from data-* attributes
                var title = source.data("title");
                title = "Are you sure want to delete these items?";
                var modal = $(this);
                $(modal).find('.title').html(title);

                $("body").on("click", settings.finalDeleteSelector, function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    // Delete
                    $.ajax({
                        url: deleteUrl,
                        method: "POST",
                        contentType: "application/json",
                        data: JSON.stringify(selectedItems),
                        success: function (response) {

                            $(modal).modal("hide");

                            if (response.success) {
                                // console.log(response);
                                loadTable();
                                ResetForm();
                                //$(".js-Aeid-code").val(response.lastCode)
                                loadSNCId();
                                toastr.success(response.message);
                            }
                            else {
                                toastr.error(response.message);
                                // console.log(response);
                            }
                        }
                    });
                });

            }).on('hide.bs.modal', function () {
                $("body").off("click", settings.finalDeleteSelector);
            });

            //#endregion

            //#region topSelector decimalSelector

            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 0 }, 500);
            });

            $("body").on("keyup", settings.decimalSelector, function () {
                var self = $(this);
                showDecimalPlaces(self.val(), self.parent().find(".input-group-text"));
            });

            //#endregion

        });

        // #region Quick add

        let loadUrl,
            target,
            reloadUrl,
            title,
            lastCode;
        // Quick add
        $("body").on("click", settings.quickAddSelector, function (e) {
            e.stopPropagation();
            e.preventDefault();
            e.stopImmediatePropagation();

            loadUrl = $(this).data("url");
            target = $(this).data("target");
            reloadUrl = $(this).data("reload-url");
            title = $(this).data("title");

            $(settings.quickAddModal + " .modal-title").html(title);
            $(settings.quickAddModal + " .modal-body").empty();

            $(settings.quickAddModal + " .modal-body").load(loadUrl, function () {
                $(settings.quickAddModal).modal("show");
                $("#header").hide();
                $(settings.quickAddModal + " .modal-body #header").hide()

                $("#left_menu").hide();
                $(settings.quickAddModal + " .modal-body #left_menu").hide()

                $("#main-content").toggleClass("collapse-main");
                $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")

                $("body").removeClass("sidebar-mini");
            })
        });

        $("body").on("click", ".js-modal-dismiss", function () {
            $("body").removeClass("sidebar-mini").addClass("sidebar-mini");

            $("#header").show();
            $(settings.quickAddModal + " .modal-body #header").show()

            $("#left_menu").show();

            $(settings.quickAddModal + " .modal-body #left_menu").show()

            $("#main-content").toggleClass("collapse-main");
            $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")


            lastCode = $(settings.quickAddModal + " #lastCode").val();

            $(settings.quickAddModal + " .modal-body").empty();
            $(settings.quickAddModal).modal("hide");

            if (title === "Contact Person Info") {
                
                if (typeof loadCP === 'function') {
                    loadCP();
                }
                return;
            }

            $(target).empty("");
            $(target).append($('<option>', {
                value: '',
                text: `--Select ${title}--`
            }));
            $.ajax({
                url: reloadUrl,
                method: "GET",
                success: function (response) {
                   // console.log(response);
                    $.each(response, function (i, item) {
                        $(target).append($('<option>', {
                            value: item.code,
                            text: item.name
                        }));
                    });

                    $(target).val(lastCode);
                }
            });
        });

        // #endregion

        //#region loadTable

        function loadTable() {
            $.get(settings.baseUrl + "/GetTableData")
                .done(html => {
                    $(".js-SupplierInformation-grid-container").html(html);
                    if ($.fn.DataTable.isDataTable("#SupplierInformation-grid")) {
                        $("#SupplierInformation-grid").DataTable().destroy();
                    }
                    setTimeout(() => {
                        $("#SupplierInformation-grid").DataTable({
                            lengthChange: true,
                            pageLength: 10,
                            lengthMenu: [
                                [10, 25, 50, -1],
                                [10, 25, 50, 'All'],
                            ],
                            order: [[1, "desc"]],
                            destroy: true,
                            paging: true,
                            searching: true,
                            responsive: true,
                            autoWidth: false,
                            columnDefs: [
                                { targets: 0, orderable: false }
                            ]
                        });
                    }, 0);
                })
                .fail((xhr, status, error) => {
                    console.error("Error loading table:", status, error, xhr.responseText);
                    toastr.error("Failed to load table data.");
                    loadForm(url);
                });
        }

        //#endregion

        //#region loadForm

        function loadForm(url) {
            return new Promise((resolve, reject) => {
                //  var employeeId = $('#EmployeeId').val();
                $.ajax({
                    url: url,
                    type: 'GET',
                    cache: false,
                    success: function (data) {
                        $(settings.formContainer).empty();
                        $(settings.formContainer).html(data);
                        // $('#EmployeeId').val(employeeId);
                        $.validator.unobtrusive.parse($(settings.formSelector));
                    },
                    error: function (error) {
                        reject(error)
                    },
                })
            })
        }

        //#endregion

        //#region EnterKeyNavigation

        function enterKeyNavigation({ formSelector = '#SupplierInformation-form', focusableSelector = 'input:visible, select:visible, textarea:visible, button:visible' } = {}) {
            const $form = $(formSelector);
            if (!$form.length) return;

            const focusNext = ($el) => {
                const $focusable = $form.find(focusableSelector).not(':disabled');
                const $next = $focusable.eq(($focusable.index($el) + 1) % $focusable.length);
                $next.data('select2') ? $next.select2('open') : $next.focus();
            };

            $form
                .off('keydown.enterNav select2:close.enterNav')
                .on('keydown.enterNav', (e) => {
                    if (e.key !== 'Enter') return; 
                    e.preventDefault(); 
                    focusNext($(e.target).closest('input, select, textarea, button'));
                })
                .on('select2:close.enterNav', 'select', (e) => focusNext($(e.target)));

            return () => $form.off('keydown.enterNav select2:close.enterNav');
        }

        // #endregion

        //#region select2

        function select2DD() {
            $('.selectpickerSup').select2({
                language: {
                    noResults: function () { return "No results found"; }
                },
                escapeMarkup: function (markup) { return markup; }
            });
        }

        // #endregion

        //#region ResetForm

        $(document).on('click', '.js-SupplierInformation-clear', function () {
            ResetForm();
            BankAccountInfoClearTable();
        });

        function ResetForm() {
            loadSNCId();
            // Reset text inputs to empty
            $('#Tc').val(0);
            $('#SupplierId').val('');
            $('#SupplierName').val('');
            $('#SupplierCode').val('');
            $('#Address').val('');
            $('#LocalOfficeAddress').val('');
            $('#City').val('');
            $('#State').val('');
            $('#ZipCode').val('');
            $('#Phone').val('');
            $('#Fax').val('');
            $('#Email').val('');
            $('#Url').val('');
            $('#Bin').val('');
            $('#VatregNo').val('');
            $('#SupplierTin').val('');
            $('#ExportLicenceNo').val('');
            $('#ContatPerson1').val('');
            $('#AccountNo').val('');
            $('#OpeningBalance').val('');
            $('#Optype').val('');
            $('#Remarks').val('');

            clearCPSelection();

            // Reset dropdowns/selects to default (empty or first option)
            $('#SupplierCategoryId').val('').trigger('change');
            $('#SupplierTypeId').val('').trigger('change');
            $('#SupplierOriginId').val('').trigger('change');
            $('#CompanyId').val('').trigger('change');
            $('#CountryId').val('').trigger('change');
            $('#SupplierBankId').val('').trigger('change');
            $('#SupplierBankBranchId').val('').trigger('change');
            $('#SalesPersonId').val('').trigger('change');
            $('#Active').val('').trigger('change');
            //$('#ContatPerson1').val('').trigger('change');
            $('#Optype').val('').trigger('change');

            // Reset date-related spans
            $('#employeeIdContainer').removeClass('d-none');
            // Hide & reset value
            $('#employeeIdContainer').addClass('d-none');

            // Hide sections

            $('#LdateModifyHide').hide();
            $('#sectionBreak').hide();

            // Reset any checkboxes (assuming similar structure to your example)
            $("#SupplierInformation-check-all").prop("checked", false);
            $('#SupplierInformation-grid input[type="checkbox"]').prop('checked', false);

        }

        //#endregion

        //#region GenerateNewId

        function loadSNCId() {
            $.ajax({
                url: "/SupplierInformation/GenerateNewId",
                type: "GET",
                dataType: "json",

                success: function (data) {
                    if (data) {
                        $('#SupplierId').val(data);
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error fetching SupplierId:", error);
                }
            })
        }

        //#endregion

        //#region validation
        function validation() {

            var sup = $('#SupplierName').val();

            if (!sup) {
                toastr.info('Enter Supplier Name');
                $('#SupplierName').trigger('focus');
                return false;
            }
            return true;
        }
        //#endregion

        //#region CP - Optimized

        let selectedCPs = [];
        let allCPs = [];

        $(document).ready(function () {
            initializeCPDD();
            loadCP();
        });

        function initializeCPDD() {
            const $button = $('#contactPersonButton');
            const $menu = $('#contactPersonMenu');

            // Button click handler - consolidated
            $button.on('click', function (e) {
                e.stopPropagation();
               // console.log("test");
                if ($menu.is(':visible')) {
                    $menu.hide();
                } else {
                    positionDropdown();
                    $menu.show();
                }
            });

            // Click outside to close
            $(document).on('click', function (e) {
                if (!$(e.target).closest('#contactPersonDropdown').length) {
                    $menu.hide();
                }
            });

            // Clear selection
            $('#clearSelection').on('click', function (e) {
                e.stopPropagation();
                clearCPSelection();
            });

            $('#selectAllCP').on('change', function (e) {
                e.stopPropagation();
                const isChecked = $(this).prop('checked');
                $('#contactPersonTableBody tr:visible .contact-checkbox').prop('checked', isChecked);
                updateSelectedCPs();
            });
        }

        function initContactPersonSearch() {
            const $searchInput = $('#cpSearch');
            if (!$searchInput.length) return;

            let inputTimer = null;

            $searchInput.off('input').on('input', function () {
                clearTimeout(inputTimer);
                inputTimer = setTimeout(() => {
                    const searchTerm = $searchInput.val().toLowerCase().trim();
                    filterContactPersonTable(searchTerm);
                }, 200);
            });
        }

        function filterContactPersonTable (searchTerm)  {
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

        function loadCP() {
            $.ajax({
                url: '/SupplierInformation/GetContactPersons',
                type: 'GET',
                success: function (response) {
                    if (response.success && response.data) {
                        allCPs = response.data; // Store for later use
                        buildCPTable();
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error loading contact persons:', error);
                }
            });
        }

        function buildCPTable() {
            data = allCPs;

            const $tbody = $('#contactPersonTableBody');

            if (!data.length) {
                $tbody.html('<tr><td colspan="5" class="text-center text-middle text-muted p-3">No contact persons found</td></tr>');
                return;
            }

            const rows = data.map(cp => {
                const isSelected = selectedCPs.some(selected => selected.cpid === cp.cpid);
                return `
            <tr>
                <td class="text-center align-middle"><input type="checkbox" class="contact-checkbox " 
                           value="${cp.cpid}" data-name="${cp.contactPersonName}"
                           ${isSelected ? 'checked' : ''}></td>
                <td>${cp.contactPersonName}</td>
                <td class="text-nowrap">${cp.designation || ''}</td>
                <td class="text-center">${cp.phone || ''}</td>
                <td>${cp.email || ''}</td>
            </tr>
        `;
            }).join('');

            $tbody.html(rows);

            $tbody.off('change', '.contact-checkbox').on('change', '.contact-checkbox', function () {
                updateSelectedCPs();
                updateSelectAllCheckbox();
            });

            initContactPersonSearch();
            updateSelectAllCheckbox();
        }

        function updateSelectedCPs() {
            selectedCPs = $('.contact-checkbox:checked').map(function () {
                return { cpid: $(this).val(), name: $(this).data('name') };
            }).get();

            updateCPDisplay();
            updateHiddenInput();
        };

        function updateSelectAllCheckbox ()  {
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

        function updateCPDisplay ()  {
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

        function updateCPDisplay() {
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
        }

        function updateHiddenInput() {
            const cpids = selectedCPs.map(cp => cp.cpid).join(',');
            $('#ContatPerson1').val(cpids);
        }

        function clearCPSelection() {
            $('.contact-checkbox').prop('checked', false);
            selectedCPs = [];
            updateCPDisplay();
            updateHiddenInput();
            updateSelectAllCheckbox();
        }

        function setSelectedCP(cpids) {
            if (!cpids) {
                clearCPSelection();
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

            updateCPDisplay();
            updateHiddenInput();
            updateSelectAllCheckbox();
        }

        const positionDropdown = () => {
            const $btn = $('#contactPersonButton');
            const $menu = $('#contactPersonMenu');
            const top = $btn.offset().top - $(window).scrollTop();
            const below = $(window).height() - top - $btn.outerHeight();
            const showAbove = below < 300 && top > 300;

            $menu.toggleClass('show-above', showAbove)
                 .toggleClass('show-below', !showAbove);
        };

        //#endregion

        //#region Bank AccountInfoValue

        function BankAccountInfoValue() {
            const BankAccountInfoForm = {
                'Sbaid': $('#Sbaid').val() || 0,
                'SupplierId': $('#SupplierId').val(),
                'BankId': $('#SupplierBankId').val(),
                'BankBranchId': $('#SupplierBankBranchId').val(),
                'AccountName': $('#AccountNo').val(),
            }
            return BankAccountInfoForm;
        }
        function ResetBankAccountInfoForm() {
            $('#Sbaid').val(0);              
            //$('#SupplierId').val('').trigger('change');       
            $('#SupplierBankId').val('').trigger('change'); 
            $('#SupplierBankBranchId').val('').trigger('change');
            $('#AccountNo').val('');    
        }

        $(document).on('click', "#BankAccountInfoSaveBtn", function () {
            var infoValue = BankAccountInfoValue();
            console.log(infoValue);
            $.ajax({
                url: "/SupplierInformation/BankAccountInfoSaveEdit",
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(infoValue),
                success: function (res) {
                    console.log(res);
                    ResetBankAccountInfoForm();
                    loadBankAccountInfoTable();
                    $('.SupplierBankAccountTemp-grid-container').show();
                },
                error: function (e) {
                    console.log(e);
                }
            });
        })

        //#endregion

        //#region loadTable

        function loadBankAccountInfoTable() {
            $.get(settings.baseUrl + "/GetTableBankAccountInfoData")
                .done(data => {
                    if ($.fn.DataTable.isDataTable("#SupplierBankAccountTemp-grid")) {
                        $("#SupplierBankAccountTemp-grid").DataTable().destroy();
                    }
                    if (data.length == 0) {
                        $('.SupplierBankAccountTemp-grid-container').hide();
                    } else {
                        $('.SupplierBankAccountTemp-grid-container').show();
                    }
                   // console.log(data);
                    $("#SupplierBankAccountTemp-grid").DataTable({
                        data: data,

                        columns: [
                            {
                                data: "sbaid",
                                render: function (data) {
                                    return `<a type='button' class="btn js-SupplierBankAccountTemp-edit" style="text-decoration: underline; color: rebeccapurple;" data-id="${data}" title="Edit">${data}</a>`;
                                }
                            },
                            { data: "bankName" },
                            { data: "bankBranchName" },
                            { data: "accountName" }
                        ],
                        lengthChange: true,
                        pageLength: 10,
                        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, 'All']],
                        order: [[1, "desc"]],
                        destroy: true,
                        paging: true,
                        searching: true,
                        responsive: true,
                        autoWidth: false,
                        columnDefs: [
                            { targets: 0, orderable: false }
                        ]
                    });
                })
                .fail((xhr, status, error) => {
                    console.error("Error loading table:", status, error, xhr.responseText);
                    toastr.error("Failed to load table data.");
                });
        }

        //#endregion

        //#region SupplierBankAccountTemp

        $(document).on('click', '.js-SupplierBankAccountTemp-edit', function () {
            let table = $("#SupplierBankAccountTemp-grid").DataTable();
            let row = table.row($(this).closest('tr'));
            const rowValue = row.data();
            console.log(rowValue);
            $('#Sbaid').val(rowValue.sbaid);
            //$('#SupplierId').val(rowValue.);
            $('#SupplierBankId').val(rowValue.bankId).trigger('change');
            $('#SupplierBankBranchId').val(rowValue.bankBranchId).trigger('change');
            $('#AccountNo').val(rowValue.accountName); 
        });

        $(document).on('click', '#BankAccountInfoDeleteBtn', function () {
            var sbaid = $("#Sbaid").val();
            $.ajax({
                url: "/SupplierInformation/BankAccountInfoDelete",
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(sbaid),
                success: function (res) {
                    console.log(res);
                    ResetBankAccountInfoForm();
                    loadBankAccountInfoTable();
                },
                error: function (e) {
                    console.log(e);
                }
            });
        })
        $(document).on('click', '#BankAccountInfoRefreshBtn', function () {
            ResetBankAccountInfoForm();
        })

        function BankAccountInfoClearTable() {
            $.ajax({
                url: "/SupplierInformation/BankAccountInfoClearTableTemp",
                type: "GET",
                success: function (res) {
                    console.log(res);
                    ResetBankAccountInfoForm();
                    loadBankAccountInfoTable();
                },
                error: function (e) {
                    console.log(e);
                }
            });
        }

        //#endregion

    }
}(jQuery))
