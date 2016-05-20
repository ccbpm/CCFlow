function replaceTrim(val) {//去除空格
    return val.replace(/[ ]/g, "");
}
//统一权限接口 true  false
var curNodeId = null;

function getCurNode() {
    return $('#appTree').tree('find', curNodeId);
}
var FK_Emp = "";
function hasAuthority() {
    var curNode = getCurNode();
    if (curNode) {
        if (curNode.attributes["authority"] == "no")
            return false; //不为空，但是没有权限
        return true; //唯一合法的情况
    }
    return false; //为空
}
//新增部门  
function append(deptSort) {
    if (hasAuthority()) {
        Application.data.appendData(deptSort, curNodeId, function (js, scope) {
            if (js == "false") {
                CC.Message.showError("部门提示", "操作失败！");
            } else {
                //加载列表
                curNodeId = js;
                LoadDeptTree();
            }
        }, this);
    }
}
//删除节点    如果选中点包含子节点---不允许删除
function deleteNode() {
    if (hasAuthority()) {
        $.messager.confirm('警告', '您确定删除吗？', function (r) {
            if (r) {
                var sonList = $('#appTree').tree('getChildren', getCurNode().target);
                if (sonList.length !== 0) {
                    CC.Message.showError("提示", "您选中的节点包含" + sonList.length + "个子节点，不可以删除！");
                    return;
                }

                Application.data.deleteNode(curNodeId, function (js, scope) {
                    if (js == "false") {
                        CC.Message.showError("操作提示", "操作失败！");
                    } else {
                        //加载列表
                        curNodeId = js;
                        LoadDeptTree();
                    }
                }, this);
            }
        });
    }
}
//上/下移  逻辑写到后台
function floatNode(v) {
    if (hasAuthority()) {
        Application.data.floatNode(v, curNodeId, function (js, scope) {
            if (js == "false") {
                CC.Message.showError("操作提示", "操作失败！");
            } else {
                //加载列表
                LoadDeptTree();
            }
        }, this);
    }
}
//查看部门信息
function checkDeptInfo() {
    if (hasAuthority()) {
        $('#deptInfoDialog').dialog({
            title: "编辑部门",
            width: 500,
            height: 500,
            closed: false,
            modal: true,
            iconCls: 'icon-rights',
            resizable: true
        });
        Application.data.checkDeptInfo(curNodeId, function (js, scope) {
            if (js != "false") {
                var pushData = eval('(' + js + ')');

                //绑定部门基本信息
                $("#dept_No").val(pushData.deptNo);
                $("#dept_Name").val(pushData.deptName);
                $("#dept_Leader").val(pushData.deptLeader);

                //绑定部门岗位
                $("#stationTree").tree({
                    data: pushData.deptStation,
                    collapsed: true,
                    lines: true
                });
                //绑定部门职务
                $("#deptDutyTree").tree({
                    data: pushData.deptDuty,
                    collapsed: true,
                    lines: true
                });
            } else {
                CC.Message.showError("操作提示", "操作失败！");
            }
        }, this);
    }
}
//保存编辑后的部门数据
function saveDeptInfo() {
    var deptName = $("#dept_Name").val();
    var deptLeader = $("#dept_Leader").val();

    var stationTreeNodes = $('#stationTree').tree('getChecked');
    var stationTreeNodesStr = "";
    for (var i = 0; i < stationTreeNodes.length; i++) {
        if (stationTreeNodes[i].attributes["isSonNode"] == "yes") {
            if (i == stationTreeNodes.length - 1) {
                stationTreeNodesStr += stationTreeNodes[i].id;
                continue;
            }
            stationTreeNodesStr += stationTreeNodes[i].id + ",";
        }
    }

    var deptDutyTreeNodes = $('#deptDutyTree').tree('getChecked');
    var deptDutyTreeNodesStr = "";
    for (var i = 0; i < deptDutyTreeNodes.length; i++) {
        if (deptDutyTreeNodes[i].attributes["isSonNode"] == "yes") {
            if (i == deptDutyTreeNodes.length - 1) {
                deptDutyTreeNodesStr += deptDutyTreeNodes[i].id;
                continue;
            }
            deptDutyTreeNodesStr += deptDutyTreeNodes[i].id + ",";
        }
    }

    Application.data.saveDeptInfo(curNodeId, deptName, deptLeader, stationTreeNodesStr, deptDutyTreeNodesStr, function (js, scope) {
        if (js == "true") {
            CC.Message.showError("操作提示", "保存成功！");
            deptInfoDialogClose();
            LoadDeptTree();
            return;
        } else {
            CC.Message.showError("操作提示", "保存失败！");
            LoadDeptTree();
            deptInfoDialogClose();
            return;
        }
    }, this);
}
//查询
function doSearch() {
    var selectedNode = $('#appTree').tree('getSelected');
    var searchVal = $("#cc").val();
    Application.data.doSearch(curNodeId, searchVal, function (js, scope) {
        if (js != "false") {
            var pushData = eval('(' + js + ')');

            $("#stationTree").tree({
                data: pushData,
                collapsed: true,
                lines: true
            });
        } else {
            CC.Message.showError("操作提示", "操作失败！");
        }
    }, this);
}
//半角转化为全角  qin
function meizz(str) {
    var tmp = '';
    for (var i = 0; i < str.length; i++) { tmp += String.fromCharCode(str.charCodeAt(i) + 65248) }
    return tmp
}
//字符的处理
function canEdit(str) {
    if (str.indexOf(',') != -1) {//表明含有,
        return true;
    }
}
//检查emp是否已经存在
function checkEmpNo() {
    var empNo = $('#empNo').val();
    empNo = replaceTrim(empNo);
    if (empNo) {
        Application.data.checkEmpNo(empNo, function (js, scope) {
            if (js == "true") {
                $.messager.alert("提示", "输入的帐号:" + empNo + "可以使用.", "info");
            } else {
                $.messager.alert("提示", "帐号:" + empNo + "已经存在！", "info");
            }
        }, this);
    }
    else {
        $.messager.alert("提示", "请输入...", "info");
    }
}
//执行修改
function doEdit() {
    if (hasAuthority()) {
        var setName = $("#setName").val();
        if (canEdit(setName)) return;

        var setZgbh = $("#setZgbh").val();
        if (canEdit(setZgbh)) return;

        var setZw = $('#setZw').combo('getValue');
        if (canEdit(setZw)) return;

        var setTel = $("#setTel").val();
        if (canEdit(setTel)) return;

        var setEamil = $("#setEamil").val();
        if (canEdit(setEamil)) return;

        var setLeader = $("#setLeader").val();
        if (canEdit(setLeader)) return;

        var setZwlb = $("#setZwlb").val();
        if (canEdit(setZwlb)) return;

        //职务类别必须是整数
        var re = /^-?\d+$/;
        if (!re.test(setZwlb)) {
            $.messager.alert("提示", "职务类别必须是整数!", "info");
            $("#setZwlb").focus();
            return;
        }

        if (replaceTrim(setName) == "") {
            $.messager.alert("提示", "姓名不可以为空!", "info");
            return;
        }
        var infoStr = setName + "," + setZgbh + "," + setZw + "," + setTel + ","
    + setEamil + "," + setLeader + "," + setZwlb;

        //获取树数据
        var stationTreeNodes = $('#empStationTree').tree('getChecked');
        var stationTreeNodesStr = "";
        for (var i = 0; i < stationTreeNodes.length; i++) {
            if (stationTreeNodes[i].attributes["isSonNode"] == "yes") {
                if (i == stationTreeNodes.length - 1) {
                    stationTreeNodesStr += stationTreeNodes[i].id;
                    continue;
                }
                stationTreeNodesStr += stationTreeNodes[i].id + ",";
            }
        }

        Application.data.editDeptEmp(curNodeId, infoStr, FK_Emp, stationTreeNodesStr, function (js, scope) {
            if (js == "true") {
                empInfoDialogClose();
                CC.Message.showError("部门提示", "保存成功！");
                refreshGrid();
            } else {
                CC.Message.showError("部门提示", "保存失败！");
                refreshGrid();
            }
        }, this);
    }
}
function initializeEmpTabs() {
    clearData();
    Application.data.getEmpInfo(curNodeId, FK_Emp, function (js, scope) {
        if (js) {
            var pushData = eval('(' + js + ')');

            $("#setName").val(pushData.setName);
            $("#setNo").val(pushData.setNo);
            $("#setZgbh").val(pushData.setZgbh);
            $("#setTel").val(pushData.setTel);
            $("#setEamil").val(pushData.setEamil);
            $("#setLeader").val(pushData.setLeader);
            $("#setZwlb").val(pushData.setZwlb);

            $('#setZw').combobox({
                data: pushData.setZw,
                valueField: 'id',
                textField: 'text',
                onSelect: function (r) {

                }
            });
            //加载部门目录
            $("#empStationTree").tree({
                data: pushData.yygw,
                collapsed: true,
                lines: true
            });
            $('#emptt').tabs('select', 0);
        }
    }, this);
}
//关联人员
function connecteEmp() {
    if (hasAuthority()) {
        Application.data.checkDeptDutyAndStation(curNodeId, function (js, scope) {
            if (js == "true") {
                $('#connecteEmp').dialog({
                    title: "关联人员",
                    width: 600,
                    height: 500,
                    closed: false,
                    modal: true,
                    iconCls: 'icon-rights',
                    resizable: true
                });
                $('#newsGrid').datagrid('clearChecked'); //必要，重置默认选中状态
                LoadGridData(1, 20);
            } else {
                checkDeptInfo();
                //CC.Message.showError("提示", "检查该部门职务和岗位是否健全!");
                return;
            }
        }, this);
    }
}
//加载关联人员grid
function LoadGridData(pageNumber, pageSize) {
    if (hasAuthority()) {
        Application.data.getOtherEmps(pageNumber, pageSize, curNodeId, function (js, scorp) {
            if (js) {
                var pushData = eval('(' + js + ')');
                $('#newsGrid').datagrid({
                    columns: [[
                    { field: 'NO', title: '编号', sortable: true, checkbox: true, align: 'left', width: 60 },
                    { field: 'NAME', title: '姓名', width: 60, align: 'center' },
                    { field: 'DEPTNAME', title: '部门', width: 100, align: 'center' }
                ]],
                    idField: 'No',
                    selectOnCheck: true,
                    checkOnSelect: true,
                    data: pushData,
                    width: 'auto',
                    height: 'auto',
                    striped: true,
                    rownumbers: true,
                    pagination: true,
                    remoteSort: false,
                    fitColumns: true,
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    pageList: [20, 30, 40, 50]
                });
                //分页
                var pg = $("#newsGrid").datagrid("getPager");
                if (pg) {
                    $(pg).pagination({
                        onRefresh: function (pageNumber, pageSize) {
                            LoadGridData(pageNumber, pageSize);
                        },
                        onSelectPage: function (pageNumber, pageSize) {
                            LoadGridData(pageNumber, pageSize);
                        }
                    });
                }
            }
        }, this);
    }
}
function glEmp() {
    var checkRows = $('#newsGrid').datagrid('getChecked');

    var empNoStr = "";
    for (var i = 0; i < checkRows.length; i++) {
        if (i == checkRows.length - 1) {
            empNoStr += checkRows[i].NO;
            continue;
        }
        empNoStr += checkRows[i].NO + ",";
    }
    if (empNoStr) {
        Application.data.glEmp(curNodeId, empNoStr, function (js, scope) {
            if (js == "true") {
                CC.Message.showError("部门提示", "操作成功！");
                $('#connecteEmp').dialog('close');
                LoadDataGridAdmin(1, 20);
            } else {
                CC.Message.showError("部门提示", "操作失败！");
                $('#connecteEmp').dialog('close');
                LoadDataGridAdmin(1, 20);
            }
        }, this);
    } else {
        $.messager.alert("提示", "没有需要关联的数据!", "info");
    }
}
//密码重置
function modifyPwd() {
    if (hasAuthority()) {
        $.messager.confirm('提示', '确定重置密码为"123"?', function (y) {
            if (y) {
                var empNo = $("#setNo").val();
                //执行删除
                Application.data.modifyPwd(empNo, function (js, scope) {
                    if (js == "true") {
                        CC.Message.showError("提示", "密码重置成功！");
                    } else {
                        CC.Message.showError("提示", "密码重置失败！");
                    }
                }, this);
            }
        });
    }
}
//查询
function searchEmp() {
    var searchText = $('#searchText').val();
    if (replaceTrim(searchText)) {
        LoadDataGridAdmin(1, 20);
    } else {
        CC.Message.showError("提示", "请输入查询文本");
        return;
    }
}
//检查该部门职务和岗位是否健全
function checkDeptDutyAndStation() {
    if (hasAuthority()) {
        Application.data.checkDeptDutyAndStation(curNodeId, function (js, scope) {
            if (js == "true") {
                AddPersonForApp();
            } else {
                checkDeptInfo();
                //CC.Message.showError("提示", "检查该部门职务和岗位是否健全!");
            }
        }, this);
    }
}
//清空缓存
function clearData() {
    $('#empName').val('');
    $('#empNo').val('');
    $('#zgbh').val('');
    $('#telephone').val('');
    $('#email').val('');
    $('#leader').val('');
    $('#zwSort').val('');
}
//加载添加人员界面
function AddPersonForApp() {
    if (hasAuthority()) {
        $('#deptEmpDialog').dialog({
            title: "人员信息",
            width: 520,
            height: 500,
            closed: false,
            modal: true,
            iconCls: 'icon-addG',
            resizable: true
        });
        clearData();
        getDutyStationDll();
    }
}
//新增人员
function addEmp() {
    var empName = $('#empName').val();
    var empNo = $('#empNo').val();
    if (empName == "" || empNo == "") {
        $.messager.alert("提示", "姓名和帐号不可以为空!", "info");
        return;
    }
    var re = /^-?\d+$/;
    if (!re.test($("#zwSort").val())) {
        $.messager.alert("提示", "职务类别必须是整数!", "info");
        $("#zwSort").focus();
        return;
    }

    //检查重名
    Application.data.checkEmpNo(replaceTrim(empNo), function (js, scope) {
        if (js == "true") {
            var infoStr = replaceTrim(empName) + "," + replaceTrim(empNo) + "," + $("#zgbh").val() + "," + $('#zw').combo('getValue') + "," + $("#telephone").val() + "," + $("#email").val() + "," + $("#leader").val() + "," + $("#zwSort").val();
            //获取新增人员选择的岗位
            var empStation = $('#gwDll').tree('getChecked');
            var empStationStr = "";
            for (var i = 0; i < empStation.length; i++) {
                if (empStation[i].attributes["isSonNode"] == "yes") {
                    if (i == empStation.length - 1) {
                        empStationStr += empStation[i].id;
                        continue;
                    }
                    empStationStr += empStation[i].id + ",";
                }
            }

            Application.data.saveDeptEmp(infoStr, curNodeId, empStationStr, function (js, scope) {
                if (js == "true") {
                    //加载列表
                    LoadDataGridAdmin(1, 20);
                    $('#deptEmpDialog').dialog("close");
                } else {
                    CC.Message.showError("部门提示", "保存失败！");
                }
            }, this);
        } else {
            $.messager.alert("提示", "帐号:" + empNo + "已经存在！", "info");
            return;
        }
    }, this);
}
//新增人员时加载岗位，职务信息
function getDutyStationDll() {
    if (hasAuthority()) {
        Application.data.getDutyStationDllInfo(curNodeId, function (js, scope) {
            if (js) {
                var pushData = eval('(' + js + ')');

                $('#zw').combobox({
                    data: pushData.zwDll,
                    valueField: 'id',
                    textField: 'text'
                });
                $("#gwDll").tree({
                    data: pushData.gwDll,
                    collapsed: true,
                    lines: true
                });
            }
        }, this);

        $('#empName').focus();
    }
}
//刷新
function refreshGrid() {
    LoadDataGridAdmin(1, 20);
}

