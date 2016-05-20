using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using BP.WF;
using BP.DA;
using BP.Web;
using BP.Sys;
using BP.En;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF
{
    /// <summary>
    /// 抄送
    /// </summary>
    public class CCWork
    {
        #region 属性.
        /// <summary>
        /// 工作节点
        /// </summary>
        public WorkNode HisWorkNode = null;
        /// <summary>
        /// 节点
        /// </summary>
        public Node HisNode
        {
            get
            {
                return this.HisWorkNode.HisNode;
            }
        }
        /// <summary>
        /// 报表
        /// </summary>
        public GERpt rptGe
        {
            get
            {
                return this.HisWorkNode.rptGe;
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.HisWorkNode.WorkID;
            }
        }
        #endregion 属性.

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="wn"></param>
        public CCWork(WorkNode wn)
        {
            this.HisWorkNode = wn;
            AutoCC();
            CCByEmps();

        }
        public void AutoCC()
        {
            if (this.HisWorkNode.HisNode.HisCCRole == CCRole.AutoCC
               || this.HisWorkNode.HisNode.HisCCRole == CCRole.HandAndAuto)
            {
            }
            else
            {
                return;
            }

            /*如果是自动抄送*/
            CC cc = this.HisWorkNode.HisNode.HisCC;

            #region 替换节点变量
            cc.CCSQL = cc.CCSQL.Replace("@FK_Node", this.HisNode.NodeID+"");
            cc.CCSQL = cc.CCSQL.Replace("@FK_Flow", this.HisNode.FK_Flow);
            cc.CCSQL = cc.CCSQL.Replace("@OID",this.HisWorkNode.WorkID+"");
            #endregion

            // 执行抄送.
            DataTable dt = cc.GenerCCers(this.HisWorkNode.rptGe);
            if (dt.Rows.Count == 0)
                return;

            string ccMsg = "@消息自动抄送给";
            string basePath = BP.WF.Glo.HostURL;
            string mailTemp = BP.DA.DataType.ReadTextFile2Html(BP.Sys.SystemConfig.PathOfDataUser + "\\EmailTemplete\\CC_" + WebUser.SysLang + ".txt");

            GenerWorkerLists gwls = null;
            if (this.HisWorkNode.town != null)
            {
                //取出抄送集合，如果待办里有此人就取消该人员的抄送.
                gwls = new GenerWorkerLists(this.WorkID, this.HisWorkNode.town.HisNode.NodeID);
            }
            foreach (DataRow dr in dt.Rows)
            {
                string toUserNo = dr[0].ToString();

                //如果待办包含了它.
                if (gwls != null && gwls.Contains(GenerWorkerListAttr.FK_Emp, toUserNo) == true)
                    continue;

                string toUserName = dr[1].ToString();

                //生成标题与内容.
                string ccTitle = cc.CCTitle.Clone() as string;
                ccTitle = BP.WF.Glo.DealExp(ccTitle, this.rptGe, null);

                string ccDoc = cc.CCDoc.Clone() as string;
                ccDoc = BP.WF.Glo.DealExp(ccDoc, this.rptGe, null);

                ccDoc = ccDoc.Replace("@Accepter", toUserNo);
                ccTitle = ccTitle.Replace("@Accepter", toUserNo);

                //抄送信息.
                ccMsg += "(" + toUserNo + " - " + toUserName + ");";
                CCList list = new CCList();
                list.MyPK = this.HisWorkNode.WorkID + "_" + this.HisWorkNode.HisNode.NodeID + "_" + dr[0].ToString();
                list.FK_Flow = this.HisWorkNode.HisNode.FK_Flow;
                list.FlowName = this.HisWorkNode.HisNode.FlowName;
                list.FK_Node = this.HisWorkNode.HisNode.NodeID;
                list.NodeName = this.HisWorkNode.HisNode.Name;
                list.Title = ccTitle;
                list.Doc = ccDoc;
                list.CCTo = dr[0].ToString();
                list.CCToName = dr[1].ToString();
                list.RDT = DataType.CurrentDataTime;
                list.Rec = WebUser.No;
                list.WorkID = this.HisWorkNode.WorkID;
                list.FID = this.HisWorkNode.HisWork.FID;
                list.InEmpWorks = this.HisNode.CCWriteTo == CCWriteTo.CCList ? false : true;    //added by liuxc,2015.7.6
                //写入待办和写入待办与抄送列表,状态不同
                if (this.HisNode.CCWriteTo == CCWriteTo.All || this.HisNode.CCWriteTo == CCWriteTo.Todolist)
                {
                    //如果为写入待办则抄送列表中置为已读，原因：只为不提示有未读抄送。
                    list.HisSta = this.HisNode.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                }
                //结束节点只写入抄送列表
                if (this.HisNode.IsEndNode == true)
                {
                    list.HisSta = CCSta.UnRead;
                    list.InEmpWorks = false;
                }
                try
                {
                    list.Insert();
                }
                catch
                {
                    list.Update();
                }

                if (BP.WF.Glo.IsEnableSysMessage == true)
                {
                    //     //写入消息提示.
                    //     ccMsg += list.CCTo + "(" + dr[1].ToString() + ");";
                    //     BP.WF.Port.WFEmp wfemp = new Port.WFEmp(list.CCTo);
                    //     string sid = list.CCTo + "_" + list.WorkID + "_" + list.FK_Node + "_" + list.RDT;
                    //     string url = basePath + "WF/Do.aspx?DoType=OF&SID=" + sid;
                    //     string urlWap = basePath + "WF/Do.aspx?DoType=OF&SID=" + sid + "&IsWap=1";
                    //     string mytemp = mailTemp.Clone() as string;
                    //     mytemp = string.Format(mytemp, wfemp.Name, WebUser.Name, url, urlWap);
                    //     string title = string.Format("工作抄送:{0}.工作:{1},发送人:{2},需您查阅",
                    //this.HisNode.FlowName, this.HisNode.Name, WebUser.Name);
                    //     BP.WF.Dev2Interface.Port_SendMsg(wfemp.No, title, mytemp, null, BP.Sys.SMSMsgType.CC, list.FK_Flow, list.FK_Node, list.WorkID, list.FID);
                }
            }

            this.HisWorkNode.addMsg(SendReturnMsgFlag.CCMsg, ccMsg);

            //写入日志.
            this.HisWorkNode.AddToTrack(ActionType.CC,WebUser.No,WebUser.Name,this.HisNode.NodeID,this.HisNode.Name, ccMsg, this.HisNode);
        }
        /// <summary>
        /// 按照约定的字段 SysCCEmps 系统人员.
        /// </summary>
        public void CCByEmps()
        {
            if (this.HisNode.HisCCRole != CCRole.BySysCCEmps)
                return;

            CC cc = this.HisNode.HisCC;

            //生成标题与内容.
            string ccTitle = cc.CCTitle.Clone() as string;
            ccTitle = BP.WF.Glo.DealExp(ccTitle, this.rptGe, null);

            string ccDoc = cc.CCDoc.Clone() as string;
            ccDoc = BP.WF.Glo.DealExp(ccDoc, this.rptGe, null);

            //取出抄送人列表
            string ccers = this.rptGe.GetValStrByKey("SysCCEmps");
            if (!string.IsNullOrEmpty(ccers))
            {
                string[] cclist = ccers.Split('|');
                Hashtable ht = new Hashtable();
                foreach (string item in cclist)
                {
                    string[] tmp = item.Split(',');
                    ht.Add(tmp[0], tmp[1]);
                }
                string ccMsg = "@消息自动抄送给";
                string basePath = BP.WF.Glo.HostURL;

                string mailTemp = BP.DA.DataType.ReadTextFile2Html(BP.Sys.SystemConfig.PathOfDataUser + "\\EmailTemplete\\CC_" + WebUser.SysLang + ".txt");
                foreach (DictionaryEntry item in ht)
                {
                    ccDoc = ccDoc.Replace("@Accepter", item.Value.ToString());
                    ccTitle = ccTitle.Replace("@Accepter", item.Value.ToString());

                    //抄送信息.
                    ccMsg += "(" + item.Value.ToString() +" - "+ item.Value.ToString() + ");";
                                        
                    #region 如果是写入抄送列表.
                    CCList list = new CCList();
                    list.MyPK = this.WorkID + "_" + this.HisNode.NodeID + "_" + item.Key.ToString();
                    list.FK_Flow = this.HisNode.FK_Flow;
                    list.FlowName = this.HisNode.FlowName;
                    list.FK_Node = this.HisNode.NodeID;
                    list.NodeName = this.HisNode.Name;
                    list.Title = ccTitle;
                    list.Doc = ccDoc;
                    list.CCTo = item.Key.ToString();
                    list.CCToName = item.Value.ToString();
                    list.RDT = DataType.CurrentDataTime;
                    list.Rec = WebUser.No;
                    list.WorkID = this.WorkID;
                    list.FID = this.HisWorkNode.HisWork.FID;
                    list.InEmpWorks = this.HisNode.CCWriteTo == CCWriteTo.CCList ? false : true;    //added by liuxc,2015.7.6
                    //写入待办和写入待办与抄送列表,状态不同
                    if (this.HisNode.CCWriteTo == CCWriteTo.All || this.HisNode.CCWriteTo == CCWriteTo.Todolist)
                    {
                        //如果为写入待办则抄送列表中置为已读，原因：只为不提示有未读抄送。
                        list.HisSta = this.HisNode.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                    }
                    //如果为结束节点，只写入抄送列表
                    if (this.HisNode.IsEndNode == true)
                    {
                        list.HisSta = CCSta.UnRead;
                        list.InEmpWorks = false;
                    }

                    //执行保存或更新
                    try
                    {
                        list.Insert();
                    }
                    catch
                    {
                        list.CheckPhysicsTable();
                        list.Update();
                    }
                    #endregion 如果要写入抄送

                    if (BP.WF.Glo.IsEnableSysMessage == true)
                    {
                        ccMsg += list.CCTo + "(" + item.Value.ToString() + ");";
                        BP.WF.Port.WFEmp wfemp = new Port.WFEmp(list.CCTo);

                        string sid = list.CCTo + "_" + list.WorkID + "_" + list.FK_Node + "_" + list.RDT;
                        string url = basePath + "WF/Do.aspx?DoType=OF&SID=" + sid;
                        url = url.Replace("//", "/");
                        url = url.Replace("//", "/");

                        string urlWap = basePath + "WF/Do.aspx?DoType=OF&SID=" + sid + "&IsWap=1";
                        urlWap = urlWap.Replace("//", "/");
                        urlWap = urlWap.Replace("//", "/");

                        string mytemp = mailTemp.Clone() as string;
                        mytemp = string.Format(mytemp, wfemp.Name, WebUser.Name, url, urlWap);

                        string title = string.Format("工作抄送:{0}.工作:{1},发送人:{2},需您查阅",
                   this.HisNode.FlowName, this.HisNode.Name, WebUser.Name);

                        BP.WF.Dev2Interface.Port_SendMsg(wfemp.No, title, mytemp, null, BP.WF.SMSMsgType.CC, list.FK_Flow, list.FK_Node, list.WorkID, list.FID);
                    }
                }

                //写入系统消息.
                this.HisWorkNode.addMsg(SendReturnMsgFlag.CCMsg, ccMsg);

                //写入日志.
                this.HisWorkNode.AddToTrack(ActionType.CC, WebUser.No, WebUser.Name, this.HisNode.NodeID, this.HisNode.Name, ccMsg, this.HisNode);

            }
        }
    }
}
