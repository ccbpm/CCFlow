using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
    /// <summary>
    /// 人员 属性
    /// </summary>
    public class EmployeeAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 性别
        /// </summary>
        public const string XB = "XB";
        /// <summary>
        /// 地址
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// 登录系统密码
        /// </summary>
        public const string PWD = "PWD";
        /// <summary>
        /// 班级
        /// </summary>
        public const string FK_BanJi = "FK_BanJi";
        /// <summary>
        /// 年龄
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
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        public const string BDT = "BDT";
        /// <summary>
        /// 是否特困生？
        /// </summary>
        public const string IsTeKunSheng = "IsTeKunSheng";
        /// <summary>
        /// 是否有重大疾病史？
        /// </summary>
        public const string IsJiBing = "IsJiBing";
        /// <summary>
        /// 是否偏远山区？
        /// </summary>
        public const string IsPianYuanShanQu = "IsPianYuanShanQu";
        /// <summary>
        /// 是否独生子
        /// </summary>
        public const string IsDuShengZi = "IsDuShengZi";
        /// <summary>
        /// 整治面貌
        /// </summary>
        public const string ZZMM = "ZZMM";
        /// <summary>
        /// 工龄
        /// </summary>
        public const string GL = "GL";

        #endregion
    }
    /// <summary>
    /// 人员
    /// </summary>
    public class Employee : BP.En.EntityNoName
    {
        #region 属性
        /// <summary>
        /// 登录系统密码
        /// </summary>
        public string PWD
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.PWD);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.PWD, value);
            }
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age
        {
            get
            {
                return this.GetValIntByKey(EmployeeAttr.Age);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.Age, value);
            }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.Addr);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.Addr, value);
            }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public int XB
        {
            get
            {
                return this.GetValIntByKey(EmployeeAttr.XB);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.XB, value);
            }
        }
        /// <summary>
        /// 性别名称
        /// </summary>
        public string XBText
        {
            get
            {
                return this.GetValRefTextByKey(EmployeeAttr.XB);
            }
        }
        /// <summary>
        /// 班级编号
        /// </summary>
        public string FK_BanJi
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.FK_BanJi);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.FK_BanJi, value);
            }
        }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string FK_BanJiText
        {
            get
            {
                return this.GetValRefTextByKey(EmployeeAttr.FK_BanJi);
            }
        }
        /// <summary>
        /// 邮件
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.Email);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.Email, value);
            }
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.Tel);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.Tel, value);
            }
        }
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegDate
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.RegDate);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.RegDate, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 实体的权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = true;
                uac.IsUpdate = true;
                uac.IsInsert = true;
                uac.IsView = true;
                return uac;
            }
        }
        /// <summary>
        /// 人员
        /// </summary>
        public Employee()
        {
        }
        /// <summary>
        /// 人员
        /// </summary>
        /// <param name="no"></param>
        public Employee(string no):base(no)
        {
        }
        #endregion

        #region 重写基类方法
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Demo_Employee", "人员");
                //基础信息.
                map.IsAllowRepeatName = true; //是否允许名称重复.
                map.IsAutoGenerNo = true; //是否自动生成编号.
                map.Java_SetCodeStruct("4"); // 4位数的编号，从 0001 开始，到 9999.

                //普通字段.
                map.AddTBStringPK(EmployeeAttr.No, null, "学号", true, true, 4, 4, 4); // 如果设置自动编号字段必须是只读的.
                map.AddTBString(EmployeeAttr.Name, null, "名称", true, false, 0, 200, 70);
                map.AddTBString(EmployeeAttr.PWD, null, "登录密码", true, false, 0, 200, 70);
                //map.AddTBString("shuoming", null, "说明", true, false, 0, 200, 70);

                map.AddTBString(EmployeeAttr.Addr, null, "地址", true, false, 0, 200, 100,false,"http://sina.com.cn");
                map.AddTBInt(EmployeeAttr.Age, 0, "年龄", true, false);
                map.AddTBInt(EmployeeAttr.GL, 0, "工龄", true, false);

                map.AddTBString(EmployeeAttr.Tel, null, "电话", true, false, 0, 200, 60);
                map.AddTBString(EmployeeAttr.Email, null, "邮件", true, false, 0, 200, 50,true);
                
                map.AddTBDateTime(EmployeeAttr.RegDate, null, "注册日期", true, true);
                map.AddTBDate(EmployeeAttr.BDT, null, "出生日期", true, true);

                map.AddTBStringDoc(EmployeeAttr.Note, null, "备注", true, false, true); //大快文本框.


                //枚举字段
                map.AddDDLSysEnum(EmployeeAttr.XB, 0, "性别", true, true, EmployeeAttr.XB, "@0=女@1=男");
                //外键字段.
                map.AddDDLEntities(EmployeeAttr.FK_BanJi, null, "班级", new BP.Demo.BPFramework.BanJis(), true);


                //增加checkbox属性.
                map.AddBoolean(EmployeeAttr.IsDuShengZi, false, "是否是独生子？", true, true);
                map.AddBoolean(EmployeeAttr.IsJiBing, false, "是否有重大疾病？", true, true);
                map.AddBoolean(EmployeeAttr.IsPianYuanShanQu, false, "是否偏远山区？", true, true);
                map.AddBoolean(EmployeeAttr.IsTeKunSheng, false, "是否是特困生？", true, true);

                // 枚举字段 - 整治面貌.
                map.AddDDLSysEnum(EmployeeAttr.ZZMM, 0, "整治面貌", true, true, EmployeeAttr.ZZMM,
                    "@0=少先队员@1=团员@2=党员");

                map.AddMyFile("简历");

                // map.AddMyFileS("简历");

                //设置查询条件。
                map.AddSearchAttr(EmployeeAttr.XB);
                map.AddSearchAttr(EmployeeAttr.FK_BanJi);

              

                //带有参数的方法.
                RefMethod rm = new RefMethod();
                rm.Title = "缴纳班费";
                rm.HisAttrs.AddTBDecimal("JinE", 100, "缴纳金额", true, false);
                rm.HisAttrs.AddTBString("Note", null, "备注", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoJiaoNaBanFei";
                map.AddRefMethod(rm);

                //不带有参数的方法.
                rm = new RefMethod();
                rm.Title = "注销学籍";
                rm.Warning = "您确定要注销吗？";
                rm.ClassMethodName = this.ToString() + ".DoZhuXiao";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
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
        protected override bool beforeUpdateInsertAction()
        {
            if (this.Email.Length == 0)
                throw new Exception("@email 不能为空.");

            return base.beforeUpdateInsertAction();
        }

          
        #endregion 重写基类方法

        #region 方法
        /// <summary>
        /// 带有参数的方法:缴纳班费
        /// 说明：都要返回string类型.
        /// </summary>
        /// <returns></returns>
        public string DoJiaoNaBanFei(decimal jine, string note)
        {

            return "学号:"+this.No+",姓名:"+this.Name+",缴纳了:"+jine+"元,说明:"+note;
        }
        /// <summary>
        /// 无参数的方法:注销学籍
        /// 说明：都要返回string类型.
        /// </summary>
        /// <returns></returns>
        public string DoZhuXiao()
        {
            return "学号:" + this.No + ",姓名:" + this.Name + ",已经注销.";
        }
        /// <summary>
        /// 校验密码
        /// </summary>
        /// <param name="pass">原始密码</param>
        /// <returns>是否成功</returns>
        public bool CheckPass(string pass)
        {
            return this.PWD.Equals(pass);
        }
        #endregion

    }
    /// <summary>
    /// 人员s
    /// </summary>
    public class Employees : BP.En.EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 人员s
        /// </summary>
        public Employees() { }
        #endregion

        #region 重写基类方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Employee();
            }
        }
        #endregion 重写基类方法

    }
}
