
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
            var en = new Entity("BP.WF.Template.FlowTab", no);
            var data = en.Delete();
            layer.msg(data);

            //如果有错误.
            if (data.indexOf("err@") == 0)
                return;

            setTimeout(function () {
                window.location.reload()
            }, 1000)
        },
        EditIt: function (no, docModel) {
            var url = "../../../WF/Comm/EnOnly.htm?EnName=BP.WF.Template.FlowTab&MyPK=" + no;
            OpenLayuiDialog(url, '', 900, 80, "auto", true);
        },
        updateSystemSort: function (pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId) {


            // todo 需要重新实现接口
            var ens = new Entities("BP.WF.Template.FlowTabs");
            var flowNo = GetQueryString("FlowNo");
            var data = ens.DoMethodReturnString("Default_Mover", flowNo, currentNodeArrStr);



        },
        updateSort: function (currentNodeArrStr) {

            var flowNo = GetQueryString("FlowNo");

            // 排序..
            var ens = new Entities("BP.WF.Template.FlowTabs");
            var data = ens.DoMethodReturnString("Default_Mover", flowNo, currentNodeArrStr);

        },
        initSortArea: function () {
            var _this = this
            this.$nextTick(function () {
                var mainContainer = this.$refs['default-row']
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
                        layer.close(_this.loadingDialog);
                        console.log(evt);
                        var currentNodeArrStr = Array.from(evt.to.querySelectorAll('div[data-id]')).map(function (item) {
                            return item.dataset.mypk
                        }).join(',');

                        console.log(currentNodeArrStr);

                        _this.updateSort(currentNodeArrStr);
                    }
                });
            })
        },
        // 是否启用
        changeEnableStatus(myEn, ctrl) {
            // 当前启用状态
            var en = new Entity("BP.WF.Template.FlowTab", myEn.MyPK);
            if (en.isEnable == 0)
                en.isEnable = 1;
            else
                en.isEnable = 0;

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

        var flowNo = GetQueryString("FlowNo");

        var ens = new Entities("BP.WF.Template.FlowTabs");
        var i = ens.Retrieve("FK_Flow", flowNo, "Idx");
        if (ens.length == 0) {
            var en = new Entity("BP.WF.Template.FlowTab");
            en.Name = "主页";
            en.Mark = "Default";
            en.Tip = "当前流程需要我做的工作，包括退回。";
            en.FK_Flow = flowNo;
            en.MyPK = flowNo + "_" + en.Mark;
            en.UrlExt = "/CCFast/StandAloneFlow/" + en.Mark + ".htm?FlowNo=" + flowNo + "&FK_Flow=" + en.FK_Flow;
            en.IsEnable = 1;
            en.Icon = "icon-drop";
            en.Idx = 0;
            en.Insert();

            var en = new Entity("BP.WF.Template.FlowTab");
            en.Name = "待办";
            en.Mark = "Todolist";
            en.Tip = "当前流程需要我做的工作，包括退回。";
            en.FK_Flow = flowNo;
            en.MyPK = flowNo + "_" + en.Mark;
            en.UrlExt = "/CCFast/StandAloneFlow/" + en.Mark + ".htm?FlowNo=" + flowNo + "&FK_Flow=" + en.FK_Flow;
            en.IsEnable = 1;
            en.Icon = "icon-drop";
            en.Idx = 1;
            en.Insert();

            var en = new Entity("BP.WF.Template.FlowTab");
            en.Name = "在途";
            en.Mark = "Runing";
            en.Tip = "我参与的但是没有完成的工作.";
            en.FK_Flow = flowNo;
            en.IsEnable = 1;
            en.Idx = 1;
            en.MyPK = flowNo + "_" + en.Mark;
            en.Icon = "icon-drop";
            en.UrlExt = "/CCFast/StandAloneFlow/" + en.Mark + ".htm?FlowNo=" + flowNo + "&FK_Flow=" + en.FK_Flow;
            en.Insert();

            var en = new Entity("BP.WF.Template.FlowTab");
            en.Name = "抄送";
            en.Mark = "CC";
            en.Tip = "别人抄送（让我知道）给我的工作，我可以评论但是不能干预流程的运行.";
            en.FK_Flow = flowNo;
            en.IsEnable = 1;
            en.Idx = 1;
            en.MyPK = flowNo + "_" + en.Mark;
            en.Icon = "icon-drop";
            en.UrlExt = "/CCFast/StandAloneFlow/" + en.Mark + ".htm?FlowNo=" + flowNo + "&FK_Flow=" + en.FK_Flow;
            en.Insert();

            var en = new Entity("BP.WF.Template.FlowTab");
            en.Name = "草稿";
            en.Mark = "Draft";
            en.FK_Flow = flowNo;
            en.Tip = "我启动的流程临时保存的，但是没有发送出去的流程。";
            en.IsEnable = 1;
            en.Idx = 1;
            en.MyPK = flowNo + "_" + en.Mark;
            en.Icon = "icon-drop";
            en.UrlExt = "/CCFast/StandAloneFlow/" + en.Mark + ".htm?FlowNo=" + flowNo + "&FK_Flow=" + en.FK_Flow;
            en.Insert();

            var en = new Entity("BP.WF.Template.FlowTab");
            en.Name = "统计";
            en.Mark = "Nums";
            en.Tip = "当前的流程统计分析.";
            en.FK_Flow = flowNo;
            en.IsEnable = 1;
            en.Idx = 1;
            en.MyPK = flowNo + "_" + en.Mark;
            en.Icon = "icon-drop";
            en.UrlExt = "/CCFast/StandAloneFlow/" + en.Mark + ".htm?FlowNo=" + flowNo + "&FK_Flow=" + en.FK_Flow;
            en.Insert();

            var en = new Entity("BP.WF.Template.FlowTab");
            en.Name = "查询";
            en.Mark = "Search";
            en.Tip = "综合查询页面.";
            en.FK_Flow = flowNo;
            en.IsEnable = 1;
            en.Idx = 1;
            en.MyPK = flowNo + "_" + en.Mark;
            en.Icon = "icon-drop";
            en.UrlExt = "/CCFast/StandAloneFlow/" + en.Mark + ".htm?FlowNo=" + flowNo + "&FK_Flow=" + en.FK_Flow;
            en.Insert();

            ens.Retrieve("FK_Flow", flowNo, "Idx");
        }

        this.myEns = ens.TurnToArry();

        // ens.DoMethodReturnJson("Default_Init", flowNo);
        //  this.myEns = ens.TurnToArry();

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
            _this.expandAll = true;
        }, 300)
    }
})

function NewIt() {

    var no = GetQueryString("FlowNo");

    var url = "SelfUrl.htm?FlowNo=" + no + "&MenuNo=" + no + "&SystemNo=" + systemNo;
    OpenLayuiDialog(url, '', 900, 80, "auto", true);
}
