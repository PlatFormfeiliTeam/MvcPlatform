function foottip () {
    $(".showtip1").popover({
        trigger: 'manual',
        placement: 'top',
        title: '<div style="text-align:center; color:red; text-decoration:underline; font-size:12px;"> 点击或扫描二维码<br>联系我们</div>',
        html: 'true', //needed to show html of course
        content: '<div id="popOverBox" align="center"><img src="/Images/float/qq.jpg" /></div>',
        animation: false
    }).on("mouseenter", function () {
        var _this = this;
        $(this).popover("show");
        $(this).siblings(".popover").on("mouseleave", function () {
            $(_this).popover('hide');
        });
    }).on("mouseleave", function () {
        var _this = this;
        setTimeout(function () {
            if (!$(".popover:hover").length) {
                $(_this).popover("hide")
            }
        }, 100);
    });


    $(".showtip2").popover({
        trigger: 'manual',
        placement: 'top',
        title: '<div style="text-align:center; color:red; text-decoration:underline; font-size:12px;">扫描二维码，关注我们</div>',
        html: 'true', //needed to show html of course
        content: '<div id="popOverBox" align="center"><img src="/Images/float/erweima.png" /></div>',
        animation: false
    }).on("mouseenter", function () {
        var _this = this;
        $(this).popover("show");
        $(this).siblings(".popover").on("mouseleave", function () {
            $(_this).popover('hide');
        });
    }).on("mouseleave", function () {
        var _this = this;
        setTimeout(function () {
            if (!$(".popover:hover").length) {
                $(_this).popover("hide")
            }
        }, 100);
    });


    $(".showtip3").popover({
        trigger: 'manual',
        placement: 'top',
        title: '<div style="text-align:center; color:red; text-decoration:underline; font-size:14px;"> 微博</div>',
        html: 'true', //needed to show html of course
        content: '<div id="popOverBox"></div>',
        animation: false
    }).on("mouseenter", function () {
        var _this = this;
        $(this).popover("show");
        $(this).siblings(".popover").on("mouseleave", function () {
            $(_this).popover('hide');
        });
    }).on("mouseleave", function () {
        var _this = this;
        setTimeout(function () {
            if (!$(".popover:hover").length) {
                $(_this).popover("hide")
            }
        }, 100);
    });
}