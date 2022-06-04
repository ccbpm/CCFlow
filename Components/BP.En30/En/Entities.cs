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
using BP.Pub;


namespace BP.En
{
    /// <summary>
    /// 数据实体集合
    /// </summary>
    [Serializable]
    public abstract class Entities : CollectionBase
    {
        #region 查询方法.
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public virtual int RetrieveAllFromDBSource()
        {
            QueryObject qo = new QueryObject(this);
            return qo.DoQuery();
        }
        public virtual int RetrieveAllFromDBSource(string orderByAttr)
        {
            QueryObject qo = new QueryObject(this);
            qo.addOrderBy(orderByAttr);
            return qo.DoQuery();
        }
        #endregion 查询方法.

        #region 过滤
        public Entity Filter(string key, string val)
        {
            foreach (Entity en in this)
            {
                if (en.GetValStringByKey(key) == val)
                    return en;
            }
            return null;
        }
       
        #endregion

        #region 虚拟方法
        /// <summary>
        /// 按照属性查询
        /// </summary>
        /// <param name="attr">属性名称</param>
        /// <param name="val">值</param>
        /// <returns>是否查询到</returns>
        public int RetrieveByAttr(string attr, object val)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(attr, val);
            return qo.DoQuery();
        }
      
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public Entities() { }
        #endregion

