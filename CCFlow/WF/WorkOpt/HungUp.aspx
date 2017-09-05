<%@ Page Title="挂起" Language="C#" MasterPageFile="../WinOpen.master"
 AutoEventWireup="true" Inherits="CCFlow.WF.WF_Hurry" Codebehind="HungUp.aspx.cs" %>
<%@ Register src="../UC/Pub.ascx" tagname="Pub" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
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
    <script language="JavaScript" src="../Comm/JS/Calendar/WdatePicker.js" type="text/javascript" ></script>
        <link href="../Comm/JS/Calendar/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

 <table style="text-align:left;width:100%; border:1px; " >
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %>  -  工作挂起与取消</caption>
<tr>
<td  style=" text-align:center">
    <br/>

 <table style=" text-align:left; width:500px">
 <tr>
 <td>
    <uc2:Pub ID="Pub1" runat="server" />
    </td>
 </tr>
 </table>
 
    <br>
    </td>
</tr>
</table>
</asp:Content>

