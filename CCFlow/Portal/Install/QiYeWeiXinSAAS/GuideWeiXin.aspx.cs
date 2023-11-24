using BP.Cloud.WeXinAPI;
using BP.DA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Sys;
using BP.WF.WeiXin;
using BP.DA;
using BP.Web;
using System.Runtime.InteropServices.WindowsRuntime;
using BP.En30.Utility.Web;
using System.Data;
using BP.Cloud.Template;

namespace CCFlow.App.Portal
{

    public partial class GuideWeiXin : System.Web.UI.Page
    {
        public void DoYiChang()
        {
            // SystemConfig.AppCenterDBCapitalAndSmallLetterModel
            Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);
            string empNo = this.Request.QueryString["EmpNo"];
            BP.WF.GenerWorkFlow gwf = new BP.WF.GenerWorkFlow(workid);

            BP.WF.GenerWorkerList gwl = new BP.WF.GenerWorkerList();
            var i = gwl.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Emp, empNo,
                GenerWorkerListAttr.IsPass, 0);
            if (i == 0)
            {
                this.Response.Write("<h3>工作已经处理或者已经失效.</h3>");
                return;
            }

            BP.Cloud.Emp emp = new BP.Cloud.Emp(gwf.OrgNo + "_" + gwl.FK_Emp);

            //执行登录.
            BP.Cloud.Dev2Interface.Port_Login(emp, false);

            string url = "";
            if (gwf.WFState == WFState.Complete)
            {
                if (this.IsMobile == false)
                    url = "/WF/MyView.htm?WorkID=" + workid + "&FK_Flow=" + gwf.FK_Flow + "&FK_Node=" + gwf.FK_Node;
                else
                    url = "/CCMobile/MyView.htm?WorkID=" + workid + "&FK_Flow=" + gwf.FK_Flow + "&FK_Node=" + gwf.FK_Node;
                this.Response.Redirect(url, true);
                return;
            }
            if (this.IsMobile == false)
                url = "/WF/MyFlow.htm?WorkID=" + workid + "&FK_Flow=" + this.Request.QueryString["FK_Flow"];
            else
                url = "/CCMobile/MyFlow.htm?WorkID=" + workid + "&FK_Flow=" + this.Request.QueryString["FK_Flow"];

