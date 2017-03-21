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
		/// <param name="frmID">表单ID</param>
		/// <param name="pkval">主键</param>
		/// <param name="atParas">参数</param>
        /// <param name="specDtlFrmID">指定明细表的参数，如果为空就标识主表数据，否则就是从表数据.</param>
		/// <returns>数据</returns>
		public static DataSet GenerDBForVSTOExcelFrmModel(string frmID, int pkval, string atParas, string specDtlFrmID=null)
		{
			//数据容器,就是要返回的对象.
			DataSet myds = new DataSet();

			//映射实体.
			MapData md = new MapData(frmID);

			//实体.
			GEEntity wk = new GEEntity(frmID);
			wk.OID = pkval;
			if (wk.RetrieveFromDBSources() == 0)
				wk.Insert();

			//把参数放入到 En 的 Row 里面。
			if (string.IsNullOrEmpty(atParas) == false)
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


			#region 表单模版信息.（含主、从表，以及从表的枚举/外键相关数据）

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


			//主表的配置信息.
			sql = "SELECT * FROM Sys_MapExt WHERE FK_MapData='" + frmID + "'";
			dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
			dt.TableName = "Sys_MapExt";
			myds.Tables.Add(dt);

			#region 加载从表表单模版信息.
			foreach (MapDtl item in md.MapDtls)
            {
                #region 返回指定的明细表的数据.
                if (string.IsNullOrEmpty(specDtlFrmID) == true)
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
				dt.TableName = "Sys_MapDtl_For_" + item.No;
				myds.Tables.Add(dt);

				//明细表的表单描述
				sql = "SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + item.No + "'";
				dtMapAttr = BP.DA.DBAccess.RunSQLReturnTable(sql);
				dtMapAttr.TableName = "Sys_MapAttr_For_" + item.No;
				myds.Tables.Add(dtMapAttr);

				//明细表的配置信息.
				sql = "SELECT * FROM Sys_MapExt WHERE FK_MapData='" + item.No + "'";
				dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
				dt.TableName = "Sys_MapExt_For_" + item.No;
				myds.Tables.Add(dt);

				#region 把从表的- 外键表/枚举 加入 DataSet.
				mes = new MapExts(item.No);
				foreach (DataRow dr in dtMapAttr.Rows)
				{
					string lgType = dr["LGType"].ToString();
					//不是枚举/外键字段
					if (lgType == "0")
						continue;

					string uiBindKey = dr["UIBindKey"].ToString();
					var mypk = dr["MyPK"].ToString();

					#region 枚举字段
					if (lgType == "1")
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

					#region 外键字段
					string UIIsEnable = dr["UIIsEnable"].ToString();
					if (UIIsEnable == "0") //字段未启用
						continue;

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
				#endregion 把从表的- 外键表/枚举 加入 DataSet.

			}
			#endregion 加载从表表单模版信息.

			#endregion 表单模版信息.

			#region 把主表数据放入.
			if (BP.Sys.SystemConfig.IsBSsystem == true)
			{
				// 处理传递过来的参数。
				foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
				{
					wk.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
				}
			}

			// 执行表单事件..
			string msg = md.FrmEvents.DoEventNode(FrmEventList.FrmLoadBefore, wk);
			if (string.IsNullOrEmpty(msg) == false)
				throw new Exception("err@错误:" + msg);

			//重设默认值.
			wk.ResetDefaultVal();

			//执行装载填充.
			me = new MapExt();
			me.MyPK = frmID + "_" + MapExtXmlList.PageLoadFull;
			if (me.RetrieveFromDBSources() == 1)
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

			#endregion 把主表数据放入.

			#region  把从表的数据放入.
			foreach (MapDtl dtl in md.MapDtls)
			{
                #region 返回指定的明细表的数据.
                if (string.IsNullOrEmpty(specDtlFrmID) == true)
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

				dtDtl.TableName = dtl.No; //修改明细表的名称.
				myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.
			}
			#endregion 把从表的数据放入.

			#region 把主表的- 外键表/枚举 加入 DataSet.
			dtMapAttr = myds.Tables["Sys_MapAttr"];
			mes = md.MapExts;
			foreach (DataRow dr in dtMapAttr.Rows)
			{
				string uiBindKey = dr["UIBindKey"].ToString();
				string myPK = dr["MyPK"].ToString();
				string lgType = dr["LGType"].ToString();
				if (lgType == "1")
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

				if (lgType != "2")
					continue;

				string UIIsEnable = dr["UIIsEnable"].ToString();
				if (UIIsEnable == "0")
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
			#endregion 把主表的- 外键表/枚举 加入 DataSet.

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
        public static DataSet GenerDBForCCFormDtl(string frmID, MapDtl dtl, int pkval, string atParas)
        {
            //数据容器,就是要返回的对象.
            DataSet myds = new DataSet();

            //映射实体.
            MapData md = new MapData(frmID);

            //实体.
            GEEntity wk = new GEEntity(frmID);
            wk.OID = pkval;
            if (wk.RetrieveFromDBSources() == 0)
                wk.Insert();

            //把参数放入到 En 的 Row 里面。
            if (string.IsNullOrEmpty(atParas) == false)
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

            #region (主表)表单模版信息.（含主枚举/外键相关数据）
            string sql = "";
            DataTable dt;

            ////增加表单字段描述.
            //string sql = "SELECT * FROM Sys_MapData WHERE No='" + frmID + "' ";
            //DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            //dt.TableName = "Sys_MapData";
            //myds.Tables.Add(dt);

            ////增加表单字段描述.
            //sql = "SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + frmID + "' ";
            //dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            //dt.TableName = "Sys_MapAttr";
            //myds.Tables.Add(dt);

            ////主表的配置信息.
            //sql = "SELECT * FROM Sys_MapExt WHERE FK_MapData='" + frmID + "'";
            //dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            //dt.TableName = "Sys_MapExt";
            //myds.Tables.Add(dt);
            #endregion  (主表)表单模版信息.（含主枚举/外键相关数据）.

            #region 加载从表表单模版信息.
            //明细表的主表描述
            sql = "SELECT * FROM Sys_MapDtl WHERE No='" + dtl.No + "'";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapDtl";
            myds.Tables.Add(dt);

            //明细表的表单描述
            sql = "SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + dtl.No + "'";
            dtMapAttr = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtMapAttr.TableName = "Sys_MapAttr";
            myds.Tables.Add(dtMapAttr);

            //明细表的配置信息.
            sql = "SELECT * FROM Sys_MapExt WHERE FK_MapData='" + dtl.No + "'";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapExt";
            myds.Tables.Add(dt);

            #region 把从表的- 外键表/枚举 加入 DataSet.
            mes = new MapExts(dtl.No);
            foreach (DataRow dr in dtMapAttr.Rows)
            {
                string lgType = dr["LGType"].ToString();
                //不是枚举/外键字段
                if (lgType == "0")
                    continue;

                string uiBindKey = dr["UIBindKey"].ToString();
                var mypk = dr["MyPK"].ToString();

                #region 枚举字段
                if (lgType == "1")
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

                #region 外键字段
                string UIIsEnable = dr["UIIsEnable"].ToString();
                if (UIIsEnable == "0") //字段未启用
                    continue;

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
            #endregion 把从表的- 外键表/枚举 加入 DataSet.


            #endregion 加载从表表单模版信息.

            #region 把主表数据放入.
            if (BP.Sys.SystemConfig.IsBSsystem == true)
            {
                // 处理传递过来的参数。
                foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                {
                    wk.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
                }
            }

            //重设默认值.
            wk.ResetDefaultVal();


            //增加主表数据.
            DataTable mainTable = wk.ToDataTableField(md.No);
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

            dtDtl.TableName = "DBDtl"; //修改明细表的名称.
            myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.

            #endregion 把从表的数据放入.

            #region 把主表的- 外键表/枚举 加入 DataSet.
            dtMapAttr = myds.Tables["Sys_MapAttr"];
            mes = md.MapExts;
            foreach (DataRow dr in dtMapAttr.Rows)
            {
                string uiBindKey = dr["UIBindKey"].ToString();
                string myPK = dr["MyPK"].ToString();
                string lgType = dr["LGType"].ToString();
                if (lgType == "1")
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

                if (lgType != "2")
                    continue;

                string UIIsEnable = dr["UIIsEnable"].ToString();
                if (UIIsEnable == "0")
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
            #endregion 把主表的- 外键表/枚举 加入 DataSet.
           
            return myds;
        }
	}
}
