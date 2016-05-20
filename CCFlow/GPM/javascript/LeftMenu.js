$(function () {
    Application.data.getLeftMenu(function (js) {
        var pushData = eval('(' + js + ')');
        var leftMenu = $(".cs-west div");
        for (var i = 0, l = pushData.length; i < l; i++) {
            var menu = $("<div title='" + pushData[i].Name + "' data-options='iconCls:'icon-reload',selected:trues'>");
            menu.append("<a id='allowStartCount' class='cs-navi-tab'>");
            menu.append("<img class='img-menu' align='middle' src='Img/Menu/Start.png' />发起</a>");
            menu.append("</div>");
            if (pushData[i].Children.length > 0) {
                for (var j = 0, k = pushData.length; j < k; j++) {


                }
            }
            menu.appendTo(leftMenu);
        }
    }, this);
});