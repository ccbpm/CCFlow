<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true"
    CodeBehind="ByFlows.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.App.CH.ByFlows" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../../../Scripts/jquery/themes/default/easyui.css" rel="stylesheet"
        type="text/css" />
    <link href="../../../../Scripts/jquery/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../../../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
        }
        .center
        {
            text-align: center;
        }
        .flowTitle div
        {
            cursor: pointer;
            width: 200px;
            overflow: hidden;
            white-space: nowrap;
            text-overflow: ellipsis;
            -o-text-overflow: ellipsis;
            -icab-text-overflow: ellipsis;
            -khtml-text-overflow: ellipsis;
            -moz-text-overflow: ellipsis;
            -webkit-text-overflow: ellipsis;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!--
1, 如果流程没有启动，就不让其显示了。
-->
    <table style="width: 100%;">
        <caption>
            按流程分析</caption>
        <tr>
            <th class="GroupTitle" rowspan="2">
                IDX
            </th>
            <th rowspan="2">
                流程名称
            </th>
            <th rowspan="2">
                发起总数
            </th>
            <th colspan="2">
                时效统计(单位:分钟)
            </th>
            <th colspan="5">
                状态分布
            </th>
            <th colspan="3">
                上月
            </th>
            <th colspan="3">
                本月
            </th>
            <th rowspan="2">
                详细
            </th>
        </tr>
        <tr>
            <th>
                节省
            </th>
            <th>
                逾期时间
            </th>
            <th>
                及时
            </th>
            <th>
                按期
            </th>
            <th>
                逾期
            </th>
            <th>
                超期
            </th>
            <th>
                按期办结率
            </th>

            <th>按期</th>
            <th>逾期</th>
            <th>按期办结率</th>


            <th>按期</th>
            <th>逾期</th>
            <th>按期办结率</th>

        </tr>
        <%
            //类别.
            BP.WF.Template.FlowSorts flowSorts = new BP.WF.Template.FlowSorts();
            flowSorts.RetrieveAll();

            //流程.
            BP.WF.Flows flows = new BP.WF.Flows();
            flows.RetrieveAll();

            // 获得流程总量.
            System.Data.DataTable dt
                = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE WFState!=0 GROUP BY FK_Flow ");


            // 获得节省时间
            System.Data.DataTable dtSaveTimeMin
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow, SUM(OverMinutes) AS OverMinutes FROM WF_CH WHERE OverMinutes <0 GROUP BY FK_Flow ");

            // 获得超期时间
            System.Data.DataTable dtOverTimeMin
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow, SUM(OverMinutes) AS OverMinutes FROM WF_CH WHERE OverMinutes > 0 GROUP BY FK_Flow  ");

            //及时完成
            System.Data.DataTable dtInTimeCount
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE CHSta='" +
                                               (int)BP.WF.Data.CHSta.JiShi + "' GROUP BY FK_Flow ");

            //按期完成
            System.Data.DataTable dtOnTimeCount
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE CHSta='" +
                                               (int)BP.WF.Data.CHSta.AnQi + "' GROUP BY FK_Flow ");
           
            // 获得逾期的工作数量.
            System.Data.DataTable dtOverTimeCount
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE CHSta='" +
                                               (int)BP.WF.Data.CHSta.YuQi + "' GROUP BY FK_Flow ");

            // 获得超期的工作数量.
            System.Data.DataTable dtCqTimeCount
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE CHSta='" +
                                               (int)BP.WF.Data.CHSta.ChaoQi + "' GROUP BY FK_Flow  ");

            //上月办结率
            DateTime dTime = DateTime.Now;
            //总数
            System.Data.DataTable dtLastMouthTotal = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT(distinct WorkID) Num FROM WF_CH WHERE FK_NY ='" +
               dTime.AddMonths(-1).ToString("yyyy-MM") + "' GROUP BY FK_Flow ");

            //按时
            System.Data.DataTable dtLastMouthBJ = BP.DA.DBAccess.RunSQLReturnTable("SELECT  FK_Flow,COUNT(distinct WorkID) Num FROM WF_CH WHERE FK_NY ='" +
                dTime.AddMonths(-1).ToString("yyyy-MM") + "' AND CHSta IN(0,1) GROUP BY FK_Flow ");
            #region //上月按期完成/预期完成

            //上月按期完成
            System.Data.DataTable dtListOnTimeCount
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE FK_NY ='" +
                dTime.AddMonths(-1).ToString("yyyy-MM") + "'AND  CHSta='" +
                                               (int)BP.WF.Data.CHSta.AnQi + "' GROUP BY FK_Flow ");
          //上月预期
            System.Data.DataTable dtListOverTimeCount
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE FK_NY ='" +
                dTime.AddMonths(-1).ToString("yyyy-MM") + "' AND CHSta='" +
                                               (int)BP.WF.Data.CHSta.YuQi + "' GROUP BY FK_Flow ");
                                               
         //本月按期完成
            System.Data.DataTable dtThisOnTimeCount
               = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE FK_NY ='" +
                  dTime.ToString("yyyy-MM") + "'AND  CHSta='" +
                                                 (int)BP.WF.Data.CHSta.AnQi + "' GROUP BY FK_Flow ");
         //本月预期完成
            System.Data.DataTable dtThisOverTimeCount
               = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE FK_NY ='" +
                  dTime.ToString("yyyy-MM") + " 'AND CHSta='" +
                                                 (int)BP.WF.Data.CHSta.YuQi + "' GROUP BY FK_Flow ");
            #endregion


            //本月办结率
            //总数
            System.Data.DataTable dtThisMouthTotal = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT(distinct WorkID) Num FROM WF_CH WHERE FK_NY ='" +
               dTime.ToString("yyyy-MM") + "' GROUP BY FK_Flow ");

            //按时
            System.Data.DataTable dtThisMouthBJ = BP.DA.DBAccess.RunSQLReturnTable("SELECT  FK_Flow,COUNT(distinct WorkID) Num FROM WF_CH WHERE FK_NY ='" +
                dTime.ToString("yyyy-MM") + "' AND CHSta IN(0,1) GROUP BY FK_Flow ");


            foreach (BP.WF.Template.FlowSort flowSort in flowSorts)
            {
        %>
        <tr>
            <th>
            </th>
            <th class="GroupField" colspan="23">
                <%=flowSort.Name%>
            </th>
        </tr>
        <% int idx = 0;
           foreach (BP.WF.Flow flow in flows)
           {
               if (flow.FK_FlowSort != flowSort.No)
                   continue; //非该类别下的流程.
               idx += 1;
               //求发起总数.
               int relNum = 0;
               foreach (System.Data.DataRow dr in dt.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       relNum = int.Parse(dr[1].ToString());
                       break;
                   }
               }
               

               //求提前完成的分钟数.
               decimal saveMinte = 0;
               foreach (System.Data.DataRow dr in dtSaveTimeMin.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       saveMinte = int.Parse(dr[1].ToString());
                       break;
                   }
               }


               //求提 逾期的 分钟数.
               decimal overMinte = 0;
               foreach (System.Data.DataRow dr in dtOverTimeMin.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       overMinte = decimal.Parse(dr[1].ToString());
                       break;
                   }
               }

               //及时
               int inTimeCount = 0;
               foreach (System.Data.DataRow dr in dtInTimeCount.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       inTimeCount = int.Parse(dr[1].ToString());
                   }
               }

               //按时
               int onTimeCount = 0;
               foreach (System.Data.DataRow dr in dtOnTimeCount.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       onTimeCount = int.Parse(dr[1].ToString());
                   }
               }


               //上月按时
               int onListTimeCount = 0;
               foreach (System.Data.DataRow dr in dtListOnTimeCount.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       onListTimeCount = int.Parse(dr[1].ToString());
                   }
               }
               
               //本月按时
               int onThisTimeCount = 0;
               foreach (System.Data.DataRow dr in dtThisOnTimeCount.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       onThisTimeCount = int.Parse(dr[1].ToString());
                   }
               }

               //逾期
               int yqTimeCount = 0;
               foreach (System.Data.DataRow dr in dtOverTimeCount.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       yqTimeCount = int.Parse(dr[1].ToString());
                   }
               }
               //本月逾期
               int yqThisTimeCount = 0;
               foreach (System.Data.DataRow dr in dtThisOverTimeCount.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       yqThisTimeCount = int.Parse(dr[1].ToString());
                   }
               }

              //上月预期

               int yqListTimeCount = 0;
               foreach (System.Data.DataRow dr in dtListOverTimeCount.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       yqListTimeCount = int.Parse(dr[1].ToString());
                   }
               }
               
               //超期
               int cqTimeCount = 0;
               foreach (System.Data.DataRow dr in dtCqTimeCount.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       cqTimeCount = int.Parse(dr[1].ToString());
                   }
               }

               //按期办结率
               int totalCount = inTimeCount + onTimeCount + yqTimeCount + cqTimeCount;
               decimal bjl = 0;
               if (totalCount != 0)
                   bjl = (inTimeCount + onTimeCount) * 100 / totalCount;


               //上月按时办结率
               decimal lastMouthBJL = 0;
               int lastMouthBJCount = 0;//办结
               int lastMouthTotalCount = 0;//总量

               foreach (System.Data.DataRow dr in dtLastMouthBJ.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       lastMouthBJCount = int.Parse(dr[1].ToString());
                   }
               }

               foreach (System.Data.DataRow dr in dtLastMouthTotal.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       lastMouthTotalCount = int.Parse(dr[1].ToString());
                   }
               }

               if (lastMouthTotalCount != 0)
               {
                   lastMouthBJL = lastMouthBJCount * 100 / lastMouthTotalCount;
               }
               
              
                   
               
               //本月按时办结率
               decimal thisMouthBJL = 0;
               int thisMouthBJCount = 0;//办结
               int thisMouthTotalCount = 0;//总量

               foreach (System.Data.DataRow dr in dtThisMouthBJ.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       thisMouthBJCount = int.Parse(dr[1].ToString());
                   }
               }

               foreach (System.Data.DataRow dr in dtThisMouthTotal.Rows)
               {
                   if (dr["FK_Flow"].ToString() == flow.No)
                   {
                       thisMouthTotalCount = int.Parse(dr[1].ToString());
                   }
               }

               if (thisMouthTotalCount != 0)
                   thisMouthBJL = thisMouthBJCount * 100 / thisMouthTotalCount;
        %>
        <tr>
            <td class="Idx">
                <%=idx %>
            </td>
            <td >
            <a href="../OneFlow/Welcome.aspx?FK_Flow=<%=flow.No %>" >
                    <%=flow.Name%> 
                    </a>
            </td>
            <td class="center">
                <%=relNum %>
            </td>
            <td class="center">
                <%=saveMinte%>
            </td>
            <td class="center">
                <%=overMinte%>
            </td>
            <td class="center">
                <%=inTimeCount%>
            </td>
            <td class="center">
                <%=onTimeCount%>
            </td>
            <td class="center">
                <%=yqTimeCount%>
            </td>
            <td class="center">
                <%=cqTimeCount%>
            </td>
            <td class="center">
                <%=bjl.ToString("0.00")%>%
            </td>

             <td class="center">
                <%=onListTimeCount %>
            </td>
            <td class="center">
                 <%=yqListTimeCount%>
            </td>
                <td class="center">
                  <%=thisMouthBJL.ToString("0.00")%>%
            </td>

            <td class="center" >
               <%=onThisTimeCount %>
            </td>
            <td  class="center"> <%=yqThisTimeCount %> </td>
            <td class="center">
                <%=lastMouthBJL.ToString("0.00")%>%
            </td>
        
            <!-- 链接到详细 -->
            <td class="center">
                <a href="../OneFlow/Nodes.aspx?FK_Flow=<%=flow.No %>">详细</a>
            </td>
            </tr>

                <%}
            }
            
                %>
        <%
            
            // 所有流程发起总数Total
            int relTotal = 0;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                    relTotal = int.Parse(dr[1].ToString());
                    break;
            }
            //求提前完成的分钟数Total.
            decimal saveMinteTotal = 0;
            foreach (System.Data.DataRow dr in dtSaveTimeMin.Rows)
            {

                saveMinteTotal = int.Parse(dr[1].ToString());
                    break;
            }
            //求提 逾期的 分钟数Total.
            decimal overMinteTotal = 0;
            foreach (System.Data.DataRow dr in dtOverTimeMin.Rows)
            {
                
                    overMinteTotal = decimal.Parse(dr[1].ToString());
                    break;
                
            }
            //及时Total
            int inTimeCountTotal = 0;
            foreach (System.Data.DataRow dr in dtInTimeCount.Rows)
            {
                
                    inTimeCountTotal = int.Parse(dr[1].ToString());
              
            }

            //按时Total
            int onTimeCountTotal = 0;
            foreach (System.Data.DataRow dr in dtOnTimeCount.Rows)
            {
                    onTimeCountTotal = int.Parse(dr[1].ToString());
            }


            //上月按时Total
            int onListTimeCountTotal = 0;
            foreach (System.Data.DataRow dr in dtListOnTimeCount.Rows)
            {
                        
                    onListTimeCountTotal = int.Parse(dr[1].ToString());
            }

            //本月按时Total
            int onThisTimeCountTotal= 0;
            foreach (System.Data.DataRow dr in dtThisOnTimeCount.Rows)
            {
              
                    onThisTimeCountTotal = int.Parse(dr[1].ToString());

            }

            //逾期Total
            int yqTimeCountTotal = 0;
            foreach (System.Data.DataRow dr in dtOverTimeCount.Rows)
            {
             
                    yqTimeCountTotal = int.Parse(dr[1].ToString());
            }
            //本月逾期Total
            int yqThisTimeCountTotal = 0;
            foreach (System.Data.DataRow dr in dtThisOverTimeCount.Rows)
            {
               
                    yqThisTimeCountTotal = int.Parse(dr[1].ToString());
            }

            //上月预期Total

            int yqListTimeCountTotal = 0;
            foreach (System.Data.DataRow dr in dtListOverTimeCount.Rows)
            {
               
                    yqListTimeCountTotal = int.Parse(dr[1].ToString());
            }

            //超期Total
            int cqTimeCountTotal = 0;
            foreach (System.Data.DataRow dr in dtCqTimeCount.Rows)
            {
                    cqTimeCountTotal = int.Parse(dr[1].ToString());
            }

            //按期办结率Total
            int totalCountTotal = inTimeCountTotal + onTimeCountTotal + yqTimeCountTotal + cqTimeCountTotal;
            decimal bjltotal = 0;
            if (totalCountTotal != 0)
                bjltotal = (inTimeCountTotal + onTimeCountTotal) * 100 / totalCountTotal;


            //上月按时办结率Total
            decimal lastMouthBJLTotal = 0;
            int lastMouthBJCountTotal = 0;//办结
            int lastMouthTotalCountTotal = 0;//总量

            foreach (System.Data.DataRow dr in dtLastMouthBJ.Rows)
            {
               
                    lastMouthBJCountTotal = int.Parse(dr[1].ToString());
            }

            foreach (System.Data.DataRow dr in dtLastMouthTotal.Rows)
            {
                    lastMouthTotalCountTotal = int.Parse(dr[1].ToString());
            }

            if (lastMouthTotalCountTotal != 0)
            {
                lastMouthBJLTotal = lastMouthBJCountTotal * 100 / lastMouthTotalCountTotal;
            }




            //本月按时办结率Total
            decimal thisMouthBJLTotal = 0;
            int thisMouthBJCountTotal = 0;//办结
            int thisMouthTotalCountTotal = 0;//总量

            foreach (System.Data.DataRow dr in dtThisMouthBJ.Rows)
            {
   
                    thisMouthBJCountTotal = int.Parse(dr[1].ToString());

            }

            foreach (System.Data.DataRow dr in dtThisMouthTotal.Rows)
            {
              
                    thisMouthTotalCountTotal = int.Parse(dr[1].ToString());
            }

            if (thisMouthTotalCountTotal != 0)
                thisMouthBJLTotal = thisMouthBJCountTotal * 100 / thisMouthTotalCountTotal;
            
              
        %>
          <tr class="flowTitle"  >
               <td class="center" colspan="2"   style=" font-size:15px; color:green" >合计 </td>
               <td class="center"><%=relTotal %></td>
              <td class="center"> <%=saveMinteTotal %> </td>
              <td class="center"> <%=overMinteTotal %> </td>
               <td class="center"> <%=inTimeCountTotal %> </td>
              <td class="center"> <%=onTimeCountTotal %> </td>
              <td class="center"> <%=yqTimeCountTotal %> </td>
              <td class="center"> <%=cqTimeCountTotal %> </td>
              <td class="center"> <%=totalCountTotal %>%</td>
              <td class="center"> <%=onListTimeCountTotal%> </td>
              <td class="center"> <%=yqListTimeCountTotal%> </td>
              <td class="center"> <%=lastMouthBJLTotal %>% </td>
              <td class="center"> <%=onThisTimeCountTotal%> </td>
              <td class="center"> <%=yqThisTimeCountTotal%> </td>
              <td class="center"> <%=thisMouthBJLTotal%>% </td>
               
               
                </tr>
       
          
    </table>
</asp:Content>
