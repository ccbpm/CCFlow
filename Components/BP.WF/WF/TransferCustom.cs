using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;
using BP.WF.Template;

namespace BP.WF
{
    /// <summary>
    /// 自定义运行路径 属性
    /// </summary>
    public class TransferCustomAttr : EntityMyPKAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 节点名称
        /// </summary>
        public const string NodeName = "NodeName";
        /// <summary>
        /// 处理人编号（多个人用逗号分开）
        /// </summary>
        public const string Worker = "Worker";
        /// <summary>
        /// 处理人显示（多个人用逗号分开）
        /// </summary>
        public const string WorkerName = "WorkerName";
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 发起时间
        /// </summary>
        public const string StartDT = "StartDT";
        /// <summary>
        /// 计划完成日期
        /// </summary>
        public const string PlanDT = "PlanDT";
        /// <summary>
        /// 要启用的子流程编号
        /// </summary>
        public const string SubFlowNo = "SubFlowNo";
        /// <summary>
        /// 是否通过了
        /// </summary>
        public const string TodolistModel = "TodolistModel";
        /// <summary>
        /// 是否启用
        /// </summary>
        public const string IsEnable = "IsEnable";
        #endregion
    }
    /// <summary>
    /// 自定义运行路径
    /// </summary>
    public class TransferCustom : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(TransferCustomAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(TransferCustomAttr.FK_Node, value);
            }
        }
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(TransferCustomAttr.WorkID);
            }
            set
            {
                this.SetValByKey(TransferCustomAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStringByKey(TransferCustomAttr.NodeName);
            }
            set
            {
                this.SetValByKey(TransferCustomAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 计划完成日期
        /// </summary>
        public string PlanDT
        {
            get
            {
                return this.GetValStringByKey(TransferCustomAttr.PlanDT);
            }
            set
            {
                this.SetValByKey(TransferCustomAttr.PlanDT, value);
            }
        }
        /// <summary>
        /// 处理人（多个人用逗号分开）
        /// </summary>
        public string Worker
        {
            get
            {
                return this.GetValStringByKey(TransferCustomAttr.Worker);
            }
            set
            {
                this.SetValByKey(TransferCustomAttr.Worker, value);
            }
        }
        /// <summary>
        /// 处理人显示（多个人用逗号分开）
        /// </summary>
        public string WorkerName
        {
            get
            {
                return this.GetValStringByKey(TransferCustomAttr.WorkerName);
            }
            set
            {
                this.SetValByKey(TransferCustomAttr.WorkerName, value);
            }
        }
        /// <summary>
        /// 要启用的子流程编号
        /// </summary>
        public string SubFlowNo
        {
            get
            {
                return this.GetValStringByKey(TransferCustomAttr.SubFlowNo);
            }
            set
            {
                this.SetValByKey(TransferCustomAttr.SubFlowNo, value);
            }
        }
        /// <summary>
        /// 多人处理工作模式
        /// </summary>
        public TodolistModel TodolistModel
        {
            get
            {
                return (TodolistModel)this.GetValIntByKey(TransferCustomAttr.TodolistModel);
            }
            set
            {
                this.SetValByKey(TransferCustomAttr.TodolistModel, (int)value);
            }
        }
        /// <summary>
        /// 发起时间（可以为空）
        /// </summary>
        public string StartDT
        {
            get
            {
                return this.GetValStringByKey(TransferCustomAttr.StartDT);
            }
            set
            {
                this.SetValByKey(TransferCustomAttr.StartDT, value);
            }
        }
        /// <summary>
        /// 顺序
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(TransferCustomAttr.Idx);
            }
            set
            {
                this.SetValByKey(TransferCustomAttr.Idx, value);
            }
        }
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(TransferCustomAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(TransferCustomAttr.IsEnable, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// TransferCustom
        /// </summary>
        public TransferCustom()
        {
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
                Map map = new Map("WF_TransferCustom", "自定义运行路径");
                map.Java_SetEnType(EnType.Admin);

                map.AddMyPK(); //唯一的主键.

                //主键.
                map.AddTBInt(TransferCustomAttr.WorkID, 0, "WorkID", true, false);
                map.AddTBInt(TransferCustomAttr.FK_Node, 0, "节点ID", true, false);
                map.AddTBString(TransferCustomAttr.NodeName, null, "节点名称", true, false, 0, 200, 10);

                map.AddTBString(TransferCustomAttr.Worker, null, "处理人(多个人用逗号分开)", true, false, 0, 200, 10);
                map.AddTBString(TransferCustomAttr.WorkerName, null, "处理人(多个人用逗号分开)", true, false, 0, 200, 10);

                map.AddTBString(TransferCustomAttr.SubFlowNo, null, "要经过的子流程编号", true, false, 0, 3, 10);
                map.AddTBDateTime(TransferCustomAttr.PlanDT, null, "计划完成日期", true, false);
                map.AddTBInt(TransferCustomAttr.TodolistModel, 0, "多人工作处理模式", true, false);
                map.AddTBInt(TransferCustomAttr.IsEnable, 0, "是否启用", true, false);

                map.AddTBInt(TransferCustomAttr.Idx, 0, "顺序号", true, false);

                //map.AddTBString(TransferCustomAttr.StartDT, null, "发起时间", true, false, 0, 20, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string DoUp()
        {
            this.DoOrderUp(TransferCustomAttr.WorkID, this.WorkID.ToString(), TransferCustomAttr.Idx);
            return "执行成功";
        }

        public string DoDown()
        {
            this.DoOrderDown(TransferCustomAttr.WorkID, this.WorkID.ToString(), TransferCustomAttr.Idx);
            return "执行成功";
        }

        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_Node + "_" + this.WorkID;
            return base.beforeInsert();
        }

        /// <summary>
        /// 获取下一个要到达的定义路径.
        /// 要分析如下几种情况:
        /// 1, 当前节点不存在队列里面，就返回第一个.
        /// 2, 如果当前队列为空,就认为需要结束掉, 返回null.
        /// 3, 如果当前节点是最后一个并且没有连接线连到固定节点,就返回null,表示要结束流程.
        /// 4. 如果当前节点是最后一个且有连接线连到固定节点
        /// </summary>
        /// <param name="workid">当前工作ID</param>
        /// <param name="currNodeID">当前节点ID</param>
        /// <returns>获取下一个要到达的定义路径,如果没有就返回空.</returns>
        public static TransferCustom GetNextTransferCustom(Int64 workid, int currNodeID)
        {
            TransferCustoms ens = new TransferCustoms();
            ens.Retrieve(TransferCustomAttr.WorkID, workid, TransferCustomAttr.Idx);
            if (ens.Count == 0)
                return null;

            //寻找当前节点的下一个. 
            bool isMeet = false;
            foreach (TransferCustom item in ens)
            {
                if (item.FK_Node == currNodeID)
                {
                    isMeet = true;
                    continue;
                }

                if (isMeet == true && item.IsEnable == true)
                    return item;
            }

            //if (currNodeID.ToString().EndsWith("01") == true)
            //{
            foreach (TransferCustom item in ens)
            {
                if(item.IsEnable == true)
                    return (TransferCustom)item;
            }

           // }

            //如果当前节点是最后一个自定义节点，且有连接线连到固定节点
            if(isMeet == true)
            {
                //判断当前节点是否连接到固定节点
                string sql = "SELECT AtPara FROM WF_Node WHERE NodeID In(SELECT ToNode FROM WF_Direction WHERE Node=" + currNodeID + ")";
                Nodes nds = new Nodes();
                nds.RetrieveInSQL(NodeAttr.NodeID,"SELECT ToNode FROM WF_Direction WHERE Node = " + currNodeID);
                foreach(Node nd in nds)
                {
                    if (nd.GetParaBoolen(NodeAttr.IsYouLiTai) == true)
                        continue;
                   
                    TransferCustom en = new TransferCustom();
                    en.FK_Node = nd.NodeID;
                    //更改流程的运行状态@yuan
                    GenerWorkFlow gwf = new GenerWorkFlow(workid);
                    gwf.TransferCustomType = TransferCustomType.ByCCBPMDefine;
                    gwf.Update();
                    return en;
                }
               
            }
                
            return null;
            // return null;
        }
    }
    /// <summary>
    /// 自定义运行路径
    /// </summary>
    public class TransferCustoms : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TransferCustom();
            }
        }
        /// <summary>
        /// 自定义运行路径
        /// </summary>
        public TransferCustoms()
        {
        }
        /// <summary>
        /// 自定义运行路径
        /// </summary>
        /// <param name="workid">工作ID</param>
        public TransferCustoms(Int64 workid)
        {
            this.Retrieve(TransferCustomAttr.WorkID, workid, TransferCustomAttr.Idx);
        }
        /// <summary>
        /// 自定义运行路径
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        public TransferCustoms(int nodeID, Int64 workid)
        {
            this.Retrieve(TransferCustomAttr.WorkID, workid, TransferCustomAttr.FK_Node, nodeID, TransferCustomAttr.Idx);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TransferCustom> ToJavaList()
        {
            return (System.Collections.Generic.IList<TransferCustom>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TransferCustom> Tolist()
        {
            System.Collections.Generic.List<TransferCustom> list = new System.Collections.Generic.List<TransferCustom>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TransferCustom)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
