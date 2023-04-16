// 菜单处理类
// 非系统菜单
var nonSystemItems = ['Forms', 'Frms', 'Flows', 'flows', 'frms', 'forms']

function MenuConvertTools(webUser, data) {
    this.webUser = webUser;
    this.data = data;
}

var webUser = new WebUser();
var sid = GetQueryString("Token");
function Start() {
    if (webUser.CCBPMRunModel == 2)
        vm.openTab(vm.GetNameByLange('faqi'), basePath + '/App/Start.htm');
    else
        vm.openTab(vm.GetNameByLange('faqi'), basePath + '/WF/Start.htm');
}

function Todolist() {
    if (webUser.CCBPMRunModel == 2)
        vm.openTab(vm.GetNameByLange('daiban'), basePath + '/App/Todolist.htm');
    else
        vm.openTab(vm.GetNameByLange('daiban'), basePath + '/WF/Todolist.htm');
}

function Runing() {
    if (webUser.CCBPMRunModel == 2)
        vm.openTab(vm.GetNameByLange('zaitu'), basePath + '/App/Runing.htm');
    else
        vm.openTab(vm.GetNameByLange('zaitu'), basePath + '/WF/Runing.htm');
}

function Batch() {
    if (webUser.CCBPMRunModel == 2)
        vm.openTab(vm.GetNameByLange('pichuli'), basePath + '/App/Batch.htm');
    else
        vm.openTab(vm.GetNameByLange('pichuli'), basePath + '/WF/Batch.htm');
}

function Search() {
    if (webUser.CCBPMRunModel == 2)
        vm.openTab(vm.GetNameByLange('chaxun'), basePath + '/App/Search.htm');
    else
        vm.openTab(vm.GetNameByLange('chaxun'), basePath + '/WF/Search.htm');
}


function OpenMessage() {
    vm.openTab(vm.GetNameByLange('xiaoxi'), basePath + '/WF/Portal/Message.htm');
}


function Infos() {
    vm.openTab('通知', basePath + '/CCFast/Infos/Default.htm');
}


function BBS() {
    vm.openTab('BBS', basePath + '/CCFast/BBS/Default.htm');
}


function Fasts() {
    var urlEnd = "?Token=" + GetQueryString("Token") + "&OrgNo=" + GetQueryString("OrgNo") + "&UserNo=" + GetQueryString("UserNo");
    vm.openTab(vm.GetNameByLange('didaima'), basePath + '/WF/GPM/Menus.htm' + urlEnd);
}

function Flows() {
    var urlEnd = "?Token=" + GetQueryString("Token") + "&OrgNo=" + GetQueryString("OrgNo") + "&UserNo=" + GetQueryString("UserNo");
    vm.openTab(vm.GetNameByLange('liucheng'), basePath + '/WF/Portal/Flows.htm' + urlEnd);
}

function Frms() {
    var urlEnd = "?Token=" + GetQueryString("Token") + "&OrgNo=" + GetQueryString("OrgNo") + "&UserNo=" + GetQueryString("UserNo");
    vm.openTab(vm.GetNameByLange('biaodan'), basePath + '/WF/Portal/Frms.htm' + urlEnd);
}

function OpenOrg() {

    var urlEnd = "?Token=" + GetQueryString("Token") + "&OrgNo=" + GetQueryString("OrgNo") + "&UserNo=" + GetQueryString("UserNo");

    if (webUser.CCBPMRunModel == 2)
        vm.openTab(vm.GetNameByLange('zuzi'), basePath + '/App/App/Organization.htm' + urlEnd);
    else
        vm.openTab(vm.GetNameByLange('zuzi'), basePath + '/GPM/Organization.htm' + urlEnd);
}

function GoToOrgEn() {

    var urlEnd = "&Token=" + GetQueryString("Token") + "&OrgNo=" + GetQueryString("OrgNo") + "&UserNo=" + GetQueryString("UserNo");
    var url = basePath + '/WF/Comm/En.htm?EnName=BP.WF.Admin.Org&No=ccflow' + urlEnd;
    window.location.href = filterXSS(url);
    //WinOpenFull(url);
    return;


    if (webUser.CCBPMRunModel == 2)
        vm.openTab('系统管理', url);
    else
        vm.openTab('系统管理', url);
}



