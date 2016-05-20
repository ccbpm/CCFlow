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
    public partial class PriForm : System.Web.UI.Page
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
        private string userNo
        {
            get
            {
                return BP.WF.Cloud.CCLover.UserNo;
            }
        }
        private string pwd
        {
            get
            {
                return BP.WF.Cloud.CCLover.Password;
            }
        }
        private string guid
        {
            get
            {
                return BP.WF.Cloud.CCLover.GUID;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ccflowCloud.GetNetState();
            }
            catch (Exception)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>netInterruptJs();</script>");
                return;
            }

            if (!IsRegUser())
            {

                this.Response.Redirect("RegUser.aspx");
                return;
            }

            string method = string.Empty;

            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "getPriFormDir"://我的私有云目录
                    s_responsetext = GetPriFormDir();
                    break;
                case "getRecentlyPriFormTemp"://最近上传私有流程
                    s_responsetext = GetRecentlyPriFormTemp();
                    break;
                case "getSearchPriFormData"://查询
                    s_responsetext = GetSearchPriFormData();
                    break;
                case "getPriFormTempByDir":
                    s_responsetext = GetPriFormTempByDir();
                    break;
                case "addPriFormDir"://添加流程类别
                    s_responsetext = AddPriFormDir();
                    break;
                case "deletePriFormDir"://删除操作
                    s_responsetext = DeletePriFormDir();
                    break;
                case "editPriFormDir":
                    s_responsetext = EditPriFormDir();
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
        /// 判断注册信息
        /// </summary>
        /// <returns></returns>
        private bool IsRegUser()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userNo) &&
                    !string.IsNullOrWhiteSpace(pwd) &&
                    !string.IsNullOrWhiteSpace(guid))
                    return true;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private string GetPriFormDir()
        {
            if (!IsRegUser())
                return "";

            DataTable dt = ccflowCloud.PriFormDir(userNo, pwd, guid);

            return BP.DA.DataTableConvertJson.TransDataTable2TreeJson(dt, "MyPK", "NAME", "ParentNo", "0");
        }

        private string GetRecentlyPriFormTemp()
        {
            if (!IsRegUser())
                return "";

            DataTable dt = ccflowCloud.GetRecentlyPriFormTemp(userNo, pwd, guid);

            return BP.DA.DataTableConvertJson.DataTable2Json(dt);
        }

        private string GetSearchPriFormData()
        {
            if (!IsRegUser())
                return "";

            string keyWords = getUTF8ToString("keyWords");

            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);

            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            int totalCount = ccflowCloud.GetFormTotalCount(false, keyWords, "", userNo);

            DataTable dt = ccflowCloud.GetFormPagingData(false, keyWords, iPageNumber, iPageSize, "", userNo);

            return BP.DA.DataTableConvertJson.DataTable2Json(dt, totalCount);
        }

        private string GetPriFormTempByDir()
        {
            if (!IsRegUser())
                return "";

            string dir = getUTF8ToString("dir");
            string keyWords = getUTF8ToString("keyWords");

            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);

            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            int totalCount = ccflowCloud.GetFormTotalCount(false, keyWords, dir, userNo);

            DataTable dt = ccflowCloud.GetFormPagingData(false, keyWords, iPageNumber, iPageSize, dir, userNo);

            return BP.DA.DataTableConvertJson.DataTable2Json(dt, totalCount);
        }
        private string AddPriFormDir()
        {
            if (!IsRegUser())
                return "";

            string parentNo = this.getUTF8ToString("parentNo");
            string dirName = this.getUTF8ToString("nodeName");

            if (string.IsNullOrWhiteSpace(parentNo) || string.IsNullOrWhiteSpace(dirName))
                return "false";

            return ccflowCloud.AddPriFormDir(userNo, pwd, guid, parentNo, dirName).ToString().ToLower();
        }

        private string DeletePriFormDir()
        {
            if (!IsRegUser())
                return "";

            string no = this.getUTF8ToString("no");
            if (string.IsNullOrWhiteSpace(no))
                return "false";

            return ccflowCloud.DeletePriFormDir(userNo, pwd, guid, no).ToString().ToLower();
        }

        private string EditPriFormDir()
        {
            if (!IsRegUser())
                return "";

            string no = this.getUTF8ToString("no");
            string dirName = this.getUTF8ToString("nodeName");
            if (string.IsNullOrWhiteSpace(no) || string.IsNullOrWhiteSpace(dirName))
                return "false";

            return ccflowCloud.EditPriFormDir(userNo, pwd, guid, no, dirName).ToString().ToLower();
        }
    }
}