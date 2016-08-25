﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SFGuide.aspx.cs" Inherits="CCFlow.WF.Comm.Sys.SFGuide" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>创建字典表向导</title>
    <link href="../Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../JScript.js" type="text/javascript"></script>
    <script src="../JS/Ctrls.js" type="text/javascript"></script>
    <script type="text/javascript">
        function generateSQL(dblink, dbname, dbtype, islocal) {
            var table = $('#ContentPlaceHolder1_Pub1_LB_Table').val();
            var no = $('#ContentPlaceHolder1_Pub1_DDL_ColValue').val();
            var name = $('#ContentPlaceHolder1_Pub1_DDL_ColText').val();
            var parentno = $('#ContentPlaceHolder1_Pub1_DDL_ColParentNo').val();
            var type = $('#ContentPlaceHolder1_Pub1_DDL_SFTableType').val();
            var txtsql = $('#ContentPlaceHolder1_Pub1_TB_SelectStatement');
            var sql;

            switch (dbtype) {
                case 'SQLServer':
                    sql = 'SELECT ' + no + ' AS No, ' + name + ' AS [Name]' + (type == '1' ? (', ' + parentno + ' AS ParentNo') : '') + ' FROM ' + (islocal ? '' : (dblink + '.' + dbname + '.')) + 'dbo.' + table;
                    break;
                case 'Oracle':
                case 'MySQL':
                    sql = 'SELECT ' + no + ' AS No, ' + name + ' AS \'Name\'' + (type == '1' ? (', ' + parentno + ' AS ParentNo') : '') + ' FROM OPENQUERY(' + dblink + ',\'SELECT * FROM ' + dbname + '.' + table + '\')';
                    break;
                case 'Informix':
                    sql = '';
                    break;
            }

            txtsql.text(sql);
        }

        function showInfo(title, msg, autoHiddenMillionSeconds) {
            $.messager.show({
                title: title,
                msg: msg,
                timeout: autoHiddenMillionSeconds,
                showType: 'slide',
                style: {
                    right: '',
                    top: document.body.scrollTop + document.documentElement.scrollTop,
                    bottom: ''
                }
            });
        }

        function showInfoAndGo(title, msg, icon, url) {
            if (url == undefined || url == null || url.length == 0) {
                $.messager.alert(title, msg, icon);
            }
            else {
                $.messager.alert(title, msg, icon, function () {
                    self.location = url;
                });
            }
        }

        function showInfoAndBack(title, msg, icon) {
            $.messager.alert(title, msg, icon, function () {
                history.back();
            });
        }

        var CONST_TYPES = [{ NO: '0', NAME: '本地的类' }, { NO: '1', NAME: '创建表' }, { NO: '2', NAME: '表或视图' }, { NO: '3', NAME: 'SQL查询表' }, { NO: '4', NAME: 'WebServices'}];
        var CONST_STRUCTS = [{ NO: '0', NAME: '普通的编码表(具有No,Name)' }, { NO: '1', NAME: '树结构(具有No,Name,ParentNo)'}];
        var CONST_GROUPTITLE = "class='GroupTitle'";
        var url = './SFGuide.aspx';
        var t;
        var srcType = 0;
        var sfno;
        var sftable = {};
        var classes;
        var srcs;
        var tvs;
        var cols;
        var mtds;

        $(function () {
            t = new CtrlFactory('mtable');
            sfno = t.getQueryString('sfno');
            $('#srcTypes').change(function () {
                loadSrcType(this.value, this.options[this.selectedIndex].text);
            });

            if (sfno) {
                //获取信息
                t.ajax(url, { method: 'getinfo', sfno: sfno }, function (msg) {
                    sftable = $.parseJSON(msg);
                    loadSrcType(sftable.SRCTYPE, null);
                }, function (msg) {
                    alert('错误：' + msg);
                });
            }
            else {
                loadSrcType(CONST_TYPES[0].NO, CONST_TYPES[0].NAME);
            }
        });

        function loadSrcType(value, text) {
            //清除第2行下面所有行
            while ($('#mtable tr').length > 2) {
                $('#mtable tr').last().remove();
            }

            var struct = (sftable.CODESTRUCT | 0).toString();
            var src = sftable.FK_SFDBSRC ? sftable.FK_SFDBSRC : 'local';

            switch (parseInt(value)) {
                case 0: //BP类

                    t.addTR(null, 'r2')
                     .addTD('r2', 'c20', CONST_GROUPTITLE, '实体类型：')
                     .addTD('r2', 'c21')
                     .addSelect('c21', 'CodeStruct', CONST_STRUCTS, struct, loadStructClass)
                     .addTD('r2', 'c22', null, '选择具体有指定字段的实体类型')
                     .addTR(null, 'r3')
                     .addTD('r3', 'c30', CONST_GROUPTITLE, '类：')
                     .addTD('r3', 'c31')
                     .addSelect('c31', 'Class', getStructClass(struct), sfno)
                     .addTD('r3', 'c32', null, '选择一个类');

                    if (struct == CONST_STRUCTS[1].NO) {
                        t.addTR(null, 'rp')
                         .addTD('rp', 'cp0', CONST_GROUPTITLE, '根节点值：')
                         .addTD('rp', 'cp1')
                         .addTextbox('cp1', 'RootValue', null, sftable.DEFVAL ? sftable.DEFVAL : '0')
                         .addTD('rp', 'cp2', null, '填写此树的根节点值');
                    }
                    break;
                case 1: //创建表
                    loadNormalInfo();

                    t.addTR(null, 'r4')
                     .addTD('r4', 'c40', CONST_GROUPTITLE, '数据源：')
                     .addTD('r4', 'c41')
                     .addSelect('c41', 'SFDBSrc', getSrcs(), src)
                     .addTD('r4', 'c42', null, '选择字典表所属数据源')
                     .addTR(null, 'r5')
                     .addTD('r5', 'c50', CONST_GROUPTITLE, '数据格式：')
                     .addTD('r5', 'c51')
                     .addSelect('c51', 'CodeStruct', CONST_STRUCTS, struct)
                     .addTD('r5', 'c52', null, '选择具体有指定字段的格式');
                    break;
                case 2: //表或视图
                    loadNormalInfo();
                    var stable = sftable.SRCTABLE ? sftable.SRCTABLE : '';
                    var colval = sftable.COLUMNVALUE ? sftable.COLUMNVALUE : '';
                    var coltext = sftable.COLUMNTEXT ? sftable.COLUMNTEXT : '';
                    var colparent = sftable.PARENTVALUE ? sftable.PARENTVALUE : '';
                    var rootvalue = sftable.DEFVAL ? sftable.DEFVAL : '0';
                    var sql = sftable.SELECTSTATEMENT ? sftable.SELECTSTATEMENT : ''

                    t.addTR(null, 'r4')
                     .addTD('r4', 'c40', CONST_GROUPTITLE, '数据源：')
                     .addTD('r4', 'c41')
                     .addSelect('c41', 'SFDBSrc', getSrcs(), src, loadTableViews)
                     .addTD('r4', 'c42', null, '选择字典表所属数据源')
                     .addTR(null, 'r5')
                     .addTD('r5', 'c50', CONST_GROUPTITLE, '表/视图：')
                     .addTD('r5', 'c51')
                     .addSelect('c51', 'SrcTable', getTableViews(src), stable, loadColumns)
                     .addTD('r5', 'c52', null, '选择一个表或视图')
                     .addTR(null, 'r6')
                     .addTD('r6', 'c60', CONST_GROUPTITLE, '数据格式：')
                     .addTD('r6', 'c61')
                     .addSelect('c61', 'CodeStruct', CONST_STRUCTS, struct, loadStructSet)
                     .addTD('r6', 'c62', null, '选择具体有指定字段的格式')
                     .addTR(null, 'r7')
                     .addTD('r7', 'c70', CONST_GROUPTITLE, '编码列：')
                     .addTD('r7', 'c71')
                     .addSelect('c71', 'ColumnValue', getColumns(stable.length == 0 ? (tvs && tvs.length > 0 ? tvs[0].NO : '') : stable), colval)
                     .addTD('r7', 'c72', null, '即No列')
                     .addTR(null, 'r8')
                     .addTD('r8', 'c80', CONST_GROUPTITLE, '标签列：')
                     .addTD('r8', 'c81')
                     .addSelect('c81', 'ColumnText', getColumns(stable.length == 0 ? (tvs && tvs.length > 0 ? tvs[0].NO : '') : stable), coltext)
                     .addTD('r8', 'c82', null, '即Name列');

                    if (struct == CONST_STRUCTS[1].NO) {
                        t.addTR(null, 'rpc')
                         .addTD('rpc', 'cpc0', CONST_GROUPTITLE, '父节点列：')
                         .addTD('rpc', 'cpc1')
                         .addSelect('cpc1', 'ParentValue', getColumns(stable.length == 0 ? (tvs && tvs.length > 0 ? tvs[0].NO : '') : stable), colparent)
                         .addTD('rpc', 'cpc2', null, '即ParentNo列')
                         .addTR(null, 'rpv')
                         .addTD('rpv', 'cpv0', CONST_GROUPTITLE, '根节点值：')
                         .addTD('rpv', 'cpv1')
                         .addTextbox('cpv1', 'RootValue', null, rootvalue)
                         .addTD('rpv', 'cpv2', null, '填写此树的根节点值');
                    }

                    t.addTR(null, 'r9')
                     .addTD('r9', 'c90', CONST_GROUPTITLE, '过滤SQL：')
                     .addTD('r9', 'c91', 'colspan="2"')
                     .addTextbox('c91', 'SelectStatement', "style='width:98%'", sql)
                     .add('#c91', "<br />比如：XXX = '002' AND YYY = 3，支持参数表达式：@WebUser.No,@WebUser.Name,@WebUser.FK_Dept,@WebUser.FK_DeptName");
                    break;
                case 3: //SQL查询表
                    loadNormalInfo();

                    var sql = sftable.SELECTSTATEMENT ? sftable.SELECTSTATEMENT : ''

                    t.addTR(null, 'r4')
                     .addTD('r4', 'c40', CONST_GROUPTITLE, '数据源：')
                     .addTD('r4', 'c41')
                     .addSelect('c41', 'SFDBSrc', getSrcs(), src)
                     .addTD('r4', 'c42', null, '选择字典表所属数据源')
                     .addTR(null, 'r5')
                     .addTD('r5', 'c50', CONST_GROUPTITLE, '数据格式：')
                     .addTD('r5', 'c51')
                     .addSelect('c51', 'CodeStruct', CONST_STRUCTS, struct, loadStructSQL)
                     .addTD('r5', 'c52', null, '选择具体有指定字段的格式');

                    if (struct == CONST_STRUCTS[1].NO) {
                        t.addTR(null, 'rp')
                         .addTD('rp', 'cp0', CONST_GROUPTITLE, '根节点值：')
                         .addTD('rp', 'cp1')
                         .addTextbox('cp1', 'RootValue', null, sftable.DEFVAL ? sftable.DEFVAL : '0')
                         .addTD('rp', 'cp2', null, '填写此树的根节点值');
                    }

                    t.addTR(null, 'r6')
                     .addTD('r6', 'c60', CONST_GROUPTITLE, 'SQL语句：')
                     .addTD('r6', 'c61', 'colspan="2"')
                     .addTextbox('c61', 'SelectStatement', "style='width:98%'", sql)
                     .add('#c61', "<br />比如：XXX = '002' AND YYY = 3，支持参数表达式：@WebUser.No,@WebUser.Name,@WebUser.FK_Dept,@WebUser.FK_DeptName");
                    break;
                case 4: //WebServices
                    loadNormalInfo();

                    var stable = sftable.SRCTABLE ? sftable.SRCTABLE : '';
                    var sql = sftable.SELECTSTATEMENT ? sftable.SELECTSTATEMENT : ''
                    src = sftable.FK_SFDBSRC ? sftable.FK_SFDBSRC : '';

                    t.addTR(null, 'r4')
                     .addTD('r4', 'c40', CONST_GROUPTITLE, '数据源：')
                     .addTD('r4', 'c41')
                     .addSelect('c41', 'SFDBSrc', getSrcs(100), src, loadWSMethods)
                     .addTD('r4', 'c42', null, '选择字典表所属WebService数据源')
                     .addTR(null, 'r5')
                     .addTD('r5', 'c50', CONST_GROUPTITLE, '方法：')
                     .addTD('r5', 'c51')
                     .addSelect('c51', 'SrcTable', getWSMethods(src.length > 0 ? src : srcs && srcs.length > 0 ? srcs[0].NO : ''), stable)
                     .addTD('r5', 'c52', null, '选择WebSerivce中提供此字典表数据的接口方法')
                     .addTR(null, 'r6')
                     .addTD('r6', 'c60', CONST_GROUPTITLE, '设置参数：')
                     .addTD('r6', 'c61', 'colspan="2"')
                     .addTextbox('c61', 'SelectStatement', "style='width:98%'", sql)
                     .add('#c61', "<br />比如：aaa = '002'&bbb = 3，支持参数表达式：@WebUser.No,@WebUser.Name,@WebUser.FK_Dept,@WebUser.FK_DeptName")
                     .addTR(null, 'r7')
                     .addTD('r7', 'c70', CONST_GROUPTITLE, '数据格式：')
                     .addTD('r7', 'c71')
                     .addSelect('c71', 'CodeStruct', CONST_STRUCTS, struct, loadStructWS)
                     .addTD('r7', 'c72', null, '选择具体有指定字段的格式');

                    if (struct == CONST_STRUCTS[1].NO) {
                        t.addTR(null, 'rp')
                         .addTD('rp', 'cp0', CONST_GROUPTITLE, '根节点值：')
                         .addTD('rp', 'cp1')
                         .addTextbox('cp1', 'RootValue', null, sftable.DEFVAL ? sftable.DEFVAL : '0')
                         .addTD('rp', 'cp2', null, '填写此树的根节点值');
                    }
                    break;
            }
        }

        function loadNormalInfo() {
            t.addTR(null, 'r2')
            .addTD('r2', 'c20', CONST_GROUPTITLE, '字典编号：')
            .addTD('r2', 'c21')
            .addTextbox('c21', 'No', null, sftable.NO ? sftable.NO : '')
            .addTD('r2', 'c22', null, '创建字典表的英文名称')
            .addTR(null, 'r3')
            .addTD('r3', 'c30', CONST_GROUPTITLE, '字典名称：')
            .addTD('r3', 'c31')
            .addTextbox('c31', 'Name', null, sftable.NAME ? sftable.NAME : '')
            .addTD('r3', 'c32', null, '创建字典表的中文名称');            
        }

        function loadStructClass(value, text) {
            $('#DDL_Class').remove();

            t.addSelect('c31', 'Class', getStructClass(value), sfno);

            if (value == CONST_STRUCTS[0].NO) {
                //删除父结点值设置行
                $('#rp').remove();
            }
            else {
                //增加父结点值设置行
                t.addTR(null, 'rp', null, '#r3')
                 .addTD('rp', 'cp0', CONST_GROUPTITLE, '根节点值：')
                 .addTD('rp', 'cp1')
                 .addTextbox('cp1', 'RootValue', null, sftable.DEFVAL ? sftable.DEFVAL : '0')
                 .addTD('rp', 'cp2', null, '填写此树的根节点值');
            }
        }

        function loadStructSet(value, text) {
            if (value == CONST_STRUCTS[0].NO) {
                //删除父结点列/值设置行
                $('#rpc').remove();
                $('#rpv').remove();
            }
            else {
                //增加父结点列/值设置行
                var stable = $('#DDL_SrcTable').val();
                var colparent = sftable.PARENTVALUE ? sftable.PARENTVALUE : '';
                var rootvalue = sftable.DEFVAL ? sftable.DEFVAL : '0';

                t.addTR(null, 'rpc', null, '#r8')
                 .addTD('rpc', 'cpc0', CONST_GROUPTITLE, '父节点列：')
                 .addTD('rpc', 'cpc1')
                 .addSelect('cpc1', 'ParentValue', getColumns(stable.length == 0 ? (tvs && tvs.length > 0 ? tvs[0].NO : '') : stable), colparent)
                 .addTD('rpc', 'cpc2', null, '即ParentNo列')
                 .addTR(null, 'rpv', null, '#rpc')
                 .addTD('rpv', 'cpv0', CONST_GROUPTITLE, '根节点值：')
                 .addTD('rpv', 'cpv1')
                 .addTextbox('cpv1', 'RootValue', null, rootvalue)
                 .addTD('rpv', 'cpv2', null, '填写此树的根节点值');
            }
        }

        function loadStructSQL(value, text) {
            if (value == CONST_STRUCTS[0].NO) {
                //删除父结点值设置行
                $('#rp').remove();
            }
            else {
                //增加父结点值设置行
                t.addTR(null, 'rp', null, '#r5')
                 .addTD('rp', 'cp0', CONST_GROUPTITLE, '根节点值：')
                 .addTD('rp', 'cp1')
                 .addTextbox('cp1', 'RootValue', null, sftable.DEFVAL ? sftable.DEFVAL : '0')
                 .addTD('rp', 'cp2', null, '填写此树的根节点值');
            }
        }

        function loadStructWS(value, text) {
            if (value == CONST_STRUCTS[0].NO) {
                //删除父结点值设置行
                $('#rp').remove();
            }
            else {
                //增加父结点值设置行
                t.addTR(null, 'rp')
                 .addTD('rp', 'cp0', CONST_GROUPTITLE, '根节点值：')
                 .addTD('rp', 'cp1')
                 .addTextbox('cp1', 'RootValue', null, sftable.DEFVAL ? sftable.DEFVAL : '0')
                 .addTD('rp', 'cp2', null, '填写此树的根节点值');
            }
        }

        function loadTableViews(value, text) {
            $('#DDL_SrcTable').remove();

            t.addSelect('c51', 'SrcTable', getTableViews(value), sftable.SRCTABLE ? sftable.SRCTABLE : '', loadColumns);

            var selectedTable = $('#DDL_SrcTable').val();

            if (selectedTable.length > 0) {
                loadColumns(selectedTable, $('#DDL_SrcTable').text());
            }
        }

        function loadWSMethods(value, text) {
            $('#DDL_SrcTable').remove();

            t.addSelect('c51', 'SrcTable', getWSMethods(value), sftable.SRCTABLE ? sftable.SRCTABLE : '')
        }

        function loadColumns(value, text) {
            var haveparent = $('#DDL_CodeStruct').val() == CONST_STRUCTS[1].NO;
            
            $('#DDL_ColumnValue').remove();
            $('#DDL_ColumnText').remove();

            if (haveparent) {
                $('#DDL_ParentValue').remove();
            }

            var colval = sftable.COLUMNVALUE ? sftable.COLUMNVALUE : '';
            var coltext = sftable.COLUMNTEXT ? sftable.COLUMNTEXT : '';
            var colparent = sftable.PARENTVALUE ? sftable.PARENTVALUE : '';

            t.addSelect('c71', 'ColumnValue', getColumns(value), colval)
             .addSelect('c81', 'ColumnText', getColumns(value), coltext);

            if (haveparent) {
                t.addSelect('cpc1', 'ParentValue', getColumns(value), colparent);
            }
        }

        function getStructClass(struct) {
            classes = [];

            t.ajax(url, { method: 'getclass', struct: struct }, false, function (msg) {
                var re = $.parseJSON(msg);

                if (re.success) {
                    classes = re.msg;
                }
                else {
                    alert('获取类出错：' + re.msg);
                    classes = [];
                }
            }, function (msg) {
                alert('获取类出错：' + msg);
                classes = [];
            });

            return classes;
        }

        function getSrcs(type) {
            srcs = [];

            t.ajax(url, { method: 'getsrcs', type: type }, false, function (msg) {
                var re = $.parseJSON(msg);

                if (re.success) {
                    srcs = re.msg;
                }
                else {
                    alert('获取数据源列表出错：' + re.msg);
                    srcs = [];
                }
            }, function (msg) {
                alert('获取数据源列表出错：' + msg);
                srcs = [];
            });

            return srcs;
        }

        function getTableViews(src) {
            tvs = [];

            t.ajax(url, { method: 'gettvs', src: src }, false, function (msg) {
                var re = $.parseJSON(msg);

                if (re.success) {
                    tvs = re.msg;
                }
                else {
                    alert('获取表/视图列表出错：' + re.msg);
                    tvs = [];
                }
            }, function (msg) {
                alert('获取表/视图列表出错：' + msg);
                tvs = [];
            });

            return tvs;
        }

        function getColumns(table) {
            cols = [];
            var src = $('#DDL_SFDBSrc').val();

            t.ajax(url, { method: 'getcols', src: src, table: table }, false, function (msg) {
                var re = $.parseJSON(msg);

                if (re.success) {
                    cols = re.msg;
                }
                else {
                    alert('获取列信息出错：' + re.msg);
                    cols = [];
                }
            }, function (msg) {
                alert('获取列信息出错：' + msg);
                cols = [];
            });

            return cols;
        }

        function getWSMethods(src) {
            mtds = [];

            t.ajax(url, { method: 'getmtds', src: src }, false, function (msg) {
                var re = $.parseJSON(msg);

                if (re.success) {
                    mtds = re.msg;
                }
                else {
                    alert('获取WebService方法列表出错：' + re.msg);
                    mtds = [];
                }
            }, function (msg) {
                alert('获取WebService方法列表出错：' + msg);
                mtds = [];
            });

            return mtds;
        }
    </script>
    <base target="_self" />
