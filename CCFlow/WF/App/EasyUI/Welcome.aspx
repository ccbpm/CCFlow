<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="CCFlow.AppDemoLigerUI.Welcome" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../../../WF/Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <style type="text/css">
        *
        {
            margin: 0;
            padding: 0;
        }
        body
        {
            font-size: 13px;
            line-height: 130%;
            padding: 60px;
        }
        .panel
        {
            width: 360px;
            border: 1px solid #0050D0;
        }
        .head
        {
            padding: 5px;
            background: #A3C0E8;
            cursor: pointer;
        }
        .content
        {
            padding: 10px;
            text-indent: 2em;
            border-top: 1px solid #0050D0;
            display: block;
        }
    </style>
    <script language="javascript" type="text/javascript">
        $(function () {
            $("#panel1 div.head").click(function () {
                $(this).next("div.content").slideToggle();
            });
            $("#panel2 h5.head").click(function () {
                $(this).next("div.content").slideToggle();
            })
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="display: inline;">
        <div id="panel1" class="panel" style="float:left">
            <div class="head">
                <h5>
                    公司简介</h5>
            </div>
            <div class="content"><br />
                公司名称:济南驰骋信息技术有限公司<br />
                Chicheng(Jinan,China) Information Technology Co., Ltd.<br />
                地址:山东省济南市文化东路51号汇东星座716室<br />
                Phone：0531-82374939(周六日不值班)<br />
                Mobile：18660153393<br />
                E-mail(msn)：chichengsoft@hotmail.com <br />
                业务联系：ccflow@ccflow.org <br />
                技术支持QQ群号：40644056<br />
                开户行:中国银行济南市高新技术开发区支行<br />
                账号:236411759398<br />
                <a href="http://www.ccflow.org/" target="_blank">公司网址：http://www.ccflow.org</a><br />
                <a href="http://bbs.ccflow.org/" target="_blank">ccflow论坛:http://bbs.ccflow.org</a><br />
            </div>
        </div>
        <div id="panel2" class="panel" style="width:460px;float:left; margin-left:30px;">
            <h5 class="head">
                产品线</h5>
            <div class="content">
               <img alt="" src="Img/ccProLine.png" style="width:400px; height:360px;" />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
