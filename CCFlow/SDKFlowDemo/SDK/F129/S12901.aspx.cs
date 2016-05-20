using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo.SDK.F129
{
    public partial class S12901 : System.Web.UI.Page
    {
        #region 接受4大参数.
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
        /// 当前节点
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
        /// 流程ID，不是分合流的时候始终等于0.
        /// </summary>
        public Int64 FID
        {
            get
            {
                if (this.Request.QueryString["FID"] == null)
                    return 0;
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        #endregion 4大参数.

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

                if (this.FK_Node != 12901)
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

        protected void Btn_Save_Click(object sender, EventArgs e)
        {

            #region 业务数据保存, 根据workid, 把它作为主键，存入数据表(此部分与ccflow无关).
            if (this.FK_Node == 12901)
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