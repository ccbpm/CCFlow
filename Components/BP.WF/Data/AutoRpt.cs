using BP.En;

namespace BP.WF.Data
{
    /// <summary>
    /// 自动报表 属性
    /// </summary>
    public class AutoRptAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 到达人员（可以为空）
        /// </summary>
        public const string ToEmps = "ToEmps";
        public const string ToEmpOfSQLs = "ToEmpOfSQLs";
        public const string ToStations = "ToStations";
        public const string ToDepts = "ToDepts";
        public const string BeiZhu = "BeiZhu";
        public const string DTOfExe = "DTOfExe";
        /// <summary>
        /// 发起时间点
        /// </summary>
        public const string StartDT = "StartDT";
        /// <summary>
        /// 执行的时间点
        /// </summary>
        public const string Dots = "Dots";
        #endregion
    }
    /// <summary>
    /// 自动报表
    /// </summary>
    public class AutoRpt : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 发起时间点
        /// </summary>
        public string StartDT
        {
            get
            {
                return this.GetValStringByKey(AutoRptAttr.StartDT);
            }
            set
            {
                this.SetValByKey(AutoRptAttr.StartDT, value);
            }
        }
        /// <summary>
        /// 执行的时间点.
        /// </summary>
        public string Dots
        {
            get
            {
                return this.GetValStringByKey(AutoRptAttr.Dots);
            }
            set
            {
                this.SetValByKey(AutoRptAttr.Dots, value);
            }
        }
        /// <summary>
        /// 执行时间
        /// </summary>
        public string DTOfExe
        {
            get
            {
                return this.GetValStringByKey(AutoRptAttr.DTOfExe);
            }
            set
            {
                this.SetValByKey(AutoRptAttr.DTOfExe, value);
            }
        }
        /// <summary>
        /// 到达的人员
        /// </summary>
        public string ToEmps
        {
            get
            {
                return this.GetValStringByKey(AutoRptAttr.ToEmps);
            }
            set
            {
                this.SetValByKey(AutoRptAttr.ToEmps, value);
            }
        }
        public string ToEmpOfSQLs
        {
            get
            {
                return this.GetValStringByKey(AutoRptAttr.ToEmpOfSQLs);
            }
            set
            {
                this.SetValByKey(AutoRptAttr.ToEmpOfSQLs, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// AutoRpt
        /// </summary>
        public AutoRpt()
        {

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

                Map map = new Map("WF_AutoRpt", "自动报表");
                map.CodeStruct = "2";

                map.AddTBStringPK(AutoRptAttr.No, null, "编号", true, true, 2, 2, 2);
                map.AddTBDateTime(AutoRptAttr.DTOfExe, null, "最近执行时间", true, true);

                map.AddTBString(AutoRptAttr.Name, null, "标题", true, false, 0, 200, 200, true);
                map.AddTBString(AutoRptAttr.StartDT, null, "发起时间点", true, false, 0, 200, 200, true);
                map.SetHelperAlert(AutoRptAttr.StartDT, "比如:08:03,20:15   多个时间点用逗号分开，一定是HH:mm格式的时间点.");

                // map.AddTBString(AutoRptAttr.ToEmps, null, "通知的人员ID", true, false, 0, 200, 10);
                map.AddTBString(AutoRptAttr.ToEmpOfSQLs, null, "通知的人员SQL", true, false, 0, 500, 10,true);
                map.SetHelperAlert(AutoRptAttr.ToEmpOfSQLs,"查询出来要通知的人员，返回列必须是No,Name 比如:SELECT top 100 No,Name FROM Port_Emp ");

                //map.AddTBString(AutoRptAttr.ToStations, null, "通知的岗位", true, false, 0, 200, 10);
                //map.AddTBString(AutoRptAttr.ToDepts, null, "通知的部门", true, false, 0, 200, 10);
              

                map.AddTBStringDoc(AutoRptAttr.Dots, null, "执行的时间点(系统写入)", true, true, true);
                map.SetHelperAlert(AutoRptAttr.Dots, "系统的日志，曾经发起的时间点记录.格式为:2020-09-10 20:22,2020-09-10 22:22");

                //
                map.AddDtl( new AutoRptDtls(), AutoRptDtlAttr.AutoRptNo);

                RefMethod rm = new RefMethod();
                rm.Title = "手工执行";
                rm.ClassMethodName = this.ToString() + ".DoIt";
                rm.RefMethodType = RefMethodType.Func;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string DoIt()
        {
            BP.WF.DTS.Auto_Rpt_Dtl_DTS dts = new BP.WF.DTS.Auto_Rpt_Dtl_DTS();
            string str=dts.Do() as string;

            BP.WF.Dev2Interface.Port_Login("admin");

            return str;
        }
    }
    /// <summary>
    /// 自动报表
    /// </summary>
    public class AutoRpts : EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new AutoRpt();
            }
        }
        /// <summary>
        /// 自动报表
        /// </summary>
        public AutoRpts() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<AutoRpt> ToJavaList()
        {
            return (System.Collections.Generic.IList<AutoRpt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<AutoRpt> Tolist()
        {
            System.Collections.Generic.List<AutoRpt> list = new System.Collections.Generic.List<AutoRpt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((AutoRpt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
