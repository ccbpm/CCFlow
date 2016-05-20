<%@ Control Language="C#" AutoEventWireup="true" Inherits="WF_MapDef_UC_MExt" Codebehind="MExt.ascx.cs" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<script type="text/javascript">
    function DoDel(mypk,fk_mapdata,extType) {
        if (window.confirm('您确定要删除吗？') == false)
            return;
        window.location.href = 'MapExt.aspx?DoType=Del&FK_MapData=' + fk_mapdata + '&ExtType=' + extType + '&MyPK=' + mypk;
    }
</script>
<div class="easyui-layout" data-options="fit:true">
    <div data-options="region:'west',split:true,title:'功能列表'" style="width:200px">
        <uc1:Pub ID="Left" runat="server" />

    </div>
    <div data-options="region:'center',title:''" style="padding:5px">
        <uc1:Pub ID="Pub2" runat="server" />
    </div>
</div>
    
    