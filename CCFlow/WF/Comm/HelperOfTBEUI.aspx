<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HelperOfTBEUI.aspx.cs"
    Inherits="CCFlow.WF.Comm.HelperOfTBEUI" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>词汇选择</title>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script src="../Scripts/jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <style type="text/css">
        .datagrid-header-check
        {
            display: none;
        }
        .datagrid-header-row
        {
            display: none;
        }
        .panel-body, .datagrid-header
        {
            border: none;
        }
    </style>
    <script type="text/ecmascript">
        var WordsSort;
        var AttrKey;
        var FK_MapData; ;
        var lb;
        $(function () {
            $('#tt').tabs({
                onSelect: function (title, index) {
                    switch (index) {  //注意tabs的顺序
                        case 0:
                            startBtn();
                            runEffect("myWords");
                            break;
                        case 1:
                            forbiddenBtn();
                            runEffect("hisWords");
                            break;
                        case 2:
                            startBtn();
                            runEffect("sysWords");
                            break;
                        case 3:
                            forbiddenBtn();
                            runEffect("readWords");
                            break;
                        default:
                            break;
                    }
                }
            });

            $('#win').window('close');
            //初始化赋值.
            WordsSort = Application.common.getArgsFromHref("WordsSort");
            AttrKey = Application.common.getArgsFromHref("AttrKey");
            FK_MapData = Application.common.getArgsFromHref("FK_MapData");
            runEffect("myWords"); //初始化加载我的词汇
        });
        function startBtn() { //启用按钮
            $('#btnAdd').linkbutton('enable');
            $('#btnEdit').linkbutton('enable');
            $('#btnDelete').linkbutton('enable');
        }
        function forbiddenBtn() {  //禁用按钮
            $('#btnAdd').linkbutton('disable');
            $('#btnEdit').linkbutton('disable');
            $('#btnDelete').linkbutton('disable');
        }
        function runEffect(v) {
            lb = v;
            LoadGridData(1, 15);
        };
        function LoadGridData(pageNumber, pageSize) {
            $('#newsGrid').datagrid('loadData', { total: 0, rows: [] });
            $('#newsGrid').datagrid('clearChecked');
            var params = {
                method: "getData",
                AttrKey: AttrKey,
                FK_MapData: FK_MapData,
                pageNumber: pageNumber,
                pageSize: pageSize,
                lb: lb
            };
            queryData(params, function (js, scope) {
                $("#pageloading").hide();
                if (js == "") js = "[]";
                if (js.status && js.status == 500) {
                    $("body").html("<b>访问页面出错，请联系管理员。<b>");
                    return;
                }
                var pushData = eval('(' + js + ')');

                $('#newsGrid').datagrid({
                    columns: [[
                    { checkbox: true },
                    { field: 'CURVALUE', title: '', width: 100, align: 'left' }
                    ]],
                    idField: 'OID',
                    selectOnCheck: false,
                    checkOnSelect: true,
                    singleSelect: true,
                    data: pushData,
                    width: 'auto',
                    height: 'auto',
                    striped: true,
                    rownumbers: true,
                    pagination: true,
                    fitColumns: true,
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    pageList: [15, 30, 40, 50],
                    loadMsg: '数据加载中......'
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

            }, this);
        }
        //刷新
        function RefreshGrid() {
            var grid = $('#newsGrid');
            var options = grid.datagrid('getPager').data("pagination").options;
            var curPage = options.pageNumber;
            var pageSize = options.pageSize;
            LoadGridData(curPage, pageSize);
        }
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "HelperOfTBEUI.aspx", //要访问的后台地址
                data: param, //要发送的数据
                async: false,
                cache: false,
                complete: function () { },
                error: function (XMLHttpRequest, errorThrown) {
                    callback(XMLHttpRequest);
                },
                success: function (msg) {
                    var data = msg;
                    callback(data, scope);
                }
            });
        }
        var insEdit = true;
        //添加数据
        function btnOpenWindow() {
            insEdit = true;
            if (lb == "readWords" || lb == "hisWords")//如果是文件,历史词汇
                return;

            $('#TextArea').val('');
            $('#win').window('open');
            $('#TextArea').focus();
        }
        function btnAddData() {
            var params;
            if (insEdit) {//添加
                var text = $('#TextArea').val();
                text = replaceTrim(text);
                if (text) {
                    params = {
                        method: "addData",
                        AttrKey: AttrKey,
                        FK_MapData: FK_MapData,
                        lb: lb,
                        text: encodeURI(text)
                    };
                } else {
                    $.messager.alert("提示", "请输入数据", "info");
                    $('#TextArea').val('');
                    $('#TextArea').focus();
                    return;
                }
            } else {//编辑
                var text = $('#TextArea').val();
                text = replaceTrim(text);
                if (text) {
                    if (text == againText) {
                        $.messager.alert("提示", "数据没有任何改变哦", "info");
                        return;
                    }
                    params = {
                        method: "editData",
                        oid: oid,
                        text: encodeURI(text),
                        lb: lb
                    };
                } else {
                    $.messager.alert("提示", "请输入数据", "info");
                    $('#TextArea').val('');
                    $('#TextArea').focus();
                    return;
                }
            }

            queryData(params, function (js, scope) {
                $('#win').window('close');
                if (js == "true") {
                    if (insEdit)
                        LoadGridData(1, 15);
                    RefreshGrid();
                } else {
                    $.messager.alert("提示", "操作失败", "info");
                }
            }, this);
        }
        var againText;
        var oid;
        //编辑词汇
        function btnEdit() {
            insEdit = false;
            if (lb == "readWords" || lb == "hisWords")//如果是文件,历史
                return;
            var rows = $('#newsGrid').datagrid('getChecked');
            if (rows.length == 1) {
                againText = rows[0].CURVALUE;
                oid = rows[0].OID;
                $('#TextArea').val(rows[0].CURVALUE);
                $('#win').window('open');
                $('#TextArea').focus();
            }
            else {
                $.messager.alert("提示", "请选择一条数据", "info");
            }
        }
        //字符的操作
        function replaceTrim(val) {//去除空格
            val = val.replace(/[ ]/g, "");
            val = val.replace(/<\/?.+?>/g, "");
            val = val.replace(/[\r\n]/g, "");
            return val;
        }
        //删除
        function btnDelete() {
            if (lb == "readWords" || lb == "hisWords")//如果是文件,历史
                return;
            var rows = $('#newsGrid').datagrid('getChecked');

            if (rows.length >= 1) {
                $.messager.confirm('提示', '确定要删除这' + rows.length + '条数据吗？', function (r) {
                    if (r) {
                        var oids = '';
                        $.each(rows, function (n, value) {
                            oids += value.OID + ",";
                        });
                        params = {
                            method: "deleteData",
                            oids: oids,
                            lb: lb
                        };
                        queryData(params, function (js, scope) {
                            if (js == "true") {
                                LoadGridData(1, 15);
                            } else {
                                $.messager.alert("提示", "操作失败", "info");
                            }
                        }, this);
                    }
                });
            }
            else {
                $.messager.alert("提示", "请选择一条数据", "info");
            }
        }
        //关闭主窗体
        function btnClose() {
            window.close();
        }
        //返回数据
        function btnOk() {
            var rows = $('#newsGrid').datagrid('getChecked');

            if (rows.length == 0) {
                $.messager.alert("提示", "请选择数据", "info");
                return;
            }

            var str = '';
            if (lb == "readWords")//如果是文件
            {
                $.each(rows, function (n, value) {
                    str += value.TxtStr;
                });

                for (var i = 0; true; i++) {
                    if (str.indexOf("ccflow_lover") != -1) {
                        str = str.replace("ccflow_lover", "\n");
                    } else {
                        break;
                    }
                }


            } else {
                $.each(rows, function (n, value) {
                    str += value.CURVALUE + ",";
                });

                str = str.substr(0, str.length - 1);
            }
            //str = str.replace(/\r/g, "");
            //str = str.replace(/\n/g, "");
            str = str.replace(/{/g, "｛");
            str = str.replace(/}/g, "｝");
            str = str.replace(/\[/g, "【");
            str = str.replace(/\]/g, "】");
            str = str.replace(/\"/g, "”");
            str = str.replace(/\'/g, "‘");

            if (str == '') {
                $.messager.alert("提示", "1.没有选中项<br />2.选中的文件不包含任何数据!", "info");
                return;
            }
            saveHistoryWords(str); //保存历史

            //兼容
            var explorer = window.navigator.userAgent;
            if (explorer.indexOf("Chrome") >= 0) {//谷歌
                window.close();
                if (window.opener.document.getElementById("ContentPlaceHolder1_MyFlowUC1_MyFlow1_UCEn1_" + id)) {
                    window.opener.document.getElementById("ContentPlaceHolder1_MyFlowUC1_MyFlow1_UCEn1_" + id).value = str;
                }
                if (window.opener.document.getElementById("ContentPlaceHolder1_ForwardUC1_Top_" + id)) {
                    window.opener.document.getElementById("ContentPlaceHolder1_ForwardUC1_Top_" + id).value = str;
                }
                if (window.opener.document.getElementById("ContentPlaceHolder1_ReturnWork1_Pub1_" + id)) {
                    window.opener.document.getElementById("ContentPlaceHolder1_ReturnWork1_Pub1_" + id).value = str;
                }
                if (window.opener.document.getElementById("Pub1_" + id)) {
                    window.opener.document.getElementById("Pub1_" + id).value = str;
                }
                if (window.opener.document.getElementById("ContentPlaceHolder1_UCEn1_" + id)) {
                    window.opener.document.getElementById("ContentPlaceHolder1_UCEn1_" + id).value = str;
                }
            }
            else {//IE...
                window.returnValue = str;
                window.close(); //关闭子窗口
            }
        }
        function saveHistoryWords(str) {
            if (lb == "readWords" || lb == "hisWords")
                return; //文件/历史数据不在保存之列

            params = {
                method: "saveHistoryData",
                str: encodeURI(str),
                AttrKey: AttrKey,
                FK_MapData: FK_MapData,
                lb: lb
            };
            queryData(params, function (js, scope) {
                if (js == "true") {
                } else {
                    $.messager.alert("提示", "操作失败", "info");
                }
            }, this);
        }
    </script>
</head>
<body class="easyui-layout body">
    <div id="pageloading">
    </div>
    <div data-options="region:'north'" style="height: 60px; border: none;">
        <div id="tt" class="easyui-tabs" style="width: auto; height: 30px;">
            <div title="我的词汇" style="padding: 20px;">
            </div>
            <div title="历史词汇" style="padding: 20px;">
            </div>
            <div title="系统词汇" style="padding: 20px;">
            </div>
            <div title="读取文件" style="padding: 20px;">
            </div>
        </div>
        <div style="background-color: #F4F4F4;">
            <div style="text-align: left; float: left;">
                <a href='javascript:void(0)' id="btnAdd" onclick='btnOpenWindow()' class='easyui-linkbutton'
                    data-options="plain:true,iconCls:'icon-add'" style='margin-left: 10px; color: blue;'>
                    添加数据</a> <a id="btnEdit" href='javascript:void(0)' onclick='btnEdit()' class='easyui-linkbutton'
                        data-options="plain:true,iconCls:'icon-edit'" style='margin-left: 10px; color: blue;'>
                        编辑</a><a href='javascript:void(0)' onclick='btnDelete()' id='btnDelete' class='easyui-linkbutton'
                            data-options="plain:true,iconCls:'icon-delete'" style='margin-left: 10px; color: blue;'>删除</a></div>
            <div style="text-align: right;">
                <a href='javascript:void(0)' onclick='btnOk()' id='btnOk' class='easyui-linkbutton'
                    data-options="plain:true,iconCls:'icon-ok'" style='margin-right: 10px; color: blue;'>
                    确定</a> <a href='javascript:void(0)' onclick='btnClose()' id='btnClose' class='easyui-linkbutton'
                        data-options="plain:true,iconCls:'icon-cancel'" style='margin-right: 20px; color: blue;'>
                        取消</a></div>
        </div>
    </div>
    <div data-options="region:'center'" style="padding: 5px;">
        <table id="newsGrid" fit="true" fitcolumns="true" class="easyui-datagrid">
        </table>
    </div>
    <div id="win" class="easyui-window" title="请输入" style="width: 400px; height: 230px;
        overflow: hidden;" data-options="iconCls:'icon-save',modal:true,collapsible:false,minimizable:false,maximizable:false">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'center'" style="text-align: center;">
                <textarea id="TextArea" cols="20" rows="2" style="width: 350px; height: 150px; margin-top: 5px;
                    overflow: hidden;"></textarea>
                <div style="width: auto; height: 20px; margin-bottom: 0px; text-align: center;">
                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok',plain:true"
                        onclick="btnAddData()">保存</a> <a href="javascript:void(0)" class="easyui-linkbutton"
                            data-options="iconCls:'icon-cancel',plain:true" onclick="$('#win').window('close');">
                            取消</a>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
