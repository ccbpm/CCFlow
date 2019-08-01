using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF.Port;
using System.Net.Mail;

namespace BP.WF
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public class SMSMsgType
    {
        /// <summary>
        /// 自定义消息
        /// </summary>
        public const string Self = "Self";
        /// <summary>
        /// 抄送消息
        /// </summary>
        public const string CC = "CC";
        /// <summary>
        /// 待办消息
        /// </summary>
        public const string SendSuccess = "SendSuccess";
        /// <summary>
        /// 其他
        /// </summary>
        public const string Etc = "Etc";
        /// <summary>
        /// 退回消息
        /// </summary>
        public const string ReturnAfter = "ReturnAfter";
        /// <summary>
        /// 移交消息
        /// </summary>
        public const string Shift = "Shift";
        /// <summary>
        /// 加签消息
        /// </summary>
        public const string AskFor = "AskFor";
        /// <summary>
        /// 挂起消息
        /// </summary>
        public const string HungUp = "HangUp";
        /// <summary>
        /// 催办消息
        /// </summary>
        public const string DoPress = "DoPress";
        /// <summary>
        /// 错误信息
        /// </summary>
        public const string Err = "Err";
    }
    /// <summary>
    /// 消息状态
    /// </summary>
    public enum MsgSta
    {
        /// <summary>
        /// 未开始
        /// </summary>
        UnRun,
        /// <summary>
        /// 成功
        /// </summary>
        RunOK,
        /// <summary>
        /// 失败
        /// </summary>
        RunError,
        /// <summary>
        /// 禁止发送
        /// </summary>
        Disable
    }
    /// <summary>
    /// 消息属性
    /// </summary>
    public class SMSAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 消息标记（有此标记的不在发送）
        /// </summary>
        public const string MsgFlag = "MsgFlag";
        /// <summary>
        /// 状态 0 未发送， 1 发送成功，2发送失败.
        /// </summary>
        public const string EmailSta = "EmailSta";
        /// <summary>
        /// 邮件
        /// </summary>
        public const string Email = "Email";
        /// <summary>
        /// 邮件标题
        /// </summary>
        public const string EmailTitle = "EmailTitle";
        /// <summary>
        /// 邮件内容
        /// </summary>
        public const string EmailDoc = "EmailDoc";
        /// <summary>
        /// 发送人
        /// </summary>
        public const string Sender = "Sender";
        /// <summary>
        /// 发送给
        /// </summary>
        public const string SendTo = "SendTo";
        /// <summary>
        /// 插入日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 发送日期
        /// </summary>
        public const string SendDT = "SendDT";
        /// <summary>
        /// 是否读取
        /// </summary>
        public const string IsRead = "IsRead";
        /// <summary>
        /// 状态 0 未发送， 1 发送成功，2发送失败.
        /// </summary>
        public const string MobileSta = "MobileSta";
        /// <summary>
        /// 手机
        /// </summary>
        public const string Mobile = "Mobile";
        /// <summary>
        /// 手机信息
        /// </summary>
        public const string MobileInfo = "MobileInfo";
        /// <summary>
        /// 是否提示过了
        /// </summary>
        public const string IsAlert = "IsAlert";
        /// <summary>
        /// 消息类型
        /// </summary>
        public const string MsgType = "MsgType";
        /// <summary>
        /// 其他参数.
        /// </summary>
        public const string Paras = "Paras";
        /// <summary>
        /// 打开的连接
        /// </summary>
        public const string OpenUrl = "OpenUrl";

    }
    /// <summary>
    /// 消息
    /// </summary> 
    public class SMS : EntityMyPK
    {
        #region 新方法 2013
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="userNo">接受人</param>
        /// <param name="msgTitle">标题</param>
        /// <param name="msgDoc">内容</param>
        /// <param name="msgFlag">标记</param>
        /// <param name="msgType">类型</param>
        /// <param name="paras">扩展参数</param>
        public static void SendMsg(string userNo, string msgTitle, string msgDoc, string msgFlag,
            string msgType, string paras)
        {

            SMS sms = new SMS();
            sms.MyPK = DBAccess.GenerGUID();
            sms.HisEmailSta = MsgSta.UnRun;

            sms.Sender = WebUser.No;
            sms.SendToEmpNo = userNo;

            sms.Title = msgTitle;
            sms.DocOfEmail = msgDoc;

            sms.Sender = BP.Web.WebUser.No;
            sms.RDT = BP.DA.DataType.CurrentDataTime;

            sms.MsgFlag = msgFlag; // 消息标志.
            sms.MsgType = msgType; // 消息类型.'

            sms.AtPara = paras;
            sms.Insert();
        }
        #endregion 新方法

        #region 手机短信属性
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.Mobile);
            }
            set
            {
                SetValByKey(SMSAttr.Mobile, value);
            }
        }
        /// <summary>
        /// 手机状态
        /// </summary>
        public MsgSta HisMobileSta
        {
            get
            {
                return (MsgSta)this.GetValIntByKey(SMSAttr.MobileSta);
            }
            set
            {
                SetValByKey(SMSAttr.MobileSta, (int)value);
            }
        }
        /// <summary>
        /// 手机信息
        /// </summary>
        public string MobileInfo
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.MobileInfo);
            }
            set
            {
                SetValByKey(SMSAttr.MobileInfo, value);
            }
        }
        #endregion

        #region  邮件属性
        /// <summary>
        /// 参数
        /// </summary>
        public string AtPara
        {
            get
            {
                return this.GetValStrByKey("AtPara", "");
            }
            set
            {
                this.SetValByKey("AtPara", value);
            }
        }
        /// <summary>
        /// 邮件状态
        /// </summary>
        public MsgSta HisEmailSta
        {
            get
            {
                return (MsgSta)this.GetValIntByKey(SMSAttr.EmailSta);
            }
            set
            {
                this.SetValByKey(SMSAttr.EmailSta, (int)value);
            }
        }
        /// <summary>
        /// Email
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.Email);
            }
            set
            {
                SetValByKey(SMSAttr.Email, value);
            }
        }
        /// <summary>
        /// 发送给
        /// </summary>
        public string SendToEmpNo
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.SendTo);
            }
            set
            {
                SetValByKey(SMSAttr.SendTo, value);
            }
        }
        public int IsRead
        {
            get
            {
                return this.GetValIntByKey(SMSAttr.IsRead);
            }
            set
            {
                this.SetValByKey(SMSAttr.IsRead, (int)value);
            }
        }
        public int IsAlert
        {
            get
            {
                return this.GetValIntByKey(SMSAttr.IsAlert);
            }
            set
            {
                this.SetValByKey(SMSAttr.IsAlert, (int)value);
            }
        }
        /// <summary>
        /// 消息标记(可以用它来避免发送重复)
        /// </summary>
        public string MsgFlag
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.MsgFlag);
            }
            set
            {
                SetValByKey(SMSAttr.MsgFlag, value);
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string MsgType
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.MsgType);
            }
            set
            {
                SetValByKey(SMSAttr.MsgType, value);
            }
        }
        /// <summary>
        /// 发送人
        /// </summary>
        public string Sender
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.Sender);
            }
            set
            {
                SetValByKey(SMSAttr.Sender, value);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.RDT);
            }
            set
            {
                this.SetValByKey(SMSAttr.RDT, value);
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.EmailTitle);
            }
            set
            {
                SetValByKey(SMSAttr.EmailTitle, value);
            }
        }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string DocOfEmail
        {
            get
            {
                string doc = this.GetValStringByKey(SMSAttr.EmailDoc);
                if (DataType.IsNullOrEmpty(doc))
                    return this.Title;

                return doc.Replace('~', '\'');
            }
            set
            {
                SetValByKey(SMSAttr.EmailDoc, value);
            }
        }
        /// <summary>
        /// 邮件内容.
        /// </summary>
        public string Doc
        {
            get
            {
                string doc = this.GetValStringByKey(SMSAttr.EmailDoc);
                if (DataType.IsNullOrEmpty(doc))
                    return this.Title;
                return doc.Replace('~', '\'');

                return this.DocOfEmail;
            }
            set
            {
                SetValByKey(SMSAttr.EmailDoc, value);
            }
        }
        /// <summary>
        /// 打开的连接
        /// </summary>
        public string OpenURL
        {
            get
            {
                return this.GetParaString(SMSAttr.OpenUrl);
            }
            set
            {
                this.SetPara(SMSAttr.OpenUrl, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenAll();
                return uac;
            }
        }
        /// <summary>
        /// 消息
        /// </summary>
        public SMS()
        {
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_SMS", "消息");

                map.AddMyPK();

                map.AddTBString(SMSAttr.Sender, null, "发送人(可以为空)", false, true, 0, 200, 20);
                map.AddTBString(SMSAttr.SendTo, null, "发送给(可以为空)", false, true, 0, 200, 20);
                map.AddTBDateTime(SMSAttr.RDT, "写入时间", true, false);

                map.AddTBString(SMSAttr.Mobile, null, "手机号(可以为空)", false, true, 0, 30, 20);
                map.AddTBInt(SMSAttr.MobileSta, (int)MsgSta.UnRun, "消息状态", true, true);
                map.AddTBString(SMSAttr.MobileInfo, null, "短信信息", false, true, 0, 1000, 20);

                map.AddTBString(SMSAttr.Email, null, "Email(可以为空)", false, true, 0, 200, 20);
                map.AddTBInt(SMSAttr.EmailSta, (int)MsgSta.UnRun, "EmaiSta消息状态", true, true);
                map.AddTBString(SMSAttr.EmailTitle, null, "标题", false, true, 0, 3000, 20);
                map.AddTBStringDoc(SMSAttr.EmailDoc, null, "内容", false, true);
                map.AddTBDateTime(SMSAttr.SendDT, null, "发送时间", false, false);

                map.AddTBInt(SMSAttr.IsRead, 0, "是否读取?", true, true);
                map.AddTBInt(SMSAttr.IsAlert, 0, "是否提示?", true, true);

                map.AddTBString(SMSAttr.MsgFlag, null, "消息标记(用于防止发送重复)", false, true, 0, 200, 20);
                map.AddTBString(SMSAttr.MsgType, null, "消息类型(CC抄送,Todolist待办,Return退回,Etc其他消息...)", false, true, 0, 200, 20);

                //其他参数.
                map.AddTBAtParas(500);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="mailTitle"></param>
        /// <param name="mailDoc"></param>
        /// <returns></returns>
        public static bool SendEmailNow(string mail, string mailTitle, string mailDoc)
        {
            System.Net.Mail.MailMessage myEmail = new System.Net.Mail.MailMessage();
            //邮件地址.
            string emailAddr = SystemConfig.GetValByKey("SendEmailAddress", null);
            if (emailAddr == null)
                emailAddr = "ccbpmtester@tom.com";

            string emailPassword = SystemConfig.GetValByKey("SendEmailPass", null);
            if (emailPassword == null)
                emailPassword = "ccbpm123";

            myEmail.From = new System.Net.Mail.MailAddress(emailAddr, "ccbpm", System.Text.Encoding.UTF8);

            myEmail.To.Add(mail);
            myEmail.Subject = mailTitle;
            myEmail.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码

           myEmail.IsBodyHtml=true;

           mailDoc = BP.DA.DataType.ParseText2Html(mailDoc);

            myEmail.Body = mailDoc;
            myEmail.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
            myEmail.IsBodyHtml = true;//是否是HTML邮件

            myEmail.Priority = MailPriority.High;//邮件优先级

            SmtpClient client = new SmtpClient();

            //是否启用ssl? 
            bool isEnableSSL = false;
            string emailEnableSSL = SystemConfig.GetValByKey("SendEmailEnableSsl", null);
            if (emailEnableSSL == null || emailEnableSSL == "0")
                isEnableSSL = false;
            else
                isEnableSSL = true;

            client.Credentials = new System.Net.NetworkCredential(emailAddr, emailPassword);

            //上述写你的邮箱和密码
            client.Port = SystemConfig.GetValByKeyInt("SendEmailPort", 587); //使用的端口
            client.Host = SystemConfig.GetValByKey("SendEmailHost", "smtp.gmail.com");

            // 经过ssl加密. 
            if (SystemConfig.GetValByKeyInt("SendEmailEnableSsl", 1) == 1)
                client.EnableSsl = true;  //经过ssl加密.
            else
                client.EnableSsl = false; //经过ssl加密.

            try
            {
                object userState = myEmail;
                client.SendAsync(myEmail, userState);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 插入之后执行的方法.
        /// </summary>
        protected override void afterInsert()
        {
            try
            {
                CCInterface.PortalInterfaceSoapClient soap = null;
                if (this.HisEmailSta == MsgSta.UnRun)
                {
                    /*发送邮件*/
                    if (DataType.IsNullOrEmpty(this.Email) == true)
                        return;

                    string emailStrs = this.Email;
                    emailStrs = emailStrs.Replace(",", ";");
                    emailStrs = emailStrs.Replace("，", ";");

                    //包含多个邮箱
                    if (emailStrs.Contains(";") == true)
                    {
                        string[] emails = emailStrs.Split(';');
                        foreach (string email in emails)
                        {
                            if (DataType.IsNullOrEmpty(email) == true)
                                continue;

                            SendEmailNow(email, this.Title, this.DocOfEmail);
                        }
                    }
                    else
                    {   //单个邮箱
                        SendEmailNow(this.Email, this.Title, this.DocOfEmail);
                    }
                    return;
                }

                if (this.HisMobileSta == MsgSta.UnRun)
                {
                    string tag = "@MsgFlag=" + this.MsgFlag + "@MsgType=" + this.MsgType + this.AtPara + "@Sender=" + this.Sender + "@SenderName=" + BP.Web.WebUser.Name;
                    string pushModel = this.GetParaString("PushModel");

                    if (DataType.IsNullOrEmpty(pushModel) == true)
                        pushModel = BP.WF.Glo.ShortMessageWriteTo.ToString();
                    string [] pushModels = pushModel.Split(',');
                    foreach (string model in pushModels)
                    {

                        switch ((ShortMessageWriteTo)Int32.Parse(model))
                        {
                            case BP.WF.ShortMessageWriteTo.ToSMSTable: //写入消息表。
                                break;
                            case BP.WF.ShortMessageWriteTo.ToWebservices: // 1 写入webservices.
                                soap = BP.WF.Glo.GetPortalInterfaceSoapClient();

                                soap.SendToWebServices(this.MyPK, WebUser.No, this.SendToEmpNo, this.Mobile, this.MobileInfo, tag, this.Title, this.OpenURL);
                                break;
                            case BP.WF.ShortMessageWriteTo.ToDingDing: // 2 写入dingding. 
                                soap = BP.WF.Glo.GetPortalInterfaceSoapClient();
                                soap.SendToDingDing(this.MyPK, WebUser.No, this.SendToEmpNo, this.Mobile, this.MobileInfo);
                                break;
                            case BP.WF.ShortMessageWriteTo.ToWeiXin: // 写入微信. 3
                                //写入微信.
                                BP.WF.WeiXin.WeiXinMessage.SendMsgToUsers(this.SendToEmpNo, this.Title, this.Doc, WebUser.No);
                                break;
                            case BP.WF.ShortMessageWriteTo.CCIM: // 写入即时通讯系统.
                                soap = BP.WF.Glo.GetPortalInterfaceSoapClient();
                                soap.SendToCCIM(this.MyPK, WebUser.No, this.SendToEmpNo, this.MobileInfo, tag);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError("@消息机制没有配置成功." + ex.Message);
            }
            base.afterInsert();
        }
    }
    /// <summary>
    /// 消息s
    /// </summary> 
    public class SMSs : Entities
    {
        /// <summary>
        /// 获得实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SMS();
            }
        }
        public SMSs()
        {
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SMS> ToJavaList()
        {
            return (System.Collections.Generic.IList<SMS>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SMS> Tolist()
        {
            System.Collections.Generic.List<SMS> list = new System.Collections.Generic.List<SMS>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SMS)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
