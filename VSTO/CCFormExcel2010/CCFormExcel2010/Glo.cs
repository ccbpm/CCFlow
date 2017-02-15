using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Management;

namespace BP.Excel
{
    /// <summary>
    /// 全局
    /// </summary>
    public class Glo
    {
        #region 参数.
        /// <summary>
        /// 当前登录用户编号
        /// </summary>
        public static string UserNo = "wangtao";
        /// <summary>
        /// 当前登录用户SID
        /// </summary>
        public static string SID = "2222";
        /// <summary>
        /// Excel表单编号
        /// </summary>
        public static string FrmID = "CY3023";
        /// <summary>
        /// 发起流程编号
        /// </summary>
        public static string FK_Flow = "002";
        /// <summary>
        /// 当前工作ID
        /// </summary>
        public static int WorkID = 1000;
        /// <summary>
        /// 当前Excel表单绑定的节点ID
        /// </summary>
        public static int FK_Node = 301;
        /// <summary>
        /// 插件引用的服务地址
        /// </summary>
        public static string WSUrl = "http://localhost:26507/WF/CCForm/CCFormAPI.asmx";
        #endregion 参数.

        /// <summary>
        /// 参数是否加载成功，加载不成功，所有插件功能不启用
        /// </summary>
        public static bool LoadSuccessful { get; set; }

        #region 方法.
        /// <summary>
        /// 得到 WebService 对象 
        /// </summary>
        /// <returns></returns>
        public static CCFormExcel2010.CCForm.CCFormAPISoapClient GetCCFormAPISoapClient()
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
            if (url == null)
                url = "http://localhost/WF/CCForm/CCFormAPI.asmx";

            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(CCFormExcel2010.CCForm.CCFormAPISoapClient).GetConstructor(
                new Type[] {
                    typeof(Binding), 
                    typeof(EndpointAddress)
                });
            return (CCFormExcel2010.CCForm.CCFormAPISoapClient)ctor.Invoke(
                new object[] { basicBinding, endPoint });
        }
        #endregion 方法.

        /// <summary>
        /// 获取EXCEL的启动参数
        /// <para>fromccflow:true表示是从ccflow启动的Excel进程</para>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetArguments()
        {
            string argstr = string.Empty;
            string prefix = "-fromccflow,";
            int beginidx = -1;
            Dictionary<string, string> args = new Dictionary<string, string>();

            using (ManagementObjectSearcher mos = new ManagementObjectSearcher(
                "SELECT CommandLine FROM Win32_Process WHERE ProcessId = "
                + System.Diagnostics.Process.GetCurrentProcess().Id))
            {
                foreach (ManagementObject mo in mos.Get())
                {
                    argstr = mo["CommandLine"] as string;
                    break;
                }
            }

            beginidx = argstr.IndexOf(prefix);

            if (beginidx == -1 || (beginidx + prefix.Length) == argstr.Length - 1)
            {
                args.Add("fromccflow", "false");
                return args;
            }

            beginidx = beginidx + prefix.Length;
            argstr = argstr.Substring(beginidx);

            if (argstr.IndexOf(' ') != -1)
                argstr = argstr.Substring(0, argstr.IndexOf(' '));

            string[] argsArr = argstr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] ars = null;
            
            args.Add("fromccflow", "true");

            foreach(string arg in argsArr)
            {
                ars = arg.Split('=');

                if (ars.Length == 1)
                    continue;

                args.Add(ars[0], ars[1]);
            }

            return args;
        }

    }
}
