using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Sys.XML;

namespace BP.Web.Port.Xml
{
    public class HelpAttr
    {
        /// <summary>
        /// 编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        public const string HelpFile = "HelpFile";
        public const string Img = "Img";

    }
	/// <summary>
	/// 
	/// </summary>
	public class Help:XmlEn
	{
		#region 属性
        public string Parent
        {
            get
            {
                switch (this.Grade)
                {
                    case 1:
                        return null;
                    case 2:
                        return this.No.Substring(0, 2);
                    case 3:
                        return this.No.Substring(0, 4);
                    default:
                        return null;
                }
            }
        }
        public int Grade
        {
            get
            {
                return this.No.Length / 2;
            }
        }
        public string Img
        {
            get
            {
                string str = this.GetValStringByKey(HelpAttr.Img);
                if (str == null || str == "")
                    if (this.No.Length >= 4)
                        return "/WF/Images/Pub/BillOpen.gif";
                    else
                        return "/WF/Images/Pub/Bill.gif";
                else
                    return str;
            }
        }
        public string HelpFile
        {
            get
            {
                return this.GetValStringByKey(HelpAttr.HelpFile);
            }
        }
		/// <summary>
		/// 编号
		/// </summary>
		public string No
		{
			get
			{
				return this.GetValStringByKey(HelpAttr.No).Trim();
			}
		}
		/// <summary>
		/// 名称
		/// </summary>
		public string Name
		{
			get
			{
				return this.GetValStringByKey(HelpAttr.Name);
			}
		}
		#endregion

		#region 构造
		public Help()
		{
		}
		/// <summary>
		/// 编号
		/// </summary>
		/// <param name="no"></param>
		public Help(string no)
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new Helps();
            }
        }
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class Helps:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
		public Helps(){}
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new Help();
			}
		}
		public override string File
		{
			get
			{
                return SystemConfig.PathOfWebApp + "\\Helper\\Help.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "Help";
			}
		}
		public override Entities RefEns
		{
			get
			{
				return null; //new BP.ZF1.Helps();
			}
		}
		#endregion
		 
	}
}
