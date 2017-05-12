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

namespace BP.WF.HttpHandler
{
    public class WF_Admin_AttrNode : BP.WF.HttpHandler.WebContralBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_AttrNode(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 考核超时规则.
        public string CHOvertimeRole_Init()
        {
            BP.WF.Node nd = new Node(this.FK_Node);

            return nd.ToJson();
        }
        public string CHOvertimeRole_Save()
        {
            BP.WF.Node nd = new Node(this.FK_Node);
            return nd.ToJson();
        }
        #endregion

        #region 多人处理规则.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string TodolistModel_Init()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            return nd.ToJson();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string TodolistModel_Save()
        {
            BP.WF.Node nd = new BP.WF.Node();
            nd.NodeID = this.FK_Node;
            nd.RetrieveFromDBSources();

            nd.TodolistModel = (TodolistModel)this.GetRequestValInt("RB_TodolistModel");  //考核方式.
            nd.TeamLeaderConfirmRole = (TeamLeaderConfirmRole)this.GetRequestValInt("DDL_TeamLeaderConfirmRole");  //考核方式.
            nd.TeamLeaderConfirmDoc = this.GetRequestVal("TB_TeamLeaderConfirmDoc");

            nd.Update();

            return "保存成功...";
        }
        
        #endregion 多人处理规则.


        #region 考核规则.
        public string CHRole_Init()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            return nd.ToJson();
        }

        public string CHRole_Save()
        {
            BP.WF.Node nd = new BP.WF.Node();
            nd.NodeID = this.FK_Node;
            nd.RetrieveFromDBSources();

            nd.HisCHWay = (CHWay)this.GetRequestValInt("RB_CHWay");  //考核方式.

            nd.TimeLimit = this.GetRequestValInt("TB_TimeLimit");
            nd.WarningDay = this.GetRequestValInt("TB_WarningDay");
            nd.TCent = this.GetRequestValInt("TB_TCent");

            nd.TWay = (BP.DA.TWay)this.GetRequestValInt("DDL_TWay");  //节假日计算方式.

            if (this.GetRequestValInt("CB_IsEval") == 1)
                nd.IsEval = true;
            else
                nd.IsEval = false;

            nd.Update();

            return "保存成功...";
        }
        #endregion 考核规则.


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

        #region 发送后转向处理规则
        public string TurnToDeal_Init()
        {

            BP.WF.Node nd = new BP.WF.Node();
            nd.NodeID = this.FK_Node;
            nd.RetrieveFromDBSources();

            Hashtable ht = new Hashtable();
            ht.Add(NodeAttr.TurnToDeal, (int)nd.HisTurnToDeal);
            ht.Add(NodeAttr.TurnToDealDoc, nd.TurnToDealDoc);

            return BP.Tools.Json.ToJsonEntityModel(ht); 
        }
        #endregion

