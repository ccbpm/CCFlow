using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;
using BP.GPM;

namespace BP.Demo.License
{
    /// <summary>
    /// 居民属性
    /// </summary>
    public class PeopleAttr : BP.En.EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 地址
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// 电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 出生日期
        /// </summary>
        public const string BDT = "BDT";
        /// <summary>
        /// 性别
        /// </summary>
        public const string XB = "XB";
        /// <summary>
        /// 年龄
        /// </summary>
        public const string Age = "Age";
        /// <summary>
        /// 性别
        /// </summary>
        public const string Email = "Email";

        #endregion
    }
    /// <summary>
    /// 居民 的摘要说明。
    /// </summary>
    public class People : EntityNoName
    {
        #region 扩展属性
        public string XB
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.XB);
            }
            set
            {
                this.SetValByKey(PeopleAttr.XB, value);
            }
        }
        public string Email
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.Email);
            }
            set
            {
                this.SetValByKey(PeopleAttr.Email, value);
            }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.Addr);
            }
            set
            {
                this.SetValByKey(PeopleAttr.Addr, value);
            }
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public string Age
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.Age);
            }
            set
            {
                this.SetValByKey(PeopleAttr.Age, value);
            }
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.Tel);
            }
            set
            {
                this.SetValByKey(PeopleAttr.Tel, value);
            }
        }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string BDT
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.BDT);
            }
            set
            {
                this.SetValByKey(PeopleAttr.BDT, value);
            }
        }
        #endregion
       
        #region 构造函数
        /// <summary>
        /// 居民
        /// </summary>
        public People()
        {
        }
        /// <summary>
        /// 居民
        /// </summary>
        /// <param name="no">编号</param>
        public People(string no)
        {
            this.No = no.Trim();
            if (this.No.Length == 0)
                throw new Exception("@要查询的居民编号为空。");

            this.Retrieve();
        }

        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
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

                Map map = new Map();

                #region 基本属性
                map.EnDBUrl =
                    new DBUrl(DBUrlType.AppCenterDSN); //要连接的数据源（表示要连接到的那个系统数据库）。
                map.PhysicsTable = "Demo_People"; // 要物理表。
                map.DepositaryOfMap = Depositary.Application;    //实体map的存放位置.
                map.DepositaryOfEntity = Depositary.None; //实体存放位置
                map.EnDesc = "用户"; // "用户"; // 实体的描述.
                map.EnType = EnType.App;   //实体类型。
                #endregion

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(PeopleAttr.No, null, "编号", true, false, 1, 60, 30);
                map.AddTBString(PeopleAttr.Name, null, "名称", true, false, 0, 999, 30);

                map.AddTBString(PeopleAttr.Addr, null, "地址", true, false, 0, 999, 30);

                map.AddTBString(PeopleAttr.Tel, null, "电话", false, false, 0, 999, 10);
                map.AddTBString(PeopleAttr.BDT, null, "出生日期", false, false, 0, 999, 10);
                map.AddTBString(PeopleAttr.Email, null, "邮箱", true, false, 0, 999, 132);

                map.AddTBString(PeopleAttr.XB, null, "性别", true, false, 0, 999, 132);
                map.AddTBString(PeopleAttr.Age, null, "年龄", true, false, 0, 999, 132);
                #endregion 字段

               
                RefMethod rm = new RefMethod();
                rm.Title = "修改密码";
                rm.ClassMethodName = this.ToString() + ".DoResetpassword";
                rm.HisAttrs.AddTBString("pass1", null, "输入密码", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("pass2", null, "再次输入", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new Peoples(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 居民s
    // </summary>
    public class Peoples : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new People();
            }
        }
        /// <summary>
        /// 居民s
        /// </summary>
        public Peoples()
        {
        }
        public override int RetrieveAll()
        {
            return base.RetrieveAll("Name");
        }
        #endregion 构造方法
    }
}
