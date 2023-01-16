using System;
using System.Collections;
using BP.En;
using BP.Web;
using BP.Difference;

namespace BP.En
{
    public enum OperatorSymbol
    {
        /// <summary>
        /// 大于
        /// </summary>
        DaYu,
        /// <summary>
        /// 等于
        /// </summary>
        DengYu,
        /// <summary>
        /// 小于
        /// </summary>
        XiaoYu,
        /// <summary>
        /// 相似
        /// </summary>
        Like,
    }
    /// <summary>
    /// 属性属性关联
    /// </summary>
    public class AARef
    {
        /// <summary>
        /// 目录属性
        /// </summary>
        public string CataAttr = null;
        /// <summary>
        /// 关联key
        /// </summary>
        public string RefKey = null;
        /// <summary>
        /// 子属性
        /// </summary>
        public string SubAttr = null;
        /// <summary>
        /// 属性属性关联
        /// </summary>
        /// <param name="CataAttr">属性</param>
        /// <param name="RefKey"></param>
        /// <param name="SubAttr"></param>
        public AARef(string cataAttr, string subAttr, string refKey)
        {
            this.CataAttr = cataAttr;
            this.SubAttr = subAttr;
            this.RefKey = refKey;

        }
    }
    public class AARefs : System.Collections.CollectionBase
    {
        #region 构造
        public AARefs()
        {
        }
        public AARefs this[int index]
        {
            get
            {
                return (AARefs)this.InnerList[index];
            }
        }
        #endregion

        #region 增加一个查询属性。
        /// <summary>
        /// 增加一个查询属性
        /// </summary>
        /// <param name="lab">标签</param>
        /// <param name="refKey">实体的属性</param>
        /// <param name="defaultvalue">默认值</param>
        public void Add(string lab, string key, string refKey, string defaultSymbol, string defaultvalue, int tbWidth)
        {
            SearchNormal aos = new SearchNormal(key, lab, refKey, defaultSymbol, defaultvalue, tbWidth, false);
            this.InnerList.Add(aos);
        }
        #endregion
    }

    /// <summary>
    /// SearchKey 的摘要说明。
    /// 用来处理一条记录的存放，问题。
    /// </summary>
    public class SearchNormal
    {
        #region 基本属性
        /// <summary>
        /// 是否隐藏
        /// </summary>
        private bool _IsHidden = false;
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsHidden
        {
            get
            {
                return _IsHidden;
            }
            set
            {
                _IsHidden = value;
            }
        }
        /// <summary>
        /// 操作是否可用
        /// </summary>
        private bool _SymbolEnable = true;
        /// <summary>
        /// 操作是否可用
        /// </summary>
        public bool SymbolEnable
        {
            get
            {
                return _SymbolEnable;
            }
            set
            {
                _SymbolEnable = value;
            }
        }

