//方式
var methods = null;
var groups = null;

window.onload = function () {
    var vm = new Vue({
        el: '#app',
        data: {
            sideBarData: {},
            iframes: [],
            activeItem: -1,
            tabDropdownVisible: false,
            top: 0,
            left: 0,
            MyDictUrl: "",
            sideBarOpen: true
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
                this.activeItem = -1
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
            openPage: function (method) {

                // alert(method.No);

                var loading = layer.msg("加载中..", {
                    icon: 16
                })

                //var load = layer.msg("正在处理,请稍候...", {
                //    icon: 16,
                //    anim: 2
                //})

                if (method.MethodModel === "Bill") {

                    //method.Docs = "./Opt/Bill.htm?FrmID=" + method.Tag1 + "&MethodNo=" + method.No + "&WorkID=" + GetQueryString("WorkID") + "&From=Dict";
                    method.Docs = "./SearchBill.htm?FrmID=" + method.Tag1 + "&MethodNo=" + method.No + "&PFrmID=" + method.FrmID + "&PWorkID=" + GetQueryString("WorkID") + "&From=Dict";

                    //if (method.Docs == "") {
                    //    alert("没有解析的mark=" + method.Mark);
                    //    return;
                    //}
                    //   alert(method.Docs);
                    //alert(method.Docs);
                }

                //如果是一个方法.
                if (method.MethodModel === "Func") {
                    method.Docs = "./Opt/DoMethod.htm?FrmID=" + method.FrmID + "&No=" + method.No + "&WorkID=" + GetQueryString("WorkID");
                }


                if (method.MethodModel === "FrmBBS") {
                    method.Docs = "./OptComponents/FrmBBS.htm?FrmID=" + method.FrmID + "&No=" + method.No + "&WorkID=" + GetQueryString("WorkID");
                }

                if (method.MethodModel === "QRCode") {
                    method.Docs = "./OptComponents/QRCode.htm?FrmID=" + method.FrmID + "&MethodNo=" + method.No + "&WorkID=" + GetQueryString("WorkID")+"&IsReadonly="+GetQueryString("IsReadonly");
                }

                //单个实体发起的流程汇总.
                if (method.MethodModel === "SingleDictGenerWorkFlows") {
                    method.Docs = "./OptOneFlow/SingleDictGenerWorkFlows.htm?FrmID=" + method.FrmID + "&No=" + method.No + "&MethodNo=" + method.No + "&WorkID=" + GetQueryString("WorkID");
                }

                //修改基础数据的的流程.
                if (method.MethodModel === "FlowBaseData") {
                    //通过找个方法 window.open(method.Docs);

                    var url = "./OptOneFlow/FlowBaseData.htm?WorkID=" + GetQueryString("WorkID");
                    url += "&FrmID=" + GetQueryString("FrmID");
                    url += "&MethodNo=" + method.No;
                    url += "&FlowNo=" + method.FlowNo;

                    //  var myurl = DoFlowBaseData(method);
                    // if (!myurl) return;
                    method.Docs = url;
                }

                //其他业务流程.
                if (method.MethodModel == "FlowEtc") {

                    var url = "./OptOneFlow/FlowEtc.htm?WorkID=" + GetQueryString("WorkID");
                    url += "&FrmID=" + GetQueryString("FrmID");
                    url += "&MethodNo=" + method.No; // GetQueryString("MethodNo");
                    url += "&FlowNo=" + method.FlowNo;
                    //  var myurl = DoFlowBaseData(method);
                    // if (!myurl) return;
                    method.Docs = url;

                    //通过找个方法 window.open(method.Docs);
                    // var myurl = DoFlowEtc(method);
                    //  if (myurl == null) return;
                    // method.Docs = myurl;
                }

                //数据版本.
                if (method.MethodModel == "DataVer") {
                    method.Docs = "./OptComponents/DataVer.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
                }

                //日志.
                if (method.MethodModel == "DictLog") {
                    method.Docs = "./OptComponents/DictLog.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
                }

                //超链接.
                if (method.MethodModel == "Link") {
                    if (method.Tag1.indexOf('?') > 0)
                        method.Docs = method.Tag1 + "&FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
                    else
                        method.Docs = method.Tag1 + "?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
                }

                if (method.Docs === "") {

                    var url = method.UrlExt;
                    if (url === "") {
                        alert("没有解析的Url-MethodModel:" + method.MethodModel + " - " + method.Mark);
                        return;
                    }
                    if (url.indexOf('?') > 0)
                        method.Docs = url + "&FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
                    else
                        method.Docs = url + "?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
                }

                var isExist = this.iframes.filter(function (iframe) {
                    return iframe.No === method.No;
                }).length > 0


                //不存在就加载.
                if (!isExist) {
                    this.iframes.push(method);
                    this.activeItem = this.getIndex(method)
                    layer.close(loading)
                    return
                }

                this.$nextTick(function () {
                    var currentIndex = this.getIndex(method)
                    this.$refs['iframe-' + currentIndex][0].contentWindow.location.reload();
                    this.activeItem = currentIndex
                    layer.close(loading)

                })


            },
            getIndex: function (method) {
                if (this.iframes.length === 0) {
                    return
                }
                for (var i = 0; i < this.iframes.length; i++) {
                    var tab = this.iframes[i]
                    if (tab.No === method.No) {
                        return i
                    }
                }
                return -1
            },


            loadData: function () {
                //获得数据源.
                var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
                handler.AddUrlData();
                var ds = handler.DoMethodReturnJSON("Method_Init");
                groups = ds["Groups"];
                methods = ds["Methods"];
                for (var i = 0; i < groups.length; i++) {
                    var group = groups[i];
                    group.open = true;
                    group.children = methods.filter(function (item) {
                        return group.No === item.GroupID && item.IsEnable==1
                    });
                }
                this.sideBarData = groups;
                console.log(this.sideBarData)
                if (methods.length == 0) {
                    $(".sidebar").hide();
                    $(".indicator").hide();
                    this.sideBarOpen = false;
                }

            },
            menuHeight: function (group) {
                return {
                    height: group.open ? (group.children.length * 40 + 60 + 'px') : '60px'
                }
            }

        },
        mounted() {

            this.MyDictUrl = "MyDict.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
            //this.MyDictUrl = "sina.com.cn";

            this.loadData()
            document.addEventListener('contextmenu', function (e) {
                e.preventDefault()
            })
        }
    })
}
$(function () {
    var theme = DealText(localStorage.getItem("themeColorInfo"));
    theme = JSON.parse(theme);
    var styleScope = document.getElementById("theme-data")
    styleScope.innerHTML = DealText("\n .sidebar .group .group-items .active{\n background-color:" + theme.selectedMenu + ";\n}");

})
