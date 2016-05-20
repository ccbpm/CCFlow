<%@ Page Title="" Language="C#" MasterPageFile="~/WF/CCForm/WinOpen.master" AutoEventWireup="true" CodeBehind="DtlOpSelectItems.aspx.cs" Inherits="CCFlow.WF.CCForm.DtlOpSelectItems" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
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

<base target=_self />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table width="100%">
<caption> <b><asp:Label id="Label1" runat="server">Label</asp:Label></b> </caption>
<tr>
<td class="ToolBar" >
<uc1:Pub ID="Pub2" runat="server" />
</td>
</tr>

<tr>
<td>
<uc1:Pub ID="Pub3" runat="server" />
</td>
</tr>
</table>

    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
