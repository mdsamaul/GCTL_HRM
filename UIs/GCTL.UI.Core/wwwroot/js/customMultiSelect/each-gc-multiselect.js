// ============================================================
// SHARED STATE
// ============================================================
const gcState = new Map();   // selector → { page, more, loading, search }
const gcUrlMap = new Map();   // selector → url
const gcSearchTimers = new Map();

// ============================================================
// FILTER VALUE REGISTRY
// ============================================================
const gcFilterRegistry = {
    company: [],
    branch: [],
    division: [],
    department: [],
    designation: [],
    employeeStatus: [],
};

function gcRegisterSelector(selector, filterType) {
    if (!filterType) return;
    const type = filterType.toLowerCase();
    if (!gcFilterRegistry[type]) gcFilterRegistry[type] = [];
    if (!gcFilterRegistry[type].includes(selector))
        gcFilterRegistry[type].push(selector);
}


// function buildReq(page, search) {
//     function collect(type) {
//         const selectors = gcFilterRegistry[type] || [];
//         const vals = [];
//         selectors.forEach(sel => {
//             const v = $(sel).val();
//             if (!v) return;
//             const arr = Array.isArray(v) ? v : [v];
//             arr.forEach(x => { if (x && !vals.includes(x)) vals.push(x); });
//         });
//         return vals;
//     }
//     return {
//         CompanyCodes: collect('company'),
//         BranchCodes: collect('branch'),
//         DivisionCodes: collect('division'),
//         DepartmentCodes: collect('department'),
//         DesignationCodes: collect('designation'),
//         EmployeeStatuses: collect('employeestatus'),
//         Page: page || 1,
//         PageSize: 10,
//         Search: search || ""
//     };
// }


function buildReq(page, search, extra) {
    function collect(type) {
        const selectors = gcFilterRegistry[type] || [];
        const vals = [];
        selectors.forEach(sel => {
            const v = $(sel).val();
            if (!v) return;
            const arr = Array.isArray(v) ? v : [v];
            arr.forEach(x => { if (x && !vals.includes(x)) vals.push(x); });
        });
        return vals;
    }
    return Object.assign({
        CompanyCodes: collect('company'),
        BranchCodes: collect('branch'),
        DivisionCodes: collect('division'),
        DepartmentCodes: collect('department'),
        DesignationCodes: collect('designation'),
        EmployeeStatuses: collect('employeestatus'),
        Page: page || 1,
        PageSize: 10,
        Search: search || ""
    }, extra || {});
}

function arrVal(selector) {
    const v = $(selector).val();
    if (!v || v.length === 0) return [];
    return Array.isArray(v) ? v : [v];
}

const gcDefaultSelectorTypeMap = {
    '#companySelect': 'company',
    '#branchSelect': 'branch',
    '#divisionSelect': 'division',
    '#departmentSelect': 'department',
    '#designationSelect': 'designation',
    '#activityStatusSelect': 'employeeStatus',
    '#employeeSelect': null, 
};

function gcBindRemoteMultiselect(selector, url, placeholder, filterType) {
    
    gcState.set(selector, { page: 1, more: true, loading: false, search: "" });
    gcUrlMap.set(selector, url);
    $(selector).attr("data-placeholder", placeholder);
    const resolvedType = filterType !== undefined ? filterType : (gcDefaultSelectorTypeMap[selector] ?? null);
    if (resolvedType) gcRegisterSelector(selector, resolvedType);
}

// ============================================================
// CASCADE MAP  (bsms_ filter page)
// ============================================================
const gcCascade = {
    "#companySelect": ["#branchSelect", "#divisionSelect", "#departmentSelect", "#designationSelect", "#employeeSelect"],
    "#branchSelect": ["#divisionSelect", "#departmentSelect", "#designationSelect", "#employeeSelect"],
    "#divisionSelect": ["#departmentSelect", "#designationSelect", "#employeeSelect"],
    "#departmentSelect": ["#designationSelect", "#employeeSelect"],
    "#designationSelect": ["#employeeSelect"],
    "#activityStatusSelect": ["#employeeSelect"]
};

gcDefaultSelectorTypeMap['#activityStatusSelect'] = 'employeeStatus';

let gcCascadeLock = false;



// ╔══════════════════════════════════════════════════════════════╗
// ║         BOOTSTRAP MULTISELECT   (prefix: bsms_)            ║
// ╚══════════════════════════════════════════════════════════════╝

