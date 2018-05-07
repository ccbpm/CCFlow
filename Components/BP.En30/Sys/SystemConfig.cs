#region Copyright
//------------------------------------------------------------------------------
// <copyright file="ConfigReaders.cs" company="BP">
//     
//      Copyright (c) 2002 Microsoft Corporation.  All rights reserved.
//     
//      BP ZHZS Team
//      Purpose: config system: finds config files, loads config factories,
//               filters out relevant config file sections
//      Date: Oct 14, 2003
//      Author: peng zhou (pengzhoucn@hotmail.com) 
//      http://www.BP.com.cn
//
// </copyright>                                                                
//------------------------------------------------------------------------------
#endregion

using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.IO;
using MySql;
using MySql.Data;
using MySql.Data.Common;
using MySql.Data.MySqlClient;
using IBM;
using IBM.Data;
using IBM.Data.Informix;
using BP.DA;


namespace BP.Sys
{
    /// <summary>
    /// 组织解构数据来源
    /// </summary>
    public enum OSDBSrc
    {
        /// <summary>
        /// 数据库.
        /// </summary>
        Database,
        /// <summary>
        /// WebServices
        /// </summary>
        WebServices
    }
    /// <summary>
    /// 组织结构类型
    /// </summary>
    public enum OSModel
    {
        /// <summary>
        /// 一个人一个部门模式.
        /// </summary>
        OneOne,
        /// <summary>
        /// 一个人多个部门模式
        /// </summary>
        OneMore
    }
    /// <summary>
    /// 结束流程 窗口
    /// </summary>
    public enum IsOpenEndFlow
    {
        /// <summary>
        /// 默认不打开.
        /// </summary>
        Close,
        /// <summary>
        /// 打开
        /// </summary>
        Open
    }
    /// <summary>
    /// 运行平台
    /// </summary>
    public enum Plant
    {
        /// <summary>
        /// 默认不打开.
        /// </summary>
        CSharp,
        /// <summary>
        /// 打开
        /// </summary>
        Java
    }
    /// <summary>
    /// 系统配值
    /// </summary>
    public class SystemConfig
    {
        #region ftp配置.
        public static string FTPServerIP
        {
            get
            {
                return SystemConfig.AppSettings["FTPServerIP"];
            }
        }

        public static string FTPUserNo
        {
            get
            {

                string str = SystemConfig.AppSettings["FTPUserNo"];
                return BP.Sys.Glo.String_JieMi(str);
            }
        }

        public static string FTPUserPassword
        {
            get
            {
                string str= SystemConfig.AppSettings["FTPUserPassword"];
                return BP.Sys.Glo.String_JieMi(str);
            }
        }

        public static string AttachWebSite
        {
            get
            {
                return SystemConfig.AppSettings["AttachWebSite"];
            }
        }
        #endregion

        #region 组织结构的配置.

        /// <summary>
        /// OS结构
        /// </summary>
        public static OSModel OSModel
        {
            get
            {
                return (OSModel)SystemConfig.GetValByKeyInt("OSModel", 0);
            }
        }
        public static OSDBSrc OSDBSrc
        {
            get
            {
                return (OSDBSrc)SystemConfig.GetValByKeyInt("OSDBSrc", 0);
            }
        }
        /// <summary>
        /// 结束流程 窗口配置
        /// </summary>
        public static IsOpenEndFlow IsOpenEndFlow
        {
            get
            {
                return (IsOpenEndFlow)SystemConfig.GetValByKeyInt("IsOpenEndFlow", 0);
            }
        }
        #endregion

