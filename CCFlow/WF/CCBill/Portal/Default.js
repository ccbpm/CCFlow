var osModel = 1;
var runOnPlant = "BP";
var initData = null;
var runModelType = 0; // 0=完整版 1=简洁版.
$(function () {

    //判断运行的类型.
    var url = GetHrefUrl();
    if (url.indexOf('Simple') > 1)
        runModelType = 1;

    if (url.indexOf('Frm') > 1)
        runModelType = 2;

    //定义等待界面的位置
    $(".mymaskContainer").offset({ left: ($(document).innerWidth() - 120) / 2, top: ($(document).innerHeight() - 50) / 2 });
    $(".mymask").show();

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
    var data = handler.DoMethodReturnString("Default_Init");

    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    if (data.indexOf('url@') == 0) {
        var url = data.replace('url@', '');
        SetHref(url);
        return;
    }


    data = JSON.parse(data);

    initData = data;

    var msg = data.Msg;
    var content = null;

    var wel = document.getElementById("welcome");

    //if (runModelType == 0)
    //    xadmin.add_tab('222','../../../DataUser/AppCoder/FlowDesignerWelcome.htm');
    //else
    //    xadmin.add_tab('222', '../../../DataUser/AppCoder/FlowDesignerWelcomeSimple.htm');

    if (runModelType == 0)
        wel.src = '../../../DataUser/AppCoder/FlowDesignerWelcome.htm';
    else
        wel.src = '../../../DataUser/AppCoder/FlowDesignerWelcomeSimple.htm';

    //$('#tabs').tabs('add', {
    //    title: '首页',
    //    id: 'WelCome',
    //    content: content,
    //    iconCls: '',
    //    closable: false
    //});

    if (data.CustomerNo == "TianYe" || data.CustomerNo == "TianYe1") {
        $("#Login2App").html("");
    }
});

function closeTab(title) {
    $('#tabs').tabs('close', title);
}
function addTab(id, title, url, iconCls, refresh) {
    //此处为适应原有GPM中的编辑系统菜单页面的打开新tab，那个只传了2个参数addTab(title,url)，edited by liuxc,2015-11-05
    if (arguments.length < 3) {
        url = title;
        title = id;
        id = Math.random().toString();

        url = '../../../GPM/' + url;

    }

    if ($('#tabs').tabs('exists', title)) {
        $('#tabs').tabs('select', title);
        var currTab = $('#tabs').tabs('getSelected').panel('options');

        if (currTab.id != id) {
            $('#tabs').tabs('update', {
                tab: $('#tabs').tabs('getSelected'),
                options: {
                    id: id,
                    content: createFrame(url)
                }
            });
        }
        //此处暂时屏掉刷新当前已经打开的页，发现如果切换非当前的已经打开的流程设计图时，会导致IE崩溃
        //                else {
        //                    var iwin = $("#tabs div[id='" + id + "']").find("iframe")[0].contentWindow;
        //                    if (iwin.location.href != url) {
        //                        iwin.location.href = url;
        //                    }
        //                    else {
        //                        iwin.location.reload();  //此处重新刷新当前页面
        //                    }
        //                }
    } else {
        var content = createFrame(url);
        $('#tabs').tabs('add', {
            title: title,
            id: id,
            content: content,
            iconCls: iconCls,
            closable: true
        });
    }
    tabClose();
}
//判断标签页是否存在
function TabFormExists() {
    var currTab = $('#tabs').tabs('getSelected');
    if (currTab) return true;

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
                tabText = $($(val).find("span")[0]).text(tabText + ' *');
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

function createFrame(url) {
    var s = '<iframe scrolling="auto" frameborder="0" Onblur="OnTabChange(this)"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
    return s;
}

//tab切换事件
function OnTabChange(scope) {
    var p = $(parent.document.getElementById("tabs")).find("li");
    var tabText = "";
    $.each(p, function (i, val) {
        if (val.className == "tabs-selected") {
            tabText = $($(val).find("span")[0]).text();
        }
    });
    var lastChar = tabText.substring(tabText.length - 1, tabText.length);
    if (lastChar == "*") {
        var contentWidow = scope.contentWindow;

        if (contentWidow && contentWidow.SaveDtlData) {
            contentWidow.SaveDtlData();
        }

        $.each(p, function (i, val) {
            if (val.className == "tabs-selected") {
                $($(val).find("span")[0]).text(tabText.substring(0, tabText.length - 1));
            }
        });
    }
}

var arrayCloseObj = new Array();
//标签关闭前事件
function EventListener_TabClose(title, index) {
    var disTab = $('#tabs').tabs('getTab', index);
    var curTab_Id = disTab.panel('options').id;
    //curTab_Id = Number(curTab_Id);
    //curTab_Id = String(curTab_Id);
    var tabs = $('#tabs').tabs('tabs');
    for (var i = 0, j = tabs.length; i < j; i++) {
        var othTab_Id = tabs[i].panel('options').id;
        var othTab_Title = tabs[i].panel('options').title;
        if (othTab_Id.length > curTab_Id.length && othTab_Id.substring(0, curTab_Id.length) == curTab_Id) {
            arrayCloseObj.push(othTab_Title);
        }
    }
}

function EventListener_TabCloseed() {
    for (var k = 0; k < arrayCloseObj.length; k++) {
        $('#tabs').tabs('close', arrayCloseObj[k]);
    }
    var tabs = $('#tabs').tabs('tabs');
    if (tabs.length == 0) {
        addTab("welcome", "首页", "Welcome.htm", "");
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
    arrayCloseObj.length = 0;
    tabCloseEven();
}
// 获取当前选中的tab
function currentSelection() {
    var currentTab = $('#tabs').tabs('getSelected').panel('options');
    return currentTab;
}
//绑定右键菜单事件
function tabCloseEven() {
    //刷新
    $('#mm-tabupdate').click(function () {
        var currTab = $('#tabs').tabs('getSelected');
        var url = $(currTab.panel('options').content).attr('src');
        if (url != undefined) {
            $('#tabs').tabs('update', {
                tab: currTab,
                options: {
                    content: createFrame(url)
                }
            })
        }
    })
    //关闭当前
    $('#mm-tabclose').click(function () {
        var currtab_title = $('#mm').data("currtab");
        $('#tabs').tabs('close', currtab_title);
    })
    //全部关闭
    $('#mm-tabcloseall').click(function () {
        $('.tabs-inner span').each(function (i, n) {
            var t = $(n).text();
            if (t != '首页') {
                $('#tabs').tabs('close', t);
            }
        });
    });
    //关闭除当前之外的TAB
    $('#mm-tabcloseother').click(function () {
        var prevall = $('.tabs-selected').prevAll();
        var nextall = $('.tabs-selected').nextAll();
        if (prevall.length > 0) {
            prevall.each(function (i, n) {
                var t = $('a:eq(0) span', $(n)).text();
                if (t != '首页') {
                    $('#tabs').tabs('close', t);
                }
            });
        }
        if (nextall.length > 0) {
            nextall.each(function (i, n) {
                var t = $('a:eq(0) span', $(n)).text();
                if (t != '首页') {
                    $('#tabs').tabs('close', t);
                }
            });
        }
        return false;
    });
    //关闭当前右侧的TAB
    $('#mm-tabcloseright').click(function () {
        var nextall = $('.tabs-selected').nextAll();
        if (nextall.length == 0) {
            return false;
        }
        nextall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            $('#tabs').tabs('close', t);
        });
        return false;
    });
    //关闭当前左侧的TAB
    $('#mm-tabcloseleft').click(function () {
        var prevall = $('.tabs-selected').prevAll();
        if (prevall.length == 0) {
            return false;
        }
        prevall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            $('#tabs').tabs('close', t);
        });
        return false;
    });
}

