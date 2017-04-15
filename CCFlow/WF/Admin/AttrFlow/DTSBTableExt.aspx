<%@ Page Title="设置同步字段的对应" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="DTSBTableExt.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.DTSBTableExt" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        table
        {
            margin: 90px;
        }
        div
        {
            align: center;
        }
        
.CaptionMsg
{
    vertical-align: middle;
    text-align: left;
    height: 32px;
    background-position: left center;
    line-height: 34px;
    font-size: 12px;
    text-align: center;
    vertical-align: middle;
    height: 30px;
    font-family: 黑体;
    font-weight: bolder;
    color: #FFFFFF;
    background: url('/DataUser/Style/Img/TitleMsg.png');
    width: 138px;
    margin-left: -10px;
    padding-left: 0px;
    background-repeat: no-repeat;
    background-attachment: fixed;
}


.CaptionMsg  a:hover
{
    color: white;
}



.CaptionMsgLong
{
    vertical-align: middle;
    text-align: left;
    height: 32px;
    background-position: left center;
    line-height: 34px;
    font-size: 12px;
    text-align: center;
    vertical-align: middle;
    height: 30px;
    font-family: 黑体;
    font-weight: bolder;
    color: #FFFFFF;
    background: url('../../../DataUser/Style/Img/TitleMsgLong.png');
    width: 238px;
    margin-left: -10px;
    padding-left: 0px;
    background-repeat: no-repeat;
    background-attachment: fixed;
    text-align:center;
}
    </style>
    <script type="text/javascript">
        function winClose() {
            window.close();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            font-family: '宋体' !important;
        }
        table
        {
            margin: 0 auto;
            width: 100%;
        }
        #ContentPlaceHolder1_Pub1_Btn_Save
        {
            margin: 0 29%;
        }
        #ContentPlaceHolder1_Pub1_Btn_Close
        {
        }
    </style>
</asp:Content>

