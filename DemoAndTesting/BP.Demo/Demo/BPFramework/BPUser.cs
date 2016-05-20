using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.Demo.BPFramework
{
    /// <summary>
    /// 注册用户 属性
    /// </summary>
    public class BPUserAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 密码
        /// </summary>
        public const string Pass = "Pass";
        /// <summary>
        /// 性别
        /// </summary>
        public const string XB = "XB";
        /// <summary>
        /// 地址
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 年月
        /// </summary>
        public const string Age = "Age";
        /// <summary>
        /// 邮件
        /// </summary>
        public const string Email = "Email";
        /// <summary>
        /// 电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 注册时间
        /// </summary>
        public const string RegDate = "RegDate";
        #endregion
    }
    /// <summary>
    /// 注册用户
    /// </summary>
    public class BPUser : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 密码
        /// </summary>
        public string Pass
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.Pass);
            }
            set
            {
                this.SetValByKey(BPUserAttr.Pass, value);
            }
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age
        {
            get
            {
                return this.GetValIntByKey(BPUserAttr.Age);
            }
            set
            {
                this.SetValByKey(BPUserAttr.Age, value);
            }
        }
        /// <summary>
        /// 性别
        /// </summary>
        public int XB
        {
            get
            {
                return this.GetValIntByKey(BPUserAttr.XB);
            }
            set
            {
                this.SetValByKey(BPUserAttr.XB, value);
            }
        }
        /// <summary>
        /// 性别名称
        /// </summary>
        public string XBText
        {
            get
            {
                return this.GetValRefTextByKey(BPUserAttr.XB);
            }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.Addr);
            }
            set
            {
                this.SetValByKey(BPUserAttr.Addr, value);
            }
        }
        /// <summary>
        /// 注册年月
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(BPUserAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// 邮件
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.Email);
            }
            set
            {
                this.SetValByKey(BPUserAttr.Email, value);
            }
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.Tel);
            }
            set
            {
                this.SetValByKey(BPUserAttr.Tel, value);
            }
        }
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegDate
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.RegDate);
            }
            set
            {
                this.SetValByKey(BPUserAttr.RegDate, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 注册用户
        /// </summary>
        public BPUser()
        {
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
                Map map = new Map("Demo_BPUser");
                map.EnDesc = "注册用户";
                
                // 普通字段
                map.AddTBStringPK(BPUserAttr.No, null, "用户名", true, false, 1, 100, 100);
                map.AddTBString(BPUserAttr.Pass, null, "密码", true, false, 0, 200, 10);
                map.AddTBString(BPUserAttr.Name, null, "名称", true, false, 0, 200, 10);
                map.AddTBInt(BPUserAttr.Age, 0, "年龄", true, false);
                map.AddTBString(BPUserAttr.Addr, null, "地址", true, false, 0, 200, 10);
                map.AddTBString(BPUserAttr.Tel, null, "电话", true, false, 0, 200, 10);
                map.AddTBString(BPUserAttr.Email, null, "邮件", true, false, 0, 200, 10);
                map.AddTBDateTime(BPUserAttr.RegDate, null, "注册日期", true, true);

                //枚举字段
                map.AddDDLSysEnum(BPUserAttr.XB, 0,"性别", false, true, BPUserAttr.XB, "@0=女@1=男");

                //外键字段.
                map.AddDDLEntities(BPUserAttr.FK_NY, null, "隶属年月", new BP.Pub.NYs(),true);

                //设置查询条件。
                map.AddSearchAttr(BPUserAttr.XB);
                map.AddSearchAttr(BPUserAttr.FK_NY);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        /// <summary>
        /// 重写基类的方法.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            //在插入之前设置注册时间.
            this.RegDate = DataType.CurrentDataTime;

            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 注册用户s
    /// </summary>
    public class BPUsers : EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BPUser();
            }
        }
        /// <summary>
        /// 注册用户s
        /// </summary>
        public BPUsers() { }
        #endregion
    }
}
