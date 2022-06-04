    
new Vue({
    el: '#myEnParas',
    data: {
        myEns: [],
        expandAll: false,
        loadingDialog: false
    },
    watch: {
        expandAll(val) {
            this.expandMenus(val);
        }
    },
    methods: {
        expandMenus: function (status) {
            for (var i = 0; i < this.myEns.length; i++) {
                this.myEns[i].open = status
            }
        },
        DeleteIt: function (no) {

            if (window.confirm("确定要删除吗?") == false)
                return;
            var en = new Entity("BP.CCFast.Portal.WindowTemplate", no);
            var data = en.Delete();
            layer.msg(data);

            //如果有错误.
            if (data.indexOf("err@") == 0)
                return;

            setTimeout(function () {
                window.location.reload()
            }, 2000)
        },
        EditIt: function (no, docModel) {
            var url = "../../Comm/En.htm?EnName=BP.CCFast.Portal.WindowExt." + docModel + "&No=" + no;
            OpenLayuiDialog(url, '', 1100, 89, "auto", false);
        },
        MoveItem(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId) {
            // todo 需要重新实现接口

            var no = GetQueryString("PageID");
            // 方法排序..
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_GPM_Window");
            handler.AddPara("PageID", no);
            handler.AddPara("MyPKs", currentNodeArrStr);
            var data = handler.DoMethodReturnString("Default_Mover");
            layer.msg(data);

        },
        initSortArea: function () {
            var _this = this
            this.$nextTick(function () {
                var childSortableContainers = this.$refs['child-row']

                new Sortable(childSortableContainers, {
                    animation: 150,
                    dataIdAttr: 'data-id',
                    ghostClass: 'blue-background-class',
                    onStart: function ( /**Event*/ evt) {
                        _this.loadingDialog = layer.msg('正在移动...', {
                            timeout: 900 * 1000
                        })
                    },
                    onEnd: function (evt) {
                        /**
                         * 这里区分两种情况，一种是跨列移动，一种非跨列移动
                         * 如果跨列移动，可以利用以下四个参数来实现
                         *
                         * @param pastNodeArrStr    被移出的列的子节点排序
                         * @param pastNodeId        被移出的列的节点id
                         * @param currentNodeArrStr 移入的列的子节点排序
                         * @param currentNodeId     移入的列的节点id
                         *
                         * 假如非跨列，此时被移出的和移入的为同一个，使用前两个参数或者后两个参数都可以实现
                         */
                        layer.close(_this.loadingDialog)
                        var pastNodeArrStr = Array.from(evt.from.querySelectorAll('div[data-id]')).map(function (item) {
                            return item.dataset.id
                        }).join(',')
                        var pastNodeId = evt.from.dataset.id
                        var currentNodeArrStr = Array.from(evt.to.querySelectorAll('div[data-id]')).map(function (item) {
                            return item.dataset.id
                        }).join(',')
                        var currentNodeId = evt.to.dataset.id
                        // 二级菜单的排序
                        _this.MoveItem(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId)
                    }
                })
            })
        },
        // 是否启用
        changemyEnEnableStatus(myEn, ctrl) {
            // 当前启用状态
            //else
            //    en.IsEnable = 0; // myEn.IsEnable;
            //en.Update();
            console.log("更新成功..");
        }
    },
    mounted: function () {
        // fix firefox bug
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        var PageID = GetQueryString("PageID");

        var ens = new Entities("BP.CCFast.Portal.WindowTemplates");
        ens.Retrieve("PageID", PageID);

        this.myEns = ens.TurnToArry(); 

        console.log(this.myEns);
        this.initSortArea();
        // this.bindMenu();

        layui.use('form', function () {
            var form = layui.form;
            form.render()
            // form.on("switch(enable)", function (e) {
            //     console.log(e)
            // })
        });
        var _this = this
        setTimeout(function () {
            _this.expandAll = true
        }, 300)
    }
})
 
function NewIt() {

    var no = GetQueryString("PageID");
    var url = "Html.htm?PageID=" + no + "&MenuNo=" + no;
    OpenLayuiDialog(url, '', 900, 80, "auto", true);
}
