using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
using BP.Web;
using BP.Sys;

namespace BP.WF.Data
{
    public class CHNodeAttr
    {
        #region 属性
        public const string MyPK = "MyPK";
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 节点名称
        /// </summary>
        public const string NodeName = "NodeName";
        /// <summary>
        /// 计划开始时间
        /// </summary>
        public const string StartDT = "StartDT";
        /// <summary>
        /// 计划结束时间
        /// </summary>
        public const string EndDT = "EndDT";
        /// <summary>
        /// 工天
        /// </summary>
        public const string GT = "GT";
        /// <summary>
        /// 阶段占比
        /// </summary>
        public const string Scale = "Scale";
        /// <summary>
        /// 总进度
        /// </summary>
        public const string TotalScale = "TotalScale";
        /// <summary>
        /// 产值
        /// </summary>
        public const string ChanZhi = "ChanZhi";

        #endregion 属性
    }
    /// <summary>
    /// 节点时限
    /// </summary> 
    public class CHNode : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(CHAttr.WorkID);
            }
            set
            {
                this.SetValByKey(CHAttr.WorkID, value);
            }
        }


       
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(CHAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStrByKey(CHNodeAttr.NodeName);
            }
            set
            {
                this.SetValByKey(CHNodeAttr.NodeName, value);
            }
        }

        /// <summary>
        /// 操作人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(CHAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 人员
        /// </summary>
        public string FK_EmpT
        {
            get
            {
                return this.GetValStringByKey(CHAttr.FK_EmpT);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_EmpT, value);
            }
        }

        /// <summary>
        /// 计划开始时间
        /// </summary>
        public string StartDT
        {
            get
            {
                return this.GetValStringByKey(CHNodeAttr.StartDT);
            }
            set
            {
                this.SetValByKey(CHNodeAttr.StartDT, value);
            }
        }
        /// <summary>
        /// 计划完成时间
        /// </summary>
        public string EndDT
        {
            get
            {
                return this.GetValStringByKey(CHNodeAttr.EndDT);
            }
            set
            {
                this.SetValByKey(CHNodeAttr.EndDT, value);
            }
        }

        /// <summary>
        /// 工天
        /// </summary>
        public int GT
        {
            get
            {
                return this.GetValIntByKey(CHNodeAttr.GT);
            }
            set
            {
                this.SetValByKey(CHNodeAttr.GT, value);
            }
        }
        /// <summary>
        /// 阶段占比
        /// </summary>
        public float Scale
        {
            get
            {
                return this.GetValFloatByKey(CHNodeAttr.Scale);
            }
            set
            {
                this.SetValByKey(CHNodeAttr.Scale, value);
            }
        }
        
        /// <summary>
        /// 总体进度
        /// </summary>
        public float TotalScale
        {
            get
            {
                return this.GetValFloatByKey(CHNodeAttr.TotalScale);
            }
            set
            {
                this.SetValByKey(CHNodeAttr.TotalScale, value);
            }
        }
        /// <summary>
        /// 产值
        /// </summary>
        public float ChanZhi
        {
            get
            {
                return this.GetValFloatByKey(CHNodeAttr.ChanZhi);
            }
            set
            {
                this.SetValByKey(CHNodeAttr.ChanZhi, value);
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = false;
                uac.IsView = true;
                return uac;
            }
        }
        /// <summary>
        /// 节点时限
        /// </summary>
        public CHNode() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pk"></param>
        public CHNode(string pk)
            : base(pk)
        {
        }
        
        #endregion

        #region Map
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_CHNode", "节点时限");

                map.AddTBInt(CHNodeAttr.WorkID, 0, "WorkID", true, true);
                map.AddTBInt(CHNodeAttr.FK_Node, 0, "节点", true, true);
                map.AddTBString(CHNodeAttr.NodeName, null, "节点名称", true, true, 0, 50, 5);

                map.AddTBString(CHAttr.FK_Emp, null, "处理人", true, true, 0, 30, 3);
                map.AddTBString(CHAttr.FK_EmpT, null, "处理人名称", true, true, 0, 200, 5);

                map.AddTBString(CHNodeAttr.StartDT, null, "计划开始时间", true, true, 0, 50, 5);
                map.AddTBString(CHNodeAttr.EndDT, null, "计划结束时间", true, true, 0, 50, 5);
                map.AddTBInt(CHNodeAttr.GT, 0, "工天", true, true);
                map.AddTBFloat(CHNodeAttr.Scale, 0, "阶段占比", true, true);
                map.AddTBFloat(CHNodeAttr.TotalScale, 0, "总进度", true, true);
                map.AddTBFloat(CHNodeAttr.ChanZhi, 0, "产值", true, true);
                
                map.AddTBAtParas(500);

                map.AddTBStringPK(CHNodeAttr.MyPK, null, "MyPK", false, false, 0, 50, 5);

               
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.WorkID + "_" + this.FK_Node;
            return base.beforeUpdateInsertAction();
        }

    }
    /// <summary>
    /// 节点时限s
    /// </summary>
    public class CHNodes :Entities
	{
		#region 构造方法属性
		/// <summary>
        /// 节点时限s
		/// </summary>
		public CHNodes(){}

        public CHNodes(Int64 WorkID)
        {
            this.Retrieve(CHNodeAttr.WorkID, WorkID);
            return;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 节点时限
        /// </summary>
        public override Entity GetNewEntity
		{
			get
			{
				return new CHNode();
			}
		}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CHNode> ToJavaList()
        {
            return (System.Collections.Generic.IList<CHNode>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CHNode> Tolist()
        {
            System.Collections.Generic.List<CHNode> list = new System.Collections.Generic.List<CHNode>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CHNode)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
