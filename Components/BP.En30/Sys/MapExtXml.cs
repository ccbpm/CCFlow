using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;


namespace BP.Sys
{
    public class MapExtXmlList
    {
        /// <summary>
        /// 获取与设置外部数据
        /// </summary>
        public const string AutoFull = "AutoFull";
        /// <summary>
        /// 活动菜单
        /// </summary>
        public const string ActiveDDL = "ActiveDDL";
        /// <summary>
        /// 输入验证
        /// </summary>
        public const string InputCheck = "InputCheck";
        /// <summary>
        /// 文本框自动填充
        /// </summary>
        public const string TBFullCtrl = "TBFullCtrl";
        /// <summary>
        /// Pop返回值
        /// </summary>
        public const string PopVal = "PopVal";
        /// <summary>
        /// Func
        /// </summary>
        public const string Func = "Func";
        /// <summary>
        /// (动态的)填充下拉框
        /// </summary>
        public const string AutoFullDLL = "AutoFullDLL";
        /// <summary>
        /// 下拉框自动填充
        /// </summary>
        public const string DDLFullCtrl = "DDLFullCtrl";
        /// <summary>
        /// 表单装载填充
        /// </summary>
        public const string PageLoadFull = "PageLoadFull";
        /// <summary>
        /// 发起流程
        /// </summary>
        public const string StartFlow = "StartFlow";
        /// <summary>
        /// 超链接.
        /// </summary>
        public const string Link = "Link";
        /// <summary>
        /// 自动生成编号
        /// </summary>
        public const string AotuGenerNo = "AotuGenerNo";
        /// <summary>
        /// 正则表达式
        /// </summary>
        public const string RegularExpression = "RegularExpression";


        public const string WordFrm = "WordFrm";
        public const string ExcelFrm = "ExcelFrm";

        /// <summary>
        /// 特别字段特殊用户权限
        /// </summary>
        public const string SepcFiledsSepcUsers = "SepcFiledsSepcUsers";


    }
	public class MapExtXml:XmlEnNoName
	{
		#region 属性
        public new string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
        public string URL
        {
            get
            {
                return this.GetValStringByKey("URL");
            }
        }
		#endregion

		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
		public MapExtXml()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new MapExtXmls();
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class MapExtXmls:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public MapExtXmls() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new MapExtXml();
			}
		}
		public override string File
		{
			get
			{
                return SystemConfig.PathOfXML + "MapExt.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "FieldExt";
			}
		}
		public override Entities RefEns
		{
			get
			{
				return null; //new BP.ZF1.AdminTools();
			}
		}
		#endregion
		 
	}
}