function bsms_GetMenu(selector) {
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

function bsms_SetCss(selector) {
    const $menu = bsms_GetMenu(selector);
    if (!$menu || !$menu[0]) return;
    $menu[0].style.setProperty('max-height', '220px', 'important');
    $menu[0].style.setProperty('overflow-y', 'auto', 'important');
    $menu[0].style.setProperty('overflow-x', 'hidden', 'important');
}

function bsms_UpdateRightClearIcon($select) {
    const $container = $select.next('.btn-group');
    const $btn = $container.find('button.multiselect');
    const selected = $select.find('option:selected').length;
    const isOpen = $btn.hasClass('is-inline-searching');
    if (selected > 0 && !isOpen) $btn.addClass('show-right-clear');
    else $btn.removeClass('show-right-clear');
}

function bsms_Reset(selector) {
    gcState.set(selector, { page: 1, more: true, loading: false, search: "" });
    $(selector).empty();
    try { $(selector).multiselect('rebuild'); } catch (e) { }
}

function bsms_RebuildKeepScroll(selector) {
    const $menu = bsms_GetMenu(selector);
    const scrollTop = $menu ? $menu[0].scrollTop : 0;
    try { $(selector).multiselect('rebuild'); } catch (e) { }
    setTimeout(() => {
        bsms_SetCss(selector);
        const $m = bsms_GetMenu(selector);
        if ($m && $m[0]) $m[0].scrollTop = scrollTop;
        const url = gcUrlMap.get(selector);
        if (url) bsms_BindScroll(selector, url);
    }, 10);
}

async function bsms_LoadNext(selector, url) {
    let st = gcState.get(selector);
    if (!st) { st = { page: 1, more: true, loading: false, search: "" }; gcState.set(selector, st); }
    if (st.loading || !st.more) return;
    st.loading = true;
    const $sel = $(selector);
    const req = buildReq(st.page, st.search);
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
        bsms_RebuildKeepScroll(selector);
    } catch (err) {
        console.error(`[BSMS LOAD] ${selector}:`, err);
    } finally {
        st.loading = false;
    }
}

function bsms_BindScroll(selector, url) {
    const $menu = bsms_GetMenu(selector);
    if (!$menu || !$menu.length) return;
    bsms_SetCss(selector);
    $menu.off("scroll.gcPaging").on("scroll.gcPaging", async function () {
        const dist = this.scrollHeight - this.scrollTop - this.clientHeight;
        if (dist < 100) {
            const st = gcState.get(selector);
            if (st && st.more && !st.loading) await bsms_LoadNext(selector, url);
        }
    });
}

function bsms_BindSearch(selector, url) {
    
    const $sel = $(selector);
    const $btnGroup = $sel.next('.btn-group');
    const $btn = $btnGroup.find('button.multiselect');
    const $inline = $btn.find('input.multiselect-inline-search');
    const $msSearch = $btnGroup.find('input.multiselect-search');
    if (!$inline.length) return;
    $msSearch.off('input keyup change');
    $inline.off('input.gcRemoteSearch').on('input.gcRemoteSearch', function () {
        const term = $(this).val() || "";
        clearTimeout(gcSearchTimers.get(selector));
        gcSearchTimers.set(selector, setTimeout(async () => {
            const st = gcState.get(selector);
            if (!st) return;
            if (st.loading) st.loading = false;
            st.page = 1; st.more = true; st.search = term;
            $sel.empty();
            try { $sel.multiselect('rebuild'); } catch (e) { }
            await bsms_LoadNext(selector, url);
            setTimeout(() => { bsms_SetCss(selector); bsms_BindScroll(selector, url); }, 50);
        }, 350));
    });
}

async function bsms_OnParentChanged(parentSelector) {
    if (gcCascadeLock) return;
    gcCascadeLock = true;
    const safetyTimer = setTimeout(() => { gcCascadeLock = false; }, 2000);
    try {
        const targets = gcCascade[parentSelector] || [];
        const parentHasValue = arrVal(parentSelector).length > 0;

        targets.forEach(s => {
            if (gcState.has(s)) bsms_Reset(s);
        });

        for (const s of targets) {
            const url = gcUrlMap.get(s);
            if (url) await bsms_LoadNext(s, url);
        }

    } finally {
        clearTimeout(safetyTimer);
        gcCascadeLock = false;
    }
}

function bsms_BindCascade() {
    Object.keys(gcCascade).forEach(parent => {
        $(parent).off("change.gcCascade").on("change.gcCascade", () => bsms_OnParentChanged(parent));
    });
}

