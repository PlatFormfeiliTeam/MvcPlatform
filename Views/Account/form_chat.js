function form_chat_init() {
    var field_image = Ext.create('Ext.Img', {
        id: 'wechat_img',
        src: '',
        width: 150,
        height: 150
        
        
    });




    ///////////////////////////////////
    formchat_panel = Ext.create('Ext.panel.Panel', {
        id: 'formchat_panel',
        //title: '<center>二维码</center>',
        labelAlign: "center",
        margin:'0 500 0 500',
        border: 0,
        items: [
            { layout: 'column', border: 0, margin: '5 0 15 0', items: [field_image] }
        ]
    });
}