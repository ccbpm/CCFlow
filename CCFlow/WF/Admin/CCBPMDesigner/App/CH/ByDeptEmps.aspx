<%@ Page Title="单个人员的信息" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="ByDeptEmps.aspx.cs" Inherits="CCFlow.WF.Admin.CCFlowDesigner.App.CH.ByDeptEmps" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<% 
    string empNo = this.Request.QueryString["FK_Emp"]; 
   
   %>

<table style="width: 100%;">
        <tr>
            <th class="GroupTitle" rowspan="2">
                IDX
            </th>
            <th rowspan="2">
                人员
            </th>
            <th rowspan="2">
                流程名称
            </th>
              <th rowspan="2">
                节点名称
            </th>
            
            <th colspan="2">
                时效统计(单位:分钟)
            </th>
            <th colspan="5">
                状态分布
            </th>
            <th colspan="3">
                上月
            </th>
            <th colspan="3">
                本月
            </th>
            <th rowspan="2">
                详细
            </th>
        </tr>
        <tr>
            <th>
                节省
            </th>
            <th>
                逾期
            </th>
            <th>
                及时
            </th>
            <th>
                按期
            </th>
            <th>
                逾期
            </th>
            <th>
                超期
            </th>
            <th>
                按期办结率
            </th>


            <th>按期</th>
            <th>预期</th>
            <th>按期办结率</th>


            <th>按期</th>
            <th>预期</th>
            <th>按期办结率</th>

        </tr>



        </table>

</asp:Content>