async function bsms_LoadAllThenSelectAll(selector, url) {
    const st = gcState.get(selector);
    if (!st) return;
    while (st.more) {
        if (st.loading) { await new Promise(r => setTimeout(r, 100)); continue; }
        await bsms_LoadNext(selector, url);
    }
    try { $(selector).multiselect('selectAll', false); $(selector).multiselect('updateButtonText'); } catch (e) { }
}

async function bsms_AutoSelectCompany(code) {
    const selector = "#companySelect";
    const $sel = $(selector);
    if ($sel.find(`option[value="${code}"]`).length === 0)
        await bsms_LoadNext(selector, gcUrlMap.get(selector) || "/GcFilters/company");
    if ($sel.find(`option[value="${code}"]`).length > 0) {
        $sel.val([code]);
        try { $sel.multiselect('rebuild'); } catch (e) { }
        $sel.trigger('change');
    }
}

function bsms_InitializeMultiselects(customConfigs) {

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
        return options.length === 1 ? $(options[0]).text().trim() : `${options.length} items selected`;
    };

    function forceInlineFocus($inline) {
        setTimeout(() => { requestAnimationFrame(() => { try { const el = $inline.get(0); if (el) el.focus(); } catch (e) { } }); }, 0);
    }

    function preventBootstrapToggleWhileTyping($select) {
        const $container = $select.next('.btn-group');
        const $btn = $container.find('button.multiselect');
        const $inline = $btn.find('input.multiselect-inline-search');
        $btn.off('keydown.inlineGuard').on('keydown.inlineGuard', function (e) {
            if (!$btn.hasClass('is-inline-searching')) return;
            if (e.key === ' ' || e.key === 'Spacebar' || e.key === 'Enter') { e.preventDefault(); e.stopPropagation(); }
        });
        $inline.off('.inlineGuard').on('keydown.inlineGuard keypress.inlineGuard keyup.inlineGuard', function (e) {
            if (e.key === 'Escape') { e.stopPropagation(); $btn.dropdown('toggle'); return; }
            if (e.key === ' ' || e.key === 'Spacebar') { if (e.type === 'keyup') e.preventDefault(); e.stopPropagation(); return; }
            if (e.key === 'Enter') { e.preventDefault(); e.stopPropagation(); return; }
            e.stopPropagation();
        });
        $inline.off('mousedown.inlineClick click.inlineClick').on('mousedown.inlineClick click.inlineClick', function (e) { e.stopPropagation(); });
    }

    function ensureClearAllRow($select, selector) {
        const $container = $select.next('.btn-group');
        const $menu = $container.find('ul.multiselect-container');
        if ($menu.find('li.multiselect-clearall').length) return;
        $menu.prepend(
            '<li class="multiselect-item multiselect-clearall">' +
            '<a href="#" class="multiselect-clearall-link" tabindex="0">' +
            '<span class="ca-icon"><i class="fa fa-times-circle"></i></span>' +
            '<span class="ca-text"> Clear all</span></a></li>'
        );
        $menu.off('click.clearAll', '.multiselect-clearall-link').on('click.clearAll', '.multiselect-clearall-link', function (e) {
            e.preventDefault(); e.stopPropagation();
            $select.multiselect('deselectAll', false);
            $select.multiselect('updateButtonText');
            $container.find('button.multiselect input.multiselect-inline-search').val('').trigger('input');
            bsms_UpdateRightClearIcon($select);
            const st = gcState.get(selector);
            if (st) st.search = "";
            $select.trigger('change');
        });
    }

    function ensureRightClearIcon($select, selector) {
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
            bsms_UpdateRightClearIcon($select);
            const st = gcState.get(selector);
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
            includeSelectAllOption: true,
            selectAllText: 'Select All',
            nonSelectedText: placeholder,
            allSelectedText: 'All Selected',
            nSelectedText: 'Selected',
            buttonWidth: '100%',
            maxHeight: 250,
            dropUp: false,
            enableClickableOptGroups: true,
            enableFiltering: true,
            enableCaseInsensitiveFiltering: false,
            filterBehavior: 'text',
            numberDisplayed: 0,
            buttonText: buttonTextFn,
            buttonTitle: buttonTextFn,
            templates: {
                button:
                    '<button type="button" class="multiselect dropdown-toggle" data-toggle="dropdown">' +
                    '<span class="multiselect-selected-text"></span>' +
                    '<input type="text" class="multiselect-inline-search" autocomplete="off" />' +
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
                $btn.find('.multiselect-inline-search').hide();
                $btn.find('.multiselect-selected-text').show();
                ensureRightClearIcon($select, selector);
                bsms_UpdateRightClearIcon($select);
            },
            onDropdownShown: function () {
                const $container = $select.next('.btn-group');
                const $btn = $container.find('button.multiselect');
                const $inline = $btn.find('.multiselect-inline-search');
                const $text = $btn.find('.multiselect-selected-text');
                ensureClearAllRow($select, selector);
                $btn.addClass('is-inline-searching');
                $btn.removeClass('show-right-clear');
                $text.hide();
                $inline.show().val('').attr('placeholder', placeholder);
                const st = gcState.get(selector);
                if (st) st.search = "";
                preventBootstrapToggleWhileTyping($select);
                forceInlineFocus($inline);
                const url = gcUrlMap.get(selector);
                if (url) {
                    setTimeout(() => {
                        bsms_SetCss(selector);
                        bsms_BindScroll(selector, url);
                        bsms_BindSearch(selector, url);
                        const curSt = gcState.get(selector);
                        if ($select.find('option').length === 0 && curSt && curSt.more && !curSt.loading)
                            bsms_LoadNext(selector, url);
                    }, 50);
                }
            },
            onDropdownHidden: function () {
                const $container = $select.next('.btn-group');
                const $btn = $container.find('button.multiselect');
                const $inline = $btn.find('.multiselect-inline-search');
                const $text = $btn.find('.multiselect-selected-text');
                $btn.removeClass('is-inline-searching');
                $inline.val('').hide();
                $text.show();
                bsms_UpdateRightClearIcon($select);
            },
            onSelectAll: function () {
                const url = gcUrlMap.get(selector);
                const st = gcState.get(selector);
                if (url && st && st.more) bsms_LoadAllThenSelectAll(selector, url);
                bsms_UpdateRightClearIcon($select);
            },
            onChange: function () {
                bsms_UpdateRightClearIcon($select);
            }
        });
    });
}



