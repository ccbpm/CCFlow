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

        #region 数据同步.
        public string DTSBTable_Init()
        {
            return "";
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

            return "";

            //fl.StartLimitAlert = this.TB_Alert.Text; //限制提示信息

            //if (this.RB_None.Checked)
            //{
            //    fl.StartLimitRole = StartLimitRole.None;
            //}

            //if (this.RB_ByTime.Checked)
            //{
            //    if (this.DDL_ByTime.SelectedIndex == 0)//一人一天一次
            //    {
            //        fl.StartLimitRole = StartLimitRole.Day;
            //        fl.StartLimitPara = this.TB_ByTimePara.Text;
            //    }

            //    if (this.DDL_ByTime.SelectedIndex == 1)//一人一周一次
            //    {
            //        fl.StartLimitRole = StartLimitRole.Week;
            //        fl.StartLimitPara = this.TB_ByTimePara.Text;
            //    }

            //    if (this.DDL_ByTime.SelectedIndex == 2)//一人一月一次
            //    {
            //        fl.StartLimitRole = StartLimitRole.Month;
            //        fl.StartLimitPara = this.TB_ByTimePara.Text;
            //    }

            //    if (this.DDL_ByTime.SelectedIndex == 3)//一人一季一次
            //    {
            //        fl.StartLimitRole = StartLimitRole.JD;
            //        fl.StartLimitPara = this.TB_ByTimePara.Text;
            //        fl.DirectUpdate();
            //    }

            //    if (this.DDL_ByTime.SelectedIndex == 4)//一人一年一次
            //    {
            //        fl.StartLimitRole = StartLimitRole.Year;
            //        fl.StartLimitPara = this.TB_ByTimePara.Text;
            //    }
            //}

            //if (this.RB_ColNotExit.Checked)//按照发起字段不能重复规则
            //{
            //    fl.StartLimitRole = StartLimitRole.ColNotExit;
            //    fl.StartLimitPara = this.TB_ColNotExit_Fields.Text;
            //}

            //if (this.RB_SQL.Checked == true)
            //{
            //    //字段参数.
            //    fl.StartLimitPara = this.TB_SQL_Para.Text;

            //    //选择的模式.
            //    if (this.DDL_SQL.SelectedIndex == 0)
            //        fl.StartLimitRole = StartLimitRole.ResultIsZero;

            //    if (this.DDL_SQL.SelectedIndex == 1)
            //        fl.StartLimitRole = StartLimitRole.ResultIsNotZero;
            //}

            fl.Update();
        }
        #endregion 发起限制.

        #region 流程字段列表
        /// <summary>
        /// 执行流程检查.
        /// </summary>
        /// <returns></returns>
        public string CheckFlow_Init()
        {
            BP.WF.Flow fl = new BP.WF.Flow(this.FK_Flow);
            string str = fl.DoCheck();
            str = str.Replace("@", "<BR>@");

            if (str == "")
                str = "检查成功.";
            return str;
        }
        /// <summary>
        /// 流程字段列表
        /// </summary>
        /// <returns></returns>
        public string FlowFields_Init()
        {
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + int.Parse(this.FK_Flow) + "Rpt");
            return attrs.ToJson();
        }
        #endregion

        #region 自动发起.
        /// <summary>
        /// 执行初始化
        /// </summary>
        /// <returns></returns>
        public string AutoStart_Init()
        {
            BP.WF.Flow en = new BP.WF.Flow();
            en.No = this.FK_Flow;
            en.RetrieveFromDBSources();

            return en.ToJson();
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string AutoStart_Save()
        {
            //执行保存.
            BP.WF.Flow en = new BP.WF.Flow(this.FK_Flow);

            en.SetValByKey(BP.WF.Template.FlowAttr.FlowRunWay, this.GetRequestValInt("RB_FlowRunWay"));

            if (en.HisFlowRunWay == FlowRunWay.SpecEmp)
                en.RunObj = this.GetRequestVal("TB_SpecEmp");

            if (en.HisFlowRunWay == FlowRunWay.DataModel)
                en.RunObj = this.GetRequestVal("TB_DataModel");

            en.DirectUpdate();
            return "保存成功.";
        }
        #endregion 自动发起.

        #region 节点属性（列表）的操作
        /// <summary>
        /// 初始化节点属性列表.
        /// </summary>
        /// <returns></returns>
        public string NodeAttrs_Init()
        {
            var strFlowId = GetRequestVal("FK_Flow");
            if (string.IsNullOrEmpty(strFlowId))
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
        /// <summary>
        /// 与业务表数据同步
        /// </summary>
        /// <returns></returns>
        public string DTSBTbale_Init()
        {
            BP.WF.Flow fl = new BP.WF.Flow(this.FK_Flow);
            return fl.ToJson();
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
        public string TruckViewPower_Init() { 
            if (string.IsNullOrEmpty(FK_Flow))
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
            try {
                BP.WF.Template.TruckViewPower en = new BP.WF.Template.TruckViewPower(FK_Flow);
                en.Retrieve();

                en = BP.Sys.PubClass.CopyFromRequestByPost(en, context.Request) as BP.WF.Template.TruckViewPower;
                en.Save();  //执行保存.
                return "保存成功";
            }
            catch {
                return "err@保存失败";
            }
            
        }
        #endregion 流程轨迹查看权限save
    }
}
