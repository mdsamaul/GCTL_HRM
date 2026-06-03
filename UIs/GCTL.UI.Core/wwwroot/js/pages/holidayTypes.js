
(function ($) {
    $.holidayTypes = function (options)
    {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#HolidayTypes-form",
            formContainer: ".js-HolidayTypes-form-container",
            gridSelector: "#HolidayTypes-grid",
            gridContainer: ".js-HolidayTypes-grid-container",
            editSelector: ".js-HolidayTypes-edit",
            saveSelector: ".js-HolidayTypes-save",
            selectAllSelector: "#HolidayTypes-check-all",
            deleteSelector: ".js-HolidayTypes-delete-confirm",
            deleteModal: "#HolidayTypes-delete-modal",
            finalDeleteSelector: ".js-HolidayTypes-delete",
            clearSelector: ".js-HolidayTypes-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-HolidayTypes-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-HolidayTypes-check-duplicateName",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () { }
        }, options);


        var baseControllerNameUrl = "/HolidayTypes";
        var testSave = baseControllerNameUrl + "/Setup";
        var nextCodeULR = baseControllerNameUrl + "/GenerateNextCodeHolidayTypes";
        var loadTableURL = baseControllerNameUrl + "/GetTableData";
        var deleteURL = baseControllerNameUrl + "/Delete";
        var getById = baseControllerNameUrl + "/Index";
        var duplicateCheckURL = baseControllerNameUrl + "/CheckAvailability";

        var selectedItems = [];

        $(() => {
            initialize();

            $("body").on('click', '#exportExcel', function ()
            {
              
                window.location.href = '/HolidayTypes/ExportToExcel';
            });
            
            // Save button click event
            $("body").on('click', settings.saveSelector, function ()
            {
                validation();
                $(settings.formSelector).submit();
            });

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
                        if (result.isDuplicate || result.noSavePermission || result.noUpdatePermission)
                        {
                            toastr.error(result.message);
                        } else if (result.isSuccess)
                        {
                            $(settings.gridContainer).html(result.html);
                            initialize();
                            ResetForm();
                            //
                            $(settings.lastCodeSelector).val(result.lastCode);
                           // alert(`LastCode: ${result.lastCode}`);
                            //
                            toastr.success(result.message);
                        } else {
                            $(settings.formSelector).html(result);
                            initialize();
                        }
                    },
                    error: function ()
                    {
                        toastr.error('Failed Insert.');
                    }
                });
            });

            // Availability check
            $("body").on("keyup", settings.availabilitySelector, function ()
            {
                var self = $(this);
                let code = $(".js-HolidayTypeCode-code").val();
                let name = self.val();

                $.ajax({
                    url: duplicateCheckURL,
                    method: "POST",
                    data: { code: code, name: name },
                    success: function (response)
                    {
                        console.log(response);
                        if (response.isSuccess)
                        {
                            toastr.error(response.message);
                        }
                    }
                });
            });





            // Edit leave type
            $("body").on('click', settings.editSelector, function () {
                var id = $(this).data('id');
                $.get(getById, { id: id }, function (result) {

                    $(settings.formSelector).html($(result).find(settings.formSelector).html());
                    $(settings.saveSelector).html('<i class="fas fa-edit"></i> Update');
                    $(settings.formSelector).attr('action', testSave);
                    $(settings.deleteSelector).data('id', id);
                }).fail(function () {
                    toastr.error('Failed Update.');
                });
            });



            
            // Select all checkboxes
            $("body").on('click', settings.selectAllSelector, function ()
            {
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
            $("body").on('click', settings.finalDeleteSelector, function (e)
            {
                e.preventDefault();
                var selectedIds = $(this).data('ids');

                $.ajax({
                    type: 'POST',
                    url: deleteURL,
                    contentType: 'application/json',
                    data: JSON.stringify(selectedIds),
                    success: function (result)
                    {
                        if (result.isSuccess)
                        {
                            initialize();
                            toastr.success(result.message);
                        } else
                        {
                            toastr.error(result.message);
                        }
                        $(settings.deleteModal).modal('hide');
                    },
                    error: function (xhr)
                    {
                        toastr.error('Failed Delete.');
                        $(settings.deleteModal).modal('hide');
                    }
                });
            });
        });




        // Initialization function
        function initialize()
        {
            loadTableData();
            ResetForm();
        }


        function GenerateNextCodeHolidayType()
        {
            $('#HolidayTypeCode').val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result) {
                    $('#HolidayTypeCode').val(result);
                },
                error: function () {
                    toastr.error('Failed Next Code.');
                }
            });
        }


        // Load leave types list
        function loadTableData()
        {
            $.ajax({
                type: 'GET',
                url: loadTableURL,
                success: function (data)
                {
                    $(settings.gridContainer).html(data);
                    dataTable();

                },
                error: function ()
                {
                    toastr.error('Failed Loading Data.');
                }
            });
        }

        // Data table initialization function
        function dataTable()
        {
            $(settings.gridSelector).DataTable({
                responsive: true,
                pageLength: 5,
                destroy: true,
                lengthMenu: [5, 10, 25, 50, 100],
            });
        }

       


        function ResetForm()
        {
            $(settings.formSelector)[0].reset();
            $('#Id').val('');
            $('#HRMDefHolidayTypeAutoID').val('');
            $('#HolidayType').val('').trigger('focus');
            $('#LdateModifyHide').hide();
            $(settings.saveSelector).html('<i class="fas fa-save"></i> Save');
            $('.text-danger').text('');
            $(settings.formSelector).attr('action', testSave);
            GenerateNextCodeHolidayType();

        }


        // Clear form on click
        $(document).on('click', settings.clearSelector, function ()
        {
            ResetForm();
        });



        // Validation function
        function validation()
        {
           
            var holidayType = $('#HolidayType').val();

            if (!holidayType)
            {
                toastr.info('Enter Holiday Type.');
                $('#HolidayType').trigger('focus')
                return false;
            }

            return true;
        }
    }
}(jQuery));
