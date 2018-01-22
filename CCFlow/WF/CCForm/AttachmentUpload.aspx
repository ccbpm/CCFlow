<%@ Page Title="上传文件" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.CCForm.WF_CCForm_AttachmentUpload" CodeBehind="AttachmentUpload.aspx.cs"  ResponseEncoding="utf-8"%>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script type="text/javascript"   src="../Scripts/config.js" ></script>
    <script type="text/javascript"   src="../Comm/Gener.js" ></script>
    <link href="../Scripts/slideBox/style/jquery.slideBox.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/slideBox/jquery.slideBox.min.js" type="text/javascript"></script>
    <script src="../Scripts/QueryString.js" type="text/javascript"></script>
    <script src="AttachmentUpload.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

        function Del(fk_ath, pkVal, delPKVal) {
            if (window.confirm('您确定要删除吗？ ') == false)
                return;
            window.location.href = 'AttachmentUpload.aspx?DoType=Del&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=<%=FK_Node %>&FK_Flow=<%=FK_Flow %>&FK_MapData=<%=FK_MapData %>&Ath=<%=Ath %>' + (GetQueryString("IsExtend")!=undefined?"&IsExtend="+GetQueryString("IsExtend"):"");
        }
   
        function OpenOfiice(fk_ath, pkVal, delPKVal, FK_MapData, NoOfObj, FK_Node) {
            var date = new Date();
            var t = date.getFullYear() + "" + date.getMonth() + "" + date.getDay() + "" + date.getHours() + "" + date.getMinutes() + "" + date.getSeconds();
            var url = '../WebOffice/AttachOffice.aspx?DoType=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + "&FK_MapData=" + FK_MapData + "&NoOfObj=" + NoOfObj + "&FK_Node=" + FK_Node + "&T=" + t;
            window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
        }

        function UploadChange(btn) {
            document.getElementById("<%= Btn_Upload.ClientID%>").click();
        }

        function OpenFileView(pkVal, delPKVal) {
            var url = 'FilesView.aspx?DoType=view&DelPKVal=' + delPKVal + '&PKVal=' + pkVal + '&FK_FrmAttachment=<%=FK_FrmAttachment %>&FK_FrmAttachmentExt=<%=FK_FrmAttachmentExt %>&FK_Flow=<%=FK_Flow %>&FK_Node=<%=FK_Node %>&WorkID=<%=WorkID %>&IsCC=<%=IsCC %>';
            window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes,resizable=yes,location=no, status=no');
        }
        function OpenView(pkVal, delPKVal) {
            var url = 'FilesView.aspx?DoType=view&DelPKVal=' + delPKVal + '&PKVal=' + pkVal + '&FK_MapData=<%=FK_MapData %>&FK_FrmAttachment=<%=FK_FrmAttachment %>&FK_FrmAttachmentExt=<%=FK_FrmAttachmentExt %>&FK_Flow=<%=FK_Flow %>&FK_Node=<%=FK_Node %>&WorkID=<%=WorkID %>&IsCC=<%=IsCC %>';
            window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes,resizable=yes,location=no, status=no');
        }
        // 上传Img.
        function UploadImg() {
            // window.(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
            //window.location.href = 'AttachmentUploadImg.aspx?FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=<%=FK_Node %>&FK_Flow = <%=FK_Flow %>&FK_MapData=<%=FK_MapData %>&Ath=<%=Ath %>';
        }

        //关闭窗口  适用于扩展属性
        function close() {
            if (parent != undefined && parent.SetAth != undefined && typeof (parent.SetAth) == "function") {
                var nameTds = $('.Idx').next();
                var nameStrs = [];
                $.each(nameTds, function (i, nameTd) {
                    nameStrs.push($(nameTd).children('a').text());
                })
                parent.SetAth(nameStrs);
            }
        }

		function setIframeHeight() {
			$("#" + window.frameElement.getAttribute("id"), parent.document).height(Math.max($(document).height(), $("body").height()) + ($(".Idx").length * 2));
		}

        $(function () {
            //是字段的扩展属性访问页面时
            if (GetQueryString('IsExtend') != undefined && GetQueryString('IsExtend')=="1") {
                $('#bar').css('display', 'block');
                $('#btn_Colse').bind('click', close);
            }
            //$('#bar').css('display', 'block');
            $('#btn_Colse').bind('click', close);

			setIframeHeight();
        });
    </script>
    <style type="text/css">
        .TBNote
        {
            border-bottom-color: Black;
            background-color: Silver;
        }
        Button
        {
            font-size: 12px;
        }
        
        .Btn
        {
            font-size: 12px;
            width:36px;
            height:30px;
            float:right;
            margin-right:15px;
        }
        input[type="button"], input[type="submit"]
{
    border: 0; /*background: #4D77A7;*/
    background: #009999;
    color: #FFF;
    font-size: 12px;
    padding: 4px 8px;
    margin: 5px 3px 5px 3px;
    border-radius: 5px;
}
        .style1
        {
            font-size: 12px;
            color: #FFFFFF;
            background-color: #2884FA;
        }
    </style>

   <%-- <link   href="../Scripts/Jquery-plug/fileupload/uploadify.css" rel="stylesheet"  type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"  type="text/javascript"></script>
    <script src="../Scripts/Jquery-plug/fileupload/jquery.uploadify.min.js" type="text/javascript"></script>
    <script src="../Scripts/Jquery-plug/fileupload/jquery.uploadify.js" type="text/javascript"></script>
     --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:Pub ID="Pub1" runat="server" EnableViewState="False" />
    <asp:Button runat="server" ID="Btn_Upload" OnClick="btn_Click" Style="display: none;" />

    <div id="bar" style="display: none;position: fixed;bottom: 0px;right: 0px;">
        <button class="btn btn-info" id="btn_Colse" style="border: 0;background: #2884fa;color: #FFF;font-size: 12px;padding: 6px 10px;border-radius: 15px;margin: 5px;">关闭</button>
     </div>
    
</asp:Content>
