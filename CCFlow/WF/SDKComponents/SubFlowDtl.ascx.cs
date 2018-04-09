using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Web;
using BP.DA;
using BP.En;
using BP.WF.Template;

namespace CCFlow.WF.SDKComponents
{
    public partial class SubFlowDtl : BP.Web.UC.UCBase3
    {
        #region 属性.
        public int FID
        {
            get
            {
                return int.Parse(this.Request.QueryString["FID"]);
            }
        }
        public int WorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["WorkID"]);
                }
                catch
                {
                    return int.Parse(this.Request.QueryString["OID"]);
                }
            }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
        public int FK_Node
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return DBAccess.RunSQLReturnValInt("SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);
                }
            }
        }
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
        /// 浏览类型，是查看还是处理
        /// </summary>
        public string DoType
        {
            get
            {
                return DataType.IsNullOrEmpty(this.Request.QueryString["DoType"]) ? "" : this.Request.QueryString["DoType"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            //查询出来所有子流程的数据.
            FrmSubFlow sf = new FrmSubFlow(this.FK_Node);

            Node nd=new Node(this.FK_Node);

            this.AddTable("width='100%'");

            //if (sf.SFCaption.Length !=0)
            //    this.AddCaption(sf.SFCaption); //标题可以为空

            if (sf.SFDefInfo.Trim().Length == 0)
                return;

            //this.AddTR();
            //this.AddTDB("style='min-width:150px;'", "标题");
            //this.AddTDB("style='width:150px;'", "停留节点");
            //this.AddTDB("style='width:80px;'", "状态");
            //this.AddTDB("style='min-width:150px;'", "处理人");
            //this.AddTDB("style='width:150px;'", "处理时间");
            //this.AddTDB("style='min-width:150px;'", "信息");
            //this.AddTREnd();

            this.AddTR();
            this.AddTDTitleExt("发起人");
            this.AddTDTitleExt("标题");
            this.AddTDTitleExt("停留节点");
            this.AddTDTitleExt("状态");
            this.AddTDTitleExt("处理人");
            this.AddTDTitleExt("处理时间");
            this.AddTDTitleExt("信息");
            this.AddTREnd();


            /*有要启动的子流程, 生成启动子流程的连接.*/
            string html = "";
            string emps = null;
            string[] strs = sf.SFDefInfo.Split(',');
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;

                if (str.Length != 3)
                    continue;

                //输出标题.
                BP.WF.Flow fl = new Flow(str);

                if (sf.SFSta == FrmSubFlowSta.Enable && this.DoType!="View")
                    html = "<div style='float:left'><img src='../Img/Max.gif' />&nbsp;" + fl.Name + "</div> <div style='float:right'>[<a href=\"javascript:OpenIt('../MyFlow.htm?FK_Flow=" + fl.No + "&PWorkID=" + this.WorkID + "&PNodeID=" + sf.NodeID + "&PFlowNo=" + nd.FK_Flow + "&PFID=" + this.FID + "')\"  >" + sf.SFCaption + "</a>]</style>";

                if (sf.SFSta == FrmSubFlowSta.Readonly || this.DoType == "View")
                    html = "<div style='float:left'><img src='../Img/Max.gif' />&nbsp;" + fl.Name + "</div></style>";

                this.AddTR();
                this.AddTD(" class=TRSum colspan=7", html);
                this.AddTREnd();

                //该流程的子流程信息.
                GenerWorkFlows gwfs = new GenerWorkFlows();
                if (sf.SFShowCtrl == SFShowCtrl.All)
                {
                    gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID, GenerWorkFlowAttr.FK_Flow, str); //流程.
                }
                else
                {
                    gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID,
                        GenerWorkFlowAttr.FK_Flow, str, GenerWorkFlowAttr.Starter, BP.Web.WebUser.No); //流程.
                }

                foreach (GenerWorkFlow item in gwfs)
                {
                    if (item.WFState == WFState.Blank)
                        continue;

                    this.AddTR();
                    this.AddTD(item.StarterName); //发起人.
                    if (item.TodoEmps.Contains("" + WebUser.No + "," + WebUser.Name + ";") == true)
                    {
                        this.AddTD("style='word-break:break-all;' title='" + item.Title + "'",
                            "<a href=\"javascript:OpenIt('../MyFlow.htm?WorkID=" + item.WorkID + "&FK_Flow=" + item.FK_Flow + "&IsCheckGuide=1&Frms="+item.Paras_Frms+"&FK_Node="+item.FK_Node+"&PNodeID="+item.PNodeID+"&PWorkID="+item.PWorkID+"')\" ><img src='../Img/Dot.png' width='9px' />&nbsp;" + item.Title + "</a>");
                    }
                    else
                    {
                        if (sf.SFOpenType == 0)
                        {
                            this.AddTD("style='word-break:break-all;' title='" + item.Title + "'",
                                "<a href=\"javascript:OpenIt('../WFRpt.htm?WorkID=" + item.WorkID + "&FK_Flow=" + item.FK_Flow + "&PWorkID=" + item.PWorkID + "&PFlowNo=" + item.PFlowNo + "&PNodeID=" + item.PNodeID + "')\" ><img src='../Img/Dot.png' width='9px' />&nbsp;" + item.Title + "</a>");
                        }
                        else
                        {
                            this.AddTD("style='word-break:break-all;' title='" + item.Title + "'",
        "<a href=\"javascript:OpenIt('../WorkOpt/FoolFrmTrack.htm?WorkID=" + item.WorkID + "&FK_Flow=" + item.FK_Flow + "')\" ><img src='../Img/Dot.png' width='9px' />&nbsp;" + item.Title + "</a>");
                        }
                    }

                    this.AddTD(item.NodeName); //到达节点名称.
                    this.AddTD(item.WFStateText); //流程的状态.

                    //if (item.WFState == WFState.Complete)
                    //    this.AddTD("已完成");
                    //else
                    //    this.AddTD("未完成");
                    emps = BP.WF.Glo.DealUserInfoShowModel(item.TodoEmps);

                    this.AddTD("title='"+emps+"'",emps); //到达人员.
                    this.AddTD(BP.DA.DataType.ParseSysDate2DateTimeFriendly(item.RDT)); //日期.
                    this.AddTD("title='"+item.FlowNote+"'",item.FlowNote); //流程备注.
                    this.AddTREnd();

                    //加载他下面的子流程.
                    InsertSubFlows(item.FK_Flow, item.FK_Node, item.WorkID, 1);
                }
            }
            this.AddTableEnd();
        }

        public void InsertSubFlows(string flowNo, int fid, Int64 workid, int layer)
        {
            //该流程的子流程信息, 并按照流程排序.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, workid, GenerWorkFlowAttr.FK_Flow); //流程.
            if (gwfs.Count == 0)
                return;

            string myFlowNo = "";
            foreach (GenerWorkFlow item in gwfs)
            {
                if (item.WFState == WFState.Blank)
                    continue;

                if (myFlowNo.Contains(item.FK_Flow)==false)
                {
                    myFlowNo = myFlowNo + "," + item.FK_Flow;

                    //输出流程.
                    BP.WF.Flow fl = new Flow(item.FK_Flow);
                    string html = "<div style='float:left'>" + DataType.GenerSpace(layer * 2) + "<img src='../Img/Max.gif' />&nbsp;" + fl.Name + "</div>";
                    this.AddTR();
                    this.AddTD(" class=TRSum colspan=6", html);
                    this.AddTREnd();
                }

                this.AddTR();
                this.AddTD("style='word-break:break-all;' title='"+item.Title+"' ", DataType.GenerSpace(layer * 2) + "<a href=\"javascript:OpenIt('../WFRpt.htm?WorkID=" + item.WorkID + "&FK_Flow=" + item.FK_Flow + "')\" ><img src='../Img/Dot.png' width='9px' />&nbsp;" + item.Title + "</a>");
                this.AddTD(item.NodeName); //到达节点名称.
                if (item.WFState == WFState.Complete)
                    this.AddTD("已完成");
                else
                    this.AddTD("未完成");

                this.AddTD("title='"+item.TodoEmps+"'",item.TodoEmps); //到达人员.
                this.AddTD(BP.DA.DataType.ParseSysDate2DateTimeFriendly(item.RDT)); //日期.
                this.AddTD("title='"+item.FlowNote+"'",item.FlowNote); //流程备注.
                this.AddTREnd();

                //加载他下面的子流程.
                InsertSubFlows(item.FK_Flow, item.FK_Node, item.WorkID, layer + 1);
            }
        }

    }
}