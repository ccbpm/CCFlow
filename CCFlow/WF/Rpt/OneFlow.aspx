<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OneFlow.aspx.cs" Inherits="CCFlow.WF.Rpt.OneFlow" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程数据一户式管理</title>
    <link href="../Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script language="javascript">
        var currShow;

        //在右侧框架中显示指定url的页面
        function OpenUrlInRightFrame(ele, url) {
            if (ele != null && ele != undefined) {
                //if (currShow == $(ele).text()) return;

                currShow = $(ele).parents('li').text(); //有回车符

                $.each($('ul.navlist'), function () {
                    $.each($(this).children('li'), function () {
                        $(this).children('div').css('font-weight', $(this).text() == currShow ? 'bold' : 'normal');
                    });
                });

                $('#context').attr('src', url + '&s=' + Math.random());
            }
        }

        $(function () {
            $('#starthref').click();
        });
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center',border:false">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'west',split:true" style="width: 200px;">
                <% if (BP.Web.WebUser.No == "admin")
                   { %>
                <div class="easyui-panel" title="报表设计/监控" data-options="collapsible:true,border:false" style="height: auto">
                    <ul class="navlist">
                        <li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, '../Admin/FoolFormDesigner/Rpt/S1_Edit.aspx?FK_MapData=<%=this.FK_MapData%>&FK_Flow=<%=this.FK_Flow%>&RptNo=<%=this.RptNo%>')">
                                    <span class="nav">1. 基本信息</span></a></div>
                        </li>
                        <li>
                            <div>
                                <a href="javascript:void(0)" id="starthref" onclick="OpenUrlInRightFrame(this, '../Admin/FoolFormDesigner/Rpt/S2_ColsChose.aspx?FK_MapData=<%=this.FK_MapData%>&FK_Flow=<%=this.FK_Flow%>&RptNo=<%=this.RptNo%>')">
                                    <span class="nav">2. 设置报表显示列</span></a></div>
                        </li>
                        <li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, '../Admin/FoolFormDesigner/Rpt/S3_ColsLabel.aspx?FK_MapData=<%=this.FK_MapData%>&FK_Flow=<%=this.FK_Flow%>&RptNo=<%=this.RptNo%>')">
                                    <span class="nav">3. 设置报表显示列次序</span></a></div>
                        </li>
                        <li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, '../Admin/FoolFormDesigner/Rpt/S5_SearchCond.aspx?FK_MapData=<%=this.FK_MapData%>&FK_Flow=<%=this.FK_Flow%>&RptNo=<%=this.RptNo%>')">
                                    <span class="nav">4. 设置报表查询条件</span></a></div>
                        </li>
                        <li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, '../Admin/FoolFormDesigner/Rpt/S8_RptExportTemplate.aspx?FK_MapData=<%=this.FK_MapData%>&FK_Flow=<%=this.FK_Flow%>&RptNo=<%=this.RptNo%>')">
                                    <span class="nav">5. 设置报表导出模板</span></a></div>
                        </li>
                        <%-- <li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, '../MapDef/Rpt/S6_Power.aspx?FK_MapData=<%=this.FK_MapData%>&FK_Flow=<%=this.FK_Flow%>&RptNo=<%=this.RptNo%>')">
                                    <span class="nav">5. 设置报表权限</span></a></div>
                        </li>
                       <li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, '../Admin/FlowDB/FlowDB.aspx?s=d34&FK_Flow=<%=this.FK_Flow%>&ExtType=StartFlow&RefNo=')">
                                    <span>6.流程监控</span></a></div>
                        </li>--%>

                        <%--<li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, '../Comm/Search.aspx?s=d34&FK_Flow=<%=this.FK_Flow%>&ExtType=StartFlow&EnsName=BP.WF.Data.GenerWorkFlowViews')">
                                    <span>7.流程监控Ext</span></a></div>
                        </li>--%>

                    </ul>
                </div>
                
                <%} %>
                <div class="easyui-panel" title="报表查看" data-options="collapsible:true,border:false" style="height: auto">
                    <ul class="navlist">
                        <li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, './Search.aspx?FK_Flow=<%=this.FK_Flow %>&RptNo=<%= this.RptNo%>')">
                                    <span>1. 查询</span></a></div>
                        </li>
                        <li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, './SearchAdv.aspx?FK_Flow=<%=this.FK_Flow %>&RptNo=<%= this.RptNo%>')">
                                    <span>2. 高级查询</span></a></div>
                        </li>
                        <li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, './Group.aspx?FK_Flow=<%=this.FK_Flow %>&RptNo=<%= this.RptNo%>')">
                                    <span>3. 分组分析</span></a></div>
                        </li>
                        <li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, './D3.aspx?FK_Flow=<%=this.FK_Flow %>&RptNo=<%= this.RptNo%>')">
                                    <span>4. 交叉报表</span></a></div>
                        </li>
                        <li>
                            <div>
                                <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, './Contrast.aspx?FK_Flow=<%=this.FK_Flow %>&RptNo=<%= this.RptNo%>')">
                                    <span>5. 对比分析</span></a></div>
                        </li>
                    </ul>
                </div>
            </div>
            <div data-options="region:'center',noheader:true" style="overflow-y: hidden">
                <iframe id="context" scrolling="auto" frameborder="0" src=""
                    style="width: 100%; height: 100%;"></iframe>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
