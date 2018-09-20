using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys.XML;


namespace BP.Sys.XML
{
	/// <summary>
	/// 属性
	/// </summary>
    public class AttrDescAttr
    {
        /// <summary>
        /// 过错行为
        /// </summary>
        public const string Attr = "Attr";
        /// <summary>
        /// 表达式
        /// </summary>
        public const string Desc = "Desc";
        /// <summary>
        /// 描述
        /// </summary>
        public const string For = "For";
        /// <summary>
        /// 高度
        /// </summary>
        public const string Height = "Height";
        /// <summary>
        /// 是否接受文件数据
        /// </summary>
        public const string IsAcceptFile = "IsAcceptFile";
    }
	/// <summary>
	/// AttrDesc 的摘要说明，属性的配置。
	/// </summary>
	public class AttrDesc:XmlEn
	{
		#region 属性
		public string Attr
		{
			get
			{
				return this.GetValStringByKey(AttrDescAttr.Attr);
			}
		}
		public string For
		{
			get
			{
				return this.GetValStringByKey(AttrDescAttr.For);
			}
		}
        public string Desc
        {
            get
            {
                return this.GetValStringByKey(AttrDescAttr.Desc);
            }
        }
        public bool IsAcceptFile
        {
            get
            {
                string s = this.GetValStringByKey(AttrDescAttr.IsAcceptFile);
                if (s == null || s == "" || s=="0")
                    return false;

                return true;
            }
        }
		public int Height
		{
            get
            {
                string str = this.GetValStringByKey(AttrDescAttr.Height);
                if (str == null || str == "")
                    return 200;
                return int.Parse(str);
            }
		}
		#endregion

		#region 构造
		public AttrDesc()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new AttrDescs();
			}
		}
		#endregion
	}
	/// <summary>
	/// 属性集合
	/// </summary>
	public class AttrDescs:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核过错行为的数据元素
		/// </summary>
		public AttrDescs()
		{
		}
		public AttrDescs(string enName)
		{
			this.RetrieveBy(AttrDescAttr.For, enName);
		}
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new AttrDesc();
			}
		}
		public override string File
		{
			get
			{
				return SystemConfig.PathOfXML+"\\Ens\\AttrDesc\\";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "Item";
			}
		}
		public override Entities RefEns
		{
			get
			{
				return null;
			}
		}
		#endregion
	}
}
