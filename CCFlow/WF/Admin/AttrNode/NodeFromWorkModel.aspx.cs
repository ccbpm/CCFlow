using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.DA;
using System.Data;

namespace CCFlow.WF.Admin
{
    public partial class NodeFromWorkModel : BP.Web.WebPage
    {

        #region 参数
        public int NodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
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
            if (this.IsPostBack==false)
            {
                
                //加载请选择一个节点表单
                DDL_Frm1();

                this.RB_Frm_0.Attributes["onclick"] = "SelectImg('0')";
                this.RB_Frm_1.Attributes["onclick"] = "SelectImg('1')";

                this.RB_tree.Attributes["onclick"] = "ChangeImg('0')";
                this.RB_tab.Attributes["onclick"] = "ChangeImg('1')";

                this.RB_CurrentForm.Attributes["onclick"] = "SetDDLEnable('" + this.DDL_Frm.ClientID + "','enable')";
                this.RB_OtherForms.Attributes["onclick"] = "SetDDLEnable('" + this.DDL_Frm.ClientID + "','disable')";


                BP.WF.Node nd = new Node(this.NodeID);

                BtnLab btn = new BtnLab(this.NodeID);

                if (btn.WebOfficeWorkModel == WebOfficeWorkModel.FrmFirst || btn.WebOfficeWorkModel == WebOfficeWorkModel.WordFirst)
                    nd.FormType = NodeFormType.WebOffice;

                switch (nd.FormType)
                {
                    //加载使用ccbpm内置的节点表单傻瓜表单
                    case NodeFormType.FixForm:
                        this.RB_FixFrm.Checked = true;
                        this.RB_Frm_1.Checked=true; //.SelectedValue ="1";

                        if (nd.NodeFrmID == "ND" + this.NodeID)
                        {
                            this.RB_CurrentForm.Checked = true;
                        }
                        else 
                        {
                            this.RB_OtherForms.Checked = true;
                            
                        }
                        this.DDL_Frm.SelectedValue = nd.NodeFrmID.Substring(2);
                        break;
                    //加载使用ccbpm内置的节点表单自由表单
                    case NodeFormType.FreeForm:
                        this.RB_FixFrm.Checked = true;
                        this.RB_Frm_0.Checked=true; //.SelectedValue ="1";
                        if (nd.NodeFrmID == "ND"+this.NodeID)
                        {
                            this.RB_CurrentForm.Checked = true;
                        }
                        else 
                        {
                            this.RB_OtherForms.Checked = true;

                        }
                        this.DDL_Frm.SelectedValue = nd.NodeFrmID.Substring(2);
                        break;
                    //加载使用嵌入式表单
                    case NodeFormType.SelfForm:
                        this.RB_SelfForm.Checked = true;
                        this.TB_CustomURL.Text = nd.FormUrl;
                        break;
                    //加载使用SDK表单
                    case NodeFormType.SDKForm:
                        this.RB_SDKForm.Checked = true;
                        this.TB_FormURL.Text = nd.FormUrl;
                        break;
                    //加载表单树
                    case NodeFormType.SheetTree:
                        this.RB_SheetTree.Checked = true;
                        this.RB_tree.Checked = true;
                        break;
                    case NodeFormType.WebOffice: //公文表单.
                        this.RB_WebOffice.Checked = true;
                        this.RB_WebOffice_FrmFirst.Checked = true; //默认为表单在前.

                        BtnLabExtWebOffice mybtn = new BtnLabExtWebOffice(this.NodeID);

                        if (btn.WebOfficeWorkModel == WebOfficeWorkModel.FrmFirst)
                            this.RB_WebOffice_FrmFirst.Checked = true;
                        else
                            this.RB_WebOffice_WordFirst.Checked = true;

                        this.RB_WebOffice_FreeFrm.Checked = true; //默认为自由表单工作模式.
                        if (mybtn.WebOfficeFrmModel == BP.Sys.FrmType.FreeFrm)
                            this.RB_WebOffice_FreeFrm.Checked = true;
                        else
                            this.RB_WebOffice_FoolForm.Checked = true;
                       
                      //  this.RB_tree.Checked = true;
                        break;
                    //加载禁用(对多表单流程有效)
                    case NodeFormType.DisableIt:
                        this.RB_SheetTree.Checked = true;
                        this.RB_tab.Checked=true;
                        break;
                }
            }
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            Save();
        }

        protected void Btn_Cancel_Click(object sender, EventArgs e)
        {
            Save();
            BP.Sys.PubClass.WinClose();
        }
        protected void Save()
        {
            Node nd = new Node(this.NodeID);
            //使用ccbpm内置的节点表单
            if (this.RB_FixFrm.Checked)
            {
                if (this.RB_Frm_0.Checked == true)
                {
                    nd.FormType = NodeFormType.FreeForm;
                    nd.DirectUpdate();
                }
                else
                {
                    nd.FormType = NodeFormType.FixForm;
                    nd.DirectUpdate();
                }
                if (this.RB_CurrentForm.Checked)
                {
                    nd.NodeFrmID = "";
                    nd.DirectUpdate();
                }
                if (this.RB_OtherForms.Checked)
                {
                    nd.NodeFrmID = "ND" + this.DDL_Frm.SelectedValue;
                    nd.DirectUpdate();
                }
            }
            //使用嵌入式表单
            if (this.RB_SelfForm.Checked)
            {
                nd.FormType = NodeFormType.SelfForm;
                nd.FormUrl = this.TB_CustomURL.Text;
                nd.DirectUpdate();
            }
            //使用SDK表单
            if (this.RB_SDKForm.Checked)
            {
                nd.FormType = NodeFormType.SDKForm;
                nd.FormUrl = this.TB_FormURL.Text;
                nd.DirectUpdate();
            }
            //绑定多表单
            if (this.RB_SheetTree.Checked)
            {
                if (this.RB_tree.Checked == true)
                {
                    nd.FormType = NodeFormType.SheetTree;
                    nd.DirectUpdate();
                }
                else
                {
                    nd.FormType = NodeFormType.DisableIt;
                    nd.DirectUpdate();
                }
            }

            //如果公文表单选择了
            if (this.RB_WebOffice.Checked)
            {
                nd.FormType = NodeFormType.WebOffice;
                nd.Update();


                //按钮标签.
                BtnLabExtWebOffice btn = new BtnLabExtWebOffice(this.NodeID);

                // tab 页工作风格.
                if (this.RB_WebOffice_FrmFirst.Checked)
                    btn.WebOfficeWorkModel = WebOfficeWorkModel.FrmFirst;
                else
                    btn.WebOfficeWorkModel = WebOfficeWorkModel.WordFirst;

                //表单工作模式.
                if (this.RB_WebOffice_FreeFrm.Checked)
                    btn.WebOfficeFrmModel = BP.Sys.FrmType.FreeFrm; 
                else
                    btn.WebOfficeFrmModel = BP.Sys.FrmType.FoolForm; 
                
                btn.Update();
            }

        }
        protected void DDL_Frm1()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            this.DDL_Frm.Items.Clear();
            BP.WF.Nodes nds = new BP.WF.Nodes(nd.HisFlow.No);
            foreach (BP.WF.Node item in nds)
            {
                this.DDL_Frm.Items.Add(new ListItem(item.NodeID + " " + item.Name, item.NodeID.ToString()));
            }
        }
    }
}