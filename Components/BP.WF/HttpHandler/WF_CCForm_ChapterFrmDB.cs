using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Difference;
using BP.Sys;
using BP.Web;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCForm_ChapterFrmDB : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCForm_ChapterFrmDB()
        {

        }

        public string ChapterFrmDB_Init()
        {
            DataSet ds = new DataSet();
            var en = new GEEntity(this.FrmID);
            en.OID = this.OID;
            if (en.RetrieveFromDBSources() == 0)
                en.InsertAsOID(this.OID);
            MapData md = new MapData(this.FrmID);
            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));
            //获取分组信息
            GroupFields gfs = new GroupFields();
            gfs.Retrieve(GroupFieldAttr.FrmID, this.FrmID, "Idx");

            MapDtls dtls = md.MapDtls;
            //获取字段信息
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.MyDataType, 1, MapAttrAttr.UIVisible, 1, "Idx");

            int mainVer = this.GetRequestValInt("MainVer");
            int compareVer = this.GetRequestValInt("CompareVer");
            if (mainVer == 0 || compareVer == 0)
                return "err@比对数据库的版本不正确:比对版本1=Ver" + mainVer + ".0 比对版本2=Ver" + compareVer + ".0";

            //获取主版本的所有信息
            FrmDBVers mainVers = new FrmDBVers();
            mainVers.Retrieve("FrmID", this.FrmID, "RefPKVal", this.OID, "Ver", mainVer);

            FrmDBVers compareVers = new FrmDBVers();
            compareVers.Retrieve("FrmID", this.FrmID, "RefPKVal", this.OID, "Ver", compareVer);

            MapAttrs newAttrs = new MapAttrs();
            //获取历史版本数据
            string mainStr = "";
            string compareStr = "";
            foreach(MapAttr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                mainStr = GetBigTextByKeyOfEn(attr.KeyOfEn, mainVers);
                compareStr = GetBigTextByKeyOfEn(attr.KeyOfEn, compareVers);
                if (mainStr.Equals(compareStr) == false)
                {
                    attr.SetPara("MainStr", mainStr);
                    attr.SetPara("CompareStr", compareStr);
                    newAttrs.AddEntity(attr);
                    continue;
                }
            }
            ds.Tables.Add(newAttrs.ToDataTableField("MapAttrs"));
            ds.Tables.Add(mainVers.ToDataTableField("MainVers"));
            ds.Tables.Add(compareVers.ToDataTableField("CompareVers"));

            //获取表单的主版本版本号
            string mainVerPK = "";
            foreach(FrmDBVer ver in mainVers)
            {
                if (DataType.IsNullOrEmpty(ver.KeyOfEn) == true)
                {
                    mainVerPK = ver.MyPK;
                    break;
                }
            }
            string compareVerPK = "";
            foreach (FrmDBVer ver in compareVers)
            {
                if (DataType.IsNullOrEmpty(ver.KeyOfEn) == true)
                {
                    compareVerPK = ver.MyPK;
                    break;
                }
            }

            //获取从表的数据
            mainStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", mainVerPK, "FrmDtlDB");
            compareStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", compareVerPK, "FrmDtlDB");
            gfs = GetNewGFS(gfs, mainStr, compareStr, "Dtl", dtls);

            //获取附件的数据
            mainStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", mainVerPK, "FrmAthDB");
            compareStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", compareVerPK, "FrmAthDB");
            gfs = GetNewGFS(gfs, mainStr, compareStr, "Ath");

            GroupFields ngfs = new GroupFields();
            ngfs.AddEntities(gfs);
            //获取ChartFrmLink的对应表单数据
            foreach (GroupField gf in gfs)
           {
                if (gf.CtrlType.Equals("ChapterFrmLinkFrm") == true)
                {
                    mainStr = "";
                    compareStr = "";
                    string sql = "SELECT MyPK,Ver FROM Sys_FrmDBVer WHERE FrmID=" + SystemConfig.AppCenterDBVarStr + "FrmID AND RefPKVal=" + SystemConfig.AppCenterDBVarStr + "RefPKVal AND Ver IN(" + mainVer + "," + compareVer + ")";
                    Paras ps = new Paras();
                    ps.SQL = sql;
                    ps.Add("FrmID", gf.CtrlID);
                    ps.Add("RefPKVal", this.OID);
                    DataTable dt = DBAccess.RunSQLReturnTable(ps);
                    foreach (DataRow dr in dt.Rows)
                    {
                        int ver = int.Parse(dr[1].ToString());
                        if (ver == mainVer)
                            mainStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", dr[0].ToString(), "FrmDB");
                        if (ver == compareVer)
                            compareStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", dr[0].ToString(), "FrmDB");
                    }
                    if (mainStr.Equals(compareStr) == true)
                        ngfs.RemoveEn(gf);
                }
                continue;
           }
           ds.Tables.Add(ngfs.ToDataTableField("GroupFields"));
            return BP.Tools.Json.ToJson(ds);
        }
        private GroupFields GetNewGFS(GroupFields gfs, string mainStr, string compareStr, string ctrlType, MapDtls dtls = null)
        {
            //主版本和比对版本都不存在数据
            if (DataType.IsNullOrEmpty(mainStr)==true && DataType.IsNullOrEmpty(compareStr) == true)
            {
                GroupFields ngfs = new GroupFields();
                ngfs.AddEntities(gfs);
                foreach (GroupField gf in gfs)
                {
                    if (gf.CtrlType.Equals(ctrlType) == true)
                        ngfs.RemoveEn(gf);
                }
                return ngfs;
            }
            //两个比对版本其中一个为空时
            if ((DataType.IsNullOrEmpty(mainStr) == true && DataType.IsNullOrEmpty(compareStr) == false)
                || (DataType.IsNullOrEmpty(mainStr) == false && DataType.IsNullOrEmpty(compareStr) == true))
                return gfs;

            //两个版本的数据都不为空时,转成DataSet
            DataSet mds = BP.Tools.Json.ToDataSet(mainStr);
            DataSet cds = BP.Tools.Json.ToDataSet(compareStr);
            DataTable cdt = null;
            foreach(DataTable mdt in mds.Tables)
            {
                cdt = cds.Tables[mdt.TableName];
                if (cdt != null)
                {
                    //比对table中的内容
                    bool flag = CompareDataTable(mdt, cdt);
                    if (flag == true)
                    {
                        if (ctrlType.Equals("Dtl") == true)
                        {
                            foreach(MapDtl dtl in dtls)
                            {
                                if(dtl.PTable.Equals(mdt.TableName)==true)
                                    gfs = RemoveByCtrlID(gfs, dtl.No);
                            }
                        }
                        if (ctrlType.Equals("Ath") == true)
                            gfs = RemoveByCtrlID(gfs, this.FrmID + "_" + mdt.TableName);
                    }   
                }
               
            }
            return gfs;
        }
        /// <summary>
        /// 比对两个DataTable在数据结构相同下数据是否相同
        /// </summary>
        /// <param name="dtA"></param>
        /// <param name="dtB"></param>
        /// <returns></returns>
        private bool CompareDataTable(DataTable dtA, DataTable dtB)
        {
            if (dtA.Rows.Count == dtB.Rows.Count)
            {
                //比内容
                for (int i = 0; i < dtA.Rows.Count; i++)
                {
                    for (int j = 0; j < dtA.Columns.Count; j++)
                    {
                        if (dtA.Rows[i][j].Equals(dtB.Rows[i][j])==false)
                        {
                            return false;
                        }
                    }
                }
                return true;
               
            }
            else
            {
                return false;
            }
        }

        private GroupFields RemoveByCtrlID(GroupFields gfs,string ctrlID)
        {
            foreach(GroupField gf in gfs)
            {
                if (gf.CtrlID.Equals(ctrlID) == true)
                {
                    gfs.RemoveEn(gf);
                    return gfs;
                }
            }
            return gfs;
        }
        public string ChapterFrmDB_DtlInit()
        {
            string dtlNo = GetRequestVal("DtlNo");
            
            DataSet myds = new DataSet();
            MapDtl dtl = new MapDtl(dtlNo);
            DataTable Sys_MapDtl = dtl.ToDataTableField("Sys_MapDtl");
            myds.Tables.Add(Sys_MapDtl);

            //明细表的表单描述
            MapAttrs attrs = dtl.MapAttrs;
            DataTable Sys_MapAttr = attrs.ToDataTableField("Sys_MapAttr");
            myds.Tables.Add(Sys_MapAttr);

            //明细表的配置信息.
            //DataTable Sys_MapExt = dtl.MapExts.ToDataTableField("Sys_MapExt");
           // myds.Tables.Add(Sys_MapExt);

            //启用附件，增加附件信息
            DataTable Sys_FrmAttachment = dtl.FrmAttachments.ToDataTableField("Sys_FrmAttachment");
            myds.Tables.Add(Sys_FrmAttachment);
            return BP.Tools.Json.ToJson(myds);
        }

        public string ChapterFrmDB_Dtl()
        {
            string mainVerPK = this.GetRequestVal("MainVerPK");
            string compareVerPK = this.GetRequestVal("CompareVerPK");
            if (DataType.IsNullOrEmpty(mainVerPK) == true || DataType.IsNullOrEmpty(compareVerPK) == true)
                return "err@获取存储版本的主键值失败,请联系管理员";
            //获取主版本的附件信息
            string mainStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", mainVerPK, "FrmDtlDB");
            string compareStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", compareVerPK, "FrmDtlDB");
            DataTable dt = new DataTable();
            dt.Columns.Add("MainDtls");
            dt.Columns.Add("CompareDtls");
            DataRow dr = dt.NewRow();
            dr[0] = mainStr;
            dr[1] = compareStr;
            dt.Rows.Add(dr);
            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 获取附件的版本比对信息
        /// </summary>
        /// <returns></returns>
        public string ChapterFrmDB_Ath()
        {
            string mainVerPK = this.GetRequestVal("MainVerPK");
            string compareVerPK = this.GetRequestVal("CompareVerPK");
            if (DataType.IsNullOrEmpty(mainVerPK) == true || DataType.IsNullOrEmpty(compareVerPK) == true)
               return "err@获取存储版本的主键值失败,请联系管理员";
            //获取主版本的附件信息
            string mainStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", mainVerPK, "FrmAthDB");
            string compareStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", compareVerPK, "FrmAthDB");
            DataTable dt = new DataTable();
            dt.Columns.Add("MainAths");
            dt.Columns.Add("CompareAths");
            DataRow dr = dt.NewRow();
            dr[0] = mainStr;
            dr[1] = compareStr;
            dt.Rows.Add(dr);
            return BP.Tools.Json.ToJson(dt);
        }

        public string ChapterFrmDB_FrmGener()
        {
            int mainVer = this.GetRequestValInt("MainVer");
            int compareVer = this.GetRequestValInt("CompareVer");
            if (mainVer==0 || compareVer==0)
                return "err@获取存储版本的主键值失败,请联系管理员";
            MapData md = new MapData(this.FrmID);
            //主表实体.
            GEEntity en = new GEEntity(this.FrmID);
            en.OID = this.OID;
            if (en.RetrieveFromDBSources() == 0)
                return "表单" + md.Name + "的OID=" + this.OID + "的数据不存在";
            string frmID = md.No;
            //根据表单存储的数据获取获取使用表单的版本号
            int frmVer = 0;
            if (en.EnMap.Attrs.Contains("AtPara") == true)
            {
                frmVer = en.GetParaInt("FrmVer");
                if (frmVer != 0 && frmVer != md.Ver2022)
                    frmID = md.No + "." + frmVer;
            }

            DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet(frmID);
            //现在版本不是主版本的情况
            if (frmID.Equals(this.FK_MapData) == false)
            {
                DataTable mddt = ds.Tables["Sys_MapData"];
                mddt.Rows[0]["AtPara"] = mddt.Rows[0]["AtPara"] + "@MainFrmID=" + this.FK_MapData;
                //如果是傻瓜表单
                if (md.HisFrmType == FrmType.FoolForm)
                {
                    DataTable athdt = ds.Tables["Sys_FrmAttachment"];
                    if (frmVer != 0 && athdt.Rows.Count != 0)
                    {
                        DataTable gfdt = ds.Tables["Sys_GroupField"];
                        foreach (DataRow dr in athdt.Rows)
                        {
                            DataRow[] gfr = gfdt.Select("CtrlID='" + dr["MyPK"] + "'");
                            if (gfr.Length != 0)
                                gfr[0]["CtrlID"] = md.No + "_" + dr["NoOfObj"];
                            dr["MyPK"] = md.No + "_" + dr["NoOfObj"];

                        }
                    }
                }
            }

            //获取主版本的表单信息
            string dbstr = SystemConfig.AppCenterDBVarStr;
            string sql = "SELECT MyPK,Ver FROM Sys_FrmDBVer WHERE FrmID=" + dbstr + "FrmID AND RefPKVal=" + dbstr + "RefPKVal AND Ver IN("+mainVer+","+compareVer+")";
            Paras ps = new Paras();
            ps.SQL = sql;
            ps.Add("FrmID", this.FrmID);
            ps.Add("RefPKVal", this.OID);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            string mainStr = "";
            string compareStr = "";
            foreach (DataRow dr in dt.Rows)
            { 
                int ver = int.Parse(dr[1].ToString());
                if(ver==mainVer)
                    mainStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", dr[0].ToString(), "FrmDB");
                if (ver == compareVer)
                    compareStr = DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", dr[0].ToString(), "FrmDB");
            }
            dt = new DataTable();
            dt.TableName = "MainData";
            dt.Columns.Add("Ver");
            dt.Columns.Add("Data");
            DataRow drr = dt.NewRow();
            drr[0] = mainVer;
            drr[1] = mainStr;
            dt.Rows.Add(drr);
            drr = dt.NewRow();
            drr[0] = compareVer;
            drr[1] = compareStr;
            dt.Rows.Add(drr);
            ds.Tables.Add(dt);
            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 比对富文本的内容
        /// </summary>
        /// <param name="keyOfEn"></param>
        /// <param name="vers"></param>
        /// <returns></returns>
        private string GetBigTextByKeyOfEn(string keyOfEn,FrmDBVers vers)
        {
            foreach(FrmDBVer ver in vers)
            {
                if (DataType.IsNullOrEmpty(ver.KeyOfEn) == true)
                    continue;
                if (ver.KeyOfEn.Equals(keyOfEn) == true)
                {
                    return DBAccess.GetBigTextFromDB("Sys_FrmDBVer", "MyPK", ver.MyPK, "FrmDB");
                }
            }
            return "";
        }
    }
}
