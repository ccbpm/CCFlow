using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using BP.Sys;
using BP.DA;
using BP.Web;

namespace BP.Sys
{
    /// <summary>
    /// 公用的静态方法.
    /// </summary>
    public class Glo
    {
        #region 业务单元.
        private static Hashtable Htable_BuessUnit = null;
        /// <summary>
        /// 获得节点事件实体
        /// </summary>
        /// <param name="enName">实例名称</param>
        /// <returns>获得节点事件实体,如果没有就返回为空.</returns>
        public static BuessUnitBase GetBuessUnitEntityByEnName(string enName)
        {
            if (Htable_BuessUnit == null || Htable_BuessUnit.Count == 0)
            {
                Htable_BuessUnit = new Hashtable();
                ArrayList al = BP.En.ClassFactory.GetObjects("BP.Sys.BuessUnitBase");
                foreach (BuessUnitBase en in al)
                {
                    Htable_BuessUnit.Add(en.ToString(), en);
                }
            }

            BuessUnitBase myen = Htable_BuessUnit[enName] as BuessUnitBase;
            if (myen == null)
            {
                //throw new Exception("@根据类名称获取业务单元实例出现错误:" + enName + ",没有找到该类的实体.");
                BP.DA.Log.DefaultLogWriteLineError("@根据类名称获取业务单元实例出现错误:" + enName + ",没有找到该类的实体.");
                return null;
            }
            return myen;
        }
        /// <summary>
        /// 获得事件实体String，根据编号或者流程标记
        /// </summary>
        /// <param name="flowMark">流程标记</param>
        /// <param name="flowNo">流程编号</param>
        /// <returns>null, 或者流程实体.</returns>
        public static string GetBuessUnitEntityStringByFlowMark(string flowMark, string flowNo)
        {
            BuessUnitBase en = GetBuessUnitEntityByFlowMark(flowMark, flowNo);
            if (en == null)
                return "";
            return en.ToString();
        }
        /// <summary>
        /// 获得业务单元.
        /// </summary>
        /// <param name="flowMark">流程标记</param>
        /// <param name="flowNo">流程编号</param>
        /// <returns>null, 或者流程实体.</returns>
        public static BuessUnitBase GetBuessUnitEntityByFlowMark(string flowMark, string flowNo)
        {

            if (Htable_BuessUnit == null || Htable_BuessUnit.Count == 0)
            {
                Htable_BuessUnit = new Hashtable();
                ArrayList al = BP.En.ClassFactory.GetObjects("BP.Sys.BuessUnitBase");
                Htable_BuessUnit.Clear();
                foreach (BuessUnitBase en in al)
                {
                    Htable_BuessUnit.Add(en.ToString(), en);
                }
            }

            foreach (string key in Htable_BuessUnit.Keys)
            {
                BuessUnitBase fee = Htable_BuessUnit[key] as BuessUnitBase;
                if (fee.ToString() == flowMark || fee.ToString().Contains("," + flowNo + ",") == true)
                    return fee;
            }
            return null;
        }
        #endregion 业务单元.

        #region 与 表单 事件实体相关.
        private static Hashtable Htable_FormFEE = null;
        /// <summary>
        /// 获得节点事件实体
        /// </summary>
        /// <param name="enName">实例名称</param>
        /// <returns>获得节点事件实体,如果没有就返回为空.</returns>
        public static FormEventBase GetFormEventBaseByEnName(string enName)
        {
            if (Htable_FormFEE == null)
            {
                Htable_FormFEE = new Hashtable();

                ArrayList al = BP.En.ClassFactory.GetObjects("BP.Sys.FormEventBase");
                Htable_FormFEE.Clear();

                foreach (FormEventBase en in al)
                {
                    Htable_FormFEE.Add(en.FormMark, en);
                }
            }

            foreach (string key in Htable_FormFEE.Keys)
            {
                FormEventBase fee = Htable_FormFEE[key] as FormEventBase;

                if (key.Contains(","))
                {
                    if (key.IndexOf(enName + ",") >= 0 || key.Length == key.IndexOf("," + enName) + enName.Length + 1)
                        return fee;
                }

                if (key == enName)
                    return fee;
            }

            return null;

        }
        #endregion 与 表单 事件实体相关.

