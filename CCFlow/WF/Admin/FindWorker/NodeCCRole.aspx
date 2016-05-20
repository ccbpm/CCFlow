﻿<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true"
    CodeBehind="NodeCCRole.aspx.cs" Inherits="CCFlow.WF.Admin.FindWorker.NodeCCRole" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script src="../../Comm/JScript.js" type="text/javascript"></script>
    <style type="text/css">
        .style1
        {
            width: 169px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
        BP.WF.Node nd = new BP.WF.Node(FK_Node);
        string sj = DateTime.Now.ToString("MMddhhmmss");

        BP.WF.Template.CCStations nss = new BP.WF.Template.CCStations();
        nss.Retrieve(BP.WF.Template.CCStationAttr.FK_Node, this.FK_Node);

        BP.WF.Template.CCDepts ndepts = new BP.WF.Template.CCDepts();
        ndepts.Retrieve(BP.WF.Template.CCDeptAttr.FK_Node, this.FK_Node);

        BP.WF.Template.CCEmps nEmps = new BP.WF.Template.CCEmps();
        nEmps.Retrieve(BP.WF.Template.CCEmpAttr.FK_Node, this.FK_Node);
    %>
    <table style="width: 100%;">
        <caption>
            抄送设置
        </caption>
        <tr>
            <th colspan="2">
                基本设置
            </th>
        </tr>
        <tr>
            <td class="style1">
                抄送规则
            </td>
            <td>
                <asp:DropDownList ID="DDL_CCRole" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="style1">
                抄送写入规则
            </td>
            <td>
                <asp:DropDownList ID="DDL_CCWriteTo" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="style1">
                标题模板
            </td>
            <td>
                <asp:TextBox ID="TB_title_MB" runat="server" Width="421px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                内容模版
            </td>
            <td>

                <asp:TextBox ID="TB_content_MB" TextMode="MultiLine" runat="server" Height="90px" 

                    Width="421px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th colspan="2">
                自动抄送人范围
            </th>
        </tr>
        <tr>
            <td class="style1">
                <asp:CheckBox ID="CC_GW" Text="抄送到岗位" runat="server" />
            </td>
            <td>
                <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotSingle.aspx?EnsName=BP.WF.Template.CCs&EnName=BP.WF.Template.CC&AttrKey=BP.WF.Template.CCStations&NodeID=<%=nd.NodeID %>&r=<%=sj%>&ShowWay=None')">
                    请选择岗位(<%=nss.Count %>)</a>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:CheckBox ID="CC_DBM" Text="抄送到部门" runat="server" />
            </td>
            <td>
                <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotSingle.aspx?EnsName=BP.WF.Template.CCs&EnName=BP.WF.Template.CC&AttrKey=BP.WF.Template.CCDepts&NodeID=<%=nd.NodeID %>&r=<%=sj%>')">
                    请选择部门(<%=ndepts.Count %>)</a>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:CheckBox ID="CC_RY" Text="抄送到人员" runat="server" />
            </td>
            <td>
                <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotSingle.aspx?EnsName=BP.WF.Template.CCs&EnName=BP.WF.Template.CC&AttrKey=BP.WF.Template.CCEmps&NodeID=<%=nd.NodeID %>&r=<%=sj%>')">
                请选择人员(<%=nEmps.Count %>)</a>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:CheckBox ID="CC_SQL" Text="按照SQL设置范围" runat="server" />
            </td>
            <td>
                <asp:TextBox ID="CC_SQL1" TextMode="MultiLine" runat="server" Width="421px"></asp:TextBox>
            </td>
        </tr>
    </table>
    <asp:Button ID="Btn_Save" runat="server" Text="保存" OnClick="Btn_Save_Click" />
    <asp:Button ID="Btn_SaveAndClose" runat="server" Text="保存并关闭" OnClick="Btn_SaveAndClose_Click" />
    <asp:Button ID="Btn_Close" runat="server" Text="关闭" OnClick="Btn_Close_Click" />
</asp:Content>
