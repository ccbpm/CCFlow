<%@ Page Title="发送阻塞模式" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="BlockModel.aspx.cs" Inherits="CCFlow.WF.Admin.AttrNode.BlockModelUI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style="width:100%;">
<caption>发送阻塞模式 </caption>
<tr>


<td valign="top"> 
<fieldset>
<legend><asp:RadioButton ID="RB_None" Text="不阻塞" GroupName="xxx" runat="server" />
    </legend>
 <ul  style="color:Gray;">
 <li>默认模式，不阻塞。</li>
 <li>如果以下几种模式不能满足需求就可以在发送成功前的事件里抛出异常，阻止向下运动。</li>
 </ul>
</fieldset>


<fieldset>
<legend><asp:RadioButton ID="RB_CurrNodeAll" Text="当前节点有未完成的子流程时" runat="server"  GroupName="xxx" /></legend>
 <ul  style="color:Gray;">
 <li> 当前节点吊起了子流程，并且有未完成的子流程时就不能向下运动。</li>
 </ul>
</fieldset>


<fieldset>
<legend><asp:RadioButton ID="RB_SpecSubFlow" Text="按约定格式阻塞未完成子流程" runat="server"  GroupName="xxx" /></legend>
<a href="javascript:ShowHidden('flows')">请设置表达式:</a>

<div id="flows" style="color:Gray; display:none">
 <ul>
 <li>当该节点向下运动时，要检查指定的历史节点曾经启动的指定的子流程全部完成，作为条件。</li>
 <li>例如：在D节点上，要检查曾经在C节点上启动的甲子流程是否全部完成，如果完成就不阻塞。</li>
 <li>配置格式：@指定的节点1=子流程编号1@指定的节点n=子流程编号n。</li>
 </ul>
 </div>
 <asp:TextBox ID="TB_SpecSubFlow" runat="server" Width="95%"></asp:TextBox>
</fieldset>


<fieldset>
<legend><asp:RadioButton ID="RB_SQL" Text="按照SQL阻塞" runat="server"  GroupName="xxx" /></legend>
<a href="javascript:ShowHidden('sql')">请输入SQL:</a>
<div id="sql" style="color:Gray; display:none">
 <ul>
 <li>配置一个SQL，该SQL返回一行一列的数值类型的值。</li>
 <li>如果该值大于0，就是true, 否则就是false.</li>
 <li>配置的参数支持ccbpm表达式。</li>
 </ul>
 </div>
<asp:TextBox ID="TB_SQL" runat="server"  TextMode="MultiLine" Rows="1"  Width="95%" ></asp:TextBox>
</fieldset>

<fieldset>
<legend><asp:RadioButton ID="RB_Exp" Text="按照表达式阻塞" runat="server"   GroupName="xxx" /></legend>
 
<a href="javascript:ShowHidden('exp')">请输入表达式:</a>
<div id="exp" style="color:Gray; display:none">
 <ul>
 <li>配置一个SQL，该SQL返回一行一列的数值类型的值。</li>
 <li>如果该值大于0，就是true, 否则就是false.</li>
 <li>配置的参数支持ccbpm表达式。</li>
 </ul>
 </div>
<asp:TextBox ID="TB_Exp" runat="server"   Width="95%" ></asp:TextBox>
</fieldset>
 
 
<fieldset>
<legend>其他选项设置</legend>

<font color="gray">被阻塞时提示信息(默认为:符合发送阻塞规则，并能向下发送):</font>
<asp:TextBox ID="TB_Alert" runat="server"   Width="95%" ></asp:TextBox>
</fieldset>
 
</td>

<td valign="top" style="width:30%;"> 
<fieldset>
<legend>帮助</legend>
<ul>
<li>发送阻塞，就是让当前节点不能向下运动的规则。</li>
<li>如果满足一定的条件，就不能让其向下运动。</li>
</ul>
</fieldset>
</td>



</tr>
<tr>
<td colspan=2>

<asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />

</td>
</tr>
</table>
</asp:Content>
