// 菜单处理类
// 非系统菜单
var nonSystemItems = ['Forms', 'Frms', 'Flows', 'flows', 'frms', 'forms']

function MenuConvertTools(webUser, data) {
    this.webUser = webUser;
    this.data = data;
}

var webUser = new WebUser();
var sid = GetQueryString("SID");
function Start() {
    if (webUser.CCBPMRunModel == 2)
        vm.openTab('发起', '../../App/Start.htm');
    else
        vm.openTab('发起', '../Start.htm');
}

function Todolist() {
    if (webUser.CCBPMRunModel == 2)
        vm.openTab('待办', '../../App/Todolist.htm');
    else
        vm.openTab('待办', '../Todolist.htm');
}

function Runing() {
    if (webUser.CCBPMRunModel == 2)
        vm.openTab('在途', '../../App/Runing.htm');
    else
        vm.openTab('在途', '../Runing.htm');
}

function Search() {
    if (webUser.CCBPMRunModel == 2)
        vm.openTab('查询', '../../App/Search.htm');
    else
        vm.openTab('查询', '../Search.htm');
}

function OpenOrg() {

    if (webUser.CCBPMRunModel == 2)
        vm.openTab('组织', '../../App/App/Organization.htm');
    else
        vm.openTab('组织', '../../GPM/Organization.htm');
}

