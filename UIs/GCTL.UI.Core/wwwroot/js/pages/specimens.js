(function ($) {
    $.specimens = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#specimen-form",
            formContainer: ".js-specimen-form-container",
            gridSelector: "#specimen-grid",
            gridContainer: ".js-specimen-grid-container",
            editSelector: ".js-specimen-edit",
            saveSelector: ".js-specimen-save",
            selectAllSelector: "#specimen-check-all",
            deleteSelector: ".js-specimen-delete-confirm",
            deleteModal: "#specimen-delete-modal",
            finalDeleteSelector: ".js-specimen-delete",
            clearSelector: ".js-specimen-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-specimen-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-specimen-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            lastCodeSelector: "#lastCode",
            load: function () {

            }
        }, options);


        var gridUrl = settings.baseUrl + "/grid";
        var saveUrl = settings.baseUrl + "/setup";
        var deleteUrl = settings.baseUrl + "/Delete";
        var selectedItems = [];
        $(() => {
            //settings.load(settings.baseUrl, settings.gridSelector);
            loadSpecimens(settings.baseUrl, settings.gridSelector);
            initialize();
            $("body").on("click", `${settings.editSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = saveUrl + "/" + $(this).data("id") ?? "";
                loadForm(url);

                $("html, body").animate({ scrollTop: 0 }, 500);
            });

            $("body").on("click", `${settings.clearSelector}`, function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.stopImmediatePropagation();

                let url = saveUrl + "/" + $(this).data("id") ?? "";
                loadForm(saveUrl)
                    .then((data) => {
                        loadSpecimens(settings.baseUrl, settings.gridSelector);
                    })
                    .catch((error) => {
                        console.log(error)
                    })
            });


           
            // Save
            $("body").on("click", settings.saveSelector, function () {
                var $valid = $(settings.formSelector).valid();
                if (!$valid) {
                    return false;
                }

                var data;
                if (settings.haseFile)
                    data = new FormData($(settings.formSelector)[0]);
                else
                    data = $(settings.formSelector).serialize();

                var url = $(settings.formSelector).attr("action");

                var options = {
                    url: url,
                    method: "POST",
                    data: data,
                    success: function (response) {
                        if (response.isSuccess) {
                            //loadForm(saveUrl)
                            //    .then((data) => {
                            //        loadSpecimens(settings.baseUrl, settings.gridSelector);
                            //        $(settings.lastCodeSelector).val(response.lastCode);
                            //    })
                            //    .catch((error) => {
                            //        console.log(error)
                            //    })
                            generateCode();
                            $("#Specimen").val("");
                            loadSpecimens(settings.baseUrl, settings.gridSelector);
                            toastr.success(response.success, 'Success');
                        }
                        else {
                            toastr.error(response.message, 'Error');
                            console.log(response);
                        }
                    }
                }
                if (settings.haseFile) {
                    options.processData = false;
                    options.contentType = false;
                }
                $.ajax(options);
            });

            $("body").on("click", settings.selectAllSelector, function () {
                $(".checkBox").prop('checked',
                    $(this).prop('checked'));
            });


            $("body").on("click", settings.deleteSelector, function (e) {
                e.preventDefault();
                $('input:checkbox.checkBox').each(function () {
                    if ($(this).prop('checked')) {
                        if (!selectedItems.includes($(this).val())) {
                            selectedItems.push($(this).val());
                        }
                    }
                });

                if (selectedItems.length > 0) {
                    $(settings.deleteModal).modal("show");
                } else {
                    toastr.info("Please select at least one item.", 'Warning');
                }
            });


            $("body").on('show.bs.modal', settings.deleteModal, function (event) {
                //event.preventDefault();
                // Get button that triggered the modal
                var source = $(event.relatedTarget);
                var id = source.data("id");

                // Extract value from data-* attributes
                var title = source.data("title");
                title = "Are you sure want to delete these items?";
                var modal = $(this);
                $(modal).find('.title').html(title);

                $("body").on("click", settings.finalDeleteSelector, function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    e.stopImmediatePropagation();


                    // Delete
                    $.ajax({
                        url: deleteUrl + "/" + selectedItems,
                        method: "POST",
                        success: function (response) {
                            console.log(response);
                            $(modal).modal("hide");
                            if (response.success) {
                                toastr.success(response.message, 'Success');
                                selectedItems = [];
                                /*settings.load(settings.baseUrl);*/
                                loadSpecimens(settings.baseUrl, settings.gridSelector);
                                // loadForm(saveUrl);
                                generateCode();
                            }
                            else {
                                toastr.error(response.message, 'Error');
                                console.log(response);
                            }
                        }
                    });
                });

            }).on('hide.bs.modal', function () {
                $("body").off("click", settings.finalDeleteSelector);
            });

            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 0 }, 500);
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
                    $("body").removeClass("sidebar-mini");
                    $(settings.quickAddModal + " .modal-body #header").hide();
                    $(settings.quickAddModal + " .modal-body #sidebar").hide();
                    $(settings.quickAddModal + " .modal-body .main-footer").hide();
                })
            });

            $("body").on("click", ".js-modal-dismiss", function () {
                $("body").removeClass("sidebar-mini").addClass("sidebar-mini");
                lastCode = $(settings.quickAddModal + " #lastCode").val();

                $(settings.quickAddModal + " .modal-body").empty();
                $(settings.quickAddModal).modal("hide");


                $(target).empty("");
                $(target).append($('<option>', {
                    value: '',
                    text: `---Select ${title}---`
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

            $("body").on("keyup", settings.availabilitySelector, function () {
                var self = $(this);
                let code = $(".js-specimen-code").val();
                let name = self.val();

                // check
                $.ajax({
                    url: settings.baseUrl + "/CheckAvailability",
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


            $("body").on("change", "#TestCategoryCode", function (e) {
                var self = $(this);
                if (self.val().length > 0) {
                    $.ajax({
                        url: normalizeUrl(getBaseUrl()) + "/Cascading/GetTestSubCategories",
                        method: "POST",
                        data: { testCategoryCode: self.val() },
                        success: function (response) {
                            $("#TestSubCategoryCode").empty();
                            $("#TestSubCategoryCode").append($('<option>', {
                                value: '',
                                text: `Select Test Sub Category`
                            }));
                            $.each(response, function (i, item) {
                                $("#TestSubCategoryCode").append($('<option>', {
                                    value: item.code,
                                    text: item.name
                                }));
                            });
                            getTests();
                            loadSpecimens(settings.baseUrl, settings.gridSelector);
                        }
                    });
                }               
            });

 
            $("body").on("change", "#TestCategoryCode, #TestSubCategoryCode", function (e) {
                getTests();
                loadSpecimens(settings.baseUrl, settings.gridSelector);
            });

       
            $("body").on("click", ".js-export", function () {
                var self = $(this);
                var categoryCode = $("#TestCategoryCode").val();

                let reportRenderType = $(".export-format").val();
                reportRenderType = "PDF";
                window.open(
                    settings.baseUrl + `/Export?categoryCode=${categoryCode}&reportType=LabSpecimens&reportRenderType=${reportRenderType}`,
                    "_blank"
                )
            });
        });



        function getTests() {
            var data = {
                testCategoryCode: $("#TestCategoryCode").val(),
                testSubCategoryCode: $("#TestSubCategoryCode").val(),
            };
            $.ajax({
                url: normalizeUrl(getBaseUrl()) + "/Cascading/GetTests",
                method: "POST",
                data: data,
                success: function (response) {
                    $("#TestChargeCode").empty();
                    //$("#TestChargeCode").append($('<option>', {
                    //    value: '',
                    //    text: `Select Test`
                    //}));
                    $.each(response, function (i, item) {
                        $("#TestChargeCode").append($('<option>', {
                            value: item.code,
                            text: item.name
                        }));
                    });

                    refreshControl();
                }
            });
        }

        function generateCode() {
            $.ajax({
                url: settings.baseUrl + "/GetNextCode",
                method: "POST",
                success: function (response) {
                    if (response.isSuccess)
                        $("#SpecimenId").val(response.message);
                    else
                        toastr.error(response.message);
                }
            });
        }
        function loadSpecimens(baseUrl, gridSelector) {
            var data = {
                categoryCode: $("#TestCategoryCode").val(),
                subCategoryCode: $("#TestSubCategoryCode").val()
            };
            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/grid",
                    type: "GET",
                    datatype: "json",
                    data: data
                },

                columnDefs: [
                    { targets: [0], orderable: false },
                ],
                columns: [
                    {
                        "data": "specimenId", "className": "text-center", "render": function (data) {
                            return ` <input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "specimenId", "render": function (data) {
                            return `<a class='btn js-specimen-edit' data-id='${data}'><i>${data}</i></a>`;
                        }
                    },
                    { "data": "specimen", "autowidth": true, "className": "text-center" },
                    { "data": "roomNo", "autowidth": true, "className": "text-center" },
                    { "data": "testChargeName", "autowidth": true, "className": "text-left" },
                    { "data": "testSubCategoryName", "autowidth": true, "className": "text-center" },
                    { "data": "testCategoryName", "autowidth": true, "className": "text-center" },                  
                    { "data": "luser", "autowidth": true, "className": "text-center" }
                ],
                lengthChange: true,
                pageLength: 10,
                lengthMenu: [
                    [10, 25, 50, -1],
                    [10, 25, 50, 'All'],
                ],
                order: [[1, "Desc"]],
                destroy: true
            });
        }
        
        function loadForm(url) {
            return new Promise((resolve, reject) => {
                $.ajax({
                    url: url,
                    type: 'GET',
                    success: function (data) {
                        $(settings.formContainer).empty();
                        $(settings.formContainer).html(data);
                        $.validator.unobtrusive.parse($(settings.formSelector));

                        initialize();
                        resolve(data)
                    },
                    error: function (error) {
                        reject(error)
                    },
                })
            })
        }

        function initialize() {
            $(settings.formSelector + ' .selectpicker').select2({
                language: {
                    noResults: function () {
                        //return 'Not found <a class="add_new_item" href="javascript:void(0)">Add New</a>';
                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                }
            });

            $('.multiselect').multiselect({
                includeSelectAllOption: true,
                enableCaseInsensitiveFiltering: true,
                buttonContainer: '<div class="btn-group w-100" />',
                onSelectAll: function (options) {
                    // alert('onSelectAll triggered, ' + options.length + ' options selected!');
                }
            });
        }

        function refreshControl() {
            $('.multiselect').multiselect('destroy');
            initialize();
        }
    }

}(jQuery));

