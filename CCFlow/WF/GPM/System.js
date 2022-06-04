/// <reference path="../scripts/layui/layuidialog.js" />
/// <reference path="../scripts/layui/layuidialog.js" />
function GenerDoc(menu) {
    return menu;
}
 
// 增加lab.
function addTab(url, title) {
    window.top.vm.openTab(title, url, true);
}

new Vue({
    el: '#flow',
    data: {
        flowNodes: [],
        expandAll: false,
        loadingDialog: false
    },
    watch: {
        expandAll(val) {
            this.expandMenus(val)
        }
    },
    methods: {
        expandMenus: function (status) {
            for (var i = 0; i < this.flowNodes.length; i++) {
                this.flowNodes[i].open = status
                this.flowNodes[i].children.forEach(function (item) {
                    item.open = status
                })
            }
        },
        expandAssignMenu: function () {
            var sysNo = GetQueryString('SystemNo')
            var moduleNo = GetQueryString('ModuleNo')
            if (!sysNo) return
            for (var i = 0; i < this.flowNodes.length; i++) {
                var system = this.flowNodes[i]
                if (system.No === sysNo) {
                    system.open = true
                    if (!moduleNo) {
                        system.children.forEach(function (item) {
                            item.open = true
                        })
                    } else {
                        for (var j = 0; j < system.children.length; j++) {
                            var module = system.children[j]
                            if (module.No === moduleNo) {
                                module.open = true
                            }
                        }
                    }
                }
            }
        },
        bindMenu: function () {
            var _this = this
            layui.use('dropdown', function () {
                var dropdown = layui.dropdown
                var systemNodeItems = [
                    { title: '<i class=icon-plus></i> 新建系统', id: "NewSystem" },
                    { title: '<i class=icon-star></i> 系统属性', id: "SystemAttr" },
                    { title: '<i class=icon-plus></i> 新建模块', id: "NewModule" },
                    { title: '<i class=icon-plus></i> 导出模板', id: "Exp" },
                    { title: '<i class=icon-close></i> 删除系统', id: "DeleteNode" }
                ]

                var systemFunc = function (data, oThis) {

                    var no = $(this.elem)[0].dataset.sysno;
                    var name = "";// $(this.elem)[0].dataset.sysname;

                    switch (data.id) {
                        case 'NewSystem':
                            _this.NewSystem();
                            break;
                        case 'NewModule':
                            _this.NewModule(no);
                            break;
                        case 'Exp':
                            _this.Exp(no);
                            break;
                        case 'DeleteNode':
                            _this.DeleteNode(no, 'BP.CCFast.CCMenu.MySystem');
                            break;
                        case 'SystemAttr':
                            _this.Edit(no, name, 'BP.CCFast.CCMenu.MySystem', no);
                            break;
                    }
                    // _this.topNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.idx)
                }
                var systemOptions = [{
                    elem: '.item-top-dp',
                    trigger: 'contextmenu',
                    data: systemNodeItems,
                    click: systemFunc
                }, {
                    elem: '.t-btn',
                    trigger: 'click',
                    data: systemNodeItems,
                    click: systemFunc
                }]

                dropdown.render(systemOptions[0]);
                dropdown.render(systemOptions[1]);

                var moduleNodeItems = [
                    { title: '<i class=icon-plus></i> 新建菜单', id: "NewMenu", },
                    { title: '<i class=icon-plus></i> 新建模块', id: "NewModule", },
                    { title: '<i class=icon-star></i> 模块属性', id: "ModuleAttr", },
                    { title: '<i class=icon-close></i> 删除模块', id: "DeleteModule", }
                ]
                var moduleFunc = function (data, oThis) {
                    //获得不了当前选择的行的主键了.
                    var moduleNo = $(this.elem)[0].dataset.moduleno;

                    switch (data.id) {
                        case 'NewModule': //新建模块.

                            var en = new Entity("BP.CCFast.CCMenu.Module", moduleNo);
                            var systemNo = en.SystemNo;

                            _this.NewModule(systemNo);
                            break
                        case 'NewMenu':
                            _this.NewMenu(moduleNo);

                            break;
                        case 'ModuleAttr':
                            _this.Edit(moduleNo, "模块", 'BP.CCFast.CCMenu.Module');

                            // _this.ModuleAttr(mmoduleNo); //模块属性.
                            break;
                        case 'DeleteModule':
                            _this.DeleteNode(moduleNo, 'BP.CCFast.CCMenu.Module');
                            break;
                    }
                }
                var moduleOptions = [{
                    elem: '.item-module-dp',
                    trigger: 'contextmenu',
                    data: moduleNodeItems,
                    click: moduleFunc
                }, {
                    elem: '.module-btn',
                    trigger: 'click',
                    data: moduleNodeItems,
                    click: moduleFunc
                }]

                dropdown.render(moduleOptions[0]);
                dropdown.render(moduleOptions[1]);

                var menuFunc = function (data, oThis) {

                    var menuNo = $(this.elem)[0].dataset.menuno;
                    var menuName = $(this.elem)[0].dataset.menuname;

                    var moduleNo = $(this.elem)[0].dataset.moduleno

                    //todo:如何获得菜单的模块编号?
                    //  var moduleNo = $(this.elem)[0].dataset.ModuleNo;

                    switch (data.id) {
                        case 'MenuAttr':
                            var menu = new Entity("BP.CCFast.CCMenu.Menu", menuNo);
                            var modeule = new Entity("BP.CCFast.CCMenu.Module", menu.ModuleNo);

                            _this.Edit(menuNo, menuName, 'BP.CCFast.CCMenu.Menu', modeule.SystemNo);
                            // _this.
                            //alert("目录属性");
                            break;
                        case 'CopyLink':
                            _this.CopyLink(menuNo);
                            break;
                        case 'NewMenu':
                            var en = new Entity("BP.CCFast.CCMenu.Menu", menuNo);
                            _this.NewMenu(en.ModuleNo);
                            //_this.NewMenu(moduleNo);
                            break;
                        case 'DeleteNode':
                            _this.DeleteNode(menuNo, 'BP.CCFast.CCMenu.Menu');
                            break;
                    }
                }
                var menuNodeItems = [
                    { title: '<i class=icon-menu></i> 新建菜单', id: "NewMenu", Icon: "icon-plus" },
                    { title: '<i class=icon-star></i> 菜单属性', id: "MenuAttr", Icon: "icon-options" },
                    { title: '<i class=icon-folder></i> 复制菜单', id: "CopyLink", Icon: "icon-magnifier-add" },
                    { title: '<i class=icon-close></i> 删除菜单', id: "DeleteNode", Icon: "icon-close" }
                ]
                var menuOptions = [{
                    elem: '.item-menu-dp',
                    trigger: 'contextmenu',
                    data: menuNodeItems,
                    click: menuFunc
                }, {
                    elem: '.menu-btn',
                    trigger: 'click',
                    data: menuNodeItems,
                    click: menuFunc
                }]

                dropdown.render(menuOptions[0]);
                dropdown.render(menuOptions[1]);
            })
        },

        //如果w=0 则是100%的宽度.
        openLayer: function (uri, name, w, h) {
            //console.log(uri, name);

            if (w == 0)
                w = window.innerWidth;

            if (w == undefined)
                w = window.innerWidth / 1.5;

            if (h == undefined)
                h = window.innerHeight;

            layer.open({
                type: 2,
                title: name,
                content: [uri, 'no'],
                area: [w + 'px', h + 'px'],
                offset: 'rb',
                shadeClose: true
            })
        },
        ///新建系统.
        NewSystem: function () {
            NewSys();
        },
        NewMenu: function (moduleNo) {

            var url = "../GPM/CreateMenu/Dict.htm?ModuleNo=" + moduleNo;

            var en = new Entity("BP.CCFast.CCMenu.Module", moduleNo);
            var systemNo = en.SystemNo;
            var reoadUrl = "Menus.htm?SystemNo=" + systemNo;
            OpenLayuiDialog(url, "", 90000, false, false, true, false, true, false, false, reoadUrl);
        },
        CopyLink: function (no) {
            var menu = new Entity("BP.CCFast.CCMenu.Menu", no);
            var url = menu.UrlExt;
            if (menu.MenuModel == "Dict")
                url = '../CCBill/SearchDict.htm?FrmID=' + menu.UrlExt;

            if (menu.MenuModel == "Bill")
                url = '../CCBill/SearchBill.htm?FrmID=' + menu.UrlExt;

            alert("您可以把如下链接绑定您的菜单上. \t\n" + url);
        },
        Exp: function (systemNo) {

            if (systemNo == "" || systemNo == undefined) {
                alert("系统编号错误，无法创建模块:" + systemNo);
                return;
            }

            var webUser = new WebUser();
            if (webUser.No != 'admin') {
                alert("只有超级用户才能执行此操作.");
                return;
            }

            if (window.confirm("您确定要导出吗？ 导出到xml需要一定的时间，请耐心等待.") == false)
                return;

            var ens = new Entity("BP.CCFast.CCMenu.MySystem", systemNo);
            var data = ens.DoMethodReturnString("DoExp");
            alert(data);
 
        },
        NewModule: function (systemNo) {

            if (systemNo == "" || systemNo == undefined) {
                alert("系统编号错误，无法创建模块:" + systemNo);
                return;
            }

            layer.prompt({
                value: '',
                title: '请输入模块名称，比如：车辆报表、统计分析、系统管理',
            }, function (value, index, elem) {
                layer.close(index)
                var en = new Entity("BP.CCFast.CCMenu.Module");
                en.Name = value;
                en.SystemNo = systemNo;
                en.IsEnable = 1;
                en.Insert();

                layer.msg("创建成功");

                setTimeout(function () {
                    SetHref("Menus.htm?SystemNo=" + en.No + "&ModuleNo=" + en.No);
                    // window.location.reload();
                }, 1000);
            });
        },
        DeleteNode: function (no, enName) {
            console.log(no, enName)
            layer.confirm('您确定要删除吗？', { icon: 3, title: '提示' }, function (index) {

                var en = new Entity(enName, no);
                en.Delete();

                if (enName == "BP.CCFast.CCMenu.Module") {
                    var url = "Menus.htm?SystemNo=" + en.SystemNo;
                    SetHref(url);
                    return;
                }

                if (enName == "BP.CCFast.CCMenu.Menu") {
                    var model = new Entity("BP.CCFast.CCMenu.Module", en.ModuleNo);
                    var url = "Menus.htm?SystemNo=" + model.SystemNo;
                    SetHref(url);
                    return;
                }

                layer.msg("删除成功")
                // 此处刷新页面更好
                setTimeout(function () {

                    window.location.reload();
                }, 1500)
            }, function (index) {
                layer.msg("已取消删除");
            })
        },
        Edit: function (no, name, enName, systemNo) {

            var url = "../Comm/EnOnly.htm?EnName=" + enName + "&No=" + no;
            if (enName.indexOf('Menu') > 0)
                url = "../Comm/En.htm?EnName=" + enName + "&No=" + no;

            //  var en = new Entity("BP.CCFast.CCMenu.Module", moduleNo);
            //  var systemNo = en.SystemNo;
            if (systemNo == null || systemNo == undefined || systemNo == '')
                systemNo = GetQueryString("SystemNo");

            var reoadUrl = "Menus.htm?SystemNo=" + systemNo;

            OpenLayuiDialog(url, "", 90000, false, false, true, false, true, false, false, reoadUrl);

            //  this.openLayer(url, );
            //   OpenLayuiDialog(url, "", 0, 0, null, false, false, true, false, , reoadUrl);
            //   OpenLayuiDialog(url,);

        },


        initSortArea: function () {
            var _this = this
            this.$nextTick(function () {
                var mainContainer = this.$refs['container']
                new Sortable(mainContainer, {
                    animation: 150,
                    dataIdAttr: 'data-id',
                    ghostClass: 'blue-background-class',
                    onStart: function ( /**Event*/ evt) {
                        _this.loadingDialog = layer.msg('正在移动...', {
                            timeout: 900 * 1000
                        })
                    },
                    onEnd: function (evt) {
                        layer.close(_this.loadingDialog)

                        var currentNodeArrStr = Array.from(evt.to.querySelectorAll('div[data-sysid]')).map(function (item) {
                            return item.dataset.sysid
                        }).join(',');

                        console.log(currentNodeArrStr);

                        _this.updateSystemSort(currentNodeArrStr);
                    }
                });
                var sortContainer = this.$refs['sort-main']
                sortContainer.forEach(function (item) {
                    new Sortable(item, {
                        group: {
                            name: 'modules'
                        },
                        animation: 150,
                        dataIdAttr: 'data-moduleid',
                        ghostClass: 'blue-background-class',
                        onStart: function ( /**Event*/ evt) {
                            _this.loadingDialog = layer.msg('正在移动...', {
                                timeout: 900 * 1000
                            })
                        },
                        onEnd: function (evt) {

                            layer.close(_this.loadingDialog)
                            var currentNodeArrStr = Array.from(evt.to.querySelectorAll('div[data-moduleid]')).map(function (item) {
                                return item.dataset.moduleid
                            }).join(',')
                            var currentNodeId = evt.to.dataset.pid
                            _this.updateModuleSort(currentNodeArrStr, currentNodeId)
                            // var oldSysIndex = evt.item.dataset.sysidx
                            // var newSysIndex = evt.to.dataset.sysidx
                            // var item = _this.flowNodes[oldSysIndex].children.splice(evt.oldDraggableIndex, 1)[0]
                            // _this.flowNodes[newSysIndex].children.splice(evt.newDraggableIndex, 0, item)
                        }
                    });
                })

                var childSortableContainers = this.$refs['child-row']
                for (var i = 0; i < childSortableContainers.length; i++) {
                    var csc = childSortableContainers[i]
                    new Sortable(csc, {
                        group: {
                            name: 'menus'
                        },
                        animation: 150,
                        dataIdAttr: 'data-id',
                        ghostClass: 'blue-background-class',
                        onStart: function ( /**Event*/ evt) {
                            _this.loadingDialog = layer.msg('正在移动...', {
                                timeout: 900 * 1000
                            })
                        },
                        onEnd: function (evt) {
                            layer.close(_this.loadingDialog)
                            var pastNodeArrStr = Array.from(evt.from.querySelectorAll('div.row[data-id]')).map(function (item) {
                                return item.dataset.id
                            }).join(',')
                            var pastNodeId = evt.from.dataset.pid
                            var currentNodeArrStr = Array.from(evt.to.querySelectorAll('div.row[data-id]')).map(function (item) {
                                return item.dataset.id
                            }).join(',')
                            var currentNodeId = evt.to.dataset.pid;

                            _this.updateMenuSort(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId)

                            // var oldSysIndex = evt.item.dataset.sysidx;
                            // var oldModuleIndex = evt.item.dataset.moduleidx;
                            // var newSysIndex = evt.to.dataset.sysidx;
                            // var newModuleIndex = evt.to.dataset.moduleidx;
                            // if (oldSysIndex === newSysIndex && oldModuleIndex === newModuleIndex) return
                            // var item = _this.flowNodes[oldSysIndex].children[oldModuleIndex].children.splice(evt.oldDraggableIndex, 1)[0]
                            // _this.flowNodes[newSysIndex].children[newModuleIndex].children.splice(evt.newDraggableIndex, 0, item)
                        }
                    })
                }

            })
        },
        updateMenuSort: function (pastNodeArrStr, pastNodeId, currentMenus, currentModuleNo) {
            // 菜单排序..
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_GPM");
            handler.AddPara("RootNo", currentModuleNo); //移动到的目录ID.
            handler.AddPara("EnNos", currentMenus); //目录下的 菜单IDs
            var data = handler.DoMethodReturnString("Menu_Move");
            layer.msg(data)
            // alert(data);
            // 无需刷新页面，此时页面节点已经更新.
        },
        updateModuleSort: function (moduleNos, systemNo) {
            // todo 实现模块在系统之间的排序（非跨系统）
            // 同无需刷新页面

            // 目录排序..
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_GPM");
            handler.AddPara("RootNo", systemNo); // 移动到的目录ID.
            handler.AddPara("EnNos", moduleNos); //目录下的 菜单IDs
            var data = handler.DoMethodReturnString("Module_Move");
            layer.msg(data)
        },
        updateSystemSort: function (systemNos) {
            // todo 系统排序
            // 同无需刷新页面

            // 目录排序..

            // 目录排序..
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_GPM");
            handler.AddPara("EnNos", systemNos); //目录下的 菜单IDs
            var data = handler.DoMethodReturnString("System_Move");
        },
        // 是否启用
        changeSystemEnableStatus(system, ctrl) {
            // 当前启用状态

            var en = new Entity("BP.CCFast.CCMenu.MySystem", system.No);
            if (en.IsEnable == 0)
                en.IsEnable = 1; // method.IsEnable;
            else
                en.IsEnable = 0; // method.IsEnable;

            en.Update();

            console.log("更新成功..");

        },
        // 是否启用
        changeMethodEnableStatus(method, ctrl) {
            // 当前启用状态

            var en = new Entity("BP.CCFast.CCMenu.Module", method.No);

            if (en.IsEnable == 0)
                en.IsEnable = 1; // method.IsEnable;
            else
                en.IsEnable = 0; // method.IsEnable;

            // alert(en.IsEnable );

            en.Update();

            console.log("更新成功..");

        },
        // 是否启用
        changeMenuEnableStatus(menu, ctrl) {
            // 当前启用状态

            var en = new Entity("BP.CCFast.CCMenu.Menu", menu.No);
            if (en.IsEnable == 0)
                en.IsEnable = 1; // method.IsEnable;
            else
                en.IsEnable = 0; // method.IsEnable;

            en.Update();

            console.log("更新成功..");

        }

    },
    mounted: function () {
        // fix firefox bug
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        var systems = new Entities("BP.CCFast.CCMenu.MySystems");
        systems.RetrieveAll();
        //把集合转化为数组。
        systems = obj2arr(systems);

        console.log(systems);

    }
})

