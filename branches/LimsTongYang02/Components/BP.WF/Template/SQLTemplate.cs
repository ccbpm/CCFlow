using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;

namespace BP.WF.Template
{
    /// <summary>
    /// SQL模板属性
    /// </summary>
    public class SQLTemplateAttr:BP.En.EntityNoNameAttr
    {
        /// <summary>
        /// SQL
        /// </summary>
        public const string Docs = "Docs";
        /// <summary>
        /// NodeID
        /// </summary>
        public const string SQLType = "SQLType";
    }
	/// <summary>
	/// SQL模板
	/// </summary>
	public class SQLTemplate : EntityNoName
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
        /// 打开的连接
        /// </summary>
        public string Docs
        {
            get
            {
                string s= this.GetValStrByKey(SQLTemplateAttr.Docs);
                if (s == "" || s == null)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(SQLTemplateAttr.Docs, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// SQL模板
		/// </summary>
		public SQLTemplate(){}
        public SQLTemplate(string no):base(no.Replace( "\n","" ).Trim() ) 
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
                Map map = new Map("WF_SQLTemplate", "SQL模板");
                
                map.Java_SetCodeStruct("3");

                map.AddTBStringPK(SQLTemplateAttr.No, null, "编号", true, true, 3, 3, 3);
                map.AddDDLSysEnum(SQLTemplateAttr.SQLType, 0, "模版SQL类型", true, true, SQLTemplateAttr.SQLType,
                    "@0=方向条件@1=接受人规则@2=下拉框数据过滤@3=级联下拉框@4=PopVal开窗返回值@5=人员选择器人员选择范围");

                map.AddTBString(SQLTemplateAttr.Name, null, "SQL说明", true, false, 0, 200, 20,true);

                map.AddTBStringDoc(SQLTemplateAttr.Docs, null, "SQL模版", true, false,true);


                //查询条件.
                map.AddSearchAttr(SQLTemplateAttr.SQLType);

                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion 
	}
	/// <summary>
    /// SQL模板s
	/// </summary>
	public class SQLTemplates: EntitiesNoName
	{
		#region 构造
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new SQLTemplate();
			}
		}
		/// <summary>
		/// SQL模板
		/// </summary>
        public SQLTemplates()
        {
        }
		#endregion

        #region 查询与构造
        #endregion 查询与构造

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SQLTemplate> ToJavaList()
        {
            return (System.Collections.Generic.IList<SQLTemplate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SQLTemplate> Tolist()
        {
            System.Collections.Generic.List<SQLTemplate> list = new System.Collections.Generic.List<SQLTemplate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SQLTemplate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
	
}
