<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="PushMessageEntity.aspx.cs" Inherits="CCFlow.WF.Admin.AttrNode.PushMessageEntity" %>
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
<table style="width:70%;" >
<tr>
<td> 发送给谁？</td>
   <td>
    <asp:RadioButton ID="RB_SMS_0" runat="server"  Text="不发送"  GroupName="xx"/>
    <br />
    <asp:RadioButton ID="RB_SMS_1" runat="server"  Text="本节点的接受人"  GroupName="xx"/>
    <br />
    <asp:RadioButton ID="RB_SMS_2" runat="server"  Text="表单上的字段作为接受对象(手机号/微信号/丁丁号/CCIM人员ID)" GroupName="xx"/>
       <asp:DropDownList ID="DDL_SMS_Fields" runat="server">
       </asp:DropDownList>

 <fieldset style=" border:0px;">
            <legend><a href="javascript:ShowHidden('sms')">短信发送内容模版:</a></legend>
            <div id="sms" style="display:none;color:Gray">
            <ul>
             <li>cc会根据不同的事件设置不同的信息模版。</li>
             <li>这些模版都是标准的提示，如果您要需要个性化的提示请修改该模版。</li>
             <li>该参数支持cc表达式。</li>
             <li>您可以使用@符号来编写您所需要的内容。</li>
             <li>对于信息提示有两个系统参数分别是{Title}流程标题， {URL} 打开流程的连接。</li>
             <li>cc在生成消息的时候会根据模版把信息替换下来，发送给用户。</li>
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
<table style="width:70%;" >
 


<tr>
<td> 发送给谁？</td>
   <td>
   <asp:RadioButton ID="RB_Email_0" runat="server"  Text="不发送"  GroupName="email"/>
    <br />
    <asp:RadioButton ID="RB_Email_1" runat="server"  Text="本节点的接受人"  GroupName="email"/>
    <br />
    <asp:RadioButton ID="RB_Email_2" runat="server"  Text="表单上的字段作为邮件" GroupName="email"/>
       <asp:DropDownList ID="DDL_Email" runat="server">
       </asp:DropDownList>
   </td>
</tr>

<tr>
<td colspan="2"  valign=top>

 <fieldset style=" border:0px;">
            <legend><a href="javascript:ShowHidden('titeemail')">邮件标题模版:</a></legend>
            <div id="titeemail" style="display:none;color:Gray">
            <ul>
             <li>该参数支持cc表达式。</li>
             <li>您可以使用@符号来编写您所需要的内容。</li>
            </ul>
            </div>
 </fieldset>

 <asp:TextBox ID="TB_Email_Title"  Width="95%" runat="server"></asp:TextBox>
 <fieldset style=" border:0px;">
            <legend><a href="javascript:ShowHidden('st2ate')">邮件内容模版:</a></legend>
            <div id="st2ate" style="display:none;color:Gray">
            <ul>
             <li>该参数支持cc表达式。</li>
             <li>您可以使用@符号来编写您所需要的内容。</li>
            </ul>
            </div>
 </fieldset>
&nbsp;<asp:TextBox ID="TB_Email_Doc" TextMode="MultiLine" runat="server"  Rows="3" Width="95%"></asp:TextBox>


    </td>

 

</tr>
</table>

</fieldset>
</td>


  <%-- <td valign=top style=" width:25%;">
    <fieldset>
    <legend>帮助</legend>
     <ul>
             <li>cc会根据不同的事件设置不同的信息模版。</li>
             <li>这些模版都是标准的提示，如果您要需要个性化的提示请修改该模版。</li>
             <li>该参数支持cc表达式。</li>
             <li>您可以使用@符号来编写您所需要的内容。</li>
             <li>对于信息提示有两个系统参数分别是{Title}流程标题， {URL} 打开流程的连接。</li>
             <li>cc在生成消息的时候会根据模版把信息替换下来，发送给用户。</li>
            </ul>
    </fieldset>
    </td>--%>
</tr>

</table>

 <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click"  />
 <input type="button" value="返回" onclick=" windows.localhost.href='PushMessage.aspx?FK_Node=<%=fk_node %>' " />

</asp:Content>
