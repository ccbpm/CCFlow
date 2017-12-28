<%@ Page language="c#" Inherits="CCFlow.Web.Comm.UI.Exit1" Codebehind="Exit.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Exit</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="Style.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="Exit" method="post" runat="server">
			<FONT face="宋体">
				<cc1:Btn id="Btn_O" style="Z-INDEX: 101; LEFT: 167px; POSITION: absolute; TOP: 163px" runat="server"
					ShowType="Confirm" Text="确定退出" onclick="Btn_O_Click"></cc1:Btn>
				<cc1:Btn id="Btn_C" style="Z-INDEX: 102; LEFT: 269px; POSITION: absolute; TOP: 163px" runat="server"
					ShowType="Cancel"></cc1:Btn>
				<cc1:Btn id="Btn1" style="Z-INDEX: 103; LEFT: 360px; POSITION: absolute; TOP: 160px" runat="server"
					Text="更改用户" onclick="Btn1_Click"></cc1:Btn>
			</FONT>
		</form>
	</body>
</HTML>
