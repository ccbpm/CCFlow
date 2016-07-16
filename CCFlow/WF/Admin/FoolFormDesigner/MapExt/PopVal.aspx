<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="PopVal.aspx.cs" Inherits="CCFlow.WF.MapDef.PopVal" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style="width:100%">
<caption>为字段[<%=this.Request.QueryString["RefNo"] %>]设置pop返回值</caption>

<%
  string fk_mapdata=this.Request.QueryString["FK_MapData"];
  string refNo=this.Request.QueryString["RefNo"];
    
   BP.Sys.MapData md=new BP.Sys.MapData(fk_mapdata);
   //查询出实体.
   BP.Sys.MapExt ext = new BP.Sys.MapExt();
   ext.Retrieve(BP.Sys.MapExtAttr.FK_MapData, fk_mapdata, 
       BP.Sys.MapExtAttr.ExtType, "PopVal",
       BP.Sys.MapExtAttr.AttrOfOper, refNo);
  %>

<tr>
<td valign="top" > 
<fieldset>
 <legend><asp:RadioButton ID="RB_Model_Inntel" runat="server" Text="ccform内置URL" GroupName="Model"  /> </legend>
 <table style=" width:100%;">
 <tr> 
 <td  colspan="2">
 <a href="javascript:ShowHidden('group')" >数据源分组SQL:</a>
 <div id="group" style="color:Gray; display:none">
 <ul>
 <li>设置一个查询的SQL语句，必须返回No,Name两个列。</li>
 <li>比如:SELECT No,Name FROM Port_Dept </li>
 <li>如果内容需要树形的方式展示，需要返回约定的三个列。</li>
 <li>比如:SELECT No,Name,ParentNo FROM Port_Dept </li>
 <li>该参数可以为空,为空的时候，就不能以树的方式展示了，系统就会使用Table的方式把数据内容展示出来。</li>
 </ul>
 </div>
   <asp:TextBox ID="TB_Group" runat="server" TextMode="MultiLine" Rows="2" ToolTip="参数支持ccbpm的表达式,点击标签显示帮助."  Width="95%"></asp:TextBox> </td>
 </tr>

 <tr> 

 <td colspan="2" >
 <a href="javascript:ShowHidden('en')">数据源SQL:</a>
 <div id="en" style="color:Gray; display:none">
 <ul>
 <li>该参数可以为空. </li>
 <li>可以配置一个查询语句，该语句，支持ccbpm的表达式。</li>
 <li>Demo:有分组, SELECT No,Name,FK_Dept FROM Port_Emp </li>
 <li>Demo:无分组: SELECT No,Name FROM Port_Emp </li>
 </ul>
 </div>

   <asp:TextBox ID="TB_Entity" runat="server" TextMode="MultiLine"  Rows="2" ToolTip="参数支持ccbpm的表达式,点击标签显示帮助." Width="95%"></asp:TextBox> </td>
 </tr>
 
  <tr> 

 <td style="width:30%">
 <a href="javascript:ShowHidden('showway')">数据呈现方式:</a>
 <div id="showway" style="color:Gray; display:none">
 <ul>
 <li>配置的数据显示的风格. </li>
 <li>如果需要树形的解构显示，分组数据源必须是树解构的数据。</li>
 </ul>
 </div>

 
 </td>
  <td>
  <asp:RadioButton ID="RB_Table" Text="表格方式" runat="server" GroupName="ShowWay" />
&nbsp;<asp:RadioButton ID="RB_Tree" Text="树形方式" runat="server"  GroupName="ShowWay" />
      </td>
 </tr>

 
  <tr> 

 <td >
 <a href="javascript:ShowHidden('select')">选择数据方式:</a>
 <div id="select" style="color:Gray; display:none">
 <ul>
 <li>单项选择，就是每次只能选择一个项目,每个内容使用的是RadioButton控件. </li>
 <li>多项选择，每次可以选择多个项目，使用checkbox控件展示数据。</li>
 <li>多项选择返回的数据使用“;”分开。</li>
 </ul>
 </div>

  </td>
  <td>
  <asp:RadioButton ID="RB_PopValSelectModel_0" Text="多项选择" runat="server" GroupName="xxxx" />
&nbsp;<asp:RadioButton ID="RB_PopValSelectModel_1" Text="单项选择" runat="server"  GroupName="xxxx" />
      </td>
 </tr>
 

<tr> 

 <td colspan=2>
 <a href="javascript:ShowHidden('back')">返回值格式:</a>
 <div id="back" style="color:Gray; display:none">
 <ul>
 <li>以什么样的格式返回到文本框上。</li>
 <li>如果是接受人文本框需要使用“;”分开。</li>
 <li>比如：发送给：xxxxx;uuu;  抄送给：yyyy;xxxx;</li>
 </ul>
 </div>
  <br />
 <asp:RadioButton ID="RB_PopValFormat_0" runat="server" Text="No(仅编号)" GroupName="back" />
 <asp:RadioButton ID="RB_PopValFormat_1" runat="server" Text="Name(仅名称)" GroupName="back" />
 <asp:RadioButton ID="RB_PopValFormat_2" runat="server" Text="No,Name(编号与名称,比如zhangsan,张三;lisi,李四;)" GroupName="back" />
      </td>
 </tr>
 </table>
</fieldset>


<fieldset>
 <legend><asp:RadioButton ID="RB_Model_Url" runat="server" Text="自定义URL" GroupName="Model" /> </legend>
 <a href="javascript:ShowHidden('url')">&nbsp;请输入一个URL:</a>
 <div id="url" style="color:Gray; display:none">
 <ul>
 <li>您选择了使用外部的URL. </li>
 <li>使用外部的URL，我们提供了一个demo,您需要按照自己的格式来编写该url.</li>
 <li>该demo的位置 /SDKFlowDemo/SDKFlowDemo/PopSelectVal.aspx </li>
 </ul>
 </div>
    <asp:TextBox ID="TB_URL" runat="server" ToolTip="参数支持ccbpm的表达式,点击标签显示帮助."  Width="95%"></asp:TextBox>
</fieldset>

  </td>

   
</tr>

<tr>
<td colspan=2  > 
    <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
    <asp:Button ID="Btn_SaveAndClose" runat="server" Text="保存&关闭" 
        onclick="Btn_SaveAndClose_Click" />
        <input type=button value="关闭"  onclick="window.close()" />

        <asp:Button ID="Btn_Delete" runat="server" Text="删除" 
        OnClientClick="return confirm('您确定要删除吗？');" 
        onclick="Btn_Delete_Click"   />
    </td>
</tr>

</table>
</asp:Content>
