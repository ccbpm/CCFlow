using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using BP.Tools;

namespace BP.WF.WXin
{
    public class WeiXin
    {
        private string appid = BP.Sys.SystemConfig.WX_CorpID;// "wx8eac6a18c5efec30";
        private string appsecret = BP.Sys.SystemConfig.WX_AppSecret;// "KfFkE9AZ3Zp09zTuKvmqWLgtLj-_cHMPTvV992apOWgSKJHcbjpbu1jYVXh7gI7K";
        public string getAccessToken()
        {
            string accessToken = string.Empty;
            string url = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=" + appid + "&corpsecret=" + appsecret + "";

            try
            {
                AccessToken AT = new AccessToken();
                HttpWebResponse response = new HttpWebResponseUtility().CreateGetHttpResponse(url, 10000, null, null);
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string str = reader.ReadToEnd();
                AT = FormatToJson.ParseFromJson<AccessToken>(str);
                reader.Dispose();
                reader.Close();
                if (response != null) response.Close();
                if (AT != null)
                {
                    accessToken = AT.access_token;
                }
            }
            catch
            {
            }
            return accessToken;
        }

        #region 发送微信信息
        public MessageErrorModel PostWeiXinMsg(StringBuilder sb)
        {
            string wxStr = string.Empty;
            string url = "https://qyapi.weixin.qq.com/cgi-bin/message/send?";
            MessageErrorModel m = new MessageErrorModel();
            wxStr = PostForWeiXin(sb, url);
            m = FormatToJson.ParseFromJson<MessageErrorModel>(wxStr);
            return m;
        }
        /// <summary>
        /// POST方式请求 微信返回信息
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <param name="URL">请求地址</param>
        /// <returns>返回字符</returns>
        public string PostForWeiXin(StringBuilder parameters, string URL)
        {
            string access_token = getAccessToken();
            string url = URL + "access_token=" + access_token;

            HttpWebResponse response = new HttpWebResponseUtility().WXCreateGetHttpResponse(url, parameters, 10000, null, Encoding.UTF8, null);
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string str = reader.ReadToEnd();
            WriteLog(url + "----------------" + parameters + "---------------" + str);
            return str;
        }
        #endregion
        #region 记录相关交互日志
        /// 写日志(用于跟踪)
        /// </summary>
        private void WriteLog(string strMemo)
        {
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(@"logs\")))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(@"logs\"));
            }
            string filename = System.Web.HttpContext.Current.Server.MapPath(@"logs/" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            StreamWriter sr = null;
            try
            {
                if (!File.Exists(filename))
                {
                    sr = File.CreateText(filename);
                }
                else
                {
                    sr = File.AppendText(filename);
                }
                sr.WriteLine(strMemo);
            }
            catch
            {
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
        }
        #endregion

        /// <summary>
        /// 下载人员头像
        /// </summary>
        public bool DownLoadUserIcon(string savePath)
        {
            if (Directory.Exists(savePath) == false)
                Directory.CreateDirectory(savePath);

            DeptMent_GetList deptMentList = GetDeptMentList();
            if (deptMentList != null && deptMentList.errcode == "0")
            {
                foreach (DeptMentInfo deptMent in deptMentList.department)
                {
                    UsersBelongDept users = GetUserListByDeptID(deptMent.id);
                    if (users != null && users.errcode == "0")
                    {
                        foreach (UserInfoBelongDept userInfo in users.userlist)
                        {
                            if (userInfo.avatar != null)
                            {
                                //大图标
                                string headimgurl = userInfo.avatar;
                                string UserIcon = savePath + "\\" + userInfo.userid + "Biger.png";
                                BP.DA.DataType.HttpDownloadFile(headimgurl, UserIcon);

                                //小图标
                                string iconSize = userInfo.avatar.Substring(headimgurl.LastIndexOf('/'));
                                if (iconSize == "/")
                                    headimgurl = userInfo.avatar + "64";
                                else
                                    headimgurl = userInfo.avatar.Substring(0, headimgurl.LastIndexOf('/')) + "64";
                                UserIcon = savePath + "\\" + userInfo.userid + "Smaller.png";
                                BP.DA.DataType.HttpDownloadFile(headimgurl, UserIcon);
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取部门集合
        /// </summary>
        public DeptMent_GetList GetDeptMentList()
        {
            string access_token = getAccessToken();
            string url = "https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token=" + access_token;
            try
            {
                string str = new HttpWebResponseUtility().HttpResponseGet(url);
                DeptMent_GetList departMentList = FormatToJson.ParseFromJson<DeptMent_GetList>(str);

                //部门集合
                if (departMentList != null)
                    return departMentList;
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 获取指定部门下的人员
        /// </summary>
        /// <param name="FK_Dept">部门编号</param>
        /// <returns></returns>
        public UsersBelongDept GetUserListByDeptID(string FK_Dept)
        {
            string access_token = getAccessToken();
            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token=" + access_token + "&department_id=" + FK_Dept + "&status=0";
            try
            {
                string str = new HttpWebResponseUtility().HttpResponseGet(url);
                UsersBelongDept users = FormatToJson.ParseFromJson<UsersBelongDept>(str);

                //人员集合
                if (users != null)
                    return users;
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return null;
        }
    }
}
