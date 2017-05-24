<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.MyFlow" CodeBehind="MyFlow.ascx.cs" %>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<%@ Register Src="UCEn.ascx" TagName="UCEn" TagPrefix="uc2" %>
<%@ Register Src="../Comm/UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc3" %>

<div style="width: 0px; height: 0px">
    <object id="LODOP_OB" classid="clsid:2105C259-1E0C-4534-8141-A753534CB4CA" width="0"
        height="0">
        <embed id="LODOP_EM" type="application/x-print-lodop" width="0" height="0" pluginspage="/DataUser/PrintTools/install_lodop32.exe" />
    </object>
</div>

<script type="text/javascript">
    var winSelectAccepter;
    //发送按钮，当
    function SendBtnCondClick(flowNo, nodeid, workID, fid) {

        if (SysCheckFrm() == false)
            return false;

        //执行同步.
        KindEditerSync();

        var dll = document.getElementById("ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_DDL_ToNode");
        if (dll) {
            var selectNodeID = dll.options[dll.selectedIndex].value;

            if (selectNodeID == "0") {
                alert('请选择分支.');
                return false;
            }

            if (selectNodeID.indexOf('.') != -1) {  //发送之前，需要调用选择人接收器.
                selectNodeID = selectNodeID.replace('.1', '');
                var url = "./WorkOpt/Accepter.htm?WorkID=" + workID + "&ToNode=" + selectNodeID + "&FK_Node=" + nodeid + "&FK_Flow=" + flowNo + "&FID=" + fid + "&type=2&DoType=AccepterSave";
                if (winSelectAccepter != null && winSelectAccepter.close != undefined) {
                    winSelectAccepter.close();
                }
                winSelectAccepter = window.open(url, "winSelectAccepter", 'height=600, width=600,scrollbars=yes');


            } else {
                //开始调用发送按钮.
                var btnSend = document.getElementById("ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_Btn_Send");
                btnSend.click();
            }
        } else {
            //开始调用发送按钮.
            var btnSend = document.getElementById("ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_Btn_Send");
            btnSend.click();
        }
    }


    //打开接收人窗口时获取返回值
    function returnWorkWindowClose(data) {
            //alert(data);
        if (data.indexOf('SaveOK@') == 0) {
            //说明保存人员成功,开始调用发送按钮.
            var btnSend = document.getElementById("ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_Btn_Send");
            btnSend.click();
        } else  {//可以重新打开接收人窗口
            winSelectAccepter = null;
        }

        // alert(data);
        //        data = data.replace('@','<br>@');
        //        $("#Msg").html(data);
        //        //window.location.href = 'MyFlowInfo.aspx?FID=';
        //        $(window.opener.document.getElementById('topBar')).find('input').attr('disabled');
        //        $('#topBar').find('input').attr('disabled', 'true');
        //        $('#topBar').find('input').css('background', 'gray');
    }

</script>

<div style="margin: 0; padding: 0;" id="D">
    <div style="width: <%=this.Width %>px;" class="topBar" id="topBar">
        <uc3:ToolBar ID="ToolBar1" runat="server" />
        <div style="float:right; font-weight:bold; font-size:17px; vertical-align:middle; margin:5px;" > <%= this.currND.Tip%>
        </div>
        <div id="Msg"></div>
    </div>
    <div style="width: <%=Width %>px;" class="flowInfo" id="flowInfo">
        <uc1:Pub ID="Pub1" runat="server" />
    </div>
    <div style="width: <%=Width %>px;" class="Message" id='Message'>
        <uc1:Pub ID="FlowMsg" runat="server" />
    </div>
    <div style="width: <%=Width %>px;" class="childThread" id='childThread'>
        <uc1:Pub ID="Pub3" runat="server" />
    </div>
    <center>
        <div style="width: <%=Width %>px; text-align: center;">
            <uc2:UCEn ID="UCEn1" runat="server" />
        </div>
    </center>
    <div style="width: <%=Width %>px;" class="pub2Class">
        <uc1:Pub ID="Pub2" runat="server" />
    </div>
    <div id="bottomToolBar" style="<%=Width %>px; text-align: left; display: none;" class="Bar">
        <uc3:ToolBar ID="ToolBar2" runat="server" />
    </div>
    <input type="hidden" id="FrmHeight" value="<%=Height %>" />
    <input type="hidden" id="BtnWord" value="<%=BtnWord %>" />
    <input type="hidden" id="HidWorkID" value="<%=this.WorkID %>" />
    <input type="hidden" id="CC_Url" value="./WorkOpt/CC.aspx?FK_Flow=<%=this.FK_Flow%>&FK_Node=<%=this.FK_Node %>&FID=<%=this.FID %>&WorkID=<%=this.WorkID %>&AtPara=23" />
    <input type="hidden" id="PrintFrom_Url" value="PrintSample.aspx?FK_Flow=<%=this.FK_Flow%>&FK_Node=<%=this.FK_Node %>&FID=<%=this.FID %>&WorkID=<%=this.WorkID %>&UserNo=<%=BP.Web.WebUser.No %>&AtPara=" />
</div>
