using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Port;
using BP.Web;
using BP.WF.Template;
using BP.WF.XML;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 表单功能界面
    /// </summary>
    public class WF_CCForm : DirectoryPageBase
    {
        #region HanderMapExt
        /// <summary>
        /// 扩展处理.
        /// </summary>
        /// <returns></returns>
        public string HandlerMapExt()
        {
            string fk_mapExt = this.GetRequestVal("FK_MapExt").ToString();
            if (string.IsNullOrEmpty(this.GetRequestVal("Key")))
                return "";

            string oid = this.GetRequestVal("OID");

            string kvs = this.GetRequestVal("KVs");

            BP.Sys.MapExt me = new BP.Sys.MapExt(fk_mapExt);
            DataTable dt = null;
            string sql = "";
            string key = this.GetRequestVal("Key");
            key = System.Web.HttpUtility.UrlDecode(key,
                System.Text.Encoding.GetEncoding("GB2312"));
            key = key.Trim();

            // key = "周";
            switch (me.ExtType)
            {

                case BP.Sys.MapExtXmlList.ActiveDDL: // 动态填充ddl。
                    sql = this.DealSQL(me.DocOfSQLDeal, key);
                    if (sql.Contains("@") == true)
                    {
                        foreach (string strKey in context.Request.QueryString)
                        {
                            sql = sql.Replace("@" + strKey, context.Request[strKey]);
                        }
                    }

                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    return JSONTODT(dt);
                case BP.Sys.MapExtXmlList.AutoFullDLL://填充下拉框
                case BP.Sys.MapExtXmlList.TBFullCtrl: // 自动完成。
                case BP.Sys.MapExtXmlList.DDLFullCtrl: // 级连ddl.
                    switch (this.GetRequestVal("DoTypeExt"))
                    {
                        case "ReqCtrl":
                            // 获取填充 ctrl 值的信息.
                            sql = this.DealSQL(me.DocOfSQLDeal, key);
                            System.Web.HttpContext.Current.Session["DtlKey"] = key;
                            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                            return JSONTODT(dt);
                            break;
                        case "ReqM2MFullList":
                            /* 获取填充的M2m集合. */
                            DataTable dtM2M = new DataTable("Head");
                            dtM2M.Columns.Add("Dtl", typeof(string));
                            string[] strsM2M = me.Tag2.Split('$');
                            foreach (string str in strsM2M)
                            {
                                if (str == "" || str == null)
                                    continue;

                                string[] ss = str.Split(':');
                                string noOfObj = ss[0];
                                string mysql = ss[1];
                                mysql = DealSQL(mysql, key);

                                DataTable dtFull = DBAccess.RunSQLReturnTable(mysql);
                                M2M m2mData = new M2M();
                                m2mData.FK_MapData = me.FK_MapData;
                                m2mData.EnOID = int.Parse(oid);
                                m2mData.M2MNo = noOfObj;
                                string mystr = ",";
                                string mystrT = "";
                                foreach (DataRow dr in dtFull.Rows)
                                {
                                    string myno = dr["No"].ToString();
                                    string myname = dr["Name"].ToString();
                                    mystr += myno + ",";
                                    mystrT += "@" + myno + "," + myname;
                                }
                                m2mData.Vals = mystr;
                                m2mData.ValsName = mystrT;
                                m2mData.InitMyPK();
                                m2mData.NumSelected = dtFull.Rows.Count;
                                m2mData.Save();

                                DataRow mydr = dtM2M.NewRow();
                                mydr[0] = ss[0];
                                dtM2M.Rows.Add(mydr);
                            }
                            return JSONTODT(dtM2M);

                            break;
                        case "ReqDtlFullList":
                            /* 获取填充的从表集合. */
                            DataTable dtDtl = new DataTable("Head");
                            dtDtl.Columns.Add("Dtl", typeof(string));
                            string[] strsDtl = me.Tag1.Split('$');
                            foreach (string str in strsDtl)
                            {
                                if (string.IsNullOrEmpty(str))
                                    continue;

                                string[] ss = str.Split(':');
                                string fk_dtl = ss[0];
                                string dtlKey = System.Web.HttpContext.Current.Session["DtlKey"] as string;
                                if (dtlKey == null)
                                    dtlKey = key;
                                string mysql = DealSQL(ss[1], dtlKey);

                                GEDtls dtls = new GEDtls(fk_dtl);
                                MapDtl dtl = new MapDtl(fk_dtl);

                                DataTable dtDtlFull = DBAccess.RunSQLReturnTable(mysql);
                                BP.DA.DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE RefPK=" + oid);
                                foreach (DataRow dr in dtDtlFull.Rows)
                                {
                                    BP.Sys.GEDtl mydtl = new GEDtl(fk_dtl);
                                    //  mydtl.OID = dtls.Count + 1;
                                    dtls.AddEntity(mydtl);
                                    foreach (DataColumn dc in dtDtlFull.Columns)
                                    {
                                        mydtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                                    }
                                    mydtl.RefPKInt = int.Parse(oid);
                                    if (mydtl.OID > 100)
                                    {
                                        mydtl.InsertAsOID(mydtl.OID);
                                    }
                                    else
                                    {
                                        mydtl.OID = 0;
                                        mydtl.Insert();
                                    }

                                }
                                DataRow drRe = dtDtl.NewRow();
                                drRe[0] = fk_dtl;
                                dtDtl.Rows.Add(drRe);
                            }
                            return JSONTODT(dtDtl);
                            break;
                        case "ReqDDLFullList":
                            /* 获取要个性化填充的下拉框. */
                            DataTable dt1 = new DataTable("Head");
                            dt1.Columns.Add("DDL", typeof(string));
                            //    dt1.Columns.Add("SQL", typeof(string));
                            string[] strs = me.Tag.Split('$');
                            foreach (string str in strs)
                            {
                                if (str == "" || str == null)
                                    continue;

                                string[] ss = str.Split(':');
                                DataRow dr = dt1.NewRow();
                                dr[0] = ss[0];
                                // dr[1] = ss[1];
                                dt1.Rows.Add(dr);
                            }
                            return JSONTODT(dt);
                            break;
                        case "ReqDDLFullListDB":
                            /* 获取要个性化填充的下拉框的值. 根据已经传递过来的 ddl id. */
                            string myDDL = this.GetRequestVal("MyDDL");
                            sql = me.DocOfSQLDeal;
                            string[] strs1 = me.Tag.Split('$');
                            foreach (string str in strs1)
                            {
                                if (str == "" || str == null)
                                    continue;

                                string[] ss = str.Split(':');
                                if (ss[0] == myDDL && ss.Length == 2)
                                {
                                    sql = ss[1];
                                    sql = this.DealSQL(sql, key);
                                    break;
                                }
                            }
                            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                            return JSONTODT(dt);
                            break;
                        default:
                            sql = this.DealSQL(me.DocOfSQLDeal, key);
                            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                            return JSONTODT(dt);
                            break;
                    }
                    return "";
                default:
                    return "err@没有解析的标记" + me.ExtType;
            }

            return "err@没有解析的标记" + me.ExtType;
        }
        private string DealSQL(string sql, string key)
        {
            sql = sql.Replace("@Key", key);
            sql = sql.Replace("@key", key);
            sql = sql.Replace("@Val", key);
            sql = sql.Replace("@val", key);

            sql = sql.Replace("@WebUser.No", WebUser.No);
            sql = sql.Replace("@WebUser.Name", WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

            string oid = this.GetRequestVal("OID");
            if (oid != null)
                sql = sql.Replace("@OID", oid);

            string kvs = this.GetRequestVal("KVs");

            if (string.IsNullOrEmpty(kvs) == false && sql.Contains("@") == true)
            {
                string[] strs = kvs.Split('~');
                foreach (string s in strs)
                {
                    if (string.IsNullOrEmpty(s)
                        || s.Contains("=") == false)
                        continue;

                    string[] mykv = s.Split('=');
                    sql = sql.Replace("@" + mykv[0], mykv[1]);

                    if (sql.Contains("@") == false)
                        break;
                }
            }

            if (sql.Contains("@") == true)
            {
                foreach (string mykey in context.Request.QueryString.AllKeys)
                {
                    sql = sql.Replace("@" + mykey, context.Request.QueryString[mykey]);
                }
            }

            dealSQL = sql;
            return sql;
        }
        private string dealSQL = "";
        public string JSONTODT(DataTable dt)
        {
            if ((BP.Sys.SystemConfig.AppCenterDBType == DBType.Informix
                     || BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle) && dealSQL != null)
            {
                /*如果数据库不区分大小写, 就要按用户输入的sql进行二次处理。*/
                string mysql = dealSQL.Trim();
                mysql = mysql.Substring(6, mysql.ToLower().IndexOf("from") - 6);
                mysql = mysql.Replace(",", " ");
                string[] strs = mysql.Split(' ');
                foreach (string s in strs)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName.ToLower() == s.ToLower())
                        {
                            dc.ColumnName = s;
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName.ToLower() == "no")
                    {
                        dc.ColumnName = "No";
                        continue;
                    }
                    if (dc.ColumnName.ToLower() == "name")
                    {
                        dc.ColumnName = "Name";
                        continue;
                    }
                }
            }

            StringBuilder JsonString = new StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                JsonString.Append("{ ");
                JsonString.Append("\"Head\":[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":\"" + dt.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":\"" + dt.Rows[i][j].ToString() + "\"");
                        }
                    }
                    /**/
                    /*end Of String*/
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }
                JsonString.Append("]}");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion HanderMapExt



        #region 执行父类的重写方法.
        /// <summary>
        /// 表单功能界面
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_CCForm(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到.@原始URL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.

        #region frm.htm 主表.
        /// <summary>
        /// 执行数据初始化
        /// </summary>
        /// <returns></returns>
        public string Frm_Init()
        {
            if (this.GetRequestVal("IsTest") != null)
            {
                MapData mymd = new MapData(this.EnsName);
                mymd.RepairMap();
                BP.Sys.SystemConfig.DoClearCash();
            }

            MapData md = new MapData(this.EnsName);

            #region 判断是否是返回的URL.
            if (md.HisFrmType == FrmType.Url)
            {
                string no = this.GetRequestVal("NO");
                string urlParas = "OID=" + this.RefOID + "&NO=" + no + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&UserNo=" + WebUser.No + "&SID=" + this.SID;

                string url = "";
                /*如果是URL.*/
                if (md.Url.Contains("?") == true)
                    url = md.Url + "&" + urlParas;
                else
                    url = md.Url + "?" + urlParas;

                return "url@" + url;
            }

            if (md.HisFrmType == FrmType.VSTOForExcel && this.GetRequestVal("IsFreeFrm") == null)
            {
                string url = "FrmVSTO.aspx?1=1&" + this.RequestParas;
                return "url@" + url;
            }

            if (md.HisFrmType == FrmType.WordFrm)
            {
                string no = this.GetRequestVal("NO");
                string urlParas = "OID=" + this.RefOID + "&NO=" + no + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&UserNo=" + WebUser.No + "&SID=" + this.SID + "&FK_MapData=" + this.FK_MapData + "&OIDPKVal=" + this.OID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                /*如果是URL.*/
                string requestParas = this.RequestParas;
                string[] parasArrary = this.RequestParas.Split('&');
                foreach (string str in parasArrary)
                {
                    if (string.IsNullOrEmpty(str) || str.Contains("=") == false)
                        continue;
                    string[] kvs = str.Split('=');
                    if (urlParas.Contains(kvs[0]))
                        continue;
                    urlParas += "&" + kvs[0] + "=" + kvs[1];
                }
                if (md.Url.Contains("?") == true)
                    return "url@FrmWord.aspx?1=2" + "&" + urlParas;
                else
                    return "url@FrmWord.aspx" + "?" + urlParas;
            }

            if (md.HisFrmType == FrmType.ExcelFrm)
                return "url@FrmExcel.aspx?1=2" + this.RequestParas;

            #endregion 判断是否是返回的URL.

            //返回自由表单解析执行器.
            if (BP.WF.Glo.IsBeta == true)
                return "url@FrmFree.htm?1=2" + this.RequestParas;
            else
                return "url@Frm.aspx?1=2" + this.RequestParas;
        }
        /// <summary>
        /// 执行数据初始化
        /// </summary>
        /// <returns></returns>
        public string FrmFree_Init()
        {
            try
            {
                MapData md = new MapData(this.EnsName);
                DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet_2017(md.No);

                #region 把主表数据放入.
                string atParas = "";
                //主表实体.
                GEEntity en = new GEEntity(this.EnsName);

                #region 求出 who is pk 值.
                Int64 pk = this.RefOID;
                if (pk == 0)
                    pk = this.OID;
                if (pk == 0)
                    pk = this.WorkID;

                if (this.FK_Node != 0 && string.IsNullOrEmpty(this.FK_Flow) == false)
                {
                    /*说明是流程调用它， 就要判断谁是表单的PK.*/
                    FrmNode fn = new FrmNode(this.FK_Flow, this.FK_Node, this.FK_MapData);
                    switch (fn.WhoIsPK)
                    {
                        case WhoIsPK.FID:
                            pk = this.FID;
                            if (pk == 0)
                                throw new Exception("@没有接收到参数FID");
                            break;
                        case WhoIsPK.PWorkID: /*父流程ID*/
                            pk = this.PWorkID;
                            if (pk == 0)
                                throw new Exception("@没有接收到参数PWorkID");
                            break;
                        case WhoIsPK.OID:
                        default:
                            break;
                    }
                }
                #endregion  求who is PK.

                en.OID = pk;
                if (en.RetrieveFromDBSources() == 0)
                    en.Insert();

                //把参数放入到 En 的 Row 里面。
                if (string.IsNullOrEmpty(atParas) == false)
                {
                    AtPara ap = new AtPara(atParas);
                    foreach (string key in ap.HisHT.Keys)
                    {
                        if (en.Row.ContainsKey(key) == true) //有就该变.
                            en.Row[key] = ap.GetValStrByKey(key);
                        else
                            en.Row.Add(key, ap.GetValStrByKey(key)); //增加他.
                    }
                }

                if (BP.Sys.SystemConfig.IsBSsystem == true)
                {
                    // 处理传递过来的参数。
                    foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                    {
                        en.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
                    }
                }

                // 执行表单事件..
                string msg = md.DoEvent(FrmEventList.FrmLoadBefore, en);
                if (string.IsNullOrEmpty(msg) == false)
                    throw new Exception("err@错误:" + msg);

                //重设默认值.
                en.ResetDefaultVal();

                //执行装载填充.
                MapExt me = new MapExt();
                if (me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull, MapExtAttr.FK_MapData, this.EnsName) == 1)
                {
                    //执行通用的装载方法.
                    MapAttrs attrs = new MapAttrs(this.EnsName);
                    MapDtls dtls = new MapDtls(this.EnsName);
                    en = BP.WF.Glo.DealPageLoadFull(en, me, attrs, dtls) as GEEntity;
                }

                //增加主表数据.
                DataTable mainTable = en.ToDataTableField(md.No);
                mainTable.TableName = "MainTable";

                ds.Tables.Add(mainTable);
                #endregion 把主表数据放入.

                return BP.Tools.Json.DataSetToJson(ds, false);
            }
            catch (Exception ex)
            {
                GEEntity myen = new GEEntity(this.EnsName);
                myen.CheckPhysicsTable();

                BP.Sys.CCFormAPI.RepareCCForm(this.EnsName);
                return "err@装载表单期间出现如下错误,ccform有自动诊断修复功能请在刷新一次，如果仍然存在请联系管理员. @" + ex.Message;
            }
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string FrmFree_Save()
        {
            try
            {
                //保存主表数据.
                GEEntity en = new GEEntity(this.EnsName);
                en.OID = this.RefOID;
                int i = en.RetrieveFromDBSources();

                en.ResetDefaultVal();

                en = BP.Sys.PubClass.CopyFromRequest(en, context.Request) as GEEntity;

                en.OID = this.RefOID;

                // 处理表单保存前事件.
                MapData md = new MapData(this.EnsName);

                md.DoEvent(FrmEventList.SaveBefore, en);

                if (i == 0)
                    en.Insert();
                else
                    en.Update();

                //处理保存后事件.
                md.DoEvent(FrmEventList.SaveAfter, en);
                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion frm.htm 主表.

        #region dtl.htm 从表.
        /// <summary>
        /// 初始化从表数据
        /// </summary>
        /// <returns>返回结果数据</returns>
        public string Dtl_Init()
        {
            #region 检查是否是测试.
            if (this.GetRequestVal("IsTest") != null)
            {
                BP.Sys.GEDtl dtl = new GEDtl(this.EnsName);
                dtl.CheckPhysicsTable();

                //MapDtl mdtl = new MapDtl(this.EnsName);
                //BP.Sys.CCFormAPI.RepareCCForm();
            }
            #endregion

            #region 组织参数.
            MapDtl mdtl = new MapDtl(this.EnsName);
            mdtl.No = this.EnsName;
            mdtl.RetrieveFromDBSources();

            if (this.FK_Node != 0 && mdtl.FK_MapData != "Temp" && this.EnsName.Contains("ND" + this.FK_Node) == false)
            {
                Node nd = new BP.WF.Node(this.FK_Node);
                /*如果
                 * 1,传来节点ID, 不等于0.
                 * 2,不是节点表单.  就要判断是否是独立表单，如果是就要处理权限方案。*/

                BP.WF.Template.FrmNode fn = new BP.WF.Template.FrmNode(nd.FK_Flow, nd.NodeID, mdtl.FK_MapData);
                int i = fn.Retrieve(FrmNodeAttr.FK_Frm, mdtl.FK_MapData, FrmNodeAttr.FK_Node, this.FK_Node);
                if (i != 0 && fn.FrmSln != 0)
                {
                    /*使用了自定义的方案.
                     * 并且，一定为dtl设定了自定义方案，就用自定义方案.
                     */
                    mdtl.No = this.EnsName + "_" + this.FK_Node;
                    mdtl.RetrieveFromDBSources();
                }
            }

            if (this.GetRequestVal("IsReadonly") == "1")
            {
                mdtl.IsInsert = false;
                mdtl.IsDelete = false;
                mdtl.IsUpdate = false;
            }
            else
            {
            }

            string strs = this.RequestParas;
            strs = strs.Replace("?", "@");
            strs = strs.Replace("&", "@");
            #endregion 组织参数.

            //获得他的描述,与数据.
            DataSet ds = BP.WF.CCFormAPI.GenerDBForCCFormDtl(mdtl.FK_MapData, mdtl, int.Parse(this.RefPKVal), strs);

            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 执行从表的保存.
        /// </summary>
        /// <returns></returns>
        public string Dtl_Save()
        {
            MapDtl mdtl = new MapDtl(this.EnsName);
            GEDtls dtls = new GEDtls(this.EnsName);
            FrmEvents fes = new FrmEvents(this.EnsName); //获得事件.
            GEEntity mainEn = null;

            #region 从表保存前处理事件.
            if (fes.Count > 0)
            {
                mainEn = mdtl.GenerGEMainEntity(this.RefPKVal);
                string msg = fes.DoEventNode(EventListDtlList.DtlSaveBefore, mainEn);
                if (msg != null)
                    throw new Exception(msg);
            }
            #endregion 从表保存前处理事件.

            #region 保存的业务逻辑.

            #endregion 保存的业务逻辑.

            return "保存成功";
        }
        /// <summary>
        /// 保存单行数据
        /// </summary>
        /// <returns></returns>
        public string Dtl_SaveRow()
        {
            //从表.
            MapDtl mdtl = new MapDtl(this.FK_MapDtl);

            //从表实体.
            GEDtl dtl = new GEDtl(this.FK_MapDtl);
            int oid = this.RefOID;
            if (oid != 0)
            {
                dtl.OID = oid;
                dtl.RetrieveFromDBSources();
            }



            #region 给实体循环赋值/并保存.
            BP.En.Attrs attrs = dtl.EnMap.Attrs;
            foreach (BP.En.Attr attr in attrs)
            {
                dtl.SetValByKey(attr.Key, this.GetRequestVal(attr.Key));
            }

            //关联主赋值.
            dtl.RefPK = this.RefPKVal;



            #region 从表保存前处理事件.
            //获得主表事件.
            FrmEvents fes = new FrmEvents(this.FK_MapData); //获得事件.
            GEEntity mainEn = null;
            if (fes.Count > 0)
            {
                mainEn = mdtl.GenerGEMainEntity(this.RefPKVal);
                string msg = fes.DoEventNode(EventListDtlList.DtlSaveBefore, mainEn);
                if (msg != null)
                    throw new Exception(msg);
            }

            if (mdtl.FEBD.Length != 0)
            {
                string str = mdtl.FEBD;
                BP.Sys.FormEventBaseDtl febd = BP.Sys.Glo.GetFormDtlEventBaseByEnName(mdtl.No);

                febd.HisEn = mdtl.GenerGEMainEntity(this.RefPKVal);
                febd.HisEnDtl = dtl;

                febd.DoIt(FrmEventListDtl.RowSaveBefore, febd.HisEn, dtl, null);
            }
            #endregion 从表保存前处理事件.



            if (dtl.OID == 0)
            {
                //dtl.OID = DBAccess.GenerOID();
                dtl.Insert();
            }
            else
            {
                dtl.Update();
            }
            #endregion 给实体循环赋值/并保存.

            #region 从表保存后处理事件。
            if (fes.Count > 0)
            {
                string msg = fes.DoEventNode(EventListDtlList.DtlSaveEnd, mainEn);
                if (msg != null)
                    throw new Exception(msg);
            }

            if (mdtl.FEBD.Length != 0)
            {
                string str = mdtl.FEBD;
                BP.Sys.FormEventBaseDtl febd = BP.Sys.Glo.GetFormDtlEventBaseByEnName(mdtl.No);

                febd.HisEn = mdtl.GenerGEMainEntity(this.RefPKVal);
                febd.HisEnDtl = dtl;

                febd.DoIt(FrmEventListDtl.RowSaveAfter, febd.HisEn, dtl, null);
            }
            #endregion 处理事件.

            //返回当前数据存储信息.
            return dtl.ToJson();

        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string Dtl_DeleteRow()
        {
            GEDtl dtl = new GEDtl(this.FK_MapDtl);
            dtl.OID = this.RefOID;
            dtl.Delete();
            return "删除成功";
        }
        /// <summary>
        /// 重新获取单个ddl数据
        /// </summary>
        /// <returns></returns>
        public string Dtl_ReloadDdl()
        {
            string Doc = this.GetRequestVal("Doc");
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(Doc);
            dt.TableName = "ReloadDdl";
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion dtl.htm 从表.
        /// <summary>
        /// 处理SQL的表达式.
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <returns>从from里面替换的表达式.</returns>
        public string DealExpByFromVals(string exp)
        {
            foreach (string strKey in context.Request.Form.AllKeys)
            {
                if (exp.Contains("@") == false)
                    return exp;
                string str = strKey.Replace("TB_", "").Replace("CB_", "").Replace("DDL_", "").Replace("RB_", "");

                exp = exp.Replace("@" + str, context.Request.Form[strKey]);
            }
            return exp;
        }
        /// <summary>
        /// 初始化树的接口
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string InitPopValTree()
        {
            string mypk = this.GetRequestVal("FK_MapExt");

            MapExt me = new MapExt();
            me.MyPK = mypk;
            me.Retrieve();

            //获得配置信息.
            Hashtable ht = me.PopValToHashtable();
            DataTable dtcfg = BP.Sys.PubClass.HashtableToDataTable(ht);

            string parentNo = this.GetRequestVal("ParentNo");
            if (parentNo == null)
                parentNo = me.PopValTreeParentNo;

            DataSet resultDs = new DataSet();
            string sqlObjs = me.PopValTreeSQL;
            sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
            sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
            sqlObjs = sqlObjs.Replace("@ParentNo", parentNo);
            sqlObjs = this.DealExpByFromVals(sqlObjs);

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlObjs);
            dt.TableName = "DTObjs";
            resultDs.Tables.Add(dt);

            //doubleTree
            if (me.PopValWorkModel == PopValWorkModel.TreeDouble && parentNo != me.PopValTreeParentNo)
            {
                sqlObjs = me.PopValDoubleTreeEntitySQL;
                sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
                sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                sqlObjs = sqlObjs.Replace("@ParentNo", parentNo);
                sqlObjs = this.DealExpByFromVals(sqlObjs);

                DataTable entityDt = BP.DA.DBAccess.RunSQLReturnTable(sqlObjs);
                entityDt.TableName = "DTEntitys";
                resultDs.Tables.Add(entityDt);
            }

            return BP.Tools.Json.ToJson(resultDs);
        }

        /// <summary>
        /// 初始化PopVal的值   除了分页表格模式之外的其他数据值
        /// </summary>
        /// <returns></returns>
        public string InitPopVal()
        {
            MapExt me = new MapExt();
            me.MyPK = this.FK_MapExt;
            me.Retrieve();

            //数据对象，将要返回的.
            DataSet ds = new DataSet();

            //获得配置信息.
            Hashtable ht = me.PopValToHashtable();
            DataTable dtcfg = BP.Sys.PubClass.HashtableToDataTable(ht);

            //增加到数据源.
            ds.Tables.Add(dtcfg);

            if (me.PopValWorkModel == PopValWorkModel.SelfUrl)
                return "@SelfUrl" + me.PopValUrl;

            if (me.PopValWorkModel == PopValWorkModel.TableOnly)
            {
                string sqlObjs = me.PopValEntitySQL;
                sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
                sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

                sqlObjs = this.DealExpByFromVals(sqlObjs);


                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlObjs);
                dt.TableName = "DTObjs";
                ds.Tables.Add(dt);
                return BP.Tools.Json.ToJson(ds);
            }

            if (me.PopValWorkModel == PopValWorkModel.Group)
            {
                /*
                 *  分组的.
                 */

                string sqlObjs = me.PopValGroupSQL;
                if (sqlObjs.Length > 10)
                {
                    sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
                    sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                    sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                    sqlObjs = this.DealExpByFromVals(sqlObjs);


                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlObjs);
                    dt.TableName = "DTGroup";
                    ds.Tables.Add(dt);
                }

                sqlObjs = me.PopValEntitySQL;
                if (sqlObjs.Length > 10)
                {
                    sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
                    sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                    sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                    sqlObjs = this.DealExpByFromVals(sqlObjs);


                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlObjs);
                    dt.TableName = "DTEntity";
                    ds.Tables.Add(dt);
                }
                return BP.Tools.Json.ToJson(ds);

            }

            if (me.PopValWorkModel == PopValWorkModel.TablePage)
            {
                /* 分页的 */
                //key
                string key = this.GetRequestVal("Key");
                if (string.IsNullOrEmpty(key) == true)
                    key = "";

                //取出来查询条件.
                string[] conds = me.PopValSearchCond.Split('$');

                //pageSize
                string pageSize = this.GetRequestVal("pageSize");
                if (string.IsNullOrEmpty(pageSize))
                    pageSize = "10";

                //pageIndex
                string pageIndex = this.GetRequestVal("pageIndex");
                if (string.IsNullOrEmpty(pageIndex))
                    pageIndex = "1";

                string sqlObjs = me.PopValTablePageSQL;
                sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
                sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                sqlObjs = sqlObjs.Replace("@Key", key);

                //三个固定参数.
                sqlObjs = sqlObjs.Replace("@PageCount", ((int.Parse(pageIndex) - 1) * int.Parse(pageSize)).ToString());
                sqlObjs = sqlObjs.Replace("@PageSize", pageSize);
                sqlObjs = sqlObjs.Replace("@PageIndex", pageIndex);
                sqlObjs = this.DealExpByFromVals(sqlObjs);

                //替换其他参数.
                foreach (string cond in conds)
                {
                    if (cond == null || cond == "")
                        continue;

                    //参数.
                    string para = cond.Substring(5, cond.IndexOf("#") - 5);
                    string val = context.Request.QueryString[para];
                    if (string.IsNullOrEmpty(val))
                    {
                        if (cond.Contains("ListSQL") == true || cond.Contains("EnumKey") == true)
                            val = "all";
                        else
                            val = "";
                    }
                    if (val == "all")
                    {
                        sqlObjs = sqlObjs.Replace(para + "=@" + para, "1=1");
                        sqlObjs = sqlObjs.Replace(para + "='@" + para + "'", "1=1");

                        int startIndex = 0;
                        while (startIndex != -1 && startIndex < sqlObjs.Length)
                        {
                            int index = sqlObjs.IndexOf("1=1", startIndex + 1);
                            if (index > 0 && sqlObjs.Substring(startIndex, index - startIndex).Trim().EndsWith("."))
                            {
                                int lastBlankIndex = sqlObjs.Substring(startIndex, index - startIndex).LastIndexOf(" ");


                                sqlObjs = sqlObjs.Remove(lastBlankIndex + startIndex + 1, index - lastBlankIndex - 1);

                                startIndex = (startIndex + lastBlankIndex) + 3;
                            }
                            else
                            {
                                startIndex = index;
                            }
                        }
                    }
                    else
                    {
                        //要执行两次替换有可能是，有引号.
                        sqlObjs = sqlObjs.Replace("@" + para, val);
                    }
                }


                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlObjs);
                dt.TableName = "DTObjs";
                ds.Tables.Add(dt);

                //处理查询条件.
                //$Para=Dept#Label=所在班级#ListSQL=Select No,Name FROM Port_Dept WHERE No='@WebUser.No'
                //$Para=XB#Label=性别#EnumKey=XB
                //$Para=DTFrom#Label=注册日期从#DefVal=@Now-30
                //$Para=DTTo#Label=到#DefVal=@Now


                foreach (string cond in conds)
                {
                    if (string.IsNullOrEmpty(cond) == true)
                        continue;

                    string sql = null;
                    if (cond.Contains("#ListSQL=") == true)
                    {
                        sql = cond.Substring(cond.IndexOf("ListSQL") + 8);
                        sql = sql.Replace("@WebUser.No", WebUser.No);
                        sql = sql.Replace("@WebUser.Name", WebUser.Name);
                        sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                        sql = this.DealExpByFromVals(sql);
                    }

                    if (cond.Contains("#EnumKey=") == true)
                    {
                        string enumKey = cond.Substring(cond.IndexOf("EnumKey") + 8);
                        sql = "SELECT IntKey AS No, Lab as Name FROM Sys_Enum WHERE EnumKey='" + enumKey + "'";
                    }

                    //处理日期的默认值
                    //DefVal=@Now-30
                    //if (cond.Contains("@Now"))
                    //{
                    //    int nowIndex = cond.IndexOf(cond);
                    //    if (cond.Trim().Length - nowIndex > 5)
                    //    {
                    //        char optStr = cond.Trim()[nowIndex + 5];
                    //        int day = 0;
                    //        if (int.TryParse(cond.Trim().Substring(nowIndex + 6), out day)) {
                    //            cond = cond.Substring(0, nowIndex) + DateTime.Now.AddDays(-1 * day).ToString("yyyy-MM-dd HH:mm");
                    //        }
                    //    }
                    //}

                    if (sql == null)
                        continue;

                    //参数.
                    string para = cond.Substring(5, cond.IndexOf("#") - 5);
                    if (ds.Tables.Contains(para) == true)
                        throw new Exception("@配置的查询,参数名有冲突不能命名为:" + para);

                    //查询出来数据，就把他放入到dataset里面.
                    DataTable dtPara = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    dtPara.TableName = para;
                    ds.Tables.Add(dtPara); //加入到参数集合.
                }


                return BP.Tools.Json.ToJson(ds);
            }

            //返回数据.
            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 初始化PopVal 分页表格模式的Count  杨玉慧
        /// </summary>
        /// <returns></returns>
        public string InitPopValTablePageCount()
        {
            MapExt me = new MapExt();
            me.MyPK = this.FK_MapExt;
            me.Retrieve();

            //数据对象，将要返回的.
            DataSet ds = new DataSet();

            //获得配置信息.
            Hashtable ht = me.PopValToHashtable();
            DataTable dtcfg = BP.Sys.PubClass.HashtableToDataTable(ht);

            //增加到数据源.
            ds.Tables.Add(dtcfg);

            if (me.PopValWorkModel == PopValWorkModel.SelfUrl)
                return "@SelfUrl" + me.PopValUrl;
            if (me.PopValWorkModel == PopValWorkModel.TablePage)
            {
                /* 分页的 */
                //key
                string key = this.GetRequestVal("Key");
                if (string.IsNullOrEmpty(key) == true)
                    key = "";

                //取出来查询条件.
                string[] conds = me.PopValSearchCond.Split('$');

                string countSQL = me.PopValTablePageSQLCount;

                //固定参数.
                countSQL = countSQL.Replace("@WebUser.No", BP.Web.WebUser.No);
                countSQL = countSQL.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                countSQL = countSQL.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                countSQL = countSQL.Replace("@Key", key);
                countSQL = this.DealExpByFromVals(countSQL);

                //替换其他参数.
                foreach (string cond in conds)
                {
                    if (cond == null || cond == "")
                        continue;

                    //参数.
                    string para = cond.Substring(5, cond.IndexOf("#") - 5);
                    string val = context.Request.QueryString[para];
                    if (string.IsNullOrEmpty(val))
                    {
                        if (cond.Contains("ListSQL") == true || cond.Contains("EnumKey") == true)
                            val = "all";
                        else
                            val = "";
                    }

                    if (val == "all")
                    {
                        countSQL = countSQL.Replace(para + "=@" + para, "1=1");
                        countSQL = countSQL.Replace(para + "='@" + para + "'", "1=1");

                        //找到para 前面表的别名   如 t.1=1 把t. 去掉
                        int startIndex = 0;
                        while (startIndex != -1 && startIndex < countSQL.Length)
                        {
                            int index = countSQL.IndexOf("1=1", startIndex + 1);
                            if (index > 0 && countSQL.Substring(startIndex, index - startIndex).Trim().EndsWith("."))
                            {
                                int lastBlankIndex = countSQL.Substring(startIndex, index - startIndex).LastIndexOf(" ");


                                countSQL = countSQL.Remove(lastBlankIndex + startIndex + 1, index - lastBlankIndex - 1);

                                startIndex = (startIndex + lastBlankIndex) + 3;
                            }
                            else
                            {
                                startIndex = index;
                            }
                        }
                    }
                    else
                    {
                        //要执行两次替换有可能是，有引号.
                        countSQL = countSQL.Replace("@" + para, val);
                    }
                }

                string count = BP.DA.DBAccess.RunSQLReturnValInt(countSQL, 0).ToString();
                DataTable dtCount = new DataTable("DTCout");
                dtCount.TableName = "DTCout";
                dtCount.Columns.Add("Count", typeof(int));
                dtCount.Rows.Add(new[] { count });
                ds.Tables.Add(dtCount);


                //处理查询条件.
                //$Para=Dept#Label=所在班级#ListSQL=Select No,Name FROM Port_Dept WHERE No='@WebUser.No'
                //$Para=XB#Label=性别#EnumKey=XB
                //$Para=DTFrom#Label=注册日期从#DefVal=@Now-30
                //$Para=DTTo#Label=到#DefVal=@Now


                foreach (string cond in conds)
                {
                    if (string.IsNullOrEmpty(cond) == true)
                        continue;

                    string sql = null;
                    if (cond.Contains("#ListSQL=") == true)
                    {
                        sql = cond.Substring(cond.IndexOf("ListSQL") + 8);
                        sql = sql.Replace("@WebUser.No", WebUser.No);
                        sql = sql.Replace("@WebUser.Name", WebUser.Name);
                        sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                        sql = this.DealExpByFromVals(sql);
                    }

                    if (cond.Contains("#EnumKey=") == true)
                    {
                        string enumKey = cond.Substring(cond.IndexOf("EnumKey") + 8);
                        sql = "SELECT IntKey AS No, Lab as Name FROM Sys_Enum WHERE EnumKey='" + enumKey + "'";
                    }

                    //处理日期的默认值
                    //DefVal=@Now-30
                    //if (cond.Contains("@Now"))
                    //{
                    //    int nowIndex = cond.IndexOf(cond);
                    //    if (cond.Trim().Length - nowIndex > 5)
                    //    {
                    //        char optStr = cond.Trim()[nowIndex + 5];
                    //        int day = 0;
                    //        if (int.TryParse(cond.Trim().Substring(nowIndex + 6), out day)) {
                    //            cond = cond.Substring(0, nowIndex) + DateTime.Now.AddDays(-1 * day).ToString("yyyy-MM-dd HH:mm");
                    //        }
                    //    }
                    //}

                    if (sql == null)
                        continue;

                    //参数.
                    string para = cond.Substring(5, cond.IndexOf("#") - 5);
                    if (ds.Tables.Contains(para) == true)
                        throw new Exception("@配置的查询,参数名有冲突不能命名为:" + para);

                    //查询出来数据，就把他放入到dataset里面.
                    DataTable dtPara = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    dtPara.TableName = para;
                    ds.Tables.Add(dtPara); //加入到参数集合.
                }


                return BP.Tools.Json.ToJson(ds);
            }
            //返回数据.
            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// /// <summary>
        /// 初始化PopVal分页表格的List  杨玉慧
        /// </summary>
        /// <returns></returns>
        public string InitPopValTablePageList()
        {
            MapExt me = new MapExt();
            me.MyPK = this.FK_MapExt;
            me.Retrieve();

            //数据对象，将要返回的.
            DataSet ds = new DataSet();

            //获得配置信息.
            Hashtable ht = me.PopValToHashtable();
            DataTable dtcfg = BP.Sys.PubClass.HashtableToDataTable(ht);

            //增加到数据源.
            ds.Tables.Add(dtcfg);

            if (me.PopValWorkModel == PopValWorkModel.SelfUrl)
                return "@SelfUrl" + me.PopValUrl;
            if (me.PopValWorkModel == PopValWorkModel.TablePage)
            {
                /* 分页的 */
                //key
                string key = this.GetRequestVal("Key");
                if (string.IsNullOrEmpty(key) == true)
                    key = "";

                //取出来查询条件.
                string[] conds = me.PopValSearchCond.Split('$');

                //pageSize
                string pageSize = this.GetRequestVal("pageSize");
                if (string.IsNullOrEmpty(pageSize))
                    pageSize = "10";

                //pageIndex
                string pageIndex = this.GetRequestVal("pageIndex");
                if (string.IsNullOrEmpty(pageIndex))
                    pageIndex = "1";

                string sqlObjs = me.PopValTablePageSQL;
                sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
                sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                sqlObjs = sqlObjs.Replace("@Key", key);

                //三个固定参数.
                sqlObjs = sqlObjs.Replace("@PageCount", ((int.Parse(pageIndex) - 1) * int.Parse(pageSize)).ToString());
                sqlObjs = sqlObjs.Replace("@PageSize", pageSize);
                sqlObjs = sqlObjs.Replace("@PageIndex", pageIndex);
                sqlObjs = this.DealExpByFromVals(sqlObjs);

                //替换其他参数.
                foreach (string cond in conds)
                {
                    if (cond == null || cond == "")
                        continue;

                    //参数.
                    string para = cond.Substring(5, cond.IndexOf("#") - 5);
                    string val = context.Request.QueryString[para];
                    if (string.IsNullOrEmpty(val))
                    {
                        if (cond.Contains("ListSQL") == true || cond.Contains("EnumKey") == true)
                            val = "all";
                        else
                            val = "";
                    }
                    if (val == "all")
                    {
                        sqlObjs = sqlObjs.Replace(para + "=@" + para, "1=1");
                        sqlObjs = sqlObjs.Replace(para + "='@" + para + "'", "1=1");

                        int startIndex = 0;
                        while (startIndex != -1 && startIndex < sqlObjs.Length)
                        {
                            int index = sqlObjs.IndexOf("1=1", startIndex + 1);
                            if (index > 0 && sqlObjs.Substring(startIndex, index - startIndex).Trim().EndsWith("."))
                            {
                                int lastBlankIndex = sqlObjs.Substring(startIndex, index - startIndex).LastIndexOf(" ");


                                sqlObjs = sqlObjs.Remove(lastBlankIndex + startIndex + 1, index - lastBlankIndex - 1);

                                startIndex = (startIndex + lastBlankIndex) + 3;
                            }
                            else
                            {
                                startIndex = index;
                            }
                        }
                    }
                    else
                    {
                        //要执行两次替换有可能是，有引号.
                        sqlObjs = sqlObjs.Replace("@" + para, val);
                    }
                }


                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlObjs);
                dt.TableName = "DTObjs";
                ds.Tables.Add(dt);

                //处理查询条件.
                //$Para=Dept#Label=所在班级#ListSQL=Select No,Name FROM Port_Dept WHERE No='@WebUser.No'
                //$Para=XB#Label=性别#EnumKey=XB
                //$Para=DTFrom#Label=注册日期从#DefVal=@Now-30
                //$Para=DTTo#Label=到#DefVal=@Now


                foreach (string cond in conds)
                {
                    if (string.IsNullOrEmpty(cond) == true)
                        continue;

                    string sql = null;
                    if (cond.Contains("#ListSQL=") == true)
                    {
                        sql = cond.Substring(cond.IndexOf("ListSQL") + 8);
                        sql = sql.Replace("@WebUser.No", WebUser.No);
                        sql = sql.Replace("@WebUser.Name", WebUser.Name);
                        sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                        sql = this.DealExpByFromVals(sql);
                    }

                    if (cond.Contains("#EnumKey=") == true)
                    {
                        string enumKey = cond.Substring(cond.IndexOf("EnumKey") + 8);
                        sql = "SELECT IntKey AS No, Lab as Name FROM Sys_Enum WHERE EnumKey='" + enumKey + "'";
                    }

                    //处理日期的默认值
                    //DefVal=@Now-30
                    //if (cond.Contains("@Now"))
                    //{
                    //    int nowIndex = cond.IndexOf(cond);
                    //    if (cond.Trim().Length - nowIndex > 5)
                    //    {
                    //        char optStr = cond.Trim()[nowIndex + 5];
                    //        int day = 0;
                    //        if (int.TryParse(cond.Trim().Substring(nowIndex + 6), out day)) {
                    //            cond = cond.Substring(0, nowIndex) + DateTime.Now.AddDays(-1 * day).ToString("yyyy-MM-dd HH:mm");
                    //        }
                    //    }
                    //}

                    if (sql == null)
                        continue;

                    //参数.
                    string para = cond.Substring(5, cond.IndexOf("#") - 5);
                    if (ds.Tables.Contains(para) == true)
                        throw new Exception("@配置的查询,参数名有冲突不能命名为:" + para);

                    //查询出来数据，就把他放入到dataset里面.
                    DataTable dtPara = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    dtPara.TableName = para;
                    ds.Tables.Add(dtPara); //加入到参数集合.
                }


                return BP.Tools.Json.ToJson(ds);
            }
            //返回数据.
            return BP.Tools.Json.ToJson(ds);
        }

        //单附件上传方法
        private void SingleAttach(HttpContext context, string attachPk, Int64 workid, Int64 fid, int fk_node, string ensName)
        {
            FrmAttachment frmAth = new FrmAttachment();
            frmAth.MyPK = attachPk;
            frmAth.RetrieveFromDBSources();

            string athDBPK = attachPk + "_" + workid;

            BP.WF.Node currND = new BP.WF.Node(fk_node);
            BP.WF.Work currWK = currND.HisWork;
            currWK.OID = workid;
            currWK.Retrieve();
            //处理保存路径.
            string saveTo = frmAth.SaveTo;

            if (saveTo.Contains("*") || saveTo.Contains("@"))
            {
                /*如果路径里有变量.*/
                saveTo = saveTo.Replace("*", "@");
                saveTo = BP.WF.Glo.DealExp(saveTo, currWK, null);
            }

            try
            {
                saveTo = context.Server.MapPath("~/" + saveTo);
            }
            catch
            {
                //saveTo = saveTo;
            }

            if (System.IO.Directory.Exists(saveTo) == false)
                System.IO.Directory.CreateDirectory(saveTo);


            saveTo = saveTo + "\\" + athDBPK + "." + context.Request.Files[0].FileName.Substring(context.Request.Files[0].FileName.LastIndexOf('.') + 1);
            context.Request.Files[0].SaveAs(saveTo);

            FileInfo info = new FileInfo(saveTo);

            FrmAttachmentDB dbUpload = new FrmAttachmentDB();
            dbUpload.MyPK = athDBPK;
            dbUpload.FK_FrmAttachment = attachPk;
            dbUpload.RefPKVal = this.WorkID.ToString();
            dbUpload.FID = fid;
            dbUpload.FK_MapData = ensName;

            dbUpload.FileExts = info.Extension;

            #region 处理文件路径，如果是保存到数据库，就存储pk.
            if (frmAth.SaveWay == 0)
            {
                //文件方式保存
                dbUpload.FileFullName = saveTo;
            }

            if (frmAth.SaveWay == 1)
            {
                //保存到数据库
                dbUpload.FileFullName = dbUpload.MyPK;
            }
            #endregion 处理文件路径，如果是保存到数据库，就存储pk.


            dbUpload.FileName = context.Request.Files[0].FileName;
            dbUpload.FileSize = (float)info.Length;
            dbUpload.Rec = WebUser.No;
            dbUpload.RecName = WebUser.Name;
            dbUpload.RDT = BP.DA.DataType.CurrentDataTime;

            dbUpload.NodeID = fk_node.ToString();
            dbUpload.Save();

            if (frmAth.SaveWay == 1)
            {
                //执行文件保存.
                BP.DA.DBAccess.SaveFileToDB(saveTo, dbUpload.EnMap.PhysicsTable, "MyPK", dbUpload.MyPK, "FDB");
            }

        }

        //多附件上传方法
        public void MoreAttach()
        {
            string PKVal = this.GetRequestVal("PKVal");
            string attachPk = this.getUTF8ToString("AttachPK");
            // 多附件描述.
            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment(attachPk);
            MapData mapData = new MapData(athDesc.FK_MapData);
            string msg = null;
            GEEntity en = new GEEntity(athDesc.FK_MapData);
            en.PKVal = PKVal;
            en.Retrieve();

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                string savePath = athDesc.SaveTo;
                if (savePath.Contains("@") == true || savePath.Contains("*") == true)
                {
                    /*如果有变量*/
                    savePath = savePath.Replace("*", "@");
                    savePath = BP.WF.Glo.DealExp(savePath, en, null);

                    if (savePath.Contains("@") && this.FK_Node != 0)
                    {
                        /*如果包含 @ */
                        BP.WF.Flow flow = new BP.WF.Flow(this.FK_Flow);
                        BP.WF.Data.GERpt myen = flow.HisGERpt;
                        myen.OID = this.WorkID;
                        myen.RetrieveFromDBSources();
                        savePath = BP.WF.Glo.DealExp(savePath, myen, null);
                    }
                    if (savePath.Contains("@") == true)
                        throw new Exception("@路径配置错误,变量没有被正确的替换下来." + savePath);
                }
                else
                {
                    savePath = athDesc.SaveTo + "\\" + PKVal;
                }

                //替换关键的字串.
                savePath = savePath.Replace("\\\\", "\\");
                try
                {
                    savePath = context.Server.MapPath("~/" + savePath);
                }
                catch (Exception)
                {
                }

                try
                {
                    if (System.IO.Directory.Exists(savePath) == false)
                        System.IO.Directory.CreateDirectory(savePath);
                }
                catch (Exception ex)
                {
                    throw new Exception("@创建路径出现错误，可能是没有权限或者路径配置有问题:" + context.Server.MapPath("~/" + savePath) + "===" + savePath + "@技术问题:" + ex.Message);
                }

                string exts = System.IO.Path.GetExtension(context.Request.Files[i].FileName).ToLower().Replace(".", "");
                string guid = BP.DA.DBAccess.GenerGUID();
                string fileName = context.Request.Files[i].FileName.Substring(0, context.Request.Files[i].FileName.LastIndexOf('.'));
                string ext = System.IO.Path.GetExtension(context.Request.Files[i].FileName);
                string realSaveTo = savePath + "\\" + guid + "." + fileName + ext;

                realSaveTo = realSaveTo.Replace("~", "-");
                realSaveTo = realSaveTo.Replace("'", "-");
                realSaveTo = realSaveTo.Replace("*", "-");

                context.Request.Files[i].SaveAs(realSaveTo);
                
                //执行附件上传前事件，added by liuxc,2017-7-15
                msg = mapData.DoEvent(FrmEventList.AthUploadeBefore, en, "@FK_FrmAttachment=" + athDesc.MyPK + "@FileFullName=" + realSaveTo);
                if (!string.IsNullOrEmpty(msg))
                {
                    BP.Sys.Glo.WriteLineError("@AthUploadeBefore事件返回信息，文件：" + context.Request.Files[i].FileName + "，" + msg);

                    try
                    {
                        File.Delete(realSaveTo);
                    }
                    catch { }

                    //note:此处如何向前uploadify传递失败信息，有待研究
                    //this.Alert("上传附件错误：" + msg, true);
                    return;
                }

                FileInfo info = new FileInfo(realSaveTo);

                FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                dbUpload.MyPK = guid; // athDesc.FK_MapData + oid.ToString();
                dbUpload.NodeID = this.FK_Node.ToString();
                dbUpload.FK_FrmAttachment = attachPk;
                dbUpload.FK_MapData = athDesc.FK_MapData;
                dbUpload.FK_FrmAttachment = attachPk;
                dbUpload.FileExts = info.Extension;

                #region 处理文件路径，如果是保存到数据库，就存储pk.
                if (athDesc.SaveWay == 0)
                {
                    //文件方式保存
                    dbUpload.FileFullName = realSaveTo;
                }

                if (athDesc.SaveWay == 1)
                {
                    //保存到数据库
                    dbUpload.FileFullName = dbUpload.MyPK;
                }
                #endregion 处理文件路径，如果是保存到数据库，就存储pk.

                dbUpload.FileName = context.Request.Files[i].FileName;
                dbUpload.FileSize = (float)info.Length;
                dbUpload.RDT = DataType.CurrentDataTimess;
                dbUpload.Rec = BP.Web.WebUser.No;
                dbUpload.RecName = BP.Web.WebUser.Name;
                dbUpload.RefPKVal = PKVal;
                dbUpload.FID = this.FID;

                //if (athDesc.IsNote)
                //    dbUpload.MyNote = this.Pub1.GetTextBoxByID("TB_Note").Text;

                //if (athDesc.Sort.Contains(","))
                //    dbUpload.Sort = this.Pub1.GetDDLByID("ddl").SelectedItemStringVal;

                dbUpload.UploadGUID = guid;
                dbUpload.Insert();

                if (athDesc.SaveWay == 1)
                {
                    //执行文件保存.
                    BP.DA.DBAccess.SaveFileToDB(realSaveTo, dbUpload.EnMap.PhysicsTable, "MyPK", dbUpload.MyPK, "FDB");
                }

                //执行附件上传后事件，added by liuxc,2017-7-15
                msg = mapData.DoEvent(FrmEventList.AthUploadeAfter, en, "@FK_FrmAttachment=" + dbUpload.FK_FrmAttachment + "@FK_FrmAttachmentDB=" + dbUpload.MyPK + "@FileFullName=" + dbUpload.FileFullName);
                if (!string.IsNullOrEmpty(msg))
                    BP.Sys.Glo.WriteLineError("@AthUploadeAfter事件返回信息，文件：" + dbUpload.FileName + "，" + msg);
            }
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="MyPK"></param>
        /// <returns></returns>
        public string DelWorkCheckAttach()
        {
            FrmAttachmentDB athDB = new FrmAttachmentDB();
            athDB.RetrieveByAttr(FrmAttachmentDBAttr.MyPK, this.MyPK);
            //删除文件
            if (athDB.FileFullName != null)
            {
                if (File.Exists(athDB.FileFullName) == true)
                    File.Delete(athDB.FileFullName);
            }
            int i = athDB.Delete(FrmAttachmentDBAttr.MyPK, this.MyPK);
            if (i > 0)
                return "true";
            return "false";
        }
    }
}
