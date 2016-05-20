<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true"
    CodeBehind="Messages.aspx.cs" Inherits="CCFlow.WF.Messages1" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="Scripts/CommonUnite.js" type="text/javascript"></script>
     <style type="text/css">
        table
        {
            border: none;
        }
        tr
        {
            border: none;
        }
        td
        {
            border: none;
        }
        .eachMsg
        {
            border-top: 1px #CDCDCD dashed;
        }
        .link
        {
            font-size: 15px;
            color: #69708D;
        }
        .sec
        {
            font-size: 15px;
            background-color: #037FCC;
            color: #F7EEAA;
        }
        .MsgDoc
        {
            padding:10px;
        }
    </style>

    <script  type="text/javascript">
        function Del(pk) {
            if (confirm('确定要删除吗?')) {
                window.location.href = '?OP=' + pk;
            }
        }
        function Rel(reTo, pk) {
            var url = 'MessagesReplay.aspx?RE=' + reTo + '&MyPK=' + pk;
            alert(url);
            WinOpen(url);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width='100%' cellspacing='0' cellpadding='0' align="left">
        <caption> <div class='CaptionMsg'>消息</div>  </caption>
        <%-- 加载类型--%>
        <script type="text/javascript">
            $(function () {
                document.getElementById("disThis").style.backgroundColor = "#7699CB";
                document.getElementById("disThis").style.color = "#F7EEAA";
                $('#win').window('close');
            });
            function replay(sendTo, id) {
                //                alert(sendTo + "---" + id);
                window.open('MessagesReplay.aspx?RE=' + sendTo + '&MyPK=' + id + '&', '', 'height=400, width=600, top=0, left=0, toolbar=no, menubar=no, scrollbars=yes, resizable=no, location=no, status=no');
            }
        </script>
        <%
            BP.WF.XML.MsgTypes tps = new BP.WF.XML.MsgTypes();
            tps.RetrieveAll();
            string link = "";
            foreach (BP.WF.XML.MsgType tp in tps)
            {
                string str = "";
                if (this.Request.QueryString["MsgType"] == tp.No || string.IsNullOrEmpty(this.Request.QueryString["MsgType"]))
                    str = " id='disThis' ";

                link += "<a class='link' " + str + " href='?MsgType=" + tp.No + "'>" + tp.Name + "</a>&nbsp;&nbsp;";
            }
       
        %>
        <tr>
            <th colspan="3" style="padding-left: 10%;">
               <div  style=" float:left"><%=link %> </div>

                <div style=" float:right">
                <a class="sec" href="javascript:if (confirm('确定要删除全部吗?')) { window.location.href = '?OP=delAll';}" >
                    清空所有</a>&nbsp;&nbsp;&nbsp;<a class="sec" target="_self" href="MessagesReplay.aspx">新建消息</a>
                </div>
            </th>
        </tr>
     
                <%
                    BP.WF.SMSs sms = null;
                    //执行删除
                    string op = this.Request.QueryString["OP"];
                    if (!string.IsNullOrEmpty(op))
                    {
                        if (op == "delAll")
                        {
                            sms = new BP.WF.SMSs();
                            sms.Delete(BP.WF.SMSAttr.SendTo, BP.Web.WebUser.No);
                        }
                        else
                        {
                            sms = new BP.WF.SMSs();
                            sms.Delete(BP.WF.SMSAttr.MyPK, op);
                        }
                    }

                    string msgType = this.Request.QueryString["MsgType"];
                    if (msgType == null)
                        msgType = "All";

                    int pageSize = 6;

                    string pageIdxStr = this.Request["PageIdx"];
                    if (pageIdxStr == null)
                        pageIdxStr = "1";
                    int pageIdx = int.Parse(pageIdxStr);

                    //实体查询.
                    sms = new BP.WF.SMSs();
                    BP.En.QueryObject qo = new BP.En.QueryObject(sms);

                    if (msgType == "All")
                    {
                        qo.AddWhere(BP.WF.SMSAttr.SendTo, BP.Web.WebUser.No);
                    }
                    else
                    {
                        qo.AddWhere(BP.WF.SMSAttr.SendTo, BP.Web.WebUser.No);
                        qo.addAnd();
                        qo.AddWhere(BP.WF.SMSAttr.MsgFlag, msgType); // 设置查询条件.
                    }
                    int allNum = qo.GetCount();
                    qo.DoQuery(BP.WF.SMSAttr.MyPK, pageSize, pageIdx);
                    int idx = 0;
                    foreach (BP.WF.SMS msg in sms)//循环输出信息 单个table
                    {
                        idx++;
                        %>
                        <tr class=" GroupField">
                        <td><img src='./Img/Msg.png' /> 第<%=idx %>条； <%=msg.Title %> </td>
                        <td>来自:<%= BP.WF.Glo.GenerUserImgSmallerHtml(msg.Sender, msg.Sender) %> </td>
                        <td> <%= BP.DA.DataType.ParseSysDate2DateTimeFriendly( msg.RDT) %> | <a href="javascript:Rel('<%=msg.Sender %>','<%=msg.MyPK %>')"> <img src='./Img/Btn/Reply.gif' /> 回复</a>  <a href="javascript:Del('<%=msg.MyPK %>')"><img src='./Img/Btn/Delete.gif' />删除</a></td>
                        </tr>

                        <tr>
                           <td colspan="3" valign="top" class="MsgDoc"  >
                                                    <%=msg.Doc%>
   
                                    </td>
                        </tr>
                <%  
                    }
                %>
        
    </table>

                <%
                    //绑定分页
                    this.Pub1.BindPageIdx(allNum, pageSize, pageIdx, "Messages.aspx?MsgType=" + msgType); %>
                <uc1:Pub ID="Pub1" runat="server" style="margin-left: auto; margin-right: auto;" />

   
</asp:Content>
