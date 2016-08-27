using System;
using System.Threading;
using System.Collections;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web.Controls;
using BP.Web;

namespace BP.DTS
{
    /// <summary>
    /// 从webservice中获取数据.
    /// </summary>
    public class GenerPortalFromWS : DataIOEn
    {
        public GenerPortalFromWS()
        {
            this.HisDoType = DoType.UnName;
            this.Title = "从Webservices里面更新Portal数据.";
        }
        public override void Do()
        {

        }
    }
    public class AddEmpLeng : DataIOEn2
    {
        public AddEmpLeng()
        {
            this.HisDoType = DoType.UnName;
            this.Title = "为操作员编号长度生级";
            this.HisRunTimeType = RunTimeType.UnName;
            this.FromDBUrl = DBUrlType.AppCenterDSN;
            this.ToDBUrl = DBUrlType.AppCenterDSN;
        }

        public override void Do()
        {
            string sql = "";
            string sql2 = "";

            Log.DebugWriteInfo("ssssssssssssssssssss");

            ArrayList al = ClassFactory.GetObjects("BP.En.Entity");
            foreach (object obj in al)
            {
                Entity en = obj as Entity;
                Map map = en.EnMap;

                try
                {
                    if (map.IsView)
                        continue;
                }
                catch
                {
                }

                string table = en.EnMap.PhysicsTable;
                foreach (Attr attr in map.Attrs)
                {
                    if (attr.Key.IndexOf("Text") != -1)
                        continue;

                    if (attr.Key == "Rec" || attr.Key == "FK_Emp" || attr.UIBindKey == "BP.Port.Emps")
                    {
                        sql += "\n update " + table + " set " + attr.Key + "='01'||" + attr.Key + " WHERE length(" + attr.Key + ")=6;";
                    }
                    else if (attr.Key == "Checker")
                    {
                        sql2 += "\n update " + table + " set " + attr.Key + "='01'||" + attr.Key + " WHERE length(" + attr.Key + ")=6;";
                    }
                }
            }
            Log.DebugWriteInfo(sql);
            Log.DebugWriteInfo("===========================" + sql2);
        }
    }
	/// <summary>
	/// 执行类型
	/// </summary>
	public enum DoType
	{
		/// <summary>
		/// 没有指定
		/// </summary>
		UnName,
		/// <summary>
		/// 先删除后插入,适合任何情况,但是运行效率较低．
		/// </summary>
		DeleteInsert,
		/// <summary>
		/// 增量同步，适合增量的情况,比如纳税人的纳税信息.
		/// 对于增量以前的部分，不更新．
		/// </summary>
		Incremental,
		/// <summary>
		/// 同步,适合任意中情况,比如纳税人．
		/// </summary>
		Inphase,
		/// <summary>
		/// 直接的导入.把一个表从另外的一个地方导入近来．
		/// </summary>
		Directly,
		/// <summary>
		/// 特殊的．
		/// </summary>
		Especial
	}
	/// <summary>
	/// 调度
	/// </summary>
	abstract public class DataIOEn :DataIOEn2
	{
		public bool Enable=true;
	}
	
	/// <summary>
	/// EnMap 的摘要说明。
	/// </summary>
	abstract public class DataIOEn2
	{
		 
		/// <summary>
		/// 获取在 DTS 中的编号。
		/// </summary>
		/// <returns></returns>
		public string GetNoInDTS()
		{
            //DTS.SysDTS dts =new SysDTS();
            //QueryObject qo = new QueryObject(dts);
            //qo.AddWhere(DTSAttr.RunText,this.ToString());
            //if (qo.DoQuery()==0)
            //    throw new Exception("没有取道调度的编号.");
            //else
            //    return dts.No;

            return null;
		}
        /// <summary>
        /// 执行它 在线程中。
        /// </summary>
        public void DoItInThread()
        {
            ThreadStart ts = new ThreadStart(this.Do);
            Thread thread = new Thread(ts);
            thread.Start();
        }

		#region Directly 
		 
		#endregion

		#region 基本属性.
		/// <summary>
		/// 选择sql .
		/// </summary>
		public string SELECTSQL=null;
		/// <summary>
		/// 数据同步类型．
		/// </summary>
		public DoType HisDoType = DoType.UnName;
		/// <summary>
		/// 运行类型时间
		/// </summary>
		public RunTimeType HisRunTimeType = RunTimeType.UnName;
        /// <summary>
        /// 标题
        /// </summary>
		public string Title="未命名数据同步";
		/// <summary>
		/// WHERE .
		/// </summary>
		public string FromWhere=null;
		/// <summary>
		/// FFs
		/// </summary>
		public FFs FFs=null;
		/// <summary>
		/// 从Table .
		/// </summary>
		public string FromTable=null;
		/// <summary>
		/// 到Table.
		/// </summary>
		public string ToTable=null;
		/// <summary>
		/// 从DBUrl.
		/// </summary>
		public DBUrlType FromDBUrl;
		/// <summary>
		/// 到DBUrl.
		/// </summary>
		public DBUrlType ToDBUrl;
		/// <summary>
		/// 更新语句
		/// </summary>
		public string UPDATEsql;
		/// <summary>
		/// 备注
		/// </summary>
		public string Note="无";

