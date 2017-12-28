<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Register TagPrefix="uc1" TagName="UCSys" Src="../UC/UCSys.ascx" %>
<%@ Page language="c#" Inherits="CCFlow.Web.Comm.UI.ToErrPage" Codebehind="ToErrorPage.aspx.cs" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>
			<%=BP.Sys.SystemConfig.SysName%>
		</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script src="../JScript.js" language="javascript"></script>
		<script language="javascript" src="../ActiveX.js"></script>
		<LINK href="../Style.css" type="text/css" rel="stylesheet">
		<meta http-equiv="Pragma" Content="No-cach">
         <script  type="text/javascript">
             function Esc() {
                 if (event.keyCode == 27)
                     window.close();
                 return true;
             }
        </script>
	</HEAD>
	<body class="Body<%=BP.Web.WebUser.Style%>"  onkeypress="Esc()" >
		<form id="ErrPage" method="post" runat="server">
			<FONT face="宋体">
				<br>
				<br>
				<TABLE id="Table1" align="center" height='80%' cellSpacing="1" cellPadding="1" width="95%"
					border="1">
					<TBODY>
						<TR bgColor="#ff9900" height="0">
							<TD style="BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; BORDER-LEFT: black 1px solid; BORDER-BOTTOM: black 1px solid; HEIGHT: 24px"><FONT face="宋体"><FONT face="宋体"><p align="center"><STRONG>&nbsp;提示信息：&nbsp;</STRONG>
									</FONT></P></FONT>
			</FONT></TD></TR>
			<TR bgColor="#eeeeee" height="100%" align="justify">
				<TD valign="top" width="100%" align="left" rowSpan="1"><FONT face="宋体">
						<uc1:UCSys id="UCSys1" runat="server"></uc1:UCSys>
					</FONT>
				</TD>
			</TR>
			<TR bgColor="#ff9900" height="0" align="center">
				<TD valign="top" height="0"><FONT face="宋体" color="#000066" size="2">
						<%=BP.Sys.SystemConfig.ServiceMail%>，<%=BP.Sys.SystemConfig.ServiceTel%>
					</FONT>
				</TD>
			</TR>
			</TBODY></TABLE></FONT>
		</form>
	</body>
</HTML>
