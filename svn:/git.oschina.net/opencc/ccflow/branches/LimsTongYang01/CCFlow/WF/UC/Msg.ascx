<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.Msg" Codebehind="Msg.ascx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>

		<script language="javascript" for="document" event="onkeydown">
<!--
  if(event.keyCode==13)
     event.keyCode=9;
-->
		</script>
		<script language="JavaScript" src="Flow.js"></script>
		<script language=javascript>
		function Open( fk_flow )
		{
		   window.open('MyFlow.aspx?FK_Flow='+fk_flow+'&IsClose=1' , 'f' + fk_flow + '',  'width=700,top=100,left=200,height=400,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
		}
		</script>
		
<table border=0 width='90%' align=center>
<tr>
<TD valign=top width='20%' align=left ><uc1:Pub ID="Left" runat="server" /></TD>
<TD valign=top><uc1:Pub ID="Pub1" runat="server" /></TD>
    </tr>
    </table>
