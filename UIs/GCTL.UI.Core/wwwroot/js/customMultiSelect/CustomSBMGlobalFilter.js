

(function ($) {
    'use strict';

    // ═══════════════════════════════════════════════════════════════
    //  HELPER
    // ═══════════════════════════════════════════════════════════════
    function debounce(fn, wait) {
        let t = null;
        return function () {
            const ctx = this, args = arguments;
            clearTimeout(t);
            t = setTimeout(() => fn.apply(ctx, args), wait);
        };
    }

    // ═══════════════════════════════════════════════════════════════
    //  DESTROY HELPERS
    // ═══════════════════════════════════════════════════════════════
    function gcDestroyMultiselect($select) {
        try {
            if ($select.data('multiselect')) {
                $select.multiselect('destroy');
            }
        } catch (e) { }
        $select.next('.btn-group').remove();
        $select.removeData('gcMsInit');
        $select.removeData('multiselect-init');
        $select.removeData('gcRemote-uid');
    }

    function gcDestroySelect2($el) {
        try {
            if ($el.data('select2')) $el.select2('destroy');
        } catch (e) { }
        $el.siblings('.select2-container').remove();
        $el.nextAll('.select2-container').remove();
        $el.removeData('gcSelect2Init');
    }

    // ═══════════════════════════════════════════════════════════════
    //  STANDARD MULTISELECT  (gc-multiselect class)
    // ═══════════════════════════════════════════════════════════════
    function initializeMultiselects(selectorOrConfig) {
        if (typeof $.fn.multiselect === 'undefined') return;

        let $targets = $();

        if (typeof selectorOrConfig === 'string') {
            $targets = $(selectorOrConfig);
        } else if (selectorOrConfig instanceof jQuery) {
            $targets = selectorOrConfig;
        } else if (typeof selectorOrConfig === 'object' && selectorOrConfig) {
            $.each(selectorOrConfig, function (selector, placeholder) {
                const $sel = $(selector);
                if ($sel.length) $sel.attr('data-placeholder', placeholder);
            });
            $targets = $(Object.keys(selectorOrConfig).join(','));
        } else {
            $targets = $('.gc-multiselect');
        }

        initializeSelectElements($targets);
    }

    function initializeSelectElements($targets) {

        const buttonTextFn = function (options, select) {
            const $select = $(select);
            const placeholder = $select.attr('data-placeholder') || 'Select';
            const id = $select.attr('id');
            const isMultiple = $select.prop('multiple');

            if (!options || options.length === 0) return placeholder;
            if (!isMultiple) return $(options[0]).text().trim();

            if (id === 'companySelect') {
                if (options.length === 1) return $(options[0]).text().trim();
                return `${options.length} items selected`;
            }

            return options.length === 1 ? '1 item selected' : `${options.length} items selected`;
        };

        // function forceInlineFocus($inline) {
        //     try { $inline.trigger('focus'); } catch (e) { }
        //     setTimeout(() => {
        //         try { $inline.trigger('focus'); } catch (e) { }
        //         requestAnimationFrame(() => {
        //             try {
        //                 const el = $inline.get(0);
        //                 if (el) {
        //                     el.focus();
        //                     const len = el.value.length;
        //                     if (el.setSelectionRange) el.setSelectionRange(len, len);
        //                 }
        //             } catch (e) { }
        //         });
        //     }, 0);
        // }
        function forceInlineFocus($inline) {
            try { $inline.trigger('focus'); } catch (e) { }
            setTimeout(() => {
                try { $inline.trigger('focus'); } catch (e) { }
                requestAnimationFrame(() => {
                    try {
                        const el = $inline.get(0);
                        if (el) {
                            el.focus();
                            // setSelectionRange bad dewa hoyeche — mouse drag-select e
                            // badha dichilo.
                        }
                    } catch (e) { }
                });
            }, 0);
        }

        function preventBootstrapToggleWhileTyping($select) {
            const $container = $select.next('.btn-group');
            const $btn = $container.find('button.multiselect');
            const $inline = $btn.find('input.multiselect-inline-search');

            $btn.off('keydown.gcInlineGuard').on('keydown.gcInlineGuard', function (e) {
                if (!$btn.hasClass('is-inline-searching')) return;
                if (e.key === ' ' || e.key === 'Spacebar' || e.key === 'Enter') {
                    e.preventDefault();
                    e.stopPropagation();
                }
            });

            $inline.off('.gcInlineGuard').on('keydown.gcInlineGuard keypress.gcInlineGuard keyup.gcInlineGuard', function (e) {
                if (e.key === 'Escape') {
                    e.stopPropagation();
                    $btn.dropdown('toggle');
                    return;
                }
                if (e.key === ' ' || e.key === 'Spacebar') {
                    if (e.type === 'keyup') e.preventDefault();
                    e.stopPropagation();
                    return;
                }
                if (e.key === 'Enter') {
                    e.preventDefault();
                    e.stopPropagation();
                    return;
                }
                e.stopPropagation();
            });

            $inline.off('mousedown.gcInlineClick click.gcInlineClick')
                .on('mousedown.gcInlineClick click.gcInlineClick', function (e) { e.stopPropagation(); });
        }

        function ensureClearAllRow($select) {
            const $container = $select.next('.btn-group');
            const $menu = $container.find('ul.multiselect-container');
            if ($menu.find('li.multiselect-clearall').length) return;

            const html =
                '<li class="multiselect-item multiselect-clearall">' +
                '  <a href="#" class="multiselect-clearall-link" tabindex="0">' +
                '    <span class="ca-x">×</span>' +
                '    <span class="ca-text"> Clear all</span>' +
                '  </a>' +
                '</li>';
            $menu.prepend(html);

            $menu.off('click.gcClearAll', '.multiselect-clearall-link')
                .on('click.gcClearAll', '.multiselect-clearall-link', function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    if ($select.prop('multiple')) {
                        $select.multiselect('deselectAll', false);
                    } else {
                        $select.val('').trigger('change');
                    }

                    $select.multiselect('updateButtonText');
                    $container.find('input.multiselect-search').val('').trigger('input').trigger('keyup');
                    $container.find('button.multiselect input.multiselect-inline-search').val('').trigger('input');
                    updateRightClearIcon($select);

                    setTimeout(() => {
                        const $btn = $container.find('button.multiselect');
                        if ($btn.hasClass('is-inline-searching')) {
                            forceInlineFocus($btn.find('input.multiselect-inline-search'));
                        }
                    }, 0);
                });
        }

        function ensureRightClearIcon($select) {
            const $container = $select.next('.btn-group');
            const $btn = $container.find('button.multiselect');

            $btn.find('.ms-right-clear').not(':first').remove();

            let $right = $btn.find('.ms-right-clear');
            if ($right.length) return;

            $right = $('<span class="ms-right-clear" title="Clear"><span class="ms-x">×</span></span>');

            const $caret = $btn.find('b.caret');
            if ($caret.length) $right.insertBefore($caret);
            else $btn.append($right);

            $right.off('.gcRightClear').on('mousedown.gcRightClear click.gcRightClear', function (e) {
                e.preventDefault();
                e.stopPropagation();

                if ($select.prop('multiple')) {
                    $select.multiselect('deselectAll', false);
                } else {
                    $select.val('').trigger('change');
                }

                $select.multiselect('updateButtonText');
                $container.find('input.multiselect-search').val('').trigger('input').trigger('keyup');
                $btn.find('input.multiselect-inline-search').val('').trigger('input');
                updateRightClearIcon($select);
            });
        }

        function updateRightClearIcon($select) {
            const $container = $select.next('.btn-group');
            const $btn = $container.find('button.multiselect');
            ensureRightClearIcon($select);

            const selectedCount = $select.find('option:selected').length;
            const isOpen = $btn.hasClass('is-inline-searching');

            $btn.removeClass('show-right-clear');
            if (selectedCount > 0 && !isOpen) {
                $btn.addClass('show-right-clear');
            }
        }

        $targets.each(function () {
            const $select = $(this);

            if ($select.data('gcMsInit') === true && $select.data('multiselect')) return;

            const isMultiple = $select.prop('multiple');
            const placeholder =
                $select.attr('data-placeholder') ||
                $select.data('placeholder') ||
                $select.attr('placeholder') ||
                'Select';

            $select.attr('data-placeholder', placeholder);

            if (!isMultiple) {
                if ($select.find('option[value=""]').length === 0) {
                    $select.prepend('<option value=""></option>');
                }
                $select.val('');
                $select.find('option').prop('selected', false);
            }

            if ($select.data('multiselect') || $select.next('.btn-group').length) {
                gcDestroyMultiselect($select);
            }

            $select.multiselect({
                includeSelectAllOption: isMultiple,
                selectAllText: 'Select All',
                nonSelectedText: placeholder,
                allSelectedText: 'All Selected',
                nSelectedText: 'Selected',
                buttonWidth: '100%',
                maxHeight: 250,
                dropUp: false,
                enableClickableOptGroups: true,
                enableFiltering: true,
                enableCaseInsensitiveFiltering: true,
                numberDisplayed: 0,
                buttonText: buttonTextFn,
                buttonTitle: buttonTextFn,

                templates: {
                    button:
                        '<button type="button" class="multiselect dropdown-toggle" data-toggle="dropdown">' +
                        '  <span class="multiselect-selected-text"></span>' +
                        '  <input type="text" class="multiselect-inline-search" autocomplete="off" />' +
                        '  <b class="caret"></b>' +
                        '</button>',
                    ul: '<ul class="multiselect-container dropdown-menu"></ul>',
                    filter:
                        '<li class="multiselect-item multiselect-filter">' +
                        '  <div class="input-group">' +
                        '    <div class="input-group-prepend"><span class="input-group-text"><i class="fa fa-search"></i></span></div>' +
                        '    <input class="form-control multiselect-search" type="text" />' +
                        '  </div>' +
                        '</li>',
                    li: '<li><a tabindex="0"><label class="checkbox"></label></a></li>',
                    divider: '<li class="multiselect-item divider"></li>',
                    liGroup: '<li class="multiselect-item multiselect-group"><label></label></li>'
                },

                onInitialized: function () {
                    const $container = $select.next('.btn-group');
                    const $btn = $container.find('button.multiselect');
                    $btn.find('.multiselect-inline-search').hide();
                    $btn.find('.multiselect-selected-text').show();
                    if (!isMultiple) $container.addClass('single-select');
                    ensureRightClearIcon($select);
                    updateRightClearIcon($select);
                    $select.data('gcMsInit', true);
                },

                onDropdownShown: function () {
                    const $container = $select.next('.btn-group');
                    const $btn = $container.find('button.multiselect');
                    const $inline = $btn.find('.multiselect-inline-search');
                    const $text = $btn.find('.multiselect-selected-text');
                    const $dropdownSearch = $container.find('input.multiselect-search');

                    ensureClearAllRow($select);
                    $btn.addClass('is-inline-searching');
                    $btn.removeClass('show-right-clear');
                    $text.hide();
                    $inline.show().val('').attr('placeholder', placeholder);
                    $dropdownSearch.val('').trigger('input').trigger('keyup');

                    $inline.off('.gcMultiselectInline').on('input.gcMultiselectInline', function (e) {
                        e.stopPropagation();
                        const term = $(this).val();
                        $dropdownSearch.val(term).trigger('input').trigger('keyup').trigger('change');
                    });

                    preventBootstrapToggleWhileTyping($select);
                    setTimeout(() => forceInlineFocus($inline), 0);
                },

                onDropdownHidden: function () {
                    const $container = $select.next('.btn-group');
                    const $btn = $container.find('button.multiselect');
                    const $inline = $btn.find('.multiselect-inline-search');
                    const $text = $btn.find('.multiselect-selected-text');
                    const $dropdownSearch = $container.find('input.multiselect-search');

                    $dropdownSearch.val('').trigger('input').trigger('keyup');
                    $btn.removeClass('is-inline-searching');
                    $inline.val('').hide();
                    $text.show();
                    updateRightClearIcon($select);
                },

                onChange: function () {
                    updateRightClearIcon($select);
                }
            });
        });
    }

    // ═══════════════════════════════════════════════════════════════
    //  SELECT2
    // ═══════════════════════════════════════════════════════════════
    function initInlineSearchSelect2(selectorOrElements) {
        const $targets = (selectorOrElements instanceof jQuery)
            ? selectorOrElements
            : $(selectorOrElements || '.gc-select2');

        $targets.each(function () {
            const $el = $(this);

            if ($el.data('gcSelect2Init') === true && $el.data('select2')) return;

            if ($el.data('select2') || $el.siblings('.select2-container').length || $el.nextAll('.select2-container').length) {
                gcDestroySelect2($el);
            }

            const placeholder = $el.data('placeholder') || $el.attr('data-placeholder') || 'Search...';

            const $wrap =
                $el.closest('.dropdown-wrapper-select2').length ? $el.closest('.dropdown-wrapper-select2')
                    : $el.closest('.gc-inline-select2').length ? $el.closest('.gc-inline-select2')
                        : $el.closest('.input-group').length ? $el.closest('.input-group')
                            : $(document.body);

            $el.select2({
                width: '100%',
                dropdownParent: $wrap,
                allowClear: true,
                minimumResultsForSearch: 0,
                placeholder: { id: '', text: placeholder }
            });

            $el.data('gcSelect2Init', true);
            $el.off('.gcInline');

            // $el.on('select2:open.gcInline', function () {
            //     const s2 = $el.data('select2');
            //     if (!s2) return;
            //     const $container = s2.$container;
            //     const $dropdown = s2.$dropdown;
            //     if (!$container || !$dropdown) return;

            //     const $selection = $container.find('.select2-selection--single');
            //     const $searchWrap = $dropdown.find('.select2-search--dropdown');
            //     const $searchField = $searchWrap.find('.select2-search__field');

            //     $selection.addClass('gc-inline-search-active');
            //     $searchWrap.addClass('gc-inline-search').appendTo($selection);
            //     $searchField.attr('placeholder', placeholder);

            //     $searchField.off('keydown.gcSpace').on('keydown.gcSpace', function (e) {
            //         if (e.key === ' ' || e.keyCode === 32) {
            //             e.stopPropagation();
            //             var el = this;
            //             var start = el.selectionStart;
            //             var end = el.selectionEnd;
            //             var val = el.value;
            //             el.value = val.substring(0, start) + ' ' + val.substring(end);
            //             el.selectionStart = el.selectionEnd = start + 1;
            //             $(el).trigger('input');
            //             e.preventDefault();
            //         }
            //     });

            //     // ── NEW: age select kora item-er text search field e pre-fill ──
            //     const $selectedOption = $el.find('option:selected');
            //     const selectedText = ($selectedOption.length && $selectedOption.val())
            //         ? $selectedOption.text().trim()
            //         : '';
            //     $searchField.val(selectedText);
            //     // setSelectionRange kora hocche na — mouse drag-select e badha
            //     // dey na, ar user cheile shurute o click kore cursor boshate parbe

            //     $searchField[0].style.width = '100%';
            //     setTimeout(function () { $searchField.trigger('focus'); }, 0);
            // });
            $el.on('select2:open.gcInline', function () {
                const s2 = $el.data('select2');
                if (!s2) return;
                const $container = s2.$container;
                const $dropdown = s2.$dropdown;
                if (!$container || !$dropdown) return;

                const $selection = $container.find('.select2-selection--single');
                const $searchWrap = $dropdown.find('.select2-search--dropdown');
                const $searchField = $searchWrap.find('.select2-search__field');

                $selection.addClass('gc-inline-search-active');
                $searchWrap.addClass('gc-inline-search').appendTo($selection);
                $searchField.attr('placeholder', placeholder);

                // ── NEW: search field e click/mousedown korle dropdown toggle
                //         hoye bondho hoye jachhilo — eta আটকে দেওয়া হলো ──
                $searchWrap.off('mousedown.gcInlineClick click.gcInlineClick')
                    .on('mousedown.gcInlineClick click.gcInlineClick', function (e) {
                        e.stopPropagation();
                    });

                $searchField.off('keydown.gcSpace').on('keydown.gcSpace', function (e) {
                    if (e.key === ' ' || e.keyCode === 32) {
                        e.stopPropagation();
                        var el = this;
                        var start = el.selectionStart;
                        var end = el.selectionEnd;
                        var val = el.value;
                        el.value = val.substring(0, start) + ' ' + val.substring(end);
                        el.selectionStart = el.selectionEnd = start + 1;
                        $(el).trigger('input');
                        e.preventDefault();
                    }
                });

                const $selectedOption = $el.find('option:selected');
                const selectedText = ($selectedOption.length && $selectedOption.val())
                    ? $selectedOption.text().trim()
                    : '';
                $searchField.val(selectedText);

                $searchField[0].style.width = '100%';
                setTimeout(function () { $searchField.trigger('focus'); }, 0);
            });

            // ── NEW: item select korar sathe sathe search field e text set ──
            $el.off('select2:select.gcInlineSync').on('select2:select.gcInlineSync', function (e) {
                const s2 = $el.data('select2');
                if (!s2) return;
                const data = e.params && e.params.data;
                const text = (data && data.text) ? data.text.trim() : '';

                const $dropdown = s2.$dropdown;
                const $container = s2.$container;
                if (!$dropdown || !$container) return;

                const $searchField = $container.find('.select2-search--dropdown.gc-inline-search .select2-search__field')
                    .add($dropdown.find('.select2-search__field'));

                $searchField.val(text);
            });

            $el.on('select2:close.gcInline', function () {
                const s2 = $el.data('select2');
                if (!s2) return;
                const $container = s2.$container;
                const $dropdown = s2.$dropdown;
                if (!$container || !$dropdown) return;

                const $selection = $container.find('.select2-selection--single');
                const $searchWrap = $selection.find('.select2-search--dropdown.gc-inline-search');

                $selection.removeClass('gc-inline-search-active');
                if ($searchWrap.length) {
                    $searchWrap.removeClass('gc-inline-search').prependTo($dropdown);
                }
            });
        });
    }

    // ═══════════════════════════════════════════════════════════════
    //  REMOTE MULTISELECT  (scroll load from SP)
    //  ✅ gc-multiselect-load-data class এর select এ ব্যবহার হয়
    // ═══════════════════════════════════════════════════════════════
    let instanceCounter = 0;

    window.gcRemoteMultiselect = function (selector, opt) {
        console.log('[gcRemote] INIT for:', selector);

        const $select = (selector instanceof jQuery) ? selector : $(selector);
        if (!$select.length) {
            console.error('[gcRemote] NOT FOUND:', selector);
            return { reset: function () { }, reload: function () { } };
        }

        if (
            $select.data('gcRemote-uid') ||
            $select.data('multiselect-init') ||
            $select.data('multiselect') ||
            $select.next('.btn-group').length
        ) {
            console.log('[gcRemote] existing init found - destroying:', $select.attr('id'));
            gcDestroyMultiselect($select);
        }

        const uid = 'gcRms_' + (++instanceCounter);
        $select.data('gcRemote-uid', uid);

        const placeholder = $select.attr('data-placeholder') || 'Select';
        const state = { page: 1, total: null, loading: false, done: false, q: '' };
        let currentXhr = null;

        // ── buttonText function ──────────────────────────────────
        const _buttonTextFn = (typeof opt.buttonText === 'function')
            ? opt.buttonText
            : function (options) {
                if (!options || options.length === 0) return placeholder;
                if (options.length === 1) return $(options[0]).text().trim();
                return options.length + ' items selected';
            };

        // ── multiselect init ─────────────────────────────────────
        $select.data('multiselect-init', true);

        $select.multiselect({
            includeSelectAllOption: true,
            selectAllText: 'Select All',
            allSelectedText: 'All Selected',
            enableFiltering: true,
            enableCaseInsensitiveFiltering: true,
            buttonWidth: '100%',
            maxHeight: 250,
            nonSelectedText: placeholder,
            numberDisplayed: 0,
            buttonText: _buttonTextFn,
            buttonTitle: _buttonTextFn,

            templates: {
                button:
                    '<button type="button" class="multiselect dropdown-toggle" data-toggle="dropdown">' +
                    '  <span class="multiselect-selected-text"></span>' +
                    '  <input type="text" class="multiselect-inline-search" autocomplete="off" />' +
                    '  <b class="caret"></b>' +
                    '</button>',
                ul: '<ul class="multiselect-container dropdown-menu"></ul>',
                filter:
                    '<li class="multiselect-item multiselect-filter" style="display:none">' +
                    '  <input class="form-control multiselect-search" type="text" />' +
                    '</li>',
                li: '<li><a tabindex="0"><label class="checkbox"></label></a></li>',
                divider: '<li class="multiselect-item divider"></li>',
                liGroup: '<li class="multiselect-item multiselect-group"><label></label></li>'
            },

            onInitialized: function () {
                const $container = $select.next('.btn-group');
                const $btn = $container.find('button.multiselect');
                $btn.find('.multiselect-inline-search').hide();
                $btn.find('.multiselect-selected-text').show();
                $select.data('gcMsInit', true);
                _ensureRightClear($select);
            },

            onDropdownShown: function () {
                const $container = $select.next('.btn-group');
                const $btn = $container.find('button.multiselect');
                const $inline = $btn.find('.multiselect-inline-search');
                const $text = $btn.find('.multiselect-selected-text');
                const $hidden = $container.find('input.multiselect-search');

                $btn.addClass('is-inline-searching');
                $btn.removeClass('show-right-clear');
                $text.hide();
                $inline.show().val('').attr('placeholder', placeholder);
                $hidden.val('').trigger('input').trigger('keyup');

                $inline.off('.gcRmInline').on('input.gcRmInline', function (e) {
                    e.stopPropagation();
                    const term = $(this).val();
                    $hidden.val(term).trigger('input').trigger('keyup').trigger('change');
                    _triggerServerSearch(term);
                });

                $inline.off('.gcRmGuard').on('keydown.gcRmGuard keypress.gcRmGuard keyup.gcRmGuard', function (e) {
                    if (e.key === ' ' || e.key === 'Spacebar' || e.keyCode === 32) {
                        e.stopPropagation();
                        return;
                    }
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        e.stopPropagation();
                        return;
                    }
                    if (e.key === 'Escape') {
                        e.stopPropagation();
                        return;
                    }
                    e.stopPropagation();
                });

                $btn.off('keydown.gcRmBtnGuard').on('keydown.gcRmBtnGuard', function (e) {
                    if (!$btn.hasClass('is-inline-searching')) return;
                    if (e.key === ' ' || e.keyCode === 32 || e.key === 'Enter') {
                        e.preventDefault();
                        e.stopPropagation();
                    }
                });

                $inline.off('mousedown.gcRmClick click.gcRmClick')
                    .on('mousedown.gcRmClick click.gcRmClick', function (e) { e.stopPropagation(); });

                $inline.off('keydown.gcRmSpace').on('keydown.gcRmSpace', function (e) {
                    if (e.key === ' ' || e.keyCode === 32) {
                        e.stopPropagation();
                        const el = this;
                        const start = el.selectionStart;
                        const end = el.selectionEnd;
                        const val = el.value;
                        el.value = val.substring(0, start) + ' ' + val.substring(end);
                        el.selectionStart = el.selectionEnd = start + 1;
                        $(el).trigger('input');
                        e.preventDefault();
                    }
                });

                setTimeout(function () {
                    try { $inline.trigger('focus'); } catch (_) { }
                }, 0);
            },

            onDropdownHidden: function () {
                const $container = $select.next('.btn-group');
                const $btn = $container.find('button.multiselect');
                const $inline = $btn.find('.multiselect-inline-search');
                const $text = $btn.find('.multiselect-selected-text');
                const $hidden = $container.find('input.multiselect-search');

                $hidden.val('').trigger('input').trigger('keyup');
                $btn.removeClass('is-inline-searching');
                $inline.val('').hide();
                $text.show();
                _updateRightClear($select);
            },

            onChange: function () {
                _updateRightClear($select);
            }
        });

        // ── server search debounced ──────────────────────────────
        const _triggerServerSearch = debounce(function (term) {
            if (term !== state.q) {
                _reset(term);
                _fetchPage();
            }
        }, 300);

        // ── right clear icon ─────────────────────────────────────
        function _ensureRightClear($sel) {
            const $container = $sel.next('.btn-group');
            const $btn = $container.find('button.multiselect');
            $btn.find('.ms-right-clear').not(':first').remove();
            if ($btn.find('.ms-right-clear').length) return;

            const $rc = $('<span class="ms-right-clear" title="Clear"><span class="ms-x">×</span></span>');
            const $caret = $btn.find('b.caret');
            if ($caret.length) $rc.insertBefore($caret); else $btn.append($rc);

            $rc.off('.gcRmRc').on('mousedown.gcRmRc click.gcRmRc', function (e) {
                e.preventDefault();
                e.stopPropagation();
                $sel.multiselect('deselectAll', false);
                $sel.multiselect('updateButtonText');
                _updateRightClear($sel);
            });
        }

        function _updateRightClear($sel) {
            const $container = $sel.next('.btn-group');
            const $btn = $container.find('button.multiselect');
            _ensureRightClear($sel);
            const cnt = $sel.find('option:selected').length;
            const open = $btn.hasClass('is-inline-searching');
            $btn.removeClass('show-right-clear');
            if (cnt > 0 && !open) $btn.addClass('show-right-clear');
        }

        // ── container / scrollable el finders ───────────────────
        function _getContainer() {
            const ms = $select.data('multiselect');
            if (ms && ms.$container && ms.$container.length) return ms.$container;
            const $next = $select.next('.btn-group');
            if ($next.length) return $next;
            return $select.parent('.btn-group');
        }

        function _getScrollableEl() {
            const $c = _getContainer();
            if (!$c.length) return $();

            const $div = $c.find('div.multiselect-container');
            if ($div.length) return $div;

            const $ul = $c.find('ul.multiselect-container');
            if ($ul.length) return $ul;

            const $menu = $c.find('.dropdown-menu');
            if ($menu.length) return $menu;

            return $();
        }

        // ── scroll bind ──────────────────────────────────────────
        function _bindScroll() {
            setTimeout(function () {
                const $el = _getScrollableEl();
                if (!$el.length) {
                    console.warn('[gcRemote] scrollable el not found for:', $select.attr('id'));
                    return;
                }

                const el = $el[0];
                console.log('[gcRemote] bindScroll →', el.tagName + '.' + el.className,
                    '| scrollH:', el.scrollHeight, '| clientH:', el.clientHeight,
                    '| overflow-y:', $el.css('overflow-y'));

                const oy = $el.css('overflow-y');
                if (oy !== 'auto' && oy !== 'scroll') {
                    $el.css({ 'overflow-y': 'auto', 'max-height': '250px' });
                }

                $el.off('scroll.' + uid).on('scroll.' + uid, function () {
                    const el = this;
                    const top = Math.ceil(el.scrollTop);
                    const client = el.clientHeight;
                    const total = el.scrollHeight;
                    const near = (top + client) >= (total - 80);

                    console.log('[gcRemote] 🔄 SCROLL | top:', top, '| client:', client,
                        '| total:', total, '| near:', near, '| loading:', state.loading, '| done:', state.done);

                    if (near && !state.loading && !state.done) {
                        console.log('[gcRemote] 📥 LOAD NEXT PAGE');
                        _fetchPage();
                    }
                });

                console.log('[gcRemote] ✅ scroll bound for:', $select.attr('id'));
            }, 150);
        }

        // ── fetch page ───────────────────────────────────────────
        async function _fetchPage() {
            if (state.loading || state.done) return;
            state.loading = true;

            console.log('[gcRemote] ▶ FETCH | type:', opt.type, '| page:', state.page, '| q:', state.q);

            const filters = typeof opt.getFilters === 'function' ? opt.getFilters() : (opt.filters || {});
            const pageSize = opt.pageSize || 100;

            if (currentXhr && currentXhr.readyState !== 4) currentXhr.abort();

            const requestData = Object.assign({
                sp: opt.sp,
                type: opt.type,
                q: state.q || '',
                page: state.page,
                pageSize: pageSize
            }, filters);

            console.log('[gcRemote] REQUEST:', JSON.stringify(requestData));

            let res;
            try {
                currentXhr = $.ajax({
                    url: opt.url,
                    method: 'GET',
                    dataType: 'json',
                    traditional: true,
                    data: requestData
                });
                res = await currentXhr;
                console.log('[gcRemote] ✅ items:', (res.items || []).length, '| total:', res.total);
            } catch (e) {
                if (e.statusText !== 'abort') {
                    console.error('[gcRemote] ❌ AJAX:', e.status, e.statusText);
                }
                state.loading = false;
                return;
            }

            const items = res.items || [];
            if (res.total != null) state.total = res.total;

            const selectedVals = ($select.val() || []).map(String);
            const existingIds = new Set(
                $select.find('option').map(function () { return String(this.value); }).get()
            );

            let added = 0;
            for (const it of items) {
                const id = String(it.id ?? it.Id ?? '');
                const name = String(it.name ?? it.Name ?? '');
                if (id && !existingIds.has(id)) {
                    $select.append(new Option(name, id, false, false));
                    added++;
                }
            }
            console.log('[gcRemote] added:', added, '| total options:', $select.find('option').length);

            const $el = _getScrollableEl();
            const savedScroll = $el.scrollTop();

            try { $select.multiselect('rebuild'); }
            catch (e) { try { $select.multiselect('refresh'); } catch (_) { } }

            if (selectedVals.length > 0) {
                $select.val(selectedVals);
                try { $select.multiselect('updateButtonText'); } catch (_) { }
            }

            setTimeout(function () {
                const $fresh = _getScrollableEl();
                if (!$fresh.length) return;

                const oy2 = $fresh.css('overflow-y');
                if (oy2 !== 'auto' && oy2 !== 'scroll') {
                    $fresh.css({ 'overflow-y': 'auto', 'max-height': '250px' });
                }

                $fresh.scrollTop(savedScroll);

                $fresh.off('scroll.' + uid).on('scroll.' + uid, function () {
                    const el = this;
                    const top = Math.ceil(el.scrollTop);
                    const cli = el.clientHeight;
                    const tot = el.scrollHeight;
                    const near = (top + cli) >= (tot - 80);

                    console.log('[gcRemote] 🔄 SCROLL(rebuild) | top:', top, '| near:', near);

                    if (near && !state.loading && !state.done) {
                        console.log('[gcRemote] 📥 LOAD NEXT PAGE (scroll)');
                        _fetchPage();
                    }
                });

                console.log('[gcRemote] ✅ scroll re-bound | scrollH:', $fresh[0].scrollHeight,
                    '| clientH:', $fresh[0].clientHeight);

                const $container = _getContainer();
                const $btn = $container.find('button.multiselect');
                if ($btn.hasClass('is-inline-searching')) {
                    const $inline = $btn.find('.multiselect-inline-search');
                    const $text = $btn.find('.multiselect-selected-text');
                    $text.hide();
                    $inline.show();
                    setTimeout(function () { try { $inline.trigger('focus'); } catch (_) { } }, 0);
                }

            }, 50);

            if (state.page === 1 && state.q === '' && opt.autoSelectFirst && items.length > 0) {
                const isAlreadySelected = ($select.val() || []).filter(x => x).length > 0;
                if (!isAlreadySelected) {
                    const firstId = String(items[0].id ?? items[0].Id ?? '');
                    if (firstId) {
                        setTimeout(function () {
                            $select.val([firstId]);
                            try { $select.multiselect('updateButtonText'); } catch (_) { }
                            _updateRightClear($select);
                            $select.trigger('change');
                            $select.trigger('change.cascade');
                            console.log('[gcRemote] ✅ autoSelectFirst:', firstId);
                        }, 60);
                    }
                }
            }


            state.page++;

            //if (state.page === 1 && state.q === '' && opt.autoSelectFirst && items.length > 0) {
            //    const firstItem = items[0];
            //    const firstId = String(firstItem.id ?? firstItem.Id ?? '');
            //    const isAlreadySelected = ($select.val() || []).length > 0;

            //    if (firstId && !isAlreadySelected) {
            //        setTimeout(function () {
            //            $select.val([firstId]);
            //            try { $select.multiselect('updateButtonText'); } catch (_) { }
            //            _updateRightClear($select);
            //            $select.trigger('change');
            //            $select.trigger('change.cascade');
            //        }, 60); // rebuild এর পরে
            //    }
            //}

            const loaded = (state.page - 1) * pageSize;
            if (state.total != null && loaded >= state.total) {
                state.done = true;
                console.log('[gcRemote] ✅ ALL LOADED. total:', state.total);
            }
            if (items.length === 0) state.done = true;

            state.loading = false;
        }

        // ── reset ────────────────────────────────────────────────
        function _reset(newQ) {
            console.log('[gcRemote] 🔁 RESET | id:', $select.attr('id'), '| q:', newQ);
            state.page = 1;
            state.total = null;
            state.loading = false;
            state.done = false;
            state.q = newQ || '';
            $select.empty();
            try { $select.multiselect('rebuild'); } catch (_) { }
        }

        // ── dropdown events (document delegate) ──────────────────
        $(document).on('shown.bs.dropdown.' + uid, function (e) {
            const $c = _getContainer();
            if (!$c.length) return;
            if (!$.contains($c[0], e.target) && !$c.is($(e.target))) return;

            console.log('[gcRemote] 🔽 DROPDOWN OPEN:', $select.attr('id'));

            _bindScroll();

            if ($select.find('option').length === 0 && !state.done) {
                _fetchPage();
            }
        });

        // expose
        return {
            reset: function (q) { _reset(q || ''); },
            reload: function () { _reset(''); _fetchPage(); }
        };
    };

    // ═══════════════════════════════════════════════════════════════
    //  REFRESH HELPERS
    // ═══════════════════════════════════════════════════════════════
    window.refreshMultiselect = function (selector) {
        const $sels = (selector instanceof jQuery) ? selector : $(selector);
        $sels.each(function () {
            const $s = $(this);
            if ($s.data('multiselect')) {
                $s.multiselect('rebuild');
                $s.multiselect('updateButtonText');
            } else {
                initializeMultiselects($s);
            }
        });
    };

    window.refreshSelect2 = function (selector) {
        const $sels = (selector instanceof jQuery) ? selector : $(selector);
        $sels.each(function () {
            const $s = $(this);
            if ($s.data('select2')) {
                $s.trigger('change.select2');
            } else {
                initInlineSearchSelect2($s);
            }
        });
    };

    // ═══════════════════════════════════════════════════════════════
    //  AUTO INIT
    //  ✅ gc-multiselect        → static options, auto-init হয়
    //  ✅ gc-multiselect-load-data → remote/gcGlobalFilter, auto-init হয় না
    // ═══════════════════════════════════════════════════════════════
    //$(document).ready(function () {
    //    initializeMultiselects('.gc-multiselect');
    //    initInlineSearchSelect2('.gc-select2');
    //});

    window.initializeMultiselects = initializeMultiselects;
    window.initInlineSearchSelect2 = initInlineSearchSelect2;

})(jQuery);

