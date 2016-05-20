using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Demo;
using System.Collections;

namespace CCFlow.SDKFlowDemo.Comm
{
    public partial class Track1 : BP.Web.UC.UCBase3
    {
        #region 流程引擎传来的变量.
        /// <summary>
        /// 工作ID，在建立草稿时已经产生了.
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                try
                {
                    return Int64.Parse(this.Request.QueryString["WorkID"]);
                }
                catch
                {
                    return 0;
                }
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
        /// <summary>
        ///  流程编号.
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
        #endregion 流程引擎传来的变量

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.WorkID == 0)
                return;

            this.AddFieldSet("流程运转记录.");

            DataSet ds=BP.WF.Dev2Interface.DB_GenerTrack(this.FK_Flow,this.WorkID,this.FID);
            DataTable dt = ds.Tables["Track"];

            this.AddTable();
            this.AddTR();
            this.AddTDTitle("IDX");
            this.AddTDTitle("日期时间");
            this.AddTDTitle("从节点");
            this.AddTDTitle("人员");
            this.AddTDTitle("到节点");
            this.AddTDTitle("人员");
            this.AddTDTitle("活动");
            this.AddTDTitle("信息");
            this.AddTDTitle("执行人");
            this.AddTREnd();

            DataView dv = dt.DefaultView;
            dv.Sort = "RDT";

            int idx = 1;
            foreach (DataRowView dr in dv)
            {
                this.AddTR();
                this.AddTDIdx(idx++);
                DateTime dtt = DataType.ParseSysDateTime2DateTime(dr[TrackAttr.RDT].ToString());
                this.AddTD(dtt.ToString("MM月dd日HH:mm"));

                this.AddTD(dr[TrackAttr.NDFromT].ToString());
                this.AddTD(dr[TrackAttr.EmpFromT].ToString());

                this.AddTD(dr[TrackAttr.NDToT].ToString());
                this.AddTD(dr[TrackAttr.EmpToT].ToString());
                ActionType at = (ActionType)int.Parse(dr[TrackAttr.ActionType].ToString());
                this.AddTD("<img src='/WF/Img/Action/" + at.ToString() + ".png' class='ICON' border=0/>" + dr[TrackAttr.ActionTypeText].ToString());
                this.AddTD(DataType.ParseText2Html(dr[TrackAttr.Msg].ToString()));
               // this.AddTD("<a href=\"javascript:WinOpen('" + this.Request.ApplicationPath + "WF/WFRpt.aspx?WorkID=" + dr[TrackAttr.WorkID].ToString() + "&FK_Flow=" + this.FK_Flow + "&DoType=View&MyPK=" + dr[TrackAttr.MyPK].ToString() + "','" + dr[TrackAttr.MyPK].ToString() + "');\">表单</a>");
                this.AddTD(dr[TrackAttr.Exer].ToString());
                this.AddTREnd();
            }
            this.AddTableEnd();

            this.AddFieldSetEnd();
        }
    }
}