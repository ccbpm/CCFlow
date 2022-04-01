using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Web;
using BP.Sys;

namespace BP.Sys
{
	/// <summary>
	/// 单据属性
	/// </summary>
    public class FrmPrintDBAttr
    {
        #region 属性
        public const string MyPK = "MyPK";

        public const string FrmID = "FrmID";
        public const string FrmName = "FrmName";
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string FrmPKVal = "FrmPKVal";
        /// <summary>
        /// 节点
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 节点名称
        /// </summary>
        public const string NodeName = "NodeName";
        /// <summary>
        /// 送达否
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string RecNo = "RecNo";
        /// <summary>
        /// 记录人名称
        /// </summary>
        public const string RecName = "RecName";
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
        /// 部门编号
        /// </summary>
        public const string RecDeptNo = "RecDeptNo";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string RecDeptName = "RecDeptName";
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// FID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// FlowNo
        /// </summary>
        public const string FlowNo = "FlowNo";
        /// <summary>
        /// 名称
        /// </summary>
        public const string FlowName = "FlowName";
        public const string Title = "Title";
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
    public class FrmPrintDB : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 路径
        /// </summary>
        public string FullPath
        {
            get
            {
                return this.GetValStringByKey(FrmPrintDBAttr.FullPath);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.FullPath, value);
            }
        }
        public string FileName
        {
            get
            {
                return this.GetValStringByKey(FrmPrintDBAttr.FileName);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.FileName, value);
            }
        }
        /// <summary>
        /// 参与人员
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStringByKey(FrmPrintDBAttr.Emps);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.Emps, value);
            }
        }
        
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(FrmPrintDBAttr.Title);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.Title, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(FrmPrintDBAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.FlowNo, value);
            }
        }
        /// <summary>
        /// 记录人
        /// </summary>
        public string RecNo
        {
            get
            {
                return this.GetValStringByKey(FrmPrintDBAttr.RecNo);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.RecNo, value);
            }
        }
        public string RecName
        {
            get
            {
                return this.GetValStringByKey(FrmPrintDBAttr.RecName);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.RecName, value);
            }
        }
        public string RecDeptNo
        {
            get
            {
                return this.GetValStringByKey(FrmPrintDBAttr.RecDeptNo);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.RecDeptNo, value);
            }
        }
        public string RecDeptName
        {
            get
            {
                return this.GetValStringByKey(FrmPrintDBAttr.RecDeptName);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.RecDeptName, value);
            }
        }
        /// <summary>
        /// 年月
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStrByKey(FrmPrintDBAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 FrmPKVal
        {
            get
            {
                return this.GetValInt64ByKey(FrmPrintDBAttr.FrmPKVal);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.FrmPKVal, value);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(FrmPrintDBAttr.FID);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.FID, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(FrmPrintDBAttr.NodeID);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.NodeID, value);
            }
        }
        public string NodeName
        {
            get
            {
                return this.GetValStringByKey(FrmPrintDBAttr.NodeName);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 单据打印时间
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(FrmPrintDBAttr.RDT);
            }
            set
            {
                this.SetValByKey(FrmPrintDBAttr.RDT, value);
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
        /// 打印的单据
        /// </summary>
        public FrmPrintDB() { }
        /// <summary>
        /// 打印的单据
        /// </summary>
        /// <param name="pk"></param>
        public FrmPrintDB(string pk)
            : base(pk)
        {
        }
        #endregion

        #region 重写方法.
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_FrmPrintDB", "打印的单据");

                map.AddMyPK(false);

                map.AddTBString(FrmPrintDBAttr.FrmID, null, "表单ID", false, false, 0, 50, 50);
                map.AddTBString(FrmPrintDBAttr.FrmName, null, "表单名字", false, false, 0, 50, 50);

                map.AddTBString(FrmPrintDBAttr.FrmPKVal, null, "实体主键值", false, false, 0, 4, 5);

                #region 流程信息.
                map.AddTBString(FrmPrintDBAttr.Title, null, "标题", false, false, 0, 900, 5);
                map.AddTBString(FrmPrintDBAttr.FlowNo, null, "流程编号", false, false, 0, 4, 5);
                map.AddTBString(FrmPrintDBAttr.FlowName, null, "流程名称", false, false, 0, 4, 5);
                map.AddTBString(FrmPrintDBAttr.NodeID, null, "节点", false, false, 0, 30, 5);
                map.AddTBString(FrmPrintDBAttr.NodeName, null, "节点名", false, false, 0, 30, 5);
                #endregion 流程信息.

                #region 打印人的信息.
                map.AddTBString(FrmPrintDBAttr.RecNo, null, "记录人", true, true, 0, 50, 5);
                map.AddTBString(FrmPrintDBAttr.RecName, null, "记录人名称", true, true, 0, 50, 5);

                map.AddTBString(FrmPrintDBAttr.RecDeptNo, null, "记录人部门编号", true, true, 0, 50, 5);
                map.AddTBString(FrmPrintDBAttr.RecDeptName, null, "记录人部门名称", true, true, 0, 50, 5);

                map.AddTBDateTime(FrmPrintDBAttr.RDT, "打印时间", true, true);
                #endregion 打印人的信息.


                map.AddTBString(FrmPrintDBAttr.FileName, null, "文件名称", false, false, 0, 2000, 5);

                map.AddTBString(FrmPrintDBAttr.FullPath, null, "FullPath", false, false, 0, 2000, 5);


                map.AddTBString(FrmPrintDBAttr.FK_NY, null, "隶属年月", true, true, 0, 50, 5);
                map.AddTBString(FrmPrintDBAttr.Emps, null, "可以查看的人", false, false, 0, 4000, 5);


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
            string path = SystemConfig.PathOfWebApp + (this.FileName);
            return path; 
        }
        /// <summary>
        /// 打开pdf
        /// </summary>
        /// <returns></returns>
        public string DoOpenPDF()
        {
            string path = SystemConfig.PathOfWebApp + (this.FileName);
            return path;
        }
    }
	/// <summary>
	/// 单据s
	/// </summary>
	public class FrmPrintDBs : EntitiesMyPK
	{
		#region 构造方法属性
		/// <summary>
        /// 单据s
		/// </summary>
		public FrmPrintDBs(){}
		#endregion 

		#region 属性
		/// <summary>
        /// 单据
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new FrmPrintDB();
			}
		}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmPrintDB> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmPrintDB>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmPrintDB> Tolist()
        {
            System.Collections.Generic.List<FrmPrintDB> list = new System.Collections.Generic.List<FrmPrintDB>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmPrintDB)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
