<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Search.ascx.cs" Inherits="CCFlow.WF.Rpt.Search" %>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<%@ Register Src="ToolBar.ascx" TagName="ToolBar" TagPrefix="uc2" %>
<%@ Register Src="../../Comm/UC/UCSys.ascx" TagName="UCSys" TagPrefix="uc3" %>
<script language="javascript" type="text/javascript">
    function ShowEn(url, wName, h, w) {
        h = 700;
        w = 900;
        var s = "dialogWidth=" + parseInt(w) + "px;dialogHeight=" + parseInt(h) + "px;resizable:yes";
        var val = window.showModalDialog(url, null, s);
        window.location.href = window.location.href;
    }

    function ImgClick() {
    }

    function OpenAttrs(ensName) {
        var url = './../../Sys/EnsAppCfg.aspx?EnsName=' + ensName;
        var s = 'dialogWidth=680px;dialogHeight=480px;status:no;center:1;resizable:yes'.toString();
        val = window.showModalDialog(url, null, s);
        window.location.href = window.location.href;
    }

    function Setting(rptNo, flowNo) {
        alert(rptNo);
        var url = '../Admin/FoolFormDesigner/Rpt/S0_RptList.htm?FK_MapData=' + rptNo + '&FK_Flow=' + flowNo;
        var s = 'dialogWidth=680px;dialogHeight=480px;status:no;center:1;resizable:yes'.toString();
        val = window.showModalDialog(url, null, s);
        window.location.href = window.location.href;
    }

    function DDL_mvals_OnChange(ctrl, ensName, attrKey) {
        var idx_Old = ctrl.selectedIndex;

        if (ctrl.options[ctrl.selectedIndex].value != 'mvals')
            return;

        if (attrKey == null)
            return;

        var timestamp = Date.parse(new Date());
        var url = 'SelectMVals.aspx?EnsName=' + ensName + '&AttrKey=' + attrKey + '&D=' + timestamp;
        var val = window.showModalDialog(url, 'dg', 'dialogHeight: 450px; dialogWidth: 450px; center: yes; help: no');

        if (val == '' || val == null) {
            ctrl.selectedIndex = 0;
        }
    }
</script>
<div class="easyui-layout" data-options="fit:true">
    <div data-options="region:'north',noheader:true,split:false" style="padding: 2px;
        height: auto; background-color: #E0ECFF; line-height: 30px">
        <uc2:ToolBar ID="ToolBar1" runat="server" />
    </div>
    <div data-options="region:'center',noheader:true,border:false">
        <uc3:UCSys ID="UCSys1" runat="server" />
        <uc1:Pub ID="Pub2" runat="server" />
    </div>
</div>
