using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.XML;
using BP.En;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_Action : BP.Web.WebPage
    {
        #region 属性.
        public string Event
        {
            get
            {
                return this.Request.QueryString["Event"];
            }
        }
        public string NodeID
        {
            get
            {
                string str = this.Request.QueryString["NodeID"];
                if (string.IsNullOrEmpty(str))
                    return "0";
                return str;
            }
        }
        public string FK_MapData
        {
            get
            {
                string fk_md = this.Request.QueryString["FK_MapData"];
                if (string.IsNullOrWhiteSpace(fk_md))
                {
                    if (this.NodeID == "0")
                        return string.Empty;

                    return "ND" + this.NodeID;
                }

                return fk_md;
            }
        }
        public new string ShowType
        {
            get
            {
                if (this.NodeID != "0")
                    return "Node";


                if (this.NodeID == "0" && string.IsNullOrEmpty(this.FK_Flow) == false && this.FK_Flow.Length >= 3)
                    return "Flow";

                if (this.NodeID == "0" && string.IsNullOrEmpty(this.FK_MapData) == false)
                    return "Frm";

                return "Node";
            }
        }
        public string FK_Flow
        {
            get
            {
                string fk_flow = this.Request.QueryString["FK_Flow"];
                return fk_flow;

                //if(string.IsNullOrWhiteSpace(fk_flow))
                //{
                //    if(!string.IsNullOrWhiteSpace(this.NodeID) && this.NodeID != "0")
                //    {
                //        string node = this.NodeID.Replace("ND", "");
                //        return node.Replace("ND", "").Substring(0, node.Length - 2).PadLeft(3, '0');
                //    }

                //    if (!string.IsNullOrWhiteSpace(this.FK_MapData) && this.FK_MapData != "0")
                //    {
                //        string node = this.FK_MapData.Replace("ND", "");
                //        return node.Replace("ND", "").Substring(0, node.Length - 2).PadLeft(3, '0');
                //    }
                //}
                //return null;
            }
        }
        /// <summary>
        /// 当前事件设置的名称
        /// </summary>
        public string CurrentEvent { get; set; }
        /// <summary>
        /// 当前事件所属事件源名称
        /// </summary>
        public string CurrentEventGroup { get; set; }

        public bool HaveMsg { get; set; }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.DoType == "Del")
            {
                FrmEvent delFE = new FrmEvent();
                delFE.MyPK = this.FK_MapData + "_" + this.Request.QueryString["RefXml"];
                delFE.Delete();
            }

            FrmEvents ndevs = new FrmEvents();

            if (!string.IsNullOrWhiteSpace(this.FK_MapData))
                ndevs.Retrieve(FrmEventAttr.FK_MapData, this.FK_MapData);

            EventLists xmls = new EventLists();
            xmls.RetrieveAll();

            BP.WF.XML.EventSources ess = new EventSources();
            ess.RetrieveAll();

            string myEvent = this.Event;
            BP.WF.XML.EventList myEnentXml = null;

            #region //生成事件列表
            foreach (EventSource item in ess)
            {
                if (item.No != this.ShowType)
                    continue;

                Pub1.Add(string.Format("<div title='{0}' style='padding:10px; overflow:auto' data-options=''>", item.Name));
                Pub1.AddUL("class='navlist'");

                string msg = "";
                foreach (BP.WF.XML.EventList xml in xmls)
                {
                    if (xml.EventType != item.No)
                        continue;

                    msg = "";
                    if (xml.IsHaveMsg == true)
                        msg = "<img src='/WF/Img/Msg.png' />";

                    FrmEvent nde = ndevs.GetEntityByKey(FrmEventAttr.FK_Event, xml.No) as FrmEvent;
                    if (nde == null)
                    {
                        if (myEvent == xml.No)
                        {
                            CurrentEventGroup = item.Name;
                            myEnentXml = xml;
                            Pub1.AddLi(string.Format("<div style='font-weight:bold'><a href='javascript:void(0)'><span class='nav'><img src='/WF/Img/Event.png' border=0/>" + msg + "{0}</span></a></div>{1}", xml.Name, Environment.NewLine));
                        }
                        else
                        {
                            Pub1.AddLi(string.Format("<div><a href='Action.aspx?NodeID={0}&Event={1}&FK_Flow={2}&tk={5}&FK_MapData={6}'><span class='nav'><img src='/WF/Img/Event.png' border=0/>" + msg + "{3}</span></a></div>{4}", NodeID, xml.No, FK_Flow, xml.Name, Environment.NewLine, new Random().NextDouble(), this.FK_MapData));
                        }
                    }
                    else
                    {
                        if (myEvent == xml.No)
                        {
                            CurrentEventGroup = item.Name;
                            myEnentXml = xml;
                            Pub1.AddLi(string.Format("<div style='font-weight:bold'><a href='javascript:void(0)'><span class='nav'><img src='/WF/Img/Event.png' border=0/>" + msg + "{0}</span></a></div>{1}", xml.Name, Environment.NewLine));
                        }
                        else
                        {
                            Pub1.AddLi(string.Format("<div><a href='Action.aspx?NodeID={0}&Event={1}&FK_Flow={2}&MyPK={3}&tk={6}&FK_MapData={7}'><span class='nav'><img src='/WF/Img/Event.png' border=0/>" + msg + "{4}</span></a></div>{5}", NodeID, xml.No, FK_Flow, nde.MyPK, xml.Name, Environment.NewLine, new Random().NextDouble(), this.FK_MapData));
                        }
                    }
                }

                Pub1.AddULEnd();
                Pub1.AddDivEnd();
            }
            #endregion

            if (myEnentXml == null)
            {
                CurrentEvent = "帮助";

                this.Pub2.Add("<div style='width:100%; text-align:left' data-options='noheader:true'>");
                this.Pub2.AddH2("事件是ccbpm与您的应用程序接口");

                if (this.NodeID != "0")
                {
                    this.Pub2.AddFieldSet("节点事件");
                    this.Pub2.AddUL();
                    this.Pub2.AddLi("流程在运动过程中，有许多的事件，比如节点发送前、发送成功后、发送失败后、退回前、退回后、撤销发送前、这小发送后、流程结束前、结束后、删除前删除后。");
                    this.Pub2.AddLi("ccbpm把事件分为流程事件与节点事件，流程属性里定义流程事件，节点属性里定义节点事件。");
                    this.Pub2.AddLi("在这些事件里ccbpm允许调用您编写的业务逻辑，完成与界面交互、与其他系统交互、与其他流程参与人员交互。");
                    this.Pub2.AddLi("按照事件发生的类型，ccbpm把事件分为：节点、表单、流程三类的事件。");
                    this.Pub2.AddLi("在BPMN2.0规范里没有定义节点事件表单事件，这是ccbpm特有的概念与元素。");
                    this.Pub2.AddULEnd();
                    this.Pub2.AddFieldSetEnd();
                }

                if (this.FK_Flow != null && this.NodeID == "0")
                {
                    this.Pub2.AddFieldSet("流程事件");
                    this.Pub2.AddUL();
                    this.Pub2.AddLi("流程在运动过程中，有许多的事件，比如节点发送前、发送成功后、发送失败后、退回前、退回后、撤销发送前、这小发送后、流程结束前、结束后、删除前删除后。");
                    this.Pub2.AddLi("ccbpm把事件分为流程事件与节点事件，流程属性里定义流程事件，节点属性里定义节点事件。");
                    this.Pub2.AddLi("在这些事件里ccbpm允许调用您编写的业务逻辑，完成与界面交互、与其他系统交互、与其他流程参与人员交互。");
                    this.Pub2.AddLi("按照事件发生的类型，ccbpm把事件分为：节点、表单、流程三类的事件。");
                    this.Pub2.AddLi("在BPMN2.0规范里定义了，流程发起事件，流程发起错误事件。在ccbpm里取消了这些概念，取而代之的是开始节点的发送前、发送失败时、发送成功时的事件与之对应。");
                    this.Pub2.AddULEnd();
                    this.Pub2.AddFieldSetEnd();
                }

                if (this.FK_MapData != null && this.FK_MapData != "")
                {
                    this.Pub2.AddFieldSet("表单事件");
                    this.Pub2.AddUL();
                    this.Pub2.AddLi("流程在运动过程中，有许多的事件，比如节点发送前、发送成功后、发送失败后、退回前、退回后、撤销发送前、这小发送后、流程结束前、结束后、删除前删除后。");
                    this.Pub2.AddLi("ccbpm把事件分为流程事件与节点事件，流程属性里定义流程事件，节点属性里定义节点事件。");
                    this.Pub2.AddLi("在这些事件里ccbpm允许调用您编写的业务逻辑，完成与界面交互、与其他系统交互、与其他流程参与人员交互。");
                    this.Pub2.AddLi("按照事件发生的类型，ccbpm把事件分为：节点、表单、流程三类的事件。");
                    this.Pub2.AddLi("在BPMN2.0规范里定义了，流程发起事件，流程发起错误事件。在ccbpm里取消了这些概念，取而代之的是开始节点的发送前、发送失败时、发送成功时的事件与之对应。");
                    this.Pub2.AddULEnd();
                    this.Pub2.AddFieldSetEnd();
                }

                this.Pub2.AddDivEnd();
                return;
            }

            FrmEvent mynde = ndevs.GetEntityByKey(FrmEventAttr.FK_Event, myEvent) as FrmEvent;
            if (mynde == null)
            {
                mynde = new FrmEvent();
                mynde.FK_Event = myEvent;
            }

            this.Title = "设置:事件接口=》" + myEnentXml.Name;
            this.CurrentEvent = myEnentXml.Name;

            //Pub2.Add("<div id='tabMain' class='easyui-tabs' data-options='fit:true'>");

            //Pub2.Add("<div title='事件接口' style='padding:5px'>" + Environment.NewLine);
            Pub2.Add("<iframe id='src1' frameborder='0' src='' style='width:100%;height:100%' scrolling='auto'></iframe>");
            //Pub2.Add("</div>" + Environment.NewLine);

            //if (myEnentXml.IsHaveMsg == true)
            //{
            //    HaveMsg = true;
            //    Pub2.Add("<div title='向当事人推送消息' style='padding:5px'>" + Environment.NewLine);
            //    Pub2.Add("<iframe id='src2' frameborder='0' src='' style='width:100%;height:100%' scrolling='auto'></iframe>");
            //    Pub2.Add("</div>" + Environment.NewLine);

            //    Pub2.Add("<div title='向其他指定的人推送消息' style='padding:5px'>" + Environment.NewLine);
            //    Pub2.Add("<iframe id='src3' frameborder='0' src='' style='width:100%;height:100%' scrolling='auto'></iframe>");
            //    Pub2.Add("</div>" + Environment.NewLine);
            //}

            //Pub2.Add("</div>");
        }
    }
}