<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_Calendar" Codebehind="Calendar.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    function CelldblClick(v1, v2) {
    }
</script>
<style type="text/css">
    
.Holiday
{
    text-align:center;
    vertical-align:middle;
    font-size:12px;
    background-color:#FFFFFF;
}

.HolidayHave
{
    font-weight:bolder;
    text-align:center;
    vertical-align:middle;
    font-size:12px;
    background-color:#FFFFFF;
}

.Day
{
    text-align:center;
    vertical-align:middle;
    font-size:12px;
}

.DayHave
{
    text-align:center;
    vertical-align:middle;
    font-size:small;
    font-size:12px;
    background-color:Orange;
}

.Week
{
    background-color:ButtonFace;
    text-align:center;
    vertical-align:middle;
    font-weight:bolder;
}

</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table   style="width:80%;height:450px;border:1px" align=center >
<tr>
<td valign=top style="width:20%">
    <uc1:Pub ID="Left" runat="server" />
    </td>
<td valign=top>
    <uc1:Pub ID="Pub1" runat="server" />
    </td>
</tr>
</table>

</asp:Content>

