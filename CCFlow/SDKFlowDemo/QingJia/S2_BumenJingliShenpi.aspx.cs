using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace CCFlow.SDKFlowDemo.QingJia
{
    public partial class S2_BumenJingliShenpi : System.Web.UI.Page
    {
        #region 接受4大参数(这四大参数是有ccflow传递到此页面上的).
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// 当前节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        #endregion 接受4大参数(这四大参数是有ccflow传递到此页面上的).


        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {

                // 查询出来.
                BP.Demo.SDK.QingJia en = new BP.Demo.SDK.QingJia();
                en.OID = (int)this.WorkID;
                en.Retrieve();

                this.TB_No.Text = en.QingJiaRenNo; //
                this.TB_Name.Text = en.QingJiaRenName; // 请假人名称.
                this.TB_DeptNo.Text = en.QingJiaRenDeptNo; //部门编号.
                this.TB_DeptName.Text = en.QingJiaRenDeptName; //部门名称
                this.TB_QingJiaYuanYin.Text = en.QingJiaYuanYin; //请假原因
                this.TB_QingJiaTianShu.Text = en.QingJiaTianShu.ToString();

            }
        }

        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            // 执行保存.
            Btn_Save_Click(null, null);

            #region 第2步: 执行发送.
            //调用发送api, 返回发送对象.
            BP.WF.SendReturnObjs objs = null;

            // 查询出来.
            BP.Demo.SDK.QingJia en = new BP.Demo.SDK.QingJia();
            en.OID = (int)this.WorkID;
            en.Retrieve();

            Hashtable ht = new Hashtable();
            ht.Add("JinE", 1000);

            ht.Add("QingJiaTianShu", en.QingJiaTianShu);
            ht.Add("SysIsReadReceipts", 1); //是否需要回执.

            objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, ht);

            //  objs = BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, 901,"zhoupeng,sds,d,sd");

            /*
             这里注意： 
             * 1,发送api有多个, 根据不同的场景使用不同的api 但是常用的就那1个，您可以产看该参数使用说明.
             *   BP.WF.Node_SendWork(string fk_flow, Int64 workID, int toNodeID, string toEmps) 
             *   重要说明:
             *    1. 如果toNodeID=0,ccbpm就会按照流程图上计算，如果为一个节点ID，ccbpm就会认为你要发送到这个节点上.
             *    2, 如果toEmps=null, ccbpm就会根据节点的配置来计算接收人，如果指定了一个或多个接收人，比如:zhangsian,lisi, 
             *       ccbpm就会认为要交给这些人处理。             *    
             * 2,回来的发送对象里面有系统变量，这些系统变量包括发送给谁了，发送到那里了.
             * 开发人员可以根据系统变量,执行相关的业务逻辑操作.
             * 
             */


            //objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);
            //objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, 0, null);

            ///***发送后，流程转向问题 BY HZM ******/
            if (en.QingJiaTianShu > 10)
                objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, 1803, "zhangsan,lisi,");
            else
                objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, 1899, null);

            //BP.WF.Dev2Interface.Node_AddNextStepAccepters(1000, 102, "zhangsna,lisi", false);
            //BP.WF.Dev2Interface.Node_AddNextStepAccepters(1000, 103, "wangwu,zhaoliu", false);
            //BP.WF.Dev2Interface.Node_AddNextStepAccepters(1000, 105, "wsxx,dd", false);

            ///***修改结束******/

            BP.Demo.SDK.ND018Rpt rpt = new BP.Demo.SDK.ND018Rpt();

            BP.Demo.SDK.QingJia qingjia = new BP.Demo.SDK.QingJia();
            qingjia.Copy(rpt);
            qingjia.Insert();

            #endregion 第2步: 执行发送.

            #region 第3步: 把发送信息提示出来.
            string info = objs.ToMsgOfText();
            info = info.Replace("\t\n", "<br>@");
            info = info.Replace("@", "<br>@");
            this.Response.Write("<font color=blue>" + info + "</font>");
            #endregion 第3步: 把发送信息提示出来.

            //设置界面按钮不可以用.
            this.Btn_Save.Enabled = false;
            this.Btn_Send.Enabled = false;
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            BP.Demo.SDK.QingJia en = new BP.Demo.SDK.QingJia();
            en.OID = (int)this.WorkID;
            en.Retrieve();

            en.NoteBM = this.TB_BMNote.Text;
            en.Update(); // 写入部门经理意见.
        }

        protected void Btn_Track_Click(object sender, EventArgs e)
        {
            //  BP.WF.Dev2Interface.UI_Window_OneWork(this.FK_Flow, this.WorkID, this.FID);
        }
        /// <summary>
        /// 退回操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Return_Click(object sender, EventArgs e)
        {
            // 这里调用这个接口，就是转向退回功能部件。
            //  BP.WF.Dev2Interface.UI_Window_Return(this.FK_Flow,this.FK_Node, this.WorkID, this.FID);
        }
    }
}