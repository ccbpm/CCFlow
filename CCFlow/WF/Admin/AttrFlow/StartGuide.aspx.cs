using BP.Web.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Web.UI.HtmlControls;
using BP.WF;
using BP.DA;
using BP.Sys;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class StartGuide1 : System.Web.UI.Page
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string SID
        {
            get
            {
                return BP.Web.WebUser.SID;
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack==false)
            {
                BP.WF.Flow en = new BP.WF.Flow(this.FK_Flow);
                en.No = this.FK_Flow;
                en.RetrieveFromDBSources();

                switch (en.StartGuideWay)
                {
                    case  BP.WF.Template.StartGuideWay.None://无
                        this.RB_None.Checked = true;
                        break;
                    case BP.WF.Template.StartGuideWay.ByHistoryUrl: //从开始节点Copy数据
                        this.RB_ByHistoryUrl.Checked = true;
                        this.TB_ByHistoryUrl.Value = en.StartGuidePara1;
                        break;
                    case  BP.WF.Template.StartGuideWay.BySelfUrl://按自定义的Url
                        this.RB_SelfUrl.Checked = true;
                        this.TB_SelfURL.Value = en.StartGuidePara1;
                        break;
                    case BP.WF.Template.StartGuideWay.BySQLOne: //按照参数.
                        this.RB_BySQLOne.Checked = true;
                        this.TB_BySQLOne1.Value = en.StartGuidePara1;
                        this.TB_BySQLOne2.Value = en.StartGuidePara2;
                        break;
                    case BP.WF.Template.StartGuideWay.ByFrms:
                        this.RB_FrmList.Checked = true;
                        break;
                    case BP.WF.Template.StartGuideWay.SubFlowGuide: //子父流程多条模式- 合卷审批.
                           this.RB_SubFlow.Checked = true;
                           this.TB_SubFlow1.Value = en.StartGuidePara1;
                           this.TB_SubFlow2.Value = en.StartGuidePara2;
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 保存.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            BP.WF.Flow en = new BP.WF.Flow(this.FK_Flow);
            en.No = this.FK_Flow;
            en.RetrieveFromDBSources();

            if (this.RB_None.Checked)
            {
                en.StartGuideWay = BP.WF.Template.StartGuideWay.None;
            }

            if (this.RB_ByHistoryUrl.Checked)
            {
                en.StartGuidePara1 = this.TB_ByHistoryUrl.Value;
                en.StartGuidePara2 = "";
                en.StartGuideWay = BP.WF.Template.StartGuideWay.ByHistoryUrl;
            }

            if (this.RB_SelfUrl.Checked)
            {
                en.StartGuidePara1 = this.TB_SelfURL.Value;
                en.StartGuidePara2 = "";
                en.StartGuideWay = BP.WF.Template.StartGuideWay.BySelfUrl;
            }

            //单条模式.
            if (this.RB_BySQLOne.Checked)
            {
                en.StartGuidePara1 = this.TB_BySQLOne1.Value;  //查询语句.
                en.StartGuidePara2 = this.TB_BySQLOne2.Value;  //列表语句.
                en.StartGuideWay = BP.WF.Template.StartGuideWay.BySQLOne;
            }

            //多条-子父流程-合卷审批.
            if (this.RB_SubFlow.Checked)
            {
                en.StartGuidePara1 = this.TB_SubFlow1.Value;  //查询语句.
                en.StartGuidePara2 = this.TB_SubFlow2.Value;  //列表语句.
                en.StartGuideWay = BP.WF.Template.StartGuideWay.SubFlowGuide;
            }



            BP.WF.Template.FrmNodes fns = new BP.WF.Template.FrmNodes(int.Parse(this.FK_Flow + "01"));
            if (fns.Count >= 2)
            {
                if (this.RB_FrmList.Checked)
                    en.StartGuideWay = BP.WF.Template.StartGuideWay.ByFrms;
            }
            en.Update();
            en.DirectUpdate();

        }
    }
}