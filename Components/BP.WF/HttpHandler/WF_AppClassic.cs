using System;
using System.Collections;
using System.Data;
using System.Text;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF.Data;
using LitJson;
using System.Net;
using System.IO;
using BP.Difference;



namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_AppClassic : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_AppClassic()
        {
        }


        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.

        #region xxx 界面 .

        #endregion xxx 界面方法.

        /// <summary>
        /// 蓝信登陆
        /// </summary>
        /// <returns></returns>
        public string LanXin_Login()
        {
            string code = GetRequestVal("code");

            if (DataType.IsNullOrEmpty(WebUser.Token) == false)
            {
                //刷新token
                string urlr = "http://xjtyjt.e.lanxin.cn:11180//sns/oauth2/refresh_token?refresh_token=" + WebUser.Token + "&appid=100243&grant_type=refresh_token";
                string resultr = HttpPostConnect(urlr, "");
                JsonData jdr = JsonMapper.ToObject(resultr);
                resultr = jdr["errcode"].ToString();
                if (resultr == "0")
                {
                    WebUser.Token = jdr["access_token"].ToString();
                }
                return WebUser.No;
            }


            //获取Token
            string url = "http://xjtyjt.e.lanxin.cn:11180/sns/oauth2/access_token?code=" + code + "&appid=100243&grant_type=authorization_code";
            string result = HttpPostConnect(url, "");
            JsonData jd = JsonMapper.ToObject(result);
            result = jd["errcode"].ToString();
            if (result != "0")
            {
                return "err@" + jd["errmsg"].ToString();
            }
            string access_token = jd["access_token"].ToString();
            string openId = jd["openid"].ToString();

            //获取用户信息
            url = "http://xjtyjt.e.lanxin.cn:11180/sns/userinfo?access_token=" + access_token + "&mobile=" + openId;
            result = HttpPostConnect(url, "");
            jd = JsonMapper.ToObject(result);
            result = jd["errcode"].ToString();
            if (result != "0")
            {
                return "err@" + jd["errmsg"].ToString();
            }
            string userNo = jd["openOrgMemberList"][0]["serialNumber"].ToString();
            string tel = jd["openOrgMemberList"][0]["mobile"].ToString();

            /**单点登陆*/
            Paras ps = new Paras();
            ps.SQL = "SELECT No FROM Port_Emp WHERE No=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "No and Tel=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Tel";
            ps.Add("No", userNo);
            ps.Add("Tel", tel);
            string No = DBAccess.RunSQLReturnString(ps);
            if (DataType.IsNullOrEmpty(No))
                return "err@用户信息不正确，请联系管理员";

            BP.WF.Dev2Interface.Port_Login(userNo);
            WebUser.Token = access_token;
            result = jd["errcode"].ToString();
            return userNo;
        }
        /// <summary>
        /// httppost方式发送数据
        /// </summary>
        /// <param name="url">要提交的url</param>
        /// <param name="postDataStr"></param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="encode">text code.</param>
        /// <returns>成功：返回读取内容；失败：0</returns>
        public static string HttpPostConnect(string serverUrl, string postData)
        {
            var dataArray = Encoding.UTF8.GetBytes(postData);
            //创建请求
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUrl);
            request.Method = "POST";
            request.ContentLength = dataArray.Length;
            //设置上传服务的数据格式  设置之后不好使
            //request.ContentType = "application/json";
            //请求的身份验证信息为默认
            request.Credentials = CredentialCache.DefaultCredentials;
            //请求超时时间
            request.Timeout = 10000;
            //创建输入流
            Stream dataStream;
            try
            {
                dataStream = request.GetRequestStream();
            }
            catch (Exception)
            {
                return "0";//连接服务器失败
            }
            //发送请求
            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();
            //读取返回消息
            string res;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {

                return "0";//连接服务器失败
            }
            return res;
        }
        /// <summary>
        /// 初始化Home
        /// </summary>
        /// <returns></returns>
        public string Home_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("UserNo", BP.Web.WebUser.No);
            ht.Add("UserName", BP.Web.WebUser.Name);

            //系统名称.
            ht.Add("SysName", BP.Difference.SystemConfig.SysName);
            ht.Add("CustomerName", BP.Difference.SystemConfig.CustomerName);

            ht.Add("Todolist_EmpWorks", BP.WF.Dev2Interface.Todolist_EmpWorks);
            ht.Add("Todolist_Runing", BP.WF.Dev2Interface.Todolist_Runing);
            ht.Add("Todolist_Sharing", BP.WF.Dev2Interface.Todolist_Sharing);
            ht.Add("Todolist_CCWorks", BP.WF.Dev2Interface.Todolist_CCWorks);
            ht.Add("Todolist_Apply", BP.WF.Dev2Interface.Todolist_Apply); //申请下来的任务个数.
            ht.Add("Todolist_Draft", BP.WF.Dev2Interface.Todolist_Draft); //草稿数量.
            ht.Add("Todolist_Complete", BP.WF.Dev2Interface.Todolist_Complete); //完成数量.
            ht.Add("UserDeptName", WebUser.FK_DeptName);

            //我发起
            MyStartFlows myStartFlows = new MyStartFlows();
            QueryObject obj = new QueryObject(myStartFlows);
            obj.AddWhere(MyStartFlowAttr.Starter, WebUser.No);
            obj.addAnd();
            //运行中\已完成\挂起\退回\转发\加签\批处理\
            obj.addLeftBracket();
            obj.AddWhere("WFState=2 or WFState=3 or WFState=4 or WFState=5 or WFState=6 or WFState=8 or WFState=10");
            obj.addRightBracket();
            obj.DoQuery();
            ht.Add("Todolist_MyStartFlow", myStartFlows.Count);

            //我参与
            MyJoinFlows myFlows = new MyJoinFlows();
            obj = new QueryObject(myFlows);
            obj.AddWhere("Emps like '%" + WebUser.No + "%'");
            obj.DoQuery();
            ht.Add("Todolist_MyFlow", myFlows.Count);

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        public string Index_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("Todolist_Runing", BP.WF.Dev2Interface.Todolist_Runing); //运行中.
            ht.Add("Todolist_EmpWorks", BP.WF.Dev2Interface.Todolist_EmpWorks); //待办
            ht.Add("Todolist_CCWorks", BP.WF.Dev2Interface.Todolist_CCWorks); //抄送.

            //本周.
            ht.Add("TodayNum", BP.WF.Dev2Interface.Todolist_CCWorks); //抄送.

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }

        #region 登录界面.
        public string Portal_Login()
        {
            string userNo = this.GetRequestVal("UserNo");

            try
            {
                BP.Port.Emp emp = new Emp(userNo);

                BP.WF.Dev2Interface.Port_Login(emp.UserID);
                return ".";
            }
            catch (Exception ex)
            {
                return "err@用户[" + userNo + "]登录失败." + ex.Message;
            }

        }
        /// <summary>
        /// 登录.
        /// </summary>
        /// <returns></returns>
        public string Login_Submit()
        {
            try
            {
                string userNo = this.GetRequestVal("TB_No");
                if (userNo == null)
                    userNo = this.GetRequestVal("TB_UserNo");

                string pass = this.GetRequestVal("TB_PW");
                if (pass == null)
                    pass = this.GetRequestVal("TB_Pass");

                BP.Port.Emp emp = new Emp();
                emp.UserID = userNo;
                //是否存在用户
                bool isExist = emp.RetrieveFromDBSources() == 0 ? false : true;
                if (isExist == false && DBAccess.IsExitsTableCol("Port_Emp", "NikeName") == true)
                {
                    /*如果包含昵称列,就检查昵称是否存在.*/
                    Paras ps = new Paras();
                    ps.SQL = "SELECT No FROM Port_Emp WHERE NikeName=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "NikeName";
                    ps.Add("NikeName", userNo);
                    string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                    if (DataType.IsNullOrEmpty(no) == false)
                    {
                        emp.No = no;
                        if (emp.RetrieveFromDBSources() != 0)
                            isExist = true;
                    }
                }
                if (isExist == false && DBAccess.IsExitsTableCol("Port_Emp", "Tel") == true)
                {
                    /*如果包含Name列,就检查Name是否存在.*/
                    Paras ps = new Paras();
                    ps.SQL = "SELECT No FROM Port_Emp WHERE Tel=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Tel";
                    ps.Add("Tel", userNo);
                    string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                    if (DataType.IsNullOrEmpty(no) == false)
                    {
                        emp.No = no;
                        if (emp.RetrieveFromDBSources() != 0)
                            isExist = true;
                    }
                }
                if (isExist == false && DBAccess.IsExitsTableCol("Port_Emp", "Email") == true)
                {
                    /*如果包含Name列,就检查Name是否存在.*/
                    Paras ps = new Paras();
                    ps.SQL = "SELECT No FROM Port_Emp WHERE Email=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Email";
                    ps.Add("Email", userNo);
                    string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                    if (DataType.IsNullOrEmpty(no) == false)
                    {
                        emp.No = no;
                        if (emp.RetrieveFromDBSources() != 0)
                            isExist = true;
                    }
                }
                if (isExist == false)
                {
                    return "err@用户名不存在.";
                }

                if (emp.CheckPass(pass) == false)
                    return "err@用户名或者密码错误.";

                //调用登录方法.
                BP.WF.Dev2Interface.Port_Login(emp.UserID);
                BP.WF.Dev2Interface.Port_GenerToken();

                return "登陆成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }


        /// <summary>
        /// 执行登录
        /// </summary>
        /// <returns></returns>
        public string Login_Init()
        {
            string doType = GetRequestVal("LoginType");
            if (DataType.IsNullOrEmpty(doType) == false && doType.Equals("Out") == true)
            {
                //清空cookie
                WebUser.Exit();
            }


            if (this.DoWhat != null && this.DoWhat.Equals("Login") == true)
            {
                //调用登录方法.
                BP.WF.Dev2Interface.Port_Login(this.UserNo, this.SID);
                return "url@Home.htm?UserNo=" + this.UserNo;
                //this.Login_Submit();
                //return;
            }

            Hashtable ht = new Hashtable();
            ht.Add("SysName", BP.Difference.SystemConfig.SysName);
            ht.Add("ServiceTel", BP.Difference.SystemConfig.ServiceTel);
            ht.Add("CustomerName", BP.Difference.SystemConfig.CustomerName);
            if (WebUser.NoOfRel == null)
            {
                ht.Add("UserNo", "");
                ht.Add("UserName", "");
            }
            else
            {
                ht.Add("UserNo", WebUser.No);

                string name = WebUser.Name;

                if (DataType.IsNullOrEmpty(name) == true)
                    ht.Add("UserName", WebUser.No);
                else
                    ht.Add("UserName", name);
            }

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        #endregion 登录界面.

        #region Welcome.htm 欢迎页面.
        public string Welcome_Init()
        {

            Hashtable ht = new Hashtable();
            // 待办.
            ht.Add("Todlist", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) as Num FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Emp='" + BP.Web.WebUser.No + "'")); //流程数

            //发起.
            ht.Add("Start", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState > 1  AND Starter='" + BP.Web.WebUser.No + "'")); //实例数.

            //逾期.
            ht.Add("OverTime", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=3 AND WFSta=1 ")); //实例数.

            //退回数
            ht.Add("ReturnNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState = 5  AND TodoEmps LIKE '%" + BP.Web.WebUser.No + ",%'"));

            //草稿.
            ht.Add("Darft", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=1  AND Starter='" + BP.Web.WebUser.No + "'"));

            //运行中.
            ht.Add("Runing", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) as Num FROM WF_GenerWorkerlist WHERE IsPass!=0 AND FK_Emp='" + BP.Web.WebUser.No + "'"));

            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 流程监控折线图数据获取
        /// </summary>
        /// <returns></returns>
        public string Welcome_EchartDataSet()
        {
            DataSet ds = new DataSet();

            string whereStr = "";
            string whereStrPuls = "";

            if (Glo.CCBPMRunModel != CCBPMRunModel.GroupInc)
            {
                whereStr += " WHERE OrgNo = '" + WebUser.OrgNo + "'";
                whereStrPuls += " AND OrgNo = '" + WebUser.OrgNo + "'";
            }

            #region 完成的流程-按月分析
            //按期完成
            string sql = "SELECT FK_NY, count(WorkID) as Num FROM WF_GenerWorkFlow WHERE WFState=3 AND SendDT<=SDTOfNode And WFSta=1 " + whereStrPuls + " GROUP BY FK_NY ";
            DataTable ComplateFlowsByNY = DBAccess.RunSQLReturnTable(sql);
            ComplateFlowsByNY.TableName = "ComplateFlowsByNY";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                ComplateFlowsByNY.Columns[0].ColumnName = "FK_NY";
                ComplateFlowsByNY.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(ComplateFlowsByNY);

            //逾期完成
            sql = "SELECT FK_NY, count(WorkID) as Num FROM WF_GenerWorkFlow WHERE WFState=3 AND SendDT>SDTOfNode And WFSta=1 " + whereStrPuls + " GROUP BY FK_NY ";
            DataTable OverComplateFlowsByNY = DBAccess.RunSQLReturnTable(sql);
            OverComplateFlowsByNY.TableName = "OverComplateFlowsByNY";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                OverComplateFlowsByNY.Columns[0].ColumnName = "FK_NY";
                OverComplateFlowsByNY.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(OverComplateFlowsByNY);
            #endregion 完成的流程-按月分析

            #region 运行中的流程
            //按部门
            //1.全部待办
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B WHERE A.FK_Dept=B.No GROUP BY B.Name";
            else
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B WHERE A.FK_Dept=B.No AND B.OrgNo='" + WebUser.OrgNo + "' GROUP BY B.Name";
            DataTable TodoListAllByDept = DBAccess.RunSQLReturnTable(sql);
            TodoListAllByDept.TableName = "TodoListAllByDept";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListAllByDept.Columns[0].ColumnName = "Name";
                TodoListAllByDept.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListAllByDept);

            //2.退回的数据
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID AND C.WFState=5 GROUP BY B.Name";
            else
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID AND C.WFState=5 AND B.OrgNo='" + WebUser.OrgNo + "' GROUP BY B.Name";
            DataTable TodoListReturnByDept = DBAccess.RunSQLReturnTable(sql);
            TodoListReturnByDept.TableName = "TodoListReturnByDept";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListReturnByDept.Columns[0].ColumnName = "Name";
                TodoListReturnByDept.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListReturnByDept);

            //3.逾期的数据
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT B.Name, count(DISTINCT C.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID  and STR_TO_DATE(A.SDT,'%Y-%m-%d %H:%i') <STR_TO_DATE(C.SDTOfNode,'%Y-%m-%d %H:%i') GROUP BY B.Name";

            }
            else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
            {
                sql = "SELECT  B.Name, count(DISTINCT C.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID  and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(TO_DATE(C.SDTOfNode, 'yyyy-mm-dd hh24:mi:ss') - TO_DATE(A.SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 GROUP BY B.Name ";
                sql += "UNION SELECT  B.Name, count(DISTINCT C.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (TO_DATE(SDTOfNode, 'yyyy-mm-dd') - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 GROUP BY B.Name";
            }
            else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                sql = "SELECT  B.Name, count(DISTINCT C.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID  and to_timestamp(CASE WHEN A.SDT='无' THEN '' ELSE A.SDT END, 'yyyy-mm-dd hh24:MI:SS') < to_timestamp(CASE WHEN C.SDTOfNode='无' THEN '' ELSE C.SDTOfNode END, 'yyyy-mm-dd hh24:MI:SS') GROUP BY B.Name";
            }
            else
            {
                sql = "SELECT  B.Name, count(DISTINCT C.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID  and convert(varchar(100),A.SDT,120) < CONVERT(varchar(100), C.SDTOfNode, 120) GROUP BY B.Name";
            }

            DataTable TodoListOverTByDept = DBAccess.RunSQLReturnTable(sql);
            TodoListOverTByDept.TableName = "TodoListOverTByDept";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListOverTByDept.Columns[0].ColumnName = "Name";
                TodoListOverTByDept.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListOverTByDept);

            //4.预警的数据
            //按流程

            //1.全部待办
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_Flow B WHERE A.FK_Flow=B.No GROUP BY B.Name";
            else
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_Flow B WHERE A.FK_Flow=B.No AND B.OrgNo='" + WebUser.OrgNo + "' GROUP BY B.Name";
            DataTable TodoListAllByFlow = DBAccess.RunSQLReturnTable(sql);
            TodoListAllByFlow.TableName = "TodoListAllByFlow";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListAllByFlow.Columns[0].ColumnName = "Name";
                TodoListAllByFlow.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListAllByFlow);

            //2.退回的数据
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID AND B.WFState=5 GROUP BY B.FlowName";
            else
                sql = "SELECT B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID AND B.WFState=5 AND B.OrgNo='" + WebUser.OrgNo + "' GROUP BY B.FlowName";
            DataTable TodoListReturnByFlow = DBAccess.RunSQLReturnTable(sql);
            TodoListReturnByFlow.TableName = "TodoListReturnByFlow";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListReturnByFlow.Columns[0].ColumnName = "Name";
                TodoListReturnByFlow.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListReturnByFlow);

            //3.逾期的数据
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID  and STR_TO_DATE(A.SDT,'%Y-%m-%d %H:%i') <STR_TO_DATE(B.SDTOfNode,'%Y-%m-%d %H:%i') GROUP BY B.FlowName";

            }
            else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
            {
                sql = "SELECT  B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID  and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(TO_DATE(B.SDTOfNode, 'yyyy-mm-dd hh24:mi:ss') - TO_DATE(A.SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 GROUP BY B.FlowName ";
                sql += "UNION SELECT  B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (TO_DATE(B.SDTOfNode, 'yyyy-mm-dd') - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 GROUP BY B.FlowName";
            }
            else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                sql = "SELECT B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID  and to_timestamp(CASE WHEN A.SDT='无' THEN '' ELSE A.SDT END, 'yyyy-mm-dd hh24:MI:SS') < to_timestamp(CASE WHEN B.SDTOfNode='无' THEN '' ELSE B.SDTOfNode END, 'yyyy-mm-dd hh24:MI:SS') GROUP BY B.FlowName";
            }
            else
            {
                sql = "SELECT B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID  and convert(varchar(100),A.SDT,120) < CONVERT(varchar(100), B.SDTOfNode, 120) GROUP BY B.FlowName";
            }

            DataTable TodoListOverTByFlow = DBAccess.RunSQLReturnTable(sql);
            TodoListOverTByFlow.TableName = "TodoListOverTByFlow";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListOverTByFlow.Columns[0].ColumnName = "Name";
                TodoListOverTByFlow.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListOverTByFlow);


            //按人员(仅限一个部门中的人员）
            //获取当前人员所在部门的所有人员
            sql = "SELECT A.No,A.Name From Port_Emp A,Port_DeptEmp B Where A.No=B.FK_Emp AND B.FK_Dept='" + WebUser.FK_Dept + "' order By A.Idx";
            DataTable Emps = DBAccess.RunSQLReturnTable(sql);
            Emps.TableName = "Emps";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                Emps.Columns[0].ColumnName = "No";
                Emps.Columns[1].ColumnName = "Name";
            }
            ds.Tables.Add(Emps);

            //1.全部待办
            sql = "SELECT FK_EmpText AS Name, count(WorkID) as Num FROM WF_GenerWorkerlist WHERE FK_Dept='" + WebUser.FK_Dept + "' GROUP BY FK_EmpText";
            DataTable TodoListAllByEmp = DBAccess.RunSQLReturnTable(sql);
            TodoListAllByEmp.TableName = "TodoListAllByEmp";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListAllByEmp.Columns[0].ColumnName = "Name";
                TodoListAllByEmp.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListAllByEmp);

            //2.退回的数据
            sql = "SELECT A.FK_EmpText AS Name, count( A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE  A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID AND B.WFState=5 GROUP BY A.FK_EmpText";
            DataTable TodoListReturnByEmp = DBAccess.RunSQLReturnTable(sql);
            TodoListReturnByEmp.TableName = "TodoListReturnByEmp";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListReturnByEmp.Columns[0].ColumnName = "Name";
                TodoListReturnByEmp.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListReturnByEmp);

            //3.逾期的数据
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT A.FK_EmpText AS Name, count( A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE  A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID  and STR_TO_DATE(A.SDT,'%Y-%m-%d %H:%i') <STR_TO_DATE(B.SDTOfNode,'%Y-%m-%d %H:%i') GROUP BY A.FK_EmpText";

            }
            else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
            {
                sql = "SELECT  A.FK_EmpText AS Name, count( A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE  A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID  and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(TO_DATE(B.SDTOfNode, 'yyyy-mm-dd hh24:mi:ss') - TO_DATE(A.SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 GROUP BY A.FK_EmpText ";
                sql += "UNION SELECT A.FK_EmpText AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (TO_DATE(B.SDTOfNode, 'yyyy-mm-dd') - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 GROUP BY A.FK_EmpText";
            }
            else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                sql = "SELECT A.FK_EmpText AS Name, count( A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID  and to_timestamp(CASE WHEN A.SDT='无' THEN '' ELSE A.SDT END, 'yyyy-mm-dd hh24:MI:SS') < to_timestamp(CASE WHEN B.SDTOfNode='无' THEN '' ELSE B.SDTOfNode END, 'yyyy-mm-dd hh24:MI:SS') GROUP BY A.FK_EmpText";
            }
            else
            {
                sql = "SELECT A.FK_EmpText AS Name, count( A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID  and convert(varchar(100),A.SDT,120) < CONVERT(varchar(100), B.SDTOfNode, 120) GROUP BY A.FK_EmpText";
            }

            DataTable TodoListOverTByEmp = DBAccess.RunSQLReturnTable(sql);
            TodoListOverTByEmp.TableName = "TodoListOverTByEmp";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListOverTByEmp.Columns[0].ColumnName = "Name";
                TodoListOverTByEmp.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListOverTByEmp);
            #endregion 运行中的流程

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion


        #region 流程监控
        /// <summary>
        /// 流程监控的基础数据
        /// </summary>
        /// <returns></returns>
        public string Watchdog_Init()
        {
            string whereStr = "";
            string whereStrPuls = "";


            if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
            {
                whereStr += " WHERE OrgNo = '" + WebUser.OrgNo + "'";
                whereStrPuls += " AND OrgNo = '" + WebUser.OrgNo + "'";

            }

            Hashtable ht = new Hashtable();
            ht.Add("FlowNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM WF_Flow " + whereStr)); //流程数

            //所有的实例数量.
            ht.Add("FlowInstaceNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState >1 " + whereStrPuls)); //实例数.

            //所有的已完成数量.
            ht.Add("FlowComplete", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=3 AND WFSta=1 " + whereStrPuls)); //实例数.

            //所有的待办数量.
            ht.Add("TodolistNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=2 " + whereStrPuls));

            //退回数.
            ht.Add("ReturnNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=5 " + whereStrPuls));

            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 流程监控折线图数据获取
        /// </summary>
        /// <returns></returns>
        public string Watchdog_EchartDataSet()
        {
            DataSet ds = new DataSet();

            string whereStr = "";
            string whereStrPuls = "";

            if (Glo.CCBPMRunModel != CCBPMRunModel.GroupInc)
            {
                whereStr += " WHERE OrgNo = '" + WebUser.OrgNo + "'";
                whereStrPuls += " AND OrgNo = '" + WebUser.OrgNo + "'";
            }

            #region 完成的流程-按月分析
            //按期完成
            string sql = "SELECT FK_NY, count(WorkID) as Num FROM WF_GenerWorkFlow WHERE WFState=3 AND SendDT<=SDTOfNode And WFSta=1 " + whereStrPuls + " GROUP BY FK_NY ";
            DataTable ComplateFlowsByNY = DBAccess.RunSQLReturnTable(sql);
            ComplateFlowsByNY.TableName = "ComplateFlowsByNY";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                ComplateFlowsByNY.Columns[0].ColumnName = "FK_NY";
                ComplateFlowsByNY.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(ComplateFlowsByNY);

            //逾期完成
            sql = "SELECT FK_NY, count(WorkID) as Num FROM WF_GenerWorkFlow WHERE WFState=3 AND SendDT>SDTOfNode And WFSta=1 " + whereStrPuls + " GROUP BY FK_NY ";
            DataTable OverComplateFlowsByNY = DBAccess.RunSQLReturnTable(sql);
            OverComplateFlowsByNY.TableName = "OverComplateFlowsByNY";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                OverComplateFlowsByNY.Columns[0].ColumnName = "FK_NY";
                OverComplateFlowsByNY.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(OverComplateFlowsByNY);
            #endregion 完成的流程-按月分析

            #region 运行中的流程
            //按部门
            //1.全部待办
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B WHERE A.FK_Dept=B.No GROUP BY B.Name";
            else
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B WHERE A.FK_Dept=B.No AND B.OrgNo='" + WebUser.OrgNo + "' GROUP BY B.Name";
            DataTable TodoListAllByDept = DBAccess.RunSQLReturnTable(sql);
            TodoListAllByDept.TableName = "TodoListAllByDept";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListAllByDept.Columns[0].ColumnName = "Name";
                TodoListAllByDept.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListAllByDept);

            //2.退回的数据
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID AND C.WFState=5 GROUP BY B.Name";
            else
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID AND C.WFState=5 AND B.OrgNo='" + WebUser.OrgNo + "' GROUP BY B.Name";
            DataTable TodoListReturnByDept = DBAccess.RunSQLReturnTable(sql);
            TodoListReturnByDept.TableName = "TodoListReturnByDept";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListReturnByDept.Columns[0].ColumnName = "Name";
                TodoListReturnByDept.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListReturnByDept);

            //3.逾期的数据
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT B.Name, count(DISTINCT C.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID  and STR_TO_DATE(A.SDT,'%Y-%m-%d %H:%i') <STR_TO_DATE(C.SDTOfNode,'%Y-%m-%d %H:%i') GROUP BY B.Name";

            }
            else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
            {
                sql = "SELECT  B.Name, count(DISTINCT C.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID  and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(TO_DATE(C.SDTOfNode, 'yyyy-mm-dd hh24:mi:ss') - TO_DATE(A.SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 GROUP BY B.Name ";
                sql += "UNION SELECT  B.Name, count(DISTINCT C.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (TO_DATE(SDTOfNode, 'yyyy-mm-dd') - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 GROUP BY B.Name";
            }
            else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                sql = "SELECT  B.Name, count(DISTINCT C.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID  and to_timestamp(CASE WHEN A.SDT='无' THEN '' ELSE A.SDT END, 'yyyy-mm-dd hh24:MI:SS') < to_timestamp(CASE WHEN C.SDTOfNode='无' THEN '' ELSE C.SDTOfNode END, 'yyyy-mm-dd hh24:MI:SS') GROUP BY B.Name";
            }
            else
            {
                sql = "SELECT  B.Name, count(DISTINCT C.WorkID) as Num FROM WF_GenerWorkerlist A,Port_Dept B,WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID  and convert(varchar(100),A.SDT,120) < CONVERT(varchar(100), C.SDTOfNode, 120) GROUP BY B.Name";
            }

            DataTable TodoListOverTByDept = DBAccess.RunSQLReturnTable(sql);
            TodoListOverTByDept.TableName = "TodoListOverTByDept";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListOverTByDept.Columns[0].ColumnName = "Name";
                TodoListOverTByDept.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListOverTByDept);

            //4.预警的数据
            //按流程

            //1.全部待办
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_Flow B WHERE A.FK_Flow=B.No GROUP BY B.Name";
            else
                sql = "SELECT B.Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_Flow B WHERE A.FK_Flow=B.No AND B.OrgNo='" + WebUser.OrgNo + "' GROUP BY B.Name";
            DataTable TodoListAllByFlow = DBAccess.RunSQLReturnTable(sql);
            TodoListAllByFlow.TableName = "TodoListAllByFlow";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListAllByFlow.Columns[0].ColumnName = "Name";
                TodoListAllByFlow.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListAllByFlow);

            //2.退回的数据
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID AND B.WFState=5 GROUP BY B.FlowName";
            else
                sql = "SELECT B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID AND B.WFState=5 AND B.OrgNo='" + WebUser.OrgNo + "' GROUP BY B.FlowName";
            DataTable TodoListReturnByFlow = DBAccess.RunSQLReturnTable(sql);
            TodoListReturnByFlow.TableName = "TodoListReturnByFlow";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListReturnByFlow.Columns[0].ColumnName = "Name";
                TodoListReturnByFlow.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListReturnByFlow);

            //3.逾期的数据
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID  and STR_TO_DATE(A.SDT,'%Y-%m-%d %H:%i') <STR_TO_DATE(B.SDTOfNode,'%Y-%m-%d %H:%i') GROUP BY B.FlowName";

            }
            else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
            {
                sql = "SELECT  B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID  and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(TO_DATE(B.SDTOfNode, 'yyyy-mm-dd hh24:mi:ss') - TO_DATE(A.SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 GROUP BY B.FlowName ";
                sql += "UNION SELECT  B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (TO_DATE(B.SDTOfNode, 'yyyy-mm-dd') - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 GROUP BY B.FlowName";
            }
            else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                sql = "SELECT B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID  and to_timestamp(CASE WHEN A.SDT='无' THEN '' ELSE A.SDT END, 'yyyy-mm-dd hh24:MI:SS') < to_timestamp(CASE WHEN B.SDTOfNode='无' THEN '' ELSE B.SDTOfNode END, 'yyyy-mm-dd hh24:MI:SS') GROUP BY B.FlowName";
            }
            else
            {
                sql = "SELECT B.FlowName AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Flow=B.FK_Flow AND A.WorkID=B.WorkID  and convert(varchar(100),A.SDT,120) < CONVERT(varchar(100), B.SDTOfNode, 120) GROUP BY B.FlowName";
            }

            DataTable TodoListOverTByFlow = DBAccess.RunSQLReturnTable(sql);
            TodoListOverTByFlow.TableName = "TodoListOverTByFlow";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListOverTByFlow.Columns[0].ColumnName = "Name";
                TodoListOverTByFlow.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListOverTByFlow);


            //按人员(仅限一个部门中的人员）
            //获取当前人员所在部门的所有人员
            sql = "SELECT A.No,A.Name From Port_Emp A,Port_DeptEmp B Where A.No=B.FK_Emp AND B.FK_Dept='" + WebUser.FK_Dept + "' order By A.Idx";
            DataTable Emps = DBAccess.RunSQLReturnTable(sql);
            Emps.TableName = "Emps";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                Emps.Columns[0].ColumnName = "No";
                Emps.Columns[1].ColumnName = "Name";
            }

            ds.Tables.Add(Emps);

            //1.全部待办
            sql = "SELECT FK_EmpText AS Name, count(WorkID) as Num FROM WF_GenerWorkerlist WHERE FK_Dept='" + WebUser.FK_Dept + "' GROUP BY FK_EmpText";
            DataTable TodoListAllByEmp = DBAccess.RunSQLReturnTable(sql);
            TodoListAllByEmp.TableName = "TodoListAllByEmp";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListAllByEmp.Columns[0].ColumnName = "Name";
                TodoListAllByEmp.Columns[1].ColumnName = "Num";
            }


            ds.Tables.Add(TodoListAllByEmp);

            //2.退回的数据
            sql = "SELECT A.FK_EmpText AS Name, count( A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE  A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID AND B.WFState=5 GROUP BY A.FK_EmpText";
            DataTable TodoListReturnByEmp = DBAccess.RunSQLReturnTable(sql);
            TodoListReturnByEmp.TableName = "TodoListReturnByEmp";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListReturnByEmp.Columns[0].ColumnName = "Name";
                TodoListReturnByEmp.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListReturnByEmp);

            //3.逾期的数据
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT A.FK_EmpText AS Name, count( A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE  A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID  and STR_TO_DATE(A.SDT,'%Y-%m-%d %H:%i') <STR_TO_DATE(B.SDTOfNode,'%Y-%m-%d %H:%i') GROUP BY A.FK_EmpText";

            }
            else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
            {
                sql = "SELECT  A.FK_EmpText AS Name, count( A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE  A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID  and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(TO_DATE(B.SDTOfNode, 'yyyy-mm-dd hh24:mi:ss') - TO_DATE(A.SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 GROUP BY A.FK_EmpText ";
                sql += "UNION SELECT A.FK_EmpText AS Name, count(DISTINCT A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (TO_DATE(B.SDTOfNode, 'yyyy-mm-dd') - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 GROUP BY A.FK_EmpText";
            }
            else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                sql = "SELECT A.FK_EmpText AS Name, count( A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID  and to_timestamp(CASE WHEN A.SDT='无' THEN '' ELSE A.SDT END, 'yyyy-mm-dd hh24:MI:SS') < to_timestamp(CASE WHEN B.SDTOfNode='无' THEN '' ELSE B.SDTOfNode END, 'yyyy-mm-dd hh24:MI:SS') GROUP BY A.FK_EmpText";
            }
            else
            {
                sql = "SELECT A.FK_EmpText AS Name, count( A.WorkID) as Num FROM WF_GenerWorkerlist A,WF_GenerWorkFlow B WHERE A.FK_Dept='" + WebUser.FK_Dept + "' AND A.WorkID=B.WorkID  and convert(varchar(100),A.SDT,120) < CONVERT(varchar(100), B.SDTOfNode, 120) GROUP BY A.FK_EmpText";
            }

            DataTable TodoListOverTByEmp = DBAccess.RunSQLReturnTable(sql);
            TodoListOverTByEmp.TableName = "TodoListOverTByEmp";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                TodoListOverTByEmp.Columns[0].ColumnName = "Name";
                TodoListOverTByEmp.Columns[1].ColumnName = "Num";
            }
            ds.Tables.Add(TodoListOverTByEmp);

            #endregion 运行中的流程


            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 流程监控

    }
}
