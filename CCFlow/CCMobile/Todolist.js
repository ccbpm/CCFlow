function Todolist_InitPage () {
    var _todohtml = "";
    var _runhtml = "";
    var _cchtml = "";
    var _completehtml = "";
    //获取待办、在途、已完成数据
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobilePortal_SaaS");
    var data = handler.DoMethodReturnString("GetGenerWorks");
    data = JSON.parse(data);
    //待办数据
    var Todolist = data.Todolist;
    //在途数据
    var Running = data.Running;
    //已完成数据
    var Complete = data.Complte;

    //抄送数据
    var CC = data.CC;

    //待办总数
    $("#Todolist_EmpWorks").html(Todolist.length);
    //在途总数
    $("#Todolist_Runnings").html(Running.length);
    //抄送总数
    $("#Todolist_CCs").html(CC.length);
    //已完成总数
    $("#Todolist_Completes").html(Complete.length);

    $("#EmpWorks_Count").html(Todolist.length);

    if (Todolist.length <= 0) {
        _todohtml += "<div class='au-prompt au-mg-top'>";
        _todohtml += "<img src='" + basePath +"/CCMobile/image/prompt1.png' style='width: 8rem;'>";
        _todohtml += "<span class='au-text-99 au-font-12'>暂无数据</span>";
        _todohtml += "</div>";
        $("#item_todolist").html(_todohtml);
    }
    else {
        //填充待办信息
        $.each(Todolist, function (i, t) {
            _todohtml += "<div class='box' id='" + t.WorkID + "_" + t.FK_Flow + "_Todolist_" + t.FK_Node + "_" + t.FID + "'><div class='box-item'><div class='flex flex-between'>";
            _todohtml += "<div class='mark type-bg'>" + t.FlowName+"</div>";
            _todohtml += "<img src='" + basePath +"/CCMobile/image/more.png' class='au-xs-ioc item'></div>";
            _todohtml += "<div class='flex'><span class='dot'></span>";
            _todohtml += "<div class='flex-1 au-m-l-8 au-text-33 au-font-12 au-font-bold overline-1'>";
            _todohtml += t.Title + "</div><div class='list-but'>";

            if (t.WFState == 5)
                _todohtml += "<div class='au-but but-color2'>被退回</div>";

            _todohtml += "</div></div>";
            _todohtml += "<div class='au-font-11 au-text-66 date-m'>";
            _todohtml += "<div>发起人：<span>" + t.StarterName + "</span></div>";
            _todohtml += "<div>发起时间：" + t.RDT + "</div>";
            _todohtml += "<div>当前节点：<span>" + t.NodeName + "</span></div></div></div>";

            if (t.WFState == 5) {
                _todohtml += "<div class='item_wrap'><div class='item_wrap_box'>";
                _todohtml += "<div><span class='mui-icon mui-icon-info'></span>";
                _todohtml += "<div>流程退回提示</div></div><div>";

                var h = new HttpHandler("BP.WF.HttpHandler.CCMobile");
                h.AddPara("WorkID", t.WorkID);
                h.AddPara("FK_Node", t.FK_Node);
                var returnData = h.DoMethodReturnString("DB_GenerReturnWorks");
                returnData = cceval('(' + returnData + ')');

                _todohtml += "<div>来自节点：</div><div>" + returnData[0].ReturnNodeName + "</div>";
                _todohtml += "</div>";
                _todohtml += "<div><div>退回人：</div><div>" + returnData[0].Returner + "</div></div>";
                _todohtml += "<div><div>退回时间：</div><div>" + returnData[0].RDT + "</div></div>";
                _todohtml += "<div><div>原因：</div><div>" + returnData[0].BeiZhu + "</div></div></div></div>";
            }

            _todohtml += "</div></div>";
        });
        $("#item_todolistData").html(_todohtml);
    }
    if (Running.length <= 0) {
        _runhtml += "<div class='au-prompt au-mg-top'>";
        _runhtml += "<img src='" + basePath +"/CCMobile/image/prompt1.png' style='width: 8rem;'>";
        _runhtml += "<span class='au-text-99 au-font-12'>暂无数据</span>";
        _runhtml += "</div>";
        $("#item_runningData").html(_runhtml);
    }
    else {
        //填充在途信息
        $.each(Running, function (i, t) {
            _runhtml += "<div class='box' id='" + t.WorkID + "_" + t.FK_Flow + "_Running_" + t.FK_Node + "_" + t.FID + "'><div class='box-item'><div class='flex flex-between'>";
            
            _runhtml += "<div class='mark type-bg'>" + t.FlowName +"</div>";
            _runhtml += "<img src='" + basePath +"/CCMobile/image/more.png' class='au-xs-ioc item'></div>";
            _runhtml += "<div class='flex'><span class='dot'></span>";
            _runhtml += "<div class='flex-1 au-m-l-8 au-text-33 au-font-12 au-font-bold overline-1'>";
            _runhtml += t.Title + "</div><div class='list-but'>";

            _runhtml += "</div></div>";
            _runhtml += "<div class='au-font-11 au-text-66 date-m'>";
            _runhtml += "<div>发起人：<span>" + t.StarterName + "</span></div>";
            _runhtml += "<div>发起时间：" + t.RDT + "</div></div></div>";

            _runhtml += "</div></div>";
        });
        $("#item_runningData").html(_runhtml);
    }

    if (Complete.length <= 0) {
        _completehtml += "<div class='au-prompt au-mg-top'>";
        _completehtml += "<img src='" + basePath +"/CCMobile/image/prompt1.png' style='width: 8rem;'>";
        _completehtml += "<span class='au-text-99 au-font-12'>暂无数据</span>";
        _completehtml += "</div>";
        $("#item_completeData").html(_completehtml);
    }
    else {
        //填充在途信息
        $.each(Complete, function (i, t) {
            _completehtml += "<div class='box' id='" + t.WorkID + "_" + t.FK_Flow + "_Running_" + t.FK_Node + "_" + t.FID + "'><div class='box-item'><div class='flex flex-between'>";

            _completehtml += "<div class='mark type-bg'>" + t.FlowName + "</div>";
            _completehtml += "<img src='" + basePath +"/CCMobile/image/more.png' class='au-xs-ioc item'></div>";
            _completehtml += "<div class='flex'><span class='dot'></span>";
            _completehtml += "<div class='flex-1 au-m-l-8 au-text-33 au-font-12 au-font-bold overline-1'>";
            _completehtml += t.Title + "</div><div class='list-but'>";

            _completehtml += "</div></div>";
            _completehtml += "<div class='au-font-11 au-text-66 date-m'>";
            _completehtml += "<div>发起人：<span>" + t.StarterName + "</span></div>";
            _completehtml += "<div>发起时间：" + t.RDT + "</div></div></div>";

            _completehtml += "</div></div>";
        });
        $("#item_completeData").html(_completehtml);
    }

    if (CC.length <= 0) {
        _cchtml += "<div class='au-prompt au-mg-top'>";
        _cchtml += "<img src='" + basePath +"/CCMobile/image/prompt1.png' style='width: 8rem;'>";
        _cchtml += "<span class='au-text-99 au-font-12'>暂无数据</span>";
        _cchtml += "</div>";
        $("#item_ccData").html(_cchtml);
    }
    else {
        //填充抄送信息
        $.each(CC, function (i, t) {
            _cchtml += "<div class='box' id='" + t.WorkID + "_" + t.FK_Flow + "_CC_" + t.FK_Node + "_" + t.FID + "'><div class='box-item'><div class='flex flex-between'>";
            _cchtml += "<div class='mark type-bg'>" + t.FlowName +"</div>";
            _cchtml += "<img src='" + basePath +"/CCMobile/image/more.png' class='au-xs-ioc item'></div>";
            _cchtml += "<div class='flex'><span class='dot'></span>";
            _cchtml += "<div class='flex-1 au-m-l-8 au-text-33 au-font-12 au-font-bold overline-1'>";
            _cchtml += t.Title + "</div><div class='list-but'>";
            _cchtml += "</div></div>";
            _cchtml += "<div class='au-font-11 au-text-66 date-m'>";
            _cchtml += "<div>抄送人：<span>" + t.Rec + "</span></div>";
            _cchtml += "<div>抄送时间：" + t.RDT + "</div>";
            if (t.Sta==0)
                _cchtml += "<div>抄送状态：未读</div></div></div>";
            else
                _cchtml += "<div>抄送状态：已读</div></div></div>";
            _cchtml += "</div></div>";
        });
        $("#item_ccData").html(_cchtml);
    }

  
    //绑定点击事件
    var boxs = $(".box");
    $.each(boxs, function (i, box) {
        var divId = $(box).attr("id");
        //将ID值进行分解
        var vals = divId.split('_');
        //WorkID
        var oid = vals[0];
        //流程编号
        var fk_flow = vals[1];
        //流程处理状态
        var type = vals[2];
        //节点编号
        var fk_node = vals[3];
        //fid
        var fid = vals[4];
        if (type == "Todolist") {
            $("#" + divId).on("tap", function () {
                var url = basePath+"/CCMobile/MyFlow.htm?FK_Flow=" + fk_flow + "&WorkID=" + oid + "&FID=" + fid + "&MyFlowFrom=todolist" + "&m=" + Math.random();
                SetHref(url);
            });
        }
        else{
            $("#" + divId).on("tap", function () {
                var url = basePath+"/CCMobile/MyView.htm?FK_Flow=" + fk_flow + "&WorkID=" + oid + "&FK_Node=" + fk_node + "&FID=" + fid + "&MyViewFrom=todolist" + "&m=" + Math.random();
                SetHref(url);
            });
        } 
    });
}