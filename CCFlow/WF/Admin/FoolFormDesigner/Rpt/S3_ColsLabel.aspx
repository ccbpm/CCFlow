<%@ Page Title="3. 设置报表显示列次序" Language="C#" MasterPageFile="RptGuide.Master" AutoEventWireup="true"
    CodeBehind="S3_ColsLabel.aspx.cs" Inherits="CCFlow.WF.MapDef.Rpt.ColsLabel" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("table tr:gt(0)").hover(
                function () { $(this).addClass("tr_hover"); },
                function () { $(this).removeClass("tr_hover"); });
        });

        //上移
        function up(obj, idxTBColumnIdx) {
            var objParentTR = $(obj).parent().parent();
            var prevTR = objParentTR.prev();
            var currTrId;
            var prevTrId;
            if (prevTR.length > 0 && !isNaN(prevTR.children(":eq(0)").text())) {
                prevTR.insertAfter(objParentTR);
                currTrId = Number(objParentTR.children(":eq(0)").text());
                prevTrId = Number(prevTR.children(":eq(0)").text());
                objParentTR.children(":eq(0)").text(prevTrId);
                prevTR.children(":eq(0)").text(currTrId);
                objParentTR.children(":eq(" + idxTBColumnIdx + ")").children(":first").val(prevTrId);
                prevTR.children(":eq(" + idxTBColumnIdx + ")").children(":first").val(currTrId);
            } else {
                return;
            }
        }
        //下移
        function down(obj, idxTBColumnIdx) {
            var objParentTR = $(obj).parent().parent();
            var nextTR = objParentTR.next();
            var currTrId;
            var nextTrId;
            if (nextTR.length > 0 && !isNaN(nextTR.children(":eq(0)").text())) {
                nextTR.insertBefore(objParentTR);
                currTrId = Number(objParentTR.children(":eq(0)").text());
                nextTrId = Number(nextTR.children(":eq(0)").text());
                objParentTR.children(":eq(0)").text(nextTrId);
                nextTR.children(":eq(0)").text(currTrId);
                objParentTR.children(":eq(" + idxTBColumnIdx + ")").children(":first").val(nextTrId);
                nextTR.children(":eq(" + idxTBColumnIdx + ")").children(":first").val(currTrId);
            } else {
                return;
            }
        }
    </script>
    <base target="_self" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub2" runat="server" />
    <br />
    <cc1:LinkBtn ID="Btn_Save1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-save'"
        Text="保存" OnClick="Btn_Save_Click" />
    <cc1:LinkBtn ID="Btn_SaveAndNext1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-save'"
        Text="保存并设置查询条件" OnClick="Btn_SaveAndNext1_Click" />
    <cc1:LinkBtn ID="Btn_Cancel1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-undo'"
        Text="取消" OnClick="Btn_Cancel_Click" />

    <br />
    <br />

</asp:Content>
