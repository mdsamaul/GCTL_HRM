// ============================================================
// SHARED STATE
// ============================================================
const filterState = new Map();
const filterUrlMap = new Map();
const filterSearchTimers = new Map();

// ============================================================
// FILTER VALUE REGISTRY
// ============================================================
const filterRegistry = {
    company: [],
    branch: [],
    division: [],
    department: [],
    designation: [],
    employeeStatus: [],
};

function registerFilterSelector(selector, filterType) {
    if (!filterType) return;
    const type = filterType.toLowerCase();
    if (!filterRegistry[type]) filterRegistry[type] = [];
    if (!filterRegistry[type].includes(selector))
        filterRegistry[type].push(selector);
}

function buildRequestPayload(page, search) {
    function collect(type) {
        const selectors = filterRegistry[type] || [];
        const vals = [];
        selectors.forEach(sel => {
            const v = $(sel).val();
            if (!v) return;
            const arr = Array.isArray(v) ? v : [v];
            arr.forEach(x => { if (x && !vals.includes(x)) vals.push(x); });
        });
        return vals;
    }
    return {
        CompanyCodes: collect('company'),
        BranchCodes: collect('branch'),
        DivisionCodes: collect('division'),
        DepartmentCodes: collect('department'),
        DesignationCodes: collect('designation'),
        EmployeeStatuses: collect('employeestatus'),
        Page: page || 1,
        PageSize: 10,
        Search: search || ""
    };
}

function getArrayValue(selector) {
    const v = $(selector).val();
    if (!v || v.length === 0) return [];
    return Array.isArray(v) ? v : [v];
}

const defaultSelectorTypeMap = {
    '#companySelect': 'company',
    '#branchSelect': 'branch',
    '#divisionSelect': 'division',
    '#departmentSelect': 'department',
    '#designationSelect': 'designation',
    '#activityStatusSelect': 'employeeStatus',
    '#employeeSelect': null,
};

function bindRemoteMultiselect(selector, url, placeholder, filterType) {
    filterState.set(selector, { page: 1, more: true, loading: false, search: "" });
    filterUrlMap.set(selector, url);
    $(selector).attr("data-placeholder", placeholder);
    const resolvedType = filterType !== undefined ? filterType : (defaultSelectorTypeMap[selector] ?? null);
    if (resolvedType) registerFilterSelector(selector, resolvedType);
}

// ============================================================
// CASCADE MAP
// ============================================================
const cascadeMap = {
    "#companySelect": ["#branchSelect", "#divisionSelect", "#departmentSelect", "#designationSelect", "#employeeSelect"],
    "#branchSelect": ["#divisionSelect", "#departmentSelect", "#designationSelect", "#employeeSelect"],
    "#divisionSelect": ["#departmentSelect", "#designationSelect", "#employeeSelect"],
    "#departmentSelect": ["#designationSelect", "#employeeSelect"],
    "#designationSelect": ["#employeeSelect"],
    "#activityStatusSelect": ["#employeeSelect"]
};

defaultSelectorTypeMap['#activityStatusSelect'] = 'employeeStatus';

let cascadeLock = false;


// ╔══════════════════════════════════════════════════════════════╗
// ║            BOOTSTRAP MULTISELECT   (prefix: ms_)           ║
// ╚══════════════════════════════════════════════════════════════╝

function ms_GetMenu(selector) {
    const $bg = $(selector).next('.btn-group');
    if (!$bg.length) return null;
    const tries = [
        $bg.find('ul.multiselect-container'),
        $bg.find('.multiselect-container'),
        $bg.find('ul.dropdown-menu'),
        $bg.find('.dropdown-menu'),
    ];
    for (const $t of tries) { if ($t.length) return $t; }
    return null;
}

function ms_SetCss(selector) {
    const $menu = ms_GetMenu(selector);
    if (!$menu || !$menu[0]) return;
    $menu[0].style.setProperty('max-height', '220px', 'important');
    $menu[0].style.setProperty('overflow-y', 'auto', 'important');
    $menu[0].style.setProperty('overflow-x', 'hidden', 'important');
}

