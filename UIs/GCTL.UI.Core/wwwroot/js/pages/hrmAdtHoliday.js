(function ($) {
    $.hrmAdtHoliday = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#HrmHolidayInformation-form",
            formContainer: ".js-HrmHolidayInformation-form-container",
            gridSelector: "#HrmHolidayInformation-grid",
            gridContainer: ".js-HrmHolidayInformation-grid-container",
            editSelector: ".js-HrmHolidayInformation-edit",
            saveSelector: ".js-HrmHolidayInformation-save",
            selectAllSelector: "#HrmHolidayInformation-check-all",
            deleteSelector: ".js-HrmHolidayInformation-delete-confirm",
            deleteModal: "#HrmHolidayInformation-delete-modal",
            finalDeleteSelector: ".js-HrmHolidayInformation-delete",
            clearSelector: ".js-HrmHolidayInformation-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-HrmHolidayInformation-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-HrmHolidayInformation-check-duplicateName",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            //lastCodeSelector: '#lastCode',
            load: function () { }
        }, options);


        var baseControllerNameUrl = "/HrmAtdHolidayInformation";
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

                            toastr.error(result.message);
                        }
                        else if (result.isSuccess)
                        {
                            $(settings.gridContainer).html(result.html);
                            initialize();

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


            //



            //

            let loadUrl,
                target,
                reloadUrl,
                title,
                lastCode;
            // Quick add
            $("body").on("click", settings.quickAddSelector, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                loadUrl = $(this).data("url");
                target = $(this).data("target");
                reloadUrl = $(this).data("reload-url");
                title = $(this).data("title");

                $(settings.quickAddModal + " .modal-title").html(title);
                $(settings.quickAddModal + " .modal-body").empty();

                $(settings.quickAddModal + " .modal-body").load(loadUrl, function () {
                    $(settings.quickAddModal).modal("show");
                    $("#header").hide();
                    $(settings.quickAddModal + " .modal-body #header").hide()

                    $("#left_menu").hide();
                    $(settings.quickAddModal + " .modal-body #left_menu").hide()

                    $("#main-content").toggleClass("collapse-main");
                    $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")

                    $("body").removeClass("sidebar-mini");
                })
            });

            $("body").on("click", ".js-modal-dismiss", function () {
                $("body").removeClass("sidebar-mini").addClass("sidebar-mini");

                $("#header").show();
                $(settings.quickAddModal + " .modal-body #header").show()

                $("#left_menu").show();

                $(settings.quickAddModal + " .modal-body #left_menu").show()

                $("#main-content").toggleClass("collapse-main");
                $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")


                lastCode = $(settings.quickAddModal + " #lastCode").val();

                $(settings.quickAddModal + " .modal-body").empty();
                $(settings.quickAddModal).modal("hide");


                $(target).empty("");
                $(target).append($('<option>', {
                    value: '',
                    text: `--Select ${title}--`
                }));
                $.ajax({
                    url: reloadUrl,
                    method: "GET",
                    success: function (response) {
                        console.log(response);
                        $.each(response, function (i, item) {
                            $(target).append($('<option>', {
                                value: item.code,
                                text: item.name
                            }));
                        });

                       
                        console.log("Selected value:", lastCode);
                        $(target).val(lastCode).trigger('change');
                       /* alert(lastCode);*/
                    }
                });
            });

         

            //
            // Availability check

            
           

            $("body").on("keyup change", "#ToDateHidden, #FromDateHidden, .js-HolidayTypeCode, .js-HrmHolidayInformation-check-duplicateName", function ()
            {

                let code = $(".js-HolidayCode-code").val();
                let holidayTypeCode = $(".js-HolidayTypeCode").val();
                let name = $(".js-HrmHolidayInformation-check-duplicateName").val();
                let fromDate = $("#FromDateHidden").val();
                let toDate = $("#ToDateHidden").val();

                console.log("Data for Duplicate Check - Code:", code, "HolidayTypeCode:", holidayTypeCode, "HolidayName:", name, "FromDate:", fromDate, "toDate:", toDate);

                $.ajax({
                    url: duplicateCheckURL,
                    method: "POST",
                    data: { code: code, name: name, holidayTypeCode: holidayTypeCode, fromDate: fromDate, toDate: toDate },
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
                        toastr.error('Failed Delele.');
                        $(settings.deleteModal).modal('hide');
                    }
                });
            });

            //



        });




        // Initialization function
        function initialize()
        {
            WEFDatePicker();
            GenerateNextCode();
            loadTableData();
            ResetForm();
            select2DD();
        }
        function select2DD()
        {

            $('#HolidayTypeCodeDropDown').select2({
                language: {
                    noResults: function ()
                    {
                        
                    }
                },
                escapeMarkup: function (markup)
                {
                    return markup;
                }
            });
        }


        //


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
                    toastr.error('Failed Load Data.');
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


        // WEF Date Picker


        function WEFDatePicker()
        {
            function calculateDaysDifference()
            {
                var fromDate = $("#FromDate").datepicker("getDate");
                var toDate = $("#ToDate").datepicker("getDate");

                if (fromDate && toDate)
                {
                    var differenceInTime = (toDate - fromDate)+1;
                    var differenceInDays = Math.ceil(differenceInTime / (1000 * 60 * 60 * 24));
                    $('#NoOfDays').val(differenceInDays)
                } else {
                    $('#NoOfDays').val('');
                }
            }

            $("#FromDate").datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+10",
                regional: 'en-GB',
                beforeShow: function (input, inst) {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onClose: function (dateText, inst) {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onSelect: function (dateText) {
                    var formattedDate = $.datepicker.formatDate('mm/dd/yy', $(this).datepicker('getDate'));
                    $('#FromDateHidden').val(formattedDate).trigger('change');

                    console.log("Updated FromDateHidden form DatePicker:", $('#FromDateHidden').val());  
                    console.log("Updated FromDate form DatePicker:", $(this).val());

                    var fromDate = $(this).datepicker('getDate');
                    $("#ToDate").datepicker("option", "minDate", fromDate);
                    calculateDaysDifference();
                    $("#FromDate").valid();
                   
                }
            });

            var initialFromDate = $("#FromDate").val();
            if (initialFromDate) {
                var formattedInitialDate = $.datepicker.formatDate('mm/dd/yy', $("#FromDate").datepicker('getDate'));
               
                $('#FromDateHidden').val(formattedInitialDate);
            }

            $("#ToDate").datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+10",
                regional: 'en-GB',
                beforeShow: function (input, inst)
                {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onClose: function (dateText, inst)
                {
                    $(this).toggleClass("placeholder-shown", !this.value);
                },
                onSelect: function (dateText)
                {
                    var formattedDate = $.datepicker.formatDate('mm/dd/yy', $(this).datepicker('getDate'));
                    $('#ToDateHidden').val(formattedDate).trigger('change');

                    console.log("ToDateHidden:", $('#ToDateHidden').val());

                    calculateDaysDifference();
                    $("#ToDate").valid();
                }
            });

            var initialToDate = $("#ToDate").val();
            if (initialToDate)
            {
                var formattedInitialDate = $.datepicker.formatDate('mm/dd/yy', $("#ToDate").datepicker('getDate'));
                $('#ToDateHidden').val(formattedInitialDate);
            }

            // Set initial minDate for ToDate if FromDate has a value (for editing)
            var initialFromDate = $("#FromDate").datepicker('getDate');
            if (initialFromDate)
            {
                $("#ToDate").datepicker("option", "minDate", initialFromDate);
            }

            $(".datepicker").trigger("input");
        }


      
        //
        function GenerateNextCode()
        {
            $("#HolidayCode").val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result)
                {

                    $('#HolidayCode').val(result);
                },
                error: function ()
                {
                    toastr.error('Failed Next Code.');
                }
            });
        }


        function ResetForm()
        {

            $(settings.formSelector)[0].reset();
            $('#Id').val('');
            $('#HolydayInfoAutoId').val('');
            $('#HolidayName').val('').trigger('focus');
           // $('#HolidayTypeCodeDropDown').val(null).trigger('change');
            $('#ToDate').val('');
            $('#FromDate').val('');
            $('#FromDateHidden').val('');
            $('#NoOfDays').val('');
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
            var name = $("#HolidayName").val();
            var dropDown = $('#HolidayTypeCodeDropDown').val();
            var fromDate = $('#FromDate').val();
            var toDate = $('#ToDate').val();
           
            if (!name)
            {
                toastr.info('Enter Holiday Name');
                $('#HolidayName').trigger('focus');
                return false;
            }
            if (!dropDown)
            {
                toastr.info('Select Holiday Type');
                $('#HolidayTypeCodeDropDown').select2('open');
                return false;
            }
            if (!fromDate)
            {
                toastr.info('Select From Date')
                $('#FromDate').trigger('focus');
                return false;
            }
            if (!toDate)
            {
                toastr.info('Select To Date');
                $('#ToDate').trigger('focus');
                return false;
            }
          

        }
    
    }
}(jQuery));













