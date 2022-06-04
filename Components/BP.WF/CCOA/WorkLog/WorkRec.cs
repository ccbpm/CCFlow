using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Sys;

namespace BP.CCOA.WorkLog
{
    /// <summary>
    /// 工作日志 属性
    /// </summary>
    public class WorkRecAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 模式
        /// </summary>
        public const string WorkRecModel = "WorkRecModel";
        /// <summary>
        /// 内容1
        /// </summary>
        public const string Doc1 = "Doc1";
        /// <summary>
        /// 内容2
        /// </summary>
        public const string Doc2 = "Doc2";
        /// <summary>
        /// 内容3
        /// </summary>
        public const string Doc3 = "Doc3";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人名称
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 隶属日期
        /// </summary>
        public const string RiQi = "RiQi";
        /// <summary>
        /// 年月
        /// </summary>
        public const string NianYue = "NianYue";
        /// <summary>
        /// 项目数
        /// </summary>
        public const string NumOfItem = "NumOfItem";
        /// <summary>
        /// 第几周
        /// </summary>
        public const string WeekNum = "WeekNum";
        /// <summary>
        /// 年度
        /// </summary>
        public const string NianDu = "NianDu";
        /// <summary>
        /// 合计小时.
        /// </summary>
        public const string HeiJiHour = "HeiJiHour";
    }
    /// <summary>
    /// 工作日志
    /// </summary>
    public class WorkRec : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get { return this.GetValStrByKey(WorkRecAttr.OrgNo); }
            set { this.SetValByKey(WorkRecAttr.OrgNo, value); }
        }
        public string Rec
        {
            get { return this.GetValStrByKey(WorkRecAttr.Rec); }
            set { this.SetValByKey(WorkRecAttr.Rec, value); }
        }
        public string RecName
        {
            get { return this.GetValStrByKey(WorkRecAttr.RecName); }
            set { this.SetValByKey(WorkRecAttr.RecName, value); }
        }
        public string RDT
        {
            get { return this.GetValStrByKey(WorkRecAttr.RDT); }
            set { this.SetValByKey(WorkRecAttr.RDT, value); }
        }
        /// <summary>
        /// 日期
        /// </summary>
        public string RiQi
        {
            get { return this.GetValStrByKey(WorkRecAttr.RiQi); }
            set { this.SetValByKey(WorkRecAttr.RiQi, value); }
        }
        /// <summary>
        /// 年月
        /// </summary>
        public string NianYue
        {
            get { return this.GetValStrByKey(WorkRecAttr.NianYue); }
            set { this.SetValByKey(WorkRecAttr.NianYue, value); }
        }
        public string NianDu
        {
            get { return this.GetValStrByKey(WorkRecAttr.NianDu); }
            set { this.SetValByKey(WorkRecAttr.NianDu, value); }
        }
        /// <summary>
        /// 项目数
        /// </summary>
        public int NumOfItem
        {
            get { return this.GetValIntByKey(WorkRecAttr.NumOfItem); }
            set { this.SetValByKey(WorkRecAttr.NumOfItem, value); }
        }
        /// <summary>
        /// 第几周？
        /// </summary>
        public int WeekNum
        {
            get { return this.GetValIntByKey(WorkRecAttr.WeekNum); }
            set { this.SetValByKey(WorkRecAttr.WeekNum, value); }
        }
        /// <summary>
        /// 合计小时
        /// </summary>
        public float HeiJiHour
        {
            get { return this.GetValFloatByKey(WorkRecAttr.HeiJiHour); }
            set { this.SetValByKey(WorkRecAttr.HeiJiHour, value); }
        }
        /// <summary>
        /// 日志类型
        /// </summary>
        public int WorkRecModel
        {
            get { return this.GetValIntByKey(WorkRecAttr.WorkRecModel); }
            set { this.SetValByKey(WorkRecAttr.WorkRecModel, value); }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (WebUser.IsAdmin)
                {
                    uac.IsUpdate = true;
                    return uac;
                }
                return base.HisUAC;
            }
        }
        /// <summary>
        /// 工作日志
        /// </summary>
        public WorkRec()
        {
        }
        public WorkRec(string mypk)
        {
            this.setMyPK(mypk);
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_WorkRec", "工作汇报");

                map.AddMyPK();

                map.AddDDLSysEnum(WorkRecAttr.WorkRecModel, 0, "模式", true, false, "WorkRecModel",
              "@0=日志@1=周报@2=月报");

                map.AddTBString(WorkRecAttr.Doc1, null, "本日内容", true, false, 0, 999, 10, true);
                map.AddTBString(WorkRecAttr.Doc2, null, "明日内容", true, false, 0, 999, 10, true);
                map.AddTBString(WorkRecAttr.Doc3, null, "遇到的问题", true, false, 0, 999, 10, true);

                map.AddTBFloat(WorkRecDtlAttr.HeiJiHour, 0, "合计小时", false, false);

                //map.AddTBInt(WorkRecAttr.Hour, 0, "小时", false, false);
                //map.AddTBInt(WorkRecAttr.Minute, 0, "分钟", false, false);

                map.AddTBInt(WorkRecAttr.NumOfItem, 0, "项目数", false, false);


                map.AddTBString(WorkRecAttr.OrgNo, null, "组织编号", false, false, 0, 100, 10);
                map.AddTBString(WorkRecAttr.Rec, null, "记录人", false, false, 0, 100, 10, true);
                map.AddTBString(WorkRecAttr.RecName, null, "记录人名称", false, false, 0, 100, 10, true);
                map.AddTBDateTime(WorkRecAttr.RDT, null, "记录时间", false, false);

                map.AddTBDate(WorkRecAttr.RiQi, null, "隶属日期", false, false);
                map.AddTBString(WorkRecAttr.NianYue, null, "年月", false, false, 0, 10, 10);
                map.AddTBString(WorkRecAttr.NianDu, null, "年度", false, false, 0, 10, 10);

                //RefMethod rm = new RefMethod();
                //rm.Title = "方法参数"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoParas";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "方法内容"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoDocs";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                ////rm.GroupName = "开发接口";
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        protected override bool beforeInsert()
        {
            //if (this.WorkRecModel == 2 || this.WorkRecModel == 2)
            //{
            int i = this.Retrieve(WorkRecAttr.WorkRecModel, 2, WorkRecAttr.RiQi, this.RiQi);
            if (i != 0)
                throw new Exception("err@日期为[" + this.RiQi + "]的日志已经存在");
            //   }

            this.setMyPK(DBAccess.GenerGUID());
            this.Rec = WebUser.No;
            this.RecName = WebUser.Name;
            this.OrgNo = WebUser.OrgNo;

            this.RDT = DataType.CurrentDateTime;
            this.RiQi = DataType.CurrentDate;

            this.NianYue = DataType.CurrentYearMonth; //隶属年月.
            this.NianDu = DataType.CurrentYear; //年度.

            this.WeekNum = DataType.CurrentWeek; //第几周？

            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = WebUser.OrgNo;


            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            //计算条数.
            this.NumOfItem = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS N FROM OA_WorkRecDtl WHERE RefPK='" + this.MyPK + "'");

            //计算合计工作小时..
            this.HeiJiHour = DBAccess.RunSQLReturnValInt("SELECT SUM(Hour) + Sum(Minute)/60.00 AS N FROM OA_WorkRecDtl WHERE RefPK='" + this.MyPK + "'");

            return base.beforeUpdate();
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 工作日志 s
    /// </summary>
    public class WorkRecs : EntitiesMyPK
    {
        #region 主页数据. Default.htm
        /// <summary>
        /// 最近的数据
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            //查询最近的日志数据.
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(WorkRecAttr.Rec, WebUser.No);
            qo.Top = 20;
            qo.addOrderByDesc(WorkRecAttr.RDT);
            qo.DoQuery();

            return this.ToJson();

        }
        /// <summary>
        /// 默认页面的本周按时完成率.
        /// </summary>
        /// <returns></returns>
        public string Default_ChartInit()
        {
            Hashtable ht = new Hashtable();

            //本周完成的数量.
            var sql = "SELECT COUNT(*) FROM OA_WorkRecDtl WHERE Rec='" + BP.Web.WebUser.No + "' AND WeekNum='" + DataType.CurrentWeek + "'";
            ht.Add("weekNum", DBAccess.RunSQLReturnValInt(sql));

            //本月的数量.
            sql = "SELECT COUNT(*) FROM OA_WorkRecDtl WHERE Rec='" + BP.Web.WebUser.No + "' AND NianYue='" + DataType.CurrentYearMonth + "'";
            ht.Add("monthNum", DBAccess.RunSQLReturnValInt(sql));

            //上一个月的的数量.
            sql = "SELECT COUNT(*) FROM OA_WorkRecDtl WHERE Rec='" + BP.Web.WebUser.No + "' AND NianYue='" + DataType.CurrentNYOfPrevious + "'";
            ht.Add("monthOfPrvNum", DBAccess.RunSQLReturnValInt(sql));

            //本月的数量.
            int monthOfPrvNum = DBAccess.RunSQLReturnValInt(sql, 0);

            //本年的数量.
            sql = "SELECT COUNT(*) FROM OA_WorkRecDtl WHERE Rec='" + BP.Web.WebUser.No + "' AND NianYue='" + DataType.CurrentYear + "'";
            ht.Add("yearNum", DBAccess.RunSQLReturnValInt(sql));

            //转成Json.
            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 月份
        /// </summary>
        /// <returns></returns>
        public string Default_Months()
        {
            var sql = "SELECT COUNT(*) AS Num, NianYue FROM OA_WorkRecDtl WHERE Rec='" + BP.Web.WebUser.No + "' AND NianDu='" + DataType.CurrentYear + "'";
            sql += " GROUP BY NianYue ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        public string Default_Prjs()
        {
            var sql = "SELECT Count(*) AS Num, PrjName FROM OA_WorkRecDtl WHERE Rec='" + BP.Web.WebUser.No + "' AND NianDu='" + DataType.CurrentYear + "'";
            sql += " GROUP BY PrjName ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);

            return "";
        }
        #endregion 主页数据.

        #region 分析页面. FenXi.htm
        /// <summary>
        /// 工作周平均工作时长.
        /// </summary>
        /// <returns></returns>
        public string FenXi_WeekAvg(string empNo)
        {
            var sql = "SELECT AVG(HeiJiHour ) as Num, WeekNum as Item  FROM OA_WorkRecDtl WHERE   Rec='" + empNo + "' AND NianDu='" + DataType.CurrentYear + "'  GROUP BY WeekNum";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 工作月平均工作时长
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public string FenXi_MonthAvg(string empNo)
        {
            var sql = "SELECT AVG(HeiJiHour ) as Num, NianYue as Item  FROM OA_WorkRecDtl WHERE   Rec='" + empNo + "' AND NianDu='" + DataType.CurrentYear + "'  GROUP BY NianYue";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 分析页面.

        #region 分析页面. FenXi.htm 项目分析。
        public string FenXi_PrjWeek(string empNo)
        {
            var sql = "SELECT SUM(HeiJiHour) as Num, PrjName, WeekNum as Item  FROM OA_WorkRecDtl WHERE   Rec='" + empNo + "' AND NianDu='" + DataType.CurrentYear + "'  GROUP BY PrjName,WeekNum";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        public string FenXi_PrjMonth(string empNo)
        {
            var sql = "SELECT SUM(HeiJiHour) as Num, PrjName, NianYue as Item  FROM OA_WorkRecDtl WHERE   Rec='" + empNo + "' AND NianDu='" + DataType.CurrentYear + "'  GROUP BY PrjName,NianYue";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        public string FenXi_Prj(string empNo)
        {
            var sql = "SELECT SUM(HeiJiHour) as Num, PrjName  FROM OA_WorkRecDtl WHERE   Rec='" + empNo + "' AND NianDu='" + DataType.CurrentYear + "'  GROUP BY PrjName";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 分析页面. FenXi.htm
        public string CheckRZ_Init()
        {
            string sql = "SELECT A.* FROM OA_WorkRec A,OA_WorkShare B WHERE A.Rec=B.EmpNo AND B.ShareToEmpNo='" + BP.Web.WebUser.No + "' AND B.ShareState=1 ORDER BY A.RDT ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// xxxx
        /// </summary>
        /// <returns></returns>
        public string FenXi_Init()
        {
            // HtmlVar信息块 ， 本周完成数， 本月完成数，
            BP.CCFast.Portal.WindowExt.HtmlVar html = new BP.CCFast.Portal.WindowExt.HtmlVar();
            html.GetValDocHtml();
            html.Insert();

            return "";

        }

        /// <summary>
        /// 工作日志
        /// </summary>
        public WorkRecs() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WorkRec();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<WorkRec> ToJavaList()
        {
            return (System.Collections.Generic.IList<WorkRec>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<WorkRec> Tolist()
        {
            System.Collections.Generic.List<WorkRec> list = new System.Collections.Generic.List<WorkRec>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((WorkRec)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
