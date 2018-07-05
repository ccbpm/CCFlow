using System;
using System.Data;
using System.Collections;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.DA;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// 读取邮件内容发起流程
    /// </summary>
    public class ReadEmailSendWork : Method
    {
        /// <summary>
        /// 读取邮件内容发起流程
        /// </summary>
        public ReadEmailSendWork()
        {
            this.Title = "读取邮件内容发起流程（根据约定的格式，读取邮件内容发送流程）";
            this.Help = "定时任务";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.No == "admin")
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string info = "开始为Track表读取邮件内容发起流程.";
            string userName = "";
            string passWord = "";
            string popServer = "";
            int port = 21;

            //POP3 NewMail = new POP3();
            //NewMail.Connect(userName, passWord, popServer, port);

            //ArrayList arr = new ArrayList();

            //for (int i = 1; i <= NewMail.Count; i++)
            //{
            //    //判断是否跟当前最大的时间作比较，大于当前时间就处理
            //    DateTime CurrentEmailDate = DateTime.Now;
            //    try
            //    {
            //        CurrentEmailDate = DateTime.Parse(NewMail.Messages[i].Date.ToString());
            //    }
            //    catch
            //    {
            //        continue;
            //    }

            //    try
            //    {
            //        if (CurrentEmailDate.CompareTo(MaxDate) > 0)
            //        {
            //            string EmailFuJian = "";
            //            NewMail.Messages[i].Charset = "GB2312"; //设置邮件的编码方式
            //            NewMail.Messages[i].Encoding = "Base64"; //设置邮件的附件编码方式
            //            NewMail.Messages[i].ISOEncodeHeaders = false; //是否将信头编码成iso-8859-1字符集
            //            for (int j = 0; j < NewMail.Messages[i].Attachments.Count; j++)
            //            {
            //                try
            //                {
            //                    string path = BaseUrl + "files/MailAttachments/";
            //                    if (!Directory.Exists(ShowMsgHelper.GetDBPath(path)))
            //                    {
            //                        Directory.CreateDirectory(ShowMsgHelper.GetDBPath(path));
            //                    }
            //                    string displayName = path + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + NewMail.Messages[i].Attachments[j].Name.Replace('/', '_').Replace('\'', '_');
            //                    string FileName = ShowMsgHelper.GetDBPath(displayName);
            //                    int size = NewMail.Messages[i].Attachments[j].Size;
            //                    size = size / (1024 * 1024);
            //                    int result = check_file(CheckFileName, CheckFileSize, displayName, size);
            //                    switch (result)
            //                    {
            //                        case 0:
            //                            break;
            //                        case 1:
            //                            NewMail.Messages[i].Attachments[j].SaveToFile(FileName);
            //                            if (EmailFuJian.Trim().Length > 0)
            //                            {
            //                                EmailFuJian = EmailFuJian + "|" + displayName;
            //                            }
            //                            else
            //                            {
            //                                EmailFuJian = displayName;
            //                            }
            //                            break;
            //                        case 2:
            //                            EmailFuJian = "文件太大已被过滤";
            //                            break;
            //                        case 3:
            //                            EmailFuJian = "类型不符合已被过滤";
            //                            break;
            //                    }
            //                }
            //                catch (Exception e)
            //                {
            //                    LogHelper Logger = new LogHelper("JmailLog");
            //                    Logger.WriteLog(string.Concat(new string[]
            //                    {
            //                        "-----------发送邮件-----------\r\n",
            //                        e.Message,
            //                        "\r\n"
            //                    }));
            //                    continue;
            //                }
            //            }
            //            #region 保存表 用于存储数据库
            //            Hashtable ht = new Hashtable();
            //           // ht["ID"] = RM.Common.DotNetCode.CommonHelper.GetGuid;
            //            ht["EmailTitle"] = NewMail.Messages[i].Subject;
            //            try
            //            {
            //                ht["EmailContent"] = NewMail.Messages[i].HTMLBody;
            //            }
            //            catch
            //            {
            //                ht["EmailContent"] = NewMail.Messages[i].Body;
            //            }
            //            ht["FromUser"] = NewMail.Messages[i].From;
            //            ht["FromUserName"] = NewMail.Messages[i].FromName;
            //            ht["EmailFuJian"] = EmailFuJian;
            //            try
            //            {
            //                ht["FromTime"] = DateTime.Parse(NewMail.Messages[i].Date.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            //            }
            //            catch
            //            {
            //                ht["FromTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //            }
            //            arr.Add(ht);
            //            #endregion
            //        }
            //    }
            //    catch (Exception ee)
            //    {
            //        atr

            //        continue;
            //    }
            //}
            //NewMail.Disconnect();
            return "";
        }
    }
}
