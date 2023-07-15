using BP.DA;
using BP.Difference;
using BP.En;
using BP.Sys;
using BP.Web;
using BP.WF;
using BP.WF.Template;
using System;
using System.Collections;
using System.Data;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CCFlow.DataUser.API.Controllers
{
    [EnableCors("*", "*", "*")]
    public class APIController : ApiController
    {
        public static Object Return_Info(int code, string msg, string data)
        {
            Hashtable ht = new Hashtable();
            ht.Add("code", code);
            ht.Add("message", msg);
            ht.Add("data", data);
            return ht;
            //string json = "{\"code\":" + code + ",\"msg\":\"" + msg + "\",\"data\":\"" + data + "\"}";
            //return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        #region 组织结构接口.
        /// <summary>
        /// 系统登陆
        /// </summary>
        /// <param name="privateKey">密钥,默认:DiGuaDiGua,IamCCBPM</param>
        /// <param name="userNo">登陆账号</param>
        /// <param name="orgNo">组织编号</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_Login(string privateKey, string userNo, string orgNo = null)
        {
            try
            {
                if (DataType.IsNullOrEmpty(userNo) == true)
                    return Return_Info(500, "账号不能为空", "");

                string localKey = BP.Difference.SystemConfig.GetValByKey("PrivateKey", "DiGuaDiGua,IamCCBPM");
                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    BP.WF.Port.Admin2Group.Org org = new BP.WF.Port.Admin2Group.Org(orgNo);
                    string key = org.GetValStrByKey("PrivateKey");
                    if (DataType.IsNullOrEmpty(key) == false)
                        localKey = key;
                }

                if (DataType.IsNullOrEmpty(localKey) == true)
                    localKey = "DiGuaDiGua,IamCCBPM";
                if (localKey.Equals(privateKey) == false)
                    return Return_Info(500, "私约错误，请检查全局文件中配置 PrivateKey", "");

                //执行本地登录.
                BP.WF.Dev2Interface.Port_Login(userNo, orgNo);
                string token = BP.WF.Dev2Interface.Port_GenerToken();

                Hashtable ht = new Hashtable();
                ht.Add("No", WebUser.No);
                ht.Add("Name", WebUser.Name);
                ht.Add("FK_Dept", WebUser.FK_Dept);
                ht.Add("FK_DeptName", WebUser.FK_DeptName);
                ht.Add("OrgNo", WebUser.OrgNo);
                ht.Add("OrgName", WebUser.OrgName);
                ht.Add("Token", token);
                // return ReturnMessage();
                return Return_Info(200, "登陆成功", BP.Tools.Json.ToJson(ht));
            }
            catch (Exception ex)
            {
                return Return_Info(500, "登陆失败", ex.Message);
            }
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_LoginOut(string userNo, string orgNo = null)
        {
            try
            {
                BP.WF.Dev2Interface.Port_SigOut();
                return Return_Info(200, "退出成功", null);
            }
            catch (Exception ex)
            {
                return Return_Info(500, "退出失败", ex.Message);
            }
        }
        #endregion 组织结构接口.

        #region 组织人员岗位维护
        /// <summary>
        /// 人员信息保存
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="orgNo"></param>
        /// <param name="userNo"></param>
        /// <param name="userName"></param>
        /// <param name="deptNo"></param>
        /// <param name="kvs"></param>
        /// <param name="stats"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_Emp_Save(string token, string orgNo, string userNo, string userName, string deptNo, string kvs, string stats)
        {
             Port_GenerToken(token);
            
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "不是管理员不能维护人员信息", null);

            string msg = BP.Port.OrganizationAPI.Port_Emp_Save(orgNo, userNo, userName, deptNo, kvs, stats);

            return Return_Info(200, "执行成功", msg);

        }
        /// <summary>
        /// 人员删除
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="orgNo">组织编号</param>
        /// <param name="userNo">人员编号</param>
        /// <returns>reutrn 1=成功,  其他的标识异常.</returns>
        [HttpGet, HttpPost]
        public Object Port_Emp_Delete(string token, string userNo, string orgNo = "")
        {
             Port_GenerToken(token);
            
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "不是管理员不能删除人员信息", null);

            return Return_Info(200, "删除成功", BP.Port.OrganizationAPI.Port_Emp_Delete(orgNo, userNo));
        }
        /// <returns>return 1 增加成功，其他的增加失败.</returns>

        /// <summary>
        /// 集团模式下同步组织以及管理员信息
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="orgNo">组织编号</param>
        /// <param name="name">组织名称</param>
        /// <param name="adminer">管理员账号</param>
        /// <param name="adminerName">管理员名字</param>
        /// <param name="keyVals">其他的值:@Leaer=zhangsan@Tel=12233333@Idx=1</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_Org_Save(string token, string orgNo, string name, string adminer, string adminerName, string keyVals = "")
        {
             Port_GenerToken(token);
          
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "[" + BP.Web.WebUser.Name + "]不是管理员不能维护组织信息", null);
            return Return_Info(200, "同步成功", BP.Port.OrganizationAPI.Port_Org_Save(orgNo, name, adminer, adminerName, keyVals));

        }

        /// <summary>
        /// 保存部门,如果有此数据则修改,无此数据则增加.
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="no">部门编号</param>
        /// <param name="name">名称</param>
        /// <param name="parentNo">父节点编号</param>
        /// <param name="orgNo">组织编号</param>
        /// <param name="keyVals">其他的值:@Leaer=zhangsan@Tel=18660153393@Idx=1</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_Dept_Save(string token, string no, string name, string parentNo, string orgNo = "", string keyVals = "")
        {
             Port_GenerToken(token);
           
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "[" + BP.Web.WebUser.Name + "]不是管理员不能维护部门信息", null);
            return Return_Info(200, "保存成功", BP.Port.OrganizationAPI.Port_Dept_Save(orgNo, no, name, parentNo, keyVals));

        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="no">要删除的部门编号</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_Dept_Delete(string token, string no)
        {
              Port_GenerToken(token);
           
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "[" + BP.Web.WebUser.Name + "]不是管理员不能删除部门信息", null);
            return Return_Info(200, "删除部门成功", BP.Port.OrganizationAPI.Port_Dept_Delete(no));

        }

        /// <summary>
        /// 保存岗位, 如果有此数据则修改，无此数据则增加.
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="orgNo">组织编号</param>
        /// <param name="no">岗位编号</param>
        /// <param name="name">岗位名称</param>
        /// <param name="keyVals">其他值</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_Station_Save(string token, string orgNo, string no, string name, string keyVals)
        {
            Port_GenerToken(token);
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "[" + BP.Web.WebUser.Name + "]不是管理员不能维护岗位信息", null);
            return Return_Info(200, "保存岗位成功", BP.Port.OrganizationAPI.Port_Station_Save(orgNo, no, name, keyVals));
        }
        /// <summary>
        /// 删除岗位.
        /// </summary>
        /// <param name="no">删除指定的岗位编号</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_Station_Delete(string token, string no)
        {
            //根据token登录
            Port_GenerToken(token);
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "[" + BP.Web.WebUser.Name + "]不是管理员不能删除岗位信息", null);
            return Return_Info(200, "删除岗位成功", BP.Port.OrganizationAPI.Port_Station_Delete(no));
        }

        /// <summary>
        /// 保存用户组, 如果有此数据则修改，无此数据则增加.
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="orgNo">组织编号</param>
        /// <param name="no">用户组编号</param>
        /// <param name="name">用户组名称</param>
        /// <param name="keyVals">其他值</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_Team_Save(string token, string orgNo, string no, string name, string keyVals)
        {
            Port_GenerToken(token);
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "[" + BP.Web.WebUser.Name + "]不是管理员不能维护用户组信息", null);
            return Return_Info(200, "保存用户组成功", BP.Port.OrganizationAPI.Port_Team_Save(orgNo, no, name, keyVals));
        }
        /// <summary>
        /// 删除用户组.
        /// </summary>
        /// <param name="no">删除指定的用户组编号</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_Team_Delete(string token, string no)
        {
            //根据token登录
            Port_GenerToken(token);
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "[" + BP.Web.WebUser.Name + "]不是管理员不能删除用户组信息", null);
            return Return_Info(200, "删除用户组成功", BP.Port.OrganizationAPI.Port_Team_Delete(no));
        }
        /// <summary>
        /// 保存用户组, 如果有此数据则修改，无此数据则增加.
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="orgNo">组织编号</param>
        /// <param name="no">用户组编号</param>
        /// <param name="name">用户组名称</param>
        /// <param name="keyVals">其他值</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_TeamType_Save(string token, string orgNo, string no, string name, string keyVals)
        {
            Port_GenerToken(token);
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "[" + BP.Web.WebUser.Name + "]不是管理员不能维护用户组信息", null);
            return Return_Info(200, "保存用户组成功", BP.Port.OrganizationAPI.Port_TeamType_Save(orgNo, no, name, keyVals));
        }
        /// <summary>
        /// 删除用户组.
        /// </summary>
        /// <param name="no">删除指定的用户组编号</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_TeamType_Delete(string token, string no)
        {
            //根据token登录
            Port_GenerToken(token);
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "[" + BP.Web.WebUser.Name + "]不是管理员不能删除用户组信息", null);
            return Return_Info(200, "删除用户组成功", BP.Port.OrganizationAPI.Port_TeamType_Delete(no));
        }

        /// <summary>
        /// 保存岗位类型, 如果有此数据则修改，无此数据则增加.
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="orgNo">组织编号</param>
        /// <param name="no">岗位类型编号</param>
        /// <param name="name">岗位类型名称</param>
        /// <param name="keyVals">其他值</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_StationType_Save(string token, string orgNo, string no, string name, string keyVals)
        {
            Port_GenerToken(token);
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "[" + BP.Web.WebUser.Name + "]不是管理员不能维护岗位类型信息", null);
            return Return_Info(200, "保存岗位类型成功", BP.Port.OrganizationAPI.Port_StationType_Save(orgNo, no, name, keyVals));
        }
        /// <summary>
        /// 删除岗位类型.
        /// </summary>
        /// <param name="no">删除指定的岗位类型编号</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Port_StationType_Delete(string token, string no)
        {
            //根据token登录
            Port_GenerToken(token);
            if (WebUser.IsAdmin == false)
                return Return_Info(500, "[" + BP.Web.WebUser.Name + "]不是管理员不能删除岗位类型信息", null);
            return Return_Info(200, "删除岗位类型成功", BP.Port.OrganizationAPI.Port_StationType_Delete(no));
        }
        #endregion  组织人员岗位维护


        #region 菜单接口.
        /// <summary>
        /// 可以发起的流程
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="domain">流程所属的域</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object DB_Start(string token, string domain="")
        {
            //根据token登录
            Port_GenerToken(token);
            
            //获取可以发起的列表
            DataTable dt = BP.WF.Dev2Interface.DB_StarFlows(BP.Web.WebUser.No, domain);
            return Return_Info(200, "获取可以发起的流程成功", BP.Tools.Json.ToJson(dt));
        }
        /// <summary>
        /// 待办
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object DB_Todolist(string token, string domain = "")
        {
            //根据token登录
            Port_GenerToken(token);
            //获取可以发起的列表
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(BP.Web.WebUser.No, 0, null, domain, null, null);
            return Return_Info(200, "获取待办列表成功", BP.Tools.Json.ToJson(dt));
        }
        /// <summary>
        /// 在途
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object DB_Runing(string token, string domain = ""    )
        {
            //根据token登录
            Port_GenerToken(token);
            //获取可以发起的列表
            DataTable dt = BP.WF.Dev2Interface.DB_GenerRuning(BP.Web.WebUser.No, false, domain);
            return Return_Info(200, "获取在途列表成功", BP.Tools.Json.ToJson(dt));
        }
        /// <summary>
        /// 草稿
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object DB_Draft(string token, string domain = "")
        {
            //根据token登录
            Port_GenerToken(token);
            //获取可以发起的列表
            DataTable dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable(null, domain);
            return Return_Info(200, "获取草稿成功", BP.Tools.Json.ToJson(dt));
        }

        /// <summary>
        /// 打开的表单
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workID">工作实例WorkID</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="fid">父WorkID</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object GenerFrmUrl(string token, Int64 workID, string flowNo, int nodeID)
        {
            //根据token登录
             Port_GenerToken(token);
            
            /*
             * 发起的url需要在该流程的开始节点的表单方案中，使用SDK表单，并把表单的url设置到里面去.
             * 设置步骤:
             * 1. 打开流程设计器.
             * 2. 在开始节点上右键，选择表单方案.
             * 3. 选择SDK表单，把url配置到文本框里去.
             * 比如: /App/F027QingJia.htm
             */
            if (nodeID == 0)
                nodeID = int.Parse(flowNo + "01");

            if (workID == 0)
                workID = BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo, BP.Web.WebUser.No);

            string url = "";
            Node nd = new Node(nodeID);
            if (nd.FormType == NodeFormType.SDKForm || nd.FormType == NodeFormType.SelfForm)
            {
                //.
                url = nd.FormUrl;
                if (url.Contains("?") == true)
                    url += "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID + "&WorkID=" + workID + "&Token=" + token + "&UserNo=" + BP.Web.WebUser.No;
                else
                    url += "?FK_Flow=" + flowNo + "&FK_Node=" + nodeID + "&WorkID=" + workID + "&Token=" + token + "&UserNo=" + BP.Web.WebUser.No;
            }
            else
            {
                url = "/WF/MyFlow.htm?FK_Flow=" + flowNo + "&FK_Node=" + nodeID + "&WorkID=" + workID + "&Token=" + token;
            }
            return Return_Info(200, "获取打开的表单成功", url);
        }
        #endregion 菜单接口.

        #region 节点方法.
        /// <summary>
        /// 创建WorkID
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="flowNo">流程编号</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Node_CreateBlankWorkID(string token, string flowNo)
        {
            //根据token登录
             Port_GenerToken(token);
           
            try
            {
                Int64 workid = Dev2Interface.Node_CreateBlankWork(flowNo, BP.Web.WebUser.No);
                return Return_Info(200, "创建WorkID成功", workid.ToString());
            }
            catch (Exception ex)
            {
                return Return_Info(500, "创建WorkID失败", ex.Message);
            }
        }
        /// <summary>
        /// 是否可以处理当前的工作
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Node_IsCanDealWork(string token, Int64 workID)
        {
            //根据token登录
            Port_GenerToken(token);
            try
            {
                GenerWorkFlow gwf = new GenerWorkFlow(workID);
                string todoEmps = gwf.TodoEmps;
                bool isCanDo = false;
                if (gwf.FK_Node.ToString().EndsWith("01") == true)
                {
                    if (gwf.Starter.Equals(BP.Web.WebUser.No) == false)
                        isCanDo = false; //处理开始节点发送后，撤销的情况，第2个节点打开了，第1个节点撤销了,造成第2个节点也可以发送下去.
                    else
                        isCanDo = true; // 开始节点不判断权限.
                }
                else
                {
                    isCanDo = todoEmps.Contains(";" + WebUser.No + ",");
                    if (isCanDo == false)
                        isCanDo = Dev2Interface.Flow_IsCanDoCurrentWork(workID, BP.Web.WebUser.No);
                }
                return Return_Info(200, "获取是否可以处理当前的工作成功", isCanDo == true ? "1" : "0");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "获取是否可以处理当前的工作失败", ex.Message);
            }
        }
        /// <summary>
        /// 设置草稿
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workID">流程实例ID</param>
        /// <param name="flowNo"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Node_SetDraft(string token, Int64 workID)
        {
            //根据token登录
            Port_GenerToken(token);
            try
            {
                BP.WF.Dev2Interface.Node_SetDraft(workID);
                return Return_Info(200, "设置草稿成功", "");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "设置草稿失败", ex.Message);
            }
        }

        [HttpGet, HttpPost]
        public Object Node_HuiQian_Delete(string token, Int64 workid, string empNo)
        {
            //根据token登录
            Port_GenerToken(token);
            try
            {
                BP.WF.Dev2Interface.Node_HuiQian_Delete(workid, empNo);
                return Return_Info(200, "删除会签人员成功", "1");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "删除会签人员失败", ex.Message);
            }
        }
        /// <summary>
        /// 会签：增加会签人
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workid"></param>
        /// <param name="empNos">人员编号:001,002</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Node_HuiQian_AddEmps(string token, Int64 workid, string empNos)
        {
            //根据token登录
            Port_GenerToken(token);
            try
            {
                BP.WF.Dev2Interface.Node_HuiQian_AddEmps(workid, "0", empNos);
                return Return_Info(200, "增加会签人员成功", "1");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "增加会签人员失败", ex.Message);
            }
        }
        /// <summary>
        /// 会签：执行发送.
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workid"></param>
        /// <param name="toNodeID">到达的节点ID, 也可以是节点Mark.</param>
        /// <returns>执行的结果</returns>
        [HttpGet, HttpPost]
        public Object Node_HuiQianDone(string token, Int64 workid, string toNodeIDStr = "0")
        {
            //根据token登录
            Port_GenerToken(token);

            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            int toNodeID = DealNodeIDStr(toNodeIDStr, gwf.FK_Flow);

            try
            {
                BP.WF.Dev2Interface.Node_HuiQianDone(workid, toNodeID);
                return Return_Info(200, "会签执行发送成功", "执行成功");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "会签执行发送失败", ex.Message);
            }
        }
        /// <summary>
        /// 把当前工作移交给指定的人员
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workID"></param>
        /// <param name="toEmpNo"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Node_Shift(string token, Int64 workID, string toEmpNo, string msg)
        {
        
            try
            {
                //根据token登录
                Port_GenerToken(token);

                msg = BP.WF.Dev2Interface.Node_Shift(workID, toEmpNo, msg);
                return Return_Info(200, "把当前工作移交给指定的人员成功", msg);
            }
            catch (Exception ex)
            {
                return Return_Info(500, "把当前工作移交给指定的人员失败", ex.Message);
            }
        }
        /// <summary>
        /// 给当前的工作增加处理人
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workID"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Node_AddTodolist(string token, Int64 workID, string empNo)
        {
            //根据token登录
            Port_GenerToken(token);
            try
            {
                BP.WF.Dev2Interface.Node_AddTodolist(workID, empNo);
                return Return_Info(200, "给当前的工作增加处理人成功", "增加人员成功");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "给当前的工作增加处理人失败", ex.Message);
            }
        }
        /// <summary>
        /// 根据ID获取当前的流程实例信息
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Flow_GenerWorkFlow(string token, Int64 workID)
        {
            //根据token登录
            Port_GenerToken(token);
            try
            {
                GenerWorkFlow gwf = new GenerWorkFlow(workID);
                return Return_Info(200, "根据ID获取当前的流程实例信息成功", gwf.ToJson());
            }
            catch (Exception ex)
            {
                return Return_Info(500, "根据ID获取当前的流程实例信息失败", ex.Message);
            }
        }
        /// <summary>
        /// 保存参数到WF_GenerWorkFlow,用于方向条件的判断
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workID">工作ID</param>
        /// <param name="paras">要保存的参数，用于控制流程转向等的参数变量，注意:数据并没有保存到表单, 格式： @Key1=Val2@Key2=Val2 </param>
        /// <returns>执行结果</returns>
        [HttpGet, HttpPost]
        public Object Flow_SaveParas(string token, Int64 workID, string paras)
        {
            //根据token登录
            try
            {
                Port_GenerToken(token);
                BP.WF.Dev2Interface.Flow_SaveParas(workID, paras);
                return Return_Info(200, "参数保存成功", "");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "参数保存失败", ex.Message);
            }
        }
        /// <summary>
        /// 保存节点表单数据
        /// </summary>
        /// <param name="token"></param>
        /// <param name="workID">工作id</param>
        /// <param name="paras">要保存的主表数据，格式： @Key1=Val2@Key2=Val2 </param>
        /// <returns>执行结果</returns>
        public Object Node_SaveWork(string token, Int64 workID, string paras)
        {
            //根据token登录
            try
            {
                Port_GenerToken(token);
                AtPara ap = new AtPara(paras);

                BP.WF.Dev2Interface.Node_SaveWork(workID, ap.HisHT);
                return Return_Info(200, "表单主表数据,执行成功.", "");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "参数保存失败", ex.Message);
            }
        }
        /// <summary>
        /// 设置标题
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workID"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Flow_SetTitle(string token, Int64 workID, string title)
        {
            //根据token登录
             Port_GenerToken(token);
            
            try
            {
                BP.WF.Dev2Interface.Flow_SetFlowTitle(null, workID, title);
                return Return_Info(200, "标题设置成功", "");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "标题设置失败", ex.Message);
            }
        }
        /// <summary>
        /// 发送接口
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workID">工作实例WorkID</param>
        /// <param name="toNodeIDStr">到达的下一个节点,默认为0,可以是节点Mark. </param>
        /// <param name="toEmps">下一个节点的接收人，多个人用逗号分开比如:zhangsan,lisi</param>
        /// <param name="paras">参数，保存到WF_GenerWorkFlow,用与参数条件,格式: @key1=val1@Key2=Val2</param>
        /// <param name="checkNote">审核意见:启用了审核组件，就需要填写审核意见,负责不让发送。</param>
        /// <returns>执行结果,可以直接提示给用户.</returns>
        [HttpGet, HttpPost]
        public Object Node_SendWork(string token, Int64 workID, string toNodeIDStr = "0", string toEmps = "", string paras = "", string checkNote = "")
        {
       
            try
            {
                //根据token登录
                Port_GenerToken(token);


                //保存参数.
                if (DataType.IsNullOrEmpty(paras) == false)
                    BP.WF.Dev2Interface.Flow_SaveParas(workID, paras);

                //写入审核意见.
                if (DataType.IsNullOrEmpty(checkNote) == false)
                    BP.WF.Dev2Interface.Node_WriteWorkCheck(workID, checkNote, null, null, null);

                string flowNo = DBAccess.RunSQLReturnString("SELECT FK_Flow FROM WF_GenerWorkFlow WHERE WorkID=" + workID);
                int toNodeID = DealNodeIDStr(toNodeIDStr, flowNo); //@hongyan.
                //执行发送.
                SendReturnObjs objs = Dev2Interface.Node_SendWork(flowNo, workID, null, null, toNodeID, toEmps);
                string msg = objs.ToMsgOfText();
                return Return_Info(200, "发送成功", msg);
            }
            catch (Exception ex)
            {
                return Return_Info(500, "发送失败", ex.Message);
            }
        }

        /// <summary>
        /// 根据流程编号获取流程实例
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="flowNo"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object DB_GenerWorkFlow(string token, string flowNo)
        {
            //根据token登录
           Port_GenerToken(token);
           
            try
            {
                GenerWorkFlows gwfs = new GenerWorkFlows();
                QueryObject qo = new QueryObject(gwfs);
                qo.AddWhere("FK_Flow", flowNo);
                qo.addAnd();
                qo.AddWhere("WFState", ">", 1);
                qo.addOrderBy("RDT");
                qo.DoQuery();
                return Return_Info(200, "根据流程编号获取流程实例成功", gwfs.ToJson());
            }
            catch (Exception ex)
            {
                return Return_Info(200, "根据流程编号获取流程实例失败", ex.Message);
            }
        }

        /// <summary>
        /// 获取当前节点可以退回到的节点集合
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作实例WorkID</param>
        /// <param name="fid">父WorkID</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object DB_GenerWillReturnNodes(string token, int nodeID, Int64 workID)
        {
          
           
            try
            {
                //根据token登录
                Port_GenerToken(token);
                DataTable dt = Dev2Interface.DB_GenerWillReturnNodes(workID);
                return Return_Info(200, "获取当前节点可以退回到的节点集合成功", BP.Tools.Json.ToJson(dt));
            }
            catch (Exception ex)
            {
                return Return_Info(500, "获取当前节点可以退回到的节点集合失败", ex.Message);
            }
        }
        private int DealNodeIDStr(string nodeIDStr, string flowNo)
        {
            int returnToNodeID = 0;
            if (DataType.IsNullOrEmpty(nodeIDStr) == true)
                return 0;
            if (DataType.IsNumStr(nodeIDStr) == true)
                returnToNodeID = int.Parse(nodeIDStr);
            else
                returnToNodeID = DBAccess.RunSQLReturnValInt("SELECT NodeID FROM WF_Node WHERE FK_Flow='" + flowNo + "' AND Mark='" + nodeIDStr + "'");
            return returnToNodeID;
        }
        /// <summary>
        /// 当前节点执行退回操作
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workID">工作实例WorkID</param>
        /// <param name="returnToNodeIDStr">退回到的节点,可以是节点的Mark</param>
        /// <param name="returnToEmp">退回到的接收人</param>
        /// <param name="msg">退回原因</param>
        /// <param name="isBackToThisNode">是否原路返回到当前节点</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Node_ReturnWork(string token, Int64 workID, string returnToNodeIDStr, string returnToEmp, string msg, bool isBackToThisNode)
        {

            try
            {
                //根据token登录
                Port_GenerToken(token);

                GenerWorkFlow gwf = new GenerWorkFlow(workID);
                //获取真实的NodeID. @hongyan.
                int returnToNodeID = DealNodeIDStr(returnToNodeIDStr, gwf.FK_Flow);

                if (returnToNodeID == 0)
                {
                    DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(workID);
                    if (dt.Rows.Count == 1)
                    {
                        returnToNodeID = Int32.Parse(dt.Rows[0]["No"].ToString());
                        returnToEmp = dt.Rows[0]["Rec"].ToString();

                    }
                }

                //执行退回.
                string strs = Dev2Interface.Node_ReturnWork(workID,
                    returnToNodeID, returnToEmp, msg, isBackToThisNode);
                return Return_Info(200, "当前节点执行退回操作成功", strs);
            }
            catch (Exception ex)
            {
                return Return_Info(500, "当前节点执行退回操作失败", ex.Message);
            }
        }
        /// <summary>
        /// 催办
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workIDs">工作实例的WorkID集合</param>
        /// <param name="msg">催办信息</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Flow_DoPress(string token, string workIDs, string msg)
        {
            //根据token登录
            Port_GenerToken(token);
            if (DataType.IsNullOrEmpty(workIDs))
            {
                return Return_Info(500, "执行批量催办的WorkIDs不能为空", "");
            }
            string[] strs = workIDs.Split(',');

            if (msg == null)
                msg = "需要您处理待办工作.";
            try
            {
                string info = "";
                foreach (string workidStr in strs)
                {
                    if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                        continue;

                    info += "@" + BP.WF.Dev2Interface.Flow_DoPress(int.Parse(workidStr), msg, true);
                }
                return Return_Info(200, "执行批量催办成功", info);
            }
            catch (Exception ex)
            {
                return Return_Info(500, "执行批量催办失败", ex.Message);
            }

        }
        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workIDs"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object CC_BatchCheckOver(string token, string workIDs)
        {
            //根据token登录
              Port_GenerToken(token);
           
            if (DataType.IsNullOrEmpty(workIDs))
            {
                return Return_Info(500, "执行批量审批的WorkIDs不能为空", "");
            }
            try
            {
                string str = BP.WF.Dev2Interface.Node_CC_SetCheckOverBatch(workIDs);

                return Return_Info(200, "批量审核成功", str);
            }
            catch (Exception ex)
            {
                return Return_Info(500, "批量审核失败", ex.Message);
            }
        }
        #endregion 节点方法.

        #region 流程方法.
        /// <summary>
        /// 批量删除流程
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workIDs"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Flow_BatchDeleteByFlag(string token, string workIDs)
        {
            //根据token登录
            Port_GenerToken(token);

            if (DataType.IsNullOrEmpty(workIDs))
            {
                return Return_Info(500, "批量删除的WorkIDs不能为空", "");
            }
            try
            {
                string[] strs = workIDs.Split(',');
                foreach (string workidStr in strs)
                {
                    if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                        continue;

                    string st1r = BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(Int64.Parse(workidStr), "删除", true);
                }
                return Return_Info(200, "删除成功", "");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "删除失败", ex.Message);
            }

        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workIDs">工作实例WorkIDs</param>
        /// <param name="isDeleteSubFlows">是否删除子流程</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Flow_BatchDeleteByReal(string token, string workIDs, bool isDeleteSubFlows)
        {
            if (DataType.IsNullOrEmpty(workIDs))
                return Return_Info(500, "批量删除的WorkIDs不能为空", "");

            try
            {
                //根据token登录
                Port_GenerToken(token);
            
                string[] strs = workIDs.Split(',');

                foreach (string workidStr in strs)
                {
                    if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                        continue;

                    string st1r = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(Int64.Parse(workidStr), isDeleteSubFlows);
                }
                return Return_Info(200, "删除成功", "");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "删除失败", ex.Message);
            }

        }
        /// <summary>
        /// 批量恢复逻辑删除的流程
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workIDs"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Flow_BatchDeleteByFlagAndUnDone(string token, string workIDs)
        {
            if (DataType.IsNullOrEmpty(workIDs))
                return Return_Info(500, "批量撤销删除的WorkIDs不能为空", "");

            try
            {
                //根据token登录
                Port_GenerToken(token);
            

                string[] strs = workIDs.Split(',');

                foreach (string workidStr in strs)
                {
                    if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                        continue;

                    string st1r = BP.WF.Dev2Interface.Flow_DoUnDeleteFlowByFlag(null, int.Parse(workidStr), "删除");
                }
                return Return_Info(200, "撤销删除成功", "");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "撤销删除失败", ex.Message);
            }

        }
        /// <summary>
        /// 批量撤回
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workids"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Flow_DoUnSend(string token, string workIDs)
        {
         
            //获取可以发起的列表
            string[] strs = workIDs.Split(',');
            if (DataType.IsNullOrEmpty(workIDs))
                return Return_Info(500, "批量撤回的WorkIDs不能为空", "");

            try
            {
                //根据token登录
                Port_GenerToken(token);


                string info = "";
                foreach (string workidStr in strs)
                {
                    if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                        continue;

                    info += BP.WF.Dev2Interface.Flow_DoUnSend(null, Int64.Parse(workidStr), 0, 0);
                }
                return Return_Info(200, "批量撤回成功", info);
            }
            catch (Exception ex)
            {
                return Return_Info(500, "批量撤回失败", ex.Message);
            }

        }
        /// <summary>
        /// 批量删除草稿
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workids"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Flow_DeleteDraft(string token, string workIDs)
        {
          
            string[] strs = workIDs.Split(',');
            if (DataType.IsNullOrEmpty(workIDs))
                return Return_Info(500, "批量删除草稿的WorkIDs不能为空", "");
            try
            {
                //根据token登录
                Port_GenerToken(token);


                foreach (string workidStr in strs)
                {
                    if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                        continue;

                    BP.WF.Dev2Interface.Node_DeleteDraft(Int64.Parse(workidStr));
                }
                return Return_Info(200, "删除成功", "");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "删除失败", ex.Message);
            }

        }
        /// <summary>
        /// 批量结束流程
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="workids"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Flow_DoFlowOver(string token, string workIDs)
        {
            string[] strs = workIDs.Split(',');
            if (DataType.IsNullOrEmpty(workIDs))
                return Return_Info(500, "批量结束流程的WorkIDs不能为空", "");
            try
            {
                //根据token登录
                Port_GenerToken(token);

                foreach (string workidStr in strs)
                {
                    if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                        continue;

                    BP.WF.Dev2Interface.Flow_DoFlowOver(Int64.Parse(workidStr), "批量结束", 1);
                }
                return Return_Info(200, "执行成功", "");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "执行失败", ex.Message);
            }

        }
        #endregion 节点方法.

        #region 批处理相关
        /// <summary>
        /// 批量处理
        /// </summary>
        /// <param name="token">密钥</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Batch_Init(string token)
        {
            //根据token登录
            Port_GenerToken(token);

            try
            {
                var handle = new BP.WF.HttpHandler.WF();
                string str = handle.Batch_Init();
                return Return_Info(200, "执行成功", str);
            }
            catch (Exception ex)
            {
                return Return_Info(500, "执行失败", ex.Message);
            }

        }
        /// <summary>
        /// 根据NodeID获取节点信息
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object En_Node(string token, int nodeID)
        {

            try
            {
                //根据token登录
                Port_GenerToken(token);
                Node node = new Node(nodeID);

                return Return_Info(200, "根据NodeID获取节点信息成功", node.ToJson());
            }
            catch (Exception ex)
            {
                return Return_Info(500, "执行失败", ex.Message);
            }

        }
        /// <summary>
        /// 根据流程编号获取流程信息
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="flowNo"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object En_Flow(string token, string flowNo)
        {
            try
            {
                //根据token登录
                Port_GenerToken(token);

                Flow flow = new Flow(flowNo);
                return Return_Info(200, "根据流程编号获取流程信息成功", flow.ToJson());
            }
            catch (Exception ex)
            {
                return Return_Info(500, "根据流程编号获取流程信息失败", ex.Message);
            }

        }
        /// <summary>
        /// 根据流程编号获取流程信息
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object WorkCheckModel_Init(string token, int nodeID)
        {
           

            try
            {
                //根据token登录
                Port_GenerToken(token);

                DataSet ds = new DataSet();
                //获取节点信息
                BP.WF.Node nd = new BP.WF.Node(nodeID);
                Flow fl = nd.HisFlow;
                ds.Tables.Add(nd.ToDataTableField("WF_Node"));
                string sql = "";
                if (nd.IsSubThread == true)
                {
                    sql = "SELECT a.*, b.Starter,b.StarterName,b.ADT,b.WorkID FROM " + fl.PTable
                              + " a , WF_EmpWorks b WHERE a.OID=B.FID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                              + " AND b.FK_Emp='" + WebUser.No + "'";
                }
                else
                {
                    sql = "SELECT a.*, b.Starter,b.StarterName,b.ADT,b.WorkID FROM " + fl.PTable
                            + " a , WF_EmpWorks b WHERE a.OID=B.WorkID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                            + " AND b.FK_Emp='" + WebUser.No + "'";
                }

                //获取待审批的流程信息集合
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Works";
                ds.Tables.Add(dt);
                //获取按钮权限
                BtnLab btnLab = new BtnLab(nodeID);
                ds.Tables.Add(btnLab.ToDataTableField("Sys_BtnLab"));
                //获取字段属性
                MapAttrs attrs = new MapAttrs("ND" + nodeID);
                //获取实际中需要展示的列.
                string batchParas = nd.GetParaString("BatchFields");
                MapAttrs realAttr = new MapAttrs();
                if (DataType.IsNullOrEmpty(batchParas) == false)
                {
                    string[] strs = batchParas.Split(',');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str)
                            || str.Contains("@PFlowNo") == true)
                            continue;

                        foreach (MapAttr attr in attrs)
                        {
                            if (str != attr.KeyOfEn)
                                continue;

                            realAttr.AddEntity(attr);
                        }
                    }
                }
                ds.Tables.Add(realAttr.ToDataTableField("Sys_MapAttr"));
                return Return_Info(200, "根据节点编号获取流程信息成功", BP.Tools.Json.ToJson(ds));
            }
            catch (Exception ex)
            {
                return Return_Info(500, "根据节点编号获取流程信息失败", ex.Message);
            }

        }

        [HttpGet, HttpPost]
        public Object Batch_InitDDL(string token, int nodeID)
        {

            try
            {
                //根据token登录
                Port_GenerToken(token);

                GenerWorkFlow gwf = new GenerWorkFlow();
                Node nd = new Node(nodeID);
                gwf.TodoEmps = WebUser.No + ",";
                DataTable mydt = BP.WF.Dev2Interface.Node_GenerDTOfToNodes(gwf, nd);
                return Return_Info(200, "执行成功", BP.Tools.Json.ToJson(mydt));
            }
            catch (Exception ex)
            {
                return Return_Info(500, "执行失败", ex.Message);
            }

        }
        [HttpGet, HttpPost]
        public Object WorkCheckModel_Send(string token, string flowNo)
        {

            try
            {
                //根据token登录
                Port_GenerToken(token);
                var handle = new BP.WF.HttpHandler.WF_WorkOpt_Batch();
                string str = handle.WorkCheckModel_Send();
                return Return_Info(200, "执行成功", str);
            }
            catch (Exception ex)
            {
                return Return_Info(500, "执行失败", ex.Message);
            }

        }

        [HttpGet, HttpPost]
        public Object Batch_Delete(string token, string workIDs)
        {
        

            try
            {
                //根据token登录
                Port_GenerToken(token);

                if (DataType.IsNullOrEmpty(workIDs) == true)
                    return Return_Info(500, "批量删除成功", "没有选择需要处理的工作");
                string msg = "";
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.RetrieveIn("WorkID", workIDs);
                foreach (GenerWorkFlow gwf in gwfs)
                {
                    msg += "@对工作(" + gwf.Title + ")处理情况如下。<br>";
                    string mes = BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(gwf.WorkID, "批量删除", true);
                    msg += mes;
                    msg += "<hr>";
                }
                return Return_Info(200, "批量删除成功", msg);
            }
            catch (Exception ex)
            {
                return Return_Info(500, "执行失败", ex.Message);
            }

        }
        #endregion 批处理相关

        #region 其他方法.
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="dtFrom">日期从</param>
        /// <param name="dtTo">日期到</param>
        /// <param name="scop">范围</param>
        /// <param name="pageIdx">分页</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Search_Init(string token, string key, string dtFrom, string dtTo, string scop, int pageIdx)
        {
           

            try
            {
                //根据token登录
                Port_GenerToken(token);

                GenerWorkFlows gwfs = new GenerWorkFlows();
                //创建查询对象.
                QueryObject qo = new QueryObject(gwfs);
                if (DataType.IsNullOrEmpty(key) == false)
                {
                    qo.AddWhere(GenerWorkFlowAttr.Title, " LIKE ", "%" + key + "%");
                    qo.addAnd();
                }

                //我参与的.
                if (scop.Equals("0") == true)
                    qo.AddWhere(GenerWorkFlowAttr.Emps, "LIKE", "%@" + WebUser.No + ",%");

                //我发起的.
                if (scop.Equals("1") == true)
                    qo.AddWhere(GenerWorkFlowAttr.Starter, "=", WebUser.No);

                //我部门发起的.
                if (scop.Equals("2") == true)
                    qo.AddWhere(GenerWorkFlowAttr.FK_Dept, "=", WebUser.FK_Dept);


                //任何一个为空.
                if (DataType.IsNullOrEmpty(dtFrom) == true || DataType.IsNullOrEmpty(dtTo) == true)
                {

                }
                else
                {
                    qo.addAnd();
                    qo.AddWhere(GenerWorkFlowAttr.RDT, ">=", dtFrom);
                    qo.addAnd();
                    qo.AddWhere(GenerWorkFlowAttr.RDT, "<=", dtTo);
                }

                var count = qo.GetCount(); //获得总数.

                qo.DoQuery("WorkID", 20, pageIdx);
                //   qo.DoQuery(); // "WorkID", 20, pageIdx);


                DataTable dt = gwfs.ToDataTableField("gwls");

                //创建容器.
                DataSet ds = new DataSet();
                ds.Tables.Add(dt); //增加查询对象.

                //增加数量.
                DataTable mydt = new DataTable();
                mydt.TableName = "count";
                mydt.Columns.Add("CC");
                DataRow dr = mydt.NewRow();
                dr[0] = count.ToString(); //把数量加进去.
                mydt.Rows.Add(dr);
                ds.Tables.Add(mydt);
                return Return_Info(200, "查询成功", BP.Tools.Json.ToJson(ds));
            }
            catch (Exception ex)
            {
                return Return_Info(500, "查询失败", ex.Message);
            }
        }
        private void Port_GenerToken(string token)
        {
            BP.WF.Dev2Interface.Port_LoginByToken(token);
        }

        private HttpResponseMessage ReturnMessage(string message)
        {


            return new HttpResponseMessage { Content = new StringContent(message, System.Text.Encoding.GetEncoding("UTF-8"), "application/json") };

        }
        #endregion 其他方法.


    }
}
