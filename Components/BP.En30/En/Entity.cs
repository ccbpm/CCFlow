using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using BP.DA;
using System.Data;
using BP.Sys;
using BP.Sys.XML;
using System.Reflection;

namespace BP.En
{
    /// <summary>
    /// Entity 的摘要说明。
    /// </summary>	
    [Serializable]
    abstract public class Entity : EnObj
    {
        #region 自动标记获取属性实体方法.
        /// <summary>
        /// 从AutoNum缓存中获取实体s
        /// </summary>
        /// <param name="ens">实体集合</param>
        /// <param name="refKey">查询的外键</param>
        /// <param name="val">外键值</param>
        /// <returns>返回实体集合</returns>
        public Entities GetEntitiesAttrFromAutoNumCash(Entities ens,
            string refKey, object refVal, string refKey2 = null, object refVal2 = null)
        {
            //获得段类名.
            string clsName = ens.ClassIDOfShort;

            //判断内存是否有？
            Entities objs = this.GetRefObject(clsName) as Entities;
            if (objs != null)
                return objs; //如果缓存有值，就直接返回.

            int count = this.GetParaInt(clsName + "_AutoNum", -1);
            if (count == -1)
            {
                if (refKey2 == null)
                    ens.Retrieve(refKey, refVal);
                else
                    ens.Retrieve(refKey, refVal, refKey2, refVal2);

                this.SetPara(clsName + "_AutoNum", ens.Count); //设置他的数量.
                this.DirectUpdate();
                this.SetRefObject(clsName, ens);
                return ens;
            }

            if (count == 0)
            {
                ens.Clear();
                this.SetRefObject(clsName, ens);
                return ens;
            }

            if (refKey2 == null)
                ens.Retrieve(refKey, refVal);
            else
                ens.Retrieve(refKey, refVal, refKey2, refVal2);

            if (ens.Count != count)
            {
                this.SetPara(clsName + "_AutoNum", ens.Count); //设置他的数量.
                this.DirectUpdate();
            }

            this.SetRefObject(clsName, ens);
            return ens;
        }
        /// <summary>
        /// 清除缓存记录
        /// 把值设置为 -1,执行的时候，让其重新获取.
        /// </summary>
        public void ClearAutoNumCash(bool isUpdata = true)
        {
            bool isHave = false;
            foreach (string key in this.atPara.HisHT.Keys)
            {
                if (DataType.IsNullOrEmpty(key) == true)
                    continue;

                if (key.EndsWith("_AutoNum") == true)
                {
                    if (this.GetParaInt(key) != -1)
                    {
                        this.SetPara(key, -1);
                        isHave = true;
                    }
                }
            }
            if (isHave == true && isUpdata == true)
                this.DirectUpdate();
        }
        #endregion 自动标记获取属性实体方法.


        #region mapInfotoJson

        #endregion

        #region 与缓存有关的操作
        private Entities _GetNewEntities = null;
        public virtual Entities GetNewEntities
        {
            get
            {
                if (_GetNewEntities == null)
                {
                    string str = this.ToString();
                    ArrayList al = ClassFactory.GetObjects("BP.En.Entities");
                    foreach (Object o in al)
                    {
                        Entities ens = o as Entities;

                        if (ens == null)
                            continue;
                        if (ens.GetNewEntity.ToString() == str)
                        {
                            _GetNewEntities = ens;
                            return _GetNewEntities;
                        }
                    }
                    throw new Exception("@no ens" + this.ToString());
                }
                return _GetNewEntities;
            }
        }
        /// <summary>
        /// 类名
        /// </summary>
        public virtual string ClassID
        {
            get
            {
                return this.ToString();
            }
        }
        /// <summary>
        /// 短类名
        /// </summary>
        public virtual string ClassIDOfShort
        {
            get
            {
                string clsID = this.ClassID;
                return clsID.Substring(clsID.LastIndexOf('.') + 1);
            }
        }
        #endregion

        #region 与sql操作有关
        protected SQLCash _SQLCash = null;
        public virtual SQLCash SQLCash
        {
            get
            {
                if (_SQLCash == null)
                {
                    _SQLCash = Cash.GetSQL(this.ToString());
                    if (_SQLCash == null)
                    {
                        _SQLCash = new SQLCash(this);
                        Cash.SetSQL(this.ToString(), _SQLCash);
                    }
                }
                return _SQLCash;
            }
            set
            {
                _SQLCash = value;
            }
        }
        /// <summary>
        /// 转化成
        /// </summary>
        /// <returns></returns>
        public string ToStringAtParas()
        {
            string str = "";
            foreach (Attr attr in this.EnMap.Attrs)
            {
                str += "@" + attr.Key + "=" + this.GetValByKey(attr.Key);
            }
            return str;
        }
        public KeyVals ToKeyVals()
        {
            KeyVals kvs = new KeyVals();
            foreach (Attr attr in this.EnMap.Attrs)
            {
                //kvs.Add(attr.Key, this.GetValByKey(attr.Key), );
            }
            return kvs;
        }

        /// <summary>
        /// 把一个实体转化成Json.
        /// </summary>
        /// <param name="isInParaFields">是否转换参数字段</param>
        /// <returns>返回该实体的单个json</returns>
        public string ToJson(bool isInParaFields = true)
        {
            Hashtable ht = this.Row;
            //如果不包含参数字段.
            if (isInParaFields == false)
                return BP.Tools.Json.ToJsonEntityModel(ht);



            if (ht.ContainsKey("AtPara") == false)
                return BP.Tools.Json.ToJsonEntityModel(ht);

            try
            {
                /*如果包含这个字段  @FK_BanJi=01 */
                AtPara ap = this.atPara;
                foreach (string key in ap.HisHT.Keys)
                {
                    if (ht.ContainsKey(key) == true)
                        continue;
                    ht.Add(key, ap.HisHT[key]);
                }

                //把参数属性移除.
                ht.Remove("_ATObj_");
            }
            catch (Exception ex)
            {
                Log.DebugWriteWarning("@ ToJson " + ex.Message);
            }

            return BP.Tools.Json.ToJson(ht);
        }

        /// <summary>
        /// 转化成json字符串，包含外键与枚举，主表使用Main命名。
        /// 外键使用外键表命名，枚举值使用枚举值命名。
        /// </summary>
        /// <returns></returns>
        public string ToJsonIncludeEnumAndForeignKey()
        {
            DataSet ds = new DataSet();

            ds.Tables.Add(this.ToDataTableField()); //把主表数据加入进去.

            Attrs attrs = this.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.UIBindKey == "")
                    continue;

                if (attr.IsEnum)
                {
                    if (ds.Tables.Contains(attr.UIBindKey))
                        continue;
                    SysEnums ses = new SysEnums(attr.UIBindKey);

                    DataTable dt = ses.ToDataTableField(attr.UIBindKey); //把枚举加入进去.
                    ds.Tables.Add(dt); //把这个枚举值加入进去..
                    continue;
                }

