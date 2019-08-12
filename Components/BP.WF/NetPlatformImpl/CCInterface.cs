using BP.WF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace BP.WF.NetPlatformImpl
{
    public class WF_Glo
    {
        /// <summary>
        /// 得到WebService对象 
        /// </summary>
        /// <returns></returns>
        public static WF.CCInterface.PortalInterfaceSoapClient GetPortalInterfaceSoapClient()
        {
            TimeSpan ts = new TimeSpan(0, 5, 0);
            var basicBinding = new BasicHttpBinding()
            {
                ReceiveTimeout = ts,
                SendTimeout = ts,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                Name = "PortalInterfaceSoap"
            };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;

            string url = "";
            if (Glo.Platform == Platform.CCFlow)
            {
                url = "/DataUser/PortalInterface.asmx";
                url = Glo.HostURL + url;
            }
            else
            {
                //  url = string.Format("/{0}webservices/webservice.*", AppName != string.Empty ? AppName + "/" : string.Empty);
                //    url = new Uri(App.Current.Host.Source, "../").ToString() + "service/Service?wsdl";
            }

            url = url.Replace("//", "/");
            url = url.Replace(":/", "://");

            //  MessageBox.Show(url);

            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(WF.CCInterface.PortalInterfaceSoapClient).GetConstructor(
                new Type[] {
                    typeof(Binding),
                    typeof(EndpointAddress)
                });
            return (WF.CCInterface.PortalInterfaceSoapClient)ctor.Invoke(
                new object[] { basicBinding, endPoint });
        }
    }
}
