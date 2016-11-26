<%@ Page Language="C#" AutoEventWireup="true" Title="流程测试" Inherits="CCFlow.WF.Admin.WF_Admin_TestFlow"  CodeBehind="TestFlow.aspx.cs" %>
<%@ Register Src="../Comm/UC/ucsys.ascx" TagName="ucsys" TagPrefix="uc2" %>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<script type="text/javascript" src="../Comm/JScript.js" />
    <script type="text/javascript" language="javascript">
        function Del(mypk, fk_flow, refoid) {
            if (window.confirm('Are you sure?') == false)
                return;

            var url = 'Do.aspx?DoType=Del&MyPK=' + mypk + '&RefOID=' + refoid;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function WinOpen(url) {
            var b = window.open(url, 'ass', 'width=700,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false');
            b.focus();
        }
        function WinOpen(url, w, h, name) {
            var b = window.open(url, name, 'width=' + w + ',height=' + h + ',scrollbars=yes,resizable=yes,toolbar=false,location=false,center: yes');
        }
        function WinOpenWAP_Cross(url) {
            var b = window.open(url, 'ass', 'width=50,top=50,left=50,height=20,scrollbars=yes,resizable=yes,toolbar=false,location=false');
        }
    </script>
    <script language="javascript" type="text/javascript" >
        function ShowIt(m) {
            var url = '../Comm/Method.aspx?M=' + m;
            var a = window.showModalDialog(url, 'OneVs', 'dialogHeight: 400px; dialogWidth: 500px; dialogTop: 100px; dialogLeft: 110px; center: yes; help: no');
        }
        function Open(no) {
            if (window.confirm('您确定要该流程编号为:'+no+'的数据吗？') == false)
                return;
            var url = '../Comm/RefMethod.aspx?Index=3&EnsName=BP.WF.Template.FlowSheets&No='+no;
            var a = window.showModalDialog(url, 'OneVs', 'dialogHeight: 400px; dialogWidth: 500px; dialogTop: 100px; dialogLeft: 110px; center: yes; help: no');
        }

        function SelectAll(cb_selectAll) {
            var arrObj = document.all;
            if (cb_selectAll.checked) {
                for (var i = 0; i < arrObj.length; i++) {
                    if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                        arrObj[i].checked = true;
                    }
                }
            } else {
                for (var i = 0; i < arrObj.length; i++) {
                    if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox')
                        arrObj[i].checked = false;
                }
            }
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
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />

</head>
<body  style="background-color:Silver; margin-top:0px;"  topmargin="0" leftmargin="0"   style="font-size:smaller" >
    <form id="form1" runat="server">

    
    <center>
<table width="910px"   border="0"  cellpadding="0" cellspacing="0" style="margin-top:0px;height:700px;background:white; text-align:center" > 

<caption>流程测试导航. 提示:下列人员，都是可以发起该流程的人员，点击一个人员进入流程发起界面。</caption>
<tr>
<td valign="top"  style="margin:5px;" >
    <uc2:ucsys ID="Ucsys1" runat="server" />
     </td>
    </tr>

    <tr>
    <td valign="top"  height="100%;margin-top:5px;">

     <br>
    <a href="javascript:ShowIt('BP.WF.DTS.ClearDB');"  >
    <img src='../Img/Btn/Delete.gif'  border=0 />清除所有流程运行的数据(此功能要在测试环境里运行)</a>
    <br><font size=2 color="Green">清除所有流程运行的数据，包括待办工作。</font>
    <br>
    <%  string flowNo = this.Request.QueryString["FK_Flow"];  %>
     <a href="javascript:Open('<%=flowNo %>');"  >
    <img src='../Img/Btn/Delete.gif'  border=0 />删除本流程数据(此功能要在测试环境里运行)</a>
    <br><font size="2" color="Green">清除本流程运行的数据，包括待办工作。</font>
     <br>
    <a href="../App/Simple/Login.aspx"  >
    <img src='../Img/Login.gif'  border=0 />直接登录极速模式</a>
    <br><font size=2 color="Green">直接登录极速模式</font>
    <br>
    <a href="../App/Classic/Login.aspx"  >
    <img src='../Img/Login.gif'  border=0 />登录经典模式</a>
    <br><font size=2 color="Green">登录经典模式</font>
    <br>
      </td>
      </tr>
      </table>
      </center>


       </form>
</body>
</html>