		public string DefaultEveryMonth="99";
		public string DefaultEveryDay="99";
		public string DefaultEveryHH="99";
		public string DefaultEveryMin="99";
		/// <summary>
		/// 类别
		/// </summary>
		public string FK_Sort="0";


//		/// <summary>
//		/// 增量更新的数据源sql.
//		/// 用这个sql，查询出来一个结果集合，这个集合用于更新的集合。
//		/// 一般的来说，这个sql是根据当前的月份自动生成的。
//		/// </summary>
//		public string IncrementalDBSourceSQL;
		#endregion

		/*
		 * 根据现实的情况我们把调度分为以下几种。
		 * 1，增量调度。
		 *    例：纳税人纳税信息，特点：
		 *   a,数据与时间成增量的增加。
		 *   b,月份以前的数据不变化。
		 * 
		 *   总结：原表数据随时间只增加，增加前的数据不变化。         
		 * 
		 * 2，变化调度。
		 *   例：纳税人信息。
		 *   特点：源表纳税人数据增，删，改，都有可能发生。
		 *
		 * 3，删除方式同步．
		 *   步骤：
		 * 　１，先删除．
		 * 　２，插入新的数据． 
		 * */
		/// <summary>
		/// 调度
		/// </summary>
		public  DataIOEn2(){}


