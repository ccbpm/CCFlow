<%@ Page Title="表单权限" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" CodeBehind="Sln.aspx.cs" Inherits="CCFlow.WF.MapDef.Sln" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript">
    function DelSln(fk_mapdata,fk_flow, fk_node, keyofen) {
        var url = 'SlnDo.aspx?DoType=DelSln&FK_MapData=' + fk_mapdata + '&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&KeyOfEn=' + keyofen;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function EditSln(fk_mapdata,fk_flow,fk_node, keyofen) {
        var url = 'SlnDo.aspx?DoType=EditSln&FK_MapData=' + fk_mapdata + '&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&KeyOfEn=' + keyofen;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function SetSln(fk_mapdata) {
        var url = 'EditMapData.aspx?DoType=EditSln&FK_MapData=' + fk_mapdata ;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function CopyIt(fk_mapdata, fk_flow, nodeID) {
        var myurl = 'Sln.aspx?DoType=Copy&FK_MapData=' + fk_mapdata + '&FK_Flow=' + fk_flow + '&FK_Node=' + nodeID;
       window.location.href = myurl;
    }

    var IsNoteNull=false;
    function CheckAll(idstr) {
        var arrObj = document.all;
        IsNoteNull = !IsNoteNull;
        for (var i = 0; i < arrObj.length; i++) {

            if (arrObj[i].type != 'checkbox')
                continue;

            var cid = arrObj[i].name;
            if (cid == null || cid == "" || cid == '')
                continue;

            if (cid.indexOf(idstr) == -1)
                continue;
            arrObj[i].checked = IsNoteNull;
            //  !arrObj[i].checked;
        }
    }

    var IsEnable = false;
    function CheckAllIsEnable(idstr) {
        var arrObj = document.all;
        IsEnable = !IsEnable;
        for (var i = 0; i < arrObj.length; i++) {

            if (arrObj[i].type != 'checkbox')
                continue;

            var cid = arrObj[i].name;
            if (cid == null || cid == "" || cid == '')
                continue;

            if (cid.indexOf(idstr) == -1)
                continue;

            arrObj[i].checked = IsEnable;
            //  !arrObj[i].checked;
        }
    }

    //编辑附件的原始属性.
    function EditFJYuanShi(fk_mapdata, ath) {
        var url = 'Attachment.aspx?FK_MapData=' + fk_mapdata + '&Ath=' + ath;
        WinShowModalDialog(url, 'ss');
        window.location.href = window.location.href;
    }

    //编辑附件在该节点权限.
    function EditFJ(fk_node, fk_mapdata, ath) {
        var url = 'Attachment.aspx?FK_Node=' + fk_node + '&FK_MapData=' + fk_mapdata + '&Ath=' + ath;
        WinShowModalDialog(url, 'ss');
        window.location.href = window.location.href;
    }

    //删除附件的权限控制.
    function DeleteFJ(fk_node, fk_mapdata, ath) {

        if (confirm('您确定要删除该控件在当前节点的权限控制吗？') == false)
            return;
        var url = 'Sln.aspx?DoType=DeleteFJ&FK_Node=' + fk_node + '&FK_MapData=' + fk_mapdata + '&Ath=' + ath;
        alert(url);
        WinShowModalDialog(url, 'ss');
        window.location.href = window.location.href;
    }


    //编辑Dtl的原始属性.
    function EditDtlYuanShi(fk_mapdata, dtlKey) {
        var url = 'MapDefDtlFreeFrm.aspx?DoType=Edit&FK_MapData=' + fk_mapdata + '&FK_MapDtl=' + dtlKey + '&DoType=Edit';
        //  var b = window.showModalDialog(url, 'ass', 'dialogHeight: 600px; dialogWidth: 700px;center: yes; help:no;resizable:yes');
        WinOpen(url,'ss');
        //window.location.href = url; //  window.location.href;
    }

    //编辑附件在该节点权限.
    function EditDtl(fk_node, fk_mapdata, dtlNo) {
        var url = 'MapDefDtlFreeFrm.aspx?DoType=Edit&FK_MapData=' + fk_mapdata + '&FK_MapDtl=' + dtlNo + '&FK_Node=' + fk_node;
    //    WinOpen(url,"dtl");
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 600px; dialogWidth: 700px;center: yes; help:no;resizable:yes');
        window.location.href = window.location.href;
    }

    //删除附件的权限控制.
    function DeleteDtl(fk_node, fk_mapdata, dtl) {

        if (confirm('您确定要删除该控件在当前节点的权限控制吗？') == false)
            return;
        var url = 'Sln.aspx?DoType=DeleteDtl&FK_Node=' + fk_node + '&FK_MapData=' + fk_mapdata + '&FK_MapDtl=' + dtl;
        alert(url);
        WinShowModalDialog(url, 'ss');
        window.location.href = window.location.href;
    }

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<h3>
<div style=" float:left;"> 
权限控制类型： <a href="?FK_MapData=<%=this.FK_MapData %>&FK_Node=<%=this.FK_Node %>&DoType=Field&FK_Flow=<%=this.FK_Flow %>">字段</a> 
 -  <a href="?FK_MapData=<%=this.FK_MapData %>&FK_Node=<%=this.FK_Node %>&DoType=FJ&FK_Flow=<%=this.FK_Flow %>">附件</a> 
 -  <a href="?FK_MapData=<%=this.FK_MapData %>&FK_Node=<%=this.FK_Node %>&DoType=Dtl&FK_Flow=<%=this.FK_Flow %>">明细表</a> 
 </div>
 <%
            string url = "<a href=\"Sln.aspx?DoType=Copy&FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "\" ><img src='/WF/Img/Btn/Copy.gif' border=0 />从其他节点Copy权限设置</a>";
             %>
             <div  style=" float:right"><%=url %></div>

             <br />
 </h3>

    <uc1:Pub ID="Pub2" runat="server" />

</asp:Content>
