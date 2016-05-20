<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/GuestApp/Site.Master"
    AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="CCFlow.SDKFlowDemo.GuestApp.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/info.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="info" class="easyui-layout">
        <div class="info_cen" data-options="region:'center'">
            <div class="info_cen_div">
                <a id="print" class="info_print_a" href="javascript:info_print()">打&nbsp;印</a>
            </div>
            <div class="info_cen_sign">
                欢迎使用ccflow工作流引擎：
            </div>
            <div class="info_cen_content">
                <div id="printDiv">

                <ul style="clear" >
                <li>您所看到的该界面是一个外部用户登录系统的demo。 我们把组织解构表内部的用户称为内部用户，组织结构外的用户称呼为外部用户。 </li>
                <li> 在一个流程的工作是由外部用户与内部用户共同完成的，我们把这样的流程称为客户参与流程。</li>
                <li> 比如：我们为学校设计一个学生请假流程，分别是学生申请=》教师审批=》通知给学生=》教务处备案。整个流程中，学生就是外部用户，教师与教务处都是内部用户，该流程由内外部两种用户完成。</li>
                <li> 比如：在一个工厂里，内部员工是内部用户，供应商，客户，都是外部用户。 </li>
                <li> 所以，内部用户是在Port_*组织结构表的用户，可以有多个外部用户。  </li>
                </ul>
                 
                </div>
            </div>
        </div>
    </div>
</asp:Content>
