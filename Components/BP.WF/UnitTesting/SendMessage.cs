using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.En;
using BP.DA;
using BP.Sys;
using BP.Web;
using System.Data;
using BP.UnitTesting;

namespace BP.UnitTesting
{
    /// <summary>
    /// 消息发送机制测试
    /// </summary>
    public class SendMessage : TestBase
    {
        #region 变量
        /// <summary>
        /// 流程编号
        /// </summary>
        public string fk_flow = "";
        /// <summary>
        /// 用户编号
        /// </summary>
        public string userNo = "";
        /// <summary>
        /// 所有的流程
        /// </summary>
        public Flow fl = null;
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 workID = 0;
        /// <summary>
        /// 发送后返回对象
        /// </summary>
        public SendReturnObjs objs = null;
        /// <summary>
        /// 工作人员列表
        /// </summary>
        public GenerWorkerList gwl = null;
        /// <summary>
        /// 流程注册表
        /// </summary>
        public GenerWorkFlow gwf = null;
        /// <summary>
        /// 发起人
        /// </summary>
        public BP.Port.Emp starterEmp = null;
        #endregion 变量

        /// <summary>
        /// 消息发送机制测试
        /// </summary>
        public SendMessage()
        {
            this.Title = "消息发送机制测试";
            this.DescIt = "流程:002请假流程(内部人员),055学生请假流程(内外部人员都有).";
            this.EditState = EditState.Editing; //已经能过.
        }

        /// <summary>
        /// 过程说明：
        /// 1，以流程 001 来测试发送流程.
        /// 2，仅仅测试发送功能，与检查发送后的数据是否完整.
        /// 3, 此测试有三个节点发起点、中间点、结束点，对应三个测试方法。
        /// </summary>
        public override void Do()
        {
            //预警.
            BP.WF.Dev2Interface.DTS_GenerWorkFlowTodoSta();


            //测试发送短消息是否。
            this.Test001_liyan_Start();

            //测试学生请假流程.
            this.Test055();

            //测试请假流程，在没有设置任何消息机制的情况下，发送默认的消息, zhanghaicheng登录，
            this.Test002();

            // 测试工作到达短消息
            this.Test002_zhoupeng_liping();

          
        }
        /// <summary>
        /// 测试让liyan发起，发送并产生一个发送成功后的短消息.
        /// </summary>
        public void Test001_liyan_Start()
        {
            //删除消息. 
            BP.DA.DBAccess.RunSQL("DELETE FROM Sys_SMS");

            // 让zhoupeng登录.
            BP.WF.Dev2Interface.Port_Login("liyan");

            //创建空白工作, 发起开始节点.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork("001");

            //执行发送工作.
            BP.WF.Dev2Interface.Node_SendWork("001", workID, null, null);


            #region 检查消息是否 符合预期.
            BP.WF.SMSs smss = new SMSs();
            smss.RetrieveAllFromDBSource();
            if (smss.Count != 1)
                throw new Exception("@应该产生 1 条，现在产生了" + smss.Count + "条。");

            foreach (BP.WF.SMS item in smss)
            {
                if (item.MsgType == EventListOfNode.SendSuccess)
                {
                    if (item.HisMobileSta != MsgSta.UnRun)
                        throw new Exception("@应该在工作到达的事件里产生一条短信息.");
                }
                else
                {
                    throw new Exception("@短消息的标记错误."+item.MsgType);
                }
            }
            #endregion 检查消息是否 符合预期.

        }
        /// <summary>
        /// 测试工作到达短消息
        /// </summary>
        public void Test002_zhoupeng_liping()
        {
            //删除消息. 
            BP.DA.DBAccess.RunSQL("DELETE FROM Sys_SMS");

            // 让zhoupeng登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //创建空白工作, 发起开始节点.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork("002");
        
            //执行发送工作.
            BP.WF.Dev2Interface.Node_SendWork("002", workID, null, null);

            #region 检查消息是否 符合预期.
            BP.WF.SMSs smss = new SMSs();
            smss.RetrieveAllFromDBSource();
            if (smss.Count == 0)
                throw new Exception("@执行了发送，应该产生消息，而没有产生。");
            if (smss.Count != 2)
                throw new Exception("@应该产生2 条，现在产生了" + smss.Count + "条。");

            foreach (BP.WF.SMS item in smss)
            {
                if (item.MsgType == EventListOfNode.SendSuccess)
                {
                    if (item.HisEmailSta != MsgSta.UnRun)
                        throw new Exception("@应该在发送成功的事件里产生一条邮件信息.");
                }

                if (item.MsgType == EventListOfNode.WorkArrive)
                {
                    if (item.HisMobileSta != MsgSta.UnRun)
                        throw new Exception("@应该在工作到达的事件里产生一条短信息.");
                }
            }
            #endregion 检查消息是否 符合预期.

        }

