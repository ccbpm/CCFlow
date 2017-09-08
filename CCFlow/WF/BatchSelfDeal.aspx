<%@ Page Title="批量处理" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true"
    CodeBehind="BatchSelfDeal.aspx.cs" Inherits="CCFlow.WF.BatchSelfDeal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>

    <%
        BP.WF.Template.BtnLab btn2 = new BP.WF.Template.BtnLab(this.FK_Node);
        
         %>
    <script type="text/javascript">
        //. 保存嵌入式表单. add 2015-01-22 for GaoLing.
        //谷歌不支持window.showModalDialog模式窗体用window.open()
        var _isOpenAccepter;
        function SaveSelfFrom() {

            if ( <%=btn2.SelectAccepterEnable %> != '0') {
                var url = '/WF/WorkOpt/Accepter.htm?FK_Flow=<%=FK_Flow %>&FK_Node=<%=FK_Node %>&WorkIDs=<%=WorkIDs %>&WorkID=0&FID=0';
                var returnVal= window.showModalDialog(url, "sd", "scrollbars=yes,resizable=yes,center=yes,minimize=yes,maximize=yes,height= 600px,width= 550px, top=50px, left= 650px,alwaysRaised=yes,depended=yes");
               //alert(returnVal);
               if (returnVal == null || returnVal == 'undefined' || returnVal == 'cancel') {
                 // alert('您没有选择要接受的人员，所以不能发送。');
                    return false;
                }
            }

            var frm = document.getElementById('SelfForm');
            if (frm == null) {
                alert('系统错误,没有找到ifromid.');
                return false;
            }

            //执行保存. 
            try {
                var v = frm.contentWindow.Save();
                if (v == 1 || v == "1" || v == true) {
                    return true;
                }
                return false;
            } catch (e) {
                alert('保存表单错误，有可能您的嵌入式表单('+frm.src+')没有Save函数，详细信息：\t\n' + e.message);
                return false;
            }
        }
        function windowReturnValue(str) {
            alert(str);
            if (str == null || str == 'undefined' || str == 'cancel') {
                alert('您没有选择要接受的人员，所以不能发送。');
                return false;
            }
            if (confirm('您确定要发送吗？') == false)
                return false;

            var frm = document.getElementById('SelfForm');
            if (frm == null) {
                alert('系统错误,没有找到ifromid.');
                return false;
            }

            //执行保存.------------------------------------------------------不知道什么意思
            return frm.contentWindow.Save();
           
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
        BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
        string url = nd.BatchParas;
        if (url.Contains("?"))
            url = url + "&FK_Node=" + this.FK_Node + "&WorkIDs=" + this.WorkIDs+"&FK_Flow="+this.FK_Flow;
        else
            url = url + "?FK_Node=" + this.FK_Node + "&WorkIDs=" + this.WorkIDs + "&FK_Flow=" + this.FK_Flow;
    %>
    <br />
    <% 
        /*说明: 选择IDs*/
        System.Data.DataTable table = BP.WF.Dev2Interface.GetBatch(this.FK_Node);

        //用于判断是否有接收人选择器.
        BP.WF.Template.BtnLab btn = new BP.WF.Template.BtnLab(this.FK_Node);
        //  btn.SelectAccepterEnable
    %>
    <table class='table' id='ifreamTable' style='width: 100%; margin-top: -15px; height: 100%;'>
        <caption>
            工作批处理</caption>
        <tr>
            <td>
                <fieldset>
                </fieldset>
                <iframe src="<%= url %>" id="SelfForm" style="width: 100%; height: 500px;">
                </iframe>
                <asp:Button ID="Btn_Send" runat="server" Text="批量发送" OnClick="Btn_Send_Click" OnClientClick="return SaveSelfFrom()"  />
                
            </td>
        </tr>
    </table>
     <div id="ErrorMessage" runat="server" style="font-size: 15px; color: Red;">
                </div>
</asp:Content>
