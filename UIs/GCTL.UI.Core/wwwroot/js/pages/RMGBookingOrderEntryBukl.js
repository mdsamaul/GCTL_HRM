(function ($) {
    $.RMGBookingOrderEntryBukl = function (options) {
        var settings = $.extend({
            baseUrl: '', load: null,
            quickAddModal: "#quickAddModal"



        }, options);
        var table = null;
        var currentBookingType = '';
        window.selectedIds = window.selectedIds || new Set();
        window.selectedBookingOrderId = window.selectedBookingOrderId || new Set();
        var purchaseTable;

        function init() {
            injectTooltipStyles();
            purchaseOrderLoad();
            RMG_BookingOrderAutoId();
            bindEvents();
            setDate();
            setDateTime();
            BookingOrderGrid();
            if (typeof settings.load === 'function') settings.load();
        }
        function showBookingLoader() {
            $("#bookingLoader").removeClass("d-none");
        }

        function hideBookingLoader(delay = 0) {
            setTimeout(() => {
                $("#bookingLoader").addClass("d-none");
            }, delay);
        }

        function RMG_BookingOrderAutoId() {
            $.ajax({
                url: settings.baseUrl + "/BookingOrderAutoId",
                type: "GET",
                success: function (res) {
                    $("#BookingOrderEntryBuklSetup_BookinOrderNo").val(res.data);
                },
                error: function (e) {
                    toastr.error("Failed to generate Booking Order Auto ID");
                }
            });
        }

        function setDate() {
            flatpickr(".flatDatePicker", CalendarService.createConfig({
                dateFormat: "d/m/Y",
                altInput: true,
                altFormat: "d/m/Y",
                allowInput: true,
                defaultDate: "today",
            }));
        }

        function setDateTime() {
            flatpickr(".flatDatePickerWithTime", {
                enableTime: true,              
                dateFormat: "d/m/Y h:i K",   
                altInput: true,
                altFormat: "d/m/Y h:i K",    
                allowInput: true,
                defaultDate: "today",
                time_24hr: false           
            });
        }
        $(document).on('change', "#BookingOrderEntryBuklSetup_SupplierId", function () {
            var id = $(this).val();

            if (!id) {
                $("#SelectedSupplierHidden").val("");
                $("#SupplierAddress").val("");

                if ($("#SupplierCountry").data('select2')) {
                    $("#SupplierCountry")
                        .val(null)
                        .prop("disabled", false)
                        .trigger('change');
                }

                return;
            }

            $("#SelectedSupplierHidden").val(id);

            $.ajax({
                url: settings.baseUrl + "/GetSupplierDetails",
                type: "GET",
                data: { supplierId: id },
                success: function (res) {

                    if (!res || res.success !== true) {
                        return;
                    }

                    $("#SupplierAddress").val(res.address ?? "");

                    if ($("#SupplierCountry").data('select2')) {
                        $("#SupplierCountry")
                            // 1. Sets the value
                            .val(res.countryId)
                            // 2. Disables the element
                            .prop("disabled", true)
                            // 3. Triggers change for Select2 to update its display
                            .trigger('change');
                    } else {
                        // fallback (non-select2)
                        $("#SupplierCountry")
                            .val(res.countryId)
                            .prop("disabled", true);
                    }

                },
                error: function (e) {

                }
            });
        });


        function bindEvents() {
            $('#BookingOrderEntryBuklSetup_BookingType').on('change', handleBookingTypeChange);
            $(document).on('click', '.btn-delete-row', deleteRow);
        }

        function destroyTable() {
            if (table) {
                disposeAllTooltips();
                table.destroy();
                table = null;
            }
            $('#bookingTable').empty();
        }

        function applySelect2() {
            $('#bookingTable select').each(function () {
                if (!$(this).hasClass("select2-hidden-accessible")) {
                    $(this).select2({
                        width: '100%',
                        dropdownAutoWidth: true,
                        theme: "bootstrap-5",
                        placeholder: "Select",
                        allowClear: true
                    });
                }
            });
        }

        $('.select2').each(function () {
            if (!$(this).hasClass("select2-hidden-accessible")) {
                $(this).select2({
                    width: '100%',
                    dropdownAutoWidth: true,
                    theme: "bootstrap-5",
                    placeholder: "Select",
                    allowClear: true
                });
            }
        });

        $(document).ready(function () {
            //
            let loadUrl,
                target,
                reloadUrl,
                title,
                lastCode;
            // Quick add
            $("body").on("click", ".js-quick-add", function (e) {

                console.log("asdfasdf");
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


                $(target).empty("");
                $(target).append($('<option>', {
                    value: '',
                    text: `--Select ${title}--`
                }));
                $.ajax({
                    url: reloadUrl,
                    method: "GET",
                    success: function (response) {
                        console.log(response);
                        $.each(response, function (i, item) {
                            $(target).append($('<option>', {
                                value: item.code,
                                text: item.name
                            }));
                        });

                        $(target).val(lastCode);
                        console.log("Testttt", lastCode);

                    }
                });
            });            
        })




        function purchaseOrderLoad() {
            if ($.fn.DataTable.isDataTable('#purchaseOrderTable')) {
                purchaseTable.clear().destroy();
            }
            var purchaseTable = $('#purchaseOrderTable').DataTable({
                "processing": true,
                "serverSide": true,
                "ajax": {
                    "url": settings.baseUrl + "/GetPurchaseOrders",
                    "type": "POST",
                    dataSrc: function (data) {
                        return data.data;
                    },
                },
                "pageLength": 4,
                "lengthMenu": [[3, 5, 10, -1], [3, 5, 10, "All"]],
                "scrollY": "250px",
                "scrollCollapse": true,
                "paging": true,
                "columns": [
                    {
                        "data": "costingId",
                        render: function (data) {
                            let checked = selectedIds.has(data) ? "checked" : "";
                            return `<input type="checkbox" class="row-check" data-id="${data}" ${checked} />`;
                        }
                    },
                    { "data": "poNo" },
                    { "data": "orderQty" },
                    { "data": "styleName" },
                    { "data": "masterPo" },
                    { "data": "funJobNo" },
                    { "data": "buyerName" }
                ]
            });

        }


        $(document).on("change", "#purchaseOrderTable .row-check", function () {
            let id = $(this).attr("data-id");

            if (!id) return;

            if (this.checked) {
                selectedIds.add(id);
            } else {
                selectedIds.delete(id);
                $("#selectAll").prop("checked", false);
            }

            let costingIds = getSelectedCostingIds();

            if (costingIds.length === 0) return;

            $.ajax({
                url: settings.baseUrl + '/GetItemTypes',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(costingIds),
                success: function (data) {
                    let select = $('#BookingOrderEntryBuklSetup_BookingType');
                    select.empty();
                    select.append('<option value="" disabled selected>--Select Booking Type--</option>');

                    $.each(data, function (i, item) {
                        select.append(
                            `<option value="${item.bookingItemTypeID}">
                        ${item.bookingItemType}
                     </option>`
                        );
                    });
                },
                error: function () {
                    toastr.error("Failed to load booking item types");
                }
            });
        });

        function buildHeaders(type) {


            let html = '<tr>';
            // Hidden headers (will be hidden via CSS/Class: header-hidden)
            html += '<th class="border-end header-hidden" style="min-width:70px">Type</th>';
            html += '<th class="border-end header-hidden" style="min-width:70px">Id</th>';
            html += '<th class="border-end header-hidden" style="min-width:70px">Integra Job No.</th>';

            // Original Visible Headers
            html += '<th class="border-end" style="min-width:70px">PO No.</th>';
            html += '<th class="border-end" style="min-width:90px">Item</th>';
            html += '<th class="border-end" style="min-width:100px">Description</th>';
            html += '<th class="border-end" style="min-width:80px">Color</th>';

            if (type === '04') {
                html += '<th class="border-end " style="min-width:80px">Size</th>';
                html += '<th class="border-end " style="min-width:80px">Length</th>';
                html += '<th class="border-end " style="min-width:80px">Unit</th>';
                html += '<th class="border-end " style="min-width:80px">Width</th>';
                html += '<th class="border-end " style="min-width:80px">Unit</th>';
                html += '<th class="border-end " style="min-width:80px">Height</th>';
                html += '<th class="border-end " style="min-width:80px">Unit</th>';
            } else if (type === '07') {
                html += `
        <th class="border-end " style="min-width:90px">
            <div class="d-flex justify-content-between align-items-center">
                <i class="fa-solid fa-plus text-dark" style="cursor:pointer;"></i>
                <span>Thread Count</span>
            </div>
        </th>
        `;
            } else if (type === '03') {
                html += '<th class="border-end " style="min-width:80px">Length</th>';
                html += '<th class="border-end " style="min-width:80px">Unit</th>';
                html += '<th class="border-end " style="min-width:80px">Width</th>';
                html += '<th class="border-end " style="min-width:80px">Unit</th>';
                html += '<th class="border-end " style="min-width:80px">Flap</th>';
                html += '<th class="border-end " style="min-width:80px">Unit</th>';
                html += '<th class="border-end " style="min-width:80px">Guest</th>';
                html += '<th class="border-end " style="min-width:80px">Unit</th>';
            }

            html += '<th class="border-end text-center" style="min-width:50px">Gar. Qty</th>';
            html += '<th class="border-end text-center" style="min-width:50px">Unit</th>';
            html += '<th class="border-end text-center" style="min-width:50px">Cons/mtr</th>';
            html += '<th class="border-end text-center" style="min-width:60px">Unit</th>';
            html += '<th class="border-end text-center" style="min-width:80px">Total Qty</th>';
            html += '<th class="border-end text-center" style="min-width:60px">Unit</th>';
            html += '<th class="border-end text-center" style="min-width:50px">Order Qty</th>';
            html += '<th class="border-end text-center" style="min-width:60px">Unit</th>';
            html += '<th class="border-end text-center" style="min-width:40px">Per (%)</th>';
            html += '<th class="border-end text-center" style="min-width:60px">Unit Price</th>';
            html += '<th class="border-end text-center" style="min-width:70px">Total Price</th>';
            html += '<th class="border-end text-center" style="min-width:50px">Curr.</th>';
            if (type !=="01") {
                html += '<th class="border-end text-center" style="min-width:80px">Remarks</th>';
            }          
            html += '<th class="text-center" style="min-width:40px">Action</th>';
            html += '</tr>';
            return html;
        }

        function buildRow(item, type, dd) {
            //debugger
            let row = '<tr>';

            //// 1. Booking Type (Hidden)
            //row += `<td class="border-end row-hidden"><input type="hidden" class="form-control form-control-sm" data-field="BookingType" value="${type || ''}"></td>`;
            row += `<td class="row-hidden"><input type="hidden" data-field="BookingType" value="${type ?? ''}"></td>`;
            //// 2. ID (Hidden, data-field="Id" for DTO mapping)
            //row += `<td class="border-end row-hidden"><input type="hidden" class="form-control form-control-sm" data-field="Id" value="${item.id || ''}"></td>`;
            row += `<td class="row-hidden"><input type="hidden" data-field="Id"value="${item.id ?? item.Id ?? item.ID ?? 0}"></td>`;
            //// 3. Integra Job No. (Hidden)
            row += `<td class="border-end row-hidden"><input type="hidden" class="form-control form-control-sm" data-field="IntegraJobNo" value="${item.integraJobNo || ''}"></td>`;


            // 4. PO No. (Visible)
            row += `<td class="border-end"><input type="text" class="form-control form-control-sm" readonly data-field="PoNo" value="${item.poNo || ''}"></td>`;

            // 5. Item ID (Dropdown)
            row += `<td class="border-end">
    <select class="form-select form-select-sm item-container" data-field="ItemId" disabled>
        <option value="">Select</option>
        ${dd.items.map(i => `
            <option value="${i.id}" ${i.id === (item.itemID || item.itemId) ? 'selected' : ''}>
                ${i.name}
            </option>
        `).join('')}
    </select>
</td>`;

            // 6. Description
            const description = (item.description || '').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
            row += `
<td class="border-end">
    <input type="text" readonly
        class="form-control form-control-sm description-input description-container"
        data-field="Description"
        data-bs-toggle="tooltip"
        data-bs-placement="top"
        data-bs-custom-class="custom-tooltip"
        data-bs-html="true"
        title="${description}"
        value="${description.replace(/&quot;/g, '"').replace(/&#39;/g, "'")}">
</td>`;

            // 7. Color
            row += `
<td class="border-end">
    <select class="form-select form-select-sm color-container" data-field="ColorId">
        <option value="">Select</option>
        ${dd.colors.map(c => `
                <option value="${c.id}"
                    ${String(c.id).padStart(3, '0') === item.colorId ? 'selected' : ''}>
                    ${c.name}
                </option>
            `).join('')
                }
    </select>
</td>`;


            // --- Type Specific Fields ---
            if (type === '04') {
                row += `<td class="border-end "><select class="form-select form-select-sm" data-field="SizeId"><option>Select</option>${dd.sizes.map(s => `<option value="${s.id}" ${s.id === (item.sizeID || item.sizeId) ? 'selected' : ''}>${s.name}</option>`).join('')}</select></td>`;
                row += `<td class="border-end "><input type="number" class="form-control form-control-sm" data-field="CartonLength" value="${item.cartonLength || ''}"></td>`;
                row += `<td class="border-end "><select class="form-select form-select-sm" data-field="LeangthUnitID">${dd.units.map(u => `<option value="${u.id}" ${u.id === item.leangthUnitID ? 'selected' : ''}>${u.name}</option>`).join('')}</select></td>`;
                row += `<td class="border-end "><input type="number" class="form-control form-control-sm" data-field="CartonWidth" value="${item.cartonWidth || ''}"></td>`;
                row += `<td class="border-end "><select class="form-select form-select-sm" data-field="WidthUnitID">${dd.units.map(u => `<option value="${u.id}" ${u.id === item.widthUnitID ? 'selected' : ''}>${u.name}</option>`).join('')}</select></td>`;
                row += `<td class="border-end "><input type="number" class="form-control form-control-sm" data-field="CatonHeight" value="${item.catonHeight || ''}"></td>`;
                row += `<td class="border-end "><select class="form-select form-select-sm" data-field="HeightUnitID">${dd.units.map(u => `<option value="${u.id}" ${u.id === item.heightUnitID ? 'selected' : ''}>${u.name}</option>`).join('')}</select></td>`;
            } else if (type === '07') {
                row += `<td class="border-end "><select class="form-select form-select-sm" data-field="ThreadCountID"><option value="">Select</option>${dd.threadCounts.map(t => `<option value="${t.id}" ${t.id === item.threadCountID ? 'selected' : ''}>${t.name}</option>`).join('')}</select></td>`;
            } else if (type === '03') {
                row += `<td class="border-end "><input type="number" class="form-control form-control-sm" data-field="Length" value="${item.length || ''}"></td>`;
                row += `<td class="border-end "><select class="form-select form-select-sm" data-field="LengthUnitID">${dd.units.map(u => `<option value="${u.id}" ${u.id === item.lengthUnitID ? 'selected' : ''}>${u.name}</option>`).join('')}</select></td>`;
                row += `<td class="border-end "><input type="number" class="form-control form-control-sm" data-field="Width" value="${item.width || ''}"></td>`;
                row += `<td class="border-end "><select class="form-select form-select-sm" data-field="WidthUnitID">${dd.units.map(u => `<option value="${u.id}" ${u.id === item.widthUnitID ? 'selected' : ''}>${u.name}</option>`).join('')}</select></td>`;
                row += `<td class="border-end "><input type="number" class="form-control form-control-sm" data-field="Flap" value="${item.flap || ''}"></td>`;
                row += `<td class="border-end "><select class="form-select form-select-sm" data-field="FlapUnitID">${dd.units.map(u => `<option value="${u.id}" ${u.id === item.flapUnitID ? 'selected' : ''}>${u.name}</option>`).join('')}</select></td>`;
                row += `<td class="border-end "><input type="number" class="form-control form-control-sm" data-field="Guest" value="${item.guest || ''}"></td>`;
                row += `<td class="border-end "><select class="form-select form-select-sm" data-field="GuestUnitID">${dd.units.map(u => `<option value="${u.id}" ${u.id === item.guestUnitID ? 'selected' : ''}>${u.name}</option>`).join('')}</select></td>`;
            }

            // --- Common Fields ---
            row += `<td class="border-end"><input type="number" class="form-control form-control-sm" readonly data-field="GarmentQty" value="${item.garmentQty || item.germentQty || ''}"></td>`;
            row += `<td class="border-end"><select class="form-select form-select-sm" disabled data-field="GarmentQtyUnitID">${dd.units.map(u => `<option value="${u.id}" ${u.id === item.garmentQtyUnitID ? 'selected' : ''}>${u.name}</option>`).join('')}</select></td>`;
            row += `<td class="border-end"><input type="number" readonly class="form-control form-control-sm" data-field="Consumption" value="${item.consumption || ''}"></td>`;
            row += `<td class="border-end"><select class="form-select form-select-sm" disabled data-field="ConsumptionUnitID">${dd.units.map(u => `<option value="${u.id}" ${u.id === item.consumptionUnitID ? 'selected' : ''}>${u.name}</option>`).join('')}</select></td>`;
            row += `<td class="border-end">
    <input type="number" readonly class="form-control form-control-sm" data-field="TotalQty" value="${item.totalQty !== null && item.totalQty !== undefined
                    ? Number(item.totalQty).toFixed(2)
                    : ''}">
</td>`;
            row += `<td class="border-end"><select class="form-select form-select-sm" disabled data-field="TotalQtyUnitID">${dd.units.map(u => `<option value="${u.id}" ${u.id === item.totalQtyUnitID ? 'selected' : ''}>${u.name}</option>`).join('')}</select></td>`;
            row += `<td class="border-end"><input type="number" class="form-control form-control-sm" readonly data-field="OrderQty" value="${item.orderQty || ''}"></td>`;
            row += `<td class="border-end"><select class="form-select form-select-sm" disabled data-field="OrderQtyUnitID">${dd.units.map(u => `<option value="${u.id}" ${u.id === item.orderQtyUnitID ? 'selected' : ''}>${u.name}</option>`).join('')}</select></td>`;
            row += `<td class="border-end"><input type="number" readonly class="form-control form-control-sm" data-field="Percentage" value="${item.percentage || 0}"></td>`;
            row += `<td class="border-end"><input type="number" readonly step="0.01" class="form-control form-control-sm" data-field="UnitPrice" value="${item.unitPrice || ''}"></td>`;
            row += `<td class="border-end"><input type="number" readonly step="0.01" class="form-control form-control-sm" data-field="TotalPrice" value="${item.totalPrice || ''}"></td>`;
            row += `<td class="border-end"><select class="form-select form-select-sm" disabled>${dd.currencies.map(c => `<option value="${c.id}" ${c.id === (item.currencyID || item.currencyId) ? 'selected' : ''}>${c.name}</option>`).join('')}</select></td>`;
            if (type!=="01") {
                row += `<td class="border-end"><input type="text" class="form-control form-control-sm" data-field="Remarks" value="${item.remarks || ''}"></td>`;
            }            
            row += `<td class="text-center"><button class="btn btn-sm btn-danger btn-delete-row"><i class="fas fa-trash"></i></button></td>`;
            row += '</tr>';
            return row;
        }

        function collectRowData($row) {
            const data = {};
            // Collect all fields with data-field attribute
            $row.find('[data-field]').each(function () {
                const fieldName = $(this).attr('data-field');
                let value = $(this).val();

                // Convert common numbers back to number type for DTO
                if (['Id', 'GarmentQty', 'Consumption', 'TotalQty', 'OrderQty', 'Percentage', 'UnitPrice', 'TotalPrice', 'CartonLength', 'CartonWidth', 'CatonHeight', 'Length', 'Width', 'Flap', 'Guest'].includes(fieldName)) {
                    // Special handling for ID (must be integer 0 or greater)
                    if (fieldName === 'Id') {
                        value = parseInt(value) || 0;
                        console.log(`  -> Parsed ID: ${value}`);
                    } else {
                        // Otherwise, convert to float/decimal
                        value = parseFloat(value) || null;
                    }
                }

                data[fieldName] = value;
            });

            // Check if ID exists (allow 0 for new rows)
            if (data.Id === undefined || data.Id === null || data.Id === '') {
                return null;
            }

            return data;
        }

        function sendUpdateToServer(data) {
            // Check if settings.baseUrl is defined and accessible
            const url = (typeof settings !== 'undefined' && settings.baseUrl)
                ? settings.baseUrl + '/UpdateBookingItem'
                : '/RMGBookingOrderEntryBukl/UpdateBookingItem';

            $.ajax({
                url: url,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (response) {
                    if (response.success) {
                        console.log('Update successful');
                    } else {
                        console.log('Update failed');
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.error('Update error:', textStatus, errorThrown);
                }
            });
        }

        // Event handler for all input/select changes in the table
        $(document).on('change', '#bookingTable input, #bookingTable select', function () {
           // debugger;

            // Check if the field is readonly or disabled
            if ($(this).prop('readonly') || $(this).prop('disabled')) {
                console.log('Field is readonly or disabled, skipping update');
                return; // Don't update if readonly or disabled
            }

            if ($(this).attr('data-field')) {
                const $row = $(this).closest('tr');
                const updateData = collectRowData($row);

                if (updateData) {
                    console.log('Sending update for editable field:', $(this).attr('data-field'));
                    sendUpdateToServer(updateData);
                }
            }
        });


        function handleBookingTypeChange() {
            //debugger
            var type = $(this).val();
            if (!type || type === '--Select Booking Type--') return;

            let costingIds = getSelectedCostingIds() || [];
            let bookingOrderIds = getselectedBookingOrderId();

            if (bookingOrderIds.length == 0) {
                if (costingIds.length === 0) {
                    toastr.warning("Please select at least one booking order");
                    return;
                }
            }

            if (costingIds.length === 0) {
                costingIds.push("edit");
            }
            var dto = {
                BookingType: type,
                CostingId: costingIds
            };


            showLoader();

            $.ajax({
                url: settings.baseUrl + "/LoadBookingTable",
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(dto),
                success: function (response) {
                    hideLoader();
                    if (!response.success) {
                        toastr.error(response.message || 'Failed');
                        return;
                    }

                    destroyTable();

                    let tableHtml = '<thead class="table-light sticky-top">' + buildHeaders(type) + '</thead>';
                    tableHtml += '<tbody>';
                    $.each(response.data || [], function (i, item) {
                        tableHtml += buildRow(item, type, response.dropdownData);
                    });
                    tableHtml += '</tbody>';

                    $('#bookingTable').html(tableHtml);
                    if ($.fn.DataTable.isDataTable('#bookingTable')) {
                        $('#bookingTable').DataTable().destroy();
                    }
                    table = $('#bookingTable').DataTable({
                        scrollX: true,
                        scrollY: '500px',
                        scrollCollapse: true,
                        paging: false,
                        info: false,
                        ordering: false,
                        autoWidth: false,
                        searching: false,
                        destroy: true,
                        dom: 'ft',
                        initComplete: function () {
                            $('.dataTables_scrollHead').css({
                                'position': 'sticky',
                                'top': '0',
                                'z-index': '10',
                                'background': 'white'
                            });
                            applySelect2();
                            initializeTooltips();
                        }
                    });

                    setTimeout(() => table.columns.adjust().draw(false), 150);
                },
                error: function () {
                    hideLoader();
                    toastr.error('Server error');
                }
            });
        }

        function deleteRow() {
            if (table) {
                table.row($(this).parents('tr')).remove().draw(false);

                setTimeout(() => {
                    table.columns.adjust();
                    applySelect2();
                    initializeTooltips();
                }, 100);
            }
        }

        function injectTooltipStyles() {
            if (!document.getElementById('custom-tooltip-styles')) {
                const style = document.createElement('style');
                style.id = 'custom-tooltip-styles';
                style.innerHTML = `
                    .custom-tooltip {
                        background-color: #333 !important;
                        color: #fff !important;
                        font-size: 14px !important;
                        padding: 8px 12px !important;
                        border-radius: 8px !important;
                        box-shadow: 0px 4px 15px rgba(0,0,0,0.3) !important;
                        max-width: 300px !important;
                        word-wrap: break-word !important;
                    }
                    .custom-tooltip .tooltip-arrow::before {
                        border-top-color: #333 !important;
                    }
                `;
                document.head.appendChild(style);
            }
        }

        function disposeAllTooltips() {
            try {
                $('#bookingTable [data-bs-toggle="tooltip"]').each(function () {
                    const tooltipInstance = bootstrap.Tooltip.getInstance(this);
                    if (tooltipInstance) {
                        tooltipInstance.dispose();
                    }
                });
            } catch (e) {

            }
        }

        function initializeTooltips() {
            disposeAllTooltips();

            setTimeout(() => {
                try {
                    $('#bookingTable [data-bs-toggle="tooltip"]').each(function () {
                        new bootstrap.Tooltip(this, {
                            trigger: 'hover focus',
                            boundary: 'window',
                            html: true
                        });
                    });
                } catch (e) {

                }
            }, 250);
        }

        function showLoader() { $('.loading-overlay').addClass('active'); }
        function hideLoader() { $('.loading-overlay').removeClass('active'); }

        init();

        function getIsoDate(dateStr) {
            if (!dateStr) return null;
            const parts = dateStr.split("/");
            if (parts.length !== 3) return null;
            const day = parseInt(parts[0], 10);
            const month = parseInt(parts[1], 10) - 1;
            const year = parseInt(parts[2], 10);

            return new Date(Date.UTC(year, month, day)).toISOString();
        }


        function getBookingDataAndSend() {
            return {
                Tc: parseFloat($("#BookingOrderEntryBuklSetup_Tc").val()) || 0,
                BookinOrderNo: $("#BookingOrderEntryBuklSetup_BookinOrderNo").val(),
                BookinDate: getIsoDate($("#BookingOrderEntryBuklSetup_BookinDate").val()),
                PurchasedOfficer: $("#BookingOrderEntryBuklSetup_PurchasedOfficer").val(),
                Remarks: $("#BookingOrderEntryBuklSetup_Remarks").val(),
                DeliveryDate: getIsoDate($("#BookingOrderEntryBuklSetup_DeliveryDate").val()),
                DeliveryAddress: $("#BookingOrderEntryBuklSetup_DeliveryAddress").val(),
                DeliveryMethod: $("#BookingOrderEntryBuklSetup_DeliveryMethod").val(),
                PaymentTerms: $("#BookingOrderEntryBuklSetup_PaymentTerms").val(),
                TermsCondition: $("#BookingOrderEntryBuklSetup_TermsCondition").val(),
                BookingType: $("#BookingOrderEntryBuklSetup_BookingType").val(),
                Pino: $("#BookingOrderEntryBuklSetup_Pino").val(),
                Pidate: getIsoDate($("#BookingOrderEntryBuklSetup_Pidate").val()),
                Pivalue: parseFloat($("#BookingOrderEntryBuklSetup_Pivalue").val()) || null,
                PicurrencyId: $("#BookingOrderEntryBuklSetup_PicurrencyId").val(),
                SupplierId: $("#BookingOrderEntryBuklSetup_SupplierId").val(),

                SelectedCostingIds: Array.from(selectedIds)
            };
        }
        $(document).on('click', '.js-booking-order-info-save', function () {

          
            let costingIds = getSelectedCostingIds() || [];
            let bookingOrderIds = getselectedBookingOrderId();

            if (bookingOrderIds.length == 0) {
                if (costingIds.length === 0) {
                    toastr.warning("Please select at least one purchase order");
                    return;
                }
            }
            var dto = getBookingDataAndSend();
            console.log('Booking DTO:', dto);
            if (!dto.BookinDate) {
                toastr.warning("Booking Date is required");
                //$("#BookingOrderEntryBuklSetup_BookinDate").focus().click(); 
                document.querySelector("#BookingOrderEntryBuklSetup_BookinDate")._flatpickr.open();
                return;
            }
            if (!dto.BookingType) {
                toastr.warning("Booking Item Type is required");
                $("#BookingOrderEntryBuklSetup_BookingType").select2('open'); // open select2 dropdown
                return;
            }
           
            if (!dto.SupplierId) {
                toastr.warning("Supplier is required");
                $("#BookingOrderEntryBuklSetup_SupplierId").select2('open'); // open select2 dropdown
                return;
            }
            if (!dto.DeliveryDate) {
                toastr.warning("Delivery Date is required");
                document.querySelector("#BookingOrderEntryBuklSetup_DeliveryDate")._flatpickr.open();
                return;
            }

            $.ajax({
                url: settings.baseUrl + '/SaveBooking',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(dto),
                success: function (response) {
                    toastr.success(response.message || "Saved successfully");

                    window.selectedIds.clear();
                    window.selectedBookingOrderId.clear();
                    if (purchaseTable) {
                        purchaseTable.ajax.reload(null, false);
                    }

                    BookingOrderGrid();
                    RMG_BookingOrderAutoId();
                },
                error: function (xhr) {
                    toastr.error("Failed to save booking");
                }, complete: function () {
                    resetBookingForm();
                }
            });
        });


        function resetBookingForm() {
            // Reset inputs
            $("#BookingOrderEntryBuklSetup_Tc").val("");
            $("#BookingOrderEntryBuklSetup_BookinOrderNo").val("");
            $("#BookingOrderEntryBuklSetup_BookinDate").val("");
            $("#BookingOrderEntryBuklSetup_PurchasedOfficer").val("");
            $("#BookingOrderEntryBuklSetup_Remarks").val("");
            $("#BookingOrderEntryBuklSetup_DeliveryDate").val("");
            $("#BookingOrderEntryBuklSetup_DeliveryAddress").val("");
            $("#BookingOrderEntryBuklSetup_DeliveryMethod").val("");
            $("#BookingOrderEntryBuklSetup_PaymentTerms").val("");
            $("#BookingOrderEntryBuklSetup_TermsCondition").val("");
            $("#BookingOrderEntryBuklSetup_BookingType").val("");
            $("#BookingOrderEntryBuklSetup_Pino").val("").prop('disabled', false);
            $("#BookingOrderEntryBuklSetup_Pidate").val("");
            $("#BookingOrderEntryBuklSetup_Pivalue").val("");
            $("#BookingOrderEntryBuklSetup_PicurrencyId").val("");
            $("#BookingOrderEntryBuklSetup_SupplierId").val("");
            $("#SupplierAddress").val("");

            // Reset Select2
            $(".select2").each(function () {
                $(this).val(null).trigger("change.select2");
            });

            // Clear selected IDs
            if (window.selectedIds) window.selectedIds.clear();
            if (window.selectedBookingOrderId) window.selectedBookingOrderId.clear();

            // Reset DataTable safely
            if ($.fn.DataTable && $('#bookingTable').length) {
                if ($.fn.dataTable.isDataTable('#bookingTable')) {
                    let table = $('#bookingTable').DataTable();
                    table.clear().draw();

                    // table.clear().destroy();
                }
            }

            // Reset checkboxes
            $('#purchaseOrderTable .row-check:checked').prop('checked', false);
            $("#selectAll").prop("checked", false);

            // Reset date pickers
            if (typeof setDate === "function") {
                setDate();
            }
            if (typeof setDateTime === "function") {             
                setDateTime();
            }
            RMG_BookingOrderAutoId();

        }

        // Bind reset to clear button
        $(document).on('click', '#js-costing-info-clear', function () {
            resetBookingForm();
        });

        $(document).on('click', '#js-costing-info-delete-confirm', function () {
            let bookingOrderIds = getselectedBookingOrderId();

            if (bookingOrderIds.length === 0) {
                toastr.warning("Please select at least one booking order to delete.");
                return;
            }


            if (confirm("Are you sure you want to delete the selected booking orders?")) {

                $.ajax({
                    url: settings.baseUrl + "/DeleteBookingOrder",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(bookingOrderIds),
                    success: function (res) {
                        if (res.success) {
                            // Success toaster
                            toastr.success(res.message || "Booking orders deleted successfully!");

                            // Grid refresh
                            BookingOrderGrid();
                            window.selectedIds.clear();
                            window.selectedBookingOrderId.clear();
                        } else {
                            toastr.error(res.message || "Failed to delete booking orders.");
                        }
                    },
                    error: function (err) {
                        toastr.error("An error occurred while deleting booking orders.");
                    }
                });
            } else {
                toastr.info("Delete action cancelled.");
            }
        });


        function BookingOrderGrid() {
            if ($.fn.DataTable.isDataTable('#bookingOrderGridTable')) {
                $('#bookingOrderGridTable').DataTable().destroy();
            }
            var bookingGridTable = $("#bookingOrderGridTable").DataTable({
                processing: true,
                serverSide: true,
                filter: true,
                orderMulti: false,
                ajax: {
                    url: settings.baseUrl + "/GetBookingList",
                    type: "POST",
                    dataSrc: function (data) {
                        return data.data;
                    }
                },
                columns: [
                    {
                        data: "tc",
                        render: function (data) {
                            let checked = selectedBookingOrderId.has(data) ? "checked" : "";
                            return `<input type="checkbox" class="row-check-master-check" data-id="${data}" ${checked} />`;
                        }
                    },
                    {
                        data: "bookingOrderNo",
                        render: function (data) {
                            return `<button type="button" class="btn btn-link row-check-master" data-id="${data}">${data}</button>`;
                        }
                    },
                    { data: "bookingDate" },
                    { data: "bookingTypeName" },
                    { data: "supplierName" },
                    { data: "styleName" },
                    { data: "poNo" },
                    { data: "integraJobNo" },
                    {
                        data: "pifilePath",
                        orderable: false,
                        render: function (file) {
                            return file ? `<a href="${file}" target="_blank" class="text-success">View</a>` : "";
                        }
                    }
                ],
                drawCallback: function () {
                    let total = $(".row-check-master-check").length;
                    let checked = $(".row-check-master-check:checked").length;
                    $("#selectMasterAll").prop("checked", total > 0 && total === checked);
                }
            });



            //$("#bookingOrderGridTable").on("click", ".row-check-master", function () {
            //    let id = $(this).data("id");
            //    let rowData = bookingGridTable.row($(this).closest("tr")).data();

            //    selectedBookingOrderId.add(id);

            //    if (rowData == undefined) {
            //        return;
            //    }
            //    populateBookingForm(rowData);

            //    $.ajax({
            //        url: settings.baseUrl + "/GetBookingItemTypes",
            //        type: "POST",
            //        data: { id },
            //        success: function (resp) {
            //            console.log(resp);
            //            if (resp.success && resp.data) {
            //                // Build headers based on first item
            //                if (resp.data.length > 0) {
            //                    buildHeaders(resp.data[0]);
            //                }

            //                // Destroy old table if exists
            //                if ($.fn.DataTable.isDataTable('#bookingTable')) {
            //                    $('#bookingTable').DataTable().clear().destroy();
            //                }

            //                // Build table HTML
            //                let tableHtml = '<thead class="table-light sticky-top">' + buildHeaders(rowData.bookingType) + '</thead>';
            //                tableHtml += '<tbody>';
            //                $.each(resp.data || [], function (i, item) {
            //                    tableHtml += buildRow(item, rowData.bookingType, resp.dropdownData);
            //                });
            //                tableHtml += '</tbody>';

            //                $('#bookingTable').html(tableHtml);

            //                // Re-initialize DataTable
            //                let table = $('#bookingTable').DataTable({
            //                    scrollX: true,
            //                    scrollY: '500px',
            //                    scrollCollapse: true,
            //                    paging: false,
            //                    info: false,
            //                    ordering: false,
            //                    autoWidth: false,
            //                    searching: false,
            //                    dom: 'ft',
            //                    initComplete: function () {
            //                        $('.dataTables_scrollHead').css({
            //                            'position': 'sticky',
            //                            'top': '0',
            //                            'z-index': '10',
            //                            'background': 'white'
            //                        });
            //                        applySelect2();
            //                        initializeTooltips();
            //                    }
            //                });

            //                setTimeout(() => table.columns.adjust().draw(false), 150);
            //            } else {
            //                alert(resp.message);
            //            }
            //        },
            //        error: function (xhr) {

            //        }
            //    });

            //});


            $("#bookingOrderGridTable").on("click", ".row-check-master", function () {

                let id = $(this).data("id");
                let rowData = bookingGridTable.row($(this).closest("tr")).data();
                if (!rowData) return;

                selectedBookingOrderId.clear();
                selectedBookingOrderId.add(id);

                populateBookingForm(rowData);
                showBookingLoader();

                $("#bookingLoader").fadeIn(100);

                setTimeout(() => {

                    $.ajax({
                        url: settings.baseUrl + "/GetBookingItemTypes",
                        type: "POST",
                        data: { id },
                        success: function (resp) {

                            if (!(resp.success && resp.data)) {
                                alert(resp.message);
                                return;
                            }

                            console.log(resp)
                            // 🔥 STEP 1: Destroy DataTable if exists
                            if ($.fn.DataTable.isDataTable('#bookingTable')) {
                                $('#bookingTable').DataTable().destroy(true);
                            }

                            // 🔥 STEP 2: Remove old table completely (wrapper সহ)
                            $('#bookingTableContainer').empty();

                            // 🔥 STEP 3: Create fresh table
                            let tableHtml = `
                    <table id="bookingTable" class="table table-bordered table-sm w-100">
                        <thead class="table-light sticky-top">
                            ${buildHeaders(rowData.bookingType)}
                        </thead>
                        <tbody>
                `;

                            $.each(resp.data || [], function (i, item) {
                                tableHtml += buildRow(item, rowData.bookingType, resp.dropdownData);
                            });

                            tableHtml += `
                        </tbody>
                    </table>
                `;

                            $('#bookingTableContainer').html(tableHtml);

                            // 🔥 STEP 4: Init DataTable
                            let table = $('#bookingTable').DataTable({
                                scrollX: true,
                                scrollY: '500px',
                                scrollCollapse: true,
                                paging: false,
                                info: false,
                                ordering: false,
                                autoWidth: false,
                                searching: false,
                                dom: 'ft',
                                destroy: true,
                                initComplete: function () {
                                    applySelect2();
                                    initializeTooltips();

                                    // Scroll table top
                                    $('.dataTables_scrollBody').scrollTop(0);
                                }
                            });

                            setTimeout(() => table.columns.adjust().draw(false), 150);
                        },
                        error: function (xhr) {
                            console.error(xhr);
                        },
                        complete: function () {
                            hideBookingLoader(200);
                            $("#bookingLoader").fadeOut(150);
                            $('html, body').animate({ scrollTop: 0 }, 300);
                        }
                    });

                }, 300);
            });



            //$("#bookingOrderGridTable").on("click", ".row-check-master", function () {

            //    let id = $(this).data("id");
            //    let rowData = bookingGridTable.row($(this).closest("tr")).data();
            //    if (!rowData) return;

            //    // ✅ Reset selection
            //    selectedBookingOrderId.clear();
            //    selectedBookingOrderId.add(id);

            //    populateBookingForm(rowData);

            //    // 🔥 SHOW LOADER immediately
            //    $("#bookingLoader").fadeIn(100);

            //    // ⏳ ADD SMALL DELAY BEFORE AJAX
            //    setTimeout(() => {

            //        $.ajax({
            //            url: settings.baseUrl + "/GetBookingItemTypes",
            //            type: "POST",
            //            data: { id },
            //            success: function (resp) {

            //                if (!resp.success) {
            //                    alert(resp.message);
            //                    return;
            //                }

            //                // 🔥 Destroy old DataTable
            //                if ($.fn.DataTable.isDataTable('#bookingTable')) {
            //                    $('#bookingTable').DataTable().destroy();
            //                }
            //                $('#bookingTable').remove();

            //                // 🔥 Recreate table
            //                let newTable = `
            //            <table id="bookingTable" class="table table-bordered table-sm w-100">
            //                <thead class="table-light sticky-top">
            //                    ${buildHeaders(rowData.bookingType)}
            //                </thead>
            //                <tbody></tbody>
            //            </table>
            //        `;
            //                $('#bookingTableWrapper').html(newTable);

            //                // 🔹 Build rows
            //                let tbodyHtml = '';
            //                $.each(resp.data || [], function (i, item) {
            //                    tbodyHtml += buildRow(item, rowData.bookingType, resp.dropdownData);
            //                });
            //                $('#bookingTable tbody').html(tbodyHtml);

            //                // 🔥 Initialize DataTable
            //                let table = $('#bookingTable').DataTable({
            //                    scrollX: true,
            //                    scrollY: '500px',
            //                    scrollCollapse: true,
            //                    paging: false,
            //                    info: false,
            //                    ordering: false,
            //                    autoWidth: false,
            //                    searching: false,
            //                    dom: 'ft',
            //                    destroy: true,
            //                    initComplete: function () {
            //                        applySelect2();
            //                        initializeTooltips();

            //                        // 🔝 Scroll table body to top
            //                        $('.dataTables_scrollBody').scrollTop(0);
            //                    }
            //                });

            //                setTimeout(() => table.columns.adjust().draw(false), 150);
            //            },
            //            error: function (xhr) {
            //                console.error(xhr);
            //            },
            //            complete: function () {
            //                // 🔥 HIDE LOADER (slightly delayed for smoothness)
            //                setTimeout(() => {
            //                    $("#bookingLoader").fadeOut(150);
            //                }, 200);

            //                // 🔝 Page scroll to top
            //                $('html, body').animate({ scrollTop: 0 }, 300);
            //            }
            //        });

            //    }, 400); // ⏱️ delay (300–500ms recommended)
            //});




        }

        // click handler for the bookingOrderNo button
        $(document).on("change", ".row-check-master-check", function () {
            let id = $(this).data("id");
            if (this.checked) {
                selectedBookingOrderId.add(id);
            } else {
                selectedBookingOrderId.delete(id);
            }
        });

        $(document).on("change", "#selectMasterAll", function () {
            let checked = this.checked;

            $(".row-check-master-check").each(function () {
                let id = $(this).data("id");
                $(this).prop("checked", checked);

                if (checked) {
                    selectedBookingOrderId.add(id);
                } else {
                    selectedBookingOrderId.delete(id);
                }
            });
        });

        function getselectedBookingOrderId() {
            if (!window.selectedBookingOrderId) {
                console.warn("selectedIds not initialized");
                return [];
            }
            //window.selectedBookingOrderId.clear();
            let arr = Array.from(window.selectedBookingOrderId);
            console.log("Final Costing ID Array:", arr);

            return arr;
        }


        function getSelectedCostingIds() {
            if (!window.selectedIds) {
                console.warn("selectedIds not initialized");
                return [];
            }

            //window.selectedIds.clear();
            let arr = Array.from(window.selectedIds);

            return arr;
        }




        function populateBookingForm(data) {
            console.log(data)
            $("#BookingOrderEntryBuklSetup_Tc").val(data.tc);
            $("#BookingOrderEntryBuklSetup_BookinOrderNo").val(data.bookingOrderNo);
            $("#BookingOrderEntryBuklSetup_BookinDate")[0]._flatpickr.setDate(data.bookingDate, true);
            $("#MasterPurchaseOrder").val(data.masterPurchaseOrder);
            $("#PoNo").val(data.poNo);
            $("#IntegraJobNo").val(data.integraJobNo);
            $("#BookingOrderEntryBuklSetup_PurchasedOfficer").val(data.purchasedOfficer).trigger("change");
            $("#BookingOrderEntryBuklSetup_Remarks").val(data.remarks);
            $("#BookingOrderEntryBuklSetup_DeliveryDate")[0]._flatpickr.setDate(data.deliveryDate, true);
            $("#BookingOrderEntryBuklSetup_DeliveryAddress").val(data.deliveryAddress);
            $("#BookingOrderEntryBuklSetup_DeliveryMethod").val(data.deliveryMethod).trigger("change");
            $("#BookingOrderEntryBuklSetup_PaymentTerms").val(data.paymentTerms).trigger("change");
            $("#BookingOrderEntryBuklSetup_TermsCondition").val(data.termsCondition).trigger("change");
            $("#BookingOrderEntryBuklSetup_BookingType").val(data.bookingType).trigger("change"); // Select2 safe
            $("#BookingEntryType").val(data.bookingEntryType);
            $("#WarehouseId").val(data.warehouseId);
            if (data.pino) {
                $("#BookingOrderEntryBuklSetup_Pino").val(data.pino).prop('disabled', true);
            }
            //$("#BookingOrderEntryBuklSetup_Pidate").val(data.pidate);
            $("#BookingOrderEntryBuklSetup_Pidate")[0]._flatpickr.setDate(data.pidate, true);
            $("#BookingOrderEntryBuklSetup_Pivalue").val(data.pivalue);
            $("#BookingOrderEntryBuklSetup_PicurrencyId").val(data.picurrencyId).trigger("change");
            $("#Mrbpid").val(data.mrbpid);
            $("#EnterFromPageName").val(data.enterFromPageName);
            $("#PifilePath").val(data.pifilePath);
            $(".showCreateDate").text(data.ldate);
            $(".showModifyDate").text(data.modifyDate);

            // Select2 fields
            $("#BuyerId").val(data.buyerId).trigger("change");
            $("#StyleId").val(data.styleId).trigger("change");
            $("#BookingOrderEntryBuklSetup_SupplierId").val(data.supplierId).trigger("change");
        }


        return {
            getTableData: function () {
                var data = [];
                $('#bookingTable tbody tr').each(function () {
                    var row = {};
                    $(this).find('input, select').each(function (i) {
                        row['field_' + i] = $(this).val();
                    });
                    data.push(row);
                });
                return data;
            },
            getCurrentBookingType: function () { return currentBookingType; },
            refreshTooltips: function () { initializeTooltips(); }
        };






    };
})(jQuery);