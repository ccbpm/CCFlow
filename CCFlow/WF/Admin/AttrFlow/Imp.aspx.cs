using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web;
using BP.Web.Comm;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class Imp : WebPage
    {
        #region 属性
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
        /// 流程类别编号
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                string flowSort = this.Request.QueryString["FK_FlowSort"];
                if (string.IsNullOrEmpty(flowSort))
                {
                    BP.WF.Flow flow = new BP.WF.Flow();
                    if (this.FK_Flow != null)
                        flow = new BP.WF.Flow(this.FK_Flow);
                    flowSort = flow.FK_FlowSort;
                }
                return flowSort;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (FU_Upload.HasFile)
            {
                string StrsavePath = Server.MapPath("..//..//..//DataUser//FlowFile");//路径
                StrsavePath = StrsavePath + "//" + FU_Upload.FileName;
                FU_Upload.SaveAs(StrsavePath);//保存文件

                int SpecifiedNumber = 0;
                BP.WF.ImpFlowTempleteModel model = BP.WF.ImpFlowTempleteModel.AsNewFlow;
                //作为新流程导入(由ccbpm自动生成新的流程编号)
                if (Import_1.Checked)
                {
                    model = BP.WF.ImpFlowTempleteModel.AsNewFlow;
                }
                //作为新流程导入(使用流程模版里面的流程编号，如果该编号已经存在系统则会提示错误)
                if (Import_2.Checked)
                {
                    model = BP.WF.ImpFlowTempleteModel.AsTempleteFlowNo;
                }
                //作为新流程导入(使用流程模版里面的流程编号，如果该编号已经存在系统则会覆盖此流程)
                if (Import_3.Checked)
                {
                    model = BP.WF.ImpFlowTempleteModel.OvrewaiteCurrFlowNo;
                }
                //导入并覆盖当前的流程
                if (Import_4.Checked)
                {
                    String StrSpecifiedNumber = this.SpecifiedNumber.Text;
                    if (StrSpecifiedNumber == null)
                    {
                        this.Alert("@请输入指定流程编号。");
                        return;
                    }

                    SpecifiedNumber = Convert.ToInt32(StrSpecifiedNumber);
                    model = BP.WF.ImpFlowTempleteModel.AsSpecFlowNo;
                }
                //执行导入
                BP.WF.Flow flow = BP.WF.Flow.DoLoadFlowTemplate(this.FK_FlowSort, StrsavePath, model, SpecifiedNumber);
                if (flow.No != "")
                {
                    this.Alert("导入成功");
                    this.Button1.Enabled = false;
                }
                else
                {
                    this.Alert("导入失败");
                }
            }
            else
            {
                this.Alert("请您选择上传的文件 文件格式为xml");
            }
        }
    }
}