<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.Comm.RefFunc.UFDot2Dot" Title="" Codebehind="Dot2Dot.aspx.cs" %>
<%@ Register src="Dot2Dot.ascx" tagname="Dot2Dot" tagprefix="uc1" %>
<%@ Register src="RefLeft.ascx" tagname="RefLeft" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <base target=_self />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc2:RefLeft ID="RefLeft1" runat="server" />
    </asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <uc1:Dot2Dot ID="Dot2Dot1" runat="server" />
</asp:Content>