function EditEmpApp() {
    var rowData = $('#empAppGrid').datagrid('getSelected');

    if (rowData == null) {
        $.messager.alert("提示", "您没有选中数据!");
        return;
    }

    $('#empInfo').dialog({
        title: "人员信息",
        width: 520,
        height: 500,
        closed: false,
        modal: true,
        iconCls: 'icon-edit',
        resizable: true
    });

    FK_Emp = rowData.NO;

    //初始化
    initializeEmpTabs();
}

//删除
function DeleteEmpApp() {
    var rows = $('#empAppGrid').datagrid('getChecked');

    if (rows.length != 0) {
        $.messager.confirm('警告', '确定删除选中数据?', function (y) {
            if (y) {
                var emps = "";
                $.each(rows, function (i, row) {
                    emps += row.NO + ","
                });
                //执行删除
                Application.data.deleteDeptEmp(curNodeId, emps, function (js, scope) {
                    //加载列表
                    LoadDataGridAdmin(1, 20);
                }, this);
            }
        });
    }
    else {
        $.messager.alert("提示", "您没有选中数据!");
    }
    empInfoDialogClose();
}
//展示部门树
function showDeptTreeCallBack(js, scope) {
    if (js == "") js = [];
    var pushData = eval('(' + js + ')');

    //加载部门目录  qinqin
    $("#appTree").tree({
        data: pushData,
        collapsed: true,
        lines: true,
        onClick: function (node) {
            if (curNodeId != node.id) {
                $("#appTree").tree("expand", node.target);
                deptNodeTreeAction(node);
            }
        },
        onContextMenu: function (e, node) {
            if (curNodeId != node.id)
                deptNodeTreeAction(node);

            e.preventDefault();
            $(this).tree('select', node.target);
            $('#mm').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        },
        onDblClick: function (node) {
            if (curNodeId != node.id) {
                $("#appTree").tree("expand", node.target);
                deptNodeTreeAction(node);
            }
        }
    });
    loadRoot()
    //关闭等待页面
    $("#pageloading").hide();
}
function deptNodeTreeAction(node) {
    $('#appTree').tree('select', node.target);
    curNodeId = node.id;
    $('#searchText').val('');

    LoadDataGridAdmin(1, 20);
}
//禁用添加按钮
function DisableAddBtn() {
    $('#addEmpApp').linkbutton('disable');
}
//启用enable
function EnableAddBtn() {
    $('#addEmpApp').linkbutton('enable');
}

