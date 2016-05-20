using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;
using System.Collections.Specialized;

namespace CCFlow.Web.WF.Comm
{
    /// <summary>
    /// GroupEnsDtl 的摘要说明。
    /// </summary>
    public partial class UIContrastDtl : BP.Web.WebPage
    {
        #region 属性。
        public string FK_Dept
        {
            get
            {
                return (string)ViewState["FK_Dept"];
            }
            set
            {
                string val = value;
                if (val == "all")
                    return;

                if (this.FK_Dept == null)
                {
                    ViewState["FK_Dept"] = value;
                    return;
                }

                if (this.FK_Dept.Length > val.Length)
                    return;

                ViewState["FK_Dept"] = value;
            }
        }
        public string ShowTitle { get; set; }
        #endregion 属性。

        protected void Page_Load(object sender, System.EventArgs e)
        {
             
            this.BindData();
        }
        public void BindData()
        {
            string ensname = this.Request.QueryString["EnsName"];
            if (ensname == null)
                ensname = this.Request.QueryString["EnsName"];

            Entities ens = BP.En.ClassFactory.GetEns(ensname);
            Entity en = ens.GetNewEntity;

            QueryObject qo = new QueryObject(ens);
            string[] strs = this.Request.RawUrl.Split('&');
            string[] strs1 = this.Request.RawUrl.Split('&');

            foreach (string str in strs)
            {
                if (str.IndexOf("EnsName") != -1)
                    continue;

                string[] mykey = str.Split('=');
                string key = mykey[0];

                if (key == "OID" || key == "MyPK")
                    continue;

                if (key == "FK_Dept")
                {
                    this.FK_Dept = mykey[1];
                    continue;
                }

                if (en.EnMap.Attrs.Contains(key) == false)
                    continue;

                if (mykey[1] == "mvals")
                {
                    //如果用户多项选择了，就要找到它的选择项目.

                    UserRegedit sUr = new UserRegedit();
                    sUr.MyPK = WebUser.No + this.EnsName + "_SearchAttrs";
                    sUr.RetrieveFromDBSources();

                    /* 如果是多选值 */
                    string cfgVal = sUr.MVals;
                    AtPara ap = new AtPara(cfgVal);
                    string instr = ap.GetValStrByKey(key);
                    string val = "";
                    if (instr == null || instr == "")
                    {
                        if (key == "FK_Dept" || key == "FK_Unit")
                        {
                            if (key == "FK_Dept")
                                val = WebUser.FK_Dept;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        instr = instr.Replace("..", ".");
                        instr = instr.Replace(".", "','");
                        instr = instr.Substring(2);
                        instr = instr.Substring(0, instr.Length - 2);
                        qo.AddWhereIn(mykey[0], instr);
                    }
                }
                else
                {
                    qo.AddWhere(mykey[0], mykey[1]);
                }
                qo.addAnd();
            }

            if (this.FK_Dept != null && (this.Request.QueryString["FK_Emp"] == null
                || this.Request.QueryString["FK_Emp"] == "all"))
            {
                if (this.FK_Dept.Length == 2)
                {
                    qo.AddWhere("FK_Dept", " = ", "all");
                    qo.addAnd();
                }
                else
                {
                    if (this.FK_Dept.Length == 8)
                    {
                        //if (this.Request.QueryString["ByLike"] != "1")
                        qo.AddWhere("FK_Dept", " = ", this.FK_Dept);
                    }
                    else
                    {
                        qo.AddWhere("FK_Dept", " like ", this.FK_Dept + "%");
                    }

                    qo.addAnd();
                }
            }

            qo.AddHD();

            if (this.DoType == "Exp")
            {
                /*如果是导出，就把它导出到excel.*/
                this.ExportDGToExcel(qo.DoQueryToTable(), en.EnMap, en.EnDesc + "数据导出");
                this.WinClose();
                return;
            }
            int num = qo.DoQuery();

            // Log.DebugWriteWarning(qo.SQL);
            // Log.DefaultLogWriteLineError(qo.SQL);

            this.ShowTitle = ens.GetNewEntity.EnMap.EnDesc + "，数据：" + num + " 条。";
            this.UCSys1.DataPanelDtl(ens, null);
        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}
