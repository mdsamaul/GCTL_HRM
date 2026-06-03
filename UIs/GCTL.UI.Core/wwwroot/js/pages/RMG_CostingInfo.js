(function ($) {
    $.RMG_CostingInfo = function (options) {
        var commonName = $.extend({
            baseUrl: "/RMG_CostingInfo",
        }, options);

        var allFilterData = {
            buyers: [],
            jobNos: [],
            styles: [],
            masterPOs: [],
            purchaseOrders: []
        };

        function showToast(iconType, message) {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true,
            });
            Toast.fire({ icon: iconType, title: message });
        }
        function showLoader() {
            if ($('#pageLoader').length === 0) {
                $('body').append(`
            <div id="pageLoader" style="
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0,0,0,0.5);
                z-index: 9999;
                display: flex;
                justify-content: center;
                align-items: center;
            ">
                <div class="spinner-border text-light" role="status" style="width: 3rem; height: 3rem;">
                    <span class="sr-only">Loading...</span>
                </div>
            </div>
            `);
            } else {
                $('#pageLoader').show();
            }
        }

        function hideLoader() {
            $('#pageLoader').fadeOut(300);
        }
        function populateDropdown(selector, data, keepValue = true) {
            const $dropdown = $(selector);
            const currentVal = keepValue ? $dropdown.val() : null;

            $dropdown.empty().append('<option value="">-- Select --</option>');

            if (data && data.length > 0) {
                $.each(data, function (index, item) {
                    $dropdown.append($('<option>', {
                        value: item.id,
                        text: item.name
                    }));
                });

                if (currentVal && $dropdown.find(`option[value="${currentVal}"]`).length > 0) {
                    $dropdown.val(currentVal).trigger('change.select2');
                } else {
                    $dropdown.val('').trigger('change.select2');
                }
            } else {
                $dropdown.trigger('change.select2');
            }
        }

        function getCurrentFilters() {
            return {
                BuyerId: $('#ddlBuyer').val() || null,
                JobNo: $('#ddlJobNo').val() || null,
                StyleId: $('#ddlStyle').val() || null,
                MPO: $('#ddlMasterPO').val() || null,
                PurchaseOrder: $('#ddlPO').val() || null
            };
        }

        function loadInitialFilterOptions() {
            $.ajax({
                url: commonName.baseUrl + '/GetFilterOptions',
                type: 'GET',
                dataType: 'json',
                beforeSend: function () {
                    $('#loadingSpinner').removeClass('d-none');
                },
                success: function (data) {
                    console.log(data)
                    allFilterData.buyers = data.buyers || [];
                    allFilterData.jobNos = data.jobNos || [];
                    allFilterData.styles = data.styles || [];
                    allFilterData.masterPOs = data.masterPOs || [];
                    allFilterData.purchaseOrders = data.purchaseOrders || [];

                    populateDropdown('#ddlBuyer', allFilterData.buyers, false);
                    populateDropdown('#ddlJobNo', allFilterData.jobNos, false);
                    populateDropdown('#ddlStyle', allFilterData.styles, false);
                    populateDropdown('#ddlMasterPO', allFilterData.masterPOs, false);
                    populateDropdown('#ddlPO', allFilterData.purchaseOrders, false);
                },
                error: function (xhr, status, error) {
                    showToast('error', 'Failed to load filter options.');
                },
                complete: function () {
                    $('#loadingSpinner').addClass('d-none');
                }
            });
        }

        function loadFilteredOptions(filterData) {
            $.ajax({
                url: commonName.baseUrl + '/GetFilterOptions',
                type: 'GET',
                data: filterData,
                dataType: 'json',
                success: function (data) {


                    // Update only dependent dropdowns based on what's selected
                    if (filterData.BuyerId && !filterData.JobNo) {
                        populateDropdown('#ddlJobNo', data.jobNos);
                        populateDropdown('#ddlStyle', data.styles);
                        populateDropdown('#ddlMasterPO', data.masterPOs);
                        populateDropdown('#ddlPO', data.purchaseOrders);
                    } else if (filterData.JobNo && !filterData.StyleId) {
                        populateDropdown('#ddlStyle', data.styles);
                        populateDropdown('#ddlMasterPO', data.masterPOs);
                        populateDropdown('#ddlPO', data.purchaseOrders);
                    } else if (filterData.StyleId && !filterData.MPO) {
                        populateDropdown('#ddlMasterPO', data.masterPOs);
                        populateDropdown('#ddlPO', data.purchaseOrders);
                    } else if (filterData.MPO && !filterData.PurchaseOrder) {
                        populateDropdown('#ddlPO', data.purchaseOrders);
                    }
                },
                error: function (xhr, status, error) {

                }
            });
        }

        function loadReport() {
            const filterData = getCurrentFilters();

            $.ajax({
                url: commonName.baseUrl + '/GetReport',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(filterData),
                dataType: 'json',
                beforeSend: function () {
                    $('#loadingSpinner').removeClass('d-none');
                    $('#tblReport tbody').html('<tr><td colspan="10" class="text-center">Loading...</td></tr>');
                },
                success: function (data) {
                    console.log(data);
                    populateForm(data);
                },
                error: function (xhr, status, error) {
                    $('#tblReport tbody').html('<tr><td colspan="10" class="text-center text-danger">Failed to load report data</td></tr>');
                    showToast('error', 'Failed to load report.');
                },
                complete: function () {
                    $('#loadingSpinner').addClass('d-none');
                }
            });
        }


        function populateForm(data) {
            // Clear input, select, radio disable 
            $('#funJobNo, #styleInput, #poInput, #productInput, #productDescription, #exportLcNo, #shipmentDate, #factorySupplier, #issuedBy').val('').prop('disabled', true);
            $('#styleWise, #poWise').prop('checked', false).prop('disabled', true);

            if (data && data.length > 0) {
                const obj = data[0];
                // Form populate  data 
                $('#funJobNo').val(obj.integraJOBNo).prop('disabled', true);
                $('#styleInput').val(obj.styleName).prop('disabled', true);
                //$('#styleInput').val(obj.styleId).prop('disabled', true);
                $('#poInput').val(obj.purchaseOrder).prop('disabled', true);
                $('#productInputId').val(obj.productId).prop('disabled', true);
                $('#productInput').val(obj.productName).prop('disabled', true);
                $('#productDescription').val(obj.pDescription).prop('disabled', true);
                $('#exportLcNo').val(obj.masterPurchaseOrder || '').prop('disabled', true);
                $('#shipmentDate').val(obj.deliveryDate ? new Date(obj.deliveryDate).toLocaleDateString('en-GB') : '').prop('disabled', true);
                $('#factorySupplier').val(obj.supplierId || '').prop('disabled', true);
                $('#issuedBy').val(obj.lUser || '').prop('disabled', true);

                // Radio buttons set 
                if (obj.stylePOWise === "Style Wise") {
                    $('#styleWise').prop('checked', true).prop('disabled', true);
                    $('#poWise').prop('disabled', true);
                } else {
                    $('#poWise').prop('checked', true).prop('disabled', true);
                    $('#styleWise').prop('disabled', true);
                }

                // Color size breakups table  append 
                appendColorSizeBreakupsToDataTable(obj.colorSizeBreakups);
            }
        }


        var colorTable;

        $(document).ready(function () {

            colorTable = $('#productColorTable').DataTable({
                searching: true,
                paging: true,
                ordering: true,
                info: true,
                lengthChange: true,
                autoWidth: false,

                language: {
                    emptyTable: "No Data Found",
                    search: "",
                    searchPlaceholder: "Search...",
                },

                dom:
                    "<'row mb-2'<'col-sm-6'l><'col-sm-6 text-end'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row mt-2'<'col-sm-6'i><'col-sm-6 text-end'p>>"
            });

        });

        function AutoIdCosting() {
            $.ajax({
                url: commonName.baseUrl + '/AutoIdCosting',
                type: 'GET',
                success: function (res) {

                    if (res) {
                        $('#costingId').val(res);
                    }
                },
                error: function (e) {

                }
            })
        }

        function LoadedViewBackData() {
            $.ajax({
                url: commonName.baseUrl + '/LoadedViewBackData',
                type: 'GET',
                success: function (res) {


                },
                error: function (e) {

                }
            })
        }



        function appendColorSizeBreakupsToDataTable(colorData) {
            // DataTable clear 
            colorTable.clear();

            if (!colorData || colorData.length === 0) {

                colorTable.row.add([
                    "No Data Found", "", "", ""
                ]).draw();

                // Colspan add  AFTER draw
                let firstCell = $('#productColorTable tbody tr td:first');
                firstCell.attr('colspan', 4).addClass("text-center fw-bold");

                //  cells remove 
                $('#productColorTable tbody tr td:not(:first)').remove();

                return;
            }

            //  data  normal rows add 
            colorData.forEach(item => {
                colorTable.row.add([
                    item.styleName || "",
                    item.colorName || "",
                    item.sizeName || "",
                    item.quantity || 0
                ]);
            });

            colorTable.draw();
        }

        $(document).ready(function () {
            // Date picker initialize 
            flatpickr("#entryDate", CalendarService.createConfig({
                altInput: true,
                altFormat: "d/m/Y",
                dateFormat: "Y-m-d",
                allowInput: true,
                defaultDate: new Date()
            }));
        });


        function bindDropdownEvents() {
            $('#ddlBuyer').on('change', function () {
                const buyerId = $(this).val();

                if (buyerId) {
                    // Buyer select  filtered options load 
                    loadFilteredOptions({ BuyerId: buyerId });
                } else {
                    // Reset to all options
                    populateDropdown('#ddlJobNo', allFilterData.jobNos, false);
                    populateDropdown('#ddlStyle', allFilterData.styles, false);
                    populateDropdown('#ddlMasterPO', allFilterData.masterPOs, false);
                    populateDropdown('#ddlPO', allFilterData.purchaseOrders, false);
                }
                loadReport();
            });

            $('#ddlJobNo').on('change', function () {
                const jobNo = $(this).val();
                const buyerId = $('#ddlBuyer').val();

                if (jobNo) {
                    // Job No select  filtered 
                    loadFilteredOptions({ BuyerId: buyerId, JobNo: jobNo });
                } else if (buyerId) {
                    loadFilteredOptions({ BuyerId: buyerId });
                } else {
                    populateDropdown('#ddlStyle', allFilterData.styles, false);
                    populateDropdown('#ddlMasterPO', allFilterData.masterPOs, false);
                    populateDropdown('#ddlPO', allFilterData.purchaseOrders, false);
                }
                loadReport();
            });

            $('#ddlStyle').on('change', function () {
                const styleId = $(this).val();
                const buyerId = $('#ddlBuyer').val();
                const jobNo = $('#ddlJobNo').val();

                if (styleId) {
                    loadFilteredOptions({ BuyerId: buyerId, JobNo: jobNo, StyleId: styleId });
                } else if (jobNo) {
                    loadFilteredOptions({ BuyerId: buyerId, JobNo: jobNo });
                } else if (buyerId) {
                    loadFilteredOptions({ BuyerId: buyerId });
                } else {
                    populateDropdown('#ddlMasterPO', allFilterData.masterPOs, false);
                    populateDropdown('#ddlPO', allFilterData.purchaseOrders, false);
                }
                loadReport();
            });

            $('#ddlMasterPO').on('change', function () {
                const mpo = $(this).val();
                const buyerId = $('#ddlBuyer').val();
                const jobNo = $('#ddlJobNo').val();
                const styleId = $('#ddlStyle').val();

                if (mpo) {
                    loadFilteredOptions({ BuyerId: buyerId, JobNo: jobNo, StyleId: styleId, MPO: mpo });
                } else if (styleId) {
                    loadFilteredOptions({ BuyerId: buyerId, JobNo: jobNo, StyleId: styleId });
                } else if (jobNo) {
                    loadFilteredOptions({ BuyerId: buyerId, JobNo: jobNo });
                } else if (buyerId) {
                    loadFilteredOptions({ BuyerId: buyerId });
                } else {
                    populateDropdown('#ddlPO', allFilterData.purchaseOrders, false);
                }
                loadReport();
            });

            $('#ddlPO').on('change', function () {
                loadReport();
            });
        }


        // ========== GLOBAL VARIABLES ==========
        let costingId = '';
        const MIN_ROWS = 10;
        let itemList = [];
        let colorList = [];
        let supplierList = [];
        let unitList = [];
        let currencyList = [];
        let responsibleList = [];

        $(document).ready(function () {
            costingId = $('#CostingId').val();
            loadDropdownData();
        });

        // ========== DROPDOWN DATA LOAD ==========
        function loadDropdownData() {
            $.ajax({
                url: commonName.baseUrl + '/LoadedViewBackData',
                method: 'GET',
                success: function (response) {
                    // Dropdown data globally store 
                    itemList = response.itemList || [];
                    colorList = response.colorList || [];
                    supplierList = response.supplierList || [];
                    unitList = response.unitList || [];
                    currencyList = response.currencyList || [];

                    //  costing details load 
                    loadCostingDetails();
                },
                error: function (error) {

                    alert("Dropdown data can't load!");
                }
            });
        }

        // ========== COSTING DETAILS LOAD ==========
        function loadCostingDetails() {

            let costingId = $("#costingId").val();
            $.ajax({
                url: commonName.baseUrl + '/GetCostingDetails',
                method: 'GET',
                data: { costingId: costingId, clearTemp: true },
                success: function (response) {
                    if (response.success) {
                        //  rows remove 
                        $('#dinamciDataAppend .data-row').remove();

                        if (response.data && response.data.length > 0) {
                            //  item  row add 
                            response.data.forEach(function (item) {
                                addRowFromData(item);
                            });
                        } else {
                            ensureMinimumRows();
                        }

                        // Select2 initialize  totals calculate 
                        initializeSelect2();
                        //calculateAllTotals();
                        calculateSummaryTotals();
                    } else {
                        alert('Error: ' + response.message);
                        ensureMinimumRows();
                    }
                },
                error: function (error) {
                    alert("Data can't load!");
                    ensureMinimumRows();
                }
            });
        }

        // ========== MINIMUM 10 ROWS ENSURE  ==========
        function ensureMinimumRows() {
            var currentRowCount = $('#dinamciDataAppend .data-row').length;
            var rowsToAdd = MIN_ROWS - currentRowCount;

            for (var i = 0; i < rowsToAdd; i++) {
                addEmptyRow();
            }
            updateRowNumbers();
        }

        // Responsible list define 
        responsibleList = [
            {
                "id": "BKK",
                "name": "BKK"
            },
            {
                "id": "FF",
                "name": "FF"
            },
            {
                "id": "THB",
                "name": "THB"
            }
        ]

        // ========== ROW HTML CREATE ==========
        function createRowHtml(item) {
            item = item || {};
            var slno = item.slno || getNextRowNumber();


            // Dropdown options build 
            var itemOptions = '<option value="">Select Item</option>';
            itemList.forEach(function (opt) {
                var selected = item.itemId && item.itemId == opt.id ? 'selected' : '';
                itemOptions += `<option value="${opt.id}" ${selected}>${opt.name}</option>`;
            });

            var colorOptions = '<option value="">Select Color</option>';
            colorList.forEach(function (opt) {
                var selected = item.colorId && item.colorId == opt.id ? 'selected' : '';
                colorOptions += `<option value="${opt.id}" ${selected}>${opt.name}</option>`;
            });

            var supplierOptions = '<option value="">Select Supplier</option>';
            supplierList.forEach(function (opt) {
                var selected = item.supplierId && item.supplierId == opt.id ? 'selected' : '';
                supplierOptions += `<option value="${opt.id}" ${selected}>${opt.name}</option>`;
            });

            var unitOptions = '<option value="">Select Unit</option>';
            unitList.forEach(function (opt) {
                var selected = item.totalQuantityUnit && item.totalQuantityUnit == opt.id ? 'selected' : '';
                unitOptions += `<option value="${opt.id}" ${selected}>${opt.name}</option>`;
            });

            var currencyOptions = '<option value="">Select Currency</option>';
            currencyList.forEach(function (opt) {
                var selected = item.totalPriceCurrencyId && item.totalPriceCurrencyId == opt.id ? 'selected' : '';
                currencyOptions += `<option value="${opt.id}" ${selected}>${opt.name}</option>`;
            });

            var responsibleOptions = '<option value="">Select Responsible</option>';
            responsibleList.forEach(function (opt) {
                var selected = item.responsibleBy && item.responsibleBy == opt.id ? 'selected' : '';
                responsibleOptions += `<option value="${opt.id}" ${selected}>${opt.name}</option>`;
            });

            // ========== RESPONSIBLE BY  AMOUNT  CURRENCY SET  ==========
            var displayAmountSH = '';
            var displayAmountBD = '';
            var displayAmountTH = '';
            var selectedCurrency = '';

            // ResponsibleBy  corresponding amount  currency set 
            if (item.responsibleBy === "BKK") {
                displayAmountSH = item.totalAmountShhkg || '';
                selectedCurrency = '002'; // USD currency ID
            } else if (item.responsibleBy === "FF") {
                displayAmountBD = item.totalAmountBdt || '';
                selectedCurrency = '001'; // BDT currency ID
            } else if (item.responsibleBy === "THB") {
                displayAmountTH = item.totalAmountThb || '';
                selectedCurrency = '003'; // EUR currency ID
            }

            //  currency select    override 
            if (selectedCurrency && !item.totalPriceCurrencyId) {
                currencyOptions = '<option value="">Select Currency</option>';
                currencyList.forEach(function (opt) {
                    var selected = selectedCurrency == opt.id ? 'selected' : '';
                    currencyOptions += `<option value="${opt.id}" ${selected}>${opt.name}</option>`;
                });
            }

            return `
    <tr class="data-row" data-id="${item.id || 0}">
        <td><span class="sl-no">${slno}</span></td>
        <td>
            <select class="form-control-sm form-control item-select select2">
                ${itemOptions}
            </select>
        </td>
        <td>
            <input type="text" class="form-control-sm form-control description" value="${item.description || ''}" placeholder="">
        </td>
        <td>
            <input type="text" class="form-control-sm form-control text-center width" value="${item.width || ''}" placeholder="">
        </td>
        <td>
            <select class="form-control-sm form-control color-select select2">
                ${colorOptions}
            </select>
        </td>
        <td>
            <select class="form-control-sm form-control supplier-select select2">
                ${supplierOptions}
            </select>
        </td>
        <td>
            <input type="text" class="form-control-sm form-control text-center po-no" value="${item.poNo || ''}" placeholder="">
        </td>
        <td>
            <input type="number" class="form-control-sm form-control text-center gar-qty" value="${item.quantity || 0}" placeholder="">
        </td>
        <td>
            <input type="number" step="0.01" class="form-control-sm form-control text-center cons-pcs" value="${item.consumption || 0}" placeholder="">
        </td>
        <td>
            <input type="number" step="0.01" class="form-control-sm form-control text-center ex-percent" value="${item.extra || 0}" placeholder="">
        </td>
        <td>
            <input type="text" class="form-control-sm form-control text-center total-qty" value="${item.totalQuantity || ''}" readonly>
        </td>
        <td>
            <select class="form-control-sm form-control unit-select select2">
                ${unitOptions}
            </select>
        </td>
        <td>
            <input type="number" step="0.01" class="form-control-sm form-control text-center unit-price" value="${item.unitPrice || 0}" placeholder="">
        </td>
        <td>
            <select class="form-control-sm form-control currency-select select2">
                ${currencyOptions}
            </select>
        </td>
        <td>
            <input type="text"
                   class="form-control-sm form-control text-center amount-sh-hkg"
                   value="${displayAmountSH}" 
                   data-value="${item.totalAmountShhkg || ''}"
                   readonly>
        </td>
        <td>
            <input type="text"
                   class="form-control-sm form-control text-center amount-bd"
                   value="${displayAmountBD}" 
                   data-value="${item.totalAmountBdt || ''}"
                   readonly>
        </td>
        <td>
            <input type="text"
                   class="form-control-sm form-control text-center amount-thb"
                   value="${displayAmountTH}" 
                   data-value="${item.totalAmountThb || ''}"
                   readonly>
        </td>
        <td>
            <select class="form-control-sm form-control responsible-select select2">
              ${responsibleOptions}
            </select>
        </td>
        <td style="white-space:nowrap;">
            <div class="d-flex justify-content-center align-items-center gap-2">
                <button type="button" class="btn btn-outline-secondary rounded-md shadow d-flex justify-content-center align-items-center add-row-btn" style="width: 25px; height:25px; font-size: 9px; line-height: 1;" title="Add Row">
                    <i class="fa fa-add"></i>
                </button>
                <button type="button" class="btn btn-outline-secondary rounded-md shadow d-flex justify-content-center align-items-center delete-row-btn" style="width: 25px; height: 25px; font-size: 9px; line-height: 1;" title="Delete Row">
                    <i class="fa fa-trash"></i>
                </button>
            </div>
        </td>
    </tr>`;
        }

        // ========== RESPONSIBLE CHANGE EVENT -  MAIN LOGIC ==========


        $(document).on("change", ".responsible-select", function () {

            var row = $(this).closest("tr");
            var responsible = $(this).val();

            var amountSH = row.find(".amount-sh-hkg");
            var amountBD = row.find(".amount-bd");
            var amountTH = row.find(".amount-thb");
            var currencySelect = row.find(".currency-select");

            // Clear all
            amountSH.val("");
            amountBD.val("");
            amountTH.val("");

            // Update based on responsible
            if (responsible === "BKK") {
                amountSH.val(amountSH.data("value"));
                amountSH.data("value", amountSH.data("value"));
                currencySelect.val("002").trigger("change");
            }
            else if (responsible === "FF") {
                amountBD.val(amountBD.data("value"));
                amountBD.data("value", amountBD.data("value"));
                currencySelect.val("001").trigger("change");
            }
            else if (responsible === "THB") {
                amountTH.val(amountTH.data("value"));
                amountTH.data("value", amountTH.data("value"));
                currencySelect.val("003").trigger("change");
            }

            // Auto save
            clearTimeout(window.autoSaveTimer);
            window.autoSaveTimer = setTimeout(function () {
                saveRow(row);
            }, 1000);
        });



        // ========== SELECT2 INITIALIZE ==========
        function initializeSelect2() {
            //$('.select2').select2({
            //    placeholder: 'Select...',
            //    allowClear: true
            //});
        }


        // ========== HELPER FUNCTIONS ==========
        function getNextRowNumber() {
            return $('#dinamciDataAppend .data-row').length + 1;
        }

        function addRowFromData(item) {
            var row = createRowHtml(item);
            insertRowBeforeSummary(row);
        }

        function addEmptyRow() {
            var rowNumber = getNextRowNumber();
            var row = createRowHtml({ slno: rowNumber });
            insertRowBeforeSummary(row);
        }

        function insertRowBeforeSummary(rowHtml) {
            // Summary row  insert 
            var $summaryRow = $('#dinamciDataAppend tr').filter(function () {
                return $(this).find('td[colspan]').length > 0;
            }).first();

            if ($summaryRow.length > 0) {
                $summaryRow.before(rowHtml);
            } else {
                $('#dinamciDataAppend').append(rowHtml);
            }
        }

        function updateRowNumbers() {
            //  rows  serial number update 
            var rowNumber = 1;
            $('#dinamciDataAppend .data-row').each(function () {
                $(this).find('.sl-no').text(rowNumber);
                rowNumber++;
            });
        }

        // ========== ADD ROW BUTTON ==========
        $(document).on('click', '.add-row-btn', function (e) {
            e.preventDefault();

            var $currentRow = $(this).closest('tr');

            //  row save  row add 
            saveRow($currentRow, function () {
                var newRow = createRowHtml({ slno: getNextRowNumber() });
                $currentRow.after(newRow);
                updateRowNumbers();
                initializeSelect2();
                //calculateAllTotals();
                calculateSummaryTotals();
            });
        });

        // ========== DELETE ROW BUTTON ==========
        $(document).on('click', '.delete-row-btn', function (e) {
            e.preventDefault();

            var $row = $(this).closest('tr');
            var rowId = $row.data('id');

            if (confirm('Are you sure you want to delete?')) {
                //  row  ID > 0, database  delete 
                if (rowId && rowId > 0) {
                    deleteRowFromDatabase(rowId, function () {
                        $row.remove();
                        updateRowNumbers();
                        //calculateAllTotals();
                        calculateSummaryTotals();
                    });
                } else {
                    //  UI  remove  (database  save )
                    $row.remove();
                    updateRowNumbers();
                    //calculateAllTotals();
                    calculateSummaryTotals();
                }
            }
        });

        // ========== EXCEL UPLOAD BUTTON ==========
        $(document).on('click', '#excelUploadBtn', function () {
            $('#excelFileInput').click();
        });

        var selectedExcelFile = null;

        // Store selected file only
        $(document).on('change', '#excelFileInput', function (e) {
            selectedExcelFile = e.target.files[0];
            if (!selectedExcelFile) return;

            $('#fileName').text(selectedExcelFile.name);
        });

        // ========== EXCEL PREVIEW BUTTON ==========
        $(document).on('click', '#excelPreviewBtn', function () {
            if (!selectedExcelFile) {
                alert("Please select an Excel file first!");
                return;
            }

            let costingId = $("#costingId").val();
            var formData = new FormData();
            formData.append('file', selectedExcelFile);
            formData.append('costingId', costingId);

            // Upload file to server
            $.ajax({
                url: commonName.baseUrl + '/UploadExcel',
                method: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                beforeSend: function () {
                    showLoader(); // ✅ correct
                },
                success: function (response) {
                    if (response.success) {
                        alert('Excel uploaded successfully!');

                        // Clear previous rows
                        $('#dinamciDataAppend .data-row').remove();

                        // Sort data by slno
                        response.data.sort(function (a, b) {
                            return (parseInt(a.slno || 0)) - (parseInt(b.slno || 0));
                        });

                        // Add rows from uploaded data
                        response.data.forEach(function (item) {
                            addRowFromData(item);
                        });

                        updateRowNumbers();
                        initializeSelect2();
                        calculateSummaryTotals();
                    } else {
                        alert('Error: ' + response.message);
                    }
                },
                error: function (error) {
                    alert('Excel upload failed!');
                },
                complete: function () {
                    hideLoader(); // ✅ hide loader after complete
                }
            });
        });

        // ========== PREVIEW MODAL SHOW  ==========
        function showPreviewModal(data) {
            var html = '<table class="table table-bordered table-sm"><thead><tr>';
            html += '<th>S/N</th><th>Material</th><th>Description</th><th>Width</th><th>Color</th><th>Supplier</th><th>P/O No.</th><th>Ord Qty</th><th>Qty/GMT</th><th>TTL QTY</th><th>Unit</th><th>Unit Price</th><th>RESPONSIBLE By</th>';
            html += '</tr></thead><tbody>';



            data.forEach(function (item) {
                html += '<tr>';
                html += '<td>' + (item.slno || '') + '</td>';
                html += '<td>' + (item.itemName || '') + '</td>';
                html += '<td>' + (item.description || '') + '</td>';
                html += '<td>' + (item.width || '') + '</td>';
                html += '<td>' + (item.colorName || '') + '</td>';
                html += '<td>' + (item.supplierName || '') + '</td>';
                html += '<td>' + (item.poNo || '') + '</td>';
                html += '<td>' + (item.quantity || 0) + '</td>';
                html += '<td>' + (item.consumption || 0) + '</td>';
                html += '<td>' + (item.totalQuantity || 0) + '</td>';
                html += '<td>' + (item.unitName || '') + '</td>';
                html += '<td>' + (item.unitPrice || 0) + '</td>';
                html += '<td>' + (item.responsibleByName || '') + '</td>';
                html += '</tr>';
            });

            html += '</tbody></table>';

            $('#previewModalBody').html(html);
            $('#previewModal').modal('show');
        }

        // ========== ROW DATA GET  ==========



        function getRowData($row) {

            return {
                id: parseInt($row.attr('data-id')) || 0,
                costingId: $("#costingId").val(),
                slno: $row.find('.sl-no').text(),
                itemId: $row.find('.item-select').val(),
                description: $row.find('.description').val(),
                width: $row.find('.width').val(),
                colorId: $row.find('.color-select').val(),
                supplierId: $row.find('.supplier-select').val(),
                poNo: $row.find('.po-no').val(),
                quantity: parseFloat($row.find('.gar-qty').val()) || 0,
                consumption: parseFloat($row.find('.cons-pcs').val()) || 0,
                extra: parseFloat($row.find('.ex-percent').val()) || 0,
                totalQuantity: parseFloat($row.find('.total-qty').val()) || 0,
                totalQuantityUnit: $row.find('.unit-select').val(),
                unitPrice: parseFloat($row.find('.unit-price').val()) || 0,
                totalPriceCurrencyId: $row.find('.currency-select').val(),
                responsibleBy: $row.find('.responsible-select').val(),
                totalAmountShhkg: parseFloat($row.find('.amount-sh-hkg').val()) || 0,
                totalAmountBdt: parseFloat($row.find('.amount-bd').val()) || 0,
                totalAmountThb: parseFloat($row.find('.amount-thb').val()) || 0

            };
        }


        // ========== ROW SAVE  ==========
        function saveRow($row, callback) {
            var rowData = getRowData($row);
            if (!rowData.itemId && !rowData.description && rowData.quantity === 0) {
                if (callback) callback();
                return;
            }

            //  URL:  row  update row
            var url = rowData.id && rowData.id > 0
                ? commonName.baseUrl + '/UpdateCostingDetail'
                : commonName.baseUrl + '/AddCostingDetail';

            $.ajax({
                url: url,
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(rowData),
                success: function (response) {
                    if (response.success) {
                        // response  ID set  tr element 

                        if (response.data && response.data.id) {
                            $row.attr('data-id', response.data.id); // HTML attribute
                            $row.data('id', response.data.id);      // jQuery data
                        }

                        // Summary calculation
                        calculateSummaryTotals();

                        if (callback) callback();
                    } else {
                        alert('Error: ' + response.message);
                    }
                },
                error: function (error) {

                    alert('Row save failed!');
                }
            });
        }

        // ========== DATABASE to ROW DELETE ==========
        function deleteRowFromDatabase(rowId, callback) {
            $.ajax({
                url: commonName.baseUrl + '/DeleteCostingDetail',
                method: 'POST',
                data: { id: rowId },
                success: function (response) {
                    if (response.success) {
                        if (callback) callback();
                    } else {
                        alert('Error: ' + response.message);
                    }
                },
                error: function (error) {

                    alert('Row delete failed!');
                }
            });
        }

        // ========== SINGLE ROW TOTAL CALCULATE ==========
        function calculateRowTotal($row) {
            // Gar Qty × Cons/Pcs × (1 + Ex%/100) = Total Qty
            var garQty = parseFloat($row.find('.gar-qty').val()) || 0;
            var consPcs = parseFloat($row.find('.cons-pcs').val()) || 0;
            var exPercent = parseFloat($row.find('.ex-percent').val()) || 0;

            var totalQty = (garQty * consPcs) * (1 + exPercent / 100);
            $row.find('.total-qty').val(totalQty.toFixed(2));

            // Total Qty × Unit Price = Amount
            var unitPrice = parseFloat($row.find('.unit-price').val()) || 0;
            var totalPrice = totalQty * unitPrice;

            // amount fields clear 
            $row.find('.amount-sh-hkg').val('');
            $row.find('.amount-bd').val('');
            $row.find('.amount-thb').val('');

            // Data-value  values store  (for later use)
            $row.find('.amount-sh-hkg').data('value', totalPrice.toFixed(2));
            $row.find('.amount-bd').data('value', totalPrice.toFixed(2));
            $row.find('.amount-thb').data('value', totalPrice.toFixed(2));

            // Currency select  corresponding amount field  value set 
            var currency = $row.find('.currency-select').val();
            var responsible = $row.find('.responsible-select').val();

            // Responsible By amount show 
            if (responsible === 'BKK') {
                $row.find('.amount-sh-hkg').val(totalPrice.toFixed(2));
            } else if (responsible === 'FF') {
                $row.find('.amount-bd').val(totalPrice.toFixed(2));
            } else if (responsible === 'THB') {
                $row.find('.amount-thb').val(totalPrice.toFixed(2));
            }
        }


        //// ========== SUMMARY TOTALS CALCULATE ==========


        function calculateSummaryTotals() {
            var costingId = $('#costingId').val();

            var data = {
                costingId: costingId,
                damagePercent: parseFloat($('#DamagePercent').val()) || 0,
                interestPercent: parseFloat($('#InterestPercent').val()) || 0,
                cmAndProfit: parseFloat($('#CmAndProfit').val()) || 0,
                handlingCharge: parseFloat($('#HandlingCharge').val()) || 0,
                productionUpchargePercent: parseFloat($('#ProductionUpcharge').val()) || 0
                //productionUpchargePercent: parseFloat($('#ProductionUpchargePercent').val()) || 0
            };

            $.ajax({
                url: commonName.baseUrl + '/CalculateSummary',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (response) {
                    if (response.success) {
                        updateSummaryFields(response.data);
                    }
                }
            });
        }


        // ========== SUMMARY FIELDS UPDATE ==========

        function updateSummaryFields(data) {
            ;

            // Sub Total
            $("#SubTotal_HKG").val(data.subTotalShhkg.toFixed(2));
            $("#SubTotal_BD").val(data.subTotalBdt.toFixed(2));
            $("#SubTotal_THB").val(data.subTotalThb.toFixed(2));

            // Sub Total (Per Gar. Qty)
            $("#SubTotalPerQty_HKG").val(data.subTotalPerGarQtyShhkg.toFixed(2));
            $("#SubTotalPerQty_BD").val(data.subTotalPerGarQtyBdt.toFixed(2));
            $("#SubTotalPerQty_THB").val(data.subTotalPerGarQtyThb.toFixed(2));

            // Damage %
            $("#Damage_HKG").val(data.damageAmountShhkg.toFixed(2));
            $("#Damage_BD").val(data.damageAmountBdt.toFixed(2));
            $("#Damage_THB").val(data.damageAmountThb.toFixed(2));

            // Interest / Overhead %
            $("#Interest_HKG").val(data.interestOverheadAmountShhkg.toFixed(2));
            $("#Interest_BD").val(data.interestOverheadAmountBdt.toFixed(2));
            $("#Interest_THB").val(data.interestOverheadAmountThb.toFixed(2));

            // Total
            $("#Total_HKG").val(data.totalShhkg.toFixed(2));
            $("#Total_BD").val(data.totalBdt.toFixed(2));
            $("#Total_THB").val(data.totalThb.toFixed(2));

            // Total Material Cost from Overseas
            $("#TotalMaterialCostOverseas").val(data.totalMaterialCostOverseas.toFixed(2));

            // Total Material Cost from Bangladesh
            $("#TotalMaterialCostBangladesh").val(data.totalMaterialCostBangladesh.toFixed(2));

            // Total Material Cost BKK + 20%
            $("#TotalMaterialCostBkk").val(data.totalMaterialCostBkk.toFixed(2));

            // CM And Profit
            $("#CmAndProfit").val(data.cmAndProfit.toFixed(2));

            // Handling Charge
            $("#HandlingCharge").val(data.handlingCharge.toFixed(2));

            // Production Upcharge
            $("#ProductionUpcharge").val(data.productionUpcharge.toFixed(2));

            // FF Price
            $("#FfPrice").val(data.ffPrice.toFixed(2));

            // Grand Total
            $("#GrandTotal").val(data.grandTotal.toFixed(2));
        }


        // ========== INPUT/SELECT CHANGE EVENT - AUTO CALCULATE  AUTO SAVE ==========

        $(document).on('change', '.data-row input, .data-row select', function () {

            var $row = $(this).closest('tr');

            // Row total calculate
            calculateRowTotal($row);

            // Summary (sub-total, total, FF price etc.)
            //calculateSummaryTotals();

            // Auto save
            clearTimeout(window.autoSaveTimer);
            window.autoSaveTimer = setTimeout(function () {
                saveRow($row);
            }, 1000);
        });


        // User input change event
        $(document).on('change', '#DamagePercent, #InterestPercent, #CmAndProfit, #HandlingCharge, #ProductionUpcharge', function () {
            calculateSummaryTotals();
        });
        // ========== ALL ROWS SAVE  ==========
        function saveAllRows() {
            var rowCount = $('#dinamciDataAppend .data-row').length;
            var savedCount = 0;

            // প্রতিটা row save 
            $('#dinamciDataAppend .data-row').each(function () {
                var $row = $(this);
                saveRow($row, function () {
                    savedCount++;
                    if (savedCount === rowCount) {
                        alert('Save success');
                    }
                });
            });
        }


        $(document).ready(function () {
            let modalInstance = null;

            // Open Modal
            $(document).on("click", ".open-modal", function () {
                const title = $(this).data("title") || "Loading...";
                const url = $(this).data("url");

                $("#commonModalTitle").text(title);
                $("#commonModalBody").html('<div class="text-center p-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');

                $.ajax({
                    url: url,
                    type: "GET",
                    success: function (response) {
                        $("#commonModalBody").html(response);

                        if (modalInstance) {
                            modalInstance.dispose();
                        }

                        const modalEl = document.getElementById('commonModal');
                        console.log("modalEl", modalEl);
                        modalInstance = new bootstrap.Modal(modalEl, {
                            backdrop: 'static',
                            keyboard: false
                        });

                        $("#header, #left_menu, .main-footer").hide();

                        $("#main-content").css({
                            "margin-left": "0",
                            "margin": "0",
                            "transition": "margin-left 0.3s ease"
                        });
                        $(".content-wrapper").css({
                            "margin-left": "0",
                            "margin": "0",
                            "transition": "margin-left 0.3s ease"
                        });
                        setTimeout(() => {
                            //console.log(modalEl);
                            // Remove ALL Select2 DOM elements
                            $('#commonModalBody').find('.select2-container').each(function () {
                                console.log('test');

                            })
                            $('#commonModalBody').find('.select2-container').remove();

                            // Destroy any remaining instances

                            $('#commonModalBody').find('select').each(function () {
                                //debugger;
                                const $select = $(this);
                                //console.log($select.closest('.modal').attr('id'));
                                // Skip if this select belongs to a nested modal
                                if ($select.closest('.modal').attr('id') !== 'commonModal') {
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

                            $('#commonModalBody').find('select').removeData('select2');
                            $('.searchable-select').select2({
                                placeholder: 'Select an option',
                                allowClear: true,
                                dropdownParent: $(`#commonModal`),
                                width: '100%',
                                language: { noResults: () => 'No results found' },
                                escapeMarkup: markup => markup
                            });
                        }, 2000);

                        $(".select2-selection__clear").css({
                            "margin": "13px 0 !important",
                        });
                        //$('.searchable-select').select2({
                        //    placeholder: 'Select an option',
                        //    dropdownParent: $(`#commonModal`),
                        //    allowClear: true,
                        //    width: '100%',
                        //    language: { noResults: () => 'No results found' },
                        //    escapeMarkup: markup => markup
                        //});
                        modalInstance.show();
                    },
                    error: function () {
                        $("#commonModalBody").html('<div class="alert alert-danger text-center">Failed to load content!</div>');
                    }
                });
            });

            $(document).on("click", ".close-common-modal", function () {
                $("#header, #left_menu, .main-footer").show();
                $("#main-content").css({
                    "margin-left": "250px",
                    "margin": "",
                    "min-height": ""
                });

                $(".select2-selection__clear").css({
                    "margin": "0px !important"
                });
                $(".content-wrapper").css({
                    "margin-left": "250px",
                    "margin": "",
                    "min-height": "",
                });

                if (modalInstance) {
                    modalInstance.hide();
                }
            });

            $('#commonModal').on('hidden.bs.modal', function () {

                $("#header, #left_menu, .main-footer").show();
                $("#main-content").css({
                    "margin-left": "250px",
                    "margin": "",
                    "min-height": ""
                });

                $('body').removeClass('modal-open');
                $('body').css('padding-right', '');
                $('.modal-backdrop').remove();
            });

        });



        //create main


        function RmgCostingInfoDto() {

            const parseDecimal = (val) => {
                const num = parseFloat(val);
                return isNaN(num) ? 0 : num;
            };

            const parseDate = (val) => {
                if (!val) return null;
                const parts = val.split('/');
                if (parts.length === 3) {
                    return parts[2] + '-' + parts[1].padStart(2, '0') + '-' + parts[0].padStart(2, '0');
                }
                return val;
            };
            const dto = {
                AutoId: parseInt($("#AutoId").val()) || 0,
                CostingId: $("#costingId").val(),
                EntryDate: parseDate($("#entryDate").val()),
                BuyerId: $("#ddlBuyer").val(),
                StyleId: $("#ddlStyle").val(),
                MasterPurchaseOrder: $("#ddlMasterPO").val(),
                PoNo: $("#poInput").val(),
                //IntegraJobNo: $("#ddlJobNo").val(),
                IntegraJobNo: $("#funJobNo").val(),
                ExportLcnoSc: $("#exportLcNo").val(),
                ShipmentDate: parseDate($("#shipmentDate").val()),
                FactorySuplier: $("#factorySupplier").val(),
                IssuedBy: $("#issuedBy").val(),
                CheckedBy: $("#checkedBy").val(),

                SubTotalAmountShhkg: parseDecimal($("#SubTotal_HKG").val()),
                SubTotalAmountBdt: parseDecimal($("#SubTotal_BD").val()),
                SubTotalAmountThb: parseDecimal($("#SubTotal_THB").val()),

                DamagePercentage: parseDecimal($("#DamagePercent").val()),
                DamageAmountShhkg: parseDecimal($("#Damage_HKG").val()),
                DamageAmountBdt: parseDecimal($("#Damage_BD").val()),
                DamageAmountThb: parseDecimal($("#Damage_THB").val()),

                InterestOverheadPercentage: parseDecimal($("#InterestPercent").val()),
                InterestOverheadShhkg: parseDecimal($("#Interest_HKG").val()),
                InterestOverheadBdt: parseDecimal($("#Interest_BD").val()),
                InterestOverheadThb: parseDecimal($("#Interest_THB").val()),

                TotalAmountShhkg: parseDecimal($("#Total_HKG").val()),
                TotalAmountBdt: parseDecimal($("#Total_BD").val()),
                TotalAmountThb: parseDecimal($("#Total_THB").val()),

                TotalMaterialCostOverseas: parseDecimal($("#TotalMaterialCostOverseas").val()),
                TotalMaterialCostBdt: parseDecimal($("#TotalMaterialCostBangladesh").val()),
                TotalMaterialCostBkk: parseDecimal($("#TotalMaterialCostBkk").val()),

                CmandProfit: parseDecimal($("#CmAndProfit").val()),
                HandlingCharge: parseDecimal($("#HandlingCharge").val()),
                ProductionUpCharge: parseDecimal($("#ProductionUpcharge").val()),
                GrandTotal: parseDecimal($("#GrandTotal").val()),
                Ffprice: parseDecimal($("#FfPrice").val())
            };

            return dto;
        }


        function resetForm() {
            $('#AutoId').val('0');
            $('#costingId').val('');
            $('#styleInput').removeClass('border-danger');
            $('#funJobNo').removeClass('border-danger');

            const entryDateInstance = document.querySelector("#entryDate")._flatpickr;
            const shipmentDateInstance = document.querySelector("#shipmentDate")._flatpickr;

            if (entryDateInstance) {
                entryDateInstance.setDate(new Date());
            }
            if (shipmentDateInstance) {
                shipmentDateInstance.clear();
            }
            $('#funJobNo,#styleInput,#poInput,#productInput , #productDescription, #exportLcNo, #factorySupplier,#issuedBy,#shipmentDate').val('');

            $('#styleWise, #poWise').prop('checked', false);

            // Clear select2 without triggering loadReport
            $('#ddlBuyer').val(null).trigger('change.select2');
            $('#ddlStyle').val(null).trigger('change.select2');
            $('#ddlMasterPO').val(null).trigger('change.select2');
            $('#ddlPO').val(null).trigger('change.select2');
            $('#ddlJobNo').val(null).trigger('change.select2');
            $('#checkedBy').val(null).trigger('change.select2');

            $('#exportLcNo, #factorySupplier, #issuedBy').val('');

            $('#SubTotal_HKG, #SubTotal_BD, #SubTotal_THB').val('0.00');
            $('#SubTotalPerQty_HKG, #SubTotalPerQty_BD, #SubTotalPerQty_THB').val('0.00');
            $('#DamagePercent, #Damage_HKG, #Damage_BD, #Damage_THB').val('0.00');
            $('#InterestPercent, #Interest_HKG, #Interest_BD, #Interest_THB').val('0.00');
            $('#Total_HKG, #Total_BD, #Total_THB').val('0.00');
            $('#TotalMaterialCostOverseas, #TotalMaterialCostBangladesh, #TotalMaterialCostBkk').val('0.00');
            $('#CmAndProfit, #HandlingCharge, #ProductionUpcharge').val('0.00');
            $('#GrandTotal, #FfPrice').val('0.00');

            $('#dinamciDataAppend .data-row').remove();
            if (colorTable) colorTable.clear().draw();

            ensureMinimumRows();
            AutoIdCosting();

            loadInitialFilterOptions();
            bindDropdownEvents();
            LoadedViewBackData();
            loadCostingGrid();


            $('html, body').animate({ scrollTop: 0 }, 500);
            //showToast('info', 'Form reset successfully');
        }

        $(document).on('click', '#js-costing-info-clear', function () {
            if (confirm('Are you sure you want to reset the form?')) {
                resetForm();
            }
        });


        //create and edit success-btn js-costing-info-save
        // Save Button Click
        $(document).on('click', ".js-costing-info-save", function () {
            var formData = RmgCostingInfoDto();
            // Optional validation
            if (!formData.CostingId || formData.CostingId.trim() === "") {
                showToast("error", "Costing ID is required.");
                $('#costingId').focus();
                return;
            }
            if (!formData.IntegraJobNo || formData.IntegraJobNo.trim() === "") {
                showToast("error", "Fun Job No. is required.");
                $('#ddlJobNo').select2('open');
                $('#funJobNo').addClass('border-danger');
                return;
            }

            if (!formData.StyleId || formData.StyleId.trim() === "") {
                showToast("error", "Style is required.");
                $('#ddlStyle').select2('open');
                $('#styleInput').addClass('border-danger');
                return;
            }


            if (!formData.EntryDate) {
                showToast("error", "Entry Date is required.");
                const entryDateInstance = document.querySelector("#entryDate")._flatpickr;
                if (entryDateInstance) {
                    entryDateInstance.open();
                }
                return;
            }


            $.ajax({
                url: "/RMG_CostingInfo/CreateUpdate",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(formData),
                beforeSend: function () {
                    showLoader();
                },
                success: function (res) {
                    if (res.isSuccess) {
                        showToast("success", res.message);
                        resetForm();
                    } else {
                        if (res.noSavePermission) {
                            showToast("error", "You have no save permission.");
                        } else if (res.noUpdatePermission) {
                            showToast("error", "You have no update permission.");
                        } else {
                            showToast("error", res.message);
                        }
                    }
                },
                error: function (e) {
                    showToast("error", "Failed to save data.");
                },
                complete: function () {
                    
                    loadCostingGrid();
                    hideLoader();
                }
            });
        });

        function loadCostingGrid() {

            if ($.fn.DataTable.isDataTable("#productIssueTable")) {
                $("#productIssueTable").DataTable().clear().destroy();
            }

            var table = $("#productIssueTable").DataTable({
                processing: true,
                serverSide: true,
                filter: true,
                orderMulti: false,
                pageLength: 10,
                ajax: {
                    url: commonName.baseUrl + "/GetAllForDataTable",
                    type: "POST",
                    dataSrc: function (json) {
                        return json.data;
                    }
                },
                columns: [
                    {
                        data: null,
                        orderable: false,
                        className: "text-center",
                        render: function (data, type, row) {
                            return `<input type="checkbox" class="row-checkbox" data-id="${row.autoId}">`;
                        }
                    },
                    {
                        data: "costingId",
                        className: "text-center",
                        render: function (data, type, row) {
                            return `<a href="javascript:void(0)" class="edit-costing" data-id="${row.autoId}">${data}</a>`;
                        }
                    },
                    {
                        data: "entryDate",
                        className: "text-center",
                        render: function (data) {
                            return moment(data).format("DD/MM/YYYY");
                        }
                    },
                    { data: "integraJobNo", className: "text-center" },
                    {
                        data: "styleName", className: "text-center"
                    },
                    { data: "masterPurchaseOrder", className: "text-center" },
                    { data: "poNo", className: "text-center" },
                    { data: "exportLcnoSc", className: "text-center" },
                    { data: "issuedBy", className: "text-center" },
                    { data: "checkedName", className: "text-center" }
                ],
                columnDefs: [
                    { width: "50px", targets: 0 }
                ],
                language: {
                    processing: '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span>'
                }
            });
        }




        $(document).ready(function () {

            loadCostingGrid();
            // Select all checkbox
            $(document).on('change', '#selectDetailsAll', function () {
                $('.row-checkbox').prop('checked', $(this).is(':checked'));
            });


            // Delete selected rows
            $(document).on('click', '#js-costing-info-delete-confirm', function () {
                var selectedIds = [];
                $('.row-checkbox:checked').each(function () {
                    selectedIds.push($(this).data('id'));
                });

                if (selectedIds.length === 0) {
                    showToast('warning', 'Please select at least one row');
                    return;
                }

                if (confirm(`Are you sure you want to delete ${selectedIds.length} record(s)?`)) {
                    deleteMultiple(selectedIds);
                }
            });

            function deleteMultiple(ids) {
                var completed = 0;
                var failed = 0;

                ids.forEach(function (id) {
                    $.ajax({
                        url: commonName.baseUrl + '/Delete',
                        type: 'POST',
                        data: { autoId: id },
                        success: function (response) {
                            if (response.success) {
                                completed++;
                            } else {
                                failed++;
                            }

                            if (completed + failed === ids.length) {
                                showToast('success', `Deleted ${completed} record(s).`);
                                $('#selectDetailsAll').prop('checked', false);
                                loadCostingGrid();
                            }
                        },
                        error: function () {
                            failed++;
                            if (completed + failed === ids.length) {
                                showToast('error', `Deleted ${completed} record(s). Failed: ${failed}`);
                                loadCostingGrid();
                            }
                        }
                    });
                });
            }

            // Edit costing - click on Costing ID
            $(document).on('click', '.edit-costing', function () {
                var autoId = $(this).data('id');

                $.ajax({
                    url: commonName.baseUrl + '/Edit',
                    type: 'GET',
                    data: { autoId: autoId },
                    beforeSend: function () {
                        showLoader();
                    },
                    success: function (response) {
                        if (response.success) {

                            populateEditForm(response.data);
                            $(".createDate").text(response.data.showCreateDate);
                            $(".updateDate").text(response.data.showModifyDate);
                            setTimeout(function () {
                                loadCostingDetailsForEdit();
                            }, 500);
                        } else {
                            showToast('error', response.message);
                        }
                    },
                    error: function () {
                        showToast('error', 'Error loading data');
                    },
                    complete: function () {
                        hideLoader();
                    }
                });
            });

            function loadCostingDetailsForEdit() {
                let costingId = $("#costingId").val();

                $.ajax({
                    url: commonName.baseUrl + '/GetCostingDetails',
                    method: 'GET',
                    data: { costingId: costingId, clearTemp: false },
                    beforeSend: function () {
                        showLoader();
                    },
                    success: function (response) {
                        if (response.success) {
                            $('#dinamciDataAppend .data-row').remove();


                            if (response.data && response.data.length > 0) {
                                response.data.sort(function (a, b) {
                                    return (parseInt(a.slno || 0)) - (parseInt(b.slno || 0));
                                });

                                response.data.forEach(function (item) {
                                    addRowFromData(item);
                                });
                            } else {
                                ensureMinimumRows();
                            }

                            updateRowNumbers();
                            initializeSelect2();
                            calculateSummaryTotals();
                        } else {
                            showToast('error', response.message);
                            ensureMinimumRows();
                        }
                    },
                    error: function (error) {

                        showToast('error', "Data can't load!");
                        ensureMinimumRows();
                    }, complete: function () {
                        hideLoader();
                    }
                });
            }

            function addRowFromData(item) {
                var row = createRowHtml(item);
                insertRowBeforeSummary(row);
            }

            function insertRowBeforeSummary(rowHtml) {
                var $summaryRow = $('#dinamciDataAppend tr').filter(function () {
                    return $(this).find('td[colspan]').length > 0;
                }).first();

                if ($summaryRow.length > 0) {
                    $summaryRow.before(rowHtml);
                } else {
                    $('#dinamciDataAppend').append(rowHtml);
                }
            }

            function populateEditForm(data) {
                $('#AutoId').val(data.autoId);
                $('#costingId').val(data.costingId);

                if (data.entryDate) {
                    $('#entryDate').val(moment(data.entryDate).format('DD/MM/YYYY'));
                }
                if (data.shipmentDate) {
                    $('#shipmentDate').val(moment(data.shipmentDate).format('DD/MM/YYYY'));
                }


                //  Buyer
                $('#ddlBuyer').val(data.buyerId).trigger('change');

                // Buyer Style trigger200ms delay
                setTimeout(function () {
                    $('#ddlJobNo').val(data.integraJobNo).trigger('change');
                }, 200);
             
                // Style MasterPO trigger  400ms delay
                setTimeout(function () {
                    $('#ddlStyle').val(data.styleId).trigger('change');
                }, 400);

                // MasterPO PO trigger  600ms delay
                setTimeout(function () {
                    $('#ddlMasterPO').val(data.masterPurchaseOrder).trigger('change');
                }, 600);

                // PO JobNo trigger  800ms delay
                setTimeout(function () {
                    $('#ddlPO').val(data.poNo).trigger('change');
                }, 800);


                $('#exportLcNo').val(data.exportLcnoSc);
                $('#factorySupplier').val(data.factorySuplier);
                $('#issuedBy').val(data.issuedBy);
                $('#checkedBy').val(data.checkedBy);

                $('#SubTotal_HKG').val(data.subTotalAmountShhkg.toFixed(2));
                $('#SubTotal_BD').val(data.subTotalAmountBdt.toFixed(2));
                $('#SubTotal_THB').val(data.subTotalAmountThb.toFixed(2));

                $('#DamagePercent').val(data.damagePercentage.toFixed(2));
                $('#Damage_HKG').val(data.damageAmountShhkg.toFixed(2));
                $('#Damage_BD').val(data.damageAmountBdt.toFixed(2));
                $('#Damage_THB').val(data.damageAmountThb.toFixed(2));

                $('#InterestPercent').val(data.interestOverheadPercentage.toFixed(2));
                $('#Interest_HKG').val(data.interestOverheadShhkg.toFixed(2));
                $('#Interest_BD').val(data.interestOverheadBdt.toFixed(2));
                $('#Interest_THB').val(data.interestOverheadThb.toFixed(2));

                $('#Total_HKG').val(data.totalAmountShhkg.toFixed(2));
                $('#Total_BD').val(data.totalAmountBdt.toFixed(2));
                $('#Total_THB').val(data.totalAmountThb.toFixed(2));

                $('#TotalMaterialCostOverseas').val(data.totalMaterialCostOverseas.toFixed(2));
                $('#TotalMaterialCostBangladesh').val(data.totalMaterialCostBdt.toFixed(2));
                $('#TotalMaterialCostBkk').val(data.totalMaterialCostBkk.toFixed(2));

                $('#CmAndProfit').val(data.cmandProfit.toFixed(2));
                $('#HandlingCharge').val(data.handlingCharge.toFixed(2));
                $('#ProductionUpcharge').val(data.productionUpCharge.toFixed(2));

                $('#GrandTotal').val(data.grandTotal.toFixed(2));
                $('#FfPrice').val(data.ffprice.toFixed(2));

                $('html, body').animate({
                    scrollTop: $('#AutoId').offset().top + 50
                }, 500);
            }

        });


        $("#downloadReport").click(function () {
            let costingId = $("#costingId").val().trim();
            let integraJobNo = $("#ddlJobNo").val().trim();
            let purchaseOrder = $("#ddlPO").val().trim();
            let productId = $("#productInputId").val().trim();
     
            if (!costingId || !integraJobNo || !purchaseOrder || !productId) {
                alert("Please enter all required fields");
                return;
            }

            $.ajax({
                url: commonName.baseUrl + "/GetCostingReport",
                type: "GET",
                data: {
                    costingId: costingId,
                    integraJobNo: integraJobNo,
                    purchaseOrder: purchaseOrder,
                    productId: productId
                },
                success: function (data) {
                    console.log(data);
                    generatePdf(data);
                },
                error: function (xhr) {
                    alert(xhr.status === 404 ? "Costing report not found" : "Error loading report");
                }
            });
        });


        function generatePdf(data) {
            const { jsPDF } = window.jspdf;

            const detailRows = (data.details || []).sort((a, b) => a.slno - b.slno);
            const cols = [30, 60, 120, 40, 45, 45, 40, 40, 35, 45, 35, 55, 55, 50];
            const tableWidth = cols.reduce((a, b) => a + b, 0);
            const margin = 40;
            const pageWidth = tableWidth + (2 * margin);
            const pageHeight = 842;

            const doc = new jsPDF('p', 'pt', [pageWidth, pageHeight]);

            const formatDate = (dateStr) => {
                if (!dateStr) return '';
                return new Date(dateStr).toLocaleDateString('en-GB');
            };

            const formatDateTime = (dateStr) => {
                if (!dateStr) return '';
                const d = new Date(dateStr);
                return d.toLocaleString('en-GB', {
                    day: '2-digit', month: '2-digit', year: 'numeric',
                    hour: '2-digit', minute: '2-digit', second: '2-digit'
                }).replace(',', '');
            };

            const formatNumber = (num) => {
                const numberValue = parseFloat(num || 0);
                if (isNaN(numberValue)) return '';
                return numberValue.toLocaleString('en-US', {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                });
            };

            const printDate = formatDateTime(new Date());

            const addHeader = (yPos) => {
                doc.setFont("times", "bold");
                doc.setFontSize(16);
                doc.text("Costing Report", pageWidth / 2, yPos, { align: "center" });
                return yPos + 20;
            };

            const addFooter = () => {
                const pageCount = doc.getNumberOfPages();
                for (let i = 1; i <= pageCount; i++) {
                    doc.setPage(i);
                    doc.setFontSize(9);
                    doc.setFont("times", "normal");
                    doc.text(`Print Date Time: ${printDate}`, margin, pageHeight - 30);
                    doc.text(`Page ${i} of ${pageCount}`, pageWidth - margin - 40, pageHeight - 30);
                    //doc.line(margin, pageHeight - 45, pageWidth - margin, pageHeight - 45);
                }
            };

            let y = margin + 10;
            y = addHeader(y);

            doc.setFontSize(10);
            doc.setFont("times", "normal");

            const left = [
                ["Costing ID", `: ${data.costingId || ''}`],
                ["Entry Date & Time", `: ${formatDateTime(data.entryDateTime)}`],
                ["Issued By", `: ${data.issuedBy || ''}`],
                ["Checked by", `: ${data.checkedName || ''}`],
                ["Remarks", `: ${data.remarks || ''}`]
            ];

            const right = [
                ["Buyer", `: ${data.buyerName || ''}`],
                ["Fun Job No.", `: ${data.funJobNo || ''}`],
                ["Style", `: ${data.styleName || ''}`],
                ["PO No.", `: ${data.poNo || ''}`],
                ["Product", `: ${data.itemName || ''}`],
                ["Product Description", `: ${data.productDescription || ''}`],
                ["Ref No./Client Ord. No.", `: ${data.refNo || ''}`],
                ["Shipment Date", `: ${formatDate(data.shipmentDate)}`]
            ];

            left.forEach(([label, value]) => {
                doc.text(label, margin, y);
                doc.text(value, margin + 120, y);
                y += 18;
            });

            y = margin + 45;
            right.forEach(([label, value]) => {
                doc.text(label, pageWidth / 2 + 20, y);
                doc.text(value, pageWidth / 2 + 130, y);
                y += 18;
            });

            y += 25;

            // Color & Size Breakup
          
            doc.setFont("times", "bold");
            doc.text("Color & Size Breckup Details :", margin, y);
            y += 20;

            const colWidthsBreakup = [50, 150, 150, 100];
            const breakupTableWidth = colWidthsBreakup.reduce((a, b) => a + b, 0);
            const tableHeaders = ["Sl No.", "Color", "Size", "Quantity"];

            // ================= HEADER ==================
            doc.setFillColor(220, 220, 220);
            doc.rect(margin, y - 12, breakupTableWidth, 20, 'F');
            doc.setDrawColor(0);
            doc.setLineWidth(0.5);
            doc.rect(margin, y - 12, breakupTableWidth, 20);

            doc.setFontSize(9);
            doc.setFont("times", "bold");

            let xPos = margin + 5;

            // ---- HEADER TEXT CENTER ALIGN EXCEPT LAST ----
            let headerX = margin;

            tableHeaders.forEach((header, i) => {

                const colWidth = colWidthsBreakup[i];
                let textX = headerX + colWidth / 2; // Center point

                // Last column keep default left-right behavior
                if (i === 3) {
                    doc.text(header, headerX + colWidth - 10, y, { align: "right" });
                } else {
                    doc.text(header, textX, y, { align: "center" });
                }

                if (i < tableHeaders.length - 1) {
                    doc.line(headerX + colWidth, y - 12, headerX + colWidth, y + 8);
                }

                headerX += colWidth;
            });

            y += 20;

            // ================= ROWS ==================
            doc.setFont("times", "normal");

            const validBreakups = (data.colorSizeBreakups || [])
                .filter(x => (x.colorName || x.color) && (x.sizeName || x.size));

            let totalQty = 0;

            validBreakups.forEach((item, idx) => {
                const colorName = item.colorName || item.color || '';
                const sizeName = item.sizeName || item.size || '';
                const qty = item.quantity || 0;
                totalQty += qty;

                doc.setFillColor(idx % 2 === 0 ? 255 : 250);
                doc.rect(margin, y - 12, breakupTableWidth, 18, 'F');
                doc.rect(margin, y - 12, breakupTableWidth, 18);

                let rowX = margin;

                // ---- Sl No (center) ----
                doc.text((idx + 1).toString(), rowX + colWidthsBreakup[0] / 2, y, { align: "center" });
                rowX += colWidthsBreakup[0];
                doc.line(rowX, y - 12, rowX, y + 6);

                // ---- Color (center) ----
                doc.text(colorName, rowX + colWidthsBreakup[1] / 2, y, { align: "center" });
                rowX += colWidthsBreakup[1];
                doc.line(rowX, y - 12, rowX, y + 6);

                // ---- Size (center) ----
                doc.text(sizeName, rowX + colWidthsBreakup[2] / 2, y, { align: "center" });
                rowX += colWidthsBreakup[2];
                doc.line(rowX, y - 12, rowX, y + 6);

                // ---- Quantity (RIGHT) ----
                doc.text(qty.toString(), rowX + colWidthsBreakup[3] - 10, y, { align: "right" });

                y += 18;
            });

            // ================= TOTAL ROW ==================
            doc.setFillColor(240, 240, 240);
            doc.rect(margin, y - 12, breakupTableWidth, 18, 'F');
            doc.rect(margin, y - 12, breakupTableWidth, 18);

            doc.setFont("times", "bold");

            // ----- MERGED CELL (Sl No + Color + Size) -----
            const mergedWidth = colWidthsBreakup[0] + colWidthsBreakup[1] + colWidthsBreakup[2];
            const mergedX = margin;

            // Draw merged cell border
            doc.rect(mergedX, y - 12, mergedWidth, 18);

            // Text aligned RIGHT inside merged cell
            doc.text("Total :", mergedX + mergedWidth - 5, y, { align: "right" });

            // ----- LAST COLUMN (Quantity) -----
            const qtyX = mergedX + mergedWidth;

            // right column border
            doc.rect(qtyX, y - 12, colWidthsBreakup[3], 18);

            // Value right aligned
            doc.text(totalQty.toString(), qtyX + colWidthsBreakup[3] - 5, y, { align: "right" });

            y += 30;

            // Main Details Table
            const drawDetailTableHeader = (yStart) => {
                doc.setFillColor(200, 200, 200);
                doc.rect(margin, yStart, tableWidth, 30, 'F');
                doc.rect(margin, yStart, tableWidth, 30);

                doc.setFontSize(8);
                doc.setFont("times", "bold");
                const headers = ["Sl No.", "Item", "Description", "Width", "Gar. Qty", "Cons./pcs.",
                    "Extra (%)", "Total", "Unit", "Unit Price", "Unit", "Amount($)-SH/HKG", "Amount($)-BD",
                    "Amount(THB)"];

                let x = margin + 2;
                headers.forEach((h, i) => {
                    const colWidth = cols[i];
                    const lines = doc.splitTextToSize(h, colWidth - 4);
                    const centerX = x + (colWidth / 2);
                    const textY = yStart + 10 + (lines.length > 1 ? 0 : 5);

                    // Draw centered text only
                    lines.forEach((line, idx) => {
                        doc.text(line, centerX, textY + (idx * 10), { align: "center" });
                    });

                    if (i < headers.length - 1) {
                        doc.line(x + colWidth, yStart, x + colWidth, yStart + 30);
                    }

                    x += colWidth;
                });

                return yStart + 30;
            };

            if (y + 30 > pageHeight - 100) {
                doc.addPage();
                y = margin + 10;
                y = addHeader(y);
            }

            y = drawDetailTableHeader(y);


            doc.setFont("times", "normal");
            detailRows.forEach((d, idx) => {
                const rowData = [
                    d.slno || '',
                    d.itemName || '',
                    d.description || '',
                    d.width || '',
                    d.quantity > 0 ? d.quantity : '',
                    d.consumption > 0 ? d.consumption.toFixed(2) : '',
                    d.extra > 0 ? d.extra + '%' : '',
                    d.total || '',
                    d.unit || '',
                    d.unitPrice > 0 ? d.unitPrice.toFixed(2) : '',
                    d.totalQuantityUnit || '',
                    d.totalAmountShhkg > 0 ? formatNumber(d.totalAmountShhkg) : '',
                    d.totalAmountBdt > 0 ? formatNumber(d.totalAmountBdt) : '',
                    d.totalAmountThb > 0 ? formatNumber(d.totalAmountThb) : ''
                ];

                const paddingTop = 10;
                const paddingBottom = 10;

                const colHeights = rowData.map((val, i) => {
                    const text = val.toString();
                    const lines = doc.splitTextToSize(text, cols[i] - 4);
                    return lines.length * 6 + paddingTop + paddingBottom;
                });
                const rowHeight = Math.max(...colHeights);

                if (y + rowHeight > pageHeight - 80) {
                    doc.addPage();
                    y = margin + 10;
                    y = addHeader(y);
                    y = drawDetailTableHeader(y);

                    doc.setFont("times", "normal");
                }

                doc.setFillColor(idx % 2 === 0 ? 255 : 252);
                doc.rect(margin, y, tableWidth, rowHeight, 'F');
                doc.rect(margin, y, tableWidth, rowHeight);

                let x = margin + 2;
                rowData.forEach((val, i) => {
                    const text = val.toString();
                    const lines = doc.splitTextToSize(text, cols[i] - 4);
                    const textHeight = lines.length * 6;
                    const textY = y + (rowHeight - textHeight) / 2 + 4;

                    //if (i === 2) {
                    //    doc.text(lines, x + 2, y + paddingTop);
                    //}

                            const centerCols = [0,3,4, 5, 6, 7, 8, 9, 10];

                            if (centerCols.includes(i)) {
                                lines.forEach((line, idxLine) => {
                                    doc.text(line, x + cols[i] / 2, textY + idxLine * 6, { align: "center" });
                                });
                            }
                    else if ([ 11, 12, 13].includes(i)) {
                        doc.text(lines, x + cols[i] - 4, textY, { align: 'right' });
                    } else {
                        doc.text(lines, x + 2, textY);
                    }

                    if (i < cols.length - 1) {
                        doc.line(x + cols[i], y, x + cols[i], y + rowHeight);
                    }
                    x += cols[i];
                });

                y += rowHeight;
            });

            // Summary Section
            if (y + 250 > pageHeight - 80) {
                doc.addPage();
                y = margin + 10;
                y = addHeader(y);
            }

            doc.setFontSize(10);
            doc.setFont("times", "normal");

            // Calculate exact column positions
            const colX = [margin];
            for (let i = 0; i < cols.length; i++) {
                colX.push(colX[i] + cols[i]);
            }

            const summaryRowHeight = 15;
            const cellPadding = 5;
            const extraLabelWidth = 2; 

            const summaryRows = [
                { label: "Sub Total:", v1: data.subTotalAmountShhkg || '0.00', v2: data.subTotalAmountBdt || '0.00', v3: data.subTotalAmountThb ||'0.00', type: 'bordered' },
                { label: "Sub Total (Per Gar. Qty):", v1: (data.subTotalAmountShhkg / data.details[0].quantity) || '0.00', v2: (data.subTotalAmountBdt / data.details[0].quantity) || '0.00', v3: (data.subTotalAmountThb / data.details[0].quantity) || '0.00', type: 'bordered' },
                { label: "Damage(%)", v1: (data.subTotalAmountShhkg / data.details[0].quantity) * (data.damagePercentage / 100)||"0.00", v2: (data.subTotalAmountBdt / data.details[0].quantity) * (data.damagePercentage / 100)||'0.00', v3: (data.subTotalAmountThb / data.details[0].quantity) * (data.damagePercentage / 100)||"0.00", type: 'bordered' },
                { label: "Interest/Overhead(%)", v1: (data.subTotalAmountShhkg / data.details[0].quantity) * (data.interestOverheadPercentage / 100)||"0.00", v2: (data.subTotalAmountBdt / data.details[0].quantity) * (data.interestOverheadPercentage / 100)||"0.00", v3: (data.subTotalAmountThb / data.details[0].quantity) * (data.interestOverheadPercentage / 100)||'0.00', type: 'bordered' },
                {
                    label: "Total:", v1: (data.subTotalAmountShhkg / data.details[0].quantity) + (data.subTotalAmountShhkg / data.details[0].quantity) * (data.damagePercentage / 100) + (data.subTotalAmountShhkg / data.details[0].quantity) * (data.interestOverheadPercentage / 100)||'0.00',
                    v2: (data.subTotalAmountBdt / data.details[0].quantity) + (data.subTotalAmountBdt / data.details[0].quantity) * (data.damagePercentage / 100) + (data.subTotalAmountBdt / data.details[0].quantity) * (data.interestOverheadPercentage / 100)||"0.00",
                    v3: (data.subTotalAmountThb / data.details[0].quantity) + (data.subTotalAmountThb / data.details[0].quantity) * (data.damagePercentage / 100) + (data.subTotalAmountThb / data.details[0].quantity) * (data.interestOverheadPercentage / 100)||'0.00', type: 'bordered-total'
                },
                { label: "Total Material Cost from Overseas:", value: data.totalMaterialCostOverseas || '0.00', unit: "USD", type: 'simple' },
                { label: "Total Material Cost from Bangladesh:", value: data.totalMaterialCostBdt || '0.00', unit: "USD", type: 'simple' },
                { label: "Total Material Cost from BKK +20%:", value: data.totalAmountThb || '0.00', unit: "USD", type: 'simple' },
                { label: "CM And Profit:", value: data.cmandProfit || '0.00', unit: "USD", type: 'simple' },
                { label: "Handling Charge:", value: data.handlingCharge || '0.00', unit: "USD", type: 'simple' },
                { label: "Production Upcharge:", value: data.productionUpCharge || '0.00', unit: "USD", type: 'simple' },
                { label: "FF Price:", value: data.ffprice || '0.00', unit: "USD", type: 'simple' },
                { label: "Total:", value: data.details[0].quantity * data.ffprice || '0.00', unit: "USD", type: 'simple-total' }
            ];

            summaryRows.forEach((row, idx) => {
                const textY = y + summaryRowHeight - cellPadding;

                if (row.label.includes("Total:")) {
                    doc.setFont("times", "bold");
                } else {
                    doc.setFont("times", "normal");
                }

                doc.setLineWidth(0.5);
                doc.setDrawColor(0);

                if (row.type === 'bordered' || row.type === 'bordered-total') {
                    // Label cell - width 
                    const labelWidth = colX[11] - colX[0] + extraLabelWidth;
                    doc.rect(colX[0], y, labelWidth, summaryRowHeight, 'S');
                    doc.text(row.label, colX[0] + labelWidth - 5, textY, { align: 'right' });

                    // Column 11 - position adjust এবং width
                    const col11X = colX[0] + labelWidth;
                    const col11W = cols[11] ;
                    doc.rect(col11X, y, col11W, summaryRowHeight, 'S');
                    doc.text(formatNumber(row.v1), col11X + col11W - 3, textY, { align: 'right' });

                    // Column 12 
                    doc.rect(colX[12]+2, y, cols[12], summaryRowHeight, 'S');
                    doc.text(formatNumber(row.v2), colX[12] + cols[12] - 3, textY, { align: 'right' });

                    // Column 13 
                    doc.rect(colX[13]+2, y, cols[13]-2, summaryRowHeight, 'S');
                    const v3Text = row.label.includes('(%)') ? parseFloat(row.v3 || 0).toFixed(2) : formatNumber(row.v3);
                    doc.text(v3Text, colX[13] + cols[13] - 3, textY, { align: 'right' });


                } else {
                    // Simple rows
                    doc.text(row.label, colX[12] - 5, textY, { align: 'right' });

                    let valueText;
                    if (row.label.includes("CM And Profit:") || row.label.includes("Handling Charge:") ||
                        row.label.includes("Production Upcharge:") || row.label.includes("FF Price:")) {
                        valueText = parseFloat(row.value || 0).toFixed(2);
                    } else {
                        valueText = formatNumber(row.value);
                    }
                    doc.rect(colX[12]+2, y, cols[12], summaryRowHeight, 'S');
                    doc.text(valueText, colX[12] + cols[12] -3, textY, { align: 'right' });

                    doc.rect(colX[13]+2, y, cols[13]-2, summaryRowHeight, 'S');
                    if (row.unit) {
                        doc.text(row.unit, colX[13] + cols[13] - 3, textY, { align: 'right' });
                    }
                }

                y += summaryRowHeight;
            });

            addFooter();
            doc.save(`Costing_Report_${data.costingId}.pdf`);
        }


        // ========== INITIALIZATION ==========
        function init() {
            loadInitialFilterOptions();
            bindDropdownEvents();

            AutoIdCosting();
            LoadedViewBackData();
        }

        init();
    };
})(jQuery);