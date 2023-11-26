using System.Net;
using BP.WF.NetPlatformImpl;

namespace BP.WF
{
    /// <summary>
    /// 调用webservices.
    /// </summary>
    public class DynamicWebService
    {
        /// <summary>
        /// 调用webservices.
        /// </summary>
        private DynamicWebService()
        {

        }
        /// <summary>
        /// 动态调用web服务
        /// </summary>
        /// <param name="url">链接串</param>
        /// <param name="methodname">方法名</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static object InvokeWebService(string url, string methodName, object[] args)
        {
            return DynamicWebService.InvokeWebService(url, null, methodName, args);
        }
        private static CookieContainer container = new CookieContainer();
        /// <summary>
        /// 动态调用web服务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="classname"></param>
        /// <param name="methodname"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object InvokeWebService(string url, string className, string methodName, object[] args)
        {
            return WF_DynamicWebService.InvokeWebService(url, className, methodName, args);
        }
    }
}

