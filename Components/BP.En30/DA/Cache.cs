using System;
using System.Collections;
using System.IO;
using System.Text;
using BP.En;
using BP.Pub;
using BP.Sys;
using BP.Web;
using BP.Difference;
using System.Collections.Generic;

namespace BP.DA
{

    /// <summary>
    /// Cache 的摘要说明。
    /// </summary>
    public class Cache
    {
        public static void ClearCache(string enName)
        {
            if (_BS_Cache != null)
            {
                if (_BS_Cache.ContainsKey(enName) == true)
                    _BS_Cache.Remove(enName);
            }

            if (_SQL_Cache != null)
            {
                if (_SQL_Cache.ContainsKey(enName) == true)
                    _SQL_Cache.Remove(enName);
            }


            if (_EnsData_Cache != null)
            {
                if (_EnsData_Cache.ContainsKey(enName) == true)
                    _EnsData_Cache.Remove(enName);
            }

            if (_Map_Cache != null)
            {
                if (_Map_Cache.ContainsKey(enName) == true)
                    _Map_Cache.Remove(enName);
            }

            if (_EnsData_Cache_Ext != null)
            {
                if (_EnsData_Cache_Ext.ContainsKey(enName) == true)
                    _EnsData_Cache_Ext.Remove(enName);
            }

            if (_Bill_Cache != null)
            {
                if (_Bill_Cache.ContainsKey(enName) == true)
                    _Bill_Cache.Remove(enName);
            }

            if (_Row_Cache != null)
            {
                if (_Row_Cache.ContainsKey(enName) == true)
                    _Row_Cache.Remove(enName);
            }

            //清除
          //  Cache2019.ClearCache();
        }
        /// <summary>
        /// 清空缓存.
        /// </summary>
        public static void ClearCache()
        {
            if (_BS_Cache != null)
                _BS_Cache.Clear();

            if (_SQL_Cache != null)
                _SQL_Cache.Clear();

            if (_EnsData_Cache != null)
                _EnsData_Cache.Clear();

            if (_Map_Cache != null)
                _Map_Cache.Clear();

            if (_EnsData_Cache_Ext != null)
                _EnsData_Cache_Ext.Clear();

            if (_Bill_Cache != null)
                _Bill_Cache.Clear();

            if (_Row_Cache != null)
                _Row_Cache.Clear();

            //清除
            Cache2019.ClearCache();
        }
        static Cache()
        {
            if (BP.Difference.SystemConfig.isBSsystem == false)
            {
                CS_Cache = new Hashtable();
            }
        }
        public static readonly Hashtable CS_Cache;