            this.Response.Redirect(url, true);
            return;
        }

        //httppost请求
        BP.WF.HttpWebResponseUtility httpWebResponseUtility = new BP.WF.HttpWebResponseUtility();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                /*if (SystemConfig.CustomerNo == "ALLMED" && DataType.IsNullOrEmpty(this.Request.QueryString["WorkID"])==false)
                {
                    DoYiChang();
                    return;
                }*/

                #region 构造企业oauth2链接,如果企业需要在打开的网页里面携带用户的身份信息，第一步需要构造如下的链接来获取code参数
                // https://open.weixin.qq.com/connect/oauth2/authorize?appid=APPID&redirect_uri=REDIRECT_URI&response_type=code&scope=SCOPE&state=STATE#wechat_redirect
                //员工点击后，页面将跳转至 redirect_uri?code=CODE&state=STATE，企业可根据code参数获得员工的userid。code长度最大为512字节。

                string redirect_uri = BP.Cloud.WeXinAPI.Glo.Domain + "App/Portal/DirectWeiXin.aspx";
                string doType = this.Request.QueryString["DoType"];

                //邮箱打开待办：2.没有细化处理，邮件的url也是微信的
                if (DataType.IsNullOrEmpty(doType) == false && doType == "OpenEmpWorkByEmail")
                {
                    OpenEmpWorkByEmail(this.Request.QueryString["WorkID"], this.Request.QueryString["GUID"], this.Request.QueryString["ToEmpUserNo"]);
                    return;
                }
                //2.
                if (DataType.IsNullOrEmpty(doType) == false)
                {
                    redirect_uri += "?DoType=" + doType;
                    redirect_uri += "&WorkID=" + this.Request.QueryString["WorkID"];
                    redirect_uri += "&FK_Flow=" + this.Request.QueryString["FK_Flow"];
                    redirect_uri += "&GUID=" + this.Request.QueryString["GUID"];

                    /* if (BP.Sys.SystemConfig.CustomerNo.Equals("ALLMED") == true)
                     {
                         Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);

                         string empNo = this.Request.QueryString["EmpNo"];
                         BP.WF.GenerWorkFlow gwf = new BP.WF.GenerWorkFlow(workid);

                         BP.WF.GenerWorkerList gwl = new BP.WF.GenerWorkerList();
                         var i = gwl.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Emp, empNo,
                             GenerWorkerListAttr.IsPass, 0);

                         if (i == 0)
                         {
                             this.Response.Write("<h3>工作已经处理或者已经失效.</h3>");
                             return;
                         }

                         BP.Cloud.Emp emp = new BP.Cloud.Emp(gwf.OrgNo + "_" + gwl.FK_Emp);

                         //执行登录.
                         BP.Cloud.Dev2Interface.Port_Login(emp, false);
                         string url = "/WF/MyFlow.htm?WorkID=" + workid + "&FK_Flow=" + this.Request.QueryString["FK_Flow"];
                         this.Response.Redirect(url, true);
                         return;
                     }*/
                }

                //处理字符串.
                redirect_uri = HttpUtility.UrlEncode(redirect_uri);

                string oatuth2 = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + BP.Cloud.WeXinAPI.Glo.SuiteID + "&redirect_uri=" + redirect_uri + "&response_type=code&scope=snsapi_base&state=zA#wechat_redirect";
                BP.DA.Log.DebugWriteInfo("oauth2:" + oatuth2);

                
                this.Response.Redirect(oatuth2, true);
                /* httpWebResponseUtility.HttpResponseGet(oatuth2);
                 BP.DA.DataType.ReadURLContext(oatuth2);*/
                #endregion
            }
        }

        private void OpenEmpWorkByEmail(string workId, string sid, string empNo)
        {
            try
            {
                string dbStr = SystemConfig.AppCenterDBVarStr;
                string sql = "select SID from port_emp where No=" + dbStr + "No";

                Paras ps = new Paras();
                ps.SQL = sql;
                ps.Add("No", empNo);

                string token = DBAccess.RunSQLReturnString(ps);
                if (string.IsNullOrEmpty(token))
                {
                    this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "pushInfo", "pushInfo('用户验证失败')", true);

                    return;
                }
                //验证token是有有效
                var empRes = BP.Cloud.Dev2Interface.CheckTokenCode(token);
                if (empRes.code != ResponseCode.success)
                {
                    this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "pushInfo", "pushInfo('用户验证失败')", true);

                    return;
                }

                sql = "select FK_Flow,PWorkID,FK_Node,WorkID,FID,AtPara from WF_EmpWorks where WorkID=" + dbStr + "WorkID";
                ps = new Paras();
                ps.SQL = sql;
                ps.Add("WorkID", long.Parse(workId));

                DataTable dt = DBAccess.RunSQLReturnTable(ps);

                var paras = dt.Rows[0]["AtPara"].ToString();
                string flowNo = dt.Rows[0]["FK_Flow"].ToString();
                string pWorkID = dt.Rows[0]["PWorkID"].ToString();
                string nodeID = dt.Rows[0]["FK_Node"].ToString();
                string fid = dt.Rows[0]["FID"].ToString();
                string workID = dt.Rows[0]["WorkID"].ToString();
                string timeKey = DateTime.Now.ToString("yyyyMMddHHmmsss");

                string url = "../../WF/MyFlow.htm?FK_Flow=" + flowNo + "&PWorkID=" + pWorkID + "&FK_Node=" + nodeID + "&FID=" + fid + "&WorkID=" + workID + "&IsRead=0&T=" + timeKey + (string.IsNullOrWhiteSpace(paras) ? "" : "" + paras.Replace("'", "\\'").Replace('@', '&'));

                this.Response.Redirect(url);
            }
            catch (Exception ex)
            {
                //Log.DebugWriteError(ex.Message);
            }
        }
        public bool IsMobile
        {
            get
            {
                bool isMobile = IsMobileUrl(Request.UserAgent);
                if (isMobile)
                    return true;

                return false;
            }
        }

        public string[] mobileTag = { "iphone", "ios", "ipad", "android", "mobile" };
        /// <summary>
        /// 判断是否是手机打开
        /// </summary>
        /// <param name="userAgent">用户浏览器代理信息</param>
        /// <returns></returns>
        public bool IsMobileUrl(string userAgent)
        {
            bool result = false;
            userAgent = userAgent.ToLower();
            foreach (string sTmp in mobileTag)
            {
                if (userAgent.Contains(sTmp))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

    }
}