//实体属性.
function EnDotHtml(enName, pkVal, title, width) {

    var url = "../Comm/En.htm?EnName=" + enName + "&PKVal=" + pkVal;

    if (title == undefined)
        title = "";
    if (width == undefined)
        width = 500;

    OpenLayuiDialog(url, title, width, 0, null, false);

}

function NewSys() {

    var url = "NewSystem.htm";
    // OpenLayuiDialog(url,"新增系统")；
    // this.openLayer(url, '新增系统');
    OpenLayuiDialog(url, "", 0, 0, null, true);
}


function ImpSys() {

    var url = "ImpSystem.htm";
    // OpenLayuiDialog(url,"新增系统")；
    // this.openLayer(url, '新增系统');
    OpenLayuiDialog(url, "", 0, 0, null, true);
}


function ManageSys() {

    var url = "SystemList.htm";
    // OpenLayuiDialog(url,"新增系统")；
    // this.openLayer(url, '新增系统');
    OpenLayuiDialog(url, "", 0, 0, null, true);

}

function DictCopy(frmID, menuID) {
    var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin_Method");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("MenuID", menuID);
    handler.DoMethodReturnJSON("");

}


function DesignerFlow(no, name) {
    var sid = GetQueryString("Token");
    var webUser = new WebUser();
    var url = "../Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + no + "&UserNo=" + webUser.No + "&Token=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
    window.top.vm.openTab(name, url, true);
}

function obj2arr(obj) {
    delete obj.Paras
    delete obj.ensName
    delete obj.length
    var arr = []
    for (var key in obj) {
        if (Object.hasOwnProperty.call(obj, key)) {
            arr.push(obj[key]);
        }
    }
    return arr
}