        #region Bill_Cache 单据模板Cache.
        private static Hashtable _Bill_Cache;
        public static Hashtable Bill_Cache
        {
            get
            {
                if (_Bill_Cache == null)
                    _Bill_Cache = new Hashtable();
                return _Bill_Cache;
            }
        }
        public static string GetBillStr(string cfile, bool isCheckCache)
        {
            cfile = cfile.Replace(".rtf.rtf", ".rtf");

            string val = Bill_Cache[cfile] as string;
            if (isCheckCache == true)
                val = null;


            if (val == null)
            {
                string file = null;
                if (cfile.Contains(":"))
                    file = cfile;
                else
                    file = BP.Difference.SystemConfig.PathOfDataUser + "CyclostyleFile/" + cfile;

                try
                {
                    StreamReader read = new StreamReader(file, System.Text.Encoding.ASCII); // 文件流.
                    val = read.ReadToEnd();  //读取完毕。
                    read.Close(); // 关闭。
                }
                catch (Exception ex)
                {
                    throw new Exception("@读取单据模板时出现错误。cfile=" + cfile + " @Ex=" + ex.Message);
                }
                _Bill_Cache[cfile] = val;
            }
            return val.Substring(0);
        }
        public static string[] GetBillParas(string cfile, string ensStrs, Entities ens)
        {
            string[] paras = Bill_Cache[cfile + "Para"] as string[];
            if (paras != null)
                return paras;

            Attrs attrs = new Attrs();
            foreach (Entity en in ens)
            {
                string perKey = en.ToString();

                Attrs enAttrs = en.EnMap.Attrs;
                foreach (Attr attr in enAttrs)
                {
                    Attr attrN = new Attr();
                    attrN.Key = perKey + "." + attr.Key;

                    if (attr.ItIsRefAttr)
                    {
                        attrN.Field = perKey + "." + attr.Key + "Text";
                    }
                    attrN.MyDataType = attr.MyDataType;
                    attrN.MyFieldType = attr.MyFieldType;
                    attrN.UIBindKey = attr.UIBindKey;
                    attrN.Field = attr.Field;
                    attrs.Add(attrN);
                }
            }

            paras = Cache.GetBillParas_Gener(cfile, attrs);
            _Bill_Cache[cfile + "Para"] = paras;
            return paras;
        }
        public static string[] GetBillParas(string cfile, string ensStrs, Entity en)
        {
            string[] paras = Bill_Cache[cfile + "Para"] as string[];
            if (paras != null)
                return paras;

            paras = Cache.GetBillParas_Gener(cfile, en.EnMap.Attrs);
            _Bill_Cache[cfile + "Para"] = paras;
            return paras;
        }
        public static string[] GetBillParas_Gener(string cfile, Attrs attrs)
        {
            cfile = cfile.Replace(".rtf.rtf", ".rtf");

            //  Attrs attrs = en.EnMap.Attrs;
            string[] paras = new string[300];
            string Billstr = Cache.GetBillStr(cfile, true);
            char[] chars = Billstr.ToCharArray();
            string para = "";
            int i = 0;
            bool haveError = false;
            string msg = "";
            foreach (char c in chars)
            {
                if (c == '>')
                {
                    #region 首先解决空格的问题.
                    string real = para.Clone().ToString();
                    if (attrs != null && real.Contains(" "))
                    {
                        real = real.Replace(" ", "");
                        Billstr = Billstr.Replace(para, real);
                        para = real;
                        haveError = true;
                    }
                    #endregion 首先解决空格的问题.

                    #region 解决特殊符号
                    if (attrs != null && real.Contains("/") && real.Contains("ND") == false)
                    {
                        haveError = true;
                        string findKey = null;
                        int keyLen = 0;
                        foreach (Attr attr in attrs)
                        {
                            if (real.Contains(attr.Key))
                            {
                                if (keyLen <= attr.Key.Length)
                                {
                                    keyLen = attr.Key.Length;
                                    findKey = attr.Key;
                                }
                            }
                        }

                        if (findKey == null)
                        {
                            msg += "@参数:<font color=red><b>[" + real + "]</b></font>可能拼写错误。";
                            continue;
                        }

                        if (real.Contains(findKey + ".NYR") == true)
                        {
                            real = findKey + ".NYR";
                        }
                        else if (real.Contains(findKey + ".RMB") == true)
                        {
                            real = findKey + ".RMB";
                        }
                        else if (real.Contains(findKey + ".RMBDX") == true)
                        {
                            real = findKey + ".RMBDX";
                        }
                        else if (real.Contains(findKey + ".Year") == true)
                        {
                            real = findKey + ".Year";
                        }
                        else if (real.Contains(findKey + ".Month") == true)
                        {
                            real = findKey + ".Month";
                        }
                        else if (real.Contains(findKey + ".Day") == true)
                        {
                            real = findKey + ".Day";
                        }
                        else
                        {
                            real = findKey;
                        }

                        Billstr = Billstr.Replace(para, real);
                        // msg += "@参数:<font color=red><b>[" + para + "]</b></font>不符合规范。";
                        //continue;
                    }
                    #endregion 首先解决空格的问题.

                    paras.SetValue(para, i);
                    i++;
                }

                if (c == '<')
                {
                    para = ""; // 如果遇到了 '<' 开始记录
                }
                else
                {
                    if (DataType.IsNullOrEmpty(c.ToString()))
                        continue;
                    para += c.ToString();
                }
            }


            if (msg != "")
            {
                string s = "@帮助信息:用记事本打开它模板,找到红色的字体. 把尖括号内部的非法字符去了,例如:《|f0|fs20RDT.NYR|lang1033|kerning2》，修改后事例：《RDT.NYR》。@注意把双引号代替单引号，竖线代替反斜线。";
                //throw new Exception("@单据模板（"+cfile+"）如下标记出现错误，系统无法修复它，需要您手工的删除标记或者用记事本打开查找到这写标记修复他们.@" + msg + s);
            }
            return paras;
        }
        #endregion

