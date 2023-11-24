function Running_InitPage(doType) {

    var _drafhtml = "";
    var _runhtml = "";
    var _comhtml = "";
    //获取我发起的流程的草稿、在途、已完成数据
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobilePortal_SaaS");
    var data = handler.DoMethodReturnString("GetMyStartGenerWorks");
    var pushData = cceval('(' + data + ')');
    //草稿数据
    var Draflist = pushData.Draflist;
    //处理中数据
    var Running = pushData.Running;
    //已完成数据
    var Complete = pushData.Complete;

    if (Running.length <= 0) {
        _runhtml += "<div class='au-prompt au-mg-top'>";
        _runhtml += "<img src='"+basePath+"/CCMobile/image/prompt1.png' style='width: 8rem;'>";
        _runhtml += "<span class='au-text-99 au-font-12'>暂无数据</span>";
        _runhtml += "</div>";
        $("#item_my_runningData").html(_runhtml);
    }
    else {
        //填充在途信息
        $.each(Running, function (i, t) {
            _runhtml += "<div class='box' id='" + t.WorkID + "_" + t.FK_Flow + "_Running_" + t.FK_Node + "_" + t.FID + "'><div class='box-item'><div class='flex flex-between'>";
            _runhtml += "<div class='mark type-bg'>"+t.FlowName+"</div>";
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
        $("#item_my_runningData").html(_runhtml);
    }

    if (Complete.length <= 0) {
        _comhtml += "<div class='au-prompt au-mg-top'>";
        _comhtml += "<img src='" + basePath +"/CCMobile/image/prompt1.png' style='width: 8rem;'>";
        _comhtml += "<span class='au-text-99 au-font-12'>暂无数据</span>";
        _comhtml += "</div>";
        $("#item_my_completeData").html(_comhtml);
    }
    else {
        //填充已完成信息
        $.each(Complete, function (i, t) {
            _comhtml += "<div class='box' id='" + t.WorkID + "_" + t.FK_Flow + "_Complete_" + t.FK_Node + "_" + t.FID + "'><div class='box-item'><div class='flex flex-between'>";
            _comhtml += "<div class='mark type-bg'>" + t.FlowName +"</div>";
            _comhtml += "<img src='" + basePath +"/CCMobile/image/more.png' class='au-xs-ioc item'></div>";
            _comhtml += "<div class='flex'><span class='dot'></span>";
            _comhtml += "<div class='flex-1 au-m-l-8 au-text-33 au-font-12 au-font-bold overline-1'>";
            _comhtml += t.Title + "</div><div class='list-but'>";


            _comhtml += "</div></div>";
            _comhtml += "<div class='au-font-11 au-text-66 date-m'>";
            _comhtml += "<div>发起人：<span>" + t.StarterName + "</span></div>";
            _comhtml += "<div>发起时间：" + t.RDT + "</div></div></div>";

            _comhtml += "</div></div>";
        });
        $("#item_my_completeData").html(_comhtml);
    }

    if (Draflist.length <= 0) {
        _drafhtml += "<div class='au-prompt au-mg-top'>";
        _drafhtml += "<img src='" + basePath +"/CCMobile/image/prompt1.png' style='width: 8rem;'>";
        _drafhtml += "<span class='au-text-99 au-font-12'>暂无数据</span>";
        _drafhtml += "</div>";
        $("#item_my_draflistData").html(_drafhtml);
    }
    else {
        //填充草稿信息
        $.each(Draflist, function (i, t) {
            _drafhtml += "<div class='box' id='" + t.WorkID + "_" + t.FK_Flow + "_Draf_" + t.FK_Node + "_" + t.FID + "'><div class='box-item'><div class='flex flex-between'>";
            _drafhtml += "<div class='mark type-bg'>" + t.FlowName +"</div>";
            _drafhtml += "<img src='" + basePath +"/CCMobile/image/more.png' class='au-xs-ioc item'></div>";
            _drafhtml += "<div class='flex'><span class='dot'></span>";
            _drafhtml += "<div class='flex-1 au-m-l-8 au-text-33 au-font-12 au-font-bold overline-1'>";
            _drafhtml += t.Title + "</div><div class='list-but'>";
            _drafhtml += "</div></div>";
            _drafhtml += "<div class='au-font-11 au-text-66 date-m'>";
           // _drafhtml += "<div>发起人：<span>" + t.StarterName + "</span></div>";
            _drafhtml += "<div>申请时间：" + t.RDT + "</div></div></div>";
            _drafhtml += "</div></div>";
        });
        $("#item_my_draflistData").html(_drafhtml);
    }


   
    //如果是在途
    if (doType == "Running") {
        //设置选中
        $("#myRunning").addClass("mui-active");
        //显示在途相关item
        $("#item_my_running").addClass("mui-active");
        //隐藏其他内容
        $("#draflist").removeClass("mui-active");
        $("#item_my_draflist").removeClass("mui-active")
        $("#myComplete").removeClass("mui-active");
        $("#item_my_complete").removeClass("mui-active");
    }//如果是已完成
    else if (doType == "Complete") {
        //隐藏待办、在途
        $("#myRunning").removeClass("mui-active");
        $("#item_my_running").removeClass("mui-active");
        $("#draflist").removeClass("mui-active");
        $("#item_my_draflist").removeClass("mui-active");
        //显示已完成
        $("#myComplete").addClass("mui-active");
        //显示已完成内容
        $("#item_my_complete").addClass("mui-active");
    }
    else {//如果是待办
        $("#myRunning").removeClass("mui-active");
        $("#item_my_running").removeClass("mui-active");
        //显示草稿
        $("#draflist").addClass("mui-active");
        //显示草稿内容
        $("#item_my_draflist").addClass("mui-active");
        $("#myComplete").removeClass("mui-active");
        $("#item_my_complete").removeClass("mui-active");
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
        if (type == "Draf") {
            $("#" + divId).on("tap", function () {
                var url = basePath+"/CCMobile/MyFlow.htm?FK_Flow=" + fk_flow + "&WorkID=" + oid + "&FK_Node=" + fk_node  + "&FID=" + fid + "&MyFlowFrom=Draf" + "&m=" + Math.random();
                SetHref(url);
            });
        }
        else {
            $("#" + divId).on("tap", function () {
                var url = basePath+"/CCMobile/MyView.htm?FK_Flow=" + fk_flow + "&WorkID=" + oid + "&FK_Node=" + fk_node + "&FID=" + fid + "&MyViewFrom=" + type + "&IsReadonly=1&m=" + Math.random();
                SetHref(url);
            });
        }
    });
}