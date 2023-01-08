var currentTopContextMenuNodes = []
var currentChildContextMenuNodes = []
var layDropdown = null

window.onload = function () {

    var Condlist = new Vue({
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
                    this.$refs['iframe-' + this.selectedTabsIndex][0].contentWindow.location.reload()
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
            openTabByMenu: function (it,key ,alignRight) {                             
                switch (key) {
                    case "NodeID2":
                        Url = "./CondPRI.htm?CondType=" + GetQueryString("CondType") + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_MainNode=" + GetQueryString("FK_MainNode") + "&FK_Node=" + GetQueryString("FK_Node") + "&FK_Attr=" + GetQueryString("FK_Attr");
                        break;
                    case "NodeID1":
                        Url =  "./CondModel/Default.htm?CondType=" + GetQueryString("CondType") + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_MainNode=" + GetQueryString("FK_MainNode") + "&FK_Node=" + GetQueryString("FK_Node") + "&FK_Attr=" + GetQueryString("FK_Attr");
                        break;
                    default :
                        Url = "../Cond2020/List.htm?CondType=" + GetQueryString("CondType") + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_MainNode=" + GetQueryString("FK_Node") + "&FK_Node=" + GetQueryString("FK_Node") + "&ToNodeID=" + it.NodeID;
                        break;
                   
                }

                this.openTab(it.Name, Url, alignRight);
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
                console.log(this.menuTreeData)
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
                var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_Cond");
                handler.AddUrlData();

                var data = handler.DoMethodReturnString("ConditionLine_Init");

                data = JSON.parse(data);
               /* Glone = { "Name": "转向规则", "NodeID": "NodeID1", "Icon": "iconfont icon-shenhe" };
                data.unshift(Glone);*/
                Gltwo = { "Name": "优先级设置", "NodeID": "NodeID2", "Icon": "iconfont icon-gaojibaobiaoshezhi" };
                data.unshift(Gltwo);
                for (var i = 0; i < data.length; i++) {
                    var en = data[i];
                    if (!en.Icon)
                    en.Icon = "iconfont icon-Send";
                }

                this.refreshMenuTree(data);
              

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

          
        },
        mounted: function () {
      
            var selectedTabsurl = "CondModel/Default.htm?CondType=" + GetQueryString("CondType") + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_MainNode=" + GetQueryString("FK_MainNode") + "&FK_Node=" + GetQueryString("FK_Node") + "&FK_Attr=" + GetQueryString("FK_Attr");
            this.selectedTabsIndexUrl = selectedTabsurl;
            
            this.initMenus()
           
        },

    })   
    window.Condlist = Condlist
}


