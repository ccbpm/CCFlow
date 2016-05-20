<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="uc1" TagName="UCSys" Src="UC/UCSys.ascx" %>
<%@ Page language="c#" Inherits="CCFlow.Web.Comm.Contrast" Codebehind="Contrast.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="UCEn" Src="UC/UCEn.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="UC/ToolBar.ascx" tagname="ToolBar" tagprefix="uc2" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>感谢您选择:<%= BP.Sys.SystemConfig.DeveloperShortName %>
			-> 信息列表</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="JavaScript" src="JScript.js"></script>
		<script language="JavaScript" src="Menu.js"></script>
		<LINK href="./Style/Table0.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body  onkeypress="Esc()" onclick="HideMenu('WebMenu1')" leftMargin=0 background="BJ1.gif" topMargin=0  >
		<form id="Form1" method="post" runat="server">
			<TABLE class="Table" id=Table1 cellSpacing=1 
cellPadding=0 width="100%" height='100%' align=left border=1 >
				<TR>
					<TD colSpan="2" height="0"><asp:label id="Label1" runat="server">Label</asp:label></TD>
				</TR>
				<TR>
					<TD colSpan="2" >
						<uc2:ToolBar ID="ToolBar1" runat="server" />
                    </TD>
				</TR>
				<TR  valign="top" height='100%'>
					<TD vAlign="top" noWrap width='20%'>
						<TABLE  id="Table1"   width='100%' height="0%" border=0 >
							<TR>
								<TD  class="GroupTitle"  >比较对象</TD>
							</TR>
							<TR>
								<TD>
									<cc1:DDL id="DDL_ContrastKey" runat="server"></cc1:DDL></TD>
							</TR>
							<TR>
								<TD style="height: 23px">
									<asp:Label id="Label2" runat="server">Label</asp:Label>:</TD>
							</TR>
							<TR>
								<TD>
									<cc1:DDL id="DDL_M1" runat="server"></cc1:DDL>
                                    <uc1:UCSys ID="UCBtn1" runat="server" />
								</TD>
							</TR>
							<TR>
								<TD>
									<asp:Label id="Label3" runat="server">Label</asp:Label>:</TD>
							</TR>
							<TR>
								<TD>
									<cc1:DDL id="DDL_M2" runat="server"></cc1:DDL>
                                    <uc1:UCSys ID="UCBtn2" runat="server" />
                                </TD>
							</TR>
							<TR>
								<TD  class="GroupTitle"  >分类条件:</TD>
							</TR>
							<TR>
								<TD><cc1:DDL id="DDL_Key" runat="server"></cc1:DDL></TD>
							</TR>
							<TR>
								<TD class="GroupTitle" >分析项目</TD>
							</TR>
							<TR>
								<TD>
									<cc1:ddl id="DDL_GroupField" runat="server"></cc1:ddl></TD>
							</TR>
							<TR>
								<TD class="GroupTitle" >分析方式</TD>
							</TR>
							<TR>
								<TD><cc1:ddl id="DDL_GroupWay" runat="server"></cc1:ddl>
									<cc1:ddl id="DDL_OrderWay" runat="server"></cc1:ddl>
								</TD>
							</TR>
						</TABLE>
					</TD>
					<TD  valign="top" width='100%'><uc1:ucsys id="UCSys1" runat="server"></uc1:ucsys>
					</TD>
				</TR>
			</TABLE>
			</form>
	</body>
</HTML>
