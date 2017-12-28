<%@ Page language="c#" Inherits="CCFlow.Web.Comm.UI.HelperOfDDL" Codebehind="HelperOfDDL.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="UCSys" Src="UC/UCSys.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>ccflow</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="JavaScript" src="JScript.js"></script>
		<base target="_self">
		<LINK href="./Style/Table0.css" type="text/css" rel="stylesheet">
		<script language=javascript>
<!--
        function CloseBar( barid)
        {
        
        }
        function OpenBar( barid )
        {
        }
//-->
</script>
	</HEAD>
	<body   onkeypress=Esc() leftMargin=0 
topMargin=0 >
		<form id="Form1" method="post" runat="server">
				<TABLE  align="left" cellSpacing="1" cellPadding="1" width="100%" border="1" class="Table" >
					 
					<TR >
						<TD  Class="toolbar" width="90%">
                            <strong>选择分组</strong>:<asp:DropDownList ID="DropDownList1" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" AutoPostBack="True">
                            </asp:DropDownList>
                            <asp:Button ID="Btn_OK" class=Btn Visible=false runat="server" OnClick="Btn_OK_Click" Text="确定选择项目" />
                            <asp:Button ID="Btn_Close1" class=Btn runat="server" Visible=false Text="关闭窗口" />
                            提示:您可点击选中并返回。</TD>
					</TR>
					<TR   valign=top  align=left  >
						<TD width="100%"  height="100%">
							<uc1:UCSys id="UCSys1" runat="server"></uc1:UCSys>
						</TD>
					</TR>
				</TABLE>
		</form>
	</body>
</HTML>
