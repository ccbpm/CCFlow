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

var treeEnsName = '';
var ensName = '';
var parentNo = 0;
//加载节点树
function LoadTreeNodes() {
    treeEnsName = getArgsFromHref("TreeEnsName");
    parentNo = getArgsFromHref("ParentNo");
    //实体名
    if (treeEnsName == '') {
        $("body").html("<b style='color:red;'>请传入正确的参数名。如：TreeEns.aspx?TreeEnsName=BP.Port.Depts&EnsName=BP.Port.Emps&RefPK=FK_Dept<br/>主意：如果根节点ParentNo不为0，需传入根节点ParentNo的值.<b>");
        return;
    }
    //父编号
    if (parentNo == '') {
        parentNo = 0;
    }

    $("#pageloading").show();
    var params = {
        method: "gettreenodes",
        TreeEnsName: treeEnsName,
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
            lines: true,
            onClick: function (node) {
                if (node) {
                    LoadGridData();
                }
            }
        });
    }, this);
}

//加载右侧数据
function LoadGridData() {
    ensName = getArgsFromHref("EnsName");
    var RefPK = getArgsFromHref("RefPK");
    var node = $('#enTree').tree('getSelected');
    if (node) {
        var params = {
            method: "getensgriddata",
            EnsName: ensName,
            RefPK: RefPK,
            FK: node.id
        };
        queryData(params, function (js, scope) {
            if (js) {
                if (js == "") js = "[]";

                //系统错误
                if (js.status && js.status == 500) {
                    $(".datagrid-view").html("<b style='color:red;'>请传入正确的参数名。如：TreeEns.aspx?TreeEnsName=BP.Port.Depts&EnsName=BP.Port.Emps&RefPK=FK_Dept<br/>主意：如果根节点ParentNo不为0，需传入根节点ParentNo的值.<b>");
                    return;
                }
                var pushData = eval('(' + js + ')');
                var fitColumns = true;
                if (pushData.columns.length > 6) {
                    fitColumns = false;
                }
                $('#ensGrid').datagrid({
                    columns: [pushData.columns],
                    data: pushData.data,
                    width: 'auto',
                    height: 'auto',
                    striped: true,
                    rownumbers: true,
                    singleSelect: true,
                    pagination: true,
                    remoteSort: false,
                    fitColumns: fitColumns,
                    pageSize: 10,
                    pageList: [10, 15, 20, 50],
                    onDblClickCell: function (index, field, value) {
                        EditEntityForm();
                    },
                    toolbar: [{ 'text': '新建', 'iconCls': 'icon-new', 'handler': 'CreateEntityForm' }, { 'text': '修改', 'iconCls': 'icon-config', 'handler': 'EditEntityForm'}],
                    loadMsg: '数据加载中......'
                });
            }
        }, this);
    } else {
        $.messager.alert('提示', '请选择节点！', 'info');
    }
}
//新建页面
function CreateEntityForm() {
    var enName = $("#enName").val();
    var RefPK = getArgsFromHref("RefPK");
    var node = $('#enTree').tree('getSelected');
    if (node) {
        var PK = $("#enPK").val();
        if (enName == "") {
            $.messager.alert('提示', '没有找到类名！', 'info');
            return;
        }
        if (RefPK == "") {
            $.messager.alert('提示', '没有找到外键值！', 'info');
            return;
        }
        var url = "UIEn.aspx?EnName=" + enName + "&" + RefPK + "=" + node.id;
        window.showModalDialog(url, '', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes; help: no');
        LoadGridData();
    }
}
//编辑页面
function EditEntityForm() {
    var enName = $("#enName").val();
    var PK = $("#enPK").val();
    if (enName == "") {
        $.messager.alert('提示', '没有找到类名！', 'info');
        return;
    }
    var url = "UIEn.aspx?EnName=" + enName;
    var row = $('#ensGrid').datagrid('getSelected');
    if (row) {
        url = "UIEn.aspx?EnName=" + enName + "&PK=" + row[PK];
    }
    window.showModalDialog(url, '', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes; help: no');
    LoadGridData();
}
//公共方法
function queryData(param, callback, scope, method, showErrMsg) {
    if (!method) method = 'GET';
    $.ajax({
        type: method, //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: "TreeEns.aspx", //要访问的后台地址
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