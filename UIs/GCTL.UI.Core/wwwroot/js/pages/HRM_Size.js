(function ($) {
    $.HRM_SizeJs = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            SizeShortName: "#sizeShortName",
            SizeName: "#sizeName",
            SizeID: "#sizeId",
            AutoId: "#Setup_AutoId",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBtn: ".btn-edit",
            SizeSaveBtn: ".js-size-save",
            DeleteBtn: "#js-size-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBtn: "#js-size-clear",
        }, options);

        var loadSizeDataUrl = commonName.baseUrl + "/LoadData";
        var autoSizeIDUrl = commonName.baseUrl + "/AutoSizeID";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteSize";
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
        autoSizeID = function () {
            $.ajax({
                url: autoSizeIDUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.SizeID).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.SizeName).val('');
            $(commonName.SizeShortName).val('');
            $(commonName.CreateDate).text('');
            $(commonName.UpdateDate).text('');
        }
        $(commonName.ClearBtn).on('click', function () {
            resetFrom();
            autoSizeID();
        })
        // get data from input
        getFromData = function () {
            var fromData = {
                AutoId: $(commonName.AutoId).val(),
                SizeID: $(commonName.SizeID).val(),
                SizeName: $(commonName.SizeName).val(),
                ShortName: $(commonName.SizeShortName).val(),
            };
            return fromData;
        }
        //exists 
        $(commonName.SizeName).on('input', function () {

            let CatagoryValue = $(this).val();

            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(CatagoryValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.SizeName).addClass('catagory-input');
                        $(commonName.SizeSaveBtn).prop('disabled', true);
                        $(commonName.SizeSaveBtn).css('border', 'none');
                    } else {
                        $(commonName.SizeName).removeClass('catagory-input');
                        $(commonName.SizeSaveBtn).prop('disabled', false);
                        $(commonName.SizeSaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })
        //create and edit
        // Save Button Click
        $(document).on('click', commonName.SizeSaveBtn, function () {
            var $btn = $(commonName.SizeSaveBtn);
            if ($btn.data('submitting')) return;
            $btn.data('submitting', true);
            var fromData = getFromData();
            if (fromData.SizeName == null || fromData.SizeName.trim() === '') {
                $(commonName.SizeName).addClass('catagory-input');
                $(commonName.SizeSaveBtn).prop('disabled', true);
                $(commonName.SizeName).focus();
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
                        $(document).trigger('data:changed', ['size']);
                        showToast("success", res.message);
                        loadSizeData();
                        resetFrom();
                        autoSizeID();
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
        function loadSizeData() {
            table.ajax.reload(null, false);
        }

        var table = $('#SizeTable').DataTable({            
            "autoWidth": true,
            "ajax": {
                "url": loadSizeDataUrl,
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
                    "data": "sizeID",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link btn-edit" data-id=${data}>${data}</button>`;
                    }
                },
                { "data": "sizeName" },
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
        $(document).on('click', commonName.EditBtn, function () {
            let id = $(this).data('id');
            $.ajax({
                url: `${PopulatedDataForUpdateUrl}?id=${id}`,
                type: "GET",
                success: function (res) {
                    selectedIds = [];
                    selectedIds.push(res.result.autoId + '');
                    $(commonName.AutoId).val(res.result.autoId);
                    $(commonName.SizeName).val(res.result.sizeName);
                    $(commonName.SizeShortName).val(res.result.shortName);
                    $(commonName.SizeID).val(res.result.sizeID);
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
                    autoSizeID();
                    loadSizeData();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            })
        })


        window.SizeModuleLoaded = true;
        // Initialize all functions
        var init = function () {
            stHeader();
            autoSizeID();
            table;
        };
        init();

    };
})(jQuery);
