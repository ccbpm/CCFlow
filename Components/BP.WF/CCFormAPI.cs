using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.WF;
using BP.DA;
using BP.Port;
using BP.Web;
using BP.WF.Template;
using BP.En;
using BP.Sys;

namespace BP.WF
{
	/// <summary>
	/// 表单引擎api
	/// </summary>
	public class CCFormAPI : Dev2Interface
	{
		/// <summary>
		/// 生成报表
		/// </summary>
		/// <param name="templeteFilePath">模版路径</param>
		/// <param name="ds">数据源</param>
		/// <returns>生成单据的路径</returns>
		public static void Frm_GenerBill(string templeteFullFile, string saveToDir, string saveFileName,
			BillFileType fileType, DataSet ds, string fk_mapData)
		{

			MapData md = new MapData(fk_mapData);
			GEEntity entity = md.GenerGEEntityByDataSet(ds);

			BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine();
			rtf.HisEns.Clear();
			rtf.EnsDataDtls.Clear();

			rtf.HisEns.AddEntity(entity);
			var dtls = entity.Dtls;

			foreach (var item in dtls)
				rtf.EnsDataDtls.Add(item);

			rtf.MakeDoc(templeteFullFile, saveToDir, saveFileName, null, false);
		}

		/// <summary>
		/// 仅获取表单数据
		/// </summary>
		/// <param name="enName"></param>
		/// <param name="pkval"></param>
		/// <param name="atParas"></param>
		/// <param name="specDtlFrmID"></param>
		/// <returns></returns>
		private static DataSet GenerDBForVSTOExcelFrmModelOfEntity(string enName, object pkval, string atParas, string specDtlFrmID = null)
		{
			DataSet myds = new DataSet();

			// 创建实体..
			Entities myens = BP.En.ClassFactory.GetEns(enName + "s");

			#region 主表

			Entity en = myens.GetNewEntity;
			en.PKVal = pkval;
			en.RetrieveFromDBSources();

			//设置外部传入的默认值.
			if (BP.Sys.SystemConfig.IsBSsystem == true)
			{
				// 处理传递过来的参数。
				foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
				{
					en.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
				}
			}

			//主表数据放入集合.
			DataTable mainTable = en.ToDataTableField();
			mainTable.TableName = "MainTable";
			myds.Tables.Add(mainTable);

			#region 主表 Sys_MapData
			string sql = "SELECT * FROM Sys_MapData WHERE 1=2 ";
			DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
			dt.TableName = "Sys_MapData";

			Map map = en.EnMapInTime;
			DataRow dr = dt.NewRow();
			dr[MapDataAttr.No] = enName;
			dr[MapDataAttr.Name] = map.EnDesc;
			dr[MapDataAttr.PTable] = map.PhysicsTable;
			dt.Rows.Add(dr);
			myds.Tables.Add(dt);
			#endregion 主表 Sys_MapData

			#region 主表 Sys_MapAttr
			sql = "SELECT * FROM Sys_MapAttr WHERE 1=2 ";
			dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
			dt.TableName = "Sys_MapAttr";
			foreach (Attr attr in map.Attrs)
			{
				dr = dt.NewRow();
				dr[MapAttrAttr.MyPK] = enName + "_" + attr.Key;
				dr[MapAttrAttr.Name] = attr.Desc;

				dr[MapAttrAttr.MyDataType] = attr.MyDataType;   //数据类型.
				dr[MapAttrAttr.MinLen] = attr.MinLength;   //最小长度.
				dr[MapAttrAttr.MaxLen] = attr.MaxLength;   //最大长度.

				// 设置他的逻辑类型.
				dr[MapAttrAttr.LGType] = 0; //逻辑类型.
				switch (attr.MyFieldType)
				{
					case FieldType.Enum:
						dr[MapAttrAttr.LGType] = 1;
						dr[MapAttrAttr.UIBindKey] = attr.UIBindKey;

						//增加枚举字段.
						if (myds.Tables.Contains(attr.UIBindKey) == false)
						{
							string mysql = "SELECT IntKey AS No, Lab as Name FROM Sys_Enum WHERE EnumKey='" + attr.UIBindKey + "' ORDER BY IntKey ";
							DataTable dtEnum = DBAccess.RunSQLReturnTable(mysql);
							dtEnum.TableName = attr.UIBindKey;
							myds.Tables.Add(dtEnum);
						}

						break;
					case FieldType.FK:
						dr[MapAttrAttr.LGType] = 2;

						Entities ens = attr.HisFKEns;
						dr[MapAttrAttr.UIBindKey] = ens.ToString();

						//把外键字段也增加进去.
						if (myds.Tables.Contains(ens.ToString()) == false && attr.UIIsReadonly == false)
						{
							ens.RetrieveAll();
							DataTable mydt = ens.ToDataTableDescField();
							mydt.TableName = ens.ToString();
							myds.Tables.Add(mydt);
						}
						break;
					default:
						break;
				}

				// 设置控件类型.
				dr[MapAttrAttr.UIContralType] = (int)attr.UIContralType;
				dt.Rows.Add(dr);
			}
			myds.Tables.Add(dt);
			#endregion 主表 Sys_MapAttr

			#region //主表 Sys_MapExt 扩展属性
			////主表的配置信息.
			//sql = "SELECT * FROM Sys_MapExt WHERE 1=2";
			//dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
			//dt.TableName = "Sys_MapExt";
			//myds.Tables.Add(dt);
			#endregion //主表 Sys_MapExt 扩展属性

			#endregion

			#region 从表
			foreach (EnDtl item in map.Dtls)
			{
				#region  把从表的数据放入集合.

				Entities dtls = item.Ens;

				QueryObject qo = qo = new QueryObject(dtls);

                if (dtls.ToString().Contains("CYSheBeiUse") == true)
                    qo.addOrderBy("RDT"); //按照日期进行排序，不用也可以.

				qo.AddWhere(item.RefKey, pkval);
				DataTable dtDtl = qo.DoQueryToTable();

				dtDtl.TableName = item.EnsName; //修改明细表的名称.
				myds.Tables.Add(dtDtl); //加入这个明细表.

				#endregion 把从表的数据放入.

				#region 从表 Sys_MapDtl (相当于mapdata)

				Entity dtl = dtls.GetNewEntity;
				map = dtl.EnMap;

				//明细表的 描述 . 
				sql = "SELECT * FROM Sys_MapDtl WHERE 1=2";
				dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
				dt.TableName = "Sys_MapDtl_For_" + item.EnsName;

				dr = dt.NewRow();
				dr[MapDtlAttr.No] = item.EnsName;
				dr[MapDtlAttr.Name] = item.Desc;
				dr[MapDtlAttr.PTable] = dtl.EnMap.PhysicsTable;
				dt.Rows.Add(dr);
				myds.Tables.Add(dt);

				#endregion 从表 Sys_MapDtl (相当于mapdata)

				#region 明细表 Sys_MapAttr
				sql = "SELECT * FROM Sys_MapAttr WHERE 1=2";
				dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
				dt.TableName = "Sys_MapAttr_For_" + item.EnsName;
				foreach (Attr attr in map.Attrs)
				{
					dr = dt.NewRow();
					dr[MapAttrAttr.MyPK] = enName + "_" + attr.Key;
					dr[MapAttrAttr.Name] = attr.Desc;

					dr[MapAttrAttr.MyDataType] = attr.MyDataType;   //数据类型.
					dr[MapAttrAttr.MinLen] = attr.MinLength;   //最小长度.
					dr[MapAttrAttr.MaxLen] = attr.MaxLength;   //最大长度.

					// 设置他的逻辑类型.
					dr[MapAttrAttr.LGType] = 0; //逻辑类型.
					switch (attr.MyFieldType)
					{
						case FieldType.Enum:
							dr[MapAttrAttr.LGType] = 1;
							dr[MapAttrAttr.UIBindKey] = attr.UIBindKey;

							//增加枚举字段.
							if (myds.Tables.Contains(attr.UIBindKey) == false)
							{
								string mysql = "SELECT IntKey AS No, Lab as Name FROM Sys_Enum WHERE EnumKey='" + attr.UIBindKey + "' ORDER BY IntKey ";
								DataTable dtEnum = DBAccess.RunSQLReturnTable(mysql);
								dtEnum.TableName = attr.UIBindKey;
								myds.Tables.Add(dtEnum);
							}
							break;
						case FieldType.FK:
							dr[MapAttrAttr.LGType] = 2;

							Entities ens = attr.HisFKEns;
							dr[MapAttrAttr.UIBindKey] = ens.ToString();

							//把外键字段也增加进去.
							if (myds.Tables.Contains(ens.ToString()) == false && attr.UIIsReadonly == false)
							{
								ens.RetrieveAll();
								DataTable mydt = ens.ToDataTableDescField();
								mydt.TableName = ens.ToString();
								myds.Tables.Add(mydt);
							}
							break;
						default:
							break;
					}

					// 设置控件类型.
					dr[MapAttrAttr.UIContralType] = (int)attr.UIContralType;
					dt.Rows.Add(dr);
				}
				myds.Tables.Add(dt);
				#endregion 明细表 Sys_MapAttr

			}
			#endregion

			return myds;
		}
		/// <summary>
		/// 仅获取表单数据
		/// </summary>
		/// <param name="frmID">表单ID</param>
		/// <param name="pkval">主键</param>
		/// <param name="atParas">参数</param>
		/// <param name="specDtlFrmID">指定明细表的参数，如果为空就标识主表数据，否则就是从表数据.</param>
		/// <returns>数据</returns>
		public static DataSet GenerDBForVSTOExcelFrmModel(string frmID, object pkval, string atParas, string specDtlFrmID = null)
		{
			//如果是一个实体类.
			if (frmID.Contains("BP."))
			{
				// 执行map同步.
				Entities ens = BP.En.ClassFactory.GetEns(frmID + "s");
				Entity en = ens.GetNewEntity;
				en.DTSMapToSys_MapData();

				return GenerDBForVSTOExcelFrmModelOfEntity(frmID, pkval, atParas, specDtlFrmID = null);

				//上面这行代码的解释（2017-04-25）：
				//若不加上这行，代码执行到“ MapData md = new MapData(frmID); ”会报错：
				//@没有找到记录[表单注册表  Sys_MapData, [ 主键=No 值=BP.LI.BZQX ]记录不存在,请与管理员联系, 或者确认输入错误.@在Entity(BP.Sys.MapData)查询期间出现错误@   在 BP.En.Entity.Retrieve() 位置 D:\ccflow\Components\BP.En30\En\Entity.cs:行号 1051
				//即使加上：
				//frmID = frmID.Substring(0, frmID.Length - 1);
				//也会出现该问题
				//2017-04-25 15:26:34：new MapData(frmID)应传入“BZQX”，但考虑到 GenerDBForVSTOExcelFrmModelOfEntity()运行稳定，暂不采用『统一执行下方代码』的方案。
			}

			//数据容器,就是要返回的对象.
			DataSet myds = new DataSet();

			//映射实体.
			MapData md = new MapData(frmID);

			//实体.
			GEEntity wk = new GEEntity(frmID);
			wk.OID = int.Parse(pkval.ToString());
			if (wk.RetrieveFromDBSources() == 0)
				wk.Insert();

            //加载事件.
            md.DoEvent(FrmEventList.FrmLoadBefore, wk, null);

			//把参数放入到 En 的 Row 里面。
			if (DataType.IsNullOrEmpty(atParas) == false)
			{
				AtPara ap = new AtPara(atParas);
				foreach (string key in ap.HisHT.Keys)
				{
					if (wk.Row.ContainsKey(key) == true) //有就该变.
						wk.Row[key] = ap.GetValStrByKey(key);
					else
						wk.Row.Add(key, ap.GetValStrByKey(key)); //增加他.
				}
			}

			//属性.
			MapExt me = null;
			DataTable dtMapAttr = null;
			MapExts mes = null;
            
          


			#region 表单模版信息.（含主、从表的，以及从表的枚举/外键相关数据）.
			//增加表单字段描述.
			string sql = "SELECT * FROM Sys_MapData WHERE No='" + frmID + "' ";
			DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
			dt.TableName = "Sys_MapData";
			myds.Tables.Add(dt);

			//增加表单字段描述.
			sql = "SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + frmID + "' ";
			dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
			dt.TableName = "Sys_MapAttr";
			myds.Tables.Add(dt);

            //增加从表信息.
            sql = "SELECT * FROM Sys_MapDtl WHERE FK_MapData='" + frmID + "' ";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapDtl";
            myds.Tables.Add(dt);


			//主表的配置信息.
			sql = "SELECT * FROM Sys_MapExt WHERE FK_MapData='" + frmID + "'";
			dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
			dt.TableName = "Sys_MapExt";
			myds.Tables.Add(dt);

			#region 加载 从表表单模版信息.（含 从表的枚举/外键相关数据）
			foreach (MapDtl item in md.MapDtls)
			{
				#region 返回指定的明细表的数据.
				if (DataType.IsNullOrEmpty(specDtlFrmID) == true)
				{
				}
				else
				{
					if (item.No != specDtlFrmID)
						continue;
				}
				#endregion 返回指定的明细表的数据.

				//明细表的主表描述
				sql = "SELECT * FROM Sys_MapDtl WHERE No='" + item.No + "'";
				dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
				dt.TableName = "Sys_MapDtl_For_" + (string.IsNullOrWhiteSpace(item.Alias) ? item.No : item.Alias);
				myds.Tables.Add(dt);

				//明细表的表单描述
				sql = "SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + item.No + "'";
				dtMapAttr = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dtMapAttr.TableName = "Sys_MapAttr_For_" + (string.IsNullOrWhiteSpace(item.Alias) ? item.No : item.Alias);
				myds.Tables.Add(dtMapAttr);

				//明细表的配置信息.
				sql = "SELECT * FROM Sys_MapExt WHERE FK_MapData='" + item.No + "'";
				dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Sys_MapExt_For_" + (string.IsNullOrWhiteSpace(item.Alias) ? item.No : item.Alias);
				myds.Tables.Add(dt);

				#region 从表的 外键表/枚举
				mes = new MapExts(item.No);
				foreach (DataRow dr in dtMapAttr.Rows)
				{
					string lgType = dr["LGType"].ToString();
					//不是枚举/外键字段
					if (lgType.Equals("0"))
						continue;

					string uiBindKey = dr["UIBindKey"].ToString();
					var mypk = dr["MyPK"].ToString();

					#region 枚举字段
					if (lgType.Equals("1") )
					{
						// 如果是枚举值, 判断是否存在.
						if (myds.Tables.Contains(uiBindKey) == true)
							continue;

						string mysql = "SELECT IntKey AS No, Lab as Name FROM Sys_Enum WHERE EnumKey='" + uiBindKey + "' ORDER BY IntKey ";
						DataTable dtEnum = DBAccess.RunSQLReturnTable(mysql);
						dtEnum.TableName = uiBindKey;
						myds.Tables.Add(dtEnum);
						continue;
					}
					#endregion

					string UIIsEnable = dr["UIIsEnable"].ToString();
					if (UIIsEnable.Equals("0")) //字段未启用
						continue;

					#region 外键字段
					// 检查是否有下拉框自动填充。
					string keyOfEn = dr["KeyOfEn"].ToString();

					#region 处理下拉框数据范围. for 小杨.
					me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
					if (me != null) //有范围限制时
					{
						string fullSQL = me.Doc.Clone() as string;
						fullSQL = fullSQL.Replace("~", ",");
						fullSQL = BP.WF.Glo.DealExp(fullSQL, wk, null);

						dt = DBAccess.RunSQLReturnTable(fullSQL);

						dt.TableName = mypk;
						myds.Tables.Add(dt);
						continue;
					}
					#endregion 处理下拉框数据范围.
					else //无范围限制时
					{
						// 判断是否存在.
						if (myds.Tables.Contains(uiBindKey) == true)
							continue;

						myds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
					}
					#endregion 外键字段
				}
				#endregion 从表的 外键表/枚举

			}
			#endregion 加载 从表表单模版信息.（含 从表的枚举/外键相关数据）

			#endregion 表单模版信息.（含主、从表的，以及从表的枚举/外键相关数据）.

			#region 主表数据
			if (BP.Sys.SystemConfig.IsBSsystem == true)
			{
				// 处理传递过来的参数。
				foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
				{
					wk.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
				}
			}

			// 执行表单事件..
			string msg = md.DoEvent(FrmEventList.FrmLoadBefore, wk);
			if (DataType.IsNullOrEmpty(msg) == false)
				throw new Exception("err@错误:" + msg);

			//重设默认值.
			wk.ResetDefaultVal();

			//执行装载填充.
			me = new MapExt();

            if (me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull, MapExtAttr.FK_MapData, frmID) == 1)
            {
                //执行通用的装载方法.
                MapAttrs attrs = new MapAttrs(frmID);
                MapDtls dtls = new MapDtls(frmID);
                wk = BP.WF.Glo.DealPageLoadFull(wk, me, attrs, dtls) as GEEntity;
            }