function ms_UpdateClearIcon($select) {
    const $container = $select.next('.btn-group');
    const $btn = $container.find('button.multiselect');
    const selected = $select.find('option:selected').length;
    const isOpen = $btn.hasClass('is-inline-searching');
    if (selected > 0 && !isOpen) $btn.addClass('show-right-clear');
    else $btn.removeClass('show-right-clear');
}

function ms_Reset(selector) {
    filterState.set(selector, { page: 1, more: true, loading: false, search: "" });
    $(selector).empty();
    try { $(selector).multiselect('rebuild'); } catch (e) { }
}

function ms_RebuildKeepScroll(selector) {
    const $menu = ms_GetMenu(selector);
    const scrollTop = $menu ? $menu[0].scrollTop : 0;
    try { $(selector).multiselect('rebuild'); } catch (e) { }
    setTimeout(() => {
        ms_SetCss(selector);
        const $m = ms_GetMenu(selector);
        if ($m && $m[0]) $m[0].scrollTop = scrollTop;
        const url = filterUrlMap.get(selector);
        if (url) ms_BindScroll(selector, url);
    }, 10);
}

async function ms_LoadNext(selector, url) {
    let st = filterState.get(selector);
    if (!st) { st = { page: 1, more: true, loading: false, search: "" }; filterState.set(selector, st); }
    if (st.loading || !st.more) return;
    st.loading = true;
    const $sel = $(selector);
    const req = buildRequestPayload(st.page, st.search);
    try {
        const res = await $.ajax({ url, type: "POST", contentType: "application/json", data: JSON.stringify(req) });
        if (!res || !res.isSuccess) return;
        const items = res.data.items || res.data.Items || [];
        const more = res.data.more ?? res.data.More ?? false;
        st.page++; st.more = more;
        items.forEach(x => {
            const code = x.code || x.Code;
            const name = x.name || x.Name;
            if (!code) return;
            if ($sel.find(`option[value="${code}"]`).length === 0)
                $sel.append(`<option value="${code}">${name}</option>`);
        });
        ms_RebuildKeepScroll(selector);
    } catch (err) {
        console.error(`[MS LOAD] ${selector}:`, err);
    } finally {
        st.loading = false;
    }
}

function ms_BindScroll(selector, url) {
    const $menu = ms_GetMenu(selector);
    if (!$menu || !$menu.length) return;
    ms_SetCss(selector);
    $menu.off("scroll.filterPaging").on("scroll.filterPaging", async function () {
        const dist = this.scrollHeight - this.scrollTop - this.clientHeight;
        if (dist < 100) {
            const st = filterState.get(selector);
            if (st && st.more && !st.loading) await ms_LoadNext(selector, url);
        }
    });
}

function ms_BindSearch(selector, url) {
    const $sel = $(selector);
    const $btnGroup = $sel.next('.btn-group');
    const $btn = $btnGroup.find('button.multiselect');
    const $inline = $btn.find('input.multiselect-inline-search');
    const $msSearch = $btnGroup.find('input.multiselect-search');
    if (!$inline.length) return;
    $msSearch.off('input keyup change');
    $inline.off('input.filterRemoteSearch').on('input.filterRemoteSearch', function () {
        const term = $(this).val() || "";
        clearTimeout(filterSearchTimers.get(selector));
        filterSearchTimers.set(selector, setTimeout(async () => {
            const st = filterState.get(selector);
            if (!st) return;
            if (st.loading) st.loading = false;
            st.page = 1; st.more = true; st.search = term;
            $sel.empty();
            try { $sel.multiselect('rebuild'); } catch (e) { }
            await ms_LoadNext(selector, url);
            setTimeout(() => { ms_SetCss(selector); ms_BindScroll(selector, url); }, 50);
        }, 350));
    });
}

async function ms_OnParentChanged(parentSelector) {
    if (cascadeLock) return;
    cascadeLock = true;
    const safetyTimer = setTimeout(() => { cascadeLock = false; }, 2000);
    try {
        const targets = cascadeMap[parentSelector] || [];
        targets.forEach(s => { if (filterState.has(s)) ms_Reset(s); });
        for (const s of targets) {
            const url = filterUrlMap.get(s);
            if (url) await ms_LoadNext(s, url);
        }
    } finally {
        clearTimeout(safetyTimer);
        cascadeLock = false;
    }
}

