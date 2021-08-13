var currentTopContextMenuNodes = []
var currentChildContextMenuNodes = []
var layDropdown = null

window.onload = function () {

    var vm = new Vue({
        el: '#g-app-main',
        data: function () {
            return {
                tabsList: [], // 打开的tab页面
                showPagesAction: false, // 打开tabs操作选项
                selectedTabsIndex: -1, // 当前所选的tab 索引
                selectedTabsIndexUrl: '', 
                sideBarOpen: true,
                inFullScreenMode: false, // 是否处于全屏模式
                showThemePicker: false, // 显示主题选择器
                classicalLayout: true,
                menuTreeData: [], // 目录数据
                subMenuData: [], // 二级目录数据
                selectedTopMenuIndex: 0,
                subMenuTitle: '', // 二级目录标题
                selectedId: -1, // 当前激活的id
                // 是否开启刷新记忆tab功能
                is_remember: false,
                webUser: new WebUser(),
                isAdmin: false,
                showShortCut: false,
                showUserActions: false,
                tabDropdownVisible: false,
                top: 0,
                left: 0,
                closeTimeout: null
            }
        },
        computed: {
            contextMenuStyle: function () {
                return {
                    position: 'fixed',
                    zIndex: 9999,
                    top: (this.top || 0) + 'px',
                    left: (this.left || 0) + 'px',
                    background: 'white',
                    padding: '0 10px',
                    border: '1px solid #eee'
                }
            }
        },
        methods: {

            openTabDropdownMenu: function (e) {
                this.tabDropdownVisible = true
                this.top = e.pageY
                this.left = e.pageX
            },
            selectTopMenu: function (index) {
                if (this.classicalLayout) return
                this.selectedTopMenuIndex = index
                this.selectedSubIndex = -1
                this.subMenuData = this.menuTreeData[index]
                this.subMenuTitle = this.menuTreeData[index].Name
                if (this.subMenuTitle.length > 4)
                    $(".line").css("width", (70 - (this.subMenuTitle.length-4)*8)+"px");
                this.sideBarOpen = true
                this.bindDropdown(this.subMenuData.type)
                this.initChildContextMenu()

            },
            fullScreenOpen: function (uri, name) {
                this.changeFullScreenStatus()
                var w = screen.width,
                    h = screen.height
                layer.open({
                    type: 2,
                    title: name,
                    content: [uri, 'no'],
                    area: [w + 'px', h + 'px'],
                    offset: 'rb',
                    shadeClose: true
                })
            },
            changeFullScreenStatus: function () {
                if (this.inFullScreenMode) {
                    this.exitFullScreen()
                    return
                }
                this.fullScreen()
            },
           
            // 改变侧边栏大小
            resizeSideBar: function () {
                this.sideBarOpen = !this.sideBarOpen
            },
            // 上一个页面
            toLeftPage: function () {
                this.$nextTick(function () {
                    var iframeTabs = this.$refs['iframe-tabs']
                    var offsetWidth = iframeTabs.offsetWidth
                    var scrollWidth = iframeTabs.scrollWidth
                    var offsetLeft = iframeTabs.offsetLeft
                    if (scrollWidth <= offsetWidth) {
                        return
                    }
                    if (offsetLeft < 0) {
                        var leftDistance = offsetLeft + offsetWidth
                        if (leftDistance > 0) {
                            leftDistance = 0
                        }
                        iframeTabs.style.left = leftDistance + 'px'
                    }

                })

            },
            // 下一个页面
            toRightPage: function () {
                this.$nextTick(function () {
                    var iframeTabs = this.$refs['iframe-tabs']
                    var offsetWidth = iframeTabs.offsetWidth
                    var scrollWidth = iframeTabs.scrollWidth
                    var offsetLeft = iframeTabs.offsetLeft
                    if (scrollWidth <= offsetWidth) {
                        return
                    }
                    if (Math.abs(offsetLeft) < scrollWidth - offsetWidth) {
                        iframeTabs.style.left = offsetLeft - offsetWidth + 'px'
                    }
                })
            },
            // 重载当前页面
            reLoadCurrentPage: function () {
                this.$nextTick(function () {
                    if (this.selectedTabsIndex === -1) {
                        this.$refs['iframe-home'].contentWindow.location.reload();
                        return
                    }
                    this.$refs['iframe-' + this.selectedTabsIndex][0].contentWindow.location
                        .reload()
                })
            },
            // 关闭当前标签页
            closeCurrentTabs: function (index) {
                if (index == undefined)
                    index = this.selectedTabsIndex;
                this.tabsList.splice(index, 1)
                var _this = this
                setTimeout(function () {
                    if (_this.tabsList.length > index) {
                        _this.selectedTabsIndex = index
                    } else {
                        _this.selectedTabsIndex = index - 1
                    }
                }, 100)
            },
            // 关闭所有
            closeAllTabs: function () {
                this.tabsList = []
                this.selectedTabsIndex = -1
                this.$nextTick(function () {
                    this.$refs['iframe-tabs'].style.left = 0 + 'px'
                })
            },
            // 关闭其他
            closeOtherTabs: function () {
                if (this.tabsList.length === 0) return
                var tab = this.tabsList[this.selectedTabsIndex]
                this.tabsList = [tab]
                this.selectedTabsIndex = 0
            },
            // 处理tab滚动
            handleTabScroll: function () {
                // 待实现
                // this.$nextTick(function() {
                //     var tabs = this.$refs['iframe-tabs']
                //     var elLeft = tabs.querySelector('.layui-this').offsetLeft


                //     if (elLeft >= 0 && elLeft <= Math.abs(tabs.offsetLeft)) {
                //         return
                //     } else {
                //         tabs.style.left = -(elLeft - elWidth) + 'px'
                //     }
                // })
            },
            openTabByMenu: function (menu, alignRight) {

                //写入日志.
                UserLogInsert("MenuClick", menu.Title + "@" + menu.Icon + "@" + menu.Url);

                this.openTab(menu.Title, menu.Url, alignRight);
            },
            openTab: function (name, src, alignRight) {              


                //如果发起实体类的流程，是通过一个页面中专过去的.
                /*
                 *  /WF/CCBill/Opt/StartFlowByNewEntity.htm
                 *  这里不解析特殊的业务逻辑, 让页面解析。
                 * 
                 */


                if (this.tabsList.length >= 30) {
                    layer.alert('最多可以打开30个标签页~');
                    return;
                }
                var obj = {
                    name: name,
                    src: src
                }

                var idx = this.checkExist(obj)
                if (idx > -1) {
                    this.selectedTabsIndex = idx
                    this.reLoadCurrentPage()
                    return
                }

                if (alignRight) {
                    this.tabsList.splice(this.selectedTabsIndex + 1, 0, obj)
                    this.selectedTabsIndex = this.selectedTabsIndex + 1
                } else {
                    this.tabsList.push(obj)
                    this.selectedTabsIndex = this.tabsList.length - 1

                }
                // if (this.tabsList.length > 5)
                //     this.handleTabScroll()
            },
            checkExist: function (obj) {
                for (var i = 0; i < this.tabsList.length; i++) {
                    var item = this.tabsList[i]
                    if (item.name === obj.name && item.src === obj.src) {
                        return i
                    }
                }
                return -1
            },
            foldMenus: function (menus, c, ev) {
                for (var i = 0; i < menus.length; i++) {
                    var item = menus[i]
                    if (item.No === c.No) {
                        item.open = !item.open
                        continue
                    }
                    item.open = false
                }

            },
            generatePickerBody: function () {
                var tag = "<div style=\"padding-left:10px;padding-right:10px;padding-top:10px\">" +
                    "<form class=\"layui-form layui-form-pane\" action=\"\">" +
                    "   <div class=\"layui-form-item\" pane>" +
                    "   <label class=\"layui-form-label\">\u5206\u680F\u5E03\u5C40</label>" +
                    "<div class=\"layui-input-block\">" +
                    "<input type=\"checkbox\" lay-skin=\"switch\" lay-text=\"\u5F00\u542F|\u5173\u95ED\" lay-filter=\"layout\" ".concat(this.classicalLayout ? '' : 'checked', ">" + "</div>" + "</div>" + "</form>" + "</div>" + "<hr class=\"layui-border-black\">" + "<div class='theme-picker'>");
                for (var key in themeData) {
                    if (themeData.hasOwnProperty(key)) {
                        var item = themeData[key]
                        tag += "\n                    <div class=\"theme\" style=\"background-color: ".concat(item.logo, "\" onclick=\"chooseTheme('").concat(key, "')\">\n                        ").concat(item.alias, "\n                    </div>\n                    ");
                    }
                }
                tag += '</div>'
                return tag;
            },
            openThemePicker: function () {
                var _this = this
                var height = window.innerHeight * 0.8
                layer.open({
                    type: 1,
                    title: '颜色与布局',
                    content: this.generatePickerBody(),
                    area: ['300px', height + 'px'],
                    offset: 'rb',
                    shadeClose: true
                })
                layui.use('form', function () {
                    var form = layui.form
                    form.render()
                    form.on("switch(layout)", function (e) {
                        _this.classicalLayout = !_this.classicalLayout
                        _this.updateLayout()
                    })
                })

            },
            updateLayout: function () {
                var layout = document.getElementById("layout-data")
                if (!this.classicalLayout) {
                    try {
                        this.classicalLayout = false                       
                        layout.innerHTML = "\n                        .g-admin-layout .layui-side{\n                            width: 220px\n                        }\n                        .g-admin-layout .layui-logo, .layui-side-menu .layui-nav{\n                            background-color: white;\n                            position: absolute;\n                            \n                            height: 50px;\n                            line-height:50px;\n                            color: #333;\n                            box-shadow: none;\n                        }\n                        .layui-side-menu .layui-side-scroll{\n                            background-color: white;\n                            width: 220px\n                        }\n                        .g-admin-pagetabs, .g-admin-layout .layui-body, .g-admin-layout .layui-footer, .g-admin-layout .g-layout-left{\n                            left:220px;\n                        }\n                        .layui-side-menu .layui-nav .layui-nav-item a{\n                            height: 30px;\n                            line-height: 30px;\n                            color:#5f626e;\n                            display: flex;\n                            align-items: center;\n                        }\n                        .layui-side-menu .layui-nav .layui-nav-item .layui-icon{\n                            margin-top: -14px;\n                        }          \n                    ";
                        localStorage.setItem("classicalLayout", "0")

                    } catch (error) {
                        layer.msg('加载失败')
                    }
                } else {
                    this.classicalLayout = true
                    layout.innerHTML = ''
                    localStorage.setItem("classicalLayout", "1")
                }
                var color = localStorage.getItem("themeColor")
                chooseTheme(color);

            },
            refreshMenuTree: function (data) {
                //this.menuTreeData = new MenuConvertTools(data).convertToTreeData()
                this.menuTreeData = new MenuConvertTools(data).convertToTreeData()
                layer.close(loading);
                //this.openThemePicker();
                //var color = localStorage.getItem("themeColor")
                //console.log(color)
                // chooseTheme(color)
                this.classicalLayout = parseInt(localStorage.getItem('classicalLayout')) === 1
                this.updateLayout()
            },
            closeDropdown: function (e) {
                try {
                    e.target.parentNode.parentNode.classList.remove('layui-show')

                } catch (e) {
                }
                var _this = this
                if (_this.closeTimeout) {
                    clearTimeout(_this.closeTimeout)
                    _this.closeTimeout = null
                }
                _this.closeTimeout = setTimeout(function () {
                    _this.showShortCut = false
                    _this.showUserActions = false
                    _this.tabDropdownVisible = false
                    clearTimeout(_this.closeTimeout)
                    _this.closeTimeout = null
                }, 300)

            },
            stopTimeout: function () {
                clearTimeout(this.closeTimeout)
                this.closeTimeout = null
            },
            
            initMenus: function () {
                /* var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
                var data = handler.DoMethodReturnString("Default_Init");
                if (data.indexOf('err@') == 0) {
                    alert(data);
                    return;
                }
                if (data.indexOf('url@') == 0) {
                    var url = data.replace('url@', '');
                    window.location.href = url;
                    return;
                }
                data = JSON.parse(data);*/
               var httpHandler = new HttpHandler("BP.WF.HttpHandler.WF_CommEntity");
                var enName = GetQueryString("EnName");
                var type = GetQueryString("type");
                var pkVal = GetPKVal();
                var isTree = GetQueryString("isTree");
                var isReadonly = GetQueryString("isReadonly");
                httpHandler.AddPara("EnName", enName);
              
                if (pkVal != null) {
                    httpHandler.AddPara("PKVal", pkVal);
                }

                var data = httpHandler.DoMethodReturnString("Entity_Init");
                if (data.indexOf('err@') == 0) {
                    $("#CCFormTabs").html(data);
                    return;
                }

                //解析json.
                frmData = JSON.parse(data);
                dtM = frmData["dtM"];
             

                this.refreshMenuTree(dtM);
              

            },
         
            bindDropdown: function (type) {
                var _this = this
                this.$nextTick(function () {
                    layui.use('dropdown', function () {

                        var dropdown = layui.dropdown

                        if (currentTopContextMenuNodes.length > 0) {
                            for (let i = 0; i < currentTopContextMenuNodes.length; i++) {
                                currentTopContextMenuNodes[i].removeEventListener('contextmenu', null)
                            }
                        }
                        if (currentChildContextMenuNodes.length > 0) {
                            for (let i = 0; i < currentChildContextMenuNodes.length; i++) {
                                currentChildContextMenuNodes[i].removeEventListener('contextmenu', null)
                            }
                        }
                        if (type === 'flow') {
                            var topFlowNodeItems = [
                                { title: '<i class=icon-plus></i> 新建流程', id: "NewFlow", Icon: "icon-plus" },
                                { title: '<i class=icon-star></i> 目录属性', id: "EditSort", Icon: "icon-options" },
                                { title: '<i class=icon-folder></i> 新建目录', id: "NewSort", Icon: "icon-magnifier-add" },
                                {
                                    title: '<i class=icon-share-alt ></i> 导入流程模版',
                                    id: "ImpFlowTemplate",
                                    Icon: "icon-plus"
                                },
                                //{ title: '新建下级目录', id: 5 },
                                { title: '<i class=icon-close></i> 删除目录', id: "DeleteSort", Icon: "icon-close" }
                            ]

                            var tfOptions = {
                                trigger: 'contextmenu',
                                data: topFlowNodeItems,
                                click: function (data, oThis) {
                                    _this.topFlowNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.idx)
                                }
                            }
                            var topFlowNodeItems = document.querySelectorAll('.flow-node')
                            for (var i = 0; i < topFlowNodeItems.length; i++) {
                                tfOptions.elem = topFlowNodeItems[i]
                                dropdown.render(tfOptions)
                            }


                            var childFlowNodeItems = [
                                { title: '<i class=icon-star></i> 流程属性', id: "Attr", Icon: "icon-options" },
                                { title: '<i class=icon-settings></i> 设计流程', id: "Designer", Icon: "icon-settings" },
                                { title: '<i class=icon-plane></i> 测试容器', id: "Start", Icon: "icon-paper-plane" },
                                { title: '<i class=icon-docs></i> 复制流程', id: "Copy", Icon: "icon-docs" },
                                { title: '<i class=icon-close></i> 删除流程', id: "Delete", Icon: "icon-close" }
                            ]
                            var cfOptions = {
                                trigger: 'contextmenu',
                                data: childFlowNodeItems,
                                click: function (data, oThis) {
                                    var dataset = $(this.elem)[0].dataset
                                    _this.childFlowNodeOption(data.id, dataset.no, dataset.name, dataset.pidx, dataset.idx)
                                }
                            }
                            var childFlowNodeItems = document.querySelectorAll('.flow-node-child')
                            for (var i = 0; i < childFlowNodeItems.length; i++) {
                                cfOptions.elem = childFlowNodeItems[i]
                                dropdown.render(cfOptions)
                            }
                            currentTopContextMenuNodes = topFlowNodeItems
                            currentChildContextMenuNodes = childFlowNodeItems
                            return
                        }

                        if (type === 'form') {
                            var topFormNodeItems = [
                                { title: '<i class=icon-plus></i> 新建表单', id: "NewFrm", Icon: "icon-plus" },
                                { title: '<i class=icon-star></i> 重命名', id: "EditSort", Icon: "icon-options" },
                                { title: '<i class=icon-folder></i> 新建目录', id: "NewFrmSort", Icon: "icon-magnifier-add" },
                                {
                                    title: '<i class=icon-share-alt ></i> 导入表单模版',
                                    id: "ImpFlowTemplate",
                                    Icon: "icon-plus"
                                },
                                //{ title: '新建下级目录', id: 5 },
                                { title: '<i class=icon-close></i> 删除目录', id: "DeleteSort", Icon: "icon-close" }
                            ]

                            var tfOptions = {
                                trigger: 'contextmenu',
                                data: topFormNodeItems,
                                click: function (data, oThis) {
                                    _this.topFormNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.idx)
                                }
                            }

                            var topFormNodeItems = document.querySelectorAll('.form-node')
                            for (var i = 0; i < topFormNodeItems.length; i++) {
                                tfOptions.elem = topFormNodeItems[i]
                                dropdown.render(tfOptions)
                            }


                            var childFormNodeItems = [
                                { title: '<i class=icon-star></i> 表单属性', id: "Attr", Icon: "icon-options" },
                                { title: '<i class=icon-settings></i> 设计表单', id: "Designer", Icon: "icon-settings" },
                                { title: '<i class=icon-plane></i> 运行表单', id: "Start", Icon: "icon-paper-plane" },
                                { title: '<i class=icon-docs></i> 复制表单', id: "Copy", Icon: "icon-docs" },
                                { title: '<i class=icon-close></i> 删除表单', id: "Delete", Icon: "icon-close" }
                            ]
                            var cfOptions = {
                                trigger: 'contextmenu',
                                data: childFormNodeItems,
                                click: function (data, oThis) {
                                    var dataset = $(this.elem)[0].dataset
                                    _this.childFormNodeOption(data.id, dataset.no, dataset.name, dataset.pidx, dataset.idx)
                                }
                            }
                            var childFormNodeItems = document.querySelectorAll('.form-node-child')
                            for (var i = 0; i < childFormNodeItems.length; i++) {
                                cfOptions.elem = childFormNodeItems[i]
                                dropdown.render(cfOptions)
                            }
                            currentTopContextMenuNodes = topFormNodeItems
                            currentChildContextMenuNodes = childFormNodeItems
                        }
                    })
                })
            },
            NewSort: function (currentElem, sameLevel) {
                //只能创建同级.
                sameLevel = true;
                //例子2
                layer.prompt({
                    value: '',
                    title: '新建' + (sameLevel ? '同级' : '子级') + '流程类别',
                }, function (value, index, elem) {
                    layer.close(index);
                    var en = new Entity("BP.WF.Template.FlowSort", currentElem);
                    var data = "";
                    if (sameLevel == true) {
                        data = en.DoMethodReturnString("DoCreateSameLevelNodeMy", value);
                    } else {
                        data = en.DoMethodReturnString("DoCreateSubNodeMy", value);
                    }
                    layer.msg("创建成功" + data);
                    setTimeout(function () {
                        window.location.reload();
                    }, 2000);
                });
            },
            DeleteSort: function (no) {

                if (window.confirm("确定要删除吗?") == false)
                    return;

                var en = new Entity("BP.WF.Template.FlowSort", no);
                var data = en.Delete();
                layer.msg(data);

                //如果有错误.
                if (data.indexOf("err@") == 0)
                    return;

                setTimeout(function () {
                    window.location.reload()
                }, 2000)
            },

            ImpFlowTemplate: function (data) {
                var sortNo = data;
                url = "./../Admin/Template/ImpFrmLocal.htm?SortNo=" + sortNo + "&Lang=CH";
                this.openTab("导入表单模版", url);
            },
            
            NewFlow: function (data, name) {

                ////  if (runModelType == 0)
                //   url = "../CCBPMDesigner/FlowDevModel/Default.htm?SortNo=" + flowSort + "&s=" + Math.random();
                //else
                url = "../Admin/CCBPMDesigner/FlowDevModel/Default.htm?SortNo=" + data + "&From=Flows.htm&RunModel=1&s=" + Math.random();
                this.openTab("新建流程", url);

            },
            NewFrm: function (data, name) {
                url = "../Admin/FoolFormDesigner/NewFrmGuide.htm?SortNo=" + data + "&From=Frms.htm&RunModel=1&s=" + Math.random();
                this.openTab("新建表单", url);
            },
            childFormNodeOption: function (key, data, name, pidx, idx) {

                switch (key) {
                    case "Attr":
                        this.flowAttr(data, name);
                        break;
                    case "Designer":
                        this.Designer(data, name);
                        break;
                    case "Start":
                        this.StartFrm(data, name);
                        break;
                    case "Copy":
                        this.copyFrm(data);
                        break;
                    case "Delete":
                        this.DeleteFlow(data, pidx, idx);
                        break;
                }
            },

            topFormNodeOption: function (key, data, name) {
                switch (key) {
                    case "EditSort":
                        this.EditSort(data, name);
                        break;
                    case "ImpFlowTemplate":
                        this.ImpFlowTemplate(data);
                        break;
                    case "NewSort":
                        this.NewSort(data, true);
                        break;
                    case "DeleteSort":
                        this.DeleteSort(data);
                        break;
                    case "NewFlow":
                        this.NewFlow(data, name);
                        break;
                    case "NewFrm":
                        this.NewFrm(data, name);
                        break;
                    default:
                        alert("没有判断的命令" + key);
                        break;
                }
            },

            childFlowNodeOption: function (key, data, name, pidx, idx) {
                console.log(key, data, name, pidx, idx)
                switch (key) {
                    case "Attr":
                        this.fFlowAttr(data, name);
                        break;
                    case "Designer":
                        this.fDesigner(data, name);
                        break;
                    case "Start":
                        this.fTestFlow(data, name);
                        break;
                    case "Copy":
                        this.fCopyFlow(data);
                        break;
                    case "Delete":
                        this.fDeleteFlow(data, pidx, idx);
                        break;
                }
            },

            topFlowNodeOption: function (key, data, name) {

                switch (key) {
                    case "EditSort":
                        this.fEditSort(data, name);
                        break;
                    case "ImpFlowTemplate":
                        this.fImpFlowTemplate(data);
                        break;
                    case "NewSort":
                        this.fNewSort(data, true);
                        break;
                    case "DeleteSort":
                        this.fDeleteSort(data);
                        break;
                    case "NewFlow":
                        this.fNewFlow(data, name);
                        break;
                    default:
                        alert("没有判断的命令" + key);
                        break;
                }
            },

            fNewFlow: function (data, name) {
                url = "../Admin/CCBPMDesigner/FlowDevModel/Default.htm?SortNo=" + data + "&From=Flows.htm&RunModel=1&s=" + Math.random();
                this.openTab("新建流程", url);

            },
            fDeleteSort: function (no) {

                if (window.confirm("确定要删除吗?") == false)
                    return;

                var en = new Entity("BP.WF.Template.FlowSort", no);
                var data = en.Delete();
                layer.msg(data);

                //如果有错误.
                if (data.indexOf("err@") == 0)
                    return;

                setTimeout(function () {
                    window.location.reload()
                }, 2000)
            },

            fImpFlowTemplate: function (data) {
                var fk_sort = data;
                url = "./../Admin/AttrFlow/Imp.htm?FK_FlowSort=" + fk_sort + "&Lang=CH";
                this.openTab("导入流程模版", url);
            },

            fNewSort: function (currentElem, sameLevel) {

                //只能创建同级.
                sameLevel = true;

                //例子2
                layer.prompt({
                    value: '',
                    title: '新建' + (sameLevel ? '同级' : '子级') + '流程类别',
                }, function (value, index, elem) {
                    layer.close(index);
                    var en = new Entity("BP.WF.Template.FlowSort", currentElem);
                    var data = "";
                    if (sameLevel == true) {
                        data = en.DoMethodReturnString("DoCreateSameLevelNodeMy", value);
                    } else {
                        data = en.DoMethodReturnString("DoCreateSubNodeMy", value);
                    }

                    layer.msg("创建成功" + data);
                    //this.EditSort(data, "编辑");
                    //return;

                    setTimeout(function () {
                        window.location.reload();
                    }, 2000);
                });
            },

            openLayer: function (uri, name, w, h) {
                //console.log(uri, name);

                if (w === 0)
                    w = window.innerWidth;

                if (w === undefined)
                    w = window.innerWidth / 2;

                if (h === undefined)
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

            fEditSort: function (no, name) {
                var url = "../Comm/EnOnly.htm?EnName=BP.WF.Template.FlowSort&No=" + no;
                this.openLayer(url, "目录:" + name);
            },

            fDeleteFlow: function (no, pidx, idx) {
                var msg = "提示: 确定要删除该流程吗?";
                msg += "\t\n1.如果该流程下有实例，您不能删除。";
                msg += "\t\n2.该流程为子流程的时候，被引用也不能删除.";
                if (window.confirm(msg) == false)
                    return;

                var load = layer.msg("正在处理,请稍候...", {
                    icon: 16,
                    anim: 5
                })
                var _this = this
                setTimeout(function () {
                    //开始执行删除.
                    var flow = new Entity("BP.WF.Flow", no);
                    var data = flow.DoMethodReturnString("DoDelete");
                    layer.msg(data);
                    if (data.indexOf("err@") == 0)
                        return;


                    layer.close(load)
                    _this.subMenuData.children[pidx].children.splice(idx, 1)
                    var leaveItems = _this.subMenuData.children[pidx].children
                    _this.$set(_this.subMenuData.children[pidx], 'children', leaveItems)
                }, 120)
            },
            fCopyFlow: function (no) {
                if (window.confirm("确定要执行流程复制吗?") == false)
                    return;
                var flow = new Entity("BP.WF.Flow", no);
                var data = flow.DoMethodReturnString("DoCopy");
                layer.msg(data);
                setTimeout(function () {
                    window.location.reload();
                }, 2000);
            },
            fDesigner: function (no, name) {
                var sid = GetQueryString("SID");
                var webUser = new WebUser();
                var url = "../Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + no + "&UserNo=" + webUser.No + "&SID=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
                this.openTab(name, url);
            },
            fFlowAttr: function (no, name) {
                var url = "../Comm/En.htm?EnName=BP.WF.Template.FlowExt&No=" + no;
                this.openTab(name, url);
                //this.openLayer(url, name,900);
            },
            fTestFlow: function (no, name) {
                var url = "../Admin/TestingContainer/TestFlow2020.htm?FK_Flow=" + no;
                window.top.vm.fullScreenOpen(url, name);
                // this.openLayer(url, name);
            },
            Designer: function (no, name) {
                var sid = GetQueryString("SID");
                var webUser = new WebUser();
                var url = "../Admin/CCFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + no + "&FrmID=" + no + "&UserNo=" + webUser.No + "&SID=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
                this.openTab(name, url);
            },
            EditSort: function (no, name) {

                var val = prompt("请输入名称", name);
                if (val == null || val == '')
                    return;

                var en = new Entity("BP.WF.Template.SysFormTree", no);
                en.Name = val;
                en.Update();
            },
            StartFrm: function (no, name) {
                var sid = GetQueryString("SID");
                var webUser = new WebUser();
                var url = "../Admin/CCFormDesigner/GoToRunFrm.htm?FK_MapData=" + no + "&FrmID=" + no + "&UserNo=" + webUser.No + "&SID=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
                this.openTab(name, url);
            },
            flowAttr: function (no, name) {
                var sid = GetQueryString("SID");
                var webUser = new WebUser();
                var url = "../Admin/CCFormDesigner/GoToFrmAttr.htm?FK_MapData=" + no + "&FrmID=" + no + "&UserNo=" + webUser.No + "&SID=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
                this.openTab(name, url);
            },
            copyFrm: function (no) {
                if (window.confirm("确定要执行表单复制吗?") == false)
                    return;
                var flow = new Entity("BP.Sys.MapData", no);
                var data = flow.DoMethodReturnString("DoCopy");
                layer.msg(data);
                setTimeout(function () {
                    window.location.reload();
                }, 2000);
            },
            DeleteFlow: function (no, pidx, idx) {
                var msg = "提示: 确定要删除该表单吗?";
                //   msg += "\t\n1.如果该流程下有实例，您不能删除。";
                //  msg += "\t\n2.该流程为子流程的时候，被引用也不能删除.";
                if (window.confirm(msg) == false)
                    return;

                var load = layer.msg("正在处理,请稍候...", {
                    icon: 16,
                    anim: 5
                })
                var _this = this
                setTimeout(function () {
                    var en = new Entity("BP.Sys.MapData", no);
                    en.Delete();
                    //  var data = flow.DoMethodReturnString("DoDelete");
                    layer.close(load);
                    // layer.msg(data);
                    _this.subMenuData.children[pidx].children.splice(idx, 1)
                    var leaveItems = _this.subMenuData.children[pidx].children
                    _this.$set(_this.subMenuData.children[pidx], 'children', leaveItems)
                }, 120)
            },
            calcClassList: function (item, type) {
                var cList = []
                if (item.type === 'flow') cList.push(type === 1 ? 'flow-node' : 'flow-node-child')
                if (item.type === 'form') cList.push(type === 1 ? 'form-node' : 'form-node-child')
                return cList
            }
        },
        mounted: function () {
            var param = document.location.search.substr(1);           
            selectedTabsurl = "../RefFunc/EnOnly.htm?" + param;
            this.selectedTabsIndexUrl = selectedTabsurl;
            
            this.initMenus()
            var _this = this
            setTimeout(function () {
                _this.bindDropdown('flow')
            }, 500)
        },

    })
    window.vm = vm

}


