﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge"> 
    <title>CCBPM设计器</title>
	<link rel="stylesheet" type="text/css" href="normalize/css/demo.css" />
	<link rel="stylesheet" type="text/css" href="normalize/css/ns-default.css" />
	<link rel="stylesheet" type="text/css" href="normalize/css/ns-style-bar.css" />
    <script type="text/javascript" src="normalize/js/modernizr.custom.js"></script>
    <link href="../../Scripts/easyUI/themes/gray/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/gray/dialog.css" rel="stylesheet" type="text/css" />
    <link href="./Img/Menu/Designer.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <script src="lib/browserReady.js" type="text/javascript"></script>
    <script src="js/CCBPMDesignerData.js" type="text/javascript"></script>
    <script src="../../Scripts/Cookie.js" type="text/javascript"></script>
    <script src="js/FuncTrees.js" type="text/javascript"></script>
    <script src="js/Left.js" type="text/javascript"></script>

    <style type="text/css">
    .icon-Search
    {
       background: url('./Img/Search.png') no-repeat;
    }
    .icon-Event
    {
       background: url('../../Img/Event.png') no-repeat;
    }
    .icon-Setting
    {
       background: url('../../Img/Setting.png') no-repeat;
    }
   
    .icon-DTS
    {
       background: url('./Img/DTS.png') no-repeat;
    }
    .icon-URL
    {
       background: url('./Img/URL.png') no-repeat;
    }
    .icon-API
    {
       background: url('./Img/API.png') no-repeat;
    }
     .icon-StartGuide
    {
       background: url('./Img/StartGuide.png') no-repeat;
    }
    .icon-limit
    {
       background: url('./Img/limit.png') no-repeat;
    }
    .icon-AutoStart
    {
       background: url('./Img/AutoStart.png') no-repeat;
    }
    .icon-DesignRpt
    {
       background: url('./Img/DesignRpt.png') no-repeat;
    }
    .icon-Group
    {
       background: url('./Img/Group.png') no-repeat;
    }
    .icon-D3
    {
       background: url('./Img/D3.png') no-repeat;
    }
    .icon-Contrast
    {
       background: url('./Img/Contrast.png') no-repeat;
    }
    .icon-SQL
    {
       background: url('./Img/SQL.png') no-repeat;
    }
    .icon-Field
    {
       background: url('./Img/Field.png') no-repeat;
    }
     .icon-Node
    {
       background: url('./Img/Node.png') no-repeat;
    }
    .icon-FuncFolder
    {
       background: url('./Img/FuncFolder.png') no-repeat;
    }
     .icon-Monitor
    {
       background: url('./Img/Monitor.png') no-repeat;
    }
      .icon-SearchKey
    {
       background: url('./Img/SearchKey.png') no-repeat;
    }
    .icon-app
    {
       background: url('./Img/app.png') no-repeat;
    }
    .icon-flows
    {
       background: url('./Img/flows.png') no-repeat;
    }
    .icon-RptOrder
    {
       background: url('./Img/RptOrder.png') no-repeat;
    }
    .icon-Grow
    {
       background: url('./Img/Grow.png') no-repeat;
    }
     .icon-Warning
    {
       background: url('./Img/Warning.png') no-repeat;
    }
       .icon-overtime
    {
       background: url('./Img/overtime.png') no-repeat;
    }
      .icon-log
    {
       background: url('./Img/log.png') no-repeat;
    }
    
    .icon-flowcloud
{
    background: url('./Img/FlowCloud.png') no-repeat;
    width: 16px;
    height: 16px;
    -webkit-background-size: cover;
    -moz-background-size: cover;
    -o-background-size: cover;
}
    .icon-Home
    {
       background: url('../CCFormDesigner/Img/Home.png') no-repeat;
    }
    .icon-Glasses
    {
       background: url('../CCFormDesigner/Img/Glasses.png') no-repeat;
    }
      .icon-JavaScript
    {
       background: url('../CCFormDesigner/Img/JavaScript.png') no-repeat;
    }
    
    .icon-LoadFull
    {
       background: url('../CCFormDesigner/Img/LoadFull.png') no-repeat;
    }
    
    
   .icon-Menu
   {
      background: url('../../../GPM/Images/Menu.png');
    }

   .icon-User
   {
      background: url('../../../GPM/Images/User.png');
    }
    .icon-Station
   {
      background: url('../../../GPM/Images/Station.png');
    }
        .icon-Group
   {
      background: url('../../../GPM/Images/Group.png');
    }

    
    </style>
    <script language="javascript" type="text/javascript">
        osModel = '<%=BP.Sys.SystemConfig.OSModel %>';

        $(function () {
            var content = createFrame('Welcome.aspx');
            $('#tabs').tabs('add', {
                title: '首页',
                id: 'WelCome',
                content: content,
                iconCls: '',
                closable: false
            });
        });
        function closeTab(title) {
            $('#tabs').tabs('close', title);
        }
        function addTab(id, title, url, iconCls, refresh) {
            //此处为适应原有GPM中的编辑系统菜单页面的打开新tab，那个只传了2个参数addTab(title,url)，edited by liuxc,2015-11-05
            if (arguments.length < 3) {
                url = title;
                title = id;
                id = Math.random().toString();

                if (url.indexOf('AppMenu.aspx') == 0) {
                    url = '../../../GPM/' + url;
                } 
            }

            if ($('#tabs').tabs('exists', title)) {
                $('#tabs').tabs('select', title);
                var currTab = $('#tabs').tabs('getSelected').panel('options');

                if (currTab.id != id) {
                    $('#tabs').tabs('update', {
                        tab: $('#tabs').tabs('getSelected'),
                        options: {
                            id: id,
                            content: createFrame(url)
                        }
                    });
                }
                //此处暂时屏掉刷新当前已经打开的页，发现如果切换非当前的已经打开的流程设计图时，会导致IE崩溃
//                else {
//                    var iwin = $("#tabs div[id='" + id + "']").find("iframe")[0].contentWindow;
//                    if (iwin.location.href != url) {
//                        iwin.location.href = url;
//                    }
//                    else {
//                        iwin.location.reload();  //此处重新刷新当前页面
//                    }
//                }
            } else {
                var content = createFrame(url);
                $('#tabs').tabs('add', {
                    title: title,
                    id: id,
                    content: content,
                    iconCls: iconCls,
                    closable: true
                });
            }
            tabClose();
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

        var arrayCloseObj = new Array();
        //标签关闭前事件
        function EventListener_TabClose(title, index) {
            var disTab = $('#tabs').tabs('getTab', index);
            var curTab_Id = disTab.panel('options').id;
            curTab_Id = Number(curTab_Id);
            curTab_Id = String(curTab_Id);
            var tabs = $('#tabs').tabs('tabs');
            for (var i = 0, j = tabs.length; i < j; i++) {
                var othTab_Id = tabs[i].panel('options').id;
                var othTab_Title = tabs[i].panel('options').title;
                if (othTab_Id.length > curTab_Id.length && othTab_Id.substring(0, curTab_Id.length) == curTab_Id) {
                    arrayCloseObj.push(othTab_Title);
                }
            }
        }

        function EventListener_TabCloseed() {
            for (var k = 0; k < arrayCloseObj.length; k++) {
                $('#tabs').tabs('close', arrayCloseObj[k]);
            }
            var tabs = $('#tabs').tabs('tabs');
            if (tabs.length == 0) {
                addTab("welcome", "首页", "WelCome.html", "");
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
            arrayCloseObj.length = 0;
            tabCloseEven();
        }
        //绑定右键菜单事件
        function tabCloseEven() {
            //刷新
            $('#mm-tabupdate').click(function () {
                var currTab = $('#tabs').tabs('getSelected');
                var url = $(currTab.panel('options').content).attr('src');
                if (url != undefined) {
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
                    return false;
                }
                prevall.each(function (i, n) {
                    var t = $('a:eq(0) span', $(n)).text();
                    $('#tabs').tabs('close', t);
                });
                return false;
            });
        }

        //登录
        function Login2App() {
            var url = "../../App/Classic/Login.aspx?DoType=Logout";
            window.open(url);
        }
        //退出
        function LoginOut() {
            $.messager.confirm("提示", "确定需要退出？", function (n) {
                if (n == true) {
                    window.location.href = "Login.aspx?DoType=Logout";
                }
            });
        }
        function BPMN_Msg(msg, callBack) {
            // create the notification
            var notification = new NotificationFx({
                message: '<span class="icon icon-megaphone"></span><div class="ns-p"><p>' + msg + '</p></div>',
                layout: 'bar',
                effect: 'slidetop',
                type: 'notice', // notice, warning or error
                onClose: function () {
                    if (callBack) callBack();
                }
            });
            notification.show();
        }
    </script>
    <style type="text/css">
        .logpanel
        {
            height:25px;
        }
        .icon-logPic
        {
	        background:url('Icons/iconsmal.png') no-repeat;
	        background-size:45px 30px; 
	        height:28px;
	        width:45px;
	        float:left;
	        display:inline;
        }
        .logPicspan
        {
            margin-left:15px;
            margin-top:6px;
            font-size:large;
            float:left;
	        display:inline;
        }
    </style>
</head>
<body class="easyui-layout" data-options="fit: true">
    <div class="panel window mymask mymaskContainer" style="display: block; width: 32px;
        height: 32px; z-index: 21008; text-align: center; font-size: 14px; font-weight: bold;">
        <img alt="" src="assets/images/loading_small.gif" align='middle' style="width: 32px;
            height: 32px;" />
    </div>
    <div class="window-mask mymask" style="width: 100%; height: 100%; display: block;
        z-index: 21006;">
    </div>
    <div data-options="region:'west',border:true,split:true" title="<div class='logpanel'><div class='icon-logPic'></div><div class='logPicspan'>驰骋BPM</div></div>" style="width: 280px;">
        <div id="menuTab" class="easyui-tabs" data-options="fit:true,border:false">
        </div>
    </div>
    <div data-options="region:'center',border:true" collapsible="true">
        <script type="text/javascript" language="javascript">
            if (!isBrowserReady()) {
                document.write('<span style="background-color: red;" >');
                document.write("您的浏览器不支持HTML5。请升级您的浏览器到高级版本，或者使用火狐、谷歌浏览器。");
                document.write("</span>");
            }
        </script>
        <div id="tabs" class="easyui-tabs" fit="true" border="false" data-options="tools:'#tab-tools',onBeforeClose:EventListener_TabClose,onClose:EventListener_TabCloseed">
        </div>
        <div id="tab-tools">
            <a href="#" class="easyui-linkbutton" data-options="plain:true" onclick="Login2App()"><b style="color: Blue;"><img alt='使用普通用户,登录前台demo系统' src='/WF/App/Classic/Img/Key.png' style="width:14px;height:14px;"/>登录前台</b></a> 
            <a href="#" class="easyui-linkbutton" data-options="plain:true" onclick="LoginOut()"><b style="color: Blue;"><img alt='退出ccbpm流程设计器' src='/WF/Img/Exit.png' style="width:14px;height:14px;"/>退出</b></a>
        </div>
    </div>
    <!--  context menu -->
    <div id="mFlowRoot" class="easyui-menu" style="width: 120px;">
        <div onclick="newFlow()" data-options="iconCls:'icon-add'">新建流程</div>

        <div onclick="SearchFlow()" data-options="iconCls:'icon-Search'">按关键字查询流程</div>

        <div onclick="newFlowSort(true)" data-options="iconCls:'icon-new'">
            新建子级类别</div>
        <div onclick="editFlowSort()" data-options="iconCls:'icon-edit'">
            编辑</div>
        <div onclick="RefreshNode('flowTree', 'F99')" data-options="iconCls:'icon-reload'">
            刷新</div>
    </div>
    <div id="mFlowSort" class="easyui-menu" style="width: 120px;">
        <div onclick="newFlow()" data-options="iconCls:'icon-add'">新建流程</div>


        <div onclick="ImpFlowBySort()" data-options="iconCls:'icon-redo'">导入流程模板</div>
        <div onclick="ExpFlowBySort()" data-options="iconCls:'icon-unredo'">导出流程模板</div>


       <%-- <div onclick="ImpLocalFlow()" data-options="iconCls:'icon-imp'">导入本机流程模版</div>
        <div onclick="ImpColudFlow()" data-options="iconCls:'icon-imp'">导入ccbpm云服务流程模版</div>--%>

        <div onclick="newFlowSort(false)" data-options="iconCls:'icon-new'">
            新建同级类别</div>
        <div onclick="newFlowSort(true)" data-options="iconCls:'icon-new'">
            新建子级类别</div>
        <div onclick="editFlowSort()" data-options="iconCls:'icon-edit'">
            编辑</div>
        <div onclick="deleteFlowSort()" data-options="iconCls:'icon-delete'">
            删除</div>
        <div onclick="RefreshNode('flowTree', 'F99')" data-options="iconCls:'icon-reload'">
            刷新</div>
    </div>
    <div id="mFlow" class="easyui-menu" style="width: 120px;">
      

        <div onclick="showFlow()" data-options="iconCls:'icon-design'">设计流程图</div>

        <div onclick="ImpFlow()" data-options="iconCls:'icon-redo'">导入流程模板</div>
        <div onclick="ExpFlow()" data-options="iconCls:'icon-unredo'">导出流程模板</div>

        <div onclick="DeleteFlow()" data-options="iconCls:'icon-delete'">删除流程</div>
        <div onclick="FlowProperty()" data-options="iconCls:'icon-sheet'">流程属性</div>
        
        <div onclick="newFlow()" data-options="iconCls:'icon-add'">新建流程</div>
        <div onclick="ImpLocalFlow()" data-options="iconCls:'icon-imp'">导入本机流程模版</div>
        <div onclick="ImpColudFlow()" data-options="iconCls:'icon-imp'">导入ccbpm云服务流程模版</div>

    </div>
    <div id="mFormRoot" class="easyui-menu" style="width: 120px;">
        <div onclick="newSrc()" data-options="iconCls:'icon-add'">
            新建数据源</div>
        <div onclick="newFrm()" data-options="iconCls:'icon-new'">
            新建表单</div>
        <div onclick="RefreshNode('formTree', '0')" data-options="iconCls:'icon-reload'">
            刷新</div>
    </div>
    <div id="mFormSrc" class="easyui-menu" style="width: 120px;">
        <div onclick="remove()" data-options="iconCls:'icon-add'">
            新建下级目录</div>
        <div onclick="newFrm()" data-options="iconCls:'icon-new'">
            新建表单</div>
        <div onclick="newSrc()" data-options="iconCls:'icon-add'">
            新建数据源</div>
    </div>
    <div id="mFormSort" class="easyui-menu" style="width: 120px;">
        <div onclick="newFrm()" data-options="iconCls:'icon-add'">
            新建/导入</div>
        <div onclick="remove()" data-options="iconCls:'icon-new'">
            新建同级类别</div>
        <div onclick="remove()" data-options="iconCls:'icon-new'">
            新建子级类别</div>
        <div onclick="remove()" data-options="iconCls:'icon-edit'">
            编辑</div>
        <div onclick="remove()" data-options="iconCls:'icon-delete'">
            删除</div>
        <div onclick="RefreshNode('formTree', '0')" data-options="iconCls:'icon-reload'">
            刷新</div>
    </div>
    <div id="mForm" class="easyui-menu" style="width: 120px;">
        <div onclick="designFrm()" data-options="iconCls:'icon-open'">
            设计表单</div>
        <div onclick="remove()" data-options="iconCls:'icon-property'">
            表单属性</div>
        <div onclick="remove()" data-options="iconCls:'icon-delete'">
            删除</div>
        <%--<div onclick="remove()" data-options="iconCls:'icon-refresh'">
            刷新</div>--%>
    </div>
    <div id="mSrcRoot" class="easyui-menu" style="width: 120px;">
        <div onclick="newSrc()" data-options="iconCls:'icon-add'">
            新建数据源</div>
        <div onclick="RefreshNode('formTree', 'SrcRoot')" data-options="iconCls:'icon-reload'">
            刷新</div>
    </div>
    <div id="mSrc" class="easyui-menu" style="width: 120px;">
        <div onclick="srcProperty()" data-options="iconCls:'icon-sheet'">
            数据源属性</div>
        <div onclick="newSrc()" data-options="iconCls:'icon-add'">
            新建数据源</div>
        <div onclick="newSrcTable()" data-options="iconCls:'icon-new'">
            新建表</div>
        <div onclick="alert('删除数据源，待开发')" data-options="iconCls:'icon-delete'">
            删除数据源</div>
        <div onclick="RefreshNode('formTree', 'SrcRoot')" data-options="iconCls:'icon-reload'">
            刷新</div>
    </div>
    <div id="mSrcTable" class="easyui-menu" style="width: 120px;">
        <div onclick="srcTableProperty()" data-options="iconCls:'icon-sheet'">
            表属性</div>
        <div onclick="newSrcTable()" data-options="iconCls:'icon-new'">
            新建表</div>
        <div onclick="srcTableData()" data-options="iconCls:'icon-edit'">
            查看&编辑数据</div>
        <div onclick="alert('删除表，待开发')" data-options="iconCls:'icon-delete'">
            删除表</div>
    </div>
    <div id="mm" class="easyui-menu cs-tab-menu">
        <div id="mm-tabupdate">刷新</div>
        <div class="menu-sep"></div>
        <div id="mm-tabclose">关闭</div>
        <div id="mm-tabcloseother">关闭其他</div>
        <div id="mm-tabcloseall">关闭全部</div>
        <div id="mm-tabcloseright">关闭右侧</div>
        <div id="mm-tabcloseleft">关闭左侧</div>
    </div>
    <div id="mDeptRoot" class="easyui-menu" style="width: 120px;">
        <div onclick="newSubDept()" data-options="iconCls:'icon-new'">
            增加下级部门</div>
        <div onclick="editDept()" data-options="iconCls:'icon-edit'">
            编辑部门</div>
        <div class="menu-sep"></div>
        <div onclick="newStation()" data-options="iconCls:'icon-add'">
            增加岗位</div>
    </div>
    <div id="mDept" class="easyui-menu" style="width: 120px;">
        <div onclick="newSubDept()" data-options="iconCls:'icon-new'">
            增加下级部门</div>
        <div onclick="newDept()" data-options="iconCls:'icon-new'">
            增加同级部门</div>
        <div onclick="editDept()" data-options="iconCls:'icon-edit'">
            编辑部门</div>
        <div onclick="deleteDept()" data-options="iconCls:'icon-delete'">
            删除部门</div>
        <div class="menu-sep"></div>
        <div onclick="newStation()" data-options="iconCls:'icon-add'">
            增加岗位</div>
    </div>
    <div id="mStation" class="easyui-menu" style="width: 120px;">
        <div onclick="editStation()" data-options="iconCls:'icon-edit'">
            编辑岗位</div>
        <div onclick="deleteStation()" data-options="iconCls:'icon-delete'">
            删除岗位</div>
        <div class="menu-sep"></div>
        <div onclick="newEmp()" data-options="iconCls:'icon-add'">
            增加人员</div>
    </div>
    <div id="mEmp" class="easyui-menu" style="width: 120px;">
        <div onclick="editEmp()" data-options="iconCls:'icon-edit'">
            编辑人员</div>
        <div onclick="deleteEmp()" data-options="iconCls:'icon-delete'">
            删除人员</div>
    </div>
    <script src="normalize/js/classie.js"></script>
    <script src="normalize/js/notificationFx.js"></script>
</body>
</html>
