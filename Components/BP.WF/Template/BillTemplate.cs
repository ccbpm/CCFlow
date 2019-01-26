using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;

namespace BP.WF.Template
{
    /// <summary>
    /// 模版类型
    /// </summary>
    public enum TemplateFileModel
    {
        /// <summary>
        /// 旧版本的rtf模版格式
        /// </summary>
        RTF,
        /// <summary>
        /// Word模版格式
        /// </summary>
        VSTOForWord,
        /// <summary>
        /// Excel模版格式
        /// </summary>
        VSTOForExcel
    }
    /// <summary>
    /// 二维码生成方式
    /// </summary>
    public enum QRModel
    {
        /// <summary>
        /// 不生成
        /// </summary>
        None,
        /// <summary>
        /// 生成
        /// </summary>
        Gener
    }
    /// <summary>
    /// 生成的类型
    /// </summary>
    public enum BillFileType
    {
        /// <summary>
        /// Word
        /// </summary>
        Word=0,
        PDF=1,
        Excel=2,
        Html=3,
        RuiLang=5
    }
    /// <summary>
    /// 生成的文件打开方式
    /// </summary>
    public enum BillOpenModel
    {
        /// <summary>
        /// 下载保存
        /// </summary>
        DownLoad=0,
        /// <summary>
        /// 在线WebOffice打开
        /// </summary>
        WebOffice=1
    }
    /// <summary>
    /// 单据模板属性
    /// </summary>
    public class BillTemplateAttr:BP.En.EntityNoNameAttr
    {
        /// <summary>
        /// 路径
        /// </summary>
        public const string TempFilePath = "TempFilePath";
        /// <summary>
        /// NodeID
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 为生成单据使用
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 单据类型
        /// </summary>
        public const string TemplateFileModel = "TemplateFileModel";
        /// <summary>
        /// 是否生成PDF
        /// </summary>
        public const string BillFileType = "BillFileType";
        /// <summary>
        /// 二维码生成方式
        /// </summary>
        public const string QRModel = "QRModel";
        /// <summary>
        /// 文件打开方式
        /// </summary>
        public const string BillOpenModel = "BillOpenModel";
    }
	/// <summary>
	/// 单据模板
	/// </summary>
	public class BillTemplate : EntityNoName
    {
        #region  属性
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
        /// 编号
        /// </summary>
        public new string No
        {
            get
            {
                string no = this.GetValStrByKey("No");
                no = no.Replace("\n", "");
                no = no.Replace(" ", "");
                return no;
            }
            set
            {
                this.SetValByKey("No", value);
                this.SetValByKey(BillTemplateAttr.TempFilePath, value);
            }
        }
        /// <summary>
        /// 生成的单据类型
        /// </summary>
        public BillFileType HisBillFileType
        {
            get
            {
                return (BillFileType)this.GetValIntByKey(BillTemplateAttr.BillFileType);
            }
            set
            {
                this.SetValByKey(BillTemplateAttr.BillFileType, (int)value);
            }
        }
        /// <summary>
        /// 二维码生成方式
        /// </summary>
        public QRModel QRModel
        {
            get
            {
                return (QRModel)this.GetValIntByKey(BillTemplateAttr.QRModel);
            }
            set
            {
                this.SetValByKey(BillTemplateAttr.QRModel, (int)value);
            }
        }
        public TemplateFileModel TemplateFileModel
        {
            get
            {
                return (TemplateFileModel)this.GetValIntByKey(BillTemplateAttr.TemplateFileModel);
            }
            set
            {
                this.SetValByKey(BillTemplateAttr.TemplateFileModel, (int)value);
            }
        }
        
