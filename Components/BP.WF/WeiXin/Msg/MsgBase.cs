using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.GPM.WeiXin.Msg
{
    /// <summary>
    /// 微信-消息公共类
    /// </summary>
    abstract public class MsgBase
    {
        /// <summary>
        /// 必须：是- 调用接口凭证
        /// </summary>
        public string Access_Token { get; set; }
        /// <summary>
        /// 必须：否- 成员ID列表（消息接收者，多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为@all，则向关注该企业应用的全部成员发送 
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 必须：否- 部门ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为@all时忽略本参数 
        /// </summary>
        public string toparty { get; set; }
        /// <summary>
        /// 必须：否- 标签ID列表，多个接收者用‘|’分隔。当touser为@all时忽略本参数 
        /// </summary>
        public string totag { get; set; }
        /// <summary>
        /// 必须：是- 企业应用的id，整型。可在应用的设置页面查看 
        /// </summary>
        public string agentid { get; set; }
        /// <summary>
        /// 必须：否- ccflow 业务ID
        /// </summary>
        public string WorkID { get; set; }

        /// <summary>
        /// 表示是否是保密消息
        /// </summary>
        private string _Safe = "0";
        /// <summary>
        /// 必须：否- 表示是否是保密消息，0表示否，1表示是，默认0
        /// </summary>
        public string safe
        {
            get
            {
                return this._Safe;
            }
            set
            {
                this._Safe = value;
            }
        }
    }
}
