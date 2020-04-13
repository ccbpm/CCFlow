using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点群组属性	  
    /// </summary>
    public class NodeGroupAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 群组
        /// </summary>
        public const string FK_Group = "FK_Group";
    }
    /// <summary>
    /// 节点群组
    /// 节点的群组有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class NodeGroup : EntityMM
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
                uac.OpenAll();
                return uac;
            }
        }
        /// <summary>
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(NodeGroupAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(NodeGroupAttr.FK_Node, value);
            }
        }
        public string FK_GroupT
        {
            get
            {
                return this.GetValRefTextByKey(NodeGroupAttr.FK_Group);
            }
        }
        /// <summary>
        /// 群组
        /// </summary>
        public string FK_Group
        {
            get
            {
                return this.GetValStringByKey(NodeGroupAttr.FK_Group);
            }
            set
            {
                this.SetValByKey(NodeGroupAttr.FK_Group, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点群组
        /// </summary>
        public NodeGroup() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeGroup", "节点岗位");

                map.AddTBIntPK(NodeGroupAttr.FK_Node, 0, "节点", false, false);

                // #warning ,这里为了方便用户选择，让分组都统一采用了枚举类型. edit zhoupeng. 2015.04.28. 注意jflow也要修改.
                map.AddDDLEntitiesPK(NodeGroupAttr.FK_Group, null, "群组",
                   new BP.GPM.Groups(), true);

                this._enMap = map;
                return this._enMap;
            }
        }

        /// <summary>
        /// 节点岗位发生变化，删除该节点记忆的接收人员。
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            RememberMe remeberMe = new RememberMe();
            remeberMe.Delete(RememberMeAttr.FK_Node, this.FK_Node);
            return base.beforeInsert();
        }
        #endregion

    }
    /// <summary>
    /// 节点群组s
    /// </summary>
    public class NodeGroups : EntitiesMM
    {
        /// <summary>
        /// 节点群组s
        /// </summary>
        public NodeGroups() { }
        /// <summary>
        /// 节点群组s
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        public NodeGroups(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeGroupAttr.FK_Node, nodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 节点群组
        /// </summary>
        /// <param name="StationNo">StationNo </param>
        public NodeGroups(string StationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeGroupAttr.FK_Group, StationNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeGroup();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeGroup> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeGroup>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NodeGroup> Tolist()
        {
            System.Collections.Generic.List<NodeGroup> list = new System.Collections.Generic.List<NodeGroup>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NodeGroup)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
