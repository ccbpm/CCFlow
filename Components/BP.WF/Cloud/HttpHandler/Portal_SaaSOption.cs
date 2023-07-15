using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Difference;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using System.Collections;
using System.Security.Policy;
using Microsoft.SqlServer.Server;

namespace BP.Cloud.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class Portal_SaaSOption : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Portal_SaaSOption()
        {
        }
        /// <summary>
        /// 注册页面提交
        /// </summary>
        /// <returns></returns>
        public string RegisterAdminer_Submit()
        {
            string ip = BP.Difference.Glo.GetIP;

            string sql = "SELECT COUNT(*) FROM Port_Org WHERE RegIP='" + ip + "' AND DTReg LIKE '" + DataType.CurrentDate + "%'";
            int num = DBAccess.RunSQLReturnValInt(sql);
            if (num >= 4)
                return "err@系统错误，今日不能连续注册。";

            BP.Cloud.Org org = new Org();
            string orgNo = this.GetRequestVal("TB_OrgNo");//管理员名称拼音.
            string orgName = this.GetRequestVal("TB_OrgName");//管理员名称拼音.

            org.No = orgNo;
            if (org.IsExits == true)
                return "err@组织账号[" + orgNo + "]已经存在。";           

            string tel = this.GetRequestVal("TB_Adminer");//管理员名称拼音.
            Emp ep = new Emp();
            ep.No = orgNo;
            if (ep.IsExits == true)
                return "err@组织账号[" + orgNo + "]已经存在。";
            if (ep.IsExit(EmpAttr.UserID, orgNo) == true)
                return "err@组织账号，已经存在.";

            ep.No = tel;
            if (ep.IsExits == true)
                return "err@该手机号已经注册请登陆，或者使用其他手机号注册.";
            if (ep.IsExit("UserID", tel) == true)
                return "err@该手机号已经注册请登陆，或者使用其他手机号注册.";

            string adminer = this.GetRequestVal("TB_AdminerName"); //管理员名称中文.

            ep.UserID = tel;
            ep.Name = adminer;

            try
            {
                //admin登录.
                BP.WF.Dev2Interface.Port_Login("admin", "100");

                //首先创建组织.
                Dev2Interface.Port_CreateOrg(orgNo, orgName);

                org.No = orgNo;
                org.RetrieveFromDBSources();
                org.NameFull = this.GetRequestVal("TB_OrgNameFull");
                org.Adminer = tel;
                org.AdminerName = adminer;

                //避免其他的注册错误.
                BP.Web.WebUser.OrgNo = org.No;
                BP.Web.WebUser.OrgName = org.Name;

                org.RegFrom = 0; //0=网站.1=企业微信.
                org.Adminer = tel;
                org.AdminerName = ep.Name;
                org.DTReg = DataType.CurrentDateTime;

                //获取来源.
                string from = this.GetRequestVal("From");
                if (DataType.IsNullOrEmpty(from) == true)
                    from = "ccbpm";
                org.UrlFrom = from;
                org.Update();


                //循环遍历 看邮箱是否唯一用户忘记密码用邮箱找回.
                string email = this.GetRequestVal("TB_Email");
                ep.Email = email;
                ep.Name = adminer;
            

                //处理拼音
                string pinyinQP = BP.DA.DataType.ParseStringToPinyin(ep.Name).ToLower();
                string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(ep.Name).ToLower();
                ep.PinYin = "," + pinyinQP + "," + pinyinJX + ",";

                ep.OrgName = this.GetRequestVal("TB_OrgName");
                ep.FK_Dept = org.No;
                ep.OrgNo = org.No;
                ep.No = ep.OrgNo + "_" + tel;
                ep.DirectInsert();

                // 密码加密。
                string pass = this.GetRequestVal("TB_PassWord2");
                if (BP.Difference.SystemConfig.IsEnablePasswordEncryption == true)
                    pass = BP.Tools.Cryptography.EncryptString(pass);
                DBAccess.RunSQL("UPDATE Port_Emp SET Pass='" + pass + "' WHERE No='" + ep.No + "'");

                //初始化数据.
                org.Adminer = ep.UserID;
                org.AdminerName = ep.Name;
                org.Update();

                //增加管理员.
                BP.WF.Port.Admin2Group.Org myorg = new BP.WF.Port.Admin2Group.Org(org.No);
                myorg.AddAdminer(tel);

                //让 组织 管理员登录.
                BP.WF.Dev2Interface.Port_Login(ep.UserID, org.No);

                //生成token.
                string token = BP.WF.Dev2Interface.Port_GenerToken("PC");
                return token;

            }
            catch (Exception ex)
            {
                org.DoDelete();
                BP.WF.Dev2Interface.Port_SigOut();
                return "err@安装期间出现错误:" + ex.Message;
            }
            ////让其退出登录.
            //BP.Web.GuestUser.Exit();
            //BP.WF.Dev2Interface.Port_Login(ep.No);
            //string orgno = WebUser.OrgNo;
        }

    }
}