// ╔══════════════════════════════════════════════════════════════╗
// ║              SELECT2    (prefix: s2_)                       ║
// ╚══════════════════════════════════════════════════════════════╝

// ── Cascade map ───────────────────────────────────────────────
const s2_CascadeMap = {};   // { parentSelector: [childSelector, ...] }

function s2_RegisterCascade(parentSelector, childSelectors) {
    s2_CascadeMap[parentSelector] = childSelectors;
}

// ── Reset ─────────────────────────────────────────────────────
function s2_Reset(selector) {
    gcState.set(selector, { page: 1, more: true, loading: false, search: "" });
    $(selector).find('option:not([value=""])').remove();
    $(selector).val(null).trigger('change.select2');
}

// ── Inject items into open Select2 dropdown UI ───────────────

function _s2_injectIntoOpenDropdown(selector, items) {
    const $resultsList = $(document).find('.select2-results__options');
    if (!$resultsList.length) return; 

    $resultsList.find('.gc-loading-item').remove();

    items.forEach(x => {
        const code = x.code || x.Code;
        const name = x.name || x.Name;
        if (!code) return;
        if ($resultsList.find(`li[data-s2-val="${CSS.escape(code)}"]`).length) return;

        const $li = $(`<li class="select2-results__option" role="option" data-s2-val="${code}" style="cursor:pointer">${name}</li>`);

        $li.on('mousedown', function (e) {
            e.preventDefault();
            e.stopPropagation();
            const $sel = $(selector);
            const isMultiple = $sel.prop('multiple');
            if (isMultiple) {
                const current = $sel.val() || [];
                const idx = current.indexOf(code);
                if (idx === -1) {
                    current.push(code);
                    $li.addClass('select2-results__option--selected');
                } else {
                    current.splice(idx, 1);
                    $li.removeClass('select2-results__option--selected');
                }
                $sel.val(current).trigger('change');
            } else {
                $sel.val(code).trigger('change');
                $sel.select2('close');
            }
        });

        $resultsList.append($li);
    });
}

// ── Load next page ────────────────────────────────────────────
// async function s2_LoadNext(selector, url) {
//     let st = gcState.get(selector);
//     if (!st) { st = { page: 1, more: true, loading: false, search: "" }; gcState.set(selector, st); }
//     if (st.loading || !st.more) return;
//     st.loading = true;

//     const $resultsList = $(document).find('.select2-results__options');
//     if ($resultsList.length && !$resultsList.find('.gc-loading-item').length) {
//         $resultsList.append('<li class="select2-results__option gc-loading-item" style="color:#999;font-style:italic">Loading...</li>');
//     }

