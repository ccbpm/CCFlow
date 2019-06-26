using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 手工启动子流程属性
    /// </summary>
    public class SubFlowHandAttr : BP.En.EntityOIDNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string FK_Flow = "FK_Flow";
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
        #endregion
    }
    /// <summary>
    /// 手工启动子流程.
    /// </summary>
    public class SubFlowHand : EntityOID
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
                uac.IsInsert=false;
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
                return this.GetValStringByKey(SubFlowHandAttr.FK_Flow);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValRefTextByKey(SubFlowHandAttr.FK_Flow);
            }
        }
        /// <summary>
        /// 条件表达式.
        /// </summary>
        public string CondExp
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandAttr.CondExp);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.CondExp, value);
            }
        }
        /// <summary>
        /// 表达式类型
        /// </summary>
        public ConnDataFrom ExpType
        {
            get
            {
                return (ConnDataFrom)this.GetValIntByKey(SubFlowHandAttr.ExpType);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.ExpType, (int)value);
            }
        }
        public string FK_Node
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandAttr.FK_Node);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.FK_Node, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 手工启动子流程
        /// </summary>
        public SubFlowHand() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeSubFlow", "手动启动子流程");

                map.AddTBIntPKOID();                 
                map.AddTBInt(SubFlowHandAttr.FK_Node, 0, "节点", false, true);

                map.AddDDLSysEnum(SubFlowHandAttr.SubFlowType, 0, "子流程类型", true, false, SubFlowHandAttr.SubFlowType,
                "@0=手动启动子流程@1=触发启动子流程@2=延续子流程");

                map.AddTBString(SubFlowYanXuAttr.FK_Flow, null, "子流程编号", true, false, 0, 10, 150, true);
                map.AddTBString(SubFlowYanXuAttr.FlowName, null, "子流程名称", true, false, 0, 200, 150, true);

                //map.AddDDLSysEnum(SubFlowHandAttr.YGWorkWay, 1, "工作方式", true, true, SubFlowHandAttr.YGWorkWay,
                //"@0=停止当前节点等待手工启动子流程运行完毕后该节点自动向下运行@1=启动手工启动子流程运行到下一步骤上去");

                map.AddDDLSysEnum(SubFlowHandAttr.ExpType, 3, "表达式类型", true, true, SubFlowHandAttr.ExpType,
                   "@3=按照SQL计算@4=按照参数计算");

                map.AddTBString(SubFlowHandAttr.CondExp, null, "条件表达式", true, false, 0, 500, 150, true);

                //@du.
                map.AddDDLSysEnum(SubFlowHandAttr.YBFlowReturnRole, 0, "退回方式", true, true, SubFlowHandAttr.YBFlowReturnRole,
                  "@0=不能退回@1=退回到父流程的开始节点@2=退回到父流程的任何节点@3=退回父流程的启动节点@4=可退回到指定的节点");

               // map.AddTBString(SubFlowHandAttr.ReturnToNode, null, "要退回的节点", true, false, 0, 200, 150, true);
                map.AddDDLSQL(SubFlowHandAttr.ReturnToNode, "0", "要退回的节点",
                    "SELECT NodeID AS No, Name FROM WF_Node WHERE FK_Flow IN (SELECT FK_Flow FROM WF_Node WHERE NodeID=@FK_Node; )",true);

                map.AddTBInt(SubFlowHandAttr.Idx, 0, "显示顺序", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 手工启动子流程集合
    /// </summary>
    public class SubFlowHands : EntitiesOID
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SubFlowHand();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 手工启动子流程集合
        /// </summary>
        public SubFlowHands()
        {
        }
        /// <summary>
        /// 手工启动子流程集合.
        /// </summary>
        /// <param name="fk_node"></param>
        public SubFlowHands(int fk_node)
        {
            this.Retrieve(SubFlowHandAttr.FK_Node, fk_node);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List   
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SubFlowHand> ToJavaList()
        {
            return (System.Collections.Generic.IList<SubFlowHand>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SubFlowHand> Tolist()
        {
            System.Collections.Generic.List<SubFlowHand> list = new System.Collections.Generic.List<SubFlowHand>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SubFlowHand)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
