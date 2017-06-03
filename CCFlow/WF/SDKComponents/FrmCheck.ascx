<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FrmCheck.ascx.cs" Inherits="CCFlow.WF.App.Comm.FrmCheck" %>
<%@ Register assembly="BP.Web.Controls" namespace="BP.Web.Controls" tagprefix="cc2" %>

<%
    string enName = "ND" + this.Request.QueryString["FK_Node"];
    string src = "/WF/WorkOpt/WorkCheck.aspx?FID=" + this.Request.QueryString["FID"];
    src += "&WorkID=" + this.Request.QueryString["WorkID"];
    src += "&FK_Node=" + this.Request.QueryString["FK_Node"];
    src += "&FK_Flow=" + this.Request.QueryString["FK_Flow"];
    src += "&IsHidden=" + this.IsHidden;
    
    int nodeID = int.Parse(this.Request.QueryString["FK_Node"]);
    BP.WF.Template.FrmWorkCheck frmWorkCheck = new BP.WF.Template.FrmWorkCheck(nodeID);
    if (frmWorkCheck.HisFrmWorkCheckSta == BP.WF.Template.FrmWorkCheckSta.Disable == false)
    {
%>

<div id="tt" class="easyui-tabs"  style="width: <%=frmWorkCheck.FWC_Wstr%>; height:<%= frmWorkCheck.FWC_Hstr %>;">
    <div title="审核信息" id='CheckInfo'   >
        <iframe id='F' src='<%=src%>' frameborder="0" style=' padding: 0px; border: 0px; margin:0px; height:99%'
            leftmargin='0' topmargin='0' width='100%'   >
        </iframe>
    </div>
</div>
<% } %>
