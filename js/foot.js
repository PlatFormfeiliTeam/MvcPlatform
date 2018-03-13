function foottip () {
    $(".showtip1").popover({
        trigger: 'manual',
        placement: 'top',
        title: '<div style="text-align:center; color:red; text-decoration:underline; font-size:12px;"> 群名称：关务云客户服务<br>群账号：526451858</div>',
        html: 'true', //needed to show html of course
        content: '<div id="popOverBox" align="center"> <a target="_blank" href="//shang.qq.com/wpa/qunwpa?idkey=e1d4f03a3e46e7343bc450b824aa98bfcdea1c3ea87de3784c7b0573c430b9cf">'
                  +'<img border="0" src="/Images/float/erweima.png" alt="关务云客户服务" title="关务云客户服务">'
                + '</a></div>',
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
        content: '<div id="popOverBox" align="center"><img src="/Images/float/wechat_fllow.png" /></div>',
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