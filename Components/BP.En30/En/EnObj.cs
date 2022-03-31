using System;
using System.Collections;
using BP.DA;
using System.Data;
using BP.Sys;
using BP.En;
using BP.Web;
using System.Linq;
using BP.Difference;


namespace BP.En
{
    /// <summary>
    /// Entity 的摘要说明。
    /// </summary>	
    [Serializable]
    abstract public class EnObj
    {
        #region 访问控制.
        private string _DBVarStr = null;
        public string HisDBVarStr
        {
            get
            {
                if (_DBVarStr != null)
                    return _DBVarStr;

                _DBVarStr = this.EnMap.EnDBUrl.DBVarStr;
                return _DBVarStr;
            }
        }
        /// <summary>
        /// 他的访问控制.
        /// </summary>
        protected UAC _HisUAC = null;
        /// <summary>
        /// 得到 uac 控制.
        /// </summary>
        /// <returns></returns>
        public virtual UAC HisUAC
        {
            get
            {
                if (_HisUAC == null)
                {
                    _HisUAC = new UAC();
                    //if (BP.Web.WebUser.No == "admin")
                    if (BP.Web.WebUser.IsAdmin)
                    {
                        _HisUAC.IsAdjunct = false;
                        _HisUAC.IsDelete = true;
                        _HisUAC.IsInsert = true;
                        _HisUAC.IsUpdate = true;
                        _HisUAC.IsView = true;
                    }
                }
                return _HisUAC;
            }
        }
        #endregion

        #region 取出外部配置的属性信息
        /// <summary>
        /// 取出Map 的扩展属性。
        /// 用于第3方的扩展属性开发。
        /// </summary>
        /// <param name="key">属性Key</param>
        /// <returns>设置的属性</returns>
        public string GetMapExtAttrByKey(string key)
        {
            Paras ps = new Paras();
            ps.Add("enName", this.ToString());
            ps.Add("key", key);

            return (string)DBAccess.RunSQLReturnVal("select attrValue from Sys_ExtMap WHERE className=" + SystemConfig.AppCenterDBVarStr + "enName AND attrKey=" + SystemConfig.AppCenterDBVarStr + "key", ps);
        }
        #endregion

        #region CreateInstance
        /// <summary>
        /// 创建一个实例
        /// </summary>
        /// <returns>自身的实例</returns>
        public Entity CreateInstance()
        {
            return this.GetType().Assembly.CreateInstance(this.ToString()) as Entity;
            //return ClassFactory.GetEn(this.ToString());
        }
        private Entities _GetNewEntities = null;
        #endregion

        #region 方法
        /// <summary>
        /// 设置默认值.
        /// </summary>
        public void ResetDefaultValRowValues()
        {
            if (this._enMap == null)
                return;

            foreach (Attr attr in this._enMap.Attrs)
            {
                string key = attr.Key;

                string v = this.GetValStringByKey(key, null);  // this._row[key] as string;

                if (v == null || v.IndexOf('@') == -1)
                    continue;

                // 设置默认值.
                if (v.Equals("@WebUser.No"))
                {

                    this.SetValByKey(attr.Key, WebUser.No);

                    continue;
                }
                else if (v.Equals("@WebUser.Name"))
                {
                    this.SetValByKey(attr.Key, WebUser.Name);
                    continue;
                }
                else if (v.Equals("@WebUser.FK_Dept"))
                {
                    this.SetValByKey(attr.Key, WebUser.FK_Dept);
                    continue;
                }
                else if (v.Equals("@WebUser.FK_DeptName"))
                {
                    this.SetValByKey(attr.Key, WebUser.FK_DeptName);
                    continue;
                }
                else if (v.Equals("@WebUser.FK_DeptNameOfFull")
                    || v.Equals("@WebUser.FK_DeptFullName"))
                {
                    this.SetValByKey(attr.Key, WebUser.FK_DeptNameOfFull);
                    continue;
                }
                else if (v.Equals("@RDT"))
                {
                    string dataFormat = "yyyy-MM-dd";
                    switch (attr.IsSupperText)
                    {
                        case 0: break;
                        case 1:
                            dataFormat = "yyyy-MM-dd HH:mm";
                            break;
                        case 2:
                            dataFormat = "yyyy-MM-dd HH:mm:ss";
                            break;
                        case 3:
                            dataFormat = "yyyy-MM";
                            break;
                        case 4:
                            dataFormat = "HH:mm";
                            break;
                        case 5:
                            dataFormat = "HH:mm:ss";
                            break;
                        case 6:
                            dataFormat = "MM-dd";
                            break;
                        default:
                            throw new Exception("没有找到指定的时间类型");
                    }
                    this.SetValByKey(attr.Key, DataType.CurrentDateByFormart(dataFormat));
                    continue;
                }

            }
        }
        /// <summary>
        /// 重新设置默信息. 
        /// </summary>
        public void ResetDefaultVal(string fk_mapdata = null, string fk_flow = null, int fk_node = 0)
        {

            ResetDefaultValRowValues();

            DataTable dt = null;
            if (fk_node != 0 && fk_node != 999999 && fk_flow != null)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT MyPK,DefVal FROM Sys_FrmSln WHERE FK_MapData =" + ps.DBStr + "FK_MapData AND FK_Flow=" + ps.DBStr + "FK_Flow AND FK_Node =" + ps.DBStr + "FK_Node";
                ps.Add("FK_MapData", fk_mapdata);
                ps.Add("FK_Flow", fk_flow);
                ps.Add("FK_Node", fk_node);

                dt = DBAccess.RunSQLReturnTable(ps);
            }

