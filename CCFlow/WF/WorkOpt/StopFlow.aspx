<%@ Page Title="中止流程" Language="C#" MasterPageFile="~/WF/SDKComponents/Site.Master" AutoEventWireup="true" CodeBehind="StopFlow.aspx.cs" Inherits="CCFlow.WF.WorkOpt.StopFlow" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <script type="text/javascript">
     function NoSubmit(ev) {
         if (window.event.srcElement.tagName == "TEXTAREA")
             return true;
         if (ev.keyCode == 13) {
             window.event.keyCode = 9;
             ev.keyCode = 9;
             return true;
         }
         return true;
         
          
     }
    </script>
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">



 <table style=" text-align:left; width:100%">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %></caption>
<tr>
<td valign=top style=" text-align:center">
    <br>
    <br>


<table style="width:500px" >

<tr>
<th valign=top colspan=2>
 结束流程(请输入结束流程的原因)
</th>
</tr>

<tr>
<td valign=top>
说明:
<ul style=" margin:3px ">
<li>1.流程结束标识流程已经走完,以后的节点不再执行.</li>
<li>2.走完后的流程在已完成的流程中可以查询到.</li>
<li>3.要想恢复流程向下运行需求告知管理员.</li>
</ul>
</td>
<td valign=top style="width:300px">
    <asp:TextBox ID="TextBox1" runat="server" Height="110px" TextMode="MultiLine" 
        Width="100%"></asp:TextBox>
    </td>
</tr>
<tr>
<td></td>
<td align=right>
    <asp:Button ID="Btn_OK" runat="server" Text="确定" onclick="Btn_OK_Click" />
    <asp:Button ID="Btn_Cancel" runat="server"  
        OnClientClick="window.location.go(-1);" Text="取消" onclick="Btn_Cancel_Click" />
    </td>
</tr>
</table>

<br>
<br>
<br>
<br>
<br>
<br>
    </td>
</tr>
</table>

</asp:Content>
