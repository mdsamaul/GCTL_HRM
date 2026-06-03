(function ($) {
    $.relationships = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#Relationships-form",
            formContainer: ".js-Relationships-form-container",
            gridSelector: "#Relationships-grid",
            gridContainer: ".js-Relationships-grid-container",
            editSelector: ".js-Relationships-edit",
            saveSelector: ".js-Relationships-save",
            selectAllSelector: "#Relationships-check-all",
            deleteSelector: ".js-Relationships-delete-confirm",
            deleteModal: "#Relationships-delete-modal",
            finalDeleteSelector: ".js-Relationships-delete",
            clearSelector: ".js-Relationships-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-Relationships-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-Relationships-check-duplicateName",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () { }
        }, options);


        var baseControllerNameUrl = "/Relationships";
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
                            $(settings.lastCodeSelector).val(result.lastCode);
                           // alert(result.lastCode);
                            initialize();

                            toastr.success(result.message, 'Success');
                        } else {
                            $(settings.formSelector).html(result);
                            initialize();
                        }
                    },
                    error: function () {
                        toastr.error('An error occurred while saving Relationship data.');
                    }
                });
            });



            // Availability check
            $("body").on("keyup", settings.availabilitySelector, function () {
                var self = $(this);
                let code = $(".js-RelationshipCode-code").val();
                let name = self.val();

                $.ajax({
                    url: duplicateCheckURL,
                    method: "POST",
                    data: { code: code, name: name },
                    success: function (response) {
                        console.log(response);
                        if (response.isSuccess) {
                            toastr.error(response.message);
                        }
                    }
                });
            });

            //


            // Edit leave type
            $("body").on('click', settings.editSelector, function () {
                var id = $(this).data('id');
                $.get(getById, { id: id }, function (result) {

                    $(settings.formSelector).html($(result).find(settings.formSelector).html());
                    $(settings.saveSelector).html('<i class="fas fa-edit"></i> Update');
                    $(settings.formSelector).attr('action', testSave);
                    $(settings.deleteSelector).data('id', id);
                }).fail(function () {
                    toastr.error('Error Job RelationShip details. Please try again.');
                });
            });

            //
            // Select all checkboxes
            $("body").on('click', settings.selectAllSelector, function () {
                $('.checkBox').prop('checked', $(this).prop('checked'));
            });

            // Delete confirmation
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
                    toastr.error('Please Relationship to delete.');
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
                    //contentType: 'application/json',
                    //data: JSON.stringify(selectedIds),
                    data: { ids: selectedIds },
                    success: function (result) {
                        if (result.isSuccess) {

                            loadTableData();
                            initialize();
                            toastr.success(result.message);
                        } else {

                            toastr.error(result.message);
                        }
                        $(settings.deleteModal).modal('hide');
                    },
                    error: function (xhr) {
                        toastr.error('An error Relationship .');
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
            -
                $.ajax({
                    type: 'GET',
                    url: loadTableURL,
                    success: function (data) {
                        $(settings.gridContainer).html(data);
                        dataTable();

                    },
                    error: function () {
                        toastr.error('An error Job Title list.');
                    }
                });
        }

        // Data table initialization function
        function dataTable() {
            $(settings.gridSelector).DataTable({
                responsive: true,
                pageLength: 10,
                destroy: true,
                lengthMenu: [5, 10, 25, 50, 100],
            });
        }

        function GenerateNextCode() {
            $(".js-RelationshipCode-code").val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result) {

                    $('.js-RelationshipCode-code').val(result);
                },
                error: function () {
                    toastr.error('An error occurred while generating the next code.');
                }
            });
        }

        function ResetForm() {

            $(settings.formSelector)[0].reset();
            $('#RelationshipCode').val('');
            $('#AutoId').val('');
            $('#Relationship').val('');
            $('#ShortName').val('');
            $('#LdateModifyHide').hide();
            $(settings.saveSelector).html('<i class="fas fa-save"></i> Save');
            $('.text-danger').text('');
            $(settings.formSelector).attr('action', testSave);
            GenerateNextCode();

        }

        // Clear form on click
        $(document).on('click', settings.clearSelector, function () {
            ResetForm();
        });

        // Validation function
        function validation() {

            var Relationship = $('#Relationship').val();

            if (!Relationship) {
                toastr.info('Job Title   is required.');
                $('#Relationship').trigger('focus')
                return false;
            }

            return true;
        }
    }

}(jQuery));