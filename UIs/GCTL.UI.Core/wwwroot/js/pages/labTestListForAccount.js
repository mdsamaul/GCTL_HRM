(function ($) {
    $.labTestListForAccount = function (options) {
        // Default options
        var settings = $.extend({
            baseUrl: "/",
            formSelector: "#labTestListForAccount-form",
            formContainer: ".js-labTestListForAccount-form-container",
            gridSelector: "#patients-grid",
            gridContainer: ".js-labTestListForAccount-grid-container",
            editSelector: ".js-labTestListForAccount-edit",
            saveSelector: ".js-labTestListForAccount-save",
            selectAllSelector: "#labTestListForAccount-check-all",
            deleteSelector: ".js-labTestListForAccount-delete-confirm",
            deleteModal: "#labTestListForAccount-delete-modal",
            finalDeleteSelector: ".js-labTestListForAccount-delete",
            clearSelector: ".js-labTestListForAccount-clear",
            topSelector: ".js-go",
            decimalSelector: ".js-labTestListForAccount-decimalplaces",
            maxDecimalPlace: 5,
            showNagativeFormat: false,
            availabilitySelector: ".js-labTestListForAccount-check-availability",
            haseFile: false,
            quickAddSelector: ".js-quick-add",
            quickAddModal: "#quickAddModal",
            isAdmin: false,
            load: function () {

            }
        }, options);


        var gridUrl = settings.baseUrl + "/grid";
        var saveUrl = settings.baseUrl + "/setup";
        var linkedApprovedUrl = settings.baseUrl + "/ApprovedLinkedWithAccount";
        var selectedItems = [];
        $(() => {
           
            initialize();

           
            
            loadTestEntries(settings.baseUrl, "#labTestListForAccount-grid");
        

           


           

            $("body").on("click", settings.selectAllSelector, function () {
                $(".checkBox").prop('checked',
                    $(this).prop('checked'));
            });


            $("body").on("click", ".js-labTestListForAccount-Approved", function (e) {
                e.preventDefault();
                $('input:checkbox.checkBox').each(function () {
                    if ($(this).prop('checked')) {
                        if (!selectedItems.includes($(this).val())) {
                            selectedItems.push($(this).val());
                        }
                    }
                });

                $.ajax({
                    url: linkedApprovedUrl + "/" + selectedItems,
                    method: "POST",
                    success: function (response) {
                       
                            loadTestEntries(settings.baseUrl, "#labTestListForAccount-grid");
                          
                        
                    }
                });

              
            });


          
          
            $("body").on("click", settings.topSelector, function (e) {
                e.preventDefault();
                $("html, body").animate({ scrollTop: 0 }, 500);
            });

            $("body").on('click', '.select-item', function () {
                $('input[class="select-item"]').not(this).prop('checked', false);
                if ($(this).is(":checked")) {

                    let url = settings.baseUrl + $(this).data("url");
                    loadForm(url);

                    $(settings.saveSelector).removeAttr("disabled");
                    $("html, body").animate({ scrollTop: 500 }, 500);
                }
            });

            $("body").on("click", "input:checkbox.checkBox", function (e) {
                if ($(this).prop('checked')) {
                    if (!selectedItems.includes($(this).val())) {
                        selectedItems.push($(this).val());
                    }
                } else {
                    selectedItems.splice($.inArray($(this).val(), selectedItems), 1);;
                }
                console.log(selectedItems);
            });

            var counter = 0;


            $("body").on("click", ".js-labTestListForAccount-GridPreivew", function (e) {
               
                loadTestEntries(settings.baseUrl, "#labTestListForAccount-grid");
            });
           

           
        });

        

       

       


        function loadTestEntries(baseUrl, gridSelector) {
         
            var dataTable = $(gridSelector).DataTable({
                ajax: {
                    url: baseUrl + "/TestEntries",
                    type: "GET",
                    datatype: "json"
                },

                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                columns: [
                    {
                        "data": "labTestNo", "className": "text-center", width: "30px",
                        render: function (data) {
                            return `<input type="checkbox" class="checkBox" value="${data}" />`;
                        }
                    },
                    {
                        "data": "labTestNo", "className": "text-center", width: "130px"
                    },
                    { "data": "labTestDateTime", "className": "text-center", width: "130px" },
                    { "data": "mrNo", "className": "text-center", width: "80px" },
                    { "data": "totalAmount", "className": "text-right", width: "80px" },
                    { "data": "discount", "className": "text-right", width: "80px" },
                    { "data": "payable", "className": "text-right", width: "80px" },
                    { "data": "due", "className": "text-right", width: "80px" },
                    { "data": "patientCode", "className": "text-center", width: "80px" },
                    { "data": "patientName", "className": "text-left", width: "150px" },
                    { "data": "phone", "width": "50px" },
                   
                    {
                        "data": "doctorName", "className": "text-left", width: "130px"
                    },
                    { "data": "referencePerson", "className": "text-left", width: "130px" }
                
                   
                ],
                lengthChange: false,
                pageLength: 10,
                order: [],
                sScrollY: "100%",
                scrollX: true,
                sScrollX: "100%",
                bDestroy: true
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
            $('.selectpicker').select2({
                dropdownAutoWidth: true,
                language: {

                    noResults: function () {
                        //return 'Not found <a class="add_new_item" href="javascript:void(0)">Add New</a>';
                    }
                },
                escapeMarkup: function (markup) {
                    return markup;
                },
               // width: "100%"
            });

            // on first focus (bubbles up to document), open the menu
            $(document).on('focus', '.select2-selection.select2-selection--single', function (e) {
                $(this).closest(".select2-container").siblings('select:enabled').select2('open');
            });

            // steal focus during close - only capture once and stop propogation
            $('select.select2').on('select2:closing', function (e) {
                $(e.target).data("select2").$selection.one('focus focusin', function (e) {
                    e.stopPropagation();
                });
            });

         
            
        }
    }
}(jQuery));

