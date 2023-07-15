using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web;

namespace BP.Cloud.OrgSetting
{
    /// <summary>
    /// 组织 属性
    /// </summary>
    public class OrgMailAttr : OrgAttr
    {
        #region 基本属性
        //
        public const string EmailHostSta = "EmailHostSta";
        public const string SendEmailHost = "SendEmailHost";
        public const string SendEmailPort = "SendEmailPort";
        public const string SendEmailAddress = "SendEmailAddress";
        public const string SendEmailPass = "SendEmailPass";
        public const string SendEmailEnableSsl = "SendEmailEnableSsl";
        #endregion
    }
    /// <summary>
    /// 组织 的摘要说明。
    /// </summary>
    public class OrgMail : EntityNoName
    {
        #region 扩展属性
        public int EmailHostSta
        {
            get
            {
                return this.GetValIntByKey(OrgMailAttr.EmailHostSta);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.EmailHostSta, value);
            }
        }
        public string SendEmailHost
        {
            get
            {
                return this.GetValStrByKey(OrgMailAttr.SendEmailHost);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.SendEmailHost, value);
            }
        }
        public int SendEmailPort
        {
            get
            {
                return this.GetValIntByKey(OrgMailAttr.SendEmailPort);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.SendEmailPort, value);
            }
        }
        public string SendEmailAddress
        {
            get
            {
                return this.GetValStrByKey(OrgMailAttr.SendEmailAddress);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.SendEmailAddress, value);
            }
        }
        public string SendEmailPass
        {
            get
            {
                return this.GetValStrByKey(OrgMailAttr.SendEmailPass);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.SendEmailPass, value);
            }
        }
        public bool SendEmailEnableSsl
        {
            get
            {
                return this.GetValBooleanByKey(OrgMailAttr.SendEmailEnableSsl);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.SendEmailEnableSsl, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 组织
        /// </summary>
        public OrgMail()
        {
        }
        public OrgMail(string no)
        {
            this.No = no;
            this.Retrieve();

            //如果使用全局的配置.
            if (this.EmailHostSta == 0)
            {
                //邮件地址.
                string emailAddr =  BP.Difference.SystemConfig.GetValByKey("SendEmailAddress", null);
                if (emailAddr == null)
                    emailAddr = "ccbpmtester@tom.com";

                string emailPassword =  BP.Difference.SystemConfig.GetValByKey("SendEmailPass", null);
                if (emailPassword == null)
                    emailPassword = "ccbpm123";

                this.SendEmailAddress = emailAddr;
                this.SendEmailPass = emailPassword;

                if (BP.Difference.SystemConfig.GetValByKeyInt("SendEmailEnableSsl", 1) == 1)
                    this.SendEmailEnableSsl = true;  //经过ssl加密.
                else
                    this.SendEmailEnableSsl = false;


                this.SendEmailPort =  BP.Difference.SystemConfig.GetValByKeyInt("SendEmailPort", 587); //使用的端口
                this.SendEmailHost =  BP.Difference.SystemConfig.GetValByKey("SendEmailHost", "smtp.gmail.com");
            }


        }
        /// <summary>
        /// 权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

                if (WebUser.No.Equals("admin") == true)
                {
                    uac.OpenAll();
                    uac.IsInsert = false;
                    uac.IsDelete = false;
                    return uac;
                }

                uac.IsInsert = false;
                uac.IsDelete = false;
                if (this.No.Equals(WebUser.OrgNo) == true)
                {
                    uac.IsUpdate = true;
                    return uac;
                }

                //删除.
                uac.IsInsert = false;
                uac.IsDelete = false;
                uac.IsView = false;
                return uac;
            }
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Org", "邮件服务器设置");
                map.setEnType(EnType.App);

                #region 字段.
                /*关于字段属性的增加 */
                map.AddTBStringPK(OrgMailAttr.No, null, "OrgNo", true, true, 1, 50, 90);
                map.AddTBString(OrgMailAttr.Name, null, "简称", true, true, 0, 200, 130);

                map.AddDDLSysEnum(OrgMailAttr.EmailHostSta, 0, "邮件服务器规则", true, true,
                    OrgMailAttr.EmailHostSta, "@0=使用系统全局设置@1=使用本组织设置@2=禁用");

                map.AddBoolean(OrgMailAttr.SendEmailEnableSsl, false, "邮件服务器是否启用Ssl?", true, true);
                map.AddTBString(OrgMailAttr.SendEmailHost, null, "EmailHost", true, false, 0, 50, 50, true);
                map.AddTBString(OrgMailAttr.SendEmailPort, null, "EmailPort", true, false, 0, 50, 50, true);
                map.AddTBString(OrgMailAttr.SendEmailAddress, null, "EmailAddress", true, false, 0, 50, 50, true);
                map.AddTBString(OrgMailAttr.SendEmailPass, null, "EmailPass", true, false, 0, 50, 50, true);
                #endregion 字段.

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email"></param>
        /// <param name="title"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public string SendEmail(string mail, string mailTitle, string mailDoc)
        {
            BP.Cloud.OrgSetting.OrgMail orgEmail = new BP.Cloud.OrgSetting.OrgMail(BP.Web.WebUser.OrgNo);
            //如果是禁用状态就返回过去.
            if (orgEmail.EmailHostSta == 2)
                return "info@禁用状态";

            System.Net.Mail.MailMessage myEmail = new System.Net.Mail.MailMessage();

            mailDoc = DataType.ParseText2Html(mailDoc);

            string displayName =  BP.Difference.SystemConfig.GetValByKey("SendEmailDisplayName", "科伦BPM");
            myEmail.From = new System.Net.Mail.MailAddress(orgEmail.SendEmailAddress);

            myEmail.To.Add(mail);
            myEmail.Subject = mailTitle;
            myEmail.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码
            myEmail.IsBodyHtml = true;
            myEmail.Body = mailDoc;
            myEmail.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
            myEmail.IsBodyHtml = true;//是否是HTML邮件
            myEmail.Priority = System.Net.Mail.MailPriority.High; // 邮件优先级

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.UseDefaultCredentials = true;
            client.EnableSsl = orgEmail.SendEmailEnableSsl;

            client.Credentials = new System.Net.NetworkCredential(orgEmail.SendEmailAddress,
                orgEmail.SendEmailPass);

            //赋值端口与主机.
            client.Port = orgEmail.SendEmailPort; //使用的端口
            client.Host = orgEmail.SendEmailHost;

            object userState = myEmail;
            //调用自带的异步方法
            //client.Send(myEmail);
            try
            {
                client.Send(myEmail);
                //object userState = myEmail;
                // client.SendAsync(myEmail, userState);
                //client.SendMailAsync(myEmail);
                return "发送成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            //client.SendMailAsync(myEmail);
            // client.SendAsync(myEmail, userState);
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new OrgMails(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 组织s
    // </summary>
    public class OrgMails : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new OrgMail();
            }
        }
        /// <summary>
        /// 组织s
        /// </summary>
        public OrgMails()
        {
        }
        #endregion 构造方法

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<OrgMail> ToJavaList()
        {
            return (System.Collections.Generic.IList<OrgMail>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<OrgMail> Tolist()
        {
            System.Collections.Generic.List<OrgMail> list = new System.Collections.Generic.List<OrgMail>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((OrgMail)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
