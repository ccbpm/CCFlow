using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.Sys;
using BP.Pub;
using BP.En;

namespace BP.Demo
{
    /// <summary>
    /// 属性
    /// </summary>
    public class RptStudent : BP.Pub.Rpt2Base
    {
        /// <summary>
        /// 标题.
        /// </summary>
        public override string Title
        {
            get
            {
                return " Title: 学生统计分析";
            }
        }

        public override int AttrDefSelected
        {
            get { return 0; }
        }

        public override BP.Pub.Rpt2Attrs AttrsOfGroup
        {
            get {

                //定义要返回的集合.
                BP.Pub.Rpt2Attrs rpts = new Rpt2Attrs();

                //定义一个报表.
                Rpt2Attr rpt = new Rpt2Attr();
                rpt.Title = "Title:我的标题";
                rpt.DESC = "Desc : 分析学生的比例.";
                rpt.DefaultShowChartType = DBAChartType.Pie;
                

                //要显示图图形.
                rpt.IsEnableColumn = true;
                rpt.IsEnableLine = true;
                rpt.IsEnablePie = true; //仅仅显示饼图.
                rpt.IsEnableTable = true;

                rpt.LineChartShowType = LineChartShowType.None;

                rpt.DBDataTable = BP.DA.DBAccess.RunSQLReturnTable("SELECT  COUNT(No) AS value, CASE XB WHEN 0 THEN '女' WHEN 1 THEN '男' ELSE '其他' END as name, XB FROM  Demo_Student WHERE 1=1 GROUP BY XB ");
                rpts.Add(rpt);

                return rpts;
            }
        }

    }
}
