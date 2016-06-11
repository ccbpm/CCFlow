using System;
using System.Collections.Generic;
using System.Text;
using BP.Sys;
using BP.DA;

namespace BP.Sys
{
    /// <summary>
    /// 公用的静态方法.
    /// </summary>
    public class Glo
    {
        #region 公共变量.
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
            UserLog ul = new UserLog();
            ul.MyPK = DBAccess.GenerGUID();
            ul.FK_Emp = empNo;
            ul.LogFlag = logType;
            ul.Docs = msg;
            ul.RDT = DataType.CurrentDataTime;
            try
            {
                if (BP.Sys.SystemConfig.IsBSsystem)
                    ul.IP = BP.Sys.Glo.Request.UserHostAddress;
            }
            catch
            {
            }
            ul.Insert();
        }
        #endregion 写入用户日志.

        /// <summary>
        /// 获得对象.
        /// </summary>
        public static System.Web.HttpRequest Request
        {
            get
            {
                return System.Web.HttpContext.Current.Request; 
            }
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
            if (fromEmpNo == null )
                fromEmpNo = "";

            if (sendToEmpNo == null || sendToEmpNo == "")
                return;

            // throw new Exception("@接受人不能为空");

            string dbStr = SystemConfig.AppCenterDBVarStr;
            //保存系统通知消息
            StringBuilder strHql1 = new StringBuilder();
            //加密处理
            msg = BP.Tools.SecurityDES.Encrypt(msg);

            Paras ps = new Paras();
            string sql = "INSERT INTO CCIM_RecordMsg (OID,SendID,MsgDateTime,MsgContent,ImageInfo,FontName,FontSize,FontBold,FontColor,InfoClass,GroupID,SendUserID) VALUES (";
            sql += dbStr + "OID,";
            sql += "'SYSTEM',";
            sql += dbStr + "MsgDateTime,";
            sql += dbStr + "MsgContent,";
            sql += dbStr + "ImageInfo,";
            sql += dbStr + "FontName,";
            sql += dbStr + "FontSize,";
            sql += dbStr + "FontBold,";
            sql += dbStr + "FontColor,";
            sql += dbStr + "InfoClass,";
            sql += dbStr + "GroupID,";
            sql += dbStr + "SendUserID)";
            ps.SQL = sql;

            Int64 messgeID = BP.DA.DBAccess.GenerOID("RecordMsgUser");

            ps.Add("OID", messgeID);
            ps.Add("MsgDateTime", now);
            ps.Add("MsgContent", msg);
            ps.Add("ImageInfo", "");
            ps.Add("FontName", "宋体");
            ps.Add("FontSize", 10);
            ps.Add("FontBold", 0);
            ps.Add("FontColor", -16777216);
            ps.Add("InfoClass", 15);
            ps.Add("GroupID", -1);
            ps.Add("SendUserID", fromEmpNo);
            BP.DA.DBAccess.RunSQL(ps);

            //保存消息发送对象,这个是消息的接收人表.
            ps = new Paras();
            ps.SQL = "INSERT INTO CCIM_RecordMsgUser (OID,MsgId,ReceiveID) VALUES ( ";
            ps.SQL += dbStr + "OID,";
            ps.SQL += dbStr + "MsgId,";
            ps.SQL += dbStr + "ReceiveID)";

            ps.Add("OID", messgeID);
            ps.Add("MsgId", messgeID);
            ps.Add("ReceiveID", sendToEmpNo);
            BP.DA.DBAccess.RunSQL(ps);
        }
    }
}
