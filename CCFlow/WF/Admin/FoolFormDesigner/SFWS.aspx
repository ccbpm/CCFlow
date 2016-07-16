<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SFWS.aspx.cs" Inherits="CCFlow.WF.MapDef.SFWS" %>

<%@ Register Src="../../UC/Pub.ascx" TagName="ucsys" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Comm/JScript.js"></script>
    <script type="text/javascript">
        function Esc() {
            if (event.keyCode == 27)
                window.close();
            return true;
        }
    </script>
    <base target="_self" />
</head>
<body topmargin="0" leftmargin="0" onkeypress="Esc()" onload="RSize()">
    <form id="form1" runat="server">
    <div align="left">
        <uc1:ucsys ID="Ucsys1" runat="server" />
    </div>
    </form>
</body>
</html>
