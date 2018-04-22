/*
树形表单工作处理器:
1,里面包含一些必备的数据.
*/
var workNode = null;
var IsReadonly = "0";

function GenerTreeFrm(wn) {
    workNode = wn;
    if (plant == "CCFlow")
        MyFlow = "MyFlow.ashx";
    FlowFormTree_Init();
}
function GetLocalPageName() {
    var strUrl = location.href;
    var arrUrl = strUrl.split("/");
    var strPageUrl = arrUrl[arrUrl.length - 1];
    strPageUrl = strPageUrl.substring(0, strPageUrl.indexOf('?'));
    return strPageUrl;
}

//表单树初始化
function FlowFormTree_Init() {

    //表单参数没有传递过去, 这个需要把url 所有的参数都要传递过去.
    //@代国强.
    //zhoupeng 2018.4.22修改.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("FlowFormTree_Init");

    if (data.indexOf('err@') == 0) {//发送时发生错误
        alert(data);
        return;
    }

    var i = 0;
    var isSelect = false;
    var pushData = eval('(' + data + ')');
    var urlExt = urlExtFrm();

    //加载类别树
    $("#flowFormTree").tree({
        data: pushData,
        iconCls: 'tree-folder',
        collapsed: true,
        lines: true,
        formatter: function (node) {
            if (i == 0) {
                if (node.attributes.NodeType == "form|0" || node.attributes.NodeType == "form|1") {
                    i++;
                    var isEdit = node.attributes.IsEdit;
                    if ((IsCC && IsCC == "1") || IsReadonly == "1") isEdit = "0";
                    var url = "./CCForm/Frm.htm?FK_MapData=" + node.id + "&IsEdit=" + isEdit + "&IsPrint=0" + urlExt;
                    addTab(node.id, node.text, url);
                }
            }
            return node.text;
        },
        onClick: function (node) {
            if (node.attributes.NodeType == "form|0" || node.attributes.NodeType == "form|1") { /*普通表单和必填表单*/
                var isEdit = node.attributes.IsEdit;
                if ((IsCC && IsCC == "1") || IsReadonly == "1") isEdit = "0";
                var url = "./CCForm/Frm.htm?FK_MapData=" + node.id + "&IsEdit=" + isEdit + "&IsPrint=0" + urlExt;
                addTab(node.id, node.text, url);
            } else if (node.attributes.NodeType == "tools|0") {/*工具栏按钮添加选项卡*/
                var url = node.attributes.Url;
                while (url.indexOf('|') >= 0) {
                    url = url.replace('|', '/');
                }
                if (url.indexOf('?') > 0) {
                    url = url + "&FK_MapData=" + node.id + "&" + urlExt;
                }
                else {
                    url = url + "?FK_MapData=" + node.id + "&" + urlExt;
                }
                addTab(node.id, node.text, url);
            } else if (node.attributes.NodeType == "tools|1") {/*工具栏按钮打开新窗体*/
                var url = node.attributes.Url;
                while (url.indexOf('|') >= 0) {
                    url = url.replace('|', '/');
                }
                if (url.indexOf('?') > 0) {
                    url = url + "&FK_MapData=" + node.id + "&" + urlExt;
                }
                else {
                    url = url + "?FK_MapData=" + node.id + "&" + urlExt;
                }
                WinOpenPage("_blank", url, node.text);
            }
        }
    });
    $("#pageloading").hide();
}

$(function () {
    var pageName = GetLocalPageName();
    if (pageName == "MyFlowTreeReadonly.htm") {
        IsReadonly = "1";
        FlowFormTree_Init();
    }
});

function addTab(id, title, url) {
    if ($('#tabs').tabs('exists', title)) {
        $('#tabs').tabs('select', title); //选中并刷新
        var currTab = $('#tabs').tabs('getSelected');
    } else {
        var content = createFrame(url);
        $('#tabs').tabs('add', {
            title: title,
            id: id,
            content: content,
            closable: true
        });
    }
    tabClose();
}

