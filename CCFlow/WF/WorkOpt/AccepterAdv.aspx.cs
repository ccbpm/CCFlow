using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BP.DA;
using BP.WF.Template;
using BP.WF;
using BP.Sys;
using BP.Web;

namespace CCFlow.WF.WorkOpt
{
    public partial class AccepterAdv : System.Web.UI.Page
    {
        #region 属性
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        private string FK_Flow
        {
            get
            {
                return this.Request["FK_Flow"];
            }
        }
        /// <summary>
        /// 流程节点编号
        /// </summary>
        private string FK_Node
        {
            get
            {
                return this.Request["FK_Node"];
            }
        }
        /// <summary>
        /// 工作编号
        /// </summary>
        private long WorkID
        {
            get
            {
                string workId = this.Request["WorkID"];
                if (!string.IsNullOrEmpty(workId))
                    return long.Parse(workId);
                return 0;
            }
        }
        /// <summary>
        /// 工作编号父编号
        /// </summary>
        private string FID
        {
            get
            {
                return this.Request["FID"];
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
                return;
            method = Request["method"].ToString();

            switch (method)
            {
                case "getreservewords"://获取常用词汇
                    s_responsetext = GetReserveWords();
                    break;
                case "getdeliverynode"://获取节点
                    s_responsetext = GetDeliveryNodes();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }

        /// <summary>
        /// 获取备用词汇
        /// </summary>
        /// <returns></returns>
        private string GetReserveWords()
        {
            StringBuilder append = new StringBuilder();
            Node curNode = new Node();
            bool isHave = curNode.RetrieveByAttr(NodeAttr.NodeID, FK_Node.Replace("ND", "").Replace("_CC", ""));
            StringBuilder leftWords = new StringBuilder();
            StringBuilder rightWords = new StringBuilder();

            //节点存在
            if (isHave)
            {
                //公文左边词语
                if (!string.IsNullOrEmpty(curNode.DocLeftWord))
                {
                    string[] arraryWord = curNode.DocLeftWord.Split('@');
                    foreach (string str in arraryWord)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;

                        if (leftWords.Length > 0) leftWords.Append(",");

                        leftWords.Append("{word:'" + str + "'}");
                    }
                }
                //处理公文右边词语
                if (!string.IsNullOrEmpty(curNode.DocRightWord))
                {
                    string[] arraryWord = curNode.DocRightWord.Split('@');
                    foreach (string str in arraryWord)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;

                        if (rightWords.Length > 0) rightWords.Append(",");

                        rightWords.Append("{word:'" + str + "'}");
                    }
                }
            }
            append.Append("{");
            append.Append("LeftWords:[");
            append.Append(leftWords);
            append.Append("],");
            append.Append("RightWords:[");
            append.Append(rightWords);
            append.Append("]");
            append.Append("}");
            //返回值
            return BP.Tools.Entitis2Json.Instance.ReplaceIllgalChart(append.ToString());
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <returns></returns>
        private string GetDeliveryNodes()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string sql = "SELECT NodeID,Name FROM WF_Node WHERE FK_Flow='" + nd.FK_Flow + "' AND DeliveryWay=" + (int)DeliveryWay.BySelected + " AND DeliveryParas LIKE '%" + nd.NodeID + "%' order by Step";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID);

            //Flow fl = new Flow(nd.FK_Flow);
            //GERpt rpt = fl.HisGERpt;
            //rpt.OID = this.WorkID;
            //rpt.RetrieveFromDBSources();
            StringBuilder append = new StringBuilder();
            append.Append("[");
            foreach (DataRow dr in dt.Rows)
            {
                BP.WF.Node mynd = new BP.WF.Node(int.Parse(dr["NodeID"].ToString()));
                if (mynd.IsStartNode)
                    continue;

                if (gwls.IsExits(GenerWorkerListAttr.FK_Node, mynd.NodeID) == true)
                    continue;

                if (append.Length > 1) append.Append(",");

                append.Append("{NodeID:'" + dr["NodeID"] + "',Name:'" + dr["Name"] + "'}");

                if (mynd.HisCCRole != CCRole.UnCC)
                {
                    /*是可以抄送的*/
                    append.Append(",{NodeID:'" + dr["NodeID"] + "_CC',Name:'阅办'}");    
                }
            }
            append.Append("]");
            return append.ToString();
        }
    }
}