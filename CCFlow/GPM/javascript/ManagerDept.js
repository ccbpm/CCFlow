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
        },
        onCheck: function (node, checked) {
            DeptEmpChildNodes(node, true);
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
                    if (nodes[i].attributes["isEmp"] == "yes")
                        pastUsers += nodes[i].id + ",";
                }

                if (pastUsers == "") {
                    CC.Message.showError("部门提示", "请选择用户！");
                    return;
                }
                //执行保存
                var selectedNode = $('#appTree').tree('getSelected');
                Application.data.saveDeptManager(pastUsers, selectedNode.id, function (js, scope) {
                    if (js == "true") {
                        //加载列表
                        LoadDataGridAdmin(1, 20);
                        $('#deptEmpDialog').dialog("close");
                    } else {
                        CC.Message.showError("部门提示", "保存失败！");
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
        CC.Message.showError("提示", "请选择部门后再试。");
    }
}

//删除部门管理员
function DeleteEmpApp() {
    var rows = $('#empAppGrid').datagrid('getChecked');
    var getDelid = "";

    $.each(rows, function (i, row) {
        if (getDelid.length > 0) getDelid += ",";
        getDelid += row.MYPK;
    });
    if (getDelid) {
        $.messager.confirm('警告', '确定删除选中数据?', function (y) {
            if (y) {
                //执行删除
                Application.data.deleteEmpDept(getDelid, function (js, scope) {
                    //加载列表
                    var selectedNode = $('#appTree').tree('getSelected');
                    LoadDataGridAdmin(1, 20);
                }, this);
            }
        });
    }
    else {
        $.messager.alert("提示", "您没有选中数据!", 'info');
    }
}

//展示部门树
function showAppsTree(js, scope) {
    if (js == "") js = [];
    var pushData = eval('(' + js + ')');

    //加载部门目录
    $("#appTree").tree({
        data: pushData,
        iconCls: 'tree-folder',
        collapsed: true,
        lines: true,
        onExpand: function (node) {
            if (node) {
                $('#appTree').tree('select', node.target);
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
    LoadDataGridAdmin(1, 20);
}
function switchRadio(v) {
    if (v == "dept") {
        if (document.getElementById("orderByDept").checked == false) {
            document.getElementById("orderByEmp").checked = true;
        } else {
            document.getElementById("orderByEmp").checked = false;
        }
    } else {
        if (document.getElementById("orderByEmp").checked == false) {
            document.getElementById("orderByDept").checked = true;
        } else {
            document.getElementById("orderByDept").checked = false;
        }
    }
    LoadDataGridAdmin(1, 20);
}
//加载管理员列表
function LoadDataGridAdmin(pageNumber, pageSize) {
    var selectedNode = $('#appTree').tree('getSelected');
    if (selectedNode) {
        var orderBy = "";

        if (document.getElementById("orderByDept").checked)
            orderBy = 'dept';

        if (document.getElementById("orderByEmp").checked)
            orderBy = 'emp';

        Application.data.loaddatagridDeptManager(selectedNode.id, orderBy, pageNumber, pageSize, function (js, scope) {
            if (js) {
                if (js == "") js = [];
                var pushData = eval('(' + js + ')');
                $('#empAppGrid').datagrid({
                    data: pushData,
                    width: 'auto',
                    pagination: true,
                    rownumbers: true,
                    singleSelect: false,
                    loadMsg: '数据加载中......',
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    pageList: [20, 3, 40, 50],
                    columns: [[
                       { field: 'MYPK', title: 'MyPK', sortable: true, checkbox: true, align: 'left', width: 60 },
                       { field: 'NO', title: '编号', sortable: true, width: 200, align: 'left' },
                       { field: 'NAME', title: '姓名', sortable: true, width: 200, align: 'left' },
                       { field: 'EMPNO', title: '员工编号', sortable: true, width: 160, align: 'left' },
                       { field: 'DEPTNAME', title: '部门', sortable: true, width: 160, align: 'left' },
                       { field: 'EMAIL', title: 'Email', sortable: true, width: 160, align: 'left' },
                       { field: 'LEADER', title: '领导', sortable: true, width: 160, align: 'left' }
                       ]]
                });
                //分页
                var pg = $("#empAppGrid").datagrid("getPager");
                if (pg) {
                    $(pg).pagination({
                        onRefresh: function (pageNumber, pageSize) {
                            LoadDataGridAdmin(pageNumber, pageSize);
                        },
                        onSelectPage: function (pageNumber, pageSize) {
                            LoadDataGridAdmin(pageNumber, pageSize);
                        }
                    });
                }
            }
        }, this);
    }
}

//加载菜单
function LoadAppTree() {
    Application.data.getDeptTreeForAdmin(0, showAppsTree, this);
}

//初始化
$(function () {
    $("#pageloading").show();
    //加载菜单
    LoadAppTree();
    //加载列表
    LoadDataGridAdmin(1, 20);
});