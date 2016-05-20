function POP_Message(id, width, height, caption, title, message, target, action) {
    this.id = id;
    this.title = title;
    this.caption = caption;
    this.message = message;
    this.target = target;
    this.action = action;
    this.width = width ? width : 200;
    this.height = height ? height : 120;
    this.timeout = 150;
    this.speed = 5000;
    this.step = 1;
    this.right = screen.width - 1;
    this.bottom = screen.height;
    this.left = this.right - this.width;
    this.top = this.bottom - this.height;
    this.timer = 0;
    this.pause = false;
    this.close = false;
    this.autoHide = true;
}

var tab = null;
function f_addTab(tabid, text, url) {
    tab.addTabItem({ tabid: tabid, text: text, url: url });
}

/**//* 
*    隐藏消息方法 
*/
POP_Message.prototype.hide = function () {
    if (this.onunload()) {
        var offset = this.height > this.bottom - this.top ? this.height : this.bottom - this.top;
        var me = this;
        if (this.timer > 0) {
            window.clearInterval(me.timer);
        }
        var fun = function () {
            if (me.pause == false || me.close) {
                var x = me.left;
                var y = 0;
                var width = me.width;
                var height = 0;
                if (me.offset > 0) {
                    height = me.offset;
                }

                y = me.bottom - height;

                if (y >= me.bottom) {
                    window.clearInterval(me.timer);
                    me.Pop.hide();
                } else {
                    me.offset = me.offset - me.step;
                }
                me.Pop.show(x, y, width, height);
            }
        }
        this.timer = window.setInterval(fun, this.speed)
    }
}

/**//* 
*    消息卸载事件，可以重写 
*/
POP_Message.prototype.onunload = function () {
    return true;
}
/**//* 
* //要修改为 收件箱  连接
*/
POP_Message.prototype.oncommand = function () {
    //this.close = true;
    this.hide();
    var mesCount = document.getElementById("messageCount").value;
    if (mesCount > 1) {
        var tab = $("#framecenter").ligerGetTabManager();
        //window.open("../AppDemoLigerUI/SmsList.aspx?type=unRead", "未读信息列表", "width: 200,height:400");
        //f_addTab('SmsList', '系统消息', 'SmsList.aspx')
        tab.addTabItem({ tabid: 'SmsList', text: '系统消息', url: 'SmsList.aspx' });
    }
    else {
        var myPK = document.getElementById("messagemyPK").value;
        $.ligerDialog.open({ height: 400, url: 'SmsListDetail.aspx?MyPK=' + myPK, width: 450, showMax: true, showToggle: true, showMin: true, isResize: true, modal: false, title: '邮件详细信息' });

    }
}
/**//* 
*    消息显示方法 
*/
POP_Message.prototype.show = function () {
    var oPopup = window.createPopup(); //IE5.5+     
    this.Pop = oPopup;
    var w = this.width;
    var h = this.height;

    var str = "<DIV style='BORDER-RIGHT: #455690 1px solid; BORDER-TOP: #a6b4cf 1px solid; Z-INDEX: 99999; LEFT: 0px; BORDER-LEFT: #a6b4cf 1px solid; WIDTH: " + w + "px; BORDER-BOTTOM: #455690 1px solid; POSITION: absolute; TOP: 0px; HEIGHT: " + h + "px; BACKGROUND-COLOR: #c9d3f3'>"
    str += "<TABLE style='BORDER-TOP: #ffffff 1px solid; BORDER-LEFT: #ffffff 1px solid' cellSpacing=0 cellPadding=0 width='100%' bgColor=#cfdef4 border=0>"
    str += "<TR>"
    str += "<TD style='FONT-SIZE: 12px;COLOR: #0f2c8c' width=30 height=24></TD>"
    str += "<TD style='PADDING-LEFT: 4px; FONT-WEIGHT: normal; FONT-SIZE: 12px; COLOR: #1f336b; PADDING-TOP: 4px' vAlign=center width='100%'>" + this.caption + "</TD>"
    str += "<TD style='PADDING-RIGHT: 2px; PADDING-TOP: 2px' vAlign=center align=right width=19>"
    str += "<SPAN title=关闭 style='FONT-WEIGHT: bold; FONT-SIZE: 12px; CURSOR: hand; COLOR: red; MARGIN-RIGHT: 4px' id='btSysClose' >×</SPAN></TD>"
    str += "</TR>"
    str += "<TR>"
    str += "<TD style='PADDING-RIGHT: 1px;PADDING-BOTTOM: 1px' colSpan=3 height=" + (h - 28) + ">"
    str += "<DIV style='BORDER-RIGHT: #b9c9ef 1px solid; PADDING-RIGHT: 8px; BORDER-TOP: #728eb8 1px solid; PADDING-LEFT: 8px; FONT-SIZE: 12px; PADDING-BOTTOM: 8px; BORDER-LEFT: #728eb8 1px solid; WIDTH: 100%; COLOR: #1f336b; PADDING-TOP: 8px; BORDER-BOTTOM: #b9c9ef 1px solid; HEIGHT: 100%'>" + this.title + "<BR><BR>"
    //str += "<DIV style='WORD-BREAK: break-all' align=left><A href='javascript:void(0)' hidefocus=false id='btCommand'><FONT color=#ff0000>" + this.message + "</FONT></A></DIV>"
    // str += "<DIV style='WORD-BREAK: break-all' align=left>" + this.message + "</DIV>"
    str += "<DIV style='WORD-BREAK: break-all' align=left>"
    str += this.message
    str += "</DIV>"
    str += "</DIV>"
    str += "</TD>"
    str += "</TR>"
    str += "</TABLE>"
    str += "</DIV>"
    oPopup.document.body.innerHTML = str;


    this.offset = 0;
    var me = this;
    oPopup.document.body.onmouseover = function () { me.pause = true; }
    oPopup.document.body.onmouseout = function () { me.pause = false; }
    var fun = function () {
        var x = me.left;
        var y = 0;
        var width = me.width;
        var height = me.height;
        if (me.offset > me.height) {
            height = me.height;
        } else {
            height = me.offset;
        }
        y = me.bottom - me.offset;
        if (y <= me.top) {
            me.timeout--;
            if (me.timeout == 0) {
                window.clearInterval(me.timer);
                if (me.autoHide) {
                    me.hide();
                }
            }
        } else {
            me.offset = me.offset + me.step;
        }
        me.Pop.show(x, y, width, height);
    }

    this.timer = window.setInterval(fun, this.speed)
    var btClose = oPopup.document.getElementById("btSysClose");
    btClose.onclick = function () {
        me.close = true;
        me.hide();
    }

    var btCommand = oPopup.document.getElementById("btCommand");
    if (btCommand != null) {
        btCommand.onclick = function () {
            me.oncommand();
        }
    }

}
/**//* 
** 设置速度方法 
**/
POP_Message.prototype.speed = function (s) {
    var t = 20;
    try {
        t = praseInt(s);
    } catch (e) { }
    this.speed = t;
}
/**//* 
** 设置步长方法 
**/
POP_Message.prototype.step = function (s) {
    var t = 1;
    try {
        t = praseInt(s);
    } catch (e) { }
    this.step = t;
}

POP_Message.prototype.rect = function (left, right, top, bottom) {
    try {
        this.left = left != null ? left : this.right - this.width;
        this.right = right != null ? right : this.left + this.width;
        this.bottom = bottom != null ? (bottom > screen.height ? screen.height : bottom) : screen.height;
        this.top = top != null ? top : this.bottom - this.height;
    } catch (e) { }
}