//登录,判断为天业BPM，转向到天业新版界面
function Login2App() {

    if (initData.RunOnPlant == "jeesite") {
        SetHref(getContextPath() + "/a/logout");
        return;
    }

    //表单引擎.
    var url = GetHrefUrl();
    if (url.indexOf("DefaultFrm.htm") != 0) {
        SetHref("../../CCBill/Portal/Login.htm?DoType=Logout");
        return;
    }

    //简洁版.
    if (url.indexOf("DefaultSimple.htm") != 0) {
        SetHref("../../CCBill/Portal/Login.htm?DoType=Logout");
        return;
    }

    //完整版.
    SetHref("../../AppClassic/Login.htm?DoType=Logout");
    return;
}

//退出
function LoginOut() {

    if (confirm("提示确定需要退出？") == false)
        return;
    SetHref("Login.htm?DoType=Logout");
}

var FLOW_TREE = "flowTree";
var FORM_TREE = "formTree";
var ORG_TREE = "OrgTree";

//设计器加载完毕隐藏等待页面
function DesignerLoaded() {
    $(".mymask").hide();
}

//右键打开流程
function showFlow() {
    var node = $('#flowTree').tree('getSelected');
    if (!node || node.attributes.ISPARENT != '0') return;
    OpenFlowToCanvas(node, node.id, node.text);
}

//重新装载流程图
function RefreshFlowJson() {

    var node = $('#flowTree').tree('getSelected');
    if (!node || node.attributes.ISPARENT != '0')
        return;

    //首先关闭tab
    closeTab(node.text);

    $(".mymask").show();

    addTab(node.id, node.text, "../CCBPMDesigner/Designer.htm?FK_Flow=" + node.id + "&UserNo=" + WebUser.No + "&Token=" + webUser.Token + "&Flow_V=0", node.iconCls);
    //延时3秒, 为什么要延迟？
    setTimeout(DesignerLoaded, 1000);
}

