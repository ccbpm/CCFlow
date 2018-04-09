using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Web.Controls;
using BP.WF;
using BP.WF.Data;
using BP.En;

namespace CCFlow.WF.Rpt
{
    public partial class RptDoc : System.Web.UI.Page
    {
        #region 属性
        public string FK_Flow
        {
            get
            {
                var flow = this.Request.QueryString["FK_Flow"];
                if (string.IsNullOrWhiteSpace(flow))
                    flow = "002";
                return flow;
            }
        }
        public int FK_Node
        {
            get
            {
                try
                {
                    string nodeid = this.Request.QueryString["NodeID"];
                    if (string.IsNullOrWhiteSpace(nodeid))
                        nodeid = this.Request.QueryString["FK_Node"];
                    if (string.IsNullOrWhiteSpace(nodeid))
                        nodeid = "205";
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
                var workid = this.Request.QueryString["WorkID"];
                if (string.IsNullOrWhiteSpace(workid))
                    workid = "132";
                return workid;
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

        /// <summary>
        /// 是否是第一次打开Word表单
        /// </summary>
        public bool IsFirst { get; set; }

        /// <summary>
        /// 填充的主表JSON数据，为含有key,value的数组
        /// </summary>
        public string ReplaceParams = "[]";

        /// <summary>
        /// 填充的主表属性名组织的JSON数据,为String数组
        /// </summary>
        public string ReplaceFields = "[]";

        /// <summary>
        /// 填充的明细表数据JSON数据，为对象数组
        /// </summary>
        public string ReplaceDtls = "[]";

        /// <summary>
        /// 填充的明细表编号JSON数据，为String数组
        /// </summary>
        public string ReplaceDtlNos = "[]";
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            //WebUser.SignInOfGener(new BP.Port.Emp("zhoupeng"));

            string type = Request["action"];
            if (DataType.IsNullOrEmpty(type))
            {
                InitOffice();
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
                    return;
                }

                InitOffice();
            }

            var root = SystemConfig.PathOfDataUser + "\\FrmOfficeTemplate\\" + this.FK_Flow + "\\";
            var rootInfo = new DirectoryInfo(root);

            if (!rootInfo.Exists)
                rootInfo.Create();

            var docFileName = "Rpt" + this.FK_Flow + "_" + this.WorkID + "_" + this.FK_Node;
            var docDirName = this.FK_Flow + "\\" + this.WorkID + "\\";
            var files = rootInfo.GetFiles("Rpt" + this.FK_Flow + "_" + this.FK_Node + ".*");
            FileInfo tmpFile = null;
            FileInfo wordFile = null;

            var pathDir = SystemConfig.PathOfDataUser + @"\FrmOfficeFiles\" + docDirName;

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
            wordFile = new FileInfo(pathDir + "\\" + docFileName + tmpFile.Extension);

            if (wordFile.Exists == false)
            {
                IsFirst = true;
                File.Copy(tmpFile.FullName, wordFile.FullName);
            }
            else
            {
                IsFirst = true;
            }

            //装载数据.
            var rptName = "ND" + int.Parse(this.FK_Flow) + "Rpt";
            var ndName = "ND" + this.FK_Node;
            var flow = new Flow(this.FK_Flow);
            Entity en = null;
            string enName = string.Empty;

            if (flow.HisDataStoreModel == BP.WF.Template.DataStoreModel.ByCCFlow)
            {
                enName = ndName;
                var gen = new GEEntity(enName);
                gen.OID = int.Parse(WorkID);

                if (gen.RetrieveFromDBSources() == 0)
                {
                    Response.Write("<h3>" + enName + "中不存在WorkID=" + WorkID + "的数据</h3>");
                    return;
                }

                en = gen;
            }
            else
            {
                enName = rptName;
                var rpt = new GERpt(enName);
                rpt.OID = int.Parse(WorkID);

                if (rpt.RetrieveFromDBSources() == 0)
                {
                    Response.Write("<h3>" + enName + "中不存在WorkID=" + WorkID + "的数据</h3>");
                    return;
                }

                en = rpt;
            }
            //var rpt = new GERpt(rptName);
            //rpt.OID = int.Parse(WorkID);
            //if (rpt.RetrieveFromDBSources() == 0)
            //{
            //    Response.Write("<h3>" + rptName + "中不存在WorkID=" + WorkID + "的数据</h3>");
            //    return;
            //}

            var attrs = new MapAttrs(enName);
            this.LoadFrmData(attrs, en);

            //替换掉 word 里面的数据.
            fileName.Text = string.Format(@"\{0}{1}{2}", docDirName, docFileName, wordFile.Extension);
            fileType.Text = wordFile.Extension.TrimStart('.');
        }
        
        private void InitOffice()
        {
            #region 初始化按钮
            AddBtn(NamesOfBtn.Save, "保存", "saveOffice");
            AddBtn(NamesOfBtn.Print, "打印", "printOffice");
            AddBtn(NamesOfBtn.Download, "下载", "DownLoad");
            #endregion
        }

        private void LoadFile()
        {
            string name = Request.QueryString["fileName"];
            var path = SystemConfig.PathOfDataUser + "\\FrmOfficeFiles" + name;

            var result = File.ReadAllBytes(path);

            Response.Clear();
            Response.BinaryWrite(result);
            Response.End();
        }

        public void LoadFrmData(MapAttrs mapAttrs, Entity en)
        {
            var dictParams = new KeyValueItemList();
            var fields = new List<string>();
            var attrVal = string.Empty;
            var type = "string";
            var attrs = en.EnMap.Attrs;

            dictParams.Add("UserNo", WebUser.No, type);
            dictParams.Add("UserName", WebUser.Name, type);
            dictParams.Add("UserDept", WebUser.FK_Dept, type);
            dictParams.Add("UserDeptName", WebUser.FK_DeptName, type);

            MapAttr mattr = null;

            foreach (Attr attr in attrs)
            {
                mattr = GetMapAttrByKeyEn(mapAttrs, attr.Key);

                type = mattr != null && mattr.IsSigan ? "sign" : "string";
                attrVal = Convert.ToString(en.GetValByKey(attr.Key));
                
                switch (attrVal)
                {
                    case "@RDT":
                        attrVal = DataType.CurrentData;
                        break;
                    case "@WebUser.No":
                        attrVal = WebUser.No;
                        break;
                    case "@WebUser.Name":
                        attrVal = WebUser.Name;
                        break;
                    case "@WebUser.FK_DeptName":
                        attrVal = WebUser.FK_DeptName;
                        break;
                    case "@WebUser.FK_DeptNameOfFull":
                        attrVal = WebUser.FK_DeptNameOfFull;
                        break;
                }
                
                dictParams.Add(attr.Key, string.IsNullOrWhiteSpace(attrVal) ? string.Empty : attrVal.Replace("\r\n", "\\r\\n"), type);
                fields.Add(attr.Key);
            }

            if (IsFirst)
            {
                ReplaceParams = GenerateParamsJsonString(dictParams);
                ReplaceFields = GenerateFieldsJsonString(fields);
            }

            var nd = new BP.WF.Node(FK_Node);
            var wk = nd.HisWork;
            wk.OID = int.Parse(WorkID);
            wk.Retrieve();
            wk.ResetDefaultVal();

            var dtlEns = wk.GetDtlsDatasOfList();

            if (dtlEns.Count == 0)
                return;

            ReplaceDtls = "[";
            ReplaceDtlNos = "[";

            MapAttrs dtlAttrs = null;
            string dtlNo = string.Empty;
            Entity enNew = null;

            foreach (var dens in dtlEns)
            {
                enNew = dens.GetNewEntity;
                dtlNo = enNew.EnMap.PhysicsTable;
                attrs = enNew.EnMap.Attrs;
                dtlAttrs = new MapAttrs(dtlNo);

                ReplaceDtlNos += "\"" + dtlNo + "\",";
                ReplaceDtls += "{\"dtlno\":\"" + dtlNo + "\",\"dtl\":[";
                var idx = 1;

                foreach (Entity den in dens)
                {
                    ReplaceDtls += "{\"rowid\":" + (idx++) + ",\"cells\":[";

                    foreach (Attr attr in attrs)
                    {
                        mattr = GetMapAttrByKeyEn(dtlAttrs, attr.Key);
                        type = mattr != null && mattr.IsSigan ? "sign" : "string";

                        ReplaceDtls += "{\"key\":\"" + attr.Key + "\",\"value\":\"" + den.GetValByKey(attr.Key) + "\",\"type\":\"" + type + "\"},";
                    }
                    
                    ReplaceDtls = ReplaceDtls.TrimEnd(',') + "]},";
                }

                ReplaceDtls = ReplaceDtls.TrimEnd(',') + "]},";
            }

            ReplaceDtls = ReplaceDtls.TrimEnd(',') + "]";
            ReplaceDtlNos = ReplaceDtlNos.TrimEnd(',') + "]";
        }

        /// <summary>
        /// 获取指定Key的MapAttr
        /// </summary>
        /// <param name="mapAttrs">MapAttrs</param>
        /// <param name="attrKey">Attr.Key</param>
        /// <returns></returns>
        private MapAttr GetMapAttrByKeyEn(MapAttrs mapAttrs, string attrKey)
        {
            foreach(MapAttr mattr in mapAttrs)
            {
                if (mattr.KeyOfEn == attrKey)
                    return mattr;
            }

            return null;
        }

        /// <summary>
        /// 转换键值集合为json字符串
        /// </summary>
        /// <param name="dictParams">键值集合</param>
        /// <returns></returns>
        private string GenerateParamsJsonString(KeyValueItemList dictParams)
        {
            return LitJson.JsonMapper.ToJson(dictParams);
            //return "[" + dictParams.Aggregate(string.Empty, (curr, next) => curr + string.Format("{{\"key\":\"{0}\",\"value\":\"{1}\",\"type\":\"{2}\"}},", next.key, (next.value ?? "").Replace("\\", "\\\\").Replace("'", "\'"), next.type)).TrimEnd(',') + "]";
        }

        /// <summary>
        /// 转换String集合为json字符串
        /// </summary>
        /// <param name="fields">String集合</param>
        /// <returns></returns>
        private string GenerateFieldsJsonString(List<string> fields)
        {
            return LitJson.JsonMapper.ToJson(fields);
            //return "[" + fields.Aggregate(string.Empty, (curr, next) => curr + "\"" + next + "\",").TrimEnd(',') + "]";
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
                    var fname = Path.GetFileName(Request.QueryString["filename"]);
                    var dir = Path.GetDirectoryName(Request.QueryString["filename"]);
                    var path = SystemConfig.PathOfDataUser + @"\FrmOfficeFiles\" + dir;

                    if (!string.IsNullOrWhiteSpace(fname))
                    {
                        postedFile.SaveAs(path + "\\" + fname);
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

        /// <summary>
        /// 本页面应用于JSON传输的辅助kv对象
        /// </summary>
        private class KeyValueItem
        {
            public string key { get; set; }
            public string value { get; set; }
            public string type { get; set; }
        }

        /// <summary>
        /// 本页面应用于JSON传输的辅助kv对象集合
        /// </summary>
        private class KeyValueItemList : List<KeyValueItem>
        {
            public void Add(string _key, string _value, string _type)
            {
                Add(new KeyValueItem {key = _key, value = _value, type = _type});
            }
        }
    }
}