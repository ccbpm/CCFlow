using BP.Sys;
using System;
using System.Data.SqlClient;

namespace BP.DA
{
	/// <summary>
	///　连接到哪个库上．
	///  他们存放在 web.config 的列表内．
	/// </summary>
	public enum DBUrlType
	{ 
		/// <summary>
		/// 主应用程序
		/// </summary>
		AppCenterDSN 
	}
	/// <summary>
	/// DBUrl 的摘要说明。
	/// </summary>
	public class DBUrl
	{
		/// <summary>
		/// 连接
		/// </summary>
		public DBUrl()
		{
		} 
		/// <summary>
		/// 连接
		/// </summary>
		/// <param name="type">连接type</param>
		public DBUrl(DBUrlType type)
		{
			this.DBUrlType=type;
		}
     
        #region 其他数据源.
        private BP.Sys.SFDBSrc _HisDBSrc = null;
        public BP.Sys.SFDBSrc HisDBSrc
        {
            get
            {
                return _HisDBSrc;
            }
            set
            {
                _HisDBSrc = value;
            }
        }
        #endregion 其他数据源.

        /// <summary>
		/// 默认值
		/// </summary>
		public DBUrlType  _DBUrlType=DBUrlType.AppCenterDSN;
		/// <summary>
		/// 要连接的到的库。
		/// </summary>
		public DBUrlType DBUrlType
		{
			get
			{
				return _DBUrlType;
			}
			set
			{
				_DBUrlType=value;
			}
		}
        public string DBVarStr
        {
            get
            {
                switch (this.DBType)
                {
                    case DBType.Oracle:
                    case DBType.PostgreSQL:
					case DBType.UX:
						return ":";
                    case DBType.MySQL:
                    case DBType.MSSQL:
                        return "@";
                    case DBType.Informix:
                        return "?";
                    default:
                        return "@";
                }
            }
        }
		/// <summary>
		/// 数据库类型
		/// </summary>
		public DBType DBType
		{
			get
			{
				switch(this.DBUrlType)
				{
					case DBUrlType.AppCenterDSN:
						return DBAccess.AppCenterDBType ; 
					default:
						throw new Exception("不明确的连接");
				}
			}
		}
	}

}
