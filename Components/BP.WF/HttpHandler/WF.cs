using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.WF;
using BP.WF.Template;
using BP.WF.Port;

namespace BP.WF.HttpHandler
{
    public class WF : DirectoryPageBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        protected override string DoDefaultMethod()
        {
            return base.DoDefaultMethod();
        }

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

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NO"].ColumnName = "";
                dt.Columns["NAME"].ColumnName = "";
                dt.Columns[""].ColumnName = "";
                dt.Columns[""].ColumnName = "";
                dt.Columns[""].ColumnName = "";
            }

            return BP.Tools.Json.DataTableToJson(dt, false);
        }
        #endregion 我的关注.

        /// <summary>
        /// 方法
        /// </summary>
        /// <returns></returns>
        public string HandlerMapExt()
        {
            WF_CCForm wf = new WF_CCForm(context);
            return wf.HandlerMapExt();
        }
        /// <summary>
        /// 获得发起列表 
        /// </summary>
        /// <returns></returns>
        public string Start_Init()
        {
            //通用的处理器.
            if (BP.Sys.SystemConfig.CustomerNo != "TianYe")
                return Start_Init2016();

            // 周朋@于庆海需要翻译.

            BP.WF.Port.WFEmp em = new WFEmp();
            em.No = BP.Web.WebUser.No;
            if (em.RetrieveFromDBSources() == 0)
            {
                em.FK_Dept = BP.Web.WebUser.FK_Dept;
                em.Name = Web.WebUser.Name;
                em.Insert();
            }
            string json = em.StartFlows;
            if (json != "")
                return json;

          

            //获得当前人员的部门,根据部门获得该人员的组织集合.
            Paras ps = new Paras();
            ps.SQL ="SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Emp="+SystemConfig.AppCenterDBVarStr+"FK_Emp";
            ps.AddFK_Emp();
            DataTable dt = DBAccess.RunSQLReturnTable(ps);

            //为当前的人员找组织编号集合.
            string orgNos = "'0'";
            foreach (DataRow dr in dt.Rows)
            {
                string deptNo = dr[0].ToString();
                while (true)
                {
                    Inc inc = new Inc();
                    inc.No = deptNo;
                    if (inc.IsExits == true)
                    {
                        orgNos += ",'" + deptNo + "'";
                        break;
                    }

                    BP.Port.Dept dept = new BP.Port.Dept(deptNo);
                    if (dept.ParentNo == "0")
                        break;
                    deptNo = dept.ParentNo;
                }
            }

            #region 获取类别列表(根据当前人员所在组织结构进行过滤类别.)
            FlowSorts fss = new FlowSorts();
            BP.En.QueryObject qo = new En.QueryObject(fss);
            if (orgNos.Contains(",")==false)
                qo.AddWhere(FlowSortAttr.OrgNo, orgNos);  //指定的类别..
            else
                qo.AddWhereIn(FlowSortAttr.OrgNo, "("+orgNos+")");  //指定的类别.

            //排序.
            qo.addOrderBy(FlowSortAttr.No, FlowSortAttr.Idx);


            DataTable dtSort=qo.DoQueryToTable();
            dtSort.TableName = "Sort";

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtSort.Columns["NO"].ColumnName = "No";
                dtSort.Columns["NAME"].ColumnName = "Name";
                dtSort.Columns["PARENTNO"].ColumnName = "ParentNo";
                dtSort.Columns["ORGNO"].ColumnName = "OrgNo";
            }

            //定义容器.
            DataSet ds = new DataSet();
            ds.Tables.Add(dtSort); //增加到里面去.
            #endregion 获取类别列表.

            //构造流程实例数据容器。
            DataTable dtStart = new DataTable();
            dtStart.TableName = "Start";
            dtStart.Columns.Add("No");
            dtStart.Columns.Add("Name");
            dtStart.Columns.Add("FK_FlowSort");
            dtStart.Columns.Add("IsBatchStart");

            //获得所有的流程（包含了所有子公司与集团的可以发起的流程但是没有根据组织结构进行过滤.）
            DataTable dtAllFlows = Dev2Interface.DB_StarFlows(Web.WebUser.No);

            //按照当前用户的流程类别权限进行过滤.
            foreach (DataRow drSort in dtSort.Rows)
            {
                foreach (DataRow drFlow in dtAllFlows.Rows)
                {
                    if (drSort["No"].ToString() != drFlow["FK_FlowSort"].ToString())
                        continue;

                    DataRow drNew = dtStart.NewRow();

                    drNew["No"] = drFlow["No"];
                    drNew["Name"] = drFlow["Name"];
                    drNew["FK_FlowSort"] = drFlow["FK_FlowSort"];
                    drNew["IsBatchStart"] = drFlow["IsBatchStart"];
                    dtStart.Rows.Add(drNew); //增加到里里面去.
                }
            }

            //把经过权限过滤的流程实体放入到集合里.
            ds.Tables.Add(dtStart); //增加到里面去.
             
            //返回组合
            json= BP.Tools.Json.DataSetToJson(ds, false);

            //把json存入数据表，避免下一次再取.
            em.StartFlows = json;
            em.Update();
             

            return json;
        }
        public string Start_Init2016()
        {
            DataSet ds = new DataSet();

            //流程类别.
            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();

            DataTable dtSort = fss.ToDataTableField("Sort");
            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            //获得能否发起的流程.
            DataTable dtStart = Dev2Interface.DB_StarFlows(Web.WebUser.No);
            dtStart.TableName = "Start";
            ds.Tables.Add(dtStart);

            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 获得发起列表
        /// </summary>
        /// <returns></returns>
        public string FlowSearch_Init()
        {
            DataSet ds = new DataSet();

            //流程类别.
            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();

            DataTable dtSort = fss.ToDataTableField("Sort");
            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            //获得能否发起的流程.
            DataTable dtStart = DBAccess.RunSQLReturnTable("SELECT No,Name, FK_FlowSort FROM WF_Flow ORDER BY FK_FlowSort,Idx");
            dtStart.TableName = "Start";

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtStart.Columns["NO"].ColumnName = "No";
                dtStart.Columns["NAME"].ColumnName = "Name";
                dtStart.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
            }

            ds.Tables.Add(dtStart);



            //返回组合
            return BP.Tools.Json.DataSetToJson(ds,false);
        }
        #region 获得列表.
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="UserNo">人员编号</param>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>运行中的流程</returns>
        public string Runing_Init()
        {
            WF_App_ACE page = new WF_App_ACE(context);
            return page.Runing_Init();
        }
        /// <summary>
        /// 执行撤销
        /// </summary>
        /// <returns></returns>
        public string Runing_UnSend()
        {
            try
            {
                return BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.WorkID);
            }
            catch(Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行催办
        /// </summary>
        /// <returns></returns>
        public string Runing_Press()
        {
            try
            {
                return BP.WF.Dev2Interface.Flow_DoPress(this.WorkID, this.GetRequestVal("Msg"),false);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 草稿
        /// </summary>
        /// <returns></returns>
        public string Draft_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable();
            return BP.Tools.Json.DataTableToJson(dt, false);
        }
        /// <summary>
        /// 删除草稿.
        /// </summary>
        /// <returns></returns>
        public string Draft_Delete()
        {
          return BP.WF.Dev2Interface.Flow_DoDeleteDraft(this.FK_Flow, this.WorkID, false);
        }
        /// <summary>
        /// 初始化待办.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Init()
        {
            WF_App_ACE en = new WF_App_ACE(context);
            return en.Todolist_Init();
        }
        /// <summary>
        /// 获得授权人的待办.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Author()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(this.No, this.FK_Node);

            //转化大写的toJson.
            return BP.Tools.Json.DataTableToJson(dt,true);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string TodolistOfAuth_Init()
        {
            return "err@尚未重构完成.";
           
            //DataTable dt = null;
            //foreach (BP.WF.Port.WFEmp item in ems)
            //{
            //    if (dt == null)
            //    {
            //        dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(item.No, null);
            //    }
            //    else
            //    {
            //    }
            //}

          //    return BP.Tools.Json.DataTableToJson(dt, false);

            //string fk_emp = this.FK_Emp;
            //if (fk_emp == null)
            //{
            //    //移除不需要前台看到的数据.
            //    DataTable dt = ems.ToDataTableField();
            //    dt.Columns.Remove("SID");
            //    dt.Columns.Remove("Stas");
            //    dt.Columns.Remove("Depts");
            //    dt.Columns.Remove("Msg");
            //    return BP.Tools.Json.DataTableToJson(dt, false);
            //}

        }
         /// <summary>
        /// 获得挂起.
        /// </summary>
        /// <returns></returns>
        public string HungUpList_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerHungUpList();
            
            //转化大写的toJson.
            return BP.Tools.Json.DataTableToJson(dt,false);
        }
        #endregion 获得列表.


        #region 共享任务池.
        /// <summary>
        /// 初始化共享任务
        /// </summary>
        /// <returns></returns>
        public string TaskPoolSharing_Init()
        {
           DataTable dt = BP.WF.Dev2Interface.DB_TaskPool();

           return BP.Tools.Json.DataTableToJson(dt, false);
        }
        /// <summary>
        /// 申请任务.
        /// </summary>
        /// <returns></returns>
        public string TaskPoolSharing_Apply()
        {
            bool b = BP.WF.Dev2Interface.Node_TaskPoolTakebackOne(this.WorkID);
            if (b == true)
                return "申请成功.";
            else
                return "err@申请失败...";
        }
        /// <summary>
        /// 我申请下来的任务
        /// </summary>
        /// <returns></returns>
        public string TaskPoolApply_Init()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_TaskPoolOfMyApply();

            return BP.Tools.Json.DataTableToJson(dt, false);
        }
        public string TaskPoolApply_PutOne()
        {
            BP.WF.Dev2Interface.Node_TaskPoolPutOne(this.WorkID);
            return "放入成功,其他的同事可以看到这件工作.您可以在任务池里看到它并重新申请下来.";
        }
        #endregion

        #region 登录相关.
        /// <summary>
        /// 返回当前会话信息.
        /// </summary>
        /// <returns></returns>
        public string Login_Init()
        {
            Hashtable ht = new Hashtable();

            if (BP.Web.WebUser.NoOfRel == null)
                ht.Add("UserNo", "");
            else
                ht.Add("UserNo", BP.Web.WebUser.No);

            if (BP.Web.WebUser.IsAuthorize)
                ht.Add("Auth", BP.Web.WebUser.Auth);
            else
                ht.Add("Auth", "");
            return BP.Tools.FormatToJson.ToJson(ht);
        }
        /// <summary>
        /// 执行登录.
        /// </summary>
        /// <returns></returns>
        public string LoginSubmit()
        {
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = this.GetValFromFrmByKey("TB_UserNo");

            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户名或密码错误.";
            string pass = this.GetValFromFrmByKey("TB_Pass");
            if (emp.Pass.Equals(pass) == false)
                return "err@用户名或密码错误.";

            //让其登录.
            string sid = BP.WF.Dev2Interface.Port_Login(emp.No);
            return sid;
        }
        /// <summary>
        /// 执行授权登录
        /// </summary>
        /// <returns></returns>
        public string LoginAs()
        {
            BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(this.No);
            if (wfemp.AuthorIsOK == false)
                return "err@授权登录失败！";
            BP.Port.Emp emp1 = new BP.Port.Emp(this.No);
            BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, BP.Web.WebUser.No, BP.Web.WebUser.Name);
            return "success@授权登录成功！";
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="UserNo"></param>
        /// <param name="Author"></param>
        /// <returns></returns>
        public string AuthExitAndLogin(string UserNo, string Author)
        {
            string msg = "suess@退出成功！";
            try
            {
                BP.Port.Emp emp = new BP.Port.Emp(UserNo);
                //首先退出
                BP.Web.WebUser.Exit();
                //再进行登录
                BP.Port.Emp emp1 = new BP.Port.Emp(Author);
                BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, null, null);
            }
            catch (Exception ex)
            {
                msg = "err@退出时发生错误。" + ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// 获取授权人列表
        /// </summary>
        /// <returns></returns>
        public string Load_Author()
        {
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM WF_EMP WHERE AUTHOR='" + BP.Web.WebUser.No + "'");
            return BP.Tools.FormatToJson.ToJson(dt);
        }
        /// <summary>
        /// 当前登陆人是否有授权
        /// </summary>
        /// <returns></returns>
        public string IsHaveAuthor()
        {
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM WF_EMP WHERE AUTHOR='" + BP.Web.WebUser.No + "'");
            WFEmp em = new WFEmp();
            em.Retrieve(WFEmpAttr.Author, BP.Web.WebUser.No);

            if (dt.Rows.Count > 0 && BP.Web.WebUser.IsAuthorize == false)
                return "suess@有授权";
            else
                return "err@没有授权";
        }
        /// <summary>
        /// 退出.
        /// </summary>
        /// <returns></returns>
        public string LoginExit()
        {
            BP.WF.Dev2Interface.Port_SigOut();
            return null;
        }
        /// <summary>
        /// 授权退出.
        /// </summary>
        /// <returns></returns>
        public string AuthExit()
        {
            return this.AuthExitAndLogin(this.No, BP.Web.WebUser.Auth);
        }
        #endregion 登录相关.

        /// <summary>
        /// 获得抄送列表
        /// </summary>
        /// <returns></returns>
        public string CC_Init()
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
            //BP.WF.SMSs ss = new BP.WF.SMSs();
            //BP.En.QueryObject qo = new BP.En.QueryObject(ss);

            System.Data.DataTable dt = null;
            if (sta == "-1")
                dt = BP.WF.Dev2Interface.DB_CCList(BP.Web.WebUser.No);
            if (sta == "0")
                dt = BP.WF.Dev2Interface.DB_CCList_UnRead(BP.Web.WebUser.No);
            if (sta == "1")
                dt = BP.WF.Dev2Interface.DB_CCList_Read(BP.Web.WebUser.No);
            if (sta == "2")
                dt = BP.WF.Dev2Interface.DB_CCList_Delete(BP.Web.WebUser.No);

            //int allNum = qo.GetCount();
            //qo.DoQuery(BP.WF.SMSAttr.MyPK, pageSize, pageIdx);

            return BP.Tools.Json.DataTableToJson(dt, false);
        }
    }
}
