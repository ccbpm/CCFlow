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
        public const string Email = "Email";
        public const string AlertWay = "AlertWay";
        public const string Stas = "Stas";
        public const string Depts = "Depts";
        public const string FK_Dept = "FK_Dept";
        public const string Idx = "Idx";
        public const string Style = "Style";
        public const string Msg = "Msg";
        public const string UseSta = "UseSta";
        /// <summary>
        /// 可以发起的流程
        /// </summary>
        public const string StartFlows = "StartFlows";
        /// <summary>
        /// 图片签名密码
        /// </summary>
        public const string SPass = "SPass";
        /// <summary>
        /// token.
        /// </summary>
        public const string Token = "Token";

        public const string OrgNo = "OrgNo";

        public const string UserID = "UserID";
        #endregion
    }
    /// <summary>
    /// 操作员
    /// </summary>
    public class WFEmp : EntityNoName
    {
        #region 基本属性
        /// <summary>
		/// 编号
		/// </summary>
		
        public new string No
        {
            get
            {
                return this.GetValStringByKey(EntityNoNameAttr.No);
            }
            set
            {
                this.SetValByKey(EmpAttr.No, value);
            }
        }
        /// <summary>
        /// 用户ID:SAAS模式下UserID是可以重复的.
        /// </summary>
        public string UserID
        {
            get
            {
                if (BP.Sys.SystemConfig.CCBPMRunModel == BP.Sys.CCBPMRunModel.SAAS)
                    return this.GetValStringByKey(WFEmpAttr.UserID);

                return this.GetValStringByKey(WFEmpAttr.No);
            }
            set
            {
                this.SetValByKey(WFEmpAttr.UserID, value);

                if (BP.Sys.SystemConfig.CCBPMRunModel == BP.Sys.CCBPMRunModel.SAAS)
                    this.SetValByKey(WFEmpAttr.No, BP.Web.WebUser.OrgNo + "_" + value);
                else
                    this.SetValByKey(WFEmpAttr.No, value);
            }
        }
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(WFEmpAttr.OrgNo, value);
            }
        }
        public string HisAlertWayT
        {
            get
            {
                return this.GetValRefTextByKey(WFEmpAttr.AlertWay);
            }
        }
        public string Token
        {
            get
            {
                return this.GetValStringByKey(WFEmpAttr.Token);
            }
            set
            {
                SetValByKey(WFEmpAttr.Token, value);
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
        /// <summary>
        /// 部门编号
        /// </summary>
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
        /// <summary>
        /// 风格文件
        /// </summary>
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

        /// <summary>
        /// 电话
        /// </summary>
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
        #endregion

        #region 构造函数
        /// <summary>
        /// 操作员
        /// </summary>
        public WFEmp() { }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="userID"></param>
        public WFEmp(string userID)
        {

            if (userID == null || userID.Length == 0)
                throw new Exception("@要查询的操作员编号为空。");

            userID = userID.Trim();
            if (BP.Sys.SystemConfig.CCBPMRunModel == BP.Sys.CCBPMRunModel.SAAS)
            {
                if (userID.Equals("admin") == true)
                    this.SetValByKey("No", userID);
                else
                    this.SetValByKey("No", BP.Web.WebUser.OrgNo + "_" + userID);
            }
            else
            {
                this.SetValByKey("No", userID);
            }

            if (this.RetrieveFromDBSources() == 0)
            {
                BP.Port.Emp emp = new BP.Port.Emp(userID);
                this.Row = emp.Row;
                this.Insert();
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
                map.AddDDLSysEnum(WFEmpAttr.AlertWay, 3, "收听方式", true, true, WFEmpAttr.AlertWay);

                map.AddTBString(WFEmpAttr.Stas, null, "岗位s", true, true, 0, 3000, 20);
                map.AddTBString(WFEmpAttr.Depts, null, "Deptss", true, true, 0, 100, 36);

                map.AddTBString(WFEmpAttr.Msg, null, "Msg", true, true, 0, 4000, 20);
                map.AddTBString(WFEmpAttr.Style, null, "Style", true, true, 0, 30, 20);

                //如果是集团模式或者是SAAS模式.
                if (BP.Sys.SystemConfig.CCBPMRunModel != BP.Sys.CCBPMRunModel.Single)
                    map.AddTBString(WFEmpAttr.UserID, null, "用户ID", true, false, 0, 50, 30);
                //隶属组织.
                map.AddTBString(WFEmpAttr.OrgNo, null, "OrgNo", true, true, 0, 100, 20);

                map.AddTBString(WFEmpAttr.SPass, null, "图片签名密码", true, true, 0, 200, 20);

                map.AddTBString(WFEmpAttr.Token, null, "token", true, true, 0, 200, 20);


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
