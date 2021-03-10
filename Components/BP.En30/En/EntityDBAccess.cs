﻿/*
简介：负责存取数据的类
创建时间：2002-10
最后修改时间：2002-10
*/
using System;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections; 
using System.Collections.Specialized;
using System.Web;
using BP.DA;
using BP.Pub;
using BP.Sys;


namespace BP.En
{
	public class EntityDBAccess
	{
		#region 对实体的基本操作
		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="en"></param>
		/// <returns></returns>
		public static int Delete(Entity en) 
		{
			if (en.EnMap.EnType==EnType.View)
				return 0;

			switch(en.EnMap.EnDBUrl.DBUrlType)
			{
				case DBUrlType.AppCenterDSN :
                    return DBAccess.RunSQL(en.SQLCash.Delete, SqlBuilder.GenerParasPK(en) );
				default :
					throw new Exception("@没有设置类型。");
			}
		}
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="en">产生要更新的语句</param>
		/// <param name="keys">要更新的属性(null,认为更新全部)</param>
		/// <returns>sql</returns>
		public static int Update(Entity en, string[] keys)
		{
			if (en.EnMap.EnType==EnType.View)
				return 0;

            var paras= SqlBuilder.GenerParas(en, keys);
            string sql = en.SQLCash.GetUpdateSQL(en, keys);
            try
            {
                switch (en.EnMap.EnDBUrl.DBUrlType)
                {
                    case DBUrlType.AppCenterDSN:
                        switch (SystemConfig.AppCenterDBType)
                        {
                            case DBType.MSSQL:
                            case DBType.Oracle:
                            case DBType.MySQL:
                            case DBType.PostgreSQL:
                                return DBAccess.RunSQL(sql, paras);
                            case DBType.Informix:
                                return DBAccess.RunSQL(en.SQLCash.GetUpdateSQL(en, keys), SqlBuilder.GenerParas_Update_Informix(en, keys));
                            case DBType.Access:
                                return DBAccess.RunSQL(SqlBuilder.UpdateOfMSAccess(en, keys));
                            default:
                                //return DBAccess.RunSQL(en.SQLCash.GetUpdateSQL(en, keys),
                                //    SqlBuilder.GenerParas(en, keys));
                                if (keys != null)
                                {
                                    Paras ps = new Paras();
                                    Paras myps = SqlBuilder.GenerParas(en, keys);
                                    foreach (Para p in myps)
                                    {
                                        foreach (string s in keys)
                                        {
                                            if (s == p.ParaName)
                                            {
                                                ps.Add(p);
                                                break;
                                            }
                                        }
                                    }
                                    return DBAccess.RunSQL(en.SQLCash.GetUpdateSQL(en, keys), ps);
                                }
                                else
                                {
                                    return DBAccess.RunSQL(en.SQLCash.GetUpdateSQL(en, keys), 
                                        SqlBuilder.GenerParas(en, keys));
                                }
                                break;

                        }
                    //case DBUrlType.DBAccessOfMSSQL1:
                    //    return DBAccessOfMSSQL1.RunSQL(SqlBuilder.Update(en, keys));
                    //case DBUrlType.DBAccessOfMSSQL2:
                    //    return DBAccessOfMSSQL2.RunSQL(SqlBuilder.Update(en, keys));
                    //case DBUrlType.DBAccessOfOracle1:
                    //    return DBAccessOfOracle1.RunSQL(SqlBuilder.Update(en, keys));
                    //case DBUrlType.DBAccessOfOracle2:
                    //    return DBAccessOfOracle2.RunSQL(SqlBuilder.Update(en, keys));
                    default:
                        throw new Exception("@没有设置类型。");
                }
            }
            catch (Exception ex)
            {
                if (SystemConfig.IsDebug)
                    en.CheckPhysicsTable();
                throw ex;
            }
		}
		#endregion 
	
		#region 产生序列号码方法
		 
		#endregion

