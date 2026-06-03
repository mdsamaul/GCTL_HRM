
(function ($)
{
    $.leaveType = function (options)
    {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#leaveType-form",
            formContainer: ".js-leaveType-form-container",
            gridSelector: "#leaveType-grid",
            gridContainer: ".js-leaveType-grid-container",
            editSelector: ".js-leaveType-edit",
            saveSelector: ".js-leaveType-save",
            selectAllSelector: "#leaveType-check-all",
            deleteSelector: ".js-leaveType-delete-confirm",
            deleteModal: "#leaveType-delete-modal",
            finalDeleteSelector: ".js-leaveType-delete",
            clearSelector: ".js-leaveType-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-leaveType-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-leaveTyp-check-duplicateName",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () { }
        }, options);

      
        var baseControllerNameUrl = "/LeaveTypes";
        var testSave = baseControllerNameUrl + "/Setup";
        var nextCodeULR = baseControllerNameUrl + "/GenerateNextLeaveTypeCode";
        var loadTableURL = baseControllerNameUrl + "/GeLeaveTypesList"; 
        var deleteURL = baseControllerNameUrl + "/Delete";
        var getById = baseControllerNameUrl + "/Index";
        var duplicateCheckURL = baseControllerNameUrl + "/CheckAvailability";
        var selectedItems = [];

        $(() =>
        {
            initialize();
            $("body").on('click', '#exportExcel', function () {
               
                window.location.href = '/LeaveTypes/ExportToExcel';
            });
            // Save button click event
            $("body").on('click', settings.saveSelector, function ()
            {
                validation();
                $(settings.formSelector).submit();
            });

            //

            $("body").on('click', '#exportExcel', function () {
                $("#loadingIndicator").show();

                $.ajax({
                    url: '/LeaveTypes/ExportToExcel',
                    method: 'GET',
                    success: function (response) {
                        window.location.href = '/LeaveTypes/ExportToExcel';
                    },
                    error: function () {
                        toastr.error('Error to Export Excel')
                    },
                    complete: function () {
                        $('#loadingIndicator').hide();
                    }
                });
            });
            //

            $("body").on('click', '#exportPdf', function () {



                $("#loadingIndicator").show();
                $.ajax({
                    url: '/LeaveTypes/ExportToPdf',
                    type: 'Get',

                    success: function (response) {
                        window.location.href = '/LeaveTypes/ExportToPdf'
                    },
                    error: function () {
                        toastr.error('An error occurred while exporting the PDF.');
                    },
                    complete: function () {
                        $('#loadingIndicator').hide();
                    }

                });
            });

            //

            // Form submission event
            $("body").on('submit', settings.formSelector, function (e)
            {
                e.preventDefault();
                var form = $(this)[0];
                var formData = new FormData(form);
                var actionUrl = $(this).attr('action');

                $.ajax({
                    type: 'POST',
                    url: actionUrl,
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        if (result.isDuplicate) {
                            toastr.warning(result.message);
                        } else if (result.isSuccess) {
                            $(settings.gridContainer).html(result.html);
                            initialize();
                            toastr.success(result.message);
                        } else {
                            $(settings.formSelector).html(result);
                        }
                    },
                    error: function () {
                        toastr.error('An error occurred while saving leave type data.');
                    }
                });
            });

            // Availability check
            //$("body").on("keyup", settings.availabilitySelector, function ()
            //{
            //    var self = $(this);
            //    let code = $(".js-leaveType-code").val();
            //    let name = self.val();

            //    $.ajax({
            //        url: duplicateCheckURL,
            //        method: "POST",
            //        data: { code: code, name: name },
            //        success: function (response)
            //        {
            //            console.log(response);
            //            if (response.isSuccess)
            //            {
            //                toastr.error(response.message);
            //            }
            //        }
            //    });
            //});

            // Edit leave type
            $("body").on('click', settings.editSelector, function ()
            {
                var id = $(this).data('id');
                $.get(getById, { id: id }, function (result)
                {
                    $(settings.formSelector).html($(result).find(settings.formSelector).html());
                    WEFDatePicker();
                    select2DD();
                    $(settings.saveSelector).html('<i class="fas fa-edit"></i> Update');
                    $(settings.formSelector).attr('action', testSave);
                    // Store the id for delete action
                    $(settings.deleteSelector).data('id', id);
                }).fail(function () {
                    toastr.error('Error fetching leave type details. Please try again.');
                });
            });

            // Select all checkboxes
            $("body").on('click', settings.selectAllSelector, function () {
                $('.checkBox').prop('checked', $(this).prop('checked'));
            });

            $("body").on('click', settings.deleteSelector, function (e) {
                e.preventDefault();

                var selectedIds = [];

                // Check if was clicked from the edit button
                if ($(this).data('id')) {
                    // If the delete button has a 'data-id' from edit,
                    var editCode = $(this).data('id');
                    selectedIds.push(editCode); // Add the single ID to the array
                } else {
                    // Otherwise, collect all selected checkboxes' IDs for bulk deletion
                    $('.checkBox:checked').each(function () {
                        selectedIds.push($(this).val());
                    });
                }

                if (selectedIds.length === 0) {
                    toastr.error('Please select at least one leave type to delete.');
                    return;
                }

                $(settings.deleteModal + ' ' + settings.finalDeleteSelector).data('ids', selectedIds);
                $(settings.deleteModal).modal('show');
            });

            // Final delete action
            $("body").on('click', settings.finalDeleteSelector, function (e) {
                e.preventDefault();
                var selectedIds = $(this).data('ids');

                $.ajax({
                    type: 'POST',
                    url: deleteURL,
                    contentType: 'application/json',
                    data: JSON.stringify(selectedIds),
                    success: function (result) {
                        if (result.isSuccess) {
                            initialize();
                            toastr.success(result.message); 
                        } else {
                            toastr.error(result.message); 
                        }
                        $(settings.deleteModal).modal('hide');
                    },
                    error: function (xhr) {
                        toastr.error('An error occurred while deleting the selected leave types.');
                        $(settings.deleteModal).modal('hide');
                    }
                });
            });
        });


       

        // Initialization function
        function initialize()
        {
            WEFDatePicker();
            GenerateNextLeaveTypeCode();
            loadLeaveTypesList();
            LeavetypeResetForm();
            select2DD();
        }

       
        // Load leave types list
        function loadLeaveTypesList()
        {
            $.ajax({
                type: 'GET',
                url: loadTableURL,
                success: function (data)
                {
                    $(settings.gridContainer).html(data);
                    dataTableLeaveType();
                   
                },
                error: function ()
                {
                    toastr.error('An error occurred while loading the leave type list.');
                }
            });
        }

        // Data table initialization function
        function dataTableLeaveType()
        {

            $('#leaveType-grid').DataTable({
                responsive: true,
                pageLength: 10,
                destroy: true,
                lengthMenu: [10, 25, 50, 100],
            });


        }

      

       
        
        // WEF Date Picker
        function WEFDatePicker()
        {
            $("#WefDate").datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+10",
                beforeShow: function (input, inst) {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onClose: function (dateText, inst) {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onSelect: function (dateText) {
                    var formattedDate = $.datepicker.formatDate('mm/dd/yy', $(this).datepicker('getDate'));
                    $('#WefHidden').val(formattedDate);
                    $("#WefDate").valid();
                }
            });

            var initialDate = $("#WefDate").val();
            if (initialDate) {
                var formattedInitialDate = $.datepicker.formatDate('mm/dd/yy', $("#WefDate").datepicker('getDate'));
                $('#WefHidden').val(formattedInitialDate);
            }

            $(".datepicker").trigger("input");
        }

        // Generate next leave type code function
        function GenerateNextLeaveTypeCode()
        {
            $("#LeaveTypeCode").val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result)
                {
                    $('#LeaveTypeCode').val(result);
                },
                error: function ()
                {
                    toastr.error('An error occurred while generating the next code.');
                }
            });
        }

       

     

        // Leave type reset form function
        function LeavetypeResetForm()
        {
            $(settings.formSelector)[0].reset();
            $('#Id').val('');
            $('#LeaveTypeCode').val('');
            $('#Name').val('').trigger('focus');
            $('#ShortName').val('');
            $('#RulePolicy').val('');
            $('#NoOfDay').val('');
            $('#Ymwd').val('Year').trigger('change');
            $('#WefDate').val('');
            $('#For').val('');
            $('#LdateModifyHide').hide();
            $(settings.saveSelector).html('<i class="fas fa-save"></i> Save');
            $('.text-danger').text('');
            $(settings.formSelector).attr('action', testSave);
            GenerateNextLeaveTypeCode();
        }

        // Clear form on click
        $(document).on('click', settings.clearSelector, function ()
        {
            LeavetypeResetForm();
        });

        function select2DD() {

            $('#selectpickerLeaveTYpe').select2({
                language: {
                    noResults: function () {

                    }
                },
                escapeMarkup: function (markup)
                {
                    return markup;
                }
            });

        }
       
        // Validation function
        function validation()
        {
            var name = $("input[name='Name']").val();
            var wef = $("#WefDate").val();

            if (!name)
            {
                toastr.info('Leave type name is required');
                $('#Name').trigger('focus');
                return false;
            }

            if (!wef)
            {
                toastr.info('W.E.F is required');
                $("#WefDate").trigger('focus');
                return false;
            }
        }
    }
}(jQuery));