/**
 * 居中弹出
 * @param {any} url 请求的url
 * @param {any} title 标题,标题为空时，这弹出框不显示title
 * @param {any} dlgWidth 弹出框宽度
 * @param {any} dlgHeight 弹出框高度
 * @param {any} offset 弹出框的位置 "auto":垂直居中 "r":右侧弹出(可以不设置高度)
 * @param {any} isRefresh 是否刷新父页面
 * @param {any} isShowOkBtn 是否显示确定按钮
 * @param {any} IsShowCloseBtn 是否显示关闭按钮
 * @param {any} okBtnFunc 确定执行的方法
 * @param {any} dlgClosedFunc 关闭执行的方法
 */
function OpenLayuiDialog(url, title, dlgWidth, dlgHeight, offset, isRefresh, isShowOkBtn, IsShowCloseBtn, okBtnFunc, dlgClosedFunc,reloadUrl,showCloseBtn) {

    title = title == null || title == undefined ? "" : title;
    var btn = [];
    if (isShowOkBtn != undefined && isShowOkBtn == true)
        btn[0] = "确定";
    if (IsShowCloseBtn != undefined && IsShowCloseBtn == true) {
        btn.length == 1 ? btn[1] = "取消" : btn[0] = "取消";
    }
    offset = offset == null || offset == undefined || offset == "" ? 'r' : offset;
    dlgHeight = dlgHeight == null || dlgHeight == undefined || dlgHeight == 0 ? 100 : dlgHeight;
  
    if (dlgWidth == null || dlgWidth == 0) {
        if (window.innerWidth)
            dlgWidth = window.innerWidth;
        else if ((document.body) && (document.body.clientWidth))
            dlgWidth = document.body.clientWidth;
        dlgWidth = dlgWidth / 2;
    }

     //如果超过屏幕的宽度，就按屏幕宽度计算。
    if (dlgWidth > window.innerWidth)  dlgWidth=window.innerWidth -150;

    var w = window;

    showCloseBtn = showCloseBtn == null || showCloseBtn == undefined || showCloseBtn === "" ? 1 : showCloseBtn;
    w.layer.open({
        type: 2 //此处以iframe举例
        , title: title
        , id:"dlg"
        , area: [dlgWidth + 'px', dlgHeight+'%']
        , maxmin: showCloseBtn==0?false:offset=="r"?false:true
        , shadeClose: true
        , closeBtn: showCloseBtn
        , offset: offset
        , content: url
        , btn: btn
        , yes: function () {
            if (okBtnFunc)
                okBtnFunc();
            layer.closeAll();
        }
        , btn2: function () {
            if (dlgClosedFunc)
                dlgClosedFunc();
            layer.closeAll();
        },
        cancel: function (index, layero) {
            if (dlgClosedFunc)
                dlgClosedFunc();
            if (isRefresh == true)
            {
            if (reloadUrl==null || reloadUrl=='' )
                location.reload();
            else
                location.href= reloadUrl;
            }


        },
        end: function () {
            if (dlgClosedFunc)
                dlgClosedFunc();
            if (isRefresh == true)
            {
                if (reloadUrl==null || reloadUrl=='' )
                      location.reload();
                else
                      location.href= reloadUrl;
            }
        }
        , success: function (layero) {
            layero.find('.layui-layer-min').remove();
        },
    });
    if (offset == "r")
        $(".layui-layer-setwin .layui-layer-close2").css("right", "-18px").css("top", "-18px");
}

/**
 * 居中弹出
 * @param {any} url 请求的url
 * @param {any} title 标题,标题为空时，这弹出框不显示title
 * @param {any} dlgWidth 弹出框宽度
 * @param {any} dlgHeight 弹出框高度
 */