        public static int RetrieveV2(Entity en, string sql,Paras paras)
        {
            try
            {
                DataTable dt = new DataTable();
                switch (en.EnMap.EnDBUrl.DBUrlType)
                {
                    case DBUrlType.AppCenterDSN:
                        dt = DBAccess.RunSQLReturnTable(sql, paras);
                        break;
                    case DBUrlType.DBAccessOfMSSQL1:
                        dt = DBAccessOfMSSQL1.RunSQLReturnTable(sql);
                        break;
                    case DBUrlType.DBAccessOfMSSQL2:
                        dt = DBAccessOfMSSQL2.RunSQLReturnTable(sql);
                        break;
                    //case DBUrlType.DBAccessOfOracle1:
                    //    dt = DBAccessOfOracle1.RunSQLReturnTable(sql);
                    //    break;
                    //case DBUrlType.DBAccessOfOracle2:
                    //    dt = DBAccessOfOracle2.RunSQLReturnTable(sql);
                    //    break;
                    default:
                        throw new Exception("@没有设置DB类型。");
                }

                if (dt.Rows.Count == 0)
                    return 0;
                Attrs attrs = en.EnMap.Attrs;
                EntityDBAccess.fullDate(dt, en, attrs);
                int i = dt.Rows.Count;
                dt.Dispose();
                return i;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public static int Retrieve(Entity en, string sql, Paras paras)
        {
            DataTable dt ;
            switch (en.EnMap.EnDBUrl.DBUrlType)
            {
                case DBUrlType.AppCenterDSN:
                    dt = DBAccess.RunSQLReturnTable(sql, paras);
                    break;
                case DBUrlType.DBAccessOfMSSQL1:
                    dt = DBAccessOfMSSQL1.RunSQLReturnTable(sql,paras);
                    break;
                case DBUrlType.DBAccessOfMSSQL2:
                    dt = DBAccessOfMSSQL2.RunSQLReturnTable(sql, paras);
                    break;
                //case DBUrlType.DBAccessOfOracle1:
                //    dt = DBAccessOfOracle1.RunSQLReturnTable(sql);
                //    break;
                //case DBUrlType.DBAccessOfOracle2:
                //    dt = DBAccessOfOracle2.RunSQLReturnTable(sql);
                //    break;
                default:
                    throw new Exception("@没有设置DB类型。");
            }

            if (dt.Rows.Count == 0)
                return 0;
            Attrs attrs = en.EnMap.Attrs;
            EntityDBAccess.fullDate(dt, en, attrs);
            int num = dt.Rows.Count;
            dt.Dispose();
            return num;
        }
		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="en">实体</param>
		/// <param name="sql">组织的查询语句</param>
		/// <returns></returns>
        public static int Retrieve(Entity en, string sql)
        {
            try
            {
                DataTable dt = new DataTable();
                switch (en.EnMap.EnDBUrl.DBUrlType)
                {
                    case DBUrlType.AppCenterDSN:
                        dt = DBAccess.RunSQLReturnTable(sql);
                        break;
                    case DBUrlType.DBAccessOfMSSQL1:
                        dt = DBAccessOfMSSQL1.RunSQLReturnTable(sql);
                        break;
                    case DBUrlType.DBAccessOfMSSQL2:
                        dt = DBAccessOfMSSQL2.RunSQLReturnTable(sql);
                        break;
                    //case DBUrlType.DBAccessOfOracle1:
                    //    dt = DBAccessOfOracle1.RunSQLReturnTable(sql);
                    //    break;
                    //case DBUrlType.DBAccessOfOracle2:
                    //    dt = DBAccessOfOracle2.RunSQLReturnTable(sql);
                    //    break;
                    default:
                        throw new Exception("@没有设置DB类型。");
                }

                if (dt.Rows.Count == 0)
                    return 0;
                Attrs attrs = en.EnMap.Attrs;
                EntityDBAccess.fullDate(dt, en, attrs);
                int i = dt.Rows.Count;
                dt.Dispose();
                return i;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
		private static void fullDate(DataTable dt, Entity en, Attrs attrs )
		{
            //判断是否加密.
            if (en.EnMap.IsJM == false)
            {
                foreach (Attr attr in attrs)
                {
                    en.Row.SetValByKey(attr.Key, dt.Rows[0][attr.Key]);
                }
                return;
            }
            
            //执行解密. @yln.
            foreach (Attr attr in attrs)
            {
                var val = dt.Rows[0][attr.Key];

                if (attr.IsPK == false && attr.MyDataType == DataType.AppString)
                    val = val;

                en.Row.SetValByKey(attr.Key, val);
            }

        }
        public static int Retrieve(Entities ens, string sql)
        {
            try
            {
                DataTable dt = new DataTable();
                switch (ens.GetNewEntity.EnMap.EnDBUrl.DBUrlType)
                {
                    case DBUrlType.AppCenterDSN:
                        dt = DBAccess.RunSQLReturnTable(sql);
                        break;
                    case DBUrlType.DBAccessOfMSSQL1:
                        dt = DBAccessOfMSSQL1.RunSQLReturnTable(sql);
                        break;
                    case DBUrlType.DBAccessOfMSSQL2:
                        dt = DBAccessOfMSSQL2.RunSQLReturnTable(sql);
                        break;
                    default:
                        throw new Exception("@没有设置DB类型。");
                }

                if (dt.Rows.Count == 0)
                    return 0;

                Map enMap = ens.GetNewEntity.EnMap;
                Attrs attrs = enMap.Attrs;

                //Entity  en1 = ens.GetNewEntity;
                foreach (DataRow dr in dt.Rows)
                {
                    Entity en = ens.GetNewEntity;
                    //Entity  en = en1.CreateInstance();
                    foreach (Attr attr in attrs)
                    {
                        en.Row.SetValByKey(attr.Key, dr[attr.Key]);
                    }
                    ens.AddEntity(en);
                }
                int i = dt.Rows.Count;
                dt.Dispose();
                return i;
                //return dt.Rows.Count;
            }
            catch (System.Exception ex)
            {
                // ens.GetNewEntity.CheckPhysicsTable();
                throw new Exception("@在[" + ens.GetNewEntity.EnDesc + "]查询时出现错误:" + ex.Message);
            }
        }
        public static int Retrieve(Entities ens, string sql, Paras paras, string[] fullAttrs)
        {
            DataTable dt =null;
            switch (ens.GetNewEntity.EnMap.EnDBUrl.DBUrlType)
            {
                case DBUrlType.AppCenterDSN:
                    dt = DBAccess.RunSQLReturnTable(sql, paras);
                    break;
                case DBUrlType.DBAccessOfMSSQL1:
                    dt = DBAccessOfMSSQL1.RunSQLReturnTable(sql,paras);
                    break;
                case DBUrlType.DBAccessOfMSSQL2:
                    dt = DBAccessOfMSSQL2.RunSQLReturnTable(sql, paras);
                    break;
                //case DBUrlType.DBAccessOfOracle1:
                //    dt = DBAccessOfOracle1.RunSQLReturnTable(sql);
                //    break;
                //case DBUrlType.DBAccessOfOracle2:
                //    dt = DBAccessOfOracle2.RunSQLReturnTable(sql);
                //    break;
                //case DBUrlType.DBAccessOfOLE:
                //    dt = DBAccessOfOLE.RunSQLReturnTable(sql);
                //    break;
                default:
                    throw new Exception("@没有设置DB类型。");
            }

            if (dt.Rows.Count == 0)
                return 0;

            //设置查询.
            QueryObject.InitEntitiesByDataTable(ens, dt, fullAttrs);

            int i = dt.Rows.Count;
            dt.Dispose();
            return i;
            //return dt.Rows.Count;
        }
	}
	
}
