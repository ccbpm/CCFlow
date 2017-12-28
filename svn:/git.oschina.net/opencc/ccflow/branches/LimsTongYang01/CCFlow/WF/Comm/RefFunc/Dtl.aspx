<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.Comm.RefFunc.Dtl" CodeBehind="Dtl.aspx.cs" %>

<%@ Register Src="RefLeft.ascx" TagName="RefLeft" TagPrefix="uc2" %>
<%@ Register Src="RefDtl.ascx" TagName="RefDtl" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc2:RefLeft ID="RefLeft1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <uc1:RefDtl ID="RefDtl1" runat="server" />
</asp:Content>
