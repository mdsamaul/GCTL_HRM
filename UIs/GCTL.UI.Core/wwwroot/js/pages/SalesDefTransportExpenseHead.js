(function ($) {
    $.SalesDefTransportExpenseHeadJs = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            ShortName: "#ShortName",
            TransportExpenseName: "#ExpenseHead",
            TransportExpenseID: "#ExpenseHeadID",
            AutoId: "#TC",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBrn: ".btn-Transport-Expense-edit",
            TransportExpenseSaveBtn: ".js-transport-save",
            DeleteBtn: "#js-transport-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-transport-clear",
        }, options);

        var loadTransportExpenseDataUrl = commonName.baseUrl + "/LoadData";
        var autoTransportExpenseIdUrl = commonName.baseUrl + "/AutoId";
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
        autoTransportExpenseId = function () {
            $.ajax({
                url: autoTransportExpenseIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.TransportExpenseID).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.TransportExpenseName).val('');
            $(commonName.ShortName).val('');
            $(commonName.CreateDate).text('');
            $(commonName.UpdateDate).text('');

            autoTransportExpenseId();
        }
        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
        })
        // get data from input
        getFromData = function () {
            var fromData = {
                TC: $(commonName.AutoId).val(),
                ExpenseHeadID: $(commonName.TransportExpenseID).val(),
                ExpenseHead: $(commonName.TransportExpenseName).val(),
                ShortName: $(commonName.ShortName).val(),
            };
            return fromData;
        }
        //exists 
        $(commonName.TransportExpenseName).on('input', function () {

            let TransportExpenseValue = $(this).val();
            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(TransportExpenseValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.TransportExpenseName).addClass('TransportExpense-input');
                        $(commonName.TransportExpenseSaveBtn).prop('disabled', true);
                    } else {
                        $(commonName.TransportExpenseName).removeClass('TransportExpense-input');
                        $(commonName.TransportExpenseSaveBtn).prop('disabled', false);
                        $(commonName.TransportExpenseSaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })
        //create and edit
        // Save Button Click
        $(document).on('click', commonName.TransportExpenseSaveBtn, function () {
            var fromData = getFromData();
            if (fromData.ExpenseHead == null || fromData.ExpenseHead.trim() === '') {
                $(commonName.TransportExpenseName).addClass('TransportExpense-input');
                $(commonName.TransportExpenseSaveBtn).prop('disabled', true);
                $(commonName.TransportExpenseName).focus();
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
                    autoTransportExpenseId();
                    loadCategoryData();
                }
            });
        });

        // Reload DataTable Function
        function loadCategoryData() {
            table.ajax.reload(null, false);
        }

        var table = $('#TransportExpenseTable').DataTable({
            "autoWidth": true,
            "ajax": {
                "url": loadTransportExpenseDataUrl,
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
                    "data": "expenseHeadID",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link btn-Transport-Expense-edit" data-id=${data}>${data}</button>`;
                    }
                },
                { "data": "expenseHead" },
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
                    $(commonName.TransportExpenseName).val(res.result.expenseHead);
                    $(commonName.ShortName).val(res.result.shortName);
                    $(commonName.TransportExpenseID).val(res.result.expenseHeadID);
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
                    autoTransportExpenseId();
                    loadCategoryData();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            })
        })


        window.TransportExpenseModuleLoaded = true;
        // Initialize all functions
        var init = function () {
            stHeader();
            autoTransportExpenseId();
            table;
        };
        init();

    };
})(jQuery);
