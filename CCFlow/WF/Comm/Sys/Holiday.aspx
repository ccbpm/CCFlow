<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Holiday.aspx.cs" Inherits="CCFlow.WF.Comm.Sys.Holiday" %>

<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>节假日设置</title>
    <link href="../Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Style/Table0.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">

    <center>
     <table border=1  >
     <caption>节假日设置</caption>
     <tr>
     <th>序</th>
     <th>月份</th>
     <th>周日</th>
     <th>周一</th>
     <th>周二</th>
     <th>周三</th>
     <th>周四</th>
     <th>周五</th>
     <th>周六</th>
     </tr>
     <uc1:Pub ID="Pub1" runat="server" />

     </table>
     <hr />
         <asp:Button ID="Btn_Save" runat="server" Text="保存" 
         onclick="Btn_Save_Click" />
     </center>

    </form>
</body>
</html>
