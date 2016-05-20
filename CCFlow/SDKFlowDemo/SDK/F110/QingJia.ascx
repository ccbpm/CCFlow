<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QingJia.ascx.cs" Inherits="CCFlow.App.F001.Apply" %>

<%@ Register src="../../../WF/SDKComponents/FrmCheck.ascx" tagname="FrmCheck" tagprefix="uc1" %>

<%@ Register src="../../../WF/SDKComponents/DocMainAth.ascx" tagname="DocMainAth" tagprefix="uc2" %>
<%@ Register src="../../../WF/SDKComponents/DocMultiAth.ascx" tagname="DocMultiAth" tagprefix="uc3" %>

<div id="WFStrl">
<asp:Button ID="Btn_Send" runat="server" Text="发送" onclick="Btn_Send_Click" />
<asp:Button ID="Btn_Return" runat="server" Text="退回" onclick="Btn_Return_Click" />
<asp:Button ID="Btn_CC" runat="server" Text="抄送" onclick="Btn_CC_Click" />
<asp:Button ID="Btn_Track" runat="server" Text="轨迹" onclick="Btn_Track_Click" />
<hr>
</div>

<table style="width:80%;aligen:center">
<caption>请假申请单</caption>

<tr>
<td>
<fieldset>
<legend>表单区域</legend>
请假人: 
时间从: 
到: 
</fieldset>
 </td>
</tr>

<tr>
<td> 

<% 
    string str = this.Request.QueryString["FK_Node"];
    if (str != "11001" && 1==3)
    {
       /*如果不是开始节点，就不它显示审核按钮. */
     %>

<fieldset>
<legend>审批区域</legend>
    <uc1:FrmCheck ID="FrmCheck1" runat="server" />
</fieldset>

<%} %>
</td>
</tr>


<tr>
<td> 
<fieldset>
<legend>单附件</legend>
    <uc2:DocMainAth ID="DocMainAth1" runat="server" />
</fieldset>
</td>
</tr>

<tr>
<td> 

<fieldset>
<legend>多附件</legend>
    <uc3:DocMultiAth ID="DocMultiAth1" runat="server" />
</fieldset>
</td>
</tr>

</table>






