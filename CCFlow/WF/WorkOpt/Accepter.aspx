<%@ Page Language="C#" MasterPageFile="../SDKComponents/AccSite.Master" AutoEventWireup="true"
    Inherits="CCFlow.WF.WF_Accepter" Title="接受人选择器" CodeBehind="Accepter.aspx.cs" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/CommonUnite.js" type="text/javascript"></script>
    <style type="text/css">
        .ctitle
        {
            text-align: center;
            color: #555;
            font-size: 18px;
            padding: 10px;
        }
    </style>
    <script type="text/javascript">
        //调用发送按钮
        function send() {
            var btn = window.opener.document.getElementById('ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_Btn_Send');
            if (btn) {
                window.opener.document.getElementById('ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_Btn_Send').click();
            }
            window.close();
        }
        function SetSelected(cb, ids) {
            var arrmp = ids.split(',');
            var arrObj = document.all;
            var isCheck = false;
            if (cb.checked)
                isCheck = true;
            else
                isCheck = false;
            for (var i = 0; i < arrObj.length; i++) {
                if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                    for (var idx = 0; idx <= arrmp.length; idx++) {
                        if (arrmp[idx] == '')
                            continue;
                        var cid = arrObj[i].name + ',';
                        var ctmp = arrmp[idx] + ',';
                        if (cid.indexOf(ctmp) > 1) {
                            arrObj[i].checked = isCheck;
                        }
                    }
                }
            }
        }

        function LoadDataGridCallBack(js, scorp) {

            $("#pageloading").hide();
            if (js == "") js = "[]";
            //系统错误
            if (js.status && js.status == 500) {
                $("body").html("<b>访问页面出错，请联系管理员。<b>");
                return;
            }

            var pushData = eval('(' + js + ')');

            $('#cc').combobox({
                data: pushData.ddl,
                valueField: 'id',
                textField: 'text',
                onSelect: function (r) {
                    var ee = $('#checkedTt').tree('find', 'CheId');
                    var children = $('#checkedTt').tree('getChildren');
                    for (var i = 0; i < children.length; i++) {
                        if (children[i].id == r.id) {
                            return;
                        }
                    }
                    $('#checkedTt').tree('append', {
                        parent: ee.target,
                        data: [{
                            id: r.id,
                            iconCls: "icon-user",
                            text: r.text
                        }]
                    });
                }
            });
            $("#checkedTt").tree({
                idField: 'id',
                data: pushData.CheId,
                animate: true,
                lines: true,
                onClick: function (node) { delNode = node; },
                onDblClick: function (node) {
                    delNode = node;
                    delEmp();
                }
            });
            $("#tt").tree({
                idField: 'id',
                iconCls: 'tree-folder',
                data: pushData.tt,
                animate: true,
                width: 300,
                height: 400,
                lines: true,
                onClick: function (node) { addNode = node; },
                onDblClick: function (node) {
                    addNode = node;
                    addEmp();
                }
            });
        }

        var addNode;
        var delNode;
        function addEmp() {
            if (addNode == null) {
                $.messager.alert("提示", "请选择左侧要添加的人员！", "info");
                return;
            }
            var ee = $('#checkedTt').tree('find', 'CheId');
            var children = $('#checkedTt').tree('getChildren');
            if (addNode.attributes["IsParent"] == 0) {
                for (var i = 0; i < children.length; i++) {
                    if (children[i].id == addNode.id) {
                        $.messager.alert("提示", "已添加！", "info");
                        return;
                    }
                }
                $('#checkedTt').tree('append', {
                    parent: ee.target,
                    data: [{
                        id: addNode.id,
                        iconCls: "icon-user",
                        text: addNode.text
                    }]
                });
            }
        }

        function delEmp() {
            if (delNode == null) {
                $.messager.alert("提示", "请选择右侧要删除的人员！", "info");
                return;
            }
            if (delNode.id != 'CheId') {
                $('#checkedTt').tree('remove', delNode.target);
            }
            delNode = null;
        }

        //加载DeptTree
        function LoadTreeData() {
            var params = {
                method: "getTreeDateMet",
                FK_Node: FK_Node,
                WorkID: WorkID,
                FK_Flow: FK_Flow,
                FID: FID,
                ToNode: ToNode,
                IsWinOpen: IsWinOpen,
                FK_Dept: FK_Dept,
                FK_Station: FK_Station,
                WorkIDs: WorkIDs,
                DoFunc: DoFunc,
                CFlowNo: CFlowNo,
                FK_Flow: FK_Flow
            };

            queryData(params, LoadDataGridCallBack, this);
        }
        //初始化
        var FK_Node;
        var WorkID;
        var FK_Flow;
        var FID;
        var ToNode;
        var IsWinOpe;
        var FK_Dept;
        var FK_Stati;
        var WorkIDs;
        var DoFunc;
        var CFlowNo;
        var Type;
        var userDo;
        $(function () {
            $('#win').window('close');

            FK_Node = Application.common.getArgsFromHref("FK_Node");
            WorkID = Application.common.getArgsFromHref("WorkID");
            FK_Flow = Application.common.getArgsFromHref("FK_Flow");
            FID = Application.common.getArgsFromHref("FID");
            ToNode = Application.common.getArgsFromHref("ToNode");
            IsWinOpen = Application.common.getArgsFromHref("IsWinOpen");
            FK_Dept = Application.common.getArgsFromHref("FK_Dept");
            FK_Station = Application.common.getArgsFromHref("FK_Station");
            WorkIDs = Application.common.getArgsFromHref("WorkIDs");
            DoFunc = Application.common.getArgsFromHref("DoFunc");
            CFlowNo = Application.common.getArgsFromHref("CFlowNo");
            Type = Application.common.getArgsFromHref("FK_Type");
            userDo = Application.common.getArgsFromHref("userDo");
            if ($.messager) {
                $.messager.defaults.ok = '确定';
                $.messager.defaults.cancel = '取消';
            }

            LoadTreeData();
        });
        var getSaveNo;
        function getSelectEmp() {
            getSaveNo = "";
            var childrenNodes = $('#checkedTt').tree('getChildren');

            if (childrenNodes.length == 1) {
                $.messager.alert("提示", "您没有选择人员!", "info");
            }
            else {
                for (var i = 0; i < childrenNodes.length; i++) {
                    if (childrenNodes[i].id != 'CheId') {
                        if (getSaveNo != '') getSaveNo += ',';
                        getSaveNo += childrenNodes[i].id;
                    }
                }
            }
        }
        function getChecked() {
            getSelectEmp();
            if (getSaveNo == "") {
                return;
            }

            var getSaveName = ''; //选择前五个,多余的...表示
            var hasSelect = $('#checkedTt').tree('getChildren');
            for (var selectN = 1; selectN < hasSelect.length; selectN++) {
                if (selectN == 5) {
                    getSaveName += hasSelect[selectN].text + '...';
                    break;
                } else {
                    if (selectN == hasSelect.length - 1) {
                        getSaveName += hasSelect[selectN].text;
                    } else {
                        getSaveName += hasSelect[selectN].text + ',';
                    }
                }
            }
            try {
                window.opener.document.getElementById("acc_link_" + ToNode).innerHTML = "选择接受人员" + "<span style='color:black;'>(" + getSaveName + ")</span>";
            } catch (e) {
                //window.parent.document.getElementById("acc_link_" + ToNode).innerHTML = "选择接受人员" + "<span style='color:black;'>(" + getSaveName + ")</span>";
            }

            var params = {
                method: "saveMet",
                getSaveNo: getSaveNo,
                FK_Node: FK_Node,
                WorkID: WorkID,
                FK_Flow: FK_Flow,
                FID: FID,
                ToNode: ToNode,
                IsWinOpen: IsWinOpen,
                FK_Dept: FK_Dept,
                FK_Station: FK_Station,
                WorkIDs: WorkIDs,
                DoFunc: DoFunc,
                CFlowNo: CFlowNo,
                FK_Flow: FK_Flow
            };
            queryData(params, function (js, scope) {
                var type = Application.common.getArgsFromHref("type");
                //发送前打开人员选择器，选择人员后自动发送
                if (type == "2") {
                    send();
                }
            }, this);

            window.returnValue = 'ok';
            window.close();
        }
        //关闭
        function cancelMet() {
            //            window.ReturnVal = 'cancel';
            window.close();
            try {
                window.opener.windowReturnValue('cancel');
            } catch (e) {

            }
        }

        function setVal(node) {
            var ee = $('#checkedTt').tree('find', 'CheId');
            var children = $('#checkedTt').tree('getChildren');
            for (var i = 0; i < children.length; i++) {
                if (children[i].id == node.id) {
                    return;
                }
            }
            $('#checkedTt').tree('append', {
                parent: ee.target,
                data: [{
                    id: node.id,
                    iconCls: "icon-user",
                    text: node.innerHTML
                }]
            });
        }
        function clearData() {
            $.messager.confirm('警告', '确定重置已选人员吗?', function (y) {
                if (y) {
                    $('#cs_message').val('');
                    $('#cs_title').val('');

                    var ee = $('#checkedTt').tree('find', 'CheId');
                    var son = $('#checkedTt').tree('getChildren', ee.target);
                    for (var i = 0; i < son.length; i++) {
                        $('#checkedTt').tree('remove', son[i].target);
                    }
                }
            });
        }
        //新增抄送功能  qin 15/6/16
        var cs_messageV;
        var cs_titleV;
        function copyTo() {
            cs_messageV = $('#cs_message').val();
            cs_titleV = $('#cs_title').val();

            if (cs_messageV == "" || cs_titleV == "") {
                $('#win').window('open');
            }
            else {
                cc_isOk();
            }
        }
        //公共方法
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "Accepter.aspx", //要访问的后台地址
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

        function cc_isCancel() {
            $('#cs_message').val('');
            $('#cs_title').val('');
            $('#win').window('close');
        }
        function cc_isOk() {
            getSelectEmp();
            if (getSaveNo == "") {
                return;
            }
            cs_messageV = $('#cs_message').val();
            cs_titleV = $('#cs_title').val();

            var params = {
                method: "copyToMet",
                getSaveNo: getSaveNo,
                cs_messageV: encodeURI(cs_messageV),
                cs_titleV: encodeURI(cs_titleV),
                FK_Node: FK_Node,
                WorkID: WorkID,
                FK_Flow: FK_Flow,
                FID: FID,
                ToNode: ToNode,
                IsWinOpen: IsWinOpen,
                FK_Dept: FK_Dept,
                FK_Station: FK_Station,
                WorkIDs: WorkIDs,
                DoFunc: DoFunc,
                CFlowNo: CFlowNo,
                FK_Flow: FK_Flow
            };
            queryData(params, function (js, scope) { }, this);
            $('#win').window('close');
            $.messager.alert("提示", "抄送成功!", "info");
        }

        function NoSubmit(ev) {
            if (window.event.srcElement.tagName == "TEXTAREA")
                return true;
            if (ev.keyCode == 13) {
                window.event.keyCode = 9;
                ev.keyCode = 9;
                return true;
            }
            return true;
        }

     
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="easyui-panel" data-options="fit:true,border:false" style="padding: 2px;
        width: 545px; height: 100%; margin: 0px; overflow: hidden;">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'north',split:true,border:false" style="height: 58px;">
                <div id="tb" style="height: 27px; background-color: #E1ECFF;">
                    <a id="copyTo" style="margin-left: 25px;" href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-childline'" onclick="copyTo()">抄送</a> 
                    <a id="clear" href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-reset'" onclick="clearData()">重置</a>
                    <a id="isOk" href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-save'" onclick="getChecked()">确定</a> 
                    <a id="delMeeting" href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-cancel'" onclick="cancelMet()">关闭</a>
                </div>
                <div style="height: 25px; background-color: #E1ECFF; padding-left: 300px;">
                    查询
                    <input id="cc" name="dept" style="width: 150px;" />
                </div>
            </div>
            <div data-options="region:'west',split:false,border:false" style="width: 250px;">
                <%-- <div style="height: 27px; background-color: #E1ECFF; padding-left: 10px;">
                    显示方式&nbsp;<select id="displayWay" class="easyui-combobox" name="dept" style="width: 80px;">
                        <option value="dept">部门</option>
                        <option value="station">岗位</option>
                    </select></div>--%>
                <div region="center" border="true" style="margin-top: 5px; padding: 0; overflow: auto;">
                    <ul class="easyui-tree" id="tt" style="margin-left: 10px;">
                    </ul>
                </div>
            </div>
            <div data-options="region:'center',border:false" style="">
                <div style="width: 28px; height: 99%; border: 1px solid #e1ecff; float: left; position: absolute;">
                    <a href="#" style="margin-left: 0px; margin-top: 200px;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-rightD'" onclick="addEmp();"></a>
                    <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-leftD'" style="margin-top: 20px;" onclick="delEmp();"></a>
                </div>
                <div style="float: left; overflow: auto; margin-left: 30px; position: relative;">
                    <ul class="easyui-tree" id="checkedTt" style="margin-left: 0px; margin-top: 5px;">
                    </ul>
                </div>
            </div>
        </div>
    </div>
    <td class="BigDoc" style="display: none; height: 1px; background-color: red;">
        <uc1:Pub ID="Left" runat="server" Visible="false" />
        <uc1:Pub ID="Pub1" runat="server" />
    </td>
    <div id="win" class="easyui-window" title="抄送" style="width: 500px; height: 400px"
        data-options="iconCls:'icon-save',modal:true">
        <table cellpadding="5">
            <tr>
                <td>标题:</td>
                <td>
                    <input class="easyui-textbox" type="text" id="cs_title" name="title" style="width: 380px;"
                        data-options="required:true" />
                </td>
            </tr>
            <tr>
                <td>消息内容:</td>
                <td>
                    <input class="easyui-textbox" id="cs_message" name="message" style="height: 260px;
                        width: 380px;" />
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="cc_isOk()">确定</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="cc_isCancel()">取消</a>
        </div>
    </div>
</asp:Content>
