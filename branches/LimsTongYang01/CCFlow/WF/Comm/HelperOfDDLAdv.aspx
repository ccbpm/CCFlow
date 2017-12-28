<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow_Comm_SearchAdv" Codebehind="HelperOfDDLAdv.aspx.cs" %>
<%@ Register Src="UC/ucsys.ascx" TagName="ucsys" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>无标题页</title>
		<LINK href="Menu.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="Menu.js"></script>
		<script language="JavaScript" src="JScript.js"></script>
		<LINK href="Table.css" type="text/css" rel="stylesheet">
</head>
<body class="Body<%=BP.Web.WebUser.Style%>" onkeypress=Esc() leftMargin=0 
topMargin=0 >
    <form id="form1" runat="server">
    <div>
    <table  border=0  align=center >
    <tr>
    <td>
        <uc1:ucsys ID="Ucsys1" runat="server" />
        </td>
        </tr>
        </Table>
    </div>
    </form>
</body>
</html>
