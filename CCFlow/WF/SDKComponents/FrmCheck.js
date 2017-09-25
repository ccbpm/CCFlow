/*
 
 审核组件js.
 
 使用场景:
 1, 使用sdk模式开发,需要用到审核组件.
 2, 使用自定义表单开发,需要用到审核组件.

 使用方法:
 1, 首先在每个节点属性上启用审核组件. 节点属性=》审核组件=》启用审核组件，如果不启用组件该组件就不显示.
 2, 把该FrmCheck.js引入到自己的页面里面去.
 3, 在自己的页面里增加一个 id= FrmCheck 的div , 这个div 的位置就是放审核组件的位置.
 4, 在您的页面里需要引用. ./WF/Scripts/jquery-1.4.1.min.js 如果您的页面中已经有了就不要引用.
 5, 在您的页面里需要引用. ./WF/Scripts/QueryString.js 如果您的页面中已经有了就不要引用.
*/

$(function () {

    var div = document.getElementById('FrmCheck');
    if (div == null || div == undefined) {
        alert('没有找到约定的标记.');
        return;
    }

    var flowNo = GetQueryString("FK_Flow");
    var nodeID = GetQueryString("FK_Node");
    var workID = GetQueryString("WorkID");

    var url = "/WF/WorkOpt/WorkCheck.htm?FK_Flow=" + flowNo + "&FK_Node=" + nodeID + "&WorkID=" + workID;
    var iframe = $("<iframe  style='width:100%; height:100%;'  src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling='no'></iframe>");
    div.appendChild(iframe);

});
