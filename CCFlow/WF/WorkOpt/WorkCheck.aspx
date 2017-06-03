<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkCheck.aspx.cs" Inherits="CCFlow.WF.WorkOpt.FrmWorkCheckUI" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Comm/JScript.js" type="text/javascript"></script>
    <link href="../../DataUser/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/Jquery-plug/fileupload/jquery.uploadify.min.js" type="text/javascript"></script>
    <link href="../Scripts/Jquery-plug/fileupload/uploadify.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/QueryString.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        var isChange = false;
        function NoSubmit(ev) {
            if (window.event.srcElement.tagName == "TEXTAREA")
                return true;
            if (ev.keyCode == 13) {
                window.event.keyCode = 9;
                ev.keyCode = 9;
                return true;
            }
            return true;
        }

        function show_and_hide_tr(tb_id, obj) {
            $("#" + tb_id).find("tr").each(function (i) {
                i > 0 ? (this.style.display == "none" ? this.style.display = "" : this.style.display = "none") : ($(this).next().css("display") == "none" ? (obj.src = '../Img/Tree/Cut.gif') : (obj.src = '../Img/Tree/Add.gif'));
            });
        }
        //执行保存
        function SaveDtlData() {
            if (isChange == true) {
                $("*[id$=Btn_Save]")[0].click();
                isChange = false;
            }
        }

        function Change(id) {
            isChange = true;
        }

        function UploadFileChange() {
            this.detachEvent('onblur', '');
            isChange = false;
        }

        //删除附件
        function DelWorkCheckAth(MyPK) {
            isChange = false;
            if (confirm("确定要删除所选文件吗？")) {
                $.ajax({
                    type: "GET", //使用GET或POST方法访问后台
                    dataType: "text", //返回json格式的数据
                    contentType: "application/json; charset=utf-8",
                    url: "../CCForm/Handler.ashx?DoType=DelWorkCheckAttach&MyPK=" + MyPK, //要访问的后台地址
                    async: false,
                    cache: false,
                    success: function (msg) {//msg为返回的数据，在这里做数据绑定
                        if (msg == "true") {
                            isChange = true;
                            SaveDtlData();
                        }
                        if (msg == "false") {
                            alert("删除失败。");
                        }
                    }
                });
            }
        }

        function TBHelp(ctrl, enName) {
            //alert(ctrl + "-" + enName);
            var explorer = window.navigator.userAgent;
            var str = "";
            var url = "../Comm/HelperOfTBEUI.aspx?EnsName=" + enName + "&AttrKey=" + ctrl + "&WordsSort=0" + "&FK_MapData=" + enName + "&id=" + ctrl;
            if (explorer.indexOf("Chrome") >= 0) {
                window.open(url, "sd", "left=200,height=500,top=150,width=600,location=yes,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no");
            }
            else {
                str = window.showModalDialog(url, 'sd', 'dialogHeight: 500px; dialogWidth:600px; dialogTop: 150px; dialogLeft: 200px; center: no; help: no');
                if (str == undefined)
                    return;
                ctrl = ctrl.replace("WorkCheck", "TB");
                $("*[id$=" + ctrl + "]").focus().val(str);
            }
        }

        function Ath(paras) {
            var url = "WorkCheckAth.aspx?1=2" + paras;
            str = window.showModalDialog(url, 'sd', 'dialogHeight: 200px; dialogWidth:800px; dialogTop: 150px; dialogLeft: 200px; center: no; help: no');
            if (str == undefined)
                return;
        }

        function AthDown(fk_ath, pkVal, delPKVal, fk_node, fk_flow, ath) {
            window.location.href = '../CCForm/AttachmentUpload.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + fk_node + '&FK_Flow=' + fk_flow + '&FK_MapData=ND' + fk_node + '&Ath=' + ath;
        }
        function AthOpenOfiice(fk_ath, pkVal, delPKVal, FK_MapData, NoOfObj, FK_Node) {
            var date = new Date();
            var t = date.getFullYear() + "" + date.getMonth() + "" + date.getDay() + "" + date.getHours() + "" + date.getMinutes() + "" + date.getSeconds();
            var url = '../WebOffice/AttachOffice.aspx?DoType=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + "&FK_MapData=" + FK_MapData + "&NoOfObj=" + NoOfObj + "&FK_Node=" + FK_Node + "&T=" + t;
            window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
        }
        function AthOpenFileView(pkVal, delPKVal, FK_FrmAttachment, FK_FrmAttachmentExt, fk_flow, fk_node, workID, isCC) {
            var url = '../CCForm/FilesView.aspx?DoType=view&DelPKVal=' + delPKVal + '&PKVal=' + pkVal + '&FK_FrmAttachment=' + FK_FrmAttachment + '&FK_FrmAttachmentExt=' + FK_FrmAttachmentExt + '&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&WorkID=' + workID + '&IsCC=' + isCC;
            window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
        }
        function AthOpenView(pkVal, delPKVal, FK_FrmAttachment, FK_FrmAttachmentExt, FK_Flow, FK_Node, WorkID, IsCC) {
            var url = '../CCForm/FilesView.aspx?DoType=view&DelPKVal=' + delPKVal + '&PKVal=' + pkVal + '&FK_FrmAttachment=' + FK_FrmAttachment + '&FK_FrmAttachmentExt=' + FK_FrmAttachmentExt + '&FK_Flow=' + FK_Flow + '&FK_Node=' + FK_Node + '&WorkID=' + WorkID + '&IsCC=' + IsCC;
            window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
        }
        $(function () {
            $('.HBtn').hide();
            $('#loading-mask').fadeOut();
            //修改以使iframe自适应审核页面的高度，以使打印时可以打全审核信息，edited by liuxc,2017-5-22
            //注意：要求审核组件位于表单页面最下方
//            if (window.parent) {
//                var height = $(document.body).height();
//                var parentIframe = window.parent.document.getElementById("FWCND" + GetQueryString("FK_Node"));

//                if (parentIframe) {
//                    var iheight = $(parentIframe).height();

//                    if (height > iheight) {
//                        $(parentIframe).height(height + 10);
//                    }

//                    var divCCForm = window.parent.document.getElementById("divCCForm");
//                    var divCCFormHeight = $(divCCForm).height();

//                    if (height > divCCFormHeight) {
//                        $(divCCForm).height(height + 20);
//                    }
//                }
//            }
        });
    </script>
    <style type="text/css">
        #Pub1_TB_Doc
        {
            border: 2px solid #D6DDE6;
        }
        #loading-mask
        {
            position: absolute;
            top: 0px;
            left: 0px;
            width: 100%;
            height: 100%;
            background: #D2E0F2;
            z-index: 20000;
        }
        #pageloading
        {
            position: absolute;
            top: 50%;
            left: 50%;
            margin: -120px 0px 0px -120px;
            text-align: center;
            border: 2px solid #8DB2E3;
            width: 200px;
            height: 40px;
            font-size: 14px;
            padding: 10px;
            font-weight: bold;
            background: #fff;
            color: #15428B;
        }
    </style>
</head>
<body leftmargin="0" topmargin="0">
    <form id="form1" runat="server">
    <div id="loading-mask" class="loddingMask">
        <div id="pageloading" class="pageloading">
            <img alt="" src="../Img/loading.gif" align="middle" />
            请稍候...
        </div>
    </div>
    <asp:Button ID="Btn_Save" runat="server" Text="" Width="0" Height="0" CssClass="HBtn"
        OnClick="btn_Save_Click" />
    <uc1:Pub ID="Pub1" runat="server" />
    </form>
</body>
</html>
