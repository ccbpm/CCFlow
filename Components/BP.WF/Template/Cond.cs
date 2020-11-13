using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Data;
using BP.Web;

namespace BP.WF.Template
{
    /// <summary>
    /// 条件属性
    /// </summary>
    public class CondAttr
    {
        /// <summary>
        /// 关联的流程编号
        /// </summary>
        public const string RefFlowNo = "RefFlowNo";
        /// <summary>
        /// 数据来源
        /// </summary>
        public const string DataFrom = "DataFrom";
        /// <summary>
        /// 属性Key
        /// </summary>
        public const string AttrKey = "AttrKey";
        /// <summary>
        /// 名称
        /// </summary>
        public const string AttrName = "AttrName";
        /// <summary>
        /// 属性
        /// </summary>
        public const string FK_Attr = "FK_Attr";
        /// <summary>
        /// 运算符号
        /// </summary>
        public const string FK_Operator = "FK_Operator";
        /// <summary>
        /// 运算的值
        /// </summary>
        public const string OperatorValue = "OperatorValue";
        /// <summary>
        /// 操作值
        /// </summary>
        public const string OperatorValueT = "OperatorValueT";
        /// <summary>
        /// Node
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 条件类型
        /// </summary>
        public const string CondType = "CondType";
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 启动的子流程(对子流程有效)
        /// </summary>
        public const string SubFlowNo = "SubFlowNo";
        /// <summary>
        /// 对方向条件有效
        /// </summary>
        public const string ToNodeID = "ToNodeID";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";

        #region 属性。
        /// <summary>
        /// 指定人员方式
        /// </summary>
        public const string SpecOperWay = "SpecOperWay";
        /// <summary>
        /// 指定人员方式的参数
        /// </summary>
        public const string SpecOperPara = "SpecOperPara";
        #endregion 属性。
    }
    /// <summary>
    /// 条件
    /// </summary>
    public class Cond : EntityMyPK
    {
        #region 参数属性.
        /// <summary>
        /// 指定人员方式
        /// </summary>
        public SpecOperWay SpecOperWay
        {
            get
            {
                return (SpecOperWay)this.GetParaInt(CondAttr.SpecOperWay);
            }
            set
            {
                this.SetPara(CondAttr.SpecOperWay, (int)value);
            }
        }
        /// <summary>
        /// 指定人员参数
        /// </summary>
        public string SpecOperPara
        {
            get
            {
                return this.GetParaString(CondAttr.SpecOperPara);
            }
            set
            {
                this.SetPara(CondAttr.SpecOperPara, value);
            }
        }
        /// <summary>
        /// 求指定的人员.
        /// </summary>
        public string SpecOper
        {
            get
            {
                var way = this.SpecOperWay;
                if (way == Template.SpecOperWay.CurrOper)
                    return BP.Web.WebUser.No;


                if (way == Template.SpecOperWay.SpecNodeOper)
                {
                    string sql = "SELECT FK_Emp FROM WF_GenerWorkerlist WHERE FK_Node=" + this.SpecOperPara + " AND WorkID=" + this.WorkID;
                    string fk_emp = DBAccess.RunSQLReturnStringIsNull(sql, null);
                    if (fk_emp == null)
                        throw new Exception("@您在配置方向条件时错误，求指定的人员的时候，按照指定的节点[" + this.SpecOperPara + "]，作为处理人，但是该节点上没有人员。查询SQL:" + sql);
                    return fk_emp;
                }

                if (way == Template.SpecOperWay.SpecSheetField)
                {
                    if (this.en.Row.ContainsKey(this.SpecOperPara.Replace("@", "")) == false)
                        throw new Exception("@您在配置方向条件时错误，求指定的人员的时候，按照指定的字段[" + this.SpecOperPara + "]作为处理人，但是该字段不存在。");

                    return this.en.GetValStringByKey(this.SpecOperPara.Replace("@", ""));
                }

                if (way == Template.SpecOperWay.CurrOper)
                {
                    if (this.en.Row.ContainsKey(this.SpecOperPara.Replace("@", "")) == false)
                        throw new Exception("@您在配置方向条件时错误，求指定的人员的时候，按照指定的字段[" + this.SpecOperPara + "]作为处理人，但是该字段不存在。");

                    return this.en.GetValStringByKey(this.SpecOperPara.Replace("@", ""));
                }

                if (way == Template.SpecOperWay.SpenEmpNo)
                {
                    if (DataType.IsNullOrEmpty(this.SpecOperPara) == false)
                        throw new Exception("@您在配置方向条件时错误，求指定的人员的时候，按照指定的人员[" + this.SpecOperPara + "]作为处理人，但是人员参数没有设置。");
                    return this.SpecOperPara;
                }

                throw new Exception("@配置异常，没有判断的条件类型。");
            }
        }
        #endregion 参数属性.

