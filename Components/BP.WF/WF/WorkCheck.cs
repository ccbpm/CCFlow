using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 审核工作节点
    /// </summary>
    public class WorkCheck
    {
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID=0;
        public Int64 FID = 0;
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID = 0;
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FlowNo = null;
        public WorkCheck(string flowNo, int nodeID, Int64 workid,Int64 fid)
        {
            this.FlowNo = flowNo;
            this.NodeID = nodeID;
            this.WorkID = workid;
            this.FID = fid;
        }
        /// <summary>
        /// 获取主键32位
        /// </summary>
        /// <returns></returns>
        public int GetMyPK32()
        {
            try
            {
                int newPK = int.Parse(this.WorkID.ToString()) + this.NodeID + int.Parse(this.FlowNo);
                string myPk = "";
                string sql = "SELECT TOP 1 RDT FROM WF_GenerWorkerlist WHERE WorkID={0} AND FK_Node={1} AND FK_Flow='{2}' ORDER BY RDT DESC";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(string.Format(sql, this.WorkID, this.NodeID, this.FlowNo));
                if (dt != null && dt.Rows.Count > 0)
                {
                    myPk = dt.Rows[0]["RDT"].ToString();
                    myPk = myPk.Replace("-", "").Replace(":", "").Replace(" ", "");
                    myPk = myPk.Substring(4);
                    newPK = int.Parse(this.WorkID.ToString()) + this.NodeID + int.Parse(this.FlowNo) + int.Parse(myPk);
                }
                return newPK;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取主键
        /// </summary>
        /// <returns></returns>
        public Int64 GetMyPK()
        {
            try
            {
                Int64 newPK = Int64.Parse(this.WorkID.ToString()) + this.NodeID + Int64.Parse(this.FlowNo);
                string myPk = "";
                string sql = "SELECT TOP 1 RDT FROM WF_GenerWorkerlist WHERE WorkID={0} AND FK_Node={1} AND FK_Flow='{2}' ORDER BY RDT DESC";


                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(string.Format(sql, this.WorkID, this.NodeID, this.FlowNo));
                if (dt != null && dt.Rows.Count > 0)
                {
                    myPk = dt.Rows[0]["RDT"].ToString();
                    myPk = myPk.Replace("-", "").Replace(":", "").Replace(" ", "");
                    myPk = myPk.Substring(2);
                    newPK = Int64.Parse(this.WorkID.ToString()) + this.NodeID + Int64.Parse(this.FlowNo) + Int64.Parse(myPk);
                }
                return newPK;
            }
            catch (Exception ex)
            {
                return 0;  
            }
        }
        public Tracks HisWorkChecks
        {
            get
            {
                if (_HisWorkChecks == null)
                {
                    _HisWorkChecks = new Tracks();
                    BP.En.QueryObject qo = new En.QueryObject(_HisWorkChecks);

                    if (this.FID != 0)
                    {
                        qo.AddWhereIn(TrackAttr.WorkID, "(" + this.WorkID + "," + this.FID + ")");
                    }
                    else
                    {
                        qo.AddWhere(TrackAttr.WorkID, this.WorkID);

                        if (this.WorkID != 0)
                        {
                            qo.addOr();
                            qo.AddWhere(TrackAttr.FID, this.WorkID);
                        }
                    }

                    qo.addOrderBy(TrackAttr.RDT);

                    string sql = qo.SQL;
                    sql = sql.Replace("WF_Track", "ND" + int.Parse(this.FlowNo) + "Track");
                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, qo.MyParas);

                    dt.DefaultView.Sort = "RDT desc";

                    BP.En.Attrs attrs = _HisWorkChecks.GetNewEntity.EnMap.Attrs;
                    foreach (DataRow dr in dt.Rows)
                    {
                        Track en = new Track();
                        foreach (BP.En.Attr attr in attrs)
                            en.Row.SetValByKey(attr.Key, dr[attr.Key]);

                        _HisWorkChecks.AddEntity(en);
                    }
                }
                return _HisWorkChecks;
            }
        }
        private Tracks _HisWorkChecks = null;
        public Tracks HisWorkChecks_New
        {
            get
            {
                if (_HisWorkChecks == null)
                {
                    _HisWorkChecks = new Tracks();

                    BP.DA.Paras ps = new BP.DA.Paras();
                    string sql = "SELECT * FROM ND" + int.Parse(this.FlowNo) + "Track WHERE ";
                    string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                    if (this.FID == 0)
                    {
                        // 为了兼容多种数据库，所以使用了两个相同的参数.
                        sql += " WorkID=" + dbstr + "WorkID OR FID=" + dbstr + "FID ORDER BY RDT DESC ";
                        ps.Add("WorkID", this.WorkID);
                        ps.Add("FID", this.WorkID);
                    }
                    else
                    {
                        //sql += " WorkID=" + dbstr + "WorkID OR WorkID=" + dbstr + "FID ORDER BY RDT DESC ";
                        // ps.Add("WorkID", this.WorkID);
                        // ps.Add("FID", this.FID);
                     //   qo.AddWhereIn(TrackAttr.WorkID, "(" + this.WorkID + "," + this.FID + ")");

                        sql += " WorkID=" + dbstr + "WorkID OR WorkID=" + dbstr + "FID ORDER BY RDT DESC ";
                        ps.Add("WorkID", this.WorkID);
                        ps.Add("FID", this.FID);
                    }

                    ps.SQL = sql;

                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);

                    //获得Attrs
                    BP.En.Attrs attrs=_HisWorkChecks.GetNewEntity.EnMap.Attrs;
                     foreach (DataRow dr in dt.Rows)
                    {
                        Track en = new Track();
                        foreach (BP.En.Attr attr in attrs)
                            en.Row.SetValByKey(attr.Key, dr[attr.Key]);

                        _HisWorkChecks.AddEntity(en);
                    }
                }
                return _HisWorkChecks;
            }
        }
    }
}
