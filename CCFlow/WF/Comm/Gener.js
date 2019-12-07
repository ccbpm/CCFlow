var loadWebUser = null;
var url = window.location.href.toLowerCase();
if (url.indexOf('login.htm') == -1 && url.indexOf('dbinstall.htm') == -1) {
    loadWebUser = new WebUser();
}
//初始化页面
$(function () {
    //   debugger;
    if (plant == "CCFlow") {
        // CCFlow
        dynamicHandler = basePath + "/WF/Comm/Handler.ashx";
    } else {
        // JFlow
        dynamicHandler = basePath + "/WF/Comm/ProcessRequest.do";
    }
    //判断登录权限.

    if (url.indexOf('login.htm') == -1 && url.indexOf('dbinstall.htm') == -1) {

        if (loadWebUser != null && (loadWebUser.No == "" || loadWebUser.No == undefined || loadWebUser.No == null)) {
            dynamicHandler = "";
            alert("登录信息丢失,请重新登录.");
            return;
        }

        //如果进入了管理员目录.
        if (url.indexOf("/admin/") != -1 && loadWebUser.No != "admin") {
            dynamicHandler = "";
            alert("管理员登录信息丢失,请重新登录,当前用户[" + loadWebUser.No + "]不能操作管理员目录功能.");
            return;
        }
    }

});