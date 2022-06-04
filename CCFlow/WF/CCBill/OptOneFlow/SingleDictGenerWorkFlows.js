
new Vue({
    el: '#method',
    data: {
        flows: [],
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
            for (var i = 0; i < this.flows.length; i++) {
                this.flows[i].open = status               
            }
        },

        OpenMyView: function (flowNo, workid) {

            var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyView");
            handler.AddPara("WorkID", workid);
            handler.AddPara("FK_Flow", flowNo);
            var data = handler.DoMethodReturnString("MyView_Init");
            if (data.indexOf('err@') == 0) {
                $("#Msg").html("<br>" + data);
                return;
            }
            if (data.indexOf('url@') == 0) {

                data = data.replace('url@', ''); //如果返回url，就直接转向.
                data = data.replace('?DoType=HttpHandler', '?');
                data = data.replace('&DoType=HttpHandler', '');
                data = data.replace('&DoMethod=MyView_Init', '');
                data = data.replace('&HttpHandlerName=BP.WF.HttpHandler.WF_MyCC', '');
                data = data.replace('?&', '?');
                //如果返回url，就直接转向.
                SetHref("../../" + data);
                return;
            }
        },
        //查询
        StartFlow: function (No, Name) {

            var frmIDOfDict = GetQueryString("FrmID");
            var workIDOfDict = GetQueryString("WorkID");

            //获得方法与流程的对应关系? 根据实体与流程的关系类型不同，调用不同的流程启动方式.
            var method = new Entity("BP.CCBill.Template.Method");
            var i = method.Retrieve("FrmID", frmIDOfDict, "FlowNo", No);
            if (i == 0) {
                alert("流程为:" + No + "的方法已经被删除.");
                return;
            }

            var url = "FlowCenter.htm?WorkID=" + workIDOfDict + "&FrmID=" + frmIDOfDict + "&MenuNo=" + No + "&FlowNo=" + Name;
            SetHref(url);
            return;
        }
    },
    mounted: function () {

        // fix firefox bug
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }
        //获得数据源.
        var handler = new HttpHandler("BP.CCBill.WF_CCBill_OptOneFlow");
        handler.AddUrlData();
        var ds = handler.DoMethodReturnJSON("SingleDictGenerWorkFlows_Init");

        // console.log(ds); 
        var flows = ds["Flows"];
        var ens = ds["GenerWorkFlows"];       

        for (var i = 0; i < flows.length; i++) {
            var fs = flows[i];
            fs.open = false;
            fs.children = [];
            fs.Icon = "icon-folder";

            for (var j = 0; j < ens.length; j++) {

                var en = ens[j];

                if (fs.No !== en.FK_Flow) continue;

                if (en.WFState <= 1) continue; //草稿与空白的

                //退回的.
                if (en.WFState === 2) { en.Icon = "icon-clock"; en.IconTitle = "运行中"; }//运行中的.
                if (en.WFState === 3) { en.Icon = "icon-check"; en.IconTitle = "已完成"; }//已完成的.
                if (en.WFState === 5) { en.Icon = "icon-action-undo"; en.IconTitle = "退回"; } //退回的.

                var dateArray = en.TodoEmps.split(",");
                en.TodoEmps = dateArray[1];
                fs.children.push(en);
            }
        }
        this.flows = flows;
        //console.log(this.flows);
        // console.log(this.generWorkFlows);
        //this.bindMenu();
        //  this.initSortArea();
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

function addTab(no, name, url) {
    window.top.vm.openTab(name, url);
}

/**
 * 发起流程
 * @param {any} flowNo
 */
function StartFlow(flowNo) {

    var frmIDOfDict = GetQueryString("FrmID");
    var workIDOfDict = GetQueryString("WorkID");

    //获得方法与流程的对应关系? 根据实体与流程的关系类型不同，调用不同的流程启动方式.
    var method = new Entity("BP.CCBill.Template.Method");
    var i = method.Retrieve("FrmID", frmIDOfDict, "FlowNo", flowNo);
    if (i == 0) {
        alert("流程为:" + flowNo + "的方法已经被删除.");
        return;
    }

    var url = "FlowCenter.htm?WorkID=" + workIDOfDict + "&FrmID=" + frmIDOfDict + "&MenuNo=" + method.No + "&FlowNo=" + flowNo;
    SetHref(url);
    return;


    ////如果是创建流程实例.
    //if (method.MethodModel == "FlowNewEntity") {
    //    //初始化页面
    //    var en = new Entity("BP.CCBill.Template.MethodFlowNewEntity", menuNo);
    //    var workID = en.DoMethodReturnString("CreateWorkID");
    //    var url = "../../MyFlowGener.htm?FK_Flow=" + en.FlowNo + "&FrmID=" + en.FrmID + "&WorkID=" + workID + "&FK_Node=" + en.FlowNo + "01";
    //    SetHref(url);
    //    return;
    //}

    //if (method.MethodModel == "FlowBaseData") {
    //    //初始化页面
    //    var en = new Entity("BP.CCBill.Template.MethodFlowNewEntity", menuNo);
    //    var workID = en.DoMethodReturnString("CreateWorkID");
    //    var url = "../../MyFlowGener.htm?FK_Flow=" + en.FlowNo + "&FrmID=" + en.FrmID + "&WorkID=" + workID + "&FK_Node=" + en.FlowNo + "01";
    //    SetHref(url);
    //    return;
    //}


}


