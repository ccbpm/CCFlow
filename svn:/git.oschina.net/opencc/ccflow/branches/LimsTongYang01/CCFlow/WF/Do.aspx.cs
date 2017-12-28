using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.Web;
using BP.WF.Template;
using BP.WF;
using BP.En;
using BP.Sys;
using System.IO;
using BP.WF.Rpt;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using BP.DA;
using System.Text;

namespace BP.Web.WF
{
    /// <summary>
    /// Do ��ժҪ˵����
    /// </summary>
    public partial class Do : PageBase
    {
        public string ActionType
        {
            get
            {
                string s = this.Request.QueryString["ActionType"];
                if (s == null)
                    s = this.Request.QueryString["DoType"];

                if (string.IsNullOrEmpty(s) && this.Request.QueryString["SID"] != null)
                    s = "Track";
                return s;
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string RefNo
        {
            get
            {
                return this.Request.QueryString["RefNo"];
            }
        }
        public string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        public string FK_Emp
        {
            get
            {
                return this.Request.QueryString["FK_Emp"];
            }
        }
        public string PageID
        {
            get
            {
                return this.Request.QueryString["PageID"];
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public int NodeID
        {
            get
            {
                string s = this.Request.QueryString["NodeID"];
                if (s == null || s == "")
                    s = this.Request.QueryString["FK_Node"];
                return int.Parse(s);
            }
        }
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSA OUR");
            Response.AddHeader("Cache-Control", "no-store");
            Response.AddHeader("Expires", "0");
            Response.AddHeader("Pragma", "no-cache");
            string url = this.Request.RawUrl;
            if (url.Contains("DTT=") == false)
            {
                //this.Response.Redirect(url + "&DTT=" + DateTime.Now.ToString("mmDDhhmmss"), true);
                //return;
            }

            try
            {
                switch (this.ActionType)
                {
                    
                    case "Focus": //��������������.
                        BP.WF.Dev2Interface.Flow_Focus(Int64.Parse(this.Request.QueryString["WorkID"]));
                        this.WinClose("ss");
                        break;
                    case "PutOne": //��������������.
                        Int64 workid42 = Int64.Parse(this.Request.QueryString["WorkID"]);
                        BP.WF.Dev2Interface.Node_TaskPoolPutOne(workid42);
                        this.WinClose("ss");
                        break;
                    case "DoAppTask": // ��������.
                        Int64 workid2 = Int64.Parse(this.Request.QueryString["WorkID"]);
                        BP.WF.Dev2Interface.Node_TaskPoolTakebackOne(workid2);
                        this.WinClose("ss");
                        return;
                    case "DoOpenCC":
                        string fk_flow1 = this.Request.QueryString["FK_Flow"];
                        string fk_node1 = this.Request.QueryString["FK_Node"];
                        string workid1 = this.Request.QueryString["WorkID"];
                        string fid1 = this.Request.QueryString["FID"];
                        string Sta = this.Request.QueryString["Sta"];
                        if (Sta == "0")
                        {
                            BP.WF.Template.CCList cc1 = new BP.WF.Template.CCList();
                            cc1.MyPK = this.Request.QueryString["MyPK"];
                            cc1.Retrieve();
                            cc1.HisSta = CCSta.Read;
                            cc1.Update();
                        }
                        this.Response.Redirect("./WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Flow=" + fk_flow1 + "&FK_Node=" + fk_node1 + "&WorkID=" + workid1 + "&FID=" + fid1, false);
                        return;
                    case "DelCC": //ɾ������.
                        CCList cc = new CCList();
                        cc.MyPK = this.MyPK;
                        cc.Retrieve();
                        cc.HisSta = CCSta.Del;
                        cc.Update();
                        this.WinClose();
                        break;
                    case "DelSubFlow": //ɾ�����̡�
                        try
                        {
                            BP.WF.Dev2Interface.Flow_DeleteSubThread(this.FK_Flow, this.WorkID, "�ֹ�ɾ��");
                            this.WinClose();
                        }
                        catch (Exception ex)
                        {
                            this.WinCloseWithMsg(ex.Message);
                        }
                        break;
                    case "DownBill":
                        Bill b = new Bill(this.MyPK);
                        b.DoOpen();
                        break;
                    case "DelDtl":
                        GEDtls dtls = new GEDtls(this.EnsName);
                        GEDtl dtl = (GEDtl)dtls.GetNewEntity;
                        dtl.OID = this.RefOID;
                        if (dtl.RetrieveFromDBSources() == 0)
                        {
                            this.WinClose();
                            break;
                        }
                        FrmEvents fes = new FrmEvents(this.EnsName); //����¼�.

                        // ����ɾ��ǰ�¼�.
                        try
                        {
                            fes.DoEventNode(BP.WF.XML.EventListDtlList.DtlItemDelBefore, dtl);
                        }
                        catch (Exception ex)
                        {
                            this.WinCloseWithMsg(ex.Message);
                            break;
                        }
                        dtl.Delete();

                        // ����ɾ�����¼�.
                        try
                        {
                            fes.DoEventNode(BP.WF.XML.EventListDtlList.DtlItemDelAfter, dtl);
                        }
                        catch (Exception ex)
                        {
                            this.WinCloseWithMsg(ex.Message);
                            break;
                        }
                        this.WinClose();
                        break;
                    case "EmpDoUp":
                        BP.WF.Port.WFEmp ep = new BP.WF.Port.WFEmp(this.RefNo);
                        ep.DoUp();

                        BP.WF.Port.WFEmps emps111 = new BP.WF.Port.WFEmps();
                        //  emps111.RemoveCash();
                        emps111.RetrieveAll();
                        this.WinClose();
                        break;
                    case "EmpDoDown":
                        BP.WF.Port.WFEmp ep1 = new BP.WF.Port.WFEmp(this.RefNo);
                        ep1.DoDown();

                        BP.WF.Port.WFEmps emps11441 = new BP.WF.Port.WFEmps();
                        //  emps11441.RemoveCash();
                        emps11441.RetrieveAll();
                        this.WinClose();
                        break;

                    case "Track": //ͨ��һ��������һ������.
                        string mySid = this.Request.QueryString["SID"];
                        string[] mystrs = mySid.Split('_');

                        Int64 myWorkID = int.Parse(mystrs[1]);
                        string fk_emp = mystrs[0];
                        int fk_node = int.Parse(mystrs[2]);
                        Node mynd = new Node();
                        mynd.NodeID = fk_node;
                        mynd.RetrieveFromDBSources();

                        string fk_flow = mynd.FK_Flow;
                        string myurl = "./WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Node=" + mynd.NodeID + "&WorkID=" + myWorkID + "&FK_Flow=" + fk_flow;
                        Web.WebUser.SignInOfGener(new BP.Port.Emp(fk_emp));
                        this.Response.Write("<script> window.location.href='" + myurl + "'</script> *^_^*  <br><br>���ڽ���ϵͳ���Ժ������ʱ��û�з�Ӧ����<a href='" + myurl + "'>��������롣</a>");
                        return;
                    case "OF": //ͨ��һ��������һ������.
                        string sid = this.Request.QueryString["SID"];
                        string[] strs = sid.Split('_');
                        GenerWorkerList wl = new GenerWorkerList();
                        int i = wl.Retrieve(GenerWorkerListAttr.FK_Emp, strs[0],
                            GenerWorkerListAttr.WorkID, strs[1],
                            GenerWorkerListAttr.IsPass, 0);

                        if (i == 0)
                        {
                            this.Response.Write("<h2>��ʾ</h2>�˹����Ѿ������˴�����ߴ�������ɾ����");
                            return;
                        }

                        BP.Port.Emp empOF = new BP.Port.Emp(wl.FK_Emp);
                        Web.WebUser.SignInOfGener(empOF);
                        string u = "MyFlow.htm?FK_Flow=" + wl.FK_Flow + "&WorkID=" + wl.WorkID + "&FK_Node=" + wl.FK_Node + "&FID=" + wl.FID;
                        this.Response.Write("<script> window.location.href='" + u + "'</script> *^_^*  <br><br>���ڽ���ϵͳ���Ժ������ʱ��û�з�Ӧ����<a href='" + u + "'>��������롣</a>");
                        return;
                    case "ExitAuth":
                        BP.Port.Emp emp = new BP.Port.Emp(this.FK_Emp);
                        //�����˳����ٽ��е�¼
                        BP.Web.WebUser.Exit();
                        BP.Web.WebUser.SignInOfGener(emp, WebUser.SysLang);
                        this.WinClose();
                        return;
                    case "LogAs":
                        BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(this.FK_Emp);
                        if (wfemp.AuthorIsOK == false)
                        {
                            this.WinCloseWithMsg("��Ȩʧ��");
                            return;
                        }
                        BP.Port.Emp emp1 = new BP.Port.Emp(this.FK_Emp);
                        BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, wfemp.Author, WebUser.Name);
                        this.WinClose();
                        return;
                    case "TakeBack": // ȡ����Ȩ��
                        BP.WF.Port.WFEmp myau = new BP.WF.Port.WFEmp(WebUser.No);
                        BP.DA.Log.DefaultLogWriteLineInfo("ȡ����Ȩ:" + WebUser.No + "ȡ���˶�(" + myau.Author + ")����Ȩ��");
                        myau.Author = "";
                        myau.AuthorWay = 0;
                        myau.Update();
                        this.WinClose();
                        return;
                    case "AutoTo": // ִ����Ȩ��
                        BP.WF.Port.WFEmp au = new BP.WF.Port.WFEmp();
                        au.No = WebUser.No;
                        au.RetrieveFromDBSources();
                        au.AuthorDate = BP.DA.DataType.CurrentData;
                        au.Author = this.FK_Emp;
                        au.AuthorWay = 1;
                        au.Save();
                        BP.DA.Log.DefaultLogWriteLineInfo("ִ����Ȩ:" + WebUser.No + "ִ���˶�(" + au.Author + ")����Ȩ��");
                        this.WinClose();
                        return;
                    case "UnSend": //ִ�г������͡�
                        this.Response.Redirect("./WorkOpt/UnSend.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow, false);
                        return;
                    case "SetBillState":
                        break;
                    case "WorkRpt":
                        Bill bk1 = new Bill(this.Request.QueryString["OID"]);
                        Node nd = new Node(bk1.FK_Node);
                        this.Response.Redirect("WFRpt.aspx?WorkID=" + bk1.WorkID + "&FID=" + bk1.FID + "&FK_Flow=" + nd.FK_Flow + "&NodeId=" + bk1.FK_Node, false);
                        //this.WinOpen();
                        //this.WinClose();
                        break;
                    case "PrintBill":
                        //Bill bk2 = new Bill(this.Request.QueryString["OID"]);
                        //Node nd2 = new Node(bk2.FK_Node);
                        //this.Response.Redirect("NodeRefFunc.aspx?NodeId=" + bk2.FK_Node + "&FlowNo=" + nd2.FK_Flow + "&NodeRefFuncOID=" + bk2.FK_NodeRefFunc + "&WorkFlowID=" + bk2.WorkID);
                        ////this.WinClose();
                        break;
                    //ɾ�������е�һ���ڵ�����ݣ��������칤��
                    case "DeleteFlow":
                        string fk_flowDel = this.Request.QueryString["FK_Flow"];
                        Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);
                        //����DoDeleteWorkFlowByReal����
                        WorkFlow wf = new WorkFlow(new Flow(fk_flowDel), workid);
                        wf.DoDeleteWorkFlowByReal(true);
                        //  Glo.ToMsg("����ɾ���ɹ�");
                        BP.WF.Glo.ToMsg("����ɾ���ɹ�");

                        //this.ToWFMsgPage("����ɾ���ɹ�");
                        break;
                    case "DownFlowSearchExcel":    //�������̲�ѯ�����ת��������߼��������ڴ�try..catch..��
                        break;
                    case "DownFlowSearchToTmpExcel":    //������ģ��
                        break;
                    default:
                        throw new Exception("ActionType error" + this.ActionType);
                }
            }
            catch (Exception ex)
            {
                this.ToErrorPage("ִ����������쳣��<BR>" + ex.Message);
            }
            //�˴�֮�����ټ�һ��switch������Ϊ�������ļ��߼��У�����Response.End()����������˷�������try..catch..�У��ᱨ�߳���ֹ�쳣
            switch (this.ActionType)
            {
                case "DownFlowSearchExcel":
                    DownMyStartFlowExcel();
                    break;
                case "DownFlowSearchToTmpExcel":    //������ģ��
                    DownMyStartFlowToTmpExcel();
                    break;
            }
        }

        /// <summary>
        /// �������ҷ�������̡���ѯ��������úõ�ģ��excel�У�/WF/RptDfine/FlowSearch.htm�еġ�������ģ�塱���ܵ���
        /// added by liuxc,2017-10-13
        /// </summary>
        public void DownMyStartFlowToTmpExcel()
        {
            string searchType = Request.QueryString["SearchType"];

            if (string.IsNullOrWhiteSpace(this.FK_Flow))
                throw new Exception("@����FK_Flow����Ϊ��");

            if (string.IsNullOrWhiteSpace(searchType))
                searchType = "My";
            
            string fk_mapdata = "ND" + int.Parse(FK_Flow) + "Rpt";
            string rptNo = fk_mapdata + searchType;
            MapData mdMyRpt = new MapData(rptNo);
            string tmpFile = null;
            string tmpDir = BP.Sys.SystemConfig.PathOfDataUser + @"TempleteExpEns\" + rptNo + @"\";
            string tmpXml = BP.Sys.SystemConfig.PathOfDataUser + @"TempleteExpEns\" + rptNo + @"\" + mdMyRpt.Name + ".xml";
            DirectoryInfo infoTmpDir = new DirectoryInfo(tmpDir);
            FileInfo[] tmpFiles = null;
            RptExportTemplate tmp = null;
            MapData mdRpt = new MapData(fk_mapdata);
            MapAttrs attrs = new MapAttrs(rptNo);

            UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + rptNo + "_SearchAttrs";
            ur.RetrieveFromDBSources();

            if (!infoTmpDir.Exists)
                infoTmpDir.Create();

            tmpFiles = infoTmpDir.GetFiles(mdMyRpt.Name + ".xls*");

            if (tmpFiles.Length > 0)
                tmpFile = tmpFiles[0].FullName;

            if (!string.IsNullOrWhiteSpace(tmpFile))
            {
                tmp = RptExportTemplate.FromXml(tmpXml);
            }
            else
            {
                throw new Exception(@"" + tmpFile + "ģ���ļ������ڣ�");
            }

            GEEntitys ges = new GEEntitys(rptNo);
            Entities ens = mdMyRpt.HisEns;
            Entity en = ens.GetNewEntity;
            ens = mdRpt.HisEns; //added by liuxc,2016-12-19,���ΪRpt�����࣬������ѯ��ʱ�򣬾Ϳ�����MyRpt�Ĳ�ѯ��������ѯ��Rptʵ�弯��
            QueryObject qo = new QueryObject(ens);

            switch (searchType)
            {
                case "My": //�ҷ����.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);
                    break;
                case "MyDept": //�Ҳ��ŷ����.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FK_Dept, WebUser.FK_Dept);
                    break;
                case "MyJoin": //�Ҳ����.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");
                    break;
                case "Adminer":
                    break;
                default:
                    throw new Exception("err@" + searchType + "��Ǵ���.");
            }

            qo = new BP.WF.HttpHandler.WF_RptDfine(HttpContext.Current).InitQueryObject(qo, mdMyRpt, ges.GetNewEntity.EnMap.Attrs, attrs, ur);
            qo.DoQuery();

            //��ȡ���̰󶨵ı����еı���Ϣ
            List<string> listFrms = new List<string>(); //�洢�󶨱�mapdata���
            FrmNodes frms = new FrmNodes();
            frms.Retrieve(FrmNodeAttr.FK_Flow, FK_Flow);

            foreach (FrmNode fn in frms)
            {
                if (listFrms.Contains(fn.FK_Frm))
                    continue;

                listFrms.Add(fn.FK_Frm);
            }
            
            GEEntitys dtlGes = null;
            QueryObject qo2 = null;
            string dtlNo = tmp.GetDtl();
            Dictionary<string, Entities> frmDatas = new Dictionary<string, Entities>(); //�洢fk_mapdata,Entities
            Dictionary<string, MapAttrs> frmAttrs = new Dictionary<string, MapAttrs>(); //�洢fk_mapdata,MapAttrs
            string oids = GetOidsJoin(ens, "OID", false);

            //��ȡ���󶨱��ļ�¼����
            frmDatas.Add(fk_mapdata, ens);
            frmAttrs.Add(fk_mapdata, new MapAttrs(fk_mapdata));

            //������ϸ����ֶζ���
            if (!string.IsNullOrWhiteSpace(dtlNo))
            {
                frmAttrs.Add(dtlNo, new MapAttrs(dtlNo));
            }

            foreach (string frm in listFrms)
            {
                //���ģ����û���漰�ñ����ֶΰ���Ϣ���򲻼��ش˱�������
                if (!tmp.HaveCellInMapData(frm))
                    continue;

                ges = new GEEntitys(frm);
                qo2 = new QueryObject(ges);

                if (ens.Count > 0)
                    qo2.AddWhereIn("OID", oids);

                qo2.DoQuery();
                frmDatas.Add(frm, ges);
                frmAttrs.Add(frm, new MapAttrs(frm));
            }

            oids = GetOidsJoin(ens, "OID", true);

            //��ȡ������ϸ��ļ�¼����
            if (!string.IsNullOrWhiteSpace(dtlNo))
            {
                dtlGes = new GEEntitys(dtlNo);
                qo2 = new QueryObject(dtlGes);

                if (ens.Count > 0)
                    qo2.AddWhereIn("RefPK", oids);

                qo2.DoQuery();
            }

            IWorkbook wb = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int r = 0;
            int c = 0;
            int lastRowIdx = 0;
            MapAttr mattr = null;
            MapAttr dmattr = null;
            IDataFormat fmt = null;
            int dtlRecordCount = dtlGes != null ? dtlGes.Count : 0;
            string workid = string.Empty;
            Entity newEn = null;
            Entities tens = null;
            DataTable dtData = new DataTable();
            DataRow dr1 = null;

            try
            {
                using (FileStream fs = new FileStream(tmpFile, FileMode.Open, FileAccess.Read))
                {
                    if (Path.GetExtension(tmpFile).ToLower() == ".xls")
                        wb = new HSSFWorkbook(fs);
                    else
                        wb = new XSSFWorkbook(fs);

                    sheet = wb.GetSheetAt(0);
                    fmt = wb.CreateDataFormat();
                    lastRowIdx = sheet.LastRowNum;

                    //��ֱ�����������ʱ���Ƚ�ȱ�ٵ�����������
                    for (int i = sheet.LastRowNum; i < tmp.BeginIdx + ens.Count + dtlRecordCount - 1; i++)
                    {
                        sheet.GetRow(lastRowIdx).CopyRowTo(i + 1);
                    }

                    //������
                    foreach (RptExportTemplateCell tcell in tmp.Cells)
                    {
                        if (string.IsNullOrWhiteSpace(tcell.KeyOfEn))
                            mattr = frmAttrs[tcell.FK_DtlMapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_DtlMapData + "_" + tcell.DtlKeyOfEn) as MapAttr;
                        else
                            mattr = frmAttrs[tcell.FK_MapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_MapData + "_" + tcell.KeyOfEn) as MapAttr;

                        switch (mattr.MyDataType)
                        {
                            case DataType.AppString:
                                dtData.Columns.Add(mattr.MyPK, typeof(string));
                                break;
                            case DataType.AppInt:
                                if (mattr.LGType == FieldTypeS.Normal)
                                    dtData.Columns.Add(mattr.MyPK, typeof(int));
                                else
                                    dtData.Columns.Add(mattr.MyPK, typeof(string));
                                break;
                            case DataType.AppFloat:
                            case DataType.AppMoney:
                                if (mattr.LGType == FieldTypeS.Normal)
                                    dtData.Columns.Add(mattr.MyPK, typeof(double));
                                else
                                    dtData.Columns.Add(mattr.MyPK, typeof(string));
                                break;
                            case DataType.AppDate:
                            case DataType.AppDateTime:
                                dtData.Columns.Add(mattr.MyPK, typeof(string));
                                break;
                            case DataType.AppBoolean:
                                dtData.Columns.Add(mattr.MyPK, typeof(bool));
                                break;
                            default:
                                throw new Exception("δ�漰�����������ͣ����������Ƿ���ȷ��");
                        }
                    }

                    for (int i = 0; i < ens.Count; i++)
                    {
                        //�����������
                        dr1 = dtData.NewRow();

                        foreach (RptExportTemplateCell tcell in tmp.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(tcell.KeyOfEn))
                                continue;

                            mattr = frmAttrs[tcell.FK_MapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_MapData + "_" + tcell.KeyOfEn) as MapAttr;
                            tens = frmDatas[tcell.FK_MapData];

                            if (tcell.FK_MapData != fk_mapdata)
                                newEn = tens.GetEntityByKey(ens[i].PKVal) ?? tens.GetNewEntity;
                            else
                                newEn = ens[i];

                            switch (mattr.MyDataType)
                            {
                                case DataType.AppString:
                                    if (mattr.LGType == FieldTypeS.Normal)
                                        dr1[mattr.MyPK] = newEn.GetValStringByKey(tcell.KeyOfEn);
                                    else
                                        dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.KeyOfEn);
                                    break;
                                case DataType.AppInt:
                                    if (mattr.LGType == FieldTypeS.Normal)
                                        dr1[mattr.MyPK] = newEn.GetValIntByKey(tcell.KeyOfEn);
                                    else
                                        dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.KeyOfEn);
                                    break;
                                case DataType.AppFloat:
                                case DataType.AppMoney:
                                    if (mattr.LGType == FieldTypeS.Normal)
                                        dr1[mattr.MyPK] = newEn.GetValDoubleByKey(tcell.KeyOfEn);
                                    else
                                        dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.KeyOfEn);
                                    break;
                                case DataType.AppDate:
                                case DataType.AppDateTime:
                                    dr1[mattr.MyPK] = newEn.GetValStringByKey(tcell.KeyOfEn, "");
                                    break;
                                case DataType.AppBoolean:
                                    dr1[mattr.MyPK] = newEn.GetValBooleanByKey(tcell.KeyOfEn);
                                    break;
                                default:
                                    throw new Exception("δ�漰�����������ͣ����������Ƿ���ȷ��");
                            }
                        }

                        dtData.Rows.Add(dr1);

                        //�����ϸ������
                        if (dtlGes == null)
                            continue;

                        workid = ens[i].GetValIntByKey("OID").ToString();

                        foreach (GEEntity gen in dtlGes)
                        {
                            if (gen.GetValStringByKey("RefPK") != workid) continue;

                            dr1 = dtData.NewRow();

                            foreach (RptExportTemplateCell tcell in tmp.Cells)
                            {
                                if (string.IsNullOrWhiteSpace(tcell.DtlKeyOfEn))
                                    continue;

                                if (string.IsNullOrWhiteSpace(tcell.KeyOfEn))
                                    mattr = frmAttrs[tcell.FK_DtlMapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_DtlMapData + "_" + tcell.DtlKeyOfEn) as MapAttr;
                                else
                                    mattr = frmAttrs[tcell.FK_MapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_MapData + "_" + tcell.KeyOfEn) as MapAttr;

                                newEn = gen;

                                switch (mattr.MyDataType)
                                {
                                    case DataType.AppString:
                                        if (mattr.LGType == FieldTypeS.Normal)
                                            dr1[mattr.MyPK] = newEn.GetValStringByKey(tcell.DtlKeyOfEn);
                                        else
                                            dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.DtlKeyOfEn);
                                        break;
                                    case DataType.AppInt:
                                        if (mattr.LGType == FieldTypeS.Normal)
                                            dr1[mattr.MyPK] = newEn.GetValIntByKey(tcell.DtlKeyOfEn);
                                        else
                                        {
                                            //�˴���Ҫ������ϸ��ĸ��ֶ����������Ƿ�������һ��
                                            dmattr = frmAttrs[tcell.FK_DtlMapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_DtlMapData + "_" + tcell.DtlKeyOfEn) as MapAttr;
                                            if (dmattr.MyDataType == mattr.MyDataType)
                                                dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.DtlKeyOfEn);
                                            else
                                                dr1[mattr.MyPK] = newEn.GetValStringByKey(tcell.DtlKeyOfEn);
                                        }
                                        break;
                                    case DataType.AppFloat:
                                    case DataType.AppMoney:
                                        if (mattr.LGType == FieldTypeS.Normal)
                                            dr1[mattr.MyPK] = newEn.GetValDoubleByKey(tcell.DtlKeyOfEn);
                                        else
                                            dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.DtlKeyOfEn);
                                        break;
                                    case DataType.AppDate:
                                    case DataType.AppDateTime:
                                        dr1[mattr.MyPK] = newEn.GetValStringByKey(tcell.DtlKeyOfEn);
                                        break;
                                    case DataType.AppBoolean:
                                        dr1[mattr.MyPK] = newEn.GetValBooleanByKey(tcell.DtlKeyOfEn);
                                        break;
                                    default:
                                        throw new Exception("δ�漰�����������ͣ����������Ƿ���ȷ��");
                                }
                            }

