(function ($) {
    $.RmgProdDefUnitType = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            ShortName: "#unitTypeShortName",
            UnitTypeName: "#unitTypeName",
            UnitTypeId: "#unitTypeId",
            DecimalPlacesLeftValue: "#decimalPlacesLeftValue",
            AutoId: "#Setup_TC",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBrn: ".btn-edit",
            UnitTypeSaveBtn: ".js-unit-type-save",
            DeleteBtn: "#js-unit-type-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-unit-type-clear",
        }, options);

        var loadCategoryDataUrl = commonName.baseUrl + "/LoadData";
        var autoUnitTypeIdUrl = commonName.baseUrl + "/AutoUnitTypeId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteUnitType";
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
        autoUnitTypeId = function () {
            $.ajax({
                url: autoUnitTypeIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.UnitTypeId).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.UnitTypeName).val('');
            $(commonName.ShortName).val('');
            $(commonName.DecimalPlacesLeftValue).val('');
            $(commonName.CreateDate).text('');
            $(commonName.UpdateDate).text('');
        }
        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
            autoUnitTypeId();
        })
        // get data from input
        getFromData = function () {
            var fromData = {
                Tc: $(commonName.AutoId).val(),
                UnitTypId: $(commonName.UnitTypeId).val(),
                UnitTypeName: $(commonName.UnitTypeName).val(),
                ShortName: $(commonName.ShortName).val(),
                DecimalPlaces: $(commonName.DecimalPlacesLeftValue).val()||0,
            };
            return fromData;
        }

        //Decimal Places
        $(commonName.DecimalPlacesLeftValue).on('input', function () {
            let valueDecimalPlaces = $(this).val();
            if (valueDecimalPlaces < 0 || valueDecimalPlaces > 5) {
                showToast('warning', "Value Invalid");
                $(commonName.DecimalPlacesLeftValue).addClass('UnitType-input');
                $(commonName.UnitTypeSaveBtn).prop('disabled', true);
            } else {
                $(commonName.DecimalPlacesLeftValue).removeClass('UnitType-input');
                $(commonName.UnitTypeSaveBtn).prop('disabled', false);
                $(commonName.UnitTypeSaveBtn).css('border', 'none');

            }
        })
        //exists 
        $(commonName.UnitTypeName).on('input', function () {

            let UnitTypeValue = $(this).val();

            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(UnitTypeValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.UnitTypeName).addClass('UnitType-input');
                        $(commonName.UnitTypeSaveBtn).prop('disabled', true);
                    } else {
                        $(commonName.UnitTypeName).removeClass('UnitType-input');
                        $(commonName.UnitTypeSaveBtn).prop('disabled', false);
                        $(commonName.UnitTypeSaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })
        //create and edit
        // Save Button Click
        $(document).on('click', commonName.UnitTypeSaveBtn, function () {
            var $btn = $(commonName.UnitTypeSaveBtn);
            if ($btn.data('submitting')) return;
            $btn.data('submitting', true);
            var fromData = getFromData();
            if (fromData.UnitTypeName == null || fromData.UnitTypeName.trim() === '') {
                $(commonName.UnitTypeName).addClass('UnitType-input');
                $(commonName.UnitTypeSaveBtn).prop('disabled', true);
                $(commonName.UnitTypeName).focus();
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
                        $(document).trigger('data:changed', ['unit']);
                        showToast("success", res.message);
                        loadCategoryData();
                        resetFrom();
                        autoUnitTypeId();
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
        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });
        // Reload DataTable Function
        function loadCategoryData() {
            table.ajax.reload(null, false);
        }

        var table = $('#unitTypeTable').DataTable({
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
                    "data": "tc",
                    "render": function (data) {
                        return `<input type="checkbox" class="row-checkbox" value=${data} />`;
                    },
                    "orderable": false
                },
                {
                    "data": "unitTypId",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link btn-edit" data-id=${data}>${data}</button>`;
                    }
                },
                { "data": "unitTypeName" },
                { "data": "shortName" },
                { "data": "decimalPlaces" }
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
                    selectedIds.push(res.result.tc + '');
                    $(commonName.AutoId).val(res.result.tc);
                    $(commonName.UnitTypeName).val(res.result.unitTypeName);
                    $(commonName.ShortName).val(res.result.shortName);
                    $(commonName.UnitTypeId).val(res.result.unitTypId);
                    $(commonName.DecimalPlacesLeftValue).val(res.result.decimalPlaces);
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
                    autoUnitTypeId();
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
            autoUnitTypeId();
            table;
        };
        init();

    };
})(jQuery);
