/**
*加载表单自定义脚本
*作者：dai
*时间：2017.4.15
*/

function LoadFormSelfJavaScript(FK_MapData) {
    var src = "/DataUser/JSLibData/" + FK_MapData + "_Self.js";
    var oHead = document.getElementsByTagName('HEAD').item(0);
    var oScript = document.createElement("script");
    oScript.type = "text/javascript";
    oScript.src = src;
    oHead.appendChild(oScript);
}