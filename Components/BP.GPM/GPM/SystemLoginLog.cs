using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using System.Text.RegularExpressions;
namespace BP.GPM
{
    /// <summary>
    /// 系统登录日志
    /// </summary>
    public class SystemLoginLogAttr
    {
        public const string OID = "OID";
        /// <summary>
        /// 人员编号
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 系统编号
        /// </summary>
        public const string FK_App = "FK_App";
        /// <summary>
        /// 内容
        /// </summary>
        public const string RContent = "RContent";
        /// <summary>
        /// 访问时间
        /// </summary>
        public const string LoginDateTime = "LoginDateTime";
        /// <summary>
        /// IP地址,
        /// </summary>
        public const string IP = "IP";
    }
    /// <summary>
    /// 系统登录日志
    /// </summary>
    public class SystemLoginLog : Entity
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public int OID
        {
            get
            {
                return this.GetValIntByKey(SystemLoginLogAttr.OID);
            }
            set
            {
                this.SetValByKey(SystemLoginLogAttr.OID, value);
            }
        }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStrByKey(SystemLoginLogAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(SystemLoginLogAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public string FK_App
        {
            get
            {
                return this.GetValStrByKey(SystemLoginLogAttr.FK_App);
            }
            set
            {
                this.SetValByKey(SystemLoginLogAttr.FK_App, value);
            }
        }
        /// <summary>
        /// 内容
        /// </summary>
        public string RContent
        {
            get
            {
                return this.GetValStrByKey(SystemLoginLogAttr.RContent);
            }
            set
            {
                this.SetValByKey(SystemLoginLogAttr.RContent, value);
            }
        }

        /// <summary>
        /// 访问时间
        /// </summary>
        public string LoginDateTime
        {
            get
            {
                return this.GetValStrByKey(SystemLoginLogAttr.LoginDateTime);
            }
            set
            {
                this.SetValByKey(SystemLoginLogAttr.LoginDateTime, value);
            }
        }
        public string IP
        {
            get
            {
                return this.GetValStrByKey(SystemLoginLogAttr.IP);
            }
            set
            {
                this.SetValByKey(SystemLoginLogAttr.IP, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 系统类别
        /// </summary>
        public SystemLoginLog()
        {
        }
        /// <summary>
        /// 系统类别
        /// </summary>
        /// <param name="mypk"></param>
        public SystemLoginLog(int oid)
        {
            this.OID = oid;
            this.Retrieve();
        }

        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("GPM_SystemLoginLog");
                map.DepositaryOfEntity = Depositary.None;
                map.EnDesc = "系统日志";

                map.AddTBIntPK(SystemLoginLogAttr.OID, 0, "编号", true, true);
                map.AddTBString(SystemLoginLogAttr.FK_Emp, null, "访问人", true, false, 0, 20, 20);
                map.AddTBString(SystemLoginLogAttr.FK_App, null, "访问系统", true, false, 0, 20, 20);
                map.AddTBString(SystemLoginLogAttr.RContent, null, "访问内容", true, false, 0, 2000, 200);
                map.AddTBDateTime(SystemLoginLogAttr.LoginDateTime, "系统访问时间", true, false);
                map.AddTBString(SystemLoginLogAttr.IP, null, "访问IP", true, false, 0, 200, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            base.beforeInsert();
            this.OID = int.Parse(BP.DA.DBAccess.GenerOID(this.ToString()).ToString());
            return true;
        }
    }

    /// <summary>
    /// 系统日志s
    /// </summary>
    public class SystemLoginLogs : Entities
    {
        #region 构造
        /// <summary>
        /// 系统日志s
        /// </summary>
        public SystemLoginLogs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SystemLoginLog();
            }
        }
        #endregion
    }
}