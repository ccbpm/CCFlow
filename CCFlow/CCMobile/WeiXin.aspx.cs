using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using System.Text;
using System.IO;
using System.Xml;
using BP.Sys;
using System.Net;
using BP.GPM.WeiXin;

namespace CCFlow.CCMobile
{
    public partial class weixin : System.Web.UI.Page
    {
        //配置的token，在设置回调API验证时，随机生成或指定的编号
        public string token = BP.Difference.SystemConfig.WX_WeiXinToken;// "675fansU6iesL88Ryd";
        protected void Page_Load(object sender, EventArgs e)
        {
            //获取企业号发送的4个参数，验证URL有效性
            string echoString = HttpContext.Current.Request.QueryString["echoStr"];//加密的随机字符串
            string signature = HttpContext.Current.Request.QueryString["msg_signature"]; //微信加密签名
            string timestamp = HttpContext.Current.Request.QueryString["timestamp"];//时间戳
            string nonce = HttpContext.Current.Request.QueryString["nonce"];//随机数
            //返回参数，微信企业号验证成功后，自动赋值，如果为空，说明验证失败
            string decryptEchoString = "";
            //开始验证
            if (CheckSignature(this.token, signature, timestamp, nonce, BP.Difference.SystemConfig.WX_CorpID, BP.Difference .SystemConfig.WX_EncodingAESKey, echoString, ref decryptEchoString))
            {
                //不为空，说明验证成功，将参数，返回给企业号
                if (!string.IsNullOrEmpty(decryptEchoString))
                {
                    HttpContext.Current.Response.Write(decryptEchoString);

                    HttpContext.Current.Response.End();
                }
            }
        }
        #region 用于 回调模式 认证方法 （完成任务可以注释 或者删除）

        #region 推送文本，图片，语音，视频信息到微信
        /// <summary>
        /// 推送文本，图片，语音，视频信息到微信 
        /// </summary>
        /// <param name="touser">接收人</param>
        /// <param name="toparty">部门编号 多个用|隔开</param>
        /// <param name="totag">标签多个用|隔开</param>
        /// <param name="msgtype">信息类型</param>
        /// <param name="msg">信息内容，如果非文本 则是 媒体 ID</param>
        /// <returns></returns>
        private StringBuilder ResponseMsg(string touser, string toparty, string totag, string msgtype, string msg)
        {
            StringBuilder sbStr = new StringBuilder();
            string msgTypeStr = string.Empty;
            switch (msgtype)
            {
                case "text"://纯文本模式
                    msgTypeStr = " \"text\": { \"content\":\"" + msg + "\"  },";
                    break;
                case "image"://图片连接方式
                    msgTypeStr = " \"image\": { \"media_id\":\"" + msg + "\"  },";
                    break;
                case "voice"://声像模式
                    msgTypeStr = " \"voice\": { \"media_id\":\"" + msg + "\"  },";
                    break;
                case "video"://视频模式
                    msgTypeStr = " \"video\": { \"media_id\":\"" + msg + "\",\"\":'标题',\"description\":'描述'  },";
                    break;
                default:
                    msgTypeStr = " \"text\": { \"content\":'数据类型错误！'  },";
                    break;
            }
            //生成消息模版，通用的，如不使用通用消息模版，需要上传自定义消息模版，并修改以下代码
            sbStr.Append("{");
            sbStr.Append("\"touser\":\"" + touser + "\",");
            sbStr.Append("\"toparty\":\"" + toparty + "\",");
            sbStr.Append("\"totag\":\"" + totag + "\",");
            sbStr.Append("\"msgtype\":\"" + msgtype + "\",");
            sbStr.Append("\"agentid\":\"" + BP.Difference.SystemConfig.WX_AgentID + "\",");
            sbStr.Append(msgTypeStr);
            sbStr.Append("\"safe\":\"0\"");
            sbStr.Append("}");
            return sbStr;
        }
        #endregion
        //<summary>
        //验证企业号签名
        //</summary>
        //<param name="token">企业号配置的Token</param>
        //<param name="signature">签名内容</param>
        //<param name="timestamp">时间戳</param>
        //<param name="nonce">nonce参数</param>
        //<param name="corpId">企业号ID标识</param>
        //<param name="encodingAESKey">加密键</param>
        //<param name="echostr">内容字符串</param>
        //<param name="retEchostr">返回的字符串</param>
        //<returns></returns>
        public bool CheckSignature(string token, string signature, string timestamp, string nonce, string corpId, string encodingAESKey, string echostr, ref string retEchostr)
        {
            //开始验证企业号code是否正确、token与encodingAESKey是否与在微信企业号中配置的一致
            WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(token, encodingAESKey, corpId);
            //开始解密验证，如果成功，设置retEchostr返回值
            int result = wxcpt.VerifyURL(signature, timestamp, nonce, echostr, ref retEchostr);
            if (result != 0)
            {
                return false;
            }
            return true;
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



    }

}