        #region 与 表单从表 事件实体相关.
        private static Hashtable Htable_FormFEEDtl = null;
        /// <summary>
        /// 获得节点事件实体
        /// </summary>
        /// <param name="enName">实例名称</param>
        /// <returns>获得节点事件实体,如果没有就返回为空.</returns>
        public static FormEventBaseDtl GetFormDtlEventBaseByEnName(string dtlEnName)
        {
            if (Htable_FormFEEDtl == null || Htable_FormFEEDtl.Count == 0)
            {
                Htable_FormFEEDtl = new Hashtable();
                ArrayList al = BP.En.ClassFactory.GetObjects("BP.Sys.FormEventBaseDtl");
                Htable_FormFEEDtl.Clear();
                foreach (FormEventBaseDtl en in al)
                {
                    Htable_FormFEEDtl.Add(en.FormDtlMark, en);
                }
            }

            foreach (string key in Htable_FormFEEDtl.Keys)
            {
                FormEventBaseDtl fee = Htable_FormFEEDtl[key] as FormEventBaseDtl;
                if (fee.FormDtlMark.IndexOf(dtlEnName) >= 0 || fee.FormDtlMark == dtlEnName)
                    return fee;
            }
            return null;
        }
        #endregion 与 表单 事件实体相关.

        #region 公共变量.
        public static string Plant = "CCFlow";
        /// <summary>
        /// 部门版本号
        /// </summary>
        public static string DeptsVersion
        {
            get
            {
                GloVar en = new GloVar();
                en.No = "DeptsVersion";
                if (en.RetrieveFromDBSources() == 0)
                {
                    en.Name = "部门版本号";
                    en.Val = BP.DA.DataType.CurrentDataTime;
                    en.GroupKey = "Glo";
                    en.Insert();
                }
                return en.Val;
            }
        }
        /// <summary>
        /// 部门数量 - 用于显示ccim的下载进度.
        /// </summary>
        public static int DeptsCount
        {
            get
            {
                return BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) as Num FROM Port_Dept");
            }
        }
        /// <summary>
        /// 人员数量 - 用于显示ccim的下载进度.
        /// </summary>
        public static int EmpsCount
        {
            get
            {
                return BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(a.No) as Num FROM Port_Emp a, Port_Dept b WHERE A.FK_Dept=B.No AND A.No NOT IN ('admin','Guest')");
            }
        }
        /// <summary>
        /// 人员版本号
        /// </summary>
        public static string UsersVersion
        {
            get
            {
                GloVar en = new GloVar();
                en.No = "UsersVersion";
                if (en.RetrieveFromDBSources() == 0)
                {
                    en.Name = "人员版本号";
                    en.Val = BP.DA.DataType.CurrentDataTime;
                    en.GroupKey = "Glo";
                    en.Insert();
                }
                return en.Val;
            }
        }
        #endregion 公共变量.

        #region 写入系统日志(写入的文件:\DataUser\Log\*.*)
        /// <summary>
        /// 写入一条消息
        /// </summary>
        /// <param name="msg">消息</param>
        public static void WriteLineInfo(string msg)
        {
            BP.DA.Log.DefaultLogWriteLineInfo(msg);
        }
        /// <summary>
        /// 写入一条警告
        /// </summary>
        /// <param name="msg">消息</param>
        public static void WriteLineWarning(string msg)
        {
            BP.DA.Log.DefaultLogWriteLineWarning(msg);
        }
        /// <summary>
        /// 写入一条错误
        /// </summary>
        /// <param name="msg">消息</param>
        public static void WriteLineError(string msg)
        {
            BP.DA.Log.DefaultLogWriteLineError(msg);
        }
        #endregion 写入系统日志

        #region 写入用户日志(写入的用户表:Sys_UserLog).
        /// <summary>
        /// 写入用户日志
        /// </summary>
        /// <param name="logType">类型</param>
        /// <param name="empNo">操作员编号</param>
        /// <param name="msg">信息</param>
        /// <param name="ip">IP</param>
        public static void WriteUserLog(string logType, string empNo, string msg, string ip)
        {
            UserLog ul = new UserLog();
            ul.MyPK = DBAccess.GenerGUID();
            ul.FK_Emp = empNo;
            ul.LogFlag = logType;
            ul.Docs = msg;
            ul.IP = ip;
            ul.RDT = DataType.CurrentDataTime;
            ul.Insert();
        }
        /// <summary>
        /// 写入用户日志
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="empNo">操作员编号</param>
        /// <param name="msg">消息</param>
        public static void WriteUserLog(string logType, string empNo, string msg)
        {
            //为了提高运行效率，把这个地方屏蔽了。
            return;
            /*
            UserLog ul = new UserLog();
            ul.MyPK = DBAccess.GenerGUID();
            ul.FK_Emp = empNo;
            ul.LogFlag = logType;
            ul.Docs = msg;
            ul.RDT = DataType.CurrentDataTime;
            try
            {
                if (BP.Sys.SystemConfig.IsBSsystem)
                    ul.IP = HttpContextHelper.Request.UserHostAddress;
            }
            catch
            {
            }
            ul.Insert();
            */
        }
        #endregion 写入用户日志.

