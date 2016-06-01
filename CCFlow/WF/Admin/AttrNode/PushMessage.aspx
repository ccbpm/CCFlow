<%@ Page Title="消息推送" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="PushMessage.aspx.cs" Inherits="CCFlow.WF.Admin.AttrNode.PushMessage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript">
    //删除.
    function Del(mypk, nodeid) {
        if (mypk == '') {
            alert("默认发送不允许删除，您可以修改。");
            return;
        }
        if (window.confirm('您确定要删除吗?') == false)
            return;
        window.location.href = 'PushMessage.aspx?MyPK='+mypk+'&DoType=Del&FK_Node='+nodeid;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style="width:100%">
<caption>
 <div style=" float:left"> 消息事件</div>   <div style="float:right"> <a href="http://ccbpm.mydoc.io"  target="_blank" >帮助</a> </div> </caption>

<tr>
<th>序</th>
<th>消息发生事件</th>
<th>启用邮件</th>
<th>邮件标题</th>
<th>启用短信</th>
<th>操作</th>
</tr>

<%
    string dotype = this.Request.QueryString["DoType"];
    string mypk = this.Request.QueryString["MyPK"];
    if (dotype == "Del")
    {
        BP.WF.Template.PushMsg enDel = new BP.WF.Template.PushMsg();
        enDel.MyPK = mypk;
        enDel.Delete();
    }
    
    int  nodeID= int.Parse( this.Request.QueryString["FK_Node"]);
    BP.WF.Template.PushMsgs ens = new BP.WF.Template.PushMsgs(nodeID);

    if (ens.Count == 0)
    {
        BP.WF.Template.PushMsg myen = new BP.WF.Template.PushMsg();
        myen.FK_Event = BP.Sys.EventListOfNode.SendSuccess;
        myen.FK_Node = nodeID;
        myen.MailPushWay = 1;
       // ens.AddEntity(myen);
    }
    

    int idx = 0;
    foreach (BP.WF.Template.PushMsg en in ens)
    {
        idx++;
        %>
        <tr>
         <td class="Idx" ><%=idx %> </td>
         <td><a href='PushMessageEntity.aspx?MyPK=<%=en.MyPK %>&FK_Node=<%=en.FK_Node %>&FK_Event=<%=en.FK_Event %>' > <%=en.FK_Event %></a></td>
         <td><%=en.MailPushWayText %></td>

         <td><%=en.MailTitle_Real%></td>
         <td><%=en.SMSPushWayText %></td>
         <td>  <a href="javascript:Del('<%=en.MyPK %>','<%=nodeID %>')" >删除</a> </td>
         </tr>
        <%
    }
%>
</table>


 <fieldset style=" border:0px;">
         <%--   <legend><a href="javascript:ShowHidden('state')">新建消息</a></legend>--%>
              <input type="button" value="新建消息" id="Btn_Save" onclick="ShowHidden('state')" />
            <div id="state" style="display:none;color:Gray">
            <ul>
            <%
                BP.WF.XML.EventLists xmls = new BP.WF.XML.EventLists();
                xmls.RetrieveAll();
                foreach (BP.WF.XML.EventList item in xmls)
                {
                    if (item.IsHaveMsg == false)
                        continue;
                    
                    %>
                <li><a href="PushMessageEntity.aspx?FK_Node=<%=nodeID %>&FK_Event=<%=item.No %>" > <%=item.Name %></a> </li>
              <%
                }
               %>
          
            </ul>
           <%-- 页面美化--%>


            </div>
 </fieldset>


</asp:Content>
