using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;


namespace BP.En
{
    /// <summary>
    /// 查询属性
    /// </summary>
    public class SearchFKEnum
    {
        /// <summary>
        /// 查询属性
        /// </summary>
        public Attr HisAttr = null;
        /// <summary>
        /// 是否显示全部
        /// </summary>
        public bool IsShowAll = true;
        /// <summary>
        /// 及联子菜单
        /// </summary>
        public string RelationalDtlKey = null;
        /// <summary>
        /// 查询的字段
        /// </summary>
        public string Key = null;
        /// <summary>
        /// 下拉框显示的宽度
        /// </summary>
        public int Width = 120;
        public SearchFKEnum()
        {
        }
    }
    /// <summary>
    /// 查询属性s
    /// </summary>
    public class SearchFKEnums : CollectionBase
    {
        public SearchFKEnums()
        {
        }
        public void Add(Attr attr, bool isShowSelectedAll, string relationalDtlKey, int width=120)
        {
            SearchFKEnum en = new SearchFKEnum();
            en.HisAttr = attr;
            en.IsShowAll = isShowSelectedAll;
            en.RelationalDtlKey = relationalDtlKey;
            en.Key = attr.Key;
            en.Width = width; //宽度.
            this.InnerList.Add(en);
        }
        public void Add(SearchFKEnum attr)
        {
            this.InnerList.Add(attr);
        }
    }
}