//     const req = buildReq(st.page, st.search);
//     try {
//         const res = await $.ajax({ url, type: "POST", contentType: "application/json", data: JSON.stringify(req) });
//         if (!res || !res.isSuccess) return;
//         const items = res.data.items || res.data.Items || [];
//         const more = res.data.more ?? res.data.More ?? false;
//         st.page++; st.more = more;

//         const $sel = $(selector);
//         const newItems = [];
//         items.forEach(x => {
//             const code = x.code || x.Code;
//             const name = x.name || x.Name;
//             if (!code) return;
//             if ($sel.find(`option[value="${code}"]`).length === 0) {
//                 $sel.append(new Option(name, code, false, false));
//                 newItems.push(x);
//             }
//         });

//         const isDropdownOpen = $(document).find('.select2-results__options').length > 0;
//         if (isDropdownOpen) {
//             _s2_injectIntoOpenDropdown(selector, newItems);
//         }
//         $sel.trigger('change.select2');

//     } catch (err) {
//         console.error(`[S2 LOAD] ${selector}:`, err);
//         $(document).find('.select2-results__options .gc-loading-item').remove();
//     } finally {
//         st.loading = false;
//         $(document).find('.select2-results__options .gc-loading-item').remove();
//     }
// }


async function s2_LoadNext(selector, url) {
    let st = gcState.get(selector);
    if (!st) { st = { page: 1, more: true, loading: false, search: "", extra: {} }; gcState.set(selector, st); }
    if (st.loading || !st.more) return;
    st.loading = true;

    const $resultsList = $(document).find('.select2-results__options');
    if ($resultsList.length && !$resultsList.find('.gc-loading-item').length) {
        $resultsList.append('<li class="select2-results__option gc-loading-item" style="color:#999;font-style:italic">Loading...</li>');
    }

    const req = buildReq(st.page, st.search, st.extra);
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

        const isDropdownOpen = $(document).find('.select2-results__options').length > 0;
        if (isDropdownOpen) _s2_injectIntoOpenDropdown(selector, newItems);
        $sel.trigger('change.select2');
    } catch (err) {
        console.error(`[S2 LOAD] ${selector}:`, err);
        $(document).find('.select2-results__options .gc-loading-item').remove();
    } finally {
        st.loading = false;
        $(document).find('.select2-results__options .gc-loading-item').remove();
    }
}

// ── Scroll bind ───────────────────────────────────────────────
function s2_BindScroll(selector, url) {
    const $resultsList = $(document).find('.select2-results__options');
    if (!$resultsList.length) return;
    $resultsList.off('scroll.gcPaging').on('scroll.gcPaging', async function () {
        const dist = this.scrollHeight - this.scrollTop - this.clientHeight;
        if (dist < 80) {
            const st = gcState.get(selector);
            if (st && st.more && !st.loading) await s2_LoadNext(selector, url);
        }
    });
}

// ── Search bind ───────────────────────────────────────────────
function s2_BindSearch(selector, url) {
    const $searchInput = $(document).find('.select2-search__field');
    if (!$searchInput.length) return;
    $searchInput.off('input.gcRemoteSearch').on('input.gcRemoteSearch', function () {
        const term = $(this).val() || "";
        clearTimeout(gcSearchTimers.get(selector));
        gcSearchTimers.set(selector, setTimeout(async () => {
            const st = gcState.get(selector);
            if (!st) return;
            if (st.loading) st.loading = false;
            st.page = 1; st.more = true; st.search = term;
            $(selector).find('option:not([value=""])').remove();
            $(document).find(".select2-results__options").empty();
            $(selector).trigger('change.select2');
            await s2_LoadNext(selector, url);
            setTimeout(() => s2_BindScroll(selector, url), 50);
        }, 350));
    });
}

// ── Open / Clear ──────────────────────────────────────────────
function s2_BindOpen(selector, url) {
    if (!url) return;
    const $sel = $(selector);

    $sel.off('select2:open.gcRemote').on('select2:open.gcRemote', function () {
        setTimeout(() => {
            s2_BindScroll(selector, url);
            s2_BindSearch(selector, url);

            // ── No results message hide ──
            $(document).find('.select2-results__message').hide();

            const st = gcState.get(selector);
            if (!st || st.loading) return;

            const $resultsList = $(document).find('.select2-results__options');
            const visibleItems = $resultsList.find('li[data-s2-val]').length;

            if (visibleItems === 0 && st.more) {
                if (st.page === 1) {
                    s2_LoadNext(selector, url);
                } else {
                    const existingItems = [];
                    $sel.find('option:not([value=""])').each(function () {
                        existingItems.push({ code: $(this).val(), name: $(this).text() });
                    });
                    if (existingItems.length) {
                        _s2_injectIntoOpenDropdown(selector, existingItems);
                    } else {
                        s2_LoadNext(selector, url);
                    }
                }
            }
        }, 150);
    });

    $sel.off('select2:clear.gcRemote').on('select2:clear.gcRemote', function () {
        const st = gcState.get(selector);
        if (st) { st.search = ""; st.page = 1; st.more = true; }
        $sel.find('option:not([value=""])').remove();
        $sel.trigger('change.select2');
    });
}

