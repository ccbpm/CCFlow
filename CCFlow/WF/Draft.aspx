<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Draft"
    Title="草稿" CodeBehind="Draft.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
        var NS4 = (document.layers);
        var IE4 = (document.all);
        var win = window;
        var n = 0;
        function findInPage(str) {

            alert(document.getElementById('string1'));
            str = document.getElementById('string1').value;
            //    alert(str);
            var txt, i, found;
            if (str == "")
                return false;
            if (NS4) {
                if (!win.find(str))
                    while (win.find(str, false, true))
                        n++;
                else
                    n++;
                if (n == 0)
                    alert("对不起！没有你要找的内容。");
            }
            if (IE4) {
                txt = win.document.body.createTextRange();
                for (i = 0; i <= n && (found = txt.findText(str)) != false; i++) {
                    txt.moveStart("character", 1);
                    txt.moveEnd("textedit");
                }
                if (found) {
                    txt.moveStart("character", -1);
                    txt.findText(str);
                    txt.select();
                    txt.scrollIntoView();
                    n++;
                }
                else {
                    if (n > 0) {
                        n = 0;
                        findInPage(str);
                    }
                    else
                        alert("对不起！没有你要找的内容。");
                }
            }
            return false;
        }

        function SetImg(appPath, id) {
            document.getElementById(id).src = appPath + 'WF/Img/Mail_Read.png';
        }

        function GroupBarClick(appPath, rowIdx) {
            var alt = document.getElementById('Img' + rowIdx).alert;
            var sta = 'block';
            if (alt == 'Max') {
                sta = 'block';
                alt = 'Min';
            } else {
                sta = 'none';
                alt = 'Max';
            }
            document.getElementById('Img' + rowIdx).src = appPath + 'WF/Img/' + alt + '.gif';
            document.getElementById('Img' + rowIdx).alert = alt;
            var i = 0
            for (i = 0; i <= 5000; i++) {
                if (document.getElementById(rowIdx + '_' + i) == null)
                    continue;

                if (sta == 'block') {
                    document.getElementById(rowIdx + '_' + i).style.display = '';
                } else {
                    document.getElementById(rowIdx + '_' + i).style.display = sta;
                }

            }
        }
        function WinOpenIt(url) {
            //窗口最大化e
            var scrWidth = screen.availWidth;
            var scrHeight = screen.availHeight;
            var self = window.open(url, '_blank', "resizable=1,scrollbars=yes");
            self.moveTo(0, 0);
            self.resizeTo(scrWidth, scrHeight);
            self.focus();
            // var newWindow = window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
            // newWindow.focus();
            return;
        }
    </script>
    <style type="text/css">
        table
        {
            font: 12px 宋体, Arial, Verdana;
        }
        .TRSum
        {
            font: 12px 宋体, Arial, Verdana;
        }
        .centerTitle th
        {
            text-align: center;
        }
        .Idx
        {
            font-size: 16px;
            font-family: Vijaya;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%
        string FK_Flow = this.Request.QueryString["FK_Flow"];
        string GroupBy = this.Request.QueryString["GroupBy"];
                GroupBy = "FlowName";

        System.Data.DataTable dt = null;
        string timeKey;

        StringBuilder sBuilder = new StringBuilder();

        string empWorksHtml = "";

        timeKey = DateTime.Now.ToString("yyyyMMddHHmmss");
        FK_Flow = this.Request.QueryString["FK_Flow"];
            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable();

        string appPath = BP.WF.Glo.CCFlowAppPath;//this.Request.ApplicationPath;
        string groupVals = "";
        foreach (System.Data.DataRow dr in dt.Rows)
        {
            if (groupVals.Contains("@" + dr[GroupBy].ToString() + ","))
                continue;
            groupVals += "@" + dr[GroupBy].ToString() + ",";
        }

        int colspan = 10;

        sBuilder.Append("<table width='100%'  cellspacing='0' cellpadding='0' align=left>");
        sBuilder.Append("<Caption ><div class='CaptionMsg' >草稿</div></Caption>");

        string extStr = "";

        sBuilder.Append("<tr class='centerTitle'>");
        sBuilder.Append("<th >ID</th>");
        sBuilder.Append("<th class='Title' width=40% nowrap=true >标题</th>");
        sBuilder.Append("<th  >流程</th>");
        sBuilder.Append("<th >日期</th>");
        sBuilder.Append("<th >备注</th>");
        sBuilder.Append("</tr>");

        int i = 0;
        bool is1 = false;
        DateTime cdt = DateTime.Now;
        string[] gVals = groupVals.Split('@');
        int gIdx = 0;
        foreach (string g in gVals)
        {
            if (string.IsNullOrEmpty(g))
                continue;
            gIdx++;
            sBuilder.Append("<tr>");
            if (GroupBy == "Rec")
            {
                sBuilder.Append("<td colspan=" + colspan + " class=TRSum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" " + " >" + "<div style='text-align:left; float:left' ><img src='Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g.Replace(",", "") + "</b>" + "</td>");
            }
            else
            {
                sBuilder.Append("<td colspan=" + colspan + " class=TRSum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" " + " >" + "<div style='text-align:left; float:left' ><img src='Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g.Replace(",", "") + "</b>" + "</td>");
            }

            sBuilder.Append("</tr>");

            foreach (System.Data.DataRow dr in dt.Rows)
            {
                if (dr[GroupBy].ToString() + "," != g)
                    continue;

                if (is1)
                {
                    sBuilder.Append("<tr bgcolor=AliceBlue " + "ID='" + gIdx + "_" + i + "'" + " >");
                }
                else
                {
                    sBuilder.Append("<tr bgcolor=white " + "ID='" + gIdx + "_" + i + "'" + " class=TR>");
                }

                is1 = !is1;
                i++;

                sBuilder.Append("<td class='Idx' nowrap>" + i + "</td>");
                if (BP.WF.Glo.IsWinOpenEmpWorks)
                {
                    sBuilder.Append("<td onclick=\"SetImg('" + appPath + "','I" + gIdx + "_" + i + "')\"" + " >" + "<a href=\"javascript:WinOpenIt('MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FID=0&WorkID=" + dr["WorkID"] + "&FK_Node=" + int.Parse(dr["FK_Flow"].ToString()) + "01&IsRead=0&T=" + timeKey + "');\" ><img class=Icon align='middle'  src='Img/Mail_UnRead.png' id='I" + gIdx + "_" + i + "' />" + dr["Title"].ToString() + "</a>" + "</td>");
                }

                sBuilder.Append("<td  nowrap >" + dr["FlowName"].ToString() + "</td>");

                sBuilder.Append("<td  nowrap class='TBDate' >" + BP.DA.DataType.ParseSysDate2DateTimeFriendly(dr["RDT"].ToString()) + "</td>");

                sBuilder.Append("<td  nowrap >" + dr["FlowNote"].ToString() + "</td>");
                sBuilder.Append("</tr>");
            }
        }
        sBuilder.Append("</table>");
        empWorksHtml = sBuilder.ToString();
    %>
    <%=empWorksHtml %>
</asp:Content>
