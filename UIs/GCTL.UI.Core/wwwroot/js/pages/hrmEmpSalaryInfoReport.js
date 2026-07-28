(function ($) {
    $.empSalaryInfoReport = function (options) {
        const _originalBuildReq = buildReq; // keep reference to original

        buildReq = function (page, search) {
            const base = _originalBuildReq(page, search);

            // Extend with page-specific params
            return Object.assign(base, {
                EmpStatuses: arrVal("#empStatusSelect"),
                JoiningDateFrom: $("#joiningDateFromSelect").val() || null,
                JoiningDateTO: $("#joiningDateToSelect").val() || null,
                EmpNatures: arrVal("#empNatureSelect"),
                EmpTypes: arrVal("#empTypeSelect")
            });
        };

        const settings = $.extend({
            baseUrl: "/",
            companyIds: "#companySelect",
            branchIds: "#branchSelect",
            departmentIds: "#departmentSelect",
            designationIds: "#designationSelect",
            employeeIds: "#employeeSelect",

            empType: "#empTypeSelect",
            empNature: "#empNatureSelect",
            empStatus: "#empStatusSelect",

            joiningDateFrom: "#joiningDateFromSelect",
            joiningDateTo: "#joiningDateToSelect",

            previewContainer: "#salaryInfoReport-container .card-body",
            previewContainerBody: "#salaryInfoReport-container",
            iFrame: "#previewIframe",
            load: () => console.log("Loading...")
        }, options);

        const urls = {
            filter: `${settings.baseUrl}/getAllFilterEmp`,
            export: `${settings.baseUrl}/ExportReport`,
            preview: `${settings.baseUrl}/PreviewReport`
        };

        let isInitialLoad = true;
        let filterChangeBound = false;
        let flatpickrInstances = {};

        const toArray = val => val ? (Array.isArray(val) ? val : [val]) : [];
        const showLoading = () => $("#customLoadingOverlay").css("display", "flex");
        const hideLoading = () => $("#customLoadingOverlay").hide();

        const showToast = (type, message) => {
            if (typeof toastr !== 'undefined') {
                toastr[type](message);
            } else {
                alert(message);
            }
        };

        const setupLoadingOverlay = () => {
            if ($("#customLoadingOverlay").length === 0) {
                $("body").append(`
                    <div id="customLoadingOverlay" style="display:none;position:fixed;top:0;left:0;width:100%;height:100%;background:rgba(0,0,0,0.5);z-index:9999;justify-content:center;align-items:center;">
                        <div style="background:white;padding:20px;border-radius:5px;box-shadow:0 0 10px rgba(0,0,0,0.3);text-align:center;">
                            <div class="spinner-border text-primary"></div>
                        </div>
                    </div>
                `);
            }
        };

        const initializeFlatpickr = () => {
            if (typeof flatpickr === 'undefined')
                return;

            const fromPicker = flatpickr(settings.joiningDateFrom, CalendarService.createConfig(
                {
                    onChange: function (selectedDates) {
                        if (selectedDates.length) {
                            toPicker.set('minDate', selectedDates[0]);
                        }
                    }
                }));

            const toPicker = flatpickr(settings.joiningDateTo, CalendarService.createConfig(
                {
                    onChange: function (selectedDates) {
                        if (selectedDates.length) {
                            fromPicker.set('maxDate', selectedDates[0]);
                        }
                    }
                }));
        }

        const getFilterValue = () => ({
            CompanyCodes: toArray($(settings.companyIds).val()),
            BranchCodes: toArray($(settings.branchIds).val()),
            DepartmentCodes: toArray($(settings.departmentIds).val()),
            DesignationCodes: toArray($(settings.designationIds).val()),
            EmployeeIDs: toArray($(settings.employeeIds).val()),
            EmpNatures: toArray($(settings.empNature).val()),
            EmpStatuses: toArray($(settings.empStatus).val()),
            EmpTypes: toArray($(settings.empType).val()),

            JoiningDateFrom: $(settings.joiningDateFrom).val() || null,
            JoiningDateTo: $(settings.joiningDateTo).val() || null
        });

        const populateSelect = (selector, dataList) => {
            const $select = $(selector);

            dataList.forEach(item => {
                if (item.code && item.name && $select.find(`option[value="${item.code}"]`).length === 0) {
                    $select.append(`<option value="${item.code}">${item.name}</option>`);
                }
            });
            $select.multiselect('rebuild');

            if (selector === settings.empStatus && isInitialLoad)
                selectActiveStatus(dataList);
        }

        const selectActiveStatus = (empStatuses) => {
            const activeStatus = empStatuses.find(status =>
                status.code === '01' ||
                status.name.toLowerCase() === 'active'
            );

            if (activeStatus) {
                $(settings.empStatus).multiselect('select', activeStatus.code, false);
            }
        }

        const clearDependentDropdowns = selectors => {
            selectors.forEach(selector => {
                $(selector).empty().multiselect('rebuild');
            });
        };

        const setupCascadingClearEvents = () => {
            const clearMap = {
                [settings.companyIds]: [settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds],
                [settings.branchIds]: [settings.departmentIds, settings.designationIds, settings.employeeIds],
                [settings.departmentIds]: [settings.designationIds, settings.employeeIds],
                [settings.designationIds]: [settings.employeeIds],
                [settings.empNature]: [settings.departmentIds, settings.designationIds, settings.employeeIds],
                [settings.empType]: [settings.departmentIds, settings.designationIds, settings.employeeIds],
                [settings.empStatus]: [settings.departmentIds, settings.designationIds, settings.employeeIds],
            };

            Object.entries(clearMap).forEach(([parent, children]) => {
                $(parent).off("change").on("change", function () {
                    clearDependentDropdowns(children);
                });
            });
        };

        const bindFilterChangeEvents = () => {
            if (!filterChangeBound) {
                const filterSelectors = [
                    settings.companyIds,
                    settings.branchIds,
                    settings.empType,
                    settings.empNature,
                    settings.empStatus
                ].join(',');

                $(filterSelectors).on("change.loadFilter", function () {
                    loadFilterEmp();
                });

                $(settings.joiningDateFrom).on('input.loadFilter', function () {
                    clearTimeout(window.yearInputTimeout);
                    window.yearInputTimeout = setTimeout(() => {
                        clearDependentDropdowns([
                            settings.departmentIds,
                            settings.designationIds,
                            settings.employeeIds,
                        ]);
                        loadFilterEmp();
                    }, 300);
                });

                $(settings.joiningDateTo).on('input.loadFilter', function () {
                    clearTimeout(window.yearInputTimeout);
                    window.yearInputTimeout = setTimeout(() => {
                        clearDependentDropdowns([
                            settings.departmentIds,
                            settings.designationIds,
                            settings.employeeIds,
                        ]);
                        loadFilterEmp();
                    }, 300);
                });

                filterChangeBound = true;
            }
        };

        const loadFilterEmp = () => {
            $.ajax({
                url: urls.filter,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(getFilterValue()),
                success: res => {
                    if (!res.isSuccess) {
                        showToast('error', res.message);
                        return;
                    }

                    const { data } = res;
                    console.log(data);

                    if (data.empNatures?.length)
                        populateSelect(settings.empNature, data.empNatures);

                    if (data.empStatuses?.length)
                        populateSelect(settings.empStatus, data.empStatuses);

                    if (data.empTypes?.length)
                        populateSelect(settings.empType, data.empTypes);

                    bindFilterChangeEvents();

                    isInitialLoad = false;
                },
                error: () => {
                    showToast('error', 'Failed to load data');
                }
            });
        };

        const showPreview = () => {
            const filterData = getFilterValue();
            const hasFilters = Object.values(filterData).some(value =>
                (Array.isArray(value) && value.length > 0) ||
                (typeof value === 'string' && value.trim() !== '')
            );

            if (!hasFilters) {
                showToast('warning', 'No working day is found to generate report');
                return;
            }

            $(settings.previewContainer).show();
            showLoading();
            $(settings.iFrame).attr('src', '');

            $.ajax({
                url: urls.preview,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ FilterData: filterData, ExportFormat: 'pdf' }),
                xhrFields: { responseType: 'blob' },
                success: data => {
                    hideLoading();
                    if (window.currentPdfUrl) {
                        window.URL.revokeObjectURL(window.currentPdfUrl);
                    }

                    const blob = new Blob([data], { type: 'application/pdf' });
                    window.currentPdfUrl = window.URL.createObjectURL(blob);
                    $(settings.iFrame).attr('src', window.currentPdfUrl);
                    $(settings.iFrame).show();
                },
                error: xhr => {
                    hideLoading();
                    $(settings.previewContainer).hide();
                    let errorMessage = 'Preview failed. Please try again.';

                    switch (xhr.status) {
                        case 400:
                            errorMessage = 'Invalid request. Please check your inputs.';
                            break;
                        case 404:
                            errorMessage = 'No data found for the selected criteria.';
                            break;
                        default:
                            errorMessage = 'An unexpected error occurred. Please try again later.';
                            break;
                    }
                    showToast('error', errorMessage);
                }
            });
        };

        const hidePreview = () => {
            $(settings.previewContainer).hide();
            $(settings.iFrame).attr('src', '');
            if (window.currentPdfUrl) {
                window.URL.revokeObjectURL(window.currentPdfUrl);
                window.currentPdfUrl = null;
            }
        };

        const exportReport = format => {
            showLoading();

            $.ajax({
                url: urls.export,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ FilterData: getFilterValue(), ExportFormat: format }),
                xhrFields: { responseType: 'blob' },
                success: (data, status, xhr) => {
                    hideLoading();

                    const contentType = xhr.getResponseHeader('Content-Type');
                    const disposition = xhr.getResponseHeader('Content-Disposition');

                    let filename;
                    if (disposition && disposition.indexOf("filename=") != -1) {
                        const match = disposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
                        if (match && match[1]) {
                            filename = match[1].replace(/['"]/g, '');
                        }
                    }

                    if (format === "download") {
                        const blob = new Blob([data], { type: 'application/pdf' });
                        const iframe = Object.assign(document.createElement('iframe'), {
                            style: 'display:none',
                            src: window.URL.createObjectURL(blob),
                            onload: function () {
                                this.contentWindow.focus();
                                this.contentWindow.print();
                            }
                        });
                        document.body.appendChild(iframe);
                        return;
                    }
                    const blob = new Blob([data], { type: contentType });
                    const link = Object.assign(document.createElement("a"), {
                        href: window.URL.createObjectURL(blob),
                        download: filename
                    });

                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                    window.URL.revokeObjectURL(link.href);
                    clearAllFilters();
                    showToast('success', 'Report exported successfully');
                },
                error: (e) => {
                    hideLoading();
                    let errorMessage = 'Export failed. Please try again.';

                    switch (e.status) {
                        case 400:
                            errorMessage = 'Invalid request. Please check your inputs.';
                            break;
                        case 404:
                            errorMessage = 'No data found for the selected criteria.';
                            break;
                        default:
                            errorMessage = 'An unexpected error occurred. Please try again later.';
                            break;
                    }
                    showToast('error', errorMessage);
                }
            });
        };

        const clearAllFilters = () => {
            const filterSelectors = [
                settings.companyIds,
                settings.branchIds,
                settings.departmentIds,
                settings.designationIds,
                settings.employeeIds,
                settings.empType,
                settings.empNature,
                settings.empStatus
            ];

            filterSelectors.forEach(selector => {
                $(selector).multiselect('deselectAll', false);
                $(selector).multiselect('updateButtonText');
            });

            if (flatpickrInstances.joiningDateFrom) {
                flatpickrInstances.joiningDateFrom.clear();
            }
            if (flatpickrInstances.joiningDateTo) {
                flatpickrInstances.joiningDateTo.clear();
            }

            hidePreview();

            isInitialLoad = true;

            loadFilterEmp();
        };

        const bindUIEvents = () => {
            $("#previewBtn").on("click", () =>
                $(settings.previewContainer).is(':visible') ? hidePreview() : showPreview()
            );
            $("#downloadBtn").on("click", () => exportReport($("#exportFormatSelect").val()));
            $("#closePreviewBtn").on("click", hidePreview);
            $("#refreshPreviewBtn").on("click", showPreview);
        };

        const init = () => {
            settings.load();
            initializeFlatpickr();
            setupLoadingOverlay();
            bindUIEvents();
            loadFilterEmp();
        };

        $(settings.previewContainer).hide();
        init();

        // ================================================================
        // NEW: ajaxPrefilter — patches outgoing /GcAccessFilter/* requests
        // to include fields the frozen gc-each-multiselect.js never sends
        // (EmployeeNatureCodes, EmployeeTypes, JoiningDate range, and a
        // fallback for EmployeeStatuses). Safe: only touches requests to
        // that endpoint family, does not modify gc-each-multiselect.js.
        // ================================================================
        $.ajaxPrefilter(function (opts) {
            if (!opts.url || opts.url.indexOf('/GcAccessFilter/') === -1) return;
            if (typeof opts.data !== 'string') return;

            let payload;
            try { payload = JSON.parse(opts.data); } catch (e) { return; }

            payload.EmployeeNatureCodes = toArray($(settings.empNature).val());
            payload.EmployeeTypes = toArray($(settings.empType).val());
            payload.EmployeeStatuses = (payload.EmployeeStatuses && payload.EmployeeStatuses.length)
                ? payload.EmployeeStatuses
                : toArray($(settings.empStatus).val());

            const jf = $(settings.joiningDateFrom).val();
            const jt = $(settings.joiningDateTo).val();
            if (jf) payload.JoiningDateFrom = jf;
            if (jt) payload.JoiningDateTo = jt;

            opts.data = JSON.stringify(payload);
        });

        // ================================================================
        // NEW: page-local replacement for ms_ApplyAccessCodeToAll.
        // Sequential (not parallel) so each dropdown's injected value is
        // available to the next request's buildRequestPayload snapshot.
        // Reuses ms_InjectSingleItem (untouched) from gc-each-multiselect.js.
        // ================================================================
        async function pageApplyAccessCodeToAll(accessCode) {
            if (!accessCode) return;

            const targets = [
                { selector: "#companySelect", url: "/GcAccessFilter/companies" },
                { selector: "#branchSelect", url: "/GcAccessFilter/branches" },
                { selector: "#divisionSelect", url: "/GcAccessFilter/divisions" },
                { selector: "#departmentSelect", url: "/GcAccessFilter/departments" },
                { selector: "#designationSelect", url: "/GcAccessFilter/designations" },
                { selector: "#employeeSelect", url: "/GcAccessFilter/employees" },
            ];

            for (const { selector, url } of targets) {
                if (!$(selector).length) continue;

                const req = buildRequestPayload(1, "");
                req.PageSize = 1;
                req.AccessCode = accessCode;
                req.EmployeeNatureCodes = toArray($(settings.empNature).val());
                req.EmployeeTypes = toArray($(settings.empType).val());

                try {
                    const res = await $.ajax({
                        url, type: "POST", contentType: "application/json",
                        data: JSON.stringify(req)
                    });
                    if (!res || !res.isSuccess) continue;
                    const items = res.data.items || res.data.Items || [];
                    if (!items.length) continue;
                    ms_InjectSingleItem(selector, items[0]);
                } catch (err) {
                    console.error(`[PAGE ACCESS CODE] ${selector} → ${url}:`, err);
                }
            }
        }

        $(document).ready(async function () {

            bindRemoteMultiselect("#companySelect", "/GcAccessFilter/companies", "Select Company", "company");
            bindRemoteMultiselect("#branchSelect", "/GcAccessFilter/branches", "Select Branch", "branch");
            bindRemoteMultiselect("#departmentSelect", "/GcAccessFilter/departments", "Select Department", "department");
            bindRemoteMultiselect("#designationSelect", "/GcAccessFilter/designations", "Select Designation", "designation");
            bindRemoteMultiselect("#employeeSelect", "/GcAccessFilter/employees", "Select Employee", null);

            // Ensure status/nature/type participate in filterRegistry-based collection
            // (fallback path if/when buildRequestPayload's collect() is used directly)
            registerFilterSelector("#empStatusSelect", "employeestatus");

            const accessCode = settings.accessCode;
            const isReadonly = accessCode === "0005"; // adjust to your actual readonly condition

            if (isReadonly) {
                // Locked dropdowns, no cascade, no Select All/search
                ms_InitializeMultiselects(null, null, true);
                await ms_ApplyAccessCodeToAll(accessCode);
            } else {
                ms_InitializeMultiselects({
                    '#empTypeSelect': 'Select Type',
                    '#empNatureSelect': 'Select Nature',
                    '#empStatusSelect': 'Select Status'
                });
                ms_InitializeMultiselects();
                ms_BindCascade();
                ms_Reset("#companySelect");
                await ms_LoadNext("#companySelect", "/GcAccessFilter/companies");
                await ms_AutoSelectCompany("001");
                $("#companySelect").trigger('change');

                // ================================================================
                // NEW: page-local cascade for empStatus/empNature/empType → employee.
                // cascadeMap in gc-each-multiselect.js has no entries for these
                // three selectors and that file can't be edited, so bind the
                // reset+reload here instead of relying on ms_BindCascade.
                // ================================================================
                $("#empStatusSelect, #empNatureSelect, #empTypeSelect")
                    .off("change.pageCascade")
                    .on("change.pageCascade", function () {
                        ms_Reset("#employeeSelect");
                        ms_LoadNext("#employeeSelect", filterUrlMap.get("#employeeSelect"));
                    });
            }
        });
    }
})(jQuery);









