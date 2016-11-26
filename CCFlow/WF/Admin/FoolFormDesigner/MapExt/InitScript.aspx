<%@ Page Title="内置表单脚本" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="InitScript.aspx.cs" Inherits="CCFlow.WF.MapDef.MapExtUI.InitScriptUI" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link  href="../../../Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
     <script type="text/javascript" >
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
</asp:Content>
