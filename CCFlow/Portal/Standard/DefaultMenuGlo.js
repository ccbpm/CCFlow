
//处理 url 根据 MenuModel 菜单类型 解析url.
function DealMenuUrl(menu) {

    if (menu.UrlExt == undefined) menu.UrlExt = menu.Url;

    var basePath = "";
    if (menu.MenuModel === "" || menu.MenuModel === null) {
        //alert("没有保存菜单标记 MenuModel  " + menu.MenuModel);
        return menu;
    }

    if (menu.MenuModel === "SelfUrl") {
        menu.Url = basePath + menu.UrlExt;
        return menu;
    }

    //如果是修改基础数据..
    if (menu.MenuModel === "FlowBaseData" || menu.MenuModel === "FlowEtc") {

        if (menu.Mark == "Todolist")
            menu.Url = basePath + "/WF/Todolist.htm?FK_Flow=" + menu.Tag1;

        if (menu.Mark == "Runing")
            menu.Url = basePath + "/WF/Runing.htm?FK_Flow=" + menu.Tag1;

        if (menu.Mark == "Start") {
            menu.Url = basePath + "/WF/MyFlow.htm?FK_Flow=" + menu.Tag1;
            menu.Icon = "icon-planct";
        }

        if (menu.Mark == "FlowSearch")
            menu.Url = basePath + "/WF/Search.htm?FK_Flow=" + menu.Tag1;

        if (menu.Mark == "FlowGroup")
            menu.Url = basePath + "/WF/Group.htm?FK_Flow=" + menu.Tag1;

        if (menu.Icon == null || menu.Icon == "") menu.Icon = "icon-paper-plane";

        return menu;
    }

    //新建实体.
    if (menu.MenuModel == "FlowNewEntity") {
        if (menu.Mark == "StartFlow")
            menu.Url = basePath + "/WF/CCBill/Opt/StartFlowByNewEntity.htm?FK_Flow=" + menu.Tag1 + "&MenuNo=" + menu.No;

        // alert(menu.Icon);
        if (menu.Icon === "" || menu.Icon == null) menu.Icon = "icon-paper-plane";

        return menu;
    }

    if (menu.MenuModel == "FlowSearch") {
        menu.Url = basePath + "/WF/Search.htm?FK_Flow=" + menu.Tag;
        if (menu.Icon === "" || menu.Icon == null) menu.Icon = "icon-paper-plane";
        return menu;
    }

    if (menu.MenuModel === "Dict") {
        if (menu.ListModel === 0) //如果是批量编辑模式.
            menu.Url = basePath + "/WF/CCBill/SearchEditer.htm?FrmID=" + menu.UrlExt;
        else
            menu.Url = basePath + "/WF/CCBill/SearchDict.htm?FrmID=" + menu.UrlExt;
        return menu;
    }

    if (menu.MenuModel === "DBList") {
        menu.Url = basePath + "/WF/CCBill/SearchDBList.htm?FrmID=" + menu.UrlExt;
        return menu;
    }

    //流程菜单.
    if (menu.MenuModel == "FlowUrl") {
        // menu.Url = basePath + "/WF/" + menu.Url;
        menu.Url = basePath + "/WF/" + menu.UrlExt;
        return menu;
    }

    if (menu.MenuModel == "DictTable") {

        url = basePath + "/WF/Admin/FoolFormDesigner/SFTableEditData.htm?FK_SFTable=" + menu.UrlExt + "&QueryType=Dict";
        menu.Url = url;
        return menu;
    }

    if (menu.MenuModel === "Bill") {
        menu.Url = basePath + "/WF/CCBill/SearchBill.htm?FrmID=" + menu.UrlExt;
        return menu;
    }

    //独立功能.
    if (menu.MenuModel === "Func" || menu.MenuModel === "StandAloneFunc") {
        menu.Url = basePath + "/WF/CCBill/Sys/Func.htm?FuncNo=" + menu.UrlExt;
        return menu;
    }

    if (menu.MenuModel === "Windows") {
        menu.Url = basePath + "/WF/Portal/Home.htm?PageID=" + menu.No;
        return menu;
    }

    if (menu.MenuModel === "Tabs") {
        menu.Url = basePath + "/WF/Portal/Tabs.htm?PageID=" + menu.No;
        return menu;
    }

    if (menu.MenuModel === "Rpt3D"  ) {
        menu.Url = basePath + "/CCFast/Rpt/Rpt3D.htm?RptNo=" + menu.No;
        return menu;
    }
    if (menu.MenuModel === "RptBlue") {
        menu.Url = basePath + "/WF/Portal/RptBlue.htm?PageID=" + menu.No;
        return menu;
    }
    if (menu.MenuModel === "RptWhite") {
        menu.Url = basePath + "/WF/Portal/RptBlue.htm?PageID=" + menu.No;
        return menu;
    }

    if (menu.Url != "") {
        if (menu.Url.indexOf("?") != -1)
            menu.Url = basePath + menu.Url + "&PageID=" + menu.No;
        else
            menu.Url = basePath + menu.Url + "?PageID=" + menu.No;
        return menu;
    }

    alert('没有判断的模式:[' + menu.MenuModel + "]  urlExt:" + menu.UrlExt);
    return menu;
}