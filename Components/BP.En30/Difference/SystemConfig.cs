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
using Oracle.ManagedDataAccess.Client;
using Kdbndp;
using System.IO;
using MySql;
using MySql.Data;
using MySql.Data.Common;
using MySql.Data.MySqlClient;
using BP.DA;
using BP.Web;
using BP.Sys;
using BP.Difference;

namespace BP.Difference
{

    /// <summary>
    /// 系统配值
    /// </summary>
    public class SystemConfig
    {
        #region ftp 配置.
        /// <summary>
        /// ftp服务器类型.
        /// </summary>
        public static string FTPServerType
        {
            get
            {
                string str = BP.Difference.SystemConfig.AppSettings["FTPServerType"];
                return BP.Sys.Base.Glo.String_JieMi(str);
            }
        }
        /// <summary>
        /// 服务器IP
        /// </summary>
        public static string FTPServerIP
        {
            get
            {
                string str = BP.Difference.SystemConfig.AppSettings["FTPServerIP"];
                return BP.Sys.Base.Glo.String_JieMi(str);
            }
        }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public static string FTPPort
        {
            get
            {
                string str = BP.Difference.SystemConfig.AppSettings["FTPPort"];
                return BP.Sys.Base.Glo.String_JieMi(str);
            }
        }
        /// <summary>
        /// 用户编号
        /// </summary>
        public static string FTPUserNo
        {
            get
            {
                string str = BP.Difference.SystemConfig.AppSettings["FTPUserNo"];
                return BP.Sys.Base.Glo.String_JieMi(str);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public static string FTPUserPassword
        {
            get
            {
                string str = BP.Difference.SystemConfig.AppSettings["FTPUserPassword"];
                return BP.Sys.Base.Glo.String_JieMi(str);
            }
        }
        /// <summary>
        /// 端口号
        /// </summary>
        public static int FTPServerPort
        {
            get
            {
                string str = BP.Difference.SystemConfig.AppSettings["FTPServerPort"];
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "21";
                str = BP.Sys.Base.Glo.String_JieMi(str);
                return int.Parse(str);
            }
        }
        /// <summary>
        /// 附件上传加密
        /// </summary>
        public static bool IsEnableAthEncrypt
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKeyBoolen("IsEnableAthEncrypt", false);
            }
        }
        /// <summary>
        /// 附件上传位置
        /// </summary>
        public static bool IsUploadFileToFTP
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKeyBoolen("IsUploadFileToFTP", false);
            }
        }

        public static string AttachWebSite
        {
            get
            {
                return BP.Difference.SystemConfig.AppSettings["AttachWebSite"];
            }
        }
        #endregion
        /// <summary>
        /// 运行的平台为转换java平台使用.
        /// </summary>
        public static Plant Plant = BP.Sys.Plant.CSharp;
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

            if (BP.Difference.SystemConfig.CS_AppSettings != null)
                SystemConfig.CS_AppSettings.Clear();
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

            //    SystemConfig.CS_AppSettings = new System.Collections.Specialized.NameValueCollection();
            SystemConfig.CS_DBConnctionDic.Clear();
            foreach (DataRow row in dscfg.Tables["add"].Rows)
            {
                SystemConfig.CS_AppSettings.Add(row["key"].ToString().Trim(), row["value"].ToString().Trim());
            }
            #endregion
        }

        #region 用户配置信息
        /// <summary>
        /// 运行模式
        /// </summary>
        public static CCBPMRunModel CCBPMRunModel
        {
            get
            {
                int val = BP.Difference.SystemConfig.GetValByKeyInt("CCBPMRunModel", 0);
                if (val == 0)
                    return CCBPMRunModel.Single;

                if (val == 1)
                    return CCBPMRunModel.GroupInc;

                if (val == 2)
                    return CCBPMRunModel.SAAS;

                return CCBPMRunModel.Single;
            }
        }
        /// <summary>
        /// token验证模式 
        /// 0=宽泛模式, 一个账号可以在多个设备登录. 
        /// 1=唯一模式. 一个账号仅仅在一台设备登录，另外的设备就会失效.
        /// </summary>
        public static int TokenModel
        {
            get
            {
                string s = AppSettings["TokenModel"];
                if (s == null)
                    return 0;
                return int.Parse(s);
            }
        }
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
                if (BP.Difference.SystemConfig.IsBSsystem)
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
                if (BP.Difference.SystemConfig.IsBSsystem && HttpContextHelper.Current != null)
                    return HttpContextHelper.PhysicalApplicationPath;
                else
                    return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }
        /// <summary>
        /// 临时文件路径
        /// </summary>
        public static string PathOfTemp
        {
            get
            {
                return PathOfDataUser + "Temp/";
            }
        }
        public static string PathOfWorkDir
        {
            get
            {
                if (BP.Difference.SystemConfig.IsBSsystem)
                {
                    string path1 = HttpContextHelper.PhysicalApplicationPath + "/../";
                    System.IO.DirectoryInfo info1 = new DirectoryInfo(path1);
                    return info1.FullName;
                }
                else
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "/../../../";
                    System.IO.DirectoryInfo info = new DirectoryInfo(path);
                    return info.FullName;
                }
            }
        }
        public static string PathOfFDB
        {
            get
            {
                string s = BP.Difference.SystemConfig.AppSettings["FDB"];
                if (DataType.IsNullOrEmpty(s) == true)
                    return PathOfWebApp + "DataUser/FDB/";
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
                return PathOfWebApp + "WF/Data/";
            }
        }
        public static string PathOfDataUser
        {
            get
            {
                string tmp = BP.Difference.SystemConfig.AppSettings["DataUserDirPath"];
                if (DataType.IsNullOrEmpty(tmp))
                {
                    tmp = PathOfWebApp + "DataUser/";
                }
                else
                {
                    if (tmp.Contains("/"))
                        tmp.Replace("/", "");
                    tmp = PathOfWebApp + tmp + "/DataUser/";
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
                return PathOfWebApp + "WF/Data/XML/";
            }
        }

        public static string PathOfCyclostyleFile
        {
            get
            {
                return PathOfWebApp + "DataUser/CyclostyleFile/";
            }
        }
        /// <summary>
        /// 应用程序名称
        /// </summary>
        public static string AppName
        {
            get
            {
                return HttpContextHelper.RequestApplicationPath.Replace("/", "");
            }
        }
        /// <summary>
        /// ccflow物理目录
        /// </summary>
        public static string CCFlowAppPath
        {
            get
            {
                if (DataType.IsNullOrEmpty(BP.Difference.SystemConfig.AppSettings["DataUserDirPath"]) == false)
                {
                    return PathOfWebApp + BP.Difference.SystemConfig.AppSettings["DataUserDirPath"];
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
                if (!DataType.IsNullOrEmpty(BP.Difference.SystemConfig.AppSettings["CCFlowAppPath"]))
                {
                    return BP.Difference.SystemConfig.AppSettings["CCFlowAppPath"];
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
                if (DataType.IsNullOrEmpty(BP.Difference.SystemConfig.AppSettings["HostURL"]) == false)
                {
                    return BP.Difference.SystemConfig.AppSettings["HostURL"];
                }
                return HostURLOfBS; // "http:/127.0.0.1/";
            }
        }

        /// <summary>
        /// 移动端用于生成url, 支持cs程序调用ws程序.
        /// </summary>
        public static string MobileURL
        {
            get
            {
                if (DataType.IsNullOrEmpty(BP.Difference.SystemConfig.AppSettings["MobileURL"]) == false)
                {
                    return BP.Difference.SystemConfig.AppSettings["MobileURL"];
                }
                return BP.Difference.SystemConfig.AppSettings["MobileURL"]; // "http:/127.0.0.1/";
            }
        }

        /// <summary>
        /// HostURL 在bs的模式下调用.
        /// </summary>
        public static string HostURLOfBS
        {
            get
            {
                string url = "http://" + HttpContextHelper.RequestUrlAuthority;
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
                if (BP.Difference.SystemConfig.IsBSsystem)
                    return HttpContextHelper.PhysicalApplicationPath;

                return AppDomain.CurrentDomain.BaseDirectory + "../../";
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
                return BP.Difference.SystemConfig._IsBSsystem;
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
            Cash.Map_Cash.Clear();
            Cash.SQL_Cash.Clear();
            Cash.EnsData_Cash.Clear();
            Cash.EnsData_Cash_Ext.Clear();
            Cash.BS_Cash.Clear();
            Cash.Bill_Cash.Clear();
            CashEntity.DCash.Clear();
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

         
        public static string UserLockTimeSeconds
        {
            get
            {
                string s = AppSettings["UserLockTimeSeconds"];
                if (s == null)
                    s = "请在web.config中配置UserLockTimeSeconds名称。";
                return s;
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

        /// <summary>
        /// 日志路径
        /// </summary>
        public static string PathOfLog
        {
            get { return PathOfWebApp + "DataUser/Log/"; }
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
        public static bool IsDisHelp
        {
            get
            {
                if (AppSettings["IsDisHelp"] == "1")
                    return true;
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
        /// <summary>
        /// 密码加密方式
        /// </summary>
        public static String GetPasswordEncryptionType
        {
            get
            {
                string s = AppSettings["PasswordEncryptionType"] as string;
                if (string.IsNullOrWhiteSpace(s))
                    return "1";
                return s;
            }
        }
        /// <summary>
        /// 是否多语言？
        /// </summary>
        public static bool IsMultilingual
        {
            get
            {
                if (AppSettings["IsMultilingual"] == "1")
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 使用的语言
        /// </summary>
        public static string Langue
        {
            get
            {
                string str = AppSettings["Langue"];
                if (DataType.IsNullOrEmpty(str))
                    return "CN";
                return str;
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
                return BP.Difference.SystemConfig.GetValByKeyInt("SessionLostMinute", 500000);
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
                if (BP.Difference.SystemConfig.CashFail == 0)
                    return false;

                if (_CashFailDateTime == null)
                {
                    _CashFailDateTime = DateTime.Now;
                    return true;
                }
                else
                {
                    TimeSpan ts = DateTime.Now - _CashFailDateTime;
                    if (ts.Minutes >= BP.Difference.SystemConfig.CashFail)
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
        /// <summary>
        /// 集团模式下的角色体系
        /// @0=每套组织都有自己的角色体系@1=所有的组织共享一套岗则体系.@2=每个部门有自己的角色体系.
        /// </summary>
        public static int GroupStationModel
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKeyInt("GroupStationModel", 0);
            }
        }

        /// <summary>
        /// 客户名称
        /// </summary>
        public static string RunOnPlant
        {
            get
            {
                return "CCFlow";
                //return AppSettings["RunOnPlant"] ?? "";
            }
        }
        public static string CustomerURL
        {
            get
            {
                return AppSettings["CustomerURL"];
            }
        }
        public static string Domain
        {
            get
            {
                return AppSettings["Domain"];
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
        /// 是否使用微信企业号中的通讯录帐号登录
        /// </summary>
        public static string OZType
        {
            get
            {
                return AppSettings["OZType"];
            }
        }
        public static string OZParentNo
        {
            get
            {
                return AppSettings["OZParentNo"];
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

        #region 微信公众号相关
        /// <summary>
        /// 公众号唯一标识
        /// </summary>
        public static string WXGZH_Appid
        {
            get
            {
                return AppSettings["WXGZH_Appid"];
            }
        }
        /// <summary>
        /// 公众号开发者密码
        /// </summary>
        public static string WXGZH_AppSecret
        {
            get
            {
                return AppSettings["WXGZH_AppSecret"];
            }
        }
        /// <summary>
        /// 公众号token
        /// </summary>
        public static string WXGZH_Token
        {
            get
            {
                return AppSettings["GZHToKen"];
            }
        }
        /// <summary>
        /// 公众号EncodingAESKey
        /// </summary>
        public static string WXGZH_AESKey
        {
            get
            {
                return AppSettings["GZHEncodingAESKey"];
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
        public static string Ding_AppKey
        {
            get
            {
                return AppSettings["Ding_AppKey"];
            }
        }
        public static string Ding_AppSecret
        {
            get
            {
                return AppSettings["Ding_AppSecret"];
            }
        }
        #endregion

        #region 百度云配置相关
        /// <summary>
        /// 百度云应用ID
        /// </summary>
        public static string AppID
        {
            get
            {
                return AppSettings["AppID"];
            }
        }
        /// <summary>
        /// 百度云应用AK
        /// </summary>
        public static string APIKey
        {
            get
            {
                return AppSettings["APIKey"];
            }
        }
        /// <summary>
        /// 百度云应用SK
        /// </summary>
        public static string SecretKey
        {
            get
            {
                return AppSettings["SecretKey"];
            }
        }
        #endregion

        #region 其他配置.
        /// <summary>
        ///取得配置 NestedNamesSection 内的相应 key 的内容.
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
                ds.ReadXml(BP.Difference.SystemConfig.PathOfXML + "KeyVal.xml");
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
                string file = BP.Difference.SystemConfig.PathOfXML + "Node/" + fk_Breed + ".xml";
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
                DataTable dt = Cash.GetObj("TConfigEns", Depositary.Application) as DataTable;
                if (dt == null)
                {
                    DataSet ds = new DataSet("dss");
                    ds.ReadXml(BP.Difference.SystemConfig.PathOfXML + "Ens/ConfigEns.xml");
                    dt = ds.Tables[0];
                    Cash.AddObj("TConfigEns", Depositary.Application, dt);
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
                ds.ReadXml(BP.Difference.SystemConfig.PathOfXML + "SQL/" + BP.Difference.SystemConfig.ThirdPartySoftWareKey + ".xml");
                DataTable dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["No"].ToString() == key)
                        return dr["SQL"].ToString();
                }
                throw new Exception("在您利用 GetXmlConfig 取值出现错误，没有找到key= " + key + ", 请检查 /Data/XML/" + BP.Difference.SystemConfig.ThirdPartySoftWareKey + ".xml. ");
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
                ds.ReadXml(BP.Difference.SystemConfig.PathOfXML + "SQL/App.xml");
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
        #endregion 其他配置.

        #region dsn
        /// <summary>
        /// 数据库连接.
        /// </summary>
        public static string _AppCenterDSN = null;
        /// <summary>
        /// 数据库连接
        /// </summary>
        public static string AppCenterDSN
        {
            get
            {
                if (_AppCenterDSN != null)
                    return _AppCenterDSN;

                string str = AppSettings["AppCenterDSN"];
                if (DataType.IsNullOrEmpty(str) == false)
                    return str;

                str = AppSettings["AppCenterDSN.encryption"];

                if (DataType.IsNullOrEmpty(str) == true)
                    throw new Exception("err@没有配置数据库连接字符串.");

                DecryptAndEncryptionHelper.decode decode = new DecryptAndEncryptionHelper.decode();
                _AppCenterDSN = decode.decode_exe(str);
                return _AppCenterDSN;
            }
            set
            {
                _AppCenterDSN = value;
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

        public static string DBAccessOfKingBaseR3
        {
            get
            {
                return AppSettings["DBAccessOfKingBaseR3"];
            }
        }

        public static string DBAccessOfKingBaseR6
        {
            get
            {
                return AppSettings["DBAccessOfKingBaseR6"];
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
                    dsn = dsn.Replace("@Path", BP.Difference.SystemConfig.PathOfWebApp);
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

        #region xx
        /// <summary>
        /// 获取主应用程序的数据库部署方式．
        /// </summary>
        public static DBModel AppCenterDBModel
        {
            get
            {
                switch (AppSettings["AppCenterDBModel"])
                {
                    case "Domain":
                        return DBModel.Domain;
                    default:
                        return DBModel.Single;
                }
            }
        }
        /// <summary>
        /// 传入的参数，是否需要类型
        /// </summary>
        public static bool AppCenterDBFieldIsParaDBType
        {
            get
            {
                switch (AppCenterDBType)
                {
                    case DBType.UX:
                    case DBType.PostgreSQL:
                        return true;
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// 获得数据是否区分大小写
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static FieldCaseModel GetFieldCaseModel(DBType db)
        {
            switch (db)
            {
                case DBType.KingBaseR6:
                    return FieldCaseModel.Lowercase; //小写的.
                case DBType.KingBaseR3: //// R3时，查询敏感设置
                    string sql = "show case_sensitive;";
                    string caseSen = DBAccess.RunSQLReturnString(sql);
                    if ("on".Equals(caseSen))
                        return FieldCaseModel.UpperCase;
                    else
                        return FieldCaseModel.None;
                case DBType.Oracle:
                    return FieldCaseModel.UpperCase;
                case DBType.PostgreSQL:
                case DBType.UX:
                    return FieldCaseModel.Lowercase; //小写的.
                default:
                    return FieldCaseModel.None;
            }
        }

        /// <summary>
        /// 大小写模式
        /// </summary>
        public static FieldCaseModel AppCenterDBFieldCaseModel
        {
            get
            {
                return GetFieldCaseModel(BP.Difference.SystemConfig.AppCenterDBType);
            }
        }
        /// <summary>
        /// 获取主应用程序的数据库类型．
        /// </summary>
        public static DBType AppCenterDBType
        {
            get
            {
                switch (AppSettings["AppCenterDBType"])
                {
                    case "MSMSSQL":
                    case "MSSQL":
                        return DBType.MSSQL;
                    case "Oracle":
                        return DBType.Oracle;
                    case "MySQL":
                        return DBType.MySQL;
                    case "PostgreSQL":
                        return DBType.PostgreSQL;
                    case "DM":
                        return DBType.DM;
                    case "Access":
                        return DBType.Access;
                    case "Informix":
                        return DBType.Informix;
                    case "UX":
                        return DBType.UX;
                    case "KingBaseR3":
                        return DBType.KingBaseR3;
                    case "KingBaseR6":
                        return DBType.KingBaseR6;
                    default:
                        return DBType.Oracle;
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

                switch (DBAccess.AppCenterDBType)
                {
                    case DA.DBType.MSSQL:
                        SqlConnection connMSSQL = new SqlConnection(BP.Difference.SystemConfig.AppCenterDSN);
                        if (connMSSQL.State != ConnectionState.Open)
                            connMSSQL.Open();
                        _AppCenterDBDatabase = connMSSQL.Database;
                        break;
                    case DA.DBType.Oracle:

                        string[] strs = BP.Difference.SystemConfig.AppCenterDSN.Split(';');
                        foreach (string str in strs)
                        {

                            if (str.ToLower().Contains("user id") == false)
                                continue;

                            string[] mystrs = str.Split('=');
                            return mystrs[1];

                        }

                        //OracleConnection connOra = new OracleConnection(BP.Difference.SystemConfig.AppCenterDSN);
                        //if (connOra.State != ConnectionState.Open)
                        //    connOra.Open();
                        //_AppCenterDBDatabase = connOra.Database;
                        break;
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        string[] strskdb = BP.Difference.SystemConfig.AppCenterDSN.Split(';');
                        foreach (string str in strskdb)
                        {

                            if (str.ToLower().Contains("user id") == false)
                                continue;

                            string[] mystrs = str.Split('=');
                            return mystrs[1];

                        }
                        //KdbndpConnection kdbndpConnection = new KdbndpConnection(BP.Difference.SystemConfig.AppCenterDSN);
                        //_AppCenterDBDatabase = kdbndpConnection.Database;
                        break;

                    case DA.DBType.MySQL:
                        MySqlConnection connMySQL = new MySqlConnection(BP.Difference.SystemConfig.AppCenterDSN);
                        _AppCenterDBDatabase = connMySQL.Database;
                        break;
                    case DA.DBType.PostgreSQL:

                        Npgsql.NpgsqlConnection myconn = new Npgsql.NpgsqlConnection();
                        myconn.ConnectionString = BP.Difference.SystemConfig.AppCenterDSN;
                        myconn.Open();
                        _AppCenterDBDatabase = myconn.Database;
                        // PostgreSQL.my
                        //PostgreSQL  SqlConnection connMySQL = new MySqlConnection(BP.Difference.SystemConfig.AppCenterDSN);
                        // _AppCenterDBDatabase = connMySQL.Database;
                        break;
                    case DA.DBType.UX:

                        Nuxsql.NuxsqlConnection mycon2n = new Nuxsql.NuxsqlConnection();
                        mycon2n.ConnectionString = BP.Difference.SystemConfig.AppCenterDSN;
                        mycon2n.Open();
                        _AppCenterDBDatabase = mycon2n.Database;
                        // PostgreSQL.my
                        //PostgreSQL  SqlConnection connMySQL = new MySqlConnection(BP.Difference.SystemConfig.AppCenterDSN);
                        // _AppCenterDBDatabase = connMySQL.Database;
                        break;
                    //From Zhou IBM 删除
                    //case DA.DBType.Informix:
                    //    IfxConnection connIFX = new IfxConnection(BP.Difference.SystemConfig.AppCenterDSN);
                    //    if (connIFX.State != ConnectionState.Open)
                    //        connIFX.Open();
                    //    _AppCenterDBDatabase = connIFX.Database;
                    //    break;
                    default:
                        throw new Exception("@没有判断的数据类型AppCenterDBDatabase.");
                        break;
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
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                    case DBType.PostgreSQL:
                    case DBType.DM:
                        return ":";
                    case DBType.MySQL:
                    case DBType.Informix:
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
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        return "Length";
                    case DBType.MSSQL:
                        return "LEN";
                    case DBType.Informix:
                        return "Length";
                    case DBType.Access:
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
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        return "substr";
                    case DBType.MSSQL:
                        return "substring";
                    case DBType.Informix:
                        return "MySubString";
                    case DBType.Access:
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
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                    case DBType.MySQL:
                    case DBType.Informix:
                        return "||";
                    default:
                        return "+";
                }
            }
        }
        public static readonly Hashtable CS_DBConnctionDic;
        /// <summary>
        /// 组织结构集成模式
        /// </summary>
        public static Boolean OrganizationIsView
        {
            get
            {
                string val = AppSettings["OrganizationIsView"];
                if (val == "0")
                    return false;
                else
                    return true;
            }
        }

        public static string DateType
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKey("DateType", "varchar");
            }
        }
        #endregion xx

    }
}