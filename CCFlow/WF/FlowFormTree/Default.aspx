<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CCFlow.WF.SheetTree.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Style/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Style/themes/default/datagrid.css" rel="stylesheet" type="text/css" />
    <link href="../Style/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/FlowFormTreeData.js" type="text/javascript"></script>
    <script src="../../DataUser/JSLibData/MyFlowPublic.js" type="text/javascript"></script>
    <script type="text/javascript">
        //然浏览器最大化.
        function ResizeWindow() {
            if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
                var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
                var myh = screen.availHeight - 5;  //定义一个myw，接受到当前全屏的高     
                window.moveTo(0, 0);           //把window放在左上角     
                window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
            }
        }
        //window.onload = ResizeWindow();
    </script>
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
            var p = $(document.getElementById("tabs")).find("li");
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

        //修改标题
        function ChangTabFormTitleRemove() {
            var tabText = "";
            var p = $(parent.document.getElementById("tabs")).find("li");
            $.each(p, function (i, val) {
                if (val.className == "tabs-selected") {
                    tabText = $($(val).find("span")[0]).text();
                }
            });

            var lastChar = tabText.substring(tabText.length - 1, tabText.length);
            if (lastChar == "*") {
                $.each(p, function (i, val) {
                    if (val.className == "tabs-selected") {
                        $($(val).find("span")[0]).text(tabText.substring(0, tabText.length - 1));
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
            //表单内容修改，执行自动保存
            var p = $(document.getElementById("tabs")).find("li");
            var tabText = "";
            $.each(p, function (i, val) {
                if (val.className == "tabs-selected") {
                    tabText = $($(val).find("span")[0]).text();
                }
            });

            var lastChar = tabText.substring(tabText.length - 1, tabText.length);
            if (scope == "btnsave") {
                var currTab = $('#tabs').tabs('getSelected');
                var currScope = currTab.find('iframe')[0];
                var contentWidow = currScope.contentWindow;
                contentWidow.isChange = true;
                contentWidow.SaveDtlData();
                $.each(p, function (i, val) {
                    if (val.className == "tabs-selected") {
                        if (lastChar == "*") {
                            $($(val).find("span")[0]).text(tabText.substring(0, tabText.length - 1));
                        }
                        else {
                            $($(val).find("span")[0]).text(tabText.substring(0, tabText.length ));
                        }
                    }
                });
                return;
            }
            if (lastChar == "*") {
                if (typeof scope == "undefined") {
                    var currTab = $('#tabs').tabs('getSelected');
                    scope = currTab.find('iframe')[0];
                }
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
            this.FK_Node = Application.common.getArgsFromHref("FK_Node");
            if (this.FK_Node) {
                while (this.FK_Node.substring(0, 1) == '0') this.FK_Node = this.FK_Node.substring(1);
                this.FK_Node = this.FK_Node.replace('#', '');
            }
            this.NodeID = Application.common.getArgsFromHref("NodeID");
            this.UserNo = Application.common.getArgsFromHref("UserNo");
            this.FID = Application.common.getArgsFromHref("FID");
            this.SID = Application.common.getArgsFromHref("SID");

            this.PWorkID = Application.common.getArgsFromHref("PWorkID");
            this.PFlowNo = Application.common.getArgsFromHref("PFlowNo");

            this.DoFunc = Application.common.getArgsFromHref("DoFunc");
            this.CFlowNo = Application.common.getArgsFromHref("CFlowNo");
            this.WorkIDs = Application.common.getArgsFromHref("WorkIDs");

            this.IsLoadData = Application.common.getArgsFromHref("IsLoadData");
            this.Paras = Application.common.getArgsFromHref("Paras");
            this.AtPara = Application.common.getArgsFromHref("AtPara");
            this.IsCC = "0";
            if (this.Paras && this.Paras.indexOf("@IsCC") >= 0) {
                this.IsCC = "1";
                this.IsLoadData = "0";
            }
            if (this.AtPara && this.AtPara.indexOf("@IsCC") >= 0) {
                this.IsCC = "1";
                this.IsLoadData = "0";
            }
        }
        //传参
        var urlExtFrm = function () {
            var extUrl = "";
            var args = new RequestArgs();
            if (args.WorkID != "")
                extUrl += "&WorkID=" + args.WorkID;
            if (args.FK_Flow != "")
                extUrl += "&FK_Flow=" + args.FK_Flow;
            if (args.FK_Node != "")
                extUrl += "&FK_Node=" + args.FK_Node;
            if (args.NodeID != "")
                extUrl += "&NodeID=" + args.NodeID;
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
            if (args.IsLoadData != "") {
                extUrl += "&IsLoadData=" + args.IsLoadData;
            }

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
                case "send": //发送
                    if (confirm("是否真的需要提交发送?")) {
                        //发送按钮置为不可用
                        $('#send').linkbutton({ disabled: true });
                        $('.tabs-inner span').each(function (i, n) {
                            var t = $(n).text();
                            if (t != '流程日志') {
                                var lastChar = t.substring(t.length - 1, t.length);
                                if (lastChar == "*") {
                                    t = t.substring(0, t.length - 1);
                                    $('#tabs').tabs('select', t);

                                    var tab = $('#tabs').tabs('getTab', t);
                                    var currScope = tab.find('iframe')[0];
                                    var contentWidow = currScope.contentWindow;
                                    contentWidow.isChange = true;
                                    contentWidow.SaveDtlData();
                                }
                                //$('#tabs').tabs('close', t);
                            }
                        });
                        //接收人规则检查
                        Application.data.checkAccepter(args.FK_Node, args.WorkID, args.FID, function (js) {
                            if (js == "byselected") {/*有用户选择接收人*/
                                var url = "../WorkOpt/Accepter.htm?WorkID=" + args.WorkID + "&FK_Node=" + args.FK_Node + "&FK_Flow=" + args.FK_Flow + "&FID=" + args.FID + "&type=1";
                                var isChrome = window.navigator.userAgent.indexOf("Chrome") !== -1;
                                if (isChrome) {
                                    $("<div id='selectaccepter'></div>").append($("<iframe width='100%' height='100%' frameborder=0 src='" + url + "'/>")).dialog({
                                        title: "选择接受人",
                                        width: 800,
                                        height: 630,
                                        autoOpen: true,
                                        modal: true,
                                        resizable: false,
                                        onClose: function () {
                                            $('#send').linkbutton({ disabled: false });
                                            $("#selectaccepter").remove();
                                            closeWin();
                                        },
                                        buttons: [{
                                            text: '确定',
                                            iconCls: 'icon-ok',
                                            handler: function () {
                                                SendCase();
                                                $('#selectaccepter').dialog("close");
                                            }
                                        }]
                                    });
                                } else {
                                    window.showModalDialog(url, "_blank", "scrollbars=yes;resizable=yes;center=yes;dialogWidth=700px;dialogHeight=600px;");
                                    SendCase();
                                }
                            } else if (js == "byuserselected") {/*有用户选择方向*/
                                $("<div id='selectToNode'></div>").append($("<iframe width='100%' height='100%' frameborder=0 src='../WorkOpt/ToNodes.aspx?WorkID=" + args.WorkID + "&FK_Node=" + args.FK_Node + "&FK_Flow=" + args.FK_Flow + "&FID=" + args.FID + "&type=1'/>")).dialog({
                                    title: "选择方向",
                                    width: 800,
                                    height: 630,
                                    autoOpen: true,
                                    modal: true,
                                    resizable: false,
                                    onClose: function () {
                                        $('#send').linkbutton({ disabled: false });
                                        $("#selectToNode").remove();
                                        closeWin();
                                    },
                                    buttons: [{
                                        text: '关闭',
                                        iconCls: 'icon-cancel',
                                        handler: function () {
                                            $('#selectToNode').dialog("close");
                                            closeWin();
                                        }
                                    }]
                                });
                            } else if (js.indexOf('error:') > 0) {
                                $('#content').html(js);
                                //弹出窗体
                                $('#msgPanel').dialog({
                                    title: "系统消息",
                                    width: 600,
                                    height: 460,
                                    closed: false,
                                    modal: true,
                                    iconCls: 'icon-save',
                                    resizable: true,
                                    onClose: function () {
                                        $('#content').html("");
                                        $('#send').linkbutton({ disabled: false });
                                    },
                                    buttons: [{
                                        text: '确定',
                                        iconCls: 'icon-ok',
                                        handler: function () {
                                            $('#send').linkbutton({ disabled: false });
                                            $('#msgPanel').dialog("close");
                                        }
                                    }]
                                });
                            }
                            else {
                                SendCase();
                            }
                        }, this);
                    }
                    break;
                case "save": //保存
                    OnTabChange("btnsave");
                    Application.data.saveBlank(args.FK_Flow, args.FK_Node, args.WorkID, function (data) {

                        if (data.indexOf("true") == 0) {
                            //$.messager.alert('提示', '保存成功！', 'info');
                        }
                        else
                            $.messager.alert('提示', data, 'info');
                    }, this);

                    break;
                case "backcase": //退回
                    var strTimeKey = "";
                    var date = new Date();
                    strTimeKey += date.getFullYear(); //年
                    strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
                    strTimeKey += date.getDate(); //日
                    strTimeKey += date.getHours(); //HH
                    strTimeKey += date.getMinutes(); //MM
                    strTimeKey += date.getSeconds(); //SS

                    window.open("../WorkOpt/ReturnWork.aspx?FK_Node=" + args.FK_Node + "&FID=" + args.FID + "&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow + "&s=" + strTimeKey, "退回窗口", "height=400, width=800,top=80,left=160,scrollbars=yes");
                    break;
                case "selectaccepter": //选择接收人
                    window.open("../WorkOpt/Accepter.htm?WorkID=" + args.WorkID + "&FK_Node=" + args.FK_Node + "&FK_Flow=" + args.FK_Flow + "&FID=" + args.FID + "&type=1", "选择收件人", "height=600, width=800,scrollbars=yes");
                    break;
                case "showchart": //轨迹
                    WinOpenPage("_blank", "../WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow + "&FID=" + args.FID + "&FK_Node=" + args.FK_Node + "&s=" + strTimeKey, "轨迹图");
                    break;
                case "childline": //子线程
                    window.open("../WorkOpt/ThreadDtl.aspx?FK_Node=" + args.FK_Node + "&FID=" + args.FID + "&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow + "&s=" + strTimeKey, "查看子线程", "height=600, width=800,scrollbars=yes");
                    break;
                case "CC": //抄送
                    window.open("../WorkOpt/CC.aspx?WorkID=" + args.WorkID + "&FK_Node=" + args.FK_Node + "&FK_Flow=" + args.FK_Flow + "&FID=" + args.FID + "&s=" + strTimeKey, "抄送", "height=600, width=800,scrollbars=yes");
                    break;
                case "Shift": //移交
                    window.open("../WorkOpt/Forward.aspx?FK_Node=" + args.FK_Node + "&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow, "移交窗口", "height=600, width=800,scrollbars=yes");
                    break;
                case "Del": //删除
                    if (confirm("是否真的需要删除?"))
                        DelCase();
                    break;
                case "Sign": //签名
                    SignCase();
                    break;
                case "endflow": //结束
                    Endflow();
                    break;
                case "workcheck": //审核
                    window.open("../WorkOpt/WorkCheck.aspx?FK_Node=" + args.FK_Node + "&FID=" + args.FID + "&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow + "&s=" + strTimeKey, "审核", "height=600, width=800,scrollbars=yes");
                    break;
                case "askfor": //加签
                    window.open("../WorkOpt/Askfor.aspx?FK_Node=" + args.FK_Node + "&FID=" + args.FID + "&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow + "&s=" + strTimeKey, "加签", "height=600, width=800,scrollbars=yes");
                    break;
                case "printdoc": //打印单据
                    if (args.WorkID == 0) {
                        $.messager.alert('提示', '没有指定的工作，不能打印！', 'info');
                        return;
                    }
                    window.showModalDialog("../CCForm/Print.aspx?FK_Node=" + args.FK_Node + "&FID=" + args.FID + "&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow + "&s=" + strTimeKey, "打印单据", "dialogHeight: 350px; dialogWidth:450px; center: yes; help: no");
                    //window.open("../CCForm/Print.aspx?FK_Node=" + args.FK_Node + "&FID=" + args.FID + "&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow + "&s=" + strTimeKey, "打印单据", "dialogHeight: 350px, dialogWidth:450px,center: yes, help: no,resizable:yes");
                    break;
                case "hungup": //挂起
                    window.open("../WorkOpt/HungUp.aspx?FK_Node=" + args.FK_Node + "&FID=" + args.FID + "&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow + "&s=" + strTimeKey, "挂起", "height=600, width=800,scrollbars=yes");
                    break;
                case "search": //查询
                    WinOpenPage("_blank", "../Rpt/Search.aspx?RptNo=ND" + parseInt(args.FK_Flow) + "MyRpt&FK_Flow=" + args.FK_Flow + "&s=" + strTimeKey, "查询");
                    break;
                case "batchworkcheck": //批量审核
                    window.open("../Batch.htm?FK_Node=" + args.FK_Node + "&FID=" + args.FID + "&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow + "&s=" + strTimeKey, "批量审核", "height=600, width=800,scrollbars=yes");
                    break;
                case "childline": //子线程
                    window.open("../WorkOpt/ThreadDtl.aspx?FK_Node=" + args.FK_Node + "&FID=" + args.FID + "&WorkID=" + args.WorkID + "&FK_Flow=" + args.FK_Flow + "&s=" + strTimeKey, "子线程", "height=600, width=800,scrollbars=yes");
                    break;
                case "jumpway": //跳转
                    window.open("../JumpWay.aspx?FK_Node=" + args.FK_Node + "&FID=" + args.FID + "&WorkID=" + args.WorkID + "&FK_Flow" + args.FK_Flow + "&s=" + strTimeKey, "跳转", "height=600, width=800,scrollbars=yes");
                    break;
                case "Shift": //移交
                    window.open("../WorkOpt/Forward.aspx?WorkID=" + args.WorkID + "&FK_Node=" + args.FK_Node + "&FK_Flow=" + args.FK_Flow + "&FK_Dept=" + args.FK_Dept, "移交", "height=600, width=800,scrollbars=yes");
                    break;
                case "CCCheckNote"://抄送
                    window.open("../WorkOpt/CCCheckNote.aspx?WorkID=" + args.WorkID + "&FK_Node=" + args.FK_Node + "&FK_Flow=" + args.FK_Flow + "&FID=" + args.FID, "抄送", "height=600, width=800,scrollbars=yes");
                    break;
                case "closeWin":
                    closeWin();
                    break;
                case "": //备份数据
            }
        }
        //打印
        function PrintForm() {

            //printPanel
        }
        //结束
        function Endflow() {
            var args = new RequestArgs();
            Application.data.endCase(args.FK_Flow, args.FK_Node, args.WorkID, function (js) {
                //  $.messager.alert('提示', '流程结束！', 'info');
                closeWin();
            }, this);

        }
        //删除
        function DelCase() {
            var args = new RequestArgs();
            var strTimeKey = "";
            var date = new Date();
            strTimeKey += date.getFullYear(); //年
            strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
            strTimeKey += date.getDate(); //日
            strTimeKey += date.getHours(); //HH
            strTimeKey += date.getMinutes(); //MM
            strTimeKey += date.getSeconds(); //SS

            Application.data.delcase(args.FK_Flow, args.FK_Node, args.WorkID, args.FID, function (js) {
                if (js) {
                    var str = js;
                    if (str == "删除成功") {
                        $("<div id='deleteresultinfo'></div>").append($("<div style='margin-left:5px; margin-top:5px;'>删除成功！</div>")).dialog({
                            title: "提示",
                            width: 200,
                            height: 105,
                            autoOpen: true,
                            modal: true,
                            resizable: false,
                            onClose: function () {
                                $("#deleteresultinfo").remove();
                                window.returnValue = "true";
                                closeWin();
                            },
                            buttons: [{
                                text: '确定',
                                iconCls: 'icon-ok',
                                handler: function () {
                                    $('#deleteresultinfo').dialog("close");
                                }
                            }]
                        });
                    }
                    else {
                        window.open("/" + str + strTimeKey, "删除窗口", " height=500, width=400,scrollbars=yes");
                    }
                }
            }, this);
        }
        //签名
        function SignCase() {
            var args = new RequestArgs();
            //意见窗体
            $('#yjPanel').dialog({
                title: "输入意见",
                width: 307,
                height: 180,
                closed: false,
                modal: true,
                iconCls: 'icon-save',
                resizable: true,
                buttons: [{
                    text: '签名',
                    iconCls: 'icon-ok',
                    handler: function () {
                        var yj = escape(document.getElementById("YJ").value);
                        Application.data.signcase(args.FK_Flow, parseInt(args.FK_Node), args.WorkID, args.FID, yj, function (js) {
                            if (js) $.messager.alert('提示', js + '！');
                        }, this);
                        $('#yjPanel').dialog("close");
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        $('#yjPanel').dialog("close");
                    }
                }]
            });
        }
        //发送
        function SendCase() {
            var args = new RequestArgs();
            Application.data.sendCase(args.FK_Flow, args.FK_Node, args.WorkID, args.DoFunc, args.CFlowNo, args.WorkIDs, function (js) {
                var strs = new Array();
                strs = js.split("|");

                //转向页面
                if (strs[0] == "SpecUrl") {
                    var url = strs[1];
                    location.href = url;
                    //                    $("<div id='selectaccepter'></div>").append($("<iframe width='100%' height='100%' frameborder=0 src='" + url + "'/>")).dialog({
                    //                        title: "发送成功-转向页面",
                    //                        width: 800,
                    //                        height: 630,
                    //                        autoOpen: true,
                    //                        modal: true,
                    //                        resizable: false,
                    //                        onClose: function () {
                    //                            $('#send').linkbutton({ disabled: false });
                    //                            $("#selectaccepter").remove();
                    //                            closeWin();
                    //                        },
                    //                        buttons: [{
                    //                            text: '确定',
                    //                            iconCls: 'icon-ok',
                    //                            handler: function () {
                    //                                $('#selectaccepter').dialog("close");
                    //                            }
                    //                        }]
                    //                    });
                    return;
                }

                $('#content').html(strs[1]);
                //弹出窗体
                $('#msgPanel').dialog({
                    title: "系统消息",
                    width: 600,
                    height: 460,
                    closed: false,
                    modal: true,
                    iconCls: 'icon-save',
                    resizable: true,
                    onClose: function () {
                        if (strs[0] == "success") {
                            window.returnValue = "true";
                            closeWin();
                        }
                        $('#send').linkbutton({ disabled: false });
                    },
                    buttons: [{
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            if (strs[0] == "success") {
                                window.returnValue = "true";
                                closeWin();
                            }
                            if (strs[0] == "error" || strs[0] == "sysError") {
                                $('#send').linkbutton({ disabled: false });
                                $('#msgPanel').dialog("close");
                            }
                        }
                    }]
                });
            }, this);
        }
        //发送到指定节点
        function SendCaseToNode() {
            var args = new RequestArgs();
            var toNode = document.getElementById("nextNodes").value;
            Application.data.sendCaseToNode(args.FK_Flow, args.FK_Node, args.WorkID, args.DoFunc, args.CFlowNo, args.WorkIDs, toNode, function (js) {
                var strs = new Array();
                strs = js.split("|");

                $('#content').html(strs[1]);
                //弹出窗体
                $('#msgPanel').dialog({
                    title: "系统消息",
                    width: 600,
                    height: 460,
                    closed: false,
                    modal: true,
                    iconCls: 'icon-save',
                    resizable: true,
                    onClose: function () {
                        if (strs[0] == "success") {
                            window.returnValue = "true";
                            closeWin();
                        }
                        $('#send').linkbutton({ disabled: false });
                    },
                    buttons: [{
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            if (strs[0] == "success") {
                                window.returnValue = "true";
                                closeWin();
                            }
                            if (strs[0] == "error" || strs[0] == "sysError") {
                                $('#send').linkbutton({ disabled: false });
                                $('#msgPanel').dialog("close");
                            }
                        }
                    }]
                });
            }, this);
        }

        //撤销发送
        function UnSend() {
            if (!confirm("您确定要撤销本次发送吗?")) return;
            var args = new RequestArgs();
            Application.data.unSendCase(args.FK_Flow, args.WorkID, function (js) {
                if (js == "true") {
                    alert("撤销成功！");
                    $('#msgPanel').dialog("close");
                } else {
                    $.ligerDialog.alert(js);
                }
            }, this);
        }

        //选择人接收器选人后
        function AccepterHtmlSave() {
            SendCase();
            $('#selectaccepter').dialog("close");
        }

        //页面初始
        $(function () {
            $("#pageloading").show();
            //初始工具栏
            var args = new RequestArgs();
            var i = 0;
            //表单树
            var urlExt = urlExtFrm();
            var IsCC = args.IsCC;
            var url = "Base/FormTreeBase.aspx?1=1" + urlExt;
            Application.data.getFlowFormTree(url, function (js) {
                var isSelect = false;
                var pushData = eval('(' + js + ')');
                //pushData = pushData[0].children;
                //加载类别树
                $("#flowFormTree").tree({
                    data: pushData,
                    iconCls: 'tree-folder',
                    collapsed: true,
                    lines: true,
                    formatter: function (node) {
                        if (i == 0) {
                            if (node.attributes.NodeType == "form|0" || node.attributes.NodeType == "form|1") {
                                i++;
                                var isEdit = node.attributes.IsEdit;
                                if (IsCC && IsCC == "1") isEdit = "0";
                                var url = "../CCForm/Frm.htm?FK_MapData=" + node.id + "&IsEdit=" + isEdit + "&IsPrint=0" + urlExt;
                                addTab(node.id, node.text, url);
                            }
                        }
                        return node.text;
                    },
                    onClick: function (node) {
                        if (node.attributes.NodeType == "form|0" || node.attributes.NodeType == "form|1") {/*普通表单和必填表单*/
                            var urlExt = urlExtFrm();
                            var isEdit = node.attributes.IsEdit;
                            if (IsCC && IsCC == "1") isEdit = "0";
                            var url = "../CCForm/Frm.htm?FK_MapData=" + node.id + "&IsEdit=" + isEdit + "&IsPrint=0" + urlExt;
                            addTab(node.id, node.text, url);
                        } else if (node.attributes.NodeType == "tools|0") {/*工具栏按钮添加选项卡*/
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
                        } else if (node.attributes.NodeType == "tools|1") {/*工具栏按钮打开新窗体*/
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
                ResizeWindow();
            }, this);
            var urlExt = urlExtFrm();
            //addTab("trackid", "流程日志", "../WorkOpt/OneWork/TruckOnly.aspx?1=1" + urlExt);
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

        //关闭所有Tab页
        function CloseAllTab() {
            $('.tabs-inner span').each(function (i, n) {
                var t = $(n).text();
                if (t != '流程日志') {
                    $('#tabs').tabs('close', t);                    
                }
            });
        }

        function closeWin() {
            if (window.dialogArguments && window.dialogArguments.window) {
                window.dialogArguments.window.location = window.dialogArguments.window.location;
            }
            if (window.opener) {
                if (window.opener.name && window.opener.name == "main") {
                    window.opener.location.href = window.opener.location.href;
                    if (window.opener.top && window.opener.top.leftFrame) {
                        window.opener.top.leftFrame.location.href = window.opener.top.leftFrame.location.href;
                    }
                } else if (window.opener.name && window.opener.name == "运行流程") { 
                    //测试运行流程，不进行刷新
                } else {
                    //window.opener.location.href = window.opener.location.href;
                }
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
        <div id="mm3" style="width: 150px;" runat="server">
        </div>
    </div>
    <div region="west" border="true" split="true" title=" " class="cs-west">
        <ul id="flowFormTree" class="easyui-tree" data-options="animate:true,dnd:false">
        </ul>
    </div>
    <div id="mainPanle" region="center" border="true" border="false">
        <div id="tabs" class="easyui-tabs" fit="true" border="false" data-options="tools:'#tab-tools'">
        </div>
    </div>
    <div id="msgPanel">
        <div id="content" style="height: 440px; overflow: auto; padding: 10px 10px 10px 10px;">
        </div>
    </div>
    <div id="yjPanel">
        <div>
            <textarea rows="5" name="YJ" id="YJ" cols="38"></textarea></div>
        <select style="width: 290px" id="YJItems" onchange='document.getElementById("YJ").value = document.getElementById("YJItems").value;'>
            <option value="">请选择意见模板</option>
            <option value="已阅">已阅</option>
            <option value="同意">同意</option>
        </select>
    </div>
    <div id="toNode">
        <div style="margin: 10px;">
            请选择：<select id="nextNodes" style="width: 120px;" />
        </div>
    </div>
    <div id="mm" class="easyui-menu cs-tab-menu">
        <div id="mm-tabupdate">刷新</div>
        <div class="menu-sep"></div>
        <div id="mm-tabclose">关闭</div>
        <div id="mm-tabcloseother">关闭其他</div>
        <div id="mm-tabcloseall">关闭全部</div>
    </div>
</body>
</html>
