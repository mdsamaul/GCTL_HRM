(function ($) {
    $.SALES_Def_Inv_MainItemGroup = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            MainSetup_MainItemID: "#MainSetup_MainItemID",
            MainSetup_MainItemName: "#MainSetup_MainItemName",
            MainSetup_TC: "#MainSetup_TC",
            MainSetup_Description: "#MainSetup_Description",
            SubGroupMainItemID: "#SubGroupMainItemID",
            SubItem_TC: "#SubItem_TC",
            SubItemTwo_TC: "#SubItemTwo_TC",
            SubItem_SubItemID: "#SubItem_SubItemID",
            SubGroupTwoMainDescription: "#SubGroupTwoMainDescription",
            SubItem_SubItemName: "#SubItem_SubItemName",
            SubGroupDescription: "#SubGroupDescription",
            SubItem_Description: "#SubItem_Description",
            SubGroupTwoMainItemID: "#SubGroupTwoMainItemID",
            SubGroupTwoSubItemID: "#SubGroupTwoSubItemID",
            SubGroupTwoDescription: "#SubGroupTwoDescription",
            SubItemTwo_SubItem2ID: "#SubItemTwo_SubItem2ID",
            SubItemTwo_SubItem2Name: "#SubItemTwo_SubItem2Name",
            SubItemTwo_Description: "#SubItemTwo_Description",
            MainItem_TC: "#MainItem_TC",
            ItemNameDescription: "#ItemNameDescription",


            MainItem_BuyerId: "#MainItem_BuyerId",
            MainItem_StyleId: "#MainItem_StyleId",
            ItemInfoMainItemID: "#ItemInfoMainItemID",
            ItemInfoMainDescription: "#ItemInfoMainDescription",
            ItemInfoSubItemID: "#ItemInfoSubItemID",
            ItemInfoSubItem2ID: "#ItemInfoSubItem2ID",
            ItemInfoSub2Description: "#ItemInfoSub2Description",
            MainItem_ItemTypeID: "#MainItem_ItemTypeID",
            MainItem_ItemID: "#MainItem_ItemID",
            MainItem_ItemName: "#MainItem_ItemName",
            ItemInfoSubDescription: "#ItemInfoSubDescription",
            //Description: "#Description",
            MainItem_PrintName: "#MainItem_PrintName",
            MainItem_ItemCode: "#MainItem_ItemCode",
            MainItem_Barcode: "#MainItem_Barcode",
            MainItem_OriginId: "#MainItem_OriginId",
            MainItem_ManufactureId: "#MainItem_ManufactureId",
            MainItem_PackageTypeId: "#MainItem_PackageTypeId",
            MainItem_PackageQuantity: "#MainItem_PackageQuantity",
            MainItem_ItemQuantity: "#MainItem_ItemQuantity",
            MainItem_ItemUnit: "#MainItem_ItemUnit",
            MainItem_ItemPrice: "#MainItem_ItemPrice",
            MainItem_CurrencyId: "#MainItem_CurrencyId",
            MainItem_Discount: "#MainItem_Discount",
            MainItem_TotalAmount: "#MainItem_TotalAmount",
            MainItem_CurrencyId2: "#MainItem_CurrencyId2",
            MainItem_WarrantyStatus: "#MainItem_WarrantyStatus",
            MainItem_WarrantyTime: "#MainItem_WarrantyTime",
            MainItem_WarrantyType: "#MainItem_WarrantyType",
            MainItem_SupplierId: "#MainItem_SupplierId",
            //MainItem_SupplierId: "#MainItem_SupplierId",
            MainItem_BranchID: "#MainItem_BranchID",



            StockLevelManagement_TC: "#StockLevelManagement_TC",
            StockLevelManagement_SLMID: "#StockLevelManagement_SLMID",
            StockLevelManagement_ItemID: "#StockLevelManagement_ItemID",
            StockLevelManagement_WarehouseID: "#StockLevelManagement_WarehouseID",
            StockLevelManagement_InStock: "#StockLevelManagement_InStock",
            StockLevelManagement_StockValue: "#StockLevelManagement_StockValue",
            StockLevelManagement_ReorderLevel: "#StockLevelManagement_ReorderLevel",
            StockLevelManagement_MaxStock: "#StockLevelManagement_MaxStock",
            StockLevelManagement_MinStock: "#StockLevelManagement_MinStock",
            StockLevelManagement_Description: "#StockLevelManagement_Description",


            showCreateModifyDateContainer: ".showCreateModifyDateContainer",
            showCreateDate: ".showCreateDate",
            showModifyDate: ".showModifyDate",

            //changeabe dropdown
            mainGroupDropdownSelect: ".main-group-item-dropdown",
            subGroupDropdownSelect: ".sub-group-item-dropdown",
            subGroup2DropdownSelect: ".sub-group-two-item-dropdown",
            supplierDropdownSelect: ".supplier-dropdown",
            MainItemSupplierAddress: "#MainItemSupplierAddress",

            ItemSaveBtn: ".js-MainI-tem-Group-save",
            ItemDeleteBtn: "#js-MainI-tem-Group-delete-confirm",
            ClearBtn: "#js-MainI-tem-Group-clear",

        }, options);


        var gridUrl = settings.baseUrl + "/grid";
        var saveEditUrl = settings.baseUrl + "/AdddEditSetup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        var loadDropdownUrl = settings.baseUrl + "/LoadDropdowns";
        var mainGroupSaveEditUrl = settings.baseUrl + "/AdddEditMainSetup";
        var subGroupSaveEditUrl = settings.baseUrl + "/AdddEditSubSetup";
        var subGroupTwoSaveEditUrl = settings.baseUrl + "/AdddEditSubTwoSetup";
        var itemInfoGroupSaveEditUrl = settings.baseUrl + "/AdddEditItemInfoSetup";
        var stockLGroupSaveEditUrl = settings.baseUrl + "/StockLevelManagementSetup";

        var LoadMainGroupDataUrl = settings.baseUrl + "/LoadMainGroupData";
        //var LoadSubGroupDataUrl = settings.baseUrl + "/LoadSubGroupData";
        //var LoadSub2GroupDataUrl = settings.baseUrl + "/LoadSub2GroupData";
        //var LoadItemInformationDataUrl = settings.baseUrl + "/LoadItemInformationData";
        //var LoadStockLevelManagementDataUrl = settings.baseUrl + "/LoadStockLevelManagementData";

        var GetAutoAllIdUrl = settings.baseUrl + "/GetAutoAllId";
        var DeleteItemUrl = settings.baseUrl + "/DeleteItemUrl";

        var ChangeAbleDropdownUrl = settings.baseUrl + "/ChangeAbleDropdown";

        $('.searchable-select').select2({        
            placeholder: 'Select an option',
            allowClear: true,
            width: '100%', 
            language: { noResults: () => 'No results found' },
            escapeMarkup: markup => markup
        });
        // Sticky header on scroll
        function stHeader() {
            window.addEventListener('scroll', function () {
                const header = document.getElementById('stickyHeader');

                if (window.scrollY > 550) {
                    header.classList.add('hide-header'); 
                    header.classList.remove('sticky-top');
                }
                else if (window.scrollY > 10) {
                    header.classList.add('sticky-top');
                    header.classList.add('sticky-scrolled');
                    header.classList.remove('hide-header');
                }
                else {
                    header.classList.add('sticky-top');
                    header.classList.remove('sticky-scrolled');
                    header.classList.remove('hide-header');
                }
            });
        }



        // SweetAlert toast message
        function showToast(iconType, message) {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 5000,
                timerProgressBar: true,
                showClass: {
                    popup: 'swal2-show swal2-fade-in'
                },
                hideClass: {
                    popup: 'swal2-hide swal2-fade-out'
                }
            });

            Toast.fire({
                icon: iconType,
                title: message
            });
        }


        $("#MainItem_CurrencyId2").prop('disabled', true).trigger('change');

        function GetTabName() {
            return $('#item-tab button[data-bs-toggle="tab"].active').text().trim();
        }
        $(document).on('click', '#item-tab button[data-bs-toggle="tab"]', function () {
            $(settings.showCreateModifyDateContainer).hide();
            let tabName = $(this).text().trim();
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
            loadDropdowns();
            if (tabName == "Main Group") {
                GetAutoAllI(tabName);
                loadMianGrid();
                $(".GroupTitle").fadeOut(200, function () {
                    $(this).text("Product / Item Setup ( Main Group )").fadeIn(200);
                });

            }
            else if (tabName == "Sub Group") {
                GetAutoAllI(tabName);               
                $(".GroupTitle").fadeOut(200, function () {
                    $(this).text("Product / Item Setup ( Sub Group )").fadeIn(200);
                });

            }
            else if (tabName == "Sub Group - 2") {
                GetAutoAllI(tabName);                
                $(".GroupTitle").fadeOut(200, function () {
                    $(this).text("Product Information ( Sub Group - 2 )").fadeIn(200);
                });

            }
            else if (tabName == "Item Information") {
                GetAutoAllI(tabName);               
                $(".GroupTitle").fadeOut(200, function () {
                    $(this).text("Item Setup ( Item Information )").fadeIn(200);
                });

            }
            else if (tabName == "Stock Level Management") {
                GetAutoAllI(tabName);
                $(".GroupTitle").fadeOut(200, function () {
                    $(this).text("Stock Level Management").fadeIn(200);
                });

            }
        });

        function loadDropdowns() {
            $.ajax({
                url: loadDropdownUrl,
                type: "GET",
                success: function (res) {

                    let tabName = GetTabName();
                    if (tabName == "Sub Group") {
                        let $mainItem = $("#SubGroupMainItemName");
                        $mainItem.empty();
                        $mainItem.append('<option></option>');
                        $.each(res.mainGroup, function (i, item) {
                            $mainItem.append(`<option value="${item.mainItemId}">${item.mainItemName}</option>`);
                        });
                    }
                    else if (tabName == "Sub Group - 2") {
                        let $sub2mainItem = $("#SubGroupTwoMainItemName");
                        $sub2mainItem.empty();
                        $sub2mainItem.append('<option></option>');
                        $.each(res.mainGroup, function (i, item) {
                            $sub2mainItem.append(`<option value="${item.mainItemId}">${item.mainItemName}</option>`);
                        });
                        let $sub2SubItem = $("#SubGroupTwoSubGroupName");
                        $sub2SubItem.empty();
                        $sub2SubItem.append('<option></option>');
                        $.each(res.subGroup, function (i, item) {
                            $sub2SubItem.append(`<option value="${item.subItemId}">${item.subItemName}</option>`);
                        });
                    }
                    else if (tabName == "Item Information") {
                        let $sub2mainItem = $("#ItemInfoMainItemName");
                        $sub2mainItem.empty();
                        $sub2mainItem.append('<option></option>');
                        $.each(res.mainGroup, function (i, item) {
                            $sub2mainItem.append(`<option value="${item.mainItemId}">${item.mainItemName}</option>`);
                        });
                        let $sub2SubItem = $("#ItemInfoSubItemName");
                        $sub2SubItem.empty();
                        $sub2SubItem.append('<option></option>');
                        $.each(res.subGroup, function (i, item) {
                            $sub2SubItem.append(`<option value="${item.subItemId}">${item.subItemName}</option>`);
                        });
                        let $sub2SubItem2 = $("#ItemInfoSubItem2Name");
                        $sub2SubItem2.empty();
                        $sub2SubItem2.append('<option></option>');
                        $.each(res.subGroup2, function (i, item) {
                            $sub2SubItem2.append(`<option value="${item.subItem2Id}">${item.subItem2Name}</option>`);
                        });
                    } else if (tabName == "Stock Level Management") {
                        let $item = $("#StockLevelManagement_ItemID");
                        $item.empty();
                        $item.append('<option></option>');
                        $.each(res.item, function (i, item) {
                            $item.append(`<option value="${item.itemId}">${item.itemName}</option>`);
                        });
                    }
                }
            });
        }

        $(document).on('click', settings.ClearBtn, function () {
            $(settings.showCreateModifyDateContainer).hide();
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
            let tabName = GetTabName();
            GetAutoAllI(tabName);
            if (tabName == "Main Group") {
                ResetMainForm();
            } else if (tabName == "Sub Group") {
                ResetSubGroupForm();
            } else if (tabName == "Sub Group - 2") {
                ResetSubGroupTwoForm();
            } else if (tabName == "Item Information") {
                ResetItemInfoGroupForm();
            } else if (tabName == "Stock Level Management") {
                ResetStockLevelForm();
            }
        })
        $(document).on('click', settings.ItemSaveBtn, function () {
            $(settings.ItemSaveBtn).prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Saving...');
            let tabName = GetTabName();
            if (tabName == "Main Group") {
                MainGroup();
            } else if (tabName == "Sub Group") {
                SubGroup();
            } else if (tabName == "Sub Group - 2") {
                SubGroupTwo();
            } else if (tabName == "Item Information") {
                ItemInformationGroup();
            } else if (tabName == "Stock Level Management") {
                StockLevelGroup();
            }
        })
        function GetMainForm() {
            formValue = {
                TC: $(settings.MainSetup_TC).val(),
                MainItemID: $(settings.MainSetup_MainItemID).val(),
                MainItemName: $(settings.MainSetup_MainItemName).val(),
                Description: $(settings.MainSetup_Description).val(),
                TabName: ""
            }
            return formValue;
        }
        function ResetMainForm() {
            $(settings.MainSetup_TC).val(0);
            $(settings.MainSetup_MainItemID).val('');
            $(settings.MainSetup_MainItemName).val('');
            $(settings.MainSetup_Description).val('');
            $('#SubGroupDescription').val('');
            //$(settings.showCreateDate).empty();
            //$(settings.showModifyDate).empty();
        }
        function ResetCreateModifyDate() {
            $(settings.showCreateDate).empty();
            $(settings.showModifyDate).empty();
            $(settings.showCreateModifyDateContainer).hide();
        }


        function MainGroup() {
            let mainFormData = GetMainForm();
            if (!mainFormData.MainItemName) {
                $('#MainSetup_MainItemName').addClass('border border-danger').focus();
                $(settings.ItemSaveBtn).prop('disabled', true).html('<i class="fa fa-save">&nbsp;</i> Save');
                return;
            }

            AllPostFacth(mainGroupSaveEditUrl, mainFormData);
        }
        $(document).on('input', $('#MainSetup_MainItemName'), function () {
            $('#MainSetup_MainItemName').removeClass('border border-danger');
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
        })
        //sub group

        function SubGroupForm() {
            formValue = {
                TC: $(settings.SubItem_TC).val(),
                SubItemID: $(settings.SubItem_SubItemID).val(),
                SubItemName: $(settings.SubItem_SubItemName).val(),
                Description: $(settings.SubItem_Description).val(),
                MainItemID: $(settings.SubGroupMainItemID).val()
            }
            return formValue;
        }
        function ResetSubGroupForm() {
            $(settings.SubItem_TC).val(0);
            $(settings.SubGroupMainItemID).val('');
            $(settings.SubItem_SubItemName).val('');
            $(settings.SubItem_Description).val('');
            $("#SubGroupMainItemName").val('').trigger('change');
            $("#SubGroupDescription").val('');
        }


        function SubGroup() {
            let subFormdata = SubGroupForm();
            if (!subFormdata.MainItemID) {
                $('#SubGroupMainItemName').select2('open');
                $(settings.ItemSaveBtn).prop('disabled', true).html('<i class="fa fa-save">&nbsp;</i> Save');
                return;
            }
            if (!subFormdata.SubItemName) {
                $('#SubItem_SubItemName').addClass('border border-danger').focus();
                $(settings.ItemSaveBtn).prop('disabled', true).html('<i class="fa fa-save">&nbsp;</i> Save');
                return;
            }

            AllPostFacth(subGroupSaveEditUrl, SubGroupForm());
        }
        $(document).on('change', $('#SubGroupMainItemName'), function () {
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
        })
        $(document).on('input', $('#SubItem_SubItemName'), function () {
            $('#SubItem_SubItemName').removeClass('border border-danger');
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
        })
        function SubGroupTwoForm() {
            formValue = {
                TC: $(settings.SubItemTwo_TC).val(),
                SubItem2ID: $(settings.SubItemTwo_SubItem2ID).val(),
                SubItem2Name: $(settings.SubItemTwo_SubItem2Name).val(),
                Description: $(settings.SubItemTwo_Description).val(),
                MainItemID: $(settings.SubGroupTwoMainItemID).val(),
                SubItemID: $(settings.SubGroupTwoSubItemID).val()
            }
            return formValue;
        }
        function ResetSubGroupTwoForm() {

            $(settings.SubItemTwo_TC).val(0);
            $(settings.SubItemTwo_SubItem2ID).val('');
            $(settings.SubItemTwo_SubItem2Name).val('');
            $(settings.SubItemTwo_Description).val('');
            $(settings.SubGroupTwoMainItemID).val('');
            $(settings.SubGroupTwoSubItemID).val('');
            $("#SubGroupTwoMainItemName").val('').trigger('change');
            $("#SubGroupTwoSubGroupName").val('').trigger('change');
            $("#SubGroupTwoMainDescription").val('');
            $("#SubGroupTwoDescription").val('');

        }


        function SubGroupTwo() {
            let subTowData = SubGroupTwoForm();
            if (!subTowData.MainItemID) {
                $('#SubGroupTwoMainItemName').select2('open');
                $(settings.ItemSaveBtn).prop('disabled', true).html('<i class="fa fa-save">&nbsp;</i> Save');
                return;
            }
            if (!subTowData.SubItemID) {
                $('#SubGroupTwoSubGroupName').select2('open');
                $(settings.ItemSaveBtn).prop('disabled', true).html('<i class="fa fa-save">&nbsp;</i> Save');
                return;
            }
            if (!subTowData.SubItem2Name) {
                $('#SubItemTwo_SubItem2Name').addClass('border border-danger').focus();
                $(settings.ItemSaveBtn).prop('disabled', true).html('<i class="fa fa-save">&nbsp;</i> Save');
                return;
            }
            AllPostFacth(subGroupTwoSaveEditUrl, SubGroupTwoForm());
        }

        $(document).on('change', $('#SubGroupTwoMainItemName'), function () {
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
        })
        $(document).on('change', $('#SubGroupTwoSubGroupName'), function () {
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
        })
        $(document).on('input', $('#SubItemTwo_SubItem2Name'), function () {
            $('#SubItemTwo_SubItem2Name').removeClass('border border-danger');
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
        })
        function ItemInfoGroupForm() {
            let formValue = {
                TC: $(settings.MainItem_TC).val() || 0,
                ItemID: $(settings.MainItem_ItemID).val() || "",
                BuyerId: $(settings.MainItem_BuyerId).val() || "",
                MasterPurchaseOrder: $(settings.ItemInfoMainItemID).val() || "",
                StyleId: $(settings.MainItem_StyleId).val() || "",
                ItemName: $(settings.MainItem_ItemName).val() || "",
                PrintName: $(settings.MainItem_PrintName).val() || "",
                TechnicalSpecification: $(settings.ItemNameDescription).val() || "",
                ItemCode: $(settings.MainItem_ItemCode).val() || "",
                Barcode: $(settings.MainItem_Barcode).val() || "",
                OriginId: $(settings.MainItem_OriginId).val() || "",
                ManufactureId: $(settings.MainItem_ManufactureId).val() || "",
                PackageTypeId: $(settings.MainItem_PackageTypeId).val() || "",
                PackageQuantity: $(settings.MainItem_PackageQuantity).val() || 0,
                ItemQuantity: $(settings.MainItem_ItemQuantity).val() || 0,
                ItemUnit: $(settings.MainItem_ItemUnit).val() || "",
                ItemPrice: $(settings.MainItem_ItemPrice).val() || 0,
                CurrencyId: $(settings.MainItem_CurrencyId).val() || "",
                TotalAmount: $(settings.MainItem_TotalAmount).val() || 0,
                CurrencyId2: $(settings.MainItem_CurrencyId2).val() || "",
                Discount: $(settings.MainItem_Discount).val() || 0,
                WarrantyStatus: $(settings.MainItem_WarrantyStatus).val() || "",
                WarrantyTime: $(settings.MainItem_WarrantyTime).val() || 0,
                WarrantyType: $(settings.MainItem_WarrantyType).val() || "",
                BranchID: $(settings.MainItem_BranchID).val() || "",
                SupplierId: $(settings.MainItem_SupplierId).val() || "",
                ItemTypeID: $(settings.MainItem_ItemTypeID).val() || "",
                SubItem2ID: $(settings.ItemInfoSubItem2ID).val() || "",
                MainItemID: $(settings.ItemInfoMainItemID).val() || "",
                SubItemID: $(settings.ItemInfoSubItemID).val() || ""
            };

            return formValue;
        }

        function ResetItemInfoGroupForm() {

            $(settings.MainItem_TC).val(0);
            //$(settings.MainItem_ItemID).val('');
            $(settings.MainItem_BuyerId).val('').trigger('change');
            $(settings.ItemInfoMainItemID).val('');
            $(settings.MainItem_ItemName).val('');
            $('#ItemNameDescription').val('');
            $(settings.MainItem_StyleId).val('').trigger('change');
            $('#ItemInfoMainItemName').val('').trigger('change');
            $('#ItemInfoSubItemName').val('').trigger('change');
            $('#ItemInfoSubItem2Name').val('').trigger('change');
            $(settings.MainItem_PrintName).val('');
            $(settings.MainItem_ItemCode).val('');
            $(settings.MainItem_Barcode).val('');
            $(settings.MainItem_OriginId).val('').trigger('change');
            $(settings.MainItem_ManufactureId).val('').trigger('change');
            $(settings.MainItem_PackageTypeId).val('').trigger('change');
            $(settings.MainItem_PackageQuantity).val('');
            $(settings.MainItem_ItemQuantity).val('');
            $(settings.MainItem_ItemUnit).val('').trigger('change');
            $(settings.MainItem_ItemPrice).val('');
            $(settings.MainItem_CurrencyId).val('').trigger('change');
            $(settings.MainItem_TotalAmount).val('');
            $(settings.MainItem_CurrencyId2).val('').trigger('change');
            $(settings.MainItem_Discount).val('');
            $(settings.MainItem_WarrantyStatus).prop('checked', false);
            $(settings.MainItem_WarrantyTime).val('');
            $(settings.MainItem_WarrantyType).val('').trigger('change');
            $(settings.MainItem_BranchID).val('').trigger('change');
            $(settings.MainItem_SupplierId).val('').trigger('change');
            $(settings.MainItem_ItemTypeID).val('').trigger('change');
            $(settings.ItemInfoSubItem2ID).val('');
            $(settings.ItemInfoMainItemID).val('');
            $(settings.ItemInfoSubItemID).val('');
            $('#ItemInfoSub2Description').val('');
            $('#ItemInfoSubDescription').val('');
            $('#ItemInfoMainDescription').val('');
            $('#MainItemSupplierAddress').val('');

        }

        $(document).on('input', settings.MainItem_ItemID, function () {
            $(settings.MainItem_ItemID).removeClass('border border-danger');
        })
        $(document).on('input', settings.MainItem_ItemName, function () {
            $(settings.MainItem_ItemName).removeClass('border border-danger');
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
        })

        $(document).on('change', '#ItemInfoMainItemName, #ItemInfoSubItemName, #ItemInfoSubItem2Name, #MainItem_ItemName', function () {
            let dataMain = $('#ItemInfoMainItemName').select2('data');
            let dataSub = $('#ItemInfoSubItemName').select2('data');
            let dataSub2 = $('#ItemInfoSubItem2Name').select2('data');
            let itemName = $('#MainItem_ItemName').val() || "";

            let parts = [];

            if (dataMain && dataMain.length > 0 && dataMain[0].text.trim() !== "") {
                parts.push(dataMain[0].text);
            }
            if (dataSub && dataSub.length > 0 && dataSub[0].text.trim() !== "") {
                parts.push(dataSub[0].text);
            }
            if (dataSub2 && dataSub2.length > 0 && dataSub2[0].text.trim() !== "") {
                parts.push(dataSub2[0].text);
            }
            if (itemName.trim() !== "") {
                parts.push(itemName);
            }

            let printName = parts.join(" - ");
            $(settings.MainItem_PrintName).val(printName);
        });

        // Item Quantity validation
        $(document).on('input', settings.MainItem_ItemQuantity, function () {
            let IQ = parseFloat($(settings.MainItem_ItemQuantity).val()) || 0;
            if (IQ <= 0) {
                $(settings.MainItem_ItemQuantity).addClass('border border-danger');
                $(settings.ItemSaveBtn).prop('disabled', true);
            } else {
                $(settings.MainItem_ItemQuantity).removeClass('border border-danger');
                $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
                calculateTotalAmount(settings);
            }
        });

        // Item Price validation
        $(document).on('input', settings.MainItem_ItemPrice, function () {
            let price = parseFloat($(settings.MainItem_ItemPrice).val()) || 0;
            if (price <= 0) {
                $(settings.MainItem_ItemPrice).addClass('border border-danger');
                $(settings.ItemSaveBtn).prop('disabled', true);
            } else {
                $(settings.MainItem_ItemPrice).removeClass('border border-danger');
                $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
                calculateTotalAmount(settings);
            }
        });

        // Discount validation
        $(document).on('input', settings.MainItem_Discount, function () {
            let discount = parseFloat($(this).val()) || 0;
            if (discount >= 0 && discount <= 100) {
                $(settings.MainItem_Discount).removeClass('border border-danger');
                $(settings.ItemSaveBtn).prop('disabled', false);
                calculateTotalAmount(settings);
            } else {
                $(settings.MainItem_Discount).addClass('border border-danger');
                $(settings.ItemSaveBtn).prop('disabled', true);
                showToast('warning', 'Discount must be between 0 and 100.');
            }
        });

        // Common function to calculate and set total amount
        function calculateTotalAmount(settings) {
            let IQ = parseFloat($(settings.MainItem_ItemQuantity).val()) || 0;
            let UP = parseFloat($(settings.MainItem_ItemPrice).val()) || 0;
            let Discount = parseFloat($(settings.MainItem_Discount).val()) || 0;

            if (IQ > 0 && UP > 0) {
                let TA = IQ * UP;
                if (Discount > 0 && Discount <= 100) {
                    TA = TA - (TA * (Discount / 100));
                }
                $(settings.MainItem_TotalAmount).val(TA.toFixed(2)).prop('disabled', true);
            } else {
                $(settings.MainItem_TotalAmount).val("").prop('disabled', true);
            }
        }

        // Item currency validation
        $(document).on('change', settings.MainItem_CurrencyId, function () {
            let currency = $(this).val();
            $(settings.MainItem_CurrencyId2).val(currency).trigger('change');
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
        });
        // Item currency validation
        $(document).on('change', settings.MainItem_ItemUnit, function () {
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
        });


        function ItemInformationGroup() {
            var itemValue = ItemInfoGroupForm();
            //debugger
            if (!itemValue.MainItemID) {
                showToast('error', "Main Item required");
                $('#ItemInfoMainItemName').select2('open');
                return;
            }
            if (!itemValue.ItemID) {
                showToast('error', "Item Id required");
                $(settings.MainItem_ItemID).addClass('border border-danger').focus();
                return;
            }
            if (!itemValue.ItemName) {
                showToast('error', "Item Name required");
                $(settings.MainItem_ItemName).addClass('border border-danger').focus();
                return;
            }
            if (!itemValue.ItemQuantity || itemValue.ItemQuantity <= 0) {
                showToast('error', "Item Quantity required");
                $(settings.MainItem_ItemQuantity).addClass('border border-danger').focus();
                return;
            }
            if (!itemValue.ItemPrice || itemValue.ItemPrice <= 0) {
                showToast('error', "Item Price required");
                $(settings.MainItem_ItemPrice).addClass('border border-danger').focus();
                return;
            }
            if (!itemValue.ItemUnit || itemValue.ItemUnit <= 0) {
                showToast('error', "Item Unit Type required");
                // select2 dropdown open + focus
                $(settings.MainItem_ItemUnit).select2('open');
                return;
            }
            if (!itemValue.CurrencyId) {
                showToast('error', "Item Currency required");
                // select2 dropdown open + focus
                $(settings.MainItem_CurrencyId).select2('open');
                return;
            }



            AllPostFacth(itemInfoGroupSaveEditUrl, ItemInfoGroupForm());
        }
        //stock level
        function StockLevelForm() {
            let formValue = {
                TC: $(settings.StockLevelManagement_TC).val() || 0,
                SLMID: $(settings.StockLevelManagement_SLMID).val() || "",
                ItemID: $(settings.StockLevelManagement_ItemID).val() || "",
                WarehouseID: $(settings.StockLevelManagement_WarehouseID).val() || "",
                InStock: $(settings.StockLevelManagement_InStock).val() || 0,
                StockValue: $(settings.StockLevelManagement_StockValue).val() || 0,
                ReorderLevel: $(settings.StockLevelManagement_ReorderLevel).val() || 0,
                MaxStock: $(settings.StockLevelManagement_MaxStock).val() || 0,
                MinStock: $(settings.StockLevelManagement_MinStock).val() || 0,
                Description: $(settings.StockLevelManagement_Description).val() || ""
            };

            return formValue;
        }

        function ResetStockLevelForm() {

            $(settings.StockLevelManagement_TC).val(0);
            //$(settings.StockLevelManagement_SLMID).val('');
            $(settings.StockLevelManagement_ItemID).val('').trigger('change');
            $(settings.StockLevelManagement_WarehouseID).val('').trigger('change');
            $(settings.StockLevelManagement_InStock).val('');
            $(settings.StockLevelManagement_StockValue).val('');
            $(settings.StockLevelManagement_ReorderLevel).val('');
            $(settings.StockLevelManagement_MaxStock).val('');
            $(settings.StockLevelManagement_MinStock).val('');
            $(settings.StockLevelManagement_Description).val('');

        }
        $(document).on('change', settings.StockLevelManagement_ItemID, function () {
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
        })
        $(document).on('change', settings.StockLevelManagement_WarehouseID, function () {
            $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
        })
        // InStock Validation
        $(document).on('input', '#StockLevelManagement_InStock', function () {
            let value = Number($('#StockLevelManagement_InStock').val());

            if (value <= 0 || isNaN(value)) {
                $('#StockLevelManagement_InStock').addClass('border border-danger');
                $(settings.ItemSaveBtn).prop('disabled', true).html('<i class="fa fa-save">&nbsp;</i> Save');
            } else {
                $('#StockLevelManagement_InStock').removeClass('border border-danger');
                $(settings.ItemSaveBtn).prop('disabled', false).html('<i class="fa fa-save">&nbsp;</i> Save');
            }
        });

        // StockValue Validation
        $(document).on('input', '#StockLevelManagement_StockValue', function () {
            let value = Number($('#StockLevelManagement_StockValue').val());

            if (value <= 0 || isNaN(value)) {
                $('#StockLevelManagement_StockValue').addClass('border border-danger');
                $(settings.ItemSaveBtn)
                    .prop('disabled', true)
                    .html('<i class="fa fa-save">&nbsp;</i> Save');
            } else {
                $('#StockLevelManagement_StockValue').removeClass('border border-danger');
                $(settings.ItemSaveBtn)
                    .prop('disabled', false)
                    .html('<i class="fa fa-save">&nbsp;</i> Save');
            }
        });

        function StockLevelGroup() {
            var stockValue = StockLevelForm();
            // Validation before submit
            if (!stockValue.ItemID) {
                $(settings.StockLevelManagement_ItemID).select2('open');
                showToast('warning', "Please select an Item before saving.");
                return;
            }

            if (!stockValue.WarehouseID) {
                $(settings.StockLevelManagement_WarehouseID).select2('open');
                showToast('warning', "Please select a Warehouse.");
                return;
            }

            if (!stockValue.InStock || stockValue.InStock <= 0) {
                $(settings.StockLevelManagement_InStock)
                    .addClass('border border-danger')
                    .focus();
                showToast('warning', "In Stock quantity must be greater than 0.");
                return;
            }

            if (!stockValue.StockValue || stockValue.StockValue <= 0) {
                $(settings.StockLevelManagement_StockValue)
                    .addClass('border border-danger')
                    .focus();
                showToast('warning', "Stock Value must be greater than 0.");
                return;
            }

            AllPostFacth(stockLGroupSaveEditUrl, StockLevelForm());
        }

        function AllPostFacth(saveEditUrl, data) {

            $.ajax({
                url: saveEditUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (res) {
                    if (res.isSuccess) {
                        let tabName = GetTabName();
                        GetAutoAllI(tabName);
                        showToast('success', res.message);
                        $("#previewImg").attr("src", "").addClass("d-none");
                        $("#photoInput").val("");
                        $("#removeBtn").addClass("d-none");
                    } else {
                        showToast('error', res.message);
                    }

                }, error: function (e) {

                },
                complete: function () {
                    $(settings.ItemSaveBtn).prop('disabled', false).html('Save');
                }

            });
        }


        function GetAutoAllI(tabName) {

            $.ajax({
                url: GetAutoAllIdUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(tabName),
                success: function (autoId) {
                    ResetCreateModifyDate();
                    let tabName = GetTabName();
                    if (tabName == "Main Group") {
                        ResetMainForm();
                        $(settings.MainSetup_MainItemID).val(autoId);
                        loadMianGrid();
                    } else if (tabName == "Sub Group") {
                        ResetSubGroupForm();
                        $(settings.SubItem_SubItemID).val(autoId);
                        //loadSubGrid();
                    } else if (tabName == "Sub Group - 2") {
                        ResetSubGroupTwoForm();
                        $(settings.SubItemTwo_SubItem2ID).val(autoId);
                        
                    } else if (tabName == "Item Information") {
                        ResetItemInfoGroupForm()
                        $(settings.MainItem_ItemID).val(autoId);
                       
                    } else if (tabName == "Stock Level Management") {
                        $(settings.StockLevelManagement_SLMID).val(autoId);
                        
                        ResetStockLevelForm();
                    }
                }, error: function (e) {
                    ;
                }
            });
        }

        $(document).on('click', settings.ItemDeleteBtn, function () {
            if (confirm("Are you sure you want to delete this item?")) {
                DeleteItem(GetTabName());
            }
        });

        var checkedIdGroups = [];
        function DeleteItem(tabName) {
            $.ajax({
                url: DeleteItemUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify({ tabName: tabName, ItemIdList: checkedIdGroups }),
                success: function (res) {

                    let tabName = GetTabName();
                    ResetMainForm();
                    GetAutoAllI(tabName);
                    if (tabName == "Main Group") {
                        loadMianGrid();
                    } else if (tabName == "Sub Group") {
                        //loadSubGrid();
                    } else if (tabName == "Sub Group - 2") {
                        
                    } else if (tabName == "Item Information") {
                       
                    } else if (tabName == "Stock Level Management") {
                        
                    }
                    //loadMianGrid();                   
                }, error: function (e) {
                    ;
                    checkedIdGroups = [];
                }, complete: function () {
                    checkedIdGroups = [];
                    $('#itemInfo-check-all').prop('checked', false);
                    $('.itemInfo-group-row-check').prop('checked', false);
                }
            });
        }

        //main group
        function loadMianGrid() {

            if ($.fn.DataTable.isDataTable('#mainGroupGrid')) {
                $('#mainGroupGrid').DataTable().destroy();
            }

            $("#mainGroupGrid").DataTable({
                processing: true,
                serverSide: true,
                searching: true,
                paging: true,
                info: true,
                autoWidth: false,
                responsive: true,
                pageLength: 10,
                lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
                order: [[1, 'asc']],
                ajax: {
                    url: LoadMainGroupDataUrl,
                    type: "POST",
                    dataType: "json",
                    "dataSrc": function (json) {
                        return json.data || [];
                    },
                    error: function (xhr, error, code) {
                    }
                },
                columns: [
                    {
                        data: "tc",
                        render: function (data, type, row) {
                            if (type === 'display') {
                                return `<input type="checkbox" class="main-group-row-check" value="${data}"/>`;
                            }
                            return data;
                        },
                        orderable: false,
                        searchable: false,
                        width: "20px",
                        className: "text-center"
                    },
                    {
                        data: "mainItemID",
                        render: function (data, type, row) {
                            if (type === 'display') {
                                return `<a href="#" class="main-group-item-link" data-id="${row.tc}">${data}</a>`;
                            }
                            return data;
                        },
                        width: "50px",
                        className: "text-center"
                    },
                    {
                        data: "mainItemName",
                        width: "200px"
                    },
                    {
                        data: "description",
                        width: "300px"
                    }
                ]
            });
        }
        // Event handlers
        $(document).on('click', '.main-group-item-link', function (e) {
            e.preventDefault();
            //var id = $(this).data('id');

            var table = $('#mainGroupGrid').DataTable();
            var row = table.row($(this).closest('tr'));

            var rowValue = row.data();
            $(settings.MainSetup_TC).val(rowValue.tc);
            $(settings.MainSetup_MainItemID).val(rowValue.mainItemID);
            $(settings.MainSetup_MainItemName).val(rowValue.mainItemName);
            $(settings.MainSetup_Description).val(rowValue.description);
            $(settings.showCreateModifyDateContainer).show();
            $(settings.showCreateDate).empty().text(rowValue.showCreateDate || "");
            $(settings.showModifyDate).empty().text(rowValue.showModifyDate || "");


            $('#mainGroup-check-all').prop('checked', false);
            $('.main-group-row-check').prop('checked', false);


            checkedIdGroups = [];
            checkedIdGroups.push(rowValue.tc + '');
        });

        $(document).on('change', '#mainGroup-check-all', function () {
            var isChecked = $(this).prop('checked');
            $('.main-group-row-check').prop('checked', isChecked);

            checkedIdGroups = $('.main-group-row-check:checked').map(function () {
                return $(this).val();
            }).get();
        });

        $(document).on('change', '.main-group-row-check', function () {
            var checkedCount = $('.main-group-row-check:checked').length;
            var totalCount = $('.main-group-row-check').length;
            $('#mainGroup-check-all').prop('checked', checkedCount === totalCount);
            checkedIdGroups = $('.main-group-row-check:checked').map(function () {
                return $(this).val();
            }).get();
        });




        //sub group

        // Event handlers
        $(document).on('click', '.sub-group-item-link', function (e) {
            e.preventDefault();
            var table = $('#subGroupGrid').DataTable();
            var row = table.row($(this).closest('tr'));

            var rowValue = row.data();
            $(settings.SubItem_TC).val(rowValue.tc);
            $('#SubGroupMainItemName').val(rowValue.mainItemID).trigger('change');
            $(settings.SubItem_SubItemID).val(rowValue.subItemID);
            $(settings.SubItem_SubItemName).val(rowValue.subItemName);
            $(settings.SubItem_Description).val(rowValue.description);
            $(settings.showCreateModifyDateContainer).show();//todo
            $(settings.showCreateDate).empty().text(rowValue.showCreateDate || "");
            $(settings.showModifyDate).empty().text(rowValue.showModifyDate || "");


            $('#subGroup-check-all').prop('checked', false);
            $('.sub-group-row-check').prop('checked', false);


            checkedIdGroups = [];
            checkedIdGroups.push(rowValue.tc + '');
        });

        $(document).on('change', '#subGroup-check-all', function () {
            var isChecked = $(this).prop('checked');
            $('.sub-group-row-check').prop('checked', isChecked);

            checkedIdGroups = $('.sub-group-row-check:checked').map(function () {
                return $(this).val();
            }).get();
        });

        $(document).on('change', '.sub-group-row-check', function () {
            var checkedCount = $('.sub-group-row-check:checked').length;
            var totalCount = $('.sub-group-row-check').length;
            $('#subGroup-check-all').prop('checked', checkedCount === totalCount);
            checkedIdGroups = $('.sub-group-row-check:checked').map(function () {
                return $(this).val();
            }).get();
        });


        //sub group 2 

        // Event handlers
        $(document).on('click', '.sub2-group-item-link', function (e) {
            e.preventDefault();         

            var table = $('#subGroup2Grid').DataTable();
            var row = table.row($(this).closest('tr'));

            var rowValue = row.data();
            if (rowValue) {

                $(settings.SubItemTwo_TC).val(rowValue.tc);
                $(settings.SubItemTwo_SubItem2ID).val(rowValue.subItem2ID);
                $(settings.SubItemTwo_SubItem2Name).val(rowValue.subItem2Name);
                $(settings.SubItemTwo_Description).val(rowValue.description);
                $('#SubGroupTwoMainItemName').val(rowValue.mainItemID).trigger('change');
                setTimeout(() => {
                    $('#SubGroupTwoSubGroupName').val(rowValue.subItemID).trigger('change');
                }, 500);
                
                $(settings.showCreateModifyDateContainer).show();
                $(settings.showCreateDate).empty().text(rowValue.showCreateDate || "");
                $(settings.showModifyDate).empty().text(rowValue.showModifyDate || "");

                $('#sub2Group-check-all').prop('checked', false);
                $('.sub2-group-row-check').prop('checked', false);

               
                checkedIdGroups = [];
                checkedIdGroups.push(rowValue.tc + '');
            }
        });

        $(document).on('change', '#sub2Group-check-all', function () {
            var isChecked = $(this).prop('checked');
            $('.sub2-group-row-check').prop('checked', isChecked);

            checkedIdGroups = $('.sub2-group-row-check:checked').map(function () {
                return $(this).val();
            }).get();
        });

        $(document).on('change', '.sub2-group-row-check', function () {
            var checkedCount = $('.sub2-group-row-check:checked').length;
            var totalCount = $('.main-group-row-check').length;
            $('#sub2Group-check-all').prop('checked', checkedCount === totalCount);
            checkedIdGroups = $('.sub2-group-row-check:checked').map(function () {
                return $(this).val();
            }).get();
        });


        //item information group 2 

        // Event handlers
        $(document).on('click', '.itemInfo-group-item-link', function (e) {
            e.preventDefault();

            var table = $('#itemInformationGrid').DataTable();

            var $tr = $(this).closest('tr');
            if ($tr.hasClass('child')) {
                $tr = $tr.prev(); 
            }

            var row = table.row($tr);
            var rowValue = row.data();

            if (!rowValue) {
                return;
            }

            $("#MainItem_TC").val(rowValue.tc);
            $(settings.MainItem_ItemID).val(rowValue.itemID);
            $(settings.MainItem_BuyerId).val(rowValue.buyerId).trigger('change');
            $(settings.MainItem_StyleId).val(rowValue.styleId).trigger('change');
            $(settings.MainItem_ItemName).val(rowValue.itemName);
            $(settings.MainItem_PrintName).val(rowValue.printName);
            $(settings.ItemNameDescription).val(rowValue.technicalSpecification || "");
            $(settings.MainItem_ItemCode).val(rowValue.itemCode);
            $(settings.MainItem_Barcode).val(rowValue.barcode);
            $(settings.MainItem_OriginId).val(rowValue.originId).trigger('change');
            $(settings.MainItem_ManufactureId).val(rowValue.manufactureId).trigger('change');
            $(settings.MainItem_PackageTypeId).val(rowValue.packageTypeId).trigger('change');
            $(settings.MainItem_PackageQuantity).val(rowValue.packageQuantity);
            $(settings.MainItem_ItemQuantity).val(rowValue.itemQuantity);
            $(settings.MainItem_ItemUnit).val(rowValue.itemUnit).trigger('change');
            $(settings.MainItem_ItemPrice).val(rowValue.itemPrice);
            $(settings.MainItem_CurrencyId).val(rowValue.currencyId).trigger('change');
            $(settings.MainItem_TotalAmount).val(rowValue.totalAmount);
            $(settings.MainItem_CurrencyId2).val(rowValue.currencyId2).trigger('change');
            $(settings.MainItem_Discount).val(rowValue.discount);
            $(settings.MainItem_WarrantyStatus).prop('checked', rowValue.warrantyStatus);
            $(settings.MainItem_WarrantyTime).val(rowValue.warrantyTime);
            $(settings.MainItem_WarrantyType).val(rowValue.warrantyType).trigger('change');
            $(settings.MainItem_BranchID).val(rowValue.branchID).trigger('change');
            $(settings.MainItem_SupplierId).val(rowValue.supplierId).trigger('change');
            $(settings.MainItem_ItemTypeID).val(rowValue.itemTypeID).trigger('change');

            $('#ItemInfoMainItemID').val(rowValue.mainItemID).trigger('change');
            $('#ItemInfoSubItemID').val(rowValue.subItemID).trigger('change');
            $('#ItemInfoSubItem2ID').val(rowValue.subItem2ID).trigger('change');

            // Dropdown chaining set with delay
         
            $('#ItemInfoMainItemName').val(rowValue.mainItemID).trigger('change.select2');
            $('#ItemInfoSubItemName').val(rowValue.subItemID).trigger('change.select2');
            $('#ItemInfoSubItem2Name').val(rowValue.subItem2ID).trigger('change.select2');

         

            // Show creation and modification dates
            $(settings.showCreateDate).empty().text(rowValue.showCreateDate || "");
            $(settings.showModifyDate).empty().text(rowValue.showModifyDate || "");
            $(settings.showCreateModifyDateContainer).show();

            getImage(rowValue.itemID);
            $('#itemInfo-check-all').prop('checked', false);
            $('.itemInfo-group-row-check').prop('checked', false);


            
            var rowValue = row.data();
            checkedIdGroups = [];
            checkedIdGroups.push(rowValue.tc+'');
        });

        function getImage(itemId) {
            $.ajax({
                url: '/SALES_Def_Inv_MainItemGroup/GetPhoto',
                type: 'GET',
                data: { itemId: itemId },
                success: function (response) {
                    if (response) {
                        $("#previewImg").removeClass("d-none").attr("src", "/SALES_Def_Inv_MainItemGroup/GetPhoto?itemId=" + itemId);
                        $("#removeBtn").removeClass("d-none");
                    }                
                },
                error: function () {
                    //$("#previewImg").attr("src", "/images/no-image.png"); // fallback
                }
            });

        }
        $(document).on('change', '#itemInfo-check-all', function () {
            var isChecked = $(this).prop('checked');
            $('.itemInfo-group-row-check').prop('checked', isChecked);

            checkedIdGroups = $('.itemInfo-group-row-check:checked').map(function () {
                return $(this).val();
            }).get();
        });

        $(document).on('change', '.itemInfo-group-row-check', function () {
            var checkedCount = $('.itemInfo-group-row-check:checked').length;
            var totalCount = $('.itemInfo-group-row-check').length;
            $('#itemInfo-check-all').prop('checked', checkedCount === totalCount);
            checkedIdGroups = $('.itemInfo-group-row-check:checked').map(function () {
                return $(this).val();
            }).get();
        });

        //Stock Level Management group 

        // Event handlers
        $(document).on('click', '.StockLevelManagement-group-item-link', function (e) {
            e.preventDefault();
            var table = $('#StockLevelManagementGrid').DataTable();
            var row = table.row($(this).closest('tr'));
            var rowValue = row.data();
            checkedIdGroups = [];
            $('#StockLevelManagement-check-all').prop('checked',false);
            $('#StockLevelManagement-group-row-check').prop('checked',false);
            checkedIdGroups.push(rowValue.tc+'');
            // Populate form fields
            $(settings.StockLevelManagement_TC).val(rowValue.tc);
            $(settings.StockLevelManagement_SLMID).val(rowValue.slmid);
            $(settings.StockLevelManagement_ItemID).val(rowValue.itemID).trigger('change');
            $(settings.StockLevelManagement_WarehouseID).val(rowValue.warehouseID).trigger('change');
            $(settings.StockLevelManagement_InStock).val(rowValue.inStock);
            $(settings.StockLevelManagement_StockValue).val(rowValue.stockValue);
            $(settings.StockLevelManagement_ReorderLevel).val(rowValue.reorderLevel);
            $(settings.StockLevelManagement_MaxStock).val(rowValue.maxStock);
            $(settings.StockLevelManagement_MinStock).val(rowValue.minStock);
            $(settings.StockLevelManagement_Description).val(rowValue.description);

            // If you also want to set the "Main Setup" section like before:
            $(settings.MainSetup_TC).val(rowValue.tc);
            $(settings.MainSetup_MainItemID).val(rowValue.itemID);
            $(settings.MainSetup_MainItemName).val(rowValue.itemName);
            $(settings.MainSetup_Description).val(rowValue.description);

            // Show create/modify dates
            $(settings.showCreateDate).empty().text(rowValue.showCreateDate || "");
            $(settings.showModifyDate).empty().text(rowValue.showModifyDate || "");
            $(settings.showCreateModifyDateContainer).show();

        });

        $(document).on('change', '#StockLevelManagement-check-all', function () {
            var isChecked = $(this).prop('checked');
            $('.StockLevelManagement-group-row-check').prop('checked', isChecked);

            checkedIdGroups = $('.StockLevelManagement-group-row-check:checked').map(function () {
                return $(this).val();
            }).get();
        });

        $(document).on('change', '.StockLevelManagement-group-row-check', function () {
            var checkedCount = $('.StockLevelManagement-group-row-check:checked').length;
            var totalCount = $('.StockLevelManagement-group-row-check').length;
            $('#StockLevelManagement-check-all').prop('checked', checkedCount === totalCount);
            checkedIdGroups = $('.StockLevelManagement-group-row-check:checked').map(function () {
                return $(this).val();
            }).get();
        });








        $(document).ready(() => {           
            GetAutoAllI("Main Group");
            stHeader();
            loadMianGrid()
            $(settings.showCreateModifyDateContainer).hide();
        })

        //#region changeable dropdown
        $(document).on('change', settings.mainGroupDropdownSelect, function () {
            let dropdownId = $(this).val();
            DropdownSelectChange(dropdownId, "MainItem");
        })
        $(document).on('change', settings.subGroupDropdownSelect, function () {
            let dropdownId = $(this).val();
            DropdownSelectChange(dropdownId, "SubItem");
        })
        $(document).on('change', settings.subGroup2DropdownSelect, function () {
            let dropdownId = $(this).val();
            DropdownSelectChange(dropdownId, "Sub2Item");
        })
        $(document).on('change', settings.supplierDropdownSelect, function () {
            let dropdownId = $(this).val();
            DropdownSelectChange(dropdownId, "supplier");
        })
        function DropdownSelectChange(dropdownId, DropdownName) {
            $.ajax({
                url: ChangeAbleDropdownUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify({ DropdownId: dropdownId, DropdownName: DropdownName }),
                success: function (res) {
                    if (res != null) {
                        populatedropdownItemInfo(res);
                    }
                },
                error: function (e) {
                    ;
                }
            });
        }

        function populatedropdownItemInfo(data) {
            let tabName = GetTabName();
            if (data.dropdownName == "MainItem") {


                if (tabName == "Sub Group") {
                    $(settings.SubGroupMainItemID).val(data.deopdownId);
                    $(settings.SubGroupDescription).val(data.descriptionName);
                } else if (tabName == "Sub Group - 2") {
                    $(settings.SubGroupTwoMainDescription).val(data.descriptionName);
                    $(settings.SubGroupTwoMainItemID).val(data.deopdownId);
                } else if (tabName == "Item Information") {
                    $(settings.ItemInfoMainItemID).val(data.deopdownId);
                    $(settings.ItemInfoMainDescription).val(data.descriptionName);
                }


            } else if (data.dropdownName == "SubItem") {
                if (tabName == "Sub Group - 2") {
                    $(settings.SubGroupTwoSubItemID).val(data.deopdownId);
                    $(settings.SubGroupTwoDescription).val(data.descriptionName);
                } else if (tabName == "Item Information") {
                    $(settings.ItemInfoSubItemID).val(data.deopdownId);
                    $(settings.ItemInfoSubDescription).val(data.descriptionName);

                }
            } else if (data.dropdownName == "Sub2Item") {
                $(settings.ItemInfoSubItem2ID).val(data.deopdownId);
                $(settings.ItemInfoSub2Description).val(data.descriptionName);
            } else if (data.dropdownName == "supplier") {
                $(settings.MainItemSupplierAddress).val(data.descriptionName);
            }
        }

        //#endregion


        $(document).ready(function () {
            let autoId = 0;

            // open file dialog
            $("#chooseBtn").on("click", function () {
                $("#photoInput").click();
            });

            // when file selected
            $("#photoInput").on("change", function () {
                $("#previewContainer").removeClass("d-none");
                let file = this.files[0];
                if (file) {
                    let reader = new FileReader();
                    reader.onload = function (e) {
                        $("#previewImg").attr("src", e.target.result).removeClass("d-none");
                        $("#removeBtn").removeClass("d-none");
                    }
                    reader.readAsDataURL(file);

                    // compress before upload (frontend)
                    compressAndUpload(file);
                }
            });

            // remove photo
            $("#removeBtn").on("click", function () {
                $("#previewImg").attr("src", "").addClass("d-none");
                $("#photoInput").val("");

                let itemId = $("#MainItem_ItemID").val();
                $.post("/SALES_Def_Inv_MainItemGroup/Delete", { itemId: itemId }, function (res) {
                    alert(res.message);
                });
                $("#removeBtn").addClass("d-none");
            });

            function compressAndUpload(file) {
                let img = new Image();
                let reader = new FileReader();
                reader.onload = function (e) {
                    img.src = e.target.result;
                };
                reader.readAsDataURL(file);

                img.onload = function () {
                    let canvas = document.createElement("canvas");
                    let ctx = canvas.getContext("2d");

                    // set max width/height
                    const MAX_WIDTH = 600;
                    const MAX_HEIGHT = 600;
                    let width = img.width;
                    let height = img.height;

                    if (width > height) {
                        if (width > MAX_WIDTH) {
                            height *= MAX_WIDTH / width;
                            width = MAX_WIDTH;
                        }
                    } else {
                        if (height > MAX_HEIGHT) {
                            width *= MAX_HEIGHT / height;
                            height = MAX_HEIGHT;
                        }
                    }
                    canvas.width = width;
                    canvas.height = height;

                    ctx.drawImage(img, 0, 0, width, height);

                    canvas.toBlob(function (blob) {
                        let formData = new FormData();
                        formData.append("Photo", blob, file.name);
                        formData.append("ItemID", $(settings.MainItem_ItemID).val());
                        formData.append("CompanyCode", "001");
                        formData.append("EmployeeID", "");

                        $.ajax({
                            url: '/SALES_Def_Inv_MainItemGroup/Upload',
                            type: 'POST',
                            data: formData,
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                autoId = res.autoId || 0;
                            },
                            error: function (err) {
                            }
                        });
                    }, 'image/jpeg', 0.7);
                };
            }
        });










        function loadChangeMainSubGrid(filterData) {
            if ($.fn.DataTable.isDataTable('#subGroupGrid')) {
                $('#subGroupGrid').DataTable().destroy();
            }

            $("#subGroupGrid").DataTable({
                processing: true,
                serverSide: false,
                searching: true,
                paging: true,
                info: true,
                autoWidth: false,
                responsive: true,
                pageLength: 10,
                order: [[1, 'asc']],
                ajax: {
                    url: "/SALES_Def_Inv_MainItemGroup/GetItemHierarchy",
                    type: "POST",
                    contentType: "application/json",
                    data: function () {
                        return JSON.stringify(filterData);
                    },
                    dataSrc: function (json) {
                        return json.data.subGroupList;
                    }
                },

                columns: [
                    {
                        data: "tc",
                        render: function (data, type, row) {
                            if (type === 'display') {
                                return `<input type="checkbox" class="sub-group-row-check" value="${data}"/>`;
                            }
                            return data;
                        },
                        orderable: false,
                        searchable: false,
                        width: "20px",
                        className: "text-center"
                    },
                    {
                        data: "subItemID",
                        render: function (data, type, row) {
                            if (type === 'display') {
                                return `<a href="#" class="sub-group-item-link" data-id="${row.tc}">${data}</a>`;
                            }
                            return data;
                        },
                        width: "40px",
                        className: "text-center"
                    },
                    {
                        data: "subItemName",
                        width: "200px"
                    },
                    {
                        data: "description",
                        width: "300px"
                    },
                    {
                        data: "mainItemName",
                        width: "150px"
                    }
                ]
            });
        }

        $(document).on('change', "#SubGroupMainItemName", function () {
            loadChangeMainSubGrid({
                MainId: $(this).val() || "",
                SubId: "",
                Sub2Id: "",
                ItemId: "",
                StockItemId: ""
            });

        })

        function loadChangeMainSub2Grid(filterData, dName) {
            if ($.fn.DataTable.isDataTable('#subGroup2Grid')) {
                $('#subGroup2Grid').DataTable().destroy();
            }


            $("#subGroup2Grid").DataTable({
                processing: true,
                serverSide: false,
                searching: true,
                paging: true,
                info: true,
                autoWidth: false,
                responsive: true,
                pageLength: 10,
                order: [[1, 'asc']],
                ajax: {
                    url: "/SALES_Def_Inv_MainItemGroup/GetItemHierarchy",
                    type: "POST",
                    contentType: "application/json",
                    data: function () {
                        return JSON.stringify(filterData);
                    },
                    dataSrc: function (json) {
                       
                        let tabName = GetTabName();
                        if (tabName == "Sub Group - 2") {
                            
                            if (dName == 'sub') {
                                let $SubItem = $("#SubGroupTwoSubGroupName");
                                $SubItem.empty();
                                $SubItem.append('<option></option>');
                                $.each(json.data.subGroupList, function (i, item) {
                                    $SubItem.append(`<option value="${item.subItemID}">${item.subItemName}</option>`);
                                });
                            }
                        }
                        return json.data.sub2GroupList;
                    }
                },

                columns: [
                    {
                        data: "tc",
                        render: function (data, type, row) {
                            if (type === 'display') {
                                return `<input type="checkbox" class="sub2-group-row-check" value="${data}"/>`;
                            }
                            return data;
                        },
                        orderable: false,
                        searchable: false,
                        width: "20px",
                        className: "text-center"
                    },
                    {
                        data: "subItem2ID",
                        render: function (data, type, row) {
                            if (type === 'display') {
                                return `<a href="#" class="sub2-group-item-link" data-id="${row.tc}">${data}</a>`;
                            }
                            return data;
                        },
                        width: "40px",
                        className: "text-center"
                    },
                    {
                        data: "subItem2Name",
                        width: "200px"
                    },
                    {
                        data: "description",
                        width: "300px"
                    },
                    {
                        data: "mainItemName",
                        width: "150px"
                    },
                    {
                        data: "subItemName",
                        width: "150px"
                    }
                ]
            });
        }

        $(document).on('change', "#SubGroupTwoMainItemName", function () {
            loadChangeMainSub2Grid({
                MainId: $(this).val() || "",
                SubId: $("#SubGroupTwoSubGroupName").val() || "",
                Sub2Id: "",
                ItemId: "",
                StockItemId: ""
            }, 'sub');

            $('#SubGroupTwoSubItemID').val('');
            $('#SubGroupTwoDescription').val('');
        })
        $(document).on('change', "#SubGroupTwoSubGroupName", function () {
            loadChangeMainSub2Grid({
                MainId: $("#SubGroupTwoMainItemName").val() || "",
                SubId: $("#SubGroupTwoSubGroupName").val() || "",
                Sub2Id: "",
                ItemId: "",
                StockItemId: ""
            }, '');

        })
        function loadChangeMainItemGrid(filterData, dName) {
            

            if ($.fn.DataTable.isDataTable('#itemInformationGrid')) {
                $('#itemInformationGrid').DataTable().destroy();
            }

            $("#itemInformationGrid").DataTable({
                processing: true,
                serverSide: false,
                searching: true,
                paging: true,
                info: true,
                autoWidth: false,
                responsive: true,
                pageLength: 10,
                order: [[1, 'asc']],
                ajax: {
                    url: "/SALES_Def_Inv_MainItemGroup/GetItemHierarchy",
                    type: "POST",
                    contentType: "application/json",
                    data: function () {
                        return JSON.stringify(filterData);  
                    },
                    dataSrc: function (json) {
                        let tabName = GetTabName();
                        if (dName == 'main' && tabName == "Item Information") {
                            let $SubItem = $("#ItemInfoSubItemName");
                            $SubItem.empty().append('<option></option>');
                            $.each(json.data.subGroupList, function (i, item) {
                                $SubItem.append(`<option value="${item.subItemID}">${item.subItemName}</option>`);
                            });

                            let $Sub2Item = $("#ItemInfoSubItem2Name");
                            $Sub2Item.empty().append('<option></option>');
                            $.each(json.data.sub2GroupList, function (i, item) {
                                $Sub2Item.append(`<option value="${item.subItem2ID}">${item.subItem2Name}</option>`);
                            });
                        }

                        if (dName == 'sub' && tabName == "Item Information") {
                            let $Sub2Item = $("#ItemInfoSubItem2Name");
                            $Sub2Item.empty().append('<option></option>');
                            $.each(json.data.sub2GroupList, function (i, item) {
                                $Sub2Item.append(`<option value="${item.subItem2ID}">${item.subItem2Name}</option>`);
                            });
                        }

                        return json.data.itemList || [];  
                    }
                },

                columns: [
                    { data: "tc", render: d => `<input type="checkbox" class="itemInfo-group-row-check" value="${d}"/>`, orderable: false, searchable: false, width: "50px", className: "text-center" },
                    { data: "itemID", render: (d, t, r) => `<a href="#" class="itemInfo-group-item-link" data-id="${r.tc}">${d}</a>`, width: "50px", className: "text-center" },
                    { data: "itemName", width: "200px" },
                    { data: "printName", width: "300px" },
                    { data: "itemTypeName", width: "150px" },
                    { data: "itemUnitName", width: "150px" },
                    { data: "buyerName", width: "150px" },
                    { data: "styleName", width: "150px" }
                ]
            });
        }
        // MainItem change
        $(document).on('change', "#ItemInfoMainItemName", function () {
            loadChangeMainItemGrid({
                MainId: $(this).val() || "",
                SubId: "",
                Sub2Id: "",
                ItemId: "",
                StockItemId: ""
            }, 'main');
            $("#ItemInfoSub2Description").val('');
            $("#ItemInfoSubItem2ID").val('');
            $("#ItemInfoSubDescription").val('');
            $("#ItemInfoSubItemID").val('');
        });

        // SubItem change
        $(document).on('change', "#ItemInfoSubItemName", function () {
            loadChangeMainItemGrid({
                MainId: $("#ItemInfoMainItemName").val() || "",
                SubId: $(this).val() || "",
                Sub2Id: "",
                ItemId: "",
                StockItemId: ""
            }, 'sub');
            $("#ItemInfoSub2Description").val('');
            $("#ItemInfoSubItem2ID").val('');
        });

        // SubItem2 change
        $(document).on('change', "#ItemInfoSubItem2Name", function () {
            loadChangeMainItemGrid({
                MainId: $("#ItemInfoMainItemName").val() || "",
                SubId: $("#ItemInfoSubItemName").val() || "",
                Sub2Id: $(this).val() || "",
                ItemId: "",
                StockItemId: ""
            }, '');
        });


       
       
        //// Example Call
        //loadSubGrid({
        //    MainId: "M001",
        //    SubId: "",
        //    Sub2Id: "",
        //    ItemId: "",
        //    StockItemId: ""
        //});



        function loadChangeStockLevelManagementGrid(filterData) {
            if ($.fn.DataTable.isDataTable('#StockLevelManagementGrid')) {
                $('#StockLevelManagementGrid').DataTable().destroy();
            }

            $("#StockLevelManagementGrid").DataTable({
                processing: true,
                serverSide: false,
                searching: true,
                paging: true,
                info: true,
                autoWidth: false,
                responsive: true,
                pageLength: 10,
                order: [[1, 'asc']],
                ajax: {
                    url: "/SALES_Def_Inv_MainItemGroup/GetItemHierarchy",
                    type: "POST",
                    contentType: "application/json",
                    data: function () {
                        return JSON.stringify(filterData);
                    },
                    dataSrc: function (json) {
                        return json.data.stockItemList;
                    }
                },

                columns: [
                    {
                        data: "tc",
                        render: function (data, type, row) {
                            if (type === 'display') {
                                return `<input type="checkbox" class="StockLevelManagement-group-row-check" value="${data}"/>`;
                            }
                            return data;
                        },
                        orderable: false,
                        searchable: false,
                        width: "50px",
                        className: "text-center"
                    },
                    {
                        data: "slmid",
                        render: function (data, type, row) {
                            if (type === 'display') {
                                return `<a href="#" class="StockLevelManagement-group-item-link" data-id="${row.tc}">${data}</a>`;
                            }
                            return data;
                        },
                        width: "50px",
                        className: "text-center"
                    },
                    {
                        data: "itemName",
                        width: "300px"
                    },
                    {
                        data: "warehouseName",
                        width: "200px"
                    },
                    {
                        data: "inStock",
                        width: "150px"
                    },
                    {
                        data: "stockValue",
                        width: "150px"
                    },
                    {
                        data: "reorderLevel",
                        width: "150px"
                    }, {
                        data: "maxStock",
                        width: "150px"
                    },
                    {
                        data: "minStock",
                        width: "150px"
                    },
                    {
                        data: "description",
                        width: "150px"
                    }
                ]
            });
        }

        $(document).on('change', "#StockLevelManagement_ItemID", function () {
            loadChangeStockLevelManagementGrid({
                MainId: "",
                SubId: "",
                Sub2Id: "",
                ItemId: $(this).val()||"",
                StockItemId: ""
            }, 'main');
        })       
    }





}(jQuery));