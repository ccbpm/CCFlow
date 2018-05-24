using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Sys;
using BP.Web;
using BP.Web.Controls;
using BP.WF.Port;
using System.Data;

namespace CCFlow.SDKFlowDemo
{
    public partial class AuthorDemo : BP.Web.WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Pub1.Clear();
            if (DataType.IsNullOrEmpty(WebUser.No))
            {
                BindLogin();
            }
            BindExit();
            //当前登录人被谁委托
            Author_CoverEmps();
            //当前登录人委托给谁
            Author_AreadyEmps();
            //设置委托
            Author_Set();
            //取消委托
            Author_Cancel();
            Author_BindData();
        }

        void BindLogin()
        {
            this.Pub1.AddFieldSet("委托接口功能测试");

            Label lbl_Login_No = new Label();
            lbl_Login_No.Text = "输入登录帐号:";
            lbl_Login_No.ID = "lbl_Login_No";
            this.Pub1.Add(lbl_Login_No);

            TB TB_WebUser_No = new TB();
            TB_WebUser_No.ID = "TB_WebUser_No";
            TB_WebUser_No.Width = 100;
            this.Pub1.Add(TB_WebUser_No);

            Btn btn_Loin = new Btn();
            btn_Loin.ID = "btn_Title_Flow_WorkID";
            btn_Loin.Text = "登录";
            btn_Loin.Width = 45;
            btn_Loin.Click += new EventHandler(btn_Loin_Click);
            this.Pub1.Add(btn_Loin);
            this.Pub1.AddFieldSetEnd();
        }

        void BindExit()
        {
            if (DataType.IsNullOrEmpty(WebUser.No))
                return;

            this.Pub1.AddFieldSet("当前登录人信息");
            this.Pub1.Add("当前登录人：" + WebUser.No + "-" + WebUser.Name);
            this.Pub1.AddBR();
            Btn btn_Loin_Exit = new Btn();
            btn_Loin_Exit.ID = "btn_Loin_Exit";
            btn_Loin_Exit.Text = "切换帐号";
            btn_Loin_Exit.Width = 80;
            btn_Loin_Exit.Click += new EventHandler(btn_Loin_Exit_Click);
            this.Pub1.Add(btn_Loin_Exit);

            this.Pub1.AddFieldSetEndBR();
        }

        void btn_Loin_Exit_Click(object sender, EventArgs e)
        {
            WebUser.Exit();
            this.Response.Redirect("AuthorDemo.aspx");
        }

        void btn_Loin_Click(object sender, EventArgs e)
        {
            TB user_no = this.Pub1.GetTBByID("TB_WebUser_No");

            if (user_no == null || user_no.Text == "")
            {
                this.Alert("请输入登录账号。");
                return;
            }
            try
            {
                Emp port_emp = new Emp(user_no.Text);
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
            // 调用登录接口,写入登录信息。
            BP.WF.Dev2Interface.Port_Login(user_no.Text);
            this.Response.Redirect("AuthorDemo.aspx");
        }

        /// <summary>
        /// 当前登录人被谁委托
        /// </summary>
        private void Author_CoverEmps()
        {
            if (DataType.IsNullOrEmpty(WebUser.No))
                return;
            /*如果是授权状态, 获取当前委托人的信息. */
            WFEmps emps = new WFEmps();
            QueryObject obj = new QueryObject(emps);
            obj.AddWhere(WFEmpAttr.Author, WebUser.No);
            obj.DoQuery();

            this.Pub1.AddFieldSet(WebUser.Name + "：被委托信息");
            if (emps == null || emps.Count == 0)
                this.Pub1.Add("无");

            foreach (WFEmp emp in emps)
            {
                this.Pub1.Add("委托人：" + emp.No + "-" + emp.Name + " 委托方式：" + emp.HisAuthorWay);
                if (emp.HisAuthorWay == AuthorWay.SpecFlows)
                    this.Pub1.Add("委托流程编号：" + emp.AuthorFlows);

                this.Pub1.AddBR();
            }
            this.Pub1.AddFieldSetEndBR();
        }
        /// <summary>
        /// 当前登录人委托给谁
        /// </summary>
        private void Author_AreadyEmps()
        {
            if (DataType.IsNullOrEmpty(WebUser.No))
                return;

            this.Pub1.AddFieldSet(WebUser.Name + "：委托信息");
            WFEmp emp = new WFEmp(WebUser.No);
            if (DataType.IsNullOrEmpty(emp.Author))
            {
                this.Pub1.Add("无");
            }

            if (!DataType.IsNullOrEmpty(emp.Author))
            {
                Emp empPort = new Emp(emp.Author);
                this.Pub1.Add("委托人：" + empPort.No + "-" + empPort.Name + " 委托方式：" + emp.HisAuthorWay);
                if (emp.HisAuthorWay == AuthorWay.SpecFlows)
                    this.Pub1.Add("委托流程编号：" + emp.AuthorFlows);
            }

            this.Pub1.AddFieldSetEndBR();
        }

        /// <summary>
        /// 设置委托
        /// </summary>
        private void Author_Set()
        {
            if (DataType.IsNullOrEmpty(WebUser.No))
                return;
            this.Pub1.AddFieldSet("设置委托");

            Label lbl_Author_No = new Label();
            lbl_Author_No.Text = "接受委托人账号:";
            lbl_Author_No.ID = "lbl_Login_No";
            this.Pub1.Add(lbl_Author_No);

            TB TB_Author_No = new TB();
            TB_Author_No.ID = "TB_Author_No";
            TB_Author_No.Width = 100;
            this.Pub1.Add(TB_Author_No);

            DDL ddl_AuthorWay = new DDL();
            ddl_AuthorWay.ID = "ddl_AuthorWay";

            ListItem item = new ListItem();
            item.Value = "0";
            item.Text = "无";
            ddl_AuthorWay.Items.Add(item);

            item = new ListItem();
            item.Value = "1";
            item.Text = "全部流程";
            ddl_AuthorWay.Items.Add(item);

            item = new ListItem();
            item.Value = "2";
            item.Text = "指定流程";
            ddl_AuthorWay.Items.Add(item);
            this.Pub1.Add(ddl_AuthorWay);

            Label lbl_Author_FlowNos = new Label();
            lbl_Author_FlowNos.Text = "流程编号(格式：,001,002):";
            lbl_Author_FlowNos.ID = "lbl_Author_FlowNos";
            this.Pub1.Add(lbl_Author_FlowNos);

            TB TB_Author_FlowNos = new TB();
            TB_Author_FlowNos.ID = "TB_Author_FlowNos";
            TB_Author_FlowNos.Width = 100;
            this.Pub1.Add(TB_Author_FlowNos);

            Btn btn_Author_Set = new Btn();
            btn_Author_Set.ID = "btn_Author_Set";
            btn_Author_Set.Text = "保存";
            btn_Author_Set.Width = 45;
            btn_Author_Set.Click += new EventHandler(btn_Author_Set_Click);
            this.Pub1.Add(btn_Author_Set);

            this.Pub1.AddFieldSetEndBR();
        }

        void btn_Author_Set_Click(object sender, EventArgs e)
        {
            TB tb_Author = this.Pub1.GetTBByID("TB_Author_No");
            DDL ddl_AuthorWay = this.Pub1.GetDDLByID("ddl_AuthorWay");
            TB tb_FlowNos = this.Pub1.GetTBByID("TB_Author_FlowNos");
            if (tb_Author == null || ddl_AuthorWay == null || tb_FlowNos == null)
                return;
            bool bResult = Dev2Interface.Flow_AuthorSave(tb_Author.Text, int.Parse(ddl_AuthorWay.SelectedItem.Value), tb_FlowNos.Text);
            if (bResult == true)
            {
                this.Alert("设置成功。");
                this.Response.Redirect("AuthorDemo.aspx");
            }
        }

        /// <summary>
        /// 取消委托
        /// </summary>
        private void Author_Cancel()
        {
            if (DataType.IsNullOrEmpty(WebUser.No))
                return;

            this.Pub1.AddFieldSet("撤销委托");

            Btn btn_Author_Cancel = new Btn();
            btn_Author_Cancel.ID = "btn_Author_Cancel";
            btn_Author_Cancel.Text = "撤销";
            btn_Author_Cancel.Width = 45;
            btn_Author_Cancel.Click += new EventHandler(btn_Author_Cancel_Click);
            this.Pub1.Add(btn_Author_Cancel);

            this.Pub1.AddFieldSetEndBR();
        }

        void btn_Author_Cancel_Click(object sender, EventArgs e)
        {
            bool bResult = Dev2Interface.Flow_AuthorCancel();
            if (bResult == true)
            {
                this.Alert("撤销成功。");
                this.Response.Redirect("AuthorDemo.aspx");
            }
        }

        /// <summary>
        /// 获取被委托待办数据
        /// </summary>
        private void Author_BindData()
        {
            if (DataType.IsNullOrEmpty(WebUser.No))
                return;

            this.Pub1.AddFieldSet("委托待办数据");
            //当前登录人的委托人
            DataTable dt_Author = Dev2Interface.DB_AuthorEmps();
            if (dt_Author != null && dt_Author.Rows.Count > 0)
            {
                string empNo = dt_Author.Rows[0]["NO"].ToString();
                DataTable dt = Dev2Interface.DB_AuthorEmpWorks(empNo);
                //委托人登录
                Dev2Interface.Port_Login(empNo);
                this.Pub1.Add("<b>当前登录人已切换为：" + empNo + "</b>");
                this.Pub1.AddBR();
                if (dt != null && dt.Rows.Count > 0)
                {
                    StringBuilder sBuilder = new StringBuilder();
                    sBuilder.Append("<table width='100%'  cellspacing='0' cellpadding='0' align=left>");
                    sBuilder.Append("<Caption ><div class='CaptionMsg' >待办</div></Caption>");

                    string extStr = "";
                    string timeKey = DateTime.Now.ToString("yyyyMMddhhff");
                    string GroupBy = this.Request.QueryString["GroupBy"];
                    string appPath = BP.WF.Glo.CCFlowAppPath;//this.Request.ApplicationPath;

                    if (DataType.IsNullOrEmpty(GroupBy))
                    {
                        if (this.DoType == "CC")
                            GroupBy = "Rec";
                        else
                            GroupBy = "FlowName";
                    }

                    sBuilder.Append("<tr class='centerTitle'>");
                    sBuilder.Append("<th >ID</th>");
                    sBuilder.Append("<th class='Title' width=40% nowrap=true >标题</th>");

                    if (GroupBy != "FlowName")
                    {
                        sBuilder.Append("<th >" + "<a href='EmpWorks.aspx?GroupBy=FlowName" + extStr + "&T=" + timeKey + "' >流程</a>" + "</th>");
                    }

                    if (GroupBy != "NodeName")
                    {
                        sBuilder.Append("<th >" + "<a href='EmpWorks.aspx?GroupBy=NodeName" + extStr + "&T=" + timeKey + "' >节点</a>" + "</th>");
                    }

                    if (GroupBy != "StarterName")
                    {
                        sBuilder.Append("<th >" + "<a href='EmpWorks.aspx?GroupBy=StarterName" + extStr + "&T=" + timeKey + "' >发起人</a>" + "</th>");
                    }

                    sBuilder.Append("<th >" + "<a href='EmpWorks.aspx?GroupBy=PRI" + extStr + "&T=" + timeKey + "' >优先级</a>" + "</th>");

                    sBuilder.Append("<th >发起日期</th>");
                    sBuilder.Append("<th >接受日期</th>");
                    sBuilder.Append("<th >应完成日期</th>");
                    sBuilder.Append("<th >状态</th>");
                    sBuilder.Append("<th >备注</th>");
                    sBuilder.Append("</tr>");

                    string groupVals = "";
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        if (groupVals.Contains("@" + dr[GroupBy].ToString() + ","))
                            continue;
                        groupVals += "@" + dr[GroupBy].ToString() + ",";
                    }

                    int i = 0;
                    int colspan = 10;
                    bool is1 = false;
                    DateTime cdt = DateTime.Now;
                    string[] gVals = groupVals.Split('@');
                    int gIdx = 0;
                    foreach (string g in gVals)
                    {
                        if (DataType.IsNullOrEmpty(g))
                            continue;
                        gIdx++;
                        sBuilder.Append("<tr>");
                        if (GroupBy == "Rec")
                        {
                            sBuilder.Append("<td colspan=" + colspan + " class=TRSum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" " + " >" + "<div style='text-align:left; float:left' ><img src='/WF/Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g.Replace(",", "") + "</b>" + "</td>");
                        }
                        else
                        {
                            sBuilder.Append("<td colspan=" + colspan + " class=TRSum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" " + " >" + "<div style='text-align:left; float:left' ><img src='/WF/Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g.Replace(",", "") + "</b>" + "</td>");
                        }

                        sBuilder.Append("</tr>");

                        foreach (System.Data.DataRow dr in dt.Rows)
                        {
                            if (dr[GroupBy].ToString() + "," != g)
                                continue;
                            string sdt = dr["SDT"] as string;
                            string paras = dr["AtPara"] as string;

                            if (is1)
                            {
                                sBuilder.Append("<tr bgcolor=AliceBlue " + "ID='" + gIdx + "_" + i + "'" + " >");
                            }
                            else
                            {
                                sBuilder.Append("<tr bgcolor=white " + "ID='" + gIdx + "_" + i + "'" + " class=TR>");
                            }

                            is1 = !is1;

                            i++;
                            int isRead = int.Parse(dr["IsRead"].ToString());

                            sBuilder.Append("<td class='Idx' nowrap>" + i + "</td>");
                            
                                if (isRead == 0)
                                {
                                    sBuilder.Append("<td onclick=\"SetImg('" + appPath + "','I" + gIdx + "_" + i + "')\"" + " >" + "<a href=\"javascript:WinOpenIt('/WF/MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "&IsRead=0&T=" + timeKey + "&Paras=" + paras + "');\" ><img class=Icon align='middle'  src='/WF/Img/Mail_UnRead.png' id='I" + gIdx + "_" + i + "' />" + dr["Title"].ToString() + "</a>" + "</td>");
                                }
                                else
                                {
                                    sBuilder.Append("<td  nowrap >" + "<a href=\"javascript:WinOpenIt('/WF/MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "&Paras=" + paras + "&T=" + timeKey + "');\"  ><img src='/WF/Img/Mail_Read.png' id='I" + gIdx + "_" + i + "' class=Icon align='middle'  />" + dr["Title"].ToString() + "</a>" + "</td>");
                                }
                            

                            if (GroupBy != "FlowName")
                            {
                                sBuilder.Append("<td  nowrap >" + dr["FlowName"].ToString() + "</td>");
                            }
                            if (GroupBy != "NodeName")
                            {
                                sBuilder.Append("<td  nowrap >" + dr["NodeName"].ToString() + "</td>");
                            }
                            if (GroupBy != "StarterName")
                            {
                                sBuilder.Append("<td  nowrap >" + BP.WF.Glo.GenerUserImgSmallerHtml(dr["Starter"].ToString(), dr["StarterName"].ToString()) + "</td>");
                            }
                            sBuilder.Append("<td  nowrap >" + "<img class=Icon src='/WF/Img/PRI/" + dr["PRI"].ToString() + ".png' class=Icon />" + "</td>");


                            sBuilder.Append("<td  nowrap class='TBDate' >" + BP.DA.DataType.ParseSysDate2DateTimeFriendly(dr["RDT"].ToString()) + "</td>");
                            sBuilder.Append("<td  nowrap class='TBDate' >" + BP.DA.DataType.ParseSysDate2DateTimeFriendly(dr["ADT"].ToString()) + "</td>");
                            sBuilder.Append("<td  nowrap class='TBDate' >" + dr["SDT"].ToString().Substring(5) + "</td>");

                            DateTime mysdt = BP.DA.DataType.ParseSysDate2DateTime(sdt);
                            if (cdt >= mysdt)
                            {
                                if (cdt.ToString("yyyy-MM-dd") == mysdt.ToString("yyyy-MM-dd"))
                                {
                                    sBuilder.Append("<td align=center nowrap >" + "<img src='/WF/Img/TolistSta/0.png' class='Icon'/><font color=green>正常</font>" + "</td>");
                                }
                                else
                                {
                                    sBuilder.Append("<td align=center nowrap >" + "<img src='/WF/Img/TolistSta/2.png' class='Icon'/><font color=red>逾期</font>" + "</td>");
                                }
                            }
                            else
                            {
                                sBuilder.Append("<td align=center nowrap >" + "<img src='/WF/Img/TolistSta/0.png'class='Icon'/>&nbsp;<font color=green>正常</font>" + "</td>");
                            }

                            sBuilder.Append("<td width='200'><div style='width:200px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap;' title='" + dr["FlowNote"] + "'>" + dr["FlowNote"] + "</div></td>");
                            sBuilder.Append("</tr>");
                        }
                    }
                    sBuilder.Append("</table>");

                    this.Pub1.Add(sBuilder.ToString());
                }
                else
                {
                    this.Pub1.Add("无数据");
                }
            }
            else
            {
                this.Pub1.Add("无数据");
            }
            this.Pub1.AddFieldSetEndBR();
        }
    }
}