                if (attr.IsFK)
                {
                    if (ds.Tables.Contains(attr.UIBindKey))
                        continue;

                    Entities ens = attr.HisFKEns;
                    ens.RetrieveAll();

                    DataTable dt = ens.ToDataTableField(attr.UIBindKey); //把外键表加入进去.
                    ds.Tables.Add(dt); //把这个枚举值加入进去..
                    continue;
                }
            }
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 创建一个空的表
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public DataTable ToEmptyTableField(Entity en = null)
        {
            DataTable dt = new DataTable();
            if (en == null)
                en = this;

            dt.TableName = en.EnMap.PhysicsTable;

            foreach (Attr attr in en.EnMap.Attrs)
            {
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        dt.Columns.Add(new DataColumn(attr.Key, typeof(string)));
                        break;
                    case DataType.AppInt:
                        dt.Columns.Add(new DataColumn(attr.Key, typeof(int)));
                        break;
                    case DataType.AppFloat:
                        dt.Columns.Add(new DataColumn(attr.Key, typeof(float)));
                        break;
                    case DataType.AppBoolean:
                        dt.Columns.Add(new DataColumn(attr.Key, typeof(string)));
                        break;
                    case DataType.AppDouble:
                        dt.Columns.Add(new DataColumn(attr.Key, typeof(double)));
                        break;
                    case DataType.AppMoney:
                        dt.Columns.Add(new DataColumn(attr.Key, typeof(double)));
                        break;
                    case DataType.AppDate:
                        dt.Columns.Add(new DataColumn(attr.Key, typeof(string)));
                        break;
                    case DataType.AppDateTime:
                        dt.Columns.Add(new DataColumn(attr.Key, typeof(string)));
                        break;
                    default:
                        throw new Exception("@bulider insert sql error: 没有这个数据类型");
                }
            }
            return dt;
        }
        public DataTable ToDataTableField(string tableName = "Main")
        {
            DataTable dt = this.ToEmptyTableField(this);
            dt.TableName = tableName;

            //增加参数列.
            if (this.Row.ContainsKey("AtPara") == true)
            {
                /*如果包含这个字段,就说明他有参数,把参数也要弄成一个列.*/
                AtPara ap = this.atPara;
                foreach (string key in ap.HisHT.Keys)
                {
                    if (dt.Columns.Contains(key) == true)
                        continue;

                    dt.Columns.Add(key, typeof(string));
                }
            }

            DataRow dr = dt.NewRow();
            foreach (Attr attr in this.EnMap.Attrs)
            {
                if (attr.MyDataType == DataType.AppBoolean)
                {
                    if (this.GetValIntByKey(attr.Key) == 1)
                        dr[attr.Key] = "1";
                    else
                        dr[attr.Key] = "0";
                    continue;
                }

                /*如果是外键 就要去掉左右空格。
                 *  */
                if (attr.MyFieldType == FieldType.FK
                    || attr.MyFieldType == FieldType.PKFK)
                {
                    dr[attr.Key] = this.GetValByKey(attr.Key).ToString().Trim();
                }
                else
                {
                    var obj = this.GetValByKey(attr.Key);
                    if (obj == null && attr.IsNum)
                    {
                        dr[attr.Key] = 0;
                        continue;
                    }

                    if (attr.IsNum == true && DataType.IsNumStr(obj.ToString()) == false)
                        dr[attr.Key] = 0;
                    else
                        dr[attr.Key] = obj;
                }
            }

            if (this.Row.ContainsKey("AtPara") == true)
            {
                /*如果包含这个字段*/
                AtPara ap = this.atPara;
                foreach (string key in ap.HisHT.Keys)
                    if (DataType.IsNullOrEmpty(dr[key].ToString()) == true)
                        dr[key] = ap.HisHT[key];
            }

            dt.Rows.Add(dr);
            return dt;
        }
        #endregion

        #region 关于database 操作
        public int RunSQL(string sql)
        {
            Paras ps = new Paras();
            ps.SQL = sql;
            return this.RunSQL(ps);
        }
        /// <summary>
        /// 在此实体是运行sql 返回结果集合
        /// </summary>
        /// <param name="sql">要运行的sql</param>
        /// <returns>执行的结果</returns>
        public int RunSQL(Paras ps)
        {
            switch (this.EnMap.EnDBUrl.DBUrlType)
            {
                case DBUrlType.AppCenterDSN:
                    return DBAccess.RunSQL(ps);
                case DBUrlType.DBAccessOfMSSQL1:
                    return DBAccessOfMSSQL1.RunSQL(ps.SQL);
                case DBUrlType.DBAccessOfMSSQL2:
                    return DBAccessOfMSSQL2.RunSQL(ps.SQL);
                //case DBUrlType.DBAccessOfOracle1:
                //    return DBAccessOfOracle1.RunSQL(ps.SQL);
                //case DBUrlType.DBAccessOfOracle2:
                //    return DBAccessOfOracle2.RunSQL(ps.SQL);
                case DBUrlType.DBSrc:
                    return this.EnMap.EnDBUrl.HisDBSrc.RunSQL(ps.SQL, ps);
                default:
                    throw new Exception("@没有设置类型。");
            }
        }
        public int RunSQL(string sql, Paras paras)
        {
            switch (this.EnMap.EnDBUrl.DBUrlType)
            {
                case DBUrlType.AppCenterDSN:
                    return DBAccess.RunSQL(sql, paras);
                case DBUrlType.DBAccessOfMSSQL1:
                    return DBAccessOfMSSQL1.RunSQL(sql, paras);

                //case DBUrlType.DBAccessOfMSSQL2:
                //    return DBAccessOfMSSQL2.RunSQL(sql, paras);
                //case DBUrlType.DBAccessOfOracle1:
                //    return DBAccessOfOracle1.RunSQL(sql);
                //case DBUrlType.DBAccessOfOracle2:
                //    return DBAccessOfOracle2.RunSQL(sql);
                case DBUrlType.DBSrc:
                    return this.EnMap.EnDBUrl.HisDBSrc.RunSQL(sql, paras);
                default:
                    throw new Exception("@没有设置类型。");
            }
        }
        /// <summary>
        /// 在此实体是运行sql 返回结果集合
        /// </summary>
        /// <param name="sql">要运行的 select sql</param>
        /// <returns>执行的查询结果</returns>
        public DataTable RunSQLReturnTable(string sql, Paras paras = null)
        {
            switch (this.EnMap.EnDBUrl.DBUrlType)
            {
                case DBUrlType.AppCenterDSN:
                    return DBAccess.RunSQLReturnTable(sql, paras);
                case DBUrlType.DBAccessOfMSSQL1:
                    return DBAccessOfMSSQL1.RunSQLReturnTable(sql, paras);
                //case DBUrlType.DBAccessOfMSSQL2:
                //    return DBAccessOfMSSQL2.RunSQLReturnTable(sql);
                //case DBUrlType.DBAccessOfOracle1:
                //    return DBAccessOfOracle1.RunSQLReturnTable(sql);
                //case DBUrlType.DBAccessOfOracle2:
                //    return DBAccessOfOracle2.RunSQLReturnTable(sql);
                case DBUrlType.DBSrc:
                    return this.EnMap.EnDBUrl.HisDBSrc.RunSQLReturnTable(sql);
                default:
                    throw new Exception("@没有设置类型。");
            }
        }

        /// <summary>
        /// 查询SQL返回int
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public int RunSQLReturnValInt(string sql, Paras paras = null)
        {
            if (paras == null)
                paras = new Paras();

            paras.SQL = sql;
            switch (this.EnMap.EnDBUrl.DBUrlType)
            {
                case DBUrlType.AppCenterDSN:
                    return DBAccess.RunSQLReturnValInt(paras, 0);
                case DBUrlType.DBAccessOfMSSQL1:
                    return DBAccessOfMSSQL1.RunSQLReturnInt(paras);
                //case DBUrlType.DBAccessOfMSSQL2:
                //    return DBAccessOfMSSQL2.RunSQLReturnTable(sql);
                //case DBUrlType.DBAccessOfOracle1:
                //    return DBAccessOfOracle1.RunSQLReturnTable(sql);
                //case DBUrlType.DBAccessOfOracle2:
                //    return DBAccessOfOracle2.RunSQLReturnTable(sql);
                case DBUrlType.DBSrc:
                    return this.EnMap.EnDBUrl.HisDBSrc.RunSQLReturnValInt(sql);
                default:
                    throw new Exception("@没有设置类型。");
            }
        }

        #endregion

        #region 关于明细的操作
        public Entities GetEnsDaOfOneVSM(AttrOfOneVSM attr)
        {
            Entities ensOfMM = attr.EnsOfMM;
            Entities ensOfM = attr.EnsOfM;
            ensOfM.Clear();

            QueryObject qo = new QueryObject(ensOfMM);
            qo.AddWhere(attr.AttrOfOneInMM, this.PKVal.ToString());
            qo.DoQuery();

            foreach (Entity en in ensOfMM)
            {
                Entity enOfM = ensOfM.GetNewEntity;
                enOfM.PKVal = en.GetValStringByKey(attr.AttrOfMInMM);
                enOfM.Retrieve();
                ensOfM.AddEntity(enOfM);
            }
            return ensOfM;
        }
        /// <summary>
        /// 取得实体集合多对多的实体集合.
        /// </summary>
        /// <param name="ensOfMMclassName">实体集合的类名称</param>
        /// <returns>数据实体</returns>
        public Entities GetEnsDaOfOneVSM(string ensOfMMclassName)
        {
            AttrOfOneVSM attr = this.EnMap.GetAttrOfOneVSM(ensOfMMclassName);

            return GetEnsDaOfOneVSM(attr);
        }
        public Entities GetEnsDaOfOneVSMFirst()
        {
            AttrOfOneVSM attr = this.EnMap.AttrsOfOneVSM[0];
            //	throw new Exception("err "+attr.Desc); 
            return this.GetEnsDaOfOneVSM(attr);
        }
        #endregion

        #region 关于明细的操作
        /// <summary>
        /// 得到他的数据实体
        /// </summary>
        /// <param name="EnsName">类名称</param>
        /// <returns></returns>
        public Entities GetDtlEnsDa(string EnsName)
        {
            Entities ens = ClassFactory.GetEns(EnsName);
            return GetDtlEnsDa(ens);
            /*
            EnDtls eds =this.EnMap.Dtls; 
            foreach(EnDtl ed in eds)
            {
                if (ed.EnsName==EnsName)
                {
                    Entities ens =ClassFactory.GetEns(EnsName) ; 
                    QueryObject qo = new QueryObject(ClassFactory.GetEns(EnsName));
                    qo.AddWhere(ed.RefKey,this.PKVal.ToString());
                    qo.DoQuery();
                    return ens;
                }
            }
            throw new Exception("@实体["+this.EnDesc+"],不包含"+EnsName);	
            */
        }
        /// <summary>
        /// 取出他的数据实体
        /// </summary>
        /// <param name="ens">集合</param>
        /// <returns>执行后的实体信息</returns>
        public Entities GetDtlEnsDa(Entities ens)
        {
            foreach (EnDtl dtl in this.EnMap.Dtls)
            {
                if (dtl.Ens.GetType() == ens.GetType())
                {
                    QueryObject qo = new QueryObject(dtl.Ens);
                    qo.AddWhere(dtl.RefKey, this.PKVal.ToString());
                    qo.DoQuery();
                    return dtl.Ens;
                }
            }
            throw new Exception("@在取[" + this.EnDesc + "]的明细时出现错误。[" + ens.GetNewEntity.EnDesc + "],不在他的集合内。");
        }

        public Entities GetDtlEnsDa(EnDtl dtl, string pkval = null)
        {
            try
            {
                if (pkval == null)
                    pkval = this.PKVal.ToString();
                QueryObject qo = new QueryObject(dtl.Ens);
                MapDtl md = new MapDtl();
                md.No = dtl.Ens.GetNewEntity.ClassID;
                if (md.RetrieveFromDBSources() == 0)
                {
                    qo.AddWhere(dtl.RefKey, pkval);
                    qo.DoQuery();
                    return dtl.Ens;
                }

                //如果是freefrm 就考虑他的权限控制问题. 
                switch (md.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:  // 按人员来控制.
                        qo.AddWhere(GEDtlAttr.RefPK, pkval);
                        qo.addAnd();
                        qo.AddWhere(GEDtlAttr.Rec, BP.Web.WebUser.No);
                        break;
                    case DtlOpenType.ForWorkID: // 按工作ID来控制
                        qo.AddWhere(GEDtlAttr.RefPK, pkval);
                        break;
                    case DtlOpenType.ForFID: // 按流程ID来控制.这里不允许修改，如需修改则加新case.
                        //if (nd == null)
                        //    throw new Exception("@当前您是配置的权限是FID,但是当前没有节点ID.");

                        //if (nd.HisNodeWorkType == BP.WF.NodeWorkType.SubThreadWork)
                        //    qo.AddWhere(GEDtlAttr.RefPK, this.FID); //edit by zhoupeng 2016.04.23
                        //else
                        qo.AddWhere(GEDtlAttr.FID, pkval);
                        break;
                }

                if (md.FilterSQLExp != "")
                {
                    string[] strs = md.FilterSQLExp.Split('=');
                    qo.addAnd();
                    qo.AddWhere(strs[0], strs[1]);
                }

                qo.DoQuery();
                return dtl.Ens;
            }
            catch (Exception)
            {
                throw new Exception("@在取[" + this.EnDesc + "]的明细时出现错误。[" + dtl.Desc + "],不在他的集合内。");
            }
        }

        //		/// <summary>
        //		/// 返回第一个实体
        //		/// </summary>
        //		/// <returns>返回第一个实体,如果没有就抛出异常</returns>
        //		public Entities GetDtl()
        //		{
        //			 return this.GetDtls(0);
        //		}
        //		/// <summary>
        //		/// 返回第一个实体
        //		/// </summary>
        //		/// <returns>返回第一个实体</returns>
        //		public Entities GetDtl(int index)
        //		{
        //			try
        //			{
        //				return this.GetDtls(this.EnMap.Dtls[index].Ens);
        //			}
        //			catch( Exception ex)
        //			{
        //				throw new Exception("@在取得按照顺序取["+this.EnDesc+"]的明细,出现错误:"+ex.Message);
        //			}			 
        //		}
        /// <summary>
        /// 取出他的明细集合。
        /// </summary>
        /// <returns></returns>
        public System.Collections.ArrayList GetDtlsDatasOfArrayList()
        {
            ArrayList al = new ArrayList();
            foreach (EnDtl dtl in this.EnMap.Dtls)
            {
                al.Add(this.GetDtlEnsDa(dtl.Ens));
            }
            return al;
        }

        public List<Entities> GetDtlsDatasOfList(string pkval = null)
        {
            List<Entities> al = new List<Entities>();
            foreach (EnDtl dtl in this.EnMap.Dtls)
            {
                al.Add(this.GetDtlEnsDa(dtl, pkval));
            }
            return al;
        }
        #endregion

        #region 检查一个属性值是否存在于实体集合中
        /// <summary>
        /// 检查一个属性值是否存在于实体集合中
        /// 这个方法经常用到在beforeinsert中。
        /// </summary>
        /// <param name="key">要检查的key.</param>
        /// <param name="val">要检查的key.对应的val</param>
        /// <returns></returns>
        protected int ExitsValueNum(string key, string val)
        {
            string field = this.EnMap.GetFieldByKey(key);
            Paras ps = new Paras();
            ps.Add("p", val);

            string sql = "SELECT COUNT( " + key + " ) FROM " + this.EnMap.PhysicsTable + " WHERE " + key + "=" + this.HisDBVarStr + "p";
            return int.Parse(DBAccess.RunSQLReturnVal(sql, ps).ToString());
        }
        #endregion

        #region 于编号有关系的处理。
        /// <summary>
        /// 这个方法是为不分级字典，生成一个编号。根据制订的 属性.
        /// </summary>
        /// <param name="attrKey">属性</param>
        /// <returns>产生的序号</returns> 
        public string GenerNewNoByKey(string attrKey, Attr attr = null)
        {
            try
            {
                string sql = null;
                if (attr == null)
                    attr = this.EnMap.GetAttrByKey(attrKey);
                //    if (attr.UIIsReadonly == false)
                //      throw new Exception("@需要自动生成编号的列(" + attr.Key + ")必须为只读。");

                string field = this.EnMap.GetFieldByKey(attrKey);
                switch (this.EnMap.EnDBUrl.DBType)
                {
                    case DBType.MSSQL:
                        sql = "SELECT CONVERT(INT, MAX(CAST(" + field + " as int)) )+1 AS No FROM " + this._enMap.PhysicsTable;
                        break;
                    case DBType.PostgreSQL:
                        sql = "SELECT to_number( MAX(" + field + ") ,'99999999')+1   FROM " + this._enMap.PhysicsTable;
                        break;
                    case DBType.Oracle:
                    case DBType.DM:
                        sql = "SELECT MAX(" + field + ") +1 AS No FROM " + this._enMap.PhysicsTable;
                        break;
                    case DBType.MySQL:
                        sql = "SELECT CONVERT(MAX(CAST(" + field + " AS SIGNED INTEGER)),SIGNED) +1 AS No FROM " + this._enMap.PhysicsTable;
                        break;
                    case DBType.Informix:
                        sql = "SELECT MAX(" + field + ") +1 AS No FROM " + this._enMap.PhysicsTable;
                        break;
                    case DBType.Access:
                        sql = "SELECT MAX( [" + field + "]) +1 AS  No FROM " + this._enMap.PhysicsTable;
                        break;
                    default:
                        throw new Exception("error");
                }
                string str = DBAccess.RunSQLReturnValInt(sql, 1).ToString();
                if (str == "0" || str == "")
                    str = "1";
                return str.PadLeft(int.Parse(this._enMap.CodeStruct), '0');
            }
            catch (Exception ex)
            {
                this.CheckPhysicsTable();
                throw ex;
            }
        }
        /// <summary>
        /// 按照一列产生顺序号码。
        /// </summary>
        /// <param name="attrKey">要产生的列</param>
        /// <param name="attrGroupKey">分组的列名</param>
        /// <param name="FKVal">分组的主键</param>
        /// <returns></returns>		
        public string GenerNewNoByKey(int nolength, string attrKey, string attrGroupKey, string attrGroupVal)
        {
            if (attrGroupKey == null || attrGroupVal == null)
                throw new Exception("@分组字段attrGroupKey attrGroupVal 不能为空");

            Paras ps = new Paras();
            ps.Add("groupKey", attrGroupKey);
            ps.Add("groupVal", attrGroupVal);

            string sql = "";
            string field = this.EnMap.GetFieldByKey(attrKey);
            ps.Add("f", attrKey);

            switch (this.EnMap.EnDBUrl.DBType)
            {
                case DBType.MSSQL:
                    sql = "SELECT CONVERT(bigint, MAX([" + field + "]))+1 AS Num FROM " + this.EnMap.PhysicsTable + " WHERE " + attrGroupKey + "='" + attrGroupVal + "'";
                    break;
                case DBType.Oracle:
                case DBType.DM:
                case DBType.Informix:
                    sql = "SELECT MAX( :f )+1 AS No FROM " + this.EnMap.PhysicsTable + " WHERE " + this.HisDBVarStr + "groupKey=" + this.HisDBVarStr + "groupVal ";
                    break;
                case DBType.MySQL:
                    sql = "SELECT MAX(" + field + ") +1 AS Num FROM " + this.EnMap.PhysicsTable + " WHERE " + attrGroupKey + "='" + attrGroupVal + "'";
                    break;
                case DBType.Access:
                    sql = "SELECT MAX([" + field + "]) +1 AS Num FROM " + this.EnMap.PhysicsTable + " WHERE " + attrGroupKey + "='" + attrGroupVal + "'";
                    break;
                default:
                    throw new Exception("error");
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql, ps);
            string str = "1";
            if (dt.Rows.Count != 0)
            {
                //System.DBNull n = new DBNull();
                if (dt.Rows[0][0] is DBNull)
                    str = "1";
                else
                    str = dt.Rows[0][0].ToString();
            }
            return str.PadLeft(nolength, '0');
        }
        public string GenerNewNoByKey(string attrKey, string attrGroupKey, string attrGroupVal)
        {
            return this.GenerNewNoByKey(int.Parse(this.EnMap.CodeStruct), attrKey, attrGroupKey, attrGroupVal);
        }
        /// <summary>
        /// 按照两列查生顺序号码。
        /// </summary>
        /// <param name="attrKey"></param>
        /// <param name="attrGroupKey1"></param>
        /// <param name="attrGroupKey2"></param>
        /// <param name="attrGroupVal1"></param>
        /// <param name="attrGroupVal2"></param>
        /// <returns></returns>
        public string GenerNewNoByKey(string attrKey, string attrGroupKey1, string attrGroupKey2, object attrGroupVal1, object attrGroupVal2)
        {
            string f = this.EnMap.GetFieldByKey(attrKey);
            Paras ps = new Paras();
            //   ps.Add("f", f);

            string sql = "";
            switch (this.EnMap.EnDBUrl.DBType)
            {
                case DBType.Oracle:
                case DBType.DM:
                case DBType.Informix:
                    sql = "SELECT   MAX(" + f + ") +1 AS No FROM " + this.EnMap.PhysicsTable;
                    break;
                case DBType.MSSQL:
                    sql = "SELECT CONVERT(INT, MAX(" + this.EnMap.GetFieldByKey(attrKey) + ") )+1 AS No FROM " + this.EnMap.PhysicsTable + " WHERE " + this.EnMap.GetFieldByKey(attrGroupKey1) + "='" + attrGroupVal1 + "' AND " + this.EnMap.GetFieldByKey(attrGroupKey2) + "='" + attrGroupVal2 + "'";
                    break;
                case DBType.Access:
                    sql = "SELECT CONVERT(INT, MAX(" + this.EnMap.GetFieldByKey(attrKey) + ") )+1 AS No FROM " + this.EnMap.PhysicsTable + " WHERE " + this.EnMap.GetFieldByKey(attrGroupKey1) + "='" + attrGroupVal1 + "' AND " + this.EnMap.GetFieldByKey(attrGroupKey2) + "='" + attrGroupVal2 + "'";
                    break;
                default:
                    break;
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql, ps);
            string str = "1";
            if (dt.Rows.Count != 0)
                str = dt.Rows[0][0].ToString();
            return str.PadLeft(int.Parse(this.EnMap.CodeStruct), '0');
        }
        /// <summary>
        /// 按照两列查生顺序号码。
        /// </summary>
        /// <param name="attrKey">字段</param>
        /// <param name="attrGroupKey1"></param>
        /// <param name="attrGroupKey2"></param>
        /// <param name="attrGroupKey3"></param>
        /// <param name="attrGroupVal1"></param>
        /// <param name="attrGroupVal2"></param>
        /// <param name="attrGroupVal3"></param>
        /// <returns></returns>
        public string GenerNewNoByKey(string attrKey, string attrGroupKey1, string attrGroupKey2, string attrGroupKey3, object attrGroupVal1, object attrGroupVal2, object attrGroupVal3)
        {
            string f = this.EnMap.GetFieldByKey(attrKey);
            Paras ps = new Paras();
            //   ps.Add("f", f);

            string sql = "";
            switch (this.EnMap.EnDBUrl.DBType)
            {
                case DBType.Oracle:
                case DBType.DM:
                case DBType.Informix:
                    sql = "SELECT   MAX(" + f + ") +1 AS No FROM " + this.EnMap.PhysicsTable;
                    break;
                case DBType.MSSQL:
                    sql = "SELECT CONVERT(INT, MAX(" + this.EnMap.GetFieldByKey(attrKey) + ") )+1 AS No FROM " + this.EnMap.PhysicsTable + " WHERE " + this.EnMap.GetFieldByKey(attrGroupKey1) + "='" + attrGroupVal1 + "' AND " + this.EnMap.GetFieldByKey(attrGroupKey2) + "='" + attrGroupVal2 + "'  AND " + this.EnMap.GetFieldByKey(attrGroupKey3) + "='" + attrGroupVal3 + "'";
                    break;
                case DBType.Access:
                    sql = "SELECT CONVERT(INT, MAX(" + this.EnMap.GetFieldByKey(attrKey) + ") )+1 AS No FROM " + this.EnMap.PhysicsTable + " WHERE " + this.EnMap.GetFieldByKey(attrGroupKey1) + "='" + attrGroupVal1 + "' AND " + this.EnMap.GetFieldByKey(attrGroupKey2) + "='" + attrGroupVal2 + "'  AND " + this.EnMap.GetFieldByKey(attrGroupKey3) + "='" + attrGroupVal3 + "'";
                    break;
                default:
                    break;
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql, ps);
            string str = "1";
            if (dt.Rows.Count != 0)
                str = dt.Rows[0][0].ToString();
            return str.PadLeft(int.Parse(this.EnMap.CodeStruct), '0');
        }
        #endregion

        #region 构造方法
        public Entity()
        {
        }
        #endregion

        #region 排序操作
        protected void DoOrderUp(string idxAttr)
        {
            //  string pkval = this.PKVal as string;
            string pkval = this.PKVal.ToString();
            string pk = this.PK;
            string table = this.EnMap.PhysicsTable;

            string sql = "SELECT " + pk + "," + idxAttr + " FROM " + table + "  ORDER BY " + idxAttr;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int idx = 0;
            string beforeNo = "";
            string myNo = "";
            bool isMeet = false;

            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                myNo = dr[pk].ToString();
                if (myNo == pkval)
                    isMeet = true;

                if (isMeet == false)
                    beforeNo = myNo;
                DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idx + " WHERE " + pk + "='" + myNo + "'");
            }
            DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idxAttr + "+1 WHERE " + pk + "='" + beforeNo + "'");
            DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idxAttr + "-1 WHERE " + pk + "='" + pkval + "'");
        }
        protected void DoOrderUp(string groupKeyAttr, string groupKeyVal, string idxAttr)
        {
            //  string pkval = this.PKVal as string;
            string pkval = this.PKVal.ToString();
            string pk = this.PK;
            string table = this.EnMap.PhysicsTable;

            string sql = "SELECT " + pk + "," + idxAttr + " FROM " + table + " WHERE " + groupKeyAttr + "='" + groupKeyVal + "' ORDER BY " + idxAttr;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int idx = 0;
            string beforeNo = "";
            string myNo = "";
            bool isMeet = false;

            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                myNo = dr[pk].ToString();
                if (myNo == pkval)
                    isMeet = true;

                if (isMeet == false)
                    beforeNo = myNo;
                DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idx + " WHERE " + pk + "='" + myNo + "'");
            }
            DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idxAttr + "+1 WHERE " + pk + "='" + beforeNo + "'");
            DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idxAttr + "-1 WHERE " + pk + "='" + pkval + "'");
        }
        protected void DoOrderUp(string groupKeyAttr, object gVal1, string gKey2, object gVal2, string idxAttr)
        {
            //  string pkval = this.PKVal as string;
            string pkval = this.PKVal.ToString();
            string pk = this.PK;
            string table = this.EnMap.PhysicsTable;

            string sql = "SELECT " + pk + "," + idxAttr + " FROM " + table + " WHERE (" + groupKeyAttr + "='" + gVal1 + "' AND " + gKey2 + "='" + gVal2 + "') ORDER BY " + idxAttr;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int idx = 0;
            string beforeNo = "";
            string myNo = "";
            bool isMeet = false;

            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                myNo = dr[pk].ToString();

                if (myNo.Equals(pkval) == true)
                    isMeet = true;

                if (isMeet == false)
                    beforeNo = myNo;

                DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idx + " WHERE " + pk + "='" + myNo + "'");
            }
            DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idxAttr + "+1 WHERE " + pk + "='" + beforeNo + "'");
            DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idxAttr + "-1 WHERE " + pk + "='" + pkval + "'");
        }
        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="groupKeyAttr"></param>
        /// <param name="gVal1"></param>
        /// <param name="gKey2"></param>
        /// <param name="gVal2"></param>
        /// <param name="gKey3"></param>
        /// <param name="gVal3"></param>
        /// <param name="idxAttr"></param>
        protected void DoOrderUp(string groupKeyAttr, object gVal1, string gKey2, object gVal2,
            string gKey3, object gVal3, string idxAttr)
        {
            //  string pkval = this.PKVal as string;
            string pkval = this.PKVal.ToString();
            string pk = this.PK;
            string table = this.EnMap.PhysicsTable;

            string sql = "SELECT " + pk + "," + idxAttr + " FROM " + table + " WHERE (" + groupKeyAttr + "='" + gVal1 + "' AND " + gKey2 + "='" + gVal2 + "' AND " + gKey3 + "='" + gVal3 + "') ORDER BY " + idxAttr;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int idx = 0;
            string beforeNo = "";
            string myNo = "";
            bool isMeet = false;

            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                myNo = dr[pk].ToString();

                if (myNo.Equals(pkval) == true)
                    isMeet = true;

                if (isMeet == false)
                    beforeNo = myNo;

                DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idx + " WHERE " + pk + "='" + myNo + "'");
            }
            DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idxAttr + "+1 WHERE " + pk + "='" + beforeNo + "'");
            DBAccess.RunSQL("UPDATE " + table + " SET " + idxAttr + "=" + idxAttr + "-1 WHERE " + pk + "='" + pkval + "'");
        }
        /// <summary>
        /// 插队
        /// </summary>
        /// <param name="idxAttr">Idx列</param>
        /// <param name="entityPKVal">要插入的指定实体主键值</param>
        /// <param name="groupKey">列名</param>
        /// <param name="groupVal">列值</param>
        protected void DoOrderInsertTo(string idxAttr, object entityPKVal, string groupKey)
        {
            string ptable = this.EnMap.PhysicsTable; // Sys_MapAttr
            string pk = this.PK; //MyPK
            int idx = this.GetValIntByKey(idxAttr); // 当前实体的idx. 10 
                                                    //   string groupVal = this.GetValStringByKey(groupKey); //分组的val.   101

            //求出来要被插队的 idx.
            string sql = "SELECT " + idxAttr + "," + groupKey + " FROM " + ptable + " WHERE " + pk + "='" + entityPKVal + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            int idxFirst = int.Parse(dt.Rows[0][0].ToString());
            string groupValFirst = dt.Rows[0][1].ToString();

            sql = "UPDATE " + ptable + " SET " + idxAttr + "=" + idxFirst + "-1, " + groupKey + "='" + groupValFirst + "' WHERE " + this.PK + "='" + this.PKVal + "'";
            DBAccess.RunSQL(sql);
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="idxAttr"></param>
        protected void DoOrderDown(string idxAttr)
        {
            string pkval = this.PKVal.ToString();
            string pk = this.PK;
            string table = this.EnMap.PhysicsTable;

            string sql = "SELECT " + pk + " ," + idxAttr + " FROM " + table + "  order by " + idxAttr;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int idx = 0;
            string nextNo = "";
            string myNo = "";
            bool isMeet = false;

            string sqls = "";
            foreach (DataRow dr in dt.Rows)
            {
                myNo = dr[pk].ToString();
                if (isMeet == true)
                {
                    nextNo = myNo;
                    isMeet = false;
                }
                idx++;

                if (myNo == pkval)
                    isMeet = true;

                sqls += "@ UPDATE " + table + " SET " + idxAttr + "=" + idx + " WHERE " + pk + "='" + myNo + "'";
            }

            sqls += "@ UPDATE  " + table + " SET " + idxAttr + "=" + idxAttr + "-1 WHERE " + pk + "='" + nextNo + "'";
            sqls += "@ UPDATE  " + table + " SET " + idxAttr + "=" + idxAttr + "+1 WHERE " + pk + "='" + pkval + "'";

            DBAccess.RunSQLs(sqls);
        }
        protected void DoOrderDown(string groupKeyAttr, string groupKeyVal, string idxAttr)
        {
            string pkval = this.PKVal.ToString();
            string pk = this.PK;
            string table = this.EnMap.PhysicsTable;

            string sql = "SELECT " + pk + " ," + idxAttr + " FROM " + table + " WHERE " + groupKeyAttr + "='" + groupKeyVal + "' order by " + idxAttr;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int idx = 0;
            string nextNo = "";
            string myNo = "";
            bool isMeet = false;

            string sqls = "";
            foreach (DataRow dr in dt.Rows)
            {
                myNo = dr[pk].ToString();
                if (isMeet == true)
                {
                    nextNo = myNo;
                    isMeet = false;
                }
                idx++;

                if (myNo == pkval)
                    isMeet = true;

                sqls += "@ UPDATE " + table + " SET " + idxAttr + "=" + idx + " WHERE " + pk + "='" + myNo + "'";
            }

            sqls += "@ UPDATE  " + table + " SET " + idxAttr + "=" + idxAttr + "-1 WHERE " + pk + "='" + nextNo + "'";
            sqls += "@ UPDATE  " + table + " SET " + idxAttr + "=" + idxAttr + "+1 WHERE " + pk + "='" + pkval + "'";

            DBAccess.RunSQLs(sqls);
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="groupKeyAttr">分组字段1</param>
        /// <param name="val1">值1</param>
        /// <param name="gKeyAttr2">字段2</param>
        /// <param name="gKeyVal2">值2</param>
        /// <param name="idxAttr">排序字段</param>
        protected void DoOrderDown(string groupKeyAttr, object val1, string gKeyAttr2,
            object gKeyVal2, string idxAttr)
        {
            string pkval = this.PKVal.ToString();
            string pk = this.PK;
            string table = this.EnMap.PhysicsTable;

            string sql = "SELECT " + pk + " ," + idxAttr + " FROM " + table + " WHERE (" + groupKeyAttr + "='" + val1 + "' AND " + gKeyAttr2 + "='" + gKeyVal2 + "' ) order by " + idxAttr;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int idx = 0;
            string nextNo = "";
            string myNo = "";
            bool isMeet = false;

            string sqls = "";
            foreach (DataRow dr in dt.Rows)
            {
                myNo = dr[pk].ToString();
                if (isMeet == true)
                {
                    nextNo = myNo;
                    isMeet = false;
                }
                idx++;

                if (myNo == pkval)
                    isMeet = true;

                sqls += "@UPDATE " + table + " SET " + idxAttr + "=" + idx + " WHERE " + pk + "='" + myNo + "' AND  (" + groupKeyAttr + "='" + val1 + "' AND " + gKeyAttr2 + "='" + gKeyVal2 + "' ) ";
            }

            sqls += "@ UPDATE  " + table + " SET " + idxAttr + "=" + idxAttr + "-1 WHERE " + pk + "='" + nextNo + "' AND (" + groupKeyAttr + "='" + val1 + "' AND " + gKeyAttr2 + "='" + gKeyVal2 + "' )";
            sqls += "@ UPDATE  " + table + " SET " + idxAttr + "=" + idxAttr + "+1 WHERE " + pk + "='" + pkval + "' AND (" + groupKeyAttr + "='" + val1 + "' AND " + gKeyAttr2 + "='" + gKeyVal2 + "' )";

            DBAccess.RunSQLs(sqls);
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="groupKeyAttr">字段1</param>
        /// <param name="val1">值1</param>
        /// <param name="gKeyAttr2">字段2</param>
        /// <param name="gKeyVal2">值2</param>
        /// <param name="gKeyAttr3">字段3</param>
        /// <param name="gKeyVal3">值3</param>
        /// <param name="idxAttr">排序字段</param>
        protected void DoOrderDown(string groupKeyAttr, object val1, string gKeyAttr2,
            object gKeyVal2, string gKeyAttr3,
            object gKeyVal3, string idxAttr)
        {
            string pkval = this.PKVal.ToString();
            string pk = this.PK;
            string table = this.EnMap.PhysicsTable;

            string sql = "SELECT " + pk + " ," + idxAttr + " FROM " + table + " WHERE (" + groupKeyAttr + "='" + val1 + "' AND " + gKeyAttr2 + "='" + gKeyVal2 + "' AND " + gKeyAttr3 + "='" + gKeyVal3 + "' ) order by " + idxAttr;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int idx = 0;
            string nextNo = "";
            string myNo = "";
            bool isMeet = false;

            string sqls = "";
            foreach (DataRow dr in dt.Rows)
            {
                myNo = dr[pk].ToString();
                if (isMeet == true)
                {
                    nextNo = myNo;
                    isMeet = false;
                }
                idx++;

                if (myNo == pkval)
                    isMeet = true;

                sqls += "@UPDATE " + table + " SET " + idxAttr + "=" + idx + " WHERE " + pk + "='" + myNo + "' AND  (" + groupKeyAttr + "='" + val1 + "' AND " + gKeyAttr2 + "='" + gKeyVal2 + "' ) ";
            }

            sqls += "@ UPDATE  " + table + " SET " + idxAttr + "=" + idxAttr + "-1 WHERE " + pk + "='" + nextNo + "' AND (" + groupKeyAttr + "='" + val1 + "' AND " + gKeyAttr2 + "='" + gKeyVal2 + "'  AND " + gKeyAttr3 + "='" + gKeyVal3 + "' )";
            sqls += "@ UPDATE  " + table + " SET " + idxAttr + "=" + idxAttr + "+1 WHERE " + pk + "='" + pkval + "' AND (" + groupKeyAttr + "='" + val1 + "' AND " + gKeyAttr2 + "='" + gKeyVal2 + "'  AND " + gKeyAttr3 + "='" + gKeyVal3 + "' )";

            DBAccess.RunSQLs(sqls);
        }
        #endregion 排序操作

        #region 直接操作
        /// <summary>
        /// 直接更新
        /// </summary>
        public int DirectUpdate()
        {
            try
            {
                return EntityDBAccess.Update(this, null);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("列名") || ex.Message.Contains("将截断字符串") || ex.Message.Contains("缺少") || ex.Message.Contains("的值太大"))
                {
                    /*说明字符串长度有问题.*/
                    this.CheckPhysicsTable();

                    //执行字段扩充检查.
                    bool isCheck = CheckPhysicsTableAutoExtFieldLength(ex);
                    if (isCheck == true)
                        return this.DirectUpdate();
                }
                throw ex;
            }
        }
        /// <summary>
        /// 直接的Insert
        /// </summary>
        public virtual int DirectInsert()
        {
            try
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        return this.RunSQL(this.SQLCash.Insert, SqlBuilder.GenerParas(this, null));
                    case DBType.Access:
                        return this.RunSQL(this.SQLCash.Insert, SqlBuilder.GenerParas(this, null));
                        break;
                    case DBType.MySQL:
                    case DBType.Informix:
                    default:
                        return this.RunSQL(this.SQLCash.Insert.Replace("[", "").Replace("]", ""), SqlBuilder.GenerParas(this, null));
                }
            }
            catch (Exception ex)
            {

                this.CheckPhysicsTable();

                //执行字段扩充检查.
                bool isCheck = CheckPhysicsTableAutoExtFieldLength(ex);
                if (isCheck == true)
                    return this.Insert();

                this.roll();
                throw ex;
            }
            //this.RunSQL(this.SQLCash.Insert, SqlBuilder.GenerParas(this, null));
        }
        /// <summary>
        /// 直接的Delete
        /// </summary>
        public void DirectDelete()
        {
            EntityDBAccess.Delete(this);
        }
        public void DirectSave()
        {
            if (this.IsExits)
                this.DirectUpdate();
            else
                this.DirectInsert();
        }
        #endregion

        #region Retrieve
        /// <summary>
        /// 按照属性查询
        /// </summary>
        /// <param name="attr">属性名称</param>
        /// <param name="val">值</param>
        /// <returns>是否查询到</returns>
        public bool RetrieveByAttrAnd(string attr1, object val1, string attr2, object val2)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(attr1, val1);
            qo.addAnd();
            qo.AddWhere(attr2, val2);

            if (qo.DoQuery() >= 1)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 按照属性查询
        /// </summary>
        /// <param name="attr">属性名称</param>
        /// <param name="val">值</param>
        /// <returns>是否查询到</returns>
        public bool RetrieveByAttrOr(string attr1, object val1, string attr2, object val2)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(attr1, val1);
            qo.addOr();
            qo.AddWhere(attr2, val2);

            if (qo.DoQuery() == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 按照属性查询
        /// </summary>
        /// <param name="attr">属性名称</param>
        /// <param name="val">值</param>
        /// <returns>是否查询到</returns>
        public bool RetrieveByAttr(string attr, object val)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(attr, val);

            if (qo.DoQuery() > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 从DBSources直接查询
        /// </summary>
        /// <returns>查询的个数</returns>
        public virtual int RetrieveFromDBSources()
        {
            try
            {
                return EntityDBAccess.Retrieve(this, this.SQLCash.Select, SqlBuilder.GenerParasPK(this));
            }
            catch
            {
                this.CheckPhysicsTable();
                return EntityDBAccess.Retrieve(this, this.SQLCash.Select, SqlBuilder.GenerParasPK(this));
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public int Retrieve(string key, object val)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(key, val);
            return qo.DoQuery();
        }

        public int Retrieve(string key1, object val1, string key2, object val2)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(key1, val1);
            qo.addAnd();
            qo.AddWhere(key2, val2);
            return qo.DoQuery();
        }
        public int Retrieve(string key1, object val1, string key2, object val2, string key3, object val3)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(key1, val1);
            qo.addAnd();
            qo.AddWhere(key2, val2);
            qo.addAnd();
            qo.AddWhere(key3, val3);
            return qo.DoQuery();
        }
        /// <summary>
        /// 按主键查询，返回查询出来的个数。
        /// 如果查询出来的是多个实体，那把第一个实体给值。	 
        /// </summary>
        /// <returns>查询出来的个数</returns>
        public virtual int Retrieve()
        {
            /*如果是没有放入缓存的实体. @wangyanyan */
            if (this.EnMap.DepositaryOfEntity == Depositary.Application)
            {
                var row = Cash2019.GetRow(this.ToString(), this.PKVal.ToString());
                if (row != null && row.Count > 2)
                {
                    this.Row = row;
                    return 1;
                }
            }

            try
            {
                int num = EntityDBAccess.Retrieve(this, this.SQLCash.Select,
                    SqlBuilder.GenerParasPK(this));
                if (num >= 1)
                {
                    //放入缓存.
                    if (this.EnMap.DepositaryOfEntity == Depositary.Application)
                        Cash2019.PutRow(this.ToString(), this.PKVal.ToString(), this.Row);
                    return num;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("does not exist")
                    || ex.Message.Contains("不存在")
                    || ex.Message.Contains("无效")
                    || ex.Message.Contains("field list"))
                {
                    this.CheckPhysicsTable();

                    if (this.EnMap.EnDBUrl.DBUrlType == DBUrlType.AppCenterDSN
                        && DBAccess.IsView(this.EnMap.PhysicsTable, SystemConfig.AppCenterDBType) == false)
                        return Retrieve(); //让其在查询一遍.
                }
                throw new Exception(ex.Message + "@在Entity(" + this.ToString() + ")查询期间出现错误@" + ex.StackTrace);
            }

            string msg = "";
            switch (this.PK)
            {
                case "OID":
                    msg += "[ 主键=OID 值=" + this.GetValStrByKey("OID") + " ]";
                    break;
                case "No":
                    msg += "[ 主键=No 值=" + this.GetValStrByKey("No") + " ]";
                    break;
                case "MyPK":
                    msg += "[ 主键=MyPK 值=" + this.GetValStrByKey("MyPK") + " ]";
                    break;
                case "NodeID":
                    msg += "[ 主键=NodeID 值=" + this.GetValStrByKey("NodeID") + " ]";
                    break;
                case "WorkID":
                    msg += "[ 主键=WorkID 值=" + this.GetValStrByKey("WorkID") + " ]";
                    break;
                default:
                    Hashtable ht = this.PKVals;
                    foreach (string key in ht.Keys)
                        msg += "[ 主键=" + key + " 值=" + ht[key] + " ]";
                    break;
            }
            Log.DefaultLogWriteLine(LogType.Error, "@没有[" + this.EnMap.EnDesc + "  " + this.EnMap.PhysicsTable + ", 类[" + this.ToString() + "], 物理表[" + this.EnMap.PhysicsTable + "] 实例。PK = " + this.GetValByKey(this.PK));
            throw new Exception("@记录[" + this.EnMap.EnDesc + "  " + this.EnMap.PhysicsTable + ", " + msg + "不存在.");
        }
        /// <summary>
        /// 判断是不是存在的方法.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsExits
        {
            get
            {
                try
                {
                    if (this.PKField.Contains(","))
                    {
                        Attrs attrs = this.EnMap.Attrs;

                        /*说明多个主键*/
                        QueryObject qo = new QueryObject(this);
                        string[] pks = this.PKField.Split(',');

                        bool isNeedAddAnd = false;
                        foreach (string pk in pks)
                        {
                            if (DataType.IsNullOrEmpty(pk))
                                continue;

                            if (isNeedAddAnd == true)
                            {
                                qo.addAnd();
                            }
                            else
                            {
                                isNeedAddAnd = true;
                            }

                            Attr attr = attrs.GetAttrByKey(pk);
                            switch (attr.MyDataType)
                            {
                                case DataType.AppBoolean:
                                case DataType.AppInt:
                                    qo.AddWhere(pk, this.GetValIntByKey(attr.Key));
                                    break;
                                case DataType.AppDouble:
                                case DataType.AppMoney:
                                    qo.AddWhere(pk, this.GetValDecimalByKey(attr.Key));
                                    break;
                                default:
                                    qo.AddWhere(pk, this.GetValStringByKey(attr.Key));
                                    break;
                            }

                        }

                        if (qo.DoQueryToTable().Rows.Count == 0)
                            return false;

                        return true;
                    }

                    object obj = this.PKVal;
                    if (obj == null || obj.ToString() == "")
                        return false;

                    if (this.IsOIDEntity)
                        if (obj.ToString() == "0")
                            return false;

                    // 生成数据库判断语句。
                    string selectSQL = "SELECT " + this.PKField + " FROM " + this.EnMap.PhysicsTable + " WHERE ";
                    switch (this.EnMap.EnDBUrl.DBType)
                    {
                        case DBType.MSSQL:
                            selectSQL += SqlBuilder.GetKeyConditionOfMS(this);
                            break;
                        case DBType.Oracle:
                        case DBType.DM:
                        case DBType.PostgreSQL:
                            selectSQL += SqlBuilder.GetKeyConditionOfOraForPara(this);
                            break;
                        case DBType.Informix:
                            selectSQL += SqlBuilder.GetKeyConditionOfInformixForPara(this);
                            break;
                        case DBType.MySQL:
                            selectSQL += SqlBuilder.GetKeyConditionOfMS(this);
                            break;
                        case DBType.Access:
                            selectSQL += SqlBuilder.GetKeyConditionOfOLE(this);
                            break;
                        default:
                            throw new Exception("@没有设计到。" + this.EnMap.EnDBUrl.DBUrlType);
                    }

                    // 从数据库里面查询，判断有没有。
                    switch (this.EnMap.EnDBUrl.DBUrlType)
                    {
                        case DBUrlType.AppCenterDSN:
                            return DBAccess.IsExits(selectSQL, SqlBuilder.GenerParasPK(this));
                        //case DBUrlType.DBAccessOfMSSQL1:
                        //    return DBAccessOfMSSQL1.IsExits(selectSQL);
                        //case DBUrlType.DBAccessOfMSSQL2:
                        //    return DBAccessOfMSSQL2.IsExits(selectSQL);
                        //case DBUrlType.DBAccessOfOracle1:
                        //    return DBAccessOfOracle1.IsExits(selectSQL);
                        //case DBUrlType.DBAccessOfOracle2:
                        //    return DBAccessOfOracle2.IsExits(selectSQL);
                        default:
                            throw new Exception("@没有设计到的DBUrl。" + this.EnMap.EnDBUrl.DBUrlType);
                    }

                }
                catch (Exception ex)
                {
                    this.CheckPhysicsTable();
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 按照主键查询，查询出来的结果不赋给当前的实体。
        /// </summary>
        /// <returns>查询出来的个数</returns>
        public DataTable RetrieveNotSetValues()
        {
            return this.RunSQLReturnTable(SqlBuilder.Retrieve(this));
        }
        /// <summary>
        /// 这个表里是否存在
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool IsExit(string pk, object val)
        {
            if (pk == "OID")
            {
                if (int.Parse(val.ToString()) == 0)
                    return false;
                else
                    return true;
            }

            QueryObject qo = new QueryObject(this);
            qo.AddWhere(pk, val);
            if (qo.DoQuery() == 0)
                return false;
            else
                return true;
        }
        public bool IsExit(string pk1, object val1, string pk2, object val2)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(pk1, val1);
            qo.addAnd();
            qo.AddWhere(pk2, val2);

            if (qo.DoQuery() == 0)
                return false;
            else
                return true;
        }

        public bool IsExit(string pk1, object val1, string pk2, object val2, string pk3, object val3)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(pk1, val1);
            qo.addAnd();
            qo.AddWhere(pk2, val2);
            qo.addAnd();
            qo.AddWhere(pk3, val3);

            if (qo.DoQuery() == 0)
                return false;
            else
                return true;
        }
        #endregion

        #region 删除.
        private bool CheckDB()
        {
            #region 检查数据.
            //CheckDatas  ens=new CheckDatas(this.EnMap.PhysicsTable);
            //foreach(CheckData en in ens)
            //{
            //    string sql="DELETE  FROM "+en.RefTBName+"   WHERE  "+en.RefTBFK+" ='"+this.GetValByKey(en.MainTBPK) +"' ";	
            //    DBAccess.RunSQL(sql);
            //}
            #endregion

            #region 判断是否有明细
            foreach (EnDtl dtl in this.EnMap.Dtls)
            {
                string sql = "DELETE  FROM  " + dtl.Ens.GetNewEntity.EnMap.PhysicsTable + "   WHERE  " + dtl.RefKey + " ='" + this.PKVal.ToString() + "' ";
                //DBAccess.RunSQL(sql);
                /*
                //string sql="SELECT "+dtl.RefKey+" FROM  "+dtl.Ens.GetNewEntity.EnMap.PhysicsTable+"   WHERE  "+dtl.RefKey+" ='"+this.PKVal.ToString() +"' ";	
                DataTable dt= DBAccess.RunSQLReturnTable(sql); 
                if(dt.Rows.Count==0)
                    continue;
                else
                    throw new Exception("@["+this.EnDesc+"],删除期间出现错误，它有["+dt.Rows.Count+"]个明细存在,不能删除！");
                    */
            }
            #endregion

            return true;
        }
        /// <summary>
        /// 删除之前要做的工作
        /// </summary>
        /// <returns></returns>
        protected virtual bool beforeDelete()
        {
            this.CheckDB();
            return true;
        }
        /// <summary>
        /// 删除它关连的实体．
        /// </summary>
        public void DeleteHisRefEns()
        {
            #region 检查数据.
            //			CheckDatas  ens=new CheckDatas(this.EnMap.PhysicsTable);
            //			foreach(CheckData en in ens)
            //			{
            //				string sql="DELETE  FROM "+en.RefTBName+"   WHERE  "+en.RefTBFK+" ='"+this.GetValByKey(en.MainTBPK) +"' ";	
            //				DBAccess.RunSQL(sql); 
            //			}
            #endregion

            #region 判断是否有明细
            foreach (EnDtl dtl in this.EnMap.Dtls)
            {
                string sql = "DELETE FROM " + dtl.Ens.GetNewEntity.EnMap.PhysicsTable + "   WHERE  " + dtl.RefKey + " ='" + this.PKVal.ToString() + "' ";
                DBAccess.RunSQL(sql);
            }
            #endregion

            #region 判断是否有一对对的关系.
            foreach (AttrOfOneVSM dtl in this.EnMap.AttrsOfOneVSM)
            {
                string sql = "DELETE  FROM " + dtl.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + "   WHERE  " + dtl.AttrOfOneInMM + " ='" + this.PKVal.ToString() + "' ";
                DBAccess.RunSQL(sql);
            }
            #endregion
        }
        /// <summary>
        /// 把缓存删除
        /// </summary>
        public void DeleteDataAndCash()
        {
            this.Delete();
            this.DeleteFromCash();
        }
        public void DeleteFromCash()
        {
            //删除缓存.
            CashEntity.Delete(this.ToString(), this.PKVal.ToString());
            // 删除数据.
            this.Row.Clear();
        }
        public int Delete()
        {
            if (this.beforeDelete() == false)
                return 0;

            int i = 0;
            try
            {
                i = EntityDBAccess.Delete(this);
            }
            catch (Exception ex)
            {
                Log.DebugWriteInfo(ex.Message);
                throw ex;
            }

            //更新缓存.  @wangyanyan
            if (this.EnMap.DepositaryOfEntity == Depositary.Application)
                Cash2019.DeleteRow(this.ToString(), this.PKVal.ToString());

            this.afterDelete();
            return i;
        }
        /// <summary>
        /// 直接删除指定的
        /// </summary>
        /// <param name="pk"></param>
        public int Delete(object pk)
        {
            Paras ps = new Paras();
            ps.Add(this.PK, pk);
            switch (this.EnMap.EnDBUrl.DBType)
            {
                case DBType.Oracle:
                case DBType.DM:
                case DBType.MSSQL:
                case DBType.MySQL:
                    return DBAccess.RunSQL("DELETE FROM " + this.EnMap.PhysicsTable + " WHERE " + this.PK + " =" + this.HisDBVarStr + pk);
                default:
                    throw new Exception("没有涉及到的类型。");
            }
        }
        /// <summary>
        /// 删除指定的数据
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="val"></param>
        public int Delete(string attr, object val)
        {
            Paras ps = new Paras();
            ps.Add(attr, val);
            if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                ps.Add(attr, BP.Sys.Glo.GenerRealType(this.EnMap.Attrs, attr, val));
            }
            else
            {
                ps.Add(attr, val);
            }

            return DBAccess.RunSQL("DELETE FROM " + this.EnMap.PhysicsTable + " WHERE " + this.EnMap.GetAttrByKey(attr).Field + " =" + this.HisDBVarStr + attr, ps);
        }
        public int Delete(string attr1, object val1, string attr2, object val2)
        {
            Paras ps = new Paras();

            if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                ps.Add(attr1, BP.Sys.Glo.GenerRealType(this.EnMap.Attrs, attr1, val1));
                ps.Add(attr2, BP.Sys.Glo.GenerRealType(this.EnMap.Attrs, attr2, val2));

            }
            else
            {
                ps.Add(attr1, val1);
                ps.Add(attr2, val2);
            }

            return DBAccess.RunSQL("DELETE FROM " + this.EnMap.PhysicsTable + " WHERE " + this.EnMap.GetAttrByKey(attr1).Field + " =" + this.HisDBVarStr + attr1 + " AND " + this.EnMap.GetAttrByKey(attr2).Field + " =" + this.HisDBVarStr + attr2, ps);
        }
        public int Delete(string attr1, object val1, string attr2, object val2, string attr3, object val3)
        {
            Paras ps = new Paras();

            if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                ps.Add(attr1, BP.Sys.Glo.GenerRealType(this.EnMap.Attrs, attr1, val1));
                ps.Add(attr2, BP.Sys.Glo.GenerRealType(this.EnMap.Attrs, attr2, val2));
                ps.Add(attr3, BP.Sys.Glo.GenerRealType(this.EnMap.Attrs, attr3, val3));
            }
            else
            {
                ps.Add(attr1, val1);
                ps.Add(attr2, val2);
                ps.Add(attr3, val3);
            }

            return DBAccess.RunSQL("DELETE FROM " + this.EnMap.PhysicsTable + " WHERE " + this.EnMap.GetAttrByKey(attr1).Field + " =" + this.HisDBVarStr + attr1 + " AND " + this.EnMap.GetAttrByKey(attr2).Field + " =" + this.HisDBVarStr + attr2 + " AND " + this.EnMap.GetAttrByKey(attr3).Field + " =" + this.HisDBVarStr + attr3, ps);
        }
        public int Delete(string attr1, object val1, string attr2, object val2, string attr3, object val3, string attr4, object val4)
        {
            Paras ps = new Paras();

            if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                ps.Add(attr1, BP.Sys.Glo.GenerRealType(this.EnMap.Attrs, attr1, val1));
                ps.Add(attr2, BP.Sys.Glo.GenerRealType(this.EnMap.Attrs, attr2, val2));
                ps.Add(attr3, BP.Sys.Glo.GenerRealType(this.EnMap.Attrs, attr3, val3));
                ps.Add(attr4, BP.Sys.Glo.GenerRealType(this.EnMap.Attrs, attr4, val4));

            }
            else
            {
                ps.Add(attr1, val1);
                ps.Add(attr2, val2);
                ps.Add(attr3, val3);
                ps.Add(attr4, val4);
            }

            return DBAccess.RunSQL("DELETE FROM " + this.EnMap.PhysicsTable + " WHERE " + this.EnMap.GetAttrByKey(attr1).Field + " =" + this.HisDBVarStr + attr1 + " AND " + this.EnMap.GetAttrByKey(attr2).Field + " =" + this.HisDBVarStr + attr2 + " AND " + this.EnMap.GetAttrByKey(attr3).Field + " =" + this.HisDBVarStr + attr3 + " AND " + this.EnMap.GetAttrByKey(attr4).Field + " =" + this.HisDBVarStr + attr4, ps);
        }
        protected virtual void afterDelete()
        {
            if (this.EnMap.DepositaryOfEntity != Depositary.Application)
                return;
            ///删除缓存。
            CashEntity.Delete(this.ToString(), this.PKVal.ToString());
            return;
        }
        #endregion

        #region 参数字段
        public AtPara atPara
        {
            get
            {
                AtPara at = this.Row.GetValByKey("_ATObj_") as AtPara;
                if (at != null)
                    return at;
                try
                {
                    string atParaStr = this.GetValStringByKey("AtPara");
                    if (DataType.IsNullOrEmpty(atParaStr))
                    {
                        /*没有发现数据，就执行初始化.*/
                        this.InitParaFields();

                        // 重新获取一次。
                        atParaStr = this.GetValStringByKey("AtPara");
                        if (DataType.IsNullOrEmpty(atParaStr))
                            atParaStr = "";

                        at = new AtPara(atParaStr);
                        this.SetValByKey("_ATObj_", at);
                        return at;
                    }
                    at = new AtPara(atParaStr);
                    this.SetValByKey("_ATObj_", at);
                    return at;
                }
                catch (Exception ex)
                {
                    throw new Exception("@获取参数AtPara时出现异常" + ex.Message + "，可能是您没有加入约定的参数字段AtPara. " + ex.Message);
                }
            }
        }
        /// <summary>
        /// 初始化参数字段(需要子类重写)
        /// </summary>
        /// <returns></returns>
        protected virtual void InitParaFields()
        {
        }
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetParaString(string key)
        {
            return atPara.GetValStrByKey(key);
        }
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isNullAsVal"></param>
        /// <returns></returns>
        public string GetParaString(string key, string isNullAsVal)
        {
            string str = atPara.GetValStrByKey(key);
            if (DataType.IsNullOrEmpty(str))
                return isNullAsVal;
            return str;
        }
        /// <summary>
        /// 获取参数Init值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetParaInt(string key, int isNullAsVal = 0)
        {
            return atPara.GetValIntByKey(key, isNullAsVal);
        }
        public float GetParaFloat(string key, float isNullAsVal = 0)
        {
            return atPara.GetValFloatByKey(key, isNullAsVal);
        }
        /// <summary>
        /// 获取参数boolen值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetParaBoolen(string key)
        {
            return atPara.GetValBoolenByKey(key);
        }
        /// <summary>
        /// 获取参数boolen值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="IsNullAsVal"></param>
        /// <returns></returns>
        public bool GetParaBoolen(string key, bool IsNullAsVal)
        {
            return atPara.GetValBoolenByKey(key, IsNullAsVal);
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void SetPara(string key, string obj)
        {
            if (atPara != null)
                this.Row.Remove("_ATObj_");

            string atParaStr = this.GetValStringByKey("AtPara");
            if (atParaStr.Contains("@" + key + "=") == false)
            {
                atParaStr += "@" + key + "=" + obj;
                this.SetValByKey("AtPara", atParaStr);
            }
            else
            {
                AtPara at = new AtPara(atParaStr);
                at.SetVal(key, obj);
                this.SetValByKey("AtPara", at.GenerAtParaStrs());
            }
        }
        public void SetPara(string key, int obj)
        {
            SetPara(key, obj.ToString());
        }
        public void SetPara(string key, float obj)
        {
            SetPara(key, obj.ToString());
        }
        public void SetPara(string key, bool obj)
        {
            if (obj == false)
                SetPara(key, "0");
            else
                SetPara(key, "1");
        }
        #endregion

        #region 通用方法
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="key"></param>
        public object GetRefObject(string key)
        {
            return this.Row["_" + key];
            //object obj = this.Row[key];
            //if (obj == null)
            //{
            //    if (this.Row.ContainsKey(key) == false)
            //        return null;
            //    obj = this.Row[key];
            //}
            //return obj;
        }
        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void SetRefObject(string key, object obj)
        {
            if (obj == null)
                return;

            this.Row.SetValByKey("_" + key, obj);
        }
        #endregion

        #region insert
        /// <summary>
        /// 在插入之前要做的工作。
        /// </summary>
        /// <returns></returns>
        protected virtual bool beforeInsert()
        {
            return true;
        }
        protected virtual bool roll()
        {
            return true;
        }
        public virtual void InsertWithOutPara()
        {
            this.RunSQL(SqlBuilder.Insert(this));
        }
        /// <summary>
        /// Insert .
        /// </summary>
        public virtual int Insert()
        {
            if (this.beforeInsert() == false)
                return 0;

            if (this.beforeUpdateInsertAction() == false)
                return 0;

            int i = 0;
            try
            {
                i = this.DirectInsert();
            }
            catch (Exception ex)
            {
                this.CheckPhysicsTable();

                //执行字段扩充检查.
                bool isCheck = CheckPhysicsTableAutoExtFieldLength(ex);
                if (isCheck == true)
                    return this.Insert();


                throw ex;
            }

            // 开始更新内存数据。 @wangyanyan
            if (this.EnMap.DepositaryOfEntity == Depositary.Application)
                Cash2019.PutRow(this.ToString(), this.PKVal.ToString(), this.Row);

            this.afterInsert();
            this.afterInsertUpdateAction();

            return i;
        }
        protected virtual void afterInsert()
        {
            //added by liuxc,2016-02-19,新建时，新增一个版本记录
            if (this.EnMap.IsEnableVer)
            {
                //增加版本为1的版本历史记录
                string enName = this.ToString();
                string rdt = DataType.CurrentDataTime;

                //edited by liuxc,2017-03-24,增加判断，如果相同主键的数据曾被删除掉，再次被增加时，会延续被删除时的版本，原有逻辑报错
                EnVer ver = new EnVer();
                ver.MyPK = enName + "_" + this.PKVal;

                if (ver.RetrieveFromDBSources() == 0)
                {
                    ver.No = enName;
                    ver.PKValue = this.PKVal.ToString();
                    ver.Name = this.EnMap.EnDesc;
                }
                else
                {
                    ver.EVer++;
                }

                ver.RDT = rdt;
                ver.Rec = BP.Web.WebUser.Name;
                ver.Save();

                // 保存字段数据.
                Attrs attrs = this.EnMap.Attrs;
                foreach (Attr attr in attrs)
                {
                    if (attr.IsRefAttr)
                        continue;

                    EnVerDtl dtl = new EnVerDtl();
                    dtl.EnVerPK = ver.MyPK;
                    dtl.EnVer = ver.EVer;
                    dtl.EnName = ver.No;
                    dtl.AttrKey = attr.Key;
                    dtl.AttrName = attr.Desc;
                    //dtl.OldVal = this.GetValStrByKey(attr.Key);   //第一个版本时，旧值没有
                    dtl.RDT = rdt;
                    dtl.Rec = BP.Web.WebUser.Name;
                    dtl.NewVal = this.GetValStrByKey(attr.Key);
                    dtl.MyPK = ver.MyPK + "_" + attr.Key + "_" + dtl.EnVer;
                    dtl.Insert();
                }
            }

            return;
        }
        /// <summary>
        /// 在更新与插入之后要做的工作.
        /// </summary>
        protected virtual void afterInsertUpdateAction()
        {
            if (this.EnMap.HisFKEnumAttrs.Count > 0)
                this.RetrieveFromDBSources();

            if (this.EnMap.IsAddRefName)
            {
                this.ReSetNameAttrVal();
                this.DirectUpdate();
            }
            return;
        }
        /// <summary>
        /// 从一个副本上copy.
        /// 用于两个数性基本相近的 实体 copy. 
        /// </summary>
        /// <param name="fromEn"></param>
        public virtual void Copy(Entity fromEn)
        {
            foreach (Attr attr in this.EnMap.Attrs)
            {
#warning zhoupeng 打开如下注释代码？没有考虑到为什么要改变PK.
                //if (attr.IsPK)
                //    continue;   //不能在打开，如果打开，就会与其他的约定出错，copy就是全部的属性，然后自己。

                object obj = fromEn.GetValByKey(attr.Key);
                if (obj == null)
                    continue;

                this.SetValByKey(attr.Key, obj);
            }
        }
        /// <summary>
        /// 从一个副本上
        /// </summary>
        /// <param name="fromRow"></param>
        public virtual void Copy(Row fromRow)
        {
            Attrs attrs = this.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                try
                {
                    this.SetValByKey(attr.Key, fromRow.GetValByKey(attr.Key));
                }
                catch
                {
                }
            }
        }
        public virtual void Copy(XmlEn xmlen)
        {
            Attrs attrs = this.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                object obj = null;
                try
                {
                    obj = xmlen.GetValByKey(attr.Key);
                }
                catch
                {
                    continue;
                }

                if (obj == null || obj.ToString() == "")
                    continue;
                this.SetValByKey(attr.Key, xmlen.GetValByKey(attr.Key));
            }
        }
        /// <summary>
        /// 复制 Hashtable
        /// </summary>
        /// <param name="ht"></param>
        public virtual void Copy(System.Collections.Hashtable ht)
        {
            foreach (string k in ht.Keys)
            {
                object obj = null;
                try
                {
                    obj = ht[k];
                }
                catch
                {
                    continue;
                }

                if (obj == null || obj.ToString() == "")
                    continue;
                this.SetValByKey(k, obj);
            }
        }
        public virtual void Copy(DataRow dr)
        {
            foreach (Attr attr in this.EnMap.Attrs)
            {
                try
                {
                    this.SetValByKey(attr.Key, dr[attr.Key]);
                }
                catch
                {
                }
            }
        }
        public string Copy(string refDoc)
        {
            foreach (Attr attr in this._enMap.Attrs)
            {
                refDoc = refDoc.Replace("@" + attr.Key, this.GetValStrByKey(attr.Key));
            }
            return refDoc;
        }


        public void Copy()
        {
            foreach (Attr attr in this.EnMap.Attrs)
            {
                if (attr.IsPK == false)
                    continue;

                if (attr.MyDataType == DataType.AppInt)
                    this.SetValByKey(attr.Key, 0);
                else
                    this.SetValByKey(attr.Key, "");
            }

            try
            {
                this.SetValByKey("No", "");
            }
            catch
            {
            }
        }
        #endregion

        #region verify
        /// <summary>
        /// 校验数据
        /// </summary>
        /// <returns></returns>
        public bool verifyData()
        {
            return true;

            string str = "";
            Attrs attrs = this.EnMap.Attrs;
            string s;
            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.MyDataType == DataType.AppString && attr.MinLength > 0)
                {
                    if (attr.UIIsReadonly)
                        continue;

                    s = this.GetValStrByKey(attr.Key);
                    // 处理特殊字符.
                    s = s.Replace("'", "~");
                    s = s.Replace("\"", "”");
                    s = s.Replace(">", "》");
                    s = s.Replace("<", "《");
                    this.SetValByKey(attr.Key, s);

                    if (s.Length < attr.MinLength || s.Length > attr.MaxLength)
                    {
                        if (attr.Key == "No" && attr.UIIsReadonly)
                        {
                            if (this.GetValStringByKey(attr.Key).Trim().Length == 0 || this.GetValStringByKey(attr.Key) == "自动生成")
                                this.SetValByKey("No", this.GenerNewNoByKey("No"));
                        }
                        else
                        {
                            str += "@[" + attr.Key + "," + attr.Desc + "]输入错误，请输入 " + attr.MinLength + " ～ " + attr.MaxLength + " 个字符范围，当前为空。";
                        }
                    }
                }

                //else if (attr.MyDataType == DataType.AppDateTime)
                //{
                //    if (this.GetValStringByKey(attr.Key).Trim().Length != 16)
                //    {
                //        //str+="@["+ attr.Desc +"]输入日期时间格式错误，输入的字段值["+this.GetValStringByKey(attr.Key)+"]不符合系统格式"+DataType.SysDataTimeFormat+"要求。";
                //    }
                //}
                //else if (attr.MyDataType == DataType.AppDate)
                //{
                //    if (this.GetValStringByKey(attr.Key).Trim().Length != 10)
                //    {
                //        //str+="@["+ attr.Desc +"]输入日期格式错误，输入的字段值["+this.GetValStringByKey(attr.Key)+"]不符合系统格式"+DataType.SysDataFormat+"要求。";
                //    }
                //}
            }

            if (str == "")
                return true;



            // throw new Exception("@在保存[" + this.EnDesc + "],PK[" + this.PK + "=" + this.PKVal + "]时出现信息录入不整错误：" + str);

            if (SystemConfig.IsDebug)
                throw new Exception("@在保存[" + this.EnDesc + "],主键[" + this.PK + "=" + this.PKVal + "]时出现信息录入不整错误：" + str);
            else
                throw new Exception("@在保存[" + this.EnDesc + "][" + this.EnMap.GetAttrByKey(this.PK).Desc + "=" + this.PKVal + "]时错误：" + str);
        }
        #endregion

        #region 更新，插入之前的工作。
        protected virtual bool beforeUpdateInsertAction()
        {
            switch (this.EnMap.EnType)
            {
                case EnType.View:
                case EnType.XML:
                case EnType.Ext:
                    return false;
                default:
                    break;
            }

            this.verifyData();
            return true;
        }
        #endregion 更新，插入之前的工作。

        #region 更新操作
        /// <summary>
        /// 更新

        /// <returns></returns>
        public virtual int Update()
        {
            return this.Update(null);
        }
        /// <summary>
        /// 仅仅更新一个属性
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="val1">val1</param>
        /// <returns>更新的个数</returns>
        public int Update(string key1, object val1)
        {
            this.SetValByKey(key1, val1);

            string sql = "";

            if (val1.GetType() == typeof(int)
                || val1.GetType() == typeof(float)
                || val1.GetType() == typeof(Int64)
                || val1.GetType() == typeof(decimal))
                sql = "UPDATE " + this.EnMap.PhysicsTable + " SET " + key1 + " =" + val1 + " WHERE " + this.PK + "='" + this.PKVal + "'";
            if (val1.GetType() == typeof(string))
                sql = "UPDATE " + this.EnMap.PhysicsTable + " SET " + key1 + " ='" + val1 + "' WHERE " + this.PK + "='" + this.PKVal + "'";

            return this.RunSQL(sql);
        }
        public int Update(string key1, object val1, string key2, object val2)
        {
            this.SetValByKey(key1, val1);
            this.SetValByKey(key2, val2);

            key1 = key1 + "," + key2;
            return this.Update(key1.Split(','));
        }
        public int Update(string key1, object val1, string key2, object val2, string key3, object val3)
        {
            this.SetValByKey(key1, val1);
            this.SetValByKey(key2, val2);
            this.SetValByKey(key3, val3);

            key1 = key1 + "," + key2 + "," + key3;
            return this.Update(key1.Split(','));
        }
        public int Update(string key1, object val1, string key2, object val2, string key3, object val3, string key4, object val4)
        {
            this.SetValByKey(key1, val1);
            this.SetValByKey(key2, val2);
            this.SetValByKey(key3, val3);
            this.SetValByKey(key4, val4);
            key1 = key1 + "," + key2 + "," + key3 + "," + key4;
            return this.Update(key1.Split(','));
        }
        public int Update(string key1, object val1, string key2, object val2, string key3, object val3, string key4, object val4, string key5, object val5)
        {
            this.SetValByKey(key1, val1);
            this.SetValByKey(key2, val2);
            this.SetValByKey(key3, val3);
            this.SetValByKey(key4, val4);
            this.SetValByKey(key5, val5);

            key1 = key1 + "," + key2 + "," + key3 + "," + key4 + "," + key5;
            return this.Update(key1.Split(','));
        }
        public int Update(string key1, object val1, string key2, object val2, string key3, object val3, string key4, object val4, string key5, object val5, string key6, object val6)
        {
            this.SetValByKey(key1, val1);
            this.SetValByKey(key2, val2);
            this.SetValByKey(key3, val3);
            this.SetValByKey(key4, val4);
            this.SetValByKey(key5, val5);
            this.SetValByKey(key6, val6);
            key1 = key1 + "," + key2 + "," + key3 + "," + key4 + "," + key5 + "," + key6;
            return this.Update(key1.Split(','));
        }
        protected virtual bool beforeUpdate()
        {
            return true;
        }
        /// <summary>
        /// 更新实体
        /// </summary>
        public int Update(string[] keys)
        {
            string str = "";
            try
            {
                str = "@更新之前出现错误 ";
                if (this.beforeUpdate() == false)
                    return 0;

                str = "@更新插入之前出现错误";
                if (this.beforeUpdateInsertAction() == false)
                    return 0;

                str = "@更新时出现错误";
                int i = EntityDBAccess.Update(this, keys);
                str = "@更新之后出现错误";

                // 开始更新内存数据。
                switch (this.EnMap.DepositaryOfEntity)
                {
                    case Depositary.Application:
                        //this.DeleteFromCash();
                        CashEntity.Update(this.ToString(), this.PKVal.ToString(), this);
                        break;
                    case Depositary.None:
                        break;
                }

                //更新缓存. @wangyanyan
                if (this.EnMap.DepositaryOfEntity == Depositary.Application)
                {
                    Cash2019.UpdateRow(this.ToString(), this.PKVal.ToString(), this.Row);
                }

                this.afterUpdate();
                str = "@更新插入之后出现错误";
                this.afterInsertUpdateAction();
                return i;
            }
            catch (System.Exception ex)
            {

                if (ex.Message.Contains("列名") || ex.Message.Contains("将截断字符串") || ex.Message.Contains("缺少") || ex.Message.Contains("的值太大"))
                {
                    /*说明字符串长度有问题.*/
                    this.CheckPhysicsTable();

                    //执行字段扩充检查.
                    bool isCheck = CheckPhysicsTableAutoExtFieldLength(ex);
                    if (isCheck == true)
                        return this.Update();

                    ///*比较参数那个字段长度有问题*/
                    //string errs = "";
                    //foreach (Attr attr in this.EnMap.Attrs)
                    //{
                    //    if (attr.MyDataType != DataType.AppString)
                    //        continue;

                    //    if (attr.MaxLength < this.GetValStrByKey(attr.Key).Length)
                    //    {
                    //        errs += "@映射里面的" + attr.Key + "," + attr.Desc + ", 相对于输入的数据:{" + this.GetValStrByKey(attr.Key) + "}, 太长。";
                    //    }
                    //}

                    //if (errs != "")
                    //    throw new Exception("@执行更新[" + this.ToString() + "]出现错误@错误字段:" + errs + " <br>清你在提交一次。" + ex.Message);
                    //else
                    //    throw ex;
                }

                Log.DefaultLogWriteLine(LogType.Error, ex.Message);
                if (SystemConfig.IsDebug)
                    throw new Exception("@[" + this.EnDesc + "]更新期间出现错误:" + str + ex.Message);
                else
                    throw ex;
            }
        }
        private int UpdateOfDebug(string[] keys)
        {
            string str = "";
            try
            {
                str = "@在更新之前出现错误";
                if (this.beforeUpdate() == false)
                {
                    return 0;
                }
                str = "@在beforeUpdateInsertAction出现错误";
                if (this.beforeUpdateInsertAction() == false)
                {
                    return 0;
                }
                int i = EntityDBAccess.Update(this, keys);
                str = "@在afterUpdate出现错误";
                this.afterUpdate();
                str = "@在afterInsertUpdateAction出现错误";
                this.afterInsertUpdateAction();
                //this.UpdateMemory();
                return i;
            }
            catch (System.Exception ex)
            {
                string msg = "@[" + this.EnDesc + "]UpdateOfDebug更新期间出现错误:" + str + ex.Message;
                Log.DefaultLogWriteLine(LogType.Error, msg);

                if (SystemConfig.IsDebug)
                    throw new Exception(msg);
                else
                    throw ex;
            }
        }

        protected virtual void afterUpdate()
        {
            if (this.EnMap.IsEnableVer)
            {
                /*处理版本号管理.*/
                // 取出来原来最后的版本数据.

                string enName = this.ToString();
                string rdt = DataType.CurrentDataTime;

                EnVer ver = new EnVer();
                ver.Retrieve(EnVerAttr.MyPK, enName + "_" + this.PKVal);

                EnVerDtl dtl = null;
                EnVerDtl dtlTemp = null;

                if (ver.RetrieveFromDBSources() == 0)
                {
                    /*初始化数据.*/
                    ver.PKValue = this.PKVal.ToString();
                    ver.No = enName;
                    ver.RDT = rdt;
                    ver.Name = this.EnMap.EnDesc;
                    ver.Rec = BP.Web.WebUser.Name;
                    ver.Insert();

                    foreach (Attr attr in this.EnMap.Attrs)
                    {
                        if (attr.IsRefAttr)
                            continue;

                        dtl = new EnVerDtl();
                        dtl.EnVerPK = ver.MyPK;
                        dtl.EnVer = ver.EVer;
                        dtl.EnName = ver.No;
                        dtl.AttrKey = attr.Key;
                        dtl.AttrName = attr.Desc;

                        dtl.RDT = rdt;
                        dtl.Rec = BP.Web.WebUser.Name;
                        dtl.NewVal = this.GetValStrByKey(attr.Key);

                        dtl.MyPK = ver.MyPK + "_" + attr.Key + "_" + dtl.EnVer;
                        dtl.Insert();
                    }
                    return;
                }

                //更新主版本信息.
                ver.EVer += 1;
                ver.Rec = BP.Web.WebUser.No;
                ver.RDT = rdt;
                ver.Update();

                //获取上一版本的数据
                EnVerDtls dtls = new EnVerDtls(ver.MyPK, ver.EVer - 1);

                // 保存字段数据.
                Attrs attrs = this.EnMap.Attrs;
                foreach (Attr attr in attrs)
                {
                    if (attr.IsRefAttr)
                        continue;

                    dtl = new BP.Sys.EnVerDtl();
                    dtl.EnVerPK = ver.MyPK;
                    dtl.EnVer = ver.EVer;
                    dtl.EnName = ver.No;
                    dtl.AttrKey = attr.Key;
                    dtl.AttrName = attr.Desc;

                    dtlTemp = dtls.GetEntityByKey(EnVerDtlAttr.AttrKey, attr.Key) as EnVerDtl;

                    if (dtlTemp != null)
                        dtl.OldVal = dtlTemp.NewVal;
                    else
                        dtl.OldVal = this.GetValStrByKey(attr.Key);

                    dtl.RDT = rdt;
                    dtl.Rec = BP.Web.WebUser.Name;
                    dtl.NewVal = this.GetValStrByKey(attr.Key);

                    dtl.MyPK = ver.MyPK + "_" + attr.Key + "_" + dtl.EnVer;
                    dtl.Insert();
                }
            }
            return;
        }

        #region 对文件的处理.
        public void SaveBigTxtToDB(string saveToField, string bigTxt)
        {

            DBAccess.SaveBigTextToDB(bigTxt, this.EnMap.PhysicsTable, this.PK, this.PKVal.ToString(), saveToField);

        }
        /// <summary>
        /// 保存文件到数据库
        /// </summary>
        /// <param name="saveToField">要保存的字段</param>
        /// <param name="bytes">文件流</param>
        public void SaveFileToDB(string saveToField, byte[] bytesOfFile)
        {
            try
            {
                DBAccess.SaveBytesToDB(bytesOfFile, this.EnMap.PhysicsTable, this.PK, this.PKVal.ToString(), saveToField);
            }
            catch (Exception ex)
            {
                /* 为了防止任何可能出现的数据丢失问题，您应该先仔细检查此脚本，然后再在数据库设计器的上下文之外运行此脚本。*/
                string sql = "";
                if (DBAccess.AppCenterDBType == DBType.MSSQL)
                    sql = "ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + saveToField + " Image NULL ";

                if (DBAccess.AppCenterDBType == DBType.Oracle)
                    sql = "ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + saveToField + " Image NULL ";

                if (DBAccess.AppCenterDBType == DBType.MySQL)
                    sql = "ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + saveToField + " Image NULL ";

                DBAccess.RunSQL(sql);

                throw new Exception("@保存文件期间出现错误，有可能该字段没有被自动创建，现在已经执行创建修复数据表，请重新执行一次." + ex.Message);
            }
        }

        /// <summary>
        /// 保存文件到数据库
        /// </summary>
        /// <param name="saveToField">要保存的字段</param>
        /// <param name="fileFullName">文件路径</param>
        public void SaveFileToDB(string saveToField, string fileFullName)
        {
            try
            {
                DBAccess.SaveFileToDB(fileFullName, this.EnMap.PhysicsTable, this.PK, this.PKVal.ToString(), saveToField);
            }
            catch (Exception ex)
            {
                /* 为了防止任何可能出现的数据丢失问题，您应该先仔细检查此脚本，然后再在数据库设计器的上下文之外运行此脚本。*/
                string sql = "";
                if (DBAccess.AppCenterDBType == DBType.MSSQL)
                    sql = "ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + saveToField + " Image NULL ";

                if (DBAccess.AppCenterDBType == DBType.Oracle)
                    sql = "ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + saveToField + " Blob NULL ";

                if (DBAccess.AppCenterDBType == DBType.MySQL)
                    sql = "ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + saveToField + " MediumBlob NULL ";

                DBAccess.RunSQL(sql);

                throw new Exception("@保存文件期间出现错误，有可能该字段没有被自动创建，现在已经执行创建修复数据表，请重新执行一次." + ex.Message);
            }
        }
        /// <summary>
        /// 从表的字段里读取文件
        /// </summary>
        /// <param name="saveToField">字段</param>
        /// <param name="filefullName">文件路径,如果为空怎不保存直接返回文件流，如果不为空则创建文件。</param>
        /// <returns>返回文件流</returns>
        public byte[] GetFileFromDB(string saveToField, string filefullName)
        {
            return DBAccess.GetByteFromDB(this.EnMap.PhysicsTable, this.PK, this.PKVal.ToString(), saveToField);
        }
        /// <summary>
        /// 从表的字段里读取string
        /// </summary>
        /// <param name="imgFieldName">字段名</param>
        /// <returns>大文本数据</returns>
        public string GetBigTextFromDB(string imgFieldName)
        {
            return DBAccess.GetBigTextFromDB(this.EnMap.PhysicsTable, this.PK, this.PKVal.ToString(), imgFieldName);
        }
        #endregion 对文件的处理.

        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public virtual int Save()
        {
            switch (this.PK)
            {
                case "OID":
                    if (this.GetValIntByKey("OID") == 0)
                    {
                        //this.SetValByKey("OID",EnDA.GenerOID());
                        this.Insert();
                        return 1;
                    }
                    else
                    {
                        this.Update();
                        return 1;
                    }
                    break;
                case "MyPK":
                case "No":
                case "ID":
                    //自动生成的MYPK，插入前获取主键
                    this.beforeUpdateInsertAction();
                    string pk = this.GetValStrByKey(this.PK);

                    if (pk == "" || pk == null)
                    {
                        this.Insert();
                        return 1;
                    }
                    else
                    {
                        int i = this.Update();
                        if (i == 0)
                        {
                            this.Insert();
                            i = 1;
                        }
                        return i;
                    }
                    break;
                default:
                    if (this.Update() == 0)
                        this.Insert();
                    return 1;
                    break;
            }
        }
        #endregion

        #region 关于数据库的处理
        /// <summary>
        /// 检查是否是日期
        /// </summary>
        protected void CheckDateAttr()
        {
            Attrs attrs = this.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyDataType == DataType.AppDate 
                    || attr.MyDataType == DataType.AppDateTime)
                {
                    DateTime dt = this.GetValDateTime(attr.Key);
                }
            }
        }
        /// <summary>
        /// 创建物理表
        /// </summary>
        protected void CreatePhysicsTable()
        {
            if (this._enMap.EnDBUrl.DBUrlType == DBUrlType.AppCenterDSN)
            {
                string sql = "";
                switch (DBAccess.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.DM:
                        sql = SqlBuilder.GenerCreateTableSQLOfOra(this);
                        break;
                    case DBType.Informix:
                        sql = SqlBuilder.GenerCreateTableSQLOfInfoMix(this);
                        break;
                    case DBType.PostgreSQL:
                        sql = SqlBuilder.GenerCreateTableSQLOfPostgreSQL(this);
                        break;
                    case DBType.MSSQL:
                        sql = SqlBuilder.GenerCreateTableSQLOfMS(this);
                        break;
                    case DBType.MySQL:
                        sql = SqlBuilder.GenerCreateTableSQLOfMySQL(this);
                        break;
                    case DBType.Access:
                        sql = SqlBuilder.GenerCreateTableSQLOf_OLE(this);
                        break;
                    default:
                        throw new Exception("@未判断的数据库类型。");
                }

                this.RunSQL(sql);
                this.CreateIndexAndPK();
                return;
            }


            if (this._enMap.EnDBUrl.DBUrlType == DBUrlType.DBAccessOfMSSQL1)
            {
                DBAccessOfMSSQL1.RunSQL(SqlBuilder.GenerCreateTableSQLOfMS(this));
                this.CreateIndexAndPK();
                return;
            }

            if (this._enMap.EnDBUrl.DBUrlType == DBUrlType.DBAccessOfMSSQL2)
            {
                DBAccessOfMSSQL2.RunSQL(SqlBuilder.GenerCreateTableSQLOfMS(this));
                this.CreateIndexAndPK();
                return;
            }

            //if (this._enMap.EnDBUrl.DBUrlType == DBUrlType.DBAccessOfOracle1)
            //{
            //    DBAccessOfOracle1.RunSQL(SqlBuilder.GenerCreateTableSQLOfOra(this));
            //    this.CreateIndexAndPK();
            //    return;
            //}
            //if (this._enMap.EnDBUrl.DBUrlType == DBUrlType.DBAccessOfOracle2)
            //{
            //    DBAccessOfOracle2.RunSQL(SqlBuilder.GenerCreateTableSQLOfOra(this));
            //    this.CreateIndexAndPK();
            //    return;
            //}

        }
        private void CreateIndexAndPK()
        {

            #region 建立主键
            if (DBAccess.IsExitsTabPK(this.EnMap.PhysicsTable) == false)
            {
                int pkconut = this.PKCount;
                if (pkconut == 1)
                {
                    DBAccess.CreatePK(this.EnMap.PhysicsTable, this.PKField, this.EnMap.EnDBUrl.DBType);
                    DBAccess.CreatIndex(this.EnMap.PhysicsTable, this.PKField);
                }
                else if (pkconut == 2)
                {

                    string pk0 = this.PKs[0];
                    string pk1 = this.PKs[1];
                    DBAccess.CreatePK(this.EnMap.PhysicsTable, pk0, pk1, this.EnMap.EnDBUrl.DBType);
                    DBAccess.CreatIndex(this.EnMap.PhysicsTable, pk0, pk1);
                }
                else if (pkconut == 3)
                {
                    string pk0 = this.PKs[0];
                    string pk1 = this.PKs[1];
                    string pk2 = this.PKs[2];
                    DBAccess.CreatePK(this.EnMap.PhysicsTable, pk0, pk1, pk2, this.EnMap.EnDBUrl.DBType);
                    DBAccess.CreatIndex(this.EnMap.PhysicsTable, pk0, pk1, pk2);
                }
            }
            #endregion
        }
        /// <summary>
        /// 如果一个属性是外键，并且它还有一个字段存储它的名称。
        /// 设置这个外键名称的属性。
        /// </summary>
        protected void ReSetNameAttrVal()
        {
            Attrs attrs = this.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.IsFKorEnum == false)
                    continue;

                string s = this.GetValRefTextByKey(attr.Key);
                this.SetValByKey(attr.Key + "Name", s);
            }
        }
        private void CheckPhysicsTable_SQL()
        {
            string table = this._enMap.PhysicsTable;
            DBType dbtype = this._enMap.EnDBUrl.DBType;
            string sqlFields = "";
            string sqlYueShu = "";


            DataTable dtAttr = DBAccess.RunSQLReturnTable(DBAccess.SQLOfTableFieldDesc(table));
            DataTable dtYueShu = DBAccess.RunSQLReturnTable(DBAccess.SQLOfTableFieldYueShu(table));

            #region 修复表字段。
            Attrs attrs = this._enMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr || attr.IsPK)
                    continue;

                string FType = "";
                string Flen = "";

                #region 判断是否存在.
                bool isHave = false;
                foreach (DataRow dr in dtAttr.Rows)
                {
                    if (dr["FName"].ToString().ToLower() == attr.Field.ToLower())
                    {
                        isHave = true;
                        FType = dr["FType"] as string;
                        Flen = dr["FLen"].ToString();
                        break;
                    }
                }
                if (isHave == false)
                {
                    /*不存在此列 , 就增加此列。*/
                    switch (attr.MyDataType)
                    {
                        case DataType.AppString:
                        case DataType.AppDate:
                        case DataType.AppDateTime:
                            int len = attr.MaxLength;
                            if (len == 0)
                                len = 200;
                            //throw new Exception("属性的最小长度不能为0。");
                            if (len > 4000)
                            {
                                if (dbtype == DBType.Access && len >= 254)
                                {
                                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + "  Memo DEFAULT '" + attr.DefaultVal + "' NULL");
                                }
                                else
                                {
                                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " NVARCHAR(MAX) DEFAULT '" + attr.DefaultVal + "' NULL");
                                }
                            }
                            else
                            {
                                if (dbtype == DBType.Access && len >= 254)
                                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + "  Memo DEFAULT '" + attr.DefaultVal + "' NULL");
                                else
                                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " NVARCHAR(" + len + ") DEFAULT '" + attr.DefaultVal + "' NULL");
                            }
                            continue;
                        case DataType.AppInt:
                            DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " INT DEFAULT '" + attr.DefaultVal + "' NULL");
                            continue;
                        case DataType.AppBoolean:
                            DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " INT DEFAULT '" + attr.DefaultVal + "' NULL");
                            continue;
                        case DataType.AppFloat:
                        case DataType.AppMoney:
                        case DataType.AppDouble:
                            DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " FLOAT DEFAULT '" + attr.DefaultVal + "' NULL");
                            continue;
                        default:
                            throw new Exception("error MyFieldType= " + attr.MyFieldType + " key=" + attr.Key);
                    }
                }
                #endregion

                #region 检查类型是否匹配.
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        if (FType.ToLower().Contains("char"))
                        {
                            /*类型正确，检查长度*/
                            if (attr.IsPK)
                                continue;

                            if (Flen == null)
                                throw new Exception("" + attr.Key + " -" + sqlFields);
                            int len = int.Parse(Flen);


                            if (len == -1)
                                continue; /*有可能是 nvarchar(MAX) */

                            if (len < attr.MaxLength)
                            {
                                try
                                {
                                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ALTER column " + attr.Key + " VARCHAR(" + attr.MaxLength + ")");
                                }
                                catch
                                {
                                    /*如果类型不匹配，就删除它在重新建, 先删除约束，在删除列，在重建。*/
                                    foreach (DataRow dr in dtYueShu.Rows)
                                    {
                                        if (dr["FName"].ToString().ToLower() == attr.Key.ToLower())
                                            DBAccess.RunSQL("ALTER TABLE " + table + " drop constraint " + dr[0].ToString());
                                    }

                                    // 在执行一遍.
                                    if (attr.MaxLength >= 4000)
                                        DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ALTER column " + attr.Key + " VARCHAR(4000)");
                                    else
                                        DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ALTER column " + attr.Key + " VARCHAR(" + attr.MaxLength + ")");
                                }
                            }
                        }
                        else
                        {
                            string err = "err@字段类型不匹配,表[" + this.EnMap.PhysicsTable + "]字段[" + attr.Key + "]名称[" + attr.Desc + "]映射类型为[" + attr.MyDataTypeStr + "],数据类型为[" + FType + "]";
                            Log.DebugWriteWarning(err);

                            // throw new Exception();

                            ///*如果类型不匹配，就删除它在重新建, 先删除约束，在删除列，在重建。*/
                            //foreach (DataRow dr in dtYueShu.Rows)
                            //{
                            //    if (dr["FName"].ToString().ToLower() == attr.Key.ToLower())
                            //        DBAccess.RunSQL("ALTER TABLE " + table + " drop constraint " + dr[0].ToString());
                            //}

                            //DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " drop column " + attr.Field);
                            //DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " NVARCHAR(" + attr.MaxLength + ") DEFAULT '" + attr.DefaultVal + "' NULL");
                            continue;
                        }
                        break;
                    case DataType.AppInt:
                    case DataType.AppBoolean:
                        if (FType.Contains("int") == false)
                        {
                            string err = "err@字段类型不匹配,表[" + this.EnMap.PhysicsTable + "]字段[" + attr.Key + "]名称[" + attr.Desc + "]映射类型为[" + attr.MyDataTypeStr + "],数据类型为[" + FType + "]";
                            Log.DebugWriteWarning(err);
                            ///*如果类型不匹配，就删除它在重新建, 先删除约束，在删除列，在重建。*/
                            //foreach (DataRow dr in dtYueShu.Rows)
                            //{
                            //    if (dr["FName"].ToString().ToLower() == attr.Key.ToLower())
                            //        DBAccess.RunSQL("alter table " + table + " drop constraint " + dr[0].ToString());
                            //}
                            //DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " drop column " + attr.Field);
                            //DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " INT DEFAULT '" + attr.DefaultVal + "' NULL");
                            //continue;
                        }
                        break;
                    case DataType.AppFloat:
                    case DataType.AppMoney:
                    case 9:
                        if (FType.Contains("float") == false)
                        {
                            string err = "err@字段类型不匹配,表[" + this.EnMap.PhysicsTable + "]字段[" + attr.Key + "]名称[" + attr.Desc + "]映射类型为[" + attr.MyDataTypeStr + "],数据类型为[" + FType + "]";
                            Log.DebugWriteWarning(err);

                            ///*如果类型不匹配，就删除它在重新建, 先删除约束，在删除列，在重建。*/
                            //foreach (DataRow dr in dtYueShu.Rows)
                            //{
                            //    if (dr["FName"].ToString().ToLower() == attr.Key.ToLower())
                            //        DBAccess.RunSQL("alter table " + table + " drop constraint " + dr[0].ToString());
                            //}
                            //DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " drop column " + attr.Field);
                            //DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " FLOAT DEFAULT '" + attr.DefaultVal + "' NULL");
                            //continue;
                        }
                        break;
                    default:
                        //  throw new Exception("error MyFieldType= " + attr.MyFieldType + " key=" + attr.Key);
                        break;
                }
                #endregion
            }
            #endregion 修复表字段。

            #region 检查枚举类型是否存在.
            attrs = this._enMap.HisEnumAttrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyDataType != DataType.AppInt)
                    continue;

                if (attr.UITag == null)
                    continue;

                try
                {
                    SysEnums ses = new SysEnums(attr.UIBindKey, attr.UITag);
                    continue;
                }
                catch
                {
                }

                try
                {
                    string[] strs = attr.UITag.Split('@');
                    SysEnums ens = new SysEnums();
                    ens.Delete(SysEnumAttr.EnumKey, attr.UIBindKey);
                    foreach (string s in strs)
                    {
                        if (DataType.IsNullOrEmpty(s) == true)
                            continue;

                        string[] vk = s.Split('=');
                        SysEnum se = new SysEnum();
                        se.IntKey = int.Parse(vk[0]);
                        se.Lab = vk[1];
                        se.EnumKey = attr.UIBindKey;
                        se.Insert();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("@自动增加枚举时出现错误，请确定您的格式是否正确。" + ex.Message + "attr.UIBindKey=" + attr.UIBindKey);
                }

            }
            #endregion

            #region 建立主键
            if (DBAccess.IsExitsTabPK(this._enMap.PhysicsTable) == false)
            {
                int pkconut = this.PKCount;
                if (pkconut == 1)
                {
                    DBAccess.CreatePK(this._enMap.PhysicsTable, this.PKField, this._enMap.EnDBUrl.DBType);
                    DBAccess.CreatIndex(this._enMap.PhysicsTable, this.PKField);
                }
                else if (pkconut == 2)
                {
                    string pk0 = this.PKs[0];
                    string pk1 = this.PKs[1];
                    DBAccess.CreatePK(this._enMap.PhysicsTable, pk0, pk1, this._enMap.EnDBUrl.DBType);
                    DBAccess.CreatIndex(this._enMap.PhysicsTable, pk0, pk1);
                }
                else if (pkconut == 3)
                {
                    string pk0 = this.PKs[0];
                    string pk1 = this.PKs[1];
                    string pk2 = this.PKs[2];
                    DBAccess.CreatePK(this._enMap.PhysicsTable, pk0, pk1, pk2, this._enMap.EnDBUrl.DBType);
                    DBAccess.CreatIndex(this._enMap.PhysicsTable, pk0, pk1, pk2);
                }
            }
            #endregion

            #region 重命名表名字段名.

            String ptable = this.EnMap.PhysicsTable;

            string sql = "exec sp_rename '" + this.EnMap.PhysicsTable + "','" + this.EnMap.PhysicsTable + "'";
            DBAccess.RunSQL(sql);

            foreach (Attr item in this.EnMap.Attrs)
            {
                if (item.IsRefAttr == true)
                    continue;

                sql = "exec sp_rename '" + ptable + ".[" + item.Key + "]','" + item.Key + "','column';";
                DBAccess.RunSQL(sql);
            }
            #endregion 重命名表名字段名.

        }
        /// <summary>
        /// PostgreSQL 检查.
        /// </summary>
        private void CheckPhysicsTable_PostgreSQL()
        {
            string table = this._enMap.PhysicsTable;
            DBType dbtype = this._enMap.EnDBUrl.DBType;
            string sqlFields = "";
            string sqlYueShu = "";

            //字段信息: 名称fname, 类型ftype, 长度flen.
            sqlFields = "SELECT a.attname as fname, format_type(a.atttypid,a.atttypmod) as type,  0 as FLen, 'xxxxxxx' as FType,";
            sqlFields += " a.attnotnull as notnull FROM pg_class as c,pg_attribute as a  ";
            sqlFields += " where c.relname = '" + this.EnMap.PhysicsTable.ToLower() + "' and a.attrelid = c.oid and a.attnum>0  ";

            //约束信息.
            //sqlYueShu = "SELECT b.name, a.name FName from sysobjects b join syscolumns a on b.id = a.cdefault where a.id = object_id('" + this.EnMap.PhysicsTable + "') ";

            DataTable dtAttr = DBAccess.RunSQLReturnTable(sqlFields);

            foreach (DataRow dr in dtAttr.Rows)
            {
                string type = dr["type"].ToString();
                if (type.Contains("char"))
                {
                    dr["ftype"] = "char";

                    int start = type.IndexOf('(') + 1;
                    int end = type.IndexOf(')');
                    string len = type.Substring(start, end - start);
                    dr["flen"] = int.Parse(len);
                }
                else
                {
                    dr["ftype"] = type;
                }

            }
            //DataTable dtYueShu = DBAccess.RunSQLReturnTable(sqlYueShu);

            #region 修复表字段。
            Attrs attrs = this._enMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr || attr.IsPK)
                    continue;

                string FType = "";
                string Flen = "";

                #region 判断是否存在.
                bool isHave = false;
                foreach (DataRow dr in dtAttr.Rows)
                {
                    if (dr["FName"].ToString().ToLower() == attr.Field.ToLower())
                    {
                        isHave = true;
                        FType = dr["FType"] as string;
                        Flen = dr["FLen"].ToString();
                        break;
                    }
                }
                if (isHave == false)
                {
                    /*不存在此列 , 就增加此列。*/
                    switch (attr.MyDataType)
                    {
                        case DataType.AppString:
                        case DataType.AppDate:
                        case DataType.AppDateTime:
                            int len = attr.MaxLength;
                            if (len == 0)
                                len = 200;
                            //throw new Exception("属性的最小长度不能为0。");
                            if (len > 4000)
                            {
                                if (dbtype == DBType.Access && len >= 254)
                                {
                                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + "  Memo DEFAULT '" + attr.DefaultVal + "' NULL");
                                }
                                else
                                {
                                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " VARCHAR(" + len + ") DEFAULT '" + attr.DefaultVal + "' NULL");
                                }
                            }
                            else
                            {
                                if (dbtype == DBType.Access && len >= 254)
                                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + "  Memo DEFAULT '" + attr.DefaultVal + "' NULL");
                                else
                                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " VARCHAR(" + len + ") DEFAULT '" + attr.DefaultVal + "' NULL");
                            }
                            continue;
                        case DataType.AppInt:
                            DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " INT DEFAULT '" + attr.DefaultVal + "' NULL");
                            continue;
                        case DataType.AppBoolean:
                            DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " INT DEFAULT '" + attr.DefaultVal + "' NULL");
                            continue;
                        case DataType.AppFloat:
                        case DataType.AppMoney:
                        case DataType.AppDouble:
                            DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " FLOAT DEFAULT '" + attr.DefaultVal + "' NULL");
                            continue;
                        default:
                            throw new Exception("err@MyFieldType= " + attr.MyFieldType + " key=" + attr.Key);
                    }
                }
                #endregion

                #region 检查类型是否匹配.
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        if (FType.ToLower().Contains("char"))
                        {
                            /*类型正确，检查长度*/
                            if (attr.IsPK)
                                continue;

                            if (Flen == null)
                                throw new Exception("" + attr.Key + " -" + sqlFields);
                            int len = int.Parse(Flen);


                            if (len == -1)
                                continue; /*有可能是 nvarchar(MAX) */

                            if (len < attr.MaxLength)
                            {
                                try
                                {
                                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ALTER column " + attr.Key + " type character varying(" + attr.MaxLength + ")");
                                }
                                catch
                                {
                                    /*如果类型不匹配，就删除它在重新建, 先删除约束，在删除列，在重建。*/
                                    //foreach (DataRow dr in dtYueShu.Rows)
                                    //{
                                    //    if (dr["FName"].ToString().ToLower() == attr.Key.ToLower())
                                    //        DBAccess.RunSQL("ALTER TABLE " + table + " drop constraint " + dr[0].ToString());
                                    //}

                                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ALTER column " + attr.Key + " type character varying(" + attr.MaxLength + ")");
                                }
                            }
                        }
                        else if (FType.ToLower().Contains("text"))
                        {

                        }
                        else
                        {
                            string err = "err@字段类型不匹配,表[" + this.EnMap.PhysicsTable + "]字段[" + attr.Key + "]名称[" + attr.Desc + "]映射类型为[" + attr.MyDataTypeStr + "],数据类型为[" + FType + "]";
                            Log.DebugWriteWarning(err);

                            // throw new Exception();

                            ///*如果类型不匹配，就删除它在重新建, 先删除约束，在删除列，在重建。*/
                            //foreach (DataRow dr in dtYueShu.Rows)
                            //{
                            //    if (dr["FName"].ToString().ToLower() == attr.Key.ToLower())
                            //        DBAccess.RunSQL("ALTER TABLE " + table + " drop constraint " + dr[0].ToString());
                            //}

                            //DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " drop column " + attr.Field);
                            DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " VARCHAR(" + attr.MaxLength + ") DEFAULT '" + attr.DefaultVal + "' NULL");
                            continue;
                        }
                        break;
                    case DataType.AppInt:
                    case DataType.AppBoolean:
                        if (FType.Contains("int") == false)
                        {
                            //  string err = "err@字段类型不匹配,表[" + this.EnMap.PhysicsTable + "]字段[" + attr.Key + "]名称[" + attr.Desc + "]映射类型为[" + attr.MyDataTypeStr + "],数据类型为[" + FType + "]";
                            //    Log.DebugWriteWarning(err);
                            ///*如果类型不匹配，就删除它在重新建, 先删除约束，在删除列，在重建。*/
                            //foreach (DataRow dr in dtYueShu.Rows)
                            //{
                            //    if (dr["FName"].ToString().ToLower() == attr.Key.ToLower())
                            //        DBAccess.RunSQL("alter table " + table + " drop constraint " + dr[0].ToString());
                            //}
                            //DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " drop column " + attr.Field);
                            //DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " INT DEFAULT '" + attr.DefaultVal + "' NULL");
                            //continue;
                        }
                        break;
                    case DataType.AppFloat:
                    case DataType.AppMoney:
                    case 9:
                        if (FType != "float")
                        {
                            //  string err = "err@字段类型不匹配,表[" + this.EnMap.PhysicsTable + "]字段[" + attr.Key + "]名称[" + attr.Desc + "]映射类型为[" + attr.MyDataTypeStr + "],数据类型为[" + FType + "]";
                            //  Log.DebugWriteWarning(err);

                            ///*如果类型不匹配，就删除它在重新建, 先删除约束，在删除列，在重建。*/
                            //foreach (DataRow dr in dtYueShu.Rows)
                            //{
                            //    if (dr["FName"].ToString().ToLower() == attr.Key.ToLower())
                            //        DBAccess.RunSQL("alter table " + table + " drop constraint " + dr[0].ToString());
                            //}
                            //DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " drop column " + attr.Field);
                            //DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " FLOAT DEFAULT '" + attr.DefaultVal + "' NULL");
                            //continue;
                        }
                        break;
                    default:
                        //  throw new Exception("error MyFieldType= " + attr.MyFieldType + " key=" + attr.Key);
                        break;
                }
                #endregion
            }
            #endregion 修复表字段。

            #region 检查枚举类型是否存在.
            attrs = this._enMap.HisEnumAttrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyDataType != DataType.AppInt)
                    continue;

                if (attr.UITag == null)
                    continue;

                try
                {
                    SysEnums ses = new SysEnums(attr.UIBindKey, attr.UITag);
                    continue;
                }
                catch
                {
                }

                try
                {
                    string[] strs = attr.UITag.Split('@');
                    SysEnums ens = new SysEnums();
                    ens.Delete(SysEnumAttr.EnumKey, attr.UIBindKey);
                    foreach (string s in strs)
                    {
                        if (DataType.IsNullOrEmpty(s) == true)
                            continue;

                        string[] vk = s.Split('=');
                        SysEnum se = new SysEnum();
                        se.IntKey = int.Parse(vk[0]);
                        se.Lab = vk[1];
                        se.EnumKey = attr.UIBindKey;
                        se.Insert();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("@自动增加枚举时出现错误，请确定您的格式是否正确。" + ex.Message + "attr.UIBindKey=" + attr.UIBindKey);
                }

            }
            #endregion

            #region 建立主键
            if (DBAccess.IsExitsTabPK(this._enMap.PhysicsTable) == false)
            {
                int pkconut = this.PKCount;
                if (pkconut == 1)
                {
                    DBAccess.CreatePK(this._enMap.PhysicsTable, this.PKField, this._enMap.EnDBUrl.DBType);
                    DBAccess.CreatIndex(this._enMap.PhysicsTable, this.PKField);
                }
                else if (pkconut == 2)
                {
                    string pk0 = this.PKs[0];
                    string pk1 = this.PKs[1];
                    DBAccess.CreatePK(this._enMap.PhysicsTable, pk0, pk1, this._enMap.EnDBUrl.DBType);
                    DBAccess.CreatIndex(this._enMap.PhysicsTable, pk0, pk1);
                }
                else if (pkconut == 3)
                {
                    string pk0 = this.PKs[0];
                    string pk1 = this.PKs[1];
                    string pk2 = this.PKs[2];
                    DBAccess.CreatePK(this._enMap.PhysicsTable, pk0, pk1, pk2, this._enMap.EnDBUrl.DBType);
                    DBAccess.CreatIndex(this._enMap.PhysicsTable, pk0, pk1, pk2);
                }
            }
            #endregion

            #region 创建索引.
            if (this._enMap.IndexField != null)
            {
                DBAccess.CreatIndex(this._enMap.PhysicsTable, this._enMap.IndexField);
            }

            int pkconut22 = this.PKCount;
            if (pkconut22 == 1)
            {
                DBAccess.CreatIndex(this._enMap.PhysicsTable, this.PKField);
            }
            else if (pkconut22 == 2)
            {
                string pk0 = this.PKs[0];
                string pk1 = this.PKs[1];
                DBAccess.CreatIndex(this._enMap.PhysicsTable, pk0, pk1);
            }
            else if (pkconut22 == 3)
            {
                string pk0 = this.PKs[0];
                string pk1 = this.PKs[1];
                string pk2 = this.PKs[2];
                DBAccess.CreatIndex(this._enMap.PhysicsTable, pk0, pk1, pk2);
            }
            #endregion
        }
        /// <summary>
        /// 自动扩展长度
        /// </summary>
        public bool CheckPhysicsTableAutoExtFieldLength(Exception ex)
        {
            this._enMap = this.EnMap;

            //  string msg = "";
            if (this._enMap.EnType == EnType.View
                || this._enMap.EnType == EnType.XML
                || this._enMap.EnType == EnType.ThirdPartApp
                || this._enMap.EnType == EnType.Ext)
                return false;


            string sql = "";

            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    return CheckPhysicsTableAutoExtFieldLength_SQL();
                case DBType.Oracle:
                case DBType.DM:
                    break;
                case DBType.MySQL:
                    // sql = "select character_maximum_length as Len, table_schema as OWNER FROM information_schema.columns WHERE TABLE_SCHEMA='" + SystemConfig.AppCenterDBDatabase + "' AND table_name ='" + this._enMap.PhysicsTable + "' and column_Name='" + attr.Field + "' AND character_maximum_length < " + attr.MaxLength;
                    //return CheckPhysicsTableAutoExtFieldLength_MySQL(sql);
                    break;
                case DBType.Informix:
                    break;
                case DBType.PostgreSQL:
                    break;
                default:
                    throw new Exception("@没有涉及到的数据库类型");
            }

            return false;
        }

        private bool CheckPhysicsTableAutoExtFieldLength_SQL()
        {
            string sqlFields = "SELECT column_name as FName,data_type as FType,CHARACTER_MAXIMUM_LENGTH as FLen from information_schema.columns where table_name='" + this.EnMap.PhysicsTable + "'";
            //sqlYueShu = "SELECT b.name, a.name FName from sysobjects b join syscolumns a on b.id = a.cdefault where a.id = object_id('" + this.EnMap.PhysicsTable + "') ";
            //原始的
            DataTable dtAttr = DBAccess.RunSQLReturnTable(sqlFields);

            //是否有? 没有check就返回.
            bool isCheckIt = false;

            //遍历属性.
            Attrs attrs = this._enMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyDataType != DataType.AppString)
                    continue;

                int dbLen = 0;
                foreach (DataRow dr in dtAttr.Rows)
                {
                    if (dr["FName"].ToString().Equals(attr.Key) == true)
                    {
                        dbLen = int.Parse(dr["FLen"].ToString());
                        break;
                    }
                }

                //如果是负数，就是maxvarchar 的类型.
                if (dbLen <= 0)
                    continue;

                //获得长度.
                string val = this.GetValStrByKey(attr.Key);
                if (val.Length <= dbLen)
                    continue;

                //字段长度.
                string sql = "";
                if (val.Length >= 4000)
                    sql = "ALTER TABLE " + this.EnMap.PhysicsTable + " ALTER column " + attr.Key + " nvarchar(MAX)";
                else
                    sql = "ALTER TABLE " + this.EnMap.PhysicsTable + " ALTER column " + attr.Key + " VARCHAR(" + val.Length + ")";

                try
                {
                    DBAccess.RunSQL(sql);
                    isCheckIt = true;
                }
                catch (Exception ex)
                {
                    var valNum = DBAccess.DropConstraintOfSQL(this.EnMap.PhysicsTable, attr.Key);
                    if (valNum > 0)
                    {
                        DBAccess.RunSQL(sql);
                        isCheckIt = true; //设置是否更新到.
                    }
                    //throw new Exception("err@类["+this.ToString()+"],表["+this.EnMap.PhysicsTable+"],自动扩展字段:(" + attr.Key + ")的长度，修复SQL("+sql+")字段长度("+val.Length+")字段值：(" + val + ")");
                }
            }
            //返回是否检测到。
            return isCheckIt;
        }
        /// <summary>
        /// 检查物理表
        /// </summary>
        public void CheckPhysicsTable()
        {
            this._enMap = this.EnMap;
            //  string msg = "";
            if (this._enMap.EnType == EnType.View
                || this._enMap.EnType == EnType.XML
                || this._enMap.EnType == EnType.ThirdPartApp
                || this._enMap.EnType == EnType.Ext)
                return;

            if (DBAccess.IsExitsObject(this._enMap.EnDBUrl, this._enMap.PhysicsTable) == false)
            {
                /* 如果物理表不存在就新建立一个物理表。*/
                this.CreatePhysicsTable();
                return;
            }
            if (this._enMap.IsView)
                return;

            //检查是否有对应的主键.
            string pk = this.PK;
            if (pk.Contains(",") == false)
                if (this.EnMap.Attrs.Contains(pk) == false)
                    throw new Exception("err@Entity" + this.ToString() + "," + this.EnMap.EnDesc + "的Map设置错误主键为【" + pk + "】但是没有" + pk + "的属性.");

            DBType dbtype = this._enMap.EnDBUrl.DBType;

            // 如果不是主应用程序的数据库就不让执行检查. 考虑第三方的系统的安全问题.
            if (this._enMap.EnDBUrl.DBUrlType
                != DBUrlType.AppCenterDSN)
                return;
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    this.CheckPhysicsTable_SQL();
                    break;
                case DBType.Oracle:
                case DBType.DM:
                    this.CheckPhysicsTable_Ora();
                    break;
                case DBType.MySQL:
                    this.CheckPhysicsTable_MySQL();
                    break;
                case DBType.Informix:
                    this.CheckPhysicsTable_Informix();
                    break;
                case DBType.PostgreSQL:
                    this.CheckPhysicsTable_PostgreSQL();
                    break;
                default:
                    throw new Exception("@没有涉及到的数据库类型");
            }

            ////检查从表.
            //MapDtls dtls = new MapDtls(this.ClassID);
            //foreach (MapDtl dtl in dtls)
            //{
            //    GEDtl dtlen = new GEDtl(dtl.No);
            //    dtlen.CheckPhysicsTable();
            //}
        }
        private void CheckPhysicsTable_Informix()
        {
            #region 检查字段是否存在
            string sql = "SELECT *  FROM " + this.EnMap.PhysicsTable + " WHERE 1=2";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //如果不存在.
            foreach (Attr attr in this.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.IsPK)
                    continue;

                if (dt.Columns.Contains(attr.Key) == true)
                    continue;

                if (attr.Key == "AID")
                {
                    /* 自动增长列 */
                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " INT  Identity(1,1)");
                    continue;
                }

                /*不存在此列 , 就增加此列。*/
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        int len = attr.MaxLength;
                        if (len == 0)
                            len = 200;

                        if (len >= 254)
                            DBAccess.RunSQL("alter table " + this.EnMap.PhysicsTable + " add " + attr.Field + " lvarchar(" + len + ") default '" + attr.DefaultVal + "'");
                        else
                            DBAccess.RunSQL("alter table " + this.EnMap.PhysicsTable + " add " + attr.Field + " varchar(" + len + ") default '" + attr.DefaultVal + "'");
                        break;
                    case DataType.AppInt:
                    case DataType.AppBoolean:
                        DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " INT8 DEFAULT " + attr.DefaultVal + " ");
                        break;
                    case DataType.AppFloat:
                    case DataType.AppMoney:
                    case DataType.AppDouble:
                        DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " FLOAT DEFAULT  " + attr.DefaultVal + " ");
                        break;
                    default:
                        throw new Exception("error MyFieldType= " + attr.MyFieldType + " key=" + attr.Key);
                }
            }
            #endregion

            #region 检查字段长度是否符合最低要求
            foreach (Attr attr in this.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                if (attr.MyDataType == DataType.AppDouble
                    || attr.MyDataType == DataType.AppFloat
                    || attr.MyDataType == DataType.AppInt
                    || attr.MyDataType == DataType.AppMoney
                    || attr.MyDataType == DataType.AppBoolean)
                    continue;

                int maxLen = attr.MaxLength;
                dt = new DataTable();
                sql = "select c.*  from syscolumns c inner join systables t on c.tabid = t.tabid where t.tabname = lower('" + this.EnMap.PhysicsTable.ToLower() + "') and c.colname = lower('" + attr.Key.ToLower() + "') and c.collength < " + attr.MaxLength;
                dt = this.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    continue;
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        if (attr.MaxLength >= 255)
                            this.RunSQL("alter table " + dr["owner"] + "." + this.EnMap.PhysicsTableExt + " modify " + attr.Field + " lvarchar(" + attr.MaxLength + ")");
                        else
                            this.RunSQL("alter table " + dr["owner"] + "." + this.EnMap.PhysicsTableExt + " modify " + attr.Field + " varchar(" + attr.MaxLength + ")");
                    }
                    catch (Exception ex)
                    {
                        Log.DebugWriteWarning(ex.Message);
                    }
                }
            }
            #endregion

            #region 检查枚举类型字段是否是INT 类型
            Attrs attrs = this._enMap.HisEnumAttrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyDataType != DataType.AppInt)
                    continue;
