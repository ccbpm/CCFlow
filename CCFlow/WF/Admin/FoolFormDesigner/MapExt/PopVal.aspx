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
 <legend><asp:RadioButton ID="RB_Model_Inntel" runat="server" Text="popVal - 分组模式" GroupName="Model"  /> </legend>
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
 <li>Demo:有分组, SELECT No,Name,FK_Dept FROM Port_Emp 约定第三列就是分组列. </li>
 <li>Demo:无分组: SELECT No,Name FROM Port_Emp 可以支持多个列，多个列就显示table.</li>
 <li>Demo:使用分页弹窗,就不能使用分组了，在分页的SQL里需要有支持@PageCount @PageSize 约定变量.</li>
 <li>SELECT No,Name, FK_Dept, Demo:需要使用分页,就不能使用分组了，在分页的SQL里需要有支持@PageCount @PageSize 约定变量.</li>
 </ul>
 </div>
  <asp:TextBox ID="TB_Entity" runat="server" TextMode="MultiLine"  Rows="2" ToolTip="参数支持ccbpm的表达式,点击标签显示帮助." Width="95%"></asp:TextBox> </td>
 </tr>

 
 <tr> 
 <td colspan="2" >
 <a href="javascript:ShowHidden('countSQL')">获取数据源总行数SQL(不分页可以为空):</a>
 <div id="countSQL" style="color:Gray; display:none">
 <ul>
 <li>该参数可以为空. </li>
 <li>可以配置一个查询语句，该语句，支持ccbpm的表达式。</li>
 <li>Demo: SELECT count(*) FROM Port_Emp WHERE 1=1 </li>
 </ul>
 </div>
 <asp:TextBox ID="TB_SQL" runat="server" TextMode="MultiLine"  Rows="2" ToolTip="参数支持ccbpm的表达式,点击标签显示帮助." Width="95%"></asp:TextBox> </td>
 </tr>

 
 <tr> 
 <td colspan="2" >
 <a href="javascript:ShowHidden('searchSQL')">搜索SQL(可以为空):</a>
 <div id="searchSQL" style="color:Gray; display:none">
 <ul>
 <li>该参数可以为空. </li>
 <li>可以配置一个查询语句，该语句，支持ccbpm的表达式。</li>
 <li>Demo:有分组, SELECT No,Name,FK_Dept FROM Port_Emp WHERE Name LIKE '%@Key%' </li>
 <li>Demo:无分组: SELECT No,Name FROM Port_Emp  WHERE Name LIKE '%@Key%' </li>
 </ul>
 </div>
   <asp:TextBox ID="TB_SearchSQL" runat="server" TextMode="MultiLine"  Rows="2" ToolTip="参数支持ccbpm的表达式,点击标签显示帮助." Width="95%"></asp:TextBox> </td>
 </tr>
  
 </table>
</fieldset>



