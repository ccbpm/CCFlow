<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>

<%@ Page Language="c#" Inherits="CCFlow.Web.Comm.Info" CodeBehind="InfoPage.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="UCSys" Src="../UC/UCSys.ascx" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>ÏûÏ¢¿ò</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="Style.css" type="text/css" rel="stylesheet">
    <script language="javascript" src="../JScript.js"></script>
    <script language="javascript" src="../Menu.js"></script>
    <script language="javascript" src="../ActiveX.js"></script>
    <base target="_self" />
    <meta http-equiv="Pragma" content="No-cach">
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
        }
        fieldset
        {
            margin: 0px auto;
            width: 50%;
            border: 0px;
        }
    </style>
</head>
<body class="Body" onkeypress="Esc()">
    <form id="ErrPage" method="post" runat="server">

   
    <uc1:UCSys ID="UCSys1" runat="server"></uc1:UCSys>
    </form>
</body>
</html>