function ms_BindCascade() {
    Object.keys(cascadeMap).forEach(parent => {
        $(parent).off("change.filterCascade").on("change.filterCascade", () => ms_OnParentChanged(parent));
    });
}

async function ms_LoadAllThenSelectAll(selector, url) {
    const st = filterState.get(selector);
    if (!st) return;
    while (st.more) {
        if (st.loading) { await new Promise(r => setTimeout(r, 100)); continue; }
        await ms_LoadNext(selector, url);
    }
    try { $(selector).multiselect('selectAll', false); $(selector).multiselect('updateButtonText'); } catch (e) { }
}

async function ms_AutoSelectCompany(code) {
    const selector = "#companySelect";
    const $sel = $(selector);
    if ($sel.find(`option[value="${code}"]`).length === 0)
        await ms_LoadNext(selector, filterUrlMap.get(selector) || "/GcFilters/company");
    if ($sel.find(`option[value="${code}"]`).length > 0) {
        $sel.val([code]);
        try { $sel.multiselect('rebuild'); } catch (e) { }
        $sel.trigger('change');
    }
}


// ============================================================
// ms_InjectSingleItem
// ── readonly mode এ: option inject → rebuild → button text set
//    checkbox নেই, Select All নেই, dropdown block
// ============================================================
function ms_InjectSingleItem(selector, data) {
    const $sel = $(selector);
    if (!$sel.length || !data) return;

    const code = data.code || data.Code;
    const name = data.name || data.Name;
    if (!code) return;

    // পেজিং বন্ধ — আর load হবে না
    filterState.set(selector, { page: 1, more: false, loading: false, search: "" });

    // Option inject
    $sel.empty();
    $sel.append(`<option value="${code}" selected="selected">${name}</option>`);
    $sel.val([code]);

    // Rebuild
    try { $sel.multiselect('rebuild'); } catch (e) { }

    setTimeout(() => {
        const $btn = $sel.next('.btn-group').find('button.multiselect');
        const $menu = $sel.next('.btn-group').find('ul.multiselect-container');

        // Select All row সরাও
        $menu.find('li.multiselect-item.multiselect-all').remove();

        // Checkbox checked + li active
        $menu.find(`input[value="${CSS.escape(code)}"]`)
            .prop('checked', true)
            .closest('li').addClass('active');

        // Button text force
        $btn.find('.multiselect-selected-text').text(name);

        // Button disable — dropdown খুলবে না
        $btn.prop('disabled', true).addClass('ms-readonly-btn');
        $btn.off('click mousedown keydown')
            .on('click mousedown keydown', function (e) {
                e.preventDefault();
                e.stopPropagation();
            });

        ms_UpdateClearIcon($sel);
    }, 150);
}


// ============================================================
// ms_ApplyAccessCodeToAll
// ── individual endpoints parallel call করে প্রতিটা dropdown এ
//    ১টা করে item inject করে (page load এই, click ছাড়া)
// ============================================================
async function ms_ApplyAccessCodeToAll(accessCode) {
    if (!accessCode) return;

    // Selector → endpoint mapping
    const targets = [
        { selector: "#companySelect", url: "/GcAccessFilter/companies" },
        { selector: "#branchSelect", url: "/GcAccessFilter/branches" },
        { selector: "#divisionSelect", url: "/GcAccessFilter/divisions" },
        { selector: "#departmentSelect", url: "/GcAccessFilter/departments" },
        { selector: "#designationSelect", url: "/GcAccessFilter/designations" },
        { selector: "#employeeSelect", url: "/GcAccessFilter/employees" },
    ];

    // সব endpoint এ একসাথে (parallel) call করো
    const requests = targets.map(({ selector, url }) => {
        // শুধু page এ আছে এমন selector এর জন্য call করো
        if (!$(selector).length) return Promise.resolve({ selector, item: null });

        const req = buildRequestPayload(1, "");
        req.PageSize = 1;

        return $.ajax({
            url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(req)
        })
            .then(res => {
                if (!res || !res.isSuccess) return { selector, item: null };
                const items = res.data.items || res.data.Items || [];
                return { selector, item: items.length ? items[0] : null };
            })
            .catch(err => {
                console.error(`[MS ACCESS CODE] ${selector} → ${url}:`, err);
                return { selector, item: null };
            });
    });

    const results = await Promise.all(requests);

    // প্রতিটা result inject করো
    results.forEach(({ selector, item }) => {
        if (!item) return;
        ms_InjectSingleItem(selector, item);
    });
}


