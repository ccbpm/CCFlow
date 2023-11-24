using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点工作角色属性	  
    /// </summary>
    public class NodeStationAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 工作角色
        /// </summary>
        public const string FK_Station = "FK_Station";
    }
    /// <summary>
    /// 节点工作角色
    /// </summary>
    public class NodeStation : EntityMyPK
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
                return this.GetValIntByKey(NodeStationAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(NodeStationAttr.FK_Node, value);
            }
        }
        public string StationT
        {
            get
            {
                return this.GetValRefTextByKey(NodeStationAttr.FK_Station);
            }
        }
        /// <summary>
        /// 工作角色
        /// </summary>
        public string StationNo
        {
            get
            {
                return this.GetValStringByKey(NodeStationAttr.FK_Station);
            }
            set
            {
                this.SetValByKey(NodeStationAttr.FK_Station, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点工作角色
        /// </summary>
        public NodeStation() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeStation", "节点角色");

                map.AddMyPK();

                map.AddTBInt(NodeStationAttr.FK_Node, 0, "节点", false, false);
                map.AddDDLEntities(NodeStationAttr.FK_Station, null, "工作角色",
                   new BP.Port.Stations(), true);

                //map.AddTBIntPK(NodeStationAttr.FK_Node, 0, "节点", false, false);
                //// #warning ,这里为了方便用户选择，让分组都统一采用了枚举类型. edit zhoupeng. 2015.04.28. 注意jflow也要修改.
                //map.AddDDLEntitiesPK(NodeStationAttr.FK_Station, null, "工作角色",
                //   new BP.Port.Stations(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        protected override bool beforeUpdateInsertAction()
        {
            this.setMyPK(this.NodeID + "_" + this.StationNo);
            return base.beforeUpdateInsertAction();
        }

        /// <summary>
        /// 节点角色发生变化，删除该节点记忆的接收人员。
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
    /// 节点工作角色s
    /// </summary>
    public class NodeStations : EntitiesMyPK
    {
        #region 构造函数.
        /// <summary>
        /// 节点工作角色
        /// </summary>
        public NodeStations() { }
        /// <summary>
        /// 节点工作角色
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        public NodeStations(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeStationAttr.FK_Node, nodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeStation();
            }
        }
        #endregion 构造函数.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeStation> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeStation>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NodeStation> Tolist()
        {
            System.Collections.Generic.List<NodeStation> list = new System.Collections.Generic.List<NodeStation>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NodeStation)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
