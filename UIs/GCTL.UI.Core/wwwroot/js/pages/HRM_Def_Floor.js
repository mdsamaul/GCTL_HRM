(function ($) {
    $.HRM_Def_FloorJs = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            ShortName: "#ShortName",
            FloorName: "#FloorName",
            FloorID: "#FloorCode",
            AutoId: "#AutoId",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBrn: ".btn-edit",
            FloorSaveBtn: ".js-floor-save",
            DeleteBtn: "#js-floor-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-floor-clear",
        }, options);

        var loadCategoryDataUrl = commonName.baseUrl + "/LoadData";
        var autoFloorIdUrl = commonName.baseUrl + "/AutoFloorId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteFloor";
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
        autoFloorId = function () {
            $.ajax({
                url: autoFloorIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.FloorID).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.FloorName).val('');
            $(commonName.ShortName).val('');
            autoFloorId();
        }
        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
        })
        // get data from input
        getFromData = function () {
            var fromData = {
                AutoId: $(commonName.AutoId).val(),
                FloorCode: $(commonName.FloorID).val(),
                FloorName: $(commonName.FloorName).val(),
                ShortName: $(commonName.ShortName).val(),
            };
            return fromData;
        }
        //exists 
        $(commonName.FloorName).on('input', function () {

            let FloorValue = $(this).val();
            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(FloorValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.FloorName).addClass('floor-input');
                        $(commonName.FloorSaveBtn).prop('disabled', true);
                    } else {
                        $(commonName.FloorName).removeClass('floor-input');
                        $(commonName.FloorSaveBtn).prop('disabled', false);
                        $(commonName.FloorSaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })
        //create and edit
        // Save Button Click
        $(document).on('click', commonName.FloorSaveBtn, function () {
            var fromData = getFromData();
            if (fromData.FloorName == null || fromData.FloorName.trim() === '') {
                $(commonName.FloorName).addClass('floor-input');
                $(commonName.FloorSaveBtn).prop('disabled', true);
                $(commonName.FloorName).focus();
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
                    autoFloorId();
                    loadCategoryData();
                }
            });
        });

        // Reload DataTable Function
        function loadCategoryData() {
            table.ajax.reload(null, false);
        }

        var table = $('#floorTable').DataTable({
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
                    "data": "floorCode",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link btn-edit" data-id=${data}>${data}</button>`;
                    }
                },
                { "data": "floorName" },
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
                    $(commonName.FloorName).val(res.result.floorName);
                    $(commonName.ShortName).val(res.result.shortName);
                    $(commonName.FloorID).val(res.result.floorCode);
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
                    autoFloorId();
                    loadCategoryData();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            })
        })


        window.floorModuleLoaded = true;
        // Initialize all functions
        var init = function () {
            stHeader();
            autoFloorId();
            table;
        };
        init();

    };
})(jQuery);
