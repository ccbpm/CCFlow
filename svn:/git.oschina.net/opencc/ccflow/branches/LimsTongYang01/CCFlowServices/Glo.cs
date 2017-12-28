using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections;
using System.IO;
using System.Data;
using System.Windows.Forms;
using BP.WF;
using BP.Sys;
using BP;
using BP.En;
using System.Data.Sql;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace SMSServices
{
    public class Glo
    {
        #region 向CCIM发送消息
        /// <summary>
        /// 产生消息,userid是为了保证写入消息的唯一性，receiveid才是真正的接收者
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="now"></param>
        /// <param name="msg"></param>
        /// <param name="sendToEmpNo">接受人</param>
        public static void SendMessage_CCIM(string userid, string now, string msg, string sendToEmpNo)
        {
            //保存系统通知消息
            StringBuilder strHql1 = new StringBuilder();

            //加密处理
             //msg = CCFlowServices.SecurityDES.Encrypt(msg);

            //BP.IM.RecordMsg rm = new BP.IM.RecordMsg();
            //rm.MyPK = userid; // 主键(保障消息的唯一性.)
            //rm.SendID = "SYSTEM"; //发送人.
            //rm.MsgDateTime = now; //发送日期.
            //rm.MsgContent = msg; // 消息内容.
            //rm.FontName = "宋体";
            //rm.FontSize = 10;
            //rm.FontBold = false;
            //rm.InfoClass = 15;
            //rm.GroupID = -1;
            //rm.Insert();

            //strHql1.Append("Insert into " + BP.WF.Glo.CCIMDBName + ".RecordMsg ([sendID],[msgDateTime],[msgContent],[ImageInfo],[fontName],[fontSize],[fontBold],");
            //strHql1.Append("[fontColor],[InfoClass],[GroupID],[SendUserID]) values(");
            //strHql1.Append("'SYSTEM',");
            //strHql1.Append("'").Append(now).Append("',");
            //strHql1.Append("'").Append(msg).Append("',");
            //strHql1.Append("'',");
            //strHql1.Append("'宋体',");
            //strHql1.Append("10,");
            //strHql1.Append("0,");
            //strHql1.Append("-16777216,");
            //strHql1.Append("15,");
            //strHql1.Append("-1,");
            //strHql1.Append("'").Append(userid).Append("')");
            //BP.DA.DBAccess.RunSQL(strHql1.ToString());

            //取出刚保存的msgID.
            int msgID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT MsgID FROM RecordMsg WHERE SendID='SYSTEM' AND MsgDateTime='" + now + "' AND SendUserID='" + userid + "'", 0 );

            if (msgID != 0)
            {
                //保存消息发送对象
                string sql = "INSERT INTO RecordMsgUser (MsgId,ReceiveID) VALUES (" + msgID + ", '" + sendToEmpNo + "' )";
                BP.DA.DBAccess.RunSQL(sql);
            }
        }
        #endregion

        public static bool IsExitProcess(string name)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process pro in processes)
            {
                if (pro.ProcessName + ".exe" == name)
                    return true;
            }
            return false;
        }
        public static bool KillProcess(string name)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process pro in processes)
            {
                if (pro.ProcessName == name)
                {
                    pro.Kill();
                    return true;
                }
            }
            return false;
        }
        public static string PathOfVisualFlow
        {
            get
            {
                //return @"D:\ccflow\VisualFlow";
                string path= Application.StartupPath + @"\.\..\..\..\CCFlow\";
                if (System.IO.Directory.Exists(path) == false)
                    throw new Exception("@没有找到web的应用程序文件夹，此程序需要读取web.config文件才能运行。");
                return path;
            }
        }
        public static void LoadConfigByFile()
        {
            // BP.WF.Glo.IntallPath = PathOfVisualFlow;

            BP.Sys.SystemConfig.IsBSsystem_Test = false;
            BP.Sys.SystemConfig.IsBSsystem = false;
            SystemConfig.IsBSsystem = false;

            string path = PathOfVisualFlow + "\\web.config"; //如果有这个文件就装载它。
            if (System.IO.File.Exists(path) == false)
            {
                MessageBox.Show("配置文件没有找到:" + path);
                return;
                //throw new Exception("配置文件没有找到:" + path);
            }

            ClassFactory.LoadConfig(path);
            try
            {
                try
                {
                    BP.Port.Emp em = new BP.Port.Emp("admin");
                }
                catch
                {
                    BP.Port.Emp em = new BP.Port.Emp("admin");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接数据库出现异常:" + ex.Message);
                return;
            }

            SystemConfig.IsBSsystem_Test = false;
            SystemConfig.IsBSsystem = false;
            SystemConfig.IsBSsystem = false;
            //   BP.Win.WF.Global.FlowImagePath = BP.WF.Global.PathOfVisualFlow + "\\Data\\FlowDesc\\";
            BP.Web.WebUser.SysLang = "CH";

            BP.Sys.SystemConfig.IsBSsystem_Test = false;
            BP.Sys.SystemConfig.IsBSsystem = false;
            SystemConfig.IsBSsystem = false;
        }
    }
}
