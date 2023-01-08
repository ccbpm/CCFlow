using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.WF.Template.Frm
{
    
    /// <summary>
    /// 打印模板属性
    /// </summary>
    public class FrmPrintTemplateAttr : BP.En.EntityMyPKAttr
    {
        /// <summary>
        /// 路径
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 路径
        /// </summary>
        public const string TempFilePath = "TempFilePath";
        /// <summary>
        /// NodeID
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FlowNo = "FlowNo";
        /// <summary>
        /// 字段名称
        /// </summary>
        public const string KeyOfEn = "KeyOfEn";
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
        public const string PrintFileType = "PrintFileType";
        /// <summary>
        /// 二维码生成方式
        /// </summary>
        public const string QRModel = "QRModel";
        /// <summary>
        /// 文件打开方式
        /// </summary>
        public const string PrintOpenModel = "PrintOpenModel";
        /// <summary>
        /// 表单的ID
        /// </summary>
        public const string FrmID = "FrmID";
    }
    /// <summary>
    /// 打印模板
    /// </summary>
    public class FrmPrintTemplate : EntityMyPK
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
        public new string MyPK
        {
            get
            {
                string no = this.GetValStrByKey("MyPK");
                no = no.Replace("\n", "");
                no = no.Replace(" ", "");
                return no;
            }
            set
            {
                this.SetValByKey("MyPK", value);
                this.SetValByKey(FrmPrintTemplateAttr.TempFilePath, value);
            }
        }
        /// <summary>
        /// 生成的单据类型
        /// </summary>
        public PrintFileType HisPrintFileType
        {
            get
            {
                return (PrintFileType)this.GetValIntByKey(FrmPrintTemplateAttr.PrintFileType);
            }
            set
            {
                this.SetValByKey(FrmPrintTemplateAttr.PrintFileType, (int)value);
            }
        }
        /// <summary>
        /// 二维码生成方式
        /// </summary>
        public QRModel QRModel
        {
            get
            {
                return (QRModel)this.GetValIntByKey(FrmPrintTemplateAttr.QRModel);
            }
            set
            {
                this.SetValByKey(FrmPrintTemplateAttr.QRModel, (int)value);
            }
        }
        public TemplateFileModel TemplateFileModel
        {
            get
            {
                return (TemplateFileModel)this.GetValIntByKey(FrmPrintTemplateAttr.TemplateFileModel);
            }
            set
            {
                this.SetValByKey(FrmPrintTemplateAttr.TemplateFileModel, (int)value);
            }
        }

        /// <summary>
        /// 生成的单据打开方式
        /// </summary>
        public PrintOpenModel PrintOpenModel
        {
            get
            {
                return (PrintOpenModel)this.GetValIntByKey(FrmPrintTemplateAttr.PrintOpenModel);
            }
            set
            {
                this.SetValByKey(FrmPrintTemplateAttr.PrintOpenModel, (int)value);
            }
        }
        /// <summary>
        /// 打开的连接
        /// </summary>
        public string TempFilePath
        {
            get
            {
                string s = this.GetValStrByKey(FrmPrintTemplateAttr.TempFilePath);
                if (DataType.IsNullOrEmpty(s) == true)
                    return this.MyPK;
                return s;
            }
            set
            {
                this.SetValByKey(FrmPrintTemplateAttr.TempFilePath, value);
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
                return this.GetValIntByKey(FrmPrintTemplateAttr.NodeID);
            }
            set
            {
                this.SetValByKey(FrmPrintTemplateAttr.NodeID, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(FrmPrintTemplateAttr.Name);
            }
            set
            {
                this.SetValByKey(FrmPrintTemplateAttr.Name, value);
            }
        }
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(FrmPrintTemplateAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmPrintTemplateAttr.FrmID, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 打印模板
		/// </summary>
		public FrmPrintTemplate() { }
        /// <summary>
        /// 打印模板
        /// </summary>
        /// <param name="mypk">主键</param>
        public FrmPrintTemplate(string mypk) : base(mypk.Replace("\n", "").Trim())
        {
        }
        /// <summary>
        /// 获得单据文件流
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public byte[] GenerTemplateFile()
        {
            byte[] bytes = DBAccess.GetByteFromDB(this.EnMap.PhysicsTable, "MyPK", this.MyPK, "DBFile");
            if (bytes != null)
                return bytes;

            //如果没有找到，就看看默认的文件是否有.
            string tempExcel =  BP.Difference.SystemConfig.PathOfDataUser + "CyclostyleFile/" + this.MyPK + ".rtf";
            if (System.IO.File.Exists(tempExcel) == false)
                tempExcel =  BP.Difference.SystemConfig.PathOfDataUser + "CyclostyleFile/Word单据模版定义演示.docx";

            bytes = DataType.ConvertFileToByte(tempExcel);
            return bytes;
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
                Map map = new Map("Sys_FrmPrintTemplate", "打印模板");
                map.IndexField = FrmPrintTemplateAttr.FrmID;

                map.AddMyPK();

                map.AddTBString(FrmPrintTemplateAttr.Name, null, "名称", true, false, 0, 200, 20);
                map.AddTBString(FrmPrintTemplateAttr.TempFilePath, null, "模板路径", true, false, 0, 200, 20);

                map.AddTBInt(FrmPrintTemplateAttr.NodeID, 0, "节点ID", true, false);
                map.AddTBString(FrmPrintTemplateAttr.FlowNo, null, "流程编号", true, false, 0, 200, 20);

                map.AddTBString(FrmPrintTemplateAttr.FrmID, null, "表单ID", false, false, 0, 60, 60);

                map.AddDDLSysEnum(FrmPrintTemplateAttr.TemplateFileModel, 0, "模版模式", true, false,
                 FrmPrintTemplateAttr.TemplateFileModel, "@0=rtf模版@1=VSTO模式的word模版@2=VSTO模式的Excel模版@3=Wps模板");

                map.AddDDLSysEnum(FrmPrintTemplateAttr.PrintFileType, 0, "生成的文件类型", true, false,
                    "PrintFileType", "@0=Word@1=PDF@2=Excel@3=Html");

                map.AddDDLSysEnum(FrmPrintTemplateAttr.PrintOpenModel, 0, "生成的文件打开方式", true, false,
                    "PrintOpenModel", "@0=下载本地@1=在线打开");

                map.AddDDLSysEnum(FrmAttachmentAttr.AthSaveWay, 0, "实例的保存方式", true, true, FrmAttachmentAttr.AthSaveWay,
               "@0=保存到web服务器@1=保存到数据库Sys_FrmPrintDB@2=ftp服务器");


                map.AddDDLSysEnum(FrmPrintTemplateAttr.QRModel, 0, "二维码生成方式", true, false,
                   FrmPrintTemplateAttr.QRModel, "@0=不生成@1=生成二维码");

                map.AddTBInt("Idx", 0, "Idx", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
                this.MyPK = DBAccess.GenerGUID();
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 打印模板s
    /// </summary>
    public class FrmPrintTemplates : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmPrintTemplate();
            }
        }
        /// <summary>
        /// 打印模板
        /// </summary>
        public FrmPrintTemplates()
        {
        }
        public FrmPrintTemplates(int nodeID)
        {
            this.Retrieve(FrmPrintTemplateAttr.NodeID, nodeID);
        }
        public FrmPrintTemplates(string flowNo)
        {
            this.Retrieve(FrmPrintTemplateAttr.FlowNo, flowNo);
        }
        #endregion


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmPrintTemplate> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmPrintTemplate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmPrintTemplate> Tolist()
        {
            System.Collections.Generic.List<FrmPrintTemplate> list = new System.Collections.Generic.List<FrmPrintTemplate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmPrintTemplate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}
