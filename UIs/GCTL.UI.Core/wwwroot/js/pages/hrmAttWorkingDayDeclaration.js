

(function ($) {
    $.hrmAttWorkingDayDeclaration = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#HRMAttWorkingDayDeclaration-form",
            formContainer: ".js-HRMAttWorkingDayDeclaration-form-container",
            gridSelector: "#HRMAttWorkingDayDeclaration-grid",
            gridContainer: ".js-HRMAttWorkingDayDeclaration-grid-container",
            editSelector: ".js-HRMAttWorkingDayDeclaration-edit",
            saveSelector: ".js-HRMAttWorkingDayDeclaration-save",
            selectAllSelector: "#HRMAttWorkingDayDeclaration-check-all",
            deleteSelector: ".js-HRMAttWorkingDayDeclaration-delete-confirm",
            deleteModal: "#HRMAttWorkingDayDeclaration-delete-modal",
            finalDeleteSelector: ".js-HRMAttWorkingDayDeclaration-delete",
            clearSelector: ".js-HRMAttWorkingDayDeclaration-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-HRMAttWorkingDayDeclaration-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-HRMAttWorkingDayDeclaration-check-duplicateName",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () { }
        }, options);


        var baseControllerNameUrl = "/HRMAttWorkingDayDeclaration";
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
                        if (result.noSavePermission || result.noUpdatePermission || result.isDuplicate)
                        {

                            toastr.error(result.message, 'Error');
                        }
                        else if (result.isSuccess)
                        {
                            $(settings.gridContainer).html(result.html);
                            initialize();

                            toastr.success(result.message,'Success');
                        } else {
                            $(settings.formSelector).html(result);
                            initialize();
                        }
                    },
                    error: function () {
                        toastr.error('Failed Indsert.','Error');
                    }
                });
            });

            // Availability check
            $("body").on("keyup", settings.availabilitySelector, function ()
            {
                var self = $(this);
                let code = $(".js-ShiftCode-code").val();
                let name = self.val();

                $.ajax({
                    url: duplicateCheckURL,
                    method: "POST",
                    data: { code: code, name: name },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.error(response.message,'Error');
                        }
                    }
                });
            });

            //


            //Orginall Code 


            // Edit leave type
            $("body").on('click', settings.editSelector, function () {
                var id = $(this).data('id');
                $.get(getById, { id: id }, function (result) {

                    $(settings.formSelector).html($(result).find(settings.formSelector).html());
                    WorkingDayDatePicker();
                    $(settings.saveSelector).html('<i class="fas fa-edit"></i> Update');
                    $(settings.formSelector).attr('action', testSave);
                    $(settings.deleteSelector).data('id', id);
                }).fail(function () {
                    toastr.error('Failed Update.','Error');
                });
            });

            //

            $("body").on("keyup change", "#WorkingDayDateHidden, .js-WorkingDayCode-code", function ()
            {

                let typeCode = $(".js-WorkingDayCode-code").val();
                let workingDayDate = $("#WorkingDayDateHidden").val();

                console.log("Data for Duplicate Check - Code:", "Code:", typeCode, "workingDayDate:", workingDayDate);

                $.ajax({
                    url: duplicateCheckURL,
                    method: "POST",
                    data: { typeCode: typeCode, workingDayDate: workingDayDate },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.error(response.message,'Error');
                        }
                    }
                });

            });

            //



            //
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

            WorkingDayDatePicker();
            GenerateNextCode();
            loadTableData();
            ResetForm();


        }


        // Load leave types list
        function loadTableData() {
            $.ajax({
                type: 'GET',
                url: loadTableURL,
                success: function (data) {
                    $(settings.gridContainer).html(data);
                    dataTable();

                },
                error: function ()
                {
                    toastr.error('Failed Load Data.','Error');
                }
            });
        }

        // Data table initialization function
        function dataTable() {
            $(settings.gridSelector).DataTable({
                responsive: true,
                pageLength: 5,
                destroy: true,
                lengthMenu: [5, 10, 25, 50, 100],
            });
        }


        function WorkingDayDatePicker() {


            $("#WorkingDayDate").datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+10",
                beforeShow: function (input, inst)
                {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onClose: function (dateText, inst) {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onSelect: function (dateText) {
                    var formattedDate = $.datepicker.formatDate('mm/dd/yy', $(this).datepicker('getDate'));
                    $('#WorkingDayDateHidden').val(formattedDate).trigger('change');
                    $("#WorkingDayDate").valid();
                }
            });

            var initialDate = $("#WorkingDayDate").val();
            if (initialDate) {
                var formattedInitialDate = $.datepicker.formatDate('mm/dd/yy', $("#WorkingDayDate").datepicker('getDate'));
                $('#WorkingDayDateHidden').val(formattedInitialDate);
            }

            $(".datepicker").trigger("input");


        }








        function GenerateNextCode()
        {
            $("#WorkingDayCode").val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result)
                {

                    $('#WorkingDayCode').val(result);
                },
                error: function ()
                {
                    toastr.error('Failed Next Code.','Error');
                }
            });
        }






        function ResetForm()
        {

            $(settings.formSelector)[0].reset();
            $('#Id').val('');
            $('#AutoId').val('');
            $('#WorkingDayCode').val('');
            $('#Remarks').val('');
            $('#WorkingDayDate').val('');
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
           
            var wefdate = $('#WorkingDayDate').val();
            if (!wefdate)
            {
                toastr.info('Select Working Day Date.');
                $("#WorkingDayDate").trigger('focus');
                return false;
            }

            return true;
        }



    }
}(jQuery));



