using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Port;
using BP.Web;
using BP.En;
using BP.WF.Template;
using BP.DA;

namespace CCFlow.Plug_in.CCFlow.WF.WorkOpt
{
    public partial class UIToNodes : BP.Web.WebPage
    {
        #region 转到节点.
        public string ToNodes
        {
            get
            {
                return this.Request.QueryString["ToNodes"];
            }
        }
        public string CFlowNo
        {
            get
            {
                return this.Request.QueryString["CFlowNo"];
            }
        }
        public string WorkIDs
        {
            get
            {
                return this.Request.QueryString["WorkIDs"];
            }
        }
        public string DoFunc
        {
            get
            {
                return this.Request.QueryString["DoFunc"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }

        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public Int64 FID
        {
            get
            {
                if (this.Request.QueryString["FID"] != null)
                    return Int64.Parse(this.Request.QueryString["FID"]);
                return 0;
            }
        }
        #endregion 转到节点

        protected void Page_Load(object sender, EventArgs e)
        {
            //获得当前节点到达的节点.
            Nodes nds = new Nodes();
            if (this.ToNodes != null)
            {
                /*解决跳转问题.*/
                string[] mytoNodes = this.ToNodes.Split(',');
                foreach (string str in mytoNodes)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;
                    nds.AddEntity(new Node(int.Parse(str)));
                }
            }
            else
            {
                nds = BP.WF.Dev2Interface.WorkOpt_GetToNodes(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
            }

            //获得上次默认选择的节点. 2015-01-15.
            int lastSelectNodeID = BP.WF.Dev2Interface.WorkOpt_ToNodes_GetLasterSelectNodeID(this.FK_Flow, this.FK_Node);
            if (lastSelectNodeID == 0 && nds.Count != 0)
                lastSelectNodeID = int.Parse(nds[0].PKVal.ToString());

            //检查是否有异表单。
            bool isSubYBD = false; //异表单?
            foreach (Node mynd in nds)
            {
                BP.Web.Controls.RadioBtn rb = new BP.Web.Controls.RadioBtn();
                if (mynd.NodeID == 0)
                {
                    rb = new BP.Web.Controls.RadioBtn();
                    rb.GroupName = "s";
                    rb.Text = "<b>可以分发启动的异表单节点</b>";
                    rb.ID = "RB_SameSheet";
                    rb.Attributes["onclick"] = "RBSameSheet(this);";
                    if (this.IsPostBack == false && lastSelectNodeID == 0)
                        rb.Checked = true;

                    // 增加选择项. add  2015-01-15.
                    if (this.IsPostBack == false && mynd.NodeID == lastSelectNodeID)
                        rb.Checked = true;

                    this.Pub1.Add(rb);
                    this.Pub1.AddBR();
                    isSubYBD = true;
                    continue;
                }
                //已有人员直接显示到人员选择器a标签上 秦15.2.5
                string sql = "SELECT A.No,a.Name FROM Port_Emp A, WF_SelectAccper B WHERE A.No=B.FK_Emp AND B.FK_Node=" + mynd.NodeID + " AND B.WorkID=" + this.WorkID;
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                string addSpan = "";
                if (dt.Rows.Count != 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i == 4)
                        {
                            addSpan += dt.Rows[i]["Name"].ToString() + "...";
                            break;
                        }
                        else
                        {
                            if (i == dt.Rows.Count - 1)
                            {
                                addSpan += dt.Rows[i]["Name"].ToString();
                            }
                            else
                            {
                                addSpan += dt.Rows[i]["Name"].ToString() + ",";
                            }
                        }
                    }
                    addSpan = "<span style='color:black;'>(" + addSpan + ")</span>";
                }
                if (isSubYBD == true)
                {
                    /*如果是异表单.*/
                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + mynd.NodeID;
                    cb.Text = mynd.Name;
                    this.Pub1.Add("&nbsp;&nbsp;&nbsp;&nbsp;");
                    this.Pub1.Add(cb);


                    if (this.IsPostBack == false && mynd.NodeID == lastSelectNodeID)
                        cb.Checked = true;

                    if (mynd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        /*由上一步发送人员选择.*/

                        this.Pub1.Add(" - <a id=\"acc_link_" + mynd.NodeID + "\" href=\"javascript:WinShowModalDialog_Accepter('Accepter.htm?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&ToNode=" + mynd.NodeID + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&type=1&DoType=Accepter_Save')\" >选择接受人员" + addSpan + "</a>");
                    }
                    this.Pub1.AddBR();
                    continue;
                }
                else
                {
                    rb = new BP.Web.Controls.RadioBtn();
                    rb.GroupName = "s";
                    rb.Text = mynd.Name;
                    rb.ID = "RB_" + mynd.NodeID;
                    rb.Attributes["onclick"] = "SetUnEable(this);";
                    this.Pub1.Add(rb);
                    if (this.IsPostBack == false && mynd.NodeID == lastSelectNodeID)
                        rb.Checked = true;

                    if (mynd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        /*由上一步发送人员选择.*/
                        this.Pub1.Add(" - <a id=\"acc_link_" + mynd.NodeID + "\" href=\"javascript:WinShowModalDialog_Accepter('Accepter.htm?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&ToNode=" + mynd.NodeID + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&type=1&DoType=Accepter_Save')\" >选择接受人员" + addSpan + "</a>");
                    }
                    this.Pub1.AddBR();
                }
            }

            this.Pub1.AddHR();
            Button btn = new Button();
            btn.ID = "To";
            BP.WF.Template.BtnLab btnlab = new BtnLab(this.FK_Node);
            btn.Text = "  " + btnlab.SendLab + "  ";
            this.Pub1.Add(btn);
            btn.Click += new EventHandler(btn_Click);

            btn = new Button();
            btn.ID = "Btn_Cancel";
            btn.Text = "取消/返回";
            this.Pub1.Add(btn);
            btn.Click += new EventHandler(btn_Click);
        }

        void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.ID == "Btn_Cancel")
            {
                string url = "../MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID;
                this.Response.Redirect(url, true);
                return;
            }

            #region 计算出来到达的节点.


            //获得当前节点到达的节点.
            Nodes nds = new Nodes();
            if (this.ToNodes != null)
            {
                /*解决跳转问题.*/
                string[] mytoNodes = this.ToNodes.Split(',');
                foreach (string str in mytoNodes)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;
                    nds.AddEntity(new Node(int.Parse(str)));
                }
            }
            else
            {
                nds = BP.WF.Dev2Interface.WorkOpt_GetToNodes(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
            }


            //  首先到非异表单去找.
            string toNodes = "";
            foreach (Node mynd in nds)
            {
                if (mynd.HisRunModel == RunModel.SubThread
                    && mynd.HisSubThreadType == SubThreadType.UnSameSheet)
                    continue; //如果是子线程节点.

                if (mynd.NodeID == 0)
                    continue;

                BP.Web.Controls.RadioBtn rb = this.Pub1.GetRadioBtnByID("RB_" + mynd.NodeID);
                if (rb.Checked == false)
                    continue;

                toNodes = mynd.NodeID.ToString();
                break;
            }

            if (toNodes == "")
            {
                // 如果在非异表单没有找到，就到异表单集合去找。 检查是否具有异表单的子线程.
                bool isHave = false;
                foreach (Node mynd in nds)
                {
                    if (mynd.NodeID == 0)
                        isHave = true;
                }

                if (isHave)
                {
                    /*增加异表单的子线程*/
                    foreach (Node mynd in nds)
                    {
                        if (mynd.HisSubThreadType != SubThreadType.UnSameSheet)
                            continue;

                        CheckBox cb = this.Pub1.GetCBByID("CB_" + mynd.NodeID);
                        if (cb == null)
                            continue;

                        if (cb.Checked == true)
                            toNodes += "," + mynd.NodeID;
                    }
                }
            }
            #endregion 计算出来选择的到达节点.

            if (toNodes == "")
            {
                this.Pub1.AddFieldSetRed("发送出现错误", "您没有选择到达的节点。");
                return;
            }

            // 执行发送.
            string msg = "";
            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();

            try
            {
                string toNodeStr = int.Parse(FK_Flow) + "01";
                //如果为开始节点
                if (toNodeStr == toNodes)
                {
                    //把参数更新到数据库里面.
                    GenerWorkFlow gwf = new GenerWorkFlow();
                    gwf.WorkID = this.WorkID;
                    gwf.RetrieveFromDBSources();
                    gwf.Paras_ToNodes = toNodes;
                    gwf.Save();

                    WorkNode firstwn = new WorkNode(wk, nd);

                    Node toNode = new Node(toNodeStr);
                    msg = firstwn.NodeSend(toNode, gwf.Starter).ToMsgOfHtml();
                }
                else
                {
                    msg = BP.WF.Dev2Interface.WorkOpt_SendToNodes(this.FK_Flow,
                        this.FK_Node, this.WorkID, this.FID, toNodes).ToMsgOfHtml();
                }
            }
            catch (Exception ex)
            {
                this.Pub1.AddFieldSetRed("发送出现错误", ex.Message);
                return;
            }

            #region 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
            try
            {
                //处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
                Glo.DealBuinessAfterSendWork(this.FK_Flow, this.WorkID, this.DoFunc, WorkIDs);
            }
            catch (Exception ex)
            {
                this.ToMsg(msg, ex.Message);
                return;
            }
            #endregion 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.

            GenerWorkFlow gwfw = new GenerWorkFlow();
            gwfw.WorkID = this.WorkID;
            gwfw.RetrieveFromDBSources();
            if (nd.IsRememberMe == true)
                gwfw.Paras_ToNodes = toNodes;
            else
                gwfw.Paras_ToNodes = "";
            gwfw.Save();

            /*处理转向问题.*/
            switch (nd.HisTurnToDeal)
            {
                case TurnToDeal.SpecUrl:
                    string myurl = nd.TurnToDealDoc.Clone().ToString();
                    if (myurl.Contains("?") == false)
                        myurl += "?1=1";
                    myurl = BP.WF.Glo.DealExp(myurl, wk, null);
                    myurl += "&FromFlow=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&PWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                    this.Response.Redirect(myurl, true);
                    return;
                case TurnToDeal.TurnToByCond:
                    TurnTos tts = new TurnTos(this.FK_Flow);
                    if (tts.Count == 0)
                        throw new Exception("@您没有设置节点完成后的转向条件。");
                    foreach (TurnTo tt in tts)
                    {
                        tt.HisWork = wk;
                        if (tt.IsPassed == true)
                        {
                            string url = tt.TurnToURL.Clone().ToString();
                            if (url.Contains("?") == false)
                                url += "?1=1";
                            url = BP.WF.Glo.DealExp(url, wk, null);
                            url += "&PFlowNo=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&PWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                            this.Response.Redirect(url, true);
                            return;
                        }
                    }
#warning 为上海修改了如果找不到路径就让它按系统的信息提示。
                    this.ToMsg(msg, "info");
                    //throw new Exception("您定义的转向条件不成立，没有出口。");
                    break;
                default:
                    this.ToMsg(msg, "info");
                    break;
            }
            return;
        }

        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Application["info" + WebUser.No] = msg;

            Glo.SessionMsg = msg;
            this.Response.Redirect("./../MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
        }
    }
}