        /// <summary>
        /// 测试学生请假流程.
        /// </summary>
        public void Test055()
        {
            //先执行一次流程检查.
            BP.WF.Flow fl = new Flow("055");
            fl.DoCheck();

            //让客户登录 0001登录，光头强.
            BP.WF.Dev2InterfaceGuest.Port_Login("0001", "光头强");
            //删除消息. 
            BP.DA.DBAccess.RunSQL("DELETE FROM Sys_SMS");

            //创建workid.
            workID = BP.WF.Dev2InterfaceGuest.Node_CreateBlankWork("055", null, null, BP.Web.GuestUser.No, BP.Web.GuestUser.Name);

            //生成参数格式，把他传入给流程引擎，让其向这个手机写入消息。
            Hashtable ht = new Hashtable();
            ht.Add("SQRSJH","18660153393");

            //向下发送.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork("055", workID, ht, null);

            string empNo = objs.VarAcceptersID;
            if (empNo != "guoxiangbin")
                throw new Exception("@应该发送给guoxiangbin 处理但是发送到了:"+ empNo);
            #region 检查消息发送的结果.

            //系统设置了向下一步接受人员发送邮件，向指定字段的作为手机号发送短信。
            BP.WF.SMSs smss = new SMSs();
            smss.RetrieveAllFromDBSource();
            if (smss.Count != 1)
                throw new Exception("@应该产生 1 条，现在产生了"+smss.Count+"条。");

            foreach (BP.WF.SMS sms in smss)
            {
                if (sms.MsgType != SMSMsgType.SendSuccess)
                    throw new Exception("@应该是 SendSuccess 禁用状态,现在是:" + sms.MsgType);

                if (sms.HisMobileSta != MsgSta.UnRun)
                    throw new Exception("@应该是 短消息 禁用状态，但是目前状态是:" + sms.HisMobileSta);

                if (sms.Sender != BP.Web.WebUser.No)
                    throw new Exception("@应该 Sender= " + BP.Web.WebUser.No + " ，但是目前是:" + sms.Sender);

                if (sms.IsRead != 0)
                    throw new Exception("@应该是 IsRead=0 ，但是目前是:" + sms.IsRead);
            }
            #endregion 检查消息发送的结果.

            //给发起人赋值.
            starterEmp = new Port.Emp(empNo);
            //让 userNo 登录.
            BP.WF.Dev2Interface.Port_Login(empNo);
        }
        /// <summary>
        /// 测试默认的请假流程发送与退回.
        /// zhanghaicheng登录，
        /// zhoupeng 审批.
        /// liping 核实.
        /// </summary>
        public void Test002()
        {
            //流程编号.
            string fk_flow = "002";
            userNo = "liyan";

            //给发起人赋值.
            starterEmp = new Port.Emp(userNo);

            //让 userNo 登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //检查201节点是否有短消息？
            PushMsgs msgs = new PushMsgs(201);
            if (msgs.Count > 0)
                throw new Exception("@测试模版变化，以下的测试将会不正确。");

            //创建空白工作, 发起开始节点.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //删除消息. 
            BP.DA.DBAccess.RunSQL("DELETE FROM Sys_SMS");

            //执行发送工作.
            BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID, null, null);

            #region 检查是否有消息产生.
            BP.WF.SMSs smss = new SMSs();
            smss.RetrieveAllFromDBSource();
            if (smss.Count == 0)
                throw new Exception("@执行了发送，应该产生消息，而没有产生。");

            if (smss.Count != 1)
                throw new Exception("@应该产生1条，现在产生了多条。");

            foreach (BP.WF.SMS sm in smss)
            {
                if (sm.HisEmailSta != MsgSta.UnRun)
                    throw new Exception("@应该是邮件启用状态，但是目前状态是:" + sm.HisEmailSta);

                if (sm.MsgType != SMSMsgType.SendSuccess)
                    throw new Exception("@应该是 SendSuccess 的类型，但是目前状态是:" + sm.MsgType);

                if (sm.SendToEmpNo != "liping")
                    throw new Exception("@应该是 liping 是接受人ID，但是目前是:" + sm.SendToEmpNo);

                if (sm.Sender != BP.Web.WebUser.No)
                    throw new Exception("@应该 Sender= " + BP.Web.WebUser.No + " ，但是目前是:" + sm.Sender);

                if (sm.IsRead != 0)
                    throw new Exception("@应该是 IsRead=0 ，但是目前是:" + sm.IsRead);
            }
            #endregion 检查是否有消息产生.

            /*
             * 让 zhanghaicheng 登录，走申请 __>> 总经理审批 __>> 人力资源备案.
             */

            //让 zhanghaicheng 登录,
            BP.WF.Dev2Interface.Port_Login("zhanghaicheng");
            //创建空白工作, 发起开始节点.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);
            //让zhanghaicheng发送.
            BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID, null, null);

            //让 zhoupeng 登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //删除消息. 
            BP.DA.DBAccess.RunSQL("DELETE FROM Sys_SMS");

            //让zhoupeng发送, 发送到人力资源备案. 并且人力资源有短消息提醒.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID, null, null);

            //到达HR节点ID.
            int nodeIDOfHR = objs.VarToNodeID;
            PushMsgs pms = new PushMsgs(nodeIDOfHR);
            if (pms.Count != 1)
                throw new Exception("@请假流程的消息机制模版, 人力资源部审批，设置的短消息到达，被改变。");

            //检查是否有消息存在.
            smss = new SMSs();
            smss.RetrieveAllFromDBSource();
            if (smss.Count == 0)
                throw new Exception("@执行了发送，应该产生消息，而没有产生，应该产生一条短消息.");

            //遍历消息.
            foreach (BP.WF.SMS item in smss)
            {
                if (item.MsgType == "WorkArrive")
                {
                    if (item.SendToEmpNo != "liping")
                        throw new Exception("@应该是 liping 现在是:" + item.SendToEmpNo);

                    if (item.Sender != "zhoupeng")
                        throw new Exception("@应该是 zhoupeng 现在是:" + item.SendToEmpNo);

                    if (item.HisMobileSta != MsgSta.UnRun)
                        throw new Exception("@ MsgSta 状态写入不正确 UnRun 现在是:" + item.SendToEmpNo);
                }
            }
        }
    }
}
