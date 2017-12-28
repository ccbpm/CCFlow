<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PointOffice.aspx.cs" Inherits="CCFlow.WF.CCForm.PointOffice"
    ResponseEncoding="UTF-8" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/PointJs.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .divButton table
        {
            border-collapse: collapse;
            border: none;
        }
        .divButton td
        {
            width: 100px;
            float: left;
            height: 20px;
        }
    </style>
</head>
<body onload="InitOffice()">
    <form id="form1" runat="server">
    <div style="visibility: hidden; width: 0px; height: 0px">
        <input id="Button3" type="button" value="打开文件" onclick="OpenWeb()" style="width: 100px;"
            runat="server" visible="False" />
        <input type="text" value="" id="TB_User" runat="server" style="width: 100px;" />
        <input type="text" value="" id="TB_FileType" runat="server" style="width: 100px;" />
        <input type="text" value="" id="TB_FilePath" runat="server" style="width: 100px;" />
        <input type="text" value="" id="TB_IsReadOnly" runat="server" style="width: 100px;" />
            <input type="text" value="" id="TB_IsPrint" runat="server" style="width: 100px;" />
        <input class="easyui-combobox" id="CB_Flow" name="CB_Flow" style="width: 100px" />
        <input type="button" value="插入流程" onclick="WebWordDownFile()" style="width: 100px"
            id="Button1" runat="server" />
        <input type="file" name="TB_Image" id="TB_Image" style="width: 100px" />
        <input type="button" value="插入图片" onclick="InputFiles()" style="width: 100px" id="Button2"
            runat="server" />
        <input type="button" value="文档保护" onclick="ProtectDoc()" style="width: 100px;" visible="False"
            id="Btn_ProDoc" runat="server" />
        <input type="button" value="解除文档保护" onclick="UnPortectDoc()" style="width: 100px;"
            id="Btn_UnProDoc" runat="server" visible="False" />
        <input type="text" value="" id="TB_Track" runat="server" style="width: 100px;" />
    </div>
    <div class="divButton">
        <table style="width: 100%">
            <tr>
                <td>
                    <input type="button" value="接受当前修订" onclick="SaveTrack()" style="width: 100px" id="Btn_AttachDoc"
                        runat="server" />
                </td>
                <td>
                    <input type="button" value="拒绝当前修订" onclick="ReturnTrack()" style="width: 100px"
                        id="Btn_UnAttachDoc" runat="server" />
                </td>
                <td>
                    <input type="button" value="保存" onclick="SaveService()" style="width: 100px" id="Btn_Save"
                        runat="server" />
                </td>
                <td>
                    <select id="sShowName" onchange="ShowUserName()" style="width: 100px" runat="server">
                        <option value="全部">全部</option>
                    </select>
                </td>
                 <td>
                    <input type="button" value="电子签章" onclick="Signature('sealBig')" style="width: 100px" id="Button4"
                        runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <div>
        <table style="width: 100%">
            <tr>
                <td>
                    <object id=WebOffice1 height=586 width='100%' style='LEFT: 0px; TOP: 0px'  classid='clsid:E77E049B-23FC-4DB8-B756-60529A35FAD5' codebase='../Activex/WebOffice.cab#V7.0.0.8'>
                        <param name='_ExtentX' value='6350'>
                        <param name='_ExtentY' value='6350'>
                    </object>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
