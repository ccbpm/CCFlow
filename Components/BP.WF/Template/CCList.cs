using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.WF.Template
{
	/// <summary>
	/// 抄送 属性
	/// </summary>
    public class CCListAttr : EntityMyPKAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 抄送内容
        /// </summary>
        public const string Doc = "Doc";
        /// <summary>
        /// 抄送的节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 从节点
        /// </summary>
        public const string NDFrom = "NDFrom";
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        public const string FlowName = "FlowName";
        public const string NodeName = "NodeName";
        /// <summary>
        /// 是否读取
        /// </summary>
        public const string Sta = "Sta";
        public const string WorkID = "WorkID";
        public const string FID = "FID";
        /// <summary>
        /// 抄送给
        /// </summary>
        public const string CCTo = "CCTo";
        /// <summary>
        /// 抄送给人员名称
        /// </summary>
        public const string CCToName = "CCToName";
        /// <summary>
        /// 抄送给到部门
        /// </summary>
        public const string CCToDept = "CCToDept";
        /// <summary>
        /// 抄送给部门名称
        /// </summary>
        public const string CCToDeptName = "CCToDeptName";
        /// <summary>
        /// 审核时间
        /// </summary>
        public const string CDT = "CDT";
        /// <summary>
        /// 抄送人员
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// RDT
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 父流程ID
        /// </summary>
        public const string PWorkID = "PWorkID";
        /// <summary>
        /// 父流程编号
        /// </summary>
        public const string PFlowNo = "PFlowNo";
        /// <summary>
        /// 优先级
        /// </summary>
        public const string PRI = "PRI";
        /// <summary>
        /// 是否加入待办列表
        /// </summary>
	    public const string InEmpWorks = "InEmpWorks";
        #endregion
    }
    public enum CCSta
    {
        /// <summary>
        /// 未读
        /// </summary>
        UnRead,
        /// <summary>
        /// 已读取
        /// </summary>
        Read,
        /// <summary>
        /// 已经回复
        /// </summary>
        CheckOver,
        /// <summary>
        /// 已删除
        /// </summary>
        Del
    }
	/// <summary>
	/// 抄送
	/// </summary>
    public class CCList : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 状态
        /// </summary>
        public CCSta HisSta
        {
            get
            {
                return (CCSta)this.GetValIntByKey(CCListAttr.Sta);
            }
            set
            {
                if (value == CCSta.Read)
                    this.CDT = DataType.CurrentDataTime;
                this.SetValByKey(CCListAttr.Sta, (int)value);
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {

                UAC uac = new UAC();
                if (BP.Web.WebUser.No != "admin")
                {
                    uac.IsView = false;
                    return uac;
                }
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        public string CCTo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.CCTo);
            }
            set
            {
                this.SetValByKey(CCListAttr.CCTo, value);
            }
        }
        /// <summary>
        /// 抄送部门
        /// </summary>
        public string CCToDept
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.CCToDept);
            }
            set
            {
                this.SetValByKey(CCListAttr.CCToDept, value);
            }
        }
        /// <summary>
        /// 抄送给Name
        /// </summary>
        public string CCToName
        {
            get
            {
                string s= this.GetValStringByKey(CCListAttr.CCToName);
                if (DataType.IsNullOrEmpty(s))
                    s=this.CCTo;
                return s;
            }
            set
            {
                this.SetValByKey(CCListAttr.CCToName, value);
            }
        }

        /// <summary>
        /// 抄送给部门名称
        /// </summary>
        public string CCToDeptName
        {
            get
            {
                string s = this.GetValStringByKey(CCListAttr.CCToDeptName);
                if (DataType.IsNullOrEmpty(s))
                    return "无";
                return s;
            }
            set
            {
                this.SetValByKey(CCListAttr.CCToDeptName, value);
            }
        }
        /// <summary>
        /// 抄送给部门名称
        /// </summary>
        public string CCToDeptNameHtml
        {
            get
            {
                string s = this.GetValStringByKey(CCListAttr.CCToDeptName);
                if (DataType.IsNullOrEmpty(s))
                    return "无";
                return DataType.ParseText2Html(s);
            }
        }
        /// <summary>
        /// 读取时间
        /// </summary>
        public string CDT
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.CDT);
            }
            set
            {
                this.SetValByKey(CCListAttr.CDT, value);
            }
        }
        /// <summary>
        /// 抄送人所在的节点编号
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(CCListAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(CCListAttr.FK_Node, value);
            }
        }
        //public int NDFrom
        //{
        //    get
        //    {
        //        return this.GetValIntByKey(CCListAttr.NDFrom);
        //    }
        //    set
        //    {
        //        this.SetValByKey(CCListAttr.NDFrom, value);
        //    }
        //}
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(CCListAttr.WorkID);
            }
            set
            {
                this.SetValByKey(CCListAttr.WorkID, value);
            }
        }
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(CCListAttr.FID);
            }
            set
            {
                this.SetValByKey(CCListAttr.FID, value);
            }
        }
        /// <summary>
        /// 父流程工作ID
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(CCListAttr.PWorkID);
            }
            set
            {
                this.SetValByKey(CCListAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 父流程编号
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.PFlowNo);
            }
            set
            {
                this.SetValByKey(CCListAttr.PFlowNo, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_FlowT
        {
            get
            {
                return this.GetValRefTextByKey(CCListAttr.FK_Flow);
            }
        }
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.FlowName);
            }
            set
            {
                this.SetValByKey(CCListAttr.FlowName, value);
            }
        }
        public string NodeName
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.NodeName);
            }
            set
            {
                this.SetValByKey(CCListAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 抄送标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.Title);
            }
            set
            {
                this.SetValByKey(CCListAttr.Title, value);
            }
        }
        /// <summary>
        /// 抄送内容
        /// </summary>
        public string Doc
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.Doc);
            }
            set
            {
                this.SetValByKey(CCListAttr.Doc, value);
            }
        }
        public string DocHtml
        {
            get
            {
                return this.GetValHtmlStringByKey(CCListAttr.Doc);
            }
        }
        /// <summary>
        /// 抄送对象
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(CCListAttr.FK_Flow, value);
            }
        }
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.Rec);
            }
            set
            {
                this.SetValByKey(CCListAttr.Rec, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.RDT);
            }
            set
            {
                this.SetValByKey(CCListAttr.RDT, value);
            }
        }
        /// <summary>
        /// 是否加入待办列表
        /// </summary>
	    public bool InEmpWorks
	    {
            get { return this.GetValBooleanByKey(CCListAttr.InEmpWorks); }
            set { this.SetValByKey(CCListAttr.InEmpWorks, value); }
	    }
        #endregion

        #region 构造函数
        /// <summary>
        /// CCList
        /// </summary>
        public CCList()
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
                Map map = new Map("WF_CCList", "抄送列表");

                map.AddMyPK();

                map.AddTBString(CCListAttr.Title, null, "标题", true, true, 0, 500, 10, true);
                map.AddTBInt(CCListAttr.Sta, 0, "状态", true, true);

                map.AddTBString(CCListAttr.FK_Flow, null, "流程编号", true, true, 0, 3, 10, true);
                map.AddTBString(CCListAttr.FlowName, null, "流程名称", true, true, 0, 200, 10, true);
                map.AddTBInt(CCListAttr.FK_Node, 0, "节点", true, true);
                map.AddTBString(CCListAttr.NodeName, null, "节点名称", true, true, 0, 500, 10, true);

                map.AddTBInt(CCListAttr.WorkID, 0, "工作ID", true, true);
                map.AddTBInt(CCListAttr.FID, 0, "FID", true, true);
                
                map.AddTBStringDoc();

                map.AddTBString(CCListAttr.Rec, null, "抄送人员", true, true, 0, 50, 10, true);
                map.AddTBDateTime(CCListAttr.RDT, null, "记录日期", true, false);


                map.AddTBString(CCListAttr.CCTo, null, "抄送给", true, false, 0, 50, 10, true);
                map.AddTBString(CCListAttr.CCToName, null, "抄送给(人员名称)", true, false, 0, 50, 10, true);

                map.AddTBString(CCListAttr.CCToDept, null, "抄送到部门", true, false, 0, 50, 10, true);
                map.AddTBString(CCListAttr.CCToDeptName, null, "抄送给部门名称", true, false, 0, 600, 10, true);

                map.AddTBDateTime(CCListAttr.CDT, null, "打开时间", true, false);

                map.AddTBString(CCListAttr.PFlowNo, null, "父流程编号", true, true, 0, 100, 10, true);
                map.AddTBInt(CCListAttr.PWorkID, 0, "父流程WorkID", true, true);
                //added by liuxc,2015.7.6，标识是否在待办列表里显示
                map.AddBoolean(CCListAttr.InEmpWorks, false, "是否加入待办列表", true, true);
                 
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 抄送
	/// </summary>
	public class CCLists: EntitiesMyPK
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new CCList();
			}
		}
		/// <summary>
        /// 抄送
		/// </summary>
		public CCLists(){}


        /// <summary>
        /// 查询出来所有的抄送信息
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        public CCLists(string flowNo, Int64 workid, Int64 fid)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCListAttr.FK_Flow, flowNo);
            qo.addAnd();
            if (fid != 0)
                qo.AddWhereIn(CCListAttr.WorkID, "(" + workid + "," + fid + ")");
            else
                qo.AddWhere(CCListAttr.WorkID, workid);
            qo.DoQuery();
        } 		 
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CCList> ToJavaList()
        {
            return (System.Collections.Generic.IList<CCList>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CCList> Tolist()
        {
            System.Collections.Generic.List<CCList> list = new System.Collections.Generic.List<CCList>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CCList)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
