using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Services;

namespace ccbpm
{
    /// <summary>
    /// 工业自动化流程也是外部调用api流程.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class IndustrialAutomationWorkflowWSAPI : System.Web.Services.WebService
    {
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        /// <summary>
        /// 创建WorkID
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="userNo">用户编号</param>
        /// <returns>创建的Int64的workid.</returns>
        [WebMethod]
        public Int64 Node_CreateBlankWork(string userNo, string sid, string flowNo)
        {
            //如果当前的用户登录信息与传递来的用户不一致，就让其调用登录接口，让其登录。
            if (BP.Web.WebUser.No != userNo)
                BP.WF.Dev2Interface.Port_Login(userNo, sid);

            flowNo = BP.DA.DataType.ParseStringOnlyIntNumber(flowNo);   //规避注入风险，added by liuxc

            //创建WorkID,并返回.
            return BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo, null, null, userNo, null);
        }
        /// <summary>
        /// 执行发送
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sid">安全校验码</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">FID</param>
        /// <param name="ht">参数 Key Value 的参数.</param>
        /// <returns></returns>
        [WebMethod]
        public string Node_SendWork(string userNo, string sid, string flowNo, int nodeID, Int64 workid, int toNodeID, string toEmps, string paras)
        {
            //如果当前的用户登录信息与传递来的用户不一致，就让其调用登录接口，让其登录。
            if (BP.Web.WebUser.No != userNo)
                BP.WF.Dev2Interface.Port_Login(userNo, sid);

            BP.DA.AtPara ap = new BP.DA.AtPara(paras);

            BP.WF.SendReturnObjs objs= BP.WF.Dev2Interface.Node_SendWork(flowNo,workid,ap.HisHT,null,toNodeID,toEmps);
            return objs.ToMsgOfSpecText(); //输出特殊的格式，让接受者解析.
        }

        /// <summary>
        /// 让用户登录
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="pass">密码</param>
        /// <returns>不成功返回空，成功返回SID</returns>
        [WebMethod]
        public string Port_Login(string userNo, string pass)
        {
            BP.Port.Emp emp = new BP.Port.Emp(userNo);
            if (emp.CheckPass(pass) == false)
                return null;
            return BP.WF.Dev2Interface.Port_Login(userNo);
        }
    }
}
