using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web.Controls;
using BP.WF.Template;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Sys;
using BP.Web;

namespace CCFlow.WF.CCForm
{
    public partial class FrmWord : BP.Web.WebPage
    {
        #region 属性
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Node
        {
            get
            {
                try
                {
                    string nodeid = this.Request.QueryString["NodeID"];
                    if (nodeid == null)
                        nodeid = this.Request.QueryString["FK_Node"];
                    return int.Parse(nodeid);
                }
                catch
                {
                    if (DataType.IsNullOrEmpty(this.FK_Flow))
                        return 0;
                    else
                        return int.Parse(this.FK_Flow); // 0; 有可能是流程调用独立表单。
                }
            }
        }
        public string WorkID
        {
            get
            {
                return this.Request.QueryString["WorkID"];
            }
        }
        public int OID
        {
            get
            {
                string cworkid = this.Request.QueryString["CWorkID"];
                if (DataType.IsNullOrEmpty(cworkid) == false && int.Parse(cworkid) != 0)
                    return int.Parse(cworkid);

                string oid = this.Request.QueryString["WorkID"];
                if (oid == null || oid == "")
                    oid = this.Request.QueryString["OID"];
                if (oid == null || oid == "")
                    oid = "0";
                return int.Parse(oid);
            }
        }
        /// <summary>
        /// 延续流程ID
        /// </summary>
        public int CWorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["CWorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 父流程ID
        /// </summary>
        public int PWorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["PWorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public int FID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int OIDPKVal
        {
            get
            {

                if (ViewState["OIDPKVal"] == null)
                    return 0;
                return int.Parse(ViewState["OIDPKVal"].ToString());
            }
            set
            {
                ViewState["OIDPKVal"] = value;
            }
        }

        public string FK_MapData
        {
            get
            {
                string s = this.Request.QueryString["FK_MapData"];
                if (s == null)
                    return "ND101";
                return s;
            }
        }

        public bool IsEdit
        {
            get
            {
                if (this.Request.QueryString["IsEdit"] == "0")
                    return false;
                return true;
            }
        }

        public string SID
        {
            get
            {
                return this.Request.QueryString["PWorkID"];
            }
        }

        public bool IsPrint
        {
            get
            {
                if (this.Request.QueryString["IsPrint"] == "1")
                    return true;
                return false;
            }
        }

        public string NodeInfo
        {
            get
            {
                BP.WF.Node nodeInfo = new BP.WF.Node(FK_Node);

                return nodeInfo.Name + ": " + WebUser.Name + "    时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public bool IsMarks { get; set; }

        public bool ReadOnly { get; set; }

        public string UserName { get; set; }

        public bool IsCheck { get; set; }

        /// <summary>
        /// 是否是第一次打开Word表单
        /// </summary>
        public bool IsFirst { get; set; }

        /// <summary>
        /// 填充的主表JSON数据，为含有key,value的数组
        /// </summary>
        public string ReplaceParams { get; set; }

        /// <summary>
        /// 填充的主表属性名组织的JSON数据,为String数组
        /// </summary>
        public string ReplaceFields { get; set; }

        /// <summary>
        /// 填充的明细表数据JSON数据，为对象数组
        /// </summary>
        public string ReplaceDtls { get; set; }

        /// <summary>
        /// 填充的明细表编号JSON数据，为String数组
        /// </summary>
        public string ReplaceDtlNos { get; set; }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            UserName = WebUser.Name;
            if (DataType.IsNullOrEmpty(FK_MapData))
            {
                divMenu.InnerHtml = "<h1 style='color:red'>传入参数错误!<h1>";
                return;
            }

            var md = new MapData(this.FK_MapData);

            string type = Request["action"];
            if (DataType.IsNullOrEmpty(type))
            {
                /*这里是什么？怎么没有注释？*/
                InitOffice(true, md);
            }
            else
            {
                if (type.Equals("LoadFile"))
                {
                    LoadFile();
                    return;
                }

                if (type.Equals("SaveFile"))
                {
                    SaveFile();
                    SaveFieldInfos();
                    return;
                }

                InitOffice(false, md);
            }

            //打开数据文件.
            //GEEntityWordFrm en = new GEEntityWordFrm(this.FK_MapData, this.OID);
            GEEntityWordFrm en = new GEEntityWordFrm();
            en.FK_MapData = this.FK_MapData;
            en.OID = this.OID;

            var root = SystemConfig.PathOfDataUser + "\\FrmOfficeTemplate\\";
            var rootInfo = new DirectoryInfo(root);

            if (!rootInfo.Exists)
                rootInfo.Create();

            var files = rootInfo.GetFiles(en.FK_MapData + ".*");
            FileInfo tmpFile = null;
            FileInfo wordFile = null;

            var pathDir = SystemConfig.PathOfDataUser + @"\FrmOfficeFiles\" + this.FK_MapData;

            if (!Directory.Exists(pathDir))
                Directory.CreateDirectory(pathDir);

            // 判断是否有这个数据文件.
            if (files.Length == 0)
            {
                Response.Write("<h3>Word表单模板文件不存在，请确认已经上传Word表单模板</h3>");
                Response.End();
                return;
            }
            tmpFile = files[0];
            wordFile = new FileInfo(pathDir + "\\" + this.OID + tmpFile.Extension);
            if (wordFile.Exists == false)
            {
                IsFirst = true;
                File.Copy(tmpFile.FullName, wordFile.FullName);
            }
            else
            {
                IsFirst = false;
            }

            if (en.RetrieveFromDBSources() == 0)
            {
                en.FilePath = wordFile.FullName;
                en.RDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                en.LastEditer = WebUser.Name;
                en.Insert(); // 执行插入。
            }

            en.ResetDefaultVal();

            //接受外部参数数据。
            string[] paras = this.RequestParas.Split('&');
            foreach (string str in paras)
            {
                if (DataType.IsNullOrEmpty(str) || str.Contains("=") == false)
                    continue;

                string[] kvs = str.Split('=');
                en.SetValByKey(kvs[0], kvs[1]);
            }

            //装载数据.
            this.LoadFrmData(new MapAttrs(this.FK_MapData), en);

            //替换掉 word 里面的数据.
            fileName.Text = string.Format(@"\{0}\{1}{2}", en.FK_MapData, this.OID, wordFile.Extension);
            fileType.Text = wordFile.Extension.TrimStart('.');
        }
        /// <summary>
        /// 保存从word中提取的数据
        /// </summary>
        private void SaveFieldInfos()
        {
            var mes = new MapExts(this.FK_MapData);
            if (mes.Count == 0) return;

            var item = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull) as MapExt;
            if (item == null) return;

            var fieldCount = 0;
            foreach (var key in Request.Form.AllKeys)
            {
                var idx = 0;
                if (key.StartsWith("field") && key.Length > 5 && int.TryParse(key.Substring(5), out idx))
                {
                    fieldCount++;
                }
            }

            var fieldsJson = string.Empty;
            for (var i = 0; i < fieldCount; i++)
            {
                fieldsJson += Request["field" + i];
            }

            var fields = LitJson.JsonMapper.ToObject<List<ReplaceField>>(fieldsJson);

            //更新主表数据
            var en = new GEEntityWordFrm(this.FK_MapData);
            en.OID = this.OID;

            if (en.RetrieveFromDBSources() == 0)
            {
                throw new Exception("OID=" + this.OID + "的数据在" + this.FK_MapData + "中不存在，请检查！");
            }

            //此处因为weboffice在上传的接口中，只有上传成功与失败的返回值，没有具体的返回信息参数，所以未做异常处理
            foreach (var field in fields)
            {
                en.SetValByKey(field.key, field.value);
            }

            en.LastEditer = WebUser.Name;
            en.RDT = DataType.CurrentDataTime;
            en.Update();

            //todo:更新明细表数据，此处逻辑可能还有待商榷
            var mdtls = new MapDtls(this.FK_MapData);
            if (mdtls.Count == 0) return;

            var dtlsCount = 0;
            foreach (var key in Request.Form.AllKeys)
            {
                var idx = 0;
                if (key.StartsWith("dtls") && key.Length > 4 && int.TryParse(key.Substring(4), out idx))
                {
                    dtlsCount++;
                }
            }

            var dtlsJson = string.Empty;
            for (var i = 0; i < dtlsCount; i++)
            {
                dtlsJson += Request["dtls" + i];
            }

            //var dtlsJson = Request["dtls"];
            var dtls = LitJson.JsonMapper.ToObject<List<ReplaceDtlTable>>(dtlsJson);
            GEDtls gedtls = null;
            GEDtl gedtl = null;
            ReplaceDtlTable wdtl = null;

            foreach (MapDtlExt mdtl in mdtls)
            {
                wdtl = dtls.FirstOrDefault(o => o.dtlno == mdtl.No);

                if (wdtl == null || wdtl.dtl.Count == 0) continue;

                //此处不是真正意义上的更新，因为不知道明细表的主键，只能将原明细表中的数据删除掉，然后再重新插入新的数据
                gedtls = new GEDtls(mdtl.No);
                gedtls.Delete(GEDtlAttr.RefPK, en.PKVal);

                foreach (var d in wdtl.dtl)
                {
                    gedtl = gedtls.GetNewEntity as GEDtl;

                    foreach (var cell in d.cells)
                    {
                        gedtl.SetValByKey(cell.key, cell.value);
                    }

                    gedtl.RefPK = en.PKVal.ToString();
                    gedtl.RDT = DataType.CurrentDataTime;
                    gedtl.Rec = WebUser.No;
                    gedtl.Insert();
                }
            }
        }

        private void InitOffice(bool isMenu, MapData md)
        {
            bool isCompleate = false;

            if (WorkID == "0")
            {
                isCompleate = false;
            }
            else
            {
                isCompleate = false;  
            }

            if (!isCompleate)
            {
                if (md.IsWoEnableReadonly)
                {
                    ReadOnly = true;
                }

                if (md.IsWoEnableCheck)
                {
                    IsCheck = true;
                }

                IsMarks = md.IsWoEnableMarks;
            }
            else
            {
                ReadOnly = true;
            }

            if (isMenu && isCompleate==false)
            {
                #region 初始化按钮
                if (md.IsWoEnableViewKeepMark)
                {
                    divMenu.InnerHtml = "<select id='marks' onchange='ShowUserName()' style='width: 100px'><option value='1'>全部</option><select>";
                }

                if (md.IsWoEnableTemplete)
                {
                    AddBtn(NamesOfBtn.Open, "打开模板", "OpenTempLate");
                }
                if (md.IsWoEnableSave)
                {
                    AddBtn(NamesOfBtn.Save, "保存", "saveOffice");
                }
                if (md.IsWoEnableRevise)
                {
                    AddBtn(NamesOfBtn.Accept, "接受修订", "acceptOffice");
                    AddBtn(NamesOfBtn.Refuse, "拒绝修订", "refuseOffice");
                }

                if (md.IsWoEnableOver)
                {
                    AddBtn("over", "套红文件", "overOffice");
                }

                if (md.IsWoEnablePrint)
                {
                    AddBtn(NamesOfBtn.Print, "打印", "printOffice");
                }
                if (md.IsWoEnableSeal)
                {
                    AddBtn(NamesOfBtn.Seal, "签章", "sealOffice");
                }
                if (md.IsWoEnableInsertFlow)
                {
                    AddBtn(NamesOfBtn.FlowImage, "插入流程图", "InsertFlow");
                }
                if (md.IsWoEnableInsertFengXian)
                {
                    AddBtn("fegnxian", "插入风险点", "InsertFengXian");
                }
                if (md.IsWoEnableDown)
                {
                    AddBtn(NamesOfBtn.Download, "下载", "DownLoad");
                }
                #endregion
            }
        }

        private void LoadFile()
        {
            string name = Request.QueryString["fileName"];
            var path = SystemConfig.PathOfDataUser + "\\FrmOfficeFiles\\" + name;

            var result = File.ReadAllBytes(path);

            Response.Clear();
            Response.BinaryWrite(result);
            Response.End();
        }

        public void LoadFrmData(MapAttrs mattrs, Entity en)
        {
            var mes = new MapExts(this.FK_MapData);
            var dictParams = new ReplaceFieldList();
            var fields = new List<string>();

            dictParams.Add("No", WebUser.No, "string");
            dictParams.Add("Name", WebUser.Name, "string");
            dictParams.Add("FK_Dept", WebUser.FK_Dept, "string");
            dictParams.Add("FK_DeptName", WebUser.FK_DeptName, "string");

            if (mes.Count == 0)
            {
                ReplaceParams = GenerateParamsJsonString(dictParams);
                ReplaceFields = "[]";
                ReplaceDtlNos = "[]";
                ReplaceDtls = "[]";
                return;
            }

            MapExt item = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull) as MapExt;
            if (item == null)
            {
                ReplaceParams = GenerateParamsJsonString(dictParams);
                ReplaceFields = "[]";
                ReplaceDtlNos = "[]";
                ReplaceDtls = "[]";
                return;
            }

            DataTable dt = null;
            MapAttr mattr = null;
            string sql = item.Tag;
            if (DataType.IsNullOrEmpty(sql) == false)
            {
                /* 如果有填充主表的sql  */
                #region 处理sql变量
                sql = sql.Replace("@WebUser.No", WebUser.No);
                sql = sql.Replace("@WebUser.Name", WebUser.Name);
                sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                sql = sql.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);

                foreach (MapAttr attr in mattrs)
                {
                    if (sql.Contains("@"))
                        sql = sql.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                    else
                        break;
                }
                #endregion 处理sql变量

                if (DataType.IsNullOrEmpty(sql) == false)
                {
                    if (sql.Contains("@"))
                        throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);
                    dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 1)
                    {
                        DataRow dr = dt.Rows[0];
                        foreach (DataColumn dc in dt.Columns)
                        {
                            //去掉一些不需要copy的字段.
                            switch (dc.ColumnName)
                            {
                                case WorkAttr.OID:
                                case WorkAttr.FID:
                                case WorkAttr.Rec:
                                case WorkAttr.MD5:
                                case WorkAttr.MyNum:
                                case WorkAttr.RDT:
                                case "RefPK":
                                case WorkAttr.RecText:
                                    continue;
                                default:
                                    break;
                            }

                            en.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());

                            mattr = mattrs.GetEntityByKey(MapAttrAttr.KeyOfEn, dc.ColumnName) as MapAttr;

                            dictParams.Add(dc.ColumnName, dr[dc.ColumnName].ToString(), mattr != null && mattr.IsSigan ? "sign" : "string");
                            fields.Add(dc.ColumnName);
                        }
                    }
                }
            }

            if (IsFirst)
                ReplaceParams = GenerateParamsJsonString(dictParams);
            else
                ReplaceParams = "[]";

            ReplaceFields = GenerateFieldsJsonString(fields);

            if (DataType.IsNullOrEmpty(item.Tag1)
                || item.Tag1.Length < 15)
            {
                ReplaceDtls = "[]";
                ReplaceDtlNos = "[]";
                return;
            }

            ReplaceDtls = "[";
            ReplaceDtlNos = "[";
            MapDtls dtls = new MapDtls(this.FK_MapData);
            // 填充从表.
            foreach (MapDtl dtl in dtls)
            {
                ReplaceDtlNos += "\"" + dtl.No + "\",";
                
                if (!IsFirst)
                    continue;

                string[] sqls = item.Tag1.Split('*');
                foreach (string mysql in sqls)
                {
                    if (DataType.IsNullOrEmpty(mysql))
                        continue;

                    if (mysql.Contains(dtl.No + "=") == false)
                        continue;

                    #region 处理sql.
                    sql = mysql;
                    sql = sql.Replace(dtl.No + "=", "");
                    sql = sql.Replace("@WebUser.No", WebUser.No);
                    sql = sql.Replace("@WebUser.Name", WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                    sql = sql.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
                    foreach (MapAttr attr in mattrs)
                    {
                        if (sql.Contains("@"))
                            sql = sql.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                        else
                            break;
                    }
                    #endregion 处理sql.

                    if (DataType.IsNullOrEmpty(sql))
                        continue;

                    if (sql.Contains("@"))
                        throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);

                    GEDtls gedtls = new GEDtls(dtl.No);
                    try
                    {
                        gedtls.Delete(GEDtlAttr.RefPK, en.PKVal);
                    }
                    catch
                    {
                        gedtls.GetNewEntity.CheckPhysicsTable();
                    }

                    dt = DBAccess.RunSQLReturnTable(sql);
                    //dictDtls.Add(dtl.No, dt);
                    ReplaceDtls += "{\"dtlno\":\"" + dtl.No + "\",\"dtl\":[";
                    var idx = 1;
                    foreach (DataRow dr in dt.Rows)
                    {
                        ReplaceDtls += "{\"rowid\":" + (idx++) + ",\"cells\":[";
                        GEDtl gedtl = gedtls.GetNewEntity as GEDtl;

                        foreach (DataColumn dc in dt.Columns)
                        {
                            gedtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());

                            mattr = dtl.MapAttrs.GetEntityByKey(MapAttrAttr.KeyOfEn, dc.ColumnName) as MapAttr;

                            ReplaceDtls += "{\"key\":\"" + dc.ColumnName + "\",\"value\":\"" + dr[dc.ColumnName] + "\",\"type\":\"" + (mattr != null && mattr.IsSigan ? "sign" : "string") + "\"},";
                        }

                        ReplaceDtls = ReplaceDtls.TrimEnd(',') + "]},";
                        gedtl.RefPK = en.PKVal.ToString();
                        gedtl.RDT = DataType.CurrentDataTime;
                        gedtl.Rec = WebUser.No;
                        gedtl.Insert();
                    }

                    ReplaceDtls = ReplaceDtls.TrimEnd(',') + "]}";
                }
            }

            ReplaceDtls = ReplaceDtls.TrimEnd(',') + "]";
            ReplaceDtlNos = ReplaceDtlNos.TrimEnd(',') + "]";
        }

        /// <summary>
        /// 转换键值集合为json字符串
        /// </summary>
        /// <param name="dictParams">键值集合</param>
        /// <returns></returns>
        private string GenerateParamsJsonString(ReplaceFieldList dictParams)
        {
            return "[" + dictParams.Aggregate(string.Empty, (curr, next) => curr + string.Format("{{\"key\":\"{0}\",\"value\":\"{1}\",\"type\":\"{2}\"}},", next.key, (next.value ?? "").Replace("\\", "\\\\").Replace("'", "\'"), next.type)).TrimEnd(',') + "]";
        }

        /// <summary>
        /// 转换String集合为json字符串
        /// </summary>
        /// <param name="fields">String集合</param>
        /// <returns></returns>
        private string GenerateFieldsJsonString(List<string> fields)
        {
            return LitJson.JsonMapper.ToJson(fields);
        }

        private void SaveFile()
        {
            try
            {
                HttpFileCollection files = HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    //检查文件扩展名字
                    HttpPostedFile postedFile = files[0];
                    var fileName = Path.GetFileName(Request.QueryString["filename"]);
                    var path = SystemConfig.PathOfDataUser + @"\FrmOfficeFiles\" + this.FK_MapData;

                    if (fileName != "")
                    {
                        postedFile.SaveAs(path + "\\" + fileName);

                        var en = new GEEntityWordFrm(this.FK_MapData);
                        en.RetrieveFromDBSources();

                        en.LastEditer = WebUser.Name;
                        en.RDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        en.Update();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AddBtn(string id, string label, string clickEvent)
        {
            var btn = new LinkBtn(true);
            btn.ID = id;
            btn.Text = label;
            btn.Attributes["onclick"] = "return " + clickEvent + "()";
            btn.PostBackUrl = "javascript:void(0)";
            divMenu.Controls.Add(btn);
        }
    }

    #region 用于和前端JS进行JSON交互定义的辅助类

    /// <summary>
    /// 字段
    /// </summary>
    public class ReplaceField
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
    }

    public class ReplaceFieldList : List<ReplaceField>
    {
        public void Add(string _key, string _value, string _type)
        {
            Add(new ReplaceField { key = _key, value = _value, type = _type });
        }
    }

    /// <summary>
    /// 明细表
    /// </summary>
    public class ReplaceDtlTable
    {
        /// <summary>
        /// 明细表编号
        /// </summary>
        public string dtlno { get; set; }
        /// <summary>
        /// 明细表行集合
        /// </summary>
        public List<ReplaceDtlRow> dtl { get; set; }
    }

    /// <summary>
    /// 明细表行
    /// </summary>
    public class ReplaceDtlRow
    {
        /// <summary>
        /// 行序号
        /// </summary>
        public string rowid { get; set; }
        /// <summary>
        /// 行单元格数据集合
        /// </summary>
        public ReplaceFieldList cells { get; set; }
    }
    #endregion
}