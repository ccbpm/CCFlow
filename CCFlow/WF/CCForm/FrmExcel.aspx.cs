using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web;
using BP.Web.Controls;
using BP.WF.Template;

namespace CCFlow.WF.CCForm
{
    public partial class FrmExcel : BP.Web.WebPage
    {
        #region 属性
        public string FK_Flow
        {
            get
            {
                var fk_flow = this.Request.QueryString["FK_Flow"];
                return fk_flow;
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
                    //if (DataType.IsNullOrEmpty(nodeid))
                    //    nodeid = "12201";
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
                //if (DataType.IsNullOrEmpty(workid))
                //    workid = "167";
                return workid;
            }
        }

        public int OID
        {
            get
            {
                string oid = this.Request.QueryString["WorkID"];
                if (DataType.IsNullOrEmpty(oid))
                    oid = this.Request.QueryString["OID"];

                if (DataType.IsNullOrEmpty(oid))
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
        public string FK_MapData
        {
            get
            {
                string s = this.Request.QueryString["FK_MapData"];
                //if (s == null)
                //    return "ExcelCSBD,Excel_CSBD3";
                return s;
            }
        }
        /// <summary>
        /// SID
        /// </summary>
        public string SID
        {
            get
            {
                return this.Request.QueryString["SID"];
            }
        }
        /// <summary>
        /// 是否打印？
        /// </summary>
        public bool IsPrint { get; set; }
        /// <summary>
        /// 是否编辑？
        /// </summary>
        public bool IsEdit { get; set; }
        /// <summary>
        /// 是否留痕？
        /// </summary>
        public bool IsMarks { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsCheck { get; set; }
        /// <summary>
        /// 是否是第一次打开Word表单
        /// </summary>
        public string IsFirsts { get; set; }
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
        /// 填充的明细表编号JSON数据，为String数组。
        /// </summary>
        public string ReplaceDtlNos { get; set; }
        /// <summary>
        /// 要显示的excel表单的FK_MapData编号集合，为String数组
        /// </summary>
        public string FK_MapDatas { get; set; }
        /// <summary>
        /// 每个excel表单对应的权限集合，为对象数组，格式如：[{"ExcelBSD1":{"OfficeSaveEnable":true,...}},{"ExcelBSD2":{"OfficeSaveEnable":false,...}]
        /// </summary>
        public string ToolbarSlns { get; set; }
        /// <summary>
        /// 填充的主表属性控制，存储在Sys_FrmSln表中各字段的可用性等控制，为对象数组，格式如：[{"ExcelBSD1":[{"Field":"aaa","UIVisible":true,"UIIsEnable":true,"IsNotNull":true},{...}]},{"ExcelBSD2":[{"Field":"bbb","UIVisible":true,"UIIsEnable":false,"IsNotNull":false},{...}]}]
        /// </summary>
        public string ReplaceFieldCtrls { get; set; }
        /// <summary>
        /// 记录每个excel表单是否是第一次加载
        /// </summary>
        public Dictionary<string, bool> firsts { get; set; }
        /// <summary>
        /// 定义一个权限控制变量.
        /// </summary>
        public ToolbarExcel toolbar = new ToolbarExcel();
        #endregion 属性

        private string[] fk_mapdatas = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //WebUser.SignInOfGener(new BP.Port.Emp("fuhui"));

            UserName = WebUser.Name;
            if (DataType.IsNullOrEmpty(this.FK_MapData))
            {
                divMenu.InnerHtml = "<h1 style='color:red'>必须传入参数FK_Mapdata!<h1>";
                return;
            }

            fk_mapdatas = FK_MapData.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //获得外部的标记。
            string type = Request["action"];
            if (DataType.IsNullOrEmpty(type))
            {
                /** 第一次进来，的时候，没有标记。
                 */
                //初始化它的解决方案.  add by stone. 2015-01-25. 增加权限控制方案，以在不同的节点实现不同的控制.
                IsEdit = string.IsNullOrWhiteSpace(Request.QueryString["IsEdit"])
                             ? true
                             : Request.QueryString["IsEdit"] == "1";
                IsPrint = string.IsNullOrWhiteSpace(Request.QueryString["IsPrint"])
                             ? true
                             : Request.QueryString["IsPrint"] == "1";

                GenerateToolbarSlns();
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
                    SaveFile(fk_mapdatas);
                    SaveFieldInfos(fk_mapdatas);
                    return;
                }

                throw new Exception("@没有处理的标记错误:" + type);
            }

            firsts = new Dictionary<string, bool>();
            GEEntityExcelFrm en = null;
            FileInfo tmpFile = null;
            FileInfo excelFile = null;
            FrmFields frmFields = null;

            //检查数据文件是否存在？如果存在并打开不存在并copy模版。
            var root = SystemConfig.PathOfDataUser + "\\FrmOfficeTemplate\\";
            var rootInfo = new DirectoryInfo(root);
            var isFirst = false;
            if (!rootInfo.Exists)
                rootInfo.Create();

            ReplaceParams = "[";
            ReplaceFields = "[";
            ReplaceDtlNos = "[";
            ReplaceDtls = "[";
            IsFirsts = "[";
            FK_MapDatas = "[";
            ReplaceFieldCtrls = "[";

            //根据excel表单no来处理各自的信息
            var pk = 0;
            foreach (var fk_md in fk_mapdatas)
            {
                //创建excel数据实体.
                en = new GEEntityExcelFrm(fk_md);
                
                var files = rootInfo.GetFiles(fk_md + ".*");
                // 判断是否有这个数据文件.
                if (files.Length == 0)
                {
                    Response.Write("<h3>Excel表单模板文件不存在，请确认已经上传Excel表单模板，该模版的位于服务器：" + rootInfo.FullName + "</h3>");
                    Response.End();
                    return;
                }

                FK_MapDatas += "{\"Name\":\"" + en.ClassID + "\",\"Text\":\"" + en.EnDesc + "\"},";

                // 检查数据目录文件是否存在？
                var pathDir = SystemConfig.PathOfDataUser + @"\FrmOfficeFiles\" + fk_md;
                if (!Directory.Exists(pathDir))
                    Directory.CreateDirectory(pathDir);

                // 判断who is pk
                pk = GetPK(fk_md);
                if (pk == 0) return;

                // 初始化数据文件. 
                tmpFile = files[0];
                excelFile = new FileInfo(pathDir + "\\" + pk + tmpFile.Extension);
                if (excelFile.Exists == false)
                {
                    /*如果不存在就copy 一个副本。*/
                    File.Copy(tmpFile.FullName, excelFile.FullName);
                    isFirst = true;
                }
                else
                {
                    //edited by liuxc,2015-3-25,此处增加判断，如果模板文件与生成的数据文件的最后修改时间是一致的，表明此数据文件还没有经过修改，也标识为第一次，加载填充数据信息
                    isFirst = excelFile.LastWriteTime.Equals(tmpFile.LastWriteTime);
                }

                firsts.Add(fk_md, isFirst);
                IsFirsts += "{\"" + fk_md + "\":" + isFirst.ToString().ToLower() + "},";

                //edited by liuxc,2015-1-30,如果在构造中使用传递OID的构造函数，则下面的Save时，第一次会插入不成功，此处是因为insert时判断OID不为0则认为是已经存在的记录，实际上此处还没有存在，所以使用下面的逻辑进行判断，如果没有该条记录，则插入新记录
                en.OID = pk;

                if (en.IsExits == false)
                    en.InsertAsOID(pk);
                else
                    en.Retrieve();

                //给实体赋值.
                en.FilePath = excelFile.FullName;
                en.RDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                en.LastEditer = WebUser.Name;
                
#warning 为什么要加入它？ 2016.12.17被 zhoupeng 注释.
               // en.SetPara(FK_Node.ToString(), true);

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

                en.Save(); //执行保存.

                //装载数据。
                ReplaceParams += "{\"" + fk_md + "\":";
                ReplaceFields += "{\"" + fk_md + "\":";
                ReplaceDtlNos += "{\"" + fk_md + "\":";
                ReplaceDtls += "{\"" + fk_md + "\":";
                ReplaceFieldCtrls += "{\"" + fk_md + "\":[";

                this.LoadFrmData(fk_md, en);

                //字段控制
                frmFields = new FrmFields(fk_md, FK_Node);
                foreach(FrmField frmField in frmFields)
                {
                    ReplaceFieldCtrls +=
                        string.Format("{{\"{0}\":{{\"Name\":\"{1}\",\"UIVisible\":{2},\"UIIsEnable\":{3},\"IsNotNull\":{4},\"OldValue\":\"{5}\"}}}},",
                                      frmField.KeyOfEn,
                                      frmField.Name,
                                      frmField.UIVisible.ToString().ToLower(),
                                      frmField.UIIsEnable.ToString().ToLower(),
                                      frmField.IsNotNull.ToString().ToLower(),
                                      frmField.UIIsEnable ? "" : en.GetValStringByKey(frmField.KeyOfEn, ""));
                }

                ReplaceParams += "},";
                ReplaceFields += "},";
                ReplaceDtlNos += "},";
                ReplaceDtls += "},";
                ReplaceFieldCtrls = ReplaceFieldCtrls.TrimEnd(',') + "]},";
            }

            ReplaceParams = ReplaceParams.TrimEnd(',') + "]";
            ReplaceFields = ReplaceFields.TrimEnd(',') + "]";
            ReplaceDtlNos = ReplaceDtlNos.TrimEnd(',') + "]";
            ReplaceDtls = ReplaceDtls.TrimEnd(',') + "]";
            IsFirsts = IsFirsts.TrimEnd(',') + "]";
            FK_MapDatas = FK_MapDatas.TrimEnd(',') + "]";
            ReplaceFieldCtrls = ReplaceFieldCtrls.TrimEnd(',') + "]";

            //替换掉 word 里面的数据.
            fileName.Text = string.Format(@"\{0}\{1}{2}", fk_mapdatas[0], pk, excelFile.Extension);
            fileType.Text = excelFile.Extension.TrimStart('.');
        }

        /// <summary>
        /// 计算PK值
        /// </summary>
        /// <param name="fk_md">FK_MapData</param>
        /// <returns></returns>
        private int GetPK(string fk_md)
        {
            // add by zhoupeng 如果这两个参数没有，就返回OID做为PK.
            if (this.FK_Flow == null || this.FK_Node==0 )
                return this.OID;

            int pk = this.OID;
            var fn = new FrmNode(this.FK_Flow, this.FK_Node, fk_md);
            switch (fn.WhoIsPK)
            {
                case WhoIsPK.FID:
                    pk = this.FID;
                    if (pk == 0)
                        throw new Exception("@没有接收到参数FID");
                    break;
                case WhoIsPK.CWorkID: /*延续流程ID*/
                    pk = this.CWorkID;
                    if (pk == 0)
                        throw new Exception("@没有接收到参数CWorkID");
                    break;
                case WhoIsPK.PWorkID: /*父流程ID*/
                    pk = this.PWorkID;
                    if (pk == 0)
                        throw new Exception("@没有接收到参数PWorkID");
                    break;
                default:
                    break;
            }

            return pk;
        }

        /// <summary>
        /// 生成各FK_MapData对应的权限控制json字符串，应用于前端JS控制相应的显示
        /// </summary>
        private void GenerateToolbarSlns()
        {
            BP.WF.Template.FrmNode fn = null;
            ToolbarExcelSln toobarsln = null;
            ToolbarSlns = "[";

            foreach (var fk_mapdata in fk_mapdatas)
            {
                ToolbarSlns += "{\"" + fk_mapdata + "\":";

                if (DataType.IsNullOrEmpty(this.FK_Flow) == false)
                {
                    /*接受到了流程编号，就要找到他的控制方案.*/
                    fn = new BP.WF.Template.FrmNode(this.FK_Flow, this.FK_Node, fk_mapdata);
                    if (fn.FrmSln == 1)
                    {
                        /* 如果是自定义方案.*/
                        toobarsln = new ToolbarExcelSln(this.FK_Flow, this.FK_Node, fk_mapdata);
                        toolbar.Row = toobarsln.Row;
                    }
                    else
                    {
                        //非自定义方案就取默认方案.
                        toolbar = new ToolbarExcel(fk_mapdata);
                    }
                }
                else
                {
                    /*没有找打他的控制方案，就取默认方案.*/
                    toolbar = new ToolbarExcel(fk_mapdata);
                }

                ToolbarSlns += GetToolbarSlnJsonString(toolbar) + "},";
            }

            ToolbarSlns = ToolbarSlns.TrimEnd(',') + "]";
        }

        /// <summary>
        /// 生成权限控制的对象json字符串
        /// </summary>
        /// <param name="toolbar">权限控制对象</param>
        /// <returns></returns>
        private string GetToolbarSlnJsonString(ToolbarExcel toolbar)
        {
            var json = "{";

            var t = toolbar.GetType();
            var props = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            object val = null;

            foreach(var prop in props)
            {
                if(!prop.Name.StartsWith("Office")) continue;

                val = prop.GetValue(toolbar, null);
                switch(prop.PropertyType.Name)
                {
                    case "Boolean":
                        json += "\"" + prop.Name + "\":" + val.ToString().ToLower() + ",";
                        break;
                    case "String":
                        json += "\"" + prop.Name + "\":\"" + (val ?? string.Empty) + "\",";
                        break;
                    default:
                        break;
                }
            }

            return json.TrimEnd(',') + "}";
        }

        /// <summary>
        /// 保存从word中提取的数据
        /// <param name="fk_mds">excel表单的编号</param>
        /// </summary>
        private void SaveFieldInfos(string[] fk_mds)
        {
            foreach (var fk_md in fk_mds)
            {
                var mes = new MapExts(fk_md);
                if (mes.Count == 0) return;

                var item = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull) as MapExt;
                if (item == null) return;

                var fieldCount = 0;
                var prefix = "field_" + fk_md;
                foreach (var key in Request.Form.AllKeys)
                {
                    var idx = 0;
                    if (key.StartsWith(prefix) && key.Length > prefix.Length && int.TryParse(key.Substring(prefix.Length), out idx))
                    {
                        fieldCount++;
                    }
                }

                var fieldsJson = string.Empty;
                for (var i = 0; i < fieldCount; i++)
                {
                    fieldsJson += Request[prefix + i];
                }

                //var fieldsJson = Request["field"];
                var fields = LitJson.JsonMapper.ToObject<List<ReplaceField>>(HttpUtility.UrlDecode(fieldsJson));

                //更新主表数据
                var en = new GEEntityExcelFrm(fk_md);
                var pk = en.OID = GetPK(fk_md);

                if (en.RetrieveFromDBSources() == 0)
                {
                    throw new Exception("OID=" + pk + "的数据在" + fk_md + "中不存在，请检查！");
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
                var mdtls = new MapDtls(fk_md);
                if (mdtls.Count == 0) return;

                var dtlsCount = 0;
                prefix = "dtls_" + fk_md;
                foreach (var key in Request.Form.AllKeys)
                {
                    var idx = 0;
                    if (key.StartsWith(prefix) && key.Length > prefix.Length && int.TryParse(key.Substring(prefix.Length), out idx))
                    {
                        dtlsCount++;
                    }
                }

                var dtlsJson = string.Empty;
                for (var i = 0; i < dtlsCount; i++)
                {
                    dtlsJson += Request[prefix + i];
                }

                //var dtlsJson = Request["dtls"];
                var dtls = LitJson.JsonMapper.ToObject<List<ReplaceDtlTable>>(HttpUtility.UrlDecode(dtlsJson));
                GEDtls gedtls = null;
                GEDtl gedtl = null;
                ReplaceDtlTable wdtl = null;

                foreach (MapDtl mdtl in mdtls)
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
        }

        private static object asyncObj = new object();

        /// <summary>
        /// 装载文件.
        /// </summary>
        private void LoadFile()
        {
            lock (asyncObj)
            {
                string name = Request.QueryString["fileName"];
                var path = SystemConfig.PathOfDataUser + "\\FrmOfficeFiles" + name;
                var result = File.ReadAllBytes(path);
                Response.Clear();
                Response.BinaryWrite(result);
                Response.End();
            }
        }
        /// <summary>
        /// 生成要返回给page的Json数据.
        /// </summary>
        /// <param name="fk_md"></param>
        /// <param name="en"></param>
        public void LoadFrmData(string fk_md, Entity en)
        {
            var dictParams = new ReplaceFieldList(); //主表参数值集合
            var fields = new List<string>(); // 主表参数名集合

            dictParams.Add("No", WebUser.No, "string");
            dictParams.Add("Name", WebUser.Name, "string");
            dictParams.Add("FK_Dept", WebUser.FK_Dept, "string");
            dictParams.Add("FK_DeptName", WebUser.FK_DeptName, "string");
            
            var mes = new MapExts(fk_md);
            MapExt item = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull) as MapExt;
            //把数据装载到表里，包括从表数据，主表数据未存储.
            MapDtls dtls = new MapDtls(fk_md);
            MapAttrs mattrs = new MapAttrs(fk_md);
            en = BP.WF.Glo.DealPageLoadFull(en, item, mattrs, dtls); // 处理表单装载数据.

            //MapData md=new MapData(this.FK_MapData);
            foreach (MapAttr mapattr in mattrs)
            {
                fields.Add(mapattr.KeyOfEn);
                dictParams.Add(mapattr.KeyOfEn, en.GetValStringByKey(mapattr.KeyOfEn), mapattr.IsSigan ? "sign" : "string");
            }

            ReplaceParams += firsts[fk_md] ? GenerateParamsJsonString(dictParams) : "[]";

            //生成json格式。
            ReplaceFields += GenerateFieldsJsonString(fields);

            if (item == null || DataType.IsNullOrEmpty(item.Tag1)
                || item.Tag1.Length < 15)
            {
                ReplaceDtls += "[]";
                ReplaceDtlNos += "[]";
                return;
            }

            var replaceDtlNos = new List<string>();
            DataSet ds = new DataSet();
            DataTable table = null;
            var sql = string.Empty;
            var pk = GetPK(fk_md);

            // 填充从表.
            foreach (MapDtl dtl in dtls)
            {
                replaceDtlNos.Add(dtl.No);

                if (!firsts[fk_md])
                    continue;

                sql = "SELECT * FROM " + dtl.PTable + " WHERE RefPK='" + pk + "'";
                table = BP.DA.DBAccess.RunSQLReturnTable(sql);
                table.TableName = dtl.No;
                ds.Tables.Add(table);
            }

            // 从表数据.
            ReplaceDtls += firsts[fk_md] ? BP.DA.DataTableConvertJson.Dataset2Json(ds) : "[]";
            ReplaceDtlNos += GenerateFieldsJsonString(replaceDtlNos);
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

        private void SaveFile(string[] fk_mds)
        {
            try
            {
                HttpFileCollection files = HttpContext.Current.Request.Files;

                if (files.Count > 0)
                {
                    //检查文件扩展名字
                    HttpPostedFile postedFile = files[0];
                    var fileName = Path.GetFileName(Request.QueryString["filename"]);

                    foreach (var fk_md in fk_mds)
                    {
                        var path = SystemConfig.PathOfDataUser + @"\FrmOfficeFiles\" + fk_md;

                        if (fileName != "")
                        {
                            postedFile.SaveAs(path + "\\" + fileName);

                            var en = new GEEntityExcelFrm(fk_md);
                            en.RetrieveFromDBSources();

                            en.LastEditer = WebUser.Name;
                            en.RDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            en.Update();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}