<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RefDtl.ascx.cs" Inherits="CCFlow.WF.Comm.RefFunc.RefDtl" %>
<%@ Register Src="./../UC/ucsys.ascx" TagName="ucsys" TagPrefix="uc1" %>
<%@ Register Src="./../UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc2" %>
<div class="easyui-layout" data-options="fit:true">
    <div data-options="region:'north',noheader:true,split:false,border:false" style="height: 30px;
        padding: 2px; background-color: #E0ECFF">
        <uc2:ToolBar ID="ToolBar1" runat="server" />
    </div>
    <div data-options="region:'center',noheader:true,border:false">
        <uc1:ucsys ID="ucsys1" runat="server" />
        <uc1:ucsys ID="ucsys2" runat="server" />
    </div>
</div>
