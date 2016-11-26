<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="ExcelFrm.aspx.cs" Inherits="CCFlow.WF.MapDef.MapExtUI.ExcelFrmUI" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link  href="../../../Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
      <link href="../../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../../../Scripts/EasyUIUtility.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
