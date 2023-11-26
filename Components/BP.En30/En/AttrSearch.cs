using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;


namespace BP.En
{
    /// <summary>
    /// 查询属性
    /// </summary>
    public class AttrSearch
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
        public string Key = null;
        /// <summary>
        /// 下拉框显示的宽度
        /// </summary>
        public int Width = 120;
        public AttrSearch()
        {
        }
    }
    /// <summary>
    /// 查询属性s
    /// </summary>
    public class AttrSearchs : CollectionBase
    {
        public AttrSearchs()
        {
        }
        public void Add(Attr attr, bool isShowSelectedAll, string relationalDtlKey, int width=120)
        {
            AttrSearch en = new AttrSearch();
            en.HisAttr = attr;
            en.IsShowAll = isShowSelectedAll;
            en.RelationalDtlKey = relationalDtlKey;
            en.Key = attr.Key;
            en.Width = width; //宽度.
            this.InnerList.Add(en);
        }

        public void Add(AttrSearch attr)
        {
            this.InnerList.Add(attr);
        }
    }
}
