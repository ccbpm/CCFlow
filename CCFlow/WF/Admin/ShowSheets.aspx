<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_WorkEndSheet" Codebehind="ShowSheets.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
</head>
<body class="Body<%=BP.Web.WebUser.Style%>"   leftMargin=0  topMargin=0>
    <form id="form1" runat="server">
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;
            height: 100%">
            <tr>
                <td valign=top>
                    <uc1:Pub ID="Pub1" runat="server" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
