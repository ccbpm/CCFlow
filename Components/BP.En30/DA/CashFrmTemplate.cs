using System;
using System.Data;
using System.Collections;
using System.Text;
using BP.En;
using BP.Pub;
using BP.Sys;

namespace BP.DA
{
    public class CashFrmTemplate
    {
        #region 缓存ht
        private static Hashtable _hts;
        #endregion

        #region 对实体的操作.
        /// <summary>
        /// 放入表单
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="ds">表单模版</param>
        public static void Put(string frmID,  DataSet ds)
        {
            string json = BP.Tools.Json.ToJson(ds);

            lock (lockObj)
            {
                if (_hts == null)
                    _hts = new Hashtable();

                if (_hts.ContainsKey(frmID) == false)
                    _hts.Add(frmID, json);
                else
                    _hts[frmID] = json;
            }
        }
       
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="frmID">表单ID</param>
        public static void Remove(string frmID)
        {
            lock (lockObj)
            {
                if (_hts == null)
                    _hts = new Hashtable();

                _hts.Remove(frmID);
            }
        }
        private static object lockObj = new object();
        /// <summary>
        /// 获得表单DataSet模式的模版数据
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <returns>表单模版</returns>
        public static DataSet GetFrmDataSetModel(string frmID)
        {
            lock (lockObj)
            {
                if (_hts == null)
                    _hts = new Hashtable();

                if (_hts.ContainsKey(frmID) == true)
                {
                    string json = _hts[frmID] as string;
                    DataSet ds = BP.Tools.Json.ToDataSet(json);
                    return ds;
                }
                return null;
            }
        }
        /// <summary>
        /// 获得表单json模式的模版数据
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <returns>json</returns>
        public static string GetFrmJsonModel(string frmID)
        {
            lock (lockObj)
            {
                if (_hts == null)
                    _hts = new Hashtable();

                if (_hts.ContainsKey(frmID) == true)
                {
                    string json = _hts[frmID] as string;
                    return json;
                }
                return null;
            }
        }
        #endregion 对实体的操作.
 
    }
}
