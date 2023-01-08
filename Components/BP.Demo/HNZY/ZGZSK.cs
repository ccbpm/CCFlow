using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP.DA;
using BP.En;

namespace BP.Demo.HNZY
{
    /// <summary>
    /// 资格证书库管理
    /// </summary>
    public class ZGZSKAttr : EntityNoNameAttr
    {
        public static string Idx = "Idx";

        #region 资格证书库管理
        /// <summary>
        /// 专业代码及名称
        /// </summary>
        public static string ZYDMJMC = "ZYDMJMC";
        /// <summary>
        /// 职业（职称）等级
        /// </summary>
        public static string ZYDJ = "ZYDJ";
        /// <summary>
        /// 双师最低合格等级
        /// </summary>
        public static string SSZDHGDJ = "SSZDHGDJ";
        /// <summary>
        /// 认定形式
        /// </summary>
        public static string RDXS = "RDXS";
        /// <summary>
        /// 发证部门
        /// </summary>
        public static string FZBM = "FZBM";

        #endregion 资格证书库管理上
    }
    public class ZGZSK : EntityNoName
    {
        /// <summary>
        /// 显示序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(ZGZSKAttr.Idx);
            }
        }

        #region 属性
        /// <summary>
        /// 专业代码及名称
        /// </summary>
        public string ZYDMJMC
        {
            get
            {
                return this.GetValStrByKey(ZGZSKAttr.ZYDMJMC);
            }
        }
        /// <summary>
        /// 职业（职称）等级
        /// </summary>
        public string ZYDJ
        {
            get
            {
                return this.GetValStrByKey(ZGZSKAttr.ZYDJ);
            }
        }
        /// <summary>
        /// 双师最低合格等级
        /// </summary>
        public string SSZDHGDJ
        {
            get
            {
                return this.GetValStrByKey(ZGZSKAttr.SSZDHGDJ);
            }
        }
        /// <summary>
        /// 认定形式
        /// </summary>
        public string RDXS
        {
            get
            {
                return this.GetValStrByKey(ZGZSKAttr.RDXS);
            }
        }
        /// <summary>
        /// 发证部门
        /// </summary>
        public string FZBM
        {
            get
            {
                return this.GetValStrByKey(ZGZSKAttr.FZBM);
            }
        }
        #endregion


        #region 构造函数
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// 双师认定批次
        /// </summary>		
        public ZGZSK() { }
        public ZGZSK(string no)
            : base(no)
        {
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Demo_ZGZSK", "资格证书库管理");

                #region 基本属性
                map.CodeStruct = "4";
                #endregion

                #region 字段
                map.AddTBStringPK(ZGZSKAttr.No, null, "编号", true, false, 0, 50, 50);
                map.AddTBString(ZGZSKAttr.ZYDMJMC, null, "专业代码及名称", true, false, 0, 50, 200);
                map.AddTBString(ZGZSKAttr.Name, null, "证书名称", true, false, 0, 50, 200);
                map.AddTBInt(ZGZSKAttr.Idx, 0, "显示序号", false, false);
                map.AddTBString(ZGZSKAttr.ZYDJ, null, "职业（职称）等级", true, false, 0, 50, 200);
                map.AddTBString(ZGZSKAttr.SSZDHGDJ, null, "双师最低合格等级", true, false, 0, 50, 200);
                map.AddTBString(ZGZSKAttr.RDXS, null, "认定形式", true, false, 0, 50, 200);
                map.AddTBString(ZGZSKAttr.FZBM, null, "发证部门", true, false, 0, 50, 200);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    public class ZGZSKs : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ZGZSK();
            }
        }
        /// <summary>
        /// 双师认定s
        /// </summary>
        public ZGZSKs()
        {
        }
        /// <summary>
        /// 双师认定s
        /// </summary>
        public ZGZSKs(string no)
        {

            this.Retrieve(ZGZSKAttr.No, no);

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
        public System.Collections.Generic.IList<ZGZSK> ToJavaList()
        {
            return (System.Collections.Generic.IList<ZGZSK>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ZGZSK> Tolist()
        {
            System.Collections.Generic.List<ZGZSK> list = new System.Collections.Generic.List<ZGZSK>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ZGZSK)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
