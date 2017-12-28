<%@ Page Title="" Language="C#" MasterPageFile="~/Prj/MasterPage.master" AutoEventWireup="true" CodeFile="EmpPrjStations.aspx.cs" Inherits="ExpandingApplication_PRJ_EmpPrjStations" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>