var systemNodes;
var moduleNode;
var menuNode;

// 获取系统菜单
MenuConvertTools.prototype.getSystemMenus = function () {

    systemNodes = [];
    systems = this.data['System'];
    moduleNode = this.data['Module'];
    menuNode = this.data['Menu'];

    var webUser = new WebUser();

    var adminMenuNodes = [];
    //循环系统.
    for (var idx = 0; idx < systems.length; idx++) {
        if(systems[idx].No === "System")
            systems[idx].IsEnable = 1;
        if(systems[idx].IsEnable == 0)
            continue;
        var systemNode = systems[idx];
        if (nonSystemItems.indexOf(systemNode.No) > -1) continue;
        systemNode.children = [];
        systemNode.open = false;
        if (systemNode.No === "System") {
            systemNode.Icon = 'icon-settings';
            systemNode.Name = "系统管理";
        }

        if (systemNode.Icon === '')
            systemNode.Icon = 'icon-settings';

        //循环模块.
        for (var idxModule = 0; idxModule < moduleNode.length; idxModule++) {

            var moduleEn = moduleNode[idxModule];
            if (moduleEn.SystemNo !== systemNode.No)
                continue; //如果不是本系统的.
            if(systemNode.No === "System")
                moduleEn.IsEnable = 1;
            if(moduleEn.IsEnable == 0)
                continue;
            moduleEn.children = [];
            if (moduleEn.Icon === "" || moduleEn.Icon == null || moduleEn.Icon === "")
                moduleEn.Icon = 'icon-list';
            moduleEn.open = false;


            //增加菜单.
            for (var idxMenu = 0; idxMenu < menuNode.length; idxMenu++) {

                var menu = menuNode[idxMenu];
                if (moduleEn.No !== menu.ModuleNo)
                    continue; // 不是本模块的。
                if(systemNode.No === "System")
                    menu.IsEnable = 1;
                if(menu.IsEnable == 0)
                    continue;
                if (menu.MenuModel == "FlowEntityBatchStart")
                    continue;

                menu = DealMenuUrl(menu);

                if (menu.Url.indexOf('@WebUser.FK_Dept') > 0)
                    menu.Url = menu.Url.replace('@WebUser.FK_Dept', webUser.FK_Dept);

                if (menu.Url.indexOf('@WebUser.No') > 0)
                    menu.Url = menu.Url.replace('@WebUser.No', webUser.No);

                if (menu.Url.indexOf('@WebUser.OrgNo') > 0)
                    menu.Url = menu.Url.replace('@WebUser.OrgNo', webUser.OrgNo);

                if (menu.Icon === '')
                    menu.Icon = 'icon-user';

                moduleEn.children.push(menu);
            }
            systemNode.children.push(moduleEn);

        }
        if(this.webUser.No === "admin" || parseInt(this.webUser.IsAdmin) === 1)
            adminMenuNodes.push(systemNode)
        else if(systemNode.children.length>0)
            adminMenuNodes.push(systemNode)
    }
    return adminMenuNodes
}

MenuConvertTools.prototype.convertToTreeData = function () {
    var topNodes = [];
    if (this.webUser.No === "admin" || parseInt(this.webUser.IsAdmin) === 1) {
        // if (this.getFlowMenu(this.data).length !== 0)
        //     topNodes.push(this.getFlowMenu(this.data));
        //  if (this.getFormMenu(this.data).length !== 0)
        //     topNodes.push(this.getFormMenu(this.data));
    }
    topNodes = topNodes.concat(this.getSystemMenus(this.data))
    // console.log(topNodes)
    return topNodes
}

function getPortalConfigByKey(key, defVal) {
    if (typeof PortalConfig == "undefined") {
        PortalConfig = {};
        PortalConfig[key] = defVal;
        return defVal;
    }
    if (PortalConfig[key] == undefined)
        PortalConfig[key] = defVal;
    return PortalConfig[key];
}
