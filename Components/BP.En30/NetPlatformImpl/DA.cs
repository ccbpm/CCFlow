using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using BP.DA;

namespace BP.NetPlatformImpl
{
    public class DA_DataType
    {
#if DEBUG
        static TimeSpan ts = new TimeSpan(0, 10, 0);
#else
       static TimeSpan ts = new TimeSpan(0, 1, 0);
#endif

        public static BP.En30.ccportal.PortalInterfaceSoapClient GetPortalInterfaceSoapClientInstance()
        {
            var basicBinding = new BasicHttpBinding()
            {
                //CloseTimeout = ts,
                //OpenTimeout = ts,
                ReceiveTimeout = ts,
                SendTimeout = ts,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                Name = "PortalInterfaceSoapClient"
            };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;

            //url.
            string url = DataType.BPMHost + "/DataUser/PortalInterface.asmx";

            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(BP.En30.ccportal.PortalInterfaceSoapClient).GetConstructor(
                new Type[] {
                    typeof(Binding),
                    typeof(EndpointAddress)
                });
            return (BP.En30.ccportal.PortalInterfaceSoapClient)ctor.Invoke(new object[] { basicBinding, endPoint });
        }
    }
}