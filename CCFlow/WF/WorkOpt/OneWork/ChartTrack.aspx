<%@ Page Title="轨迹图" Language="C#" MasterPageFile="OneWork.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.OneWork.WF_WorkOpt_OneWork_ChartTrack" CodeBehind="ChartTrack.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" language="javascript">
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
            return retval;
        }

        $(function () {
            var fid = getArgsFromHref("FID");
            var fk_flow = getArgsFromHref("FK_Flow");
            var workId = getArgsFromHref("WorkID");
            var iframe = document.getElementById("content");
//            iframe.src = "ChartTrack.htm?FID=" + fid + "&FK_Flow=" + fk_flow + "&WorkID=" + workId; //SL版轨迹图
            iframe.src = "../../Admin/CCBPMDesigner/truck/Truck.htm?FID=" + fid + "&FK_Flow=" + fk_flow + "&WorkID=" + workId;    //JS版轨迹图
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'center'" style="overflow-y: hidden;
            padding: 5px">
            <iframe id="content" frameborder="0" scrolling="no" style="width: 100%; height: 100%">
            </iframe>
        </div>
    </div>
</asp:Content>
