using BP.DA;
using BP.En;
using BP.Port;


namespace BP.WF.Template.CCEn
{
    /// <summary>
    /// 抄送到角色 属性	  
    /// </summary>
    public class CCStationAttr
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
    /// 抄送到角色
    /// 节点的工作角色有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class CCStation : EntityMM
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
                return this.GetValIntByKey(CCStationAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(CCStationAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string FK_StationT
        {
            get
            {
                return this.GetValRefTextByKey(CCStationAttr.FK_Station);
            }
        }
        /// <summary>
        /// 工作角色
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(CCStationAttr.FK_Station);
            }
            set
            {
                this.SetValByKey(CCStationAttr.FK_Station, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 抄送到角色
        /// </summary>
        public CCStation() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_CCStation", "抄送角色");

                map.AddDDLEntitiesPK(CCStationAttr.FK_Node, 0, DataType.AppInt, "节点", new Nodes(), NodeAttr.NodeID, NodeAttr.Name, true);
                map.AddDDLEntitiesPK(CCStationAttr.FK_Station, null, "工作角色", new Stations(), true);
              
                this._enMap = map;

                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 抄送到角色
    /// </summary>
    public class CCStations : EntitiesMM
    {
        /// <summary>
        /// 他的工作角色
        /// </summary>
        public Stations HisStations
        {
            get
            {
                Stations ens = new Stations();
                foreach (CCStation ns in this)
                {
                    ens.AddEntity(new Station(ns.FK_Station));
                }
                return ens;
            }
        }
        /// <summary>
        /// 他的工作节点
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (CCStation ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Node));
                }
                return ens;

            }
        }
        /// <summary>
        /// 抄送到角色
        /// </summary>
        public CCStations() { }
        /// <summary>
        /// 抄送到角色
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        public CCStations(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCStationAttr.FK_Node, nodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 抄送到角色
        /// </summary>
        /// <param name="StationNo">StationNo </param>
        public CCStations(string StationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCStationAttr.FK_Station, StationNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CCStation();
            }
        }
        /// <summary>
        /// 工作角色对应的节点
        /// </summary>
        /// <param name="stationNo">工作角色编号</param>
        /// <returns>节点s</returns>
        public Nodes GetHisNodes(string stationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCStationAttr.FK_Station, stationNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (CCStation en in this)
            {
                ens.AddEntity(new Node(en.FK_Node));
            }
            return ens;
        }
        /// <summary>
        /// 转向此节点的集合的Nodes
        /// </summary>
        /// <param name="nodeID">此节点的ID</param>
        /// <returns>转向此节点的集合的Nodes (FromNodes)</returns> 
        public Stations GetHisStations(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCStationAttr.FK_Node, nodeID);
            qo.DoQuery();

            Stations ens = new Stations();
            foreach (CCStation en in this)
            {
                ens.AddEntity(new Station(en.FK_Station));
            }
            return ens;
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CCStation> ToJavaList()
        {
            return (System.Collections.Generic.IList<CCStation>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CCStation> Tolist()
        {
            System.Collections.Generic.List<CCStation> list = new System.Collections.Generic.List<CCStation>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CCStation)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