// ============================================================
// ms_InitializeMultiselects
// ============================================================
function ms_InitializeMultiselects(customConfigs, accessCode, readonly) {
    const isReadonly = readonly === true;

    const defaultConfigs = {
        '#companySelect': 'Select Company',
        '#branchSelect': 'Select Branch',
        '#divisionSelect': 'Select Division',
        '#departmentSelect': 'Select Department',
        '.gc-department': 'Select Department',
        '#designationSelect': 'Select Designation',
        '.gc-designation': 'Select Designation',
        '#employeeSelect': 'Select Employee'
    };
    const configs = customConfigs || defaultConfigs;

    const buttonTextFn = function (options, select) {
        const placeholder = $(select).attr('data-placeholder') || 'Select';
        if (options.length === 0) return placeholder;
        return options.length === 1
            ? $(options[0]).text().trim()
            : `${options.length} items selected`;
    };

    function forceInlineFocus($inline) {
        setTimeout(() => {
            requestAnimationFrame(() => {
                try { const el = $inline.get(0); if (el) el.focus(); } catch (e) { }
            });
        }, 0);
    }

    function preventBootstrapToggleWhileTyping($select) {
        const $container = $select.next('.btn-group');
        const $btn = $container.find('button.multiselect');
        const $inline = $btn.find('input.multiselect-inline-search');
        $btn.off('keydown.inlineGuard').on('keydown.inlineGuard', function (e) {
            if (!$btn.hasClass('is-inline-searching')) return;
            if (e.key === ' ' || e.key === 'Spacebar' || e.key === 'Enter') {
                e.preventDefault(); e.stopPropagation();
            }
        });
        $inline.off('.inlineGuard').on('keydown.inlineGuard keypress.inlineGuard keyup.inlineGuard', function (e) {
            if (e.key === 'Escape') { e.stopPropagation(); $btn.dropdown('toggle'); return; }
            if (e.key === ' ' || e.key === 'Spacebar') {
                if (e.type === 'keyup') e.preventDefault();
                e.stopPropagation(); return;
            }
            if (e.key === 'Enter') { e.preventDefault(); e.stopPropagation(); return; }
            e.stopPropagation();
        });
        $inline.off('mousedown.inlineClick click.inlineClick')
            .on('mousedown.inlineClick click.inlineClick', function (e) { e.stopPropagation(); });
    }

    function ensureClearAllRow($select, selector) {
        if (isReadonly) return;
        const $container = $select.next('.btn-group');
        const $menu = $container.find('ul.multiselect-container');
        if ($menu.find('li.multiselect-clearall').length) return;
        $menu.prepend(
            '<li class="multiselect-item multiselect-clearall">' +
            '<a href="#" class="multiselect-clearall-link" tabindex="0">' +
            '<span class="ca-icon"><i class="fa fa-times-circle"></i></span>' +
            '<span class="ca-text"> Clear all</span></a></li>'
        );
        $menu.off('click.clearAll', '.multiselect-clearall-link')
            .on('click.clearAll', '.multiselect-clearall-link', function (e) {
                e.preventDefault(); e.stopPropagation();
                $select.multiselect('deselectAll', false);
                $select.multiselect('updateButtonText');
                $container.find('button.multiselect input.multiselect-inline-search').val('').trigger('input');
                ms_UpdateClearIcon($select);
                const st = filterState.get(selector);
                if (st) st.search = "";
                $select.trigger('change');
            });
    }

    function ensureRightClearIcon($select, selector) {
        if (isReadonly) return;
        const $container = $select.next('.btn-group');
        const $btn = $container.find('button.multiselect');
        if ($btn.find('.ms-right-clear').length) return;
        const $right = $('<span class="ms-right-clear" title="Clear"><i class="fa fa-times"></i></span>');
        const $caret = $btn.find('b.caret');
        if ($caret.length) $right.insertBefore($caret); else $btn.append($right);
        $right.on('mousedown click', function (e) {
            e.preventDefault(); e.stopPropagation();
            $select.multiselect('deselectAll', false);
            $select.multiselect('updateButtonText');
            $btn.find('input.multiselect-inline-search').val('').trigger('input');
            ms_UpdateClearIcon($select);
            const st = filterState.get(selector);
            if (st) st.search = "";
            $select.trigger('change');
        });
    }

    $.each(configs, function (selector, placeholder) {
        const $select = $(selector);
        if (!$select.length) return;
        $select.attr('data-placeholder', placeholder);
        try { $select.multiselect('destroy'); } catch (e) { }

        $select.multiselect({
            // readonly mode এ Select All ও filtering বন্ধ
            includeSelectAllOption: !isReadonly,
            selectAllText: 'Select All',
            nonSelectedText: placeholder,
            allSelectedText: 'All Selected',
            nSelectedText: 'Selected',
            buttonWidth: '100%',
            maxHeight: 250,
            dropUp: false,
            enableClickableOptGroups: true,
            enableFiltering: !isReadonly,
            enableCaseInsensitiveFiltering: false,
            filterBehavior: 'text',
            numberDisplayed: 0,
            buttonText: buttonTextFn,
            buttonTitle: buttonTextFn,
            templates: {
                button:
                    '<button type="button" class="multiselect dropdown-toggle" data-toggle="dropdown">' +
                    '<span class="multiselect-selected-text"></span>' +
                    // readonly হলে inline search input রাখবো না
                    (isReadonly ? '' : '<input type="text" class="multiselect-inline-search" autocomplete="off" />') +
                    '<b class="caret"></b></button>',
                ul: '<ul class="multiselect-container dropdown-menu"></ul>',
                filter:
                    '<li class="multiselect-item multiselect-filter">' +
                    '<div class="input-group">' +
                    '<div class="input-group-prepend"><span class="input-group-text"><i class="fa fa-search"></i></span></div>' +
                    '<input class="form-control multiselect-search" type="text" placeholder="Search..." /></div></li>',
                li: '<li><a tabindex="0"><label class="checkbox"></label></a></li>',
                divider: '<li class="multiselect-item divider"></li>',
                liGroup: '<li class="multiselect-item multiselect-group"><label></label></li>'
            },

            onInitialized: function () {
                const $container = $select.next('.btn-group');
                const $btn = $container.find('button.multiselect');
                if (isReadonly) {
                    // readonly: init এ button disable করো
                    $btn.prop('disabled', true).addClass('ms-readonly-btn');
                    $btn.off('click mousedown keydown')
                        .on('click mousedown keydown', function (e) {
                            e.preventDefault(); e.stopPropagation();
                        });
                } else {
                    $btn.find('.multiselect-inline-search').hide();
                    $btn.find('.multiselect-selected-text').show();
                    ensureRightClearIcon($select, selector);
                    ms_UpdateClearIcon($select);
                }
            },

            onDropdownShown: function () {
                // readonly mode এ dropdown কখনো খুলবে না
                if (isReadonly) {
                    try { $select.multiselect('hide'); } catch (e) { }
                    return;
                }
                const $container = $select.next('.btn-group');
                const $btn = $container.find('button.multiselect');
                const $inline = $btn.find('.multiselect-inline-search');
                const $text = $btn.find('.multiselect-selected-text');
                ensureClearAllRow($select, selector);
                $btn.addClass('is-inline-searching');
                $btn.removeClass('show-right-clear');
                $text.hide();
                $inline.show().val('').attr('placeholder', placeholder);
                const st = filterState.get(selector);
                if (st) st.search = "";
                preventBootstrapToggleWhileTyping($select);
                forceInlineFocus($inline);
                const url = filterUrlMap.get(selector);
                if (url) {
                    setTimeout(() => {
                        ms_SetCss(selector);
                        ms_BindScroll(selector, url);
                        ms_BindSearch(selector, url);
                        const curSt = filterState.get(selector);
                        if ($select.find('option').length === 0 && curSt && curSt.more && !curSt.loading)
                            ms_LoadNext(selector, url);
                    }, 50);
                }
            },

            onDropdownHidden: function () {
                if (isReadonly) return;
                const $container = $select.next('.btn-group');
                const $btn = $container.find('button.multiselect');
                const $inline = $btn.find('.multiselect-inline-search');
                const $text = $btn.find('.multiselect-selected-text');
                $btn.removeClass('is-inline-searching');
                $inline.val('').hide();
                $text.show();
                ms_UpdateClearIcon($select);
            },

            onSelectAll: function () {
                if (isReadonly) return;
                const url = filterUrlMap.get(selector);
                const st = filterState.get(selector);
                if (url && st && st.more) ms_LoadAllThenSelectAll(selector, url);
                ms_UpdateClearIcon($select);
            },

            onChange: function () {
                if (isReadonly) return;
                ms_UpdateClearIcon($select);
            }
        });
    });
}


