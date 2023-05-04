//方式
var methods = null;
var groups = null;

window.onload = function () {
    window.vm = new Vue({
        el: '#app',
        data: {
            sideBarData: {},
            iframes: [],
            forms: [],
            activeItem: 0,
            tabDropdownVisible: false,
            top: 0,
            left: 0,
            frmGenerUrl: "",
            frmGenerName: "",
            frmGenerNo: "",
            sideBarOpen: false,
            IsReadonly: GetQueryString("IsReadonly"),
            IsCC: "0",
            WorkID: GetQueryString("WorkID"),
            FK_Node: GetQueryString("FK_Node"),
            FK_Flow: GetQueryString("FK_Flow"),
            FID: GetQueryString("FID"),
            PWorkID: GetQueryString("PWorkID"),
            IsShowBtn: true,
            UrlParams: '',

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
                }
            },
            sideBarStyle: function () {
                return {
                    width: this.sideBarOpen ? '240px' : '5px'
                }
            },
            mainStyle: function () {
                return {
                    width: this.sideBarOpen ? 'calc(100% - 240px)' : '100%'
                }
            },
            contentStyle: function () {
                return {
                    width: this.sideBarOpen ? 'calc(100vw - 262px)' : '100%'
                }
            }
        },
        methods: {
            // 重载当前页面
            reLoadCurrentPage: function () {
                var _this = this
                this.$nextTick(function () {
                    if (this.activeItem === -1) {
                        _this.$refs['iframe-home'].contentWindow.location.reload();
                        return
                    }

                    _this.$refs['iframe-' + _this.activeItem][0].contentWindow.location
                        .reload()
                })
            },
            // 关闭当前标签页
            closeCurrentTabs: function (index) {
                if (index == 0)
                    return;
                this.iframes.splice(index, 1)
                var _this = this
                setTimeout(function () {
                    if (_this.iframes.length > index) {
                        _this.activeItem = index
                        return
                    }
                    _this.activeItem = index - 1

                }, 100)
            },
            // 关闭所有
            closeAllTabs: function () {
                this.$set(this, 'iframes', [])
                this.activeItem = 0;
            },
            // 关闭其他
            closeOtherTabs: function () {
                if (this.iframes.length === 0) return
                var currentTab = JSON.parse(JSON.stringify(this.iframes[this.activeItem]))
                this.$set(this, 'iframes', [currentTab])
                this.activeItem = 0
            },
            openTabDropdownMenu: function (e) {
                this.tabDropdownVisible = true
                this.top = e.pageY
                this.left = e.pageX
            },
            openPage: function (formData, url) {
                var iframe = this.$refs['iframe-' + this.activeItem];
                if (iframe != null && iframe != undefined) {
                    iframe = iframe[0];
                    var contentWindow = iframe.contentWindow;
                    try {
                        if (contentWindow != null && contentWindow.Save != undefined && typeof (contentWindow.Save) == "function") {
                            contentWindow.Save(0);
                        }
                    } catch (e) {
                        //页面存在跨域
                        contentWindow.postMessage({ Save: "Save" }, "*")
                    }
                    
                }


                //保存当前打开的Tab
                var loading = layer.msg("加载中..", {
                    icon: 16
                })

                if (formData.No != "PrintDoc" && (url == undefined || url == "")) {
                    var isEdit = formData.IsEdit;
                    if ((this.IsCC && this.IsCC == "1") || this.IsReadonly == "1")
                        isEdit = "0";
                    var isReadonly = this.IsReadonly;
                    if (isEdit == "0")
                        isReadonly = "1";

                    url = basePath + "/WF/CCForm/Frm.htm";
                    url += "?FK_MapData=" + formData.No + "&IsEdit=" + isEdit + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&PWorkID=" + this.PWorkID + "&IsReadonly=" + isReadonly + this.UrlParams;
                }
                if (formData.No == "PrintDoc")
                    url = basePath + "/WF/WorkOpt/PrintDoc.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + Math.random();

                formData.Docs = url;

                var isExist = this.iframes.filter(function (iframe) {
                    return iframe.No === formData.No;
                }).length > 0

                this.frmGenerNo = formData.No;

                //不存在就加载.
                if (!isExist) {
                    this.iframes.push(formData);
                    this.activeItem = this.getIndex(formData)
                    layer.close(loading)
                    return
                }

               

                this.$nextTick(function () {
                    var currentIndex = this.getIndex(formData)
                    if (currentIndex == -1)
                        this.$refs['iframe-home'].contentWindow.location.reload();
                    else
                        this.$refs['iframe-' + currentIndex][0].contentWindow.location.reload();
                    this.activeItem = currentIndex
                    layer.close(loading)

                })


            },
            getIndex: function (formData) {
                if (this.iframes.length === 0) {
                    return;
                }
                for (var i = 0; i < this.iframes.length; i++) {
                    var tab = this.iframes[i]
                    if (tab.No === formData.No) {
                        return i
                    }
                }
                return -1
            },
			openTab: function (formData) {
                //打开一个tab页  lyc
                var isExist = this.iframes.filter(function (iframe) {
                    return iframe.No === formData.No;
                }).length > 0
                //不存在就加载.
                if (!isExist) {
                    this.iframes.push(formData);
                    this.activeItem = this.getIndex(formData)
                    return
                }
            },
            refreshParentTab(formData) {
                var _this = this
                this.$nextTick(function () {
                    if (this.activeItem === -1) {
                        _this.$refs['iframe-home'].contentWindow.location.reload();
                        return
                    }

                    _this.$refs['iframe-' + formData.ParentNo][0].contentWindow.location
                        .reload()
                })
            },

            loadData: function () {
                var params = getQueryStringFromUrl(GetHrefUrl());
                var paramUrl = "";
                if (Array.isArray(params)) {
                    $.each(params, function (i, param) {
                        if (param.indexOf("FK_MapData") != -1)
                            return true;
                        if (param.indexOf("FrmID") != -1)
                            return true;
                        if (param.indexOf("WorkID") != -1)
                            return true;
                        if (param.indexOf("FID") != -1)
                            return true;
                        if (param.indexOf("PWorkID") != -1)
                            return true;
                        if (param.indexOf("FK_Node") != -1)
                            return true;
                        if (param.indexOf("IsReadonly") != -1)
                            return true;
                        paramUrl += "&" + param;
                    })
                }
                this.UrlParams = paramUrl;
                var url = $('#ccbpmJS')[0].src;
                var type = getQueryStringByNameFromUrl(url, "type");
                if (type == "MyView")
                    this.IsReadonly = 1;
                if (type == "MyFrm") {
                    this.IsReadonly = 1;
                    this.IsShowBtn = false;
                }

                var node = new Entity("BP.WF.Node", GetQueryString("FK_Node"));
                //修改网页标题.
                document.title = node.FlowName + ',' + node.Name;
                //获得数据源.
                var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
                handler.AddUrlData();
                handler.AddPara("IsReadonly", this.IsReadonly);
                var data = handler.DoMethodReturnString("FlowFormTree2021_Init");

                if (data.indexOf('err@') == 0) {//发送时发生错误
                    layer.alert(data);
                    return;
                }

                data = JSON.parse(data);
                trees = data["FormTree"];
                this.forms = data["Forms"];
                for (var i = 0; i < trees.length; i++) {
                    var tree = trees[i];
                    tree.open = true;
                    tree.children = this.forms.filter(function (item) {
                        return tree.No === item.ParentNo
                    });
                }
                if (this.forms.length == 1) {
                    this.sideBarOpen = false;
                    $(".indicator").remove();
                } else {
                    this.sideBarOpen = true;
                    this.sideBarData = trees;
                }
                //在forms的后面增加一个打印的数据
                this.forms.push({
                    No: "PrintDoc",
                    ParentNo: "",
                    Name: "打印单据"

                })
               
                
                this.frmGenerUrl = "./CCForm/Frm.htm?FK_MapData=" + this.forms[0].No + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&PWorkID=" + this.PWorkID + "&IsReadonly=" + this.IsReadonly + this.UrlParams;
                this.frmGenerName = this.forms[0].Name;
                this.frmGenerNo = this.forms[0].No;
                this.openPage(this.forms[0]);
                ShowWorkReturnTip(data, this.IsReadonly);
                //显示
                console.log(this.sideBarData)
            },
            ChangeItem: function (index, formData) {
                if (index == 0)
                    this.activeItem = 0;
                else
                    this.activeItem = index;
                this.frmGenerNo = formData.No;
            },
            menuHeight: function (formTree) {
                return {
                    height: formTree.open ? (formTree.children.length * 40 + 60 + 'px') : '60px'
                }
            }

        },
        mounted() {

            this.loadData();
            var color = localStorage.getItem("themeColor")
            chooseTheme(color);
            document.addEventListener('contextmenu', function (e) {
                e.preventDefault()
            });
            if (typeof AfterWindowLoad == "function")
                AfterWindowLoad();
        }
    })
}

