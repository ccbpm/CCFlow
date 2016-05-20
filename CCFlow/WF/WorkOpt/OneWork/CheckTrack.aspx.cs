using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Sys;

namespace ShenZhenGovOA.WF.WorkOpt.OneWork
{
    public partial class CheckTrack : System.Web.UI.Page
    {
        #region 属性
        public string CCID
        {
            get
            {
                return this.Request.QueryString["CCID"];
            }
        }
        public   string DoType
        {
            get
            {
                return this.Pub2.Request.QueryString["DoType"];
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Pub2.Request.QueryString["FK_Node"]);
            }
        }
        public int StartNodeID
        {
            get
            {
                return int.Parse(this.FK_Flow + "01");
            }
        }
        public string FK_Flow
        {
            get
            {
                string flow = this.Request.QueryString["FK_Flow"];
                if (flow == null)
                {
                    throw new Exception("@没有获取它的流程编号。");
                }
                else
                {
                    return flow;
                }
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Pub2.Request.QueryString["WorkID"]);
            }
        }
        public int NodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Pub2.Request.QueryString["NodeID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int FID
        {
            get
            {
                try
                {
                    return int.Parse(this.Pub2.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            FreeModel();
        }

        public void FreeModel()
        {
            string sqlOfWhere1 = "";

            string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras prs = new Paras();
            if (this.FID == 0)
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "WorkID11 OR WorkID=" + dbStr + "WorkID12 )  ";
                prs.Add("WorkID11", this.WorkID);
                prs.Add("WorkID12", this.WorkID);
            }
            else
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "FID11 OR WorkID=" + dbStr + "FID12 ) ";
                prs.Add("FID11", this.FID);
                prs.Add("FID12", this.FID);
            }

            string sql = "";
            sql = "SELECT MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(this.FK_Flow) + "Track " + sqlOfWhere1;
            prs.SQL = sql;
            DataTable dt = null;
            try
            {
                dt = DBAccess.RunSQLReturnTable(prs);
            }
            catch
            {
                // 处理track表.
                Track.CreateOrRepairTrackTable(this.FK_Flow);
                dt = DBAccess.RunSQLReturnTable(prs);
            }

            DataView dv = dt.DefaultView;
            dv.Sort = "RDT";

            foreach (DataRowView dr in dv)
            {
                ActionType at = (ActionType)int.Parse(dr[TrackAttr.ActionType].ToString());
                if (at != ActionType.WorkCheck)
                    continue;

                DateTime dtt = DataType.ParseSysDateTime2DateTime(dr[TrackAttr.RDT].ToString());
                string rdt = dtt.ToString("yyyy年MM月dd日HH:mm");
                string ndName = dr[TrackAttr.NDFromT].ToString();
                string empName = dr[TrackAttr.EmpFromT].ToString();
                string info = dr[TrackAttr.Msg].ToString();

                this.Pub2.AddH3(" ---- 节点:" + ndName);
                this.Pub2.Add(DataType.ParseText2Html(info));
                this.Pub2.Add("<br><div style='float:right'>" + empName + " - " + rdt + "&nbsp;&nbsp;&nbsp;&nbsp;</div><br><hr>");
            }

            //if (this.CCID != null)
            //{
            //    CCList cl = new CCList();
            //    cl.MyPK = this.CCID;
            //    cl.RetrieveFromDBSources();
            //    this.Pub2.AddFieldSet(cl.Title);
            //    this.Pub2.Add("抄送人:" + cl.Rec + ", 抄送日期:" + cl.RDT);
            //    this.Pub2.AddHR();
            //    this.Pub2.Add(cl.DocHtml);
            //    this.Pub2.AddFieldSetEnd();
            //    if (cl.HisSta == CCSta.UnRead)
            //    {
            //        cl.HisSta = CCSta.Read;
            //        cl.Update();
            //    }
            //}
        }
        public void TableMode()
        {
            this.Pub2.AddTable();
            this.Pub2.AddTR();
            this.Pub2.AddTDTitle("IDX");
            this.Pub2.AddTDTitle("日期时间");
            this.Pub2.AddTDTitle("从节点");
            this.Pub2.AddTDTitle("人员");
            this.Pub2.AddTDTitle("到节点");
            this.Pub2.AddTDTitle("人员");
            this.Pub2.AddTDTitle("活动");
            this.Pub2.AddTDTitle("信息");
            this.Pub2.AddTDTitle("表单");
            this.Pub2.AddTDTitle("执行人");
            this.Pub2.AddTREnd();

            string sqlOfWhere1 = "";

            string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras prs = new Paras();
            if (this.FID == 0)
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "WorkID11 OR WorkID=" + dbStr + "WorkID12 )  ";
                prs.Add("WorkID11", this.WorkID);
                prs.Add("WorkID12", this.WorkID);
            }
            else
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "FID11 OR WorkID=" + dbStr + "FID12 ) ";
                prs.Add("FID11", this.FID);
                prs.Add("FID12", this.FID);
            }

            string sql = "";
            sql = "SELECT MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(this.FK_Flow) + "Track " + sqlOfWhere1;
            prs.SQL = sql;
            DataTable dt = null;
            try
            {
                dt = DBAccess.RunSQLReturnTable(prs);
            }
            catch
            {
                // 处理track表.
                Track.CreateOrRepairTrackTable(this.FK_Flow);
                dt = DBAccess.RunSQLReturnTable(prs);
            }

            DataView dv = dt.DefaultView;
            dv.Sort = "RDT";

            int idx = 1;
            foreach (DataRowView dr in dv)
            {
                ActionType at = (ActionType)int.Parse(dr[TrackAttr.ActionType].ToString());
                if (at != ActionType.WorkCheck)
                    continue;

                this.Pub2.AddTR();
                this.Pub2.AddTDIdx(idx++);
                DateTime dtt = DataType.ParseSysDateTime2DateTime(dr[TrackAttr.RDT].ToString());
                this.Pub2.AddTD(dtt.ToString("yyyy年MM月dd日HH:mm"));

                this.Pub2.AddTD(dr[TrackAttr.NDFromT].ToString());


                this.Pub2.AddTD(dr[TrackAttr.EmpFromT].ToString());
                this.Pub2.AddTD(dr[TrackAttr.NDToT].ToString());
                this.Pub2.AddTD(dr[TrackAttr.EmpToT].ToString());

                this.Pub2.AddTD("<img src='../../Img/Action/" + at.ToString() + ".png' class='ActionType' border=0/>" + BP.WF.Track.GetActionTypeT(at));

                // 删除信息
                string tag = dr[TrackAttr.Tag].ToString();
                string msg = dr[TrackAttr.Msg].ToString();
                switch (at)
                {
                    case ActionType.CallChildenFlow: //被调用父流程吊起。
                        tag = dr[TrackAttr.Tag].ToString();
                        if (string.IsNullOrEmpty(tag) == false)
                        {
                            AtPara ap = new AtPara(tag);
                            this.Pub2.AddTD("class=TD", "<a target=b" + ap.GetValStrByKey("PWorkID") + " href='Track.aspx?WorkID=" + ap.GetValStrByKey("PWorkID") + "&FK_Flow=" + ap.GetValStrByKey("PFlowNo") + "' >" + msg + "</a>");
                        }
                        else
                        {
                            this.Pub2.AddTD("class=TD", msg);
                        }
                        break;
                    case ActionType.StartChildenFlow: //吊起子流程。
                        tag = dr[TrackAttr.Tag].ToString();
                        if (string.IsNullOrEmpty(tag) == false)
                        {
                            AtPara ap = new AtPara(tag);
                            this.Pub2.AddTD("class=TD", "<a target=b" + ap.GetValStrByKey("CWorkID") + " href='Track.aspx?WorkID=" + ap.GetValStrByKey("CWorkID") + "&FK_Flow=" + ap.GetValStrByKey("CFlowNo") + "' >" + msg + "</a>");
                        }
                        else
                        {
                            this.Pub2.AddTD("class=TD", msg);
                        }
                        break;
                    default:
                        this.Pub2.AddTD(DataType.ParseText2Html(msg));
                        break;
                }

                this.Pub2.AddTD("<a href=\"javascript:WinOpen('" + BP.WF.Glo.CCFlowAppPath + "WF/WFRpt.aspx?WorkID=" + dr[TrackAttr.WorkID].ToString() + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + dr[TrackAttr.NDTo].ToString() + "&DoType=View&MyPK=" + dr[TrackAttr.MyPK].ToString() + "','" + dr[TrackAttr.MyPK].ToString() + "');\">表单</a>");
                this.Pub2.AddTD(dr[TrackAttr.Exer].ToString());
                this.Pub2.AddTREnd();
            }
            this.Pub2.AddTableEnd();

            if (this.CCID != null)
            {
                CCList cl = new CCList();
                cl.MyPK = this.CCID;
                cl.RetrieveFromDBSources();
                this.Pub2.AddFieldSet(cl.Title);
                this.Pub2.Add("抄送人:" + cl.Rec + ", 抄送日期:" + cl.RDT);
                this.Pub2.AddHR();
                this.Pub2.Add(cl.DocHtml);
                this.Pub2.AddFieldSetEnd();
                if (cl.HisSta == CCSta.UnRead)
                {
                    cl.HisSta = CCSta.Read;
                    cl.Update();
                }
            }
        }
    }
}