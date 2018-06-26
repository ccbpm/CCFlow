using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF
{
    public partial class WF_AllotTask : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// WorkID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        /// <summary>
        /// FID
        /// </summary>
        public int FID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// IsFHL
        /// </summary>
        public bool IsFHL
        {
            get
            {
                if (this.WorkID == this.FID)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// NodeID
        /// </summary>
        public int NodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["NodeID"]);
                }
                catch
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
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
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "工作分配";
            if (this.IsPostBack == false)
            {
                string fk_emp = this.Request.QueryString["FK_Emp"];
                string sid = this.Request.QueryString["SID"];

                if (fk_emp != null )
                {
                    if (BP.Web.WebUser.CheckSID(fk_emp, sid) == false)
                        return;

                    Emp emp = new Emp(fk_emp);
                    BP.Web.WebUser.SignInOfGener(emp);
                }
            }

            // 当前用的员工权限。
            this.Pub1.Clear();

            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.NodeID, true);

            if (WebUser.IsWap)
                this.Pub1.AddFieldSet("<a href='./WAP/Home.aspx' ><img src='/WF/Img/Home.gif' border=0/>主页</a> - 工作分配");
            else
                this.Pub1.AddFieldSet("工作分配");

            string ids = "";
            this.Pub1.AddUL();
            foreach (GenerWorkerList wl in wls)
            {
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + wl.FK_Emp;
                ids += "," + cb.ID;

                cb.Text = BP.WF.Glo.DealUserInfoShowModel(wl.FK_Emp, wl.FK_EmpText);
                cb.Checked = wl.IsEnable;
                this.Pub1.Add("<li>");
                this.Pub1.Add(cb);
                this.Pub1.Add("</li>");
            }
            this.Pub1.AddULEnd();

            this.Pub1.AddHR();
            Btn btn = new Btn();
            btn.ID = "Btn_Do";
            btn.Text = "  确定  ";
            btn.Click += new EventHandler(BPToolBar1_ButtonClick);
            this.Pub1.Add(btn);

            CheckBox cbx = new CheckBox();
            cbx.ID = "seleall";
            cbx.Text = "选择全部";
            cbx.Checked = true;
            cbx.Attributes["onclick"] = "SetSelected(this,'" + ids + "')";
            this.Pub1.Add(cbx);
            //this.Pub1.Add("<input type=button value='取消' onclick='window.close();'  />");
            this.Pub1.Add("<br><br>帮助:系统会记住本次的工作指定，下次您在发送时间它会自动把工作投递给您本次指定的人。");
            this.Pub1.AddFieldSetEnd();

        }
        private void BPToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            this.Confirm();
            return;
        }
        /// <summary>
        /// 确定窗口
        /// </summary>
        public void Confirm()
        {
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.NodeID, true);
            try
            {
                #region 检查选择的人员是否为 0 .
                bool isHave0 = true;
                foreach (GenerWorkerList wl in wls)
                {
                    CheckBox cb = this.Pub1.GetCBByID("CB_" + wl.FK_Emp);
                    if (cb.Checked)
                    {
                        isHave0 = false;
                        break;
                    }
                }

                if (isHave0 == true)
                {
                    this.Alert("当前工作中你没有分配给任何人，此工作将不能被其他人所执行！");
                    return;
                }
                #endregion 检查选择的人员是否为 0 .

                #region 执行分配工作 - 更新选择的状态.
                foreach (GenerWorkerList wl in wls)
                {
                    CheckBox cb = this.Pub1.GetCBByID("CB_" + wl.FK_Emp);
                    if (wl.IsEnable != cb.Checked)
                    {
                        wl.IsEnable = cb.Checked;
                        wl.Update();
                    }
                }
                #endregion 执行分配工作 - 更新选择的状态.

                // 保存记忆功能，让下次发送可以记忆住他。
                RememberMe rm = new RememberMe();
                rm.FK_Emp = BP.Web.WebUser.No;
                rm.FK_Node = NodeID;
                rm.Objs = "@";
                rm.ObjsExt = "";

                foreach (GenerWorkerList wl in wls)
                {
                    if (wl.IsEnable == false)
                        continue;

                    rm.Objs += wl.FK_Emp + "@";
                    rm.ObjsExt += wl.FK_EmpText + "&nbsp;&nbsp;";
                }

                rm.Emps = "@";
                rm.EmpsExt = "";

                foreach (GenerWorkerList wl in wls)
                {
                    rm.Emps += wl.FK_Emp + "@";

                    string empInfo = BP.WF.Glo.DealUserInfoShowModel(wl.FK_Emp, wl.FK_EmpText);
                    if (rm.Objs.IndexOf(wl.FK_Emp) != -1)
                        rm.EmpsExt += "<font color=green>" + BP.WF.Glo.DealUserInfoShowModel(wl.FK_Emp, wl.FK_EmpText) + "</font>&nbsp;&nbsp;";
                    else
                        rm.EmpsExt += "<strike><font color=red>(" + BP.WF.Glo.DealUserInfoShowModel(wl.FK_Emp, wl.FK_EmpText) + "</font></strike>&nbsp;&nbsp;";
                }
                rm.Save();

                if (WebUser.IsWap)
                {
                    this.Pub1.Clear();
                    this.Pub1.AddFieldSet("提示信息");
                    this.Pub1.Add("<br>&nbsp;&nbsp;任务分配成功，特别提示：当下一次流程发送时系统会按照您设置的路径进行智能投递。");
                    this.Pub1.AddUL();
                    this.Pub1.AddLi("<a href='./WAP/Home.aspx' ><img src='/WF/Img/Home.gif' border=0/>主页</a>");
                    this.Pub1.AddLi("<a href='./WAP/Start.aspx' ><img src='/WF/Img/Start.gif' border=0/>发起</a>");
                    this.Pub1.AddLi("<a href='./WAP/Runing.aspx' ><img src='/WF/Img/Runing.gif' border=0/>待办</a>");
                    this.Pub1.AddULEnd();
                    this.Pub1.AddFieldSetEnd();
                }
                else
                {
                    this.ToMsg("任务分配成功!!!", "Info");
                    //this.WinCloseWithMsg("任务分配成功。");
                }
            }
            catch (Exception ex)
            {
                this.Response.Write(ex.Message);
                Log.DebugWriteWarning(ex.Message);
                this.Alert("任务分配出错：" + ex.Message);
            }
        }
        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Application["info" + WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            this.Response.Redirect("../MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.NodeID + "&WorkID=" + this.WorkID, false);
        }

        public void DealWithFHLFlow(ArrayList al, GenerWorkerLists wlSeles)
        {
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Retrieve(GenerWorkerListAttr.FID, this.FID);

            DBAccess.RunSQL("UPDATE  WF_GenerWorkerlist SET IsEnable=0  WHERE FID=" + this.FID);

            string emps = "";
            string myemp = "";
            foreach (Object obj in al)
            {
                emps += obj.ToString() + ",";
                myemp = obj.ToString();
                DBAccess.RunSQL("UPDATE  WF_GenerWorkerlist SET IsEnable=1  WHERE FID=" + this.FID + " AND FK_Emp='" + obj + "'");
            }

            //BP.WF.Node nd = new BP.WF.Node(NodeID);
            //Work wk = nd.HisWork;
            //wk.OID = this.WorkID;
            //wk.Retrieve();
            //wk.Emps = emps;
            //wk.Update();
        }

        public void DealWithPanelFlow(ArrayList al, GenerWorkerLists wlSeles)
        {
            // 删除当前非配的工作。
            // 已经非配或者自动分配的任务。
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            int NodeID = gwf.FK_Node;
            Int64 workId = this.WorkID;
            //GenerWorkerLists wls = new GenerWorkerLists(this.WorkID,NodeID);
            DBAccess.RunSQL("UPDATE  WF_GenerWorkerlist SET IsEnable=0  WHERE WorkID=" + this.WorkID + " AND FK_Node=" + NodeID);
            //  string vals = "";
            string emps = "";
            string myemp = "";
            foreach (Object obj in al)
            {
                emps += obj.ToString() + ",";
                myemp = obj.ToString();
                DBAccess.RunSQL("UPDATE  WF_GenerWorkerlist SET IsEnable=1  WHERE WorkID=" + this.WorkID + " AND FK_Node=" + NodeID + " AND fk_emp='" + obj + "'");
            }

            BP.WF.Node nd = new BP.WF.Node(NodeID);
            Work wk = nd.HisWork;

            wk.OID = this.WorkID;
            wk.Retrieve();

            wk.Emps = emps;
            wk.Update();
        }
    }
}