﻿using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using System.Collections;
using BP.Port;
using BP.WF.Data;
using BP.WF.Template;
using BP.WF.Port;
using System.Collections.Generic;

namespace BP.WF.Template
{
    /// <summary>
    /// 这里存放每个节点的信息.	 
    /// </summary>
    public class NodeSimple : Entity
    {
        #region 节点属性.
        /// <summary>
        /// 节点编号
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        public DeliveryWay HisDeliveryWay
        {
            get
            {
                return (DeliveryWay)this.GetValIntByKey(NodeAttr.DeliveryWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryWay, value);
            }
        }
        public string DeliveryParas
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DeliveryParas);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.Name);
            }
            set
            {
                this.SetValByKey(NodeAttr.Name, value);
            }
        }
        public float X
        {
            get
            {
                return this.GetValFloatByKey(NodeAttr.X);
            }
            set
            {
                this.SetValByKey(NodeAttr.X, value);
            }
        }
        /// <summary>
        /// y
        /// </summary>
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(NodeAttr.Y);
            }
            set
            {
                this.SetValByKey(NodeAttr.Y, value);
            }
        }

        public string HisToNDs
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.HisToNDs);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisToNDs, value);
            }
        }
        public bool IsResetAccepter
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsResetAccepter);
            }
        }
        #endregion 节点属性.

        #region 构造函数
        /// <summary>
        /// 节点
        /// </summary>
        public NodeSimple() { }
        /// <summary>
        /// 节点
        /// </summary>
        /// <param name="_oid">节点ID</param>	
        public NodeSimple(int nodeID)
        {
            this.NodeID = nodeID;
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

                Map map = new Map("WF_Node", "节点");

                map.DepositaryOfEntity= Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                #region 基本属性.
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "名称", true, false, 0, 150, 10);
                map.AddTBString(NodeAttr.FK_Flow, null, "流程编号", true, false, 0, 5, 10);
                map.AddTBInt(NodeAttr.RunModel, 0, "运行模式", true, true);

                map.AddTBInt(NodeAttr.DeliveryWay, 0, "运行模式", true, true);
                map.AddTBString(NodeAttr.DeliveryParas, null, "参数", true, false, 0, 300, 10);
                map.AddTBString(NodeAttr.HisToNDs, null, "转到的节点", false, false, 0, 50, 10);
                map.AddBoolean(NodeAttr.IsResetAccepter, false, "可逆节点时重新计算接收人?", true, true, true);
                map.AddTBInt(NodeAttr.NodeType, 0, "节点类型", true, true);
                map.AddTBInt(NodeAttr.CondModel, 0, "方向条件控制规则", true, true);
                map.AddTBInt(NodeAttr.Step, 0, "步骤", true, true);
                #endregion 基本属性.

                map.AddTBInt(NodeAttr.X, 0, "X坐标", false, false);
                map.AddTBInt(NodeAttr.Y, 0, "Y坐标", false, false);

                map.AddTBAtParas(500);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 节点集合
    /// </summary>
    public class NodeSimples : Entities
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeSimple();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点集合
        /// </summary>
        public NodeSimples()
        {
        }
        /// <summary>
        /// 节点集合.
        /// </summary>
        /// <param name="FlowNo"></param>
        public NodeSimples(string fk_flow)
        {
            this.Retrieve(NodeAttr.FK_Flow, fk_flow, NodeAttr.Step);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeSimple> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeSimple>)this;
        }
        /// <summary>
        /// 转化成list 为了翻译成java的需要
        /// </summary>
        /// <returns>List</returns>
        public List<BP.WF.Template.NodeSimple> Tolist()
        {
            List<BP.WF.Template.NodeSimple> list = new List<BP.WF.Template.NodeSimple>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BP.WF.Template.NodeSimple)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
