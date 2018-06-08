var toolbar = [{ 'text': '打开新窗口', 'iconCls': 'icon-save-close', 'handler': 'OpenNewWindow' }
    , { 'text': '增加同级', 'iconCls': 'icon-new', 'handler': 'addSampleNode' }
    , { 'text': '增加下级', 'iconCls': 'icon-new', 'handler': 'addChildNode' }
    , { 'text': '上移', 'iconCls': 'icon-remove', 'handler': 'tranUp' }
    , { 'text': '下移', 'iconCls': 'icon-remove', 'handler': 'tranDown' }
    , { 'text': '删除', 'iconCls': 'icon-cancel', 'handler': 'delNode' }
    , { 'text': '修改', 'iconCls': 'icon-edit', 'handler': 'editNode', menus: [{ text: 'Add', iconCls: 'icon-add', handler: function () { alert('add') } }, { text: 'Cut', iconCls: 'icon-cut', disabled: true, handler: function () { alert('cut') } }, '-', { text: 'Save', iconCls: 'icon-save', handler: function () { alert('save') } }]}];

var appName = "";
var curMenuNo = "";
var isModelWindow = true;
function winOpen(menuNo) {
    var strTimeKey = "";
    curMenuNo = menuNo;
    var date = new Date();
    strTimeKey += date.getFullYear(); //年
    strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
    strTimeKey += date.getDate(); //日
    strTimeKey += date.getHours(); //HH
    strTimeKey += date.getMinutes(); //MM
    strTimeKey += date.getSeconds(); //SS
    var val = window.showModalDialog("../WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.GPM.Menus&PK=" + menuNo + "&T=" + strTimeKey, "属性", "dialogWidth=800px;dialogHeight=460px;dialogTop=140px;dialogLeft=260px");
    if (isModelWindow) {
        LoadGrid();
    } else {
        LoadGrid2();
    }
}
//新建选项卡
function AddTab(title, url) {
    window.parent.addTab(title, url);
}
//打开新窗口
function OpenNewWindow() {
    var row = $('#menuGrid').treegrid('getSelected');
    if (row) {
        if (row.MENUTYPE == "5") {
            $.messager.alert('提示:', '功能点菜单没有子级！', 'info');
        }
        else {
            var url = "AppMenu.aspx?FK_App=" + appName + "&No=" + row.NO;
            var title = row.NAME + "菜单";
            AddTab(title, url);
        }

    }
    else {
        $.messager.alert('提示:', '请先选择项！', 'info');
    }
}
//新增同级
function addSampleNode() {
    var row = $('#menuGrid').treegrid('getSelected');
    if (row) {
        if (row.NO == no) {
            $.messager.alert('提示:', '当前菜单不允许新建同级！', 'info');
        }
        else {
            Application.data.nodeManage(row.NO, "sample", function (js, scop) {
                curMenuNo = row.NO;
                if (isModelWindow) {
                    LoadGrid();
                } else {
                    LoadGrid2();
                }
                //$('#test').treegrid('expandTo', '009');
                //$('#menuGrid').treegrid('select', '009');
            }, this);
        }

    }
    else {
        $.messager.alert('提示:', '请先选择项！', 'info');
    }
}
//新增子级
function addChildNode() {
    var row = $('#menuGrid').treegrid('getSelected');
    if (row) {
        Application.data.nodeManage(row.NO, "children", function (js, scop) {
            curMenuNo = row.NO;
            if (isModelWindow) {
                LoadGrid();
            } else {
                LoadGrid2();
            }
            //$('#test').treegrid('expandTo', '009');
            //$('#menuGrid').treegrid('select', '009');
        }, this);
    }
    else {
        $.messager.alert('提示:', '请先选择项！', 'info');
    }
}
//上移
function tranUp() {
    var row = $('#menuGrid').treegrid('getSelected');
    if (row) {
        Application.data.nodeManage(row.NO, "doup", function (js, scop) {
            curMenuNo = row.NO;
            if (isModelWindow) {
                LoadGrid();
            } else {
                LoadGrid2();
            }
            //$('#test').treegrid('expandTo', '009');
            //$('#menuGrid').treegrid('select', '009');
        }, this);
    }
    else {
        $.messager.alert('提示:', '请先选择项！', 'info');
    }
}
//下移
function tranDown() {
    var row = $('#menuGrid').treegrid('getSelected');
    if (row) {
        Application.data.nodeManage(row.NO, "dodown", function (js, scop) {
            curMenuNo = row.NO;
            if (isModelWindow) {
                LoadGrid();
            } else {
                LoadGrid2();
            }
            //$('#test').treegrid('expandTo', '009');
            //$('#menuGrid').treegrid('select', '009');
        }, this);
    }
    else {
        $.messager.alert('提示:', '请先选择项！', 'info');
    }
}
//删除
function delNode() {
    var row = $('#menuGrid').treegrid('getSelected');
    if (row) {
        var msg = "您确定删除所选项？";
        var isLeaf = 0;
        if (row.children) isLeaf = row.children.length;
        if (isLeaf > 0) {
            msg = "子项将一起删除，您确定删除？";
        }
        if (row.Flag.indexOf("Flow") > -1) {
            $.messager.alert('提示:', '该菜单为流程菜单，不能进行删除！', 'info');
            return;
        }
        //消息提醒
        $.messager.confirm('提示', msg, function (r) {
            if (r) {
                curMenuNo = null;
                Application.data.nodeManage(row.NO, "delete", function (js, scop) {
                    if (isModelWindow) {
                        LoadGrid();
                    } else {
                        LoadGrid2();
                    }
                    //$('#test').treegrid('expandTo', '009');
                    //$('#menuGrid').treegrid('select', '009');
                }, this);
            }
        });
    }
    else {
        $.messager.alert('提示:', '请先选择项！', 'info');
    }
}
//修改
function editNode() {
    var row = $('#menuGrid').treegrid('getSelected');
    if (row) {
        winOpen(row.NO);
    }
}
//新建流程模式菜单
function NewFlowModel() {
    var row = $('#menuGrid').treegrid('getSelected');
    if (row) {
        Application.data.getFlowTree("0", function (js, scorp) {
            curMenuNo = row.NO;
            LoadMainTree("flow", js, "流程树");
        }, this);
    }
    else {
        $.messager.alert('提示:', '请先选择项！', 'info');
    }
}
//新建表单模式菜单
function NewFormModel() {
    var row = $('#menuGrid').treegrid('getSelected');
    if (row) {
        Application.data.getFormTree("0", function (js, scorp) {
            curMenuNo = row.NO;
            LoadMainTree("form", js, "表单库");
        }, this);
    }
    else {
        $.messager.alert('提示:', '请先选择项！', 'info');
    }
}

