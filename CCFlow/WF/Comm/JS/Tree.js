if ($.fn.datagrid) {
    $.fn.datagrid.defaults.loadMsg = '正在处理，请稍待。。。';
}
if ($.fn.treegrid && $.fn.datagrid) {
    $.fn.treegrid.defaults.loadMsg = $.fn.datagrid.defaults.loadMsg;
}
if ($.messager) {
    $.messager.defaults.ok = '确定';
    $.messager.defaults.cancel = '取消';
}
function getArgsFromHref(sArgName) {
    var sHref = window.location.href;
    var args = sHref.split("?");
    var retval = "";
    if (args[0] == sHref) /*参数为空*/
    {
        return retval; /*无需做任何处理*/
    }
    var str = args[1];
    args = str.split("&");
    for (var i = 0; i < args.length; i++) {
        str = args[i];
        var arg = str.split("=");
        if (arg.length <= 1) continue;
        if (arg[0] == sArgName) retval = arg[1];
    }
    while (retval.indexOf('#') >= 0) {
        retval = retval.replace('#', '');
    }
    return retval;
}

var ensName = '';
var parentNo = 0;
//加载节点树
function LoadTreeNodes() {
    ensName = getArgsFromHref("EnsName");
    parentNo = getArgsFromHref("ParentNo");
    //实体名
    if (ensName == '') {
        $("body").html("<b style='color:red;'>请传入正确的参数名。如：Tree.aspx?EnsName=BP.Port.Depts<br/>主意：如果根节点ParentNo不为0，需传入根节点ParentNo的值.<b>");
        return;
    }
    //父编号
    if (parentNo == '') {
        parentNo = 0;
    }

    $("#pageloading").show();
    var params = {
        method: "gettreenodes",
        EnsName: ensName,
        ParentNo: parentNo,
        isLoadChild: true
    };
    queryData(params, function (js, scope) {
        $("#pageloading").hide();
        //系统错误
        if (js.readyState && js.readyState == 4 && js.readyState == 0) js = "[]";
        //系统错误
        if (js.status && js.status == 500) {
            $("body").html(js.responseText);
            return;
        }
        //捕获错误
        if (js.indexOf("error:") > -1) {
            $("body").html("<br/><b style='color:red;'>" + js + "<b>");
            return;
        }
        var pushData = eval('(' + js + ')');
        //加载类别树
        $("#enTree").tree({
            data: pushData,
            iconCls: 'tree-folder',
            collapsed: true,
            lines: true
        });
        $("#enTree").bind('contextmenu', function (e) {
            e.preventDefault();
            $('#treeMM').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        });
    }, this);
}

//树节点操作
function treeNodeManage(dowhat, nodeNo, callback, scope) {
    var params = {
        method: "treesortmanage",
        EnsName: ensName,
        dowhat: dowhat,
        nodeNo: nodeNo
    };
    queryData(params, callback, scope);
}

//创建同级目录
function CreateSampleNode() {
    var node = $('#enTree').tree('getSelected');
    if (node) {
        treeNodeManage("sample", node.id, function (js) {
            if (js) {
                var parentNode = $('#enTree').tree('getParent', node.target);
                var pushData = eval('(' + js + ')');
                $('#enTree').tree('append', {
                    parent: (parentNode ? parentNode.target : null),
                    data: [{
                        id: pushData.No,
                        text: pushData.Name,
                        iconCls: 'tree_folder'
                    }]
                });
            }

        }, this);
    } else {
        $.messager.alert('提示', '请选择节点！', 'info');
    }
}
//创建下级目录
function CreateSubNode() {
    var node = $('#enTree').tree('getSelected');
    if (node) {
        treeNodeManage("children", node.id, function (js) {
            if (js) {
                var pushData = eval('(' + js + ')');
                $('#enTree').tree('append', {
                    parent: (node ? node.target : null),
                    data: [{
                        id: pushData.No,
                        text: pushData.Name,
                        iconCls: 'tree_folder'
                    }]
                });
            }

        }, this);
    } else {
        $.messager.alert('提示', '请选择节点！', 'info');
    }
}

//修改
function EditNode() {
    var node = $('#enTree').tree('getSelected');
    if (node) {
        var enName = $("#enName").val();
        if (enName == "") {
            $.messager.alert('提示', '没有找到类名！', 'info');
            return;
        }
        var url = "UIEn.aspx?EnName=" + enName + "&PK=" + node.id;
        window.showModalDialog(url, '编辑', 'dialogHeight: 650px; dialogWidth: 850px; dialogTop: 100px; dialogLeft: 150px; scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes; help: no');


        var params = {
            method: "gettreenodename",
            EnsName: ensName,
            nodeNo: node.id
        };

        queryData(params, function (js) {
            if (js != "") {
                $('#enTree').tree('update', { target: node.target, text: js });
            }
        }, this);
    } else {
        $.messager.alert('提示', '请选择节点！', 'info');
    }
}

//删除节点
function DeleteNode() {
    if (!confirm("是否真的需要删除?"))
        return;
    var node = $('#enTree').tree('getSelected');
    if (node) {
        //删除
        treeNodeManage("delete", node.id, function (js) {
            $('#enTree').tree('remove', node.target);
        }, this);
    } else {
        $.messager.alert('提示', '请选择节点。', 'info');
    }
}

//上移
function DoUp() {
    var node = $('#enTree').tree('getSelected');
    if (node) {
        treeNodeManage("doup", node.id, function (js) {
            LoadTreeNodes();
            $('#enTree').tree('expandAll');
        }, this);
    } else {
        $.messager.alert('提示', '请选择节点。', 'info');
    }
}
//下移
function DoDown() {
    var node = $('#enTree').tree('getSelected');
    if (node) {
        treeNodeManage("dodown", node.id, function (js) {
            LoadTreeNodes();
            $('#enTree').tree('expandAll');
        }, this);
    } else {
        $.messager.alert('提示', '请选择节点。', 'info');
    }
}

//公共方法
function queryData(param, callback, scope, method, showErrMsg) {
    if (!method) method = 'GET';
    $.ajax({
        type: method, //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: "Tree.aspx", //要访问的后台地址
        data: param, //要发送的数据
        async: false,
        cache: false,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            callback(XMLHttpRequest);
        },
        success: function (msg) {//msg为返回的数据，在这里做数据绑定
            var data = msg;
            callback(data, scope);
        }
    });
}
//3秒后加载
setTimeout("LoadTreeNodes()", 3000);