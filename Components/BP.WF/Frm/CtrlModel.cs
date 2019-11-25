using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF.Template;
using BP.WF;

namespace BP.Frm
{
    /// <summary>
    ///  控制模型-属性
    /// </summary>
    public class CtrlModelAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 
        /// </summary>
        public const string CtrlObj = "CtrlObj";
        /// <summary>
        /// 所有的人
        /// </summary>
        public const string IsEnableAll = "IsEnableAll";
        public const string IsEnableStation = "IsEnableStation";
        public const string IsEnableDept = "IsEnableDept";
        public const string IsEnableUser = "IsEnableUser";

        public const string IDOfUsers = "IDOfUsers";
        public const string IDOfStations = "IDOfStations";
        public const string IDOfDepts = "IDOfDepts";
    }
    /// <summary>
    /// 控制模型
    /// </summary>
    public class CtrlModel : BP.En.EntityMyPK
    {
        #region 基本属性.
        /// <summary>
        /// 表单数据
        /// </summary>
        public string FrmDB = null;
        public string FK_Flow = null;
        #endregion 基本属性.

        #region 字段属性.
        #endregion attrs

        #region 构造.
        public string RptName = null;
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_CtrlModel", "控制模型表");

                #region 字段
                map.AddMyPK();  //增加一个自动增长的列.

                //map.AddTBInt(CtrlModelAttr.ActionType, 0, "类型", true, false);
                //map.AddTBString(CtrlModelAttr.ActionTypeText, null, "类型(名称)", true, false, 0, 30, 100);
                //map.AddTBInt(CtrlModelAttr.FID, 0, "流程ID", true, false);
                //map.AddTBInt(CtrlModelAttr.WorkID, 0, "工作ID", true, false);

                //map.AddTBInt(CtrlModelAttr.NDFrom, 0, "从节点", true, false);
                //map.AddTBString(CtrlModelAttr.NDFromT, null, "从节点(名称)", true, false, 0, 300, 100);

                //map.AddTBInt(CtrlModelAttr.NDTo, 0, "到节点", true, false);
                //map.AddTBString(CtrlModelAttr.NDToT, null, "到节点(名称)", true, false, 0, 999, 900);

                //map.AddTBString(CtrlModelAttr.EmpFrom, null, "从人员", true, false, 0, 20, 100);
                //map.AddTBString(CtrlModelAttr.EmpFromT, null, "从人员(名称)", true, false, 0, 30, 100);

                //map.AddTBString(CtrlModelAttr.EmpTo, null, "到人员", true, false, 0, 2000, 100);
                //map.AddTBString(CtrlModelAttr.EmpToT, null, "到人员(名称)", true, false, 0, 2000, 100);

                //map.AddTBString(CtrlModelAttr.RDT, null, "日期", true, false, 0, 20, 100);
                //map.AddTBFloat(CtrlModelAttr.WorkTimeSpan, 0, "时间跨度(天)", true, false);
                //map.AddTBStringDoc(CtrlModelAttr.Msg, null, "消息", true, false);
                //map.AddTBStringDoc(CtrlModelAttr.NodeData, null, "节点数据(日志信息)", true, false);
                //map.AddTBString(CtrlModelAttr.Tag, null, "参数", true, false, 0, 300, 3000);
                //map.AddTBString(CtrlModelAttr.Exer, null, "执行人", true, false, 0, 200, 100);

                #endregion 字段

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 控制模型
        /// </summary>
        public CtrlModel()
        {
        }
        /// <summary>
        /// 增加授权人
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
        #endregion 构造.
    }
    /// <summary>
    /// 控制模型集合s
    /// </summary>
    public class CtrlModels : BP.En.EntitiesMyPK
    {
        #region 构造方法.
        /// <summary>
        /// 控制模型集合
        /// </summary>
        public CtrlModels()
        {
        }
        public override Entity GetNewEntity
        {
            get
            {
                return new CtrlModel();
            }
        }
        #endregion 构造方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CtrlModel> ToJavaList()
        {
            return (System.Collections.Generic.IList<CtrlModel>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CtrlModel> Tolist()
        {
            System.Collections.Generic.List<CtrlModel> list = new System.Collections.Generic.List<CtrlModel>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CtrlModel)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}