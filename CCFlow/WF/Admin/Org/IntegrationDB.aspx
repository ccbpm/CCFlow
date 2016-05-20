<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/Org/Site.Master" AutoEventWireup="true" CodeBehind="IntegrationDB.aspx.cs" Inherits="CCFlow.WF.Admin.Org.IntegrationDB" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<h3>步骤1：设置组织机构模式</h3>
<hr>
<asp:RadioButton ID="RB0" runat="server" GroupName="xxx" Text="一个用户一个部门模式。" />
<asp:RadioButton ID="RB1" runat="server"  GroupName="xxx" Text="一个用户多个部门模式。" />



<h3>步骤2：组织结构维护方式</h3>
<hr>
<asp:RadioButton ID="RadioButton1" runat="server" GroupName="xxx" Text="由CCBPM组织结构维护。" />
<asp:RadioButton ID="RadioButton2" runat="server"  GroupName="xxx" Text="集成我自己开发框架下的组织结构或者现在已有系统的组织结构。" />




<h3>步骤3：选择组织结构来源</h3>
<hr>
<asp:RadioButton ID="RadioButton3" runat="server" GroupName="xxx" Text="使用数据源直接连接。" />
<asp:RadioButton ID="RadioButton4" runat="server"  GroupName="xxx" Text="使用WebServces模式。" />
<asp:RadioButton ID="RadioButton5" runat="server"  GroupName="xxx" Text="使用AD。" />



<fieldset>
<legend>帮助</legend>
<ul>
<li> 第2中模式已经包含了第1种模式的需求。</li>
<li> 第1中模式更方便集成，并且容易理解，权限组织结构简单，第1种模式可以随着业务发展的需要切换到第2中模式上去。

    </li>
</ul>
</fieldset>

   <h3>选择数据源：
   <asp:DropDownList ID="DropDownList1" runat="server">
       </asp:DropDownList>
    </h3>

<h3>步骤4：配置查询语句</h3>
<hr>
   <hr/>
   <fieldset>
   <legend>部门类型:</legend>
   说明：必须有No,Name列
   <input type=text  style=" width:100%;Height:40px" />
           <input type="button"  value="检查正确性"  />
           <input type="button"  value="打开数据源"  />
   </fieldset>

   
   <fieldset>
   <legend>部门集成:</legend>
   说明：您需要填写一个查询语句能够获取到
   No , Name,Parent 列.并且跟目录的ParentNo必须是0.
   <input type=text  style=" width:100%;Height:40px" />
           <input type="button"  value="检查正确性"  />
           <input type="button"  value="打开数据源"  />
   </fieldset>


     <fieldset>
   <legend>岗位类型:</legend>
   说明：必须有No,Name列
   <input type=text   style=" width:100%;Height:40px" />
           <input type="button"  value="检查正确性"  />
           <input type="button"  value="打开数据源"  />
   </fieldset>


   <fieldset>
   <legend>岗位集成:</legend>
   说明：您需要填写一个查询语句能够获取到No , Name,FK_StationType 列. 
   <input type=text  style=" width:100%;Height:40px" />
           <input type="button"  value="检查正确性"  />
           <input type="button"  value="打开数据源"  />
   </fieldset>


   
   <fieldset>
   <legend>职务集成:</legend>
   说明：. 
   <input type=text  style=" width:100%;Height:40px" />
           <input type="button"  value="检查正确性"  />
           <input type="button"  value="打开数据源"  />
   </fieldset>


   
   <fieldset>
   <legend>用户集成:</legend>
   说明：您需要填写一个查询语句能够获取到
   No , Name,FK_Dept,Email,SID 列.并且必须包含一个admin帐号作为系统管理员.
   例如： SELECT UserNo as No
   <input type=text  style=" width:100%;Height:40px" />
           <input type="button"  value="检查正确性"  />
           <input type="button"   value="打开数据源"  />
   </fieldset>


   <fieldset>
   <legend>人员与部门的集成:</legend>
   说明：您需要填写一个查询语句能够获取到 MyPK, FK_Emp, FK_Dept 列. MyPK=FK_Emp+'_'+FK_Dept
   <input type=text  style=" width:100%;Height:40px" />
           <input type="button"  value="检查正确性"  />
           <input type="button"  value="打开数据源"  />
   </fieldset>


   
   <fieldset>
   <legend>人员与岗位集成:</legend>
   说明：您需要填写一个查询语句能够获取到 MyPK, FK_Emp, FK_Dept, FK_Station 列. MyPK=FK_Emp+'_'+FK_Dept+'_'+FK_Staion
   <input type=text  style=" width:100%;Height:40px" />
           <input type="button"  value="检查正确性"  />
           <input type="button"  value="打开数据源"  />
   </fieldset>

   <hr>

   
   <div style=" float:right">
           <input type="button"  value="设置全部"  />
           </div>


  
       
</asp:Content>