// (function ($) {
//     $.empSalaryInfoReport = function (options) {
//         const _originalBuildReq = buildReq; // keep reference to original

//         buildReq = function (page, search) {
//             const base = _originalBuildReq(page, search);

//             // Extend with page-specific params
//             return Object.assign(base, {
//                 EmpStatuses: arrVal("#empStatusSelect"),
//                 JoiningDateFrom: $("#joiningDateFromSelect").val() || null,
//                 JoiningDateTO: $("#joiningDateToSelect").val() || null,
//                 EmpNatures: arrVal("#empNatureSelect"),
//                 EmpTypes: arrVal("#empTypeSelect")
//             });
//         };

//         const settings = $.extend({
//             baseUrl: "/",
//             companyIds: "#companySelect",
//             branchIds: "#branchSelect",
//             departmentIds: "#departmentSelect",
//             designationIds: "#designationSelect",
//             employeeIds: "#employeeSelect",

//             empType: "#empTypeSelect",
//             empNature: "#empNatureSelect",
//             empStatus: "#empStatusSelect",
            
//             joiningDateFrom: "#joiningDateFromSelect",
//             joiningDateTo: "#joiningDateToSelect",

//             previewContainer: "#salaryInfoReport-container .card-body",
//             previewContainerBody: "#salaryInfoReport-container",
//             iFrame:"#previewIframe",
//             load: () => console.log("Loading...")
//         }, options);

