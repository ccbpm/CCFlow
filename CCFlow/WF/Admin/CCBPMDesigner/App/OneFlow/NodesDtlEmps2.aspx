<%@ Page Title="工作考核分析" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="NodesDtlEmps2.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.App.OneFlow.NodesDtlEmps2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%
    int nodeid = int.Parse(this.Request.QueryString["FK_Node"]);
    //int nodeid = 902;
   
  %>
<table id="main_table" style="width:100%;">
<tr>
<th rowspan="2" >序 </th>
<th rowspan="2" >工作人员</th>
<th colspan="5" >考核状态分布(单位:个)</th>
<th colspan="3" >本月数据</th>
<th colspan="3" >上月数据</th>

</tr>

<tr>
<th>及时完成 </th>
<th>按期完成 </th>
<th>逾期完成 </th>
<th>超期完成</th>
<th>按期办结率</th>


<th>按期完成 </th>
<th>逾期完成 </th>
<th>按期完成率 </th>

<th>按期完成 </th>
<th>逾期完成 </th>
<th>按期完成率 </th>
</tr>
<%
    //获取当前月份
    string date = DateTime.Now.ToString("yyyy-MM");
    //获取上一个月份
    string dateL = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
    //获取序号
    int i = 0;
    
    //首先获取当前节点所有的处理人
    string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
    BP.DA.Paras ps = new BP.DA.Paras();
    ps = new BP.DA.Paras();
    ps.SQL = "SELECT A.FK_Emp,B.NAME FROM (SELECT FK_Emp FROM WF_CH WHERE  FK_Node="+dbstr+"FK_Node Group By FK_Emp) A "
            +" Left Join Port_Emp B On A.FK_Emp=B.No ";
    ps.Add("FK_Node",nodeid);
    System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
    dt.Columns.Add("Aqjbl");
    
    //生成表格
    foreach (System.Data.DataRow row in dt.Rows)
    {
        i++;
        //获取及时完成的数量
        ps = new BP.DA.Paras();
        ps.SQL = "SELECT COUNT("+BP.WF.Data.CHAttr.CHSta+") FROM WF_CH WHERE CHSta="+dbstr+"CHSta AND FK_Node="+dbstr+"FK_Node"
            + " AND FK_Emp=" + dbstr + "FK_Emp";
        ps.Add(BP.WF.Data.CHAttr.CHSta,(int)BP.WF.Data.CHSta.JiShi);
        ps.Add("FK_Node",nodeid);
        ps.Add("FK_Emp", row["FK_Emp"]);
        int JiShi = BP.DA.DBAccess.RunSQLReturnValInt(ps,0);
        //获取按期完成的数量
        ps = new BP.DA.Paras();
        ps.SQL = "SELECT COUNT(" + BP.WF.Data.CHAttr.CHSta + ") FROM WF_CH WHERE CHSta=" + dbstr + "CHSta AND FK_Node=" + dbstr + "FK_Node"
            + " AND FK_Emp="+dbstr+"FK_Emp";
        ps.Add(BP.WF.Data.CHAttr.CHSta, (int)BP.WF.Data.CHSta.AnQi);
        ps.Add("FK_Node", nodeid);
        ps.Add("FK_Emp", row["FK_Emp"]);
        int AnQi = BP.DA.DBAccess.RunSQLReturnValInt(ps,0);

        //获取逾期完成的数量
        ps = new BP.DA.Paras();
        ps.SQL = "SELECT COUNT(" + BP.WF.Data.CHAttr.CHSta + ") FROM WF_CH WHERE CHSta=" + dbstr + "CHSta AND FK_Node=" + dbstr + "FK_Node"
            +" AND FK_Emp="+dbstr+"FK_Emp";
        ps.Add(BP.WF.Data.CHAttr.CHSta, (int)BP.WF.Data.CHSta.YuQi);
        ps.Add("FK_Node", nodeid);
        ps.Add("FK_Emp", row["FK_Emp"]);
        int YuQi = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

        //获取超期完成的数量
        ps = new BP.DA.Paras();
        ps.SQL = "SELECT COUNT(" + BP.WF.Data.CHAttr.CHSta + ") FROM WF_CH WHERE CHSta=" + dbstr + "CHSta AND FK_Node=" + dbstr + "FK_Node"
            + " AND FK_Emp=" + dbstr + "FK_Emp";
        ps.Add(BP.WF.Data.CHAttr.CHSta, (int)BP.WF.Data.CHSta.ChaoQi);
        ps.Add("FK_Node", nodeid);
        ps.Add("FK_Emp", row["FK_Emp"]);
        int ChaoQi = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

        //获取按期办结率
        string aqjbl = "";
        //如果及时完成数量与按期完成数量都为0，那么按期办结率为0
        if (JiShi == 0 && AnQi == 0)
            aqjbl = "0.00";
        else
        {
            float bjl = (float)(JiShi + AnQi) / (JiShi + AnQi + YuQi + ChaoQi);
            aqjbl = (bjl*100).ToString("0.00");
        }

        row["Aqjbl"] = aqjbl;
        
        //获取按期完成的数量
        ps = new BP.DA.Paras();
        ps.SQL = "SELECT COUNT(" + BP.WF.Data.CHAttr.CHSta + ") FROM WF_CH WHERE CHSta<" + dbstr + "CHSta AND FK_Node=" + dbstr + "FK_Node"
            + " AND FK_Emp=" + dbstr + "FK_Emp AND FK_NY="+dbstr+"FK_NY";
        ps.Add(BP.WF.Data.CHAttr.CHSta, (int)BP.WF.Data.CHSta.YuQi);
        ps.Add("FK_Node", nodeid);
        ps.Add("FK_Emp", row["FK_Emp"]);
        ps.Add("FK_NY",date);
        int MNumAqN = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

        //获取逾期完成的数量
        ps = new BP.DA.Paras();
        ps.SQL = "SELECT COUNT(" + BP.WF.Data.CHAttr.CHSta + ") FROM WF_CH WHERE CHSta>" + dbstr + "CHSta AND FK_Node=" + dbstr + "FK_Node"
            + " AND FK_Emp=" + dbstr + "FK_Emp AND FK_NY="+ dbstr +"FK_NY";
        ps.Add(BP.WF.Data.CHAttr.CHSta, (int)BP.WF.Data.CHSta.AnQi);
        ps.Add("FK_Node", nodeid);
        ps.Add("FK_Emp", row["FK_Emp"]);
        ps.Add("FK_NY", date);
        int MNumYqN = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

        //获取按期完成率
        string aqwcl="";
        //如果按期完成数量为0，则按期完成率为0
        if (MNumAqN == 0)
            aqwcl = "0.00%";
        //如果预期完成数量为0，按期数量不为0，完成率为100
        else if (MNumAqN != 0 && MNumYqN == 0)
            aqwcl = "100%";
        //如果数量都不为0，开始计算完成率
        else if (MNumAqN != 0 && MNumYqN != 0)
        {
            float aqwc = (float)MNumAqN / (MNumAqN + MNumYqN);
            aqwcl = (aqwc*100).ToString("0.00") + "%";
        }

        //获取上月按期完成的数量
        ps = new BP.DA.Paras();
        ps.SQL = "SELECT COUNT(" + BP.WF.Data.CHAttr.CHSta + ") FROM WF_CH WHERE CHSta<" + dbstr + "CHSta AND FK_Node=" + dbstr + "FK_Node"
            + " AND FK_Emp=" + dbstr + "FK_Emp AND FK_NY=" + dbstr + "FK_NY";
        ps.Add(BP.WF.Data.CHAttr.CHSta, (int)BP.WF.Data.CHSta.YuQi);
        ps.Add("FK_Node", nodeid);
        ps.Add("FK_Emp", row["FK_Emp"]);
        ps.Add("FK_NY", dateL);
        int MNumAqL = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

        //获取上月逾期完成的数量
        ps = new BP.DA.Paras();
        ps.SQL = "SELECT COUNT(" + BP.WF.Data.CHAttr.CHSta + ") FROM WF_CH WHERE CHSta>" + dbstr + "CHSta AND FK_Node=" + dbstr + "FK_Node"
            + " AND FK_Emp=" + dbstr + "FK_Emp AND FK_NY=" + dbstr + "FK_NY";
        ps.Add(BP.WF.Data.CHAttr.CHSta, (int)BP.WF.Data.CHSta.AnQi);
        ps.Add("FK_Node", nodeid);
        ps.Add("FK_Emp", row["FK_Emp"]);
        ps.Add("FK_NY", dateL);
        int MNumYqL = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

        //获取上月按期完成率
        string Laqwcl = "";
        //如果上月按期完成的数量为0，则完成率为0
        if (MNumAqL == 0)
            Laqwcl = "0.00%";
        //如果上月按期完成数量不为0，预期数量为0，则完成率为100
        else if (MNumAqL != 0 && MNumYqL == 0)
            Laqwcl = "100%";
        else if (MNumAqL != 0 && MNumYqL != 0)
        {
            float Laqwc = (float)MNumAqL / (MNumAqL + MNumYqL);
            Laqwcl = (Laqwc*100).ToString("0.00") + "%";
        }
        %>
        <tr>
        <td class="Idx"><%=i %> </td>
        <td class="center"><%=row["Name"] %></td>
        <td class="center"><%=JiShi %> </td>
        <td class="center"><%=AnQi %> </td>
        <td class="center"><%=YuQi %> </td>
        <td class="center"><%=ChaoQi %></td>
        <td class="center"><%=aqjbl %>%</td>
        <td class="center"><%=MNumAqN %> </td>
        <td class="center"><%=MNumYqN %> </td>
        <td class="center"><%=aqwcl %> </td>
        <td class="center"><%=MNumAqL %> </td>
        <td class="center"><%=MNumYqL %> </td>
        <td class="center"><%=Laqwcl %> </td>
        </tr>
        <%
    }
     %>

