<%@ Page Title="单个流程监控面板" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.App.OneFlow.Welcome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../../../Scripts/jquery/themes/default/easyui.css" rel="stylesheet"
        type="text/css" />
    <link href="../../../../Scripts/jquery/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../../../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script src="../../../../Scripts/jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <link href="../../../../Comm/Charts/css/style_3.css" rel="stylesheet" type="text/css" />
    <link href="../../../../Comm/Charts/css/prettify.css" rel="stylesheet" type="text/css" />
    <script src="../../../../Comm/Charts/js/prettify.js" type="text/javascript"></script>
    <script src="../../../../Comm/Charts/js/json2_3.js" type="text/javascript"></script>
    <script src="../../../../Comm/Charts/js/FusionCharts.js" type="text/javascript"></script>
    <script src="../../../../Comm/Charts/js/FusionChartsExportComponent.js" type="text/javascript"></script>
    <link href="../../../../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../../../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        window.onload = function () {
            //实例增长分析.
            var slStr1 = $("#<%=HiddenField1.ClientID%>").val();
            var myChart1 = new FusionCharts("../../../../Comm/Charts/swf/MSColumn3D.swf", "CharZ", '1340', '370', '0', '0');
            myChart1.setDataXML(slStr1);
            myChart1.render("slChart1");
            //平均耗时分析(天).
            var slStr2 = $("#<%=HiddenField2.ClientID%>").val();
            var myChart2 = new FusionCharts("../../../../Comm/Charts/swf/MSColumn3D.swf", "CharZ", '1340', '370', '0', '0');
            myChart2.setDataXML(slStr2);
            myChart2.render("slChart2");
            //
            var slStr3 = $("#<%=HiddenField3.ClientID%>").val();
            var myChart3 = new FusionCharts("../../../../Comm/Charts/swf/MSColumn3D.swf", "CharZ", '1340', '370', '0', '0');
            myChart3.setDataXML(slStr3);
            myChart3.render("slChart3");


          
        }
    </script>
    <%
        // edit by 柳辉.
        string flowNo = this.Request.QueryString["FK_Flow"];
        BP.WF.Flow fl = new BP.WF.Flow(flowNo);
    
    %>
    <table style="width: 100%">
        <caption><%=fl.Name %>  - 监控面板 </caption>
        <tr>
            <td valign="top">
                <fieldset>
                    <%
                        //获得统计数据.
                        string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                        BP.DA.Paras ps = new BP.DA.Paras();
                        ps.SQL = "SELECT COUNT(WorkID) as Num, WFState FROM WF_GenerWorkFlow WHERE FK_Flow=" + dbStr + "FK_Flow GROUP BY WFState";
                        ps.Add(BP.WF.GenerWorkFlowAttr.FK_Flow, flowNo);
                        System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
                        int allNum = 0; //所有数量.
                        int runing = 0; //运行的数量.
                        int returnNum = 0; //退回数量.
                        int coumputleNum = 0; //完成数量.
                        int delNum = 0; //删除.
                        int hunupNum = 0; //挂起.

                        foreach (System.Data.DataRow dr in dt.Rows)
                        {
                            int staInt = int.Parse(dr["WFState"].ToString());
                            BP.WF.WFState sta = (BP.WF.WFState)staInt;

                            int num = int.Parse(dr["Num"].ToString());
                            allNum += num; //所有的数量.
                            //退回数据.
                            if (sta == BP.WF.WFState.ReturnSta)
                                returnNum = num;
                            //删除.
                            if (sta == BP.WF.WFState.Delete)
                                delNum = num;
                            //挂起.
                            if (sta == BP.WF.WFState.HungUp)
                                hunupNum = num;
                        }
                        //其他状态数量.
                        int etcNum = allNum - runing - returnNum - coumputleNum - delNum;
                    %>
                    <legend>该流程实例数量统计 </legend>
                    <table style="width: 80%;">
                        <tr>
                            <td>流程总数 </td>
                            <td><%=allNum%> </td>
                            <td>个</td>
                        </tr>

                        <tr>
                            <td>正在运行数 </td>
                            <td><%=runing%> </td>
                            <td>个</td>
                        </tr>

                        <tr>
                            <td>完成数 </td>
                            <td><%=coumputleNum%> </td>
                            <td>个</td>
                        </tr>

                        <tr>
                            <td>退回中数 </td>
                            <td><%=returnNum %> </td>
                            <td>个</td>
                        </tr>

                        <tr>
                            <td>挂起 </td>
                            <td><%=hunupNum %> </td>
                            <td>个</td>
                        </tr>

                        <tr>
                            <td>其他运行数 </td>
                            <td><%=etcNum%> </td>
                            <td>个</td>
                        </tr>

                    </table>
                </fieldset>
            </td>


            <td valign="top">
                <fieldset>
                    <%
                        //获得统计数据.
                        ps = new BP.DA.Paras();
                        ps.SQL = "SELECT COUNT(WorkID) as Num, WFState FROM WF_GenerWorkFlow WHERE FK_Flow=" + dbStr + "FK_Flow AND FK_NY=" + dbStr + "FK_NY GROUP BY WFState ";
                        ps.Add(BP.WF.GenerWorkFlowAttr.FK_Flow, flowNo);
                        ps.Add(BP.WF.GenerWorkFlowAttr.FK_NY, BP.DA.DataType.CurrentYearMonth);

                        dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
                        foreach (System.Data.DataRow dr in dt.Rows)
                        {
                            int staInt = int.Parse(dr["WFState"].ToString());
                            BP.WF.WFState sta = (BP.WF.WFState)staInt;

                            int num = int.Parse(dr["Num"].ToString());
                            allNum += num; //所有的数量.
                            //退回数据.
                            if (sta == BP.WF.WFState.ReturnSta)
                                returnNum = num;
                            //删除.
                            if (sta == BP.WF.WFState.Delete)
                                delNum = num;
                        }
                        //其他状态数量.
                        etcNum = allNum - runing - returnNum - coumputleNum - delNum;
                    %>
                    <legend>本月实例数量统计 </legend>
                    <table style="width: 80%;">
                        <tr>
                            <td>流程总数 </td>
                            <td><%=allNum%> </td>
                            <td>个</td>
                        </tr>

                        <tr>
                            <td>正在运行数 </td>
                            <td><%=runing%> </td>
                            <td>个</td>
                        </tr>

                        <tr>
                            <td>完成数 </td>
                            <td><%=coumputleNum%> </td>
                            <td>个</td>
                        </tr>

                        <tr>
                            <td>退回中数 </td>
                            <td><%=returnNum %> </td>
                            <td>个</td>
                        </tr>

                        <tr>
                            <td>挂起 </td>
                            <td><%=hunupNum %> </td>
                            <td>个</td>
                        </tr>

                        <tr>
                            <td>其他运行数 </td>
                            <td><%=etcNum%> </td>
                            <td>个</td>
                        </tr>
                    </table>
                </fieldset>
            </td>


            <td valign="top">
                <fieldset>
                    <legend>考核总体分析</legend>
                    <%
                        int empNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Port_Emp ");
                        int deptNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Port_Dept ");
                        int stationNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Port_Station ");
                    %>
                    <table style="width: 100%;">
                        <tr>
                            <td>提前完成分钟数
                            </td>
                            <td>
                                <%=empNum%>
                            </td>
                            <td>分钟</td>
                        </tr>
                        <tr>
                            <td>逾期分钟数
                            </td>
                            <td>
                                <%=deptNum%>
                            </td>
                            <td>分钟</td>
                        </tr>
                        <tr>
                            <td>按时完成</td>
                            <td><%=stationNum%></td>
                            <td>个</td>
                        </tr>
                        <tr>
                            <td>超期完成</td>
                            <td><%=stationNum%></td>
                            <td>个</td>
                        </tr>
                        <tr>
                            <td>按期办结率</td>
                            <td><%=stationNum%></td>
                            <td>%</td>
                        </tr>
                        <tr>
                            <td>在运行的预期
                            </td>
                            <td>
                                <font color="red">
                                    <%=stationNum%>
                                </font>
                            </td>
                            <td>个
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>

        <tr>
            <td colspan="3">实例增长分析.
   <% 
       ps = new BP.DA.Paras();
       ps.SQL = "SELECT COUNT(WorkID) as Num, FK_NY,FlowName FROM WF_GenerWorkFlow WHERE FK_Flow=" + dbStr + "FK_Flow GROUP BY FK_NY,FlowName ";
       ps.Add(BP.WF.GenerWorkFlowAttr.FK_Flow, flowNo);
       dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
       // ps.Add(BP.WF.GenerWorkFlowAttr.FK_NY, BP.DA.DataType.CurrentYearMonth);
       StringBuilder stringbuilder1 = new StringBuilder();
       string title1 = string.Empty;
       DateTime nowtime1 = DateTime.Now;
       int setCount1 = 0;
       int maxValue1 = 0;
       List<string> listMouth1 = new List<string>();
       for (int i = -11; i <= 0; i++)
       {
           listMouth1.Add(nowtime1.AddMonths(i).ToString("yyyy-MM"));
       }

       stringbuilder1.Append("<categories>");
       foreach (string lm in listMouth1)
       {
           stringbuilder1.Append("<category label='" + lm + "' />");
       }
       stringbuilder1.Append("</categories>");
       foreach (System.Data.DataRow dr in dt.Rows)
       {

           stringbuilder1.Append("<dataset seriesName='" + dr["FlowName"] + "'>");
           foreach (string lm in listMouth1)
           {
               ps = new BP.DA.Paras();
               ps.SQL = "SELECT COUNT(WorkID) as Num, FK_NY FROM WF_GenerWorkFlow WHERE FK_Flow=" + dbStr + "FK_Flow AND FK_NY LIKE '%" + lm + "%' GROUP BY FK_NY ";
               ps.Add(BP.WF.GenerWorkFlowAttr.FK_Flow, flowNo);
               setCount1 = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

               if (setCount1 > maxValue1)
                   maxValue1 = setCount1;

               stringbuilder1.Append("<set value='" + setCount1 + "' />");
           }
           stringbuilder1.Append("</dataset>");
       }
       stringbuilder1.Append("</chart>");
       maxValue1 += 10;//加10  不置顶
       stringbuilder1.Insert(0, "<chart baseFontSize='14'  subcaption='" + "流程:[" + fl.No + "]" + fl.Name +
                          "-" + listMouth1[0] + "至" + listMouth1[11] + "统计" + "' formatNumberScale='0' divLineAlpha='20'" +
                          " divLineColor='CC3300' alternateHGridColor='CC3300' shadowAlpha='40'" +
                          " numvdivlines='9'  bgColor='FFFFFF,CC3300' bgAngle='270' bgAlpha='10,10'" +
                          " alternateHGridAlpha='5'   yAxisMaxValue ='" + maxValue1 + "'>");

       this.HiddenField1.Value = stringbuilder1.ToString();
   %>
                <asp:HiddenField ID="HiddenField1" runat="server" />

                <div class="chart_div_con" id="slChart1">
                </div>


            </td>
        </tr>



        <tr>
            <td colspan="3">平均耗时分析(天).
   <% 
       ps = new BP.DA.Paras();
       ps.SQL = "SELECT FK_NY, AVG(TSpan) AS TSpan FROM WF_GenerWorkFlow  WHERE FK_Flow=" + dbStr + "FK_Flow AND WFState=3 GROUP BY FK_NY";
       ps.Add(BP.WF.GenerWorkFlowAttr.FK_Flow, flowNo);
       dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
       // ps.Add(BP.WF.GenerWorkFlowAttr.FK_NY, BP.DA.DataType.CurrentYearMonth);
       StringBuilder stringbuilder2 = new StringBuilder();
       string title2 = string.Empty;
       DateTime nowtime2 = DateTime.Now;
       int setCount2 = 0;
       int maxValue2 = 0;

       string dt2 = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("dd");//本月第一天
       string dt1 = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("dd");//本月最后一天
       
       List<string> listMouth2 = new List<string>();
       for (int i = int.Parse(dt2); i <= int.Parse(dt1); i++)
       {
           //listMouth2.Add(nowtime1.AddMonths(i).ToString("yyyy-MM"));
           listMouth2.Add(DateTime.Now.AddDays(i - DateTime.Now.Day).ToString("MM-dd"));
       }

       stringbuilder2.Append("<categories>");
       foreach (string lm in listMouth2)
       {
           stringbuilder2.Append("<category label='" + lm + "' />");
       }
       stringbuilder2.Append("</categories>");
       foreach (System.Data.DataRow dr in dt.Rows)
       {

           stringbuilder2.Append("<dataset seriesName='平均耗时'>");
           foreach (string lm in listMouth2)
           {
               ps = new BP.DA.Paras();
               ps.SQL = "SELECT COUNT(WorkID) as Num, FK_NY FROM WF_GenerWorkFlow WHERE FK_Flow=" + dbStr + "FK_Flow AND FK_NY LIKE '%" + lm + "%' GROUP BY FK_NY ";
               ps.Add(BP.WF.GenerWorkFlowAttr.FK_Flow, flowNo);
               setCount2 = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

               foreach (System.Data.DataRow drx in dt.Rows)
               {
                    setCount2 = int.Parse(drx["TSpan"].ToString());   
               }
               if (setCount2 > maxValue2)
                   maxValue2 = setCount2;

               stringbuilder2.Append("<set value='" + setCount2 + "' />");
           }
           stringbuilder2.Append("</dataset>");
       }
       stringbuilder2.Append("</chart>");
       maxValue2 += 10;//加10  不置顶

       stringbuilder2.Insert(0, "<chart baseFontSize='14'  subcaption='" + "流程:[" + fl.No + "]" + fl.Name +
                          "-平均耗时分析(天)统计" + "' formatNumberScale='0' divLineAlpha='20'" +
                          " divLineColor='CC3300' alternateHGridColor='CC3300' shadowAlpha='40'" +
                          " numvdivlines='9'  bgColor='FFFFFF,CC3300' bgAngle='270' bgAlpha='10,10'" +
                          " alternateHGridAlpha='5'   yAxisMaxValue ='" + maxValue2 + "'>");

       this.HiddenField2.Value = stringbuilder2.ToString();
       
   %>
                 <asp:HiddenField ID="HiddenField2" runat="server" />

                <div class="chart_div_con" id="slChart2">
                </div>


            </td>
        </tr>
        <tr>
            <td colspan="3">考核信息分析.
   <% 
       ps = new BP.DA.Paras();
       ps.SQL = "SELECT  A.CHSta, B.Lab, A.FK_NY,COUNT(A.MyPK) as Num FROM WF_CH A , Sys_Enum B WHERE A.CHSta=b.IntKey AND B.EnumKey='CHSta' AND A.FK_Flow=" + dbStr + "FK_Flow GROUP BY A.CHSta,A.FK_NY,B.Lab";
       ps.Add(BP.WF.GenerWorkFlowAttr.FK_Flow, flowNo);
       dt = BP.DA.DBAccess.RunSQLReturnTable(ps);

       StringBuilder stringbuilder3 = new StringBuilder();
       string title3 = string.Empty;
       DateTime nowtime3 = DateTime.Now;
       int setCount3 = 0;
       int maxValue3 = 0;
       List<string> listMouth3 = new List<string>();
       for (int i = -11; i <= 0; i++)
       {
           listMouth3.Add(nowtime3.AddMonths(i).ToString("yyyy-MM"));
       }

       
       stringbuilder3.Append("<categories>");
       foreach (string lm in listMouth3)
       {
           stringbuilder3.Append("<category label='" + lm + "' />");
       }
       stringbuilder3.Append("</categories>");
       foreach (System.Data.DataRow dr in dt.Rows)
       {
           stringbuilder3.Append("<dataset seriesName='" + dr["Lab"] + "'>");
           foreach (string lm in listMouth3)
           {
               ps = new BP.DA.Paras();
               ps.SQL = "SELECT  A.CHSta, B.Lab, A.FK_NY,COUNT(A.MyPK) as Num FROM WF_CH A , Sys_Enum B WHERE A.CHSta=b.IntKey AND B.EnumKey='CHSta' AND A.FK_Flow=" + dbStr + "FK_Flow AND FK_NY LIKE '%" + lm + "%' GROUP BY A.CHSta,A.FK_NY,B.Lab ";

               ps.Add(BP.WF.GenerWorkFlowAttr.FK_Flow, flowNo);
               dt = BP.DA.DBAccess.RunSQLReturnTable(ps);

               foreach (System.Data.DataRow drc in dt.Rows)
               {
                   setCount3 = int.Parse(dr["Num"].ToString());   
               }
               
               if (setCount3> maxValue3)
                   maxValue3 = setCount3;

               stringbuilder3.Append("<set value='" + setCount3 + "' />");
           }
           stringbuilder3.Append("</dataset>");
       }
       stringbuilder3.Append("</chart>");
       maxValue3 += 10;//加10  不置顶
       stringbuilder3.Insert(0,"<chart baseFontSize='14'  subcaption='" + "流程:[" + fl.No + "]" + fl.Name +
                          "-考核信息分析" + "' formatNumberScale='0' divLineAlpha='20'" +
                          " divLineColor='CC3300' alternateHGridColor='CC3300' shadowAlpha='40'" +
                          " numvdivlines='9'  bgColor='FFFFFF,CC3300' bgAngle='270' bgAlpha='10,10'" +
                          " alternateHGridAlpha='5'   yAxisMaxValue ='" + maxValue3 + "'>");

       this.HiddenField3.Value = stringbuilder3.ToString();
       
   %>
             <asp:HiddenField ID="HiddenField3" runat="server" />

                <div class="chart_div_con" id="slChart3">
                </div>


            </td>
        </tr>


    </table>



</asp:Content>