// ╔══════════════════════════════════════════════════════════════╗
// ║              SELECT2    (prefix: sl_)                       ║
// ╚══════════════════════════════════════════════════════════════╝

const sl_CascadeMap = {};

function sl_RegisterCascade(parentSelector, childSelectors) {
    sl_CascadeMap[parentSelector] = childSelectors;
}

function sl_Reset(selector) {
    filterState.set(selector, { page: 1, more: true, loading: false, search: "" });
    $(selector).find('option:not([value=""])').remove();
    $(selector).val(null).trigger('change.select2');
}

function _sl_injectIntoOpenDropdown(selector, items) {
    const $resultsList = $(document).find('.select2-results__options');
    if (!$resultsList.length) return;
    $resultsList.find('.filter-loading-item').remove();
    items.forEach(x => {
        const code = x.code || x.Code;
        const name = x.name || x.Name;
        if (!code) return;
        if ($resultsList.find(`li[data-sl-val="${CSS.escape(code)}"]`).length) return;
        const $li = $(`<li class="select2-results__option" role="option" data-sl-val="${code}" style="cursor:pointer">${name}</li>`);
        $li.on('mousedown', function (e) {
            e.preventDefault(); e.stopPropagation();
            const $sel = $(selector);
            const isMultiple = $sel.prop('multiple');
            if (isMultiple) {
                const current = $sel.val() || [];
                const idx = current.indexOf(code);
                if (idx === -1) { current.push(code); $li.addClass('select2-results__option--selected'); }
                else { current.splice(idx, 1); $li.removeClass('select2-results__option--selected'); }
                $sel.val(current).trigger('change');
            } else {
                $sel.val(code).trigger('change');
                $sel.select2('close');
            }
        });
        $resultsList.append($li);
    });
}

