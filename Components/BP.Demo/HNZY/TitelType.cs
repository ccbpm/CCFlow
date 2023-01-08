using System;
using System.Collections.Generic;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Demo.HNZY
{
    public class TitelTypeAttr : EntityNoNameAttr
    {
    }
    public class TitelType : EntityNoName
    {


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
        /// 职称类别
        /// </summary>		
        public TitelType() { }
        public TitelType(string no)
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
                Map map = new Map("Demo_TitelType", "职称类别");

                #region 基本属性
                map.CodeStruct = "4";
                #endregion

                #region 字段
                map.AddTBStringPK(TitelTypeAttr.No, null, "编号", true, false, 0, 50, 50);
                map.AddTBString(TitelTypeAttr.Name, null, "名称", true, false, 0, 50, 200);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 职称类型ss
    /// </summary>
    public class TitelTypes : EntitiesNoName
    {
        #region 
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TitelType();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 职称类型s
        /// </summary>
        public TitelTypes() { }

        /// <summary>
        /// 职称类型s
        /// </summary>
        /// <param name="no"></param>
        public TitelTypes(string no)
        {
            this.Retrieve(JobLevelAttr.No, no);
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
        public System.Collections.Generic.IList<TitelType> ToJavaList()
        {
            return (System.Collections.Generic.IList<TitelType>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TitelType> Tolist()
        {
            System.Collections.Generic.List<TitelType> list = new System.Collections.Generic.List<TitelType>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TitelType)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}

