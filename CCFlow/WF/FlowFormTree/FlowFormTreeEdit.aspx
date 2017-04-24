<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowFormTreeEdit.aspx.cs"
    Inherits="CCFlow.WF.SheetTree.FlowFormTreeEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Style/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Style/themes/default/datagrid.css" rel="stylesheet" type="text/css" />
    <link href="../Style/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/FlowFormTreeData.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            font: 12px/12px Arial, sans-serif, Verdana, Tahoma;
            padding: 0;
            margin: 0;
        }
        a:link
        {
            text-decoration: none;
        }
        a:visited
        {
            text-decoration: none;
        }
        a:hover
        {
            text-decoration: underline;
        }
        a:active
        {
            text-decoration: none;
        }
        .cs-north
        {
            height: 40px;
        }
        .cs-north-bg
        {
            width: 100%;
            height: 38px;
        }
        .cs-west
        {
            width: 260px;
            padding: 0px;
        }
        .cs-south
        {
            height: 25px;
            background: url('themes/pepper-grinder/images/ui-bg_fine-grain_15_ffffff_60x60.png') repeat-x;
            padding: 0px;
            text-align: center;
        }
        .cs-navi-tab
        {
            padding: 5px;
            display: block;
            line-height: 18px;
            height: 18px;
            padding-left: 16px;
            text-decoration: none;
            border: 1px solid white;
            border-bottom: 1px #E5E5E5 solid;
        }
        .cs-navi-tab:hover
        {
            background: #FFEEAC;
            border: 1px solid #DB9F00;
        }
        .cs-tab-menu
        {
            width: 120px;
        }
        .cs-home-remark
        {
            padding: 10px;
        }
        .wrapper
        {
            float: right;
            height: 30px;
            margin-left: 10px;
        }
        .ui-skin-nav
        {
            float: right;
            padding: 0;
            margin-right: 10px;
            list-style: none outside none;
            height: 20px;
            visibility: hidden;
        }
        
        .ui-skin-nav .li-skinitem
        {
            float: left;
            font-size: 12px;
            line-height: 30px;
            margin-left: 10px;
            text-align: center;
        }
        .ui-skin-nav .li-skinitem span
        {
            cursor: pointer;
            width: 10px;
            height: 10px;
            display: inline-block;
        }
        .ui-skin-nav .li-skinitem span.cs-skin-on
        {
            border: 1px solid #FFFFFF;
        }
        
        .ui-skin-nav .li-skinitem span.gray
        {
            background-color: gray;
        }
        .ui-skin-nav .li-skinitem span.pepper-grinder
        {
            background-color: #BC3604;
        }
        .ui-skin-nav .li-skinitem span.blue
        {
            background-color: blue;
        }
        .ui-skin-nav .li-skinitem span.cupertino
        {
            background-color: #D7EBF9;
        }
        .ui-skin-nav .li-skinitem span.dark-hive
        {
            background-color: black;
        }
        .ui-skin-nav .li-skinitem span.sunny
        {
            background-color: #FFE57E;
        }
        .img-menu
        {
            height: 18px;
            width: 18px;
        }
        .space
        {
            color: #E7E7E7;
        }
        .l-topmenu-welcome
        {
            position: absolute;
            height: 24px;
            line-height: 24px;
            right: 30px;
            top: 3px;
            color: #070A0C;
        }
    </style>
    <script type="text/javascript">
        function addTab(id, title, url) {
            if ($('#tabs').tabs('exists', title)) {
                $('#tabs').tabs('select', title); //选中并刷新
                var currTab = $('#tabs').tabs('getSelected');
                //                var url = $(currTab.panel('options').content).attr('src');
                //                if (url != undefined && currTab.panel('options').title != '首页') {
                //                    $('#tabs').tabs('update', {
                //                        tab: currTab,
                //                        options: {
                //                            content: createFrame(url)
                //                        }
                //                    })
                //                }
            } else {
                var content = createFrame(url);
                $('#tabs').tabs('add', {
                    title: title,
                    id: id,
                    content: content,
                    closable: true
                });
            }
            tabClose();
        }

        //判断标签页是否存在
        function TabFormExists() {
            var currTab = $('#tabs').tabs('getSelected');
            if (currTab) return true;

            return false;
        }
        //修改标题
        function ChangTabFormTitle() {
            var tabText = "";
            var p = $(parent.document.getElementById("tabs")).find("li");
            $.each(p, function (i, val) {
                if (val.className == "tabs-selected") {
                    tabText = $($(val).find("span")[0]).text();
                }
            });

            var lastChar = tabText.substring(tabText.length - 1, tabText.length);
            if (lastChar != "*") {
                $.each(p, function (i, val) {
                    if (val.className == "tabs-selected") {
                        tabText = $($(val).find("span")[0]).text(tabText + '*');
                    }
                });
            }
        }

        function createFrame(url) {
            var s = '<iframe scrolling="auto" frameborder="0" Onblur="OnTabChange(this)"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
            return s;
        }

        //tab切换事件
        function OnTabChange(scope) {
            var p = $(parent.document.getElementById("tabs")).find("li");
            var tabText = "";
            $.each(p, function (i, val) {
                if (val.className == "tabs-selected") {
                    tabText = $($(val).find("span")[0]).text();
                }
            });
            var lastChar = tabText.substring(tabText.length - 1, tabText.length);
            if (lastChar == "*") {
                var contentWidow = scope.contentWindow;
                contentWidow.SaveDtlData();
                $.each(p, function (i, val) {
                    if (val.className == "tabs-selected") {
                        $($(val).find("span")[0]).text(tabText.substring(0, tabText.length - 1));
                    }
                });
            }
        }

        function tabClose() {
            /*双击关闭TAB选项卡*/
            $(".tabs-inner").dblclick(function () {
                var currTab = $('#tabs').tabs('getSelected');
                if (currTab) {
                    var currtab_title = currTab.panel('options').title;
                    $('#tabs').tabs('close', currtab_title);
                }
            })
            /*为选项卡绑定右键*/
            $(".tabs-inner").bind('contextmenu', function (e) {
                $('#mm').menu('show', {
                    left: e.pageX,
                    top: e.pageY
                });
                var subtitle = "";
                var currTab = $('#tabs').tabs('getSelected');
                if (currTab) {
                    subtitle = currTab.panel('options').title;
                }

                $('#mm').data("currtab", subtitle);
                $('#tabs').tabs('select', subtitle);
                return false;
            });
        }
        //绑定右键菜单事件
        function tabCloseEven() {
            //刷新
            $('#mm-tabupdate').click(function () {
                var currTab = $('#tabs').tabs('getSelected');
                var url = $(currTab.panel('options').content).attr('src');
                if (url != undefined && currTab.panel('options').title != '首页') {
                    $('#tabs').tabs('update', {
                        tab: currTab,
                        options: {
                            content: createFrame(url)
                        }
                    })
                }
            })
            //关闭当前
            $('#mm-tabclose').click(function () {
                var currtab_title = $('#mm').data("currtab");
                $('#tabs').tabs('close', currtab_title);
            })
            //全部关闭
            $('#mm-tabcloseall').click(function () {
                $('.tabs-inner span').each(function (i, n) {
                    var t = $(n).text();
                    if (t != '首页') {
                        $('#tabs').tabs('close', t);
                    }
                });
            });
            //关闭除当前之外的TAB
            $('#mm-tabcloseother').click(function () {
                var prevall = $('.tabs-selected').prevAll();
                var nextall = $('.tabs-selected').nextAll();
                if (prevall.length > 0) {
                    prevall.each(function (i, n) {
                        var t = $('a:eq(0) span', $(n)).text();
                        if (t != '首页') {
                            $('#tabs').tabs('close', t);
                        }
                    });
                }
                if (nextall.length > 0) {
                    nextall.each(function (i, n) {
                        var t = $('a:eq(0) span', $(n)).text();
                        if (t != '首页') {
                            $('#tabs').tabs('close', t);
                        }
                    });
                }
                return false;
            });
            //关闭当前右侧的TAB
            $('#mm-tabcloseright').click(function () {
                var nextall = $('.tabs-selected').nextAll();
                if (nextall.length == 0) {
                    //msgShow('系统提示','后边没有啦~~','error');
                    alert('后边没有啦~~');
                    return false;
                }
                nextall.each(function (i, n) {
                    var t = $('a:eq(0) span', $(n)).text();
                    $('#tabs').tabs('close', t);
                });
                return false;
            });
            //关闭当前左侧的TAB
            $('#mm-tabcloseleft').click(function () {
                var prevall = $('.tabs-selected').prevAll();
                if (prevall.length == 0) {
                    alert('到头了，前边没有啦~~');
                    return false;
                }
                prevall.each(function (i, n) {
                    var t = $('a:eq(0) span', $(n)).text();
                    $('#tabs').tabs('close', t);
                });
                return false;
            });

            //退出
            $("#mm-exit").click(function () {
                $('#mm').menu('hide');
            })
        }
        //获取参数
        var RequestArgs = function () {
            this.WorkID = Application.common.getArgsFromHref("WorkID");
            this.FK_Flow = Application.common.getArgsFromHref("FK_Flow");
            //this.FK_Node = Application.common.getArgsFromHref("FK_Node");
            this.FK_Node = parseInt(this.FK_Flow) + '01';
            this.NodeID = Application.common.getArgsFromHref("NodeID");
            this.UserNo = Application.common.getArgsFromHref("UserNo");
            this.FID = Application.common.getArgsFromHref("FID");
            this.SID = Application.common.getArgsFromHref("SID");

            this.PWorkID = Application.common.getArgsFromHref("PWorkID");
            this.PFlowNo = Application.common.getArgsFromHref("PFlowNo");


            this.DoFunc = Application.common.getArgsFromHref("DoFunc");
            this.CFlowNo = Application.common.getArgsFromHref("CFlowNo");
            this.WorkIDs = Application.common.getArgsFromHref("WorkIDs");


            this.IsLoadData = 1;
        }
        //传参
        var urlExtFrm = function () {
            var extUrl = "";
            var args = new RequestArgs();
            if (args.WorkID != "")
                extUrl += "&WorkID=" + args.WorkID;
            if (args.FK_Flow != "") {
                extUrl += "&FK_Flow=" + args.FK_Flow;
                extUrl += "&FK_Node=" + parseInt(args.FK_Flow);
                extUrl += "&NodeID=" + parseInt(args.FK_Flow);
            }
            if (args.UserNo != "")
                extUrl += "&UserNo=" + args.UserNo;
            if (args.FID != "")
                extUrl += "&FID=" + args.FID;
            if (args.SID != "")
                extUrl += "&SID=" + args.SID;

            if (args.PWorkID != "")
                extUrl += "&PWorkID=" + args.PWorkID;
            if (args.PFlowNo != "")
                extUrl += "&PFlowNo=" + args.PFlowNo;
            if (args.IsLoadData != "")
                extUrl += "&IsLoadData=" + args.IsLoadData;

            //获取其他参数
            var sHref = window.location.href;
            var args = sHref.split("?");
            var retval = "";
            if (args[0] != sHref) /*参数不为空*/
            {
                var str = args[1];
                args = str.split("&");
                for (var i = 0; i < args.length; i++) {
                    str = args[i];
                    var arg = str.split("=");
                    if (arg.length <= 1) continue;
                    //不包含就添加
                    if (extUrl.indexOf(arg[0]) == -1) {
                        extUrl += "&" + arg[0] + "=" + arg[1];
                    }
                }
            }

            return extUrl;
        }
        //事件
        function EventFactory(event) {
            var args = new RequestArgs();
            var strTimeKey = "";
            var date = new Date();
            strTimeKey += date.getFullYear(); //年
            strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
            strTimeKey += date.getDate(); //日
            strTimeKey += date.getHours(); //HH
            strTimeKey += date.getMinutes(); //MM
            strTimeKey += date.getSeconds(); //SS

            switch (event) {
                case "save": //保存
                    $.messager.alert('提示', '保存成功！', 'info');
                    break;
                case "showchart": //轨迹
                    WinOpenPage("_blank", "../WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow + "&FID=" + args.FID + "&FK_Node=" + args.FK_Node + "&s=" + strTimeKey, "轨迹图");
                    break;
                case "search": //查询
                    WinOpenPage("_blank", "../Rpt/Search.aspx?RptNo=ND" + parseInt(args.FK_Flow) + "MyRpt&FK_Flow=" + args.FK_Flow + "&s=" + strTimeKey, "查询");
                    break;
                case "closeWin":
                    closeWin();
                    break;
                case "": //备份数据
            }
        }

        //页面初始
        $(function () {
            $("#pageloading").show();
            //初始工具栏
            var args = new RequestArgs();
            //表单树
            Application.data.getFlowFormTree(args.FK_Flow, 0, function (js) {
                var isSelect = false;
                var pushData = eval('(' + js + ')');
                //加载类别树
                $("#flowFormTree").tree({
                    data: pushData,
                    iconCls: 'tree-folder',
                    collapsed: true,
                    lines: true,
                    onClick: function (node) {
                        if (node.attributes.NodeType == "form|0" || node.attributes.NodeType == "form|1") {/*普通表单和必填表单*/
                            var urlExt = urlExtFrm();
                            var url = "../CCForm/Frm.htm?FK_MapData=" + node.id + "&IsEdit=1&IsPrint=0" + urlExt;
                            addTab(node.id, node.text, url);
                        } else if (node.attributes.NodeType == "tools|0") {
                            var urlExt = urlExtFrm();
                            var url = node.attributes.Url;
                            while (url.indexOf('|') >= 0) {
                                url = url.replace('|', '/');
                            }
                            if (url.indexOf('?') > 0) {
                                url = url + "&FK_MapData=" + node.id + "&" + urlExt;
                            }
                            else {
                                url = url + "?FK_MapData=" + node.id + "&" + urlExt;
                            }
                            addTab(node.id, node.text, url);
                        } else if (node.attributes.NodeType == "tools|1") {
                            var urlExt = urlExtFrm();
                            var url = node.attributes.Url;
                            while (url.indexOf('|') >= 0) {
                                url = url.replace('|', '/');
                            }
                            if (url.indexOf('?') > 0) {
                                url = url + "&FK_MapData=" + node.id + "&" + urlExt;
                            }
                            else {
                                url = url + "?FK_MapData=" + node.id + "&" + urlExt;
                            }
                            WinOpenPage("_blank", url, node.text);
                        }
                    }
                });
                $("#pageloading").hide();
                $("#send").show();
                $("#save").show();
                $("#A6").show();
            }, this);
            var urlExt = urlExtFrm();
            if (args.WorkID) {
                addTab("trackid", "轨迹", "../WorkOpt/OneWork/OneWork.htm?CurrTab=Track" + urlExt);
            }
            //tab页操作事件
            tabCloseEven();
        });

        function setCookie(name, value) {//两个参数，一个是cookie的名子，一个是值
            var Days = 30; //此 cookie 将被保存 30 天
            var exp = new Date();    //new Date("December 31, 9998");
            exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
            document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
        }

        function getCookie(name) {//取cookies函数        
            var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
            if (arr != null) return unescape(arr[2]); return null;
        }
        //打开窗体
        function WinOpenPage(target, url, title) {
            window.open(url, target, "left=0,top=0,width=" + (screen.availWidth - 10) + ",height=" + (screen.availHeight - 50) + ",scrollbars,resizable=yes,toolbar=yes,menubar=yes'");
        }
        function WinOpen(url, winName) {
            var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
            newWindow.focus();
            return;
        }
        function closeWin() {
            if (window.dialogArguments && window.dialogArguments.window) {
                window.dialogArguments.window.location = window.dialogArguments.window.location;
            }
            window.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="pageloading">
    </div>
    <div region="north" border="true" class="cs-north">
        <div class="cs-north-bg">
            <div id="toolBars" style="padding: 5px; background: #EAF2FE;" runat="server">
            </div>
        </div>
    </div>
    <div region="west" border="true" split="true" title="资料树" class="cs-west">
        <ul id="flowFormTree" class="easyui-tree" data-options="animate:true,dnd:false">
        </ul>
    </div>
    <div id="mainPanle" region="center" border="true" border="false">
        <div id="tabs" class="easyui-tabs" fit="true" border="false" data-options="tools:'#tab-tools'">
        </div>
    </div>
    <div id="mm" class="easyui-menu cs-tab-menu">
        <div id="mm-tabupdate">
            刷新</div>
        <div class="menu-sep">
        </div>
        <div id="mm-tabclose">
            关闭</div>
        <div id="mm-tabcloseother">
            关闭其他</div>
        <div id="mm-tabcloseall">
            关闭全部</div>
    </div>
</body>
</html>
