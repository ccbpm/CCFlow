<%@ Page Title="超时处理规则" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="CHOvertimeRole.aspx.cs" Inherits="CCFlow.WF.Admin.AttrNode.CHOvertimeRole" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style=" width:100%;">
<caption>超时处理规则</caption>

<tr>

<td>
<%
    int nodeID = int.Parse(this.Request.QueryString["FK_Node"]);
    BP.WF.Node nd = new BP.WF.Node(nodeID);
  %>
<fieldset>
<legend><asp:RadioButton ID="RB_0" Text="不处理"  Checked="true" runat="server" GroupName="xxxx" />
    </legend>
<ul style=" color:Gray">
<li>超时的时候一直处理超时的状态。</li>
</ul>
</fieldset>


<fieldset>
<legend><asp:RadioButton ID="RB_1" Text="自动向下运动" runat="server"  GroupName="xxxx"/>
    </legend>
<ul style=" color:Gray">
<li>超时了当前节点自动运动到下一个环节，如果要控制特定的条件下不向下运动，就需要在当前节点的发送前事件里编写相关的业务逻辑。</li>
</ul>
</fieldset>


<fieldset>
<legend><asp:RadioButton ID="RB_2" Text="跳转到指定的节点" runat="server" GroupName="xxxx" />
    </legend>
    <ul style=" color:Gray">
    <li>
      要跳转到的节点:<asp:DropDownList ID="DDL_Node" runat="server">
    </asp:DropDownList>
    </li>
    </ul>
</fieldset>



<fieldset>
<legend><asp:RadioButton ID="RB_3" Text="移交给指定的人员" runat="server" GroupName="xxxx" />
    </legend>

     <a href="javascript:ShowHidden('shift');" > 请输入要移交的工作人员：</a>
 <div id="shift" style="display:none">
  <ul>
   <li>接受输入的必须是人员的工作帐号。</li>
   <li>如果有多个人元用半角的逗号分开，比如: zhangsan,lisi。</li>
   <li>超时后就自动的移交给指定的工作人员。</li>
  </ul>
  </div>
      
      <asp:TextBox ID="TB_3_Shift" runat="server" Width="95%"></asp:TextBox>

</fieldset>


<fieldset>
<legend><asp:RadioButton ID="RB_4" Text="向指定的人员发信息,如果设置为空就向当前人发信息." runat="server" GroupName="xxxx" />
    </legend>
     <a href="javascript:ShowHidden('emps');" > 请输入要发送的工作人员：</a>
 <div id="emps" style="display:none">
  <ul>
   <li>接受输入的必须是人员的工作帐号。</li>
   <li>如果有多个人元用半角的逗号分开，比如: zhangsan,lisi。</li>
   <li>超时后，系统就会向这些人员发送消息。</li>
  </ul>
  </div>

<asp:TextBox ID="TB_4_SendMsg" runat="server" Width="95%"></asp:TextBox>




</fieldset>

<fieldset>
<legend><asp:RadioButton ID="RB_5" Text="删除流程" runat="server" GroupName="xxxx" />
    </legend>
 <ul style=" color:Gray">
  <li>超时后就自动删除当前的流程。</li>
</ul>
</fieldset>


<fieldset>
<legend><asp:RadioButton ID="RB_6" Text="执行SQL" runat="server" GroupName="xxxx" />
   </legend>

     <a href="javascript:ShowHidden('sql');" > 请输入要执行的SQL：</a>
 <div id="sql" style="display:none">
  <ul>
   <li>当前的的sql支持ccbpm的表达式.</li>
   <li>执行相关的SQL，处理相关的业务逻辑。</li>
  </ul>
  </div>

    <asp:TextBox ID="TB_6_SQL" runat="server" Width="95%"></asp:TextBox>
</fieldset>


<fieldset>
<legend>其他选项 </legend>
    <asp:CheckBox ID="CB_IsEval" runat="server" Text="是否质量考核点" />
</fieldset>



    <asp:Button ID="Btn_Save" runat="server" Text=" 保 存 " 
        onclick="Btn_Save_Click" />

</td>


<td valign="top" >
 
</td>


</tr>
</table>
</asp:Content>