        #region 公共方法
        /// <summary>
        /// 是否存在key= val 的实体。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool IsExits(string key, object val)
        {
            foreach (Entity en in this)
            {
                if (en.GetValStringByKey(key) == val.ToString())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 创建一个该集合的元素的类型的新实例
        /// </summary>
        /// <returns></returns>
        public abstract Entity GetNewEntity { get; }
        /// <summary>
        /// 根据位置取得数据
        /// </summary>
        public Entity this[int index]
        {
            get
            {
                return this.InnerList[index] as Entity;
            }
        }
        /// <summary>
        /// 将对象添加到集合尾处，如果对象已经存在，则不添加
        /// </summary>
        /// <param name="entity">要添加的对象</param>
        /// <returns>返回添加到的地方</returns>
        public virtual int AddEntity(Entity entity)
        {
            if (entity == null)
                throw new Exception("加入的 AddEntity 不能为空。");
            return this.InnerList.Add(entity);
        }
      
        public virtual void AddEntities(Entities ens)
        {
            foreach (Entity en in ens)
                this.AddEntity(en);
            // this.InnerList.AddRange(ens);
        }
        /// <summary>
        /// 增加entities
        /// </summary>
        /// <param name="pks">主键的值，中间用@符号隔开</param>
        public virtual void AddEntities(string pks)
        {
            this.Clear();
            if (pks == null || pks == "")
                return;

            string[] strs = pks.Split('@');
            foreach (string str in strs)
            {
                if (str == null || str == "")
                    continue;

                Entity en = this.GetNewEntity;
                en.PKVal = str;
                if (en.RetrieveFromDBSources() == 0)
                    continue;
                this.AddEntity(en);
            }
        }
       
        /// <summary>
        /// 判断是不是包含指定的Entity .
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public bool Contains(Entity en)
        {
            return this.Contains(en.PKVal);
        }
       
        public bool Contains(Entities ens, string key)
        {
            if (ens.Count == 0)
                return false;
            foreach (Entity en in ens)
            {
                if (this.Contains(key, en.GetValByKey(key)) == false)
                    return false;
            }
            return true;
        }
        
        /// <summary>
        /// 是不是包含指定的PK
        /// </summary>
        /// <param name="pkVal"></param>
        /// <returns></returns>
        public bool Contains(object pkVal)
        {
            string pk = this.GetNewEntity.PK;
            return this.Contains(pk, pkVal);
        }
        /// <summary>
        /// 指定的属性里面是否包含指定的值.
        /// </summary>
        /// <param name="attr">指定的属性</param>
        /// <param name="pkVal">指定的值</param>
        /// <returns>返回是否等于</returns>
        public bool Contains(string attr, object pkVal)
        {
            foreach (Entity myen in this)
            {
                if (myen.GetValByKey(attr).ToString().Equals(pkVal.ToString()))
                    return true;
            }
            return false;
        }
        public bool Contains(string attr1, object pkVal1, string attr2, object pkVal2)
        {
            foreach (Entity myen in this)
            {
                if (myen.GetValByKey(attr1).ToString().Equals(pkVal1.ToString()) && myen.GetValByKey(attr2).ToString().Equals(pkVal2.ToString())
                    )
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// 创建立本身的一个实例.
        /// </summary>
        /// <returns>Entities</returns>
        public Entities CreateInstance()
        {
            return ClassFactory.GetEns(this.ToString());
        }
        #endregion

        #region 获取一个实体
        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Entity GetEntityByKey(object val)
        {
            string pk = this.GetNewEntity.PK;
            foreach (Entity en in this)
            {
                if (en.GetValStrByKey(pk) == val.ToString())
                    return en;
            }
            return null;
        }
        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Entity GetEntityByKey(string attr, object val)
        {
            foreach (Entity en in this)
            {
                if (en.GetValByKey(attr).Equals(val))
                    return en;
            }
            return null;
        }
        public Entity GetEntityByKey(string attr, int val)
        {
            foreach (Entity en in this)
            {
                if (en.GetValIntByKey(attr) == val)
                    return en;
            }
            return null;
        }
        public Entity GetEntityByKey(string attr1, object val1, string attr2, object val2)
        {
            foreach (Entity en in this)
            {
                if (en.GetValStrByKey(attr1) == val1.ToString()
                    && en.GetValStrByKey(attr2) == val2.ToString())
                    return en;
            }
            return null;
        }
      
        /// <summary>
        /// 获得entis
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Entities GetEntitiesByKey(string attr, string val)
        {
            Entities ens = this.GetNewEntity.GetNewEntities;
            foreach (Entity en in this)
            {
                if (en.GetValStrByKey(attr).Equals(val) == false)
                    continue;
                ens.AddEntity(en);
            }
            if (ens.Count == 0)
                return null;
            return ens;
        }
        #endregion

        #region  对一个属性操作
        
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetSumIntByKey(string key)
        {
            int sum = 0;
            foreach (Entity en in this)
            {
                sum += en.GetValIntByKey(key);
            }
            return sum;
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float GetSumFloatByKey(string key)
        {
            float sum = 0;
            foreach (Entity en in this)
            {
                sum += en.GetValFloatByKey(key);
            }
            return sum;
        }

        /// <summary>
        /// 求个数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetCountByKey(string key, string val)
        {
            int sum = 0;
            foreach (Entity en in this)
            {
                if (en.GetValStringByKey(key) == val)
                    sum++;
            }
            return sum;
        }
     
        #endregion

        #region 对集合的操作
        /// <summary>
        /// 装载到内存
        /// </summary>
        /// <returns></returns>
        public int FlodInCash()
        {
            //this.Clear();
            QueryObject qo = new QueryObject(this);

            // qo.Top = 2000;
            int num = qo.DoQuery();

            /* 把查询个数加入内存 */
            Entity en = this.GetNewEntity;
            CashEntity.PubEns(en.ToString(), this, en.PK);
            BP.DA.Log.DebugWriteInfo("成功[" + en.ToString() + "-" + num + "]放入缓存。");
            return num;
        }
        
        /// <summary>
        /// 从集合中删除该对象
        /// </summary>
        /// <param name="entity"></param>
        public  void RemoveEn(Entity entity)
        {
            this.InnerList.Remove(entity);
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="pk"></param>
        public  void RemoveEn(string pk)
        {
            string key = this.GetNewEntity.PK;
            RemoveEn(key, pk);
        }
        public  void RemoveEn(string key, string val)
        {
            foreach (Entity en in this)
            {
                if (en.GetValStringByKey(key) == val)
                {
                    this.RemoveEn(en);
                    return;
                }
            }
        }
        public  void Remove(string pks)
        {
            //设置
            pks = pks.Replace(",", "@");

            string[] mypks = pks.Split('@');
            string pkAttr = this.GetNewEntity.PK;

            foreach (string pk in mypks)
            {
                if (pk == null || pk.Length == 0)
                    continue;

                this.RemoveEn(pkAttr, pk);
            }
        }

        /// <summary>
        /// 删除table.
        /// </summary>
        /// <returns></returns>
        public int ClearTable()
        {
            Entity en = this.GetNewEntity;
            return en.RunSQL("DELETE FROM " + en.EnMap.PhysicsTable);
        }
        /// <summary>
        /// 删除集合内的对象
        /// </summary>
        public void Delete()
        {
            foreach (Entity en in this)
                if (en.IsExits)
                    en.Delete();
            this.Clear();
        }
        public int RunSQL(string sql)
        {
            return this.GetNewEntity.RunSQL(sql);
        }
        public int Delete(string key, object val)
        {
            Entity en = this.GetNewEntity;
            Paras ps = new Paras();
            ps.SQL = "DELETE FROM " + en.EnMap.PhysicsTable + " WHERE " + key + "=" + en.HisDBVarStr + "p";

            if (val.GetType() != typeof(string))
            {
                Attr attr = en.EnMap.GetAttrByKey(key);
                if (attr.MyDataType == DataType.AppString)
                {
                    ps.Add("p", val.ToString());
                }
                else
                {
                    ps.Add("p", val);
                }
            }
            else
            {
                ps.Add("p", val);
            }
            //  ps.Add("p", val);
            return en.RunSQL(ps);
        }

        public int Delete(string key1, object val1, string key2, object val2)
        {
            Entity en = this.GetNewEntity;
            Paras ps = new Paras();
            ps.SQL = "DELETE FROM " + en.EnMap.PhysicsTable + " WHERE " + key1 + "=" + en.HisDBVarStr + "p1 AND " + key2 + "=" + en.HisDBVarStr + "p2";
            if (val1.GetType() != typeof(string))
            {
                Attr attr = en.EnMap.GetAttrByKey(key1);
                if (attr.MyDataType == DataType.AppString)
                {
                    ps.Add("p1", val1.ToString());
                }
                else
                {
                    ps.Add("p1", val1);
                }
            }
            else
            {
                ps.Add("p1", val1);
            }

            if (val2.GetType() != typeof(string))
            {
                Attr attr = en.EnMap.GetAttrByKey(key2);
                if (attr.MyDataType == DataType.AppString)
                {
                    ps.Add("p2", val2.ToString());
                }
                else
                {
                    ps.Add("p2", val2);
                }
            }
            else
            {
                ps.Add("p2", val2);
            }


            return en.RunSQL(ps);
        }
        public int Delete(string key1, object val1, string key2, object val2, string key3, object val3)
        {
            Entity en = this.GetNewEntity;
            Paras ps = new Paras();
            ps.SQL = "DELETE FROM " + en.EnMap.PhysicsTable + " WHERE " + key1 + "=" + en.HisDBVarStr + "p1 AND " + key2 + "=" + en.HisDBVarStr + "p2 AND " + key3 + "=" + en.HisDBVarStr + "p3";
            if (val1.GetType() != typeof(string))
            {
                Attr attr = en.EnMap.GetAttrByKey(key1);
                if (attr.MyDataType == DataType.AppString)
                {
                    ps.Add("p1", val1.ToString());
                }
                else
                {
                    ps.Add("p1", val1);
                }
            }
            else
            {
                ps.Add("p1", val1);
            }

            if (val2.GetType() != typeof(string))
            {
                Attr attr = en.EnMap.GetAttrByKey(key2);
                if (attr.MyDataType == DataType.AppString)
                {
                    ps.Add("p2", val2.ToString());
                }
                else
                {
                    ps.Add("p2", val2);
                }
            }
            else
            {
                ps.Add("p2", val2);
            }

            if (val3.GetType() != typeof(string))
            {
                Attr attr = en.EnMap.GetAttrByKey(key3);
                if (attr.MyDataType == DataType.AppString)
                {
                    ps.Add("p3", val3.ToString());
                }
                else
                {
                    ps.Add("p3", val3);
                }
            }
            else
            {
                ps.Add("p3", val3);
            }
             
            return en.RunSQL(ps);
        }
        public int Delete(string key1, object val1, string key2, object val2, string key3, object val3, string key4, object val4)
        {
            Entity en = this.GetNewEntity;
            Paras ps = new Paras();
            ps.SQL = "DELETE FROM " + en.EnMap.PhysicsTable + " WHERE " + key1 + "=" + en.HisDBVarStr + "p1 AND " + key2 + "=" + en.HisDBVarStr + "p2 AND " + key3 + "=" + en.HisDBVarStr + "p3 AND " + key4 + "=" + en.HisDBVarStr + "p4";
            if (val1.GetType() != typeof(string))
            {
                Attr attr = en.EnMap.GetAttrByKey(key1);
                if (attr.MyDataType == DataType.AppString)
                {
                    ps.Add("p1", val1.ToString());
                }
                else
                {
                    ps.Add("p1", val1);
                }
            }
            else
            {
                ps.Add("p1", val1);
            }

            if (val2.GetType() != typeof(string))
            {
                Attr attr = en.EnMap.GetAttrByKey(key2);
                if (attr.MyDataType == DataType.AppString)
                {
                    ps.Add("p2", val2.ToString());
                }
                else
                {
                    ps.Add("p2", val2);
                }
            }
            else
            {
                ps.Add("p2", val2);
            }

            if (val3.GetType() != typeof(string))
            {
                Attr attr = en.EnMap.GetAttrByKey(key3);
                if (attr.MyDataType == DataType.AppString)
                {
                    ps.Add("p3", val3.ToString());
                }
                else
                {
                    ps.Add("p3", val3);
                }
            }
            else
            {
                ps.Add("p3", val3);
            }

            if (val4.GetType() != typeof(string))
            {
                Attr attr = en.EnMap.GetAttrByKey(key4);
                if (attr.MyDataType == DataType.AppString)
                {
                    ps.Add("p4", val4.ToString());
                }
                else
                {
                    ps.Add("p4", val4);
                }
            }
            else
            {
                ps.Add("p4", val4);
            }
            return en.RunSQL(ps);
        }
       
        public void SaveToXml(string file)
        {
            string dir = "";

            if (file.Contains("\\"))
            {
                dir = file.Substring(0, file.LastIndexOf('\\'));
            }
            else if (file.Contains("/"))
            {
                dir = file.Substring(0, file.LastIndexOf("/"));
            }

            if (dir != "")
            {
                if (System.IO.Directory.Exists(dir) == false)
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
            }

            DataTable dt = this.ToDataTableField();
            DataSet ds = new DataSet();
            ds.Tables.Add(dt); //  this.ToDataSet();
            ds.WriteXml(file);
        }
        #endregion

        #region 查询方法
        /// <summary>
        /// 按LIKE 去查.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public virtual int RetrieveByLike(string key, string vals)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(key, " LIKE ", vals);
            return qo.DoQuery();
        }

        /// <summary>
        /// 按照IDs查询并且排序
        /// 比如: FrmID  IN  '001','002' 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public int RetrieveInOrderBy(String key, String vals, String orderByKey)
        {
            QueryObject qo = new QueryObject(this);
            if (vals.Contains("(") == false)
                qo.AddWhere(key, " IN ", "(" + vals + ")");
            else
                qo.AddWhere(key, " IN ", vals);

            return qo.DoQuery();

            if (DataType.IsNullOrEmpty(orderByKey) == false)
                qo.addOrderBy(orderByKey);

            return qo.DoQuery();
        }
        /// <summary>
        /// 按照IDs查询
        /// 比如: FrmID  IN  '001','002' 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public virtual int RetrieveIn(string key, string vals, string orderby = null)
        {
            QueryObject qo = new QueryObject(this);

            if (vals.Contains("(") == false)
                qo.AddWhere(key, " IN ", "(" + vals + ")");
            else
                qo.AddWhere(key, " IN ", vals);

            if (orderby != null)
                qo.addOrderBy(orderby);

            return qo.DoQuery();
        }

        public virtual int RetrieveInSQL(string attr, string sql)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(attr, sql);
            return qo.DoQuery();
        }
        public virtual int RetrieveInSQL(string attr, string sql, string orderBy)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(attr, sql);
            qo.addOrderBy(orderBy);
            return qo.DoQuery();
        }

        public virtual int RetrieveInSQL(string sql)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(this.GetNewEntity.PK, sql);
            return qo.DoQuery();
        }
        public virtual int Retrieve(string orderby)
        {
            QueryObject qo = new QueryObject(this);
            qo.addOrderBy(orderby);
            return qo.DoQuery();
        }

        public virtual int Retrieve(string key, object val, string orderby = null)
        {
            QueryObject qo = new QueryObject(this);

            if (BP.Difference.SystemConfig.AppCenterDBFieldIsParaDBType == true)
                qo.AddWhere(key, BP.Sys.Base.Glo.GenerRealType(this.GetNewEntity.EnMap.Attrs, key, val));
            else
                qo.AddWhere(key, val);

            if (orderby != null)
                qo.addOrderBy(orderby); //这个排序方式不要变化，否则会影响其他的地方。
            return qo.DoQuery();
        }
        public virtual int Retrieve(string key, object val, string key2, object val2, string ordery = null)
        {
            QueryObject qo = new QueryObject(this);

            //是否需要参数类型
            if (BP.Difference.SystemConfig.AppCenterDBFieldIsParaDBType == true)
            {
                qo.AddWhere(key, BP.Sys.Base.Glo.GenerRealType(this.GetNewEntity.EnMap.Attrs, key, val));
                qo.addAnd();
                qo.AddWhere(key2, BP.Sys.Base.Glo.GenerRealType(this.GetNewEntity.EnMap.Attrs, key2, val2));
            }
            else
            {
                qo.AddWhere(key, val);
                qo.addAnd();
                qo.AddWhere(key2, val2);
            }
            if (ordery != null)
                qo.addOrderBy(ordery);
            return qo.DoQuery();
        }

        public int Retrieve(string key, object val, string key2, object val2, string key3, object val3, string key4, object val4, string orderBy = null)
        {
            QueryObject qo = new QueryObject(this);

            if (BP.Difference.SystemConfig.AppCenterDBFieldIsParaDBType == true)
            {
                qo.AddWhere(key, BP.Sys.Base.Glo.GenerRealType(this.GetNewEntity.EnMap.Attrs, key, val));
                qo.addAnd();
                qo.AddWhere(key2, BP.Sys.Base.Glo.GenerRealType(this.GetNewEntity.EnMap.Attrs, key2, val2));
                qo.addAnd();
                qo.AddWhere(key3, BP.Sys.Base.Glo.GenerRealType(this.GetNewEntity.EnMap.Attrs, key3, val3));
                qo.addAnd();
                qo.AddWhere(key4, BP.Sys.Base.Glo.GenerRealType(this.GetNewEntity.EnMap.Attrs, key4, val4));
            }
            else
            {
                qo.AddWhere(key, val);
                qo.addAnd();
                qo.AddWhere(key2, val2);
                qo.addAnd();
                qo.AddWhere(key3, val3);
                qo.addAnd();
                qo.AddWhere(key4, val4);
            }

            if (orderBy != null)
                qo.addOrderBy(orderBy);
            return qo.DoQuery();
        }
        public int Retrieve(string key, object val, string key2, object val2, string key3, object val3, string orderBy = null)
        {
            QueryObject qo = new QueryObject(this);

            if (BP.Difference.SystemConfig.AppCenterDBFieldIsParaDBType == true)
            {
                qo.AddWhere(key, BP.Sys.Base.Glo.GenerRealType(this.GetNewEntity.EnMap.Attrs, key, val));
                qo.addAnd();
                qo.AddWhere(key2, BP.Sys.Base.Glo.GenerRealType(this.GetNewEntity.EnMap.Attrs, key2, val2));
                qo.addAnd();
                qo.AddWhere(key3, BP.Sys.Base.Glo.GenerRealType(this.GetNewEntity.EnMap.Attrs, key3, val3));
            }
            else
            {
                qo.AddWhere(key, val);
                qo.addAnd();
                qo.AddWhere(key2, val2);
                qo.addAnd();
                qo.AddWhere(key3, val3);
            }
            if (orderBy != null)
                qo.addOrderBy(orderBy);
            return qo.DoQuery();
        }
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public virtual int RetrieveAll()
        {
            return this.RetrieveAll(null);
        }
        public virtual int RetrieveAllOrderByRandom()
        {
            QueryObject qo = new QueryObject(this);
            qo.addOrderByRandom();
            return qo.DoQuery();
        }
        public virtual int RetrieveAllOrderByRandom(int topNum)
        {
            QueryObject qo = new QueryObject(this);
            qo.Top = topNum;
            qo.addOrderByRandom();
            return qo.DoQuery();
        }
        public virtual int RetrieveAll(int topNum, string orderby)
        {
            QueryObject qo = new QueryObject(this);
            qo.Top = topNum;
            qo.addOrderBy(orderby);
            return qo.DoQuery();
        }
        public virtual int RetrieveAll(int topNum, string orderby, bool isDesc)
        {
            QueryObject qo = new QueryObject(this);
            qo.Top = topNum;
            if (isDesc)
                qo.addOrderByDesc(orderby);
            else
                qo.addOrderBy(orderby);
            return qo.DoQuery();
        }
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public virtual int RetrieveAll(string orderBy)
        {
            QueryObject qo = new QueryObject(this);
            if (orderBy != null)
                qo.addOrderBy(orderBy);
            return qo.DoQuery();
        }
        /// <summary>
        /// 查询全部。
        /// </summary>
        /// <returns></returns>
        public virtual int RetrieveAll(string orderBy1, string orderBy2)
        {
            QueryObject qo = new QueryObject(this);
            if (orderBy1 != null)
                qo.addOrderBy(orderBy1, orderBy2);
            return qo.DoQuery();
        }
        /// <summary>
        /// 按照最大个数查询。
        /// </summary>
        /// <param name="MaxNum">最大NUM</param>
        /// <returns></returns>
        public int RetrieveAll(int MaxNum)
        {
            QueryObject qo = new QueryObject(this);
            qo.Top = MaxNum;
            return qo.DoQuery();
        }
        /// <summary>
        /// 查询全部的结果放到DataTable。
        /// </summary>
        /// <returns></returns>
        public DataTable RetrieveAllToTable()
        {
            QueryObject qo = new QueryObject(this);
            return qo.DoQueryToTable();
        }
        private DataTable DealBoolTypeInDataTable(Entity en, DataTable dt)
        {

            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyDataType == DataType.AppBoolean)
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = "tmp" + attr.Key;
                    col.DataType = typeof(bool);
                    dt.Columns.Add(col);
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[attr.Key].ToString() == "1")
                            dr["tmp" + attr.Key] = true;
                        else
                            dr["tmp" + attr.Key] = false;
                    }
                    dt.Columns.Remove(attr.Key);
                    dt.Columns["tmp" + attr.Key].ColumnName = attr.Key;
                    continue;
                }
                if (attr.MyDataType == DataType.AppDateTime || attr.MyDataType == DataType.AppDate)
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = "tmp" + attr.Key;
                    col.DataType = typeof(DateTime);
                    dt.Columns.Add(col);
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            dr["tmp" + attr.Key] = DateTime.Parse(dr[attr.Key].ToString());
                        }
                        catch
                        {
                            if (attr.DefaultVal.ToString() == "")
                                dr["tmp" + attr.Key] = DateTime.Now;
                            else
                                dr["tmp" + attr.Key] = DateTime.Parse(attr.DefaultVal.ToString());

                        }

                    }
                    dt.Columns.Remove(attr.Key);
                    dt.Columns["tmp" + attr.Key].ColumnName = attr.Key;
                    continue;
                }
            }
            return dt;
        }
        /// <summary>
        /// ToJson.
        /// </summary>
        /// <returns></returns>
        public string ToJson(string dtName = "dt")
        {
            return BP.Tools.Json.ToJson(this.ToDataTableField(dtName));
        }
        public DataTable ToDataTableStringField(string tableName = "dt")
        {
            DataTable dt = this.ToEmptyTableStringField();
            Entity en = this.GetNewEntity;
            Attrs attrs = en.EnMap.Attrs;

            dt.TableName = tableName;
            foreach (Entity myen in this)
            {
                DataRow dr = dt.NewRow();
                foreach (Attr attr in attrs)
                {
                    if (attr.MyDataType == DataType.AppBoolean)
                    {
                        if (myen.GetValIntByKey(attr.Key) == 1)
                            dr[attr.Key] = "1";
                        else
                            dr[attr.Key] = "0";
                        continue;
                    }

                    /*如果是外键 就要去掉左右空格。
                     *  */
                    if (attr.MyFieldType == FieldType.FK
                        || attr.MyFieldType == FieldType.PKFK)
                        dr[attr.Key] = myen.GetValByKey(attr.Key).ToString().Trim();
                    else
                        dr[attr.Key] = myen.GetValByKey(attr.Key);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        /// <summary>
        /// 把当前实体集合的数据库转换成Table。
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable ToDataTableField(string tableName = "dt")
        {
            DataTable dt = this.ToEmptyTableField();

            Entity en = this.GetNewEntity;
            Attrs attrs = en.EnMap.Attrs;

            dt.TableName = tableName;

            for (int i = 0; i < this.Count; i++)
            {
                Entity myen = this[i];
                if (myen == null)
                    continue;

                DataRow dr = dt.NewRow();
                foreach (Attr attr in attrs)
                {
                    if (attr.MyDataType == DataType.AppBoolean)
                    {
                        if (myen.GetValIntByKey(attr.Key) == 1)
                            dr[attr.Key] = "1";
                        else
                            dr[attr.Key] = "0";
                        continue;
                    }

                    if (myen.Row.ContainsKey(attr.Key) == false)
                        continue;

                    var val = myen.Row[attr.Key];
                    // var val = myen myen.GetValByKey(attr.Key);
                    if (val == null)
                        continue;

                    /*如果是外键 就要去掉左右空格
                     *  */
                    if (attr.MyFieldType == FieldType.FK
                        || attr.MyFieldType == FieldType.PKFK)
                        dr[attr.Key] = val.ToString().Trim();
                    else
                    {
                        try
                        {
                            dr[attr.Key] = val;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("err@列[" + attr.Key + "]，设置值:" + val + "错误，也许是类型匹配");
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public DataTable ToDataTableDesc()
        {
            DataTable dt = this.ToEmptyTableDesc();
            Entity en = this.GetNewEntity;

            dt.TableName = en.EnMap.PhysicsTable;
            foreach (Entity myen in this)
            {
                DataRow dr = dt.NewRow();
                foreach (Attr attr in en.EnMap.Attrs)
                {

                    if (attr.MyDataType == DataType.AppBoolean)
                    {
                        if (myen.GetValBooleanByKey(attr.Key))
                            dr[attr.Desc] = "是";
                        else
                            dr[attr.Desc] = "否";
                        continue;
                    }
                    dr[attr.Desc] = myen.GetValByKey(attr.Key);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public DataTable ToEmptyTableDescField()
        {
            DataTable dt = new DataTable();
            Entity en = this.GetNewEntity;
            try
            {

                foreach (Attr attr in en.EnMap.Attrs)
                {

                    switch (attr.MyDataType)
                    {
                        case DataType.AppString:
                            dt.Columns.Add(new DataColumn(attr.Desc.Trim() + attr.Key, typeof(string)));
                            break;
                        case DataType.AppInt:
                            dt.Columns.Add(new DataColumn(attr.Desc.Trim() + attr.Key, typeof(int)));
                            break;
                        case DataType.AppFloat:
                            dt.Columns.Add(new DataColumn(attr.Desc.Trim() + attr.Key, typeof(float)));
                            break;
                        case DataType.AppBoolean:
                            dt.Columns.Add(new DataColumn(attr.Desc.Trim() + attr.Key, typeof(string)));
                            break;
                        case DataType.AppDouble:
                            dt.Columns.Add(new DataColumn(attr.Desc.Trim() + attr.Key, typeof(double)));
                            break;
                        case DataType.AppMoney:
                            dt.Columns.Add(new DataColumn(attr.Desc.Trim() + attr.Key, typeof(double)));
                            break;
                        case DataType.AppDate:
                            dt.Columns.Add(new DataColumn(attr.Desc.Trim() + attr.Key, typeof(string)));
                            break;
                        case DataType.AppDateTime:
                            dt.Columns.Add(new DataColumn(attr.Desc.Trim() + attr.Key, typeof(string)));
                            break;
                        default:
                            throw new Exception("@bulider insert sql error: 没有这个数据类型");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(en.EnDesc + " error " + ex.Message);

            }
            return dt;
        }
        public DataTable ToDataTableDescField(string tableName = "dt")
        {
            DataTable dt = this.ToEmptyTableDescField();
            dt.TableName = tableName;

            Entity en = this.GetNewEntity;

            dt.TableName = en.EnMap.PhysicsTable;
            foreach (Entity myen in this)
            {
                DataRow dr = dt.NewRow();
                foreach (Attr attr in en.EnMap.Attrs)
                {

                    if (attr.MyDataType == DataType.AppBoolean)
                    {
                        if (myen.GetValBooleanByKey(attr.Key))
                            dr[attr.Desc.Trim() + attr.Key] = "是";
                        else
                            dr[attr.Desc.Trim() + attr.Key] = "否";
                        continue;
                    }
                    dr[attr.Desc.Trim() + attr.Key] = myen.GetValByKey(attr.Key);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 空的Table
        /// 取到一个空表结构。
        /// </summary>
        /// <returns></returns>
        public DataTable ToEmptyTableField(Entity en = null)
        {
            DataTable dt = new DataTable();
            if (en == null)
                en = this.GetNewEntity;

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
                        dt.Columns.Add(new DataColumn(attr.Key, typeof(decimal)));
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
        public DataTable ToEmptyTableStringField()
        {
            DataTable dt = new DataTable();
            Entity en = this.GetNewEntity;
            dt.TableName = en.EnMap.PhysicsTable;
            foreach (Attr attr in en.EnMap.Attrs)
            {
                dt.Columns.Add(new DataColumn(attr.Key, typeof(string)));
            }
            return dt;
        }
        public DataTable ToEmptyTableDesc()
        {
            DataTable dt = new DataTable();
            Entity en = this.GetNewEntity;
            try
            {

                foreach (Attr attr in en.EnMap.Attrs)
                {
                    switch (attr.MyDataType)
                    {
                        case DataType.AppString:
                            dt.Columns.Add(new DataColumn(attr.Desc, typeof(string)));
                            break;
                        case DataType.AppInt:
                            dt.Columns.Add(new DataColumn(attr.Desc, typeof(int)));
                            break;
                        case DataType.AppFloat:
                            dt.Columns.Add(new DataColumn(attr.Desc, typeof(float)));
                            break;
                        case DataType.AppBoolean:
                            dt.Columns.Add(new DataColumn(attr.Desc, typeof(string)));
                            break;
                        case DataType.AppDouble:
                            dt.Columns.Add(new DataColumn(attr.Desc, typeof(double)));
                            break;
                        case DataType.AppMoney:
                            dt.Columns.Add(new DataColumn(attr.Desc, typeof(double)));
                            break;
                        case DataType.AppDate:
                            dt.Columns.Add(new DataColumn(attr.Desc, typeof(string)));
                            break;
                        case DataType.AppDateTime:
                            dt.Columns.Add(new DataColumn(attr.Desc, typeof(string)));
                            break;
                        default:
                            throw new Exception("@bulider insert sql error: 没有这个数据类型");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(en.EnDesc + " error " + ex.Message);

            }
            return dt;
        }
        #endregion
         
        #region 查询from cash
        /// <summary>
        /// 缓存查询: 根据 in sql 方式进行。
        /// </summary>
        /// <param name="cashKey">指定的缓存Key，全局变量不要重复。</param>
        /// <param name="inSQL">sql 语句</param>
        /// <returns>返回放在缓存里面的结果集合</returns>
        public int RetrieveFromCashInSQL(string cashKey, string inSQL)
        {
            this.Clear();
            Entities ens = Cash.GetEnsDataExt(cashKey) as Entities;
            if (ens == null)
            {
                QueryObject qo = new QueryObject(this);
                qo.AddWhereInSQL(this.GetNewEntity.PK, inSQL);
                qo.DoQuery();
                Cash.SetEnsDataExt(cashKey, this);
            }
            else
            {
                this.AddEntities(ens);
            }
            return this.Count;
        }
        /// <summary>
        /// 缓存查询: 根据相关的条件
        /// </summary>
        /// <param name="attrKey">属性: 比如 FK_Sort</param>
        /// <param name="val">值: 比如:01 </param>
        /// <param name="top">最大的取值信息</param>
        /// <param name="orderBy">排序字段</param>
        /// <param name="isDesc"></param>
        /// <returns>返回放在缓存里面的结果集合</returns>
        public int RetrieveFromCash(string attrKey, object val, int top, string orderBy, bool isDesc)
        {
            string cashKey = this.ToString() + attrKey + val + top + orderBy + isDesc;
            this.Clear();
            Entities ens = Cash.GetEnsDataExt(cashKey);
            if (ens == null)
            {
                QueryObject qo = new QueryObject(this);
                qo.Top = top;

                if (attrKey == "" || attrKey == null)
                {
                }
                else
                {
                    qo.AddWhere(attrKey, val);
                }

                if (orderBy != null)
                {
                    if (isDesc)
                        qo.addOrderByDesc(orderBy);
                    else
                        qo.addOrderBy(orderBy);
                }

                qo.DoQuery();
                Cash.SetEnsDataExt(cashKey, this);
            }
            else
            {
                this.AddEntities(ens);
            }
            return this.Count;
        }
        /// <summary>
        /// 缓存查询: 根据相关的条件
        /// </summary>
        /// <param name="attrKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public int RetrieveFromCash(string attrKey, object val)
        {
            return RetrieveFromCash(attrKey, val, 99999, null, true);
        }
        /// <summary>
        /// 缓存查询: 根据相关的条件
        /// </summary>
        /// <param name="attrKey"></param>
        /// <param name="val"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public int RetrieveFromCash(string attrKey, object val, string orderby)
        {
            return RetrieveFromCash(attrKey, val, 99999, orderby, true);
        }
        /// <summary>
        /// 缓存查询: 根据相关的条件
        /// </summary>
        /// <param name="top"></param>
        /// <param name="orderBy"></param>
        /// <param name="isDesc"></param>
        /// <returns></returns>
        public int RetrieveFromCash(string orderBy, bool isDesc, int top)
        {
            return RetrieveFromCash(null, null, top, orderBy, isDesc);
        }
        #endregion
         

        #region 类名属性.
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
        #endregion 类名属性.

    }
}