//         const urls = {
//             filter: `${settings.baseUrl}/getAllFilterEmp`,
//             export: `${settings.baseUrl}/ExportReport`,
//             preview: `${settings.baseUrl}/PreviewReport`
//         };

//         let isInitialLoad = true;
//         let filterChangeBound = false;
//         let flatpickrInstances = {};

//         const toArray = val => val ? (Array.isArray(val) ? val : [val]) : [];
//         const showLoading = () => $("#customLoadingOverlay").css("display", "flex");
//         const hideLoading = () => $("#customLoadingOverlay").hide();

//         const showToast = (type, message) => {
//             if (typeof toastr !== 'undefined') {
//                 toastr[type](message);
//             } else {
//                 alert(message);
//             }
//         };

//         const setupLoadingOverlay = () => {
//             if ($("#customLoadingOverlay").length === 0) {
//                 $("body").append(`
//                     <div id="customLoadingOverlay" style="display:none;position:fixed;top:0;left:0;width:100%;height:100%;background:rgba(0,0,0,0.5);z-index:9999;justify-content:center;align-items:center;">
//                         <div style="background:white;padding:20px;border-radius:5px;box-shadow:0 0 10px rgba(0,0,0,0.3);text-align:center;">
//                             <div class="spinner-border text-primary"></div>
                            
