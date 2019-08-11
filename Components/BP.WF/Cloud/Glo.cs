using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace BP.WF.Cloud
{
    public class Glo
    {
        /// <summary>
        /// 获得Soap
        /// </summary>
        /// <returns></returns>
        public static BP.WF.CloudWS.WSSoapClient GetSoap()
        {
            TimeSpan ts = new TimeSpan(0, 1, 0);
            var basicBinding = new BasicHttpBinding()
            {
                ReceiveTimeout = ts,
                SendTimeout = ts,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                Name = "WSSoapClient"
            };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;
            string url = "http://online.ccflow.org/App/TemplateInterface/WS.asmx";
            //string url = "http://localhost:8482/App/TemplateInterface/WS.asmx";

            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(BP.WF.CloudWS.WSSoapClient).GetConstructor(
                new Type[] { 
                    typeof(Binding),
                    typeof(EndpointAddress) 
                });

            return (BP.WF.CloudWS.WSSoapClient)ctor.Invoke(new object[] { basicBinding, endPoint });
        }
    }
}