/**
 * 增加退回
 */
function ShowWorkReturnTip(flowData, isReadonly) {
    if (isReadonly == 1)
        return;
    //显示退回消息
    var gwf = flowData.WF_GenerWorkFlow[0];
    var scrip = GetPara(gwf.AtPara, "ScripMsg");
    var scripNodeID = GetPara(gwf.AtPara, "ScripNodeID");
    if (scrip == null || scrip == undefined)
        scrip = "";
    if ((flowData.AlertMsg != undefined && flowData.AlertMsg.length != 0) || (scrip != "" && scripNodeID != GetQueryString("FK_Node"))) {
        var _html = "";
        $.each(flowData.AlertMsg, function (i, item) {
            if (item.Title == "退回信息")
                _html += "<div style='padding: 10px 0px 0px 10px;line-height: 24px;color:red'>";
            if (item.Title == "催办信息")
                _html += "<div style='padding: 10px 0px 0px 10px;line-height: 24px;font-weight:bold'>";
            else
                _html += "<div style='padding: 10px 0px 0px 10px;line-height: 24px;'>";
            _html += (i + 1) + "." + item.Title + "<br/>";
            _html += item.Msg;
            _html += "</div>";
        });

        if (scripNodeID != GetQueryString("FK_Node") && scrip != "") {
            _html += "<div style='padding: 10px 0px 0px 10px;line-height: 24px;'>";
            _html += "小纸条<br/>";
            _html += scrip;
            _html += "</div>";
        }
        var h = window.innerHeight - 240;
        //退回消息
        layer.open({
            type: 1,
            skin: '', //加上边框
            area: ['420px', h + 'px'], //宽高
            content: _html
        });
    }
}

