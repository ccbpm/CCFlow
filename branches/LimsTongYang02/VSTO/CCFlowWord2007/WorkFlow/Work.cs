using System;
using System.Collections.Generic;
using System.Data;
using BP.DA;

namespace BP.WF
{
    /// <summary>
    /// 工作
    /// </summary>
    public class Work
    {
        #region Property
        public int NodeID;
        public Int64 OID;
        public Int64 FID;
        public string FK_Dept;
        public int NodeState;
        public string RDT;
        public string CDT;
        public string Rec;
        public string Title;
        public string PTable
        {
            get
            {
                return "ND" + this.NodeID.ToString();
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public WFState HisWFState
        {
            get;
            set;
        }
        #endregion

        #region Construction

        public Work()
        {
        }

        /// <summary>
        /// 工作类
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="workid"></param>
        public Work(int nodeId, Int64 workid)
        {
            this.NodeID = nodeId;
            this.OID = workid;

            string sql = "SELECT * FROM " + this.PTable + " WHERE OID=" + workid;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@无此节点数据.");

            this.FID = int.Parse(dt.Rows[0]["FID"].ToString());
            this.FK_Dept = dt.Rows[0]["FK_Dept"].ToString();
            this.RDT = dt.Rows[0]["RDT"].ToString();
            this.CDT = dt.Rows[0]["CDT"].ToString();
            this.Rec = dt.Rows[0]["Rec"].ToString();
            this.Title = dt.Rows[0]["Title"].ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 执行更新
        /// </summary>
        public void Update()
        {
            DBAccess.RunSQL("UPDATE " + this.PTable + " SET Title='" + this.Title + "', RDT='" + this.RDT + "' WHERE OID=" + this.OID);
        }
        /// <summary>
        /// 执行插入
        /// </summary>
        public int Insert()
        {
            this.OID = DBAccess.GenerOID();
            string sql = "INSERT INTO ND" + this.NodeID + " (OID,FID,FK_Dept,NodeState,RDT,CDT,Rec,Title) VALUES (" + this.OID + "," + this.FID + ",'" + this.FK_Dept + "'," + this.NodeState + ",'" + this.RDT + "','" + this.CDT + "','" + this.Rec + "','" + this.Title + "')";
            DBAccess.RunSQL(sql);
            return (int)this.OID;
        }
        #endregion
    }
}
