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
    /// 独立运行流程设置 属性
    /// </summary>
    public class StandAloneFlowAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 功能ID
        /// </summary>
        public const string Docs = "Docs";
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
        /// 年月
        /// </summary>
        public const string NianYue = "NianYue";
        public const string IsStar = "IsStar";
    }
    /// <summary>
    /// 独立运行流程设置
    /// </summary>
    public class StandAloneFlow : EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get { return this.GetValStrByKey(StandAloneFlowAttr.OrgNo); }
            set { this.SetValByKey(StandAloneFlowAttr.OrgNo, value); }
        }
        /// <summary>
        /// 记录人
        /// </summary>
        public string Rec
        {
            get { return this.GetValStrByKey(StandAloneFlowAttr.Rec); }
            set { this.SetValByKey(StandAloneFlowAttr.Rec, value); }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get { return this.GetValStrByKey(StandAloneFlowAttr.RDT); }
            set { this.SetValByKey(StandAloneFlowAttr.RDT, value); }
        }
        /// <summary>
        /// 年月
        /// </summary>
        public string NianYue
        {
            get { return this.GetValStrByKey(StandAloneFlowAttr.NianYue); }
            set { this.SetValByKey(StandAloneFlowAttr.NianYue, value); }
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
        /// 独立运行流程设置
        /// </summary>
        public StandAloneFlow()
        {
        }
        public StandAloneFlow(string mypk)
        {
            this.No = mypk;
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

                Map map = new Map("WF_Flow", "独立运行流程设置");

                map.AddTBStringPK(StandAloneFlowAttr.No, null, "流程编号", true, true, 0, 100, 10);
                map.AddTBString(StandAloneFlowAttr.Name, null, "流程名称", true, false, 0, 300, 10);

                //  map.AddBoolean(StandAloneFlowAttr.IsStar, false, "是否标星", false, false);

                //map.AddTBString(StandAloneFlowAttr.Is, null, "流程名称", true, false, 0, 300, 10);
                //map.AddTBStringDoc(StandAloneFlowAttr.Docs, null, "内容", true, false);
                //map.AddTBString(StandAloneFlowAttr.OrgNo, null, "OrgNo", false, false, 0, 100, 10);
                //map.AddTBString(StandAloneFlowAttr.Rec, null, "记录人", false, false, 0, 100, 10, true);
                //map.AddTBDateTime(StandAloneFlowAttr.RDT, null, "记录时间", false, false);
                //map.AddTBString(StandAloneFlowAttr.NianYue, null, "NianYue", false, false, 0, 10, 10);
                //map.AddTBInt(StandAloneFlowAttr.IsStar, 0, "是否标星", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        protected override bool beforeInsert()
        {
            throw new Exception("err@");
            return base.beforeInsert();
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 独立运行流程设置 s
    /// </summary>
    public class StandAloneFlows : EntitiesNoName
    {
        /// <summary>
        /// 独立运行流程设置
        /// </summary>
        public StandAloneFlows() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new StandAloneFlow();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<StandAloneFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<StandAloneFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<StandAloneFlow> Tolist()
        {
            System.Collections.Generic.List<StandAloneFlow> list = new System.Collections.Generic.List<StandAloneFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((StandAloneFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
