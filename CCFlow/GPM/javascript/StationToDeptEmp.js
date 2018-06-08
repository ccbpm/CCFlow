//获取岗位类型数据
function LoadStationTypeTree() {
    Application.data.generStationTree(showStationsTree, this);
}

//加载岗位
function showStationsTree(js, scope) {
    if (js == "") js = [];
    var pushData = eval('(' + js + ')');

    //加载系统目录
    $("#StationsTree").tree({
        data: pushData,
        iconCls: 'tree-folder',
        collapsed: true,
        lines: true,
        onExpand: function (node) {
            if (node) {

            }
        },
        onClick: function (node) {
            if (node && node.iconCls == "icon-accept") {
                GetTemplatePanel();
            }
        }
    });
    $("#pageloading").hide();
}

//获取岗位的权限
function GetTemplatePanel() {
    var node = $('#StationsTree').tree('getSelected');
    
    if (hasPower(node)) {
        var FK_Dept = $('#DDL_DeptTree').combotree('getValue')
        var viewModel = "0";
        if ($("#CB_ViewModel").attr("checked")) {
            viewModel = "1";
        }
        
        if (node) {
            Application.data.getDeptEmpStationTemplateData(node.id, FK_Dept, viewModel, function (js, scope) {
                if (js) {
                    var panel = document.getElementById('templatePanel');
                    panel.innerHTML = '正在加载 ......';
                    if (js.status == 500) {
                        panel.innerHTML = '加载数据出错';
                        return;
                    }
                    var data = eval('(' + js + ')');
                    try { eval(data); } catch (e) { alert('Data Format Error!'); }
                    //默认按人员分配岗位模版
                    var template = " <#macro userlist data> "
                             + " <#list data.bmList as bmList> "
                             + " <caption><b style='color:blue;'> "
                             + " <#if (bmList.NAMEOFPATH=='')>"
                             + "        ${bmList.NAME}</b></caption><hr>"
                             + "   <#else>"
                             + "        ${bmList.NAMEOFPATH}</b></caption><hr>"
                             + " </#if>"
                             + " <#list data.empList as empList> "
                             + " <#if (empList.FK_DEPT==bmList.NO)>"
                             + "   <#if (empList.ISCHECK==1)>"
                             + "       <input type='checkbox' checked='true'  name='ckgroup'  id='${empList.FK_DEPT}@${empList.NO}'> ${empList.NAME}"
                             + "   <#else>"
                             + "       <input type='checkbox'   name='ckgroup'  id='${empList.FK_DEPT}@${empList.NO}'> ${empList.NAME}"
                             + "   </#if>"
                             + " </#if>"
                             + " </#list> "
                             + " <br><br> "
                             + " </#list>"
                             + " </#macro>";

                    //得到内容
                    var source = easyTemplate(template, data);
                    panel.innerHTML = source;
                    if (data.bmList && data.bmList.length == 0) {
                        panel.innerHTML = '无相符数据';
                    }
                }
            }, this);
        }
    }
}

//保存选择项
function SaveStationForEmp(IsClearSave) {
    var saveNos = "";
    var selected = $('#StationsTree').tree('getSelected');
    var deptTree = $('#DDL_DeptTree').combotree('tree');
    var treeChecked = deptTree.tree('getSelected');
    if (treeChecked) {
        var dept = treeChecked.id;
        if (hasPower(selected)) {
            if (hasPower(treeChecked)) {
                var ckgroup = document.getElementsByName("ckgroup");
                if (selected) {
                    //获取选择项
                    for (var i = 0; i < ckgroup.length; i++) {
                        if (ckgroup[i].checked) {
                            saveNos += ckgroup[i].id + ",";
                        }

                    }
                    //组织选择数据
                    if (saveNos.length > 0)
                        saveNos = saveNos.substring(0, saveNos.length - 1);

                    Application.data.saveStationForDeptEmps(selected.id, dept, saveNos, IsClearSave, function (scorp) {
                        if (scorp == "true") {
                            CC.Message.showMsg("系统提示", "保存成功！");
                            GetTemplatePanel();
                        } else {
                            CC.Message.showError("系统提示", "保存失败，请刷新后重试！");
                        }
                    }, this);
                } else {
                    CC.Message.showError("系统提示", "请选择菜单！");
                }
            }
        }
    }
    else {
        $.messager.alert("警告", "请选择部门！", "warning"); return false;
    }
}
function hasPower(ck) {
    if (ck) {
        if (ck.attributes["authority"] == "no") {
            $.messager.alert("警告", "您没有权限处理当前操作！", "warning"); return false;
        }  //不为空，但是没有权限
        else {
            return true;
        } //唯一合法的情况
    } else {
        $.messager.alert("警告", "您没有权限处理当前操作！", "warning");
        return false;
    }  //为空
}
//全选
function CheckedAll() {
    var checkedSta = false;
    var ckbAll = document.getElementById("ckbAllText");
    var selected = $('#StationsTree').tree('getSelected');
    //岗位如果没有选择项就返回
    if (!selected) return;

    if (ckbAll.innerHTML == "全选") {
        checkedSta = true;
        ckbAll.innerHTML = "清空";
    } else {
        ckbAll.innerHTML = "全选";
    }
    var ckgroup = document.getElementsByName("ckgroup");
    for (var i = 0; i < ckgroup.length; i++) {
        var ck = $('#StationsTree').tree('find', ckgroup[i]);
        if (hasPower(ck)) {
            ckgroup[i].checked = checkedSta;
        }
    }
}

//初始化
$(function () {
    $("#pageloading").show();
    //加载岗位
    LoadStationTypeTree();
    //部门树
    Application.data.getOrganizationTreeForAdmin("0", function (scorp) {
        var pushData = eval('(' + scorp + ')');
        $('#DDL_DeptTree').combotree({
            lines: true,
            data: pushData,
            onLoadSuccess: function () {

            },
            onClick: function (node) {
                if (node) {
                    GetTemplatePanel();
                }
            }
        });
    }, this);
});