        /// <summary>
        /// 运行的平台为转换java平台使用.
        /// </summary>
        public static Plant Plant = Sys.Plant.CSharp;
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="cfgFile"></param>
        public static void ReadConfigFile(string cfgFile)
        {
            #region 清除缓存
            BP.En.ClassFactory._BPAssemblies = null;
            if (BP.En.ClassFactory.Htable_Ens != null)
                BP.En.ClassFactory.Htable_Ens.Clear();

            if (BP.En.ClassFactory.Htable_XmlEn != null)
                BP.En.ClassFactory.Htable_XmlEn.Clear();

            if (BP.En.ClassFactory.Htable_XmlEns != null)
                BP.En.ClassFactory.Htable_XmlEns.Clear();

            if (BP.Sys.SystemConfig.CS_AppSettings != null)
                BP.Sys.SystemConfig.CS_AppSettings.Clear();
            #endregion 清除缓存

            #region 加载 Web.Config 文件配置
            if (File.Exists(cfgFile) == false)
                throw new Exception("文件不存在 [" + cfgFile + "]");
            string _RefConfigPath = cfgFile;
            StreamReader read = new StreamReader(cfgFile);
            string firstline = read.ReadLine();
            string cfg = read.ReadToEnd();
            read.Close();

            int start = cfg.ToLower().IndexOf("<appsettings>");
            int end = cfg.ToLower().IndexOf("</appsettings>");

            cfg = cfg.Substring(start, end - start + "</appsettings".Length + 1);

            string tempFile = "Web.config.xml";

            StreamWriter write = new StreamWriter(tempFile);
            write.WriteLine(firstline);
            write.Write(cfg);
            write.Flush();
            write.Close();

            DataSet dscfg = new DataSet("cfg");
            dscfg.ReadXml(tempFile);

            //    BP.Sys.SystemConfig.CS_AppSettings = new System.Collections.Specialized.NameValueCollection();
            BP.Sys.SystemConfig.CS_DBConnctionDic.Clear();
            foreach (DataRow row in dscfg.Tables["add"].Rows)
            {
                BP.Sys.SystemConfig.CS_AppSettings.Add(row["key"].ToString().Trim(), row["value"].ToString().Trim());
            }
            #endregion
        }

        #region 关于开发商的信息
        public static string Ver
        {
            get
            {
                try
                {
                    return AppSettings["Ver"];
                }
                catch
                {
                    return "1.0.0";
                }
            }
        }
        public static string TouchWay
        {
            get
            {
                try
                {
                    return AppSettings["TouchWay"];
                }
                catch
                {
                    return SystemConfig.CustomerTel + " 地址:" + SystemConfig.CustomerAddr;
                }
            }
        }
        public static string CopyRight
        {
            get
            {
                try
                {
                    return AppSettings["CopyRight"];
                }
                catch
                {
                    return "版权所有@" + CustomerName;
                }
            }
        }
        #endregion

        #region 用户配置信息
        /// <summary>
        /// 系统语言（）
        /// 对多语言的系统有效。
        /// </summary>
        public static string SysLanguage
        {
            get
            {
                string s = AppSettings["SysLanguage"];
                if (s == null)
                    s = "CH";
                return s;
            }
        }
        #endregion

