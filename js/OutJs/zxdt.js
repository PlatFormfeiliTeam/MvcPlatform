$(function(){		
	//设计案例切换
	$('.title-list li').mouseover(function(){
		var liindex = $('.title-list li').index(this);
		$(this).addClass('on').siblings().removeClass('on');
		$('.product-wrap div.product').eq(liindex).fadeIn(150).siblings('div.product').hide();
		var liWidth = $('.title-list li').width();
		$('.lanrenzhijia .title-list p').stop(false,true).animate({'left' : liindex * liWidth + 'px'},300);
	});
	

	$('.product-wrap .product li').hover(function(){
	    $(this).css("background-color","#CEE4F1");
	    $(this).find('p > a').css('color', '#CEE4F1');
	},function(){
	    $(this).css("background-color", "#fafafa");
		$(this).find('p > a').css('color','#666666');
	});
	});
