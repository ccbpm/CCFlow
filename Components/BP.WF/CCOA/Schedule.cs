using BP.DA;
using BP.Web;
using BP.En;

namespace BP.CCOA
{
    /// <summary>
    /// 日程 属性
    /// </summary>
    public class ScheduleAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 功能ID
        /// </summary>
        public const string DTStart = "DTStart";
        /// <summary>
        /// 从
        /// </summary>
        public const string TimeStart = "TimeStart";

        /// <summary>
        /// 功能来源
        /// </summary>
        public const string DTEnd = "DTEnd";
        /// <summary>
        /// 时间从
        /// </summary>
        public const string TimeEnd = "TimeEnd";
        /// <summary>
        /// 持续时间
        /// </summary>
        public const string ChiXuTime = "ChiXuTime";

        /// <summary>
        /// 重复方式
        /// </summary>
        public const string Repeats = "Repeats";
        /// <summary>
        /// 位置
        /// </summary>
        public const string Local = "Local";
        /// <summary>
        /// 描述
        /// </summary>
        public const string MiaoShu = "MiaoShu";
        /// <summary>
        /// 提醒时间
        /// </summary>
        public const string DTAlert = "DTAlert";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 年月.
        /// </summary>
        public const string NianYue = "NianYue";

    }
    /// <summary>
    /// 日程
    /// </summary>
    public class Schedule : EntityMyPK
    {
        #region 基本属性
        public string NianYue
        {
            get { return this.GetValStrByKey(ScheduleAttr.NianYue); }
            set { this.SetValByKey(ScheduleAttr.NianYue, value); }
        }
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get { return this.GetValStrByKey(ScheduleAttr.OrgNo); }
            set { this.SetValByKey(ScheduleAttr.OrgNo, value); }
        }
        public string Rec
        {
            get { return this.GetValStrByKey(ScheduleAttr.Rec); }
            set { this.SetValByKey(ScheduleAttr.Rec, value); }
        }
        public string RDT
        {
            get { return this.GetValStrByKey(ScheduleAttr.RDT); }
            set { this.SetValByKey(ScheduleAttr.RDT, value); }
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
        /// 日程
        /// </summary>
        public Schedule()
        {
        }
        public Schedule(string mypk)
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

                Map map = new Map("OA_Schedule", "日程");

                map.AddMyPK();
                map.AddTBString(ScheduleAttr.Name, null, "标题", true, false, 0, 300, 10, true);

                map.AddTBDate(ScheduleAttr.DTStart, null, "开始时间", true, false);
                map.AddTBDate(ScheduleAttr.DTEnd, null, "结束时间", true, false);

                map.AddTBString(ScheduleAttr.TimeStart, null, "TimeStart", true, false, 0, 10, 10);
                map.AddTBString(ScheduleAttr.TimeEnd, null, "TimeEnd", true, false, 0, 10, 10);

                map.AddTBString(ScheduleAttr.ChiXuTime, null, "持续时间", true, false, 0, 10, 10);


                map.AddTBDateTime(ScheduleAttr.DTAlert, null, "提醒时间", true, false);

                map.AddDDLSysEnum(ScheduleAttr.Repeats, 0, "重复", true, false, "Repeat",
              "@0=永不@1=每年@2=每月");

                map.AddTBString(ScheduleAttr.Local, null, "位置", true, false, 0, 300, 10, true);
                map.AddTBString(ScheduleAttr.MiaoShu, null, "描述", true, false, 0, 300, 10, true);

                map.AddTBString(ScheduleAttr.NianYue, null, "隶属年月", false, false, 0, 10, 10);

                map.AddTBString(ScheduleAttr.OrgNo, null, "OrgNo", false, false, 0, 100, 10);
                map.AddTBString(ScheduleAttr.Rec, null, "记录人", false, false, 0, 100, 10, true);
                map.AddTBDateTime(ScheduleAttr.RDT, null, "记录时间", false, false);


                //RefMethod rm = new RefMethod();
                //rm.Title = "方法参数"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoParas";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                ////rm.GroupName = "开发接口";
                ////  map.AddRefMethod(rm);

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
            this.setMyPK(DBAccess.GenerGUID());
            this.Rec = WebUser.No;
            this.OrgNo = WebUser.OrgNo;

            this.RDT = DataType.CurrentDateTime;
            this.NianYue = DataType.CurrentYearMonth;

            return base.beforeInsert();
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 日程 s
    /// </summary>
    public class Schedules : EntitiesMyPK
    {
        /// <summary>
        /// 查询事件到.
        /// </summary>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public string DTFromTo(string dtFrom, string dtTo)
        {
            this.RetrieveAll();
            return this.ToJson();
        }
        /// <summary>
        /// 日程
        /// </summary>
        public Schedules() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Schedule();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Schedule> ToJavaList()
        {
            return (System.Collections.Generic.IList<Schedule>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Schedule> Tolist()
        {
            System.Collections.Generic.List<Schedule> list = new System.Collections.Generic.List<Schedule>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Schedule)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