//打开流程到流程图
function OpenFlowToCanvas(node, id, text) {
    $(".mymask").show();
    if (node.attributes.DTYPE == "2") {//BPMN模式
        addTab(id, text, "../CCBPMDesigner/Designer.htm?FK_Flow=" + node.id + "&UserNo=" + WebUser.No + "&Token=" + webUser.Token + "&Flow_V=2", node.iconCls);
    } else if (node.attributes.DTYPE == "1") {//CCBPM
        addTab(id, text, "../CCBPMDesigner/Designer.htm?FK_Flow=" + node.id + "&UserNo=" + WebUser.No + "&Token=" + webUser.Token + "&Flow_V=1", node.iconCls);
    } else {
        //if (confirm("此流程版本为V1.0,是否执行升级为V2.0 ?")) {
        var attrs = node.attributes;    //这样写，是为了不将attributes里面原有的属性丢失，edited by liuxc,2015-11-05
        attrs.DTYPE = "1";
        attrs.Url = "../CCBPMDesigner/Designer.htm?FK_Flow=" + node.id + "&UserNo=" + WebUser.No + "&Token=" + webUser.Token + "&Flow_V=1";
        $('#flowTree').tree('update', {
            target: node.target,
            attributes: attrs
        });
        addTab(id, text, "../CCBPMDesigner/Designer.htm?FK_Flow=" + id + "&UserNo=" + WebUser.No + "&Token=" + webUser.Token + "&Flow_V=0", node.iconCls);
        //        } else {
        //            addTab(id, text, "DesignerSL.htm?FK_Flow=" + id + "&UserNo=" + WebUser.No + "&Token=" + webUser.Token + "&Flow_V=0", node.iconCls);
        //        }
    }
    //延时3秒
    setTimeout(DesignerLoaded, 1000);
}

var currSortId = "";
/// <summary>新建流程</summary>
function newFlow() {

    var currSort = $('#flowTree').tree('getSelected');
    var currSortId = "99";
    if (currSort && currSort.attributes["ISPARENT"] != 0) { //edit by qin 2016/2/16
        currSortId = $('#flowTree').tree('getSelected').id; //liuxc,20150323
    }

    var flowSort = currSortId.replace("F", "");

    var dgId = "iframDg";
    if (runModelType == 0)
        url = "../CCBPMDesigner/NewFlow.htm?sort=" + flowSort + "&s=" + Math.random();
    else
        url = "../CCBPMDesigner/NewFlow.htm?sort=" + flowSort + "&RunModel=1&s=" + Math.random();

    OpenEasyUiDialog(url, dgId, '新建流程', 650, 350, 'icon-new', true, function () {

        var win = document.getElementById(dgId).contentWindow;
        var newFlowInfo = win.getNewFlowInfo();

        if (newFlowInfo.FlowName == null || newFlowInfo.FlowName.length == 0
            || newFlowInfo.TreeFlowSort == null || newFlowInfo.TreeFlowSort.length == 0) {

            alert('信息填写不完整:' + newFlowInfo.FlowName + newFlowInfo.FlowSort);
            //$.messager.alert('错误', '信息填写不完整', 'error');
            return false;
        }

        var flowFrmType = newFlowInfo.FlowFrmType;
        if (newFlowInfo.RunModel == 1) {

            if (flowFrmType == 3 || flowFrmType == 4) {
                if (newFlowInfo.FrmUrl == "" || newFlowInfo.FrmUrl == null
                    || newFlowInfo.FrmUrl == undefined) {
                    alert('请输入url');
                    return false;
                }
            }
        }

        //判断流程标记是否存在  19.10.22 by sly
        if (newFlowInfo.FlowMark != "") {
            var flows = new Entities("BP.WF.Flows");
            flows.Retrieve("FlowMark", newFlowInfo.FlowMark);
            if (flows.length > 0) {
                alert('该流程标记[' + newFlowInfo.FlowMark + ']已经存在系统中');
                return false;
            }
        }

        var html = $("#ShowMsg").html();
        $("#ShowMsg").html(html + " ccbpm 正在创建流程请稍后....");
        $("#ShowMsg").css({ "width": "320px" });
        $(".mymask").show();

        newFlowInfo.FlowSort = flowSort;

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
        handler.AddJson(newFlowInfo);
        var data = handler.DoMethodReturnString("Defualt_NewFlow");

        $(".mymask").hide();
        $("#ShowMsg").html(html);
        $("#ShowMsg").css({ "width": "32px" });
        if (data.indexOf('err@') == 0) {
            alert(data);
            return;
        }

        var flowNo = data;
        var flowName = newFlowInfo.FlowName;

        //在左侧流程树上增加新建的流程,并选中
        //获取新建流程所属的类别节点
        //todo:此处还有问题，类别id与流程id可能重复，重复就会出问题，解决方案有待进一步确定
        var sort = "F" + newFlowInfo.FlowSort;

        var parentNode = $('#flowTree').tree('find', sort);
        var node = $('#flowTree').tree('append', {
            parent: parentNode.target,
            data: [{
                id: flowNo,
                text: flowNo + '.' + flowName,
                attributes: { ISPARENT: '0', TTYPE: 'FLOW', DTYPE: newFlowInfo.FlowVersion, MenuId: "mFlow", Url: "Designer.htm?FK_Flow=@@id&UserNo=@@WebUser.No&Token=@@webUser.Token" },
                iconCls: 'icon-flow1',
                checked: false
            }]
        });
        var nodeData = {
            id: flowNo,
            text: flowNo + '.' + flowName,
            attributes: { ISPARENT: '0', TTYPE: 'FLOW', DTYPE: newFlowInfo.FlowVersion, MenuId: "mFlow", Url: "Designer.htm?FK_Flow=@@id&UserNo=@@WebUser.No&Token=@@webUser.Token" },
            iconCls: 'icon-flow1',
            checked: false
        };
        //展开到指定节点
        $('#flowTree').tree('expandTo', $('#flowTree').tree('find', flowNo).target);
        $('#flowTree').tree('select', $('#flowTree').tree('find', flowNo).target);

        //在右侧流程设计区域打开新建的流程
        RefreshFlowJson();

        //打开流程.
        //OpenFlowToCanvas(nodeData, flowNo, nodeData.text);


    }, null);
}

