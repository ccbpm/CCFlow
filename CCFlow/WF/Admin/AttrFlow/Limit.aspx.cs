using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.DA;
using System.Data;

namespace CCFlow.WF.Admin.AttrFlow
{

    public partial class Limit : System.Web.UI.Page
    {
        #region 参数
        public int FK_Flow
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Flow"]);
            }

        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string flowNo = this.Request.QueryString["FK_Flow"];
                BP.WF.Flow fl = new BP.WF.Flow();
                fl.No = flowNo;
                fl.RetrieveFromDBSources();
                this.TB_Alert.Text = fl.StartLimitAlert; //限制规则提示.

                switch (fl.StartLimitRole)
                {
                    case StartLimitRole.None: //不限制.
                        this.RB_None.Checked = true; 
                        break;
                    case StartLimitRole.Day: //天.
                        this.RB_ByTime.Checked = true;
                        this.DDL_ByTime.SelectedValue = "0";
                        this.TB_Alert.Text = fl.StartLimitAlert;
                        this.TB_ByTimePara.Text = fl.StartLimitPara;
                        break;
                    case StartLimitRole.Week: //周.
                        this.RB_ByTime.Checked = true;
                        this.DDL_ByTime.SelectedValue = "1";
                      this.TB_Alert.Text = fl.StartLimitAlert;
                        this.TB_ByTimePara.Text = fl.StartLimitPara;
                        break;
                    case StartLimitRole.Month: //月份.
                        this.RB_ByTime.Checked = true;
                        this.DDL_ByTime.SelectedValue = "2";
                         this.TB_Alert.Text = fl.StartLimitAlert;
                        this.TB_ByTimePara.Text = fl.StartLimitPara;
                        break;
                    case StartLimitRole.JD: //月份.
                        this.RB_ByTime.Checked = true;
                        this.DDL_ByTime.SelectedValue = "3";
                        this.TB_Alert.Text = fl.StartLimitAlert;
                        this.TB_ByTimePara.Text = fl.StartLimitPara;
                        break;
                    case StartLimitRole.Year: //年度.
                        this.RB_ByTime.Checked = true;
                        this.DDL_ByTime.SelectedValue = "4";
                       this.TB_Alert.Text = fl.StartLimitAlert;
                        this.TB_ByTimePara.Text = fl.StartLimitPara;
                        break;
                    case StartLimitRole.OnlyOneSubFlow: // 为子流程时仅仅只能被调用1此.
                        this.RB_OnlyOneSubFlow.Checked = true;
                        break;
                    case StartLimitRole.ColNotExit: //发起的列不能重复,(多个列可以用逗号分开).
                        this.RB_ColNotExit.Checked = true;
                        this.TB_ColNotExit_Fields.Text = fl.StartLimitPara;
                        break;
                    case StartLimitRole.ResultIsZero: //小于等于0.
                        this.RB_SQL.Checked = true;
                        this.DDL_SQL.SelectedIndex =0;
                        this.TB_SQL_Para.Text = fl.StartLimitPara;
                        break;
                    case StartLimitRole.ResultIsNotZero: //大于 0 .
                        this.RB_SQL.Checked = true;
                        this.DDL_SQL.SelectedIndex = 1;
                        this.TB_SQL_Para.Text = fl.StartLimitPara;
                        break;
                    default:
                        break;
                }
            }

        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            Save();

        }
        protected void Save()
        {
            string flowNo = this.Request.QueryString["FK_Flow"];
            BP.WF.Flow fl = new BP.WF.Flow(flowNo);

            fl.StartLimitAlert = this.TB_Alert.Text; //限制提示信息

            if (this.RB_None.Checked)
            {
                fl.StartLimitRole = StartLimitRole.None;
            }

            if (this.RB_ByTime.Checked)
            {
                if (this.DDL_ByTime.SelectedIndex == 0)//一人一天一次
                {
                    fl.StartLimitRole = StartLimitRole.Day;
                    fl.StartLimitPara = this.TB_ByTimePara.Text;
                }

                if (this.DDL_ByTime.SelectedIndex == 1)//一人一周一次
                {
                    fl.StartLimitRole = StartLimitRole.Week;
                    fl.StartLimitPara = this.TB_ByTimePara.Text;
                }

                if (this.DDL_ByTime.SelectedIndex == 2)//一人一月一次
                {
                    fl.StartLimitRole = StartLimitRole.Month;
                    fl.StartLimitPara = this.TB_ByTimePara.Text;
                }

                if (this.DDL_ByTime.SelectedIndex == 3)//一人一季一次
                {
                    fl.StartLimitRole = StartLimitRole.JD;
                    fl.StartLimitPara = this.TB_ByTimePara.Text;
                    fl.DirectUpdate();
                }

                if (this.DDL_ByTime.SelectedIndex == 4)//一人一年一次
                {
                    fl.StartLimitRole = StartLimitRole.Year;
                    fl.StartLimitPara = this.TB_ByTimePara.Text;
                }
            }

            if (this.RB_ColNotExit.Checked)//按照发起字段不能重复规则
            {
                fl.StartLimitRole = StartLimitRole.ColNotExit;
                fl.StartLimitPara = this.TB_ColNotExit_Fields.Text;
            }

            if (this.RB_SQL.Checked ==true)
            {
                //字段参数.
                fl.StartLimitPara = this.TB_SQL_Para.Text;

                //选择的模式.
                if (this.DDL_SQL.SelectedIndex == 0)
                    fl.StartLimitRole = StartLimitRole.ResultIsZero;

                if (this.DDL_SQL.SelectedIndex == 1)
                    fl.StartLimitRole = StartLimitRole.ResultIsNotZero;
            }

            fl.Update();
        }
    }
}