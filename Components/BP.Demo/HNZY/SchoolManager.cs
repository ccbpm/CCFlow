using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP.DA;
using BP.En;

namespace BP.Demo.HNZY
{
    public enum XZ
    { 
        /// <summary>
        /// 公办
        /// </summary>
        GB=0,
        /// <summary>
        /// 民办
        /// </summary>
        MB=1,
        /// <summary>
        /// 其他
        /// </summary>
        QT=2
    }
    public class SchoolManagerAttr:EntityNoNameAttr
    {
        /// <summary>
        /// 学校简称
        /// </summary>
        public static string PathName = "PathName";
        /// <summary>
        /// 所属教育局
        /// </summary>
        public static string Department = "Department";
        /// <summary>
        /// 招生代码
        /// </summary>
        public static string ZSDM = "ZSDM";
        /// <summary>
        /// 性质
        /// </summary>
        public static string XZ = "ZX";
        /// <summary>
        /// 类别
        /// </summary>
        public static string LB = "LB";
        /// <summary>
        /// 评估类别
        /// </summary>
        public static string PGLB = "PGLB";
        /// <summary>
        /// 学校所在地
        /// </summary>
        public static string SZD = "SZD";
        /// <summary>
        /// 详细地址
        /// </summary>
        public static string Address = "Address";
        /// <summary>
        /// 邮政编码
        /// </summary>
        public static string YZBM = "YZBM";
        /// <summary>
        /// 联系电话
        /// </summary>
        public static string Tel = "Tel";
        /// <summary>
        /// 网址
        /// </summary>
        public static string WZ = "WZ";
    }
    public class SchoolManager : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 简称
        /// </summary>
        public string PathName
        {
            get
            {
                return this.GetValStrByKey(SchoolManagerAttr.PathName);
            }
        }
        /// <summary>
        /// 属于教育局
        /// </summary>
        public string Department
        {
            get
            {
                return this.GetValStrByKey(SchoolManagerAttr.Department);
            }
        }
        /// <summary>
        /// 招生代码
        /// </summary>
        public string ZSDM
        {
            get
            {
                return this.GetValStrByKey(SchoolManagerAttr.ZSDM);
            }
        }
        /// <summary>
        /// 性质
        /// </summary>
        public int XZ
        {
            get
            {
                return this.GetValIntByKey(SchoolManagerAttr.XZ);
            }
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string LB
        {
            get
            {
                return this.GetValStrByKey(SchoolManagerAttr.LB);
            }
        }
        /// <summary>
        /// 评估类别
        /// </summary>
        public string PGLB
        {
            get
            {
                return this.GetValStrByKey(SchoolManagerAttr.PGLB);
            }
        }
        /// <summary>
        /// 学校所在地
        /// </summary>
        public string SZD
        {
            get
            {
                return this.GetValStrByKey(SchoolManagerAttr.SZD);
            }
        }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address
        {
            get
            {
                return this.GetValStrByKey(SchoolManagerAttr.Address);
            }
        }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string YZBM
        {
            get
            {
                return this.GetValStrByKey(SchoolManagerAttr.YZBM);
            }
        }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStrByKey(SchoolManagerAttr.Tel);
            }
        }
        /// <summary>
        /// 网址
        /// </summary>
        public string WZ
        {
            get
            {
                return this.GetValStrByKey(SchoolManagerAttr.WZ);
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
        public SchoolManager()
        {
        }
        /// <summary>
        /// 权限中心
        /// </summary>
        /// <param name="no"></param>
        public SchoolManager(string no)
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

                Map map = new Map("Demo_SchoolManager");
                map.DepositaryOfEntity = Depositary.None;
                map.EnDesc = "省属学校管理";
                map.EnType = EnType.Sys;


                map.AddTBStringPK(SchoolManagerAttr.No, null, "编码", true, true, 0, 100, 300);
                map.AddTBString(SchoolManagerAttr.Name,null,"全称",true,false,0,100,300);
                map.AddTBString(SchoolManagerAttr.PathName, null, "简称", true, false, 0, 300, 200);
                map.AddDDLEntities(SchoolManagerAttr.Department, null, "主管教育局", new CityDepartments(), true);
                map.AddTBString(SchoolManagerAttr.ZSDM, null, "招生代码", true, false, 0, 300, 200);

                map.AddDDLSysEnum(SchoolManagerAttr.XZ, 0, "性质", true, true, SchoolManagerAttr.XZ, "@0=公办@1=民办@2=其他",true);
                map.AddDDLSysEnum(SchoolManagerAttr.LB,0,"类别",true,true, SchoolManagerAttr.LB, "@0=普通中专@1=高等中专",true);
                map.AddDDLSysEnum(SchoolManagerAttr.PGLB,0, "评估类型",true,true, SchoolManagerAttr.PGLB, "@0=未评估@1=普通中专@2=高等中专",true);

                map.AddDDLEntities(SchoolManagerAttr.SZD, null, "学校所在地", new BP.CN.Citys(), true);

                map.AddTBString(SchoolManagerAttr.Address, null, "详细地址", true, false, 0, 300, 300);
                map.AddTBString(SchoolManagerAttr.YZBM, null, "邮政编码", true, false, 0, 100, 200);
                map.AddTBString(SchoolManagerAttr.Tel, null, "联系电话", true, false, 0, 300, 100);
                map.AddTBString(SchoolManagerAttr.WZ, null, "网址", true, false, 0, 100, 200);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    public class SchoolManagers : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SchoolManager();
            }
        }
        /// <summary>
        /// 学校s
        /// </summary>
        public SchoolManagers()
        {
        }
        /// <summary>
        /// 学校s
        /// </summary>
        public SchoolManagers(string no)
        {

            this.Retrieve(SchoolManagerAttr.No, no);

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
        public System.Collections.Generic.IList<SchoolManager> ToJavaList()
        {
            return (System.Collections.Generic.IList<SchoolManager>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SchoolManager> Tolist()
        {
            System.Collections.Generic.List<SchoolManager> list = new System.Collections.Generic.List<SchoolManager>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SchoolManager)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