// ── Internal cascade ──────────────────────────────────────────
async function _s2_cascadeFromParent(parentSelector) {
    if (gcCascadeLock) return;
    gcCascadeLock = true;
    const safetyTimer = setTimeout(() => { gcCascadeLock = false; }, 3000);
    try {
        const targets = s2_CascadeMap[parentSelector] || [];
        const val = $(parentSelector).val();
        const hasVal = val !== null && val !== "" && !(Array.isArray(val) && val.length === 0);
        targets.forEach(s => s2_Reset(s));
        if (hasVal) {
            for (const s of targets) {
                const url = gcUrlMap.get(s);
                if (url) await s2_LoadNext(s, url);
            }
        }
    } finally {
        clearTimeout(safetyTimer);
        gcCascadeLock = false;
    }
}

// ── SINGLE select init ────────────────────────────────────────
/**
 * @param {string}   selector       "#OfficialInfoCompanyCode"
 * @param {string}   url            "/GcFilters/company"
 * @param {string}   placeholder    "Select Company"
 * @param {string}   filterType     "company" | "branch" | "division" | "department" | "designation" | "employeeStatus"
 * @param {string[]} cascadeTargets ["#OfficialInfoBranchCode", "#departmentSelect"]  (optional)
 * @param {object}   extraOptions   extra Select2 options  (optional)
 */

// function s2_InitSingle(selector, url, placeholder, filterType, cascadeTargets, extraOptions) {
//     gcState.set(selector, { page: 1, more: true, loading: false, search: "" });
//     gcUrlMap.set(selector, url);
//     gcRegisterSelector(selector, filterType);

//     if (cascadeTargets && cascadeTargets.length)
//         s2_RegisterCascade(selector, cascadeTargets);

//     const $sel = $(selector);
//     if ($sel.hasClass('select2-hidden-accessible')) {
//         try { $sel.select2('destroy'); } catch (e) { }
//     }
//     $sel.removeAttr('multiple');

//     $sel.select2(Object.assign({
//         placeholder: placeholder || "Select",
//         allowClear: true,
//         width: '100%',
//         minimumResultsForSearch: 0,
//         language: {
//             searching: function () {
//                 return "Loading...";
//             },
//             noResults: function () {
//                 let st = gcState.get(selector);
//                 if (st && st.loading) return "Loading...";
//                 return "No data found";
//             }
//         }
//     }, extraOptions || {}));

//     $sel.on('select2:opening', function (e) {
//         let st = gcState.get(selector);
//         if (st && st.loading) {
//             e.preventDefault();
//         }
//     });
//     s2_BindOpen(selector, url);

//     if (cascadeTargets && cascadeTargets.length) {
//         $sel.off('change.gcCascade').on('change.gcCascade', function () {
//             _s2_cascadeFromParent(selector);
//         });
//     }
// }


function s2_InitSingle(selector, url, placeholder, filterType, cascadeTargets, extraOptions, extraReqParams) {
    gcState.set(selector, { page: 1, more: true, loading: false, search: "", extra: extraReqParams || {} });
    gcUrlMap.set(selector, url);
    gcRegisterSelector(selector, filterType);

    if (cascadeTargets && cascadeTargets.length)
        s2_RegisterCascade(selector, cascadeTargets);

    const $sel = $(selector);
    if ($sel.hasClass('select2-hidden-accessible')) {
        try { $sel.select2('destroy'); } catch (e) { }
    }
    $sel.removeAttr('multiple');

    $sel.select2(Object.assign({
        placeholder: placeholder || "Select",
        allowClear: true,
        width: '100%',
        minimumResultsForSearch: 0,
        language: {
            searching: function () { return "Loading..."; },
            noResults: function () {
                let st = gcState.get(selector);
                if (st && st.loading) return "Loading...";
                return "No data found";
            }
        }
    }, extraOptions || {}));

    $sel.on('select2:opening', function (e) {
        let st = gcState.get(selector);
        if (st && st.loading) e.preventDefault();
    });
    s2_BindOpen(selector, url);

    if (cascadeTargets && cascadeTargets.length) {
        $sel.off('change.gcCascade').on('change.gcCascade', function () {
            _s2_cascadeFromParent(selector);
        });
    }
}


