using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Pub;
using BP.Sys;

namespace BP.En
{
    /// <summary>
    /// QueryObject 的摘要说明。
    /// </summary>
    public class QueryObject
    {
        private Entity _en = null;
        private Entities _ens = null;
        private string _sql = "";
        private Entity En
        {
            get
            {
                if (this._en == null)
                    return this.Ens.GetNewEntity;
                else
                    return this._en;
            }
            set
            {
                this._en = value;
            }
        }
        private Entities Ens
        {
            get
            {
                return this._ens;
            }
            set
            {
                this._ens = value;
            }
        }
        /// <summary>
        /// 处理Order by , group by . 
        /// </summary>
        private string _groupBy = "";
        /// <summary>
        /// 要得到的查询sql 。
        /// </summary>
        public string SQL
        {
            get
            {
                string sql = "";
                string selecSQL = SqlBuilder.SelectSQL(this.En, this.Top);
                if (this._sql == null || this._sql.Length == 0)
                    sql = selecSQL + this._groupBy + this._orderBy;
                else
                {
                    if (selecSQL.Contains(" WHERE "))
                        sql = selecSQL + "  AND ( " + this._sql + " ) " + _groupBy + this._orderBy;
                    else
                        sql = selecSQL + " WHERE   ( " + this._sql + " ) " + _groupBy + this._orderBy;
                }


                sql = sql.Replace("  ", " ");
                sql = sql.Replace("  ", " ");

                sql = sql.Replace("WHERE AND", "WHERE");
                sql = sql.Replace("WHERE  AND", "WHERE");

                sql = sql.Replace("WHERE ORDER", "ORDER");

                return sql;
            }
            set
            {
                this._sql = this._sql + " " + value;
            }
        }
        public string SQLWithOutPara
        {
            get
            {
                string sql = this.SQL;
                foreach (Para en in this.MyParas)
                {
                    sql = sql.Replace(SystemConfig.AppCenterDBVarStr + en.ParaName, "'" + en.val.ToString() + "'");
                }
                return sql;
            }
        }
        public void AddWhere(string str)
        {
            this._sql = this._sql + " " + str;
        }
        /// <summary>
        /// 修改于2009 -05-12 
        /// </summary>
        private int _Top = -1;
        public int Top
        {
            get
            {
                return _Top;
            }
            set
            {
                this._Top = value;
            }
        }
        private Paras _Paras = null;
        public Paras MyParas
        {
            get
            {
                if (_Paras == null)
                    _Paras = new Paras();
                return _Paras;
            }
            set
            {
                _Paras = value;
            }
        }
        private Paras _ParasR = null;
        public Paras MyParasR
        {
            get
            {
                if (_ParasR == null)
                    _ParasR = new Paras();
                return _ParasR;
            }
        }
        public void AddPara(string key, object v)
        {
            key = "P" + key;
            this.MyParas.Add(key, v);
        }
        public QueryObject()
        {
        }
        /// DictBase
        public QueryObject(Entity en)
        {
            this.MyParas.Clear();
            this._en = en;

            this.HisDBType = this._en.EnMap.EnDBUrl.DBType;
            this.HisDBUrlType = this._en.EnMap.EnDBUrl.DBUrlType;
        }
        public QueryObject(Entities ens)
        {
            this.MyParas.Clear();
            ens.Clear();
            this._ens = ens;

            Entity en = this._ens.GetNewEntity;

            this.HisDBType = en.EnMap.EnDBUrl.DBType;
            this.HisDBUrlType = en.EnMap.EnDBUrl.DBUrlType;
        }
        public BP.DA.DBType HisDBType = DBType.MSSQL;
        public BP.DA.DBUrlType HisDBUrlType = DBUrlType.AppCenterDSN;

