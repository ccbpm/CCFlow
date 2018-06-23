<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="D3.ascx.cs" Inherits="CCFlow.WF.Rpt.UC.D3" %>
<%@ Register Src="ToolBar.ascx" TagName="ToolBar" TagPrefix="uc1" %>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc2" %>
<script language="JavaScript" src="/WF/Comm/JScript.js" type="text/javascript" />
<script type="text/javascript" language="JavaScript">
    //  事件.
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
    function Cell(d1, d2, paras) {
    }
</script>
<div class="easyui-layout" data-options="fit:true">
    <div data-options="region:'north',noheader:true,split:false" style="padding: 2px;
        height: auto; background-color: #E0ECFF; line-height:30px">
        <uc1:ToolBar ID="ToolBar1" runat="server" />
    </div>
    <div data-options="region:'west',title:'分析条件'" style="padding: 5px; width: 200px;">
        <uc2:Pub ID="Left" runat="server" />
    </div>
    <div data-options="region:'center'" style="padding: 5px;">
        <uc2:Pub ID="Right" runat="server" />
    </div>
</div>