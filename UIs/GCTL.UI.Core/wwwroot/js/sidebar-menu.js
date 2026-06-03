$.sidebarMenu = function (menu) {
    var animationSpeed = 300,
        subMenuSelector = '.sidebar-submenu';

    $(menu).on('click', 'li a', function (e) {
        e.stopImmediatePropagation();
        e.stopPropagation();

        var $this = $(this);
        var checkElement = $this.next();

        if (checkElement.is(subMenuSelector) && checkElement.is(':visible')) {
            checkElement.slideUp(animationSpeed, function () {
                checkElement.removeClass('menu-open');
            });
            checkElement.parent("li").removeClass("active");
        }

        //If the menu is not visible
        else if ((checkElement.is(subMenuSelector)) && (!checkElement.is(':visible'))) {
            //Get the parent menu
            var parent = $this.parents('ul').first();
            //Close all open menus within the parent
            var ul = parent.find('ul:visible').slideUp(animationSpeed);
            //Remove the menu-open class from the parent
            ul.removeClass('menu-open');
            //Get the parent li
            var parent_li = $this.parent("li");

            //Open the target menu and add the menu-open class
            checkElement.slideDown(animationSpeed, function () {
                //Add the class active to the parent li
                checkElement.addClass('menu-open');
                parent.find('li.active').removeClass('active');
                parent_li.addClass('active');
            });
        }
        //if this isn't a link, prevent the page from being redirected
        if (checkElement.is(subMenuSelector)) {
            e.preventDefault();
        }
    });
}



// sidebar menu js
$("body").on("click", "#left_menu_open_close", function () {
    // close and open 
    if ($("#left_menu").width() < 200) {
        $(".brand-text").fadeIn("fast");
        $("#left_menu").width(200)

        $("#left_menu").addClass("menu_open_close")
        $("#main_area").css('margin-left', '200px');
        $(".sidebar-menu li a span,.sidebar-menu li a small,.sidebar-menu li a i:last-child").show()
        $(".sidebar-menu li ul").removeClass('mini_sidebar')
        $("#left_menu").css("overflow-x", "hidden")
    } else {
        $(".brand-text").fadeOut("fast");
        $("#left_menu").width(120)
        $("#main_area").css('margin-left', '120px');
        $(".sidebar-menu li a span,.sidebar-menu li a small,.sidebar-menu li a i:last-child").hide()
        $(".sidebar-menu li ul").addClass('mini_sidebar')
        $(".sidebar-menu li ul ul").removeClass('mini_sidebar')
        $(".sidebar-menu ul li a span,.sidebar-menu ul li a small,.sidebar-menu ul li a i:last-child").show()
        // $(".sidebar-menu").children("ul")
        $("#left_menu").css("overflow-x", "visible")

    }
});

// menu inisilize
$.sidebarMenu($('.sidebar-menu'))
// inisialise scroll
// $("#left_menu").mCustomScrollbar(); //not apply admin sidebar





