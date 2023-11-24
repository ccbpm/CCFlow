using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.GPM.DTalk;
using System.Web.Security.AntiXss;
using BP.WF.Port;

namespace CCFlow.CCMobile
{
    public partial class DingTalk : System.Web.UI.Page
    {
        public string DoType
        {
            get
            {
                return this.Request["DoType"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.DoType != null)
            {
                string responseVal = "";
                switch (DoType)
                {
                    case "loginmobfromdingtalk":
                        responseVal = getUserID();
                        break;
                    case "getuserdingcode":
                        responseVal = GetUserDingCode();
                        break;
                }
                ReturnVal(responseVal);
            }
        }
        /// <summary>
        /// 获取用户账号
        /// </summary>
        /// <returns></returns>
        public string getUserID()
        {
            string code = AntiXssEncoder.HtmlEncode(Request["Code"],true);
            string dingCode = BP.Difference .SystemConfig.Ding_CorpID;// Request["DingCode"];
            DingDing dingTalk = new DingDing();
            string userId = dingTalk.GetUserID(code);
            if (string.IsNullOrEmpty(userId) == true)
                return "{code:'error',Msg:'验证失败，没有从钉钉获取到用户账号。code:"+code+"'}";
            if (userId.Contains("超时") == true)
                return "{code:'error',Msg:'" + userId + "，请检查网络是否畅通。'}";

            BP.Port.Emp emp = new BP.Port.Emp();

            int row = emp.Retrieve(BP.Port.EmpAttr.Tel, userId);
            if (row == 0)
                return "{code:'error',Msg:'没有找到匹配用户+" + userId + "，登录失败！'}";

            if (!string.IsNullOrEmpty(dingCode))
            {
                //记录下来
                //WFEmp wfEmp = new WFEmp(emp.No);
                //wfEmp. = dingCode;
                //wfEmp.DirectUpdate();
            }
            BP.WF.Dev2Interface.Port_Login(emp.No);
            return "{code:'ok',Msg:'" + userId + "'}";
        }
        /// <summary>
        /// 获取用户钉钉code
        /// </summary>
        /// <returns></returns>
        private string GetUserDingCode()
        {
            //string userNo = Request["UserNo"];
            //WFEmp wfEmp = new WFEmp(userNo);
            //if (string.IsNullOrEmpty(wfEmp.TM))
            //    return "{code:'error',Msg:'没有应用操作权限或没有认证！'}";
            //return "{code:'ok',Msg:'" + wfEmp.TM + "'}";
            return "{code:'ok',Msg:'" + BP.Difference .SystemConfig.Ding_CorpID + "'}";
        }
        public void ReturnVal(string val)
        {
            if (string.IsNullOrEmpty(val))
                val = "";
            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(val);
            Response.End();
        }
    }
}