//排序
var sortAgain = 0;
var orderBy = '';
function LoadGridOrderBy(lbtn) {
    var isASC = sortAgain % 2 != 0;
    orderBy = isASC ? "Name ASC" : "Name DESC";
    $(lbtn).linkbutton({ iconCls: isASC ? "icon-downG" : "icon-upG", text: isASC ? "按姓名降序" : "按姓名升序" });
    sortAgain += 1;

    LoadDataGridAdmin(1, 20);
}

//加载人员列表
function LoadDataGridAdmin(pageNumber, pageSize) {
    $('#empAppGrid').datagrid('loadData', { total: 0, rows: [] }); //清空下方DateGrid

    var searchText = $('#searchText').val();
    Application.data.LoadDataGridDeptEmp(curNodeId, orderBy, replaceTrim(searchText), pageNumber, pageSize, function (js, scope) {
        if (js) {
            if (js == "") js = [];
            var pushData = eval('(' + js + ')');

            $('#empAppGrid').datagrid({
                data: pushData,
                width: 'auto',
                rownumbers: true,
                pagination: true,
                singleSelect: true,
                selectOnCheck: false,
                loadMsg: '数据加载中......',
                pageNumber: pageNumber,
                pageSize: pageSize,
                pageList: [20, 30, 40, 50],
                columns: [[
                       { checkbox: true },
                       { field: 'NO', title: '编号', sortable: true, align: 'left', width: 100 },
                       { field: 'NAME', title: '姓名', sortable: true, width: 200, align: 'center' },
                       { field: 'EMPNO', title: '员工编号', sortable: true, width: 160, align: 'center' },
                       { field: 'TEL', title: '电话', sortable: true, width: 160, align: 'center' },
                       { field: 'EMAIL', title: '邮箱', sortable: true, width: 160, align: 'center' },
                       { field: 'FK_DUTY', title: '职务', sortable: true, width: 160, align: 'center' }
                       ]],

                onDblClickRow: function (rowIndex, rowData) {
                    $('#empInfo').dialog({
                        title: "人员信息",
                        width: 520,
                        height: 500,
                        closed: false,
                        modal: true,
                        iconCls: 'icon-edit',
                        resizable: true
                    });
                    FK_Emp = rowData.NO;

                    //初始化
                    initializeEmpTabs();
                },
                onRowContextMenu: function (e, rowIndex, rowData) {
                    e.preventDefault();

                    if (!rowData) return;

                    $("#empAppGrid").datagrid('selectRow', rowIndex);
                    $("#mEmp").menu('show', {
                        left: e.pageX,
                        top: e.pageY
                    });
                }
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

//加载菜单
function LoadDeptTree() {
    Application.data.getOrganizationTreeForAdmin(0, showDeptTreeCallBack, this);
}

//初始化
$(function () {
    deptInfoDialogClose();
    $('#deptEmpDialog').dialog("close");
    $('#empInfo').dialog("close");
    $('#connecteEmp').dialog("close");
    $("#pageloading").show();

    //加载菜单
    LoadDeptTree();
});
//窗体操作
function deptInfoDialogClose() {
    $('#deptInfoDialog').dialog("close");
}
function deptEmpDialogClose() {
    $('#deptEmpDialog').dialog("close");
}
function glryDialogClose() {
    $('#connecteEmp').dialog("close");
}
function empInfoDialogClose() {
    $('#empInfo').dialog("close");
}
//默认显示根节点信息
function loadRoot() {
    var node;
    if (curNodeId) {
        node = $('#appTree').tree('find', curNodeId);
    }
    else {
        node = $('#appTree').tree('getRoot');
        curNodeId = node.id;
    }

    $('#appTree').tree('select', node.target);
    $('#appTree').tree('expandTo', node.target);
    $('#appTree').tree('expand', node.target);

    $('#searchText').val('');
    LoadDataGridAdmin(1, 20);
}
