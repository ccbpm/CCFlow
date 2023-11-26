using System;
using System.Collections;
using System.Text;
using BP.En;
using BP.Pub;

namespace BP.DA
{
    /// <summary>
    /// 实体缓存
    /// </summary>
    public class Cash2019
    {
        #region 缓存的操作.
        /// <summary>
        /// 清除所有的实体缓存.
        /// </summary>
        public static void ClearCash()
        {
            _hts = null;
        }
        /// <summary>
        /// 清除指定的缓存数据.
        /// </summary>
        /// <param name="enName">实体名称</param>
        public static void ClearCashSpecEnName(string enName)
        {
            Hashtable ht = hts[enName] as Hashtable;
            if (ht != null)
            {
                hts.Remove(enName);
            }
        }
        #endregion 缓存的操作.

        #region 缓存ht
        private static Hashtable _hts;
        public static Hashtable hts
        {
            get
            {
                if (_hts == null)
                    _hts = new Hashtable();
                return _hts;
            }
        }
        #endregion

        #region 对实体的操作.
        /// <summary>
        /// 把实体放入缓存里面
        /// </summary>
        /// <param name="enName"></param>
        /// <param name="ens"></param>
        /// <param name="enPK"></param>
        public static void PutRow(string enName, string pkVal, Row row)
        {
            lock (lockObj)
            {
                Hashtable ht = hts[enName] as Hashtable;
                if (ht == null)
                {
                    ht = new Hashtable();
                    hts.Add(enName, ht);
                }
                if (ht.ContainsKey(pkVal) == true)
                    ht[pkVal] = row;
                else
                    ht.Add(pkVal, row);
            }
        }
        public static void UpdateRow(string enName, string pkVal, Row row)
        {
            lock (lockObj)
            {
                Hashtable ht = hts[enName] as Hashtable;
                if (ht == null)
                {
                    ht = new Hashtable();
                    hts.Add(enName, ht);
                }
                ht[pkVal] = row;
            }
        }
        public static void DeleteRow(string enName, string pkVal)
        {
            lock (lockObj)
            {
                Hashtable ht = hts[enName] as Hashtable;
                if (ht == null)
                {
                    ht = new Hashtable();
                    hts.Add(enName, ht);
                }
                ht.Remove(pkVal.ToString());
            }
        }
        private static object lockObj = new object();
        /// <summary>
        /// 获得实体类
        /// </summary>
        /// <param name="enName">实体名字</param>
        /// <param name="pkVal">键</param>
        /// <returns>row</returns>
        public static Row GetRow(string enName, string pkVal)
        {
            lock (lockObj)
            {
                Hashtable ht = hts[enName] as Hashtable;
                if (ht == null)
                    return null;
                if (ht.ContainsKey(pkVal) == true)
                    return ht[pkVal] as Row;
                return null;
            }
        }
        #endregion 对实体的操作.

        #region 对实体的集合操作.
        /// <summary>
        /// 把集合放入缓存.
        /// </summary>
        /// <param name="ensName">集合实体类名</param>
        /// <param name="ens">实体集合</param>
        public static void PutEns(string ensName, Entities ens)
        {

        }
        /// <summary>
        /// 获取实体集合类
        /// </summary>
        /// <param name="ensName">集合类名</param>
        /// <param name="pkVal">主键</param>
        /// <returns>实体集合</returns>
        public static Entities GetEns(string ensName, object pkVal)
        {
            return null;
        }
        #endregion 对实体的集合操作.

    }
}
