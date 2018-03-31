using System;
using System.Web;
using System.IO;
using System.Data;
using System.Web.UI.WebControls;
using BP.Web;
using BP.DA;
using BP.En;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BP.WF
{
    /// <summary>
    /// PortalPage 的摘要说明。
    /// </summary>
    public class CCFlowPage : BP.Web.PageBase
    {
        #region 属性
        public string RequestParas
        {
            get
            {
                string urlExt = "";
                string rawUrl = this.Request.RawUrl;
                rawUrl = "&" + rawUrl.Substring(rawUrl.IndexOf('?') + 1);
                string[] paras = rawUrl.Split('&');
                foreach (string para in paras)
                {
                    if (para == null
                        || para.Equals("") == true
                        || para.Contains("=") == false)
                        continue;
                    urlExt += "&" + para;
                }
                return urlExt;
            }
        }
        /// <summary>
        /// key.
        /// </summary>
        public string Key
        {
            get
            {
                return this.Request.QueryString["Key"];
            }
        }
        /// <summary>
        /// _HisEns
        /// </summary>
        public Entities _HisEns = null;
        /// <summary>
        /// 他的相关功能
        /// </summary>
        public Entities HisEns
        {
            get
            {
                if (this.EnsName != null)
                {
                    if (this._HisEns == null)
                        _HisEns = BP.En.ClassFactory.GetEns(this.EnsName);
                }
                return _HisEns;
            }
        }
        private Entity _HisEn = null;
        /// <summary>
        /// 他的相关功能
        /// </summary>
        public Entity HisEn
        {
            get
            {
                if (_HisEn == null)
                    _HisEn = this.HisEns.GetNewEntity;
                return _HisEn;
            }
        }
        #endregion

        #region 属性.
        public string PageID
        {
            get
            {
                return this.CurrPage;
            }
        }
        public string CurrPage
        {
            get
            {
                string url = BP.Sys.Glo.Request.RawUrl;
                int i = url.LastIndexOf("/") + 1;
                int i2 = url.IndexOf(".aspx") - 6;
                try
                {
                    url = url.Substring(i);
                    url = url.Substring(0, url.IndexOf(".aspx"));
                    return url;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + url + " i=" + i + " i2=" + i2);
                }
            }
        }
        public string DoType
        {
            get
            {
                string str = this.Request.QueryString["DoType"];
                if (string.IsNullOrEmpty(str))
                    str = null;
                return str;
            }
        }
        public string EnsName
        {
            get
            {
                string s = this.Request.QueryString["EnsName"];
                if (s == null)
                    s = this.Request.QueryString["EnsName"];

                return s;
            }
        }
        public string EnName
        {
            get
            {
                string s = this.Request.QueryString["EnName"];
                if (s == null)
                    s = this.Request.QueryString["EnName"];
                return s;
            }
        }
        public string RefPK
        {
            get
            {
                string s = this.Request.QueryString["RefPK"];
                if (string.IsNullOrEmpty(s))
                    s = this.Request.QueryString["PK"];

                return s;
            }
        }
        /// <summary>
        /// 页面Index.
        /// </summary>
        public int PageIdx
        {
            get
            {
                string str = this.Request.QueryString["PageIdx"];
                if (string.IsNullOrEmpty(str))
                    return 1;
                return int.Parse(str);
            }
            set
            {
                ViewState["PageIdx"] = value;
            }
        }
        public string RefNo
        {
            get
            {
                string s = this.Request.QueryString["RefNo"];
                if (DataType.IsNullOrEmpty(s) )
                    s = this.Request.QueryString["No"];

                if (s == null || s.Equals(""))
                    s = null;
                return s;
            }
        }
        /// <summary>
        /// 当前页面的参数．
        /// </summary> 
        public string Paras
        {
            get
            {
                string str = "";
                foreach (string s in this.Request.QueryString)
                {
                    str += "&" + s + "=" + this.Request.QueryString[s];
                }
                return str;
            }
        }
        #endregion 属性.

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}

