<%@ Page Title="操作" Language="C#" MasterPageFile="OneWork.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.OneWork.WF_WorkOpt_OneWork_OP" CodeBehind="OP.aspx.cs" %>

<%@ Register Src="../../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="../../Comm/JScript.js" type="text/javascript"></script>
    <script type="text/javascript">
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

        //删除流程.
        function DeleteFlowInstance(flowNo, workid) {
            var url = '../DeleteFlowInstance.htm?FK_Flow=' + flowNo + '&WorkID=' + workid;
            WinOpen(url);
        }
        function DoFunc(doType, workid, fk_flow, fk_node) {

            if (doType == 'Del' || doType == 'Reset') {
                if (confirm('您确定要执行吗？') == false)
                    return;
            }

            var url = '';
            if (doType == 'HungUp' || doType == 'UnHungUp') {
                url = './../HungUpOp.aspx?WorkID=' + workid + '&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node;
                var str = window.showModalDialog(url, '', 'dialogHeight: 350px; dialogWidth:500px;center: no; help: no');
                if (str == undefined)
                    return;
                if (str == null)
                    return;
                //this.close();
                window.location.href = window.location.href;
                return;
            }
            url = 'OP.aspx?DoType=' + doType + '&WorkID=' + workid + '&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node;
            window.location.href = url;
        }
        function Takeback(workid, fk_flow, fk_node, toNode) {
            if (confirm('您确定要执行吗？') == false)
                return;
            var url = '../../GetTask.aspx?DoType=Tackback&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&ToNode=' + toNode + '&WorkID=' + workid;
            window.location.href = url;
        }
        function UnSend(fk_flow, workID,fid) {

//            var url = "CancelWork.aspx?WorkID=" + workID + "&FK_Flow=" + fk_flow+"&FID="+fid+"&FK_Node="+"<%=FK_Node %>";
//            WinShowModalDialog_Accepter(url);

            if (confirm('您确定要执行撤销吗?') == false)
                return;

            var url = "OP.aspx?DoType=UnSend&FK_Node=<%=FK_Node %>&FK_Flow=" + fk_flow + "&WorkID=" + workID;
            $.post(url, null, function (msg) {
                $('#winMsg').html(msg);
                $('#winMsg').window('open');
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'center'" style="padding: 5px">
            <uc1:Pub ID="Pub2" runat="server" />
        </div>
    </div>
    <div id="winMsg" class="easyui-window" title="信息" data-options="modal:true,closed:true,iconCls:'icon-tip'"
        style="width: 350px; height: 150px; padding: 10px">
    </div>
</asp:Content>
