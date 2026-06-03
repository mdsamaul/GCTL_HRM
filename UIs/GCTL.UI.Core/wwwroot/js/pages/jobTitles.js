(function ($) {
    $.jobTitles = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#JobTitle-form",
            formContainer: ".js-JobTitle-form-container",
            gridSelector: "#JobTitle-grid",
            gridContainer: ".js-JobTitle-grid-container",
            editSelector: ".js-JobTitle-edit",
            saveSelector: ".js-JobTitle-save",
            selectAllSelector: "#JobTitle-check-all",
            deleteSelector: ".js-JobTitle-delete-confirm",
            deleteModal: "#JobTitle-delete-modal",
            finalDeleteSelector: ".js-JobTitle-delete",
            clearSelector: ".js-JobTitle-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-JobTitle-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-JobTitle-check-duplicateName",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () { }
        }, options);


        var baseControllerNameUrl = "/JobTitles";
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

                            toastr.warning(result.message);
                        }
                        else if (result.isSuccess) {
                            $(settings.gridContainer).html(result.html);
                            initialize();

                            toastr.success(result.message);
                        } else {
                            $(settings.formSelector).html(result);
                            initialize();
                        }
                    },
                    error: function () {
                        toastr.error('saving Job Title data.');
                    }
                });
            });

            // Availability check
            //$("body").on("keyup", settings.availabilitySelector, function () {
            //    var self = $(this);
            //    let code = $(".js-JobTitleId-code").val();
            //    let name = self.val();

            //    $.ajax({
            //        url: duplicateCheckURL,
            //        method: "POST",
            //        data: { code: code, name: name },
            //        success: function (response) {
            //            console.log(response);
            //            if (response.isSuccess) {
            //                toastr.info(response.message);
            //            }
            //        }
            //    });
            //});

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
                    toastr.warning(' Please try again.');
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
                    toastr.success('Please Job Title to delete.');
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
                        //console.log(result);
                        if (result.success) {

                           
                            toastr.success(result.message);
                            initialize();
                        } else {

                            toastr.info(result.message);
                            initialize();
                        }
                        $(settings.deleteModal).modal('hide'); 
                    },
                    error: function (xhr) {
                        toastr.error('An error Job Title .');
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
            dataTable();

        }

        // Load leave types list
        function loadTableData() {-
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
            $(".js-JobTitleId-code").val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result) {

                    $('.js-JobTitleId-code').val(result);
                },
                error: function () {
                    toastr.error('An error occurred while generating the next code.');
                }
            });
        }

        function ResetForm() {

            $(settings.formSelector)[0].reset();
            $('#JobTitleId').val('');
            $('#AutoId').val('');
            $('#JobTitle').val('');
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

            var JobTitle = $('#JobTitle').val();

            if (!JobTitle) {
                toastr.info('Enter Job Title');
                $('#JobTitle').trigger('focus')
                return false;
            }

            return true;
        }
    }
}(jQuery));



