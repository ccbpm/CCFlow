using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using BP.DA;
using BP.Web;
using BP.WF.Data;
using BP.WF;
using BP.En;
using BP.WF.Template;
using BP.Sys;


namespace CCFlow.WF.SDKComponents.Base
{
    public partial class SDKBase : System.Web.UI.Page
    {
        #region 参数.
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        public string DoFunc
        {
            get
            {
                return getUTF8ToString("DoFunc");
            }
        }
        public string CFlowNo
        {
            get
            {
                return getUTF8ToString("CFlowNo");
            }
        }
        public string WorkIDs
        {
            get
            {
                return getUTF8ToString("WorkIDs");
            }
        }
        public string FK_Flow
        {
            get
            {
                return getUTF8ToString("FK_Flow");
            }
        }
        public int FK_Node
        {
            get
            {
                string fk_node = getUTF8ToString("FK_Node");
                if (!string.IsNullOrEmpty(fk_node))
                    return Int32.Parse(getUTF8ToString("FK_Node"));
                return 0;
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(getUTF8ToString("WorkID"));
            }
        }
        public Int64 FID
        {
            get
            {
                return Int64.Parse(getUTF8ToString("FID"));
            }
        }
        #endregion 参数.
        protected void Page_Load(object sender, EventArgs e)
        {

            if (BP.Web.WebUser.No == null)
                return;

            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (!string.IsNullOrEmpty(Request["method"]))
                method = Request["method"].ToString();

            switch (method)
            {
                case "getapptoolbar":
                    s_responsetext = GetAppToolBar();
                    break;
                case "getflowformtree"://获取表单树
                    s_responsetext = GetFlowFormTree();
                    break;
                case "checkaccepter"://接受人检查
                    s_responsetext = CheckAccepterOper();
                    break;
                case "sendcase"://执行发送
                    s_responsetext = SendCase();
                    break;
                case "sendcasetonode"://执行发送到指定节点
                    s_responsetext = SendCaseToNode();
                    break;
                case "unsendcase"://撤销发送
                    s_responsetext = UnSendCase();
                    break;
                case ""://保存
                    s_responsetext = Send();
                    break;
                case "delcase"://删除流程
                    s_responsetext = Delcase();
                    break;
                case "signcase"://流程签名
                    s_responsetext = Signcase();
                    break;
                case "endcase"://结束流程
                    s_responsetext = EndCase();
                    break;
                case "readCC":
                    s_responsetext = ReadCC();
                    break;
                    
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";

            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }

        public string ReadCC()
        {
            string str = "";
            try
            {
                int nodeID = int.Parse(Request["FK_Node"]);
                long workID = Int64.Parse(Request["WorkID"]);

                BP.WF.Dev2Interface.Node_CC_SetSta(nodeID, workID, BP.Web.WebUser.No, BP.WF.Template.CCSta.CheckOver);
                str = "true";
            }
            catch (Exception)
            {
                str = "false";

            }
            return str;


        }
        /// <summary>
        /// 保存 
        /// </summary>
        /// <returns></returns>
        public string Send()
        {
            return "";
        }
        /// <summary>
        /// 结束流程
        /// </summary>
        /// <returns></returns>
        public string EndCase()
        {
            string flowId = getUTF8ToString("flowId");
            long workID = Int64.Parse(getUTF8ToString("workId"));
            
            return BP.WF.Dev2Interface.Flow_DoFlowOverByCoercion(flowId,this.FK_Node, workID, this.FID, "");
        }
        /// <summary>
        /// 删除流程
        /// </summary>
        public string Delcase()
        {
            string flowId = getUTF8ToString("flowId");
            int fk_Node = Int32.Parse(getUTF8ToString("nodeId"));
            long workID = Int64.Parse(getUTF8ToString("workId"));
            long fId = Int64.Parse(getUTF8ToString("fId"));
            BP.WF.Node currND = new BP.WF.Node(fk_Node);

            return BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(flowId, workID, false);
        }
        /// <summary>
        /// 签名流程
        /// </summary>
        public string Signcase()
        {
            string flowId = getUTF8ToString("flowId");
            int fk_Node = Int32.Parse(getUTF8ToString("nodeId"));
            long workID = Int64.Parse(getUTF8ToString("workId"));
            long fId = Int64.Parse(getUTF8ToString("fId"));
            if (fId > 0) workID = fId;
            string yj = getUTF8ToString("yj");
            if (yj == null || yj == "") yj = "已阅";

            BP.DA.Paras ps = new BP.DA.Paras();
            ps.Add("FK_Node", fk_Node);

            DataTable Sys_FrmSln = BP.DA.DBAccess.RunSQLReturnTable("select Sys_FrmSln.FK_MapData,Sys_FrmSln.KeyOfEn,Sys_FrmSln.IsSigan,Sys_MapAttr.MyDataType from Sys_FrmSln,Sys_MapAttr where Sys_FrmSln.UIIsEnable=1 and Sys_FrmSln.IsNotNull=1 and Sys_FrmSln.FK_Node=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "FK_Node and Sys_FrmSln.KeyOfEn=Sys_MapAttr.KeyOfEn and Sys_FrmSln.FK_MapData=Sys_MapAttr.FK_MapData", ps);
            Boolean IsSign = false;
            foreach (DataRow DR in Sys_FrmSln.Rows)
            {
                string PTableField = DR["KeyOfEn"].ToString();
                string autotext = "";
                if (DR["IsSigan"].ToString() == "1")
                    autotext = WebUser.No;
                else if (DR["MyDataType"].ToString() == "6")//时间字段
                    autotext = DateTime.Now.ToString("yyyy-MM-dd");
                else if (DR["MyDataType"].ToString() == "1")//意见字段
                {
                    PTableField = PTableField.ToUpper();
                    if (PTableField.EndsWith("YJ") || PTableField.EndsWith("YJ1") || PTableField.EndsWith("YJ2") || PTableField.EndsWith("YJ3") || PTableField.EndsWith("YJ4") || PTableField.EndsWith("YJ5") || PTableField.EndsWith("YJ6") || PTableField.EndsWith("YJ7") || PTableField.EndsWith("YJ8") || PTableField.EndsWith("YJ9"))
                        autotext = yj;
                    else
                        continue;
                }
                else
                    continue;
                string PTable = BP.DA.DBAccess.RunSQLReturnString("select PTable from Sys_MapData where No='" + DR["FK_MapData"].ToString() + "'");
                if (PTable != null)
                {
                    Int32 HavData = BP.DA.DBAccess.RunSQLReturnValInt("select count(oid) from " + PTable + " where oid=" + workID.ToString());
                    if (HavData == 0)
                        BP.DA.DBAccess.RunSQL("insert into " + PTable + "(oid," + PTableField + ") values(" + workID.ToString() + ",'" + autotext + "')");
                    else
                        BP.DA.DBAccess.RunSQL("update " + PTable + " set " + PTableField + "='" + autotext + "' where oid=" + workID.ToString());
                    IsSign = true;
                }
            }
            if (IsSign)
                return "签名完毕";
            else
                return "没有签名可签";
        }
        /// <summary>
        /// 获取工具栏
        /// </summary>
        /// <returns></returns>
        private string GetAppToolBar()
        {
            int fk_Node = Int32.Parse(getUTF8ToString("nodeId"));
            BtnLab btnLab = new BtnLab(fk_Node);
            StringBuilder toolsBar = new StringBuilder();
            toolsBar.Append("{");

            //系统工具
            toolsBar.Append("tools:[");
            //Send,Save,Thread,Return,CC,Shift,Del,EndFLow，RptTrack,HungUp"
            //发送
            if (btnLab.SendEnable)
            {
                toolsBar.Append("{");
                toolsBar.Append("no:'Send',btnlabel:'" + btnLab.SendLab + "'");
                toolsBar.Append("},");
            }
            //保存
            if (btnLab.SaveEnable)
            {
                toolsBar.Append("{");
                toolsBar.Append("no:'Save',btnlabel:'" + btnLab.SaveLab + "'");
                toolsBar.Append("},");
            }
            //子线程
            if (btnLab.ThreadEnable)
            {
                toolsBar.Append("{");
                toolsBar.Append("no:'Thread',btnlabel:'" + btnLab.ThreadLab + "'");
                toolsBar.Append("},");
            }

            //退回
            if (btnLab.ReturnEnable)
            {
                toolsBar.Append("{");
                toolsBar.Append("no:'Return',btnlabel:'" + btnLab.ReturnLab + "'");
                toolsBar.Append("},");
            }
            //抄送
            if (btnLab.CCRole != 0)
            {
                toolsBar.Append("{");
                toolsBar.Append("no:'CC',btnlabel:'" + btnLab.CCLab + "'");
                toolsBar.Append("},");
            }
            //移交
            if (btnLab.ShiftEnable)
            {
                toolsBar.Append("{");
                toolsBar.Append("no:'Shift',btnlabel:'" + btnLab.ShiftLab + "'");
                toolsBar.Append("},");
            }
            //删除 
            if (btnLab.DeleteEnable != 0)
            {
                toolsBar.Append("{");
                toolsBar.Append("no:'Del',btnlabel:'" + btnLab.DeleteLab + "'");
                toolsBar.Append("},");
            }
            //结束 
            if (btnLab.EndFlowEnable)
            {
                toolsBar.Append("{");
                toolsBar.Append("no:'EndFLow',btnlabel:'" + btnLab.EndFlowLab + "'");
                toolsBar.Append("},");
            }
            //打印 
            if (btnLab.PrintDocEnable)
            {
                toolsBar.Append("{");
                toolsBar.Append("no:'Rpt',btnlabel:'" + btnLab.PrintDocLab + "'");
                toolsBar.Append("},");
            }
            //轨迹
            if (btnLab.TrackEnable)
            {
                toolsBar.Append("{");
                toolsBar.Append("no:'Track',btnlabel:'" + btnLab.TrackLab + "'");
                toolsBar.Append("},");
            }
            //挂起
            if (btnLab.HungEnable)
            {
                toolsBar.Append("{");
                toolsBar.Append("no:'HungUp',btnlabel:'" + btnLab.HungLab + "'");
                toolsBar.Append("},");
            }
            if (toolsBar.Length > 8)
                toolsBar.Remove(toolsBar.Length - 1, 1);
            toolsBar.Append("]");
            //扩展工具
            NodeToolbars extToolBars = new NodeToolbars();
            extToolBars.RetrieveByAttr(NodeToolbarAttr.FK_Node, fk_Node);
            toolsBar.Append(",extTools:[");
            if (extToolBars.Count > 0)
            {
                foreach (NodeToolbar item in extToolBars)
                {
                    toolsBar.Append("{OID:'" + item.OID + "',Title:'" + item.Title + "',Target:'" + item.Target + "',Url:'" + item.Url + "'},");
                }
                toolsBar.Remove(toolsBar.Length - 1, 1);
            }
            toolsBar.Append("]");
            toolsBar.Append("}");
            return toolsBar.ToString();
        }

        /// <summary>
        /// 检查节点是否是启用接收人选择器
        /// </summary>
        /// <returns></returns>
        private string CheckAccepterOper()
        {
            int tempToNodeID = 0;
            //获取到当前节点
            Node _HisNode = new Node(this.FK_Node);

            /*如果到达的点为空 */
            Nodes nds = _HisNode.HisToNodes;
            if (nds.Count == 0)
            {
                //当前点是最后的一个节点，不能使用此功能
                return "end";
            }
            else if (nds.Count == 1)
            {
                BP.WF.Node toND = nds[0] as BP.WF.Node;
                tempToNodeID = toND.NodeID;
            }
            else
            {
                foreach (BP.WF.Node mynd in nds)
                {
                    //if (mynd.HisDeliveryWay != DeliveryWay.BySelected)
                    //    continue;

                    GERpt _wk = _HisNode.HisFlow.HisGERpt;
                    _wk.OID = this.WorkID;
                    _wk.Retrieve();
                    _wk.ResetDefaultVal();

                    #region 过滤不能到达的节点.
                    Cond cond = new Cond();
                    int i = cond.Retrieve(CondAttr.FK_Node, _HisNode.NodeID, CondAttr.ToNodeID, mynd.NodeID);
                    if (i == 0)
                        continue; // 没有设置方向条件，就让它跳过去。
                    cond.WorkID = this.WorkID;
                    cond.en = _wk;
                    if (cond.IsPassed == false)
                        continue;
                    #endregion 过滤不能到达的节点.
                    tempToNodeID = mynd.NodeID;
                }
            }
            //不存在下一个节点,检查是否配置了有用户选择节点
            if (tempToNodeID == 0)
            {
                try
                {
                    //检查必填项
                    BP.WF.WorkNode workeNode = new WorkNode(this.WorkID, this.FK_Node);
                    workeNode.CheckFrmIsNotNull();
                }
                catch (Exception ex)
                {
                    return "error:" + ex.Message;
                }
                //按照用户选择计算
                if (_HisNode.CondModel == CondModel.ByUserSelected)
                {
                    return "byuserselected";
                }
                return "notonode";
            }

            //判断到达的节点是否是按接受人选择
            Node toNode = new Node(tempToNodeID);
            if (toNode.HisDeliveryWay == DeliveryWay.BySelected)
            {
                return "byselected";
            }
            return "nodata";
        }

        /// <summary>
        /// 执行发送
        /// </summary>
        /// <returns></returns>
        private string SendCase()
        {
            string resultMsg = "";
            try
            {
                if (Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Flow, this.FK_Node, this.WorkID, WebUser.No) == false)
                {
                    resultMsg = "error|您好：" + BP.Web.WebUser.No + ", " + WebUser.Name + "当前的工作已经被处理，或者您没有执行此工作的权限。";
                }
                SendReturnObjs returnObjs = Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);
                resultMsg = returnObjs.ToMsgOfHtml();
                if (resultMsg.IndexOf("@<a") > 0)
                {
                    string kj = resultMsg.Substring(0, resultMsg.IndexOf("@<a"));
                    resultMsg = resultMsg.Substring(resultMsg.IndexOf("@<a")) + "<br/><br/>" + kj;
                }
                //撤销单据
                int docindex = resultMsg.IndexOf("@<img src='../../Img/FileType/doc.gif' />");
                if (docindex != -1)
                {
                    String kj = resultMsg.Substring(0, docindex);
                    String kp = "";
                    int nextdocindex = resultMsg.IndexOf("@", docindex + 1);
                    if (nextdocindex != -1)
                        kp = resultMsg.Substring(nextdocindex);
                    resultMsg = kj + kp;
                }
                //撤销 撤销本次发送
                int UnSendindex = resultMsg.IndexOf("@<a href='../../MyFlowInfo.aspx?DoType=UnSend");
                if (UnSendindex != -1)
                {
                    String kj = resultMsg.Substring(0, UnSendindex);
                    String kp = "";
                    int nextUnSendindex = resultMsg.IndexOf("@", UnSendindex + 1);
                    if (nextUnSendindex != -1)
                        kp = resultMsg.Substring(nextUnSendindex);
                    resultMsg = kj + "<a href='javascript:UnSend();'><img src='../../Img/Action/UnSend.png' border=0/>撤销本次发送</a>" + kp;
                }

                resultMsg = resultMsg.Replace("指定特定的处理人处理", "指定人员");
                resultMsg = resultMsg.Replace("发手机短信提醒他(们)", "短信通知");
                resultMsg = resultMsg.Replace("撤销本次发送", "撤销案件");
                resultMsg = resultMsg.Replace("新建流程", "发起案件");
                resultMsg = resultMsg.Replace("。", "");
                resultMsg = resultMsg.Replace("，", "");

                resultMsg = resultMsg.Replace("@下一步", "<br/><br/>&nbsp;&nbsp;&nbsp;下一步");
                resultMsg = "success|<br/>" + resultMsg.Replace("@", "&nbsp;&nbsp;&nbsp;");

                #region 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
                /*这里有两种情况
                 * 1，从中间的节点，通过批量处理，也就是说合并审批处理的情况，这种情况子流程需要执行到下一步。
                   2，从流程已经完成，或者正在运行中，也就是说合并审批处理的情况. */
                try
                {
                    //处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
                    BP.WF.Glo.DealBuinessAfterSendWork(this.FK_Flow, this.WorkID, this.DoFunc, WorkIDs);
                }
                catch (Exception ex)
                {
                    resultMsg = "sysError|" + ex.Message.Replace("@", "<br/>");
                    return resultMsg;
                }
                #endregion 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.

            }
            catch (Exception ex)
            {
                resultMsg = "sysError|" + ex.Message.Replace("@", "<br/>");
            }
            return resultMsg;
        }

