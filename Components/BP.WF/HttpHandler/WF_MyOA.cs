using System;
using System.Text;
using BP.DA;
using BP.Port.WeiXin;
using BP.Tools;
using System.Linq;
using static BP.Port.WeiXin.Msg.WeiXinGZHModel;
using System.Collections;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 集成企业微信OA类
    /// </summary>
    public class WF_MyOA : DirectoryPageBase
    {
        /// <summary>
        /// 集成企业微信OA类
        /// </summary>
        public WF_MyOA()
        {

        }

        /// <summary>
        /// 初始化(处理登录用户)
        /// </summary>
        /// <returns></returns>
        public string MyOA_Init()
        {
            string code = this.GetRequestVal("code");
            //string state = this.GetRequestVal("state"); //执行标记.
            Hashtable myht = new Hashtable();
            if (BP.Web.WebUser.No == null)
            {
                /* 如果当前登录人员帐号为 null .*/
                string accessToken = WeiXinEntity.getAccessToken(); //获取 AccessToken
                myht.Add("Token", accessToken);
                string userId = getUserId(code, accessToken);
                User user = FormatToJson.ParseFromJson<User>(userId);

                if (user.ErrCode != 0)
                {
                    //throw new Exception("err@当前登录帐号为空，请检查AccessToken是否正确：" + accessToken);
                    return "err@当前登录帐号为空，请检查AccessToken是否正确：" + accessToken;
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
                            string str = BP.DA.DataType.ReadURLContext(url, 4000, Encoding.UTF8);
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
                                //throw new Exception("err@用户名错误，没有找到登录信息：" + accessToken);
                                return "err@用户名错误，没有找到登录信息：" + accessToken;
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
            
            return BP.Tools.Json.ToJson(myht);

            //if (!state.Contains(','))
            //    Response.Redirect("../CCMobile/" + state + ".htm");
            //else
            //{
            //    string[] vals = state.Split(',');
            //    if (vals[0].Contains("URL_") == true)
            //    {
            //        string url = vals[0].Substring(4);
            //        string dummyData = url.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
            //        if (dummyData.Length % 4 > 0)
            //        {
            //            dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
            //        }
            //        //Base64位解码
            //        byte[] bytes = Convert.FromBase64String(dummyData);
            //        url = UTF8Encoding.UTF8.GetString(bytes);
            //        Response.Redirect(url);
            //    }

            //    if (vals[0].Contains("MyView") == true)
            //    {
            //        Response.Redirect("../CCMobile/MyView.htm?WorkID=" + vals[1].Replace("WorkID_", "") + "&FK_Node=" + vals[2].Replace("FK_Node_", "") + "");
            //    }

            //    if (vals[0].Contains("FlowNo"))
            //    {
            //        string[] pks = vals[0].Split('_');
            //        Response.Redirect("../CCMobile/MyFlow.htm?FK_Flow=" + pks[1] + "&state=" + vals[1] + "");
            //    }

            //}
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

    }
}
