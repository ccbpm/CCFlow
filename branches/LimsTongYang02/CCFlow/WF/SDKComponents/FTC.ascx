<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FTC.ascx.cs" Inherits="CCFlow.WF.SDKComponents.FTC" %>

<%
    string enName = "ND" + this.Request.QueryString["FK_Node"];
    string src = "/WF/WorkOpt/WorkCheck.htm?FID=" + this.Request.QueryString["FID"];
    src += "&WorkID=" + this.Request.QueryString["WorkID"];
    src += "&FK_Node=" + this.Request.QueryString["FK_Node"];
    src += "&FK_Flow=" + this.Request.QueryString["FK_Flow"];
    
    string srcTrack = "/WF/WorkOpt/FTC.aspx?FK_Flow="+this.Request.QueryString["FK_Flow"];
    srcTrack += "&FK_Node=" + this.Request.QueryString["FK_Node"];
    srcTrack += "&WorkID=" + this.Request.QueryString["WorkID"];
    
    int nodeID = int.Parse(this.Request.QueryString["FK_Node"]);
    BP.WF.Template.FrmTransferCustom frmTC = new BP.WF.Template.FrmTransferCustom(nodeID);
    if (frmTC.FTCSta != BP.WF.Template.FTCSta.Disable)
    {
%>

<div id="tt" class="easyui-tabs"  style="width: <%=frmTC.FTC_Wstr%>; height:<%= frmTC.FTC_Hstr %>;">
    <div title="设计接受人" id='FTC'   >
        <iframe id='F' src='<%=src%>' frameborder="0" style=' padding: 0px; border: 0px; margin:0px; height:99%'
            leftmargin='0' topmargin='0' width='100%'   >
        </iframe>
    </div>
</div>
<% } %>