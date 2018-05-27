using System;
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
using System.Data;
using System.Web.UI;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    public class WF_Admin_AttrFlow : BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_AttrFlow(HttpContext mycontext)
        {
            this.context = mycontext;
        }

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
            string code = BP.DA.DataType.ReadTextFile(tmpPath); //, System.Text.Encoding.UTF8).Replace("F001Templepte", string.Format("FEE{0}", flow.No)).Replace("@FlowName", flow.Name).Replace("@FlowNo", flow.No);
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


            //this.Pub1.AddLi(string.Format("<a href=\"APICodeFEE.aspx?FK_Flow={0}&Download=1\" target=\"_blank\" class=\"easyui-linkbutton\" data-options=\"iconCls:'icon-save',plain:true\">下载代码</a><br />", FK_Flow));

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

        #region 发起限制.
        public string Limit_Init()
        {
            BP.WF.Flow fl = new BP.WF.Flow();
            fl.No = this.FK_Flow;
            fl.RetrieveFromDBSources();
            return fl.ToJson();
        }
        public string Limit_Save()
        {
            BP.WF.Flow fl = new BP.WF.Flow(this.FK_Flow);
            fl.StartLimitRole = (StartLimitRole)this.GetRequestValInt("StartLimitRole");
            fl.StartLimitPara = this.GetRequestVal("StartLimitPara");
            fl.StartLimitAlert = this.GetRequestVal("StartLimitAlert");
            fl.Update();
            return "保存成功.";
        }
        #endregion 发起限制.



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

            //获得数据源的表.
            BP.Sys.SFDBSrc src = new SFDBSrc("local");
            DataTable dt = src.GetTables();
            dt.TableName = "Tables";
            ds.Tables.Add(dt);


            //把节点信息放入.
            BP.WF.Nodes nds = new Nodes(this.FK_Flow);
            DataTable dtNode = nds.ToDataTableField("Nodes");
            ds.Tables.Add(dtNode);


            // 把流程信息放入.
            BP.WF.Flow fl = new BP.WF.Flow(this.FK_Flow);
            DataTable dtFlow = fl.ToDataTableField(this.FK_Flow);
            ds.Tables.Add(dtFlow);

            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        /// <summary>
        /// 与业务表数据同步
        /// </summary>
        /// <returns></returns>
        public string DTSBTable_Save()
        {
            Flow flow = new Flow(this.FK_Flow);

            BP.WF.Template.FlowDTSWay dtsWay = (BP.WF.Template.FlowDTSWay)this.GetRequestValInt("RB_DTSWay");

            flow.DTSWay = dtsWay;
            if (flow.DTSWay == FlowDTSWay.None)
            {
                flow.Update();
                return "保存成功.";
            }

            flow.DTSDBSrc = this.GetRequestVal("DDL_DBSrc");
            flow.DTSBTable = this.GetRequestVal("DDL_Table");

            DTSField field = (DTSField)this.GetRequestValInt("DTSField");

            if (field == 0)
                field = DTSField.SameNames;
            flow.DTSField = field;

            SFDBSrc s = new SFDBSrc("local");
            if (field == DTSField.SameNames)
            {
                DataTable dt = s.GetColumns(flow.PTable);

                s = new SFDBSrc(flow.DTSDBSrc);// this.src);
                DataTable ywDt = s.GetColumns(flow.DTSBTable);// this.ywTableName);

                string str = "";
                string ywStr = "";
                foreach (DataRow ywDr in ywDt.Rows)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (ywDr["No"].ToString().ToUpper() == dr["No"].ToString().ToUpper())
                        {
                            if (dr["No"].ToString().ToUpper() == "OID")
                            {
                                flow.DTSBTablePK = "OID";
                            }
                            str += dr["No"].ToString() + ",";
                            ywStr += ywDr["No"].ToString() + ",";
                        }
                    }
                }

                if (!DataType.IsNullOrEmpty(str))
                    flow.DTSFields = str.TrimEnd(',') + "@" + ywStr.TrimEnd(',');
                else
                {
                    PubClass.Alert("未检测到业务主表【" + flow.PTable + "】与表【" + flow.DTSBTable + "】有相同的字段名.");
                    return "";//不执行保存
                }
            }
            else//按设置的字段匹配   检查在
            {
                try
                {
                    s = new SFDBSrc("local");
                    string str = flow.DTSFields;

                    string[] arr = str.Split('@');


                    string sql = "SELECT " + arr[0] + " FROM " + flow.PTable;

                    s.RunSQL(sql);

                    s = new SFDBSrc(flow.DTSDBSrc);

                    sql = "SELECT " + arr[1] + ", " + flow.DTSBTablePK
                        + " FROM " + flow.DTSBTable;

                    s.RunSQL(sql);

                }
                catch
                {
                    //PubClass.Alert(ex.Message);
                    PubClass.Alert("设置的字段有误.【" + flow.DTSFields + "】");
                    return "";//不执行保存
                }
            }
            flow.Update();
            return flow.ToJson();
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

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtColms.Columns["NO"].ColumnName = "No";
                dtColms.Columns["NAME"].ColumnName = "Name";
            }

            ds.Tables.Add(dtColms);

            //属性列表.
            MapAttrs attrs = new MapAttrs("ND" + int.Parse(this.FK_Flow) + "Rpt");
            DataTable dtAttrs = attrs.ToDataTableStringField("Sys_MapAttr");
            ds.Tables.Add(dtAttrs);

            //转化成json,返回.
            return BP.Tools.Json.DataSetToJson(ds, false);
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
                int val = this.GetRequestValInt("CB_" + attr.KeyOfEn);
                if (val == 0)
                    continue;

                string refField = this.GetRequestVal("DDL_" + attr.KeyOfEn);

                //如果选中的业务字段重复，抛出异常
                if (ywStr.Contains("@" + refField + "@"))
                {
                    err += "@配置【" + attr.KeyOfEn + " - " + attr.Name +
                        "】错误, 请确保选中业务字段的唯一性，该业务字段已经被其他字段所使用。";
                }
                lcStr += "@" + attr.KeyOfEn + "@,";
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
                lcStr = "@OID@," + lcStr;
                ywStr = "@" + ddl_key + "@," + ywStr;
            }
            else
            {
                if (ywStr.Contains("@" + ddl_key + "@"))
                {
                    err += "@请确保选中业务字段的唯一性，该业务字段【" + ddl_key +
                        "】已经被其他字段所使用。";
                }
                lcStr = "@GUID@," + lcStr;
                ywStr = "@" + ddl_key + "@," + ywStr;
            }

            if (err != "")
                return "err@" + err;

            lcStr = lcStr.Replace("@", "");
            ywStr = ywStr.Replace("@", "");


            //去除最后一个字符的操作
            if (DataType.IsNullOrEmpty(lcStr) || DataType.IsNullOrEmpty(ywStr))
            {
                return "err@要配置的内容为空...";
            }
            lcStr = lcStr.Substring(0, lcStr.Length - 1);
            ywStr = ywStr.Substring(0, ywStr.Length - 1);


            //数据存储格式   a,b,c@a_1,b_1,c_1
            fl.DTSFields = lcStr + "@" + ywStr;
            fl.DTSBTablePK = pk;
            fl.Update();

            return "设置成功.";
        }
        #endregion

        #region 前置导航
        /// <summary>
        /// 前置导航
        /// </summary>
        /// <returns></returns>
        public string StartGuide_Init()
        {
            BP.WF.Flow en = new BP.WF.Flow();
            en.No = this.FK_Flow;
            en.RetrieveFromDBSources();

            BP.WF.Template.FrmNodes fns = new BP.WF.Template.FrmNodes(int.Parse(this.FK_Flow + "01"));


            if (fns.Count > 2)
            {
                en.Row.Add("nodesCount", "true");
            }
            else
            {
                en.Row.Add("nodesCount", "false");
            }

            en.Row.Add("userId", BP.Web.WebUser.SID);
            //en.Row = ht;
            return en.ToJson();

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
                Flow en = new Flow();
                en.No = this.FK_Flow;
                en.Retrieve();

                int val = this.GetRequestValInt("RB_StartGuideWay");

                en.SetValByKey(BP.WF.Template.FlowAttr.StartGuideWay, val);

                if (en.StartGuideWay == Template.StartGuideWay.None)
                {
                    en.StartGuideWay = BP.WF.Template.StartGuideWay.None;
                }

                if (en.StartGuideWay == Template.StartGuideWay.ByHistoryUrl)
                {
                    en.StartGuidePara1 = this.GetRequestVal("TB_ByHistoryUrl");
                    en.StartGuidePara2 = "";
                    en.StartGuideWay = BP.WF.Template.StartGuideWay.ByHistoryUrl;
                }

                if (en.StartGuideWay == Template.StartGuideWay.BySelfUrl)
                {
                    en.StartGuidePara1 = this.GetRequestVal("TB_SelfURL");
                    en.StartGuidePara2 = "";
                    en.StartGuideWay = BP.WF.Template.StartGuideWay.BySelfUrl;
                }

                //单条模式.
                if (en.StartGuideWay == Template.StartGuideWay.BySQLOne)
                {
                    en.StartGuidePara1 = this.GetRequestVal("TB_BySQLOne1");  //查询语句.
                    en.StartGuidePara2 = this.GetRequestVal("TB_BySQLOne2");  //列表语句.
                    en.StartGuideWay = BP.WF.Template.StartGuideWay.BySQLOne;
                }
                //多条模式
                if (en.StartGuideWay == Template.StartGuideWay.BySQLMulti)
                {
                    en.StartGuidePara1 = this.GetRequestVal("TB_BySQLMulti1");  //查询语句.
                    en.StartGuidePara2 = this.GetRequestVal("TB_BySQLMulti2");  //列表语句.
                    en.StartGuideWay = BP.WF.Template.StartGuideWay.BySQLMulti;
                }
                //多条-子父流程-合卷审批.
                if (en.StartGuideWay == Template.StartGuideWay.SubFlowGuide)
                {
                    en.StartGuidePara1 = this.GetRequestVal("TB_SubFlow1");  //查询语句.
                    en.StartGuidePara2 = this.GetRequestVal("TB_SubFlow2");  //列表语句.
                    en.StartGuideWay = BP.WF.Template.StartGuideWay.SubFlowGuide;
                }



                BP.WF.Template.FrmNodes fns = new BP.WF.Template.FrmNodes(int.Parse(this.FK_Flow + "01"));
                if (fns.Count >= 2)
                {
                    if (en.StartGuideWay == Template.StartGuideWay.ByFrms)
                        en.StartGuideWay = BP.WF.Template.StartGuideWay.ByFrms;
                }

                //右侧的超链接.
                en.StartGuideLink = this.GetRequestVal("TB_GuideLink");
                en.StartGuideLab = this.GetRequestVal("TB_GuideLab");

                en.Update();
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
        public string TruckViewPower_Init()
        {
            if (DataType.IsNullOrEmpty(FK_Flow))
            {
                throw new Exception("流程编号为空");
                return "err@流程编号为空";
            }
            else
            {
                BP.WF.Template.TruckViewPower en = new BP.WF.Template.TruckViewPower(FK_Flow);
                en.Retrieve();
                return en.ToJson();
            }
        }
        #endregion 流程轨迹查看权限save

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

                en = BP.Sys.PubClass.CopyFromRequestByPost(en, context.Request) as BP.WF.Template.TruckViewPower;
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
            HttpFileCollection files = context.Request.Files;
            if (files.Count == 0)
                return "err@请选择要上传的流程模版。";

            //设置文件名
            string fileNewName = DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" + System.IO.Path.GetFileName(files[0].FileName);

            //文件存放路径
            string filePath = BP.Sys.SystemConfig.PathOfTemp + "\\" + fileNewName;
            files[0].SaveAs(filePath);

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
            BP.WF.Flow flow = BP.WF.Flow.DoLoadFlowTemplate(FK_FlowSort, filePath, model, flowNo);

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
            string path = SystemConfig.PathOfWebApp + "\\WF\\Admin\\ClientBin\\NodeIcon\\";
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
        public string NodesIconSelect_Save()
        {
            string icon = this.GetRequestVal("ICON");

            Node nd = new Node(this.FK_Node);
            nd.ICON = icon;
            nd.Update();

            return "保存成功...";
        }
        #endregion 修改node Icon.


    }
}
