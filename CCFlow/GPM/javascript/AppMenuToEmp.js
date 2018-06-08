var curModel = "emp";
//加载菜单
function showMenusTree(js, scope) {
    if (js == "") js = [];
    var pushData = eval('(' + js + ')');

    //加载系统目录
    $("#menuTree").tree({
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

    $("#pageloading").hide();
}
//加载子节点
function LoadMenuChildNodes(node, expand) {
    var childNodes = $('#menuTree').tree('getChildren', node.target);
    if (childNodes && childNodes.length > 0 && childNodes[0].text == '加载中...') {
        $('#menuTree').tree('remove', childNodes[0].target);
        Application.data.getMenusOfMenuForEmp(node.id, "true", function (js) {
            if (js && js != '[]') {
                var pushData = eval('(' + js + ')');
                $('#menuTree').tree('append', { parent: node.target, data: pushData });
                if (expand) $('#menuTree').tree('expand', node.target);
            }
            GetTemplatePanel(node);
        }, this);
    } else {
        GetTemplatePanel(node);
    }
}

//获取菜单数据
function LoadMenuTree() {
    Application.data.getMenusOfMenuForEmp(1000, "false", showMenusTree, this);
}

//何种模式分配权限
function DistributeRight(model) {
    curModel = model;
    if (curModel == "group") {
        $("#curModelText").html("当前选择按权限组分配权限");
    }
    if (curModel == "station") {
        $("#curModelText").html("当前选择按岗位分配权限");
    }
    if (curModel == "emp") {
        $("#curModelText").html("当前选择按人员分配权限");
    }
    var selected = $('#menuTree').tree('getSelected');
    //菜单如果有选择项就刷新
    if (selected) GetTemplatePanel(selected);
}


//获取菜单的权限
function GetTemplatePanel(node) {
    if (node) {
        Application.data.getTemplateData(node.id, curModel, function (js, scope) {
            if (js) {
                var panel = document.getElementById('templatePanel');
                panel.innerHTML = '正在加载 ......';
                if (js.status == 500) {
                    panel.innerHTML = '加载数据出错';
                    return;
                }
                var data = eval('(' + js + ')');
                try { eval(data); } catch (e) { alert('Data Format Error!'); }
                //默认按人员分配菜单模版
                var template = " <#macro userlist data> "
                             + " <#list data.bmList as bmList> "
                             + " <caption><b style='color:blue;'>${bmList.NAMEOFPATH}</b></caption><hr>"
                             + " <#list data.empList as empList> "
                             + " <#if (empList.FK_DEPT==bmList.NO)>"
                             + "   <#if (empList.ISCHECK==1)>"
                             + "       <input type='checkbox' checked='true'  name='ckgroup'  id='${empList.NO}'> ${empList.NAME}"
                             + "   <#else>"
                             + "       <input type='checkbox'   name='ckgroup'  id='${empList.NO}'> ${empList.NAME}"
                             + "   </#if>"
                             + " </#if>"
                             + " </#list> "
                             + " <br><br> "
                             + " </#list>"
                             + " </#macro>";
                //按岗位分配模版
                if (curModel == "station") {
                    template = " <#macro stationlist data> "
                             + " <#list data.station as stationlist> "
                             + " <#if (stationlist.ISCHECK==1)>"
                             + "     <input type='checkbox' checked='true' name='ckgroup'  id='${stationlist.NO}'> ${stationlist.NAME} <hr>"
                             + " <#else>"
                             + "     <input type='checkbox' name='ckgroup' id='${stationlist.NO}'> ${stationlist.NAME} <hr>"
                             + " </#if>"
                             + " </#list>"
                             + " </#macro>";
                }
                //按权限组分配模版
                if (curModel == "group") {
                    template = " <#macro grouplist data> "
                             + " <#list data.group as groupList> "
                             + " <#if (groupList.ISCHECK==1)>"
                             + "     <input type='checkbox' checked='true' name='ckgroup'  id='${groupList.NO}'> ${groupList.NAME} <hr>"
                             + " <#else>"
                             + "     <input type='checkbox' name='ckgroup' id='${groupList.NO}'> ${groupList.NAME} <hr>"
                             + " </#if>"
                             + " </#list>"
                             + " </#macro>";
                }
                //得到内容
                var source = easyTemplate(template, data);
                panel.innerHTML = source;
            }
        }, this);
    }
}

//保存选择项
function SaveMenuForEmp() {
    var saveNos = "";
    var selected = $('#menuTree').tree('getSelected');
    var ckgroup = document.getElementsByName("ckgroup");

    if (selected) {
        //获取选择项
        for (var i = 0; i < ckgroup.length; i++) {
            if (ckgroup[i].checked) {
                saveNos += ckgroup[i].id + ",";
            }
        }

        if (saveNos.length > 0)
            saveNos = saveNos.substring(0, saveNos.length - 1);

        var childNodes = $('#menuTree').tree('getChildren', selected.target);
        if (childNodes && childNodes.length > 0) {
            //消息提醒
            $.messager.confirm('提示', "所选菜单下含有子菜单，是否对子菜单进行授权？", function (r) {
                if (r) {
                    SaveMenuData(selected.id, saveNos, true);
                }
                else {
                    SaveMenuData(selected.id, saveNos, false);
                }
            });
        }
        else {
            SaveMenuData(selected.id, saveNos, false);
        }

    } else {
        CC.Message.showError("系统提示", "请选择菜单！");
    }
}
function SaveMenuData(menuNo, ckNos, saveChildNode) {
    $("#pageloading").show();
    //保存菜单权限
    Application.data.saveMenuForEmp(menuNo, ckNos, curModel, saveChildNode, function (js, scope) {
        if (js) {
            CC.Message.showError("系统提示", "保存成功！");
        }
        $("#pageloading").hide();
    }, this);
}
//全选
function CheckedAll() {
    var checkedSta = false;
    var ckbAll = document.getElementById("ckbAllText");

    var selected = $('#menuTree').tree('getSelected');
    //菜单如果没有选择项就返回
    if (!selected) return;

    if (ckbAll.innerHTML == "全选") {
        checkedSta = true;
        ckbAll.innerHTML = "清空";
    } else {
        ckbAll.innerHTML = "全选";
    }
    var ckgroup = document.getElementsByName("ckgroup");
    for (var i = 0; i < ckgroup.length; i++) {
        ckgroup[i].checked = checkedSta;
    }
}

//初始化
$(function () {
    $("#pageloading").show();
    //加载菜单
    LoadMenuTree();
});