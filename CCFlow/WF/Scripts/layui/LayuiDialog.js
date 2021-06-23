﻿/**
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
function OpenLayuiDialog(url, title, dlgWidth, dlgHeight, offset, isRefresh, isShowOkBtn, IsShowCloseBtn, okBtnFunc, dlgClosedFunc) {

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
   /* if (window.parent) {
        w = window.parent;
        if (url.indexOf("../../../") != -1)
            url = url.replace("../../../", "../../");
        else if (url.indexOf("../../") != -1)
            url = url.replace("../../", "../");
        else if (url.indexOf("../") != -1) {
            w = window;
        }
        else if (url.indexOf("./") != -1)
            w = window;
    }
        */
    w.layer.open({
        type: 2 //此处以iframe举例
        , title: title
        , area: [dlgWidth + 'px', dlgHeight+'%']
        , maxmin: true
        , shadeClose: true
        , offset: offset
        , content: url
        , btn: btn
        , yes: function () {
            okBtnFunc;
        }
        , btn2: function () {
            dlgClosedFunc
            layer.closeAll();
        },
        cancel: function (index, layero) {
            debugger
            if (isRefresh == true)
                location.reload();
        },
        end: function () {
            debugger
            if (isRefresh == true)
                location.reload();
        }
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