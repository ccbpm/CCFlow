using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BP.Port
{
    public class WebUser
    {
        private static string _No = null;
        public static string No
        {
            get
            {
                if (_No != null)
                    return _No;

                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("UserNo") == false)
                    throw new Exception("@丢失UserNo参数.");
                _No = System.Windows.Browser.HtmlPage.Document.QueryString["UserNo"];
                return _No;
            }
            set
            {
                _No = value;
            }
        }

        private static string _SID = null;
        public static string SID
        {
            get
            {
                if (_SID != null)
                    return _SID;

                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("SID") == false)
                    return "123";

                    //throw new Exception("@丢失_SID参数.");
                _SID = System.Windows.Browser.HtmlPage.Document.QueryString["SID"];
                return _SID;
            }
            set
            {
                _No = value;
            }
        }
    }
}
