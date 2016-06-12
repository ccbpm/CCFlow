﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FilesView.aspx.cs" Inherits="CCFlow.WF.CCForm.FilesView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <base target="_self"" />
    <script type="text/javascript">
        function AutoResizeImage(maxWidth, maxHeight, objImg) {
            var img = new Image();
            img.src = objImg.src;
            var hRatio;
            var wRatio;
            var Ratio = 1;
            var w = img.width;
            var h = img.height;
            wRatio = maxWidth / w;
            hRatio = maxHeight / h;
            if (maxWidth == 0 && maxHeight == 0) {
                Ratio = 1;
            } else if (maxWidth == 0) {//
                if (hRatio < 1) Ratio = hRatio;
            } else if (maxHeight == 0) {
                if (wRatio < 1) Ratio = wRatio;
            } else if (wRatio < 1 || hRatio < 1) {
                Ratio = (wRatio <= hRatio ? wRatio : hRatio);
            }
            if (Ratio < 1) {
                w = w * Ratio;
                h = h * Ratio;
            }
            objImg.height = h;
            objImg.width = w;
        }
</script>

</head>
<body>
    <form id="form1" runat="server">
    <div  style=" text-align:center">
     <uc1:Pub ID="Pub1" runat="server" />
     </div>
    </form>
</body>
</html>