#warning 没有处理好。
                continue;

                sql = "SELECT DATA_TYPE FROM ALL_TAB_COLUMNS WHERE  TABLE_NAME='" + this.EnMap.PhysicsTableExt.ToLower() + "' AND COLUMN_NAME='" + attr.Field.ToLower() + "'";
                string val = DBAccess.RunSQLReturnString(sql);
                if (val == null)
                    Log.DefaultLogWriteLineError("@没有检测到字段:" + attr.Key);

                if (val.IndexOf("CHAR") != -1)
                {
                    /*如果它是 varchar 字段*/
                    sql = "SELECT OWNER FROM ALL_TAB_COLUMNS WHERE upper(TABLE_NAME)='" + this.EnMap.PhysicsTableExt.ToUpper() + "' AND UPPER(COLUMN_NAME)='" + attr.Field.ToUpper() + "' ";
                    string OWNER = DBAccess.RunSQLReturnString(sql);
                    try
                    {
                        this.RunSQL("alter table  " + this.EnMap.PhysicsTableExt + " modify " + attr.Field + " NUMBER ");
                    }
                    catch (Exception ex)
                    {
                        Log.DefaultLogWriteLineError("运行sql 失败:alter table  " + this.EnMap.PhysicsTableExt + " modify " + attr.Field + " NUMBER " + ex.Message);
                    }
                }
            }
            #endregion

            #region 检查枚举类型是否存在.
            attrs = this._enMap.HisEnumAttrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyDataType != DataType.AppInt)
                    continue;
                if (attr.UITag == null)
                    continue;
                try
                {
                    SysEnums ses = new SysEnums(attr.UIBindKey, attr.UITag);
                    continue;
                }
                catch
                {
                }
                string[] strs = attr.UITag.Split('@');
                SysEnums ens = new SysEnums();
                ens.Delete(SysEnumAttr.EnumKey, attr.UIBindKey);
                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s) == true)
                        continue;

                    string[] vk = s.Split('=');
                    SysEnum se = new SysEnum();
                    se.IntKey = int.Parse(vk[0]);
                    se.Lab = vk[1];
                    se.EnumKey = attr.UIBindKey;
                    se.Insert();
                }
            }
            #endregion

            this.CreateIndexAndPK();
        }
        private void CheckPhysicsTable_MySQL()
        {
            #region 检查字段是否存在
            string sql = "SELECT *  FROM " + this._enMap.PhysicsTable + " WHERE 1=2";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //如果不存在.
            foreach (Attr attr in this._enMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.IsPK)
                    continue;

                if (dt.Columns.Contains(attr.Key) == true)
                    continue; //不存在此列.

                if (attr.Key == "AID")
                {
                    /* 自动增长列 */
                    DBAccess.RunSQL("ALTER TABLE " + this._enMap.PhysicsTable + " ADD " + attr.Field + " INT  Identity(1,1) COMMENT '" + attr.Desc + "'");
                    continue;
                }

                /*不存在此列 , 就增加此列。*/
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        int len = attr.MaxLength;
                        if (len == 0)
                            len = 200;
                        if (len > 3000)
                            DBAccess.RunSQL("ALTER TABLE " + this._enMap.PhysicsTable + " ADD " + attr.Field + " TEXT COMMENT '" + attr.Desc + "'");
                        else
                            DBAccess.RunSQL("ALTER TABLE " + this._enMap.PhysicsTable + " ADD " + attr.Field + " NVARCHAR(" + len + ") DEFAULT '" + attr.DefaultVal + "' NULL COMMENT '" + attr.Desc + "'");
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        DBAccess.RunSQL("ALTER TABLE " + this._enMap.PhysicsTable + " ADD " + attr.Field + " NVARCHAR(20) DEFAULT '" + attr.DefaultVal + "' NULL COMMENT '" + attr.Desc + "'");
                        break;
                    case DataType.AppInt:
                    case DataType.AppBoolean:
                        DBAccess.RunSQL("ALTER TABLE " + this._enMap.PhysicsTable + " ADD " + attr.Field + " INT DEFAULT '" + attr.DefaultVal + "' NULL COMMENT '" + attr.Desc + "'");
                        break;
                    case DataType.AppFloat:
                        DBAccess.RunSQL("ALTER TABLE " + this._enMap.PhysicsTable + " ADD " + attr.Field + " FLOAT (11,2) DEFAULT '" + attr.DefaultVal + "' NULL COMMENT '" + attr.Desc + "'");
                        break;
                    case DataType.AppMoney:
                        DBAccess.RunSQL("ALTER TABLE " + this._enMap.PhysicsTable + " ADD " + attr.Field + " DECIMAL (20,4) DEFAULT '" + attr.DefaultVal + "' NULL COMMENT '" + attr.Desc + "'");
                        break;
                    case DataType.AppDouble:
                        DBAccess.RunSQL("ALTER TABLE " + this._enMap.PhysicsTable + " ADD " + attr.Field + " DOUBLE DEFAULT '" + attr.DefaultVal + "' NULL COMMENT '" + attr.Desc + "'");
                        break;
                    default:
                        throw new Exception("error MyFieldType= " + attr.MyFieldType + " key=" + attr.Key);
                }
            }
            #endregion

            #region 检查字段长度是否符合最低要求
            foreach (Attr attr in this._enMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                if (attr.MyDataType == DataType.AppDouble
                    || attr.MyDataType == DataType.AppFloat
                    || attr.MyDataType == DataType.AppInt
                    || attr.MyDataType == DataType.AppMoney
                    || attr.MyDataType == DataType.AppBoolean)
                    continue;

                int maxLen = attr.MaxLength;
                dt = new DataTable();
                sql = "select character_maximum_length as Len, table_schema as OWNER FROM information_schema.columns WHERE TABLE_SCHEMA='" + SystemConfig.AppCenterDBDatabase + "' AND table_name ='" + this._enMap.PhysicsTable + "' and column_Name='" + attr.Field + "' AND character_maximum_length < " + attr.MaxLength;
                dt = this.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    continue;
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        if (attr.MaxLength < 3000)
                            this.RunSQL("alter table " + dr["OWNER"] + "." + this._enMap.PhysicsTableExt + " modify " + attr.Field + " NVARCHAR(" + attr.MaxLength + ")");
                        else
                            this.RunSQL("alter table " + dr["OWNER"] + "." + this._enMap.PhysicsTableExt + " modify " + attr.Field + " text");
                    }
                    catch (Exception ex)
                    {
                        Log.DebugWriteWarning(ex.Message);
                    }
                }
            }
            #endregion

            #region 检查枚举类型字段是否是INT 类型
            Attrs attrs = this._enMap.HisEnumAttrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyDataType != DataType.AppInt)
                    continue;

                sql = "SELECT DATA_TYPE FROM information_schema.columns WHERE table_name='" + this._enMap.PhysicsTable + "' AND COLUMN_NAME='" + attr.Field + "' and table_schema='" + SystemConfig.AppCenterDBDatabase + "'";
                string val = DBAccess.RunSQLReturnString(sql);
                if (val == null)
                {
                    Log.DefaultLogWriteLineError("@没有检测到字段eunm" + attr.Key);
                    continue;
                }

                if (val.ToUpper().IndexOf("CHAR") != -1)
                {
                    /*如果它是 varchar 字段*/
                    sql = "SELECT table_schema as OWNER FROM information_schema.columns WHERE  table_name='" + this._enMap.PhysicsTableExt + "' AND COLUMN_NAME='" + attr.Field + "' and table_schema='" + SystemConfig.AppCenterDBDatabase + "'";
                    string OWNER = DBAccess.RunSQLReturnString(sql);
                    try
                    {
                        this.RunSQL("alter table  " + this._enMap.PhysicsTableExt + " modify " + attr.Field + " NUMBER ");
                    }
                    catch (Exception ex)
                    {
                        Log.DefaultLogWriteLineError("运行sql 失败:alter table  " + this._enMap.PhysicsTableExt + " modify " + attr.Field + " NUMBER " + ex.Message);
                    }
                }
            }
            #endregion

            #region 检查枚举类型是否存在.
            attrs = this._enMap.HisEnumAttrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyDataType != DataType.AppInt)
                    continue;
                if (attr.UITag == null)
                    continue;
                try
                {
                    SysEnums ses = new SysEnums(attr.UIBindKey, attr.UITag);
                    continue;
                }
                catch
                {
                }
                string[] strs = attr.UITag.Split('@');
                SysEnums ens = new SysEnums();
                ens.Delete(SysEnumAttr.EnumKey, attr.UIBindKey);
                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s) == true)
                        continue;

                    string[] vk = s.Split('=');
                    SysEnum se = new SysEnum();
                    se.IntKey = int.Parse(vk[0]);
                    se.Lab = vk[1];
                    se.EnumKey = attr.UIBindKey;
                    se.Insert();
                }
            }
            #endregion

            this.CreateIndexAndPK();
        }
        private void CheckPhysicsTable_Ora()
        {
            #region 检查字段是否存在
            //string sql = "SELECT * FROM " + this.EnMap.PhysicsTable + " WHERE 1=2 ";
            string sql = "SELECT WMSYS.WM_CONCAT(DISTINCT(column_name)) AS Column_Name  FROM all_tab_cols WHERE table_name = '" + this.EnMap.PhysicsTable.ToUpper() + "' AND owner='" + SystemConfig.AppCenterDBDatabase.ToUpper() + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return;

            string fields = "," + dt.Rows[0][0].ToString() + ",";
            fields = fields.ToUpper();
            //如果不存在.
            foreach (Attr attr in this.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.IsPK)
                    continue;

                if (fields.Contains("," + attr.Key.ToUpper() + ",") == true)
                    continue;

                //if (fields.Contains(attr.Key.ToUpper() + ",") == true)
                //    continue;

                //if (fields.Contains(","+attr.Key.ToUpper()) == true)
                //    continue;

                if (attr.Key == "AID")
                {
                    /* 自动增长列 */
                    DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " INT  Identity(1,1)");
                    continue;
                }

                /*不存在此列 , 就增加此列。*/
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        int len = attr.MaxLength;
                        if (len == 0)
                            len = 200;
                        DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " VARCHAR(" + len + ") DEFAULT '" + attr.DefaultVal + "' NULL");
                        break;
                    case DataType.AppInt:
                    case DataType.AppBoolean:
                        if (attr.IsPK == true)
                            DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " INT DEFAULT '" + attr.DefaultVal + "' NOT NULL");
                        else
                            DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " INT DEFAULT '" + attr.DefaultVal + "'   NULL");
                        break;
                    case DataType.AppFloat:
                    case DataType.AppMoney:
                    case DataType.AppDouble:
                        DBAccess.RunSQL("ALTER TABLE " + this.EnMap.PhysicsTable + " ADD " + attr.Field + " FLOAT DEFAULT '" + attr.DefaultVal + "' NULL");
                        break;
                    default:
                        throw new Exception("error MyFieldType= " + attr.MyFieldType + " Key=" + attr.Key);
                }
            }
            #endregion

            #region 检查字段长度是否符合最低要求
            foreach (Attr attr in this.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                if (attr.MyDataType == DataType.AppDouble
                    || attr.MyDataType == DataType.AppFloat
                    || attr.MyDataType == DataType.AppInt
                    || attr.MyDataType == DataType.AppMoney
                    || attr.MyDataType == DataType.AppBoolean)
                    continue;

                int maxLen = attr.MaxLength;
                dt = new DataTable();
                //sql = "SELECT DATA_LENGTH AS LEN, OWNER FROM ALL_TAB_COLUMNS WHERE upper(TABLE_NAME)='" + this.EnMap.PhysicsTableExt.ToUpper() 
                //    + "' AND UPPER(COLUMN_NAME)='" + attr.Field.ToUpper() + "' AND DATA_LENGTH < " + attr.MaxLength;

                //update dgq 2016-5-24 不取所有用户的表列名，只要取自己的就可以了
                sql = "SELECT DATA_LENGTH AS LEN FROM USER_TAB_COLUMNS WHERE upper(TABLE_NAME)='" + this.EnMap.PhysicsTableExt.ToUpper()
                    + "' AND UPPER(COLUMN_NAME)='" + attr.Field.ToUpper() + "' AND DATA_LENGTH < " + attr.MaxLength;
                dt = this.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    continue;

                foreach (DataRow dr in dt.Rows)
                {
                    //this.RunSQL("alter table " + dr["OWNER"] + "." + this.EnMap.PhysicsTableExt + " modify " + attr.Field + " varchar2(" + attr.MaxLength + ")");

                    this.RunSQL("alter table " + this.EnMap.PhysicsTableExt + " modify " + attr.Field + " varchar2(" + attr.MaxLength + ")");

                }
            }
            #endregion

            #region 检查枚举类型字段是否是INT 类型
            Attrs attrs = this._enMap.HisEnumAttrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyDataType != DataType.AppInt)
                    continue;
                sql = "SELECT DATA_TYPE FROM ALL_TAB_COLUMNS WHERE upper(TABLE_NAME)='" + this.EnMap.PhysicsTableExt.ToUpper() + "' AND UPPER(COLUMN_NAME)='" + attr.Field.ToUpper() + "' ";
                string val = DBAccess.RunSQLReturnString(sql);
                if (val == null)
                    Log.DefaultLogWriteLineError("@没有检测到字段eunm" + attr.Key);
                if (val.IndexOf("CHAR") != -1)
                {
                    /*如果它是 varchar 字段*/
                    sql = "SELECT OWNER FROM ALL_TAB_COLUMNS WHERE upper(TABLE_NAME)='" + this.EnMap.PhysicsTableExt.ToUpper() + "' AND UPPER(COLUMN_NAME)='" + attr.Field.ToUpper() + "' ";
                    string OWNER = DBAccess.RunSQLReturnString(sql);
                    try
                    {
                        this.RunSQL("alter table  " + this.EnMap.PhysicsTableExt + " modify " + attr.Field + " NUMBER ");
                    }
                    catch (Exception ex)
                    {
                        Log.DefaultLogWriteLineError("运行sql 失败:alter table  " + this.EnMap.PhysicsTableExt + " modify " + attr.Field + " NUMBER " + ex.Message);
                    }
                }
            }
            #endregion

            #region 检查枚举类型是否存在.
            attrs = this._enMap.HisEnumAttrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyDataType != DataType.AppInt)
                    continue;
                if (attr.UITag == null)
                    continue;
                try
                {
                    SysEnums ses = new SysEnums(attr.UIBindKey, attr.UITag);
                    continue;
                }
                catch
                {
                }
                string[] strs = attr.UITag.Split('@');
                SysEnums ens = new SysEnums();
                ens.Delete(SysEnumAttr.EnumKey, attr.UIBindKey);
                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s) == true)
                        continue;

                    string[] vk = s.Split('=');
                    SysEnum se = new SysEnum();
                    se.IntKey = int.Parse(vk[0]);
                    se.Lab = vk[1];
                    se.EnumKey = attr.UIBindKey;
                    se.Insert();
                }
            }
            #endregion
            this.CreateIndexAndPK();
        }
        #endregion

        /// <summary>
        /// 把entity的实体属性调度到en里面去.
        /// </summary>
        public MapData DTSMapToSys_MapData(string fk_mapdata = null)
        {
            if (fk_mapdata == null)
            {
                //fk_mapdata = this.ClassIDOfShort;

                //为了适应配置vsto系统的需要，这里需要全称.
                fk_mapdata = this.ToString();
            }

            Map map = this.EnMap;

            //获得短的类名称.

            #region 更新主表信息.
            MapData md = new MapData();
            md.No = fk_mapdata;
            if (md.RetrieveFromDBSources() == 0)
                md.Insert();

            md.EnPK = this.PK; //主键
            md.EnsName = fk_mapdata; //类名.
            md.Name = map.EnDesc;
            md.PTable = map.PhysicsTable;
            md.Update();
            #endregion 更新主表信息.

            //删除.
            MapAttrs attrs = new MapAttrs();
            attrs.Delete(MapAttrAttr.FK_MapData, fk_mapdata);

            //同步属性 mapattr.
            DTSMapToSys_MapData_InitMapAttr(map.Attrs, fk_mapdata);

            #region 同步从表.
            //同步从表.
            EnDtls dtls = map.Dtls;
            foreach (EnDtl dtl in dtls)
            {
                MapDtl mdtl = new MapDtl();

                Entity enDtl = dtl.Ens.GetNewEntity;

                mdtl.No = enDtl.ClassIDOfShort;
                if (mdtl.RetrieveFromDBSources() == 0)
                    mdtl.Insert();

                mdtl.Name = enDtl.EnDesc;
                mdtl.FK_MapData = fk_mapdata;
                mdtl.PTable = enDtl.EnMap.PhysicsTable;

                mdtl.RefPK = dtl.RefKey; //关联的主键.

                mdtl.Update();

                //同步字段.
                DTSMapToSys_MapData_InitMapAttr(enDtl.EnMap.Attrs, enDtl.ClassIDOfShort);
            }
            #endregion 同步从表.

            return md;
        }
        /// <summary>
        /// 同步字段属性
        /// </summary>
        /// <param name="attrs"></param>
        /// <param name="fk_mapdata"></param>
        private void DTSMapToSys_MapData_InitMapAttr(Attrs attrs, string fk_mapdata)
        {
            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr)
                    continue;

                MapAttr mattr = new MapAttr();
                mattr.KeyOfEn = attr.Key;
                mattr.FK_MapData = fk_mapdata;
                mattr.MyPK = mattr.FK_MapData + "_" + mattr.KeyOfEn;
                mattr.RetrieveFromDBSources();

                mattr.Name = attr.Desc;
                mattr.DefVal = attr.DefaultVal.ToString();
                mattr.KeyOfEn = attr.Field;

                mattr.MaxLen = attr.MaxLength;
                mattr.MinLen = attr.MinLength;
                mattr.UIBindKey = attr.UIBindKey;
                mattr.UIIsLine = attr.UIIsLine;
                mattr.UIHeight = 0;

                if (attr.MaxLength > 3000)
                    mattr.UIHeight = 10;

                mattr.UIWidth = attr.UIWidth;
                mattr.MyDataType = attr.MyDataType;

                mattr.UIRefKey = attr.UIRefKeyValue;

                mattr.UIRefKeyText = attr.UIRefKeyText;
                mattr.UIVisible = attr.UIVisible;

                //设置显示与隐藏，按照默认值.
                if (mattr.GetParaString("SearchVisable") == "")
                {
                    if (mattr.UIVisible == true)
                        mattr.SetPara("SearchVisable", 1);
                    else
                        mattr.SetPara("SearchVisable", 0);
                }


                switch (attr.MyFieldType)
                {
                    case FieldType.Enum:
                    case FieldType.PKEnum:
                        mattr.UIContralType = attr.UIContralType;
                        mattr.LGType = FieldTypeS.Enum;
                        mattr.UIIsEnable = attr.UIIsReadonly;
                        break;
                    case FieldType.FK:
                    case FieldType.PKFK:
                        mattr.UIContralType = attr.UIContralType;
                        mattr.LGType = FieldTypeS.FK;
                        //attr.MyDataType = (int)FieldType.FK;
                        mattr.UIRefKey = "No";
                        mattr.UIRefKeyText = "Name";
                        mattr.UIIsEnable = attr.UIIsReadonly;
                        break;
                    default:
                        mattr.UIContralType = UIContralType.TB;
                        mattr.LGType = FieldTypeS.Normal;
                        mattr.UIIsEnable = !attr.UIIsReadonly;
                        switch (attr.MyDataType)
                        {
                            case DataType.AppBoolean:
                                mattr.UIContralType = UIContralType.CheckBok;
                                mattr.UIIsEnable = attr.UIIsReadonly;
                                break;
                            case DataType.AppDate:
                                //if (this.Tag == "1")
                                //    attr.DefaultVal = DataType.CurrentData;
                                break;
                            case DataType.AppDateTime:
                                //if (this.Tag == "1")
                                //    attr.DefaultVal = DataType.CurrentData;
                                break;
                            default:
                                break;
                        }
                        break;
                }
                if (mattr.Update() == 0)
                    mattr.Insert();
            }
        }
    }
}
