using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 延续子流程属性
    /// </summary>
    public class NodeYGFlowAttr : BP.En.EntityOIDNameAttr
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
        #endregion
    }
    /// <summary>
    /// 延续子流程.	 
    /// </summary>
    public class NodeYGFlow : EntityOID
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
                return this.GetValStringByKey(NodeYGFlowAttr.FK_Flow);
            }
            set
            {
                SetValByKey(NodeYGFlowAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValRefTextByKey(NodeYGFlowAttr.FK_Flow);
            }
        }
        /// <summary>
        /// 条件表达式.
        /// </summary>
        public string CondExp
        {
            get
            {
                return this.GetValStringByKey(NodeYGFlowAttr.CondExp);
            }
            set
            {
                SetValByKey(NodeYGFlowAttr.CondExp, value);
            }
        }
        /// <summary>
        /// 表达式类型
        /// </summary>
        public ConnDataFrom ExpType
        {
            get
            {
                return (ConnDataFrom)this.GetValIntByKey(NodeYGFlowAttr.ExpType);
            }
            set
            {
                SetValByKey(NodeYGFlowAttr.ExpType, (int)value);
            }
        }
        public string FK_Node
        {
            get
            {
                return this.GetValStringByKey(NodeYGFlowAttr.FK_Node);
            }
            set
            {
                SetValByKey(NodeYGFlowAttr.FK_Node, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 延续子流程
        /// </summary>
        public NodeYGFlow() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeSubFlow", "延续子流程");

                map.AddTBIntPKOID();                 
                map.AddTBInt(NodeYGFlowAttr.FK_Node, 0, "节点", false, true);

                map.AddDDLEntities(NodeYGFlowAttr.FK_Flow, null, "延续子流程", new Flows(), false);

                //map.AddDDLSysEnum(NodeYGFlowAttr.YGWorkWay, 1, "工作方式", true, true, NodeYGFlowAttr.YGWorkWay,
                //    "@0=停止当前节点等待延续子流程运行完毕后该节点自动向下运行@1=启动延续子流程运行到下一步骤上去");

                map.AddTBInt(NodeYGFlowAttr.Idx, 0, "显示顺序", true, false);
                map.AddDDLSysEnum(NodeYGFlowAttr.ExpType, 3, "表达式类型", true, true, NodeYGFlowAttr.ExpType,
                   "@3=按照SQL计算@4=按照参数计算");

                map.AddTBString(NodeYGFlowAttr.CondExp, null, "条件表达式", true, false, 0, 500, 150, true);

                //@du.
                map.AddDDLSysEnum(NodeYGFlowAttr.YBFlowReturnRole, 0, "退回方式", true, true, NodeYGFlowAttr.YBFlowReturnRole,
                  "@0=不能退回@1=退回到父流程的开始节点@2=退回到父流程的任何节点@3=退回父流程的启动节点@4=可退回到指定的节点");

               // map.AddTBString(NodeYGFlowAttr.ReturnToNode, null, "要退回的节点", true, false, 0, 200, 150, true);
                map.AddDDLSQL(NodeYGFlowAttr.ReturnToNode, "0", "要退回的节点",
                    "SELECT NodeID AS No, Name FROM WF_Node WHERE FK_Flow IN (SELECT FK_Flow FROM WF_Node WHERE NodeID=@FK_Node; )",true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 延续子流程集合
    /// </summary>
    public class NodeYGFlows : EntitiesOID
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeYGFlow();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 延续子流程集合
        /// </summary>
        public NodeYGFlows()
        {
        }
        /// <summary>
        /// 延续子流程集合.
        /// </summary>
        /// <param name="fk_node"></param>
        public NodeYGFlows(int fk_node)
        {
            this.Retrieve(NodeYGFlowAttr.FK_Node, fk_node);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List   
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeYGFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeYGFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NodeYGFlow> Tolist()
        {
            System.Collections.Generic.List<NodeYGFlow> list = new System.Collections.Generic.List<NodeYGFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NodeYGFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
