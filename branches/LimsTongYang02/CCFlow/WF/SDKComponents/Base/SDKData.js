var DefuaultUrl = "/WF/SDKComponents/Base/SDKBase.aspx";

ccflowSDK = {}
Application = {}

DataFactory = function () {
    this.data = new ccflowSDK.Data(DefuaultUrl);
    this.common = new ccflowSDK.common();
}

jQuery(function ($) {
    Application = new DataFactory();
});

//公共方法
ccflowSDK.common = function () {
    //sArgName表示要获取哪个参数的值
    this.getArgsFromHref = function (sArgName) {
        var sHref = window.location.href;
        var args = sHref.split("?");
        var retval = "";
        if (args[0] == sHref) /*参数为空*/
        {
            return retval; /*无需做任何处理*/
        }
        var str = args[1];
        args = str.split("&");
        for (var i = 0; i < args.length; i++) {
            str = args[i];
            var arg = str.split("=");
            if (arg.length <= 1) continue;
            if (arg[0] == sArgName) retval = arg[1];
        }
        return retval;
    }
}
//数据访问
ccflowSDK.Data = function (url) {
    this.url = url;
    //获取工具栏
    this.getAppToolBar = function (nodeId, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getapptoolbar",
            nodeId: nodeId
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取左边表单树
    this.getFlowFormTree = function (flowId, nodeId, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getflowformtree",
            flowId: flowId,
            nodeId: nodeId
        };
        queryData(tUrl, params, callback, scope);
    }
    //接收人选择
    this.checkAccepter = function (FK_Node, WorkID, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "checkaccepter",
            FK_Node: FK_Node,
            WorkID: WorkID
        };
        queryData(tUrl, params, callback, scope);
    }
    //设置抄送已阅
    this.ReadCC = function (FK_Node, WorkID, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "readCC",
            FK_Node: FK_Node,
            WorkID: WorkID
        };
        queryData(tUrl, params, callback, scope);
    }
    //执行发送
    this.sendCase = function (FK_Flow, FK_Node, WorkID, DoFunc, CFlowNo, WorkIDs, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "sendcase",
            FK_Flow: FK_Flow,
            FK_Node: FK_Node,
            WorkID: WorkID,
            DoFunc: DoFunc,
            CFlowNo: CFlowNo,
            WorkIDs: WorkIDs
        };
        queryData(tUrl, params, callback, scope);
    }
    //执行发送到指定节点
    this.sendCaseToNode = function (FK_Flow, FK_Node, WorkID, DoFunc, CFlowNo, WorkIDs, ToNode, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "sendcasetonode",
            FK_Flow: FK_Flow,
            FK_Node: FK_Node,
            WorkID: WorkID,
            DoFunc: DoFunc,
            CFlowNo: CFlowNo,
            WorkIDs: WorkIDs,
            ToNode: ToNode
        };
        queryData(tUrl, params, callback, scope);
    }
    //撤销发送
    this.unSendCase = function (flowId, workId, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "unsendcase",
            FK_Flow: flowId,
            WorkID: workId
        };
        queryData(tUrl, params, callback, scope);
    }
    //删除流程
    this.delcase = function (flowId, nodeId, workId, fId, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "delcase",
            flowId: flowId,
            nodeId: nodeId,
            workId: workId,
            fId: fId
        };
        queryData(tUrl, params, callback, scope);
    }
    //流程签名
    this.signcase = function (flowId, nodeId, workId, fId, yj, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "signcase",
            flowId: flowId,
            nodeId: nodeId,
            workId: workId,
            fId: fId,
            yj: yj
        };
        queryData(tUrl, params, callback, scope);
    }
    //结束流程
    this.endCase = function (flowId, workId, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "endcase",
            flowId: flowId,
            workId: workId
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
            async: false,
            cache: false,
            complete: function () { }, //AJAX请求完成时隐藏loading提示
            error: function (XMLHttpRequest, errorThrown) {
                callback(XMLHttpRequest);
                alert("error");
            },
            success: function (msg) {//msg为返回的数据，在这里做数据绑定
                var data = msg;
                callback(data, scope);
            }
        });
    }

    //公共方法
    function queryPostData(url, param, callback, scope) {
        $.post(url, param, callback);
    }
}