(function ($) {
    $(function () {
        // var css = '.card-header, .card-header { background: #f4f6fb !important; background-color: #f4f6fb !important; background-image: none !important; color: #333 !important; border-bottom: 1px solid rgba(0,0,0,0.08) !important; }';

        var css = '.card-header, .card-header { background: linear-gradient(135deg, #f8faff 0%, #f4f6fb 100%) !important; background-color: #f4f6fb !important; background-image: none !important; color: #333 !important; border-bottom: none !important; box-shadow: 0 2px 8px rgba(0,0,0,0.06), 0 1px 2px rgba(0,0,0,0.04) !important; }';
        var style = document.createElement('style');
        style.type = 'text/css';
        if (style.styleSheet) {
            style.styleSheet.cssText = css;
        } else {
            style.appendChild(document.createTextNode(css));
        }
        document.head.appendChild(style);

        var mo = new MutationObserver(function () {
            if (!document.head.contains(style)) {
                document.head.appendChild(style);
            }
        });
        mo.observe(document.head, { childList: true });
    });
    function applyEmpIdLinkStyle() {
        document.querySelectorAll('table.dataTable').forEach(table => {
            const headerCells = table.querySelectorAll('thead th');

            headerCells.forEach((th, index) => {
                // normalize: lowercase, strip periods/spaces -> "Emp. ID" / "Employee ID" / "emp id" all match
                const normalized = th.textContent.trim().toLowerCase().replace(/[.\s]+/g, '');

                if (normalized === 'empid' || normalized === 'employeeid') {
                    if (!table.id) {
                        table.id = 'dt-' + Math.random().toString(36).slice(2, 11);
                    }
                    const colNum = index + 1; // CSS nth-child is 1-indexed
                    const styleId = `empid-style-${table.id}`;

                    if (!document.getElementById(styleId)) {
                        const style = document.createElement('style');
                        style.id = styleId;
                        console.log(colNum);
                        style.textContent = `
        #${table.id} tbody tr td:nth-child(${colNum}) {
            font-weight: 500 !important;
            font-family: Arial, sans-serif;
            color: #0000e0 !important;
        }
      `;
                        document.head.appendChild(style);
                    }
                }
            });
        });
    }

    document.addEventListener('DOMContentLoaded', applyEmpIdLinkStyle);

    if (window.jQuery) {
        $(document).on('init.dt', applyEmpIdLinkStyle);
    }
})(jQuery);