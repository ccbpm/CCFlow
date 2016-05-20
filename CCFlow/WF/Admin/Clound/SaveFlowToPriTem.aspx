<%@ Page Title="流程导入" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true"
    CodeBehind="SaveFlowToPriTem.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.SaveFlowToPriTem" %>

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
                    <legend>私有流程云</legend>
                    <ul style="list-style: none;">
                        <li>请选择私有流程类别:
                            <asp:DropDownList ID="DropDownList1" runat="server" Width="214px">
                            </asp:DropDownList>
                        </li>
                        <div style="text-align: center; padding: 5px;">
                            <asp:Button ID="Button1" runat="server" Text="保存" OnClick="Button1_Click" />
                        </div>
                    </ul>
                </fieldset>
            </td>
        </tr>
    </table>
</asp:Content>
