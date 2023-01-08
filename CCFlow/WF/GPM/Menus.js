function GenerDoc(menu) {
    return menu;
}

//属性.
function AttrFrm(enName, title, pkVal) {
    var url = basePath + "/WF/Comm/En.htm?EnName=" + enName + "&No=" + pkVal;
    title = "";
    OpenLayuiDialog(url, title, 5000, 0, null, false);
    return;
}

//打开设计表单.
function GoToFrmDesigner(frmID) {
    var url = basePath + "/WF/Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + frmID + '&Token=' + GetQueryString("Token");
    addTab(url, "设计:" + frmID);
    return;
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
                url = basePath + '/WF/CCBill/SearchDict.htm?FrmID=' + menu.UrlExt;

            if (menu.MenuModel == "Bill")
                url = basePath + '/WF/CCBill/SearchBill.htm?FrmID=' + menu.UrlExt;

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

            //  var url = menu.UrlExt;

            //layer.prompt({
            //    value: '',
            //    title: '请输入模块名称，比如：车辆报表、统计分析、系统管理',
            //}, function (value, index, elem) {
            //    layer.close(index);

            //    var en = new Entity("BP.CCFast.CCMenu.Module");
            //    en.Name = value;
            //    en.SystemNo = systemNo;
            //    en.IsEnable = 1;
            //    en.Insert();

            //    layer.msg("创建成功");

            //    setTimeout(function () {
            //        SetHref( "Menus.htm?SystemNo=" + en.No + "&ModuleNo=" + en.No;
            //        // window.location.reload();
            //    }, 1000);
            //});
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

            var url = basePath + "/WF/Comm/EnOnly.htm?EnName=" + enName + "&No=" + no;
            if (enName.indexOf('Menu') > 0)
                url = basePath + "/WF/Comm/En.htm?EnName=" + enName + "&No=" + no;

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
        systems = obj2arr(systems)

        //模块.
        var modules = new Entities("BP.CCFast.CCMenu.Modules");
        modules.RetrieveAll();

        for (var i = 0; i < modules.length; i++) {
            var en = modules[i];
            if (en.Icon === "")
                modules[i].Icon = "icon-folder";
        }
        modules = obj2arr(modules);


        //菜单.
        var menus = new Entities("BP.CCFast.CCMenu.Menus");
        menus.RetrieveAll();
        menus = obj2arr(menus);

        var btnStyle = "class='layui-btn layui-btn-primary layui-border-blue layui-btn-xs'";
        menus.forEach(function (menu) {

            var docs = "";

            if (menu.MenuModel == "SingleDictGenerWorkFlows") {
                menu.MenuModel = "单实体流程列表";
                if (menu.Icon === "") menu.Icon = "icon-notebook";
                menu.Docs = "一个实体所有发起的流程列表.";
            }

            if (menu.Mark === "Calendar") {
                menu.Docs = " <a " + btnStyle + " href=\"javascript:OpenLayuiDialog('" + basePath + menu.UrlExt + "','" + menu.Name + "',9000); \"  >打开</a>";
            }

            if (menu.Mark === "Task") {
                menu.Docs = " <a " + btnStyle + " href=\"javascript:OpenLayuiDialog('" + basePath + menu.UrlExt + "','" + menu.Name + "',9000); \"  >打开</a>";
            }

            if (menu.Mark === "Notepad") {
                menu.Docs = " <a " + btnStyle + " href=\"javascript:OpenLayuiDialog('" + basePath + menu.UrlExt + "','" + menu.Name + "',9000); \"  >打开</a>";
            }

            if (menu.Mark === "WorkRec") {
                menu.Docs = " <a " + btnStyle + " href=\"javascript:OpenLayuiDialog('" + basePath + menu.UrlExt + "','" + menu.Name + "',9000); \"  >打开</a>";
            }

            if (menu.Mark === "Info") {
                menu.Docs = " <a " + btnStyle + " href=\"javascript:OpenLayuiDialog('" + basePath + menu.UrlExt + "','" + menu.Name + "',9000); \"  >打开</a>";
                menu.Docs += "-<a " + btnStyle + " href=\"javascript:OpenLayuiDialog('" + basePath + menu.UrlExt + "','" + menu.Name + "',9000); \"  >编辑信息</a>";
                menu.Docs += "-<a " + btnStyle + " href=\"javascript:OpenLayuiDialog('" + basePath + menu.UrlExt + "','" + menu.Name + "',9000); \"  >编辑类别</a>";
            }

            if (menu.Mark === "KnowledgeManagement") {
                menu.Docs = "<a " + btnStyle + " href=\"javascript:OpenLayuiDialog('" + basePath + menu.UrlExt + "','" + menu.Name + "',9000); \"  >编辑</a>";
            }

            //独立流程
            if (menu.MenuModel == "FlowNewEntity" || menu.MenuModel == "FlowEtc" || menu.MenuModel == "FlowBaseData") {

                if (menu.MenuModel == "FlowNewEntity") menu.MenuModel = "新建实体类";
                if (menu.MenuModel == "FlowBaseData") menu.MenuModel = "基础资料修改类";
                if (menu.MenuModel == "FlowEtc") menu.MenuModel = "实体其他业务类";

                if (menu.Mark === "StartFlow") menu.Icon = "icon-paper-plane";
                if (menu.Mark === "Todolist") menu.Icon = "icon-bell";
                if (menu.Mark === "Runing") menu.Icon = "icon-clock";
                if (menu.Mark === "Group") menu.Icon = "icon-chart";
                if (menu.Mark === "Search") menu.Icon = "icon-grid";

                var doc = "<a " + btnStyle + "  href=\"javascript:DesignerFlow('" + menu.Tag1 + "','" + menu.Name + "');\" >设计流程</a>";

                //var html = "<a " + btnStyle + "  href=\"javascript:LayuiPopRight('" + menu.UrlExt + "','" + menu.Name + "','0',false);\" >启动流程</a>";
                var html = "";
                if (menu.Mark === "StartFlow") {
                    html += "<a " + btnStyle + "  href=\"javascript:EnDotHtml('BP.CCBill.Template.MethodFlowNewEntity','" + menu.No + "','','800');\" >属性</a>";
                }

                menu.Docs = html + doc;
            }


            //修改基础数据.
            if (menu.MenuModel === "FlowBaseData") {

                menu.MenuModel = "流程";

                var btn = "<a " + btnStyle + " href=\"javascript:DesignerFlow('" + menu.Tag1 + "','" + menu.Name + "');\" >流程设计</a>";

                if (menu.Mark === "Start") {

                    menu.MenuModel = "发起流程";
                    if (menu.Icon === "") menu.Icon = "icon-paper-plane";
                    menu.Docs = "发起流程" + menu.Tag1 + ",把实体数据传入表单." + btn;
                }

                if (menu.Mark === "Todolist") {
                    menu.MenuModel = "流程:待办";
                    if (menu.Icon === "") menu.Icon = "icon-bell";

                    menu.Docs = "流程:" + menu.Tag1 + ",待办." + btn;
                }
                if (menu.Mark === "Runing") {
                    menu.MenuModel = "流程:未完成的";
                    if (menu.Icon === "") menu.Icon = "icon-clock";


                    menu.Docs = "流程:" + menu.Tag1 + ",未完成的." + btn;
                }
                if (menu.Mark === "Search") {
                    menu.MenuModel = "实体资料变更";
                    if (menu.Icon === "") menu.Icon = "icon-magnifier";
                    menu.Docs = "流程:" + menu.Tag1 + ",查询." + btn;
                }

                if (menu.Mark === "FlowGroup") {
                    menu.MenuModel = "流程:分析";
                    if (menu.Icon === "") menu.Icon = "icon-chart";
                    menu.Docs = "流程:" + menu.Tag1 + ",分析." + btn;
                }
            }

            if (menu.MenuModel === "Dict" || menu.MenuModel === "DBList") {

                var html = "";
                if (menu.MenuModel === "DBList") {
                    html += "<a " + btnStyle + " href=\"javascript:addTab('" + basePath + "/WF/CCBill/SearchDBList.htm?FrmID=" + menu.UrlExt + "','" + menu.Name + "');\"  >打开</a>";
                }

                if (menu.MenuModel === "Dict") {
                    if (menu.Icon === "") menu.Icon = "icon-notebook";
                    html += "<a " + btnStyle + " href=\"javascript:addTab('" + basePath + "/WF/CCBill/SearchDict.htm?FrmID=" + menu.UrlExt + "','" + menu.Name + "');\"  >打开</a>";
                }

                // var url = "../CCBill/Admin/SearchCond.htm?FrmID=" + menu.UrlExt;
                //OpenLayuiDialog(url, title, 5000, 0, null, false);
                //  html += "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('" + url + "','','90000',0,null,false)\" >条件</a>";
                //html += "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('../CCBill/Admin/Collection/Default.htm?FrmID=" + menu.UrlExt + "&ModuleNo=" + menu.ModuleNo + "','',700,0,null,true);\" >列表组件</a>";
                //   html += "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('../CCBill/Admin/Collection/Default.htm?FrmID=" + menu.UrlExt + "&ModuleNo=" + menu.ModuleNo + "','列表:" + menu.Name + "');\" >列表组件</a>";

                html += "<a " + btnStyle + "  href=\"javascript:addTab('" + basePath + "/WF/CCBill/Admin/Collection.htm?FrmID=" + menu.UrlExt + "&ModuleNo=" + menu.ModuleNo + "&MenuModel=" + menu.MenuModel + "','方法:" + menu.Name + "');\" >列表组件</a>";
                html += "<a " + btnStyle + "  href=\"javascript:addTab('" + basePath + "/WF/CCBill/Admin/Method.htm?FrmID=" + menu.UrlExt + "&ModuleNo=" + menu.ModuleNo + "&MenuModel=" + menu.MenuModel + "','方法:" + menu.Name + "');\" >实体组件</a>";
                html += "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCBill.FrmDict','" + menu.Name + "','" + menu.UrlExt + "')\" >属性</a>";

                if (menu.MenuModel === "DBList") {
                    menu.MenuModel = "数据源实体";
                    html += "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCBill.DBList','" + menu.Name + "','" + menu.UrlExt + "')\" >数据源</a>";
                }

                if (menu.MenuModel === "Dict") {
                    menu.MenuModel = "实体";
                    html += "<a " + btnStyle + "  href=\"javascript:GoToFrmDesigner('" + menu.UrlExt + "')\" >表单设计</a>";
                    html += "&nbsp; <a href='https://www.bilibili.com/video/BV1sy4y157ac/' target='_blank' class='icon-camrecorder'></a>";
                }
                menu.Docs = html;
            }

            if (menu.MenuModel == "FlowUrl") {

                if (menu.Icon === "") menu.Icon = "icon-heart";

                var html = "<a " + btnStyle + "  href=\"javascript:addTab('" + basePath + "/WF/" + menu.UrlExt + "','" + menu.Name + "');\"  >" + menu.Name + "</a>";
                menu.Docs = html;
            }

            if (menu.MenuModel == "Rpt3D" || menu.MenuModel == "Rpt3D") {

                if (menu.Icon === "") menu.Icon = "icon-heart";

                var url = basePath + "/CCFast/Rpt/Rpt3D.htm?RptNo=" + menu.No;
                var html = "<a " + btnStyle + "  href=\"javascript:addTab('" + url + "','" + menu.Name + "');\"  >运行</a>";

                url = basePath + '/WF/Comm/En.htm?EnName=BP.CCFast.Rpt.Rpt3D&No=' + menu.No;
                // url = basePath + '/WF/Comm/En.htm?EnName=BP.CCFast.Rpt.Rpt3D&No=' + menu.No;
                html += "<a " + btnStyle + "  href=\"javascript:addTab('" + url + "','" + menu.Name + "');\"  >属性</a>";

                menu.Docs = html;
            }

            //发起指定的流程的时候.
            if (menu.MenuModel == "StartSpecFlow") {
                if (menu.Icon === "") menu.Icon = "icon-heart";

                var html = "<a " + btnStyle + "  href='" + basePath + "/WF/MyFlow.htm?FK_Flow=" + menu.UrlExt + "' target=_blank >发起:" + menu.Name + "</a>";
                html += "|<a " + btnStyle + "  href=''>测试容器</a>";

                html += "<a " + btnStyle + " href=\"javascript:DesignerFlow('" + menu.Tag1 + "','" + menu.Name + "');\" >设计流程</a>";
                menu.Docs = html;
            }

            if (menu.MenuModel == "" || menu.MenuModel === "SelfUrl") {
                menu.MenuModel = "自定义菜单";

                if (menu.Icon === "") menu.Icon = "icon-user";

                var html = "<a " + btnStyle + "  href=\"javascript:addTab('" + menu.UrlExt + "','" + menu.Name + "');\"  >打开</a> : " + menu.UrlExt;
                menu.Docs = html;
            }

            if (menu.MenuModel == "DictTable") {

                menu.MenuModel = "字典表";
                if (menu.Icon === "") menu.Icon = "icon-control-pause";

                var url = basePath + "/WF/Admin/FoolFormDesigner/SFTableEditData.htm?FK_SFTable=" + menu.UrlExt + "&QueryType=Dict";
                var html = "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('" + url + "','" + menu.Name + "','700',0,null,false);\" >打开字典:" + menu.UrlExt + "</a>";
                menu.Docs = html;
            }

            if (menu.MenuModel == "Func") {
                menu.MenuModel = "独立功能";
                //if (menu.Icon === "") menu.Icon = "icon-energy";
                //var html = "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCBill.Sys.Func','" + menu.Name + "','" + menu.UrlExt + "')\" >功能属性</a>";
                //var url = "../CCBill/Func/Func.htm?MyPK=" + menu.UrlExt + "&From=Desinger";
                //html += "<a " + btnStyle + "  href=\"javascript:LayuiPopRight('" + url + "','" + menu.Name + "','0',false);\" >功能执行</a>";
                //  menu.Docs = html;

                var url = basePath + "/WF/CCBill/Sys/Func.htm?MyPK=" + menu.UrlExt + "&From=Desinger";
                doc = "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('" + url + "','" + menu.Name + "','700',0,null,false);\" >打开</a>";
                //    doc += "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCBill.Sys.Func','" + menu.Name + "','" + menu.UrlExt + "')\" >功能属性</a>";

                var url = basePath + "/WF/CCBill/Admin/MethodDocSys/Default.htm?No=" + menu.UrlExt;
                doc += "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('" + url + "','',9000,0,null,false)\" >编写脚本</a>";
                menu.Docs = doc;
            }

            if (menu.MenuModel == "Windows") {

                menu.MenuModel = "统计分析";
                if (menu.Icon === "") menu.Icon = "icon-energy";

                if (menu.Tag1 == "" || menu.Tag1.length == 0)
                    menu.Tag1 = "RptWhite";

                var url = basePath + "/WF/Portal/" + menu.Tag1 + ".htm?PageID=" + menu.No;
                var html = "<a " + btnStyle + " href=\"javascript:addTab('" + url + "','" + menu.Name + "');\"  >打开</a>";

                // url = "../GPM/Window/Default.htm?PageID=" + menu.No;
                //  WF / Portal / Home.htm ? PageID = 48339c5c - a264 - 43d4 - 841c - 79b46fbfda3d & viewid=Edit
                url = basePath + "/WF/Portal/" + menu.Tag1 + ".htm?viewid=Edit&PageID=" + menu.No;

                html += "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('" + url + "','" + menu.Name + "','1500',0,null,false);\" >编辑窗体</a>";
                menu.Docs = html;
            }

            //独立流程
            if (menu.MenuModel == "StandAloneFlow") {

                menu.MenuModel = "独立流程";

                if (menu.Mark === "StartFlow") menu.Icon = "icon-paper-plane";
                if (menu.Mark === "Todolist") menu.Icon = "icon-bell";
                if (menu.Mark === "Runing") menu.Icon = "icon-clock";
                if (menu.Mark === "Group") menu.Icon = "icon-chart";
                if (menu.Mark === "Search") menu.Icon = "icon-grid";  //magnifier

                //var html = "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCBill.Sys.Func','" + menu.Name + "','" + menu.UrlExt + "')\" >功能属性</a>";
                var html = "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('" + menu.UrlExt + "','" + menu.Name + "','700',0,null,false);\" >执行</a>";

                var url = basePath + "/CCFast/StandAloneFlow/Admin/Default.htm?FlowNo=" + menu.Tag1;
                html += "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('" + url + "','" + menu.Name + "','900',0,null,false);\" >设置</a>";
                // html += "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCFast.StandAloneFlow','" + menu.Name + "','" + menu.Tag1 + "')\" >设置</a>";

                var doc = "  <a href=\"javascript:DesignerFlow('" + menu.Tag1 + "','" + menu.Name + "');\" ><i class=icon-heart ></i>设计流程" + menu.Tag1 + "</a>";

                menu.Docs = html + doc;
            }


            if (menu.MenuModel === "Bill") {
                menu.MenuModel = "单据";
                var html = "";
                html += "<a " + btnStyle + "  href=\"javascript:addTab('" + basePath + "/WF/CCBill/SearchBill.htm?FrmID=" + menu.UrlExt + "','" + menu.Name + "');\"  >打开</a>";
                html += "<a " + btnStyle + "  href=\"javascript:addTab('" + basePath + "/WF/CCBill/Admin/Collection.htm?FrmID=" + menu.UrlExt + "&ModuleNo=" + menu.ModuleNo + "&MenuModel=" + menu.MenuModel + "','方法:" + menu.Name + "');\" >列表组件</a>";
                html += "<a " + btnStyle + "  href=\"javascript:addTab('" + basePath + "/WF/CCBill/Admin/Method.htm?FrmID=" + menu.UrlExt + "&ModuleNo=" + menu.ModuleNo + "&MenuModel=" + menu.MenuModel + "','方法:" + menu.Name + "');\" >单据组件</a>";
                html += "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCBill.FrmBill','" + menu.Name + "','" + menu.UrlExt + "')\" >单据属性</a>";
                html += "<a " + btnStyle + "  href=\"javascript:GoToFrmDesigner('" + menu.UrlExt + "')\" >表单设计</a>";
                menu.Docs = html;
            }

            if (menu.MenuModel === "Tabs") {

                menu.MenuModel = "标签容器";

                var url = basePath + "/WF/Portal/Tabs.htm?PageID=" + menu.No;
                var html = "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('" + url + "','','900',0,null,false);\" >打开</a>";

                url = basePath + "/WF/GPM/Tabs/Default.htm?RefMenuNo=" + menu.No + "&SystemNo=" + menu.SystemNo + "&MoudleNo=" + menu.ModuleNo;
                html += "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('" + url + "','" + menu.Name + "','900',0,null,false);\" >设计容器</a>";

                menu.Docs = html;
            }

            var url = basePath + "/WF/GPM/PowerCenter.htm?CtrlObj=Menu&CtrlPKVal=" + menu.No + "&CtrlGroup=Menu";

            menu.MenuCtrlWayText = "<a " + btnStyle + "  href =\"javascript:OpenLayuiDialog('" + url + "','" + menu.Name + "','700',0,null,false);\" >权限</a>";

            // item.Docs = GenerDoc(item);
            //if (menu.MenuModel == "Dict") menu.MenuModel = "实体";
            //if (menu.MenuModel == "DictTable") menu.MenuModel = "字典表";
            //if (menu.MenuModel == "Bill") menu.MenuModel = "单据";
            //if (menu.MenuModel == "SelfUrl") menu.MenuModel = "自定义";
            //if (menu.MenuModel == "Search") menu.MenuModel = "查询";
            //if (menu.MenuModel == "Start") menu.MenuModel = "发起";
            //if (menu.MenuModel == "Todolist") menu.MenuModel = "待办";
            //  item.Doc = "ssssssss";SSS

        })


        for (var i = 0; i < systems.length; i++) {

            var sys = systems[i];
            sys.open = false
            sys.children = [];
            var sysurl = basePath + "/WF/GPM/PowerCenter.htm?CtrlObj=MenuSystem&CtrlPKVal=" + sys.No + "&CtrlGroup=Menu";
            sys.itemCtrlWayText = "<a " + btnStyle + "  href =\"javascript:OpenLayuiDialog('" + sysurl + "','" + sys.Name + "','700',0,null,false);\" >权限</a>";
            var childModules = modules.filter(function (module) {
                // return module.SystemNo === ''
                return module.SystemNo === sys.No
            })
            for (var j = 0; j < childModules.length; j++) {
                var module = childModules[j]
                module.open = false
                var moduleurl = basePath + "/WF/GPM/PowerCenter.htm?CtrlObj=MenuModule&CtrlPKVal=" + module.No + "&CtrlGroup=Menu";

                module.moduleCtrlWayText = "<a " + btnStyle + "  href =\"javascript:OpenLayuiDialog('" + moduleurl + "','" + module.Name + "','700',0,null,false);\" >权限</a>";
                module.children = menus.filter(function (menu) {
                    return menu.ModuleNo == module.No
                })

            }
            sys.children = childModules;
        }
        console.log(systems)
        this.flowNodes = systems;
        this.bindMenu();
        this.initSortArea();
        this.expandAssignMenu()
    }
})

//实体属性.
function EnDotHtml(enName, pkVal, title, width) {

    var url = basePath + "/WF/Comm/En.htm?EnName=" + enName + "&PKVal=" + pkVal;

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
    var url = basePath + "/WF/Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + no + "&UserNo=" + webUser.No + "&Token=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
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
