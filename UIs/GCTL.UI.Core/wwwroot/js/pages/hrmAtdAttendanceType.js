
(function ($) {
    $.hrmAtdAttendanceType = function (options)
    {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#HRMATDAttendanceType-form",
            formContainer: ".js-HRMATDAttendanceType-form-container",
            gridSelector: "#HRMATDAttendanceType-grid",
            gridContainer: ".js-HRMATDAttendanceType-grid-container",
            editSelector: ".js-HRMATDAttendanceType-edit",
            saveSelector: ".js-HRMATDAttendanceType-save",
            selectAllSelector: "#HRMATDAttendanceType-check-all",
            deleteSelector: ".js-HRMATDAttendanceType-delete-confirm",
            deleteModal: "#HRMATDAttendanceType-delete-modal",
            finalDeleteSelector: ".js-HRMATDAttendanceType-delete",
            clearSelector: ".js-HRMATDAttendanceType-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-HRMATDAttendanceType-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: "#AttendanceTypeName",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () { }
        }, options);


        var baseControllerNameUrl = "/HRMATDAttendanceType";
        var testSave = baseControllerNameUrl + "/Setup";
        var nextCodeULR = baseControllerNameUrl + "/GenerateNextCode";
        var loadTableURL = baseControllerNameUrl + "/GetTableData";
        var deleteURL = baseControllerNameUrl + "/Delete";
        var getById = baseControllerNameUrl + "/Index";
        var duplicateCheckURL = baseControllerNameUrl + "/CheckAvailability";

        var selectedItems = [];

        $(() => {




            initialize();



            // Save button click event
            $("body").on('click', settings.saveSelector, function () {
                validation();
                $(settings.formSelector).submit();
            });

            // Form submission event
            $("body").on('submit', settings.formSelector, function (e) {
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
                        if (result.noSavePermission || result.noUpdatePermission || result.isDuplicate) {

                            toastr.error(result.message, 'Error');
                        }
                        else if (result.isSuccess) {
                            $(settings.gridContainer).html(result.html);
                            initialize();

                            toastr.success(result.message,'Success');
                        } else {
                            $(settings.formSelector).html(result);
                            initialize();
                        }
                    },
                    error: function () {
                        toastr.error('Failed Insert.','Error');
                    }
                });
            });

            // Availability check
            $("body").on("keyup", settings.availabilitySelector, function ()
            {
                var self = $(this);
                let typeCode = $(".js-AttendanceTypeCode-code").val();
                let name = self.val();

                $.ajax({
                    url: duplicateCheckURL,
                    method: "POST",
                    data: { typeCode: typeCode, name: name },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess)
                        {
                            toastr.error(response.message,'Error');
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
                    toastr.error('Failed Update','Error');
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
                        if (result.refError) {
                            toastr.warning(result.message, 'Warning');
                        } else if (result.isSuccess) {

                            initialize();
                            toastr.success(result.message,'Success');
                        } else {

                            toastr.error(result.message,'Error');
                        }
                        $(settings.deleteModal).modal('hide');
                    },
                    error: function (xhr) {
                        toastr.error('Failed Delete .','Error');
                        $(settings.deleteModal).modal('hide');
                    }
                });
            });

            //



        });




        // Initialization function
        function initialize() {

            GenerateNextCode();
            loadTableData();
            ResetForm();

        }


       


        // Load leave types list
        function loadTableData() {
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
                    toastr.error('Failed Loading Data.','Error');
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




        function GenerateNextCode()
        {
            $("#AttendanceTypeCode").val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result)
                {

                    $('#AttendanceTypeCode').val(result);
                },
                error: function ()
                {
                    toastr.error('Failed Next Code.','Error');
                }
            });
        }
        //


        //


        function ResetForm()
        {

            $(settings.formSelector)[0].reset();
            $('#Id').val('');
            $('#AutoId').val('');
            $('#AttendanceTypeCode').val('');
            $('#AttendanceTypeName').val('').trigger('focus');
            $('#ShortName').val('');
            $('#LdateModifyHide').hide();
            $(settings.saveSelector).html('<i class="fas fa-save"></i> Save');
            $('.text-danger').text('');
            $(settings.formSelector).attr('action', testSave);
            GenerateNextCode();

        }


        // Clear form on click
        $(document).on('click', settings.clearSelector, function ()
        {
            ResetForm();
        });



        // Validation function
        function validation()
        {

            var attendanceType = $('#AttendanceTypeName').val();

            if (!attendanceType)
            {
                toastr.info('Enter Attendance Type.');
                $('#AttendanceTypeName').trigger('focus')
                return false;
            }

            return true;
        }



    }
}(jQuery));
