function AddFlowMenu() {
    var row = $('#menuGrid').treegrid('getSelected');
    if (row) {
        Application.data.getFormTree("0", function (js, scorp) {
            curMenuNo = row.NO;
            var url = '/WF/MapDef/Rpt/Do.aspx?DoType=AddFlowMenu&RefNo=' + curMenuNo;
            window.showModalDialog(url, "sd", "scrollbars=yes,resizable=yes,center=yes,minimize=yes,resizable=no,maximize=yes,height= 600px,width= 550px, top=50px, left= 650px");
            window.location.href = window.location.href;
            // LoadMainTree("form", js, "表单库");
        }, this);
    }
    else {
        $.messager.alert('提示:', '请先选择项！', 'info');
    }
}
 

//加载所选节点的子节点
function FlowSortChildNodes(node) {
    var childNodes = $('#mainTree').tree('getChildren', node.target);
    if (childNodes && childNodes.length > 0 && childNodes[0].text == '加载中...') {
        $('#mainTree').tree('remove', childNodes[0].target);
        Application.data.getFlowTree(node.id, function (js, scope) {
            if (js) {
                var pushData = eval('(' + js + ')');
                $('#mainTree').tree('append', { parent: node.target, data: pushData });
                $('#mainTree').tree('expand', node.target);
            }
        }, this);
    }
}
//加载所选表单库子节点
function FormSortChildNodes(node) {
    var childNodes = $('#mainTree').tree('getChildren', node.target);
    if (childNodes && childNodes.length > 0 && childNodes[0].text == '加载中...') {
        $('#mainTree').tree('remove', childNodes[0].target);
        Application.data.getFormTree(node.id, function (js, scope) {
            if (js) {
                var pushData = eval('(' + js + ')');
                $('#mainTree').tree('append', { parent: node.target, data: pushData });
                $('#mainTree').tree('expand', node.target);
            }
        }, this);
    }
}

