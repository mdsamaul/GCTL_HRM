(function ($) {
    $.SalesSupplier = function (options) {
        var commonName = $.extend({
            baseUrl: "/",
            ShortName: "#ShortName",
            SupplierName: "#SupplierName",
            SupplierID: "#SupplierID",
            AutoId: "#AutoId",
            SupplierAddress: "#SupplierAddress",
            DropdownSupplierType: ".dropdownSupplierType",
            DropdownSupplierCountry: ".dropdownSupplierCountry",
            Phone: "#Phone",
            Email: "#Email",
            FAX: "#FAX",
            URL: "#URL",
            BinNo: "#BinNo",
            Tin: "#Tin",
            VatRegNo: "#VatRegNo",
            ContatPerson1: "#ContatPerson1",
            Designation1: "#Designation1",
            Phone1: "#Phone1",
            Email1: "#Email1",
            ContatPerson2: "#ContatPerson2",
            Designation2: "#Designation2",
            Email2: "#Email2",
            Phone2: "#Phone2",
            Remarks: "#Remarks",
            RowCheckbox: ".row-checkbox",
            SelectedAll: "#selectAll",
            EditBtn: ".supplier-btn-edit",
            SupplierTypeSaveBtn: ".js-SupplierType-save",
            DeleteBtn: "#js-inv-SupplierType-delete-confirm",
            UpdateDate: ".updateDate",
            CreateDate: ".createDate",
            ClearBrn: "#js-SupplierType-clear",
            SupplierTypeBtn: ".supplierTypeBtn",
            SupplierTypeCountryBtn:".supplierTypeCountryBtn",
            SupplierTypeContainer: "#supplierTypeContainer",
            SupplierCountryContainer: "#supplierCountryContainer",
            SupplierCountryContainer: "#supplierCountryContainer",
            ClosesupplierTypeModel: ".closesupplierTypeModel",

        }, options);

        var loadSalesSupplierDataUrl = commonName.baseUrl + "/LoadData";
        var autoSupplierTypeIdUrl = commonName.baseUrl + "/AutoSupplierTypeId";
        var CreateUpdateUrl = commonName.baseUrl + "/CreateUpdate";
        var PopulatedDataForUpdateUrl = commonName.baseUrl + "/PopulatedDataForUpdate";
        var deleteUrl = commonName.baseUrl + "/deleteSupplierType";
        var alreadyExistUrl = commonName.baseUrl + "/alreadyExist";

        var supplierTypeUrl = "/InvDefSupplierType/Index?isPartial=true";
        var supplierCountryUrl = "/Core_Country/Index?isPartial=true";
        var CloseSupplierTypeModelUrl = commonName.baseUrl + "/CloseSupplierType";
        var CloseCountryModelUrl = commonName.baseUrl + "/CloseCountryModel";

        // Sticky header on scroll
        function stHeader() {
            window.addEventListener('scroll', function () {
                const header = document.getElementById('stickyHeader');
                if (window.scrollY > 10) {
                    header.classList.add('scrolled');
                } else {
                    header.classList.remove('scrolled');
                }
            });
        }

        // SweetAlert toast message
        function showToast(iconType, message) {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 5000,
                timerProgressBar: true,
                showClass: {
                    popup: 'swal2-show swal2-fade-in'
                },
                hideClass: {
                    popup: 'swal2-hide swal2-fade-out'
                }
            });

            Toast.fire({
                icon: iconType,
                title: message
            });
        }
       
        autoSupplierTypeId = function () {
            $.ajax({
                url: autoSupplierTypeIdUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.SupplierID).val(res.data);
                },
                error: function (e) {
                }
            });
        }

        resetFrom = function () {
            $(commonName.AutoId).val(0);
            $(commonName.SupplierID).val('');
            $(commonName.SupplierName).val('');
            $(commonName.ShortName).val('');
            $(commonName.SupplierAddress).val('');
            $(commonName.Phone).val('');
            $(commonName.Email).val('');
            $(commonName.FAX).val('');
            $(commonName.URL).val('');
            $(commonName.BinNo).val('');
            $(commonName.Tin).val('');
            $(commonName.VatRegNo).val('');
            $(commonName.ContatPerson1).val('');
            $(commonName.Designation1).val('');
            $(commonName.Phone1).val('');
            $(commonName.Email1).val('');
            $(commonName.ContatPerson2).val('');
            $(commonName.Designation2).val('');
            $(commonName.Phone2).val('');
            $(commonName.Email2).val('');
            $(commonName.Remarks).val('');

            // Dropdown reset
            $(commonName.DropdownSupplierType).val('').trigger('change');
            $(commonName.DropdownSupplierCountry).val('').trigger('change');

            
        }

        $(commonName.ClearBrn).on('click', function () {
            resetFrom();
            autoSupplierTypeId();
        })
        // get data from input       

        getFromData = function () {
            var fromData = {
                AutoId: $(commonName.AutoId).val(),
                SupplierID: $(commonName.SupplierID).val(),
                SupplierName: $(commonName.SupplierName).val(),
                SupplierAddress: $(commonName.SupplierAddress).val(),
                Phone: $(commonName.Phone).val(),
                Email: $(commonName.Email).val(),
                FAX: $(commonName.FAX).val(),
                URL: $(commonName.URL).val(),
                BinNo: $(commonName.BinNo).val(),
                Tin: $(commonName.Tin).val(),
                VatRegNo: $(commonName.VatRegNo).val(),
                ContatPerson1: $(commonName.ContatPerson1).val(),
                Designation1: $(commonName.Designation1).val(),
                Phone1: $(commonName.Phone1).val(),
                Email1: $(commonName.Email1).val(),
                ContatPerson2: $(commonName.ContatPerson2).val(),
                Designation2: $(commonName.Designation2).val(),
                Phone2: $(commonName.Phone2).val(),
                Email2: $(commonName.Email2).val(),
                Remarks: $(commonName.Remarks).val(), 

                // Dropdown values
                SupplierTypeId: $(commonName.DropdownSupplierType).val(),
                CountryId: $(commonName.DropdownSupplierCountry).val(),
            };

            return fromData;
        }

        //exists 
        $(commonName.SupplierName).on('input', function () {

            let SupplierTypeValue = $(this).val();
            $.ajax({
                url: alreadyExistUrl,
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(SupplierTypeValue),
                success: function (res) {
                    if (res.isSuccess) {
                        showToast('warning', res.message);
                        $(commonName.SupplierName).addClass('SupplierType-input');
                        $(commonName.SupplierTypeSaveBtn).prop('disabled', true);
                    } else {
                        $(commonName.SupplierName).removeClass('SupplierType-input');
                        $(commonName.SupplierTypeSaveBtn).prop('disabled', false);
                        $(commonName.SupplierTypeSaveBtn).css('border', 'none');

                    }
                }, error: function (e) {
                }
            });
        })
        $(commonName.SupplierAddress).on('input', function () {
            let SupplierTypeValue = $(this).val();
            if (SupplierTypeValue == null || SupplierTypeValue.trim() === '') {
                showToast('warning', "Address Requird");
                $(commonName.SupplierAddress).addClass('SupplierType-input');
                $(commonName.SupplierTypeSaveBtn).prop('disabled', true);
                return
            } else {
                $(commonName.SupplierAddress).removeClass('SupplierType-input');
                $(commonName.SupplierTypeSaveBtn).prop('disabled', false);
                $(commonName.SupplierTypeSaveBtn).css('border', 'none');
                return
            }
           
        })
        //create and edit
        // Save Button Click
        $(document).on('click', commonName.SupplierTypeSaveBtn, function () {
            var $btn = $(commonName.SupplierTypeSaveBtn);
            if ($btn.data('submitting')) return;
            $btn.data('submitting', true);
            var fromData = getFromData();
            if (fromData.SupplierName == null || fromData.SupplierName.trim() === '') {
                showToast('warning', "Supplier Requird");
                $(commonName.SupplierName).addClass('SupplierType-input');
                $(commonName.SupplierTypeSaveBtn).prop('disabled', true);
                $(commonName.SupplierName).focus();
                return;
            }

            if (fromData.SupplierAddress == null || fromData.SupplierAddress.trim() === '') {
                showToast('warning', "Address Requird");
                $(commonName.SupplierAddress).addClass('SupplierType-input');
                $(commonName.SupplierTypeSaveBtn).prop('disabled', true);
                $(commonName.SupplierAddress).focus();
                return;
            }

            $btn.prop('disabled', true);

            $.ajax({
                url: CreateUpdateUrl,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(fromData),
                success: function (res) {
                    if (res.isSuccess) {
                        $(document).trigger('data:changed', ['supplier']);
                        showToast("success", res.message);
                        loadSalesSupplierData();
                        resetFrom();
                        autoSupplierTypeId();
                    } else {
                        showToast("error", res.message);
                    }
                },
                error: function (e) {
                    showToast("error", res.message);
                },
                complete: function () {
                    $btn.prop('disabled', false);
                    $btn.data('submitting', false);
                 
                }
            });
        });

        // Reload DataTable Function
        function loadSalesSupplierData() {
            table.ajax.reload(null, false);
        }

        var table = $('#supplierTypeTable').DataTable({

            "autoWidth": true,
            "ajax": {
                "url": loadSalesSupplierDataUrl, 
                "type": "GET",
                "datatype": "json",
                "dataSrc": function (json) {
                    return json.data || [];
                },
                "error": function (xhr, error, thrown) {
                    showToast("error", "Data loading failed: " + xhr.statusText);
                }
            },
            "columns": [
                {
                    "data": "autoId",
                    "render": function (data) {
                        return `<input type="checkbox" class="row-checkbox" value="${data}" />`;
                    },
                    "orderable": false
                },
                {
                    "data": "supplierID",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-link supplier-btn-edit" data-id="${data}">${data}</button>`;
                    }
                },
                {"data": "supplierType" },
                { "data": "supplierName" },
                { "data": "supplierAddress" },
                { "data": "contactPerson" }
            ],
            "paging": true,
            "pagingType": "full_numbers",
            "searching": true,
            "ordering": true,
            "responsive": true,
            "language": {
                "search": "Search...",
                "lengthMenu": "Show _MENU_ entries per page",
                "zeroRecords": "No data found",
                "info": "Showing _START_ to _END_ of _TOTAL_ entries",
                "paginate": {
                    "first": "First",
                    "last": "Last",
                    "next": "Next",
                    "previous": "Previous"
                }
            }
        });

        let selectedIds = [];
        //edit       

        $(document).on('click', commonName.EditBtn, function () {
            let id = $(this).data('id');
            $.ajax({
                url: `${PopulatedDataForUpdateUrl}?id=${id}`,
                type: "GET",
                success: function (res) {
                    selectedIds = [];
                    selectedIds.push(res.result.autoId + '');

                    // Basic Info
                    $(commonName.AutoId).val(res.result.autoId);
                    $(commonName.SupplierID).val(res.result.supplierID);
                    $(commonName.SupplierName).val(res.result.supplierName);
                    $(commonName.ShortName).val(res.result.shortName); 
                    $(commonName.SupplierAddress).val(res.result.supplierAddress);
                    $(commonName.Phone).val(res.result.phone);
                    $(commonName.Email).val(res.result.email);
                    $(commonName.FAX).val(res.result.fax);
                    $(commonName.URL).val(res.result.url);

                    // Business Info
                    $(commonName.BinNo).val(res.result.binNo);
                    $(commonName.Tin).val(res.result.tin);
                    $(commonName.VatRegNo).val(res.result.vatRegNo);
                    $(commonName.Remarks).val(res.result.remarks);

                    // Contact Person 1
                    $(commonName.ContatPerson1).val(res.result.contatPerson1);
                    $(commonName.Designation1).val(res.result.designation1);
                    $(commonName.Phone1).val(res.result.phone1);
                    $(commonName.Email1).val(res.result.email1);

                    // Contact Person 2
                    $(commonName.ContatPerson2).val(res.result.contatPerson2);
                    $(commonName.Designation2).val(res.result.designation2);
                    $(commonName.Phone2).val(res.result.phone2);
                    $(commonName.Email2).val(res.result.email2);

                    // Dropdowns (trigger change if using select2/searchable)
                    $(commonName.DropdownSupplierType).val(res.result.supplierTypeId).trigger('change');
                    $(commonName.DropdownSupplierCountry).val(res.result.countryId).trigger('change');

                    // Date Display
                    $(commonName.CreateDate).text(res.result.showCreateDate || '');
                    $(commonName.UpdateDate).text(res.result.showModifyDate || '');
                },
                error: function (e) {
                  
                },
                complete: function () {
                    
                }
            });
        });


        //selected id        

        $(document).on('change', commonName.RowCheckbox, function () {
            const id = $(this).val();
            if ($(this).is(':checked')) {
                if (!selectedIds.includes(id)) {
                    selectedIds.push(id);
                }
            } else {
                selectedIds = selectedIds.filter(item => item != id);
            }

            let totalCheckboxes = $(commonName.RowCheckbox).length;
            let totalChecked = $(commonName.RowCheckbox + ":checked").length;

            $('#selectAll').prop('checked', totalChecked === totalCheckboxes);
        })
        //select all
        $(document).on('change', commonName.SelectedAll, function () {
            const isChecked = $(this).is(':checked');
            $(commonName.RowCheckbox).prop('checked', isChecked).trigger('change');
        })
        $(document).on('click', commonName.DeleteBtn, function () {
            $.ajax({
                url: deleteUrl,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(selectedIds),
                success: function (res) {
                    showToast(res.isSuccess ? "success" : "error", res.message)
                },
                error: function (e) {
                }, complete: function () {
                    resetFrom();
                    autoSupplierTypeId();
                    loadSalesSupplierData();
                    $('#selectAll').prop('checked', false);
                    selectedIds = [];
                }
            })
        })

        $(document).ready(function () {
            if ($('#supplierTypeModal').length === 0) {
                const modalHtml = `
        <div class="modal fade" id="supplierTypeModal" tabindex="-1" aria-labelledby="supplierTypeModalLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
    <div class="modal-dialog modal-dialog-centered custom-salse-supp-modal modal-xl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="supplierTypeModalLabel">Supplier Type</h5>
                <button type="button" class="btn-close closesupplierTypeModel" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div id="supplierTypeContainer"></div>
            </div>
        </div>
    </div>
</div>
        `;
                $('body').append(modalHtml);
            }
        });


        $(document).ready(function () {
            if ($('#supplierTypeCountryModal').length === 0) {
                const modalHtml = `
        <div class="modal fade" id="supplierTypeCountryModal" tabindex="-1" aria-labelledby="supplierTypeModalCountryLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
    <div class="modal-dialog modal-dialog-centered custom-salse-supp-modal modal-xl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Country</h5>
                <button type="button" class="btn-close closeCountryModel" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div id="supplierCountryContainer">
                </div>
            </div>
        </div>
    </div>
</div>
        `;
                $('body').append(modalHtml);
            }
        });


        $(commonName.SupplierTypeBtn).on('click', function () {
            $.ajax({
                url: supplierTypeUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.SupplierTypeContainer).html(res);
                    //$('#brandContainer').html(result);
                    $("#supplierTypeModal").modal('show');
                    if (typeof $.InvDefSupplierType == 'function') {
                        $.InvDefSupplierType({
                            baseUrl: '/InvDefSupplierType',
                            isPartial: true
                        });
                    }                  
                }, error: function (e) {
                }
            });
        })
        $(commonName.SupplierTypeCountryBtn).on('click', function () {
          
            $.ajax({
                url: supplierCountryUrl,
                type: "GET",
                success: function (res) {
                    $(commonName.SupplierCountryContainer).html(res);
                    //$('#brandContainer').html(result);
                    $("#supplierTypeCountryModal").modal('show');
                    if (typeof $.Core_CountryJs == 'function') {
                        $.Core_CountryJs({
                            baseUrl: '/Core_Country',
                            isPartial: true
                        });
                    }                  
                }, error: function (e) {
                }
            });
        })
        $(document).on('click', '.closesupplierTypeModel', function () {       
            $('.modal').modal('hide');
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open');  
            $.ajax({
                url: CloseSupplierTypeModelUrl,
                type: "GET",
                success: function (res) {
                    //$(commonName.DropdownSupplierType).empty();
                    res.forEach(function (item) {
                        $(commonName.DropdownSupplierType).append(
                            $('<option></option>').val(item.supplierTypeId).text(item.supplierType)
                        );
                    });
                },
                error: function (e) {
                }
            });
        });
        $(document).on('click', '.closeCountryModel', function () {       
            $('.modal').modal('hide');
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open');              
        });


        window.SalesSupplierModuleLoaded = true;
        // Initialize all functions
        var init = function () {
            stHeader();
            autoSupplierTypeId();
            table;
        };
        init();

    };
})(jQuery);
