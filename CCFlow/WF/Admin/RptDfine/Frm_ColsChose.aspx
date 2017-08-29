<%@ Page Title="" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true"
    CodeBehind="Frm_ColsChose.aspx.cs" Inherits="CCFlow.WF.MapDef.Rpt.Frm_ColsChose" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        #top
        {
            width: 100%;
            height: 24px;
            text-align: center;
        }
        #top *
        {
            position: relative;
        }
        .toolBar
        {
            width: 100%;
            float: left;
            z-index: 1000;
            overflow: visible;
            position: fixed;
            left: 0px;
            top: 0px;
            _position: absolute;
            _top: expression_r(documentElement.scrollTop);
            background: #909090;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function Esc() {

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="toolBar">
        <div id="top">
            <asp:Button ID="Btn_Save" runat="server" Text="保存" Width="60" OnClick="Btn_Save_Click" />
            <asp:Button ID="Btn_Column" runat="server" Text="设置列表属性" Width="100" OnClick="Btn_Column_Click" />
        </div>
    </div>
    <div style="width: 100%; height: auto; display: table; margin-top: 20px;">
        <uc1:Pub ID="Pub1" runat="server" />
    </div>
</asp:Content>