        /// <summary>
        /// 标签
        /// </summary>
        private string _Lab = "";
        /// <summary>
        /// 标签
        /// </summary>
        public string Lab
        {
            get
            {
                return _Lab;
            }
            set
            {
                _Lab = value;
            }
        }
        /// <summary>
        /// 查询默认值
        /// </summary>
        private string _DefaultVal = "";
        /// <summary>
        /// OperatorKey
        /// </summary>
        public string DefaultVal
        {
            get
            {
                return _DefaultVal;
            }
            set
            {
                _DefaultVal = value;
            }
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValRun
        {
            get
            {
                if (_DefaultVal == null)
                    return null;

                if (_DefaultVal.Contains("@"))
                {
                    if (_DefaultVal.Contains("@WebUser.No"))
                        return _DefaultVal.Replace("@WebUser.No", WebUser.No);

                    if (_DefaultVal.Contains("@WebUser.Name"))
                        return _DefaultVal.Replace("@WebUser.Name", WebUser.Name);

                    if (_DefaultVal.Contains("@WebUser.FK_Dept"))
                        return _DefaultVal.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

                    if (_DefaultVal.Contains("@WebUser.DeptParentNo"))
                        return _DefaultVal.Replace("@WebUser.DeptParentNo", WebUser.DeptParentNo);

                    if (_DefaultVal.Contains("@WebUser.FK_DeptName"))
                        return _DefaultVal.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);

                    if (_DefaultVal.Contains("@WebUser.OrgNo"))
                        return _DefaultVal.Replace("@WebUser.OrgNo", WebUser.OrgNo);

                    if (_DefaultVal.Contains("@WebUser.FK_DeptNameOfFull"))
                        return _DefaultVal.Replace("@WebUser.FK_DeptNameOfFull", WebUser.FK_DeptNameOfFull);

                    // 处理传递过来的参数。
                    //foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                    //2019-07-25 zyt改造
                    foreach (string k in HttpContextHelper.RequestParamKeys)
                    {
                        if (_DefaultVal.Contains(k))
                            return _DefaultVal.Replace("@" + k, HttpContextHelper.RequestParams(k));
                    }
                    //foreach (string k in System.Web.HttpContext.Current.Request.Form.AllKeys)
                    //{
                    //    if (_DefaultVal.Contains(k))
                    //        return _DefaultVal.Replace("@" + k, System.Web.HttpContext.Current.Request.Form[k]);
                    //}

                    //if (_DefaultVal.Contains("@WebUser.FK_Unit"))
                    //    return _DefaultVal.Replace("@WebUser.FK_Unit", WebUser.FK_Unit);

                }
                return _DefaultVal;
            }
        }
        /// <summary>
        /// 默认的操作符号.
        /// </summary>
        private string _defaultSymbol = "=";
        /// <summary>
        /// 操作符号
        /// </summary>
        public string DefaultSymbol
        {
            get
            {
                return _defaultSymbol;
            }
            set
            {
                _defaultSymbol = value;
            }
        }
        /// <summary>
        /// 对应的属性
        /// </summary>
        private string _RefAttr = "";
        /// <summary>
        /// 对应的属性
        /// </summary>
        public string RefAttrKey
        {
            get
            {
                return _RefAttr;
            }
            set
            {
                _RefAttr = value;
            }
        }
        /// <summary>
        /// Key
        /// </summary>
        private string _Key = "";
        /// <summary>
        /// Key
        /// </summary>
        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value;
            }
        }
        /// <summary>
        /// TB 宽度
        /// </summary>
        private int _TBWidth = 10;
        /// <summary>
        /// TBWidth 
        /// </summary>
        public int TBWidth
        {
            get
            {
                return _TBWidth;
            }
            set
            {
                _TBWidth = value;
            }
        }
        #endregion

        #region 构造方法
        public SearchNormal()
        {
        }
        /// <summary>
        /// 构造一个普通的查询属性
        /// </summary>
        public SearchNormal(string key, string lab, string refAttr, string DefaultSymbol, string defaultValue, int tbwidth, bool isHidden)
        {
            this.Key = key;
            this.Lab = lab;
            this.RefAttrKey = refAttr;
            this.DefaultSymbol = DefaultSymbol;
            this.DefaultVal = defaultValue;
            this.TBWidth = tbwidth;
            this.IsHidden = isHidden;
        }
        #endregion
    }
    /// <summary>
    /// SearchKey 集合
    /// </summary>
    public class SearchNormals : System.Collections.CollectionBase
    {
        #region 构造
        public SearchNormals()
        {
        }
        public SearchNormals this[int index]
        {
            get
            {
                return (SearchNormals)this.InnerList[index];
            }
        }
        #endregion

        #region 增加一个查询属性。

        /// <summary>
        /// 增加一个查询属性
        /// </summary>
        /// <param name="lab">标签</param>
        /// <param name="refKey">实体的属性</param>
        /// <param name="defaultvalue">默认值</param>
        public void Add(SearchNormal en)
        {
            this.InnerList.Add(en);
        }
        #endregion
    }
}
