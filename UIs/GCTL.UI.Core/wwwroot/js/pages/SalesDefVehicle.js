(function ($) {
    $.SalesDefVehicleJs = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            VehicleNo: "#VehicleNo",
            TransportCapacity: "#TransportCapacity",
            VehicleID: "#VehicleID",
            AutoId: "#TC",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBrn: ".btn-transport-info-edit",
            VehicleTypeSaveBtn: ".js-transport-info-save",
            DeleteBtn: "#js-transport-info-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-transport-info-clear",
            VehicleTypeModalBtn: "#trapnsportTypeAddmoreDetailsBtn",
            VehicleTypeModalContainer: "#VehicleTypeModalContainer",
            CloaseLoadedTransportTypelistBtn: ".cloaseLoadedTransportTypelistBtn",
            CloaseLoadedTransportTypelistSelectContainer: "#CloaseLoadedTransportTypelistSelectContainer",
            CompanySelectValue:".companySelectValue",
        }, options);

        var loadVehicleTypeDataUrl = commonName.baseUrl + "/LoadData";
        var AutoTransportInfoIdUrl = commonName.baseUrl + "/AutoTransportInfoId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteTransport";
        var alreadyExistUrl = commonName.baseUrl + "/alreadyExist";
        var VehicleTypeModuleLoadedUrl = "/SalesDefVehicleType/Index?isPartial=true";
        var CloaseLoadedTransportTypelistUrl = commonName.baseUrl + "/CloaseLoadedTransportTypelist";
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

        $(commonName.VehicleTypeModalBtn).on('click', function () {
            $.ajax({
                url: VehicleTypeModuleLoadedUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.VehicleTypeModalContainer).html(res);
                    if (typeof $.SalesDefVehicleTypeJs == 'function') {
                        var options = {
                            baseUrl: '/SalesDefVehicleType',
                            isPartial: true,
                        };
                        $.SalesDefVehicleTypeJs(options);
                    }
                },
                error: function (e) {
                    showToast("error", "Failed to load module data.");
                }
            });
        })

        $(commonName.CloaseLoadedTransportTypelistBtn).on('click', function () {
            $.ajax({
                url: CloaseLoadedTransportTypelistUrl,
                type: "GET",
                success: function (res) {$(commonName.CloaseLoadedTransportTypelistSelectContainer).empty();
                    res.transportTypeList.forEach(item => {
                        $(commonName.CloaseLoadedTransportTypelistSelectContainer).append(`
                        <option value=${item.vehicleTypeId}>${item.vehicleType}</option>
                        `);
                    })
                    
                }
            });
        });
        AutoTransportInfoId = function () {
            $.ajax({
                url: AutoTransportInfoIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.VehicleID).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.VehicleNo).val('');
            $(commonName.TransportCapacity).val('');
            $(commonName.CloaseLoadedTransportTypelistSelectContainer).val('').trigger('change');
            $(commonName.ShortName).val('');
            $(commonName.CreateDate).text('');
            $(commonName.UpdateDate).text('');

            AutoTransportInfoId();
        }
        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
        })
        // get data from input
        getFromData = function () {
            var fromData = {
                TC: $(commonName.AutoId).val(),
                CompanyCode: $(commonName.CompanySelectValue).val(),
                VehicleID: $(commonName.VehicleID).val(),
                VehicleTypeID: $(commonName.CloaseLoadedTransportTypelistSelectContainer).val(),
                VehicleNo: $(commonName.VehicleNo).val(),
                TransportCapacity: $(commonName.TransportCapacity).val(),
            };
            return fromData;
        }
        //exists 
        $(commonName.VehicleNo).on('input', function () {

            let VehicleTypeValue = $(this).val();
            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(VehicleTypeValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.VehicleNo).addClass('VehicleType-input');
                        $(commonName.VehicleTypeSaveBtn).prop('disabled', true);
                    } else {
                        $(commonName.VehicleNo).removeClass('VehicleType-input');
                        $(commonName.VehicleTypeSaveBtn).prop('disabled', false);
                        $(commonName.VehicleTypeSaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })
        //exists
        $(commonName.CloaseLoadedTransportTypelistSelectContainer).on('change', function () {
            $(commonName.VehicleNo).removeClass('VehicleType-input');
            $(commonName.VehicleTypeSaveBtn).prop('disabled', false);
            $(commonName.VehicleTypeSaveBtn).css('border', 'none');
        })
        //create and edit

        $('.searchable-select').select2({
            placeholder: 'Select an option',
            allowClear: false,
            width: '100%'
        });
        // Save Button Click
        $(document).on('click', commonName.VehicleTypeSaveBtn, function () {
            var fromData = getFromData();
            if (fromData.VehicleTypeID == null || fromData.VehicleTypeID.trim() === '') {
                $(commonName.CloaseLoadedTransportTypelistSelectContainer).addClass('VehicleType-input');
                $(commonName.VehicleTypeSaveBtn).prop('disabled', true);
                $(commonName.CloaseLoadedTransportTypelistSelectContainer).focus();
                $(commonName.CloaseLoadedTransportTypelistSelectContainer).select2('open');
                return;
            }
            if (fromData.VehicleNo == null || fromData.VehicleNo.trim() === '') {
                $(commonName.VehicleNo).addClass('VehicleType-input');
                $(commonName.VehicleTypeSaveBtn).prop('disabled', true);
                $(commonName.VehicleNo).focus();
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
                    AutoTransportInfoId();
                    loadCategoryData();
                }
            });
        });

        // Reload DataTable Function
        function loadCategoryData() {
            table.ajax.reload(null, false);
        }

        var table = $('#VehicleTransportInfoTable').DataTable({
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
                    "data": "vehicleID",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link btn-transport-info-edit" data-id=${data}>${data}</button>`;
                    }
                },
                {"data": "vehicleNo" },
                {"data": "vehicleTypeName" },
                {"data": "transportCapacity"},
                {"data": "companyName" }
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
                    $(commonName.VehicleID).val(res.result.vehicleID);
                    $(commonName.VehicleNo).val(res.result.vehicleNo);
                    $(commonName.CompanySelectValue).val(res.result.companyCode).trigger('change');
                    $(commonName.CloaseLoadedTransportTypelistSelectContainer).val(res.result.vehicleTypeID).trigger('change');
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
                    AutoTransportInfoId();
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
            AutoTransportInfoId();
            table;
        };
        init();

    };
})(jQuery);