/// <summary>新建流程类别子级</summary>
/// <param name="isSub" type="Boolean">是否是新建子级流程类别</param>
function newFlowSort(isSub) {

    var currSort = $('#flowTree').tree('getSelected');
    if (currSort == null || undefined == currSort.attributes.ISPARENT ||
        currSort.attributes.ISPARENT != '1' || (currSort.attributes.IsRoot == '1' && isSub == false))
        return;

    var propName = (isSub ? '子级' : '同级') + '流程类别';
    var val = promptGener(propName, '');
    if (val == null || val.length == 0) {
        alert('必须输入名称.');
        return false;
    }

    //传入参数
    var doWhat = isSub ? 'NewSubFlowSort' : 'NewSameLevelFlowSort';

    //创建目录.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
    handler.AddPara("No", currSort.id);
    handler.AddPara("Name", val);
    var data = handler.DoMethodReturnString(doWhat);

    var parentNode = isSub ? currSort : $('#flowTree').tree('getParent', currSort.target);

    $('#flowTree').tree('append', {
        parent: parentNode.target,
        data: [{
            id: data,
            text: val,
            attributes: { ISPARENT: '1', MenuId: "mFlowSort", TType: "FLOWTYPE" },
            checked: false,
            iconCls: 'icon-tree_folder',
            state: 'open',
            children: []
        }]
    });

    $('#flowTree').tree('select', $('#flowTree').tree('find', data).target);
}

//修改流程类别
function editFlowSort() {
    /// <summary>编辑流程类别</summary>
    var currSort = $('#flowTree').tree('getSelected');
    if (currSort == null)
        return;

    var val = prompt("请输入类别名称", currSort.text);
    if (val == null || val == '')
        return;

    var sortNo = currSort.id;
    sortNo = sortNo.substring(1);
    var en = new Entity("BP.WF.Template.FlowSort", sortNo);
    en.Name = val;
    en.Update();

    $('#flowTree').tree('update', {
        target: currSort.target,
        text: val
    });

}

//删除流程类别.
function deleteFlowSort() {
    /// <summary>删除流程类别</summary>
    var currSort = $('#flowTree').tree('getSelected');
    if (currSort == null || currSort.attributes.ISPARENT == undefined)
        return;

    if (window.confirm("你确定要删除名称为“" + currSort.text + "”的流程类别吗？") == false)
        return;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
    handler.AddPara("FK_FlowSort", currSort.id);
    var data = handler.DoMethodReturnString("DelFlowSort");
    alert(data);
    if (data.indexOf("err@") == 0)
        return "";

    //删除节点
    $('#flowTree').tree('remove', currSort.target);
}

/// <summary>流程树节点属性</summary>
function viewFlowSort() {

    var currSort = $('#flowTree').tree('getSelected');
    var currSortId = "99";
    if (currSort && currSort.attributes["ISPARENT"] != 0) {
        currSortId = $('#flowTree').tree('getSelected').id;
    }
    var dgId = "iframDgView";

    currSortId = currSortId.replace(/'F'/, '');
    currSortId = currSortId.replace("F", "");

    //  var url = "viewFlowShort.htm?sort=" + currSortId + "&s=" + Math.random();
    var url = "../../Comm/EnOnly.htm?EnName=BP.WF.Template.FlowSort&No=" + currSortId + "&s=" + Math.random();
    OpenEasyUiDialog(url, dgId, '流程树节点属性', 420, 300, 'icon-flow', false, function () {

        var win = document.getElementById(dgId).contentWindow;
        //var newFlowInfo = win.getNewFlowInfo();

    }, null);
}

