(function ($) {
    $.patientTypes = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            CompanyMultiSelectInput: "#",
            ShortName: "#ShortName",
            supplierName: "#supplierName",
            PurchaseOrderNo: "#purchaseOrderNo",
            StationaryDepartment: "#stationaryDepartment",
            AutoId: "#Setup_TC",
            InvoiceNo: "#stationeryInvoiceNo",
            InvoiceDate: "#stationeryInvoiceDate",
            InvoiceValue: "#stationeryInvoiceValue",
            InvoiceChallanNo: "#stationeryInvoiceChallanNo",
            InvoiceChallanDate: "#stationeryInvoiceChallanDate",
            InvoicePurchaseBy: "#stationeryInvoicePurchaseBy",
            StationeryRemarks: "#stationeryRemarks",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBtn: ".stationary-btn-edit",
            PrintStationerySaveBtn: ".js-Printing-Stationery-Purchase-Entry-save",
            DeleteBtn: "#js-Printing-Stationery-Purchase-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-Printing-Stationery-Purchase-clear",
            SizeDropdown: ".sizeSelect",
            ProductModalBtn: "#productModalBtn",
            ProductPartialContainer: "#productPartialContainer",
            ProductBrandModalBtn: "#productBrandModalBtn",
            ProductBrandContainer: "#productBrandContainer",
            ProductModelBtn: "#productModelBtn",
            ProductModelCloseBtn: ".closeModelModel",
            ProductModelContainer: "#productModelContainer",
            ProductSizeCloseBtn: ".closeSizeModel",
            ProductUnitCloseBtn: ".closeUnitModel",
            UnitDropdown: ".unitOfProduct",
            ProductSizeModalBtn: "#productSizeModalBtn",
            SizeModelContainer: "#sizeModelContainer",
            ProductUnitModalBtn: "#productUnitBtn",
            ProductUnitModelContainer: "#productUnitModelContainer",
            AddmoreDetailsBtn: "#addmoreDetailsBtn",
            ProductModelDropdown: ".modelPopulateFromBrandId",
            SupplierModalBtn: "#supplierModalBtn",
            SupplierContainer: "#supplierContainer",
            SupplierListBtn: ".supplierListBtn",
            SalesSuppAddress: "#salesSuppAddress",
            ProductSelectId: ".productSelectId",
            ProductDescription: ".productDescription",
            BrandIdFromDropdown: ".brandIdFromDropdown",
            ModelPopulateFromBrandId: ".modelPopulateFromBrandId",
            UnitPriceOfProduct: ".unitPriceOfProduct",
            QtyOfProduct: ".qtyOfProduct",
            TotalPriceOfProductMulQty: ".totalPriceOfProductMulQty",
            UnitOfProduct: ".unitOfProduct",
            TotalPriceOfProductAddProductPrice: "#totalPriceOfProductAddProductPrice",
            DetailsClear: ".delete-clear-row-btn",
            StationarySupplierModalClose: "#stationarySupplierModalClose",
            ProductItemCloseBtn: "#productItemCloseBtn",
            CloseProductBrandModel: ".closeProductBrandModel",
        }, options);

        // ─── URLs ───────────────────────────────────────────────────────────
        var filterUrl = commonName.baseUrl + "/GetFilterData";
        var loadCategoryDataUrl = commonName.baseUrl + "/LoadData";
        var AutoPrintingStationeryPurchaseIdUrl = commonName.baseUrl + "/AutoPrintingStationeryPurchaseId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deletePrintingStationeryPurchase";
        var partialProductUrl = "/ItemMasterInformation/index?isPartial=true";
        var partialBrandUrl = "/Brand/Index?isPartial=true";
        var productModelUrl = "/ItemModel/Index?isPartial=true";
        var productUnitModalUrl = "/RMG_Prod_Def_UnitType/Index?isPartial=true";
        var productSizeModalUrl = "/HRM_Size/Index?isPartial=true";
        var SupplierModalUrl = "/SalesSupplier/Index?isPartial=true";
        var supplierDetailsUrl = commonName.baseUrl + "/supplierIdDetails";
        var productSelectIdDetailsUrl = commonName.baseUrl + "/productSelectIdDetails";
        var brandIdDetailsonModelUrl = commonName.baseUrl + "/brandIdDetailsonModel";
        var addMoreLoadProductUrl = commonName.baseUrl + "/addMoreLoadProduct";
        var SupplierCloseUrl = commonName.baseUrl + "/SupplierCloseList";
        var productItemCloseUrl = commonName.baseUrl + "/productItemClose";
        var CloseProductBrandListUrl = commonName.baseUrl + "/BrandListClose";
        var productModelCloseUrl = commonName.baseUrl + "/ModelListClose";
        var productSizeCloseUrl = commonName.baseUrl + "/SizeListClose";
        var productUnitCloseUrl = commonName.baseUrl + "/UnitListClose";

        // ─── dataList (module-level, always defined) ─────────────────────────
        var dataList = [];

        // ─── Helpers ─────────────────────────────────────────────────────────
        function stHeader() {
            window.addEventListener('scroll', function () {
                const header = document.getElementById('stickyHeader');
                if (!header) return;
                header.classList.toggle('scrolled', window.scrollY > 10);
            });
        }

        function showToast(iconType, message) {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 5000,
                timerProgressBar: true,
                showClass: { popup: 'swal2-show swal2-fade-in' },
                hideClass: { popup: 'swal2-hide swal2-fade-out' }
            });
            Toast.fire({ icon: iconType, title: message });
        }

        function datePiker(selector, inputDate) {
            const parsedDate = inputDate ? new Date(inputDate) : new Date();
            flatpickr(selector, CalendarService.createConfig({ defaultDate: parsedDate }));
        }

        function calculateGrandTotal() {
            let total = 0;
            $('.totalPriceOfProductMulQty').each(function () {
                total += parseFloat($(this).val()) || 0;
            });
            $('#totalPriceOfProductAddProductPrice').val(total.toFixed(2));
        }

        // ─── Select2 init ────────────────────────────────────────────────────
        $('.searchable-select').select2({ allowClear: false, width: '100%' });

        // ─── Time picker ─────────────────────────────────────────────────────
        const timePicker = flatpickr("#inlineTimePicker", {
            enableTime: true,
            noCalendar: true,
            inline: true,
            defaultDate: new Date(),
            dateFormat: "h:i:S K",
            time_24hr: false,
            enableSeconds: true,
            minuteIncrement: 1,
            secondIncrement: 1,
            onChange: function (selectedDates, dateStr) {
                document.getElementById("timePicker").value = dateStr;
            }
        });

        //$(document).ready(async function () {


        //    gcBindRemoteMultiselect("#stationeryInvoicePurchaseBy", "/GcFilters/employee", "Select Purchase By");

        //    bsms_InitializeMultiselects();
        //    bsms_BindCascade();

        //});

        $(document).ready(async function () {      
            s2_InitializeMultiselects(); 
             s2_InitSingle("#stationeryInvoicePurchaseBy", "/GcFilters/employee", "Select Purchase By", "employee");
      
             $('[data-toggle="tooltip"]').tooltip();
            
         });

        // ════════════════════════════════════════════════════════════════════
        // GLOBAL DATA CHANGED LISTENER
        // ════════════════════════════════════════════════════════════════════
        $(document).off('data:changed').on('data:changed', function (e, type) {
            switch (type) {
                case 'product': window.productDataChanged = true; break;
                case 'brand': window.brandDataChanged = true; break;
                case 'model': window.modelDataChanged = true; break;
                case 'size': window.sizeDataChanged = true; break;
                case 'unit': window.unitDataChanged = true; break;
                case 'supplier': window.supplierDataChanged = true; break;
            }
        });

        // ════════════════════════════════════════════════════════════════════
        // PRODUCT MODAL
        // ════════════════════════════════════════════════════════════════════
        $(commonName.ProductModalBtn).on('click', function () {
            window.productDataChanged = false;
            $.ajax({
                url: partialProductUrl, type: "GET",
                success: function (res) {
                    $(commonName.ProductPartialContainer).html(res);
                    if (typeof $.ItemMasterInformation == 'function') {
                        $.ItemMasterInformation({ baseUrl: '/ItemMasterInformation', isPartial: true });
                    }
                }, error: function () { }
            });
        });

        $(commonName.ProductItemCloseBtn).off('click').on('click', function () {
            if (!window.productDataChanged) return;
            window.productDataChanged = false;
            $.ajax({
                url: productItemCloseUrl, type: "GET",
                success: function (res) {
                    $(commonName.ProductSelectId).empty();
                    res.data.forEach(function (p) {
                        $(commonName.ProductSelectId).append(
                            `<option value="${p.productCode}">${p.productName}</option>`
                        );
                    });
                }, error: function () { }
            });
        });

        // ════════════════════════════════════════════════════════════════════
        // BRAND MODAL
        // ════════════════════════════════════════════════════════════════════
        $(commonName.ProductBrandModalBtn).on('click', function () {
            window.brandDataChanged = false;
            $.ajax({
                url: partialBrandUrl, type: "GET",
                success: function (res) {
                    $(commonName.ProductBrandContainer).html(res);
                    if (typeof $.HrmBrand == 'function') {
                        $.HrmBrand({ baseUrl: '/Brand', isPartial: true });
                    }
                }, error: function () { }
            });
        });

        $(commonName.CloseProductBrandModel).off('click').on('click', function () {
            if (!window.brandDataChanged) return;
            window.brandDataChanged = false;
            $.ajax({
                url: CloseProductBrandListUrl, type: "GET",
                success: function (res) {
                    $(commonName.BrandIdFromDropdown).empty();
                    res.data.forEach(function (b) {
                        $(commonName.BrandIdFromDropdown).append(
                            `<option value="${b.brandId}">${b.brandName}</option>`
                        );
                    });
                }, error: function () { }
            });
        });

        // ════════════════════════════════════════════════════════════════════
        // MODEL MODAL
        // ════════════════════════════════════════════════════════════════════
        $(commonName.ProductModelBtn).on('click', function () {
            window.modelDataChanged = false;
            $.ajax({
                url: productModelUrl, type: "GET",
                success: function (res) {
                    $(commonName.ProductModelContainer).html(res);
                    if (typeof $.ItemModel == 'function') {
                        $.ItemModel({ baseUrl: '/ItemModel', isPartial: true });
                    }
                }, error: function () { }
            });
        });

        $(commonName.ProductModelCloseBtn).off('click').on('click', function () {
            if (!window.modelDataChanged) return;
            window.modelDataChanged = false;
            $.ajax({
                url: productModelCloseUrl, type: "GET",
                success: function (res) {
                    $(commonName.ProductModelDropdown).empty();
                    res.data.forEach(function (m) {
                        $(commonName.ProductModelDropdown).append(
                            `<option value="${m.modelId}">${m.modelName}</option>`
                        );
                    });
                }, error: function () { }
            });
        });

        // ════════════════════════════════════════════════════════════════════
        // SIZE MODAL
        // ════════════════════════════════════════════════════════════════════
        $(commonName.ProductSizeModalBtn).on('click', function () {
            window.sizeDataChanged = false;
            $.ajax({
                url: productSizeModalUrl, type: "GET",
                success: function (res) {
                    $(commonName.SizeModelContainer).html(res);
                    if (typeof $.HRM_SizeJs == 'function') {
                        $.HRM_SizeJs({ baseUrl: '/HRM_Size', isPartial: true });
                    }
                }, error: function () { }
            });
        });

        $(commonName.ProductSizeCloseBtn).off('click').on('click', function () {
            if (!window.sizeDataChanged) return;
            window.sizeDataChanged = false;
            $.ajax({
                url: productSizeCloseUrl, type: "GET",
                success: function (res) {
                    $(commonName.SizeDropdown).empty();
                    res.data.forEach(function (s) {
                        $(commonName.SizeDropdown).append(
                            `<option value="${s.sizeId}">${s.sizeName}</option>`
                        );
                    });
                }, error: function () { }
            });
        });

        // ════════════════════════════════════════════════════════════════════
        // UNIT MODAL
        // ════════════════════════════════════════════════════════════════════
        $(commonName.ProductUnitModalBtn).on('click', function () {
            window.unitDataChanged = false;
            $.ajax({
                url: productUnitModalUrl, type: "GET",
                success: function (res) {
                    $(commonName.ProductUnitModelContainer).html(res);
                    if (typeof $.RmgProdDefUnitType == 'function') {
                        $.RmgProdDefUnitType({ baseUrl: '/RMG_Prod_Def_UnitType', isPartial: true });
                    }
                }, error: function () { }
            });
        });

        $(commonName.ProductUnitCloseBtn).off('click').on('click', function () {
            if (!window.unitDataChanged) return;
            window.unitDataChanged = false;
            $.ajax({
                url: productUnitCloseUrl, type: "GET",
                success: function (res) {
                    $(commonName.UnitDropdown).empty();
                    res.data.forEach(function (u) {
                        $(commonName.UnitDropdown).append(
                            `<option value="${u.unitTypId}">${u.unitTypeName}</option>`
                        );
                    });
                }, error: function () { }
            });
        });

        // ════════════════════════════════════════════════════════════════════
        // SUPPLIER MODAL
        // ════════════════════════════════════════════════════════════════════
        $(commonName.SupplierModalBtn).on('click', function () {
            window.supplierDataChanged = false;
            $.ajax({
                url: SupplierModalUrl, type: "GET",
                success: function (res) {
                    $(commonName.SupplierContainer).html(res);
                    if (typeof $.SalesSupplier == 'function') {
                        $.SalesSupplier({ baseUrl: '/SalesSupplier', isPartial: true });
                    }
                }, error: function () { }
            });
        });

        $(commonName.StationarySupplierModalClose).off('click').on('click', function () {
            if (!window.supplierDataChanged) return;
            window.supplierDataChanged = false;
            $.ajax({
                url: SupplierCloseUrl, type: "GET",
                success: function (res) {
                    if (res.data && Array.isArray(res.data)) {
                        $(commonName.SupplierListBtn).empty();
                        res.data.forEach(function (s) {
                            $(commonName.SupplierListBtn).append(
                                `<option value="${s.supplierId}">${s.supplierName}</option>`
                            );
                        });
                        if (res.data.length > 0) {
                            $(commonName.SupplierListBtn)
                                .val(res.data[0].supplierId)
                                .trigger('change');
                            $('#salesSuppAddress').val(res.data[0].supplierAddress || '');
                        }
                    }
                }, error: function () { }
            });
        });

        // ════════════════════════════════════════════════════════════════════
        // SUPPLIER DROPDOWN CHANGE
        // ════════════════════════════════════════════════════════════════════
        $(commonName.SupplierListBtn).on('change', function () {
            var supplierId = $(this).val();
            $.ajax({
                url: supplierDetailsUrl, type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(supplierId),
                success: function (res) {
                    if (res.data != null) {
                        let address = res.data.supplierAddress || '';
                        $('#salesSuppAddress').val(address);
                        $('#salesSuppAddressWrapper').attr('title', address);
                    }
                }, error: function () { }
            });
        });

        // ════════════════════════════════════════════════════════════════════
        // PRODUCT ROW — productSelectId change
        // ════════════════════════════════════════════════════════════════════
        $(document).on('change', '.productSelectId', function () {
            let productId = $(this).val();
            let $row = $(this).closest('tr');
            $.ajax({
                url: productSelectIdDetailsUrl, type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(productId),
                success: function (res) {
                    if (res.data != null) {
                        $row.find('.productDescription').val(res.data.description);
                        let brandDropdown = $row.find('.brandIdFromDropdown');
                        brandDropdown.empty().append('<option value="">Brand</option>');
                        res.data.brandList.forEach(function (brand) {
                            brandDropdown.append(
                                `<option value="${brand.brandID}">${brand.brandName}</option>`
                            );
                        });
                        $row.find('.unitPriceOfProduct').val(res.data.purchaseCost);
                        $row.find('.qtyOfProduct').val(1);
                        $row.find('.totalPriceOfProductMulQty').val(res.data.purchaseCost);
                        $row.find('.unitOfProduct').val(res.data.unitID).trigger('change');
                        $row.find('.modelPopulateFromBrandId').empty().append('<option value="">Model</option>');
                        calculateGrandTotal();
                    }
                }, error: function () { }
            });
        });

        // ─── Brand change → load models ───────────────────────────────────
        $(document).on('change', '.brandIdFromDropdown', function () {
            let brandId = $(this).val();
            let $row = $(this).closest('tr');
            $.ajax({
                url: brandIdDetailsonModelUrl, type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(brandId),
                success: function (res) {
                    if (res.data != null) {
                        let $modelDropdown = $row.find('.modelPopulateFromBrandId');
                        $modelDropdown.empty().append('<option value="">Model</option>');
                        res.data.forEach(function (model) {
                            $modelDropdown.append(
                                `<option value="${model.modelID}">${model.modelName}</option>`
                            );
                        });
                    }
                }, error: function () { }
            });
        });

        // ─── Qty change ───────────────────────────────────────────────────
        $(document).on('input', '.qtyOfProduct', function () {
            let $row = $(this).closest('tr');
            let qtyValue = parseFloat($(this).val()) || 0;
            let unitPrice = parseFloat($row.find('.unitPriceOfProduct').val()) || 0;
            let $totalPrice = $row.find('.totalPriceOfProductMulQty');

            if (qtyValue > 0) {
                $totalPrice.val((qtyValue * unitPrice).toFixed(2));
                $(this).removeClass('printingStation-input');
                $totalPrice.removeClass('printingStation-input');
                $('#printStationerySaveBtn').prop('disabled', false);
                calculateGrandTotal();
            } else {
                $(this).addClass('printingStation-input');
                $totalPrice.addClass('printingStation-input');
                $('#totalPriceOfProductAddProductPrice').addClass('printingStation-input');
                $('#printStationerySaveBtn').prop('disabled', true);
                $totalPrice.val(0);
                $('#totalPriceOfProductAddProductPrice').val(0);
                showToast("warning", "Quantity must be at least one or more.");
            }
        });

        // ════════════════════════════════════════════════════════════════════
        // dataList builder
        // ════════════════════════════════════════════════════════════════════
        function listOfProdut() {
            dataList = []; // ✅ always reset before building
            let allRows = $('table #dinamciDataAppend tr.data-row');
            allRows.each(function () {
                let $row = $(this);
                dataList.push({
                    TC: 0,
                    PurchaseReceiveNo: "",
                    ProductCode: $row.find('.productSelectId').val(),
                    Description: $row.find('.productDescription').val(),
                    BrandID: $row.find('.brandIdFromDropdown').val(),
                    ModelID: $row.find('.modelPopulateFromBrandId').val(),
                    SizeID: $row.find('.sizeSelect').val(),
                    WarrantyPeriod: $row.find('.warrantyInput').val(),
                    WarrentyTypeID: $row.find('.periodSelect').val(),
                    ReqQty: parseFloat($row.find('.qtyOfProduct').val()) || 0,
                    UnitTypID: $row.find('.unitOfProduct').val(),
                    UnitPrice: parseFloat($row.find('.unitPriceOfProduct').val()) || 0,
                    TotalPrice: parseFloat($row.find('.totalPriceOfProductMulQty').val()) || 0,
                    SLNO: 0
                });
            });
        }

        // ════════════════════════════════════════════════════════════════════
        // ADD MORE ROW
        // ════════════════════════════════════════════════════════════════════
        $(document).on('click', '#addmoreDetailsBtn', function () {
            $.ajax({
                url: addMoreLoadProductUrl, type: "GET",
                success: function (res) {
                    let productOptions = buildOptions(res.productList, 'Product');
                    let sizeOptions = buildOptions(res.sizeList, 'Size');
                    let periodOptions = buildOptions(res.periodList, 'Period');
                    let unitOptions = buildOptions(res.unitList, 'Unit');

                    let newRow = buildDataRow(productOptions, sizeOptions, periodOptions, unitOptions);
                    $('table #dinamciDataAppend tr:last').before(newRow);
                    $('.searchable-select').select2({ width: '100%' });
                    updateActionButtons();
                }
            });
        });

        // ════════════════════════════════════════════════════════════════════
        // ROW BUILDERS (no duplication)
        // ════════════════════════════════════════════════════════════════════
        function buildOptions(list, placeholder) {
            let html = `<option value="">${placeholder}</option>`;
            list.forEach(function (item) {
                html += `<option value="${item.value}">${item.text}</option>`;
            });
            return html;
        }

        function buildDataRow(productOptions, sizeOptions, periodOptions, unitOptions) {
            return `
<tr class="data-row">
    <td><select class="form-control-sm form-control searchable-select productSelectId">${productOptions}</select></td>
    <td><input type="text" class="form-control-sm form-control productDescription" placeholder="Description"/></td>
    <td><select class="form-control-sm form-control searchable-select brandIdFromDropdown"><option value="">Brand</option></select></td>
    <td><select class="form-control-sm form-control searchable-select modelPopulateFromBrandId"><option value="">Model</option></select></td>
    <td><select class="form-control-sm form-control searchable-select sizeSelect">${sizeOptions}</select></td>
    <td><input type="number" class="form-control-sm form-control warrantyInput" placeholder="Warranty"/></td>
    <td><select class="form-control-sm form-control searchable-select periodSelect">${periodOptions}</select></td>
    <td><input type="number" class="form-control-sm form-control qtyOfProduct text-center" placeholder="Qty"/></td>
    <td><select class="form-control-sm form-control searchable-select unitOfProduct">${unitOptions}</select></td>
    <td><input type="number" class="form-control-sm form-control unitPriceOfProduct text-end" value="0" readonly/></td>
    <td><input type="number" class="form-control-sm form-control totalPriceOfProductMulQty text-end" value="0" readonly/></td>
    <td>
        <div class="d-flex gap-2">
            <button class="btn btn-outline-success rounded-md d-flex justify-content-center align-items-center"
                    id="addmoreDetailsBtn" style="width:30px;height:30px;font-size:9px;">
                <i class="fas fa-plus"></i>
            </button>
            <button class="btn btn-outline-danger rounded-md d-flex justify-content-center align-items-center delete-row-btn"
                    style="width:30px;height:30px;font-size:9px;">
                <i class="fas fa-trash-alt"></i>
            </button>
        </div>
    </td>
</tr>`;
        }

        function buildTotalRow(totalAmount) {
            return `
<tr class="total-row">
    <td colspan="10"><div class="total-label">Total:</div></td>
    <td>
        <input type="number" class="form-control-sm form-control text-end"
               value="${totalAmount || 0}"
               id="totalPriceOfProductAddProductPrice" readonly/>
    </td>
    <td></td>
</tr>`;
        }

        // ════════════════════════════════════════════════════════════════════
        // appendProductRow — single default row + total row
        // ════════════════════════════════════════════════════════════════════
        function appendProductRow(resData) {
            let productOptions = buildOptions(resData.productList, 'Product');
            let sizeOptions = buildOptions(resData.sizeList, 'Size');
            let periodOptions = buildOptions(resData.periodList, 'Period');
            let unitOptions = buildOptions(resData.unitList, 'Unit');

            let $tableBody = $('#dinamciDataAppend');
            $tableBody.empty(); // ✅ temizle, sonra ekle
            $tableBody.append(buildDataRow(productOptions, sizeOptions, periodOptions, unitOptions));
            $tableBody.append(buildTotalRow(0));
            $('.searchable-select').select2({ width: '100%' });
            updateActionButtons();
        }

        // ════════════════════════════════════════════════════════════════════
        // updateActionButtons
        // ════════════════════════════════════════════════════════════════════
        function updateActionButtons() {
            const $rows = $('#dinamciDataAppend tr.data-row');

            if ($rows.length === 1) {
                $rows.first().find('td').last().html(`
                    <div class="d-flex gap-2">
                        <button class="btn btn-outline-success rounded-md d-flex justify-content-center align-items-center"
                                id="addmoreDetailsBtn" style="width:30px;height:30px;font-size:9px;">
                            <i class="fas fa-plus"></i>
                        </button>
                        <button class="btn btn-outline-danger rounded-md d-flex justify-content-center align-items-center delete-clear-row-btn"
                                style="width:30px;height:30px;font-size:9px;">
                            <i class="fa fa-eraser"></i>
                        </button>
                    </div>`);
            } else {
                $rows.each(function () {
                    $(this).find('td').last().html(`
                        <div class="d-flex gap-2">
                            <button class="btn btn-outline-success rounded-md d-flex justify-content-center align-items-center"
                                    id="addmoreDetailsBtn" style="width:30px;height:30px;font-size:9px;">
                                <i class="fas fa-plus"></i>
                            </button>
                            <button class="btn btn-outline-danger rounded-md d-flex justify-content-center align-items-center delete-row-btn"
                                    style="width:30px;height:30px;font-size:9px;">
                                <i class="fas fa-trash-alt"></i>
                            </button>
                        </div>`);
                });
            }
        }

        // ─── Delete row ───────────────────────────────────────────────────
        $(document).on('click', '.delete-row-btn', function () {
            const $tableBody = $('#dinamciDataAppend');
            const $dataRows = $tableBody.find('tr.data-row');
            let $targetRow = $(this).closest('tr');

            if ($dataRows.length === 1) {
                // ✅ শুধু clear করো, row remove করো না
                $targetRow.find('input[type=text], input[type=number], textarea').val('');
                $targetRow.find('input[type=checkbox], input[type=radio]').prop('checked', false);
                $targetRow.find('select').val(null).trigger('change');
            } else {
                $targetRow.remove();
            }

            calculateGrandTotal();
            updateActionButtons();
        });

        $(document).on('click', '.delete-clear-row-btn', function () {
            let $row = $(this).closest('tr');
            $row.find('input[type=text], input[type=number], textarea').val('');
            $row.find('input[type=checkbox], input[type=radio]').prop('checked', false);
            $row.find('select').val(null).trigger('change');
            calculateGrandTotal();
        });

        // ════════════════════════════════════════════════════════════════════
        // AutoId
        // ════════════════════════════════════════════════════════════════════
        AutoPrintingStationeryPurchaseId = function () {
            $.ajax({
                url: AutoPrintingStationeryPurchaseIdUrl, type: "GET",
                success: function (res) {
                    $(commonName.PurchaseOrderNo).val(res.data);
                }, error: function () { }
            });
        };

        // ════════════════════════════════════════════════════════════════════
        // resetForm — always resets to 1 clean row
        // ════════════════════════════════════════════════════════════════════
        resetForm = function () {
            $(commonName.AutoId).val(0);
            $(commonName.MainCompanyCode).val('');
            $(commonName.PurchaseOrderNo).val('');
            $(commonName.SupplierListBtn).val('').trigger('change');
            $(commonName.SalesSuppAddress).val('');
            $(commonName.StationaryDepartment).val('').trigger('change');
            $(commonName.InvoiceNo).val('');
            $(commonName.InvoiceValue).val('');
            $(commonName.InvoiceChallanNo).val('');
            $(commonName.InvoicePurchaseBy).val('').trigger('change');
            $(commonName.StationeryRemarks).val('');
            $(commonName.CompanyCode).val('');
            $(commonName.TotalPriceOfProductAddProductPrice).val('');
            $(commonName.CreateDate).text('');
            $(commonName.UpdateDate).text('');

            const today = new Date();
            const formattedDate = today.toISOString().split('T')[0];
            datePiker("#datePicker1", formattedDate);
            datePiker($(commonName.InvoiceDate), formattedDate);
            datePiker($(commonName.InvoiceChallanDate), formattedDate);

            if ($("#inlineTimePicker")[0]?._flatpickr) {
                $("#inlineTimePicker")[0]._flatpickr.setDate(today, true);
            }

            // ✅ dataList reset
            dataList = [];

            // ✅ table reset → 1 clean row
            $.ajax({
                url: addMoreLoadProductUrl, type: "GET",
                success: function (res) {
                    appendProductRow(res);
                }
            });
        };

        $(commonName.ClearBrn).on('click', function () {
            resetForm();
            AutoPrintingStationeryPurchaseId();
        });

        // ════════════════════════════════════════════════════════════════════
        // getFromData
        // ════════════════════════════════════════════════════════════════════
        function formatDateTimeToSql(dateStr, timeStr) {
            if (timeStr.includes('AM') || timeStr.includes('PM')) {
                timeStr = convertTo24Hour(timeStr);
            }
            const dt = new Date(`${dateStr}T${timeStr}Z`);
            const options = {
                timeZone: 'Asia/Dhaka',
                year: 'numeric', month: '2-digit', day: '2-digit',
                hour: '2-digit', minute: '2-digit', second: '2-digit',
                hour12: false,
            };
            const parts = new Intl.DateTimeFormat('en-GB', options).formatToParts(dt);
            let year, month, day, hour, minute, second;
            parts.forEach(p => {
                if (p.type === 'year') year = p.value;
                if (p.type === 'month') month = p.value;
                if (p.type === 'day') day = p.value;
                if (p.type === 'hour') hour = p.value;
                if (p.type === 'minute') minute = p.value;
                if (p.type === 'second') second = p.value;
            });
            return `${year}-${month}-${day} ${hour}:${minute}:${second}.000`;
        }

        function convertTo24Hour(timeStr) {
            const [time, modifier] = timeStr.split(' ');
            let [hours, minutes, seconds] = time.split(':');
            if (modifier === 'PM' && hours !== '12') hours = String(parseInt(hours, 10) + 12);
            if (modifier === 'AM' && hours === '12') hours = '00';
            return `${hours.padStart(2, '0')}:${minutes}:${seconds}`;
        }

        getFromData = function () {
            listOfProdut(); // ✅ dataList এখানে fill হয়
            const date = $("#datePicker1").val();
            const time = $("#inlineTimePicker").val();
            if (!date || !time || isNaN(Date.parse(`${date} ${time}`))) {
                showToast("error", "Please select a valid Receive Date and Time.");
                const fp = $("#datePicker1")[0]._flatpickr;
                if (fp) fp.open();
                $("#datePicker1").addClass("printingStation-input");
                $(commonName.PrintStationerySaveBtn).prop('disabled', true);
                return null;
            }
            $("#datePicker1").removeClass("printingStation-input");
            return {
                TC: $(commonName.AutoId).val() ? parseInt($(commonName.AutoId).val()) : 0,
                MainCompanyCode: $(commonName.MainCompanyCode).val() || null,
                PurchaseReceiveNo: $(commonName.PurchaseOrderNo).val() || null,
                SupplierID: $(commonName.SupplierListBtn).val() || null,
                ReceiveDate: new Date(formatDateTimeToSql(date, time)).toISOString(),
                DepartmentCode: $(commonName.StationaryDepartment).val() || null,
                InvoiceNo: $(commonName.InvoiceNo).val() || null,
                InvoiceDate: $(commonName.InvoiceDate).val()
                    ? new Date($(commonName.InvoiceDate).val()).toISOString() : null,
                InvoiceValue: parseFloat($(commonName.InvoiceValue).val()) || 0,
                ChallanNo: $(commonName.InvoiceChallanNo).val() || null,
                ChallanDate: $(commonName.InvoiceChallanDate).val()
                    ? new Date($(commonName.InvoiceChallanDate).val()).toISOString() : null,
                EmployeeID_ReceiveBy: $(commonName.InvoicePurchaseBy).val() || null,
                Remarks: $(commonName.StationeryRemarks).val() || null,
                TotalAmount: parseFloat($(commonName.TotalPriceOfProductAddProductPrice).val()) || 0,
                CompanyCode: $(commonName.CompanyCode).val() || null,
                ShowCreateDate: null,
                ShowModifyDate: null,
                purchaseOrderReceiveDetailsDTOs: dataList
            };
        };

        // ════════════════════════════════════════════════════════════════════
        // SAVE BUTTON — off().on() দিয়ে double-bind বন্ধ
        // ════════════════════════════════════════════════════════════════════
        $(document).off('click', commonName.PrintStationerySaveBtn)
            .on('click', commonName.PrintStationerySaveBtn, function () {
                var $btn = $(commonName.PrintStationerySaveBtn);
                if ($btn.data('submitting')) return; // ✅ double-click guard

                var fromData = getFromData();
                if (!fromData) return;

                if (!fromData.SupplierID || fromData.SupplierID.trim() === "") {
                    showToast("error", "Please select a supplier.");
                    $('.supplierListBtn').addClass('printingStation-input').select2('open');
                    $btn.prop('disabled', true);
                    return;
                }

                var details = fromData.purchaseOrderReceiveDetailsDTOs;
                if (!details || details.length === 0) {
                    showToast("error", "Please add at least one product.");
                    $btn.prop('disabled', true);
                    return;
                }

                $('.productSelectId').removeClass('printingStation-input');
                for (let i = 0; i < details.length; i++) {
                    if (!details[i].ProductCode || details[i].ProductCode.trim() === "") {
                        showToast("error", `Product selection missing in row ${i + 1}`);
                        let $sel = $('.productSelectId').eq(i).addClass('printingStation-input');
                        $sel.hasClass("select2-hidden-accessible") ? $sel.select2('open') : $sel.focus();
                        $btn.prop('disabled', true);
                        return;
                    }
                }

                // ✅ disable + flag
                $btn.prop('disabled', true).data('submitting', true);

                $.ajax({
                    url: CreateUpdateUrl, type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(fromData),
                    success: function (res) {
                        showToast(res.isSuccess ? "success" : "error", res.message);
                    },
                    error: function () {
                        showToast("error", "Something went wrong.");
                    },
                    complete: function () {
                        // ✅ re-enable
                        $btn.prop('disabled', false).data('submitting', false);
                        resetForm();
                        AutoPrintingStationeryPurchaseId();
                        loadCategoryData();
                        // dataList & table reset হয় resetForm() এর ভেতরে
                    }
                });
            });

        // ════════════════════════════════════════════════════════════════════
        // Input validation helpers
        // ════════════════════════════════════════════════════════════════════
        $(document).on('change', '#datePicker1', function () {
            $(commonName.PrintStationerySaveBtn).prop('disabled', false);
        });
        $(document).on('change', commonName.SupplierListBtn, function () {
            $('.supplierListBtn').removeClass('printingStation-input');
            $(commonName.PrintStationerySaveBtn).prop('disabled', false);
        });
        $(document).on('change', commonName.ProductSelectId, function () {
            $(commonName.PrintStationerySaveBtn).prop('disabled', false);
        });

        // ════════════════════════════════════════════════════════════════════
        // EDIT
        // ════════════════════════════════════════════════════════════════════
        $(document).on('click', commonName.EditBtn, function () {
            let id = $(this).data('id');
            $.ajax({
                url: `${PopulatedDataForUpdateUrl}?id=${id}`, type: "GET",
                success: function (res) {
                    selectedIds = [res.result.tc + ''];

                    $(commonName.AutoId).val(res.result.tc);
                    $(commonName.MainCompanyCode).val(res.result.mainCompanyCode);
                    $(commonName.PurchaseOrderNo).val(res.result.purchaseReceiveNo);
                    $(commonName.SupplierListBtn).val(res.result.supplierID).trigger("change");
                    $(commonName.StationaryDepartment).val(res.result.departmentCode);
                    $(commonName.InvoiceNo).val(res.result.invoiceNo);
                    $(commonName.CreateDate).text(res.result.showCreateDate);
                    $(commonName.UpdateDate).text(res.result.showModifyDate);
                    $(commonName.InvoiceValue).val(res.result.invoiceValue);
                    $(commonName.InvoiceChallanNo).val(res.result.challanNo);
                    $(commonName.InvoicePurchaseBy).val(res.result.employeeID_ReceiveBy).trigger('change');
                    $(commonName.StationeryRemarks).val(res.result.remarks);
                    $(commonName.CompanyCode).val(res.result.companyCode);
                    $(commonName.TotalPriceOfProductAddProductPrice).val(res.result.totalAmount);

                    if (res.result.receiveDate) {
                        const parts = res.result.receiveDate.split("T");
                        datePiker("#datePicker1", parts[0]);
                        if ($("#inlineTimePicker")[0]._flatpickr) {
                            const [h, m] = parts[1].split(":");
                            $("#inlineTimePicker")[0]._flatpickr.setDate(`${h}:${m}`, true);
                        }
                    }
                    if (res.result.invoiceDate) {
                        datePiker($(commonName.InvoiceDate), res.result.invoiceDate.split("T")[0]);
                    }
                    if (res.result.challanDate) {
                        datePiker($(commonName.InvoiceChallanDate), res.result.challanDate.split("T")[0]);
                    }

                    let $body = $('table #dinamciDataAppend');
                    $body.empty();

                    let details = res.result.purchaseOrderReceiveDetailsDTOs;
                    if (!details || details.length === 0) {
                        $.ajax({
                            url: addMoreLoadProductUrl, type: "GET",
                            success: function (r) { appendProductRow(r); }
                        });
                        return;
                    }

                    details.forEach(function (item) {
                        $body.append(`
<tr class="data-row">
    <td><select class="form-control-sm form-control searchable-select productSelectId">
        <option value="${item.productCode}" selected>${item.productName}</option>
    </select></td>
    <td><input type="text" class="form-control-sm form-control productDescription" value="${item.description || ''}"/></td>
    <td><select class="form-control-sm form-control searchable-select brandIdFromDropdown">
        <option value="${item.brandID}" selected>${item.brandName}</option>
    </select></td>
    <td><select class="form-control-sm form-control searchable-select modelPopulateFromBrandId">
        <option value="${item.modelID}" selected>${item.modelName}</option>
    </select></td>
    <td><select class="form-control-sm form-control searchable-select sizeSelect">
        <option value="${item.sizeID}" selected>${item.sizeName}</option>
    </select></td>
    <td><input type="number" class="form-control-sm form-control warrantyInput" value="${item.warrantyPeriod || ''}"/></td>
    <td><select class="form-control-sm form-control searchable-select periodSelect">
        <option value="${item.warrentyTypeID}" selected>${item.warrantyPeriodName}</option>
    </select></td>
    <td><input type="number" class="form-control-sm form-control qtyOfProduct text-center" value="${item.reqQty || 0}"/></td>
    <td><select class="form-control-sm form-control searchable-select unitOfProduct">
        <option value="${item.unitTypID}" selected>${item.unitTypName}</option>
    </select></td>
    <td><input type="number" class="form-control-sm form-control unitPriceOfProduct text-end" value="${item.unitPrice || 0}" readonly/></td>
    <td><input type="number" class="form-control-sm form-control totalPriceOfProductMulQty text-end" value="${item.totalPrice || 0}" readonly/></td>
    <td>
        <div class="d-flex gap-2">
            <button class="btn btn-outline-success rounded-md d-flex justify-content-center align-items-center"
                    id="addmoreDetailsBtn" style="width:30px;height:30px;font-size:9px;">
                <i class="fas fa-plus"></i>
            </button>
            <button class="btn btn-outline-danger rounded-md d-flex justify-content-center align-items-center delete-row-btn"
                    style="width:30px;height:30px;font-size:9px;">
                <i class="fas fa-trash-alt"></i>
            </button>
        </div>
    </td>
</tr>`);
                    });

                    $body.append(buildTotalRow(res.result.totalAmount));
                    $('.searchable-select').select2({ width: '100%' });
                    updateActionButtons();
                },
                error: function () { showToast("error", "Failed to load data"); }
            });
        });

        // ════════════════════════════════════════════════════════════════════
        // CHECKBOX / SELECT ALL / DELETE
        // ════════════════════════════════════════════════════════════════════
        let selectedIds = [];

        $(document).on('change', commonName.RowCheckbox, function () {
            const id = $(this).val();
            if ($(this).is(':checked')) {
                if (!selectedIds.includes(id)) selectedIds.push(id);
            } else {
                selectedIds = selectedIds.filter(item => item != id);
            }
            let total = $(commonName.RowCheckbox).length;
            let checked = $(commonName.RowCheckbox + ":checked").length;
            $('#selectAll').prop('checked', total === checked);
        });

        $(document).on('change', commonName.SelectedAll, function () {
            $(commonName.RowCheckbox).prop('checked', $(this).is(':checked')).trigger('change');
        });

        $(document).on('click', commonName.DeleteBtn, function () {
            $.ajax({
                url: deleteUrl, type: "POST",
                contentType: "application/json",
                data: JSON.stringify(selectedIds),
                success: function (res) {
                    showToast(res.isSuccess ? "success" : "error", res.message);
                },
                error: function () { },
                complete: function () {
                    resetForm();
                    AutoPrintingStationeryPurchaseId();
                    loadCategoryData();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            });
        });

        // ════════════════════════════════════════════════════════════════════
        // CLOSE MODAL HELPERS
        // ════════════════════════════════════════════════════════════════════
        function closeAllModals(callback) {
            $(".modal").modal('hide');
            setTimeout(function () {
                if (typeof callback === 'function') callback();
            }, 300);
        }

        $(document).on('click', ".closesupplierTypeModel", function () {
            closeAllModals(() => $(commonName.SupplierModalBtn).trigger('click'));
        });
        $(document).on('click', ".closeCountryModel", function () {
            closeAllModals(() => $(commonName.SupplierModalBtn).trigger('click'));
        });
        $(document).on('click', ".closeBrandModel", function () {
            closeAllModals(() => $(commonName.ProductModalBtn).trigger('click'));
        });
        $(document).on('click', ".closeCatagoryModel", function () {
            closeAllModals(() => $(commonName.ProductModalBtn).trigger('click'));
        });
        $(document).on('click', ".itemBrandModalLabelClose", function () {
            closeAllModals(() => $(commonName.ProductModelBtn).trigger('click'));
        });

        // ════════════════════════════════════════════════════════════════════
        // DataTable
        // ════════════════════════════════════════════════════════════════════
        function loadCategoryData() {
            tableContainer.ajax.reload(null, false);
        }

        var tableContainer = $('#printingStationTable').DataTable({
            "ajax": {
                "url": loadCategoryDataUrl, "type": "GET", "datatype": "json",
                "dataSrc": function (json) { return json.data || []; },
                "error": function (xhr) {
                    showToast("error", "Data loading failed: " + xhr.statusText);
                }
            },
            "columns": [
                {
                    "data": "tc",
                    "render": function (data) {
                        return `<input type="checkbox" class="row-checkbox" value="${data}"/>`;
                    }, "orderable": false
                },
                {
                    "data": "purchaseReceiveNo",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link stationary-btn-edit" data-id="${data}">${data}</button>`;
                    }
                },
                { "data": "showReceiveDate" },
                { "data": "departmentName" },
                { "data": "supplierName" },
                { "data": "invoiceNo" },
                {
                    "data": "totalAmount",
                    "render": function (data, type, row) {
                        return data != null ? data : (row.invoiceValue || 0);
                    }
                },
                { "data": "employeeID_ReceiveBy" },
                { "data": "companyCode" }
            ],
            "columnDefs": [{ "targets": 2, "width": "auto" }],
            "paging": true,
            "pagingType": "full_numbers",
            "searching": true,
            "ordering": true,
            "responsive": true,
            "autoWidth": true,
            "language": {
                "search": "Search....",
                "lengthMenu": "Show _MENU_ entries per page",
                "zeroRecords": "No data found",
                "info": "Showing _START_ to _END_ of _TOTAL_ entries",
                "paginate": { "first": "First", "last": "Last", "next": "Next", "previous": "Previous" }
            }
        });

        // ════════════════════════════════════════════════════════════════════
        // INIT
        // ════════════════════════════════════════════════════════════════════
        window.categoryModuleLoaded = true;

        var init = function () {
            stHeader();
            datePiker(".datePicker");
            AutoPrintingStationeryPurchaseId();
        };

        setTimeout(function () {
            resetForm();
            AutoPrintingStationeryPurchaseId();
        }, 100);

        init();
    };
})(jQuery);