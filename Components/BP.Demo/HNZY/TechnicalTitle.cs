using System;
using System.Collections.Generic;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Demo.HNZY
{
    public class TechnicalTitleAttr : EntityNoNameAttr
    {
        public static string TitleType = "TitleType";
    }
    public class TechnicalTitle : EntityNoName
    {
        public string TitleType
        {
            get 
            {
                return this.GetValStringByKey(TechnicalTitleAttr.TitleType);
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
        /// 职称
        /// </summary>		
        public TechnicalTitle() { }
        public TechnicalTitle(string no)
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
                Map map = new Map("Demo_TechnicalTitle", "职称");

                #region 基本属性
                map.CodeStruct = "4";
                #endregion

                #region 字段
                map.AddTBStringPK(TechnicalTitleAttr.No, null, "编号", true, false, 0, 50, 50);
                map.AddTBString(TechnicalTitleAttr.Name, null, "名称", true, false, 0, 50, 200);
                map.AddTBString(TechnicalTitleAttr.TitleType, null, "职称系列", true, false, 0, 50, 200);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 职称ss
    /// </summary>
    public class TechnicalTitles : EntitiesNoName
    {
        #region 
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TechnicalTitle();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 职称s
        /// </summary>
        public TechnicalTitles() { }

        /// <summary>
        /// 职称s
        /// </summary>
        /// <param name="no"></param>
        public TechnicalTitles(string no)
        {
            this.Retrieve(TechnicalTitleAttr.No, no);
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
        public System.Collections.Generic.IList<TechnicalTitle> ToJavaList()
        {
            return (System.Collections.Generic.IList<TechnicalTitle>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TechnicalTitle> Tolist()
        {
            System.Collections.Generic.List<TechnicalTitle> list = new System.Collections.Generic.List<TechnicalTitle>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TechnicalTitle)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}
