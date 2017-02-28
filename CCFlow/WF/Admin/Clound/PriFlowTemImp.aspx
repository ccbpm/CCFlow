<%@ Page Title="流程导入" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true"
    CodeBehind="PriFlowTemImp.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.PriFlowTemImp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <div id="LoadingBar" style="margin-left: auto; margin-right: auto; margin-top: 40%;
        width: 250px; height: 38px; line-height: 38px; padding-left: 50px; padding-right: 5px;
        background: url(../../Scripts/easyUI/themes/default/images/pagination_loading.gif) no-repeat scroll 5px 10px;
        border: 2px solid #95B8E7; color: #696969; font-family: 'Microsoft YaHei'">
        正在连接到云服务器,请稍候…
    </div>
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="js/loading.js" type="text/javascript"></script>
    <script type="text/javascript">
        function openFlow(flowDType, flowName, flowNo, webUserNo, webUserSID) {
            if (confirm("导入成功,是否打开流程")) {
                if (flowDType == 2) {//BPMN模式
                    window.parent.closeTab(flowName);
                    window.parent.addTab('flow', flowName, "../CCBPMDesigner/Designer.aspx?FK_Flow=" +
                        flowNo + "&UserNo=" + webUserNo + "&SID=" + webUserSID + "&Flow_V=2", '');
                } else if (flowDType == 1) {//CCBPM
                    window.parent.closeTab(flowName);
                    window.parent.addTab('flow', flowName, '../CCBPMDesigner/Designer.aspx?FK_Flow=' +
                  flowNo + "&UserNo=" + webUserNo + "&SID=" + webUserSID + "&Flow_V=1", '');
                } else {
                    //                    if (confirm("此流程版本为V1.0,是否执行升级为V2.0 ?")) {
                    window.parent.closeTab(flowName);
                    window.parent.addTab('flow', flowName, "../CCBPMDesigner/Designer.aspx?FK_Flow=" +
                  flowNo + "&UserNo=" + webUserNo + "&SID=" + webUserSID + "&Flow_V=0", '');
                    //                    } else {
                    //                        window.parent.closeTab(flowName);
                    //                        window.parent.addTab('flow', flowName, "../CCBPMDesigner/DesignerSL.htm?FK_Flow=" +
                    //                  flowNo + "&UserNo=" + webUserNo + "&SID=" + webUserSID + "&Flow_V=0", '');
                    //                    }
                }
                window.parent.closeTab("流程信息"); //关闭导入tabs
            }
        }
    </script>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            font-size: 12px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100%">
        <tr>
            <td valign="top">
                <fieldset>
                    <legend>请选择导入类别</legend>
                    <ul style="list-style: none;">
                        <li>请选择流程类别:
                            <asp:DropDownList ID="DropDownList1" runat="server" Width="214px">
                            </asp:DropDownList>
                        </li>
                    </ul>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <fieldset>
                    <legend>请选择导入方式</legend>
                    <ul style="list-style: none;">
                        <li>
                            <asp:RadioButton ID="RB_Import_1" Text="作为新流程导入(由ccbpm自动生成新的流程编号)" GroupName="Import_mode"
                                runat="server" Checked="true" />
                            <br />
                            <asp:RadioButton ID="RB_Import_2" Text="作为新流程导入(使用流程模版里面的流程编号，如果该编号已经存在系统则会提示错误)"
                                GroupName="Import_mode" runat="server" />
                            <br />
                            <asp:RadioButton ID="RB_Import_3" Text="作为新流程导入(使用流程模版里面的流程编号，如果该编号已经存在系统则会覆盖此流程)"
                                GroupName="Import_mode" runat="server" />
                            <br />
                            <asp:RadioButton ID="RB_Import_4" Text="按指定流程编号导入" GroupName="Import_mode" runat="server" />
                            指定的流程编号:<asp:TextBox ID="SpecifiedNumber" runat="server"></asp:TextBox>
                            <br />
                        </li>
                    </ul>
                    <div style="text-align: center; padding: 5px;">
                        <asp:Button ID="Button1" runat="server" Text="执行导入" OnClick="Button1_Click" />
                    </div>
                </fieldset>
            </td>
        </tr>
    </table>
</asp:Content>
