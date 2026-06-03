(function ($) {
    $.HRMTransportAssignEntryJs = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            TransportTypeId: "#TransportTypeId",
            TransportNoId: "#TransportNoId",
            HelperId: "#HelperId",
            DriverSelectEmpId: "#DriverSelectEmpId",
            EffectiveDate: "#EffectiveDate",
            TransportUser: "#UserSelectEmpId",
            Active: "#Active",
            TransportAssignEntryId: "#TAID",
            AutoId: "#AutoId",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBrn: ".btn-transport-entry-edit",
            VehicleTypeSaveBtn: ".js-transport-entry-assign-save",
            DeleteBtn: "#js-transport-entry-assign-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-transport-entry-assign-clear",
            DEmpName:"#DEmpName",
            DEmpDesignation:"#DEmpDesignation",
            DEmpDepartment:"#DEmpDepartment",
            DEmpPhone:"#DEmpPhone",
        }, options);

        var loadVehicleTypeDataUrl = commonName.baseUrl + "/LoadData";
        var autoIdUrl = commonName.baseUrl + "/AutoId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteTransport";
        var alreadyExistUrl = commonName.baseUrl + "/alreadyExist";
        var LoadEmpDetailsUrl = commonName.baseUrl + "/GetEmpDetailsId"; 
        var transportTypeUrl = commonName.baseUrl + "/transportTypeGetByTransportNo"; 
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

      
        $(document).ready(function () {
            setTimeout(function () {
                $('.multiselect-clear-filter i').removeClass('glyphicon glyphicon-remove-circle')
                    .addClass('fa fa-times-circle');
            }, 200);
        });

        let UserEmplist = [];
        //function DriverSelectEmpId() {
        //    var $dropdown = $('#DriverSelectEmpId');
        //    if ($dropdown.length && typeof $.fn.multiselect !== 'undefined') {
        //        if (!$dropdown.data('multiselect')) {
        //            $dropdown.multiselect({
        //                enableFiltering: true,
        //                enableCaseInsensitiveFiltering: true,
        //                filterPlaceholder: 'Search for an employee...',
        //                buttonWidth: '100%',
        //                maxHeight: 300,
        //                nonSelectedText: '--Select Employee--',
        //                buttonClass: 'btn btn-outline-secondary text-start',
        //                onChange: function (option, checked) {
        //                    let selectedValue = $dropdown.val();
        //                }
        //            });
        //        }
        //    }
        //}
        //function TransportNoList() {
        //    var $dropdown = $(commonName.TransportNoId);
        //    if ($dropdown.length && typeof $.fn.multiselect !== 'undefined') {
        //        if (!$dropdown.data('multiselect')) {
        //            $dropdown.multiselect({
        //                enableFiltering: true,
        //                enableCaseInsensitiveFiltering: true,
        //                filterPlaceholder: 'Search for an employee...',
        //                buttonWidth: '100%',
        //                maxHeight: 300,
        //                nonSelectedText: '--Select Employee--',
        //                buttonClass: 'btn btn-outline-secondary text-start',
        //                onChange: function (option, checked) {
        //                    let selectedValue = $dropdown.val();

        //                }
        //            });
        //        }
        //    }
        //}

        //function DriverHelperSelectEmpMultiselect() {
        //    var $dropdown = $(commonName.HelperId);
        //    if ($dropdown.length && typeof $.fn.multiselect !== 'undefined') {
        //        if (!$dropdown.data('multiselect')) {
        //            $dropdown.multiselect({
        //                enableFiltering: true,
        //                enableCaseInsensitiveFiltering: true,
        //                filterPlaceholder: 'Search for an employee...',
        //                buttonWidth: '100%',
        //                maxHeight: 300,
        //                nonSelectedText: '--Select Employee--',
        //                buttonClass: 'btn btn-outline-secondary text-start',
        //                onChange: function (option, checked) {
        //                    let selectedValue = $dropdown.val();

        //                }
        //            });
        //        }
        //    }
        //}


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





        const effectiveDatePicker = flatpickr("input[name='EffectiveDate']", {
            altInput: true,
            altFormat: "d/m/Y",
            dateFormat: "Y-m-d",
            allowInput: true,
            defaultDate: "today"
        });

        function isValidDate(dateStr) {
            const date = Date.parse(dateStr);
            return !isNaN(date);
        }

        $(commonName.DriverSelectEmpId).on('change', function () {
          
            var selectedValue = $(this).val();
            $.ajax({
                url: LoadEmpDetailsUrl,
                type: "POST",
                contentType:'application/json',
                data: JSON.stringify(selectedValue ),
                success: function (res) {                   
                        $(commonName.DEmpName).text(res.data?.empName);
                        $(commonName.DEmpDepartment).text(res.data?.department);
                        $(commonName.DEmpDesignation).text(res.data?.designation);
                        $(commonName.DEmpPhone).text(res.data?.phone);                  
                   
                }, error: function (e) {
                }
            })
        })
        autoTransportAssignEntryId = function () {
            $.ajax({
                url: autoIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.TransportAssignEntryId).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.TransportAssignEntryId).val('');
            $(commonName.Active).prop('checked', false);
          
            $(commonName.TransportNoId).val('').trigger('change');
            $(commonName.HelperId).val('').trigger('change');
            $(commonName.TransportTypeId).val('').trigger('change');

            UserEmplist = [];
            
            if ($('#UserSelectEmpId').length && $('#UserSelectEmpId').data('multiselect')) {
                $('#UserSelectEmpId').multiselect('deselectAll', false);
                $('#UserSelectEmpId').multiselect('refresh');
            }

            // DriverSelectEmpId reset  
            $('#DriverSelectEmpId').val(null).trigger('change');
            $(commonName.TransportNoId).val(null).trigger('change');
            $(commonName.HelperId).val(null).trigger('change');

            $('#TransportTypeId')
                .val('');     

           
                if ($(commonName.TransportTypeId).data('multiselect')) {
                    $(commonName.TransportTypeId).multiselect('rebuild');
                    //$(commonName.TransportTypeId).change();
                    $(commonName.TransportTypeId).multiselect('disable');
                }
            

           

            // Clear employee details
            $(commonName.DEmpName).text("");
            $(commonName.DEmpPhone).text("");
            $(commonName.DEmpDesignation).text("");
            $(commonName.DEmpDepartment).text("");
            $(commonName.CreateDate).text("");
            $(commonName.UpdateDate).text("");

            // Reset date picker
            if (typeof effectiveDatePicker !== 'undefined') {
                effectiveDatePicker.setDate("today", true);
            }

            autoTransportAssignEntryId();
        }
        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
        })
        // get data from input
        getFromData = function () {
            var fromData = {
                AutoId: $(commonName.AutoId).val(),
                TAID: $(commonName.TransportAssignEntryId).val(),
                EmployeeID: $(commonName.DriverSelectEmpId).val(),
                TransportNoId: $(commonName.TransportNoId).val(),
                HelperId: $(commonName.HelperId).val(),
                TransportTypeId: $(commonName.TransportTypeId).val(),
                EffectiveDate: $(commonName.EffectiveDate).val(),
                Active: $(commonName.Active).prop("checked") ? "true" : "false",             
                TransportUser: $('#UserSelectEmpId').val() || []
            };
            return fromData;
        }
        //exists 
        $([commonName.DriverSelectEmpId, commonName.TransportUser, commonName.TransportNoId].join(',')).on('change', function () {
            $(commonName.VehicleTypeSaveBtn).prop('disabled', false);

        });


        
        //create and edit
        // Save Button Click
        $(document).on('click', commonName.VehicleTypeSaveBtn, function () {
            var fromData = getFromData();
          
            if (!fromData.EmployeeID || fromData.EmployeeID.trim() === '') {
                $(commonName.VehicleTypeSaveBtn).prop('disabled', true);

                var $dropdown = $('#DriverSelectEmpId');

                if ($dropdown.length && $dropdown.data('multiselect')) {
                    var $button = $dropdown.siblings('.btn-group').find('button.multiselect');

                    if ($button.length) {
                        $button.focus();
                        setTimeout(function () {
                            $button.click();
                        }, 50);
                    }
                }
                return;
            }



            if (fromData.TransportNoId == null || fromData.TransportNoId.trim() === '') {               
                $(commonName.VehicleTypeSaveBtn).prop('disabled', true);
              
                var $dropdown = $(commonName.TransportNoId);

                if ($dropdown.length && $dropdown.data('multiselect')) {
                    var $button = $dropdown.siblings('.btn-group').find('button.multiselect');

                    if ($button.length) {
                        $button.focus();
                        setTimeout(function () {
                            $button.click();
                        }, 50);
                    }
                }

                return;
            }
            if (fromData.TransportUser == null || fromData.TransportUser.length === 0) {               
                $(commonName.VehicleTypeSaveBtn).prop('disabled', true);

                var $dropdown = $("#UserSelectEmpId");

                if ($dropdown.length && $dropdown.data('multiselect')) {
                    var $button = $dropdown.siblings('.btn-group').find('button.multiselect');

                    if ($button.length) {
                        $button.focus();
                        setTimeout(function () {
                            $button.click();
                        }, 50);
                    }
                } 
                return;
            }
            if (!fromData.EffectiveDate || fromData.EffectiveDate.trim() === '' || !isValidDate(fromData.EffectiveDate)) {
                $(commonName.VehicleTypeSaveBtn).prop('disabled', true);
                effectiveDatePicker.open();
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
                    autoTransportAssignEntryId();
                    loadCategoryData();
                }
            });
        });

        // Reload DataTable Function
        function loadCategoryData() {
            table.ajax.reload(null, false);
        }

        var table = $('#TransportAssignEntyTable').DataTable({
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
                    "data": "autoId",
                    "render": function (data) {
                        return `<input type="checkbox" class="row-checkbox" value=${data} />`;
                    },
                    "orderable": false
                },
                {
                    "data": "taid",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link btn-transport-entry-edit" data-id=${data}>${data}</button>`;
                    }
                },
                { "data": "showTransportNoId" },
                { "data": "showTransportTypeId" },
                { "data": "showEffectiveDate" },
                { "data": "active" },
                { "data": "showEmployeeID" },
                { "data": "entryUserEmployeeID" },
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
        let selectedIds = [];
        //edit
        //$(document).on('click', commonName.EditBrn, function () {
        //    let id = $(this).data('id');

        //    $.ajax({
        //        url: `${PopulatedDataForUpdateUrl}?id=${id}`,
        //        type: "GET",
        //        success: function (res) {
        //            selectedIds = [];
        //            selectedIds.push(res.result.autoId + '');
        //            $(commonName.AutoId).val(res.result.autoId);
        //            $(commonName.TransportAssignEntryId).val(res.result.taid);
        //            $(commonName.TransportTypeId).val(res.result.transportTypeId).trigger('change');

        //            $(commonName.DriverSelectEmpId).val(res.result.employeeID);
        //            if ($(commonName.DriverSelectEmpId).data('multiselect')) {
        //                $(commonName.DriverSelectEmpId).multiselect('rebuild');
        //                $(commonName.DriverSelectEmpId).change();
        //            }



        //            $(commonName.TransportNoId).val(res.result.transportNoId);
        //            if ($(commonName.TransportNoId).data('multiselect')) {
        //                $(commonName.TransportNoId).multiselect('rebuild');
        //                $(commonName.TransportNoId).change();
        //            }
        //            $(commonName.TransportUser).val(res.result.transportUser);
        //            if ($(commonName.TransportUser).data('multiselect')) {
        //                $(commonName.TransportUser).multiselect('rebuild');
        //                $(commonName.TransportUser).change();
        //            }
        //            $(commonName.HelperId).val(res.result.helperId).multiselect('rebuild');
        //            if (res.result.effectiveDate) {
        //                effectiveDatePicker.setDate(res.result.effectiveDate);
        //            }
        //            $(commonName.Active).prop('checked', res.result.active === true || res.result.active === "true");
        //            $(commonName.CreateDate).text(res.result.showCreateDate);
        //            $(commonName.UpdateDate).text(res.result.showModifyDate);
        //        },
        //        error: function (e) {
        //        }, complete: function () {
        //        }
        //    });
        //});

        $(document).on('click', commonName.EditBrn, function () {
            let id = $(this).data('id');

            $.ajax({
                url: `${PopulatedDataForUpdateUrl}?id=${id}`,
                type: "GET",
                success: function (res) {

                    selectedIds = [];
                    selectedIds.push(res.result.autoId + '');
                    $(commonName.AutoId).val(res.result.autoId);
                    $(commonName.TransportAssignEntryId).val(res.result.taid);

                    // ✅ Transport Type (Bootstrap Multiselect)
                    $(commonName.TransportTypeId).val(res.result.transportTypeId);
                    if ($(commonName.TransportTypeId).data('multiselect')) {
                        $(commonName.TransportTypeId).multiselect('rebuild');
                        $(commonName.TransportTypeId).change();
                    }

                    // ✅ Driver (Select2)
                    $(commonName.DriverSelectEmpId)
                        .val(res.result.employeeID)
                        .trigger('change');

                    // ✅ Transport No (Select2)
                    $(commonName.TransportNoId)
                        .val(res.result.transportNoId)
                        .trigger('change');

                    // ✅ Transport User (Bootstrap Multiselect)
                    $(commonName.TransportUser).val(res.result.transportUser);
                    if ($(commonName.TransportUser).data('multiselect')) {
                        $(commonName.TransportUser).multiselect('rebuild');
                        $(commonName.TransportUser).change();
                    }

                    // ✅ Helper (Select2)
                    $(commonName.HelperId)
                        .val(res.result.helperId)
                        .trigger('change');

                    if (res.result.effectiveDate) {
                        effectiveDatePicker.setDate(res.result.effectiveDate);
                    }

                    $(commonName.Active).prop('checked', res.result.active === true || res.result.active === "true");
                    $(commonName.CreateDate).text(res.result.showCreateDate);
                    $(commonName.UpdateDate).text(res.result.showModifyDate);
                },
                error: function (e) {
                },
                complete: function () {
                }
            });
        });

        $(document).on('change', commonName.TransportNoId, function () {
            var transportNoId = $(this).val();
            $.ajax({
                url: transportTypeUrl,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify( transportNoId),
                success: function (res) {
                    if (res.data.length >0) {
                        $(commonName.TransportTypeId).val(res.data[0].vehicleTypeId);
                        if ($(commonName.TransportTypeId).data('multiselect')) {
                            $(commonName.TransportTypeId).multiselect('rebuild');
                            //$(commonName.TransportTypeId).change();
                            $(commonName.TransportTypeId).multiselect('disable');
                        }
                    }                   
                }
            });
        })
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
                    autoTransportAssignEntryId();
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
            autoTransportAssignEntryId();
            //setTimeout(function () {
            //    UserSelectEmpMultiselect();
            //    DriverHelperSelectEmpMultiselect();
            //}, 100);
            //setTimeout(function () {
            //    DriverSelectEmpId();
            //}, 100);

            //setTimeout(function () {
            //    TransportNoList();
            //}, 100);
           
                initializeMultiselects('.gc-multiselect');
                initInlineSearchSelect2('.gc-select2');
          
            table;
        };
        init();

    };
})(jQuery);
