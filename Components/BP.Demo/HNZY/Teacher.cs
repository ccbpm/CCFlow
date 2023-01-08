using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP.DA;
using BP.En;

namespace BP.Demo.HNZY
{
    public class TeacherAttr:EntityNoNameAttr
    {
        #region 个人基本信息字段
        /// <summary>
        /// 身份证号
        /// </summary>
        public static string IDCars = "IDCars";
        /// <summary>
        /// 政治面貌
        /// </summary>
        public static string ZZMM = "ZZMM";
        /// <summary>
        /// 教师类别
        /// </summary>
        public static string TeachersType = "TeachersType";
        /// <summary>
        /// 联系电话
        /// </summary>
        public static string Tel = "Tel";
        /// <summary>
        /// 其他联系方式
        /// </summary>
        public static string OtherTel = "OtherTel";
        /// <summary>
        /// 联系地址
        /// </summary>
        public static string Address = "Address";
        #endregion 个人基本信息字段

        #region 教室业务基本信息字段
        /// <summary>
        /// 最高学历
        /// </summary>
        public static string ZGXL = "ZGXL";
        /// <summary>
        /// 所学专业类别
        /// </summary>
        public static string SXZY = "SXZY";
        /// <summary>
        /// 所学专业名称
        /// </summary>
        public static string ZYMC = "ZYMC";
        /// <summary>
        /// 毕业院校
        /// </summary>
        public static string BYYX = "BYYX";
        /// <summary>
        /// 毕业时间
        /// </summary>
        public static string BYSJ = "BYSJ";
        
