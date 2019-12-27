using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using BP.WF.Data;
using LitJson;
using System.Net;
using System.IO;


namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_AppClassic : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_AppClassic()
        {
        }


        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.

        #region xxx 界面 .

        #endregion xxx 界面方法.

        /// <summary>
        /// 蓝信登陆
        /// </summary>
        /// <returns></returns>
        public string LanXin_Login()
        {
            string code = GetRequestVal("code");
            
            if(DataType.IsNullOrEmpty(WebUser.Token) == false)
            {
                //刷新token
                string urlr = "http://xjtyjt.e.lanxin.cn:11180//sns/oauth2/refresh_token?refresh_token="+WebUser.Token+"&appid=100243&grant_type=refresh_token";
                string resultr = HttpPostConnect(urlr, "");
                JsonData jdr = JsonMapper.ToObject(resultr);
                resultr = jdr["errcode"].ToString();
                if (resultr == "0")
                {
                    WebUser.Token = jdr["access_token"].ToString();
                }
                 return WebUser.No;
            }


            //获取Token
            string url = "http://xjtyjt.e.lanxin.cn:11180/sns/oauth2/access_token?code=" + code + "&appid=100243&grant_type=authorization_code";
            string result = HttpPostConnect(url, "");
            JsonData jd = JsonMapper.ToObject(result);
            result = jd["errcode"].ToString();
            if (result != "0")
            {
                return "err@" + jd["errmsg"].ToString();
            }
            string access_token = jd["access_token"].ToString();
            string openId = jd["openid"].ToString();

            //获取用户信息
            url = "http://xjtyjt.e.lanxin.cn:11180/sns/userinfo?access_token=" + access_token + "&mobile=" + openId;
            result = HttpPostConnect(url, "");
            jd = JsonMapper.ToObject(result);
            result = jd["errcode"].ToString();
            if (result != "0")
            {
                return "err@" + jd["errmsg"].ToString();
            }
            string userNo = jd["openOrgMemberList"][0]["serialNumber"].ToString();
            string tel = jd["openOrgMemberList"][0]["mobile"].ToString();

            /**单点登陆*/
            Paras ps = new Paras();
            ps.SQL = "SELECT No FROM Port_Emp WHERE No=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "No and Tel=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "Tel";
            ps.Add("No", userNo);
            ps.Add("Tel", tel);
            string No = BP.DA.DBAccess.RunSQLReturnString(ps);
            if (DataType.IsNullOrEmpty(No))
                return "err@用户信息不正确，请联系管理员";

            BP.WF.Dev2Interface.Port_Login(userNo);
            WebUser.Token = access_token;            
            result = jd["errcode"].ToString();
            return userNo;
        }
        /// <summary>
        /// httppost方式发送数据
        /// </summary>
        /// <param name="url">要提交的url</param>
        /// <param name="postDataStr"></param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="encode">text code.</param>
        /// <returns>成功：返回读取内容；失败：0</returns>
        public static string HttpPostConnect(string serverUrl, string postData)
        {
            var dataArray = Encoding.UTF8.GetBytes(postData);
            //创建请求
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUrl);
            request.Method = "POST";
            request.ContentLength = dataArray.Length;
            //设置上传服务的数据格式  设置之后不好使
            //request.ContentType = "application/json";
            //请求的身份验证信息为默认
            request.Credentials = CredentialCache.DefaultCredentials;
            //请求超时时间
            request.Timeout = 10000;
            //创建输入流
            Stream dataStream;
            try
            {
                dataStream = request.GetRequestStream();
            }
            catch (Exception)
            {
                return "0";//连接服务器失败
            }
            //发送请求
            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();
            //读取返回消息
            string res;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {

                return "0";//连接服务器失败
            }
            return res;
        }
        /// <summary>
        /// 初始化Home
        /// </summary>
        /// <returns></returns>
        public string Home_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("UserNo", BP.Web.WebUser.No);
            ht.Add("UserName", BP.Web.WebUser.Name);

            //系统名称.
            ht.Add("SysName", BP.Sys.SystemConfig.SysName);
            ht.Add("CustomerName", BP.Sys.SystemConfig.CustomerName);

            ht.Add("Todolist_EmpWorks", BP.WF.Dev2Interface.Todolist_EmpWorks);
            ht.Add("Todolist_Runing", BP.WF.Dev2Interface.Todolist_Runing);
            ht.Add("Todolist_Sharing", BP.WF.Dev2Interface.Todolist_Sharing);
            ht.Add("Todolist_CCWorks", BP.WF.Dev2Interface.Todolist_CCWorks);
            ht.Add("Todolist_Apply", BP.WF.Dev2Interface.Todolist_Apply); //申请下来的任务个数.
            ht.Add("Todolist_Draft", BP.WF.Dev2Interface.Todolist_Draft); //草稿数量.
            ht.Add("Todolist_Complete", BP.WF.Dev2Interface.Todolist_Complete); //完成数量.
            ht.Add("UserDeptName", WebUser.FK_DeptName);

            //我发起
            MyStartFlows myStartFlows = new MyStartFlows();
            QueryObject obj = new QueryObject(myStartFlows);
            obj.AddWhere(MyStartFlowAttr.Starter, WebUser.No);
            obj.addAnd();
            //运行中\已完成\挂起\退回\转发\加签\批处理\
            obj.addLeftBracket();
            obj.AddWhere("WFState=2 or WFState=3 or WFState=4 or WFState=5 or WFState=6 or WFState=8 or WFState=10");
            obj.addRightBracket();
            obj.DoQuery();
            ht.Add("Todolist_MyStartFlow", myStartFlows.Count);

            //我参与
            MyJoinFlows myFlows = new MyJoinFlows();
            obj = new QueryObject(myFlows);
            obj.AddWhere("Emps like '%" + WebUser.No + "%'");
            obj.DoQuery();
            ht.Add("Todolist_MyFlow", myFlows.Count);

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        public string Index_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("Todolist_Runing", BP.WF.Dev2Interface.Todolist_Runing); //运行中.
            ht.Add("Todolist_EmpWorks", BP.WF.Dev2Interface.Todolist_EmpWorks); //待办
            ht.Add("Todolist_CCWorks", BP.WF.Dev2Interface.Todolist_CCWorks); //抄送.

            //本周.
            ht.Add("TodayNum", BP.WF.Dev2Interface.Todolist_CCWorks); //抄送.

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }

        #region 登录界面.
        public string Portal_Login()
        {
            string userNo = this.GetRequestVal("UserNo");

            try
            {
                BP.Port.Emp emp = new Emp(userNo);

                BP.WF.Dev2Interface.Port_Login(emp.No);
                return ".";
            }
            catch (Exception ex)
            {
                return "err@用户[" + userNo + "]登录失败." + ex.Message;
            }

        }
        /// <summary>
        /// 登录.
        /// </summary>
        /// <returns></returns>
        public string Login_Submit()
        {
            try
            {
                string userNo = this.GetRequestVal("TB_No");
                if (userNo == null)
                    userNo = this.GetRequestVal("TB_UserNo");

                string pass = this.GetRequestVal("TB_PW");
                if (pass == null)
                    pass = this.GetRequestVal("TB_Pass");

                BP.Port.Emp emp = new Emp();
                emp.No = userNo;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    if (DBAccess.IsExitsTableCol("Port_Emp", "NikeName") == true)
                    {
                        /*如果包含昵称列,就检查昵称是否存在.*/
                        Paras ps = new Paras();
                        ps.SQL = "SELECT No FROM Port_Emp WHERE NikeName=" + SystemConfig.AppCenterDBVarStr + "NikeName";
                        ps.Add("NikeName", userNo);
                        string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                        if (no == null)
                            return "err@用户名或者密码错误.";

                        emp.No = no;
                        int i = emp.RetrieveFromDBSources();
                        if (i == 0)
                            return "err@用户名或者密码错误.";
                    }
                    else if (DBAccess.IsExitsTableCol("Port_Emp", "Name") == true)
                    {
                        /*如果包含Name列,就检查Name是否存在.*/
                        Paras ps = new Paras();
                        ps.SQL = "SELECT No FROM Port_Emp WHERE Name=" + SystemConfig.AppCenterDBVarStr + "Name";
                        ps.Add("Name", userNo);
                        string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                        if (no == null)
                            return "err@用户名或者密码错误.";

                        emp.No = no;
                        int i = emp.RetrieveFromDBSources();
                        if (i == 0)
                            return "err@用户名或者密码错误.";


                    }
                    else
                    {
                        return "err@用户名或者密码错误.";
                    }
                }

                if (emp.CheckPass(pass) == false)
                    return "err@用户名或者密码错误.";

                //调用登录方法.
                BP.WF.Dev2Interface.Port_Login(emp.No);

                return "";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

       
        /// <summary>
        /// 执行登录
        /// </summary>
        /// <returns></returns>
        public string Login_Init()
        {
            string doType = GetRequestVal("LoginType");
            if(DataType.IsNullOrEmpty(doType) == false && doType.Equals("Out") == true)
            {
                //清空cookie
                WebUser.Exit();
            }
            Hashtable ht = new Hashtable();
            ht.Add("SysName", SystemConfig.SysName);
            ht.Add("ServiceTel", SystemConfig.ServiceTel);
            ht.Add("CustomerName", SystemConfig.CustomerName);
            if (WebUser.NoOfRel == null)
            {
                ht.Add("UserNo", "");
                ht.Add("UserName", "");
            }
            else
            {
                ht.Add("UserNo", WebUser.No);

                string name = WebUser.Name;

                if (DataType.IsNullOrEmpty(name) == true)
                    ht.Add("UserName", WebUser.No);
                else
                    ht.Add("UserName", name);
            }

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        #endregion 登录界面.

    }
}
