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
        var md = getQueryStringByName('FK_MapData');
        var attrs;
        var dtlattrs;
        var currCellName;
        var currCellFieldInfo;
        var currDtl;
        var isBeginIdx = 0;
        var beginIdx = 0;
        var isBeginDtlIdx = false;

        function useTmp(obj) {
            /// <summary>使用此模板</summary>
            var tmpName = $(obj).parent().parent().attr("data-tmp");
            ajax({ method: 'use', FK_Flow: flowNo, FK_MapData: md, RptNo: rptNo, tmp: encodeURIComponent(tmpName) }, function (msg) {
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
            /// <summary>重命名模板文件</summary>
            var tmpName = $(obj).parent().parent().attr("data-tmp");
            OpenEasyUiSampleEditDialog('模板文件', '重命名', tmpName.substring(0, tmpName.indexOf('.')), function (newName, oldName) {
                ajax({ method: 'rename', FK_Flow: flowNo, FK_MapData: md, RptNo: rptNo, tmp: encodeURIComponent(tmpName), newTmp: encodeURIComponent(newName) }, function (msg) {
                    var re = $.parseJSON(msg);

                    if (re.success) {
                        var name = re.msg;
                        $("#tmps tr[data-tmp='" + tmpName + "']").children(":eq(1)").children(":eq(0)").text(name);
                        $("#tmps tr[data-tmp='" + tmpName + "']").attr("data-tmp", name);

                        if ($('#setDiv').attr('data-tmp').length > 0) {
                            $('#setDiv').attr('data-tmp', name);
                        }
                    }
                });
            }, tmpName);
        }

        function setTmp(obj) {
            /// <summary>配置模板信息</summary>
            var tmpName = $(obj).parent().parent().attr("data-tmp");
            ajax({ method: 'set', FK_Flow: flowNo, FK_MapData: md, RptNo: rptNo, tmp: encodeURIComponent(tmpName) }, function (msg) {
                var re = $.parseJSON(msg);

                if (re.success) {
                    //首先清空因上一次打开所留的信息
                    closeSet();

                    attrs = re.attrs;
                    dtlattrs = re.dtlattrs;
                    $('#setDiv').attr('data-tmp', tmpName);
                    $('#setDiv').show();
                    $('#excelDiv').html(re.setinfo);
                    $(document.body).append(re.menu);
                    $(document.body).append(re.dtlmenu);
                    $.parser.parse();

                    beginIdx = parseInt($('#excel').attr('data-beginidx'));
                    beginSetIdx();
                    isBeginIdx = 0;

                    $('#excel td').hover(function () {
                        currCellName = $(this).attr('data-name');
                        currCellFieldInfo = $(this).attr('data-field').split('`');
                        var tip = $(this).attr('data-tooltip');

                        if (isBeginIdx == 0) {
                            $(this).addClass('tdhover');
                            $(this).tooltip({
                                position: 'top',
                                content: '<span style="font-size:16px;font-weight:bold;">' + currCellName + (tip && tip.length > 1 ? ('&nbsp;&nbsp;' + tip) : '') + '</span>',
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

                        if (isBeginDtlIdx) {
                            //仅当该单元格已经设置主表字段对应，才可以设置明细表字段对应。2016-08-18，暂去掉
                            //                            if ($(this).attr('data-field').length == 0) {
                            //                                return;
                            //                            }

                            $('#mDtlAttrs').attr('data-td', $(this).attr('data-name'));
                            $('#mDtlAttrs').menu('show', {
                                left: e.pageX,
                                top: e.pageY
                            });
                            return;
                        }

                        if (isBeginIdx == 0) {
                            $('#mAttrs').attr('data-td', $(this).attr('data-name'));
                            $('#mAttrs').menu('show', {
                                left: e.pageX,
                                top: e.pageY
                            });
                        }
                        else if (isBeginIdx == 1) {
                            beginIdx = parseInt($(this).attr('data-rowid'));
                            $('#excel').attr('data-direction', 1);
                            $('#excel').attr('data-beginidx', beginIdx);
                            alert('填充数据将从第 ' + (beginIdx + 1) + ' 行开始！');
                        }
                        else {
                            beginIdx = parseInt($(this).attr('data-colid'));
                            $('#excel').attr('data-direction', 2);
                            $('#excel').attr('data-beginidx', beginIdx);
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
                                //td.attr('data-dtlField', '');
                                //td.attr('data-tooltip', '');
                                var dtlField = td.attr('data-dtlField');
                                if (dtlField && dtlField.length > 0) {
                                    var dtlArr = dtlField.split('.');
                                    var dtlAttr = getDtlMapAttr(dtlArr[0] + '.' + dtlArr[2]);
                                    td.attr('data-tooltip', '明细表：' + dtlAttr.FK_MAPDATA + '[' + dtlAttr.FK_MAPDATANAME + '] ' + dtlAttr.KEYOFEN + '[' + dtlAttr.NAME + ']');
                                }
                                else {
                                    td.attr('data-tooltip', '');
                                }

                                currDtl = getCurrDtl();
                            }
                            else {
                                var attr = getMapAttr(item.name);
                                td.css("background-color", "#FFF5B1");
                                td.attr('data-field', attr.FK_MAPDATA + '`' + attr.FK_MAPDATANAME + '`' + attr.KEYOFEN + '`' + attr.NAME);

                                var dtlField = td.attr('data-dtlField');
                                var tip = (attr.FK_MAPDATA == md ? '' : (attr.FK_MAPDATA + '[' + attr.FK_MAPDATANAME + '] ')) + attr.KEYOFEN + '[' + attr.NAME + ']';

                                if (dtlField && dtlField.length > 0) {
                                    var dtlArr = dtlField.split('.');
                                    var dtlAttr = getDtlMapAttr(dtlArr[0] + '.' + dtlArr[2]);
                                    td.attr('data-tooltip', tip + '<br />明细表：' + dtlAttr.FK_MAPDATA + '[' + dtlAttr.FK_MAPDATANAME + '] ' + dtlAttr.KEYOFEN + '[' + dtlAttr.NAME + ']');
                                }
                                else {
                                    td.attr('data-tooltip', tip);
                                }
                            }
                        }
                    });

                    $('#mDtlAttrs').menu({
                        onClick: function (item) {
                            var cellName = $('#mDtlAttrs').attr('data-td');
                            var td = $("#excel td[data-name='" + cellName + "']");
                            var attr = td.attr('data-field').split('`');

                            if (item.name == 'deleteDtlField') {
                                td.attr('data-dtlField', '');

                                var field = td.attr('data-field');

                                if (field && field.length > 0) {
                                    td.attr('data-tooltip', (attr[0] == md ? '' : (attr[0] + '[' + attr[1] + '] ')) + attr[2] + '[' + attr[3] + ']');
                                }
                                else {
                                    td.attr('data-tooltip', '');
                                }

                                currDtl = getCurrDtl();
                            }
                            else {
                                var dtlAttr = getDtlMapAttr(item.name);

                                if (currDtl && dtlAttr.FK_MAPDATA != currDtl) {
                                    alert('模板中不能定义多个明细表填充，当前定义的明细表为：' + currDtl);
                                    return;
                                }

                                td.attr('data-dtlField', dtlAttr.FK_MAPDATA + '`' + dtlAttr.FK_MAPDATANAME + '`' + dtlAttr.KEYOFEN + '`' + dtlAttr.NAME);
                                td.css("background-color", "#FFF5B1");

                                var field = td.attr('data-field');
                                var tip = '明细表：' + dtlAttr.FK_MAPDATA + '[' + dtlAttr.FK_MAPDATANAME + '] ' + dtlAttr.KEYOFEN + '[' + dtlAttr.NAME + ']';

                                if (field && field.length > 0) {
                                    td.attr('data-tooltip', (attr[0] == md ? '' : (attr[0] + '[' + attr[1] + '] ')) + attr[2] + '[' + attr[3] + ']<br />' + tip);
                                }
                                else {
                                    td.attr('data-tooltip', tip);
                                }
                            }
                        }
                    });

                    //设置所有已定义单元格的背景颜色
                    $("#excel td[data-field!='']").css("background-color", "#FFF5B1");
                    $("#excel td[data-dtlField!='']").css("background-color", "#FFF5B1");

                    //获取已定义明细表，只能配置一个明细表
                    currDtl = getCurrDtl();
                }
            });
        }

        function getCurrDtl() {
            /// <summary>获取当前已经定义的明细表编号</summary>
            var dtlTds = $("#excel td[data-dtlField!='']");
            if (dtlTds.length > 0) {
                return $(dtlTds[0]).attr('data-dtlField').split('`')[0];
            }

            return null;
        }

        function getMapAttr(names) {
            /// <summary>根据menu-item的name，获取对应的字段对象</summary>
            /// <param name="names" type="String">name，格式如：ND2Rpt.FK_Dept</param>
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

        function getDtlMapAttr(names) {
            /// <summary>根据menu-item的name，获取对应的明细表字段对象</summary>
            /// <param name="names" type="String">name，格式如：ND201Dtl1.Name</param>
            var ns = names.split('.');
            var attr;

            $.each(dtlattrs, function () {
                if (this.FK_MAPDATA == ns[0] && this.KEYOFEN == ns[1]) {
                    attr = this;
                    return false;
                }
            });

            return attr;
        }

        function delTmp(obj) {
            /// <summary>删除模板</summary>
            var tmpName = $(obj).parent().parent().attr("data-tmp");
            if (!confirm('你确定要删除“' + tmpName + '”模板吗？')) {
                return;
            }

            ajax({ method: 'del', FK_Flow: flowNo, FK_MapData: md, RptNo: rptNo, tmp: encodeURIComponent(tmpName) }, function (msg) {
                var re = $.parseJSON(msg);

                if (re.success) {
                    $("#tmps tr[data-tmp='" + tmpName + "']").remove();

                    if ($('#setDiv').attr('data-tmp').length > 0) {
                        closeSet();
                    }
                }
            });
        }

        function downTmp(obj) {
            /// <summary>下载模板</summary>
            var tmpName = $(obj).parent().parent().attr("data-tmp");
            window.open('S8_RptExportTemplate.aspx?method=down&RptNo=' + rptNo + '&tmp=' + encodeURIComponent(tmpName), '_blank');
        }

        function beginSetIdx(isVertical) {
            /// <summary>设置导出数据填充开始的行/列号</summary>
            /// <param name="isVertical" type="Boolean">是否是垂直方向填充</param>
            isBeginDtlIdx = false;

            if (isVertical == undefined) {
                isBeginIdx = parseInt($('#excel').attr('data-direction'));
            }
            else {
                isBeginIdx = isVertical ? 1 : 2;
            }

            $('#excel').attr('data-direction', isBeginIdx);

            $('#btnBeginSetIdx').splitbutton({
                text: isBeginIdx == 1 ? '开始行号' : '开始列号'
            });

            for (var i = 1; i < 3; i++) {
                $('#mBeginIdx').menu('setIcon', {
                    target: $('#mItem' + i)[0],
                    iconCls: i == isBeginIdx ? 'icon-ok' : 'icon-xxxx'
                });
            }
        }

        function beginSetField() {
            /// <summary>设置主表字段绑定</summary>
            isBeginIdx = 0;
            isBeginDtlIdx = false;
        }

        function beginSetDtlField() {
            /// <summary>设置明细表字段绑定</summary>
            isBeginDtlIdx = true;
        }

        function ajax(data, successFunction, errorFunction) {
            /// <summary>与后台交互</summary>
            /// <param name="data" type="Object">向后台发送的参数对象，格式如：{ method: 'save', FK_Flow: '002',...}</param>
            /// <param name="successFunction" type="Function">交互成功后，运行的函数</param>
            /// <param name="errorFunction" type="Function">交互失败后，运行的函数</param>
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
            /// <summary>关闭模板配置界面</summary>
            $('#excelDiv').html();
            $('#setDiv').attr('data-tmp', '');
            $('#setDiv').hide();

            attrs = undefined;
            dtlattrs = undefined;
            currCellName = undefined;
            currCellFieldInfo = undefined;
            isBeginIdx = 0;
            beginIdx = 0;
        }

        function saveSet() {
            /// <summary>保存模板配置信息</summary>
            var tmpName = $('#setDiv').attr('data-tmp');
            var re = isBeginIdx + '`' + beginIdx;
            var ns;
            var nsDtl;

            $.each($("#excel td[data-field!='']"), function () {
                ns = $(this).attr('data-field').split('`');
                nsDtl = $(this).attr('data-dtlField').split('`');
                re += '`' + $(this).attr('data-rowid') + '^' + $(this).attr('data-colid') + '^' + (ns.length == 4 ? ns[0] : '') + '^' + (ns.length == 4 ? ns[2] : '') + '^' + (nsDtl.length == 4 ? nsDtl[0] : '') + '^' + (nsDtl.length == 4 ? nsDtl[2] : '');
            });

            $.each($("#excel td[data-dtlField!='']"), function () {
                ns = $(this).attr('data-field').split('`');
                nsDtl = $(this).attr('data-dtlField').split('`');
                re += '`' + $(this).attr('data-rowid') + '^' + $(this).attr('data-colid') + '^' + (ns.length == 4 ? ns[0] : '') + '^' + (ns.length == 4 ? ns[2] : '') + '^' + (nsDtl.length == 4 ? nsDtl[0] : '') + '^' + (nsDtl.length == 4 ? nsDtl[2] : '');
            });

            ajax({ method: 'save', FK_Flow: flowNo, FK_MapData: md, RptNo: rptNo, tmp: encodeURIComponent(tmpName), data: encodeURIComponent(re) }, function (msg) {
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
        <div id="setDiv" style="width: 98%" data-tmp="">
            <a id="btnSave" class="easyui-linkbutton" href="javascript:void(0)" onclick="saveSet()"
                data-options="plain:true,iconCls:'icon-save'">保存</a> <a id="btnSetField" class="easyui-linkbutton"
                    href="javascript:void(0)" onclick="beginSetField()" data-options="plain:true,iconCls:'icon-accept'">
                    主表字段对应</a> <a id="A1" class="easyui-linkbutton" href="javascript:void(0)" onclick="beginSetDtlField()"
                        data-options="plain:true,iconCls:'icon-accept'">明细表字段对应</a> <a id="btnBeginSetIdx"
                            class="easyui-splitbutton" href="javascript:void(0)" onclick="beginSetIdx()"
                            data-options="menu:'#mBeginIdx',plain:true,iconCls:'icon-manual'">开始行号</a>
            <a id="btnClose" class="easyui-linkbutton" href="javascript:void(0)" onclick="closeSet()"
                data-options="plain:true,iconCls:'icon-closecol'">关闭</a>
            <br />
            <div id="excelDiv">
            </div>
        </div>
        <div id="mBeginIdx" style="width: 100px;">
            <div id="mItem1" onclick="beginSetIdx(true)" data-options="iconCls:''">
                开始行号</div>
            <div id="mItem2" onclick="beginSetIdx(false)" data-options="iconCls:''">
                开始列号</div>
        </div>
    </div>
    </form>
</body>
</html>
