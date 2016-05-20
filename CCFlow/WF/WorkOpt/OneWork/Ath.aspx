<%@ Page Title="流程附件" Language="C#" MasterPageFile="OneWork.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.OneWork.WF_WorkOpt_OneWork_Ath" CodeBehind="Ath.aspx.cs" %>

<%@ Register Src="../../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'center'" style="padding: 5px">
            <uc1:Pub ID="Pub1" runat="server" />
        </div>
    </div>
</asp:Content>