        /// <summary>
        /// 生成的单据打开方式
        /// </summary>
        public BillOpenModel BillOpenModel
        {
            get
            {
                return (BillOpenModel)this.GetValIntByKey(BillTemplateAttr.BillOpenModel);
            }
            set
            {
                this.SetValByKey(BillTemplateAttr.BillOpenModel, (int)value);
            }
        }
        /// <summary>
        /// 打开的连接
        /// </summary>
        public string TempFilePath
        {
            get
            {
                string s= this.GetValStrByKey(BillTemplateAttr.TempFilePath);
                if (s == "" || s == null)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(BillTemplateAttr.TempFilePath, value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                Node nd = new Node(this.NodeID);
                return nd.Name;
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(BillTemplateAttr.NodeID);
            }
            set
            {
                this.SetValByKey(BillTemplateAttr.NodeID, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 单据模板
		/// </summary>
		public BillTemplate(){}
        public BillTemplate(string no):base(no.Replace( "\n","" ).Trim() ) 
        {
        }
        /// <summary>
        /// 获得Excel文件流
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public void GenerTemplateFile(ref byte[] bytes)
        {
            bytes = BP.DA.DBAccess.GetByteFromDB(this.EnMap.PhysicsTable, "No", this.No, "DBFile");
            if (bytes != null)
                return;

            //如果没有找到，就看看默认的文件是否有.
            string tempExcel = BP.Sys.SystemConfig.PathOfDataUser + "CyclostyleFile\\" + this.No + ".rtf";
            if (System.IO.File.Exists(tempExcel) == false)
                tempExcel = BP.Sys.SystemConfig.PathOfDataUser + "CyclostyleFile\\Word单据模版定义演示.docx";

            bytes = BP.DA.DataType.ConvertFileToByte(tempExcel);
            return;
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
                Map map = new Map("WF_BillTemplate", "单据模板");
                
                map.Java_SetCodeStruct("6");

                map.AddTBStringPK(BillTemplateAttr.No, null, "No", true, false, 1, 190, 6);
                map.AddTBString(BillTemplateAttr.Name, null, "Name", true, false, 0, 200, 20);
                map.AddTBString(BillTemplateAttr.TempFilePath, null, "模板路径", true, false, 0, 200, 20);
                map.AddTBInt(BillTemplateAttr.NodeID, 0, "NodeID", true, false);

                map.AddDDLSysEnum(BillTemplateAttr.BillFileType, 0, "生成的文件类型", true, false,
                    "BillFileType","@0=Word@1=PDF@2=Excel(未完成)@3=Html(未完成)");

                map.AddDDLSysEnum(BillTemplateAttr.BillOpenModel, 0, "生成的文件打开方式", true, false,
                    "BillOpenModel", "@0=下载本地@1=在线WebOffice打开");

                map.AddDDLSysEnum(BillTemplateAttr.QRModel, 0, "二维码生成方式", true, false,
                   BillTemplateAttr.QRModel, "@0=不生成@1=生成二维码");


                map.AddDDLSysEnum(BillTemplateAttr.TemplateFileModel, 0, "模版模式", true, false,
                 BillTemplateAttr.TemplateFileModel, "@0=rtf模版@1=vsto模式的word模版@2=vsto模式的excel模版");

                map.AddTBString("Idx", null, "Idx", false, false, 0, 200, 20);

               // map.AddTBString(BillTemplateAttr.ExpField, null, "要排除的字段", false, false, 0, 800, 20);
               // map.AddTBString(BillTemplateAttr.ReplaceVal, null, "要替换的值", false, false, 0, 3000, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion 
	}
	/// <summary>
    /// 单据模板s
	/// </summary>
	public class BillTemplates: EntitiesNoName
	{
		#region 构造
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new BillTemplate();
			}
		}
		/// <summary>
		/// 单据模板
		/// </summary>
        public BillTemplates()
        {
        }
		#endregion

        #region 查询与构造
        /// <summary>
        /// 按节点查询
        /// </summary>
        /// <param name="nd"></param>
        public BillTemplates(Node nd)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(BillTemplateAttr.NodeID, nd.NodeID);
            if (nd.IsStartNode)
            {
                qo.addOr();
                qo.AddWhere("No", "SLHZ");
            }
            qo.DoQuery();
        }
        /// <summary>
        /// 按流程查询
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        public BillTemplates(string fk_flow)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(BillTemplateAttr.NodeID, "SELECT NodeID FROM WF_Node WHERE fk_flow='" + fk_flow + "'");
            qo.DoQuery();
        }
        /// <summary>
        /// 按节点查询
        /// </summary>
        /// <param name="fk_node">节点ID</param>
        public BillTemplates(int fk_node)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(BillTemplateAttr.NodeID, fk_node);
            qo.DoQuery();
        }
        #endregion 查询与构造

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<BillTemplate> ToJavaList()
        {
            return (System.Collections.Generic.IList<BillTemplate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<BillTemplate> Tolist()
        {
            System.Collections.Generic.List<BillTemplate> list = new System.Collections.Generic.List<BillTemplate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BillTemplate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
	
}
