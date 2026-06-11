(function ($) {
    $(function () {
        var css = '.card-header, .card-header { background: #ff0000 !important; background-color: #ff0000 !important; background-image: none !important; color: #ffffff !important; border-bottom: 1px solid rgba(0,0,0,0.08) !important; }';
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
})(jQuery);