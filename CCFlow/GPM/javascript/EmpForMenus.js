//树是否还在加载
var treeIsLoading = true;
//加载部门人员
function DeptEmpCallBack(js, scope) {

    if (js == "") js = "[]";
    var pushData = eval('(' + js + ')');
    var title = $("#copyMsg").text();
    //加载部门人员树
    $("#deptEmpTree").tree({
        data: pushData,
        iconCls: 'tree-folder',
        checkbox: true,
        dnd: false,
        onBeforeExpand: function (node) {
            if (node) {
                DeptEmpChildNodes(node);
            }
        },
        onClick: function (node) {
            if (node) {
                DeptEmpChildNodes(node);
            }
        }
    });
    //弹出窗体
    $('#deptEmpDialog').dialog({
        title: title,
        width: 500,
        height: 530,
        closed: false,
        modal: true,
        iconCls: 'icon-rights',
        resizable: true,
        toolbar: [{
            text: '清空式复制',
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
                //获取复制对象
                var options = document.getElementById("lbEmp").options;
                var copyNo = options[copySelectIndex].value;
                //进行保存
                Application.data.clearOfCopyUserPower(copyNo, pastUsers, function (js, scope) {
                    if (js == "success") {
                        CC.Message.showError("系统提示", "授权成功！");
                    } else {
                        CC.Message.showError("系统提示", "授权失败！" + js);
                    }
                    $('#deptEmpDialog').dialog("close");
                }, this);
            }
        }, '-', {
            text: '覆盖式复制',
            iconCls: 'icon-save-close',
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
                //获取复制对象
                var options = document.getElementById("lbEmp").options;
                var copyNo = options[copySelectIndex].value;
                //进行保存
                Application.data.coverOfCopyUserPower(copyNo, pastUsers, function (js, scope) {
                    if (js == "success") {
                        CC.Message.showError("系统提示", "授权成功！");
                    } else {
                        CC.Message.showError("系统提示", "授权失败！" + js);
                    }
                    $('#deptEmpDialog').dialog("close");
                }, this);
            }
        }]
    });

}

//加载所选节点的子节点
function DeptEmpChildNodes(node) {
    var childNodes = $('#deptEmpTree').tree('getChildren', node.target);
    if (childNodes && childNodes.length > 0 && childNodes[0].text == '加载中...') {
        $('#deptEmpTree').tree('remove', childNodes[0].target);
        Application.data.getDeptEmpChildNodes(node.id, function (js, scope) {
            if (js) {
                var pushData = eval('(' + js + ')');
                $('#deptEmpTree').tree('append', { parent: node.target, data: pushData });
                $('#deptEmpTree').tree('expand', node.target);
            }
        }, this);
    }
}

//复制权限
var copySelectIndex = null;
function CopyRight() {
    copySelectIndex = document.getElementById("lbEmp").selectedIndex;
    if (copySelectIndex >= 0) {
        var options = document.getElementById("lbEmp").options;
        $("#copyMsg").text("复制了：" + options[copySelectIndex].text + " 的权限");
        Application.data.getDeptEmpTree(DeptEmpCallBack, this);

    } else {
        CC.Message.showError("提示", "请选择复制对象。");
    }
}

