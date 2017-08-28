<%@ Page Title="5. 设置报表权限" Language="C#" MasterPageFile="RptGuide.Master" AutoEventWireup="true"
    CodeBehind="S6_Power.aspx.cs" Inherits="CCFlow.WF.MapDef.Rpt.S6_Power" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <script type="text/javascript">
        function OpenDailog(dlgTitle, rpt, rptNo) {
            var url = "../../../Comm/RefFunc/Dot2DotSingle.aspx?EnsName=BP.WF.Rpt.MapRpts&EnName=BP.WF.Rpt.MapRpt&AttrKey=BP.WF.Rpt." + rpt + "&No=" + rptNo;
            OpenEasyUiDialog(url, 'eudlgframe', dlgTitle, 520, 321, null, false);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<fieldset>
<legend>查看权限控制</legend>
[ ]任何人都可以查看，流程数据。
[ ]具有如下 {岗位/部门} 的可以查看。
   岗位
   部门.
[ ]如下指定的人可以查看,请配置一个sql.    
</fieldset>

---- 部门数据权限 ------------
   【】可查看所有部门数据。
   【】可查看本部门数据。
    【】 可查看本部门，本部门子级部门数据。
    【】仅可以查看我参与的流程数据。
    【】可以查看指定部门的流程数据。


    <ul class="navlist">
        <li>
            <div>
                <a href="javascript:OpenDailog('1. 岗位权限', 'RptStations','<%=this.RptNo%>')"><span class="nav">
                    1. 岗位权限</span></a></div>
        </li>
        <li>
            <div>
                <a href="javascript:OpenDailog('2. 部门权限', 'RptDepts','<%=this.RptNo%>')"><span class="nav">
                    2. 部门权限</span></a></div>
        </li>
        <li>
            <div>
                <a href="javascript:OpenDailog('3. 人员权限', 'RptEmps','<%=this.RptNo%>')"><span class="nav">
                    3. 人员权限</span></a></div>
        </li>
    </ul>




    <cc1:LinkBtn ID="Btn_Cancel1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-undo'"
        Text="取消" OnClick="Btn_Cancel_Click" />


</asp:Content>
