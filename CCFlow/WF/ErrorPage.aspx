<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="CCFlow.WF.ErrorPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

 <script src="Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <meta charset="UTF-8">
    <title>错误页面</title>
    <style type="text/css">
        *
        {
            margin: 0;
            padding: 0;
        }
        body
        {
            background: #F8F8F8;
            font-size: 14px;
        }
        .main_outer
        {
            background: url(Style/Img/background.png) no-repeat center 310px;
            width: 1050px;
            margin: 50px auto;
        }
        .main
        {
            background: url(Style/Img/not-found.png) no-repeat 57px -20px;
        }
        .main_inner
        {
            background: url(Style/Img/shadow.gif) no-repeat center 363px;
            padding-bottom: 50px;
        }
        .tips
        {
            height: 310px;
            background: url(Style/Img/cup.png) no-repeat 758px bottom;
            padding: 15px 25px;
            font-family: microsoft yahei;
            padding-right: 185px;
            padding-top: 50px;
        }
        .contentDiv
        {
            margin-left: 540px;
            width: 510px;
            height: 170px;
            overflow: scroll;
            word-break: break-all;
            word-wrap: break-word;
            margin-bottom: 15px;
        }
    </style>
    <script type="text/javascript">
        $(function () {

            HideDivInfo();
        });
        function HideDivInfo() {
            var oTb = document.getElementById('table1');
            if (oTb.rows[1].style.display == "none") {
                for (var i = 1; i < oTb.rows.length; i++) {
                    oTb.rows[i].style.display = "";
                }
                $('#title').html("<b class='bt'>异常信息&nbsp;（隐藏）</b>");
            }
            else {
                for (var i = 1; i < oTb.rows.length; i++) {
                    oTb.rows[i].style.display = "none";
                }
                $('#title').html("<b class='bt'>异常信息&nbsp;（显示）</b>");
            }
        }
    </script>
</head>
<body>
    <div class="main_outer">
        <div class="main">
            <div class="main_inner">
                <div class="tips">
                    <div class="contentDiv">
                        <table id="table1" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td onclick="HideDivInfo()" id="title">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>异常页面:</b>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div id="errorUrl" runat="server">
                                    </div>
                                </td>
                            </tr>
                           <tr>
                               <td><b>异常信息:</b></td>
                           </tr>
                            <tr>
                                <td>
                                    <div id="errorMessage" runat="server">
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="shadow">
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>