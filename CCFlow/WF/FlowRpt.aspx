<%@ Page Title="" Language="C#" MasterPageFile="WinOpenSimple.master" AutoEventWireup="true" CodeBehind="FlowRpt.aspx.cs" Inherits="CCFlow.WF.FlowRpt" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript">
    function WinOpen(url, winName) {
        var newWindow = window.open(url, winName, 'height=800,width=1030,top=' + (window.screen.availHeight - 800) / 2 + ',left=' + (window.screen.availWidth - 1030) / 2 + ',scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
        newWindow.focus();
        return;
    }
</script>

<style  type="text/css" >
</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 

<table style=" width:100%;">
<caption  class=CaptionMsgLong> 按流程查询 - <a href="Rpt/EasySearchMyFlow.aspx">高级查询</a> </caption>
 <td>

 <div style='float:left'>提示：点击流程名称，进入流程数据查询。</div>
 <div style='float:right'><a href="javascript:WinOpen('KeySearch.aspx',900,900);" >关键字查询</a> - 
                <a href="javascript:WinOpen('Comm/Search.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews',900,900);" >综合查询</a> - 
                <a href="javascript:WinOpen('Comm/Group.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews',900,900); " >综合分析</a> - 
                <a href="javascript:WinOpen('Comm/Search.aspx?EnsName=BP.WF.WorkFlowDeleteLogs',900,900); "  >删除日志</a></div>
 </td>
<tr>
<td>
    <uc1:Pub ID="Pub1" runat="server" />
    </td>
</tr>

    </table>
 
</asp:Content>