using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.CCBill.Template
{
	/// <summary>
	/// 台账子流程属性
	/// </summary>
    public class DictFlowAttr:EntityMyPKAttr
    {
        #region 基本属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 子流程编号
        /// </summary>
        public const string FlowNo = "FlowNo";
        /// <summary>
        /// 标签
        /// </summary>
        public const string Label = "Label";
        /// <summary>
        /// 是否显示在表格右边
        /// </summary>
        public const string IsShowListRight = "IsShowListRight";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";
        #endregion 基本属性.
    }
	/// <summary>
	/// 台账子流程
	/// </summary>
    public class DictFlow : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(DictFlowAttr.FrmID);
            }
            set
            {
                this.SetValByKey(DictFlowAttr.FrmID, value);
            }
        }
       
        /// <summary>
        /// 方法名
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(DictFlowAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(DictFlowAttr.FlowNo, value);
            }
        }
        /// <summary>
        /// 显示标签
        /// </summary>
        public string Label
        {
            get
            {
                return this.GetValStringByKey(DictFlowAttr.Label);
            }
            set
            {
                this.SetValByKey(DictFlowAttr.Label, value);
            }
        }
        /// <summary>
        /// 是否显示在表格右边
        /// </summary>
        public int IsShowListRight
        {
            get
            {
                return this.GetValIntByKey(DictFlowAttr.IsShowListRight);
            }
            set
            {
                this.SetValByKey(DictFlowAttr.IsShowListRight, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(DictFlowAttr.Idx);
            }
            set
            {
                this.SetValByKey(DictFlowAttr.Idx, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 台账子流程
        /// </summary>
        public DictFlow()
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

                Map map = new Map("Frm_DictFlow", "台账子流程");

                map.AddMyPK();

                map.AddTBString(DictFlowAttr.FrmID, null, "表单ID", true, false, 0, 300, 10);
                map.AddTBString(DictFlowAttr.FlowNo, null, "流程编号", true, false, 0, 20, 10);
                map.AddTBString(DictFlowAttr.Label, null, "功能标签", true, false, 0, 20, 10);
                map.AddTBInt(DictFlowAttr.IsShowListRight, 0, "是否显示在列表右边", true, false);

                map.AddTBInt(DictFlowAttr.Idx, 0, "Idx", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 移动.
        public void DoUp()
        {
            this.DoOrderUp(DictFlowAttr.FrmID, this.FrmID, DictFlowAttr.Idx);
        }
        public void DoDown()
        {
            this.DoOrderDown(DictFlowAttr.FrmID, this.FrmID, DictFlowAttr.Idx);
        }
        #endregion 移动.

    }
    /// <summary>
    /// 台账子流程
    /// </summary>
    public class DictFlows : EntitiesMyPK
    {
        /// <summary>
        /// 台账子流程
        /// </summary>
        public DictFlows() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DictFlow();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DictFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<DictFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DictFlow> Tolist()
        {
            System.Collections.Generic.List<DictFlow> list = new System.Collections.Generic.List<DictFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DictFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
