<%@ Page Title="绑定独立表单" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" CodeBehind="BindFrms.aspx.cs" Inherits="CCFlow.WF.Admin.BindFrms" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    function New() {
        window.location.href = window.location.href;
    }

    function BindFrms(nodeid, fk_flow) {
        var url = "BindFrms.aspx?FK_Node=" + nodeid + '&FK_Flow='+ fk_flow +'&DoType=SelectedFrm';
        window.location.href = url;
    }

    function WinField(fk_mapdata, nodeid, fk_flow) {
        var url = "../MapDef/Sln.aspx?FK_MapData=" + fk_mapdata + "&FK_Node=" + nodeid + '&FK_Flow=' + fk_flow + '&DoType=Field';
        WinOpen(url);
    }

    function WinFJ(fk_mapdata, nodeid, fk_flow) {
        var url = "../MapDef/Sln.aspx?FK_MapData=" + fk_mapdata + "&FK_Node=" + nodeid + '&FK_Flow=' + fk_flow + '&DoType=FJ';
        WinOpen(url);
    }

    function WinDtl(fk_mapdata, nodeid, fk_flow) {
        var url = "../MapDef/Sln.aspx?FK_MapData=" + fk_mapdata + "&FK_Node=" + nodeid + '&FK_Flow=' + fk_flow + '&DoType=Dtl';
        WinOpen(url);
    }

    function ToolbarExcel(fk_mapdata, nodeid, fk_flow) {
        var pk = fk_mapdata + '_' + nodeid + '_' + fk_flow;
        var url = "../Comm/RefFunc/UIEn.aspx?EnName=BP.Sys.ToolbarExcelSln&PK=" + pk;
        WinOpen(url);
    }

    function ToolbarWord(fk_mapdata, nodeid, fk_flow) {
        var pk = fk_mapdata + '_' + nodeid + '_' + fk_flow;
        var url = "../Comm/RefFunc/UIEn.aspx?EnName=BP.Sys.ToolbarWordSln&PK=" + pk;
        WinOpen(url);
    }

    function AddIt(fk_mapdata, fk_node, fk_flow) {
        var url = 'FlowFrms.aspx?DoType=Add&FK_MapData=' + fk_mapdata + '&FK_Node=' + fk_node + '&FK_Flow=' + fk_flow;
        window.location.href = url;
    }
    function DelIt(fk_mapdata, fk_node, fk_flow) {
        if (window.confirm('您确定要移除吗？') == false)
            return;
        var url = 'FlowFrms.aspx?DoType=Del&FK_MapData=' + fk_mapdata + '&FK_Node=' + fk_node + '&FK_Flow=' + fk_flow;
        window.location.href = url;
    }
</script>
<base target="_self" />
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
