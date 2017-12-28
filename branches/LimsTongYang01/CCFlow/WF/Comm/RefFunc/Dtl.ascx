<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.Comm.RefFunc.DtlUC"
    CodeBehind="Dtl.ascx.cs" %>
<%@ Register Src="./../UC/ucsys.ascx" TagName="ucsys" TagPrefix="uc1" %>
<%@ Register Src="./../UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc2" %>
<table id="Table1" align="left" width="100%">
    <tr>
        <td class="ToolBar">
            <uc2:ToolBar ID="ToolBar1" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <uc1:ucsys ID="ucsys1" runat="server" />
            <uc1:ucsys ID="ucsys2" runat="server" />
        </td>
    </tr>
</table>
