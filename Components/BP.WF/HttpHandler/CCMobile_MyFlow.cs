using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobile_MyFlow : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CCMobile_MyFlow()
        {
            BP.Web.WebUser.SheBei = "Mobile";
        }
        /// <summary>
        /// 获得工作节点
        /// </summary>
        /// <returns></returns>
        public string GenerWorkNode()
        {

            WF_MyFlow en = new WF_MyFlow();
            return en.GenerWorkNode();
        }
        /// <summary>
        /// 绑定多表单中获取节点表单的数据
        /// </summary>
        /// <returns></returns>
        public string GetNoteValue()
        {
            int fk_node = this.FK_Node;
            if (fk_node == 0)
                fk_node = int.Parse(this.FK_Flow + "01");
            Node nd = new Node(fk_node);
            #region  获取节点表单的数据
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();
            wk.ResetDefaultVal();
            if (SystemConfig.IsBSsystem == true)
            {
                // 处理传递过来的参数。
                foreach (string k in HttpContextHelper.RequestQueryStringKeys)
                {
                    if (DataType.IsNullOrEmpty(k) == true)
                        continue;

                    wk.SetValByKey(k, HttpContextHelper.RequestParams(k));
                }

                // 处理传递过来的frm参数。
                foreach (string k in HttpContextHelper.RequestParamKeys)
                {
                    if (DataType.IsNullOrEmpty(k) == true)
                        continue;

                    wk.SetValByKey(k, HttpContextHelper.RequestParams(k));
                }
            }
            #endregion 获取节点表单的数据
            //节点表单字段
            MapData md = new MapData(nd.NodeFrmID);
            MapAttrs attrs = md.MapAttrs;
            DataTable dt = new DataTable();
            dt.TableName = "Node_Note";
            dt.Columns.Add("KeyOfEn", typeof(string));
            dt.Columns.Add("NoteVal", typeof(string));
            string nodeNote = nd.GetParaString("NodeNote");

            foreach (MapAttr attr in attrs)
            {
                if (nodeNote.Contains("," + attr.KeyOfEn + ",") == false)
                    continue;
                string text = "";
                switch (attr.LGType)
                {
                    case FieldTypeS.Normal:  // 输出普通类型字段.
                        if (attr.MyDataType == 1 && (int)attr.UIContralType == DataType.AppString)
                        {

                            if (attrs.Contains(attr.KeyOfEn + "Text") == true)
                                text = wk.GetValRefTextByKey(attr.KeyOfEn);
                            if (DataType.IsNullOrEmpty(text))
                                if (attrs.Contains(attr.KeyOfEn + "T") == true)
                                    text = wk.GetValStrByKey(attr.KeyOfEn + "T");
                        }
                        else
                        {
                            text = wk.GetValStrByKey(attr.KeyOfEn);
                            if (attr.IsRichText == true)
                            {
                                text = text.Replace("white-space: nowrap;", "");
                            }
                        }

                        break;
                    case FieldTypeS.Enum:
                    case FieldTypeS.FK:
                        text = wk.GetValRefTextByKey(attr.KeyOfEn);
                        break;
                    default:
                        break;
                }
                DataRow dr = dt.NewRow();
                dr["KeyOfEn"] = attr.KeyOfEn;
                dr["NoteVal"] = text;
                dt.Rows.Add(dr);

            }

            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得toolbar
        /// </summary>
        /// <returns></returns>
        public string InitToolBar()
        {
            DataSet ds = new DataSet();

            //节点信息
            Node nd = new Node(this.FK_Node);
            ds.Tables.Add(nd.ToDataTableField("WF_Node"));

            //流程信息
            Flow flow = new Flow(this.FK_Flow);
            ds.Tables.Add(flow.ToDataTableField("WF_Flow"));

            //操作按钮信息
            BtnLab btnLab = new BtnLab(this.FK_Node);
            ds.Tables.Add(btnLab.ToDataTableField("WF_BtnLab"));

            #region  加载自定义的button.
            BP.WF.Template.NodeToolbars bars = new NodeToolbars();
            bars.Retrieve(NodeToolbarAttr.FK_Node, this.FK_Node, NodeToolbarAttr.ShowWhere, (int)ShowWhere.Toolbar);
            ds.Tables.Add(bars.ToDataTableField("WF_NodeToolbar"));
            #endregion  //加载自定义的button.

            #region 处理是否是加签，或者是否是会签模式.
            bool isAskForOrHuiQian = false;
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            if (this.FK_Node.ToString().EndsWith("01") == false)
            {
                if (gwf.WFState == WFState.Askfor)
                    isAskForOrHuiQian = true;

                /*判断是否是加签状态，如果是，就判断是否是主持人，如果不是主持人，就让其 isAskFor=true ,屏蔽退回等按钮.*/
                /**说明：针对于组长模式的会签，协作模式的会签加签人仍可以加签*/
                if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianing)
                {
                    //初次打开会签节点时
                    if (DataType.IsNullOrEmpty(gwf.HuiQianZhuChiRen) == true)
                    {
                        if (gwf.TodoEmps.Contains(WebUser.No + ",") == false)
                            isAskForOrHuiQian = true;
                    }

                    //执行会签后的状态
                    if (btnLab.HuiQianRole == HuiQianRole.TeamupGroupLeader && btnLab.HuiQianLeaderRole == 0)
                    {
                        if (gwf.HuiQianZhuChiRen != WebUser.No && gwf.GetParaString("AddLeader").Contains(WebUser.No + ",") == false)
                            isAskForOrHuiQian = true;
                    }
                    else
                    {
                        if (gwf.HuiQianZhuChiRen.Contains(WebUser.No + ",") == false && gwf.GetParaString("AddLeader").Contains(WebUser.No + ",") == false)
                            isAskForOrHuiQian = true;
                    }
                }
                DataTable dt = new DataTable();
                dt.TableName = "HuiQian";
                dt.Columns.Add("isAskForOrHuiQian", typeof(int));
                DataRow dr = dt.NewRow();
                if (isAskForOrHuiQian == true)
                    dr["isAskForOrHuiQian"] = 1;
                else
                    dr["isAskForOrHuiQian"] = 0;
                dt.Rows.Add(dr);

                ds.Tables.Add(dt);
            }
            #endregion 处理是否是加签，或者是否是会签模式，.

            #region 按钮旁的下拉框
            if (nd.CondModel != DirCondModel.ByLineCond)
            {
                if (nd.IsStartNode == true || gwf.TodoEmps.Contains(WebUser.No + ",") == true)
                {
                    /*如果当前不是主持人,如果不是主持人，就不让他显示下拉框了.*/

                    /*如果当前节点，是可以显示下拉框的.*/
                    Nodes nds = nd.HisToNodes;

                    DataTable dtToNDs = new DataTable();
                    dtToNDs.TableName = "ToNodes";
                    dtToNDs.Columns.Add("No", typeof(string));   //节点ID.
                    dtToNDs.Columns.Add("Name", typeof(string)); //到达的节点名称.
                    dtToNDs.Columns.Add("IsSelectEmps", typeof(string)); //是否弹出选择人的对话框？
                    dtToNDs.Columns.Add("IsSelected", typeof(string));  //是否选择？

                    #region 增加到达延续子流程节点。
                    if (nd.SubFlowYanXuNum >= 0)
                    {
                        SubFlowYanXus ygflows = new SubFlowYanXus(this.FK_Node);
                        foreach (SubFlowYanXu item in ygflows)
                        {
                            DataRow dr = dtToNDs.NewRow();
                            dr["No"] = item.SubFlowNo + "01";
                            dr["Name"] = "启动:" + item.SubFlowName;
                            dr["IsSelectEmps"] = "1";
                            dr["IsSelected"] = "0";
                            dtToNDs.Rows.Add(dr);
                        }
                    }
                    #endregion 增加到达延续子流程节点。

                    #region 到达其他节点.
                    //上一次选择的节点.
                    int defalutSelectedNodeID = 0;
                    if (nds.Count > 1)
                    {
                        string mysql = "";
                        // 找出来上次发送选择的节点.
                        if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                            mysql = "SELECT  top 1 NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND ActionType=1 ORDER BY WorkID DESC";
                        else if (SystemConfig.AppCenterDBType == DBType.Oracle)
                            mysql = "SELECT * FROM ( SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND ActionType=1 ORDER BY WorkID DESC ) WHERE ROWNUM =1";
                        else if (SystemConfig.AppCenterDBType == DBType.MySQL)
                            mysql = "SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND ActionType=1 ORDER BY WorkID  DESC limit 1,1";
                        else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                            mysql = "SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND ActionType=1 ORDER BY WorkID  DESC limit 1";

                        //获得上一次发送到的节点.
                        defalutSelectedNodeID = DBAccess.RunSQLReturnValInt(mysql, 0);
                    }

                    #region 为天业集团做一个特殊的判断.
                    if (SystemConfig.CustomerNo == "TianYe" && nd.Name.Contains("董事长") == true)
                    {
                        /*如果是董事长节点, 如果是下一个节点默认的是备案. */
                        foreach (Node item in nds)
                        {
                            if (item.Name.Contains("备案") == true && item.Name.Contains("待") == false)
                            {
                                defalutSelectedNodeID = item.NodeID;
                                break;
                            }
                        }
                    }
                    #endregion 为天业集团做一个特殊的判断.


                    foreach (Node item in nds)
                    {
                        DataRow dr = dtToNDs.NewRow();
                        dr["No"] = item.NodeID;
                        dr["Name"] = item.Name;
                        //if (item.hissel

                        if (item.HisDeliveryWay == DeliveryWay.BySelected)
                            dr["IsSelectEmps"] = "1";
                        else
                            dr["IsSelectEmps"] = "0";  //是不是，可以选择接受人.

                        //设置默认选择的节点.
                        if (defalutSelectedNodeID == item.NodeID)
                            dr["IsSelected"] = "1";
                        else
                            dr["IsSelected"] = "0";

                        dtToNDs.Rows.Add(dr);
                    }
                    #endregion 到达其他节点。


                    //增加一个下拉框, 对方判断是否有这个数据.
                    ds.Tables.Add(dtToNDs);
                }
            }
            #endregion 按钮旁的下拉框

            return BP.Tools.Json.ToJson(ds);
        }
        public string MyFlow_Init()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.MyFlow_Init();
        }
        public string MyFlow_StopFlow()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.MyFlow_StopFlow();
        }
        public string Save()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.Save();
        }
        public string Send()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.Send();
        }
        public string StartGuide_Init()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.StartGuide_Init();
        }
        public string FrmGener_Init()
        {
            WF_CCForm ccfrm = new WF_CCForm();
            return ccfrm.FrmGener_Init();
        }
        public string FrmGener_Save()
        {
            WF_CCForm ccfrm = new WF_CCForm();
            string str = ccfrm.FrmGener_Save();

            // 这里保存的时候，需要保存到草稿,没有看到PC端对应的方法。
            string nodeIDStr = this.FK_Node.ToString();
            if (nodeIDStr.EndsWith("01") == true)
            {
                Flow fl = new Flow(this.FK_Flow);
                if (fl.DraftRole == DraftRole.SaveToDraftList)
                    BP.WF.Dev2Interface.Node_SetDraft(this.FK_Flow, this.WorkID);

                if (fl.DraftRole == DraftRole.SaveToTodolist)
                    BP.WF.Dev2Interface.Node_SetDraft2Todolist(this.FK_Flow, this.WorkID);
            }
            return str;
        }

        public string MyFlowGener_Delete()
        {
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByWriteLog(this.FK_Flow, this.WorkID, Web.WebUser.Name + "用户删除", true);
            return "删除成功...";
        }

        public string AttachmentUpload_Down()
        {
            WF_CCForm ccform = new WF_CCForm();
            return ccform.AttachmentUpload_Down();
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="enName"></param>
        /// <returns></returns>
        public string RetrieveFieldGroup()
        {
            string FrmID = this.GetRequestVal("FrmID");
            GroupFields gfs = new GroupFields();
            QueryObject qo = new QueryObject(gfs);
            qo.AddWhere(GroupFieldAttr.FrmID, FrmID);
            //qo.addAnd();
            //qo.AddWhereIsNull(GroupFieldAttr.CtrlID);
            int num = qo.DoQuery();

            if (num == 0)
            {
                GroupField gf = new GroupField();
                gf.FrmID = FrmID;
                MapData md = new MapData();
                md.No = FrmID;
                if (md.RetrieveFromDBSources() == 0)
                    gf.Lab = "基础信息";
                else
                    gf.Lab = md.Name;
                gf.Idx = 0;
                gf.Insert();
                gfs.AddEntity(gf);
            }
            return gfs.ToJson();
        }
    }
}
