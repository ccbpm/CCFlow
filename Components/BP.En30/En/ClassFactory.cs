
using System;
using System.Collections;
using System.ComponentModel;
using System.Data; 
using System.Web;
using System.Reflection;
using System.IO;
using BP.DA;
using BP.En;   
using BP.Sys;
using BP.Pub;
using BP.Sys.XML;

 
namespace BP.En
{ 
	/// <summary>
	/// ClassFactory 的摘要说明。
	/// </summary>
    public class ClassFactory 
    {
        #region public moen
        /// <summary>
        /// 装载xml配置文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="tableName">物理表名</param>
        /// <param name="key">key</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public static bool LoadConfigXml(string path, string tableName, string key, string val)
        {
            try
            {
                BP.Sys.SystemConfig.CS_AppSettings.Clear();
            }
            catch
            {
            }

            DataSet ds = new DataSet();
            ds.ReadXml(path);

            DataTable dt = ds.Tables[tableName];
          //  BP.Sys.SystemConfig.CS_AppSettings = new System.Collections.Specialized.NameValueCollection();
            BP.Sys.SystemConfig.CS_DBConnctionDic.Clear();
            foreach (DataRow row in dt.Rows)
            {
                BP.Sys.SystemConfig.CS_AppSettings.Add(row[key].ToString().Trim(), row[val].ToString().Trim());
            }
            ds.Dispose();
            BP.Sys.SystemConfig.IsBSsystem_Test = false;
            BP.Sys.SystemConfig.IsBSsystem = false;
            return true;
        }
        /// <summary>
        /// 加载web.config配置文件
        /// </summary>
        /// <param name="cfgFile">web.config配置文件路径</param>
        /// <returns>true or false</returns>
        public static bool LoadConfig(string cfgFile)
        {
            try
            {
                BP.Sys.SystemConfig.CS_AppSettings.Clear();
            }
            catch
            {
            }

            SystemConfig.IsBSsystem = false;

            #region 加载 Web.Config 文件配置
            if (!File.Exists(cfgFile))
                throw new Exception("找不到配置文件[" + cfgFile + "]2");

            DataSet ds = new DataSet();
            ds.ReadXml(cfgFile);

            StreamReader read = new StreamReader(cfgFile);
            string firstline = read.ReadLine();
            string cfg = read.ReadToEnd();
            read.Close();

            int start = cfg.ToLower().IndexOf("<appsettings>");
            int end = cfg.ToLower().IndexOf("</appsettings>");

            cfg = cfg.Substring(start, end - start + "</appsettings".Length + 1);

            cfgFile = "__$AppConfig.cfg";
            StreamWriter write = new StreamWriter(cfgFile);
            write.WriteLine(firstline);
            write.Write(cfg);
            write.Flush();
            write.Close();

            DataSet dscfg = new DataSet("cfg");
            try
            {
                dscfg.ReadXml(cfgFile);
            }
            catch (Exception ex)
            {
                throw new Exception("加载配置文件[" + cfgFile + "]失败！\n" + ex.Message + "启动失败！");
            }

            //   BP.Sys.SystemConfig.CS_AppSettings = new System.Collections.Specialized.NameValueCollection();

            BP.Sys.SystemConfig.CS_DBConnctionDic.Clear();
            DataTable dt = dscfg.Tables["add"];
            foreach (DataRow dr in dt.Rows)
            {
                string key = dr["key"] as string;
                if (key == null || key == "")
                    continue;

                string value = dr["value"] as string;
                if (value == null || value == "")
                    continue;

                BP.Sys.SystemConfig.CS_AppSettings.Add(key, value);
            }
            dscfg.Dispose();

            // 增加特殊判断。
            SystemConfig.AppCenterDSN = SystemConfig.AppCenterDSN.Replace("VisualFlowDesigner", "VisualFlow");
            #endregion
            return true;
        }
        #endregion

        #region 与报表有关系的