</table>
<br />
<table style="width:100%;">
<tr>
<th style="text-align:center"> 按期办结率统计 </th></tr>
</table>
<%
    //获取节点信息
    BP.WF.Node nd = new BP.WF.Node(nodeid);
    StringBuilder sb = new StringBuilder();

    foreach (System.Data.DataRow it in dt.Rows)
    {
        //生成数据集
        sb.Append("<set label='" + it["Name"] + "' value='" + it["Aqjbl"] + "' />");
    }
    //添加折线图中的阴影样式
    sb.Append("<styles><definition><style name='LineShadow' type='shadow' color='333333' distance='6'/></definition>");
    sb.Append("<application><apply toObject='DATAPLOT' styles='LineShadow' /></application></styles>");
    sb.Append("</chart>");
    //添加折线图的整体样式  
    sb.Insert(0, "<chart   caption='" + "节点:[" + nd.NodeID + "]" + nd.Name + "-" + "按期办结率统计" + "' bgColor='#809FFE, FFFFFF' bgAlpha='100' baseFontColor='#333333'" +
                        " canvasBgAlpha='0' canvasBorderColor='#809FFE' divLineColor='#809FFE' baseFontSize='12' divLineAlpha='100'" +
                        " numVDivlines='10' vDivLineisDashed='1' showAlternateVGridColor='1' lineColor='BBDA00'" +
                        " anchorRadius='4' anchorBgColor='BBDA00' anchorBorderColor='FFFFFF' " +
                        " anchorBorderThickness='2' showValues='0' numberSuffix='%' toolTipBgColor='#BFDFFE'" +
                        " toolTipBorderColor='#BFDFFE' alternateHGridAlpha='5' yAxisMaxValue='100' >");
    //将数据集存储在一个隐藏控件中，方便JS读取值 
    this.hfd.Value = "{rowsCount:\"" + dt.Rows.Count + "\",chartData:\"" + sb.ToString() + "\"}";
     %>
<asp:HiddenField ID="hfd" runat="server" />
<div id="fc" style="width:100%;">
 </div>
   <script type="text/javascript">
       $(function () {
           var chartData = $("#<%=hfd.ClientID%>").val();//获取存储在隐藏控件中的数据集
           var data = eval('(' + chartData + ')');
           var w = $("#main_table").css("width"); //设置折线图的宽度
           var chart;//调用折线图显示器
           var chart = new FusionCharts("../../../../Comm/Charts/swf/Line.swf", "CharZ", w, '350', '0', '0');
           chart.setDataXML(data.chartData);
           chart.render("fc");//在DIV中显示折线图
       });
    </script>
</asp:Content>
