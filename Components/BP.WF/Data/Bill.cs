using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Web;
using BP.Sys;

namespace BP.WF
{
	/// <summary>
	/// 单据属性
	/// </summary>
    public class BillAttr
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
        /// FK_Bill
        /// </summary>
        public const string FK_Bill = "FK_Bill";
        /// <summary>
        /// FK_Bill
        /// </summary>
        public const string Url = "Url";
        /// <summary>
        /// 送达否
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 发起人
        /// </summary>
        public const string FK_Starter = "FK_Starter";
        /// <summary>
        /// 文号
        /// </summary>
        public const string FilePrix = "FilePrix";
        /// <summary>
        /// FileName
        /// </summary>
        public const string FileName = "FileName";
        /// <summary>
        /// 记录时间．
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// FID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// FK_Flow
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// FK_BillType
        /// </summary>
        public const string FK_BillType = "FK_BillType";
        public const string Title = "Title";
        public const string StartDT = "StartDT";
        /// <summary>
        /// 参与人
        /// </summary>
        public const string Emps = "Emps";
        /// <summary>
        /// 全路径
        /// </summary>
        public const string FullPath = "FullPath";
        #endregion
    }
	/// <summary>
	/// 单据
	/// </summary> 
    public class Bill : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 路径
        /// </summary>
        public string FullPath
        {
            get
            {
                return this.GetValStringByKey(BillAttr.FullPath);
            }
            set
            {
                this.SetValByKey(BillAttr.FullPath, value);
            }
        }
        /// <summary>
        /// 参与人员
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStringByKey(BillAttr.Emps);
            }
            set
            {
                this.SetValByKey(BillAttr.Emps, value);
            }
        }
        /// <summary>
        /// 发起日期
        /// </summary>
        public string StartDT
        {
            get
            {
                return this.GetValStringByKey(BillAttr.StartDT);
            }
            set
            {
                this.SetValByKey(BillAttr.StartDT, value);
            }
        }
        /// <summary>
        /// 单据类型
        /// </summary>
        public string FK_BillType
        {
            get
            {
                return this.GetValStringByKey(BillAttr.FK_BillType);
            }
            set
            {
                this.SetValByKey(BillAttr.FK_BillType, value);
            }
        }
        /// <summary>
        /// 单据类型名称
        /// </summary>
        public string FK_BillTypeT
        {
            get
            {
                return this.GetValStrByKey(BillAttr.FK_BillType);
            }
        }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(BillAttr.Title);
            }
            set
            {
                this.SetValByKey(BillAttr.Title, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(BillAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(BillAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FK_FlowT
        {
            get
            {
                return this.GetValRefTextByKey(BillAttr.FK_Flow);
            }
        }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string FK_StarterT
        {
            get
            {
                return this.GetValRefTextByKey(BillAttr.FK_Starter);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public string FK_Starter
        {
            get
            {
                return this.GetValStringByKey(BillAttr.FK_Starter);
            }
            set
            {
                this.SetValByKey(BillAttr.FK_Starter, value);
            }
        }
        /// <summary>
        /// 操作人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(BillAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(BillAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 操作人员名称
        /// </summary>
        public string FK_EmpT
        {
            get
            {
                return this.GetValRefTextByKey(BillAttr.FK_Emp);
            }
        }
        /// <summary>
        /// 单据名称
        /// </summary>
        public string FK_BillText
        {
            get
            {
                return this.GetValRefTextByKey(BillAttr.FK_Bill);
            }
        }
        /// <summary>
        /// 单据编号
        /// </summary>
        public string FK_Bill
        {
            get
            {
                return this.GetValStrByKey(BillAttr.FK_Bill);
            }
            set
            {
                this.SetValByKey(BillAttr.FK_Bill, value);
            }
        }
        /// <summary>
        /// 年月
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStrByKey(BillAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(BillAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(BillAttr.WorkID);
            }
            set
            {
                this.SetValByKey(BillAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(BillAttr.FID);
            }
            set
            {
                this.SetValByKey(BillAttr.FID, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(BillAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(BillAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string FK_NodeT
        {
            get
            {
                Node nd = new Node(this.FK_Node);
                return nd.Name;
                //return this.GetValRefTextByKey(BillAttr.FK_Node);
            }
        }
        /// <summary>
        /// 单据打印时间
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(BillAttr.RDT);
            }
            set
            {
                this.SetValByKey(BillAttr.RDT, value);
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(BillAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(BillAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string FK_DeptT
        {
            get
            {
                return this.GetValRefTextByKey(BillAttr.FK_Dept);
            }
        }
        /// <summary>
        /// 超连接
        /// </summary>
        public string Url
        {
            get
            {
                return this.GetValStringByKey(BillAttr.Url);
            }
            set
            {
                this.SetValByKey(BillAttr.Url, value);
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
        /// 单据
        /// </summary>
        public Bill() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pk"></param>
        public Bill(string pk)
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
                Map map = new Map("WF_Bill", "单据");

                map.AddMyPK(false);

                map.AddTBInt(BillAttr.WorkID, 0, "工作ID", false, true);
                map.AddTBInt(BillAttr.FID, 0, "FID", false, true);
                map.AddTBString(BillAttr.FK_Flow, null, "流程", false, false, 0, 4, 5);
                map.AddTBString(BillAttr.FK_BillType, null, "单据类型", false, false, 0, 300, 5);
                map.AddTBString(BillAttr.Title, null, "标题", false, false, 0, 900, 5);
                map.AddTBString(BillAttr.FK_Starter, null, "发起人", true, true, 0, 50, 5);
                map.AddTBDateTime(BillAttr.StartDT, "发起时间", true, true);

                map.AddTBString(BillAttr.Url, null, "Url", false, false, 0, 2000, 5);
                map.AddTBString(BillAttr.FullPath, null, "FullPath", false, false, 0, 2000, 5);

                map.AddDDLEntities(BillAttr.FK_Emp, null, "打印人", new Emps(), false);
                map.AddTBDateTime(BillAttr.RDT, "打印时间", true, true);

                map.AddDDLEntities(BillAttr.FK_Dept, null, "隶属部门", new BP.Port.Depts(), false);
                
                map.AddDDLEntities(BillAttr.FK_NY, null, "隶属年月", new BP.Pub.NYs(), false);

                map.AddTBString(BillAttr.Emps, null, "Emps", false, false, 0, 4000, 5);

                map.AddTBString(BillAttr.FK_Node, null, "节点", false, false, 0, 30, 5);
                map.AddTBString(BillAttr.FK_Bill, null, "FK_Bill", false, false, 0, 500, 5);
                map.AddTBIntMyNum();

                map.AddSearchAttr(BillAttr.FK_Dept);
                map.AddSearchAttr(BillAttr.FK_NY);

                map.AddSearchAttr(BillAttr.FK_Emp);

                RefMethod rm = new RefMethod();
                rm.Title = "打开";
                rm.ClassMethodName = this.ToString() + ".DoOpen";
                rm.Icon = "../../WF/Img/FileType/doc.gif";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "打开";
                rm.ClassMethodName = this.ToString() + ".DoOpenPDF";
                rm.Icon = "../../WF/Img/FileType/pdf.gif";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 打开
        /// </summary>
        /// <returns></returns>
        public string DoOpen()
        {
            string path = HttpContextHelper.PhysicalApplicationPath + (this.Url);
            return path; 
        }
        /// <summary>
        /// 打开pdf
        /// </summary>
        /// <returns></returns>
        public string DoOpenPDF()
        {
            string path = HttpContextHelper.PhysicalApplicationPath + (this.Url);
            return path;
        }
    }
	/// <summary>
	/// 单据s
	/// </summary>
	public class Bills :EntitiesMyPK
	{
		#region 构造方法属性
		/// <summary>
        /// 单据s
		/// </summary>
		public Bills(){}
		#endregion 

		#region 属性
		/// <summary>
        /// 单据
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new Bill();
			}
		}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Bill> ToJavaList()
        {
            return (System.Collections.Generic.IList<Bill>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Bill> Tolist()
        {
            System.Collections.Generic.List<Bill> list = new System.Collections.Generic.List<Bill>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Bill)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
