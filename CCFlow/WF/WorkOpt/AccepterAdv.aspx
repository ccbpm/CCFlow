<%@ Page Title="接受人选择器" Language="C#" MasterPageFile="~/WF/Rpt/SiteEUI.Master" AutoEventWireup="true"
    CodeBehind="AccepterAdv.aspx.cs" Inherits="CCFlow.WF.WorkOpt.AccepterAdv" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .wordPanel
        {
            width: 100%;
            height: 100%;
            overflow: auto;
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
            text-decoration: none;
        }
        a:active
        {
            text-decoration: none;
        }
        .cs-navi-tab
        {
            padding: 2px;
            display: block;
            line-height: 16px;
            height: 16px;
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
    </style>
    <script language="javascript" type="text/javascript">
        //保存
        function Save() {

        }

        //取消
        function CloseWin() {
            window.close();
        }
        //初始化备用词汇
        function InitReserveWords(NodeID) {
            $("#leftWordPanel").html('');
            $("#rightWordPanel").html('');

            var params = {
                method: 'getreservewords',
                FK_Node: NodeID
            };
            queryData(params, function (js, scope) {
                var pushData = eval('(' + js + ')');

                //左边词汇
                var leftWords = pushData.LeftWords;
                for (var i = leftWords.length - 1; i >= 0; i--) {
                    $("<a id='leftWord" + i + "' class='cs-navi-tab' href=javascript:AddWordToContent('" + leftWords[i].word + "','leftWord" + i + "');>" + leftWords[i].word + "</a>").prependTo("#leftWordPanel");
                }
                //右边词汇
                var rightWords = pushData.RightWords;
                for (var j = leftWords.length-1; j >= 0; j--) {
                    $("<a class='cs-navi-tab' href=javascript:AddWordToContent('" + rightWords[j].word + "');>" + rightWords[j].word + "</a>").prependTo("#rightWordPanel");
                }

            }, this);
        }
        function AddWordToContent(word, ctrID) {
            //            $("p").remove("#second");
            //$("#" + ctrID).remove();     //删除匹配元素
            var content = $("#wordContent").val();
            content += word;
            $("#wordContent").val(content);
        }

        //初始化Tab页
        function InitTabPage() {
            var curNodeID = Application.common.getArgsFromHref("FK_Node");
            var workID = Application.common.getArgsFromHref("WorkID");

            var params = {
                method: 'getdeliverynode',
                FK_Node: curNodeID,
                WorkID: workID
            };
            queryData(params, function (js, scope) {
                var pushData = eval('(' + js + ')');
                
                for (var i = 0; i < pushData.length; i++) {
                    var tabContent = "<div id='" + pushData[i].NodeID + "_Tab'  class='easyui-layout'>";
                    tabContent += "     <div id='" + pushData[i].NodeID + "_Emps' data-options=\"region:'west',split:false,collapsible:false\" title='备选词条及标点' style='width: 180px;overflow: hidden;'>sdd</div>";
                    tabContent += "     <div id='" + pushData[i].NodeID + "_Checked' data-options=\"region:'east',split:false,collapsible:false\" title='备选词条' style='width: 180px;'>sdff</div>";
                    tabContent += "</div>";
                    $('#tabs').tabs('add', {
                        title: pushData[i].Name,
                        id: pushData[i].NodeID,
                        content: tabContent,
                        closable: false
                    });
                }
                //选择Tab页,获取人员与备选词汇
                $('#tabs').tabs({
                    onSelect: function (title, index) {
                        var tab = $('#tabs').tabs('getSelected');
                        InitReserveWords(tab[0].id);
                    }
                });
            }, this);
        }
        $(function () {
            $("#pageloading").hide();
            InitTabPage();
        });

        //公共方法
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "AccepterAdv.aspx", //要访问的后台地址
                data: param, //要发送的数据
                async: false,
                cache: false,
                complete: function () { }, //AJAX请求完成时隐藏loading提示
                error: function (XMLHttpRequest, errorThrown) {
                    $("body").html("<b>访问页面出错，传入参数错误。<b>");
                    //callback(XMLHttpRequest);
                },
                success: function (msg) {//msg为返回的数据，在这里做数据绑定
                    var data = msg;
                    callback(data, scope);
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="pageloading">
    </div>
    <div data-options="region:'north',split:false" style="height: 34px; overflow: hidden;">
        <div style="padding: 3px; background: #fafafa;">
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-save'" onclick="Save()">保存</a> 
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-cancel'" onclick="CloseWin()">取消</a>
        </div>
    </div>
    <div data-options="region:'west',split:false,collapsible:false" title="备选词条及标点" style="width: 180px;
        overflow: hidden;">
        <div id="leftWordPanel" class="wordPanel">
        </div>
    </div>
    <div region="center" border="true" style="margin: 0; padding: 0; overflow: hidden;">
        <div id="tabs" class="easyui-tabs" fit="true" border="false">
        </div>
    </div>
    <div data-options="region:'east',split:false,collapsible:false" title="备选词条" style="width: 180px;">
        <div id="rightWordPanel" class="wordPanel">
        </div>
    </div>
    <div data-options="region:'south',split:false,collapsible:false" title="办理意见" style="height: 160px;">
        <textarea id="wordContent" rows="5" cols="20" style="width: 99%; height: 95%;"></textarea>
    </div>
</asp:Content>
