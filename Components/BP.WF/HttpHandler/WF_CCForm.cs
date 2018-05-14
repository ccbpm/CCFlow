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
using BP.En;
using ICSharpCode.SharpZipLib.Zip;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 表单功能界面
    /// </summary>
    public class WF_CCForm : DirectoryPageBase
    {
        #region 多附件.
        public string Ath_Init()
        {
            try
            {
                DataSet ds = new DataSet();

                FrmAttachment athDesc = this.GenerAthDesc();


                //查询出来数据实体.
                BP.Sys.FrmAttachmentDBs dbs = BP.WF.Glo.GenerFrmAttachmentDBs(athDesc, this.PKVal,this.FK_FrmAttachment);

                #region 如果图片显示.(先不考虑.)
                if (athDesc.FileShowWay == FileShowWay.Pict)
                {
                    /* 如果是图片轮播，就在这里根据数据输出轮播的html代码.*/
                    if (dbs.Count == 0 && athDesc.IsUpload == true)
                    {
                        /*没有数据并且，可以上传,就转到上传的界面上去.*/
                        return "url@AthImg.htm?1=1" + this.RequestParas;
                    }
                }
                #endregion 如果图片显示.

                #region 执行装载模版.
                if (dbs.Count == 0 && athDesc.IsWoEnableTemplete == true)
                {
                    /*如果数量为0,就检查一下是否有模版如果有就加载模版文件.*/
                    string templetePath = BP.Sys.SystemConfig.PathOfDataUser + "AthTemplete\\" + athDesc.NoOfObj.Trim();
                    if (Directory.Exists(templetePath) == false)
                        Directory.CreateDirectory(templetePath);

                    /*有模版文件夹*/
                    DirectoryInfo mydir = new DirectoryInfo(templetePath);
                    FileInfo[] fls = mydir.GetFiles();
                    if (fls.Length == 0)
                        throw new Exception("@流程设计错误，该多附件启用了模版组件，模版目录:" + templetePath + "里没有模版文件.");

                    foreach (FileInfo fl in fls)
                    {
                        if (System.IO.Directory.Exists(athDesc.SaveTo) == false)
                            System.IO.Directory.CreateDirectory(athDesc.SaveTo);

                        int oid = BP.DA.DBAccess.GenerOID();
                        string saveTo = athDesc.SaveTo + "\\" + oid + "." + fl.Name.Substring(fl.Name.LastIndexOf('.') + 1);
                        if (saveTo.Contains("@") == true || saveTo.Contains("*") == true)
                        {
                            /*如果有变量*/
                            saveTo = saveTo.Replace("*", "@");
                            if (saveTo.Contains("@") && this.FK_Node != 0)
                            {
                                /*如果包含 @ */
                                BP.WF.Flow flow = new BP.WF.Flow(this.FK_Flow);
                                BP.WF.Data.GERpt myen = flow.HisGERpt;
                                myen.OID = this.WorkID;
                                myen.RetrieveFromDBSources();
                                saveTo = BP.WF.Glo.DealExp(saveTo, myen, null);
                            }
                            if (saveTo.Contains("@") == true)
                                throw new Exception("@路径配置错误,变量没有被正确的替换下来." + saveTo);
                        }
                        fl.CopyTo(saveTo);

                        FileInfo info = new FileInfo(saveTo);
                        FrmAttachmentDB dbUpload = new FrmAttachmentDB();

                        dbUpload.CheckPhysicsTable();
                        dbUpload.MyPK = athDesc.FK_MapData + oid.ToString();
                        dbUpload.NodeID = FK_Node.ToString();
                        dbUpload.FK_FrmAttachment = this.FK_FrmAttachment;

                        if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                        {
                            /*如果是继承，就让他保持本地的PK. */
                            dbUpload.RefPKVal = this.PKVal.ToString();
                        }

                        if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                        {
                            /*如果是协同，就让他是PWorkID. */
                            string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + this.PKVal, 0).ToString();
                            if (pWorkID == null || pWorkID == "0")

                                pWorkID = this.PKVal;
                            dbUpload.RefPKVal = pWorkID;
                        }

                        dbUpload.FK_MapData = athDesc.FK_MapData;
                        dbUpload.FK_FrmAttachment = this.FK_FrmAttachment;

                        dbUpload.FileExts = info.Extension;
                        dbUpload.FileFullName = saveTo;
                        dbUpload.FileName = fl.Name;
                        dbUpload.FileSize = (float)info.Length;

                        dbUpload.RDT = DataType.CurrentDataTime;
                        dbUpload.Rec = BP.Web.WebUser.No;
                        dbUpload.RecName = BP.Web.WebUser.Name;

                        dbUpload.Insert();

                        dbs.AddEntity(dbUpload);
                    }
                }
                #endregion 执行装载模版.

                #region 处理权限问题.
                // 处理权限问题, 有可能当前节点是可以上传或者删除，但是当前节点上不能让此人执行工作。
                // bool isDel = athDesc.IsDeleteInt == 0 ? false : true;
                bool isDel = athDesc.HisDeleteWay == AthDeleteWay.None ? false : true;
                bool isUpdate = athDesc.IsUpload;
                if (isDel == true || isUpdate == true)
                {
                    if (this.WorkID != 0
                        && DataType.IsNullOrEmpty(this.FK_Flow) == false
                        && this.FK_Node != 0)
                    {
                        isDel = BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Flow, this.FK_Node, this.WorkID, WebUser.No);
                        if (isDel == false)
                            isUpdate = false;
                    }
                }
                athDesc.IsUpload = isUpdate;
                //athDesc.HisDeleteWay = AthDeleteWay.DelAll; 
                #endregion 处理权限问题.

                //增加附件描述.
                ds.Tables.Add(athDesc.ToDataTableField("AthDesc"));

                //增加附件.
                ds.Tables.Add(dbs.ToDataTableField("DBAths"));
              
                //返回.
                return BP.Tools.Json.ToJson(ds);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion 多附件.

        #region HanderMapExt
        /// <summary>
        /// 扩展处理.
        /// </summary>
        /// <returns></returns>
        public string HandlerMapExt()
        {
            string fk_mapExt = this.GetRequestVal("FK_MapExt").ToString();
            if (DataType.IsNullOrEmpty(this.GetRequestVal("Key")))
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
                        case "ReqDtlFullList":
                            /* 获取填充的从表集合. */
                            DataTable dtDtl = new DataTable("Head");
                            dtDtl.Columns.Add("Dtl", typeof(string));
                            string[] strsDtl = me.Tag1.Split('$');
                            foreach (string str in strsDtl)
                            {
                                if (DataType.IsNullOrEmpty(str))
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
                            if (DataType.IsNullOrEmpty(me.Tag) == false)
                            {
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
                                return JSONTODT(dt1);
                            }
                            return "";
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
        /// <summary>
        /// 处理sql.
        /// </summary>
        /// <param name="sql">要执行的sql</param>
        /// <param name="key">关键字值</param>
        /// <returns>执行sql返回的json</returns>
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

            if (DataType.IsNullOrEmpty(kvs) == false && sql.Contains("@") == true)
            {
                string[] strs = kvs.Split('~');
                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s)
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
                if (mysql == "")
                    return "";

                mysql = mysql.Substring(6, mysql.ToLower().IndexOf("from") - 6);
                mysql = mysql.Replace(",", "|");
                string[] strs = mysql.Split('|');
                string[] pstr = null;
                string ns = null;

                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s))
                        continue;
                    //处理ORACLE中获取字段使用别名的情况，使用别名的字段，取别名
                    ns = s.Trim();
                    pstr = ns.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (pstr.Length > 1)
                    {
                        ns = pstr[pstr.Length - 1].Replace("\"", "");
                    }

                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName.ToLower() == ns.ToLower())
                        {
                            dc.ColumnName = ns;
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
            JsonString.Append("{ ");
            JsonString.Append("\"Head\":[ ");

            if (dt != null && dt.Rows.Count > 0)
            {
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
            }

            JsonString.Append("]}");

            return JsonString.ToString();
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

            if (md.HisFrmType == FrmType.Entity)
            {
                string no = this.GetRequestVal("NO");
                string urlParas = "OID=" + this.RefOID + "&NO=" + no + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&UserNo=" + WebUser.No + "&SID=" + this.SID;

                BP.En.Entities ens = BP.En.ClassFactory.GetEns(md.PTable);

                BP.En.Entity en = ens.GetNewEntity;

                if (en.IsOIDEntity == true)
                {
                    BP.En.EntityOID enOID = null;

                    enOID = en as BP.En.EntityOID;
                    if (enOID == null)
                    {
                        return "err@系统错误，无法将" + md.PTable + "转化成BP.En.EntityOID.";
                    }

                    enOID.SetValByKey("OID", this.WorkID);

                    if (en.RetrieveFromDBSources() == 0)
                    {
                        foreach (string key in context.Request.QueryString.Keys)
                        {
                            enOID.SetValByKey(key, context.Request.QueryString[key]);
                        }
                        enOID.SetValByKey("OID", this.WorkID);

                        enOID.InsertAsOID(this.WorkID);
                    }
                }
                return "url@../Comm/En.htm?EnName=" + md.PTable + "&PKVal=" + this.WorkID;
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
                    if (DataType.IsNullOrEmpty(str) || str.Contains("=") == false)
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

            //处理参数.
            string paras = this.RequestParas;
            paras = paras.Replace("&DoType=Frm_Init", "");

          
                if (md.HisFrmType == FrmType.FreeFrm)
                {
                    if (this.GetRequestVal("Readonly") == "1" || this.GetRequestVal("IsEdit") == "0")
                        return "url@FrmGener.htm?1=2" + paras;
                    else
                        return "url@FrmGener.htm?1=2" + paras;
                }

                if (md.HisFrmType == FrmType.VSTOForExcel || md.HisFrmType == FrmType.ExcelFrm )
                {
                    if (this.GetRequestVal("Readonly") == "1" || this.GetRequestVal("IsEdit") == "0")
                        return "url@FrmVSTO.aspx?1=2" + paras;
                    else
                        return "url@FrmVSTO.aspx?1=2" + paras;
                }

                if (this.GetRequestVal("Readonly") == "1" || this.GetRequestVal("IsEdit") == "0")
                    return "url@FrmGener.htm?1=2" + paras;
                else
                    return "url@FrmGener.htm?1=2" + paras;
            
        }

        /// <summary>
        /// 附件图片
        /// </summary>
        /// <returns></returns>
        public string FrmImgAthDB_Init()
        {
            string ImgAthPK = this.GetRequestVal("ImgAth");

            FrmImgAthDBs imgAthDBs = new FrmImgAthDBs();
            QueryObject obj = new QueryObject(imgAthDBs);
            obj.AddWhere(FrmImgAthDBAttr.FK_MapData, this.FK_MapData);
            obj.addAnd();
            obj.AddWhere(FrmImgAthDBAttr.FK_FrmImgAth, ImgAthPK);
            obj.addAnd();
            obj.AddWhere(FrmImgAthDBAttr.RefPKVal, this.MyPK);
            obj.DoQuery();
            return BP.Tools.Entitis2Json.ConvertEntities2ListJson(imgAthDBs);
        }
        /// <summary>
        /// 上传编辑图片
        /// </summary>
        /// <returns></returns>
        public string FrmImgAthDB_Upload()
        {
            string ImgAthPK = this.GetRequestVal("ImgAth");
            int zoomW = this.GetRequestValInt("zoomW");
            int zoomH = this.GetRequestValInt("zoomH");

            HttpFileCollection files = this.context.Request.Files;
            if (files.Count > 0 && files[0].ContentLength > 0)
            {
                string myName = ImgAthPK + "_" + this.MyPK;
                //生成新路径，解决返回相同src后图片不切换问题
                string newName = ImgAthPK + "_" + this.MyPK + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                string webPath = BP.WF.Glo.CCFlowAppPath + "DataUser/ImgAth/Data/" + newName + ".png";
                string saveToPath = this.context.Server.MapPath(BP.WF.Glo.CCFlowAppPath + "DataUser/ImgAth/Data");
                string fileUPloadPath = this.context.Server.MapPath(BP.WF.Glo.CCFlowAppPath + "DataUser/ImgAth/Upload");
                //创建路径
                if (!Directory.Exists(saveToPath))
                    Directory.CreateDirectory(saveToPath);
                if (!Directory.Exists(fileUPloadPath))
                    Directory.CreateDirectory(fileUPloadPath);

                saveToPath = saveToPath + "\\" + newName + ".png";
                fileUPloadPath = fileUPloadPath + "\\" + newName + ".png";
                files[0].SaveAs(saveToPath);

                //源图像  
                System.Drawing.Bitmap oldBmp = new System.Drawing.Bitmap(saveToPath);

                //新图像,并设置新图像的宽高  
                System.Drawing.Bitmap newBmp = new System.Drawing.Bitmap(zoomW, zoomH);
                System.Drawing.Graphics draw = System.Drawing.Graphics.FromImage(newBmp);//从新图像获取对应的Graphics  
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, zoomW, zoomH);//指定绘制新图像的位置和大小  
                draw.DrawImage(oldBmp, rect);//把源图像的全部完整的内容，绘制到新图像rect这个区域内，

                draw.Dispose();
                oldBmp.Dispose();//一定要把源图Dispose调，因为保存的是相同路径，需要把之前的图顶替调，如果不释放的话会报错：（GDI+ 中发生一般性错误。）  
                newBmp.Save(saveToPath);//保存替换到同一个路径 

                //复制一份
                File.Copy(saveToPath, fileUPloadPath, true);
                //获取文件大小
                FileInfo fileInfo = new FileInfo(saveToPath);
                float fileSize = 0;
                if (fileInfo.Exists)
                    fileSize = float.Parse(fileInfo.Length.ToString());

                //更新数据表                
                FrmImgAthDB imgAthDB = new FrmImgAthDB();
                imgAthDB.MyPK = myName;
                imgAthDB.FK_MapData = this.FK_MapData;
                imgAthDB.FK_FrmImgAth = ImgAthPK;
                imgAthDB.RefPKVal = this.MyPK;
                imgAthDB.FileFullName = webPath;
                imgAthDB.FileName = newName;
                imgAthDB.FileExts = "png";
                imgAthDB.FileSize = fileSize;
                imgAthDB.RDT = DateTime.Now.ToString("yyyy-MM-dd mm:HH");
                imgAthDB.Rec = BP.Web.WebUser.No;
                imgAthDB.RecName = BP.Web.WebUser.Name;
                imgAthDB.Save();
                return "{SourceImage:\"" + webPath + "\"}";
            }
            return "{err:\"没有选择文件\"}";
        }

        /// <summary>
        /// 剪切图片
        /// </summary>
        /// <returns></returns>
        public string FrmImgAthDB_Cut()
        {
            string ImgAthPK = this.GetRequestVal("ImgAth");

            int zoomW = this.GetRequestValInt("zoomW");
            int zoomH = this.GetRequestValInt("zoomH");
            int x = this.GetRequestValInt("cX");
            int y = this.GetRequestValInt("cY");
            int w = this.GetRequestValInt("cW");
            int h = this.GetRequestValInt("cH");

            string myPK = ImgAthPK + "_" + this.MyPK;
            FrmImgAthDB imgAthDB = new FrmImgAthDB(myPK);

            string appPath = SystemConfig.CCFlowAppPath;
            appPath = SystemConfig.CCFlowWebPath;

            string newName = ImgAthPK + "_" + this.MyPK + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string webPath = BP.WF.Glo.CCFlowAppPath + "DataUser/ImgAth/Data/" + newName + ".png";
            string savePath = SystemConfig.CCFlowAppPath + "DataUser/ImgAth/Data/" + newName + ".png";
            //获取上传的大图片
            string strImgPath = this.context.Server.MapPath(SystemConfig.CCFlowWebPath + "DataUser/ImgAth/Upload/" + imgAthDB.FileName + ".png");
            if (File.Exists(strImgPath) == true)
            {
                //剪切图
                bool bSuc = Crop(strImgPath, savePath, w, h, x, y);
               imgAthDB.FileFullName = webPath;
               imgAthDB.Update();
               return webPath;
            }
            return imgAthDB.FileFullName;
        }

        /// <summary>
        /// 剪裁图像
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        private bool Crop(string Img,string savePath, int Width, int Height, int X, int Y)
        {
            try
            {
                using (var OriginalImage = new System.Drawing.Bitmap(Img))
                {
                    using (var bmp = new System.Drawing.Bitmap(Width, Height, OriginalImage.PixelFormat))
                    {
                        bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                        using (System.Drawing.Graphics Graphic = System.Drawing.Graphics.FromImage(bmp))
                        {
                            Graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            Graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            Graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            Graphic.DrawImage(OriginalImage, new System.Drawing.Rectangle(0, 0, Width, Height), X, Y, Width, Height, System.Drawing.GraphicsUnit.Pixel);
                            //var ms = new MemoryStream();
                            bmp.Save(savePath);
                            //return ms.GetBuffer();
                            return true;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
            return false;
        }
        #endregion frm.htm 主表.

        #region DtlFrm
        public string DtlFrm_Init()
        {
            Int64 pk = this.RefOID;
            if (pk == 0)
                pk = this.OID;
            if (pk == 0)
                pk = this.WorkID;

            if (pk != 0)
                return FrmGener_Init();
            MapDtl dtl = new MapDtl(this.EnsName);

            GEEntity en = new GEEntity(this.EnsName);
            if (BP.Sys.SystemConfig.IsBSsystem == true)
            {
                // 处理传递过来的参数。
                foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                {
                    en.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
                }
            }

            //设置主键.
            en.OID = DBAccess.GenerOID(this.EnsName);
            en.SetValByKey("RefPK", this.RefPKVal);
            en.Insert();

            return "url@DtlFrm.htm?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&FrmType=" + (int)dtl.HisEditModel + "&OID=" + en.OID;
        }

        public string DtlFrm_Delete()
        {
            try
            {
                GEEntity en = new GEEntity(this.EnsName);
                en.OID = this.OID;
                en.Delete();

                return "删除成功.";
            }
            catch (Exception ex)
            {
                return "err@删除错误:" + ex.Message;
            }
        }

        #endregion DtlFrm

        #region frmFree
        /// <summary>
        /// 执行数据初始化
        /// </summary>
        /// <returns></returns>
        public string FrmGener_Init()
        {
            try
            {
                MapData md = new MapData(this.EnsName);
                DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No);

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

                if (this.FK_Node != 0 && DataType.IsNullOrEmpty(this.FK_Flow) == false)
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
                if (en.OID == 0)
                {
                    en.ResetDefaultVal();
                }
                else
                {
                    if (en.RetrieveFromDBSources() == 0)
                        en.Insert();
                }

                //把参数放入到 En 的 Row 里面。
                if (DataType.IsNullOrEmpty(atParas) == false)
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
                if (DataType.IsNullOrEmpty(msg) == false)
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

                #region 把外键表加入DataSet
                DataTable dtMapAttr = ds.Tables["Sys_MapAttr"];

                MapExts mes = md.MapExts;

                foreach (DataRow dr in dtMapAttr.Rows)
                {
                    string lgType = dr["LGType"].ToString();
                    if (lgType.Equals("2")==false)
                        continue;

                    string UIIsEnable = dr["UIVisible"].ToString();
                    if (UIIsEnable == "0")
                        continue;

                    //string lgType = dr[MapAttrAttr.LGType].ToString();
                    //if (lgType == "0")
                    //    continue

                    string uiBindKey = dr["UIBindKey"].ToString();
                    if (DataType.IsNullOrEmpty(uiBindKey) == true)
                    {
                        string myPK = dr["MyPK"].ToString();
                        /*如果是空的*/
                        //   throw new Exception("@属性字段数据不完整，流程:" + fl.No + fl.Name + ",节点:" + nd.NodeID + nd.Name + ",属性:" + myPK + ",的UIBindKey IsNull ");
                    }

                    // 检查是否有下拉框自动填充。
                    string keyOfEn = dr["KeyOfEn"].ToString();
                    string fk_mapData = dr["FK_MapData"].ToString();

                    #region 处理下拉框数据范围. for 小杨.
                    me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                    if (me != null)
                    {
                        string fullSQL = me.Doc.Clone() as string;
                        fullSQL = fullSQL.Replace("~", ",");
                        fullSQL = BP.WF.Glo.DealExp(fullSQL, en, null);
                        DataTable dt = DBAccess.RunSQLReturnTable(fullSQL);
                        dt.TableName = keyOfEn; //可能存在隐患，如果多个字段，绑定同一个表，就存在这样的问题.
                        ds.Tables.Add(dt);
                        continue;
                    }
                    #endregion 处理下拉框数据范围.

                    // 判断是否存在.
                    if (ds.Tables.Contains(uiBindKey) == true)
                        continue;

                    ds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
                }
                #endregion End把外键表加入DataSet

                #region 加入组件的状态信息, 在解析表单的时候使用.
                if (this.FK_Node != 0 && this.FK_Node != 999999)
                {
                    Node nd = new Node(this.FK_Node);
                    nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

                    BP.WF.Template.FrmNodeComponent fnc = new FrmNodeComponent(nd.NodeID);
                    if (nd.NodeFrmID != "ND" + nd.NodeID)
                    {
                        /*说明这是引用到了其他节点的表单，就需要把一些位置元素修改掉.*/
                        int refNodeID = int.Parse(nd.NodeFrmID.Replace("ND", ""));

                        BP.WF.Template.FrmNodeComponent refFnc = new FrmNodeComponent(refNodeID);

                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_H, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_H));
                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_W, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_W));
                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_X, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_X));
                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_Y, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_Y));


                        fnc.SetValByKey(FrmSubFlowAttr.SF_H, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_H));
                        fnc.SetValByKey(FrmSubFlowAttr.SF_W, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_W));
                        fnc.SetValByKey(FrmSubFlowAttr.SF_X, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_X));
                        fnc.SetValByKey(FrmSubFlowAttr.SF_Y, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_Y));

                        fnc.SetValByKey(FrmThreadAttr.FrmThread_H, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_H));
                        fnc.SetValByKey(FrmThreadAttr.FrmThread_W, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_W));
                        fnc.SetValByKey(FrmThreadAttr.FrmThread_X, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_X));
                        fnc.SetValByKey(FrmThreadAttr.FrmThread_Y, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_Y));

                        fnc.SetValByKey(FrmTrackAttr.FrmTrack_H, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_H));
                        fnc.SetValByKey(FrmTrackAttr.FrmTrack_W, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_W));
                        fnc.SetValByKey(FrmTrackAttr.FrmTrack_X, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_X));
                        fnc.SetValByKey(FrmTrackAttr.FrmTrack_Y, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_Y));

                        fnc.SetValByKey(FTCAttr.FTC_H, refFnc.GetValFloatByKey(FTCAttr.FTC_H));
                        fnc.SetValByKey(FTCAttr.FTC_W, refFnc.GetValFloatByKey(FTCAttr.FTC_W));
                        fnc.SetValByKey(FTCAttr.FTC_X, refFnc.GetValFloatByKey(FTCAttr.FTC_X));
                        fnc.SetValByKey(FTCAttr.FTC_Y, refFnc.GetValFloatByKey(FTCAttr.FTC_Y));
                    }

                    ds.Tables.Add(fnc.ToDataTableField("WF_FrmNodeComponent"));
                    ds.Tables.Add(nd.ToDataTableField("WF_Node"));

                }
                #endregion 加入组件的状态信息, 在解析表单的时候使用.

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
        public string FrmFreeReadonly_Init()
        {
            try
            {
                MapData md = new MapData(this.EnsName);
                DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No);

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

                if (this.FK_Node != 0 && DataType.IsNullOrEmpty(this.FK_Flow) == false)
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
                en.RetrieveFromDBSources();

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
        public string FrmGener_Save()
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
        #endregion

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

            if (this.FK_Node != 0 && mdtl.FK_MapData != "Temp" && this.EnsName.Contains("ND" + this.FK_Node) == false && this.FK_Node != 999999)
            {
                Node nd = new BP.WF.Node(this.FK_Node);
                /*如果
                 * 1,传来节点ID, 不等于0.
                 * 2,不是节点表单.  就要判断是否是独立表单，如果是就要处理权限方案。*/

                BP.WF.Template.FrmNode fn = new BP.WF.Template.FrmNode(nd.FK_Flow, nd.NodeID, mdtl.FK_MapData);
                int i = fn.Retrieve(FrmNodeAttr.FK_Frm, mdtl.FK_MapData, FrmNodeAttr.FK_Node, this.FK_Node);
                if (i != 0 && fn.FrmSln > 1)
                {
                    /*使用了自定义的方案. 
                     * 并且，一定为dtl设定了自定义方案，就用自定义方案.
                     */
                    mdtl.No = this.EnsName + "_" + this.FK_Node;
                    mdtl.RetrieveFromDBSources();
                }

                if (i != 0 && fn.FrmSln == 1)
                {
                    mdtl.IsInsert = false;
                    mdtl.IsDelete = false;
                    mdtl.IsUpdate = false;
                }
            }

            if (this.GetRequestVal("IsReadonly") == "1" )
            {
                mdtl.IsInsert = false;
                mdtl.IsDelete = false;
                mdtl.IsUpdate = false;
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
            //if (this.RefPKVal == "0" || this.RefPKVal == "")
            //    return "err@从表保存[Dtl_SaveRow],失败没有接收到refpk的值";

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
                if (DataType.IsNullOrEmpty(msg) == false)
                    return "err@" + msg;
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


            //一直找不到refpk  值为null .
            dtl.RefPK = this.RefPKVal;
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
                if (DataType.IsNullOrEmpty(msg) == false)
                    return "err@" + msg;
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

        #region dtl.Card
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string DtlCard_Init()
        {
            DataSet ds = new DataSet();

            MapDtl md = new MapDtl(this.EnsName);

            //主表数据.
            DataTable dt = md.ToDataTableField("Main");
            ds.Tables.Add(dt);

            //主表字段.
            MapAttrs attrs = md.MapAttrs;
            ds.Tables.Add(attrs.ToDataTableField("MapAttrs"));

            //从表.
            MapDtls dtls = md.MapDtls;
            ds.Tables.Add(dtls.ToDataTableField("MapDtls"));

            //从表的从表.
            foreach (MapDtl dtl in dtls)
            {
                MapAttrs subAttrs = new MapAttrs(dtl.No);
                ds.Tables.Add(subAttrs.ToDataTableField(dtl.No));
            }

            //从表的数据.
            GEDtls enDtls = new GEDtls(this.EnsName);
            enDtls.Retrieve(GEDtlAttr.RefPK, this.RefPKVal);
            ds.Tables.Add(enDtls.ToDataTableField("DTDtls"));

            return BP.Tools.Json.ToJson(ds);

        }
        /// <summary>
        /// 获得从表的从表数据
        /// </summary>
        /// <returns></returns>
        public string DtlCard_Init_Dtl()
        {
            DataSet ds = new DataSet();

            MapDtl md = new MapDtl(this.EnsName);

            //主表数据.
            DataTable dt = md.ToDataTableField("Main");
            ds.Tables.Add(dt);

            //主表字段.
            MapAttrs attrs = md.MapAttrs;
            ds.Tables.Add(attrs.ToDataTableField("MapAttrs"));

            GEDtls enDtls = new GEDtls(this.EnsName);
            enDtls.Retrieve(GEDtlAttr.RefPK, this.RefPKVal);
            ds.Tables.Add(enDtls.ToDataTableField("DTDtls"));

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion dtl.Card


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
        public string PopVal_InitTree()
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

            //判断是否是oracle.
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["PARENTNO"].ColumnName = "ParentNo";
            }
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


                //判断是否是oracle.
                if (SystemConfig.AppCenterDBType == DBType.Oracle)
                {
                    entityDt.Columns["NO"].ColumnName = "No";
                    entityDt.Columns["NAME"].ColumnName = "Name";
                    // entityDt.Columns["PARENTNO"].ColumnName = "ParentNo";
                }

            }

            return BP.Tools.Json.ToJson(resultDs);
        }

        /// <summary>
        /// 处理DataTable中的列名，将不规范的No,Name,ParentNo列纠正
        /// </summary>
        /// <param name="dt"></param>
        public void DoCheckTableColumnNameCase(DataTable dt)
        {
            foreach(DataColumn col in dt.Columns)
            {
                switch(col.ColumnName.ToLower())
                {
                    case "no":
                        col.ColumnName = "No";
                        break;
                    case "name":
                        col.ColumnName = "Name";
                        break;
                    case "parentno":
                        col.ColumnName = "ParentNo";
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 初始化PopVal的值   除了分页表格模式之外的其他数据值
        /// </summary>
        /// <returns></returns>
        public string PopVal_Init()
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
                DoCheckTableColumnNameCase(dt);
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
                    DoCheckTableColumnNameCase(dt);
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
                    DoCheckTableColumnNameCase(dt);
                    ds.Tables.Add(dt);
                }
                return BP.Tools.Json.ToJson(ds);

            }

            if (me.PopValWorkModel == PopValWorkModel.TablePage)
            {
                /* 分页的 */
                //key
                string key = this.GetRequestVal("Key");
                if (DataType.IsNullOrEmpty(key) == true)
                    key = "";

                //取出来查询条件.
                string[] conds = me.PopValSearchCond.Split('$');

                //pageSize
                string pageSize = this.GetRequestVal("pageSize");
                if (DataType.IsNullOrEmpty(pageSize))
                    pageSize = "10";

                //pageIndex
                string pageIndex = this.GetRequestVal("pageIndex");
                if (DataType.IsNullOrEmpty(pageIndex))
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
                    if (DataType.IsNullOrEmpty(val))
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
                DoCheckTableColumnNameCase(dt);
                ds.Tables.Add(dt);

                //处理查询条件.
                //$Para=Dept#Label=所在班级#ListSQL=Select No,Name FROM Port_Dept WHERE No='@WebUser.No'
                //$Para=XB#Label=性别#EnumKey=XB
                //$Para=DTFrom#Label=注册日期从#DefVal=@Now-30
                //$Para=DTTo#Label=到#DefVal=@Now


                foreach (string cond in conds)
                {
                    if (DataType.IsNullOrEmpty(cond) == true)
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
                    DoCheckTableColumnNameCase(dt);
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
        public string PopVal_InitTablePageCount()
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
                if (DataType.IsNullOrEmpty(key) == true)
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
                    if (DataType.IsNullOrEmpty(val))
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
                    if (DataType.IsNullOrEmpty(cond) == true)
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
        public string PopVal_InitTablePageList()
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
                if (DataType.IsNullOrEmpty(key) == true)
                    key = "";

                //取出来查询条件.
                string[] conds = me.PopValSearchCond.Split('$');

                //pageSize
                string pageSize = this.GetRequestVal("pageSize");
                if (DataType.IsNullOrEmpty(pageSize))
                    pageSize = "10";

                //pageIndex
                string pageIndex = this.GetRequestVal("pageIndex");
                if (DataType.IsNullOrEmpty(pageIndex))
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
                    if (DataType.IsNullOrEmpty(val))
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
                    if (DataType.IsNullOrEmpty(cond) == true)
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
            if (frmAth.AthSaveWay == AthSaveWay.IISServer)
            {
                //文件方式保存
                dbUpload.FileFullName = saveTo;
            }

            if (frmAth.AthSaveWay == AthSaveWay.DB)
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

            if (frmAth.AthSaveWay == AthSaveWay.DB)
            {
                //执行文件保存.
                BP.DA.DBAccess.SaveFileToDB(saveTo, dbUpload.EnMap.PhysicsTable, "MyPK", dbUpload.MyPK, "FDB");
            }
        }

        //多附件上传方法
        public void MoreAttach()
        {
            string PKVal = this.GetRequestVal("PKVal");
            string attachPk = this.GetRequestVal("AttachPK");
            string paras = this.GetRequestVal("parasData");
            // 多附件描述.
            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment(attachPk);
            MapData mapData = new MapData(athDesc.FK_MapData);
            string msg = null;
            GEEntity en = new GEEntity(athDesc.FK_MapData);
            en.PKVal = PKVal;
            en.Retrieve();

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                HttpPostedFile file = context.Request.Files[i];

                #region 文件上传的iis服务器上 or db数据库里.
                if (athDesc.AthSaveWay == AthSaveWay.IISServer)
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

                    string exts = System.IO.Path.GetExtension(file.FileName).ToLower().Replace(".", "");
                    string guid = BP.DA.DBAccess.GenerGUID();
                    string fileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                    string ext = System.IO.Path.GetExtension(file.FileName);
                    string realSaveTo = savePath + "\\" + guid + "." + fileName + ext;

                    realSaveTo = realSaveTo.Replace("~", "-");
                    realSaveTo = realSaveTo.Replace("'", "-");
                    realSaveTo = realSaveTo.Replace("*", "-");

                    file.SaveAs(realSaveTo);

                    //执行附件上传前事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeBefore, en, "@FK_FrmAttachment=" + athDesc.MyPK + "@FileFullName=" + realSaveTo);
                    if (!DataType.IsNullOrEmpty(msg))
                    {
                        BP.Sys.Glo.WriteLineError("@AthUploadeBefore事件返回信息，文件：" + file.FileName + "，" + msg);

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
                    if (athDesc.IsExpCol == true)
                    {
                        if (paras != null && paras.Length > 0)
                        {
                            foreach(string para in paras.Split('@')){
                                 dbUpload.SetPara(para.Split('=')[0],para.Split('=')[1]);
                            }
                        }
                    }
                       

                    #region 处理文件路径，如果是保存到数据库，就存储pk.
                    if (athDesc.AthSaveWay == AthSaveWay.IISServer)
                    {
                        //文件方式保存
                        dbUpload.FileFullName = realSaveTo;
                    }

                    if (athDesc.AthSaveWay == AthSaveWay.FTPServer)
                    {
                        //保存到数据库
                        dbUpload.FileFullName = dbUpload.MyPK;
                    }
                    #endregion 处理文件路径，如果是保存到数据库，就存储pk.

                    dbUpload.FileName = file.FileName;
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

                    if (athDesc.AthSaveWay == AthSaveWay.DB)
                    {
                        //执行文件保存.
                        BP.DA.DBAccess.SaveFileToDB(realSaveTo, dbUpload.EnMap.PhysicsTable, "MyPK", dbUpload.MyPK, "FDB");
                    }

                    //执行附件上传后事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeAfter, en, "@FK_FrmAttachment=" + dbUpload.FK_FrmAttachment + "@FK_FrmAttachmentDB=" + dbUpload.MyPK + "@FileFullName=" + dbUpload.FileFullName);
                    if (!DataType.IsNullOrEmpty(msg))
                        BP.Sys.Glo.WriteLineError("@AthUploadeAfter事件返回信息，文件：" + dbUpload.FileName + "，" + msg);
                }
                #endregion 文件上传的iis服务器上 or db数据库里.

                #region 保存到数据库 / FTP服务器上.
                if (athDesc.AthSaveWay == AthSaveWay.DB || athDesc.AthSaveWay == AthSaveWay.FTPServer)
                {
                    string guid = DBAccess.GenerGUID();

                    //把文件临时保存到一个位置.
                    string temp = SystemConfig.PathOfTemp + "" + guid + ".tmp";
                    try
                    {
                        file.SaveAs(temp);
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.Delete(temp);
                        file.SaveAs(temp);
                    }

                  //  fu.SaveAs(temp);

                    //执行附件上传前事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeBefore, en, "@FK_FrmAttachment=" + athDesc.MyPK + "@FileFullName=" + temp);
                    if (DataType.IsNullOrEmpty(msg) == false)
                    {
                        BP.Sys.Glo.WriteLineError("@AthUploadeBefore事件返回信息，文件：" + file.FileName + "，" + msg);

                        try
                        {
                            File.Delete(temp);
                        }
                        catch
                        {
                        }

                        throw new Exception("err@上传附件错误：" + msg);
                     }

                    FileInfo info = new FileInfo(temp);
                    FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                    dbUpload.MyPK = BP.DA.DBAccess.GenerGUID();
                    dbUpload.NodeID = FK_Node.ToString();
                    dbUpload.FK_FrmAttachment = athDesc.MyPK;
                    dbUpload.FID = this.FID; //流程id.
                    if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                    {
                        /*如果是继承，就让他保持本地的PK. */
                        dbUpload.RefPKVal = PKVal.ToString();
                    }

                    if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                    {
                        /*如果是协同，就让他是PWorkID. */
                        string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + PKVal, 0).ToString();
                        if (pWorkID == null || pWorkID == "0")
                            pWorkID = PKVal;
                        dbUpload.RefPKVal = pWorkID;
                    }

                    dbUpload.FK_MapData = athDesc.FK_MapData;
                    dbUpload.FK_FrmAttachment = athDesc.MyPK;
                   // dbUpload.AthSaveWay = athDesc.AthSaveWay; //设置保存方式,以方便前台展示读取.
                    //dbUpload.FileExts = info.Extension;
                    // dbUpload.FileFullName = saveTo;
                    dbUpload.FileName = file.FileName;
                    dbUpload.FileSize = (float)info.Length;
                    dbUpload.RDT = DataType.CurrentDataTimess;
                    dbUpload.Rec = BP.Web.WebUser.No;
                    dbUpload.RecName = BP.Web.WebUser.Name;
                    if (athDesc.IsExpCol == true)
                    {
                        if (paras != null && paras.Length > 0)
                        {
                            foreach (string para in paras.Split('@'))
                            {
                                dbUpload.SetPara(para.Split('=')[0], para.Split('=')[1]);
                            }
                        }
                    }

                    dbUpload.UploadGUID = guid;

                    if (athDesc.AthSaveWay == AthSaveWay.DB)
                    {
                        dbUpload.Insert();
                        //把文件保存到指定的字段里.
                        dbUpload.SaveFileToDB("FileDB", temp);
                    }

                    if (athDesc.AthSaveWay == AthSaveWay.FTPServer)
                    {
                        /*保存到fpt服务器上.*/
                        FtpSupport.FtpConnection ftpconn = new FtpSupport.FtpConnection(SystemConfig.FTPServerIP,
                            SystemConfig.FTPUserNo, SystemConfig.FTPUserPassword);

                        string ny = DateTime.Now.ToString("yyyy_MM");

                        //判断目录年月是否存在.
                        if (ftpconn.DirectoryExist(ny) == false)
                            ftpconn.CreateDirectory(ny);
                        ftpconn.SetCurrentDirectory(ny);

                        //判断目录是否存在.
                        if (ftpconn.DirectoryExist(athDesc.FK_MapData) == false)
                            ftpconn.CreateDirectory(athDesc.FK_MapData);

                        //设置当前目录，为操作的目录。
                        ftpconn.SetCurrentDirectory(athDesc.FK_MapData);

                        //把文件放上去.
                        ftpconn.PutFile(temp, guid + "." + dbUpload.FileExts);
                        ftpconn.Close();

                        //设置路径.
                        dbUpload.FileFullName = ny + "//" + athDesc.FK_MapData + "//" + guid + "." + dbUpload.FileExts;
                        dbUpload.Insert();
                    }

                    //执行附件上传后事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeAfter, en, "@FK_FrmAttachment=" + dbUpload.FK_FrmAttachment + "@FK_FrmAttachmentDB=" + dbUpload.MyPK + "@FileFullName=" + temp);
                    if (DataType.IsNullOrEmpty(msg)==false)
                        BP.Sys.Glo.WriteLineError("@AthUploadeAfter事件返回信息，文件：" + dbUpload.FileName + "，" + msg);

                }
                #endregion 保存到数据库.
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

        /// <summary>
        /// 表单处理加载
        /// </summary>
        /// <returns></returns>
        public string FrmSingle_Init()
        {
            if (string.IsNullOrWhiteSpace(this.FK_MapData))
                throw new Exception("FK_MapData参数不能为空");

            MapData md = new MapData();
            md.No = this.FK_MapData;

            if (md.RetrieveFromDBSources() == 0)
                throw new Exception("未检索到FK_MapData=" + this.FK_MapData + "的表单，请核对参数");

            int minOID = 10000000;//最小OID设置为一千万
            int oid = this.OID;
            Hashtable ht = new Hashtable();
            GEEntity en = md.HisGEEn;

            if (oid == 0)
                oid = minOID;

            en.OID = oid;

            if(en.RetrieveFromDBSources() == 0)
            {
                ht.Add("IsExist", 0);
            }
            else
            {
                ht.Add("IsExist", 1);
            }

            ht.Add("OID", oid);
            ht.Add("UserNo", WebUser.No);
            ht.Add("SID", WebUser.SID);

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }

        #region 从表的选项.
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        public string DtlOpt_Init()
        {
            MapDtl dtl = new MapDtl(this.FK_MapDtl);
            if (DataType.IsNullOrEmpty(dtl.ImpSQLInit))
            {
                return "err@从表加载语句为空，请设置从表加载的sql语句。";
            }
            else
            {
                DataSet ds = new DataSet();
                DataTable dt = DBAccess.RunSQLReturnTable(dtl.ImpSQLInit);

                return BP.Tools.Json.ToJson(dt);
            }
        }
        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        public string DtlOpt_Add()
        {
            MapDtl dtl = new MapDtl(this.FK_MapDtl);
            string pks = this.GetRequestVal("PKs");

            string[] strs = pks.Split(',');
            int i = 0;
            foreach (string str in strs)
            {
                if (str == "CheckAll" || str == null || str == "")
                    continue;

                GEDtl gedtl = new BP.Sys.GEDtl(this.FK_MapDtl);
                string sql = dtl.ImpSQLFullOneRow;
                sql = sql.Replace("@Key", str);

                DataTable dt = DBAccess.RunSQLReturnTable(sql);

                if (dt.Rows.Count == 0)
                    return "err@导入数据失败:" + sql;

                gedtl.Copy(dt.Rows[0]);
                gedtl.RefPK = this.GetRequestVal("RefPKVal");
                gedtl.InsertAsNew();
                i++;
            }

            return "成功的导入了["+i+"]行数据...";
        }
        /// <summary>
        /// 执行查询.
        /// </summary>
        /// <returns></returns>
        public string DtlOpt_Search()
        {
            MapDtl dtl = new MapDtl(this.FK_MapDtl);
            
            string sql = dtl.ImpSQLSearch;
            sql = sql.Replace("@Key", this.GetRequestVal("Key"));
            sql = sql.Replace("@WebUser.No",WebUser.No);
            sql = sql.Replace("@WebUser.Name", WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

            DataSet ds = new DataSet();
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 从表的选项.

        #region 打印.
        public string Print_Init()
        {
            string ApplicationPath = this.context.Request.PhysicalApplicationPath;

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string path = ApplicationPath + @"DataUser\CyclostyleFile\FlowFrm\" + nd.FK_Flow + "\\" + nd.NodeID + "\\";
            string[] fls = null;
            try
            {
                fls = System.IO.Directory.GetFiles(path);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }


            DataTable dt = new DataTable();
            dt.Columns.Add("BillNo");
            dt.Columns.Add("BillName");

            int idx = 0;
            int fileIdx = -1;
            foreach (string f in fls)
            {
                fileIdx++;
                string myfile = f.Replace(path, "");
                string[] strs = myfile.Split('.');
                idx++;

                DataRow dr = dt.NewRow();
                dr["BillNo"] = strs[0];
                dr["BillName"] = strs[1];

                dt.Rows.Add(dr);
            }

            //返回json.
            return BP.Tools.Json.ToJson(dt);
        }
        public string Print_Done()
        {
            int billIdx = this.GetValIntFromFrmByKey("BillIdx");

            string ApplicationPath = "";
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string path = ApplicationPath + "\\DataUser\\CyclostyleFile\\FlowFrm\\" + nd.FK_Flow + "\\" + nd.NodeID + "\\";
            if (System.IO.Directory.Exists(path) == false)
            {
                return "err@模版文件没有找到。" + path;
            }

            string[] fls = System.IO.Directory.GetFiles(path);
            string file = fls[billIdx];
            file = file.Replace(ApplicationPath + @"DataUser\CyclostyleFile", "");

            FileInfo finfo = new FileInfo(file);
            string tempName = finfo.Name.Split('.')[0];
            string tempNameChinese = finfo.Name.Split('.')[1];

            string toPath = ApplicationPath + @"DataUser\Bill\FlowFrm\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (System.IO.Directory.Exists(toPath) == false)
                System.IO.Directory.CreateDirectory(toPath);

            // string billFile = toPath + "\\" + tempName + "." + this.FID + ".doc";
            string billFile = toPath + "\\" +  tempNameChinese + "." + this.WorkID + ".doc";

            BP.Pub.RTFEngine engine = new BP.Pub.RTFEngine();
            if (tempName.ToLower() == "all")
            {
                /* 说明要从所有的独立表单上取数据. */
                FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
                foreach (FrmNode fn in fns)
                {
                    GEEntity ge = new GEEntity(fn.FK_Frm, this.WorkID);
                    engine.AddEn(ge);
                    MapDtls mdtls = new MapDtls(fn.FK_Frm);
                    foreach (MapDtl dtl in mdtls)
                    {
                        GEDtls enDtls = dtl.HisGEDtl.GetNewEntities as GEDtls;
                        enDtls.Retrieve(GEDtlAttr.RefPK, this.WorkID);
                        engine.EnsDataDtls.Add(enDtls);
                    }
                }

                // 增加主表.
                GEEntity myge = new GEEntity("ND" + nd.NodeID, this.WorkID);
                engine.AddEn(myge);

                //增加从表
                MapDtls mymdtls = new MapDtls(tempName);
                foreach (MapDtl dtl in mymdtls)
                {
                    GEDtls enDtls = dtl.HisGEDtl.GetNewEntities as GEDtls;
                    enDtls.Retrieve(GEDtlAttr.RefPK, this.WorkID);
                    engine.EnsDataDtls.Add(enDtls);
                }

                //增加多附件数据
                FrmAttachments aths = new FrmAttachments(tempName);
                foreach (FrmAttachment athDesc in aths)
                {
                    FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
                    if (athDBs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, athDesc.MyPK, FrmAttachmentDBAttr.RefPKVal, this.WorkID, "RDT") == 0)
                        continue;

                    engine.EnsDataAths.Add(athDesc.NoOfObj, athDBs);
                }
                // engine.MakeDoc(file, toPath, tempName + "." + this.WorkID + ".doc", null, false);
                engine.MakeDoc(file, toPath,  tempNameChinese + "." + this.WorkID + ".doc", null, false);
            }
            else
            {
                // 增加主表.
                GEEntity myge = new GEEntity(tempName, this.WorkID);
                engine.HisGEEntity = myge;
                engine.AddEn(myge);

                //增加从表.
                MapDtls mymdtls = new MapDtls(tempName);
                foreach (MapDtl dtl in mymdtls)
                {
                    GEDtls enDtls = dtl.HisGEDtl.GetNewEntities as GEDtls;
                    enDtls.Retrieve(GEDtlAttr.RefPK, this.WorkID);
                    engine.EnsDataDtls.Add(enDtls);
                }

                //增加轨迹表.
                Paras ps = new BP.DA.Paras();
                ps.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                ps.Add(TrackAttr.ActionType, (int)ActionType.WorkCheck);
                ps.Add(TrackAttr.WorkID, this.WorkID);
                engine.dtTrack = BP.DA.DBAccess.RunSQLReturnTable(ps);

                engine.MakeDoc(file, toPath,  tempNameChinese + "." + this.WorkID + ".doc", null, false);
            }

            #region 保存单据，以方便查询.
            Bill bill = new Bill();
            bill.MyPK = this.FID + "_" + this.WorkID + "_" + this.FK_Node + "_" + billIdx;
            bill.WorkID = this.WorkID;
            bill.FK_Node = this.FK_Node;
            bill.FK_Dept = WebUser.FK_Dept;
            bill.FK_Emp = WebUser.No;

            bill.Url = "/DataUser/Bill/FlowFrm/" + DateTime.Now.ToString("yyyyMMdd") + "/" + tempNameChinese + "." + this.WorkID + ".doc";
            bill.FullPath = toPath + file;

            bill.RDT = DataType.CurrentDataTime;
            bill.FK_NY = DataType.CurrentYearMonth;
            bill.FK_Flow = this.FK_Flow;
            if (this.WorkID != 0)
            {
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = this.WorkID;
                if (gwf.RetrieveFromDBSources() == 1)
                {
                    bill.Emps = gwf.Emps;
                    bill.FK_Starter = gwf.Starter;
                    bill.StartDT = gwf.RDT;
                    bill.Title = gwf.Title;
                    bill.FK_Dept = gwf.FK_Dept;
                }
            }

            try
            {
                bill.Insert();
            }
            catch
            {
                bill.Update();
            }
            #endregion

            BillTemplates templates = new BillTemplates();
            int iHave = templates.Retrieve(BillTemplateAttr.NodeID, this.FK_Node, BillTemplateAttr.BillOpenModel, (int)BillOpenModel.WebOffice);
            //在线WebOffice打开
            if (iHave > 0)
            {
                return "url@../WebOffice/PrintOffice.htm?MyPK=" + bill.MyPK;
            }
            else
            {
                BP.Sys.PubClass.OpenWordDocV2(billFile, tempNameChinese + ".doc");
            }

            return "err@功能尚未完成..";
        }
        #endregion 打印.

        #region 附件组件.
        /// <summary>
        /// 执行删除
        /// </summary>
        /// <returns></returns>
        public string AttachmentUpload_Del()
        {
            //执行删除.
            string delPK = this.GetRequestVal("DelPKVal");

            FrmAttachmentDB delDB = new FrmAttachmentDB();
            delDB.MyPK = delPK == null ? this.MyPK : delPK;
            delDB.RetrieveFromDBSources();
            delDB.Delete(); //删除上传的文件.
            return "删除成功.";
        }
        public string AttachmentUpload_DownByStream()
        {
           // return AttachmentUpload_Down(true);
            return AttachmentUpload_Down();
        }
        /// <summary>
        /// 下载
        /// </summary>
        /// <returns></returns>
        public string AttachmentUpload_Down()
        {
            FrmAttachmentDB downDB = new FrmAttachmentDB();
            downDB.MyPK = this.MyPK;
            downDB.Retrieve();

            FrmAttachment dbAtt = new FrmAttachment();
            dbAtt.MyPK = downDB.FK_FrmAttachment;
            dbAtt.Retrieve();

            if (dbAtt.AthSaveWay == AthSaveWay.IISServer)
            {
                //PubClass.DownloadFile(downDB.FileFullName, downDB.FileName);
                return "url@" + DataType.PraseStringToUrlFileName( downDB.FileFullName);
            }

            if (dbAtt.AthSaveWay == AthSaveWay.FTPServer)
            {
                string fileFullName = downDB.GenerTempFile(dbAtt.AthSaveWay);

                // BP.Sys.PubClass.DownloadFileV2(fileFullName, downDB.FileName);
                // return "";
                // PubClass.DownloadFile(downDB.MakeFullFileFromFtp(), downDB.FileName);

                return "url@" + DataType.PraseStringToUrlFileName(fileFullName);
            }

            if (dbAtt.AthSaveWay == AthSaveWay.DB)
            {
                return "fromdb";

                //PubClass.DownloadHttpFile(downDB.FileFullName, downDB.FileName);
            }
            return "正在下载.";
        }

        public void AttachmentDownFromByte()
        {
            FrmAttachmentDB downDB = new FrmAttachmentDB();
            downDB.MyPK = this.MyPK;
            downDB.Retrieve();
            downDB.FileName = HttpUtility.UrlEncode(downDB.FileName);
            byte[] byteList = downDB.GetFileFromDB("FileDB", null);
            if (byteList != null)
            {
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + downDB.FileName);
                HttpContext.Current.Response.ContentType = "application/octet-stream;charset=gb2312";
                HttpContext.Current.Response.BinaryWrite(byteList);
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.Close();
            }
        }
        /// <summary>
        /// 附件ID.
        /// </summary>
        public string FK_FrmAttachment
        {
            get
            {
                return this.GetRequestVal("FK_FrmAttachment");
            }
        }
        public BP.Sys.FrmAttachment GenerAthDescOfFoolTruck()
        {
            FoolTruckNodeFrm sln = new FoolTruckNodeFrm();
            sln.MyPK = this.FK_MapData + "_" + this.FK_Node + "_" + this.FK_Flow;
            if (sln.RetrieveFromDBSources() == 0 || sln.FrmSln==1 || sln.FrmSln==0 )
            {
                /*没有查询到解决方案, 就是只读方案 */
                BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment();
                athDesc.MyPK = this.FK_FrmAttachment;
                athDesc.IsUpload = false;
                athDesc.IsDownload = false;
                athDesc.HisDeleteWay = AthDeleteWay.None; //删除模式.
                return athDesc;
            }

            //如果是自定义方案,就查询自定义方案信息.
            if (sln.FrmSln == 2)
            {
                /*没有查询到解决方案, 就是只读方案 */
                BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment();
                athDesc.MyPK = this.FK_Node+"_Ath1";
                if (athDesc.RetrieveFromDBSources() == 0)
                {
                    athDesc.IsUpload = false;
                    athDesc.HisDeleteWay = AthDeleteWay.None;
                    athDesc.IsDownload = false;
                    athDesc.HisCtrlWay = AthCtrlWay.WorkID; //没有方案.
                    athDesc.Insert();
                }
                return athDesc;
            }
           
            return null;
        }
        /// <summary>
        /// 生成描述
        /// </summary>
        /// <returns></returns>
        public BP.Sys.FrmAttachment GenerAthDesc()
        {
            #region 为累加表单做的特殊判断.
            if (this.GetRequestValInt("FormType") == 10)
            {
                if (this.FK_FrmAttachment.Contains(this.FK_MapData) == false)
                    return GenerAthDescOfFoolTruck();
            }
            #endregion


            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment();
            athDesc.MyPK = this.FK_FrmAttachment;
            if (this.FK_Node == 0 || this.FK_Flow == null)
            {
                athDesc.RetrieveFromDBSources();
                return athDesc;
            }

            athDesc.MyPK = this.FK_FrmAttachment;
            int result = athDesc.RetrieveFromDBSources();

            #region 判断是否是明细表的多附件.
            if (result == 0 && DataType.IsNullOrEmpty(this.FK_Flow) == false
               && this.FK_FrmAttachment.Contains("AthMDtl"))
            {
                athDesc.FK_MapData = this.FK_MapData;
                athDesc.NoOfObj = "AthMDtl";
                athDesc.Name = "我的从表附件";
                athDesc.UploadType = AttachmentUploadType.Multi;
                athDesc.Insert();
            }
            #endregion 判断是否是明细表的多附件。

            #region 判断是否可以查询出来，如果查询不出来，就可能是公文流程。
            if (result == 0 && DataType.IsNullOrEmpty(this.FK_Flow) == false
                && this.FK_FrmAttachment.Contains("DocMultiAth"))
            {
                /*如果没有查询到它,就有可能是公文多附件被删除了.*/
                athDesc.MyPK = this.FK_FrmAttachment;
                athDesc.NoOfObj = "DocMultiAth";
                athDesc.FK_MapData = this.FK_MapData;
                athDesc.Exts = "*.*";

                //存储路径.
                athDesc.SaveTo = "/DataUser/UploadFile/";
                athDesc.IsNote = false; //不显示note字段.
                athDesc.IsVisable = false; // 让其在form 上不可见.

                //位置.
                athDesc.X = (float)94.09;
                athDesc.Y = (float)333.18;
                athDesc.W = (float)626.36;
                athDesc.H = (float)150;

                //多附件.
                athDesc.UploadType = AttachmentUploadType.Multi;
                athDesc.Name = "公文多附件(系统自动增加)";
                athDesc.SetValByKey("AtPara",
                    "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=@IsWoEnableTemplete=1");
                athDesc.Insert();

                //有可能在其其它的节点上没有这个附件，所以也要循环增加上它.
                BP.WF.Nodes nds = new BP.WF.Nodes(this.FK_Flow);
                foreach (BP.WF.Node nd in nds)
                {
                    athDesc.FK_MapData = "ND" + nd.NodeID;
                    athDesc.MyPK = athDesc.FK_MapData + "_" + athDesc.NoOfObj;
                    if (athDesc.IsExits == true)
                        continue;

                    athDesc.Insert();
                }

                //重新查询一次，把默认值加上.
                athDesc.RetrieveFromDBSources();
            }
            #endregion 判断是否可以查询出来，如果查询不出来，就可能是公文流程。

            #region 处理权限方案。
            /*首先判断是否具有权限方案*/
            string at = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new BP.DA.Paras();
            ps.SQL = "SELECT FrmSln FROM WF_FrmNode WHERE FK_Node=" + at + "FK_Node AND FK_Flow=" + at + "FK_Flow AND FK_Frm=" + at + "FK_Frm";
            ps.Add("FK_Node", this.FK_Node);
            ps.Add("FK_Flow", this.FK_Flow);
            ps.Add("FK_Frm", this.FK_MapData);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count == 0)
            {
                athDesc.RetrieveFromDBSources();
            }
            else
            {
                int sln = int.Parse(dt.Rows[0][0].ToString());
                if (sln == 0)
                {
                    athDesc.RetrieveFromDBSources();
                }
                else
                {
                    result = athDesc.Retrieve(FrmAttachmentAttr.FK_MapData, this.FK_MapData,
                        FrmAttachmentAttr.FK_Node, this.FK_Node, FrmAttachmentAttr.NoOfObj, this.Ath);

                    if (result == 0) /*如果没有定义，就获取默认的.*/
                        athDesc.RetrieveFromDBSources();
                    //  throw new Exception("@该独立表单在该节点("+this.FK_Node+")使用的是自定义的权限控制，但是没有定义该附件的权限。");
                }
            }
            #endregion 处理权限方案。

            return athDesc;
        }
        /// <summary>
        /// 打包下载.
        /// </summary>
        /// <returns></returns>
        public string AttachmentUpload_DownZip()
        {
            string zipName = this.WorkID + "_" + this.FK_FrmAttachment;

            #region 处理权限控制.
            BP.Sys.FrmAttachment athDesc = this.GenerAthDesc();

            //查询出来数据实体.
            BP.Sys.FrmAttachmentDBs dbs = BP.WF.Glo.GenerFrmAttachmentDBs(athDesc, this.PKVal, this.FK_FrmAttachment);
            #endregion 处理权限控制.

            if (dbs.Count == 0)
                return "err@文件不存在，不需打包下载。";

            string basePath = SystemConfig.PathOfDataUser + "Temp";
            string tempUserPath = basePath + "\\" + WebUser.No;
            string tempFilePath = basePath + "\\" + WebUser.No+"\\"+this.OID;
            string zipPath = basePath + "\\" + WebUser.No;
            string zipFile = zipPath + "\\" + zipName + ".zip";

            string info = "";
            try
            {
                //删除临时文件，保证一个用户只能存一份，减少磁盘占用空间.
                info = "@创建用户临时目录:" + tempUserPath;
                if (System.IO.Directory.Exists(tempUserPath) == false)
                    System.IO.Directory.CreateDirectory(tempUserPath);

                //如果有这个临时的目录就把他删除掉.
                if (System.IO.Directory.Exists(tempFilePath) == true)
                    System.IO.Directory.Delete(tempFilePath,true);

                System.IO.Directory.CreateDirectory(tempFilePath);
            }
            catch (Exception ex)
            {
                return "err@组织临时目录出现错误:" + ex.Message;
            }

            try
            {
                foreach (FrmAttachmentDB db in dbs)
                {
                    string copyToPath = tempFilePath;

                    //求出文件路径.
                    string fileTempPath = db.GenerTempFile(athDesc.AthSaveWay);
                    if (DataType.IsNullOrEmpty(db.Sort) == false)
                    {
                        copyToPath = tempFilePath + "//" + db.Sort;
                        if (System.IO.Directory.Exists(copyToPath) == false)
                            System.IO.Directory.CreateDirectory(copyToPath);
                    }
                    //新文件目录
                    copyToPath = copyToPath + "//" + db.FileName;

                    if (File.Exists(fileTempPath) == true)
                    {
                        File.Copy(fileTempPath, copyToPath, true);
                    }
                }
            }
            catch (Exception ex)
            {
                return "err@组织文件期间出现错误:" + ex.Message;
            }

            try
            {
                while (System.IO.File.Exists(zipFile) == true)
                {
                    System.IO.File.Delete(zipFile);
                }
                //执行压缩.
                FastZip fz = new FastZip();
                fz.CreateZip(zipFile, tempFilePath, true, "");
                //删除临时文件夹
                System.IO.Directory.Delete(tempFilePath, true);
            }
            catch (Exception ex)
            {
                return "err@执行压缩出现错误:" + ex.Message + ",路径tempPath:" + tempFilePath + ",zipFile=" + zipFile;
            }

            if (System.IO.File.Exists(zipFile) == false)
                return "err@压缩文件未生成成功,请在点击一次.";

            zipName = DataType.PraseStringToUrlFileName(zipName);

            string url = HttpContext.Current.Request.ApplicationPath + "DataUser/Temp/" + WebUser.No + "/" + zipName + ".zip";
            return "url@" + url;

        }
        public string DearFileName(string fileName)
        {
            fileName= DearFileNameExt(fileName,"+","%2B");
            fileName= DearFileNameExt(fileName," ","%20");
            fileName= DearFileNameExt(fileName,"/","%2F");
            fileName= DearFileNameExt(fileName,"?","%3F");
            fileName= DearFileNameExt(fileName,"%","%25");
            fileName= DearFileNameExt(fileName,"#","%23");
            fileName= DearFileNameExt(fileName,"&","%26");
            fileName= DearFileNameExt(fileName,"=","%3D");
            return fileName;
        }
        public string DearFileNameExt(string fileName, string val, string replVal)
        {
            fileName=fileName.Replace(val,replVal);
            fileName=fileName.Replace(val,replVal);
            fileName=fileName.Replace(val,replVal);
            fileName=fileName.Replace(val,replVal);
            fileName=fileName.Replace(val,replVal);
            fileName=fileName.Replace(val,replVal);
            fileName=fileName.Replace(val,replVal);
            fileName=fileName.Replace(val,replVal);
            return fileName;
        }
        #endregion 附件组件


        #region 必须传递参数
        /// <summary>
        /// 执行的内容
        /// </summary>
        public string DoWhat
        {
            get
            {
                string str = this.GetRequestVal("DoWhat");
                if (DataType.IsNullOrEmpty(str))
                    return "Frm";
                return str;
            }
        }
        /// <summary>
        /// 当前的用户
        /// </summary>
        public string UserNo
        {
            get
            {
                return this.GetRequestVal("UserNo");
            }
        }
        /// <summary>
        /// 用户的安全校验码(请参考集成章节)
        /// </summary>
        public string SID
        {
            get
            {
                return this.GetRequestVal("SID");
            }
        }
        public string AppPath
        {
            get
            {
                return BP.WF.Glo.CCFlowAppPath;
            }
        }
        public string Port_Init()
        {
            #region 安全性校验.
            if (this.UserNo == null || this.SID == null || this.DoWhat == null || this.FrmID == null)
            {
                return "err@必要的参数没有传入，请参考接口规则。";
            }

            if (BP.WF.Dev2Interface.Port_CheckUserLogin(this.UserNo, this.SID) == false)
            {
                return "err@非法的访问，请与管理员联系。SID=" + this.SID;
            }

            if (BP.Web.WebUser.No != this.UserNo)
            {
                BP.WF.Dev2Interface.Port_SigOut();
                BP.WF.Dev2Interface.Port_Login(this.UserNo);
            }
            if (this.GetValIntFromFrmByKey("IsMobile") == 1)
                BP.Web.WebUser.UserWorkDev = UserWorkDev.Mobile;
            else
                BP.Web.WebUser.UserWorkDev = UserWorkDev.PC;
            #endregion 安全性校验.

            #region 生成参数串.
            string paras = "";
            foreach (string str in this.context.Request.QueryString)
            {
                string val = this.GetRequestVal(str);
                if (val.IndexOf('@') != -1)
                    throw new Exception("您没有能参数: [ " + str + " ," + val + " ] 给值 ，URL 将不能被执行。");
                switch (str.ToLower())
                {
                    case "fk_mapdata":
                    case "workid":
                    case "fk_node":
                    case "sid":
                        break;
                    default:
                        paras += "&" + str + "=" + val;
                        break;
                }
            }
            #endregion 生成参数串.

            string url = "";
            switch (this.DoWhat)
            {
                case "Frm": //如果是调用Frm的查看界面.
                    url = "Frm.htm?FK_MapData=" + this.FrmID + "&OID=" + this.OID + paras;
                    break;
                case "Search": //调用查询界面.
                    url = "../Comm/Search.htm?EnsName=" + this.FrmID + paras;
                    break;
                case "Group": //分组分析界面.
                    url = "../Comm/Group.htm?EnsName=" + this.FrmID + paras;
                    break;
                default:
                    break;
            }
            return "url@" + url;
        }
        #endregion
     



    }
}
