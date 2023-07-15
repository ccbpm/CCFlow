using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QuanYiSoft
{
    public class ClientAPISAAS
    {
        #region 公共变量设置
        public static string Domain = ""; //同一个系统下的流程表单目录,可以为空.
        public static string CCBPMHost = "http://localhost:2296"; //链接的ccbpm服务器.
        public static string OrgNo = "ccs"; //组织(系统编号). 为集团或者-SAAS模式有效.
        public static string PrivateKey = "DiGuaDiGua,IamCCBPM"; //私约.
        /// <summary>
        /// 数据返回格式:
        /// 不同的客户要求格式不同，在这里统一做转换即可.
        /// </summary>
        /// <param name="code">代码：200=成功，500=失败.</param>
        /// <param name="msg">消息</param>
        /// <param name="data">执行的数据,可以为空,也可以为json.</param>
        /// <returns>返回的是json格式的数据</returns>
        public static string Return_Info(int code, string msg, string data)
        {
            string json = "{ code:'" + code + "',msg:'" + msg + "',data:\"" + data + "\"}";
            return json;
        }
        private static string HttpPostConnect(string apiName)
        {
            string url = CCBPMHost + "/WF/API/" + apiName;
            return BP.Tools.PubGlo.HttpPostConnect(url, "");
            //string data =BP.Tools.Encode(data);
        }
        #endregion 公共变量设置

        #region 组织结构- 登陆登出.
        /// <summary>
        /// 登陆获得token
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="orgNo">组织编号(对saas/集团版有效.)</param>
        /// <returns>与用户的信息</returns>
        public static string Port_Login(string userNo)
        {
            try
            {
                string str = HttpPostConnect("Port_Login?userNo=" + userNo+ "&privateKey="+ PrivateKey+"&orgNo="+OrgNo);
                if (str != null && str.Equals("err@") == true)
                    return Return_Info(500, str, str);
                return Return_Info(200, str, str);
            }
            catch (Exception e)
            {
                return Return_Info(500, e.Message, e.Message);
            }
        }
        public static string Port_LoginOut(string userNo)
        {
            string str = HttpPostConnect("Port_LoginOut?userNo=" + userNo + "&PrivateKey=" + PrivateKey);
            return Return_Info(200, "退出成功", str);
        }
        #endregion 组织结构-登陆登出

        #region 组织结构同步.
        public static string Port_Emp_Save(string token, string userNo, string userName, string deptNo, string kv, string stats)
        {
            string msg = "";
            try
            {
                if (msg.Contains("err@") == true)
                    return Return_Info(500, msg, msg);
                return Return_Info(200, msg, msg);
            }
            catch (Exception ex)
            {
                return Return_Info(500, ex.Message, ex.StackTrace);
            }
        }
        public static string Port_Emp_Delete(string token, string userNo)
        {
            string msg = "";
            try
            {

                if (msg.Contains("err@") == true)
                    return Return_Info(500, msg, msg);
                return Return_Info(200, msg, msg);
            }
            catch (Exception ex)
            {
                return Return_Info(500, ex.Message, ex.StackTrace);
            }
        }
        public static string Port_Org_Save(string token, string name, string adminer, string adminerName, string keyVals)
        {
            string msg = "";
            try
            {

                if (msg.Contains("err@") == true)
                    return Return_Info(500, msg, msg);
                return Return_Info(200, msg, msg);
            }
            catch (Exception ex)
            {
                return Return_Info(500, ex.Message, ex.StackTrace);
            }
        }
        public static string Port_Dept_Save(string token, string no, string name, string parentNo, string keyVals)
        {
            string msg = "";
            try
            {

                if (msg.Contains("err@") == true)
                    return Return_Info(500, msg, msg);
                return Return_Info(200, msg, msg);
            }
            catch (Exception ex)
            {
                return Return_Info(500, ex.Message, ex.StackTrace);
            }
        }
        public static string Port_Dept_Delete(string token, string no)
        {
            string msg = "";
            try
            {

                if (msg.Contains("err@") == true)
                    return Return_Info(500, msg, msg);
                return Return_Info(200, msg, msg);
            }
            catch (Exception ex)
            {
                return Return_Info(500, ex.Message, ex.StackTrace);
            }
        }
        #endregion 组织结构同步.
    }
}
