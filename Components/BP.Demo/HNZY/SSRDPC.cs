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
    /// 双师认定批次设置
    /// </summary>
    public class SSRDPCAttr : EntityNoNameAttr
    {
        public static string Idx = "Idx";

        #region 双师认定批次信息字段
        /// <summary>
        /// 教师申请始日
        /// </summary>
        public static string JSSQKSR = "JSSQKSR";
        /// <summary>
        /// 教师申请止日
        /// </summary>
        public static string JSSQJZR = "JSSQJZR";
        /// <summary>
        /// 学校初审始日
        /// </summary>
        public static string XXCSKSR = "XXCSKSR";
        /// <summary>
        /// 市县审核止日
        /// </summary>
        public static string SXSHJZR = "SXSHJZR";

        #endregion 双师认定批次信息字段
    }
    public class SSRDPC:EntityNoName
    {
        /// <summary>
        /// 显示序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(SSRDPCAttr.Idx);
            }
        }

        #region 属性
        /// <summary>
        /// 教师申请始日
        /// </summary>
        public string JSSQKSR
        {
            get
            {
                return this.GetValStrByKey(SSRDPCAttr.JSSQKSR);
            }
        }
        /// <summary>
        /// 教师申请止日
        /// </summary>
        public string JSSQJZR
        {
            get
            {
                return this.GetValStrByKey(SSRDPCAttr.JSSQJZR);
            }
        }
        /// <summary>
        /// 学校初审始日
        /// </summary>
        public string XXCSKSR
        {
            get
            {
                return this.GetValStrByKey(SSRDPCAttr.XXCSKSR);
            }
        }
        /// <summary>
        /// 市县审核止日
        /// </summary>
        public string SXSHJZR
        {
            get
            {
                return this.GetValStrByKey(SSRDPCAttr.SXSHJZR);
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
        public SSRDPC() { }
        public SSRDPC(string no)
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
                Map map = new Map("Demo_SSRDPC", "双师认定批次设置");

                #region 基本属性
                map.CodeStruct = "4";
                #endregion

                #region 字段
                map.AddTBStringPK(SSRDPCAttr.No, null, "批次编号", true, false, 0, 50, 50);
                map.AddTBString(SSRDPCAttr.Name, null, "批次名称", true, false, 0, 50, 200);
                map.AddTBInt(SSRDPCAttr.Idx, 0, "显示序号", false, false);
                map.AddTBDate(SSRDPCAttr.JSSQKSR, "教师申请始日", true, false);
                map.AddTBDate(SSRDPCAttr.JSSQJZR, "教师申请止日", true, false);
                map.AddTBDate(SSRDPCAttr.XXCSKSR, "学校初审始日", true, false);
                map.AddTBDate(SSRDPCAttr.SXSHJZR, "市县审核止日", true, false);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    public class SSRDPCs : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SSRDPC();
            }
        }
        /// <summary>
        /// 双师认定s
        /// </summary>
        public SSRDPCs()
        {
        }
        /// <summary>
        /// 双师认定s
        /// </summary>
        public SSRDPCs(string no)
        {

            this.Retrieve(SSRDPCAttr.No, no);

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
        public System.Collections.Generic.IList<SSRDPC> ToJavaList()
        {
            return (System.Collections.Generic.IList<SSRDPC>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SSRDPC> Tolist()
        {
            System.Collections.Generic.List<SSRDPC> list = new System.Collections.Generic.List<SSRDPC>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SSRDPC)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
