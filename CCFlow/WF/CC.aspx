<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="CC.aspx.cs" Inherits="CCFlow.WF.CC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 
<table width='100%'  cellspacing='0' cellpadding='0' align=left>
<caption ><div class='CaptionMsg' >抄送</div></caption>
<tr>
<th  colspan=8 >
<% 
    string sta = this.Request.QueryString["Sta"];
    if (sta == null || sta=="")
        sta = "-1";

    int pageSize = 6;// int.Parse(pageSizeStr);

    string pageIdxStr = this.Request["PageIdx"];
    if (pageIdxStr == null)
        pageIdxStr = "1";
    int pageIdx = int.Parse(pageIdxStr);
    
    //实体查询.
    BP.WF.SMSs ss = new BP.WF.SMSs();
    BP.En.QueryObject qo = new BP.En.QueryObject(ss);
    
    System.Data.DataTable dt=null;
    if (sta == "-1")
        dt = BP.WF.Dev2Interface.DB_CCList(BP.Web.WebUser.No);
    if (sta == "0")
        dt = BP.WF.Dev2Interface.DB_CCList_UnRead(BP.Web.WebUser.No);
    if (sta == "1")
        dt = BP.WF.Dev2Interface.DB_CCList_Read(BP.Web.WebUser.No);
    if (sta == "2")
        dt = BP.WF.Dev2Interface.DB_CCList_Delete(BP.Web.WebUser.No);
    
    int allNum=qo.GetCount();
    qo.DoQuery(BP.WF.SMSAttr.MyPK, pageSize, pageIdx);

    //绑定分页
   // this.Pub1.BindPageIdx(allNum, pageSize, pageIdx, "CC.aspx?Sta=" + sta);

    BP.WF.XML.CCMenus tps = new BP.WF.XML.CCMenus();
    tps.RetrieveAll();
    string link = ""; // "<a href='?MsgType=All'>全部</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
    foreach (BP.WF.XML.CCMenu tp in tps)
    {
        link += "<a href='?Sta=" + tp.No + "'>" + tp.Name + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
    }
    
   %>
   <%= link%>
</th>
</tr>


<tr>
<th>序 </th>
<th>工作流程 </th>
<th>节点 </th>
<th>抄送人 </th>
<th>标题 </th>
<th>内容 </th>
<th>日期 </th>
<th>操作 </th>

</tr>

 
 <%
     int idx = 0;
     foreach (System.Data.DataRow dr in dt.Rows)
     {
         idx++;
         string rec= dr[BP.WF.Template.CCListAttr.Rec] as string ;
         
         string title = dr[BP.WF.Template.CCListAttr.Title] as string;
         string doc = dr[BP.WF.Template.CCListAttr.Doc] as string;
         string rdt = dr[BP.WF.Template.CCListAttr.RDT] as string;
         string mypk = dr[BP.WF.Template.CCListAttr.MyPK] as string;
         string flowNo = dr[BP.WF.Template.CCListAttr.FK_Flow] as string;
         string flowName = dr[BP.WF.Template.CCListAttr.FlowName] as string;
         
         string fk_node = dr[BP.WF.Template.CCListAttr.FK_Node].ToString(); 
         string nodeName = dr[BP.WF.Template.CCListAttr.NodeName].ToString(); 
         
         string workid = dr[BP.WF.Template.CCListAttr.WorkID].ToString(); // as string;
         string fid = dr[BP.WF.Template.CCListAttr.FID].ToString();
         
     %>
     <tr>

     <td class=Idx><%=idx %></td>
     <td  ><%=flowName %></td>
     <td  ><%=nodeName %></td>
     <td  > <%=rec %></td>

     <td  ><a href="WFRpt.aspx?FK_Flow=<%=flowNo %>&FK_Node=<%=fk_node %>&WorkID=<%=workid %>&FID=<%=fid %>" target=_blank > <%=title %> </a></td>

<%--     <td  ><a href="WFRpt.aspx?FK_Flow=<%=flowNo %>&FK_Node=<%=fk_node %>&WorkID=<%=workid %>&FID=<%=fid %>&DoType=CC&CCSta=<%=sta %>&CCID=<%=mypk %>" target=_blank > <%=title %> </a></td>
--%>
     <td  > <%=BP.DA.DataType.ParseText2Html(doc) %></td>
     <td  ><%=BP.DA.DataType.ParseSysDate2DateTimeFriendly(rdt) %></td>
     <td  ><a href="javascript:Replay('<%=mypk %>')">回复</a> </td>
     </tr>
         
  <% } %>
</table>
<input type=checkbox  value="选择"   />选择 <input type="button" value="删除" />

 
</asp:Content>
