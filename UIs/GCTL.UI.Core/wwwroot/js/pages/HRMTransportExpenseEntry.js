(function ($) {
    $.HRMTransportExpenseEntryJs = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            autoTeid: "#teid",
            ExpenseHeadList: "#ExpenseHeadList",
            TeDate: "#teDate",
            TransportUser: "#UserSelectEmpId",
            Active: "#Active",
            TransportAssignEntryId: "#TAID",
            AutoId: "#TransportExpenseAutoId",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBrn: ".btn-transport-expense-entry-edit",
            TransportExpenseSaveBtn: ".js-transport-entry-assign-save",
            DeleteBtn: "#js-transport-expense-entry-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-transport-Expense-entry-clear",
            DEmpName: "#DEmpName",
            DEmpDesignation: "#DEmpDesignation",
            DEmpDepartment: "#DEmpDepartment",
            DEmpPhone: "#DEmpPhone",

            ExpenceTransportAddMoreBtn: "#ExpenceTransportAddMore",
            ExpenceTransportContainer: "#ExpenceTransportContainer",
            ExpenceTransportDeleteBtn: ".ExpenceTransportDeleteBtn",
            AddMoreExpenceHead: ".addMoreExpenceHead",
            ExpenseHeadId: "#ExpenseHeadId",
            Amount: "#Amount",
            Remarks: "#Remarks",
            DetailsAutoid: "#detailsAutoid",
            TransportNoId: "#TransportNoId",
            TransportDriverDisable: "#TransportDriverDisable",
            TransportTypeDisable: "#TransportTypeDisable",
            TempDetailsClear:"#TempDetailsClear",
        }, options);

        var loadTransportExpenseDataUrl = commonName.baseUrl + "/LoadData";
        var autoIdUrl = commonName.baseUrl + "/AutoId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteTransport";
        var alreadyExistUrl = commonName.baseUrl + "/alreadyExist";
        var LoadEmpDetailsUrl = commonName.baseUrl + "/GetEmpDetailsId";
        var transportTypeUrl = commonName.baseUrl + "/transportTypeGetByTransportNo";

        var TransportExpenseDetailsUrl = commonName.baseUrl + "/TransportExpenseDetails";
        var TransportExpenseTempDetailsListUrl = commonName.baseUrl + "/TransportExpenseTempDetailsList";
        var TransportExpenseTempDetailsEditUrl = commonName.baseUrl + "/TransportExpenseTempDetailsEdit";
        var TransportExpenseMasterDetailsEditUrl = commonName.baseUrl + "/TransportExpenseMasterDetailsEdit";
        var DeleteTransportExpenseUrl = commonName.baseUrl + "/DeleteTransportExpense";
        var GetAllTransportDetailsUrl = commonName.baseUrl + "/GetAllTransportDetails";
        var ReloadDataBackTempToDetailsUrl = commonName.baseUrl + "/ReloadDataBackTempToDetails";

        // Global variables
        let selectedIds = [];
        let UserEmplist = [];
        let table; // Declare table variable globally

        // Sticky header on scroll
        function stHeader() {
            window.addEventListener('scroll', function () {
                const header = document.getElementById('stickyHeader');
                if (header && window.scrollY > 10) {
                    header.classList.add('scrolled');
                } else if (header) {
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

        function DetailsTempData() {
            var DetailsData = {
                Teid: $(commonName.autoTeid).val(),
                Tedid: $(commonName.TransportNoId).val(),
                ExpenseHeadId: $(commonName.ExpenseHeadId).val(),
                Amount: $(commonName.Amount).val(),
                Remarks: $(commonName.Remarks).val(),
                AutoId: $(commonName.DetailsAutoid).val() || 0
            };
            return DetailsData;
        }
        $(document).on('click', commonName.TempDetailsClear, function () {
            ResetDetailsTempData();
        })
        function ResetDetailsTempData() {
            $(commonName.ExpenseHeadId).val('').multiselect('refresh');
            $(commonName.Amount).val('');
            $(commonName.Remarks).val('');
            $(commonName.DetailsAutoid).val(0);
        }
        $(document).on('input', commonName.Amount, function () {
            var amount = $(this).val();
            if (amount<0) {
                $(commonName.Amount).addClass('transportExpenseAmount');
                $(commonName.ExpenceTransportAddMoreBtn).prop('disabled', true);
                return
            }
            $(commonName.Amount).removeClass('transportExpenseAmount');
            $(commonName.ExpenceTransportAddMoreBtn).prop('disabled', false);
        })


        $(document).on('click', commonName.ExpenceTransportAddMoreBtn, function () {
            var detailsData = DetailsTempData();
            if (!detailsData.Tedid || detailsData.Tedid.trim() === "") {
                $(commonName.ExpenceTransportAddMoreBtn).prop('disabled', true);
                var $dropdownTransportNoId = $('#TransportNoId');
                if ($dropdownTransportNoId.length && $dropdownTransportNoId.data('multiselect')) {
                    var $dTButton = $dropdownTransportNoId.siblings('.btn-group').find('button.multiselect');
                    if ($dTButton.length) {
                        $dTButton.focus();
                        setTimeout(function () {
                            $dTButton.click();
                        }, 50);
                    }
                }
                return;
            }

            if (!detailsData.ExpenseHeadId || detailsData.ExpenseHeadId.trim() === "") {
                $(commonName.ExpenceTransportAddMoreBtn).prop('disabled', true);
                var $dropdownExpenseHead = $('#ExpenseHeadId');
                if ($dropdownExpenseHead.length && $dropdownExpenseHead.data('multiselect')) {
                    var $dehButton = $dropdownExpenseHead.siblings('.btn-group').find('button.multiselect');
                    if ($dehButton.length) {
                        $dehButton.focus();
                        setTimeout(function () {
                            $dehButton.click();
                        },50);
                    }
                }
                return;
            }
            if (!detailsData.Amount || detailsData.Amount.trim() === "" || detailsData.Amount < 0) {
                $(commonName.ExpenceTransportAddMoreBtn).prop('disabled', true);
                $(commonName.Amount).focus();
                $(commonName.Amount).addClass('transportExpenseAmount');
                return;
            }


            //if (!fromData.TEDID || fromData.TEDID.trim() === '') {
            //    $(commonName.TransportExpenseSaveBtn).prop('disabled', true);

            //    var $dropdownTransportNoId = $('#TransportNoId');
            //    if ($dropdownTransportNoId.length && $dropdownTransportNoId.data('multiselect')) {
            //        var $button = $dropdownTransportNoId.siblings('.btn-group').find('button.multiselect');
            //        if ($button.length) {
            //            $button.focus();
            //            setTimeout(function () {
            //                $button.click();
            //            }, 50);
            //        }
            //    }
            //    return;
            //}






            $.ajax({
                url: TransportExpenseDetailsUrl,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(detailsData),
                success: function (res) {
                    if (res.isSuccess) {
                        DetailsTempDataTable();
                        ResetDetailsTempData();
                        showToast('success', res.message);
                    } else {
                        showToast("error", res.message);
                    }
                }
            });
        });

        // Load DataTable with better error handling
        function DetailsTempDataTable() {
            // Check if table element exists
            if (!$('#TransportExpenseDetailsTempTable').length) {
                return;
            }

            // Destroy existing DataTable if it exists
            if ($.fn.DataTable.isDataTable('#TransportExpenseDetailsTempTable')) {
                $('#TransportExpenseDetailsTempTable').DataTable().clear().destroy();
            }

            try {
                $('#TransportExpenseDetailsTempTable').DataTable({
                    ajax: {
                        url: TransportExpenseTempDetailsListUrl,
                        type: "GET",
                        datatype: "json",
                        dataSrc: function (json) {
                            return json.data || []; 
                        },
                        error: function (xhr, error, thrown) {
                            showToast("error", "Failed to load data: " + error);
                        }
                    },
                    responsive: true,
                    autoWidth: false,
                    processing: true,
                    serverSide: false,
                    paging: false,
                    searching: true,
                    ordering: false,
                    info: true,
                    lengthChange: true,
                    pageLength: 'All',
                    columns: [
                        {
                            data: 'autoId',
                            render: function (data, type, row, meta) {
                                if (!data) return '';
                                var sl = (meta.row + 1).toString().padStart(3, '0');
                                return `<a href="javascript:void(0);" class="serial-edit" data-id="${row.autoId || ''}">${sl}</a>`;
                            },
                            width: "150px"
                        },
                        {
                            data: 'expenseHead',
                            name: 'Expense Head',
                            defaultContent: ''
                        },{
                            data: 'amount',
                            name: 'Amount',
                            defaultContent: ''
                        },
                        {
                            data: 'remarks',
                            name: 'Remarks',
                            defaultContent: ''
                        },
                        {
                            data: null,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                return `                       
                        <button type="button" class="btn btn-sm btn-danger deleteDetailsExpenseBtn" data-id="${row.autoId || ''}">
                            <i class="fas fa-times"></i>
                        </button>
                    `;
                            }
                        }
                    ]
                });
            } catch (error) {
                showToast("error", "Failed to initialize table: " + error.message);
            }
        }

        $(document).on("click", ".serial-edit", function () {
            var id = $(this).data("id");
            EditTransportExpense(id);
        });

        

        // Example Edit function
        function EditTransportExpense(autoId) {
            $.ajax({
                url: `${TransportExpenseTempDetailsEditUrl}/${autoId}`,
                type: "GET",
                success: function (res) {
                    if (res.data) {
                        $("#Amount").val(res.data.amount || '');
                        $("#Remarks").val(res.data.remarks || '');
                        $("#ExpenseHeadId").val(res.data.expenseHeadId || '').multiselect('rebuild');
                        $("#TransportNoId").val(res.data.tedid).multiselect('rebuild');
                        //$("#TransportNoId").val(res.data.tedid).multiselect('refresh');
                        GetAllTransportDetailsFun(res.data.tedid);
                        $("#teid").val(res.data.teid);
                        $(commonName.DetailsAutoid).val(res.data.autoId || '');
                    }
                },
                error: function (xhr) {
                    showToast("error", "Error loading expense data. Please try again.");
                }
            });
        }
       
        function EditTransportMasterExpense(autoId) {
            $.ajax({
                url: `${TransportExpenseMasterDetailsEditUrl}/${autoId}`,
                type: "GET",
                success: function (res) {
                    if (res.data) {
                        //$("#Amount").val(res.data.amount || '');
                        //$("#Remarks").val(res.data.remarks || '');
                        //$("#ExpenseHeadId").val(res.data.expenseHeadId || '').multiselect('rebuild');
                        $("#TransportNoId").val(res.data.tedid).multiselect('rebuild');
                        //$("#TransportNoId").val(res.data.tedid).multiselect('refresh');
                        GetAllTransportDetailsFun(res.data.tedid);
                        $("#teid").val(res.data.teid);
                        //$(commonName.DetailsAutoid).val(res.data.autoId || '');
                    }
                },
                error: function (xhr) {
                    showToast("error", "Error loading expense data. Please try again.");
                }
            });
        }

        // Delete Row
        $(document).on('click', '.deleteDetailsExpenseBtn', function () {
            var id = $(this).data("id");
            $.ajax({
                url: DeleteTransportExpenseUrl + "?id=" + id,
                type: "POST",
                success: function (res) {
                    DetailsTempDataTable();
                }
            });
        });

        $(document).on('change', commonName.TransportNoId, function () {
            var transportNo = $(this).val();
            GetAllTransportDetailsFun(transportNo);
        });

        function GetAllTransportDetailsFun(trnsId) {
            $.ajax({
                url: GetAllTransportDetailsUrl + "?trnsId=" + trnsId,
                type: "POST",
                success: function (res) {
                    var $dropdownTransportTypeDisable = $("#TransportTypeDisable");
                    $dropdownTransportTypeDisable.empty();
                    var $dropdownTransportDriverDisable = $("#TransportDriverDisable");
                    $dropdownTransportDriverDisable.empty();
                    var $dropdownTransportDriverPhoneDisable = $("#TransportDriverPhone");
                    $dropdownTransportDriverPhoneDisable.empty();
                    var $dropdownTransportDriverInUserDisable = $("#TransportDriverInUser");
                    $dropdownTransportDriverInUserDisable.empty();

                    if (res.data && res.data.length > 0) {
                        $.each(res.data, function (i, transport) {
                            $dropdownTransportTypeDisable.append(
                                $('<option></option>').val(transport.transportTypeId || '').text(transport.transportType || '')
                            );
                            $dropdownTransportDriverDisable.append(
                                $('<option></option>').val(transport.driverId || '').text(transport.driverName || '')
                            );
                            $dropdownTransportDriverPhoneDisable.append(
                                $('<option></option>').val(transport.driverPhone || '').text(transport.driverPhone || '')
                            );
                          
                            if (transport.transportUsers && transport.transportUsers.length > 0) {
                                var TotalUser = transport.transportUsers.length || 0;
                                var $dropdown = $('#TransportDriverInUser');
                                $dropdown.empty();

                                $.each(transport.transportUsers, function (i, transportUser) {
                                    $dropdown.append(
                                        $('<option></option>').val(transportUser.userId || '').text(transportUser.userName || '')
                                    );
                                });

                                $('#totalUserCount').text(TotalUser);

                            } else {
                                $('#totalUserCount').text("");
                            }

                        });
                    }

                    // Refresh multiselect if using bootstrap-multiselect
                    if ($dropdownTransportTypeDisable.data('multiselect')) {
                        $dropdownTransportTypeDisable.multiselect('rebuild');
                    }
                    if ($dropdownTransportDriverDisable.data('multiselect')) {
                        $dropdownTransportDriverDisable.multiselect('rebuild');
                    }
                    if ($dropdownTransportDriverPhoneDisable.data('multiselect')) {
                        $dropdownTransportDriverPhoneDisable.multiselect('rebuild');
                    }
                    if ($dropdownTransportDriverInUserDisable.data('multiselect')) {
                        $dropdownTransportDriverInUserDisable.multiselect('rebuild');
                    }
                },
                error: function (e) {
                }
            });
        }

        function GetAllTransportDetailsAllFun() {
            $.ajax({
                url: GetAllTransportDetailsUrl + "?trnsId=" + '',
                type: "POST",
                success: function (res) {
                    var $dropdown = $(commonName.TransportNoId);
                    $dropdown.empty();
                    $dropdown.append('<option value="">--Select Transport--</option>');

                    if (res.data && res.data.length > 0) {
                        $.each(res.data, function (i, transport) {
                            $dropdown.append(
                                $('<option></option>').val(transport.transportID || '').text(transport.transportNo || '')
                            );
                        });
                    }

                    if ($dropdown.data('multiselect')) {
                        $dropdown.multiselect('rebuild');
                    }
                },
                error: function (e) {
                }
            });
        }

        function TransportNoList() {
            var $dropdown = $(commonName.TransportNoId);
            if ($dropdown.length && typeof $.fn.multiselect !== 'undefined') {
                if (!$dropdown.data('multiselect')) {
                    $dropdown.multiselect({
                        enableFiltering: true,
                        enableCaseInsensitiveFiltering: true,
                        filterPlaceholder: 'Search for transport...',
                        buttonWidth: '100%',
                        maxHeight: 300,
                        nonSelectedText: '--Select Transport--',
                        buttonClass: 'btn btn-outline-secondary text-start',
                        onChange: function (option, checked) {
                            let selectedValue = $dropdown.val();
                        }
                    });
                }
            }
        }

        $(document).on('click', commonName.ExpenceTransportDeleteBtn, function () {
            $(this).closest('tr').remove();
        });

        $(document).ready(function () {
            setTimeout(function () {
                $('.multiselect-clear-filter i').removeClass('glyphicon glyphicon-remove-circle')
                    .addClass('fa fa-times-circle');
            }, 200);
        });

        function ExpenseHeadList() {
            var $dropdown = $('#ExpenseHeadId');
            if ($dropdown.length && typeof $.fn.multiselect !== 'undefined') {
                if (!$dropdown.data('multiselect')) {
                    $dropdown.multiselect({
                        enableFiltering: true,
                        enableCaseInsensitiveFiltering: true,
                        filterPlaceholder: 'Search for expense head...',
                        buttonWidth: '100%',
                        maxHeight: 300,
                        nonSelectedText: '--Select Expense Head--',
                        buttonClass: 'btn btn-outline-secondary text-start w-100',
                        onChange: function (option, checked) {
                            let selectedValue = $dropdown.val();
                        }
                    });
                }
            }
        }

        function UserSelectEmpMultiselect() {
            if (typeof $.fn.multiselect === 'undefined') {
                return;
            }

            try {
                if ($('#UserSelectEmpId').length) {
                    $('#UserSelectEmpId').multiselect({
                        includeSelectAllOption: true,
                        selectAllText: 'Select All Users',
                        enableFiltering: true,
                        enableCaseInsensitiveFiltering: true,
                        filterPlaceholder: 'Search users...',
                        buttonWidth: '100%',
                        maxHeight: 300,
                        nonSelectedText: '--Select Users--',
                        allSelectedText: 'All Users Selected',
                        nSelectedText: ' users selected',
                        buttonClass: 'btn btn-outline-secondary',

                        onChange: function (option, checked) {
                            let value = option.val();

                            if (checked) {
                                if (!UserEmplist.includes(value)) {
                                    UserEmplist.push(value);
                                }
                            } else {
                                UserEmplist = UserEmplist.filter(id => id !== value);
                            }
                        },

                        onSelectAll: function () {
                            UserEmplist = $('#UserSelectEmpId option').map(function () {
                                return $(this).val();
                            }).get();
                        },

                        onDeselectAll: function () {
                            UserEmplist = [];
                        }
                    });
                }
            } catch (error) {
            }
        }

        let teDatePicker;
        function initializeDatePicker() {
            //if (typeof flatpickr !== 'undefined' && $("input[name='teDate']").length) {
            //    teDatePicker = flatpickr("input[name='teDate']", {
            //        altInput: true,
            //        altFormat: "d/m/Y",
            //        dateFormat: "Y-m-d",
            //        allowInput: true,
            //        defaultDate: "today"
            //    });
            //}
            flatpickr($("#teDate"), CalendarService.createConfig(
                {
                    defaultDate: new Date(),
                }));
        }

        function isValidDate(dateStr) {
            const date = Date.parse(dateStr);
            return !isNaN(date);
        }

        $(commonName.ExpenseHeadList).on('change', function () {
            var selectedValue = $(this).val();
            $.ajax({
                url: LoadEmpDetailsUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(selectedValue),
                success: function (res) {
                    $(commonName.DEmpName).text(res.data?.empName || '');
                    $(commonName.DEmpDepartment).text(res.data?.department || '');
                    $(commonName.DEmpDesignation).text(res.data?.designation || '');
                    $(commonName.DEmpPhone).text(res.data?.phone || '');
                },
                error: function (e) {
                }
            });
        });

        autoTransportAssignEntryId = function () {
            $.ajax({
                url: autoIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.autoTeid).val(res.data || '');
                },
                error: function (e) {
                }
            });
        };

        resetFrom = function () {
            $(commonName.AutoId).val(0);

            // Reset TransportNo
            $(commonName.TransportNoId).val('');
            if ($(commonName.TransportNoId).data('multiselect')) {
                $(commonName.TransportNoId).multiselect('refresh');
            }

            // Reset Driver
            $(commonName.TransportDriverDisable).empty().append('<option value="">--Select Employee--</option>');
           
            $('#TransportTypeDisable').empty()
                .append('<option value="">--Select Employee--</option>');
            $('#TransportDriverInUser').empty()
                .append('<option value="">--Select User--</option>');
            $('#TransportDriverPhone').val('').prop('placeholder', 'Enter Phone');



            // Reset User Employee List
            UserEmplist = [];
            if ($('#UserSelectEmpId').length && $('#UserSelectEmpId').data('multiselect')) {
                $('#UserSelectEmpId').multiselect('deselectAll', false).multiselect('refresh');
            }

            // Reset Expense Head
            $('#ExpenseHeadList').val('');
            if ($('#ExpenseHeadList').data('multiselect')) {
                $('#ExpenseHeadList').multiselect('refresh');
            }

            // Clear dates
            $(commonName.CreateDate).text("");
            $(commonName.UpdateDate).text("");

            // Reset date picker if exists
            if (typeof teDatePicker !== 'undefined') {
                teDatePicker.setDate("today", true);
            }

            // Generate new ID
            autoTransportAssignEntryId();
        };


        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
            ResetDetailsTempData();
            ReloadDataBackTempToDetails();
        });

        getFromData = function () {
            var fromData = {
                AutoId: $(commonName.AutoId).val() || 0,
                TEDID: $(commonName.TransportNoId).val(),
                TEID: $(commonName.autoTeid).val(),
                TEDate: $(commonName.TeDate).val(),
            };
            return fromData;
        };

        $([commonName.TransportNoId, commonName.TeDate, commonName.ExpenseHeadId].join(',')).on('change', function () {
            $(commonName.TransportExpenseSaveBtn).prop('disabled', false);
            $(commonName.ExpenceTransportAddMoreBtn).prop('disabled', false);
        });

        $(document).on('click', commonName.TransportExpenseSaveBtn, function () {
            var fromData = getFromData();

            if (!fromData.TEDID || fromData.TEDID.trim() === '') {
                $(commonName.TransportExpenseSaveBtn).prop('disabled', true);

                var $dropdownTransportNoId = $('#TransportNoId');
                if ($dropdownTransportNoId.length && $dropdownTransportNoId.data('multiselect')) {
                    var $button = $dropdownTransportNoId.siblings('.btn-group').find('button.multiselect');
                    if ($button.length) {
                        $button.focus();
                        setTimeout(function () {
                            $button.click();
                        }, 50);
                    }
                }
                return;
            }

            if (!fromData.TEDate || fromData.TEDate.trim() === '' || !isValidDate(fromData.TEDate)) {
                $(commonName.TransportExpenseSaveBtn).prop('disabled', true);
                if (teDatePicker) {
                    teDatePicker.open();
                }
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
                    showToast("error", e.message || "An error occurred");
                },
                complete: function () {
                    resetFrom();
                    autoTransportAssignEntryId();
                    loadCategoryData();
                    DetailsTempDataTable();
                    initializeMainDataTable();
                }
            });
        });

        function loadCategoryData() {
            if (table) {
                table.ajax.reload(null, false);
            }
        }

        // Initialize main DataTable with better error handling
        function initializeMainDataTable() {
            // Check if table element exists
            if (!$('#TransportExpenseEntyTable').length) {               
                return;
            }

            try {
                table = $('#TransportExpenseEntyTable').DataTable({
                    destroy: true,
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
                            "data": "autoId",
                            width: "75px",
                            "render": function (data) {
                                return `<input type="checkbox" class="row-checkbox" value="${data || ''}" />`;
                            },
                            "orderable": false
                        },
                        {
                            "data": "teid",
                            "render": function (data) {
                                return `<button class="btn btn-sm btn-link btn-transport-expense-entry-edit" 
                        data-id="${data || ''}">${data || ''}</button>`;
                            }
                        },
                        { "data": "teDate", "defaultContent": "" },       
                        { "data": "vehicleNo", "defaultContent": "" },    
                        { "data": "fullName", "defaultContent": "" },            
                        { "data": "telephone", "defaultContent": "" },      
                        { "data": "companyName", "defaultContent": "" } 
                    ],
                    "paging": true,
                    "pagingType": "full_numbers",
                    "searching": true,
                    "ordering": true,
                    "responsive": true,
                    "autoWidth": true,
                    "lengthMenu": [[5, 10, 50, 100, -1], [5, 10, 50, 100, "All"]],
                    "pageLength": 10,
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
            } catch (error) {
                showToast("error", "Failed to initialize main table: " + error.message);
            }
        }

        // Edit functionality
        $(document).on('click', commonName.EditBrn, function () {
            let id = $(this).data('id');
            $.ajax({
                url: `${PopulatedDataForUpdateUrl}?id=${id}`,
                type: "GET",
                success: function (res) {
                    selectedIds = [];
                    selectedIds.push(res.result.autoId + '');
                    $(commonName.AutoId).val(res.result.autoId || '');
                    $(commonName.TransportNoId).val(res.result.tedid || '').multiselect('rebuild');
                    EditTransportMasterExpense(res.result.autoId);                    
                    if (res.result.teDate && teDatePicker) {
                        teDatePicker.setDate(res.result.teDate);
                    }

                    $(commonName.Active).prop('checked', res.result.active === true || res.result.active === "true");
                    $(commonName.CreateDate).text(res.result.showCreateDate || '');
                    $(commonName.UpdateDate).text(res.result.showModifyDate || '');
                },
                error: function (e) {
                },
                complete: function () {
                    DetailsTempDataTable();
                    initializeMainDataTable();
                }
            });
        });

        function ReloadDataBackTempToDetails() {
            $.ajax({
                url: ReloadDataBackTempToDetailsUrl,
                Type: "GET",
                success: function (res) {
                }, error: function (e) {
                }, complete: function () {
                    DetailsTempDataTable();
                    initializeMainDataTable();
                }

            });
        }

        // Checkbox selection handling
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
        });

        // Select all functionality
        $(document).on('change', commonName.SelectedAll, function () {
            const isChecked = $(this).is(':checked');
            $(commonName.RowCheckbox).prop('checked', isChecked).trigger('change');
        });

        // Delete functionality
        $(document).on('click', commonName.DeleteBtn, function () {
            $.ajax({
                url: deleteUrl,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(selectedIds),
                success: function (res) {
                    showToast(res.isSuccess ? "success" : "error", res.message);
                },
                error: function (e) {
                },
                complete: function () {
                    resetFrom();
                    autoTransportAssignEntryId();
                    loadCategoryData();
                    DetailsTempDataTable();
                    initializeMainDataTable();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            });
        });

        window.TransportExpenseModuleLoaded = true;

        // Initialize all functions
        var init = function () {
            stHeader();
            autoTransportAssignEntryId();

            setTimeout(function () {
                UserSelectEmpMultiselect();
            }, 100);

            setTimeout(function () {
                ExpenseHeadList();
            }, 100);

            setTimeout(function () {
                TransportNoList();
                GetAllTransportDetailsAllFun();
            }, 100);

            setTimeout(function () {
                initializeDatePicker();
            }, 100);

            setTimeout(function () {
                DetailsTempDataTable();
                initializeMainDataTable();
                ReloadDataBackTempToDetails();
            }, 200);
        };

        init();
    };
})(jQuery);