//上移流程类别
function moveUpFlowSort() {
    var currSort = $('#flowTree').tree('getSelected');
    if (currSort == null) return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
    handler.AddPara("FK_FlowSort", currSort.id);
    var data = handler.DoMethodReturnString("MoveUpFlowSort");

    var before = $(currSort.target).parent().prev();
    if (before.length == 0 || $('#flowTree').tree('getData', before.children()[0]).attributes.TTYPE != "FLOWTYPE") {
        return;
    }

    $(currSort.target).parent().insertBefore(before);
}

//下移流程类别
function moveDownFlowSort() {
    var currSort = $('#flowTree').tree('getSelected');
    if (currSort == null) return;


    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
    handler.AddPara("FK_FlowSort", currSort.id);
    var data = handler.DoMethodReturnString("MoveDownFlowSort");

    var next = $(currSort.target).parent().next();
    if (next.length == 0 || $('#flowTree').tree('getData', next.children()[0]).attributes.TTYPE != "FLOWTYPE") {
        return;
    }
    $(currSort.target).parent().insertAfter(next);
}

function CloseAllTabs() {
    $('.tabs-inner span').each(function (i, n) {
        var t = $(n).text();
        if (t != '首页') {
            $('#tabs').tabs('close', t);
        }
    });
}



//导入流程
function ImpFlow() {
    var currFlow = $('#flowTree').tree('getSelected');
    if (currFlow == null || currFlow.attributes.ISPARENT != '0') {
        alert('没有获得当前的流程编号.');
        return;
    }
    var fk_flow = currFlow.id;
    url = "./../AttrFlow/Imp.htm?FK_Flow=" + fk_flow + "&Lang=CH";
    addTab(fk_flow + "PO", "导入流程模版", url);
}

//导入流程
function ImpFlowBySort() {
    var currFlow = $('#flowTree').tree('getSelected');
    var fk_flowSort = currFlow.id;
    fk_flowSort = fk_flowSort.replace("F", "");
    url = "./../AttrFlow/Imp.htm?FK_FlowSort=" + fk_flowSort + "&Lang=CH";
    addTab(fk_flowSort + "PO", "导入流程模版", url);
}

//添加流程到流程树
function AppendFlowToFlowSortTree(FK_FlowSort, FK_Flow, FlowName) {
    var flowSortNode = $('#flowTree').tree('find', "F" + FK_FlowSort);
    $('#flowTree').tree('append', {
        parent: flowSortNode.target,
        data: [{
            id: FK_Flow,
            text: FK_Flow + "." + FlowName,
            attributes: { ISPARENT: '0', MenuId: "mFlow", TType: "FLOW" },
            checked: false,
            iconCls: 'icon-flow1',
            state: 'open',
            children: []
        }]
    });

    $("#flowTree").tree("expand", flowSortNode.target);
    $('#flowTree').tree('select', $('#flowTree').tree('find', FK_Flow).target);

    //在右侧流程设计区域打开新建的流程
    RefreshFlowJson();
}

//导出流程
function ExpFlow() {
    var currFlow = $('#flowTree').tree('getSelected');
    if (currFlow == null || currFlow.attributes.ISPARENT != '0') {
        alert('没有获得当前的流程编号.');
        return;
    }

    var fk_flow = currFlow.id;
    url = "./../AttrFlow/Exp.htm?FK_Flow=" + fk_flow + "&Lang=CH";
    addTab(fk_flow + "PO", "导出流程模版", url);
}

//删除流程
function DeleteFlow() {
    /// <summary>删除流程</summary>
    var currFlow = $('#flowTree').tree('getSelected');
    if (currFlow == null || currFlow.attributes.ISPARENT != '0')
        return;

    if (window.confirm("你确定要删除名称为“" + currFlow.text + "”的流程吗？") == false)
        return;

    //执行删除流程.
    var en = new Entity("BP.WF.Flow", currFlow.id);
    var data = en.DoMethodReturnString("DoDelete");
    alert(data);
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    //如果右侧有打开该流程，则关闭
    var currFlowTab = $('#tabs').tabs('getTab', currFlow.text);
    if (currFlowTab) {
        //todo:此处因为有关闭前事件，直接这样用会弹出提示关闭框，怎么解决有待进一步确认
        $('#tabs').tabs('close', currFlow.text);
    }
    $('#flowTree').tree('remove', currFlow.target);
}