		/// <summary>
		/// 直接道入
		/// </summary>
		/// <param name="fromSQL">sql</param>
		/// <param name="toPTable">table</param>
		/// <param name="pk">key,用于建立索引与pk</param>
		public void Directly(string fromSQL, string toPTable, string pk)
		{
			this.Directly(fromSQL,toPTable);
			this.ToDBUrlRunSQL("CREATE INDEX "+toPTable+"ID ON "+toPTable+" ("+pk+")");
		}
		/// <summary>
		/// 直接道入
		/// </summary>
		/// <param name="fromSQL"></param>
		/// <param name="toPTable"></param>
		/// <param name="pk1"></param>
		/// <param name="pk2"></param>
		public void Directly(string fromSQL, string toPTable, string pk1,string pk2)
		{
			this.Directly(fromSQL,toPTable);
			this.ToDBUrlRunSQL("CREATE INDEX "+toPTable+"ID ON "+toPTable+" ("+pk1+","+pk2+")");
		}
		/// <summary>
		/// 直接道入
		/// </summary>
		/// <param name="fromSQL"></param>
		/// <param name="toPTable"></param>
		/// <param name="pk1"></param>
		/// <param name="pk2"></param>
		public void Directly(string fromSQL, string toPTable, string pk1,string pk2,string pk3)
		{
			this.Directly(fromSQL,toPTable);
			this.ToDBUrlRunSQL("CREATE INDEX "+toPTable+"ID ON "+toPTable+" ("+pk1+","+pk2+","+pk3+")");
		}
		/// <summary>
		/// 直接从另外一个数据库中，把数据导入到，目标数据库．
		/// 对于复杂的导入可以用这种方式,进行特殊处理．
		/// 数据源泉是 SELECTSQL ,指定的sql.
		/// selectsql 只能识别如下数据类型．
		/// char, int , float, decimal . 
		/// 如果不能适合以上的数据类型．请转换为以上的数据类型．　
		/// </summary>
		public void Directly(string fromSQL, string toPTable)
		{
			// 得到数据源泉．
			DataTable dt =this.FromDBUrlRunSQLReturnTable( fromSQL ); 
 
			#region 形成 insert into 的前一部分．
			string sql=null;
			sql="INSERT INTO "+toPTable+"(";
			foreach(DataColumn dc in dt.Columns )
			{
				sql+=dc.ColumnName+",";
			}
			sql=sql.Substring(0,sql.Length-1);
			sql+=") VALUES (";
			#endregion


			// 删除目的表数据．
			try
			{
				this.ToDBUrlRunSQL(" drop table "+ toPTable  );
			}
			catch
			{
			}

			// 建立一个新表。
			string createTable="CREATE TABLE "+toPTable+" (";
			foreach(DataColumn dc in dt.Columns)
			{
				switch(dc.DataType.ToString())
				{
					case "System.String":
						// 取到最大的长度。
//						int len=0;
//						foreach(DataRow dr in dt.Rows)
//						{
//							if (len < dr[dc.ColumnName].ToString().Length )
//								len=dr[dc.ColumnName].ToString().Length;
//						}
//						len+=10;
						createTable+=dc.ColumnName+" nvarchar (700) NULL  ," ;
						break;
					case "System.Int16":
					case "System.Int32":
					case "System.Int64":
						createTable+=dc.ColumnName+" int NULL," ;
						break;
					case "System.Decimal":
						createTable+=dc.ColumnName+" decimal NULL,";
						break;
					default:
						createTable+=dc.ColumnName+" float NULL,"; 
						break;
				}
			}
			createTable=createTable.Substring(0,createTable.Length-1);
			createTable+=")";
			this.ToDBUrlRunSQL(createTable);



			string sql2=null; 
			// 遍例数据源，inset 到目的表．　
			string errormsg="";
			foreach(DataRow dr in dt.Rows)
			{
				sql2=sql;
				foreach(DataColumn dc in dt.Columns)
				{
					sql2+="'"+dr[dc.ColumnName]+"',";
				}
				sql2=sql2.Substring(0,sql2.Length-1)+")";
				try
				{
					this.ToDBUrlRunSQL(sql2);
				}
				catch(Exception ex)
				{
					errormsg+=ex.Message;
				}
			}
			if (errormsg!="")
				throw new Exception(" data output error: "+errormsg );

		}
		#region 公共方法．
		/// <summary>
		/// 数据源 run sql ,返回table .
		/// </summary>
		/// <param name="selectSql"></param>
		/// <returns></returns>
		public DataTable FromDBUrlRunSQLReturnTable(string selectSql)
		{
			// 得到数据源．
			DataTable dt = new DataTable();
			switch(this.FromDBUrl)
			{
				case DBUrlType.AppCenterDSN:
					dt=DBAccess.RunSQLReturnTable( selectSql);
					break;
				case DBUrlType.DBAccessOfMSSQL1:
                    dt = DBAccessOfMSSQL1.RunSQLReturnTable(selectSql);
					break;
                case DBUrlType.DBAccessOfMSSQL2:
                    dt = DBAccessOfMSSQL2.RunSQLReturnTable(selectSql);
                    break;
				case DBUrlType.DBAccessOfODBC:
					dt=DBAccessOfODBC.RunSQLReturnTable( selectSql );
					break;
				case DBUrlType.DBAccessOfOLE:
					dt=DBAccessOfOLE.RunSQLReturnTable( selectSql );
					break;
				case DBUrlType.DBAccessOfOracle1:
					dt=DBAccessOfOracle1.RunSQLReturnTable( selectSql );
					break;
                case DBUrlType.DBAccessOfOracle2:
                    dt = DBAccessOfOracle2.RunSQLReturnTable(selectSql);
                    break;
				default:
					break;
			}
			return dt;
		}
		public int ToDBUrlRunSQL(string sql)
		{
			switch(this.ToDBUrl)
			{
				case DBUrlType.AppCenterDSN:
					return DBAccess.RunSQL(sql);
				case DBUrlType.DBAccessOfMSSQL1:
					return DBAccessOfMSSQL1.RunSQL(sql);
                case DBUrlType.DBAccessOfMSSQL2:
                    return DBAccessOfMSSQL2.RunSQL(sql);
				case DBUrlType.DBAccessOfODBC:
					return DBAccessOfODBC.RunSQL(sql);
				case DBUrlType.DBAccessOfOLE:
					return DBAccessOfOLE.RunSQL(sql);
				case DBUrlType.DBAccessOfOracle1:
					return DBAccessOfOracle1.RunSQL(sql);
                case DBUrlType.DBAccessOfOracle2:
                    return DBAccessOfOracle2.RunSQL(sql);
				default:
					throw new Exception("@ error it");
			}
		}
		public int ToDBUrlRunDropTable(string table)
		{
			switch(this.ToDBUrl)
			{
				case DBUrlType.AppCenterDSN:
					return DBAccess.RunSQLDropTable(table);
				case DBUrlType.DBAccessOfMSSQL1:
					return DBAccessOfMSSQL1.RunSQL(table);
                case DBUrlType.DBAccessOfMSSQL2:
                    return DBAccessOfMSSQL2.RunSQL(table);
				case DBUrlType.DBAccessOfODBC:
					return DBAccessOfODBC.RunSQL(table);
				case DBUrlType.DBAccessOfOLE:
					return DBAccessOfOLE.RunSQL(table);
				case DBUrlType.DBAccessOfOracle1:
					return DBAccessOfOracle1.RunSQLTRUNCATETable(table);
                case DBUrlType.DBAccessOfOracle2:
                    return DBAccessOfOracle2.RunSQLTRUNCATETable(table);
				default:
					throw new Exception("@ error it");
			}
		}
		/// <summary>
		/// 是否存在?
		/// </summary>
		/// <param name="sql">要判断的sql</param>
		/// <returns></returns>
		public bool ToDBUrlIsExit(string sql)
		{
			switch(this.ToDBUrl)
			{
				case DBUrlType.AppCenterDSN:
					return DBAccess.IsExits(sql);
				case DBUrlType.DBAccessOfMSSQL1:
					return DBAccessOfMSSQL1.IsExits(sql);
                case DBUrlType.DBAccessOfMSSQL2:
                    return DBAccessOfMSSQL2.IsExits(sql);
				case DBUrlType.DBAccessOfODBC:
					return DBAccessOfODBC.IsExits(sql);
				case DBUrlType.DBAccessOfOLE:
					return DBAccessOfOLE.IsExits(sql);
				case DBUrlType.DBAccessOfOracle1:
					return DBAccessOfOracle1.IsExits(sql);
                case DBUrlType.DBAccessOfOracle2:
                    return DBAccessOfOracle2.IsExits(sql);
				default:
					throw new Exception("@ error it");
			}
		}
		#endregion

		#region 方法， New   2005-01-29

		/// <summary>
		/// 执行，用于子类的重写。
		/// </summary>
		public virtual void Do()
		{
			if ( this.HisDoType==DoType.UnName)
				throw new Exception("@没有说明同步的类型,请在基础信息里面设置同步的类型(构造函数．)．");

			if (this.HisDoType==DoType.DeleteInsert)
				this.DeleteInsert();

			if (this.HisDoType==DoType.Inphase)
				this.Inphase();

			if (this.HisDoType==DoType.Incremental)
				this.Incremental();
		}

