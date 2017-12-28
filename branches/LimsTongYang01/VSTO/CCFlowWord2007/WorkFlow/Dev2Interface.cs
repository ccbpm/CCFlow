using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Channels;
using CCFlowWord2007;
using CCFlowWord2007.CCFlow;
using BP.WF;

namespace BP.WF
{
    /// <summary>
    /// 数据库连接
    /// 2013.11.09 Liuxc
    /// </summary>
    public static class Dev2Interface
    {
        static Dev2Interface()
        {
            client = Dev2Interface.GetDesignerServiceInstance();
        }
        private static CCFlowAPISoapClient client;
        /// <summary>
        /// 得到WebService对象 
        /// </summary>
        /// <returns></returns>
        public static CCFlowAPISoapClient GetDesignerServiceInstance()
        {
            var basicBinding = new BasicHttpBinding()
            {
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                Name = "WSDesignerSoap"
            };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;
            string url = BP.WF.Glo.WFServ + "/WF/WorkOpt/CCFlowAPI.asmx";
            url = url.Replace("//", "/");
            url = url.Replace(":/", "://");
            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(CCFlowAPISoapClient).GetConstructor(new Type[] { typeof(Binding), typeof(EndpointAddress) });
            return (CCFlowAPISoapClient)ctor.Invoke(new object[] { basicBinding, endPoint });
        }

        #region 获取数据
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public static DataTable DataTable_DB_GenerCanStartFlowsOfDataTable()
        {
            return client.DataTable_DB_GenerCanStartFlowsOfDataTable(BP.Web.WebUser.No);
        }
        /// <summary>
        /// 获取他能发起的流程。
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_GenerCanStartFlowsOfDataTable()
        {
            string strs = client.DB_GenerCanStartFlowsOfDataTable(BP.Web.WebUser.No);
            return ToDataTable(strs);
        }
        public static DataTable ToDataTable(string strs)
        {
            byte[] array = Encoding.ASCII.GetBytes(strs);
            MemoryStream stream = new MemoryStream(array);             //convert stream 2 string      

            DataSet ds = new DataSet();
            ds.ReadXml(stream);

            return ds.Tables[0];
        }
        #endregion 获取数据


        #region 节点方法
        /// <summary>
        /// 产生WorkID
        /// </summary>
        /// <param name="flowNo"></param>
        /// <returns></returns>
        public static Int64 Node_CreateBlankWork(string flowNo)
        {
           return client.Node_CreateBlankWork(flowNo, BP.Web.WebUser.No, null);
        }
        /// <summary>
        /// 执行发送
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="toNodeID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        public static string Node_SendWork(string flowNo, Int64 workID, int toNodeID)
        {
            return client.Node_SendWork(flowNo, toNodeID, workID, null,BP.Web.WebUser.No);
        }
        #endregion 节点方法


        #region Port方法
        /// <summary>
        /// 执行登录
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="pass">密码</param>
        /// <returns></returns>
        public static string Port_Login(string userNo,string pass)
        {
            return client.Port_Login(userNo, pass);
        }
        #endregion 节点方法

    }
}
