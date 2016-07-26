using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.Web;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF.Template;

namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_ExpImp : BP.Web.WebPage
    {
        #region 属性.
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
        public string FromMap
        {
            get
            {
                return this.Request.QueryString["FromMap"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "ccfrom:导入导出";
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = this.FK_MapData;
            md.RetrieveFromDBSources();
            switch (this.DoType)
            {
                case "Exp":
                    DataSet ds = md.GenerHisDataSet();
                    string file = this.Request.PhysicalApplicationPath + "\\DataUser\\Temp\\" + this.FK_MapData + ".xml";
                    ds.WriteXml(file);
                    BP.Sys.PubClass.DownloadFile(file, md.Name + ".xml");
                    this.WinClose();
                    break;
                case "Imp":
                    MapData mdForm = new MapData(this.FromMap);
                    MapData.ImpMapData(this.FK_MapData, mdForm.GenerHisDataSet(), true);
                    this.WinClose();
                    return;
                case "Share":
                    this.Share();
                    break;
                default:
                    this.BindHome();
                    break;
            }
        }
        public void Share()
        {
            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeftTX("在施工中..");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("采集");
            this.Pub1.AddTDTitle("说明");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("表单类别");
            MapData md = new MapData(this.FK_MapData);
            TextBox tb = new TextBox();
            tb.ID = "TB_Sort";
            if (string.IsNullOrEmpty(md.FK_FrmSort.Trim()))
            {
                /*没有类别，就考虑是节点表单*/
            }
            else
            {
                SysFormTree fs = new SysFormTree();
                fs.No = md.No;
                fs.RetrieveFromDBSources();
                tb.Text = md.No;
            }
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("表单名称");
            tb = new TextBox();
            tb.ID = "TB_Name";
            tb.Text = md.Name;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            //this.Pub1.AddTR();
            //this.Pub1.AddTD("设计者");
            //tb = new TextBox();
            //tb.ID = "TB_Designer";
            //tb.Text = md.Designer;
            //this.Pub1.AddTD(tb);

            //this.Pub1.AddTD();
            //this.Pub1.AddTREnd();

            //this.Pub1.AddTR();
            //this.Pub1.AddTD("设计单位");
            //tb = new TextBox();
            //tb.ID = "TB_DesignerContact";
            //tb.Text = md.DesignerContact;
            //this.Pub1.AddTD(tb);
            //this.Pub1.AddTD();
            //this.Pub1.AddTREnd();

            //this.Pub1.AddTR();
            //this.Pub1.AddTD("联系方式");
            //tb = new TextBox();
            //tb.ID = "TB_DesignerContact";
            //tb.Text = md.DesignerContact;
            //tb.Columns = 50;
            //this.Pub1.AddTD(tb);
            //this.Pub1.AddTD();
            //this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD();
            Button btn = new Button();
            btn.CssClass = "Btn";
            btn.ID = "Btn_Save";
            btn.Text = "Share It";
            btn.Click += new EventHandler(btn_ShareIt_Click);
            this.Pub1.AddTD(btn);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

            this.Pub1.AddFieldSet("共享说明");
            this.Pub1.Add("如果您想批量的共享表单，请转到流程设计器-功能执行-导出流程与表单模板，发送邮件到template@ccflow.org中。");
            this.Pub1.AddFieldSetEnd();
        }
        void btn_ShareIt_Click(object sender, EventArgs e)
        {
        }
        public void BindHome()
        {
            this.Pub1.AddFieldSet("导出表单模板");
            this.Pub1.AddUL();
            this.Pub1.AddLi("<a href='ExpImp.aspx?DoType=Exp&FK_MapData=" + this.FK_MapData + "' target=_blank >导出表单模板并下载</a>");
            this.Pub1.AddLi("<a href='../Comm/Method.aspx?M=BP.WF.GenerTemplate' >导出全部的流程模板与表单模板到服务器上。</a>");
            // this.Pub1.AddLi("<a href='ExpImp.aspx?DoType=Share&FK_MapData=" + this.FK_MapData + "'>共享此表单给互联网其它的朋友。</a>");
            this.Pub1.AddLi("<a href=\"javascript:alert('此功能在施工中，敬请期待。\t\n您可以把此模板文件导出后发送到:template@ccflow.org.');\" >共享此表单给互联网其它的朋友。</a>");
            this.Pub1.AddULEnd();
            this.Pub1.AddFieldSetEnd();

            //this.Pub1.AddFieldSet("从互联网上导入");
            //this.Pub1.Add("ccflow流程模板与表单模板交流地址。<a href=\"javascript:alert('此功能在施工中，敬请期待。');\" >http://template.ccflow.org/</a>。");

            TextBox tb = new TextBox();
            //tb.Text = "";
            //tb.ID = "TB_Net";
            //tb.Columns = 50;
            //this.Pub1.AddBR(tb);

            Button btn = new Button();
            //btn.Text = "导入";
            //btn.ID = "Btn_Net";
            //btn.CssClass = "Btn";
            //btn.Click += new EventHandler(btn_Imp_Click);
            //this.Pub1.Add(btn);
            //this.Pub1.AddFieldSetEnd();

            this.Pub1.AddFieldSet("从本机上导入");
            this.Pub1.Add("特别说明:执行导入系统将会清除当前表单信息。表单模板(*.xml)");
            HtmlInputFile fu = new HtmlInputFile();
            fu.ID = "F";
            this.Pub1.Add(fu);
            btn = new Button();
            btn.Text = "导入";
            btn.ID = "Btn_Local";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_Imp_Click);
            this.Pub1.Add(btn);
            this.Pub1.AddFieldSetEnd();

            if (string.IsNullOrEmpty(this.FK_Flow) == false)
            {
                this.Pub1.AddFieldSet("从本流程节点上导入");
                DataTable dt = DBAccess.RunSQLReturnTable("SELECT NodeID,Step,Name FROM WF_Node WHERE FK_Flow='" + this.FK_Flow + "'");
                this.Pub1.AddUL();
                foreach (DataRow dr in dt.Rows)
                {
                    string str = dr["NodeID"].ToString();
                    if (this.FK_MapData.Contains(str) == true)
                        continue;

                    this.Pub1.AddLi("ExpImp.aspx?DoType=Imp&FK_Flow=" + this.FK_Flow + "&FK_MapData=" + this.FK_MapData + "&FromMap=ND" + dr["NodeID"], "节点ID:" + dr["NodeID"] + ",步骤:" + dr["Step"] + "," + dr["Name"].ToString());
                    //  window.location.href = 'ExpImp.aspx?DoType=Imp&FK_Flow=" + fk_flow + "&FK_MapData=" +FK_MapData + "&FromMap=' + fk_Frm;
                    //     this.Pub1.AddLi("<a href=\"javascript:LoadFrm('" + this.FK_Flow + "','" + this.FK_MapData + "','ND" + dr["NodeID"] + "');\" >" + dr["Name"].ToString() + "</a>");
                    //  this.Pub1.AddLi("<a href=\"javascript:LoadFrm('" + this.FK_Flow + "','" + this.FK_MapData + "','ND" + dr["NodeID"] + "');\" >" + dr["Name"].ToString() + "</a>");
                }
                this.Pub1.AddULEnd();
                this.Pub1.AddFieldSetEnd();
            }

            // 检查是否有流程编号.
            //this.Pub1.AddFieldSet("从本流程节点表单导入");
            //this.Pub1.Add("特别说明:执行导入系统将会清除当前表单信息。");
            //this.Pub1.AddBR("表单模板(*.xml)");
            //HtmlInputFile fu = new HtmlInputFile();
            //fu.ID = "F";
            //this.Pub1.Add(fu);
            //Button btn = new Button();
            //btn.Text = "执行导入";
            //btn.Click += new EventHandler(btn_Click);
            //this.Pub1.Add(btn);
            //this.Pub1.AddFieldSetEnd();
        }
        void btn_Imp_Click(object sender, EventArgs e)
        {
            try
            {
                string path = BP.Sys.SystemConfig.PathOfTemp;  
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                string file =  path+ this.FK_MapData + ".xml";

                Button btn = sender as Button;
                if (btn.ID == "Btn_Local")
                {
                    HtmlInputFile myfu = this.Pub1.FindControl("F") as HtmlInputFile;
                    myfu.PostedFile.SaveAs(file);
                }

                if (btn.ID == "Btn_Net")
                {
                    string url = this.Pub1.GetTextBoxByID("TB_Net").Text;
                    if (string.IsNullOrEmpty(url))
                    {
                        this.Alert("请输入url.");
                        return;
                    }
                    string context = BP.DA.DataType.ReadURLContext(url, 9999, System.Text.Encoding.UTF32);
                    if (context.Contains("Sys_MapAttr") == false)
                        throw new Exception("读取的文件错误可能是非法的url.\t\n" + url);
                    BP.DA.DataType.SaveAsFile(file, context);
                }

                try
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(file);
                    BP.Sys.MapData.ImpMapData(this.FK_MapData, ds, true);
                    this.WinClose();
                }
                catch (Exception ex)
                {
                    throw new Exception("@导入错误:" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }
    }
}