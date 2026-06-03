

    (function ($) {
        $.companyWeekEnds = function (options) {
            // Default options
            var settings = $.extend({
                baseUrl: "/",
                formSelector: "#companyWeekEnds-form",
                formContainer: ".js-companyWeekEnds-form-container",
                gridSelector: "#companyWeekEnds-grid",
                gridContainer: ".js-companyWeekEnds-grid-container",
                editSelector: ".js-companyWeekEnds-edit",
                saveSelector: ".js-companyWeekEnds-save",
                selectAllSelector: "#companyWeekEnds-check-all",
                deleteSelector: ".js-companyWeekEnds-delete-confirm",
                deleteModal: "#companyWeekEnds-delete-modal",
                finalDeleteSelector: ".js-companyWeekEnds-delete",
                clearSelector: ".js-companyWeekEnds-clear",
                topSelector: ".js-go",
                decimalSelector: ".js-companyWeekEnds-decimalplaces",
                maxDecimalPlace: 5,
                showNagativeFormat: false,
                availabilitySelector: ".js-companyWeekEnds-check-duplicateName",
                haseFile: false,
                quickAddSelector: ".js-quick-add",
                quickAddModal: "#quickAddModal",
                lastCodeSelector: '#lastCode',
                load: function () { }
            }, options);


            var baseControllerNameUrl = "/HRMCompanyWeekEnds";
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
                            if (result.isDuplicate || result.noSavePermission || result.noUpdatePermission) {
                                toastr.error(result.message,'Error');
                            } else if (result.isSuccess) {
                                $(settings.gridContainer).html(result.html);
                                initialize();

                                toastr.success(result.message,'Success');
                            } else {
                                $(settings.formSelector).html(result);
                                initialize();
                            }
                        },
                        error: function () {
                            toastr.error('Failed Insert.');
                        }
                    });
                });

               

                
                $("body").on("change", "input[name='Weekend']", function () {
                    let code = $(".js-ShiftCode-code").val();
                    let weekends = $("input[name='Weekend']:checked").map(function () {
                        return $(this).val();
                    }).get();

                    $.ajax({
                        url: duplicateCheckURL,
                        method: "POST",
                        data: { code: code, weekends: weekends },
                        traditional: true,
                        success: function (response)
                        {
                            if (response.isSuccess)
                            {
                              
                                toastr.error(response.message,'Error');
                            }
                            
                        },
                        error: function (xhr, status, error) {
                            console.log("Error occurred:", error);
                        }
                    });
                });





                //




                //Orginall Code 


                // Edit leave type
                $("body").on('click', settings.editSelector, function () {
                    var id = $(this).data('id');
                    $.get(getById, { id: id }, function (result)

                    {

                        $(settings.formSelector).html($(result).find(settings.formSelector).html());
                        EffectiveDatePicker();
                        $(settings.saveSelector).html('<i class="fas fa-edit"></i> Update');
                        $(settings.formSelector).attr('action', testSave);
                        $(settings.deleteSelector).data('id', id);
                    }).fail(function ()
                    {
                        toastr.error('Error Update.','Error');
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
                        error: function (xhr)
                        {
                            toastr.error('Failed Delete.','Error');
                            $(settings.deleteModal).modal('hide');
                        }
                    });
                });

                //



            });




            // Initialization function
            function initialize()
            {
                EffectiveDatePicker();
                GenerateNextCode();
                loadTableData();
                ResetForm();
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
                        toastr.error('Failed Get Data .','Error');
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


            function EffectiveDatePicker()
            {
                

                $("#EffectiveDate").datepicker({
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
                        $('#EffectiveDateHidden').val(formattedDate);
                        $("#EffectiveDate").valid();
                    }
                });

                var initialDate = $("#EffectiveDate").val();
                if (initialDate) {
                    var formattedInitialDate = $.datepicker.formatDate('mm/dd/yy', $("#EffectiveDate").datepicker('getDate'));
                    $('#EffectiveDateHidden').val(formattedInitialDate);
                }

                $(".datepicker").trigger("input");

                
            }


            



            

            function GenerateNextCode()
            {
                $("#CompanyWeekEndCode").val('');
                $.ajax({
                    type: 'GET',
                    url: nextCodeULR,
                    success: function (result)
                    {

                        $('#CompanyWeekEndCode').val(result);
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
                $('#CompanyWeekEndCode').val('');
                $('#CompanyWeekEndCode').val('');
                $('input[name="Weekend"]').prop('checked', false); 
                $('#EffectiveDate').val('');
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
                var weekendSelected = $("input[name='Weekend']:checked").length > 0;
                var wefdate = $('#EffectiveDate').val();

                if (!weekendSelected)
                {
                    toastr.info('Select Weekend.');
                    return false;
                }
                if (!wefdate) {
                    toastr.info('Select Effective Date.');
                    return false;
                }

                return true; 
            }


           
        }
    }(jQuery));



