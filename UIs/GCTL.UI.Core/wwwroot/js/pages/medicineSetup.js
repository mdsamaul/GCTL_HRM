(function ($) {
    $.medicines = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#medicine-form",
            formContainer: ".js-medicine-form-container",
            gridSelector: "#medicines-grid",
            gridContainer: ".js-medicine-grid-container",
            editSelector: ".js-medicine-edit",
            saveSelector: ".js-medicine-save",
            selectAllSelector: "#medicine-check-all",
            deleteSelector: ".js-medicine-delete-confirm",
            deleteModal: "#medicine-delete-modal",
            finalDeleteSelector: ".js-medicine-delete",
            clearSelector: ".js-medicine-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-medicine-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-medicine-check-availability",
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
            initialize();
            medicineDescription();
            packingDetails();

            $("body").on("keyup", settings.availabilitySelector, function () {
                var self = $(this);
                let code = $(".js-medicine-code").val();
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

            $("body").on("click", ".js-file-chooser", function (e) {
                e.preventDefault();
                var target = $(this).data("target");
                $(target).trigger("click");
            });

            $("body").on("change", ".js-file", function (e) {
                e.preventDefault();
                var target = $(this).data("target");
                showImagePreview($(this), target);
            });

            $("body").on("click", ".js-clear-file", function (e) {
                e.preventDefault();
                var file = $(this).data("file");
                var tag = $(this).data("tag");
                clearImage(file, tag);
            });

            $("body").on("keyup", "#MedicineName", function () {
                medicineDescription();
            });

            $("body").on("keyup", "#Strength", function () {
                medicineDescription();
            });

            $("body").on("keyup", "#Quantity", function () {
                packingDetails();
            });

            $("body").on("change", "#UnitId", function () {
                packingDetails();
            });

            $("body").on("change", "#PackUnitId", function () {
                packingDetails();
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
                    // $("#header").hide();
                    $(settings.quickAddModal + " .modal-body #header").hide()

                    // $("#left_menu").hide();
                    $(settings.quickAddModal + " .modal-body #left_menu").hide()

                    // $("#main-content").toggleClass("collapse-main");
                    $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")

                    $("body").removeClass("sidebar-mini");
                })
            });

            $("body").on("click", ".js-modal-dismiss", function () {
                $("body").removeClass("sidebar-mini").addClass("sidebar-mini");
                $(settings.quickAddModal + " .modal-body #header").show()

                //  $("#left_menu").show();
                $(settings.quickAddModal + " .modal-body #left_menu").show()

                // $("#main-content").toggleClass("collapse-main");
                $(settings.quickAddModal + " .modal-body #main-content").toggleClass("collapse-main")


                lastCode = $(settings.quickAddModal + " #lastCode").val();

                $(settings.quickAddModal + " .modal-body").empty();
                $(settings.quickAddModal).modal("hide");

                $(target).empty("");
                $(target).append($('<option>', {
                    value: '',
                    text: `Select ${title}`
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

                initialize();
            });
        });


        function showImagePreview(input, target) {
            //var target = $(input).data("target");
            if (input[0].files && input[0].files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $(target).prop('src', e.target.result);
                };
                reader.readAsDataURL(input[0].files[0]);
            }
        }

        function clearImage(file, tag) {
            console.log(file);
            console.log(tag);
            $(file).removeAttr("src");
            $(tag).val(true);
        }

        function medicineDescription() {
            var medicineName = $("#MedicineName").val() ?? "";
            var medicineStrength = $("#Strength").val() ?? "";
            var description = medicineName;
            if (medicineStrength.length > 0)
                description += ` ${medicineStrength}`;

            $("#MedicineDescription").val(description);
        }

        function packingDetails() {
            var quantity = $("#Quantity").val() ?? "";
            var unit = $("#UnitId option:selected").text() ?? "";
            var packUnit = $("#PackUnitId option:selected").text() ?? "";
            var packSize = quantity;
            if (unit.length > 0)
                packSize += ` ${unit}`;

            if (packUnit.length > 0)
                packSize += `/${packUnit}`;

            $("#PackSize").val(packSize);
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
        }
    }
}(jQuery));

