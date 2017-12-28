using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.XML;
namespace BP.HTTP.Xml
{
	/// <summary>
	/// 属性
	/// </summary>
    public class RunSQLAttr
    {
        public const string Url = "Url";
        public const string FK_Img = "FK_Img";
        public const string Para = "Para";
        public const string ValType = "ValType";
        public const string Val = "Val";
        public const string Encode = "Encode";
    }
	/// <summary>
	/// AD 的摘要说明。
	/// 考核编号的数据元素
	/// 1，它是 AD 的一个明细。
	/// 2，它表示一个数据元素。
	/// </summary>
	public class RunSQL:XmlEnNoName
	{
		#region 构造
		public RunSQL()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new RunSQLs();
            }
        }
		#endregion
	}
	/// <summary>
    /// RunSQL
	/// </summary>
    public class RunSQLs : XmlEns
    {
        #region 构造
        /// <summary>
        /// 考核编号的数据元素
        /// </summary>
        public RunSQLs() { }
     
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new RunSQL();
            }
        }
        /// <summary>
        /// 文件
        /// </summary>
        public override string File
        {
            get
            {
                return SystemConfig.PathOfWebApp + "\\RunSQL\\RunSQL.xml";
            }
        }
        /// <summary>
        /// 物理表名
        /// </summary>
        public override string TableName
        {
            get
            {
                return "RunSQL";
            }
        }
        
        #endregion
    }
}
