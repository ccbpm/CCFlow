using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Collections;
using System.IO;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Xml.Schema;
using BP.DA;
using BP.En;
using IBM.Data.Informix;
using MySql.Data.MySqlClient;

namespace BP.Sys
{
    /// <summary>
    /// 数据源类型
    /// </summary>
    public enum DBSrcType
    {
        /// <summary>
        /// 本机数据库
        /// </summary>
        Localhost = 0,
        /// <summary>
        /// SQL
        /// </summary>
        SQLServer = 1,
        /// <summary>
        /// Oracle
        /// </summary>
        Oracle = 2,
        /// <summary>
        /// MySQL
        /// </summary>
        MySQL = 3,
        /// <summary>
        /// Informix
        /// </summary>
        Informix = 4,
        /// <summary>
        /// WebService数据源
        /// </summary>
        WebServices = 100,
        /// <summary>
        /// 海尔的Dubbo服务.
        /// </summary>
        Dubbo=50
    }
    /// <summary>
    /// 数据源
    /// </summary>
    public class SFDBSrcAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 数据源类型
        /// </summary>
        public const string DBSrcType = "DBSrcType";
        /// <summary>
        /// 用户编号
        /// </summary>
        public const string UserID = "UserID";
        /// <summary>
        /// 密码
        /// </summary>
        public const string Password = "Password";
        /// <summary>
        /// IP地址
        /// </summary>
        public const string IP = "IP";
        /// <summary>
        /// 数据库名称
        /// </summary>
        public const string DBName = "DBName";
    }
    /// <summary>
    /// 数据源
    /// </summary>
    public class SFDBSrc : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 标签
        /// </summary>
        public string Icon
        {
            get
            {
                switch (this.DBSrcType)
                {
                    case Sys.DBSrcType.Localhost:
                        return "<img src='/WF/Img/DB.gif' />";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 是否是树形实体?
        /// </summary>
        public string UserID
        {
            get
            {
                return this.GetValStringByKey(SFDBSrcAttr.UserID);
            }
            set
            {
                this.SetValByKey(SFDBSrcAttr.UserID, value);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get
            {
                return this.GetValStringByKey(SFDBSrcAttr.Password);
            }
            set
            {
                this.SetValByKey(SFDBSrcAttr.Password, value);
            }
        }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName
        {
            get
            {
                return this.GetValStringByKey(SFDBSrcAttr.DBName);
            }
            set
            {
                this.SetValByKey(SFDBSrcAttr.DBName, value);
            }
        }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBSrcType DBSrcType
        {
            get
            {
                return (DBSrcType)this.GetValIntByKey(SFDBSrcAttr.DBSrcType);
            }
            set
            {
                this.SetValByKey(SFDBSrcAttr.DBSrcType, (int)value);
            }
        }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP
        {
            get
            {
                return this.GetValStringByKey(SFDBSrcAttr.IP);
            }
            set
            {
                this.SetValByKey(SFDBSrcAttr.IP, value);
            }
        }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBType HisDBType
        {
            get
            {
                switch (this.DBSrcType)
                {
                    case Sys.DBSrcType.Localhost:
                        return SystemConfig.AppCenterDBType;
                    case Sys.DBSrcType.SQLServer:
                        return DBType.MSSQL;
                    case Sys.DBSrcType.Oracle:
                        return DBType.Oracle;
                    case Sys.DBSrcType.MySQL:
                        return DBType.MySQL;
                    case Sys.DBSrcType.Informix:
                        return DBType.Informix;
                    default:
                        throw new Exception("@没有判断的数据库类型.");
                }
            }
        }
        #endregion

        #region 数据库访问方法
        /// <summary>
        /// 运行SQL返回数值
        /// </summary>
        /// <param name="sql">一行一列的SQL</param>
        /// <param name="isNullAsVal">如果为空，就返回制定的值.</param>
        /// <returns>要返回的值</returns>
        public int RunSQLReturnInt(string sql, int isNullAsVal)
        {
            DataTable dt = this.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return isNullAsVal;
            return int.Parse(dt.Rows[0][0].ToString());
        }
        /// <summary>
        /// 运行SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int RunSQL(string sql)
        {
            int i = 0;
            switch (this.DBSrcType)
            {
                case Sys.DBSrcType.Localhost:
                    return BP.DA.DBAccess.RunSQL(sql);
                case Sys.DBSrcType.SQLServer:
                    SqlConnection conn = new SqlConnection(this.ConnString);
                    SqlCommand cmd = null;

                    try
                    {
                        conn.Open();
                        cmd = new SqlCommand(sql, conn);
                        cmd.CommandType = CommandType.Text;
                        i = cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        conn.Close();
                        return i;
                    }
                    catch (Exception ex)
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                        if (cmd != null)
                            cmd.Dispose();
                        throw new Exception("RunSQL 错误，SQL=" + sql);
                    }
                case Sys.DBSrcType.Oracle:
                    OracleConnection connOra = new OracleConnection(this.ConnString);
                    OracleCommand cmdOra = null;

                    try
                    {
                        connOra.Open();
                        cmdOra = new OracleCommand(sql, connOra);
                        cmdOra.CommandType = CommandType.Text;
                        i = cmdOra.ExecuteNonQuery();
                        cmdOra.Dispose();
                        connOra.Close();
                        return i;
                    }
                    catch (Exception ex)
                    {
                        if (connOra.State == ConnectionState.Open)
                            connOra.Close();
                        if (cmdOra != null)
                            cmdOra.Dispose();
                        throw new Exception("RunSQL 错误，SQL=" + sql);
                    }
                case Sys.DBSrcType.MySQL:
                    MySqlConnection connMySQL = new MySqlConnection(this.ConnString);
                    MySqlCommand cmdMySQL = null;
                    try
                    {
                        connMySQL.Open();
                        cmdMySQL = new MySqlCommand(sql, connMySQL);
                        cmdMySQL.CommandType = CommandType.Text;
                        i = cmdMySQL.ExecuteNonQuery();
                        cmdMySQL.Dispose();
                        connMySQL.Close();
                        return i;
                    }
                    catch (Exception ex)
                    {
                        if (connMySQL.State == ConnectionState.Open)
                            connMySQL.Close();
                        if (cmdMySQL != null)
                            cmdMySQL.Dispose();
                        throw new Exception("RunSQL 错误，SQL=" + sql);
                    }
                case Sys.DBSrcType.Informix:
                    IfxConnection connIfx = new IfxConnection(this.ConnString);
                    IfxCommand cmdIfx = null;
                    try
                    {
                        connIfx.Open();
                        cmdIfx = new IfxCommand(sql, connIfx);
                        cmdIfx.CommandType = CommandType.Text;
                        i = cmdIfx.ExecuteNonQuery();
                        cmdIfx.Dispose();
                        connIfx.Close();
                        return i;
                    }
                    catch (Exception ex)
                    {
                        if (connIfx.State == ConnectionState.Open)
                            connIfx.Close();
                        if (cmdIfx != null)
                            cmdIfx.Dispose();
                        throw new Exception("RunSQL 错误，SQL=" + sql);
                    }
                default:
                    throw new Exception("@没有判断的支持的数据库类型.");
            }

            return 0;
        }
        /// <summary>
        /// 运行SQL
        /// </summary>
        /// <param name="runObj"></param>
        /// <returns></returns>
        public DataTable RunSQLReturnTable(string runObj)
        {
            return RunSQLReturnTable(runObj, new Paras() );
        }
        /// <summary>
        /// 运行SQL返回datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable RunSQLReturnTable(string runObj, Paras ps)
        {
            switch (this.DBSrcType)
            {
                case DBSrcType.Localhost: //如果是本机，直接在本机上执行.
                    return BP.DA.DBAccess.RunSQLReturnTable(runObj, ps);
                case DBSrcType.SQLServer: //如果是SQLServer.
                    SqlConnection connSQL = new SqlConnection(this.ConnString);
                    SqlDataAdapter ada = null;
                    SqlParameter myParameter = null;

                    try
                    {
                        connSQL.Open(); //打开.
                        ada = new SqlDataAdapter(runObj, connSQL);
                        ada.SelectCommand.CommandType = CommandType.Text;

                        // 加入参数
                        if (ps != null)
                        {
                            foreach (Para para in ps)
                            {
                                myParameter = new SqlParameter(para.ParaName, para.val);
                                myParameter.Size = para.Size;
                                ada.SelectCommand.Parameters.Add(myParameter);
                            }
                        }

                        DataTable oratb = new DataTable("otb");
                        ada.Fill(oratb);
                        ada.Dispose();
                        connSQL.Close();
                        return oratb;
                    }
                    catch (Exception ex)
                    {
                        if (ada != null)
                            ada.Dispose();
                        if (connSQL.State == ConnectionState.Open)
                            connSQL.Close();
                        throw new Exception("SQL=" + runObj + " Exception=" + ex.Message);
                    }
                case Sys.DBSrcType.Oracle:
                    OracleConnection oracleConn = new OracleConnection(ConnString);
                    OracleDataAdapter oracleAda = null;
                    OracleParameter myParameterOrcl = null;

                    try
                    {
                        oracleConn.Open();
                        oracleAda = new OracleDataAdapter(runObj, oracleConn);
                        oracleAda.SelectCommand.CommandType = CommandType.Text;

                        if (ps != null)
                        {
                            // 加入参数
                            foreach (Para para in ps)
                            {
                                myParameterOrcl = new OracleParameter(para.ParaName, para.val);
                                myParameterOrcl.Size = para.Size;
                                oracleAda.SelectCommand.Parameters.Add(myParameterOrcl);
                            }
                        }

                        DataTable oracleTb = new DataTable("otb");
                        oracleAda.Fill(oracleTb);
                        oracleAda.Dispose();
                        oracleConn.Close();
                        return oracleTb;
                    }
                    catch (Exception ex)
                    {
                        if (oracleAda != null)
                            oracleAda.Dispose();
                        if (oracleConn.State == ConnectionState.Open)
                            oracleConn.Close();
                        throw new Exception("SQL=" + runObj + " Exception=" + ex.Message);
                    }
                case Sys.DBSrcType.MySQL:
                    MySqlConnection mysqlConn = new MySqlConnection(ConnString);
                    MySqlDataAdapter mysqlAda = null;
                    MySqlParameter myParameterMysql = null;

                    try
                    {
                        mysqlConn.Open();
                        mysqlAda = new MySqlDataAdapter(runObj, mysqlConn);
                        mysqlAda.SelectCommand.CommandType = CommandType.Text;

                        if (ps != null)
                        {
                            // 加入参数
                            foreach (Para para in ps)
                            {
                                myParameterMysql = new MySqlParameter(para.ParaName, para.val);
                                myParameterMysql.Size = para.Size;
                                mysqlAda.SelectCommand.Parameters.Add(myParameterMysql);
                            }
                        }

                        DataTable mysqlTb = new DataTable("otb");
                        mysqlAda.Fill(mysqlTb);
                        mysqlAda.Dispose();
                        mysqlConn.Close();
                        return mysqlTb;
                    }
                    catch (Exception ex)
                    {
                        if (mysqlAda != null)
                            mysqlAda.Dispose();
                        if (mysqlConn.State == ConnectionState.Open)
                            mysqlConn.Close();
                        throw new Exception("SQL=" + runObj + " Exception=" + ex.Message);
                    }
                case Sys.DBSrcType.Informix:
                    IfxConnection ifxConn = new IfxConnection(ConnString);
                    IfxDataAdapter ifxAda = null;
                    IfxParameter myParameterIfx = null;

                    try
                    {
                        ifxConn.Open();
                        ifxAda = new IfxDataAdapter(runObj, ifxConn);
                        ifxAda.SelectCommand.CommandType = CommandType.Text;

                        if (ps != null)
                        {
                            // 加入参数
                            foreach (Para para in ps)
                            {
                                myParameterIfx = new IfxParameter(para.ParaName, para.val);
                                myParameterIfx.Size = para.Size;
                                ifxAda.SelectCommand.Parameters.Add(myParameterIfx);
                            }
                        }

                        DataTable ifxTb = new DataTable("otb");
                        ifxAda.Fill(ifxTb);
                        ifxAda.Dispose();
                        ifxConn.Close();
                        return ifxTb;
                    }
                    catch (Exception ex)
                    {
                        if (ifxAda != null)
                            ifxAda.Dispose();
                        if (ifxConn.State == ConnectionState.Open)
                            ifxConn.Close();
                        throw new Exception("SQL=" + runObj + " Exception=" + ex.Message);
                    }
                default:
                    break;
            }

            return null;
        }

        public DataTable RunSQLReturnTable(string sql, int startRecord, int recordCount)
        {
            switch (this.DBSrcType)
            {
                case DBSrcType.Localhost: //如果是本机，直接在本机上执行.
                    return BP.DA.DBAccess.RunSQLReturnTable(sql);
                case DBSrcType.SQLServer: //如果是SQLServer.
                    SqlConnection connSQL = new SqlConnection(this.ConnString);
                    SqlDataAdapter ada = null;
                    try
                    {
                        connSQL.Open(); //打开.
                        ada = new SqlDataAdapter(sql, connSQL);
                        ada.SelectCommand.CommandType = CommandType.Text;
                        DataTable oratb = new DataTable("otb");
                        ada.Fill(startRecord, recordCount, oratb);
                        ada.Dispose();
                        connSQL.Close();
                        return oratb;
                    }
                    catch (Exception ex)
                    {
                        if (ada != null)
                            ada.Dispose();
                        if (connSQL.State == ConnectionState.Open)
                            connSQL.Close();
                        throw new Exception("SQL=" + sql + " Exception=" + ex.Message);
                    }
                case Sys.DBSrcType.Oracle:
                    OracleConnection oracleConn = new OracleConnection(ConnString);
                    OracleDataAdapter oracleAda = null;

                    try
                    {
                        oracleConn.Open();
                        oracleAda = new OracleDataAdapter(sql, oracleConn);
                        oracleAda.SelectCommand.CommandType = CommandType.Text;
                        DataTable oracleTb = new DataTable("otb");
                        oracleAda.Fill(startRecord, recordCount, oracleTb);
                        oracleAda.Dispose();
                        oracleConn.Close();
                        return oracleTb;
                    }
                    catch (Exception ex)
                    {
                        if (oracleAda != null)
                            oracleAda.Dispose();
                        if (oracleConn.State == ConnectionState.Open)
                            oracleConn.Close();
                        throw new Exception("SQL=" + sql + " Exception=" + ex.Message);
                    }
                case Sys.DBSrcType.MySQL:
                    MySqlConnection mysqlConn = new MySqlConnection(ConnString);
                    MySqlDataAdapter mysqlAda = null;

                    try
                    {
                        mysqlConn.Open();
                        mysqlAda = new MySqlDataAdapter(sql, mysqlConn);
                        mysqlAda.SelectCommand.CommandType = CommandType.Text;
                        DataTable mysqlTb = new DataTable("otb");
                        mysqlAda.Fill(startRecord, recordCount, mysqlTb);
                        mysqlAda.Dispose();
                        mysqlConn.Close();
                        return mysqlTb;
                    }
                    catch (Exception ex)
                    {
                        if (mysqlAda != null)
                            mysqlAda.Dispose();
                        if (mysqlConn.State == ConnectionState.Open)
                            mysqlConn.Close();
                        throw new Exception("SQL=" + sql + " Exception=" + ex.Message);
                    }
                case Sys.DBSrcType.Informix:
                    IfxConnection ifxConn = new IfxConnection(ConnString);
                    IfxDataAdapter ifxAda = null;

                    try
                    {
                        ifxConn.Open();
                        ifxAda = new IfxDataAdapter(sql, ifxConn);
                        ifxAda.SelectCommand.CommandType = CommandType.Text;
                        DataTable ifxTb = new DataTable("otb");
                        ifxAda.Fill(startRecord, recordCount, ifxTb);
                        ifxAda.Dispose();
                        ifxConn.Close();
                        return ifxTb;
                    }
                    catch (Exception ex)
                    {
                        if (ifxAda != null)
                            ifxAda.Dispose();
                        if (ifxConn.State == ConnectionState.Open)
                            ifxConn.Close();
                        throw new Exception("SQL=" + sql + " Exception=" + ex.Message);
                    }
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// 获取SQLServer链接服务器的表/视图名，根据链接服务器的命名规则组合
        /// </summary>
        /// <param name="objName">表/视图名称</param>
        /// <returns></returns>
        public string GetLinkedServerObjName(string objName)
        {
            //目前还只是考虑到SqlServer数据库中建立链接服务器的功能，其他数据库还没有考虑
            //Oracle中有DBLink功能，但具体还没有研究；MySQL中的Federated引擎功能还不完善，貌似只能增加mysql的外链数据库，且效率可能不大好也
            switch (this.DBSrcType)
            {
                case Sys.DBSrcType.Localhost:
                    if (DBAccess.AppCenterDBType != DBType.MSSQL)
                        throw new Exception("目前只支持CCFlow主数据库为SqlServer的模式，其他数据库类型暂不支持建立数据源。");

                    return objName;
                case Sys.DBSrcType.SQLServer:
                    return string.Format("{0}.{1}.dbo.{2}", this.No, this.DBName, objName);
                case Sys.DBSrcType.Oracle:
                    return string.Format("{0}..{1}.{2}", this.No, this.UserID.ToUpper(), objName.ToUpper());
                case Sys.DBSrcType.MySQL:
                    return string.Format("OPENQUERY({0},'SELECT * FROM {1}')", this.No, objName);
                case Sys.DBSrcType.Informix:
                    return string.Format("OPENQUERY({0},'SELECT * FROM {1}')", this.No, objName);
                default:
                    throw new Exception("@未涉及的数据库类型。");
            }
        }

        /// <summary>
        /// 判断数据源所在库中是否已经存在指定名称的对象【表/视图】
        /// </summary>
        /// <param name="objName">表/视图 名称</param>
        /// <returns>如果不存在，返回null，否则返回对象的类型：TABLE(表)、VIEW(视图)、PROCEDURE(存储过程，判断不完善)、OTHER(其他类型)</returns>
        public string IsExistsObj(string objName)
        {
            string sql = string.Empty;
            DataTable dt = null;

            switch (this.DBSrcType)
            {
                case Sys.DBSrcType.Localhost:
                    sql = GetIsExitsSQL(DBAccess.AppCenterDBType, objName, DBAccess.GetAppCenterDBConn.Database);
                    dt = DBAccess.RunSQLReturnTable(sql);
                    break;
                case Sys.DBSrcType.SQLServer:
                    sql = GetIsExitsSQL(DBType.MSSQL, objName, this.DBName);
                    dt = RunSQLReturnTable(sql);
                    break;
                case Sys.DBSrcType.Oracle:
                    sql = GetIsExitsSQL(DBType.Oracle, objName, this.DBName);
                    dt = RunSQLReturnTable(sql);
                    break;
                case Sys.DBSrcType.MySQL:
                    sql = GetIsExitsSQL(DBType.MySQL, objName, this.DBName);
                    dt = RunSQLReturnTable(sql);
                    break;
                case Sys.DBSrcType.Informix:
                    sql = GetIsExitsSQL(DBType.Informix, objName, this.DBName);
                    dt = RunSQLReturnTable(sql);
                    break;
                default:
                    throw new Exception("@未涉及的数据库类型。");
            }

            return dt.Rows.Count == 0 ? null : dt.Rows[0][0].ToString();
        }

        /// <summary>
        /// 获取判断数据库中是否存在指定名称的表/视图SQL语句
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="objName">表/视图名称</param>
        /// <param name="dbName">数据库名称</param>
        /// <returns></returns>
        public string GetIsExitsSQL(DBType dbType, string objName, string dbName)
        {
            switch (dbType)
            {
                case DBType.MSSQL:
                case DBType.PostgreSQL:
                    return string.Format("SELECT (CASE s.xtype WHEN 'U' THEN 'TABLE' WHEN 'V' THEN 'VIEW' WHEN 'P' THEN 'PROCEDURE' ELSE 'OTHER' END) OTYPE FROM sysobjects s WHERE s.name = '{0}'", objName);
                case DBType.Oracle:
                    return string.Format("SELECT uo.OBJECT_TYPE OTYPE FROM user_objects uo WHERE uo.OBJECT_NAME = '{0}'", objName.ToUpper());
                case DBType.MySQL:
                    return string.Format("SELECT (CASE t.TABLE_TYPE WHEN 'BASE TABLE' THEN 'TABLE' ELSE 'VIEW' END) OTYPE FROM information_schema.tables t WHERE t.TABLE_SCHEMA = '{1}' AND t.TABLE_NAME = '{0}'", objName, dbName);
                case DBType.Informix:
                    return string.Format("SELECT (CASE s.tabtype WHEN 'T' THEN 'TABLE' WHEN 'V' THEN 'VIEW' ELSE 'OTHER' END) OTYPE FROM systables s WHERE s.tabname = '{0}'", objName);
                case DBType.DB2:
                    return string.Format("");
                case DBType.Access:
                    return string.Format("");
                default:
                    throw new Exception("@没有涉及的数据库类型。");
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 数据源
        /// </summary>
        public SFDBSrc()
        {
        }
        public SFDBSrc(string mypk)
        {
            this.No = mypk;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_SFDBSrc", "数据源");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddTBStringPK(SFDBSrcAttr.No, null, "数据源编号(必须是英文)", true, false, 1, 20, 20);
                map.AddTBString(SFDBSrcAttr.Name, null, "数据源名称", true, false, 0, 30, 20);

                map.AddDDLSysEnum(SFDBSrcAttr.DBSrcType, 0, "数据源类型", true, true,
                  SFDBSrcAttr.DBSrcType, "@0=应用系统主数据库(默认)@1=SQLServer数据库@2=Oracle数据库@3=MySQL数据库@4=Informix数据库@50=Dubbo服务@100=WebService数据源");

                map.AddTBString(SFDBSrcAttr.UserID, null, "数据库登录用户ID", true, false, 0, 30, 20);
                map.AddTBString(SFDBSrcAttr.Password, null, "数据库登录用户密码", true, false, 0, 30, 20);
                map.AddTBString(SFDBSrcAttr.IP, null, "IP地址/数据库实例名", true, false, 0, 500, 20);
                map.AddTBString(SFDBSrcAttr.DBName, null, "数据库名称/Oracle保持为空", true, false, 0, 30, 20);

                //map.AddDDLSysEnum(SFDBSrcAttr.DBSrcType, 0, "数据源类型", true, true,
                //    SFDBSrcAttr.DBSrcType, "@0=应用系统主数据库@1=SQLServer@2=Oracle@3=MySQL@4=Infomix");

                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "测试连接";
                rm.ClassMethodName = this.ToString() + ".DoConn";
                rm.RefMethodType = RefMethodType.Func; // 仅仅是一个功能.
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 方法.
        /// <summary>
        /// 是表还是视图？
        /// </summary>
        /// <param name="tabelOrViewName"></param>
        /// <returns></returns>
        public bool IsView(string tabelOrViewName)
        {
            string sql = "";
            switch (this.HisDBType)
            {
                case DBType.Oracle:
                    sql = "SELECT TABTYPE  FROM TAB WHERE UPPER(TNAME)=:v";
                    DataTable oradt = DBAccess.RunSQLReturnTable(sql, "v", tabelOrViewName.ToUpper());
                    if (oradt.Rows.Count == 0)
                        throw new Exception("@表不存在[" + tabelOrViewName + "]");
                    if (oradt.Rows[0][0].ToString().ToUpper().Trim() == "V".ToString())
                        return true;
                    else
                        return false;
                    break;
                case DBType.Access:
                    sql = "select   Type   from   msysobjects   WHERE   UCASE(name)='" + tabelOrViewName.ToUpper() + "'";
                    DataTable dtw = DBAccess.RunSQLReturnTable(sql);
                    if (dtw.Rows.Count == 0)
                        throw new Exception("@表不存在[" + tabelOrViewName + "]");
                    if (dtw.Rows[0][0].ToString().Trim() == "5")
                        return true;
                    else
                        return false;
                case DBType.MSSQL:
                    sql = "select xtype from sysobjects WHERE name =" + SystemConfig.AppCenterDBVarStr + "v";
                    DataTable dt1 = DBAccess.RunSQLReturnTable(sql, "v", tabelOrViewName);
                    if (dt1.Rows.Count == 0)
                        throw new Exception("@表不存在[" + tabelOrViewName + "]");

                    if (dt1.Rows[0][0].ToString().ToUpper().Trim() == "V".ToString())
                        return true;
                    else
                        return false;
                case DBType.Informix:
                    sql = "select tabtype from systables where tabname = '" + tabelOrViewName.ToLower() + "'";
                    DataTable dtaa = DBAccess.RunSQLReturnTable(sql);
                    if (dtaa.Rows.Count == 0)
                        throw new Exception("@表不存在[" + tabelOrViewName + "]");

                    if (dtaa.Rows[0][0].ToString().ToUpper().Trim() == "V")
                        return true;
                    else
                        return false;
                case DBType.MySQL:
                    sql = "SELECT Table_Type FROM information_schema.TABLES WHERE table_name='" + tabelOrViewName + "' and table_schema='" + SystemConfig.AppCenterDBDatabase + "'";
                    DataTable dt2 = DBAccess.RunSQLReturnTable(sql);
                    if (dt2.Rows.Count == 0)
                        throw new Exception("@表不存在[" + tabelOrViewName + "]");

                    if (dt2.Rows[0][0].ToString().ToUpper().Trim() == "VIEW")
                        return true;
                    else
                        return false;
                default:
                    throw new Exception("@没有做的判断。");
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql, "v", tabelOrViewName.ToUpper());
            if (dt.Rows.Count == 0)
                throw new Exception("@表不存在[" + tabelOrViewName + "]");

            if (dt.Rows[0][0].ToString() == "VIEW")
                return true;
            else
                return false;
            return true;
        }

        /// <summary>
        /// 连接字符串.
        /// </summary>
        public string ConnString
        {
            get
            {
                switch (this.DBSrcType)
                {
                    case Sys.DBSrcType.Localhost:
                        return BP.Sys.SystemConfig.AppCenterDSN;
                    case Sys.DBSrcType.SQLServer:
                        return "password=" + this.Password + ";persist security info=true;user id=" + this.UserID + ";initial catalog=" + this.DBName + ";data source=" + this.IP + ";timeout=999;multipleactiveresultsets=true";
                    case Sys.DBSrcType.Oracle:
                        return "user id=" + this.UserID + ";data source=" + this.IP + ";password=" + this.Password + ";Max Pool Size=200";
                    case Sys.DBSrcType.MySQL:
                        return "Data Source=" + this.IP + ";Persist Security info=True;Initial Catalog=" + this.DBName + ";User ID=" + this.UserID + ";Password=" + this.Password + ";";
                    case Sys.DBSrcType.Informix:
                        return "Host=" + this.IP + "; Service=; Server=; Database=" + this.DBName + "; User id=" + this.UserID + "; Password=" + this.Password + "; ";  //Service为监听客户端连接的服务名，Server为数据库实例名，这两项没提供
                    default:
                        throw new Exception("@没有判断的类型.");
                }
            }
        }
        /// <summary>
        /// 执行连接
        /// </summary>
        /// <returns></returns>
        public string DoConn()
        {
            if (this.No == "local")
                return "本地连接不需要测试是否连接成功.";

            if (this.DBSrcType == Sys.DBSrcType.Localhost)
                //throw new Exception("@在该系统中只能有一个本地连接.");
                return "@在该系统中只能有一个本地连接.";

            string dsn = "";
            if (this.DBSrcType == Sys.DBSrcType.SQLServer)
            {
                try
                {
                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
                    conn.ConnectionString = this.ConnString;
                    conn.Open();
                    conn.Close();

                    //删除应用.
                    try
                    {
                        BP.DA.DBAccess.RunSQL("Exec sp_droplinkedsrvlogin " + this.No + ",Null ");
                        BP.DA.DBAccess.RunSQL("Exec sp_dropserver " + this.No);
                    }
                    catch
                    {
                    }

                    //创建应用.
                    string sql = "";
                    sql += "sp_addlinkedserver @server='" + this.No + "', @srvproduct='', @provider='SQLOLEDB', @datasrc='" + this.IP + "'";
                    BP.DA.DBAccess.RunSQL(sql);

                    //执行登录.
                    sql = "";
                    sql += " EXEC sp_addlinkedsrvlogin '" + this.No + "','false', NULL, '" + this.UserID + "', '" + this.Password + "'";
                    BP.DA.DBAccess.RunSQL(sql);

                    return "恭喜您，该(" + this.Name + ")连接配置成功。";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            if (this.DBSrcType == Sys.DBSrcType.Oracle)
            {
                try
                {
                    //  dsn = "user id=" + this.UserID + ";data source=" + this.DBName + ";password=" + this.Password + ";Max Pool Size=200";
                    System.Data.OracleClient.OracleConnection conn = new System.Data.OracleClient.OracleConnection();
                    conn.ConnectionString = this.ConnString;
                    conn.Open();
                    conn.Close();
                    return "恭喜您，该(" + this.Name + ")连接配置成功。";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            if (this.DBSrcType == Sys.DBSrcType.MySQL)
            {
                try
                {
                    //   dsn = "Data Source=" + this.IP + ";Persist Security info=True;Initial Catalog=" + this.DBName + ";User ID=" + this.UserID + ";Password=" + this.Password + ";";
                    MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
                    conn.ConnectionString = this.ConnString;
                    conn.Open();
                    conn.Close();
                    return "恭喜您，该(" + this.Name + ")连接配置成功。";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            if (this.DBSrcType == Sys.DBSrcType.Informix)
            {
                try
                {
                    IfxConnection conn = new IfxConnection();
                    conn.ConnectionString = this.ConnString;
                    conn.Open();
                    conn.Close();
                    return "恭喜您，该(" + this.Name + ")连接配置成功。";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            if (this.DBSrcType == Sys.DBSrcType.WebServices)
            {
                string url = this.IP +
                             (this.IP.EndsWith(".asmx") ? "?wsdl" : this.IP.EndsWith(".svc") ? "?singleWsdl" : "");

                try
                {
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                    myRequest.Method = "GET";　              //设置提交方式可以为＂ｇｅｔ＂，＂ｈｅａｄ＂等
                    myRequest.Timeout = 30000;　             //设置网页响应时间长度
                    myRequest.AllowAutoRedirect = false;//是否允许自动重定向
                    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                    return myResponse.StatusCode == HttpStatusCode.OK ? "连接配置成功。" : "连接配置失败。";//返回响应的状态
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            return "没有涉及到的连接测试类型...";
        }
        /// <summary>
        /// 获取所有数据表，不包括视图
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllTablesWithoutViews()
        {
            var sql = new StringBuilder();
            var dbType = this.DBSrcType;
            if (dbType == Sys.DBSrcType.Localhost)
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        dbType = Sys.DBSrcType.SQLServer;
                        break;
                    case DBType.Oracle:
                        dbType = Sys.DBSrcType.Oracle;
                        break;
                    case DBType.MySQL:
                        dbType = Sys.DBSrcType.MySQL;
                        break;
                    case DBType.Informix:
                        dbType = Sys.DBSrcType.Informix;
                        break;
                    default:
                        throw new Exception("没有涉及到的连接测试类型...");
                }
            }

            switch (dbType)
            {
                case Sys.DBSrcType.SQLServer:
                    sql.AppendLine("SELECT NAME AS No,");
                    sql.AppendLine("       NAME");
                    sql.AppendLine("FROM   sysobjects");
                    sql.AppendLine("WHERE  xtype = 'U'");
                    sql.AppendLine("ORDER BY");
                    sql.AppendLine("       Name");
                    break;
                case Sys.DBSrcType.Oracle:
                    sql.AppendLine("SELECT uo.OBJECT_NAME No,");
                    sql.AppendLine("       uo.OBJECT_NAME Name");
                    sql.AppendLine("  FROM user_objects uo");
                    sql.AppendLine(" WHERE uo.OBJECT_TYPE = 'TABLE'");
                    sql.AppendLine(" ORDER BY uo.OBJECT_NAME");
                    break;
                case Sys.DBSrcType.MySQL:
                    sql.AppendLine("SELECT ");
                    sql.AppendLine("    table_name No,");
                    sql.AppendLine("    table_name Name");
                    sql.AppendLine("FROM");
                    sql.AppendLine("    information_schema.tables");
                    sql.AppendLine("WHERE");
                    sql.AppendLine(string.Format("    table_schema = '{0}'", this.DBSrcType == Sys.DBSrcType.Localhost ? DBAccess.GetAppCenterDBConn.Database : this.DBName));
                    sql.AppendLine("        AND table_type = 'BASE TABLE'");
                    sql.AppendLine("ORDER BY table_name;");
                    break;
                case Sys.DBSrcType.Informix:
                    sql.AppendLine("");
                    break;
                default:
                    break;
            }

            DataTable allTables = null;
            if (this.No == "local")
            {
                allTables = DBAccess.RunSQLReturnTable(sql.ToString());
            }
            else
            {
                var dsn = GetDSN();
                var conn = GetConnection(dsn);
                try
                {
                    conn.Open();
                    allTables = RunSQLReturnTable(sql.ToString(), conn, dsn, CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw new Exception("@失败:" + ex.Message + " dns:" + dsn);
                }
            }

            return allTables;
        }
        /// <summary>
        /// 获得数据列表.
        /// </summary>
        /// <returns></returns>
        public DataTable GetTables(bool isCutFlowTables=false)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("SELECT ss.SrcTable FROM Sys_SFTable ss WHERE ss.FK_SFDBSrc = '{0}'", this.No);

            var allTablesExist = DBAccess.RunSQLReturnTable(sql.ToString());

            sql.Clear();

            var dbType = this.DBSrcType;
            if (dbType == Sys.DBSrcType.Localhost)
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        dbType = Sys.DBSrcType.SQLServer;
                        break;
                    case DBType.Oracle:
                        dbType = Sys.DBSrcType.Oracle;
                        break;
                    case DBType.MySQL:
                        dbType = Sys.DBSrcType.MySQL;
                        break;
                    case DBType.Informix:
                        dbType = Sys.DBSrcType.Informix;
                        break;
                    default:
                        throw new Exception("没有涉及到的连接测试类型...");
                }
            }

            switch (dbType)
            {
                case Sys.DBSrcType.SQLServer:
                    sql.AppendLine("SELECT NAME AS No,");
                    sql.AppendLine("       [Name] = '[' + (CASE xtype WHEN 'U' THEN '表' ELSE '视图' END) + '] ' + ");
                    sql.AppendLine("       NAME,");
                    sql.AppendLine("       xtype");
                    sql.AppendLine("FROM   sysobjects");
                    sql.AppendLine("WHERE  (xtype = 'U' OR xtype = 'V')");
                    //   sql.AppendLine("       AND (NAME NOT LIKE 'ND%')");
                    sql.AppendLine("       AND (NAME NOT LIKE 'Demo_%')");
                    sql.AppendLine("       AND (NAME NOT LIKE 'Sys_%')");
                    sql.AppendLine("       AND (NAME NOT LIKE 'WF_%')");
                    sql.AppendLine("       AND (NAME NOT LIKE 'GPM_%')");
                    sql.AppendLine("ORDER BY");
                    sql.AppendLine("       xtype,");
                    sql.AppendLine("       NAME");
                    break;
                case Sys.DBSrcType.Oracle:
                    sql.AppendLine("SELECT uo.OBJECT_NAME AS No,");
                    sql.AppendLine("       '[' || (CASE uo.OBJECT_TYPE");
                    sql.AppendLine("         WHEN 'TABLE' THEN");
                    sql.AppendLine("          '表'");
                    sql.AppendLine("         ELSE");
                    sql.AppendLine("          '视图'");
                    sql.AppendLine("       END) || '] ' || uo.OBJECT_NAME AS Name,");
                    sql.AppendLine("       CASE uo.OBJECT_TYPE");
                    sql.AppendLine("         WHEN 'TABLE' THEN");
                    sql.AppendLine("          'U'");
                    sql.AppendLine("         ELSE");
                    sql.AppendLine("          'V'");
                    sql.AppendLine("       END AS xtype");
                    sql.AppendLine("  FROM user_objects uo");
                    sql.AppendLine(" WHERE (uo.OBJECT_TYPE = 'TABLE' OR uo.OBJECT_TYPE = 'VIEW')");
                    //sql.AppendLine("   AND uo.OBJECT_NAME NOT LIKE 'ND%'");
                    sql.AppendLine("   AND uo.OBJECT_NAME NOT LIKE 'Demo_%'");
                    sql.AppendLine("   AND uo.OBJECT_NAME NOT LIKE 'Sys_%'");
                    sql.AppendLine("   AND uo.OBJECT_NAME NOT LIKE 'WF_%'");
                    sql.AppendLine("   AND uo.OBJECT_NAME NOT LIKE 'GPM_%'");
                    sql.AppendLine(" ORDER BY uo.OBJECT_TYPE, uo.OBJECT_NAME");
                    break;
                case Sys.DBSrcType.MySQL:
                    sql.AppendLine("SELECT ");
                    sql.AppendLine("    table_name AS No,");
                    sql.AppendLine("    CONCAT('[',");
                    sql.AppendLine("            CASE table_type");
                    sql.AppendLine("                WHEN 'BASE TABLE' THEN '表'");
                    sql.AppendLine("                ELSE '视图'");
                    sql.AppendLine("            END,");
                    sql.AppendLine("            '] ',");
                    sql.AppendLine("            table_name) AS Name,");
                    sql.AppendLine("    CASE table_type");
                    sql.AppendLine("        WHEN 'BASE TABLE' THEN 'U'");
                    sql.AppendLine("        ELSE 'V'");
                    sql.AppendLine("    END AS xtype");
                    sql.AppendLine("FROM");
                    sql.AppendLine("    information_schema.tables");
                    sql.AppendLine("WHERE");
                    sql.AppendLine(string.Format("    table_schema = '{0}'", this.DBSrcType == Sys.DBSrcType.Localhost ? DBAccess.GetAppCenterDBConn.Database : this.DBName));
                    sql.AppendLine("        AND (table_type = 'BASE TABLE'");
                    sql.AppendLine("        OR table_type = 'VIEW')");
                    //   sql.AppendLine("       AND (table_name NOT LIKE 'ND%'");
                    sql.AppendLine("        AND table_name NOT LIKE 'Demo_%'");
                    sql.AppendLine("        AND table_name NOT LIKE 'Sys_%'");
                    sql.AppendLine("        AND table_name NOT LIKE 'WF_%'");
                    sql.AppendLine("        AND table_name NOT LIKE 'GPM_%'");
                    sql.AppendLine("ORDER BY table_type , table_name;");
                    break;
                case Sys.DBSrcType.Informix:
                    sql.AppendLine("");
                    break;
                default:
                    break;
            }

            DataTable allTables = null;
            if (this.No == "local")
            {
                allTables = DBAccess.RunSQLReturnTable(sql.ToString());
            }
            else
            {
                var dsn = GetDSN();
                var conn = GetConnection(dsn);
                try
                {
                    conn.Open();
                    allTables = RunSQLReturnTable(sql.ToString(), conn, dsn, CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw new Exception("@失败:" + ex.Message + " dns:" + dsn);
                }
            }

            //去除已经使用的表
            var filter = string.Empty;
            foreach (DataRow dr in allTablesExist.Rows)
                filter += string.Format("No='{0}' OR ", dr[0]);

            if (filter != "")
            {
                var deletedRows = allTables.Select(filter.TrimEnd(" OR ".ToCharArray()));
                foreach (DataRow dr in deletedRows)
                {
                    allTables.Rows.Remove(dr);
                }
            }

            //去掉系统表.
            if (isCutFlowTables == true)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("No", typeof(string));
                dt.Columns.Add("Name", typeof(string));

                foreach (DataRow dr in allTables.Rows)
                {
                    string no = dr["No"].ToString();

                    if (no.Contains("WF_") 
                        || no.Contains("Track")
                        || no.Contains("Sys_") 
                        || no.Contains("Demo_"))
                        continue;

                    DataRow mydr = dt.NewRow();
                    mydr["No"] = dr["No"];
                    mydr["Name"] = dr["Name"];
                    dt.Rows.Add(mydr);
                }

                return dt;
            }

            return allTables;
        }
        /// <summary>
        /// 获取连接字符串
        /// <para></para>
        /// <para>added by liuxc,2015-6-9</para>
        /// </summary>
        /// <returns></returns>
        private string GetDSN()
        {
            string dsn = "";

            var dbType = this.DBSrcType;
            if (dbType == Sys.DBSrcType.Localhost)
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        dbType = Sys.DBSrcType.SQLServer;
                        break;
                    case DBType.Oracle:
                        dbType = Sys.DBSrcType.Oracle;
                        break;
                    case DBType.MySQL:
                        dbType = Sys.DBSrcType.MySQL;
                        break;
                    case DBType.Informix:
                        dbType = Sys.DBSrcType.Informix;
                        break;
                    default:
                        throw new Exception("没有涉及到的连接测试类型...");
                }
            }

            switch (dbType)
            {
                case Sys.DBSrcType.SQLServer:
                    dsn = "Password=" + this.Password + ";Persist Security Info=True;User ID=" + this.UserID +
                      ";Initial Catalog=" + this.DBName + ";Data Source=" + this.IP +
                      ";Timeout=999;MultipleActiveResultSets=true";
                    break;
                case Sys.DBSrcType.Oracle:
                    dsn = "user id=" + this.UserID + ";data source=" + this.DBName + ";password=" + this.Password + ";Max Pool Size=200";
                    break;
                case Sys.DBSrcType.MySQL:
                    dsn = "Data Source=" + this.IP + ";Persist Security info=True;Initial Catalog=" + this.DBName + ";User ID=" + this.UserID + ";Password=" + this.Password + ";";
                    break;
                case Sys.DBSrcType.Informix:
                    dsn = "Provider=Ifxoledbc;Data Source=" + this.DBName + "@" + this.IP + ";User ID=" + this.UserID + ";Password=" + this.Password + ";";
                    break;
                default:
                    throw new Exception("没有涉及到的连接测试类型...");
            }
            return dsn;
        }
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="dsn">连接字符串</param>
        /// <returns></returns>
        private System.Data.Common.DbConnection GetConnection(string dsn)
        {
            System.Data.Common.DbConnection conn = null;

            var dbType = this.DBSrcType;
            if (dbType == Sys.DBSrcType.Localhost)
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        dbType = Sys.DBSrcType.SQLServer;
                        break;
                    case DBType.Oracle:
                        dbType = Sys.DBSrcType.Oracle;
                        break;
                    case DBType.MySQL:
                        dbType = Sys.DBSrcType.MySQL;
                        break;
                    case DBType.Informix:
                        dbType = Sys.DBSrcType.Informix;
                        break;
                    default:
                        throw new Exception("没有涉及到的连接测试类型...");
                }
            }

            switch (dbType)
            {
                case Sys.DBSrcType.SQLServer:
                    conn = new System.Data.SqlClient.SqlConnection(dsn);
                    break;
                case Sys.DBSrcType.Oracle:
                    conn = new System.Data.OracleClient.OracleConnection(dsn);
                    break;
                case Sys.DBSrcType.MySQL:
                    conn = new MySql.Data.MySqlClient.MySqlConnection(dsn);
                    break;
                case Sys.DBSrcType.Informix:
                    conn = new System.Data.OleDb.OleDbConnection(dsn);
                    break;
            }

            return conn;
        }

        private DataTable RunSQLReturnTable(string sql, System.Data.Common.DbConnection conn, string dsn, CommandType cmdType)
        {
            if (conn is System.Data.SqlClient.SqlConnection)
                return BP.DA.DBAccess.RunSQLReturnTable(sql, (System.Data.SqlClient.SqlConnection)conn, dsn, cmdType);
            if (conn is System.Data.OleDb.OleDbConnection)
                return BP.DA.DBAccess.RunSQLReturnTable(sql, (System.Data.OleDb.OleDbConnection)conn, cmdType);
            if (conn is System.Data.OracleClient.OracleConnection)
                return BP.DA.DBAccess.RunSQLReturnTable(sql, (System.Data.OracleClient.OracleConnection)conn, cmdType, dsn);
            if (conn is MySqlConnection)
            {
                var mySqlConn = (MySqlConnection)conn;
                if (mySqlConn.State != ConnectionState.Open)
                    mySqlConn.Open();

                var ada = new MySqlDataAdapter(sql, mySqlConn);
                ada.SelectCommand.CommandType = CommandType.Text;


                try
                {
                    DataTable oratb = new DataTable("otb");
                    ada.Fill(oratb);
                    ada.Dispose();

                    conn.Close();
                    conn.Dispose();
                    return oratb;
                }
                catch (Exception ex)
                {
                    ada.Dispose();
                    conn.Close();
                    throw new Exception("SQL=" + sql + " Exception=" + ex.Message);
                }
            }

            throw new Exception("没有涉及到的连接测试类型...");
            return null;
        }

        /// <summary>
        /// 修改表/视图/列名称（不完善）
        /// </summary>
        /// <param name="objType">修改对象的类型，TABLE(表)、VIEW(视图)、COLUMN(列)</param>
        /// <param name="oldName">旧名称</param>
        /// <param name="newName">新名称</param>
        /// <param name="tableName">修改列名称时，列所属的表名称</param>
        public void Rename(string objType, string oldName, string newName, string tableName = null)
        {
            var dbType = this.DBSrcType;
            if (dbType == Sys.DBSrcType.Localhost)
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        dbType = Sys.DBSrcType.SQLServer;
                        break;
                    case DBType.Oracle:
                        dbType = Sys.DBSrcType.Oracle;
                        break;
                    case DBType.MySQL:
                        dbType = Sys.DBSrcType.MySQL;
                        break;
                    case DBType.Informix:
                        dbType = Sys.DBSrcType.Informix;
                        break;
                    default:
                        throw new Exception("@没有涉及到的连接测试类型。");
                }
            }

            switch (dbType)
            {
                case Sys.DBSrcType.SQLServer:
                    if (objType.ToLower() == "column")
                        RunSQL(string.Format("EXEC SP_RENAME '{0}', '{1}', 'COLUMN'", oldName, newName));
                    else
                        RunSQL(string.Format("EXEC SP_RENAME '{0}', '{1}'", oldName, newName));
                    break;
                case Sys.DBSrcType.Oracle:
                    if (objType.ToLower() == "column")
                        RunSQL(string.Format("ALTER TABLE {0} RENAME COLUMN {1} TO {2}", tableName, oldName, newName));
                    else if (objType.ToLower() == "table")
                        RunSQL(string.Format("ALTER TABLE {0} RENAME TO {1}", oldName, newName));
                    else if (objType.ToLower() == "view")
                        RunSQL(string.Format("RENAME {0} TO {1}", oldName, newName));
                    else
                        throw new Exception("@未涉及到的Oracle数据库改名逻辑。");
                    break;
                case Sys.DBSrcType.MySQL:
                    if (objType.ToLower() == "column")
                    {
                        string sql = string.Format("SELECT c.COLUMN_TYPE FROM information_schema.columns c WHERE c.TABLE_SCHEMA = '{0}' AND c.TABLE_NAME = '{1}' AND c.COLUMN_NAME = '{2}'", this.DBName, tableName, oldName);

                        DataTable dt = RunSQLReturnTable(sql);
                        if (dt.Rows.Count > 0)
                        {
                            RunSQL(string.Format("ALTER TABLE {0} CHANGE COLUMN {1} {2} {3}", tableName, oldName,
                                                 newName, dt.Rows[0][0]));
                        }
                    }
                    else if (objType.ToLower() == "table")
                    {
                        RunSQL(string.Format("ALTER TABLE `{0}`.`{1}` RENAME `{0}`.`{2}`", this.DBName, oldName, newName));
                    }
                    else if (objType.ToLower() == "view")
                    {
                        string sql = string.Format(
                            "SELECT t.VIEW_DEFINITION FROM information_schema.views t WHERE t.TABLE_SCHEMA = '{0}' AND t.TABLE_NAME = '{1}'",
                            this.DBName, oldName);

                        DataTable dt = RunSQLReturnTable(sql);
                        if (dt.Rows.Count == 0)
                        {
                            RunSQL("DROP VIEW " + oldName);
                        }
                        else
                        {
                            RunSQL(string.Format("CREATE VIEW {0} AS {1}", newName, dt.Rows[0][0]));
                            RunSQL("DROP VIEW " + oldName);
                        }
                    }
                    else
                        throw new Exception("@未涉及到的Oracle数据库改名逻辑。");
                    break;
                case Sys.DBSrcType.Informix:

                    break;
                default:
                    throw new Exception("@没有涉及到的数据库类型。");
            }
        }


        /// <summary>
        /// 获取表的字段信息
        /// </summary>
        /// <param name="tableName">表/视图名称</param>
        /// <returns>有四个列 No,Name,DBType,DBLength 分别标识  列的字段名，列描述，类型，长度。</returns>
        public DataTable GetColumns(string tableName)
        {
            //SqlServer数据库
            var sql = new StringBuilder();

            var dbType = this.DBSrcType;
            if (dbType == Sys.DBSrcType.Localhost)
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        dbType = Sys.DBSrcType.SQLServer;
                        break;
                    case DBType.Oracle:
                        dbType = Sys.DBSrcType.Oracle;
                        break;
                    case DBType.MySQL:
                        dbType = Sys.DBSrcType.MySQL;
                        break;
                    case DBType.Informix:
                        dbType = Sys.DBSrcType.Informix;
                        break;
                    default:
                        throw new Exception("没有涉及到的连接测试类型...");
                }
            }

            switch (dbType)
            {
                case Sys.DBSrcType.SQLServer:
                    sql.AppendLine("SELECT sc.name as No,");
                    sql.AppendLine("       st.name AS [DBType],");
                    sql.AppendLine("       (");
                    sql.AppendLine("           CASE ");
                    sql.AppendLine("                WHEN st.name = 'nchar' OR st.name = 'nvarchar' THEN sc.length / 2");
                    sql.AppendLine("                ELSE sc.length");
                    sql.AppendLine("           END");
                    sql.AppendLine("       ) AS DBLength,");
                    sql.AppendLine("       sc.colid,");
                    sql.AppendLine(string.Format("       {0} AS [Name]", SqlBuilder.GetIsNullInSQL("ep.[value]", "''")));
                    sql.AppendLine("FROM   dbo.syscolumns sc");
                    sql.AppendLine("       INNER JOIN dbo.systypes st");
                    sql.AppendLine("            ON  sc.xtype = st.xusertype");
                    sql.AppendLine("       LEFT OUTER JOIN sys.extended_properties ep");
                    sql.AppendLine("            ON  sc.id = ep.major_id");
                    sql.AppendLine("            AND sc.colid = ep.minor_id");
                    sql.AppendLine("            AND ep.name = 'MS_Description'");
                    sql.AppendLine(string.Format("WHERE  sc.id = OBJECT_ID('dbo.{0}')", tableName));
                    break;
                case Sys.DBSrcType.Oracle:
                    sql.AppendLine("SELECT utc.COLUMN_NAME AS No,");
                    sql.AppendLine("       utc.DATA_TYPE   AS DBType,");
                    sql.AppendLine("       utc.CHAR_LENGTH AS DBLength,");
                    sql.AppendLine("       utc.COLUMN_ID   AS colid,");
                    sql.AppendLine(string.Format("       {0}    AS Name", SqlBuilder.GetIsNullInSQL("ucc.comments", "''")));
                    sql.AppendLine("  FROM user_tab_cols utc");
                    sql.AppendLine("  LEFT JOIN user_col_comments ucc");
                    sql.AppendLine("    ON ucc.table_name = utc.TABLE_NAME");
                    sql.AppendLine("   AND ucc.column_name = utc.COLUMN_NAME");
                    sql.AppendLine(string.Format(" WHERE utc.TABLE_NAME = '{0}'", tableName.ToUpper()));
                    sql.AppendLine(" ORDER BY colid ASC");

                    break;
                case Sys.DBSrcType.MySQL:
                    sql.AppendLine("SELECT ");
                    sql.AppendLine("    column_name AS 'No',");
                    sql.AppendLine("    data_type AS 'DBType',");
                    sql.AppendLine(string.Format("    {0} AS DBLength,", SqlBuilder.GetIsNullInSQL("character_maximum_length", "numeric_precision")));
                    sql.AppendLine("    ordinal_position AS colid,");
                    sql.AppendLine("    column_comment AS 'Name'");
                    sql.AppendLine("FROM");
                    sql.AppendLine("    information_schema.columns");
                    sql.AppendLine("WHERE");
                    sql.AppendLine(string.Format("    table_schema = '{0}'", this.DBSrcType == Sys.DBSrcType.Localhost ? DBAccess.GetAppCenterDBConn.Database : this.DBName));
                    sql.AppendLine(string.Format("        AND table_name = '{0}';", tableName));
                    break;
                case Sys.DBSrcType.Informix:
                    break;
                default:
                    throw new Exception("没有涉及到的连接测试类型...");
            }


            if (this.No == "local")
                return DBAccess.RunSQLReturnTable(sql.ToString());

            var dsn = GetDSN();
            var conn = GetConnection(dsn);

            try
            {
                //System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
                //conn.ConnectionString = dsn;
                conn.Open();

                return RunSQLReturnTable(sql.ToString(), conn, dsn, CommandType.Text);
            }
            catch (Exception ex)
            {
                throw new Exception("@失败:" + ex.Message + " dns:" + dsn);
            }

            return null;
        }

        protected override bool beforeDelete()
        {
            if (this.No == "local")
                throw new Exception("@默认连接(local)不允许删除、更新.");

            string str = "";
            MapDatas mds = new MapDatas();
            mds.Retrieve(MapDataAttr.DBSrc, this.No);
            if (mds.Count != 0)
            {
                str += "如下表单使用了该数据源，您不能删除它。";
                foreach (MapData md in mds)
                {
                    str += "@\t\n" + md.No + " - " + md.Name;
                }
            }

            SFTables tabs = new SFTables();
            tabs.Retrieve(SFTableAttr.FK_SFDBSrc, this.No);
            if (tabs.Count != 0)
            {
                str += "如下 table 使用了该数据源，您不能删除它。";
                foreach (SFTable tab in tabs)
                {
                    str += "@\t\n" + tab.No + " - " + tab.Name;
                }
            }

            if (str != "")
                throw new Exception("@删除数据源的时候检查，是否有引用，出现错误：" + str);

            return base.beforeDelete();
        }
        protected override bool beforeUpdate()
        {
            if (this.No == "local")
                throw new Exception("@默认连接(local)不允许删除、更新.");
            return base.beforeUpdate();
        }
        //added by liuxc,2015-11-10,新建修改时，判断只能加一个本地主库数据源
        protected override bool beforeUpdateInsertAction()
        {
            if (this.No != "local" && this.DBSrcType == Sys.DBSrcType.Localhost)
                throw new Exception("@在该系统中只能有一个本地连接，请选择其他数据源类型。");

            return base.beforeUpdateInsertAction();
        }
        #endregion 方法.

    }
    /// <summary>
    /// 数据源s
    /// </summary>
    public class SFDBSrcs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 数据源s
        /// </summary>
        public SFDBSrcs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFDBSrc();
            }
        }
        #endregion

        public override int RetrieveAll()
        {
            int i = this.RetrieveAllFromDBSource();
            if (i == 0)
            {
                SFDBSrc src = new SFDBSrc();
                src.No = "local";
                src.Name = "应用系统主数据库(默认)";
                src.Insert();
                this.AddEntity(src);
                return 1;
            }
            return i;
        }
        /// <summary>
        /// 查询数据源
        /// </summary>
        /// <returns>返回查询的个数</returns>
        public int RetrieveDBSrc()
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(SFDBSrcAttr.DBSrcType, " < ", 100);
            int i= qo.DoQuery();
            if (i == 0)
                return this.RetrieveAll();
            return i;
        }
        /// <summary>
        /// 查询数据源
        /// </summary>
        /// <returns>返回查询的个数</returns>
        public int RetrieveWCSrc()
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(SFDBSrcAttr.DBSrcType, "= ", (int) DBSrcType.WebServices );
            int i = qo.DoQuery();
            if (i == 0)
                return this.RetrieveAll();
            return i;
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SFDBSrc> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFDBSrc>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFDBSrc> Tolist()
        {
            System.Collections.Generic.List<SFDBSrc> list = new System.Collections.Generic.List<SFDBSrc>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFDBSrc)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
