using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace BP.WF.Difference
{
    public class Glo
    {
        public static string Sha1Signature(string str)
        {
            string s = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1").ToString();
            return s.ToLower();
        }
    }
    
}
