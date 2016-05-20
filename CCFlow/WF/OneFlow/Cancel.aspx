<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.OneFlow.WF_OneFlow_Cancel" Codebehind="Cancel.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Right" Runat="Server">
    <% 
    /*获取数据.*/
    string fk_flow = this.Request.QueryString["FK_Flow"];
   // System.Data.DataTable dt = BP.WF.Dev2Interface.DB_NDxxRpt(fk_flow, BP.WF.WFState.Delete);
    System.Data.DataTable dt = BP.WF.Dev2Interface.DB_GenerDeleteWorkList(BP.Web.WebUser.No,fk_flow);
        
    %>
<table  style="width:100%">
<Caption class='Caption' align="left" style="background:url('../Comm/Style/BG_Title.png') repeat-x ; height:30px ; line-height:30px" >
撤销</Caption>
<tr>
<th class="Title" width="1%">IDX</th>
<th class="Title">流水号</th>
<th class="Title">流程标题</th>
<th class="Title">时间</th>
<th class="Title">操作</th>
</tr>
<%
    int workid = 0;
    string title, rdt, fid;
    int idx = 0;
    string appPath = this.Request.ApplicationPath;
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        workid = int.Parse(dr["WorkID"].ToString());
        title = dr["Title"].ToString();
        rdt = dr["RDT"].ToString();
        fid = dr["FID"].ToString();
        idx++;
   %>
   <tr>
   <td class="Idx">
       <asp:CheckBox ID="CB"  Text=""  runat="server" /><%=idx %>
   </td>
   <td class="TD" width="5%" width="1%" ><%=workid%></td>
   <td class="TD" width="75%"><a href="javascript:WinOpen('../WFRpt.aspx?WorkID=<%=workid%>&FK_Flow=<%=fk_flow%>&FID=<%=fid%>')" ><img src='../Img/Action/FlowOverByCoercion.png' class="Icon"  /><%=title %></a></td>
   <td class="TD" width="8%" ><%=rdt.Substring(5)%></td>
   <td class="TD" width="8%" ><img src='../Img/Action/RebackFlow.png' class="Icon" />恢复启用</td>
   </tr>
 <% } %>
</table>
</asp:Content>

