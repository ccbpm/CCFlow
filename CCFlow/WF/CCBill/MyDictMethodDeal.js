
/** 
 * 方法处理的Js 执行一个方法, 要根据方法的属性进行解析.
 *  1. 该js被两个页面引用 MyDict.htm MyDictFrameWork.htm
 *  
*/
function DoMethond(methodID) {

    //不同的方法类型执行不同的操作.
    var en = GetMethoh(methodID);

    if (en.RefMethodType == MethodModel.Link) {
        DoLink(en);
        return;
    }

    //如果要发起流程.
    if (en.MethodModel == MethodModel.FlowBaseData
        || en.MethodModel == MethodModel.FlowNewEntity
        || en.MethodModel == MethodModel.FlowEtc) {
        DoFlow(en);
        return;
    }

    //执行方法.
    DoGenerMethond(en);

    // Done('" + fm.WarningMsg + "', '" + fm.MyPK + "', '" + fm.Name + "', " + fm.WhatAreYouTodo + ", '" + fm.PopWidth + "', '" + fm.PopHeight + "', " + fm.RefMethodType + ", " + fm.MethodModel + ")
}

function GetMethoh(id) {
    for (var i = 0; i < methods.length; i++) {

        var en = methods[i];
        if (en.MyPK == id)
            return en;
    }

    alert("在methods没有找到ID=" + id + " 的数据.");
}

function DoLink(en) {

    //打开链接
    var mlink = new Entity("BP.CCBill.Template.MethodLink", en.MyPK);
    var linkUrl = mlink.MethodDoc_Url;
    if (linkUrl != null && linkUrl != undefined && linkUrl != "") {
        linkUrl = linkUrl.replace("@basePath", basePath);
        linkUrl = DealJsonExp(frmData.MainTable[0], linkUrl);
        var openType = mlink.RefMethodType;
        //模态窗口打开
        if (openType == 0) {
            OpenBootStrapModal(url, "MethodeLink", methodName, w, h, null, true, null, null, null);
        }
        //新窗口打开
        if (openType == 1 || openType == 2) {
            window.open(linkUrl);
        }
    }
}
 
String.prototype.replaceAll = function (FindText, RepText) {
    return this.replace(new RegExp(FindText, "g"), RepText);
}

// 执行方法通用的方法.
function DoGenerMethond(en) {

    var msg = en.WarningMsg; //提示的消息.
    var w = en.PopWidth; //宽度
    var h = en.PopHeight; //高度.

    // msg, funMyPK, methodName, afterOper, w, h, methodType, methodModel

    //执行方法.
    var isHaveAttr = false;
    var attrs = new Entities("BP.Sys.MapAttrs", "FK_MapData", en.MyPK);
    for (var i = 0; i < attrs.length; i++) {
        var attr = attrs[i];
        isHaveAttr = true;
    }

    //带有参数的方法.
    if (isHaveAttr == true) {

        if (w == 0) w = 560;
        if (h == 0) h = 260;

        var url = "./Opt/DoMethodPara.htm?MyPK=" + en.MyPK + "&WorkID=" + GetQueryString("WorkID") + "&FrmID=" + GetQueryString("FrmID");
        //WinOpen(url);
        OpenBootStrapModal(url, "MethodePara", en.Name, w, h, null, false, null, null, function () {
            //afterOper=0 关闭提示窗口，不做任何操作

            //afterOper=1 关闭提示窗口刷新页面
            if (afterOper == 1)
                Reload();

            //afterOper=2 关闭提示窗口跳转到Search.htm
            if (afterOper == 2) {
                if (window.parent.location.href.indexOf("SearchDict.htm") != -1) {
                    window.close();
                }
                else
                    SetHref("./SearchDict.htm?FrmID=" + GetQueryString("FrmID"));
            }
        });
        return;
    }

    if (w == 0) w = 260;
    if (h == 0) h = 160;

    //不带有参数的方法.
    if (msg != '' && msg != null) {
        if (window.confirm(msg) == false)
            return;
    }

    var url = "./Opt/DoMethod.htm?MyPK=" + en.MyPK + "&WorkID=" + GetQueryString("WorkID") + "&FrmID=" + GetQueryString("FrmID");
    //WinOpen(url);
    OpenBootStrapModal(url, "Methode", en.Name, w, h, null, true, null, null, function () {

        //afterOper=0 关闭提示窗口，不做任何操作
        //afterOper=1 关闭提示窗口刷新页面
        if (afterOper == 1)
            Reload();

        //afterOper=2 关闭提示窗口跳转到Search.htm
        if (afterOper == 2) {

            if (window.parent.location.href.indexOf("SearchBill") != -1) {
                window.close();
                return;
            }

            SetHref("./SearchDict.htm?FrmID=" + GetQueryString("FrmID"));
        }
    });
}
