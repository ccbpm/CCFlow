<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.WFRpt" CodeBehind="WFRpt.ascx.cs" %>
<%@ Register Src="UCEn.ascx" TagName="UCEn" TagPrefix="uc1" %>
<%@ Register Src="../Comm/UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc3" %>
<link id="myFlowcss" href="" rel="stylesheet" type="text/css" />
<script type="text/javascript" language="javascript" src="/DataUser/PrintTools/LodopFuncs.js"></script>
<script type="text/javascript">
    $(function () {
        var userStyle = ""; // "<%=BP.WF.Glo.GetUserStyle %>";
        $('#myFlowcss').attr('href', '/DataUser/Style/MyFlow.css');
        var screenHeight = window.screen.height; //document.documentElement.clientHeight;
        var topBarHeight = 40;
        var allHeight = topBarHeight;
        if ("<%=BtnWord %>" == "2")
            allHeight = allHeight + 30;
        var frmHeight = "<%=Height %>";
        if (screenHeight > parseFloat(frmHeight) + allHeight) {
            $("#divCCForm").height(screenHeight - allHeight);
        }
    });
</script>
<script language="javascript" type="text/javascript">
    var LODOP; //声明为全局变量 
    function printFrom() {
        var url = "PrintSample.aspx?FK_Flow=<%=this.FK_Flow%>&FK_Node=<%=this.FK_Node %>&FID=<%=this.FID %>&WorkID=<%=this.WorkID %>&UserNo=<%=BP.Web.WebUser.No %>&AtPara=";
        LODOP = getLodop(document.getElementById('LODOP_OB'), document.getElementById('LODOP_EM'));
        LODOP.PRINT_INIT("打印表单");
        // LODOP.ADD_PRINT_URL(30, 20, 746, "100%", location.href);
//        LODOP.ADD_PRINT_HTM(0, 0, "100%", "100%", document.getElementById("divCCForm").innerHTML);
         LODOP.ADD_PRINT_URL(0, 0, "100%", "100%", url);
        LODOP.SET_PRINT_STYLEA(0, "HOrient", 3);
        LODOP.SET_PRINT_STYLEA(0, "VOrient", 3);
        //		LODOP.SET_SHOW_MODE("MESSAGE_GETING_URL",""); //该语句隐藏进度条或修改提示信息
        //		LODOP.SET_SHOW_MODE("MESSAGE_PARSING_URL","");//该语句隐藏进度条或修改提示信息
        //  LODOP.PREVIEW();
        LODOP.PREVIEW();
        return false;
    }
</script>
<div style="width: 0px; height: 0px">
    <object id="LODOP_OB" classid="clsid:2105C259-1E0C-4534-8141-A753534CB4CA" width="0"
        height="0">
        <embed id="LODOP_EM" type="application/x-print-lodop" width="0" height="0" pluginspage="/DataUser/PrintTools/install_lodop32.exe"></embed>
    </object>
</div>
<%--<div style="width: <%=Width %>px; margin: 0 auto; background: white; text-align: left;">
    <asp:Button ID="Btn_Print" runat="server" Text="打印" CssClass="Btn" Visible="true"
        OnClientClick="return printFrom()" />
</div>--%>
 <div style="width: <%=Width %>px;" class="topBar" id="topBar">
        <uc3:ToolBar ID="ToolBar1" runat="server" />
    </div>
<div style="width: <%=Width %>px; margin: 0 auto; background: white; border-top: 1px solid #4D77A7;">
    <br />
</div>
<uc1:UCEn ID="UCEn1" runat="server" />
<script type="text/javascript">
    function ReinitIframe(frmID, tdID) {
        try {
            var iframe = document.getElementById(frmID);
            var tdF = document.getElementById(tdID);
            iframe.height = iframe.contentWindow.document.body.scrollHeight;
            iframe.width = iframe.contentWindow.document.body.scrollWidth;
            if (tdF.width < iframe.width) {
                tdF.width = iframe.width;
            } else {
                iframe.width = tdF.width;
            }
            tdF.height = iframe.height;
            return;
        } catch (ex) {
            return;
        }
        return;
    }
</script>
<style type="text/css">
    .ActionType
    {
        width: 16px;
        height: 16px;
    }
</style>
