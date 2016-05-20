using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.CloudWS;
using System.Data;

namespace CCFlow.WF.Admin.Clound
{
    public partial class PubFlow : System.Web.UI.Page
    {
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        private WSSoapClient ccflowCloud
        {
            get
            {
                WSSoapClient cloud = BP.WF.Cloud.Glo.GetSoap();
                return cloud;
            }
        }
        /// <summary>
        /// 要到导入的流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.FK_Flow != null)
            {
                BP.WF.Glo.CurrFlow = this.FK_Flow;
            }

            try
            {
                ccflowCloud.GetNetState();
            }
            catch (Exception)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>netInterruptJs();</script>");
                return;
            }

            string method = string.Empty;

            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "getFlowTreeJSON"://共有云流程类别树形json
                    s_responsetext = GetFlowTreeJSON();
                    break;
                case "getRecentlyFlowTemp"://最近上传流程
                    s_responsetext = GetRecentlyFlowTemp();
                    break;
                case "getSearchData"://查询
                    s_responsetext = GetSearchData();
                    break;
                case "getFlowTempByDir":
                    s_responsetext = GetFlowTempByDir();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";

            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }
        /// <summary>
        /// 共有云流程类别树形json
        /// </summary>
        /// <returns></returns>
        private string GetFlowTreeJSON()
        {
            DataTable dt = ccflowCloud.GetPubFlowTree();

            return BP.DA.DataTableConvertJson.TransDataTable2TreeJson(dt, "NO", "NAME", "ParentNo", "0");
        }
        /// <summary>
        /// 最近上传流程
        /// </summary>
        /// <returns></returns>
        private string GetRecentlyFlowTemp()
        {
            DataTable dt = ccflowCloud.GetRecentlyFlowTemp();

            return BP.DA.DataTableConvertJson.DataTable2Json(dt);
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        private string GetSearchData()
        {

            DataTable dt = ccflowCloud.GetRecentlyFlowTemp();

            string keyWords = getUTF8ToString("keyWords");

            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);

            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);



            int totalCount = ccflowCloud.GetTotalCount(true, keyWords, "","");

            dt = ccflowCloud.GetPagingData(true, keyWords, iPageNumber, iPageSize, "","");

            return BP.DA.DataTableConvertJson.DataTable2Json(dt, totalCount);
        }

        private string GetFlowTempByDir()
        {
            string dir = getUTF8ToString("dir");
            string keyWords = getUTF8ToString("keyWords");

            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);

            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);



            int totalCount = ccflowCloud.GetTotalCount(true, keyWords, dir,"");

            DataTable dt = ccflowCloud.GetPagingData(true, keyWords, iPageNumber, iPageSize, dir,"");

            return BP.DA.DataTableConvertJson.DataTable2Json(dt, totalCount);
        }
    }
}