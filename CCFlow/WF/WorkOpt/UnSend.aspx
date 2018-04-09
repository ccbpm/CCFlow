<%@ Page Title="撤销发送" Language="C#" MasterPageFile="~/WF/SDKComponents/Site.Master" AutoEventWireup="true" CodeBehind="UnSend.aspx.cs" Inherits="CCFlow.WF.WorkOpt.UnSend" %>
<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />

 <table style=" text-align:left; width:100%">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %></caption>
<tr>
<td valign=top style="text-align:center">

<%
    // 参数.
    Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);
    string flowNo = this.Request.QueryString["FK_Flow"];
    
    string doIt = this.Request.QueryString["DoIt"];
    BP.WF.GenerWorkFlow gwf = new BP.WF.GenerWorkFlow(workid);
    BP.WF.Node nd = new BP.WF.Node(gwf.FK_Node);
    if (DataType.IsNullOrEmpty(doIt) == true)
    {
        %>
         <fieldset>
         <legend>节点撤销信息</legend>

         <ul>
         <li>流程标题：<%=gwf.Title %>   </li>
         <li>停留节点：<%=gwf.NodeName %></li>
         <li>工作人员：<%=gwf.TodoEmps %></li>
         </ul>
         <br>

         <center>
         
         <% if (nd.HisRunModel == BP.WF.RunModel.SubThread)
            {
                BP.WF.Nodes fromNDS = nd.FromNodes;
                if (fromNDS.Count != 1)
                {
                    
                }
          %>

         [<a href='UnSend.aspx?DoIt=DelSubThread&FK_Flow=<%=flowNo %>&WorkID=<%=workid %>' >删除子线程</a>] 
         - [<a href='/WF/WorkOpt/Forward.htm?DoIt=Shift&FK_Node=<%=nd.NodeID %>&FK_Flow=<%=flowNo %>&WorkID=<%=workid %>' >移交</a>] 
         - [<a href='UnSend.aspx?FK_Flow=<%=flowNo %>&WorkID=<%=workid %>' >取消</a>]

         </center>
          <%

            }
            else
            { 
                /*其他的节点.*/
                %>

         <a href="UnSend.aspx?DoIt=Cancel&FK_Flow=<%=flowNo %>&WorkID=<%=workid %>" > <img style="vertical-align:middle;" alt=""
             src="../Scripts/easyUI/themes/icons/ok.png" />确定撤销</a>
             
             &nbsp;&nbsp;&nbsp;&nbsp;<a href="UnSend.aspx?FK_Flow=<%=flowNo %>&WorkID=<%=workid %>" ><img  style="vertical-align:middle;" alt=""
                 src="../Scripts/easyUI/themes/icons/redo.png" />取消撤销</a></center>

         <%} %>
          
         </fieldset>
        <%
    }
            if (doIt == "Cancel")
            {
                /* 执行撤销... */
                try
                {
                    string str1 = BP.WF.Dev2Interface.Flow_DoUnSend(flowNo, workid);
                    BP.WF.Glo.ToMsg(str1);
                    return;
                }
                catch (Exception ex)
                {
                    BP.WF.Glo.ToMsgErr(ex.Message);
                    return;
                }
            }

            if (doIt == "DelSubThread")
            {
                /*删除子线程.*/
                try
                {
                    BP.WF.Dev2Interface.Flow_DeleteSubThread(flowNo, workid, "dd");
                    BP.WF.Glo.ToMsg("删除成功...");
                    return;
                }
                catch (Exception ex)
                {
                    BP.WF.Glo.ToMsgErr(ex.Message);
                    return;
                }
            }
    
    /*
     *  songhonggang：来处理。
     *  要求：
     *  1，要有两个选择项 【确定撤销】【不撤销返回】
     *  2，如果是当前处理人员是分流节点的人员，就提示正确的信息。
     *  3，如果
     * 
     */
 %>
   
    
    </td>
</tr>
</table>

</asp:Content>
