using System;
using System.Text.RegularExpressions;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Cloud
{
    /// <summary>
    /// 操作员 属性
    /// </summary>
    public class GuestAttr : BP.En.EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 密码
        /// </summary>
        public const string Password = "Password";
        /// <summary>
        /// 邮箱
        /// </summary>
        public const string Email = "Email";
        /// <summary>
        /// 拼音
        /// </summary>
        public const string PinYin = "PinYin";
        /// <summary>
        /// Addr
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// QQ号
        /// </summary>
        public const string QQ = "QQ";
        /// <summary>
        /// 组织解构编码
        /// </summary>
        public const string OrgNo = "OrgNo";
        #endregion
    }
    /// <summary>
    /// 操作员 的摘要说明。
    /// </summary>
    public class Guest : EntityNoName
    {
        #region 扩展属性
        /// <summary>
        /// 拼音
        /// </summary>
        public string PinYin
        {
            get
            {
                return this.GetValStrByKey(GuestAttr.PinYin);
            }
            set
            {
                this.SetValByKey(GuestAttr.PinYin, value);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get
            {
                return this.GetValStrByKey(GuestAttr.Password);
            }
            set
            {
                this.SetValByKey(GuestAttr.Password, value);
            }
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStrByKey(GuestAttr.Tel);
            }
            set
            {
                this.SetValByKey(GuestAttr.Tel, value);
            }
        }
        public string Email
        {
            get
            {
                return this.GetValStrByKey(GuestAttr.Email);
            }
            set
            {
                this.SetValByKey(GuestAttr.Email, value);
            }
        }
        public string QQ
        {
            get
            {
                return this.GetValStrByKey(GuestAttr.QQ);
            }
            set
            {
                this.SetValByKey(GuestAttr.QQ, value);
            }
        }
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(GuestAttr.Addr);
            }
            set
            {
                this.SetValByKey(GuestAttr.Addr, value);
            }
        }
        /// <summary>
        /// 组织结构编码
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(GuestAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(GuestAttr.OrgNo, value);
            }
        }
        #endregion

        #region 公共方法
        #endregion 公共方法

        #region 构造函数
        /// <summary>
        /// 操作员
        /// </summary>
        public Guest()
        {
        }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="no">编号</param>
        public Guest(string no)
        {

            this.No = no;
            this.Retrieve();
        }

        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.IsAdmin == true)
                    uac.OpenAll();
                else
                    uac.Readonly();
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

                Map map = new Map("Port_Guest", "用户");
                map.EnType = EnType.App;
                #region 字段

                /*关于字段属性的增加 */
                map.AddTBStringPK(GuestAttr.No, null, "身份ID", true, false, 1, 500, 90);
                map.AddTBString(GuestAttr.Name, null, "姓名", true, false, 0, 500, 130);

                map.AddTBString(GuestAttr.Tel, null, "电话", true, false, 0, 20, 130);
                map.AddTBString(GuestAttr.Password, null, "密码", true, false, 0, 20, 130);

                map.AddTBString(GuestAttr.Email, null, "邮箱", true, false, 0, 100, 132, true);
                map.AddTBString(GuestAttr.PinYin, null, "拼音", true, false, 0, 1000, 132, true);

                map.AddTBString(GuestAttr.QQ, null, "QQ", true, false, 0, 500, 132, true);
                map.AddTBString(GuestAttr.Addr, null, "Addr", true, false, 0, 500, 132, true);
                map.AddTBString(GuestAttr.OrgNo, null, "OrgNo", false, false, 0, 36, 36);
                #endregion 字段

                this._enMap = map;
                return this._enMap;
            }
        }
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
        /// <summary>
        /// 删除之后要做的事情
        /// </summary>
        protected override void afterDelete()
        {
            base.afterDelete();
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new Guests(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 操作员s
    // </summary>
    public class Guests : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Guest();
            }
        }
        /// <summary>
        /// 操作员s
        /// </summary>
        public Guests()
        {
        }

        #endregion 构造方法

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Guest> ToJavaList()
        {
            return (System.Collections.Generic.IList<Guest>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Guest> Tolist()
        {
            System.Collections.Generic.List<Guest> list = new System.Collections.Generic.List<Guest>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Guest)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
