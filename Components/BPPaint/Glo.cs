using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using BPPaint;
using BPPaint.FF;
namespace BP
{
    public class Glo
    {
        /// <summary>
        /// 得到WebService对象
        /// </summary>
        /// <returns></returns>
        public static BPPaint.FF.CCFormSoapClient GetCCFormSoapClientServiceInstance()
        {
            var basicBinding = new BasicHttpBinding() { MaxBufferSize = 2147483647, MaxReceivedMessageSize = 2147483647, Name = "CCFormSoapClient" };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;
            var endPoint = new EndpointAddress(Glo.BPMHost + "/WF/MapDef/CCForm/CCForm.asmx");
            var ctor =
                typeof(CCFormSoapClient).GetConstructor(new Type[] { typeof(Binding), typeof(EndpointAddress) });
            return (CCFormSoapClient)ctor.Invoke(new object[] { basicBinding, endPoint });
        }

        /// <summary>
        /// 当前BPMHost 
        /// </summary>
        private static string _BPMHost = null;
        /// <summary>
        /// 当前BPMHost 
        /// 比如:http://demo.ccflow.org:8888
        /// </summary>
        public static string BPMHost
        {
            get
            {
                if (_BPMHost != null)
                    return _BPMHost;
                var location = (HtmlPage.Window.GetProperty("location")) as ScriptObject;
                _BPMHost = "http://" + location.GetProperty("host");
                return _BPMHost;
            }
        }

    }
}

