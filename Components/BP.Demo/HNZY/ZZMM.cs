using System;
using System.Collections.Generic;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Demo.HNZY
{
    public class ZZMMAttr: EntityNoNameAttr
    {
        public static string Idx = "Idx";
    }
    public class ZZMM : EntityNoName
    {
        /// <summary>
        /// 显示序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(ZZMMAttr.Idx);
            }
        }

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
        /// 政治面貌
        /// </summary>		
        public ZZMM() { }
        public ZZMM(string no)
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
                Map map = new Map("Demo_ZZMM", "政治面貌");

                #region 基本属性
                map.CodeStruct = "4";
                #endregion

                #region 字段
                map.AddTBStringPK(ZZMMAttr.No, null, "编号", true, false, 0, 50, 50);
                map.AddTBString(ZZMMAttr.Name, null, "名称", true, false, 0, 50, 200);
                map.AddTBInt(ZZMMAttr.Idx, 0, "显示序号", false, false);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 政治面貌s
    /// </summary>
    public class ZZMMs : EntitiesNoName
    {
        #region 
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ZZMM();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 政治面貌s
        /// </summary>
        public ZZMMs() { }

        /// <summary>
        /// 政治面貌s
        /// </summary>
        /// <param name="no"></param>
        public ZZMMs(string no)
        {
            this.Retrieve(ZZMMAttr.No, no);
        }
        #endregion
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
        public System.Collections.Generic.IList<ZZMM> ToJavaList()
        {
            return (System.Collections.Generic.IList<ZZMM>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ZZMM> Tolist()
        {
            System.Collections.Generic.List<ZZMM> list = new System.Collections.Generic.List<ZZMM>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ZZMM)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}