			//增加主表数据.
			DataTable mainTable = wk.ToDataTableField(md.No);
			mainTable.TableName = "MainTable";
			myds.Tables.Add(mainTable);

			#endregion 主表数据

			#region  从表数据
			foreach (MapDtl dtl in md.MapDtls)
			{
				#region 返回指定的明细表的数据.
				if (DataType.IsNullOrEmpty(specDtlFrmID) == true)
				{
				}
				else
				{
					if (dtl.No != specDtlFrmID)
						continue;
				}
				#endregion 返回指定的明细表的数据.

				GEDtls dtls = new GEDtls(dtl.No);
				QueryObject qo = null;
				try
				{
					qo = new QueryObject(dtls);
					switch (dtl.DtlOpenType)
					{
						case DtlOpenType.ForEmp:  // 按人员来控制.
							qo.AddWhere(GEDtlAttr.RefPK, pkval);
							qo.addAnd();
							qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
							break;
						case DtlOpenType.ForWorkID: // 按工作ID来控制
							qo.AddWhere(GEDtlAttr.RefPK, pkval);
							break;
						case DtlOpenType.ForFID: // 按流程ID来控制.
							qo.AddWhere(GEDtlAttr.FID, pkval);
							break;
					}
				}
				catch
				{
					dtls.GetNewEntity.CheckPhysicsTable();
				}

				//条件过滤.
				if (dtl.FilterSQLExp != "")
				{
					string[] strs = dtl.FilterSQLExp.Split('=');
					qo.addAnd();
					qo.AddWhere(strs[0], strs[1]);
				}

				//从表
				DataTable dtDtl = qo.DoQueryToTable();

				// 为明细表设置默认值.
				MapAttrs dtlAttrs = new MapAttrs(dtl.No);
				foreach (MapAttr attr in dtlAttrs)
				{
					//处理它的默认值.
					if (attr.DefValReal.Contains("@") == false)
						continue;

					foreach (DataRow dr in dtDtl.Rows)
						dr[attr.KeyOfEn] = attr.DefVal;
				}

				dtDtl.TableName = string.IsNullOrWhiteSpace(dtl.Alias) ? dtl.No : dtl.Alias; //edited by liuxc,2017-10-10.如果有别名，则使用别名，没有则使用No
				myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.
			}
			#endregion 从表数据

