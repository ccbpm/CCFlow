<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/WinOpen.master" AutoEventWireup="true" Inherits="SDKFlows_MyForm" Codebehind="MyForm.aspx.cs" %>

<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
&nbsp;&nbsp;&nbsp;&nbsp; 这是个性化表单演示. 
    你可以这里任意自由的发挥，以满足ccflow表单不能完成的客户需求样式，请注意传递给你的参数接收并处理这些参数。这些参数分别是。<br />
&nbsp;&nbsp; 流程编号，工作ID，节点ID ..... 你可以接收这些参数处理它们。<br />
    <br />
    <uc1:Pub ID="Pub1" runat="server" />
    <br />
    <br />
    <br />
</asp:Content>

