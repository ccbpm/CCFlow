<%@ Page Title="单选按钮的高级属性" Language="C#" MasterPageFile="~/WF/MapDef/WinOpen.master" AutoEventWireup="true" CodeBehind="RadioBtns.aspx.cs" Inherits="CCFlow.WF.MapDef.RadioBtns" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%
    //获得变量.
    string keyofEn = this.Request.QueryString["KeyOfEn"];
    string fk_mapdata = this.Request.QueryString["FK_MapData"];
    
    string selectIntKey = this.Request.QueryString["SelectIntKey"]; //选择的值.
    if (selectIntKey == null)
        selectIntKey = "1";
    
    //属性.
    BP.Sys.MapAttr attr = new BP.Sys.MapAttr(fk_mapdata + "_" + keyofEn);

    //单选按钮.
    BP.Sys.FrmRBs rbs=new BP.Sys.FrmRBs(fk_mapdata, keyofEn);
    
    
    //获得枚举值.
    BP.Sys.SysEnums ses = new BP.Sys.SysEnums(attr.UIBindKey);
%>

<table style="width:100%;" >
<caption>RadioButton字段[<%=attr.Name %>]的高级设置</caption>
<tr>

<td valign="top">

<fieldset>
<legend> 列表值 </legend>
<ul>
<%foreach (BP.Sys.SysEnum item in ses)
	{
        if (item.IntKey.ToString() == selectIntKey)
        {
	 %>
<li> <b><%=item.Lab%></b></li>

 <%  }else{ %>
     <li> <a href="?KeyOfEn=<%=keyofEn %>&FK_MapData=<%=fk_mapdata %>&SelectIntKey=<%=item.IntKey %>"> <%=item.Lab%></a></li>
   <%} %>
<%} %>
</ul>
</fieldset>


</td>

<td  valign="top"> 

 
<%
    
 %>

 <fieldset>
 <legend><asp:RadioButton ID="RB_0" runat="server"  GroupName="xx" Text="不设置" /> </legend>
 
 默认的选择项，不设置任何脚本与交互。   
    </fieldset>

    <fieldset>
 <legend><asp:RadioButton ID="RB_1" runat="server"  GroupName="xx" Text="执行JS脚本" /></legend>
 默认的选择项，不设置任何脚本与交互。   
  <div id="JS" >
       <textarea rows="3" cols="50"> 
       </textarea>
    </div>
    </fieldset>


    <fieldset>
 <legend>
    <asp:RadioButton ID="RB_2" runat="server"  GroupName="xx" Text="联动其他的控件使其属性该表(可见，只读)" /></legend>

    <div id="Fields" >
    <table> 
    <tr> 
    <th>序</th>
    <th>字段</th>
    <th>字段名</th>
    <th>可用</th>
    <th>可见</th>
     </tr>

     <%
         int idx = 0;
         BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs(fk_mapdata);
         foreach (BP.Sys.MapAttr myattr in attrs)
         {
             idx++;
      %>
     <tr> 

    <td><%=idx %></td>
    <td><%=myattr.KeyOfEn%></td>
    <td><%=myattr.Name%></td>

    <td><asp:CheckBox ID="CB_Visable_" runat="server" Text="可见" /></td>
    <td><asp:CheckBox ID="CB_Enable_"  runat="server" Text="可用" /> </td>
     </tr>

      <%} %>
    </table>
    </div>

     </fieldset>


</td>

</tr>
<tr>
<td colspan="2">  
    <asp:Button ID="Btn_Save" runat="server" Text="保存" />
    <asp:Button ID="Btn_SaveAndClose" runat="server" Text="保存并关闭" />
    <asp:Button ID="Btn_Close" runat="server" Text="关闭" />
    </td>
</tr>
</table>


</asp:Content>