			#region 主表的 外键表/枚举
			dtMapAttr = myds.Tables["Sys_MapAttr"];
			mes = md.MapExts;
			foreach (DataRow dr in dtMapAttr.Rows)
			{
				string uiBindKey = dr["UIBindKey"].ToString();
				string myPK = dr["MyPK"].ToString();
				string lgType = dr["LGType"].ToString();
				if (lgType.Equals("1") )
				{
					// 如果是枚举值, 判断是否存在., 
					if (myds.Tables.Contains(uiBindKey) == true)
						continue;

					string mysql = "SELECT IntKey AS No, Lab as Name FROM Sys_Enum WHERE EnumKey='" + uiBindKey + "' ORDER BY IntKey ";
					DataTable dtEnum = DBAccess.RunSQLReturnTable(mysql);
					dtEnum.TableName = uiBindKey;
					myds.Tables.Add(dtEnum);
					continue;
				}

				if (lgType.Equals("2") ==false)
					continue;

				string UIIsEnable = dr["UIIsEnable"].ToString();
				if (UIIsEnable.Equals("0"))
					continue;

				// 检查是否有下拉框自动填充。
				string keyOfEn = dr["KeyOfEn"].ToString();
				string fk_mapData = dr["FK_MapData"].ToString();

				#region 处理下拉框数据范围. for 小杨.
				me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
				if (me != null)
				{
					string fullSQL = me.Doc.Clone() as string;
					fullSQL = fullSQL.Replace("~", ",");
					fullSQL = BP.WF.Glo.DealExp(fullSQL, wk, null);
					dt = DBAccess.RunSQLReturnTable(fullSQL);
					dt.TableName = myPK; //可能存在隐患，如果多个字段，绑定同一个表，就存在这样的问题.
					myds.Tables.Add(dt);
					continue;
				}
				#endregion 处理下拉框数据范围.

				dt = BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey);
				dt.TableName = uiBindKey;
				myds.Tables.Add(dt);
			}
			#endregion 主表的 外键表/枚举

