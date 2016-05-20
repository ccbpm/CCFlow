<%@ Register TagPrefix="uc1" TagName="UCEn" Src="UC/UCEn.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Page language="c#" Inherits="CCFlow.Web.Comm.UIRefMethod1" Codebehind="Method.aspx.cs" %>
<%@ Register Src="UC/ucsys.ascx" TagName="ucsys" TagPrefix="uc2" %>

<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Esc ¼ü¹Ø±Õ´°¿Ú.</title>
		<meta content="Microsoft FrontPage 5.0" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="JavaScript" src="JScript.js"></script>
		<script language="JavaScript" src="Menu.js"></script>
		<LINK href="./Style/Table.css" type="text/css" rel="stylesheet">

		<base target="_self" />
		<script language="javascript" for="document" event="onkeydown">
<!--
 if (window.event.srcElement.tagName="TEXTAREA") 
     return false;
  if(event.keyCode==13)
     event.keyCode=9;
-->
</script>
	</HEAD>
	<body  onkeypress=Esc() leftMargin=0 topMargin=0>
		<form id="Form1" method="post" runat="server">
        <br />
        <table style="width:80%;align:center" align=center>
        <tr>
        <td>
						    <uc1:UCEn id="UCEn1" runat="server"></uc1:UCEn>
                            </td>
                            </tr>
         </table>
		</form>
	</body>
</HTML>