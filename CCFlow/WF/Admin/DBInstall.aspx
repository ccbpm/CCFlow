<%@ Page Title="ccflow数据库修复与安装" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_DBInstall" Codebehind="DBInstall.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<center>
<table  style="text-align:center;width:600px" >
<tr>
<td>

    <div style='float:left' >数据库修复与安装.</div>
     <div style='float:right' > <img src='../../DataUser/Icon/LogBiger.png' width="300px" border="0px" /> </div>

    <uc1:Pub ID="Pub1" runat="server" />
    </td>
    </tr>
    </table>
    </div>
    </center>
</asp:Content>

