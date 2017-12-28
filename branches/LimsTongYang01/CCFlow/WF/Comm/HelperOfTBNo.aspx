<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow_Comm_HelperOfTBNum" Codebehind="HelperOfTBNo.aspx.cs" %>
<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<%@ Register Src="UC/ucsys.ascx" TagName="ucsys" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
		<title>快速获取编号, Esc 键关闭窗口.</title>
		<meta content="Microsoft FrontPage 5.0" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		
		<LINK href="Menu.css" type="text/css" rel="stylesheet">
		<LINK href="Table.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="JScript.js"></script>
		<script language="JavaScript" src="Menu.js"></script>
		<script language="JavaScript" src="ActiveX.js"></script>
		
	</HEAD>
	<body onkeypress="Esc()" leftMargin="0" topMargin="0" class=Body >
    <form id="form1" runat="server">
    <div>
        <table border="0" cellpadding="0"   cellspacing="0" style="width: 100%; height: 100%">
            <tr>
                <td style="height: 1px">
                   <uc1:ucsys ID="UcTitle" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="height: 1px" bgcolor=ActiveBorder>
                    关键字:<asp:TextBox ID="TextBox1" runat="server" Width="133px"></asp:TextBox>
                    <asp:DropDownList ID="DropDownList1" runat="server">
                    </asp:DropDownList>
                    <asp:Button ID="Button1" class=Btn runat="server" OnClick="Button1_Click" Text="  查  找  " /></td>
            </tr>
            <tr>
                <td>
                    <uc1:ucsys ID="Ucsys1" runat="server" />
                    <uc1:ucsys ID="Ucsys2" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
