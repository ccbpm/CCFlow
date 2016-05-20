<%@ Page Title="Url条件" Language="C#" MasterPageFile="Condition.master" AutoEventWireup="true"
    CodeBehind="CondByUrl.aspx.cs" Inherits="CCFlow.WF.Admin.CondByUrl" %>

<%@ Register Src="UC/CondByUrl.ascx" TagName="CondByUrl" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="server">
    <uc1:CondByUrl ID="CondByUrl1" runat="server" />
</asp:Content>