async function sl_LoadNext(selector, url) {
    let st = filterState.get(selector);
    if (!st) { st = { page: 1, more: true, loading: false, search: "" }; filterState.set(selector, st); }
    if (st.loading || !st.more) return;
    st.loading = true;
    const $resultsList = $(document).find('.select2-results__options');
    if ($resultsList.length && !$resultsList.find('.filter-loading-item').length)
        $resultsList.append('<li class="select2-results__option filter-loading-item" style="color:#999;font-style:italic">Loading...</li>');
    const req = buildRequestPayload(st.page, st.search);
    try {
        const res = await $.ajax({ url, type: "POST", contentType: "application/json", data: JSON.stringify(req) });
        if (!res || !res.isSuccess) return;
        const items = res.data.items || res.data.Items || [];
        const more = res.data.more ?? res.data.More ?? false;
        st.page++; st.more = more;
        const $sel = $(selector);
        const newItems = [];
        items.forEach(x => {
            const code = x.code || x.Code;
            const name = x.name || x.Name;
            if (!code) return;
            if ($sel.find(`option[value="${code}"]`).length === 0) {
                $sel.append(new Option(name, code, false, false));
                newItems.push(x);
            }
        });
        if ($(document).find('.select2-results__options').length > 0)
            _sl_injectIntoOpenDropdown(selector, newItems);
        $sel.trigger('change.select2');
    } catch (err) {
        console.error(`[SL LOAD] ${selector}:`, err);
        $(document).find('.select2-results__options .filter-loading-item').remove();
    } finally {
        st.loading = false;
        $(document).find('.select2-results__options .filter-loading-item').remove();
    }
}