//保存 0单保存 1发送的保存
function Save(saveType) {
    //正在保存弹出层
    var index = layer.msg('正在保存，请稍后..', {
        icon: 16
        , shade: 0.01
    });
    //获取表单数据
    if (window.vm.iframes.length == 0) {
        layer.close(index);//关闭正在保存
        return false;
    }
    var isSaveTrue = true;
    var iframes = window.vm.iframes;
    //保存frms中的数据
    $.each(iframes, function (idx, obj) {
        var iframe = window.vm.$refs['iframe-' + idx];
        if (iframe != null && iframe != undefined) {
            iframe = iframe[0];
            var contentWindow = iframe.contentWindow;
            try {
                if (contentWindow != null && contentWindow.Save != undefined && typeof (contentWindow.Save) == "function") {
                    if (contentWindow.Save(saveType) == false)
                        isSaveTrue = false;

                }
            } catch (e) {
                //页面存在跨域
                isSaveTrue = contentWindow.postMessage({ Save: "Save" }, "*")
            }
            
        }
    })
    if (isSaveTrue == false)
        return false;

    //保存前事件
    if (typeof beforeSave != 'undefined' && beforeSave(saveType) instanceof Function)
        if (beforeSave(saveType) == false)
            return false;
    if (isSaveTrue == true) {
        //判断内容是否保存到待办
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddUrlData();
        handler.AddPara("SaveType", saveType);
        handler.DoMethodReturnString("SaveFlow_ToDraftRole");
    }
    layer.close(index);
    setToobarEnable();
    return true;
}
//阅读并关闭.
function Close() {
    if (window.top.vm && typeof window.top.vm.closeCurrentTabs == "function")
        window.top.vm.closeCurrentTabs(window.top.vm.selectedTabsIndex);
    else
        window.close();
}


