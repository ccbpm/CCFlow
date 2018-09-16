using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.NodeAttr
{
    /// <summary>
    /// 测试找领导
    /// </summary>
    public class FindLeader : TestBase
    {
        /// <summary>
        /// 测试找领导
        /// </summary>
        public FindLeader()
        {
            this.Title = "测试找领导";
            this.DescIt = "流程: 以demo 流程 054:找人规则(找领导) 为例测试。";
            this.EditState = EditState.Passed;
        }

        #region 全局变量
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
        #endregion 变量

        /// <summary>
        /// 测试案例说明:
        /// 1， 分别列举4种。
        /// 2， 测试找领导的两种模式
        /// </summary>
        public override void Do()
        {
            this.fk_flow = "054";
            fl = new Flow(this.fk_flow);
            string sUser = "zhoushengyu";
            BP.WF.Dev2Interface.Port_Login(sUser);

            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送, 直接领导.
            Hashtable ht = new Hashtable();
            ht.Add("FindLeader", 0); // 找直接领导.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, ht);

            #region 分析预期
            if (objs.VarAcceptersID != "zhanghaicheng")
                throw new Exception("@按照找直接领导的方式0, （直接领导模式）找领导错误，应当是zhanghaicheng现在是" + objs.VarAcceptersID);
            #endregion

            //执行撤销发送. 指定级别的主管.
            BP.WF.Dev2Interface.Flow_DoUnSend(fl.No, workID);
            ht.Clear(); //清除参数.
            ht.Add("FindLeader", 1); // 找指定职务级别的领导.
            objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, ht);

            #region 分析预期
            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@按照找直接领导的方式1, （指定级别的领导）找领导错误，应当是zhoupeng现在是" + objs.VarAcceptersID);
            #endregion



            //执行撤销发送. 特定职务的领导.
            BP.WF.Dev2Interface.Flow_DoUnSend(fl.No, workID);
            ht.Clear(); //清除参数.
            ht.Add("FindLeader", 2); // 找指定职务级别的领导.
            objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, ht);

            #region 分析预期
            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@按照找直接领导的方式1, （指定级别的领导）找领导错误，应当是zhoupeng现在是" + objs.VarAcceptersID);
            #endregion


            //执行撤销发送. 特定职务的领导.
            BP.WF.Dev2Interface.Flow_DoUnSend(fl.No, workID);
            ht.Clear(); //清除参数.
            ht.Add("FindLeader", 2); // 找指定职务级别的领导.
            objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, ht);

            #region 分析预期
            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@按照找直接领导的方式1, （指定级别的领导）找领导错误，应当是zhoupeng现在是" + objs.VarAcceptersID);
            #endregion


            //执行撤销发送. 特定岗位的领导.
            BP.WF.Dev2Interface.Flow_DoUnSend(fl.No, workID);
            ht.Clear(); //清除参数.
            ht.Add("FindLeader", 3); // 找指定职务级别的领导.
            objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, ht);

            #region 分析预期
            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@按照找直接领导的方式1, （指定级别的领导）找领导错误，应当是zhoupeng现在是" + objs.VarAcceptersID);
            #endregion


            // 现在已经把第一排的节点，都已经找全了， 测试左边的两个case .找谁?

            // 测试找谁, 指定节点工作人员的直接领导。
            this.DoFineWho_1();


            // 测试找谁, 指定表单字段的工作人员的直接领导。
            this.DoFineWho_2();

        }
        /// <summary>
        /// 测试找谁? 指定节点工作人员的直接领导。
        /// </summary>
        public void DoFineWho_1()
        {
            string sUser = "zhoushengyu";
            BP.WF.Dev2Interface.Port_Login(sUser);

            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送, 直接领导.
            Hashtable ht = new Hashtable();
            ht.Add("FindLeader", 0); // 找直接领导.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, ht);

            // 让zhanghaicheng登录, 执行第二步骤地发送。
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);

            // 让zhanghaicheng 执行发送，应该还发送给zhanghaicheng.
            objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

            #region 分析预期
            if (objs.VarAcceptersID != "zhanghaicheng")
                throw new Exception("@按照找直接领导的方式1, （指定级别的领导）找领导错误，应当是zhanghaicheng现在是" + objs.VarAcceptersID);
            #endregion

        }
        /// <summary>
        /// 指定表单字段的人员的直接领导。
        /// </summary>
        public void DoFineWho_2()
        {
            string sUser = "zhoushengyu";
            BP.WF.Dev2Interface.Port_Login(sUser);

            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送, 直接领导.
            Hashtable ht = new Hashtable();
            ht.Add("FindLeader", 1); // 找直接领导.
            ht.Add("RenYuanBianHao", "zhoutianjiao");
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, ht);

            // 让zhanghaicheng登录, 执行第二步骤地发送。
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);

            // 让zhanghaicheng 执行发送，应该还发送给zhanghaicheng.
            objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

            #region 分析预期
            if (objs.VarAcceptersID != "qifenglin")
                throw new Exception("@按照找直接领导的方式1, （指定级别的领导）找领导错误，应当是qifenglin现在是" + objs.VarAcceptersID);
            #endregion
        }
    }
}
