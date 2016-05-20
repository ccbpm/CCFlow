<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="NodeEventEntity.aspx.cs" Inherits="CCFlow.WF.Admin.EventEntity.NodeEventEntity" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="100%">
<caption>节点事件实体类</caption>
<tr>
<td>
    <uc1:Pub ID="Pub1" runat="server" />
    </td>
    </tr>
    </table>
</asp:Content>