//流程属性,树上的.
function FlowProperty() {

    var currFlow = $('#flowTree').tree('getSelected');
    if (currFlow == null || currFlow.attributes.ISPARENT != '0')
        return;

    var userNo = GetQueryString("UserNo");

    var fk_flow = currFlow.id;
    url = "../../Comm/En.htm?DoType=En&EnName=BP.WF.Template.FlowExt&PKVal=" + fk_flow + "&Lang=CH&UserNo=" + WebUser.No;
    addTab(currFlow + "PO", "流程属性" + fk_flow, url);
    //WinOpen(url);
}

//上移流程
function moveUpFlow() {

    var currFlow = $('#flowTree').tree('getSelected');
    if (currFlow == null || currFlow.attributes.ISPARENT != '0')
        return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
    handler.AddPara("FK_Flow", currFlow.id);
    var data = handler.DoMethodReturnString("MoveUpFlow");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    var before = $(currFlow.target).parent().prev();
    if (before.length == 0 || $('#flowTree').tree('getData', before.children()[0]).attributes.TTYPE != "FLOW") {
        return;
    }

    $(currFlow.target).parent().insertBefore(before);
}

//下移流程
function moveDownFlow() {
    var currFlow = $('#flowTree').tree('getSelected');
    if (currFlow == null || currFlow.attributes.ISPARENT != '0')
        return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
    handler.AddPara("FK_Flow", currFlow.id);
    var data = handler.DoMethodReturnString("MoveDownFlow");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    var next = $(currFlow.target).parent().next();
    if (next.length == 0 || $('#flowTree').tree('getData', next.children()[0]).attributes.TTYPE != "FLOW") {
        return;
    }

    $(currFlow.target).parent().insertAfter(next);
}

//新建表单树类别
function newCCFormSort(isSub) {

    var currCCFormSort = $('#formTree').tree('getSelected');
    if (currCCFormSort == null || currCCFormSort.attributes.TType != "FORMTYPE")
        return;

    var propName = (isSub ? '子级' : '同级') + '表单类别';
    var val = promptGener(propName, "我的目录");
    if (val == null || val == undefined)
        return;

    var en = new Entity("BP.WF.Template.SysFormTree", currCCFormSort.id);
    var data = "";
    if (isSub)
        data = en.DoMethodReturnString("DoCreateSubNodeIt", val);
    else
        data = en.DoMethodReturnString("DoCreateSameLevelNodeIt", val);

    var parentNode = isSub ? currCCFormSort : $('#formTree').tree('getParent', currCCFormSort.target);

    $('#formTree').tree('append', {
        parent: parentNode.target,
        data: [{
            id: data,
            text: val,
            attributes: { MenuId: "mFormSort", TType: "FORMTYPE" },
            checked: false,
            iconCls: 'icon-tree_folder',
            state: 'open',
            children: []
        }]
    });

    $('#formTree').tree('select', $('#formTree').tree('find', data).target);

}

//编辑表单树类别
function EditCCFormSort() {

    var currCCFormSort = $('#formTree').tree('getSelected');
    if (currCCFormSort == null
        || currCCFormSort.attributes.TType != "FORMTYPE")
        return;

    var val = prompt("请输入表单类别名称", currCCFormSort.text);
    if (val == null || val == '')
        return;

    var sortNo = currCCFormSort.id;
    var en = new Entity("BP.WF.Template.SysFormTree", sortNo);
    en.Name = val;
    en.Update();

    $('#formTree').tree('update', {
        target: currCCFormSort.target,
        text: val
    });
    $('#formTree').tree('select', $('#formTree').tree('find', data).target);

}
//删除表单树类别
function DeleteCCFormSort() {
    var currFormSort = $('#formTree').tree('getSelected');
    if (currFormSort == null || currFormSort.attributes.TType != 'FORMTYPE')
        return;

    if (window.confirm("你确定要删除名称为“" + currFormSort.text + "”的类别吗？") == false)
        return;

    //删除表单树类别.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
    handler.AddPara("No", currFormSort.id);
    var data = handler.DoMethodReturnString("CCForm_DelFormSort");

    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }
    alert(data);
    $('#flowTree').tree('remove', currFormSort.target);
}

//上移表单类别
function moveUpCCFormSort() {
    var currFormSort = $('#formTree').tree('getSelected');
    if (currFormSort == null)
        return;

    var no = currFormSort.id;
    var en = new Entity("BP.WF.Template.SysFormTree", no);
    en.DoMethodReturnString("DoUp");

    var before = $(currFormSort.target).parent().prev();
    if (before.length == 0 || $('#formTree').tree('getData', before.children()[0]).attributes.TType != "FORMTYPE") {
        return;
    }

    $(currFormSort.target).parent().insertBefore(before);
}

//下移表单类别
function moveDownCCFormSort() {

    var currFormSort = $('#formTree').tree('getSelected');
    if (currFormSort == null)
        return;

    var no = currFormSort.id;
    var en = new Entity("BP.WF.Template.SysFormTree", no);
    en.DoMethodReturnString("DoDown");

    var next = $(currFormSort.target).parent().next();
    if (next.length == 0 || $('#formTree').tree('getData', next.children()[0]).attributes.TType != "FORMTYPE") {
        return;
    }

    $(currFormSort.target).parent().insertAfter(next);
}

