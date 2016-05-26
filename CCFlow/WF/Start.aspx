<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true"
    CodeBehind="Start.aspx.cs" Inherits="CCFlow.WF.JZFlows" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function StartListUrl(appPath, url, fk_flow, pageid) {
            var v = window.showModalDialog(url, 'sd', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
            //alert(v);
            if (v == null || v == "")
                return;

            window.location.href = appPath + '../MyFlow.aspx?FK_Flow=' + fk_flow + v;
        }
        function WinOpenIt(url) {
            var newWindow = window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
            newWindow.focus();
            return;
        }

        function WinOpen(url, winName) {
            var newWindow = window.open(url, winName, 'height=800,width=1030,top=' + (window.screen.availHeight - 800) / 2 + ',left=' + (window.screen.availWidth - 1030) / 2 + ',scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
            newWindow.focus();
            return;
        }
    </script>
    <style type="text/css">
        table
        {
            margin: 0px;
            padding: 0px;
        }
        td
        {
            font-family: Microsoft YaHei;
        }
        ul
        {
            margin-left: -10px;
            margin-top: 2px;
            font: 12px 宋体, Arial, Verdana;
        }
        span
        {
            font-size: 16px;
            font-family: Vijaya;
            margin-right: 5px;
        }
        .noHaveIt
        {
        }
        .haveIt
        {
            color: Blue;
            font-weight: bolder;
        }
        li
        {
            height: 20px;
            line-height:20px;
            margin-top: 3px;
        }
        .op
        {
            float: right;
        }
        .left
        {
            float: left;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
        BP.WF.Flows fls = new BP.WF.Flows();
        fls.RetrieveAll();

        BP.WF.Template.FlowSorts ens = new BP.WF.Template.FlowSorts();
        ens.RetrieveAll();

        System.Data.DataTable dt = BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(BP.Web.WebUser.No);

        int cols = 3; //定义显示列数 从0开始。
        decimal widthCell = 100 / cols;

        string startHtml = "";
        StringBuilder sBuilder = new StringBuilder();

        sBuilder.Append("<table width=100% border=0>");
        sBuilder.Append("<Caption ><div class='CaptionMsg' >发起流程</div></Caption>");//面板标题

        int idx = -1;
        bool is1 = false;

        string timeKey = "s" + this.Session.SessionID + DateTime.Now.ToString("yyMMddHHmmss");
        foreach (BP.WF.Template.FlowSort en in ens)
        {
            if (en.ParentNo == "0"
                || en.ParentNo == ""
                || en.No == "")
                continue;

            //如果该目录下流程数量为空就返回.
            if (fls.GetCountByKey(BP.WF.Template.FlowAttr.FK_FlowSort, en.No) == 0)
                continue;

            idx++;
            if (idx == 0)
            {
                if (is1)
                    sBuilder.Append("<tr bgcolor=AliceBlue>");
                else
                    sBuilder.Append("<tr bgcolor=white>");

                is1 = !is1;
            }
            sBuilder.Append("<td width='" + widthCell + "%' border=0 valign=top nowrap >");
            //输出类别.
            sBuilder.Append(en.Name);
            sBuilder.Append("<ol>");

            #region 输出流程。
            int index = 1;
            foreach (BP.WF.Flow fl in fls)
            {
                if (fl.FlowAppType == BP.WF.FlowAppType.DocFlow)
                    continue;

                if (fl.FK_FlowSort != en.No)
                    continue;
                if (fl.IsCanStart == false)
                    continue;

                bool isHaveIt = false;
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    if (dr["No"].ToString() != fl.No)
                        continue;
                    isHaveIt = true;
                    break;
                }

                string extUrl = "";
                if (fl.IsBatchStart)
                    extUrl = "<div class='op'><a href='/WF/BatchStart.aspx?FK_Flow=" + fl.No + "' >批量发起</a>|<a href='/WF/Rpt/Search.aspx?RptNo=ND" + int.Parse(fl.No) + "MyRpt&FK_Flow=" + fl.No + "'>查询</a>|<a href=\"javascript:WinOpen('/WF/WorkOpt/OneWork/ChartTrack.aspx?FK_Flow=" + fl.No + "&DoType=Chart&T=" + timeKey + "','sd');\"  >图</a></div>";
                else
                    extUrl = "<div class='op'><a  href='/WF/Rpt/Search.aspx?RptNo=ND" + int.Parse(fl.No) + "MyRpt&FK_Flow=" + fl.No + "'>查询</a>|<a href=\"javascript:WinOpen('/WF/Admin/CCBPMDesigner/truck/Truck.htm?FK_Flow=" + fl.No + "&WorkID=null&FID=null&DoType=Chart&T=" + timeKey + "','sd');\"  >流程图</a></div>";

                if (isHaveIt)
                {
                    if (BP.WF.Glo.IsWinOpenStartWork == 1)
                    {
                        sBuilder.Append("<li><b class='left'><a href=\"javascript:WinOpenIt('MyFlow.aspx?FK_Flow=" + fl.No + "&FK_Node=" + int.Parse(fl.No) + "01&T=" + timeKey + "');\" >" + fl.Name + "</a></b>" + extUrl + "</li>");
                    }
                    else if (BP.WF.Glo.IsWinOpenStartWork == 2)
                    {
                        sBuilder.Append("<li><b class='left'><a href=\"javascript:WinOpenIt('/WF/OneFlow/MyFlow.aspx?FK_Flow=" + fl.No + "&FK_Node=" + int.Parse(fl.No) + "01&T=" + timeKey + "');\" >" + fl.Name + "</a></b>" + extUrl + "</li>");
                    }
                    else
                    {
                        sBuilder.Append("<li><b class='left'><a href='MyFlow.aspx?FK_Flow=" + fl.No + "&FK_Node=ND" + int.Parse(fl.No) + "01' >" + fl.Name + "</a></b>" + extUrl + "</li>");
                    }
                }
                else
                {
                    sBuilder.Append("<li>" + fl.Name + "</li>");
                }
                index += 1;
            }
            #endregion 输出流程。

            sBuilder.Append("</ol>");
            sBuilder.Append("</td>");
            if (idx == cols - 1)
            {
                idx = -1;
                sBuilder.Append("</tr>");
            }
        }

        while (idx != -1)
        {
            idx++;
            if (idx == cols - 1)
            {
                idx = -1;
                sBuilder.Append("</td>");
                sBuilder.Append("</tr>");
            }
            else
            {
                sBuilder.Append("</td>");
            }
        }
        sBuilder.Append("</table>");
        startHtml = sBuilder.ToString();
    %>
    <%=startHtml%>
</asp:Content>
