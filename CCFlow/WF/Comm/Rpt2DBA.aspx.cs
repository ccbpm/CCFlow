using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using System.Linq;
using System.Web.UI.WebControls;
using BP.En;
using BP.DA;

namespace CCFlow.WF.Comm
{
    public partial class Rpt2DBA : BP.Web.WebPage
    {
        #region 属性.
        public string Rpt2Name
        {
            get
            {
                return this.Request.QueryString["Rpt2Name"];
            }
        }

        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdxStr = this.Request.QueryString["Idx"];
               // YBUser.ModuleVSta(Rpt2Name.Replace("BP.YueBing.", ""), IdxStr);
            }

          //  BP.OA.GPM.CheckUserCanLookThisPage(BP.Web.WebUser.No);

            Response.HeaderEncoding = System.Text.Encoding.UTF8;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
        }

        /// <summary>
        /// added by liuxc,2015-03-25,为将本页查询参数传递到明细表页面而增加的逻辑
        /// </summary>
        /// <param name="kvs">由拼接好的本页查询参数串</param>
        /// <returns></returns>
        public string CheckUrlParams(string kvs)
        {
            var kv = new Dictionary<string, string>();
            var querys = Request.Url.Query.TrimStart('?')
                .Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] qs = null;
            var exists = new[] { "rpt2name", "idx" };

            foreach (var q in querys)
            {
                qs = q.Split('=');

                if (qs.Length != 2 || exists.Contains(qs[0].ToLower()))
                    continue;

                kv.Add(qs[0], qs[1]);
            }

            foreach (var de in kv)
            {
                if (kvs.IndexOf("&" + de.Key + "=") == -1)
                    kvs += "&" + de.Key + "=" + de.Value;
            }

            return kvs;
        }
    }
}