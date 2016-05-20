<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/GuestApp/Site.Master"
    AutoEventWireup="true" CodeBehind="Start.aspx.cs" Inherits="CCFlow.SDKFlowDemo.GuestApp.Start" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/info.css" rel="stylesheet" type="text/css" />
    <link href="../../DataUser/Style/Table0.css" rel="stylesheet" type="text/css" />
     <script type="text/javascript">
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="info" class="easyui-layout">
        <div class="info_cen" data-options="region:'center'">
            <div class="info_cen_div">
                <a id="print" class="info_print_a" href="javascript:info_print()">打&nbsp;印</a>
            </div>
            <div class="info_cen_sign">
               流程发起：
            </div>
            <div class="info_cen_content">
                <div id="printDiv">
                    <%
                        System.Data.DataTable dt = BP.WF.Dev2InterfaceGuest.DB_GenerCanStartFlowsOfDataTable();
                    %>
                    <table style="width: 100%;">
                     
                        <tr>
                            <th>序 </th>
                            <th>流程编号</th>
                            <th> 流程名称</th>
                            <th>历史发起的流程</th>
                        </tr>
                        <%
                            int idx = 0;
                            foreach (System.Data.DataRow dr in dt.Rows)
                            {
                                idx++;
                                int flowID = int.Parse(dr["No"].ToString());
                        %>
                        <tr>
                            <td class="Idx">
                                <%=idx %>
                            </td>
                            <td>
                                <%=dr["No"] %>
                            </td>
                            <td>
                                <a href="/WF/MyFlow.aspx?FK_Flow=<%=dr["No"] %>&GuestNo=<%=BP.Web.GuestUser.No%>&GuestName=<%=BP.Web.GuestUser.Name%>" target=_blank >
                                    <%=dr["Name"] %>
                                </a>
                            </td>
                            <td>
                                <a href="/WF/Rpt/Search.aspx?FK_Flow=<%=dr["No"] %>&RptNo=ND<%=flowID %>MyRpt">查询</a>
                                | <a href="/WF/Rpt/Group.aspx?FK_Flow=<%=dr["No"] %>&RptNo=ND<%=flowID %>MyRpt">分析</a>
                            </td>
                        </tr>
                        <% } %>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
