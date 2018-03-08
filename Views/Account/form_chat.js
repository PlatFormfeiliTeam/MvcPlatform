function form_chat_init() {


    var field_wx_logo = Ext.create('Ext.Img',
        {
            id: 'wechat_log',
            src: '/Images/wx_logo.png',
            
            width: 36,
            height: 36
        });

    var field_wx = {
        xtype: 'label',
        margin: '0,20,0,0',
        html: '<h4 style="margin-top:9px;margin-bottom:2px"><span><i></i>&nbsp;微信</span></h4>'
    }

    var field_follow = {
        xtype: 'label',
        margin: '0,20,0,0',
        html: '<h4 style="margin-top:20px;margin-bottom:10px"><span><i></i>&nbsp;&nbsp;关注关务云</span></h4><div style="width: 1200px;border-bottom: 1px solid #DFE9F6;"></div>'
    }

    var field_follow2 = {
        xtype: 'label',
        margin: '0,20,0,0',
        html: '<h4 style="margin-top:40px;margin-bottom:10px"><span><i></i><br/>&nbsp;关注关务云微信公众号，实时接收看板消息提醒！</span></h4>'
    }

    var field_wx_follow_img = Ext.create('Ext.Img',
        {
            id: 'follow',
            src: '/Images/follow.jpg',
            margin: '0,20,0,0',
            
            width: 160,
            height: 160
        });

    var field_follow_tip = {
        xtype: 'label',
        //margin: '0,0,0,0',
        html: '<h5><span><i></i>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;公众号:关务云</span></h5>'
    }

    var button_wx = {
        id: 'wx_button',
        xtype: 'button',
        text: ' 绑定 ',
        style: {
            marginBottom: '10px', //距底部高度
            marginLeft: '430px', //距左边宽度
            marginRight: '10px', //距右边宽度
            marginTop:'8px'
        },
        handler: function () {
            Ext.Ajax.request({
                url: "/Account/LoadCurrentUser",
                success: function (response, option) {
                    addCustomer_Win();
                    var data = Ext.decode(response.responseText);
                    Ext.getCmp('wechat_img').setSrc("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + data.a.ticket + "");
                }
            });

        }
    }

    ///////////////////////////////////
    formchat_panel = Ext.create('Ext.panel.Panel',
        {
            id: 'formchat_panel',
            labelAlign: "center",
            border: 0,
            height: 350,
            items: [
                { layout: 'column',margin:'20,0,0,0', border: 0, items: [field_wx_logo, field_wx, button_wx] },
                { layout: 'column', border: 0, items: [field_follow] },
                { layout: 'column', border: 0, items: [field_wx_follow_img, field_follow2] },
                { layout: 'column', border: 0, items: [field_follow_tip] }
            ]
        });

    //弹出绑定
    function form_bind() {
        var field_image = Ext.create('Ext.Img',
            {
                id: 'wechat_img',
                src: '',
                width: 300,
                height: 300
            });
        var field_wx_tip = {
            xtype: 'label',
            margin: '0,20,0,5',
            html: '<h4><span><i></i>&nbsp;微信扫描二维码，绑定账号</span></h4>'
        }
        var formpanel_Win = Ext.create('Ext.form.Panel',
            {
                id: 'formpanel_Win',
                
                border: 0,
                buttonAlign: 'center',
                items: [
                    { layout: 'column', height: 300, margin: '5 0 0 200', border: 0, items: [field_image] },
                    { layout: 'column', margin: '0 0 0 230', border: 0, items: [field_wx_tip] }
                ]
            });

        
    }

    function addCustomer_Win() {
        form_bind();
        var win = Ext.create("Ext.window.Window",
            {
                id: "win_d",
                title: '微信绑定',
                plain: true,                
                x: 600,
                y:96,
                width: 700,
                height: 500,
                modal: true,
                items: [Ext.getCmp('formpanel_Win')]
            });
        win.show();
    }
}