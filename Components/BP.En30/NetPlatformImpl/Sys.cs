using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.NetPlatformImpl
{
    public static class PubClass
    {
        public static string RequestParas
        {
            get
            {
                string urlExt = "";
                string rawUrl = System.Web.HttpContext.Current.Request.RawUrl;
                rawUrl = "&" + rawUrl.Substring(rawUrl.IndexOf('?') + 1);
                string[] paras = rawUrl.Split('&');
                foreach (string para in paras)
                {
                    if (para == null
                        || para == ""
                        || para.Contains("=") == false)
                        continue;
                    urlExt += "&" + para;
                }
                return urlExt;
            }
        }
    }
}
