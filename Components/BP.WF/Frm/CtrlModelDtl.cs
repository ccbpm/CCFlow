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
    public class CtrlModelDtlAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 控制类型
        /// </summary>
        public const string CtrlObj = "CtrlObj";
        /// <summary>
        /// 组织类型 Station,Emp,Dept
        /// </summary>
        public const string OrgType = "OrgType";
        /// <summary>
        /// IDs
        /// </summary>
        public const string IDs = "IDs";
    }
    /// <summary>
    /// 控制模型
    /// </summary>
    public class CtrlModelDtl : BP.En.EntityMyPK
    {
        #region 基本属性.
        #endregion 基本属性.

        #region 构造.
        public string RptName = null;
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_CtrlModelDtl", "控制模型表Dtl");

                #region 字段
                map.AddMyPK();  //增加一个自动增长的列.

                //map.AddTBInt(CtrlModelDtlAttr.ActionType, 0, "类型", true, false);
                //map.AddTBString(CtrlModelDtlAttr.ActionTypeText, null, "类型(名称)", true, false, 0, 30, 100);
                //map.AddTBInt(CtrlModelDtlAttr.FID, 0, "流程ID", true, false);
                //map.AddTBInt(CtrlModelDtlAttr.WorkID, 0, "工作ID", true, false);

                //map.AddTBInt(CtrlModelDtlAttr.NDFrom, 0, "从节点", true, false);
                //map.AddTBString(CtrlModelDtlAttr.NDFromT, null, "从节点(名称)", true, false, 0, 300, 100);

                //map.AddTBInt(CtrlModelDtlAttr.NDTo, 0, "到节点", true, false);
                //map.AddTBString(CtrlModelDtlAttr.NDToT, null, "到节点(名称)", true, false, 0, 999, 900);

                //map.AddTBString(CtrlModelDtlAttr.EmpFrom, null, "从人员", true, false, 0, 20, 100);
                //map.AddTBString(CtrlModelDtlAttr.EmpFromT, null, "从人员(名称)", true, false, 0, 30, 100);

                //map.AddTBString(CtrlModelDtlAttr.EmpTo, null, "到人员", true, false, 0, 2000, 100);
                //map.AddTBString(CtrlModelDtlAttr.EmpToT, null, "到人员(名称)", true, false, 0, 2000, 100);

                //map.AddTBString(CtrlModelDtlAttr.RDT, null, "日期", true, false, 0, 20, 100);
                //map.AddTBFloat(CtrlModelDtlAttr.WorkTimeSpan, 0, "时间跨度(天)", true, false);
                //map.AddTBStringDoc(CtrlModelDtlAttr.Msg, null, "消息", true, false);
                //map.AddTBStringDoc(CtrlModelDtlAttr.NodeData, null, "节点数据(日志信息)", true, false);
                //map.AddTBString(CtrlModelDtlAttr.Tag, null, "参数", true, false, 0, 300, 3000);
                //map.AddTBString(CtrlModelDtlAttr.Exer, null, "执行人", true, false, 0, 200, 100);
                //#endregion 字段.

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 控制模型
        /// </summary>
        public CtrlModelDtl()
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
    public class CtrlModelDtls : BP.En.EntitiesMyPK
    {
        #region 构造方法.
        /// <summary>
        /// 控制模型集合
        /// </summary>
        public CtrlModelDtls()
        {
        }
        public override Entity GetNewEntity
        {
            get
            {
                return new CtrlModelDtl();
            }
        }
        #endregion 构造方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CtrlModelDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<CtrlModelDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CtrlModelDtl> Tolist()
        {
            System.Collections.Generic.List<CtrlModelDtl> list = new System.Collections.Generic.List<CtrlModelDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CtrlModelDtl)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}