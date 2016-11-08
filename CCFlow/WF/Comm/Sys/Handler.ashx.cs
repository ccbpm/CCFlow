using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Data;
using System.Web.Services.Description;
using System.Xml.Schema;
using BP.DA;
using BP.En;
using BP.Sys;

namespace CCFlow.WF.Comm.Sys
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        #region 执行.
        public HttpContext context = null;
        /// <summary>
        /// 执行类型
        /// </summary>
        public string DoType
        {
            get
            {
                string str = context.Request.QueryString["DoType"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        #endregion

        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;

            if (string.IsNullOrWhiteSpace(DoType))
                return;

            //通用局部变量定义
            string resultString = string.Empty;
            string sfno = context.Request.QueryString["sfno"];
            SFTable sftable = null;
            DataTable dt = null;
            StringBuilder s = null;

            try
            {
                switch (DoType)
                {
                    #region SFGuide.htm
                    case "sfguide_getinfo": //获取数据源字典表信息
                        if (string.IsNullOrWhiteSpace(sfno))
                            resultString = ReturnJson(false, "参数不正确", false);
                        else
                        {
                            sftable = new SFTable(sfno);
                            dt = sftable.ToDataTableField("info");

                            foreach (DataColumn col in dt.Columns)
                                col.ColumnName = col.ColumnName.ToUpper();

                            resultString = ReturnJson(true, BP.Tools.Json.ToJson(dt), true);// "{\"success\": true, \"data\": " + +"\"}";
                        }
                        break;
                    case "sfguide_saveinfo":    //保存
                        bool isnew = Convert.ToBoolean(context.Request.QueryString["isnew"]);
                        sfno = context.Request.QueryString["NO"];
                        string name = context.Request.QueryString["NAME"];
                        int srctype = int.Parse(context.Request.QueryString["SRCTYPE"]);
                        int codestruct = int.Parse(context.Request.QueryString["CODESTRUCT"]);
                        string defval = context.Request.QueryString["DEFVAL"];
                        string sfdbsrc = context.Request.QueryString["FK_SFDBSRC"];
                        string srctable = context.Request.QueryString["SRCTABLE"];
                        string columnvalue = context.Request.QueryString["COLUMNVALUE"];
                        string columntext = context.Request.QueryString["COLUMNTEXT"];
                        string parentvalue = context.Request.QueryString["PARENTVALUE"];
                        string tabledesc = context.Request.QueryString["TABLEDESC"];
                        string selectstatement = context.Request.QueryString["SELECTSTATEMENT"];

                        //判断是否已经存在
                        sftable = new SFTable();
                        sftable.No = sfno;

                        if (isnew && sftable.RetrieveFromDBSources() > 0)
                        {
                            resultString = ReturnJson(false, "字典编号" + sfno + "已经存在，不允许重复。", false);
                        }
                        else
                        {
                            sftable.Name = name;
                            sftable.SrcType = (SrcType)srctype;
                            sftable.CodeStruct = (CodeStruct)codestruct;
                            sftable.DefVal = defval;
                            sftable.FK_SFDBSrc = sfdbsrc;
                            sftable.SrcTable = srctable;
                            sftable.ColumnValue = columnvalue;
                            sftable.ColumnText = columntext;
                            sftable.ParentValue = parentvalue;
                            sftable.TableDesc = tabledesc;
                            sftable.SelectStatement = selectstatement;

                            switch (sftable.SrcType)
                            {
                                case SrcType.BPClass:
                                    string[] nos = sftable.No.Split('.');
                                    sftable.FK_Val = "FK_" + nos[nos.Length - 1].TrimEnd('s');
                                    sftable.FK_SFDBSrc = "local";
                                    break;
                                default:
                                    sftable.FK_Val = "FK_" + sftable.No;
                                    break;
                            }

                            try
                            {
                                sftable.Save();
                                resultString = ReturnJson(true, "保存成功！", false);
                            }
                            catch (Exception ex)
                            {
                                resultString = ReturnJson(false, ex.Message, false);
                            }
                        }
                        break;
                    case "sfguide_getclass": //获取类列表
                        string stru = context.Request.QueryString["struct"];
                        int st = 0;

                        if (string.IsNullOrWhiteSpace(stru) || !int.TryParse(stru, out st))
                        {
                            resultString = ReturnJson(false, "参数不正确", false);
                        }
                        else
                        {
                            string error = string.Empty;
                            ArrayList arr = null;
                            SFTables sfs = new SFTables();
                            Entities ens = null;
                            SFTable sf = null;
                            sfs.Retrieve(SFTableAttr.SrcType, (int)SrcType.BPClass);

                            try
                            {
                                switch (st)
                                {
                                    case 0:
                                        arr = ClassFactory.GetObjects("BP.En.EntityNoName");
                                        break;
                                    case 1:
                                        arr = ClassFactory.GetObjects("BP.En.EntitySimpleTree");
                                        break;
                                    default:
                                        arr = new ArrayList();
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                error = "ClassFactory.GetObjects错误:" + ex.Message;
                            }

                            if (string.IsNullOrWhiteSpace(error))
                            {
                                s = new StringBuilder("[");

                                foreach (BP.En.Entity en in arr)
                                {
                                    try
                                    {
                                        if (en == null)
                                            continue;

                                        ens = en.GetNewEntities;

                                        if (ens == null)
                                            continue;

                                        sf = sfs.GetEntityByKey(ens.ToString()) as SFTable;

                                        if ((sf != null && sf.No != sfno) ||
                                            string.IsNullOrWhiteSpace(ens.ToString()))
                                            continue;

                                        s.Append(string.Format(
                                            "{{\"NO\":\"{0}\",\"NAME\":\"{0}[{1}]\",\"DESC\":\"{1}\"}},", ens,
                                            en.EnDesc));
                                    }
                                    catch
                                    {
                                        //BP.DA.Log
                                        //  BP.DA.Log.DefaultLogWriteLine(ex.Message);
                                        continue;
                                    }
                                }

                                resultString = ReturnJson(true, s.ToString().TrimEnd(',') + "]", true);
                            }
                            else
                            {
                                resultString = ReturnJson(false, error, false);
                            }
                        }
                        break;
                    case "sfguide_getsrcs": //获取数据源列表
                        string type = context.Request.QueryString["type"];
                        int itype;
                        bool onlyWS = false;

                        SFDBSrcs srcs = new SFDBSrcs();

                        if (!string.IsNullOrWhiteSpace(type) && int.TryParse(type, out itype))
                        {
                            onlyWS = true;
                            srcs.Retrieve(SFDBSrcAttr.DBSrcType, itype);
                        }
                        else
                        {
                            srcs.RetrieveAll();
                        }

                        dt = srcs.ToDataTableField();

                        foreach (DataColumn col in dt.Columns)
                            col.ColumnName = col.ColumnName.ToUpper();

                        if (!onlyWS)
                        {
                            List<DataRow> wsRows = new List<DataRow>();

                            foreach (DataRow r in dt.Rows)
                            {
                                if (Equals(r["DBSRCTYPE"], (int)DBSrcType.WebServices))
                                    wsRows.Add(r);
                            }

                            foreach (DataRow r in wsRows)
                                dt.Rows.Remove(r);
                        }

                        resultString = ReturnJson(true, BP.Tools.Json.ToJson(dt), true);
                        break;
                    case "sfguide_gettvs": //获取表/视图列表
                        string src = context.Request.QueryString["src"];

                        if (string.IsNullOrWhiteSpace(src))
                        {
                            resultString = ReturnJson(false, "参数不正确", false);
                        }
                        else
                        {
                            SFDBSrc sr = new SFDBSrc(src);
                            dt = sr.GetTables();

                            foreach (DataColumn col in dt.Columns)
                                col.ColumnName = col.ColumnName.ToUpper();

                            resultString = ReturnJson(true, BP.Tools.Json.ToJson(dt), true);
                        }
                        break;
                    case "sfguide_getcols": //获取表/视图的列信息
                        src = context.Request.QueryString["src"];
                        string table = context.Request.QueryString["table"];

                        if (string.IsNullOrWhiteSpace(src))
                        {
                            resultString = ReturnJson(false, "参数不正确", false);
                        }
                        else if (string.IsNullOrWhiteSpace(table))
                        {
                            resultString = ReturnJson(true, "[]", true);
                        }
                        else
                        {
                            SFDBSrc sr = new SFDBSrc(src);
                            dt = sr.GetColumns(table);

                            foreach (DataColumn col in dt.Columns)
                                col.ColumnName = col.ColumnName.ToUpper();

                            foreach (DataRow r in dt.Rows)
                            {
                                r["NAME"] = r["NO"] +
                                            (r["NAME"] == null || r["NAME"] == DBNull.Value ||
                                             string.IsNullOrWhiteSpace(r["NAME"].ToString())
                                                 ? ""
                                                 : string.Format("[{0}]", r["NAME"]));
                            }

                            resultString = ReturnJson(true, DataTableConvertJson.DataTable2Json(dt), true);
                        }
                        break;
                    case "sfguide_getmtds": //获取WebService方法列表
                        src = context.Request.QueryString["src"];

                        if (string.IsNullOrWhiteSpace(src))
                        {
                            resultString = ReturnJson(false, "参数不正确", false);
                        }
                        else
                        {
                            SFDBSrc sr = new SFDBSrc(src);

                            if (sr.DBSrcType != DBSrcType.WebServices)
                            {
                                resultString = ReturnJson(false, "数据源“" + sr.Name + "”不是WebService数据源", false);
                            }
                            else
                            {
                                List<WSMethod> mtds = GetWebServiceMethods(sr);

                                resultString = ReturnJson(true, LitJson.JsonMapper.ToJson(mtds), true);
                            }
                        }
                        break;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                resultString = ReturnJson(false, ex.Message, false);
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(resultString);
        }
        /// <summary>
        /// 获取webservice方法列表
        /// </summary>
        /// <param name="dbsrc">WebService数据源</param>
        /// <returns></returns>
        public List<WSMethod> GetWebServiceMethods(SFDBSrc dbsrc)
        {
            if (dbsrc == null || string.IsNullOrWhiteSpace(dbsrc.IP)) return new List<WSMethod>();

            var wsurl = dbsrc.IP.ToLower();
            if (!wsurl.EndsWith(".asmx") && !wsurl.EndsWith(".svc"))
                throw new Exception("@失败:" + dbsrc.No + " 中WebService地址不正确。");

            wsurl += wsurl.EndsWith(".asmx") ? "?wsdl" : "?singleWsdl";

            #region //解析WebService所有方法列表
            //var methods = new Dictionary<string, string>(); //名称Name，全称Text
            List<WSMethod> mtds = new List<WSMethod>();
            WSMethod mtd = null;
            var wc = new WebClient();
            var stream = wc.OpenRead(wsurl);
            var sd = ServiceDescription.Read(stream);
            var eles = sd.Types.Schemas[0].Elements.Values.Cast<XmlSchemaElement>();
            var s = new StringBuilder();
            XmlSchemaComplexType ctype = null;
            XmlSchemaSequence seq = null;
            XmlSchemaElement res = null;

            foreach (var ele in eles)
            {
                if (ele == null) continue;

                var resType = string.Empty;
                var mparams = string.Empty;

                //获取接口返回元素
                res = eles.FirstOrDefault(o => o.Name == (ele.Name + "Response"));

                if (res != null)
                {
                    mtd = new WSMethod();
                    //1.接口名称 ele.Name
                    mtd.NO = ele.Name;
                    mtd.PARAMS = new Dictionary<string, string>();
                    //2.接口返回值类型
                    ctype = res.SchemaType as XmlSchemaComplexType;
                    seq = ctype.Particle as XmlSchemaSequence;

                    if (seq != null && seq.Items.Count > 0)
                        mtd.RETURN = resType = (seq.Items[0] as XmlSchemaElement).SchemaTypeName.Name;
                    else
                        continue;// resType = "void";   //去除不返回结果的接口

                    //3.接口参数
                    ctype = ele.SchemaType as XmlSchemaComplexType;
                    seq = ctype.Particle as XmlSchemaSequence;

                    if (seq != null && seq.Items.Count > 0)
                    {
                        foreach (XmlSchemaElement pe in seq.Items)
                        {
                            mparams += pe.SchemaTypeName.Name + " " + pe.Name + ", ";
                            mtd.PARAMS.Add(pe.Name, pe.SchemaTypeName.Name);
                        }

                        mparams = mparams.TrimEnd(", ".ToCharArray());
                    }

                    mtd.NAME = string.Format("{0} {1}({2})", resType, ele.Name, mparams);
                    mtds.Add(mtd);
                    //methods.Add(ele.Name, string.Format("{0} {1}({2})", resType, ele.Name, mparams));
                }
            }

            stream.Close();
            stream.Dispose();
            wc.Dispose();
            #endregion

            return mtds;
        }

        /// <summary>
        /// 生成返给前台页面的JSON字符串信息
        /// </summary>
        /// <param name="success">是否操作成功</param>
        /// <param name="msg">消息</param>
        /// <param name="haveMsgJsoned">msg是否已经JSON化</param>
        /// <returns></returns>
        private string ReturnJson(bool success, string msg, bool haveMsgJsoned)
        {
            string kh = haveMsgJsoned ? "" : "\"";
            return "{\"success\":" + success.ToString().ToLower() + ",\"msg\":" + kh + (haveMsgJsoned ? msg : msg.Replace("\"", "'")) +
                   kh + "}";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public class WSMethod
        {
            public string NO { get; set; }

            public string NAME { get; set; }

            public Dictionary<string, string> PARAMS { get; set; }

            public string RETURN { get; set; }
        }
    }
}