<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StartSubFlow.ascx.cs" Inherits="CCFlow.WF.SDKComponents.Templelte.StartSubFlow" %>
<h3>发起子流程demo</h3>

流程编号:<asp:TextBox ID="TB_FlowNo" runat="server"></asp:TextBox>
<p>
    子流程发送到的第二个节点,如果为空就让流程自动寻找,<asp:TextBox ID="TB_NodeID" runat="server"></asp:TextBox>
</p>
子流程第二个节点的接受人,如果为空就让流程自动寻找,多个人用逗号分开,<asp:TextBox ID="TB_ToEmps" runat="server"></asp:TextBox>
<br />
<br />
<asp:Button ID="Btn_Start" runat="server" Text="启动子流程" 
    onclick="Btn_Start_Click" />
