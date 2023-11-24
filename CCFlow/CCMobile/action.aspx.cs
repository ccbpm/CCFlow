using System;
using System.Linq;
using BP.WF;
using BP.Port.WeiXin;
using BP.En;
using BP.DA;
using System.Text;
using BP.Tools;

namespace CCFlow.CCMobile
{
    public partial class action : System.Web.UI.Page
    {
        public string GetVal(string key)
        {
            string val = this.Request.QueryString[key];
            return BP.Tools.DealString.DealStr(val);
        }

        #region 参数。
        public string DoType
        {
            get
            {
                return this.GetVal("DoType");
            }
        }
        public string UserNo
        {
            get
            {
                return this.GetVal("UserNo");
            }
        }
        public string Password
        {
            get
            {
                return this.GetVal("Password");
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
                case "Login":
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
            if (this.DoType != null)
            {
                DoTypeAction();
                return;
            }

            string code =  this.GetVal("code");
            string state = this.GetVal("state"); //执行标记.
            //if (state.Equals("TodoList", StringComparison.OrdinalIgnoreCase) == false)
            //    return;

            if (BP.Web.WebUser.No == null)
            {
                /* 如果当前登录人员帐号为 null .*/
                string accessToken = WeiXinEntity.getAccessToken(); //获取 AccessToken
                string userId = getUserId(code, accessToken);
                User user = FormatToJson.ParseFromJson<User>(userId);
                
                if (string.IsNullOrEmpty(userId) == true)
                {
                    this.Response.Write("@当前登录帐号为空，请检查AccessToken是否正确：" + accessToken);
                    return;
                }
                else
                {
                    BP.Port.Emps emps = new BP.Port.Emps();
                    int num = emps.Retrieve(BP.Port.EmpAttr.No, user.UserId);
                    if (num <= 0)
                    { 
                        //是否使用微信企业号中的组织结构进行登录
                        if (BP.Difference.SystemConfig.OZType == "1")
                        {
                            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token=" + accessToken + "&userid=" + user.UserId;
                            string str = BP.DA.DataType.ReadURLContext(url,4000, Encoding.UTF8);
                            //人员详细信息
                            UserEntity userEn = FormatToJson.ParseFromJson<UserEntity>(str);

                            //所属部门
                            Array depts = userEn.department as Array;
                            foreach (object item in depts)
                            {
                                BP.Port.Depts dts = new BP.Port.Depts();
                                url = "https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token=" + accessToken + "&id=" + item;
                                str = BP.DA.DataType.ReadURLContext(url, 4000, Encoding.UTF8);

                                //部门详细信息
                                DeptList mydepts = FormatToJson.ParseFromJson<DeptList>(str);
                                //如果部门不存在，插入部门信息
                                if (dts.Retrieve(BP.Port.DeptAttr.No, item) <= 0)
                                {
                                    foreach (DeptEntity deptMent in mydepts.department)
                                    {
                                        dts = new BP.Port.Depts();
                                        if (dts.Retrieve(BP.Port.DeptAttr.No, item) > 0)
                                            continue;
                                        else
                                        {
                                            BP.Port.Dept dp = new BP.Port.Dept();
                                            dp.No = deptMent.id;
                                            dp.Name = deptMent.name;
                                            dp.ParentNo = deptMent.parentid;
                                            dp.Insert();
                                        }
                                    }
                                }
                                else //如果存在，更新部门名称
                                {
                                    foreach (DeptEntity deptMent in mydepts.department)
                                    {
                                        BP.Port.Dept dp = new BP.Port.Dept(deptMent.id);
                                        dp.Name = deptMent.name;
                                        dp.ParentNo = deptMent.parentid;
                                        dp.Update();
                                    }
                                }
                                //插入人员表
                                BP.Port.Emp emp = new BP.Port.Emp();
                                emp.No = user.UserId;
                                emp.DeptNo = item.ToString();
                                emp.Name = userEn.name;
                                emp.Tel = userEn.mobile;
                                emp.Insert();

                                //插入部门表
                                BP.Port.DeptEmp deptEmp = new BP.Port.DeptEmp();
                                deptEmp.MyPK = item + "_" + user.UserId;
                                deptEmp.DeptNo = item.ToString();
                                deptEmp.EmpNo = user.UserId;
                                deptEmp.Insert();
                            }
                            //执行登录
                            BP.WF.Dev2Interface.Port_Login(user.UserId);
                        }
                        else
                        {
                            emps = new BP.Port.Emps();
                            num = emps.Retrieve(BP.Port.EmpAttr.Tel, user.UserId);
                            if (num <= 0)
                            {
                                this.Response.Write("@用户名错误，没有找到登录信息：" + accessToken);
                                return;
                            }
                            foreach (BP.Port.Emp emp in emps)
                            {
                                BP.WF.Dev2Interface.Port_Login(emp.No);
                                break;
                            }
                        }
                    }
                    else
                    {
                        BP.WF.Dev2Interface.Port_Login(user.UserId);
                    }
                }
            }
           
            if (!state.Contains(','))
                Response.Redirect("../CCMobile/" + state + ".htm");
            else
            {
                string[] vals = state.Split(',');
                if(vals[0].Contains("URL_") == true)
                {
                    string url = vals[0].Substring(4);
                    string dummyData = url.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                    if (dummyData.Length % 4 > 0)
                    {
                        dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                    }
                    //Base64位解码
                    byte[] bytes = Convert.FromBase64String(dummyData);
                    url = UTF8Encoding.UTF8.GetString(bytes);
                    Response.Redirect(url);
                }

                if (vals[0].Contains("MyView") == true)
                {
                    Response.Redirect("../CCMobile/MyView.htm?WorkID=" + vals[1].Replace("WorkID_","") + "&FK_Node=" + vals[2].Replace("FK_Node_","") + "");
                }

                if (vals[0].Contains("FlowNo"))
                {
                    string[] pks = vals[0].Split('_');
                    Response.Redirect("../CCMobile/MyFlow.htm?FK_Flow=" + pks[1] + "&state=" + vals[1] + "");
                }

            }
        }
        /// <summary>
        /// 获得用户ID
        /// </summary>
        /// <param name="code"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public string getUserId(string code, string accessToken)
        {
            string url = "https://qyapi.weixin.qq.com/cgi-bin/auth/getuserinfo?access_token=" + accessToken + "&code=" + code;
            return DataType.ReadURLContext(url, 39000, Encoding.UTF8);
        }

        
        //获取用户ID
        //public string getUserId_bak2020(string code, string accessToken)
        //{
        //    string userId = string.Empty;

        //    string url = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token=" + accessToken + "&code=" + code + "&agentid=2";
        //    try
        //    {
        //        User user = new User();
        //        HttpWebResponse response = new BP.EAI.Plugins.HttpWebResponseUtility().CreateGetHttpResponse(url, 10000, null, null);
        //        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        //        string str = reader.ReadToEnd();
        //        user = FormatToJson.ParseFromJson<User>(str);
        //        reader.Dispose();
        //        reader.Close();
        //        if (response != null) response.Close();
        //        if (user != null)
        //            userId = user.UserId;
        //    }
        //    catch
        //    {
        //    }
        //    return userId;
        //}
    }


}