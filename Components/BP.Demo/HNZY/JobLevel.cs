using System;
using System.Collections.Generic;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Demo.HNZY
{
    public class JobLevelAttr : EntityNoNameAttr
    {
    }
    public class JobLevel : EntityNoName
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
        /// 职务
        /// </summary>		
        public JobLevel() { }
        public JobLevel(string no)
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
                Map map = new Map("Demo_JobLeve", "职务");

                #region 基本属性
                map.CodeStruct = "4";
                #endregion

                #region 字段
                map.AddTBStringPK(JobLevelAttr.No, null, "编号", true, false, 0, 50, 50);
                map.AddTBString(JobLevelAttr.Name, null, "名称", true, false, 0, 50, 200);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 职务ss
    /// </summary>
    public class JobLevels : EntitiesNoName
    {
        #region 
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new JobLevel();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 职务s
        /// </summary>
        public JobLevels() { }

        /// <summary>
        /// 职务s
        /// </summary>
        /// <param name="no"></param>
        public JobLevels(string no)
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
        public System.Collections.Generic.IList<JobLevel> ToJavaList()
        {
            return (System.Collections.Generic.IList<JobLevel>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<JobLevel> Tolist()
        {
            System.Collections.Generic.List<JobLevel> list = new System.Collections.Generic.List<JobLevel>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((JobLevel)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}
