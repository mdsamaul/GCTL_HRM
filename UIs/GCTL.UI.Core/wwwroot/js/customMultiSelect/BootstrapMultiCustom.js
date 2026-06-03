(function ($) {
    'use strict';    

    function gcDestroyMultiselect($select) {
        try {
            if ($select.data('multiselect')) {
                $select.multiselect('destroy');
            }
        } catch (e) { }

        $select.next('.btn-group').remove();

        $select.removeData('gcMsInit');
    }

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
            const isMultiple = $select.prop("multiple");

            if (!options || options.length === 0) return placeholder;
            if (!isMultiple) return $(options[0]).text().trim();

            if (id === 'companySelect') {
                if (options.length === 1) return $(options[0]).text().trim();
                return `${options.length} items selected`;
            }

            return options.length === 1 ? '1 item selected' : `${options.length} items selected`;
        };

        function forceInlineFocus($inline) {
            try { $inline.trigger('focus'); } catch (e) { }
            setTimeout(() => {
                try { $inline.trigger('focus'); } catch (e) { }
                requestAnimationFrame(() => {
                    try {
                        const el = $inline.get(0);
                        if (el) {
                            el.focus();
                            const len = el.value.length;
                            if (el.setSelectionRange) el.setSelectionRange(len, len);
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

                    const isMultiselect = $select.prop('multiple');

                    if (isMultiselect) {
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

            $right = $(
                '<span class="ms-right-clear" title="Clear">' +
                '  <span class="ms-x">×</span>' +
                '</span>'
            );

            const $caret = $btn.find('b.caret');
            if ($caret.length) $right.insertBefore($caret);
            else $btn.append($right);

            $right.off('.gcRightClear').on('mousedown.gcRightClear click.gcRightClear', function (e) {
                e.preventDefault();
                e.stopPropagation();

                const isMultiselect = $select.prop('multiple');

                if (isMultiselect) {
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

            if ($select.data('gcMsInit') === true && $select.data('multiselect')) {
                return;
            }

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
                    if (!isMultiple) {
                        $container.addClass('single-select');
                    }
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

    function gcDestroySelect2($el) {
        try {
            if ($el.data('select2')) $el.select2('destroy');
        } catch (e) { }

        $el.siblings('.select2-container').remove();
        $el.nextAll('.select2-container').remove();

        $el.removeData('gcSelect2Init');
    }

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

                $searchWrap
                    .addClass('gc-inline-search')
                    .appendTo($selection);


                $searchField.attr('placeholder', placeholder);
                //New from here
                $searchField.off('keydown.gcSpace').on('keydown.gcSpace', function (e) {
                    if (e.key === ' ' || e.keyCode === 32) {
                        e.stopPropagation(); // prevent Select2 from eating it
                        // manually insert a space at cursor position
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
                //
                $searchField.val('');
                $searchField[0].style.Width = '100%';

                setTimeout(function () {
                    $searchField.trigger('focus');
                }, 0);
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
                    $searchWrap
                        .removeClass('gc-inline-search')
                        .prependTo($dropdown);
                }
            });
        });
    }

   

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

    /* =========================
       Auto init
    ========================== */
    $(document).ready(function () {
        initializeMultiselects('.gc-multiselect');
        initInlineSearchSelect2('.gc-select2');
    });

    window.initializeMultiselects = initializeMultiselects;
    window.initInlineSearchSelect2 = initInlineSearchSelect2;

})(jQuery);