function sl_BindScroll(selector, url) {
    const $resultsList = $(document).find('.select2-results__options');
    if (!$resultsList.length) return;
    $resultsList.off('scroll.filterPaging').on('scroll.filterPaging', async function () {
        const dist = this.scrollHeight - this.scrollTop - this.clientHeight;
        if (dist < 80) {
            const st = filterState.get(selector);
            if (st && st.more && !st.loading) await sl_LoadNext(selector, url);
        }
    });
}

function sl_BindSearch(selector, url) {
    const $searchInput = $(document).find('.select2-search__field');
    if (!$searchInput.length) return;
    $searchInput.off('input.filterRemoteSearch').on('input.filterRemoteSearch', function () {
        const term = $(this).val() || "";
        clearTimeout(filterSearchTimers.get(selector));
        filterSearchTimers.set(selector, setTimeout(async () => {
            const st = filterState.get(selector);
            if (!st) return;
            if (st.loading) st.loading = false;
            st.page = 1; st.more = true; st.search = term;
            $(selector).find('option:not([value=""])').remove();
            $(document).find(".select2-results__options").empty();
            $(selector).trigger('change.select2');
            await sl_LoadNext(selector, url);
            setTimeout(() => sl_BindScroll(selector, url), 50);
        }, 350));
    });
}

function sl_BindOpen(selector, url) {
    if (!url) return;
    const $sel = $(selector);
    $sel.off('select2:open.filterRemote').on('select2:open.filterRemote', function () {
        setTimeout(() => {
            sl_BindScroll(selector, url);
            sl_BindSearch(selector, url);
            const st = filterState.get(selector);
            if (!st || st.loading) return;
            const $resultsList = $(document).find('.select2-results__options');
            const visibleItems = $resultsList.find('li[data-sl-val]').length;
            if (visibleItems === 0 && st.more) {
                if (st.page === 1) {
                    sl_LoadNext(selector, url);
                } else {
                    const existingItems = [];
                    $sel.find('option:not([value=""])').each(function () {
                        existingItems.push({ code: $(this).val(), name: $(this).text() });
                    });
                    if (existingItems.length) _sl_injectIntoOpenDropdown(selector, existingItems);
                    else sl_LoadNext(selector, url);
                }
            }
        }, 150);
    });
    $sel.off('select2:clear.filterRemote').on('select2:clear.filterRemote', function () {
        const st = filterState.get(selector);
        if (st) { st.search = ""; st.page = 1; st.more = true; }
        $sel.find('option:not([value=""])').remove();
        $sel.trigger('change.select2');
    });
}

async function _sl_cascadeFromParent(parentSelector) {
    if (cascadeLock) return;
    cascadeLock = true;
    const safetyTimer = setTimeout(() => { cascadeLock = false; }, 3000);
    try {
        const targets = sl_CascadeMap[parentSelector] || [];
        const val = $(parentSelector).val();
        const hasVal = val !== null && val !== "" && !(Array.isArray(val) && val.length === 0);
        targets.forEach(s => sl_Reset(s));
        if (hasVal) {
            for (const s of targets) {
                const url = filterUrlMap.get(s);
                if (url) await sl_LoadNext(s, url);
            }
        }
    } finally {
        clearTimeout(safetyTimer);
        cascadeLock = false;
    }
}

function sl_InitSingle(selector, url, placeholder, filterType, cascadeTargets, extraOptions, accessCode, readonly) {
    filterState.set(selector, { page: 1, more: true, loading: false, search: "" });
    filterUrlMap.set(selector, url);
    registerFilterSelector(selector, filterType);
    if (cascadeTargets && cascadeTargets.length)
        sl_RegisterCascade(selector, cascadeTargets);
    const $sel = $(selector);
    if ($sel.hasClass('select2-hidden-accessible')) { try { $sel.select2('destroy'); } catch (e) { } }
    $sel.removeAttr('multiple');
    $sel.select2(Object.assign({ placeholder: placeholder || "Select", allowClear: !readonly, width: '100%', minimumResultsForSearch: 0 }, extraOptions || {}));
    if (!readonly) {
        sl_BindOpen(selector, url);
        if (cascadeTargets && cascadeTargets.length)
            $sel.off('change.filterCascade').on('change.filterCascade', function () { _sl_cascadeFromParent(selector); });
    } else {
        $sel.prop('disabled', true);
    }
    if (accessCode) _sl_applyAccessCode(selector, url, accessCode, readonly);
}

