<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Face_EmpWorks"
    Title="待办工作" CodeBehind="EmpWorks.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">    
        a
        {
            color:#0066CC;
            text-decoration:none;
        }
        a:hover
        {
            color:#0084C5;
            text-decoration:underline;
        }
    </style>
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
    
<script  language="javascript" type="text/javascript" >
    function ExitAuth(fk_emp) {
        var url = 'Do.aspx?DoType=ExitAuth&FK_Emp=' + fk_emp;
        WinShowModalDialog(url, '');
        window.location.href = 'EmpWorks.aspx';
    }
    function NoSubmit(fk_emp) {
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

        string HungUp = this.Request.QueryString["IsHungUp"];
        bool IsHungUp = true;
        if (string.IsNullOrEmpty(HungUp))
            IsHungUp = false;

        string GroupBy = this.Request.QueryString["GroupBy"];

        if (string.IsNullOrEmpty(GroupBy))
        {
            if (this.DoType == "CC")
                GroupBy = "Rec";
            else
                GroupBy = "FlowName";
        }

        System.Data.DataTable dt = null;
        string timeKey;

        StringBuilder sBuilder = new StringBuilder();

        string empWorksHtml = "";

        timeKey = DateTime.Now.ToString("yyyyMMddHHmmss");
        FK_Flow = this.Request.QueryString["FK_Flow"];
        if (IsHungUp)
            dt = BP.WF.Dev2Interface.DB_GenerHungUpList();
        else
        {
            if (string.IsNullOrEmpty(FK_Flow))
                dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();
            else
                dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(BP.Web.WebUser.No, FK_Flow);
        }

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
            sBuilder.Append("<Caption ><div class='CaptionMsg' >待办</div></Caption>");
            

        string extStr = "";
        if (IsHungUp)
            extStr = "&IsHungUp=1";

        sBuilder.Append("<tr class='centerTitle'>");
        sBuilder.Append("<th >ID</th>");
        sBuilder.Append("<th class='Title' width=40% nowrap=true >标题</th>");

        if (GroupBy != "FlowName")
        {
            sBuilder.Append("<th >" + "<a href='EmpWorks.aspx?GroupBy=FlowName" + extStr + "&T=" + timeKey + "' >流程</a>" + "</th>");
        }

        if (GroupBy != "NodeName")
        {
            sBuilder.Append("<th >" + "<a href='EmpWorks.aspx?GroupBy=NodeName" + extStr + "&T=" + timeKey + "' >节点</a>" + "</th>");
        }

        if (GroupBy != "StarterName")
        {
            sBuilder.Append("<th >" + "<a href='EmpWorks.aspx?GroupBy=StarterName" + extStr + "&T=" + timeKey + "' >发起人</a>" + "</th>");
        }

        sBuilder.Append("<th >" + "<a href='EmpWorks.aspx?GroupBy=PRI" + extStr + "&T=" + timeKey + "' >优先级</a>" + "</th>");

        sBuilder.Append("<th >发起日期</th>");
        sBuilder.Append("<th >接受日期</th>");
        sBuilder.Append("<th >应完成日期</th>");
        sBuilder.Append("<th >状态</th>");
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
                string sdt = dr["SDT"] as string;
                string paras = dr["AtPara"] as string;

                paras = paras.Replace("'", "\\'");

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
                int isRead = int.Parse(dr["IsRead"].ToString());

                sBuilder.Append("<td class='Idx' nowrap>" + i + "</td>");
                if (BP.WF.Glo.IsWinOpenEmpWorks)
                {
                    if (isRead == 0)
                    {
                        sBuilder.Append("<td onclick=\"SetImg('" + appPath + "','I" + gIdx + "_" + i + "')\"" + " >" + "<a href=\"javascript:WinOpenIt('MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "&IsRead=0&T=" + timeKey + "&Paras=" + paras + "');\" ><img class=Icon align='middle'  src='Img/Mail_UnRead.png' id='I" + gIdx + "_" + i + "' />" + dr["Title"].ToString() + "</a>" + "</td>");
                    }
                    else
                    {
                        sBuilder.Append("<td  nowrap >" + "<a href=\"javascript:WinOpenIt('MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "&Paras=" + paras + "&T=" + timeKey + "');\"  ><img src='Img/Mail_Read.png' id='I" + gIdx + "_" + i + "' class=Icon align='middle'  />" + dr["Title"].ToString() + "</a>" + "</td>");
                    }
                }
                else
                {
                    if (isRead == 0)
                    {
                        sBuilder.Append("<td onclick=\"SetImg('" + appPath + "','I" + gIdx + "_" + i + "')\" " + " >" + "<a href=\"MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "&IsRead=0&Paras=" + paras + "&T=" + timeKey + "\" ><img class=Icon src='Img/Mail_UnRead.png' align='middle'  id='I" + gIdx + "_" + i + "' />" + dr["Title"].ToString() + "</a>" + "</td>");
                    }
                    else
                    {
                        sBuilder.Append("<td  nowrap >" + "<a href=\"MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "&Paras=" + paras + "&T=" + timeKey + "\" ><img class=Icon src='Img/Mail_Read.png' align='middle'  id='I" + gIdx + "_" + i + "' />" + dr["Title"].ToString() + "</a>" + "</td>");
                    }
                }

                if (GroupBy != "FlowName")
                {
                    sBuilder.Append("<td  nowrap >" + dr["FlowName"].ToString() + "</td>");
                }
                if (GroupBy != "NodeName")
                {
                    sBuilder.Append("<td  nowrap >" + dr["NodeName"].ToString() + "</td>");
                }
                if (GroupBy != "StarterName")
                {
                    sBuilder.Append("<td  nowrap >" + BP.WF.Glo.GenerUserImgSmallerHtml(dr["Starter"].ToString(), dr["StarterName"].ToString()) + "</td>");
                }
                sBuilder.Append("<td  nowrap >" + "<img class=Icon src='Img/PRI/" + dr["PRI"].ToString() + ".png' class=Icon />" + "</td>");


                sBuilder.Append("<td  nowrap class='TBDate' >" + BP.DA.DataType.ParseSysDate2DateTimeFriendly(dr["RDT"].ToString()) + "</td>");
                sBuilder.Append("<td  nowrap class='TBDate' >" + BP.DA.DataType.ParseSysDate2DateTimeFriendly(dr["ADT"].ToString()) + "</td>");
                sBuilder.Append("<td  nowrap class='TBDate' >" + dr["SDT"].ToString().Substring(5) + "</td>");

                DateTime mysdt = BP.DA.DataType.ParseSysDate2DateTime(sdt);
                if (cdt >= mysdt)
                {
                    if (cdt.ToString("yyyy-MM-dd") == mysdt.ToString("yyyy-MM-dd"))
                    {
                        sBuilder.Append("<td align=center nowrap >" + "<img src='/WF/Img/TolistSta/0.png' class='Icon'/><font color=green>正常</font>" + "</td>");
                    }
                    else
                    {
                        sBuilder.Append("<td align=center nowrap >" + "<img src='/WF/Img/TolistSta/2.png' class='Icon'/><font color=red>逾期</font>" + "</td>");
                    }
                }
                else
                {
                    sBuilder.Append("<td align=center nowrap >" + "<img src='/WF/Img/TolistSta/0.png'class='Icon'/>&nbsp;<font color=green>正常</font>" + "</td>");
                }

                sBuilder.Append("<td width='200'><div style='width:200px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap;' title='" + dr["FlowNote"] + "'>" + dr["FlowNote"] + "</div></td>");
                sBuilder.Append("</tr>");
            }
        }
        sBuilder.Append("</table>");

        if (BP.Web.WebUser.IsAuthorize == true)
        {
            sBuilder.Append("<br><br><br><div style='float:right;' ><a href=\"javascript:ExitAuth('" + BP.Web.WebUser.No + "')\" >退出授权登录模式返回(" + BP.Web.WebUser.Auth + ")的待办</a></div>");
        }
        else
        {
            sBuilder.Append("<br><br><br><div style='float:right;' ><a href=\"AutoTodolist.aspx\" >查看授权人的待办工作</a></div>");
        }
           

        empWorksHtml = sBuilder.ToString();
    %>
    <%=empWorksHtml %>

</asp:Content>
