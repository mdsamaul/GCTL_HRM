(function ($) {
    $.performancesType = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#Performancess-form",
            formContainer: ".js-Performancess-form-container",
            gridSelector: "#Performancess-grid",
            gridContainer: ".js-Performancess-grid-container",
            editSelector: ".js-Performancess-edit",
            saveSelector: ".js-Performancess-save",
            selectAllSelector: "#Performancess-check-all",
            deleteSelector: ".js-Performancess-delete-confirm",
            deleteModal: "#Performancess-delete-modal",
            finalDeleteSelector: ".js-Performancess-delete",
            clearSelector: ".js-Performancess-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-Performancess-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-Performancess-check-duplicateName",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: '#lastCode',

            load: function () { }
        }, options);

        var baseControllerNameUrl = "/PerformancesType";
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
                var jobId = $(".js-JobTitleId").val();

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
                            loadTableData(jobId);
                            toastr.success(result.message, 'success');
                        } else {
                            $(settings.formSelector).html(result);
                            initialize();
                        }
                    },
                    error: function () {
                        toastr.error('An error saving Performance Entry.');
                    }
                });
            });

            // Availability check
            $("body").on("keyup", settings.availabilitySelector, function () {
                var self = $(this);
                let code = $(".js-PerformanceCode-code").val();
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
                    toastr.error('Error Performance Entry details.');
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
                    toastr.error('select Performance to delete.');
                    return;
                }

                $(settings.deleteModal + ' ' + settings.finalDeleteSelector).data('ids', selectedIds);
                $(settings.deleteModal).modal('show');
            });

            // Final delete action
            $("body").on('click', settings.finalDeleteSelector, function (e) {
                e.preventDefault();
                var selectedIds = $(this).data('ids');
                var jobId = $(".js-JobTitleId").val();

                $.ajax({
                    type: 'POST',
                    url: deleteURL,
                    contentType: 'application/json',
                    data: JSON.stringify(selectedIds),
                    success: function (result) {
                        if (result.isSuccess) {

                            loadTableData(jobId);
                           
                            initialize();
                            toastr.success(result.message);
                        } else {

                            toastr.error(result.message);
                        }
                        $(settings.deleteModal).modal('hide');
                        
                    },
                    error: function (xhr) {
                        toastr.error('An deleting the selected Performance Entry.');
                        $(settings.deleteModal).modal('hide');
                       
                    }
                });
            });

            




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
                        $.each(response, function (i, item) {
                            $(target).append($('<option>', {
                                value: item.code,
                                text: item.name
                            }));
                        });
                        $(target).val(lastCode);
                    }
                });
            });
        });




        // Initialization function
        function initialize() {

            GenerateNextCode();
            //loadTableData();
            /*loadTableData(#jobTitleId)*/
            ResetForm();
            $(settings.formSelector + ' .selectpicker2').select2({
                language: {
                    noResults: function () {
                        
                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });
        }

        //  

        $("body").on("change", ".js-JobTitleId", function ()
        {
            var jobTitleId = $(".js-JobTitleId").val();
            loadTableData(jobTitleId); 
        });

        function loadTableData(jobTitleId) {
            $.ajax({
               
                type: 'GET',
                url: loadTableURL, 
                data: { jobTitleCode: jobTitleId }, 
                success: function (data) {
                    console.log("Response data:", data);
                    $(settings.gridContainer).html(data);
                    dataTable();
                },
                error: function (xhr, status, error) {
                  
                    toastr.error("loading the performance.");
                }
            });
        }

        //
    


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
            $("#PerformanceCode").val('');
            $.ajax({
                type: 'GET',
                url: nextCodeULR,
                success: function (result) {

                    $('#PerformanceCode').val(result);
                },
                error: function () {
                    toastr.error(' generating the next code.');
                }
            });
        }

        function ResetForm() {

           /* $(settings.formSelector)[0].reset();*/
            $('#PerformanceCode').val('');
          //  $('#JobTitleId').val('');
            $('#AutoId').val('');
            $('#Performance').val('');
            $('#PerformanceShortName').val('');
            $('#LdateModifyHide').hide();
            $(settings.saveSelector).html('<i class="fas fa-save"></i> Save');
            $('.text-danger').text('');
            $(settings.formSelector).attr('action', testSave);
            GenerateNextCode();
            loadTableData('#jobTitleId');

        }


        // Clear form on click
        $(document).on('click', settings.clearSelector, function () {
            // ResetForm();  

             $(settings.formSelector)[0].reset();
            $('#PerformanceCode').val('');
            $('#JobTitleId').trigger('change');
            $('#AutoId').val('');
            $('#Performance').val('');
            $('#PerformanceShortName').val('');
            $('#LdateModifyHide').hide();
            $(settings.saveSelector).html('<i class="fas fa-save"></i> Save');
            $('.text-danger').text('');
            $(settings.formSelector).attr('action', testSave);
            GenerateNextCode();
            loadTableData('#jobTitleId');
           
        });



        // Validation function
        function validation() {

            var Performance = $('#Performance').val();

            if (!Performance) {
                toastr.info('Performance is required.');
                $('#Performance').trigger('focus')
                return false;
            }

            return true;
        }

    }
}(jQuery));