        #region 基本属性.
        public GERpt en = null;
        /// <summary>
        /// 数据来源
        /// </summary>
        public ConnDataFrom HisDataFrom
        {
            get
            {
                return (ConnDataFrom)this.GetValIntByKey(CondAttr.DataFrom);
            }
            set
            {
                this.SetValByKey(CondAttr.DataFrom, (int)value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(CondAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(CondAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 隶属流程编号，用于备份删除.
        /// </summary>
        public string RefFlowNo
        {
            get
            {
                return this.GetValStringByKey(CondAttr.RefFlowNo);
            }
            set
            {
                this.SetValByKey(CondAttr.RefFlowNo, value);
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStringByKey(CondAttr.Note);
            }
            set
            {
                this.SetValByKey(CondAttr.Note, value);
            }
        }
        /// <summary>
        /// 条件类型(表单条件，岗位条件，部门条件，开发者参数)
        /// </summary>
        public CondType CondType
        {
            get
            {
                return (CondType)this.GetValIntByKey(CondAttr.CondType);
            }
            set
            {
                this.SetValByKey(CondAttr.CondType, (int)value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(CondAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(CondAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 对方向条件有效
        /// </summary>
        public int ToNodeID
        {
            get
            {
                return this.GetValIntByKey(CondAttr.ToNodeID);
            }
            set
            {
                this.SetValByKey(CondAttr.ToNodeID, value);
            }
        }
        #endregion 

        protected override bool beforeInsert()
        {
            //设置他的主键。
            this.MyPK = DBAccess.GenerGUID();
            return base.beforeInsert();
        }

        #region 实现基本的方方法
        /// <summary>
        /// 属性
        /// </summary>
        public string FK_Attr
        {
            get
            {
                return this.GetValStringByKey(CondAttr.FK_Attr);
            }
            set
            {
                if (value == null)
                    throw new Exception("FK_Attr不能设置为null");

                value = value.Trim();

                this.SetValByKey(CondAttr.FK_Attr, value);

                BP.Sys.MapAttr attr = new BP.Sys.MapAttr(value);

                if (attr.LGType == FieldTypeS.Enum)
                {
                    /*是一个枚举类型的*/
                    SysEnum se = new SysEnum(attr.UIBindKey, this.OperatorValueInt);
                    this.OperatorValueT = se.Lab;
                }

                this.SetValByKey(CondAttr.AttrKey, attr.KeyOfEn);
                this.SetValByKey(CondAttr.AttrName, attr.Name);

            }
        }
        /// <summary>
        /// 要运算的实体属性
        /// </summary>
        public string AttrKey
        {
            get
            {
                return this.GetValStringByKey(CondAttr.AttrKey);
            }
            set
            {
                this.SetValByKey(CondAttr.AttrKey, value);
            }
        }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttrName
        {
            get
            {
                return this.GetValStringByKey(CondAttr.AttrName);
            }
            set
            {
                this.SetValByKey(CondAttr.AttrName, value);
            }
        }
        /// <summary>
        /// Idx
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(CondAttr.Idx);
            }
            set
            {
                this.SetValByKey(CondAttr.Idx, value);
            }
        }
        /// <summary>
        /// 操作的值
        /// </summary>
        public string OperatorValueT
        {
            get
            {
                return this.GetValStringByKey(CondAttr.OperatorValueT);
            }
            set
            {
                this.SetValByKey(CondAttr.OperatorValueT, value);
            }
        }
        /// <summary>
        /// 运算符号
        /// </summary>
        public string FK_Operator
        {
            get
            {
                string s = this.GetValStringByKey(CondAttr.FK_Operator);
                if (s == null || s == "")
                    return "=";
                return s;
            }
            set
            {
                string val = "";

                switch (value)
                {
                    case "dengyu":
                        val = "=";
                        break;
                    case "dayu":
                        val = ">";
                        break;
                    case "dayudengyu":
                        val = ">=";
                        break;
                    case "xiaoyu":
                        val = "<";
                        break;
                    case "xiaoyudengyu":
                        val = "<=";
                        break;
                    case "budengyu":
                        val = "!=";
                        break;
                    case "like":
                        val = " LIKE ";
                        break;
                    default:
                        break;
                }

                this.SetValByKey(CondAttr.FK_Operator, val);
            }
        }
        /// <summary>
        /// 运算值
        /// </summary>
        public object OperatorValue
        {
            get
            {
                string s = this.GetValStringByKey(CondAttr.OperatorValue);
                s = s.Replace("~", "'");
                return s;
            }
            set
            {
                this.SetValByKey(CondAttr.OperatorValue, value as string);
            }
        }
        /// <summary>
        /// 操作值Str
        /// </summary>
        public string OperatorValueStr
        {
            get
            {
                string sql = this.GetValStringByKey(CondAttr.OperatorValue);
                sql = sql.Replace("~", "'");
                return sql;
            }
        }
        /// <summary>
        /// 操作值int
        /// </summary>
        public int OperatorValueInt
        {
            get
            {
                return this.GetValIntByKey(CondAttr.OperatorValue);
            }
        }
        /// <summary>
        /// 操作值boolen
        /// </summary>
        public bool OperatorValueBool
        {
            get
            {
                return this.GetValBooleanByKey(CondAttr.OperatorValue);
            }
        }
        private Int64 _FID = 0;
        public Int64 FID
        {
            get
            {
                return _FID;
            }
            set
            {
                _FID = value;
            }
        }

        /// <summary>
        /// workid
        /// </summary>
        private Int64 _WorkID = 0;
        public Int64 WorkID
        {
            get
            {
                return _WorkID;
            }
            set
            {
                _WorkID = value;
            }
        }
        /// <summary>
        /// 条件消息
        /// </summary>
        public string MsgOfCond = "";
        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="fk_node">节点ID</param>
        public void DoUp(int fk_node)
        {
            int condtypeInt = (int)this.CondType;
            this.DoOrderUp(CondAttr.FK_Node, fk_node.ToString(), CondAttr.CondType, condtypeInt.ToString(), CondAttr.Idx);
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="fk_node">节点ID</param>
        public void DoDown(int fk_node)
        {
            int condtypeInt = (int)this.CondType;
            this.DoOrderDown(CondAttr.FK_Node, fk_node.ToString(), CondAttr.CondType, condtypeInt.ToString(), CondAttr.Idx);
        }
        /// <summary>
        /// 方向条件-下移
        /// </summary>
        public void DoDown2020Cond()
        {
            if (this.CondType == CondType.Dir)
                this.DoOrderDown(CondAttr.FK_Node, this.FK_Node, CondAttr.ToNodeID,
                    this.ToNodeID, CondAttr.CondType, (int)CondType.Dir, CondAttr.Idx);

            if (this.CondType == CondType.Flow)
                this.DoOrderDown(CondAttr.FK_Node, this.FK_Node, CondAttr.CondType, (int)CondType.Flow, CondAttr.Idx);

            if (this.CondType == CondType.SubFlow)
                this.DoOrderDown(CondAttr.FK_Node, this.FK_Node, CondAttr.CondType, (int)CondType.SubFlow, CondAttr.Idx);

            if (this.CondType == CondType.Node)
                this.DoOrderDown(CondAttr.FK_Node, this.FK_Node, CondAttr.CondType, (int)CondType.Node, CondAttr.Idx);
        }
        /// <summary>
        /// 方向条件-上移
        /// </summary>
        public void DoUp2020Cond()
        {
            if (this.CondType == CondType.Dir)
                this.DoOrderUp(CondAttr.FK_Node, this.FK_Node, CondAttr.ToNodeID,
                    this.ToNodeID, CondAttr.CondType, (int)CondType.Dir, CondAttr.Idx);

            if (this.CondType == CondType.Flow)
                this.DoOrderUp(CondAttr.FK_Node, this.FK_Node, CondAttr.CondType, (int)CondType.Flow, CondAttr.Idx);

            if (this.CondType == CondType.SubFlow)
                this.DoOrderUp(CondAttr.FK_Node, this.FK_Node, CondAttr.CondType, (int)CondType.SubFlow, CondAttr.Idx);

            if (this.CondType == CondType.Node)
                this.DoOrderUp(CondAttr.FK_Node, this.FK_Node, CondAttr.CondType, (int)CondType.Node, CondAttr.Idx);
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 条件
        /// </summary>
        public Cond() { }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="mypk"></param>
        public Cond(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 这个条件能不能通过
        /// </summary>
        public virtual bool IsPassed
        {
            get
            {
                Node nd = new Node(this.FK_Node);
                if (this.en == null)
                {
                    #region 实体不存在则进行重新初始化
                    GERpt en = nd.HisFlow.HisGERpt;
                    try
                    {
                        en.SetValByKey("OID", this.WorkID);
                        en.Retrieve();
                        en.ResetDefaultVal();
                        this.en = en;
                    }
                    catch (Exception ex)
                    {
                        //this.Delete();
                        return false;
                        //throw new Exception("@在取得判断条件实体[" + nd.EnDesc + "], 出现错误:" + ex.Message + "@错误原因是定义流程的判断条件出现错误,可能是你选择的判断条件工作类是当前工作节点的下一步工作造成,取不到该实体的实例.");
                    }
                    #endregion
                }

                if (this.HisDataFrom == ConnDataFrom.Stas)
                {
                    #region 按岗位控制
                    string strs = this.OperatorValue.ToString();
                    strs += this.OperatorValueT.ToString();

                    string strs1 = "";

                    BP.GPM.DeptEmpStations sts = new BP.GPM.DeptEmpStations();
                    sts.Retrieve("FK_Emp", this.SpecOper);
                    foreach (BP.GPM.DeptEmpStation st in sts)
                    {
                        if (strs.Contains("@" + st.FK_Station + "@"))
                        {
                            this.MsgOfCond = "@以岗位判断方向，条件为true：岗位集合" + strs + "，操作员(" + BP.Web.WebUser.No + ")岗位:" + st.FK_Station + st.FK_StationT;
                            return true;
                        }
                        strs1 += st.FK_Station + "-" + st.FK_StationT;
                    }


                    this.MsgOfCond = "@以岗位判断方向，条件为false：岗位集合" + strs + "，操作员(" + BP.Web.WebUser.No + ")岗位:" + strs1;
                    return false;
                    #endregion
                }

                if (this.HisDataFrom == ConnDataFrom.Depts)
                {
                    #region 按部门控制
                    string strs = this.OperatorValue.ToString();
                    strs += this.OperatorValueT.ToString();

                    BP.GPM.DeptEmps sts = new BP.GPM.DeptEmps();

                    sts.Retrieve(BP.GPM.DeptEmpAttr.FK_Emp, this.SpecOper);

                    //@于庆海.
                    BP.Port.Emp emp = new BP.Port.Emp(this.SpecOper);
                    emp.NoOfSAAS = this.SpecOper;
                    if (emp.RetrieveFromDBSources() == 1)
                    {
                        BP.GPM.DeptEmp de = new GPM.DeptEmp();
                        de.FK_Dept = emp.FK_Dept;
                        sts.AddEntity(de);
                    }


                    string strs1 = "";
                    foreach (BP.GPM.DeptEmp st in sts)
                    {
                        if (strs.Contains("@" + st.FK_Dept + "@"))
                        {
                            this.MsgOfCond = "@以岗位判断方向，条件为true：部门集合" + strs + "，操作员(" + BP.Web.WebUser.No + ")部门:" + st.FK_Dept;
                            return true;
                        }
                        strs1 += st.FK_Dept;
                    }

                    this.MsgOfCond = "@以部门判断方向，条件为false：部门集合" + strs + "，操作员(" + BP.Web.WebUser.No + ")部门:" + strs1;
                    return false;

                    #endregion
                }

                if (this.HisDataFrom == ConnDataFrom.SQL)
                {
                    #region 按SQL 计算
                    //this.MsgOfCond = "@以表单值判断方向，值 " + en.EnDesc + "." + this.AttrKey + " (" + en.GetValStringByKey(this.AttrKey) + ") 操作符:(" + this.FK_Operator + ") 判断值:(" + this.OperatorValue.ToString() + ")";
                    string sql = this.OperatorValueStr;
                    sql = sql.Replace("~", "'");
                    sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                    sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

                    //获取参数值
                    //System.Collections.Specialized.NameValueCollection urlParams = HttpContextHelper.RequestParams; // System.Web.HttpContext.Current.Request.Form;
                    foreach (string key in HttpContextHelper.RequestParamKeys)
                    {
                        //循环使用数组
                        if (DataType.IsNullOrEmpty(key) == false && sql.Contains(key) == true)
                            sql = sql.Replace("@" + key, HttpContextHelper.RequestParams(key));
                        //sql = sql.Replace("@" + key, urlParams[key]);
                    }

                    if (en.IsOIDEntity == true)
                    {
                        sql = sql.Replace("@WorkID", en.GetValStrByKey("OID"));
                        sql = sql.Replace("@OID", en.GetValStrByKey("OID"));
                    }

                    if (sql.Contains("@") == true)
                    {
                        /* 如果包含 @ */
                        foreach (Attr attr in this.en.EnMap.Attrs)
                        {
                            sql = sql.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
                        }
                    }

                    int result = DBAccess.RunSQLReturnValInt(sql, -1);
                    if (result <= 0)
                        return false;

                    return true;
                    #endregion 按SQL 计算

                }

                if (this.HisDataFrom == ConnDataFrom.SQLTemplate)
                {
                    #region 按SQLTemplate 计算
                    //this.MsgOfCond = "@以表单值判断方向，值 " + en.EnDesc + "." + this.AttrKey + " (" + en.GetValStringByKey(this.AttrKey) + ") 操作符:(" + this.FK_Operator + ") 判断值:(" + this.OperatorValue.ToString() + ")";
                    string fk_sqlTemplate = this.OperatorValueStr;
                    SQLTemplate sqltemplate = new SQLTemplate();
                    sqltemplate.No = fk_sqlTemplate;
                    if (sqltemplate.RetrieveFromDBSources() == 0)
                        throw new Exception("@配置的SQLTemplate编号为[" + sqltemplate + "]被删除了,判断条件丢失.");

                    string sql = sqltemplate.Docs;
                    sql = sql.Replace("~", "'");
                    sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                    sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

                    if (en.IsOIDEntity == true)
                    {
                        sql = sql.Replace("@WorkID", en.GetValStrByKey("OID"));
                        sql = sql.Replace("@OID", en.GetValStrByKey("OID"));
                    }

                    if (sql.Contains("@") == true)
                    {
                        /* 如果包含 @ */
                        foreach (Attr attr in this.en.EnMap.Attrs)
                        {
                            sql = sql.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
                        }
                    }

                    int result = DBAccess.RunSQLReturnValInt(sql, -1);
                    if (result <= 0)
                        return false;

                    if (result >= 1)
                        return true;

                    throw new Exception("@您设置的sql返回值，不符合农芯BPM的要求，必须是0或大于等于1。");
                    #endregion
                }

                if (this.HisDataFrom == ConnDataFrom.Url)
                {
                    #region URL 参数计算
                    string url = this.OperatorValueStr;
                    if (url.Contains("?") == false)
                        url = url + "?1=2";

                    url = url.Replace("@SDKFromServHost", SystemConfig.AppSettings["SDKFromServHost"]);
                    url = BP.WF.Glo.DealExp(url, this.en, "");

                    #region 加入必要的参数.
                    if (url.Contains("&FK_Flow") == false)
                        url += "&FK_Flow=" + this.FK_Flow;
                    if (url.Contains("&FK_Node") == false)
                        url += "&FK_Node=" + this.FK_Node;

                    if (url.Contains("&WorkID") == false)
                        url += "&WorkID=" + this.WorkID;

                    if (url.Contains("&FID") == false)
                        url += "&FID=" + this.FID;

                    if (url.Contains("&SID") == false)
                        url += "&SID=" + BP.Web.WebUser.SID;

                    if (url.Contains("&UserNo") == false)
                        url += "&UserNo=" + BP.Web.WebUser.No;


                    #endregion 加入必要的参数.

                    #region 对url进行处理.
                    if (SystemConfig.IsBSsystem)
                    {
                        /*是bs系统，并且是url参数执行类型.*/
                        string myurl = HttpContextHelper.RequestRawUrl;// BP.Sys.Glo.Request.RawUrl;
                        if (myurl.IndexOf('?') != -1)
                            myurl = myurl.Substring(myurl.IndexOf('?'));

                        myurl = myurl.Replace("?", "&");
                        string[] paras = myurl.Split('&');
                        foreach (string s in paras)
                        {
                            string[] strs = s.Split('=');

                            //如果已经有了这个参数.
                            if (url.Contains(strs[0] + "=") == true)
                                continue;

                            if (url.Contains(s))
                                continue;
                            url += "&" + s;
                        }
                        url = url.Replace("&?", "&");
                    }

                    //替换特殊的变量.
                    url = url.Replace("&?", "&");

                    if (SystemConfig.IsBSsystem == false)
                    {
                        /*非bs模式下调用,比如在cs模式下调用它,它就取不到参数. */
                    }


                    if (url.Contains("http") == false)
                    {
                        /*如果没有绝对路径 */
                        if (SystemConfig.IsBSsystem)
                        {
                            /*在cs模式下自动获取*/
                            string host = HttpContextHelper.RequestUrlHost;//BP.Sys.Glo.Request.Url.Host;
                            if (url.Contains("@AppPath"))
                                url = url.Replace("@AppPath", "http://" + host + HttpContextHelper.RequestApplicationPath);//BP.Sys.Glo.Request.ApplicationPath
                            else//BP.Sys.Glo.Request.Url.Authority
                                url = "http://" + HttpContextHelper.RequestUrlAuthority + url;
                        }

                        if (SystemConfig.IsBSsystem == false)
                        {
                            /*在cs模式下它的baseurl 从web.config中获取.*/
                            string cfgBaseUrl = SystemConfig.AppSettings["HostURL"];
                            if (DataType.IsNullOrEmpty(cfgBaseUrl))
                            {
                                string err = "调用url失败:没有在web.config中配置BaseUrl,导致url事件不能被执行.";
                                Log.DefaultLogWriteLineError(err);
                                throw new Exception(err);
                            }
                            url = cfgBaseUrl + url;
                        }
                    }
                    #endregion 对url进行处理.

                    #region 求url的值
                    try
                    {
                        url = url.Replace("'", "");
                        // url = url.Replace("//", "/");
                        // url = url.Replace("//", "/");
                        System.Text.Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
                        string text = DataType.ReadURLContext(url, 8000, encode);
                        if (text == null)
                            //throw new Exception("@流程设计的方向条件错误，执行的URL错误:" + url + ", 返回为null, 请检查设置是否正确。");
                            return false;

                        if (DataType.IsNullOrEmpty(text) == true)
                            // throw new Exception("@错误，没有接收到返回值.");
                            return false;

                        if (DataType.IsNumStr(text) == false)
                            //throw new Exception("@错误，不符合约定的格式，必须是数字类型。");
                            return false;
                        try
                        {
                            float f = float.Parse(text);
                            if (f > 0)
                                return true;
                            else
                                return false;
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@判断url方向出现错误:" + ex.Message + ",执行url错误:" + url);
                    }
                    #endregion 对url进行处理.

                    #endregion
                }

                if (this.HisDataFrom == ConnDataFrom.WebApi)
                {
                    #region WebApi接口
                    //返回值
                    string postData = "";
                    string apiUrl = this.OperatorValueStr;
                    if (apiUrl.Contains("@WebApiHost"))//可以替换配置文件中配置的webapi地址
                        apiUrl = apiUrl.Replace("@WebApiHost", SystemConfig.AppSettings["WebApiHost"]);

                    //如果有参数
                    if (apiUrl.Contains("?"))
                    {
                        //api接口地址
                        string apiHost = apiUrl.Split('?')[0];
                        //api参数
                        string apiParams = apiUrl.Split('?')[1];
                        //参数替换
                        apiParams = BP.WF.Glo.DealExp(apiParams, nd.HisWork);
                        //执行POST
                        postData = BP.WF.Glo.HttpPostConnect(apiHost, apiParams);

                        if (postData == "true")
                            return true;
                        else
                            return false;
                    }
                    else
                    {//如果没有参数，执行GET
                        postData = BP.WF.Glo.HttpGet(apiUrl);
                        if (postData == "true")
                            return true;
                        else
                            return false;
                    }
                    #endregion WebApi接口
                }
                #region 审核组件的立场
                if(this.HisDataFrom == ConnDataFrom.WorkCheck)
                {
                    //获取当前节点的审核组件信息
                    string tag = BP.WF.Dev2Interface.GetCheckTag(this.FK_Flow, this.WorkID, this.FK_Node, WebUser.No);
                    if (tag.Contains("@FWCView="+this.OperatorValue) == true)
                        return true;
                    return false;
                }
                #endregion 审核组件的立场

                if (this.HisDataFrom == ConnDataFrom.Paras)
                {
                    Hashtable ht = en.Row;
                    return BP.WF.Glo.CondExpPara(this.OperatorValueStr, ht, en.OID);
                }

                //从节点表单里判断.
                if (this.HisDataFrom == ConnDataFrom.NodeForm)
                {
                    if (en.EnMap.Attrs.Contains(this.AttrKey) == false)
                        throw new Exception("err@判断条件方向出现错误：实体：" + nd.EnDesc + " 属性" + this.AttrKey + "已经被删除方向条件判断失败.");

                    this.MsgOfCond = "@以表单值判断方向，值 " + en.EnDesc + "." + this.AttrKey + " (" + en.GetValStringByKey(this.AttrKey) + ") 操作符:(" + this.FK_Operator + ") 判断值:(" + this.OperatorValue.ToString() + ")";
                    return CheckIsPass(en);
                }

                //从独立表单里判断.
                if (this.HisDataFrom == ConnDataFrom.StandAloneFrm)
                {
                    MapAttr attr = new MapAttr(this.FK_Attr);
                    attr.MyPK = this.FK_Attr;
                    if (attr.RetrieveFromDBSources() == 0)
                        throw new Exception("err@到达【"+this.ToNodeID+"】方向条件设置错误,原来做方向条件的字段:"+this.FK_Attr+",已经不存在了.");

                    GEEntity myen = new GEEntity(attr.FK_MapData, en.OID);
                    return CheckIsPass(myen);
                }
                return false;
            }
        }
        private bool CheckIsPass(Entity en)
        {

            try
            {
                switch (this.FK_Operator.Trim().ToLower())
                {
                    case "<>":
                    case "!=":
                    case "budingyu":
                    case "budengyu": //不等于.
                        if (en.GetValStringByKey(this.AttrKey).Equals(this.OperatorValue.ToString()) == false)
                            return true;
                        else
                            return false;
                    case "=":  // 如果是 = 
                    case "dengyu":
                        if (en.GetValStringByKey(this.AttrKey).Equals(this.OperatorValue.ToString().Replace("\"", "")) == true)
                            return true;
                        else
                            return false;
                    case ">":
                    case "dayu":
                        if (en.GetValDoubleByKey(this.AttrKey) > Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;

                    //if (en.GetValDoubleByKey(this.AttrKey) > Double.Parse(this.OperatorValue.ToString()))
                    //    return true;
                    //else
                    //    return false;
                    case ">=":
                    case "dayudengyu":
                        if (en.GetValDoubleByKey(this.AttrKey) >= Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;
                    case "<":
                    case "xiaoyu":
                        if (en.GetValDoubleByKey(this.AttrKey) < Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;
                    //if (en.GetValDoubleByKey(this.AttrKey) < Double.Parse(this.OperatorValue.ToString()))
                    //    return true;
                    //else
                    //    return false;
                    case "<=":
                    case "xiaoyudengyu":
                        if (en.GetValDoubleByKey(this.AttrKey) <= Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;
                    case "like":
                    case "baohan":
                        if (en.GetValStringByKey(this.AttrKey).IndexOf(this.OperatorValue.ToString()) == -1)
                            return false;
                        else
                            return true;
                    default:
                        throw new Exception("@没有找到操作符号(" + this.FK_Operator.Trim().ToLower() + ").");
                }
            }
            catch (Exception ex)
            {
                Node nd23 = new Node(this.FK_Node);
                throw new Exception("@判断条件:Node=[" + this.FK_Node + "," + nd23.EnDesc + "], 出现错误。@" + ex.Message + "。有可能您设置了非法的条件判断方式。");
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("WF_Cond", "条件");

                map.AddMyPK();


                //用于整体流程的删除，导入，导出.
                map.AddTBString(CondAttr.RefFlowNo, null, "流程编号", true, true, 0, 5, 20);


                //@0=节点完成条件@1=流程条件@2=方向条件@3=启动子流程
                map.AddTBInt(CondAttr.CondType, 0, "条件类型", true, true);

                //@0=NodeForm表单数据,1=StandAloneFrm独立表单,2=Stas岗位数据,3=Depts,4=按sql计算.
                //5,按sql模版计算.6,按参数,7=按Url @=100条件表达式.
                map.AddTBInt(CondAttr.DataFrom, 0, "条件数据来源0表单,1岗位(对方向条件有效)", true, true);
                map.AddTBString(CondAttr.FK_Flow, null, "流程", true, true, 0, 4, 20);

                //对于启动子流程规则有效.
                map.AddTBString(CondAttr.SubFlowNo, null, "子流程编号", true, true, 0, 5, 20);

                map.AddTBInt(CondAttr.FK_Node, 0, "节点ID(对方向条件有效)", true, true);
                map.AddTBInt(CondAttr.ToNodeID, 0, "ToNodeID（对方向条件有效）", true, true);

                map.AddTBString(CondAttr.FK_Attr, null, "属性", true, true, 0, 80, 20);
                map.AddTBString(CondAttr.AttrKey, null, "属性键", true, true, 0, 60, 20);
                map.AddTBString(CondAttr.AttrName, null, "中文名称", true, true, 0, 500, 20);
                map.AddTBString(CondAttr.FK_Operator, "=", "运算符号", true, true, 0, 60, 20);
                map.AddTBString(CondAttr.OperatorValue, "", "要运算的值", true, true, 0, 4000, 20);
                map.AddTBString(CondAttr.OperatorValueT, "", "要运算的值T", true, true, 0, 4000, 20);

                map.AddTBString(CondAttr.Note, null, "备注", true, true, 0, 500, 20);
                map.AddTBInt(CondAttr.Idx, 1, "优先级", true, true);

                //参数 for wangrui add 2015.10.6. 条件为station,depts模式的时候，需要指定人员。
                map.AddTBAtParas(2000);

                map.AddTBInt(CondAttr.Idx, 0, "Idx", true, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {

            if ( DataType.IsNullOrEmpty( this.RefFlowNo)==true)
            {
                if (this.CondType== CondType.Dir
                    || this.CondType == CondType.Node 
                    || this.CondType== CondType.SubFlow)
                {
                    Node nd = new Node(this.FK_Node);
                    this.RefFlowNo = nd.FK_Flow;
                }

                if (this.CondType == CondType.Flow)
                {
                    this.RefFlowNo = this.FK_Flow;
                    if (DataType.IsNullOrEmpty(this.RefFlowNo) == true)
                        throw new Exception("err@流程完成条件设置错误，没有给FK_Flow赋值。");
                }
            }

            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 条件s
    /// </summary>
    public class Conds : Entities
    {
        #region 属性
        public string ConditionDesc
        {
            get
            {
                return "";
            }
        }
        /// <summary>
        /// 获得Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get { return new Cond(); }
        }
        /// <summary>
        /// 执行计算
        /// </summary>
        /// <param name="runModel">模式</param>
        /// <returns></returns>
        public bool GenerResult(GERpt en = null)
        {   
            if (this.Count == 0)
                throw new Exception("err@没有要计算的条件，无法计算.");

            //给条件赋值.
            if (en != null)
            {
                foreach (Cond cd in this)
                {
                    cd.WorkID = en.OID;
                    cd.en = en;
                }
            }

            #region 首先计算简单的.
            //如果只有一个条件,就直接范围该条件的执行结果.
            if (this.Count == 1)
            {
                Cond cond = this[0] as Cond;
                return cond.IsPassed;
            }
            #endregion 首先计算简单的.

            #region 处理混合计算。
            string exp = "";
            foreach (Cond item in this)
            {
                if (item.HisDataFrom == ConnDataFrom.CondOperator)
                {
                    exp += " " + item.OperatorValue;
                    continue;
                }

                if (item.IsPassed)
                    exp += " 1=1 ";
                else
                    exp += " 1=2 ";
            }

            //如果是混合计算.
            string sql = "";
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sql = " SELECT TOP 1 No FROM WF_Emp WHERE " + exp;
                    break;
                case DBType.MySQL:
                    sql = " SELECT No FROM WF_Emp WHERE " + exp + "    limit 1 ";
                    break;
                case DBType.Oracle:
                case DBType.DM:
                    sql = " SELECT No FROM WF_Emp WHERE " + exp + "    rownum <=1 ";
                    break;
                default:
                    throw new Exception("err@没有做的数据库类型判断.");
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return false;
            return true;
            #endregion 处理混合计算。
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string MsgOfDesc
        {
            get
            {
                string msg = "";
                foreach (Cond c in this)
                {
                    msg += "@" + c.MsgOfCond;
                }
                return msg;
            }
        }
        public int NodeID = 0;
        #endregion

        #region 构造
        /// <summary>
        /// 条件
        /// </summary>
        public Conds()
        {
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        public Conds(string fk_flow)
        {
            this.Retrieve(CondAttr.FK_Flow, fk_flow);
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="ct">类型</param>
        /// <param name="nodeID">节点</param>
        public Conds(CondType ct, int nodeID, Int64 workid, GERpt enData)
        {
            this.NodeID = nodeID;
            this.Retrieve(CondAttr.FK_Node, nodeID, CondAttr.CondType, (int)ct, CondAttr.Idx);
            foreach (Cond en in this)
            {
                en.WorkID = workid;
                en.en = enData;
            }
        }
        /// <summary>
        /// 条件 - 配置信息
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="nodeID"></param>
        public Conds(CondType ct, int nodeID)
        {
            this.Retrieve(CondAttr.FK_Node, nodeID, CondAttr.CondType, (int)ct, CondAttr.Idx);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Cond> ToJavaList()
        {
            return (System.Collections.Generic.IList<Cond>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Cond> Tolist()
        {
            System.Collections.Generic.List<Cond> list = new System.Collections.Generic.List<Cond>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Cond)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