		#region 用于增量调度
		/// <summary>
		/// 增量调度：
		/// 比如： 纳税人的纳税信息。
		/// 特点：1， 数据与时间成增量的增加。
		///       2， 月份以前的数据不变化。
		/// </summary>
		public void Incremental()
		{
			/*
			 * 实现步骤：
			 * 1，组成sql.
			 * 2，执行更新。
			 *  
			 * */
			this.DoBefore();  // 调用，更新前的业务逻辑处理。

			#region  得到要更新的数据源。
			DataTable FromDataTable= this.GetFromDataTable();
			#endregion

			#region 开始执行更新。
			string isExitSql="";
			string InsertSQL="";
			//遍历 数据源表.
			foreach(DataRow FromDR in FromDataTable.Rows)
			{
				#region 判断是否存在．
				/* 判断是否存在，如果存在continue. 不存在就 insert.  */
			    isExitSql="SELECT * FROM "+this.ToTable+" WHERE ";
				foreach(FF ff in this.FFs)
				{
					if (ff.IsPK==false)
						continue;
					isExitSql+= ff.ToField +"='"+FromDR[ff.FromField]+ "' AND ";
				}

				isExitSql=isExitSql.Substring(0,isExitSql.Length-5);

				if (DBAccess.IsExits(isExitSql))  //如果不存在就 insert . 
					continue;
				#endregion  判断是否存在

				#region 执行插入操作
				InsertSQL="INSERT INTO "+this.ToTable +"(";
				foreach(FF ff in this.FFs)
				{
					InsertSQL+=ff.ToField.ToString()+",";
				}
				InsertSQL=InsertSQL.Substring(0,InsertSQL.Length-1);
				InsertSQL+=") values(";
				foreach(FF ff in this.FFs)
				{
					if(ff.DataType==DataType.AppString||ff.DataType==DataType.AppDateTime)
					{
						InsertSQL+="'"+FromDR[ff.FromField].ToString()+"',";
					}
					else
					{
						InsertSQL+=FromDR[ff.FromField].ToString()+",";
					}
				}
				InsertSQL=InsertSQL.Substring(0,InsertSQL.Length-1);
				InsertSQL+=")";
				switch(this.ToDBUrl)
				{
					case DA.DBUrlType.AppCenterDSN:
						DBAccess.RunSQL(InsertSQL);
						break;
					case DA.DBUrlType.DBAccessOfMSSQL1:
                        DBAccessOfMSSQL1.RunSQL(InsertSQL);
						break;
                    case DA.DBUrlType.DBAccessOfMSSQL2:
                        DBAccessOfMSSQL2.RunSQL(InsertSQL);
                        break;
					case DA.DBUrlType.DBAccessOfOLE:
						DBAccessOfOLE.RunSQL(InsertSQL);
						break;
					case DA.DBUrlType.DBAccessOfOracle1:
						DBAccessOfOracle1.RunSQL(InsertSQL);
						break;
                    case DA.DBUrlType.DBAccessOfOracle2:
                        DBAccessOfOracle2.RunSQL(InsertSQL);
                        break;
					case DA.DBUrlType.DBAccessOfODBC:
						DBAccessOfODBC.RunSQL(InsertSQL);
						break;
					default:
						break;
				}
				#endregion 执行插入操作

			}
			#endregion 结束,开始执行更新

			this.DoAfter(); // 调用，更新之后的业务处理。
		}
		/// <summary>
		/// 增量调度以前要执行的方法。
		/// </summary>
		protected virtual void DoBefore()
		{
		}
		/// <summary>
		/// 增量调度之后要执行的方法。
		/// </summary>
		protected virtual void DoAfter()
		{
		}
		#endregion

