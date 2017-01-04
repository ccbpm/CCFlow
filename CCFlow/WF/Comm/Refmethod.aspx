<%@ Register TagPrefix="uc1" TagName="UCEn" Src="UC/UCEn.ascx" %>

<%@ Page Language="c#" Inherits="CCFlow.Web.Comm.UIRefMethod" CodeBehind="Refmethod.aspx.cs" %>

<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>功能执行:输入参数点确定/执行按钮 - Esc 键关闭窗口.</title>
    <meta content="Microsoft FrontPage 5.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <script language="JavaScript" src="JScript.js"></script>
    <script language="JavaScript" src="Menu.js"></script>
    <base target="_self" />
    <link href="./Style/Table.css" type="text/css" rel="stylesheet">
    <link href="./Style/Table0.css" type="text/css" rel="stylesheet">

     <link href='./Style/Table0.css' rel='stylesheet' type='text/css' />
        <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
        <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
        <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
        <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="JScript.js" type="text/javascript"></script>
		<script language="JavaScript" src="Menu.js" type="text/javascript"></script>
		<script language="JavaScript" src="ShortKey.js" type="text/javascript"></script>
        <script src="./JS/Calendar/WdatePicker.js" type="text/javascript"></script>
        <link href="./JS/Calendar/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
        <link href='./Style/Table0.css' rel='stylesheet' type='text/css' />

</head>
<body onkeypress="javascript:Esc();" leftmargin="0" topmargin="0">
    <form id="Form1" method="post" runat="server">
    <table id="Table1" cellspacing="1" cellpadding="1" width="100%" border="1" class="Table"
        border="0">
       <caption class=ToolBar >
                <asp:Label ID="Label1" runat="server">Label</asp:Label>
          </caption>
        <tr>
            <td class="TD">
                <asp:Button ID="btnSave" runat="server" Text="  执 行  " OnClick="BPToolBar1_ButtonClick" />
                <asp:Button ID="btnClose" runat="server" Text=" 关 闭 " OnClick="btnClose_Click" />
            </td>
        </tr>
        <tr>
            <td class="TD">
                <uc1:UCEn ID="UCEn1" runat="server"></uc1:UCEn>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
