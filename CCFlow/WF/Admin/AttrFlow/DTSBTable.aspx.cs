using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.WF;
using BP.WF.Template;
using BP.DA;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class DTSBTable : System.Web.UI.Page
    {
        public string FK_Flow
        {
            get
            {
                string str = this.Request.QueryString["FK_Flow"];
                if (string.IsNullOrEmpty(str))
                    return "001";
                return str;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                BP.WF.Flow fl = new BP.WF.Flow(this.FK_Flow);

                //设置状态.
                if (fl.DTSWay == BP.WF.Template.FlowDTSWay.None)
                    this.RB_DTSWay0.Checked = true;
                else
                    this.RB_DTSWay1.Checked = true;

                //绑定数据源.
                BP.Sys.SFDBSrcs srcs = new BP.Sys.SFDBSrcs();
                srcs.RetrieveAll();
                BP.Web.Controls.Glo.DDL_BindEns(this.DDL_DBSrc, srcs, fl.DTSDBSrc);

                //设置自动.
                this.DDL_DBSrc.AutoPostBack = true;
                this.DDL_DBSrc.SelectedIndexChanged += new EventHandler(DDL_DBSrc_SelectedIndexChanged);

                //绑定表.
                BP.Sys.SFDBSrc src = new SFDBSrc(fl.DTSDBSrc);
                DataTable dt = src.GetTables();
                dt = RemoveView(dt);//去除视图

                BP.Web.Controls.Glo.DDL_BindDataTable(this.DDL_Table, dt, fl.DTSBTable);
                //设置自动.
                this.DDL_Table.AutoPostBack = true;
                this.DDL_Table.SelectedIndexChanged += new EventHandler(DDL_Table_SelectedIndexChanged);

                //绑定字段同步的方式.
                if (fl.DTSField == BP.WF.Template.DTSField.SameNames)
                    this.RB_DTSField0.Checked = true;
                else
                    this.RB_DTSField1.Checked = true;

                //绑定同步的时间.
                if (fl.DTSTime == BP.WF.Template.FlowDTSTime.AllNodeSend)
                {
                    this.RB_DTSTime0.Checked = true;
                }
                if (fl.DTSTime == BP.WF.Template.FlowDTSTime.SpecNodeSend)
                {
                    this.RB_DTSTime1.Checked = true;
                }
                if (fl.DTSTime == BP.WF.Template.FlowDTSTime.WhenFlowOver)
                {
                    this.RB_DTSTime2.Checked = true;
                }
            }
        }
        /// <summary>
        /// 去除视图
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable RemoveView(DataTable dt)
        {

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i]["xtype"].ToString().ToUpper() == "V")
                {
                    dt.Rows.RemoveAt(i);
                }
            }

            return dt;
        }

        public void DDL_DBSrc_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dbsrc = this.DDL_DBSrc.SelectedValue;

            //绑定表. 
            BP.Sys.SFDBSrc src = new SFDBSrc(dbsrc);
            DataTable dt = src.GetTables();

            dt = RemoveView(dt);

            Flow fl = new Flow(this.FK_Flow);
            BP.Web.Controls.Glo.DDL_BindDataTable(this.DDL_Table, dt, fl.DTSBTable);

            //System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'>   window.parent.closeTab('设置字段匹配'); </script> ");

            System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'>" +
                "try{window.parent.closeTab('设置字段匹配');}catch{} </script> ");
        }
        /// <summary>
        /// 保存数据同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            Flow flow = new Flow(this.FK_Flow);

            #region 不同步
            if (this.RB_DTSWay0.Checked)
            {
                flow.DTSWay = FlowDTSWay.None;
            }
            #endregion

            #region 同步
            if (this.RB_DTSWay1.Checked)
            {
                flow.DTSDBSrc = this.DDL_DBSrc.SelectedValue;
                flow.DTSBTable = this.DDL_Table.SelectedValue;

                DTSField field = DTSField.SameNames;

                if (this.RB_DTSField1.Checked)
                    field = DTSField.SpecField;

                flow.DTSField = field;

                FlowDTSTime time = FlowDTSTime.AllNodeSend;

                if (this.RB_DTSTime1.Checked)
                    time = FlowDTSTime.SpecNodeSend;

                if (this.RB_DTSTime2.Checked)
                    time = FlowDTSTime.WhenFlowOver;

                if (time == FlowDTSTime.SpecNodeSend)
                {
                    string specNodes = this.HiddenField.Value.TrimEnd(',');
                    if (string.IsNullOrEmpty(specNodes))
                    {
                        PubClass.Alert("没有设置要同步的节点");
                        return;
                    }
                    else
                    {
                        flow.DTSSpecNodes = specNodes.TrimEnd(',');
                    }
                }

                flow.DTSTime = time;
                flow.DTSWay = FlowDTSWay.Syn;

                #region 字段名相同
                SFDBSrc s = new SFDBSrc("local");
                if (field == DTSField.SameNames)
                {
                    DataTable dt = s.GetColumns(flow.PTable);

                    s = new SFDBSrc(this.DDL_DBSrc.SelectedValue);// this.src);
                    DataTable ywDt = s.GetColumns(this.DDL_Table.SelectedValue);// this.ywTableName);

                    string str = "";
                    string ywStr = "";
                    foreach (DataRow ywDr in ywDt.Rows)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (ywDr["No"].ToString().ToUpper() == dr["No"].ToString().ToUpper())
                            {
                                if (dr["No"].ToString().ToUpper() == "OID")
                                {
                                    flow.DTSBTablePK = "OID";
                                }
                                str += dr["No"].ToString() + ",";
                                ywStr += ywDr["No"].ToString() + ",";
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(str))
                        flow.DTSFields = str.TrimEnd(',') + "@" + ywStr.TrimEnd(',');
                    else
                    {
                        PubClass.Alert("未检测到业务主表【" + flow.PTable + "】与表【" + this.DDL_Table.SelectedValue + "】有相同的字段名.");
                        return;//不执行保存
                    }
                }
                else//按设置的字段匹配   检查在
                {
                    try
                    {
                        s = new SFDBSrc("local");
                        string str = flow.DTSFields;

                        string[] arr = str.Split('@');


                        string sql = "SELECT " + arr[0] + " FROM " + flow.PTable;

                        s.RunSQL(sql);

                        s = new SFDBSrc(this.DDL_DBSrc.SelectedValue);

                        sql = "SELECT " + arr[1] + ", " + flow.DTSBTablePK
                            + " FROM " + flow.DTSBTable;

                        s.RunSQL(sql);

                    }
                    catch
                    {
                        //PubClass.Alert(ex.Message);
                        PubClass.Alert("设置的字段有误.【" + flow.DTSFields + "】");
                        return;//不执行保存
                    }
                }
                #endregion
            }
            #endregion

            flow.Update();
        }

        //值改变时,关闭配置页面,防止使用旧的表来配置
        protected void DDL_Table_SelectedIndexChanged(object sender, EventArgs e)
        {
            //兼容旧版本,全部升级之后删除2
            //System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'>" +
            //                " window.parent.closeTab('设置字段匹配'); </script> ");//1

            System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'>" +
           " try{window.parent.closeTab('设置字段匹配');}catch{} </script> ");//2
        }

    }
}