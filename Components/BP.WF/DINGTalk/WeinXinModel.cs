using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.WXin
{
    public  class AccessToken
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }
    public  class User
    {
        public string UserId { get; set; }
        public string DeviceId { get; set; }
    }
    public class MessageErrorModel
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string invaliduser { get; set; }
        public string invalidparty { get; set; }
        public string invalidtag { get; set; }
    }

   public class WeinXinModel
    {
    }
}
