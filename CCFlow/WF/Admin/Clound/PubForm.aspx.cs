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
    public partial class PubForm : System.Web.UI.Page
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
        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;

            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "getFormTreeJSON"://表单树
                    s_responsetext = GetFormTreeJSON();
                    break;
                case "getRecentlyFormTemp"://最近上传的表单
                    s_responsetext = GetRecentlyFormTemp();
                    break;
                case "getSearchFormData"://查询
                    s_responsetext = GetSearchFormData();
                    break;
                case "getFormTempByDir":
                    s_responsetext = GetFormTempByDir();
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
        ///  获取共有表单目录
        /// </summary>
        /// <returns></returns>
        private string GetFormTreeJSON()
        {
            DataTable dt = ccflowCloud.GetPubFormTree();

            return BP.DA.DataTableConvertJson.TransDataTable2TreeJson(dt, "NO", "NAME", "ParentNo", "0");
        }
        /// <summary>
        /// 最近上传表单
        /// </summary>
        /// <returns></returns>
        private string GetRecentlyFormTemp()
        {
            DataTable dt = ccflowCloud.GetRecentlyFormTemp();

            return BP.DA.DataTableConvertJson.DataTable2Json(dt);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        private string GetSearchFormData()
        {

            DataTable dt = ccflowCloud.GetRecentlyFlowTemp();

            string keyWords = getUTF8ToString("keyWords");

            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);

            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);



            int totalCount = ccflowCloud.GetFormTotalCount(true, keyWords, "", "");

            dt = ccflowCloud.GetFormPagingData(true, keyWords, iPageNumber, iPageSize, "", "");

            return BP.DA.DataTableConvertJson.DataTable2Json(dt, totalCount);
        }

        private string GetFormTempByDir()
        {
            string dir = getUTF8ToString("dir");
            string keyWords = getUTF8ToString("keyWords");

            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);

            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);



            int totalCount = ccflowCloud.GetFormTotalCount(true, keyWords, dir, "");

            DataTable dt = ccflowCloud.GetFormPagingData(true, keyWords, iPageNumber, iPageSize, dir, "");

            return BP.DA.DataTableConvertJson.DataTable2Json(dt, totalCount);
        }
    }
}