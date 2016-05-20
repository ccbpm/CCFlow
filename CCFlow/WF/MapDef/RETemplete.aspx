<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    CodeBehind="RETemplete.aspx.cs" Inherits="CCFlow.WF.MapDef.RegularExpressionTemplete" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function DoIt(fk_mapdata, keyOfEn, forCtrl, reid,reName) {
            if (window.confirm('您确定清空当前的设置加载此表达式吗？') == false)
                return;
            document.getElementById("selectRe").value = reName;
            window.location.href = 'RETemplete.aspx?FK_MapData=' + fk_mapdata + '&KeyOfEn=' + keyOfEn + '&REID=' + reid + '&ForCtrl=' + forCtrl;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset style="width: 150px;">
        <legend>关于事件模版</legend>使用事件模版
        <ul>
            <li>能够帮助您快速的定义表单字段事件。</li>
        </ul>
    </fieldset>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
