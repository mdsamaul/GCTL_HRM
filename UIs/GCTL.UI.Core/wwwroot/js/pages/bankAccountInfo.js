(function ($) {
    $.bankAccountInfo = function (options) {
        // Default options
        var settings = $.extend({
            
            baseUrl: "/",
            formSelector: "#BankAccountInfo-form",
            formContainer: ".js-BankAccountInfo-form-container",
            gridSelector: "#BankAccountInfo-grid",
            gridContainer: ".js-BankAccountInfo-grid-container",
            editSelector: ".js-BankAccountInfo-edit",
            saveSelector: ".js-BankAccountInfo-save",
            selectAllSelector: "#BankAccountInfo-check-all",
            deleteSelector: ".js-BankAccountInfo-delete-confirm",
            deleteModal: "#BankAccountInfo-delete-modal",
            finalDeleteSelector: ".js-BankAccountInfo-delete",
            clearSelector: ".js-BankAccountInfo-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-BankAccountInfo-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: "#BankBranchName",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',
            load: function () { }
        }, options);


        var baseControllerNameUrl = "/BankAccountInfo";
        var testSave = baseControllerNameUrl + "/Setup";
        var nextCodeULR = baseControllerNameUrl + "/GenerateNextCode";
        var loadTableURL = baseControllerNameUrl + "/GetTableData";
        var deleteURL = baseControllerNameUrl + "/Delete";
        var getById = baseControllerNameUrl + "/Index";
        var duplicateCheckURL = baseControllerNameUrl + "/CheckAvailability";

        var selectedItems = [];

        $(() => {




            initialize();
            bankBranchFilterDD();


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
                        if (result.noSavePermission || result.noUpdatePermission || result.isDuplicate)
                        {

                            toastr.error(result.message);
                        }

                        else if (result.isSuccess)
                        {
                            $(settings.gridContainer).html(result.html);
                            initialize();
                            $(settings.lastCodeSelector).val(result.lastCode);
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
            //

            // Edit leave type
            $("body").on('click', settings.editSelector, function () {
                var id = $(this).data('id');
              
                $.get(getById, { id: id }, function (result)
                {

                    $(settings.formSelector).html($(result).find(settings.formSelector).html());
                    var bankId = $("#BankId").val();
                    var branchId = $("#BranchId").val();

                    // Store the selected BranchId for later use (important for editing)
                    $("#BranchId").data('selected', branchId);
                    select2DD();
                    bankBranchFilterDD();
                    $(settings.saveSelector).html('<i class="fas fa-edit"></i> Update');
                    $(settings.formSelector).attr('action', testSave);
                   
                    if (bankId)
                    {
                        loadBranchesForBank(bankId);
                    }

                    $(settings.deleteSelector).data('id', id);
                }).fail(function () {
                    toastr.error('Error Get Data.');
                });
            });



            //

            //
            // Availability check
            $("body").on("keyup change", "#AccountNo,#AccountName,#BankId,#BranchId", function ()
            {
                //var self = $(this);
                let accountName = $("#AccountName").val();
                let typeCode = $("#AccInfoId").val();
                // let name = self.val();
                let accountNo = $('#AccountNo').val();
                var bankId = $("#BankId").val();
                var branchId = $("#BranchId").val();
                $.ajax({
                    url: duplicateCheckURL,
                    method: "POST",
                    data: { typeCode: typeCode, accountName: accountName, accountNo: accountNo, bankId: bankId, branchId: branchId },
                    success: function (response) {
                        if (response.isSuccess) {
                            toastr.warning(response.message);

                        }
                    }
                });
            });

            //Drop down branch according to Bank

            function bankBranchFilterDD() {
                $("body").on("change", "#BankId", function ()
                {
                    var bankId = $(this).val();
                    var branchDropdown = $('#BranchId');

                   // branchDropdown.empty();
                    branchDropdown.empty().append('<option value="">--Select Branch Name--</option>');

                    if (bankId) {
                        $.ajax({
                            type: 'GET',
                            url: '/BankAccountInfo/GetBranchesByBankId',
                            data: { bankId: bankId },
                            success: function (data) {
                                if (data && data.length > 0)
                                {
                                    $.each(data, function (index, item)
                                    {
                                        branchDropdown.append('<option value="' + item.value + '">' + item.text + '</option>');
                                    });
                                } else {

                                    toastr.warning('No branches found for the selected bank.'); 
                                }
                            },
                            error: function ()
                            {
                                toastr.error('Failed to load branches.');
                            }
                        });
                    }
                });
            }


            //

            function loadBranchesForBank(bankId) {
                var branchDropdown = $('#BranchId');
                branchDropdown.empty().append('<option value="">--Select Branch Name--</option>');

                if (bankId) {
                    $.ajax({
                        type: 'GET',
                        url: '/BankAccountInfo/GetBranchesByBankId',
                        data: { bankId: bankId },
                        success: function (data) {
                            if (data && data.length > 0) {
                                $.each(data, function (index, item)
                                {
                                    branchDropdown.append('<option value="' + item.value + '">' + item.text + '</option>');
                                });
                                var selectedBranchId = $("#BranchId").data('selected'); 
                                if (selectedBranchId)
                                {
                                    branchDropdown.val(selectedBranchId);
                                }
                            } else
                            {
                                /*toastr.warning('No branches found for the selected bank.');*/
                            }
                        },
                        error: function ()
                        {
                            toastr.error('Failed to load branches.');
                        }
                    });
                }
            }

           
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

                if (selectedIds.length === 0)
                {
                    toastr.error('Please select Bank  to delete.');
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
        });




        // Initialization function
        function initialize() {


            select2DD();
            GenerateNextCode();
            loadTableData();
            ResetForm();
            
        }

        function select2DD() {
            $('.selectpickerBankAccount').select2({
                language: {
                    noResults: function () {

                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });
        }


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
                lengthMenu: [ 10, 25, 50, 100],
            });
        }




        function GenerateNextCode() {
            $("#AccInfoId").val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result) {

                    $('#AccInfoId').val(result);
                },
                error: function () {
                    toastr.error('Failed Next Code.',);
                    //  /location.assign('/Accounts / Login');
                    // window.location.href = '/Departments/Index';
                    //window.location.href = '/Accounts/Login';
                }
            });
        }




        function ResetForm()
        {

            $(settings.formSelector)[0].reset();
            $('#Id').val('');
            $('#AutoId').val('');
            $('#BankId').val(null).trigger('change');
            $('#BranchId').val(null).trigger('change');
           // $('#AccInfoId').val('');
            $('#AccountNo').val('');
            $('#AccountName').val('');
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

            var bankName = $("#BankId").val();
           var branchName = $("#BranchId").val();
            var acNo = $('#AccountNo').val();
            var acName = $('#AccountName').val();

            if (!bankName) {
                toastr.warning('Select Bank Name.');
                 $('#BankId').select2('open')
                return false;
            }

            if (!branchName) {
                toastr.warning('Select Branch Name.');
              
                 $('#BranchId').select2('open')
                return false;
            }

            if (!acName)
            {
                toastr.warning('Enter A/C Name.');
             
                return false;
            }
            if (!acNo)
            {
                toastr.warning('Enter A/C No.')
                return false;
            }
            return true;
        }



    }
}(jQuery));







