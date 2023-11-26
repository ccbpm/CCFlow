using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace BP.En
{
    /// <summary>
    /// 缓存SQL
    /// </summary>
    public class SQLCash
    {
        public string EnName = null;
        public string Insert = null;
        public string Update = null;
        public string Delete = null;
        public string Select = null;
        public SQLCash()
        { 

        }
        public SQLCash(Entity en)
        {
            this.EnName = en.ToString();
            this.Insert = SqlBuilder.InsertForPara(en);
            this.Update = SqlBuilder.UpdateForPara(en, null);
            this.Delete = SqlBuilder.DeleteForPara(en);
            this.Select = SqlBuilder.RetrieveForPara(en);
        }
        /// <summary>
        /// 获取指定的key, 返回更新的语句。
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string GetUpdateSQL(Entity en, string[] keys)
        {
            if (keys == null)
                return this.Update;

            string mykey = "";
            foreach (string k in keys)
                mykey += k;

            string sql = this.UpdateSQLs[mykey] as string;
            if (sql == null)
                UpdateSQLs[mykey] = SqlBuilder.UpdateForPara(en, keys);

            sql = UpdateSQLs[mykey] as string;

            if (sql == null)
                throw new Exception("@error");

            return sql;
        }

        #region UpdateSQLs
        private   Hashtable _UpdateSQLs;
        public Hashtable UpdateSQLs
        {
            get
            {
                if (_UpdateSQLs == null)
                    _UpdateSQLs = new Hashtable();
                return _UpdateSQLs;
            }
        }
        #endregion
    }
}
