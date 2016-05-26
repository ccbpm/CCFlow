<%@ Page Title="表单启用规则" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="FrmEnableRole.aspx.cs" Inherits="CCFlow.WF.Admin.FlowFrm.FrmEnableRole" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<style  type="text/css" >
ul
{
    
}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%
    string frmID = this.Request.QueryString["FK_MapData"];
    BP.Sys.MapData md=new BP.Sys.MapData(frmID);
    int  nodeID = int.Parse( this.Request.QueryString["FK_Node"]);
    BP.WF.Node nd=new BP.WF.Node(nodeID);

 %>
<table  style=" width:100%;">

<caption>节点[<%=nd.Name%>] - 表单[<%=md.Name%>]</caption>

<tr>
<td>

<fieldset>
<legend><asp:RadioButton ID="RB_0" runat="server" Text="始终启用" GroupName="xxx" /></legend>
<ul >
<li>始终启用该表单，任何情况下都启用该表单。</li>
</ul>
</fieldset>

<fieldset>
<legend><asp:RadioButton ID="RB_1" runat="server" Text="有数据时启用"  GroupName="xxx" /></legend>
<ul>
<li>如果当前节点有数据，就要启用它，如果当前节点有数据初始化数据，就启用该表单。</li>
</ul>
</fieldset>

<fieldset>
<legend><asp:RadioButton ID="RB_2" runat="server" Text="有参数时启用" GroupName="xxx" /></legend>
<ul>
<li>当外面的参数传递过来该表单ID的时候，就启用该表单。</li>
<li>通过外部的URL传递过来的参数，这个参数名称是固定的 Frms，比如:&Frms=Demo_Frm1，Demo_Frm1是一个表单ID.</li>
<li> 如果多个表单用逗号分开，比如：&Frms=Demo_Frm1,Demo_Frm2,Demo_Frm3</li>
</ul>
</fieldset>

<fieldset>
<legend><asp:RadioButton ID="RB_3" runat="server" Text="表单字段表达式成立的时候(未完成)." GroupName="xxx" /></legend>
<ul>
<li>当指定表单的指定的字段等于特定的值的时候，该表单启用。</li>
<li>指定方式类似于方向条件，比如：当金额大于xxx元时。</li>
</ul>
</fieldset>


<fieldset>
<legend><asp:RadioButton ID="RB_4" runat="server" Text="当SQL表达式返回大于0的数据的时" GroupName="xxx" /></legend>
<ul>
<li>请配置一个SQL语句该语句支持cc表达式,并且返回一行一列，如果该值大于0，这个表单就启用，否则就不启用。</li>
</ul>
<asp:TextBox ID="TB_SQL" runat="server" TextMode="MultiLine" Rows="4"  Columns="80" >
</asp:TextBox>

</fieldset>


<fieldset>
<legend><asp:RadioButton ID="RB_5" runat="server" Text="不启用（禁用它）" GroupName="xxx" /></legend>
<ul>
<li>这个是节点表单的默认值</li>
<li>如果其他表单暂时不启用，就设置该规则。</li>
</ul>

</fieldset>


 </td>
</tr>


<tr>
<td>
  <asp:Button ID="Btn_SaveAndClose" runat="server" Text="保存并关闭" 
        onclick="Btn_SaveAndClose_Click" />
    <asp:Button ID="Btn_Close" runat="server" Text="关闭"  OnClientClick="javascript:windows.close()" />
</td>
</tr>


</table>
</asp:Content>
