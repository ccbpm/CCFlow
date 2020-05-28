﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.WF.Template;
using BP.WF.XML;
using System.IO;
using BP.Tools;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BP.WF.HttpHandler
{
    public class WF_Admin_AttrNode : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_AttrNode()
        {

        }

        #region 事件基类.
        /// <summary>
        /// 事件类型
        /// </summary>
        public new string ShowType
        {
            get
            {
                if (this.FK_Node != 0)
                    return "Node";

                if (this.FK_Node == 0 && DataType.IsNullOrEmpty(this.FK_Flow) == false && this.FK_Flow.Length >= 3)
                    return "Flow";

                if (this.FK_Node == 0 && DataType.IsNullOrEmpty(this.FK_MapData) == false)
                    return "Frm";

                return "Node";
            }
        }
        /// <summary>
        /// 获得该节点下已经绑定该类型的实体.
        /// </summary>
        /// <returns></returns>
        public string ActionDtl_Init()
        {
            //业务单元集合.
            DataTable dtBuess = new DataTable();
            dtBuess.Columns.Add("No", typeof(string));
            dtBuess.Columns.Add("Name", typeof(string));
            dtBuess.TableName = "BuessUnits";
            ArrayList al = BP.En.ClassFactory.GetObjects("BP.Sys.BuessUnitBase");
            foreach (BuessUnitBase en in al)
            {
                DataRow dr = dtBuess.NewRow();
                dr["No"] = en.ToString();
                dr["Name"] = en.Title;
                dtBuess.Rows.Add(dr);
            }

            return BP.Tools.Json.ToJson(dtBuess);
        }
        #endregion 事件基类.


        #region   公文维护
        public string IsExitNodeTempData()
        {
            int workId = int.Parse(this.GetRequestVal("workId"));
            string flowNo = this.GetRequestVal("fk_flow");
            byte[] bytes = BP.DA.DBAccess.GetByteFromDB("ND" + int.Parse(flowNo) + "Rpt", "OID", workId.ToString(), "WordFile");
            if (bytes == null)
                return "err@公文数据不存在.";

            return "成功获取数据.";
        }

        /// <summary>
        /// 选择一个模版
        /// </summary>
        /// <returns></returns>
        public string SelectDocTemp_Save()
        {
            string docTempNo = this.GetRequestVal("no");

            DocTemplate docTemplate = new DocTemplate(docTempNo);

            if (File.Exists(docTemplate.FilePath) == false)
                return "err@选择的模版文件不存在.";

            //获得模版的流.
            var bytes = BP.DA.DataType.ConvertFileToByte(docTemplate.FilePath);

            //保存到数据库里.
            Flow fl = new Flow(this.FK_Flow);
            BP.DA.DBAccess.SaveBytesToDB(bytes, fl.PTable, "OID", this.WorkID,
                "WordFile");

            ////模板与业务的绑定.
            //DocTempFlow dtf = new DocTempFlow();
            //dtf.CheckPhysicsTable();

            //if (dtf.IsExit(DocTempFlowAttr.WorkID, workId))
            //{
            //    dtf.Delete();
            //}
            //dtf.WorkID = workId;
            //dtf.TempNo = docTempNo;
            //dtf.MyPK = workId + "_" + docTempNo;
            //dtf.Insert();

            return "模板导入成功.";
        }

        public string FlowDocInit()
        {
            MethodReturnMessage<string> msg = new MethodReturnMessage<string>
            {
                Success = true
            };

            try
            {
                int nodeId = int.Parse(this.GetRequestVal("nodeId"));
                int workId = int.Parse(this.GetRequestVal("workId"));
                string flowNo = this.GetRequestVal("fk_flow");
                string tableName = "ND" + int.Parse(flowNo) + "Rpt";

                string str = "WordFile";
                if (BP.DA.DBAccess.IsExitsTableCol(tableName, str) == false)
                {
                    /*如果没有此列，就自动创建此列.*/
                    string sql = "ALTER TABLE " + tableName + " ADD  " + str + " image ";

                    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                        sql = "ALTER TABLE " + tableName + " ADD  " + str + " image ";

                    BP.DA.DBAccess.RunSQL(sql);
                }

                byte[] bytes = BP.DA.DBAccess.GetByteFromDB(tableName, "OID", workId.ToString(), "WordFile");
                Node node = new Node(nodeId);

                if (!node.IsStartNode)
                {
                    if (bytes == null)
                    {
                        msg.Message = "{\"IsStartNode\":0,\"IsExistFlowData\":0,\"IsExistTempData\":0}";
                    }
                    else
                    {
                        msg.Message = "{\"IsStartNode\":0,\"IsExistFlowData\":1,\"IsExistTempData\":0}";
                    }
                }
                else//开始节点
                {
                    DocTemplates dts = new DocTemplates();
                    int count = dts.Retrieve(DocTemplateAttr.FK_Node, nodeId);

                    if (bytes == null)
                    {
                        if (count == 0)
                        {
                            msg.Message = "{\"IsStartNode\":1,\"IsExistFlowData\":0,\"IsExistTempData\":0}";
                            msg.Data = null;
                        }
                        else
                        {
                            msg.Message = "{\"IsStartNode\":1,\"IsExistFlowData\":0,\"IsExistTempData\":" + count + "}";
                            msg.Data = dts.ToJson();
                        }
                    }
                    else
                    {
                        if (count == 0)
                        {
                            msg.Message = "{\"IsStartNode\":1,\"IsExistFlowData\":1,\"IsExistTempData\":0}";
                            msg.Data = null;
                        }
                        else
                        {
                            msg.Message = "{\"IsStartNode\":1,\"IsExistFlowData\":1,\"IsExistTempData\":" + count + "}";
                            msg.Data = dts.ToJson();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg.Success = false;
                msg.Message = ex.Message;
            }

            return LitJson.JsonMapper.ToJson(msg);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string DocTemp_Del()
        {
            int no = int.Parse(this.GetRequestVal("no"));

            BP.WF.Template.DocTemplate dt = new DocTemplate();
            dt.Retrieve(DocTemplateAttr.No, no);
            dt.Delete();

            return "操作成功";
        }
        /// <summary>
        /// 模版文件上传
        /// </summary>
        /// <returns></returns>
        public string DocTemp_Upload()
        {
            if (HttpContextHelper.RequestFilesCount == 0)
                return "err@请上传模版.";

            Node nd = new Node(this.FK_Node);

            //上传附件.
            var file = HttpContextHelper.RequestFiles(0);
            var fileName = file.FileName;
            string path = SystemConfig.PathOfDataUser + "DocTemplate\\" + nd.FK_Flow;
            string fileFullPath = path + "\\" + fileName;

            //上传文件.
            if (System.IO.Directory.Exists(path)==false)
                System.IO.Directory.CreateDirectory(path);
            HttpContextHelper.UploadFile(file, fileFullPath);

            //插入模版.
            DocTemplate dt = new DocTemplate();
            dt.FK_Node = FK_Node;
            dt.No = DA.DBAccess.GenerGUID();
            dt.Name = fileName;
            dt.FilePath = fileFullPath; //路径
            dt.FK_Node = this.FK_Node;
            dt.Insert();

            //保存文件.
            DBAccess.SaveFileToDB(fileFullPath, dt.EnMap.PhysicsTable, "No", dt.No, "FileTemplate");
            return dt.ToJson();
        }
        #endregion

        #region  单据模版维护
        /// <summary>
        /// @李国文.
        /// </summary>
        /// <returns></returns>
        public string Bill_Save()
        {
            BillTemplate bt = new BillTemplate();
            if (HttpContextHelper.RequestFilesCount == 0)
                return "err@请上传模版.";

            //上传附件
            string filepath = "";
            //HttpPostedFile file = HttpContext.Current.Request.Files[0];
            //HttpPostedFile file = HttpContextHelper.RequestFiles(0);
            var file = HttpContextHelper.RequestFiles(0);
            string fileName = file.FileName;
            fileName = fileName.Substring(fileName.IndexOf(this.GetRequestVal("TB_Name")));
            fileName = fileName.ToLower();

            filepath = SystemConfig.PathOfDataUser + "CyclostyleFile\\" + fileName;
            //file.SaveAs(filepath);
            HttpContextHelper.UploadFile(file, filepath);

            bt.NodeID = this.FK_Node;
            bt.FK_MapData = this.FK_MapData;
            bt.No = this.GetRequestVal("TB_No");

            if (DataType.IsNullOrEmpty(bt.No))
                bt.No = DA.DBAccess.GenerOID("Template").ToString();

            bt.Name = this.GetRequestVal("TB_Name");
            bt.TempFilePath = fileName; //文件.

            //打印的文件类型.
            bt.HisBillFileType = (BillFileType)this.GetRequestValInt("DDL_BillFileType");

            //打开模式.
            bt.BillOpenModel = (BillOpenModel)this.GetRequestValInt("DDL_BillOpenModel");

            //二维码模式.
            bt.QRModel = (QRModel)this.GetRequestValInt("DDL_BillOpenModel");

            //模版类型.rtf / VSTOForWord / VSTOForExcel .
            if (fileName.Contains(".doc"))
                bt.TemplateFileModel = TemplateFileModel.VSTOForWord;

            if (fileName.Contains(".xls"))
                bt.TemplateFileModel = TemplateFileModel.VSTOForExcel;

            if (fileName.Contains(".rtf"))
                bt.TemplateFileModel = TemplateFileModel.RTF;

            bt.Save();

            bt.SaveFileToDB("DBFile", filepath); //把文件保存到数据库里. 

            return "保存成功.";
        }
        /// <summary>
        /// 下载文件.
        /// </summary>
        public void Bill_Download()
        {
            BillTemplate en = new BillTemplate(this.No);
            string MyFilePath = en.TempFilePath;
            //HttpResponse response = context.Response;

            //response.Clear();
            //response.Buffer = true;
            //response.Charset = "utf-8";
            //response.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}", en.TempFilePath.Substring(MyFilePath.LastIndexOf('\\') + 1)));
            //response.ContentEncoding = System.Text.Encoding.UTF8;
            //response.BinaryWrite(System.IO.File.ReadAllBytes(MyFilePath));
            //response.End();

            HttpContextHelper.ResponseWrite("Charset");
            HttpContextHelper.ResponseWriteHeader("Content-Disposition", string.Format("attachment;filename={0}", en.TempFilePath.Substring(MyFilePath.LastIndexOf('\\') + 1)));
            HttpContextHelper.Response.ContentType = "application/octet-stream;charset=utf-8";
            HttpContextHelper.ResponseWriteFile(MyFilePath);
        }
        #endregion

        #region  节点消息
        public string PushMsg_Init()
        {
            //增加上单据模版集合.
            int nodeID = this.GetRequestValInt("FK_Node");
            BP.WF.Template.PushMsgs ens = new BP.WF.Template.PushMsgs(nodeID);
            return ens.ToJson();
        }
        public string PushMsg_Save()
        {
            BP.WF.Template.PushMsg msg = new BP.WF.Template.PushMsg();
            msg.MyPK = this.MyPK;
            msg.RetrieveFromDBSources();

            msg.FK_Event = this.FK_Event;
            msg.FK_Node = this.FK_Node;

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            BP.WF.Nodes nds = new BP.WF.Nodes(nd.FK_Flow);
            msg.FK_Flow = nd.FK_Flow;

            //推送方式。
            msg.SMSPushWay = Convert.ToInt32(HttpContextHelper.RequestParams("RB_SMS").Replace("RB_SMS_", ""));

            //表单字段作为接收人.
            msg.SMSField = HttpContextHelper.RequestParams("DDL_SMS_Fields");

            #region 其他节点的处理人方式（求选择的节点）
            string nodesOfSMS = "";
            foreach (BP.WF.Node mynd in nds)
            {
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (key.Contains("CB_SMS_" + mynd.NodeID)
                        && nodesOfSMS.Contains(mynd.NodeID + "") == false)
                        nodesOfSMS += mynd.NodeID + ",";


                }
            }
            msg.SMSNodes = nodesOfSMS;
            #endregion 其他节点的处理人方式（求选择的节点）

            //按照SQL
            msg.BySQL = HttpContextHelper.RequestParams("TB_SQL");

            //发给指定的人员
            msg.ByEmps = HttpContextHelper.RequestParams("TB_Emps");

            //短消息发送设备
            msg.SMSPushModel = this.GetRequestVal("PushModel");

            //邮件标题
            msg.MailTitle_Real = HttpContextHelper.RequestParams("TB_title");

            //短信内容模版.
            msg.SMSDoc_Real = HttpContextHelper.RequestParams("TB_SMS");

            //节点预警
            if (this.FK_Event == BP.Sys.EventListOfNode.NodeWarning)
            {
                int noticeType = Convert.ToInt32(HttpContextHelper.RequestParams("RB_NoticeType").Replace("RB_NoticeType", ""));
                msg.SetPara("NoticeType", noticeType);
                int hour = Convert.ToInt32(HttpContextHelper.RequestParams("TB_NoticeHour"));
                msg.SetPara("NoticeHour", hour);
            }

            //节点逾期
            if (this.FK_Event == BP.Sys.EventListOfNode.NodeOverDue)
            {
                int noticeType = Convert.ToInt32(HttpContextHelper.RequestParams("RB_NoticeType").Replace("RB_NoticeType", ""));
                msg.SetPara("NoticeType", noticeType);
                int day = Convert.ToInt32(HttpContextHelper.RequestParams("TB_NoticeDay"));
                msg.SetPara("NoticeDay", day);
            }

            //保存.
            if (DataType.IsNullOrEmpty(msg.MyPK) == true)
            {
                msg.MyPK = BP.DA.DBAccess.GenerGUID();
                msg.Insert();
            }
            else
            {
                msg.Update();
            }

            return "保存成功..";
        }

        public string PushMsgEntity_Init()
        {
            DataSet ds = new DataSet();

            //字段下拉框.
            //select * from Sys_MapAttr where FK_MapData='ND102' and LGType = 0 AND MyDataType =1

            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs();
            attrs.Retrieve(BP.Sys.MapAttrAttr.FK_MapData, "ND" + this.FK_Node, "LGType", 0, "MyDataType", 1);
            ds.Tables.Add(attrs.ToDataTableField("FrmFields"));

            //节点 
            //TODO 数据太多优化一下
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            BP.WF.Nodes nds = new BP.WF.Nodes(nd.FK_Flow);
            ds.Tables.Add(nds.ToDataTableField("Nodes"));

            //mypk
            BP.WF.Template.PushMsg msg = new BP.WF.Template.PushMsg();
            msg.MyPK = this.MyPK;
            msg.RetrieveFromDBSources();
            ds.Tables.Add(msg.ToDataTableField("PushMsgEntity"));

            return BP.Tools.Json.DataSetToJson(ds, false);
        }


        #endregion

        #region 表单模式
        /// <summary>
        /// 表单模式
        /// </summary>
        /// <returns></returns>
        public string NodeFromWorkModel_Init()
        {
            //数据容器.
            DataSet ds = new DataSet();

            // 当前节点信息.
            Node nd = new Node(this.FK_Node);

            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
            nd.NodeFrmID = nd.NodeFrmID;
            // nd.FormUrl = nd.FormUrl;

            DataTable mydt = nd.ToDataTableField("WF_Node");
            ds.Tables.Add(mydt);

            BtnLab btn = new BtnLab(this.FK_Node);
            DataTable dtBtn = btn.ToDataTableField("WF_BtnLab");
            ds.Tables.Add(dtBtn);

            //节点s
            Nodes nds = new Nodes(nd.FK_Flow);

            //节点s
            ds.Tables.Add(nds.ToDataTableField("Nodes"));

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 表单模式
        /// </summary>
        /// <returns></returns>
        public string NodeFromWorkModel_Save()
        {
            Node nd = new Node(this.FK_Node);

            BP.Sys.MapData md = new BP.Sys.MapData("ND" + this.FK_Node);

            //用户选择的表单类型.
            string selectFModel = this.GetValFromFrmByKey("FrmS");

            //使用ccbpm内置的节点表单
            if (selectFModel == "DefFrm")
            {
                //呈现风格
                string frmModel = this.GetValFromFrmByKey("RB_Frm");
                if (frmModel == "0")
                {
                    //自由表单
                    nd.FormType = NodeFormType.FreeForm;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm;
                    md.Update();
                }
                else
                {
                    //傻瓜表单
                    nd.FormType = NodeFormType.FoolForm;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FoolForm;
                    md.Update();
                }
                //表单引用
                string refFrm = this.GetValFromFrmByKey("RefFrm");
                //当前节点表单
                if (refFrm == "0")
                {
                    nd.NodeFrmID = "";
                    nd.DirectUpdate();
                }
                //其他节点表单
                if (refFrm == "1")
                {
                    nd.NodeFrmID = "ND" + this.GetValFromFrmByKey("DDL_Frm");
                    nd.DirectUpdate();
                }
            }

            //使用傻瓜轨迹表单模式.
            if (selectFModel == "FoolTruck")
            {
                nd.FormType = NodeFormType.FoolTruck;
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.FoolForm;  //同时更新表单表住表.
                md.Update();
            }

            //使用嵌入式表单
            if (selectFModel == "SelfForm")
            {
                nd.FormType = NodeFormType.SelfForm;
                nd.FormUrl = this.GetValFromFrmByKey("TB_CustomURL");
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.Url;  //同时更新表单表住表.
                md.Url = this.GetValFromFrmByKey("TB_CustomURL");
                md.Update();

            }
            //使用SDK表单
            if (selectFModel == "SDKForm")
            {
                nd.FormType = NodeFormType.SDKForm;
                nd.FormUrl = this.GetValFromFrmByKey("TB_FormURL");
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.Url;
                md.Url = this.GetValFromFrmByKey("TB_FormURL");
                md.Update();

            }
            //绑定多表单
            if (selectFModel == "SheetTree")
            {

                string sheetTreeModel = this.GetValFromFrmByKey("SheetTreeModel");

                if (sheetTreeModel == "0")
                {
                    nd.FormType = NodeFormType.SheetTree;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm; //同时更新表单表住表.
                    md.Update();
                }
                else
                {
                    nd.FormType = NodeFormType.DisableIt;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm; //同时更新表单表住表.
                    md.Update();
                }
            }

            return "保存成功...";
        }
        #endregion 表单模式

        #region 手机表单字段排序
        #region SortingMapAttrs_Init

        public string SortingMapAttrs_Init()
        {
            MapDatas mapdatas;
            MapAttrs attrs;
            GroupFields groups;
            MapDtls dtls;
            FrmAttachments athMents;
            FrmBtns btns;

            Nodes nodes = null;

            #region 获取数据
            mapdatas = new MapDatas();
            QueryObject qo = new QueryObject(mapdatas);
            qo.AddWhere(MapDataAttr.No, "Like", FK_MapData + "%");
            qo.addOrderBy(MapDataAttr.Idx);
            qo.DoQuery();

            attrs = new MapAttrs();
            qo = new QueryObject(attrs);
            qo.AddWhere(MapAttrAttr.FK_MapData, FK_MapData);
            qo.addAnd();
            qo.AddWhere(MapAttrAttr.EditType, 0);
            qo.addOrderBy(MapAttrAttr.GroupID, MapAttrAttr.Idx);
            qo.DoQuery();

            btns = new FrmBtns(this.FK_MapData);
            athMents = new FrmAttachments(this.FK_MapData);
            dtls = new MapDtls(this.FK_MapData);

            groups = new GroupFields();
            qo = new QueryObject(groups);
            qo.AddWhere(GroupFieldAttr.FrmID, FK_MapData);
            qo.addOrderBy(GroupFieldAttr.Idx);
            qo.DoQuery();
            #endregion

            DataSet ds = new DataSet();

            BindData4SortingMapAttrs_Init(mapdatas,
            attrs,
            groups,
            dtls,
            athMents,
            btns,
            nodes,
            ds);

            //控制页面按钮需要的
            MapDtl tdtl = new MapDtl();
            tdtl.No = FK_MapData;
            if (tdtl.RetrieveFromDBSources() == 1)
            {
                ds.Tables.Add(tdtl.ToDataTableField("tdtl"));
            }

            return BP.Tools.Json.ToJson(ds);
        }

        private void BindData4SortingMapAttrs_Init(MapDatas mapdatas,
            MapAttrs attrs,
            GroupFields groups,
            MapDtls dtls,
            FrmAttachments athMents,
            FrmBtns btns,
            Nodes nodes,
            DataSet ds)
        {
            MapData mapdata = mapdatas.GetEntityByKey(FK_MapData) as MapData;
            DataTable dtAttrs = attrs.ToDataTableField("dtAttrs");
            DataTable dtDtls = dtls.ToDataTableField("dtDtls");
            DataTable dtGroups = groups.ToDataTableField("dtGroups");
            DataTable dtNoGroupAttrs = null;
            DataRow[] rows_Attrs = null;
            int idx_Attr = 1;
            int gidx = 1;
            GroupField group = null;

            if (mapdata != null)
            {
                #region 一、面板1、 分组数据+未分组数据

                #region A、构建数据dtNoGroupAttrs，这个放在前面
                //检索全部字段，查找出没有分组或分组信息不正确的字段，存入“无分组”集合
                dtNoGroupAttrs = dtAttrs.Clone();

                foreach (DataRow dr in dtAttrs.Rows)
                {
                    if (IsExistInDataRowArray(dtGroups.Rows, GroupFieldAttr.OID, dr[MapAttrAttr.GroupID]) == false)
                        dtNoGroupAttrs.Rows.Add(dr.ItemArray);
                }
                #endregion

                #region B、构建数据dtGroups，这个放在后面(！！涉及更新数据库)
                #region 如果没有，则创建分组（1.明细2.多附件3.按钮）
                //01、未分组明细表,自动创建一个
                foreach (MapDtl mapDtl in dtls)
                {
                    if (GetGroupID(mapDtl.No, groups) == 0)
                    {
                        group = new GroupField();
                        group.Lab = mapDtl.Name;
                        group.FrmID = mapDtl.FK_MapData;
                        group.CtrlType = GroupCtrlType.Dtl;
                        group.CtrlID = mapDtl.No;
                        group.Insert();

                        groups.AddEntity(group);
                    }
                }
                //02、未分组多附件自动分配一个
                foreach (FrmAttachment athMent in athMents)
                {
                    if (GetGroupID(athMent.MyPK, groups) == 0)
                    {
                        group = new GroupField();
                        group.Lab = athMent.Name;
                        group.EnName = athMent.FK_MapData;
                        group.CtrlType = GroupCtrlType.Ath;
                        group.CtrlID = athMent.MyPK;
                        group.Insert();

                        athMent.GroupID = group.OID;
                        athMent.Update();

                        groups.AddEntity(group);
                    }
                }

                //03、未分组按钮自动创建一个
                foreach (FrmBtn fbtn in btns)
                {
                    if (GetGroupID(fbtn.MyPK, groups) == 0)
                    {
                        group = new GroupField();
                        group.Lab = fbtn.Text;
                        group.FrmID = fbtn.FK_MapData;
                        group.CtrlType = GroupCtrlType.Btn;
                        group.CtrlID = fbtn.MyPK;
                        group.Insert();

                        fbtn.GroupID = group.OID;
                        fbtn.Update();

                        groups.AddEntity(group);
                    }
                }
                #endregion

                dtGroups = groups.ToDataTableField("dtGroups");
                #endregion


                #endregion


                #region 三、其他。如果是明细表的字段排序，则增加“返回”按钮；否则增加“复制排序”按钮,2016-03-21

                DataTable isDtl = new DataTable();
                isDtl.Columns.Add("tdDtl", typeof(int));
                isDtl.Columns.Add("FK_MapData", typeof(string));
                isDtl.Columns.Add("No", typeof(string));
                isDtl.TableName = "TRDtl";

                DataRow tddr = isDtl.NewRow();

                MapDtl tdtl = new MapDtl();
                tdtl.No = FK_MapData;
                if (tdtl.RetrieveFromDBSources() == 1)
                {
                    tddr["tdDtl"] = 1;
                    tddr["FK_MapData"] = tdtl.FK_MapData;
                    tddr["No"] = tdtl.No;
                }
                else
                {
                    tddr["tdDtl"] = 0;
                    tddr["FK_MapData"] = FK_MapData;
                    tddr["No"] = tdtl.No;
                }


                isDtl.Rows.Add(tddr.ItemArray);
                #endregion

                #region 增加节点信息
                if (DataType.IsNullOrEmpty(FK_Flow) == false)
                {
                    nodes = new Nodes();
                    nodes.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, FK_Flow, BP.WF.Template.NodeAttr.Step);

                    if (nodes.Count == 0)
                    {
                        string nodeid = FK_MapData.Replace("ND", "");
                        string flowno = string.Empty;

                        if (nodeid.Length > 2)
                        {
                            flowno = nodeid.Substring(0, nodeid.Length - 2).PadLeft(3, '0');
                            nodes.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, flowno, BP.WF.Template.NodeAttr.Step);
                        }
                    }
                    DataTable dtNodes = nodes.ToDataTableField("dtNodes");
                    dtNodes.TableName = "dtNodes";
                    ds.Tables.Add(dtNodes);
                }

                #endregion

                ds.Tables.Add(mapdatas.ToDataTableField("mapdatas"));
                dtGroups.TableName = "dtGroups";
                ds.Tables.Add(dtGroups);
                dtNoGroupAttrs.TableName = "dtNoGroupAttrs";
                ds.Tables.Add(dtNoGroupAttrs);
                dtAttrs.TableName = "dtAttrs";
                ds.Tables.Add(dtAttrs);
                dtDtls.TableName = "dtDtls";
                ds.Tables.Add(dtDtls);
                ds.Tables.Add(athMents.ToDataTableField("athMents"));
                ds.Tables.Add(btns.ToDataTableField("btns"));
                ds.Tables.Add(isDtl);

                //ds.Tables.Add(nodes.ToDataTableField("nodes"));
            }
        }

        /// <summary>
        /// 判断在DataRow数组中，是否存在指定列指定值的行
        /// </summary>
        /// <param name="rows">DataRow数组</param>
        /// <param name="field">指定列名</param>
        /// <param name="value">指定值</param>
        /// <returns></returns>
        private bool IsExistInDataRowArray(DataRowCollection rows, string field, object value)
        {
            foreach (DataRow row in rows)
            {
                int rw = int.Parse(row[field].ToString());
                if (rw == int.Parse(value.ToString()))
                    return true;
            }

            return false;
        }

        private int GetGroupID(string ctrlID, GroupFields gfs)
        {
            GroupField gf = gfs.GetEntityByKey(GroupFieldAttr.CtrlID, ctrlID) as GroupField;
            return gf == null ? 0 : gf.OID;
        }

        #endregion

        #region 重置字段顺序
        /// <summary>
        /// 重置字段顺序
        /// </summary>
        /// <returns></returns>
        public string SortingMapAttrs_ReSet()
        {
            try
            {
                MapAttrs attrs = new MapAttrs();
                QueryObject qo = new QueryObject(attrs);
                qo.AddWhere(MapAttrAttr.FK_MapData, FK_MapData);//添加查询条件
                qo.addAnd();
                qo.AddWhere(MapAttrAttr.UIVisible, true);
                qo.addOrderBy(MapAttrAttr.Y, MapAttrAttr.X);
                qo.DoQuery();//执行查询
                int rowIdx = 0;
                //执行更新
                foreach (MapAttr mapAttr in attrs)
                {
                    mapAttr.Idx = rowIdx;
                    mapAttr.DirectUpdate();
                    rowIdx++;
                }

                return "重置成功！";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message.ToString();
            }
        }
        #endregion

        #region 设置在手机端显示的字段
        /// <summary>
        /// 保存需要在手机端表单显示的字段
        /// </summary>
        /// <returns></returns>
        public string SortingMapAttrs_From_Save()
        {
            //获取需要显示的字段集合
            var atts = this.GetRequestVal("attrs");
            try
            {
                MapAttrs attrs = new MapAttrs(FK_MapData);
                MapAttr att = null;
                //更新每个字段的显示属性
                foreach (MapAttr attr in attrs)
                {
                    att = attrs.GetEntityByKey(MapAttrAttr.FK_MapData, FK_MapData, MapAttrAttr.KeyOfEn, attr.KeyOfEn) as MapAttr;
                    if (atts.Contains("," + attr.KeyOfEn + ","))
                    {
                        att.IsEnableInAPP = true;
                    }
                    else
                        att.IsEnableInAPP = false;
                    att.Update();

                    //if (atts.Contains("," + FK_MapData + "_" + attr.KeyOfEn + ",") == true)
                    //{
                    //    FrmAttachment ath = new FrmAttachment(FK_MapData + "_" + attr.KeyOfEn);
                    //    ath.SetPara("IsShowMobile", 1);
                    //    ath.Update();

                    //}
                }
                //获取附件
                FrmAttachments aths = new FrmAttachments();
                aths.Retrieve(FrmAttachmentAttr.FK_MapData, this.FK_MapData, FrmAttachmentAttr.FK_Node, 0);
                foreach (FrmAttachment ath in aths)
                {
                    if (atts.Contains("," + ath.MyPK + ",") == true)
                        ath.SetPara("IsShowMobile", 1);
                    else
                        ath.SetPara("IsShowMobile", 0);
                    ath.Update();
                }
                return "保存成功！";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message.ToString();
            }
        }
        #endregion

        #region 将分组、字段排序复制到其他节点
        /// <summary>
        /// 将分组、字段排序复制到其他节点
        /// </summary>
        /// <returns></returns>
        public string SortingMapAttrs_Copy()
        {
            try
            {
                string[] nodeids = this.GetRequestVal("NodeIDs").Split(',');

                MapDatas mapdatas = new MapDatas();
                QueryObject obj = new QueryObject(mapdatas);
                obj.AddWhere(MapDataAttr.No, "Like", FK_MapData + "%");
                obj.addOrderBy(MapDataAttr.Idx);
                obj.DoQuery();

                MapAttrs attrs = new MapAttrs();
                obj = new QueryObject(attrs);
                obj.AddWhere(MapAttrAttr.FK_MapData, FK_MapData);
                obj.addAnd();
                obj.AddWhere(MapAttrAttr.UIVisible, true);
                obj.addOrderBy(MapAttrAttr.GroupID, MapAttrAttr.Idx);
                obj.DoQuery();

                FrmBtns btns = new FrmBtns(this.FK_MapData);
                FrmAttachments athMents = new FrmAttachments(this.FK_MapData);
                MapDtls dtls = new MapDtls(this.FK_MapData);

                GroupFields groups = new GroupFields();
                obj = new QueryObject(groups);
                obj.AddWhere(GroupFieldAttr.FrmID, FK_MapData);
                obj.addOrderBy(GroupFieldAttr.Idx);
                obj.DoQuery();

                string tmd = null;
                GroupField group = null;
                MapDatas tmapdatas = null;
                MapAttrs tattrs = null, oattrs = null, tarattrs = null;
                GroupFields tgroups = null, ogroups = null, targroups = null;
                MapDtls tdtls = null;
                MapData tmapdata = null;
                MapAttr tattr = null;
                GroupField tgrp = null;
                MapDtl tdtl = null;
                int maxGrpIdx = 0;  //当前最大分组排序号
                int maxAttrIdx = 0; //当前最大字段排序号
                int maxDtlIdx = 0;  //当前最大明细表排序号
                List<string> idxGrps = new List<string>();  //复制过的分组名称集合
                List<string> idxAttrs = new List<string>(); //复制过的字段编号集合
                List<string> idxDtls = new List<string>();  //复制过的明细表编号集合

                foreach (string nodeid in nodeids)
                {
                    tmd = "ND" + nodeid;

                    #region 获取数据
                    tmapdatas = new MapDatas();
                    QueryObject qo = new QueryObject(tmapdatas);
                    qo.AddWhere(MapDataAttr.No, "Like", tmd + "%");
                    qo.addOrderBy(MapDataAttr.Idx);
                    qo.DoQuery();

                    tattrs = new MapAttrs();
                    qo = new QueryObject(tattrs);
                    qo.AddWhere(MapAttrAttr.FK_MapData, tmd);
                    qo.addAnd();
                    qo.AddWhere(MapAttrAttr.UIVisible, true);
                    qo.addOrderBy(MapAttrAttr.GroupID, MapAttrAttr.Idx);
                    qo.DoQuery();

                    tgroups = new GroupFields();
                    qo = new QueryObject(tgroups);
                    qo.AddWhere(GroupFieldAttr.FrmID, tmd);
                    qo.addOrderBy(GroupFieldAttr.Idx);
                    qo.DoQuery();

                    tdtls = new MapDtls();
                    qo = new QueryObject(tdtls);
                    qo.AddWhere(MapDtlAttr.FK_MapData, tmd);
                    qo.addAnd();
                    qo.AddWhere(MapDtlAttr.IsView, true);
                    //qo.addOrderBy(MapDtlAttr.RowIdx);
                    qo.DoQuery();
                    #endregion

                    #region 复制排序逻辑

                    #region //分组排序复制
                    foreach (GroupField grp in groups)
                    {
                        //通过分组名称来确定是同一个组，同一个组在不同的节点分组编号是不一样的
                        tgrp = tgroups.GetEntityByKey(GroupFieldAttr.Lab, grp.Lab) as GroupField;
                        if (tgrp == null) continue;

                        tgrp.Idx = grp.Idx;
                        tgrp.DirectUpdate();

                        maxGrpIdx = Math.Max(grp.Idx, maxGrpIdx);
                        idxGrps.Add(grp.Lab);
                    }

                    foreach (GroupField grp in tgroups)
                    {
                        if (idxGrps.Contains(grp.Lab))
                            continue;

                        grp.Idx = maxGrpIdx = maxGrpIdx + 1;
                        grp.DirectUpdate();
                    }
                    #endregion

                    #region //字段排序复制
                    foreach (MapAttr attr in attrs)
                    {
                        //排除主键
                        if (attr.IsPK == true)
                            continue;

                        tattr = tattrs.GetEntityByKey(MapAttrAttr.KeyOfEn, attr.KeyOfEn) as MapAttr;
                        if (tattr == null) continue;

                        group = groups.GetEntityByKey(GroupFieldAttr.OID, attr.GroupID) as GroupField;

                        //比对字段的分组是否一致，不一致则更新一致
                        if (group == null)
                        {
                            //源字段分组为空，则目标字段分组置为0
                            tattr.GroupID = 0;
                        }
                        else
                        {
                            //此处要判断目标节点中是否已经创建了这个源字段所属分组，如果没有创建，则要自动创建
                            tgrp = tgroups.GetEntityByKey(GroupFieldAttr.Lab, group.Lab) as GroupField;

                            if (tgrp == null)
                            {
                                tgrp = new GroupField();
                                tgrp.Lab = group.Lab;
                                tgrp.FrmID = tmd;
                                tgrp.Idx = group.Idx;
                                tgrp.Insert();
                                tgroups.AddEntity(tgrp);

                                tattr.GroupID = tgrp.OID;
                            }
                            else
                            {
                                if (tgrp.OID != tattr.GroupID)
                                    tattr.GroupID = tgrp.OID;
                            }
                        }

                        tattr.Idx = attr.Idx;
                        tattr.DirectUpdate();
                        maxAttrIdx = Math.Max(attr.Idx, maxAttrIdx);
                        idxAttrs.Add(attr.KeyOfEn);
                    }

                    foreach (MapAttr attr in tattrs)
                    {
                        //排除主键
                        if (attr.IsPK == true)
                            continue;
                        if (idxAttrs.Contains(attr.KeyOfEn))
                            continue;

                        attr.Idx = maxAttrIdx = maxAttrIdx + 1;
                        attr.DirectUpdate();
                    }
                    #endregion

                    #region //明细表排序复制
                    string dtlIdx = string.Empty;
                    GroupField tgroup = null;
                    int groupidx = 0;
                    int tgroupidx = 0;

                    foreach (MapDtl dtl in dtls)
                    {
                        dtlIdx = dtl.No.Replace(dtl.FK_MapData + "Dtl", "");
                        tdtl = tdtls.GetEntityByKey(MapDtlAttr.No, tmd + "Dtl" + dtlIdx) as MapDtl;

                        if (tdtl == null)
                            continue;

                        //判断目标明细表是否有分组，没有分组，则创建分组
                        tgroup = GetGroup(tdtl.No, tgroups);
                        tgroupidx = tgroup == null ? 0 : tgroup.Idx;
                        group = GetGroup(dtl.No, groups);
                        groupidx = group == null ? 0 : group.Idx;

                        if (tgroup == null)
                        {
                            group = new GroupField();
                            group.Lab = tdtl.Name;
                            group.FrmID = tdtl.FK_MapData;
                            group.CtrlType = GroupCtrlType.Dtl;
                            group.CtrlID = tdtl.No;
                            group.Idx = groupidx;
                            group.Insert();

                            tgroupidx = groupidx;
                            tgroups.AddEntity(group);
                        }

                        #region 1.明细表排序
                        if (tgroupidx != groupidx && group != null)
                        {
                            tgroup.Idx = groupidx;
                            tgroup.DirectUpdate();

                            tgroupidx = groupidx;
                            tmapdata = tmapdatas.GetEntityByKey(MapDataAttr.No, tdtl.No) as MapData;
                            if (tmapdata != null)
                            {
                                tmapdata.Idx = tgroup.Idx;
                                tmapdata.DirectUpdate();
                            }
                        }

                        maxDtlIdx = Math.Max(tgroupidx, maxDtlIdx);
                        idxDtls.Add(dtl.No);
                        #endregion

                        #region 2.获取源节点明细表中的字段分组、字段信息
                        oattrs = new MapAttrs();
                        qo = new QueryObject(oattrs);
                        qo.AddWhere(MapAttrAttr.FK_MapData, dtl.No);
                        qo.addAnd();
                        qo.AddWhere(MapAttrAttr.UIVisible, true);
                        qo.addOrderBy(MapAttrAttr.GroupID, MapAttrAttr.Idx);
                        qo.DoQuery();

                        ogroups = new GroupFields();
                        qo = new QueryObject(ogroups);
                        qo.AddWhere(GroupFieldAttr.FrmID, dtl.No);
                        qo.addOrderBy(GroupFieldAttr.Idx);
                        qo.DoQuery();
                        #endregion

                        #region 3.获取目标节点明细表中的字段分组、字段信息
                        tarattrs = new MapAttrs();
                        qo = new QueryObject(tarattrs);
                        qo.AddWhere(MapAttrAttr.FK_MapData, tdtl.No);
                        qo.addAnd();
                        qo.AddWhere(MapAttrAttr.UIVisible, true);
                        qo.addOrderBy(MapAttrAttr.GroupID, MapAttrAttr.Idx);
                        qo.DoQuery();

                        targroups = new GroupFields();
                        qo = new QueryObject(targroups);
                        qo.AddWhere(GroupFieldAttr.FrmID, tdtl.No);
                        qo.addOrderBy(GroupFieldAttr.Idx);
                        qo.DoQuery();
                        #endregion

                        #region 4.明细表字段分组排序
                        maxGrpIdx = 0;
                        idxGrps = new List<string>();

                        foreach (GroupField grp in ogroups)
                        {
                            //通过分组名称来确定是同一个组，同一个组在不同的节点分组编号是不一样的
                            tgrp = targroups.GetEntityByKey(GroupFieldAttr.Lab, grp.Lab) as GroupField;
                            if (tgrp == null) continue;

                            tgrp.Idx = grp.Idx;
                            tgrp.DirectUpdate();

                            maxGrpIdx = Math.Max(grp.Idx, maxGrpIdx);
                            idxGrps.Add(grp.Lab);
                        }

                        foreach (GroupField grp in targroups)
                        {
                            if (idxGrps.Contains(grp.Lab))
                                continue;

                            grp.Idx = maxGrpIdx = maxGrpIdx + 1;
                            grp.DirectUpdate();
                        }
                        #endregion

                        #region 5.明细表字段排序
                        maxAttrIdx = 0;
                        idxAttrs = new List<string>();

                        foreach (MapAttr attr in oattrs)
                        {
                            tattr = tarattrs.GetEntityByKey(MapAttrAttr.KeyOfEn, attr.KeyOfEn) as MapAttr;
                            if (tattr == null) continue;

                            group = ogroups.GetEntityByKey(GroupFieldAttr.OID, attr.GroupID) as GroupField;

                            //比对字段的分组是否一致，不一致则更新一致
                            if (group == null)
                            {
                                //源字段分组为空，则目标字段分组置为0
                                tattr.GroupID = 0;
                            }
                            else
                            {
                                //此处要判断目标节点中是否已经创建了这个源字段所属分组，如果没有创建，则要自动创建
                                tgrp = targroups.GetEntityByKey(GroupFieldAttr.Lab, group.Lab) as GroupField;

                                if (tgrp == null)
                                {
                                    tgrp = new GroupField();
                                    tgrp.Lab = group.Lab;
                                    tgrp.EnName = tdtl.No;
                                    tgrp.Idx = group.Idx;
                                    tgrp.Insert();
                                    targroups.AddEntity(tgrp);

                                    tattr.GroupID = tgrp.OID;
                                }
                                else
                                {
                                    if (tgrp.OID != tattr.GroupID)
                                        tattr.GroupID = tgrp.OID;
                                }
                            }

                            tattr.Idx = attr.Idx;
                            tattr.DirectUpdate();
                            maxAttrIdx = Math.Max(attr.Idx, maxAttrIdx);
                            idxAttrs.Add(attr.KeyOfEn);
                        }

                        foreach (MapAttr attr in tarattrs)
                        {
                            if (idxAttrs.Contains(attr.KeyOfEn))
                                continue;

                            attr.Idx = maxAttrIdx = maxAttrIdx + 1;
                            attr.DirectUpdate();
                        }
                        #endregion
                    }

                    //确定目标节点中，源节点没有的明细表的排序
                    foreach (MapDtl dtl in tdtls)
                    {
                        if (idxDtls.Contains(dtl.No))
                            continue;

                        maxDtlIdx = maxDtlIdx + 1;
                        tgroup = GetGroup(dtl.No, tgroups);

                        if (tgroup == null)
                        {
                            tgroup = new GroupField();
                            tgroup.Lab = tdtl.Name;
                            tgroup.FrmID = tdtl.FK_MapData;
                            tgroup.CtrlType = GroupCtrlType.Dtl;
                            tgroup.CtrlID = tdtl.No;
                            tgroup.Idx = maxDtlIdx;
                            tgroup.Insert();

                            tgroups.AddEntity(group);
                        }

                        if (tgroup.Idx != maxDtlIdx)
                        {
                            tgroup.Idx = maxDtlIdx;
                            tgroup.DirectUpdate();
                        }

                        tmapdata = tmapdatas.GetEntityByKey(MapDataAttr.No, dtl.No) as MapData;
                        if (tmapdata != null)
                        {
                            tmapdata.Idx = maxDtlIdx;
                            tmapdata.DirectUpdate();
                        }
                    }
                    #endregion

                    #endregion

                }
                return "复制成功！";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message.ToString();
            }
        }

        private GroupField GetGroup(string ctrlID, GroupFields gfs)
        {
            return gfs.GetEntityByKey(GroupFieldAttr.CtrlID, ctrlID) as GroupField;
        }
        #endregion

        #endregion

        #region 表单模式
        public string SortingMapAttrs_Save()
        {
            Node nd = new Node(this.FK_Node);

            BP.Sys.MapData md = new BP.Sys.MapData("ND" + this.FK_Node);

            //用户选择的表单类型.
            string selectFModel = this.GetValFromFrmByKey("FrmS");

            //使用ccbpm内置的节点表单
            if (selectFModel == "DefFrm")
            {
                string frmModel = this.GetValFromFrmByKey("RB_Frm");
                if (frmModel == "0")
                {
                    nd.FormType = NodeFormType.FreeForm;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm;
                    md.Update();
                }
                else
                {
                    nd.FormType = NodeFormType.FoolForm;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FoolForm;
                    md.Update();
                }

                string refFrm = this.GetValFromFrmByKey("RefFrm");

                if (refFrm == "0")
                {
                    nd.NodeFrmID = "";
                    nd.DirectUpdate();
                }

                if (refFrm == "1")
                {
                    nd.NodeFrmID = "ND" + this.GetValFromFrmByKey("DDL_Frm");
                    nd.DirectUpdate();
                }
            }

            //使用傻瓜轨迹表单模式.
            if (selectFModel == "FoolTruck")
            {
                nd.FormType = NodeFormType.FoolTruck;
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.FoolForm;  //同时更新表单表住表.
                md.Update();
            }

            //使用嵌入式表单
            if (selectFModel == "SelfForm")
            {
                nd.FormType = NodeFormType.SelfForm;
                nd.FormUrl = this.GetValFromFrmByKey("TB_CustomURL");
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.Url;  //同时更新表单表住表.
                md.Url = this.GetValFromFrmByKey("TB_CustomURL");
                md.Update();

            }
            //使用SDK表单
            if (selectFModel == "SDKForm")
            {
                nd.FormType = NodeFormType.SDKForm;
                nd.FormUrl = this.GetValFromFrmByKey("TB_FormURL");
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.Url;
                md.Url = this.GetValFromFrmByKey("TB_FormURL");
                md.Update();

            }
            //绑定多表单
            if (selectFModel == "SheetTree")
            {

                string sheetTreeModel = this.GetValFromFrmByKey("SheetTreeModel");

                if (sheetTreeModel == "0")
                {
                    nd.FormType = NodeFormType.SheetTree;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm; //同时更新表单表住表.
                    md.Update();
                }
                else
                {
                    nd.FormType = NodeFormType.DisableIt;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm; //同时更新表单表住表.
                    md.Update();
                }
            }
            return "保存成功...";
        }
        #endregion 表单模式

        #region 节点属性（列表）的操作
        /// <summary>
        /// 初始化节点属性列表.
        /// </summary>
        /// <returns></returns>
        public string NodeAttrs_Init()
        {
            var strFlowId = GetRequestVal("FK_Flow");
            if (DataType.IsNullOrEmpty(strFlowId))
            {
                return "err@参数错误！";
            }
            Nodes nodes = new Nodes();
            nodes.Retrieve("FK_Flow", strFlowId);
            //因直接使用nodes.ToJson()无法获取某些字段（e.g.HisFormTypeText,原因：Node没有自己的Attr类）
            //故此处手动创建前台所需的DataTable
            DataTable dt = new DataTable();
            dt.Columns.Add("NodeID");	//节点ID
            dt.Columns.Add("Name");		//节点名称
            dt.Columns.Add("HisFormType");		//表单方案
            dt.Columns.Add("HisFormTypeText");
            dt.Columns.Add("HisRunModel");		//节点类型
            dt.Columns.Add("HisRunModelT");

            dt.Columns.Add("HisDeliveryWay");	//接收方类型
            dt.Columns.Add("HisDeliveryWayText");
            dt.Columns.Add("HisDeliveryWayJsFnPara");
            dt.Columns.Add("HisDeliveryWayCountLabel");
            dt.Columns.Add("HisDeliveryWayCount");	//接收方Count

            dt.Columns.Add("HisCCRole");	//抄送人
            dt.Columns.Add("HisCCRoleText");
            dt.Columns.Add("HisFrmEventsCount");	//消息&事件Count
            dt.Columns.Add("HisFinishCondsCount");	//流程完成条件Count
            DataRow dr;
            foreach (Node node in nodes)
            {
                dr = dt.NewRow();
                dr["NodeID"] = node.NodeID;
                dr["Name"] = node.Name;
                dr["HisFormType"] = node.HisFormType;
                dr["HisFormTypeText"] = node.HisFormTypeText;
                dr["HisRunModel"] = node.HisRunModel;
                dr["HisRunModelT"] = node.HisRunModelT;
                dr["HisDeliveryWay"] = node.HisDeliveryWay;
                dr["HisDeliveryWayText"] = node.HisDeliveryWayText;

                //接收方数量
                var intHisDeliveryWayCount = 0;
                if (node.HisDeliveryWay == BP.WF.DeliveryWay.ByStation)
                {
                    dr["HisDeliveryWayJsFnPara"] = "ByStation";
                    dr["HisDeliveryWayCountLabel"] = "岗位";
                    BP.WF.Template.NodeStations nss = new BP.WF.Template.NodeStations();
                    intHisDeliveryWayCount = nss.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, node.NodeID);
                }
                else if (node.HisDeliveryWay == BP.WF.DeliveryWay.ByDept)
                {
                    dr["HisDeliveryWayJsFnPara"] = "ByDept";
                    dr["HisDeliveryWayCountLabel"] = "部门";
                    BP.WF.Template.NodeDepts nss = new BP.WF.Template.NodeDepts();
                    intHisDeliveryWayCount = nss.Retrieve(BP.WF.Template.NodeDeptAttr.FK_Node, node.NodeID);
                }
                else if (node.HisDeliveryWay == BP.WF.DeliveryWay.ByBindEmp)
                {
                    dr["HisDeliveryWayJsFnPara"] = "ByDept";
                    dr["HisDeliveryWayCountLabel"] = "人员";
                    BP.WF.Template.NodeEmps nes = new BP.WF.Template.NodeEmps();
                    intHisDeliveryWayCount = nes.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, node.NodeID);
                }
                dr["HisDeliveryWayCount"] = intHisDeliveryWayCount;

                //抄送
                dr["HisCCRole"] = node.HisCCRole;
                dr["HisCCRoleText"] = node.HisCCRoleText;

                //消息&事件Count
                BP.Sys.FrmEvents fes = new BP.Sys.FrmEvents();
                dr["HisFrmEventsCount"] = fes.Retrieve(BP.Sys.FrmEventAttr.FK_MapData, "ND" + node.NodeID);

                //流程完成条件Count
                BP.WF.Template.Conds conds = new BP.WF.Template.Conds(BP.WF.Template.CondType.Flow, node.NodeID);
                dr["HisFinishCondsCount"] = conds.Count;

                dt.Rows.Add(dr);
            }
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion

        #region 特别控件特别用户权限
        public string SepcFiledsSepcUsers_Init()
        {

            /*string fk_mapdata = this.GetRequestVal("FK_MapData");
            if (DataType.IsNullOrEmpty(fk_mapdata))
                fk_mapdata = "ND101";

            string fk_node = this.GetRequestVal("FK_Node");
            if (DataType.IsNullOrEmpty(fk_node))
                fk_mapdata = "101";


            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs(fk_mapdata);

            BP.Sys.FrmImgs imgs = new BP.Sys.FrmImgs(fk_mapdata);

            BP.Sys.MapExts exts = new BP.Sys.MapExts();
            int mecount = exts.Retrieve(BP.Sys.MapExtAttr.FK_MapData, fk_mapdata,
                BP.Sys.MapExtAttr.Tag, this.GetRequestVal("FK_Node"),
                BP.Sys.MapExtAttr.ExtType, "SepcFiledsSepcUsers");

            BP.Sys.FrmAttachments aths = new BP.Sys.FrmAttachments(fk_mapdata);

            exts = new BP.Sys.MapExts();
            exts.Retrieve(BP.Sys.MapExtAttr.FK_MapData, fk_mapdata,
                BP.Sys.MapExtAttr.Tag, this.GetRequestVal("FK_Node"),
                BP.Sys.MapExtAttr.ExtType, "SepcAthSepcUsers");
            */
            return "";//toJson
        }
        #endregion

        #region 批量发起规则设置
        public string BatchStartFields_Init()
        {
            int nodeID = int.Parse(this.FK_Node.ToString());
            //获取节点字段集合
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nodeID);
            //获取节点对象
            BP.WF.Node nd = new BP.WF.Node(nodeID);
            //获取批量发起设置规则
            BP.Sys.SysEnums ses = new BP.Sys.SysEnums(BP.WF.Template.NodeAttr.BatchRole);
            //获取当前节点设置的批处理规则
            string srole = "";
            if (nd.HisBatchRole == BatchRole.None)
                srole = "0";
            else if (nd.HisBatchRole == BatchRole.Ordinary)
                srole = "1";
            else
                srole = "2";
            return "{\"nd\":" + nd.ToJson() + ",\"ses\":" + ses.ToJson() + ",\"attrs\":" + attrs.ToJson() + ",\"BatchRole\":" + srole + "}";
        }
        #endregion

        #region 批量发起规则设置save
        public string BatchStartFields_Save()
        {
            int nodeID = int.Parse(this.FK_Node.ToString());
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nodeID);
            BP.WF.Node nd = new BP.WF.Node(nodeID);

            //给变量赋值.
            //批处理的类型
            int selectval = int.Parse(this.GetRequestVal("DDL_BRole"));
            switch (selectval)
            {
                case 0:
                    nd.HisBatchRole = BP.WF.BatchRole.None;
                    break;
                case 1:
                    nd.HisBatchRole = BP.WF.BatchRole.Ordinary;
                    break;
                default:
                    nd.HisBatchRole = BP.WF.BatchRole.Group;
                    break;
            }
            //批处理的数量
            nd.BatchListCount = int.Parse(this.GetRequestVal("TB_BatchListCount"));
            //批处理的参数 
            string sbatchparas = "";
            if (this.GetRequestVal("CheckBoxIDs") != null)
            {
                sbatchparas = this.GetRequestVal("CheckBoxIDs");
            }
            nd.BatchParas = sbatchparas;
            nd.Update();

            return "保存成功.";
        }
        #endregion

        #region 发送阻塞模式
        public string BlockModel_Save()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            nd.BlockAlert = this.GetRequestVal("TB_Alert"); //提示信息.

            int val = this.GetRequestValInt("RB_BlockModel");
            nd.SetValByKey(BP.WF.Template.NodeAttr.BlockModel, val);
            if (nd.BlockModel == BP.WF.BlockModel.None)
                nd.BlockModel = BP.WF.BlockModel.None;

            if (nd.BlockModel == BP.WF.BlockModel.CurrNodeAll)
                nd.BlockModel = BP.WF.BlockModel.CurrNodeAll;

            if (nd.BlockModel == BP.WF.BlockModel.SpecSubFlow)
            {
                nd.BlockModel = BP.WF.BlockModel.SpecSubFlow;
                nd.BlockExp = this.GetRequestVal("TB_SpecSubFlow");
            }

            if (nd.BlockModel == BP.WF.BlockModel.BySQL)
            {
                nd.BlockModel = BP.WF.BlockModel.BySQL;
                nd.BlockExp = this.GetRequestVal("TB_SQL");
            }

            if (nd.BlockModel == BP.WF.BlockModel.ByExp)
            {
                nd.BlockModel = BP.WF.BlockModel.ByExp;
                nd.BlockExp = this.GetRequestVal("TB_Exp");
            }

            if (nd.BlockModel == BP.WF.BlockModel.SpecSubFlowNode)
            {
                nd.BlockModel = BP.WF.BlockModel.SpecSubFlowNode;
                nd.BlockExp = this.GetRequestVal("TB_SpecSubFlowNode");
            }
            if (nd.BlockModel == BP.WF.BlockModel.SameLevelSubFlow)
            {
                nd.BlockModel = BP.WF.BlockModel.SameLevelSubFlow;
                nd.BlockExp = this.GetRequestVal("TB_SameLevelSubFlow");
            }

            nd.BlockAlert = this.GetRequestVal("TB_Alert");
            nd.Update();

            return "保存成功.";
        }
        #endregion

        #region 可以撤销的节点
        public string CanCancelNodes_Save()
        {
            BP.WF.Template.NodeCancels rnds = new BP.WF.Template.NodeCancels();
            rnds.Delete(BP.WF.Template.NodeCancelAttr.FK_Node, this.FK_Node);

            BP.WF.Nodes nds = new Nodes();
            nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

            int i = 0;
            foreach (BP.WF.Node nd in nds)
            {
                string cb = this.GetRequestVal("CB_" + nd.NodeID);
                if (cb == null || cb == "")
                    continue;

                NodeCancel nr = new NodeCancel();
                nr.FK_Node = this.FK_Node;
                nr.CancelTo = nd.NodeID;
                nr.Insert();
                i++;
            }
            if (i == 0)
                return "请您选择要撤销的节点。";

            return "设置成功.";
        }
        #endregion

        #region 表单检查(CheckFrm.htm)
        public string CheckFrm_Check()
        {
            if (BP.Web.WebUser.No != "admin")
                return "err@只有管理员有权限进行此项操作！";

            if (string.IsNullOrWhiteSpace(this.FK_MapData))
                return "err@参数FK_MapData不能为空！";

            string msg = string.Empty;

            //1.检查字段扩展设置
            MapExts mes = new MapExts(this.FK_MapData);
            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            MapDtls dtls = new MapDtls(this.FK_MapData);
            Entity en = null;
            string fieldMsg = string.Empty;

            //1.1主表
            foreach (MapExt me in mes)
            {
                if (!string.IsNullOrWhiteSpace(me.AttrOfOper))
                {
                    en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, me.AttrOfOper);

                    if (en != null && !string.IsNullOrWhiteSpace(me.AttrsOfActive))
                        en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, me.AttrsOfActive);
                }

                if (en == null)
                {
                    me.DirectDelete();
                    msg += "删除扩展设置中MyPK=" + me.PKVal + "的设置项；<br />";
                }
            }

            //1.2明细表
            foreach (MapDtl dtl in dtls)
            {
                mes = new MapExts(dtl.No);
                attrs = new MapAttrs(dtl.No);

                foreach (MapExt me in mes)
                {
                    if (!string.IsNullOrWhiteSpace(me.AttrOfOper))
                    {
                        en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, me.AttrOfOper);

                        if (en != null && !string.IsNullOrWhiteSpace(me.AttrsOfActive))
                            en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, me.AttrsOfActive);
                    }

                    if (en == null)
                    {
                        me.DirectDelete();
                        msg += "删除扩展设置中MyPK=" + me.PKVal + "的设置项；<br />";
                    }
                }
            }

            //2.检查字段权限
            FrmFields ffs = new FrmFields();
            ffs.Retrieve(FrmFieldAttr.FK_MapData, this.FK_MapData);

            //2.1主表
            foreach (FrmField ff in ffs)
            {
                en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, ff.KeyOfEn);

                if (en == null)
                {
                    ff.DirectDelete();
                    msg += "删除字段权限中MyPK=" + ff.PKVal + "的设置项；<br />";
                }
            }

            //2.2明细表
            foreach (MapDtl dtl in dtls)
            {
                ffs = new FrmFields();
                ffs.Retrieve(FrmFieldAttr.FK_MapData, dtl.No);
                attrs = new MapAttrs(dtl.No);

                foreach (FrmField ff in ffs)
                {
                    en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, ff.KeyOfEn);

                    if (en == null)
                    {
                        ff.DirectDelete();
                        msg += "删除字段权限中MyPK=" + ff.PKVal + "的设置项；<br />";
                    }
                }
            }

            msg += "检查完成！";

            return msg;
        }
        #endregion

    }
}