        /// <summary>
        /// 执行发送到指定节点
        /// </summary>
        /// <returns></returns>
        private string SendCaseToNode()
        {
            int ToNode = Convert.ToInt32(getUTF8ToString("ToNode"));
            string resultMsg = "";
            try
            {
                if (Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Flow, this.FK_Node, this.WorkID, WebUser.No) == false)
                {
                    return resultMsg = "error|您好：" + BP.Web.WebUser.No + ", " + WebUser.Name + "当前的工作已经被处理，或者您没有执行此工作的权限。";
                }
                SendReturnObjs returnObjs = Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, ToNode, null);
                resultMsg = returnObjs.ToMsgOfHtml();
                if (resultMsg.IndexOf("@<a") > 0)
                {
                    string kj = resultMsg.Substring(0, resultMsg.IndexOf("@<a"));
                    resultMsg = resultMsg.Substring(resultMsg.IndexOf("@<a")) + "<br/><br/>" + kj;
                }
                //撤销单据
                int docindex = resultMsg.IndexOf("@<img src='../../Img/FileType/doc.gif' />");
                if (docindex != -1)
                {
                    String kj = resultMsg.Substring(0, docindex);
                    String kp = "";
                    int nextdocindex = resultMsg.IndexOf("@", docindex + 1);
                    if (nextdocindex != -1)
                        kp = resultMsg.Substring(nextdocindex);
                    resultMsg = kj + kp;
                }
                //撤销 撤销本次发送
                int UnSendindex = resultMsg.IndexOf("@<a href='MyFlowInfo.aspx?DoType=UnSend");
                if (UnSendindex != -1)
                {
                    String kj = resultMsg.Substring(0, UnSendindex);
                    String kp = "";
                    int nextUnSendindex = resultMsg.IndexOf("@", UnSendindex + 1);
                    if (nextUnSendindex != -1)
                        kp = resultMsg.Substring(nextUnSendindex);
                    resultMsg = kj + "<a href='javascript:UnSend();'><img src='../../Img/Action/UnSend.png' border=0/>撤销本次发送</a>" + kp;
                }