//                         </div>
//                     </div>
//                 `);
//             }
//         };

//         const initializeFlatpickr = () => {
//             if (typeof flatpickr === 'undefined')
//                 return;
            
//             const fromPicker = flatpickr(settings.joiningDateFrom, CalendarService.createConfig(
//                 {
//                 onChange: function (selectedDates) {
//                     if (selectedDates.length) {
//                         toPicker.set('minDate', selectedDates[0]);
//                         //if (!isInitialLoad) loadFilterEmp();
//                     }
//                 }
//             }));

//             const toPicker = flatpickr(settings.joiningDateTo, CalendarService.createConfig(
//                 {
//                 onChange: function (selectedDates) {
//                     if (selectedDates.length) {
//                         fromPicker.set('maxDate', selectedDates[0]);
//                         //if (!isInitialLoad) loadFilterEmp();
//                     }
//                 }
//             }));
//         }

//         //const initializeMultiselects = () => {
//         //    const selectors = [
//         //        [settings.companyIds, 'Company(s)'],
//         //        [settings.branchIds, 'Branch(es)'],
//         //        [settings.departmentIds, 'Department(s)'],
//         //        [settings.designationIds, 'Designation(s)'],
//         //        [settings.employeeIds, 'Employee(s)'],
//         //        [settings.empNature, 'Employee Nature(s)'],
//         //        [settings.empType, 'Employee Type(s)'],
//         //        [settings.empStatus, 'Employee Status(s)']
//         //    ];

