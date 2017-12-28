using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace IndustrialAutomationWorkflow
{
    public class Glo
    {
        static TimeSpan ts = new TimeSpan(0, 10, 0);
        /// <summary>
        /// 得到WebService对象
        /// </summary>
        /// <returns></returns>
        public static IndustrialAutomationWorkflow.ccbpmAPI.IndustrialAutomationWorkflowWSAPISoapClient  
            GetSoapClientInstance(string wsUrl)
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
            string url = wsUrl;

            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(IndustrialAutomationWorkflow.ccbpmAPI.IndustrialAutomationWorkflowWSAPISoapClient).GetConstructor(
                new Type[] { 
                    typeof(Binding),
                    typeof(EndpointAddress) 
                });
            return (IndustrialAutomationWorkflow.ccbpmAPI.IndustrialAutomationWorkflowWSAPISoapClient)ctor.Invoke(new object[] { basicBinding, endPoint });
        }
    }
}
