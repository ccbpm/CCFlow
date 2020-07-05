using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.GPM.WeiXin;
using BP.GPM.DTalk.DINGTalk;
using BP.GPM.DTalk;
using System.Collections;
using BP.WF;
using BP.WF.Data;

namespace BP.GPM
{
    /// <summary>
    /// 权限调用API
    /// </summary>
    public class Dev2Interface
    {
        #region 菜单权限
        #endregion 菜单权限

        #region 登陆接口
        /// <summary>
        /// 用户登陆,此方法是在开发者校验好用户名与密码后执行
        /// </summary>
        /// <param name="userNo">用户名</param>
        /// <param name="SID">安全ID,请参考流程设计器操作手册</param>
        public static void Port_Login(string userNo, string sid)
        {
            string sql = "SELECT SID FROM Port_Emp WHERE No='" + userNo + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("用户不存在或者SID错误。");

            if (dt.Rows[0]["SID"].ToString() != sid)
                throw new Exception("用户不存在或者SID错误。");

            BP.Port.Emp emp = new BP.Port.Emp(userNo);
            WebUser.SignInOfGener(emp);
            return;
        }
        /// <summary>
        /// 用户登陆,此方法是在开发者校验好用户名与密码后执行
        /// </summary>
        /// <param name="userNo">用户名</param>
        public static void Port_Login(string userNo)
        {
            BP.Port.Emp emp = new BP.Port.Emp(userNo);
            WebUser.SignInOfGener(emp);
            return;
        }
        /// <summary>
        /// 注销当前登录
        /// </summary>
        public static void Port_SigOut()
        {
            WebUser.Exit();
        }
        /// <summary>
        /// 获取未读的消息
        /// 用于消息提醒.
        /// </summary>
        /// <param name="userNo">用户ID</param>
        public static string Port_SMSInfo(string userNo)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT MyPK, EmailTitle  FROM sys_sms WHERE SendToEmpID=" + SystemConfig.AppCenterDBVarStr + "SendToEmpID AND IsAlert=0";
            ps.Add("SendToEmpID", userNo);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            string strs = "";
            foreach (DataRow dr in dt.Rows)
            {
                strs += "@" + dr[0] + "=" + dr[1].ToString();
            }
            ps = new Paras();
            ps.SQL = "UPDATE  sys_sms SET IsAlert=1 WHERE  SendToEmpID=" + SystemConfig.AppCenterDBVarStr + "SendToEmpID AND IsAlert=0";
            ps.Add("SendToEmpID", userNo);
            DBAccess.RunSQL(ps);
            return strs;
        }
        #endregion 登陆接口