//         //    selectors.forEach(([selector, text]) => {
//         //        $(selector).multiselect({
//         //            enableFiltering: true,
//         //            includeSelectAllOption: true,
//         //            selectAllText: 'Select All',
//         //            nonSelectedText: `Select ${text}`,
//         //            nSelectedText: 'Selected',
//         //            allSelectedText: 'All Selected',
//         //            filterPlaceholder: 'Search.......',
//         //            buttonWidth: '100%',
//         //            maxHeight: 350,
//         //            enableClickableOptGroups: true,
//         //            numberDisplayed: 1,
//         //            enableCaseInsensitiveFiltering: true
//         //        });
//         //    });
//         //};


//         const getFilterValue = () => ({
//             CompanyCodes: toArray($(settings.companyIds).val()),
//             BranchCodes: toArray($(settings.branchIds).val()),
//             DepartmentCodes: toArray($(settings.departmentIds).val()),
//             DesignationCodes: toArray($(settings.designationIds).val()),
//             EmployeeIDs: toArray($(settings.employeeIds).val()),
//             EmpNatures: toArray($(settings.empNature).val()),
//             EmpStatuses: toArray($(settings.empStatus).val()),
//             EmpTypes: toArray($(settings.empType).val()),

//             JoiningDateFrom: $(settings.joiningDateFrom).val() || null,
//             JoiningDateTo: $(settings.joiningDateTo).val() || null
//         });

