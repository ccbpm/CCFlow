<%@ Page Title="表单扩展设置" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.MapDef.WF_MapDef_AutoFullUI" CodeBehind="AutoFull_old.aspx.cs" %>
<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link  href="/WF/Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
      <link href="/WF/Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="/WF/Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="/WF/Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="/WF/Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
            <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
