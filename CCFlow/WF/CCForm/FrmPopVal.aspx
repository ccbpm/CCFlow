<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Web.CCForm.WF_FrmPopVal" Codebehind="FrmPopVal.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    /* 设置选框 cb1.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')"; */
    function SetSelected(cb, ids) {
        //alert(ids);
        var arrmp = ids.split(',');
        var arrObj = document.all;
        var isCheck = false;
        if (cb.checked)
            isCheck = true;
        else
            isCheck = false;
        for (var i = 0; i < arrObj.length; i++) {
            if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                for (var idx = 0; idx <= arrmp.length; idx++) {
                    if (arrmp[idx] == '')
                        continue;
                    var cid = arrObj[i].name + ',';
                    var ctmp = arrmp[idx] + ',';
                    if (cid.indexOf(ctmp) > 1) {
                        arrObj[i].checked = isCheck;
                        //                    alert(arrObj[i].name + ' is checked ');
                        //                    alert(cid + ctmp);
                    }
                }
            }
        }
    }
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
    <asp:TreeView ID="TreeView1" runat="server" ImageSet="BulletedList2" 
        ShowExpandCollapse="False">
        <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
        <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" 
            HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
        <ParentNodeStyle Font-Bold="False" />
        <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" 
            HorizontalPadding="0px" VerticalPadding="0px" />
    </asp:TreeView>
</asp:Content>