//         const populateSelect = (selector, dataList) => {
//             const $select = $(selector);

//             dataList.forEach(item => {
//                 if (item.code && item.name && $select.find(`option[value="${item.code}"]`).length === 0) {
//                     $select.append(`<option value="${item.code}">${item.name}</option>`);
//                 }
//             });
//             $select.multiselect('rebuild');

//             if (selector === settings.empStatus && isInitialLoad)
//                 selectActiveStatus(dataList);
//         }

//         const selectActiveStatus = (empStatuses) => {
//             const activeStatus = empStatuses.find(status =>
//                 status.code === '01' ||
//                 status.name.toLowerCase() === 'active'
//             );

//             if (activeStatus) {
//                 // ✅ 'false' prevents triggering the change event
//                 $(settings.empStatus).multiselect('select', activeStatus.code, false);
//             }
//         }

//         const clearDependentDropdowns = selectors => {
//             selectors.forEach(selector => {
//                 $(selector).empty().multiselect('rebuild');
//             });
//         };

//         const setupCascadingClearEvents = () => {
//             const clearMap = {
//                 [settings.companyIds]: [settings.branchIds, settings.departmentIds, settings.designationIds, settings.employeeIds],
//                 [settings.branchIds]: [settings.departmentIds, settings.designationIds, settings.employeeIds],
//                 [settings.departmentIds]: [settings.designationIds, settings.employeeIds],
//                 [settings.designationIds]: [settings.employeeIds],
//                 [settings.empNature]: [settings.departmentIds, settings.designationIds, settings.employeeIds],
//                 [settings.empType]: [settings.departmentIds, settings.designationIds, settings.employeeIds],
//                 [settings.empStatus]: [settings.departmentIds, settings.designationIds, settings.employeeIds],
//             };

//             Object.entries(clearMap).forEach(([parent, children]) => {
//                 $(parent).off("change").on("change", function () {
//                     clearDependentDropdowns(children);
//                 });
//             });
//         };

//         const bindFilterChangeEvents = () => {
//             if (!filterChangeBound) {
//                 const filterSelectors = [
//                     settings.companyIds,
//                     settings.branchIds,
//                     //settings.departmentIds,
//                     //settings.designationIds,
//                     //settings.employeeIds,
//                     settings.empType,
//                     settings.empNature,
//                     settings.empStatus
//                 ].join(',');

//                 $(filterSelectors).on("change.loadFilter", function () {
//                     loadFilterEmp();
//                 });

//                 //$(settings.joiningDateFrom + ',' + settings.joiningDateTo).on("change.loadFilter", function () {
//                 //    if (!isInitialLoad) {
//                 //        loadFilterEmp();
//                 //    }
//                 //});

//                 $(settings.joiningDateFrom).on('input.loadFilter', function () {
//                     clearTimeout(window.yearInputTimeout);
//                     window.yearInputTimeout = setTimeout(() => {
//                         clearDependentDropdowns([
//                             settings.departmentIds,
//                             settings.designationIds,
//                             settings.employeeIds,
//                             //settings.typeIds,
//                         ]);
//                         loadFilterEmp();
//                     }, 300);
//                 });

//                 $(settings.joiningDateTo).on('input.loadFilter', function () {
//                     clearTimeout(window.yearInputTimeout);
//                     window.yearInputTimeout = setTimeout(() => {
//                         clearDependentDropdowns([
//                             settings.departmentIds,
//                             settings.designationIds,
//                             settings.employeeIds,
//                             //settings.typeIds,
//                         ]);
//                         loadFilterEmp();
//                     }, 300);
//                 });

//                 filterChangeBound = true;
//             }
//         };

