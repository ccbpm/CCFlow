<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="True" CodeBehind="BatchStartGroup.aspx.cs" Inherits="CCFlow.WF.BatchStartGroup" %>
<%@ Page language="c#" Inherits="CCFlow.WF.BatchStartGroup" Codebehind="BatchStartGroup.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="UCSys" Src="./Comm/UC/UCSys.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Register src="./Comm/UC/ToolBar.ascx" tagname="ToolBar" tagprefix="uc2" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>感谢您选择:<%=BP.Sys.SystemConfig.DeveloperShortName%>信息列表</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE" />
        <link href='./Comm/Style/Table0.css' rel='stylesheet' type='text/css' />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<script language="JavaScript" src="./Comm/JScript.js" type="text/javascript"></script>
		<script language="javascript" type="text/javascript">
		    function ShowEn(url, wName, h, w) {
		        var s = "dialogWidth=" + parseInt(w) + "px;dialogHeight=" + parseInt(h) + "px;resizable:yes";
		        var val = window.showModalDialog(url, null, s);
		        window.location.href = window.location.href;
		    }
		    function ImgClick() {
		    }
		    function OpenAttrs(ensName) {
		        var url = './Sys/EnsAppCfg.aspx?EnsName=' + ensName;
		        var s = 'dialogWidth=680px;dialogHeight=480px;status:no;center:1;resizable:yes'.toString();
		        val = window.showModalDialog(url, null, s);
		        window.location.href = window.location.href;
		    }
		    function DDL_mvals_OnChange(ctrl, ensName, attrKey) {
		        var idx_Old = ctrl.selectedIndex;
		        if (ctrl.options[ctrl.selectedIndex].value != 'mvals')
		            return;
		        if (attrKey == null)
		            return;

		        var url = 'SelectMVals.aspx?EnsName=' + ensName + '&AttrKey=' + attrKey;
		        var val = window.showModalDialog(url, 'dg', 'dialogHeight: 450px; dialogWidth: 450px; center: yes; help: no');
		        if (val == '' || val == null) {
		            // if (idx_Old==ctrl.options.cont
		            ctrl.selectedIndex = 0;
		            //    ctrl.options[0].selected = true;
		        }
		    }
	</script>
	</HEAD>
	<body   onkeypress="Esc()"   onkeydown='DoKeyDown();' topmargin="0" leftmargin="0" >
		<form id="Form1" method="post" runat="server">
			<table id="Table1" align="left" cellSpacing="1" cellPadding="1" border="0" width="100%">
			<TR>
                <td>
				 <caption style="background:url('../Comm/Style/BG_Title.png') repeat-x ; height:30px ; line-height:30px"   >
						<asp:Label id="Label1" runat="server">Label</asp:Label>
                        </caption>
                        </td>
				</TR>

				<TR>
					<TD class="ToolBar"   >
                        <uc2:ToolBar ID="ToolBar1" runat="server" />
                    </TD>
				</TR>
				<TR align="justify" height="0px" valign=top >
					<TD  width='100%'  >
						<uc1:UCSys id="UCSys1" runat="server"></uc1:UCSys>
					</TD>
				</TR>
				<TR>
					<TD Class="Toolbar" ><FONT face="宋体" size=2>
							<uc1:UCSys id="UCSys2" runat="server"></uc1:UCSys></FONT>
					</TD>
				</TR>
			</TABLE>
			 
		</form>
	</body>
</HTML>

