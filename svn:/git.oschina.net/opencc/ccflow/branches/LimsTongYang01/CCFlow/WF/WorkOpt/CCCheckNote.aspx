<%@ Page Title="抄送审核" Language="C#" MasterPageFile="../SDKComponents/Site.master" AutoEventWireup="true" CodeBehind="CCCheckNote.aspx.cs" Inherits="CCFlow.WF.WorkOpt.CCCheckNote" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <script type="text/javascript">
     function NoSubmit(ev) {
         if (window.event.srcElement.tagName == "TEXTAREA")
             return true;
         if (ev.keyCode == 13) {
             window.event.keyCode = 9;
             ev.keyCode = 9;
             return true;
         }
         return true;
     }
    </script>
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


 <table style=" text-align:left; width:100%">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %></caption>
<tr>
<td valign=top style=" text-align:center">
    <br>
    <br>


<table style="width:500px" >

<tr>
<th valign=top>
<%  BP.WF.Template.FrmWorkCheck en = new BP.WF.Template.FrmWorkCheck(int.Parse(this.Request.QueryString["FK_Node"])); %>
<%=en.FWCOpLabel %>
</th>
</tr>

<tr>
<td valign=top>
    <asp:TextBox ID="TextBox1" runat="server" Height="94px" TextMode="MultiLine" 
        Width="100%"></asp:TextBox>
    </td>
</tr>
<tr>
<td align=right>
    <asp:Button ID="Btn_OK" runat="server" Text="确定" onclick="Btn_OK_Click" />
    <asp:Button ID="Btn_Cancel" runat="server" OnClientClick="window.close();return false;" Text="取消" />
    </td>
</tr>
</table>


<br>
<br>
<br>
<br>
<br>
<br>



 
    </td>
</tr>
</table>
</asp:Content>
