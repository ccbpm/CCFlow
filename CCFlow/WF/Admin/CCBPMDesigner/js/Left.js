/// <reference path="../../../Scripts/EasyUIUtility.js" />
/// <reference path="FuncTrees.js" />

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

    $.post(Handler, {
        DoType: 'Flow_ResetFlowVersion',
        FK_Flow: node.id
    }, function (jsonData) {

        if (jsonData.indexOf('err@') == 0) {
            alert(jsonData);
            return;
        }

        $(".mymask").show();

        addTab(node.id, node.text, "Designer.htm?FK_Flow=" + node.id + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&Flow_V=0", node.iconCls);
        //延时3秒, 为什么要延迟？
        setTimeout(DesignerLoaded, 1000);
    });
}

//打开流程到流程图
function OpenFlowToCanvas(node, id, text) {
    $(".mymask").show();
    if (node.attributes.DTYPE == "2") {//BPMN模式
        addTab(id, text, "Designer.htm?FK_Flow=" + node.id + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&Flow_V=2", node.iconCls);
    } else if (node.attributes.DTYPE == "1") {//CCBPM
        addTab(id, text, "Designer.htm?FK_Flow=" + node.id + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&Flow_V=1", node.iconCls);
    } else {
        //if (confirm("此流程版本为V1.0,是否执行升级为V2.0 ?")) {
        var attrs = node.attributes;    //这样写，是为了不将attributes里面原有的属性丢失，edited by liuxc,2015-11-05
        attrs.DTYPE = "1";
        attrs.Url = "Designer.htm?FK_Flow=" + node.id + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&Flow_V=1";
        $('#flowTree').tree('update', {
            target: node.target,
            attributes: attrs
        });
        addTab(id, text, "Designer.htm?FK_Flow=" + id + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&Flow_V=0", node.iconCls);
        //        } else {
        //            addTab(id, text, "DesignerSL.htm?FK_Flow=" + id + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&Flow_V=0", node.iconCls);
        //        }
    }
    //延时3秒
    setTimeout(DesignerLoaded, 1000);
}

/// <summary>新建流程</summary>
function newFlow() {

    var currSort = $('#flowTree').tree('getSelected');
    var currSortId = "99";
    if (currSort && currSort.attributes["ISPARENT"] != 0) { //edit by qin 2016/2/16
        currSortId = $('#flowTree').tree('getSelected').id; //liuxc,20150323
    }
    var dgId = "iframDg";
    var url = "NewFlow.htm?sort=" + currSortId + "&s=" + Math.random();
    OpenEasyUiDialog(url, dgId, '新建流程', 650, 350, 'icon-new', true, function () {

        var win = document.getElementById(dgId).contentWindow;
        var newFlowInfo = win.getNewFlowInfo();

        if (newFlowInfo.flowName == null || newFlowInfo.flowName.length == 0 || newFlowInfo.flowSort == null || newFlowInfo.flowSort.length == 0) {
            $.messager.alert('错误', '信息填写不完整', 'error');
            return false;
        }
        //传入参数
        var params = {
            action: "NewFlow",
            paras: newFlowInfo.flowSort + ',' + newFlowInfo.flowName + ',' + newFlowInfo.dataStoreModel + ',' + newFlowInfo.pTable + ',' + newFlowInfo.flowCode + ',' + newFlowInfo.FlowVersion
        };

        //访问服务
        ajaxService(params, function (data) {

            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }

            var flowNo = data;
            var flowName = newFlowInfo.flowName;

            //在左侧流程树上增加新建的流程,并选中
            //获取新建流程所属的类别节点
            //todo:此处还有问题，类别id与流程id可能重复，重复就会出问题，解决方案有待进一步确定
            var parentNode = $('#flowTree').tree('find', "F" + newFlowInfo.flowSort);
            var node = $('#flowTree').tree('append', {
                parent: parentNode.target,
                data: [{
                    id: flowNo,
                    text: flowNo + '.' + flowName,
                    attributes: { ISPARENT: '0', TTYPE: 'FLOW', DTYPE: newFlowInfo.FlowVersion, MenuId: "mFlow", Url: "Designer.htm?FK_Flow=@@id&UserNo=@@WebUser.No&SID=@@WebUser.SID" },
                    iconCls: 'icon-flow1',
                    checked: false
                }]
            });
            var nodeData = {
                id: flowNo,
                text: flowNo + '.' + flowName,
                attributes: { ISPARENT: '0', TTYPE: 'FLOW', DTYPE: newFlowInfo.FlowVersion, MenuId: "mFlow", Url: "Designer.htm?FK_Flow=@@id&UserNo=@@WebUser.No&SID=@@WebUser.SID" },
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

        }, this);
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
    var val = window.prompt(propName, '');
    if (val == null || val.length == 0) {
        alert('必须输入名称.');
        return false;
    }

    //传入参数
    var doWhat = isSub ? 'NewSubFlowSort' : 'NewSameLevelFlowSort';
    var params = {
        action: doWhat,
        No: currSort.id,
        Name: val
    };

    ajaxService(params, function (data) {
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

    }, this);
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

    //传入后台参数
    var params = {
        DoType: "EditFlowSort",
        No: currSort.id,
        Name: val
    };

    ajaxService(params, function (data) {

        if (data.indexOf('err@') == 0) {
            alert(data);
        }

        $('#flowTree').tree('update', {
            target: currSort.target,
            text: val
        });

    }, this);
}

function deleteFlowSort() {
    /// <summary>删除流程类别</summary>
    var currSort = $('#flowTree').tree('getSelected');
    if (currSort == null || currSort.attributes.ISPARENT == undefined) return;

    OpenEasyUiConfirm("你确定要删除名称为“" + currSort.text + "”的流程类别吗？", function () {
        //传入后台参数
        var params = {
            DoType: "DelFlowSort",
            FK_FlowSort: currSort.id
        };
        ajaxService(params, function (data) {
            alert(data);
            //删除节点
            $('#flowTree').tree('remove', currSort.target);
        });
    });
}

/// <summary>流程树节点属性</summary>
function viewFlowSort() {

    var currSort = $('#flowTree').tree('getSelected');
    var currSortId = "99";
    if (currSort && currSort.attributes["ISPARENT"] != 0) {
        currSortId = $('#flowTree').tree('getSelected').id;
    }
    var dgId = "iframDgView";
    var url = "viewFlowShort.htm?sort=" + currSortId + "&s=" + Math.random();
    OpenEasyUiDialog(url, dgId, '流程树节点属性', 420, 300, 'icon-flow', false, function () {

        var win = document.getElementById(dgId).contentWindow;
        //var newFlowInfo = win.getNewFlowInfo();

    }, null);
}

//上移流程类别
function moveUpFlowSort() {
    var currSort = $('#flowTree').tree('getSelected');
    if (currSort == null) return;

    //传入后台参数
    var params = {
        DoType: "MoveUpFlowSort",
        FK_FlowSort: currSort.id
    };
    ajaxService(params, function (data) {
        var before = $(currSort.target).parent().prev();
        if (before.length == 0 || $('#flowTree').tree('getData', before.children()[0]).attributes.TTYPE != "FLOWTYPE") {
            return;
        }

        $(currSort.target).parent().insertBefore(before);
    });
}

//下移流程类别
function moveDownFlowSort() {
    var currSort = $('#flowTree').tree('getSelected');
    if (currSort == null) return;

    //传入后台参数
    var params = {
        DoType: "MoveDownFlowSort",
        FK_FlowSort: currSort.id
    };
    ajaxService(params, function (data) {
        var next = $(currSort.target).parent().next();
        if (next.length == 0 || $('#flowTree').tree('getData', next.children()[0]).attributes.TTYPE != "FLOWTYPE") {
            return;
        }

        $(currSort.target).parent().insertAfter(next);
    });
}

function CloseAllTabs() {
    $('.tabs-inner span').each(function (i, n) {
        var t = $(n).text();
        if (t != '首页') {
            $('#tabs').tabs('close', t);
        }
    });
}

//查询流程
function SearchFlow() {
    url = "./../CCBPMDesigner/SearchFlow.htm?Lang=CH";
    addTab("SPO", "查询流程", url);
}

//查询表单
function SearchForm() {
    url = "./../CCFormDesigner/SearchForm.htm?Lang=CH";
    addTab("SPO", "查询表单", url);
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

    OpenEasyUiConfirm("你确定要删除名称为“" + currFlow.text + "”的流程吗？", function () {
        var params = {
            DoType: "DelFlow",
            FK_Flow: currFlow.id
        };
        ajaxService(params, function (data) {

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


            //alert(data);

        }, this);
    });
}


//流程属性,树上的.
function FlowProperty() {

    var currFlow = $('#flowTree').tree('getSelected');
    if (currFlow == null || currFlow.attributes.ISPARENT != '0')
        return;

    var userNo = GetQueryString("UserNo");

    var fk_flow = currFlow.id;
    url = "../XAP/DoPort.htm?DoType=En&EnName=BP.WF.Template.FlowExt&PK=" + fk_flow + "&Lang=CH&UserNo=" + WebUser.No;
    addTab(currFlow + "PO", "流程属性" + fk_flow, url);
    //WinOpen(url);
}

//上移流程
function moveUpFlow() {
    var currFlow = $('#flowTree').tree('getSelected');
    if (currFlow == null || currFlow.attributes.ISPARENT != '0')
        return;

    //传入后台参数
    var params = {
        DoType: "MoveUpFlow",
        FK_Flow: currFlow.id
    };
    ajaxService(params, function (data) {
        var before = $(currFlow.target).parent().prev();
        if (before.length == 0 || $('#flowTree').tree('getData', before.children()[0]).attributes.TTYPE != "FLOW") {
            return;
        }

        $(currFlow.target).parent().insertBefore(before);
    });
}

//下移流程
function moveDownFlow() {
    var currFlow = $('#flowTree').tree('getSelected');
    if (currFlow == null || currFlow.attributes.ISPARENT != '0')
        return;

    //传入后台参数
    var params = {
        DoType: "MoveDownFlow",
        FK_Flow: currFlow.id
    };
    ajaxService(params, function (data) {
        var next = $(currFlow.target).parent().next();
        if (next.length == 0 || $('#flowTree').tree('getData', next.children()[0]).attributes.TTYPE != "FLOW") {
            return;
        }

        $(currFlow.target).parent().insertAfter(next);
    });
}

//新建表单树类别
function newCCFormSort(isSub) {
    var currCCFormSort = $('#formTree').tree('getSelected');
    if (currCCFormSort == null || currCCFormSort.attributes.TType != "FORMTYPE")
        return;

    var propName = (isSub ? '子级' : '同级') + '表单类别';

    OpenEasyUiSampleEditDialog(propName, '新建', null, function (val) {
        if (val == null || val.length == 0) {
            $.messager.alert('错误', '请输入' + propName + '！', 'error');
            return false;
        }

        //传入参数
        var doWhat = isSub ? 'CCForm_NewSubSort' : 'CCForm_NewSameLevelSort';
        var params = {
            action: doWhat,
            No: currCCFormSort.id,
            Name: val
        };

        ajaxService(params, function (data) {
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

        }, this);
    }, null, false, 'icon-new');
}

//编辑表单树类别
function EditCCFormSort() {
    var currCCFormSort = $('#formTree').tree('getSelected');
    if (currCCFormSort == null || currCCFormSort.attributes.TType != "FORMTYPE")
        return;

    OpenEasyUiSampleEditDialog("编辑类别名称", '', currCCFormSort.text, function (val) {
        if (val == null || val.length == 0) {
            $.messager.alert('错误', '请输入类别名称', 'error');
            return false;
        }

        //传入参数
        var params = {
            action: "CCForm_EditCCFormSort",
            No: currCCFormSort.id,
            Name: val
        };

        ajaxService(params, function (data) {

            $('#formTree').tree('update', {
                target: currCCFormSort.target,
                text: val
            });
            $('#formTree').tree('select', $('#formTree').tree('find', data).target);

        }, this);
    }, null, false, 'icon-new');
}
//删除表单树类别
function DeleteCCFormSort() {
    var currFormSort = $('#formTree').tree('getSelected');
    if (currFormSort == null || currFormSort.attributes.TType != 'FORMTYPE')
        return;

    OpenEasyUiConfirm("你确定要删除名称为“" + currFormSort.text + "”的类别吗？", function () {
        var params = {
            DoType: "CCForm_DelFormSort",
            No: currFormSort.id
        };
        ajaxService(params, function (data) {
            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }
            alert(data);
            $('#flowTree').tree('remove', currFormSort.target);

        }, this);
    });
}

//上移表单类别
function moveUpCCFormSort() {
    var currFormSort = $('#formTree').tree('getSelected');
    if (currFormSort == null)
        return;
    //传入后台参数
    var params = {
        DoType: "CCForm_MoveUpCCFormSort",
        No: currFormSort.id
    };
    ajaxService(params, function (data) {
        var before = $(currFormSort.target).parent().prev();
        if (before.length == 0 || $('#formTree').tree('getData', before.children()[0]).attributes.TType != "FORMTYPE") {
            return;
        }

        $(currFormSort.target).parent().insertBefore(before);
    });
}

//下移表单类别
function moveDownCCFormSort() {
    var currFormSort = $('#formTree').tree('getSelected');
    if (currFormSort == null)
        return;

    //传入后台参数
    var params = {
        DoType: "CCForm_MoveDownCCFormSort",
        No: currFormSort.id
    };
    ajaxService(params, function (data) {
        var next = $(currFormSort.target).parent().next();
        if (next.length == 0 || $('#formTree').tree('getData', next.children()[0]).attributes.TType != "FORMTYPE") {
            return;
        }

        $(currFormSort.target).parent().insertAfter(next);
    });
}

//新建表单
function newFrm() {
    var node = $('#formTree').tree('getSelected');
    if (!node) {
        return;
    }

    var url = "../CCFormDesigner/NewFrmGuide.htm?Step=0";
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
            attributes: { MenuId: "mForm", TType: "FORM", Url: "../CCFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + No },
            checked: false,
            iconCls: 'icon-form',
            state: 'open',
            children: []
        }]
    });
    $("#formTree").tree("expand", sortNode.target);
    $('#formTree').tree('select', $('#formTree').tree('find', No).target);

    //打开表单
    addTab("DesignerFreeFrm" + No, Name, "../CCFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + No);
}

//表单属性
function CCForm_Attr() {
    var node = $('#formTree').tree('getSelected');
    if (!node) {
        alert('请选择表单.');
        return;
    }
    var url = '../../Comm/En.htm?EnsName=BP.WF.Template.MapFrmFrees&PK=' + node.id;
    OpenEasyUiDialog(url, "CCForm_Attr", '表单属性', 900, 560, "icon-window");
}

//设计自由表单
function designFreeFrm() {
    var node = $('#formTree').tree('getSelected');
    if (!node) {
        alert('请选择表单.');
        return;
    }
    addTab("DesignerFreeFrm" + node.id, node.text, "../CCFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + node.id);
}

//设计傻瓜表单
function designFoolFrm() {
    var node = $('#formTree').tree('getSelected');
    if (!node) {
        alert('请选择表单.');
        return;
    }
    addTab("DesignerFoolFrm" + node.id, node.text, "../FoolFormDesigner/Designer.htm?FK_MapData=" + node.id + "&MyPK=" + node.id + "&IsEditMapData=True");
}

//上移表单
function moveUpCCFormTree() {
    var currForm = $('#formTree').tree('getSelected');
    if (currForm == null)
        return;
    //传入后台参数
    var params = {
        DoType: "CCForm_MoveUpCCFormTree",
        FK_MapData: currForm.id
    };
    ajaxService(params, function (data) {
        var before = $(currForm.target).parent().prev();
        if (before.length == 0 || $('#formTree').tree('getData', before.children()[0]).attributes.TType != "FORM") {
            return;
        }

        $(currForm.target).parent().insertBefore(before);
    });
}

//下移表单
function moveDownCCFormTree() {
    var currForm = $('#formTree').tree('getSelected');
    if (currForm == null)
        return;

    //传入后台参数
    var params = {
        DoType: "CCForm_MoveDownCCFormTree",
        FK_MapData: currForm.id
    };
    ajaxService(params, function (data) {
        var next = $(currForm.target).parent().next();
        if (next.length == 0 || $('#formTree').tree('getData', next.children()[0]).attributes.TType != "FORM") {
            return;
        }

        $(currForm.target).parent().insertAfter(next);
    });
}

//删除流程树表单
function deleteCCFormTreeMapData() {
    var currForm = $('#formTree').tree('getSelected');
    if (currForm == null)
        return;

    OpenEasyUiConfirm("你确定要删除名称为“" + currForm.text + "”的表单吗？", function () {
        //传入后台参数
        var params = {
            DoType: "CCForm_DeleteCCFormMapData",
            FK_MapData: currForm.id
        };
        ajaxService(params, function (data) {
            alert("删除成功！");
            //如果右侧有打开该表单，则关闭
            var currTab = $('#tabs').tabs('getTab', currForm.text);
            if (currTab) {
                $('#tabs').tabs('close', currForm.text);
            }
            //删除节点
            $('#formTree').tree('remove', currForm.target);
        });
    });
}

function CopyFrm() {

    var node = $('#formTree').tree('getSelected');
    if (!node) {
        alert('请选择表单.');
        return;
    }

    var age = prompt("请输入你的年龄:", "20");
    if (age != null) {
        alert("你今年" + age + "岁了!");
    } else {
        alert("你按了[取消]按钮");
    }

    // window.p
    //   if ( window.config()
    // addTab("DesignerFrm" + node.id, "设计表单-" + node.text, "../CCFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + node.id);
}


//新建数据源，added by liuxc,2015-10-7
function newSrc() {
    //  var url = "../../Comm/En.htm?EnsName=BP.Sys.SFDBSrcs";
    var url = "../../Comm/Sys/SFDBSrcNewGuide.htm?DoType=New";
    //OpenEasyUiDialog(url, "euiframeid", '新建数据源', 800, 495, 'icon-new');
    //todo:增加数据源后，在树上增加新结节的逻辑
    addTab("NewSrc", "新建数据源", url);
}

//新建数据源表
function newSrcTable() {
    var url = "../FoolFormDesigner/CrateSFGuide.htm?DoType=New&FromApp=SL";
    //OpenEasyUiDialog(url, "euiframeid", '新建数据源表', 800, 495, 'icon-new');
    //todo:增加数据源表后，在树上增加新结节的逻辑
    addTab("NewSrcTable", "新建数据源表", url);
}

//数据源属性
function srcProperty() {
    var srcNode = $('#formTree').tree('getSelected');
    if (!srcNode || srcNode.attributes.TTYPE != 'SRC') {
        $.messager.alert('错误', '请选择数据源！', 'error');
        return;
    }

    var url = '../../Comm/Sys/SFDBSrcNewGuide.htm?DoType=Edit&No=' + srcNode.id + '&t=' + Math.random();
    //OpenEasyUiDialog(url, "euiframeid", srcNode.text + ' 属性', 800, 495, 'icon-edit');
    //todo:数据源属性修改后，在树上的结节信息的相应变更逻辑
    addTab(srcNode.id, srcNode.text, url, srcNode.iconCls);
}

//数据源表属性
function srcTableProperty() {
    var srcTableNode = $('#formTree').tree('getSelected');
    if (!srcTableNode || srcTableNode.attributes.TTYPE != 'SRCTABLE') {
        $.messager.alert('错误', '请选择数据源表！', 'error');
        return;
    }

    var url = '../FoolFormDesigner/Do.htm?DoType=EditSFTable&RefNo=' + srcTableNode.id + '&t=' + Math.random();
    //OpenEasyUiDialog(url, "euiframeid", srcTableNode.text + ' 属性', 800, 495, 'icon-edit');
    //todo:数据源表属性修改后，在树上的结节信息的相应变更逻辑
    addTab(srcTableNode.id, srcTableNode.text, url, srcTableNode.iconCls);
}

//数据源表数据查看/编辑
function srcTableData() {
    var srcTableNode = $('#formTree').tree('getSelected');
    if (!srcTableNode || srcTableNode.attributes.TTYPE != 'SRCTABLE') {
        $.messager.alert('错误', '请选择数据源表！', 'error');
        return;
    }

    var url = "../FoolFormDesigner/SFTableEditData.htm?FK_SFTable=" + srcTableNode.id; //todo:此处BP.Pub.Days样式的，页面报错
    //OpenEasyUiDialog(url, "euiframeid", srcTableNode.text + ' 数据编辑', 800, 495, 'icon-edit');
    addTab(srcTableNode.id, srcTableNode.text + ' 数据编辑', url, srcTableNode.iconCls);
}

/*组织结构树操作开始*/
function getSelected(sTreeId, sName, oChecks) {
    var node = $("#" + sTreeId).tree("getSelected");

    if (!node) {
        $.messager.alert("提示", "请选择" + (sName ? sName : "树结点") + "！", "warning");
        return null;
    }

    if (!oChecks) {
        return node;
    }

    var pass = true;
    var exist = false;

    for (var field in oChecks) {
        exist = false;

        if (node[field]) {
            exist = true;

            if (node[field] != oChecks[field]) {
                pass = false;
                break;
            }
        }

        if (!exist && node.attributes && node.attributes[field]) {
            exist = true;

            if (node.attributes[field] != oChecks[field]) {
                pass = false;
                break;
            }
        }

        if (!exist) {
            pass = false;
        }

        if (!pass) {
            break;
        }
    }

    if (!pass) {
        $.messager.alert("提示", "请选择" + (sName ? sName : "树结点") + "！检查规则不通过！", "warning");
        return null;
    }

    return node;
}

function newDept() {
    var node = getSelected(ORG_TREE, "部门", { TTYPE: "DEPT" });
    if (!node) return;

    var pnode = $("#" + ORG_TREE).tree("getParent", node.target);
    addTab("NewDept", "新建同级部门", "../../Comm/En.htm?EnsName=BP.GPM.Depts&ParentNo=" + node.id, "icon-new");
}

function newSubDept() {

}

function editDept() {

}

function deleteDept() {

}

function newStation() {

}

function editStation() {

}

function deleteStation() {

}

function newEmp() {

}

function editEmp() {

}

function deleteEmp() {

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
//用户信息
var WebUser = { No: '', Name: '', FK_Dept: '', SID: '' };
function InitUserInfo() {
    var params = {
        action: "GetWebUserInfo"
    };
    ajaxService(params, function (data) {

        if (data.indexOf('err@') == 0) {
            alert(data);
            window.location.href = "Login.htm?DoType=Logout";
            return;
        }

        var jdata = $.parseJSON(data);
        WebUser.No = jdata.No;
        WebUser.Name = jdata.Name;
        WebUser.FK_Dept = jdata.FK_Dept;
        WebUser.SID = jdata.SID;
    }, this);
}

function GenerStructureTree(parentrootid, pnodeid, treeid) {
    ajaxService({ action: "GetStructureTreeRootTable", parentrootid: parentrootid }, function (re) {
        var data = $.parseJSON(re);
        var roottarget;

        if (pnodeid) {
            roottarget = $("#" + treeid).tree("find", pnodeid).target;
        }

        $("#" + treeid).tree("append", {
            parent: roottarget,
            data: [{
                id: "DEPT_" + data[0].NO,
                text: data[0].NAME,
                state: "closed",
                attributes: { TType: "DEPT", IsLoad: false },
                children: [{
                    text: "加载中..."
                }]
            }]
        });

        $("#" + treeid).tree({
            onExpand: function (node) {
                ShowSubDepts(node, treeid);
            }
        });

        $("#" + treeid).tree("expand", $("#" + treeid).tree("getChildren", "DEPT_" + data[0].NO)[0].target);
    });
}

function ShowSubDepts(node, treeid) {
    if (node.attributes.IsLoad) {
        return;
    }

    var isStation = node.attributes.TType == "STATION";
    var data;

    if (isStation) {
        var deptNo = node.attributes.DeptId;
        var stationNo = node.attributes.StationId;

        ajaxService({ action: "GetEmpsByStationTable", DeptNo: deptNo, StationNo: stationNo }, function (re) {
            data = $.parseJSON(re);

            var children = $("#" + treeid).tree('getChildren', node.target);
            if (children && children.length >= 1) {
                if (children[0].text == "加载中...") {
                    $("#" + treeid).tree("remove", children[0].target);
                }
            }

            $.each(data, function () {
                $("#" + treeid).tree("append", {
                    parent: node.target,
                    data: [{
                        id: this.PARENTNO + "|" + this.NO,
                        text: this.NAME,
                        iconCls: "icon-user",
                        attributes: { TType: "EMP", StationId: stationNo, DeptId: deptNo }
                    }]
                });
            });

            node.attributes.IsLoad = true;
        });
    }
    else {
        var deptid = node.id.replace(/DEPT_/g, "");
        ajaxService({ action: "GetSubDeptsTable", rootid: deptid }, function (re) {
            data = $.parseJSON(re);

            var children = $("#" + treeid).tree('getChildren', node.target);
            if (children && children.length >= 1) {
                if (children[0].text == "加载中...") {
                    $("#" + treeid).tree("remove", children[0].target);
                }
            }

            $.each(data, function () {
                var n = {
                    id: this.TTYPE + "_" + this.NO,
                    text: this.NAME,
                    attributes: {
                        TType: this.TTYPE
                    }
                };

                switch (this.TTYPE) {
                    case "DEPT":
                        n.iconCls = "icon-tree_folder";
                        n.state = "closed";
                        n.attributes.IsLoad = false;
                        n.children = [{
                            text: "加载中..."
                        }];
                        break;
                    case "STATION":
                        n.iconCls = "icon-station";
                        n.state = "closed";
                        n.attributes.IsLoad = false;
                        n.attributes.StationId = this.NO;
                        n.attributes.DeptId = deptid;
                        n.children = [{
                            text: "加载中..."
                        }];
                        break;
                    case "EMP":
                        n.iconCls = "icon-user";
                        n.attributes.DeptId = deptid;
                        n.attributes.EmpId = this.NO;
                        break;
                }

                $("#" + treeid).tree("append", {
                    parent: node.target,
                    data: [n]
                });
            });

            node.attributes.IsLoad = true;
        });
    }
}

var treesObj;   //保存功能区处理对象

$(function () {
    $(".mymask").show();
    //InitUserInfo();

    var params = {
        action: "GetWebUserInfo"
    };

    ajaxService(params, function (data) {

        if (data.indexOf('err@') == 0) {
            alert(data);
            window.location.href = "Login.htm?DoType=Logout";
            return;
        }

        var jdata = $.parseJSON(data);
        WebUser.No = jdata.No;
        WebUser.Name = jdata.Name;
        WebUser.FK_Dept = jdata.FK_Dept;
        WebUser.SID = jdata.SID;

        SetTreeRoot(jdata);

        treesObj = new FuncTrees("menuTab");
        treesObj.loadTrees();
        //定义等待界面的位置
        $(".mymaskContainer").offset({ left: ($(document).innerWidth() - 120) / 2, top: ($(document).innerHeight() - 50) / 2 });
        $(".mymask").hide();
    }, this);
});

function SetTreeRoot(data) {
    //functrees[0].Nodes[0].RootParentId = data.RootOfFlow;
    //functrees[1].Nodes[0].RootParentId = data.RootOfForm;
    functrees[2].Nodes[0].MethodParams[0].value = data.RootOfDept;
}

//通过标题删除标签
function TabCloseByTitle(TabTitle) {
    //如果右侧有打开该表单，则关闭
    var currTab = $('#tabs').tabs('getTab', TabTitle);
    if (currTab) {
        $('#tabs').tabs('close', TabTitle);
    }
}