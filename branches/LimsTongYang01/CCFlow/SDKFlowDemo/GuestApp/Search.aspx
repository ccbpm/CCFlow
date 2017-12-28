<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/GuestApp/Site.Master"
    AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="CCFlow.SDKFlowDemo.GuestApp.Search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/info.css" rel="stylesheet" type="text/css" />
    <link href="../../DataUser/Style/Table0.css" rel="stylesheet" type="text/css" />
     <script type="text/javascript">
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="info" class="easyui-layout">
        <div class="info_cen" data-options="region:'center'">
            <div class="info_cen_div">
                <a id="print" class="info_print_a" href="javascript:info_print()">打&nbsp;印</a>
            </div>
            <div class="info_cen_sign">
               流程查询：
            </div>
            <div class="info_cen_content">
                <div id="printDiv">
                  
                </div>
            </div>
        </div>
    </div>
</asp:Content>
