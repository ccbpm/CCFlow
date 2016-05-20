<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocRelease.ascx.cs" Inherits="CCFlow.SDKFlowDemo.SDK.F111.DocRelease" %>
<%@ Register src="/WF/SDKComponents/FrmCheck.ascx" tagname="FrmCheck" tagprefix="uc2" %>
<%@ Register src="/WF/SDKComponents/DocMainAth.ascx" tagname="DocMainAth" tagprefix="uc1" %>
<%@ Register src="/WF/SDKComponents/DocMultiAth.ascx" tagname="DocMultiAth" tagprefix="uc3" %>

<asp:Button ID="Btn_Send" runat="server" Text="发送" onclick="Btn_Send_Click" />
<asp:Button ID="Btn_Return" runat="server" Text="退回" onclick="Btn_Return_Click" />
<asp:Button ID="Btn_Track" runat="server" Text="轨迹" onclick="Btn_Track_Click" />
<asp:Button ID="Btn_AskForHelp" runat="server" Text="加签" onclick="Btn_AskForHelp_Click" />
<asp:Button ID="Btn_CC" runat="server" Text="抄送" onclick="Btn_CC_Click" />

<hr>

<div  id="Div_Msg" >
</div>

<div style="text-align:center">
</div>

<table style="width:100%">
<caption><font color="red"><H1>深圳人大公文</H1> </font> </caption>
<tr>
<td>发文单位</td>
<td>发文单位</td>
</tr>

<tr>
<td>正文</td>
<td colspan=1>
    <uc1:DocMainAth ID="DocMainAth1" runat="server" />
    </td>
</tr>

<tr>
<td colspan=2>
附件:<br />
    <uc3:DocMultiAth ID="DocMultiAth1" runat="server" />
    </td>
</tr>

 
<tr>
<td colspan="2" >
    <uc2:FrmCheck ID="FrmCheck1" runat="server" />
    </td>
</tr>


</table>