//判断标签页是否存在
function TabFormExists() {
    var currTab = $('#tabs').tabs('getSelected');
    if (currTab)
        return true;
    return false;
}

//修改标题
function ChangTabFormTitle() {
    var tabText = "";
    var p = $(document.getElementById("tabs")).find("li");
    $.each(p, function (i, val) {
        if (val.className.indexOf("tabs-selected") > -1) {
            tabText = $($(val).find("span")[0]).text();
        }
    });

    var lastChar = tabText.substring(tabText.length - 1, tabText.length);
    if (lastChar != "*") {
        $.each(p, function (i, val) {
            if (val.className.indexOf("tabs-selected") > -1) {
                tabText = $($(val).find("span")[0]).text(tabText + '*');
            }
        });
    }
}

//修改标题
function ChangTabFormTitleRemove() {
    var tabText = "";
    var p = $(parent.document.getElementById("tabs")).find("li");
    $.each(p, function (i, val) {
        if (val.className.indexOf("tabs-selected") > -1) {
            tabText = $($(val).find("span")[0]).text();
        }
    });

    var lastChar = tabText.substring(tabText.length - 1, tabText.length);
    if (lastChar == "*") {
        $.each(p, function (i, val) {
            if (val.className.indexOf("tabs-selected") > -1) {
                $($(val).find("span")[0]).text(tabText.substring(0, tabText.length - 1));
            }
        });
    }
}

//tab切换事件
function OnTabChange(scope) {
    //表单内容修改，执行自动保存
    var p = $(document.getElementById("tabs")).find("li");
    var tabText = "";
    $.each(p, function (i, val) {
        if (val.className.indexOf("tabs-selected") > -1) {
            tabText = $($(val).find("span")[0]).text();
        }
    });

    var lastChar = tabText.substring(tabText.length - 1, tabText.length);
    if (scope == "btnsave") {
        var currTab = $('#tabs').tabs('getSelected');
        var currScope = currTab.find('iframe')[0];
        var contentWidow = currScope.contentWindow;
        contentWidow.IsChange = true;
        contentWidow.SaveDtlData();
        $.each(p, function (i, val) {
            if (val.className.indexOf("tabs-selected") > -1) {
                if (lastChar == "*") {
                    $($(val).find("span")[0]).text(tabText.substring(0, tabText.length - 1));
                }
                else {
                    $($(val).find("span")[0]).text(tabText.substring(0, tabText.length));
                }
            }
        });
        return;
    }
    if (lastChar == "*") {
        if (typeof scope == "undefined") {
            var currTab = $('#tabs').tabs('getSelected');
            scope = currTab.find('iframe')[0];
        }
        var contentWidow = scope.contentWindow;
        contentWidow.SaveDtlData();
        $.each(p, function (i, val) {
            if (val.className.indexOf("tabs-selected") > -1) {
                $($(val).find("span")[0]).text(tabText.substring(0, tabText.length - 1));
            }
        });
    }
}

function tabClose() {
    /*双击关闭TAB选项卡*/
    $(".tabs-inner").dblclick(function () {
        var currTab = $('#tabs').tabs('getSelected');
        if (currTab) {
            var currtab_title = currTab.panel('options').title;
            $('#tabs').tabs('close', currtab_title);
        }
    })
    /*为选项卡绑定右键*/
    $(".tabs-inner").bind('contextmenu', function (e) {
        $('#mm').menu('show', {
            left: e.pageX,
            top: e.pageY
        });
        var subtitle = "";
        var currTab = $('#tabs').tabs('getSelected');
        if (currTab) {
            subtitle = currTab.panel('options').title;
        }

        $('#mm').data("currtab", subtitle);
        $('#tabs').tabs('select', subtitle);
        return false;
    });
}

function createFrame(url) {
    var s = '<iframe scrolling="auto" frameborder="0" Onblur="OnTabChange(this)"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
    return s;
}

