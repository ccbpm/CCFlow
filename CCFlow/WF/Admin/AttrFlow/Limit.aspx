<%@ Page Title="" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="Limit.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.Limit" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
   
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table style=" width:100%"> 
<caption> 发起限制规则</caption>
<tr>
<td valign=top style="width:20%;" > 
<fieldset>
<legend> 填写帮助</legend>

<ul>
<li>发起限制，根据不同的应用场景来设置的发起频率的限制规则。</li>
<li>比如：纳税人注销流程只能一个纳税人注销一次，不允许多次注销，发起多次，就是违反也正常的业务逻辑。。</li>

</ul>
</fieldset>
 </td>

<td valign="top">
<fieldset>
<legend><asp:RadioButton ID="RB_None" Text="不限制（默认）"  GroupName="xzgz" runat="server" Checked="true"/></legend>
<ul  style="color:Gray">
<li>不限制发起次数，比如报销流程、请款流程。</li>
</ul>
</fieldset>


<fieldset>
<legend><asp:RadioButton ID="RB_ByTime" Text="按时间规则计算" GroupName="xzgz" runat="server" /></legend>

<table style=" width:100%" >
<tr>
<td style=" width:80px;">规则模式：</td>
<td> <asp:DropDownList ID="DDL_ByTime" runat="server">
                    <asp:ListItem Value="0">每人每天一次</asp:ListItem>
                    <asp:ListItem Value="1">每人每周一次</asp:ListItem>
                    <asp:ListItem Value="2">每人每月一次</asp:ListItem>
                    <asp:ListItem Value="3">每人每季一次</asp:ListItem>
                    <asp:ListItem Value="4">每人每年一次</asp:ListItem>     
                </asp:DropDownList> </td>

                <td class="style2" >
                <font  color="gray">
                <ul>
                <li> 按照选择的时间频率进行设置发起限制</li>
                <li> <a href="javascript:WinOpen('http://bbs.ccflow.org/showtopic-3711.aspx')">更多规则模式与参数设置</a>  </li>
                </ul>
                </font>
                </td>
</tr>
<tr>
<td colspan=3>
 <a href="javascript:ShowHidden('FQGZ')">发起时间段限制参数设置:</a>
                <div id="FQGZ" style=" display:none; color:Gray"> 
                <ul>
                <li>该设置，可以为空。</li> 
                <li>用来限制该流程可以在什么时间段内发起。</li> 
                <li>例如:按照每人每天一次设置时间范围，规则参数：@08:30-09:00@18:00-18:30，解释：该流程只能在08:30-09:00与18:00-18:30两个时间段发起且只能发起一次。</li> 
                </ul>
                </div>
 <asp:TextBox ID="TB_ByTimePara" runat="server" Width="95%" Height="20px"></asp:TextBox> 
       </td>
</tr>
</table>
</fieldset>

<fieldset>
<legend> <asp:RadioButton ID="RB_OnlyOneSubFlow" Text="为子流程时，仅仅只能被调用1次." GroupName="xzgz" runat="server" /> </legend>

<ul>
<li>如果当前为子流程，仅仅只能被调用1次。</li>
</ul>
</fieldset>

<fieldset>
<legend><asp:RadioButton ID="RB_ColNotExit" Text="按照发起字段不能重复规则" GroupName="xzgz" runat="server" /></legend>
 
 <a href="javascript:ShowHidden('fields')">填写设置字段</a>
                <div id="fields" style=" display:none; color:Gray"> 
                <ul>
                <li>设置一个列允许重复，比如：NSRBH</li> 
                <li>设置多个列的时候，需要用逗号分开，比如：field1,field2</li> 
                <li>流程在发起的时候如果发现，该列是重复的，就抛出异常，阻止流程发起。</li> 
                <li>比如：纳税人注销流程，一个纳税人只能发起一次注销，就要配置纳税人字段，让其不能重复。</li> 
                </ul>
                </div>

 <asp:TextBox ID="TB_ColNotExit_Fields" runat="server" Width="95%" Height="20px"></asp:TextBox>
</fieldset>

<fieldset>
<legend><asp:RadioButton ID="RB_SQL" Text="按SQL规则" runat="server" GroupName="xzgz" /></legend>
&nbsp;&nbsp;规则模式：
  <asp:DropDownList ID="DDL_SQL" runat="server">
                    <asp:ListItem Value="0">设置的SQL数据为空，或者返回结果为零时可以启动</asp:ListItem>
                    <asp:ListItem Value="1">设置的SQL数据为空，或者返回结果为零时不可以启动</asp:ListItem>
                </asp:DropDownList> 
<br />
 <a href="javascript:ShowHidden('Div2')">SQL规则参数:</a>
                <div id="Div2" style=" display:none; color:Gray"> 
                <ul>
                    <li>例如：SELECT COUNT(*) AS Num FROM TABLE1 WHERE NAME='@MyFieldName'</li>
                    <li>解释：编写一个sql语句返回一行一列，如果信息是0，就是可以启动，非0就不可以启动。</li>
                    <li>该参数支持ccbpm的表达式。</li>
                 </ul>
                 </div>
    <asp:TextBox ID="TB_SQL_Para" runat="server"  Rows="2" TextMode="MultiLine" Width="95%" ></asp:TextBox>
</fieldset>


<fieldset>
<legend> <a href="javascript:ShowHidden('msgAlert')">限制提示信息:</a></legend>

                <div id="msgAlert" style=" display:none; color:Gray"> 
                <ul>
                    <li>例如:您的发起的流程违反了xxx限制规则，不能发起该流程。</li>
                    <li>当限制规则起作用的时候，应该提示给用户什么信息。</li>
                    <li>该信息在创建工作失败的时候提示。</li>
                 </ul>
                 </div>
    <asp:TextBox ID="TB_Alert"  Width="95%" runat="server"></asp:TextBox>
</fieldset>


</td>
</tr>


<tr>
<td> </td>
<td> <asp:Button class="easyui-linkbutton" ID="Btn_Save" runat="server" Text="保存" OnClick="Btn_Save_Click" /> </td>
</tr>

</table>


    

</asp:Content>
