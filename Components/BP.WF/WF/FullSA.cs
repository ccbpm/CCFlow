using System;
using System.Collections.Generic;
using BP.DA;
using System.Data;
using BP.En;
using BP.WF.Template;
using BP.Web;
using BP.Port;
using BP.Sys;
using static iTextSharp.text.pdf.AcroFields;

namespace BP.WF
{
    /// <summary>
    /// 计算未来处理人
    /// </summary>
    public class FullSA
    {
        #region 方法.
        /// <summary>
        /// 计算两个时间点.
        /// </summary>
        /// <param name="sa"></param>
        /// <param name="nd"></param>
        private void InitDT(SelectAccper sa, Node nd)
        {
            //计算上一个时间的发送点.
            if (this.LastTimeDot == null)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT SDT FROM WF_GenerWorkerlist WHERE WorkID=" + ps.DBStr + "WorkID AND FK_Node=" + ps.DBStr + "FK_Node";
                ps.Add("WorkID", this.HisCurrWorkNode.WorkID);
                ps.Add("FK_Node", nd.NodeID);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);

                foreach (DataRow dr in dt.Rows)
                {
                    this.LastTimeDot = dr[0].ToString();
                    break;
                }
            }

            //上一个节点的发送时间点或者 到期的时间点，就是当前节点的接受任务的时间。
            sa.PlanADT = this.LastTimeDot;

            //计算当前节点的应该完成日期。
            DateTime dtOfShould = Glo.AddDayHoursSpan(this.LastTimeDot, nd.TimeLimit, nd.TimeLimitHH,
                nd.TimeLimitMM, nd.TWay);
            sa.PlanSDT = dtOfShould.ToString(DataType.SysDatatimeFormat);

            //给最后的时间点复制.
            this.LastTimeDot = sa.PlanSDT;
        }
        /// <summary>
        /// 当前节点应该完成的日期.
        /// </summary>
        private string LastTimeDot = null;
        /// <summary>
        /// 工作Node.
        /// </summary>
        public WorkNode HisCurrWorkNode = null;
        #endregion 方法.

        /// <summary>
        /// 自动计算未来处理人（该方法在发送成功后执行.）
        /// </summary>
        public FullSA()
        {
        }
        public Int64 WorkID = 0;
        public GERpt geRpt = null;
        public GenerWorkFlow gwf = null;
        public Work wk = null;

        /// <summary>
        /// 自动计算未来处理人（该方法在发送成功后执行.）
        /// </summary>
        /// <param name="currWorkNode">执行的WorkNode</param>
        public void DoIt2023(WorkNode currWorkNode)
        {
            //如果当前不需要计算未来处理人.
            if (currWorkNode.HisFlow.ItIsFullSA == false
                && currWorkNode.ItIsSkip == false)
                return;

            //如果到达最后一个节点,就不处理了.
            if (currWorkNode.HisNode.ItIsEndNode)
                return;

            //设置变量.
            this.WorkID = currWorkNode.WorkID;
            this.geRpt = currWorkNode.rptGe;
            this.gwf = currWorkNode.HisGenerWorkFlow;
            this.wk = currWorkNode.HisWork;

            //初始化一些变量.
            this.HisCurrWorkNode = currWorkNode;
            Node currND = currWorkNode.HisNode;
            Int64 workid = currWorkNode.HisWork.OID;
            //Node toNode = currWorkNode.NodeSend_GenerNextStepNode(true);
           
            

            //1.调用处理下一个节点的接收人.
            WebUserCopy webUser = new WebUserCopy();
            webUser.LoadWebUser();
            //获得到达的节点.
            Nodes toNodes = currND.HisToNodes;

            //调用到达node.
            foreach(Node toNode in toNodes)
                InitToNode(currWorkNode,toNode, webUser);

            //更新人员.
            DBAccess.RunSQL("UPDATE WF_SelectAccper SET EmpName = (SELECT Name FROM Port_Emp WHERE NO=WF_SelectAccper.FK_Emp ) WHERE WF_SelectAccper.WorkID=" + this.WorkID);

        }

        /// <summary>
        /// 处理到达的节点.
        /// </summary>
        /// <param name="toND"></param>
        public void InitToNode(WorkNode currWN, Node toNd, WebUserCopy webUser)
        {
           
            //工作节点.
            WorkNode town = new WorkNode(this.wk, toNd);
            town.WebUser= webUser; //更改身份.

            town.rptGe = this.geRpt; //设置变量.
            town.HisGenerWorkFlow = this.gwf; 

            //开始找人.
            FindWorker fw = new FindWorker();
            fw.WebUser = webUser; //设置实体.
            fw.currWn = currWN;
            Node toNode = town.HisNode;
            //if ((currWN.HisNode.TodolistModel == TodolistModel.Teamup || currWN.HisNode.TodolistModel == TodolistModel.TeamupGroupLeader)
            //    && (toNode.HisDeliveryWay == DeliveryWay.ByStation || toNode.HisDeliveryWay == DeliveryWay.BySenderParentDeptLeader || toNode.HisDeliveryWay == DeliveryWay.BySenderParentDeptStations))
            //    return Teamup_InitWorkerLists(fw, town);
           DataTable dt = null;
            try
            {
                dt = fw.DoIt(currWN.HisFlow, currWN, town);
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("url@") == false)
                    throw new Exception(ex.Message);
                dt = new DataTable();
            }
           
            if (dt == null)
                throw new Exception(BP.WF.Glo.multilingual("@没有找到接收人.", "WorkNode", "not_found_receiver"));

            //删除可能有的数据.
            DBAccess.RunSQL("DELETE FROM WF_SelectAccper WHERE WorkID=" + this.WorkID + " AND FK_Node =" + toNode.NodeID);

            //把人员保存进去.
            SelectAccper sa = new SelectAccper();
            foreach (DataRow dr in dt.Rows)
            {
                string no = dr[0].ToString();
                string name = "";
                if (dt.Columns.Count==2)
                    name = dr[0].ToString();

                sa = new SelectAccper();
                sa.EmpNo = no;
                sa.EmpName = name;
                sa.NodeID = toNd.NodeID;
                sa.NodeName = toNd.Name;
                sa.NodeIdx = toNd.Step; //步骤.

                sa.WorkID = this.WorkID;
                sa.Info = "无";
                sa.AccType = 0;
                sa.ResetPK();
                if (sa.IsExits)
                    continue;
                //计算接受任务时间与应该完成任务时间.
                InitDT(sa, toNode);
                sa.SetPara("IsFullSA", 0);//IsFullSA是否是计算出的处理人，0表示是，1表示否
                sa.Insert();
            }

          
            //定义变量.
            WebUserCopy myWebUser = new WebUserCopy();
            if (DataType.IsNullOrEmpty(sa.EmpNo) == true)
                sa.EmpNo = webUser.No;
            myWebUser.LoadEmpNo(sa.EmpNo);

            //执行抄送.
            WorkCC cc=new WorkCC(town, myWebUser);
            cc.DoCC("FullSA"); //执行抄送动作.
            if (toNd.ItIsEndNode == false)
            {
                //计算获得到达的节点.
                //Node toNodeTo = town.NodeSend_GenerNextStepNode();
                Nodes toNodes = toNode.HisToNodes;

                //调用到达node.
                foreach (Node toNodeTo in toNodes)
                    InitToNode(town, toNodeTo, myWebUser);
                //递归调用.
                //InitToNode(town, toNodeTo, myWebUser);
            }
        }

    }
}
