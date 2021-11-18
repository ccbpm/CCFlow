using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.CCBill.Template;

namespace BP.CCFast
{
    /// <summary>
    /// 工作内容 属性
    /// </summary>
    public class WorkRecDtlAttr : WorkRecAttr
    {
        /// <summary>
        /// 关联主键
        /// </summary>
        public const string RefPK = "RefPK";
        /// <summary>
        /// 项目
        /// </summary>
        public const string PrjName = "PrjName";
        /// <summary>
        /// 内容
        /// </summary>
        public const string Doc = "Doc";
        /// <summary>
        /// 结果
        /// </summary>
        public const string Result = "Result";
        /// <summary>
        /// 小时
        /// </summary>
        public const string Hour = "Hour";
        /// <summary>
        /// 分钟
        /// </summary>
        public const string Minute = "Minute";
        /// <summary>
        /// 合计小时数
        /// </summary>
        public const string HeiJiHour = "HeiJiHour";
        /// <summary>
        /// 项目数
        /// </summary>
        public const string NumOfItem = "NumOfItem";

    }
    /// <summary>
    /// 工作内容
    /// </summary>
    public class WorkRecDtl : EntityMyPK
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

        public float HeiJiHour
        {
            get { return this.GetValFloatByKey(WorkRecDtlAttr.HeiJiHour); }
            set { this.SetValByKey(WorkRecDtlAttr.HeiJiHour, value); }
        }
        public int Hour
        {
            get { return this.GetValIntByKey(WorkRecDtlAttr.Hour); }
            set { this.SetValByKey(WorkRecDtlAttr.Hour, value); }
        }
        public int Minute
        {
            get { return this.GetValIntByKey(WorkRecDtlAttr.Minute); }
            set { this.SetValByKey(WorkRecDtlAttr.Minute, value); }
        }
        public int WeekNum
        {
            get { return this.GetValIntByKey(WorkRecDtlAttr.WeekNum); }
            set { this.SetValByKey(WorkRecDtlAttr.WeekNum, value); }
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
        /// 工作内容
        /// </summary>
        public WorkRecDtl()
        {
        }
        public WorkRecDtl(string mypk)
        {
            this.MyPK = mypk;
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

                Map map = new Map("OA_WorkRecDtl", "工作内容");

                map.AddMyPK();

                map.AddTBString(WorkRecDtlAttr.RefPK, null, "RefPK", false, false, 0, 40, 10);

                map.AddTBString(WorkRecDtlAttr.PrjName, null, "项目名称", false, false, 0, 90, 10);
                map.AddTBString(WorkRecDtlAttr.Doc, null, "内容", false, false, 0, 999, 10);
                map.AddTBString(WorkRecDtlAttr.Result, null, "结果", false, false, 0, 999, 10);

                map.AddTBInt(WorkRecDtlAttr.Hour, 0, "小时", false, false);
                map.AddTBInt(WorkRecDtlAttr.Minute, 0, "分钟", false, false);
                map.AddTBFloat(WorkRecDtlAttr.HeiJiHour, 0, "合计小时", false, false);

                map.AddTBString(WorkRecAttr.OrgNo, null, "组织编号", false, false, 0, 100, 10);
                map.AddTBString(WorkRecAttr.Rec, null, "记录人", false, false, 0, 100, 10, true);
                map.AddTBString(WorkRecAttr.RecName, null, "记录人名称", false, false, 0, 100, 10, true);
                map.AddTBDateTime(WorkRecAttr.RDT, null, "记录时间", false, false);

                map.AddTBDate(WorkRecAttr.RiQi, null, "隶属日期", false, false);

                map.AddTBString(WorkRecAttr.NianYue, null, "年月", false, false, 0, 10, 10);
                map.AddTBString(WorkRecAttr.NianDu, null, "年度", false, false, 0, 10, 10);

                map.AddTBInt(WorkRecDtlAttr.WeekNum, 0, "周次", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        protected override bool beforeInsert()
        {
            this.MyPK = DBAccess.GenerGUID();

            this.Rec = WebUser.No;
            this.RecName = WebUser.Name;

            this.OrgNo = WebUser.OrgNo;

            this.RDT = DataType.CurrentDataTime;
           // this.RiQi = DataType.CurrentData;

            this.NianYue = DataType.CurrentYearMonth; //隶属年月.

            //小时数.
            this.HeiJiHour = this.Hour + this.Minute / 60;

            //第几周.
            this.WeekNum = DataType.CurrentWeek;

            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = WebUser.OrgNo;

            return base.beforeInsert();
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 工作内容 s
    /// </summary>
    public class WorkRecDtls : EntitiesMyPK
    {
        /// <summary>
        /// 工作内容
        /// </summary>
        public WorkRecDtls() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WorkRecDtl();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<WorkRecDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<WorkRecDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<WorkRecDtl> Tolist()
        {
            System.Collections.Generic.List<WorkRecDtl> list = new System.Collections.Generic.List<WorkRecDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((WorkRecDtl)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
