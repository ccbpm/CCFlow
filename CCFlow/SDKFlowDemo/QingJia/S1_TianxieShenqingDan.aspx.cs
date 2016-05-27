using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo.QingJia
{
    public partial class S1_TianxieShenqingDan : System.Web.UI.Page
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
                //查询出来数据给表单赋值。
                BP.Demo.SDK.QingJia en = new BP.Demo.SDK.QingJia();
                en.OID = (int)this.WorkID;
                if (en.RetrieveFromDBSources() == 1)
                {
                    /* 数据源已经有 */
                    this.TB_No.Text = en.QingJiaRenNo; //
                    this.TB_Name.Text = en.QingJiaRenName; // 请假人名称.
                    this.TB_DeptNo.Text = en.QingJiaRenDeptNo; //部门编号.
                    this.TB_DeptName.Text = en.QingJiaRenDeptName; //部门名称
                    this.TB_QingJiaYuanYin.Text = en.QingJiaYuanYin; //请假原因
                    this.TB_QingJiaTianShu.Text = en.QingJiaTianShu.ToString(); //请假天数
                }
                else
                {
                    /*给他默认值*/
                    this.TB_No.Text = BP.Web.WebUser.No;
                    this.TB_Name.Text = BP.Web.WebUser.Name;
                    this.TB_DeptNo.Text = BP.Web.WebUser.FK_Dept;
                    this.TB_DeptName.Text = BP.Web.WebUser.FK_DeptName;
                }
            }
        }
        /// <summary>
        /// 执行发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            //第1步: 执行保存.
            this.Btn_Save_Click(null, null);

            // 检查完整性
            BP.Demo.SDK.QingJia en = new BP.Demo.SDK.QingJia();
            en.OID = (int)this.WorkID;
            en.Retrieve();
            if (en.QingJiaTianShu <= 0)
            {
                this.Response.Write("<font color=red>保存失败，请假天数不能小于等于零。</font>");
                return;
            }

            #region 第2步: 执行发送.
            BP.WF.SendReturnObjs objs = null;
            try
            {
                //编写我的业务逻辑....

                //调用发送api, 返回发送对象.
                objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);

            }
            catch(Exception ex)
            {
                this.Response.Write("<font color=red>发送期间出现异常:" + ex.Message + "</font>");
                return;
            }

        //    objs.v
            /*
             这里注意：
             * 1,发送api有多个, 根据不同的场景使用不同的api 但是常用的就那1个，您可以产看该参数使用说明.
             * BP.WF.Node_SendWork(string fk_flow, Int64 workID, int toNodeID, string toEmps) 
             * 2,回来的发送对象里面有系统变量，这些系统变量包括发送给谁了，发送到那里了.
             * 开发人员可以根据系统变量,执行相关的业务逻辑操作.
             */
            #endregion 第2步: 执行发送.

            #region 第3步: 把发送信息提示出来.
            //objs.v
            string info = objs.ToMsgOfHtml();
            info = info.Replace("\t\n", "<br>@");
            info = info.Replace("@", "<br>@");
            this.Response.Write("<font color=blue>"+info+"</font>");
            #endregion 第3步: 把发送信息提示出来.

            this.Btn_Save.Enabled = false;
            this.Btn_Send.Enabled = false;
        }
        /// <summary>
        /// 执行保存.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            #region 业务数据保存, 根据workid, 把它作为主键，存入数据表(此部分与ccflow无关).
            //本demo我们使用BP框架做了数据存储.
            BP.Demo.SDK.QingJia en = new BP.Demo.SDK.QingJia();
            en.OID = (int)this.WorkID;
            en.QingJiaRenNo = BP.Web.WebUser.No;
            en.QingJiaRenName = BP.Web.WebUser.Name;
            en.QingJiaRenDeptNo = BP.Web.WebUser.FK_Dept;
            en.QingJiaRenDeptName = BP.Web.WebUser.FK_DeptName;
            en.QingJiaYuanYin = this.TB_QingJiaYuanYin.Text;
            en.QingJiaTianShu = float.Parse(this.TB_QingJiaTianShu.Text);

            if (en.IsExits == false)
                en.InsertAsOID(this.WorkID);  /*如果已经不存在.*/
            else
                en.Update();
            #endregion 业务数据保存, 根据workid, 把它作为主键，存入数据表.

        }

        protected void Btn_Track_Click(object sender, EventArgs e)
        {
            BP.WF.Dev2Interface.UI_Window_OneWork(this.FK_Flow, this.WorkID, this.FID);
        }
    }
}