(function ($) {
    $.bankBranchInformations = function (options) {
        // Default options
        var settings = $.extend({

            baseUrl: "/",
            formSelector: "#BankBranchInformations-form",
            formContainer: ".js-BankBranchInformations-form-container",
            gridSelector: "#BankBranchInformations-grid",
            gridContainer: ".js-BankBranchInformations-grid-container",
            editSelector: ".js-BankBranchInformations-edit",
            saveSelector: ".js-BankBranchInformations-save",
            selectAllSelector: "#BankBranchInformations-check-all",
            deleteSelector: ".js-BankBranchInformations-delete-confirm",
            deleteModal: "#BankBranchInformations-delete-modal",
            finalDeleteSelector: ".js-BankBranchInformations-delete",
            clearSelector: ".js-BankBranchInformations-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-BankBranchInformations-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: "#BankBranchName",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () { }
        }, options);


        var baseControllerNameUrl = "/BankBranchInformations";
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

                            toastr.error(result.message);
                        }

                        else if (result.isSuccess) {
                            $(settings.gridContainer).html(result.html);
                            initialize();
                            $(settings.lastCodeSelector).val(result.lastCode);
                            // alert(`LastCode: ${result.lastCode}`);
                            toastr.success(result.message);
                        } else {

                            toastr.success(result.message);
                            $(settings.formSelector).html(result);
                            initialize();
                        }
                    },
                    error: function () {
                        toastr.error('Failed Insert.');
                    }
                });
            });

            // Availability check
            $("body").on("keyup change", "#BankId,#BankBranchName", function () {
                //var self = $(this);
                let name = $("#BankBranchName").val();
                let typeCode = $(".js-BankBranchId-code").val();
                // let name = self.val();
                let bankId = $('#BankId').val();
                $.ajax({
                    url: duplicateCheckURL,
                    method: "POST",
                    data: { typeCode: typeCode, bankId: bankId, name: name },
                    success: function (response) {
                        if (response.isSuccess) {
                            toastr.warning(response.message);

                        }
                    }
                });
            });

            //


            //


            // Edit leave type
            $("body").on('click', settings.editSelector, function () {
                var id = $(this).data('id');
                $.get(getById, { id: id }, function (result) {

                    $(settings.formSelector).html($(result).find(settings.formSelector).html());
                    select2DD();
                    $(settings.saveSelector).html('<i class="fas fa-edit"></i> Update');
                    $(settings.formSelector).attr('action', testSave);
                }).fail(function () {
                    toastr.error('Error Get Data.');
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

                $('.checkBox:checked').each(function () {
                    selectedIds.push($(this).val());
                });

                if (selectedIds.length === 0) {
                    toastr.error('Please select Bank Branch to delete.');
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
                        toastr.error('Failed Delete.');
                        $(settings.deleteModal).modal('hide');
                    }
                });
            });

            //



        });

        // Initialization function
        function initialize() {


            select2DD();
            GenerateNextCode();
            loadTableData();
            ResetForm();

        }

        function select2DD() {
            $('.selectpickerBankDD').select2({
                width: '100%',
                language: {
                    noResults: function () {

                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });
        }


        function loadTableData() {
            $.ajax({
                type: 'GET',
                url: loadTableURL,
                success: function (data) {
                    $(settings.gridContainer).html(data);
                    dataTable();
                },
                error: function () {
                    toastr.error('Failed Load Data');
                }
            })
        }

        // Data table initialization function
        function dataTable() {
            $(settings.gridSelector).DataTable({
                responsive: true,
                pageLength: 10,
                destroy: true,
                lengthMenu: [10, 25, 50, 100],
            });
        }

        function GenerateNextCode() {
            $("#BankBranchMasterId").val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result) {

                    $('#BankBranchMasterId').val(result);
                },
                error: function () {
                    toastr.error('Failed Next Code.',);
                    //  /location.assign('/Accounts / Login');
                    // window.location.href = '/Departments/Index';
                    //window.location.href = '/Accounts/Login';
                }
            });
        }

        function ResetForm() {

            $(settings.formSelector)[0].reset();
            $('#BankBranchAutoId').val('');
            $('.selectpickerBankDD').val(null).trigger('change');
            $('#BankBranchMasterId').val('');
            $('#BankBranchName').val('');
            $('#Address').val('');
            $('#ShortName').val('');
            $('#Phone').val('');
            $('#Swiftcode').val('');
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

            var bankName = $('#BankIdDropDown').val();
            var branchName = $('#BankBranchMasterId').val();
            if (!bankName)
            {
                toastr.warning('Select Bank.');
                $('#BankIdDropDown').select2('open');
                return false;
            }
            if (!branchName) {
                toastr.warning('Enter Branch Name');
                $('#BankBranchMasterId').trigger('focus');
                return false;
            }
            return true;
        }
    }
}(jQuery));

