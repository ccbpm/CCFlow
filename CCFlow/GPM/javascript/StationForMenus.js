//初始化
$(function () {
    //获取岗位信息
    Application.data.getStations(StationsCallBack, this);
    //加载菜单
    $("#lbStation").bind('contextmenu', function (e) {
        e.preventDefault();
        $('#mm1').menu('show', {
            left: e.pageX,
            top: e.pageY
        });
    });
    //查询 事件 
    $('#QueryStation').bind('click', function () {
        QueryStation();
    });
    //保存 事件
    $('#saveRight').bind('click', function () {
        SaveStationOfMenus();
    });
});
//加载所有岗位
function StationsCallBack(js, scope) {
    if (js == "") js = "[]";
    var pushData = eval('(' + js + ')');
    var objSelect = document.getElementById("lbStation");
    if (pushData.length > 0) {
        $.each(pushData, function (index, val) {
            var varItem = new Option("[" + val.No + "]" + val.Name, val.No);
            objSelect.options.add(varItem);
        });
    }
    $("#pageloading").hide();
}

//所选岗位发生变化时
function LoadMenusOnStationChange() {
    //加载菜单
    var selectIndex = document.getElementById("lbStation").selectedIndex;
    var options = document.getElementById("lbStation").options;
    if (selectIndex >= 0) {
        $("#pageloading").show();
        Application.data.getStationOfMenusByNo(options[selectIndex].value, 1000, 'false', LoadMenuTreeCallBack, this);
    }
}
//加载菜单
function LoadMenuTreeCallBack(js, scope) {
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
        onCheck: function (node) {
            if (!treeIsLoading) {
                var pasteSelectIndex = document.getElementById("lbStation").selectedIndex;
                if (pasteSelectIndex >= 0) {
                    $('#saveRight').linkbutton('enable');
                }
            }
        },
        onLoadSuccess: function () {
            treeIsLoading = false;
        },
        onExpand: function (node) {
            if (node) {
                StationMenuChildNodes(node, false);
            }
        },
        onClick: function (node) {
            if (node) {
                StationMenuChildNodes(node, true);
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
//加载所选菜单节点的子节点
function StationMenuChildNodes(node, expand) {
    var childNodes = $('#menuTree').tree('getChildren', node.target);
    if (childNodes && childNodes.length > 0 && childNodes[0].text == '加载中...') {
        $('#menuTree').tree('remove', childNodes[0].target);
        var selectIndex = document.getElementById("lbStation").selectedIndex;
        var options = document.getElementById("lbStation").options;
        if (selectIndex >= 0) {
            Application.data.getStationOfMenusByNo(options[selectIndex].value, node.id, 'true', function (js) {
                if (js && js != '[]') {
                    var pushData = eval('(' + js + ')');
                    $('#menuTree').tree('append', { parent: node.target, data: pushData });
                    if (expand) $('#menuTree').tree('expand', node.target);
                }
            }, this);
        }
    }
}
// 查询 事件
function QueryStation() {
    var objSearch = $('#searchContent').val();
    //清空
    document.getElementById("lbStation").length = 0;
    $("#pageloading").show();
    Application.data.getStationByName(encodeURI(objSearch), function (js, scope) {
        if (js == "") js = "[]";
        var pushData = eval('(' + js + ')');
        var objSelect = document.getElementById("lbStation");
        if (pushData.length > 0) {
            $.each(pushData, function (index, val) {
                var varItem = new Option("[" + val.No + "]" + val.Name, val.No);
                objSelect.options.add(varItem);
            });
        }
        $("#pageloading").hide();
    }, this);
}
//保存 事件
function SaveStationOfMenus() {
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
            for (var i = 0; i < nodes.length; i++) {
                var nodeText = nodes[i].text;
                if (nodeText == "加载中...") {
                    continue;
                }
                if (menuIds != '')
                    menuIds += ',';
                menuIds += nodes[i].id;
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
        var selectIndex = document.getElementById("lbStation").selectedIndex;
        var options = document.getElementById("lbStation").options;
        if (selectIndex >= 0) {
            //保存数据
            Application.data.saveStationOfMenus(options[selectIndex].value, menuIds, menuIdsUn, menuIdsUnExt, function (js, scope) {
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
//复制权限到其他岗位
var copySelectIndex = null;
function CopyRight() {
    copySelectIndex = document.getElementById("lbStation").selectedIndex;
    if (copySelectIndex >= 0) {
        var options = document.getElementById("lbStation").options;
        $("#copyMsg").text("复制了：" + options[copySelectIndex].text + " 的权限");
        Application.data.getStations(StationCallBack, this);
    } else {
        CC.Message.showError("提示", "请选择复制对象。");
    }
}

//加载需要复制的岗位
function StationCallBack(js, scope) {
    if (js == "") js = "[]";
    var pushData = eval('(' + js + ')');
    var title = $("#copyMsg").text();

    var options = document.getElementById("lbStation").options;
    var copyNo = options[copySelectIndex].value;
    var treeData = "[{ id:'0', text: '岗位', state: 'open', children:[";
    var children = "";
    if (pushData.length > 0) {
        $.each(pushData, function (index, val) {
            if (val.No != copyNo) {
                if (children.length > 0) children += ",";
                children += "{ id:'" + val.No + "', text:'" + val.Name + "', iconCls: 'icon-seasons'}";
            }
        });
    }
    treeData += children;
    treeData += "]}]";
    pushData = eval('(' + treeData + ')');
    //加载
    $("#StationTree").tree({
        data: pushData,
        iconCls: 'tree-folder',
        checkbox: true,
        dnd: false,
        onClick: function (node) {
            if (node) {
                if (node.checked == true) {
                    $('#StationTree').tree('uncheck', node.target);
                } else {
                    $('#StationTree').tree('check', node.target);
                }
            }
        }
    });
    //弹出窗体
    $('#StationDialog').dialog({
        title: title,
        width: 500,
        height: 530,
        closed: false,
        modal: true,
        iconCls: 'icon-rights',
        resizable: true,
        toolbar: [{
            text: '清空式复制',
            iconCls: 'icon-save-close',
            handler: function () {
                var nodes = $('#StationTree').tree('getChecked');
                var pastStationNos = "";
                for (var i = 0; i < nodes.length; i++) {
                    if (pastStationNos != "")
                        pastStationNos += ",";
                    pastStationNos += nodes[i].id;
                }
                if (pastStationNos == "") {
                    CC.Message.showError("系统提示", "请选择岗位！");
                    return;
                }
                //获取复制对象
                var options = document.getElementById("lbStation").options;
                var copyNo = options[copySelectIndex].value;
                //进行保存
                Application.data.clearOfCopyStation(copyNo, pastStationNos, function (js, scope) {
                    if (js == "success") {
                        CC.Message.showError("系统提示", "授权成功！");
                    } else {
                        CC.Message.showError("系统提示", "授权失败！" + js);
                    }
                    $('#StationDialog').dialog("close");
                }, this);

            }
        }, '-', {
            text: '覆盖式复制',
            iconCls: 'icon-save-close',
            handler: function () {
                var nodes = $('#StationTree').tree('getChecked');
                var pastStationNos = "";
                for (var i = 0; i < nodes.length; i++) {
                    if (pastStationNos != "")
                        pastStationNos += ",";
                    pastStationNos += nodes[i].id;
                }
                if (pastStationNos == "") {
                    CC.Message.showError("系统提示", "请选择岗位！");
                    return;
                }
                //获取复制对象
                var options = document.getElementById("lbStation").options;
                var copyNo = options[copySelectIndex].value;
                //进行保存
                Application.data.coverOfCopyStation(copyNo, pastStationNos, function (js, scope) {
                    if (js == "success") {
                        CC.Message.showError("系统提示", "授权成功！");
                    } else {
                        CC.Message.showError("系统提示", "授权失败！" + js);
                    }
                    $('#StationDialog').dialog("close");
                }, this);

            }
        }]
    });
}