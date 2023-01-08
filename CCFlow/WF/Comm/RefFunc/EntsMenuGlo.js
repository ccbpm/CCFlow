
//处理 url 根据 MenuModel 菜单类型 解析url.
function DealMenuUrl(menu) {

    if (menu.Icon == "" || menu.Icon == null) {
        menu.Icon = "icon-user";
    }

    if (menu.UrlExt == undefined) menu.UrlExt = menu.Url;

    var basePath = "";
    if (menu.MenuModel === "" || menu.MenuModel === null) {
        return menu;
    }

    if (menu.MenuModel === "SelfUrl") {
        menu.Url = basePath + + menu.UrlExt;
        return menu;
    }

    alert('没有判断的模式:' + menu.MenuModel);
    return menu;
}