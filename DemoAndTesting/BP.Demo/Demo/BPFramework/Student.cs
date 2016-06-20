using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
    /// <summary>
    /// 学生 属性
    /// </summary>
    public class StudentAttr : EntityNoNameAttr
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
        #endregion
    }
    /// <summary>
    /// 学生
    /// </summary>
    public class Student : BP.En.EntityNoName
    {
        #region 属性
        /// <summary>
        /// 登录系统密码
        /// </summary>
        public string PWD
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.PWD);
            }
            set
            {
                this.SetValByKey(StudentAttr.PWD, value);
            }
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age
        {
            get
            {
                return this.GetValIntByKey(StudentAttr.Age);
            }
            set
            {
                this.SetValByKey(StudentAttr.Age, value);
            }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Addr);
            }
            set
            {
                this.SetValByKey(StudentAttr.Addr, value);
            }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public int XB
        {
            get
            {
                return this.GetValIntByKey(StudentAttr.XB);
            }
            set
            {
                this.SetValByKey(StudentAttr.XB, value);
            }
        }
        /// <summary>
        /// 性别名称
        /// </summary>
        public string XBText
        {
            get
            {
                return this.GetValRefTextByKey(StudentAttr.XB);
            }
        }

        /// <summary>
        /// 班级编号
        /// </summary>
        public string FK_BanJi
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.FK_BanJi);
            }
            set
            {
                this.SetValByKey(StudentAttr.FK_BanJi, value);
            }
        }
          /// <summary>
        /// 班级名称
        /// </summary>
        public string FK_BanJiText
        {
            get
            {
                return this.GetValRefTextByKey(StudentAttr.FK_BanJi);
            }
        }
        /// <summary>
        /// 邮件
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Email);
            }
            set
            {
                this.SetValByKey(StudentAttr.Email, value);
            }
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Tel);
            }
            set
            {
                this.SetValByKey(StudentAttr.Tel, value);
            }
        }
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegDate
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.RegDate);
            }
            set
            {
                this.SetValByKey(StudentAttr.RegDate, value);
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
                return uac;
            }
        }
        /// <summary>
        /// 学生
        /// </summary>
        public Student()
        {
        }
        /// <summary>
        /// 学生
        /// </summary>
        /// <param name="no"></param>
        public Student(string no):base(no)
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

                Map map = new Map();

                //基础信息.
                map.EnDesc = "学生";
                map.PhysicsTable = "Demo_Student";
                map.IsAllowRepeatName = true; //是否允许名称重复.
                map.IsAutoGenerNo = true; //是否自动生成编号.
                map.CodeStruct = "4"; // 4位数的编号，从 0001 开始，到 9999.

                // 普通字段
                map.AddTBStringPK(StudentAttr.No, null, "学号", true, true, 4, 4, 4); // 如果设置自动编号字段必须是只读的.
                map.AddTBString(StudentAttr.Name, null, "名称", true, false, 0, 200, 70);
                map.AddTBString(StudentAttr.PWD, null, "登录密码", true, false, 0, 200, 70);


                map.AddTBString(StudentAttr.Addr, null, "地址", true, false, 0, 200, 100,true);
                map.AddTBInt(StudentAttr.Age, 0, "年龄", true, false);
                map.AddTBString(StudentAttr.Tel, null, "电话", true, false, 0, 200, 60);
                map.AddTBString(StudentAttr.Email, null, "邮件", true, false, 0, 200, 50);
                map.AddTBDateTime(StudentAttr.RegDate, null, "注册日期", true, true);
                map.AddTBStringDoc(StudentAttr.Note, null, "备注", true, false, true); //大快文本框.


                //枚举字段
                map.AddDDLSysEnum(StudentAttr.XB, 0, "性别", true, true,StudentAttr.XB,"@0=女@1=男");

                //外键字段.
                map.AddDDLEntities(StudentAttr.FK_BanJi, null,"班级", new BP.Demo.BPFramework.BanJis(), true);


                map.AddMyFile("简历");

               // map.AddMyFileS("简历");

                //设置查询条件。
                map.AddSearchAttr(StudentAttr.XB);
                map.AddSearchAttr(StudentAttr.FK_BanJi);

                //多对多的映射.
                map.AttrsOfOneVSM.Add(new StudentKeMus(), new KeMus(), StudentKeMuAttr.FK_Student,
                  StudentKeMuAttr.FK_KeMu, KeMuAttr.Name, KeMuAttr.No, "学习的科目");

                //明细表映射.
                map.AddDtl(new Resumes(), ResumeAttr.FK_Stu);

                //带有参数的方法.
                RefMethod rm = new RefMethod();
                rm.Title = "缴纳班费";
                rm.HisAttrs.AddTBDecimal("JinE", 100, "缴纳金额", true, false);
                rm.HisAttrs.AddTBString("Note", null, "备注", true, false,0,100,100);
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
        protected override bool beforeDelete()
        {
            return base.beforeDelete();
        }
        protected override bool beforeUpdate()
        {
            return base.beforeUpdate();
        }
        protected override void afterInsertUpdateAction()
        {
            base.afterInsertUpdateAction();
        }
        protected override void afterDelete()
        {
            base.afterDelete();
        }
        protected override void afterInsert()
        {
            base.afterInsert();
        }
        protected override void afterUpdate()
        {
            base.afterUpdate();
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
    /// 学生s
    /// </summary>
    public class Students : BP.En.EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 学生s
        /// </summary>
        public Students() { }
        #endregion

        #region 重写基类方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Student();
            }
        }
        #endregion 重写基类方法

    }
}
