<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/GuestApp/Site.Master"
    AutoEventWireup="true" CodeBehind="Todolist.aspx.cs" Inherits="CCFlow.SDKFlowDemo.GuestApp.Todolist" %>

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
          
            <div class="info_cen_content">
                <div id="printDiv">
                    <%
                        System.Data.DataTable dt = BP.WF.Dev2InterfaceGuest.DB_GenerEmpWorksOfDataTable(BP.Web.GuestUser.No);
                        if (dt.Rows.Count == 0)
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "hide", "hide();", true);
                        else
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "show", "show();", true);
                    %>
                    <table style="width: 100%;">
                        <tr>
                            <th> 序 </th>
                            <th> 标题 </th>
                            <th> 流程</th>
                            <th>停留节点</th>
                            <th>发送人 </th>
                            <th>发送时间</th>
                        </tr>
                        <%
                            int idx = 0;
                            foreach (System.Data.DataRow dr in dt.Rows)
                            {
                                idx++;
                        %>
                        <tr>
                            <td>
                                <%=idx %>
                            </td>
                            <td><a href="javascript:void(0)" onclick="window.open('/WF/MyFlow.aspx?WorkID=<%=dr["WorkID"] %>&FK_Flow=<%=dr["FK_Flow"] %>&FK_Node=<%=dr["FK_Node"] %>&FID=<%=dr["FID"] %>')">
                                    <%=dr["Title"] %></a>
                            </td>
                            <td> <%=dr["FlowName"] %> </td>
                            <td><%=dr["NodeName"] %></td>
                            <td><%=dr["Sender"] %></td>
                            <td> <%=dr["RDT"] %> </td>
                        </tr>
                        <%
                            }
                        %>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
