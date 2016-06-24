using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Port;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.Sys;
using BP.WF.Template;

namespace CCFlow.WF.WorkOpt
{
    public partial class WF_WorkOpt_PrintDoc : BP.Web.WebPage
    {
        #region 属性
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }

        public string FK_Flow
        {
            get { return Request.QueryString["FK_Flow"]; }
        }
        public Int64 WorkID
        {
            get
            {
                return int.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public string FK_Bill
        {
            get
            {
                return this.Request.QueryString["FK_Bill"];
            }
        }

        private bool isRuiLang = false;

        public bool IsRuiLang
        {
            get { return isRuiLang; }
            set { isRuiLang = value; }
        }
        #endregion end 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            BillTemplates templetes = new BillTemplates();
            templetes.Retrieve(BillTemplateAttr.NodeID, this.FK_Node);
            if (templetes.Count == 0)
            {
                this.WinCloseWithMsg("当前节点上没有绑定单据模板。");
                return;
            }

            if (templetes.Count == 1)
            {
               PrintDocV3(templetes[0] as BillTemplate);
                return;
            }

            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft("请选择要打印的单据");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("单据编号");
            this.Pub1.AddTDTitle("单据名称");
            this.Pub1.AddTDTitle("打印");
            this.Pub1.AddTREnd();

            foreach (BillTemplate en in templetes)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTD(en.No);
                this.Pub1.AddTD(en.Name);
                this.Pub1.AddTD("<a href='PrintDoc.aspx?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Bill=" + en.No + "&FK_Flow="+this.FK_Flow+"' >打印</a>");
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();

            if (this.FK_Bill != null)
            {
                BillTemplate templete = new BillTemplate(this.FK_Bill);

                if (templete.HisBillFileType == BillFileType.RuiLang)
                    this.PrintDocV4(templete);
                else
                    this.PrintDocV2(templete);
            }
        }
        /// <summary>
        /// 瑞郎
        /// </summary>
        /// <param name="func"></param>
        public void PrintDocV4(BillTemplate func)
        {
            IsRuiLang = true;

            Button button = new Button();

            button.OnClientClick = "return btnPreview_onclick('"+func.Url+"')";

            button.Text = "预览 '"+func.Name+"'";

            this.Pub1.Add(button);
        }

        public void PrintDocV3(BillTemplate funcs)
        {

            switch (funcs.HisBillFileType)
                {
                    case BillFileType.Word:
                    case BillFileType.Excel:
                        PrintDocV2(funcs);
                        break;
                    case BillFileType.RuiLang:
                        PrintDocV4(funcs);
                        break;
                    default:
                        break;
                }

            
        }
        /// <summary>
        /// 打印单据
        /// </summary>
        /// <param name="func"></param>
        public void PrintDocV2(BillTemplate func)
        {
            string billInfo = "";
            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();
            wk.ResetDefaultVal();

            string file = DateTime.Now.Year + "_" + WebUser.FK_Dept + "_" + func.No + "_" + WorkID + ".doc";
            BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine();

            string[] paths;
            string path;
            try
            {
                #region 生成单据
                rtf.HisEns.Clear();
                rtf.EnsDataDtls.Clear();
                if (func.NodeID == 0)
                {

                }
                else
                {
                    //WorkNodes wns = new WorkNodes();
                    //if (nd.HisRunModel == RunModel.FL
                    //    || nd.HisRunModel == RunModel.FHL
                    //    || nd.HisRunModel == RunModel.HL)
                    //    wns.GenerByFID(nd.HisFlow, this.WorkID);
                    //else
                    //    wns.GenerByWorkID(nd.HisFlow, this.WorkID);

                    //把流程主表数据放入里面去.
                    GEEntity ndxxRpt = new GEEntity("ND" + int.Parse(nd.FK_Flow) + "Rpt");
                    ndxxRpt.PKVal = this.WorkID;
                    ndxxRpt.Retrieve();
                    ndxxRpt.Copy(wk);

                    //把数据赋值给wk.
                    wk.Row = ndxxRpt.Row;
                    rtf.HisGEEntity = wk;

                    //加入他的明细表.
                    List<Entities> al = wk.GetDtlsDatasOfList();
                    foreach (Entities ens in al)
                        rtf.AddDtlEns(ens);

                    //rtf.AddEn(wk);
                    ////if (wns.Count == 0)
                    ////    works = nd.HisWorks;
                    ////else
                    ////    works = wns.GetWorks;
                    //foreach (Work mywk in works)
                    //{
                    //    if (mywk.OID == 0)
                    //        continue;
                    //    rtf.AddEn(mywk);
                    //    rtf.ensStrs += ".ND" + mywk.NodeID;
                    //}

                }

                paths = file.Split('_');
                path = paths[0] + "/" + paths[1] + "/" + paths[2] + "/";

                string billUrl = BP.WF.Glo.CCFlowAppPath+ "DataUser/Bill/" + path + file;

                if (func.HisBillFileType == BillFileType.PDF)
                {
                    billUrl = billUrl.Replace(".doc", ".pdf");
                    billInfo += "<img src='/WF/Img/FileType/PDF.gif' /><a href='" + billUrl + "' target=_blank >" + func.Name + "</a>";
                }
                else
                {
                    billInfo += "<img src='/WF/Img/FileType/doc.gif' /><a href='" + billUrl + "' target=_blank >" + func.Name + "</a>";
                }

                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + "\\" + WebUser.FK_Dept + "\\" + func.No + "\\";
              //  path = Server.MapPath(path);
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                rtf.MakeDoc(func.Url + ".rtf",
                    path, file, func.ReplaceVal, false);
                #endregion

                #region 转化成pdf.
                if (func.HisBillFileType == BillFileType.PDF)
                {
                    string rtfPath = path + file;
                    string pdfPath = rtfPath.Replace(".doc", ".pdf");
                    try
                    {
                        BP.WF.Glo.Rtf2PDF(rtfPath, pdfPath);
                    }
                    catch (Exception ex)
                    {
                        billInfo = ex.Message;
                        //this.addMsg("RptError", "产生报表数据错误:" + ex.Message);
                    }
                }
                #endregion

                #region 保存单据
                Bill bill = new Bill();
                bill.MyPK = wk.FID + "_" + wk.OID + "_" + nd.NodeID + "_" + func.No;
                bill.FID = wk.FID;
                bill.WorkID = wk.OID;
                bill.FK_Node = wk.NodeID;
                bill.FK_Dept = WebUser.FK_Dept;
                bill.FK_Emp = WebUser.No;
                bill.Url = billUrl;
                bill.RDT = DataType.CurrentDataTime;
                bill.FullPath = path + file;
                bill.FK_NY = DataType.CurrentYearMonth;
                bill.FK_Flow = nd.FK_Flow;
                bill.FK_BillType = func.FK_BillType;
                bill.Emps = rtf.HisGEEntity.GetValStrByKey("Emps");
                bill.FK_Starter = rtf.HisGEEntity.GetValStrByKey("Rec");
                bill.StartDT = rtf.HisGEEntity.GetValStrByKey("RDT");
                bill.Title = rtf.HisGEEntity.GetValStrByKey("Title");
                bill.FK_Dept = rtf.HisGEEntity.GetValStrByKey("FK_Dept");
                try
                {
                    bill.Save();
                }
                catch
                {
                    bill.Update();
                }
                #endregion
            }
            catch (Exception ex)
            {
                BP.WF.DTS.InitBillDir dir = new BP.WF.DTS.InitBillDir();
                dir.Do();
                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + "\\" + WebUser.FK_Dept + "\\" + func.No + "\\";
                string msgErr = "@" + string.Format("生成单据失败，请让管理员检查目录设置") + "[" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
                billInfo += "@<font color=red>" + msgErr + "</font>";
                throw new Exception(msgErr + "@其它信息:" + ex.Message);
            }

            this.Pub1.AddFieldSet("打印单据");
            this.Pub1.AddUL();
            this.Pub1.AddLi(billInfo);
            this.Pub1.AddULEnd();
            this.Pub1.AddFieldSetEnd();
            return;
        }

        public void PrintDoc(BillTemplate en)
        {
            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();
            string msg = "";
            string file = DataType.CurrentYear + "_" + WebUser.FK_Dept + "_" + en.No + "_" + this.WorkID + ".doc";
            BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine();
            //        Works works;
            string[] paths;
            string path;
            try
            {
                #region 生成单据
                rtf.HisEns.Clear();
                rtf.EnsDataDtls.Clear();
                rtf.AddEn(wk);
                rtf.ensStrs += ".ND" + wk.NodeID;
                ArrayList al = wk.GetDtlsDatasOfArrayList();
                foreach (Entities ens in al)
                    rtf.AddDtlEns(ens);

                BP.Sys.GEEntity ge = new BP.Sys.GEEntity("ND" + int.Parse(nd.FK_Flow) + "Rpt");
                ge.Copy(wk);
                rtf.HisGEEntity = ge;

                paths = file.Split('_');
                path = paths[0] + "/" + paths[1] + "/" + paths[2] + "/";

                path = BP.WF.Glo.FlowFileBill + DataType.CurrentYear + "\\" + WebUser.FK_Dept + "\\" + en.No + "\\";
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);
                // rtf.ensStrs = ".ND";
                rtf.MakeDoc(en.Url + ".rtf",
                    path, file, en.ReplaceVal, false);
                #endregion

                #region 转化成pdf.
                if (en.HisBillFileType == BillFileType.PDF)
                {
                    string rtfPath = path + file;
                    string pdfPath = rtfPath.Replace(".doc", ".pdf");
                    try
                    {
                        BP.WF.Glo.Rtf2PDF(rtfPath, pdfPath);

                        file = file.Replace(".doc", ".pdf");
                        System.IO.File.Delete(rtfPath);

                        file = file.Replace(".doc", ".pdf");
                        //System.IO.File.Delete(rtfPath);
                    }
                    catch (Exception ex)
                    {
                        msg += ex.Message;
                    }
                }
                #endregion

                string url = BP.WF.Glo.CCFlowAppPath + "DataUser/Bill/" + DataType.CurrentYear + "/" + WebUser.FK_Dept + "/" + en.No + "/" + file;
                this.Response.Redirect(url, false);
                //         BP.Sys.PubClass.OpenWordDocV2( path+file, en.Name);
            }
            catch (Exception ex)
            {
                BP.WF.DTS.InitBillDir dir = new BP.WF.DTS.InitBillDir();
                dir.Do();
                path = BP.WF.Glo.FlowFileBill + DataType.CurrentYear + "\\" + WebUser.FK_Dept + "\\" + en.No + "\\";
                string msgErr = "@生成单据失败，请让管理员检查目录设置 [" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
                throw new Exception(msgErr + "@其它信息:" + ex.Message);
            }
        }
    }

}