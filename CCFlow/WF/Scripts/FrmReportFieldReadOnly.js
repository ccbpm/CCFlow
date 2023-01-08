//设置
function SetEntity() {
    var fk_MapData = $("#EnNo").val();
    var url = basePath +"/WF/Admin/FoolFormDesigner/Rpt/Frm_ColsChose.aspx?FK_MapData=" + fk_MapData;
    $("<div id='dialogEnPanel'></div>").append($("<iframe width='100%' height='100%' frameborder=0 src='" + url + "'/>")).dialog({
        title: "窗口",
        width: 800,
        height: 550,
        autoOpen: true,
        modal: true,
        resizable: true,
        onClose: function () {
            $("#dialogEnPanel").remove();
            var pg = $('#ensGrid').datagrid('getPager');
            var curPage = $(pg).pagination.pageNumber;
            LoadGridData(curPage, 20);
        },
        buttons: [{ text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                $('#dialogEnPanel').dialog("close");
            }
        }]
    });
}

//修改
function EditEntityForm() {
    var row = $('#ensGrid').datagrid('getSelected');
    if (row) {
        OpenDialog(row["OID"], 'edit');
    } else {
        $.messager.alert('提示', '请选择记录后再试！', 'info');
    }
}

//弹出页面
function OpenDialog(oid, showModel) {
    var fk_MapData = $("#EnNo").val();
    var dialogModel = showModel;
    var date = new Date();
    var strTimeKey = "";
    strTimeKey += date.getFullYear(); //年
    strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
    strTimeKey += date.getDate(); //日
    strTimeKey += date.getHours(); //HH
    strTimeKey += date.getMinutes(); //MM
    strTimeKey += date.getSeconds(); //SS
    var url = "../CCForm/Frm.htm?FK_MapData=" + fk_MapData + "&WorkID=" + oid + "&IsEdit=0&T=" + strTimeKey;
    var winWidth = document.body.clientWidth;
    //计算显示宽度
    winWidth = winWidth * 0.9;
    if (winWidth > 820) winWidth = 920;

    var winheight = document.body.clientHeight;
    //计算显示高度
    winheight = winheight * 0.98
    if (winheight > 780) winheight = 780;

    $("<div id='dialogEnPanel' style='z-index: 9999;'></div>").append($("<iframe id='dialogFrame' width='100%' height='100%' onload='focus()' frameborder=0 src='" + url + "'/>")).dialog({
        title: "窗口",
        width: winWidth,
        height: winheight,
        autoOpen: true,
        modal: true,
        resizable: true,
        onClose: function () {
            //不保存就删除
            if (dialogModel == "create") {
                var fk_MapData = $("#EnNo").val();
                var params = {
                    method: 'deleteentity',
                    FK_MapData: fk_MapData,
                    OID: oid
                };
                queryData(params, function (js, scope) { }, this);
            }
            $("#dialogFrame").remove();
            $("#dialogEnPanel").remove();
            var pg = $('#ensGrid').datagrid('getPager');
            var curPage = $(pg).pagination.pageNumber;
            LoadGridData(curPage, 20);
        },
        buttons: [{ text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                $('#dialogEnPanel').dialog("close");
            }
        }]
    });
}
//加载表格数据
function LoadGridData(pageNumber, pageSize) {
    var fk_Mapdata = $("#EnNo").val();
    var params = {
        method: "getensgriddata",
        FK_MapData: fk_Mapdata,
        pageNumber: pageNumber,
        pageSize: pageSize
    };
    queryData(params, function (js, scope) {
        $("#pageloading").hide();
        if (js) {
            if (js == "") js = "[]";

            //系统错误
            if (js.status && js.status == 500) {
                $("body").html("<b style='color:red;'>请传入正确的参数名。<b>");
                return;
            }

            var pushData = cceval('(' + js + ')');
            var fitColumns = true;
            if (pushData.columns.length > 7) {
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
                pageNumber: pageNumber,
                pageSize: pageSize,
                pageList: [20, 30, 40, 50],
                onDblClickCell: function (index, field, value) {
                    EditEntityForm();
                },
                loadMsg: '数据加载中......'
            });

            var pg = $("#ensGrid").datagrid("getPager");
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

        } else {
            $.messager.confirm('确认对话框', '没有对此表进行设置显示列，是否现在进行设置？', function (r) {
                if (r) {
                    SetEntity();
                }
            });
        }
    }, this);
}

$(function () {
    LoadGridData(1, 20);
});

//公共方法
function queryData(param, callback, scope, method, showErrMsg) {
    if (!method) method = 'GET';
    $.ajax({
        type: method, //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: "Search.htm", //要访问的后台地址
        data: param, //要发送的数据
        async: false,
        cache: false,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            $("body").html("<b>访问页面出错，传入参数错误。<b>");
            //callback(XMLHttpRequest);
        },
        success: function (msg) {//msg为返回的数据，在这里做数据绑定
            var data = msg;
            callback(data, scope);
        }
    });
}