        #region 逻辑处理
        /// <summary>
        /// 封装了AppSettings
        /// </summary>		
        private static NameValueCollection _CS_AppSettings;
        public static NameValueCollection CS_AppSettings
        {
            get
            {
                if (_CS_AppSettings == null)
                    _CS_AppSettings = new NameValueCollection();
                return _CS_AppSettings;
            }
            set
            {
                _CS_AppSettings = value;
            }
        }
        /// <summary>
        /// 封装了AppSettings
        /// </summary>
        public static NameValueCollection AppSettings
        {
            get
            {
                if (SystemConfig.IsBSsystem)
                {
                    return System.Configuration.ConfigurationManager.AppSettings;
                }
                else
                {
                    return CS_AppSettings;
                }
            }
        }
        static SystemConfig()
        {
            CS_DBConnctionDic = new Hashtable();
        }
        /// <summary>
        /// 应用程序路径
        /// </summary>
        public static string PhysicalApplicationPath
        {
            get
            {
                if (SystemConfig.IsBSsystem && HttpContext.Current != null)
                    return HttpContext.Current.Request.PhysicalApplicationPath;
                else
                    return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }
        /// <summary>
        /// 文件放置的路径
        /// </summary>
        public static string PathOfUsersFiles
        {
            get
            {
                return "/Data/Files/";
            }
        }
        /// <summary>
        /// 临时文件路径
        /// </summary>
        public static string PathOfTemp
        {
            get
            {
                return PathOfDataUser + "\\Temp\\";
            }
        }
        public static string PathOfWorkDir
        {
            get
            {
                if (BP.Sys.SystemConfig.IsBSsystem)
                {
                    string path1 = HttpContext.Current.Request.PhysicalApplicationPath + "\\..\\";
                    System.IO.DirectoryInfo info1 = new DirectoryInfo(path1);
                    return info1.FullName;
                }
                else
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\..\\";
                    System.IO.DirectoryInfo info = new DirectoryInfo(path);
                    return info.FullName;
                }
            }
        }
        public static string PathOfFDB
        {
            get
            {
                string s = SystemConfig.AppSettings["FDB"];
                if (s == "" || s == null)
                    return PathOfWebApp + "\\DataUser\\FDB\\";
                return s;
            }
        }
        /// <summary>
        /// 数据文件
        /// </summary>
        public static string PathOfData
        {
            get
            {
                return PathOfWebApp + SystemConfig.AppSettings["DataDirPath"] + "\\Data\\";
            }
        }
        public static string PathOfDataUser
        {
            get
            {
                string tmp = SystemConfig.AppSettings["DataUserDirPath"];
                if (DataType.IsNullOrEmpty(tmp))
                {
                    tmp = PathOfWebApp + "DataUser\\";
                }
                else
                {
                    if (tmp.Contains("\\"))
                    {
                        tmp.Replace("\\", "");
                    }

                    tmp = PathOfWebApp + tmp + "\\DataUser\\";
                }
                return tmp;
            }
        }
        /// <summary>
        /// XmlFilePath
        /// </summary>
        public static string PathOfXML
        {
            get
            {
                return PathOfWebApp + SystemConfig.AppSettings["DataDirPath"] + "\\Data\\XML\\";
            }
        }
        public static string PathOfAppUpdate
        {
            get
            {
                return PathOfWebApp + SystemConfig.AppSettings["DataDirPath"] + "\\Data\\AppUpdate\\";
            }
        }
        public static string PathOfCyclostyleFile
        {
            get
            {
                return PathOfWebApp + "\\DataUser\\CyclostyleFile\\";
            }
        }
        /// <summary>
        /// 应用程序名称
        /// </summary>
        public static string AppName
        {
            get
            {
                return BP.Sys.Glo.Request.ApplicationPath.Replace("/", "");
            }
        }
        /// <summary>
        /// ccflow物理目录
        /// </summary>
        public static string CCFlowAppPath
        {
            get
            {
                if (!DataType.IsNullOrEmpty(SystemConfig.AppSettings["DataUserDirPath"]))
                {
                    return PathOfWebApp + SystemConfig.AppSettings["DataUserDirPath"];
                }
                return PathOfWebApp;
            }
        }
        /// <summary>
        /// ccflow网站目录
        /// </summary>
        public static string CCFlowWebPath
        {
            get
            {
                if (!DataType.IsNullOrEmpty(SystemConfig.AppSettings["CCFlowAppPath"]))
                {
                    return SystemConfig.AppSettings["CCFlowAppPath"];
                }
                return "/";
            }
        }
        /// <summary>
        /// 网站地址用于生成url, 支持cs程序调用ws程序.
        /// </summary>
        public static string HostURL
        {
            get
            {
                if (DataType.IsNullOrEmpty(SystemConfig.AppSettings["HostURL"])==false)
                {
                    return SystemConfig.AppSettings["HostURL"];
                }
                return HostURLOfBS; // "http:/127.0.0.1/";
            }
        }
        /// <summary>
        /// HostURL 在bs的模式下调用.
        /// </summary>
        public static string HostURLOfBS
        {
            get
            {
                string url = "http://" + System.Web.HttpContext.Current.Request.Url.Authority;
                return url;
            }
        }
        /// <summary>
        /// WebApp Path.
        /// </summary>
        public static string PathOfWebApp
        {
            get
            {
                if (SystemConfig.IsBSsystem)
                {
                    return BP.Sys.Glo.Request.PhysicalApplicationPath;
                }
                else
                {
                    if (SystemConfig.SysNo == "FTA")
                        return AppDomain.CurrentDomain.BaseDirectory;
                    else
                        return AppDomain.CurrentDomain.BaseDirectory + "..\\..\\";
                }
            }
        }
        #endregion

