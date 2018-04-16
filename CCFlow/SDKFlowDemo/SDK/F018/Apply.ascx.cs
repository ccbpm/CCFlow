using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Demo;
using BP.Demo.SDK;
using BP.Demo.FlowEvent;
using BP.Demo.BPFramework;
using BP.Web;
using BP.DA;


namespace CCFlow.SDKFlowDemo.SDK.F018
{
    public partial class Apply : BP.Web.UC.UCBase3
    {
        #region 变量
        public Int64 WorkID
        {
            get
            {
                string temp = this.Request.QueryString["WorkID"];
                if (DataType.IsNullOrEmpty(temp))
                {
                    return 0;
                }
                else
                {
                    return Int64.Parse(temp);
                }
            }
        }
        public Int64 FID
        {
            get
            {
                string temp = this.Request.QueryString["FID"];
                if (DataType.IsNullOrEmpty(temp))
                {
                    return 0;
                }
                else
                {
                    return Int64.Parse(temp);
                }
            }
        }
        public int FK_Node
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return 101;
                }
            }
        }
        public string FK_Flow
        {
            get
            {
                return Request.QueryString["FK_Flow"];
            }
        }
        #endregion 变量
        protected void Page_Load(object sender, EventArgs e)
        {
            //检查物理表
            ND018Rpt rpt = new ND018Rpt();
            rpt.CheckPhysicsTable();

            if (this.IsPostBack == false)
            {
                //绑定前台输入的信息
                BindCtrl();
            }
        }
        public void BindCtrl()
        {
            ND018Rpt rpt = null;
            //判断是否有父流程
            if (FID == 0)
                rpt = new ND018Rpt(WorkID);
            else
                rpt = new ND018Rpt(FID);

            //将申请人相关信息自动填充
            this.TB_No.Text = WebUser.No;
            this.TB_Name.Text = WebUser.Name;
            this.TB_DeptNo.Text = WebUser.FK_Dept;
            this.TB_DeptName.Text = WebUser.FK_DeptName;

            //给标准字段赋值。ccflow标准字段、命名规则请查看文档
            this.SetCtrlValues(rpt);

            //如果不是开始节点，不能修改请假信息
            if (this.FK_Node != 1801)
            {
                this.TB_No.Enabled = false;
                this.TB_Name.Enabled = false;
                this.TB_DeptNo.Enabled = false;
                this.TB_DeptName.Enabled = false;
                this.TB_QingJiaTianShu.Enabled = false;
                this.TB_QingJiaYuanYin.Enabled = false;
            }
            //如果不是部门经理审批环节，将不能填写办理意见.
            if (this.FK_Node != 1802)
            {
                this.TB_NoteBM.Enabled = false;
            }
            if (this.FK_Node != 1803)
            {
                this.TB_NoteZJL.Enabled = false;
            }
        }
    }
}