// ── MULTIPLE select init ──────────────────────────────────────
/**
 * @param {string}   selector       "#companySelect"
 * @param {string}   url            "/GcFilters/company"
 * @param {string}   placeholder    "Select Company"
 * @param {string}   filterType     "company" | "branch" | ...
 * @param {string[]} cascadeTargets (optional)
 * @param {object}   extraOptions   (optional)
 */
function s2_InitMultiple(selector, url, placeholder, filterType, cascadeTargets, extraOptions) {
    gcState.set(selector, { page: 1, more: true, loading: false, search: "" });
    gcUrlMap.set(selector, url);
    gcRegisterSelector(selector, filterType);

    if (cascadeTargets && cascadeTargets.length)
        s2_RegisterCascade(selector, cascadeTargets);

    const $sel = $(selector);
    if ($sel.hasClass('select2-hidden-accessible')) {
        try { $sel.select2('destroy'); } catch (e) { }
    }
    $sel.attr('multiple', 'multiple');

    $sel.select2(Object.assign({
        placeholder: placeholder || "Select",
        allowClear: true,
        width: '100%',
        closeOnSelect: false,
        minimumResultsForSearch: 0,
        language: {
            searching: function () {
                return "Loading...";
            },
            noResults: function () {
                return "No data found";
            }
        }
    }, extraOptions || {}));

    s2_BindOpen(selector, url);

    if (cascadeTargets && cascadeTargets.length) {
        $sel.off('change.gcCascade').on('change.gcCascade', function () {
            _s2_cascadeFromParent(selector);
        });
    }
}

// ── Default filter selects ────────────────────────────────────
function s2_InitializeMultiselects(customConfigs) {
    const defaultConfigs = [
        { selector: '#companySelect', url: '/GcFilters/company', placeholder: 'Select Company', filterType: 'company', cascade: ['#branchSelect', '#divisionSelect', '#departmentSelect', '#designationSelect', '#employeeSelect'] },
        { selector: '#branchSelect', url: '/GcFilters/branch', placeholder: 'Select Branch', filterType: 'branch', cascade: ['#divisionSelect', '#departmentSelect', '#designationSelect', '#employeeSelect'] },
        { selector: '#divisionSelect', url: '/GcFilters/division', placeholder: 'Select Division', filterType: 'division', cascade: ['#departmentSelect', '#designationSelect', '#employeeSelect'] },
        { selector: '#departmentSelect', url: '/GcFilters/department', placeholder: 'Select Department', filterType: 'department', cascade: ['#designationSelect', '#employeeSelect'] },
        { selector: '#designationSelect', url: '/GcFilters/designation', placeholder: 'Select Designation', filterType: 'designation', cascade: ['#employeeSelect'] },
        { selector: '#employeeSelect', url: '/GcFilters/employee', placeholder: 'Select Employee', filterType: 'employeeStatus', cascade: [] },
    ];
    const configs = customConfigs || defaultConfigs;
    configs.forEach(cfg => {
        if (!$(cfg.selector).length) return;
        s2_InitMultiple(cfg.selector, cfg.url, cfg.placeholder, cfg.filterType, cfg.cascade);
    });
}

// ── Cascade bind (filter page — gcCascade map) ───────────────
async function s2_OnParentChanged(parentSelector) {
    if (gcCascadeLock) return;
    gcCascadeLock = true;
    const safetyTimer = setTimeout(() => { gcCascadeLock = false; }, 2000);
    try {
        const targets = gcCascade[parentSelector] || [];
        const parentHasValue = arrVal(parentSelector).length > 0;
        targets.forEach(s => s2_Reset(s));
        if (parentHasValue) {
            for (const s of targets) {
                const url = gcUrlMap.get(s);
                if (url) await s2_LoadNext(s, url);
            }
        }
    } finally {
        clearTimeout(safetyTimer);
        gcCascadeLock = false;
    }
}

function s2_BindCascade() {
    Object.keys(gcCascade).forEach(parent => {
        $(parent).off("change.gcCascade").on("change.gcCascade", () => s2_OnParentChanged(parent));
    });
}

