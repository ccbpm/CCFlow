using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using BP.WF.Data;
using BP.WF.HttpHandler;
using BP.WF.XML;

namespace BP.CZBank
{
    /// <summary>
    /// 初始化函数
    /// </summary>
    public class App : DirectoryPageBase
    {
        #region 处理page接口.
        /// <summary>
        /// 执行的内容
        /// </summary>
        public string DoWhat
        {
            get
            {
                return this.GetRequestVal("DoWhat");
            }
        }
        /// <summary>
        /// 当前的用户
        /// </summary>
        public string UserNo
        {
            get
            {
                return this.GetRequestVal("UserNo");
            }
        }
        /// <summary>
        /// 用户的安全校验码(请参考集成章节)
        /// </summary>
        public string SID
        {
            get
            {
                return this.GetRequestVal("SID");
            }
        }

        public string Port_Init()
        {
            #region 安全性校验.
            if (this.UserNo == null || this.SID == null || this.DoWhat == null)
                return "err@必要的参数没有传入，请参考接口规则。";

            if (BP.WF.Dev2Interface.Port_CheckUserLogin(this.UserNo, this.SID) == false)
                return "err@非法的访问，请与管理员联系。SID=" + this.SID;

            if (BP.Web.WebUser.No.Equals(this.UserNo) == false)
            {
                BP.WF.Dev2Interface.Port_SigOut();
                try
                {
                    BP.WF.Dev2Interface.Port_Login(this.UserNo, this.SID);
                }
                catch (Exception ex)
                {
                    return "err@安全校验出现错误:" + ex.Message;
                }
            }
            #endregion 安全性校验.

            #region 生成参数串.
            string paras = "";
            foreach (string str in this.context.Request.QueryString)
            {
                string val = this.GetRequestVal(str);
                if (val.IndexOf('@') != -1)
                    return "err@您没有能参数: [ " + str + " ," + val + " ] 给值 ，URL 将不能被执行。";

                switch (str)
                {
                    case DoWhatList.DoNode:
                    case DoWhatList.Emps:
                    case DoWhatList.EmpWorks:
                    case DoWhatList.FlowSearch:
                    case DoWhatList.Login:
                    case DoWhatList.MyFlow:
                    case DoWhatList.MyWork:
                    case DoWhatList.Start:
                    case DoWhatList.Start5:
                    case DoWhatList.StartSimple:
                    case DoWhatList.FlowFX:
                    case DoWhatList.DealWork:
                    case "FK_Flow":
                    case "WorkID":
                    case "FK_Node":
                    case "SID":
                        break;
                    default:
                        paras += "&" + str + "=" + val;
                        break;
                }
            }
            string nodeID = int.Parse(this.FK_Flow + "01").ToString();
            #endregion 生成参数串.


            //发起流程.
            if (this.DoWhat.Equals("StartClassic") == true)
            {
                if (this.FK_Flow == null)
                    return "url@Home.htm";
                else
                    return "url@Home.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID;
            }

            //打开工作轨迹。
            if (this.DoWhat.Equals(DoWhatList.OneWork) == true)
            {
                if (this.FK_Flow == null || this.WorkID == null)
                    throw new Exception("@参数 FK_Flow 或者 WorkID 为 Null 。");
                return "url@WFRpt.htm?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras;
            }

            //发起页面.
            if (this.DoWhat.Equals(DoWhatList.Start) == true)
            {
                if (this.FK_Flow == null)
                    return "url@Start.htm";
                else
                    return "url@MyFlow.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID;
            }

            //处理工作.
            if (this.DoWhat.Equals(DoWhatList.DealWork) == true)
            {
                if (DataType.IsNullOrEmpty(this.FK_Flow) || this.WorkID == 0)
                    return "err@参数 FK_Flow 或者 WorkID 为Null 。";

                return "url@MyFlow.htm?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras;
            }

            //请求在途.
            if (this.DoWhat.Equals(DoWhatList.Runing) == true)
            {
                return "url@Runing.htm?FK_Flow=" + this.FK_Flow;
            }

            //请求待办。
            if (this.DoWhat.Equals(DoWhatList.EmpWorks) == true || this.DoWhat.Equals("Todolist") == true)
            {
                if (DataType.IsNullOrEmpty(this.FK_Flow))
                    return "url@Todolist.htm";
                else
                    return "url@Todolist.htm?FK_Flow=" + this.FK_Flow;
            }

            //请求流程查询。
            if (this.DoWhat.Equals(DoWhatList.FlowSearch) == true)
            {
                if (DataType.IsNullOrEmpty(this.FK_Flow))
                    return "url@./RptSearch/Default.htm";
                else
                    return "url@./RptDfine/FlowSearch.htm?2=1&FK_Flow=001&EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras;
            }

            //流程查询小页面.
            if (this.DoWhat.Equals(DoWhatList.FlowSearchSmall) == true)
            {
                if (this.FK_Flow == null)
                    return "url@./RptSearch/Default.htm";
                else
                    return "url./Comm/Search.htm?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras;
            }

            //打开消息.
            if (this.DoWhat.Equals(DoWhatList.DealMsg) == true)
            {
                string guid = this.GetRequestVal("GUID");
                BP.WF.SMS sms = new SMS();
                sms.MyPK = guid;
                sms.Retrieve();

                //判断当前的登录人员.
                if (BP.Web.WebUser.No != sms.SendToEmpNo)
                    BP.WF.Dev2Interface.Port_Login(sms.SendToEmpNo);

                BP.DA.AtPara ap = new AtPara(sms.AtPara);
                switch (sms.MsgType)
                {
                    case SMSMsgType.SendSuccess: // 发送成功的提示.

                        if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(ap.GetValStrByKey("FK_Flow"),
                            ap.GetValIntByKey("FK_Node"), ap.GetValInt64ByKey("WorkID"), BP.Web.WebUser.No) == true)
                            return "url@MyFlow.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                        else
                            return "url@WFRpt.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                    default: //其他的情况都是查看工作报告.
                        return "url@WFRpt.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                }
            }

            return "err@没有约定的标记:DoWhat=" + this.DoWhat;
        }
        #endregion 处理page接口.

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="mycontext"></param>
        public App(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 获得发起流程
        /// </summary>
        /// <returns></returns>
        public string Start_Init()
        {
            BP.WF.HttpHandler.WF wf = new WF.HttpHandler.WF(this.context);
            return wf.Start_Init();
        }
        /// <summary>
        /// 获得待办
        /// </summary>
        /// <returns></returns>
        public string Todolist_Init()
        {
            BP.WF.HttpHandler.WF wf = new WF.HttpHandler.WF(this.context);
            return wf.Todolist_Init();
        }

        /// <summary>
        /// 获取退回消息
        /// </summary>
        /// <returns></returns>
        public string DB_GenerReturnWorks()
        {
            /* 如果工作节点退回了*/
            BP.WF.ReturnWorks rws = new BP.WF.ReturnWorks();
            rws.Retrieve(BP.WF.ReturnWorkAttr.ReturnToNode, this.FK_Node, BP.WF.ReturnWorkAttr.WorkID, this.WorkID, BP.WF.ReturnWorkAttr.RDT);
            StringBuilder append = new StringBuilder();
            append.Append("[");
            if (rws.Count != 0)
            {
                foreach (BP.WF.ReturnWork rw in rws)
                {
                    append.Append("{");
                    append.Append("ReturnNodeName:'" + rw.ReturnNodeName + "',");
                    append.Append("ReturnerName:'" + rw.ReturnerName + "',");
                    append.Append("RDT:'" + rw.RDT + "',");
                    append.Append("NoteHtml:'" + rw.BeiZhuHtml + "'");
                    append.Append("},");
                }
                append.Remove(append.Length - 1, 1);
            }
            append.Append("]");
            return BP.Tools.Entitis2Json.Instance.ReplaceIllgalChart(append.ToString());
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="UserNo">人员编号</param>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>运行中的流程</returns>
        public string Runing_Init()
        {

            BP.WF.HttpHandler.WF wf = new WF.HttpHandler.WF(this.context);
            return wf.Runing_Init();

            //DataTable dt = null;
            //dt = BP.WF.Dev2Interface.DB_GenerRuning();
            //return BP.Tools.Json.DataTableToJson(dt,false);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public string GetUserInfo()
        {
            if (WebUser.No == null)
                return "{err:'nologin'}";

            StringBuilder append = new StringBuilder();
            append.Append("{");
            string userPath = HttpContext.Current.Server.MapPath("/DataUser/UserIcon/");
            string userIcon = userPath + BP.Web.WebUser.No + "Biger.png";
            if (System.IO.File.Exists(userIcon))
            {
                append.Append("UserIcon:'" + BP.Web.WebUser.No + "Biger.png'");
            }
            else
            {
                append.Append("UserIcon:'DefaultBiger.png'");
            }
            append.Append(",UserName:'" + BP.Web.WebUser.Name + "'");
            append.Append(",UserDeptName:'" + BP.Web.WebUser.FK_DeptName + "'");
            append.Append("}");
            return append.ToString();
        }

        /// <summary>
        /// 初始化赋值.
        /// </summary>
        /// <returns></returns>
        public string Home_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("UserNo", BP.Web.WebUser.No);
            ht.Add("UserName", BP.Web.WebUser.Name);

            //系统名称.
            ht.Add("SysName", BP.Sys.SystemConfig.SysName);
            ht.Add("CustomerName", BP.Sys.SystemConfig.CustomerName);

            ht.Add("Todolist_EmpWorks", BP.WF.Dev2Interface.Todolist_EmpWorks);
            ht.Add("Todolist_Runing", BP.WF.Dev2Interface.Todolist_Runing);
            ht.Add("Todolist_Sharing", BP.WF.Dev2Interface.Todolist_Sharing);
            ht.Add("Todolist_CCWorks", BP.WF.Dev2Interface.Todolist_CCWorks);
            ht.Add("Todolist_Apply", BP.WF.Dev2Interface.Todolist_Apply); //申请下来的任务个数.
            ht.Add("Todolist_Draft", BP.WF.Dev2Interface.Todolist_Draft); //草稿数量.

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
        /// <summary>
        /// 转换成菜单.
        /// </summary>
        /// <returns></returns>
        public string Home_Menu()
        {
            DataSet ds = new DataSet();

            BP.WF.XML.ClassicMenus menus = new BP.WF.XML.ClassicMenus();
            menus.RetrieveAll();

           DataTable dtMain=  menus.ToDataTable();
           dtMain.TableName = "ClassicMenus";
           ds.Tables.Add(dtMain);

           BP.WF.XML.ClassicMenuAdvFuncs advMenms = new BP.WF.XML.ClassicMenuAdvFuncs();
           advMenms.RetrieveAll();

           DataTable dtMenuAdv = advMenms.ToDataTable();
           dtMenuAdv.TableName = "ClassicMenusAdv";
           ds.Tables.Add(dtMenuAdv);

           return BP.Tools.Json.ToJson(ds);
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
            throw new Exception("@标记["+this.DoType+"]，没有找到.");
        }
        #endregion 执行父类的重写方法.

        #region 控制台.
        /// <summary>
        /// 控制台信息.
        /// </summary>
        /// <returns></returns>
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
        #endregion 控制台.

        /// <summary>
        /// 设置
        /// </summary>
        /// <returns></returns>
        public string Setting_Init()
        {
            return "";
        }

        #region 登录界面.
        /// <summary>
        /// 登录.
        /// </summary>
        /// <returns></returns>
        public string Login_Submit()
        {
            string userNo = this.GetRequestVal("TB_UserNo");
            string pass = this.GetRequestVal("TB_Pass");

            BP.Port.Emp emp = new Emp();
            emp.No = userNo;
            if (emp.RetrieveFromDBSources() == 0)
            {
                if (DBAccess.IsExitsTableCol("Port_Emp", "NikeName") == true)
                {
                    /*如果包含昵称列,就检查昵称是否存在.*/
                    string sql = "SELECT No FROM Port_Emp WHERE NikeName='" + userNo + "'";
                    string no = DBAccess.RunSQLReturnStringIsNull(sql, null);
                    if (no == null)
                        return "err@用户名或者密码错误.";

                    emp.No = no;
                    int i = emp.RetrieveFromDBSources();
                    if (i == 0)
                        return "err@用户名或者密码错误.";
                }
                else
                {
                    return "err@用户名或者密码错误.";
                }
            }

            if (emp.CheckPass(pass) ==false )
                return "err@用户名或者密码错误.";

            //调用登录方法.
            BP.WF.Dev2Interface.Port_Login(emp.No, emp.Name, emp.FK_Dept, emp.FK_DeptText);

            return "登录成功.";
        }
        public string Login_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("SysName", SystemConfig.SysName);
            ht.Add("ServiceTel", SystemConfig.ServiceTel);
            ht.Add("CustomerName", SystemConfig.CustomerName);

            if ( WebUser.NoOfRel == null )
            {
                ht.Add("UserNo", "");
                ht.Add("UserName", "");
            }
            else
            {
                ht.Add("UserNo", WebUser.No);

                string name = WebUser.Name;

                if (string.IsNullOrEmpty(name) == true)
                    ht.Add("UserName", WebUser.No);
                else
                    ht.Add("UserName", name);
            }

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        #endregion 登录界面.

        #region 草稿.
        /// <summary>
        /// 草稿.
        /// </summary>
        /// <returns></returns>
        public string Draft_Init()
        {
            System.Data.DataTable dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable(this.FK_Flow);

            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 草稿.

        #region 草稿删除.
        /// <summary>
        /// 草稿.
        /// </summary>
        /// <returns></returns>
        public string Draft_Delete()
        {
            try
            {
                BP.WF.Dev2Interface.Node_DeleteDraft(this.FK_Flow, this.WorkID);
                return "删除成功";
            }
            catch (Exception e)
            {
                return "err@" + e.Message;
            }
        }
        #endregion 草稿删除.

        #region 抄送流程.
        /// <summary>
        /// 抄送流程
        /// </summary>
        /// <returns></returns>
        public string cc_Init()
        {
            string sta = this.GetRequestVal("Sta");
            if (sta == null || sta == "")
                sta = "-1";

            int pageSize = 6;// int.Parse(pageSizeStr);

            string pageIdxStr = this.GetRequestVal("PageIdx");
            if (pageIdxStr == null)
                pageIdxStr = "1";
            int pageIdx = int.Parse(pageIdxStr);

            //实体查询.
            BP.WF.SMSs ss = new BP.WF.SMSs();
            BP.En.QueryObject qo = new BP.En.QueryObject(ss);

            System.Data.DataTable dt = null;
            if (sta == "-1")
                dt = BP.WF.Dev2Interface.DB_CCList(BP.Web.WebUser.No);
            if (sta == "0")
                dt = BP.WF.Dev2Interface.DB_CCList_UnRead(BP.Web.WebUser.No);
            if (sta == "1")
                dt = BP.WF.Dev2Interface.DB_CCList_Read(BP.Web.WebUser.No);
            if (sta == "2")
                dt = BP.WF.Dev2Interface.DB_CCList_Delete(BP.Web.WebUser.No);

            int allNum = qo.GetCount();
            qo.DoQuery(BP.WF.SMSAttr.MyPK, pageSize, pageIdx);

            //绑定分页
            // this.Pub1.BindPageIdx(allNum, pageSize, pageIdx, "CC.aspx?Sta=" + sta);

            /*BP.WF.XML.CCMenus tps = new BP.WF.XML.CCMenus();
            tps.RetrieveAll();
            string link = ""; // "<a href='?MsgType=All'>全部</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            foreach (BP.WF.XML.CCMenu tp in tps)
            {
                link += "<a href='?Sta=" + tp.No + "'>" + tp.Name + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            }*/
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 抄送.

        #region 我的关注流程.
        /// <summary>
        /// 我的关注流程
        /// </summary>
        /// <returns></returns>
        public string Focus_Init()
        {
            string flowNo = this.GetRequestVal("FK_Flow");

            int idx = 0;
            //获得关注的数据.
            System.Data.DataTable dt = BP.WF.Dev2Interface.DB_Focus(flowNo, BP.Web.WebUser.No);
            SysEnums stas = new SysEnums("WFSta");
            string[] tempArr;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                int wfsta = int.Parse(dr["WFSta"].ToString());
                //edit by liuxc,2016-10-22,修复状态显示不正确问题
                string wfstaT = (stas.GetEntityByKey(SysEnumAttr.IntKey, wfsta) as SysEnum).Lab;
                string currEmp = string.Empty;

                if (wfsta != (int)BP.WF.WFSta.Complete)
                {
                    //edit by liuxc,2016-10-24,未完成时，处理当前处理人，只显示处理人姓名
                    foreach (string emp in dr["ToDoEmps"].ToString().Split(';'))
                    {
                        tempArr = emp.Split(',');

                        currEmp += tempArr.Length > 1 ? tempArr[1] : tempArr[0] + ",";
                    }

                    currEmp = currEmp.TrimEnd(',');

                    //currEmp = dr["ToDoEmps"].ToString();
                    //currEmp = currEmp.TrimEnd(';');
                }
                dr["ToDoEmps"] = currEmp;
                dr["FlowNote"] = wfstaT;
                dr["AtPara"] = (wfsta == (int)BP.WF.WFSta.Complete ? dr["Sender"].ToString().TrimStart('(').TrimEnd(')').Split(',')[1] : "");
            }
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 我的关注.

        #region 取消关注流程.
        /// <summary>
        /// 我的关注流程
        /// </summary>
        /// <returns></returns>
        public string Focus_Delete()
        {
            BP.WF.Dev2Interface.Flow_Focus(Int64.Parse(this.GetRequestVal("WorkID")));
            return "您已取消关注！";
        }
        #endregion 取消关注流程.

        #region 流程查询.
        /// <summary>
        /// 流程查询
        /// </summary>
        /// <returns></returns>
        public string FlowRpt_Init()
        {
            StringBuilder Pub1 = new StringBuilder(); 
            BP.WF.Flows fls = new BP.WF.Flows();
            fls.RetrieveAll();

            FlowSorts ens = new FlowSorts();
            ens.RetrieveAll();

            DataTable dt = BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(BP.Web.WebUser.No);

            int cols = 3; //定义显示列数 从0开始。
            decimal widthCell = 100 / cols;
            Pub1.Append("<Table width=100% border=0>");
            int idx = -1;
            bool is1 = false;

            //string timeKey = "s" + this.Session.SessionID + DateTime.Now.ToString("yyMMddHHmmss");
            foreach (FlowSort en in ens)
            {
                if (en.ParentNo == "0"
                    || en.ParentNo == ""
                    || en.No == "")
                    continue;

                idx++;
                if (idx == 0)
                    Pub1.Append(AddTR(is1));
                    is1 = !is1;

                Pub1.Append(AddTDBegin("width='" + widthCell + "%' border=0 valign=top"));
                //输出类别.
                //this.Pub1.AddFieldSet(en.Name);
                Pub1.Append(AddB(en.Name));
                Pub1.Append(AddUL());

                #region 输出流程。
                foreach (Flow fl in fls)
                {
                    if (fl.FlowAppType == FlowAppType.DocFlow)
                        continue;

                    //如果该目录下流程数量为空就返回.
                    if (fls.GetCountByKey(BP.WF.Template.FlowAttr.FK_FlowSort, en.No) == 0)
                        continue;

                    if (fl.FK_FlowSort != en.No)
                        continue;

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["No"].ToString() != fl.No)
                            continue;
                        break;
                    }

                    Pub1.Append(AddLi(" <a  href=\"javascript:WinOpen('../WF/Rpt/Search.aspx?RptNo=ND" + int.Parse(fl.No) + "MyRpt&FK_Flow=" + fl.No + "');\" >" + fl.Name + "</a> "));
                }
                #endregion 输出流程。

                Pub1.Append(AddULEnd());

                Pub1.Append(AddTDEnd());
                if (idx == cols - 1)
                {
                    idx = -1;
                    Pub1.Append(AddTREnd());
                }
            }

            while (idx != -1)
            {
                idx++;
                if (idx == cols - 1)
                {
                    idx = -1;
                    Pub1.Append(AddTD());
                    Pub1.Append(AddTREnd());
                }
                else
                {
                    Pub1.Append(AddTD());
                }
            }
            Pub1.Append(AddTableEnd());
            return Pub1.ToString();
        }
        #endregion 流程查询.



        #region 添加公共的字符串拼接方法table.
        ////正常的页面方法请放在此方法前面
        ///添加公共的字符串拼接方法table
        public string AddTR(bool item)
        {
            if (item)
                return "\n<TR bgcolor=AliceBlue >";
            else
                return "\n<TR bgcolor=white class=TR>";
        }

        public string AddTDBegin(string attr)
        {
            return "\n<TD " + attr + " nowrap >";
        }

        public string AddB(string s)
        {
            if (s == null || s == "")
                return "";
            return "<B>" + s + "</B>";
        }
        public string AddUL()
        {
            return "<ul>";
        }

        public string AddLi(string html)
        {
            return "<li>" + html + "</li> \t\n";
        }

        public string AddULEnd()
        {
            return "</ul>\t\n";
        }

        public string AddTDEnd()
        {
            return "\n</TD>";
        }

        public string AddTREnd()
        {
            return "\n</TR>";
        }

        public string AddTD()
        {
            return "\n<TD >&nbsp;</TD>";
        }

        public string AddTableEnd()
        {
            return "</Table>";
        }
        #endregion 添加公共的字符串拼接方法table.
    }
   
}
