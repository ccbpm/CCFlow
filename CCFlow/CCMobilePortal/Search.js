var tspan = GetQueryString("TSpan");
var wfstate = GetQueryString("WFState");
//if (wfstate == null)
//    wfstate = 1;

function Search_InitPage() {
    if (tspan == null || tspan == undefined)
        tspan = "3";
    document.getElementById('offCanvasBtn').addEventListener('tap', function () {
        window.location.href = "./SearchCondtion.htm?TSpan=" + tspan + "&WFState=" + wfstate;

    });
    document.getElementById("SearchKey").addEventListener("keypress", function (event) {
        if (event.keyCode == "13") {
            document.activeElement.blur(); //收起虚拟键盘
            Search_InitPage(); //完成搜索事件
            event.preventDefault(); // 阻止默认事件---阻止页面刷新
        }
    });

    //清空查询条件的事件
    mui("#SearchKey")[0].addEventListener('focus', function () {
        mui(".mui-icon-clear")[0].addEventListener('tap', function () {
            Search_InitPage();
        });
    })

    var _html = "";
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobilePortal_SaaS");
    handler.AddPara("SearchKey", $("#SearchKey").val());
    handler.AddPara("TSpan", tspan);
    handler.AddPara("WFState", wfstate);
    handler.AddPara("DTFrom",GetQueryString("DTFrom"));
    handler.AddPara("DTTo", GetQueryString("DTTo"));
    var data = handler.DoMethodReturnString("Search_Init");
    if (data.indexOf("err@") != -1) {
        mui.alert(data);
        console.log(data);
        _html += "<div class='au-prompt au-mg-top'>";
        _html += "<img src='../CCMobile/image/prompt1.png' style='width: 8rem;'>";
        _html += "<span class='au-text-99 au-font-12'>数据查询有误</span>";
        _html += "</div>";
        $("#datalist").html(_html);
        return;

    }

    data = JSON.parse(data);
    $("#EmpWorks_Count").html(data.Todolist_EmpWorks[0].EmpWorks);
    if (data.WF_GenerWorkFlows.length == 0) {
        _html += "<div class='au-prompt au-mg-top'>";
        _html += "<img src='../CCMobile/image/prompt1.png' style='width: 8rem;'>";
        _html += "<span class='au-text-99 au-font-12'>暂无数据</span>";
        _html += "</div>";
        $("#datalist").html(_html);
        return;
    }

    $.each(data.WF_GenerWorkFlows, function (i, t) {
        _html += "<div class='box' id='" + t.WorkID + "_" + t.FK_Flow + "_Running_" + t.FK_Node + "_" + t.FID + "'><div class='box-item'><div class='flex flex-between'>";

        _html += "<div class='mark type-bg'>" + t.FlowName + "</div>";
        _html += "<img src='../CCMobile/image/more.png' class='au-xs-ioc item'></div>";
        _html += "<div class='flex'><span class='dot'></span>";
        _html += "<div class='flex-1 au-m-l-8 au-text-33 au-font-12 au-font-bold overline-1'>";
        _html += t.Title + "</div><div class='list-but'>";

        _html += "</div></div>";
        _html += "<div class='au-font-11 au-text-66 date-m'>";
        _html += "<div>发起人：<span>" + t.StarterName + "</span></div>";
        _html += "<div>发起时间：" + t.RDT + "</div></div></div>";

        _html += "</div></div>";
    });
    $("#datalist").html(_html);
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
      
        $("#" + divId).on("tap", function () {
            var url = "../CCMobile/MyView.htm?FK_Flow=" + fk_flow + "&WorkID=" + oid + "&FK_Node=" + fk_node + "&FID=" + fid + "&MyViewFrom=search" + "&m=" + Math.random();
            window.location.href = filterXSS(url);
        });
       
    });
   

}
