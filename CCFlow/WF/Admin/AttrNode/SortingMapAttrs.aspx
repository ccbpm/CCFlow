<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SortingMapAttrs.aspx.cs" Inherits="CCFlow.WF.Admin.SortingMapAttrs" %>
<%@ Register Src="../Pub.ascx" TagName="uc1" TagPrefix="Pub" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>手机屏幕显示字段排序</title>
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>

    <script type="text/javascript" src="../../Scripts/config.js"></script>
    <script src="../../Comm/Gener.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

        function ShowEditWindow(field, url) {

            if (!field || !url) {
                return;
            }

            OpenDialogAndCloseRefresh(url, "编辑字段：" + field, 700, 550, "icon-edit");
        }
        //新增分组名称
        function GroupFieldNew(ensName) {
            var url = '../FoolFormDesigner/GroupField.htm?DoType=NewGroup&FK_MapData=' + ensName + "&inlayer=1";
            OpenDialogAndCloseRefresh(url, "新建分组", 600, 550, "icon-new");
        }
        //编辑分组名称
        function GroupField(fk_mapdata, OID) {
            var url = '../FoolFormDesigner/GroupField.htm?FK_MapData=' + fk_mapdata + "&GroupField=" + OID + "&inlayer=1";
            OpenDialogAndCloseRefresh(url, "编辑分组", 600, 550, "icon-edit");
        }
        //.net保存并关闭层所用函数
        function closeDlg() {
            $('#eudlg').dialog("close");
        }
        //编辑明细表
        function EditDtl(mypk, dtlKey) {
            //var url = '../FoolFormDesigner/MapDtl.aspx?DoType=Edit&FK_MapData=' + mypk + '&FK_MapDtl=' + dtlKey;
            var url = "/WF/Comm/En.htm?EnsName=BP.WF.Template.MapDtlExts&PK=" + dtlKey;

            OpenDialogAndCloseRefresh(url, "编辑明细表", 720, 550, "icon-edit");
        }
        //编辑多附件
        function EditAthMent(fk_mapdata, athMentKey) {
            var url = '../FoolFormDesigner/Attachment.aspx?FK_MapData=' + fk_mapdata + '&Ath=' + athMentKey;
            OpenDialogAndCloseRefresh(url, "编辑多附件", 720, 550, "icon-edit");
        }
        //预览手机端
        function Form_View(FK_MapData, FK_Flow) {
            var url = '../../../CCMobile/Frm.aspx?FK_MapData=' + FK_MapData + '&IsTest=1&WorkID=0&FK_Node=999999';
            OpenDialogAndCloseRefresh(url, "预览手机端表单", 600, 550, "icon-edit", function () { });
        }
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
        <div data-options="region:'center',title:'字段排序',border:false" style="padding: 5px">
            <Pub:uc1 ID="pub1" runat="server" />
            <asp:HiddenField ID="hidCopyNodes" runat="server" />
        </div>
    </form>
</body>
</html>