        #region GPM接口
        /// <summary>
        /// 获取一个操作人员对于一个系统的权限
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="app">系统编号</param>
        /// <returns>结果集</returns>
        public static DataTable DB_Menus(string userNo, string app)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM GPM_EmpMenu WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND FK_App=" + SystemConfig.AppCenterDBVarStr + "FK_App ";
            ps.Add("FK_Emp", userNo);
            ps.Add("FK_App", app);
            return DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取一个操作人员对此应用可以访问的系统
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <returns>结果集</returns>
        public static DataTable DB_Apps(string userNo)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM GPM_EmpApp WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp ";
            ps.Add("FK_Emp", userNo);
            return DBAccess.RunSQLReturnTable(ps);
        }
        #endregion GPM接口

        /// <summary>
        /// 推送消息到微信
        /// </summary>
        /// <param name="WorkID">WorkID</param>
        /// <param name="sender">发送人</param>
        public static MessageErrorModel PushMessageToTelByWeiXin(long WorkID, string sender)
        {
            //企业应用必须存在
            string agentId = BP.Sys.SystemConfig.WX_AgentID ?? null;
            if (agentId != null)
            {
                //获取 AccessToken
                string accessToken = BP.GPM.WeiXin.WeiXinEntity.getAccessToken();

                //当前业务
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = WorkID;
                gwf.RetrieveFromDBSources();
                //接收人
                Monitors empWorks = new Monitors();
                QueryObject obj = new QueryObject(empWorks);
                obj.AddWhere(MonitorAttr.WorkID, WorkID);
                obj.addOr();
                obj.AddWhere(MonitorAttr.FID, WorkID);
                obj.DoQuery();

                //发送给多人的消息格式   zhangsan|lisi|wangwu
                //此处根据手机号作为推送人的帐号，便于关联
                string toUsers = "";
                foreach (Monitor empWork in empWorks)
                {
                    if (toUsers.Length > 0)
                        toUsers += "|";
                    Emp emp = new Emp(empWork.FK_Emp);
                    toUsers += emp.Tel;
                }
                if (toUsers.Length == 0)
                    return null;

                //消息样式为图文连接
                News_Articles newArticle = new News_Articles();

                //设置消息标题
                newArticle.title = "待办：" + gwf.Title;

                //设置消息内容主体
                string msgConten = "业务名称：" + gwf.FlowName + "\n";
                msgConten += "申请人：" + gwf.StarterName + "\n";
                msgConten += "申请部门：" + gwf.DeptName + "\n";
                msgConten += "当前步骤：" + gwf.NodeName + "\n";
                msgConten += "上一步处理人：" + sender + "\n";

                newArticle.description = msgConten;

                //设置图片连接
                string New_Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + BP.Sys.SystemConfig.WX_CorpID
                    + "&redirect_uri=" + BP.Sys.SystemConfig.WX_MessageUrl + "/CCMobile/action.aspx&response_type=code&scope=snsapi_base&state=TodoList#wechat_redirect";

                newArticle.url = New_Url;

                //http://discuz.comli.com/weixin/weather/icon/cartoon.jpg
                newArticle.picurl = BP.Sys.SystemConfig.WX_MessageUrl + "/DataUser/ICON/CCBPM.png";

                //加入消息
                WX_Msg_News wxMsg = new WX_Msg_News();
                wxMsg.Access_Token = accessToken;
                wxMsg.agentid = BP.Sys.SystemConfig.WX_AgentID;
                wxMsg.touser = toUsers;
                wxMsg.articles.Add(newArticle);
                //执行发送
                return BP.GPM.WeiXin.Glo.PostMsgOfNews(wxMsg);
            }
            return null;
        }
        /// <summary>
        /// 推送消息到钉钉
        /// </summary>
        /// <param name="msgType">消息类型</param>
        /// <param name="WorkID">WorkID</param>
        /// <param name="sender">发送人</param>
        public static Ding_Post_ReturnVal PushMessageToTelByDingDing(DingMsgType msgType, long WorkID, string sender)
        {
            //当前业务
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = WorkID;
            gwf.RetrieveFromDBSources();
            //获取接收人
            Monitors empWorks = new Monitors();
            QueryObject obj = new QueryObject(empWorks);
            obj.AddWhere(MonitorAttr.WorkID, WorkID);
            obj.addOr();
            obj.AddWhere(MonitorAttr.FID, WorkID);
            obj.DoQuery();


            //结束不发送消息
            if (gwf.WFState == WFState.Complete)
                return null;

            string toUsers = "";
            foreach (Monitor empWork in empWorks)
            {
                if (toUsers.Length > 0)
                    toUsers += "|";
                Emp emp = new Emp(empWork.FK_Emp);
                toUsers += emp.Tel;
            }
            if (toUsers.Length == 0)
                return null;

            switch (msgType)
            {
                //文本类型
                case DingMsgType.text:
                    Ding_Msg_Text msgText = new Ding_Msg_Text();
                    msgText.Access_Token = DingDing.getAccessToken();
                    msgText.agentid = SystemConfig.Ding_AgentID;
                    msgText.touser = toUsers;
                    msgText.content = gwf.Title + "\n发送人：" + sender + "\n时间：" + BP.DA.DataType.CurrentDataTimeCNOfShort;
                    return DingTalk_Message.Msg_AgentText_Send(msgText);
                //连接类型
                case DingMsgType.link:
                    Ding_Msg_Link msgLink = new Ding_Msg_Link();
                    msgLink.Access_Token = DingDing.getAccessToken();
                    msgLink.touser = toUsers;
                    msgLink.agentid = SystemConfig.Ding_AgentID;
                    msgLink.messageUrl = SystemConfig.Ding_MessageUrl + "/CCMobile/login.aspx";
                    msgLink.picUrl = "@lALOACZwe2Rk";
                    msgLink.title = gwf.Title;
                    msgLink.text = "发送人：" + sender + "\n时间：" + BP.DA.DataType.CurrentDataTimeCNOfShort;
                    return DingTalk_Message.Msg_AgentLink_Send(msgLink);
                //工作消息类型
                case DingMsgType.OA:
                    string[] users = toUsers.Split('|');
                    string faildSend = "";
                    Ding_Post_ReturnVal postVal = null;
                    foreach (string user in users)
                    {
                        Ding_Msg_OA msgOA = new Ding_Msg_OA();
                        msgOA.Access_Token = DingDing.getAccessToken();
                        msgOA.agentid = SystemConfig.Ding_AgentID;
                        msgOA.touser = user;
                        msgOA.messageUrl = SystemConfig.Ding_MessageUrl + "/CCMobile/DingAction.aspx?ActionFrom=message&UserID=" + user
                            + "&ActionType=ToDo&FK_Flow=" + gwf.FK_Flow + "&FK_Node=" + gwf.FK_Node
                            + "&WorkID=" + WorkID + "&FID=" + gwf.FID;
                        //00是完全透明，ff是完全不透明，比较适中的透明度值是 1e
                        msgOA.head_bgcolor = "FFBBBBBB";
                        msgOA.head_text = "审批";
                        msgOA.body_title = gwf.Title;
                        Hashtable hs = new Hashtable();
                        hs.Add("流程名", gwf.FlowName);
                        hs.Add("当前节点", gwf.NodeName);
                        hs.Add("申请人", gwf.StarterName);
                        hs.Add("申请时间", gwf.RDT);
                        msgOA.body_form = hs;
                        msgOA.body_author = sender;
                        postVal = DingTalk_Message.Msg_OAText_Send(msgOA);
                        if (postVal.errcode != "0")
                        {
                            if (faildSend.Length > 0)
                                faildSend += ",";
                            faildSend += user;
                        }
                    }
                    //有失败消息
                    if (faildSend.Length > 0)
                    {
                        postVal.errcode = "500";
                        postVal.errmsg = faildSend + "消息发送失败";
                    }
                    return postVal;
            }
            return null;
        }

    }
}
