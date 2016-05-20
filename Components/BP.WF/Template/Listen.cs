using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
	/// <summary>
	/// 消息收听属性
	/// </summary>
    public class ListenAttr
    {
        public const string Doc = "Doc";
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 收听节点
        /// </summary>
        public const string Nodes = "Nodes";
        /// <summary>
        /// 描述
        /// </summary>
        public const string NodesDesc = "NodesDesc";
        /// <summary>
        /// 收听方式
        /// </summary>
        public const string AlertWay = "AlertWay";
        /// <summary>
        /// Title
        /// </summary>
        public const string Title = "Title";
    }
	/// <summary>
	/// 消息收听
	/// 节点的收听节点有两部分组成.	 
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
    public class Listen : EntityOID
    {
        #region 基本属性
        //public BP.WF.Port.AlertWay HisAlertWay
        //{
        //    get
        //    {
        //        return (BP.WF.Port.AlertWay)this.GetValIntByKey(ListenAttr.AlertWay);
        //    }
        //    set
        //    {
        //        this.SetValByKey(ListenAttr.AlertWay, (int)value);
        //    }
        //}
      
        /// <summary>
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(ListenAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(ListenAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 收听节点
        /// </summary>
        public string Nodes
        {
            get
            {
                return this.GetValStringByKey(ListenAttr.Nodes);
            }
            set
            {
                this.SetValByKey(ListenAttr.Nodes, value);
            }
        }
        public string NodesDesc
        {
            get
            {
                return this.GetValStringByKey(ListenAttr.NodesDesc);
            }
            set
            {
                this.SetValByKey(ListenAttr.NodesDesc, value);
            }
        }
        public string Doc
        {
            get
            {
                string s= this.GetValStringByKey(ListenAttr.Doc);
                if (string.IsNullOrEmpty(s) == true)
                    s = "";
                return s;
            }
            set
            {
                this.SetValByKey(ListenAttr.Doc, value);
            }
        }
        public string Title
        {
            get
            {
                return this.GetValStringByKey(ListenAttr.Title);
            }
            set
            {
                this.SetValByKey(ListenAttr.Title, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 消息收听
        /// </summary>
        public Listen()
        {
        }
        public Listen(int oid)
        {
            this.OID = oid;
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

                Map map = new Map("WF_Listen", "消息收听");

                map.AddTBIntPKOID();
                map.AddTBInt(ListenAttr.FK_Node, 0, "节点", true, false);
                map.AddTBString(ListenAttr.Nodes, null, "Nodes", true, false, 0, 400, 10);
                map.AddTBString(ListenAttr.NodesDesc, null, "描述", true, false, 0, 400, 10);
                map.AddTBString(ListenAttr.Title, null, "Title", true, false, 0, 400, 10);
                map.AddTBStringDoc();
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        
    }
	/// <summary>
	/// 消息收听
	/// </summary>
    public class Listens : EntitiesOID
    {
        #region 构造.
        /// <summary>
        /// 消息收听
        /// </summary>
        public Listens() { }
        /// <summary>
        /// 消息收听
        /// </summary>
        /// <param name="fk_flow"></param>
        public Listens(string fk_flow)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(ListenAttr.FK_Node, "SELECT NodeID FROM WF_Node WHERE FK_Flow='" + fk_flow + "'");
            qo.DoQuery();
        }
        /// <summary>
        /// 消息收听
        /// </summary>
        /// <param name="nodeID"></param>
        public Listens(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(ListenAttr.FK_Node, nodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Listen();
            }
        }
        #endregion 构造.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Listen> ToJavaList()
        {
            return (System.Collections.Generic.IList<Listen>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Listen> Tolist()
        {
            System.Collections.Generic.List<Listen> list = new System.Collections.Generic.List<Listen>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Listen)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