        #region Conn Cache
        private static Hashtable _Conn_Cache;
        public static Hashtable Conn_Cache
        {
            get
            {
                if (_Conn_Cache == null)
                    _Conn_Cache = new Hashtable();
                return _Conn_Cache;
            }
        }
        public static object GetConn(string fk_emp)
        {
            return Conn_Cache[fk_emp];
        }
        public static void SetConn(string fk_emp, object csh)
        {
            if (fk_emp == null)
                throw new Exception("fk_emp.  csh 参数有一个为空。");
            Conn_Cache[fk_emp] = csh;
        }
        #endregion

        #region BS_Cache
        private static String bsCacheKey = SystemConfig.RedisCacheKey("BSCache");
        private static Hashtable _BS_Cache = new Hashtable();
        public static Hashtable BS_Cache
        {
            get
            {
                if (SystemConfig.RedisIsEnable())
                {
                    List<Hashtable> list = HttpContextHelper.RedisUtils.GetAllHashValues<Hashtable>(bsCacheKey);
                    if (list == null || list.Count==0)
                        _BS_Cache = null;
                    else
                        _BS_Cache = list[0];
                }
                if (_BS_Cache == null)
                    _BS_Cache = new Hashtable();
                return _BS_Cache;
            }
        }
        #endregion

