using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using BP.WF;
using BP.Port.WeiXin;
using BP.En;
using BP.DA;
using System.Text;
using BP.Tools;
using BP.Sys;
using System.Data;
using BP.Port.WeiXin.Msg;

namespace CCFlow.CCMobile
{
    public partial class actionWX : System.Web.UI.Page
    {
        #region 参数。
        public string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public string UserNo
        {
            get
            {
                return this.Request.QueryString["UserNo"];
            }
        }
        public string Password
        {
            get
            {
                return this.Request.QueryString["Password"];
            }
        }
        #endregion 参数。

        /// <summary>
        /// 执行方法.
        /// </summary>
        public void DoTypeAction()
        {
            switch (this.DoType)
            {
                case "Login"://登录
                    BP.Port.Emp emp = new BP.Port.Emp();
                    emp.No = this.UserNo;
                    if (emp.IsExits == false)
                    {
                        ReturnVal("0");
                        return;
                    }
                    if (emp.Pass == this.Password)
                    {
                        BP.WF.Dev2Interface.Port_Login(this.UserNo);
                        ReturnVal("1");
                        return;
                    }
                    ReturnVal("0");
                    return;
                default:
                    break;
            }
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
        protected void Page_Load(object sender, EventArgs e)
        {
            //第一步：获取企业微信跳转页面中的code，每次登录，code都不一致
            string code = Request["code"];
            string state = Request["state"]; //重定向执行标记.

            //用户唯一标识
            string openid = "";
            //网页授权接口调用凭证
            string access_token = "";
            //用户刷新access_token 
            string refresh_token = "";

            //第二步：通过code换取网页授权access_token
            WeiXinGZHModel.AccessToken accessToken = new WeiXinGZHModel.AccessToken();
            accessToken = WeiXinGZHEntity.getAccessToken(code);
            if (accessToken.errcode == "0" || DataType.IsNullOrEmpty(accessToken.errcode))
            {
                openid = accessToken.openid;
                access_token = accessToken.access_token;
                refresh_token = accessToken.refresh_token;
            }
            else
            {
                this.Response.Write("@获取网页授权失败，errcode：" + accessToken.errcode);
                return;
            }

            //第三步：获取用户信息
            WeiXinGZHModel.GZHUser user = new WeiXinGZHModel.GZHUser();
            user = WeiXinGZHEntity.getUserInfo(access_token, openid);
            if (user.errcode != "0" && !DataType.IsNullOrEmpty(user.errcode))
            {
                this.Response.Write("@获取用户信息失败，errcode：" + user.errcode);
                return;
            }

            //第四步：验证是否已注册本系统
            BP.Port.WeiXin.Emp emp = new BP.Port.WeiXin.Emp();
            if (emp.IsExit("OpenID", user.openid) == false)
            {
                emp.No = user.openid;
                emp.Name = user.nickname;
                emp.OpenID = user.openid;
                string sql = "SELECT No  FROM Port_Dept WHERE NAME LIKE '%外部用户%'  ";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count != 1)
                {
                    this.Response.Write("@获取外部用户部门失败：，errcode：" + sql);
                    return;
                }
                emp.DeptNo = dt.Rows[0][0].ToString();
                emp.Insert();
            }
            BP.WF.Dev2Interface.Port_Login(emp.No);
            Response.Redirect("../CCMobilePortal/Home.htm?UserNo=" + emp.No + "&openID=" + emp.OpenID + "&FK_Dept=" + emp.DeptNo, true);
            return;
        }
        /// <summary>
        /// 获得用户ID
        /// </summary>
        /// <param name="code"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public string getUserId(string code, string accessToken)
        {
            //获取用户信息
            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token=" + accessToken + "&code=" + code + "&agentid=2";
            return DataType.ReadURLContext(url, 39000, Encoding.UTF8);
        }
    }

}
