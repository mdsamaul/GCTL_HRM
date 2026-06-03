(function ($) {
    $.ItemModel = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            ShortName: "#shortName",
            ItemModelName: "#itemModelName",
            ItemModelBrand: "#itemModelBrand",
            ItemModelID: "#itemModelId",
            AutoId: "#Setup_AutoId",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAllSupplierTypeTable",
            EditBrn: ".model-btn-edit",
            ItemModelSaveBtn: ".js-inv-ItemModel-save",
            DeleteBtn: "#js-inv-ItemModel-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-ItemModel-clear",
            ItemBrandBtn: "#itemBrandBtn",
            ItemBrandContainer:"#itemBrandContainer",
        }, options);

        var loadCategoryDataUrl = commonName.baseUrl + "/LoadData";
        var autoItemModelIdUrl = commonName.baseUrl + "/AutoItemModelId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteItemModel";
        var alreadyExistUrl = commonName.baseUrl + "/alreadyExist";
        var itemBranchUrl = "/Brand/Index?isPartial=true";
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
        autoItemModelId = function () {
            $.ajax({
                url: autoItemModelIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.ItemModelID).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.ItemModelName).val('');
            $(commonName.ShortName).val('');
            $(commonName.ItemModelBrand).val('').trigger('change');
            autoItemModelId();
        }
        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
        })
        // get data from input
        getFromData = function () {
            var fromData = {
                AutoId: $(commonName.AutoId).val()||0,
                ModelID: $(commonName.ItemModelID).val(),
                BrandID: $(commonName.ItemModelBrand).val(),
                ModelName: $(commonName.ItemModelName).val(),
                ShortName: $(commonName.ShortName).val(),
            };
            return fromData;
        }
        //exists 
        $(commonName.ItemModelName).on('input', function () {

            let ItemModelValue = $(this).val();

            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(ItemModelValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.ItemModelName).addClass('itemModel-input');
                        $(commonName.ItemModelSaveBtn).prop('disabled', true);
                    } else {
                        $(commonName.ItemModelName).removeClass('itemModel-input');
                        $(commonName.ItemModelSaveBtn).prop('disabled', false);
                        $(commonName.ItemModelSaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })

        $(commonName.ItemModelBrand).on('input', function () {

            let ItemModelValue = $(this).val();
            if (!ItemModelValue) {
                showToast('warning', "Brand Requird");
                $(commonName.ItemModelBrand).addClass('itemModel-input');
                $(commonName.ItemModelSaveBtn).prop('disabled', true);
            } else {
                $(commonName.ItemModelBrand).removeClass('itemModel-input');
                $(commonName.ItemModelSaveBtn).prop('disabled', false);
                $(commonName.ItemModelSaveBtn).css('border', 'none');

            }
        })
       

        //create and edit
        // Save Button Click
        $(document).on('click', commonName.ItemModelSaveBtn, function () {
            var $btn = $(commonName.ItemModelSaveBtn);
            if ($btn.data('submitting')) return;
            $btn.data('submitting', true);

            var fromData = getFromData();
            if (fromData.ModelName == null || fromData.ModelName.trim() === '') {
                $(commonName.ItemModelName).addClass('itemModel-input');
                $(commonName.ItemModelSaveBtn).prop('disabled', true);
                $(commonName.ItemModelName).focus();
                return;
            }

            if (fromData.BrandID == null || fromData.BrandID.trim() === '') {
                
                $(commonName.ItemModelSaveBtn).prop('disabled', true);
                $(commonName.ItemModelBrand).select2(); 
                $(commonName.ItemModelBrand).select2('open');
                $(commonName.ItemModelBrand).addClass('itemModel-input');
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
                        $(document).trigger('data:changed', ['model']);
                        showToast("success", res.message);
                        loadCategoryData();
                        resetFrom();
                        autoItemModelId();
                    } else {
                        showToast("error", res.message);
                    }
                },
                error: function (e) {
                    showToast("error", res.message);
                },
                complete: function () {
                    $btn.prop('disabled', false);
                    $btn.data('submitting', false);
                }
            });
        });

        // Reload DataTable Function
        function loadCategoryData() {
            table.ajax.reload(null, false);
        }

        var table = $('#itemModelTable').DataTable({
            "autoWidth": true,
            "ajax": {
                "url": loadCategoryDataUrl,
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
                    "data": "modelID",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link model-btn-edit" data-id=${data}>${data}</button>`;
                    }
                },
                { "data": "modelName" },
                { "data": "shortName" },
                { "data": "brandName" }
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
                    $(commonName.ItemModelName).val(res.result.modelName);
                    $(commonName.ShortName).val(res.result.shortName);
                    $(commonName.ItemModelID).val(res.result.modelID);
                    //$(commonName.ItemModelBrand).val(res.result.brandID).trigger('change');
                    $('#itemModelBrand').val(res.result.brandID).trigger('change');
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
                    autoItemModelId();
                    loadCategoryData();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            })
        })

        $(document).ready(function () {
            if ($('#itemBrandModal').length === 0) {
                const modalHtml = `
         <div class="modal fade" id="itemBrandModal" tabindex="-1" aria-labelledby="itemBrandModalLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
                <div class="modal-dialog modal-dialog-centered custom-modal-item modal-xl">
                    <div class="modal-content">

                        <div class="modal-header">
                            <h5 class="modal-title" id="itemBrandModalLabel">Brand</h5>
                            <button type="button" class="btn-close itemBrandModalLabelClose" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>

                        <div class="modal-body">
                            <div id="itemBrandContainer"></div>
                        </div>
                    </div>
                </div>
            </div>
        `;
                $('body').append(modalHtml);
            }
        });

        $(commonName.ItemBrandBtn).on('click', function () {
            $.ajax({
                url: itemBranchUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.ItemBrandContainer).html(res);
                    if (typeof $.HrmBrand === 'function') {
                        $.HrmBrand({
                            baseUrl: "/Brand",
                            isPartial: true
                        })
                    }
                }, error: function (e) {
                    alert("Failed to load brand page");
                }
            });
        })
        $('.searchable-select').select2({
            placeholder: 'Select an option',
            allowClear: false,
            width: '100%'
        });
        window.categoryModuleLoaded = true;
        // Initialize all functions
        var init = function () {
            stHeader();
            autoItemModelId();
            table;
        };
        init();

    };
})(jQuery);
