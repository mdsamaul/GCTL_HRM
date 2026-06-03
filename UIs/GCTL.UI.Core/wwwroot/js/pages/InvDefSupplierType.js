(function ($) {
    $.InvDefSupplierType = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            ShortName: "#Setup_ShortName",
            SupplierType: "#Setup_SupplierType",
            SupplierTypeID: "#Setup_SupplierTypeId",
            AutoId: "#Setup_AutoId",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAllSupplierTypeTable",
            EditBrn: ".SupplierTypeEditBtn",
            SupplierTypeSaveBtn: ".js-InvDefSupplierType-save",
            DeleteBtn: "#js-InvDefSupplierType-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-InvDefSupplierType-clear",
        }, options);

        var loadSuplierTypeDataUrl = commonName.baseUrl + "/LoadData";
        var autoSupplierTypeIdUrl = commonName.baseUrl + "/AutoSuplierTypeId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteSupplierType";
        var alreadyExistUrl = commonName.baseUrl + "/alreadyExist";

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
        autoSupplierTypeId = function () {
            $.ajax({
                url: autoSupplierTypeIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.SupplierTypeID).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.SupplierType).val('');
            $(commonName.ShortName).val('');
            $(commonName.CreateDate).text('');
            $(commonName.UpdateDate).text('');
            autoSupplierTypeId();
        }
        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
        })
        // get data from input
        getFromData = function () {
            var fromData = {
                AutoId: $(commonName.AutoId).val(),
                SupplierTypeID: $(commonName.SupplierTypeID).val(),
                SupplierType: $(commonName.SupplierType).val(),
                ShortName: $(commonName.ShortName).val(),
            };
            return fromData;
        }
        //exists 
        $(commonName.SupplierType).on('input', function () {
            let SupplierTypeValue = $(this).val();   
            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(SupplierTypeValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.SupplierType).addClass('SupplierType-input');
                        $(commonName.SupplierTypeSaveBtn).prop('disabled', true);
                    } else {
                        $(commonName.SupplierType).removeClass('SupplierType-input');
                        $(commonName.SupplierTypeSaveBtn).prop('disabled', false);
                        $(commonName.SupplierTypeSaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })
        //create and edit
        // Save Button Click
        $(document).on('click', commonName.SupplierTypeSaveBtn, function () {

            var fromData = getFromData();
            if (fromData.SupplierType == null || fromData.SupplierType.trim() === '') {
                $(commonName.SupplierType).addClass('SupplierType-input');
                $(commonName.SupplierTypeSaveBtn).prop('disabled', true);
                $(commonName.SupplierType).focus();
                return;
            }


            $.ajax({
                url: CreateUpdateUrl,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(fromData),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast("success", res.message);
                    } else {
                        showToast("error", res.message);
                    }
                },
                error: function (e) {
                    showToast("error", res.message);
                },
                complete: function () {
                    resetFrom();
                    autoSupplierTypeId();
                    loadCategoryData();
                }
            });
        });

        // Reload DataTable Function
        function loadCategoryData() {
            table.ajax.reload(null, false);
        }

        var table = $('#SupplierTypeTable').DataTable({
            "autoWidth": true,
            "ajax": {
                "url": loadSuplierTypeDataUrl,
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
                    "data": "supplierTypeId",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link SupplierTypeEditBtn" data-id=${data}>${data}</button>`;
                    }
                },
                { "data": "supplierType" },
                { "data": "shortName" }
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
                    $(commonName.SupplierType).val(res.result.supplierType);
                    $(commonName.ShortName).val(res.result.shortName);
                    $(commonName.SupplierTypeID).val(res.result.supplierTypeId);
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
                    autoSupplierTypeId();
                    loadCategoryData();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            })
        })


        window.categoryModuleLoaded = true;
        // Initialize all functions
        var init = function () {
            stHeader();
            autoSupplierTypeId();
            table;           
        };
        init();

    };
})(jQuery);
