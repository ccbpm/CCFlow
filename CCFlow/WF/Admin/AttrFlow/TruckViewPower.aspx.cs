using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class UITruckViewPower : System.Web.UI.Page
    {
        #region 属性.
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
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(FK_Flow))
                {
                    throw new Exception("流程编号为空");
                }
                else
                {

                    BP.WF.Template.TruckViewPower en = new BP.WF.Template.TruckViewPower(FK_Flow);
                    if (en.PStarter==true)
                    {
                      
                        this.CB_FQR.Checked=true;
                    }
                    if (en.PWorker==true)
                    {
                        this.CB_CYR.Checked = true;
                    }
                    if (en.PCCer==true)
                    {
                        this.CB_CSR.Checked = true;
                    }

                    if (en.PMyDept==true)
                    {
                        this.CB_BBM.Checked = true;
                    }
                    if (en.PPMyDept==true)
                    {
                        this.CB_ZSSJ.Checked = true;
                    }
                    if (en.PPDept==true)
                    {
                        this.CB_SJ.Checked = true; 
                    }

                    if (en.PSameDept==true)
                    {
                        this.CB_PJ.Checked = true;
                    }
                    if ( en.PSpecDept==true)
                    {
                        
                        this.QY_ZDBM.Checked = true;
                    }
                    if (string.IsNullOrEmpty(en.PSpecDeptExt))
                    {
                        this.TB_ZDBM.Text = en.PSpecDeptExt;
                    }
                    if (en.PSpecSta == true)
                    {
                     
                        this.QY_ZDGW.Checked = true;
                    }
                    if (string.IsNullOrEmpty(en.PSpecStaExt))
                    {
                        this.TB_ZDGW.Text = en.PSpecStaExt;
                    }
                    if (en.PSpecGroup == true)
                    {
                        
                        this.QY_ZDQXZ.Checked = true;

                    }
                    if (string.IsNullOrEmpty(en.PSpecGroupExt))
                    {
                        this.TB_ZDQXZ.Text = en.PSpecGroupExt;
                    }
                    if (en.PSpecEmp == true)
                    {
                       
                        this.QY_ZDRY.Checked = true;
                    }

                    if (string.IsNullOrEmpty(en.PSpecEmpExt))
                    {
                        this.TB_ZDRY.Text = en.PSpecEmpExt;
                    }
                }
            }

        }



        protected void Btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            BP.WF.Template.TruckViewPower en = new BP.WF.Template.TruckViewPower(FK_Flow);

            if (string.IsNullOrEmpty(FK_Flow))
            {
                throw new Exception("@流程编号为空");
            }

            else
            {
                if (this.CB_FQR.Checked)
                {
                    en.PStarter = true;

                }
                else
                {
                    en.PStarter = false;
                }
                 if (this.CB_CYR.Checked)
                {
                    en.PWorker = true;
                }
                 else
                 {
                     en.PWorker = false;
                 }
                 if (this.CB_CSR.Checked)
                {
                    en.PCCer = true;
                }
                 else
                 {
                     en.PCCer = false;
                 }

                 if (this.CB_BBM.Checked)
                {
                    en.PMyDept = true;
                }
                 else
                 {
                     en.PMyDept = false; 
                 }
                if (this.CB_ZSSJ.Checked)
                {
                    en.PPMyDept = true;
                }
                else
                {
                    en.PPMyDept = false;
                }

                if (this.CB_SJ.Checked)
                {
                    en.PPDept = true;
                }
                else
                {
                    en.PPDept = false;
                }
               if (this.CB_PJ.Checked)
                {
                    en.PSameDept = true;
                }
               else
               {
                   en.PSameDept = false;
               }
               if (this.QY_ZDBM.Checked)
               {
                   en.PSpecDept = true;
               }
               else
               {
                   en.PSpecDept = false;
               }
               if (string.IsNullOrEmpty(this.TB_ZDBM.Text))
               {
                   en.PSpecDeptExt = this.TB_ZDBM.Text;
               }
               else
               {
                   en.PSpecDeptExt = "";
               }

               if (this.QY_ZDGW.Checked)
               {
                   en.PSpecSta = true;
               }
               else
               {
                   en.PSpecSta = false;
               }
               if (string.IsNullOrEmpty(this.TB_ZDGW.Text))
               {
                   en.PSpecStaExt = this.TB_ZDGW.Text;
               }
               else
               {
                   en.PSpecStaExt = "";
               }
               if (this.QY_ZDQXZ.Checked)
               {
                   en.PSpecGroup = true;

               }
               else
               {
                   en.PSpecGroup = false;
               }
               if (string.IsNullOrEmpty(this.TB_ZDQXZ.Text))
               {
                   en.PSpecGroupExt = this.TB_ZDQXZ.Text;
               }
               else
               {
                   en.PSpecGroupExt = "";
               }
               if (this.QY_ZDRY.Checked)
               {
                   en.PSpecEmp = true;
               }
               else
               {
                   en.PSpecEmp = false;
               }

               if (string.IsNullOrEmpty(this.TB_ZDRY.Text))
               {
                   en.PSpecEmpExt = this.TB_ZDRY.Text;
               }
               else
               {
                   en.PSpecEmpExt = "";
               }
                en.Update();

            }

            //this.Save();
            BP.Sys.PubClass.WinClose();

        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {

            BP.WF.Template.TruckViewPower en = new BP.WF.Template.TruckViewPower(FK_Flow);

            if (string.IsNullOrEmpty(FK_Flow))
            {
                throw new Exception("@流程编号为空");
            }

            else
            {
                if (this.CB_FQR.Checked)
                {
                    en.PStarter = true;

                }
                else
                {
                    en.PStarter = false;
                }
                if (this.CB_CYR.Checked)
                {
                    en.PWorker = true;
                }
                else
                {
                    en.PWorker = false;
                }
                if (this.CB_CSR.Checked)
                {
                    en.PCCer = true;
                }
                else
                {
                    en.PCCer = false;
                }

                if (this.CB_BBM.Checked)
                {
                    en.PMyDept = true;
                }
                else
                {
                    en.PMyDept = false;
                }
                if (this.CB_ZSSJ.Checked)
                {
                    en.PPMyDept = true;
                }
                else
                {
                    en.PPMyDept = false;
                }

                if (this.CB_SJ.Checked)
                {
                    en.PPDept = true;
                }
                else
                {
                    en.PPDept = false;
                }
                if (this.CB_PJ.Checked)
                {
                    en.PSameDept = true;
                }
                else
                {
                    en.PSameDept = false;
                }
                if (this.QY_ZDBM.Checked)
                {
                    en.PSpecDept = true;
                }
                else
                {
                    en.PSpecDept = false;
                }
                if (string.IsNullOrEmpty(this.TB_ZDBM.Text))
                {
                    en.PSpecDeptExt = this.TB_ZDBM.Text;
                }
                else
                {
                    en.PSpecDeptExt = "";
                }

                if (this.QY_ZDGW.Checked)
                {
                    en.PSpecSta = true;
                }
                else
                {
                    en.PSpecSta = false;
                }
                if (string.IsNullOrEmpty(this.TB_ZDGW.Text))
                {
                    en.PSpecStaExt = this.TB_ZDGW.Text;
                }
                else
                {
                    en.PSpecStaExt = "";
                }
                if (this.QY_ZDQXZ.Checked)
                {
                    en.PSpecGroup = true;

                }
                else
                {
                    en.PSpecGroup = false;
                }
                if (string.IsNullOrEmpty(this.TB_ZDQXZ.Text))
                {
                    en.PSpecGroupExt = this.TB_ZDQXZ.Text;
                }
                else
                {
                    en.PSpecGroupExt = "";
                }
                if (this.QY_ZDRY.Checked)
                {
                    en.PSpecEmp = true;
                }
                else
                {
                    en.PSpecEmp = false;
                }

                if (string.IsNullOrEmpty(this.TB_ZDRY.Text))
                {
                    en.PSpecEmpExt = this.TB_ZDRY.Text;
                }
                else
                {
                    en.PSpecEmpExt = "";
                }
                en.Update();

            }

           
            //BP.Sys.PubClass.WinClose();

            this.Save();
        }

        protected void Btn_Close_Click(object sender, EventArgs e)
        {
            BP.Sys.PubClass.WinClose();
        }

        public void Save()
        {

        }

    }
}