//新建表单
function newFrm(frmType) {

    var node = $('#formTree').tree('getSelected');
    if (!node) {
        return;
    }

    var url = "../CCFormDesigner/NewFrmGuide.htm?Step=0&EntityType=" + frmType;
    if (node.attributes) {
        if (node.attributes.TType == "SRC") {
            url += "&Src=" + node.id;
        } else if (node.attributes.TType == "FORMTYPE") {
            //在表单类别上单击，则传递表单类别
            var pnode = $('#formTree').tree('getParent', node.target);
            if (pnode != null) {
                url += "&FK_FrmSort=" + node.id;

                while (pnode && pnode.attributes) {
                    if (pnode.attributes.TType == "SRC") {
                        url += "&Src=" + pnode.id;
                        break;
                    }
                    pnode = $('#formTree').tree('getParent', pnode.target);
                }
            }
        }
    }
    //如果右侧有打开该表单，则关闭
    var currTab = $('#tabs').tabs('getTab', "新建表单");
    if (currTab) {
        $('#tabs').tabs('close', "新建表单");
    }
    addTab("NewFrm", "新建表单", url);
}

///表单树添加表单项
///FK_FormTree:表单类别编号，No:表单编号，Name:表单名称
function AppendFrmToFormTree(FK_FormTree, No, Name) {
    var sortNode = $('#formTree').tree('find', FK_FormTree);
    $('#formTree').tree('append', {
        parent: sortNode.target,
        data: [{
            id: No,
            text: Name,
            attributes: { MenuId: "mForm", TType: "FORM", Url: "../FoolFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + No },
            checked: false,
            iconCls: 'icon-form',
            state: 'open',
            children: []
        }]
    });
    $("#formTree").tree("expand", sortNode.target);
    $('#formTree').tree('select', $('#formTree').tree('find', No).target);

    //打开表单
    addTab("DesignerFreeFrm" + No, Name, "../FoolFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + No);
}

//表单属性
function CCForm_Attr() {
    var node = $('#formTree').tree('getSelected');
    if (!node) {
        alert('请选择表单.');
        return;
    }
    var url = '../../Comm/En.htm?EnName=BP.WF.Template.Frm.MapFrmFree&PKVal=' + node.id;
    OpenEasyUiDialog(url, "CCForm_Attr", '表单属性', 900, 560, "icon-window");
}
//单据属性
function Bill_CCForm() {

    // if (plant != 'CCFlow') {
    //     alert('功能尚未同步到该版本上.');
    //     return;
    // }

    var node = $('#formTree').tree('getSelected');
    if (!node) {
        alert('请选择一个表单.');
        return;
    }

    // alert('sss');

    var en = new Entity("BP.CCBill.FrmBill", node.id);

    //流程单据.
    if (en.EntityType == 0)
        url = '../../Comm/En.htm?EnName=BP.WF.Template.Frm.MapFrmFree&PKVal=' + node.id;

    if (en.EntityType == 1)
        url = '../../Comm/En.htm?EnName=BP.CCBill.FrmBill&PKVal=' + node.id;

    if (en.EntityType == 2 || en.EntityType == 3)
        url = '../../Comm/En.htm?EnName=BP.CCBill.FrmDict&PKVal=' + node.id;

    //   alert(en.EntityType);
    // http: //localhost:2207/WF/Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.Frm.MapFrmFree&PKVal=CCFrm_GDZC&s=0.635120123659069

    OpenEasyUiDialog(url, "CCForm_Attr", '表单属性', 900, 560, "icon-window");
}
//打开单据
function Bill_Open() {

    // if (plant != 'CCFlow') {
    //     alert('功能尚未同步到该版本上.');
    //     return;
    // }

    var node = $('#formTree').tree('getSelected');
    if (!node) {
        alert('请选择一个表单.');
        return;
    }

    var en = new Entity("BP.CCBill.Template.FrmTemplate", node.id);
    if (en.EntityType == 0) {
        alert('独立表单暂不支持列表打开...');
        return;
    }

    var url = "";

    //检查单据的数据库表字段是否完整
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner2018")
    handler.AddPara("EnsName", node.id);
    handler.DoMethodReturnString("CheckBillFrm");


    //单据模式.
    if (en.EntityType == 1) {
        url = '../../CCBill/SearchBill.htm?FrmID=' + node.id;
        if (en.EntityShowModel == 1)
            url = '../../CCBill/BillTree.htm?FrmID=' + node.id;
    }


    //如果是实体就判断他的编辑模式.
    if (en.EntityType == 2) {
        if (en.EntityShowModel == 0) {
            if (en.EntityEditModel == 0)
                url = '../../CCBill/SearchDict.htm?FrmID=' + node.id;
            else
                url = '../../CCBill/SearchEditer.htm?FrmID=' + node.id;
        }

        if (en.EntityShowModel == 1) {
            url = '../../CCBill/DictTree.htm?FrmID=' + node.id;
        }
    }
    //树状结构
    if (en.EntityType == 3) {
        url = '../../CCBill/MyEntityTree.htm?FrmID=' + node.id;
    }

    window.open(url);
    return;
    //WinOpen(url);

    OpenEasyUiDialog(url, "CCForm_Attr", '我的单据', 1100, 560, "icon-window");
}
//设计自由表单
function designFreeFrm() {
    var node = $('#formTree').tree('getSelected');
    if (!node) {
        alert('请选择表单.');
        return;
    }
    addTab("DesignerFreeFrm" + node.id, node.text, "../FoolFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + node.id);
}

//设计傻瓜表单
function designFoolFrm() {
    var node = $('#formTree').tree('getSelected');
    if (!node) {
        alert('请选择表单.');
        return;
    }
    addTab("DesignerFoolFrm" + node.id, node.text, "../FoolFormDesigner/Designer.htm?FK_MapData=" + node.id + "&IsFirst=1&MyPK=" + node.id + "&IsEditMapData=True");
}

//上移表单
function moveUpCCFormTree() {

    var currForm = $('#formTree').tree('getSelected');
    if (currForm == null)
        return;

    var en = new Entity("BP.Sys.MapData", currForm.id);
    en.DoMethodReturnString("DoUp");

    var before = $(currForm.target).parent().prev();
    if (before.length == 0 || $('#formTree').tree('getData', before.children()[0]).attributes.TType != "FORM") {
        return;
    }

    $(currForm.target).parent().insertBefore(before);
}

//下移表单
function moveDownCCFormTree() {
    var currForm = $('#formTree').tree('getSelected');
    if (currForm == null)
        return;

    var en = new Entity("BP.Sys.MapData", currForm.id);
    en.DoMethodReturnString("DoOrderDown");

    var next = $(currForm.target).parent().next();
    if (next.length == 0 || $('#formTree').tree('getData', next.children()[0]).attributes.TType != "FORM") {
        return;
    }

    $(currForm.target).parent().insertAfter(next);
}


//删除流程树表单
function deleteCCFormTreeMapData() {

    var currForm = $('#formTree').tree('getSelected');
    if (currForm == null)
        return;

    if (confirm('您确认要删除吗？') == false)
        return;
    var en = new Entity("BP.Sys.MapData", currForm.id);
    en.Delete();

    //如果右侧有打开该表单，则关闭
    var currTab = $('#tabs').tabs('getTab', currForm.text);
    if (currTab) {
        $('#tabs').tabs('close', currForm.text);
    }
    //删除节点
    $('#formTree').tree('remove', currForm.target);
}

function CopyFrm() {

    var node = $('#formTree').tree('getSelected');
    if (!node) {
        alert('请选择表单.');
        return;
    }

    var frmID = promptGener('新的表单ID', node.id);
    var frmName = promptGener('新的表单名称', node.text);
    if (frmID == null || frmID == "") {
        alert("表单ID不能为空");
        return;
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCFormDesigner");
    handler.AddPara("FromFrmID", node.id);
    handler.AddPara("ToFrmID", frmID)
    handler.AddPara("ToFrmName", frmName)
    var data = handler.DoMethodReturnString("DoCopyFrm", frmID, frmName);

    if (data.indexOf('err@') != -1) {
        alert(data);
        return;
    }

    //表单库增加表单节点
    AppendFrmToFormTree(node.attributes.Node.ParentId, frmID, frmName);

    //设计表单.
    addTab("DesignerFrm" + frmID, "设计表单-" + frmName, "../FoolFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + frmID);
}


/*组织结构树操作结束*/

//打开窗体
function WinOpen(url) {
    var winWidth = 850;
    var winHeight = 680;
    if (screen && screen.availWidth) {
        winWidth = screen.availWidth;
        winHeight = screen.availHeight - 36;
    }
    window.open(url, "_blank", "height=" + winHeight + ",width=" + winWidth + ",top=0,left=0,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no");
}

var treesObj;   //保存功能区处理对象
var webUser = new WebUser();
if (webUser.No == "") {
    alert("登录信息丢失, 请重新登录.");
    SetHref("./Login.htm");
}


$(function () {

    treesObj = new FuncTrees("menuTab");
    treesObj.loadTrees();
    $(".mymask").hide();

});



//通过标题删除标签
function TabCloseByTitle(TabTitle) {
    //如果右侧有打开该表单，则关闭
    var currTab = $('#tabs').tabs('getTab', TabTitle);
    if (currTab) {
        $('#tabs').tabs('close', TabTitle);
    }
}