        public string HisVarStr
        {
            get
            {
                switch (this.HisDBType)
                {
                    case DBType.MSSQL:
                    case DBType.Access:
                    case DBType.MySQL:
                        return "@";
                    case DBType.Informix:
                        return "?";
                    default:
                        return ":";
                }
            }
        }
        /// <summary>
        /// 增加函数查寻．
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="exp">表达格式 大于，等于，小于</param>
        /// <param name="len">长度</param>
        public void AddWhereLen(string attr, string exp, int len, BP.DA.DBType dbtype)
        {
            this.SQL = "( " + BP.Sys.SystemConfig.AppCenterDBLengthStr + "( " + attr2Field(attr) + " ) " + exp + " '" + len.ToString() + "')";
        }
        /// <summary>
        /// 增加查询条件，条件用 IN 表示．sql必须是一个列的集合．
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="sql">此sql,必须是有一个列的集合．</param>
        public void AddWhereInSQL(string attr, string sql)
        {
            this.AddWhere(attr, " IN ", "( " + sql + " )");
        }
        /// <summary>
        /// 增加查询条件，条件用 IN 表示．sql必须是一个列的集合．
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="sql">此sql,必须是有一个列的集合．</param>
        public void AddWhereInSQL(string attr, string sql, string orderBy)
        {
            this.AddWhere(attr, " IN ", "( " + sql + " )");
            this.addOrderBy(orderBy);
        }
        /// <summary>
        /// 增加查询条件，条件用 IN 表示．sql必须是一个列的集合．
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="sql">此sql,必须是有一个列的集合．</param>
        public void AddWhereNotInSQL(string attr, string sql)
        {
            this.AddWhere(attr, " NOT IN ", " ( " + sql + " ) ");
        }
        public void AddWhereNotIn(string attr, string val)
        {
            this.AddWhere(attr, " NOT IN ", " ( " + val + " ) ");
        }
        /// <summary>
        /// 增加条件, DataTable 第一列的值．
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="dt">第一列是要组合的values</param>
        public void AddWhereIn(string attr, DataTable dt)
        {
            string strs = "";
            foreach (DataRow dr in dt.Rows)
            {
                strs += dr[0].ToString() + ",";
            }
            strs = strs.Substring(strs.Length - 1, 0);
            this.AddWhereIn(attr, strs);
        }
        /// <summary>
        /// 增加条件,vals 必须是sql可以识别的字串．
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="vals">用 , 分开的．</param>
        public void AddWhereIn(string attr, string vals)
        {
            this.AddWhere(attr, " IN ", vals);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="exp"></param>
        /// <param name="val"></param>
        public void AddWhere(string attr, string exp, string val)
        {
            AddWhere(attr, exp, val, null);
        }
        /// <summary>
        /// 增加条件
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="exp">操作符号（根据不同的数据库）</param>
        /// <param name="val">值</param>
        /// <param name="paraName">参数名称，可以为null, 如果查询中有多个参数中有相同属性名的需要，分别给他们起一个参数名。</param>
        public void AddWhere(string attr, string exp, string val, string paraName)
        {
            if (val == null)
                val = "";

            if (val == "all")
            {
                this.SQL = "( 1=1 )";
                return;
            }

            if (exp.ToLower().Contains(" in"))
            {
                this.SQL = "( " + attr2Field(attr) + " " + exp + "  " + val + " )";
                return;
            }

            if (exp.ToLower().Contains("like"))
            {
                if (attr == "FK_Dept")
                {
                    val = val.Replace("'", "");
                    val = val.Replace("%", "");

                    switch (this.HisDBType)
                    {
                        case DBType.Oracle:
                            this.SQL = "( " + attr2Field(attr) + " " + exp + " '%'||" + this.HisVarStr + "FK_Dept||'%' )";
                            this.MyParas.Add("FK_Dept", val);
                            break;
                        default:
                            //this.SQL = "( " + attr2Field(attr) + " " + exp + "  '" + this.HisVarStr + "FK_Dept%' )";
                            this.SQL = "( " + attr2Field(attr) + " " + exp + "  '" + val + "%' )";
                            //this.MyParas.Add("FK_Dept", val);
                            break;
                    }
                }
                else
                {
                    if (val.Contains(":") || val.Contains("@"))
                    {
                        this.SQL = "( " + attr2Field(attr) + " " + exp + "  " + val + " )";
                    }
                    else
                    {
                        if (val.Contains("'") == false)
                            this.SQL = "( " + attr2Field(attr) + " " + exp + "  '" + val + "' )";
                        else
                            this.SQL = "( " + attr2Field(attr) + " " + exp + "  " + val + " )";
                    }
                }
                return;
            }
            if (this.HisVarStr == "?")
            {
                this.SQL = "( " + attr2Field(attr) + " " + exp + "?)";
                this.MyParas.Add(attr, val);
            }
            else
            {
                if (paraName == null)
                {
                    this.SQL = "( " + attr2Field(attr) + " " + exp + this.HisVarStr + attr + ")";
                    this.MyParas.Add(attr, val);
                }
                else
                {
                    this.SQL = "( " + attr2Field(attr) + " " + exp + this.HisVarStr + paraName + ")";
                    this.MyParas.Add(paraName, val);
                }
            }
        }
        public void AddWhereDept(string val)
        {
            string attr = "FK_Dept";
            string exp = "=";

            if (val.Contains("'") == false)
                this.SQL = "( " + attr2Field(attr) + " " + exp + "  '" + val + "' )";
            else
                this.SQL = "( " + attr2Field(attr) + " " + exp + "  " + val + " )";
        }
        /// <summary>
        /// 是空的
        /// </summary>
        /// <param name="attr"></param>
        public void AddWhereIsNull(string attr)
        {
            this.SQL = "( " + attr2Field(attr) + "  IS NULL OR  " + attr2Field(attr) + "='' )";
        }
        public void AddWhereField(string attr, string exp, string val)
        {
            if (val.ToString() == "all")
            {
                this.SQL = "( 1=1 )";
                return;
            }

            if (exp.ToLower().Contains(" in"))
            {
                this.SQL = "( " + attr + " " + exp + "  " + val + " )";
                return;
            }

            this.SQL = "( " + attr + " " + exp + " :" + attr + " )";
            this.MyParas.Add(attr, val);
        }
        public void AddWhereField(string attr, string exp, int val)
        {
            if (val.ToString() == "all")
            {
                this.SQL = "( 1=1 )";
                return;
            }

            if (exp.ToLower().Contains(" in"))
            {
                this.SQL = "( " + attr + " " + exp + "  " + val + " )";
                return;
            }

            if (attr == "RowNum")
            {
                this.SQL = "( " + attr + " " + exp + "  " + val + " )";
                return;
            }

            if (this.HisVarStr == "?")
                this.SQL = "( " + attr + " " + exp + "?)";
            else
                this.SQL = "( " + attr + " " + exp + "  " + this.HisVarStr + attr + " )";

            this.MyParas.Add(attr, val);
        }
        /// <summary>
        /// 增加条件
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="exp">操作符号（根据不同的数据库）</param>
        /// <param name="val">值</param>
        public void AddWhere(string attr, string exp, int val)
        {
            if (attr == "RowNum")
            {
                this.SQL = "( " + attr2Field(attr) + " " + exp + " " + val + ")";
            }
            else
            {
                if (this.HisVarStr == "?")
                    this.SQL = "( " + attr2Field(attr) + " " + exp + "?)";
                else
                    this.SQL = "( " + attr2Field(attr) + " " + exp + this.HisVarStr + attr + ")";

                this.MyParas.Add(attr, val);
            }
        }
        public void AddHD()
        {
            this.SQL = "(  1=1 ) ";
        }
        /// <summary>
        /// 非恒等。
        /// </summary>
        public void AddHD_Not()
        {
            this.SQL = "(  1=2 ) ";
        }
        /// <summary>
        /// 增加条件
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="exp">操作符号（根据不同的数据库）</param>
        /// <param name="val">值</param>
        public void AddWhere(string attr, string exp, float val)
        {
            this.MyParas.Add(attr, val);
            if (this.HisVarStr == "?")
                this.SQL = "( " + attr2Field(attr) + " " + exp + "?)";
            else
                this.SQL = "( " + attr2Field(attr) + " " + exp + " " + this.HisVarStr + attr + ")";
        }
        /// <summary>
        /// 增加条件(默认的是= )
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="val">值</param>
        public void AddWhere(string attr, string val)
        {
            this.AddWhere(attr, "=", val);
        }
        /// <summary>
        /// 增加条件(默认的是= )
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="val">值</param>
        public void AddWhere(string attr, int val)
        {
            this.AddWhere(attr, "=", val);
        }
        /// <summary>
        /// 增加条件
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="val">值 true/false</param>
        public void AddWhere(string attr, bool val)
        {
            if (val)
                this.AddWhere(attr, "=", 1);
            else
                this.AddWhere(attr, "=", 0);
        }
        public void AddWhere(string attr, Int64 val)
        {
            this.AddWhere(attr, val.ToString());
        }
        public void AddWhere(string attr, float val)
        {
            this.AddWhere(attr, "=", val);
        }
        public void AddWhere(string attr, object val)
        {
            if (val == null)
                throw new Exception("Attr=" + attr + ", 值是空 is null");

            if (val.GetType() == typeof(int) || val.GetType() == typeof(Int32))
            {
                //int i = int.Parse(val.ToString()) ;
                this.AddWhere(attr, "=", (Int32)val);
                return;
            }
            this.AddWhere(attr, "=", val.ToString());
        }

        public void addLeftBracket()
        {
            this.SQL = " ( ";
        }

        public void addRightBracket()
        {
            this.SQL = " ) ";
        }

        public void addAnd()
        {
            this.SQL = " AND ";
        }

        public void addOr()
        {
            this.SQL = " OR ";
        }

        #region 关于endsql
        public void addGroupBy(string attr)
        {
            this._groupBy = " GROUP BY  " + attr2Field(attr);
        }

        public void addGroupBy(string attr1, string attr2)
        {
            this._groupBy = " GROUP BY  " + attr2Field(attr1) + " , " + attr2Field(attr2);
        }

        private string _orderBy = "";
        public void addOrderBy(string attr)
        {
            if (this._orderBy.IndexOf("ORDER BY") != -1)
            {
                this._orderBy = " , " + attr2Field(attr);
            }
            else
            {
                this._orderBy = " ORDER BY " + attr2Field(attr);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attr"></param>
        public void addOrderByRandom()
        {
            if (this._orderBy.IndexOf("ORDER BY") != -1)
            {
                this._orderBy = " , NEWID()";
            }
            else
            {
                this._orderBy = " ORDER BY NEWID()";
            }
        }
        /// <summary>
        /// addOrderByDesc
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="desc"></param>
        public void addOrderByDesc(string attr)
        {
            this._orderBy = " ORDER BY " + attr2Field(attr) + " DESC ";
        }
        public void addOrderByDesc(string attr1, string attr2)
        {
            this._orderBy = " ORDER BY  " + attr2Field(attr1) + " DESC ," + attr2Field(attr2) + " DESC";
        }
        public void addOrderBy(string attr1, string attr2)
        {
            this._orderBy = " ORDER BY  " + attr2Field(attr1) + "," + attr2Field(attr2);
        }
        #endregion

        public void addHaving() { }
        /// 清除查询条件
        public void clear()
        {
            this._sql = "";
            this._groupBy = "";
            //this._orderBy = "";
            this.MyParas.Clear();
        }
        private Map _HisMap;
        public Map HisMap
        {
            get
            {
                if (_HisMap == null)
                    _HisMap = this.En.EnMap;
                return _HisMap;
            }
            set
            {
                _HisMap = value;
            }
        }
        /// <summary>
        /// 增加字段.
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        private string attr2Field(string attrKey)
        {
            Attr attr = this.HisMap.GetAttrByKey(attrKey);
            if (attr.IsRefAttr == true)
            {
              //  Entity en = attr.HisFKEn;
                if (this.HisDBType == DBType.Oracle)
                    return "T" + attr.Key.Replace("Text", "") + ".Name";
                else
                {
                    Entity en = attr.HisFKEn;
                    return en.EnMap.PhysicsTable + "_" + attr.Key.Replace("Text", "") + ".Name";
                }
                   
            }

            return this.HisMap.PhysicsTable + "." + attr.Field;
           // return this.HisMap.PhysicsTable + "."+attr;
        }
        public DataTable DoGroupReturnTable(Entity en, Attrs attrsOfGroupKey, Attr attrGroup, GroupWay gw, OrderWay ow)
        {
            switch (en.EnMap.EnDBUrl.DBType)
            {
                case DBType.Oracle:
                    return DoGroupReturnTableOracle(en, attrsOfGroupKey, attrGroup, gw, ow);
                default:
                    return DoGroupReturnTableSqlServer(en, attrsOfGroupKey, attrGroup, gw, ow);
            }
        }
        public DataTable DoGroupReturnTableOracle(Entity en, Attrs attrsOfGroupKey, Attr attrGroup, GroupWay gw, OrderWay ow)
        {
            #region  生成要查询的语句
            string fields = "";
            string str = "";
            foreach (Attr attr in attrsOfGroupKey)
            {
                if (attr.Field == null)
                    continue;

                str = "," + attr.Field;
                fields += str;
            }

            if (attrGroup.Key == "MyNum")
            {
                switch (gw)
                {
                    case GroupWay.BySum:
                        fields += ", COUNT(*) AS MyNum";
                        break;
                    case GroupWay.ByAvg:
                        fields += ", AVG(" + attrGroup.Field + ") AS MyNum";
                        break;
                    default:
                        throw new Exception("no such case:");
                }
            }
            else
            {
                switch (gw)
                {
                    case GroupWay.BySum:
                        fields += ",SUM(" + attrGroup.Field + ") AS " + attrGroup.Key;
                        break;
                    case GroupWay.ByAvg:
                        fields += ",AVG(" + attrGroup.Field + ") AS " + attrGroup.Key;
                        break;
                    default:
                        throw new Exception("no such case:");
                }
            }

            string by = "";
            foreach (Attr attr in attrsOfGroupKey)
            {
                if (attr.Field == null)
                    continue;

                str = "," + attr.Field;
                by += str;
            }
            by = by.Substring(1);
            //string sql 
            string sql = "SELECT " + fields.Substring(1) + " FROM " + this.En.EnMap.PhysicsTable + " WHERE " + this._sql + " Group BY " + by;
            #endregion

            #region
            Map map = new Map();
            map.PhysicsTable = "@VT@";
            map.Attrs = attrsOfGroupKey;
            map.Attrs.Add(attrGroup);
            #endregion .

            string sql1 = SqlBuilder.SelectSQLOfOra(en.ToString(), map) + " " + SqlBuilder.GenerFormWhereOfOra(en, map);

            sql1 = sql1.Replace("@TopNum", "");
            sql1 = sql1.Replace("FROM @VT@", "FROM (" + sql + ") VT");
            sql1 = sql1.Replace("@VT@", "VT");
            sql1 = sql1.Replace("TOP", "");

            if (ow == OrderWay.OrderByUp)
                sql1 += " ORDER BY " + attrGroup.Key + " DESC ";
            else
                sql1 += " ORDER BY " + attrGroup.Key;

            return DBAccess.RunSQLReturnTable(sql1, this.MyParas);
        }

        public DataTable DoGroupReturnTableSqlServer(Entity en, Attrs attrsOfGroupKey, Attr attrGroup, GroupWay gw, OrderWay ow)
        {

            #region  生成要查询的语句
            string fields = "";
            string str = "";
            foreach (Attr attr in attrsOfGroupKey)
            {
                if (attr.Field == null)
                    continue;
                str = "," + attr.Field;
                fields += str;
            }

            if (attrGroup.Key == "MyNum")
            {
                switch (gw)
                {
                    case GroupWay.BySum:
                        fields += ", COUNT(*) AS MyNum";
                        break;
                    case GroupWay.ByAvg:
                        fields += ", AVG(*)   AS MyNum";
                        break;
                    default:
                        throw new Exception("no such case:");
                }
            }
            else
            {
                switch (gw)
                {
                    case GroupWay.BySum:
                        fields += ",SUM(" + attrGroup.Field + ") AS " + attrGroup.Key;
                        break;
                    case GroupWay.ByAvg:
                        fields += ",AVG(" + attrGroup.Field + ") AS " + attrGroup.Key;
                        break;
                    default:
                        throw new Exception("no such case:");
                }
            }

            string by = "";
            foreach (Attr attr in attrsOfGroupKey)
            {
                if (attr.Field == null)
                    continue;

                str = "," + attr.Field;
                by += str;
            }
            by = by.Substring(1);
            //string sql 
            string sql = "SELECT " + fields.Substring(1) + " FROM " + this.En.EnMap.PhysicsTable + " WHERE " + this._sql + " Group BY " + by;
            #endregion

            #region
            Map map = new Map();
            map.PhysicsTable = "@VT@";
            map.Attrs = attrsOfGroupKey;
            map.Attrs.Add(attrGroup);
            #endregion .
            //string sql1=SqlBuilder.SelectSQLOfMS( map )+" "+SqlBuilder.GenerFormWhereOfMS( en,map) + "   AND ( " + this._sql+" ) "+_endSql;

            string sql1 = SqlBuilder.SelectSQLOfMS(map) + " " + SqlBuilder.GenerFormWhereOfMS(en, map);

            sql1 = sql1.Replace("@TopNum", "");

            sql1 = sql1.Replace("FROM @VT@", "FROM (" + sql + ") VT");

            sql1 = sql1.Replace("@VT@", "VT");
            sql1 = sql1.Replace("TOP", "");
            if (ow == OrderWay.OrderByUp)
                sql1 += " ORDER BY " + attrGroup.Key + " DESC ";
            else
                sql1 += " ORDER BY " + attrGroup.Key;
            return DBAccess.RunSQLReturnTable(sql1, this.MyParas);
        }
        /// <summary>
        /// 分组查询，返回datatable.
        /// </summary>
        /// <param name="attrsOfGroupKey"></param>
        /// <param name="groupValField"></param>
        /// <param name="gw"></param>
        /// <returns></returns>
        public DataTable DoGroupReturnTable1(Entity en, Attrs attrsOfGroupKey, Attr attrGroup, GroupWay gw, OrderWay ow)
        {
            #region  生成要查询的语句
            string fields = "";
            string str = "";
            foreach (Attr attr in attrsOfGroupKey)
            {
                if (attr.Field == null)
                    continue;
                str = "," + attr.Field;
                fields += str;
            }

            if (attrGroup.Key == "MyNum")
            {
                switch (gw)
                {
                    case GroupWay.BySum:
                        fields += ", COUNT(*) AS MyNum";
                        break;
                    case GroupWay.ByAvg:
                        fields += ", AVG(*)   AS MyNum";
                        break;
                    default:
                        throw new Exception("no such case:");
                }
            }
            else
            {
                switch (gw)
                {
                    case GroupWay.BySum:
                        fields += ",SUM(" + attrGroup.Field + ") AS " + attrGroup.Key;
                        break;
                    case GroupWay.ByAvg:
                        fields += ",AVG(" + attrGroup.Field + ") AS " + attrGroup.Key;
                        break;
                    default:
                        throw new Exception("no such case:");
                }
            }

            string by = "";
            foreach (Attr attr in attrsOfGroupKey)
            {
                if (attr.Field == null)
                    continue;

                str = "," + attr.Field;
                by += str;
            }
            by = by.Substring(1);
            //string sql 
            string sql = "SELECT " + fields.Substring(1) + " FROM " + this.En.EnMap.PhysicsTable + " WHERE " + this._sql + " Group BY " + by;
            #endregion

            #region
            Map map = new Map();
            map.PhysicsTable = "@VT@";
            map.Attrs = attrsOfGroupKey;
            map.Attrs.Add(attrGroup);
            #endregion .

            //string sql1=SqlBuilder.SelectSQLOfMS( map )+" "+SqlBuilder.GenerFormWhereOfMS( en,map) + "   AND ( " + this._sql+" ) "+_endSql;

            string sql1 = SqlBuilder.SelectSQLOfMS(map) + " " + SqlBuilder.GenerFormWhereOfMS(en, map);

            sql1 = sql1.Replace("@TopNum", "");
            sql1 = sql1.Replace("FROM @VT@", "FROM (" + sql + ") VT");
            sql1 = sql1.Replace("@VT@", "VT");
            sql1 = sql1.Replace("TOP", "");
            if (ow == OrderWay.OrderByUp)
                sql1 += " ORDER BY " + attrGroup.Key + " DESC ";
            else
                sql1 += " ORDER BY " + attrGroup.Key;
            return DBAccess.RunSQLReturnTable(sql1);
        }
        public string[] FullAttrs = null;
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns></returns>
        public int DoQuery()
        {
            try
            {
                if (this._en == null)
                    return this.doEntitiesQuery();
                else
                    return this.doEntityQuery();
            }
            catch (Exception ex)
            {
                if (this._en == null)
                    this._ens.GetNewEntity.CheckPhysicsTable();
                else
                    this._en.CheckPhysicsTable();
                throw ex;
            }
        }
        public int DoQueryBak20111203()
        {
            try
            {
                if (this._en == null)
                {
                    return this.doEntitiesQuery();
                }
                else
                    return this.doEntityQuery();
            }
            catch (Exception ex)
            {
                try
                {
                    if (this._en == null)
                        this.Ens.GetNewEntity.CheckPhysicsTable();
                    else
                        this._en.CheckPhysicsTable();
                }
                catch
                {
                }
                throw ex;
            }
        }
        public string DealString(string sql)
        {
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            string strs = "";
            foreach (DataRow dr in dt.Rows)
            {
                strs += ",'" + dr[0].ToString() + "'";
            }
            return strs.Substring(1);
        }
        public string GenerPKsByTableWithPara(string pk, string sql, int from, int to)
        {
            //Log.DefaultLogWriteLineWarning(" ***************************** From= " + from + "  T0" + to);
            DataTable dt = DBAccess.RunSQLReturnTable(sql, this.MyParas);
            string pks = "";
            int i = 0;
            int paraI = 0;

            string dbStr = SystemConfig.AppCenterDBVarStr;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                if (i > from)
                {
                    paraI++;
                    //pks += "'" + dr[0].ToString() + "'";
                    if (dbStr == "?")
                        pks += "?,";
                    else
                        pks += SystemConfig.AppCenterDBVarStr + "R" + paraI + ",";

                    this.MyParasR.Add("R" + paraI, dr[0].ToString());
                    if (i >= to)
                        return pks.Substring(0, pks.Length - 1);
                }
            }
            if (pks == "")
            {
                return null;
                //return " '1'  ";
                return "  ";
            }
            return pks.Substring(0, pks.Length - 1);
        }
        public string GenerPKsByTable(string sql, int from, int to)
        {
            //Log.DefaultLogWriteLineWarning(" ***************************** From= " + from + "  T0" + to);
            DataTable dt = DBAccess.RunSQLReturnTable(sql, this.MyParas);
            string pks = "";
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                if (i > from)
                {
                    if (i >= to)
                    {
                        pks += "'" + dr[0].ToString() + "'";
                        return pks;
                    }
                    else
                        pks += "'" + dr[0].ToString() + "',";
                }
            }
            if (pks == "")
                return "  '11111111' ";
            return pks.Substring(0, pks.Length - 1);
        }
        /// <summary>
        /// 删除当前查询的排序字段，然后可以再次增加其他的排序字段
        /// <para>added by liuxc,2015.3.18,为解决默认增加的是主键字段排序，但此排序字段未提供删除方法的问题</para>
        /// </summary>
        public void ClearOrderBy()
        {
            this._orderBy = string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIdx"></param>
        /// <returns></returns>
        public int DoQuery(string pk, int pageSize, int pageIdx)
        {
            if (pk == "OID" || pk=="WorkID" )
                return DoQuery(pk, pageSize, pageIdx, pk, true);
            else
                return DoQuery(pk, pageSize, pageIdx, pk, false);
        }
        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <param name="pk">主键</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIdx">第x页</param>
        /// <param name="orderby">排序</param>
        /// <param name="orderway">排序方式: 两种情况 Down UP </param>
        /// <returns>查询结果</returns>
        public int DoQuery(string pk, int pageSize, int pageIdx, string orderBy, string orderWay)
        {
            if (orderWay.ToLower().Trim() == "up")
                return DoQuery(pk, pageSize, pageIdx, orderBy, false);
            else
                return DoQuery(pk, pageSize, pageIdx, orderBy, true);
        }
        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <param name="pk">主键</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIdx">第x页</param>
        /// <param name="orderby">排序</param>
        /// <returns>查询结果</returns>
        public int DoQuery(string pk, int pageSize, int pageIdx, bool isDesc)
        {
            return DoQuery(pk, pageSize, pageIdx, pk, isDesc);
        }
        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <param name="pk">主键</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIdx">第x页</param>
        /// <param name="orderby">排序</param>
        /// <param name="orderway">排序方式: 两种情况 desc 或者 为 null. </param>
        /// <returns>查询结果</returns>
        public int DoQuery(string pk, int pageSize, int pageIdx, string orderBy, bool isDesc)
        {
            int pageNum = 0;
            
            //如果没有加入排序字段，使用主键
            if (DataType.IsNullOrEmpty(this._orderBy))
            {
                string isDescStr = "";
                if (isDesc)
                    isDescStr = " DESC ";

                if (DataType.IsNullOrEmpty(orderBy)) 
                    orderBy = pk;

                this._orderBy =  attr2Field(orderBy) + isDescStr;
            }

            if (this._orderBy.Contains("ORDER BY") == false)
                _orderBy = " ORDER BY " + this._orderBy;

            try
            {
                if (this._en == null)
                {
                    int recordConut = 0;
                    recordConut = this.GetCount(); // 获取 它的数量。

                    if (recordConut == 0)
                    {
                        this._ens.Clear();
                        return 0;
                    }

                    // xx!5555 提出的错误.
                    if (pageSize == 0)
                        pageSize = 12;

                    decimal pageCountD = decimal.Parse(recordConut.ToString()) / decimal.Parse(pageSize.ToString()); // 页面个数。
                    string[] strs = pageCountD.ToString("0.0000").Split('.');
                    if (int.Parse(strs[1]) > 0)
                        pageNum = int.Parse(strs[0]) + 1;
                    else
                        pageNum = int.Parse(strs[0]);

                    int myleftCount = recordConut - (pageNum * pageSize);

                    pageNum++;
                    int top = pageSize * (pageIdx - 1);

                    string sql = "";
                    Entity en = this._ens.GetNewEntity;
                    Map map = en.EnMap;
                    int toIdx = 0;
                    string pks = "";
                    switch (map.EnDBUrl.DBType)
                    {
                        case DBType.Oracle:
                            toIdx = top + pageSize;
                            if (this._sql == "" || this._sql == null)
                            {
                               

                                if (top == 0)
                                    sql = "SELECT * FROM ( SELECT  " + pk + " FROM " + map.PhysicsTable + " " + this._orderBy + "   ) WHERE ROWNUM <=" + pageSize;
                                else
                                    sql = "SELECT * FROM ( SELECT  " + pk + " FROM " + map.PhysicsTable + " " + this._orderBy + ") ";
                            }
                            else
                            {
                                string mysql = this.SQL;
                                mysql = mysql.Substring(mysql.IndexOf("FROM "));

                                if (top == 0)
                                    sql = "SELECT * FROM ( SELECT " + map.PhysicsTable + "." + pk + " " + mysql + " )  WHERE ROWNUM <=" + pageSize;
                                else
                                    sql = "SELECT * FROM ( SELECT " + map.PhysicsTable + "." + pk + " " + mysql + " ) ";
                                //sql = "SELECT * FROM ( SELECT  " + pk + " FROM " + map.PhysicsTable + " WHERE " + this._sql + " " + this._orderBy + "   ) ";
                            }

                            sql = sql.Replace("AND ( ( 1=1 ) )", " ");

                            pks = this.GenerPKsByTableWithPara(pk, sql, top, toIdx);
                            this.clear();
                            this.MyParas = this.MyParasR;
                            if (pks != null)
                                this.AddWhereIn(pk, "(" + pks + ")");
                            else
                                this.AddHD();

                            this.Top = pageSize;
                            return this.doEntitiesQuery();
                        case DBType.Informix:
                            toIdx = top + pageSize;
                            if (this._sql == "" || this._sql == null)
                            {
                                if (top == 0)
                                    sql = " SELECT first " + pageSize + "  " + this.En.PKField + " FROM " + map.PhysicsTable + " " + this._orderBy;
                                else
                                    sql = " SELECT  " + this.En.PKField + " FROM " + map.PhysicsTable + " " + this._orderBy;
                            }
                            else
                            {
                                string mysql = this.SQL;
                                mysql = mysql.Substring(mysql.IndexOf("FROM "));
                                if (top == 0)
                                    sql = "SELECT first " + pageSize + " " + this.En.PKField + "  " + mysql;
                                else
                                    sql = "SELECT  " + this.En.PKField + " " + mysql;
                            }

                            sql = sql.Replace("AND ( ( 1=1 ) )", " ");

                            pks = this.GenerPKsByTableWithPara(pk, sql, top, toIdx);
                            this.clear();
                            this.MyParas = this.MyParasR;

                            if (pks == null)
                                this.AddHD_Not();
                            else
                                this.AddWhereIn(pk, "(" + pks + ")");

                            this.Top = pageSize;
                            return this.doEntitiesQuery();
                        case DBType.MySQL:
                            toIdx = top + pageSize;
                            if (this._sql == "" || this._sql == null)
                            {
                                if (top == 0)
                                    sql = " SELECT  " + this.En.PKField + " FROM " + map.PhysicsTable + " " + this._orderBy + " LIMIT "+pageSize;
                                else
                                    sql = " SELECT  " + this.En.PKField + " FROM " + map.PhysicsTable + " " + this._orderBy;
                            }
                            else
                            {
                                string mysql = this.SQL;
                                mysql = mysql.Substring(mysql.IndexOf("FROM "));

                                if (top == 0)
                                    sql = "SELECT " + map.PhysicsTable + "." + this.En.PKField + " " + mysql + " LIMIT " + pageSize;
                                else
                                    sql = "SELECT " + map.PhysicsTable + "." + this.En.PKField + " " + mysql;
                            }

                            sql = sql.Replace("AND ( ( 1=1 ) )", " ");

                            pks = this.GenerPKsByTableWithPara(pk, sql, top, toIdx);
                            this.clear();
                            this.MyParas = this.MyParasR;

                            if (pks == null)
                                this.AddHD_Not();
                            else
                                this.AddWhereIn(pk, "(" + pks + ")");

                            this.Top = pageSize;
                            return this.doEntitiesQuery();
                        case DBType.MSSQL:
                        default:
                            toIdx = top + pageSize;
                            if (this._sql == "" || this._sql == null)
                            {
                                //此处去掉原有的第1页时用top pagesize的写法，会导致第1页数据查询出来的不准确，统一都用下面的写法，edited by liuxc,2017-8-30
                                //此处查询数据，除第1页外，有可能会造排序不正确，但每一页的数据是准确的，限于原有写法，没法改动此处逻辑解决这个问题
                                    sql = " SELECT  [" + this.En.PKField + "] FROM " + map.PhysicsTable + " " + this._orderBy;
                            }
                            else
                            {
                                    string mysql = this.SQL;
                                    mysql = mysql.Substring(mysql.IndexOf("FROM "));
                                    sql = "SELECT " + map.PhysicsTable + "." + this.En.PKField + " as  [" + this.En.PKField + "]  " + mysql;
                            }

                            sql = sql.Replace("AND ( ( 1=1 ) )", " ");

                            pks = this.GenerPKsByTableWithPara(pk, sql, top, toIdx);
                            this.clear();
                            this.MyParas = this.MyParasR;

                            if (pks == null)
                                this.AddHD_Not();
                            else
                                this.AddWhereIn(pk, "(" + pks + ")");

                            this.Top = pageSize;
                            return this.doEntitiesQuery();
                    }
                }
                else
                    return this.doEntityQuery();
            }
            catch (Exception ex)
            {
                try
                {
                    if (this._en == null)
                        this.Ens.GetNewEntity.CheckPhysicsTable();
                    else
                        this._en.CheckPhysicsTable();
                }
                catch
                {
                }
                throw ex;
            }
        }
        /// <summary>
        /// 按照
        /// </summary>
        /// <returns></returns>
        public DataTable DoQueryToTable()
        {
            try
            {
                string sql = this.SQL;
                sql = sql.Replace("WHERE (1=1) AND ( AND ( ( ( 1=1 ) ) AND ( ( 1=1 ) ) ) )", "");

                return DBAccess.RunSQLReturnTable(sql, this.MyParas);
            }
            catch (Exception ex)
            {
                if (this._en == null)
                    this.Ens.GetNewEntity.CheckPhysicsTable();
                else
                    this._en.CheckPhysicsTable();
                throw ex;
            }
        }
        /// <summary>
        /// 得到返回的数量
        /// </summary>
        /// <returns>得到返回的数量</returns>
        public int GetCount()
        {
            string sql = this.SQL;
            //sql="SELECT COUNT(*) "+sql.Substring(sql.IndexOf("FROM") ) ;
            string ptable = this.En.EnMap.PhysicsTable;
            string pk = this.En.PKField;

            switch (this.En.EnMap.EnDBUrl.DBType)
            {
                case DBType.Oracle:
                    if (this._sql == "" || this._sql == null)
                        sql = "SELECT COUNT(" + ptable + "." + pk + ") as C FROM " + ptable;
                    else
                        sql = "SELECT COUNT(" + ptable + "." + pk + ") as C " + sql.Substring(sql.IndexOf("FROM "));
                    break;
                default:
                    if (this._sql == "" || this._sql == null)
                        sql = "SELECT COUNT(" + ptable + "." + pk + ") as C FROM " + ptable;
                    else
                    {
                        sql = sql.Substring(sql.IndexOf("FROM "));
                        if(sql.IndexOf("ORDER BY")>=0)
                            sql = sql.Substring(0,sql.IndexOf("ORDER BY")-1);
                        sql = "SELECT COUNT(" + ptable + "." + pk + ") as C " + sql;
                    }
                      

                    //sql="SELECT COUNT(*) as C "+this._endSql  +sql.Substring(  sql.IndexOf("FROM ") ) ;
                    //sql="SELECT COUNT(*) as C FROM "+ this._ens.GetNewEntity.EnMap.PhysicsTable+ "  " +sql.Substring(sql.IndexOf("WHERE") ) ;
                    //int i = sql.IndexOf("ORDER BY") ;
                    //if (i!=-1)
                    //	sql=sql.Substring(0,i);
                    break;
            }
            try
            {
                int i = DBAccess.RunSQLReturnValInt(sql, this.MyParas);
                if (this.Top == -1)
                    return i;

                if (this.Top >= i)
                    return i;
                else
                    return this.Top;
            }
            catch (Exception ex)
            {
             //   if (SystemConfig.IsDebug)
                this.En.CheckPhysicsTable();
                throw ex;
            }
        }
        /// <summary>
        /// 最大的数量
        /// </summary>
        /// <param name="topNum">最大的数量</param>
        /// <returns>要查询的信息</returns>
        public DataTable DoQueryToTable(int topNum)
        {
            this.Top = topNum;
            return DBAccess.RunSQLReturnTable(this.SQL, this.MyParas);
        }

        private int doEntityQuery()
        {
            return EntityDBAccess.Retrieve(this.En, this.SQL, this.MyParas);
        }
        private int doEntitiesQuery()
        {
            switch (this.HisDBType)
            {
                case DBType.Oracle:
                    if (this.Top != -1)
                    {
                        this.addAnd();
                        this.AddWhereField("RowNum", "<=", this.Top);
                    }
                    break;
                case DBType.MSSQL:
                case DBType.MySQL:
                default:
                    break;
            }
            return EntityDBAccess.Retrieve(this.Ens, this.SQL, this.MyParas, this.FullAttrs);
        }
        /// <summary>
        /// 根据data初始化entiies.
        /// </summary>
        /// <param name="ens">实体s</param>
        /// <param name="dt">数据表</param>
        /// <param name="fullAttrs">要填充的树形</param>
        /// <returns>初始化后的ens</returns>
        public static Entities InitEntitiesByDataTable(Entities ens, DataTable dt, string[] fullAttrs)
        {
            if (fullAttrs == null)
            {
                Map enMap = ens.GetNewEntity.EnMap;
                Attrs attrs = enMap.Attrs;
                try
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Entity en = ens.GetNewEntity;
                        foreach (Attr attr in attrs)
                        {
                            //if (attr.IsRefAttr)
                            //    continue;
                            if (dt.Columns.Contains(attr.Key) == false)
                                continue;
                            en.Row.SetValByKey(attr.Key, dr[attr.Key]);
                        }
                        ens.AddEntity(en);
                    }
                }
                catch (Exception ex)
                {
#warning 不应该出现的错误. 2011-12-03 add
                    string cols = "";
                    foreach (DataColumn dc in dt.Columns)
                    {
                        cols += " , " + dc.ColumnName;
                    }
                    throw new Exception("Columns=" + cols + "@Ens=" + ens.ToString() + " @异常信息:" + ex.Message);
                }
            }
            else
            {

                foreach (DataRow dr in dt.Rows)
                {
                    Entity en = ens.GetNewEntity;
                    foreach (string str in fullAttrs)
                        en.Row.SetValByKey(str, dr[str]);
                    ens.AddEntity(en);
                }
            }
            return ens;
        }
    }
}
