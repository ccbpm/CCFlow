using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP.DA;
using BP.En;

namespace BP.Demo.HNZY
{
    public class TeacherManageAttr : EntityNoNameAttr
    {
        #region 个人基本信息字段
        /// <summary>
        /// 身份证号
        /// </summary>
        public static string IDCars = "IDCars";
        /// <summary>
        /// 出生日期
        /// </summary>
        public static string CSRQ = "CSRQ";
        /// <summary>
        /// 性别
        /// </summary>
        public static string Sex = "Sex";
        /// <summary>
        /// 政治面貌
        /// </summary>
        public static string ZZMM = "ZZMM";
        /// <summary>
        /// 教师类别
        /// </summary>
        public static string TeachersType = "TeachersType";
        /// <summary>
        /// 婚姻状况
        /// </summary>
        public static string HYZK = "HYZK";
        /// <summary>
        /// 是否在编
        /// </summary>
        public static string SFZB = "SFZB";
        /// <summary>
        /// 联系地址
        /// </summary>
        public static string Address = "Address";
        /// <summary>
        /// Email
        /// </summary>
        public static string Email = "Email";
        /// <summary>
        /// 所属学校
        /// </summary>
        public static string SSXX = "SSXX";
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
        public static string ZCXL = "ZCXL";
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
        /// 教师类别
        /// </summary>
        public static string JSLB = "JSLB";
        /// <summary>
        /// 任教课程分类
        /// </summary>
        public static string RJKCFL = "RJKCFL";
        /// <summary>
        /// 任教课程名称
        /// </summary>
        public static string RJKCMC = "RJKCMC";
        /// <summary>
        /// 是否双师
        /// </summary>
        public static string SFSS = "SFSS";
        /// <summary>
        /// 双师证书级别
        /// </summary>
        public static string SSZSJB = "SSZSJB";
        /// <summary>
        /// 双师证书名称
        /// </summary>
        public static string SSZSMC = "SSZSMC";
        /// <summary>
        /// 录入时间
        /// </summary>
        public static string SSRLSJ = "SSRLSJ";
        #endregion 教室业务基本信息字段
    }
    public class TeacherManage : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCars
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.IDCars);
            }
        }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string CSRQ
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.CSRQ);
            }
        }
        /// <summary>
        /// 所属学校
        /// </summary>
        public string SSXX
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.SSXX);
            }
        }
        /// <summary>
        /// 政治面貌
        /// </summary>
        public int Sex
        {
            get
            {
                return this.GetValIntByKey(TeacherManageAttr.Sex);
            }
        }
        /// <summary>
        /// 政治面貌
        /// </summary>
        public string ZZMM
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.ZZMM);
            }
        }
        /// <summary>
        /// 教师类别
        /// </summary>
        public string TeachersType
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.TeachersType);
            }
        }
        /// <summary>
        /// 婚姻状况
        /// </summary>
        public int HYZK
        {
            get
            {
                return this.GetValIntByKey(TeacherManageAttr.HYZK);
            }
        }
        /// <summary>
        /// 是否在编
        /// </summary>
        public int SFZB
        {
            get
            {
                return this.GetValIntByKey(TeacherManageAttr.SFZB);
            }
        }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.Email);
            }
        }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.Address);
            }
        }
        /// <summary>
        /// 最高学历
        /// </summary>
        public string ZGXL
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.ZGXL);
            }
        }
        /// <summary>
        /// 所学专业类别
        /// </summary>
        public string SXZY
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.SXZY);
            }
        }
        /// <summary>
        /// 所学专业名称
        /// </summary>
        public string ZYMC
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.ZYMC);
            }
        }
        /// <summary>
        /// 毕业院校
        /// </summary>
        public string BYYX
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.BYYX);
            }
        }
        /// <summary>
        /// 毕业时间
        /// </summary>
        public string BYSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.BYSJ);
            }
        }
        /// <summary>
        /// 最好学位
        /// </summary>
        public string ZZXW
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.ZZXW);
            }
        }
        /// <summary>
        ///学位类别
        /// </summary>
        public string XWLB
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.XWLB);
            }
        }
        /// <summary>
        /// 学位专业
        /// </summary>
        public string XWZY
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.XWZY);
            }
        }
        /// <summary>
        /// 学位院校
        /// </summary>
        public string XWYX
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.XWYX);
            }
        }
        /// <summary>
        /// 取得学位时间
        /// </summary>
        public string QDXWSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.QDXWSJ);
            }
        }
        /// <summary>
        /// 现任职称
        /// </summary>
        public string XRZC
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.XRZC);
            }
        }
        /// <summary>
        /// 职称系列
        /// </summary>
        public string ZCXL
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.ZCXL);
            }
        }
        /// <summary>
        /// 聘任时间
        /// </summary>
        public string PRSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.PRSJ);
            }
        }
        /// <summary>
        /// 现任行政职务
        /// </summary>
        public string XRXZZW
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.XRXZZW);
            }
        }
        /// <summary>
        /// 职务级别
        /// </summary>
        public string ZWJB
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.ZWJB);
            }
        }
        /// <summary>
        /// 任职时间
        /// </summary>
        public string ZWRZSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.ZWRZSJ);
            }
        }
        /// <summary>
        /// 参加工作时间
        /// </summary>
        public string CJGZSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.CJGZSJ);
            }
        }
        /// <summary>
        /// 从事教育时间
        /// </summary>
        public string CSJYSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.CSJYSJ);
            }
        }
        /// <summary>
        /// 任教课程分类
        /// </summary>
        public string RJKCFL
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.RJKCFL);
            }
        }
        /// <summary>
        /// 任教课程名称
        /// </summary>
        public string RJKCMC
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.RJKCMC);
            }
        }
        /// <summary>
        /// 教师类别
        /// </summary>
        public int JSLB
        {
            get
            {
                return this.GetValIntByKey(TeacherManageAttr.JSLB);
            }
        }
        /// <summary>
        /// 是否双师
        /// </summary>
        public int SFSS
        {
            get
            {
                return this.GetValIntByKey(TeacherManageAttr.SFSS);
            }
        }
        /// <summary>
        /// 双师证书级别
        /// </summary>
        public int SSZSJB
        {
            get
            {
                return this.GetValIntByKey(TeacherManageAttr.SSZSJB);
            }
        }
        /// <summary>
        /// 双师证书名称
        /// </summary>
        public string SSZSMC
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.SSZSMC);
            }
        }
        /// <summary>
        /// 录入时间
        /// </summary>
        public string SSRLSJ
        {
            get
            {
                return this.GetValStrByKey(TeacherManageAttr.SSRLSJ);
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
        public TeacherManage()
        {
        }
        /// <summary>
        /// 权限中心
        /// </summary>
        /// <param name="no"></param>
        public TeacherManage(string no)
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

                map.AddTBStringPK(TeacherManageAttr.No, null, "教师编码", true, true, 0, 100, 300);
                map.AddTBString(TeacherManageAttr.Name, null, "姓名", true, true, 0, 100, 300);
                map.AddTBString(TeacherManageAttr.IDCars, null, "身份证号", true, true, 0, 300, 200);
                map.AddTBDate(TeacherManageAttr.CSRQ, "出生日期", true, false);
                map.AddDDLSysEnum(TeacherManageAttr.Sex, 0, "性别", true, true, TeacherManageAttr.Sex, "@0=男@1=女", true);
                map.AddDDLEntities(TeacherManageAttr.ZZMM, null, "政治面貌", new ZZMMs(), true);
                map.AddDDLSysEnum(TeacherManageAttr.TeachersType, 0, "教师类别", true, true, TeacherManageAttr.TeachersType, "@0=兼职@1=非兼职", true);
                map.AddDDLSysEnum(TeacherManageAttr.HYZK, 0, "婚姻状况", true, true, TeacherManageAttr.HYZK, "@0=未婚@1=已婚", true);
                map.AddDDLSysEnum(TeacherManageAttr.SFZB, 0, "是否在编", true, true, TeacherManageAttr.SFZB, "@0=否@1=是", true);
                map.AddTBString(TeacherManageAttr.Address, null, "联系地址", true, false, 0, 300, 200);
                map.AddTBString(TeacherManageAttr.Email, null, "邮箱", true, false, 0, 300, 200);
                map.AddDDLEntities(TeacherManageAttr.SSXX, null, "所属学校", new SchoolManagers(), true);

                map.AddDDLEntities(TeacherManageAttr.ZGXL, null, "最高学历", new Educations(), true);
                map.AddDDLEntities(TeacherManageAttr.SXZY, null, "所学专业类别", new MajorCategorys(), true);
                map.AddTBString(TeacherManageAttr.ZYMC, null, "所学专业名称", true, false, 0, 300, 200);
                map.AddTBString(TeacherManageAttr.BYYX, null, "毕业院校", true, false, 0, 300, 200);
                map.AddTBDate(TeacherManageAttr.BYSJ, "毕业时间", true, false);
                map.AddDDLSysEnum(TeacherManageAttr.ZZXW, 0, "最高学位", true, true, TeacherManageAttr.ZZXW, "@0=无@1=学士@2=硕士@3=博士", true);
                map.AddDDLEntities(TeacherManageAttr.XWLB, null, "学位类别", new DegreeCategorys(), true);
                map.AddTBString(TeacherManageAttr.XWZY, null, "学位专业", true, false, 0, 300, 200);
                map.AddTBString(TeacherManageAttr.XWYX, null, "学位院校", true, false, 0, 300, 200);
                map.AddTBDate(TeacherManageAttr.QDXWSJ, "取得学位时间", true, false);

                map.AddDDLEntities(TeacherManageAttr.XRZC, null, "现任职称", new TechnicalTitles(), true);
                map.AddDDLEntities(TeacherManageAttr.ZCXL, null, "职称系列", new TitelTypes(), true);
                map.AddTBDate(TeacherManageAttr.PRSJ, "聘任时间", true, false);
                map.AddTBString(TeacherManageAttr.XRXZZW, null, "现任行政职务", true, false, 0, 300, 200);
                map.AddDDLEntities(TeacherManageAttr.ZWJB, null, "职务级别", new JobLevels(), true);
                map.AddTBDate(TeacherManageAttr.ZWRZSJ, "任职时间", true, false);

                map.AddTBDate(TeacherManageAttr.CJGZSJ, "参加工作时间", true, false);
                map.AddTBDate(TeacherManageAttr.CSJYSJ, "从事教育时间", true, false);

                map.AddDDLSysEnum(TeacherManageAttr.JSLB, 0, "教师类别", true, true, TeacherManageAttr.JSLB, "@0=专任@1=兼任", true);
                map.AddDDLEntities(TeacherManageAttr.RJKCFL, null, "任教课程分类", new Classifications(), true);
                map.AddTBString(TeacherManageAttr.RJKCMC, null, "任教课程名称", true, false, 0, 300, 200);

                map.AddDDLSysEnum(TeacherManageAttr.SFSS, 0, "是否双师型", true, true, TeacherManageAttr.SFSS, "@0=否@1=是", true);
                map.AddDDLSysEnum(TeacherManageAttr.SSZSJB, 0, "双师证书级别", true, true, TeacherManageAttr.SSZSJB, "@0=无@1=一级@2=二级", true);
                map.AddTBString(TeacherManageAttr.SSZSMC, null, "双师证书名称", true, false, 0, 300, 200);
                map.AddTBDate(TeacherManageAttr.SSRLSJ, "录入时间", true, false);

                //map.AddSearchAttr(TeacherManageAttr.No);
                //map.AddSearchAttr(TeacherManageAttr.IDCars);
                map.AddSearchAttr(TeacherManageAttr.SSXX);
                //map.AddSearchAttr(TeacherManageAttr.Name);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    public class TeacherManages : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TeacherManage();
            }
        }
        /// <summary>
        /// 教师s
        /// </summary>
        public TeacherManages()
        {
        }
        /// <summary>
        /// 教师s
        /// </summary>
        public TeacherManages(string no)
        {

            this.Retrieve(TeacherManageAttr.No, no);

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
        public System.Collections.Generic.IList<TeacherManage> ToJavaList()
        {
            return (System.Collections.Generic.IList<TeacherManage>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TeacherManage> Tolist()
        {
            System.Collections.Generic.List<TeacherManage> list = new System.Collections.Generic.List<TeacherManage>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TeacherManage)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
