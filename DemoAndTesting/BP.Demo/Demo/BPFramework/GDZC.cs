using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
    /// <summary>
    /// 固定资产 属性
    /// </summary>
    public class GDZCAttr : EntityNoNameAttr
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
        public const string jinE = "jinE";
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
        #endregion

        /// <summary>
        /// 片区
        /// </summary>
        public const string FK_PQ = "FK_PQ";
        /// <summary>
        /// 省份
        /// </summary>
        public const string FK_SF = "FK_SF";
        /// <summary>
        /// 城市
        /// </summary>
        public const string FK_City = "FK_City";
    }
    /// <summary>
    /// 固定资产
    /// </summary>
    public class GDZC : BP.En.EntityNoName
    {
        #region 属性
        /// <summary>
        /// 登录系统密码
        /// </summary>
        public string PWD
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.PWD);
            }
            set
            {
                this.SetValByKey(GDZCAttr.PWD, value);
            }
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public int jinE
        {
            get
            {
                return this.GetValIntByKey(GDZCAttr.jinE);
            }
            set
            {
                this.SetValByKey(GDZCAttr.jinE, value);
            }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.Addr);
            }
            set
            {
                this.SetValByKey(GDZCAttr.Addr, value);
            }
        }
        /// <summary>
        /// 性别
        /// </summary>
        public int XB
        {
            get
            {
                return this.GetValIntByKey(GDZCAttr.XB);
            }
            set
            {
                this.SetValByKey(GDZCAttr.XB, value);
            }
        }
        /// <summary>
        /// 性别名称
        /// </summary>
        public string XBText
        {
            get
            {
                return this.GetValRefTextByKey(GDZCAttr.XB);
            }
        }
        /// <summary>
        /// 班级编号
        /// </summary>
        public string FK_BanJi
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.FK_BanJi);
            }
            set
            {
                this.SetValByKey(GDZCAttr.FK_BanJi, value);
            }
        }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string FK_BanJiText
        {
            get
            {
                return this.GetValRefTextByKey(GDZCAttr.FK_BanJi);
            }
        }
        /// <summary>
        /// 邮件
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.Email);
            }
            set
            {
                this.SetValByKey(GDZCAttr.Email, value);
            }
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.Tel);
            }
            set
            {
                this.SetValByKey(GDZCAttr.Tel, value);
            }
        }
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegDate
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.RegDate);
            }
            set
            {
                this.SetValByKey(GDZCAttr.RegDate, value);
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
                //  uac.LoadRightFromCCGPM(this); //从GPM里面装载.
                // return uac;
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = true;
                    uac.IsView = true;
                }
                else
                {
                    uac.IsView = true;
                }
                uac.IsImp = true;
                return uac;
            }
        }
        /// <summary>
        /// 固定资产
        /// </summary>
        public GDZC()
        {
        }
        /// <summary>
        /// 固定资产
        /// </summary>
        /// <param name="no"></param>
        public GDZC(string no)
            : base(no)
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

                Map map = new Map("Demo_GDZC", "固定资产");

                //基础信息.
                map.IsAllowRepeatName = true; //是否允许名称重复.
                map.IsAutoGenerNo = true; //是否自动生成编号.
                map.Java_SetCodeStruct("4"); // 4位数的编号，从 0001 开始，到 9999.

                #region 字段映射 - 普通字段.
                map.AddTBStringPK(GDZCAttr.No, null, "固定资产编号", true, true, 4, 4, 90); // 如果设置自动编号字段必须是只读的.
                map.AddTBString(GDZCAttr.Name, null, "名称", true, false, 0, 200, 70);

                map.AddTBString(GDZCAttr.Addr, null, "地址", true, false, 0, 200, 100, true);
                map.AddTBInt(GDZCAttr.jinE, 18, "金额", true, false);
                map.AddTBInt("Yuanzhi", 18, "原值", true, false);


                #endregion 字段映射 - 普通字段.

                map.AddMyFile("照片");
                //map.AddMyFileS("简历");

                //#region 设置查询条件.
                ////string类型，多关键字查询条件.
                //map.SearchFields = "@地址=" + GDZCAttr.Addr;
                //map.SearchFields += "@电话=" + GDZCAttr.Tel;

                ////数值类型，范围查询条件.
                //map.SearchFieldsOfNum = "@年龄=" + GDZCAttr.jinE;

                ////设置日期查询条件.
                //map.DTSearchKey = GDZCAttr.RegDate;
                //map.DTSearchLable = "注册日期";
                //map.DTSearchWay = Sys.DTSearchWay.ByDate; 

                ////外键与枚举。
                //map.AddSearchAttr(GDZCAttr.XB,1001); //宽度大于1000就是换行.
                //map.AddSearchAttr(GDZCAttr.FK_BanJi);

                ////隐藏的查询条件.
                ////map.AddHidden("XB", " = ", "0");
                //#endregion 设置查询条件.


                //#region 与其他实体的映射.
                ////多对多的映射.
                //map.AttrsOfOneVSM.Add(new GDZCKeMus(), new KeMus(), GDZCKeMuAttr.FK_GDZC,
                //  GDZCKeMuAttr.FK_KeMu, KeMuAttr.Name, KeMuAttr.No, "选修的科目");

                ////明细表映射.
                //map.AddDtl(new Resumes(), ResumeAttr.RefPK);
                //#endregion 与其他实体的映射.

               // #region 方法映射.

               // //带有参数的方法.
               // RefMethod rm = new RefMethod();
               // rm.Title = "缴纳班费";
               // rm.HisAttrs.AddTBDecimal("JinE", 100, "缴纳金额", true, false);
               // rm.HisAttrs.AddTBString("Note", null, "备注", true, false, 0, 100, 100);
               //// rm.HisAttrs.AddTBString("Nowete", null, "22备注", true, false, 0, 100, 100);
               // rm.ClassMethodName = this.ToString() + ".DoJiaoNaBanFei";
               // rm.GroupName = "功能执行测试";
               // //  rm.IsCanBatch = false; //是否可以批处理？
               // map.AddRefMethod(rm);

               // //不带有参数的方法.
               // rm = new RefMethod();
               // rm.Title = "注销学籍";
               // rm.Warning = "您确定要注销吗？";
               // rm.ClassMethodName = this.ToString() + ".DoZhuXiao";
               // rm.IsForEns = true;
               // rm.IsCanBatch = true; //是否可以批处理？
               // map.AddRefMethod(rm);

               // //不带有参数的方法.
               // rm = new RefMethod();
               // rm.Title = "发起xx流程";
               // rm.ClassMethodName = this.ToString() + ".DoStartFlow";
               // rm.RefMethodType = RefMethodType.LinkeWinOpen;
               // rm.IsCanBatch = false; //是否可以批处理？
               // map.AddRefMethod(rm);

               // //不带有参数的方法.
               // rm = new RefMethod();
               // rm.Title = "打印固定资产证";
               // rm.ClassMethodName = this.ToString() + ".DoPrintStuLicence";
               // rm.IsCanBatch = true; //是否可以批处理？
               // map.AddRefMethod(rm);
               // #endregion 方法映射.

                ////不带有参数的方法.
                //rm = new RefMethod();
                //rm.Title = "批量打印固定资产证";
                //rm.ClassMethodName = this.ToString() + ".EnsMothed";
                ////rm.IsForEns = true; //是否可以批处理？
                //rm.RefMethodType = RefMethodType.FuncBacthEntities; //是否可以批处理？
                //map.AddRefMethod(rm);

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
            return base.beforeUpdateInsertAction();
        }

        #endregion 重写基类方法

        #region 方法
        public string DoPrintStuLicence()
        {

            BP.Pub.RTFEngine en = new Pub.RTFEngine();

            GDZC stu = new GDZC(this.No);

            en.HisGEEntity = stu; //当前的实体.

            //增加从表.
            BP.Demo.Resumes dtls = new Resumes();
            dtls.Retrieve(ResumeAttr.RefPK, stu.No);
            en.AddDtlEns(dtls);

            string saveTo = BP.Sys.SystemConfig.PathOfTemp; // \\DataUser\\Temp\\
            string billFileName = this.No + "StuTest.doc";

            //要生成的数据.
            en.MakeDoc(BP.Sys.SystemConfig.PathOfDataUser + "\\CyclostyleFile\\GDZCDemo.rtf", saveTo, billFileName, false);

            string url = "/DataUser/Temp/" + billFileName;

            string info = "单据生成成功:<a href='" + url + "' >打印</a>，<a href='/SDKFlowDemo/App/PrintJoin.aspx'>拼接打印</a>";
            return info;
        }
        public string DoStartFlow()
        {
            return "/WF/MyFlow.htm?FK_Flow=001&FK_Studept=" + this.No + "&StuName=" + this.Name;
        }
        /// <summary>
        /// 带有参数的方法:缴纳班费
        /// 说明：都要返回string类型.
        /// </summary>
        /// <returns></returns>
        public string DoJiaoNaBanFei(decimal jine, string note)
        {
            return "学号:" + this.No + ",姓名:" + this.Name + ",缴纳了:" + jine + "元,说明:" + note;
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

        protected override bool beforeDelete()
        {

            return base.beforeDelete();
        }

    }
    /// <summary>
    /// 固定资产s
    /// </summary>
    public class GDZCs : BP.En.EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 固定资产s
        /// </summary>
        public GDZCs() { }
        #endregion

        #region 重写基类方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GDZC();
            }
        }
        #endregion 重写基类方法

        #region 测试方法.
        public string EnsMothed()
        {
            return "EnsMothed@执行成功.";
        }
        public string EnsMothedParas(string para1, string para2)
        {
            return "EnsMothedParas@执行成功." + para1 + " - " + para2;
        }
        #endregion

    }
}
