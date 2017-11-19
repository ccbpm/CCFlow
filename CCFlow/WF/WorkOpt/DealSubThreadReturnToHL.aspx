<%@ Page Title="" Language="C#" MasterPageFile="../SDKComponents/Site.master" AutoEventWireup="true"
    CodeBehind="DealSubThreadReturnToHL.aspx.cs" Inherits="CCFlow.WF.WorkOpt.DealSubThreadReturnToHL" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
    </style>
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

        function ChooseEmp() {

            var hValue = $("#<%=TB_ShiftNo.ClientID %>").val();
            var url = 'SelectUser.aspx?OID=123&CtrlVal=' + hValue;
            var v = window.showModalDialog(url, 'dfg', 'dialogHeight: 450px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
            if (v == null || v == '' || v == 'NaN') {
                return false;
            }

            var arr = v.split('~');
            var emparr = arr[0].split(',');
            $("#<%=TB_ShiftNo.ClientID %>").val(arr[0]);

            if (emparr.length > 1) {
                alert('输入的移交人（' + arr[1] + '）不正确，移交人只能选择一个，请重新选择！');
                return false;
            }
            $("#<%=TB_ShiftName.ClientID %>").val(arr[1]);
            return false;
        }
    </script>
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="style1">
        <caption>
            您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %>,
            退回给子线程处理人.</caption>
        <tr> 
            <td style="text-align: left">
                <%
                    //int fk_node = int.Parse(this.Request.QueryString["FK_Node"]);
                    //Int64.workid = Int64.Parse(this.Request.QueryString["WorkID"]);

                    /* 如果工作节点退回了*/
                    BP.WF.ReturnWorks rws = new BP.WF.ReturnWorks();
                    rws.Retrieve(BP.WF.ReturnWorkAttr.ReturnToNode, this.FK_Node,
                        BP.WF.ReturnWorkAttr.WorkID, this.WorkID,
                        BP.WF.ReturnWorkAttr.RDT);
                    string msgInfo = "";
                    if (rws.Count != 0)
                    {
                        foreach (BP.WF.ReturnWork rw in rws)
                        {
                            msgInfo += "<fieldset width='100%' ><legend>&nbsp; 来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='../../DataUser/ReturnLog/" + this.FK_Flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a></legend>";
                            msgInfo += rw.BeiZhuHtml;
                            msgInfo += "</fieldset>";
                        }
                        //   this.FlowMsg.AlertMsg_Info("流程退回提示", msgInfo);
                        //gwf.WFState = WFState.Runing;
                        //gwf.DirectUpdate();
                    }
                %>                <%=msgInfo%>
            </td>
        </tr>
        <tr>
            <td>
                移交其他人处理
            </td>
        </tr>
        <tr>
            <td>
                移交人
                <asp:TextBox runat="server" Columns="40" ID="TB_ShiftName"></asp:TextBox>
                <asp:HiddenField runat="server" ID="TB_ShiftNo" />
                <asp:Button Text="选择人" ID="Btn_ChooseEmp" runat="server" OnClientClick="return ChooseEmp();" />
            </td>
        </tr>
        <tr>
            <td>
                处理人信息
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="TB_Doc" TextMode="MultiLine" runat="server" Height="112px" Width="591px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="text-align: center">
                <asp:Button ID="Btn_Send" runat="server" Text="退回给子线程处理人" OnClick="Btn_Send_Click" />
                <asp:Button ID="Btn_Shfit" runat="server" Text="移交给其他人处理" OnClick="Btn_Shift_Click" />
                <asp:Button ID="Btn_Del" runat="server" Text="删除子线程" OnClick="Btn_Del_Click" />
                <asp:Button ID="Btn_UnSend" runat="server" Text="撤销分流点发送" 
                    OnClick="Btn_UnSend_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
