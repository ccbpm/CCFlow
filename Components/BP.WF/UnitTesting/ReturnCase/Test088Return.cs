using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.ReturnCase
{
    public class Test088Return : TestBase
    {
        /// <summary>
        /// 测试退回
        /// </summary>
        public Test088Return()
        {
            this.Title = "流程中间步骤具有分合流的退回";
            this.DescIt = "1,退回到开始点，一步步的向下发送。2，退回到分流点，一步步的向下发送.";
            this.DescIt += "重复1,2,测试退回并原路返回。";
            this.EditState = EditState.Passed;
        }
        public override void Do()
        {
          
            #region 测试退回到开始节点的三种模式.
            //测试从最后一个节点退回到开始节点，然后让其一步步的发送.
            this.TestReturnToStartNode();

            //测试从最后一个节点退回到开始节点，然后让其原路返回.
            this.TestReturnToStartNodeWithTrackback();

            //测试从最后一个节点退回到开始节点，然后让其原路返回， 然后在让其退回并不原路返回。
            this.TestReturnToStartNodeWithTrackback_1();
            #endregion 测试退回到开始节点的三种模式.


            #region 测试退回到分流节点的三种模式.
            this.ReturnToFenLiuNode();
            this.ReturnToFenLiuNodeWithTrackback();
            this.ReturnToFenLiuNodeWithTrackback_1();
            #endregion 测试退回到分流节点的三种模式.

            return;
        }

        #region 测试退回到分流点.
        /// <summary>
        /// 测试退回到分流点，然后让其一步步的发送.
        /// </summary>
        public void ReturnToFenLiuNode()
        {
            //创建测试案例.
            Int64 workid = this.CreateTastCase();

            //让他登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //向开始节点退回，并不原路返回。
            BP.WF.Dev2Interface.Node_ReturnWork("088", workid, 0, 8899, 8802, "test", false);

            //让 qifenglin , 登录让其发送,这是合流点发送.
            BP.WF.Dev2Interface.Port_Login("qifenglin");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            // 让分流点发送.
            string sql = "SELECT WorkID,FK_Emp FROM WF_GenerWorkerlist WHERE FID=" + workid;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                Int64 id = Int64.Parse(dr["WorkID"].ToString());
                string emp = dr[1].ToString();
                BP.WF.Dev2Interface.Port_Login(emp);
                BP.WF.Dev2Interface.Node_SendWork("088", id);
            }

            //让 zhanghaicheng , 登录让其发送.
            BP.WF.Dev2Interface.Port_Login("zhanghaicheng");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            //让 zhoupeng , 登录让其发送。
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            /*
             * 运行到这里执行成功。
             */
        }

        /// <summary>
        /// 测试从最后一个节点退回到分流点，然后让其原路返回..
        /// </summary>
        public void ReturnToFenLiuNodeWithTrackback()
        {
            //创建测试案例.
            Int64 workid = this.CreateTastCase();

            //让他登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //向开始节点退回，并不原路返回。
            BP.WF.Dev2Interface.Node_ReturnWork("088", workid, 0, 8899, 8802, "test", true);

            //让 zhoutianjiao , 登录让其发送。
            BP.WF.Dev2Interface.Port_Login("qifenglin");
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork("088", workid);
            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@退回到开始节点并原路返回错误,应该返回给zhoupeng,现在是:" + objs.VarAcceptersID);
            
            //让zhoupeng 登录，发送下去。
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);
            objs = BP.WF.Dev2Interface.Node_SendWork("088", workid);
        }

        /// <summary>
        /// 测试从最后一个节点退回到分流点，然后让其原路返回.
        /// 返回到最后一个节点后，让其在执行一次退回但是并不原路返回，让发起人执行发送
        /// 看看是否是按照步骤一步步的向下运动。
        /// </summary>
        public void ReturnToFenLiuNodeWithTrackback_1()
        {
            //创建测试案例.
            Int64 workid = this.CreateTastCase();

            //让他登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //向开始节点退回，并不原路返回。
            BP.WF.Dev2Interface.Node_ReturnWork("088", workid, 0, 8899, 8802, "test", true);

            //让 zhoutianjiao , 登录让其发送。
            BP.WF.Dev2Interface.Port_Login("qifenglin");
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork("088", workid);
            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@退回到开始节点并原路返回错误,应该返回给zhoupeng,现在是:" + objs.VarAcceptersID);

            //在执行一次退回,但不原路返回.
            BP.WF.Dev2Interface.Node_ReturnWork("088", workid, 0, 8899, 8802, "test", false);


            //让qifenglin登录, 测试下一步骤，是否按照一步步的走向结束节点。
            BP.WF.Dev2Interface.Port_Login("qifenglin");
            objs = BP.WF.Dev2Interface.Node_SendWork("088", workid);

            // 让分流点发送.
            string sql = "SELECT WorkID,FK_Emp FROM WF_GenerWorkerlist WHERE FID=" + workid;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("没有查询到子线程.");

            foreach (DataRow dr in dt.Rows)
            {
                Int64 id = Int64.Parse(dr["WorkID"].ToString());
                string emp = dr[1].ToString();
                BP.WF.Dev2Interface.Port_Login(emp);
                BP.WF.Dev2Interface.Node_SendWork("088", id);
            }

            //让 zhanghaicheng , 登录让其发送.
            BP.WF.Dev2Interface.Port_Login("zhanghaicheng");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            //让 zhoupeng , 登录让其发送。
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            /*
             * 运行到这里执行成功。
             */
        }

        #endregion 测试退回到分流点.


        #region 测试退回到开始节点的三种模式
        /// <summary>
        /// 测试从最后一个节点退回到开始节点，然后让其原路返回.
        /// 返回到最后一个节点后，让其在执行一次退回但是并不原路返回，让发起人执行发送
        /// 看看是否是按照步骤一步步的向下运动。
        /// </summary>
        public void TestReturnToStartNodeWithTrackback_1()
        {
            //创建测试案例.
            Int64 workid = this.CreateTastCase();

            //让他登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //向开始节点退回，并不原路返回。
            BP.WF.Dev2Interface.Node_ReturnWork("088", workid, 0, 8899, 8801, "test", true);

            //让 zhoutianjiao , 登录让其发送。
            BP.WF.Dev2Interface.Port_Login("zhoutianjiao");
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork("088", workid);
            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@退回到开始节点并原路返回错误,应该返回给zhoupeng,现在是:" + objs.VarAcceptersID);

            //在执行一次退回,但不原路返回.
            BP.WF.Dev2Interface.Node_ReturnWork("088", workid, 0, 8899, 8801, "test", false);


            //让zhoutianjiao登录, 测试下一步骤，是否按照一步步的走向结束节点。
            BP.WF.Dev2Interface.Port_Login("zhoutianjiao");
            objs = BP.WF.Dev2Interface.Node_SendWork("088", workid);
         
            //让 qifenglin , 登录让其发送,这是合流点发送.
            BP.WF.Dev2Interface.Port_Login("qifenglin");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            // 让分流点发送.
            string sql = "SELECT WorkID,FK_Emp FROM WF_GenerWorkerlist WHERE FID=" + workid;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                Int64 id = Int64.Parse(dr["WorkID"].ToString());
                string emp = dr[1].ToString();
                BP.WF.Dev2Interface.Port_Login(emp);
                BP.WF.Dev2Interface.Node_SendWork("088", id);
            }

            //让 zhanghaicheng , 登录让其发送.
            BP.WF.Dev2Interface.Port_Login("zhanghaicheng");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            //让 zhoupeng , 登录让其发送。
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            /*
             * 运行到这里执行成功。
             */
        }

        /// <summary>
        /// 测试从最后一个节点退回到开始节点，然后让其原路返回..
        /// </summary>
        public void TestReturnToStartNodeWithTrackback()
        {
            //创建测试案例.
            Int64 workid = this.CreateTastCase();

            //让他登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //向开始节点退回，并不原路返回。
            BP.WF.Dev2Interface.Node_ReturnWork("088", workid, 0, 8899, 8801, "test", true);

            //让 zhoutianjiao , 登录让其发送。
            BP.WF.Dev2Interface.Port_Login("zhoutianjiao");
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork("088", workid);
            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@退回到开始节点并原路返回错误,应该返回给zhoupeng,现在是:" + objs.VarAcceptersID);

            //让zhoupeng 登录，发送下去。
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);
            objs = BP.WF.Dev2Interface.Node_SendWork("088", workid);

        }
        /// <summary>
        /// 测试从最后一个节点退回到开始节点，然后让其一步步的发送.
        /// </summary>
        public void TestReturnToStartNode()
        {
            //创建测试案例.
            Int64 workid = this.CreateTastCase();

            //让他登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //向开始节点退回，并不原路返回。
            BP.WF.Dev2Interface.Node_ReturnWork("088", workid, 0, 8899, 8801, "test", false);

            //让 zhoutianjiao , 登录让其发送。
            BP.WF.Dev2Interface.Port_Login("zhoutianjiao");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            //让 qifenglin , 登录让其发送,这是合流点发送.
            BP.WF.Dev2Interface.Port_Login("qifenglin");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            // 让分流点发送.
            string sql = "SELECT WorkID,FK_Emp FROM WF_GenerWorkerlist WHERE FID=" + workid;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                Int64 id = Int64.Parse(dr["WorkID"].ToString());
                string emp = dr[1].ToString();
                BP.WF.Dev2Interface.Port_Login(emp);
                BP.WF.Dev2Interface.Node_SendWork("088", id);
            }

            //让 zhanghaicheng , 登录让其发送.
            BP.WF.Dev2Interface.Port_Login("zhanghaicheng");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            //让 zhoupeng , 登录让其发送。
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            BP.WF.Dev2Interface.Node_SendWork("088", workid);

            /*
             * 运行到这里执行成功。
             */
        }
        #endregion 测试退回到开始节点的三种模式

        /// <summary>
        /// 创建一个测试场景，让他跑到结束节点.
        /// </summary>
        public Int64 CreateTastCase()
        {
            string fk_flow = "088";
            string startUser = "zhoutianjiao";
            BP.Port.Emp starterEmp = new Port.Emp(startUser);

            Flow fl = new Flow(fk_flow);

            //让周天娇登录, 在以后，就可以访问WebUser.No, WebUser.Name 。
            BP.WF.Dev2Interface.Port_Login(startUser);

            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //执行向 分流点 发送, qifenglin接受.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            if (objs.VarAcceptersID != "qifenglin")
                throw new Exception("@接受人错误，应当是qifenglin,现在是:" + objs.VarAcceptersID);

            //让分流点的发起人登录.
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);
            //执行向下发送.
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            if (objs.VarAcceptersID != "zhangyifan,zhoushengyu,")
                throw new Exception("@接受人错误，应当是zhangyifan,zhoushengyu,现在是:" + objs.VarAcceptersID);

            string sql = "SELECT WorkID,FK_Emp FROM WF_GenerWorkerlist WHERE FID=" + workid;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                Int64 id = Int64.Parse(dr["WorkID"].ToString());
                string emp = dr[1].ToString();
                BP.WF.Dev2Interface.Port_Login(emp);
                objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, id);
            }

           
            //让qifenglin登录,发到最后一个节点,完成tastcase.
            BP.WF.Dev2Interface.Port_Login("zhanghaicheng");
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            if (objs.VarAcceptersID!="zhoupeng")
                throw new Exception("@接受人错误，应当是zhoupeng,现在是:" + objs.VarAcceptersID);

            return workid;
        }
       
    }
}