                resultMsg = resultMsg.Replace("指定特定的处理人处理", "指定人员");
                resultMsg = resultMsg.Replace("发手机短信提醒他(们)", "短信通知");
                resultMsg = resultMsg.Replace("撤销本次发送", "撤销案件");
                resultMsg = resultMsg.Replace("新建流程", "发起案件");
                resultMsg = resultMsg.Replace("。", "");
                resultMsg = resultMsg.Replace("，", "");

                resultMsg = resultMsg.Replace("@下一步", "<br/><br/>&nbsp;&nbsp;&nbsp;下一步");
                resultMsg = "success|<br/>" + resultMsg.Replace("@", "&nbsp;&nbsp;&nbsp;");

                #region 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
                /*这里有两种情况
                 * 1，从中间的节点，通过批量处理，也就是说合并审批处理的情况，这种情况子流程需要执行到下一步。
                   2，从流程已经完成，或者正在运行中，也就是说合并审批处理的情况. */
                try
                {
                    //处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
                    BP.WF.Glo.DealBuinessAfterSendWork(this.FK_Flow, this.WorkID, this.DoFunc, WorkIDs);
                }
                catch (Exception ex)
                {
                    resultMsg = "sysError|" + ex.Message.Replace("@", "<br/>");
                    return resultMsg;
                }
                #endregion 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.

            }
            catch (Exception ex)
            {
                resultMsg = "sysError|" + ex.Message.Replace("@", "<br/>");
            }
            return resultMsg;
        }

        /// <summary>
        /// 撤销发送
        /// </summary>
        /// <returns></returns>
        private string UnSendCase()
        {
            try
            {
                string FK_Flow = getUTF8ToString("FK_Flow");
                string WorkID = getUTF8ToString("WorkID");
                string str1 = BP.WF.Dev2Interface.Flow_DoUnSend(FK_Flow, Int64.Parse(WorkID));
                return "true";
            }
            catch (Exception ex)
            {
                return "{message:'执行撤消失败，失败信息" + ex.Message + "'}";
            }
        }
        /// <summary>
        /// 获取表单树
        /// </summary>
        /// <returns></returns>
        BP.WF.Template.FlowFormTrees appFlowFormTree = new FlowFormTrees();
        private string GetFlowFormTree()
        {
            string flowId = getUTF8ToString("flowId");
            string nodeId = getUTF8ToString("nodeId");

            //add root
            BP.WF.Template.FlowFormTree root = new BP.WF.Template.FlowFormTree();
            root.No = "01";
            root.ParentNo = "0";
            root.Name = "目录";
            root.NodeType = "root";
            appFlowFormTree.Clear();
            appFlowFormTree.AddEntity(root);

            #region 添加表单及文件夹

            //节点表单
            FrmNodes frmNodes = new FrmNodes();
            QueryObject qo = new QueryObject(frmNodes);
            qo.AddWhere(FrmNodeAttr.FK_Node, nodeId);
            qo.addAnd();
            qo.AddWhere(FrmNodeAttr.FK_Flow, flowId);
            qo.addOrderBy(FrmNodeAttr.Idx);
            qo.DoQuery();
            //文件夹
            SysFormTrees formTrees = new SysFormTrees();
            formTrees.RetrieveAll(SysFormTreeAttr.Name);
            //所有表单集合
            MapDatas mds = new MapDatas();
            mds.Retrieve(MapDataAttr.AppType, (int)AppType.Application);
            foreach (FrmNode frmNode in frmNodes)
            {
                foreach (MapData md in mds)
                {
                    if (frmNode.FK_Frm != md.No)
                        continue;

                    foreach (SysFormTree formTree in formTrees)
                    {
                        if (md.FK_FormTree != formTree.No)
                            continue;

                        if (!appFlowFormTree.Contains("No", formTree.No))
                        {
                            BP.WF.Template.FlowFormTree nodeFolder = new BP.WF.Template.FlowFormTree();
                            nodeFolder.No = formTree.No;
                            nodeFolder.ParentNo = root.No;
                            nodeFolder.Name = formTree.Name;
                            nodeFolder.NodeType = "folder";
                            appFlowFormTree.AddEntity(nodeFolder);
                        }
                    }
                    //检查必填项
                    bool IsNotNull = false;
                    FrmFields formFields = new FrmFields();
                    QueryObject obj = new QueryObject(formFields);
                    obj.AddWhere(FrmFieldAttr.FK_Node, nodeId);
                    obj.addAnd();
                    obj.AddWhere(FrmFieldAttr.FK_MapData, md.No);
                    obj.addAnd();
                    obj.AddWhere(FrmFieldAttr.IsNotNull, "1");
                    obj.DoQuery();
                    if (formFields != null && formFields.Count > 0) IsNotNull = true;

                    BP.WF.Template.FlowFormTree nodeForm = new BP.WF.Template.FlowFormTree();
                    nodeForm.No = md.No;
                    nodeForm.ParentNo = md.FK_FormTree;
                    nodeForm.Name = md.Name;
                    nodeForm.NodeType = IsNotNull ? "form|1" : "form|0";
                    appFlowFormTree.AddEntity(nodeForm);
                }
            }
            #endregion
            //扩展工具，显示位置为表单树类型
            NodeToolbars extToolBars = new NodeToolbars();
            QueryObject info = new QueryObject(extToolBars);
            info.AddWhere(NodeToolbarAttr.FK_Node, nodeId);
            info.addAnd();
            info.AddWhere(NodeToolbarAttr.ShowWhere, (int)ShowWhere.Tree);
            info.DoQuery();

            foreach (NodeToolbar item in extToolBars)
            {
                string url = "";
                if (string.IsNullOrEmpty(item.Url))
                    continue;

                url = item.Url;

                BP.WF.Template.FlowFormTree formTree = new BP.WF.Template.FlowFormTree();
                formTree.No = item.OID.ToString();
                formTree.ParentNo = "01";
                formTree.Name = item.Title;
                formTree.NodeType = "tools|0";
                if (!string.IsNullOrEmpty(item.Target) && item.Target.ToUpper() == "_BLANK")
                {
                    formTree.NodeType = "tools|1";
                }

                formTree.Url = url;
                appFlowFormTree.AddEntity(formTree);
            }
            TansEntitiesToGenerTree(appFlowFormTree, root.No, "");
            return appendMenus.ToString();
        }

        //private string GetFlowFormTree_old()
        //{
        //    string flowId = getUTF8ToString("flowId");
        //    string nodeId = getUTF8ToString("nodeId");
        //    //独立表单树父节点
        //    BP.WF.Template.FlowFormTrees flowFormTree = new BP.WF.Template.FlowFormTrees();
        //    QueryObject obj = new QueryObject(flowFormTree);
        //    obj.AddWhere(FlowFormTreeAttr.FK_Flow, flowId);
        //    obj.addAnd();
        //    obj.AddWhere(FlowFormTreeAttr.ParentNo, "01");
        //    obj.addOrderBy(FlowFormTreeAttr.Name);
        //    obj.DoQuery();
        //    //如果为空，则初始数据
        //    if (flowFormTree == null || flowFormTree.Count == 0)
        //    {
        //        InitFlowFormTree(flowId);

        //        //重新获取
        //        flowFormTree = new BP.WF.Template.FlowFormTrees();
        //        obj = new QueryObject(flowFormTree);
        //        obj.AddWhere(FlowFormTreeAttr.FK_Flow, flowId);
        //        obj.addAnd();
        //        obj.AddWhere(FlowFormTreeAttr.ParentNo, "01");
        //        obj.addOrderBy(FlowFormTreeAttr.Idx);
        //        obj.DoQuery();
        //    }
        //    //独立表单
        //    FlowForms flowForms = new FlowForms();
        //    flowForms.RetrieveByAttr(FlowFormAttr.FK_Flow, flowId);
        //    //流程节点表单
        //    NodeForms nodeForms = new NodeForms();
        //    nodeForms.RetrieveByAttr(NodeFormAttr.FK_Node, nodeId);
        //    //如果节点表单不存在
        //    if (nodeForms == null || nodeForms.Count == 0)
        //    {
        //        InitNodeForms(flowId, nodeId);
        //        //重新查询
        //        nodeForms = new NodeForms();
        //        nodeForms.RetrieveByAttr(NodeFormAttr.FK_Node, nodeId);
        //    }
        //    //add root
        //    BP.WF.Template.FlowFormTree root = new BP.WF.Template.FlowFormTree();
        //    root.No = "01";
        //    root.ParentNo = "0";
        //    root.Name = "目录";
        //    root.NodeType = "root";
        //    appFlowFormTree.Clear();
        //    appFlowFormTree.AddEntity(root);
        //    //添加json
        //    foreach (BP.WF.Template.FlowFormTree item in flowFormTree)
        //    {
        //        item.NodeType = "folder";
        //        appFlowFormTree.AddEntity(item);
        //        foreach (FlowForm flowForm in flowForms)
        //        {
        //            if (item.No != flowForm.FK_FlowFormTree)
        //                continue;

        //            foreach (NodeForm nodeForm in nodeForms)
        //            {
        //                if (nodeForm.FK_SysForm == flowForm.FK_SysForm)
        //                {
        //                    SysForm sysForm = new SysForm(nodeForm.FK_SysForm);
        //                    BP.WF.Template.FlowFormTree formTree = new BP.WF.Template.FlowFormTree();
        //                    formTree.No = nodeForm.FK_SysForm;
        //                    formTree.ParentNo = item.No;
        //                    formTree.Name = sysForm.Name;
        //                    formTree.NodeType = "form";
        //                    appFlowFormTree.AddEntity(formTree);
        //                }
        //            }
        //        }
        //        //添加子节点
        //        GetChildTreeNode(flowId, item.No, flowForms, nodeForms);
        //    }
        //    //扩展工具
        //    NodeToolbars extToolBars = new NodeToolbars();
        //    extToolBars.RetrieveByAttr(NodeToolbarAttr.FK_Node, nodeId);

        //    foreach (NodeToolbar item in extToolBars)
        //    {
        //        string url = "";
        //        if (string.IsNullOrEmpty(item.Url))
        //            continue;

        //        url = item.Url;

        //        BP.WF.Template.FlowFormTree formTree = new BP.WF.Template.FlowFormTree();
        //        formTree.No = item.OID.ToString();
        //        formTree.ParentNo = "01";
        //        formTree.Name = item.Title;
        //        formTree.NodeType = "tools";
        //        formTree.Url = url;
        //        appFlowFormTree.AddEntity(formTree);
        //    }
        //    TansEntitiesToGenerTree(appFlowFormTree, root.No, "");
        //    return appendMenus.ToString();
        //}

        //private void GetChildTreeNode(string flowId, string parentNo, FlowForms flowForms, NodeForms nodeForms)
        //{
        //    //独立表单树父节点
        //    BP.WF.Template.FlowFormTrees flowFormTree = new BP.WF.Template.FlowFormTrees();
        //    QueryObject obj = new QueryObject(flowFormTree);
        //    obj.AddWhere(FlowFormTreeAttr.FK_Flow, flowId);
        //    obj.addAnd();
        //    obj.AddWhere(FlowFormTreeAttr.ParentNo, parentNo);
        //    obj.addOrderBy(FlowFormTreeAttr.Name);
        //    obj.DoQuery();

        //    foreach (BP.WF.Template.FlowFormTree item in flowFormTree)
        //    {
        //        item.NodeType = "folder";
        //        appFlowFormTree.AddEntity(item);
        //        foreach (FlowForm flowForm in flowForms)
        //        {
        //            if (item.No != flowForm.FK_FlowFormTree)
        //                continue;

        //            foreach (NodeForm nodeForm in nodeForms)
        //            {
        //                if (nodeForm.FK_SysForm == flowForm.FK_SysForm)
        //                {
        //                    SysForm sysForm = new SysForm(nodeForm.FK_SysForm);
        //                    BP.WF.Template.FlowFormTree formTree = new BP.WF.Template.FlowFormTree();
        //                    formTree.No = nodeForm.FK_SysForm;
        //                    formTree.ParentNo = item.No;
        //                    formTree.Name = sysForm.Name;
        //                    formTree.NodeType = "form";
        //                    appFlowFormTree.AddEntity(formTree);
        //                }
        //            }
        //        }
        //        //添加子节点
        //        GetChildTreeNode(flowId, item.No, flowForms, nodeForms);
        //    }
        //}

        #region 初始节点表单树
        /// <summary>
        /// 初始节点表单树
        /// </summary>
        //private void InitFlowFormTree(string flowId)
        //{
        //    BP.WF.Flow flow = new Flow(flowId);
        //    //获取模版
        //    SysFormTree sysFormTree = new SysFormTree();
        //    sysFormTree.RetrieveByAttr(SysFormTreeAttr.Name, flow.Name);

        //    if (sysFormTree != null)
        //    {
        //        //添加到流程
        //        BP.WF.Template.FlowFormTree flowFormTree = new BP.WF.Template.FlowFormTree();
        //        QueryObject objFlowFormTree = new QueryObject(flowFormTree);
        //        objFlowFormTree.AddWhere("No", "01");
        //        objFlowFormTree.DoQuery();

        //        if (flowFormTree == null || flowFormTree.Name == "")
        //        {
        //            flowFormTree = new BP.WF.Template.FlowFormTree();
        //            flowFormTree.No = "01";
        //            flowFormTree.Name = "目录";
        //            flowFormTree.ParentNo = "0";
        //            flowFormTree.Idx = 0;
        //            flowFormTree.IsDir = true;
        //            flowFormTree.DirectInsert();
        //        }
        //        string subNo = flowFormTree.DoCreateSubNode().No;
        //        flowFormTree = new BP.WF.Template.FlowFormTree(subNo);
        //        flowFormTree.Name = flow.Name;
        //        flowFormTree.FK_Flow = flowId;
        //        flowFormTree.Update();
        //        //添加子级
        //        InitChildNode(flowId, sysFormTree.No, subNo);
        //    }
        //}

        //private void InitNodeForms(string flowId, string nodeId)
        //{
        //    FlowForms flowForm = new FlowForms(flowId);

        //    NodeForm fnodeForm = new NodeForm();
        //    fnodeForm.Delete(NodeFormAttr.FK_Node, nodeId);
        //    foreach (FlowForm item in flowForm)
        //    {
        //        fnodeForm = new NodeForm();
        //        fnodeForm.FK_Node = nodeId;
        //        fnodeForm.FK_SysForm = item.FK_SysForm;
        //        fnodeForm.Insert();
        //    }
        //}

        //private void InitChildNode(string flowId, string parentNo, string wfParetNo)
        //{
        //    //获取模版
        //    SysFormTrees formTrees = new SysFormTrees();
        //    QueryObject objInfo = new QueryObject(formTrees);
        //    objInfo.AddWhere(SysFormTreeAttr.ParentNo, parentNo);
        //    objInfo.addOrderBy(SysFormTreeAttr.Name);
        //    objInfo.DoQuery();

        //    BP.WF.Template.FlowFormTree pFlowFormTree = new BP.WF.Template.FlowFormTree(wfParetNo);

        //    foreach (SysFormTree item in formTrees)
        //    {
        //        string subNo = pFlowFormTree.DoCreateSubNode().No;
        //        BP.WF.Template.FlowFormTree flowFormTree = new BP.WF.Template.FlowFormTree(subNo);
        //        flowFormTree.Name = item.Name;
        //        flowFormTree.FK_Flow = flowId;
        //        flowFormTree.Update();
        //        InitChildNode(flowId, item.No, subNo);
        //    }
        //    //表单树表单
        //    SysForm sysForm = new SysForm();
        //    DataTable dt_Forms = sysForm.RunSQLReturnTable("SELECT No,Name FROM Sys_MapData WHERE FK_FormTree='" + parentNo + "' ORDER BY Name");
        //    //独立表单
        //    foreach (DataRow row in dt_Forms.Rows)
        //    {
        //        FlowForm flowForm = new FlowForm();
        //        flowForm.FK_Flow = flowId;
        //        flowForm.FK_SysForm = row["No"].ToString();
        //        flowForm.FK_FlowFormTree = wfParetNo;
        //        flowForm.Insert();
        //    }
        //}
        #endregion

        /// <summary>
        /// 将实体转为树形
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="rootNo"></param>
        /// <param name="checkIds"></param>
        StringBuilder appendMenus = new StringBuilder();
        StringBuilder appendMenuSb = new StringBuilder();
        public void TansEntitiesToGenerTree(Entities ens, string rootNo, string checkIds)
        {
            EntityMultiTree root = ens.GetEntityByKey(rootNo) as EntityMultiTree;
            if (root == null)
                throw new Exception("@没有找到rootNo=" + rootNo + "的entity.");
            appendMenus.Append("[{");
            appendMenus.Append("\"id\":\"" + rootNo + "\"");
            appendMenus.Append(",\"text\":\"" + root.Name + "\"");

            //attributes
            BP.WF.Template.FlowFormTree formTree = root as BP.WF.Template.FlowFormTree;
            if (formTree != null)
            {
                string url = formTree.Url == null ? "" : formTree.Url;
                url = url.Replace("/", "|");
                appendMenus.Append(",\"attributes\":{\"NodeType\":\"" + formTree.NodeType + "\",\"Url\":\"" + url + "\"}");
            }
            // 增加它的子级.
            appendMenus.Append(",\"children\":");
            AddChildren(root, ens, checkIds);
            appendMenus.Append(appendMenuSb);
            appendMenus.Append("}]");
        }

        public void AddChildren(EntityMultiTree parentEn, Entities ens, string checkIds)
        {
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();

            appendMenuSb.Append("[");
            foreach (EntityMultiTree item in ens)
            {
                if (item.ParentNo != parentEn.No)
                    continue;

                if (checkIds.Contains("," + item.No + ","))
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":true");
                else
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":false");


                //attributes
                BP.WF.Template.FlowFormTree formTree = item as BP.WF.Template.FlowFormTree;
                if (formTree != null)
                {
                    string url = formTree.Url == null ? "" : formTree.Url;
                    string ico = "icon-tree_folder";
                    url = url.Replace("/", "|");
                    appendMenuSb.Append(",\"attributes\":{\"NodeType\":\"" + formTree.NodeType + "\",\"Url\":\"" + url + "\"}");
                    //图标
                    if (formTree.NodeType == "form|0")
                    {
                        ico = "form0";
                    }
                    if (formTree.NodeType == "form|1")
                    {
                        ico = "form1";
                    }
                    if (formTree.NodeType.Contains("tools"))
                    {
                        ico = "icon-4";
                    }
                    appendMenuSb.Append(",iconCls:\"");
                    appendMenuSb.Append(ico);
                    appendMenuSb.Append("\"");
                }
                // 增加它的子级.
                appendMenuSb.Append(",\"children\":");
                AddChildren(item, ens, checkIds);
                appendMenuSb.Append("},");
            }
            if (appendMenuSb.Length > 1)
                appendMenuSb = appendMenuSb.Remove(appendMenuSb.Length - 1, 1);
            appendMenuSb.Append("]");
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();
        }
    }
}