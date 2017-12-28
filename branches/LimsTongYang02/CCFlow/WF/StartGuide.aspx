<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" CodeBehind="StartGuide.aspx.cs" Inherits="CCFlow.WF.StartGuide" %>

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <uc1:Pub ID="Pub1" runat="server" />
    <br />
    <%--<hr style=" background-color:White">--%>
    <uc1:Pub ID="Pub2" runat="server" />

</asp:Content>
