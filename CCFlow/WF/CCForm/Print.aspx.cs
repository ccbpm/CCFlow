using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.WF.Data;
using BP.Sys;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
namespace CCFlow.WF.CCForm
{
    public partial class WF_CCForm_Print : BP.Web.WebPage
    {
        #region 属性
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
                try
                {
                    return Int64.Parse(this.Request.QueryString["WorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public Int64 FID
        {
            get
            {
                try
                {
                    return Int64.Parse(this.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public string BillIdx
        {
            get
            {
                return this.Request.QueryString["BillIdx"];
            }
        }
        #endregion 属性

        string ApplicationPath = null;
        public void PrintBill()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string path = ApplicationPath + "\\DataUser\\CyclostyleFile\\FlowFrm\\" + nd.FK_Flow + "\\" + nd.NodeID + "\\";
            if (System.IO.Directory.Exists(path) == false)
            {
                this.Pub1.AddMsgOfWarning("获取模版错误", "模版文件没有找到。" + path);
                return;
            }

            string[] fls = System.IO.Directory.GetFiles(path);
            string file = fls[int.Parse(this.BillIdx)];
            file = file.Replace(ApplicationPath + @"DataUser\CyclostyleFile", "");

            FileInfo finfo = new FileInfo(file);
            string tempName = finfo.Name.Split('.')[0];
            string tempNameChinese = finfo.Name.Split('.')[1];

            string toPath = ApplicationPath + @"DataUser\Bill\FlowFrm\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (System.IO.Directory.Exists(toPath) == false)
                System.IO.Directory.CreateDirectory(toPath);

            // string billFile = toPath + "\\" + tempName + "." + this.FID + ".doc";
            string billFile = toPath + "\\" + Server.UrlDecode(tempNameChinese) + "." + this.WorkID + ".doc";

            BP.Pub.RTFEngine engine = new BP.Pub.RTFEngine();
            if (tempName.ToLower() == "all")
            {
                /* 说明要从所有的独立表单上取数据. */
                FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
                foreach (FrmNode fn in fns)
                {
                    GEEntity ge = new GEEntity(fn.FK_Frm, this.WorkID);
                    engine.AddEn(ge);
                    MapDtls mdtls = new MapDtls(fn.FK_Frm);
                    foreach (MapDtl dtl in mdtls)
                    {
                        GEDtls enDtls = dtl.HisGEDtl.GetNewEntities as GEDtls;
                        enDtls.Retrieve(GEDtlAttr.RefPK, this.WorkID);
                        engine.EnsDataDtls.Add(enDtls);
                    }
                }

                // 增加主表.
                GEEntity myge = new GEEntity("ND" + nd.NodeID, this.WorkID);
                engine.AddEn(myge);

                //增加从表
                MapDtls mymdtls = new MapDtls(tempName);
                foreach (MapDtl dtl in mymdtls)
                {
                    GEDtls enDtls = dtl.HisGEDtl.GetNewEntities as GEDtls;
                    enDtls.Retrieve(GEDtlAttr.RefPK, this.WorkID);
                    engine.EnsDataDtls.Add(enDtls);
                }

                //增加多附件数据
                FrmAttachments aths = new FrmAttachments(tempName);
                foreach (FrmAttachment athDesc in aths)
                {
                    FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
                    if (athDBs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, athDesc.MyPK, FrmAttachmentDBAttr.RefPKVal, this.WorkID, "RDT") == 0)
                        continue;

                    engine.EnsDataAths.Add(athDesc.NoOfObj, athDBs);
                }
                // engine.MakeDoc(file, toPath, tempName + "." + this.WorkID + ".doc", null, false);
                engine.MakeDoc(file, toPath, Server.UrlDecode(tempNameChinese) + "." + this.WorkID + ".doc", null, false);
            }
            else
            {
                // 增加主表.
                GEEntity myge = new GEEntity(tempName, this.WorkID);
                engine.HisGEEntity = myge;
                engine.AddEn(myge);

                //增加从表.
                MapDtls mymdtls = new MapDtls(tempName);
                foreach (MapDtl dtl in mymdtls)
                {
                    GEDtls enDtls = dtl.HisGEDtl.GetNewEntities as GEDtls;
                    enDtls.Retrieve(GEDtlAttr.RefPK, this.WorkID);
                    engine.EnsDataDtls.Add(enDtls);
                }

                //增加轨迹表.
                Paras ps = new BP.DA.Paras();
                ps.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                ps.Add(TrackAttr.ActionType, (int)ActionType.WorkCheck);
                ps.Add(TrackAttr.WorkID, this.WorkID);
                engine.dtTrack = BP.DA.DBAccess.RunSQLReturnTable(ps);

                engine.MakeDoc(file, toPath, Server.UrlDecode(tempNameChinese) + "." + this.WorkID + ".doc", null, false);
            }

            #region 保存单据，以方便查询.
            Bill bill = new Bill();
            bill.MyPK = this.FID + "_" + this.WorkID + "_" + this.FK_Node + "_" + this.BillIdx;
            bill.WorkID = this.WorkID;
            bill.FK_Node = this.FK_Node;
            bill.FK_Dept = WebUser.FK_Dept;
            bill.FK_Emp = WebUser.No;

            bill.Url = "/DataUser/Bill/FlowFrm/" + DateTime.Now.ToString("yyyyMMdd") + "/" + Server.UrlDecode(tempNameChinese) + "." + this.WorkID + ".doc";
            bill.FullPath = toPath + file;

            bill.RDT = DataType.CurrentDataTime;
            bill.FK_NY = DataType.CurrentYearMonth;
            bill.FK_Flow = this.FK_Flow;
            if (this.WorkID != 0)
            {
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = this.WorkID;
                if (gwf.RetrieveFromDBSources() == 1)
                {
                    bill.Emps = gwf.Emps;
                    bill.FK_Starter = gwf.Starter;
                    bill.StartDT = gwf.RDT;
                    bill.Title = gwf.Title;
                    bill.FK_Dept = gwf.FK_Dept;
                }
            }

            try
            {
                bill.Insert();
            }
            catch
            {
                bill.Update();
            }
            #endregion

            BillTemplates templates = new BillTemplates();
            int iHave = templates.Retrieve(BillTemplateAttr.NodeID, this.FK_Node, BillTemplateAttr.BillOpenModel, (int)BillOpenModel.WebOffice);
            //在线WebOffice打开
            if (iHave > 0)
            {
                Response.Redirect("../WebOffice/PrintOffice.aspx?MyPK=" + bill.MyPK, true);
            }
            else
            {
                BP.Sys.PubClass.OpenWordDocV2(billFile, tempNameChinese + ".doc");
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ApplicationPath = this.Request.PhysicalApplicationPath;
            this.Title = "单据打印";
            if (this.BillIdx != null)
            {
                this.PrintBill();
                return;
            }

            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID");
            this.Pub1.AddTDTitle("表单编号");
            this.Pub1.AddTDTitle("表单名");
            this.Pub1.AddTDTitle("下载");
            this.Pub1.AddTREnd();
            
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string path = ApplicationPath + @"DataUser\CyclostyleFile\FlowFrm\" + nd.FK_Flow + "\\" + nd.NodeID + "\\";
            string[] fls = null;
            try
            {
                fls = System.IO.Directory.GetFiles(path);
            }
            catch
            {
                this.Pub1.AddTableEnd();
                this.Pub1.AddMsgOfWarning("获取模版错误", "模版文件没有找到。" + path);
                return;
            }

            int idx = 0;
            int fileIdx = -1;
            foreach (string f in fls)
            {
                fileIdx++;
                string myfile = f.Replace(path, "");
                string[] strs = myfile.Split('.');
                idx++;

                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD(strs[0]);
                this.Pub1.AddTD(strs[1]);

                if (f.Contains(".grf"))
                {
                    string fileName = f.Split('\\')[f.Split('\\').Length - 1];
                    this.Pub1.AddTD("<a href='javascript:btnPreview_onclick(\"" + fileName + "\")' >打印</a>");
                }
                else
                {
                    this.Pub1.AddTD("<a href='Print.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&BillIdx=" + fileIdx + "' target=_blank >打印</a>");
                }

                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
    }
}