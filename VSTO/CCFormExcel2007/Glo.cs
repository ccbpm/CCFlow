using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Collections.Generic;


namespace BP.Excel
{
    /// <summary>
    /// 全局
    /// </summary>
    public class Glo
    {
        #region 参数.
        public static string UserNo="wangtao";
        public static string SID="2222";
        public static string FrmID = "CY3023";
        public static string FK_Flow = "002";
        public static int WorkID=1000;
        public static int FK_Node=301;
        public static string WSUrl = "http://localhost:26507/WF/CCForm/CCFormAPI.asmx";
        #endregion 参数.

        #region 方法.
        /// <summary>
        /// 得到 WebService 对象 
        /// </summary>
        /// <returns></returns>
        public static CCFlowExcel2007.CCForm.CCFormAPISoapClient GetCCFormAPISoapClient()
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

            string url = Glo.WSUrl;

            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(CCFlowExcel2007.CCForm.CCFormAPISoapClient).GetConstructor(
                new Type[] {
                    typeof(Binding), 
                    typeof(EndpointAddress)
                });
            return (CCFlowExcel2007.CCForm.CCFormAPISoapClient)ctor.Invoke(
                new object[] { basicBinding, endPoint });
        }
        #endregion 方法.



    }
}
