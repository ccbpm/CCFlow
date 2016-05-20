using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程岗位属性属性	  
    /// </summary>
    public class FlowEmpAttr
    {
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
    }
    /// <summary>
    /// 流程岗位属性
    /// 流程的人员有两部分组成.	 
    /// 记录了从一个流程到其他的多个流程.
    /// 也记录了到这个流程的其他的流程.
    /// </summary>
    public class FlowEmp : EntityMM
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
                return uac;
            }
        }
        /// <summary>
        ///流程
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FlowEmpAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FlowEmpAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(FlowEmpAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(FlowEmpAttr.FK_Emp, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 流程岗位属性
        /// </summary>
        public FlowEmp() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_FlowEmp", "流程岗位属性信息");
                 

                map.AddDDLEntitiesPK(FlowEmpAttr.FK_Flow, null, "FK_Flow", new Flows(), true);
                map.AddDDLEntitiesPK(FlowEmpAttr.FK_Emp, null, "人员", new Emps(), true);
                this._enMap = map;

                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 流程岗位属性
    /// </summary>
    public class FlowEmps : EntitiesMM
    {
        /// <summary>
        /// 他的人员
        /// </summary>
        public Stations HisStations
        {
            get
            {
                Stations ens = new Stations();
                foreach (FlowEmp ns in this)
                {
                    ens.AddEntity(new Station(ns.FK_Emp));
                }
                return ens;
            }
        }
        /// <summary>
        /// 他的工作流程
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (FlowEmp ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Flow));
                }
                return ens;

            }
        }
        /// <summary>
        /// 流程岗位属性
        /// </summary>
        public FlowEmps() { }
        /// <summary>
        /// 流程岗位属性
        /// </summary>
        /// <param name="NodeID">流程ID</param>
        public FlowEmps(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowEmpAttr.FK_Flow, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 流程岗位属性
        /// </summary>
        /// <param name="StationNo">StationNo </param>
        public FlowEmps(string StationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowEmpAttr.FK_Emp, StationNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowEmp();
            }
        }
        /// <summary>
        /// 取到一个人员集合能够访问到的流程s
        /// </summary>
        /// <param name="sts">人员集合</param>
        /// <returns></returns>
        public Nodes GetHisNodes(Stations sts)
        {
            Nodes nds = new Nodes();
            Nodes tmp = new Nodes();
            foreach (Station st in sts)
            {
                tmp = this.GetHisNodes(st.No);
                foreach (Node nd in tmp)
                {
                    if (nds.Contains(nd))
                        continue;
                    nds.AddEntity(nd);
                }
            }
            return nds;
        }
        /// <summary>
        /// 取到一个工作人员能够访问到的流程。
        /// </summary>
        /// <param name="empId">工作人员ID</param>
        /// <returns></returns>
        public Nodes GetHisNodes_del(string empId)
        {
            Emp em = new Emp(empId);
            return this.GetHisNodes(em.HisStations);
        }
        /// <summary>
        /// 人员对应的流程
        /// </summary>
        /// <param name="stationNo">人员编号</param>
        /// <returns>流程s</returns>
        public Nodes GetHisNodes(string stationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowEmpAttr.FK_Emp, stationNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (FlowEmp en in this)
            {
                ens.AddEntity(new Node(en.FK_Flow));
            }
            return ens;
        }
        /// <summary>
        /// 转向此流程的集合的Nodes
        /// </summary>
        /// <param name="nodeID">此流程的ID</param>
        /// <returns>转向此流程的集合的Nodes (FromNodes)</returns> 
        public Stations GetHisStations(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowEmpAttr.FK_Flow, nodeID);
            qo.DoQuery();

            Stations ens = new Stations();
            foreach (FlowEmp en in this)
            {
                ens.AddEntity(new Station(en.FK_Emp));
            }
            return ens;
        }


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowEmp> Tolist()
        {
            System.Collections.Generic.List<FlowEmp> list = new System.Collections.Generic.List<FlowEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
