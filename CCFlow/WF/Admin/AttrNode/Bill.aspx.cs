using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.En;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;
namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_BillSet : BP.Web.WebPage
    {
        public int NodeID
        {
            get
            {
                return int.Parse(this.Request.QueryString["NodeID"]);
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
        /// 获取文件路径
        /// </summary>
        public string FileFullPath(string fileName, BillTemplate bt)
        {
            string fileType = "";
            if (bt.HisBillFileType == BillFileType.RuiLang)
            {
                fileType = ".grf";
            }
            else
            {
                fileType = ".rtf";
            }

            string filePath = BP.Sys.SystemConfig.PathOfCyclostyleFile + "\\" + bt.No + fileType;
            BP.WF.Node curNode = new BP.WF.Node(this.NodeID);
            //表单树时对存放路径进行修改
            if (curNode.FormType == NodeFormType.SheetTree)
            {
                bt.Url = "FlowFrm\\" + this.FK_Flow + "\\" + this.NodeID + "\\" + fileName.Replace(fileType, "");
                filePath = BP.Sys.SystemConfig.PathOfCyclostyleFile + "\\FlowFrm\\" + this.FK_Flow + "\\" + this.NodeID;
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                filePath = BP.Sys.SystemConfig.PathOfCyclostyleFile + "\\FlowFrm\\" + this.FK_Flow + "\\" + this.NodeID + "\\" + fileName;
            }
            return filePath;
        }
        public void DoNew(BillTemplate bill)
        {
            this.Ucsys1.Clear();
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            this.Ucsys1.AddTable();
            this.Ucsys1.AddCaptionLeft("<a href='Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID + "' >" + "返回" + "</a> - <a href=Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID + "&DoType=New ><img  border=0 src='../Img/Btn/New.gif' />新建</a>");
            this.Ucsys1.AddTR();
            this.Ucsys1.AddTDTitle("项目");
            this.Ucsys1.AddTDTitle("输入");
            this.Ucsys1.AddTDTitle("备注");
            this.Ucsys1.AddTREnd();

            this.Ucsys1.AddTR();
            this.Ucsys1.AddTD("单据类型"); // 单据/单据名称
            DDL ddl = new DDL();
            ddl.ID = "DDL_BillType";

            BillTypes ens = new BillTypes();
            ens.RetrieveAllFromDBSource();

            if (ens.Count == 0)
            {
                BillType enB = new BillType();
                enB.Name = "新建类型" + "1";
                enB.FK_Flow = this.FK_Flow;
                enB.No = "01";
                enB.Insert();
                ens.AddEntity(enB);
            }

            ddl.BindEntities(ens);
            ddl.SetSelectItem(bill.FK_BillType);
            this.Ucsys1.AddTD(ddl);
            this.Ucsys1.AddTD("<a href='Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID + "&DoType=EditType'><img src='../Img/Btn/Edit.gif' border=0/>类别维护</a>");
            this.Ucsys1.AddTREnd();

            this.Ucsys1.AddTR();
            this.Ucsys1.AddTD("编号");
            TB tb = new TB();
            tb.ID = "TB_No";
            tb.Text = bill.No;
            tb.Enabled = false;
            if (tb.Text == "")
                tb.Text = "系统自动生成";

            this.Ucsys1.AddTD(tb);
            this.Ucsys1.AddTD("");
            this.Ucsys1.AddTREnd();

            this.Ucsys1.AddTR();
            this.Ucsys1.AddTD("名称"); // 单据/单据名称
            tb = new TB();
            tb.ID = "TB_Name";
            tb.Text = bill.Name;
            tb.Columns = 40;
            this.Ucsys1.AddTD("colspan=2", tb);
            this.Ucsys1.AddTREnd();

            this.Ucsys1.AddTR();
            this.Ucsys1.AddTD("生成的文件类型"); // 单据/单据名称
            ddl = new DDL();
            ddl.ID = "DDL_BillFileType";
            ddl.BindSysEnum("BillFileType");
            ddl.SetSelectItem((int)bill.HisBillFileType);
            this.Ucsys1.AddTD(ddl);
            this.Ucsys1.AddTD("目前不支持excel,html格式.");
            this.Ucsys1.AddTREnd();

            this.Ucsys1.AddTR();
            this.Ucsys1.AddTD("单据模板");
            HtmlInputFile file = new HtmlInputFile();
            file.ID = "f";
            file.Attributes["width"] = "100%";
            this.Ucsys1.AddTD("colspan=2", file);
            this.Ucsys1.AddTREnd();

            this.Ucsys1.AddTRSum();
            this.Ucsys1.Add("<TD class=TD colspan=3 align=center>");
            Button btn = new Button();
            btn.CssClass = "Btn";
            btn.ID = "Btn_Save";
            btn.Text = "保存";
            this.Ucsys1.Add(btn);
            btn.Click += new EventHandler(btn_Click);
            this.Ucsys1.Add(btn);
            if (bill.No.Length > 1)
            {


                btn = new Button();
                btn.ID = "Btn_Del";
                btn.CssClass = "Btn";
                btn.Text = "删除"; // "删除单据";
                this.Ucsys1.Add(btn);
                btn.Attributes["onclick"] += " return confirm('您确认吗？');";
                btn.Click += new EventHandler(btn_Del_Click);
            }
            string url = "";
            string fileType = "";
            if (bill.HisBillFileType == BillFileType.RuiLang)
            {
                fileType = "grf";
            }
            else
            {
                fileType = "rtf";
            }

            if (this.RefNo != null)
                url = "<a href='../../DataUser/CyclostyleFile/" + bill.Url + "." + fileType + "'><img src='../Img/Btn/save.gif' border=0/> 模板下载</a>";

            this.Ucsys1.Add(url + "</TD>");
            this.Ucsys1.AddTREnd();

            this.Ucsys1.AddTable();
        }
        void btn_Gener_Click(object sender, EventArgs e)
        {
            string url = "Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID + "&DoType=Edit&RefNo=" + this.RefNo;
            this.Response.Redirect(url, true);
        }
        void btn_Click(object sender, EventArgs e)
        {
            HtmlInputFile file = this.Ucsys1.FindControl("f") as HtmlInputFile;
            BillTemplate bt = new BillTemplate();
            bt.NodeID = this.NodeID;
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            if (this.RefNo != null)
            {
                bt.No = this.RefNo;
                bt.Retrieve();
                bt = this.Ucsys1.Copy(bt) as BillTemplate;
                bt.NodeID = this.NodeID;
                bt.FK_BillType = this.Ucsys1.GetDDLByID("DDL_BillType").SelectedItemStringVal;
                if (file.Value == null || file.Value.Trim() == "")
                {
                    bt.Update();
                    this.Alert("保存成功");
                    return;
                }

                if (bt.HisBillFileType == BillFileType.RuiLang)
                {
                    if (file.Value.ToLower().Contains(".grf") == false)
                    {
                        this.Alert("@错误，非法的 grf 格式文件。");
                        return;
                    }
                }
                else
                {
                    if (file.Value.ToLower().Contains(".rtf") == false)
                    {
                        this.Alert("@错误，非法的 rtf 格式文件。");
                        return;
                    }
                }
                string temp = "";
                string tempName = "";
                if (bt.HisBillFileType == BillFileType.RuiLang)
                {
                    tempName = "Temp.grf";
                    temp = BP.Sys.SystemConfig.PathOfCyclostyleFile + "\\Temp.grf";
                    file.PostedFile.SaveAs(temp);
                }
                else
                {
                    tempName = "Temp.rtf";
                    temp = BP.Sys.SystemConfig.PathOfCyclostyleFile + "\\Temp.rtf";
                    file.PostedFile.SaveAs(temp);
                }



                //检查文件是否正确。
                try
                {
                    string[] paras = BP.DA.Cash.GetBillParas_Gener(tempName, nd.HisFlow.HisGERpt.EnMap.Attrs);
                }
                catch (Exception ex)
                {
                    this.Ucsys2.AddMsgOfWarning("错误信息", ex.Message);
                    return;
                }
                string fullFile = FileFullPath(file.PostedFile.FileName, bt);//BP.Sys.SystemConfig.PathOfCyclostyleFile + "\\" + bt.No + ".rtf";
                System.IO.File.Copy(temp, fullFile, true);
                bt.Update();
                return;
            }
            bt = this.Ucsys1.Copy(bt) as BillTemplate;

            if (file.Value != null)
            {
                if (bt.HisBillFileType == BillFileType.RuiLang)
                {
                    if (file.Value.ToLower().Contains(".grf") == false)
                    {
                        this.Alert("@错误，非法的 grf 格式文件。");
                        // this.Alert("@错误，非法的 rtf 格式文件。");
                        return;
                    }
                }
                else
                {
                    if (file.Value.ToLower().Contains(".rtf") == false)
                    {
                        this.Alert("@错误，非法的 rtf 格式文件。");
                        // this.Alert("@错误，非法的 rtf 格式文件。");
                        return;
                    }
                }

            }
            else
            {
                this.Alert("请上传文件。");
                // this.Alert("@错误，非法的 rtf 格式文件。");
                return;

            }

            /* 如果包含这二个字段。*/
            string fileName = file.PostedFile.FileName;
            fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            if (bt.Name == "")
            {

                bt.Name = fileName.Replace(".rtf", "");
                bt.Name = fileName.Replace(".grf", "");
            }

            try
            {
                bt.No = BP.Tools.chs2py.convert(bt.Name);
                if (bt.IsExits)
                    bt.No = bt.No + "." + BP.DA.DBAccess.GenerOID().ToString();
            }
            catch
            {
                bt.No = BP.DA.DBAccess.GenerOID().ToString();
            }
            string tmp = "";
            string tmpName = "";
            if (bt.HisBillFileType == BillFileType.RuiLang)
            {
                tmpName = "Temp.grf";
                tmp = BP.Sys.SystemConfig.PathOfCyclostyleFile + "\\Temp.grf";
                file.PostedFile.SaveAs(tmp);
            }
            else
            {
                tmpName = "Temp.rtf";
                tmp = BP.Sys.SystemConfig.PathOfCyclostyleFile + "\\Temp.rtf";
                file.PostedFile.SaveAs(tmp);
            }


            //检查文件是否正确。
            try
            {
                string[] paras1 = BP.DA.Cash.GetBillParas_Gener(tmpName, nd.HisFlow.HisGERpt.EnMap.Attrs);
            }
            catch (Exception ex)
            {
                this.Ucsys2.AddMsgOfWarning("Error:", ex.Message);
                return;
            }

            string fullFile1 = FileFullPath(fileName, bt);//BP.Sys.SystemConfig.PathOfCyclostyleFile + "\\" + bt.No + ".rtf";
            System.IO.File.Copy(tmp, fullFile1, true);
            // file.PostedFile.SaveAs(fullFile1);
            bt.FK_BillType = this.Ucsys1.GetDDLByID("DDL_BillType").SelectedItemStringVal;
            bt.Insert();

            #region 更新节点信息。
            string Billids = "";
            BillTemplates tmps = new BillTemplates(nd);
            foreach (BillTemplate Btmp in tmps)
            {
                Billids += "@" + Btmp.No;
            }
            nd.HisBillIDs = Billids;
            nd.Update();
            #endregion 更新节点信息。

            this.Response.Redirect("Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID, true);
        }
        void btn_Del_Click(object sender, EventArgs e)
        {
            BillTemplate t = new BillTemplate();
            t.No = this.RefNo;
            t.Delete();

            #region 更新节点信息。
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            string Billids = "";
            BillTemplates tmps = new BillTemplates(nd);
            foreach (BillTemplate tmp in tmps)
            {
                Billids += "@" + tmp.No;
            }
            nd.HisBillIDs = Billids;
            nd.Update();
            #endregion 更新节点信息。
            this.Response.Redirect("Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID, true);
        }
        /// <summary>
        /// 类别修改
        /// </summary>
        public void EditTypes()
        {
            this.Ucsys1.AddTable();
            this.Ucsys1.AddCaptionLeft("<a href='Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID + "'>返回</a> -单据类别维护");
            this.Ucsys1.AddTR();
            this.Ucsys1.AddTDTitle("类别编号");
            this.Ucsys1.AddTDTitle("类别名称");
            this.Ucsys1.AddTREnd();

            BillTypes ens = new BillTypes();
            ens.RetrieveAll();
            for (int i = 1; i < 18; i++)
            {
                this.Ucsys1.AddTR();
                this.Ucsys1.AddTD(i.ToString().PadLeft(2, '0'));
                TextBox tb = new TextBox();
                tb.ID = "TB_" + i;
                tb.Columns = 50;
                try
                {
                    BillType en = ens[i - 1] as BillType;
                    tb.Text = en.Name;
                    this.Ucsys1.AddTD(tb);
                }
                catch
                {
                    this.Ucsys1.AddTD(tb);
                }
                this.Ucsys1.AddTREnd();
            }

            this.Ucsys1.AddTableEndWithHR();
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "保存";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_SaveTypes_Click);
            this.Ucsys1.Add(btn);
        }
        protected void btn_SaveTypes_Click(object sender, EventArgs e)
        {
            BillTypes ens = new BillTypes();
            ens.RetrieveAll();
            ens.Delete();
            for (int i = 1; i < 18; i++)
            {
                string name = this.Ucsys1.GetTextBoxByID("TB_" + i).Text;
                if (string.IsNullOrEmpty(name))
                    continue;

                BillType en = new BillType();
                en.No = i.ToString().PadLeft(2, '0');
                en.Name = name;
                en.FK_Flow = this.FK_Flow;
                en.Insert();
            }
            this.Alert("保存成功.");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "节点单据设计"; //"节点单据设计";
            switch (this.DoType)
            {
                case "Edit":
                    BillTemplate bk1 = new BillTemplate(this.RefNo);
                    bk1.NodeID = this.NodeID;
                    this.DoNew(bk1);
                    return;
                case "New":
                    BillTemplate bk = new BillTemplate();
                    bk.NodeID = this.RefOID;
                    this.DoNew(bk);
                    return;
                case "EditType":
                    EditTypes();
                    return;
                default:
                    break;
            }

            BillTemplates Bills = new BillTemplates(this.NodeID);
            if (Bills.Count == 0)
            {
                this.Response.Redirect("Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID + "&DoType=New", true);
                return;
            }

            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            this.Title = nd.Name + " - " + "单据管理";  //单据管理
            this.Ucsys1.AddTable();
            if (this.RefNo == null)
                this.Ucsys1.AddCaptionLeft("<a href='Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID + "&DoType=New'><img src='../Img/Btn/New.gif' border=0/>新建</a> -<a href='Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID + "&DoType=EditType'><img src='../Img/Btn/Edit.gif' border=0/>类别维护</a>");
            this.Ucsys1.AddTR();
            this.Ucsys1.AddTDTitle("IDX");
            this.Ucsys1.AddTDTitle("编号");
            this.Ucsys1.AddTDTitle("名称");
            this.Ucsys1.AddTDTitle("操作");
            this.Ucsys1.AddTREnd();
            int i = 0;
            foreach (BillTemplate Bill in Bills)
            {
                i++;
                this.Ucsys1.AddTR();
                this.Ucsys1.AddTDIdx(i);

                this.Ucsys1.AddTD(Bill.No);
                string fileUrl = "";
                //../WorkOpt/GridEdit.aspx?grf=" +Bill.Url + ".grf&t="+DateTime.Now.ToString("yyMMddhh:mm:ss")+" target='_blank'
                if (Bill.HisBillFileType == BillFileType.RuiLang)
                {
                    string name = Bill.Url;

                    name = name.Replace('\\', '-');

                    //if (name.Split('\\').Count() > 2)
                    //{
                    //    string tempName = "";
                    //    foreach (string single in name.Split('\\'))
                    //    {
                    //        tempName += single + "-";
                    //    }
                    //    name = tempName.Substring(0, tempName.Length - 1);
                    //}
                    fileUrl = "<a href='Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID +
                              "&DoType=Edit&RefNo=" + Bill.No +
                              "'><img src='../Img/Btn/Edit.gif' border=0/编辑/a>|<a href='../../../DataUser/CyclostyleFile/" +
                              Bill.Url + ".grf'><img src='../Img/Btn/Save.gif' border=0/> 模板下载</a>|<a href='javascript:openEidt(\"" + name + "\")'  ><img src='../Img/Btn/Edit.gif' />编辑模版</a>";
                }
                else
                {
                    fileUrl = "<a href='Bill.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID +
                            "&DoType=Edit&RefNo=" + Bill.No +
                            "'><img src='../Img/Btn/Edit.gif' border=0/编辑/a>|<a href='../../DataUser/CyclostyleFile/" +
                            Bill.Url + ".rtf'><img src='../Img/Btn/save.gif' border=0/> 模板下载</a>";
                }
                this.Ucsys1.AddTD("<img src='../Img/Btn/Word.gif' >" + Bill.Name);
                this.Ucsys1.AddTD(fileUrl);
                this.Ucsys1.AddTREnd();
            }
            this.Ucsys1.AddTableEnd();
        }
    }

}