<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true"
    CodeBehind="S4_ColsOrder.aspx.cs" Inherits="CCFlow.WF.MapDef.Rpt.ShowCols" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function DoLeft(fk_flow, rptNo, mapAttr, tkey) {
            window.location.href = '?DoType=ColumnsOrder&ActionType=Left&FK_Flow=' + fk_flow + '&RptNo=' + rptNo + '&FK_MapAttr=' + mapAttr+'&T='+tkey;
        }
        function DoRight(fk_flow, rptNo, idx,tkey) {
            window.location.href = '?DoType=ColumnsOrder&ActionType=Right&FK_Flow=' + fk_flow + '&RptNo=' + rptNo + '&FK_MapAttr=' + idx + '&T=' + tkey;
        }
    </script>
    <base target="_self" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <base target="_self" />
    <%
        string rptNo = this.Request.QueryString["RptNo"];
        string fk_MapData = this.Request.QueryString["FK_MapData"];
        string fk_flow = this.Request.QueryString["FK_Flow"];
    %>
    <div style="width: 100%; height: auto; margin: 0 auto;">
        <div style="width: 100%; height: auto; display: table;">
            <uc1:Pub ID="Pub2" runat="server" />
        </div>
        <div>
            <asp:Button ID="Btn_Save" runat="server" Text="SaveAndClose" Width="100" OnClick="Btn_Save_Click" />
            <asp:Button ID="Btn_SaveAndNext" runat="server" Text="SaveAndNext" Width="100" OnClick="Btn_Save_Click" />
            <asp:Button ID="Btn_Close" runat="server" Text="Cancel" Width="100" OnClick="Btn_Close_Click" />
        </div>
    </div>
</asp:Content>
