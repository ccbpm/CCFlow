<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" CodeBehind="ImgAth.aspx.cs"
    Inherits="CCFlow.WF.CCForm.ImgAth" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="ImgAth/js/jquery1.4.2.min.js" type="text/javascript"></script>
    <script src="ImgAth/js/jquery.bitmapcutter.js" type="text/javascript"></script>
    <link href="ImgAth/css/jquery.bitmapcutter.css" rel="stylesheet" type="text/css" />
    <script src="ImgAth/ajaxfileupload.js" type="text/javascript"></script>
    <script type="text/javascript">
        //关闭窗体
        function closeWin() {
            window.close();
        }

        function ajaxFileUpload() {
            var ImgAth = "<%=ImgAths %>";
            var MyPK = "<%=MyPK %>";

            $.ajaxFileUpload({
                url: 'FileUpload.ashx?ImgAth=' + ImgAth + '&MyPK=' + MyPK,
                secureuri: false,
                type:"post",
                fileElementId: 'fileToUpload',
                dataType: 'json',
                success: function (data, status) {
                    if (typeof (data.error) != 'undefined') {
                        if (data.error != '') {
                            alert(data.error);
                        }
                        else {
                            var cutW = document.getElementById("cutW").value;
                            var cutH = document.getElementById("cutH").value;
                            document.getElementById('container').innerHTML = '';
                            ImageCut(data.msg, cutW, cutH);
                            document.getElementById('ContentPlaceHolder1_sourceImg').value = data.msg;
                            document.getElementById('ContentPlaceHolder1_newImgUrl').value = data.msg;
                            var btn = document.getElementById("ContentPlaceHolder1_refresh");
                            if (btn) {
                                btn.click();
                            }
                        }
                    }
                },
                error: function (data, status, e) {
                    alert(e);
                }
            });
            return false;
        }
        //照片裁剪代码
        function ImageCut(src, width, height) {
            $("#ContentPlaceHolder1_refresh").hide();
            var cutW = 320;
            var cutH = 220;
            if (width) {
                cutW = parseFloat(width);
            }
            if (height) {
                cutH = parseFloat(height);
            }

            $("#container").html('');
            src = src.replace(/http\:\/\/([\w\W]*?)\//g, "");
//            $.fn.bitmapCutter({
//                src: src,
//                renderTo: '#container',
//                holderSize: { width: 420, height: 400 },
//                cutterSize: { width: cutW, height: cutH },
//                onGenerated: function (newSrc) {//裁完并保存后返回保存后图片地址
//                    document.getElementById('ContentPlaceHolder1_newImgUrl').value = newSrc;
//                },
//                rotateAngle: 90,
//                lang: { clockwise: '顺时针旋转{0}度.' }
//            });
        }
        function imageSave() {
            document.getElementById("<%=btnImageSave.ClientID %>").click();
        }
        
    </script>
    <style type="text/css">
        img
        {
            border: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table>
        <tr>
            <td>
                <div style="font-size: 12px; margin-left: 0px; margin-right: 0px;">
                    &nbsp&nbsp&nbsp; 上传图片:<%--<input id="fileToUpload" name="fileToUpload" type="file" runat="server" onchange="ajaxFileUpload();" />--%>
                    <asp:FileUpload ID="fileToUpload" runat="server" onchange="imageSave()" />
                    <asp:Button ID="btnSubmit" class="Btn" runat="server" Text="保 存" OnClick="btnSubmit_Click" />
                    <asp:Button ID="btnImageSave" class="Btn" runat="server" Text="保 存" style="display:none;" OnClick="btnImaeSave_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="Btn" value="关闭" onclick="closeWin()" />
                    <asp:Button ID="refresh" Text="刷新" class="Btn" Width="0" Height="0" runat="server"
                        OnClick="refresh_Click" />
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div id="container">
                </div>
            </td>
        </tr>
    </table>
    <input type="hidden" id="cutW" />
    <input type="hidden" id="cutH" />
    <input type="hidden" id="sourceImg" runat="server" />
    <input type="hidden" id="newImgUrl" runat="server" />
</asp:Content>