			//返回生成的dataset.
			return myds;
		}
		/// <summary>
		/// 获取从表数据，用于显示dtl.htm 
		/// </summary>
		/// <param name="frmID">表单ID</param>
		/// <param name="pkval">主键</param>
		/// <param name="atParas">参数</param>
		/// <param name="specDtlFrmID">指定明细表的参数，如果为空就标识主表数据，否则就是从表数据.</param>
		/// <returns>数据</returns>
		public static DataSet GenerDBForCCFormDtl(string frmID, MapDtl dtl, int pkval, string atParas,Int64 fid=0)
		{
			//数据容器,就是要返回的对象.
			DataSet myds = new DataSet();

			//映射实体.
			//MapData md = new MapData(frmID);

			//实体.
			GEEntity en = new GEEntity(frmID);
            en.OID = pkval;
            if (en.RetrieveFromDBSources() == 0)
                en.Insert();

			//把参数放入到 En 的 Row 里面。
			if (DataType.IsNullOrEmpty(atParas) == false)
			{
				AtPara ap = new AtPara(atParas);
				foreach (string key in ap.HisHT.Keys)
				{
                    try
                    {
                        if (en.Row.ContainsKey(key) == true) //有就该变.
                            en.Row[key] = ap.GetValStrByKey(key);
                        else
                            en.Row.Add(key, ap.GetValStrByKey(key)); //增加他.
                    }
                    catch(Exception ex)
                    {
                        throw new Exception(key);
                    }
				}
			}

			#region 加载从表表单模版信息.

            DataTable Sys_MapDtl = dtl.ToDataTableField("Sys_MapDtl");
            myds.Tables.Add(Sys_MapDtl);

			//明细表的表单描述
            DataTable Sys_MapAttr = dtl.MapAttrs.ToDataTableField("Sys_MapAttr");
            myds.Tables.Add(Sys_MapAttr);

			//明细表的配置信息.
            DataTable Sys_MapExt = dtl.MapExts.ToDataTableField("Sys_MapExt");
            myds.Tables.Add(Sys_MapExt);

			#region 把从表的- 外键表/枚举 加入 DataSet.
            MapExts mes = dtl.MapExts;
            MapExt me = null;

            foreach (DataRow dr in Sys_MapAttr.Rows)
            {
                string lgType = dr["LGType"].ToString();
                string ctrlType = dr[MapAttrAttr.UIContralType].ToString();

                ////不是枚举/外键字段
                //if (lgType.Equals("0") && ctrlType.Equals("0") )
                //    continue;

                string uiBindKey = dr["UIBindKey"].ToString();
                if (DataType.IsNullOrEmpty(uiBindKey) == true)
                    continue;

                var mypk = dr["MyPK"].ToString();

                #region 枚举字段
                if (lgType.Equals("1")==true)
                {
                    // 如果是枚举值, 判断是否存在.
                    if (myds.Tables.Contains(uiBindKey) == true)
                        continue;

                    string mysql = "SELECT IntKey AS No, Lab as Name FROM Sys_Enum WHERE EnumKey='" + uiBindKey + "' ORDER BY IntKey ";
                    DataTable dtEnum = DBAccess.RunSQLReturnTable(mysql);
                    dtEnum.TableName = uiBindKey;

                    dtEnum.Columns[0].ColumnName = "No";
                    dtEnum.Columns[1].ColumnName = "Name";

                    myds.Tables.Add(dtEnum);
                    continue;
                }
                #endregion

                #region 外键字段
                string UIIsEnable = dr["UIIsEnable"].ToString();
                if (UIIsEnable.Equals("0")) //字段未启用
                    continue;

                // 检查是否有下拉框自动填充。
                string keyOfEn = dr["KeyOfEn"].ToString();

                #region 处理下拉框数据范围. for 小杨.
                me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                if (me != null) //有范围限制时
                {
                    string fullSQL = me.Doc.Clone() as string;
                    fullSQL = fullSQL.Replace("~", ",");
                    fullSQL = BP.WF.Glo.DealExp(fullSQL, en, null);

                    DataTable dt = DBAccess.RunSQLReturnTable(fullSQL);

                    dt.TableName = uiBindKey;

                    dt.Columns[0].ColumnName = "No";
                    dt.Columns[1].ColumnName = "Name";

                    myds.Tables.Add(dt);
                    continue;
                }
                #endregion 处理下拉框数据范围.

                // 判断是否存在.
                if (myds.Tables.Contains(uiBindKey) == true)
                    continue;

                // 获得数据.
                DataTable mydt = BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey);
                myds.Tables.Add(mydt);

                #endregion 外键字段
            }
			#endregion 把从表的- 外键表/枚举 加入 DataSet.


			#endregion 加载从表表单模版信息.

			#region 把主表数据放入.
			if (BP.Sys.SystemConfig.IsBSsystem == true)
			{
				// 处理传递过来的参数。
				foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
				{
                    en.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
				}
			}

			//重设默认值.
            en.ResetDefaultVal();


			//增加主表数据.
            DataTable mainTable = en.ToDataTableField(frmID);
			mainTable.TableName = "MainTable";
			myds.Tables.Add(mainTable);
			#endregion 把主表数据放入.

			#region  把从表的数据放入.
			GEDtls dtls = new GEDtls(dtl.No);
			QueryObject qo = null;
            try
            {
                qo = new QueryObject(dtls);
                switch (dtl.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:  // 按人员来控制.
                        qo.AddWhere(GEDtlAttr.RefPK, pkval);
                        qo.addAnd();
                        qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                        break;
                    case DtlOpenType.ForWorkID: // 按工作ID来控制
                        qo.AddWhere(GEDtlAttr.RefPK, pkval);
                        qo.addOr();
                        qo.AddWhere(GEDtlAttr.FID, pkval);
                        break;
                    case DtlOpenType.ForFID: // 按流程ID来控制.
                        if (fid == 0)
                            qo.AddWhere(GEDtlAttr.FID, pkval);
                        else
                            qo.AddWhere(GEDtlAttr.FID, fid);
                        break;
                }
            }
            catch (Exception ex)
            {
                dtls.GetNewEntity.CheckPhysicsTable();
                throw ex;
            }

			//条件过滤.
			if (dtl.FilterSQLExp != "")
			{
				string[] strs = dtl.FilterSQLExp.Split('=');
                if (strs.Length == 2)
                {
                    qo.addAnd();
                    qo.AddWhere(strs[0], strs[1]);
                }
			}

            //增加排序.
        //    qo.addOrderByDesc( dtls.GetNewEntity.PKField );

			//从表
			DataTable dtDtl = qo.DoQueryToTable();

            //查询所有动态SQL查询类型的字典表记录
            SFTable sftable = null;
            DataTable dtsftable = null;
            DataRow[] drs = null;

            SFTables sftables = new SFTables();
            sftables.Retrieve(SFTableAttr.SrcType, (int)SrcType.SQL);

			// 为明细表设置默认值.
			MapAttrs dtlAttrs = new MapAttrs(dtl.No);
			foreach (MapAttr attr in dtlAttrs)
            {
                #region 修改区分大小写.
                if (BP.DA.DBType.Oracle == SystemConfig.AppCenterDBType)
                {
                    foreach (DataColumn dr in dtDtl.Columns)
                    {
                        var a = attr.KeyOfEn;
                        var b = dr.ColumnName;
                        if (attr.KeyOfEn.ToUpper().Equals(dr.ColumnName))
                        {
                            dr.ColumnName = attr.KeyOfEn;
                            continue;
                        }

                        if (attr.LGType == FieldTypeS.Enum || attr.LGType == FieldTypeS.FK)
                        {
                            if (dr.ColumnName.Equals(  attr.KeyOfEn.ToUpper() + "TEXT"))
                            {
                                dr.ColumnName = attr.KeyOfEn + "Text";
                            }
                        }
                    }
                    foreach (DataRow dr in dtDtl.Rows)
                    {
                        //本身是大写的不进行修改
                        if (DataType.IsNullOrEmpty(dr[attr.KeyOfEn] + ""))
                        {
                            dr[attr.KeyOfEn] = dr[attr.KeyOfEn.ToUpper()];
                            dr[attr.KeyOfEn.ToUpper()] = null;
                        }
                    }
                }
                #endregion 修改区分大小写.

                if (attr.UIContralType == UIContralType.TB)
                    continue;

                //处理增加动态SQL查询类型的下拉框选中值Text值，added by liuxc,2017-9-22
                if(attr.UIContralType== UIContralType.DDL 
                    &&  attr.LGType == FieldTypeS.Normal 
                    && attr.UIIsEnable == true)
                {
                    sftable = sftables.GetEntityByKey(attr.UIBindKey) as SFTable;
                    if (sftable != null)
                    {
                        dtsftable = sftable.GenerHisDataTable;

                        //为Text赋值
                        foreach (DataRow dr in dtDtl.Rows)
                        {
                            drs = dtsftable.Select("No='" + dr[attr.KeyOfEn] + "'");
                            if (drs.Length == 0)
                                continue;

                            dr[attr.KeyOfEn + "Text"] = drs[0]["Name"];
                        }
                    }
                }

                //处理它的默认值.
				if (attr.DefValReal.Contains("@") == false)
					continue;

				foreach (DataRow dr in dtDtl.Rows)
					dr[attr.KeyOfEn] = attr.DefVal;
			}

			dtDtl.TableName = "DBDtl"; //修改明细表的名称.
			myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.
			#endregion 把从表的数据放入.


            //放入一个空白的实体，用与获取默认值.
            GEDtl dtlBlank = dtls.GetNewEntity as GEDtl;
            dtlBlank.ResetDefaultVal();

            myds.Tables.Add(dtlBlank.ToDataTableField("Blank"));

           // myds.WriteXml("c:\\xx.xml");

			return myds;
		}
	}
}
