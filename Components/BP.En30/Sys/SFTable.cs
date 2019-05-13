using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Net;
using System.Xml;
using BP.DA;
using BP.En;
using Microsoft.CSharp;
using System.Web.Services.Description;

namespace BP.Sys
{
    /// <summary>
    /// 表数据来源类型
    /// </summary>
    public enum SrcType
    {
        /// <summary>
        /// 本地的类
        /// </summary>
        BPClass = 0,
        /// <summary>
        /// 通过ccform创建表
        /// </summary>
        CreateTable = 1,
        /// <summary>
        /// 表或视图
        /// </summary>
        TableOrView = 2,
        /// <summary>
        /// SQL查询数据
        /// </summary>
        SQL = 3,
        /// <summary>
        /// WebServices
        /// </summary>
        WebServices = 4,
        /// <summary>
        /// hand
        /// </summary>
        Handler = 5,
        /// <summary>
        /// JS请求数据.
        /// </summary>
        JQuery = 6
    }
    /// <summary>
    /// 编码表类型
    /// </summary>
    public enum CodeStruct
    {
        /// <summary>
        /// 普通的编码表
        /// </summary>
        NoName,
        /// <summary>
        /// 树编码表(No,Name,ParentNo)
        /// </summary>
        Tree,
        /// <summary>
        /// 行政机构编码表
        /// </summary>
        GradeNoName
    }
    /// <summary>
    /// 用户自定义表
    /// </summary>
    public class SFTableAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 是否可以删除
        /// </summary>
        public const string IsDel = "IsDel";
        /// <summary>
        /// 字段
        /// </summary>
        public const string FK_Val = "FK_Val";
        /// <summary>
        /// 数据表描述
        /// </summary>
        public const string TableDesc = "TableDesc";
        /// <summary>
        /// 默认值
        /// </summary>
        public const string DefVal = "DefVal";
        /// <summary>
        /// 数据源
        /// </summary>
        public const string DBSrc = "DBSrc";
        /// <summary>
        /// 是否是树
        /// </summary>
        public const string IsTree = "IsTree";
        /// <summary>
        /// 表类型
        /// </summary>
        public const string SrcType = "SrcType";
        /// <summary>
        /// 字典表类型
        /// </summary>
        public const string CodeStruct = "CodeStruct";

