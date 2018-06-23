<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Group.ascx.cs" Inherits="CCFlow.WF.Rpt.UC.Group" %>
<%@ Register TagPrefix="uc1" TagName="UCSys" Src="../../Comm/UC/UCSys.ascx" %>
<%@ Register Src="ToolBar.ascx" TagName="ToolBar" TagPrefix="uc2" %>
<%@ Register Src="../../Comm/UC/UCGraphics.ascx" TagName="ucgraphics" TagPrefix="uc3" %>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc4" %>
<style type="text/css">
    .MyTable
    {
        text-align: left;
    }
</style>

<base target="_self" />
<script type="text/javascript">
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
</script>
<div class="easyui-layout" data-options="fit:true">
    <div data-options="region:'north',noheader:true,split:false" style="padding: 2px;
        height: auto; background-color: #E0ECFF; line-height: 30px">
        <uc2:ToolBar ID="ToolBar1" runat="server" />
    </div>
    <div data-options="region:'west',title:'分组条件',split:true" style="padding: 5px; width: 260px;">
        <div class="easyui-panel" title="显示内容" style="padding: 5px; margin-bottom: 5px">
            <asp:CheckBoxList ID="CheckBoxList1" BorderStyle="None" Width="100%" runat="server"
                AutoPostBack="true">
            </asp:CheckBoxList>
        </div>
        <div class="easyui-panel" title="分析项目" style="padding: 5px; margin-bottom: 5px">
            <uc1:UCSys ID="UCSys2" runat="server"></uc1:UCSys>
        </div>
        <div class="easyui-panel" title="图表" style="padding: 5px; margin-bottom: 5px">
            <table class="Table" cellpadding="0" cellspacing="0" border="0" style="width: 100%">
                <tr>
                    <td>
                        高度：<asp:TextBox ID="TB_H" runat="server" Text="460" Style="width: 60px; height: auto;
                            text-align: right"></asp:TextBox>
                    </td>
                    <td style="text-align: right">
                        宽度：<asp:TextBox ID="TB_W" runat="server" Text="800" Style="width: 60px; height: auto;
                            text-align: right"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="CB_IsShowPict" runat="server" Text="显示图表" AutoPostBack="true" ToolTip="注意：仅当“显示内容”选择1项时，图表功能才可用！"
                            Style="float: left" /><span style="float: right"><asp:LinkButton ID="lbtnApply" runat="server"
                                CssClass="easyui-linkbutton" data-options="iconCls:'icon-ok',plain:true" OnClick="lbtnApply_Click">应用</asp:LinkButton></span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div data-options="region:'center'" style="padding: 5px;">
        <uc1:UCSys ID="UCSys3" runat="server"></uc1:UCSys>
        <uc1:UCSys ID="UCSys1" runat="server"></uc1:UCSys>
        <uc1:UCSys ID="UCSys4" runat="server"></uc1:UCSys>
    </div>
</div>
