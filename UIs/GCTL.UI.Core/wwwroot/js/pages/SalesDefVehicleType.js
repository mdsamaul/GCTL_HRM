(function ($) {
    $.SalesDefVehicleTypeJs = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            ShortName: "#ShortName",
            VehicleTypeName: "#VehicleType",
            VehicleTypeID: "#VehicleTypeID",
            AutoId: "#TC",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBrn: ".btn-transport-edit",
            VehicleTypeSaveBtn: ".js-transport-save",
            DeleteBtn: "#js-transport-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-transport-clear",
        }, options);

        var loadVehicleTypeDataUrl = commonName.baseUrl + "/LoadData";
        var autoVehicleTypeIdUrl = commonName.baseUrl + "/AutoVehicleTypeId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteTransport";
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
        autoVehicleTypeId = function () {
            $.ajax({
                url: autoVehicleTypeIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.VehicleTypeID).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.VehicleTypeName).val('');
            $(commonName.ShortName).val('');
            $(commonName.CreateDate).text('');
            $(commonName.UpdateDate).text('');

            autoVehicleTypeId();
        }
        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
        })
        // get data from input
        getFromData = function () {
            var fromData = {
                TC: $(commonName.AutoId).val(),
                VehicleTypeID: $(commonName.VehicleTypeID).val(),
                VehicleType: $(commonName.VehicleTypeName).val(),
                ShortName: $(commonName.ShortName).val(),
            };
            return fromData;
        }
        //exists 
        $(commonName.VehicleTypeName).on('input', function () {

            let VehicleTypeValue = $(this).val();
            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(VehicleTypeValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.VehicleTypeName).addClass('VehicleType-input');
                        $(commonName.VehicleTypeSaveBtn).prop('disabled', true);
                    } else {
                        $(commonName.VehicleTypeName).removeClass('VehicleType-input');
                        $(commonName.VehicleTypeSaveBtn).prop('disabled', false);
                        $(commonName.VehicleTypeSaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })

        
        //create and edit
        // Save Button Click
        $(document).on('click', commonName.VehicleTypeSaveBtn, function () {
            var fromData = getFromData();
            if (fromData.VehicleType == null || fromData.VehicleType.trim() === '') {
                $(commonName.VehicleTypeName).addClass('VehicleType-input');
                $(commonName.VehicleTypeSaveBtn).prop('disabled', true);
                $(commonName.VehicleTypeName).focus();
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
                    autoVehicleTypeId();
                    loadCategoryData();
                }
            });
        });

        // Reload DataTable Function
        function loadCategoryData() {
            table.ajax.reload(null, false);
        }

        var table = $('#VehicleTypeTable').DataTable({
            destroy: true,
            "autoWidth": true,
            "ajax": {
                "url": loadVehicleTypeDataUrl,
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
                    "data": "vehicleTypeID",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link btn-transport-edit" data-id=${data}>${data}</button>`;
                    }
                },
                { "data": "vehicleType" },
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
                    selectedIds.push(res.result.tc + '');
                    $(commonName.AutoId).val(res.result.tc);
                    $(commonName.VehicleTypeName).val(res.result.vehicleType);
                    $(commonName.ShortName).val(res.result.shortName);
                    $(commonName.VehicleTypeID).val(res.result.vehicleTypeID);
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
                    autoVehicleTypeId();
                    loadCategoryData();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            })
        })


        window.VehicleTypeModuleLoaded = true;
        // Initialize all functions
        var init = function () {
            stHeader();
            autoVehicleTypeId();
            table;
        };
        init();

    };
})(jQuery);
