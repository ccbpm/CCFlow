using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.EAI.Plugins.DDSDK
{
    /// <summary>
    /// 签名包
    /// </summary>
    public class SignPackage
    {
        public string agentId { get; set; }
        public string corpId { get; set; }
        public string timeStamp { get; set; }
        public string nonceStr { get; set; }
        public string signature { get; set; }
        public string url { get; set; }
        public string rawstring { get; set; }
        public string jsticket { get; set; }
    }
}
