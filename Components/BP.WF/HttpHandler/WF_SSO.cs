using System;
using System.Data;
using BP.DA;
using BP.Web;
using System.Collections;
using System.Xml;
using System.IO;
using BP.Tools;
using Newtonsoft.Json.Linq;
using BP.Difference;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_SSO : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_SSO()
        {
        }

        #region  界面 .
        public string SSO_Init()
        {

            if (WebUser.No == null)
            {
                Hashtable ht = new Hashtable();
                ht.Add("code", 200);
                ht.Add("SSOPath", BP.Difference.SystemConfig.GetValByKey("SSOPath", ""));
                ht.Add("JumpSSOServicePath", BP.Difference.SystemConfig.GetValByKey("JumpSSOServicePath", ""));
                return BP.Tools.Json.ToJson(ht);
            }
            return "info@已经登陆.";
        }
        public string SSO_Callback()
        {
            string ticket = this.GetRequestVal("Ticket");
            return SSO_LoginByST(ticket);
        }
        private string SSO_LoginByST(string ticket)
        {
            Hashtable ht = new Hashtable();
            //獲取验证ST路徑
            string SSOPathST = BP.Difference.SystemConfig.GetValByKey("SSOPathST", "");
            if (DataType.IsNullOrEmpty(SSOPathST))
            {
                return "err@沒有配置ST验证路径";
            }
            //验证ST
            string apiUrl = SSOPathST + "?ticket=" + ticket + "&service=" + BP.Difference.SystemConfig.GetValByKey("HostURL", "");
            string xmlString = BP.Tools.PubGlo.HttpGet(apiUrl);
            StringReader StrStream = null;
            XmlTextReader Xmlrdr = null;
            try
            {
                // 创建XmlDocument对象
                XmlDocument xmlDoc = new XmlDocument();
                // 加载XML字符串
                xmlDoc.LoadXml(xmlString);

                DataSet ds = new DataSet();
                //读取文件中的字符流
                StrStream = new StringReader(xmlDoc.InnerXml);
                //获取StrStream中的数据
                Xmlrdr = new XmlTextReader(StrStream);
                //ds获取Xmlrdr中的数据
                ds.ReadXml(Xmlrdr);
                if (ds.Tables.Contains("authenticationSuccess"))
                {
                    //登录成功
                    DataTable dt = ds.Tables["authenticationSuccess"];
                    //获得用户名.
                    string userNo = dt.Rows[0]["user"].ToString();
                    //执行登录
                    BP.WF.Dev2Interface.Port_Login(userNo, null, null, true);
                    string token = BP.WF.Dev2Interface.Port_GenerToken("PC");
                    //登录成功返回的参数
                    ht.Add("code", 200);
                    ht.Add("Token", token);
                    return BP.Tools.Json.ToJson(ht);
                }
                else
                {
                    DataTable dt = ds.Tables[0];
                    //登录失败，返回错误提示信息
                    ht.Add("code", 500);
                    ht.Add("msg", "ST无效，" + dt.Rows[0]["code"].ToString());
                    return BP.Tools.Json.ToJson(ht);
                }
            }
            catch (Exception e)
            {
                //返回异常
                ht.Add("code", 500);
                ht.Add("msg", e.Message);
                return BP.Tools.Json.ToJson(ht);
            }
            finally
            {
                //释放资源
                if (Xmlrdr != null)
                {
                    Xmlrdr.Close();
                    StrStream.Close();
                    StrStream.Dispose();
                }
            }
        }
        public string SSO_GetSTByTGT()
        {
            string tgt = HttpContextHelper.RequestCookieGet("", "CASTGC");
            if (DataType.IsNullOrEmpty(tgt) == true)
                throw new Exception("err@单点登录失败，未获取到凭证CASTGC");

            string serviceUrl = BP.Difference.SystemConfig.GetValByKey("HostURL", "");
            string url = BP.Difference.SystemConfig.GetValByKey("SSOPath", "") + "/v1/tickets/" + tgt + "?service=" + serviceUrl;
            string postData = "service=" + serviceUrl;
            string result = BP.Tools.PubGlo.HttpPostConnect(url, postData, "POST");
            if (result == null || result.Equals(""))
                return "err@根据TGT获取ST失败:没有返回结果";
            //数据序列化
            JObject jsonData = result.ToJObject();
            //code=0，表示请求成功，否则失败
            string code = jsonData["code"] != null ? jsonData["code"].ToString() : "";
            string msg = jsonData["msg"].ToString();
            if (code.Equals("0") == false)
                throw new Exception("err@根据TGT获取ST失败,错误码:" + code + ",错误信息:" + msg);
            string data = jsonData["data"].ToString();
            jsonData = data.ToJObject();
            string st = jsonData["st"] != null ? jsonData["st"].ToString() : "";
            if (st.Equals(""))
                throw new Exception("@根据TGT获取ST值失败");

            return SSO_LoginByST(st);
        }
        #endregion 界面方法.
    }
}