//         const loadFilterEmp = () => {
//             $.ajax({
//                 url: urls.filter,
//                 type: "POST",
//                 contentType: "application/json",
//                 data: JSON.stringify(getFilterValue()),
//                 success: res => {
//                     if (!res.isSuccess) {
//                         showToast('error', res.message);
//                         return;
//                     }

//                     const { data } = res;
//                     console.log(data);
//                     if (data.companies?.length)
//                         populateSelect(settings.companyIds, data.companies);

//                     if (data.branches?.length)
//                         populateSelect(settings.branchIds, data.branches);

//                     //if (data.departments?.length)
//                     //    populateSelect(settings.departmentIds, data.departments);

//                     //if (data.designations?.length)
//                     //    populateSelect(settings.designationIds, data.designations);

//                     //if (data.employeeIds?.length)
//                     //    populateSelect(settings.employeeIds, data.employeeIds);

//                     if (data.empNatures?.length)
//                         populateSelect(settings.empNature, data.empNatures);

//                     if (data.empStatuses?.length)
//                         populateSelect(settings.empStatus, data.empStatuses);

//                     if (data.empTypes?.length)
//                         populateSelect(settings.empType, data.empTypes);

//                     //setupCascadingClearEvents();
//                     bindFilterChangeEvents();

//                     isInitialLoad = false;
//                 },
//                 error: () => {
//                     showToast('error', 'Failed to load data');
//                 }
//             });
//         };

//         const showPreview = () => {
//             const filterData = getFilterValue();
//             const hasFilters = Object.values(filterData).some(value =>
//                 (Array.isArray(value) && value.length > 0) ||
//                 (typeof value === 'string' && value.trim() !== '')
//             );

//             if (!hasFilters) {
//                 showToast('warning', 'No working day is found to generate report');
//                 return;
//             }

//             $(settings.previewContainer).show();
//             // $("#previewLoadingIndicator").show();
//             showLoading();
//             $(settings.iFrame).attr('src', '');

//             $.ajax({
//                 url: urls.preview,
//                 type: "POST",
//                 contentType: "application/json",
//                 data: JSON.stringify({ FilterData: filterData, ExportFormat: 'pdf' }),
//                 xhrFields: { responseType: 'blob' },
//                 success: data => {
//                     hideLoading();
//                     if (window.currentPdfUrl) {
//                         window.URL.revokeObjectURL(window.currentPdfUrl);
//                     }

//                     const blob = new Blob([data], { type: 'application/pdf' });
//                     window.currentPdfUrl = window.URL.createObjectURL(blob);
//                     $(settings.iFrame).attr('src', window.currentPdfUrl);
//                     $(settings.iFrame).show();
//                 },
//                 error: xhr => {
//                     hideLoading();
//                     $(settings.previewContainer).hide();
//                     let errorMessage = 'Preview failed. Please try again.';

//                     switch (xhr.status) {
//                         case 400:
//                             errorMessage = 'Invalid request. Please check your inputs.';
//                             break;
//                         case 404:
//                             errorMessage = 'No data found for the selected criteria.';
//                             break;
//                         default:
//                             errorMessage = 'An unexpected error occurred. Please try again later.';
//                             break;
//                     }
//                     showToast('error', errorMessage);
//                 }
//             });
//         };

//         const hidePreview = () => {
//             $(settings.previewContainer).hide();
//             $(settings.iFrame).attr('src', '');
//             if (window.currentPdfUrl) {
//                 window.URL.revokeObjectURL(window.currentPdfUrl);
//                 window.currentPdfUrl = null;
//             }
//         };

//         const exportReport = format => {
//             showLoading();

//             $.ajax({
//                 url: urls.export,
//                 type: "POST",
//                 contentType: "application/json",
//                 data: JSON.stringify({ FilterData: getFilterValue(), ExportFormat: format }),
//                 xhrFields: { responseType: 'blob' },
//                 success: (data, status, xhr) => {
//                     hideLoading();

//                     const contentType = xhr.getResponseHeader('Content-Type');
//                     const disposition = xhr.getResponseHeader('Content-Disposition');

//                     let filename;
//                     if (disposition && disposition.indexOf("filename=") != -1) {
//                         const match = disposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
//                         if (match && match[1]) {
//                             filename = match[1].replace(/['"]/g, '');
//                         }
//                     }

