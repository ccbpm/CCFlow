using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.DA;

namespace CCFlow.WF.Admin.FindWorker
{
    public partial class Leader : BP.Web.WebPage
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
               string val= this.Request.QueryString["FK_Flow"];
               if (string.IsNullOrEmpty(val))
                   return "001";
               return val;
            }
        }
        public string FK_Node
        {
            get
            {
                string val = this.Request.QueryString["FK_Node"];
                if (string.IsNullOrEmpty(val))
                    return "101";
                return val;
            }
        }
        public string S1
        {
            get
            {
                string val= this.Request.QueryString["S1"];
                if (string.IsNullOrEmpty(val))
                    return null;
                return val;
            }
        }
        public string S2
        {
            get
            {
                string val = this.Request.QueryString["S2"];
                if (string.IsNullOrEmpty(val))
                    return null;
                return val;
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            FindWorkerRole en = new FindWorkerRole();
            en.OID = this.RefOID;
            if (en.OID != 0)
                en.Retrieve();

            if (this.RefOID != 0 && this.S1 == null)
            {
                if (en.SortVal1 != "0")
                {
                    this.Response.Redirect("Leader.aspx?S1=" + en.SortVal1 + "&RefOID=" + this.RefOID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node, true);
                    return;
                }
            }

            #region 1级
            this.UCS1.AddFieldSet("人员范围其它参数");
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_S1"; // 第一纬度.
            ddl.AutoPostBack = true;
            ddl.Items.Add(new ListItem("当前提交人", "0"));
            ddl.Items.Add(new ListItem("指定节点的提交人", "1"));
            ddl.Items.Add(new ListItem("按表单字段指定的提交人", "2"));
            if (this.S1 != null)
                ddl.SetSelectItem(this.S1);
            else
                ddl.SetSelectItem(en.SortVal1);

            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
            this.UCS1.Add("选择人员");
            this.UCS1.Add(ddl);
            #endregion 2级

            #region 2级
            if (this.S1 == "1" )
            {
                this.UCS2.AddFieldSet("您需要指定一个节点.");
                this.UCS2.Add("选择节点");
                ddl = new BP.Web.Controls.DDL();
                ddl.ID = "DDL_V1"; // 第一个纬度的参数.
                ddl.BindSQL("SELECT NodeID AS No, Name FROM WF_Node WHERE FK_Flow='" + this.FK_Flow + "' ORDER BY NODEID ",
                    "No", "Name", "20");
                this.UCS2.Add(ddl);
                ddl.SetSelectItem(en.TagVal1); // 第一纬度的参数.
                this.UCS2.AddFieldSetEnd();
            }

            if (this.S1 == "2")
            {
                this.UCS2.AddFieldSet("选择表单字段");
                ddl = new BP.Web.Controls.DDL();
                ddl.ID = "DDL_V1";  //第一个纬度的参数
                ddl.BindSQL("SELECT KeyOfEn as No, KeyOfEn+' - '+Name as Name FROM Sys_MapAttr WHERE FK_MapData='ND" + int.Parse(this.FK_Flow) + "Rpt' AND MyDataType=1 ",
                    "No", "Name", "20");

                this.UCS2.Add("选择一个字段");
                this.UCS2.Add(ddl);
                ddl.SetSelectItem(en.TagVal1); // 第一纬度的参数.
                this.UCS2.AddFieldSetEnd();
            }
            #endregion 2级

            #region 绑定后半部分.
            this.UCS3.AddFieldSet("人员范围其它参数");
            // 其他的配置信息.
            BP.Web.Controls.RadioBtn rb = new BP.Web.Controls.RadioBtn();
            rb.GroupName = "s";
            rb.ID = "RB_0";
            rb.Text = "直接主管";
            if (en.SortVal2 == "0")
                rb.Checked = true;

            this.UCS3.Add(rb);
            this.UCS3.AddHR();

            //特定职务级别的主管
            rb = new BP.Web.Controls.RadioBtn();
            rb.GroupName = "s";
            rb.ID = "RB_1";
            rb.Text = "特定职务级别的主管";
            if (en.SortVal2 == "1")
                rb.Checked = true;

            this.UCS3.Add(rb);
            ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_DutyLevel";
            ddl.BindSQL("SELECT distinct DutyLevel AS No, DutyLevel as Name FROM Port_DeptEmp WHERE DutyLevel IS NOT NULL",
                "No", "Name", "20");
            this.UCS3.Add(ddl);
            ddl.SetSelectItem(en.TagVal2); 
            this.UCS3.AddHR();

            // 特定职务的领导
            rb = new BP.Web.Controls.RadioBtn();
            rb.GroupName = "s";
            rb.ID = "RB_2";
            rb.Text = "特定职务的领导";

            if (en.SortVal2 == "2")
                rb.Checked = true;

            this.UCS3.Add(rb);
            ddl = new BP.Web.Controls.DDL();
            ddl.BindSQL("SELECT No, Name FROM Port_Duty ", "No", "Name", "20");
            ddl.SetSelectItem(en.TagVal2);
            ddl.ID = "DDL_Duty";

            this.UCS3.Add(ddl);
            this.UCS3.AddHR();

            // 特定岗位
            rb = new BP.Web.Controls.RadioBtn();
            rb.GroupName = "s";
            rb.ID = "RB_3";
            rb.Text = "特定岗位";
            this.UCS3.Add(rb);
            if (en.SortVal2 == "3")
                rb.Checked = true;
            ddl = new BP.Web.Controls.DDL();
            ddl.BindSQL("SELECT No, Name FROM Port_Station ", "No", "Name", "20");
            ddl.SetSelectItem(en.TagVal2);
            ddl.ID = "DDL_Station";
            this.UCS3.Add(ddl);
            this.UCS3.AddFieldSetEnd();
            #endregion 绑定后半部分.

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "保存";
            btn.Click += new EventHandler(btn_Click);
            this.UCS3.Add(btn);
        }

        void btn_Click(object sender, EventArgs e)
        {
            FindWorkerRole fwr = new FindWorkerRole();
            fwr.CheckPhysicsTable();
            fwr.OID = this.RefOID;
            if (fwr.OID != 0)
                fwr.Retrieve();

            #region 处理描述.
            // 一级
            fwr.SortVal0 = this.PageID;
            fwr.SortText0 = "上级领导";

            // 二级.
            fwr.SortVal1 = this.UCS1.GetDDLByID("DDL_S1").SelectedItemStringVal;
            fwr.SortText1 = this.UCS1.GetDDLByID("DDL_S1").SelectedItem.Text;

            // 三级.
            if (this.UCS3.GetRadioBtnByID("RB_0").Checked)
            {
                fwr.SortVal2 = "0";
                fwr.SortText2 = this.UCS3.GetRadioBtnByID("RB_0").Text;
            }

            if (this.UCS3.GetRadioBtnByID("RB_1").Checked)
            {
                fwr.SortVal2 = "1";
                fwr.SortText2 = this.UCS3.GetRadioBtnByID("RB_1").Text;
            }

            if (this.UCS3.GetRadioBtnByID("RB_2").Checked)
            {
                fwr.SortVal2 = "2";
                fwr.SortText2 = this.UCS3.GetRadioBtnByID("RB_2").Text;
            }

            if (this.UCS3.GetRadioBtnByID("RB_3").Checked)
            {
                fwr.SortVal2 = "3";
                fwr.SortText2 = this.UCS3.GetRadioBtnByID("RB_3").Text;
            }

            if (fwr.SortVal2 == "")
            {
                fwr.SortVal2 = "0";
                fwr.SortText2 = this.UCS3.GetRadioBtnByID("RB_0").Text;
            }
            #endregion 处理描述.

            #region 处理数据
            try
            {
                // 获取1纬度的参数.
                fwr.TagVal1 = this.UCS2.GetDDLByID("DDL_V1").SelectedItemStringVal;
                fwr.TagText1 = this.UCS2.GetDDLByID("DDL_V1").SelectedItem.Text;
            }
            catch
            {
                fwr.TagVal1 = ""; // this.UCS2.GetDDLByID("DDL_V1").SelectedItemStringVal;
                fwr.TagText1 = ""; // this.UCS2.GetDDLByID("DDL_V1").SelectedItem.Text;
            }

            // 处理第2纬度的参数.
            switch (fwr.HisFindLeaderModel)
            {
                case FindLeaderModel.SpecDutyLevelLeader:
                    fwr.TagVal2 = this.UCS3.GetDDLByID("DDL_DutyLevel").SelectedItemStringVal;
                    fwr.TagText2 = this.UCS3.GetDDLByID("DDL_DutyLevel").SelectedItem.Text;
                    break;
                case FindLeaderModel.DutyLeader:
                    fwr.TagVal2 = this.UCS3.GetDDLByID("DDL_Duty").SelectedItemStringVal;
                    fwr.TagText2 = this.UCS3.GetDDLByID("DDL_Duty").SelectedItem.Text;
                    break;
                case FindLeaderModel.SpecStation:
                    fwr.TagVal2 = this.UCS3.GetDDLByID("DDL_Station").SelectedItemStringVal;
                    fwr.TagText2 = this.UCS3.GetDDLByID("DDL_Station").SelectedItem.Text;
                    break;
                default:
                    fwr.TagVal2 = "";
                    fwr.TagText2 = "";
                    break;
            }
            #endregion 处理数据

            fwr.FK_Node = int.Parse(this.FK_Node);
            fwr.Save();

            //设置成bpm的状态.
            Node nd = new Node(this.FK_Node);
            nd.HisDeliveryWay = DeliveryWay.ByCCFlowBPM;
            nd.DirectUpdate();

            this.Response.Redirect("List.aspx?"+this.FK_Flow+"&FK_Node="+this.FK_Node,true);
            //this.WinCloseWithMsg("保存成功...");
        }

        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s1 = this.UCS1.GetDDLByID("DDL_S1").SelectedItemStringVal;
            this.Response.Redirect("Leader.aspx?S1=" + s1 + "&RefOID=" + this.RefOID+"&FK_Flow="+this.FK_Flow+"&FK_Node="+this.FK_Node, true);
        }
    }
}