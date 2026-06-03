(function ($) {
    $.Core_CountryJs = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            CountryName: "#CountryName",
            CountryID: "#CountryID",
            AutoId: "#CountryCode",
            ISOCode: "#ISOCode",
            IOCCode: "#IOCCode",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBrn: ".btn-edit",
            CountrySaveBtn: ".js-country-save",
            DeleteBtn: "#js-country-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-country-clear",
        }, options);

        var loadCountryDataUrl = commonName.baseUrl + "/LoadData";
        var autoCountryIdUrl = commonName.baseUrl + "/AutoCountryId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteCountry";
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
        autoCountryId = function () {
            $.ajax({
                url: autoCountryIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.CountryID).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.CountryName).val('');
            $(commonName.IOCCode).val('');
            $(commonName.ISOCode).val('');
            autoCountryId();
        }
        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
        })
        // get data from input
        getFromData = function () {
            var fromData = {
                CountryCode: $(commonName.AutoId).val(),
                CountryID: $(commonName.CountryID).val(),
                CountryName: $(commonName.CountryName).val(),
                IOCCode: $(commonName.IOCCode).val(),
                ISOCode: $(commonName.ISOCode).val(),
            };
            return fromData;
        }
        //exists 
        $(commonName.CountryName).on('input', function () {

            let CountryValue = $(this).val();
            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(CountryValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.CountryName).addClass('country-input');
                        $(commonName.CountrySaveBtn).prop('disabled', true);
                    } else {
                        $(commonName.CountryName).removeClass('country-input');
                        $(commonName.CountrySaveBtn).prop('disabled', false);
                        $(commonName.CountrySaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })
        //create and edit
        // Save Button Click
        $(document).on('click', commonName.CountrySaveBtn, function () {
            var fromData = getFromData();
            if (fromData.CountryName == null || fromData.CountryName.trim() === '') {
                $(commonName.CountryName).addClass('country-input');
                $(commonName.CountrySaveBtn).prop('disabled', true);
                $(commonName.CountryName).focus();
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
                    autoCountryId();
                    loadCountryData();
                }
            });
        });

        // Reload DataTable Function
        function loadCountryData() {
            table.ajax.reload(null, false);
        }

        var table = $('#countryTable').DataTable({
            "autoWidth": true,
            "ajax": {
                "url": loadCountryDataUrl,
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
                    "data": "countryCode",
                    "render": function (data) {
                        return `<input type="checkbox" class="row-checkbox" value=${data} />`;
                    },
                    "orderable": false
                },
                {
                    "data": "countryID",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link btn-edit" data-id=${data}>${data}</button>`;
                    }
                },
                { "data": "countryName" },
                {"data": "iocCode" },
                { "data": "isoCode" }
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
                    selectedIds.push(res.result.countryCode + '');
                    $(commonName.AutoId).val(res.result.countryCode);
                    $(commonName.CountryName).val(res.result.countryName);
                    $(commonName.ISOCode).val(res.result.isoCode);
                    $(commonName.IOCCode).val(res.result.iocCode);
                    $(commonName.CountryID).val(res.result.countryID);
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
                    autoCountryId();
                    loadCountryData();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            })
        })


        window.countryModuleLoaded = true;
        // Initialize all functions
        var init = function () {
            stHeader();
            autoCountryId();
            table;
        };
        init();

    };
})(jQuery);
