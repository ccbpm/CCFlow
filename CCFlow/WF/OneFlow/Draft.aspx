<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.OneFlow.WF_OneFlow_Draft" Codebehind="Draft.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Right" Runat="Server">
    <%
    /*获取数据.*/
    string fk_flow = this.Request.QueryString["FK_Flow"];
    string fk_node = int.Parse(fk_flow)+"01";
        
    System.Data.DataTable dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable(fk_flow);
    %>
<table  style="width:100%">
<Caption class='Caption' align=left style="background:url('../Comm/Style/BG_Title.png') repeat-x ; height:30px ; line-height:30px" >
草稿</Caption>
<tr>
<th class="Title">IDX </th>
<th class="Title">流水号</th>
<th class="Title">流程标题</th>
<th class="Title">时间</th>
<th class="Title">操作</th>
</tr>
<%
    int workid = 0;
    string title, rdt;
    int idx = 0;
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        workid = int.Parse(dr["OID"].ToString());
        title = dr["Title"].ToString();
        rdt = dr["RDT"].ToString();
        idx++;
    %>
   <tr>
   <td class="Idx">
       <asp:CheckBox ID="CB"  Text=""  runat="server" /><%=idx %>
   </td>
   <td class="TD" width="5%"><%=workid%></td>
   <td class="TD" width="80%"><a href="MyFlow.aspx?WorkID=<%=workid %>&FK_Flow=<%=fk_flow %>&FK_Node=<%=fk_node %>" ><img src='../Img/WFState/Draft.png' class="Icon"  border=0/><%=title%></a></td>
   <td class="TD"><%=rdt.Substring(5)%></td>

   <td class="TD"><a href="javascript:DelDraft('<%=workid %>','<%=fk_node %>');" ><img src='/WF/Img/Btn/Delete.gif' class="Icon" >
   
       </a></td>
   </tr>
 <% } %>
</table>
<asp:Button  ID="Btn_Del" runat="server" Text="批量删除" onclick="Del_Click"  />
</asp:Content>

