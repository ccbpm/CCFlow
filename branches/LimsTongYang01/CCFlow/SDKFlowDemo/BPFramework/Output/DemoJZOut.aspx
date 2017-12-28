<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DemoJZOut.aspx.cs" Inherits="CCFlow.SDKFlowDemo.DemoJZOut" %>

<%@ Register src="../../Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>数据矩阵输出(N宫格输出)</title>
    <link href="/DataUser/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="/DataUser/Style/Table.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <uc1:Pub ID="Pub1" runat="server" />
    
    </div>
    </form>
</body>
</html>