                            dtData.Rows.Add(dr1);
                        }
                    }

                    //д��excel��Ԫ��ֵ
                    for (int i = 0; i < dtData.Rows.Count; i++)
                    {
                        dr1 = dtData.Rows[i];

                        foreach (RptExportTemplateCell tcell in tmp.Cells)
                        {
                            r = tmp.Direction == FillDirection.Vertical
                                    ? (i + tmp.BeginIdx)
                                    : tcell.RowIdx;
                            c = tmp.Direction == FillDirection.Vertical
                                    ? tcell.ColumnIdx
                                    : (i + tmp.BeginIdx);
                            row = sheet.GetRow(r);
                            cell = row.GetCell(c);

                            if (cell == null)
                            {
                                cell = row.CreateCell(c);
                            }

                            if (string.IsNullOrWhiteSpace(tcell.KeyOfEn))
                                mattr = frmAttrs[tcell.FK_DtlMapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_DtlMapData + "_" + tcell.DtlKeyOfEn) as MapAttr;
                            else
                                mattr = frmAttrs[tcell.FK_MapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_MapData + "_" + tcell.KeyOfEn) as MapAttr;

                            switch (mattr.MyDataType)
                            {
                                case DataType.AppString:
                                    cell.SetCellValue(dr1[mattr.MyPK] as string);
                                    break;
                                case DataType.AppInt:
                                    if (mattr.LGType == FieldTypeS.Normal)
                                        cell.SetCellValue((int)dr1[mattr.MyPK]);
                                    else
                                        cell.SetCellValue(dr1[mattr.MyPK] as string);
                                    break;
                                case DataType.AppFloat:
                                case DataType.AppMoney:
                                    if (mattr.LGType == FieldTypeS.Normal)
                                        cell.SetCellValue((double)dr1[mattr.MyPK]);
                                    else
                                        cell.SetCellValue(dr1[mattr.MyPK] as string);
                                    break;
                                case DataType.AppDate:
                                    cell.SetCellValue(dr1[mattr.MyPK] as string);
                                    cell.CellStyle.DataFormat = fmt.GetFormat("yyyy-m-d;@");
                                    break;
                                case DataType.AppDateTime:
                                    cell.SetCellValue(dr1[mattr.MyPK] as string);
                                    cell.CellStyle.DataFormat = fmt.GetFormat("yyyy-m-d h:mm;@");
                                    break;
                                case DataType.AppBoolean:
                                    cell.SetCellValue((bool)dr1[mattr.MyPK]);
                                    break;
                                default:
                                    throw new Exception("δ�漰�����������ͣ����������Ƿ���ȷ��");
                            }
                        }
                    }

                    //��������
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.Write(ms);
                        byte[] bs = ms.GetBuffer(); //2016-12-17��ֱ��ʹ��ms�ᱨ�������Ƚ������ݴ洢������ʹ��

                        Response.AddHeader("Content-Length", bs.Length.ToString());
                        Response.ContentType = "application/octet-stream";
                        Response.AddHeader("Content-Disposition",
                                           "attachment; filename=" +
                                           HttpUtility.UrlEncode(
                                               mdMyRpt.Name + "_" +
                                               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                                               Path.GetExtension(tmpFile), Encoding.UTF8));
                        Response.BinaryWrite(bs);
                        wb = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("���ݵ���ʱ���ִ���@������Ϣ��" + ex.Message);
            }
        }

        /// <summary>
        /// �������ҷ�������̡���ѯ�����Excel�ļ���/WF/RptDfine/FlowSearch.htm�еġ����������ܵ���
        /// </summary>
        public void DownMyStartFlowExcel()
        {
            string searchType = Request.QueryString["SearchType"];

            if (string.IsNullOrWhiteSpace(this.FK_Flow))
                throw new Exception("@����FK_Flow����Ϊ��");

            if (string.IsNullOrWhiteSpace(searchType))
                searchType = "My";

            string rptmd = "ND" + int.Parse(this.FK_Flow) + "Rpt";
            string rptNo = rptmd + searchType;

            UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + rptNo + "_SearchAttrs";
            ur.RetrieveFromDBSources();
            
            MapData md = new MapData(rptNo);
            MapAttrs attrs = new MapAttrs(rptNo);
            GEEntitys ges = new GEEntitys(rptNo);
            QueryObject qo = new QueryObject(ges);

            switch (searchType)
            {
                case "My": //�ҷ����.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);
                    break;
                case "MyDept": //�Ҳ��ŷ����.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FK_Dept, WebUser.FK_Dept);
                    break;
                case "MyJoin": //�Ҳ����.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");
                    break;
                case "Adminer":
                    break;
                default:
                    throw new Exception("err@" + searchType + "��Ǵ���.");
            }
            
            qo = new BP.WF.HttpHandler.WF_RptDfine(HttpContext.Current).InitQueryObject(qo, md, ges.GetNewEntity.EnMap.Attrs, attrs, ur);

            DataTable dt = qo.DoQueryToTable();
            DataTable myDT = new DataTable();

            foreach (MapAttr attr in attrs)
            {
                if (attr.KeyOfEn == "MyNum")
                    continue;

                Type t = null;

                switch (attr.LGType)
                {
                    case FieldTypeS.Normal:
                        switch (attr.MyDataType)
                        {
                            case BP.DA.DataType.AppInt:
                                t = typeof(int);
                                break;
                            case BP.DA.DataType.AppFloat:
                            case BP.DA.DataType.AppDouble:
                            case BP.DA.DataType.AppMoney:
                                t = typeof(double);
                                break;
                            default:
                                t = typeof(string);
                                break;
                        }
                        break;
                    default:
                        t = typeof(string);
                        break;
                }

                myDT.Columns.Add(new DataColumn(attr.Name, t));
                myDT.Columns[attr.Name].ExtendedProperties.Add("width", attr.UIWidthInt);

                if (attr.IsNum && attr.LGType == FieldTypeS.Normal && "OID,FID,PWorkID,FlowEndNode,PNodeID".IndexOf(attr.KeyOfEn) == -1)
                    myDT.Columns[attr.Name].ExtendedProperties.Add("sum", attr.IsSum);
            }

            foreach (DataRow dr in dt.Rows)
            {
                DataRow myDR = myDT.NewRow();

                foreach (MapAttr attr in attrs)
                {
                    if (attr.KeyOfEn == "MyNum")
                        continue;

                    switch (attr.LGType)
                    {
                        case FieldTypeS.Normal:
                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:
                                case BP.DA.DataType.AppDate:
                                case BP.DA.DataType.AppDateTime:
                                case BP.DA.DataType.AppInt:
                                case BP.DA.DataType.AppFloat:
                                case BP.DA.DataType.AppDouble:
                                case BP.DA.DataType.AppMoney:
                                    myDR[attr.Name] = dr[attr.Field];
                                    break;
                                case BP.DA.DataType.AppBoolean:
                                    if (dr[attr.Field].ToString() == "0")
                                        myDR[attr.Name] = "��";
                                    else
                                        myDR[attr.Name] = "��";
                                    break;
                            }
                            break;
                        case FieldTypeS.Enum:
                            SysEnum sem = new SysEnum();
                            sem.Retrieve(SysEnumAttr.EnumKey, attr.KeyOfEn, SysEnumAttr.IntKey, dr[attr.Field]);
                            myDR[attr.Name] = sem.Lab;
                            break;
                        case FieldTypeS.FK:
                            string tabName = attr.UIBindKey;
                            if (attr.KeyOfEn == "FK_NY")
                            {
                                tabName = "Pub_NY";
                            }
                            else if (attr.KeyOfEn == "FK_Dept")
                            {
                                tabName = "Port_Dept";
                            }
                            DataTable drDt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM " + tabName + " WHERE NO='" + dr[attr.Field] + "'");
                            if (drDt.Rows.Count > 0)
                                myDR[attr.Name] = drDt.Rows[0]["NAME"].ToString();
                            break;
                        case FieldTypeS.WinOpen:
                            break;
                    }
                }

                myDT.Rows.Add(myDR);
            }

            Flow flow = new Flow(this.FK_Flow);
            string name = string.Empty;

            switch(searchType)
            {
                case "My": //�ҷ����.
                    name = "�ҷ��������";
                    break;
                case "MyDept": //�Ҳ��ŷ����.
                    name = "���ŷ��������";
                    break;
                case "MyJoin": //�Ҳ����.
                    name = "������������";
                    break;
                case "Adminer":
                    name = "�߼���ѯ";
                    break;
                default:
                    break;
            }

            name += "��" + flow.Name + "��";
            string filename = Request.PhysicalApplicationPath + @"\Temp\" + name + "_" + DateTime.Today.ToString("yyyy��MM��dd��") + ".xls";
            CCFlow.WF.Comm.Utilities.NpoiFuncs.DataTableToExcel(myDT, filename, name,
                                                                BP.Web.WebUser.Name, true, true, true);
        }

        /// <summary>
        /// ��ȡָ���ֶε�ƴ���ַ�����ʽ����Ӣ�Ķ�������
        /// </summary>
        /// <param name="ens">ʵ�弯��</param>
        /// <param name="field">�ֶ�</param>
        /// <param name="isVarchar">ֵ�Ƿ����ַ�</param>
        /// <returns></returns>
        private string GetOidsJoin(Entities ens, string field, bool isVarchar)
        {
            string oids = string.Empty;

            foreach (Entity en1 in ens)
            {
                oids += (isVarchar ? "'" : "") + en1.GetValByKey(field) + (isVarchar ? "'" : "") + ",";
            }

            return "(" + oids.TrimEnd(',') + ")";
        }

        #region Web ������������ɵĴ���
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: �õ����� ASP.NET Web ���������������ġ�
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
        /// �˷��������ݡ�
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion
    }
}
