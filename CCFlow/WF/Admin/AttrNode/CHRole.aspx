<%@ Page Title="考核规则" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="CHRole.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.CHRoleUI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style=" width:100%;">
<caption>考核规则</caption>
<tr>
<td>
<%
    int nodeID = int.Parse(this.Request.QueryString["FK_Node"]);
    BP.WF.Node nd = new BP.WF.Node(nodeID);
  %>
<fieldset>
<legend><asp:RadioButton ID="RB_None" Text="不考核"  Checked="true" runat="server" GroupName="xxxx" />
    </legend>
<ul >
<li style=" color:Gray">默认为不考核，当前节点不设置任何形式的考核。</li>
</ul>
</fieldset>

<fieldset>
<legend><asp:RadioButton ID="RB_ByTime" Text="按时效考核" runat="server" GroupName="xxxx" /></legend>

<table style=" width:100%;">
<tr>
<td nowarp=true> 限期完成时限：</td>
<td> <asp:TextBox ID="TB_TSpanDay" runat="server" Width="35px"></asp:TextBox>天,</td>
<td> <asp:TextBox ID="TB_TSpanHour" runat="server" Width="35px"></asp:TextBox>小时.</td>
<td style=" color:Gray">工作量考核是按照小时来计算(必须输入正整数,不能大于8)</td>
</tr>

<tr>
<td> 预警：</td>
<td> <asp:TextBox ID="TB_WarningDay" runat="server" Width="35px"></asp:TextBox>天,</td>
<td> <asp:TextBox ID="TB_WarningHour" runat="server" Width="35px"></asp:TextBox>小时.</td>
<td style=" color:Gray">提前xx天xx小时预警(必须输入正整数)，预警就是提醒该工作应该处理了的时间点。</td>
</tr>

<tr>
<td> 扣分</td>
<td> <asp:TextBox ID="TB_TCent" runat="server" Width="35px"></asp:TextBox>分</td>
<td colspan="2" style=" color:Gray">此分值可以作为时效考核的依据或者参考，每延期1小时的扣分。
<br />如果设置此分值，那末系统就会计算出来得分。该分值，可以转化为奖励或者罚款的金额。
</td>
</tr>


<tr>
<td> 预警提醒规则</td>
<td>  <asp:DropDownList ID="DDL_WAlertRole" runat="server">
    </asp:DropDownList>
    </td>
<td> 提醒方式</td>
<td>  <asp:DropDownList ID="DDL_WAlertWay" runat="server">
    </asp:DropDownList>
    </td>
</tr>

<tr>
<td> 逾期提醒规则</td>
<td>  <asp:DropDownList ID="DDL_TAlertRole" runat="server">
    </asp:DropDownList>
    </td>
<td> 提醒方式</td>
<td>  <asp:DropDownList ID="DDL_TAlertWay" runat="server">
    </asp:DropDownList>
    </td>
</tr>




</table>
</fieldset>
  
<fieldset>
<legend><asp:RadioButton ID="RB_ByWorkNum" Text="按工作量考核" runat="server"  GroupName="xxxx"/>
    </legend>
<ul style=" color:Gray">
<li>按照处理工作的多少进行考核。</li>
<li>这样的节点，一般都是多人处理的节点。</li>
</ul>
</fieldset>



<fieldset>
<legend> 是否是质量考核点？</legend>

<ul style=" color:Gray">
<li>质量考核，是当前节点对上一步的工作进行一个工作好坏的一个考核。</li>
<li>考核的方式是对上一个节点进行打分，该分值记录到WF_CHEval的表里，开发人员对WF_CHEval的数据根据用户的需求进行二次处理。</li>
</ul>

<asp:CheckBox ID="CB_IsEval" runat="server" Text="是否是质量考核点？" />

</fieldset>



    <asp:Button ID="Btn_Save" runat="server" Text=" 保 存 " 
        onclick="Btn_Save_Click" />
</td>

<td valign="top" style="white-space:30%;">
 
</td>

</tr>
</table>

</asp:Content>
