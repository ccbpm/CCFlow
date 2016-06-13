<%@ Page Language="C#" MasterPageFile="WinOpenEUI.master" AutoEventWireup="true"
    ValidateRequest="false" Inherits="CCFlow.WF.WF_MyFlowSmall" Title="工作处理" CodeBehind="MyFlow.aspx.cs" %>

<%@ Register Src="UC/MyFlowUC.ascx" TagName="MyFlowUC" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="Comm/JScript.js" type="text/javascript"></script>
    <script src="Comm/JS/TBHelpDiv.js" type="text/javascript"></script>
    <script src="CCForm/MapExt.js" type="text/javascript"></script>
    <script src="../DataUser/JSLibData/MyFlowPublic.js" type="text/javascript"></script>
    <script src="Style/Frm/jquery.idTabs.min.js" type="text/javascript"></script>
    <script src="Comm/JS/Calendar/WdatePicker.js" type="text/javascript"></script>
    <link href="Comm/JS/Calendar/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="../DataUser/PrintTools/LodopFuncs.js"></script>
    <script type="text/javascript" language="javascript" src="Scripts/MyFlow.js"></script>
    <script type="text/javascript">
        function OpenOfiice(fk_ath, pkVal, delPKVal, FK_MapData, NoOfObj, FK_Node) {
            var date = new Date();
            var t = date.getFullYear() + "" + date.getMonth() + "" + date.getDay() + "" + date.getHours() + "" + date.getMinutes() + "" + date.getSeconds();

            var url = 'WebOffice/AttachOffice.aspx?DoType=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + "&FK_MapData=" + FK_MapData + "&NoOfObj=" + NoOfObj + "&FK_Node=" + FK_Node + "&T=" + t;
            //var url = 'WebOffice.aspx?DoType=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal;
            // var str = window.showModalDialog(url, '', 'dialogHeight: 1250px; dialogWidth:900px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no;resizable:yes');
            //var str = window.open(url, '', 'dialogHeight: 1200px; dialogWidth:1110px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no;resizable:yes');
            window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');

        }

        //按钮.
        function FocusBtn(btn, workid) {
            if (btn.value == '关注') {
                btn.value = '取消关注';
            }
            else {
                btn.value = '关注';
            }
            $.ajax({ url: "Do.aspx?ActionType=Focus&WorkID=" + workid, async: false });
        }

        function ReturnVal(ctrl, url, winName) {
            if (url == "")
                return;
            //update by dgq 2013-4-12 判断有没有？
            if (ctrl && ctrl.value != "") {
                if (url.indexOf('?') > 0)
                    url = url + '&CtrlVal=' + ctrl.value;
                else
                    url = url + '?CtrlVal=' + ctrl.value;
            }
            //修改标题控制不进行保存
            if (typeof self.parent.TabFormExists != 'undefined') {
                var bExists = self.parent.TabFormExists();
                if (bExists) {
                    self.parent.ChangTabFormTitleRemove();
                }
            }
            
            //OpenJbox();
            try {
                $.jBox("iframe:" + url, {
                    title: '选择窗',
                    width: 760,
                    height: 450,
                    buttons: { '确定': 'ok' },
                    submit: function (v, h, f) {
                        var iframeWin = h[0].firstChild.contentWindow;
                        if (iframeWin) {
                            if (typeof iframeWin.getReturnText != 'undefined') {
                                var txtId = ctrl.id;
                                ctrl.value = iframeWin.getReturnText();
                                if (typeof iframeWin.getReturnValue != 'undefined') {
                                    $('#' + txtId + "_ReValue").val(iframeWin.getReturnValue());
                                }
                            } else {
                                ctrl.value = iframeWin.returnValue;
                            }
                        }
                        //移除
                        $(".jbox-body").remove();
                    }
                });
            } catch (e) {
                alert(e);
            }
//            if (window.ActiveXObject) {//如果是IE浏览器，执行下列方法
//                var v = window.showModalDialog(url, winName, 'scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes;dialogHeight: 650px; dialogWidth: 850px; dialogTop: 100px; dialogLeft: 150px;');
//                if (v == null || v == '' || v == 'NaN') {
//                    return;
//                }
//                ctrl.value = v;
//            }
//            else {//如果是chrome，执行下列方法a
//                try {
//                    $.jBox("iframe:" + url, {
//                        title: '标题',
//                        width: 800,
//                        height: 350,
//                        buttons: { 'Sure': 'ok' },
//                        submit: function (v, h, f) {
//                            var row = h[0].firstChild.contentWindow.getSelected();
//                            ctrl.value = row.Name;
//                        }
//                    });
//                } catch (e) {
//                    alert(e);
//                }
//            }
            //修改标题，失去焦点时进行保存
            if (typeof self.parent.TabFormExists != 'undefined') {
                var bExists = self.parent.TabFormExists();
                if (bExists) {
                    self.parent.ChangTabFormTitle();
                }
            }
            return;
        }
        //然浏览器最大化.
        function ResizeWindow() {
            if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
                var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
                var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高     
                window.moveTo(0, 0);           //把window放在左上角     
                window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
            }
        }
        window.onload = ResizeWindow;
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src="Scripts/jBox/jquery.jBox-2.3.min.js" type="text/javascript"></script>
    <link href="Scripts/jBox/Skins/Blue/jbox.css" rel="stylesheet" type="text/css" />  
    <uc1:MyFlowUC ID="MyFlowUC1" runat="server" />
    
</asp:Content>
