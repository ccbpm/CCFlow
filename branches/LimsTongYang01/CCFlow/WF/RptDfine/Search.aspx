<%@ Page Title="查询" Language="C#" MasterPageFile="Single.Master" AutoEventWireup="true"
    Inherits="CCFlow.WF.Rpt.WF_Rpt_Search" CodeBehind="Search.aspx.cs" %>

<%@ Register Src="UC/Search.ascx" TagName="Search" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Comm/JS/Calendar/WdatePicker.js" type="text/javascript"></script>
    <script language="JavaScript" src="../Comm/JScript.js" type="text/javascript"></script>
</asp:Content> 
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc2:Search ID="Search1" runat="server" />
</asp:Content>
