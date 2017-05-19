<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintOffice.aspx.cs" Inherits="CCFlow.WF.WebOffice.PrintOffice" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>打印预览</title>
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jBox/jquery.jBox-2.3.min.js" type="text/javascript"></script>
    <link href="../Scripts/jBox/Skins/Blue/jbox.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/QueryString.js" type="text/javascript"></script>
    <style type="text/css">
        .btn
        {
            border: 0;
            background: #4D77A7;
            color: #FFF;
            font-size: 12px;
            padding: 6px 10px;
            margin: 5px;
        }
    </style>
    <script type="text/javascript">
        var webOffice = null;
        var isOpen = false;
        var marksType = "doc,docx";

        $(function () {
            webOffice = document.all.WebOffice1;
            pageLoadding('正在打开模板...');
            try {
                var myPK = GetQueryString("MyPK");
                alert(myPK);
                var url = location.href + "&action=LoadFile";
                var fileExt = "doc";
                var openType = webOffice.LoadOriginalFile(url, fileExt);
                if (openType > 0) {
                    SetTrack(0);
                    isOpen = true;
                    loaddingOut('打开完成。');
                } else {
                    $.jBox.tip('');
                    $.jBox.alert('打开文档失败', '异常');
                }
            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        });

        //设置留痕,显示所有的留痕用户,是否只读文档
        function SetTrack(track) {
            webOffice.SetTrackRevisions(track);
        }

        //打印
        function printOffice() {
            try {
                if (isOpen) {
                    webOffice.PrintDoc(1);
                } else {
                    $.jBox.alert('请打开公文!', '提示');
                }
            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
            return false;
        }

        function pageLoadding(msg) {
            $.jBox.tip(msg, 'loading');
        }

        function loaddingOut(msg) {
            $.jBox.tip(msg, 'success');

        }
                
        function closeDoc() {
            webOffice.SetCurrUserName("");
            webOffice.closeDoc(0);
        }
    </script>
</head>
<body onunload="closeDoc()" style="padding: 0px; margin: 0px;" class="easyui-layout">
    <div data-options="region:'center',border:false,noheader:true" >
        <script src="../Scripts/LoadInAsp.js" type="text/javascript"></script>
    </div>
</body>
</html>
