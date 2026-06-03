
(function ($) {
    $.ItemMasterInformation = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            CatagoryBtn: ".catagoryBtn",
            CloseCatagoryModel: ".closeCatagoryModel",
            DropdownCategory:".dropdownCategory",
            DropdownBrand:".dropdownBrand",
            BrandBtn:"#brandBtn",
            CloseBrandModel: ".closeBrandModel",
            ProductCode: "#productCode",
            ProductName: "#productName",
            ProductDescription: "#productDescription",
            UnitDropdownVale: ".unitDropdownVale",
            PurchaseCost: "#purchaseCost",
            AutoId: "#Setup_AutoId",
            ProductSaveBtn: ".js-Printing-Stationery-Purchase-save",
            ProductItemPrintBtn:".js-Item-product-print",

            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBrn: ".item-btn-edit",
            DeleteBtn: "#js-Item-product-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-Product-clear",

        }, options);

        var categoryListUrl = commonName.baseUrl + "/categoryList";
        var brandListUrl = commonName.baseUrl + "/brandList";
        var loadProductDataUrl = commonName.baseUrl + "/LoadData";
        var autoProductIdUrl = commonName.baseUrl + "/AutoProductId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteProduct";
        var alreadyExistUrl = commonName.baseUrl + "/alreadyExist";
        var DownloadItemInformationReportUrl = commonName.baseUrl + "/DownloadItemInformationReport";
        function stHeader() {
            window.addEventListener('scroll', function () {
                const header = document.getElementById('stickyHeader');
                if (window.scrollY > 10) {
                    header.classList.add('scrolled');
                } else {
                    header.classList.remove('scrolled');
                }
            });
        }

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


        $(document).ready(function () {
            if ($('#catagoryModal').length === 0) {
                const modalHtml = `
        <div class="modal fade" id="catagoryModal" tabindex="-1" aria-labelledby="catagoryModalLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
            <div class="modal-dialog modal-dialog-centered modal-custom-item-master modal-xl">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Category</h5>
                        <button type="button" class="btn-close closeCatagoryModel" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div id="catagoryContainer"></div>
                    </div>
                </div>
            </div>
        </div>
        `;
                $('body').append(modalHtml);
            }
        });

        // Button click handler
        $(commonName.CatagoryBtn).on('click', function () {
            const $btn = $(this);
        
            $.ajax({
                url: '/INV_Catagory/Index?isPartial=true',
                type: 'GET',
                success: function (result) {
                    $('#catagoryContainer').html(result);
                    $('#catagoryModal').modal('show'); 
                    if (typeof $.INV_Catagory === 'function') {
                        var options = {
                            baseUrl: '/INV_Catagory',
                            isPartial: true,
                        };
                        $.INV_Catagory(options);
                    }
                },
                error: function () {
                    alert("Failed to load category page");
                }               
            });
        });


        $(document).ready(function () {
            if ($('#brandModal').length === 0) {
                const modalHtml = `
           <div class="modal fade" id="brandModal" tabindex="-1" aria-labelledby="brandModalLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
                <div class="modal-dialog modal-dialog-centered modal-custom-item-master modal-xl">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Brand</h5>
                            <button type="button" class="btn-close closeBrandModel" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <div id="brandContainer"></div>
                        </div>
                    </div>
                </div>
            </div>
        `;
                $('body').append(modalHtml);
            }
        });

        
        // Then simple click handler
        $(commonName.BrandBtn).on('click', function () {           
            const $btn = $(this);
           
            $.ajax({
                url: '/Brand/Index?isPartial=true',
                type: 'GET',
                success: function (result) {
                    $('#brandContainer').html(result);

                    $('#brandModal').modal('show');

                    if (typeof $.HrmBrand === 'function') {
                        $.HrmBrand({
                            baseUrl: '/Brand',
                            isPartial: true
                        });
                    }
                },
                error: function () {
                    alert("Failed to load brand page");
                }
            });
        });

        $(commonName.CloseCatagoryModel).on('click', function () {  
            $('.modal').modal('hide');
            $('.modal-backdrop').remove(); 
            $('body').removeClass('modal-open');
            $.ajax({
                url: categoryListUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.DropdownCategory).empty();

                    res.data.forEach(function (item) {
                        $(commonName.DropdownCategory).append(
                            $('<option></option>').val(item.catagoryId).text(item.catagoryName)
                        );
                    });

                }, error: function (error) {
                }
            });
        })

        $(".closeBrandModel").on('click', function () {
            $('.modal').modal('hide');
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open');
            $.ajax({
                url: brandListUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.DropdownBrand).empty();

                    res.data.forEach(function (item) {
                       
                        $(commonName.DropdownBrand).append(
                            $('<option></option>').val(item.brandId).text(item.brandName)
                        );
                    });
                }, error: function (error) {
                }
            });
        })      

        // Sticky header on scroll
        function stHeader() {
            window.addEventListener('scroll', function () {
                const header = document.getElementById('stickyHeader');
                if (window.scrollY > 10) {
                    header.classList.add('scrolled');
                } else {
                    header.classList.remove('scrolled');
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
        autoProductId = function () {
            $.ajax({
                url: autoProductIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.ProductCode).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
            autoProductId();
        })
        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.ProductCode).val('');
            $(commonName.ProductName).val('');
            $(commonName.ProductDescription).val('');
            $(commonName.UnitDropdownVale).val('').trigger('change');
            $(commonName.PurchaseCost).val('');
            $(commonName.DropdownCategory).val('').trigger('change');
            $(commonName.DropdownBrand).val('').trigger('change'); 

            $(commonName.ShortName).val('');
            $(commonName.CreateDate).text('');
            $(commonName.UpdateDate).text('');

            $('.Product-input').removeClass('Product-input');

            $(commonName.ProductSaveBtn).prop('disabled', false);
        };

        // get data from input
        getFromData = function () {
            var fromData = {
                AutoId: $(commonName.AutoId).val() || 0,
                ProductCode: $(commonName.ProductCode).val(),
                ProductName: $(commonName.ProductName).val(),
                Description: $(commonName.ProductDescription).val(),
                UnitID: $(commonName.UnitDropdownVale).val(),
                PurchaseCost: $(commonName.PurchaseCost).val()||0,
                CatagoryName: $(commonName.DropdownCategory).val(),
                BrandName: $(commonName.DropdownBrand).val(),
            };
            return fromData;
        }
        //exists 
        $(commonName.ProductName).on('input', function () {
            let ProductValue = $(this).val();
            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(ProductValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.ProductName).addClass('Product-input');
                        $(commonName.ProductSaveBtn).prop('disabled', true);
                    } else {
                        $(commonName.ProductName).removeClass('Product-input');
                        $(commonName.ProductSaveBtn).prop('disabled', false);
                        $(commonName.ProductSaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })
        //create and edit

        $(document).off('click', commonName.ProductSaveBtn)
            .on('click', commonName.ProductSaveBtn, function () {

                var $btn = $(commonName.ProductSaveBtn);
                if ($btn.data('submitting')) return; 
                $btn.data('submitting', true);
                var fromData = getFromData();
                if (!fromData.ProductName || fromData.ProductName.trim() === '') {
                    $(commonName.ProductName).addClass('Product-input').focus();
                    $btn.prop('disabled', true);
                    $btn.data('submitting', false);
                    return;
                }

                $btn.prop('disabled', true);

                $.ajax({
                    url: CreateUpdateUrl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(fromData),
                    success: function (res) {
                        if (res.isSuccess) {
                            $(document).trigger('data:changed', ['product']);
                            loadProductData();
                            resetFrom();
                            autoProductId();
                            showToast("success", res.message);
                        } else {
                            showToast("error", res.message);
                        }
                    },
                    error: function (e) {
                        showToast("error", "Something went wrong");
                    },
                    complete: function () {
                        $btn.prop('disabled', false);
                        $btn.data('submitting', false);
                       
                    }
                });
            });
        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });
        // Reload DataTable Function
        function loadProductData() {
            table.ajax.reload(null, false);
        }
        if ($.fn.DataTable.isDataTable('#ProductTable')) {
            $('#ProductTable').DataTable().clear().destroy();
        }
        var table = $('#ProductTable').DataTable({
            "autoWidth": true,
            "ajax": {
                "url": loadProductDataUrl,
                "type": "GET",
                "datatype": "json",
                "dataSrc": function (json) {
                    return json.data || [];
                },
                "error": function (xhr, error, thrown) {
                    showToast("error", "Data loading failed: " + xhr.statusText);
                }
            },
            "columns": [
                {
                    "data": "autoId",
                    "render": function (data) {
                        return `<input type="checkbox" class="row-checkbox" value=${data} />`;
                    },
                    "orderable": false
                },
                {
                    "data": "productCode",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link item-btn-edit" data-id=${data}>${data}</button>`;
                    }
                },
                { "data": "productName" },
                { "data": "brandName" },
                { "data": "catagoryName" },
                {"data": "unitID" },
                { "data": "purchaseCost" },
            ],
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
                "paginate": {
                    "first": "First",
                    "last": "Last",
                    "next": "Next",
                    "previous": "Previous"
                }
            }
        });
        $(document).on('click', commonName.ProductItemPrintBtn, function () {
            $.ajax({
                url: loadProductDataUrl,
                type: "GET",
                success: function (res) {
                    downloadItemInfoExcel(res.data);
                }
            });
        })//todo

        function downloadItemInfoExcel(data) {
            $.ajax({
                url: DownloadItemInformationReportUrl,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data),
                xhrFields: {
                    responseType: 'blob'
                },
                success: function (blob) {
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = "ItemInformationReport.xlsx";
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                },
                error: function () {
                    alert("Failed to download report.");
                }
            });
        }

        let selectedIds = [];
        //edit
        $(document).on('click', commonName.EditBrn, function () {
            let id = $(this).data('id');
            $.ajax({
                url: `${PopulatedDataForUpdateUrl}?id=${id}`,
                type: "GET",
                success: function (res) {
                    selectedIds = [];
                    selectedIds.push(res.result.autoId + '');
                    $(commonName.AutoId).val(res.result.autoId);
                    $(commonName.ProductCode).val(res.result.productCode);
                    $(commonName.ProductName).val(res.result.productName);
                    $(commonName.DropdownBrand).val(res.result.brandId);
                    $(commonName.DropdownCategory).val(res.result.catagoryId);
                    $(commonName.UnitDropdownVale).val(res.result.unitID);
                    $(commonName.ProductDescription).val(res.result.description);
                    $(commonName.PurchaseCost).val(res.result.purchaseCost);
                    $(commonName.CreateDate).text(res.result.showCreateDate);
                    $(commonName.UpdateDate).text(res.result.showModifyDate);
                },
                error: function (e) {
                }, complete: function () {
                }
            });
        });

        //selected id        

        $(document).on('change', commonName.RowCheckbox, function () {
            const id = $(this).val();
            if ($(this).is(':checked')) {
                if (!selectedIds.includes(id)) {
                    selectedIds.push(id);
                }
            } else {
                selectedIds = selectedIds.filter(item => item != id);
            }

            let totalCheckboxes = $(commonName.RowCheckbox).length;
            let totalChecked = $(commonName.RowCheckbox + ":checked").length;

            $('#selectAll').prop('checked', totalChecked === totalCheckboxes);
        })
        //select all
        $(document).on('change', commonName.SelectedAll, function () {
            const isChecked = $(this).is(':checked');
            $(commonName.RowCheckbox).prop('checked', isChecked).trigger('change');
        })
        $(document).on('click', commonName.DeleteBtn, function () {
            $.ajax({
                url: deleteUrl,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(selectedIds),
                success: function (res) {
                    showToast(res.isSuccess ? "success" : "error", res.message)
                },
                error: function (e) {
                }, complete: function () {
                    resetFrom();
                    autoProductId();
                    loadProductData();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            })
        })


        window.ProductModuleLoaded = true;


        var init = function () {
            stHeader(); 
            autoProductId();
            table;
        };
        init();
    };
})(jQuery);
