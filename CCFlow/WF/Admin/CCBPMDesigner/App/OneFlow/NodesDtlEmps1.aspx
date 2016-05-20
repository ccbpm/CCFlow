<%@ Page Title="工作时长分析" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="NodesDtlEmps1.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.App.OneFlow.NodesDtlEmps1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table id="main" style="width:100%;">
<tr>
<th rowspan="2" >序 </th>
<th rowspan="2" >工作人员</th>
<th colspan="4" >总体时长分析(单位:分钟数)</th>
<th colspan="4" >月份分析(单位:分钟数)</th>
</tr>

<tr>
<th>最长 </th>
<th>最短 </th>
<th>平均 </th>
<th>离散系数</th>

<th>本月平均 </th>
<th>上月平均 </th>
<th>节省 </th>
<th>效率提高</th>
</tr>
<%
    //节点ID.
    int nodeid = int.Parse(this.Request.QueryString["FK_Node"]);
    //int nodeid = 902;

    //获取当前月份
    string date=DateTime.Now.ToString("yyyy-MM");
    //获取上一个月份
    string dateL = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
    //获取序号
    int i = 0;
    
    //先获取每个节点处理人所用处理时间的最大值、最小值和平均值
    BP.DA.Paras ps = new BP.DA.Paras();
    string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
    ps = new BP.DA.Paras();
    ps.SQL = "SELECT C.Name,b.FK_Emp,B.maxnum,B.minnum,B.avgnum FROM "
                +"( SELECT a.FK_Emp ,MAX(a.UseMinutes) maxnum,MIN(a.UseMinutes) minnum,AVG(a.UseMinutes) avgnum FROM "
                + "(SELECT FK_Emp,UseMinutes FROM WF_CH WHERE FK_Node=" + dbstr + "FK_Node) a Group BY FK_Emp ) B  LEFT JOIN Port_Emp c "
                +"ON FK_Emp=C.No";
    ps.Add("FK_Node",nodeid);
    System.Data.DataTable dtNum = BP.DA.DBAccess.RunSQLReturnTable(ps);
           
    //生成表格
    foreach (System.Data.DataRow row in dtNum.Rows)
    {
        i++;
        //当前月平均值
        int avgN = 0;
        //上一个月平均值
        int avgL = 0;
        //节省时间
        int spare = 0;
        //效率提高
        string improve = "";
        //离散系数
        string CV = "0";
        
        //获取离散系数
        float fc = 0;
        ps = new BP.DA.Paras();
        ps.SQL = "SELECT UseMinutes FROM WF_CH WHERE FK_Node=" + dbstr + "FK_Node AND FK_Emp=" + dbstr + "FK_Emp";
        ps.Add("FK_Node",nodeid);
        ps.Add("FK_Emp", row["FK_Emp"].ToString());
        System.Data.DataTable dtCv = BP.DA.DBAccess.RunSQLReturnTable(ps);
        //先获取方差
        foreach (System.Data.DataRow item in dtCv.Rows)
        {
            fc = fc + (((float.Parse(item["UseMinutes"].ToString()) - float.Parse(row["avgnum"].ToString()))) * ((float.Parse(item["UseMinutes"].ToString()) - float.Parse(row["avgnum"].ToString()))));
        }
        //得到方差
        fc = (float)fc / dtCv.Rows.Count;
        fc = float.Parse(fc.ToString("0.00"));
        //获取标准差
        double SD = Math.Sin((double)fc);
        //得到离散系数
        float vcs = (float)SD / float.Parse(row["avgnum"].ToString());
        CV = vcs.ToString("0.00");
        
        //获取当前月平均值
        ps = new BP.DA.Paras();
        ps.SQL = "SELECT FK_Emp,avg(UseMinutes) avgN FROM WF_CH WHERE FK_Node=" + dbstr + "FK_Node AND FK_NY=" + dbstr + "FK_NY AND FK_Emp=" + dbstr + "FK_Emp"
            + " GROUP BY FK_Emp";
        ps.Add("FK_Node",nodeid);
        ps.Add("FK_NY",date);
        ps.Add("FK_Emp", row["FK_Emp"].ToString());
        System.Data.DataTable dtAvgN = BP.DA.DBAccess.RunSQLReturnTable(ps);
        //如果没有数据，默认为平均值为0
        if (dtAvgN.Rows.Count == 0)
            avgN = 0;
        else
            avgN =Int32.Parse( dtAvgN.Rows[0]["avgN"].ToString());

        //获取上一个月的平均值
        ps = new BP.DA.Paras();
        ps.SQL = "SELECT FK_Emp,avg(UseMinutes) avgL FROM WF_CH WHERE FK_Node=" + dbstr + "FK_Node AND FK_NY=" + dbstr + "FK_NY AND FK_Emp=" + dbstr + "FK_Emp"
            + " GROUP BY FK_Emp";
        ps.Add("FK_Node",nodeid);
        ps.Add("FK_NY",dateL);
        ps.Add("FK_Emp", row["FK_Emp"].ToString());
        System.Data.DataTable dtAvgL = BP.DA.DBAccess.RunSQLReturnTable(ps);
        //如果没有数据，默认为平均值为0
        if (dtAvgL.Rows.Count == 0)
            avgL = 0;
        else
            avgL =Int32.Parse( dtAvgL.Rows[0]["avgL"].ToString());

        //节约时间
        spare = avgL - avgN;
        //如果上个月平均值为空或这个月平均值为空，即最近两个月没有处理工作
        if (avgL == 0 || avgN==0)
        {
            improve = "0";
        }
        else
        {
            //获取提高率
            float num = (float)avgN / avgL;
            //默认获取小数点后两位
            float avg = float.Parse(num.ToString("0.00"));
            if (avg > 1)
            {
                //获得提高率
                avg = (avg - 1) * 100;
                improve = "-" + avg.ToString() + "%";
            }
            else
            {
                if (avg != 0)
                {
                    //获得提高率
                    avg = avg * 100;
                    improve = avg.ToString() + "%";
                }
                else
                    improve = "0";
            }
        }
        %>

        <tr>
        <td class="Idx" ><%=i %> </td>
        <td class="center"><%=row["Name"] %></td>
        <td class="center"><%=row["maxnum"] %> </td>
        <td class="center"><%=row["minnum"] %> </td>
        <td class="center"><%=row["avgnum"] %> </td>
        <td class="center"><%=CV %></td>

        <td class="center"><%=avgN %> </td>
        <td class="center"><%=avgL %> </td>
        <td class="center"><%=spare %> </td>
        <td class="center"><%=improve %></td>
        </tr>
           <%  }
    %>
