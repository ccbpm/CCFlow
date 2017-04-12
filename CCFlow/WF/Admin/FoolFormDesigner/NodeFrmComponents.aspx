<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="NodeFrmComponents.aspx.cs" Inherits="CCFlow.WF.MapDef.NodeFrmComponents" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<% if (this.Request.QueryString["DoType"] == "FWC")
   { %>

   <table style="width:100%; height:100%;">
   <tr>
   <td >部门<br>经理<br>审核</td>
   <td >  同意 </td>
   </tr>

   <tr>
   <td>&nbsp;</td>
   <td>审核人：xxxxxx <div style=" float:right"> 审核时间：xxxx年xx月xx日</div> </td>
   </tr>


    <tr>
   <td >财务<br>经理<br>审核</td>
   <td >  票据合法，同意。 </td>
   </tr>

   <tr>
   <td>&nbsp;</td>
   <td>审核人：xxxxxx <div style=" float:right"> 审核时间：xxxx年xx月xx日</div> </td>
   </tr>
   
   <tr>
   <td >总经理<br>审核</td>
   <td >  同意 </td>
   </tr>

   <tr>
   <td>&nbsp;</td>
   <td>审核人：xxxxxx  <div style=" float:right"> 审核时间：xxxx年xx月xx日</div></td>
   </tr>

   </table>
    
<%} %>


<% if (this.Request.QueryString["DoType"] == "SubFlow")
   { %>


   <table  style="width:100%; height:100%;">
   <tr>
   <th>流程</th>
   <th>标题</th>
   <th>启动日期</th>
   <th>状态</th>
   <th>运行到节点</th>
   <th>当前处理人</th>
   </tr>

   <tr>
   <td>xx子流程</td>
   <td>xxxxxxxx</td>
   <td>xxxx年xx月xx日xx时</td>
   <td>运行中</td>
   <td>部门经理审批</td>
   <td>张三</td>
   </tr>


   <tr>
   <td>xx子流程</td>
   <td>xxxxxxxx</td>
   <td>xxxx年xx月xx日xx时</td>
   <td>已结束</td>
   <td>人力资源备案</td>
   <td>李四</td>
   </tr>


   
   <tr>
   <td>xx子流程</td>
   <td>xxxxxxxx</td>
   <td>xxxx年xx月xx日xx时</td>
   <td>已结束</td>
   <td>人力资源备案</td>
   <td>王五</td>
   </tr>

   </table>
    

<%} %>


<% if (this.Request.QueryString["DoType"] == "FrmThread")
   { %>
   <table  style="width:100%; height:100%;">
   <tr>
   <th>标题</th>
   <th>当前处理人</th>
   <th>运行到节点</th>
   <th>状态</th>
   <th>操作</th>
   </tr>

   <tr>
   <td>xxxxxxxx</td>
   <td>张三</td>
   <td>填写报告</td>
   <td>运行中</td>
   <td>删除</td>
   </tr>

    <tr>
   <td>xxxxxxxx</td>
   <td>李四</td>
   <td>填写报告</td>
   <td>已经完成</td>
   <td>删除</td>
   </tr>


    <tr>
   <td>xxxxxxxx</td>
   <td>王五</td>
   <td>填写报告</td>
   <td>运行中</td>
   <td>删除</td>
   </tr>


 <tr>
   <td>xxxxxxxx</td>
   <td>赵六</td>
   <td>填写报告</td>
   <td>已经完成</td>
   <td>删除</td>
   </tr>

   
 <tr>
   <td>xxxxxxxx</td>
   <td>甲乙丙</td>
   <td>填写报告</td>
   <td>已经完成</td>
   <td>删除</td>
   </tr>

   </table>


<%} %>


<% if (this.Request.QueryString["DoType"] == "FrmTrack")
   { %>
   
   <img src='./Img/Track.png' style=" width:100%; height:100%" />

<%} %>


<% if (this.Request.QueryString["DoType"] == "FrmFTC")
   { %>
   <table  style="width:100%; height:100%;">

    <tr>
   <td>节点步骤</td>
   <td> 处理人</td>
   <td> 计划完成日期 </td>
   </tr>


   <tr>
   <td>节点1</td>
   <td> 张三 </td>
   <td> xxxx年xx月xx日 </td>
   </tr>
     
     
   <tr>
   <td>节点2</td>
   <td> 李四 </td>
   <td> xxxx年xx月xx日 </td>
   </tr>
   
     
   <tr>
   <td>节点3</td>
   <td> 王五 </td>
   <td> xxxx年xx月xx日 </td>
   </tr>

       
   <tr>
   <td>节点4</td>
   <td> 赵六 </td>
   <td> xxxx年xx月xx日 </td>
   </tr>

        
   <tr>
   <td>节点5</td>
   <td> 张思 </td>
   <td> xxxx年xx月xx日 </td>
   </tr>

    <tr>
   <td>节点6</td>
   <td> 孙倩 </td>
   <td> xxxx年xx月xx日 </td>
   </tr>

     <tr>
   <td>节点7</td>
   <td> 姚家 </td>
   <td> xxxx年xx月xx日 </td>
   </tr>

   </table>

<%} %>


</asp:Content>
