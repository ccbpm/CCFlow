var DefuaultUrl = "Base/DataService.aspx";

ccflow = {}
Application = {}

DataFactory = function () {
    this.data = new ccflow.Data(DefuaultUrl);
}
//配置文件
ccflow.config = {
    IsWinOpenStartWork: 1, //是否启动工作时打开新窗口 0=在本窗口打开,1=在新窗口打开, 2=打开流程一户式窗口
    IsWinOpenEmpWorks: "TRUE"//是否打开待办工作时打开新窗口
};

jQuery(function ($) {
    Application = new DataFactory();

    //系统初始
    Application.data.init(function (json) {
        if (json != "") {
            var configData = eval('(' + json + ')');
            ccflow.config.IsWinOpenStartWork = configData.config[0].IsWinOpenStartWork;
            ccflow.config.IsWinOpenEmpWorks = configData.config[0].IsWinOpenEmpWorks;
        }
    }, this);
});

ccflow.Data = function (url) {
    this.url = url;
    //获取流程列表
    this.getStartFlow = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "startflow" };
        queryData(tUrl, params, callback, scope);
    }

    //获取流程列表
    this.getStartFlowTree = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "startflowTree" };
        queryData(tUrl, params, callback, scope);
    }
    //获取待办列表
    this.getEmpWorks = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "getempworks" };
        queryData(tUrl, params, callback, scope);
    }
    //获取抄送列表
    this.getCCFlowList = function (ccSta, callback, scope) {
        var tUrl = this.url;
        var params = { method: "getccflowlist", ccSta: ccSta };
        queryData(tUrl, params, callback, scope);
    }
    //获取挂起流程
    this.getHungUpList = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "gethunguplist" };
        queryData(tUrl, params, callback, scope);
    }
    //获取在途列表
    this.getRunning = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "Running" };
        queryData(tUrl, params, callback, scope);
    }
    //撤销发送
    this.unSend = function (fkFlow, workId, callback, scope) {
        var tUrl = this.url;
        var params = { method: "unsend", FK_Flow: fkFlow, WorkID: workId };
        queryData(tUrl, params, callback, scope);
    }
    //工作流查询
    this.flowSearch = function (callback, scope) {
        var tUrl = this.url;
        

        var params = { method: "flowsearch" };
        queryData(tUrl, params, callback, scope);
    }
    //获取通讯录信息
    this.getEmps = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "getemps" };
        queryData(tUrl, params, callback, scope);
    }
    //取回审批
    this.getTask = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "gettask" };
        queryData(tUrl, params, callback, scope);
    }
    //关键字查询
    this.keySearch = function (checkBox, content, queryType, callback, scope) {
        var tUrl = this.url;
        var params = { method: "keySearch", checkBox: checkBox, content: encodeURI(content), queryType: queryType };
        queryData(tUrl, params, callback, scope);
    }
    //获取配置参数
    this.init = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "getconfigparm" };
        queryData(tUrl, params, callback, scope);
    }
    //获取待办、抄送、挂起数量
    this.getEmpWorkCounts = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "getempworkcounts" };
        queryData(tUrl, params, callback, scope);
    }
    //获取历史发起
    this.getHistoryStartFlow = function (flowNo, callback, scope) {
        var tUrl = this.url;
        var params = { method: "historystartflow", FK_FLOW: flowNo };
        queryData(tUrl, params, callback, scope);
    }
    //弹出 系统消息 窗口  2013.05.23 H
    this.popAlert = function (type, callback, scope) {
        var tUrl = this.url;
        var params = { method: "popAlert", type: type };
        queryData(tUrl, params, callback, scope);
    }

    //修改 数据 状态   2013.05.23 H
    this.upMsgSta = function (my_PK, callback, scope) {
        var tUrl = this.url;
        var params = { method: "upMsgSta", myPK: my_PK };
        queryData(tUrl, params, callback, scope);
    }
    //获取 详细数据   2013.05.23 H
    this.getDetailSms = function (my_PK, callback, scope) {
        var tUrl = this.url;
        var params = { method: "getDetailSms", myPK: my_PK };
        queryData(tUrl, params, callback, scope);
    }
    //加载菜单 2013.07.23 H 
    this.getMenu = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "getmenu" };
        queryData(tUrl, params, callback, scope);
    }
    //获取所有的历史流程
    this.getStoryHistory = function (fk_flow, callback, scope) {
        var tUrl = this.url;
        var params = { method: "getstoryHistory", FK_Flow: fk_flow };
        queryData(tUrl, params, callback, scope);
    }
    //新建月计划
    this.createMonthPlan = function (callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "createmonthplan"
        };
        queryData(tUrl, params, callback, scope);
    }
    //月计划汇总
    this.monthPlanCollect = function (callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "monthplancollect"
        };
        queryData(tUrl, params, callback, scope);
    }
    //业务流程操作
    this.workFlowManage = function (doWhat, flowIdAndWorkId, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "workflowmanage",
            doWhat: doWhat,
            flowIdAndWorkId: flowIdAndWorkId
        };
        queryData(tUrl, params, callback, scope);
    }
    //加载流程树 H 
    this.treedata = function (callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "treeData"
        };
        queryData(tUrl, params, callback, scope);
    }
    //创建空流程
    this.createEmptyCase = function (flowId, title, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "createemptycase",
            flowId: flowId,
            title: encodeURI(title)
        };
        queryData(tUrl, params, callback, scope);
    }
    //公共方法
    function queryData(url, param, callback, scope, method, showErrMsg) {
        if (!method) method = 'GET';
        $.ajax({
            type: method, //使用GET或POST方法访问后台
            dataType: "text", //返回json格式的数据
            contentType: "application/json; charset=utf-8",
            url: url, //要访问的后台地址
            data: param, //要发送的数据
            async: true,
            cache: false,
            complete: function () { $("#load").hide(); }, //AJAX请求完成时隐藏loading提示
            error: function (XMLHttpRequest, errorThrown) {
                callback(XMLHttpRequest);
            },
            success: function (msg) {//msg为返回的数据，在这里做数据绑定
                var data = msg;
                callback(data, scope);
            }
        });
    }
}