<%@ Page Title="请您选择要办理的业务" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="StartGuideFrms.aspx.cs" Inherits="CCFlow.WF.WorkOpt.StartGuideFrms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%
     string flowNo=this.Request.QueryString["FK_Flow"];
     BP.WF.Template.FrmNodes fns = new BP.WF.Template.FrmNodes(int.Parse(flowNo+"01") );
     %>

     <fieldset>
     <legend> <h2>请选择您要办理的业务</h2> </legend>
     <ul>
     <% 
         foreach (BP.WF.Template.FrmNode item in fns)
         {
             if (item.FrmEnableRole != BP.WF.Template.FrmEnableRole.WhenHaveFrmPara)
                 continue;
             
             string url = "../MyFlow.aspx?FK_Flow=" + flowNo + "&FK_Node=" + int.Parse(flowNo) + "01&WorkID=0&IsCheckGuide=1";
             url += "&Frms=" + item.FK_Frm;
           %>
          <li> <a href="<%=url %>"> <%=item.HisFrm.Name %> </a> </li>
        <% } %>
     </ul>
     </fieldset>
</asp:Content>
