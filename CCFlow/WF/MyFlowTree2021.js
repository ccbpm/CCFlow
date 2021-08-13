//方式
var methods = null;
var groups = null;

window.onload = function () {
    var vm = new Vue({
        el: '#app',
        data: {
            sideBarData: {},
            iframes: [],
            activeItem: 0,
            tabDropdownVisible: false,
            top: 0,
            left: 0,
            frmGenerUrl: "",
            frmGenerName:"",
            frmGenerNo:"",
            sideBarOpen: true,
            IsReadonly:GetQueryString("IsReadonly"),
            IsCC:"0",
            WorkID:GetQueryString("WorkID"),
            FK_Node:GetQueryString("FK_Node"),
            FK_Flow:GetQueryString("FK_Flow"),
            FID:GetQueryString("FID"),
            PWorkID:GetQueryString("PWorkID")

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
                if(index==0)
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
                this.activeItem =0;
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
            openPage: function (formData) {
                var iframe = this.$refs['iframe-' + this.activeItem];
                 if(iframe!=null && iframe!=undefined){
                    iframe = iframe[0];
                    var obj = iframe.contentWindow.document.getElementById('SaveBtn');
                    $(obj).trigger("click"); 
                  }
                   
                
                //保存当前打开的Tab
                var loading = layer.msg("加载中..", {
                    icon: 16
                })
                 var isEdit = formData.IsEdit;
                if ((this.IsCC && this.IsCC == "1") || this.IsReadonly == "1")
                    isEdit = "0";
                var isReadonly = this.IsReadonly;
                if (isEdit == "0")
                    isReadonly="1";

                var url = "./CCForm/Frm.htm?FK_MapData=" + formData.No + "&IsEdit=" + isEdit + "&WorkID=" + this.WorkID+"&FK_Flow="+this.FK_Flow+"&FK_Node="+this.FK_Node+"&FID="+this.FID+"&PWorkID="+this.PWorkID+"&IsReadonly="+isReadonly;
                formData.Docs = url;
                
                var isExist = this.iframes.filter(function (iframe) {
                    return iframe.No === formData.No ;
                }).length > 0
               
                //不存在就加载.
                if (!isExist) {
                    this.iframes.push(formData);
                    this.activeItem = this.getIndex(formData)
                    layer.close(loading)
                    return
                }

                this.$nextTick(function () {
                    var currentIndex = this.getIndex(formData)
                    if(currentIndex==-1)
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


            loadData: function () {
                //获得数据源.
                var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
                handler.AddUrlData();
                var data = handler.DoMethodReturnString("FlowFormTree2021_Init");

                if (data.indexOf('err@') == 0) {//发送时发生错误
                    layer.alert(data);
                    return;
                }

                data = JSON.parse(data);
                trees = data["FormTree"];
                forms = data["Forms"];
                for (var i = 0; i < trees.length; i++) {
                    var tree = trees[i];
                    tree.open = true;
                    tree.children = forms.filter(function (item) {
                        return tree.No === item.ParentNo
                    });
                }
                this.sideBarData = trees;
                this.frmGenerUrl = "./CCForm/Frm.htm?FK_MapData=" + forms[0].No + "&WorkID=" + this.WorkID+"&FK_Node="+this.FK_Node+"&FID="+this.FID+"&PWorkID="+this.PWorkID+"&IsReadonly="+this.IsReadonly;
                this.frmGenerName = forms[0].Name;
                this.frmGenerNo = forms[0].No;
                this.openPage(forms[0]);
                console.log(this.sideBarData)
            },
            menuHeight: function (formTree) {
                return {
                    height: formTree.open ? (formTree.children.length * 40 + 60 + 'px') : '60px'
                }
            }

        },
        mounted() {

            this.loadData()
            document.addEventListener('contextmenu', function (e) {
                e.preventDefault()
            })
        }
    })
}
