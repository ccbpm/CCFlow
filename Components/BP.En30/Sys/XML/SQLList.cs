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
    public class SQLListAttr
    {
        /// <summary>
        /// 编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// SQL
        /// </summary>
        public const string SQL = "SQL";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";

    }
	/// <summary>
	/// SQLList 的摘要说明，属性的配置。
	/// </summary>
	public class SQLList:XmlEn
	{
		#region 属性
        public string No
        {
            get
            {
                return this.GetValStringByKey(SQLListAttr.No);
            }
        }
        public string SQL
        {
            get
            {
                return this.GetValStringByKey(SQLListAttr.SQL);
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
		{
			get
			{
                return this.GetValStringByKey(SQLListAttr.Note);
			}
		}
		#endregion

		#region 构造
        /// <summary>
        /// 查询
        /// </summary>
		public SQLList()
		{
		}
        /// <summary>
        /// 按照SQL来查询
        /// </summary>
        /// <param name="no"></param>
        public SQLList(string no)
        {
            this.RetrieveByPK("No", no);
        }
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new SQLLists();
			}
		}
		#endregion
	}
	/// <summary>
	/// 属性集合
	/// </summary>
	public class SQLLists:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核过错行为的数据元素
		/// </summary>
		public SQLLists()
		{
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
				return new SQLList();
			}
		}
		public override string File
		{
			get
			{
                return SystemConfig.PathOfXML + "\\SQLList.xml";
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
