using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;
using BP.Port;
using BP.En;
using BP.Web;
using System.Drawing;
using System.Text;
using System.IO;

namespace BP.WF.Port
{
    /// <summary>
    /// 授权方式
    /// </summary>
    public enum AuthorWay
    {
        /// <summary>
        /// 不授权
        /// </summary>
        None,
        /// <summary>
        /// 全部授权
        /// </summary>
        All,
        /// <summary>
        /// 指定流程授权
        /// </summary>
        SpecFlows
    }
    public enum AlertWay
    {
        /// <summary>
        /// 不提示
        /// </summary>
        None,
        /// <summary>
        /// 手机短信
        /// </summary>
        SMS,
        /// <summary>
        /// 邮件
        /// </summary>
        Email,
        /// <summary>
        /// 手机短信+邮件
        /// </summary>
        SMSAndEmail,
        /// <summary>
        /// 内部消息
        /// </summary>
        AppSystemMsg
    }
    /// <summary>
    /// 操作员
    /// </summary>
    public class WFEmpAttr
    {
        #region 基本属性
        /// <summary>
        /// No
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 申请人
        /// </summary>
        public const string Name = "Name";
        public const string LoginData = "LoginData";
        public const string Tel = "Tel";
        /// <summary>
        /// 授权人
        /// </summary>
        public const string Author = "Author";
        /// <summary>
        /// 授权日期
        /// </summary>
        public const string AuthorDate = "AuthorDate";
        /// <summary>
        /// 是否处于授权状态
        /// </summary>
        public const string AuthorWay = "AuthorWay";
        /// <summary>
        /// 授权自动收回日期
        /// </summary>
        public const string AuthorToDate = "AuthorToDate";
        public const string Email = "Email";
        public const string AlertWay = "AlertWay";
        public const string Stas = "Stas";
        public const string Depts = "Depts";
        public const string FK_Dept = "FK_Dept";
        public const string Idx = "Idx";
        public const string FtpUrl = "FtpUrl";
        public const string Style = "Style";
        public const string Msg = "Msg";
        public const string TM = "TM";
        public const string UseSta = "UseSta";
        /// <summary>
        /// 授权的人员
        /// </summary>
        public const string AuthorFlows = "AuthorFlows";
        /// <summary>
        /// 可以发起的流程
        /// </summary>
        public const string StartFlows = "StartFlows";
        /// <summary>
        /// 图片签名密码
        /// </summary>
        public const string SPass = "SPass";
        #endregion
    }
    /// <summary>
    /// 操作员
    /// </summary>
    public class WFEmp : EntityNoName
    {
        #region 基本属性
        public string HisAlertWayT
        {
            get
            {
                return this.GetValRefTextByKey(WFEmpAttr.AlertWay);
            }
        }
        public AlertWay HisAlertWay
        {
            get
            {
                return (AlertWay)this.GetValIntByKey(WFEmpAttr.AlertWay);
            }
            set
            {
                SetValByKey(WFEmpAttr.AlertWay, (int)value);
            }
        }
        /// <summary>
        /// 用户状态
        /// </summary>
        public int UseSta
        {
            get
            {
                return this.GetValIntByKey(WFEmpAttr.UseSta);
            }
            set
            {
                SetValByKey(WFEmpAttr.UseSta, value);
            }
        }
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.FK_Dept);
            }
            set
            {
                SetValByKey(WFEmpAttr.FK_Dept, value);
            }
        }
        public string Style
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.Style);
            }
            set
            {
                this.SetValByKey(WFEmpAttr.Style, value);
            }
        }
        public string TM
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.TM);
            }
            set
            {
                this.SetValByKey(WFEmpAttr.TM, value);
            }
        }
        /// <summary>
        /// 微信号的OpenID.
        /// </summary>
        public string OpenID
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.TM);
            }
            set
            {
                this.SetValByKey(WFEmpAttr.TM, value);
            }
        }
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.Tel);
            }
            set
            {
                SetValByKey(WFEmpAttr.Tel, value);
            }
        }
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(WFEmpAttr.Idx);
            }
            set
            {
                SetValByKey(WFEmpAttr.Idx, value);
            }
        }
        public string TelHtml
        {
            get
            {
                if (this.Tel.Length == 0)
                    return "未设置";
                else
                    return "<a href=\"javascript:WinOpen('./Msg/SMS.aspx?Tel=" + this.Tel + "');\"  ><img src='/WF/Img/SMS.gif' border=0/>" + this.Tel + "</a>";
            }
        }
        public string EmailHtml
        {
            get
            {
                if (this.Email == null || this.Email.Length == 0)
                    return "未设置";
                else
                    return "<a href='Mailto:" + this.Email + "' ><img src='/WF/Img/SMS.gif' border=0/>" + this.Email + "</a>";
            }
        }
        public string Email
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.Email);
            }
            set
            {
                SetValByKey(WFEmpAttr.Email, value);
            }
        }
        public string Author
        {
            get
            {
                return this.GetValStrByKey(WFEmpAttr.Author);
            }
            set
            {
                SetValByKey(WFEmpAttr.Author, value);
            }
        }
        public string AuthorDate
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.AuthorDate);
            }
            set
            {
                SetValByKey(WFEmpAttr.AuthorDate, value);
            }
        }
        public string AuthorToDate
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.AuthorToDate);
            }
            set
            {
                SetValByKey(WFEmpAttr.AuthorToDate, value);
            }
        }
        /// <summary>
        /// 授权的流程
        /// </summary>
        public string AuthorFlows
        {
            get
            {
                string s = this.GetValStringByKey(WFEmpAttr.AuthorFlows);
                s = s.Replace(",", "','");
                return "('" + s + "')";
            }
            set
            {
                //授权流程为空时的bug  解决
                if (!DataType.IsNullOrEmpty(value))
                {
                    SetValByKey(WFEmpAttr.AuthorFlows, value.Substring(1));
                }
                else
                {
                    SetValByKey(WFEmpAttr.AuthorFlows, "");
                }
                //SetValByKey(WFEmpAttr.AuthorFlows, value.Substring(1));
            }
        }
        /// <summary>
        /// 发起流程.
        /// </summary>
        public string StartFlows
        {
            get
            {
                return this.GetValStrByKey(WFEmpAttr.StartFlows);
            }
            set
            {
                SetValByKey(WFEmpAttr.StartFlows, value);
            }
        }
        /// <summary>
        /// 图片签名密码
        /// </summary>
        public string SPass
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.SPass);
            }
            set
            {
                SetValByKey(WFEmpAttr.SPass, value);
            }
        }
        public string FtpUrl
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.FtpUrl);
            }
            set
            {
                SetValByKey(WFEmpAttr.FtpUrl, value);
            }
        }
        /// <summary>
        /// 授权方式
        /// </summary>
        public AuthorWay HisAuthorWay
        {
            get
            {
                return (AuthorWay)this.AuthorWay;
            }
        }
        /// <summary>
        /// 授权方式
        /// </summary>
        public int AuthorWay
        {
            get
            {
                return this.GetValIntByKey(WFEmpAttr.AuthorWay);
            }
            set
            {
                SetValByKey(WFEmpAttr.AuthorWay, value);
            }
        }
        public bool AuthorIsOK
        {
            get
            {
                int b = this.GetValIntByKey(WFEmpAttr.AuthorWay);
                if (b == 0)
                    return false; //不授权.

                // if (DataType.IsNullOrEmpty(this.Author) == true)
                //  return false;

                if (this.AuthorToDate.Length < 4)
                    return true; /*没有填写时间,当做无期限*/

                DateTime dt = DataType.ParseSysDateTime2DateTime(this.AuthorToDate);
                if (dt < DateTime.Now)
                    return false;

                return true;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 操作员
        /// </summary>
        public WFEmp() { }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="no"></param>
        public WFEmp(string no)
        {
            this.No = no;
            try
            {
                if (this.RetrieveFromDBSources() == 0)
                {
                    Emp emp = new Emp(no);
                    this.Copy(emp);
                    this.Insert();
                }
            }
            catch
            {
                this.CheckPhysicsTable();
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

                Map map = new Map("WF_Emp", "操作员");

                map.AddTBStringPK(WFEmpAttr.No, null, "No", true, true, 1, 50, 36);
                map.AddTBString(WFEmpAttr.Name, null, "Name", true, false, 0, 50, 20);
                map.AddTBInt(WFEmpAttr.UseSta, 1, "用户状态0禁用,1正常.", true, true);

                map.AddTBString(WFEmpAttr.Tel, null, "Tel", true, true, 0, 50, 20);
                map.AddTBString(WFEmpAttr.FK_Dept, null, "FK_Dept", true, true, 0, 100, 36);
                map.AddTBString(WFEmpAttr.Email, null, "Email", true, true, 0, 50, 20);
                map.AddTBString(WFEmpAttr.TM, null, "即时通讯号", true, true, 0, 50, 20);

                map.AddDDLSysEnum(WFEmpAttr.AlertWay, 3, "收听方式", true, true,
                    WFEmpAttr.AlertWay);
                map.AddTBString(WFEmpAttr.Author, null, "授权人", true, true, 0, 50, 20);
                map.AddTBString(WFEmpAttr.AuthorDate, null, "授权日期", true, true, 0, 50, 20);

                //0不授权， 1完全授权，2，指定流程范围授权. 
                map.AddTBInt(WFEmpAttr.AuthorWay, 0, "授权方式", true, true);
                map.AddTBDate(WFEmpAttr.AuthorToDate, null, "授权到日期", true, true);

                map.AddTBString(WFEmpAttr.AuthorFlows, null, "可以执行的授权流程", true, true, 0, 3900, 0);

                map.AddTBString(WFEmpAttr.Stas, null, "岗位s", true, true, 0, 3000, 20);
                map.AddTBString(WFEmpAttr.Depts, null, "Deptss", true, true, 0, 100, 36);

                map.AddTBString(WFEmpAttr.FtpUrl, null, "FtpUrl", true, true, 0, 50, 20);
                map.AddTBString(WFEmpAttr.Msg, null, "Msg", true, true, 0, 4000, 20);
                map.AddTBString(WFEmpAttr.Style, null, "Style", true, true, 0, 4000, 20);

                map.AddTBStringDoc(WFEmpAttr.StartFlows, null, "可以发起的流程", true, true);

                map.AddTBString(WFEmpAttr.SPass, null, "图片签名密码", true, true, 0, 200, 20);

                map.AddTBInt(WFEmpAttr.Idx, 0, "Idx", false, false);

                map.AddTBAtParas(3500); //增加字段.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 方法
        protected override bool beforeUpdate()
        {
            string msg = "";
            //if (this.Email.Length == 0)
            //{
            //    if (this.HisAlertWay == AlertWay.SMSAndEmail || this.HisAlertWay == AlertWay.Email)
            //        msg += "错误：您设置了用e-mail接收信息，但是您没有设置e-mail。";
            //}

            //if (this.Tel.Length == 0)
            //{
            //    if (this.HisAlertWay == AlertWay.SMSAndEmail || this.HisAlertWay == AlertWay.SMS)
            //        msg += "错误：您设置了用短信接收信息，但是您没有设置手机号。";
            //}

            //EmpStations ess = new EmpStations();
            //ess.Retrieve(EmpStationAttr.FK_Emp, this.No);
            //string sts = "";
            //foreach (EmpStation es in ess)
            //{
            //    sts += es.FK_StationT + " ";
            //}
            //this.Stas = sts;

            if (msg != "")
                throw new Exception(msg);

            return base.beforeUpdate();
        }
        protected override bool beforeInsert()
        {
            this.UseSta = 1;
            return base.beforeInsert();
        }
        #endregion

        public static void DTSData()
        {
            string sql = "select No from Port_Emp where No not in (select No from WF_Emp)";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                BP.Port.Emp emp1 = new BP.Port.Emp(dr["No"].ToString());
                BP.WF.Port.WFEmp empWF = new BP.WF.Port.WFEmp();
                empWF.Copy(emp1);
                try
                {
                    empWF.UseSta = 1;
                    empWF.DirectInsert();
                }
                catch
                {
                }
            }
        }
        public void DoUp()
        {
            this.DoOrderUp("FK_Dept", this.FK_Dept, "Idx");
            return;
        }
        public void DoDown()
        {
            this.DoOrderDown("FK_Dept", this.FK_Dept, "Idx");
            return;
        }
    }
    /// <summary>
    /// 操作员s 
    /// </summary>
    public class WFEmps : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 操作员s
        /// </summary>
        public WFEmps()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WFEmp();
            }
        }

        public override int RetrieveAll()
        {
            return base.RetrieveAll("FK_Dept", "Idx");
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<WFEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<WFEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<WFEmp> Tolist()
        {
            System.Collections.Generic.List<WFEmp> list = new System.Collections.Generic.List<WFEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((WFEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }

}
