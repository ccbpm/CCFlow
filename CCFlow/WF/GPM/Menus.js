function GenerDoc(menu) {
    return menu;
}

//属性.
function AttrFrm(enName, title, pkVal) {
    var url = "../Comm/En.htm?EnName=" + enName + "&No=" + pkVal;
    title = "";
    OpenLayuiDialog(url, title, 5000, 0, null, false);
    return;
}

//打开设计表单.
function GoToFrmDesigner(frmID) {
    var url = "../Admin/CCFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + frmID;
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
            if(!sysNo) return
            for (var i = 0; i < this.flowNodes.length; i++) {
                var system = this.flowNodes[i]
                if (system.No === sysNo) {
                    system.open = true
                    if(!moduleNo){
                        system.children.forEach(function (item) {
                            item.open = true
                        })
                    }else{
                        for (var j = 0; j < system.children.length; j++) {
                            var module = system.children[j]
                            if(module.No === moduleNo){
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
                    {title: '<i class=icon-plus></i> 新建系统', id: "NewSystem"},
                    {title: '<i class=icon-star></i> 系统属性', id: "SystemAttr"},
                    {title: '<i class=icon-plus></i> 新建模块', id: "NewModule"},
                    {title: '<i class=icon-close></i> 删除系统', id: "DeleteNode"}
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
                        case 'DeleteNode':
                            _this.DeleteNode(no, 'BP.GPM.Menu2020.MySystem');
                            break;
                        case 'SystemAttr':
                            _this.Edit(no, name, 'BP.GPM.Menu2020.MySystem');
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
                    {title: '<i class=icon-plus></i> 新建菜单', id: "NewMenu",},
                    {title: '<i class=icon-plus></i> 新建模块', id: "NewModule",},
                    {title: '<i class=icon-star></i> 模块属性', id: "ModuleAttr",},
                    {title: '<i class=icon-close></i> 删除模块', id: "DeleteModule",}
                ]
                var moduleFunc = function (data, oThis) {
                    //获得不了当前选择的行的主键了.
                    var moduleNo = $(this.elem)[0].dataset.moduleno;

                    switch (data.id) {
                        case 'NewModule': //新建模块.

                            var en = new Entity("BP.GPM.Menu2020.Module", moduleNo);
                            var systemNo = en.SystemNo;

                            _this.NewModule(systemNo);
                            break
                        case 'NewMenu':
                            _this.NewMenu(moduleNo);
                            break;
                        case 'ModuleAttr':
                            _this.Edit(moduleNo, "模块", 'BP.GPM.Menu2020.Module');

                            // _this.ModuleAttr(mmoduleNo); //模块属性.
                            break;
                        case 'DeleteModule':
                            _this.DeleteNode(moduleNo, 'BP.GPM.Menu2020.Module');
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
                            _this.Edit(menuNo, menuName, 'BP.GPM.Menu2020.Menu');
                            // _this.
                            //alert("目录属性");
                            break;
                        case 'CopyLink':
                            _this.CopyLink(menuNo);
                            break;
                        case 'NewMenu':
                            var en = new Entity("BP.GPM.Menu2020.Menu", menuNo);
                            _this.NewMenu(en.ModuleNo);
                            //_this.NewMenu(moduleNo);
                            break;
                        case 'DeleteNode':
                            _this.DeleteNode(menuNo, 'BP.GPM.Menu2020.Menu');
                            break;
                    }
                }
                var menuNodeItems = [
                    {title: '<i class=icon-menu></i> 新建菜单', id: "NewMenu", Icon: "icon-plus"},
                    {title: '<i class=icon-star></i> 菜单属性', id: "MenuAttr", Icon: "icon-options"},
                    {title: '<i class=icon-folder></i> 复制菜单', id: "CopyLink", Icon: "icon-magnifier-add"},
                    {title: '<i class=icon-close></i> 删除菜单', id: "DeleteNode", Icon: "icon-close"}
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
            //@yln 怎么不能刷洗？
            //    OpenLayuiDialog(url, "新建菜单" 0, 0, null, true);
            OpenLayuiDialog(url, "", 90000, false, false, true);


            //    this.openLayer(url, '新增目录');
        },
        CopyLink: function (no) {
            var menu = new Entity("BP.GPM.Menu2020.Menu", no);
            var url = menu.UrlExt;
            if (menu.MenuModel == "Dict")
                url = '../CCBill/SearchDict.htm?FrmID=' + menu.UrlExt;

            if (menu.MenuModel == "Bill")
                url = '../CCBill/SearchBill.htm?FrmID=' + menu.UrlExt;

            alert("您可以把如下链接绑定您的菜单上. \t\n" + url);
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
                var en = new Entity("BP.GPM.Menu2020.Module");
                en.Name = value;
                en.SystemNo = systemNo;
                en.Insert();

                layer.msg("创建成功");

                setTimeout(function () {
                    window.location.href = "Menus.htm?SystemNo=" + en.No + "&ModuleNo=" + en.No;
                    // window.location.reload();
                }, 1000);
            });
        },
        DeleteNode: function (no, enName) {
            console.log(no, enName)
            layer.confirm('您确定要删除吗？', {icon: 3, title: '提示'}, function (index) {
                var en = new Entity(enName, no);
                en.Delete();

                layer.msg("删除成功")
                // 此处刷新页面更好
                setTimeout(function () {
                    window.location.reload();
                }, 1500)
            }, function (index) {
                layer.msg("已取消删除");
            })
        },
        Edit: function (no, name, enName) {

            var url = "../Comm/EnOnly.htm?EnName=" + enName + "&No=" + no;

            if (enName.indexOf('Menu') > 0)
                url = "../Comm/En.htm?EnName=" + enName + "&No=" + no;

            //  this.openLayer(url, );
            OpenLayuiDialog(url, "", 0, 0, null, false);

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

                       // console.log(currentNodeArrStr);

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
                            var oldSysIndex = evt.item.dataset.sysidx
                            var newSysIndex = evt.to.dataset.sysidx
                            var item = _this.flowNodes[oldSysIndex].children.splice(evt.oldDraggableIndex, 1)[0]
                            _this.flowNodes[newSysIndex].children.splice(evt.newDraggableIndex, 0, item)
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
                            var pastNodeArrStr = Array.from(evt.from.querySelectorAll('div[data-id]')).map(function (item) {
                                return item.dataset.id
                            }).join(',')
                            var pastNodeId = evt.from.dataset.pid
                            var currentNodeArrStr = Array.from(evt.to.querySelectorAll('div[data-id]')).map(function (item) {
                                return item.dataset.id
                            }).join(',')
                            var currentNodeId = evt.to.dataset.pid;
                            _this.updateMenuSort(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId)
                            var oldSysIndex = evt.item.dataset.sysidx;
                            var oldModuleIndex = evt.item.dataset.moduleidx;
                            var newSysIndex = evt.to.dataset.sysidx;
                            var newModuleIndex = evt.to.dataset.moduleidx;
                            if(oldSysIndex === newSysIndex && oldModuleIndex === newModuleIndex) return
                            var item = _this.flowNodes[oldSysIndex].children[oldModuleIndex].children.splice(evt.oldDraggableIndex, 1)[0]
                            _this.flowNodes[newSysIndex].children[newModuleIndex].children.splice(evt.newDraggableIndex, 0, item)
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

        }
    },
    mounted: function () {
        // fix firefox bug
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        var systems = new Entities("BP.GPM.Menu2020.MySystems");
        systems.RetrieveAll();
        systems = obj2arr(systems)

        //模块.
        var modules = new Entities("BP.GPM.Menu2020.Modules");
        modules.RetrieveAll();

        for (var i = 0; i < modules.length; i++) {
            var en = modules[i];
            if (en.Icon === "")
                modules[i].Icon = "icon-folder";
        }
        modules = obj2arr(modules);


        //菜单.
        var menus = new Entities("BP.GPM.Menu2020.Menus");
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
                if (menu.Mark === "FlowSearch") {
                    menu.MenuModel = "流程:查询";
                    if (menu.Icon === "") menu.Icon = "icon-magnifier";

                    menu.Docs = "流程:" + menu.Tag1 + ",查询." + btn;
                }
                if (menu.Mark === "FlowGroup") {
                    menu.MenuModel = "流程:分析";
                    if (menu.Icon === "") menu.Icon = "icon-chart";
                    menu.Docs = "流程:" + menu.Tag1 + ",分析." + btn;
                }
            }

            if (menu.MenuModel === "Dict") {

                menu.MenuModel = "实体";

                if (menu.Icon === "") menu.Icon = "icon-notebook";

                var html = "";
                if (menu.ListModel == 333)
                    html += "<a " + btnStyle + " href=\"javascript:addTab('../CCBill/SearchEditer.htm?FrmID=" + menu.UrlExt + "','" + menu.Name + "');\" >列表</a>";
                else
                    html += "<a " + btnStyle + " href=\"javascript:addTab('../CCBill/SearchDict.htm?FrmID=" + menu.UrlExt + "','" + menu.Name + "');\"  >列表</a>";
                //var url = "/Comm/RefFunc/En.htm?EnName=BP.CCBill.FrmDict&PKVal=Dict_CESHI1";
                // html += "<a class='layui-btn layui-btn-primary layui-border-blue layui-btn-xs' href='../Comm/En.htm?EnName=BP.CCBill.FrmDict&PKVal=" + menu.UrlExt + "' target=_blank >属性</a>";
                html += "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCBill.FrmDict','" + menu.Name + "','" + menu.UrlExt + "')\" >属性</a>";
                html += "<a " + btnStyle + "  href=\"javascript:addTab('../CCBill/Admin/Method.htm?FrmID=" + menu.UrlExt + "&ModuleNo=" + menu.ModuleNo + "','方法:" + menu.Name + "');\" >方法</a>";

                html += "<a " + btnStyle + "  href=\"javascript:GoToFrmDesigner('" + menu.UrlExt + "')\" >表单设计</a>";
                html += " <span class='layui-badge-rim'>实体:" + menu.UrlExt + "</span>";

                menu.Docs = html;

            }

            if (menu.MenuModel == "FlowUrl") {

                if (menu.Icon === "") menu.Icon = "icon-heart";

                var html = "<a " + btnStyle + "  href=\"javascript:addTab('../" + menu.UrlExt + "','" + menu.Name + "');\"  >" + menu.Name + "</a>";
                menu.Docs = html;
            }

            //发起指定的流程的时候.
            if (menu.MenuModel == "StartSpecFlow") {
                if (menu.Icon === "") menu.Icon = "icon-heart";

                var html = "<a " + btnStyle + "  href='../MyFlow.htm?FK_Flow=" + menu.UrlExt + "' target=_blank >发起:" + menu.Name + "</a>";
                html += "|<a " + btnStyle + "  href=''>测试容器</a>";

                html += "<a " + btnStyle + " href=\"javascript:DesignerFlow('" + menu.Tag1 + "','" + menu.Name + "');\" >设计流程</a>";
                menu.Docs = html;
            }

            if (menu.MenuModel == "Bill") {
                var html = "单据:" + menu.UrlExt + "<a " + btnStyle + "  href='../CCBill/SearchBill.htm?FrmID=" + menu.UrlExt + "' target=_blank >列表</a>";
                html += " <a " + btnStyle + "  href=''>设计</a>";
                htm = "<a class='layui-btn layui-btn-primary layui-border-blue layui-btn-xs' href='../CCBill/SearchBill.htm?FrmID=" + menu.UrlExt + "' target=_blank >单据:" + menu.UrlExt + "</a>";
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


                var url = "../Admin/FoolFormDesigner/SFTableEditData.htm?FK_SFTable=" + menu.UrlExt + "&QueryType=Dict";
                var html = "<a " + btnStyle + "  href=\"javascript:LayuiPopRight('" + url + "','" + menu.Name + "','0',false);\" >字典:" + menu.UrlExt + "</a>";
                menu.Docs = html;
            }

            if (menu.MenuModel == "Func") {
                menu.MenuModel = "独立功能";
                if (menu.Icon === "") menu.Icon = "icon-energy";
                var html = "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCBill.Sys.Func','" + menu.Name + "','" + menu.UrlExt + "')\" >功能属性</a>";

                var url = "../CCBill/Func/Func.htm?MyPK=" + menu.UrlExt + "&From=Desinger";
                html += "<a " + btnStyle + "  href=\"javascript:LayuiPopRight('" + url + "','" + menu.Name + "','0',false);\" >功能执行</a>";
                menu.Docs = html;
            }

            //独立流程
            if (menu.MenuModel == "StandAloneFlow") {

                menu.MenuModel = "独立流程";

                if (menu.Mark === "StartFlow") menu.Icon = "icon-paper-plane";
                if (menu.Mark === "Todolist") menu.Icon = "icon-bell";
                if (menu.Mark === "Runing") menu.Icon = "icon-clock";
                if (menu.Mark === "Group") menu.Icon = "icon-chart";
                if (menu.Mark === "Search") menu.Icon = "icon-grid"; //magnifier


                var doc = "<a href=\"javascript:DesignerFlow('" + menu.Tag1 + "','" + menu.Name + "');\" ><i class=icon-heart ></i>设计流程" + menu.Tag1 + "</a>";
                // var html = "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCBill.Sys.Func','" + menu.Name + "','" + menu.UrlExt + "')\" >功能属性</a>";
                var html = "<a " + btnStyle + "  href=\"javascript:LayuiPopRight('" + menu.UrlExt + "','" + menu.Name + "','0',false);\" >执行</a>";
                menu.Docs = html + doc;
            }

            //独立流程
            if (menu.MenuModel == "FlowNewEntity") {

                menu.MenuModel = "新建实体流程";

                if (menu.Mark === "StartFlow") menu.Icon = "icon-paper-plane";
                if (menu.Mark === "Todolist") menu.Icon = "icon-bell";
                if (menu.Mark === "Runing") menu.Icon = "icon-clock";
                if (menu.Mark === "Group") menu.Icon = "icon-chart";
                if (menu.Mark === "Search") menu.Icon = "icon-grid"; //magnifier

                var doc = "<a " + btnStyle + "  href=\"javascript:DesignerFlow('" + menu.Tag1 + "','" + menu.Name + "');\" >设计流程</a>";
                var html = "<a " + btnStyle + "  href=\"javascript:LayuiPopRight('" + menu.UrlExt + "','" + menu.Name + "','0',false);\" >执行:" + menu.Tag1 + "</a>";

                if (menu.Mark === "StartFlow") {
                    html += "<a " + btnStyle + "  href=\"javascript:EnDotHtml('BP.CCBill.Template.MethodFlowNewEntity','" + menu.No + "','','800');\" >属性</a>";
                }

                menu.Docs = html + doc;
            }


            // item.Docs = GenerDoc(item);


            //if (menu.MenuModel == "Dict") menu.MenuModel = "实体";
            //if (menu.MenuModel == "DictTable") menu.MenuModel = "字典表";
            //if (menu.MenuModel == "Bill") menu.MenuModel = "单据";
            //if (menu.MenuModel == "SelfUrl") menu.MenuModel = "自定义";
            //if (menu.MenuModel == "Search") menu.MenuModel = "查询";
            //if (menu.MenuModel == "Start") menu.MenuModel = "发起";
            //if (menu.MenuModel == "Todolist") menu.MenuModel = "待办";
            //  item.Doc = "ssssssss";

        })


        for (var i = 0; i < systems.length; i++) {
            var sys = systems[i];
            sys.open = false
            sys.children = []
            var childModules = modules.filter(function (module) {
                // return module.SystemNo === ''
                return module.SystemNo === sys.No
            })
            for (var j = 0; j < childModules.length; j++) {
                var module = childModules[j]
                module.open = false
                module.children = menus.filter(function (menu) {
                    return menu.ModuleNo == module.No
                })

            }
            sys.children = childModules;
        }

        this.flowNodes = systems;
        this.bindMenu();
        this.initSortArea();
        this.expandAssignMenu()
    }
})

//实体属性.
function EnDotHtml(enName, pkVal, title, width) {

    var url = "../Comm/En.htm?EnName=" + enName + "&PKVal=" + pkVal;

    if (title == undefined)
        title = "";
    if (width == undefined)
        width = 500;

    LayuiPopRight(url, title, width, false);

}

function NewSys() {

    var url = "NewSystem.htm";
    // OpenLayuiDialog(url,"新增系统")；
    // this.openLayer(url, '新增系统');
    OpenLayuiDialog(url, "", 0, 0, null, true);
}

function DictCopy(frmID, menuID) {
    var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin_CreateFunc");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("MenuID", menuID);
    handler.DoMethodReturnJSON("");

}


function DesignerFlow(no, name) {
    var sid = GetQueryString("SID");
    var webUser = new WebUser();
    var url = "../Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + no + "&UserNo=" + webUser.No + "&SID=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
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