//                     if (format === "download") {
//                         const blob = new Blob([data], { type: 'application/pdf' });
//                         const iframe = Object.assign(document.createElement('iframe'), {
//                             style: 'display:none',
//                             src: window.URL.createObjectURL(blob),
//                             onload: function () {
//                                 this.contentWindow.focus();
//                                 this.contentWindow.print();
//                             }
//                         });
//                         document.body.appendChild(iframe);
//                         return;
//                     }
//                     const blob = new Blob([data], { type: contentType });
//                     const link = Object.assign(document.createElement("a"), {
//                         href: window.URL.createObjectURL(blob),
//                         download: filename
//                     });

//                     document.body.appendChild(link);
//                     link.click();
//                     document.body.removeChild(link);
//                     window.URL.revokeObjectURL(link.href);
//                     clearAllFilters();
//                     showToast('success', 'Report exported successfully');
//                 },
//                 error: (e) => {
//                     hideLoading();
//                     let errorMessage = 'Export failed. Please try again.'; 

//                     switch (e.status) {
//                         case 400:
//                             errorMessage = 'Invalid request. Please check your inputs.';
//                             break;
//                         case 404:
//                             errorMessage = 'No data found for the selected criteria.';
//                             break;
//                         default:
//                             errorMessage = 'An unexpected error occurred. Please try again later.';
//                             break;
//                     }
//                     showToast('error', errorMessage);
//                 }
//             });
//         };

//         const clearAllFilters = () => {
//             const filterSelectors = [
//                 settings.companyIds,
//                 settings.branchIds,
//                 settings.departmentIds,
//                 settings.designationIds,
//                 settings.employeeIds,
//                 settings.empType,
//                 settings.empNature,
//                 settings.empStatus
//             ];

//             filterSelectors.forEach(selector => {
//                 $(selector).multiselect('deselectAll', false);
//                 $(selector).multiselect('updateButtonText');
//             });

//             if (flatpickrInstances.joiningDateFrom) {
//                 flatpickrInstances.joiningDateFrom.clear();
//             }
//             if (flatpickrInstances.joiningDateTo) {
//                 flatpickrInstances.joiningDateTo.clear();
//             }

//             hidePreview();

//             isInitialLoad = true;

//             loadFilterEmp();
//         };

//         const bindUIEvents = () => {
//             $("#previewBtn").on("click", () =>
//                 $(settings.previewContainer).is(':visible') ? hidePreview() : showPreview()
//             );
//             $("#downloadBtn").on("click", () => exportReport($("#exportFormatSelect").val()));
//             $("#closePreviewBtn").on("click", hidePreview);
//             $("#refreshPreviewBtn").on("click", showPreview);
//         };

//         const init = () => {
//             settings.load();
           
//             if (typeof $.fn.multiselect !== 'undefined') {
//                 initializeMultiselects({
//                     '#companySelect': 'Select Company',
//                     '#branchSelect': 'Select Branch',
//                     //'#employeeSelect': 'Select Employee',
//                     //'#departmentSelect': 'Select Department',
//                     //'#designationSelect': 'Select Designation',
//                     '#empTypeSelect': 'Select Type',
//                     '#empNatureSelect': 'Select Nature',
//                     '#empStatusSelect': 'Select Status'
//                 });
//             } else {
//                 console.error('Bootstrap Multiselect not loaded!');
//             }

//             initializeFlatpickr();
//             setupLoadingOverlay();
//             bindUIEvents();
//             loadFilterEmp(); 
//         };

//         $(settings.previewContainer).hide();
//         init();

//         $(document).ready(async function () {

//             Object.keys(gcCascade).forEach(k => delete gcCascade[k]);
//             Object.assign(gcCascade, {
//                 "#companySelect": ["#branchSelect", "#departmentSelect", "#designationSelect", "#employeeSelect"],
//                 "#branchSelect": ["#departmentSelect", "#designationSelect", "#employeeSelect"],
//                 "#departmentSelect": ["#designationSelect", "#employeeSelect"],
//                 "#designationSelect": ["#employeeSelect"],
//                 "#empStatusSelect": [ "#employeeSelect"],
//                 "#empNatureSelect": [ "#employeeSelect"],
//                 "#empTypeSelect": [ "#employeeSelect"],
//             });

//             gcBindRemoteMultiselect("#departmentSelect", "/GcFilters/department", "Select Department");
//             gcBindRemoteMultiselect("#designationSelect", "/GcFilters/designation", "Select Designation");
//             gcBindRemoteMultiselect("#employeeSelect", "employee", "Select Employee");

//             bsms_InitializeMultiselects({
//                 '#departmentSelect': 'Select Department',
//                 '#designationSelect': 'Select Designation',
//                 '#employeeSelect': 'Select Employee'
//             });
//             bsms_BindCascade();
//         });

//     }
// })(jQuery);