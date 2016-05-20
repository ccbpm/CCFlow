using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.Template.XML
{
    /// <summary>
    /// 表单事件
    /// </summary>
	public class FrmEventXml:XmlEn
	{
		#region 属性
        public string No
        {
            get
            {
                return this.GetValStringByKey("No");
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
		/// <summary>
		/// 图片
		/// </summary>
		public string Img
		{
			get
			{
				return  this.GetValStringByKey("Img") ;
			}
		}
        public string Title
        {
            get
            {
                return this.GetValStringByKey("Title");
            }
        }
        public string Url
        {
            get
            {
                 string url=this.GetValStringByKey("Url");
                 if (url == "")
                     url = "javascript:" + this.GetValStringByKey("OnClick") ;
                 return url;
            }
        }
		#endregion

		#region 构造
		/// <summary>
		/// 表单事件
		/// </summary>
		public FrmEventXml()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new FrmEventXmls();
			}
		}
		#endregion
	}
	/// <summary>
    /// 表单事件
	/// </summary>
	public class FrmEventXmls:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public FrmEventXmls() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new FrmEventXml();
			}
		}
		public override string File
		{
			get
			{
               // return SystemConfig.PathOfWebApp + "\\WF\\MapDef\\Style\\XmlDB.xml";

                return SystemConfig.PathOfData + "\\XML\\XmlDB.xml";

			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
                return "FrmEvent";
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