        /// <summary>
        /// 最高学位
        /// </summary>
        public static string ZZXW = "ZZXW";
        /// <summary>
        /// 学位类别
        /// </summary>
        public static string XWLB = "XWLB";
        /// <summary>
        /// 学位专业
        /// </summary>
        public static string XWZY = "XWZY";
        /// <summary>
        /// 学位院校
        /// </summary>
        public static string XWYX = "XWYX";
        /// <summary>
        /// 取得学位时间
        /// </summary>
        public static string QDXWSJ = "QDXWSJ";
        /// <summary>
        /// 现任职称
        /// </summary>
        public static string XRZC = "XRZC";
        /// <summary>
        /// 职称系列
        /// </summary>
        public static string  ZCXL= "ZCXL";
        /// <summary>
        /// 聘任时间
        /// </summary>
        public static string PRSJ = "PRSJ";
        /// <summary>
        /// 现任行政职务
        /// </summary>
        public static string XRXZZW = "XRXZZW";
        /// <summary>
        /// 职务级别
        /// </summary>
        public static string ZWJB = "ZWJB";
        /// <summary>
        /// 职务任职时间
        /// </summary>
        public static string ZWRZSJ = "ZWRZSJ";
        /// <summary>
        /// 参加工作时间
        /// </summary>
        public static string CJGZSJ = "CJGZSJ";
        /// <summary>
        /// 从事教育时间
        /// </summary>
        public static string CSJYSJ = "CSJYSJ";
        /// <summary>
        /// 任教课程分类
        /// </summary>
        public static string RJKCFL = "RJKCFL";
        /// <summary>
        /// 任教课程名称
        /// </summary>
        public static string RJKCMC = "RJKCMC";
        #endregion 教室业务基本信息字段
    }
    public class Teacher : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCars
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.IDCars);
            }
        }
        /// <summary>
        /// 政治面貌
        /// </summary>
        public string ZZMM
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.ZZMM);
            }
        }
        /// <summary>
        /// 教师类别
        /// </summary>
        public string TeachersType
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.TeachersType);
            }
        }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.Tel);
            }
        }
        /// <summary>
        /// 其他联系方式
        /// </summary>
        public string OtherTel
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.OtherTel);
            }
        }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.Address);
            }
        }
        /// <summary>
        /// 最高学历
        /// </summary>
        public string ZGXL
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.ZGXL);
            }
        }
        /// <summary>
        /// 所学专业类别
        /// </summary>
        public string SXZY
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.SXZY);
            }
        }
        /// <summary>
        /// 所学专业名称
        /// </summary>
        public string ZYMC
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.ZYMC);
            }
        }
        /// <summary>
        /// 毕业院校
        /// </summary>
        public string BYYX
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.BYYX);
            }
        }
        /// <summary>
        /// 毕业时间
        /// </summary>
        public string BYSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.BYSJ);
            }
        }
        /// <summary>
        /// 最好学位
        /// </summary>
        public string ZZXW
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.ZZXW);
            }
        }
        /// <summary>
        ///学位类别
        /// </summary>
        public string XWLB
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.XWLB);
            }
        }
        /// <summary>
        /// 学位专业
        /// </summary>
        public string XWZY
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.XWZY);
            }
        }
        /// <summary>
        /// 学位院校
        /// </summary>
        public string XWYX
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.XWYX);
            }
        }
        /// <summary>
        /// 取得学位时间
        /// </summary>
        public string QDXWSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.QDXWSJ);
            }
        }
        /// <summary>
        /// 现任职称
        /// </summary>
        public string XRZC
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.XRZC);
            }
        }
        /// <summary>
        /// 职称系列
        /// </summary>
        public string ZCXL
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.ZCXL);
            }
        }
        /// <summary>
        /// 聘任时间
        /// </summary>
        public string PRSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.PRSJ);
            }
        }
        /// <summary>
        /// 现任行政职务
        /// </summary>
        public string XRXZZW
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.XRXZZW);
            }
        }
        /// <summary>
        /// 职务级别
        /// </summary>
        public string ZWJB
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.ZWJB);
            }
        }
        /// <summary>
        /// 任职时间
        /// </summary>
        public string ZWRZSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.ZWRZSJ);
            }
        }
        /// <summary>
        /// 参加工作时间
        /// </summary>
        public string CJGZSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.CJGZSJ);
            }
        }
        /// <summary>
        /// 从事教育时间
        /// </summary>
        public string CSJYSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.CSJYSJ);
            }
        }
        /// <summary>
        /// 任教课程分类
        /// </summary>
        public string RJKCFL
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.RJKCFL);
            }
        }
        /// <summary>
        /// 任教课程名称
        /// </summary>
        public string RJKCMC
        {
            get
            {
                return this.GetValStrByKey(TeacherAttr.RJKCMC);
            }
        }
        #endregion
        #region 按钮权限控制
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenAll();
                return uac;
            }
        }
        #endregion
        #region 构造方法
        /// <summary>
        /// 权限中心
        /// </summary>
        public Teacher()
        {
        }
        /// <summary>
        /// 权限中心
        /// </summary>
        /// <param name="no"></param>
        public Teacher(string no)
        {
            this.No = no;
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

                Map map = new Map("Demo_Teacher");
                map.DepositaryOfEntity = Depositary.None;
                map.EnDesc = "教师信息管理";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(SchoolManagerAttr.No, null, "教师编码", true, true, 0, 100, 300);
                map.AddTBString(SchoolManagerAttr.Name, null, "姓名", true, false, 0, 100, 300);
                map.AddTBString(TeacherAttr.IDCars, null, "身份证号", true, false, 0, 300, 200);
                map.AddDDLEntities(TeacherAttr.ZZMM, null, "政治面貌", new ZZMMs(), true);
                map.AddDDLSysEnum(TeacherAttr.TeachersType, 0, "教师类别", true, true, TeacherAttr.TeachersType, "@0=兼职@1=非兼职", true);
                map.AddTBString(TeacherAttr.Tel, null, "联系电话", true, false, 0, 300, 200);
                map.AddTBString(TeacherAttr.OtherTel, null, "其他联系方式", true, false, 0, 300, 200);
                map.AddTBString(TeacherAttr.Address, null, "联系地址", true, false, 0, 300, 200);

                map.AddDDLEntities(TeacherAttr.ZGXL, null, "最高学历", new Educations(), true);
                map.AddDDLEntities(TeacherAttr.SXZY, null, "所学专业类别", new MajorCategorys(), true);
                map.AddTBString(TeacherAttr.ZYMC, null, "所学专业名称", true, false, 0, 300, 200);
                map.AddTBString(TeacherAttr.BYYX, null, "毕业院校", true, false, 0, 300, 200);
                map.AddTBDate(TeacherAttr.BYSJ, "毕业时间", true, false);
                map.AddDDLSysEnum(TeacherAttr.ZZXW, 0, "最高学位", true, true, TeacherAttr.ZZXW, "@0=无@1=学士@2=硕士@3=博士", true);
                map.AddDDLEntities(TeacherAttr.XWLB, null, "学位类别", new DegreeCategorys(), true);
                map.AddTBString(TeacherAttr.XWZY, null, "学位专业", true, false, 0, 300, 200);
                map.AddTBString(TeacherAttr.XWYX, null, "学位院校", true, false, 0, 300, 200);
                map.AddTBDate(TeacherAttr.QDXWSJ, "取得学位时间", true, false);

                map.AddDDLEntities(TeacherAttr.XRZC, null, "现任职称", new TechnicalTitles(), true);
                map.AddDDLEntities(TeacherAttr.ZCXL, null, "职称系列", new TitelTypes(), true);
                map.AddTBDate(TeacherAttr.PRSJ, "聘任时间", true, false);
                map.AddTBString(TeacherAttr.XRXZZW, null, "现任行政职务", true, false, 0, 300, 200);
                map.AddDDLEntities(TeacherAttr.ZWJB, null, "职务级别", new JobLevels(), true);
                map.AddTBDate(TeacherAttr.ZWRZSJ, "任职时间", true, false);

                map.AddTBDate(TeacherAttr.CJGZSJ, "参加工作时间", true, false);
                map.AddTBDate(TeacherAttr.CSJYSJ, "从事教育时间", true, false);

                map.AddDDLEntities(TeacherAttr.RJKCFL, null, "任教课程分类", new Classifications(), true);
                map.AddTBString(TeacherAttr.RJKCMC, null, "任教课程名称", true, false, 0, 300, 200);

                

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    public class Teachers : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Teacher();
            }
        }
        /// <summary>
        /// 教师s
        /// </summary>
        public Teachers()
        {
        }
        /// <summary>
        /// 教师s
        /// </summary>
        public Teachers(string no)
        {

            this.Retrieve(TeacherAttr.No, no);

        }
        #endregion 构造方法


        #region 重写查询,add by zhoupeng 2015.09.30 为了适应能够从 webservice 数据源查询数据.
        /// <summary>
        /// 重写查询全部适应从WS取数据需要
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            //if (BP.Web.WebUser.No != "admin")
            //    throw new Exception("@您没有查询的权限.");


            return base.RetrieveAll();

        }
        /// <summary>
        /// 重写重数据源查询全部适应从WS取数据需要
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAllFromDBSource()
        {

            return base.RetrieveAllFromDBSource();

        }
        #endregion 重写查询.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Teacher> ToJavaList()
        {
            return (System.Collections.Generic.IList<Teacher>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Teacher> Tolist()
        {
            System.Collections.Generic.List<Teacher> list = new System.Collections.Generic.List<Teacher>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Teacher)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
