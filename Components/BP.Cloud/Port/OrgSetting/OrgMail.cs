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
        /// <summary>
        /// 该人员是否被禁用.
        /// </summary>
        public bool IsEnable
        {
            get
            {
                if (this.No == "admin")
                    return true;

                string sql = "SELECT COUNT(FK_OrgMail) FROM Port_DeptOrgMailStation WHERE FK_OrgMail='" + this.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return false;

                sql = "SELECT COUNT(FK_OrgMail) FROM Port_DeptOrgMail WHERE FK_OrgMail='" + this.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return false;

                return true;
            }
        }
        public string Addr
        {
            get
            {

                return this.GetValStrByKey(OrgMailAttr.Addr);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.Addr, value);
            }
        }
        public string GUID
        {
            get
            {
                return this.GetValStrByKey(OrgMailAttr.GUID);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.GUID, value);
            }
        }
        /// <summary>
        /// 拼音
        /// </summary>
        public string Adminer
        {
            get
            {
                return this.GetValStrByKey(OrgMailAttr.Adminer);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.Adminer, value);
            }
        }
        /// <summary>
        /// 全名
        /// </summary>
        public string NameFull
        {
            get
            {
                return this.GetValStrByKey(OrgMailAttr.NameFull);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.NameFull, value);
            }
        }
        /// <summary>
        /// 统计用的JSON
        /// </summary>
        public string JSONOfTongJi
        {
            get
            {
                return this.GetValStrByKey(OrgMailAttr.JSONOfTongJi);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.JSONOfTongJi, value);
            }
        }
        /// <summary>
        /// 注册年月
        /// </summary>
        public string FK_HY
        {
            get
            {
                return this.GetValStrByKey(OrgMailAttr.FK_HY);
            }
            set
            {
                this.SetValByKey(OrgMailAttr.FK_HY, value);
            }
        }
        /// <summary>
        /// 拼音
        /// </summary>
        public string PinYin
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.PinYin);
            }
            set
            {
                this.SetValByKey(EmpAttr.PinYin, value);
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
                map.EnType = EnType.App;

                #region 字段.
                /*关于字段属性的增加 */
                map.AddTBStringPK(OrgMailAttr.No, null, "OrgNo", true, false, 1, 50, 90);
                map.AddTBString(OrgMailAttr.Name, null, "简称", true, false, 0, 200, 130);

                map.AddDDLSysEnum(OrgMailAttr.EmailHostSta, 0, "邮件服务器规则", true, true,
                    OrgMailAttr.EmailHostSta, "@0=使用系统全局设置@1=使用本组织设置@2=禁用");

                map.AddTBString(OrgMailAttr.SendEmailHost, null, "EmailHost", true, false, 0, 50, 50, true);
                map.AddTBString(OrgMailAttr.SendEmailPort, null, "EmailPort", true, false, 0, 50, 50, true);
                map.AddTBString(OrgMailAttr.SendEmailAddress, null, "EmailAddress", true, false, 0, 50, 50, true);
                map.AddTBString(OrgMailAttr.SendEmailPass, null, "EmailPass", true, false, 0, 50, 50, true);
                map.AddBoolean(OrgMailAttr.SendEmailEnableSsl, false, "是否启用Ssl?", true, false);
                #endregion 字段.

                this._enMap = map;
                return this._enMap;
            }
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
