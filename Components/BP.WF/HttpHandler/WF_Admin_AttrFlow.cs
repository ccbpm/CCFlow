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
    public class WF_Admin_AttrFlow : BP.WF.HttpHandler.WebContralBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_AttrFlow(HttpContext mycontext)
        {
            this.context = mycontext;
        }

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
            BP.WF.Flow en = new BP.WF.Flow(this.FK_Flow);
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

            //if (this.RB_HandWork.Checked)
            //{
            //    en.HisFlowRunWay = BP.WF.FlowRunWay.HandWork;
            //}

            //if (this.RB_SpecEmp.Checked)
            //{
            //    en.HisFlowRunWay = BP.WF.FlowRunWay.SpecEmp;
            //    en.RunObj = this.TB_SpecEmp.Text;
            //}

            en.RunObj = this.GetRequestVal("TB_SpecEmp");

            //if (this.RB_DataModel.Checked)
            //{
            //    en.RunObj = this.TB_DataModel.Text;
            //    en.HisFlowRunWay = BP.WF.FlowRunWay.DataModel;
            //}

            //if (this.RB_InsertModel.Checked)
            //{
            //    en.HisFlowRunWay = BP.WF.FlowRunWay.InsertModel;
            //}
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
            dt.Columns.Add("HisListensCount");	//消息收听Count
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

                //消息收听Count
                BP.WF.Template.Listens lns = new BP.WF.Template.Listens(node.NodeID);
                dr["HisListensCount"] = lns.Count;

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
            BP.WF.Flow en = new BP.WF.Flow(this.FK_Flow);
            en.No = this.FK_Flow;
            en.RetrieveFromDBSources();
            return en.ToJson();

            string str = "{";
            //右侧的超链接.
            str += "\"TB_GuideLink\":" + "\"" + en.StartGuideLink + "\"";
            str += ",\"TB_GuideLab\":" + "\"" + en.StartGuideLab + "\"";
            Boolean RB_None = false; Boolean RB_ByHistoryUrl = false; Boolean RB_SelfUrl = false;
            Boolean RB_BySQLOne = false; Boolean RB_FrmList = false; Boolean RB_SubFlow = false; Boolean RB_SubFlowShow = false;
            string TB_ByHistoryUrl = ""; string TB_SelfURL = ""; string TB_BySQLOne1 = "";
            string TB_BySQLOne2 = ""; string TB_SubFlow1 = ""; string TB_SubFlow2 = "";
            switch (en.StartGuideWay)
            {
                case BP.WF.Template.StartGuideWay.None://无
                    RB_None = true;
                    break;
                case BP.WF.Template.StartGuideWay.ByHistoryUrl: //从开始节点Copy数据
                    RB_ByHistoryUrl = true;
                    TB_ByHistoryUrl = en.StartGuidePara1;
                    break;
                case BP.WF.Template.StartGuideWay.BySelfUrl://按自定义的Url
                    RB_SelfUrl = true;
                    TB_SelfURL = en.StartGuidePara1;
                    break;
                case BP.WF.Template.StartGuideWay.BySQLOne: //按照参数.
                    RB_BySQLOne = true;
                    TB_BySQLOne1 = en.StartGuidePara1;
                    TB_BySQLOne2 = en.StartGuidePara2;
                    break;
                case BP.WF.Template.StartGuideWay.ByFrms:
                    RB_FrmList = true;
                    break;
                case BP.WF.Template.StartGuideWay.SubFlowGuide: //子父流程多条模式- 合卷审批.
                    RB_SubFlow = true;
                    TB_SubFlow1 = en.StartGuidePara1;
                    TB_SubFlow2 = en.StartGuidePara2;
                    break;
                default:
                    break;
            }

            BP.WF.Template.FrmNodes fns = new BP.WF.Template.FrmNodes(int.Parse(this.FK_Flow + "01"));
            if (fns.Count > 2)
            {
                RB_SubFlowShow = true;
            }
            str += ",\"RB_None\":" + "\"" + RB_None + "\"";
            str += ",\"RB_ByHistoryUrl\":" + "\"" + RB_ByHistoryUrl + "\"";
            str += ",\"RB_SelfUrl\":" + "\"" + RB_SelfUrl + "\"";
            str += ",\"RB_BySQLOne\":" + "\"" + RB_BySQLOne + "\"";
            str += ",\"RB_FrmList\":" + "\"" + RB_FrmList + "\"";
            str += ",\"RB_SubFlow\":" + "\"" + RB_SubFlow + "\"";
            str += ",\"RB_SubFlowShow\":" + "\"" + RB_SubFlowShow + "\"";
            str += ",\"TB_ByHistoryUrl\":" + "\"" + TB_ByHistoryUrl + "\"";
            str += ",\"TB_SelfURL\":" + "\"" + TB_SelfURL + "\"";
            str += ",\"TB_BySQLOne1\":" + "\"" + TB_BySQLOne1 + "\"";
            str += ",\"TB_BySQLOne2\":" + "\"" + TB_BySQLOne2 + "\"";
            str += ",\"TB_SubFlow1\":" + "\"" + TB_SubFlow1 + "\"";
            str += ",\"TB_SubFlow2\":" + "\"" + TB_SubFlow2 + "\"";
            str += ",\"userId\":" + "\"" + BP.Web.WebUser.SID + "\"";

            return str + "}";

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
                Flow fl = new Flow();
                fl.No = this.FK_Flow;
                fl.Retrieve();

                int val=this.GetRequestValInt("RB_StartGuideWay");

                fl.SetValByKey(BP.WF.Template.FlowAttr.StartGuideWay, val);

                if (fl.StartGuideWay == Template.StartGuideWay.None)
                {

                }


                if (fl.StartGuideWay == Template.StartGuideWay.ByHistoryUrl)
                {
                    fl.StartGuidePara1 = this.GetRequestVal("TB_ByHistoryUrl");
                    fl.StartGuidePara2 = "";
                }

                fl.Update();
                return "sss保存成功.........";
            }
            catch(Exception ex)
            {
                return "err@" + ex.Message;
            }


            //fl.SetValByKey( GenerWorkFlowAtt

            //fl = BP.Sys.PubClass.CopyFromRequestByPost(fl, context.Request) as Flow;
            //fl.Save();  //执行保存

            //BP.WF.Flow en = new BP.WF.Flow(this.FK_Flow);
            //en.No = this.FK_Flow;
            //en.RetrieveFromDBSources();

            //if ("RB_None" == this.GetRequestVal("xz"))
            //{
            //    en.StartGuideWay = BP.WF.Template.StartGuideWay.None;
            //}

            //if ("RB_ByHistoryUrl" == this.GetRequestVal("xz"))
            //{
            //    en.StartGuidePara1 = this.GetRequestVal("TB_ByHistoryUrl");
            //    en.StartGuidePara2 = "";
            //    en.StartGuideWay = BP.WF.Template.StartGuideWay.ByHistoryUrl;
            //}

            //if ("RB_SelfUrl" == this.GetRequestVal("xz"))
            //{
            //    en.StartGuidePara1 = this.GetRequestVal("TB_SelfURL");
            //    en.StartGuidePara2 = "";
            //    en.StartGuideWay = BP.WF.Template.StartGuideWay.BySelfUrl;
            //}

            ////单条模式.
            //if ("RB_BySQLOne" == this.GetRequestVal("xz"))
            //{
            //    en.StartGuidePara1 = this.GetRequestVal("TB_BySQLOne1");  //查询语句.
            //    en.StartGuidePara2 = this.GetRequestVal("TB_BySQLOne2");  //列表语句.
            //    en.StartGuideWay = BP.WF.Template.StartGuideWay.BySQLOne;
            //}

            ////多条-子父流程-合卷审批.
            //if ("RB_SubFlow" == this.GetRequestVal("xz"))
            //{
            //    en.StartGuidePara1 = this.GetRequestVal("TB_SubFlow1");  //查询语句.
            //    en.StartGuidePara2 = this.GetRequestVal("TB_SubFlow2");  //列表语句.
            //    en.StartGuideWay = BP.WF.Template.StartGuideWay.SubFlowGuide;
            //}



            //BP.WF.Template.FrmNodes fns = new BP.WF.Template.FrmNodes(int.Parse(this.FK_Flow + "01"));
            //if (fns.Count >= 2)
            //{
            //    if ("RB_FrmList" == this.GetRequestVal("xz"))
            //        en.StartGuideWay = BP.WF.Template.StartGuideWay.ByFrms;
            //}

            ////右侧的超链接.
            //en.StartGuideLink = this.GetRequestVal("TB_GuideLink");
            //en.StartGuideLab = this.GetRequestVal("TB_GuideLab");

            //en.Update();
            //en.DirectUpdate();
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
                }
                else
                {
                    string str = "{";
                    BP.WF.Template.TruckViewPower en = new BP.WF.Template.TruckViewPower(FK_Flow);
                    if (en.PStarter==true)
                    {
                      str+="\"CB_FQR\":"+"\"true\",";
                    }
                    if (en.PWorker==true)
                    {
                        str+="\"CB_CYR\":"+"\"true\",";
                    }
                    if (en.PCCer==true)
                    {
                        str+="\"CB_CSR\":"+"\"true\",";
                    }

                    if (en.PMyDept==true)
                    {
                        str+="\"CB_BBM\":"+"\"true\",";
                    }
                    if (en.PPMyDept==true)
                    {
                        str+="\"CB_ZSSJ\":"+"\"true\",";
                    }
                    if (en.PPDept==true)
                    {
                        str+="\"CB_SJ\":"+"\"true\",";
                    }

                    if (en.PSameDept==true)
                    {
                        str+="\"CB_PJ\":"+"\"true\",";
                    }
                    if ( en.PSpecDept==true)
                    {
                        str+="\"QY_ZDBM\":"+"\"true\",";
                    }
                    if (string.IsNullOrEmpty(en.PSpecDeptExt))
                    {
                        str+="\"TB_ZDBM\":"+"\""+en.PSpecDeptExt+"\",";
                    }
                    if (en.PSpecSta == true)
                    {
                        str+="\"QY_ZDGW\":"+"\"true\",";
                    }
                    if (string.IsNullOrEmpty(en.PSpecStaExt))
                    {
                        str+="\"TB_ZDGW\":"+"\""+en.PSpecStaExt+"\",";
                    }
                    if (en.PSpecGroup == true)
                    {
                        str+="\"QY_ZDQXZ\":"+"\"true\",";

                    }
                    if (string.IsNullOrEmpty(en.PSpecGroupExt))
                    {
                        str+="\"TB_ZDQXZ\":"+"\""+en.PSpecGroupExt+"\",";
                    }
                    if (en.PSpecEmp == true)
                    {
                        str+="\"QY_ZDQXZ\":"+"\"true\",";
                    }

                    if (string.IsNullOrEmpty(en.PSpecEmpExt))
                    {
                        str += "\"TB_ZDRY\":" + "\"" + en.PSpecEmpExt + "\",";
                    }
                    if (str.Length>2)
                    {
                        str = str.Substring(0,str.Length-1);
                    }
                    str += "}";
                    return str;
                }
            
        }
        #endregion 流程轨迹查看权限
    }
}
