using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.WF.Data
{
    /// <summary>
    /// 流程 属性
    /// </summary>
    public class FlowSimpleAttr : EntityNoNameAttr
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
    /// 流程
    /// </summary>
    public class FlowSimple : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 发起时间点
        /// </summary>
        public string StartDT
        {
            get
            {
                return this.GetValStringByKey(FlowSimpleAttr.StartDT);
            }
            set
            {
                this.SetValByKey(FlowSimpleAttr.StartDT, value);
            }
        }
        /// <summary>
        /// 执行的时间点.
        /// </summary>
        public string Dots
        {
            get
            {
                return this.GetValStringByKey(FlowSimpleAttr.Dots);
            }
            set
            {
                this.SetValByKey(FlowSimpleAttr.Dots, value);
            }
        }
        /// <summary>
        /// 执行时间
        /// </summary>
        public string DTOfExe
        {
            get
            {
                return this.GetValStringByKey(FlowSimpleAttr.DTOfExe);
            }
            set
            {
                this.SetValByKey(FlowSimpleAttr.DTOfExe, value);
            }
        }
        /// <summary>
        /// 到达的人员
        /// </summary>
        public string ToEmps
        {
            get
            {
                return this.GetValStringByKey(FlowSimpleAttr.ToEmps);
            }
            set
            {
                this.SetValByKey(FlowSimpleAttr.ToEmps, value);
            }
        }
        public string ToEmpOfSQLs
        {
            get
            {
                return this.GetValStringByKey(FlowSimpleAttr.ToEmpOfSQLs);
            }
            set
            {
                this.SetValByKey(FlowSimpleAttr.ToEmpOfSQLs, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// FlowSimple
        /// </summary>
        public FlowSimple()
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

                Map map = new Map("WF_Flow", "流程");
                map.EnType = EnType.View;

                map.AddTBStringPK(FlowSimpleAttr.No, null, "编号", true, true, 3, 3, 3);
                map.AddTBString(FlowSimpleAttr.Name, null, "名称", true, false, 0, 200, 200, true);
                map.AddTBString("OrgNo", null, "OrgNo", false, false, 0, 200, 200);

                map.AddHidden("OrgNo", " = ", "@WebUser.OrgNo");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
 
    }
    /// <summary>
    /// 流程
    /// </summary>
    public class FlowSimples : EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowSimple();
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public FlowSimples() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowSimple> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowSimple>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowSimple> Tolist()
        {
            System.Collections.Generic.List<FlowSimple> list = new System.Collections.Generic.List<FlowSimple>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowSimple)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
