<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.MyFlowInfo"
    CodeBehind="MyFlowInfo.ascx.cs" %>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<%@ Register Src="./../Comm/UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc4" %>
<%@ Register Src="MyFlowInfoWap.ascx" TagName="MyFlowInfoWap" TagPrefix="uc3" %>
<br>
<div align="center">
    <span>
        <uc4:ToolBar ID="ToolBar1" runat="server" />
        <uc3:MyFlowInfoWap ID="MyFlowInfoWap1" runat="server" />
    </span>
</div>