        #region 链接到其他系统获取数据的属性。
        /// <summary>
        /// 数据源
        /// </summary>
        public const string FK_SFDBSrc = "FK_SFDBSrc";
        /// <summary>
        /// 数据源表
        /// </summary>
        public const string SrcTable = "SrcTable";
        /// <summary>
        /// 显示的值
        /// </summary>
        public const string ColumnValue = "ColumnValue";
        /// <summary>
        /// 显示的文字
        /// </summary>
        public const string ColumnText = "ColumnText";
        /// <summary>
        /// 父结点值
        /// </summary>
        public const string ParentValue = "ParentValue";
        /// <summary>
        /// 查询语句
        /// </summary>
        public const string SelectStatement = "SelectStatement";
        /// <summary>
        /// 缓存分钟数
        /// </summary>
        public const string CashMinute = "CashMinute";
        /// <summary>
        /// 最近缓存的时间
        /// </summary>
        public const string RootVal = "RootVal";
        /// <summary>
        /// 加入日期
        /// </summary>
        public const string RDT = "RDT";
        #endregion 链接到其他系统获取数据的属性。
    }
    /// <summary>
    /// 用户自定义表
    /// </summary>
    public class SFTable : EntityNoName
    {
        #region 数据源属性.
        /// <summary>
        /// 获得外部数据表
        /// </summary>
        public System.Data.DataTable GenerHisDataTable
        {
            get
            {
                //创建数据源.
                SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);

                #region BP类
                if (this.SrcType == Sys.SrcType.BPClass)
                {
                    Entities ens = ClassFactory.GetEns(this.No);
                    return ens.RetrieveAllToTable();
                }
                #endregion

                #region  WebServices
                // this.SrcType == Sys.SrcType.WebServices，by liuxc 
                //暂只考虑No,Name结构的数据源，2015.10.04，added by liuxc
                if (this.SrcType == Sys.SrcType.WebServices)
                {
                    var td = this.TableDesc.Split(','); //接口名称,返回类型
                    var ps = (this.SelectStatement ?? string.Empty).Split('&');
                    var args = new ArrayList();
                    string[] pa = null;

                    foreach (var p in ps)
                    {
                        if (string.IsNullOrWhiteSpace(p)) continue;

                        pa = p.Split('=');
                        if (pa.Length != 2) continue;

                        //此处要SL中显示表单时，会有问题
                        try
                        {
                            if (pa[1].Contains("@WebUser.No"))
                                pa[1] = pa[1].Replace("@WebUser.No", BP.Web.WebUser.No);
                            if (pa[1].Contains("@WebUser.Name"))
                                pa[1] = pa[1].Replace("@WebUser.Name", BP.Web.WebUser.Name);
                            if (pa[1].Contains("@WebUser.FK_Dept"))
                                pa[1] = pa[1].Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                            if (pa[1].Contains("@WebUser.FK_DeptName"))
                                pa[1] = pa[1].Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                        }
                        catch
                        { }

                        if (pa[1].Contains("@WorkID"))
                            pa[1] = pa[1].Replace("@WorkID", BP.Sys.Glo.Request["WorkID"] ?? "");
                        if (pa[1].Contains("@NodeID"))
                            pa[1] = pa[1].Replace("@NodeID", BP.Sys.Glo.Request["NodeID"] ?? "");
                        if (pa[1].Contains("@FK_Node"))
                            pa[1] = pa[1].Replace("@FK_Node", BP.Sys.Glo.Request["FK_Node"] ?? "");
                        if (pa[1].Contains("@FK_Flow"))
                            pa[1] = pa[1].Replace("@FK_Flow", BP.Sys.Glo.Request["FK_Flow"] ?? "");
                        if (pa[1].Contains("@FID"))
                            pa[1] = pa[1].Replace("@FID", BP.Sys.Glo.Request["FID"] ?? "");

                        args.Add(pa[1]);
                    }

                    var result = InvokeWebService(src.IP, td[0], args.ToArray());

                    switch (td[1])
                    {
                        case "DataSet":
                            return result == null ? new DataTable() : (result as DataSet).Tables[0];
                        case "DataTable":
                            return result as DataTable;
                        case "Json":
                            var jdata = LitJson.JsonMapper.ToObject(result as string);

                            if (!jdata.IsArray)
                                throw new Exception("@返回的JSON格式字符串“" + (result ?? string.Empty) + "”不正确");

                            var dt = new DataTable();
                            dt.Columns.Add("No", typeof(string));
                            dt.Columns.Add("Name", typeof(string));

                            for (var i = 0; i < jdata.Count; i++)
                            {
                                dt.Rows.Add(jdata[i]["No"].ToString(), jdata[i]["Name"].ToString());
                            }

                            return dt;
                        case "Xml":
                            if (result == null || string.IsNullOrWhiteSpace(result.ToString()))
                                throw new Exception("@返回的XML格式字符串不正确。");

                            var xml = new XmlDocument();
                            xml.LoadXml(result as string);

                            XmlNode root = null;

                            if (xml.ChildNodes.Count < 2)
                                root = xml.ChildNodes[0];
                            else
                                root = xml.ChildNodes[1];

                            dt = new DataTable();
                            dt.Columns.Add("No", typeof(string));
                            dt.Columns.Add("Name", typeof(string));

                            foreach (XmlNode node in root.ChildNodes)
                            {
                                dt.Rows.Add(node.SelectSingleNode("No").InnerText,
                                            node.SelectSingleNode("Name").InnerText);
                            }

                            return dt;
                        default:
                            throw new Exception("@不支持的返回类型" + td[1]);
                    }
                }
                #endregion

                #region SQL查询.外键表/视图，edited by liuxc,2016-12-29
                if (this.SrcType == Sys.SrcType.TableOrView)
                {
                    string sql = "SELECT " + this.ColumnValue + " No, " + this.ColumnText + " Name";
                    if (this.CodeStruct == Sys.CodeStruct.Tree)
                        sql += ", " + this.ParentValue + " ParentNo";

                    sql += " FROM " + this.SrcTable;
                    return src.RunSQLReturnTable(sql);
                }
                #endregion SQL查询.外键表/视图，edited by liuxc,2016-12-29


                #region 动态SQL，edited by liuxc,2016-12-29
                if (this.SrcType == Sys.SrcType.SQL)
                {
                    string runObj = this.SelectStatement;

                    if (DataType.IsNullOrEmpty(runObj))
                        throw new Exception("@外键类型SQL配置错误," + this.No + " " + this.Name + " 是一个(SQL)类型(" + this.GetValStrByKey("SrcType") + ")，但是没有配置sql.");

                    if (runObj == null)
                        runObj = string.Empty;

                    runObj = runObj.Replace("~", "'");
                    if (runObj.Contains("@WebUser.No"))
                        runObj = runObj.Replace("@WebUser.No", BP.Web.WebUser.No);

                    if (runObj.Contains("@WebUser.Name"))
                        runObj = runObj.Replace("@WebUser.Name", BP.Web.WebUser.Name);

                    if (runObj.Contains("@WebUser.FK_Dept"))
                        runObj = runObj.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

                    return src.RunSQLReturnTable(runObj);
                }
                #endregion

                #region 自定义表.
                if (this.SrcType == Sys.SrcType.CreateTable)
                {
                    string sql = "SELECT No, Name FROM " + this.No;
                    return src.RunSQLReturnTable(sql);
                }
                #endregion

                return null;

                //throw new Exception("@没有判断的数据类型." + this.SrcType + " - " + this.SrcTypeText);
            }
        }
        /// <summary>
        /// 自动生成编号
        /// </summary>
        /// <returns></returns>
        public string GenerSFTableNewNo()
        {
            string table = this.SrcTable;
            try
            {
                string sql = null;
                string field = "No";
                switch (this.EnMap.EnDBUrl.DBType)
                {
                    case DBType.MSSQL:
                        sql = "SELECT CONVERT(INT, MAX(CAST(" + field + " as int)) )+1 AS No FROM " + table;
                        break;
                    case DBType.PostgreSQL:
                        sql = "SELECT to_number( MAX(" + field + ") ,'99999999')+1   FROM " + table;
                        break;
                    case DBType.Oracle:
                        sql = "SELECT MAX(" + field + ") +1 AS No FROM " + table;
                        break;
                    case DBType.MySQL:
                        sql = "SELECT CONVERT(MAX(CAST(" + field + " AS SIGNED INTEGER)),SIGNED) +1 AS No FROM " + table;
                        break;
                    case DBType.Informix:
                        sql = "SELECT MAX(" + field + ") +1 AS No FROM " + table;
                        break;
                    case DBType.Access:
                        sql = "SELECT MAX( [" + field + "]) +1 AS  No FROM " + table;
                        break;
                    default:
                        throw new Exception("error");
                }
                string str = DBAccess.RunSQLReturnValInt(sql, 1).ToString();
                if (str == "0" || str == "")
                    str = "1";
                return str.PadLeft(3, '0');
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// 实例化 WebServices
        /// </summary>
        /// <param name="url">WebServices地址</param>
        /// <param name="methodname">调用的方法</param>
        /// <param name="args">把webservices里需要的参数按顺序放到这个object[]里</param>
        public object InvokeWebService(string url, string methodname, object[] args)
        {

            //这里的namespace是需引用的webservices的命名空间，在这里是写死的，大家可以加一个参数从外面传进来。
            string @namespace = "BP.RefServices";
            try
            {
                if (url.EndsWith(".asmx"))
                    url += "?wsdl";
                else if (url.EndsWith(".svc"))
                    url += "?singleWsdl";

                //获取WSDL
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url);
                ServiceDescription sd = ServiceDescription.Read(stream);
                string classname = sd.Services[0].Name;
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);

                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider csc = new CSharpCodeProvider();
                ICodeCompiler icc = csc.CreateCompiler();

                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //编译代理类
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }

                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);

                return mi.Invoke(obj, args);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region 链接到其他系统获取数据的属性
        /// <summary>
        /// 数据源
        /// </summary>
        public string FK_SFDBSrc
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.FK_SFDBSrc);
            }
            set
            {
                this.SetValByKey(SFTableAttr.FK_SFDBSrc, value);
            }
        }
        public string FK_SFDBSrcT
        {
            get
            {
                return this.GetValRefTextByKey(SFTableAttr.FK_SFDBSrc);
            }
        }
        /// <summary>
        /// 数据缓存时间
        /// </summary>
        public string RootVal
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.RootVal);
            }
            set
            {
                this.SetValByKey(SFTableAttr.RootVal, value);
            }
        }
        /// <summary>
        /// 同步间隔
        /// </summary>
        public int CashMinute
        {
            get
            {
                return this.GetValIntByKey(SFTableAttr.CashMinute);
            }
            set
            {
                this.SetValByKey(SFTableAttr.CashMinute, value);
            }
        }

        /// <summary>
        /// 物理表名称
        /// </summary>
        public string SrcTable
        {
            get
            {
                string str = this.GetValStringByKey(SFTableAttr.SrcTable);
                if (str == "" || str == null)
                    return this.No;
                return str;
            }
            set
            {
                this.SetValByKey(SFTableAttr.SrcTable, value);
            }
        }
        /// <summary>
        /// 值/主键字段名
        /// </summary>
        public string ColumnValue
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.ColumnValue);
            }
            set
            {
                this.SetValByKey(SFTableAttr.ColumnValue, value);
            }
        }
        /// <summary>
        /// 显示字段/显示字段名
        /// </summary>
        public string ColumnText
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.ColumnText);
            }
            set
            {
                this.SetValByKey(SFTableAttr.ColumnText, value);
            }
        }
        /// <summary>
        /// 父结点字段名
        /// </summary>
        public string ParentValue
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.ParentValue);
            }
            set
            {
                this.SetValByKey(SFTableAttr.ParentValue, value);
            }
        }
        /// <summary>
        /// 查询语句
        /// </summary>
        public string SelectStatement
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.SelectStatement);
            }
            set
            {
                this.SetValByKey(SFTableAttr.SelectStatement, value);
            }
        }
        /// <summary>
        /// 加入日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.RDT);
            }
            set
            {
                this.SetValByKey(SFTableAttr.RDT, value);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否是类
        /// </summary>
        public bool IsClass
        {
            get
            {
                if (this.No.Contains("."))
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是否是树形实体?
        /// </summary>
        public bool IsTree
        {
            get
            {
                if (this.CodeStruct == Sys.CodeStruct.NoName)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 数据源类型
        /// </summary>
        public SrcType SrcType
        {
            get
            {
                if (this.No.Contains("BP.") == true)
                    return SrcType.BPClass;
                else
                {
                    SrcType src = (SrcType)this.GetValIntByKey(SFTableAttr.SrcType);
                    if (src == Sys.SrcType.BPClass)
                        return Sys.SrcType.CreateTable;
                    return src;
                }
            }
            set
            {
                this.SetValByKey(SFTableAttr.SrcType, (int)value);
            }
        }
        /// <summary>
        /// 数据源类型名称
        /// </summary>
        public string SrcTypeText
        {
            get
            {
                switch (this.SrcType)
                {
                    case Sys.SrcType.TableOrView:
                        if (this.IsClass)
                            return "<img src='/WF/Img/Class.png' width='16px' broder='0' />实体类";
                        else
                            return "<img src='/WF/Img/Table.gif' width='16px' broder='0' />表/视图";
                    case Sys.SrcType.SQL:
                        return "<img src='/WF/Img/SQL.png' width='16px' broder='0' />SQL表达式";
                    case Sys.SrcType.WebServices:
                        return "<img src='/WF/Img/WebServices.gif' width='16px' broder='0' />WebServices";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 字典表类型
        /// <para>0：NoName类型</para>
        /// <para>1：NoNameTree类型</para>
        /// <para>2：NoName行政区划类型</para>
        /// </summary>
        public CodeStruct CodeStruct
        {
            get
            {
                return (CodeStruct)this.GetValIntByKey(SFTableAttr.CodeStruct);
            }
            set
            {
                this.SetValByKey(SFTableAttr.CodeStruct, (int)value);
            }
        }
        /// <summary>
        /// 编码类型
        /// </summary>
        public string CodeStructT
        {
            get
            {
                return this.GetValRefTextByKey(SFTableAttr.CodeStruct);
            }
        }
        /// <summary>
        /// 值
        /// </summary>
        public string FK_Val
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.FK_Val);
            }
            set
            {
                this.SetValByKey(SFTableAttr.FK_Val, value);
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string TableDesc
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.TableDesc);
            }
            set
            {
                this.SetValByKey(SFTableAttr.TableDesc, value);
            }
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefVal
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.DefVal);
            }
            set
            {
                this.SetValByKey(SFTableAttr.DefVal, value);
            }
        }
        public EntitiesNoName HisEns
        {
            get
            {
                if (this.IsClass)
                {
                    EntitiesNoName ens = (EntitiesNoName)BP.En.ClassFactory.GetEns(this.No);
                    ens.RetrieveAll();
                    return ens;
                }

                BP.En.GENoNames ges = new GENoNames(this.No, this.Name);
                ges.RetrieveAll();
                return ges;
            }
        }
        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 用户自定义表
        /// </summary>
        public SFTable()
        {
        }
        public SFTable(string mypk)
        {
            this.No = mypk;
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
                switch (this.No)
                {
                    case "BP.Pub.NYs":
                        this.Name = "年月";
                        this.FK_Val = "FK_NY";
                        this.Insert();
                        break;
                    case "BP.Pub.YFs":
                        this.Name = "月";
                        this.FK_Val = "FK_YF";
                        this.Insert();
                        break;
                    case "BP.Pub.Days":
                        this.Name = "天";
                        this.FK_Val = "FK_Day";
                        this.Insert();
                        break;
                    case "BP.Pub.NDs":
                        this.Name = "年";
                        this.FK_Val = "FK_ND";
                        this.Insert();
                        break;
                    default:
                        throw new Exception(ex.Message);
                }
            }
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_SFTable", "字典表");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddTBStringPK(SFTableAttr.No, null, "表英文名称", true, false, 1, 200, 20);
                map.AddTBString(SFTableAttr.Name, null, "表中文名称", true, false, 0, 200, 20);

                map.AddDDLSysEnum(SFTableAttr.SrcType, 0, "数据表类型", true, false, SFTableAttr.SrcType,
                    "@0=本地的类@1=创建表@2=表或视图@3=SQL查询表@4=WebServices@5=微服务Handler外部数据源@6=JavaScript外部数据源@7=动态Json");

                map.AddDDLSysEnum(SFTableAttr.CodeStruct, 0, "字典表类型", true, false, SFTableAttr.CodeStruct);
                map.AddTBString(SFTableAttr.RootVal, null, "根节点值", false, false, 0, 200, 20);


                map.AddTBString(SFTableAttr.FK_Val, null, "默认创建的字段名", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.TableDesc, null, "表描述", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.DefVal, null, "默认值", true, false, 0, 200, 20);


                //数据源.
                map.AddDDLEntities(SFTableAttr.FK_SFDBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);

                map.AddTBString(SFTableAttr.SrcTable, null, "数据源表", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ColumnValue, null, "显示的值(编号列)", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ColumnText, null, "显示的文字(名称列)", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ParentValue, null, "父级值(父级列)", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.SelectStatement, null, "查询语句", false, false, 0, 1000, 600, true);

                map.AddTBDateTime(SFTableAttr.RDT, null, "加入日期", false, false);

                //查找.
                map.AddSearchAttr(SFTableAttr.FK_SFDBSrc);

                RefMethod rm = new RefMethod();
                rm.Title = "查看数据";
                rm.ClassMethodName = this.ToString() + ".DoEdit";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = false;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "创建Table向导";
                //rm.ClassMethodName = this.ToString() + ".DoGuide";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.IsForEns = false;
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "数据源管理";
                //rm.ClassMethodName = this.ToString() + ".DoMangDBSrc";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.IsForEns = false;
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 数据源管理
        /// </summary>
        /// <returns></returns>
        public string DoMangDBSrc()
        {
            return SystemConfig.CCFlowWebPath + "WF/Comm/Sys/SFDBSrcNewGuide.htm";
        }
        /// <summary>
        /// 创建表向导
        /// </summary>
        /// <returns></returns>
        public string DoGuide()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/CreateSFGuide.htm";
        }
        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <returns></returns>
        public string DoEdit()
        {
            if (this.IsClass)
                return SystemConfig.CCFlowWebPath + "WF/Comm/Ens.htm?EnsName=" + this.No;
            else
                return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/SFTableEditData.htm?FK_SFTable=" + this.No;
        }
        protected override bool beforeDelete()
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.UIBindKey, this.No);
            if (attrs.Count != 0)
            {
                string err = "";
                foreach (MapAttr item in attrs)
                    err += " @ " + item.MyPK + " " + item.Name;
                throw new Exception("@如下实体字段在引用:" + err + "。您不能删除该表。");
            }
            return base.beforeDelete();
        }
        protected override bool beforeInsert()
        {
            //利用这个时间串进行排序.
            this.RDT = DataType.CurrentDataTime;


            #region 如果是本地类. @于庆海.
            if (this.SrcType == Sys.SrcType.BPClass)
            {
                Entities ens = ClassFactory.GetEns(this.No);
                Entity en = ens.GetNewEntity;
                this.Name = en.EnDesc;

                //检查是否是树结构.
                if (en.IsTreeEntity == true)
                    this.CodeStruct = Sys.CodeStruct.Tree;
                else
                    this.CodeStruct = Sys.CodeStruct.NoName;
            }
            #endregion 如果是本地类.

            #region 本地类，物理表..
            if (this.SrcType == Sys.SrcType.CreateTable)
            {
                if (DBAccess.IsExitsObject(this.No) == true)
                {
                    return base.beforeInsert();
                    //throw new Exception("err@表名[" + this.No + "]已经存在，请使用其他的表名.");
                }

                string sql = "";
                if (this.CodeStruct == Sys.CodeStruct.NoName || this.CodeStruct == Sys.CodeStruct.GradeNoName)
                {
                    sql = "CREATE TABLE " + this.No + " (";
                    sql += "No varchar(30) NOT NULL,";
                    sql += "Name varchar(3900) NULL";
                    sql += ")";
                }

                if (this.CodeStruct == Sys.CodeStruct.Tree)
                {
                    sql = "CREATE TABLE " + this.No + " (";
                    sql += "No varchar(30) NOT NULL,";
                    sql += "Name varchar(3900)  NULL,";
                    sql += "ParentNo varchar(3900)  NULL";
                    sql += ")";
                }
                this.RunSQL(sql);

                //初始化数据.
                this.InitDataTable();
            }
            #endregion 如果是本地类.

            return base.beforeInsert();
        }

        protected override void afterInsert()
        {
            try
            {
                if (this.SrcType == Sys.SrcType.TableOrView)
                {
                    //暂时这样处理
                    string sql = "CREATE VIEW " + this.No + " (";
                    sql += "[No],";
                    sql += "[Name]";
                    sql += (this.CodeStruct == Sys.CodeStruct.Tree ? ",[ParentNo])" : ")");
                    sql += " AS ";
                    sql += "SELECT " + this.ColumnValue + " No," + this.ColumnText + " Name" + (this.CodeStruct == Sys.CodeStruct.Tree ? ("," + this.ParentValue + " ParentNo") : "") + " FROM " + this.SrcTable + (string.IsNullOrWhiteSpace(this.SelectStatement) ? "" : (" WHERE " + this.SelectStatement));

                    if (Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
                    {
                        sql = sql.Replace("[", "`").Replace("]", "`");
                    }
                    else
                    {
                        sql = sql.Replace("[", "").Replace("]", "");
                    }
                    this.RunSQL(sql);
                }

                //if (this.SrcType == Sys.SrcType.SQL)
                //{
                //    //暂时这样处理
                //    string sql = "CREATE VIEW " + this.No + " (";
                //    sql += "[No],";
                //    sql += "[Name]";
                //    sql += (this.CodeStruct == Sys.CodeStruct.Tree ? ",[ParentNo])" : ")");
                //    sql += " AS ";
                //    sql += this.SelectStatement;

                //    if (Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
                //    {
                //        sql = sql.Replace("[", "`").Replace("]", "`");
                //    }
                //    else
                //    {
                //        sql = sql.Replace("[", "").Replace("]", "");
                //    }
                //    this.RunSQL(sql);
                //}
            }
            catch (Exception ex)
            {
                //创建视图失败时，删除此记录，并提示错误
                this.DirectDelete();
                throw ex;
            }

            base.afterInsert();
        }

        /// <summary>
        /// 获得该数据源的数据
        /// </summary>
        /// <returns></returns>
        public DataTable GenerData_bak()
        {
            string sql = "";
            DataTable dt = null;
            if (this.SrcType == Sys.SrcType.CreateTable)
            {
                sql = "SELECT No,Name FROM " + this.SrcTable;
                dt = this.RunSQLReturnTable(sql);
            }

            if (this.SrcType == Sys.SrcType.TableOrView)
            {
                sql = "SELECT No,Name FROM " + this.SrcTable;
                dt = this.RunSQLReturnTable(sql);
            }

            if (dt == null)
                throw new Exception("@没有判断的数据.");

            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";

            return dt;
        }
        /// <summary>
        /// 返回json.
        /// </summary>
        /// <returns></returns>
        public string GenerDataOfJson()
        {
            return BP.Tools.Json.ToJson(this.GenerHisDataTable);
        }

        /// <summary>
        /// 初始化数据.
        /// </summary>
        public void InitDataTable()
        {
            DataTable dt = this.GenerHisDataTable;

            string sql = "";
            if (dt.Rows.Count == 0)
            {
                /*初始化数据.*/
                if (this.CodeStruct == Sys.CodeStruct.Tree)
                {
                    sql = "INSERT INTO " + this.SrcTable + " (No,Name,ParentNo) VALUES('1','" + this.Name + "','0') ";
                    this.RunSQL(sql);

                    for (int i = 1; i < 4; i++)
                    {
                        string no = i.ToString();
                        no = no.PadLeft(3, '0');

                        sql = "INSERT INTO " + this.SrcTable + " (No,Name,ParentNo) VALUES('" + no + "','Item" + no + "','1') ";
                        this.RunSQL(sql);
                    }
                }

                if (this.CodeStruct == Sys.CodeStruct.NoName)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        string no = i.ToString();
                        no = no.PadLeft(3, '0');
                        sql = "INSERT INTO " + this.SrcTable + " (No,Name) VALUES('" + no + "','Item" + no + "') ";
                        this.RunSQL(sql);
                    }
                }
            }
        }

    }
    /// <summary>
    /// 用户自定义表s
    /// </summary>
    public class SFTables : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 用户自定义表s
        /// </summary>
        public SFTables()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFTable();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SFTable> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFTable>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFTable> Tolist()
        {
            System.Collections.Generic.List<SFTable> list = new System.Collections.Generic.List<SFTable>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFTable)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
