<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true"
    CodeBehind="Nodes.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.App.OneFlow.Nodes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../../../Scripts/jquery/themes/default/easyui.css" rel="stylesheet"
        type="text/css" />
    <link href="../../../../Scripts/jquery/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../../../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../../../Scripts/jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100%; min-width: 750px;">
        <caption>
            节点数据分析</caption>
        <tr>
            <th class="GroupTitle" rowspan="2">
                IDX
            </th>
            <th class="GroupTitle center" rowspan="2">
                节点
            </th>
            <th class="GroupTitle center" rowspan="2">
                岗位
            </th>
            <th class="GroupTitle center" colspan="2">
                用时分析(单位:分钟)
            </th>
            <th class="GroupTitle center" colspan="4">
                状态分析
            </th>
            <th class="GroupTitle center" rowspan="2">
                按期办结率
            </th>
            <th class="GroupTitle center" rowspan="2">
                停留<br>逾期
            </th>
            <th class="GroupTitle center" rowspan="2">
                停留<br>待办
            </th>
            <th class="GroupTitle center" rowspan="2">
                工作<br>人员
            </th>
        </tr>
        <tr>
            <th class="GroupTitle center">
                提前
            </th>
            <th class="GroupTitle center">
                逾期
            </th>
            <th class="GroupTitle center">
                按时
            </th>
            <th class="GroupTitle center">
                及时
            </th>
            <th class="GroupTitle center">
                超期
            </th>
            <th class="GroupTitle center">
                逾期
            </th>
        </tr>
        <%
            string flowNo = this.Request.QueryString["FK_Flow"];
            BP.WF.Nodes nds = new BP.WF.Nodes(flowNo);

            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;

            int idx = 0;
            foreach (BP.WF.Node item in nds)
            {
                idx++;

                //提前完成的分钟数.
                BP.DA.Paras ps = new BP.DA.Paras();
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT SUM(" + BP.WF.Data.CHAttr.OverMinutes + ") FROM  WF_CH WHERE FK_Node=" + dbstr + "FK_Node";
                ps.Add(BP.WF.Data.CHAttr.FK_Node, item.NodeID);
                decimal saveMinte = BP.DA.DBAccess.RunSQLReturnValDecimal(ps, 0, 2);

                //计算预期的分钟数.
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT SUM(" + BP.WF.Data.CHAttr.OverMinutes + ") FROM  WF_CH WHERE FK_Node=" + dbstr + "FK_Node";
                ps.Add(BP.WF.Data.CHAttr.FK_Node, item.NodeID);
                decimal yqMinute = BP.DA.DBAccess.RunSQLReturnValDecimal(ps, 0, 2);

                //按期完成.
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(" + BP.WF.Data.CHAttr.MyPK + ") FROM  WF_CH WHERE CHSta=" + dbstr + "CHSta  AND FK_Node=" + dbstr + "FK_Node";
                ps.Add(BP.WF.Data.CHAttr.CHSta, (int)BP.WF.Data.CHSta.AnQi);
                ps.Add(BP.WF.Data.CHAttr.FK_Node, item.NodeID);
                int ch0 = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

                //及时完成.
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(" + BP.WF.Data.CHAttr.MyPK + ") FROM  WF_CH WHERE CHSta=" + dbstr + "CHSta  AND FK_Node=" + dbstr + "FK_Node";
                ps.Add(BP.WF.Data.CHAttr.CHSta, (int)BP.WF.Data.CHSta.JiShi);
                ps.Add(BP.WF.Data.CHAttr.FK_Node, item.NodeID);
                int ch1 = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

                //预期完成.
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(" + BP.WF.Data.CHAttr.MyPK + ") FROM  WF_CH WHERE CHSta=" + dbstr + "CHSta  AND FK_Node=" + dbstr + "FK_Node";
                ps.Add(BP.WF.Data.CHAttr.CHSta, (int)BP.WF.Data.CHSta.YuQi);
                ps.Add(BP.WF.Data.CHAttr.FK_Node, item.NodeID);
                int ch2 = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

                //超期完成.
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(" + BP.WF.Data.CHAttr.MyPK + ") FROM  WF_CH WHERE CHSta=" + dbstr + "CHSta  AND FK_Node=" + dbstr + "FK_Node";
                ps.Add(BP.WF.Data.CHAttr.CHSta, (int)BP.WF.Data.CHSta.YuQi);
                ps.Add(BP.WF.Data.CHAttr.FK_Node, item.NodeID);
                int ch3 = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

                //按期完成率。
                decimal rate = 0;
                int all = ch0 + ch1 + ch2 + ch3;

                if (all != 0)
                    rate = (ch0 + ch1) / all * 100;

                //停留的逾期.
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT( distinct WorkID) AS Num FROM  WF_GenerWorkerlist WHERE IsPass=0  AND FK_Node=" + dbstr + "FK_Node";
                ps.Add(BP.WF.Data.CHAttr.FK_Node, item.NodeID);
                int tolistYuQi = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

                //停留待办.
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT( distinct WorkID) AS Num FROM  WF_GenerWorkerlist WHERE IsPass=0  AND FK_Node=" + dbstr + "FK_Node";
                ps.Add(BP.WF.Data.CHAttr.FK_Node, item.NodeID);
                int tolist = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);


                //工作人员.
                string worker = "";
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT DISTINCT EmpFrom, EmpFromT FROM ND" + int.Parse(flowNo) + "Track WHERE NDFrom=" + dbstr + "FK_Node";
                ps.Add(BP.WF.Data.CHAttr.FK_Node, item.NodeID);
                System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);

                if (dt.Rows.Count == 0)
                {
                    worker = "无";
                }
                else
                {
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        worker += dr[1].ToString() + "、";
                    }
                    worker = worker.TrimEnd('、');
                }
        %>
        <tr>
            <td class="Idx">
                <%=idx %>
            </td>
            <td class="node">
                <div title="<%=item.Name %>" class="easyui-tooltip infTip">
                    [<%=item.NodeID  %>]
                    <%=item.Name  %></div>
            </td>
            <% if (item.HisStationsStr.Length > 4)
               {%>
            <td class="center" style="min-width: 55px;">
                <a title="<%=item.HisStationsStr.TrimEnd(',') %>" class="easyui-tooltip infTip" href="javascript:;">
                    点击查看</a>
            </td>
            <%}
               else
               { %>
            <td class="center" style="width: 55px;">
                <%=item.HisStationsStr.TrimEnd(',') %>
            </td>
            <%} %>
            <td class="center">
                <%=saveMinte%>
            </td>
            <td class="center">
                <%=yqMinute%>
            </td>
            <td class="center">
                <%=ch0 %>
            </td>
            <td class="center">
                <%=ch1 %>
            </td>
            <td class="center">
                <%=ch2 %>
            </td>
            <td class="center">
                <%=ch3 %>
            </td>
            <td class="center">
                <%=rate.ToString("0.00") %>
            </td>
            <td class="center">
                <%=tolistYuQi%>
            </td>
            <td class="center">
                <%=tolist %>
            </td>
            <td class="center">
                <%
                    if (worker.Length > 4)
                    {%>
                <a title="<%=worker %>" class="easyui-tooltip infTip" href="javascript:window.parent.closeTab('节点明细');window.parent.addTab('NodesDtlEmps','节点明细','../../Admin/CCBPMDesigner/App/OneFlow/NodesDtlEmps.aspx?FK_Node=<%=item.NodeID %>&','');">
                    点击查看</a>
                <%}
                    else
                    {%>
                <a title="<%=worker %>" href="javascript:window.parent.closeTab('节点明细');window.parent.addTab('NodesDtlEmps','节点明细','../../Admin/CCBPMDesigner/App/OneFlow/NodesDtlEmps.aspx?FK_Node=<%=item.NodeID %>&','');">
                    <%=worker %></a>
                <%}
                %>
            </td>
        </tr>
        <%}
        %>
    </table>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
        }
        .center
        {
            text-align: center;
        }
        .node div
        {
            cursor: pointer;
            width: 140px;
            overflow: hidden;
            white-space: nowrap;
            text-overflow: ellipsis;
            -o-text-overflow: ellipsis;
            -icab-text-overflow: ellipsis;
            -khtml-text-overflow: ellipsis;
            -moz-text-overflow: ellipsis;
            -webkit-text-overflow: ellipsis;
        }
        
    </style>
</asp:Content>
