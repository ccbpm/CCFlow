<%@ Page Title="抄送" Language="C#" MasterPageFile="SiteMenu.Master" AutoEventWireup="true" CodeBehind="CC.aspx.cs" Inherits="CCFlow.App.CC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%
    string ShowType = this.Request.QueryString["ShowType"];
    if (ShowType == null)
        ShowType = "0";
    
    //获取抄送工作。
    System.Data.DataTable dt = null;
    if (ShowType == "0")
    {
        /*未读的抄送*/
        dt = BP.WF.Dev2Interface.DB_CCList_UnRead(BP.Web.WebUser.No);
    }
    if (ShowType == "1")
    {
        /*已读的抄送*/
        dt = BP.WF.Dev2Interface.DB_CCList_Read(BP.Web.WebUser.No);
    }
    if (ShowType == "2")
    {
        /*删除的抄送*/
        dt = BP.WF.Dev2Interface.DB_CCList_Delete(BP.Web.WebUser.No);
    }
    
   // 输出结果
   %>
    <table style="width:90%;"  >
   
   <caption>抄送 (<%=BP.WF.Dev2Interface.Todolist_CCWorks %>) 
    [<a href='CC.aspx?ShowType=0'>未读</a>][<a href='CC.aspx?ShowType=1'>已读</a>][<a href='CC.aspx?ShowType=2'>删除</a>]
   </caption>
   <tr>
   <th>抄送人</th>
   <th>标题</th>
   <th>流程</th>
   <th>发起时间</th>
   <th>详细信息</th>
   </tr>
   
      <%
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        string workid = dr["WorkID"].ToString();
        string fid = dr["FID"].ToString();
        string fk_flow = dr["FK_Flow"].ToString();
        string fk_node = dr["FK_Node"].ToString();

        string url = "../WF/WFRpt.htm?FK_Flow="+fk_flow+"&FK_Node="+fk_node+"&WorkID="+workid+"&FID="+fid;
        
        %>
        <tr>
        <td><%=dr["Rec"]%></td>
        <td><%=dr["FlowName"]%></td>
        <td><%=dr["RDT"]%></td>
        <td><%=dr["NodeName"]%></td>
        <td><a href='<%=url %>' target="_blank" >详细</a></td>
        </tr>

        <tr>
        <td colspan=5 >
        标题：<%=dr["Title"] %>
        <hr />
        抄送内容:
        <hr>
        
        <%=dr["Doc"]%>
        
        </td>

        </tr>


   <% } %> 
   </table>


</asp:Content>