function OpenFullLayuiDialog(url, title) {

    title = title == null || title == undefined ? "" : title;
    var w = window;
    w.layer.open({
        type: 2 //此处以iframe举例
        , title: title
        , id: "dlg"
        , area: ['100%','100%']
        , maxmin: false
   
        , offset: ['0px', '0px']
        , content: url
        , yes: function () {
            layer.closeAll();
        }
        , btn2: function () {
            layer.closeAll();
        },
         success: function (layero) {
            layero.find('.layui-layer-min').remove();
        },
    });
    
}
/**
 * 右侧呼出
 * @param {any} urlExt 请求的url
 * @param {any} title 标题
 * @param {any} dlgWidth 页面宽度
 * @param {any} isRefresh 是否刷新
 */
function LayuiPopRight(url, title, dlgWidth, isRefresh) {
    if (dlgWidth == null || dlgWidth == 0) {
        if (window.innerWidth)
            dlgWidth = window.innerWidth;
        else if ((document.body) && (document.body.clientWidth))
            dlgWidth = document.body.clientWidth;
        dlgWidth = dlgWidth/2;
    }
    top.layui.admin.popupRight({
        id: 'Lay_PopupRight'
        , area: [dlgWidth + "px", '100%']
        , success: function () {
            top.layui.view(this.id).render('system/comm', {
                url: url,
                //title: title,
               
            });
        }
        , end: function () {
            if (isRefresh == true)
                window.location.reload();
                //layui.form.render();
                
        }

    });
}

/**
 * 居中弹出
 * @param {any} content 请求的内容
 * @param {any} title 标题,标题为空时，这弹出框不显示title
 * @param {any} dlgWidth 弹出框宽度
 * @param {any} dlgHeight 弹出框高度
 * @param {any} isRefresh 是否刷新父页面
 * @param {any} isShowOkBtn 是否显示确定按钮
 * @param {any} IsShowCloseBtn 是否显示关闭按钮
 * @param {any} okBtnFunc 确定执行的方法
 * @param {any} dlgClosedFunc 关闭执行的方法
 */
function OpenOtherLayuiDialog(content,title, dlgWidth, dlgHeight,divID, isRefresh, isShowOkBtn, IsShowCloseBtn, okBtnFunc, dlgClosedFunc) {

    title = title == null || title == undefined ? "" : title;
    var btn = [];
    if (isShowOkBtn != undefined && isShowOkBtn == true)
        btn[0] = "确定";
    if (IsShowCloseBtn != undefined && IsShowCloseBtn == true) {
        btn.length == 1 ? btn[1] = "取消" : btn[0] = "取消";
    }
    dlgHeight = dlgHeight == null || dlgHeight == undefined || dlgHeight == 0 ? 100 : dlgHeight;

    if (dlgWidth == null || dlgWidth == 0) {
        if (window.innerWidth)
            dlgWidth = window.innerWidth;
        else if ((document.body) && (document.body.clientWidth))
            dlgWidth = document.body.clientWidth;
        dlgWidth = dlgWidth / 2;
    }

    //如果超过屏幕的宽度，就按屏幕宽度计算。
    if (dlgWidth > window.innerWidth) dlgWidth = window.innerWidth - 150;

    var w = window;
 
    w.layer.open({
        type: 1 //此处以iframe举例
        , title: title
        , id: divID == null || divID == undefined ? "dlg" : divID
        , area: [dlgWidth + 'px', dlgHeight + '%']
        , maxmin: true
        , shadeClose: true
        , content: ""
        , btn: btn
        ,success: function (layero, index) {
            cceval(content);
            $("#" + this.id).show();
        }
        , yes: function () {
            if (okBtnFunc)
                okBtnFunc();
            layer.closeAll();
        }
        , btn2: function () {
            if (dlgClosedFunc)
                dlgClosedFunc();
            layer.closeAll();
        },
        cancel: function (index, layero) {
            if (dlgClosedFunc)
                dlgClosedFunc();
            if (isRefresh == true)
                location.reload();
        },
        end: function () {
            if (dlgClosedFunc)
                dlgClosedFunc();
            if (isRefresh == true)
                location.reload();
        }
    });
   
}