        #region 发送后转向处理规则Save
        /// <summary>
        /// 前置导航save
        /// </summary>
        /// <returns></returns>
        public string TurnToDeal_Save()
        {
            try
            {
                int nodeID = int.Parse(this.FK_Node.ToString());
                BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nodeID);
                BP.WF.Node nd = new BP.WF.Node(nodeID);

                int val = this.GetRequestValInt("TurnToDeal");

                //遍历页面radiobutton
                if (0 == val)
                {
                    nd.HisTurnToDeal = BP.WF.TurnToDeal.CCFlowMsg;
                }
                else if (1 == val)
                {
                    nd.HisTurnToDeal = BP.WF.TurnToDeal.SpecMsg;
                    nd.TurnToDealDoc = this.GetRequestVal("TB_SpecMsg");
                }
                else
                {
                    nd.HisTurnToDeal = BP.WF.TurnToDeal.SpecUrl;
                    nd.TurnToDealDoc = this.GetRequestVal("TB_SpecURL");
                }
                //执行保存操作
                nd.Update();

                return "保存成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion

        #region 特别控件特别用户权限
        public string SepcFiledsSepcUsers_Init()
        {

            /*string fk_mapdata = this.GetRequestVal("FK_MapData");
            if (string.IsNullOrEmpty(fk_mapdata))
                fk_mapdata = "ND101";

            string fk_node = this.GetRequestVal("FK_Node");
            if (string.IsNullOrEmpty(fk_node))
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
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nodeID);
            BP.WF.Node nd = new BP.WF.Node(nodeID);

            BP.Sys.SysEnums ses = new BP.Sys.SysEnums(BP.WF.Template.NodeAttr.BatchRole);

            return "{\"nd\":" + nd.ToJson() + ",\"ses\":" + ses.ToJson() + ",\"attrs\":" + attrs.ToJson() + "}";
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
            if (this.GetRequestVal("CB_Node") != null)
            {
                sbatchparas = this.GetRequestVal("CB_Node");
            }
            nd.BatchParas = sbatchparas;
            nd.Update();

            return "保存成功.";
        }
        #endregion

        #region 发送阻塞模式
        public string BlockModel_Init()
        {

            BP.WF.Node nd = new BP.WF.Node();
            nd.NodeID = this.FK_Node;
            nd.RetrieveFromDBSources();

            return nd.ToJson();
        }
        public string BlockModel_Save()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            nd.BlockAlert = this.GetRequestVal("TB_Alert"); //提示信息.
            int val = this.GetRequestValInt("RB_BlockModel");
            nd.SetValByKey(BP.WF.Template.NodeAttr.BlockModel, val);
            if (nd.BlockModel == BP.WF.BlockModel.None)
                nd.BlockModel = BP.WF.BlockModel.None;

            if (nd.BlockModel == BP.WF.BlockModel.CurrNodeAll)
            {
                nd.BlockModel = BP.WF.BlockModel.CurrNodeAll;
            }

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

            nd.BlockAlert = this.GetRequestVal("TB_Alert");
            nd.Update();

            return "保存成功.";
        }
        #endregion

        #region 可以撤销的节点
        public string CanCancelNodes_Init()
        {

            BP.WF.Node mynd = new BP.WF.Node();
            mynd.NodeID = this.FK_Node;
            mynd.RetrieveFromDBSources();

            BP.WF.Template.NodeCancels rnds = new BP.WF.Template.NodeCancels();
            rnds.Retrieve(NodeCancelAttr.FK_Node, this.FK_Node);

            BP.WF.Nodes nds = new Nodes();
            nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

            return "{\"mynd\":" + mynd.ToJson() + ",\"rnds\":" + rnds.ToJson() + ",\"nds\":" + nds.ToJson() + "}";
        }
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
                if (cb == null||cb=="")
                    continue;

                NodeCancel nr = new NodeCancel();
                nr.FK_Node = this.FK_Node;
                nr.CancelTo = nd.NodeID;
                nr.Insert();
                i++;
            }
            if (i == 0)
            {
                return "请您选择要撤销的节点。";
            }
            return "设置成功.";
        }
        #endregion


        #region 可以退回的节点
        public string CanReturnNodes_Init()
        {

            BP.WF.Node mynd = new BP.WF.Node();
            mynd.NodeID = this.FK_Node;
            mynd.RetrieveFromDBSources();

            BP.WF.Template.NodeReturns rnds = new BP.WF.Template.NodeReturns();
            rnds.Retrieve(NodeReturnAttr.FK_Node, this.FK_Node);

            BP.WF.Nodes nds = new Nodes();
            nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

            return "{\"mynd\":" + mynd.ToJson() + ",\"rnds\":" + rnds.ToJson() + ",\"nds\":" + nds.ToJson() + "}";
        }
        public string CanReturnNodes_Save()
        {
            BP.WF.Template.NodeReturns rnds = new BP.WF.Template.NodeReturns();
            rnds.Delete(BP.WF.Template.NodeReturnAttr.FK_Node, this.FK_Node);

            BP.WF.Nodes nds = new Nodes();
            nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

            int i = 0;
            foreach (BP.WF.Node nd in nds)
            {
                string cb = this.GetRequestVal("CB_" + nd.NodeID);
                if (cb == null || cb == "")
                    continue;

                NodeReturn nr = new NodeReturn();
                nr.FK_Node = this.FK_Node;
                nr.ReturnTo = nd.NodeID;
                nr.Insert();
                i++;
            }
            if (i == 0)
            {
                return "请您选择要撤销的节点。";
            }
            return "设置成功.";
        }
        #endregion


        #region 消息事件
        public string PushMessage_Init()
        {
            BP.WF.Template.PushMsg enDel = new BP.WF.Template.PushMsg();
            enDel.FK_Node = this.FK_Node;
            enDel.RetrieveFromDBSources();
            return enDel.ToJson();
        }

        public string PushMessage_Delete()
        {
            BP.WF.Template.PushMsg enDel = new BP.WF.Template.PushMsg();
            enDel.MyPK = this.MyPK; ;
            enDel.Delete();
            return "删除成功";
        }

        public string PushMessage_ShowHidden()
        {
            BP.WF.XML.EventLists xmls = new BP.WF.XML.EventLists();
            xmls.RetrieveAll();
            foreach (BP.WF.XML.EventList item in xmls)
            {
                if (item.IsHaveMsg == false)
                    continue;

            }
            return BP.Tools.Json.ToJson(xmls);


        }

        public string PushMessageEntity_Init()
        {
            var fk_node = GetRequestVal("FK_Node");
            BP.WF.Template.PushMsg en = new BP.WF.Template.PushMsg();
            en.MyPK = this.MyPK;
            en.FK_Event = this.FK_Event;
            en.RetrieveFromDBSources();
            return en.ToJson();
        }
        public string PushMessageEntity_Save()
        {
            BP.WF.Template.PushMsg msg = new BP.WF.Template.PushMsg();
            msg.MyPK = this.MyPK;
            msg.RetrieveFromDBSources();
            msg.FK_Event = this.FK_Event;
            msg.FK_Node = this.FK_Node;
            // msg = BP.Sys.PubClass.CopyFromRequestByPost(msg, context.Request) as BP.WF.Template.PushMsg;
            msg.Save();  //执行保存.

            return "保存成功...";
        }
        #endregion
    }
}