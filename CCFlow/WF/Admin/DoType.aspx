<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_DoType" Codebehind="DoType.aspx.cs" %>

<%@ Register Src="../Comm/UC/ucsys.ascx" TagName="ucsys" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>

    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />

</head>
<body  topmargin="0" leftmargin="0"   style="font-size:smaller">
    <form id="form1" runat="server">

    <table style=" width:100%;">
    <caption><%=this.Title %></caption>

    <tr>
    
    <td>
    <div  style='width:80%' >
        <uc1:ucsys ID="Ucsys1" runat="server" />
    </div>
    </td>


    </tr>
    </table>
    </form>
</body>
</html>