        #region 共同变量。
        public static bool IsBSsystem_Test = true;
        /// <summary>
        /// 是不是BS系统结构。
        /// </summary>
        private static bool _IsBSsystem = true;
        /// <summary>
        /// 是不是BS系统结构。
        /// </summary>
        public static bool IsBSsystem
        {
            get
            {
                // return true;
                return SystemConfig._IsBSsystem;
            }
            set
            {
                SystemConfig._IsBSsystem = value;
            }
        }
        public static bool IsCSsystem
        {
            get
            {
                return !SystemConfig._IsBSsystem;
            }
        }
        #endregion

        #region 系统配置信息
        /// <summary>
        /// 执行清空
        /// </summary>
        public static void DoClearCash()
        {
            // HttpRuntime.UnloadAppDomain();
            BP.DA.Cash.Map_Cash.Clear();
            BP.DA.Cash.SQL_Cash.Clear();
            BP.DA.Cash.EnsData_Cash.Clear();
            BP.DA.Cash.EnsData_Cash_Ext.Clear();
            BP.DA.Cash.BS_Cash.Clear();
            BP.DA.Cash.Bill_Cash.Clear();
            BP.DA.CashEntity.DCash.Clear();
        }
        /// <summary>
        /// 系统编号
        /// </summary>		 
        public static string SysNo
        {
            get { return AppSettings["SysNo"]; }
        }

        /// <summary>
        /// 系统名称
        /// </summary>
        public static string SysName
        {
            get
            {
                string s = AppSettings["SysName"];
                if (s == null)
                    s = "请在web.config中配置SysName名称。";
                return s;
            }
        }
        public static string OrderWay
        {
            get
            {
                return AppSettings["OrderWay"];
            }
        }
        public static int PageSize
        {
            get
            {
                try
                {
                    return int.Parse(AppSettings["PageSize"]);
                }
                catch
                {
                    return 99999;
                }
            }
        }
        public static int MaxDDLNum
        {
            get
            {
                try
                {
                    return int.Parse(AppSettings["MaxDDLNum"]);
                }
                catch
                {
                    return 50;
                }
            }
        }
        public static int PageSpan
        {
            get
            {
                try
                {
                    return int.Parse(AppSettings["PageSpan"]);
                }
                catch
                {
                    return 20;
                }
            }
        }
        /// <summary>
        ///  到的路径.PageOfAfterAuthorizeLogin
        /// </summary>
        public static string PageOfAfterAuthorizeLogin
        {
            get { return BP.Sys.Glo.Request.ApplicationPath + "" + AppSettings["PageOfAfterAuthorizeLogin"]; }
        }
        /// <summary>
        /// 丢失session 到的路径.
        /// </summary>
        public static string PageOfLostSession
        {
            get { return BP.Sys.Glo.Request.ApplicationPath + "" + AppSettings["PageOfLostSession"]; }
        }
        /// <summary>
        /// 日志路径
        /// </summary>
        public static string PathOfLog
        {
            get { return PathOfWebApp + "\\DataUser\\Log\\"; }
        }

        /// <summary>
        /// 系统名称
        /// </summary>
        public static int TopNum
        {
            get
            {
                try
                {
                    return int.Parse(AppSettings["TopNum"]);
                }
                catch
                {
                    return 99999;
                }
            }
        }
        /// <summary>
        /// 服务电话
        /// </summary>
        public static string ServiceTel
        {
            get { return AppSettings["ServiceTel"]; }
        }
        /// <summary>
        /// 服务E-mail
        /// </summary>
        public static string ServiceMail
        {
            get { return AppSettings["ServiceMail"]; }
        }
        /// <summary>
        /// 第3方软件
        /// </summary>
        public static string ThirdPartySoftWareKey
        {
            get
            {
                return AppSettings["ThirdPartySoftWareKey"];
            }
        }
        /// <summary>
        /// 是否启用CCIM?
        /// </summary>
        public static bool IsEnableCCIM
        {
            get
            {
                if (AppSettings["IsEnableCCIM"] == "1")
                    return true;
                else
                    return false;
            }
        }
        
