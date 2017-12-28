<%@ Page language="c#" Inherits="CCFlow.Web.Comm.Sys.TestCash" Codebehind="TestCash.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="UCSys" Src="../UC/UCSys.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Test</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
						<LINK href="../Style/Table0.css" type="text/css" rel="stylesheet">

	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 48px; POSITION: absolute; TOP: 64px" cellSpacing="1"
				cellPadding="1" width="300" border="1">
				<TR>
					<TD>
						<asp:Button id="Button1" runat="server" Text="Button" onclick="Button1_Click"></asp:Button>
						<asp:Button id="Button2" runat="server" Text="Button" onclick="Button2_Click"></asp:Button></TD>
				</TR>
				<TR>
					<TD>
						<uc1:UCSys id="UCSys1" runat="server"></uc1:UCSys></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
