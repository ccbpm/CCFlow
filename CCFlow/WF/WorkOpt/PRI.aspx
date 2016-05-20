<%@ Page Title="重要性" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="PRI.aspx.cs" Inherits="SQApp.WF.WorkOpt.PRI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%
    Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);

    BP.WF.GenerWorkFlow gwf = new BP.WF.GenerWorkFlow(workid);
%>

<fieldset>
<legend>重要性
    </legend>
    <asp:RadioButton ID="RadioButton1" Text="高" GroupName="a" runat="server" />
    <br />
    <asp:RadioButton ID="RadioButton2" Text="中" GroupName="a" runat="server" />
    <br />

    <asp:RadioButton ID="RadioButton3" Text="低" GroupName="a" runat="server" />
    <br />

    <asp:Button ID="Button1" runat="server" Text="保存" />
    <asp:Button ID="Button2" runat="server" Text="取消" />

</fieldset>
</asp:Content>
