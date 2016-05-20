<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow_Comm_UC_UIEn" CodeBehind="UIEn.ascx.cs" %>
<%@ Register Src="UCEn.ascx" TagName="UCEn" TagPrefix="uc1" %>
<%@ Register Src="ToolBar.ascx" TagName="ToolBar" TagPrefix="uc2" %>
<div id="uienright" class="easyui-layout" data-options="fit:true">
    <div data-options="region:'north',noheader:true,split:false,border:false" style="height: 30px;
        padding: 2px; background-color:#E0ECFF; overflow-y:hidden">
            <uc2:ToolBar ID="ToolBar1" runat="server" />
    </div>
    <div data-options="region:'center',noheader:true,border:false">
        <uc1:UCEn ID="UCEn1" runat="server" />
    </div>
</div>
