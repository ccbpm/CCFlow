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
        public const string FilePath = "FilePath";
        /// <summary>
        /// NodeID
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
    }
    /// <summary>
    /// 公文模板
    /// </summary>
    public class DocTemplate : EntityNoName
    {
        #region  属性
        /// <summary>
        /// 文件
        /// </summary>
        public byte[] FileBytes
        {
            get
            {
                //转化为字节.
                byte[] bytes = null;
                bytes = BP.DA.DataType.ConvertFileToByte(this.FilePath);
                return bytes;
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
                return this.GetValStrByKey(DocTemplateAttr.No);
            }
            set
            {
                this.SetValByKey(DocTemplateAttr.No, value);
            }
        }
        /// <summary>
        /// 路径
        /// </summary>
        public string FilePath
        {
            get
            {
                return this.GetValStrByKey(DocTemplateAttr.FilePath);
            }
            set
            {
                this.SetValByKey(DocTemplateAttr.FilePath, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(DocTemplateAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(DocTemplateAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStrByKey(DocTemplateAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(DocTemplateAttr.FK_Flow, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 公文模板
		/// </summary>
		public DocTemplate() { }
        /// <summary>
        /// 公文模板
        /// </summary>
        /// <param name="no"></param>
        public DocTemplate(string no) : base(no)
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

                Map map = new Map("WF_DocTemplate", "公文模板");
                //map.Java_SetCodeStruct("6");

                map.AddTBStringPK(DocTemplateAttr.No, null, "No", true, true, 1, 50, 20);
                map.AddTBString(DocTemplateAttr.Name, null, "名称", true, false, 0, 200, 20);
                map.AddTBString(DocTemplateAttr.FilePath, null, "模板路径", true, false, 0, 200, 20);
                map.AddTBInt(DocTemplateAttr.FK_Node, 0, "节点ID", true, false);
                map.AddTBString(DocTemplateAttr.FK_Flow, null, "流程编号", true, false, 0, 200, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 公文模板s
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
        /// 公文模板
        /// </summary>
        public DocTemplates()
        {
        }
        #endregion

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
