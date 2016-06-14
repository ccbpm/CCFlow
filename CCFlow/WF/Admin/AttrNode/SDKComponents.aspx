<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="SDKComponents.aspx.cs" Inherits="CCFlow.WF.Admin.AttrNode.SDKComponents" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style="width:100%;height:100%;">
<caption> SDK 表单组件属性</caption>
<tr>
<td style=" width:20%;"  valign=top nowarp=true >

<% 
    string ctrlType = this.Request.QueryString["CtrlType"];
    string nodeID=this.Request.QueryString["FK_Node"];
    int fk_node = int.Parse(nodeID);

    string src = "../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmWorkCheck&PK=" + nodeID;
    if (ctrlType == "DocMultiAth")
        src = "";

    BP.WF.Node nd = new BP.WF.Node(fk_node);
     %>
     <ul>
        <li> <a href="../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmWorkCheck&PK=<%=nodeID %>" target="Doc" >审核组件</a> </li>
        <li> <a href="../../MapDef/Attachment.aspx?FK_MapData=ND<%=nodeID %>&Ath=DocMultiAth&FK_Node=<%=nodeID %>" target="Doc">多附件</a> </li>
        <li> <a href="../../MapDef/Attachment.aspx?FK_MapData=ND<%=nodeID %>&Ath=DocMainAth&FK_Node=<%=nodeID %>" target="Doc">单附件</a> </li>
        <li> <a href="SubFlows.aspx?FK_Node=<%=nodeID %>" target="Doc">父子流程</a> </li>

        <li> <a href="../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.BtnLab&PK=<%=nodeID %>" target="Doc">工具栏</a> </li>
        
        <li> <a href="../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmWorkCheck&PK=<%=nodeID %>" target="Doc">轨迹</a> </li>

        <%--<li> <a href="../../Comm/RefFunc/UIEn.aspx?EnName=FrmWorkCheck&PK=<%=nodeID %>" target="Doc">接受人</a> </li>--%>

        <% if (nd.HisNodeWorkType == BP.WF.NodeWorkType.WorkHL || nd.HisNodeWorkType == BP.WF.NodeWorkType.WorkFHL)
           { %>
          <li> <a href="../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmWorkCheck&PK=<%=nodeID %>" target="Doc">子线程</a> </li>
         <%} %>
     </ul>
 </td>




<td style="width:100%;height:500px" >
 <iframe src="<%=src %>" style="width:100%;height:100%;" name="Doc" > 
 </iframe> 
 </td>

</tr>
</table>
</asp:Content>