        /// <summary>
        /// 初始化附件信息
        /// 如果手工的上传的附件，就要把附加的信息映射出来.
        /// </summary>
        /// <param name="en"></param>
        public static void InitEntityAthInfo(BP.En.Entity en)
        {
            //求出保存路径.
            string path = en.EnMap.FJSavePath;
            if (path == "" || path == null || path == string.Empty)
                path = BP.Sys.SystemConfig.PathOfDataUser + en.ToString() + "\\";

            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);

            //获得该目录下所有的文件.
            string[] strs = System.IO.Directory.GetFiles(path);
            string pkval = en.PKVal.ToString();
            string myfileName = null;
            foreach (string str in strs)
            {
                if (str.Contains(pkval + ".") == false)
                    continue;

                myfileName = str;
                break;
            }

            if (myfileName == null)
                return;

            /* 如果包含这二个字段。*/
            string fileName = myfileName;
            fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);

            en.SetValByKey("MyFilePath", path);

            string ext = "";
            if (fileName.IndexOf(".") != -1)
                ext = fileName.Substring(fileName.LastIndexOf(".") + 1);

            string reldir = path;
            if (reldir.Length > SystemConfig.PathOfDataUser.Length)
                reldir =
                    reldir.Substring(reldir.ToLower().IndexOf(@"\datauser\") + @"\datauser\".Length).Replace(
                        @"\", "/");
            else
                reldir = "";

            if (reldir.Length > 0 && Equals(reldir[0], '/') == true)
                reldir = reldir.Substring(1);

            if (reldir.Length > 0 && Equals(reldir[reldir.Length - 1], '/') == false)
                reldir += "/";

            en.SetValByKey("MyFileExt", ext);
            en.SetValByKey("MyFileName", fileName);
            en.SetValByKey("WebPath", "/DataUser/" + reldir + en.PKVal + "." + ext);

            string fullFile = path + @"\" + en.PKVal + "." + ext;

            System.IO.FileInfo info = new System.IO.FileInfo(fullFile);
            en.SetValByKey("MyFileSize", BP.DA.DataType.PraseToMB(info.Length));
            if (DataType.IsImgExt(ext))
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(fullFile);
                en.SetValByKey("MyFileH", img.Height);
                en.SetValByKey("MyFileW", img.Width);
                img.Dispose();
            }
            en.Update();
        }


        /// <summary>
        /// 产生消息,senderEmpNo是为了保证写入消息的唯一性，receiveid才是真正的接收者.
        /// 如果插入失败.
        /// </summary>
        /// <param name="fromEmpNo">发送人</param>
        /// <param name="now">发送时间</param>
        /// <param name="msg">消息内容</param>
        /// <param name="sendToEmpNo">接受人</param>
        public static void SendMessageToCCIM(string fromEmpNo, string sendToEmpNo, string msg, string now)
        {
            //暂停对ccim消息提醒的支持.
            return;
        }
        /// <summary>
        /// 处理生成提示信息,不友好的提示.
        /// </summary>
        /// <param name="alertInfo">从系统里抛出的错误信息.</param>
        /// <returns>返回的友好提示信息.</returns>
        public static string GenerFriendlyAlertHtmlInfo(string alertInfo)
        {
            // 格式为: err@错误中文提示信息. tech@info 数据库错误,查询sqL为.
            return alertInfo;
        }

        #region 加密解密文件.
        public static void File_JiaMi(string fileFullPath)
        {
            //南京宝旺达.
            if (SystemConfig.CustomerNo == "BWDA")
            {

            }
        }
        public static void File_JieMi(string fileFullPath)
        {
            //南京宝旺达.
            if (SystemConfig.CustomerNo == "BWDA")
            {

            }
        }
        /// <summary>
        /// 字符串的解密
        /// </summary>
        /// <param name="str">加密的字符串</param>
        /// <returns>返回解密后的字符串</returns>
        public static string String_JieMi(string str)
        {
            //南京宝旺达.
            if (SystemConfig.CustomerNo == "BWDA")
            {
                return str;
            }

            return str;
        }
        #endregion 加密解密文件.

    }
}
