﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Organization.aspx.cs" Inherits="GMP2.GPM.Organization" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="themes/default/easyui.css" />
    <link rel="stylesheet" type="text/css" href="themes/icon.css" />
    <script type="text/javascript" src="jquery/jquery-1.7.2.min.js"></script>
    <script type="text/javascript" src="jquery/jquery.easyui.min.js"></script>
    <script src="jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="javascript/CC.MessageLib.js" type="text/javascript"></script>
    <script src="javascript/AppData.js" type="text/javascript"></script>
    <script src="javascript/Organization.js" type="text/javascript"></script>
    <style type="text/css">
    .tableLabel
    {
        text-align:right;
        width:80px;
    }
    #emptt input
    {
        width:200px;
    }
    </style>
</head>
<body class="easyui-layout">
    <div id="pageloading">
    </div>
    <div data-options="region:'west',split:true" style="width: 350px; padding: 1px; overflow: hidden;">
        <div class="easyui-panel" style="padding: 1px; background-color: #F4F4F4; border: 0px;
            height: 30px; width: auto;">
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="checkDeptInfo()" data-options="plain:true,iconCls:'icon-edit'">编辑</a> 
            <a href="javascript:void(0)" class="easyui-menubutton" data-options="menu:'#addNewDept',iconCls:'icon-new'">新建</a>
            <div id="addNewDept" style="width: 10px;">
                <div data-options="iconCls:'icon-add'" onclick="append('peer')">同级</div>
                <div data-options="iconCls:'icon-add'" onclick="append('son')">子级</div>
            </div>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-sum',plain:true" onclick="connecteEmp()">关联人员</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-rights',plain:true" onclick="ViewDisableEmps(1,20)">已禁用人员</a>
        </div>
        <div style="width: 100%; height: 100%; overflow: auto;">
            <ul id="appTree" class="easyui-tree-line" data-options="animate:false,dnd:false">
            </ul>
        </div>
    </div>
    <div data-options="region:'center',split:true" style="overflow: hidden;">
        <div id="tb">
            <a href="#" id="addEmpApp" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-addG'" onclick="checkDeptDutyAndStation();">添加人员</a> 
            <a href="#" id="editEmpApp" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-edit'" onclick="EditEmpApp()">编辑人员</a>
            <a href="#" id="delEmpApp" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-deleteG'" onclick="DeleteEmpApp()">删除人员</a> 
            <a href="#" id="disableEmpApp" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-tip'" onclick="DisableEmpApp()">禁用人员</a> 
            <a href="#" id="refreshData" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-reloadG'" onclick="refreshGrid()">刷新</a>
            <a href="#" id="orderData" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-downG'" onclick="LoadGridOrderBy(this)">按姓名降序</a>
            <input id="searchText" class="easyui-textbox" type="text" style=" height:18px;"/>
            <a href="#" id="searchEmp" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-search'" onclick="SearchByEmpNoOrName(1,20)">查询</a>
        </div>
        <table id="empAppGrid" fit="true" fitcolumns="true" toolbar="#tb" class="easyui-datagrid">
        </table>
    </div>
    <div id="deptEmpDialog" class="easyui-dialog">
        <div id="addEmptt" class="easyui-tabs" style="width: auto; height: 440px; border: 0px;" data-options="border:false">
            <div title="基本信息" style="padding: 20px;">
                <table cellpadding="3">
                    <tr>
                        <td class="tableLabel">姓名:</td>
                        <td>
                            <input id="empName" class="easyui-textbox" type="text" name="name" data-options="required:true"/>
                        </td>
                        <td class="tableLabel">账号:</td>
                        <td title="双击我检查是否重名!" class="easyui-tooltip">
                            <input id="empNo" class="easyui-textbox" type="text" name="email" ondblclick="checkEmpNo()"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="tableLabel">员工工号:</td>
                        <td>
                            <input id="zgbh" class="easyui-textbox" type="text" name="name"/>
                        </td>
                        <td class="tableLabel">职务:</td>
                        <td tabindex="3">
                            <input id="zw" class="easyui-textbox" type="text" name="email" data-options="editable:false"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="tableLabel">电话:</td>
                        <td>
                            <input id="telephone" class="easyui-textbox" type="text" name="name"/>
                        </td>
                        <td class="tableLabel">邮件:</td>
                        <td>
                            <input id="email" class="easyui-textbox" type="text" name="email"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="tableLabel">直属领导:</td>
                        <td>
                            <input id="leader" class="easyui-textbox" type="text" name="name"/>
                        </td>
                        <td class="tableLabel">职务类别:</td>
                        <td>
                            <input id="zwSort" class="easyui-textbox" type="text" name="zwSortText"/>
                        </td>
                    </tr>
                </table>
            </div>
            <div title="拥有岗位" style="padding: 20px;">
                <ul id="gwDll" class="easyui-tree-line" data-options="animate:false,dnd:false,checkbox:true"></ul>
                <%--<input id="gwDll" class="easyui-textbox" type="text" name="gwxlk" data-options="editable:false"></input>--%>
            </div>
        </div>
        <div style="width: auto; height: 20px; margin-bottom: 0px; text-align: right;">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="addEmp()">保存</a> 
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="deptEmpDialogClose()">取消</a>
        </div>
    </div>
    <div id="mm" class="easyui-menu" style="width: 120px;">
        <div data-options="iconCls:'icon-edit'" onclick="checkDeptInfo()">编辑部门</div>
        <div data-options="iconCls:'icon-add'" onclick="append('peer')">新建同级部门</div>
        <div data-options="iconCls:'icon-add'" onclick="append('son')">新建子级部门</div>
        <div data-options="iconCls:'icon-up'" onclick="floatNode('up')">上移</div>
        <div data-options="iconCls:'icon-down'" onclick="floatNode('down')">下移</div>
        <div data-options="iconCls:'icon-sum'" onclick="connecteEmp()">关联人员</div>
        <div data-options="iconCls:'icon-delete'" onclick="deleteNode()">删除</div>
        <div class="menu-sep"></div>
        <div data-options="iconCls:'icon-reload'" onclick="LoadDeptTree()">刷新</div>
    </div>
    <div id="deptInfoDialog" class="easyui-dialog">
        <div id="tt" class="easyui-tabs" style="width: auto; height: 440px; border: 0px;"
            data-options="border:false">
            <div title="基本信息" data-options="iconCls:'icon-3'" style="padding-top: 40px; margin-left: 60px">
                <div>
                    &nbsp;&nbsp;编&nbsp;&nbsp;号：
                    <input id="dept_No" style="width: 200px" class="easyui-validatebox" type="text" name="deptNo" disabled="disabled" />
                </div>
                <div style="margin-top: 20px;">
                    &nbsp;&nbsp;名&nbsp;&nbsp;称：
                    <input id="dept_Name" style="width: 200px" class="easyui-validatebox" type="text" name="deptName" />
                </div>
                <div style="margin-top: 20px;">
                    &nbsp;&nbsp;领&nbsp;&nbsp;导：
                    <input id="dept_Leader" style="width: 200px" class="easyui-validatebox" type="text" name="deptLeader" />
                </div>
                <div style="margin-top:20px;">
                    上级部门：<input id="DDL_ParentDept" class="easyui-combotree" style="width: 200px;" />
                </div>
            </div>
            <div title="部门岗位" data-options="iconCls:'icon-2'" style="overflow: auto;">
                <div style="width: auto; height: 25px; padding-left: 20px; background-color: #CDE2FC;">
                    关键字：<input id="cc" name="dept" style="width: 200px;" />
                    <a id="isOk" href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-search'" onclick="doSearch()">查询</a>
                </div>
                <ul id="stationTree" class="easyui-tree-line" data-options="animate:false,dnd:false,checkbox:true">
                </ul>
            </div>
            <div title="部门职务" data-options="iconCls:'icon-person'">
                <ul id="deptDutyTree" class="easyui-tree-line" data-options="animate:false,dnd:false,checkbox:true">
                </ul>
            </div>
        </div>
        <div style="width: auto; height: 20px; margin-bottom: 0px; text-align: right;">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-delete'" onclick="deleteNode()">删除</a> 
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="saveDeptInfo()">保存</a> 
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="deptInfoDialogClose()">取消</a>
        </div>
    </div>
    <div id="mEmp" class="easyui-menu" style="width: 120px;">
        <div data-options="iconCls:'icon-addG'" onclick="checkDeptDutyAndStation()">添加人员</div>
        <div data-options="iconCls:'icon-edit'" onclick="EditEmpApp()">编辑人员</div>
        <div data-options="iconCls:'icon-deleteG'" onclick="DeleteEmpApp()">删除人员</div>
        <div class="menu-sep"></div>
        <div data-options="iconCls:'icon-reload'" onclick="refreshGrid()">刷新</div>
    </div>
    <div id="empDepts_Panel" class="easyui-dialog">
        <div style="width: auto; height: 25px; margin-top: 0px; text-align: left; background-color: #F4F4F4;padding-left: 20px;">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok',plain:true" onclick="EmpBlongToDept()">确定</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel',plain:true" onclick="empBelongDeptDialogClose();">关闭</a>
        </div>
        <ul id="deptBelongTree" class="easyui-tree"></ul>
    </div>
    <div id="empInfo" class="easyui-dialog">
        <div style="width: auto; height: 25px; margin-top: 0px; text-align: left; background-color: #F4F4F4;
            padding-left: 20px;">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-reset',plain:true" onclick="modifyPwd()">重置密码</a> 
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="doEdit()">保存</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel',plain:true" onclick="empInfoDialogClose()">取消</a>
        </div>
        <div id="emptt" class="easyui-tabs" style="width: auto; height: 430px; border: 0px;"
            data-options="border:false">
            <div title="基本信息" data-options="iconCls:'icon-3'" style="padding-top: 40px; margin-left:10px;" align="left">
                <table cellpadding="3" width="100%">
                    <tr>
                        <td class="tableLabel">姓名:</td>
                        <td>
                            <input id="setName" class="easyui-textbox" type="text" name="name"/>
                        </td>
                        <td class="tableLabel">账号:</td>
                        <td>
                            <input id="setNo" class="easyui-textbox" type="text" name="no" disabled="disabled"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="tableLabel">员工工号:</td>
                        <td>
                            <input onfocus="this.style.imeMode='inactive'" id="setZgbh" class="easyui-textbox" type="text" name="name"/>
                        </td>
                        <td class="tableLabel">职务:</td>
                        <td>
                            <input id="setZw" class="easyui-textbox" type="text" name="zw" data-options="editable:false"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="tableLabel">电话:</td>
                        <td>
                            <input id="setTel" onfocus="this.style.imeMode='inactive'" class="easyui-textbox" type="text" name="name" data-options="required:true"/>
                        </td>
                        <td class="tableLabel">邮件:</td>
                        <td>
                            <input id="setEamil" class="easyui-textbox" type="text" name="email" data-options="required:true,validType:'email'"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="tableLabel">直属领导:</td>
                        <td>
                            <input id="setLeader" class="easyui-textbox" type="text" name="name" data-options="required:true"/>
                        </td>
                        <td class="tableLabel">职务类别:</td>
                        <td>
                            <input id="setZwlb" class="easyui-textbox" type="text" name="email" data-options="required:true,validType:'email'"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" style="text-align:left;">
                             <div id="tbDeptBelong">
                                <a href="#" class="easyui-linkbutton" data-options="plain:true">归属部门</a>
                                <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-addG'" onclick="ShowDeptTree();">添加</a>
                                <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-person'" onclick="ReplaceBelongDept();">修改为主部门</a>
                                <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-delete'" onclick="RemoveBelongDept();">移除</a>
                            </div>
                            <div style="width:630px; height:180px;">
                                <table id="gdDeptBelong" fit="true" fitcolumns="true" toolbar="#tbDeptBelong" class="easyui-datagrid"></table>
                                <input type="hidden" id="HD_Emp_FK_Dept" />
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div title="拥有岗位" data-options="iconCls:'icon-2'" style="overflow: auto;">
                <ul id="empStationTree" class="easyui-tree-line" data-options="animate:false,dnd:false,checkbox:true"></ul>
            </div>
        </div>
    </div>
    <div id="connecteEmp" class="easyui-dialog" style="overflow: hidden;">
        <table id="gridOtherDeptEmps" fit="true" fitcolumns="true" toolbar="#glryTb" class="easyui-datagrid"></table>
        <div id="glryTb">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok',plain:true" onclick="SaveCheckedEmps()">确定</a> 
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel',plain:true" onclick="$('#connecteEmp').dialog('close');">关闭</a>
        </div>
        <input type="hidden" id="HD_EmpsFrom" value="connecte" />
    </div>
</body>
</html>
