<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Left.aspx.cs" Inherits="CCFlow.App.AppDemo_Left" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>导航</title>
    <link href="css/basic.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
    <script src="/WF/Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script language="JavaScript" type="text/javascript">
        function myrefresh() {
            window.location.reload();
        }

        setInterval('GetEmpWorks()', 300000); //指定10秒刷新一次
        //div展开 关闭
        function block_none(obj, ig) {
            var dv = document.getElementById(obj);
            var ig = document.getElementById(ig);
            if (dv.style.display == "none") {
                dv.style.display = "block";
                ig.src = "Img/Menu/zhankai.jpg";
            } else {
                dv.style.display = "none";
                ig.src = "Img/Menu/shouqi.jpg";
            }
        }
        function GetEmpWorks() {
            var param = { method: "getempworks" };
            $.ajax({
                type: 'get', //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: 'LigerUI/Base/GTDataService.aspx', //要访问的后台地址
                data: param, //要发送的数据
                async: true,
                cache: false,
                complete: function () { }, //AJAX请求完成时隐藏loading提示
                error: function (XMLHttpRequest, errorThrown) {
                },
                success: function (data) {//msg为返回的数据，在这里做数据绑定
                    if (data) {
                        var pushData = eval('(' + data + ')');
                        document.getElementById('empWorks').innerHTML = '待办(' + pushData.Rows.length + ')';
                    }
                }
            });

        }

        function GetRunning() {
            var param = { method: "Running" };

            $.ajax({
                type: 'get', //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: 'LigerUI/Base/GTDataService.aspx', //要访问的后台地址
                data: param, //要发送的数据
                async: true,
                cache: false,
                complete: function () { }, //AJAX请求完成时隐藏loading提示
                error: function (XMLHttpRequest, errorThrown) {
                },
                success: function (data) {//msg为返回的数据，在这里做数据绑定
                    if (data) {
                        var pushData = eval('(' + data + ')');
                        try {
                            document.getElementById('empRunning').innerHTML = '已办(在途)(' + pushData.Rows.length + ')';
                        }
                        catch (e) {
                            document.getElementById('empRunning').innerHTML = '已办(在途)(0)';
                        }
                    }
                }
            });
        }

        function GetPrefect() {
            var param = { method: "perfectproinfo" };

            $.ajax({
                type: 'get', //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: 'LigerUI/Base/GTDataService.aspx', //要访问的后台地址
                data: param, //要发送的数据
                async: true,
                cache: false,
                complete: function () { }, //AJAX请求完成时隐藏loading提示
                error: function (XMLHttpRequest, errorThrown) {
                },
                success: function (data) {//msg为返回的数据，在这里做数据绑定
                    if (data) {
                        var pushData = eval('(' + data + ')');
                        try {
                            document.getElementById('empPrefect').innerHTML = '退回项目(' + pushData.Rows.length + ')';
                        }
                        catch (e) {
                            document.getElementById('empPrefect').innerHTML = '退回项目(0)';
                        }
                    }
                }
            });
        }
        $(function () {
//            GetEmpWorks();
//            GetRunning();
//            GetPrefect();
        });
    </script>

    <style  type="text/css" >
        .FuncBar
        {
            margin-left:12px;
            padding-left:12px;
            font-size:15px;
            float:left;
            color:White;
        ｝
    </style>
</head>
<body style="background: #256db5">
    <form id="form1" runat="server">
    <div class="JS_zhut">
        <div class="JS_zhut_n">
            <div class="left fl">
                <div>
                    <!-- 工作流程 -->
                    <div class="title" onclick="block_none('divWorkFlow','imgWorkFlow') " style="background-image: url('Img/Menu/FuncBar.jpg');
                        width: 167px;">  
                        <div class="FuncBar" style="" >BP组件</div>
                        <span style="margin-left: 126px; margin-right: 8px; margin-bottom: 10px; margin-top: 10px;">
                            <img id="imgWorkFlow" alt="" src="Img/Menu/zhankai.jpg" /></span>
                    </div>
                    <div id="divWorkFlow">
                        <ul id="Ul1" class="menu" runat="server">
                            <li runat="server" id="Li3" style="background: url('Img/Menu/Start.jpg') -1px 0px">
                                <a href="/WF/Comm/Ens.aspx?EnsName=BP.Demo.BanJis" target="main">班级维护</a></li>

                                 <li runat="server" id="Li1" style="background: url('Img/Menu/Start.jpg') -1px 0px">
                                <a href="/WF/Comm/UIEn.aspx?EnsName=BP.Demo.BPFramework.Students" target="main">新建学生</a></li>

                            <li runat="server" id="Li14" style="background: url('Img/Menu/Start.jpg') -1px 0px">
                                <a href="/WF/Comm/Search.aspx?EnsName=BP.Demo.BPFramework.Students" target="main">学生查询</a></li>

                             <li runat="server" id="Li2" style="background: url('Img/Menu/Start.jpg') -1px 0px">
                                <a href="/WF/Comm/Group.aspx?EnsName=BP.Demo.BPFramework.Students" target="main">学生统计</a></li>
                        </ul>
                    </div>
                    <!-- end工作流程 -->

                    <div>
                       <div class="title" onclick="block_none('div_CLGL','imgCLGL') " style="background-image: url('Img/Menu/FuncBar.jpg');
                        width: 167px;">  <div class="FuncBar" >数据采集</div>
                            <span style="margin-left: 126px; margin-right: 8px; margin-bottom: 10px; margin-top: 10px;">
                                <img id="imgCLGL" src="Img/Menu/zhankai.jpg" /></span>
                        </div>
                        <div id="div_CLGL" style="height: 0px;">
                            <ul id="Ul4" class="menu" runat="server">

                                <li runat="server" id="Li11" style="background: url(Img/Menu/lxba.jpg) -1px 0px">
                                <a href="/SDKFlowDemo/BPFramework/DataInput/DemoRegUserWithRequest.aspx" target="main">通用界面</a></li>

                                 <li runat="server" id="Li4" style="background: url(Img/Menu/lxba.jpg) -1px 0px">
                                <a href="/SDKFlowDemo/BPFramework/DataInput/DemoRegUserWithUserContral.aspx" target="main">用户控件</a></li>

                                 <li runat="server" id="Li6" style="background: url(Img/Menu/lxba.jpg) -1px 0px">
                                <a href="/SDKFlowDemo/BPFramework/DataInput/Cell2D.aspx" target="main">2维表格</a></li>

                                  <li runat="server" id="Li5" style="background: url(Img/Menu/lxba.jpg) -1px 0px">
                                <a href="/SDKFlowDemo/BPFramework/DataInput/Cell3DLeft.aspx" target="main">3维2维度在左边</a></li>

                                 <li runat="server" id="Li9" style="background: url(Img/Menu/lxba.jpg) -1px 0px">
                                <a href="/SDKFlowDemo/BPFramework/DataInput/Cell3DLeft.aspx" target="main">3维2维度在上边</a></li>
                                 
                            </ul>
                        </div>
                    </div>
                </div>
                 

                <div>
                   <div class="title" onclick="block_none('div_XTWH','imgXTWH') " style="background-image: url('Img/Menu/FuncBar.jpg');
                        width: 167px;">  <div class="FuncBar" >输出</div>
                        <span style="margin-left: 126px; margin-right: 8px; margin-bottom: 10px; margin-top: 10px;">
                            <img id="imgXTWH" src="Img/Menu/zhankai.jpg" /></span>
                    </div>
                    <div id="div_XTWH" style="height: 50px;">
                        <ul id="Ul2" class="menu" runat="server">

                            <li runat="server" id="Li17" style="background: url('Img/Menu/dbjtj.jpg') -1px 0px">
                                <a href="/SDKFlowDemo/BPFramework/Output/DemoJZOut.aspx" target="main">矩阵输出</a></li>

                                 <li runat="server" id="Li7" style="background: url('Img/Menu/dbjtj.jpg') -1px 0px">
                                <a href="/SDKFlowDemo/BPFramework/Output/DemoTurnPage.aspx" target="main">翻页</a></li>

                                  <li runat="server" id="Li8" style="background: url('Img/Menu/dbjtj.jpg') -1px 0px">
                                <a href="/SDKFlowDemo/BPFramework/Output/DemoUserList.aspx" target="main">不分页</a></li>
                        </ul>
                    </div>
                </div>
                  

            </div>
        </div>
    </div>
    </form>
</body>
</html>
