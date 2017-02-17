<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="PushMessageEntity.aspx.cs" Inherits="CCFlow.WF.Admin.AttrNode.PushMessageEntity" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%
    string eventType = this.Request.QueryString["FK_Event"];
    string mypk = this.Request.QueryString["MyPK"];
    int fk_node =  int.Parse( this.Request.QueryString["FK_Node"]);

    BP.WF.Node nd = new BP.WF.Node(fk_node);

    BP.WF.Template.PushMsg en = new BP.WF.Template.PushMsg();
    en.MyPK = mypk;
    en.FK_Event = eventType;
    en.RetrieveFromDBSources();
%>

<table style="width:100%">
<caption> <div style=" float:left"><%=eventType %>   -  消息实体</div> 
  <div style="float:right"> <a href="http://ccbpm.mydoc.io"  target="_blank" >帮助</a> </div> </caption>
<tr>
<td>

<fieldset> 
<legend>启用短信设置</legend>
   <table style="width:100%;" >
   <tr>
   <td colspan="2"><asp:RadioButton ID="RB_SMS_0" runat="server"  Text="不发送"  GroupName="xx"/> </td>
   </tr>
    
    <tr>
     <td colspan="2"><asp:RadioButton ID="RB_SMS_1" runat="server"  Text="本节点的接受人"  GroupName="xx"/> </td>
   </tr>

    <tr>
    <td style="width:200px;" ><asp:RadioButton ID="RB_SMS_2" runat="server" Text="表单上的字段作为接受对象" GroupName="xx"/> </td>
   <td> <asp:DropDownList ID="DDL_SMS_Fields" runat="server">
       </asp:DropDownList> (手机号/微信号/丁丁号/CCIM人员ID) </td>
   </tr>

   <tr>
    <td><asp:RadioButton ID="RB_SMS_3" runat="server"  Text="其他节点的处理人"  GroupName="xx"/> </td>
    <td>
        <uc1:Pub ID="Pub1" runat="server" />
       </td>
   </tr>

   <tr>
   <td colspan=2> <fieldset style=" border:0px;">
            <legend><a href="javascript:ShowHidden('sms')">短信发送内容模版:</a></legend>
            <div id="sms" style="display:none;color:Gray">
            <ul>
             <li>cc会根据不同的事件设置不同的信息模版。</li>
             <li>这些模版都是标准的提示，如果您要需要个性化的提示请修改该模版。</li>
             <li>该参数支持cc表达式。</li>
             <li>您可以使用@符号来编写您所需要的内容。</li>
             <li>对于信息提示有两个系统参数分别是{Title}流程标题， {URL} 打开流程的连接。</li>
             <li>cc在生成消息的时候会根据模版把信息替换下来，发送给用户。</li>
             <li>最新格式的cc字段表达式为: <font color=red> @+字段名+分号</font> 比如 @QingJiaLeiXing; </li>
            </ul>
            </div>
 </fieldset>
 
<asp:TextBox ID="TB_SMS" TextMode="MultiLine" runat="server"  Rows="3" Width="95%"></asp:TextBox>
 </td>
   </tr>
   </table>

</fieldset>
 
<!--------   邮件提醒.   -------------------------->
<fieldset> 
<legend>启用邮件提醒</legend>
<table style="width:100%;" >
<tr>
<td colspan="2" ><asp:RadioButton ID="RB_Email_0" runat="server"  Text="不发送"  GroupName="email"/>   </td>
</tr>
<tr>
<td colspan="2"> <asp:RadioButton ID="RB_Email_1" runat="server"  Text="本节点的接受人"  GroupName="email"/>  </td>
</tr>
<tr>
<td><asp:RadioButton ID="RB_Email_2" runat="server"  Text="表单上的字段作为邮件" GroupName="email"/>   </td>
<td>选择一个字段:<asp:DropDownList ID="DDL_Email" runat="server">
       </asp:DropDownList>  
       </td>
</tr>
   <tr>
    <td><asp:RadioButton ID="RB_Email_3" runat="server"  Text="其他节点的处理人"  GroupName="email"/> </td>
    <td>
        <uc1:Pub ID="Pub2" runat="server" />
       </td>
   </tr>
<tr>
  <td style="width:200px;" > 
            <fieldset style="border:0px;">
            <legend><a href="javascript:ShowHidden('titeemail')">邮件标题模版:</a></legend>
            <div id="titeemail" style="display:none;color:Gray">
            <ul>
             <li>该参数支持cc表达式。</li>
             <li>您可以使用@符号来编写您所需要的内容。</li>
             <li>最新格式的cc字段表达式为:<font color=red> @+字段名+分号</font>比如 @QingJiaLeiXing; </li>
            </ul>
            </div>
            </fieldset>
   </td>
   <td> <asp:TextBox ID="TB_Email_Title"  Width="500px" runat="server"></asp:TextBox> </td>
</tr>


<tr>
<td> 
<fieldset style=" border:0px;">
            <legend><a href="javascript:ShowHidden('st2ate')">邮件内容模版:</a></legend>
            <div id="st2ate" style="display:none;color:Gray" >
            <ul>
             <li>该参数支持cc表达式。</li>
             <li>您可以使用@符号来编写您所需要的内容。</li>
             <li>最新格式的cc字段表达式为:<font color=red> @+字段名+分号</font>比如 @QingJiaLeiXing; </li>
            </ul>
            </div>
 </fieldset>
  </td>

<td><asp:TextBox ID="TB_Email_Doc" TextMode="MultiLine" runat="server"  Rows="3" Width="500px"></asp:TextBox> </td>
</tr>
</table>

</fieldset>



 <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click"  />
 <asp:Button ID="Btn_Back" runat="server" Text="返回" onclick="Btn_Back_Click"   />

 
</asp:Content>