function sl_InitMultiple(selector, url, placeholder, filterType, cascadeTargets, extraOptions, accessCode, readonly) {
    filterState.set(selector, { page: 1, more: true, loading: false, search: "" });
    filterUrlMap.set(selector, url);
    registerFilterSelector(selector, filterType);
    if (cascadeTargets && cascadeTargets.length)
        sl_RegisterCascade(selector, cascadeTargets);
    const $sel = $(selector);
    if ($sel.hasClass('select2-hidden-accessible')) { try { $sel.select2('destroy'); } catch (e) { } }
    $sel.attr('multiple', 'multiple');
    $sel.select2(Object.assign({ placeholder: placeholder || "Select", allowClear: !readonly, width: '100%', closeOnSelect: false, minimumResultsForSearch: 0 }, extraOptions || {}));
    if (!readonly) {
        sl_BindOpen(selector, url);
        if (cascadeTargets && cascadeTargets.length)
            $sel.off('change.filterCascade').on('change.filterCascade', function () { _sl_cascadeFromParent(selector); });
    } else {
        $sel.prop('disabled', true);
    }
    if (accessCode) _sl_applyAccessCode(selector, url, accessCode, readonly);
}

async function _sl_applyAccessCode(selector, url, accessCode, readonly) {
    const $sel = $(selector);
    const st = filterState.get(selector);
    if (st) { st.page = 1; st.more = true; st.search = accessCode; }
    const req = buildRequestPayload(1, accessCode);
    req.PageSize = 1;
    try {
        const res = await $.ajax({ url, type: "POST", contentType: "application/json", data: JSON.stringify(req) });
        if (!res || !res.isSuccess) return;
        const items = res.data.items || res.data.Items || [];
        if (!items.length) return;
        const x = items[0];
        const code = x.code || x.Code;
        const name = x.name || x.Name;
        if (!code) return;
        $sel.find('option:not([value=""])').remove();
        $sel.append(new Option(name, code, true, true));
        $sel.val(code).trigger('change.select2');
        if (st) { st.more = false; st.search = ""; }
        if (readonly) $sel.prop('disabled', true);
    } catch (err) {
        console.error(`[SL ACCESS CODE] ${selector}:`, err);
    }
}


// ============================================================
// HOW TO USE
// ============================================================

// ── accessCode = "0005"  →  সব dropdown page load এই populate + disabled ──
// ── accessCode ≠ "0005"  →  Normal, শুধু company auto-select             ──

// $(document).ready(async function () {
//
//     bindRemoteMultiselect("#companySelect",     "/GcAccessFilter/companies",    "Select Company",     "company");
//     bindRemoteMultiselect("#branchSelect",      "/GcAccessFilter/branches",     "Select Branch",      "branch");
//     bindRemoteMultiselect("#divisionSelect",    "/GcAccessFilter/divisions",    "Select Division",    "division");
//     bindRemoteMultiselect("#departmentSelect",  "/GcAccessFilter/departments",  "Select Department",  "department");
//     bindRemoteMultiselect("#designationSelect", "/GcAccessFilter/designations", "Select Designation", "designation");
//     bindRemoteMultiselect("#employeeSelect",    "/GcAccessFilter/employees",    "Select Employee",    null);
//
//     var accessCode = $("#hdnAccessCode").val();
//     var isReadonly  = accessCode === "0005";
//
//     if (isReadonly) {
//         // ── Readonly mode ──
//         // 1. Disabled init (Select All নেই, search নেই, dropdown block)
//         ms_InitializeMultiselects(null, null, true);
//         // 2. Page load এই সব endpoint hit → item inject → auto-selected
//         await ms_ApplyAccessCodeToAll(accessCode);
//     } else {
//         // ── Normal mode ──
//         ms_InitializeMultiselects();
//         ms_BindCascade();
//         ms_Reset("#companySelect");
//         await ms_LoadNext("#companySelect", "/GcAccessFilter/companies");
//         await ms_AutoSelectCompany("001");
//     }
// });