using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.WF
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
        /// 工作节点
        /// </summary>
        public const string NodeIDWork = "NodeIDWork";
        /// <summary>
        /// 抄送节点
        /// </summary>
        public const string NodeIDCC = "NodeIDCC";
        public const string NodeIDCCName = "NodeIDCCName";
        /// <summary>
        /// 流程
        /// </summary>
        public const string FlowNo = "FlowNo";
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
        /// 审核时间（回复时间）
        /// </summary>
        public const string CDT = "CDT";
        /// <summary>
        /// 阅读时间
        /// </summary>
        public const string ReadDT = "ReadDT";
        /// <summary>
        /// 抄送人员
        /// </summary>
        public const string RecEmpNo = "RecEmpNo";
        /// <summary>
        /// 抄送人员名称
        /// </summary>
        public const string RecEmpName = "RecEmpName";
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
        /// <summary>
        /// domain
        /// </summary>
        public const string Domain = "Domain";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        #endregion

        public const string CCToOrgNo = "CCToOrgNo";
        public const string CCToOrgName = "CCToOrgName";
        public const string CCToDept = "CCToDept";
        public const string CCToDeptName = "CCToDeptName";

        public const string ToNodeID = "ToNodeID";
        public const string ToNodeName = "ToNodeName";

        public const string DeptNo = "DeptNo";
        public const string DeptName = "DeptName";


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
                //@sly 这里去掉了业务逻辑.
                if (value == CCSta.Read)
                    this.ReadDT = DataType.CurrentDateTime;
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
        /// <summary>
        /// 域
        /// </summary>
        public string Domain
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.Domain);
            }
            set
            {
                this.SetValByKey(CCListAttr.Domain, value);
            }
        }
        /// <summary>
        /// 抄送给
        /// </summary>
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
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(CCListAttr.OrgNo, value);
            }
        }

        public int ToNodeID
        {
            get
            {
                return this.GetValIntByKey(CCListAttr.ToNodeID);
            }
            set
            {
                this.SetValByKey(CCListAttr.ToNodeID, value);
            }
        }
        public string ToNodeName
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.ToNodeName);
            }
            set
            {
                this.SetValByKey(CCListAttr.ToNodeName, value);
            }
        }
        /// <summary>
        /// 抄送给Name
        /// </summary>
        public string CCToName
        {
            get
            {
                string s = this.GetValStringByKey(CCListAttr.CCToName);
                if (DataType.IsNullOrEmpty(s))
                    s = this.CCTo;
                return s;
            }
            set
            {
                this.SetValByKey(CCListAttr.CCToName, value);
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
        public int NodeIDCC
        {
            get
            {
                return this.GetValIntByKey(CCListAttr.NodeIDCC);
            }
            set
            {
                this.SetValByKey(CCListAttr.NodeIDCC, value);
            }
        }
        public int NodeIDWork
        {
            get
            {
                return this.GetValIntByKey(CCListAttr.NodeIDWork);
            }
            set
            {
                this.SetValByKey(CCListAttr.NodeIDWork, value);
            }
        }


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
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(CCListAttr.FlowNo, value);
            }
        }
        public string RecEmpNo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.RecEmpNo);
            }
            set
            {
                this.SetValByKey(CCListAttr.RecEmpNo, value);
            }
        }
        public string RecEmpName
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.RecEmpName);
            }
            set
            {
                this.SetValByKey(CCListAttr.RecEmpName, value);
            }
        }
        /// <summary>
        /// 读取日期
        /// </summary>
        public string ReadDT
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.ReadDT);
            }
            set
            {
                this.SetValByKey(CCListAttr.ReadDT, value);
            }
        }
        /// <summary>
        /// 写入日期
        /// </summary>
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

                map.AddMyPK(); //组合主键 WorkID+"_"+NodeID+"_"+FK_Emp 
                map.AddTBString(CCListAttr.Title, null, "标题", true, true, 0, 500, 10, true);
                //状态  @0=抄送@1=已读@2=已回复@3=已删除
                map.AddTBInt(CCListAttr.Sta, 0, "状态", true, true);
                map.AddTBString(CCListAttr.FlowNo, null, "流程编号", true, true, 0, 5, 10, true);
                map.AddTBString(CCListAttr.FlowName, null, "名称", true, true, 0, 200, 10, true);

                map.AddTBInt(CCListAttr.NodeIDWork, 0, "工作节点", true, true);//工作节点.
                map.AddTBString(CCListAttr.NodeName, null, "工作节点名称", true, true, 0, 500, 10, true);

                map.AddTBInt(CCListAttr.NodeIDCC, 0, "抄送节点ID", true, true);//工作节点.

                map.AddTBInt(CCListAttr.WorkID, 0, "工作ID", true, true);
                map.AddTBInt(CCListAttr.FID, 0, "FID", true, true);

                map.AddTBString(CCListAttr.CCTo, null, "抄送给", true, false, 0, 50, 10, true);
                map.AddTBString(CCListAttr.CCToName, null, "抄送给(人员名称)", true, false, 0, 50, 10, true);

                map.AddTBString(CCListAttr.DeptNo, null, "被抄送人部门", true, false, 0, 50, 10, true);
                map.AddTBString(CCListAttr.DeptName, null, "被抄送人部门", true, false, 0, 50, 10, true);

                map.AddTBDateTime(CCListAttr.ReadDT, null, "阅读时间", true, false);

                map.AddTBString(CCListAttr.PFlowNo, null, "父流程编号", true, true, 0, 100, 10, true);
                map.AddTBInt(CCListAttr.PWorkID, 0, "父流程WorkID", true, true);
                map.AddBoolean(CCListAttr.InEmpWorks, false, "是否加入待办列表", true, true);

                map.AddTBString(CCListAttr.RecEmpNo, null, "抄送人员", true, true, 0, 50, 10, true);
                map.AddTBString(CCListAttr.RecEmpName, null, "抄送人员", true, true, 0, 50, 10, true);
                map.AddTBDateTime(CCListAttr.RDT, null, "抄送日期", true, false);

                //add by zhoupeng  
                map.AddTBString(CCListAttr.Domain, null, "Domain", true, true, 0, 50, 10, true);
                map.AddTBString(CCListAttr.OrgNo, null, "OrgNo", true, true, 0, 50, 10, true);
                
                map.AddTBInt(CCListAttr.NodeIDCC, 0, "抄送到节点ID", true, true); //工作节点.
                map.AddTBString(CCListAttr.NodeIDCCName, null, "抄送到节点名称", true, true, 0, 50, 10, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            if (this.OrgNo == null)
                this.OrgNo = BP.Web.WebUser.OrgNo;
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 抄送
    /// </summary>
    public class CCLists : EntitiesMyPK
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
        public CCLists() { }


        /// <summary>
        /// 查询出来所有的抄送信息
        /// </summary>
        /// <param name="NodeID"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        public CCLists(int NodeID, Int64 workid, Int64 fid)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCListAttr.NodeIDWork, NodeID);
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
