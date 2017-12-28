using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.App.F001
{
    public partial class S101 : System.Web.UI.Page
    {
        #region 公用方法
        /// <summary>
        /// 转到提示信息界面.
        /// </summary>
        /// <param name="msg">提示信息</param>
        public void ToMsg(string msg)
        {
            System.Web.HttpContext.Current.Session["info"] = msg;
            System.Web.HttpContext.Current.Application["info" + BP.Web.WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            System.Web.HttpContext.Current.Response.Redirect(
                "/SDKFlowDemo/SDK/Info.aspx?xx=" + DateTime.Now.ToString(), false);
        }
        /// <summary>
        /// 转到提示错误信息界面
        /// </summary>
        /// <param name="msg">错误信息</param>
        public void ToErrorPage(string msg)
        {
            System.Web.HttpContext.Current.Session["info"] = msg;
            System.Web.HttpContext.Current.Application["info" + BP.Web.WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            System.Web.HttpContext.Current.Response.Redirect("/SDKFlowDemo/SDK/ErrorPage.aspx?xxx=" + DateTime.Now.ToString(), false);
        }
        #endregion 公用方法

        #region 接受4大参数.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        #endregion 4大参数.

        protected void Page_Load(object sender, EventArgs e)
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            this.Page.Title = "您好:" + BP.Web.WebUser.No + " - " + BP.Web.WebUser.Name + " . 当前节点:" + nd.Name;
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

                if (this.FK_Node != 11001)
                {
                    /*如果是不是开始节点,让其只读.*/
                    this.TB_No.ReadOnly = true; //
                    this.TB_Name.ReadOnly = true; // 请假人名称.
                    this.TB_DeptNo.ReadOnly = true; //部门编号.
                    this.TB_DeptName.ReadOnly = true; //部门名称
                    
                    this.TB_QingJiaTianShu.ReadOnly = true;
                    this.TB_QingJiaYuanYin.ReadOnly = true;
                }
            }
        }
        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            try
            {
                Btn_Save_Click(null, null); //执行保存.
                BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);// 执行发送.
                this.ToMsg(objs.ToMsgOfHtml()); //输出信息.
            }
            catch (Exception ex)
            {
                this.ToMsg(ex.Message);
            }
        }
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            #region 业务数据保存, 根据workid, 把它作为主键，存入数据表(此部分与ccflow无关).
            if (this.FK_Node == 11001)
            {
                /*如果是开始节点，才执行保存.*/
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
            }
            #endregion 业务数据保存, 根据workid, 把它作为主键，存入数据表.
        }

    }
}