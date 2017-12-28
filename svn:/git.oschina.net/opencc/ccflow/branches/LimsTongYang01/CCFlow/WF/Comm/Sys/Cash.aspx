<%@ Page language="c#" Inherits="CCFlow.Web.Comm.Sys.Cash" Codebehind="Cash.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="UCSys" Src="../UC/UCSys.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Cash</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../Style/Table0.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<FONT face="ËÎÌå">
				<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px" cellSpacing="1"
					cellPadding="1" width="100%" height='100%' border="1">
					<TR>
						<TD height='1%'>
							<uc1:UCSys id="UCSys1" runat="server"></uc1:UCSys></TD>
					</TR>
					<TR>
						<TD valign="top" height='100%'>
							<uc1:UCSys id="UCSys2" runat="server"></uc1:UCSys></TD>
					</TR>
				</TABLE>
			</FONT>
		</form>
	</body>
</HTML>
