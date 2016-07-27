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
        BPClass,
        /// <summary>
        /// 新建表
        /// </summary>
        NewTable,
        /// <summary>
        /// 本地表或者视图
        /// </summary>
        TableOrView,
        /// <summary>
        /// 通过一个SQL确定的一个外部数据源
        /// </summary>
        SQL,
        /// <summary>
        /// 通过WebServices获得的一个数据源
        /// </summary>
        WebServices
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
        public const string CashDT = "CashDT";
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

                #region 如果是一个SQL.
                if (this.SrcType == Sys.SrcType.SQL)
                {
                    string runObj = this.SelectStatement;
                    runObj = runObj.Replace("~", "'");
                    if (runObj.Contains("@WebUser.No"))
                        runObj = runObj.Replace("@WebUser.No", BP.Web.WebUser.No);

                    if (runObj.Contains("@WebUser.Name"))
                        runObj = runObj.Replace("@WebUser.Name", BP.Web.WebUser.Name);

                    if (runObj.Contains("@WebUser.FK_Dept"))
                        runObj = runObj.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                    return src.RunSQLReturnTable(runObj);
                }
                #endregion 如果是一个SQL.

                #region 如果是一个外键表.
                if (this.SrcType == Sys.SrcType.TableOrView)
                {
                    /*如果是表或者视图*/
                    if (this.IsClass)
                    {
                        /*如果是一个类*/
                        Entities ens = ClassFactory.GetEns(this.No);
                        return  ens.RetrieveAllToTable();
                    }
                    else
                    {
                        string sql = "SELECT No, Name FROM "+this.No;
                        return src.RunSQLReturnTable(sql);
                    }
                }
                #endregion 如果是一个SQL.

                throw new Exception("@没有判断的数据类型.");
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
        public string CashDT
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.CashDT);
            }
            set
            {
                this.SetValByKey(SFTableAttr.CashDT, value);
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
                return this.GetValStringByKey(SFTableAttr.SrcTable);
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
                return (SrcType)this.GetValIntByKey(SFTableAttr.SrcType);
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
                Map map = new Map("Sys_SFTable");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "字典表";
                map.EnType = EnType.Sys;


                map.AddTBStringPK(SFTableAttr.No, null, "表英文名称", true, false, 1, 200, 20);
                map.AddTBString(SFTableAttr.Name, null, "表中文名称", true, false, 0, 200, 20);

                map.AddDDLSysEnum(SFTableAttr.SrcType, 0, "数据表类型", true, false, SFTableAttr.SrcType,
                    "@0=外键表@1=外部表(SQL表)@2=WebService表(通过WS服务表)");

                map.AddDDLSysEnum(SFTableAttr.CodeStruct, 0, "字典表类型", true, false, SFTableAttr.CodeStruct);

                map.AddTBString(SFTableAttr.FK_Val, null, "默认创建的字段名", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.TableDesc, null, "表描述", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.DefVal, null, "默认值", true, false, 0, 200, 20);

                //与同步相关的数据.
                map.AddTBString(SFTableAttr.CashDT, null, "上次同步的时间", false, false, 0, 200, 20);
                map.AddTBInt(SFTableAttr.CashMinute, 0, "数据缓存时间(0表示不缓存)", false, false);

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

                rm = new RefMethod();
                rm.Title = "创建Table向导";
                rm.ClassMethodName = this.ToString() + ".DoGuide";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = false;
                map.AddRefMethod(rm);

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
            return SystemConfig.CCFlowWebPath + "WF/Comm/Sys/SFDBSrcNewGuide.aspx";
        }
        /// <summary>
        /// 创建表向导
        /// </summary>
        /// <returns></returns>
        public string DoGuide()
        {
            return SystemConfig.CCFlowWebPath + "WF/Comm/Sys/SFGuide.aspx";
        }
        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <returns></returns>
        public string DoEdit()
        {
            if (this.IsClass)
                return SystemConfig.CCFlowWebPath + "WF/Comm/Ens.aspx?EnsName=" + this.No;
            else
                return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/SFTableEditData.aspx?FK_SFTable=" + this.No;
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
            return base.beforeInsert();
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
