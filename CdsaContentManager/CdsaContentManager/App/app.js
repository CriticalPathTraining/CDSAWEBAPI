var myApp;
(function (myApp) {
    var params = new URLSearchParams(window.location.search);
    var leftNavCollapsed = !params.has("collapsed");
    ;
    console.log(leftNavCollapsed);
    $(function () {
        console.log("Hey");
        $("#left-nav-toggle").click(onNavigationToggle);
        $(window).resize(updateUI);
        onNavigationToggle();
        $.each($("a"), function (index, element) {
            if (element.href === window.location.href) {
                $(element).addClass("active");
            }
        });
    });
    function onNavigationToggle() {
        leftNavCollapsed = !leftNavCollapsed;
        updateUI();
    }
    function onNavPickerSelect(evt) {
        var targetId = evt.currentTarget.id;
        var index = parseInt(targetId);
        console.log("index: " + index);
    }
    function updateUI() {
        console.log("Hello");
        if (leftNavCollapsed) {
            $("#left-nav").addClass("navigationPaneCollapsed");
            $("#content-body").addClass("contentBodyNavigationCollapsed");
            $(".leftNavItemCaption").addClass("leftNavHide").hide();
            $.each($("a.leftNavLink"), function (index, element) {
                console.log(element.href);
                element.href = element.href += "?collapsed=true";
            });
        }
        else {
            $("#left-nav").removeClass("navigationPaneCollapsed");
            $("#content-body").removeClass("contentBodyNavigationCollapsed");
            $(".leftNavItemCaption").removeClass("leftNavHide").show();
            $.each($("a.leftNavLink"), function (index, element) {
                console.log(element.href);
                element.href = element.href.split("?")[0];
            });
        }
        var windowHeight = $(window).height();
        var bannerHeight = $("#banner").height();
        $("#left-nav").height(windowHeight - bannerHeight);
        //console.log($(window).width() - $("#left-nav").width());
    }
    myApp.updateUI = updateUI;
})(myApp || (myApp = {}));
//# sourceMappingURL=app.js.map