// ═══════════════════════════════════════════════════════════════
//  GLOBAL FILTER
//  ✅ gc-multiselect-load-data class এর select গুলো handle করে
// ═══════════════════════════════════════════════════════════════
window.gcGlobalFilter = (function () {

    function arrVal(sel) {
        const v = $(sel).val();
        if (!v) return [];
        return Array.isArray(v) ? v.filter(x => x) : [v].filter(x => x);
    }

    function initMultiselect(options) {      
        const settings = {
            company: '#companySelect',
            branch: '#branchSelect',
            department: '#departmentSelect',
            designation: '#designationSelect',
            employee: '#employeeSelect',
            employeeStatuses: '',
            ...options
        };
        function statusArr() {
            return getStatusArr(settings.employeeStatuses);
        }
        // =========================
        // COMPANY
        // ========================= 
        window.companyMs = window.gcRemoteMultiselect(settings.company, {
            url: '/GlobalFilterPagedLookup/PagedLookup',
            sp: 'dbo.SP_GetGlobalScrollLoadLookupPaged',
            type: 'Company',
            pageSize: 100,
            autoSelectFirst: true,  
            getFilters: function () {
                return {                   
                    employeeStatuses: statusArr()                     
                };
            },
            buttonText: function (options) {
                if (!options || options.length === 0) return 'Select Company';
                if (options.length === 1) return $(options[0]).text().trim();
                return options.length + ' items selected';
            }
        });
        // =========================
        // BRANCH
        // =========================
        const branchMs = window.gcRemoteMultiselect(settings.branch, {
            url: '/GlobalFilterPagedLookup/PagedLookup',
            sp: 'dbo.SP_GetGlobalScrollLoadLookupPaged',
            type: 'Branch',
            pageSize: 100,
            getFilters: function () {
                return { companyCodes: arrVal(settings.company), employeeStatuses: statusArr() };

            }
        });

        // =========================
        // DEPARTMENT
        // =========================
        const deptMs = window.gcRemoteMultiselect(settings.department, {
            url: '/GlobalFilterPagedLookup/PagedLookup',
            sp: 'dbo.SP_GetGlobalScrollLoadLookupPaged',
            type: 'Department',
            pageSize: 100,
            getFilters: function () {
                return {
                    companyCodes: arrVal(settings.company),
                    branchCodes: arrVal(settings.branch),
                    employeeStatuses: statusArr() 
                };
            }
        });

        // =========================
        // DESIGNATION
        // =========================
        const desigMs = window.gcRemoteMultiselect(settings.designation, {
            url: '/GlobalFilterPagedLookup/PagedLookup',
            sp: 'dbo.SP_GetGlobalScrollLoadLookupPaged',
            type: 'Designation',
            pageSize: 100,
            getFilters: function () {
                return {
                    companyCodes: arrVal(settings.company),
                    branchCodes: arrVal(settings.branch),
                    departmentCodes: arrVal(settings.department),
                    employeeStatuses: statusArr() 
                };
            }
        });

        // =========================
        // EMPLOYEE
        // =========================
        const empMs = window.gcRemoteMultiselect(settings.employee, {
            url: '/GlobalFilterPagedLookup/PagedLookup',
            sp: 'dbo.SP_GetGlobalScrollLoadLookupPaged',
            type: 'Employee',
            pageSize: 100,
            getFilters: function () {
                return {
                    companyCodes: arrVal(settings.company),
                    branchCodes: arrVal(settings.branch),
                    departmentCodes: arrVal(settings.department),
                    designationCodes: arrVal(settings.designation),
                    employeeStatuses: statusArr() 
                };
            }
        });
        function getStatusArr(statusSel) {
            if (!statusSel) return [];                       // All
            if (typeof statusSel === 'string' && !statusSel.startsWith('#') && !statusSel.startsWith('.')) {
                // hardcoded value passed directly e.g. '01'
                return statusSel ? [statusSel] : [];
            }
            // radio group
            const $radio = $('input[type="radio"][name="' + statusSel.replace(/^[#.]/, '') + '"]:checked');
            if ($radio.length) {
                const v = $radio.val();
                return v ? [v] : [];
            }
            // select / any other input
            const v = $(statusSel).val();
            if (!v) return [];
            return Array.isArray(v) ? v.filter(x => x) : [v].filter(x => x);
        }


        // =========================
        // CASCADING RESET
        // =========================
        $(settings.company).off('change.cascade').on('change.cascade', function () {
            _domReset([settings.branch, settings.department, settings.designation, settings.employee]);
            branchMs.reset();
            deptMs.reset();
            desigMs.reset();
            empMs.reset();
        });

        $(settings.branch).off('change.cascade').on('change.cascade', function () {
            _domReset([settings.department, settings.designation, settings.employee]);
            deptMs.reset();
            desigMs.reset();
            empMs.reset();
        });

        $(settings.department).off('change.cascade').on('change.cascade', function () {
            _domReset([settings.designation, settings.employee]);
            desigMs.reset();
            empMs.reset();
        });

        $(settings.designation).off('change.cascade').on('change.cascade', function () {
            _domReset([settings.employee]);
            empMs.reset();
        });
        if (settings.employeeStatus && typeof settings.employeeStatus === 'string'
            && (settings.employeeStatus.startsWith('#') || settings.employeeStatus.startsWith('.'))) {

            // <select> element
            $(settings.employeeStatus).off('change.gcStatus').on('change.gcStatus', _onStatusChange);
        }

        // radio group — bind by name attribute value (strip leading dot/hash if any)
        const radioName = settings.employeeStatus
            ? settings.employeeStatus.replace(/^[#.]/, '')
            : null;

        if (radioName && radioName.length > 0) {
            $('input[type="radio"][name="' + radioName + '"]')
                .off('change.gcStatus')
                .on('change.gcStatus', _onStatusChange);
        }

        function _onStatusChange() {
            // Reset all dropdowns when status changes
            _domReset([
                settings.company,
                settings.branch,
                settings.department,
                settings.designation,
                settings.employee
            ]);
            window.companyMs.reset();
            branchMs.reset(); deptMs.reset(); desigMs.reset(); empMs.reset();

            // Re-fetch company with new status immediately
            // (dropdown open করলে auto fetch হবে, কিন্তু আমরা eager load করতে চাই)
            // companyMs.reload() call করলে data re-fetch হবে
            setTimeout(function () {
                window.companyMs.reload();
            }, 50);
        }

        // ── DOM reset helper ─────────────────────────────────────
        function _domReset(selectors) {
            selectors.forEach(function (sel) {
                $(sel).empty();
                try { $(sel).multiselect('rebuild'); } catch (e) { }
            });
        }

        return { companyMs: window.companyMs, branchMs, deptMs, desigMs, empMs };
    }

    return { initMultiselect };
    //    function _domReset(selectors) {
    //        selectors.forEach(function (sel) {
    //            $(sel).empty();
    //            try { $(sel).multiselect('rebuild'); } catch (e) { }
    //        });
    //    }

    //    return { companyMs, branchMs, deptMs, desigMs, empMs };
    //}

    //return { initMultiselect };

})();