//加载流程、表单库树
function LoadMainTree(model, js, title) {
    if (js == "") js = "[]";
    var pushData = eval('(' + js + ')');

    //加载流程类别和流程
    $("#mainTree").tree({
        data: pushData,
        iconCls: 'tree-folder',
        checkbox: false,
        dnd: false,
        onBeforeExpand: function (node) {
            if (node) {
                var exp = true;
                var childNodes = $('#mainTree').tree('getChildren', node.target);
                if (childNodes && childNodes.length > 0 && childNodes[0].text == '加载中...') {
                    exp = false;
                }
                if (model == "flow") {
                    FlowSortChildNodes(node);
                }
                if (model == "form") {
                    FormSortChildNodes(node);
                }
                return exp;
            }
        },
        onClick: function (node) {
            if (node) {
                if (model == "flow") {
                    FlowSortChildNodes(node);
                }
                if (model == "form") {
                    FormSortChildNodes(node);
                }
            }
        }
    });
    //弹出窗体
    $('#modelDialog').dialog({
        title: title,
        width: 500,
        height: 530,
        closed: false,
        modal: true,
        iconCls: 'icon-rights',
        resizable: true,
        toolbar: [{
            text: '快速创建菜单',
            iconCls: 'icon-save-close',
            handler: function () {
                var nodes = $('#mainTree').tree('getSelected');
                var pastNos = "";
                var pastSortNos = "";
                //子项
                if (nodes.iconCls == "icon-4") {
                    pastNos = nodes.id;
                }
                //类别
                if (nodes.iconCls == "icon-tree_folder") {
                    if (pastSortNos != "")
                        pastSortNos = nodes.id;
                }
                if (pastNos == "" && pastSortNos == "") {
                    $.messager.alert('提示:', '请选择内容后再进行保存！' + pastNos + "|" + pastSortNos + nodes, 'info');
                    return;
                }
                //保存流程\表单库菜单
                Application.data.saveFlowFormMenu(model, curMenuNo, pastSortNos, pastNos, function (js, scope) {
                    if (isModelWindow) {
                        LoadGrid();
                    } else {
                        LoadGrid2();
                    }
                    $('#modelDialog').dialog("close");
                }, this);

            }
        }, {
            text: '自定义创建菜单',
            iconCls: 'icon-config',
            handler: function () {
                $('#modelDialog').dialog("close");
            }
        }, '-', {
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                $('#modelDialog').dialog("close");
            }
        }]
    });
}

function LoadGrid() {
    Application.data.getAppMenus(appName, function (js, scope) {
        if (js == "") js = [];
        var pushData = eval('(' + js + ')');
        $('#menuGrid').treegrid({
            data: pushData,
            idField: 'NO',
            treeField: 'NAME',
            toolbar: toolbar,
            striped: true,
            loadMsg: '数据加载中......',
            columns: [[
                   { field: 'NAME', title: '菜单名', width: 280, formatter: function (value, rowData, rowIndex) {
                       return "<a href='javascript:void(0)' onclick=winOpen('" + rowData.NO + "');>" + rowData.NAME + "</a>";
                   }
                   },
                   { field: 'NO', title: '编号', width: 60 },
                   { field: 'FLAG', title: '标识', width: 100 },
                   { field: 'URL', title: 'Url', width: 260, align: 'left' },
                   { field: 'WEBPATH', title: '图标名称', width: 200 },
                   { field: 'MYFILEPATH', title: '图标路径', width: 180 }
                   ]],
            onContextMenu: function (e, node) {
                e.preventDefault();
                // select the node
                $('#tt').tree('select', node.target);
                // display context menu
                $('#mm').menu('show', {
                    left: e.pageX,
                    top: e.pageY
                });
            },
            onDblClickCell: function (index, field, value) {
                winOpen(field.NO);
            }
        });
        if (curMenuNo != "" && curMenuNo != null) {
            var nodeData = $('#menuGrid').treegrid('find', curMenuNo);
            if (nodeData) $('#menuGrid').treegrid('select', curMenuNo);
        }
    }, this);
}
var no = "";
//初始页面
$(function () {
    appName = Application.common.getArgsFromHref("FK_App");
    no = Application.common.getArgsFromHref("No");
    if (no == "") {
        LoadGrid();
    }
    else {
        isModelWindow = false;
        LoadGrid2();
    }
});
function LoadGrid2() {
    Application.data.getAppChildMenus(appName, no, function (js, scope) {
        if (js == "") js = [];
        var pushData = eval('(' + js + ')');
        $('#menuGrid').treegrid({
            data: pushData,
            idField: 'NO',
            treeField: 'NAME',
            toolbar: toolbar,
            striped: true,
            loadMsg: '数据加载中......',
            columns: [[
                   { field: 'NAME', title: '菜单名', width: 280, formatter: function (value, rowData, rowIndex) {
                       return "<a href='javascript:void(0)' onclick=winOpen('" + rowData.NO + "');>" + rowData.NAME + "</a>";
                   }
                   },
                   { field: 'NO', title: '编号', width: 100 },
                   { field: 'FLAG', title: '标识', width: 100 },
                   { field: 'URL', title: 'Url', width: 260, align: 'left' },
                   { field: 'MYFILENAME', title: '图标名称', width: 80 },
                   { field: 'MYFILEPATH', title: '图标路径', width: 80 }
                   ]],
            onContextMenu: function (e, node) {
                e.preventDefault();
                // select the node
                $('#tt').tree('select', node.target);
                // display context menu
                $('#mm').menu('show', {
                    left: e.pageX,
                    top: e.pageY
                });
            },
            onDblClickCell: function (index, field, value) {
                winOpen(field.NO);
            }
        });
        if (curMenuNo != "" && curMenuNo != null) {
            var nodeData = $('#menuGrid').treegrid('find', curMenuNo);
            if (nodeData) $('#menuGrid').treegrid('select', curMenuNo);
        }
    }, this);
}