//打开窗体
function WinOpenPage(target, url, title) {
    window.open(url, target, "left=0,top=0,width=" + (screen.availWidth - 10) + ",height=" + (screen.availHeight - 50) + ",scrollbars,resizable=yes,toolbar=yes,menubar=yes'");
}

//获取参数
var RequestArgs = function () {
    this.WorkID = GetQueryString("WorkID");
    this.FK_Flow = GetQueryString("FK_Flow");
    this.FK_Node = GetQueryString("FK_Node");
    if (this.FK_Node) {
        while (this.FK_Node.substring(0, 1) == '0') this.FK_Node = this.FK_Node.substring(1);
        this.FK_Node = this.FK_Node.replace('#', '');
    }
    this.NodeID = GetQueryString("NodeID");
    this.UserNo = GetQueryString("UserNo");
    this.FID = GetQueryString("FID");
    this.SID = GetQueryString("SID");

    this.PWorkID = GetQueryString("PWorkID");
    this.PFlowNo = GetQueryString("PFlowNo");

    this.DoFunc = GetQueryString("DoFunc");
    this.CFlowNo = GetQueryString("CFlowNo");
    this.WorkIDs = GetQueryString("WorkIDs");

    this.IsLoadData = GetQueryString("IsLoadData");
    this.Paras = GetQueryString("Paras");
    this.AtPara = GetQueryString("AtPara");
    this.IsCC = "0";
    if (this.Paras && this.Paras.indexOf("@IsCC") >= 0) {
        this.IsCC = "1";
        this.IsLoadData = "0";
    }
    if (this.AtPara && this.AtPara.indexOf("@IsCC") >= 0) {
        this.IsCC = "1";
        this.IsLoadData = "0";
    }
}

//传参
var urlExtFrm = function () {
    var extUrl = "";
    var args = new RequestArgs();
    if (args.WorkID != "")
        extUrl += "&WorkID=" + args.WorkID;
    if (args.FK_Flow != "")
        extUrl += "&FK_Flow=" + args.FK_Flow;
    if (args.FK_Node != "")
        extUrl += "&FK_Node=" + args.FK_Node;
    if (args.NodeID != "")
        extUrl += "&NodeID=" + args.NodeID;
    if (args.UserNo != "")
        extUrl += "&UserNo=" + args.UserNo;
    if (args.FID != "")
        extUrl += "&FID=" + args.FID;
    if (args.SID != "")
        extUrl += "&SID=" + args.SID;
    if (args.PWorkID != "")
        extUrl += "&PWorkID=" + args.PWorkID;
    if (args.PFlowNo != "")
        extUrl += "&PFlowNo=" + args.PFlowNo;
    if (args.IsLoadData != "") {
        extUrl += "&IsLoadData=" + args.IsLoadData;
    }
    extUrl += "&IsReadonly=" + IsReadonly;

    //获取其他参数
    var sHref = window.location.href;
    var args = sHref.split("?");
    var retval = "";
    if (args[0] != sHref) /*参数不为空*/
    {
        var str = args[1];
        args = str.split("&");
        for (var i = 0; i < args.length; i++) {
            str = args[i];
            var arg = str.split("=");
            if (arg.length <= 1) continue;
            //不包含就添加
            if (arg[0] == "DoType")
                continue;
            if (extUrl.indexOf(arg[0]) == -1) {
                extUrl += "&" + arg[0] + "=" + arg[1];
            }
        }
    }

    return extUrl;
}

//公共方法
function AjaxService(param, callback, scope, levPath) {
    $.ajax({
        type: "GET", //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: MyFlow, //要访问的后台地址
        data: param, //要发送的数据
        async: true,
        cache: false,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            callback(XMLHttpRequest);
        },
        success: function (msg) {//msg为返回的数据，在这里做数据绑定
            var data = msg;
            callback(data, scope);
        }
    });
}