<%@ Page Title="流转自定义" Language="C#" MasterPageFile="../SDKComponents/Site.master"
    AutoEventWireup="true" CodeBehind="TransferCustom.aspx.cs" Inherits="CCFlow.WF.WorkOpt.TransferCustomUI" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <style type="text/css">
        .headerStyle
        {
            height: 30px;
            line-height: 30px;
        }
    </style>
    <script type="text/javascript">
        //by liuxc
        var txtId;
        var hiddenId;
        var currObj;

        var DF = {
            COL_IDX: ":eq(0)",
            COL_NODE_NAME: ":eq(1)",
            COL_SUB_FLOW: ":eq(2)",
            COL_WORKER: ":eq(3)",
            COL_CC: ":eq(4)",
            COL_TO_MODEL: ":eq(5)",
            COL_DATE: ":eq(6)",
            COL_FUNC: ":last-child",
            COL_COUNT: 8
        };

        $(document).ready(function () {
            //计算列数
            DF.COL_COUNT = $("table tr").first().children().length;

            //设置“处理模式”的值
            $.each($("input[id*='hid_todolistmodel_']"), function(){
                //alert($(this).val());
                $(this).parent().children(":eq(0)").combobox("select", this.value);
            });

            $("table tr:gt(0)").hover(
                function () { $(this).addClass("tr_hover"); },
                function () { $(this).removeClass("tr_hover"); });
                        $('#mainLayout').layout('panel', 'center').panel('setTitle', "你好：<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name).Replace("\"","\\\"") %>");
        });

        //选择处理人后操作
        function selectEmp(obj) {
            txtId = $(obj).prev().attr('id');
            hiddenId = $(obj).next().attr('id');

            var url = '../Comm/Port/SelectUser_jq.aspx';
            var selected = $('#' + hiddenId).val();
            if (selected != null && selected.length > 0) {
                url += '?In=' + selected + '&tk=' + Math.random();
            }

            OpenEasyUiDialog(url, 'eudlgframe', '选择人员', 760, 470, 'icon-user', true, function () {
                var innerWin = document.getElementById('eudlgframe').contentWindow;
                $('#' + txtId).text(innerWin.getReturnText());
                $('#' + hiddenId).val(innerWin.getReturnValue());
            });
        }

        //选择步骤或流程后操作
        function selectNode(iframeId) {
            var innerWin = document.getElementById(iframeId).contentWindow;
            var selectType = innerWin.getSelectedType();
            var selectValue = innerWin.getSelectedValue();

            if (selectValue == null || selectValue == undefined) {
                $.messager.alert('错误', '请选择步骤或流程！', 'error');
                return;
            }

            if (selectType == "node") {
                appendNode(selectValue);
            }
            else {
                if (selectValue.split(',').length < 2) {
                    $.messager.alert('错误', '您选择的是流程类别，不是流程，请重新选择！', 'error');
                    return;
                }

                appendFlow(selectValue);
            }
        }

        //增加流程步骤
        //val格式：Step,NodeId,NodeName
        function appendNode(val) {
            var arr = val.split(',');
            var step = arr[0];
            var nodeId = arr[1];
            var nodeName = arr[2];
            var newTR;
            var existNodes = $("tr[data-node='" + nodeId + "']");

            //如果当前步骤在本页面中没有相关数据，则使用返回的val数据创建一个
            if (existNodes.length == 0) {
                newTR = $("tr[data-desc='true']").first().clone();
                newTR.children(DF.COL_IDX).children(":eq(0)").attr('value', step);
                newTR.children(DF.COL_IDX).children(":eq(2)").attr('value', nodeId);
                newTR.children(DF.COL_NODE_NAME).text(nodeName);
                newTR.children(DF.COL_SUB_FLOW).children(":eq(0)").text('——');
                newTR.children(DF.COL_SUB_FLOW).children(":eq(1)").hide();
                newTR.children(DF.COL_SUB_FLOW).children(":eq(2)").attr('value', '');
                newTR.children(DF.COL_WORKER).children(":eq(0)").text('');
                newTR.children(DF.COL_WORKER).children(":eq(2)").attr('value', '');
                newTR.children(DF.COL_CC).children(":eq(0)").text('');
                newTR.children(DF.COL_CC).children(":eq(2)").attr('value', '');                
                newTR.children(DF.COL_DATE).text('——');
            }
            else {
                newTR = existNodes.first().clone();
            }

            //必须重新命名处理人的这两个控件ID，因为这两个ID要用到，不能重复
            newTR.children(DF.COL_WORKER).children(":eq(0)").attr('id', 'worker_' + nodeId + randomid());
            newTR.children(DF.COL_WORKER).children(":eq(2)").attr('id', 'hid_emp' + randomid());
            newTR.children(DF.COL_CC).children(":eq(0)").attr('id', 'cc_' + nodeId + randomid());
            newTR.children(DF.COL_CC).children(":eq(2)").attr('id', 'hid_cc' + randomid());
            newTR.children(DF.COL_TO_MODEL).children(":eq(0)").attr('id', 'todomodel_' + nodeId + randomid());
            newTR.children(DF.COL_TO_MODEL).children(":eq(2)").attr('id', 'hid_todolistmodel_' + randomid());
            newTR.insertAfter(currObj);

            if (newTR.attr('data-desc') == 'nocalc') {
                newTR.children(DF.COL_FUNC).detach();
                newTR.append($("tr[data-desc='true']").first().children(":last-child").clone());
                newTR.attr('data-desc', 'true');
                newTR.children(DF.COL_WORKER).children(":eq(1)").show();
                newTR.children(DF.COL_CC).children(":eq(1)").show();
                newTR.children(DF.COL_DATE).text('——');
            }
            else if (newTR.attr('data-desc') == 'false') {
                newTR.attr('data-desc', 'true');
                newTR.children(DF.COL_WORKER).children(":eq(1)").show();
                newTR.children(DF.COL_CC).children(":eq(1)").show();
                newTR.children(DF.COL_FUNC).children(":hidden").show();
                newTR.children(DF.COL_FUNC).children(":last-child").hide();
            }

            newTR.children(DF.COL_TO_MODEL).children(":eq(0)").combobox({disabled:false});
            newTR.children(DF.COL_TO_MODEL).children(":eq(2)").remove();      

            checkIdx();
            newTR.hover(
                function () { $(this).addClass("tr_hover"); },
                function () { $(this).removeClass("tr_hover"); });
        }

        //增加流程
        //val格式：SubFlowNo,SubFlowName
        function appendFlow(val) {
            var arr = val.split(',');
            currObj.children(DF.COL_SUB_FLOW).children(":eq(0)").text(arr[1]);
            currObj.children(DF.COL_SUB_FLOW).children(":eq(1)").show();
            currObj.children(DF.COL_SUB_FLOW).children(":eq(2)").val(arr[0]);
        }

        //删除子流程
        function deleteSubFlow(obj) {
            $(obj).prev().text('——');
            $(obj).next().val('');
            $(obj).hide();
        }

        //保存
        function saveIdx() {
            var str = '';
            $.each($("tr[data-desc='true']"), function () {
                str += '@' + $(this).children(DF.COL_IDX).children(':last-child').val() + '`';  //节点ID
                str += $(this).children(DF.COL_SUB_FLOW).children(':last-child').val() + '`';   //子流程NO
                str += $(this).children(DF.COL_TO_MODEL).children(':last-child').val() + '`'; //处理模式
                str += $(this).children(DF.COL_WORKER).children(':last-child').val() + "`"; //处理人
                str += $(this).children(DF.COL_CC).children(':last-child').val() + "`"; //抄送人编号
                str += $(this).children(DF.COL_CC).children(':eq(0)').text();    //抄送人名称
            });

            $('#<%=hid_idx_all.ClientID %>').val(str);
            return true;
        }

        //上移
        function up(obj) {
            var objParentTR = $(obj).parent().parent();
            var prevTR = objParentTR.prev();
            if (prevTR.length > 0 && prevTR.children().length == DF.COL_COUNT) {
                prevTR.insertAfter(objParentTR);
                checkIdx();
            } else {
                return;
            }
        }

        //下移
        function down(obj) {
            var objParentTR = $(obj).parent().parent();
            var nextTR = objParentTR.next();
            if (nextTR.length > 0 && nextTR.children().length == DF.COL_COUNT) {
                nextTR.insertBefore(objParentTR);
                checkIdx();
            } else {
                return;
            }
        }

        //禁用
        function forbid(obj) {
            var objParentTR = $(obj).parent().parent();
            var lastTR = objParentTR.parent().children(":last-child");
            objParentTR.attr('data-desc', 'false');
            objParentTR.children(DF.COL_IDX).children(":eq(1)").text('——');
            objParentTR.children(DF.COL_WORKER).children(":eq(1)").hide();
            objParentTR.children(DF.COL_CC).children(":eq(1)").hide();
            objParentTR.children(DF.COL_TO_MODEL).children(":eq(0)").hide();
            objParentTR.children(DF.COL_TO_MODEL).children(":eq(1)").hide();
            objParentTR.children(DF.COL_FUNC).children(":visible").hide();
            objParentTR.children(DF.COL_FUNC).children(":last-child").show();
            objParentTR.insertAfter(lastTR);
            checkIdx();
            objParentTR.removeClass("tr_hover");
        }

        //启用
        function use(obj) {
            var objParentTR = $(obj).parent().parent();
            var forbidTR = $("tr[data-desc='forbid']");
            objParentTR.attr('data-desc', 'true');
            objParentTR.children(DF.COL_WORKER).children(":eq(1)").show();
            objParentTR.children(DF.COL_CC).children(":eq(1)").show();
            objParentTR.children(DF.COL_TO_MODEL).children(":eq(1)").show();
            objParentTR.children(DF.COL_FUNC).children(":hidden").show();
            objParentTR.children(DF.COL_FUNC).children(":last-child").hide();
            objParentTR.insertBefore(forbidTR);
            checkIdx(objParentTR);
            objParentTR.removeClass("tr_hover");
        }

        //插入步骤/流程
        function insert(obj) {
            var objParentTR = $(obj).parent().parent();
            var workerid = objParentTR.children(DF.COL_WORKER).children(":last-child").val();
            var url = 'SelectNode.aspx?FK_Node=<%=this.FK_Node %>&FK_Flow=<%=this.FK_Flow %>&WorkID=<%=this.WorkID %>&FID=<%=this.FID %>&WorkerId=' + workerid;

            currObj = $(obj).parent().parent();
            OpenEasyUiDialog(url, 'eudlgframe', '插入步骤/流程', 400, 360, 'icon-insert', true, selectNode, 'eudlgframe');
        }

        //检查修正序号
        function checkIdx(objTR) {
            var i = $("tr[data-desc='nocalc']").length;

            if (objTR != undefined && objTR != null) {
                objTR.children(DF.COL_IDX).children(":eq(1)").text('第' + (i + $("tr[data-desc='true']").length) + '步');
            }
            else {
                $.each($("tr[data-desc='true']"), function (idx, tr) {
                    $(tr).children(DF.COL_IDX).children(":eq(1)").text('第' + (i + idx + 1) + '步');
                });
            }
        }

        function setTodolistModel(record, obj){
            $(obj).parent().children(":last-child").val(record.value);
        }

        //获取随机小数后面的部分，用于命名不重复
        function randomid() {
            return Math.random().toString().substring(2);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="mainLayout" class="easyui-layout" data-options="fit:true">
        <div data-options="region:'center',border:false,title:'<%=this.Title %>',headerCls:'headerStyle'"
            style="padding: 5px">
            <table class="Table" cellpadding="0" cellspacing="0" border="0" style="width: 100%;
                line-height: 22px">
                <tr>
                    <td class="GroupTitle" style="width: 70px; text-align: center">
                        步骤
                    </td>
                    <td class="GroupTitle" style="width: 100px; text-align: center">
                        节点
                    </td>
                    <td class="GroupTitle" style="width: 50px; text-align: center">
                        子流程
                    </td>
                    <td class="GroupTitle" style="width: 140px; text-align: center">
                        处理人
                    </td>
                    <td class="GroupTitle" style="width: 140px; text-align: center">
                        抄送人
                    </td>
                    <td class="GroupTitle" style="width: 70px; text-align: center">
                        处理模式
                    </td>
                    <td class="GroupTitle" style="width: 80px; text-align: center">
                        日期
                    </td>
                    <td class="GroupTitle" style="width: 240px; text-align: center">
                        操作
                    </td>
                </tr>
                <tr>
                    <td colspan="8" class="GroupTitle">
                        已完成的步骤
                    </td>
                </tr>
                <asp:Repeater ID="rptOverNodes" runat="server">
                    <ItemTemplate>
                        <tr data-desc="nocalc" data-node='<%# Eval("FK_Node") %>'>
                            <td class="Idx">
                                <asp:HiddenField ID="hid_idx" runat="server" Value='<%# Eval("Step") %>' />
                                <span>第<%# Eval("Step") %>步</span>
                                <asp:HiddenField ID="hid_node" runat="server" Value='<%# Eval("FK_Node") %>' />
                            </td>
                            <td>
                                <%# Eval("FK_NodeText")%>
                            </td>
                            <td>
                                <span>——</span> <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-delete',plain:true"
                                    onclick="deleteSubFlow(this)" title="删除" style="display: none"></a>
                                <input type="hidden" id="hid_subflow" value="" />
                            </td>
                            <td style="text-align: center">
                                <span id='worker_<%# Eval("FK_Node") %>_<%# Eval("Step") %>'>
                                    <%# Eval("FK_EmpText")%></span> <a href="javascript:void(0)" class="easyui-linkbutton"
                                        data-options="iconCls:'icon-user'" onclick="selectEmp(this)" title="修改" style="display: none">
                                    </a>
                                <asp:HiddenField ID="hid_emp" runat="server" Value='<%# Eval("FK_Emp") %>' />
                            </td>
                            <td style="text-align: center">
                                <span id='cc_<%# Eval("FK_Node") %>_<%# Eval("Step") %>'>
                                    <%# Eval("FK_CCText")%></span> <a href="javascript:void(0)" class="easyui-linkbutton"
                                        data-options="iconCls:'icon-user'" onclick="selectEmp(this)" title="修改" style="display: none">
                                    </a>
                                <asp:HiddenField ID="hid_cc" runat="server" Value='<%# Eval("FK_CC") %>' />
                            </td>
                            <td style="text-align: center">
                                <select id='todomodel_<%# Eval("FK_Node") %>_<%# Eval("Step") %>' class="easyui-combobox"
                                    style="width: 100px;" data-options="disabled:true,onSelect:function(record){setTodolistModel(record, this);}">
                                    <option value="0" selected="selected">抢办模式</option>
                                    <option value="1">协作模式</option>
                                    <option value="2">队列模式</option>
                                    <option value="3">共享模式</option>
                                </select>
                                <asp:HiddenField ID="hid_todolistmodel" runat="server" Value='<%# Eval("TodolistModel") %>' />
                            </td>
                            <td style="text-align: center">
                                <%# Eval("RDT").ToString().Split(' ')[0] %>
                            </td>
                            <td style="text-align: center">
                                ——
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td colspan="8" class="GroupTitle">
                        当前步骤
                    </td>
                </tr>
                <tr data-desc="nocalc">
                    <td class="Idx">
                        第<asp:Literal ID="litCurrentStep" runat="server"></asp:Literal>步
                    </td>
                    <td>
                        <asp:Label ID="lblFK_NodeText" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        ——
                    </td>
                    <td style="text-align: center">
                        <asp:Label ID="lblFK_EmpText" runat="server" Text=""></asp:Label>
                    </td>
                    <td style="text-align: center">
                        <asp:Label ID="lblFK_CCText" runat="server" Text=""></asp:Label>
                    </td>
                    <td style="text-align: center">
                        <asp:Label ID="lblTodoListModelText" runat="server" Text=""></asp:Label>
                    </td>
                    <td style="text-align: center">
                        <asp:Label ID="lblRDT" runat="server" Text=""></asp:Label>
                    </td>
                    <td style="text-align: center">
                        ——
                    </td>
                </tr>
                <tr data-desc="order">
                    <td colspan="8" class="GroupTitle">
                        流程运转步骤排列
                    </td>
                </tr>
                <asp:Repeater ID="rptNextNodes" runat="server">
                    <ItemTemplate>
                        <tr data-desc="true" data-node='<%# Eval("FK_Node") %>'>
                            <td class="Idx">
                                <asp:HiddenField ID="hid_idx" runat="server" Value='<%# Eval("Idx") %>' />
                                <span>第<%# Eval("Idx") %>步</span>
                                <asp:HiddenField ID="hid_node" runat="server" Value='<%# Eval("FK_Node") %>' />
                            </td>
                            <td>
                                <%# Eval("FK_NodeText")%>
                            </td>
                            <td>
                                <span>
                                    <%#Eval("SubFlowName") %></span> <a href="javascript:void(0)" class="easyui-linkbutton"
                                        data-options="iconCls:'icon-delete',plain:true" onclick="deleteSubFlow(this)"
                                        title="删除" style='display: <%# Eval("SubFlowNo").ToString().Length>0 ?"inline-block":"none" %>'>
                                    </a>
                                <input type="hidden" id="hid_subflow" value='<%#Eval("SubFlowNo") %>' />
                            </td>
                            <td style="text-align: center">
                                <span id='worker_<%# Eval("FK_Node") %>_<%# Eval("Idx") %>'>
                                    <%# Eval("WorkerText")%></span> <a href="javascript:void(0)" class="easyui-linkbutton"
                                        data-options="iconCls:'icon-user'" onclick="selectEmp(this)" title="修改">
                                </a>
                                <asp:HiddenField ID="hid_emp" runat="server" Value='<%# Eval("Worker") %>' />
                            </td>
                            <td style="text-align: center">
                                <span id='cc_<%# Eval("FK_Node") %>_<%# Eval("Idx") %>'>
                                    <%# Eval("FK_CCText")%></span> <a href="javascript:void(0)" class="easyui-linkbutton"
                                        data-options="iconCls:'icon-user'" onclick="selectEmp(this)" title="修改">
                                </a>
                                <asp:HiddenField ID="hid_cc" runat="server" Value='<%# Eval("FK_CC") %>' />
                            </td>
                            <td style="text-align: center">
                                <select id='todomodel_<%# Eval("FK_Node") %>_<%# Eval("Idx") %>' class="easyui-combobox"
                                    style="width: 100px;" data-options="onSelect:function(record){setTodolistModel(record, this);}">
                                    <option value="0" selected="selected">抢办模式</option>
                                    <option value="1">协作模式</option>
                                    <option value="2">队列模式</option>
                                    <option value="3">共享模式</option>
                                </select>
                                <asp:HiddenField ID="hid_todolistmodel" runat="server" Value='<%# Eval("TodolistModel") %>' />
                            </td>
                            <td style="text-align: center">
                                ——
                            </td>
                            <td style="text-align: center">
                                <a href="#" class="easyui-linkbutton" onclick="up(this)" data-options="iconCls:'icon-up'"
                                    title="上移"></a>&nbsp;<a href="#" class="easyui-linkbutton" onclick="down(this)" data-options="iconCls:'icon-down'"
                                        title="下移"> </a>&nbsp;<a href="#" class="easyui-linkbutton" onclick="insert(this)"
                                            data-options="iconCls:'icon-insert'" title="插入步骤/流程"> </a>&nbsp;<a href="#" class="easyui-linkbutton"
                                                onclick="forbid(this)" data-options="iconCls:'icon-delete2'" title="禁用">
                                </a>&nbsp;<a href="#" class="easyui-linkbutton" onclick="use(this)" data-options="iconCls:'icon-add2'"
                                    title="启用" style="display: none">
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr data-desc="forbid">
                    <td colspan="8" class="GroupTitle">
                        已禁用的步骤
                    </td>
                </tr>
            </table>
            <p style="text-align: center">
                <asp:HiddenField ID="hid_idx_all" runat="server" />
                <asp:LinkButton ID="lbtnUseAutomic" runat="server" CssClass="easyui-linkbutton" OnClick="lbtnUseAutomic_Click"
                    data-options="iconCls:'icon-auto'">执行自动运行模式并返回</asp:LinkButton>&nbsp;&nbsp;<asp:LinkButton
                        ID="lbtnUseManual" runat="server" CssClass="easyui-linkbutton" OnClick="lbtnUseManual_Click"
                        OnClientClick="saveIdx()" data-options="iconCls:'icon-manual'">执行手动设置运行模式并返回</asp:LinkButton>
            </p>
        </div>
    </div>
</asp:Content>
