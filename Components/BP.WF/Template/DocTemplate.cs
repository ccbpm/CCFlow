using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;

namespace BP.WF.Template
{
    /// <summary>
    /// 属性
    /// </summary>
    public class DocTemplateAttr : BP.En.EntityNoNameAttr
    {
        /// <summary>
        /// 路径
        /// </summary>
        public const string TempFilePath = "TempFilePath";
        /// <summary>
        /// NodeID
        /// </summary>
        public const string NodeID = "NodeID";
    }
    /// <summary>
    /// 单据模板
    /// </summary>
    public class DocTemplate : EntityNoName
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
                this.SetValByKey(DocTemplateAttr.TempFilePath, value);
            }
        }
        /// <summary>
        /// 打开的连接
        /// </summary>
        public string TempFilePath
        {
            get
            {
                string s = this.GetValStrByKey(DocTemplateAttr.TempFilePath);
                if (s == "" || s == null)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(DocTemplateAttr.TempFilePath, value);
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
                return this.GetValIntByKey(DocTemplateAttr.NodeID);
            }
            set
            {
                this.SetValByKey(DocTemplateAttr.NodeID, value);
            }
        }
     
        #endregion

        #region 构造函数
        /// <summary>
        /// 单据模板
		/// </summary>
		public DocTemplate() { }
        public DocTemplate(string no) : base(no.Replace("\n", "").Trim())
        {
        }
        /// <summary>
        /// 获得单据文件流
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public byte[] GenerTemplateFile()
        {
            byte[] bytes = BP.DA.DBAccess.GetByteFromDB(this.EnMap.PhysicsTable, "No", this.No, "DBFile");
            if (bytes != null)
                return bytes;

            //如果没有找到，就看看默认的文件是否有.
            string tempExcel = BP.Sys.SystemConfig.PathOfDataUser + "CyclostyleFile\\" + this.No + ".rtf";
            if (System.IO.File.Exists(tempExcel) == false)
                tempExcel = BP.Sys.SystemConfig.PathOfDataUser + "CyclostyleFile\\Word单据模版定义演示.docx";

            bytes = BP.DA.DataType.ConvertFileToByte(tempExcel);
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
                Map map = new Map("WF_DocTemplate", "公文模板");

                map.Java_SetCodeStruct("6");

                map.AddTBStringPK(DocTemplateAttr.No, null, "No", true, true, 6, 6, 20);
                map.AddTBString(DocTemplateAttr.Name, null, "名称", true, false, 0, 200, 20);
                map.AddTBString(DocTemplateAttr.TempFilePath, null, "模板路径", true, false, 0, 200, 20);
                map.AddTBInt(DocTemplateAttr.NodeID, 0, "NodeID", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 单据模板s
    /// </summary>
    public class DocTemplates : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DocTemplate();
            }
        }
        /// <summary>
        /// 单据模板
        /// </summary>
        public DocTemplates()
        {
        }
        #endregion

        #region 查询与构造
        /// <summary>
        /// 按节点查询
        /// </summary>
        /// <param name="nd"></param>
        public DocTemplates(Node nd)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(DocTemplateAttr.NodeID, nd.NodeID);
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
        public DocTemplates(string fk_flow)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(DocTemplateAttr.NodeID, "SELECT NodeID FROM WF_Node WHERE fk_flow='" + fk_flow + "'");
            qo.DoQuery();
        }
        /// <summary>
        /// 按节点查询
        /// </summary>
        /// <param name="fk_node">节点ID</param>
        public DocTemplates(int fk_node)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(DocTemplateAttr.NodeID, fk_node);
            qo.DoQuery();
        }
        #endregion 查询与构造

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DocTemplate> ToJavaList()
        {
            return (System.Collections.Generic.IList<DocTemplate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DocTemplate> Tolist()
        {
            System.Collections.Generic.List<DocTemplate> list = new System.Collections.Generic.List<DocTemplate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DocTemplate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}
