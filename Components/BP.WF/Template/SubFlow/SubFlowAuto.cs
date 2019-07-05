using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 自动触发子流程属性
    /// </summary>
    public class SubFlowAutoAttr : BP.En.EntityOIDNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 流程名称
        /// </summary>
        public const string FlowName = "FlowName";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 显示在那里？
        /// </summary>
        public const string YGWorkWay = "YGWorkWay";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 表达式类型
        /// </summary>
        public const string ExpType = "ExpType";
        /// <summary>
        /// 条件表达式
        /// </summary>
        public const string CondExp = "CondExp";
        /// <summary>
        /// 越轨子流程退回类型
        /// </summary>
        public const string YBFlowReturnRole = "YBFlowReturnRole";
        /// <summary>
        /// 要退回的节点
        /// </summary>
        public const string ReturnToNode = "ReturnToNode";
        /// <summary>
        /// 子流程类型
        /// </summary>
        public const string SubFlowType = "SubFlowType";
        /// <summary>
        /// 子流程模式
        /// </summary>
        public const string SubFlowModel = "SubFlowModel";
        #endregion
    }
    /// <summary>
    /// 自动触发子流程.
    /// </summary>
    public class SubFlowAuto : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.FK_Flow);
            }
            set
            {
                SetValByKey(SubFlowAutoAttr.FK_Flow, value);
            }
        }   
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.FK_Flow);
            }
        }
        /// <summary>
        /// 条件表达式.
        /// </summary>
        public string CondExp
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.CondExp);
            }
            set
            {
                SetValByKey(SubFlowAutoAttr.CondExp, value);
            }
        }
        /// <summary>
        /// 表达式类型
        /// </summary>
        public ConnDataFrom ExpType
        {
            get
            {
                return (ConnDataFrom)this.GetValIntByKey(SubFlowAutoAttr.ExpType);
            }
            set
            {
                SetValByKey(SubFlowAutoAttr.ExpType, (int)value);
            }
        }
        public string FK_Node
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.FK_Node);
            }
            set
            {
                SetValByKey(SubFlowAutoAttr.FK_Node, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 自动触发子流程
        /// </summary>
        public SubFlowAuto() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeSubFlow", "自动触发子流程");

                map.AddMyPK();

                map.AddTBInt(SubFlowAutoAttr.FK_Node, 0, "节点", false, true);

                map.AddDDLSysEnum(SubFlowAutoAttr.SubFlowType, 2, "子流程类型", true, false, SubFlowAutoAttr.SubFlowType,
                "@0=手动启动子流程@1=触发启动子流程@2=自动触发子流程");

                map.AddDDLSysEnum(SubFlowAutoAttr.SubFlowModel, 0, "子流程模式", true, true, SubFlowAutoAttr.SubFlowModel,
                "@0=下级子流程@1=同级子流程");
                

                map.AddTBString(SubFlowAutoAttr.FK_Flow, null, "子流程编号", true, true, 0, 10, 150, false);
                map.AddTBString(SubFlowAutoAttr.FlowName, null, "子流程名称", true, true, 0, 200, 150, false);

                map.AddDDLSysEnum(SubFlowAutoAttr.ExpType, 3, "表达式类型", true, true, SubFlowAutoAttr.ExpType,
                   "@3=按照SQL计算@4=按照参数计算");

                map.AddTBString(SubFlowAutoAttr.CondExp, null, "条件表达式", true, false, 0, 500, 150, true);

                //@du.
                map.AddDDLSysEnum(SubFlowAutoAttr.YBFlowReturnRole, 0, "退回方式", true, true, SubFlowAutoAttr.YBFlowReturnRole,
                  "@0=不能退回@1=退回到父流程的开始节点@2=退回到父流程的任何节点@3=退回父流程的启动节点@4=可退回到指定的节点");

                // map.AddTBString(SubFlowAutoAttr.ReturnToNode, null, "要退回的节点", true, false, 0, 200, 150, true);
                map.AddDDLSQL(SubFlowAutoAttr.ReturnToNode, "0", "要退回的节点",
                    "SELECT NodeID AS No, Name FROM WF_Node WHERE FK_Flow IN (SELECT FK_Flow FROM WF_Node WHERE NodeID=@FK_Node; )", true);

                map.AddTBInt(SubFlowAutoAttr.Idx, 0, "显示顺序", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 设置主键
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            this.MyPK = this.FK_Node + "_" + this.FK_Flow + "_1";
            return base.beforeInsert();
        }

        #region 移动.
        /// <summary>
        /// 上移
        /// </summary>
        /// <returns></returns>
        public string DoUp()
        {
            this.DoOrderUp(SubFlowAutoAttr.FK_Node, this.FK_Node, SubFlowAutoAttr.SubFlowType, "2", SubFlowAutoAttr.Idx);
            return "执行成功";
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <returns></returns>
        public string DoDown()
        {
            this.DoOrderDown(SubFlowAutoAttr.FK_Node, this.FK_Node, SubFlowAutoAttr.SubFlowType, "2", SubFlowAutoAttr.Idx);
            return "执行成功";
        }
        #endregion 移动.

    }
    /// <summary>
    /// 自动触发子流程集合
    /// </summary>
    public class SubFlowAutos : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SubFlowAuto();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 自动触发子流程集合
        /// </summary>
        public SubFlowAutos()
        {
        }
        /// <summary>
        /// 自动触发子流程集合.
        /// </summary>
        /// <param name="fk_node"></param>
        public SubFlowAutos(int fk_node,int subFlowType)
        {
            this.Retrieve(SubFlowAutoAttr.FK_Node, fk_node, SubFlowAutoAttr.SubFlowType, subFlowType);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SubFlowAuto> ToJavaList()
        {
            return (System.Collections.Generic.IList<SubFlowAuto>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SubFlowAuto> Tolist()
        {
            System.Collections.Generic.List<SubFlowAuto> list = new System.Collections.Generic.List<SubFlowAuto>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SubFlowAuto)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