        /// <summary>
        /// 设置对象实例上指定属性的值
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="propertyName">属性名，属性为非静态特性</param>
        /// <param name="val">值</param>
        public static void SetValue(object obj, string propertyName, object val)
        {
            Type tp = obj.GetType();
            PropertyInfo p = tp.GetProperty(propertyName);
            if (p == null)
                throw new Exception("设置属性值失败！类型[" + tp + "]没有属性[" + propertyName + "]");
            p.SetValue(obj, val, null);
        }
        /// <summary>
        /// 获取对象实例上指定属性的值
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>值</returns>
        public static object GetValue(object obj, string propertyName)
        {
            Type tp = obj.GetType();
            PropertyInfo p = tp.GetProperty(propertyName);
            if (p == null)
                throw new Exception("获取属性值失败！类型[" + tp + "]没有属性[" + propertyName + "]");
            object val = p.GetValue(obj, null);
            return val;
        }
        /// <summary>
        /// 获取对象实例上指定属性的值，转换为string
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns>值</returns>
        public static string GetValueToStr(object obj, string propertyName)
        {
            object val = GetValue(obj, propertyName);
            if (val == null)
                return "";
            else
                return val.ToString();
        }
        #endregion

        #region 构造函数， 属性
        static ClassFactory()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(path + "bin\\"))
            {
                if (!DataType.IsNullOrEmpty(SystemConfig.AppSettings["CCFlowAppPath"]) && Directory.Exists(path + SystemConfig.AppSettings["CCFlowAppPath"] + "bin\\"))
                {
                    _BasePath = path + SystemConfig.AppSettings["CCFlowAppPath"] + "bin\\";
                }
                else
                {
                    _BasePath = path + "bin\\";
                }
            }
            else
            {
                _BasePath = path;
            }
        }
        private static string _BasePath = null;
        public static string BasePath
        {
            get
            {
                if (_BasePath == null)
                {
                    if (SystemConfig.AppSettings["InstallPath"] == null)
                        _BasePath = "D:\\";
                    else
                        _BasePath = SystemConfig.AppSettings["InstallPath"];
                }
                return _BasePath;
            }
        }
        #endregion 属性

        #region 程序集
        public static Assembly[] _BPAssemblies = null;
        /// <summary>
        /// 获取取程序集[dll]
        /// </summary>
        /// <returns></returns>
        public static Assembly[] BPAssemblies
        {
            get
            {
                if (_BPAssemblies == null)
                {
                    string[] fs = System.IO.Directory.GetFiles(BasePath, "BP.*.dll");
                    string[] fs1 = System.IO.Directory.GetFiles(BasePath, "*.ssss");

                    string strs = "";
                    foreach (string str in fs)
                    {
                        strs += str + ";";
                    }

                    foreach (string str in fs1)
                    {
                        strs += str + ";";
                    }

                    fs = strs.Split(';');
                    // 有多少个 不包含 .Web. 的ddl .
                    int fsCount = 0;
                    foreach (string s in fs)
                    {
                        if (s.Length == 0)
                            continue;

                        //if (s.IndexOf(".Web.") != -1)
                        //    continue;

                        fsCount++;
                    }

                    //把它们加入到 asss 里面去。
                    Assembly[] asss = new Assembly[fsCount];
                    int idx = 0;
                    int fsIndex = -1;
                    foreach (string s in fs)
                    {
                        fsIndex++;
                        //if (s.IndexOf(".Web.") != -1)
                        //    continue;

                        if (s.Length == 0)
                            continue;

                        asss[idx] = Assembly.LoadFrom(fs[fsIndex]);
                        idx++;
                    }
                    _BPAssemblies = asss;
                }
                return _BPAssemblies;
            }
        }

        public static Assembly[] BPAssemblies_Bak
        {
            get
            {
                if (_BPAssemblies == null)
                {
                    string[] fs = System.IO.Directory.GetFiles(BasePath, "BP.*.dll");
                    string[] fs1 = System.IO.Directory.GetFiles(BasePath, "*.ssss");

                    string strs = "";
                    foreach (string str in fs)
                    {
                        strs += str + ";";
                    }

                    foreach (string str in fs1)
                    {
                        strs += str + ";";
                    }

                    fs = strs.Split(';');
                    // 有多少个 不包含 .Web. 的ddl .
                    int fsCount = 0;
                    foreach (string s in fs)
                    {
                        if (s.Length == 0)
                            continue;

                        if (s.IndexOf(".Web.") != -1)
                            continue;

                        fsCount++;
                    }

                    //把它们加入到 asss 里面去。
                    Assembly[] asss = new Assembly[fsCount];
                    int idx = 0;
                    int fsIndex = -1;
                    foreach (string s in fs)
                    {
                        fsIndex++;
                        if (s.IndexOf(".Web.") != -1)
                            continue;

                        if (s.Length == 0)
                            continue;

                        asss[idx] = Assembly.LoadFrom(fs[fsIndex]);
                        idx++;
                    }
                    _BPAssemblies = asss;
                }
                return _BPAssemblies;
            }
        }

        /// <summary>
        /// 把class 放在内存中去
        /// </summary>
        public static void PutClassIntoCash()
        {
            Entity en = ClassFactory.GetEn("BP.Sys.FAQ");
            Entities ens = ClassFactory.GetEns("BP.Sys.FAQs");
        }
        #endregion 程序集

        #region 类型
        public static Type GetBPType(string className)
        {
            Type typ = null;
            foreach (Assembly ass in BPAssemblies)
            {
                typ = ass.GetType(className);
                if (typ != null)
                    return typ;
            }
            return typ;
        }

        public static ArrayList GetBPTypes(string baseEnsName)
        {
            ArrayList arr = new ArrayList();
            Type baseClass = null;
            foreach (Assembly ass in BPAssemblies)
            {
                if (baseClass == null)
                    baseClass = ass.GetType(baseEnsName);
                Type[] tps = ass.GetTypes();
                for (int i = 0; i < tps.Length; i++)
                {
                    if (tps[i].IsAbstract
                        || tps[i].BaseType == null
                        || !tps[i].IsClass
                        || !tps[i].IsPublic
                        )
                        continue;
                    Type tmp = tps[i].BaseType;

                    if (tmp.Namespace == null)
                        throw new Exception(tmp.FullName);

                    while (tmp != null && tmp.Namespace.IndexOf("BP") != -1)
                    {
                        if (tmp.FullName == baseEnsName)
                            arr.Add(tps[i]);
                        tmp = tmp.BaseType;
                    }
                }
            }
            if (baseClass == null)
            {
                throw new Exception("@找不到类型:" + baseEnsName + "！");
            }
            return arr;

        }

        public static bool IsFromType(string childTypeFullName, string parentTypeFullName)
        {
            foreach (Assembly ass in BPAssemblies)
            {
                Type childType = ass.GetType(childTypeFullName);
                while (childType != null && childType.BaseType != null)
                {
                    if (childType.BaseType.FullName == parentTypeFullName)
                        return true;
                    childType = childType.BaseType;
                }
            }
            return false;
        }
        #endregion 类型

        #region 对象实例
        /// <summary>
        /// 尽量不用此方法来获取事例
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static object GetObject_OK(string className)
        {
            if (className == "" || className == null)
                throw new Exception("@要转化类名称为空...");

            Type ty = null;
            object obj = null;
            foreach (Assembly ass in BPAssemblies)
            {
                ty = ass.GetType(className);
                if (ty == null)
                    continue;

                obj = ass.CreateInstance(className);
                if (obj != null)
                    return obj;
                else
                    throw new Exception("@创建对象实例 " + className + " 失败！");

            }
            if (obj == null)
                throw new Exception("@创建对象类型 " + className + " 失败，请确定拼写是否错误。");

            return obj;
        }
        /// <summary>
        /// 根据一个抽象的基类，取出此系统中从他上面继承的子类集合。
        /// 非抽象的类。
        /// </summary>
        /// <param name="baseEnsName">抽象的类名称</param>
        /// <returns>ArrayList</returns>
        public static ArrayList GetObjects(string baseEnsName)
        {
            ArrayList arr = new ArrayList();
            Type baseClass = null;
            foreach (Assembly ass in BPAssemblies)
            {
                if (baseClass == null)
                    baseClass = ass.GetType(baseEnsName);

                Type[] tps = null;
                try
                {
                    tps = ass.GetTypes();
                }
                catch
                {
                    //throw new Exception(ass.FullName+ass.Evidence.ToString()+ ex.Message);
                    continue;
                }

                for (int i = 0; i < tps.Length; i++)
                {
                    if (tps[i].IsAbstract
                        || tps[i].BaseType == null
                        || !tps[i].IsClass
                        || !tps[i].IsPublic
                        )
                        continue;

                    Type tmp = tps[i].BaseType;
                    if (tmp.Namespace == null)
                        throw new Exception(tmp.FullName);

                    while (tmp != null && tmp.Namespace.IndexOf("BP") != -1)
                    {
                        if (tmp.FullName == baseEnsName)
                            arr.Add(ass.CreateInstance(tps[i].FullName));
                        tmp = tmp.BaseType;
                    }
                }
            }
            if (baseClass == null)
            {
                throw new Exception("@找不到类型" + baseEnsName + "！");
            }
            return arr;
        }
        #endregion 实例

        #region 其他
        /// <summary>
        /// 生成系统报表
        /// </summary>
        /// <returns></returns>
        public static string SysReport()
        {
            return null;
        }

        #region 获取 en
        private static Hashtable Htable_En;
        /// <summary>
        /// 得到一个实体
        /// </summary>
        /// <param name="className">类名称</param>
        /// <returns>En</returns>
        public static Entity GetEn(string className)
        {
            //判断标记初始化实体.
            if (className.Contains(".") == false)
            {
                if (className.Contains("Dtl") == true)
                    return new GEDtl(className); //明细表.
                else
                    return new GEEntity(className); //表单实体.
            }

            if (Htable_En == null)
            {
                Htable_En = new Hashtable();
                string cl = "BP.En.EnObj";
                ArrayList al = ClassFactory.GetObjects(cl);
                foreach (Entity en in al)
                {
                    string key = string.Empty;
                    if (null == en || DataType.IsNullOrEmpty(key = en.ToString()))
                        continue;

                    if (Htable_En.ContainsKey(key) == false)
                        Htable_En.Add(key, en);
                    else
                    {
                        continue;
                        //Htable_En[key] = en;
                    }
                }
            }
            object tmp = Htable_En[className];
            return (tmp as Entity);
        }
        #endregion

        #region 获取 GetDataIOEn
        private static Hashtable Htable_DataIOEn;
        /// <summary>
        /// 得到一个实体
        /// </summary>
        /// <param name="className">类名称</param>
        /// <returns>En</returns>
        public static BP.DTS.DataIOEn GetDataIOEn(string className)
        {
            if (Htable_DataIOEn == null)
            {
                Htable_DataIOEn = new Hashtable();
                string cl = "BP.DTS.DataIOEn";
                ArrayList al = ClassFactory.GetObjects(cl);
                foreach (BP.DTS.DataIOEn en in al)
                    Htable_DataIOEn.Add(en.ToString(), en);
            }
            object tmp = Htable_DataIOEn[className];
            return (tmp as BP.DTS.DataIOEn);
        }
        #endregion

        #region 获取 GetMethod
        private static Hashtable Htable_Method;
        /// <summary>
        /// 得到一个实体
        /// </summary>
        /// <param name="className">类名称</param>
        /// <returns>En</returns>
        public static BP.En.Method GetMethod(string className)
        {
            if (Htable_Method == null)
            {
                Htable_Method = new Hashtable();
                string cl = "BP.En.Method";
                ArrayList al = ClassFactory.GetObjects(cl);
                foreach (BP.En.Method en in al)
                    Htable_Method.Add(en.ToString(), en);
            }
            object tmp = Htable_Method[className];
            return (tmp as BP.En.Method);
        }
        #endregion

        #region 获取 Entities
        public static Hashtable Htable_Ens;
        /// <summary>
        /// 得到一个实体
        /// </summary>
        /// <param name="className">类名称</param>
        /// <returns>En</returns>
        public static Entities GetEns(string className)
        {
            if (className.Contains(".") == false)
            {
                GEEntitys myens = new GEEntitys(className);
                return myens;
            }

            if (Htable_Ens == null || Htable_Ens.Count == 0)
            {
                Htable_Ens = new Hashtable();
                string cl = "BP.En.Entities";
                ArrayList al = ClassFactory.GetObjects(cl);

                Htable_Ens.Clear();
                foreach (Entities en in al)
                {
                    if (en.ToString() == null)
                        continue;

                    try
                    {
                        Htable_Ens.Add(en.ToString(), en);
                    }
                    catch
                    {
                    }
                }
            }
            Entities ens = Htable_Ens[className] as Entities;

#warning 会清除 cash 中的数据。
            return ens;
        }
        #endregion

        #region 获取 EventBase
        public static Hashtable Htable_Evbase;
        /// <summary>
        /// 得到一个事件实体
        /// </summary>
        /// <param name="className">类名称</param>
        /// <returns>BP.Sys.EventBase</returns>
        public static BP.Sys.EventBase GetEventBase(string className)
        {
            if (Htable_Evbase == null || Htable_Evbase.Count == 0)
            {
                Htable_Evbase = new Hashtable();
                string cl = "BP.Sys.EventBase";
                ArrayList al = ClassFactory.GetObjects(cl);
                Htable_Evbase.Clear();
                foreach (EventBase en in al)
                {
                    if (en.ToString() == null)
                        continue;
                    try
                    {
                        Htable_Evbase.Add(en.ToString(), en);
                    }
                    catch
                    {
                    }
                }
            }
            BP.Sys.EventBase ens = Htable_Evbase[className] as EventBase;
            return ens;
        }
        #endregion

        #region 获取 xmlEns
        public static Hashtable Htable_XmlEns;
        /// <summary>
        /// 得到一个实体
        /// </summary>
        /// <param name="className">类名称</param>
        /// <returns>En</returns>
        public static XmlEns GetXmlEns(string className)
        {
            if (Htable_XmlEns == null)
            {
                Htable_XmlEns = new Hashtable();
                string cl = "BP.Sys.XML.XmlEns";
                ArrayList al = ClassFactory.GetObjects(cl);
                foreach (XmlEns en in al)
                    Htable_XmlEns.Add(en.ToString(), en);
            }
            object tmp = Htable_XmlEns[className];
            return (tmp as XmlEns);
        }
        #endregion

        #region 获取 xmlen
        public static Hashtable Htable_XmlEn;
        /// <summary>
        /// 得到一个实体
        /// </summary>
        /// <param name="className">类名称</param>
        /// <returns>En</returns>
        public static XmlEn GetXmlEn(string className)
        {
            if (Htable_XmlEn == null)
            {
                Htable_XmlEn = new Hashtable();
                string cl = "BP.Sys.XML.XmlEn";
                ArrayList al = ClassFactory.GetObjects(cl);
                foreach (XmlEn en in al)
                    Htable_XmlEn.Add(en.ToString(), en);
            }
            object tmp = Htable_XmlEn[className];
            return (tmp as XmlEn);
        }
        #endregion

        #region 获取 Htable_Rpt2Base.
        /// <summary>
        /// 实体集合
        /// </summary>
        public static Hashtable Htable_Rpt2Base;
        /// <summary>
        /// 得到一个实体
        /// </summary>
        /// <param name="rpt2Base">类名称</param>
        /// <returns>BP.Rpt.Rpt2Base</returns>
        public static BP.Rpt.Rpt2Base GetRpt2Base(string rpt2Base)
        {
            if (Htable_Rpt2Base == null)
            {
                Htable_Rpt2Base = new Hashtable();
                string cl = "BP.Rpt.Rpt2Base";
                ArrayList al = ClassFactory.GetObjects(cl);
                foreach (BP.Rpt.Rpt2Base en in al)
                    Htable_Rpt2Base.Add(en.ToString(), en);
            }
            object tmp = Htable_Rpt2Base[rpt2Base];
            return (tmp as BP.Rpt.Rpt2Base);
        }
        #endregion

        #endregion


        #region 获取 HandlerBase
        private static Hashtable Htable_HandlerPage;
        /// <summary>
        /// 得到一个实体
        /// </summary>
        /// <param name="className">类名称</param>
        /// <returns>En</returns>
        public static object GetHandlerPage(string className)
        {
            if (Htable_HandlerPage == null)
            {
                Htable_HandlerPage = new Hashtable();
                string cl = "BP.WF.HttpHandler.DirectoryPageBase";
                ArrayList al = ClassFactory.GetObjects(cl);
                foreach (Object en in al)
                {
                    string key = string.Empty;
                    if (null == en || DataType.IsNullOrEmpty(key = en.ToString()))
                        continue;

                    if (Htable_HandlerPage.ContainsKey(key) == false)
                        Htable_HandlerPage.Add(key, en);

                }
            }
            return Htable_HandlerPage[className];
        }
        #endregion
    }
}
