<%@ Page Title="交叉分析" Language="C#" MasterPageFile="Single.Master" AutoEventWireup="true"
    CodeBehind="D3.aspx.cs" Inherits="CCFlow.WF.Rpt.D3" %>

<%@ Register Src="UC/D3.ascx" TagName="D3" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Comm/JS/Calendar/WdatePicker.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:D3 ID="D31" runat="server" />
</asp:Content>
