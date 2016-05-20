//加载部门人员
function DeptEmpCallBack(js, scope) {
    if (js == "") js = "[]";
    var pushData = eval('(' + js + ')');
    //加载部门人员树
    $("#deptEmpTree").tree({
        data: pushData,
        iconCls: 'tree-folder',
        checkbox: true,
        dnd: false,
        onExpand: function (node) {
            if (node) {
                DeptEmpChildNodes(node, false);
            }
        },
        onClick: function (node) {
            if (node) {
                DeptEmpChildNodes(node, true);
            }
        }
    });
    //弹出窗体
    $('#deptEmpDialog').dialog({
        title: "选择人员",
        width: 500,
        height: 500,
        closed: false,
        modal: true,
        iconCls: 'icon-rights',
        resizable: true,
        toolbar: [{
            text: '保存',
            iconCls: 'icon-save',
            handler: function () {
                var nodes = $('#deptEmpTree').tree('getChecked');
                var pastUsers = "";
                for (var i = 0; i < nodes.length; i++) {
                    var isLeaf = $('#deptEmpTree').tree('isLeaf', nodes[i].target);
                    if (isLeaf) {
                        if (pastUsers != "")
                            pastUsers += ",";
                        pastUsers += nodes[i].id;
                    }
                }

                if (pastUsers == "") {
                    CC.Message.showError("系统提示", "请选择用户！");
                    return;
                }
                //执行保存
                var selectedNode = $('#appTree').tree('getSelected');
                Application.data.saveEmpApp(pastUsers, selectedNode.id, function (js, scope) {
                    if (js == "true") {
                        //加载列表
                        LoadDataGridAdmin(selectedNode.id);
                        $('#deptEmpDialog').dialog("close");
                    } else {
                        CC.Message.showError("系统提示", "保存失败！");
                    }
                }, this);
            }
        }, '-', {
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                $('#deptEmpDialog').dialog("close");
            }
        }]
    });

}

//加载所选节点的子节点
function DeptEmpChildNodes(node, expand) {
    var childNodes = $('#deptEmpTree').tree('getChildren', node.target);
    if (childNodes && childNodes.length > 0 && childNodes[0].text == '加载中...') {
        $('#deptEmpTree').tree('remove', childNodes[0].target);
        Application.data.getDeptEmpChildNodes(node.id, function (js, scope) {
            if (js) {
                var pushData = eval('(' + js + ')');
                $('#deptEmpTree').tree('append', { parent: node.target, data: pushData });
                if (expand) $('#deptEmpTree').tree('expand', node.target);
            }
        }, this);
    }
}

//加载人员界面
function AddPersonForApp() {
    var selectedNode = $('#appTree').tree('getSelected');
    if (selectedNode) {
        Application.data.getDeptEmpTree(DeptEmpCallBack, this);
    } else {
        CC.Message.showError("提示", "请选择系统后再试。");
    }
}

//删除系统管理员
function DeleteEmpApp() {
    var rows = $('#empAppGrid').datagrid('getChecked');
    if (rows) {
        $.messager.confirm('警告', '确定删除选中数据?', function (y) {
            if (y) {
                var getDelid = "";
                $.each(rows, function (i, row) {
                    if (getDelid.length > 0) getDelid += ",";
                    getDelid += row.MyPK;
                });
                //执行删除
                Application.data.deleteEmpApp(getDelid, function (js, scope) {
                    //加载列表
                    var selectedNode = $('#appTree').tree('getSelected');
                    LoadDataGridAdmin(selectedNode.id);
                }, this);
            }
        });
    }
    else {
        $.messager.alert("提示", "您没有选中数据!");
    }
}

//展示系统树
function showAppsTree(js, scope) {
    if (js == "") js = [];
    var pushData = eval('(' + js + ')');

    //加载系统目录
    $("#appTree").tree({
        data: pushData,
        iconCls: 'tree-folder',
        collapsed: true,
        lines: true,
        onExpand: function (node) {
            if (node) {
                LoadMenuChildNodes(node, false);
            }
        },
        onClick: function (node) {
            if (node) {
                LoadMenuChildNodes(node, true);
            }
        }
    });
    //关闭等待页面
    $("#pageloading").hide();
}

//禁用添加按钮
function DisableAddBtn() {
    $('#addEmpApp').linkbutton('disable');
}
//启用enable
function EnableAddBtn() {
    $('#addEmpApp').linkbutton('enable');
}

//加载子节点
function LoadMenuChildNodes(node, expand) {
    var childNodes = $('#appTree').tree('getChildren', node.target);
    if (childNodes && childNodes.length > 0 && childNodes[0].text == '加载中...') {
        $('#appTree').tree('remove', childNodes[0].target);
    }
    //默认启用添加按钮
    EnableAddBtn();
    //根节点
    var rootNode = $('#appTree').tree('getRoot');
    var parentNode = $('#appTree').tree('getParent', node.target);
    if (parentNode) {
        if (rootNode.id == parentNode.id) {
            DisableAddBtn();
        }
    } else {
        DisableAddBtn();
    }
    LoadDataGridAdmin(node.id);
}

//管理员排序
function LoadGridOrderBy(orderBy) {
    var selectedNode = $('#appTree').tree('getSelected');
    if (selectedNode) {
        LoadDataGridAdmin(selectedNode.id, orderBy);
    } else {
        CC.Message.showError("提示", "请选择系统后再试。");
    }
}

//加载管理员列表
function LoadDataGridAdmin(menuNo, orderBy) {
    if (menuNo) {
        Application.data.LoadDataGridEmpApp(menuNo, orderBy, function (js, scope) {
            if (js) {
                if (js == "") js = [];
                var pushData = eval('(' + js + ')');
                $('#empAppGrid').datagrid({
                    data: pushData,
                    width: 'auto',
                    rownumbers: true,
                    singleSelect: false,
                    loadMsg: '数据加载中......',
                    columns: [[
                       { field: 'MyPK', title: '编号', sortable: true, checkbox: true, align: 'left', width: 60 },
                       { field: 'AppName', title: '系统名称', sortable: true, width: 200, align: 'left' },
                       { field: 'DeptName', title: '部门名称', sortable: true, width: 160, align: 'left' },
                       { field: 'Name', title: '姓名', sortable: true, width: 120, align: 'left' }
                       ]]
                });
            }
        }, this);
    }
}

//加载菜单
function LoadAppTree() {
    Application.data.getAppTreeForAdmin(0, showAppsTree, this);
}

//初始化
$(function () {
    $("#pageloading").show();
    //加载菜单
    LoadAppTree();
    //加载列表
    LoadDataGridAdmin("0");
});