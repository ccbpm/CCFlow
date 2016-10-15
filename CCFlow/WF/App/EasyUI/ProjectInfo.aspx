<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProjectInfo.aspx.cs" Inherits="CCFlow.AppDemoLigerUI.ProjectInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/WF/Comm/JS/EasyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="/WF/Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="/WF/Comm/JS/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="/WF/Comm/JS/EasyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="/WF/Comm/JS/EasyUI/locale/easyui-lang-zh_CN.js" type="text/javascript"
        charset="UTF-8"></script>
    <script language="javascript" type="text/javascript">
        //打开窗体
        function WinOpenIt(url, workId, text) {
            
            var winWidth = 850;
            var winHeight = 680;
            if (screen && screen.availWidth) {
                winWidth = screen.availWidth;
                winHeight = screen.availHeight - 36;
            }
            try {
                var vReturnValue = window.showModalDialog(url, "_blank", "scrollbars=yes;resizable=yes;center=yes;dialogWidth=" + winWidth + ";dialogHeight=" + winHeight + ";dialogTop=0px;dialogLeft=0px;");
            } catch (ex) {
                window.open(url, "_blank", "height=" + winHeight + ",width=" + winWidth + ",top=0,left=0,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no");
            }
            LoadEmpWorkData();
        }
        //打开抄送窗体
        function WinOpenCCIt(ccid, fk_flow, fk_node, workid, fid, sta) {
            var url = '';
            url = '../WF/Do.aspx?DoType=DoOpenCC&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&WorkID=' + workid + '&FID=' + fid + '&Sta=' + sta + '&MyPK=' + ccid;
            window.open(url, 'z');
            LoadCCFowData();
        }
        //撤销发送
        function UnSend(fkFlow, workId) {
            $.messager.confirm('提示', '您确定要撤销本次发送吗？', function (yes) {
                if (yes) {
                    Application.data.unSend(fkFlow, workId, function (js) {
                        if (js) {
                            //                    var msg = eval('(' + js + ')');
                            //$.ligerDialog.alert(msg.message);
                            $.messager.alert('提示', js);
                            LoadGrid();
                        }
                    });
                }
            });
        }
        //催办
        function Press(url) {
            window.showModalDialog(url, 'sd', 'dialogHeight: 200px; dialogWidth: 400px;center: yes; help: no;');
        }
        function winOpen(url) {
            window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
        }
        //加载grid数据
        function LoadDataGrid(gridData, scope) {
            if (gridData) {
                if (gridData == "") gridData = "[]";
                var pushData = eval('(' + gridData + ')');
                var fitColumns = true;
                if (scope.columns.length > 7) {
                    fitColumns = false;
                }

                $('#ensGrid').datagrid({
                    columns: [scope.columns],
                    data: pushData,
                    width: 'auto',
                    height: 'auto',
                    striped: true,
                    rownumbers: true,
                    singleSelect: true,
                    pagination: false,
                    remoteSort: false,
                    fitColumns: fitColumns,
                    onDblClickCell: function (index, field, value) {

                    },
                    loadMsg: '数据加载中......'
                });
            }
            else {
                ShowHomePage();
                $.messager.alert('提示', '没有获取到数据！', 'info');
            }
        }
        //待办
        function LoadEmpWorkData() {
            this.columns = [
                   { title: '标题', field: 'Title', width: 260, align: 'left', formatter: function (value, rec) {
                       var title = "";
                       if (rec.IsRead == 0) {
                           title = "<a href=\"javascript:WinOpenIt('../WF/MyFlow.aspx?FK_Flow=" + rec.FK_Flow + "&FK_Node=" + rec.FK_Node
                           + "&FID=" + rec.FID + "&WorkID=" + rec.WorkID + "&AtPara=" + rec.AtPara + "&IsRead=0"
                           + "','" + rec.WorkID + "','" + rec.FlowName + "');\" ><img align='middle' alt='' id='" + rec.WorkID
                           + "' src='Img/Menu/Mail_UnRead.png' border=0 width=20 height=20 />" + rec.Title + "</a>";
                       } else {
                           title = "<a href=\"javascript:WinOpenIt('../WF/MyFlow.aspx?FK_Flow=" + rec.FK_Flow + "&FK_Node=" + rec.FK_Node
                           + "&FID=" + rec.FID + "&WorkID=" + rec.WorkID + "&AtPara=" + rec.AtPara + "','" + rec.WorkID
                           + "','" + rec.FlowName + "');\"  ><img align='middle' border=0 width=20 height=20 id='" + rec.WorkID + "' alt='' src='Img/Menu/Mail_Read.png'/>" + rec.Title + "</a>";
                       }
                       return title;
                   }
                   },
                   { title: '流程名称', width: 160, field: 'FlowName' },
                   { title: '当前节点', width: 120, field: 'NodeName' },
                   { title: '发起人', width: 100, field: 'StarterName' },
                   { title: '发起日期', width: 100, field: 'RDT' },
                   { title: '接受日期', width: 100, field: 'ADT' },
                   { title: '应完成日期', width: 100, field: 'SDT'}];

            var params = {
                method: "getempworks",
                FK_Flow: getArgsFromHref("FK_Flow"),
                ProjNo: getArgsFromHref("id")
            };
            queryData(params, LoadDataGrid, this);
        }
        //抄送列表
        function LoadCCFowData() {
            this.columns = [
                   { title: '标题', field: 'Title', width: 260, align: 'left', formatter: function (value, rec) {
                       var title = "<a href=javascript:WinOpenCCIt('" + rec.MyPK + "','" + rec.FK_Flow + "','" + rec.FK_Node
                       + "','" + rec.WorkID + "','" + rec.FID + "','" + rec.Sta + "');><img align=middle src='Img/Menu/CCSta/" + rec.Sta
                       + ".png' id=" + rec.MyPK + ">" + rec.Title + "</a>日期:" + rec.RDT;
                       return title;
                   }
                   },
                   { title: '流程名称', field: 'FlowName', width: 160 },
                   { title: '当前节点', field: 'NodeName', width: 120 },
                   { title: '内容', field: 'Doc', width: 200 },
                   { title: '抄送人', field: 'Rec', width: 100 },
                   { title: '抄送日期', field: 'RDT', width: 100}];
            var params = {
                method: "getccflowdata",
                FK_Flow: getArgsFromHref("FK_Flow"),
                ProjNo: getArgsFromHref("id")
            };
            queryData(params, LoadDataGrid, this);
        }
        //在途列表
        function LoadRuningFowData() {
            this.columns = [{ title: '标题', field: 'Title', width: 240, align: 'left', formatter: function (value, rec) {
                var h = "../WF/WFRpt.aspx?WorkID=" + rec.WorkID + "&FK_Flow=" + rec.FK_Flow + "&FID=" + rec.FID;
                return "<a href='javascript:void(0);' onclick=winOpen('" + h + "')><img align='middle' border=0 width='20' height='20' src='Img/Menu/Runing.png'/>" + rec.Title + "</a>";
            }
            },
                   { title: '当前节点', field: 'NodeName', width: 120 },
                   { title: '发起人', field: 'StarterName', width: 100 },
                   { title: '发起日期', field: 'RDT', width: 100 },
                   { title: '操作', field: 'opt', width: 200,
                       formatter: function (value, rec) {
                           var h2 = "../WF/WorkOpt/Press.htm?FID=" + rec.FID + '&WorkID=' + rec.WorkID + '&FK_Flow=' + rec.FK_Flow;
                           return "<a href='javascript:void(0);' onclick=UnSend('" + rec.FK_Flow + "','" + rec.WorkID + "') ><img align='middle' width='20' height='20' src='../WF/Img/Action/UnSend.png' border=0 />撤消发送</a>&nbsp;&nbsp;&nbsp;<a href='javascript:void(0);' onclick=Press('" + h2 + "')><img width='20' height='20' align='middle' src='../WF/Img/Action/Press.png' border=0 />催办</a>";

                       }
                   }];
            var params = {
                method: "getruningflowdata",
                FK_Flow: getArgsFromHref("FK_Flow"),
                ProjNo: getArgsFromHref("id")
            };
            queryData(params, LoadDataGrid, this);
        }
        //挂起列表
        function LoadHumUpFowData() {
            this.columns = [{ title: '标题', field: 'Title', width: 240, align: 'left', formatter: function (rowindex, rowdata) {
                var title = "<a href=javascript:winOpen('../WF/MyFlow.aspx?FK_Flow=" + rowdata.FK_Flow
                               + "&FK_Node=" + rowdata.FK_Node + "&FID=" + rowdata.FID + "&WorkID=" + rowdata.WorkID
                               + "&IsRead=0');><img align='middle' alt='' src='Img/Menu/Mail_UnRead.png' border=0 id='" + rowdata.MyPK
                               + "'/>" + rowdata.Title + "</a>";
                return title;
            }
            },
            { title: '流程名称', field: 'FlowName', width: 200 },
            { title: '当前节点', field: 'NodeName', width: 140 },
            { title: '发起人', field: 'StarterName', width: 100 },
            { title: '发起日期', field: 'RDT', width: 100 },
            { title: '接受日期', field: 'SDTOfFlow', width: 100 },
            { title: '应完成日期', field: 'SDTOfNode', width: 100}];
            var params = {
                method: "gethunupflowdata",
                FK_Flow: getArgsFromHref("FK_Flow"),
                ProjNo: getArgsFromHref("id")
            };
            queryData(params, LoadDataGrid, this);
        }
        //办结流程
        function LoadOverFowData() {
            this.columns = [{ title: '标题', field: 'Title', width: 240, align: 'left', formatter: function (rowindex, rowdata) {
                var titleText = "../WF/WorkOpt/OneWork/Track.aspx?WorkID=" + rowdata.OID + "&FK_Flow=" + rowdata.FK_Flow;
                titleText = "<a href=javascript:winOpen('" + titleText + "');>" + rowdata.Title + "</a>";
                return titleText;
            }
            },
            { title: '部门', field: 'FK_DeptText', width: 200 },
            { title: '发起人', field: 'StarterName', width: 100 },
            { title: '发起时间', field: 'FlowStartRDT', width: 100 },
            { title: '流程状态', field: 'WFStateText', width: 100 },
            { title: '最后处理时间', field: 'FlowEnderRDT', width: 100}];
            var FK_Flow = getArgsFromHref("FK_Flow");
            if (FK_Flow == "") {
                FK_Flow = "999";
            }
            var params = {
                method: "loadoverflowdata",
                FK_Flow: FK_Flow,
                ProjNo: getArgsFromHref("id")
            };
            queryData(params, LoadDataGrid, this);
        }
        //工具按钮事件
        function ToolBarClick(item) {
            //发起
            if (item == "start") {
                var fk_flow = getArgsFromHref("FK_Flow");
                var url = "../../Flow/WF/MyFlow.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_flow + "01&ProjNo=" + getArgsFromHref("id");
                var params = {
                    method: "iscanstartthisflow",
                    FK_Flow: getArgsFromHref("FK_Flow")
                };
                queryData(params, function (js) {
                    try {
                        var pushData = eval('(' + js + ')');
                        if (pushData.IsCan == "true") {
                            if (window.parent && window.parent.addNewTab) {
                                window.parent.addNewTab(pushData.FlowName + "-发起", url);
                            } else {
                                WinOpenIt(url, fk_flow, "发起");
                            }
                        } else {
                            $.messager.alert('提示', '您没有发起此业务的权限！');
                        }
                    } catch (e) {
                        $.messager.alert('提示', '您没有发起此业务的权限！');
                    }
                }, this);
            }
            //待办
            if (item == "empworks") {
                $('#index_layout').layout('panel', 'center').panel('setTitle', '当前位置: 待办');
                LoadEmpWorkData();
            }
            //抄送
            if (item == "cc") {
                $('#index_layout').layout('panel', 'center').panel('setTitle', '当前位置: 抄送');
                LoadCCFowData();
            }
            //挂起
            if (item == "hunup") {
                $('#index_layout').layout('panel', 'center').panel('setTitle', '当前位置: 挂起');
                LoadHumUpFowData();
            }
            //在途
            if (item == "runing") {
                $('#index_layout').layout('panel', 'center').panel('setTitle', '当前位置: 在途');
                LoadRuningFowData();
            }
            //办结
            if (item == "sendover") {
                $('#index_layout').layout('panel', 'center').panel('setTitle', '当前位置: 已办');
                LoadOverFowData();
            }
        }

        function IsCanStartThisFlow() {
            var params = {
                method: "iscanstartthisflow",
                FK_Flow: getArgsFromHref("FK_Flow")
            };
            queryData(params, function (js) {
                var pushData = eval('(' + js + ')');
                if (pushData.IsCan == "true") {
                    alert(document.getElementById("LinkBtn_Start").style);
                    $('#LinkBtn_Start').linkbutton('enable');
                }
            }, this);
        }

        //初始化
        $(function () {
            LoadEmpWorkData();
            //IsCanStartThisFlow();
        });
        function getArgsFromHref(sArgName) {
            var sHref = window.location.href;
            var args = sHref.split("?");
            var retval = "";
            if (args[0] == sHref) /*参数为空*/
            {
                return retval; /*无需做任何处理*/
            }

            var str = args[1];
            args = str.split("&");
            for (var i = 0; i < args.length; i++) {
                str = args[i];
                var arg = str.split("=");
                if (arg.length <= 1) continue;
                if (arg[0] == sArgName) retval = arg[1];
            }
            while (retval.indexOf('#') >= 0) {
                retval = retval.replace('#', '');
            }
            return retval;
        }
        //公共方法
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "ProjectInfo.aspx", //要访问的后台地址
                data: param, //要发送的数据
                async: false,
                cache: false,
                complete: function () { }, //AJAX请求完成时隐藏loading提示
                error: function (XMLHttpRequest, errorThrown) {
                    callback(XMLHttpRequest);
                },
                success: function (msg) {//msg为返回的数据，在这里做数据绑定
                    var data = msg;
                    callback(data, scope);
                }
            });
        }
    </script>
    <style type="text/css">
        body, html
        {
            height: 100%;
        }
        body
        {
            padding: 0px;
            margin: 0;
            overflow: hidden;
        }
    </style>
</head>
<body class="easyui-layout" id="index_layout">
    <div data-options="region:'center',border:false" style="margin: 0; padding: 0; overflow: auto;"
        title="当前位置：待办">
        <div id="tb">
            <a href="#" id="LinkBtn_Start" style="float: left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-add'"
                onclick="ToolBarClick('start')">发起</a>
            <div class="datagrid-btn-separator">
            </div>
            <a href="#" style="float: left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-department'"
                onclick="ToolBarClick('empworks')">待办</a>
            <%--<div class="datagrid-btn-separator">
            </div>
            <a href="#" style="float: left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-sheet'"
                onclick="ToolBarClick('cc')">抄送</a>
            <div class="datagrid-btn-separator">
            </div>
            <a href="#" style="float: left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-auto'"
                onclick="ToolBarClick('hunup')">挂起</a>--%>
            <div class="datagrid-btn-separator">
            </div>
            <a href="#" style="float: left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-insert'"
                onclick="ToolBarClick('runing')">在途</a>
            <div class="datagrid-btn-separator">
            </div>
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-insert'"
                onclick="ToolBarClick('sendover')">已办</a>
        </div>
        <table id="ensGrid" toolbar="#tb" class="easyui-datagrid">
        </table>
    </div>
</body>
</html>