</table>
<br />
<table style="width:100%;">
<tr>
<th style="text-align:center"> 平均用时统计 </th></tr>
</table>
<%
    BP.WF.Node nd = new BP.WF.Node(nodeid);
    StringBuilder sb = new StringBuilder();
    int toTop = 0;
    int maxAvg = 0;

    foreach (System.Data.DataRow it in dtNum.Rows)
    {
        //生成数据集
        sb.Append("<set label='" + it["Name"] + "' value='" + it["avgnum"] + "' />");
        maxAvg = Int32.Parse(it["avgnum"].ToString());

        if (maxAvg > toTop)
            toTop = maxAvg;
    }
    //设置折线图中的阴影样式
    sb.Append("<styles><definition><style name='LineShadow' type='shadow' color='333333' distance='6'/></definition>");
    sb.Append("<application><apply toObject='DATAPLOT' styles='LineShadow' /></application></styles>");
    sb.Append("</chart>");

    toTop += 100;//y轴中的最高值为获取的最大值+100.
    //设置折线图的整体样式
    sb.Insert(0, "<chart baseFontSize='14'  subcaption='" + "节点:[" + nd.NodeID + "]" + nd.Name +
                        "-" + "平均工作时长统计" + "' formatNumberScale='0' bgColor='#809FFE, FFFFFF' bgAlpha='100' baseFontColor='#333333' divLineAlpha='20'" +
                        " canvasBgAlpha='0' canvasBorderColor='#809FFE' divLineColor='#809FFE' baseFontSize='12' divLineAlpha='100'" +
                        " numVDivlines='10' vDivLineisDashed='1' showAlternateVGridColor='1' lineColor='BBDA00'" +
                        " anchorRadius='4' anchorBgColor='BBDA00' anchorBorderColor='FFFFFF' toolTipBgColor='#BFDFFE' " +
                        " toolTipBorderColor='#BFDFFE' " +
                        " alternateHGridAlpha='5' anchorBorderThickness='2' showValues='0'  yAxisMaxValue ='" + toTop + "'>");
    //将数据集存储在一个隐藏控件中，方便JS读取值 
    this.hd.Value = "{rowsCount:\"" + dtNum.Rows.Count + "\",chartData:\"" + sb.ToString() + "\"}";
     %>
<asp:HiddenField ID="hd" runat="server" />
<div id="fcDiv" style="width:100%;">
 </div>
   <script type="text/javascript">
       $(function () {
           var chartData = $("#<%=hd.ClientID%>").val();//读取存储于隐藏控件中的数据集
           var data = eval('(' + chartData + ')');
           var w = $("#main").css("width"); //设置折线图的宽度
           var chart;//调用折线图显示器
           var chart = new FusionCharts("../../../../Comm/Charts/swf/Line.swf", "CharZ", w, '350', '0', '0');
           chart.setDataXML(data.chartData);
           chart.render("fcDiv");//在DIV中显示折线图
       });
    </script>
</asp:Content>
