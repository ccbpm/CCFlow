using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;
using BP.Port;
using BP.En;
using BP.Sys;

namespace CCFlow.WF.CCForm
{
    public partial class Port : System.Web.UI.Page
    {
        #region 必须传递参数
        /// <summary>
        /// 执行的内容
        /// </summary>
        public string DoWhat
        {
            get
            {
                string str= this.Request.QueryString["DoWhat"];
                if (string.IsNullOrEmpty(str))
                    return "Frm";
                return str;
            }
        }
        /// <summary>
        /// 当前的用户
        /// </summary>
        public string UserNo
        {
            get
            {
                return this.Request.QueryString["UserNo"];
            }
        }
        /// <summary>
        /// 用户的安全校验码(请参考集成章节)
        /// </summary>
        public string SID
        {
            get
            {
                return this.Request.QueryString["SID"];
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                string str= this.Request.QueryString["FrmID"];
                if (string.IsNullOrEmpty(str))
                    str = this.Request.QueryString["FK_MapData"];
                return str;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public Int64 OID
        {
            get
            {
                string str = this.Request.QueryString["OID"];
                if (string.IsNullOrEmpty(str))
                    str = this.Request.QueryString["WorkID"];

                if (string.IsNullOrEmpty(str) == false)
                    str = "0";
                return Int64.Parse(str);
            }
        }
        public string AppPath
        {
            get
            {
                return BP.WF.Glo.CCFlowAppPath;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

            #region 安全性校验.
            if (this.UserNo == null || this.SID == null || this.DoWhat == null || this.FrmID == null)
            {
                this.ToErrorPage("@必要的参数没有传入，请参考接口规则。");
                return;
            }

            if (BP.WF.Dev2Interface.Port_CheckUserLogin(this.UserNo, this.SID) == false)
            {
                this.ToErrorPage("@非法的访问，请与管理员联系。SID=" + this.SID);
                return;
            }

            if (BP.Web.WebUser.No != this.UserNo)
            {
                BP.WF.Dev2Interface.Port_SigOut();
                BP.WF.Dev2Interface.Port_Login(this.UserNo, true);
            }
            if (this.Request.QueryString["IsMobile"] == "1")
                BP.Web.WebUser.UserWorkDev = UserWorkDev.Mobile;
            else
                BP.Web.WebUser.UserWorkDev = UserWorkDev.PC;
            #endregion 安全性校验.

            #region 生成参数串.
            string paras = "";
            foreach (string str in this.Request.QueryString)
            {
                string val = this.Request.QueryString[str];
                if (val.IndexOf('@') != -1)
                    throw new Exception("您没有能参数: [ " + str + " ," + val + " ] 给值 ，URL 将不能被执行。");
                switch (str.ToLower())
                {
                    case "fk_mapdata":
                    case "workid":
                    case "fk_node":
                    case "sid":
                        break;
                    default:
                        paras += "&" + str + "=" + val;
                        break;
                }
            }
            #endregion 生成参数串.

            string url = "";
            switch (this.DoWhat)
            {
                case "Frm": //如果是调用Frm的查看界面.
                    url = "Frm.aspx?FK_MapData=" + this.FrmID + "&OID=" + this.OID + paras;
                    break;
                case "Search": //调用查询界面.
                    url = "../Comm/Search.aspx?EnsName=" + this.FrmID +  paras;
                    break;
                case "Group": //分组分析界面.
                    url = "../Comm/Group.aspx?EnsName=" + this.FrmID + paras;
                    break;
                default:
                    break;
            }
            this.Response.Redirect(url, true);
        }
        /// <summary>
        /// 切换到信息也面。
        /// </summary>
        /// <param name="mess"></param>
        public void ToErrorPage(string mess)
        {
            System.Web.HttpContext.Current.Session["info"] = mess;
            System.Web.HttpContext.Current.Response.Redirect(this.AppPath + "WF/Comm/Port/InfoPage.aspx");
            return;
        }
    }
}