        #region SQL Cache
        private static String sqlCacheKey = SystemConfig.RedisCacheKey("SQLCache");
        private static Hashtable _SQL_Cache = new Hashtable();
        public static Hashtable SQL_Cache
        {
            get
            {
                if (SystemConfig.RedisIsEnable())
                {
                    List<Hashtable> list = HttpContextHelper.RedisUtils.GetAllHashValues<Hashtable>(sqlCacheKey);
                    if (list == null || list.Count == 0)
                        _SQL_Cache = null;
                    else
                        _SQL_Cache = list[0];
                }
                    
                if (_SQL_Cache == null)
                    _SQL_Cache = new Hashtable();
                return _SQL_Cache;
            }
        }
        public static SQLCache GetSQL(string clName)
        {
            return SQL_Cache[clName] as BP.En.SQLCache;
        }
        public static void SetSQL(string clName, BP.En.SQLCache csh)
        {
            if (clName == null || csh == null)
                throw new Exception("clName.  csh 参数有一个为空。");
            SQL_Cache[clName] = csh;
            if (SystemConfig.RedisIsEnable())
                HttpContextHelper.RedisUtils.SetInHash(sqlCacheKey, clName, csh);
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="clName"></param>
        public static void ClearSQL(string clName)
        {
            SQL_Cache.Remove(clName);
            if (SystemConfig.RedisIsEnable())
                HttpContextHelper.RedisUtils.RemoveHash(sqlCacheKey);
        }

        #endregion

        #region EnsData Cache
        private static String ensDataCacheKey = SystemConfig.RedisCacheKey("EnsDataCache");
        private static Hashtable _EnsData_Cache;
        public static Hashtable EnsData_Cache
        {
            get
            {
                if (SystemConfig.RedisIsEnable())
                {
                    List<Hashtable> list = HttpContextHelper.RedisUtils.GetAllHashValues<Hashtable>(ensDataCacheKey);
                    if (list == null || list.Count == 0)
                        _EnsData_Cache = null;
                    else
                        _EnsData_Cache = list[0];
                }
                if (_EnsData_Cache == null)
                    _EnsData_Cache = new Hashtable();
                return _EnsData_Cache;
            }
        }
        public static BP.En.Entities GetEnsData(string clName)
        {
            Entities ens = EnsData_Cache[clName] as BP.En.Entities;
            if (ens == null)
                return null;

            if (ens.Count == 0)
                return null;
            return ens;
        }
        public static void EnsDataSet(string clName, BP.En.Entities obj)
        {


            EnsData_Cache[clName] = obj;
            if (SystemConfig.RedisIsEnable())
                HttpContextHelper.RedisUtils.SetInHash(ensDataCacheKey, clName, obj);

        }
        public static void Remove(string clName)
        {
            EnsData_Cache.Remove(clName);
        }
        #endregion

        #region EnsData Cache 扩展 临时的Cache 文件。
        private static String ensCacheExtKey = SystemConfig.RedisCacheKey("EnsDataCacheExt");
        private static Hashtable _EnsData_Cache_Ext;
        public static Hashtable EnsData_Cache_Ext
        {
            get
            {
                if (SystemConfig.RedisIsEnable())
                {
                    List<Hashtable> list = HttpContextHelper.RedisUtils.GetAllHashValues<Hashtable>(ensCacheExtKey);
                    if (list == null || list.Count == 0)
                        _EnsData_Cache_Ext = null;
                    else
                        _EnsData_Cache_Ext = list[0];
                }
                if (_EnsData_Cache_Ext == null)
                    _EnsData_Cache_Ext = new Hashtable();
                return _EnsData_Cache_Ext;
            }
        }
        /// <summary>
        /// 为部分数据做的缓冲处理
        /// </summary>
        /// <param name="clName"></param>
        /// <returns></returns>
        public static BP.En.Entities GetEnsDataExt(string clName)
        {
            // 判断是否失效了。
            if (BP.Difference.SystemConfig.isTempCacheFail)
            {
                EnsData_Cache_Ext.Clear();
                return null;
            }

            try
            {
                BP.En.Entities ens;
                ens = EnsData_Cache_Ext[clName] as BP.En.Entities;
                return ens;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 为部分数据做的缓冲处理
        /// </summary>
        /// <param name="clName"></param>
        /// <param name="obj"></param>
        public static void SetEnsDataExt(string clName, BP.En.Entities obj)
        {
            if (clName == null || obj == null)
                throw new Exception("clName.  obj 参数有一个为空。");
            EnsData_Cache_Ext[clName] = obj;
            if (SystemConfig.RedisIsEnable())
                HttpContextHelper.RedisUtils.SetInHash(ensCacheExtKey, clName, obj);
        }
        #endregion

        #region TSmap Cache
        private static String mapCacheTSKey = SystemConfig.RedisCacheKey("MapCacheTS");
        private static Hashtable _Map_CacheTS;
        public static Hashtable Map_CacheTS
        {
            get
            {
                if (SystemConfig.RedisIsEnable())
                {
                    List<Hashtable> list = HttpContextHelper.RedisUtils.GetAllHashValues<Hashtable>(mapCacheTSKey);
                    if (list == null || list.Count == 0)
                        _Map_CacheTS = null;
                    else
                        _Map_CacheTS = list[0];
                }

                if (_Map_CacheTS == null)
                    _Map_CacheTS = new Hashtable();
                return _Map_CacheTS;
            }
        }
        public static BP.En.Map GetMapTS(string clName)
        {
            try
            {
                return Map_CacheTS[clName] as BP.En.Map;
            }
            catch
            {
                return null;
            }
        }
        public static void SetMapTS(string clName, BP.En.Map map)
        {
            if (clName == null)
                return;
            //    throw new Exception("clName.不能为空。");
            if (map == null)
            {
                Map_CacheTS.Remove(clName);
                return;
            }
            Map_CacheTS[clName] = map;
            if (SystemConfig.RedisIsEnable())
                HttpContextHelper.RedisUtils.SetInHash(mapCacheTSKey, clName, map);
        }
        /// <summary>
        /// 是否存map.
        /// </summary>
        /// <param name="clName"></param>
        /// <returns></returns>
        public static bool IsExitMapTS(string clName)
        {
            if (clName == null)
                throw new Exception("clName.不能为空。");

            return Map_CacheTS.ContainsKey(clName);
        }
        #endregion


        #region map Cache
        private static String mapCacheKey = SystemConfig.RedisCacheKey("MapCache");
        private static Hashtable _Map_Cache;
        public static Hashtable Map_Cache
        {
            get
            {
                if (SystemConfig.RedisIsEnable())
                {
                    List<Hashtable> list = HttpContextHelper.RedisUtils.GetAllHashValues<Hashtable>(mapCacheKey);
                    if (list == null || list.Count == 0)
                        _Map_Cache = null;
                    else
                        _Map_Cache = list[0];
                }
                if (_Map_Cache == null)
                    _Map_Cache = new Hashtable();
                return _Map_Cache;
            }
        }
        public static BP.En.Map GetMap(string clName)
        {
            try
            {
                return Map_Cache[clName] as BP.En.Map;
            }
            catch
            {
                return null;
            }
        }
        public static void SetMap(string clName, BP.En.Map map)
        {
            if (clName == null)
                return;
            //    throw new Exception("clName.不能为空。");
            if (map == null)
            {
                Map_Cache.Remove(clName);
                return;
            }
            Map_Cache[clName] = map;
            if (SystemConfig.RedisIsEnable())
                HttpContextHelper.RedisUtils.SetInHash(mapCacheKey, clName, map);
        }
        /// <summary>
        /// 是否存map.
        /// </summary>
        /// <param name="clName"></param>
        /// <returns></returns>
        public static bool IsExitMap(string clName)
        {
            if (clName == null)
                throw new Exception("clName.不能为空。");

            return Map_Cache.ContainsKey(clName);
        }
        #endregion

        #region row Cache
        private static Hashtable _Row_Cache;
        public static Hashtable Row_Cache
        {
            get
            {
                if (SystemConfig.RedisIsEnable())
                {
                    List<Hashtable> list = HttpContextHelper.RedisUtils.GetAllHashValues<Hashtable>(mapCacheKey);
                    if (list == null || list.Count == 0)
                        _Map_Cache = null;
                    else
                        _Map_Cache = list[0];
                }

                if (_Row_Cache == null)
                    _Row_Cache = new Hashtable();
                return _Row_Cache;
            }
        }
        public static BP.En.Row GetRow(string clName)
        {
            BP.En.Row row = Row_Cache[clName] as BP.En.Row;
            if (row == null)
                return null;

            return (BP.En.Row)row.Clone();
        }
        public static void SetRow(string clName, BP.En.Row map)
        {
            if (clName == null)
                return;
            //    throw new Exception("clName.不能为空。");
            if (map == null)
            {
                Row_Cache.Remove(clName);
                return;
            }
            Row_Cache[clName] = map;
        }
        /// <summary>
        /// 是否存map.
        /// </summary>
        /// <param name="clName"></param>
        /// <returns></returns>
        public static bool IsExitRow(string clName)
        {
            if (clName == null)
                throw new Exception("clName.不能为空。");

            return Row_Cache.ContainsKey(clName);
        }
        #endregion

        #region 取出对象
        /// <summary>
        /// 从 Cache 里面取出对象.
        /// </summary>
        public static object GetObj(string key, Depositary where)
        {

#if DEBUG
            if (where == Depositary.None)
                throw new Exception("您没有把[" + key + "]放到session or application 里面不能找出他们.");
#endif

            if (BP.Difference.SystemConfig.isBSsystem)
            {
                if (where == Depositary.Application)
                    // return  System.Web.HttpContext.Current.Cache[key];
                    return BS_Cache[key]; //  System.Web.HttpContext.Current.Cache[key];
                else
                    return HttpContextHelper.SessionGet(key);
            }
            else
            {
                return CS_Cache[key];
            }
        }
        public static object GetObj(string key)
        {
            if (BP.Difference.SystemConfig.isBSsystem)
            {
                object obj = BS_Cache[key]; // Cache.GetObjFormApplication(key, null);
                if (obj == null)
                    obj = Cache.GetObjFormSession(key);
                return obj;
            }
            else
            {
                return CS_Cache[key];
            }
        }
        /// <summary>
        /// 删除 like 名称的缓存对象。
        /// </summary>
        /// <param name="likeKey"></param>
        /// <returns></returns>
        public static int DelObjFormApplication(string likeKey)
        {
            int i = 0;
            if (BP.Difference.SystemConfig.isBSsystem)
            {
                string willDelKeys = "";
                foreach (string key in BS_Cache.Keys)
                {
                    if (key.Contains(likeKey) == false)
                        continue;
                    willDelKeys += "@" + key;
                }

                string[] strs = willDelKeys.Split('@');
                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s) == true)
                        continue;
                    BS_Cache.Remove(s);
                    i++;
                }
            }
            else
            {
                string willDelKeys = "";
                foreach (string key in CS_Cache.Keys)
                {
                    if (key.Contains(likeKey) == false)
                        continue;
                    willDelKeys += "@" + key;
                }

                string[] strs = willDelKeys.Split('@');
                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s) == true)
                        continue;
                    CS_Cache.Remove(s);
                    i++;
                }
            }

            return i;
        }
        public static object GetObjFormApplication(string key, object isNullAsVal)
        {
            if (BP.Difference.SystemConfig.isBSsystem)
            {
                object obj = BS_Cache[key]; // System.Web.HttpContext.Current.Cache[key];
                if (obj == null)
                    return isNullAsVal;
                else
                    return obj;
            }
            else
            {
                object obj = CS_Cache[key];
                if (obj == null)
                    return isNullAsVal;
                else
                    return obj;
            }
        }
        public static object GetObjFormSession(string key)
        {
            if (BP.Difference.SystemConfig.isBSsystem)
            {
                try
                {
                    return HttpContextHelper.SessionGet(key);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return CS_Cache[key];
            }
        }
        #endregion

        #region Remove Obj
        /// <summary>
        /// RemoveObj
        /// </summary>
        /// <param name="key"></param>
        /// <param name="where"></param>
        public static void RemoveObj(string key, Depositary where)
        {
            if (Cache.IsExits(key, where) == false)
                return;

            if (BP.Difference.SystemConfig.isBSsystem)
            {
                if (where == Depositary.Application)
                    CacheHelper.Remove(key);
                else
                    HttpContextHelper.Session.Remove(key);
            }
            else
            {
                CS_Cache.Remove(key);
            }
        }
        #endregion

        #region 放入对象
        public static void RemoveObj(string key)
        {
            BS_Cache.Remove(key);
        }
        public static void AddObj(string key, Depositary where, object obj)
        {
            if (key == null)
                throw new Exception("您需要为obj=" + obj.ToString() + ",设置为主键值。key");

            if (obj == null)
                throw new Exception("您需要为obj=null  设置为主键值。key=" + key);

#if DEBUG
            if (where == Depositary.None)
                throw new Exception("您没有把[" + key + "]放到 session or application 里面设置他们.");
#endif
            //if (Cache.IsExits(key, where))
            //    return;

            if (BP.Difference.SystemConfig.isBSsystem)
            {
                if (where == Depositary.Application)
                {
                    BS_Cache[key] = obj;
                }
                else
                {
                    HttpContextHelper.SessionSet(key, obj);
                }
            }
            else
            {
                if (CS_Cache.ContainsKey(key))
                    CS_Cache[key] = obj;
                else
                    CS_Cache.Add(key, obj);
            }
        }
        #endregion

        #region 判断对象是不是存在
        /// <summary>
        /// 判断对象是不是存在
        /// </summary>
        public static bool IsExits(string key, Depositary where)
        {
            if (BP.Difference.SystemConfig.isBSsystem)
            {
                if (where == Depositary.Application)
                {
                    return CacheHelper.Contains(key);
                }
                else
                {
                    return HttpContextHelper.SessionGet(key) != null;
                }
            }
            else
            {
                return CS_Cache.ContainsKey(key);
            }
        }
        #endregion

    }

    public class CacheEntity
    {
        #region Hashtable 属性
        private static String dCacheKey = SystemConfig.RedisCacheKey("DCache");
        private static Hashtable _Cache=new Hashtable();
        public static Hashtable DCache
        {
            get
            {
                if (SystemConfig.RedisIsEnable())
                {
                    List<Hashtable> list = HttpContextHelper.RedisUtils.GetAllHashValues<Hashtable>(dCacheKey);
                    if (list == null || list.Count == 0)
                        _Cache = null;
                    else
                        _Cache = list[0];
                }
                

                if (_Cache == null)
                    _Cache = new Hashtable();
                return _Cache;
            }
        }
        #endregion

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="enName"></param>
        /// <param name="key"></param>
        /// <param name="en"></param>
        public static void Update(string enName, string key, Entity en)
        {
            Hashtable ht = CacheEntity.DCache[enName] as Hashtable;
            if (ht == null)
            {
                ht = new Hashtable();
                CacheEntity.DCache[enName] = ht;
            }
            ht[key] = en;

            //清除集合.
            CacheEntity.DCache.Remove(enName + "Ens");
        }
        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="enName">实体Name</param>
        /// <param name="pkVal">主键值</param>
        /// <returns>返回这个实体</returns>
        public static Entity Select(string enName, string pkVal)
        {
            Hashtable ht = CacheEntity.DCache[enName] as Hashtable;
            if (ht == null)
                return null;

            return ht[pkVal] as Entity;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="enName"></param>
        /// <param name="pkVal"></param>
        public static void Delete(string enName, string pkVal)
        {
            Hashtable ht = CacheEntity.DCache[enName] as Hashtable;
            if (ht == null)
                return;

            ht.Remove(pkVal);
            //清除集合.
            CacheEntity.DCache.Remove(enName + "Ens");
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="enName"></param>
        /// <param name="en"></param>
        /// <param name="pkVal"></param>
        public static void Insert(string enName, string pkVal, Entity en)
        {
            Hashtable ht = CacheEntity.DCache[enName] as Hashtable;
            if (ht == null)
                return;

            //edited by liuxc,2014-8-21 17:21
            if (ht.ContainsKey(pkVal))
                ht[pkVal] = en;
            else
                ht.Add(pkVal, en);

            //清除集合.
            CacheEntity.DCache.Remove(enName + "Ens");
        }
    }
}