</head>
<body class="easyui-layout">
    <form id="aspnetForm" runat="server">
    <div data-options="region:'center',title:'创建数据源表'" style="padding: 5px;">
        <%--<uc1:Pub ID="Pub1" runat="server" />--%>
        <table id="mtable" class="Table" cellpadding="0" cellspacing="0" border="0" style="width: 98%">
            <tr>
                <td class="GroupTitle" width="120px">
                    项目
                </td>
                <td class="GroupTitle" width="300px">
                    值
                </td>
                <td class="GroupTitle">
                    备注
                </td>
            </tr>
            <tr>
                <td class="GroupTitle">
                    数据源表类型：
                </td>
                <td width="300px">
                    <select id="srcTypes">
                        <option value="0">本地的类</option>
                        <option value="1">创建表</option>
                        <option value="2">表或视图</option>
                        <option value="3">SQL查询表</option>
                        <option value="4">WebServices</option>
                    </select>
                </td>
                <td>
                    选择5种类型中的一种
                </td>
            </tr>
        </table>
        <br />
        <a href="javascript:void(0)" id="Btn_Create" class="easyui-linkbutton" data-options="iconCls:'icon-save'"
            onclick="Create()">创建</a> <a href="javascript:void(0)" id="Btn_ShowData" class="easyui-linkbutton"
                data-options="iconCls:'icon-open'" onclick="Create()">查看数据</a>
    </div>
    </form>
</body>
</html>
