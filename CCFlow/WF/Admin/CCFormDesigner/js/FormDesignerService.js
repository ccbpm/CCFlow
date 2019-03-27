
//检查登录.
function checklogin(fCallback, oScope) {
    /// <summary>检测登录信息</summary>
    /// <param name="fCallback" type="Function">检测完之后，要运行的方法</param>
    /// <param name="oScope" type="Object">检测完之后，要运行的方法的参数</param>

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCFormDesigner");
    var data = handler.DoMethodReturnString("LetLogin");
    if (data.indexOf("err@") != -1) {
        return;
    }
    return;

}