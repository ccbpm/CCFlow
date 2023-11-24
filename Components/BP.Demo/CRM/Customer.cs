using System;
using BP.En;

namespace BP.Demo
{
    /// <summary>
    /// 客户属性
    /// </summary>
    public class CustomerAttr : BP.En.EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        public const string Addr = "Addr";
        /// <summary>
        /// 密码
        /// </summary>
        public const string Pass = "Pass";
        /// <summary>
        /// Linker
        /// </summary>
        public const string Linker = "Linker";
        /// <summary>
        /// 手机号码
        /// </summary>
        public const string Tel = "Tel";

        public const string KeHuMingCheng = "KeHuMingCheng";
        public const string ShouHuoDiZhi = "ShouHuoDiZhi";
        public const string ShouHuoRen = "ShouHuoRen";
        public const string LianXiFangShi = "LianXiFangShi";

        public const string Creater = "Creater";
        public const string CreaterName = "CreaterName";
        public const string CreatRDT = "CreatRDT";
        #endregion
        public const string SiJiName = "SiJiName";
        public const string SiJiTel = "SiJiTel";
        public const string SiJiCPH = "SiJiCPH";
        public const string NumOfPrj = "NumOfPrj";
        public const string NumOfOK = "NumOfOK";
    }
    /// <summary>
    /// Customer 的摘要说明。
    /// </summary>
    public class Customer : EntityNoName
    {
        #region 属性.
        public string CreatRDT
        {
            get
            {
                return this.GetValStrByKey(CustomerAttr.CreatRDT);
            }
            set
            {

                this.SetValByKey(CustomerAttr.CreatRDT, value);
            }
        }
        public string CreaterName
        {
            get
            {
                return this.GetValStrByKey(CustomerAttr.CreaterName);
            }
            set
            {

                this.SetValByKey(CustomerAttr.CreaterName, value);
            }
        }
        public string Creater
        {
            get
            {
                return this.GetValStrByKey(CustomerAttr.Creater);
            }
            set
            {

                this.SetValByKey(CustomerAttr.Creater, value);
            }
        }
        public string DeptNo
        {
            get
            {
                return this.GetValStrByKey(CustomerAttr.FK_Dept);
            }
            set
            {

                this.SetValByKey(CustomerAttr.FK_Dept, value);
            }
        }
        public string Addr
        {
            get
            {
                return this.GetValStrByKey(CustomerAttr.Addr);
            }
            set
            {

                this.SetValByKey(CustomerAttr.Addr, value);
            }
        }
        public string Tel
        {
            get
            {
                return this.GetValStrByKey(CustomerAttr.Tel);
            }
            set
            {

                this.SetValByKey(CustomerAttr.Tel, value);
            }
        }
        public string ShouHuoDiZhi
        {
            get
            {
                return this.GetValStrByKey(CustomerAttr.ShouHuoDiZhi);
            }
            set
            {

                this.SetValByKey(CustomerAttr.ShouHuoDiZhi, value);
            }
        }

        public string ShouHuoRen
        {
            get
            {
                return this.GetValStrByKey(CustomerAttr.ShouHuoRen);
            }
            set
            {

                this.SetValByKey(CustomerAttr.ShouHuoRen, value);
            }
        }
        public string LianXiFangShi
        {
            get
            {
                return this.GetValStrByKey(CustomerAttr.LianXiFangShi);
            }
            set
            {

                this.SetValByKey(CustomerAttr.LianXiFangShi, value);
            }
        }
        #endregion 属性.

        /// <summary>
        /// 客户
        /// </summary>
        public Customer()
        {
        }
        /// <summary>
        /// 客户编号
        /// </summary>
        /// <param name="_No">No</param>
        public Customer(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                uac.IsUpdate = true;
                uac.IsDelete = true;
                uac.IsInsert = true;
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

                Map map = new Map("Demo_Customer", "客户");
                map.setIsAutoGenerNo(true);
                map.setCodeStruct("4");

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(CustomerAttr.No, null, "编号", true, true, 4, 4, 4);
                map.AddTBString(CustomerAttr.Name, null, "名称", true, false, 0, 100, 100);
                map.AddTBString(CustomerAttr.Linker, null, "联系人", true, false, 0, 100, 100);
                map.AddTBString(CustomerAttr.Tel, null, "联系电话", true, false, 0, 100, 100);
                map.AddTBString(CustomerAttr.Addr, null, "地址", false, false, 0, 200, 100, true);

                map.AddTBDate(CustomerAttr.CreatRDT, null, "创建日期", true, true);
                #endregion 字段

                map.DTSearchKey = CustomerAttr.CreatRDT;
                map.DTSearchWay = BP.Sys.DTSearchWay.ByDate;
                map.DTSearchLabel = "创建日期";

                //     map.AddSearchAttr(CustomerAttr.FK_Dept); //查询条件.
                this._enMap = map;
                return this._enMap;
            }
        }

        public override Entities GetNewEntities
        {
            get { return new Customers(); }
        }
    }
    /// <summary>
    /// 客户s
    /// </summary>
    public class Customers : EntitiesNoName
    {
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Customer();
            }
        }
        /// <summary>
        /// 客户s
        /// </summary>
        public Customers()
        {
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Customer> ToJavaList()
        {
            return (System.Collections.Generic.IList<Customer>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Customer> Tolist()
        {
            System.Collections.Generic.List<Customer> list = new System.Collections.Generic.List<Customer>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Customer)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.


    }
}