		#region 删除(清空) 之后插入(适合任何一种数据调度)
		/// <summary>
		/// 删除之后插入, 用于数据量不太大,更新频率不太频繁的数据处理.
		/// </summary>
		public  void DeleteInsert()
		{
			this.DoBefore(); //调用业务处理。
			// 得到源表.
			DataTable FromDataTable= this.GetFromDataTable();
			this.DeleteObjData();

			#region  遍历源表 插入操作
			string InsertSQL="";
			foreach(DataRow FromDR in FromDataTable.Rows)
			{
				 
				InsertSQL="INSERT INTO "+this.ToTable +"(";
				foreach(FF ff in this.FFs)
				{
					InsertSQL+=ff.ToField.ToString()+",";
				}
				InsertSQL=InsertSQL.Substring(0,InsertSQL.Length-1);
				InsertSQL+=") values(";
				foreach(FF ff in this.FFs)
				{
					if(ff.DataType==DataType.AppString||ff.DataType==DataType.AppDateTime)
						InsertSQL+="'"+FromDR[ff.FromField].ToString()+"',";
					else
						InsertSQL+=FromDR[ff.FromField].ToString()+",";
				}
				InsertSQL=InsertSQL.Substring(0,InsertSQL.Length-1);
				InsertSQL+=")";
				
				switch(this.ToDBUrl)
				{
					case DA.DBUrlType.AppCenterDSN:
						DBAccess.RunSQL(InsertSQL);
						break;
					case DA.DBUrlType.DBAccessOfMSSQL1:
						DBAccessOfMSSQL1.RunSQL(InsertSQL);
						break;
                    case DA.DBUrlType.DBAccessOfMSSQL2:
                        DBAccessOfMSSQL2.RunSQL(InsertSQL);
                        break;
					case DA.DBUrlType.DBAccessOfOLE:
						DBAccessOfOLE.RunSQL(InsertSQL);
						break;
					case DA.DBUrlType.DBAccessOfOracle1:
                        DBAccessOfOracle1.RunSQL(InsertSQL);
						break;
                    case DA.DBUrlType.DBAccessOfOracle2:
                        DBAccessOfOracle2.RunSQL(InsertSQL);
                        break;
					case DA.DBUrlType.DBAccessOfODBC:
						DBAccessOfODBC.RunSQL(InsertSQL);
						break;
					default:
						break;
				}
				 
			}
			#endregion

			

			this.DoAfter(); // 调用业务处理。

		}
		public void DeleteObjData()
		{
			#region 删除表内容
			switch(this.ToDBUrl)
			{
				case DA.DBUrlType.AppCenterDSN:
                    DBAccess.RunSQL("DELETE FROM  " + this.ToTable);
					break;
				case DA.DBUrlType.DBAccessOfMSSQL1:
                    DBAccessOfMSSQL1.RunSQL("DELETE  FROM " + this.ToTable);						
					break;
                case DA.DBUrlType.DBAccessOfMSSQL2:
                    DBAccessOfMSSQL2.RunSQL("DELETE  FROM " + this.ToTable);
                    break;
				case DA.DBUrlType.DBAccessOfOLE:
                    DBAccessOfOLE.RunSQL("DELETE FROM  " + this.ToTable);
					break;
				case DA.DBUrlType.DBAccessOfOracle1:
                    DBAccessOfOracle1.RunSQL("DELETE  FROM " + this.ToTable);
					break;
                case DA.DBUrlType.DBAccessOfOracle2:
                    DBAccessOfOracle2.RunSQL("DELETE  FROM " + this.ToTable);
                    break;
				case DA.DBUrlType.DBAccessOfODBC:
                    DBAccessOfODBC.RunSQL("DELETE FROM  " + this.ToTable);
					break;
				default:
					break;
			}
			#endregion
		}
		 
		#endregion

		#region 同步数据。
		/// <summary>
		/// 得到数据源。
		/// </summary>
		/// <returns></returns>
		public DataTable GetToDataTable()
		{
			string sql="SELECT * FROM "+this.ToTable;
			DataTable FromDataTable = new DataTable();
			switch(this.ToDBUrl)
			{
				case DA.DBUrlType.AppCenterDSN:
					FromDataTable=DBAccess.RunSQLReturnTable(sql);
					break;
				case DA.DBUrlType.DBAccessOfMSSQL2:
                    FromDataTable = DBAccessOfMSSQL2.RunSQLReturnTable(sql);
					break;
                case DA.DBUrlType.DBAccessOfMSSQL1:
                    FromDataTable = DBAccessOfMSSQL1.RunSQLReturnTable(sql);
                    break;
				case DA.DBUrlType.DBAccessOfOLE:
					FromDataTable=DBAccessOfOLE.RunSQLReturnTable(sql);
					break;
				case DA.DBUrlType.DBAccessOfOracle1:
                    FromDataTable = DBAccessOfOracle1.RunSQLReturnTable(sql);
					break;
                case DA.DBUrlType.DBAccessOfOracle2:
                    FromDataTable = DBAccessOfOracle2.RunSQLReturnTable(sql);
                    break;
				case DA.DBUrlType.DBAccessOfODBC:
					FromDataTable=DBAccessOfODBC.RunSQLReturnTable(sql);
					break;
				default:
					throw new Exception("the to dburl error DBUrlType ");
			}

			return FromDataTable;

		}
		/// <summary>
		/// 得到数据源。
		/// </summary>
		/// <returns>数据源</returns> 
		public DataTable GetFromDataTable()
		{
			string FromSQL="SELECT ";
			foreach(FF ff in this.FFs)
			{
				//对日期型的判断
				if(ff.DataType==DataType.AppDateTime)
				{
					FromSQL+=" CASE  "+
                        " when datalength( CONVERT(NVARCHAR,datepart(month," + ff.FromField + " )))=1 then datename(year," + ff.FromField + " )+'-'+('0'+CONVERT(NVARCHAR,datepart(month," + ff.FromField + " ))) " +
						" else "+
                        " datename(year," + ff.FromField + " )+'-'+CONVERT(NVARCHAR,datepart(month," + ff.FromField + " )) " +
						" END "+
						" AS "+ff.FromField+" , ";
				}
				else
				{
					FromSQL+=ff.FromField+",";
				}
			}

			FromSQL=FromSQL.Substring(0,FromSQL.Length-1);
			FromSQL+=" from "+ this.FromTable;
			FromSQL+=this.FromWhere;
			DataTable FromDataTable=new DataTable();
			switch(this.FromDBUrl)
			{
				case DA.DBUrlType.AppCenterDSN:
					FromDataTable=DBAccess.RunSQLReturnTable(FromSQL);
					break;
				case DA.DBUrlType.DBAccessOfMSSQL1:
                    FromDataTable = DBAccessOfMSSQL1.RunSQLReturnTable(FromSQL);
					break;
                case DA.DBUrlType.DBAccessOfMSSQL2:
                    FromDataTable = DBAccessOfMSSQL2.RunSQLReturnTable(FromSQL);
                    break;
				case DA.DBUrlType.DBAccessOfOLE:
					FromDataTable=DBAccessOfOLE.RunSQLReturnTable(FromSQL);
					break;
				case DA.DBUrlType.DBAccessOfOracle1:
                    FromDataTable = DBAccessOfOracle1.RunSQLReturnTable(FromSQL);
					break;
                case DA.DBUrlType.DBAccessOfOracle2:
                    FromDataTable = DBAccessOfOracle2.RunSQLReturnTable(FromSQL);
                    break;
				case DA.DBUrlType.DBAccessOfODBC:
					FromDataTable=DBAccessOfODBC.RunSQLReturnTable(FromSQL);
					break;
				default:
					throw new Exception("the from dburl error DBUrlType ");
			}
			return FromDataTable;
		}
		#endregion

