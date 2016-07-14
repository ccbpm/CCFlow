<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="S8_RptExportTemplate.aspx.cs"
    Inherits="CCFlow.WF.MapDef.Rpt.S8_RptExportTemplate" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<%@ Import Namespace="System.IO" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>设置报表导出模板</title>
    <link href="../../Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <script src="../../Scripts/QueryString.js" type="text/javascript"></script>
    <style type="text/css">
        .tdhover
        {
            background-color: #FFEF8B;
        }
    </style>
    <script type="text/javascript">
        var rptNo = getQueryStringByName('RptNo');
        var flowNo = getQueryStringByName('FK_Flow');
        var md = 'ND2Rpt'; //  getQueryStringByName('FK_MapData');
        var attrs;
        var currCellName;
        var currCellFieldInfo;
        var isBeginIdx = 0;
        var beginIdx = 0;

        function useTmp(obj) {
            var tmpName = $(obj).parent().parent().attr("data-tmp");
            ajax({ method: 'use', RptNo: rptNo, tmp: encodeURIComponent(tmpName) }, function (msg) {
                var re = $.parseJSON(msg);

                if (re.success) {
                    var oldTempName = re.msg;
                    var oldTR = $("#tmps tr[data-tmp='" + oldTempName + "']");
                    var newTR = $("#tmps tr[data-tmp='" + tmpName + "']");

                    if (oldTR.length > 0) {
                        oldTR.children(":eq(2)").children(":eq(0)").linkbutton({
                            iconCls: 'icon-accept',
                            text: '使用此模板',
                            disabled: false
                        });
                        oldTR.children(":eq(2)").children(":eq(0)").attr("onclick", "useTmp(this)");
                    }

                    newTR.children(":eq(2)").children(":eq(0)").linkbutton({
                        iconCls: 'icon-bind',
                        text: '使用中……',
                        disabled: true
                    });
                    newTR.children(":eq(2)").children(":eq(0)").removeAttr("onclick");
                }
            });
        }

        function renameTmp(obj) {
            var tmpName = $(obj).parent().parent().attr("data-tmp");
            OpenEasyUiSampleEditDialog('模板文件', '重命名', tmpName.substring(0, tmpName.indexOf('.')), function (newName, oldName) {
                ajax({ method: 'rename', RptNo: rptNo, tmp: encodeURIComponent(tmpName), newTmp: encodeURIComponent(newName) }, function (msg) {
                    var re = $.parseJSON(msg);

                    if (re.success) {
                        var name = re.msg;
                        $("#tmps tr[data-tmp='" + tmpName + "']").children(":eq(1)").children(":eq(0)").text(name);
                        $("#tmps tr[data-tmp='" + tmpName + "']").attr("data-tmp", name);
                    }
                });
            }, tmpName);
        }

        function setTmp(obj) {
            var tmpName = $(obj).parent().parent().attr("data-tmp");
            ajax({ method: 'set', RptNo: rptNo, tmp: encodeURIComponent(tmpName) }, function (msg) {
                var re = $.parseJSON(msg);

                if (re.success) {
                    //首先清空因上一次打开所留的信息
                    closeSet();

                    attrs = re.attrs;
                    $('#setDiv').attr('data-tmp', tmpName);
                    $('#setDiv').show();
                    $('#excelDiv').html(re.setinfo);
                    $(document.body).append(re.menu);
                    $.parser.parse();

                    $('#excel td').hover(function () {
                        currCellName = $(this).attr('data-name');
                        currCellFieldInfo = $(this).attr('data-field').split('`');

                        if (isBeginIdx == 0) {
                            $(this).addClass('tdhover');
                            $(this).tooltip({
                                position: 'top',
                                content: '<span style="font-size:16px;font-weight:bold;">' + currCellName + (currCellFieldInfo && currCellFieldInfo.length == 4 ? ('&nbsp;&nbsp;' + $(this).attr('data-tooltip')) : '') + '</span>',
                                onShow: function () {
                                    $(this).tooltip('tip').css({
                                        backgroundColor: '#FFEF8B',
                                        borderColor: 'red',
                                        color: 'red'
                                    });
                                }
                            }).tooltip('show');
                        }
                        else if (isBeginIdx == 1) { //开始行号设置
                            $(this).parent().addClass('tdhover');
                        }
                        else {  //开始列号设置
                            $("#excel td[data-colid='" + $(this).attr('data-colid') + "']").addClass('tdhover');
                        }
                    }, function () {
                        if (isBeginIdx == 0) {
                            currCellName = null;
                            $(this).removeClass('tdhover');
                            $(this).tooltip('hide').tooltip('destroy');
                        }
                        else if (isBeginIdx == 1) {
                            $(this).parent().removeClass('tdhover');
                        }
                        else {
                            $("#excel td[data-colid='" + $(this).attr('data-colid') + "']").removeClass('tdhover');
                        }
                    });

                    $('#excel td').bind("click", function (e) {
                        e.preventDefault();

                        if (isBeginIdx == 0) {
                            $('#mAttrs').attr('data-td', $(this).attr('data-name'));
                            $('#mAttrs').menu('show', {
                                left: e.pageX,
                                top: e.pageY
                            });
                        }
                        else if (isBeginIdx == 1) {
                            beginIdx = parseInt($(this).attr('data-rowid'));
                            alert('填充数据将从第 ' + (beginIdx + 1) + ' 行开始！');
                        }
                        else {
                            beginIdx = parseInt($(this).attr('data-colid'));
                            alert('填充数据将从第 ' + (beginIdx + 1) + ' 列开始！');
                        }
                    });

                    $('#mAttrs').menu({
                        onClick: function (item) {
                            var cellName = $('#mAttrs').attr('data-td');
                            var td = $("#excel td[data-name='" + cellName + "']");

                            if (item.name == 'deleteField') {
                                td.css("background-color", '');
                                td.attr('data-field', '');
                                td.attr('data-tooltip', '');
                            }
                            else {
                                var attr = getMapAttr(item.name);
                                td.css("background-color", "#FFF5B1");
                                td.attr('data-field', attr.FK_MAPDATA + '`' + attr.FK_MAPDATANAME + '`' + attr.KEYOFEN + '`' + attr.NAME);
                                td.attr('data-tooltip', (attr.FK_MAPDATA == md ? '' : (attr.FK_MAPDATA + '[' + attr.FK_MAPDATANAME + '] ')) + attr.KEYOFEN + '[' + attr.NAME + ']');
                            }
                        }
                    });

                    $("#excel td[data-field!='']").css("background-color", "#FFF5B1");
                }
            });
        }

        function getMapAttr(names) {
            var ns = names.split('.');
            var attr;

            $.each(attrs, function () {
                if (this.FK_MAPDATA == ns[0] && this.KEYOFEN == ns[1]) {
                    attr = this;
                    return false;
                }
            });

            return attr;
        }

        function getCellFieldInfoForToolTip(aFieldInfo) {
            var tip = '';

            if (aFieldInfo && aFieldInfo.length == 4) {
                tip = '&nbsp;&nbsp;';

                if (aFieldInfo[0] != 'ND' + parseInt(flowNo, 10) + 'Rpt') {
                    tip += aFieldInfo[1] + '&nbsp;';
                }

                tip += aFieldInfo[2] + '[' + aFieldInfo[3] + ']';
            }

            return tip;
        }

        function delTmp(obj) {
            var tmpName = $(obj).parent().parent().attr("data-tmp");
            if (!confirm('你确定要删除“' + tmpName + '”模板吗？')) {
                return;
            }

            ajax({ method: 'del', RptNo: rptNo, tmp: encodeURIComponent(tmpName) }, function (msg) {
                var re = $.parseJSON(msg);

                if (re.success) {
                    $("#tmps tr[data-tmp='" + tmpName + "']").remove();
                }
            });
        }

        function downTmp(obj) {
            var tmpName = $(obj).parent().parent().attr("data-tmp");
            window.open('S8_RptExportTemplate.aspx?method=down&RptNo=' + rptNo + '&tmp=' + encodeURIComponent(tmpName), '_blank');
        }

        function beginSetIdx(isVertical) {
            isBeginIdx = isVertical ? 1 : 2;
        }

        function beginSetField() {
            isBeginIdx = 0;
        }

        function ajax(data, successFunction, errorFunction) {
            var url = 'S8_RptExportTemplate.aspx?t=' + Math.random();

            $.ajax({
                type: "GET", //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: url, //要访问的后台地址
                data: data, //要发送的数据
                async: true,
                cache: false,
                error: function (XMLHttpRequest, errorThrown) {
                    if (errorFunction) {
                        errorFunction(XMLHttpRequest);
                    }
                },
                success: function (msg) {//msg为返回的数据，在这里做数据绑定
                    var d = msg;
                    if (successFunction) {
                        successFunction(d);
                    }
                }
            });
        }

        function closeSet() {
            $('#excelDiv').html();
            $('#setDiv').hide();

            attrs = undefined;
            currCellName = undefined;
            currCellFieldInfo = undefined;
            isBeginIdx = 0;
            beginIdx = 0;
        }

        function saveSet() {
            var tmpName = $('#setDiv').attr('data-tmp');
            var re = isBeginIdx + '`' + beginIdx;
            var ns;

            $.each($("#excel td[data-field!='']"), function () {
                ns = $(this).attr('data-field').split('`');
                re += '`' + $(this).attr('data-rowid') + '^' + $(this).attr('data-colid') + '^' + ns[0] + '^' + ns[2];
            });

            ajax({ method: 'save', RptNo: rptNo, tmp: encodeURIComponent(tmpName), data: encodeURIComponent(re) }, function (msg) {
                var re = $.parseJSON(msg);

                if (re.success) {
                    alert('保存成功！');
                    closeSet();
                }
                else {
                    alert('保存失败，错误信息：' + re.msg);
                }
            });
        }

        $(function () {
            $('#setDiv').hide();
        });
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center',title:'<%=this.Title %>',border:false" style="padding: 5px;
        height: auto">
        <table id="tmps" cellpadding="0" cellspacing="0" border="1" class="Table" style="width: 100%">
            <tr>
                <td class="GroupTitle" style="text-align: center; width: 40px">
                    序
                </td>
                <td class="GroupTitle" style="width: 260px">
                    模板名称
                </td>
                <td class="GroupTitle">
                    操作
                </td>
            </tr>
            <%
                FileInfo[] files = new DirectoryInfo(TmpDir).GetFiles();
                int i = 1;
                string filename = null;

                foreach (FileInfo file in files)
                {
                    if (Exts.IndexOf(file.Extension.ToLower()) == -1)
                        continue;

                    filename = Path.GetFileNameWithoutExtension(file.FullName);
            %>
            <tr data-tmp='<%=file.Name %>'>
                <td class="Idx" style="text-align: center">
                    <%=i++ %>
                </td>
                <td>
                    <a href="javascript:void(0)" onclick="downTmp(this)">
                        <%=file.Name %></a>
                </td>
                <td>
                    <%
                    if (MData.Name == filename)
                    {
                    %>
                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-bind',disabled:true">
                        使用中……</a>
                    <%
                    }
                    else
                    {%>
                    <a href="javascript:void(0)" onclick="useTmp(this)" class="easyui-linkbutton" data-options="iconCls:'icon-accept'">
                        使用此模板</a>
                    <%
                    }%>
                    &nbsp; <a href="javascript:void(0)" onclick="renameTmp(this)" class="easyui-linkbutton"
                        data-options="iconCls:'icon-edit'">重命名</a> &nbsp; <a href="javascript:void(0)" onclick="setTmp(this)"
                            class="easyui-linkbutton" data-options="iconCls:'icon-config'">配置</a> &nbsp;
                    <a href="javascript:void(0)" onclick="delTmp(this)" class="easyui-linkbutton" data-options="iconCls:'icon-delete'">
                        删除</a>
                </td>
            </tr>
            <%
                }
            %>
            <tr>
                <td class="Idx" style="text-align: center">
                    <%=i %>
                </td>
                <td colspan="2">
                    上传模板（格式仅限Microsoft Excel文档 [*.xls, *.xlsx]）：<asp:FileUpload ID="fileUpload" runat="server" />&nbsp;<cc1:LinkBtn
                        ID="lbtnUpload" runat="server" CssClass="easyui-linkbutton" data-options="iconCls:'icon-up'">上传</cc1:LinkBtn>
                </td>
            </tr>
        </table>
        <br />
        <div id="setDiv" style="width: 98%">
            <a id="btnSave" class="easyui-linkbutton" href="javascript:void(0)" onclick="saveSet()"
                data-options="plain:true,iconCls:'icon-save'">保存</a> <a id="btnSetField" class="easyui-linkbutton"
                    href="javascript:void(0)" onclick="beginSetField()" data-options="plain:true,iconCls:'icon-accept'">
                    字段对应</a> <a id="btnBeginSetIdx" class="easyui-splitbutton" href="javascript:void(0)"
                        onclick="beginSetIdx(true)" data-options="menu:'#mBeginIdx',plain:true,iconCls:'icon-ok'">
                        开始行号</a> <a id="btnClose" class="easyui-linkbutton" href="javascript:void(0)" onclick="closeSet()"
                            data-options="plain:true,iconCls:'icon-closecol'">关闭</a>
            <br />
            <div id="excelDiv">
            </div>
        </div>
        <div id="mBeginIdx" style="width: 100px;">
            <div onclick="beginSetIdx(true)" data-options="iconCls:'icon-ok'">
                开始行号</div>
            <div onclick="beginSetIdx(false)" data-options="iconCls:'icon-cancel'">
                开始列号</div>
        </div>
    </div>
    </form>
</body>
</html>
