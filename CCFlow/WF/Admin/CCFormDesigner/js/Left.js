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
    if (!node || node.attributes.ISPARENT != '0') return;
    //首先关闭tab
    closeTab(node.text);
    $.post("/WF/Admin/CCBPMDesigner/controller.ashx", {
        action: 'ccbpm_flow_resetversion',
        FK_Flow: node.id
    }, function (jsonData) {
        $(".mymask").show();
        addTab(node.id, node.text, "Designer.htm?FK_Flow=" + node.id + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&Flow_V=0", node.iconCls);
        //延时3秒
        setTimeout(DesignerLoaded, 3000);
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
        //        if (confirm("此流程版本为V1.0,是否执行升级为V2.0 ?")) {
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
    setTimeout(DesignerLoaded, 3000);
}

/// <summary>新建流程</summary>
function newFlow() {
    var currSort = $('#flowTree').tree('getSelected');
    var currSortId = "99";
    if (currSort && currSort.attributes["ISPARENT"] != 0) {//edit by qin 2016/2/16
        currSortId = $('#flowTree').tree('getSelected').id; //liuxc,20150323
    }
    var dgId = "iframDg";
    var url = "DialogCtr/NewFlow.htm?sort=" + currSortId + "&s=" + Math.random();

    OpenEasyUiDialog(url, dgId, '新建流程', 650, 500, 'icon-new', true, function () {
        var win = document.getElementById(dgId).contentWindow;
        var newFlowInfo = win.getNewFlowInfo();

        if (newFlowInfo.flowName == null
         || newFlowInfo.flowName.length == 0
         || newFlowInfo.flowSort == null
         || newFlowInfo.flowSort.length == 0) {
            $.messager.alert('错误', '信息填写不完整', 'error');
            return false;
        }

        //传入参数
        var params = {
            method: "Do",
            doWhat: "NewFlow",
            para1: newFlowInfo.flowSort + ',' + newFlowInfo.flowName + ',' + newFlowInfo.dataStoreModel + ',' + newFlowInfo.pTable + ',' + newFlowInfo.flowCode + ',' + newFlowInfo.FlowVersion
        };
        //访问服务
        ajaxServiceDefault(params, function (data) {

            if (data.indexOf('@err') == 0) {
                alert(data);
                return;
            }

            //@代国强怎么在这里增加一个等待执行的提示窗口 ，提示为正在创建流程,请稍后.... 

            var jdata = $.parseJSON(data);
            if (jdata.success) {
                //在左侧流程树上增加新建的流程,并选中
                //获取新建流程所属的类别节点
                //todo:此处还有问题，类别id与流程id可能重复，重复就会出问题，解决方案有待进一步确定
                var parentNode = $('#flowTree').tree('find', "F" + newFlowInfo.flowSort);
                var node = $('#flowTree').tree('append', {
                    parent: parentNode.target,
                    data: [{
                        id: jdata.data.no,
                        text: jdata.data.name,
                        attributes: { ISPARENT: '0', TTYPE: 'FLOW', DTYPE: newFlowInfo.FlowVersion, MenuId: "mFlow", Url: "Designer.htm?FK_Flow=@@id&UserNo=@@WebUser.No&SID=@@WebUser.SID" },
                        iconCls: 'icon-flow1',
                        checked: false
                    }]
                });
                var nodeData = {
                    id: jdata.data.no,
                    text: jdata.data.name,
                    attributes: { ISPARENT: '0', TTYPE: 'FLOW', DTYPE: newFlowInfo.FlowVersion, MenuId: "mFlow", Url: "Designer.htm?FK_Flow=@@id&UserNo=@@WebUser.No&SID=@@WebUser.SID" },
                    iconCls: 'icon-flow1',
                    checked: false
                };
                //展开到指定节点
                $('#flowTree').tree('expandTo', $('#flowTree').tree('find', jdata.data.no).target);
                $('#flowTree').tree('select', $('#flowTree').tree('find', jdata.data.no).target);
                //在右侧流程设计区域打开新建的流程
                OpenFlowToCanvas(nodeData, jdata.data.no, jdata.data.name);
            }
            else {
                $.messager.alert('错误', '新建流程失败：' + jdata.msg, 'error');
            }
        }, this);
    }, null);
}

/// <summary>新建流程类别子级</summary>
/// <param name="isSub" type="Boolean">是否是新建子级流程类别</param>
function newFlowSort(isSub) {
    var currSort = $('#flowTree').tree('getSelected');
    if (currSort == null || undefined == currSort.attributes.ISPARENT ||
                currSort.attributes.ISPARENT != '1' || (currSort.attributes.IsRoot == '1' && isSub == false)) return;

    var propName = (isSub ? '子级' : '同级') + '流程类别';
    OpenEasyUiSampleEditDialog(propName, '新建', null, function (val) {
        if (val == null || val.length == 0) {
            $.messager.alert('错误', '请输入' + propName + '！', 'error');
            return false;
        }

        //传入参数.
        var doWhat = isSub ? 'NewSubFlowSort' : 'NewSameLevelFlowSort';
        var params = {
            method: "Do",
            doWhat: doWhat,
            para1: currSort.id + ',' + val
        };

        ajaxServiceDefault(params, function (data) {
            var jdata = $.parseJSON(data);
            if (jdata.success) {
                var parentNode = isSub ? currSort : $('#flowTree').tree('getParent', currSort.target);

                $('#flowTree').tree('append', {
                    parent: parentNode.target,
                    data: [{
                        id: jdata.data,
                        text: val,
                        attributes: { ISPARENT: '1', MenuId: "mFlowSort" },
                        checked: false,
                        iconCls: 'icon-tree_folder',
                        state: 'open',
                        children: []
                    }]
                });

                $('#flowTree').tree('select', $('#flowTree').tree('find', jdata.data).target);
            }
            else {
                $.messager.alert('错误', '新建' + propName + '失败：' + jdata.msg, 'error');
            }
        }, this);
    }, null, false, 'icon-new');
}

//修改流程类别
function editFlowSort() {
    /// <summary>编辑流程类别</summary>
    var currSort = $('#flowTree').tree('getSelected');
    if (currSort == null) return;

    OpenEasyUiSampleEditDialog('流程类别', '编辑', currSort.text, function (val) {
        if (val == null || val.length == 0) {
            $.messager.alert('错误', '请输入流程类别！', 'error');
            return false;
        }
        //传入后台参数
        var params = {
            method: "Do",
            doWhat: "EditFlowSort",
            para1: currSort.id + ',' + val
        };

        ajaxServiceDefault(params, function (data) {
            var jdata = $.parseJSON(data);
            if (jdata.success) {
                $('#flowTree').tree('update', {
                    target: currSort.target,
                    text: val
                });
            }
            else {
                $.messager.alert('错误', '编辑流程类别失败：' + jdata.msg, 'error');
            }
        });
    }, null, false, 'icon-edit');
}

function deleteFlowSort() {
    /// <summary>删除流程类别</summary>
    var currSort = $('#flowTree').tree('getSelected');
    if (currSort == null || currSort.attributes.ISPARENT == undefined) return;

    OpenEasyUiConfirm("你确定要删除名称为“" + currSort.text + "”的流程类别吗？", function () {
        //传入后台参数
        var params = {
            method: "Do",
            doWhat: "DelFlowSort",
            para1: currSort.id
        };
        ajaxServiceDefault(params, function (data) {
            var jdata = $.parseJSON(data);
            if (jdata.success == true) {
                CloseAllTabs();
                $('#flowTree').tree('remove', currSort.target);
            } else if (jdata.success == false && jdata.reason == "havesubsorts") {
                OpenEasyUiConfirm("所选类别下包含子流程类别，确定强制删除吗？", function () {
                    //传入后台参数
                    var params = {
                        method: "Do",
                        doWhat: "DelFlowSort",
                        force: "true",
                        para1: currSort.id
                    };
                    ajaxServiceDefault(params, function (data) {
                        var jdata = $.parseJSON(data);
                        if (jdata.success == true) {
                            CloseAllTabs();
                            $('#flowTree').tree('remove', currSort.target);
                        } else {
                            $.messager.alert('错误', '删除流程类别失败：' + jdata.msg, 'error');
                        }
                    });

                });
            } else if (jdata.success == false && jdata.reason == "haveflows") {
                OpenEasyUiConfirm("所选类别下包含流程，确定强制删除吗？", function () {
                    //传入后台参数
                    var params = {
                        method: "Do",
                        doWhat: "DelFlowSort",
                        force: "true",
                        para1: currSort.id
                    };
                    ajaxServiceDefault(params, function (data) {
                        var jdata = $.parseJSON(data);
                        if (jdata.success == true) {
                            CloseAllTabs();
                            $('#flowTree').tree('remove', currSort.target);
                        } else {
                            $.messager.alert('错误', '删除流程类别失败：' + jdata.msg, 'error');
                        }
                    });

                });
            }
            else {
                $.messager.alert('错误', '删除流程类别失败：' + jdata.msg, 'error');
            }
        });
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

//导入流程
function ImpFlowBySort() {
    var currFlow = $('#flowTree').tree('getSelected');
    var fk_flowSort = currFlow.id;
    fk_flowSort = fk_flowSort.replace("F", "");
    url = "./../AttrFlow/Imp.htm?FK_FlowSort=" + fk_flowSort + "&Lang=CH";
    addTab(fk_flowSort + "PO", "导入流程模版", url);
}

//导出流程
function ExpFlowBySort() {
    var currFlow = $('#flowTree').tree('getSelected');
    if (currFlow == null) {
        alert('没有获得当前的流程编号.');
        return;
    }
    var fk_flowSort = currFlow.id;
    fk_flowSort = fk_flowSort.replace("F", "");
    url = "./../AttrFlow/Exp.htm?FK_FlowSort=" + fk_flowSort + "&Lang=CH";
    addTab(fk_flowSort + "PO", "导出流程模版", url);
}


//删除流程
function DeleteFlow() {
    /// <summary>删除流程</summary>
    var currFlow = $('#flowTree').tree('getSelected');
    if (currFlow == null || currFlow.attributes.ISPARENT != '0')
        return;

    OpenEasyUiConfirm("你确定要删除名称为“" + currFlow.text + "”的流程吗？", function () {
        var params = {
            method: "Do",
            doWhat: "DelFlow",
            para1: currFlow.id
        };
        ajaxServiceDefault(params, function (data) {
            var jdata = $.parseJSON(data);
            if (jdata.success) {
                //如果右侧有打开该流程，则关闭
                var currFlowTab = $('#tabs').tabs('getTab', currFlow.text);
                if (currFlowTab) {
                    //todo:此处因为有关闭前事件，直接这样用会弹出提示关闭框，怎么解决有待进一步确认
                    $('#tabs').tabs('close', currFlow.text);
                }
                $('#flowTree').tree('remove', currFlow.target);
            }
            else {
                $.messager.alert('错误', '删除流程失败：' + jdata.msg, 'error');
            }
        }, this);
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
        if (node.attributes.TTYPE == "SRC") {
            url += "&Src=" + node.id;
        }
        else if (node.attributes.TTYPE == "FORMTYPE") {
            //在表单类别上单击，则传递表单类别
            var pnode = $('#formTree').tree('getParent', node.target);
            if (pnode != null) {
                url += "&FrmType=" + node.id;

                while (pnode && pnode.attributes) {
                    if (pnode.attributes.TTYPE == "SRC") {
                        url += "&Src=" + pnode.id;
                        break;
                    }

                    pnode = $('#formTree').tree('getParent', pnode.target);
                }
            }
        }
    }

    addTab("NewFrm", "新建表单", url);
}

function designFrm() {
    var node = $('#formTree').tree('getSelected');
    if (!node) {
        return;
    }

    addTab("DesignerFrm" + node.id, "设计表单-" + node.text, "../CCFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + node.id);
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
//    var url = "../../Comm/Sys/SFGuide.htm?DoType=New&FromApp=SL";
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
        method: "WebUserInfo"
    };
    ajaxServiceDefault(params, function (data) {
        var jdata = $.parseJSON(data);
        if (jdata.success) {
            WebUser.No = jdata.data.No;
            WebUser.Name = jdata.data.Name;
            WebUser.FK_Dept = jdata.data.FK_Dept;
            WebUser.SID = jdata.data.SID;
        }
        else {
            alert('获取当前登录用户失败：' + jdata.msg);
            window.location.href = "Login.aspx?DoType=Logout";
        }
    }, this);
}

var treesObj;   //保存功能区处理对象

$(function () {
    $(".mymask").show();
    InitUserInfo();
    treesObj = new FuncTrees("menuTab");
    treesObj.loadTrees();
    //定义等待界面的位置
    $(".mymaskContainer").offset({ left: ($(document).innerWidth() - 120) / 2, top: ($(document).innerHeight() - 50) / 2 });
    $(".mymask").hide();
});

