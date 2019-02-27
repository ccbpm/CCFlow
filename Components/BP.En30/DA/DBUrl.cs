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
		AppCenterDSN,
		/// <summary>
		/// 1号连接
		/// </summary>
		DBAccessOfOracle1,
		/// <summary>
		/// 2号连接
		/// </summary>
		DBAccessOfOracle2,
		/// <summary>
		/// 1号连接
		/// </summary>
        DBAccessOfMSSQL1,
        /// <summary>
        /// 2号连接
        /// </summary>
		DBAccessOfMSSQL2,
		/// <summary>
		/// access的连接．
		/// </summary>
		DBAccessOfOLE,
		/// <summary>
		/// ODBC连接
		/// </summary>
		DBAccessOfODBC,
        /// <summary>
        /// 数据源连接
        /// </summary>
        DBSrc
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
        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="type">连接type</param>
        public DBUrl(string dbSrc)
        {
            //数据库类型.
            this.DBUrlType =  DA.DBUrlType.DBSrc;

            //数据库连接.
            this.HisDBSrc = new BP.Sys.SFDBSrc(dbSrc);
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
					case DBUrlType.DBAccessOfMSSQL1:
                    case DBUrlType.DBAccessOfMSSQL2:
						return DBType.MSSQL;
					case DBUrlType.DBAccessOfOLE:
						return DBType.Access;
					case DBUrlType.DBAccessOfOracle1:
                    case DBUrlType.DBAccessOfOracle2:
						return DBType.Oracle ;
                    case DBUrlType.DBSrc:
                        return this.HisDBSrc.HisDBType;
					default:
						throw new Exception("不明确的连接");
				}
			}
		}
	}

}
