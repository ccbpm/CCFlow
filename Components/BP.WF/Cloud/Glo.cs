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
            // xs 2019-7-29
            return BP.WF.NetPlatformImpl.Cloud_Glo.GetSoap();
        }
    }
}
