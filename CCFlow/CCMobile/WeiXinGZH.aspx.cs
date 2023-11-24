
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;
using System.Xml;
using BP.DA;
using BP.Port.WeiXin;
using BP.Port.WeiXin.Msg;
using BP.Web;
using BP.WF.Port;

namespace CCFlow.CCMobile
{
    public partial class WeiXinGZH : System.Web.UI.Page
    {
        //配置的token，在设置回调API验证时，随机生成或指定的编号
        public string token = BP.Difference.SystemConfig.WXGZH_Token;
        protected void Page_Load(object sender, EventArgs e)
        {
            //获取公众号发送的4个参数，验证URL有效性
            string echoString = AntiXssEncoder.HtmlEncode(HttpContext.Current.Request.QueryString["echostr"], true);//加密的随机字符串
            string signature = AntiXssEncoder.HtmlEncode(HttpContext.Current.Request.QueryString["signature"], true); //微信加密签名
            string timestamp = AntiXssEncoder.HtmlEncode(HttpContext.Current.Request.QueryString["timestamp"], true);//时间戳
            string nonce = AntiXssEncoder.HtmlEncode(HttpContext.Current.Request.QueryString["nonce"], true);//随机数

            byte[] data = new byte[this.Request.InputStream.Length];
            this.Request.InputStream.Read(data, 0, data.Length); //获得传入来的数据.
            string txt = System.Text.Encoding.UTF8.GetString(data);


            string state = "";
            string eventKey = "";
            string openid = "";
            string ticket = "";
            XmlDocument xml = new XmlDocument();
            if (!DataType.IsNullOrEmpty(txt)) {
                xml.LoadXml(txt);

                state = xml.GetElementsByTagName("Event")[0].InnerText;
                eventKey = xml.GetElementsByTagName("EventKey")[0].InnerText;
                openid = xml.GetElementsByTagName("FromUserName")[0].InnerText;
                ticket = xml.GetElementsByTagName("Ticket")[0].InnerText;
            }
           
            //返回参数，微信公众号验证成功后，自动赋值，如果为空，说明验证失败
            string decryptEchoString = "";
            //开始验证
            if (CheckSignature(signature, timestamp, nonce, echoString, ref decryptEchoString))
            {
                //不为空，说明验证成功，将参数，返回给公众号
                if (!string.IsNullOrEmpty(decryptEchoString))
                {
                    if (state == "SCAN")
                    {
                        Emp emp = new Emp();
                        if (eventKey=="124") {
                            sendMsg(openid);
                        }
                        else
                        {
                            if (emp.IsExit(EmpAttr.OpenID, openid))
                            {
                                WFEmp wFEmp = new WFEmp();
                                if (wFEmp.IsExit(WFEmpAttr.No, emp.No))
                                {
                                    wFEmp.Token = ticket;
                                    wFEmp.Update();
                                }
                                else
                                {
                                    wFEmp.No = emp.No;
                                    wFEmp.Name = emp.Name;
                                    wFEmp.Token = ticket;
                                    wFEmp.Insert();
                                }
                                emp.OpenID = openid;
                                emp.Update();
                            }
                        }
                    }
                    HttpContext.Current.Response.Write(decryptEchoString);
                    HttpContext.Current.Response.End();

                }
            }

        }
        //<summary>
        //验证公众号签名
        //</summary>
        //<param name="signature">签名内容</param>
        //<param name="timestamp">时间戳</param>
        //<param name="nonce">nonce参数</param>
        //<param name="echostr">内容字符串</param>
        //<param name="retEchostr">返回的字符串</param>
        //<returns></returns>
        public bool CheckSignature( string signature, string timestamp, string nonce, string echostr, ref string retEchostr)
        {
            var token = this.token;
            var parameter = new List<string> { token, timestamp, nonce };
            parameter.Sort();
            var parameterStr = parameter[0] + parameter[1] + parameter[2];
            retEchostr = GetSHA1(parameterStr).Replace("-", "").ToLower();
            if (retEchostr == signature)
                return true;

            return false;
        }
        //SHA1加密
        public string GetSHA1(string input)
        {
            var output = string.Empty;
            var sha1 = new SHA1CryptoServiceProvider();
            var inputBytes = UTF8Encoding.UTF8.GetBytes(input);
            var outputBytes = sha1.ComputeHash(inputBytes);
            sha1.Clear();
            output = BitConverter.ToString(outputBytes);
            return output;
        }
        /// <summary>
        /// 向用户推送认证通知
        /// </summary>
        public void sendMsg(string touser) {
            string msg=WeiXinGZHEntity.SendMsg(touser);
        } 
    }
}