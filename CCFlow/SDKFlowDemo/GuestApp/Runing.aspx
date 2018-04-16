<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/GuestApp/Site.Master"
    AutoEventWireup="true" CodeBehind="Runing.aspx.cs" Inherits="CCFlow.SDKFlowDemo.GuestApp.Runing" %>

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
                在途流程：
            </div>
            <div class="info_cen_content">
                <div id="printDiv">
                    <%
                        System.Data.DataTable dt = BP.WF.Dev2InterfaceGuest.DB_GenerRuning();
                    %>
                    <table style="width: 100%;">
                        <tr>
                            <th>序</th>
                            <th> 标题</th>
                            <th>流程名称</th>
                            <th>停留节点</th>
                            <th>处理人</th>
                        </tr>
                        <%
                            int idx = 0;
                            foreach (System.Data.DataRow dr in dt.Rows)
                            {
                                idx++;
                                int workID = int.Parse(dr["WorkID"].ToString());
                                string title = dr["Title"].ToString();
                                string flowNo = dr["FK_Flow"].ToString();
                                string flowName = dr["FlowName"].ToString();
          
          
                        %>
                        <tr>
                            <td class="Idx">
                                <%=idx %>
                            </td>
                            <td>
                                <a href="javascript:WinOpen('/WF/WFRpt.htm?FK_Flow=<%=dr["FK_Flow"] %>&GuestNo=<%=BP.Web.GuestUser.No%>&GuestName=<%=BP.Web.GuestUser.Name%>&WorkID=<%=workID %>')">
                                    <%=title %>
                                </a>
                            </td>
                            <td><%=dr["FlowName"] %></td>
                            <td> <%=dr["NodeName"] %> </td>
                            <td> <%=dr["TodoEmps"]%> </td>

                        </tr>
                        <% } %>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