// ── Select All ────────────────────────────────────────────────
async function s2_LoadAllThenSelectAll(selector, url) {
    const st = gcState.get(selector);
    if (!st) return;
    while (st.more) {
        if (st.loading) { await new Promise(r => setTimeout(r, 100)); continue; }
        await s2_LoadNext(selector, url);
    }
    try {
        $(selector).find('option').prop('selected', true);
        $(selector).trigger('change.select2');
    } catch (e) { }
}

//async function s2_AutoSelectCompany(code) {
//    const selector = "#companySelect";
//    const $sel = $(selector);
//    const url = gcUrlMap.get(selector);
//    if ($sel.find(`option[value="${code}"]`).length === 0 && url)
//        await s2_LoadNext(selector, url);
//    if ($sel.find(`option[value="${code}"]`).length > 0)
//        $sel.val([code]).trigger('change');
//}

async function s2_AutoSelectCompany(code, selector = "#companySelect") {

    const $sel = $(selector);
    const url = gcUrlMap.get(selector);

    if ($sel.find(`option[value="${code}"]`).length === 0 && url) {
        await s2_LoadNext(selector, url);
    }

    if ($sel.find(`option[value="${code}"]`).length > 0) {
        $sel.val(code).trigger("change");
    }
}

/**
 * Edit page এ default value set
 * @param {string}  selector        "#OfficialInfoCompanyCode"
 * @param {string}  code            value to select
 * @param {string}  name            display text
 * @param {boolean} triggerCascade  true → children reload হবে (default: true)
 */
async function s2_SetDefault(selector, code, name, triggerCascade) {
    if (!code) return;
    const $sel = $(selector);

    if ($sel.find(`option[value="${code}"]`).length === 0)
        $sel.append(new Option(name || code, code, true, true));

    $sel.val(code).trigger('change.select2');

    if (triggerCascade !== false)
        await _s2_cascadeFromParent(selector);
}



// ============================================================
// HOW TO USE
// ============================================================
//
// ── Filter page — Bootstrap Multiselect ──────────────────────
// $(document).ready(async function () {
//     gcBindRemoteMultiselect("#companySelect",     "/GcFilters/company",     "Select Company",     "company");
//     gcBindRemoteMultiselect("#branchSelect",      "/GcFilters/branch",      "Select Branch",      "branch");
//     gcBindRemoteMultiselect("#divisionSelect",    "/GcFilters/division",    "Select Division",    "division");
//     gcBindRemoteMultiselect("#departmentSelect",  "/GcFilters/department",  "Select Department",  "department");
//     gcBindRemoteMultiselect("#designationSelect", "/GcFilters/designation", "Select Designation", "designation");
//     gcBindRemoteMultiselect("#employeeSelect",    "/GcFilters/employee",    "Select Employee",    "employeeStatus");
//     bsms_InitializeMultiselects();
//     bsms_BindCascade();
//     bsms_Reset("#companySelect");
//     await bsms_LoadNext("#companySelect", "/GcFilters/company");
//     await bsms_AutoSelectCompany("001");
// });
//
// ── Filter page — Select2 multiple ───────────────────────────
// $(document).ready(async function () {
//     s2_InitializeMultiselects();   // সব default config এ init + cascade bind
//     await s2_LoadNext("#companySelect", "/GcFilters/company");
//     await s2_AutoSelectCompany("001");
// });
//
// ── Custom page — Select2 SINGLE with cascade + default ───────
// $(document).ready(async function () {
//
//     s2_InitSingle("#OfficialInfoCompanyCode", "/GcFilters/company", "Select Company", "company",
//         ["#OfficialInfoBranchCode", "#OfficialInfoDepartmentCode"]
//     );
//     s2_InitSingle("#OfficialInfoBranchCode", "/GcFilters/branch", "Select Branch", "branch",
//         ["#OfficialInfoDepartmentCode"]
//     );
//     s2_InitSingle("#OfficialInfoDepartmentCode", "/GcFilters/department", "Select Department", "department");
//
//     // ── Edit/Create page: default value set ──
//     var companyCode = "@Model.CompanyCode";
//     var companyName = "@Model.CompanyName";
//     var branchCode  = "@Model.BranchCode";
//     var branchName  = "@Model.BranchName";
//
//     if (companyCode) {
//         // company set → cascade trigger → branch list load হবে
//         await s2_SetDefault("#OfficialInfoCompanyCode", companyCode, companyName, true);
//         // branch set → cascade trigger → department list load হবে
//         if (branchCode)
//             await s2_SetDefault("#OfficialInfoBranchCode", branchCode, branchName, true);
//     }
// });