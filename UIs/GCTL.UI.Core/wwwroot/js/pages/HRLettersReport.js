(function ($) {
    $.HRLettersReport = function (options) {

        var settings = $.extend({
            baseUrl: "/",
            EmployeeSelect: "#employeeSelect",
            SCEmployeeSelect: "#sCEmployeeSelect",
            DateApplied: "#iDateApplied",
            load: function () { }
        }, options);

        var empDetailsUrl = settings.baseUrl + "/EmpDetails";
        var exportUrl = settings.baseUrl + "/Export";
        var EmployeesByLetterTypeUrl = settings.baseUrl + "/GetEmployeesByLetterType";
        var EMP_SELECTOR = '#employeeSelect';
        // ── Init ──────────────────────────────────────────────────────────
        $(document).ready(async function () {
            s2_InitSingle("#companySelect", "/GcFilters/company", "Select Company", "company");
            //s2_InitSingle("#employeeSelect", "/GcFilters/employee", "Select Employee", "employee");
            s2_InitSingle("#sCEmployeeSelect", "/GcFilters/employee", "Select Employee", "employee");

            $('[data-toggle="tooltip"]').tooltip();
            await s2_AutoSelectCompany("001");

            
                flatpickr(settings.DateApplied, {
                    dateFormat: "Y-m-d",
                    altInput: true,
                    altFormat: "d/m/Y",
                    allowInput: true,
                    onReady: function (selectedDates, dateStr, instance) {
                        instance.input.placeholder = "dd/mm/yyyy";
                    }
                });

                flatpickr(settings.DateApplied, CalendarService.createConfig(
                    {
                        defaultDate: new Date(),
                    }
                ));
        });

        // ── Employee select change ─────────────────────────────────────────
        $(document).on('change', settings.EmployeeSelect + ', ' + settings.SCEmployeeSelect, function () {
            const value = $(this).val();
            const id = this.id;
            const hrLettersId = $("#hrLetterTypeSelect").val();
            if (!value) { clearFields(id); return; }
            //if (id == 'employeeSelect' && hrLettersId == "005") {
            //    $(".terminationDate").fadeIn(100);
            //    $(".iNidRight").fadeIn(100);
            //    $(".iSrvleft").fadeIn(100);
            //    $(".iSrvRight").fadeOut(100);
            //    $(".iNidLeft").fadeOut(100);
            //} else {
            //    $(".terminationDate").fadeOut(100);
            //    $(".iSrvRight").fadeIn(100);
            //    $(".iNidRight").fadeOut(100);
            //    $(".iSrvleft").fadeOut(100);
            //    $(".iNidLeft").fadeIn(100);
            //}
        
          
            if (id == "employeeSelect" && (hrLettersId == "005" || hrLettersId == "018")) {

                $(".terminationDate").stop(true, true).slideDown(200).fadeTo(200, 1);
                $(".iNidRight").stop(true, true).slideDown(200);
                $(".iSrvleft").stop(true, true).slideDown(200);

                $(".iSrvRight").stop(true, true).slideUp(200);
                $(".iNidLeft").stop(true, true).slideUp(200);

            } else if (id != "sCEmployeeSelect"){

                $(".terminationDate").stop(true, true).slideUp(200);
                $(".iSrvRight").stop(true, true).slideDown(200);
                $(".iNidRight").stop(true, true).slideUp(200);
                $(".iSrvleft").stop(true, true).slideUp(200);
                $(".iNidLeft").stop(true, true).slideDown(200);
            }
            empDetails(value, id);
        });

        function setValue(selector, value) {
            const el = $(selector);
            el.is("input") ? el.val(value || "") : el.text(value || "-");
        }

        function clearFields(sourceId) {
            if (sourceId === "employeeSelect") {
                $("#iDept, #iJoin, #iNidValRight, #iNidValRight, #iName, #iDesig, #iSrvValRight, #iSrvValLeft, #showTerminationDate").text("");
            }
            if (sourceId === "sCEmployeeSelect") {
                $("#iDesigInput, #iMobile, #iTelephone")
                    .val("").prop('readonly', false).removeClass("bg-light");
            }
        }

        function empDetails(empId, sourceId) {
            var $empSelect = $(EMP_SELECTOR);
            $.ajax({
                url: empDetailsUrl,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(empId),
                success: function (res) {
                    console.log(res);
                    if (sourceId === "employeeSelect") {
                        setValue("#iDept", res.departmentName);
                        setValue("#iJoin", res.joiningDate);
                        setValue("#showTerminationDate", res.leavingDate);
                        setValue("#iNidValRight", res.nationalIDNO);
                        setValue("#iNidValLeft", res.nationalIDNO);
                        setValue("#iName", res.employeeName);
                        setValue("#iDesig", res.designationName);
                        setValue("#iSrvValRight", res.serviceLength);
                        setValue("#iSrvValLeft", res.serviceLength);
                        // _empAddCopyIcon($empSelect); 
                    }
                    if (sourceId === "sCEmployeeSelect") {
                        setValue("#iDesigInput", res.designationName);
                        setValue("#iMobile", res.officialPhone);
                        setValue("#iTelephone", res.telephone);
                        $("#iDesigInput, #iMobile, #iTelephone")
                            .prop('readonly', true).addClass("bg-light");
                    }
                },
                error: function () {
                    toastr.error("Failed to load employee details.");
                }
            });
        }

        // ── Collect form data ──────────────────────────────────────────────
        function collectRequest(isPreview) {
            return {
                employeeCode: $("#employeeSelect").val(),
                signatoryEmployeeCode: $("#sCEmployeeSelect").val(),
                hrLetterTypeId: $("#hrLetterTypeSelect").val(),
                designation: $("#iDesigInput").val(),
                mobile: $("#iMobile").val(),
                telephone: $("#iTelephone").val(),
                AppliedDate: $("#iDateApplied").val(),
                reportFormat: $("#reportText").val(),
                isPreview: isPreview
            };
        }

        function validate(req) {
            if (!req.employeeCode) {
                toastr.warning("Please select an Employee.");
                $("#employeeSelect").select2("open");
                return false;
            }
            if (!req.hrLetterTypeId) {
                toastr.warning("Please select HR Letter Type.");
                $("#hrLetterTypeSelect").select2("open");
                return false;
            }
            if (!req.signatoryEmployeeCode) {
                toastr.warning("Please select a Signatory Employee.");
                $("#sCEmployeeSelect").select2("open");
                return false;
            }
            if (!req.AppliedDate) {
                toastr.warning("Please enter Date Applied.");
                $("#iDateApplied")[0]._flatpickr.open();
                return false;
            }
            return true;
        }

        function sendRequest(req, fileName) {
            $.ajax({
                url: exportUrl,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(req),
                xhrFields: { responseType: "blob" },
                beforeSend: function () {
                    $("#btnPreviewPdf, #downloadReport").prop("disabled", true);
                },
                success: function (blob) {
                    const url = URL.createObjectURL(new Blob([blob], { type: "application/pdf" }));

                    if (req.isPreview) {
                        // New tab PDF open
                        window.open(url, "_blank");
                    } else {
                        // Download
                        const link = document.createElement("a");
                        link.href = url;
                        link.download = fileName;
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                    }
                    URL.revokeObjectURL(url);
                },
                error: function (xhr) {
                    let msg = "Operation failed.";
                    if (xhr.responseJSON?.message) msg = xhr.responseJSON.message;
                    toastr.error(msg);
                },
                complete: function () {
                    $("#btnPreviewPdf, #downloadReport").prop("disabled", false);
                }
            });
        }

        // ── Preview button ─────────────────────────────────────────────────
        $(document).on("click", "#btnPreviewPdf", function () {
            const req = collectRequest(true);
            if (!validate(req)) return;
            sendRequest(req, null);
        });

        // ── Download button ────────────────────────────────────────────────
       

        $(document).on("click", "#downloadReport", function () {
            const req = collectRequest(false);
            if (!validate(req)) return;

            const letterTypeNames = {
                "005": "Termination_Letter",
                "010": "NOC_Travel",
                "019": "Internship_Offer_Letter",
                "014": "Internship_Certificate",
                "018": "Recommendation_Letter",
                "016": "NOC_Education",
                "017": "NOC_General"
            };

            const letterTypeName = letterTypeNames[req.hrLetterTypeId] ?? "Discharge_Certificate";
            const date = new Date().toISOString().slice(0, 10).replace(/-/g, "");
            const fileName = `${letterTypeName}_${req.employeeCode}_${date}.pdf`;

            sendRequest(req, fileName);
        });

        function _empAddCopyIcon($sel) {
            
            $('.emp-selected-copy').remove();
            $(EMP_SELECTOR).next('.select2-container').find('.emp-selected-copy').remove();

            var selectedText = $sel.val() || '';
            if (!selectedText) return;

            var $container = $sel.next('.select2-container');
            if (!$container.length) return;

            var $icon = $(
                '<span class="emp-selected-copy" title="Copy ID" ' +
                'style="position:absolute;right:38px;top:50%;transform:translateY(-50%);' +
                'cursor:pointer;color:#888;font-size:11px;z-index:10;padding:2px 5px;' +
                'background:#f0f0f0;border-radius:3px;line-height:1;">' +
                '<i class="fa fa-copy"></i>' +
                '</span>'
            );

            $container.css('position', 'relative');

            $icon.on('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                var currentVal = $(EMP_SELECTOR).val() || '';
                _empCopyToClipboard(currentVal);
            });

            $container.append($icon);
        }
        // copy icon
        function _empRemoveCopyIcon() {
            $(EMP_SELECTOR).next('.select2-container').find('.emp-selected-copy').remove();
        }

        function _empCopyToClipboard(text) {
            
            if (!text) return;
            toastr.options = { positionClass: "toast-bottom-right" };
            try {
                var $temp = $('<input>');
                $('body').append($temp);
                $temp.val(text).select();
                document.execCommand('copy');
                $temp.remove();
                toastr.success('ID copied: ' + text);
            } catch (e) {
                toastr.error('Copy failed.');
            }
        }
        $(document).on('change', "#hrLetterTypeSelect", function () {
          
            var letterTypeId = $(this).val();
            var $employeeSelect = $('#employeeSelect');

            // reset
            $employeeSelect.empty().append('<option value=""></option>');

            if (!letterTypeId) return;

            $.ajax({
                url: EmployeesByLetterTypeUrl,
                type: 'GET',
                data: { letterTypeId: letterTypeId },
                success: function (data) {
                    $.each(data, function (i, item) {
                        $employeeSelect.append(
                            $('<option>', {
                                value: item.employeeId,
                                text: item.employeeName
                            })
                        );
                    });

                    // Select2 refresh
                    $employeeSelect.trigger('change');
                },
                error: function () {
                    toastr.error('Failed to load employees.');
                }
            });
        });


    };
}(jQuery));