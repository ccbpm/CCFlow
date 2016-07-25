<%@ Page Title="新增字段向导" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="FieldTypeList.aspx.cs" Inherits="CCFlow.WF.Admin.FoolFormDesigner.FieldTypeList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style="width:100%;">
<tr>
<th colspan="2">新增普通类型的字段</th>
</tr>

<tr>
<td valign="top" style="width:auto" >
<!--  xxxxxxxxxxxxxxxxxxxx   -->
<table>
<tr>
<td nowarp=true >字段中文名称</td>
<td> 
    <asp:TextBox ID="TB_Name"  AutoPostBack=true runat="server" 
        ontextchanged="TB_Name_TextChanged" ></asp:TextBox>
    </td>
</tr>

<tr>
<td  nowarp=true>字段名</td>
<td> 
    <asp:TextBox ID="TB_No" runat="server"></asp:TextBox>
    </td>
</tr>

<tr>
<td> 
    <asp:RadioButton ID="RB_PY_0" GroupName="PY" Text="全拼" AutoPostBack=true runat="server"  Checked=true
        oncheckedchanged="RB_PY_CheckedChanged" />
    </td>
<td> 
    <asp:RadioButton ID="RB_PY_1"  GroupName="PY" Text="简拼" AutoPostBack=true runat="server" 
        oncheckedchanged="RB_PY_CheckedChanged" />
    </td>
</tr>


<tr>
<td colspan="2"> 
    <asp:Button ID="Btn_Save" runat="server" Text="创建(C)" AccessKey="c" 
        onclick="Btn_Save_Click" />
    </td>
</tr>

</table>

<!-- end  xxxxxxxxxxxxxxxxxxxx   -->

 </td>

<td style=" width:60%"> 
    <asp:RadioButton ID="RB_String" Text="字符型。 "  GroupName="F" Checked=true runat="server" /> 
    <font color="Gray" >如:姓名、地址、邮编、电话</font><br/>

    <asp:RadioButton ID="RB_Int" Text="整数型。"  GroupName="F"  runat="server" />
    <font color="Gray" >如:年龄、个数。</font><br />

    <asp:RadioButton ID="RB_Money" Text="金额型。"  GroupName="F"  runat="server" />
    <font color="Gray" >如:单价、薪水。</font><br />

    <asp:RadioButton ID="RB_Float" Text="浮点型。"  GroupName="F"  runat="server" />
    <font color="Gray" >如：身高、体重、长度。</font><br />

     <asp:RadioButton ID="RB_Double" Text="双精度。"  GroupName="F"  runat="server" />
      <font color="Gray" >如：亿万、兆数值类型单位。</font><br />

      <asp:RadioButton ID="RB_Data" Text="日期型。"  GroupName="F"  runat="server" />
       <font color="Gray"> 如：出生日期、发生日期。</font><br />

      <asp:RadioButton ID="RB_DataTime" Text="日期时间型。"  GroupName="F"  runat="server" />
        <font color="Gray" >如：发生日期时间</font><br />

         <asp:RadioButton ID="RB_Boolen" Text="Boole型(是/否)。"  GroupName="F"  runat="server" />
        <font color="Gray" >如：是否完成、是否达标</font><br />

    </td>


</tr>


<tr>
<th colspan="2">新增枚举字段(用来表示，状态、类型...的数据。)</th>
</tr>

<tr>
<td colspan="2">

<ul>
<li><a href='SysEnumList.aspx?DoType=AddSysEnum&FK_MapData=<%=this.FK_MapData %>&Idx=&GroupField=<%=this.GroupField %>'><b>枚举型</b></a> -  比如：性别:男/女。请假类型：事假/病假/婚假/产假/其它。</li> 	
<li> <a href="SysEnum.aspx?DoType=New&FK_MapData=<%=this.FK_MapData  %>&Idx=&GroupField=<%=this.GroupField %>"  >新建枚举 </a> </li>
</ul>	

 </td>
</tr>


<tr>
<th colspan="2"> 新增下拉框(外键、外部表、WebServices)字段(通常只有编号名称两个列)</th>
</tr>

