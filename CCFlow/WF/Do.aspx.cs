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
    /// Do 的摘要说明。
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

                if (DataType.IsNullOrEmpty(s) && this.Request.QueryString["SID"] != null)
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
                    
                    case "Focus": //把任务放入任务池.
                        BP.WF.Dev2Interface.Flow_Focus(Int64.Parse(this.Request.QueryString["WorkID"]));
                        this.WinClose("ss");
                        break;
                    case "PutOne": //把任务放入任务池.
                        Int64 workid42 = Int64.Parse(this.Request.QueryString["WorkID"]);
                        BP.WF.Dev2Interface.Node_TaskPoolPutOne(workid42);
                        this.WinClose("ss");
                        break;
                    case "DoAppTask": // 申请任务.
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
                    case "DelCC": //删除抄送.
                        CCList cc = new CCList();
                        cc.MyPK = this.MyPK;
                        cc.Retrieve();
                        cc.HisSta = CCSta.Del;
                        cc.Update();
                        this.WinClose();
                        break;
                    case "DelSubFlow": //删除进程。
                        try
                        {
                            BP.WF.Dev2Interface.Flow_DeleteSubThread(this.FK_Flow, this.WorkID, "手工删除");
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
                        FrmEvents fes = new FrmEvents(this.EnsName); //获得事件.

                        // 处理删除前事件.
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

                        // 处理删除后事件.
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

                    case "Track": //通过一个串来打开一个工作.
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
                        this.Response.Write("<script> window.location.href='" + myurl + "'</script> *^_^*  <br><br>正在进入系统请稍后，如果长时间没有反应，请<a href='" + myurl + "'>点这里进入。</a>");
                        return;
                    case "OF": //通过一个串来打开一个工作.
                        string sid = this.Request.QueryString["SID"];
                        string[] strs = sid.Split('_');
                        GenerWorkerList wl = new GenerWorkerList();
                        int i = wl.Retrieve(GenerWorkerListAttr.FK_Emp, strs[0],
                            GenerWorkerListAttr.WorkID, strs[1],
                            GenerWorkerListAttr.IsPass, 0);

                        if (i == 0)
                        {
                            this.Response.Write("<h2>提示</h2>此工作已经被别人处理或者此流程已删除。");
                            return;
                        }

                        BP.Port.Emp empOF = new BP.Port.Emp(wl.FK_Emp);
                        Web.WebUser.SignInOfGener(empOF);
                        string u = "MyFlow.htm?FK_Flow=" + wl.FK_Flow + "&WorkID=" + wl.WorkID + "&FK_Node=" + wl.FK_Node + "&FID=" + wl.FID;
                        this.Response.Write("<script> window.location.href='" + u + "'</script> *^_^*  <br><br>正在进入系统请稍后，如果长时间没有反应，请<a href='" + u + "'>点这里进入。</a>");
                        return;
                    case "ExitAuth":
                        BP.Port.Emp emp = new BP.Port.Emp(this.FK_Emp);
                        //首先退出，再进行登录
                        BP.Web.WebUser.Exit();
                        BP.Web.WebUser.SignInOfGener(emp, WebUser.SysLang);
                        this.WinClose();
                        return;
                    case "LogAs":
                        BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(this.FK_Emp);
                        if (wfemp.AuthorIsOK == false)
                        {
                            this.WinCloseWithMsg("授权失败");
                            return;
                        }
                        BP.Port.Emp emp1 = new BP.Port.Emp(this.FK_Emp);
                        BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, wfemp.Author, WebUser.Name);
                        this.WinClose();
                        return;
                    case "TakeBack": // 取消授权。
                        BP.WF.Port.WFEmp myau = new BP.WF.Port.WFEmp(WebUser.No);
                        BP.DA.Log.DefaultLogWriteLineInfo("取消授权:" + WebUser.No + "取消了对(" + myau.Author + ")的授权。");
                        myau.Author = "";
                        myau.AuthorWay = 0;
                        myau.Update();
                        this.WinClose();
                        return;
                    case "AutoTo": // 执行授权。
                        BP.WF.Port.WFEmp au = new BP.WF.Port.WFEmp();
                        au.No = WebUser.No;
                        au.RetrieveFromDBSources();
                        au.AuthorDate = BP.DA.DataType.CurrentData;
                        au.Author = this.FK_Emp;
                        au.AuthorWay = 1;
                        au.Save();
                        BP.DA.Log.DefaultLogWriteLineInfo("执行授权:" + WebUser.No + "执行了对(" + au.Author + ")的授权。");
                        this.WinClose();
                        return;
                    case "UnSend": //执行撤消发送。
                        this.Response.Redirect("./WorkOpt/UnSend.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow, false);
                        return;
                    case "SetBillState":
                        break;
                    case "WorkRpt":
                        Bill bk1 = new Bill(this.Request.QueryString["OID"]);
                        Node nd = new Node(bk1.FK_Node);
                        this.Response.Redirect("WFRpt.htm?WorkID=" + bk1.WorkID + "&FID=" + bk1.FID + "&FK_Flow=" + nd.FK_Flow + "&NodeId=" + bk1.FK_Node, false);
                        //this.WinOpen();
                        //this.WinClose();
                        break;
                    case "PrintBill":
                        //Bill bk2 = new Bill(this.Request.QueryString["OID"]);
                        //Node nd2 = new Node(bk2.FK_Node);
                        //this.Response.Redirect("NodeRefFunc.aspx?NodeId=" + bk2.FK_Node + "&FlowNo=" + nd2.FK_Flow + "&NodeRefFuncOID=" + bk2.FK_NodeRefFunc + "&WorkFlowID=" + bk2.WorkID);
                        ////this.WinClose();
                        break;
                    //删除流程中第一个节点的数据，包括待办工作
                    case "DeleteFlow":
                        string fk_flowDel = this.Request.QueryString["FK_Flow"];
                        Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);
                        //调用DoDeleteWorkFlowByReal方法
                        WorkFlow wf = new WorkFlow(new Flow(fk_flowDel), workid);
                        wf.DoDeleteWorkFlowByReal(true);
                        //  Glo.ToMsg("流程删除成功");
                        BP.WF.Glo.ToMsg("流程删除成功");

                        //this.ToWFMsgPage("流程删除成功");
                        break;
                    case "DownFlowSearchExcel":    //下载流程查询结果，转到下面的逻辑，不放在此try..catch..中
                        break;
                    case "DownFlowSearchToTmpExcel":    //导出到模板
                        break;
                    default:
                        throw new Exception("ActionType error" + this.ActionType);
                }
            }
            catch (Exception ex)
            {
                this.ToErrorPage("执行其间如下异常：<BR>" + ex.Message);
            }
            //此处之所以再加一个switch，是因为在下载文件逻辑中，调用Response.End()方法，如果此方法放在try..catch..中，会报线程中止异常
            switch (this.ActionType)
            {
                case "DownFlowSearchExcel":
                    DownMyStartFlowExcel();
                    break;
                case "DownFlowSearchToTmpExcel":    //导出到模板
                    DownMyStartFlowToTmpExcel();
                    break;
            }
        }

        /// <summary>
        /// 导出“我发起的流程”查询结果到设置好的模板excel中，/WF/RptDfine/FlowSearch.htm中的“导出至模板”功能调用
        /// added by liuxc,2017-10-13
        /// </summary>
        public void DownMyStartFlowToTmpExcel()
        {
            string searchType = Request.QueryString["SearchType"];

            if (string.IsNullOrWhiteSpace(this.FK_Flow))
                throw new Exception("@参数FK_Flow不能为空");

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
                throw new Exception(@"" + tmpFile + "模板文件不存在！");
            }

            GEEntitys ges = new GEEntitys(rptNo);
            Entities ens = mdMyRpt.HisEns;
            Entity en = ens.GetNewEntity;
            ens = mdRpt.HisEns; //added by liuxc,2016-12-19,变更为Rpt集合类，这样查询的时候，就可以用MyRpt的查询条件，查询出Rpt实体集合
            QueryObject qo = new QueryObject(ens);

            switch (searchType)
            {
                case "My": //我发起的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);
                    break;
                case "MyDept": //我部门发起的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FK_Dept, WebUser.FK_Dept);
                    break;
                case "MyJoin": //我参与的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");
                    break;
                case "Adminer":
                    break;
                default:
                    throw new Exception("err@" + searchType + "标记错误.");
            }

            qo = new BP.WF.HttpHandler.WF_RptDfine(HttpContext.Current).InitQueryObject(qo, mdMyRpt, ges.GetNewEntity.EnMap.Attrs, attrs, ur);
            qo.DoQuery();

            //获取流程绑定的表单库中的表单信息
            List<string> listFrms = new List<string>(); //存储绑定表单mapdata编号
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
            Dictionary<string, Entities> frmDatas = new Dictionary<string, Entities>(); //存储fk_mapdata,Entities
            Dictionary<string, MapAttrs> frmAttrs = new Dictionary<string, MapAttrs>(); //存储fk_mapdata,MapAttrs
            string oids = GetOidsJoin(ens, "OID", false);

            //获取各绑定表单的记录集合
            frmDatas.Add(fk_mapdata, ens);
            frmAttrs.Add(fk_mapdata, new MapAttrs(fk_mapdata));

            //增加明细表的字段定义
            if (!string.IsNullOrWhiteSpace(dtlNo))
            {
                frmAttrs.Add(dtlNo, new MapAttrs(dtlNo));
            }

            foreach (string frm in listFrms)
            {
                //如果模板中没有涉及该表单的字段绑定信息，则不加载此表单的数据
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

            //获取定义明细表的记录集合
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

                    //垂直方向填充数据时，先将缺少的行数增加上
                    for (int i = sheet.LastRowNum; i < tmp.BeginIdx + ens.Count + dtlRecordCount - 1; i++)
                    {
                        sheet.GetRow(lastRowIdx).CopyRowTo(i + 1);
                    }

                    //生成列
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
                                throw new Exception("未涉及到的数据类型，请检查数据是否正确。");
                        }
                    }

                    for (int i = 0; i < ens.Count; i++)
                    {
                        //添加主表数据
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
                                    throw new Exception("未涉及到的数据类型，请检查数据是否正确。");
                            }
                        }

                        dtData.Rows.Add(dr1);

                        //添加明细表数据
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
                                            //此处需要区别明细表的该字段数据类型是否与主表一致
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
                                        throw new Exception("未涉及到的数据类型，请检查数据是否正确。");
                                }
                            }

                            dtData.Rows.Add(dr1);
                        }
                    }

                    //写入excel单元格值
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
                                    throw new Exception("未涉及到的数据类型，请检查数据是否正确。");
                            }
                        }
                    }

                    //弹出下载
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.Write(ms);
                        byte[] bs = ms.GetBuffer(); //2016-12-17，直接使用ms会报错，所以先将流内容存储出来再使用

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
                throw new Exception("数据导出时出现错误，@错误信息：" + ex.Message);
            }
        }

        /// <summary>
        /// 导出“我发起的流程”查询结果到Excel文件，/WF/RptDfine/FlowSearch.htm中的“导出”功能调用
        /// </summary>
        public void DownMyStartFlowExcel()
        {
            string searchType = Request.QueryString["SearchType"];

            if (string.IsNullOrWhiteSpace(this.FK_Flow))
                throw new Exception("@参数FK_Flow不能为空");

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
                case "My": //我发起的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);
                    break;
                case "MyDept": //我部门发起的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FK_Dept, WebUser.FK_Dept);
                    break;
                case "MyJoin": //我参与的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");
                    break;
                case "Adminer":
                    break;
                default:
                    throw new Exception("err@" + searchType + "标记错误.");
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
                                        myDR[attr.Name] = "否";
                                    else
                                        myDR[attr.Name] = "是";
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
                case "My": //我发起的.
                    name = "我发起的流程";
                    break;
                case "MyDept": //我部门发起的.
                    name = "部门发起的流程";
                    break;
                case "MyJoin": //我参与的.
                    name = "我审批的流程";
                    break;
                case "Adminer":
                    name = "高级查询";
                    break;
                default:
                    break;
            }

            name += "（" + flow.Name + "）";
            string filename = Request.PhysicalApplicationPath + @"\Temp\" + name + "_" + DateTime.Today.ToString("yyyy年MM月dd日") + ".xls";
            CCFlow.WF.Comm.Utilities.NpoiFuncs.DataTableToExcel(myDT, filename, name,
                                                                BP.Web.WebUser.Name, true, true, true);
        }

        /// <summary>
        /// 获取指定字段的拼接字符串形式，用英文逗号相连
        /// </summary>
        /// <param name="ens">实体集合</param>
        /// <param name="field">字段</param>
        /// <param name="isVarchar">值是否是字符</param>
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

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion
    }
}