//加载目录
function showMenusTree(js, scope) {
    if (js == "") js = [];
    var pushData = eval('(' + js + ')');
    treeIsLoading = true;
    //加载系统目录
    $("#menuTree").tree({
        data: pushData,
        iconCls: 'tree-folder',
        collapsed: true,
        checkbox: true,
        lines: true,
        onCheck: function () {
            if (!treeIsLoading) {
                var pasteSelectIndex = document.getElementById("lbEmp").selectedIndex;
                if (pasteSelectIndex >= 0) {
                    $('#saveRight').linkbutton('enable');
                }
            }
        },
        onLoadSuccess: function () {
            treeIsLoading = false;
            //            $(this).find('span.tree-checkbox').unbind().click(function () {
            //                return false;
            //            });
        },
        onExpand: function (node) {
            if (node) {
                EmpMenuChildNodes(node, false);
            }
        },
        onClick: function (node) {
            if (node) {
                EmpMenuChildNodes(node, true);
            }
        }
    });
    $("#menuTree").bind('contextmenu', function (e) {
        e.preventDefault();
        $('#treeMM').menu('show', {
            left: e.pageX,
            top: e.pageY
        });
    });
    $("#pageloading").hide();
    $('#saveRight').linkbutton('disable');
}
//加载所选节点的子节点
function EmpMenuChildNodes(node, expand) {
    var childNodes = $('#menuTree').tree('getChildren', node.target);
    if (childNodes && childNodes.length > 0 && childNodes[0].text == '加载中...') {
        $('#menuTree').tree('remove', childNodes[0].target);

        var selectIndex = document.getElementById("lbEmp").selectedIndex;
        var options = document.getElementById("lbEmp").options;
        if (selectIndex >= 0) {
            Application.data.getEmpOfMenusByEmpNo(options[selectIndex].value, node.id, "true", function (js) {
                if (js && js != '[]') {
                    var pushData = eval('(' + js + ')');
                    $('#menuTree').tree('append', { parent: node.target, data: pushData });
                    if (expand) $('#menuTree').tree('expand', node.target);
                }
            }, this);
        }
    }
}
//保存用户菜单权限
function SaveUserOfMenus() {
    var saveRight = $('#saveRight').linkbutton("options");
    if (saveRight && saveRight.disabled == false) {
        var menuIds = "";
        var menuIdsUn = "";
        var menuIdsUnExt = "";
        var nodes = [];
        var nodesUn = [];
        var nodesUnExt = [];
        //处理未完全选中项
        $("#menuTree").find('.tree-checkbox2').each(function () {
            var node = $(this).parent();
            nodesUn.push($.extend({}, $.data(node[0], 'tree-node'), {
                target: node[0],
                checked: node.find('.tree-checkbox').hasClass('tree-checkbox2')
            }));
        });
        if (nodesUn.length > 0) {
            for (var i = 0; i < nodesUn.length; i++) {
                if (menuIdsUn != '')
                    menuIdsUn += ',';
                menuIdsUn += nodesUn[i].id;
            }
        }
        //处理完全选中项
        nodes = $('#menuTree').tree('getChecked');
        if (nodes.length > 0) {
            for (var k = 0; k < nodes.length; k++) {
                var nodeText = nodes[k].text;
                if (nodeText == "加载中...") {
                    continue;
                }
                if (menuIds != '')
                    menuIds += ',';
                menuIds += nodes[k].id;
            }
        }
        //处理未展开而且包含已选中项
        $("#menuTree").find('.collaboration').each(function () {
            var node = $(this).parent();
            nodesUnExt.push($.extend({}, $.data(node[0], 'tree-node'), {
                target: node[0],
                checked: node.find('.tree-checkbox').hasClass('tree-checkbox0')
            }));
        });
        if (nodesUnExt.length > 0) {
            for (var i = 0; i < nodesUnExt.length; i++) {
                var childNodes = $('#menuTree').tree('getChildren', nodesUnExt[i].target);
                if (nodesUnExt[i].checked == true) {
                    //未展开需要在后台遍历子项
                    if (childNodes && childNodes.length > 0 && childNodes[0].text == '加载中...') {
                        if (menuIdsUnExt != '')
                            menuIdsUnExt += ',';
                        menuIdsUnExt += nodesUnExt[i].id;
                    }
                    //做为未完全选择项来进行保存
                    if (menuIdsUn != '')
                        menuIdsUn += ',';
                    menuIdsUn += nodesUnExt[i].id;
                }
            }
        }
        var selectIndex = document.getElementById("lbEmp").selectedIndex;
        var options = document.getElementById("lbEmp").options;
        if (selectIndex >= 0) {
            //保存数据
            $("#empNo").val(options[selectIndex].value);
            $("#menuIds").val(menuIds);
            $("#menuIdsUn").val(menuIdsUn);

            Application.data.saveUserOfMenus(options[selectIndex].value, menuIds, menuIdsUn, menuIdsUnExt, function (js, scope) {
                if (js == "success") {
                    CC.Message.showError("系统提示", "保存成功！");
                    $('#saveRight').linkbutton('disable');
                } else {
                    CC.Message.showError("系统提示", "保存失败！" + js);
                }
            }, this);
        }
    }
}
//所选人发生变化时
function LoadMenusOnEmpChange() {
    //            var saveRight = $('#saveRight').linkbutton("options");
    //            if (saveRight && saveRight.disabled == false) {
    //                $.messager.confirm('提示', '所选人员菜单发生变化，是否保存?', function (r) {
    //                    if (r) {
    //                        SaveUserOfMenus();
    //                    } else {
    //                        $('#saveRight').linkbutton('disable');
    //                    }
    //                });
    //            }
    //加载菜单
    var selectIndex = document.getElementById("lbEmp").selectedIndex;
    var options = document.getElementById("lbEmp").options;
    if (selectIndex >= 0) {
        $("#pageloading").show();
        Application.data.getEmpOfMenusByEmpNo(options[selectIndex].value, 1000, "false", showMenusTree, this);
    }
}
//查询
function QueryEmps() {
    var objSearch = $('#searchConten').val();
    //清空
    document.getElementById("lbEmp").length = 0;
    $("#pageloading").show();
    Application.data.getEmpsByNoOrName(encodeURI(objSearch), function (js, scope) {
        if (js == "") js = "[]";
        var pushData = eval('(' + js + ')');
        var objSelect = document.getElementById("lbEmp");
        if (pushData.length > 0) {
            $.each(pushData, function (index, val) {
                var varItem = new Option("[" + val.No + "]" + val.Name, val.No);
                objSelect.options.add(varItem);
            });
        }
        $("#pageloading").hide();
    }, this);
}
//加载人员信息
function EmpsCallBack(js, scope) {
    if (js == "") js = "[]";
    var pushData = eval('(' + js + ')');
    var objSelect = document.getElementById("lbEmp");
    if (pushData.length > 0) {
        $.each(pushData, function (index, val) {
            var varItem = new Option("[" + val.No + "]" + val.Name, val.No);
            objSelect.options.add(varItem);
        });
    }
    $("#pageloading").hide();
}
//初始化
$(function () {
    //获取人员信息
    Application.data.getEmps(EmpsCallBack, this);
    //绑定事件
    $("#lbEmp").bind('contextmenu', function (e) {
        e.preventDefault();
        $('#mm1').menu('show', {
            left: e.pageX,
            top: e.pageY
        });
    });
    $('#saveRight').bind('click', function () {
        SaveUserOfMenus();
    });
});