<tr>
<td colspan="2" >
<ul>
<li><a href='SFList.aspx?DoType=AddSFTable&FK_MapData=<%=this.FK_MapData %>&FType=Class&Idx=&GroupField=<%=this.GroupField %>'><b>外键、外部数据、webservices数据类型</b></a> -  比如：岗位、税种、行业、科目，本机上一个表组成一个下拉框。<a href='SFTable.aspx?DoType=New&FK_MapData=<%=this.FK_MapData %>&GroupField=<%=this.GroupField %>'>新建表</a></li> 	

<li><a href='SFList.aspx?DoType=AddSFTable&FK_MapData=<%=this.FK_MapData %>&FType=Class&Idx=&GroupField=<%=this.GroupField %>'><b>外键型</b></a> -  比如：岗位、税种、行业、科目，本机上一个表组成一个下拉框。<a href='SFTable.aspx?DoType=New&FK_MapData=<%=this.FK_MapData %>&GroupField=<%=this.GroupField %>'>新建表</a></li> 	
<li><a href='SQLList.aspx?FK_MapData=<%=this.FK_MapData %>&GroupField=<%=this.GroupField %>'><b>外部表</b></a> -  比如：配置一个SQL通过数据库连接或获取的外部数据，组成一个下拉框。</li> 	
<li><a href='Do.aspx?DoType=AddSFWebServeces&MyPK=ND17501&FType=Class&Idx=&GroupField='><b>WebServices</b></a> -  比如：通过调用Webservices接口获得数据，组成一个下拉框。</li> 	
</ul>
</td>
</tr>

<tr>
<th colspan="2"> 从已有表里导入字段</th>
</tr>

<tr>
<td colspan="2">

<ul>
<li><a href="javascript:WinOpen('ImpTableField.aspx?FK_MapData=<%=this.FK_MapData %>&FType=Class&Idx=&GroupField=<%=this.GroupField %>');" ><b>导入字段</b></a>
 &nbsp;&nbsp;从现有的表中导入字段,以提高开发的速度与字段拼写的正确性.</li> 	
</ul>	

 </td>
</tr>


<tr>
<th colspan="2"> 增加系统字段-隐藏/显示</th>
</tr>

<tr>
<td colspan="2">

><fieldset width='100%' ><legend>&nbsp;<div onclick="javascript:HidShowSysFieldImp();" >增加系统字段-隐藏/显示</div> &nbsp;</legend><div id='SysField' style='display:none' >        /// <summary>
<BR>        /// 发送人员字段
<BR>        /// 用在节点发送时确定下一个节点接受人员, 类似与发送邮件来选择接受人.
<BR>        /// 并且在下一个节点属性的 访问规则中选择【按表单SysSendEmps字段计算】有效。
<BR>        /// </summary>
<BR>        public const string SysSendEmps = "SysSendEmps";
<BR>        /// <summary>
<BR>        /// 抄送人员字段
<BR>        /// 当前的工作需要抄送时, 就需要在当前节点表单中，增加此字段。
<BR>        /// 并且在节点属性的抄送规则中选择【按表单SysCCEmps字段计算】有效。
<BR>        /// 如果有多个操作人员，字段的接受值用逗号分开。比如: zhangsan,lisi,wangwu
<BR>        /// </summary>
<BR>        public const string SysCCEmps = "SysCCEmps";
<BR>        /// <summary>
<BR>        /// 流程应完成日期
<BR>        /// 说明：在开始节点表单中增加此字段，用来标记此流程应当完成的日期.
<BR>        /// 用户在发送后就会把此值记录在WF_GenerWorkFlow 的 SDTOfFlow 中.
<BR>        /// 此字段显示在待办，发起，在途，删除，挂起列表里.
<BR>        /// </summary>
<BR>        public const string SysSDTOfFlow = "SysSDTOfFlow";
<BR>        /// <summary>
<BR>        /// 节点应完成时间
<BR>        /// 说明：在开始节点表单中增加此字段，用来标记此节点的下一个节点应该完成的日期.
<BR>        /// </summary>
<BR>        public const string SysSDTOfNode = "SysSDTOfNode";</div></fieldset>
      



 </td>
</tr>



</table>
</asp:Content>
