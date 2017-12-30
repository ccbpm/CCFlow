<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" ValidateRequest="false" AutoEventWireup="true"
    Inherits="CCFlow.WF.Comm.RefFunc.UIEn" CodeBehind="UIEn.aspx.cs" %>

<%@ Register Src="../UC/UIEn.ascx" TagName="UIEn" TagPrefix="uc2" %>
<%@ Register Src="RefLeft.ascx" TagName="RefLeft" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="../../Scripts/config.js" type="text/javascript"></script>
	<script src="../Gener.js" type="text/javascript"></script>

    <script type="text/javascript">

		closeWhileEscUp();

        function selectTab(tabTitle) {
            $('#nav-tab').tabs('select', tabTitle);
        }

        $(document).ready(function () {
            if(<%=this.HiddenLeft.ToString().ToLower() %>){
                $('body').layout('remove','west');  //$('body').layout('collapse','west');
            }

            if(<%=this.HiddenTop.ToString().ToLower() %>){
                $('#uienright').layout('remove','north');
            }
            
            var tabts = $("#rightFrame a.tabs-inner");

            $.each(tabts, function (i) {
                $(this).attr('title', $("#rightFrame div[data-g='" + $(this).text() + "']").attr('data-gd'));
            });

            //选中上次保存之前当前打开的标签
            var urlParams = location.search.substr(1).split('&');
            $.each(urlParams, function () {
                var a = this.split('=');
                if (a[0] == 'tab') {
                    $('#nav-tab').tabs('select', decodeURIComponent(a[1]));
                    return;
                }
            });
        });
    </script>
    <base target="_self" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:RefLeft ID="RefLeft1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <uc2:UIEn ID="UIEn1" runat="server" />
</asp:Content>
