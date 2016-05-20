<%@ Page Language="C#" MasterPageFile="../SDKComponents/Site.master" AutoEventWireup="true"
 Inherits="CCFlow.WF.WF_AllotTask" Title="分配工作" Codebehind="AllotTask.aspx.cs" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="javascript" type="text/javascript" >
    function RSize() {
        if (document.body.scrollWidth > (window.screen.availWidth - 100)) {
            window.dialogWidth = (window.screen.availWidth - 100).toString() + "px"
        } else {
            window.dialogWidth = (document.body.scrollWidth + 50).toString() + "px"
        }

        if (document.body.scrollHeight > (window.screen.availHeight - 70)) {
            window.dialogHeight = (window.screen.availHeight - 50).toString() + "px"
        } else {
            window.dialogHeight = (document.body.scrollHeight + 115).toString() + "px"
        }

        window.dialogLeft = ((window.screen.availWidth - document.body.clientWidth) / 2).toString() + "px"
        window.dialogTop = ((window.screen.availHeight - document.body.clientHeight) / 2).toString() + "px"
    }
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
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
 
    <table style=" text-align:left; width:100%">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %>

        </caption>
<tr>
<td style=" text-align:center">
<uc1:Pub 
        ID="Pub1" runat="server" />
 </td>
</tr>
</table>

</asp:Content>