        public static bool IsEnableNull
        {
            get
            {
                if (AppSettings["IsEnableNull"] == "1")
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是否 debug 状态
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                if (AppSettings["IsDebug"] == "1")
                    return true;
                else
                    return false;
            }
        }
        public static bool IsOpenSQLCheck
        {
            get
            {
                if (AppSettings["IsOpenSQLCheck"] == "0")
                    return false;
                else
                    return true;
            }
        }
        /// <summary>
        /// 是不是多系统工作。
        /// </summary>
        public static bool IsMultiSys
        {
            get
            {
                if (AppSettings["IsMultiSys"] == "1")
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是不是多线程工作。
        /// </summary>
        public static bool IsMultiThread_del
        {
            get
            {
                if (AppSettings["IsMultiThread"] == "1")
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是不是多语言版本
        /// </summary>
        public static bool IsMultiLanguageSys
        {
            get
            {
                if (AppSettings["IsMultiLanguageSys"] == "1")
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是否启用密码加密
        /// </summary>
        public static bool IsEnablePasswordEncryption
        {
            get
            {
                string s = AppSettings["IsEnablePasswordEncryption"] as string;
                if (s == null || s == "0")
                    return false;
                return true;
            }
        }
        #endregion

        #region 处理临时缓存
        /// <summary>
        /// 回话丢失时间长度(默认为500分钟)
        /// </summary>
        public static int SessionLostMinute
        {
            get
            {
                return SystemConfig.GetValByKeyInt("SessionLostMinute", 500000);
            }
        }
        /// <summary>
        /// 放在 Temp 中的cash 多少时间失效。
        /// 0, 表示永久不失效。
        /// </summary>
        private static int CashFail
        {
            get
            {
                try
                {
                    return int.Parse(AppSettings["CashFail"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 当前的 TempCash 是否失效了
        /// </summary>
        public static bool IsTempCashFail
        {
            get
            {
                if (SystemConfig.CashFail == 0)
                    return false;

                if (_CashFailDateTime == null)
                {
                    _CashFailDateTime = DateTime.Now;
                    return true;
                }
                else
                {
                    TimeSpan ts = DateTime.Now - _CashFailDateTime;
                    if (ts.Minutes >= SystemConfig.CashFail)
                    {
                        _CashFailDateTime = DateTime.Now;
                        return true;
                    }
                    return false;
                }
            }
        }
        public static DateTime _CashFailDateTime;
        #endregion

        #region 客户配置信息
        /// <summary>
        /// 客户编号
        /// </summary>
        public static string CustomerNo
        {
            get
            {
                return AppSettings["CustomerNo"];
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public static string CustomerName
        {
            get
            {
                return AppSettings["CustomerName"];
            }
        }
        public static string CustomerURL
        {
            get
            {
                return AppSettings["CustomerURL"];
            }
        }
        /// <summary>
        /// 客户简称
        /// </summary>
        public static string CustomerShortName
        {
            get
            {
                return AppSettings["CustomerShortName"];
            }
        }
        /// <summary>
        /// 客户地址
        /// </summary>
        public static string CustomerAddr
        {
            get
            {
                return AppSettings["CustomerAddr"];
            }
        }
        /// <summary>
        /// 客户电话
        /// </summary>
        public static string CustomerTel
        {
            get
            {
                return AppSettings["CustomerTel"];
            }
        }
        #endregion

        #region 微信相关配置信息
        /// <summary>
        /// 企业标识
        /// </summary>
        public static string WX_CorpID
        {
            get
            {
                return AppSettings["CorpID"];
            }
        }
        /// <summary>
        /// 帐号钥匙
        /// </summary>
        public static string WX_AppSecret
        {
            get
            {
                return AppSettings["AppSecret"];
            }
        }
        /// <summary>
        /// 应用令牌
        /// </summary>
        public static string WX_WeiXinToken
        {
            get
            {
                return AppSettings["WeiXinToken"];
            }
        }
        /// <summary>
        /// 应用加密所用的秘钥
        /// </summary>
        public static string WX_EncodingAESKey
        {
            get
            {
                return AppSettings["EncodingAESKey"];
            }
        }
        /// <summary>
        /// 进入应用后的欢迎提示
        /// </summary>
        public static bool WeiXin_AgentWelCom
        {
            get
            {
                return GetValByKeyBoolen("WeiXin_AgentWelCom", false);
            }
        }
        /// <summary>
        /// 应用ID
        /// </summary>
        public static string WX_AgentID
        {
            get
            {
                return AppSettings["AgentID"];
            }
        }
        /// <summary>
        /// 消息链接网址
        /// </summary>
        public static string WX_MessageUrl
        {
            get
            {
                return AppSettings["WeiXin_MessageUrl"];
            }
        }
        #endregion

        #region 钉钉配置相关
        /// <summary>
        /// 企业标识
        /// </summary>
        public static string Ding_CorpID
        {
            get
            {
                return AppSettings["Ding_CorpID"];
            }
        }
        /// <summary>
        /// 密钥
        /// </summary>
        public static string Ding_CorpSecret
        {
            get
            {
                return AppSettings["Ding_CorpSecret"];
            }
        }
        /// <summary>
        /// 登录验证密钥
        /// </summary>
        public static string Ding_SSOsecret
        {
            get
            {
                return AppSettings["Ding_SSOsecret"];
            }
        }
        /// <summary>
        /// 消息超链接服务器地址
        /// </summary>
        public static string Ding_MessageUrl
        {
            get
            {
                return AppSettings["Ding_MessageUrl"];
            }
        }
        /// <summary>
        /// 企业应用编号
        /// </summary>
        public static string Ding_AgentID
        {
            get
            {
                return AppSettings["Ding_AgentID"];
            }
        }
        #endregion

        /// <summary>
        ///取得配置 NestedNamesSection 内的相应 key 的内容
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static NameValueCollection GetConfig(string key)
        {
            Hashtable ht = (Hashtable)System.Configuration.ConfigurationManager.GetSection("NestedNamesSection");
            return (NameValueCollection)ht[key];
        }
        public static string GetValByKey(string key, string isNullas)
        {
            string s = AppSettings[key];
            if (s == null)
                s = isNullas;
            return s;
        }
        public static bool GetValByKeyBoolen(string key, bool isNullas)
        {
            string s = AppSettings[key];
            if (s == null)
                return isNullas;

            if (s == "1")
                return true;
            else
                return false;
        }
        public static int GetValByKeyInt(string key, int isNullas)
        {
            string s = AppSettings[key];
            if (s == null)
                return isNullas;
            return int.Parse(s);
        }
        /// <summary>
        /// 工作小时数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="isNullas">如果是空返回的值</param>
        /// <returns></returns>
        public static float GetValByKeyFloat(string key, int isNullas)
        {
            string s = AppSettings[key];
            if (s == null)
                return isNullas;
            return float.Parse(s);
        }
        public static string GetConfigXmlKeyVal(string key)
        {
            try
            {
                DataSet ds = new DataSet("dss");
                ds.ReadXml(BP.Sys.SystemConfig.PathOfXML + "\\KeyVal.xml");
                DataTable dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Key"].ToString() == key)
                        return dr["Val"].ToString();
                }
                throw new Exception("在您利用GetXmlConfig 取值出现错误，没有找到key= " + key + ", 请检查 /data/Xml/KeyVal.xml. ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetConfigXmlNode(string fk_Breed, string enName, string key)
        {
            try
            {
                string file = BP.Sys.SystemConfig.PathOfXML + "\\Node\\" + fk_Breed + ".xml";
                DataSet ds = new DataSet("dss");
                try
                {
                    ds.ReadXml(file);
                }
                catch
                {
                    return null;
                }
                DataTable dt = ds.Tables[0];
                if (dt.Columns.Contains(key) == false)
                    throw new Exception(file + "配置错误，您没有按照格式配置，它不包含标记 " + key);
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["NodeEnName"].ToString() == enName)
                    {
                        if (dr[key].Equals(DBNull.Value))
                            return null;
                        else
                            return dr[key].ToString();

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取xml中的配置信息
        /// GroupTitle, ShowTextLen, DefaultSelectedAttrs, TimeSpan
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ensName"></param>
        /// <returns></returns>
        public static string GetConfigXmlEns(string key, string ensName)
        {
            try
            {
                DataTable dt = BP.DA.Cash.GetObj("TConfigEns", BP.DA.Depositary.Application) as DataTable;
                if (dt == null)
                {
                    DataSet ds = new DataSet("dss");
                    ds.ReadXml(BP.Sys.SystemConfig.PathOfXML + "\\Ens\\ConfigEns.xml");
                    dt = ds.Tables[0];
                    BP.DA.Cash.AddObj("TConfigEns", BP.DA.Depositary.Application, dt);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Key"].ToString() == key && dr["For"].ToString() == ensName)
                        return dr["Val"].ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetConfigXmlSQL(string key)
        {
            try
            {
                DataSet ds = new DataSet("dss");
                ds.ReadXml(BP.Sys.SystemConfig.PathOfXML + "\\SQL\\" + BP.Sys.SystemConfig.ThirdPartySoftWareKey + ".xml");
                DataTable dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["No"].ToString() == key)
                        return dr["SQL"].ToString();
                }
                throw new Exception("在您利用 GetXmlConfig 取值出现错误，没有找到key= " + key + ", 请检查 /Data/XML/" + SystemConfig.ThirdPartySoftWareKey + ".xml. ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetConfigXmlSQLApp(string key)
        {
            try
            {
                DataSet ds = new DataSet("dss");
                ds.ReadXml(BP.Sys.SystemConfig.PathOfXML + "\\SQL\\App.xml");
                DataTable dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["No"].ToString() == key)
                        return dr["SQL"].ToString();
                }
                throw new Exception("在您利用 GetXmlConfig 取值出现错误，没有找到key= " + key + ", 请检查 /Data/XML/App.xml. ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetConfigXmlSQL(string key, string replaceKey, string replaceVal)
        {
            return GetConfigXmlSQL(key).Replace(replaceKey, replaceVal);
        }
        public static string GetConfigXmlSQL(string key, string replaceKey1, string replaceVal1, string replaceKey2, string replaceVal2)
        {
            return GetConfigXmlSQL(key).Replace(replaceKey1, replaceVal1).Replace(replaceKey2, replaceVal2);
        }

        #region dsn
        /// <summary>
        /// 数据库连接
        /// </summary>
        public static string AppCenterDSN
        {
            get
            {
                return AppSettings["AppCenterDSN"];
            }
            set
            {
                AppSettings["AppCenterDSN"] = value;
            }
        }
        public static string DBAccessOfOracle
        {
            get
            {
                return AppSettings["DBAccessOfOracle"];
            }
        }
        public static string DBAccessOfOracle1
        {
            get
            {
                return AppSettings["DBAccessOfOracle1"];
            }
        }
        public static string DBAccessOfMSSQL
        {
            get
            {
                return AppSettings["DBAccessOfMSSQL"];
            }
        }
        public static string DBAccessOfOLE
        {
            get
            {
                string dsn = AppSettings["DBAccessOfOLE"];
                if (dsn.IndexOf("@Pass") != -1)
                    dsn = dsn.Replace("@Pass", "helloccs");

                if (dsn.IndexOf("@Path") != -1)
                    dsn = dsn.Replace("@Path", SystemConfig.PathOfWebApp);
                return dsn;

            }
        }
        public static string DBAccessOfODBC
        {
            get
            {
                return AppSettings["DBAccessOfODBC"];
            }
        }
        #endregion
        /// <summary>
        /// 获取主应用程序的数据库部署方式．
        /// </summary>
        public static BP.DA.DBModel AppCenterDBModel
        {
            get
            {
                switch (AppSettings["AppCenterDBModel"])
                {
                    case "Domain":
                        return BP.DA.DBModel.Domain;
                    default:
                        return BP.DA.DBModel.Single;
                }
            }
        }
        /// <summary>
        /// 获取主应用程序的数据库类型．
        /// </summary>
        public static BP.DA.DBType AppCenterDBType
        {
            get
            {
                switch (AppSettings["AppCenterDBType"])
                {
                    case "MSMSSQL":
                    case "MSSQL":
                        return BP.DA.DBType.MSSQL;
                    case "Oracle":
                        return BP.DA.DBType.Oracle;
                    case "MySQL":
                        return BP.DA.DBType.MySQL;
                    case "Access":
                        return BP.DA.DBType.Access;
                    case "Informix":
                        return BP.DA.DBType.Informix;
                    default:
                        return BP.DA.DBType.Oracle;
                }
            }
        }
        private static string _AppCenterDBDatabase = null;
        /// <summary>
        /// 数据库名称
        /// </summary>
        public static string AppCenterDBDatabase
        {
            get
            {
                if (_AppCenterDBDatabase == null)
                {
                    switch (BP.DA.DBAccess.AppCenterDBType)
                    {
                        case DA.DBType.MSSQL:
                            SqlConnection connMSSQL = new SqlConnection(SystemConfig.AppCenterDSN);
                            if (connMSSQL.State != ConnectionState.Open)
                                connMSSQL.Open();
                            _AppCenterDBDatabase = connMSSQL.Database;
                            break;
                        case DA.DBType.Oracle:
                            OracleConnection connOra = new OracleConnection(SystemConfig.AppCenterDSN);
                            if (connOra.State != ConnectionState.Open)
                                connOra.Open();
                            _AppCenterDBDatabase = connOra.Database;
                            break;
                        case DA.DBType.MySQL:
                            MySqlConnection connMySQL = new MySqlConnection(SystemConfig.AppCenterDSN);
                            if (connMySQL.State != ConnectionState.Open)
                                connMySQL.Open();
                            _AppCenterDBDatabase = connMySQL.Database;
                            break;
                        case DA.DBType.Informix:
                            IfxConnection connIFX = new IfxConnection(SystemConfig.AppCenterDSN);
                            if (connIFX.State != ConnectionState.Open)
                                connIFX.Open();
                            _AppCenterDBDatabase = connIFX.Database;
                            break;
                        default:
                            throw new Exception("@没有判断的数据类型.");
                            break;
                    }
                }

                // 返回database.
                return _AppCenterDBDatabase;
            }
        }
        /// <summary>
        /// 获取不同类型的数据库变量标记
        /// </summary>
        public static string AppCenterDBVarStr
        {
            get
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case BP.DA.DBType.Oracle:
                        return ":";
                    case BP.DA.DBType.MySQL:
                    case BP.DA.DBType.Informix:
                        return "?";
                    default:
                        return "@";
                }
            }
        }

        public static string AppCenterDBLengthStr
        {
            get
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case BP.DA.DBType.Oracle:
                        return "Length";
                    case BP.DA.DBType.MSSQL:
                        return "LEN";
                    case BP.DA.DBType.Informix:
                        return "Length";
                    case BP.DA.DBType.Access:
                        return "Length";
                    default:
                        return "Length";
                }
            }
        }
        /// <summary>
        /// 获取不同类型的substring函数的书写
        /// </summary>
        public static string AppCenterDBSubstringStr
        {
            get
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case BP.DA.DBType.Oracle:
                        return "substr";
                    case BP.DA.DBType.MSSQL:
                        return "substring";
                    case BP.DA.DBType.Informix:
                        return "MySubString";
                    case BP.DA.DBType.Access:
                        return "Mid";
                    default:
                        return "substring";
                }
            }
        }
        public static string AppCenterDBAddStringStr
        {
            get
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case BP.DA.DBType.Oracle:
                    case BP.DA.DBType.MySQL:
                    case BP.DA.DBType.Informix:
                        return "||";
                    default:
                        return "+";
                }
            }
        }
        public static readonly Hashtable CS_DBConnctionDic;
    }
}