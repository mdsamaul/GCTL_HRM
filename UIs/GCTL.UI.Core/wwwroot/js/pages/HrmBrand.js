(function ($) {
    $.HrmBrand = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            ShortName: "#brandShortName",
            BrandName: "#brandName",
            BrandID: "#BrandID",
            AutoId: "#Setup_AutoId",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBrn: ".brand-btn-edit",
            BrandSaveBtn: ".js-brand-save",
            DeleteBtn: "#js-inv-Brand-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-brand-clear",
        }, options);

        var loadBrandDataUrl = commonName.baseUrl + "/LoadData";
        var autoBrandIdUrl = commonName.baseUrl + "/AutoBrandId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteBrand";
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
        autoBrandId = function () {
            $.ajax({
                url: autoBrandIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.BrandID).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        $(commonName.ClearBrn).on('click', function () {           
            resetFrom();
            autoBrandId();
        })
        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.BrandName).val('');
            $(commonName.ShortName).val('');
            $(commonName.CreateDate).text('');
            $(commonName.UpdateDate).text('');
        }
        // get data from input
        getFromData = function () {
            var fromData = {
                AutoId: $(commonName.AutoId).val(),
                BrandID: $(commonName.BrandID).val(),
                BrandName: $(commonName.BrandName).val(),
                ShortName: $(commonName.ShortName).val(),
            };
            return fromData;
        }
        //exists 
        $(commonName.BrandName).on('input', function () {

            let BrandValue = $(this).val();

            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(BrandValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.BrandName).addClass('Brand-input');
                        $(commonName.BrandSaveBtn).prop('disabled', true);
                    } else {
                        $(commonName.BrandName).removeClass('Brand-input');
                        $(commonName.BrandSaveBtn).prop('disabled', false);
                        $(commonName.BrandSaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })
        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });
        //create and edit
        // Save Button Click
        $(document).on('click', commonName.BrandSaveBtn, function () {
            var $btn = $(commonName.BrandSaveBtn);
            if ($btn.data('submitting')) return;
            $btn.data('submitting', true);
            var fromData = getFromData();
            if (fromData.BrandName == null || fromData.BrandName.trim() === '') {
                $(commonName.BrandName).addClass('Brand-input');
                $(commonName.BrandSaveBtn).prop('disabled', true);
                $(commonName.BrandName).focus();
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
                        $(document).trigger('data:changed', ['brand']);
                        showToast("success", res.message);
                        loadBrandData();
                        resetFrom();
                        autoBrandId();
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
        function loadBrandData() {
            table.ajax.reload(null, false);
        }
        if ($.fn.DataTable.isDataTable('#brandTable')) {
            $('#brandTable').DataTable().destroy();
        }
        var table = $('#brandTable').DataTable({           
            "autoWidth": true,
            "ajax": {
                "url": loadBrandDataUrl,
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
                    "data": "brandID",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link brand-btn-edit" data-id=${data}>${data}</button>`;
                    }
                },
                { "data": "brandName" },
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
                    $(commonName.BrandName).val(res.result.brandName);
                    $(commonName.ShortName).val(res.result.shortName);
                    $(commonName.BrandID).val(res.result.brandID);
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
                    autoBrandId();
                    loadBrandData();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            })
        })


        window.BrandModuleLoaded = true;
        // Initialize all functions
        var init = function () {
            stHeader();
            autoBrandId();
            table;
        };
        init();

    };
})(jQuery);