            Attrs attrs = this.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr)
                    this.SetValRefTextByKey(attr.Key, "");

                string v = attr.DefaultValOfReal as string;

                //先判断是否设置了字段权限
                if (dt!=null && dt.Rows.Count != 0)
                {
                    string mypk = fk_mapdata + "_" + fk_node + "_" + attr.Key;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string myp1k = dr[0].ToString();
                        if (myp1k.Equals(mypk) == true)
                        {
                            v = dr[1] as string;
                            break;
                        }
                    }
                }

                if (v == null || v.Contains("@") == false)
                    continue;

                string myval = this.GetValStrByKey(attr.Key);

                // 设置默认值.
                switch (v)
                {
                    case "@WebUser.No":
                    case "@CurrWorker":
                        if (attr.UIIsReadonly == true)
                        {
                            this.SetValByKey(attr.Key, WebUser.No);
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(myval) || myval == v)
                                this.SetValByKey(attr.Key, WebUser.No);
                        }
                        continue;
                    case "@WebUser.Name":
                        if (attr.UIIsReadonly == true)
                        {
                            this.SetValByKey(attr.Key, WebUser.Name);
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(myval) || myval == v)
                                this.SetValByKey(attr.Key, WebUser.Name);
                        }
                        continue;
                    case "@WebUser.FK_Dept":
                        if (attr.UIIsReadonly == true)
                        {
                            this.SetValByKey(attr.Key, WebUser.FK_Dept);
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(myval) || myval == v)
                                this.SetValByKey(attr.Key, WebUser.FK_Dept);
                        }
                        continue;
                    case "@WebUser.FK_DeptName":
                        if (attr.UIIsReadonly == true)
                        {
                            this.SetValByKey(attr.Key, WebUser.FK_DeptName);
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(myval) || myval == v)
                                this.SetValByKey(attr.Key, WebUser.FK_DeptName);
                        }
                        continue;
                    case "@WebUser.FK_DeptNameOfFull":
                    case "@WebUser.FK_DeptFullName":
                        if (attr.UIIsReadonly == true)
                        {
                            this.SetValByKey(attr.Key, WebUser.FK_DeptNameOfFull);
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(myval) || myval == v)
                                this.SetValByKey(attr.Key, WebUser.FK_DeptNameOfFull);
                        }
                        continue;
                    case "@WebUser.OrgNo":
                        if (attr.UIIsReadonly == true)
                        {
                            this.SetValByKey(attr.Key, WebUser.OrgNo);
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(myval) || myval == v)
                                this.SetValByKey(attr.Key, WebUser.OrgNo);
                        }
                        continue;
                    case "@WebUser.OrgName":
                        if (attr.UIIsReadonly == true)
                        {
                            this.SetValByKey(attr.Key, WebUser.OrgName);
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(myval) || myval == v)
                                this.SetValByKey(attr.Key, WebUser.OrgName);
                        }
                        continue;
                    case "@RDT":
                        string dataFormat = "yyyy-MM-dd";
                        switch (attr.IsSupperText)
                        {
                            case 0: break;
                            case 1:
                                dataFormat = "yyyy-MM-dd HH:mm";
                                break;
                            case 2:
                                dataFormat = "yyyy-MM-dd HH:mm:ss";
                                break;
                            case 3:
                                dataFormat = "yyyy-MM";
                                break;
                            case 4:
                                dataFormat = "HH:mm";
                                break;
                            case 5:
                                dataFormat = "HH:mm:ss";
                                break;
                            case 6:
                                dataFormat = "MM-dd";
                                break;
                            default:
                                throw new Exception("没有找到指定的时间类型");
                        }

                        if (attr.UIIsReadonly == true)
                        {
                            /// if (myval == v)
                            this.SetValByKey(attr.Key, DataType.CurrentDateByFormart(dataFormat));
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(myval) || myval == v)
                            {
                                this.SetValByKey(attr.Key, DataType.CurrentDateByFormart(dataFormat));
                            }
                        }
                        continue;
                    case "@FK_ND":
                        if (attr.UIIsReadonly == true)
                        {
                            this.SetValByKey(attr.Key, DataType.CurrentYear);
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(myval) || myval == v)
                                this.SetValByKey(attr.Key, DataType.CurrentYear);
                        }
                        continue;
                    case "@yyyy年MM月dd日":
                    case "@yyyy年MM月dd日HH时mm分":
                    case "@yy年MM月dd日":
                    case "@yy年MM月dd日HH时mm分":
                    case "@yyyy-MM-dd":
                        if (attr.UIIsReadonly == true)
                        {
                            this.SetValByKey(attr.Key, DateTime.Now.ToString(v.Replace("@", "")));
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(myval) || myval == v)
                                this.SetValByKey(attr.Key, DateTime.Now.ToString(v.Replace("@", "")));
                        }
                        continue;
                    default:
                        if (SystemConfig.IsBSsystem == true && HttpContextHelper.RequestParamKeys.Contains(v.Replace("@", "")) == true)
                        {
                            if (attr.UIIsReadonly == true)
                            {
                                this.SetValByKey(attr.Key, HttpContextHelper.RequestParams(v.Replace("@", "")));
                            }
                            else
                            {
                                if (DataType.IsNullOrEmpty(myval) || myval == v)
                                    this.SetValByKey(attr.Key, HttpContextHelper.RequestParams(v.Replace("@", "")));
                            }
                            continue; 
                        }
                        GloVar gloVar = new GloVar();
                        gloVar.PKVal = v;
                        int count = gloVar.RetrieveFromDBSources();
                        if (count == 1)
                        {
                            //执行SQL获取默认值
                            string sql = gloVar.Val;
                            if (DataType.IsNullOrEmpty(sql) == true)
                                continue;

                            sql = DealExp(sql, this.Row, null);
                            if (sql.ToUpper().Contains("SELECT") == false)
                            {
                                this.SetValByKey(attr.Key, sql);
                                continue;
                            }

                            try
                            {
                                //这里有异常就要跑出来
                                string val = DBAccess.RunSQLReturnString(sql);
                                this.SetValByKey(attr.Key, val);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("err@为类：" + this.ToString() + ",字段：" + attr.Key + ",变量表达式:" + v + ",设置信息:" + gloVar.ToJson() + ",设置默认值解析SQL:" + sql + " ，原始设置SQL:" + gloVar.Val + ",执行SQL期间出现错误.");
                            }
                        }
                        continue;
                }
            }
        }

        /// <summary>
        /// 处理表达式
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="en">数据源</param>
        /// <param name="errInfo">错误</param>
        /// <returns></returns>
        private static string DealExp(string exp, Row row, string errInfo)
        {
            if (exp.Contains("@") == false)
                return exp;

            exp = exp.Replace("~", "'");

            //首先替换加; 的。
            exp = exp.Replace("@WebUser.No;", WebUser.No);
            exp = exp.Replace("@WebUser.Name;", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_Dept;", WebUser.FK_Dept);
            exp = exp.Replace("@WebUser.FK_DeptName;", WebUser.FK_DeptName);

            // 替换没有 ; 的 .
            exp = exp.Replace("@WebUser.No", WebUser.No);
            exp = exp.Replace("@WebUser.Name", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
            exp = exp.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            //  exp = exp.Replace("@WorkID", "0");

            if (exp.Contains("@") == false)
            {
                exp = exp.Replace("~", "'");
                return exp;
            }

            //增加对新规则的支持. @MyField; 格式.
            if (row != null)
            {
                //特殊判断.
                if (row.ContainsKey("OID") == true)
                    exp = exp.Replace("@WorkID", row["OID"].ToString());

                if (exp.Contains("@") == false)
                    return exp;

                foreach (string key in row.Keys)
                {
                    if (exp.Contains("@" + key + ";"))
                        exp = exp.Replace("@" + key + ";", row[key].ToString());
                }
                if (exp.Contains("@") == false)
                    return exp;

                #region 解决排序问题.
                string mystrs = "";
                foreach (string key in row.Keys)
                {
                    mystrs += "@" + key;
                }


                string[] strs = mystrs.Split('@');
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("No", typeof(string)));
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;

                    DataRow dr = dt.NewRow();
                    dr[0] = str;
                    dt.Rows.Add(dr);
                }
                DataView dv = dt.DefaultView;
                dv.Sort = "No DESC";
                DataTable dtNew = dv.Table;
                #endregion  解决排序问题.

                #region 替换变量.
                foreach (DataRow dr in dtNew.Rows)
                {
                    string key = dr[0].ToString();
                    bool isStr = key.Contains(",");
                    if (isStr == true)
                    {
                        key = key.Replace(",", "");
                        exp = exp.Replace("@" + key, row.GetValStrByKey(key));
                    }
                    else
                        exp = exp.Replace("@" + key, row.GetValStrByKey(key));
                }
                #endregion

                if (exp.Contains("@") == false)
                    return exp;

                //替换全部的变量.
                foreach (string key in row.Keys)
                {
                    if (exp.Contains("@" + key))
                        exp = exp.Replace("@" + key, row[key].ToString());
                }
            }



            if (exp.Contains("@") && SystemConfig.IsBSsystem == true)
            {
                /*如果是bs*/
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    exp = exp.Replace("@" + key, HttpContextHelper.RequestParams(key));
                }
            }

            exp = exp.Replace("~", "'");

            return exp;
        }

        /// <summary>
        /// 把所有的值都设置成默认值，但是主键除外。
        /// </summary>
        public void ResetDefaultValAllAttr()
        {
            Attrs attrs = this.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.UIIsReadonly == false && attr.DefaultValOfReal != null)
                    continue;

                if (attr.IsPK)
                    continue;

                string v = attr.DefaultValOfReal as string;
                if (v == null)
                {
                    this.SetValByKey(attr.Key, "");
                    continue;
                }

                if (v.Contains("@") == false && v != null)
                {
                    this.SetValByKey(attr.Key, v);
                    continue;
                }


                // 设置默认值.
                switch (v)
                {
                    case "@WebUser.No":
                        this.SetValByKey(attr.Key, WebUser.No);
                        continue;
                    case "@WebUser.Name":
                        this.SetValByKey(attr.Key, WebUser.Name);
                        continue;
                    case "@WebUser.FK_Dept":
                        this.SetValByKey(attr.Key, WebUser.FK_Dept);
                        continue;
                    case "@WebUser.FK_DeptName":
                        this.SetValByKey(attr.Key, WebUser.FK_DeptName);
                        continue;
                    case "@WebUser.FK_DeptNameOfFull":
                        this.SetValByKey(attr.Key, WebUser.FK_DeptNameOfFull);
                        continue;
                    case "@RDT":
                        if (attr.MyDataType == DataType.AppDate)
                            this.SetValByKey(attr.Key, DataType.CurrentDate);
                        else
                            this.SetValByKey(attr.Key, DataType.CurrentDateTime);
                        continue;
                    default:
                        continue;
                }
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 实体
        /// </summary>
        public EnObj()
        {
        }
        private Map _tmpEnMap = null;
        /// <summary>
        /// Map
        /// </summary>
        protected Map _enMap
        {
            get
            {
                if (_tmpEnMap == null)
                    _tmpEnMap = Cash.GetMap(this.ToString());
                return _tmpEnMap;
            }
            set
            {
                if (value == null)
                {
                    _tmpEnMap = null;
                    return;
                }

                Map mp = (Map)value;
                if (SystemConfig.IsDebug)
                {

                }

                if (mp == null || mp.DepositaryOfMap == Depositary.None)
                {
                    _tmpEnMap = mp;
                    return;
                }

                Cash.SetMap(this.ToString(), mp);
                _tmpEnMap = mp;
            }
        }
        /// <summary>
        /// 子类需要继承
        /// </summary>
        public abstract Map EnMap
        {
            get;
        }
        /// <summary>
        /// 动态的获取map
        /// </summary>
        public Map EnMapInTime
        {
            get
            {
                _tmpEnMap = null;
                Cash.SetMap(this.ToString(), null);
                return this.EnMap;
            }
        }
        #endregion

        #region row 存放实体数据的
        /// <summary>
        /// 实体的 map 信息。	
        /// </summary>		
        //public abstract void EnMap();		
        private Row _row = null;
        public Row Row
        {
            get
            {
                if (this._row == null)
                {
                    this._row = new Row();
                    this._row.LoadAttrs(this.EnMap.Attrs);
                }
                return this._row;
            }
            set
            {
                this._row = value;
            }
        }
        #endregion

        #region 关于属性的操作。

        #region 设置值方法
        /// <summary>
        /// 设置object类型的值
        /// </summary>
        /// <param name="attrKey">attrKey</param>
        /// <param name="val">val</param>
        public void SetValByKey(string attrKey, string val)
        {
            if (val == null)
                val = "";
            this.Row.SetValByKey(attrKey, val);
        }
        public void SetValByKey(string attrKey, int val)
        {
            this.Row.SetValByKey(attrKey, val);
        }
        public void SetValByKey(string attrKey, Int64 val)
        {
            this.Row.SetValByKey(attrKey, val);
        }
        public void SetValByKey(string attrKey, float val)
        {
            this.Row.SetValByKey(attrKey, val);
        }
        public void SetValByKey(string attrKey, decimal val)
        {
            this.Row.SetValByKey(attrKey, val);
        }
        public void SetValByKey(string attrKey, object val)
        {
            this.Row.SetValByKey(attrKey, val);
        }

        public void SetValByDesc(string attrDesc, object val)
        {
            if (val == null)
                throw new Exception("@不能设置属性[" + attrDesc + "]null 值。");
            this.Row.SetValByKey(this.EnMap.GetAttrByDesc(attrDesc).Key, val);
        }

        /// <summary>
        /// 设置关联类型的值
        /// </summary>
        /// <param name="attrKey">attrKey</param>
        /// <param name="val">val</param>
        public void SetValRefTextByKey(string attrKey, object val)
        {
            this.SetValByKey(attrKey + "Text", val);
        }
        /// <summary>
        /// 设置bool类型的值
        /// </summary>
        /// <param name="attrKey">attrKey</param>
        /// <param name="val">val</param>
        public void SetValByKey(string attrKey, bool val)
        {
            if (val)
                this.SetValByKey(attrKey, 1);
            else
                this.SetValByKey(attrKey, 0);
        }
        /// <summary>
        /// 设置默认值
        /// </summary>
        public void SetDefaultVals()
        {
            foreach (Attr attr in this.EnMap.Attrs)
            {
                this.SetValByKey(attr.Key, attr.DefaultVal);
            }
        }
        /// <summary>
        /// 设置日期类型的值
        /// </summary>
        /// <param name="attrKey">attrKey</param>
        /// <param name="val">val</param>
        public void SetDateValByKey(string attrKey, string val)
        {
            try
            {
                this.SetValByKey(attrKey, DataType.StringToDateStr(val));
            }
            catch (System.Exception ex)
            {
                throw new Exception("@不合法的日期数据格式:key=[" + attrKey + "],value=" + val + " " + ex.Message);
            }
        }
        #endregion

        #region 取值方法
        /// <summary>
        /// 取得Object
        /// </summary>
        /// <param name="attrKey"></param>
        /// <returns></returns>
        public Object GetValByKey(string attrKey)
        {
            return this.Row.GetValByKey(attrKey);

            //try
            //{
            //    return this.Row.GetValByKey(attrKey);				
            //}
            //catch(Exception ex)
            //{
            //    throw new Exception(ex.Message+"  "+attrKey+" EnsName="+this.ToString() );
            //}
        }
        /// <summary>
        /// GetValDateTime
        /// </summary>
        /// <param name="attrKey"></param>
        /// <returns></returns>
        public DateTime GetValDateTime(string attrKey)
        {
            try
            {
                return DataType.ParseSysDateTime2DateTime(this.GetValStringByKey(attrKey));
            }
            catch (Exception ex)
            {
                throw new Exception("字段[" + this.EnMap.GetAttrByKey(attrKey).Desc + "],值[" + this.GetValStringByKey(attrKey) + "]，不是一个有效的时间格式.");
            }
        }
        /// <summary>
        /// 在确定  attrKey 存在 map 的情况下才能使用它
        /// </summary>
        /// <param name="attrKey"></param>
        /// <returns></returns>
        public string GetValStrByKey(string key)
        {
            if (this.Row.GetValByKey(key) != null)
                return this.Row.GetValByKey(key).ToString();
            return "";
        }
        public string GetValStrByKey(string key, string isNullAs)
        {
            try
            {
                string s = this.Row.GetValByKey(key).ToString();
                if (DataType.IsNullOrEmpty(s))
                    return isNullAs;
                else
                    return s;
            }
            catch
            {
                return isNullAs;
            }
        }
        /// <summary>
        /// 取得String
        /// </summary>
        /// <param name="attrKey"></param>
        /// <returns></returns>
        public string GetValStringByKey(string attrKey)
        {
            if (this.Row == null)
                throw new Exception("@没有初始化Row.");

            try
            {
                return this.Row[attrKey].ToString();
            }
            catch (Exception ex)
            {
                if (this.Row.ContainsKey(attrKey) == false)
                    throw new Exception("@获取值期间出现如下异常：" + ex.Message + "  " + attrKey + " 您没有在类【" + this.ToString() + "】增加这个属性.");

                string val = this.Row[attrKey] as string;
                if (val == null)
                    return "";
                return val;
            }
        }
        public string GetValStringByKey(string attrKey, string defVal)
        {
            string val = this.GetValStringByKey(attrKey);
            if (val == null || val == "")
                return defVal;
            else
                return val;
        }
        /// <summary>
        ///  取出大块文本
        /// </summary>
        /// <returns></returns>
        public string GetValDocText()
        {
            string s = this.GetValStrByKey("Doc");
            if (s.Trim().Length != 0)
                return s;

            s = SysDocFile.GetValTextV2(this.ToString(), this.PKVal.ToString());
            this.SetValByKey("Doc", s);
            return s;
        }
        public string GetValDocHtml()
        {
            string s = this.GetValHtmlStringByKey("Doc");
            if (s.Trim().Length != 0)
                return s;

            s = SysDocFile.GetValHtmlV2(this.ToString(), this.PKVal.ToString());
            this.SetValByKey("Doc", s);
            return s;
        }
        /// <summary>
        /// 取到Html 信息。
        /// </summary>
        /// <param name="attrKey">attr</param>
        /// <returns>html.</returns>
        public string GetValHtmlStringByKey(string attrKey)
        {
            return DataType.ParseText2Html(this.GetValStringByKey(attrKey));
        }
        public string GetValHtmlStringByKey(string attrKey, string defval)
        {
            return DataType.ParseText2Html(this.GetValStringByKey(attrKey, defval));
        }
        /// <summary>
        /// 取得枚举或者外键的标签
        /// 如果是枚举就获取枚举标签.
        /// 如果是外键就获取为外键的名称.
        /// </summary>
        /// <param name="attrKey"></param>
        /// <returns></returns>
        public string GetValRefTextByKey(string attrKey)
        {
            string str = "";
            try
            {
                str = this.Row.GetValByKey(attrKey + "Text") as string;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + attrKey);
            }
            if (str == null)
            {
                /*判断是否是外键字段？*/
                if (this.Row.ContainsKey(attrKey + "Text") == false)
                    throw new Exception(attrKey + "非外键或者枚举，不能使用:GetValRefTextByKey ，获取数据。");
            }
            return str;
        }
        public Int64 GetValInt64ByKey(string key)
        {
            return Int64.Parse(this.GetValStringByKey(key, "0"));
        }

        public int GetValIntByKey(string key, int isNullAsVal)
        {
            string str = this.GetValStrByKey(key);
            if (str == null || str == "" || str == "null")
                return isNullAsVal;

            if (DataType.IsNumStr(str) == false)
                return isNullAsVal;
            return int.Parse(str);
        }
        /// <summary>
        /// 根据key 得到int val
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetValIntByKey(string key)
        {
            try
            {
                string str = this.GetValStrByKey(key);
                return string.IsNullOrWhiteSpace(str) ? 0 : int.Parse(this.GetValStrByKey(key));
            }
            catch (Exception ex)
            {
                //if (SystemConfig.IsDebug == false)
                //    throw new Exception("@[" + this.EnMap.GetAttrByKey(key).Desc + "]请输入数字，您输入的是[" + this.GetValStrByKey(key) + "]。");
                //else
                //    throw new Exception("@表[" + this.EnDesc + "]在获取属性[" + key + "]值,出现错误，不能将[" + this.GetValStringByKey(key) + "]转换为int类型.错误信息：" + ex.Message + "@请检查是否在存储枚举类型时，您在SetValbyKey中没有转换。正确做法是:this.SetValByKey( Key ,(int)value)  ");

                string v = this.GetValStrByKey(key).ToLower();
                if (v == "null")
                    return 0;

                if (v == "true")
                {
                    this.SetValByKey(key, 1);
                    return 1;
                }
                if (v == "false")
                {
                    this.SetValByKey(key, 0);
                    return 0;
                }

                if (key == "OID")
                {
                    this.SetValByKey("OID", 0);
                    return 0;
                }

                if (this.GetValStrByKey(key) == "")
                {
                    Attr attr = this.EnMap.GetAttrByKey(key);
                    if (attr.IsNull)
                        return 567567567;
                    else
                        return int.Parse(attr.DefaultVal.ToString());
                }

                throw new Exception("@实体类[" + this.ToString() + "]@[" + this.EnMap.GetAttrByKey(key).Desc + "]请输入数字，您输入的是[" + this.GetValStrByKey(key) + "],错误信息:" + ex.Message);
            }
        }
        /// <summary>
        /// 根据key 得到 bool val
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetValBooleanByKey(string key)
        {
            string s = this.GetValStrByKey(key);
            if (DataType.IsNullOrEmpty(s))
                s = this.EnMap.GetAttrByKey(key).DefaultVal.ToString();

            if (int.Parse(s) < 0 || int.Parse(s) == 0)
                return false;

            if (s == "1")
                return true;

            if (s.ToUpper().Equals("FALSE"))
                return false;
            if (s.ToUpper().Equals("TRUE"))
                return true;

            if (DataType.IsNullOrEmpty(s))
                return false;

            if (int.Parse(s) == 0)
                return false;

            return true;
        }

        public bool GetValBooleanByKey(string key, bool defval)
        {
            try
            {

                if (int.Parse(this.GetValStringByKey(key)) == 0)
                    return false;
                else
                    return true;
            }
            catch
            {
                return defval;
            }
        }
        public string GetValBoolStrByKey(string key)
        {
            if (int.Parse(this.GetValStringByKey(key)) == 0)
                return "否";
            else
                return "是";
        }
        /// <summary>
        /// 根据key 得到flaot val
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float GetValFloatByKey(string key, int blNum)
        {
            string val = this.Row.GetValByKey(key).ToString();
            if (DataType.IsNullOrEmpty(val))
                return float.Parse(blNum.ToString("0.00"));

            return float.Parse(float.Parse(val).ToString("0.00"));
        }
        /// <summary>
        /// 根据key 得到flaot val
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float GetValFloatByKey(string key)
        {
            try
            {
                string str = this.Row.GetValByKey(key).ToString();
                return string.IsNullOrWhiteSpace(str) ? 0 : float.Parse(float.Parse(str).ToString("0.00"));
            }
            catch
            {
                if (this.GetValStringByKey(key) == "")
                {
                    Attr attr = this.EnMap.GetAttrByKey(key);
                    if (attr.IsNull)
                        return 567567567;
                    else
                        return float.Parse(attr.DefaultVal.ToString());
                }
                return 0;
            }
        }
        /// <summary>
        /// 根据key 得到flaot val
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public decimal GetValDecimalByKey(string key)
        {
            try
            {
                return decimal.Parse(this.Row[key].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("@表[" + this.EnDesc + "]在获取属性[" + key + "]值,出现错误，不能将[" + this.GetValStrByKey(key) + "]转换为 decimal 类型.错误信息：" + ex.Message);
            }
        }
        public decimal GetValDecimalByKeyIsNullAsVal(string key, decimal val)
        {
            try
            {
                return GetValDecimalByKey(key);
            }
            catch (Exception ex)
            {
                return val;
            }
        }
        public double GetValDoubleByKey(string key)
        {
            try
            {
                return double.Parse(this.GetValStrByKey(key));
            }
            catch (Exception ex)
            {
                throw new Exception("@表[" + this.EnDesc + "]在获取属性[" + key + "]值,出现错误，不能将[" + this.GetValStringByKey(key) + "]转换为double类型.错误信息：" + ex.Message);
            }
        }
        #endregion

        #endregion


        #region 属性
        /// <summary>
        /// 是否是空白的实体？
        /// </summary>
        public bool IsBlank
        {
            get
            {
                if (this._row == null)
                    return true;

                Attrs attrs = this.EnMap.Attrs;
                foreach (Attr attr in attrs)
                {
                    if (attr.UIIsReadonly && attr.IsFKorEnum == false)
                        continue;

                    //日期类型.  这里需要翻译.
                    if (attr.Key.Equals("RDT") || attr.Key.Equals("Rec"))
                        continue;

                    if (attr.DefaultValOfReal != null && attr.DefaultValOfReal.Contains("@") == true)
                        continue;

                    if (attr.IsFK)
                    {
                        if (this.GetValByKey(attr.Key) == "" || this.GetValByKey(attr.Key) == attr.DefaultValOfReal)
                            continue;
                        return false;
                    }

                    string str = this.GetValStrByKey(attr.Key);

                    //判断是否是数值类型.
                    if (attr.IsNum == true)
                        if (this.GetValFloatByKey(attr.Key) != float.Parse(attr.DefaultValOfReal))
                            return false;

                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    if (str.Equals(attr.DefaultVal) == false)
                        return false;
                }
                return true;
            }
        }
        /// <summary>
        /// 获取或者设置
        /// 是不是空的实体.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (this._row == null)
                {
                    return true;
                }
                else
                {
                    if (this.PKVal == null || this.PKVal.ToString() == "0" || this.PKVal.ToString() == "")
                        return true;
                    return false;
                }
            }
            set
            {
                this._row = null;
            }
        }
        /// <summary>
        /// 对这个实体的描述
        /// </summary>
        public String EnDesc
        {
            get
            {
                return this.EnMap.EnDesc;
            }
        }
        /// <summary>
        /// 取到主健值。如果它的主健不唯一，就返回第一个值。
        /// 获取或设置
        /// </summary>
        public Object PKVal
        {
            get
            {
                return this.GetValByKey(this.PK);
            }
            set
            {
                this.SetValByKey(this.PK, value);
            }
        }
        /// <summary>
        /// 如果只有一个主键,就返回PK,如果有多个就返回第一个.PK
        /// </summary>
        public int PKCount
        {
            get
            {
                switch (this.PK)
                {
                    case "OID":
                    case "No":
                    case "MyPK":
                        return 1;
                    default:
                        break;
                }

                int i = 0;
                foreach (Attr attr in this.EnMap.Attrs)
                {
                    if (attr.MyFieldType == FieldType.PK || attr.MyFieldType == FieldType.PKEnum || attr.MyFieldType == FieldType.PKFK)
                        i++;
                }
                if (i == 0)
                    throw new Exception("@没有给【" + this.EnDesc + "，" + this.EnMap.PhysicsTable + "】定义主键。");
                else
                    return i;
            }
        }
        /// <summary>
        /// 是不是OIDEntity
        /// </summary>
        public bool IsOIDEntity
        {
            get
            {
                if (this.PK == "OID")
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 是不是OIDEntity
        /// </summary>
        public bool IsNoEntity
        {
            get
            {
                if (this.PK == "No")
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 是否是TreeEntity
        /// </summary>
        public bool IsTreeEntity
        {
            get
            {
                if (this.GetType().BaseType.FullName.ToLower().Equals("bp.en.entitytree") == true)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 是不是IsMIDEntity
        /// </summary>
        public bool IsMIDEntity
        {
            get
            {
                if (this.PK == "MID")
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 如果只有一个主键,就返回PK,如果有多个就返回第一个.PK
        /// </summary>
        public virtual string PK
        {
            get
            {
                string pks = "";
                foreach (Attr attr in this.EnMap.Attrs)
                {
                    switch (attr.Key)
                    {
                        case "OID":
                            return "OID";
                        case "No":
                            return "No";
                        case "MyPK":
                            return "MyPK";
                        default:
                            break;
                    }

                    if (attr.MyFieldType == FieldType.PK
                        || attr.MyFieldType == FieldType.PKEnum || attr.MyFieldType == FieldType.PKFK)
                        pks += attr.Key + ",";
                }
                if (pks == "")
                    throw new Exception("@没有给【" + this.EnDesc + "，" + this.EnMap.PhysicsTable + "】定义主键。");
                pks = pks.Substring(0, pks.Length - 1);
                return pks;
            }
        }
        public virtual string PKField
        {
            get
            {
                foreach (Attr attr in this.EnMap.Attrs)
                {
                    switch (attr.Key)
                    {
                        case "OID":
                            return attr.Field;
                        case "No":
                            return attr.Field;
                        case "MyPK":
                            return attr.Field;
                        default:
                            break;
                    }

                    if (attr.MyFieldType == FieldType.PK
                        || attr.MyFieldType == FieldType.PKEnum
                        || attr.MyFieldType == FieldType.PKFK)
                        return attr.Field;
                }

                throw new Exception("@没有给【" + this.EnDesc + "】定义主键。");
            }
        }
        /// <summary>
        /// 如果只有一个主键,就返回PK,如果有多个就返回第一个.PK
        /// </summary>
        public string[] PKs
        {
            get
            {
                string[] strs1 = new string[this.PKCount];
                int i = 0;
                foreach (Attr attr in this.EnMap.Attrs)
                {
                    if (attr.MyFieldType == FieldType.PK || attr.MyFieldType == FieldType.PKEnum || attr.MyFieldType == FieldType.PKFK)
                    {
                        strs1[i] = attr.Key;
                        i++;
                    }
                }
                return strs1;
            }
        }
        /// <summary>
        /// 取到主健值。
        /// </summary>
        public Hashtable PKVals
        {
            get
            {
                Hashtable ht = new Hashtable();
                string[] strs = this.PKs;
                foreach (string str in strs)
                {
                    ht.Add(str, this.GetValStringByKey(str));
                }
                return ht;
            }
        }
        #endregion

    }

}
