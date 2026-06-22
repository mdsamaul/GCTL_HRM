(function () {
  

    var DESIGN = {
        'save': { icon: 'fas fa-save', color: '#228c37', label: 'Save' },
        'update': { icon: 'fa fa-edit', color: '#0ea5e9', label: 'Update' },
        'delete': { icon: 'fas fa-trash-alt', iconColor: '#ef4444', label: 'Delete' },
        'clear': { icon: 'fas fa-redo', label: 'Clear' },
        'favorite': { icon: 'fas fa-star', extraClass: 'btn-favorite', label: 'Favorite' }
    };
    var OLD_CLASS_RE = /^(success|danger|warning|info|primary)-btn$/;

    var CLASS_KEYWORDS = [
        [/save/i, 'save'],
        [/update|edit/i, 'update'],
        [/delete|remove/i, 'delete'],
        [/clear|reset/i, 'clear'],
        [/favorite|fav\b/i, 'favorite']
    ];
    var ICON_KEYWORDS = [
        [/fa-save/, 'save'],
        [/fa-edit|fa-pen/, 'update'],
        [/fa-trash/, 'delete'],
        [/fa-sync|fa-redo|fa-eraser/, 'clear'],
        [/fa-star/, 'favorite']
    ];

    function getButtonKey(btn) {
        var title = (btn.getAttribute('title') || '').trim().toLowerCase();
        if (DESIGN[title]) return title;

        var cls = btn.className || '';
        for (var i = 0; i < CLASS_KEYWORDS.length; i++) {
            if (CLASS_KEYWORDS[i][0].test(cls)) return CLASS_KEYWORDS[i][1];
        }

        var icon = btn.querySelector('i');
        var iconCls = icon ? icon.className : '';
        for (var j = 0; j < ICON_KEYWORDS.length; j++) {
            if (ICON_KEYWORDS[j][0].test(iconCls)) return ICON_KEYWORDS[j][1];
        }
        return null;
    }

    // icon bad diye button-er actual visible text ber kora (nbsp soho)
    function getVisibleText(btn) {
        var clone = btn.cloneNode(true);
        var iconClone = clone.querySelector('i');
        if (iconClone) iconClone.remove();
        return clone.textContent.replace(/\u00A0/g, ' ').trim();
    }

    function normalizeButton(btn, key) {
        var cfg = DESIGN[key];
        Array.prototype.slice.call(btn.classList).forEach(function (c) {
            if (OLD_CLASS_RE.test(c)) btn.classList.remove(c);
        });
        btn.removeAttribute('style');
        btn.classList.remove('btn-circle', 'float-right', 'btn-sm');
        btn.classList.add('btn');
        if (cfg.extraClass) btn.classList.add(cfg.extraClass);

        // icon-color na thakle cfg.color fallback hisebe use hobe (icon+text same color hoye jabe)
        var iconColorToUse = cfg.iconColor || cfg.color || '';

        var icon = btn.querySelector('i');
        if (icon) {
            icon.className = cfg.icon;
            icon.style.marginRight = '0px';
            if (iconColorToUse) icon.style.setProperty('color', iconColorToUse, 'important');
            icon.innerHTML = '';
        }

        if (!getVisibleText(btn) && cfg.label) {
            btn.appendChild(document.createTextNode(cfg.label));
        }

        // button-er nijer text color (global black !important override korar jonno)
        if (cfg.color) {
            btn.style.setProperty('color', cfg.color, 'important');
        }
    }

    // print/export button generic-vabe khoja — class-name e "print"/"export" thakle
    // ba icon-e fa-print/fa-download thakle dhore nibe (page-specific class hole o miss hobe na)
    function findPrintButton(toolbar) {
        var explicit = toolbar.querySelector(
            '.js-loan-entry-report-print, .js-print, button[title="Print"], button[title="Export"]'
        );
        if (explicit) return explicit;

        var buttons = toolbar.querySelectorAll('button');
        for (var i = 0; i < buttons.length; i++) {
            var btn = buttons[i];
            if (btn.classList.contains('dropdown-item')) continue;
            if (btn.closest('.dropdown-menu')) continue;
            if (/print|export/i.test(btn.className)) return btn;
            var icon = btn.querySelector('i');
            if (icon && /fa-print|fa-download/.test(icon.className)) return btn;
        }
        return null;
    }

    function reorganizeToolbar(toolbar) {
        var printBtn = findPrintButton(toolbar);

        if (printBtn) {
            var oldParent = printBtn.parentElement;
            var hasOtherButtons = Array.prototype.some.call(oldParent.children, function (el) {
                return el.tagName === 'BUTTON' && el !== printBtn;
            });
            var dropdownGroup;
            if (oldParent.classList.contains('btn-group') && !hasOtherButtons) {
                dropdownGroup = oldParent;
            } else {
                dropdownGroup = document.createElement('div');
                dropdownGroup.className = 'btn-group';
                oldParent.insertBefore(dropdownGroup, printBtn);
                dropdownGroup.appendChild(printBtn);
            }
            dropdownGroup.dataset.keepGroup = 'true';

            Array.prototype.slice.call(printBtn.classList).forEach(function (c) {
                if (OLD_CLASS_RE.test(c)) printBtn.classList.remove(c);
            });
            printBtn.removeAttribute('style');
            printBtn.classList.add('btn', 'btn-default', 'dropdown-toggle');
            printBtn.setAttribute('data-toggle', 'dropdown');
            printBtn.setAttribute('aria-haspopup', 'true');
            printBtn.setAttribute('aria-expanded', 'false');

            // icon + label shobshomoy force kore "download / Export" e bosiye dewa
            var pIcon = printBtn.querySelector('i');
            if (!pIcon) {
                pIcon = document.createElement('i');
                printBtn.insertBefore(pIcon, printBtn.firstChild);
            }
            pIcon.className = 'fa fa-download';
            pIcon.style.marginRight = '6px';
            pIcon.innerHTML = '';

            Array.prototype.slice.call(printBtn.childNodes).forEach(function (node) {
                if (node.nodeType === 3) node.remove(); // purono text node (Print/khali) shoriye dilam
            });
            printBtn.appendChild(document.createTextNode('Export'));

            var menu = dropdownGroup.querySelector('.dropdown-menu');
            if (!menu) {
                menu = document.createElement('div');
                menu.className = 'dropdown-menu dropdown-menu-right';
                menu.innerHTML =
                    '<button class="dropdown-item d-flex align-items-center js-export-excel" type="button">' +
                    '<i class="fas fa-file-excel" style="color:#16a34a; margin-right:8px;"></i> Excel</button>' +
                    '<button class="dropdown-item d-flex align-items-center js-export-pdf" type="button">' +
                    '<i class="fas fa-file-pdf" style="color:#dc2626; margin-right:8px;"></i> PDF</button>';
                dropdownGroup.appendChild(menu);
            }
        }

        Array.prototype.slice.call(toolbar.querySelectorAll('.btn-group')).forEach(function (group) {
            if (group.dataset.keepGroup === 'true') return;
            var parent = group.parentElement;
            while (group.firstChild) parent.insertBefore(group.firstChild, group);
            group.remove();
        });
    }

    // toolbar-er column-er default Bootstrap right-padding shoriye dewa (NOC Entry pattern: col-md-7 px-0)
    function fixToolbarSpacing(toolbar) {
        var col = toolbar.closest('[class*="col-"]');
        if (col && !col.classList.contains('px-0')) {
            col.classList.add('px-0');
        }

        var row = toolbar.closest('.row');
        if (row) {
            row.style.marginLeft = '0';
            row.style.marginRight = '0';
        }
    }

    function initToolbars() {
        document.querySelectorAll('.btn-toolbar').forEach(function (toolbar) {
            fixToolbarSpacing(toolbar);
            reorganizeToolbar(toolbar);
            toolbar.querySelectorAll('button').forEach(function (btn) {
                var key = getButtonKey(btn);
                if (key && DESIGN[key]) normalizeButton(btn, key);
            });
            forceTooltipBottom(toolbar); 
        });
    }
    function forceTooltipBottom(toolbar) {
        if (!(window.jQuery && jQuery.fn.tooltip)) return;

        toolbar.querySelectorAll('[data-toggle="tooltip"]').forEach(function (el) {
            el.setAttribute('data-placement', 'bottom');
            var $el = jQuery(el);

            try {
                $el.tooltip('dispose'); // Bootstrap 4.2+
            } catch (err) {
                // purono Bootstrap version-e dispose nei, manually data clear kora
                $el.tooltip('hide');
                $el.removeData('bs.tooltip');
            }

            $el.tooltip({ placement: 'bottom' });
        });
    }
    document.body.addEventListener('click', function (e) {
        var excel = e.target.closest('.js-export-excel');
        if (excel) {
            var g1 = excel.closest('.btn-group') || excel.closest('.btn-toolbar');
            if (g1) g1.dispatchEvent(new CustomEvent('export', { detail: { type: 'excel' } }));
            return;
        }
        var pdf = e.target.closest('.js-export-pdf');
        if (pdf) {
            var g2 = pdf.closest('.btn-group') || pdf.closest('.btn-toolbar');
            if (g2) g2.dispatchEvent(new CustomEvent('export', { detail: { type: 'pdf' } }));
        }
    });

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initToolbars);
    } else {
        initToolbars();
    }

    window.reinitToolbars = initToolbars;
})();