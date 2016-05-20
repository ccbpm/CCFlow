using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using System.Text.RegularExpressions;
namespace BP.GPM
{
    /// <summary>
    /// 访问系统菜单日志
    /// </summary>
    public class SystemMenuLogAttr
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
        /// 菜单编号
        /// </summary>
        public const string FK_Menu = "FK_Menu";
        /// <summary>
        /// 内容
        /// </summary>
        public const string RContent = "RContent";
        /// <summary>
        /// 访问时间
        /// </summary>
        public const string CreateTime = "CreateTime";
    }
    /// <summary>
    /// 访问系统菜单日志
    /// </summary>
    public class SystemMenuLog : Entity
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
                return this.GetValStrByKey(SystemMenuLogAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(SystemMenuLogAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public string FK_App
        {
            get
            {
                return this.GetValStrByKey(SystemMenuLogAttr.FK_App);
            }
            set
            {
                this.SetValByKey(SystemMenuLogAttr.FK_App, value);
            }
        }
        /// <summary>
        /// 菜单编号
        /// </summary>
        public string FK_Menu
        {
            get
            {
                return this.GetValStrByKey(SystemMenuLogAttr.FK_Menu);
            }
            set
            {
                this.SetValByKey(SystemMenuLogAttr.FK_Menu, value);
            }
        }
        /// <summary>
        /// 内容
        /// </summary>
        public int Content
        {
            get
            {
                return this.GetValIntByKey(SystemMenuLogAttr.RContent);
            }
            set
            {
                this.SetValByKey(SystemMenuLogAttr.RContent, value);
            }
        }

        /// <summary>
        /// 访问时间
        /// </summary>
        public int CreateTime
        {
            get
            {
                return this.GetValIntByKey(SystemMenuLogAttr.CreateTime);
            }
            set
            {
                this.SetValByKey(SystemMenuLogAttr.CreateTime, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 访问系统菜单日志
        /// </summary>
        public SystemMenuLog()
        {
        }
        /// <summary>
        /// 访问系统菜单日志
        /// </summary>
        /// <param name="mypk"></param>
        public SystemMenuLog(int oid)
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
                Map map = new Map("GPM_SystemMenuLog");
                map.DepositaryOfEntity = Depositary.None;
                map.EnDesc = "访问系统菜单日志";

                map.AddTBIntPK(SystemLoginLogAttr.OID, 0, "编号", true, true);
                map.AddTBString(SystemMenuLogAttr.FK_Emp, null, "人员账号", true, false, 0, 100, 20);
                map.AddTBString(SystemMenuLogAttr.FK_App, null, "系统编号", true, false, 0, 50, 20);
                map.AddTBString(SystemMenuLogAttr.FK_Menu, null, "菜单编号", true, false, 0, 50, 20);
                map.AddTBString(SystemMenuLogAttr.RContent, null, "内容", true, false, 0, 3900, 20);
                map.AddTBDateTime(SystemMenuLogAttr.CreateTime, "创建时间", true, false);

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
    /// 访问系统菜单日志s
    /// </summary>
    public class SystemMenuLogs : Entities
    {
        #region 构造
        /// <summary>
        /// 访问系统菜单日志s
        /// </summary>
        public SystemMenuLogs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SystemMenuLog();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SystemMenuLog> ToJavaList()
        {
            return (System.Collections.Generic.IList<SystemMenuLog>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SystemMenuLog> Tolist()
        {
            System.Collections.Generic.List<SystemMenuLog> list = new System.Collections.Generic.List<SystemMenuLog>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SystemMenuLog)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
