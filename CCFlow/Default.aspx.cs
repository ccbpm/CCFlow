using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;
using BP.En;
using BP.Port;
using BP.DA;
using BP.Sys;
using BP.WF;
using BP.WF.Template;
using System.Drawing;
using BP.WF.Port;
using BP.Difference;
using CCFlow.DataUser.API;

namespace CCFlow
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          //  EventDoType.Disable
           // DataType.AppBoolean
            //  BP.Sys.SFTable
            //  EnConfig
            //  string str=  QuanYiSoft.ClientAPISAAS.Port_Login("ccs");
            //     this.Response.Write(str);
            //  BP.Sys.SFTable
            //  this.Response.Write("OSModel=" + SystemConfig.CCBPMRunModel.ToString());
            return;
            //    FlowDevModel.JiJian
            //  BP.WF.Dev2Interface.Node_DoCCCheckNote
            // WFEmp emp = new WFEmp();

            //   objs.ToJson
            //BP.WF.Dev2Interface.Port_Login("admin");
            //string path = "D:\\bpmn20.xml";
            //BP.WF.Template.TemplateGlo.NewFlowByBPMN("001", path);
            //this.Response.Write(flowNo);
            //return;

            // DataTable Diagram = ds.Tables[""]
            ////登陆.
            //BP.WF.Dev2Interface.Port_Login("zhangsan", "wz");

            //string url = BP.WF.Dev2Interface.Port_GenerToken();

            ////登陆by token.
            //BP.WF.Dev2Interface.Port_LoginByToken("xxx");

            ////创建一个空白的workid. 实例.
            //Int64 workID = Dev2Interface.Node_CreateBlankWork("QingJia");
            //Int64 work2ID = Dev2Interface.Node_CreateBlankWork("001");

            ////保存参数.
            //Hashtable ht = new Hashtable();
            //// ht.Add("QingJiaTianShu", 100);
            //ht.Add("ABC", 100);
            //Dev2Interface.Node_SaveWork("001", 101, workID, ht);
            ////
            //Dev2Interface.Node_SendWork("001", workID);

            ////执行发送.
            //Dev2Interface.Node_SendWork("001", workID, 109, "lisi");

            return;
            //BP.WF.Dev2Interface.Port_Login("admin");
            ////for (int i = 0; i < 200; i++)
            ////{
            ////    BP.Port.Emp emp = new Emp();
            ////    emp.No = DBAccess.GenerGUID();
            ////    emp.Name = emp.No;
            ////    emp.FK_Dept = "1001";
            ////    emp.Insert();
            ////}

            //DateTime dt = DateTime.Now;
            //Int64 workID = Dev2Interface.Node_CreateBlankWork("021");
            ////  BP.WF.Dev2Interface.Node_SaveWork("021", 2101, workID);
            //string sql = "SELECT No,Name FROM Port_Emp ";
            //DataTable DT = DBAccess.RunSQLReturnTable(sql);

            //string empNos = "";
            //foreach (DataRow dr in DT.Rows)
            //{
            //    empNos += dr[0].ToString() + ",";
            //}
            //string strs = BP.WF.Dev2Interface.Node_SendWork("021", workID, null, null, 0, empNos).ToMsgOfHtml();


            //DateTime dt1 = DateTime.Now;
            //TimeSpan ts = dt1 - dt;
            //this.Response.Write(strs +" <hr>"+ts.Milliseconds);

            // string sqls = " update WF_GenerWorkerlist set fk_emp=fk_emp  where 1=1 ; update WF_GenerWorkerlist set fk_emp=fk_emp  where 1=1 ;";
            //  DBAccess.RunSQL(sqls);
            //  BP.Web.WebUser.IsAdmin
            //   BP.WF.Port.Admin2Group.Dept
            //  BP.WF.Port.AdminGroup.Orgs
            //    BP.WF.Dev2Interface.Port_Login("admin");
            // BP.WF.Dev2Interface.Port_LoginByToken();
            return;
            //BP.WF.Dev2Interface.Port_Login("admin");
            //Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("001");
            //var dirs = BP.WF.Dev2Interface.Node_GetNextStepNodesByNodeID(101);
            //Direction  dir = dirs[0] as Direction;
            //var dt = BP.WF.Dev2Interface.Node_GetNextStepEmpsByNodeID(dir.ToNode,workid);
            //string js3on = BP.Tools.Json.ToJson(dt);
            //this.Response.Write(js3on);
            // BP.WF.Dev2Interface.Node_GetNextStepNode
            //BP.WF.Dev2Interface.Port_Login("admin");
            ////创建一个项目编号, 根据模板编号.
            //string prjNo = BP.TA.TaskAPI.Prj_CreateNo("009");
            ////发起项目.
            //BP.TA.TaskAPI.Prj_Start(prjNo);
            //BP.TA.TaskAPI.Prj_Start(prjNo);
            //WFState.Blank
            // BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork("001", 10022); //发送成功后返回一个对象.
            // var toNodeID = objs.VarToNodeID; //到达的节点IDs, 比如： 106
            // var toEmpIDs = objs.VarAcceptersID; //到达的人员ID, 比如; zhangsan,lisi
            // var aletMsg = objs.ToMsgOfHtml(); //提示信息. 当前工作已完成, 下一步发送给xxx, 发送到xxxx.
            //string valJson = BP.Tools.Json.ToJson(objs);  // 把这个对象组成一个Json. 
            return;

            BP.TA.Track tk = new BP.TA.Track();
            tk.CheckPhysicsTable();

            BP.DA.DBAccess.RunSQL("DELETE FROM TA_Project ");
            BP.DA.DBAccess.RunSQL("DELETE FROM TA_Task ");
            BP.DA.DBAccess.RunSQL("DELETE FROM TA_WorkerList ");
            BP.DA.DBAccess.RunSQL("DELETE FROM TA_Track ");

            //for (int i = 0; i < 10; i++)
            //{
            //    //创建一个项目编号, 根据模板编号.
            //    string prjNo = BP.TA.TaskAPI.Prj_CreateNo("009");

            //    //发起项目.
            //    BP.TA.TaskAPI.Prj_Start(prjNo);
            //}
            return;

            //   en.
            //BP.WF.Dev2Interface.Port_Login("admin");
            //BP.WF.Flows ens = new Flows();
            //ens.RetrieveAll();
            //ens.GetNewEntity.CheckPhysicsTable();

            //foreach (Flow fl in ens)
            //{
            //    GERpt rpt = fl.HisGERpt;
            //    rpt.CheckPhysicsTable();
            //}

            // Flow
            //  WorkFlow
            //登录.
            //DateTime dt = System.DateTime.Now;
            //BP.WF.Dev2Interface.Port_Login("admin");
            //for (int i = 0; i < 100; i++)
            //{
            //    //创建workid.
            //    Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("016");

            //    //执行发送.
            //    BP.WF.Dev2Interface.Node_SendWork("016", workid, 1603,"admin");

            //    //执行结束.
            //    BP.WF.Dev2Interface.Flow_DoFlowOver(workid, "ss", 0);
            //}
            //DateTime dt1 = System.DateTime.Now;

            //TimeSpan ts = dt1 - dt;
            //this.Response.Write(" = "+ts.TotalSeconds+": xxxxxxxxxxxxxxxxxxxx");
            return;


            ////1. 循环所有的业务数据.
            //DataTable dt = DBAccess.RunSQLReturnTable("SELECT * FROM XXXX");

            ////2. 遍历所有的数据.
            //foreach (DataRow item in dt.Rows)
            //{
            //    //2.1 获取业务主键,该业务主键.
            //    string buessPK = "111";

            //    //2.2 检查是否已经发起,该流程.
            //    if (1 == 1)
            //        continue; //如果有该笔数据，conctinue.

            //    //2.3 创建一个workid
            //    Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("001", BP.Web.WebUser.No);

            //    //2.4 更新GenerWorkFlow.
            //    GenerWorkFlow gwf = new GenerWorkFlow(workid);
            //    gwf.WFState = WFState.Complete;
            //    gwf.Title = "设置标题.";
            //    gwf.Starter = "发起人";
            //    gwf.StarterName = "发起人name";
            //    gwf.Update();

            //    //2.5 写拉NDxxxRpt 流程业务表里.

            //    //2.6 获得自己的流程日志.
            //    DataTable dtLog = DBAccess.RunSQLReturnTable("SELECT ");

            //    //2.7 写入日志.
            //    foreach (DataRow mydr in dtLog.Rows)
            //    {

            //    }
            //}
            ////  BP.WF.DeliveryWay.BySpecNodeEmp
            //return;
            //BP.WF.Dev2Interface.Port_Login("admin");

            ////创建workid.
            //Int64 workID = BP.WF.Dev2Interface.Node_CreateBlankWork("013");

            ////修改主表数据.
            //GEEntity frm = new GEEntity("Frm_YanShouDAN");
            //frm.OID = workID;
            //frm.RetrieveFromDBSources();
            //frm.SetValByKey("XiangMuBianHao", "xxxxxxxxxxxxxxx");
            //frm.SetValByKey("SQR", "周朋");
            //frm.SetValByKey("Tel", "xxxx周朋");
            //frm.Update();

            ////插入从表数据.
            //GEDtl frmDtl = new GEDtl("Frm_YanShouDANDtl1");
            //frmDtl.SetValByKey("RefPK", workID);
            //frmDtl.SetValByKey("xxx", "xxx");
            //frmDtl.OID = 0;
            //frmDtl.Insert();

            //frmDtl = new GEDtl("Frm_YanShouDANDtl1");
            //frmDtl.SetValByKey("RefPK", workID);
            //frmDtl.SetValByKey("xxx", "xxx");
            //frmDtl.OID = 0;
            //frmDtl.Insert();


            ////执行发送.
            //SendReturnObjs obj= BP.WF.Dev2Interface.Node_SendWork("013", workID, 1305, "lisi");

            //SendReturnObjs obj = BP.WF.Dev2Interface.Node_SendWork("013", workID, 0, "lisi");
            //SendReturnObjs obj = BP.WF.Dev2Interface.Node_SendWork("013", workID, 0, "");

            //string smg= obj.ToMsgOfHtml();

            return;
            //string cfg = "@EdityType=1@EnName=BP.TXS.XXX";
            //AtPara ap = new AtPara(cfg);
            //string enName = ap.GetValStrByKey("EnName");
            //int editType = ap.GetValIntByKey("EdityType");
            //return;

            //string msg = "";
            ////    string flowNo = "080";
            //string starter = "ht1";

            //BP.WF.Dev2Interface.Port_Login(starter);
            //msg += "@用户:" + starter + "已经安全登录.";

            //Int64 workID = BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo);
            //msg += "@创建的WorkID:" + workID + ".";

            //////构造参数。
            ////Hashtable ht = new Hashtable();
            ////ht.Add("ZuoWuJingLi1", "xxx,xxx,ccc,xx");//作物经理.
            ////ht.Add("JiShuJingLi", "xxx,xxx,ccc,xx");//技术经理..


            ////发送到节点2, 本级：技术经理岗位, 如果没有就跳转到节点3区域经理节点.
            //SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workID);
            //msg += "@发送到了:" + objs.VarToNodeID + ", " + objs.VarToNodeName + ", " + objs.VarAcceptersID + ":" + objs.VarAcceptersName;
            //msg += "发送消息:" + objs.ToMsgOfText();
            //if (objs.VarToNodeID != 6102)
            //    msg += "@<font color=red>已经执行了跳转，跳转到[" + objs.VarToNodeID + " - " + objs.VarToNodeName + "]</font>";

            ////岗位齐全模式. dbhttsy 东部区域B, 岗位不全.
            ////string msg = this.Case1("dbhttsy");
            ////msg = msg.Replace("@", "<br>@");
            ////this.Response.Write(msg);
            return;

            ////岗位齐全模式， 测试通过.
            //string msg = this.Case1("httsy");
            //msg = msg.Replace("@", "<br>@");
            //this.Response.Write(msg);
            //return;

            //F   BP.WF.Dev2Interface.Flow_DoRebackWorkFlow
            //组织参数.
            //Hashtable ht = new Hashtable();
            //ht.Add("PrjNo", "项目编号");
            //ht.Add("PrjName", "项目名称");
            //ht.Add("JinE", 500000.00); //项目金额, 根据项目金额大小自动转向.
            ////调用发送接口.
            //BP.WF.Dev2Interface.Node_SendWork("001", 1002, ht, null);


            // WFState.Complete
            return;
            //string userNo = "zhangsan";
            //BP.WF.Dev2Interface.Port_Login(userNo); //让张三登录。
            //// 生成当前登录的token.
            //string token = BP.WF.Dev2Interface.Port_GenerToken("PC");
            ////使用token登录.
            //BP.WF.Dev2Interface.Port_LoginByToken(token);
            ////退出当前登录.
            //BP.WF.Dev2Interface.Port_SigOut();
            //BP.WF.WFState
            //BP.Web.WebUser.No
            return;

            BP.WF.Glo.UpdataCCFlowVer();
            return;

            if (BP.Difference.SystemConfig.CustomerNo.Equals("PM") == true)
            {
            }
            //return;
            //BP.Port.Emp emp = new Emp();
            //emp.Name = "张三";
            //emp.No = "009";
            //emp.FK_Dept = "100";
            //emp.Insert();
            ////BP.Demo.BanJi en = new BP.Demo.BanJi();
            ////en.Name = "1年级";
            ////en.BZR = "张三";
            ////en.Insert();
            //return;

            return;
            BP.WF.Dev2Interface.Port_Login("admin");
            BP.Sys.MapDtl md = new MapDtl("ND102Dtl1");
            string json = md.ToJson();
            return;
            //   BP.En.ClassFactory.GetEn
            //   BP.WF.DeliveryWay
            //string name = "liuwei";  NewFLow
            //if (name.Equals("liuwei") == true)
            //{
            //    BP.WF.Dev2Interface.Port_Login("admin");
            //    string ur13l = "/WF/CCForm/ChapterFrm.htm?FrmID=Frm_ZhangJieBiaoDAN&OID=100";
            //    this.Response.Redirect(ur13l, true);
            //    return;
            //}
            //BP.WF.Dev2Interface.Port_Login("admin");
            //BP.Port.Depts ens = new BP.Port.Depts();
            //ens.RetrieveAll();
            //foreach (BP.Port.Dept item in ens)
            //{
            //    item.DoResetPathName();
            //    item.Update();
            //}
            //return;
        }

        public void JianYuStations()
        {
            return;
            BP.Port.Emps ens = new Emps();
            ens.RetrieveAll();

            DBAccess.RunSQL("DELETE FROM Port_DeptEmp ");
            DBAccess.RunSQL("DELETE FROM Port_DeptEmpStation ");

            foreach (Emp item in ens)
            {
                BP.Port.DeptEmp de = new BP.Port.DeptEmp();
                de.FK_Emp = item.No;
                de.FK_Dept = item.No;
                de.Insert();
                BP.Port.DeptEmpStation des = new BP.Port.DeptEmpStation();
                if (item.Name.Contains("分监区") == true)
                {

                    des.FK_Dept = item.No;
                    des.FK_Emp = item.No;
                    des.FK_Station = "98";
                    des.Insert();
                    continue;
                }

                if (item.Name.Contains("监区") == true)
                {
                    des.FK_Dept = item.No;
                    des.FK_Emp = item.No;
                    des.FK_Station = "97";
                    des.Insert();
                    continue;
                }


                if (item.Name.Contains("刑罚") == true)
                {
                    des.FK_Dept = item.No;
                    des.FK_Emp = item.No;
                    des.FK_Station = "6";
                    des.Insert();
                    continue;
                }

                des.FK_Dept = item.No;
                des.FK_Emp = item.No;
                des.FK_Station = "19";
                des.Insert();
            }


        }
        /// <summary>
        /// 合同提审员-东部B .
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public string Case1(string userNo)
        {


            //GEEntity en = new GEEntity("Frm_CheLiang");
            //en.Name = "xxx";
            //en.Tel = "xxx";
            //en.Insert();

            //BP.Port.Station sta = new Station();
            //sta.Name = "xxx";
            //sta.OrgNo = "ss";
            //sta.Update();

            //BP.Port.Stations ens = new Stations();
            //ens.Retrieve();


            //GEEntity en = new GEEntity("Frm_CheLiang","zhoupeng");
            //en.Tel = "sss";
            //en.Update();
            ////xx
            //en.Delete();


            //BP.Web.WebUser.OrgNo
            //BP.Port.Emp emp = new Emp();
            //emp.Retrieve();

            // BP.En.ClassFactory.GetEn
            // BP.WF.Glo.up
            //BP.WF.Dev2Interface.Flow_DoPress

            string msg = "";
            string flowNo = "061";
            string userID = "zhangsan";
            BP.WF.Dev2Interface.Port_Login(userID);
            msg += "@用户:" + userNo + "已经安全登录.";

            Int64 workID = BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo);
            msg += "@创建的WorkID:" + workID + ".";

            //发送到节点2, 本级：技术经理岗位, 如果没有就跳转到节点3区域经理节点.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workID);

            ////Hashtable ht = new Hashtable();
            ////ht.Add("JE", 1000);
            ////SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workID, ht);

            //SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workID,6109,"lisi");

            //BP.WF.Dev2Interface.Port_Login("lisi");

            ////删除的.
            //BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(workID);

            ////结束.
            //BP.WF.Dev2Interface.Flow_DoFlowOver(workID, 0);

            ////sdssds
            //BP.WF.Dev2Interface.Node_Shift(workID, "wangwu,zhaowuli,sss", "sdsdfsdfsdfsd");
            ////ssss
            //BP.WF.Dev2Interface.Node_ReturnWork(workID, "wangwu,zhaowuli,s", "102","ssdsds",false);

            //SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workID, 6109, "");
            //SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workID, 0, "");

            msg += "@发送到了:" + objs.VarToNodeID + ", " + objs.VarToNodeName + ", " + objs.VarAcceptersID + ":" + objs.VarAcceptersName;
            msg += "发送消息:" + objs.ToMsgOfText();
            if (objs.VarToNodeID != 6102)
                msg += "@<font color=red>已经执行了跳转，跳转到[" + objs.VarToNodeID + " - " + objs.VarToNodeName + "]</font>";

            // 第2个节点:
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);
            msg += "@节点2用户:" + objs.VarAcceptersID + "已经安全登录.";

            objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workID);
            msg += "@发送到了:" + objs.VarToNodeID + ", " + objs.VarToNodeName + ", " + objs.VarAcceptersID + ":" + objs.VarAcceptersName;
            msg += "发送消息:" + objs.ToMsgOfText();

            // 第3个节点:
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);
            msg += "@节点3用户:" + objs.VarAcceptersID + "已经安全登录.";

            objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workID);
            msg += "@发送到了:" + objs.VarToNodeID + ", " + objs.VarToNodeName + ", " + objs.VarAcceptersID + ":" + objs.VarAcceptersName;
            msg += "发送消息:" + objs.ToMsgOfText();

            if (objs.VarAcceptersID == null || objs.IsStopFlow == true)
                return msg += "@流程已经结束.";

            // 第4个节点:
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);
            msg += "@节点3用户:" + objs.VarAcceptersID + "已经安全登录.";
            objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workID);
            msg += "@发送到了:" + objs.VarToNodeID + ", " + objs.VarToNodeName + ", " + objs.VarAcceptersID + ":" + objs.VarAcceptersName;
            msg += "发送消息:" + objs.ToMsgOfText();

            if (objs.VarAcceptersID == null || objs.IsStopFlow == true)
                return msg += "@流程已经结束.";

            if (objs.VarAcceptersID.Equals(userNo) == false)
                msg += "@系统错误,应该返回到开始人:" + userNo + ",现在发送给了," + objs.VarAcceptersID;

            return msg;
        }
        /// <summary>
        /// 二级管理员同步
        /// </summary>
        public void DTSOrg()
        {
            string orgNo = BP.Web.WebUser.OrgNo;
            //1. 删除全部的数据.
            string sql = "DELETE FROM Port_Dept_TB WHERE OrgNo='" + orgNo + "'";
            DBAccess.RunSQL(sql);
            sql = "DELETE FROM Port_Emp_TB WHERE OrgNo='" + orgNo + "'";
            DBAccess.RunSQL(sql);
            sql = "DELETE FROM Port_DeptEmp_TB WHERE OrgNo='" + orgNo + "'";
            DBAccess.RunSQL(sql);

            //2. 把获得数据写入到这三个表.
            //3. 开始写入人员数据表.
            //3.1 更新部门,查询出来新增部分.
            sql = "SELECT * FROM Port_Dept_TB WHERE No Not IN (SELECT No from Port_Dept where OrgNo='" + orgNo + "') AND OrgNo='" + orgNo + "'";
            DataTable dtDept = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dtDept.Rows)
            {
                BP.Port.Dept dept = new BP.Port.Dept();
                dept.No = dr[0].ToString();
                dept.Name = dr[1].ToString();
                dept.ParentNo = dr[2].ToString();
                dept.Insert();
            }
            //3.2 查询出来删除的.
            sql = "SELECT * FROM Port_Dept WHERE No Not IN ( SELECT No from Port_Dept_TB where OrgNo='" + orgNo + "') AND OrgNo='" + orgNo + "'";
            dtDept = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dtDept.Rows)
            {
                BP.Port.Dept dept = new BP.Port.Dept();
                dept.No = dr[0].ToString();
                dept.Delete();
            }
            //3.3 查询出来名称变化的.
            sql = "SELECT A.No,A.Name FROM Port_Dept_TB A, Port_Dept B WHERE A.No=B.No AND A.Name!=B.Name ";
            dtDept = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dtDept.Rows)
            {
                BP.Port.Dept dept = new BP.Port.Dept();
                dept.No = dr[0].ToString();
                dept.Retrieve();
                dept.Name = dr[1].ToString();
                dept.Update();
            }

            //4. 同步人员数据.
            //4.1 更新部门,查询出来新增部分.
            sql = "SELECT * FROM Port_Emp_TB WHERE No Not IN (SELECT No from Port_Emp where OrgNo='" + orgNo + "') AND OrgNo='" + orgNo + "'";
            DataTable dtEmp = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dtEmp.Rows)
            {
                Emp emp = new Emp();
                emp.No = dr[0].ToString();
                emp.Name = dr[1].ToString();
                emp.OrgNo = dr[2].ToString();
                emp.Pass = "";
                emp.Insert();
            }
            //4.2 查询出来删除的.
            sql = "SELECT * FROM Port_Emp WHERE No Not IN ( SELECT No from Port_Emp_TB where OrgNo='" + orgNo + "') AND OrgNo='" + orgNo + "'";
            dtEmp = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dtEmp.Rows)
            {
                Emp emp = new Emp();
                emp.No = dr[0].ToString();
                emp.Delete();
            }
            //4.3 查询出来名称变化的.
            sql = "SELECT A.No,A.Name FROM Port_Emp_TB A, Port_Emp B WHERE A.No=B.No AND A.Name!=B.Name ";
            dtEmp = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dtEmp.Rows)
            {
                Emp emp = new Emp();
                emp.No = dr[0].ToString();
                emp.Retrieve();
                emp.Name = dr[1].ToString();
                emp.Update();
            }


            //5. 同步人员部门数据.
            //5.1 更新部门,查询出来新增部分.
            sql = "SELECT * FROM Port_DeptEmp_TB WHERE MyPK Not IN (SELECT MyPK from Port_DeptEmp WHERE OrgNo='" + orgNo + "') AND OrgNo='" + orgNo + "'";
            DataTable dtDeptEmp = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dtDeptEmp.Rows)
            {
                DeptEmp de = new DeptEmp();
                de.MyPK = dr[0].ToString();
                de.FK_Emp = dr[1].ToString();
                de.FK_Dept = dr[2].ToString();
                de.OrgNo = dr[3].ToString();
                de.Insert();
            }
            //5.2 查询出来删除的.
            sql = "SELECT * FROM Port_DeptEmp WHERE No Not IN ( SELECT No from Port_DeptEmp_TB where OrgNo='" + orgNo + "') AND OrgNo='" + orgNo + "'";
            dtDeptEmp = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dtDeptEmp.Rows)
            {
                DeptEmp de = new DeptEmp();
                de.MyPK = dr[0].ToString();
                de.Delete();
            }

        }
        public void TestCash()
        {
            /* BP.WF.Node nd = new Node(101);
             string html = "";
             foreach (Attr item in nd.EnMap.Attrs)
             {
                 html += "@" + item.Key + " - " + nd.GetValStrByKey(item.Key);
             }
             this.Response.Write(html);
             nd.Name = "abc123";
             nd.Update();

             html = "<hr>";
             foreach (Attr item in nd.EnMap.Attrs)
             {
                 html += "@" + item.Key + " - " + nd.GetValStrByKey(item.Key);
             }
             this.Response.Write(html);*/
        }
    }
}