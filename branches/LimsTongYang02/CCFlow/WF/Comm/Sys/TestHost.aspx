<%@ Page language="c#" Inherits="CCFlow.Web.Comm.Sys.TestHost" Codebehind="TestHost.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="UCSys" Src="../UC/UCSys.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>TestHost</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
								<LINK href="../Style/Table0.css" type="text/css" rel="stylesheet">

	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table1" style="WIDTH: 100%; HEIGHT: 100%" cellSpacing="1" cellPadding="1" border="1">
				<TR>
					<TD bgcolor="activeborder">请注意测试之前需要把所有的外部连接关闭掉。</TD>
				</TR>
				<TR>
					<TD>
						<asp:RadioButton id="RadioButton_Select" runat="server" Text="测试海量查询" GroupName="SSS"></asp:RadioButton>
						<asp:RadioButton id="RadioButton_RunSQL" runat="server" Text="测试海量连接" GroupName="SSS"></asp:RadioButton>
						<asp:RadioButton id="RadioButton_OutHtml" runat="server" Text="测试海量输出" GroupName="SSS"></asp:RadioButton>
						<asp:RadioButton id="RadioButton_RunSQLRVal" runat="server" Text="测试海量单值查询" GroupName="SSS"></asp:RadioButton><FONT face="宋体">
						</FONT>
					</TD>
				</TR>
				<TR>
					<TD>
						指定查询语句
						<asp:TextBox id="TextBox_SQL" runat="server" Width="344px">SELECT * FROM DS_TAXPAYER</asp:TextBox><BR>
						执行次数:&nbsp;
						<asp:TextBox id="TextBox_RunTime" runat="server" Width="344px">10</asp:TextBox><BR>
						<asp:Button id="Button1_Do" runat="server" Text="Do" Width="80px" onclick="Button1_Do_Click"></asp:Button>
						<asp:Button id="Button2_Help" runat="server" Text="Help" Width="80px"></asp:Button></TD>
				</TR>
				<TR>
					<TD height='100%' valign="top">
						<uc1:UCSys id="UCSys1" runat="server"></uc1:UCSys><BR>
						<uc1:UCSys id="UCSys2" runat="server"></uc1:UCSys></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
