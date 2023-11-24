using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点用户组属性	  
    /// </summary>
    public class NodeTeamAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 用户组
        /// </summary>
        public const string FK_Team = "FK_Team";
    }
    /// <summary>
    /// 节点用户组
    /// 节点的用户组有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class NodeTeam : EntityMyPK
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
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeTeamAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(NodeTeamAttr.FK_Node, value);
            }
        }
        public string FK_TeamT
        {
            get
            {
                return this.GetValRefTextByKey(NodeTeamAttr.FK_Team);
            }
        }
        /// <summary>
        /// 用户组
        /// </summary>
        public string FK_Team
        {
            get
            {
                return this.GetValStringByKey(NodeTeamAttr.FK_Team);
            }
            set
            {
                this.SetValByKey(NodeTeamAttr.FK_Team, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点用户组
        /// </summary>
        public NodeTeam() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeTeam", "节点权限组");


                map.AddMyPK();

                map.AddTBInt(NodeTeamAttr.FK_Node, 0, "节点", false, false);
                map.AddDDLEntities(NodeTeamAttr.FK_Team, null, "用户组",
                   new BP.Port.Teams(), true);

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeUpdateInsertAction()
        {
            this.setMyPK(this.NodeID + "_" + this.FK_Team);
            return base.beforeUpdateInsertAction();
        }

        /// <summary>
        /// 节点权限组发生变化，删除该节点记忆的接收人员。
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            RememberMe remeberMe = new RememberMe();
            remeberMe.Delete(RememberMeAttr.FK_Node, this.NodeID);
            return base.beforeInsert();
        }
        #endregion

    }
    /// <summary>
    /// 节点用户组s
    /// </summary>
    public class NodeTeams : EntitiesMyPK
    {
        /// <summary>
        /// 节点用户组s
        /// </summary>
        public NodeTeams() { }
        /// <summary>
        /// 节点用户组s
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        public NodeTeams(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeTeamAttr.FK_Node, nodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 节点用户组
        /// </summary>
        /// <param name="StationNo">StationNo </param>
        public NodeTeams(string StationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeTeamAttr.FK_Team, StationNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeTeam();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeTeam> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeTeam>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NodeTeam> Tolist()
        {
            System.Collections.Generic.List<NodeTeam> list = new System.Collections.Generic.List<NodeTeam>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NodeTeam)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