		#endregion end 方法New peng 2005-01-29

		#region 方法
		/// <summary>
		/// 同步更新.
		/// </summary>
		public void Inphase()
		{
			#region 得到源表
			this.DoBefore();

			string FromSQL="SELECT ";
			foreach(FF ff in this.FFs)
			{
				//对日期型的判断
				if(ff.DataType==DataType.AppDateTime)
				{
					FromSQL+=" CASE  "+
                        " when datalength( CONVERT(NVARCHAR,datepart(month," + ff.FromField + " )))=1 then datename(year," + ff.FromField + " )+'-'+('0'+CONVERT(NVARCHAR,datepart(month," + ff.FromField + " ))) " +
						" else "+
                        " datename(year," + ff.FromField + " )+'-'+CONVERT(NVARCHAR,datepart(month," + ff.FromField + " )) " +
						" END "+
						" AS "+ff.FromField+" , ";
				}
				else
				{
					FromSQL+=ff.FromField+",";
				}
			}
			FromSQL=FromSQL.Substring(0,FromSQL.Length-1);
			FromSQL+=" from "+ this.FromTable;
			FromSQL+=this.FromWhere;
			DataTable FromDataTable=new DataTable();
			switch(this.FromDBUrl)
			{
				case DA.DBUrlType.AppCenterDSN:
					FromDataTable=DBAccess.RunSQLReturnTable(FromSQL);
					break;
				case DA.DBUrlType.DBAccessOfMSSQL1:
                    FromDataTable = DBAccessOfMSSQL1.RunSQLReturnTable(FromSQL);
					break;
                case DA.DBUrlType.DBAccessOfMSSQL2:
                    FromDataTable = DBAccessOfMSSQL2.RunSQLReturnTable(FromSQL);
                    break;
				case DA.DBUrlType.DBAccessOfOLE:
					FromDataTable=DBAccessOfOLE.RunSQLReturnTable(FromSQL);
					break;
				case DA.DBUrlType.DBAccessOfOracle2:
                    FromDataTable = DBAccessOfOracle2.RunSQLReturnTable(FromSQL);
					break;
                case DA.DBUrlType.DBAccessOfOracle1:
                    FromDataTable = DBAccessOfOracle1.RunSQLReturnTable(FromSQL);
                    break;
				case DA.DBUrlType.DBAccessOfODBC:
					FromDataTable=DBAccessOfODBC.RunSQLReturnTable(FromSQL);
					break;
				default:
					break;
			}
			#endregion

			#region 得到目的表(字段只包含主键)
			string ToSQL="SELECT ";
			foreach(FF ff in this.FFs)
			{
				if (ff.IsPK==false)
					continue;
				ToSQL+=ff.ToField+",";
			}
			ToSQL=ToSQL.Substring(0,ToSQL.Length-1);
			ToSQL+=" FROM "+ this.ToTable;
			DataTable ToDataTable=new DataTable();
			switch(this.ToDBUrl)
			{
				case DA.DBUrlType.AppCenterDSN:
					ToDataTable=DBAccess.RunSQLReturnTable(ToSQL);
					break;
				case DA.DBUrlType.DBAccessOfMSSQL1:
                    ToDataTable = DBAccessOfMSSQL1.RunSQLReturnTable(ToSQL);
					break;
                case DA.DBUrlType.DBAccessOfMSSQL2:
                    ToDataTable = DBAccessOfMSSQL2.RunSQLReturnTable(ToSQL);
                    break;
				case DA.DBUrlType.DBAccessOfOLE:
					ToDataTable=DBAccessOfOLE.RunSQLReturnTable(ToSQL);
					break;
				case DA.DBUrlType.DBAccessOfOracle1:
                    ToDataTable = DBAccessOfOracle1.RunSQLReturnTable(ToSQL);
					break;
                case DA.DBUrlType.DBAccessOfOracle2:
                    ToDataTable = DBAccessOfOracle2.RunSQLReturnTable(ToSQL);
                    break;
				case DA.DBUrlType.DBAccessOfODBC:
					ToDataTable=DBAccessOfODBC.RunSQLReturnTable(ToSQL);
					break;
				default:
					break;
			}
			#endregion

			string SELECTSQL="";
			string InsertSQL="";
			string UpdateSQL="";
			string DeleteSQL="";
			//int i=0;
			//int j=0;
			int result=0;

			#region  遍历源表
			foreach(DataRow FromDR in FromDataTable.Rows)
			{
				UpdateSQL="UPDATE  "+this.ToTable+" SET ";				
				foreach(FF ff in this.FFs)
				{
					switch(ff.DataType)
					{
						case DataType.AppDateTime:
						case DataType.AppString:
							UpdateSQL+=  ff.ToField+ "='"+FromDR[ff.FromField].ToString()+"',";
							break;
						case DataType.AppFloat:
						case DataType.AppInt:
						case DataType.AppMoney:
						case DataType.AppDate:
						case DataType.AppDouble:
							UpdateSQL+=  ff.ToField+ "="+FromDR[ff.FromField].ToString()+"," ;
							break;
						default:
							throw new Exception("没有涉及到的数据类型.");
					}
				}
				UpdateSQL=UpdateSQL.Substring(0,UpdateSQL.Length-1);
				UpdateSQL+=" WHERE ";
				foreach(FF ff in this.FFs)
				{
					if (ff.IsPK==false)
						continue;
					UpdateSQL+= ff.ToField +"='"+FromDR[ff.FromField]+ "' AND ";
				}

				UpdateSQL=UpdateSQL.Substring(0,UpdateSQL.Length-5);
				switch(this.ToDBUrl)
				{
					case DA.DBUrlType.AppCenterDSN:
						result=DBAccess.RunSQL(UpdateSQL);
						break;
					case DA.DBUrlType.DBAccessOfMSSQL1:
						string a=UpdateSQL;
                        result = DBAccessOfMSSQL1.RunSQL(UpdateSQL);						
						break;
                    case DA.DBUrlType.DBAccessOfMSSQL2:
                        string b = UpdateSQL;
                        result = DBAccessOfMSSQL2.RunSQL(UpdateSQL);
                        break;
					case DA.DBUrlType.DBAccessOfOLE:
						result=DBAccessOfOLE.RunSQL(UpdateSQL);						
						break;
					case DA.DBUrlType.DBAccessOfOracle1:
						result=DBAccessOfOracle1.RunSQL(UpdateSQL);	
						break;
                    case DA.DBUrlType.DBAccessOfOracle2:
                        result = DBAccessOfOracle2.RunSQL(UpdateSQL);
                        break;
					case DA.DBUrlType.DBAccessOfODBC:
						result=DBAccessOfODBC.RunSQL(UpdateSQL);		
						break;
					default:
						break;
				}
				if(result==0)
				{
					//插入操作
					InsertSQL="INSERT INTO "+this.ToTable +"(";
					foreach(FF ff in this.FFs)
					{
						InsertSQL+=ff.ToField.ToString()+",";
					}
					InsertSQL=InsertSQL.Substring(0,InsertSQL.Length-1);
					InsertSQL+=") values(";
					foreach(FF ff in this.FFs)
					{
						if(ff.DataType==DataType.AppString||ff.DataType==DataType.AppDateTime)
						{
							InsertSQL+="'"+FromDR[ff.FromField].ToString()+"',";
						}
						else
						{
							InsertSQL+=FromDR[ff.FromField].ToString()+",";
						}
					}
					InsertSQL=InsertSQL.Substring(0,InsertSQL.Length-1);
					InsertSQL+=")";
					switch(this.ToDBUrl)
					{
						case DA.DBUrlType.AppCenterDSN:
							DBAccess.RunSQL(InsertSQL);
							break;
						case DA.DBUrlType.DBAccessOfMSSQL1:
                            DBAccessOfMSSQL1.RunSQL(InsertSQL);
							break;
                        case DA.DBUrlType.DBAccessOfMSSQL2:
                            DBAccessOfMSSQL2.RunSQL(InsertSQL);
                            break;
						case DA.DBUrlType.DBAccessOfOLE:
							DBAccessOfOLE.RunSQL(InsertSQL);
							break;
						case DA.DBUrlType.DBAccessOfOracle1:
                            DBAccessOfOracle1.RunSQL(InsertSQL);
							break;
                        case DA.DBUrlType.DBAccessOfOracle2:
                            DBAccessOfOracle2.RunSQL(InsertSQL);
                            break;
						case DA.DBUrlType.DBAccessOfODBC:
							DBAccessOfODBC.RunSQL(InsertSQL);
							break;
						default:
							break;
					}
				}
				
			}
			#endregion

			#region	遍历目的表 如果该条记录存在,continue,如果该条记录不存在,则根据主键删除目的表的对应数据
			foreach(DataRow ToDR in ToDataTable.Rows)
			{
				SELECTSQL="SELECT ";
				foreach(FF ff in this.FFs)
				{
					if (ff.IsPK==false)
						continue;
					SELECTSQL+=ff.FromField+",";
				}
				SELECTSQL=SELECTSQL.Substring(0,SELECTSQL.Length-1);
				SELECTSQL+=" FROM "+this.FromTable+" WHERE ";
				foreach(FF ff in this.FFs)
				{
					if (ff.IsPK==false)
						continue;
					if(ff.DataType==DataType.AppDateTime)
					{
						SELECTSQL+=" case "+
							" when datalength( CONVERT(NVARCHAR,datepart(month,"+ff.FromField+" )))=1 then datename(year,"+ff.FromField+" )+'-'+('0'+CONVERT(VARCHAR,datepart(month,"+ff.FromField+" ))) "+
							" else "+
							" datename(year,"+ff.FromField+" )+'-'+CONVERT(VARCHAR,datepart(month,"+ff.FromField+" )) "+
							" END "+
							"='"+ToDR[ff.ToField].ToString()+"' AND ";
					}
					else
					{
						if(ff.DataType==DataType.AppString)
							SELECTSQL+=ff.FromField+"='"+ToDR[ff.ToField].ToString()+"' AND ";
						else
							SELECTSQL+=ff.FromField+"="+ToDR[ff.ToField].ToString()+" AND ";
					}
				}
				SELECTSQL=SELECTSQL.Substring(0,SELECTSQL.Length-5);
				//SELECTSQL+=this.FromWhere;
				result=0;
				switch(this.FromDBUrl)
				{
					case DA.DBUrlType.AppCenterDSN:
						result=DBAccess.RunSQLReturnCOUNT(SELECTSQL);
						break;
					case DA.DBUrlType.DBAccessOfMSSQL1:
                        result = DBAccessOfMSSQL1.RunSQLReturnCOUNT(SELECTSQL);
						break;
                    case DA.DBUrlType.DBAccessOfMSSQL2:
                        result = DBAccessOfMSSQL2.RunSQLReturnCOUNT(SELECTSQL);
                        break;
					case DA.DBUrlType.DBAccessOfOLE:
						result=DBAccessOfOLE.RunSQLReturnCOUNT(SELECTSQL);
						break;
					case DA.DBUrlType.DBAccessOfOracle1:
                        result = DBAccessOfOracle1.RunSQL(SELECTSQL);
						break;
                    case DA.DBUrlType.DBAccessOfOracle2:
                        result = DBAccessOfOracle2.RunSQL(SELECTSQL);
                        break;
					case DA.DBUrlType.DBAccessOfODBC:
						result=DBAccessOfODBC.RunSQLReturnCOUNT(SELECTSQL);
						break;
					default:
						break;
				}

				if(result!=1)
				{
					//delete
                    DeleteSQL = "delete FROM  " + this.ToTable + " WHERE ";
					foreach(FF ff in this.FFs)
					{
						if (ff.IsPK==false)
							continue;
						if(ff.DataType==DataType.AppString)
							DeleteSQL+=ff.ToField+"='"+ToDR[ff.ToField].ToString()+"' AND ";
						else
							DeleteSQL+=ff.ToField+"="+ToDR[ff.ToField].ToString()+" AND ";
					}
					DeleteSQL=DeleteSQL.Substring(0,DeleteSQL.Length-5);
					switch(this.ToDBUrl)
					{
						case DA.DBUrlType.AppCenterDSN:
							DBAccess.RunSQL(DeleteSQL);
							break;
						case DA.DBUrlType.DBAccessOfMSSQL1:
                            DBAccessOfMSSQL1.RunSQL(DeleteSQL);						
							break;
                        case DA.DBUrlType.DBAccessOfMSSQL2:
                            DBAccessOfMSSQL2.RunSQL(DeleteSQL);
                            break;
						case DA.DBUrlType.DBAccessOfOLE:
							DBAccessOfOLE.RunSQL(DeleteSQL);
							break;
						case DA.DBUrlType.DBAccessOfOracle1:
                            DBAccessOfOracle1.RunSQL(DeleteSQL);
							break;
                        case DA.DBUrlType.DBAccessOfOracle2:
                            DBAccessOfOracle2.RunSQL(DeleteSQL);
                            break;
						case DA.DBUrlType.DBAccessOfODBC:
							DBAccessOfODBC.RunSQL(DeleteSQL);
							break;
						default:
							break;
					}
					continue;
				}
				else if(result>1)
				{
					throw new Exception("目的数据异常错误＋表名；关键字"+this.ToTable+"关键字"+ToDR[0].ToString());
				} 
			}
			#endregion			

			if(this.UPDATEsql!=null)
			{
				switch(this.ToDBUrl)
				{
					case DA.DBUrlType.AppCenterDSN:
						DBAccess.RunSQL(UPDATEsql);
						break;
                    //case DA.DBUrlType.DBAccessOfMSSQL:
                    //    DBAccess.RunSQL(UPDATEsql);						
                    //    break;
					case DA.DBUrlType.DBAccessOfOLE:
						DBAccessOfOLE.RunSQL(UPDATEsql);
						break;
                    //case DA.DBUrlType.DBAccessOfOracle:
                    //    DBAccessOfOracle.RunSQL(UPDATEsql);
                    //    break;
					case DA.DBUrlType.DBAccessOfODBC:
						DBAccessOfODBC.RunSQL(UPDATEsql);
						break;
					default:
						break;
				}
			}
			this.DoAfter();
		}		 
		#endregion
	}	
}
