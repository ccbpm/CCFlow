using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.SendCase
{
    public class Send122 : TestBase
    {
        /// <summary>
        /// 一人处理多个子线程流程
        /// </summary>
        public Send122()
        {
            this.Title = "一人处理多个子线程流程(任务维度流程)";
            this.DescIt = "流程:122，一个人处理多个子线程的case, 带有任务维度的流程。";
            this.EditState = EditState.UnOK;
        }

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
        public Int64 workid = 0;
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
        /// 1, 此流程针对于122分合流程进行， zhanghaicheng发起，zhoushengyu,zhangyifan, 两个人处理子线程，
        ///    zhanghaicheng 接受子线程汇总数据.
        /// 2, 测试方法体分成三大部分. 发起，子线程处理，合流点执行，分别对应: Step1(), Step2_1(), Step2_2()，Step3() 方法。
        /// 3，针对发送测试，不涉及到其它的功能.
        /// </summary>
        public override void Do()
        {
            //初始化变量.
            fk_flow = "122";
            userNo = "zhanghaicheng";

            fl = new Flow(fk_flow);

            //执行第1步检查，创建工作与发送.
            this.Step1();
 
        }
        /// <summary>
        /// 创建流程，发送分流点第1步.
        /// </summary>
        public void Step1()
        {
            Flow fl = new Flow("122");
            fl.DoDelData();


            // 让zhanghaicheng 登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //创建空白工作, 发起开始节点.
            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //获得他的work.
            Node nd = new Node(12201);
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.Retrieve();

            // 初始化明细表的接收人数据.
            MapDtl dtl = wk.HisMapDtls[0] as MapDtl;
            GEDtl enDtl = dtl.HisGEDtl;
            enDtl.RefPK = workid.ToString();
            enDtl.SetValByKey("ChuLiRen", "zhangyifan");
            enDtl.SetValByKey("ChuLiRenMingCheng", "张一帆");
            enDtl.SetValByKey("PiCiHao", "AA");  //批次号.
            enDtl.Insert();

            enDtl = dtl.HisGEDtl;
            enDtl.RefPK = workid.ToString();
            enDtl.SetValByKey("ChuLiRen", "zhangyifan");
            enDtl.SetValByKey("ChuLiRenMingCheng", "张一帆");
            enDtl.SetValByKey("PiCiHao", "AA");  //批次号.
            enDtl.Insert();


            enDtl = dtl.HisGEDtl;
            enDtl.RefPK = workid.ToString();
            enDtl.SetValByKey("ChuLiRen", "zhangyifan");
            enDtl.SetValByKey("ChuLiRenMingCheng", "张一帆");
            enDtl.SetValByKey("PiCiHao", "BB"); //批次号.
            enDtl.Insert();

            // 执行向下发送.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.fk_flow, this.workid);

            if (objs.VarTreadWorkIDs == "")
                throw new Exception("sss");


        }
       
    }
}
