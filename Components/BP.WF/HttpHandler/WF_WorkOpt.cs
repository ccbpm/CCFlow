using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using System.IO;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF.Data;
using BP.WF.Template;
using BP.WF.DTS;
using BP.Difference;
using BP.WF.Template.SFlow;
using BP.WF.Template.Frm;
using System.Text;
using System.Threading;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_WorkOpt : DirectoryPageBase
    {
        /// <summary>
        /// 最新的消息
        /// </summary>
        /// <returns></returns>
        public string GenerMsg_Init()
        {
            //生成消息的ID.
            string sql = "";
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle)
            {

                sql = "SELECT RDT FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Flow is not null AND FK_Emp='" + BP.Web.WebUser.No + "' ORDER BY RDT DESC ";
            }
            else
            {
                sql = "SELECT RDT FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Flow !='' AND FK_Emp='" + BP.Web.WebUser.No + "' ORDER BY RDT DESC ";

            }


            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            Hashtable ht = new Hashtable();
            if (dt.Rows.Count == 0)
            {
                ht.Add("ID", 0);
            }
            else
            {
                ht.Add("ID", dt.Rows[0][0].ToString());
            }

            ht.Add("Num", dt.Rows.Count);
            return BP.Tools.Json.ToJson(ht);
        }
        /**
         * 执行定时触发
         * 
         * @return
         * @throws Exception
         */
        public string ccbpmServices()
        {
            //自动节点.
            AutoRun_WhoExeIt dts = new AutoRun_WhoExeIt();
            string msg1 = dts.Do() as string;

            //发起流程.
            AutoRunStratFlows en = new AutoRunStratFlows();
            string msg = en.Do() as string;


            ccbpmServices dts3 = new ccbpmServices();
            string msg3 = dts3.Do() as string;

            return "执行完成。 <br>" + msg + "<br>" + msg1 + " <br>" + msg3;
        }

        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <returns></returns>
        public string ThreadDtl_DelSubThread()
        {
            BP.WF.Dev2Interface.Flow_DeleteSubThread(this.WorkID, "手工删除");
            return "删除成功";
        }


        #region 打印 rtf
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string PrintDoc_Init()
        {
            String billNo = this.GetRequestVal("FK_Bill");
            FrmPrintTemplate templete = new FrmPrintTemplate();
            if (DataType.IsNullOrEmpty(billNo) == false)
            {
                templete.setMyPK(billNo);
                int count = templete.RetrieveFromDBSources();
                if (count == 0)
                    return "err@表单模板的编号[" + billNo + "]数据不存在,请查看设计是否正确";
            }
            String frmID = this.FrmID;
            Node nd = null;
            if (DataType.IsNullOrEmpty(frmID) == true && (this.NodeID != 0 && this.NodeID != 9999))
            {
                nd = new Node(this.NodeID);
                if (nd.HisFormType == NodeFormType.SDKForm || nd.HisFormType == NodeFormType.SelfForm)
                    return "err@SDK表单、嵌入式表单暂时不支持打印功能";

                if (nd.HisFormType == NodeFormType.SheetTree)
                {
                    //获取该节点绑定的表单
                    // 所有表单集合.
                    MapDatas mds = new MapDatas();
                    mds.RetrieveInSQL("SELECT FK_Frm FROM WF_FrmNode WHERE FK_Node=" + this.NodeID + " AND FrmEnableRole !=5");
                    return "info@" + BP.Tools.Json.ToJson(mds.ToDataTableField("dt"));
                }
                frmID = "ND" + this.NodeID;
                if (nd.HisFormType == NodeFormType.FoolForm && DataType.IsNullOrEmpty(frmID) == false)
                {
                    FrmPrintTemplates templetes = new FrmPrintTemplates();
                    templetes.Retrieve(FrmPrintTemplateAttr.FrmID, frmID);
                    if (templetes.Count == 0)
                        return "err@当前节点上没有绑定单据模板。";
                    if (templetes.Count > 1)
                        return templetes.ToJson("dt");
                    templete = templetes[0] as FrmPrintTemplate;
                }
                if (nd.HisFormType == NodeFormType.RefOneFrmTree || nd.HisFlow.FlowDevModel == FlowDevModel.JiJian)
                {
                    frmID = nd.NodeFrmID;
                    MapData md = new MapData(frmID);
                    FrmPrintTemplates templetes = new FrmPrintTemplates();
                    if (md.HisFrmType == FrmType.ChapterFrm)
                    {
                        //如果是章节表单，需要获取当前表单上关联的表单
                        String sql = "SELECT CtrlID From Sys_GroupField Where CtrlType='ChapterFrmLinkFrm' AND FrmID='" + frmID + "'";
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        String val = "'" + frmID + "'";
                        foreach (DataRow dr in dt.Rows)
                        {
                            val += ",'" + dr[0] + "'";
                        }


                        templetes.RetrieveIn(FrmPrintTemplateAttr.FrmID, val);
                    }
                    else
                    {
                        templetes.Retrieve(FrmPrintTemplateAttr.FrmID, frmID);
                    }
                    if (templetes.Count == 0)
                        return "err@当前节点上没有绑定单据模板。";
                    if (templetes.Count > 1)
                        return templetes.ToJson("dt");
                    templete = templetes[0] as FrmPrintTemplate;
                }
            }
            //单据的打印
            if(DataType.IsNullOrEmpty(templete.MyPK) == true)
            {
                FrmPrintTemplates templetes = new FrmPrintTemplates();
                templetes.Retrieve(FrmPrintTemplateAttr.FrmID, frmID);
                if(templetes.Count == 0)
                        return "err@当前节点上没有绑定单据模板。";
                templete = templetes[0] as FrmPrintTemplate;
            }
            String sourceType = this.GetRequestVal("SourceType");
            if (DataType.IsNullOrEmpty(sourceType) == false && sourceType.Equals("Bill"))
                return PrintDoc_FormDoneIt(null, this.WorkID, this.FID, frmID, templete);

            if (nd != null && nd.HisFormType == NodeFormType.RefOneFrmTree)
                return PrintDoc_FormDoneIt(nd, this.WorkID, this.FID, templete.FrmID, templete);
            return PrintDoc_DoneIt(templete.MyPK);

        }
        /// <summary>
        /// 执行打印
        /// </summary>
        /// <returns></returns>
        public string PrintDoc_Done()
        {
            string FrmPrintTemplateNo = this.GetRequestVal("FK_Bill");
            return PrintDoc_DoneIt(FrmPrintTemplateNo);
        }

        /// <summary>
        /// 打印pdf.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public string PrintDoc_DoneIt(string FrmPrintTemplateNo = null)
        {
            Node nd = new Node(this.NodeID);

            if (FrmPrintTemplateNo == null)
                FrmPrintTemplateNo = this.GetRequestVal("FK_Bill");

            FrmPrintTemplate func = new FrmPrintTemplate(FrmPrintTemplateNo);

            //如果不是 FrmPrintTemplateExcel 打印.
            if (func.TemplateFileModel == TemplateFileModel.VSTOForExcel)
                return "url@httpccword://-fromccflow,App=FrmPrintTemplateExcel,TemplateNo=" + func.MyPK + ",WorkID=" + this.WorkID + ",FK_Flow=" + this.FlowNo + ",FK_Node=" + this.NodeID + ",UserNo=" + BP.Web.WebUser.No + ",Token=" + BP.Web.WebUser.Token;

            //如果不是 FrmPrintTemplateWord 打印
            if (func.TemplateFileModel == TemplateFileModel.VSTOForWord)
                return "url@httpccword://-fromccflow,App=FrmPrintTemplateWord,TemplateNo=" + func.MyPK + ",WorkID=" + this.WorkID + ",FK_Flow=" + this.FlowNo + ",FK_Node=" + this.NodeID + ",UserNo=" + BP.Web.WebUser.No + ",Token=" + BP.Web.WebUser.Token;
            if (func.TemplateFileModel == TemplateFileModel.WPS)
                return PrintDoc_WpsWord(nd, this.WorkID, this.FID, func.FrmID, func);


            string billInfo = "";

            string ccformId = this.GetRequestVal("CCFormID");
            if (DataType.IsNullOrEmpty(ccformId) == false)
                return PrintDoc_FormDoneIt(nd, this.WorkID, this.FID, ccformId, func);

            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();

            string file = DateTime.Now.Year + "_" + WebUser.DeptNo + "_" + func.MyPK + "_" + WorkID + ".doc";

            string tempFilePath = func.TempFilePath;
            if (tempFilePath.Contains(".rtf") == false)
                tempFilePath = tempFilePath + ".rtf";
            BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine(tempFilePath);
            string rtfStr = rtf._rtfStr;

            string[] paths;
            string path;
            Int64 newWorkID = 0;
            try
            {
                #region 生成单据
                rtf.HisEns.Clear();
                rtf.EnsDataDtls.Clear();
                if (func.NodeID != 0)
                {
                    //把流程主表数据放入里面去.
                    GEEntity ndxxRpt = new GEEntity("ND" + int.Parse(nd.FlowNo) + "Rpt");
                    try
                    {
                        ndxxRpt.PKVal = this.WorkID;
                        ndxxRpt.Retrieve();

                        newWorkID = this.WorkID;
                    }
                    catch (Exception ex)
                    {
                        if (FID > 0)
                        {
                            ndxxRpt.PKVal = this.FID;
                            ndxxRpt.Retrieve();

                            newWorkID = this.FID;

                            wk = null;
                            wk = nd.HisWork;
                            wk.OID = this.WorkID;
                            wk.RetrieveFromDBSources();
                        }
                        else
                        {
                            path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + "/" + WebUser.DeptNo + "/" + func.MyPK + "/";
                            string msgErr = "@" + string.Format("生成单据失败，请让管理员检查目录设置") + "[" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
                            billInfo += "@<font color=red>" + msgErr + "</font>";
                            throw new Exception(msgErr + "@其它信息:" + ex.Message);
                        }
                    }
                    ndxxRpt.Copy(wk);

                    //把数据赋值给wk. 有可能用户还没有执行流程检查，字段没有同步到 NDxxxRpt.
                    if (ndxxRpt.Row.Count > wk.Row.Count)
                        wk.Row = ndxxRpt.Row;

                    rtf.HisGEEntity = wk;

                    //加入他的明细表.
                    List<Entities> al = wk.GetDtlsDatasOfList();
                    foreach (Entities ens in al)
                        rtf.AddDtlEns(ens);

                    //增加多附件数据
                    FrmAttachments aths = wk.HisFrmAttachments;
                    foreach (FrmAttachment athDesc in aths)
                    {
                        FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
                        if (athDBs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, athDesc.MyPK, FrmAttachmentDBAttr.RefPKVal, newWorkID, "RDT") == 0)
                            continue;

                        rtf.EnsDataAths.Add(athDesc.NoOfObj, athDBs);
                    }

                    //把审核日志表加入里面去. 
                    Paras ps = new Paras();
                    string trackTable = "ND" + int.Parse(nd.FlowNo) + "Track";
                    bool isHaveWriteDB = false;
                    if (DBAccess.IsExitsTableCol(trackTable, "WriteDB") == true)
                        isHaveWriteDB = true;
                    if (isHaveWriteDB == true)
                        ps.SQL = "SELECT MyPK,NDFrom,ActionType,EmpFrom,EmpFromT,RDT,Msg,WriteDB FROM " + trackTable + " WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                    else
                        ps.SQL = "SELECT MyPK,NDFrom,ActionType,EmpFrom,EmpFromT,RDT,Msg FROM " + trackTable + " WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                    ps.Add(TrackAttr.ActionType, (int)ActionType.WorkCheck);
                    ps.Add(TrackAttr.WorkID, newWorkID);
                    DataTable dt = DBAccess.RunSQLReturnTable(ps);
                    if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
                    {
                        dt.Columns[0].ColumnName = "MyPK";
                        dt.Columns[1].ColumnName = "NDFrom";
                        dt.Columns[2].ColumnName = "ActionType";
                        dt.Columns[3].ColumnName = "EmpFrom";
                        dt.Columns[4].ColumnName = "EmpFromT";
                        dt.Columns[5].ColumnName = "RDT";
                        dt.Columns[6].ColumnName = "Msg";
                        if (isHaveWriteDB == true)
                            dt.Columns[7].ColumnName = "WriteDB";
                    }

                    if (DBAccess.IsExitsTableCol(trackTable, "WriteDB") == true)
                    {
                        foreach (DataRow dr in dt.Rows)
                            dr["WriteDB"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", dr[0].ToString(), "WriteDB");
                    }

                    rtf.dtTrack = dt;

                    //获取启用审核组件的节点
                    ps = new Paras();
                    ps.SQL = "SELECT NodeID FROM WF_Node WHERE FWCSta=1 AND FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow";
                    ps.Add(NodeAttr.FK_Flow, nd.FlowNo);
                    rtf.wks = DBAccess.RunSQLReturnTable(ps);
                }

                paths = file.Split('_');
                path = paths[0] + "/" + paths[1] + "/" + paths[2] + "/";
                string fileModelT = "rtf";
                if ((int)func.TemplateFileModel == 1)
                    fileModelT = "word";
                string billUrl = "file@" + fileModelT + "@" + BP.WF.Glo.CCFlowAppPath + "DataUser/Bill/" + path + file;

                if (func.HisPrintFileType == PrintFileType.PDF)
                    billUrl = billUrl.Replace(".doc", ".pdf");

                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + "/" + WebUser.DeptNo + "/" + func.MyPK + "/";
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                string tempFile = func.TempFilePath;
                if (tempFile.Contains(".rtf") == false)
                    tempFile = tempFile + ".rtf";

                //用于扫描打印.
                string qrUrl = SystemConfig.HostURL + "WF/WorkOpt/PrintDocQRGuide.htm?MyPK=" + func.MyPK;
                rtf.MakeDoc(tempFile,
                    path, file, qrUrl);
                #endregion

                #region 转化成pdf.
                if (func.HisPrintFileType == PrintFileType.PDF)
                {
                    string rtfPath = path + file;
                    string pdfPath = rtfPath.Replace(".doc", ".pdf");
                    try
                    {
                        // BP.WF.Glo.Rtf2PDF(rtfPath, pdfPath);
                    }
                    catch (Exception ex)
                    {
                        return "err@" + ex.Message;
                    }
                }
                #endregion

                //在线WebOffice打开
                if (func.PrintOpenModel == PrintOpenModel.WebOffice)
                    return "err@【/WF/WebOffice/PrintOffice.htm】该文件没有重构好,您可以找到旧版本解决，或者自己开发。";
                return billUrl;
            }
            catch (Exception ex)
            {
                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + "/" + WebUser.DeptNo + "/" + func.MyPK + "/";
                string msgErr = "@" + string.Format("生成单据失败，请让管理员检查目录设置") + "[" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
                return "err@<font color=red>" + msgErr + "</font>" + ex.Message;
            }
        }
        #endregion

        public string PrintDoc_FormDoneIt(Node nd, long workID, long fid, string formID, FrmPrintTemplate func)
        {
            Int64 pkval = workID;
            Work wk = null;
            string billInfo = "";
            MapData mapData = new MapData(formID);
            if (nd != null)
            {
                BP.WF.Template.FrmNode fn = new FrmNode();
                fn = new FrmNode(nd.NodeID, formID);
                //先判断解决方案
                if (fn != null && fn.WhoIsPK != WhoIsPK.OID)
                {
                    if (fn.WhoIsPK == WhoIsPK.PWorkID)
                        pkval = this.PWorkID;
                    if (fn.WhoIsPK == WhoIsPK.FID)
                        pkval = fid;
                }
                wk = nd.HisWork;
                wk.OID = pkval;
                wk.RetrieveFromDBSources();
            }
            if (mapData.HisFrmType == FrmType.ChapterFrm)
            {
                return PrintDoc_ChapterFormDoneIt(nd, workID, pkval, formID, func, wk, mapData);
            }
            string file = DateTime.Now.Year + "_" + WebUser.DeptNo + "_" + func.MyPK + "_" + WorkID + ".doc";

            string tempFile = func.TempFilePath;
            if (tempFile.Contains(".rtf") == false)
                tempFile = tempFile + ".rtf";
            BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine(tempFile);
            string rtfStr = rtf._rtfStr;

            string[] paths;
            string path;
            Int64 newWorkID = 0;
            try
            {
                #region 生成单据
                rtf.HisEns.Clear();
                rtf.EnsDataDtls.Clear();
                if (DataType.IsNullOrEmpty(func.FrmID) == false)
                {
                    //把流程主表数据放入里面去.
                    GEEntity ndxxRpt = new GEEntity(formID);
                    try
                    {
                        ndxxRpt.PKVal = pkval;
                        ndxxRpt.Retrieve();

                        newWorkID = pkval;
                    }
                    catch (Exception ex)
                    {
                        if (FID > 0)
                        {
                            ndxxRpt.PKVal = this.FID;
                            ndxxRpt.Retrieve();

                            newWorkID = this.FID;

                            wk = null;
                            wk = nd.HisWork;
                            wk.OID = this.WorkID;
                            wk.RetrieveFromDBSources();
                        }
                        else
                        {
                            path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + "/" + WebUser.DeptNo + "/" + func.MyPK + "/";
                            string msgErr = "@" + string.Format("生成单据失败，请让管理员检查目录设置") + "[" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
                            billInfo += "@<font color=red>" + msgErr + "</font>";
                            throw new Exception(msgErr + "@其它信息:" + ex.Message);
                        }
                    }
                    //ndxxRpt.Copy(wk);

                    //把数据赋值给wk. 有可能用户还没有执行流程检查，字段没有同步到 NDxxxRpt.
                    //if (ndxxRpt.Row.Count > wk.Row.Count)
                    //   wk.Row = ndxxRpt.Row;

                    rtf.HisGEEntity = ndxxRpt;

                    //加入他的明细表.
                    List<Entities> al = new List<Entities>();
                    MapDtls mapdtls = mapData.MapDtls;
                    foreach (MapDtl dtl in mapdtls)
                    {
                        GEDtls dtls1 = new GEDtls(dtl.No);
                        mapData.EnMap.AddDtl(dtls1, "RefPK");
                    }
                    al = mapData.GetDtlsDatasOfList(pkval.ToString());

                    foreach (Entities ens in al)
                        rtf.AddDtlEns(ens);

                    //加入明细附件表
                    foreach (MapDtl dtl in mapdtls)
                    {
                        FrmAttachments athDtls = dtl.FrmAttachments;
                        rtf.AddDtlOfAthEns(athDtls);
                    }

                    //增加多附件数据
                    FrmAttachments aths = mapData.FrmAttachments;
                    foreach (FrmAttachment athDesc in aths)
                    {
                        FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
                        if (athDBs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, athDesc.MyPK, FrmAttachmentDBAttr.RefPKVal, newWorkID, "RDT") == 0)
                            continue;

                        rtf.EnsDataAths.Add(athDesc.NoOfObj, athDBs);
                    }

                    if (nd != null)
                    {
                        //把审核日志表加入里面去.
                        Paras ps = new Paras();
                        string trackTable = "ND" + int.Parse(nd.FlowNo) + "Track";
                        bool isHaveWriteDB = false;
                        if (DBAccess.IsExitsTableCol(trackTable, "WriteDB") == true)
                            isHaveWriteDB = true;
                        if (isHaveWriteDB == true)
                            ps.SQL = "SELECT MyPK,NDFrom,ActionType,EmpFrom,EmpFromT,RDT,Msg,WriteDB FROM " + trackTable + " WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                        else
                            ps.SQL = "SELECT MyPK,NDFrom,ActionType,EmpFrom,EmpFromT,RDT,Msg FROM " + trackTable + " WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                        ps.Add(TrackAttr.ActionType, (int)ActionType.WorkCheck);
                        ps.Add(TrackAttr.WorkID, newWorkID);
                        DataTable dt = DBAccess.RunSQLReturnTable(ps);
                        if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
                        {
                            dt.Columns[0].ColumnName = "MyPK";
                            dt.Columns[1].ColumnName = "NDFrom";
                            dt.Columns[2].ColumnName = "ActionType";
                            dt.Columns[3].ColumnName = "EmpFrom";
                            dt.Columns[4].ColumnName = "EmpFromT";
                            dt.Columns[5].ColumnName = "RDT";
                            dt.Columns[6].ColumnName = "Msg";
                            if (isHaveWriteDB == true)
                                dt.Columns[8].ColumnName = "WriteDB";
                        }

                        if (DBAccess.IsExitsTableCol(trackTable, "WriteDB") == true)
                        {
                            foreach (DataRow dr in dt.Rows)
                                dr["WriteDB"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", dr[0].ToString(), "WriteDB");
                        }

                        rtf.dtTrack = dt;

                        //获取启用审核组件的节点
                        ps = new Paras();
                        ps.SQL = "SELECT NodeID FROM WF_Node WHERE FWCSta=1 AND FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow";
                        ps.Add(NodeAttr.FK_Flow, nd.FlowNo);
                        rtf.wks = DBAccess.RunSQLReturnTable(ps);
                    }
                }

                paths = file.Split('_');
                path = DateTime.Now.Year + "/" + WebUser.DeptNo + "/" + func.MyPK + "/";
                string fileModelT = "rtf";
                if ((int)func.TemplateFileModel == 1)
                    fileModelT = "word";

                string billUrl = "file@" + fileModelT + "@" + BP.WF.Glo.CCFlowAppPath + "DataUser/Bill/" + path + file;

                if (func.HisPrintFileType == PrintFileType.PDF)
                    billUrl = billUrl.Replace(".doc", ".pdf");

                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + "/" + WebUser.DeptNo + "/" + func.MyPK + "/";
                //  path = Server.MapPath(path);
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                //string tempFile = func.TempFilePath;
                //if (tempFile.Contains(".rtf") == false)
                //    tempFile = tempFile + ".rtf";

                //用于扫描打印.
                string qrUrl = SystemConfig.HostURL + "WF/WorkOpt/PrintDocQRGuide.htm?MyPK=" + func.MyPK;
                rtf.MakeDoc(tempFile, path, file, qrUrl);
                #endregion

                #region 转化成pdf.
                if (func.HisPrintFileType == PrintFileType.PDF)
                {
                    string rtfPath = path + file;
                    string pdfPath = rtfPath.Replace(".doc", ".pdf");
                    try
                    {
                        // BP.WF.Glo.Rtf2PDF(rtfPath, pdfPath);
                    }
                    catch (Exception ex)
                    {
                        return "err@" + ex.Message;
                    }
                }
                #endregion

                //在线WebOffice打开
                if (func.PrintOpenModel == PrintOpenModel.WebOffice)
                    return "err@【/WF/WebOffice/PrintOffice.htm】该文件没有重构好,您可以找到旧版本解决，或者自己开发。";
                return billUrl;
            }
            catch (Exception ex)
            {
                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + "/" + WebUser.DeptNo + "/" + func.MyPK + "/";
                string msgErr = "@" + string.Format("生成单据失败，请让管理员检查目录设置") + "[" + BP.WF.Glo.FlowFileBill + "]。err@" + ex.Message + " @File=" + file + " @Path:" + path;
                return "err@<font color=red>" + msgErr + "</font>" + ex.Message;
            }
        }

        public string PrintDoc_ChapterFormDoneIt(Node nd, long workID, long pkval, string formID, FrmPrintTemplate func, Work wk, MapData mapData)
        {
            string billInfo = "";
            string file = DateTime.Now.Year + "_" + WebUser.DeptNo + "_" + func.MyPK + "_" + WorkID + ".doc";
            string tempFile = func.TempFilePath;
            if (tempFile.Contains(".rtf") == false)
                tempFile = tempFile + ".rtf";
            BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine(tempFile);
            string rtfStr = rtf._rtfStr;
            string hidegroup = wk.GetParaString("HideGroup", "");
            string hideAttrs = wk.GetParaString("HideAttrs", "");
            //章节表单且根据当前流程筛选的可显示的章节打印输出
            GroupFields gfs = mapData.GroupFields;
            MapAttrs mapAttrs = mapData.MapAttrs;
            hidegroup = "," + hidegroup + ",";
            hideAttrs = "," + hideAttrs + ",";
            //按照分组处理
            //递归层数
            int recursionCount = 1;
            int idx = 0;
            Dictionary<int, string> numberMap = new Dictionary<int, string>();
            numberMap.Add(1, "一");
            numberMap.Add(2, "二");
            numberMap.Add(3, "三");
            numberMap.Add(4, "四");
            numberMap.Add(5, "五");
            numberMap.Add(6, "六");
            numberMap.Add(7, "七");
            numberMap.Add(8, "八");
            numberMap.Add(9, "九");
            numberMap.Add(10, "十");
            numberMap.Add(11, "十一");
            numberMap.Add(12, "十二");
            numberMap.Add(13, "十三");
            foreach (GroupField gf in gfs)
            {
                if (DataType.IsNullOrEmpty(gf.GetValStringByKey("ParentOID")) == true)
                {
                    if (hidegroup.Contains("," + gf.OID + ",") == false)
                    {
                        if (gf.Lab.Equals("封面") == false)
                            idx++;
                        if (rtfStr.Contains("<Group." + gf.OID + ">") == true)
                        {
                            rtfStr = rtfStr.Replace("<Group." + gf.OID + ">", idx.ToString() + rtf.GetCode(gf.Lab));
                            //递归获取下面的子级
                            rtfStr = GetChildNodes(gf.OID, gfs, rtfStr, hidegroup, idx.ToString(), rtf);
                        }
                    }
                    else
                    {
                        if (rtfStr.Contains("<Group." + gf.OID + ">") == true)
                            rtfStr = GetRtfStr(rtfStr, "<Group." + gf.OID + ">");
                        //递归获取下面的子级
                        rtfStr = GetChildNodes(gf.OID, gfs, rtfStr, hidegroup, idx.ToString(), rtf);
                    }
                }
            }
            rtfStr = GetAttrChildNodes(mapAttrs, rtfStr, hideAttrs, idx.ToString(), rtf);
            rtf._rtfStr = rtfStr;
            string[] paths;
            string path;
            Int64 newWorkID = 0;
            try
            {
                #region 生成单据
                rtf.HisEns.Clear();
                rtf.EnsDataDtls.Clear();
                if (DataType.IsNullOrEmpty(func.FrmID) == false)
                {
                    //把流程主表数据放入里面去.
                    GEEntity ndxxRpt = new GEEntity(formID);
                    try
                    {
                        ndxxRpt.PKVal = pkval;
                        ndxxRpt.Retrieve();
                        newWorkID = pkval;
                    }
                    catch (Exception ex)
                    {
                        if (FID > 0)
                        {
                            ndxxRpt.PKVal = this.FID;
                            ndxxRpt.Retrieve();
                            newWorkID = this.FID;

                            wk = null;
                            wk = nd.HisWork;
                            wk.OID = this.WorkID;
                            wk.RetrieveFromDBSources();
                        }
                        else
                        {
                            path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + "/" + WebUser.DeptNo + "/" + func.MyPK + "/";
                            string msgErr = "@" + string.Format("生成单据失败，请让管理员检查目录设置") + "[" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
                            billInfo += "@<font color=red>" + msgErr + "</font>";
                            throw new Exception(msgErr + "@其它信息:" + ex.Message);
                        }
                    }
                    rtf.HisGEEntity = ndxxRpt;
                    //加入他的明细表.
                    List<Entities> al = new List<Entities>();
                    MapDtls mapdtls = mapData.MapDtls;
                    foreach (MapDtl dtl in mapdtls)
                    {
                        GEDtls dtls1 = new GEDtls(dtl.No);
                        mapData.EnMap.AddDtl(dtls1, "RefPK");
                    }
                    al = mapData.GetDtlsDatasOfList(pkval.ToString());

                    foreach (Entities ens in al)
                        rtf.AddDtlEns(ens);

                    //加入明细附件表
                    foreach (MapDtl dtl in mapdtls)
                    {
                        FrmAttachments athDtls = dtl.FrmAttachments;
                        rtf.AddDtlOfAthEns(athDtls);
                    }

                    //增加多附件数据
                    FrmAttachments aths = mapData.FrmAttachments;
                    foreach (FrmAttachment athDesc in aths)
                    {
                        FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
                        if (athDBs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, athDesc.MyPK, FrmAttachmentDBAttr.RefPKVal, newWorkID, "RDT") == 0)
                        {
                            rtfStr = GetRtfStr(rtfStr, "<Ath." + athDesc.NoOfObj + ".ImgAth>");
                            rtfStr = GetRtfStr(rtfStr, "<Ath." + athDesc.NoOfObj + ">");
                            continue;
                        }
                        rtf.EnsDataAths.Add(athDesc.NoOfObj, athDBs);
                    }
                    rtf._rtfStr = rtfStr;

                    if (nd != null)
                    {
                        //把审核日志表加入里面去.
                        Paras ps = new Paras();
                        string trackTable = "ND" + int.Parse(nd.FlowNo) + "Track";
                        bool isHaveWriteDB = false;
                        if (DBAccess.IsExitsTableCol(trackTable, "WriteDB") == true)
                            isHaveWriteDB = true;
                        if (isHaveWriteDB == true)
                            ps.SQL = "SELECT MyPK,NDFrom,ActionType,EmpFrom,EmpFromT,RDT,Msg,WriteDB FROM " + trackTable + " WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                        else
                            ps.SQL = "SELECT MyPK,NDFrom,ActionType,EmpFrom,EmpFromT,RDT,Msg FROM " + trackTable + " WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                        ps.Add(TrackAttr.ActionType, (int)ActionType.WorkCheck);
                        ps.Add(TrackAttr.WorkID, newWorkID);
                        DataTable dt = DBAccess.RunSQLReturnTable(ps);
                        if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
                        {
                            dt.Columns[0].ColumnName = "MyPK";
                            dt.Columns[1].ColumnName = "NDFrom";
                            dt.Columns[2].ColumnName = "ActionType";
                            dt.Columns[3].ColumnName = "EmpFrom";
                            dt.Columns[4].ColumnName = "EmpFromT";
                            dt.Columns[5].ColumnName = "RDT";
                            dt.Columns[6].ColumnName = "Msg";
                            if (isHaveWriteDB == true)
                                dt.Columns[8].ColumnName = "WriteDB";
                        }

                        if (DBAccess.IsExitsTableCol(trackTable, "WriteDB") == true)
                        {
                            foreach (DataRow dr in dt.Rows)
                                dr["WriteDB"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", dr[0].ToString(), "WriteDB");
                        }

                        rtf.dtTrack = dt;

                        //获取启用审核组件的节点
                        ps = new Paras();
                        ps.SQL = "SELECT NodeID FROM WF_Node WHERE FWCSta=1 AND FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow";
                        ps.Add(NodeAttr.FK_Flow, nd.FlowNo);
                        rtf.wks = DBAccess.RunSQLReturnTable(ps);
                    }
                }

                paths = file.Split('_');
                path = paths[0] + "/" + paths[1] + "/" + paths[2] + "/";
                string fileModelT = "rtf";
                if ((int)func.TemplateFileModel == 1)
                    fileModelT = "word";

                string billUrl = "url@" + fileModelT + "@" + BP.WF.Glo.CCFlowAppPath + "DataUser/Bill/" + path + file;

                if (func.HisPrintFileType == PrintFileType.PDF)
                    billUrl = billUrl.Replace(".doc", ".pdf");

                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + "/" + WebUser.DeptNo + "/" + func.MyPK + "/";
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);



                //用于扫描打印.
                string qrUrl = SystemConfig.HostURL + "WF/WorkOpt/PrintDocQRGuide.htm?MyPK=" + func.MyPK;
                rtf.MakeDoc(tempFile, path, file, qrUrl);
                #endregion

                #region 转化成pdf.
                if (func.HisPrintFileType == PrintFileType.PDF)
                {
                    string rtfPath = path + file;
                    string pdfPath = rtfPath.Replace(".doc", ".pdf");
                    try
                    {
                        // BP.WF.Glo.Rtf2PDF(rtfPath, pdfPath);
                    }
                    catch (Exception ex)
                    {
                        return "err@" + ex.Message;
                    }
                }
                #endregion

                //在线WebOffice打开
                if (func.PrintOpenModel == PrintOpenModel.WebOffice)
                    return "err@【/WF/WebOffice/PrintOffice.htm】该文件没有重构好,您可以找到旧版本解决，或者自己开发。";
                return billUrl;
            }
            catch (Exception ex)
            {
                path = BP.WF.Glo.FlowFileBill + DateTime.Now.Year + "/" + WebUser.DeptNo + "/" + func.MyPK + "/";
                string msgErr = "@" + string.Format("生成单据失败，请让管理员检查目录设置") + "[" + BP.WF.Glo.FlowFileBill + "]。err@" + ex.Message + " @File=" + file + " @Path:" + path;
                return "err@<font color=red>" + msgErr + "</font>" + ex.Message;
            }
        }
        /// <summary>
        /// Wps打印Word文档
        /// </summary>
        /// <param name="nd"></param>
        /// <param name="workID"></param>
        /// <param name="fid"></param>
        /// <param name="formID"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public string PrintDoc_WpsWord(Node nd, long workID, long fid, string formID, FrmPrintTemplate func)
        {
            DataSet ds = new DataSet();
            DataTable dt = func.ToDataTableField("Sys_FrmPrintTemplate");
            ds.Tables.Add(dt);
            Int64 pkval = workID;
            Work wk = null;
            string billInfo = "";
            if (nd != null)
            {
                BP.WF.Template.FrmNode fn = new FrmNode(nd.NodeID, formID);
                //先判断解决方案
                if (fn != null && fn.WhoIsPK != WhoIsPK.OID)
                {
                    if (fn.WhoIsPK == WhoIsPK.PWorkID)
                        pkval = this.PWorkID;
                    if (fn.WhoIsPK == WhoIsPK.FID)
                        pkval = fid;
                }
                wk = nd.HisWork;
                wk.OID = this.WorkID;
                wk.RetrieveFromDBSources();
            }
            MapData mapData = new MapData(formID);
            Int64 newWorkID = 0;

            if (DataType.IsNullOrEmpty(func.FrmID) == false)
            {
                //把流程主表数据放入里面去.
                GEEntity ndxxRpt = new GEEntity(formID);
                try
                {
                    ndxxRpt.PKVal = pkval;
                    ndxxRpt.Retrieve();

                    newWorkID = pkval;
                }
                catch (Exception ex)
                {
                    if (FID > 0)
                    {
                        ndxxRpt.PKVal = this.FID;
                        ndxxRpt.Retrieve();

                        newWorkID = this.FID;

                        wk = null;
                        wk = nd.HisWork;
                        wk.OID = this.WorkID;
                        wk.RetrieveFromDBSources();
                    }
                }
                //加入主表信息
                dt = ndxxRpt.ToDataTableField("MainTable");
                ds.Tables.Add(dt);
                dt = mapData.MapAttrs.ToDataTableField("Sys_MapAttr");
                ds.Tables.Add(dt);

                //加入他的明细表.
                MapDtls dtls = mapData.MapDtls;
                dt = dtls.ToDataTableField("Sys_MapDtl");
                ds.Tables.Add(dt);
                foreach (MapDtl dtl in dtls)
                {
                    GEDtls dtls1 = new GEDtls(dtl.No);
                    EnDtl endtl = new EnDtl();
                    endtl.Ens = dtls1;
                    endtl.RefKey = "RefPK";
                    dt = dtl.GetDtlEnsDa(endtl, pkval.ToString()).ToDataTableField(dtl.No);
                    ds.Tables.Add(dt);
                }

                //增加多附件数据
                FrmAttachments aths = mapData.FrmAttachments;
                DataTable athdt = aths.ToDataTableField("Sys_FrmAttachment");
                ds.Tables.Add(athdt);
                foreach (FrmAttachment athDesc in aths)
                {
                    FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
                    if (athDBs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, athDesc.MyPK, FrmAttachmentDBAttr.RefPKVal, newWorkID, "RDT") == 0)
                        continue;
                    dt = athDBs.ToDataTableField(athDesc.MyPK);
                    ds.Tables.Add(dt);
                }

                if (nd != null)
                {
                    //把审核日志表加入里面去.
                    Paras ps = new Paras();
                    ps.SQL = "SELECT * FROM ND" + int.Parse(nd.FlowNo) + "Track WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                    ps.Add(TrackAttr.ActionType, (int)ActionType.WorkCheck);
                    ps.Add(TrackAttr.WorkID, newWorkID);

                    dt = DBAccess.RunSQLReturnTable(ps);
                    dt.TableName = "WF_Track";
                    ds.Tables.Add(dt);
                }
            }
            return BP.Tools.Json.ToJson(ds);
        }
        private string GetChildNodes(int parentID, GroupFields gfs, string rtfStr, string hidegroup, string parentIdx, BP.Pub.RTFEngine rtf)
        {
            int idx = 0;
            foreach (GroupField gf in gfs)
            {
                if (gf.GetValIntByKey("ParentOID") == parentID)
                {

                    if (hidegroup.Contains("," + gf.OID + ",") == false)
                    {
                        idx++;
                        if (rtfStr.Contains("<Group." + gf.OID + ">") == true)
                        {
                            rtfStr = rtfStr.Replace("<Group." + gf.OID + ">", parentIdx + "." + idx.ToString() + rtf.GetCode(gf.Lab));
                            //处理
                            rtfStr = GetChildNodes(gf.OID, gfs, rtfStr, hidegroup, parentIdx + "." + idx.ToString(), rtf);
                        }
                    }
                    else
                    {
                        //移除这个分组标签
                        if (rtfStr.Contains("<Group." + gf.OID + ">") == true)
                            rtfStr = GetRtfStr(rtfStr, "<Group." + gf.OID + ">");
                        //递归获取下面的子级
                        rtfStr = GetChildNodes(gf.OID, gfs, rtfStr, hidegroup, parentIdx + "." + idx, rtf);
                    }

                    //移除从表
                    if (gf.CtrlType.Equals("Dtl") == true && rtfStr.Contains("<" + gf.CtrlID + ".Begin>") == true)
                    {
                        if (hidegroup.Contains("," + gf.OID + ",") == true)
                        {
                            int pos_rowKey = rtfStr.IndexOf("<" + gf.CtrlID + ".Begin>") + 1;
                            int row_start = -1, row_end = -1;
                            row_start = rtfStr.Substring(0, pos_rowKey).LastIndexOf("\\pard");
                            pos_rowKey = rtfStr.IndexOf("<" + gf.CtrlID + ".End>") + 1;
                            row_end = rtfStr.Substring(pos_rowKey).IndexOf("\\par") + 6;
                            if (row_start != -1 && row_end != -1)
                                rtfStr = rtfStr.Substring(0, row_start) + rtfStr.Substring(pos_rowKey).Substring(row_end);
                        }
                        else
                        {
                            //移除这个分组标签
                            rtfStr = GetRtfStr(rtfStr, "<" + gf.CtrlID + ".Begin>");
                            rtfStr = GetRtfStr(rtfStr, "<" + gf.CtrlID + ".End>");
                        }


                    }
                    //移除附件
                    if (gf.CtrlType.Equals("Ath") == true && hidegroup.Contains("," + gf.OID + ",") == true)
                    {
                        string athObj = gf.CtrlID.Replace(gf.FrmID + "_", "");
                        if (rtfStr.Contains("<Ath." + athObj + ".") == true)
                        {
                            rtfStr = GetRtfStr(rtfStr, "<Ath." + athObj + ".ImgAth>");
                            rtfStr = GetRtfStr(rtfStr, "<Ath." + athObj + ">");
                        }

                    }
                }
            }
            return rtfStr;
        }
        private string GetRtfStr(string rtfStr, string tag)
        {
            //移除这个分组标签
            if (tag.Contains(".End"))
            {
                return rtfStr.Replace(tag, "");
            }

            int pos_rowKey = rtfStr.IndexOf(tag) + 1;
            int row_start = -1, row_end = -1;
            if (pos_rowKey > 0)
            {
                row_start = rtfStr.Substring(0, pos_rowKey).LastIndexOf("\\pard");
                //int idx1 = rtfStr.Substring(pos_rowKey).IndexOf("\\par \\ltrrow");
                int idx2 = rtfStr.Substring(pos_rowKey).IndexOf("\\par");
                row_end = idx2 + 4;
                /*if (idx2 != -1)
                {
                    if(idx1 == idx2)
                        row_end = idx1 + 13;
                    if(idx2<idx1)
                        row_end = idx2 + 6;
                }*/
            }
            if (row_start != -1 && row_end != -1)
                rtfStr = rtfStr.Substring(0, row_start) + rtfStr.Substring(pos_rowKey).Substring(row_end);
            return rtfStr;
        }
        private string GetAttrChildNodes(MapAttrs mapAttrs, string rtfStr, string hideAttrs, string parentIdx, BP.Pub.RTFEngine rtf)
        {
            foreach (MapAttr mapAttr in mapAttrs)
            {
                if (rtfStr.Contains("<" + mapAttr.KeyOfEn + ">") == true)
                {
                    if (hideAttrs.Contains("," + mapAttr.KeyOfEn + ",") == true)
                    {
                        //移除这个分组标签
                        rtfStr = GetRtfStr(rtfStr, "<" + mapAttr.KeyOfEn + ">");
                        //rtfStr = rtfStr.Replace("<" + mapAttr.KeyOfEn + ">", "");
                    }
                }
            }
            return rtfStr;
        }
        /// <summary>
        /// 执行挂起
        /// </summary>
        /// <returns></returns>
        public string Hungup_Save()
        {
            int delayWay = this.GetRequestValInt("DelayWay");
            string strRelDate = this.GetRequestVal("RelDate");
            string relDate;

            if (delayWay == 1)
            {
                if (DataType.IsNullOrEmpty(strRelDate))
                    return "err@截止时间不可以为空";

                DateTime dtimeRelDate = DateTime.Parse(strRelDate);
                if (dtimeRelDate < DateTime.Now)
                    return "err@截止时间不可以小于当前时间";

                relDate = DataType.SysDateTimeFormat(dtimeRelDate);
            }
            else
                relDate = "";

            BP.WF.Dev2Interface.Node_HungupWork(this.WorkID, delayWay,
                relDate, this.GetRequestVal("Doc"));
            return "挂起成功.";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_WorkOpt()
        {
        }
        /// <summary>
        /// 打包下载
        /// </summary>
        /// <returns></returns>
        public string Packup_Init()
        {
            try
            {
                string sourceType = this.GetRequestVal("SourceType");
                //打印单据实体、单据表单
                if (DataType.IsNullOrEmpty(sourceType) == false && sourceType.Equals("Bill"))
                {
                    return MakeForm2Html.MakeBillToPDF(this.GetRequestVal("FrmID"), this.WorkID, null, null);
                }
                int nodeID = this.NodeID;
                if (this.NodeID == 0)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                    nodeID = gwf.NodeID;
                }

                Node nd = new Node(nodeID);
                //树形表单方案单独打印
                if ((nd.HisFormType == NodeFormType.SheetTree && nd.HisPrintPDFModle == 1))
                {
                    //获取该节点绑定的表单
                    // 所有表单集合.
                    MapDatas mds = new MapDatas();
                    mds.RetrieveInSQL("SELECT FK_Frm FROM WF_FrmNode WHERE FK_Node=" + this.NodeID + " AND FrmEnableRole != 5");
                    return "info@" + BP.Tools.Json.ToJson(mds.ToDataTableField());
                }
                return BP.WF.MakeForm2Html.MakeCCFormToPDF(nd, this.WorkID, this.FlowNo, null, null);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /**
	     * 独立表单PDF打印
	     * @return
	     * @throws Exception 
	     */
        public String Packup_FromInit()
        {
            try
            {
                int nodeID = this.NodeID;
                if (this.NodeID == 0)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                    nodeID = gwf.NodeID;
                }
                Node nd = new Node(nodeID);
                return BP.WF.MakeForm2Html.MakeFormToPDF(this.GetRequestVal("FrmID"), this.GetRequestVal("FrmName"), nd, this.WorkID, this.FlowNo, null, false, this.GetRequestVal("BasePath"));
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 扫描二维码获得文件.
        /// </summary>
        /// <returns></returns>
        public string PrintDocQRGuide_Init()
        {
            try
            {
                int nodeID = this.NodeID;
                if (this.NodeID == 0)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                    nodeID = gwf.NodeID;
                }

                Node nd = new Node(nodeID);
                Work wk = nd.HisWork;
                return BP.WF.MakeForm2Html.MakeCCFormToPDF(nd, this.WorkID, this.FlowNo, null, null);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 选择表单,发起前置导航.
        /// </summary>
        /// <returns></returns>
        public string StartGuideFrms_Init()
        {
            string sql = "SELECT No,Name From Sys_MapData A,WF_FrmNode B WHERE A.No=B.FK_Frm AND B.FK_Node=" + SystemConfig.AppCenterDBVarStr
                    + "FK_Node AND FrmEnableRole=" + (int)BP.WF.Template.FrmEnableRole.WhenHaveFrmPara;
            Paras ps = new Paras();
            ps.SQL = sql;
            ps.Add(FrmNodeAttr.FK_Node, int.Parse(this.FlowNo + "01"));
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            dt.TableName = "Frms";
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            Flow flow = new Flow(this.FlowNo);
            ds.Tables.Add(flow.ToDataTableField("WF_Flow"));
            return BP.Tools.Json.ToJson(ds);
        }

        #region 公文处理.
        /// <summary>
        /// 直接下载
        /// </summary>
        /// <returns></returns>
        public string DocWord_OpenByHttp()
        {
            string DocName = this.GetRequestVal("DocName");//获取上传的公文模板名称
            //生成文件.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Flow fl = new Flow(this.FlowNo);

            string file = SystemConfig.PathOfTemp + "/" + DocName;
            DBAccess.GetFileFromDB(file, fl.PTable, "OID", this.WorkID.ToString(), "DocWordFile");
            return "../../DataUser/Temp/" + DocName;

        }
        /// <summary>
        /// 重置公文文件.
        /// </summary>
        /// <returns></returns>
        public string DocWord_ResetFile()
        {
            Flow fl = new Flow(this.FlowNo);
            string sql = "UPDATE " + fl.PTable + " SET DocWordFile=NULL WHERE OID=" + this.WorkID;
            DBAccess.RunSQL(sql);
            return "重新生成模版成功.";
        }
        /// <summary>
        /// 生成文件模版
        /// </summary>
        /// <returns></returns>
        public string DocWord_Init()
        {
            BtnLab lab = new BtnLab(this.NodeID);
            if (lab.OfficeBtnEnableInt == 0)
                return "err@当前节点没有启用公文.";

            //首先判断是否生成公文文件？ todo. 
            Flow fl = new Flow(this.FlowNo);
            byte[] val = DBAccess.GetByteFromDB(fl.PTable, "OID", this.WorkID.ToString(), "DocWordFile");
            if (val != null)
                return "info@OfficeBtnEnable=" + lab.OfficeBtnEnableInt.ToString() + ";请下载文件"; //如果已经有这个模版了.

            DocTemplate en = new DocTemplate();
            //求出要生成的模版.
            DocTemplates ens = new DocTemplates();
            ens.Retrieve(DocTemplateAttr.FK_Node, this.NodeID);
            if (ens.Count > 1)
                return "url@DocWordSelectDocTemp.htm";

            //如果没有模版就给他一个默认的模版.
            if (ens.Count == 0)
                en.FilePath = SystemConfig.PathOfDataUser + "DocTemplete/Default.docx";

            if (ens.Count == 1)
                en = ens[0] as DocTemplate;

#warning 替换变量. todo.

            DBAccess.SaveBytesToDB(en.FileBytes, fl.PTable, "OID", this.WorkID, "DocWordFile");
            return "info@OfficeBtnEnable=" + lab.OfficeBtnEnableInt.ToString() + ";请下载文件"; //如果已经有这个模版了.
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <returns></returns>
        public string DocWord_Upload()
        {
            if (HttpContextHelper.RequestFilesCount == 0)
                return "err@请上传模版.";

            //上传附件
            //HttpPostedFile file = HttpContextHelper.RequestFiles(0);
            string fileName = HttpContextHelper.GetNameByIdx(0);
            string path = SystemConfig.PathOfTemp + DBAccess.GenerGUID() + ".docx";

            HttpContextHelper.UploadFile(HttpContextHelper.RequestFiles(0), path);

            //插入模版.
            DocTemplate dt = new DocTemplate();
            dt.NodeID = this.NodeID;
            dt.No = DBAccess.GenerGUID();
            dt.Name = fileName;
            dt.FilePath = path; //路径
            dt.NodeID = this.NodeID;
            dt.Insert();

            Flow fl = new Flow(this.FlowNo);
            DBAccess.SaveFileToDB(path, fl.PTable, "OID", this.WorkID.ToString(), "DocWordFile");

            return "上传成功.";
        }

        /// <summary>
        /// 选择一个模版
        /// </summary>
        /// <returns></returns>
        public string DocWordSelectDocTemp_Imp()
        {
            Node node = new Node(this.NodeID);
            if (node.ItIsStartNode == false)
                return "err@不是开始节点不可以执行模板导入.";

            DocTemplate docTemplate = new DocTemplate(this.No);
            if (File.Exists(docTemplate.FilePath) == false)
                return "err@选择的模版文件不存在,请联系管理员.";

            byte[] bytes = DataType.ConvertFileToByte(docTemplate.FilePath);
            Flow fl = new Flow(this.FlowNo);
            DBAccess.SaveBytesToDB(bytes, fl.PTable, "OID", this.WorkID, "DocWordFile");
            return "模板导入成功.";
        }
        #endregion

        #region 通用人员选择器.
        public string AccepterOfGener_Init()
        {
            Entities ens = AccepterOfGener_Init_Ext();
            return ens.ToJson();
        }
        public string AccepterOfGener_Init_TS()
        {
            Entities ens = AccepterOfGener_Init_Ext();

            return ens.ToJson();
        }
        /// <summary>
        /// 通用人员选择器Init
        /// </summary>
        /// <returns></returns>
        public SelectAccpers AccepterOfGener_Init_Ext()
        {
            /* 获得上一次发送的人员列表. */
            int toNodeID = this.GetRequestValInt("ToNode");
            Selector selector = new Selector(toNodeID);


            //查询出来,已经选择的人员.
            SelectAccpers sas = new SelectAccpers();
            int i = sas.Retrieve(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID,
                this.WorkID, SelectAccperAttr.Idx);
            if (selector.ItIsAutoLoadEmps == false)
                return sas;
            if (i == 0)
            {
                //获得最近的一个workid.
                string trackTable = "ND" + int.Parse(this.FlowNo) + "Track";
                Paras ps = new Paras();
                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                {
                    ps.SQL = "SELECT TOP 1 Tag,EmpTo FROM " + trackTable + " WHERE NDTo=" + SystemConfig.AppCenterDBVarStr + "NDTo AND (ActionType=0 OR ActionType=1) AND EmpFrom=" + SystemConfig.AppCenterDBVarStr + "EmpFrom ORDER BY WorkID desc  ";
                    ps.Add("NDTo", toNodeID);
                    ps.Add("EmpFrom", WebUser.No);
                }
                else if (SystemConfig.AppCenterDBType == DBType.DM ||
                    SystemConfig.AppCenterDBType == DBType.Oracle
                    || SystemConfig.AppCenterDBType == DBType.KingBaseR3
                    || SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                {
                    ps.SQL = "SELECT * FROM (SELECT  Tag,EmpTo,WorkID FROM " + trackTable + " A WHERE A.EmpFrom=" + SystemConfig.AppCenterDBVarStr + "EmpFrom AND A.NDFrom=" + SystemConfig.AppCenterDBVarStr + "NDFrom AND A.NDTo=" + SystemConfig.AppCenterDBVarStr + "NDTo AND (ActionType=0 OR ActionType=1) AND EmpFrom=" + SystemConfig.AppCenterDBVarStr + "EmpFrom ORDER BY WorkID DESC ) WHERE ROWNUM =1";
                    ps.Add("EmpFrom", BP.Web.WebUser.No);
                    ps.Add("NDFrom", this.NodeID);
                    ps.Add("NDTo", toNodeID);
                }
                else if (SystemConfig.AppCenterDBType == DBType.MySQL)
                {
                    ps.SQL = "SELECT Tag,EmpTo FROM " + trackTable + " A WHERE A.NDFrom=" + SystemConfig.AppCenterDBVarStr + "NDFrom AND A.NDTo=" + SystemConfig.AppCenterDBVarStr + "NDTo AND (ActionType=0 OR ActionType=1) AND EmpFrom=" + SystemConfig.AppCenterDBVarStr + "EmpFrom ORDER BY WorkID  DESC limit 1,1 ";
                    ps.Add("NDFrom", this.NodeID);
                    ps.Add("NDTo", toNodeID);
                    ps.Add("EmpFrom", WebUser.No);
                }
                else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || SystemConfig.AppCenterDBType == DBType.UX)
                {
                    ps.SQL = "SELECT Tag,EmpTo FROM " + trackTable + " A WHERE A.NDFrom=:NDFrom AND A.NDTo=:NDTo AND (ActionType=0 OR ActionType=1) AND EmpFrom=:EmpFrom ORDER BY WorkID  DESC limit 1 ";
                    ps.Add("NDFrom", this.NodeID);
                    ps.Add("NDTo", toNodeID);
                    ps.Add("EmpFrom", WebUser.No);
                }
                else
                {
                    //ps.SQL = "SELECT Tag, EmpTo FROM " + trackTable + " WHERE NDTo=" + SystemConfig.AppCenterDBVarStr + "NDTo AND (ActionType=0 OR ActionType=1) AND EmpFrom=" + SystemConfig.AppCenterDBVarStr + "EmpFrom ORDER BY WorkID DESC  ";
                    //ps.Add("NDTo", toNodeID);
                    //ps.Add("EmpFrom", WebUser.No);
                    throw new Exception("err@没有判断的数据库类型....");
                }

                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count != 0)
                {
                    string tag = dt.Rows[0]["Tag"].ToString();
                    string emps = dt.Rows[0]["EmpTo"].ToString();
                    if (tag != null && tag.Contains("EmpsAccepter=") == true)
                    {
                        string[] strs = tag.Split('@');
                        foreach (string str in strs)
                        {
                            if (str == null || str == "" || str.Contains("EmpsAccepter=") == false)
                                continue;
                            string[] mystr = str.Split('=');
                            if (mystr.Length == 2)
                            {
                                emps = mystr[1];
                            }
                        }
                    }

                    if (emps.Contains(",") == true)
                    {
                        if (emps.Contains("'") == true)
                        {
                            string[] strs = emps.Split(';');
                            foreach (string str in strs)
                            {
                                string[] emp = str.Split(',');
                                string empNo = emp[0];
                                BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, empNo, false);
                            }
                        }
                    }
                    else
                    {
                        BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, emps, false);
                    }
                }

                if (dt.Rows.Count != 0)
                    sas.Retrieve(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID, this.WorkID);
            }
            //判断人员是否已经删除
            //if (sas.Count != 0)
            //{
            //    for (int k = sas.Count - 1; k >= 0; k--)
            //    {
            //        SelectAccper sa = sas[k] as SelectAccper;
            //        Emp emp = new Emp(sa.FK_Emp);
            //        if (emp.DeptNo == "")
            //        {
            //            sas.RemoveEn(sa);
            //            sa.Delete();
            //        }

            //    }
            //}
            List<string> deleteItems = new List<string>();
            if (sas.Count > 0)
            {
                for (int k = sas.Count - 1; k >= 0; k--)
                {
                    SelectAccper sa = sas[k] as SelectAccper;
                    if (sa.DeptNo == "")
                    {
                        sas.RemoveEn(sa);
                        deleteItems.Add(sa.DeptNo);
                    }
                }
            }
            if (deleteItems.Count > 0)
            {
                new Thread(() =>
                {
                    SelectAccpers accepters = new SelectAccpers();
                    accepters.Delete("FK_Dept", "");
                }).Start();
            }
            return sas;
        }
        /// <summary>
        /// 增加接收人.
        /// </summary>
        /// <returns></returns>
        public string AccepterOfGener_AddEmps()
        {
            try
            {
                //到达的节点ID.
                int toNodeID = this.GetRequestValInt("ToNode");
                string emps = this.GetRequestVal("AddEmps");

                //增加到里面去.
                BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, emps, false);

                //查询出来,已经选择的人员.
                SelectAccpers sas = new SelectAccpers();
                sas.Retrieve(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID,
                    this.WorkID, SelectAccperAttr.Idx);

                return sas.ToJson();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("INSERT") == true)
                    return "err@人员名称重复,导致部分人员插入失败.";

                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行发送.
        /// </summary>
        /// <returns></returns>
        public string AccepterOfGener_Send()
        {
            try
            {
                int toNodeID = this.GetRequestValInt("ToNode");
                if (toNodeID != 0) //排除协作模式下的会签
                {
                    Node nd = new Node(toNodeID);
                    if (nd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        /* 仅仅设置一个,检查压入的人员个数.*/
                        Paras ps = new Paras();
                        ps.SQL = "SELECT count(WorkID) as Num FROM WF_SelectAccper WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND AccType=0";
                        ps.Add("FK_Node", toNodeID);
                        ps.Add("WorkID", this.WorkID);
                        int num = DBAccess.RunSQLReturnValInt(ps, 0);
                        if (num == 0)
                            return "err@请指定下一步工作的处理人.";
                        Selector sr = new Selector(toNodeID);
                        if (sr.ItIsSimpleSelector == true)
                        {
                            if (num != 1)
                                return "err@您只能选择一个接受人,请移除其他的接受人然后执行发送.";
                        }
                    }
                }

                SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FlowNo, this.WorkID, toNodeID, null);
                string strs = objs.ToMsgOfHtml();
                strs = strs.Replace("@", "<br>@");
                #region 处理发送后转向.
                //当前节点.
                Node currNode = new Node(this.NodeID);
                /*处理转向问题.*/
                switch (currNode.HisTurnToDeal)
                {
                    case TurnToDeal.SpecUrl:
                        string myurl = currNode.TurnToDealDoc.Clone().ToString();
                        if (myurl.Contains("?") == false)
                            myurl += "?1=1";
                        Attrs myattrs = currNode.HisWork.EnMap.Attrs;
                        Work hisWK = currNode.HisWork;
                        foreach (Attr attr in myattrs)
                        {
                            if (myurl.Contains("@") == false)
                                break;
                            myurl = myurl.Replace("@" + attr.Key, hisWK.GetValStrByKey(attr.Key));
                        }
                        myurl = myurl.Replace("@WebUser.No", BP.Web.WebUser.No);
                        myurl = myurl.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        myurl = myurl.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);

                        if (myurl.Contains("@"))
                        {
                            BP.WF.Dev2Interface.Port_SendMsg("admin", currNode.Name + "在" + currNode.Name + "节点处，出现错误", "流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl, "Err" + currNode.No + "_" + this.WorkID, SMSMsgType.Err, this.FlowNo, this.NodeID, this.WorkID, this.FID);
                            throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl);
                        }

                        if (myurl.Contains("PWorkID") == false)
                            myurl += "&PWorkID=" + this.WorkID;

                        myurl += "&FromFlow=" + this.FlowNo + "&FromNode=" + this.NodeID + "&UserNo=" + WebUser.No + "&Token=" + WebUser.Token;
                        return "TurnUrl@" + myurl;

                    case TurnToDeal.TurnToByCond:

                        return strs;
                    default:
                        strs = strs.Replace("@WebUser.No", BP.Web.WebUser.No);
                        strs = strs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        strs = strs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);
                        return strs;
                }
                #endregion
                return strs;
            }
            catch (Exception ex)
            {

                return "err@" + ex.Message;
            }
        }
        #endregion

        // 查询select集合
        public string AccepterOfGener_SelectEmps()
        {
            string sql = "";
            string emp = this.GetRequestVal("TB_Emps");

            #region 保障查询语句的安全.
            emp = emp.ToLower();
            emp = emp.Replace("'", "");
            emp = emp.Replace("&", "&amp");
            emp = emp.Replace("<", "&lt");
            emp = emp.Replace(">", "&gt");
            emp = emp.Replace("delete", "");
            emp = emp.Replace("update", "");
            emp = emp.Replace("insert", "");
            #endregion 保障查询语句的安全.

            bool isPinYin = DBAccess.IsExitsTableCol("Port_Emp", "PinYin");
            if (isPinYin == true)
            {
                //标识结束，不要like名字了.
                if (emp.Contains("/"))
                {
                    //if (SystemConfig.CustomerNo == "TianYe") // 只改了oracle的
                    //{
                    //    //string endSql = "";
                    //    //if (BP.Web.WebUser.DeptNo.IndexOf("18099") == 0)
                    //    //    endSql = " AND B.No LIKE '18099%' ";
                    //    //else
                    //    //    endSql = " AND B.No NOT LIKE '18099%' ";

                    //    string specFlowNos = SystemConfig.AppSettings["SpecFlowNosForAccpter"];
                    //    if (DataType.IsNullOrEmpty(specFlowNos) == true)
                    //        specFlowNos = ",001,";

                    //    string specEmpNos = "";
                    //    if (specFlowNos.Contains(this.NodeID.ToString() + ",") == false)
                    //        specEmpNos = " AND a.No!='00000001' ";

                    //    sql = "SELECT a." + Glo.UserNo + ",a.Name || '/' || b.FullName as Name FROM Port_Emp a, Port_Dept b WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%') AND rownum<=12 " + specEmpNos;
                    //}
                    //else
                    //{
                    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    {
                        if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                            sql = "SELECT TOP 12 a.No as No,a.Name +'/'+b.Name as Name FROM Port_Emp a,Port_Dept b  WHERE a.OrgNo=" + WebUser.OrgNo + " AND (a.fk_dept=b.no) AND (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%') ";
                        else
                            sql = "SELECT TOP 12 a.No,a.Name +'/'+b.Name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.FK_Dept=b.No) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%') ";
                    }
                    if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.KingBaseR3 || SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                        sql = "SELECT a.No,a.Name || '/' || b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%') AND rownum<=12 ";
                    if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || SystemConfig.AppCenterDBType == DBType.UX)
                        sql = "SELECT a.No,CONCAT(a.Name,'/',b.name) as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%') LIMIT 12";

                }
                else
                {
                    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    {
                        if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                            sql = "SELECT TOP 12 a.No as No,a.Name +'/'+b.name as Name FROM Port_Emp a,Port_Dept b  WHERE A.OrgNo='" + WebUser.OrgNo + "' AND (a.fk_dept=b.no) and ( a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%')";
                        else
                            sql = "SELECT TOP 12 a.No,a.Name +'/'+b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR  a.PinYin LIKE '%," + emp.ToLower() + "%')";
                    }
                    if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.KingBaseR3 || SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                        sql = "SELECT a.No,a.Name || '/' || b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and ( a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR  a.PinYin LIKE '%," + emp.ToLower() + "%') AND rownum<=12 ";
                    if (SystemConfig.AppCenterDBType == DBType.MySQL
                        || SystemConfig.AppCenterDBType == DBType.PostgreSQL
                        || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB
                        || SystemConfig.AppCenterDBType == DBType.UX)
                        if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                            sql = "SELECT a.No,CONCAT(a.Name,'/',b.name) as Name FROM Port_Emp a,Port_Dept b  WHERE  A.OrgNo='" + WebUser.OrgNo + "' AND (a.fk_dept=b.no) and ( a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR  a.PinYin LIKE '%," + emp.ToLower() + "%' ) LIMIT 12";
                        else
                            sql = "SELECT a.No,CONCAT(a.Name,'/',b.name) as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and ( a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR  a.PinYin LIKE '%," + emp.ToLower() + "%' ) LIMIT 12";

                }
            }
            else
            {
                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                {
                    if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                        sql = "SELECT TOP 12 a.No,a.Name +'/'+b.Name as Name FROM Port_Emp a,Port_Dept b  WHERE  a.OrgNo='" + WebUser.OrgNo + "'  AND (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%')";
                    else
                        sql = "SELECT TOP 12 a.No,a.Name +'/'+b.Name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%')";
                }
                if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.KingBaseR3 || SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                    sql = "SELECT a.No,a.Name || '/' || b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%') and rownum<=12 ";
                if (SystemConfig.AppCenterDBType == DBType.MySQL
                    || SystemConfig.AppCenterDBType == DBType.PostgreSQL
                    || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB
                    || SystemConfig.AppCenterDBType == DBType.UX)
                {
                    //@lyc
                    if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                        sql = "SELECT a.No,CONCAT(a.Name,'/',b.name) as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%') LIMIT 12";
                    else
                        sql = "SELECT a.No,CONCAT(a.Name,'/',b.name) as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%')  AND a.OrgNo='" + BP.Web.WebUser.OrgNo + "' LIMIT 12 ";
                }
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";

            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获取本组织之外的联络人员
        /// </summary>
        /// <returns></returns>
        public string AccepterOfOfficer_Init()
        {
            Paras paras = new Paras();
            paras.SQL = "SELECT A.No,A.Name,FK_Dept, B.NameOfPath As DeptName,A.OrgNo From Port_Emp A,Port_Dept B WHERE A.DeptNo = B.No AND IsOfficer=1 AND A.OrgNo!=" + SystemConfig.AppCenterDBVarStr + "OrgNo";
            paras.Add("OrgNo", WebUser.OrgNo);
            DataTable dt = DBAccess.RunSQLReturnTable(paras);
            foreach (DataRow dr in dt.Rows)
            {
                string deptNo = dr[2] as string;
                string deptName = dr[3] as string;
                if (deptName.Contains("/") == false)
                {
                    string name = BP.WF.Dev2Interface.GetParentNameByCurrNo(deptNo, "Port_Dept", dr[4] as string);
                    if (deptName.Equals(name) == false)
                    {
                        DBAccess.RunSQL("UPDATE Port_Dept SET NameOfPath='" + name + "' WHERE No='" + deptNo + "'");
                        dr[3] = name;
                    }

                }
            }

            return BP.Tools.Json.ToJson(dt);
        }
        #region 会签.
        /// <summary>
        /// 会签
        /// </summary>
        /// <returns></returns>
        public string HuiQian_Init()
        {

            DataSet ds = BP.WF.Dev2Interface.Node_HuiQian_Init(this.WorkID);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <returns></returns>
        public string HuiQian_Delete()
        {
            BP.WF.Dev2Interface.Node_HuiQian_Delete(this.WorkID, this.GetRequestVal("FK_Emp"));
            return HuiQian_Init();
        }
        /// <summary>
        /// 增加审核人员
        /// </summary>
        /// <returns></returns>
        public string HuiQian_AddEmps()
        {
            return BP.WF.Dev2Interface.Node_HuiQian_AddEmps(this.WorkID, this.GetRequestVal("HuiQianType"), this.GetRequestVal("AddEmps"));
        }
        #endregion

        #region 与会签相关的.
        // 查询select集合
        public string HuiQian_SelectEmps()
        {
            return AccepterOfGener_SelectEmps();
        }
        /// <summary>
        /// 增加主持人
        /// </summary>
        /// <returns></returns>
        public string HuiQian_AddLeader()
        {
            return BP.WF.Dev2Interface.Node_HuiQian_AddLeader(this.WorkID);
        }
        /// <summary>
        /// 保存并关闭
        /// </summary>
        /// <returns></returns>
        public string HuiQian_SaveAndClose()
        {

            return BP.WF.Dev2Interface.Node_HuiQianDone(this.WorkID, this.ToNodeID);

        }
        #endregion

        #region 审核组件.
        /// <summary>
        /// 校验密码
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_CheckPass()
        {
            string sPass = this.GetRequestVal("SPass");
            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
            if (emp.SPass == sPass)
                return "签名成功";
            return "err@密码错误";
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_ChangePass()
        {
            string sPass = this.GetRequestVal("SPass");
            string sPass1 = this.GetRequestVal("SPass1");
            string sPass2 = this.GetRequestVal("SPass2");

            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
            if (emp.SPass == sPass)
                return "旧密码错误";

            if (sPass1.Equals(sPass2) == false)
                return "err@两次输入的密码不一致";
            emp.SPass = sPass2;
            emp.Update();
            return "密码修改成功";
        }

        /// <summary>
        /// 初始化审核组件数据.
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_Init()
        {
            if (BP.Web.WebUser.No == null)
                return "err@登录信息丢失,请重新登录.";

            //表单库审核组件流程编号为null的异常处理
            if (DataType.IsNullOrEmpty(this.FlowNo) == true)
                return "err@流程信息丢失,请联系管理员.";

            DataSet ds = new DataSet();

            #region 定义变量.

            //查询审核意见的表名
            string trackTable = "ND" + int.Parse(this.FlowNo) + "Track";

            //当前节点的审核信息
            NodeWorkCheck wcDesc = new NodeWorkCheck(this.NodeID);

            NodeWorkCheck frmWorkCheck = null;
            FrmAttachmentDBs athDBs = null; //上传附件集合
            Nodes nds = new Nodes(this.FlowNo);

            Node nd = null;


            Track tkDoc = null;
            string nodes = ""; //可以审核的节点.

            bool isExitTb_doc = true;

            DataRow row = null;

            //是不是只读?
            bool isReadonly = this.GetRequestVal("IsReadonly") != null && this.GetRequestVal("IsReadonly").Equals("1") ? true : false;

            //是否可以编辑审核意见
            bool isCanDo = false;
            if (isReadonly == true)
                isCanDo = false;
            else
                isCanDo = BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.WorkID, BP.Web.WebUser.No);


            NodeWorkCheck fwc = null;
            DataTable dt = null;

            //当前流程的审核组件的HuiQian
            NodeWorkChecks fwcs = new NodeWorkChecks();
            fwcs.Retrieve(NodeAttr.FK_Flow, this.FlowNo, NodeAttr.Step);
            ds.Tables.Add(wcDesc.ToDataTableField("WF_FrmWorkCheck"));


            //审核意见存储集合
            DataTable tkDt = new DataTable("Tracks");
            tkDt.Columns.Add("MyPk", typeof(string));
            tkDt.Columns.Add("NodeID", typeof(int));
            tkDt.Columns.Add("NodeName", typeof(string));
            tkDt.Columns.Add("Msg", typeof(string));
            tkDt.Columns.Add("EmpFrom", typeof(string));
            tkDt.Columns.Add("EmpFromT", typeof(string));
            tkDt.Columns.Add("DeptName", typeof(string));
            tkDt.Columns.Add("RDT", typeof(string));
            tkDt.Columns.Add("IsDoc", typeof(bool));
            tkDt.Columns.Add("ParentNode", typeof(int));
            tkDt.Columns.Add("ActionType", typeof(int));
            tkDt.Columns.Add("Tag", typeof(string));
            tkDt.Columns.Add("FWCView", typeof(string));
            tkDt.Columns.Add("WritImg", typeof(string));
            tkDt.Columns.Add("WriteStamp", typeof(string));
            //流程附件集合.
            DataTable athDt = new DataTable("Aths");
            athDt.Columns.Add("NodeID", typeof(int));
            athDt.Columns.Add("MyPK", typeof(string));
            athDt.Columns.Add("FK_FrmAttachment", typeof(string));
            athDt.Columns.Add("FK_MapData", typeof(string));
            athDt.Columns.Add("FileName", typeof(string));
            athDt.Columns.Add("FileExts", typeof(string));
            athDt.Columns.Add("CanDelete", typeof(bool));

            //当前节点的流程数据
            FrmAttachmentDBs frmathdbs = new FrmAttachmentDBs();
            frmathdbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment,
                "ND" + this.NodeID + "_FrmWorkCheck", FrmAttachmentDBAttr.RefPKVal,
                this.WorkID.ToString(), FrmAttachmentDBAttr.Rec, WebUser.No, FrmAttachmentDBAttr.RDT);

            foreach (FrmAttachmentDB athDB in frmathdbs)
            {
                row = athDt.NewRow();
                row["NodeID"] = this.NodeID;
                row["MyPK"] = athDB.MyPK;
                row["FK_FrmAttachment"] = athDB.FK_FrmAttachment;
                row["FK_MapData"] = athDB.FrmID;
                row["FileName"] = athDB.FileName;
                row["FileExts"] = athDB.FileExts;
                row["CanDelete"] = athDB.Rec == WebUser.No && isReadonly == false ? 1 : 0;
                athDt.Rows.Add(row);
            }
            ds.Tables.Add(athDt);

            //审核组件的定义
            WorkCheck wc = null;
            if (this.FID != 0)
                wc = new WorkCheck(this.FlowNo, this.NodeID, this.FID, 0);
            else
                wc = new WorkCheck(this.FlowNo, this.NodeID, this.WorkID, this.FID);


            //是否屏蔽正在审批的节点的审批意见(在在途，已完成等查看页面使用)
            bool isShowCurrNodeInfo = true;
            GenerWorkFlow gwf = new GenerWorkFlow();
            if (this.WorkID != 0)
            {
                gwf.WorkID = this.WorkID;
                gwf.Retrieve();
            }

            if (isReadonly == true && gwf.WFState == WFState.Runing && gwf.NodeID == this.NodeID)
                isShowCurrNodeInfo = false;

            /*
             * 获得当前节点已经审核通过的人员.
             * 比如：多人处理规则中的已经审核同意的人员，会签人员,组合成成一个字符串。
             * 格式为: ,zhangsan,lisi,
             * 用于处理在审核列表中屏蔽临时的保存的审核信息.
             * 12 为芒果增加一个非正常完成状态.
             * */
            string checkerPassed = ",";
            if (gwf.WFState != WFState.Complete && (int)gwf.WFState != 12)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerlist WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND IsPass=1 AND FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node Order By RDT,CDT";
                ps.Add("WorkID", this.WorkID);
                ps.Add("FK_Node", this.NodeID);
                DataTable checkerPassedDt = DBAccess.RunSQLReturnTable(ps);
                foreach (DataRow dr in checkerPassedDt.Rows)
                {
                    checkerPassed += dr["FK_Emp"] + ",";
                }
            }

            #endregion 定义变量.

            #region 判断是否显示 - 历史审核信息显示
            bool isDoc = false;
            Tracks tks = null;
            if (wcDesc.FWCListEnable == true)
            {
                tks = wc.HisWorkChecks;

                foreach (Track tk in tks)
                {
                    //评论组件
                    if (tk.HisActionType == ActionType.FlowBBS)
                        continue;

                    //不是审核状态、启动子流程、退回状态就跳出循环
                    if (tk.HisActionType != ActionType.WorkCheck
                        && tk.HisActionType != ActionType.StartChildenFlow
                        && tk.HisActionType != ActionType.Return)
                        continue;

                    //当前节点只显示自己审核的意见时
                    if (wcDesc.FWCMsgShow == 1 && tk.EmpFrom.Equals(WebUser.No) == false)
                        continue;

                    //退回
                    if (tk.HisActionType == ActionType.Return || tk.HisActionType == ActionType.ReturnAndBackWay)
                    {
                        //1.不显示退回意见 2.显示退回意见但是不是退回给本节点的意见
                        if (wcDesc.FWCIsShowReturnMsg == 0)
                            continue;
                        if (wcDesc.FWCIsShowReturnMsg == 1 && tk.NDTo != this.NodeID)
                            continue;
                    }

                    #region 多人处理（协作，会签，队列）
                    //如果是多人处理，流程未运行完成，就让其显示已经审核过的意见.
                    if (tk.NDFrom == this.NodeID
                        && checkerPassed.Contains("," + tk.EmpFrom + ",") == false
                        && (gwf.WFState != WFState.Complete && (int)gwf.WFState != 12))
                        continue;

                    if (gwf.NodeID == tk.NDFrom && checkerPassed.Contains("," + tk.EmpFrom + ",") == false
                       && (gwf.WFState != WFState.Complete && (int)gwf.WFState != 12))
                        continue;

                    /* if (tk.NDFrom == this.NodeID && gwf.HuiQianTaskSta != HuiQianTaskSta.None)
                     {
                         //判断会签, 去掉正在审批的节点.
                         if (tk.NDFrom == this.NodeID && isShowCurrNodeInfo == false)
                             continue;
                     }
 */

                    #endregion 多人处理（协作，会签，队列）
                    //为false可能为子流程数据
                    if (tk.NDFrom.ToString().StartsWith(int.Parse(this.FlowNo) + "") == true)
                    {
                        //当前节点在后面设计被删除时也需要屏蔽
                        nd = nds.GetEntityByKey(tk.NDFrom) as Node;
                        if (nd == null)
                            continue;
                    }




                    row = tkDt.NewRow();
                    row["MyPk"] = tk.MyPK;
                    row["NodeID"] = tk.NDFrom;
                    row["NodeName"] = tk.NDFromT;
                    fwc = fwcs.GetEntityByKey(tk.NDFrom) as NodeWorkCheck;

                    // zhoupeng 增加了判断，在会签的时候最后会签人发送前不能填写意见.
                    if (tk.NDFrom == this.NodeID && tk.EmpFrom == BP.Web.WebUser.No && isCanDo && isDoc == false)
                    {
                        isDoc = true;
                        row["IsDoc"] = true;
                    }
                    else
                        row["IsDoc"] = false;


                    row["ParentNode"] = 0;
                    row["RDT"] = DataType.IsNullOrEmpty(tk.RDT) ? "" : tk.NDFrom == tk.NDTo && DataType.IsNullOrEmpty(tk.Msg) ? "" : tk.RDT;

                    if (isReadonly == false && tk.EmpFrom == WebUser.No && this.NodeID == tk.NDFrom
                         && isExitTb_doc && (wcDesc.HisFrmWorkCheckType == FWCType.Check || (
                                        (wcDesc.HisFrmWorkCheckType == FWCType.DailyLog || wcDesc.HisFrmWorkCheckType == FWCType.WeekLog)
                                        && DateTime.Parse(tk.RDT).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                                        || (wcDesc.HisFrmWorkCheckType == FWCType.MonthLog
                                        && DateTime.Parse(tk.RDT).ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM"))
                                        ))
                    {
                        bool isLast = true;
                        foreach (Track tk1 in tks)
                        {
                            if (tk1.HisActionType == tk.HisActionType
                                && tk1.NDFrom == tk.NDFrom
                                && tk1.RDT.CompareTo(tk.RDT) > 0)
                            {
                                isLast = false;
                                break;
                            }
                        }

                        if (isLast && isDoc == false && gwf.WFState != WFState.Complete)
                        {
                            isExitTb_doc = false;
                            row["IsDoc"] = true;
                            isDoc = true;
                            row["Msg"] = Dev2Interface.GetCheckInfo(this.FlowNo, this.WorkID, this.NodeID, wcDesc.FWCDefInfo);
                            tkDoc = tk;
                        }
                        else
                        {
                            row["Msg"] = tk.MsgHtml;
                        }
                    }
                    else
                    {
                        row["Msg"] = tk.MsgHtml;
                    }

                    row["EmpFrom"] = tk.EmpFrom;
                    row["EmpFromT"] = tk.EmpFromT;
                    //获取部门
                    string DeptName = "";
                    string[] Arrays = tk.NodeData.Split('@');
                    foreach (string i in Arrays)
                    {
                        if (i.Contains("DeptName="))
                        {
                            DeptName = i.Split('=')[1];
                        }
                    }
                    row["DeptName"] = DeptName;
                    row["ActionType"] = tk.HisActionType;
                    row["Tag"] = tk.Tag;
                    row["FWCView"] = fwc != null ? fwc.FWCView : "";
                    if (wcDesc.SigantureEnabel != 2 && wcDesc.SigantureEnabel != 3 && wcDesc.SigantureEnabel != 5)
                        row["WritImg"] = "";
                    else
                        row["WritImg"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", tk.MyPK, "WriteDB");
                    if (wcDesc.SigantureEnabel != 4 && wcDesc.SigantureEnabel != 5)
                        row["WriteStamp"] = "";
                    else
                        row["WriteStamp"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", tk.MyPK, "FrmDB");
                    tkDt.Rows.Add(row);

                    #region //审核组件附件数据
                    athDBs = new FrmAttachmentDBs();
                    QueryObject obj_Ath = new QueryObject(athDBs);
                    obj_Ath.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, "ND" + tk.NDFrom + "_FrmWorkCheck");
                    obj_Ath.addAnd();
                    obj_Ath.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());
                    obj_Ath.addAnd();
                    obj_Ath.AddWhere(FrmAttachmentDBAttr.Rec, tk.EmpFrom);
                    obj_Ath.addOrderBy(FrmAttachmentDBAttr.RDT);
                    obj_Ath.DoQuery();

                    foreach (FrmAttachmentDB athDB in athDBs)
                    {
                        row = athDt.NewRow();
                        row["NodeID"] = tk.NDFrom;
                        row["MyPK"] = athDB.MyPK;
                        row["FK_FrmAttachment"] = athDB.FK_FrmAttachment;
                        row["FK_MapData"] = athDB.FrmID;
                        row["FileName"] = athDB.FileName;
                        row["FileExts"] = athDB.FileExts;
                        row["CanDelete"] = athDB.FrmID == this.NodeID.ToString() && athDB.Rec == WebUser.No && isReadonly == false;
                        athDt.Rows.Add(row);
                    }
                    #endregion

                    #region //子流程的审核组件数据
                    if (tk.FID != 0 && tk.HisActionType == ActionType.StartChildenFlow && tkDt.Select("ParentNode=" + tk.NDFrom).Length == 0)
                    {
                        string[] paras = tk.Tag.Split('@');
                        string[] p1 = paras[1].Split('=');
                        string fk_flow = p1[1]; //子流程编号

                        string[] p2 = paras[2].Split('=');
                        string workId = p2[1]; //子流程ID.
                        int biaoji = 0;

                        WorkCheck subwc = new WorkCheck(fk_flow, int.Parse(fk_flow + "01"), Int64.Parse(workId), 0);
                        string subtrackTable = "ND" + int.Parse(fk_flow) + "Track";
                        Tracks subtks = subwc.HisWorkChecks;
                        //取出来子流程的所有的节点。
                        Nodes subNds = new Nodes(fk_flow);
                        foreach (Node item in subNds)     //主要按顺序显示
                        {
                            foreach (Track mysubtk in subtks)
                            {
                                if (item.NodeID != mysubtk.NDFrom)
                                    continue;

                                /*输出该子流程的审核信息，应该考虑子流程的子流程信息, 就不考虑那样复杂了.*/
                                if (mysubtk.HisActionType == ActionType.WorkCheck)
                                {
                                    // 发起多个子流程时，发起人只显示一次
                                    if (mysubtk.NDFrom == int.Parse(fk_flow + "01") && biaoji == 1)
                                        continue;
                                    NodeWorkCheck subFrmCheck = new NodeWorkCheck("ND" + mysubtk.NDFrom);
                                    row = tkDt.NewRow();
                                    row["NodeID"] = mysubtk.NDFrom;
                                    row["NodeName"] = string.Format("(子流程){0}", mysubtk.NDFromT);
                                    row["Msg"] = mysubtk.MsgHtml;
                                    row["EmpFrom"] = mysubtk.EmpFrom;
                                    row["EmpFromT"] = mysubtk.EmpFromT;
                                    //获取部门
                                    DeptName = "";
                                    Arrays = tk.NodeData.Split('@');
                                    foreach (string i in Arrays)
                                    {
                                        if (i.Contains("DeptName="))
                                        {
                                            DeptName = i.Split('=')[1];
                                        }
                                    }
                                    row["DeptName"] = DeptName;
                                    row["RDT"] = mysubtk.RDT;
                                    row["IsDoc"] = false;
                                    row["ParentNode"] = tk.NDFrom;
                                    row["ActionType"] = mysubtk.HisActionType;
                                    row["Tag"] = mysubtk.Tag;
                                    row["FWCView"] = subFrmCheck.FWCView;
                                    if (subFrmCheck.SigantureEnabel != 2 && subFrmCheck.SigantureEnabel != 3 && subFrmCheck.SigantureEnabel != 5)
                                        row["WritImg"] = "";
                                    else
                                        row["WritImg"] = DBAccess.GetBigTextFromDB(subtrackTable, "MyPK", mysubtk.MyPK, "WriteDB");
                                    if (subFrmCheck.SigantureEnabel != 4 && subFrmCheck.SigantureEnabel != 5)
                                        row["WriteStamp"] = "";
                                    else
                                        row["WriteStamp"] = DBAccess.GetBigTextFromDB(subtrackTable, "MyPK", mysubtk.MyPK, "FrmDB");

                                    tkDt.Rows.Add(row);

                                    if (mysubtk.NDFrom == int.Parse(fk_flow + "01"))
                                    {
                                        biaoji = 1;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                }
            }
            #endregion 判断是否显示 - 历史审核信息显示

            #region 审核意见默认填写

            //首先判断当前是否有此意见? 如果是退回的该信息已经存在了.
            bool isHaveMyInfo = false;
            foreach (DataRow dr in tkDt.Rows)
            {
                string fk_node = dr["NodeID"].ToString();
                string empFrom = dr["EmpFrom"].ToString();
                if (int.Parse(fk_node) == this.NodeID && empFrom == BP.Web.WebUser.No)
                    isHaveMyInfo = true;
            }

            // 增加默认的审核意见.
            if (isExitTb_doc && wcDesc.HisFrmWorkCheckSta == FrmWorkCheckSta.Enable && isCanDo
                && isReadonly == false && isHaveMyInfo == false)
            {
                DataRow[] rows = null;
                nd = nds.GetEntityByKey(this.NodeID) as Node;
                if (wcDesc.FWCOrderModel == FWCOrderModel.SqlAccepter)
                {
                    rows = tkDt.Select("NodeID=" + this.NodeID + " AND Msg='' AND EmpFrom='" + WebUser.No + "'");

                    if (rows.Length == 0)
                        rows = tkDt.Select("NodeID=" + this.NodeID + " AND EmpFrom='" + WebUser.No + "'", "RDT DESC");

                    if (rows.Length > 0)
                    {
                        row = rows[0];
                        row["IsDoc"] = true;

                        row["Msg"] = Dev2Interface.GetCheckInfo(this.FlowNo, this.WorkID, this.NodeID, wcDesc.FWCDefInfo);
                        if (row["Msg"].ToString().Equals(""))
                            row["RDT"] = "";

                    }
                    else
                    {
                        row = tkDt.NewRow();
                        row["NodeID"] = this.NodeID;
                        row["NodeName"] = nd.FWCNodeName;
                        row["IsDoc"] = true;
                        row["ParentNode"] = 0;
                        row["RDT"] = "";

                        row["Msg"] = Dev2Interface.GetCheckInfo(this.FlowNo, this.WorkID, this.NodeID, wcDesc.FWCDefInfo);
                        row["EmpFrom"] = WebUser.No;
                        row["EmpFromT"] = WebUser.Name;
                        row["DeptName"] = WebUser.DeptName;
                        row["ActionType"] = ActionType.Forward;
                        row["Tag"] = Dev2Interface.GetCheckTag(this.FlowNo, this.WorkID, this.NodeID, WebUser.No);
                        tkDt.Rows.Add(row);
                    }
                }
                else
                {
                    row = tkDt.NewRow();
                    row["NodeID"] = this.NodeID;
                    row["NodeName"] = nd.FWCNodeName;
                    row["IsDoc"] = true;
                    row["ParentNode"] = 0;
                    row["RDT"] = "";

                    row["Msg"] = Dev2Interface.GetCheckInfo(this.FlowNo, this.WorkID, this.NodeID, wcDesc.FWCDefInfo);
                    row["EmpFrom"] = WebUser.No;
                    row["EmpFromT"] = WebUser.Name;
                    row["DeptName"] = WebUser.DeptName;
                    row["ActionType"] = ActionType.Forward;
                    row["Tag"] = Dev2Interface.GetCheckTag(this.FlowNo, this.WorkID, this.NodeID, WebUser.No);
                    if (wcDesc.SigantureEnabel == 5)
                    {
                        string sql = "Select MyPK From " + trackTable + "  WHERE ActionType=" + (int)ActionType.WorkCheck + " AND  NDFrom=" + this.NodeID + " AND WorkID=" + this.WorkID + " AND EmpFrom = '" + WebUser.No + "'";
                        string mypk = DBAccess.RunSQLReturnStringIsNull(sql, "");
                        if (DataType.IsNullOrEmpty(mypk) == false)
                        {
                            row["WritImg"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", mypk, "WriteDB");
                            row["WriteStamp"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", mypk, "FrmDB");
                        }
                        else
                        {
                            row["WritImg"] = "";
                            row["WriteStamp"] = "";

                        }
                    }
                    if (wcDesc.SigantureEnabel == 2 || wcDesc.SigantureEnabel == 3)
                    {
                        string sql = "Select MyPK From " + trackTable + "  WHERE ActionType=" + (int)ActionType.WorkCheck + " AND  NDFrom=" + this.NodeID + " AND WorkID=" + this.WorkID + " AND EmpFrom = '" + WebUser.No + "'";
                        string mypk = DBAccess.RunSQLReturnStringIsNull(sql, "");
                        if (DataType.IsNullOrEmpty(mypk) == false)
                            row["WritImg"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", mypk, "WriteDB");
                        else
                            row["WritImg"] = "";
                        row["WriteStamp"] = "";
                    }
                    if (wcDesc.SigantureEnabel == 4)
                    {
                        string sql = "Select MyPK From " + trackTable + "  WHERE ActionType=" + (int)ActionType.WorkCheck + " AND  NDFrom=" + this.NodeID + " AND WorkID=" + this.WorkID + " AND EmpFrom = '" + WebUser.No + "'";
                        string mypk = DBAccess.RunSQLReturnStringIsNull(sql, "");
                        if (DataType.IsNullOrEmpty(mypk) == false)
                            row["WriteStamp"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", mypk, "FrmDB");
                        else
                            row["WriteStamp"] = "";
                        row["WritImg"] = "";
                    }
                    if (wcDesc.SigantureEnabel == 0 || wcDesc.SigantureEnabel == 1)
                    {
                        row["WritImg"] = "";
                        row["WriteStamp"] = "";
                    }
                    tkDt.Rows.Add(row);
                }
            }
            #endregion

            #region 显示有审核组件，但还未审核的节点. 包括退回后的.
            if (tks == null)
                tks = wc.HisWorkChecks;
            WF_WorkOpt_OneWork oneWork = new WF_WorkOpt_OneWork();
            string nodeIDs = oneWork.GetFutureNodeNotContainsEmps();
            foreach (NodeWorkCheck item in fwcs)
            {
                if (item.FWCIsShowTruck == false)
                    continue;  //不需要显示历史记录.
                if (nodeIDs.Contains(item.NodeID + ",") == false)
                    continue;
                //是否已审核.
                bool isHave = false;
                foreach (BP.WF.Track tk in tks)
                {
                    //翻译.
                    if (tk.NDFrom == item.NodeID && tk.HisActionType == ActionType.WorkCheck)
                    {
                        isHave = true; //已经有了
                        break;
                    }
                }

                if (isHave == true)
                    continue;

                row = tkDt.NewRow();
                row["NodeID"] = item.NodeID;

                Node mynd = (Node)nds.GetEntityByKey(item.NodeID);
                row["NodeName"] = mynd.FWCNodeName;
                row["IsDoc"] = false;
                row["ParentNode"] = 0;
                row["RDT"] = "";
                row["Msg"] = "未审批";
                row["EmpFrom"] = "";
                row["EmpFromT"] = "";
                row["DeptName"] = "";
                row["WritImg"] = "";
                row["WriteStamp"] = "";
                tkDt.Rows.Add(row);
            }
            #endregion 增加空白.

            ds.Tables.Add(tkDt);


            //如果有 SignType 列就获得签名信息.
            if (SystemConfig.CustomerNo == "TianYe")
            {
                string tTable = "ND" + int.Parse(this.FlowNo) + "Track";
                string sql = "SELECT distinct a.No, a.SignType, a.EleID FROM Port_Emp a, " + tTable + " b WHERE (A.No='" + WebUser.No + "') OR B.ActionType=22 AND a.No=b.EmpFrom AND B.WorkID=" + this.WorkID;

                DataTable dtTrack = DBAccess.RunSQLReturnTable(sql);
                dtTrack.TableName = "SignType";

                dtTrack.Columns["NO"].ColumnName = "No";
                dtTrack.Columns["SIGNTYPE"].ColumnName = "SignType";
                dtTrack.Columns["ELEID"].ColumnName = "EleID";

                ds.Tables.Add(dtTrack);
            }

            string str = BP.Tools.Json.ToJson(ds);

            return str;
        }
        /// <summary>
        /// 初始化审核组件数据.
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_Init2019()
        {
            if (BP.Web.WebUser.No == null)
                return "err@登录信息丢失,请重新登录.";

            #region 定义变量.
            string trackTable = "ND" + int.Parse(this.FlowNo) + "Track";
            NodeWorkCheck wcDesc = new NodeWorkCheck(this.NodeID); // 当前节点的审核组件
            NodeWorkCheck frmWorkCheck = null;
            FrmAttachmentDBs athDBs = null;    //附件数据
            Nodes nds = new Nodes(this.FlowNo); //该流程的所有节点
            NodeWorkChecks fwcs = new NodeWorkChecks();
            Node nd = null;
            WorkCheck wc = null;
            Tracks tks = null;
            Track tkDoc = null;
            string nodes = ""; //已经审核过的节点.
            bool isCanDo = false;//是否可以审批
            bool isExitTb_doc = true;
            DataSet ds = new DataSet();
            DataRow row = null;

            //是不是只读?
            bool isReadonly = false;
            if (this.GetRequestVal("IsReadonly") != null && this.GetRequestVal("IsReadonly").Equals("1"))
                isReadonly = true;

            DataTable nodeEmps = new DataTable();
            NodeWorkCheck fwc = null;
            DataTable dt = null;
            int idx = 0;
            int noneEmpIdx = 0;

            fwcs.Retrieve(NodeAttr.FK_Flow, this.FlowNo, NodeAttr.Step);
            ds.Tables.Add(wcDesc.ToDataTableField("WF_FrmWorkCheck")); //当前的节点审核组件定义，放入ds.

            //审核意见的存储集合
            DataTable tkDt = new DataTable("Tracks");
            tkDt.Columns.Add("MyPk", typeof(string));
            tkDt.Columns.Add("NodeID", typeof(int));
            tkDt.Columns.Add("NodeName", typeof(string));
            tkDt.Columns.Add("Msg", typeof(string));
            tkDt.Columns.Add("EmpFrom", typeof(string));
            tkDt.Columns.Add("EmpFromT", typeof(string));
            tkDt.Columns.Add("DeptName", typeof(string));
            tkDt.Columns.Add("RDT", typeof(string));
            tkDt.Columns.Add("IsDoc", typeof(bool));
            tkDt.Columns.Add("ParentNode", typeof(int));
            tkDt.Columns.Add("ActionType", typeof(int));
            tkDt.Columns.Add("Tag", typeof(string));
            tkDt.Columns.Add("FWCView", typeof(string));
            tkDt.Columns.Add("WritImg", typeof(string));
            tkDt.Columns.Add("WriteStamp", typeof(string));
            //流程附件.
            DataTable athDt = new DataTable("Aths");
            athDt.Columns.Add("NodeID", typeof(int));
            athDt.Columns.Add("MyPK", typeof(string));
            athDt.Columns.Add("FK_FrmAttachment", typeof(string));
            athDt.Columns.Add("FK_MapData", typeof(string));
            athDt.Columns.Add("FileName", typeof(string));
            athDt.Columns.Add("FileExts", typeof(string));
            athDt.Columns.Add("CanDelete", typeof(bool));
            //当前节点的流程数据
            FrmAttachmentDBs frmathdbs = new FrmAttachmentDBs();
            frmathdbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, "ND" + this.NodeID + "_FrmWorkCheck",
                FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString(), "Rec", WebUser.No, FrmAttachmentDBAttr.RDT);

            foreach (FrmAttachmentDB athDB in frmathdbs)
            {
                row = athDt.NewRow();
                row["NodeID"] = this.NodeID;
                row["MyPK"] = athDB.MyPK;
                row["FK_FrmAttachment"] = athDB.FK_FrmAttachment;
                row["FK_MapData"] = athDB.FrmID;
                row["FileName"] = athDB.FileName;
                row["FileExts"] = athDB.FileExts;
                row["CanDelete"] = athDB.Rec == WebUser.No && isReadonly == false;
                athDt.Rows.Add(row);
            }
            ds.Tables.Add(athDt);

            if (this.FID != 0)
                wc = new WorkCheck(this.FlowNo, this.NodeID, this.WorkID, this.FID);
            else
                wc = new WorkCheck(this.FlowNo, this.NodeID, this.WorkID, 0);

            //是否只读？
            if (isReadonly == true)
                isCanDo = false;
            else
                isCanDo = BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.WorkID, BP.Web.WebUser.No);

            //如果是查看状态, 为了屏蔽掉正在审批的节点, 在查看审批意见中.
            bool isShowCurrNodeInfo = true;
            GenerWorkFlow gwf = new GenerWorkFlow();
            if (this.WorkID != 0)
            {
                gwf.WorkID = this.WorkID;
                gwf.Retrieve();
            }

            if (isCanDo == false && isReadonly == true)
            {
                if (gwf.WFState == WFState.Runing && gwf.NodeID == this.NodeID)
                    isShowCurrNodeInfo = false;
            }

            /*
             * 获得当前节点已经审核通过的人员.
             * 比如：多人处理规则中的已经审核同意的人员，会签人员,组合成成一个字符串。
             * 格式为: ,zhangsan,lisi,
             * 用于处理在审核列表中屏蔽临时的保存的审核信息.
             * 12 为芒果增加一个非正常完成状态.
             * */
            string checkerPassed = ",";
            if (gwf.WFState != WFState.Complete && (int)gwf.WFState != 12)
            {
                //Paras ps = new Paras();
                //ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerlist WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND IsPass=1 AND FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node  Order By RDT,CDT";
                //ps.Add("WorkID", this.WorkID);
                //ps.Add("FK_Node", this.NodeID);

                string sql = "SELECT EmpFrom as FK_Emp  FROM ND" + int.Parse(this.FlowNo) + "Track WHERE WorkID =" + this.WorkID + "  AND NDFrom = " + this.NodeID;
                DataTable checkerPassedDt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in checkerPassedDt.Rows)
                {
                    checkerPassed += dr["FK_Emp"] + ",";
                }
            }

            #endregion 定义变量.

            #region 判断是否显示 - 历史审核信息显示
            bool isDoc = false;
            if (wcDesc.FWCListEnable == true)
            {
                tks = wc.HisWorkChecks;

                foreach (BP.WF.Track tk in tks)
                {
                    if (tk.HisActionType == ActionType.FlowBBS)
                        continue;

                    if (wcDesc.FWCMsgShow == 1 && tk.EmpFrom.Equals(WebUser.No) == false)
                        continue;

                    switch (tk.HisActionType)
                    {
                        case ActionType.WorkCheck:
                        case ActionType.Forward:
                        case ActionType.StartChildenFlow:
                        case ActionType.ForwardHL:
                            if (nodes.Contains(tk.NDFrom + ",") == false)
                                nodes += tk.NDFrom + ",";
                            break;
                        case ActionType.Return:
                        case ActionType.ReturnAndBackWay:
                            if (wcDesc.FWCIsShowReturnMsg == 1 && tk.NDTo == this.NodeID)
                            {
                                if (nodes.Contains(tk.NDFrom + ",") == false)
                                    nodes += tk.NDFrom + ",";
                            }
                            if (wcDesc.FWCIsShowReturnMsg == 2)
                            {
                                if (nodes.Contains(tk.NDFrom + ",") == false)
                                    nodes += tk.NDFrom + ",";
                            }
                            break;
                        default:
                            continue;
                    }
                }
                foreach (Track tk in tks)
                {

                    if (nodes.Contains(tk.NDFrom + ",") == false)
                        continue;

                    //退回
                    if (tk.HisActionType == ActionType.Return || tk.HisActionType == ActionType.ReturnAndBackWay)
                    {
                        //1.不显示退回意见 2.显示退回意见但是不是退回给本节点的意见
                        if (wcDesc.FWCIsShowReturnMsg == 0)
                            continue;
                        if (wcDesc.FWCIsShowReturnMsg == 1 && tk.NDTo != this.NodeID)
                            continue;
                    }

                    //  此部分被zhoupeng注释, 在会签的时候显示不到意见。
                    // 如果是当前的节点.当前人员可以处理, 已经审批通过的人员.
                    if (tk.NDFrom == this.NodeID
                        && isCanDo == true
                        && tk.EmpFrom.Equals(WebUser.No) == false
                        && checkerPassed.Contains("," + tk.EmpFrom + ",") == false)
                        continue;

                    if (tk.NDFrom == this.NodeID && gwf.HuiQianTaskSta != HuiQianTaskSta.None)
                    {
                        //判断会签, 去掉正在审批的节点.
                        if (tk.NDFrom == this.NodeID && isShowCurrNodeInfo == false)
                            continue;
                    }
                    //如果是多人处理，就让其显示已经审核过的意见.
                    //if (tk.NDFrom == this.NodeID&& checkerPassed.IndexOf("," + tk.EmpFrom + ",") < 0 && (gwf.WFState != WFState.Complete && (int)gwf.WFState != 12))
                    //    continue;

                    fwc = fwcs.GetEntityByKey(tk.NDFrom) as NodeWorkCheck;
                    if (fwc == null || fwc.FWCSta != FrmWorkCheckSta.Enable)
                        continue;

                    //历史审核信息现在存放在流程前进的节点中
                    switch (tk.HisActionType)
                    {
                        case ActionType.Forward:
                        case ActionType.ForwardAskfor:
                        case ActionType.Start:
                        //case ActionType.UnSend:
                        // case ActionType.ForwardFL:
                        case ActionType.ForwardHL:
                        case ActionType.SubThreadForward:
                        case ActionType.TeampUp:
                        case ActionType.Return:
                        case ActionType.ReturnAndBackWay:
                        case ActionType.StartChildenFlow:
                        case ActionType.FlowOver:
                            row = tkDt.NewRow();
                            row["MyPk"] = tk.MyPK;
                            row["NodeID"] = tk.NDFrom;
                            row["NodeName"] = tk.NDFromT;

                            // zhoupeng 增加了判断，在会签的时候最后会签人发送前不能填写意见.
                            if (tk.NDFrom == this.NodeID && tk.EmpFrom == BP.Web.WebUser.No && isCanDo && isDoc == false)
                            {
                                //修改测试
                                isDoc = true;

                            }

                            row["IsDoc"] = false;

                            row["ParentNode"] = 0;
                            row["RDT"] = DataType.IsNullOrEmpty(tk.RDT) ? "" : tk.NDFrom == tk.NDTo && DataType.IsNullOrEmpty(tk.Msg) ? "" : tk.RDT;
                            //row["T_NodeIndex"] = tk.Row["T_NodeIndex"];
                            //row["T_CheckIndex"] = tk.Row["T_CheckIndex"];

                            row["Msg"] = tk.MsgHtml;


                            row["EmpFrom"] = tk.EmpFrom;
                            row["EmpFromT"] = tk.EmpFromT;
                            //获取部门
                            string DeptName = "";
                            string[] Arrays = tk.NodeData.Split('@');
                            foreach (string i in Arrays)
                            {
                                if (i.Contains("DeptName="))
                                {
                                    DeptName = i.Split('=')[1];
                                }
                            }
                            row["DeptName"] = DeptName;
                            row["ActionType"] = tk.HisActionType;
                            row["Tag"] = tk.Tag;
                            if (wcDesc.SigantureEnabel != 2 && wcDesc.SigantureEnabel != 3 && wcDesc.SigantureEnabel != 5)
                                row["WritImg"] = "";
                            else
                                row["WritImg"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", tk.MyPK, "WriteDB");
                            if (wcDesc.SigantureEnabel != 4 && wcDesc.SigantureEnabel != 5)
                                row["WriteStamp"] = "";
                            else
                                row["WriteStamp"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", tk.MyPK, "FrmDB");
                            tkDt.Rows.Add(row);

                            #region 审核组件附件数据

                            //athDBs = new FrmAttachmentDBs();
                            //QueryObject obj_Ath = new QueryObject(athDBs);
                            //obj_Ath.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, "ND" + tk.NDFrom + "_FrmWorkCheck");
                            //obj_Ath.addAnd();
                            //obj_Ath.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());
                            //obj_Ath.addAnd();
                            //obj_Ath.AddWhere(FrmAttachmentDBAttr.Rec, tk.EmpFrom);
                            //obj_Ath.addOrderBy(FrmAttachmentDBAttr.RDT);
                            //obj_Ath.DoQuery();

                            //foreach (FrmAttachmentDB athDB in athDBs)
                            //{
                            //    row = athDt.NewRow();
                            //    row["NodeID"] = tk.NDFrom;
                            //    row["MyPK"] = athDB.MyPK;
                            //    row["FK_FrmAttachment"] = athDB.FK_FrmAttachment;
                            //    row["FK_MapData"] = athDB.FrmID;
                            //    row["FileName"] = athDB.FileName;
                            //    row["FileExts"] = athDB.FileExts;
                            //    row["CanDelete"] = athDB.FrmID == this.NodeID.ToString() && athDB.Rec == WebUser.No && isReadonly == false;
                            //    athDt.Rows.Add(row);
                            //}
                            #endregion

                            #region //子流程的审核组件数据
                            if (tk.FID != 0
                                && tk.HisActionType == ActionType.StartChildenFlow && tkDt.Select("ParentNode=" + tk.NDFrom).Length == 0)
                            {
                                string[] paras = tk.Tag.Split('@');
                                string[] p1 = paras[1].Split('=');
                                string fk_flow = p1[1]; //子流程编号

                                string[] p2 = paras[2].Split('=');
                                string workId = p2[1]; //子流程ID.
                                int biaoji = 0;

                                WorkCheck subwc = new WorkCheck(fk_flow, int.Parse(fk_flow + "01"), Int64.Parse(workId), 0);
                                string subtrackTable = "ND" + int.Parse(fk_flow) + "Track";
                                Tracks subtks = subwc.HisWorkChecks;
                                //取出来子流程的所有的节点。
                                Nodes subNds = new Nodes(fk_flow);
                                foreach (Node item in subNds)     //主要按顺序显示
                                {
                                    foreach (Track mysubtk in subtks)
                                    {
                                        if (item.NodeID != mysubtk.NDFrom)
                                            continue;

                                        /*输出该子流程的审核信息，应该考虑子流程的子流程信息, 就不考虑那样复杂了.*/
                                        if (mysubtk.HisActionType == ActionType.WorkCheck)
                                        {
                                            // 发起多个子流程时，发起人只显示一次
                                            if (mysubtk.NDFrom == int.Parse(fk_flow + "01") && biaoji == 1)
                                                continue;
                                            NodeWorkCheck subFrmCheck = new NodeWorkCheck("ND" + mysubtk.NDFrom);
                                            row = tkDt.NewRow();
                                            row["NodeID"] = mysubtk.NDFrom;
                                            row["NodeName"] = string.Format("(子流程){0}", mysubtk.NDFromT);
                                            row["Msg"] = mysubtk.MsgHtml;
                                            row["EmpFrom"] = mysubtk.EmpFrom;
                                            row["EmpFromT"] = mysubtk.EmpFromT;

                                            //获取部门
                                            DeptName = "";
                                            Arrays = mysubtk.NodeData.Split('@');
                                            foreach (string i in Arrays)
                                            {
                                                if (i.Contains("DeptName="))
                                                {
                                                    DeptName = i.Split('=')[1];
                                                }
                                            }
                                            row["DeptName"] = DeptName;
                                            row["RDT"] = mysubtk.RDT;
                                            row["IsDoc"] = false;
                                            row["ParentNode"] = tk.NDFrom;
                                            // row["T_NodeIndex"] = idx++;
                                            // row["T_CheckIndex"] = noneEmpIdx++;
                                            row["ActionType"] = mysubtk.HisActionType;
                                            row["Tag"] = mysubtk.Tag;
                                            if (subFrmCheck.SigantureEnabel != 2 && subFrmCheck.SigantureEnabel != 3 && subFrmCheck.SigantureEnabel != 5)
                                                row["WritImg"] = "";
                                            else
                                                row["WritImg"] = DBAccess.GetBigTextFromDB(subtrackTable, "MyPK", mysubtk.MyPK, "WriteDB");
                                            if (subFrmCheck.SigantureEnabel != 4 && subFrmCheck.SigantureEnabel != 5)
                                                row["WriteStamp"] = "";
                                            else
                                                row["WriteStamp"] = DBAccess.GetBigTextFromDB(subtrackTable, "MyPK", mysubtk.MyPK, "FrmDB");

                                            tkDt.Rows.Add(row);

                                            if (mysubtk.NDFrom == int.Parse(fk_flow + "01"))
                                            {
                                                biaoji = 1;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                            break;
                        default: break;
                    }
                }

#warning 处理审核信息,删除掉他.
                if (tkDoc != null && 1 == 2)
                {
                    //判断可编辑审核信息是否处于最后一条，不处于最后一条，则将其移到最后一条
                    DataRow rdoc = tkDt.Select("IsDoc=True")[0];
                    if (tkDt.Rows.IndexOf(rdoc) != tkDt.Rows.Count - 1)
                    {
                        tkDt.Rows.Add(rdoc.ItemArray)["RDT"] = "";

                        rdoc["IsDoc"] = false;
                        rdoc["RDT"] = tkDoc.RDT;
                        rdoc["Msg"] = tkDoc.MsgHtml;
                    }
                }
            }
            #endregion 判断是否显示 - 历史审核信息显示

            #region 审核意见默认填写

            //首先判断当前是否有此意见? 如果是退回的该信息已经存在了.
            bool isHaveMyInfo = false;


            // 增加默认的审核意见.
            if (isExitTb_doc && wcDesc.HisFrmWorkCheckSta == FrmWorkCheckSta.Enable && isCanDo
                && isReadonly == false && isHaveMyInfo == false)
            {
                DataRow[] rows = null;
                nd = nds.GetEntityByKey(this.NodeID) as Node;
                if (wcDesc.FWCOrderModel == FWCOrderModel.SqlAccepter)
                {
                    rows = tkDt.Select("NodeID=" + this.NodeID + " AND Msg='' AND EmpFrom='" + WebUser.No + "'");

                    if (rows.Length == 0)
                        rows = tkDt.Select("NodeID=" + this.NodeID + " AND EmpFrom='" + WebUser.No + "'", "RDT DESC");

                    if (rows.Length > 0)
                    {
                        row = rows[0];
                        row["IsDoc"] = true;

                        row["Msg"] = Dev2Interface.GetCheckInfo(this.FlowNo, this.WorkID, this.NodeID, wcDesc.FWCDefInfo);
                        if (row["Msg"].ToString().Equals(""))
                            row["RDT"] = "";

                    }
                    else
                    {
                        row = tkDt.NewRow();
                        row["NodeID"] = this.NodeID;
                        row["NodeName"] = nd.FWCNodeName;
                        row["IsDoc"] = true;
                        row["ParentNode"] = 0;
                        row["RDT"] = "";

                        row["Msg"] = Dev2Interface.GetCheckInfo(this.FlowNo, this.WorkID, this.NodeID, wcDesc.FWCDefInfo);
                        row["EmpFrom"] = WebUser.No;
                        row["EmpFromT"] = WebUser.Name;
                        row["DeptName"] = WebUser.DeptName;
                        //row["T_NodeIndex"] = ++idx;
                        //row["T_CheckIndex"] = ++noneEmpIdx;
                        row["ActionType"] = ActionType.Forward;
                        row["Tag"] = Dev2Interface.GetCheckTag(this.FlowNo, this.WorkID, this.NodeID, WebUser.No);
                        if (wcDesc.SigantureEnabel == 5)
                        {
                            string sql = "Select MyPK From " + trackTable + "  WHERE ActionType=" + (int)ActionType.WorkCheck + " AND  NDFrom=" + this.NodeID + " AND  NDTo=" + this.NodeID + " AND WorkID=" + this.WorkID + " AND EmpFrom = '" + WebUser.No + "'";
                            string mypk = DBAccess.RunSQLReturnStringIsNull(sql, "");
                            if (DataType.IsNullOrEmpty(mypk) == false)
                            {
                                row["WritImg"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", mypk, "WriteDB");
                                row["WriteStamp"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", mypk, "FrmDB");
                            }
                            else
                            {
                                row["WritImg"] = "";
                                row["WriteStamp"] = "";

                            }
                        }
                        if (wcDesc.SigantureEnabel == 2 || wcDesc.SigantureEnabel == 3)
                        {
                            string sql = "Select MyPK From " + trackTable + "  WHERE ActionType=" + (int)ActionType.WorkCheck + " AND  NDFrom=" + this.NodeID + " AND  NDTo=" + this.NodeID + " AND WorkID=" + this.WorkID + " AND EmpFrom = '" + WebUser.No + "'";
                            string mypk = DBAccess.RunSQLReturnStringIsNull(sql, "");
                            if (DataType.IsNullOrEmpty(mypk) == false)
                                row["WritImg"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", mypk, "WriteDB");
                            else
                                row["WritImg"] = "";
                            row["WriteStamp"] = "";
                        }
                        if (wcDesc.SigantureEnabel == 4)
                        {
                            string sql = "Select MyPK From " + trackTable + "  WHERE ActionType=" + (int)ActionType.WorkCheck + " AND  NDFrom=" + this.NodeID + " AND  NDTo=" + this.NodeID + " AND WorkID=" + this.WorkID + " AND EmpFrom = '" + WebUser.No + "'";
                            string mypk = DBAccess.RunSQLReturnStringIsNull(sql, "");
                            if (DataType.IsNullOrEmpty(mypk) == false)
                                row["WriteStamp"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", mypk, "FrmDB");
                            else
                                row["WriteStamp"] = "";
                            row["WritImg"] = "";
                        }
                        if (wcDesc.SigantureEnabel == 0 || wcDesc.SigantureEnabel == 1)
                        {
                            row["WritImg"] = "";
                            row["WriteStamp"] = "";
                        }
                        tkDt.Rows.Add(row);
                    }
                }
                else
                {
                    row = tkDt.NewRow();
                    row["NodeID"] = this.NodeID;
                    row["NodeName"] = nd.FWCNodeName;
                    row["IsDoc"] = true;
                    row["ParentNode"] = 0;
                    row["RDT"] = "";

                    row["Msg"] = Dev2Interface.GetCheckInfo(this.FlowNo, this.WorkID, this.NodeID, wcDesc.FWCDefInfo);
                    row["EmpFrom"] = WebUser.No;
                    row["EmpFromT"] = WebUser.Name;
                    row["DeptName"] = WebUser.DeptName;
                    //row["T_NodeIndex"] = ++idx;
                    //row["T_CheckIndex"] = ++noneEmpIdx;
                    row["ActionType"] = ActionType.Forward;
                    row["Tag"] = Dev2Interface.GetCheckTag(this.FlowNo, this.WorkID, this.NodeID, WebUser.No);
                    if (wcDesc.SigantureEnabel != 2)
                        row["WritImg"] = "";
                    else
                    {
                        string sql = "Select MyPK From " + trackTable + "  WHERE ActionType=" + (int)ActionType.WorkCheck + " AND  NDFrom=" + this.NodeID + " AND  NDTo=" + this.NodeID + " AND WorkID=" + this.WorkID + " AND EmpFrom = '" + WebUser.No + "'";
                        row["WritImg"] = DBAccess.GetBigTextFromDB(trackTable, "MyPK", DBAccess.RunSQLReturnVal(sql) == null ? null : DBAccess.RunSQLReturnVal(sql).ToString(), "WriteDB");
                    }
                    tkDt.Rows.Add(row);
                }
            }
            #endregion

            #region 显示有审核组件，但还未审核的节点. 包括退回后的.
            if (tks == null)
                tks = wc.HisWorkChecks;

            WF_WorkOpt_OneWork oneWork = new WF_WorkOpt_OneWork();
            string nodeIDs = oneWork.GetFutureNodeNotContainsEmps();
            foreach (NodeWorkCheck item in fwcs)
            {
                if (item.FWCIsShowTruck == false)
                    continue;  //不需要显示历史记录.
                if (nodeIDs.Contains(item.NodeID + ",") == false)
                    continue;
                //是否已审核.
                bool isHave = false;
                foreach (BP.WF.Track tk in tks)
                {
                    //翻译.
                    if (tk.NDFrom == this.NodeID && tk.HisActionType == ActionType.WorkCheck)
                    {
                        isHave = true; //已经有了
                        break;
                    }
                }

                if (isHave == true)
                    continue;

                row = tkDt.NewRow();
                row["NodeID"] = item.NodeID;

                Node mynd = (Node)nds.GetEntityByKey(item.NodeID);
                row["NodeName"] = mynd.FWCNodeName;
                row["IsDoc"] = false;
                row["ParentNode"] = 0;
                row["RDT"] = "";
                row["Msg"] = "未审批";
                row["EmpFrom"] = "";
                row["EmpFromT"] = "";
                row["DeptName"] = "";
                //row["T_NodeIndex"] = ++idx;
                //row["T_CheckIndex"] = ++noneEmpIdx;

                tkDt.Rows.Add(row);
            }
            #endregion 增加空白.

            ds.Tables.Add(tkDt);


            //如果有 SignType 列就获得签名信息.
            if (SystemConfig.CustomerNo == "TianYe")
            {
                string tTable = "ND" + int.Parse(this.FlowNo) + "Track";
                string sql = "SELECT distinct a.No, a.SignType, a.EleID FROM Port_Emp a, " + tTable + " b WHERE (A.No='" + WebUser.No + "') OR B.ActionType=22 AND a.No=b.EmpFrom AND B.WorkID=" + this.WorkID;

                DataTable dtTrack = DBAccess.RunSQLReturnTable(sql);
                dtTrack.TableName = "SignType";

                dtTrack.Columns["NO"].ColumnName = "No";
                dtTrack.Columns["SIGNTYPE"].ColumnName = "SignType";
                dtTrack.Columns["ELEID"].ColumnName = "EleID";

                ds.Tables.Add(dtTrack);
            }

            string str = BP.Tools.Json.ToJson(ds);
            //用于jflow数据输出格式对比.
            //  DataType.WriteFile("c:/WorkCheck_Init_ccflow.txt", str);
            return str;
        }
        /// <summary>
        /// 获取审核组件中刚上传的附件列表信息
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_GetNewUploadedAths()
        {
            DataRow row = null;
            string athNames = GetRequestVal("Names");
            string attachPK = GetRequestVal("AttachPK");

            DataTable athDt = new DataTable("Aths");
            athDt.Columns.Add("NodeID", typeof(int));
            athDt.Columns.Add("MyPK", typeof(string));
            athDt.Columns.Add("FK_FrmAttachment", typeof(string));
            athDt.Columns.Add("FK_MapData", typeof(string));
            athDt.Columns.Add("FileName", typeof(string));
            athDt.Columns.Add("FileExts", typeof(string));
            athDt.Columns.Add("CanDelete", typeof(string));
            athDt.Columns.Add("Rec", typeof(string));

            FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
            QueryObject obj_Ath = new QueryObject(athDBs);
            obj_Ath.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, "ND" + this.NodeID + "_FrmWorkCheck");
            obj_Ath.addAnd();
            obj_Ath.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());
            obj_Ath.addOrderBy(FrmAttachmentDBAttr.RDT);
            obj_Ath.DoQuery();

            foreach (FrmAttachmentDB athDB in athDBs)
            {
                if (athNames != null && athNames.ToLower().IndexOf("|" + athDB.FileName.ToLower() + "|") == -1)
                    continue;

                row = athDt.NewRow();

                row["NodeID"] = this.NodeID;
                row["MyPK"] = athDB.MyPK;
                row["FK_FrmAttachment"] = athDB.FK_FrmAttachment;
                row["FK_MapData"] = athDB.FrmID;
                row["FileName"] = athDB.FileName;
                row["FileExts"] = athDB.FileExts;
                row["CanDelete"] = athDB.Rec == WebUser.No ? 1 : 0;
                row["Rec"] = athDB.Rec;
                athDt.Rows.Add(row);
            }

            return BP.Tools.Json.ToJson(athDt);
        }
        /// <summary>
        /// 获取附件链接
        /// </summary>
        /// <param name="athDB"></param>
        /// <returns></returns>
        private string GetFileAction(FrmAttachmentDB athDB)
        {
            if (athDB == null || athDB.FileExts == "") return "#";

            FrmAttachment athDesc = new FrmAttachment(athDB.FK_FrmAttachment);
            switch (athDB.FileExts)
            {
                case "doc":
                case "docx":
                case "xls":
                case "xlsx":
                    return "javascript:AthOpenOfiice('" + athDB.FK_FrmAttachment + "','" + this.WorkID + "','" + athDB.MyPK + "','" + athDB.FrmID + "','" + athDB.FK_FrmAttachment + "','" + this.NodeID + "')";
                case "txt":
                case "jpg":
                case "jpeg":
                case "gif":
                case "png":
                case "bmp":
                case "ceb":
                    return "javascript:AthOpenView('" + athDB.RefPKVal + "','" + athDB.MyPK + "','" + athDB.FK_FrmAttachment + "','" + athDB.FileExts + "','" + this.FlowNo + "','" + athDB.FrmID + "','" + this.WorkID + "','false')";
                case "pdf":
                    return athDesc.SaveTo + this.WorkID + "/" + athDB.MyPK + "." + athDB.FileName;
            }

            return "javascript:AthDown('" + athDB.FK_FrmAttachment + "','" + this.WorkID + "','" + athDB.MyPK + "','" + athDB.FrmID + "','" + this.FlowNo + "','" + athDB.FK_FrmAttachment + "')";
        }

        /// <summary>
        /// 审核信息保存.
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_Save()
        {
            //设计的时候,workid=0,不让其存储.
            if (this.WorkID == 0)
                return "";

            // 审核信息.
            string msg = "";
            string writeImg = GetRequestVal("WriteImg");
            string handlerName = GetRequestVal("HandlerName");
            //if (DataType.IsNullOrEmpty(writeImg) == false)
            //   writeImg = HttpUtility.UrlDecode(writeImg, Encoding.UTF8);//writeImg.Replace('~', '+');
            string writeStamp = GetRequestVal("WriteStamp");
            string dotype = GetRequestVal("ShowType");
            string doc = GetRequestVal("Doc");
            bool isCC = GetRequestVal("IsCC") == "1";

            string fwcView = null;
            if (DataType.IsNullOrEmpty(GetRequestVal("FWCView")) == false)
                fwcView = "@FWCView=" + GetRequestVal("FWCView").Trim();
            //查看时取消保存
            if (dotype != null && dotype == "View")
                return "";

            //内容为空，取消保存，20170727取消此处限制
            //if (DataType.IsNullOrEmpty(doc.Trim()))
            //    return "";

            string val = string.Empty;
            NodeWorkCheck wcDesc = new NodeWorkCheck(this.NodeID);
            if (DataType.IsNullOrEmpty(wcDesc.FWCFields) == false)
            {
                //循环属性获取值
                Attrs fwcAttrs = new Attrs();
                // Attrs fwcAttrs = new Attrs(wcDesc.FWCFields);

                foreach (Attr attr in fwcAttrs)
                {
                    if (attr.UIContralType == UIContralType.TB)
                    {
                        val = GetRequestVal("TB_" + attr.Key);

                        msg += attr.Key + "=" + val + ";";
                    }
                    else if (attr.UIContralType == UIContralType.CheckBok)
                    {
                        val = GetRequestVal("CB_" + attr.Key);

                        msg += attr.Key + "=" + Convert.ToInt32(val) + ";";
                    }
                    else if (attr.UIContralType == UIContralType.DDL)
                    {
                        val = GetRequestVal("DDL_" + attr.Key);

                        msg += attr.Key + "=" + val + ";";
                    }
                }
            }
            else
            {
                // 加入审核信息.
                msg = doc;
            }

            //在审核人打开后，申请人撤销，就不不能让其保存.(在途页面不做判断)
            string sql = "SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID;
            if (DBAccess.RunSQLReturnValInt(sql) != this.NodeID && handlerName.IndexOf("WF_MyView") == -1)
                return "err@当前工作已经被撤销或者已经移动到下一个节点您不能在执行审核.";

            // 处理人大的需求，需要把审核意见写入到FlowNote里面去.
            sql = "UPDATE WF_GenerWorkFlow SET FlowNote='" + msg + "' WHERE WorkID=" + this.WorkID;
            DBAccess.RunSQL(sql);

            // 判断是否是抄送?
            if (isCC)
            {
                // 写入审核信息，有可能是update数据。
                Dev2Interface.Node_WriteWorkCheck(this.WorkID, msg, wcDesc.FWCOpLabel, wcDesc.SigantureEnabel == 2 ? writeImg : "");

                //设置抄送状态 - 已经审核完毕.
                Dev2Interface.Node_CC_SetSta(this.NodeID, this.WorkID, WebUser.No, CCSta.CheckOver);

                sql = "SELECT MyPK,RDT FROM ND" + int.Parse(this.FlowNo) + "Track WHERE WorkID=" + this.WorkID + " AND NDFrom = " + this.NodeID + " AND ActionType = " + (int)ActionType.WorkCheck + " AND EmpFrom = '" + WebUser.No + "'";
                DataTable dtt = DBAccess.RunSQLReturnTable(sql, 1, 1, "MyPK", "RDT", "DESC");
                //判断是不是签章
                if (DataType.IsNullOrEmpty(writeStamp) == false)
                {
                    //writeStamp = HttpUtility.UrlDecode(writeStamp, Encoding.UTF8);// writeStamp.Replace('~', '+');
                    string mypk = dtt.Rows.Count > 0 ? dtt.Rows[0]["MyPK"].ToString() : "";
                    if (DataType.IsNullOrEmpty(mypk) == false)
                    {
                        DBAccess.SaveBigTextToDB(writeStamp, "ND" + int.Parse(this.FlowNo) + "Track", "MyPK", mypk, "FrmDB");
                    }
                }
                return dtt.Rows.Count > 0 ? dtt.Rows[0]["RDT"].ToString() : ""; ;
            }

            #region 根据类型写入数据  qin
            if (wcDesc.HisFrmWorkCheckType == FWCType.Check)  //审核组件
            {
                //判断是否审核组件中“协作模式下操作员显示顺序”设置为“按照接受人员列表先后顺序(官职大小)”，删除原有的空审核信息
                if (wcDesc.FWCOrderModel == FWCOrderModel.SqlAccepter)
                {
                    sql = "DELETE FROM ND" + int.Parse(this.FlowNo) + "Track WHERE WorkID = " + this.WorkID +
                          " AND ActionType = " + (int)ActionType.WorkCheck + " AND NDFrom = " + this.NodeID +
                          " AND NDTo = " + this.NodeID + " AND EmpFrom = '" + WebUser.No + "'";
                    DBAccess.RunSQL(sql);
                }

                Dev2Interface.Node_WriteWorkCheck(this.WorkID, msg, wcDesc.FWCOpLabel,
                    wcDesc.SigantureEnabel == 2 || wcDesc.SigantureEnabel == 3 || wcDesc.SigantureEnabel == 5 ? writeImg : "", fwcView);
            }

            if (wcDesc.HisFrmWorkCheckType == FWCType.DailyLog)//日志组件
            {
                Dev2Interface.WriteTrackDailyLog(this.FlowNo, this.NodeID, wcDesc.Name, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);
            }
            if (wcDesc.HisFrmWorkCheckType == FWCType.WeekLog)//周报
            {
                Dev2Interface.WriteTrackWeekLog(this.FlowNo, this.NodeID, wcDesc.Name, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);
            }
            if (wcDesc.HisFrmWorkCheckType == FWCType.MonthLog)//月报
            {
                Dev2Interface.WriteTrackMonthLog(this.FlowNo, this.NodeID, wcDesc.Name, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);
            }
            #endregion

            sql = "SELECT MyPK,RDT FROM ND" + int.Parse(this.FlowNo) + "Track WHERE WorkID=" + this.WorkID + " AND NDFrom = " + this.NodeID + " AND ActionType = " + (int)ActionType.WorkCheck + " AND EmpFrom = '" + WebUser.No + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql, 1, 1, "MyPK", "RDT", "DESC");
            //判断是不是签章
            if (DataType.IsNullOrEmpty(writeStamp) == false)
            {
                //writeStamp = HttpUtility.UrlDecode(writeStamp, Encoding.UTF8);// writeStamp.Replace('~', '+');
                string mypk = dt.Rows.Count > 0 ? dt.Rows[0]["MyPK"].ToString() : "";
                if (DataType.IsNullOrEmpty(mypk) == false)
                {
                    DBAccess.SaveBigTextToDB(writeStamp, "ND" + int.Parse(this.FlowNo) + "Track", "MyPK", mypk, "FrmDB");
                }
            }

            return dt.Rows.Count > 0 ? dt.Rows[0]["RDT"].ToString() : "";
        }
        #endregion

        #region 工作分配.
        /// <summary>
        /// 分配工作
        /// </summary>
        /// <returns></returns>
        public string AllotTask_Init()
        {
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.NodeID, true);
            return wls.ToJson();
        }
        /// <summary>
        /// 分配工作
        /// </summary>
        /// <returns></returns>
        public string AllotTask_Save()
        {
            return "";
        }
        #endregion

        #region 执行跳转.
        /// <summary>
        /// 返回可以跳转的节点.
        /// </summary>
        /// <returns></returns>
        public string FlowSkip_Init()
        {
            Node nd = new Node(this.NodeID);
            BP.WF.Template.BtnLab lab = new BtnLab(this.NodeID);

            string sql = "SELECT NodeID,Name FROM WF_Node WHERE FK_Flow='" + this.FlowNo + "'";
            switch (lab.JumpWayEnum)
            {
                case JumpWay.Previous:
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE NodeID IN (SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " )";
                    break;
                case JumpWay.Next:
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE NodeID NOT IN (SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " ) AND FK_Flow='" + this.FlowNo + "'";
                    break;
                case JumpWay.AnyNode:
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE FK_Flow='" + this.FlowNo + "' ORDER BY STEP";
                    break;
                case JumpWay.JumpSpecifiedNodes:
                    sql = nd.JumpToNodes;
                    sql = sql.Replace("@WebUser.No", WebUser.No);
                    sql = sql.Replace("@WebUser.Name", WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_Dept", WebUser.DeptNo);
                    if (sql.Contains("@"))
                    {
                        Work wk = nd.HisWork;
                        wk.OID = this.WorkID;
                        wk.RetrieveFromDBSources();
                        foreach (Attr attr in wk.EnMap.Attrs)
                        {
                            if (sql.Contains("@") == false)
                                break;
                            sql = sql.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                        }
                    }
                    break;
                case JumpWay.CanNotJump:
                    return "err@此节点不允许跳转.";
                default:
                    return "err@未判断";
            }

            sql = sql.Replace("~", "'");
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //如果是oracle,就转成小写.
            if (SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["NODEID"].ColumnName = "NodeID";
                dt.Columns["NAME"].ColumnName = "Name";
            }

            if (SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["nodeid"].ColumnName = "NodeID";
                dt.Columns["name"].ColumnName = "Name";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 执行跳转
        /// </summary>
        /// <returns></returns>
        public string FlowSkip_Do()
        {
            try
            {
                Node ndJump = new Node(this.GetRequestValInt("GoNode"));
                BP.WF.WorkNode wn = new BP.WF.WorkNode(this.WorkID, this.NodeID);
                string msg = wn.NodeSend(ndJump, null).ToMsgOfHtml();
                return msg;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion 执行跳转.

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {

                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到.");
        }
        #endregion 执行父类的重写方法.

        #region 抄送Adv.
        /// <summary>
        /// 选择权限组
        /// </summary>
        /// <returns></returns>
        public string CCAdv_SelectGroups()
        {
            string sql = "SELECT NO,NAME FROM GPM_Group ORDER BY IDX";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 抄送初始化.
        /// </summary>
        /// <returns></returns>
        public string CCAdv_Init()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Hashtable ht = new Hashtable();
            ht.Add("Title", gwf.Title);

            //计算出来曾经抄送过的人.
            Paras ps = new Paras();
            ps.SQL = "SELECT CCToName FROM WF_CCList WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("FK_Node", this.NodeID);
            ps.Add("WorkID", this.WorkID);
            DataTable mydt = DBAccess.RunSQLReturnTable(ps);
            string toAllEmps = "";
            foreach (DataRow dr in mydt.Rows)
                toAllEmps += dr[0].ToString() + ",";

            ht.Add("CCTo", toAllEmps);

            // 根据他判断是否显示权限组。
            if (DBAccess.IsExitsObject("GPM_Group") == true)
                ht.Add("IsGroup", "1");
            else
                ht.Add("IsGroup", "0");

            //返回流程标题.
            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        /// <summary>
        /// 选择部门呈现信息.
        /// </summary>
        /// <returns></returns>
        public string CCAdv_SelectDepts()
        {
            BP.Port.Depts depts = new BP.Port.Depts();
            depts.RetrieveAll();
            return depts.ToJson();
        }
        /// <summary>
        /// 选择部门呈现信息.
        /// </summary>
        /// <returns></returns>
        public string CCAdv_SelectStations()
        {
            //角色类型.
            string sql = "SELECT NO,NAME FROM Port_StationType ORDER BY NO";
            DataSet ds = new DataSet();
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Port_StationType";
            ds.Tables.Add(dt);

            //角色.
            string sqlStas = "SELECT NO,NAME,FK_STATIONTYPE FROM Port_Station ORDER BY FK_STATIONTYPE,NO";
            DataTable dtSta = DBAccess.RunSQLReturnTable(sqlStas);
            dtSta.TableName = "Port_Station";
            ds.Tables.Add(dtSta);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 抄送发送.
        /// </summary>
        /// <returns></returns>
        public string CCAdv_Send()
        {
            //人员信息. 格式 zhangsan,张三;lisi,李四;
            string emps = this.GetRequestVal("Emps");

            //角色信息. 格式:  001,002,003,
            string stations = this.GetRequestVal("Stations");
            stations = stations.Replace(";", ",");

            //权限组. 格式:  001,002,003,
            string groups = this.GetRequestVal("Groups");
            if (groups == null)
                groups = "";
            groups = groups.Replace(";", ",");

            //部门信息.  格式: 001,002,003,
            string depts = this.GetRequestVal("Depts");
            //标题.
            string title = this.GetRequestVal("TB_Title");
            //内容.
            string doc = this.GetRequestVal("TB_Doc");

            //调用抄送接口执行抄送.
            string ccRec = BP.WF.Dev2Interface.Node_CC_WriteTo_CClist(this.NodeID, this.WorkID, title, doc, emps, depts, stations, groups);

            if (ccRec == "")
                return "没有抄送到任何人。";
            else
                return "本次抄送给如下人员：" + ccRec;
            //return "执行抄送成功.emps=(" + emps + ")  depts=(" + depts + ") stas=(" + stations + ") 标题:" + title + " ,抄送内容:" + doc;
        }
        #endregion 抄送Adv.

        #region 抄送普通的抄送.
        public string CC_AddEmps()
        {
            string toEmpStrs = this.GetRequestVal("AddEmps");
            toEmpStrs = toEmpStrs.Replace(",", ";");
            string[] toEmps = toEmpStrs.Split(';');
            string infos = "";
            foreach (string empStr in toEmps)
            {
                if (DataType.IsNullOrEmpty(empStr) == true)
                    continue;

                BP.Port.Emp emp = new BP.Port.Emp(empStr);

                CCList cc = new CCList();
                cc.FlowNo = this.FlowNo;
                cc.NodeIDWork = this.NodeID;
                cc.WorkID = this.WorkID;
                cc.RecEmpNo = emp.No;
                cc.RecEmpName = emp.Name;

                cc.Insert();
            }

            return "";
        }

        /// <summary>
        /// 抄送发送.
        /// </summary>
        /// <returns></returns>
        public string CC_Send()
        {
            //人员信息. 格式 zhangsan,张三;lisi,李四;
            string emps = this.GetRequestVal("Emps");

            //角色信息. 格式:  001,002,003,
            string stations = this.GetRequestVal("Stations");
            if (DataType.IsNullOrEmpty(stations) == false)
                stations = stations.Replace(";", ",");

            //权限组. 格式:  001,002,003,
            string groups = this.GetRequestVal("Groups");
            if (groups == null)
                groups = "";
            groups = groups.Replace(";", ",");

            //部门信息.  格式: 001,002,003,
            string depts = this.GetRequestVal("Depts");
            //标题.
            string title = this.GetRequestVal("TB_Title");
            //内容.
            string doc = this.GetRequestVal("TB_Doc");

            //调用抄送接口执行抄送.
            string ccRec = BP.WF.Dev2Interface.Node_CC_WriteTo_CClist(this.NodeID, this.WorkID, title, doc, emps, depts, stations, groups);

            //该节点上设置为未启动.
            DBAccess.RunSQL("UPDATE WF_CCList SET Sta=-1 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + this.NodeID);

            if (ccRec == "")
                return "没有抄送到任何人。";
            else
                return "本次抄送给如下人员：" + ccRec;
            //return "执行抄送成功.emps=(" + emps + ")  depts=(" + depts + ") stas=(" + stations + ") 标题:" + title + " ,抄送内容:" + doc;
        }
        #endregion 抄送普通的抄送.


        public string DeleteFlowInstance_Init()
        {
            if (BP.WF.Dev2Interface.Flow_IsCanDeleteFlowInstance(this.FlowNo,
                this.WorkID, BP.Web.WebUser.No) == false)
                return "err@您没有删除该流程的权限";
            //获取节点中配置的流程删除规则
            if (this.NodeID != 0)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT wn.DelEnable FROM WF_Node wn WHERE wn.NodeID = " + SystemConfig.AppCenterDBVarStr + "NodeID";
                ps.Add("NodeID", this.NodeID);
                return DBAccess.RunSQLReturnValInt(ps) + "";
            }

            return "删除成功.";
        }

        public string DeleteFlowInstance_DoDelete()
        {
            if (BP.WF.Dev2Interface.Flow_IsCanDeleteFlowInstance(this.FlowNo,
                this.WorkID, BP.Web.WebUser.No) == false)
                return "err@您没有删除该流程的权限.";

            string deleteWay = this.GetRequestVal("RB_DeleteWay");
            string doc = this.GetRequestVal("TB_Doc");

            //是否要删除子流程？ 这里注意变量的获取方式，你可以自己定义.
            string isDeleteSubFlow = this.GetRequestVal("CB_IsDeleteSubFlow");

            bool isDelSubFlow = false;
            if (isDeleteSubFlow == "1")
                isDelSubFlow = true;

            //按照标记删除.
            if (deleteWay == "1")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(this.WorkID, doc, isDelSubFlow);

            //彻底删除.
            if (deleteWay == "3")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.WorkID, isDelSubFlow);

            //彻底并放入到删除轨迹里.
            if (deleteWay == "2")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByWriteLog(this.FlowNo, this.WorkID, doc, isDelSubFlow);

            return "流程删除成功.";
        }
        /// <summary>
        /// 获得节点表单数据.
        /// </summary>
        /// <returns></returns>
        //public string ViewWorkNodeFrm()
        //{
        //    Node nd = new Node(this.NodeID);
        //    nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

        //    Hashtable ht = new Hashtable();
        //    ht.Add("FormType", nd.FormType.ToString());
        //    ht.Add("Url", nd.FormUrl + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FlowNo + "&FK_Node=" + this.NodeID);

        //    if (nd.FormType == NodeFormType.SDKForm)
        //        return BP.Tools.Json.ToJsonEntityModel(ht);

        //    if (nd.FormType == NodeFormType.SelfForm)
        //        return BP.Tools.Json.ToJsonEntityModel(ht);

        //    //表单模版.
        //    DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(nd.NodeFrmID);
        //    string json = BP.WF.Dev2Interface.CCFrom_GetFrmDBJson(this.FlowNo, this.MyPK);
        //    DataTable mainTable = BP.Tools.Json.ToDataTableOneRow(json);
        //    mainTable.TableName = "MainTable";
        //    myds.Tables.Add(mainTable);

        //    //MapExts exts = new MapExts(nd.HisWork.ToString());
        //    //DataTable dtMapExt = exts.ToDataTableDescField();
        //    //dtMapExt.TableName = "Sys_MapExt";
        //    //myds.Tables.Add(dtMapExt);

        //    return BP.Tools.Json.ToJson(myds);
        //}

        /// <summary>
        /// 回复加签信息.
        /// </summary>
        /// <returns></returns>
        public string AskForRe()
        {
            string note = this.GetRequestVal("Note"); //原因.
            return BP.WF.Dev2Interface.Node_AskforReply(this.WorkID, note);
        }
        /// <summary>
        /// 执行加签
        /// </summary>
        /// <returns>执行信息</returns>
        public string Askfor()
        {
            Int64 workID = int.Parse(this.GetRequestVal("WorkID")); //工作ID
            string toEmp = this.GetRequestVal("ToEmp"); //让谁加签?
            string note = this.GetRequestVal("Note"); //原因.
            string model = this.GetRequestVal("Model"); //模式.

            BP.WF.AskforHelpSta sta = BP.WF.AskforHelpSta.AfterDealSend;
            if (model == "1")
                sta = BP.WF.AskforHelpSta.AfterDealSendByWorker;

            return BP.WF.Dev2Interface.Node_Askfor(workID, sta, toEmp, note);
        }
        /// <summary>
        /// 人员选择器
        /// </summary>
        /// <returns></returns>
        public string SelectEmps_Init()
        {
            string fk_dept = this.DeptNo;
            if (DataType.IsNullOrEmpty(fk_dept) == true || fk_dept.Equals("undefined") == true)
                fk_dept = BP.Web.WebUser.DeptNo;

            DataSet ds = new DataSet();

            string sql = "";
            if (SystemConfig.AppCenterDBType == DBType.DM)
                sql = "SELECT \"No\",Name,ParentNo FROM Port_Dept WHERE No='" + fk_dept + "' OR ParentNo='" + fk_dept + "' ORDER BY Idx";
            else
                sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE No='" + fk_dept + "' OR ParentNo='" + fk_dept + "' ORDER BY Idx";

            //如果是节水公司的.
            //if (SystemConfig.CustomerNo == "TianYe" && WebUser.FK_Dept.IndexOf("18099") == -1)
            //    sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE  No='" + fk_dept + "'  OR (ParentNo='" + fk_dept + "' AND No!='18099') ORDER BY Idx ";

            DataTable dtDept = DBAccess.RunSQLReturnTable(sql);
            if (dtDept.Rows.Count == 0)
            {
                fk_dept = BP.Web.WebUser.DeptNo;
                if (SystemConfig.AppCenterDBType == DBType.DM)
                    sql = "SELECT \"No\",Name,ParentNo FROM Port_Dept WHERE No='" + fk_dept + "' OR ParentNo='" + fk_dept + "' ORDER BY Idx ";
                else
                    sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE No='" + fk_dept + "' OR ParentNo='" + fk_dept + "' ORDER BY Idx ";
                dtDept = DBAccess.RunSQLReturnTable(sql);
            }

            dtDept.TableName = "Depts";
            ds.Tables.Add(dtDept);

            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtDept.Columns[0].ColumnName = "No";
                dtDept.Columns[1].ColumnName = "Name";
                dtDept.Columns[2].ColumnName = "ParentNo";
            }


            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                if (SystemConfig.AppCenterDBType == DBType.DM)
                    sql = "SELECT distinct CONCAT('Emp_',A.No ) AS \"No\", A.Name, '" + fk_dept + "' as FK_Dept, a.Idx FROM Port_Emp A LEFT JOIN Port_DeptEmp B  ON A.No=B.FK_Emp WHERE A.FK_Dept='" + fk_dept + "' OR B.FK_Dept='" + fk_dept + "' ";
                else
                    sql = "SELECT distinct CONCAT('Emp_',A.No ) AS No, A.Name, '" + fk_dept + "' as FK_Dept, a.Idx FROM Port_Emp A LEFT JOIN Port_DeptEmp B  ON A.No=B.FK_Emp WHERE A.FK_Dept='" + fk_dept + "' OR B.FK_Dept='" + fk_dept + "' ";
                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                {
                    if (SystemConfig.AppCenterDBType == DBType.DM)
                        sql = "SELECT distinct 'Emp_'+A.No AS \"No\", A.Name, '" + fk_dept + "' as FK_Dept, a.Idx FROM Port_Emp A LEFT JOIN Port_DeptEmp B  ON A.No=B.FK_Emp WHERE A.FK_Dept='" + fk_dept + "' OR B.FK_Dept='" + fk_dept + "' ";
                    else
                        sql = "SELECT distinct 'Emp_'+A.No AS No, A.Name, '" + fk_dept + "' as FK_Dept, a.Idx FROM Port_Emp A LEFT JOIN Port_DeptEmp B  ON A.No=B.FK_Emp WHERE A.FK_Dept='" + fk_dept + "' OR B.FK_Dept='" + fk_dept + "' ";
                }
                sql += " ORDER BY A.Idx ";
            }
            else
            {
                // 原来的SQL sql = "SELECT distinct CONCAT('Emp_', A.No) AS No,A.Name, '" + fk_dept + "' as FK_Dept, a.Idx FROM Port_Emp A LEFT JOIN Port_DeptEmp B  ON A.No=B.FK_Emp WHERE A.FK_Dept='" + fk_dept + "' OR B.FK_Dept='" + fk_dept + "' ";

                if (SystemConfig.AppCenterDBType == DBType.DM)
                {
                    sql = "SELECT CONCAT('Emp_',No) AS \"No\", Name, FK_Dept,Idx FROM Port_Emp WHERE FK_Dept='" + fk_dept + "'";
                    sql += " UNION ALL ";
                    sql += "SELECT CONCAT('Emp_',A.No) AS \"No\",A.Name, A.FK_Dept,A.Idx FROM Port_Emp A, Port_DeptEmp B WHERE A.No=B.FK_Emp AND B.FK_Dept='" + fk_dept + "'";
                }
                else
                {
                    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    {
                        sql = "SELECT  'Emp_'+ No AS No, Name, FK_Dept,Idx FROM Port_Emp WHERE FK_Dept='" + fk_dept + "'";
                        sql += " UNION ";
                        sql += "SELECT  'Emp_'+ A.No AS No,A.Name, '" + fk_dept + "' as FK_Dept, a.Idx FROM Port_Emp A ,Port_DeptEmp B  WHERE A.No=B.FK_Emp AND B.FK_Dept='" + fk_dept + "' ";
                    }
                    else
                    {
                        sql = "SELECT CONCAT('Emp_',No) AS No, Name, FK_Dept,Idx FROM Port_Emp WHERE FK_Dept='" + fk_dept + "'";
                        sql += " UNION ";
                        sql += "SELECT CONCAT('Emp_',A.No) AS No,A.Name, A.FK_Dept,A.Idx FROM Port_Emp A, Port_DeptEmp B WHERE A.No=B.FK_Emp AND B.FK_Dept='" + fk_dept + "'";
                    }

                }
                sql = "SELECT Distinct * FROM (" + sql + ")A Order By Idx";
            }

            DataTable dtEmps = DBAccess.RunSQLReturnTable(sql);
            dtEmps.TableName = "Emps";
            ds.Tables.Add(dtEmps);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtEmps.Columns[0].ColumnName = "No";
                dtEmps.Columns[1].ColumnName = "Name";
                dtEmps.Columns[2].ColumnName = "FK_Dept";
            }

            //转化为 json 
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 处理发送初始化已经选择的人员信息.
        /// </summary>
        /// <returns></returns>
        public string SendWorkOpt_Init()
        {
            string sql = "";
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sql = "SELECT top 1 MyPK FROM WF_WorkOpt WHERE NodeID=" + this.NodeID + " AND ToNodeID=" + this.ToNodeID + " AND EmpNo='" + BP.Web.WebUser.No + "' AND WorkID!=" + this.WorkID + " ORDER BY StartRDT DESC ";
                    break;
                case DBType.MySQL:
                case DBType.UX:
                case DBType.PostgreSQL:
                case DBType.HGDB:
                    sql = "SELECT MyPK FROM WF_WorkOpt WHERE NodeID=" + this.NodeID + " AND ToNodeID=" + this.ToNodeID + " AND EmpNo='" + BP.Web.WebUser.No + "' AND WorkID!=" + this.WorkID + " ORDER BY StartRDT DESC  limit 1";
                    break;
                case DBType.Oracle:
                case DBType.DM:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = "SELECT MyPK FROM WF_WorkOpt WHERE NodeID=" + this.NodeID + " AND ToNodeID=" + this.ToNodeID + " AND EmpNo='" + BP.Web.WebUser.No + "' AND WorkID!=" + this.WorkID + " AND ROWNUM = 1 ORDER BY StartRDT DESC ";
                    break;
                default:
                    throw new Exception("err@没有判断的数据库类型");
            }

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            NodeSimple toND = new NodeSimple(this.ToNodeID);
            WorkOpt en = new WorkOpt();

            string val = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);
            if (val == null)
            {
                en.WorkID = this.WorkID;
                en.FlowNo = this.FlowNo;

                string[] strs = this.MyPK.Split('_');
                en.EmpNo = BP.Web.WebUser.No;
                en.NodeID = this.NodeID;
                en.ToNodeID = this.ToNodeID;
                en.FlowNo = this.FlowNo;
                en.WorkID = this.WorkID;
                en.MyPK = this.MyPK;

                //流程信息.
                en.SetValByKey(WorkOptAttr.SendRDT, gwf.SendDT);//发送日期.
                en.SetValByKey(WorkOptAttr.SendSDT, gwf.SDTOfNode); //节点应完成时间.
                en.SetValByKey(WorkOptAttr.SenderName, gwf.Sender); //发送人.
                en.SetValByKey(WorkOptAttr.Title, gwf.Title);

                en.SetValByKey(WorkOptAttr.NodeName, gwf.NodeName);
                en.SetValByKey(WorkOptAttr.ToNodeName, toND.Name);

                en.SetValByKey(WorkOptAttr.ToNodeName, toND.Name); //到达的节点名称.

                //如果是开始节点.
                if (en.NodeID.ToString().EndsWith("01") == true)
                {
                    en.SetValByKey(WorkOptAttr.SendSDT, "无"); //节点应完成时间.
                    en.SetValByKey(WorkOptAttr.SendRDT, "无");//发送日期.
                }

                en.Save();
                return "info@执行成功.";
            }

            //加载上一笔数据，让其可以自动记录上一次的处理人。
            WorkOpt opt = new WorkOpt(val);

            en = new WorkOpt(this.MyPK);
            en.Copy(opt);
            en.WorkID = this.WorkID;
            en.FlowNo = this.FlowNo;
            en.MyPK = this.MyPK;

            //流程信息.
            en.SetValByKey(WorkOptAttr.SendRDT, gwf.SendDT);
            en.SetValByKey(WorkOptAttr.SendSDT, gwf.SDTOfNode);
            en.SetValByKey(WorkOptAttr.SenderName, gwf.Sender);
            en.SetValByKey(WorkOptAttr.Title, gwf.Title);

            en.SetValByKey(WorkOptAttr.NodeName, gwf.NodeName);
            en.SetValByKey(WorkOptAttr.ToNodeName, toND.Name);

            //如果是开始节点.
            if (en.NodeID.ToString().EndsWith("01") == true)
            {
                en.SetValByKey(WorkOptAttr.SendSDT, "无"); //节点应完成时间.
                en.SetValByKey(WorkOptAttr.SendRDT, "无");//发送日期.
            }

            en.Update();
            return "info@执行成功.";
        }

        #region 选择接受人.
        /// <summary>
        /// 初始化接受人.
        /// </summary>
        /// <returns></returns>
        public string Accepter_Init()
        {
            /*如果是协作模式, 就要检查当前是否主持人, 当前是否是会签模式. */
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.NodeID != this.NodeID && this.NodeID != 0)
                return "err@当前流程已经运动到[" + gwf.NodeName + "]上,当前处理人员为[" + gwf.TodoEmps + "]，this.NodeID=" + this.NodeID;

            //当前节点ID.
            Node nd = new Node(gwf.NodeID);

            //判断当前是否是协作模式.
            if (nd.TodolistModel == TodolistModel.Teamup && nd.ItIsStartNode == false)
            {
                if (gwf.TodoEmps.Contains(WebUser.No + ","))
                {
                    /* 说明我是主持人之一, 我就可以选择接受人,发送到下一个节点上去. */
                }
                else
                {
                    //  string err= "err@流程配置逻辑错误，当前节点是协作模式，当前节点的方向条件不允许[发送按钮旁下拉框选择(默认模式)].";
                    //  err += "，如果需要手工选择，请使用[节点属性]-[设置方向条件]-[按照用户执行发送后手工选择计算]模式计算.";
                    //  return err;

                    /* 不是主持人就执行发送，返回发送结果. */

                    //判断是否有不发送标记？
                    if (this.GetRequestValBoolen("IsSend") == true)
                    {
                        SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(gwf.FlowNo, this.WorkID);
                        return "info@" + objs.ToMsgOfHtml();
                    }
                }
            }

            int toNodeID = this.GetRequestValInt("ToNode");
            if (toNodeID == 0)
            {
                Nodes nds = nd.HisToNodes;
                if (nds.Count == 1)
                    toNodeID = nds[0].GetValIntByKey("NodeID");
                else
                    return "err@参数错误,必须传递来到达的节点ID ToNode .";
            }

            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();

            Selector select = new Selector(toNodeID);

            if (select.SelectorModel == SelectorModel.GenerUserSelecter)
                return "url@AccepterOfGener.htm?WorkID=" + this.WorkID + "&FK_Node=" + gwf.NodeID + "&FK_Flow=" + nd.FlowNo + "&ToNode=" + toNodeID + "&PWorkID=" + gwf.PWorkID + "&PageName=AccepterOfGener";

            if (select.SelectorModel == SelectorModel.AccepterOfDeptStationEmp)
                return "url@AccepterOfDeptStationEmp.htm?WorkID=" + this.WorkID + "&FK_Node=" + gwf.NodeID + "&FK_Flow=" + nd.FlowNo + "&ToNode=" + toNodeID + "&PWorkID=" + gwf.PWorkID + "&PageName=AccepterOfDeptStationEmp";

            if (select.SelectorModel == SelectorModel.Url)
                return "BySelfUrl@" + select.SelectorP1 + "?WorkID=" + this.WorkID + "&FK_Node=" + gwf.NodeID + "&FK_Flow=" + nd.FlowNo + "&ToNode=" + toNodeID + "&PWorkID=" + gwf.PWorkID + "&PageName=" + select.SelectorP1;

            //获得 部门与人员.
            DataSet ds = select.GenerDataSet(toNodeID, wk);

            if (SystemConfig.CustomerNo == "TianYe") //天业集团，去掉00000001董事长
            {
            }

            //增加判断.
            if (ds.Tables["Emps"].Rows.Count == 0)
                return "err@配置接受人范围为空,请联系管理员.";

            ////只有一个人，就让其发送下去.
            //if (ds.Tables["Emps"].Rows.Count == 1)
            //{
            //    string emp = ds.Tables["Emps"].Rows[0][0].ToString();
            //    SendReturnObjs objs= BP.WF.Dev2Interface.Node_SendWork(this.FlowNo, this.WorkID, toNodeID, emp);
            //    return  "info@"+objs.ToMsgOfText();
            //}

            #region 计算上一次选择的结果, 并把结果返回过去.
            string sql = "";
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.TableName = "Selected";
            if (select.ItIsAutoLoadEmps == true)
            {
                //  WorkOpt en = new WorkOpt();
                //   en.GetValFloatByKey
                //   en.Insert();

                //获取当前节点的SelectAccper的值
                SelectAccpers selectAccpers = new SelectAccpers();
                selectAccpers.Retrieve(SelectAccperAttr.WorkID, this.WorkID, SelectAccperAttr.FK_Node, toNodeID);
                if (selectAccpers.Count != 0)
                {
                    foreach (SelectAccper sa in selectAccpers)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = sa.EmpNo;
                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                        sql = "SELECT  top 1 Tag,EmpTo FROM ND" + int.Parse(nd.FlowNo) + "Track A WHERE A.NDFrom=" + this.NodeID + " AND A.NDTo=" + toNodeID + " AND ActionType=1 ORDER BY WorkID DESC";
                    else if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.KingBaseR3 || SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                        sql = "SELECT * FROM (SELECT  Tag,EmpTo,WorkID FROM ND" + int.Parse(nd.FlowNo) + "Track A WHERE A.NDFrom=" + this.NodeID + " AND A.NDTo=" + toNodeID + " AND ActionType=1 ORDER BY WorkID DESC ) WHERE ROWNUM =1";
                    else if (SystemConfig.AppCenterDBType == DBType.MySQL)
                        sql = "SELECT  Tag,EmpTo FROM ND" + int.Parse(nd.FlowNo) + "Track A WHERE A.NDFrom=" + this.NodeID + " AND A.NDTo=" + toNodeID + " AND ActionType=1 ORDER BY WorkID  DESC limit 1,1 ";
                    else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || SystemConfig.AppCenterDBType == DBType.UX)
                        sql = "SELECT  Tag,EmpTo FROM ND" + int.Parse(nd.FlowNo) + "Track A WHERE A.NDFrom=" + this.NodeID + " AND A.NDTo=" + toNodeID + " AND ActionType=1 ORDER BY WorkID  DESC limit 1 ";

                    DataTable mydt = DBAccess.RunSQLReturnTable(sql);
                    string emps = "";
                    if (mydt.Rows.Count != 0)
                    {
                        emps = mydt.Rows[0]["Tag"].ToString();
                        if (emps == "" || emps == null)
                        {
                            emps = mydt.Rows[0]["EmpTo"].ToString();
                            emps = emps + "," + emps;
                        }
                    }

                    string[] strs = emps.Split(';');
                    foreach (string str in strs)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;

                        string[] emp = str.Split(',');
                        if (emp.Length != 2)
                            continue;

                        DataRow dr = dt.NewRow();
                        dr[0] = emp[0];
                        dr[1] = DBAccess.RunSQLReturnString("SELECT Name FROM Port_Emp WHERE No='" + emp[0] + "'");
                        dt.Rows.Add(dr);
                    }
                }
            }

            //增加一个table.
            ds.Tables.Add(dt);
            #endregion 计算上一次选择的结果, 并把结果返回过去.

            //返回json.
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 保存.
        /// </summary>
        /// <returns></returns>
        public string Accepter_Save()
        {
            try
            {
                //求到达的节点. 
                int toNodeID = 0;
                if (this.GetRequestVal("ToNode") != "0")
                    toNodeID = int.Parse(this.GetRequestVal("ToNode"));

                if (toNodeID == 0)
                {   //没有就获得第一个节点.
                    Node nd = new Node(this.NodeID);
                    Nodes nds = nd.HisToNodes;
                    toNodeID = nds[0].GetValIntByKey("NodeID");
                }

                //求发送到的人员.
                // string selectEmps = this.GetValFromFrmByKey("SelectEmps");
                string selectEmps = this.GetRequestVal("SelectEmps");
                selectEmps = selectEmps.Replace(";", ",");

                //保存接受人.
                BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, selectEmps, true);
                return "SaveOK@" + selectEmps;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string Accepter_Send()
        {
            //求到达的节点. 
            int toNodeID = 0;
            if (this.GetRequestVal("ToNode") != "0")
                toNodeID = int.Parse(this.GetRequestVal("ToNode"));

            string emps = this.GetRequestVal("SelectEmps");
            string ahther = this.GetRequestVal("Auther");

            return Accepter_SendExt(this.WorkID, toNodeID, emps, ahther);
        }
        /// <summary>
        /// 执行保存并发送.
        /// </summary>
        /// <returns>返回发送的结果.</returns>
        private string Accepter_SendExt(Int64 workid, int toNodeID, string selectEmps, string Auther)
        {
            try
            {
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                int currNodeID = gwf.NodeID;

                if (toNodeID == 0)
                {   //没有就获得第一个节点.
                    Node nd = new Node(gwf.NodeID);
                    Nodes nds = nd.HisToNodes;
                    toNodeID = nds[0].GetValIntByKey("NodeID");
                }
                selectEmps = selectEmps.Replace(";", ",");

                #region 处理授权人.
                //授权人
                string auther = this.GetRequestVal("Auther");
                if (DataType.IsNullOrEmpty(auther) == false)
                {
                    //  BP.Web.WebUser.IsAuthorize = true;
                    BP.Web.WebUser.Auth = auther;
                    BP.Web.WebUser.AuthName = BP.DA.DBAccess.RunSQLReturnString("SELECT Name FROM Port_Emp WHERE No='" + auther + "'");
                }
                else
                {
                    // BP.Web.WebUser.IsAuthorize = true;
                    BP.Web.WebUser.Auth = "";
                    BP.Web.WebUser.AuthName = ""; // BP.DA.DBAccess.RunSQLReturnString("SELECT Name FROM Port_Emp WHERE No='" + auther + "'");
                }
                #endregion 处理授权人.


                //执行发送.
                SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(gwf.FlowNo,
                    workid, toNodeID, selectEmps);

                string msg = objs.ToMsgOfHtml();
                #region 处理授权 
                gwf = new GenerWorkFlow(workid);
                if (DataType.IsNullOrEmpty(auther) == false)
                {
                    gwf.SetPara("Auth", BP.Web.WebUser.AuthName + "授权");
                    gwf.Update();
                }
                #endregion 处理授权 

                //当前节点.
                Node currNode = new Node(currNodeID);

                #region 处理发送后转向.
                /*处理转向问题.*/
                switch (currNode.HisTurnToDeal)
                {
                    case TurnToDeal.SpecUrl:
                        string myurl = currNode.TurnToDealDoc.Clone().ToString();
                        if (myurl.Contains("?") == false)
                            myurl += "?1=1";
                        Attrs myattrs = currNode.HisWork.EnMap.Attrs;
                        Work hisWK = currNode.HisWork;
                        foreach (Attr attr in myattrs)
                        {
                            if (myurl.Contains("@") == false)
                                break;
                            myurl = myurl.Replace("@" + attr.Key, hisWK.GetValStrByKey(attr.Key));
                        }
                        myurl = myurl.Replace("@WebUser.No", BP.Web.WebUser.No);
                        myurl = myurl.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        myurl = myurl.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);

                        if (myurl.Contains("@"))
                        {
                            BP.WF.Dev2Interface.Port_SendMsg("admin", currNode.FlowName + "在" + currNode.Name + "节点处，出现错误", "流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl, "Err" + currNode.No + "_" + gwf.WorkID, SMSMsgType.Err, gwf.FlowNo, gwf.NodeID, gwf.WorkID, gwf.FID);
                            throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl);
                        }

                        if (myurl.Contains("PWorkID") == false)
                            myurl += "&PWorkID=" + gwf.PWorkID;

                        if (myurl.Contains("WorkID") == false)
                            myurl += "&WorkID=" + gwf.WorkID;

                        myurl += "&FromFlow=" + gwf.FlowNo + "&FromNode=" + gwf.NodeID + "&UserNo=" + WebUser.No + "&Token=" + WebUser.Token;
                        return "TurnUrl@" + myurl;

                    case TurnToDeal.TurnToByCond:

                        return msg;
                    default:
                        msg = msg.Replace("@WebUser.No", BP.Web.WebUser.No);
                        msg = msg.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        msg = msg.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);
                        return msg;
                }
                #endregion 处理发送后转向.
                return msg;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion

        #region 回滚.
        /// <summary>
        /// 回滚操作.
        /// </summary>
        /// <returns></returns>
        public string Rollback_Init()
        {
            string andsql = " ";
            andsql += "  ActionType=" + (int)ActionType.Start;
            andsql += " OR ActionType=" + (int)ActionType.TeampUp;
            andsql += " OR ActionType=" + (int)ActionType.Forward;
            andsql += " OR ActionType=" + (int)ActionType.HuiQian;
            andsql += " OR ActionType=" + (int)ActionType.ForwardFL;
            andsql += " OR ActionType=" + (int)ActionType.FlowOver;
            andsql += " OR ActionType=" + (int)ActionType.Skip;

            string sql = "SELECT RDT,NDFrom, NDFromT,EmpFrom,EmpFromT  FROM ND" + int.Parse(this.FlowNo) + "Track WHERE WorkID=" + this.WorkID + " AND(" + andsql + ") Order By RDT DESC";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "RDT";
                dt.Columns[1].ColumnName = "NDFrom";
                dt.Columns[2].ColumnName = "NDFromT";
                dt.Columns[3].ColumnName = "EmpFrom";
                dt.Columns[4].ColumnName = "EmpFromT";
            }


            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 执行回滚操作
        /// </summary>
        /// <returns></returns>
        public string Rollback_Done()
        {
            FlowExt flow = new FlowExt(this.FlowNo);
            return flow.DoRebackFlowData(this.WorkID, this.NodeID, this.GetRequestVal("Msg"));
        }
        #endregion 回滚.

        #region 工作退回.
        /// <summary>
        /// 获得可以退回的节点.
        /// </summary>
        /// <returns>退回信息</returns>
        public string Return_Init()
        {
            try
            {

                DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(this.WorkID);

                //如果只有一个退回节点，就需要判断是否启用了单节点退回规则.
                if (dt.Rows.Count == 1)
                {
                    Node nd = new Node(this.NodeID);
                    if (nd.ReturnOneNodeRole != 0)
                    {
                        /* 如果:启用了单节点退回规则.
                         */
                        string returnMsg = "";
                        if (nd.ReturnOneNodeRole == 1 && DataType.IsNullOrEmpty(nd.ReturnField) == false)
                        {
                            /*从表单字段里取意见.*/
                            Work wk = nd.HisWork;
                            wk.OID = this.WorkID;
                            wk.RetrieveFromDBSources();
                            if (wk.EnMap.Attrs.Contains(nd.ReturnField) == false)
                                return "err@系统设置错误，您设置的单节点退回，退回信息字段拼写错误或者不存在" + nd.ReturnField;
                            returnMsg = wk.GetValStrByKey(nd.ReturnField);
                            // DBAccess.RunSQLReturnStringIsNull(sql, "未填写意见");
                        }

                        if (nd.ReturnOneNodeRole == 2)
                        {
                            /*从审核组件里取意见.*/
                            string sql = "SELECT Msg FROM ND" + int.Parse(nd.FlowNo) + "Track WHERE WorkID=" + this.WorkID + " AND NDFrom=" + this.NodeID + " AND EmpFrom='" + WebUser.No + "' AND ActionType=" + (int)ActionType.WorkCheck;
                            returnMsg = DBAccess.RunSQLReturnStringIsNull(sql, "未填写意见");
                        }

                        int toNodeID = int.Parse(dt.Rows[0][0].ToString());

                        string info = BP.WF.Dev2Interface.Node_ReturnWork(this.FlowNo, this.WorkID, 0, this.NodeID, toNodeID, returnMsg, false);
                        return "info@" + info;
                    }
                }
                return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行退回,返回退回信息.
        /// </summary>
        /// <returns></returns>
        public string DoReturnWork()
        {
            string returnToNode = this.GetRequestVal("ReturnToNode");
            string[] vals = returnToNode.Split(new string[] { "=>" }, StringSplitOptions.None);
            if(vals.Length<2)
                vals = returnToNode.Split('@');
            int toNodeID = int.Parse(vals[0]);

            string toEmp = vals[1];
            string reMesage = this.GetRequestVal("ReturnInfo");

            bool isBackBoolen = false;
            if (this.GetRequestVal("IsBack").Equals("1") == true)
                isBackBoolen = true;

            bool isKill = false; //是否全部退回.
            string isKillEtcThread = this.GetRequestVal("IsKillEtcThread");
            if (DataType.IsNullOrEmpty(isKillEtcThread) == false && isKillEtcThread.Equals("1") == true)
                isKill = true;

            string pageData = this.GetRequestVal("PageData");

            return BP.WF.Dev2Interface.Node_ReturnWork(this.WorkID, toNodeID, toEmp, reMesage, isBackBoolen, pageData, isKill);
        }
        #endregion

        /// <summary>
        /// 执行移交.
        /// </summary>
        /// <returns></returns>
        public string Shift_Save()
        {
            string msg = this.GetRequestVal("Message");
            string toEmp = this.GetRequestVal("ToEmp");
            return BP.WF.Dev2Interface.Node_Shift(this.WorkID, toEmp, msg);
        }
        /// <summary>
        /// 撤销移交
        /// </summary>
        /// <returns></returns>
        public string UnShift()
        {
            return BP.WF.Dev2Interface.Node_ShiftUn(this.WorkID);
        }
        /// <summary>
        /// 执行催办
        /// </summary>
        /// <returns></returns>
        public string Press()
        {
            string msg = this.GetRequestVal("Msg");
            //调用API.
            return BP.WF.Dev2Interface.Flow_DoPress(this.WorkID, msg, true);
        }

        #region 流程数据模版. for 浙商银行 by zhoupeng.
        /// <summary>
        /// 流程数据模版
        /// </summary>
        /// <returns></returns>
        public string DBTemplate_Init()
        {
            DataSet ds = new DataSet();
            //获取模版.
            Paras ps = new Paras();
            //按关键字查询模板的Title
            string SeachType = this.GetRequestVal("SeachType");
            string SearchKey = this.GetRequestVal("SearchKey");
            string SQL = "";

            if (SeachType == "2")
            {
                string[] SearchKeys = SearchKey.Split(',');
                SQL = "SELECT WorkID,Title,AtPara FROM WF_GenerWorkFlow WHERE FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow AND(";
                for (int i = 0; i < SearchKeys.Length; i++)
                {
                    if (SearchKeys[i] != "")
                        SQL += " Title like '%" + SearchKeys[i] + "%' OR";
                }
                if (SQL.Substring(SQL.Length - 2, 2) == "OR")
                    SQL = SQL.Remove(SQL.Length - 2, 2);

                SQL += " )AND WFState = 3  AND ATPARA LIKE '%@DBTemplate=1%'";

            }
            if (SeachType == "1")
            {
                SQL = "SELECT WorkID,Title,AtPara FROM WF_GenerWorkFlow WHERE FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow AND Title like '%" + SearchKey + "%' AND  WFState=3 AND ATPARA LIKE '%@DBTemplate=1%'";
            }
            else if (SeachType == "0")
            {
                SQL = "SELECT WorkID,Title,AtPara FROM WF_GenerWorkFlow WHERE FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow AND WFState=3 AND ATPARA LIKE '%@DBTemplate=1%' ";
            }


            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.KingBaseR3 || SystemConfig.AppCenterDBType == DBType.KingBaseR6)
            {
                SQL += " AND rownum<=15 ORDER BY RDT";
            }
            if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || SystemConfig.AppCenterDBType == DBType.UX)
            {
                SQL += " ORDER BY RDT LIMIT 15";
            }
            if (SystemConfig.AppCenterDBType == DBType.MSSQL)
            {
                SQL.Replace("SELECT ", " ");
                SQL = "SELECT TOP 15 " + SQL;
            }

            ps.SQL = SQL;
            ps.Add("FK_Flow", this.FlowNo);
            //ps.Add("Starter", WebUser.No);
            DataTable dtTemplate = DBAccess.RunSQLReturnTable(ps);
            dtTemplate.TableName = "DBTemplate";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtTemplate.Columns[0].ColumnName = "WorkID";
                dtTemplate.Columns[1].ColumnName = "Title";
            }

            //把模版名称替换 title. 
            foreach (DataRow dr in dtTemplate.Rows)
            {
                string str = dr[2].ToString();
                AtPara ap = new AtPara(str);
                string dbtemplateName = ap.GetValStrByKey("DBTemplateName");
                if (DataType.IsNullOrEmpty(dbtemplateName) == false)
                    dr["Title"] = dbtemplateName;
            }

            ds.Tables.Add(dtTemplate);

            // 获取历史发起数据.
            ps = new Paras();
            if (SystemConfig.AppCenterDBType == DBType.MSSQL)
            {
                ps.SQL = "SELECT TOP 30 WorkID,Title FROM WF_GenerWorkFlow WHERE FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow AND WFState=3 AND FK_Dept=" + SystemConfig.AppCenterDBVarStr + "Starter AND ATPARA NOT LIKE '%@DBTemplate=1%' ORDER BY RDT ";
                ps.Add("FK_Flow", this.FlowNo);
                ps.Add("FK_Dept", WebUser.DeptNo);
            }
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.KingBaseR3 || SystemConfig.AppCenterDBType == DBType.KingBaseR6)
            {
                ps.SQL = "SELECT WorkID,Title FROM WF_GenerWorkFlow WHERE FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow AND WFState=3 AND FK_Dept=" + SystemConfig.AppCenterDBVarStr + "Starter AND ATPARA NOT LIKE '%@DBTemplate=1%' AND rownum<=30 ORDER BY RDT ";
                ps.Add("FK_Flow", this.FlowNo);
                ps.Add("FK_Dept", WebUser.DeptNo);
            }
            if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || SystemConfig.AppCenterDBType == DBType.UX)
            {
                ps.SQL = "SELECT WorkID,Title FROM WF_GenerWorkFlow WHERE FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow AND WFState=3 AND FK_Dept=" + SystemConfig.AppCenterDBVarStr + "Starter AND ATPARA NOT LIKE '%@DBTemplate=1%' ORDER BY RDT LIMIT 30";
                ps.Add("FK_Flow", this.FlowNo);
                ps.Add("FK_Dept", WebUser.DeptNo);
            }
            DataTable dtHistroy = DBAccess.RunSQLReturnTable(ps);
            dtHistroy.TableName = "History";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtHistroy.Columns[0].ColumnName = "WorkID";
                dtHistroy.Columns[1].ColumnName = "Title";
            }
            ds.Tables.Add(dtHistroy);

            //转化为 json.
            return BP.Tools.Json.ToJson(ds);

        }

        public string DBTemplate_SaveAsDBTemplate()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.Paras_DBTemplate = true;
            gwf.Paras_DBTemplateName = HttpUtility.UrlDecode(this.GetRequestVal("Title"), System.Text.Encoding.UTF8);//this.GetRequestVal("Title");
            gwf.Update();
            return "设置成功";
        }

        public string DBTemplate_DeleteDBTemplate()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.Paras_DBTemplate = false;
            gwf.Update();

            return "设置成功";
        }

        public string DBTemplate_StartFlowAsWorkID()
        {
            return "设置成功";
        }
        #endregion 流程数据模版.

        #region tonodes
        /// <summary>
        /// 初始化.
        /// </summary>
        /// <returns></returns>
        public string ToNodes_Init()
        {
            //获取到下一个节点的节点Nodes

            //获得当前节点到达的节点.
            Nodes nds = new Nodes();
            string toNodes = this.GetRequestVal("ToNodes");
            if (DataType.IsNullOrEmpty(toNodes) == false)
            {
                /*解决跳转问题.*/
                string[] mytoNodes = toNodes.Split(',');
                foreach (string str in mytoNodes)
                {
                    if (string.IsNullOrEmpty(str) == true)
                        continue;
                    nds.AddEntity(new Node(int.Parse(str)));
                }
            }
            else
            {
                nds = BP.WF.Dev2Interface.WorkOpt_GetToNodes(this.FlowNo, this.NodeID, this.WorkID, this.FID);
            }

            //获得上次默认选择的节点
            int lastSelectNodeID = BP.WF.Dev2Interface.WorkOpt_ToNodes_GetLasterSelectNodeID(this.FlowNo, this.NodeID);
            if (lastSelectNodeID == 0 && nds.Count != 0)
                lastSelectNodeID = int.Parse(nds[0].PKVal.ToString());


            DataSet ds = new DataSet();
            ds.Tables.Add(nds.ToDataTableField("Nodes"));
            DataTable dt = new DataTable("SelectNode");
            dt.Columns.Add("NodeID");
            DataRow dr = dt.NewRow();
            dr["NodeID"] = lastSelectNodeID;
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);
            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        public string ToNodes_Send()
        {
            string toNodes = this.GetRequestVal("ToNodes");
            // 执行发送.
            string msg = "";
            Node nd = new Node(this.NodeID);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();

            try
            {
                string toNodeStr = int.Parse(this.FlowNo) + "01";
                //如果为开始节点
                if (toNodeStr == toNodes)
                {
                    //把参数更新到数据库里面.
                    GenerWorkFlow gwf = new GenerWorkFlow();
                    gwf.WorkID = this.WorkID;
                    gwf.RetrieveFromDBSources();
                    gwf.Paras_ToNodes = toNodes;
                    gwf.Save();

                    WorkNode firstwn = new WorkNode(wk, nd);

                    Node toNode = new Node(toNodeStr);
                    msg = firstwn.NodeSend(toNode, gwf.Starter).ToMsgOfHtml();
                }
                else
                {
                    msg = BP.WF.Dev2Interface.WorkOpt_SendToNodes(this.FlowNo,
                        this.NodeID, this.WorkID, this.FID, toNodes).ToMsgOfHtml();
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

            GenerWorkFlow gwfw = new GenerWorkFlow();
            gwfw.WorkID = this.WorkID;
            gwfw.RetrieveFromDBSources();
            if (nd.ItIsRememberMe == true)
                gwfw.Paras_ToNodes = toNodes;
            else
                gwfw.Paras_ToNodes = "";
            gwfw.Save();

            //当前节点.
            Node currNode = new Node(this.NodeID);
            Flow currFlow = new Flow(this.FlowNo);

            #region 处理发送后转向.
            try
            {
                /*处理转向问题.*/
                switch (currNode.HisTurnToDeal)
                {
                    case TurnToDeal.SpecUrl:
                        string myurl = currNode.TurnToDealDoc.Clone().ToString();
                        if (myurl.Contains("?") == false)
                            myurl += "?1=1";
                        Attrs myattrs = currNode.HisWork.EnMap.Attrs;
                        Work hisWK = currNode.HisWork;
                        foreach (Attr attr in myattrs)
                        {
                            if (myurl.Contains("@") == false)
                                break;
                            myurl = myurl.Replace("@" + attr.Key, hisWK.GetValStrByKey(attr.Key));
                        }
                        myurl = myurl.Replace("@WebUser.No", BP.Web.WebUser.No);
                        myurl = myurl.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        myurl = myurl.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);

                        if (myurl.Contains("@"))
                        {
                            BP.WF.Dev2Interface.Port_SendMsg("admin", currFlow.Name + "在" + currNode.Name + "节点处，出现错误", "流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl, "Err" + currNode.No + "_" + this.WorkID, SMSMsgType.Err, this.FlowNo, this.NodeID, this.WorkID, this.FID);
                            throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl);
                        }

                        if (myurl.Contains("PWorkID") == false)
                            myurl += "&PWorkID=" + this.WorkID;

                        myurl += "&FromFlow=" + this.FlowNo + "&FromNode=" + this.NodeID + "&UserNo=" + WebUser.No + "&Token=" + WebUser.Token;
                        return "TurnUrl@" + myurl;
                    case TurnToDeal.TurnToByCond:

                        return msg;
                    default:
                        msg = msg.Replace("@WebUser.No", BP.Web.WebUser.No);
                        msg = msg.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        msg = msg.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);
                        return msg;
                }
                #endregion

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("请选择下一步骤工作") == true || ex.Message.Contains("用户没有选择发送到的节点") == true)
                {
                    if (currNode.CondModel == DirCondModel.ByLineCond)
                        return "url@./WorkOpt/ToNodes.htm?FK_Flow=" + this.FlowNo + "&FK_Node=" + this.NodeID + "&WorkID=" + this.WorkID + "&FID=" + this.FID;

                    return "err@下一个节点的接收人规则是，当前节点选择来选择，在当前节点属性里您没有启动接受人按钮，系统自动帮助您启动了，请关闭窗口重新打开。" + ex.Message;
                }

                GenerWorkFlow HisGenerWorkFlow = new GenerWorkFlow(this.WorkID);
                //防止发送失败丢失接受人，导致不能出现下拉方向选择框. @杜.
                if (HisGenerWorkFlow != null)
                {
                    //如果是会签状态.
                    if (HisGenerWorkFlow.HuiQianTaskSta == HuiQianTaskSta.HuiQianing)
                    {
                        //如果是主持人.
                        if (HisGenerWorkFlow.HuiQianZhuChiRen == WebUser.No)
                        {
                            string empStrSepc = BP.Web.WebUser.No + "," + BP.Web.WebUser.Name + ";";
                            if (HisGenerWorkFlow.TodoEmps.Contains(empStrSepc) == false)
                            {
                                HisGenerWorkFlow.TodoEmps += empStrSepc;
                                HisGenerWorkFlow.Update();
                            }
                        }
                        else
                        {
                            //非主持人.
                            string empStrSepc = BP.Web.WebUser.No + "," + BP.Web.WebUser.Name + ";";
                            if (HisGenerWorkFlow.TodoEmps.Contains(empStrSepc) == false)
                            {
                                HisGenerWorkFlow.TodoEmps += empStrSepc;
                                HisGenerWorkFlow.Update();
                            }
                        }
                    }


                    if (HisGenerWorkFlow.HuiQianTaskSta != HuiQianTaskSta.HuiQianing)
                    {
                        if (HisGenerWorkFlow.TodoEmps.Contains(BP.Web.WebUser.No + ",") == false)
                        {
                            HisGenerWorkFlow.TodoEmps += WebUser.No + "," + BP.Web.WebUser.Name + ";";
                            HisGenerWorkFlow.Update();
                        }
                    }
                }
                return ex.Message;

            }
        }
        #endregion tonodes

        #region 自定义.
        public string TZWorkerRole_Init()
        {
            DataSet ds = new DataSet();

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.TransferCustomType != TransferCustomType.ByWorkerSet)
            {
                gwf.TransferCustomType = TransferCustomType.ByWorkerSet;
                gwf.Update();
            }

            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            //当前运行到的节点
            Node currNode = new Node(gwf.NodeID);

            //所有的节点s.
            BtnLabs nds = new BtnLabs();
            nds.Retrieve("FK_Flow", this.FlowNo, "NodeID");
            ds.Tables.Add(nds.ToDataTableField("WF_Node"));

            //工作人员列表.已经走完的节点与人员.
            GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID);
            GenerWorkerList gwln = (GenerWorkerList)gwls.GetEntityByKey(GenerWorkerListAttr.FK_Node, this.NodeID);
            if (gwln == null)
            {
                gwln = new GenerWorkerList();
                gwln.NodeID = currNode.NodeID;
                gwln.NodeName = currNode.Name;
                gwln.EmpNo = WebUser.No;
                gwln.EmpName = WebUser.Name;
                gwls.AddEntity(gwln);
            }
            ds.Tables.Add(gwls.ToDataTableField("WF_GenerWorkerlist"));

            SelectAccpers accpers = new SelectAccpers();
            accpers.Retrieve("WorkID", this.WorkID, "FK_Node");

            ds.Tables.Add(accpers.ToDataTableField("WF_SelectAccpers"));

            if (ds.Tables["WF_SelectAccpers"].Rows.Count > 0)
            {
                // 创建一个用于存储匹配行的列表
                List<DataRow> matchedRows = new List<DataRow>();

                // 遍历第一个DataTable的行
                foreach (DataRow row1 in ds.Tables["WF_GenerWorkFlow"].Rows)
                {
                    long fkNodeValue1 = Convert.ToInt64(row1["FK_Node"]);

                    // 遍历第二个DataTable的行
                    foreach (DataRow row2 in ds.Tables["WF_SelectAccpers"].Rows)
                    {
                        long fkNodeValue2 = Convert.ToInt64(row2["FK_Node"]);

                        // 检查FK_Node字段值是否相同
                        if (fkNodeValue1 != fkNodeValue2)
                        {
                            // 将匹配的行添加到列表中
                            matchedRows.Add(row2);
                        }
                    }
                }
                if (matchedRows.Count > 0)
                {
                    return BP.Tools.Json.ToJson(ds);
                }
            }

            #region 执行计算未来处理人.

            Work wk = currNode.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();
            WorkNode wn = new WorkNode(wk, currNode);
            wn.HisFlow.ItIsFullSA = true;

            //执行计算未来处理人.
            FullSA fsa = new FullSA();
            fsa.DoIt2023(wn);
            #endregion 执行计算未来处理人.

            accpers.Retrieve("WorkID", this.WorkID, "FK_Node");

            ds.Tables.Add(accpers.ToDataTableField("WF_SelectAccpers"));

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 重置未来计算人
        /// </summary>
        /// <returns></returns>
        public string TZWorkerRole_Reset()
        {
            DataSet ds = new DataSet();

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.TransferCustomType != TransferCustomType.ByWorkerSet)
            {
                gwf.TransferCustomType = TransferCustomType.ByWorkerSet;
                gwf.Update();
            }

            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            //当前运行到的节点
            Node currNode = new Node(gwf.NodeID);

            //所有的节点s.
            BtnLabs nds = new BtnLabs();
            nds.Retrieve("FK_Flow", this.FlowNo, "NodeID");
            ds.Tables.Add(nds.ToDataTableField("WF_Node"));

            //工作人员列表.已经走完的节点与人员.
            GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID);
            GenerWorkerList gwln = (GenerWorkerList)gwls.GetEntityByKey(GenerWorkerListAttr.FK_Node, this.NodeID);
            if (gwln == null)
            {
                gwln = new GenerWorkerList();
                gwln.NodeID = currNode.NodeID;
                gwln.NodeName = currNode.Name;
                gwln.EmpNo = WebUser.No;
                gwln.EmpName = WebUser.Name;
                gwls.AddEntity(gwln);
            }
            ds.Tables.Add(gwls.ToDataTableField("WF_GenerWorkerlist"));
            #region 执行计算未来处理人.

            Work wk = currNode.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();
            WorkNode wn = new WorkNode(wk, currNode);
            wn.HisFlow.ItIsFullSA = true;

            //执行计算未来处理人.
            FullSA fsa = new FullSA();
            fsa.DoIt2023(wn);
            #endregion 执行计算未来处理人.

            SelectAccpers accpers = new SelectAccpers();
            accpers.Retrieve("WorkID", this.WorkID, "FK_Node");

            ds.Tables.Add(accpers.ToDataTableField("WF_SelectAccpers"));

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string TransferCustom_Init()
        {
            DataSet ds = new DataSet();

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.TransferCustomType != TransferCustomType.ByWorkerSet)
            {
                gwf.TransferCustomType = TransferCustomType.ByWorkerSet;
                gwf.Update();
            }

            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            //当前运行到的节点
            Node currNode = new Node(gwf.NodeID);

            //所有的节点s.
            Nodes nds = new Nodes(this.FlowNo);
            //ds.Tables.Add(nds.ToDataTableField("WF_Node"));

            //工作人员列表.已经走完的节点与人员.
            GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID);
            GenerWorkerList gwln = (GenerWorkerList)gwls.GetEntityByKey(GenerWorkerListAttr.FK_Node, this.NodeID);
            if (gwln == null)
            {
                gwln = new GenerWorkerList();
                gwln.NodeID = currNode.NodeID;
                gwln.NodeName = currNode.Name;
                gwln.EmpNo = WebUser.No;
                gwln.EmpName = WebUser.Name;
                gwls.AddEntity(gwln);
            }
            ds.Tables.Add(gwls.ToDataTableField("WF_GenerWorkerlist"));

            //设置的手工运行的流转信息.
            TransferCustoms tcs = new TransferCustoms(this.WorkID);
            if (tcs.Count == 0)
            {
                #region 执行计算未来处理人.

                Work wk = currNode.HisWork;
                wk.OID = this.WorkID;
                wk.Retrieve();
                WorkNode wn = new WorkNode(wk, currNode);
                wn.HisFlow.ItIsFullSA = true;

                //执行计算未来处理人.
                FullSA fsa = new FullSA();
                fsa.DoIt2023(wn);
                #endregion 执行计算未来处理人.

                foreach (Node nd in nds)
                {
                    if (nd.NodeID == this.NodeID)
                        continue;
                    if (nd.GetParaBoolen(NodeAttr.IsYouLiTai) == false)
                        continue;
                    Entity gwl = gwls.GetEntityByKey(GenerWorkerListAttr.FK_Node, nd.NodeID);
                    if (gwl == null)
                    {
                        /*说明没有 */
                        TransferCustom tc = new TransferCustom();
                        tc.WorkID = this.WorkID;
                        tc.NodeID = nd.NodeID;
                        tc.NodeName = nd.Name;

                        #region 计算出来当前节点的工作人员.
                        SelectAccpers sas = new SelectAccpers();
                        sas.Retrieve(SelectAccperAttr.WorkID, this.WorkID, SelectAccperAttr.FK_Node, nd.NodeID);

                        string workerID = "";
                        string workerName = "";
                        foreach (SelectAccper sa in sas)
                        {
                            workerID += sa.EmpNo + ",";
                            workerName += sa.EmpName + ",";
                        }
                        #endregion 计算出来当前节点的工作人员.

                        tc.Worker = workerID;
                        tc.WorkerName = workerName;
                        tc.Idx = nd.Step;
                        tc.ItIsEnable = true;
                        if (nd.HisCHWay == CHWay.ByTime && nd.GetParaInt("CHWayOfTimeRole") == 2)
                        {
                            tc.PlanDT = DataType.SysDateTimeFormat(DateTime.Now.AddDays(1));
                        }
                        tc.Insert();
                    }
                }
                tcs = new TransferCustoms(this.WorkID);
            }

            ds.Tables.Add(tcs.ToDataTableField("WF_TransferCustoms"));

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 自定义.

        #region 时限初始化数据
        public string CH_Init()
        {
            DataSet ds = new DataSet();

            //获取处理信息的列表
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FK_Flow, this.FlowNo, GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.RDT);
            DataTable dt = gwls.ToDataTableField("WF_GenerWorkerlist");
            ds.Tables.Add(dt);

            //获取流程信息
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            Flow flow = new Flow(this.FlowNo);
            ds.Tables.Add(flow.ToDataTableField("WF_Flow"));

            //获取流程流转自定义的数据
            string sql = "SELECT FK_Node AS NodeID,NodeName AS Name From WF_TransferCustom WHERE WorkID=" + WorkID + " AND IsEnable=1 Order By Idx";
            DataTable dtYL = DBAccess.RunSQLReturnTable(sql);

            //删除不启用的游离态节点时限设置
            sql = "DELETE FROM WF_CHNode WHERE WorkID=" + WorkID + " AND FK_Node IN(SELECT FK_Node FROM WF_TransferCustom WHERE WorkID=" + WorkID + " AND IsEnable=0 )";
            DBAccess.RunSQL(sql);

            //节点时限表
            CHNodes chNodes = new CHNodes(this.WorkID);



            #region 获取流程节点信息的列表
            Nodes nds = new Nodes(this.FlowNo);
            //如果是游离态的节点有可能调整顺序
            dt = new DataTable();
            dt.TableName = "WF_Node";
            dt.Columns.Add("NodeID");
            dt.Columns.Add("Name");
            dt.Columns.Add("SDTOfNode");//节点应完成时间
            dt.Columns.Add("PlantStartDt");//节点计划开始时间
            dt.Columns.Add("GS");//工时

            DataRow dr;
            bool isFirstY = true;
            //上一个节点的时间
            string beforeSDTOfNode = "";
            //先排序运行过的节点
            CHNode chNode = null;
            foreach (GenerWorkerList gwl in gwls)
            {
                chNode = chNodes.GetEntityByKey(CHNodeAttr.FK_Node, gwl.NodeID) as CHNode;
                if (chNode != null)
                {
                    chNode.SetPara("RDT", gwl.RDT);
                    chNode.SetPara("CDT", gwl.CDT);
                    chNode.SetPara("IsPass", gwl.ItIsPass);
                    chNode.StartDT = gwl.RDT;
                    chNode.EndDT = gwl.CDT;
                    chNode.EmpNo = gwl.EmpNo;
                    chNode.EmpT = gwl.EmpName;
                    continue;

                }
                chNode = new CHNode();
                chNode.WorkID = this.WorkID;
                chNode.NodeID = gwl.NodeID;
                chNode.NodeName = gwl.NodeName;
                chNode.StartDT = gwl.RDT;
                chNode.EndDT = gwl.CDT;
                chNode.EmpNo = gwl.EmpNo;
                chNode.EmpT = gwl.EmpName;
                chNode.SetPara("RDT", gwl.RDT);
                chNode.SetPara("CDT", gwl.CDT);
                chNode.SetPara("IsPass", gwl.ItIsPass);
                chNodes.AddEntity(chNode);
                beforeSDTOfNode = gwl.CDT;
            }
            foreach (Node node in nds)
            {
                GenerWorkerList gwl = gwls.GetEntityByKey(GenerWorkerListAttr.FK_Node, node.NodeID) as GenerWorkerList;
                if (gwl != null)
                    continue;

                //已经设定
                chNode = chNodes.GetEntityByKey(CHNodeAttr.FK_Node, node.NodeID) as CHNode;
                if (chNode != null)
                    continue;

                string sdtOfNode = "";
                string plantStartDt = "";
                if (node.GetParaBoolen("IsYouLiTai") == true)
                {
                    if (isFirstY == true)
                    {
                        foreach (DataRow drYL in dtYL.Rows)
                        {
                            chNode = chNodes.GetEntityByKey(CHNodeAttr.FK_Node, int.Parse(drYL["NodeID"].ToString())) as CHNode;
                            if (chNode != null)
                                continue;
                            chNode = new CHNode();
                            chNode.WorkID = this.WorkID;
                            chNode.NodeID = int.Parse(drYL["NodeID"].ToString());
                            chNode.NodeName = drYL["Name"].ToString();
                            //计划开始时间
                            plantStartDt = beforeSDTOfNode;
                            chNode.StartDT = plantStartDt;
                            //计划完成时间
                            sdtOfNode = sdtOfNode = getSDTOfNode(node, beforeSDTOfNode, gwf);
                            chNode.EndDT = sdtOfNode;
                            //工时
                            int gty = 0;
                            if (DataType.IsNullOrEmpty(plantStartDt) == false && DataType.IsNullOrEmpty(sdtOfNode) == false)
                                gty = DataType.SpanDays(plantStartDt, sdtOfNode, false);

                            chNode.GT = gty;

                            beforeSDTOfNode = sdtOfNode;
                            chNodes.AddEntity(chNode);
                        }
                        isFirstY = false;
                    }
                    continue;
                }
                chNode = new CHNode();
                chNode.WorkID = this.WorkID;
                chNode.NodeID = node.NodeID;
                chNode.NodeName = node.Name;

                //计划开始时间
                plantStartDt = beforeSDTOfNode;
                chNode.StartDT = plantStartDt;

                //计划完成时间
                sdtOfNode = getSDTOfNode(node, beforeSDTOfNode, gwf);
                chNode.EndDT = sdtOfNode;

                //计算初始值工天
                int gs = 0;
                if (DataType.IsNullOrEmpty(plantStartDt) == false && DataType.IsNullOrEmpty(sdtOfNode) == false)
                    gs = DataType.SpanDays(plantStartDt, sdtOfNode, false);
                chNode.GT = gs;
                beforeSDTOfNode = sdtOfNode;
                chNodes.AddEntity(chNode);

            }
            #endregion 流程节点信息

            ds.Tables.Add(chNodes.ToDataTableField("WF_CHNode"));
            //获取当前节点信息
            Node nd = new Node(this.NodeID);
            ds.Tables.Add(nd.ToDataTableField("WF_CurrNode"));


            #region 获取剩余天数
            Part part = new Part();
            part.setMyPK(nd.FlowNo + "_0_DeadLineRole");
            int count = part.RetrieveFromDBSources();
            int day = 0; //含假期的天数
            DateTime dateT = DateTime.Now;
            if (count > 0)
            {
                //判断是否包含假期
                if (int.Parse(part.Tag4) == 0)
                {
                    string holidays = BP.Sys.GloVar.Holidays;
                    while (true)
                    {
                        if (dateT.CompareTo(DataType.ParseSysDate2DateTime(gwf.SDTOfFlow)) >= 0)
                            break;

                        if (holidays.Contains(dateT.ToString("MM-dd")))
                        {
                            dateT = dateT.AddDays(1);
                            day++;
                            continue;
                        }
                        dateT = dateT.AddDays(1);
                    }

                }

            }
            string spanTime = GetSpanTime(DateTime.Now, DataType.ParseSysDate2DateTime(gwf.SDTOfFlow), day);
            dt = new DataTable();
            dt.TableName = "SpanTime";
            dt.Columns.Add("SpanTime");
            dr = dt.NewRow();
            dr["SpanTime"] = spanTime;
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);
            #endregion 获取剩余天数

            return BP.Tools.Json.ToJson(ds);
        }


        private string getSDTOfNode(Node node, string beforeSDTOfNode, GenerWorkFlow gwf)
        {
            DateTime SDTOfNode = DateTime.Now;
            if (beforeSDTOfNode == "")
                beforeSDTOfNode = gwf.SDTOfNode;
            //按天、小时考核
            if (node.GetParaInt("CHWayOfTimeRole") == 0)
            {
                //增加天数. 考虑到了节假日. 
                int timeLimit = node.TimeLimit;
                SDTOfNode = Glo.AddDayHoursSpan(DateTime.Parse(beforeSDTOfNode), node.TimeLimit,
                    node.TimeLimitHH, node.TimeLimitMM, node.TWay);
            }
            //按照节点字段设置
            if (node.GetParaInt("CHWayOfTimeRole") == 1)
            {
                //获取设置的字段、
                string keyOfEn = node.GetParaString("CHWayOfTimeRoleField");
                if (DataType.IsNullOrEmpty(keyOfEn) == true)
                    node.HisCHWay = CHWay.None;
                else
                    SDTOfNode = DataType.ParseSysDateTime2DateTime(node.HisWork.GetValByKey(keyOfEn).ToString());

            }
            return DataType.SysDateTimeFormat(SDTOfNode);
        }
        #endregion 时限初始化数据

        #region 节点时限重新设置
        public string CH_Save()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            //获取流程应完成时间
            string sdtOfFow = this.GetRequestVal("GWF");
            if (DataType.IsNullOrEmpty(sdtOfFow) == false && gwf.SDTOfFlow != sdtOfFow)
                gwf.SDTOfFlow = sdtOfFow;

            //获取节点的时限设置
            Nodes nds = new Nodes(this.FlowNo);
            CHNode chNode = null;
            foreach (Node nd in nds)
            {
                chNode = new CHNode();
                string startDT = this.GetRequestVal("StartDT_" + nd.NodeID);
                string endDT = this.GetRequestVal("EndDT_" + nd.NodeID);
                int gt = this.GetRequestValInt("GT_" + nd.NodeID);
                float scale = this.GetRequestValFloat("Scale_" + nd.NodeID);
                float totalScale = this.GetRequestValFloat("TotalScale_" + nd.NodeID);

                chNode.WorkID = this.WorkID;
                chNode.NodeID = nd.NodeID;
                chNode.NodeName = nd.Name;
                if (DataType.IsNullOrEmpty(startDT) == false)
                    chNode.StartDT = startDT;
                if (DataType.IsNullOrEmpty(endDT) == false)
                    chNode.EndDT = endDT;

                chNode.GT = gt;
                chNode.Scale = scale;
                chNode.TotalScale = totalScale;
                chNode.Save();
            }
            gwf.Update();
            return "保存成功";
        }
        #endregion 节点时限重新设置

        #region 节点备注的设置
        public string Note_Init()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(this.FlowNo) + "Track WHERE ActionType=" + SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            ps.Add("WorkID", this.WorkID);

            //转化成json
            return BP.Tools.Json.ToJson(DBAccess.RunSQLReturnTable(ps));
        }

        /// <summary>
        /// 保存备注.
        /// </summary>
        /// <returns></returns>
        public string Note_Save()
        {
            string msg = this.GetRequestVal("Msg");
            //需要删除track表中的数据是否存在备注
            string sql = "DELETE From ND" + int.Parse(this.FlowNo) + "Track WHERE WorkID=" + WorkID + " AND NDFrom=" + this.NodeID + " AND EmpFrom='" + WebUser.No + "' And ActionType=" + (int)ActionType.FlowBBS;
            DBAccess.RunSQL(sql);
            //增加track
            Node nd = new Node(this.NodeID);
            Glo.AddToTrack(ActionType.FlowBBS, this.FlowNo, this.WorkID, this.FID, nd.NodeID, nd.Name, WebUser.No, WebUser.Name, nd.NodeID, nd.Name, WebUser.No, WebUser.Name, msg, null);

            //发送消息
            string empsStrs = DBAccess.RunSQLReturnStringIsNull("SELECT Emps FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID, "");
            string[] myEmpStrs = empsStrs.Split('@');
            //标题
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            string title = "流程名称为" + gwf.FlowName + "标题为" + gwf.Title + "在节点增加备注说明" + msg;

            foreach (string emp in myEmpStrs)
            {
                if (DataType.IsNullOrEmpty(emp))
                    continue;
                //获得当前人的邮件.
                BP.WF.Port.WFEmp empEn = new BP.WF.Port.WFEmp(emp);

                BP.WF.Dev2Interface.Port_SendMsg(empEn.No, title, msg, null, "NoteMessage", this.FlowNo, this.NodeID, this.WorkID, this.FID);

            }
            return "保存成功";
        }

        #endregion 节点备注的设置

        private static string GetSpanTime(DateTime t1, DateTime t2, int day)
        {
            TimeSpan span = t2 - t1;
            int days = span.Days;
            int hours = span.Hours;
            int minutes = span.Minutes;

            if (days == 0 && hours == 0 && minutes == 0)
                minutes = span.Seconds > 0 ? 1 : 0;

            string spanStr = string.Empty;

            if (days > 0)
                spanStr += (days - day) + "天";

            if (hours > 0)
                spanStr += hours + "时";

            if (minutes > 0)
                spanStr += minutes + "分";

            if (spanStr.Length == 0)
                spanStr = "0分";

            return spanStr;
        }
        /// <summary>
        /// 流程的常用语
        /// </summary>
        /// <returns></returns>
        public string UsefulExpresFlow_Init()
        {
            //AttrKey =WorkCheck, FlowBBS, WorkReturn
            string attrKey = this.GetRequestVal("AttrKey");

            FastInputs ens = new FastInputs();
            ens.Retrieve(FastInputAttr.CfgKey, "Flow",
                FastInputAttr.EnsName, "Flow",
                FastInputAttr.AttrKey, attrKey,
                FastInputAttr.FK_Emp, WebUser.No);

            if (ens.Count > 0)
                return ens.ToJson();

            if (attrKey.Equals("Comment"))
            {
                FastInput en = new FastInput();
                en.setMyPK(DBAccess.GenerGUID());
                en.EnsName = "Flow";
                en.CfgKey = "Flow";
                en.AttrKey = attrKey;
                en.Vals = "已阅";
                en.EmpNo = WebUser.No;
                en.Insert();
            }
            if (attrKey.Equals("CYY"))
            {
                FastInput en = new FastInput();
                en.setMyPK(DBAccess.GenerGUID());
                en.EnsName = "Flow";
                en.CfgKey = "Flow";
                en.AttrKey = attrKey;
                en.Vals = "同意";
                en.EmpNo = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.setMyPK(DBAccess.GenerGUID());
                en.EnsName = "Flow";
                en.CfgKey = "Flow";
                en.AttrKey = attrKey;
                en.Vals = "不同意";
                en.EmpNo = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.setMyPK(DBAccess.GenerGUID());
                en.EnsName = "Flow";
                en.CfgKey = "Flow";
                en.AttrKey = attrKey;
                en.Vals = "同意，请领导批示";
                en.EmpNo = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.setMyPK(DBAccess.GenerGUID());
                en.EnsName = "Flow";
                en.CfgKey = "Flow";
                en.AttrKey = attrKey;

                en.Vals = "同意办理";
                en.EmpNo = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.setMyPK(DBAccess.GenerGUID());
                en.EnsName = "Flow";
                en.CfgKey = "Flow";
                en.AttrKey = attrKey;

                en.Vals = "情况属实报领导批准";
                en.EmpNo = WebUser.No;
                en.Insert();
            }

            ens.Retrieve(FastInputAttr.CfgKey, "Flow",
               FastInputAttr.EnsName, "Flow",
               FastInputAttr.AttrKey, attrKey,
               FastInputAttr.FK_Emp, WebUser.No);
            return ens.ToJson();
        }

        #region 
        /// <summary>
        /// 批量发起子流程.
        /// </summary>
        /// <returns></returns>
        public string SubFlowGuid_Send()
        {
            //获得选择的实体信息. 格式为: 001@运输司,002@法制司
            Log.DebugWriteInfo("开始时间:" + DataType.CurrentDateTimeCNOfLong);
            string selectNos = GetRequestVal("SelectNos");
            if (DataType.IsNullOrEmpty(selectNos) == true)
                return "err@没有选择需要启动子流程的信息";

            string subFlowMyPK = GetRequestVal("SubFlowMyPK");

            //前置导航的子流程的配置.
            SubFlowHandGuide subFlow = new SubFlowHandGuide(subFlowMyPK);
            SubFlowHand subFlowH = new SubFlowHand(subFlowMyPK);

            //选择的编号. selectNos格式为 001@开发司,002@运输司,
            string[] strs = selectNos.Split(',');
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            try
            {
                List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();
                HttpContext ctx = HttpContextHelper.Current;
                // 设置最大线程
                ThreadPool.SetMaxThreads(16, 16);
                // 设置最小线程
                ThreadPool.SetMinThreads(8, 8);
                int count = 0;
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    // str的格式为:002@运输司
                    string[] enNoName = str.Split('@');
                    if (enNoName.Length < 2)
                        return "err@" + enNoName[0] + "不存在名称";
                    count++;
                    //实例化同步工具
                    ManualResetEvent mre = new ManualResetEvent(false);
                    manualEvents.Add(mre);
                    //将任务放入线程池中，让线程池中的线程执行该任务
                    ThreadPool.QueueUserWorkItem(o =>
                    {
                        HttpContext.Current = ctx;
                        SendSingleSubFlow(subFlowH, ht, gwf);
                        mre.Set();//告诉主线程子线程执行完了,如果不给ManualResetEvent实例调用这个方法,主线程会一直等待子线程调用ManualResetEvent实例的Set方法
                    });
                    //mre.WaitOne();
                }
                WaitHandle.WaitAll(manualEvents.ToArray());
            }
            catch (Exception ex)
            {
                Log.DebugWriteError("发送子流程部分失败:" + ex.Message);
                return "err@发送子流程部分失败";
            }

            if (subFlow.SubFlowHidTodolist == true)
            {
                //发送子流程后不显示父流程待办，设置父流程已经的待办已经处理 100
                DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=100 Where WorkID=" + this.WorkID + " AND FK_Node=" + this.NodeID);

            }
            Log.DebugWriteInfo("结束时间:" + DataType.CurrentDateTimeCNOfLong);
            return "发起子流程成功";
        }
        public void SendSingleSubFlow(SubFlowHand subFlowH, Hashtable ht, GenerWorkFlow gwf)
        {
            //创建WORKID

            Log.DebugWriteInfo("线程开始:" + DataType.CurrentDateTimeCNOfLong);
            Thread.Sleep(1000);
            long workidSubFlow = 0;
            if (subFlowH.SubFlowModel == SubFlowModel.SubLevel)
                workidSubFlow = BP.WF.Dev2Interface.Node_CreateBlankWork(subFlowH.SubFlowNo, null, null, WebUser.No, null, this.WorkID, this.FID, this.FlowNo, this.NodeID);
            if (subFlowH.SubFlowModel == SubFlowModel.SameLevel)
                workidSubFlow = BP.WF.Dev2Interface.Node_CreateBlankWork(subFlowH.SubFlowNo, null, null, WebUser.No, null, gwf.PWorkID, gwf.PFID, gwf.PFlowNo, gwf.PNodeID);

            //设置父子关系
            if (subFlowH.SubFlowModel == SubFlowModel.SubLevel)
                BP.WF.Dev2Interface.SetParentInfo(subFlowH.SubFlowNo, workidSubFlow, this.WorkID, WebUser.No, this.NodeID);
            if (subFlowH.SubFlowModel == SubFlowModel.SameLevel)
            {

                BP.WF.Dev2Interface.SetParentInfo(subFlowH.SubFlowNo, workidSubFlow, gwf.PWorkID, WebUser.No, gwf.PNodeID); //父子关系
                                                                                                                            //设置同级关系
                GenerWorkFlow subgwf = new GenerWorkFlow(workidSubFlow);
                subgwf.SetPara("SLFlowNo", this.FlowNo);
                subgwf.SetPara("SLNodeID", this.NodeID);
                subgwf.SetPara("SLWorkID", this.WorkID);
                subgwf.SetPara("SLEmp", BP.Web.WebUser.No);
                subgwf.Update();

            }

            //发送子流程
            BP.WF.Dev2Interface.Node_SendWork(subFlowH.SubFlowNo, workidSubFlow);
            Log.DebugWriteInfo("线程结束:" + DataType.CurrentDateTimeCNOfLong);


        }

        /// <summary>
        /// 会签子流程-删除草稿
        /// </summary>
        /// <returns></returns>
        public string SubFlowGuid_DeleteSubFlowDraf()
        {
            Node nd = new Node(int.Parse(this.FlowNo + "01"));
            string sql = "SELECT PTable FROM Sys_MapData WHERE No='" + nd.NodeFrmID + "'";
            string pTableOfSubFlow = DBAccess.RunSQLReturnString(sql);

            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID);
            DBAccess.RunSQL("DELETE FROM " + pTableOfSubFlow + " WHERE OID=" + this.WorkID);
            return "删除成功";
        }
        #endregion 会签.


        public string JumpWay_Init()
        {
            Node node = new Node(this.NodeID);
            string sql = "";
            switch (node.JumpWay)
            {
                case JumpWay.CanNotJump://不跳转
                    break;
                case JumpWay.Previous://向前跳转
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE NodeID IN (SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " )";
                    break;
                case JumpWay.Next://向后跳转
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE NodeID NOT IN (SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " ) AND FK_Flow='" + this.FlowNo + "'";
                    break;
                case JumpWay.AnyNode://任意节点
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE FK_Flow='" + this.FlowNo + "' ORDER BY STEP";
                    break;
                case JumpWay.JumpSpecifiedNodes://指定节点
                    sql = node.JumpToNodes;
                    sql = sql.Replace("@WebUser.No", WebUser.No);
                    sql = sql.Replace("@WebUser.Name", WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_Dept", WebUser.DeptNo);
                    if (sql.Contains("@"))
                    {
                        Work wk = node.HisWork;
                        wk.OID = this.WorkID;
                        wk.RetrieveFromDBSources();
                        foreach (Attr attr in wk.EnMap.Attrs)
                        {
                            if (sql.Contains("@") == false)
                                break;
                            sql = sql.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                        }
                    }
                    break;
                default:
                    throw new Exception(node.JumpWay + "还未增加改类型的判断.");
            }
            sql = sql.Replace("~", "'");
            if (DataType.IsNullOrEmpty(sql) == false)
            {
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase
                    || SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        string colName = col.ColumnName.ToLower();
                        switch (colName)
                        {
                            case "nodeid":
                                col.ColumnName = "NodeID";
                                break;
                            case "name":
                                col.ColumnName = "Name";
                                break;
                            default:
                                break;
                        }
                    }
                }
                return BP.Tools.Json.ToJson(dt);
            }
            return "";
        }


        public string JumpWay_Send()
        {
            try
            {
                int toNodeID = this.GetRequestValInt("ToNode");

                SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FlowNo, this.WorkID, toNodeID, null);
                string strs = objs.ToMsgOfHtml();
                strs = strs.Replace("@", "<br>@");

                #region 处理发送后转向.
                //当前节点.
                Node currNode = new Node(this.NodeID);
                /*处理转向问题.*/
                switch (currNode.HisTurnToDeal)
                {
                    case TurnToDeal.SpecUrl:
                        string myurl = currNode.TurnToDealDoc.Clone().ToString();
                        if (myurl.Contains("?") == false)
                            myurl += "?1=1";
                        Attrs myattrs = currNode.HisWork.EnMap.Attrs;
                        Work hisWK = currNode.HisWork;
                        foreach (Attr attr in myattrs)
                        {
                            if (myurl.Contains("@") == false)
                                break;
                            myurl = myurl.Replace("@" + attr.Key, hisWK.GetValStrByKey(attr.Key));
                        }
                        myurl = myurl.Replace("@WebUser.No", BP.Web.WebUser.No);
                        myurl = myurl.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        myurl = myurl.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);

                        if (myurl.Contains("@"))
                        {
                            BP.WF.Dev2Interface.Port_SendMsg("admin", currNode.Name + "在" + currNode.Name + "节点处，出现错误", "流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl, "Err" + currNode.No + "_" + this.WorkID, SMSMsgType.Err, this.FlowNo, this.NodeID, this.WorkID, this.FID);
                            throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl);
                        }

                        if (myurl.Contains("PWorkID") == false)
                            myurl += "&PWorkID=" + this.WorkID;

                        myurl += "&FromFlow=" + this.FlowNo + "&FromNode=" + this.NodeID + "&UserNo=" + WebUser.No + "&Token=" + WebUser.Token;
                        return "TurnUrl@" + myurl;
                    case TurnToDeal.TurnToByCond:

                        return strs;
                    default:
                        strs = strs.Replace("@WebUser.No", BP.Web.WebUser.No);
                        strs = strs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        strs = strs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);
                        return strs;
                }
                #endregion

                return strs;
            }
            catch (Exception ex)
            {

                if (ex.Message.IndexOf("url@") != -1)
                    return ex.Message.Replace("/WorkOpt/", "/");
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 发送抄送审核处理
        /// </summary>
        /// <returns></returns>
        public string TS_Send()
        {
            //操作类型 发送，抄送（不做处理），发送+审核，发送+抄送+审核
            string operType = this.GetRequestVal("OperType");
            WorkOpt workOpt = new WorkOpt(this.MyPK);
            if (DataType.IsNullOrEmpty(operType) == true)
                return "err@不可能存在的错误,没有获取到参数:" + operType;

            #region 执行发送.
            //组合要发送到的人员编号：把部门人员，三者相加进来.
            string emps = workOpt.SendEmps;
            emps += ","+BP.Port.Glo.GenerEmpNosByDeptNos(workOpt.SendDepts);
            emps += BP.Port.Glo.GenerEmpNosByStationNos(workOpt.SendStas);
            if (DataType.IsNullOrEmpty(emps) == true)
                return "err@接受人不能为空.";

            //执行发送:
            string sendStr = this.Accepter_SendExt(workOpt.WorkID, workOpt.ToNodeID, emps, "");

            //写入小纸条.
            if (DataType.IsNullOrEmpty(workOpt.SendNote) == false)
            {
                GenerWorkFlow gwf = new GenerWorkFlow(workOpt.WorkID);
                gwf.SetPara("ScripNodeID", workOpt.NodeID);
                gwf.SetPara("ScripMsg", workOpt.SendNote);
                gwf.Update();
            }
            #endregion 执行发送.

            /*  #region 执行抄送.
              //组合要发送到的人员编号：把部门人员，三者相加进来.
              emps = workOpt.CCEmps;
              emps += BP.Port.Glo.GenerEmpNosByDeptNos(workOpt.CCDepts);
              emps += BP.Port.Glo.GenerEmpNosByStationNos(workOpt.CCStations);
              string ccMsg = BP.WF.Dev2Interface.Node_CCToEmps(workOpt.NodeID, workOpt.WorkID, emps, "来自" + BP.Web.WebUser.Name + "的抄送", workOpt.CCNote);
              #endregion 执行抄送.*/

            return sendStr;
        }



        private void SaveWorkCheck(WorkOpt workOpt)
        {
            //暂时未处理
        }
        private void SendWork(WorkOpt workOpt)
        {
            //执行发送.

        }

    }
}
