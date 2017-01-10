<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FrmCheck.ascx.cs" Inherits="CCFlow.WF.App.Comm.FrmCheck" %>
<%@ Register src="FrmCheck.ascx" tagname="FrmCheck" tagprefix="uc1" %>
<%@ Register src="FTC.ascx" tagname="FTC" tagprefix="uc2" %>
<%@ Register src="AccepterCCerSelecter.ascx" tagname="AccepterCCerSelecter" tagprefix="uc3" %>
<%@ Register assembly="CCFlow" namespace="CCFlow.WF.WorkOpt" tagprefix="cc1" %>
<%@ Register assembly="BP.Web.Controls" namespace="BP.Web.Controls" tagprefix="cc2" %>
<%
    string enName = "ND" + this.Request.QueryString["FK_Node"];
    string src = "/WF/WorkOpt/WorkCheck.aspx?FID=" + this.Request.QueryString["FID"];
    src += "&WorkID=" + this.Request.QueryString["WorkID"];
    src += "&FK_Node=" + this.Request.QueryString["FK_Node"];
    src += "&FK_Flow=" + this.Request.QueryString["FK_Flow"];
    src += "&IsHidden=" + this.IsHidden;
    string srcTrack = "/WF/WorkOpt/TrackChart.aspx?FK_Flow="+this.Request.QueryString["FK_Flow"];
    srcTrack += "&FK_Node=" + this.Request.QueryString["FK_Node"];
    srcTrack += "&WorkID=" + this.Request.QueryString["WorkID"];
    BP.WF.Template.FrmWorkCheck wc = new BP.WF.Template.FrmWorkCheck(int.Parse(this.Request.QueryString["FK_Node"]));
    
    int nodeID = int.Parse(this.Request.QueryString["FK_Node"]);
    BP.WF.Template.FrmWorkCheck frmWorkCheck = new BP.WF.Template.FrmWorkCheck(nodeID);
    if (wc.HisFrmWorkCheckSta == BP.WF.Template.FrmWorkCheckSta.Disable == false)
    {
%>

<div id="tt" class="easyui-tabs"  style="width: <%=frmWorkCheck.FWC_Wstr%>; height:<%= frmWorkCheck.FWC_Hstr %>;">
    <div title="审核信息" id='CheckInfo'   >
        <iframe id='F' src='<%=src%>' frameborder="0" style=' padding: 0px; border: 0px; margin:0px; height:99%'
            leftmargin='0' topmargin='0' width='100%'   >
        </iframe>

    </div>
</div>
<uc1:FrmCheck ID="FrmCheck1" runat="server" />
<uc2:FTC ID="FTC1" runat="server" />
<uc3:AccepterCCerSelecter ID="AccepterCCerSelecter1" runat="server" />
<cc1:PostBackTextBox ID="PostBackTextBox1" runat="server"></cc1:PostBackTextBox>
<cc2:BPRadioButtonList ID="BPRadioButtonList1" runat="server">
</cc2:BPRadioButtonList>
<cc2:DatePicker ID="DatePicker1" runat="server" />
<cc2:LineChart ID="LineChart1" runat="server" />

<% } %>
