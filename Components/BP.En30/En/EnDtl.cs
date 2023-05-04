using System;
using System.Collections;
using BP.DA;

namespace BP.En
{
    /// <summary>
    /// 从表的编辑模式
    /// </summary>
    public enum DtlEditerModel
    {
        /// <summary>
        /// 批量编辑
        /// </summary>
        DtlBatch = 0,
        /// <summary>
        /// 查询编辑模式
        /// </summary>
        DtlSearch = 1,
        /// <summary>
        /// 自定义URL
        /// </summary>
        DtlURL = 2,
        /// <summary>
        /// 在EnOnly显示查询
        /// </summary>
        DtlBatchEnonly = 3,
        /// <summary>
        /// 在EnOnly
        /// </summary>
        DtlSearchEnonly = 4,

        DtlURLEnonly = 5,
    }
    /// <summary>
    /// EnDtl 的摘要说明。
    /// </summary>
    public class EnDtl
    {
        /// <summary>
        /// 明细
        /// </summary>
        public EnDtl()
        {
        }
        /// <summary>
        /// 编辑器模式 0=默认的DtlBatch.htm, 1=DtlSearch.htm 
        /// </summary>
        public DtlEditerModel DtlEditerModel = DtlEditerModel.DtlBatch; 
        /// <summary>
        /// 类名称
        /// </summary>
        public string EnsName
        {
            get
            {
                return this.Ens.ToString();
            }
        }
        /// <summary>
        /// 明细
        /// </summary>
        public Entities Ens = null;
        public string UrlExt = null;
        /// <summary>
        /// 他关连的 key
        /// </summary>
        public string RefKey = null;
        private string _desc = "";
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc
        {
            get
            {
                if(DataType.IsNullOrEmpty(_desc))
                    return this.Ens.GetNewEntity.EnDesc;
                return this._desc;
            }
            set
            {
                this._desc = value;
            }
        }
        /// <summary>
        /// 显示到分组
        /// </summary>
        public string GroupName = null;
        public string Icon = null;

    }
    /// <summary>
    /// 从表
    /// </summary>
    [Serializable]
    public class EnDtls : CollectionBase
    {
        /// <summary>
        /// 加入
        /// </summary>
        /// <param name="attr">attr</param>
        public void Add(EnDtl en)
        {
            if (this.IsExits(en))
                return;
            this.InnerList.Add(en);
        }
        /// <summary>
        /// 是不是存在集合里面
        /// </summary>
        /// <param name="en">要检查的EnDtl</param>
        /// <returns>true/false</returns>
        public bool IsExits(EnDtl en)
        {
            foreach (EnDtl dtl in this)
            {
                if (dtl.Ens == en.Ens)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 根据索引访问集合内的元素Attr。
        /// </summary>
        public EnDtl this[int index]
        {
            get
            {
                return (EnDtl)this.InnerList[index];
            }
        }
    }
}
