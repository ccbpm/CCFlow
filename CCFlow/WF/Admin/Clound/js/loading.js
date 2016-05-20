/*add  by  qin*/
window.onload = initPage();
function initPage() {
    var objLoading = document.getElementById("LoadingBar");
    if (objLoading != null) {
        objLoading.style.display = "none";
    }
}
function netInterruptJs() {
$('body').html("<div style='margin-top:40%;margin-left:auto;margin-right:auto;"+
               "width: 600px;color:red; height: 20px;font-family: \'Microsoft YaHei\''>"+
               "您没有连接到互联网,请先连接ccbpm云服务器," +
               "点击<a href='javascript:window.location.reload();'>刷新</a><div/>");

}
