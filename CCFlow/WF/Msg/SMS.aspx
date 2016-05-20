<%@ Page Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Msg.WF_SMS" Title="未命名頁面" Codebehind="SMS.aspx.cs" %>
<%@ Register src="../UC/SMS.ascx" tagname="SMS" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class=BigDoc>
    <uc1:SMS ID="SMS1" runat="server" />
    </div>
    </asp:Content>

