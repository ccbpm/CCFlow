using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Cloud;
using BP.Sys;

namespace BP.Cloud.Template
{
    /// <summary>
    /// 流程属性
    /// </summary>
    public class FlowAttr : BP.WF.Template.FlowAttr
    {
        /// <summary>
        /// 肖像
        /// </summary>
        public const string ICON = "ICON";
        /// <summary>
        /// 颜色
        /// </summary>
        public const string Color = "Color";

        public const string WorkModel = "WorkModel";
        public const string AtPara = "AtPara";
    }
    /// <summary>
    ///  流程
    /// </summary>
    public class Flow : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(FlowAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(FlowAttr.OrgNo, value);
            }
        }
        public string ICON
        {
            get
            {
                return this.GetValStrByKey(FlowAttr.ICON);
            }
            set
            {
                this.SetValByKey(FlowAttr.ICON, value);
            }
        }

        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 流程
        /// </summary>
        public Flow()
        {
        }
        /// <summary>
        /// 流程
        /// </summary>
        /// <param name="_No"></param>
        public Flow(string _No) : base(_No) { }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        /// <summary>
        /// 流程Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Flow", "流程");

                map.AddTBStringPK(FlowAttr.No, null, "编号", true, true, 1, 100, 20);
                map.AddTBString(FlowAttr.Name, null, "名称", true, false, 0, 200, 30, true);
                map.AddTBString(FlowAttr.FK_FlowSort, null, "类别", false, false, 0, 200, 30, true);
                map.AddTBString(FlowAttr.OrgNo, null, "组织编号", true, false, 0, 150, 30);
                map.AddTBString(FlowAttr.ICON, null, "流程图标", true, false, 0, 200,30);
                map.AddTBInt(FlowAttr.Idx, 0, "Idx", false, false);
                map.AddTBInt(FlowAttr.FlowFrmModel, 0, "流程表单类型", false, false);
                map.AddTBString(FlowAttr.PTable, null, "流程数据存储主表", true, false, 0, 30, 10);
                map.AddTBInt(FlowAttr.WorkModel, 0, "工作模式", false, false);
                map.AddTBString(FlowAttr.AtPara, null, "AtPara", false, false, 0, 200, 30);

                this._enMap = map;
                return this._enMap;
            }
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 流程
    /// </summary>
    public class Flows : EntitiesTree
    {
        /// <summary>
        /// 流程s
        /// </summary>
        public Flows() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Flow();
            }
        }
        public override int RetrieveAll()
        {
            int i = this.Retrieve(FlowAttr.OrgNo, BP.Web.WebUser.OrgNo,
                FlowAttr.Idx);
            return i;
        }
    }
}
