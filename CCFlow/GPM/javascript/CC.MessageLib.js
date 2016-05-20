CC = {}
Message = {}

MessageFactory = function () {
    this.Message = new Message.Factory();
}

jQuery(function ($) {
    CC = new MessageFactory();
});


Message.Factory = function () {
    this.showMsg = function (title,msg) {
        $.messager.show({
            title: title,
            msg: "<span><img align='middle' alt='' src='Images/bird.png'/><b>" + msg + "</b></span>",
            timeout: 5000,
            showType: 'show'
        });
    }
    this.showError = function (title, msg) {
        $.messager.show({
            title: title,
            width:300,
            height:160,
            msg: "<span style='color:Red;'><img align='middle' alt='' src='Images/bird.png'/><b>" + msg + "</b></span>",
            timeout: 5000,
            showType: 'show'
        });
    }
}