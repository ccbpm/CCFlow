using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF.Port;

namespace BP.Demo
{
	/// <summary>
	/// 制度章节属性
	/// </summary>
    public class NodeDtlAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 章节编号
        /// </summary>
        public const string FK_ZhiDuDtl = "FK_ZhiDuDtl";
    }
	/// <summary>
	/// 制度章节
	/// 节点的章节编号有两部分组成.	 
	/// 记录了从一个节点调用其他的多个节点.
	/// 也记录了调用这个节点的其他的节点.
	/// </summary>
    public class NodeDtl : EntityMM
    {
        #region 基本属性
        /// <summary>
        ///节点
        /// </summary>
        public string FK_Node
        {
            get
            {
                return this.GetValStringByKey(NodeDtlAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(NodeDtlAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 章节编号
        /// </summary>
        public string FK_ZhiDuDtl
        {
            get
            {
                return this.GetValStringByKey(NodeDtlAttr.FK_ZhiDuDtl);
            }
            set
            {
                this.SetValByKey(NodeDtlAttr.FK_ZhiDuDtl, value);
            }
        }
        public string FK_ZhiDuDtlT
        {
            get
            {
                return this.GetValRefTextByKey(NodeDtlAttr.FK_ZhiDuDtl);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 制度章节
        /// </summary>
        public NodeDtl() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Demo_NodeDtl", "制度章节");
                 

                map.AddTBIntPK(NodeDtlAttr.FK_Node, 0, "节点", true, true);
                map.AddTBStringPK(NodeDtlAttr.FK_ZhiDuDtl, null, "节点", true, true,1,100,100);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 制度章节
	/// </summary>
    public class NodeDtls : EntitiesMM
    {
        /// <summary>
        /// 制度章节
        /// </summary>
        public NodeDtls() { }
        /// <summary>
        /// 制度章节
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public NodeDtls(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeDtlAttr.FK_Node, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 得调用它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeDtl();
            }
        }
    }
}
