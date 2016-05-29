using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.Web;
using BP.En;
using BP.DA;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP;

namespace CCFlow.WF
{
    public partial class Face_EmpWorks : BP.Web.WebPage
    {
        #region 属性.
        //public string FK_Flow
        //{
        //    get
        //    {
        //        string s = this.Request.QueryString["FK_Flow"];
        //        if (s == null)
        //            return this.ViewState["FK_Flow"] as string;
        //        return s;
        //    }
        //    set
        //    {
        //        this.ViewState["FK_Flow"] = value;
        //    }
        //}
        //public bool IsHungUp
        //{
        //    get
        //    {
        //        string s = this.Request.QueryString["IsHungUp"];
        //        if (s == null)
        //            return false;
        //        else
        //            return true;
        //    }
        //}
        //public string GroupBy
        //{
        //    get
        //    {
        //        string s = this.Request.QueryString["GroupBy"];
        //        if (s == null)
        //        {
        //            if (this.DoType == "CC")
        //                s = "Rec";
        //            else
        //                s = "FlowName";
        //        }
        //        return s;
        //    }
        //}
        #endregion 属性.

        //public DataTable dt = null;
        //string timeKey;
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    }
}