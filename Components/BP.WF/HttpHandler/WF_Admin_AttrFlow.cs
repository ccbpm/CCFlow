﻿using System;
using System.Collections;
using System.Data;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    public class WF_Admin_AttrFlow : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_AttrFlow()
        {
        }

        #region 修改轨迹.
        public string EditTrackDtl_Init()
        {
            Track tk = new Track(this.FK_Flow, this.MyPK);
            return tk.Msg;
        }
        public string EditTrackDtl_Save()
        {
            string msg = this.GetRequestVal("Msg");
            string tackTable = "ND" + int.Parse(this.FK_Flow) + "Track";
            string sql = "UPDATE " + tackTable + " SET Msg='" + msg + "' WHERE MyPK='" + this.MyPK + "'";
            DBAccess.RunSQL(sql);
            return "修改成功";
        }
        public string EditTrackDtl_Delete()
        {
            string tackTable = "ND" + int.Parse(this.FK_Flow) + "Track";
            string sql = "DELETE FROM  " + tackTable + " WHERE MyPK='" + this.MyPK + "'";
            DBAccess.RunSQL(sql);
            return "删除成功.";
        }
        #endregion


        #region APICodeFEE_Init.
        /// <summary>
        /// 代码生成器.
        /// </summary>
        /// <returns></returns>
        public string APICodeFEE_Init()
        {
            if (string.IsNullOrWhiteSpace(FK_Flow))
                return "err@FK_Flow参数不能为空！";

            Flow flow = new Flow(this.FK_Flow);

            string tmpPath = "";

            if (BP.WF.Glo.Platform == Platform.CCFlow)
                tmpPath = SystemConfig.PathOfWebApp + @"\WF\Admin\AttrFlow\APICodeFEE.txt.CCFlow";
            else
                tmpPath = SystemConfig.PathOfWebApp + @"\WF\Admin\AttrFlow\APICodeFEE.txt.JFlow";

            if (System.IO.File.Exists(tmpPath) == false)
                return string.Format(@"未找到事件编写模板文件“{0}”，请联系管理员！", tmpPath);

            string Title = flow.Name + "[" + flow.No + "]";
            string code = DataType.ReadTextFile(tmpPath); //, System.Text.Encoding.UTF8).Replace("F001Templepte", string.Format("FEE{0}", flow.No)).Replace("@FlowName", flow.Name).Replace("@FlowNo", flow.No);
            code = code.Replace("F001Templepte", string.Format("FEE{0}", flow.No)).Replace("@FlowName", flow.Name).Replace("@FlowNo", flow.No);


            //此处将重要行标示出来，根据下面的数组中的项来检索重要行号
            string[] lineStrings = new[]
                                       {
                                           "namespace BP.FlowEvent",
                                           ": BP.WF.FlowEventBase",
                                           "public override string FlowMark",
                                           "public override string SendWhen()",
                                           "public override string SendSuccess()",
                                           "public override string SendError()",
                                           "public override string FlowOnCreateWorkID()",
                                           "public override string FlowOverBefore()",
                                           "public override string FlowOverAfter()",
                                           "public override string BeforeFlowDel()",
                                           "public override string AfterFlowDel()",
                                           "public override string SaveAfter()",
                                           "public override string SaveBefore()",
                                           "public override string UndoneBefore()",
                                           "public override string UndoneAfter()",
                                           "public override string ReturnBefore()",
                                           "public override string ReturnAfter()",
                                           "public override string AskerAfter()",
                                           "public override string AskerReAfter()"
                                       };



            string msg = "<script type=\"text/javascript\">SyntaxHighlighter.highlight();</script>";
            msg += string.Format("<pre type=\"syntaxhighlighter\" class=\"brush: csharp; html-script: false; highlight: [{2}]\" title=\"{0}[编号：{1}] 流程自定义事件代码生成\">", flow.Name, flow.No, APICodeFEE_Init_GetImportantLinesNumbers(lineStrings, code));
            msg += code.Replace("<", "&lt;");    //SyntaxHighlighter中，使用<Pre>包含的代码要将左尖括号改成其转义形式
            msg += "</pre>";

            return msg;
        }
        /// <summary>
        /// 获取重要行的标号连接字符串，如3,6,8
        /// </summary>
        /// <param name="lineInStrings">重要行中包含的字符串数组，只要行中包含其中的一项字符串，则这行就是重要行</param>
        /// <param name="str">要检索的字符串，使用Environment.NewLine分行</param>
        /// <returns></returns>
        private string APICodeFEE_Init_GetImportantLinesNumbers(string[] lineInStrings, string str)
        {
            string[] lines = str.Replace(Environment.NewLine, "`").Split('`');
            string nums = string.Empty;

            for (int i = 0; i < lines.Length; i++)
            {
                foreach (string instr in lineInStrings)
                {
                    if (lines[i].IndexOf(instr) != -1)
                    {
                        nums += (i + 1) + ",";
                        break;
                    }
                }
            }

            return nums.TrimEnd(',');
        }
        #endregion APICodeFEE_Init.

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

        #region 与业务表数据同步
        public string DTSBTable_Init()
        {
            DataSet ds = new DataSet();

            // 把流程信息放入.
            BP.WF.Flow fl = new BP.WF.Flow(this.FK_Flow);
            DataTable dtFlow = fl.ToDataTableField("Flow");
            ds.Tables.Add(dtFlow);

            //获得数据源的表.
            BP.Sys.SFDBSrc src = new SFDBSrc(fl.DTSDBSrc);
            DataTable dt = src.GetTables();
            if (src.DBSrcType == Sys.DBSrcType.Oracle || src.DBSrcType == Sys.DBSrcType.PostgreSQL)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
            }
            dt.TableName = "Tables";
            ds.Tables.Add(dt);


            //把节点信息放入.
            BP.WF.Nodes nds = new Nodes(this.FK_Flow);
            DataTable dtNode = nds.ToDataTableField("Nodes");
            ds.Tables.Add(dtNode);




            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        /// <summary>
        /// 与业务表数据同步
        /// </summary>
        /// <returns></returns>
        public string DTSBTable_Save()
        {
            //获取流程属性
            Flow flow = new Flow(this.FK_Flow);
            //获取主键方式
            DataDTSWay dtsWay = (DataDTSWay)this.GetRequestValInt("RB_DTSWay");

            FlowDTSTime dtsTime = (FlowDTSTime)this.GetRequestValInt("RB_DTSTime");

            flow.DTSWay = dtsWay;
            flow.DTSTime = dtsTime;

            if (flow.DTSWay == DataDTSWay.None)
            {
                flow.Update();
                return "保存成功.";
            }

            //保存配置信息
            flow.DTSDBSrc = this.GetRequestVal("DDL_DBSrc");
            flow.DTSBTable = this.GetRequestVal("DDL_Table");
            flow.DTSSpecNodes = this.GetRequestVal("CheckBoxIDs").TrimEnd(',');

            flow.DirectUpdate();
            return "保存成功";
        }

        #endregion
        #region 数据同步数据源变化时，关联表的列表发生变化
        public string DTSBTable_DBSrcChange()
        {
            string dbsrc = this.GetRequestVal("DDL_DBSrc");
            //绑定表. 
            BP.Sys.SFDBSrc src = new SFDBSrc(dbsrc);
            DataTable dt = src.GetTables();
            if (src.DBSrcType == Sys.DBSrcType.Oracle || src.DBSrcType == Sys.DBSrcType.PostgreSQL)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion

        #region 数据调度 - 字段映射.
        public string DTSBTableExt_Init()
        {
            //定义数据容器.
            DataSet ds = new DataSet();

            //获得数据表列.
            SFDBSrc src = new SFDBSrc(this.GetRequestVal("FK_DBSrc"));

            DataTable dtColms = src.GetColumns(this.GetRequestVal("TableName"));
            dtColms.TableName = "Cols";
            if (src.DBSrcType == Sys.DBSrcType.Oracle || src.DBSrcType == Sys.DBSrcType.PostgreSQL)
            {
                dtColms.Columns["NO"].ColumnName = "No";
                dtColms.Columns["NAME"].ColumnName = "Name";
            }
            ds.Tables.Add(dtColms); //列名.

            //属性列表.
            MapAttrs attrs = new MapAttrs("ND" + int.Parse(this.FK_Flow) + "Rpt");
            DataTable dtAttrs = attrs.ToDataTableStringField("Sys_MapAttr");
            ds.Tables.Add(dtAttrs);

            //加入流程配置信息
            Flow flow = new Flow(this.FK_Flow);
            DataTable dtFlow = flow.ToDataTableField("Flow");
            ds.Tables.Add(dtFlow);

            //转化成json,返回.
            return BP.Tools.Json.DataSetToJson(ds);
        }
        public string DTSBTableExt_Save()
        {
            string rpt = "ND" + int.Parse(this.FK_Flow) + "Rpt";
            Flow fl = new Flow(this.FK_Flow);
            MapAttrs attrs = new MapAttrs(rpt);

            string pk = this.GetRequestVal("DDL_OID");
            if (DataType.IsNullOrEmpty(pk) == true)
                return "err@必须设置业务表的主键，否则无法同步。";


            string lcStr = "";//要同步的流程字段
            string ywStr = "";//第三方字段
            string err = "";
            foreach (MapAttr attr in attrs)
            {
                int val = this.GetRequestValChecked("CB_" + attr.KeyOfEn);
                if (val == 0)
                    continue;

                string refField = this.GetRequestVal("DDL_" + attr.KeyOfEn);

                //如果选中的业务字段重复，抛出异常
                if (ywStr.Contains("@" + refField + "@"))
                {
                    err += "@配置【" + attr.KeyOfEn + " - " + attr.Name +
                        "】错误, 请确保选中业务字段的唯一性，该业务字段已经被其他字段所使用。";
                }
                lcStr += "" + attr.KeyOfEn + "=" + refField + "@";
                ywStr += "@" + refField + "@,";
            }

            //    BP.Web.Controls.RadioBtn rb = this.Pub1.GetRadioBtnByID("rb_workId");

            int pkModel = this.GetRequestValInt("PKModel");

            string ddl_key = this.GetRequestVal("DDL_OID");
            if (pkModel == 0)
            {
                if (ywStr.Contains("@" + ddl_key + "@"))
                {
                    err += "@请确保选中业务字段的唯一性，该业务字段【" + ddl_key +
                        "】已经被其他字段所使用。";
                }
                lcStr = "OID=" + ddl_key + "@" + lcStr;
                ywStr = "@" + ddl_key + "@," + ywStr;
            }
            else
            {
                if (ywStr.Contains("@" + ddl_key + "@"))
                {
                    err += "@请确保选中业务字段的唯一性，该业务字段【" + ddl_key +
                        "】已经被其他字段所使用。";
                }
                lcStr = "GUID=" + ddl_key + "@" + lcStr;
                ywStr = "@" + ddl_key + "@," + ywStr;
            }

            if (err != "")
                return "err@" + err;

            //lcStr = lcStr.Replace("@", "");
            ywStr = ywStr.Replace("@", "");


            //去除最后一个字符的操作
            if (DataType.IsNullOrEmpty(lcStr) || DataType.IsNullOrEmpty(ywStr))
            {
                return "err@要配置的内容为空...";
            }
            lcStr = lcStr.Substring(0, lcStr.Length - 1);
            ywStr = ywStr.Substring(0, ywStr.Length - 1);


            //数据存储格式   a,b,c@a_1,b_1,c_1
            fl.DTSFields = lcStr;
            fl.DTSBTablePK = pk;
            fl.DirectUpdate();

            return "设置成功.";
        }
        #endregion

        #region 前置导航save
        /// <summary>
        /// 前置导航save
        /// </summary>
        /// <returns></returns>
        public string StartGuide_Save()
        {
            try
            {
                //Flow en = new Flow();
                //en.No = this.FK_Flow;
                //en.Retrieve();

                //int val = this.GetRequestValInt("RB_StartGuideWay");

                //en.SetValByKey(BP.WF.Template.FlowAttr.StartGuideWay, val);

                //if (en.StartGuideWay == Template.StartGuideWay.None)
                //{
                //    en.StartGuideWay = BP.WF.Template.StartGuideWay.None;
                //}

                //if (en.StartGuideWay == Template.StartGuideWay.ByHistoryUrl)
                //{
                //    en.StartGuidePara1 = this.GetRequestVal("TB_ByHistoryUrl");
                //    en.StartGuidePara2 = "";
                //    en.StartGuideWay = BP.WF.Template.StartGuideWay.ByHistoryUrl;
                //}

                //if (en.StartGuideWay == Template.StartGuideWay.BySelfUrl)
                //{
                //    en.StartGuidePara1 = this.GetRequestVal("TB_SelfURL");
                //    en.StartGuidePara2 = "";
                //    en.StartGuideWay = BP.WF.Template.StartGuideWay.BySelfUrl;
                //}

                ////单条模式.
                //if (en.StartGuideWay == Template.StartGuideWay.BySQLOne)
                //{
                //    en.StartGuidePara1 = this.GetRequestVal("TB_BySQLOne1");  //查询语句.
                //    en.StartGuidePara2 = this.GetRequestVal("TB_BySQLOne2");  //列表语句.

                //    //@李国文.
                //    en.StartGuidePara3 = this.GetRequestVal("TB_BySQLOne3");  //单行赋值语句.
                //    en.StartGuideWay = BP.WF.Template.StartGuideWay.BySQLOne;
                //}
                ////多条模式
                //if (en.StartGuideWay == Template.StartGuideWay.BySQLMulti)
                //{
                //    en.StartGuidePara1 = this.GetRequestVal("TB_BySQLMulti1");  //查询语句.
                //    en.StartGuidePara2 = this.GetRequestVal("TB_BySQLMulti2");  //列表语句.
                //    en.StartGuideWay = BP.WF.Template.StartGuideWay.BySQLMulti;
                //}
                ////多条-子父流程-合卷审批.
                //if (en.StartGuideWay == Template.StartGuideWay.SubFlowGuide)
                //{
                //    en.StartGuidePara1 = this.GetRequestVal("TB_SubFlow1");  //查询语句.
                //    en.StartGuidePara2 = this.GetRequestVal("TB_SubFlow2");  //列表语句.
                //    en.StartGuideWay = BP.WF.Template.StartGuideWay.SubFlowGuide;
                //}

                //BP.WF.Template.FrmNodes fns = new BP.WF.Template.FrmNodes(int.Parse(this.FK_Flow + "01"));
                //if (fns.Count >= 2)
                //{
                //    if (en.StartGuideWay == Template.StartGuideWay.ByFrms)
                //        en.StartGuideWay = BP.WF.Template.StartGuideWay.ByFrms;
                //}

                ////右侧的超链接.
                //en.StartGuideLink = this.GetRequestVal("TB_GuideLink");
                //en.StartGuideLab = this.GetRequestVal("TB_GuideLab");

               // en.Update();
                return "保存成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion

        #region 流程轨迹查看权限
        /// <summary>
        /// 流程轨迹查看权限
        /// </summary>
        /// <returns></returns>
        public string TruckViewPower_Save()
        {
            try
            {
                BP.WF.Template.TruckViewPower en = new BP.WF.Template.TruckViewPower(FK_Flow);
                en.Retrieve();

                en = BP.Pub.PubClass.CopyFromRequestByPost(en) as BP.WF.Template.TruckViewPower;
                en.Save();  //执行保存.
                return "保存成功";
            }
            catch
            {
                return "err@保存失败";
            }

        }
        #endregion 流程轨迹查看权限save

        #region 数据导入.
        /// <summary>
        /// 流程模版导入.
        /// </summary>
        /// <returns></returns>
        public string Imp_Done()
        {
            var files = HttpContextHelper.RequestFiles();  //context.Request.Files;
            if (files.Count == 0)
                return "err@请选择要上传的流程模版。";

            //设置文件名
            string fileNewName = DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" + System.IO.Path.GetFileName(files[0].FileName);

            //文件存放路径
            string filePath = SystemConfig.PathOfTemp + "\\" + fileNewName;
            //files[0].SaveAs(filePath);
            HttpContextHelper.UploadFile(files[0], filePath);

            string flowNo = this.FK_Flow;
            string FK_FlowSort = this.GetRequestVal("FK_Sort");
            //检查流程编号
            if (DataType.IsNullOrEmpty(flowNo) == false)
            {
                Flow fl = new Flow(flowNo);
                FK_FlowSort = fl.FK_FlowSort;
            }
            //检查流程类别编号
            if (DataType.IsNullOrEmpty(FK_FlowSort))
                return "err@所选流程类别编号不存在。";

            //导入模式
            BP.WF.ImpFlowTempleteModel model = (BP.WF.ImpFlowTempleteModel)this.GetRequestValInt("ImpWay");
            if (model == ImpFlowTempleteModel.AsSpecFlowNo)
                flowNo = this.GetRequestVal("SpecFlowNo");

            //执行导入
            BP.WF.Flow flow = BP.WF.Template.TemplateGlo.LoadFlowTemplate(FK_FlowSort, filePath, model, flowNo);
            flow.DoCheck(); //要执行一次检查.

            Hashtable ht = new Hashtable();
            ht.Add("FK_Flow", flow.No);
            ht.Add("FlowName", flow.Name);
            ht.Add("FK_FlowSort", flow.FK_FlowSort);
            ht.Add("Msg", "导入成功,流程编号为:" + flow.No + "名称为:" + flow.Name);
            return BP.Tools.Json.ToJson(ht);
        }
        #endregion 数据导入.

        #region 修改node Icon.
        /// <summary>
        /// 修改节点ICON
        /// </summary>
        /// <returns></returns>
        public string NodesIcon_Init()
        {
            DataSet ds = new System.Data.DataSet();
            Nodes nds = new Nodes(this.FK_Flow);
            DataTable dt = nds.ToDataTableField("Nodes");
            ds.Tables.Add(dt);

            //把文件放入ds.
            string path = SystemConfig.PathOfWebApp + "WF\\Admin\\ClientBin\\NodeIcon\\";
            string[] strs = System.IO.Directory.GetFiles(path);
            DataTable dtIcon = new System.Data.DataTable();
            dtIcon.Columns.Add("No");
            dtIcon.Columns.Add("Name");
            foreach (string str in strs)
            {
                string fileName = str.Substring(str.LastIndexOf("\\") + 1);
                fileName = fileName.Substring(0, fileName.LastIndexOf("."));

                DataRow dr = dtIcon.NewRow();
                dr[0] = fileName;
                dr[1] = fileName;
                dtIcon.Rows.Add(dr);
            }

            dtIcon.TableName = "ICONs";
            ds.Tables.Add(dtIcon);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 修改node Icon.

        /// <summary>
        /// 流程时限消息设置
        /// </summary>
        /// <returns></returns>
        public string PushMsgEntity_Init()
        {
            DataSet ds = new DataSet();

            //流程上的字段
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs();
            attrs.Retrieve(BP.Sys.MapAttrAttr.FK_MapData, "ND" + int.Parse(this.FK_Flow) + "rpt", "LGType", 0, "MyDataType", 1);
            ds.Tables.Add(attrs.ToDataTableField("FrmFields"));

            //节点 
            BP.WF.Nodes nds = new BP.WF.Nodes(this.FK_Flow);
            ds.Tables.Add(nds.ToDataTableField("Nodes"));

            //mypk
            BP.WF.Template.PushMsg msg = new BP.WF.Template.PushMsg();
            msg.MyPK = this.MyPK;
            msg.RetrieveFromDBSources();
            ds.Tables.Add(msg.ToDataTableField("PushMsgEntity"));

            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        /// <summary>
        /// 流程时限消息设置
        /// </summary>
        /// <returns></returns>
        public string PushMsg_Save()
        {
            BP.WF.Template.PushMsg msg = new BP.WF.Template.PushMsg();
            msg.MyPK = this.MyPK;
            msg.RetrieveFromDBSources();

            msg.FK_Event = this.FK_Event;  //流程时限规则
            msg.FK_Flow = this.FK_Flow;

            BP.WF.Nodes nds = new BP.WF.Nodes(this.FK_Flow);

            #region 求出来选择的节点.
            string nodesOfSMS = "";
            string nodesOfEmail = "";
            foreach (BP.WF.Node mynd in nds)
            {
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (key.Contains("CB_Station_" + mynd.NodeID)
                        && nodesOfSMS.Contains(mynd.NodeID + "") == false)
                        nodesOfSMS += mynd.NodeID + ",";

                    if (key.Contains("CB_SMS_" + mynd.NodeID)
                        && nodesOfSMS.Contains(mynd.NodeID + "") == false)
                        nodesOfSMS += mynd.NodeID + ",";

                    if (key.Contains("CB_Email_" + mynd.NodeID)
                        && nodesOfEmail.Contains(mynd.NodeID + "") == false)
                        nodesOfEmail += mynd.NodeID + ",";
                }
            }

            //节点.
            msg.MailNodes = nodesOfEmail;
            msg.SMSNodes = nodesOfSMS;
            #endregion 求出来选择的节点.

            #region 短信保存.
            //短消息发送设备
            msg.SMSPushModel = this.GetRequestVal("PushModel");

            //短信推送方式。
            msg.SMSPushWay = Convert.ToInt32(HttpContextHelper.RequestParams("RB_SMS").Replace("RB_SMS_", ""));

            //短信手机字段.
            msg.SMSField = HttpContextHelper.RequestParams("DDL_SMS_Fields");
            //替换变量
            string smsstr = HttpContextHelper.RequestParams("TB_SMS");
            //扬玉慧 此处是配置界面  不应该把用户名和用户编号转化掉
            //smsstr = smsstr.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            //smsstr = smsstr.Replace("@WebUser.No", BP.Web.WebUser.No);

            // smsstr = smsstr.Replace("@RDT",);
            //短信内容模版.
            msg.SMSDoc_Real = smsstr;
            #endregion 短信保存.

            #region 邮件保存.
            //邮件.
            //msg.MailPushWay = Convert.ToInt32(HttpContext.Current.Request["RB_Email"].ToString().Replace("RB_Email_", "")); ;
            //2019-07-25 zyt改造
            msg.MailPushWay = Convert.ToInt32(HttpContextHelper.RequestParams("RB_Email").Replace("RB_Email_", ""));
            //邮件标题与内容.
            msg.MailTitle_Real = HttpContextHelper.RequestParams("TB_Email_Title");
            msg.MailDoc_Real = HttpContextHelper.RequestParams("TB_Email_Doc");

            //邮件地址.
            msg.MailAddress = HttpContextHelper.RequestParams("DDL_Email_Fields");

            #endregion 邮件保存.

            //保存.
            if (DataType.IsNullOrEmpty(msg.MyPK) == true)
            {
                msg.MyPK = DBAccess.GenerGUID();
                msg.Insert();
            }
            else
            {
                msg.Update();
            }

            return "保存成功..";
        }

        #region 欢迎页面初始化.
        /// <summary>
        /// 欢迎页面初始化-获得数量.
        /// </summary>
        /// <returns></returns>
        public string GraphicalAnalysis_Init()
        {
            Hashtable ht = new Hashtable();
            string fk_flow = GetRequestVal("FK_Flow");
            //所有的实例数量.
            ht.Add("FlowInstaceNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState >1 AND Fk_flow = '" + fk_flow + "'")); //实例数.

            //所有的待办数量.
            ht.Add("TodolistNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=2 AND Fk_flow = '" + fk_flow + "'"));

            //所有的运行中的数量.
            ht.Add("RunNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFSta!=1 AND WFState!=3 AND Fk_flow = '" + fk_flow + "'"));

            //退回数.
            ht.Add("ReturnNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=5 AND Fk_flow = '" + fk_flow + "'"));

            //说有逾期的数量.
            if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                ht.Add("OverTimeNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM WF_EMPWORKS where STR_TO_DATE(SDT,'%Y-%m-%d %H:%i') < now() AND Fk_flow = '" + fk_flow + "'"));

            }
            else if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                string sql = "SELECT COUNT(*) from (SELECT *  FROM WF_EMPWORKS WHERE  REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(sysdate - TO_DATE(SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 AND Fk_flow = '" + fk_flow + "'";

                sql += "UNION SELECT* FROM WF_EMPWORKS WHERE  REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (sysdate - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 AND Fk_flow = '" + fk_flow + "')";

                ht.Add("OverTimeNum", DBAccess.RunSQLReturnValInt(sql));
            }
            else
            {
                ht.Add("OverTimeNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM WF_EMPWORKS where convert(varchar(100),SDT,120) < CONVERT(varchar(100), GETDATE(), 120) AND Fk_flow = '" + fk_flow + "'"));
            }

            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 获得数量  流程饼图，部门柱状图，月份折线图.
        /// </summary>
        /// <returns></returns>
        public string GraphicalAnalysis_DataSet()
        {
            DataSet ds = new DataSet();
            string fk_flow = GetRequestVal("FK_Flow");
            #region  实例分析
            //月份分组.
            string sql = "SELECT FK_NY, count(WorkID) as Num FROM WF_GenerWorkFlow WHERE WFState >1 AND Fk_flow = '" + fk_flow + "' GROUP BY FK_NY";
            DataTable FlowsByNY = DBAccess.RunSQLReturnTable(sql);
            FlowsByNY.TableName = "FlowsByNY";
            ds.Tables.Add(FlowsByNY);

            //部门分组.
            sql = "SELECT DeptName, count(WorkID) as Num FROM WF_GenerWorkFlow WHERE WFState >1 AND Fk_flow = '" + fk_flow + "' GROUP BY DeptName ";
            DataTable FlowsByDept = DBAccess.RunSQLReturnTable(sql);
            FlowsByDept.TableName = "FlowsByDept";
            ds.Tables.Add(FlowsByDept);
            #endregion 实例分析。


            #region 待办 分析
            //待办 - 部门分组.
            sql = "SELECT DeptName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 AND Fk_flow = '" + fk_flow + "' GROUP BY DeptName";
            DataTable TodolistByDept = DBAccess.RunSQLReturnTable(sql);
            TodolistByDept.TableName = "TodolistByDept";
            ds.Tables.Add(TodolistByDept);

            //逾期的 - 人员分组.
            if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT  p.name,COUNT (w.WorkID) AS Num from Port_Emp p,WF_EmpWorks w  WHERE p. NO = w.FK_Emp AND WFState >1 and STR_TO_DATE(SDT,'%Y-%m-%d %H:%i') < now() AND Fk_flow = '" + fk_flow + "' GROUP BY p.name,w.FK_Emp";

            }
            else if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sql = "SELECT  p.name,COUNT (w.WorkID) AS Num from Port_Emp p,WF_EmpWorks w  WHERE p. NO = w.FK_Emp AND WFState >1 and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(sysdate - TO_DATE(SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 AND Fk_flow = '" + fk_flow + "' GROUP BY p.name,w.FK_Emp ";
                sql += "UNION SELECT  p.name,COUNT (w.WorkID) AS Num from Port_Emp p,WF_EmpWorks w  WHERE p. NO = w.FK_Emp AND WFState >1 and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (sysdate - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 AND Fk_flow = '" + fk_flow + "' GROUP BY p.name,w.FK_Emp";
            }
            else
            {
                sql = "SELECT  p.name,COUNT (w.WorkID) AS Num from Port_Emp p,WF_EmpWorks w  WHERE p. NO = w.FK_Emp AND WFState >1 and convert(varchar(100),SDT,120) < CONVERT(varchar(100), GETDATE(), 120) AND Fk_flow = '" + fk_flow + "' GROUP BY p.name,w.FK_Emp";
            }
            DataTable OverTimeByEmp = DBAccess.RunSQLReturnTable(sql);
            OverTimeByEmp.TableName = "OverTimeByEmp";
            ds.Tables.Add(OverTimeByEmp);
            //逾期的 - 部门分组.
            if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT DeptName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 and STR_TO_DATE(SDT,'%Y-%m-%d %H:%i') < now() AND Fk_flow = '" + fk_flow + "' GROUP BY DeptName";

            }
            else if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sql = "SELECT DeptName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(sysdate - TO_DATE(SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 AND Fk_flow = '" + fk_flow + "' GROUP BY DeptName ";
                sql += "UNION SELECT DeptName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (sysdate - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 AND Fk_flow = '" + fk_flow + "' GROUP BY DeptName";
            }
            else
            {
                sql = "SELECT DeptName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 and convert(varchar(100),SDT,120) < CONVERT(varchar(100), GETDATE(), 120) AND Fk_flow = '" + fk_flow + "' GROUP BY DeptName";
            }
            DataTable OverTimeByDept = DBAccess.RunSQLReturnTable(sql);
            OverTimeByDept.TableName = "OverTimeByDept";
            ds.Tables.Add(OverTimeByDept);
            //逾期的 - 节点分组.
            if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "Select NodeName,count(*) as Num from WF_EmpWorks WHERE WFState >1 and STR_TO_DATE(SDT,'%Y-%m-%d %H:%i') < now() AND Fk_flow = '" + fk_flow + "' GROUP BY NodeName";

            }
            else if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sql = "Select NodeName,count(*) as Num from WF_EmpWorks WHERE WFState >1 and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(sysdate - TO_DATE(SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 AND Fk_flow = '" + fk_flow + "' GROUP BY NodeName ";
                sql += "UNION Select NodeName,count(*) as Num from WF_EmpWorks WHERE WFState >1 and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (sysdate - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 AND Fk_flow = '" + fk_flow + "' GROUP BY NodeName";
            }
            else
            {
                sql = "Select NodeName,count(*) as Num from WF_EmpWorks WHERE WFState >1 and convert(varchar(100),SDT,120) < CONVERT(varchar(100), GETDATE(), 120) AND Fk_flow = '" + fk_flow + "' GROUP BY NodeName";
            }
            DataTable OverTimeByNode = DBAccess.RunSQLReturnTable(sql);
            OverTimeByNode.TableName = "OverTimeByNode";
            ds.Tables.Add(OverTimeByNode);
            #endregion 逾期。


            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 欢迎页面初始化.


        #region 泳道图.
        /// <summary>
        /// 泳道图
        /// </summary>
        /// <returns></returns>
        public string Lane_Init()
        {
            BP.WF.Template.Lanes ens = new Lanes(this.FK_Flow);
            return ens.ToJson();
        }
        #endregion 泳道图.

    }
}
