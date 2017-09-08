using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using System.Collections;
using System.Net;
using System.Xml.Schema;
using System.Web.Services.Description;
using System.Linq;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Comm_Sys : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Comm_Sys(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            string sfno = context.Request.QueryString["sfno"];
            SFTable sftable = null;
            DataTable dt = null;
            StringBuilder s = null;

            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                case "sfguide_getinfo": //获取数据源字典表信息
                    if (string.IsNullOrWhiteSpace(sfno))
                        return "err@参数不正确";

                    sftable = new SFTable(sfno);
                    dt = sftable.ToDataTableField("info");

                    foreach (DataColumn col in dt.Columns)
                        col.ColumnName = col.ColumnName.ToUpper();

                    return BP.Tools.Json.ToJson(dt);
                case "sfguide_saveinfo":    //保存
                    bool isnew = Convert.ToBoolean(context.Request.QueryString["isnew"]);
                    sfno = context.Request.QueryString["NO"];
                    string myname = context.Request.QueryString["NAME"];
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
                        return "err@字典编号" + sfno + "已经存在，不允许重复。";

                    sftable.Name = myname;
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

                    sftable.Save();
                    return "保存成功！";

                case "sfguide_getclass": //获取类列表
                    string stru = context.Request.QueryString["struct"];
                    int st = 0;

                    if (string.IsNullOrWhiteSpace(stru) || !int.TryParse(stru, out st))
                        throw new Exception("err@参数不正确.");

                    string error = string.Empty;
                    ArrayList arr = null;
                    SFTables sfs = new SFTables();
                    Entities ens = null;
                    SFTable sf = null;
                    sfs.Retrieve(SFTableAttr.SrcType, (int)SrcType.BPClass);

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
                            continue;
                        }
                    }
                    return s.ToString().TrimEnd(',') + "]";
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

                    if (onlyWS == false)
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
                    return BP.Tools.Json.ToJson(dt);

                case "sfguide_gettvs": //获取表/视图列表
                    string src = context.Request.QueryString["src"];

                    SFDBSrc sr = new SFDBSrc(src);
                    dt = sr.GetTables();

                    foreach (DataColumn col in dt.Columns)
                        col.ColumnName = col.ColumnName.ToUpper();

                    return BP.Tools.Json.ToJson(dt);
                case "sfguide_getcols": //获取表/视图的列信息
                    src = context.Request.QueryString["src"];
                    string table = context.Request.QueryString["table"];

                    if (string.IsNullOrWhiteSpace(src))
                        throw new Exception("err@参数不正确");


                    if (string.IsNullOrWhiteSpace(table))
                    {
                        return "[]";
                    }

                    sr = new SFDBSrc(src);
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

                    return BP.Tools.Json.ToJson(dt);

                case "sfguide_getmtds": //获取WebService方法列表
                    src = context.Request.QueryString["src"];
                    if (string.IsNullOrWhiteSpace(src))
                        return "err@系统中没有webservices类型的数据源，该类型的外键表不能创建，请维护数据源.";

                    sr = new SFDBSrc(src);

                    if (sr.DBSrcType != DBSrcType.WebServices)
                        return "err@数据源“" + sr.Name + "”不是WebService数据源.";

                    List<WSMethod> mtds = GetWebServiceMethods(sr);

                    return LitJson.JsonMapper.ToJson(mtds);
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.


        #region 数据源管理
        public string SFDBSrcNewGuide_GetList()
        {
            //SysEnums enums = new SysEnums(SFDBSrcAttr.DBSrcType);
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();

            return srcs.ToJson();
        }

        public string SFDBSrcNewGuide_LoadSrc()
        {
            DataSet ds = new DataSet();

            SFDBSrc src = new SFDBSrc();
            if (!string.IsNullOrWhiteSpace(this.GetRequestVal("No")))
                src = new SFDBSrc(No);
            ds.Tables.Add(src.ToDataTableField("SFDBSrc"));

            SysEnums enums = new SysEnums();
            enums.Retrieve(SysEnumAttr.EnumKey, SFDBSrcAttr.DBSrcType, SysEnumAttr.IntKey);
            ds.Tables.Add(enums.ToDataTableField("DBSrcType"));

            return BP.Tools.Json.ToJson(ds);
        }

        public string SFDBSrcNewGuide_SaveSrc()
        {
            SFDBSrc src = new SFDBSrc();
            src.No = this.GetRequestVal("TB_No");
            if (src.RetrieveFromDBSources() > 0 && this.GetRequestVal("NewOrEdit") == "New")
            {
                return ("已经存在数据源编号为“" + src.No + "”的数据源，编号不能重复！");
            }
            src.Name = this.GetRequestVal("TB_Name");
            src.DBSrcType = (DBSrcType)this.GetRequestValInt("DDL_DBSrcType");
            switch (src.DBSrcType)
            {
                case DBSrcType.SQLServer:
                case DBSrcType.Oracle:
                case DBSrcType.MySQL:
                case DBSrcType.Informix:
                    if (src.DBSrcType != DBSrcType.Oracle)
                        src.DBName = this.GetRequestVal("TB_DBName");
                    else
                        src.DBName = string.Empty;
                    src.IP = this.GetRequestVal("TB_IP");
                    src.UserID = this.GetRequestVal("TB_UserID");
                    src.Password = this.GetRequestVal("TB_Password");
                    break;
                case DBSrcType.WebServices:
                    src.DBName = string.Empty;
                    src.IP = this.GetRequestVal("TB_IP");
                    src.UserID = string.Empty;
                    src.Password = string.Empty;
                    break;
                default:
                    break;
            }
            //测试是否连接成功，如果连接不成功，则不允许保存。
            string testResult = src.DoConn();

            if (testResult.IndexOf("连接配置成功") == -1)
            {
                return (testResult + ".保存失败！");
            }

            src.Save();

            return "保存成功..";
        }

        public string SFDBSrcNewGuide_DelSrc()
        {
            string no = this.GetRequestVal("No");

            //检验要删除的数据源是否有引用
            SFTables sfs = new SFTables();
            sfs.Retrieve(SFTableAttr.FK_SFDBSrc, no);

            if (sfs.Count > 0)
            {
                //Alert("当前数据源已经使用，不能删除！");
                return "当前数据源已经使用，不能删除！";
                //return;
            }
            SFDBSrc src = new SFDBSrc(no);
            src.Delete();
            return "删除成功..";
        }
        #endregion

        #region Methods
        /// <summary>
        /// 获取webservice方法列表
        /// </summary>
        /// <param name="dbsrc">WebService数据源</param>
        /// <returns></returns>
        public List<WSMethod> GetWebServiceMethods(SFDBSrc dbsrc)
        {
            if (dbsrc == null || string.IsNullOrWhiteSpace(dbsrc.IP))
                return new List<WSMethod>();

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
        #endregion
    }
}