<fieldset>
 <legend><asp:RadioButton ID="RB_PageModel" runat="server" Text="popVal - 分页模式" GroupName="Model"  /> </legend>
 <table style=" width:100%;">

 <tr> 
 <td colspan="2" >
 <a href="javascript:ShowHidden('en')">数据源SQL:</a>
 <div id="Div2" style="color:Gray; display:none">
 <ul>
 <li>该参数可以为空. </li>
 <li>可以配置一个查询语句，该语句，支持ccbpm的表达式。</li>
 <li>Demo:有分组, SELECT No,Name,FK_Dept FROM Port_Emp 约定第三列就是分组列. </li>
 <li>Demo:无分组: SELECT No,Name FROM Port_Emp 可以支持多个列，多个列就显示table.</li>
 <li>Demo:使用分页弹窗,就不能使用分组了，在分页的SQL里需要有支持@PageCount @PageSize 约定变量.</li>
 <li>SELECT No,Name, FK_Dept, Demo:需要使用分页,就不能使用分组了，在分页的SQL里需要有支持@PageCount @PageSize 约定变量.</li>
 </ul>
 </div>
  <asp:TextBox ID="TextBox2" runat="server" TextMode="MultiLine"  Rows="2" ToolTip="参数支持ccbpm的表达式,点击标签显示帮助." Width="95%"></asp:TextBox> </td>
 </tr>
 
 <tr> 
 <td colspan="2" >
 <a href="javascript:ShowHidden('countSQL')">获取数据源总行数SQL(不分页可以为空):</a>
 <div id="Div3" style="color:Gray; display:none">
 <ul>
 <li>该参数可以为空. </li>
 <li>可以配置一个查询语句，该语句，支持ccbpm的表达式。</li>
 <li>Demo: SELECT count(*) FROM Port_Emp WHERE 1=1 </li>
 </ul>
 </div>
 <asp:TextBox ID="TextBox3" runat="server" TextMode="MultiLine"  Rows="2" ToolTip="参数支持ccbpm的表达式,点击标签显示帮助." Width="95%"></asp:TextBox> </td>
 </tr>
 
 <tr> 
 <td colspan="2" >
 <a href="javascript:ShowHidden('searchSQL')">搜索SQL(可以为空):</a>
 <div id="Div4" style="color:Gray; display:none">
 <ul>
 <li>该参数可以为空. </li>
 <li>可以配置一个查询语句，该语句，支持ccbpm的表达式。</li>
 <li>Demo:有分组, SELECT No,Name,FK_Dept FROM Port_Emp WHERE Name LIKE '%@Key%' </li>
 <li>Demo:无分组: SELECT No,Name FROM Port_Emp  WHERE Name LIKE '%@Key%' </li>
 </ul>
 </div>
   <asp:TextBox ID="TextBox4" runat="server" TextMode="MultiLine"  Rows="2" ToolTip="参数支持ccbpm的表达式,点击标签显示帮助." Width="95%"></asp:TextBox> </td>
 </tr>

  <tr> 
 <td style="width:30%">
 <a href="javascript:ShowHidden('showway')">数据呈现方式:</a>
 <div id="Div5" style="color:Gray; display:none">
 <ul>
  <li>配置的数据显示的风格. </li>
  <li>如果需要树形的解构显示，分组数据源必须是树解构的数据。</li>
 </ul>
 </div>

 </td>
 <td>
  <asp:RadioButton ID="RadioButton2" Text="表格方式" runat="server" GroupName="ShowWay" />
&nbsp;<asp:RadioButton ID="RadioButton3" Text="树形方式" runat="server"  GroupName="ShowWay" />
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

<fieldset style="line-height:30px">
<legend>其它配置</legend>


<table> 
 <tr> 
 <td colspan="2" >

宽度：<asp:TextBox ID="TB_Width" runat="server"  Width="100px" Text="760"></asp:TextBox>
高度：<asp:TextBox ID="TB_Height" runat="server"  Width="100px" Text="450"></asp:TextBox>
开窗标题：<asp:TextBox ID="TB_Title" runat="server"  Width="300px" Text="选择"></asp:TextBox>
 </td>
  </tr> 
   
 
<tr> 
 <td >
 <a href="javascript:ShowHidden('back')">返回值格式:</a>
 <div id="back" style="color:Gray; display:none">
 <ul>
 <li>以什么样的格式返回到文本框上。</li>
 <li>如果是接受人文本框需要使用“;”分开。</li>   
 <li>比如：发送给：xxxxx;uuu;  抄送给：yyyy;xxxx;</li>
 </ul>
 </div>
 </td>

 <td>
 <asp:RadioButton ID="RB_PopValFormat_0" runat="server" Text="No(仅编号)" GroupName="back" />
 <asp:RadioButton ID="RB_PopValFormat_1" runat="server" Text="Name(仅名称)" GroupName="back" />
 <asp:RadioButton ID="RB_PopValFormat_2" runat="server" Text="No,Name(编号与名称,比如zhangsan,张三;lisi,李四;)" GroupName="back" />
   </td>
 </tr>



 </table>

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
