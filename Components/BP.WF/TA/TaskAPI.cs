using BP.CCOA;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Web;
using BP.WF;
using NPOI.POIFS.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace BP.TA
{
    public class TaskAPI
    {
        #region 菜单列表.
        /// <summary>
        /// 发起
        /// </summary>
        /// <returns></returns>
        public static string DB_Start()
        {
            Templates ens = new Templates();
            return BP.Tools.Json.ToJson(ens.ToDataTableField());
        }
        /// <summary>
        /// 待办
        /// </summary>
        /// <returns></returns>
        public static string DB_Todolist()
        {
            Tasks tas = new Tasks();
            QueryObject qo = new QueryObject(tas);
            qo.addLeftBracket();
            qo.AddWhere(TaskAttr.EmpNo, WebUser.No);
            qo.addAnd();
            qo.AddWhereIn(TaskAttr.TaskSta, "(1,4,6)"); // 待办的, 审核中的,需要重做的.
            qo.addRightBracket();

            qo.addOr();

            qo.addLeftBracket();
            qo.AddWhere(TaskAttr.SenderNo, WebUser.No);
            qo.addAnd();
            qo.AddWhereIn(TaskAttr.TaskSta, "(2,5)"); // 审核中, 退回.
            qo.addRightBracket();

            qo.addOrderBy("ADT");

            qo.DoQuery();

            return BP.Tools.Json.ToJson(tas.ToDataTableField());
        }
        /// <summary>
        /// 在途
        /// </summary>
        /// <returns></returns>
        public static string DB_Running()
        {
            return "";
        }
        #endregion 菜单列表.

        #region 项目
        /// <summary>
        /// 创建工作
        /// </summary>
        /// <param name="templateNo">模板编号</param>
        /// <returns></returns>
        public static string Prj_CreateNo(string templateNo)
        {
            Template template = new Template(templateNo);

            //该模板类型下是否有这个项目.
            Project prj = new Project();
            int i = prj.Retrieve(ProjectAttr.PrjSta, 0, "TemplateNo", template.No, ProjectAttr.StarterNo, WebUser.No);
            if (i == 1)
                return prj.No;

            prj.Name = template.Name;
            prj.PrjDesc = template.PrjDesc; //任务描述.
            prj.StarterNo = WebUser.No;
            prj.StarterName = WebUser.Name;
            prj.PrjSta = 0; //空白状态.
            prj.No = BP.DA.DBAccess.GenerOID("PrjNo").ToString().PadLeft(5, '0');
            prj.RDT = DataType.CurrentDateTime;
            prj.TemplateNo = template.No;
            prj.TemplateName = template.Name;
            prj.Insert();

            if (template.TaskModel.Equals("Daily") == true)
            {
                prj.PrjSta = 2; //运行状态.
                prj.Update();
                return prj.No;
            }

            if (template.TaskModel.Equals("Section") == true)
            {
                //获得节点.
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.TemplateNo, templateNo, "Idx");

                //为节点的人员创建工作.
                foreach (Node nd in nds)
                {
                    Task ta = new Task();

                    ta.NodeNo = nd.No;
                    ta.NodeName = nd.Name;


                    ta.Title = nd.Name;
                    ta.TaskSta = 0;
                    ta.ItIsRead = 0;

                    //发送人= 任务的下达人.
                    ta.SenderNo = BP.Web.WebUser.No;
                    ta.SenderName = BP.Web.WebUser.Name;

                    //生成负责人.
                    string fzrEmpNo = nd.GenerFZR();
                    if (DataType.IsNullOrEmpty(fzrEmpNo) == false)
                    {
                        Emp fzr = new Emp();
                        ta.EmpNo = fzr.No;
                        ta.EmpName = fzr.Name;
                    }

                    //Todo: 生成协助人.暂不实现.
                    //赋值项目信息.
                    ta.PrjNo = prj.No;
                    ta.PrjName = prj.Name;
                    ta.WCL = 0;
                    ta.PRI = 0;
                    ta.TaskSta = 0;
                    ta.StarterNo = BP.Web.WebUser.No;
                    ta.StarterName = BP.Web.WebUser.Name;
                    ta.InsertAsOID(DBAccess.GenerOID("Task"));
                    //ta.EmpNo = item.
                }
                return prj.No;
            }

            if (template.TaskModel.Equals("TaskTree") == true)
            {

            }

            return "err@没有判断的类型:" + template.TaskModel;
        }
        public static string Prj_Start(string prjNo)
        {
            Project prj = new Project(prjNo);
            if (prj.PrjSta == 0 || prj.PrjSta == 1)
            {
            }
            else
            {
                //throw new Exception("项目已经启动了,您不能在执行启动。");
                return "err@项目已经启动了,您不能在执行启动。";
            }
            prj.PrjSta = 2; //设置启动状态.
            prj.RDT = DataType.CurrentDateTime;

            // 获得所有的任务.
            Tasks tas = new Tasks();
            tas.Retrieve(TaskAttr.PrjNo, prjNo);

            // 工作人员.
            WorkerList wl = new WorkerList();
            wl.PrjNo = prj.No;
            wl.PrjName = prj.Name;
            wl.RDT = DataType.CurrentDateTime;

            // 列表.
            string msg = "任务分配给:";
            foreach (Task ta in tas)
            {
                ta.PrjSta = prj.PrjSta;
                ta.PrjName = prj.Name;
                ta.PRI = prj.PRI;
                ta.TaskSta = TaskSta.Todolist; //设置启动.
                ta.RDT = DataType.CurrentDateTime;
                ta.ADT = DataType.CurrentDateTime;
                ta.Update();

                //开始创建任务.
                wl.setMyPK(ta.TaskID + "_" + ta.EmpNo);
                wl.EmpNo = ta.EmpNo; //工作人员.
                wl.EmpName = ta.EmpName;

                wl.PrjNo = ta.PrjNo; //项目信息.
                wl.PrjName = prj.Name;

                wl.SenderNo = WebUser.No; //发送人.
                wl.SenderName = WebUser.Name;
                wl.RDT = DataType.CurrentDateTime;  //记录日期
                wl.ADT = DataType.CurrentDateTime; //活动日期.
                wl.Insert(); //插入任务.

                msg += "节点:" + ta.NodeName + " 人员:" + ta.EmpName;

                TaskAPI.Port_WriteTrack(ta.PrjNo, ta.OID, "Alte", "工作分配", "任务[" + ta.Title + "]分配给[" + ta.EmpName + "]", WebUser.No, WebUser.Name);
            }

            prj.Msg = msg; //更新消息.
            prj.NumComplete = 0;
            prj.NumTasks = tas.Count;
            prj.Update();

            BP.TA.TaskAPI.Port_WriteTrack(prjNo, 0, "Start", "项目启动", msg, WebUser.No, WebUser.Name);
            return "启动成功:" + msg;
        }
        /// <summary>
        /// 设置完成
        /// </summary>
        /// <param name="prjNo">项目编号</param>
        /// <returns>执行结果</returns>
        public static string Prj_Complete(string prjNo)
        {
            DBAccess.RunSQL("UPDATE TA_Project SET PrjSta=" + PrjSta.Complete + ",WCL=100 WHERE No='" + prjNo + "'");
            DBAccess.RunSQL("UPDATE TA_Task SET TaskSta=" + TaskSta.WorkOver + " WHERE PrjNo='" + prjNo + "'");

            BP.TA.TaskAPI.Port_WriteTrack(prjNo, 0, "Complete", "完成", "项目完成.", WebUser.No, WebUser.Name);
            return "项目完成.";
        }
        /// <summary>
        /// 物理删除
        /// </summary>
        /// <param name="prjNo">项目编号</param>
        /// <returns>执行结果</returns>
        public static string Prj_DeleteByRel(string prjNo)
        {
            DBAccess.RunSQL("DELETE FROM TA_Project WHERE No='" + prjNo + "'");
            DBAccess.RunSQL("DELETE FROM TA_Task WHERE PrjNo='" + prjNo + "'");
            DBAccess.RunSQL("DELETE FROM TA_Track WHERE PrjNo='" + prjNo + "'");
            return "删除成功.";
        }
        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="prjNo"></param>
        /// <returns></returns>
        public static string Prj_DeleteByFlag(string prjNo)
        {
            DBAccess.RunSQL("UPDATE TA_Project SET PrjSta=7 WHERE No='" + prjNo + "'");
            DBAccess.RunSQL("UPDATE TA_Task SET TaskSta=7 WHERE PrjNo='" + prjNo + "'");

            BP.TA.TaskAPI.Port_WriteTrack(prjNo, 0, "DeleteByFlag", "逻辑删除", "逻辑删除.", WebUser.No, WebUser.Name);
            return "删除成功.";
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="templateNo">模板编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="actionType">活动类型</param>
        /// <param name="actionTypeName">活动名称</param>
        /// <param name="docs">执行内容</param>
        public static void Port_WriteTrack(string prjNo, int workid, string actionType, string actionTypeName, string docs, string empNo, string empName, int wcl = 0, int useHH = 0, int useMM = 0)
        {
            Track ta = new Track();
            ta.setMyPK(DBAccess.GenerGUID());
            ta.TaskID = workid;
            ta.PrjNo = prjNo;

            if (empNo == null)
                empNo = WebUser.No;
            if (empName == null)
                empName = WebUser.Name;

            //当事人.
            ta.EmpNo = empNo;
            ta.EmpName = empName;


            ta.ActionType = actionType;
            ta.ActionName = actionTypeName;
            ta.Docs = docs;
            ta.RecNo = WebUser.No;
            ta.RecName = WebUser.Name;
            ta.RDT = DataType.CurrentDateTime;

            ta.UseHH = useHH;
            ta.UseMM = useMM;

            ta.Insert();
        }
        #endregion 项目


        #region 任务

        /// <summary>
        /// 任务审核
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="checkResult">审核结果0=不通过,1=通过.</param>
        /// <param name="docs">审核信息</param>
        /// <returns></returns>
        public static string Task_CheckSubmit(Int64 taskID, int checkResult, string docs)
        {
            Task ta = new Task(taskID);
            if (ta.TaskSta == TaskSta.SubmitChecking)
                return "err@当前不是待审核状态，您不能执行该操作.";

            //不通过.
            if (checkResult == 0)
            {
                TaskAPI.Port_WriteTrack(ta.PrjNo, ta.OID, "TaskSubmit", "任务审核", "不通过,意见:" + docs, WebUser.No, WebUser.Name);
                ta.TaskSta = TaskSta.ReTodolist;
                ta.WCL = 50; //完成率设置100%.
            }
            else
            {
                TaskAPI.Port_WriteTrack(ta.PrjNo, ta.OID, "TaskSubmit", "任务审核", "通过,意见:" + docs, WebUser.No, WebUser.Name);
                ta.TaskSta = TaskSta.ReTodolist;
            }
            ta.NowMsg = docs;
            ta.ADT = DataType.CurrentDateTime; //活动日期.
            ta.Update();
            return "审核提交成功.";
        }
        public static string Task_CheckReturn(Int64 taskID, int checkResult, string docs, string shiftEmpNo)
        {
            Task ta = new Task(taskID);
            if (ta.TaskSta != TaskSta.ReturnWork)
                return "err@当前不是退回审核状态，您不能执行该操作.";

            //不通过.
            if (checkResult == 0)
            {
                TaskAPI.Port_WriteTrack(ta.PrjNo, ta.OID, "CheckReturn", "任务退回审核", "不通过,意见:" + docs, WebUser.No, WebUser.Name);
                ta.TaskSta = TaskSta.ReTodolist;
                //  ta.WCL = 0; //完成率设置100%.
            }
            //同意.
            if (checkResult == 1)
            {
                TaskAPI.Port_WriteTrack(ta.PrjNo, ta.OID, "CheckReturn", "任务退回审核", "通过,意见:" + docs, WebUser.No, WebUser.Name);
                ta.TaskSta = TaskSta.WorkOver;
                ta.WCL = 0; //完成率设置未 0 .
            }
            //移交给其他人.
            if (checkResult == 2)
                return "err@该功能还没有完成.";

            ta.NowMsg = docs;
            ta.ADT = DataType.CurrentDateTime; //活动日期.
            ta.Update();

            return "退回审核提交成功.";
        }

        /// <summary>
        /// 任务退回
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="docs">退回原因</param>
        /// <returns>退回结果</returns>
        public static string Task_Return(Int64 taskID, string docs)
        {
            Task ta = new Task(taskID);
            if (ta.TaskSta == TaskSta.ReturnWork)
                return "err@当前已经是退回,不能重复执行.";

            ta.WCL = 0; //完成率设置 0 .
            ta.TaskSta = TaskSta.ReturnWork; //让其检查.
            ta.ADT = DataType.CurrentDateTime; //活动日期.
            ta.Update();

            TaskAPI.Port_WriteTrack(ta.PrjNo, ta.OID, "ReturnWork", "退回", "退回原因:" + docs, WebUser.No, WebUser.Name);

            return "已经退回给[" + ta.SenderName + "]，请等待他批准。";
        }
        /// <summary>
        /// 移交给指定的人
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="toEmpNo"></param>
        /// <param name="docs"></param>
        /// <returns></returns>
        public static string Task_Shift(Int64 taskID, string toEmpNo, string docs)
        {
            Emp emp = new Emp(toEmpNo);

            Task ta = new Task(taskID);
            ta.EmpNo = toEmpNo;
            ta.EmpName = emp.Name;

            ta.TaskSta = TaskSta.Todolist;

            ta.WCL = 0; //完成率设置 0 .
            ta.TaskSta = TaskSta.Todolist; //设置待办..
            ta.ADT = DataType.CurrentDateTime; //活动日期.
            ta.NowMsg = "移交原因:[" + docs + "]移交人:[" + WebUser.Name + "]移交日期:[" + ta.ADT + "]";
            ta.Update();

            TaskAPI.Port_WriteTrack(ta.PrjNo, ta.OID, "Shift", "移交", ta.NowMsg, WebUser.No, WebUser.Name);

            return "移交[" + emp.Name + "]";
        }
        public static string Task_HuiBao(Int64 taskID, string docs1, string docs2, int wcl, int hh, int mm)
        {
            Task ta = new Task(taskID);
            ta.WCL = 0; //完成率设置 0 .
            if (wcl == 100)
            {
                ta.TaskSta = TaskSta.SubmitChecking; //提交状态,让其检查.
                ta.NowMsg = docs1;
            }
            else
            {
                ta.TaskSta = TaskSta.Todolist;
                ta.NowMsg = "工作内容:[" + docs1 + "]计划:[" + docs2 + "]";
            }

            ta.ADT = DataType.CurrentDateTime; //活动日期.
            ta.Update();


            if (wcl == 100)
            {
                TaskAPI.Port_WriteTrack(ta.PrjNo, ta.OID, "TaskSubmit", "任务提交", ta.NowMsg, WebUser.No, WebUser.Name, wcl, hh, mm);
                return "汇报给[" + ta.SenderName + "]";
            }
            else
            {
                TaskAPI.Port_WriteTrack(ta.PrjNo, ta.OID, "HuiBao", "汇报工作", ta.NowMsg, WebUser.No, WebUser.Name, wcl, hh, mm);
                return "任务提交给:[" + ta.SenderName + "]审核.";
            }

        }
        #endregion 任务

    }
}
