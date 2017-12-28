<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Page language="c#" Inherits="CCFlow.Web.Comm.UI.WF.ChangePass12" Codebehind="ChangePass.aspx.cs" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>ChangePass</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language="JavaScript" src="../JScript.js"></script>
		<base target=_self />
	</HEAD>
	<body class="Body<%=BP.Web.WebUser.Style%>"   >
		<form id="ChangePass" method="post" runat="server">
			  
				<TABLE id="Table1"  style="Z-INDEX: 101; LEFT: 143px; WIDTH: 261px; POSITION: absolute; TOP: 100px; HEIGHT: 120px"
					cellSpacing="1" cellPadding="1" width="261" border="1" >
					<TR>
						<TD colspan="2">
							<asp:Label id="Label1" runat="server">Label</asp:Label></TD>
					</TR>
					<TR>
						<TD style="WIDTH: 133px">旧密码:</TD>
						<TD>
							<cc1:TB id="TB1" runat="server" TextMode="Password"></cc1:TB></TD>
					</TR>
					<TR>
						<TD style="WIDTH: 133px; HEIGHT: 27px">新密码:</TD>
						<TD style="HEIGHT: 27px">
							<cc1:TB id="TB2" runat="server" TextMode="Password"></cc1:TB></TD>
					</TR>
					<TR>
						<TD style="WIDTH: 133px">确认:</TD>
						<TD>
							<cc1:TB id="TB3" runat="server" TextMode="Password"></cc1:TB></TD>
					</TR>
					<TR>
						<TD style="WIDTH: 133px"></TD>
						<TD>
							<cc1:Btn id="Btn_Save" runat="server" Text="确认(O)" accessKey="O" ShowType="Confirm"></cc1:Btn>
							<cc1:Btn id="Btn_C" runat="server" accessKey="C" ShowType="Cancel"></cc1:Btn></TD>
					</TR>
				</TABLE>
		</form>
	</body>
</HTML>