//流程菜单.
MenuConvertTools.prototype.getFlowMenu = function () {

    var flowTree = this.data['FlowTree'] ? this.data['FlowTree'] : [];

    var ccbpmRunModel = this.webUser.CCBPMRunModel;
    var orgNo = this.webUser.OrgNo;

    var flows = this.data['Flows'] ? this.data['Flows'] : [];
    var topFlowNode = [];
    for (var i = 0; i < flowTree.length; i++) {

        if (ccbpmRunModel == 2) {
            if (flowTree[i].ParentNo == '100') {
                flowTree[i].ParentNo = '0';
                flowTree[i].No = '1';
            }
            if (flowTree[i].ParentNo === orgNo)
                flowTree[i].ParentNo = '1';
        }

        if (flowTree[i].ParentNo != "0")
            continue;

        var en = flowTree[i];
        en.Icon = "icon-organization";
        en.Name = "流程设计";
        topFlowNode.push(en);
        break;
    }
    for (var i = 0; i < topFlowNode.length; i++) {
        topFlowNode[i].children = [];
        //便利子级目录.
        for (var j = 0; j < flowTree.length; j++) {
            if (topFlowNode[i].No != flowTree[j].ParentNo)
                continue;
            topFlowNode[i].open = false

            topFlowNode[i].type = "flow"
            flowTree[j].children = [];
            flowTree.Icon = "icon-layers";

            //加入流程节点.
            for (var k = 0; k < flows.length; k++) {
                if (flows[k].TreeNo === flowTree[j].No) {
                    flowTree[j].open = false;
                    var en = flows[k];
                    en.type = 'flow'
                    // if (en.WorkType == 1)
                    en.Icon = "icon-heart";
                    en.Url = "../Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + en.No + "&OrgNo=" + this.webUser.OrgNo + "&SID=" + sid + "&UserNo=" + this.webUser.No;
                    en.Url = en.Url + "&From=Ver2021";
                    //alert(en.Url);
                    flowTree[j].type = "flow"
                    flowTree[j].children.push(en);
                }
            }
            var en = flowTree[j];
            en.Icon = "icon-organization";
            en.type = "flow";
            topFlowNode[i].children.push(en);
        }
    }
    if (topFlowNode.length > 0)
        return topFlowNode[0];
    else
        return {};
}
// 获取表单菜单
MenuConvertTools.prototype.getFormMenu = function () {
    var formTree = this.data['FrmTree'] ? this.data['FrmTree'] : [];


    var ccbpmRunModel = this.webUser.CCBPMRunModel;
    var orgNo = this.webUser.OrgNo;

    var forms = this.data['Frms'] ? this.data['Frms'] : [];
    var topFormNode = [];
    for (var i = 0; i < formTree.length; i++) {

        if (ccbpmRunModel == 2) {
            if (formTree[i].ParentNo == '100') {
                formTree[i].ParentNo = '0';
                formTree[i].No = '1';
            }
            if (formTree[i].ParentNo === orgNo)
                formTree[i].ParentNo = '1';
        }

        if (formTree[i].ParentNo === "0") {
            formTree[i].Icon = "icon-layers";
            formTree[i].Name = "表单设计";
            topFormNode.push(formTree[i]);
        }
    }

    for (var i = 0; i < topFormNode.length; i++) {
        topFormNode[i].children = [];
        topFormNode[i].type = "form"
        for (var j = 0; j < formTree.length; j++) {
            if (topFormNode[i].No === formTree[j].ParentNo) {
                topFormNode[i].open = false;
                formTree[j].children = [];
                for (var k = 0; k < forms.length; k++) {

                    var frm = forms[k];
                    if (frm.TreeNo === formTree[j].No) {
                        formTree[j].open = false;
                        frm.Icon = "icon-doc";
                        frm.type = 'form'
                        if (parseInt(frm.FrmType) === 1)
                            frm.Icon = "icon-doc";

                        frm.Url = "../Admin/CCFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + frm.No + "&From=2021ver&SID=" + sid;

                        formTree[j].children.push(frm);
                        formTree[j].type = "form"
                    }
                }

                var en = formTree[j];
                en.Icon = "icon-layers";
                en.type = 'form';
                topFormNode[i].children.push(en);
            }
        }
    }
    if (topFormNode.length > 0)
        return topFormNode[0];
    else
        return {};
}
var systemNodes;
var moduleNode;
var menuNode;
// 获取系统菜单
MenuConvertTools.prototype.getSystemMenus = function () {

    systemNodes = this.data['System'];
    moduleNode = this.data['Module'];
    menuNode = this.data['Menu'];

    var adminMenuNodes = [];
    for (var idx = 0; idx < systemNodes.length; idx++) {

        var systemNode = systemNodes[idx];
        if (nonSystemItems.indexOf(systemNode.No) > -1) continue;
        systemNode.children = [];
        systemNode.open = false;
        if (systemNode.No === "System") {
            systemNode.Icon = 'icon-settings';
            systemNode.Name = "系统管理";
        }

        if (systemNode.Icon === '')
            systemNode.Icon = 'icon-settings';

        for (var idxModule = 0; idxModule < moduleNode.length; idxModule++) {

            var moduleEn = moduleNode[idxModule];
            if (moduleEn.SystemNo !== systemNode.No)
                continue; //如果不是本系统的.
            moduleEn.children = [];
            if (moduleEn.Icon === "" || moduleEn.Icon == null || moduleEn.Icon === "")
                moduleEn.Icon = 'icon-list';
            moduleEn.open = false;
            //增加菜单.
            for (var idxMenu = 0; idxMenu < menuNode.length; idxMenu++) {

                var menu = menuNode[idxMenu];
                if (moduleEn.No !== menu.ModuleNo)
                    continue; // 不是本模块的。
                if (menu.MenuModel == "FlowEntityBatchStart")
                    continue;
                menu = DealMenuUrl(menu);

                if (menu.Icon === '')
                    menu.Icon = 'icon-user';

                moduleEn.children.push(menu);
            }
            systemNode.children.push(moduleEn);

        }
        adminMenuNodes.push(systemNode)
    }
    return adminMenuNodes
}
MenuConvertTools.prototype.convertToTreeData = function () {
    var topNodes = [];
    if (this.webUser.No === "admin" || parseInt(this.webUser.IsAdmin) === 1) {
        if (this.getFlowMenu(this.data).length !== 0)
            topNodes.push(this.getFlowMenu(this.data));
        if (this.getFormMenu(this.data).length !== 0)
            topNodes.push(this.getFormMenu(this.data));
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
